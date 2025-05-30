﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Security.Claims;
using BlogProject.Data.UnitOfWorks;
using BlogProject.Entity.DTOs.Articles;
using BlogProject.Entity.Entities;
using BlogProject.Entity.Enums;
using BlogProject.Service.Extensions;
using BlogProject.Service.Helpers.Images;
using BlogProject.Service.Services.Abstractions;

namespace BlogProject.Service.Services.Concrete
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IImageHelper imageHelper;
        private readonly ClaimsPrincipal _user;

        public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IImageHelper imageHelper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            _user = httpContextAccessor.HttpContext.User;
            this.imageHelper = imageHelper;
        }
        public async Task<ArticleListDto> GetAllByPagingAsync(Guid? categoryId, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;
            var articles = categoryId == null
                ? await unitOfWork.GetRepository<Article>().GetAllAsync(a => !a.IsDeleted, a => a.Category, i => i.Image, u => u.User)
                : await unitOfWork.GetRepository<Article>().GetAllAsync(a => a.CategoryId == categoryId && !a.IsDeleted,
                    a => a.Category, i => i.Image, u => u.User);
            var sortedArticles = isAscending
                ? articles.OrderBy(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                : articles.OrderByDescending(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            var mappedArticles = mapper.Map<IList<ArticleDto>>(sortedArticles);
            return new ArticleListDto
            {
                Articles = mappedArticles,
                CategoryId = categoryId == null ? null : categoryId.Value,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = articles.Count,
                IsAscending = isAscending
            };
        }
        public async Task CreateArticleAsync(ArticleAddDto articleAddDto)
        {
            var userId = _user.GetLoggedInUserId();
            var userEmail = _user.GetLoggedInEmail();

            // Önce resmi kaydet
            var imageUpload = await imageHelper.Upload(articleAddDto.Title, articleAddDto.Photo, ImageType.Post);

            // Resim kaydedildi mi kontrol et
            if (imageUpload == null)
            {
                throw new Exception("Resim yüklenemedi");
            }

            // Log ekleyin
            Console.WriteLine($"Resim kaydedildi: {imageUpload.FullName}");

            // Yeni Image oluştur
            Image image = new(imageUpload.FullName, articleAddDto.Photo.ContentType, userEmail);
            await unitOfWork.GetRepository<Image>().AddAsync(image);

            // Makale oluştur ve Image ID'sini ayarla
            var article = new Article(articleAddDto.Title, articleAddDto.Content, userId, userEmail, articleAddDto.CategoryId, image.Id);
            await unitOfWork.GetRepository<Article>().AddAsync(article);

            // Değişiklikleri kaydet
            await unitOfWork.SaveAsync();
        }
        public async Task<List<ArticleDto>> GetAllArticlesWithCategoryNonDeletedAsync()
        {

            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(x => !x.IsDeleted, x => x.Category);
            var map = mapper.Map<List<ArticleDto>>(articles);

            return map;
        }
        public async Task<ArticleDto> GetArticleWithCategoryNonDeletedAsync(Guid articleId)
        {

            var article = await unitOfWork.GetRepository<Article>().GetAsync(x => !x.IsDeleted && x.Id == articleId, x => x.Category, i => i.Image, u => u.User);
            var map = mapper.Map<ArticleDto>(article);

            return map;
        }
        public async Task<string> UpdateArticleAsync(ArticleUpdateDto articleUpdateDto)
        {
            var userEmail = _user.GetLoggedInEmail();
            var article = await unitOfWork.GetRepository<Article>().GetAsync(x => !x.IsDeleted && x.Id == articleUpdateDto.Id, x => x.Category, i => i.Image);

            if (articleUpdateDto.Photo != null)
            {
                imageHelper.Delete(article.Image.FileName);

                var imageUpload = await imageHelper.Upload(articleUpdateDto.Title, articleUpdateDto.Photo, ImageType.Post);
                Image image = new(imageUpload.FullName, articleUpdateDto.Photo.ContentType, userEmail);
                await unitOfWork.GetRepository<Image>().AddAsync(image);

                article.ImageId = image.Id;

            }

            mapper.Map(articleUpdateDto, article);
            //article.Title = articleUpdateDto.Title;
            //article.Content = articleUpdateDto.Content;
            //article.CategoryId = articleUpdateDto.CategoryId;
            article.ModifiedDate = DateTime.Now;
            article.ModifiedBy = userEmail;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();

            return article.Title;

        }
        public async Task<string> SafeDeleteArticleAsync(Guid articleId)
        {
            var userEmail = _user.GetLoggedInEmail();
            var article = await unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);

            article.IsDeleted = true;
            article.DeletedDate = DateTime.Now;
            article.DeletedBy = userEmail;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();

            return article.Title;
        }

        public async Task<List<ArticleDto>> GetAllArticlesWithCategoryDeletedAsync()
        {
            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(x => x.IsDeleted, x => x.Category);
            var map = mapper.Map<List<ArticleDto>>(articles);

            return map;
        }

        public async Task<string> UndoDeleteArticleAsync(Guid articleId)
        {
            var article = await unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);

            article.IsDeleted = false;
            article.DeletedDate = null;
            article.DeletedBy = null;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();

            return article.Title;
        }

        public async Task<ArticleListDto> SearchAsync(string keyword, int currentPage = 1, int pageSize = 3, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;
            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(
                a => !a.IsDeleted && (a.Title.Contains(keyword) || a.Content.Contains(keyword) || a.Category.Name.Contains(keyword)),
            a => a.Category, i => i.Image, u => u.User);

            var sortedArticles = isAscending
                ? articles.OrderBy(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                : articles.OrderByDescending(a => a.CreatedDate).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            var mappedArticles = mapper.Map<IList<ArticleDto>>(sortedArticles);
            return new ArticleListDto
            {
                Articles = mappedArticles,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = articles.Count,
                IsAscending = isAscending
            };
        }

        public async Task AddArticleVisitorAsync(Guid articleId, string ipAddress, string userAgent)
        {
            // Önce ziyaretçiyi bul veya oluştur
            var visitors = await unitOfWork.GetRepository<Visitor>().GetAllAsync(v => v.IpAddress == ipAddress);
            Visitor visitor;

            if (visitors.Any())
            {
                visitor = visitors.First();
            }
            else
            {
                visitor = new Visitor(ipAddress, userAgent);
                await unitOfWork.GetRepository<Visitor>().AddAsync(visitor);
                await unitOfWork.SaveAsync();
            }

            // Ziyaretçi kaydını al 
            var existingVisit = await unitOfWork.GetRepository<ArticleVisitor>()
                .GetAllAsync(av => av.ArticleId == articleId && av.VisitorId == visitor.Id);

            // Eğer bu ziyaretçi daha önce bu makaleyi ziyaret etmediyse kaydet
            if (!existingVisit.Any())
            {
                var articleVisitor = new ArticleVisitor(articleId, visitor.Id);
                await unitOfWork.GetRepository<ArticleVisitor>().AddAsync(articleVisitor);

                // Ayrıca makale görüntülenme sayısını artır
                var article = await unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);
                article.ViewCount += 1;
                await unitOfWork.GetRepository<Article>().UpdateAsync(article);

                await unitOfWork.SaveAsync();
            }
        }

        public async Task<List<ArticleDto>> GetAllByUserIdAsync(Guid userId)
        {
            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(
                x => !x.IsDeleted && x.UserId == userId,
                x => x.Category, x => x.Image, x => x.User);

            var map = mapper.Map<List<ArticleDto>>(articles);
            return map;
        }


    }
}
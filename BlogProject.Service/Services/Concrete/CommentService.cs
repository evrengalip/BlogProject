// BlogProject.Service/Services/Concrete/CommentService.cs
using AutoMapper;
using BlogProject.Data.UnitOfWorks;
using BlogProject.Entity.DTOs.Comments;
using BlogProject.Entity.Entities;
using BlogProject.Service.Extensions;
using BlogProject.Service.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogProject.Service.Services.Concrete
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            _user = httpContextAccessor.HttpContext.User;
        }

        public async Task<List<CommentDto>> GetAllCommentsByArticleIdAsync(Guid articleId)
        {
            var comments = await unitOfWork.GetRepository<Comment>().GetAllAsync(
                x => !x.IsDeleted && x.ArticleId == articleId,
                x => x.User, x => x.User.Image);

            var mappedComments = mapper.Map<List<CommentDto>>(comments);

            // Populate additional fields
            foreach (var comment in mappedComments)
            {
                var matchingComment = comments.FirstOrDefault(c => c.Id == comment.Id);
                if (matchingComment != null)
                {
                    comment.UserFirstName = matchingComment.User.FirstName;
                    comment.UserLastName = matchingComment.User.LastName;
                    comment.UserImageUrl = matchingComment.User.Image?.FileName ?? "default-user.jpg";
                }
            }


            return mappedComments;
        }

        public async Task CreateCommentAsync(CommentAddDto commentAddDto)
        {
            var userId = _user.GetLoggedInUserId();
            var userEmail = _user.GetLoggedInEmail();

            var comment = new Comment(commentAddDto.Text, commentAddDto.ArticleId, userId);
            comment.CreatedBy = userEmail;

            await unitOfWork.GetRepository<Comment>().AddAsync(comment);
            await unitOfWork.SaveAsync();
        }

        public async Task<string> DeleteCommentAsync(Guid commentId)
        {
            var userEmail = _user.GetLoggedInEmail();
            var comment = await unitOfWork.GetRepository<Comment>().GetByGuidAsync(commentId);

            comment.IsDeleted = true;
            comment.DeletedDate = DateTime.Now;
            comment.DeletedBy = userEmail;

            await unitOfWork.GetRepository<Comment>().UpdateAsync(comment);
            await unitOfWork.SaveAsync();

            return comment.Text;
        }
    }
}
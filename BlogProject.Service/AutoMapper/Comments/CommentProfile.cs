// BlogProject.Service/AutoMapper/Comments/CommentProfile.cs
using AutoMapper;
using BlogProject.Entity.DTOs.Comments;
using BlogProject.Entity.Entities;

namespace BlogProject.Service.AutoMapper.Comments
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<CommentDto, Comment>().ReverseMap();
            CreateMap<CommentAddDto, Comment>().ReverseMap();
        }
    }
}
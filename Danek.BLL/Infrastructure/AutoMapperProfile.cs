using System.Globalization;
using AutoMapper;
using Danek.BLL.DTOs;
using Danek.Web.Models;

namespace Danek.BLL.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            var currentCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            CreateMap<ListBookDto, Book>().ReverseMap();
            CreateMap<GetBookDto, Book>().ReverseMap();
            CreateMap<AddBookDto, Book>().ReverseMap();
            CreateMap<EditBookDto, Book>().ReverseMap();
        }
    }
}

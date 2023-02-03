using System;
using AutoMapper;
using FinanceControl.Application.Services.User.DTO_s;
using FinanceControl.Application.Services.User.Model;

namespace FinanceControl.Application.Services.User.Mapper;

public class UserMapper : Profile
{
    public UserMapper()
    {
        #region [ Request ]



        #endregion

        #region [ Response ]

        CreateMap<UserModel, UserResponse>()
            .ForPath(dest => dest.Thumbnail, src => src.MapFrom(x => Convert.ToBase64String(x.Thumbnail)));

        #endregion
    }
}
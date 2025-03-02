using AutoMapper;
using FinanceControl.Application.Services.Pix.DTO_s.Response;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;

namespace FinanceControl.Application.Services.Pix.Mapper
{
    public class PixMapper : Profile
    {
        public PixMapper()
        {
            #region [ Request ]

            #endregion

            #region [ Response ]

            CreateMap<PixModel, PixResponse>()
                .ForPath(dest => dest.Type, src => src.MapFrom(x => x.Type.GetEnumDescription()));

            #endregion
        }
    }
}

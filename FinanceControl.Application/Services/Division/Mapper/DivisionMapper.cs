using AutoMapper;
using FinanceControl.Application.Services.Division.DTO_s.Response;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Application.Services.Division.Mapper
{
    public class DivisionMapper : Profile
    {
        public DivisionMapper()
        {
            #region [ Request ]

            #endregion

            #region [ Response ]

            CreateMap<DivisionModel, DivisionResponse>();
            CreateMap<LimitModel, LimitResponse>();

            #endregion
        }
    }
}

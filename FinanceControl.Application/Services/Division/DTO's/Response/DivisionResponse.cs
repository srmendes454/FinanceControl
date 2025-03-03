using System;
using System.Collections.Generic;

namespace FinanceControl.Application.Services.Division.DTO_s.Response
{
    public class DivisionResponse
    {
        public Guid DivisionId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double Percent { get; set; }
        public double LimitValue { get; set; }
    }

    public class DivisionAllResponse
    {
        public Guid DivisionId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double Percent { get; set; }
        public double LimitValue { get; set; }
        public List<LimitResponse> Limits { get; set; }
    }

    public class LimitResponse
    {
        public Guid LimitId { get; set; }
        public string Name { get; set; }
        public double Percent { get; set; }
        public double PercentUsed { get; set; }
        public double Value { get; set; }
        public double ValueUsed { get; set; }
    }
}

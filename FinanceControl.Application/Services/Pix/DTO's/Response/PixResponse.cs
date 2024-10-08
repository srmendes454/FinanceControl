using System;

namespace FinanceControl.Application.Services.Pix.DTO_s.Response
{
    public class PixResponse
    {
        public Guid PixId { get; set; }
        public string Name { get; set; }
        public string LinkedAccount { get; set; }
        public int ExpirationDay { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
    }
}

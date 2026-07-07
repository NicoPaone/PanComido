using System.Collections.Generic;

namespace PanComido.Presentacion.DTOs.ErrorResponse
{
    public class ErrorResponseDto
    {
        public string Error { get; set; } = string.Empty;
        public string Code { get; set; } = "error";
        public string TraceId { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Details { get; set; }
    }
}

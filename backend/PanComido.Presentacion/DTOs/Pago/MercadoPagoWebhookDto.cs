namespace PanComido.Presentacion.DTOs.Pago
{
    public class MercadoPagoWebhookDto
    {
        public string? Type { get; set; }
        public WebhookDataDto? Data { get; set; }
    }

    public class WebhookDataDto 
    {
        public string? Id { get; set; }
    }
}

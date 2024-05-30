namespace Mango.Service.ProductAPI.Models.DTOs
{
    public class ResponseDto
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string StatusMessage { get; set; } = "";

    }
}

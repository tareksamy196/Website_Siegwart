namespace Website.Siegwart.BLL.Dtos.User
{
    public class ErrorDto
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
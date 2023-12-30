namespace Authentication.Api.Model.Dto
{
    public class ApiResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}

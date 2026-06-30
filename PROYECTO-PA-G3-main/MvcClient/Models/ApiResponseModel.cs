
namespace Uam.LabHelpDesk.MvcClient.Models
{
    public class ApiResponseModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Result { get; set; }
    }
}
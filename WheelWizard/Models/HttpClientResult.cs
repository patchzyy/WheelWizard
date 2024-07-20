namespace CT_MKWII_WPF.Models;

public class HttpClientResult<T>
{
    public required bool Succeeded { get; init; }
    public required int StatusCode { get; init; }
    public string? StatusMessage { get; init; }
    public T? Content { get; set; }
    
    // Returns the status code group (1xx, 2xx, 3xx, 4xx, 5xx)
    public int StatusCodeGroup => (StatusCode / 100);
    
}


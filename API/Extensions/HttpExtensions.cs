using System.Text.Json;
using API.Helpers;

namespace API.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse repsonse, PaginationHeader header)
    {
        var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        repsonse.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
        repsonse.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
}

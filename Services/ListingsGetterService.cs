using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

public class ListingsGetter
{
    private static readonly HttpClient Client = new(
        new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip |
                                     DecompressionMethods.Deflate
        })
    {
        BaseAddress = new Uri("https://www.binance.com")
    };

    public async Task<AnnouncementResponse?> GetListings()
    {
        try
        {
            var response = await Client.GetAsync(
                "/bapi/apex/v1/public/apex/cms/article/list/query?type=1&pageNo=1&pageSize=1&catalogId=48");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AnnouncementResponse>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public static List<string> ExtractSymbols(string message)
    {
        return new Regex(@"\(([A-Z]+)\)")
            .Matches(message)
            .Select(match => match.Groups[1].Value)
            .ToList();
    }
}

using SteamGameTracker.Services.API.URLs;
using System.Net.Http;
using System.Text.Json;

namespace SteamGameTracker.Services.API
{
    public abstract class ApiServiceBase
    {
        protected readonly ILogger Log;
        protected readonly HttpClient Client;
        protected readonly IUrlFormatter UrlFormatter;

        public ApiServiceBase(HttpClient httpClient, ILogger logger, IUrlFormatter urlFormatter)
        {
            Client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Log = logger ?? throw new ArgumentNullException(nameof(logger));
            UrlFormatter = urlFormatter ?? throw new ArgumentNullException(nameof(urlFormatter));
        }

        public async Task<TDto?> GetDtoAsync<TDto>(
            string url,
            CancellationToken cancellationToken = default)
            where TDto : class
        {
            try
            {
                Log.LogInformation("Sending GET request to URL '{Url}'", url);

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                // Configure request to timeout after a reasonable period
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    timeoutCts.Token,
                    cancellationToken
                );

                var response = await Client.SendAsync(request, linkedCts.Token);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<TDto>(linkedCts.Token);
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "HTTP request failed for URL'{Url}' with Http status code '{httpStatusCode}'", url, ex.StatusCode);
            }
            catch (JsonException ex)
            {
                Log.LogError(ex, "JSON deserialization error for URL '{Url}' and DTO '{TDto}'", url, typeof(TDto));
            }
            catch (OperationCanceledException ex)
            {
                Log.LogWarning(ex, "Request to URL '{Url}' was cancelled or timed out", url);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Fatal error trying to get from URL '{Url}' and DTO '{TDto}'", url, typeof(TDto));
            }

            return null;
        }
    }
}

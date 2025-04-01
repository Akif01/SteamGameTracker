using System.Text;
using Microsoft.Extensions.Logging;
using SteamGameTracker.Services.API;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Utils
{
    public class UrlFormatter : IUrlFormatter
    {
        private readonly ILogger<UrlFormatter> _logger;

        public UrlFormatter(ILogger<UrlFormatter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GetFormattedUrl(FormattableUrlBase formattableUrl)
        {
            var rawUrl = formattableUrl.ProvideUrlWithPlaceholders();
            var placeholderValueDict = formattableUrl.ProvidePlaceHolderValueDict();

            if (placeholderValueDict.Count == 0)
            {
                _logger.LogInformation($"URL '{rawUrl}' was not formatted, since no placeholders were provided.");
                return rawUrl;
            }

            var formattedUrlBuilder = new StringBuilder(rawUrl);

            _logger.LogInformation($"Formatting of URL '{rawUrl}' has started.\n" +
                $"Placeholders: '{string.Join(",", placeholderValueDict.Keys)}'.\n" +
                $"Values: '{string.Join(",", placeholderValueDict.Values)}'");

            foreach (var kvp in placeholderValueDict)
            {
                var key = $"{{{kvp.Key}}}";
                formattedUrlBuilder.Replace(key, kvp.Value.ToString());
            }

            string formattedUrl = formattedUrlBuilder.ToString();
            _logger.LogInformation($"Formatted URL of '{rawUrl}' is '{formattedUrl}'.");

            return formattedUrl;
        }
    }
}

using System.Text;

namespace SteamGameTracker.Services.API.URLs
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
                _logger.LogInformation("URL '{RawURL}' was not formatted, since no placeholders were provided.", rawUrl);
                return rawUrl;
            }

            var formattedUrlBuilder = new StringBuilder(rawUrl);

            foreach (var kvp in placeholderValueDict)
            {
                var key = $"{{{kvp.Key}}}";
                formattedUrlBuilder.Replace(key, kvp.Value.ToString());
            }

            string formattedUrl = formattedUrlBuilder.ToString();
            _logger.LogInformation("Formatted URL of '{RawURL}' is '{FormattedUrl}'.", rawUrl, formattedUrl);

            return formattedUrl;
        }
    }
}

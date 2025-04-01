namespace SteamGameTracker.Services.API.URLs
{
    public abstract class FormattableUrlBase
    {
        public abstract string ProvideUrlWithPlaceholders();
        public abstract Dictionary<string, IConvertible> ProvidePlaceHolderValueDict();
    }
}

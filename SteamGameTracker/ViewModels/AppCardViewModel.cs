namespace SteamGameTracker.ViewModels
{
    public record AppCardViewModel(int Id, 
        string Name, 
        string LargeCapsuleImageURL, 
        int PriceInCents, 
        string Currency, 
        string ShortDescription, 
        List<string> Genres, 
        int NumberOfCurrentPlayers);
}

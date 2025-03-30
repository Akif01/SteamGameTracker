using SteamGameTracker.DataTransferObjects;

namespace SteamGameTracker.Models
{
    public class NumberOfCurrentPlayersModel : ModelBase<NumberOfCurrentPlayersDTO>
    {
        public int NumberOfCurrentPlayers { get; set; }
        public bool IsSuccess { get; set; }

        public NumberOfCurrentPlayersModel(NumberOfCurrentPlayersDTO dto) : base(dto)
        {
        }

        protected override void PopulateFromDTO(NumberOfCurrentPlayersDTO dto)
        {
            IsSuccess = dto.Response.Result == 1;
            NumberOfCurrentPlayers = dto.Response.PlayerCount;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

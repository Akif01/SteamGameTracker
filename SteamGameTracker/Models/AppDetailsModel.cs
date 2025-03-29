using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.DataTransferObjects.SteamApi.Models;

namespace SteamGameTracker.Models
{
    public class AppDetailsModel : ModelBase<SuccessDTO>
    {
        public int Id { get; private set; }
        public bool Success { get; private set; }
        public bool IsGame { get; private set; }
        public string Name { get; private set; }
        public int RequiredAge { get; private set; }

        public AppDetailsModel(SuccessDTO dto) : base(dto)
        {
        }

        protected override void PopulateFromDTO(SuccessDTO dto)
        {
            Id = dto.Data.SteamAppId;
            Success = dto.Success;
            IsGame = dto.Data.Type == "game";
            Name = dto.Data.Name;
            RequiredAge = dto.Data.RequiredAge;
        }
    }
}

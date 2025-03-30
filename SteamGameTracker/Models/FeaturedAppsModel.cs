using SteamGameTracker.DataTransferObjects;

namespace SteamGameTracker.Models
{
    public class FeaturedAppsModel : ModelBase<FeaturedAppsDTO>
    {
        public FeaturedAppsModel(FeaturedAppsDTO dto) : base(dto)
        {
        }

        public override bool IsValid()
        {
            return true;
        }

        protected override void PopulateFromDTO(FeaturedAppsDTO dto)
        {
            
        }
    }
}

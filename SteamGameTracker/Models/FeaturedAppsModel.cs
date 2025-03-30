using SteamGameTracker.DataTransferObjects;

namespace SteamGameTracker.Models
{
    public class FeaturedAppsModel : ModelBase<FeaturedAppsDTO>
    {
        public List<FeaturedItemModel> WindowsFeaturedApps { get; private set; } = new();

        public FeaturedAppsModel(FeaturedAppsDTO dto) : base(dto)
        {
        }

        public override bool IsValid()
        {
            return true;
        }

        protected override void PopulateFromDTO(FeaturedAppsDTO dto)
        {
            foreach (var featueredWindowsItemDTO in dto.FeaturedWindows)
            {
                var model = new FeaturedItemModel(featueredWindowsItemDTO);

                WindowsFeaturedApps.Add(model);
            }
        }
    }
}

using SteamGameTracker.DataTransferObjects;

namespace SteamGameTracker.Models
{
    public class AppsModel : ModelBase<AppsDTO>
    {
        public List<AppModel> Apps { get; private set; } = [];

        public AppsModel(AppsDTO dto) : base(dto)
        {
        }

        protected override void PopulateFromDTO(AppsDTO dto)
        {
            foreach (var app in dto.Apps) 
            {
                var appModel = new AppModel(app);
                Apps.Add(appModel);
            }
        }

        public override bool IsValid() => true;
    }
}

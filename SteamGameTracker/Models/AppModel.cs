using SteamGameTracker.DataTransferObjects;

namespace SteamGameTracker.Models
{
    public class AppModel : ModelBase<AppDTO>
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public AppModel(AppDTO dto) : base(dto)
        {
        }

        protected override void PopulateFromDTO(AppDTO dto)
        {
            Id = dto.AppId;
            Name = dto.Name;
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}

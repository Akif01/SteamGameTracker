using SteamGameTracker.DataTransferObjects;

namespace SteamGameTracker.Models
{
    public class FeaturedItemModel : ModelBase<FeaturedItemDTO>
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public FeaturedItemModel(FeaturedItemDTO dto) : base(dto)
        {
        }

        protected override void PopulateFromDTO(FeaturedItemDTO dto)
        {
            Id = dto.Id;
            Name = dto.Name;
        }

        public override bool IsValid()
        {
            return Id > 0 &&
                !string.IsNullOrEmpty(Name);
        }
    }
}

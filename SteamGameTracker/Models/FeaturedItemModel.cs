using SteamGameTracker.DataTransferObjects;

namespace SteamGameTracker.Models
{
    public class FeaturedItemModel : ModelBase<FeaturedItemDTO>
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int FinalPriceCents { get; private set; }
        public string Currency { get; private set; }
        public string LargeCapsuleImageURL { get; private set; }

        public FeaturedItemModel(FeaturedItemDTO dto) : base(dto)
        {
        }

        protected override void PopulateFromDTO(FeaturedItemDTO dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            FinalPriceCents = dto.FinalPrice;
            Currency = dto.Currency;
            LargeCapsuleImageURL = dto.LargeCapsuleImage;
        }

        public override bool IsValid()
        {
            return Id > 0 &&
                !string.IsNullOrEmpty(Name);
        }
    }
}

namespace SteamGameTracker.Models
{

    public abstract class ModelBase<TDto> where TDto : class
    {
        protected ModelBase(TDto dto)
        {
            PopulateFromDTO(dto);
            if (!IsValid())
                throw new ArgumentException(nameof(dto));
        }

        protected abstract void PopulateFromDTO(TDto dto);
        public abstract bool IsValid();
    }
}

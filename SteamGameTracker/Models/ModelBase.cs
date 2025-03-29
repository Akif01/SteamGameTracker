namespace SteamGameTracker.Models
{

    public abstract class ModelBase<TDto> where TDto : class
    {
        protected ModelBase(TDto dto)
        {
            PopulateFromDTO(dto);
        }

        protected abstract void PopulateFromDTO(TDto dto);
    }
}

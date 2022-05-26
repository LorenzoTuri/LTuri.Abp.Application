namespace LTuri.Abp.Application.EntityTypes
{
    public interface ITranslatableEntity<TTranslationEntityType> 
        where TTranslationEntityType : ITranslationEntity
    {
        List<TTranslationEntityType> Translations { get; set; }
    }
}

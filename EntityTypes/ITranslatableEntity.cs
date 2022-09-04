namespace LTuri.Abp.Application.EntityTypes
{
    /// <summary>
    /// TODO: docs
    /// </summary>
    /// <typeparam name="TTranslationEntityType"></typeparam>
    public interface ITranslatableEntity<TTranslationEntityType> 
        where TTranslationEntityType : ITranslationEntity
    {
        List<TTranslationEntityType> Translations { get; set; }
    }
}

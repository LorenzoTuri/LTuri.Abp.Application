using System.Runtime.Serialization;
using Volo.Abp;

namespace LTuri.Abp.Application.Exceptions
{
    [Serializable]
    public class FilterValueTypeConversionException : BusinessException, IUserFriendlyException
    {
        public FilterValueTypeConversionException(string value, string propertyType) : 
            base("400", $"Cannot convert '{value}' to type '{propertyType}'") { }
        protected FilterValueTypeConversionException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }
    }
}

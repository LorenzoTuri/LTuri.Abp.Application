using System.Runtime.Serialization;
using Volo.Abp;

namespace LTuri.Abp.Application.Exceptions
{
    [Serializable]
    public class FilterByWrongParameterException : BusinessException, IUserFriendlyException
    {
        public FilterByWrongParameterException(string field, string type) : 
            base("400", $"Parameter '{field}' not found in '{type}'")
        { }
        public FilterByWrongParameterException(string field, string type, Exception inner) : 
            base("400", $"Parameter '{field}' not found in '{type}'", null, inner)
        { }

        protected FilterByWrongParameterException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }
    }
}

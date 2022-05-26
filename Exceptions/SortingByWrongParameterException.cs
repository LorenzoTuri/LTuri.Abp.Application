using System.Runtime.Serialization;
using Volo.Abp;

namespace LTuri.Abp.Application.Exceptions
{
    [Serializable]
    public class SortingByWrongParameterException : BusinessException, IUserFriendlyException
    {
        public SortingByWrongParameterException(string by, string type) : 
            base("400", $"Parameter '{by}' not found in '{type}'")
        { }
        public SortingByWrongParameterException(string by, string type, Exception inner) :
            base("400", $"Parameter '{by}' not found in '{type}'", null, inner)
        { }

        protected SortingByWrongParameterException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }
    }
}

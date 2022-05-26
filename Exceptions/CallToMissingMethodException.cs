using System.Runtime.Serialization;
using Volo.Abp;

namespace LTuri.Abp.Application.Exceptions
{
    [Serializable]
    public class CallToMissingMethodException : BusinessException, IUserFriendlyException
    {

        public CallToMissingMethodException(string method, string propertyName) : 
            base("400", $"Can't use {method} on {propertyName}") { }

        protected CallToMissingMethodException(SerializationInfo info, StreamingContext context) : 
            base(info, context) {}
    }
}

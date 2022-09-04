using System.Runtime.Serialization;
using Volo.Abp;

namespace LTuri.Abp.Application.Exceptions
{
    [Serializable]
    public class ServiceNotFoundException : BusinessException, IUserFriendlyException
    {
        public ServiceNotFoundException(string serviceName, string serviceDependency) : 
            base("500", $"Cannot add {serviceName}: {serviceDependency} not found.")
        { }
        
        protected ServiceNotFoundException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }
    }
}

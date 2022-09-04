using System.Runtime.Serialization;
using Volo.Abp;

namespace LTuri.Abp.Application.Exceptions
{
    [Serializable]
    public class AntlrParsingException : BusinessException, IUserFriendlyException
    {

        public AntlrParsingException(string where) :
            base("400", "Antlr parsing exception", where)
        { }

        protected AntlrParsingException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        { }
    }
}

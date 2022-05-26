using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using LTuri.Abp.Application.Exceptions;

namespace LTuri.Abp.Application.Antlr
{
    public class ThrowingErrorListener : BaseErrorListener
    {
        public override void SyntaxError(
            [NotNull] IRecognizer recognizer,
            [Nullable] IToken offendingSymbol,
            int line,
            int charPositionInLine,
            [NotNull] string msg,
            [Nullable] RecognitionException e
        )
        {
            throw new AntlrParsingException("line " + line + ":" + charPositionInLine + " " + msg);
        }
    }
}

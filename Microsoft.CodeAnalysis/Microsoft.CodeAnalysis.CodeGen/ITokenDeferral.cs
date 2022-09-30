using System.Collections.Immutable;

using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.CodeGen
{
    public interface ITokenDeferral
    {
        ArrayMethods ArrayMethods { get; }

        uint GetFakeStringTokenForIL(string value);

        uint GetFakeSymbolTokenForIL(IReference value, SyntaxNode syntaxNode, DiagnosticBag diagnostics);

        uint GetFakeSymbolTokenForIL(ISignature value, SyntaxNode syntaxNode, DiagnosticBag diagnostics);

        uint GetSourceDocumentIndexForIL(DebugSourceDocument document);

        IFieldReference GetFieldForData(ImmutableArray<byte> data, SyntaxNode syntaxNode, DiagnosticBag diagnostics);

        IMethodReference GetInitArrayHelper();

        string GetStringFromToken(uint token);

        object GetReferenceFromToken(uint token);
    }
}

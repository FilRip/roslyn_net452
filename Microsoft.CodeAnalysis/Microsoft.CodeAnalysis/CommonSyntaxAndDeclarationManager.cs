using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis
{
    public abstract class CommonSyntaxAndDeclarationManager
    {
        public readonly ImmutableArray<SyntaxTree> ExternalSyntaxTrees;

        public readonly string ScriptClassName;

        public readonly SourceReferenceResolver Resolver;

        public readonly CommonMessageProvider MessageProvider;

        public readonly bool IsSubmission;

        public CommonSyntaxAndDeclarationManager(ImmutableArray<SyntaxTree> externalSyntaxTrees, string scriptClassName, SourceReferenceResolver resolver, CommonMessageProvider messageProvider, bool isSubmission)
        {
            ExternalSyntaxTrees = externalSyntaxTrees;
            ScriptClassName = scriptClassName ?? "";
            Resolver = resolver;
            MessageProvider = messageProvider;
            IsSubmission = isSubmission;
        }
    }
}

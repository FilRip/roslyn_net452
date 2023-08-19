// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

        protected CommonSyntaxAndDeclarationManager(
            ImmutableArray<SyntaxTree> externalSyntaxTrees,
            string scriptClassName,
            SourceReferenceResolver resolver,
            CommonMessageProvider messageProvider,
            bool isSubmission)
        {
            this.ExternalSyntaxTrees = externalSyntaxTrees;
            this.ScriptClassName = scriptClassName ?? "";
            this.Resolver = resolver;
            this.MessageProvider = messageProvider;
            this.IsSubmission = isSubmission;
        }
    }
}

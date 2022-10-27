// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    internal partial class SyntaxToken
    {
        internal class MissingTokenWithTrivia : SyntaxTokenWithTrivia
        {
            internal MissingTokenWithTrivia(SyntaxKind kind, GreenNode leading, GreenNode trailing)
                : base(kind, leading, trailing)
            {
                this.flags &= ~NodeFlags.IsNotMissing;
            }

            internal MissingTokenWithTrivia(SyntaxKind kind, GreenNode leading, GreenNode trailing, DiagnosticInfo[] diagnostics, SyntaxAnnotation[] annotations)
                : base(kind, leading, trailing, diagnostics, annotations)
            {
                this.flags &= ~NodeFlags.IsNotMissing;
            }

            internal MissingTokenWithTrivia(ObjectReader reader)
                : base(reader)
            {
                this.flags &= ~NodeFlags.IsNotMissing;
            }

            static MissingTokenWithTrivia()
            {
                ObjectBinder.RegisterTypeReader(typeof(MissingTokenWithTrivia), r => new MissingTokenWithTrivia(r));
            }

            public override string Text
            {
                get { return string.Empty; }
            }

            public override object Value
            {
                get
                {
                    return this.Kind switch
                    {
                        SyntaxKind.IdentifierToken => string.Empty,
                        _ => null,
                    };
                }
            }

            public override SyntaxToken TokenWithLeadingTrivia(GreenNode trivia)
            {
                return new MissingTokenWithTrivia(this.Kind, trivia, this.TrailingField, this.GetDiagnostics(), this.GetAnnotations());
            }

            public override SyntaxToken TokenWithTrailingTrivia(GreenNode trivia)
            {
                return new MissingTokenWithTrivia(this.Kind, this.LeadingField, trivia, this.GetDiagnostics(), this.GetAnnotations());
            }

            public override GreenNode SetDiagnostics(DiagnosticInfo[] diagnostics)
            {
                return new MissingTokenWithTrivia(this.Kind, this.LeadingField, this.TrailingField, diagnostics, this.GetAnnotations());
            }

            public override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
            {
                return new MissingTokenWithTrivia(this.Kind, this.LeadingField, this.TrailingField, this.GetDiagnostics(), annotations);
            }
        }
    }
}

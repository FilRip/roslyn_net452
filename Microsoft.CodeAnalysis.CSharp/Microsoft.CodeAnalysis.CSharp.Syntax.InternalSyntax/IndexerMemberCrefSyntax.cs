using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class IndexerMemberCrefSyntax : MemberCrefSyntax
    {
        internal readonly SyntaxToken thisKeyword;

        internal readonly CrefBracketedParameterListSyntax? parameters;

        public SyntaxToken ThisKeyword => thisKeyword;

        public CrefBracketedParameterListSyntax? Parameters => parameters;

        public IndexerMemberCrefSyntax(SyntaxKind kind, SyntaxToken thisKeyword, CrefBracketedParameterListSyntax? parameters, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(thisKeyword);
            this.thisKeyword = thisKeyword;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
        }

        public IndexerMemberCrefSyntax(SyntaxKind kind, SyntaxToken thisKeyword, CrefBracketedParameterListSyntax? parameters, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(thisKeyword);
            this.thisKeyword = thisKeyword;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
        }

        public IndexerMemberCrefSyntax(SyntaxKind kind, SyntaxToken thisKeyword, CrefBracketedParameterListSyntax? parameters)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(thisKeyword);
            this.thisKeyword = thisKeyword;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => thisKeyword,
                1 => parameters,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.IndexerMemberCrefSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitIndexerMemberCref(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitIndexerMemberCref(this);
        }

        public IndexerMemberCrefSyntax Update(SyntaxToken thisKeyword, CrefBracketedParameterListSyntax parameters)
        {
            if (thisKeyword != ThisKeyword || parameters != Parameters)
            {
                IndexerMemberCrefSyntax indexerMemberCrefSyntax = SyntaxFactory.IndexerMemberCref(thisKeyword, parameters);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    indexerMemberCrefSyntax = indexerMemberCrefSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    indexerMemberCrefSyntax = indexerMemberCrefSyntax.WithAnnotationsGreen(annotations);
                }
                return indexerMemberCrefSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new IndexerMemberCrefSyntax(base.Kind, thisKeyword, parameters, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new IndexerMemberCrefSyntax(base.Kind, thisKeyword, parameters, GetDiagnostics(), annotations);
        }

        public IndexerMemberCrefSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            thisKeyword = node;
            CrefBracketedParameterListSyntax crefBracketedParameterListSyntax = (CrefBracketedParameterListSyntax)reader.ReadValue();
            if (crefBracketedParameterListSyntax != null)
            {
                AdjustFlagsAndWidth(crefBracketedParameterListSyntax);
                parameters = crefBracketedParameterListSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(thisKeyword);
            writer.WriteValue(parameters);
        }

        static IndexerMemberCrefSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(IndexerMemberCrefSyntax), (ObjectReader r) => new IndexerMemberCrefSyntax(r));
        }
    }
}

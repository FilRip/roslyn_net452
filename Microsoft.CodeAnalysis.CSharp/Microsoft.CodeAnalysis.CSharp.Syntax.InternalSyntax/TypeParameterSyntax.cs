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

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class TypeParameterSyntax : CSharpSyntaxNode
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken? varianceKeyword;

        internal readonly SyntaxToken identifier;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken? VarianceKeyword => varianceKeyword;

        public SyntaxToken Identifier => identifier;

        public TypeParameterSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? varianceKeyword, SyntaxToken identifier, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (varianceKeyword != null)
            {
                AdjustFlagsAndWidth(varianceKeyword);
                this.varianceKeyword = varianceKeyword;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public TypeParameterSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? varianceKeyword, SyntaxToken identifier, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (varianceKeyword != null)
            {
                AdjustFlagsAndWidth(varianceKeyword);
                this.varianceKeyword = varianceKeyword;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public TypeParameterSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? varianceKeyword, SyntaxToken identifier)
            : base(kind)
        {
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (varianceKeyword != null)
            {
                AdjustFlagsAndWidth(varianceKeyword);
                this.varianceKeyword = varianceKeyword;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => varianceKeyword,
                2 => identifier,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTypeParameter(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitTypeParameter(this);
        }

        public TypeParameterSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken varianceKeyword, SyntaxToken identifier)
        {
            if (attributeLists != AttributeLists || varianceKeyword != VarianceKeyword || identifier != Identifier)
            {
                TypeParameterSyntax typeParameterSyntax = SyntaxFactory.TypeParameter(attributeLists, varianceKeyword, identifier);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    typeParameterSyntax = typeParameterSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    typeParameterSyntax = typeParameterSyntax.WithAnnotationsGreen(annotations);
                }
                return typeParameterSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new TypeParameterSyntax(base.Kind, attributeLists, varianceKeyword, identifier, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new TypeParameterSyntax(base.Kind, attributeLists, varianceKeyword, identifier, GetDiagnostics(), annotations);
        }

        public TypeParameterSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                varianceKeyword = syntaxToken;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            identifier = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(varianceKeyword);
            writer.WriteValue(identifier);
        }

        static TypeParameterSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(TypeParameterSyntax), (ObjectReader r) => new TypeParameterSyntax(r));
        }
    }
}

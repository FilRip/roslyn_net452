using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class SubpatternSyntax : CSharpSyntaxNode
    {
        internal readonly NameColonSyntax? nameColon;

        internal readonly PatternSyntax pattern;

        public NameColonSyntax? NameColon => nameColon;

        public PatternSyntax Pattern => pattern;

        public SubpatternSyntax(SyntaxKind kind, NameColonSyntax? nameColon, PatternSyntax pattern, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            if (nameColon != null)
            {
                AdjustFlagsAndWidth(nameColon);
                this.nameColon = nameColon;
            }
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
        }

        public SubpatternSyntax(SyntaxKind kind, NameColonSyntax? nameColon, PatternSyntax pattern, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            if (nameColon != null)
            {
                AdjustFlagsAndWidth(nameColon);
                this.nameColon = nameColon;
            }
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
        }

        public SubpatternSyntax(SyntaxKind kind, NameColonSyntax? nameColon, PatternSyntax pattern)
            : base(kind)
        {
            base.SlotCount = 2;
            if (nameColon != null)
            {
                AdjustFlagsAndWidth(nameColon);
                this.nameColon = nameColon;
            }
            AdjustFlagsAndWidth(pattern);
            this.pattern = pattern;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => nameColon,
                1 => pattern,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSubpattern(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitSubpattern(this);
        }

        public SubpatternSyntax Update(NameColonSyntax nameColon, PatternSyntax pattern)
        {
            if (nameColon != NameColon || pattern != Pattern)
            {
                SubpatternSyntax subpatternSyntax = SyntaxFactory.Subpattern(nameColon, pattern);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    subpatternSyntax = subpatternSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    subpatternSyntax = subpatternSyntax.WithAnnotationsGreen(annotations);
                }
                return subpatternSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SubpatternSyntax(base.Kind, nameColon, pattern, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SubpatternSyntax(base.Kind, nameColon, pattern, GetDiagnostics(), annotations);
        }

        public SubpatternSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            NameColonSyntax nameColonSyntax = (NameColonSyntax)reader.ReadValue();
            if (nameColonSyntax != null)
            {
                AdjustFlagsAndWidth(nameColonSyntax);
                nameColon = nameColonSyntax;
            }
            PatternSyntax node = (PatternSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            pattern = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(nameColon);
            writer.WriteValue(pattern);
        }

        static SubpatternSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(SubpatternSyntax), (ObjectReader r) => new SubpatternSyntax(r));
        }
    }
}

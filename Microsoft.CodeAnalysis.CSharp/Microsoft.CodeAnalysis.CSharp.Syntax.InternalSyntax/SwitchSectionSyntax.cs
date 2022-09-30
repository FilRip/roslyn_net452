using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class SwitchSectionSyntax : CSharpSyntaxNode
    {
        internal readonly GreenNode? labels;

        internal readonly GreenNode? statements;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SwitchLabelSyntax> Labels => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SwitchLabelSyntax>(labels);

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Statements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(statements);

        public SwitchSectionSyntax(SyntaxKind kind, GreenNode? labels, GreenNode? statements, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            if (labels != null)
            {
                AdjustFlagsAndWidth(labels);
                this.labels = labels;
            }
            if (statements != null)
            {
                AdjustFlagsAndWidth(statements);
                this.statements = statements;
            }
        }

        public SwitchSectionSyntax(SyntaxKind kind, GreenNode? labels, GreenNode? statements, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            if (labels != null)
            {
                AdjustFlagsAndWidth(labels);
                this.labels = labels;
            }
            if (statements != null)
            {
                AdjustFlagsAndWidth(statements);
                this.statements = statements;
            }
        }

        public SwitchSectionSyntax(SyntaxKind kind, GreenNode? labels, GreenNode? statements)
            : base(kind)
        {
            base.SlotCount = 2;
            if (labels != null)
            {
                AdjustFlagsAndWidth(labels);
                this.labels = labels;
            }
            if (statements != null)
            {
                AdjustFlagsAndWidth(statements);
                this.statements = statements;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => labels,
                1 => statements,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.SwitchSectionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSwitchSection(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitSwitchSection(this);
        }

        public SwitchSectionSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SwitchLabelSyntax> labels, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> statements)
        {
            if (labels != Labels || statements != Statements)
            {
                SwitchSectionSyntax switchSectionSyntax = SyntaxFactory.SwitchSection(labels, statements);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    switchSectionSyntax = switchSectionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    switchSectionSyntax = switchSectionSyntax.WithAnnotationsGreen(annotations);
                }
                return switchSectionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SwitchSectionSyntax(base.Kind, labels, statements, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SwitchSectionSyntax(base.Kind, labels, statements, GetDiagnostics(), annotations);
        }

        public SwitchSectionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                labels = greenNode;
            }
            GreenNode greenNode2 = (GreenNode)reader.ReadValue();
            if (greenNode2 != null)
            {
                AdjustFlagsAndWidth(greenNode2);
                statements = greenNode2;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(labels);
            writer.WriteValue(statements);
        }

        static SwitchSectionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(SwitchSectionSyntax), (ObjectReader r) => new SwitchSectionSyntax(r));
        }
    }
}

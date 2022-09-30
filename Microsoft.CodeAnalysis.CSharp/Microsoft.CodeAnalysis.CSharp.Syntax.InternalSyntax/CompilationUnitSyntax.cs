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

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class CompilationUnitSyntax : CSharpSyntaxNode
    {
        internal readonly GreenNode? externs;

        internal readonly GreenNode? usings;

        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? members;

        internal readonly SyntaxToken endOfFileToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExternAliasDirectiveSyntax> Externs => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExternAliasDirectiveSyntax>(externs);

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<UsingDirectiveSyntax> Usings => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<UsingDirectiveSyntax>(usings);

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax> Members => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax>(members);

        public SyntaxToken EndOfFileToken => endOfFileToken;

        public CompilationUnitSyntax(SyntaxKind kind, GreenNode? externs, GreenNode? usings, GreenNode? attributeLists, GreenNode? members, SyntaxToken endOfFileToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            if (externs != null)
            {
                AdjustFlagsAndWidth(externs);
                this.externs = externs;
            }
            if (usings != null)
            {
                AdjustFlagsAndWidth(usings);
                this.usings = usings;
            }
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (members != null)
            {
                AdjustFlagsAndWidth(members);
                this.members = members;
            }
            AdjustFlagsAndWidth(endOfFileToken);
            this.endOfFileToken = endOfFileToken;
        }

        public CompilationUnitSyntax(SyntaxKind kind, GreenNode? externs, GreenNode? usings, GreenNode? attributeLists, GreenNode? members, SyntaxToken endOfFileToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            if (externs != null)
            {
                AdjustFlagsAndWidth(externs);
                this.externs = externs;
            }
            if (usings != null)
            {
                AdjustFlagsAndWidth(usings);
                this.usings = usings;
            }
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (members != null)
            {
                AdjustFlagsAndWidth(members);
                this.members = members;
            }
            AdjustFlagsAndWidth(endOfFileToken);
            this.endOfFileToken = endOfFileToken;
        }

        public CompilationUnitSyntax(SyntaxKind kind, GreenNode? externs, GreenNode? usings, GreenNode? attributeLists, GreenNode? members, SyntaxToken endOfFileToken)
            : base(kind)
        {
            base.SlotCount = 5;
            if (externs != null)
            {
                AdjustFlagsAndWidth(externs);
                this.externs = externs;
            }
            if (usings != null)
            {
                AdjustFlagsAndWidth(usings);
                this.usings = usings;
            }
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (members != null)
            {
                AdjustFlagsAndWidth(members);
                this.members = members;
            }
            AdjustFlagsAndWidth(endOfFileToken);
            this.endOfFileToken = endOfFileToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => externs,
                1 => usings,
                2 => attributeLists,
                3 => members,
                4 => endOfFileToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCompilationUnit(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitCompilationUnit(this);
        }

        public CompilationUnitSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExternAliasDirectiveSyntax> externs, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax> members, SyntaxToken endOfFileToken)
        {
            if (externs != Externs || usings != Usings || attributeLists != AttributeLists || members != Members || endOfFileToken != EndOfFileToken)
            {
                CompilationUnitSyntax compilationUnitSyntax = SyntaxFactory.CompilationUnit(externs, usings, attributeLists, members, endOfFileToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    compilationUnitSyntax = compilationUnitSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    compilationUnitSyntax = compilationUnitSyntax.WithAnnotationsGreen(annotations);
                }
                return compilationUnitSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new CompilationUnitSyntax(base.Kind, externs, usings, attributeLists, members, endOfFileToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new CompilationUnitSyntax(base.Kind, externs, usings, attributeLists, members, endOfFileToken, GetDiagnostics(), annotations);
        }

        public CompilationUnitSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                externs = greenNode;
            }
            GreenNode greenNode2 = (GreenNode)reader.ReadValue();
            if (greenNode2 != null)
            {
                AdjustFlagsAndWidth(greenNode2);
                usings = greenNode2;
            }
            GreenNode greenNode3 = (GreenNode)reader.ReadValue();
            if (greenNode3 != null)
            {
                AdjustFlagsAndWidth(greenNode3);
                attributeLists = greenNode3;
            }
            GreenNode greenNode4 = (GreenNode)reader.ReadValue();
            if (greenNode4 != null)
            {
                AdjustFlagsAndWidth(greenNode4);
                members = greenNode4;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            endOfFileToken = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(externs);
            writer.WriteValue(usings);
            writer.WriteValue(attributeLists);
            writer.WriteValue(members);
            writer.WriteValue(endOfFileToken);
        }

        static CompilationUnitSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(CompilationUnitSyntax), (ObjectReader r) => new CompilationUnitSyntax(r));
        }
    }
}

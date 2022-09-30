using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ClassOrStructConstraintSyntax : TypeParameterConstraintSyntax
    {
        internal readonly SyntaxToken classOrStructKeyword;

        internal readonly SyntaxToken? questionToken;

        public SyntaxToken ClassOrStructKeyword => classOrStructKeyword;

        public SyntaxToken? QuestionToken => questionToken;

        public ClassOrStructConstraintSyntax(SyntaxKind kind, SyntaxToken classOrStructKeyword, SyntaxToken? questionToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(classOrStructKeyword);
            this.classOrStructKeyword = classOrStructKeyword;
            if (questionToken != null)
            {
                AdjustFlagsAndWidth(questionToken);
                this.questionToken = questionToken;
            }
        }

        public ClassOrStructConstraintSyntax(SyntaxKind kind, SyntaxToken classOrStructKeyword, SyntaxToken? questionToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(classOrStructKeyword);
            this.classOrStructKeyword = classOrStructKeyword;
            if (questionToken != null)
            {
                AdjustFlagsAndWidth(questionToken);
                this.questionToken = questionToken;
            }
        }

        public ClassOrStructConstraintSyntax(SyntaxKind kind, SyntaxToken classOrStructKeyword, SyntaxToken? questionToken)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(classOrStructKeyword);
            this.classOrStructKeyword = classOrStructKeyword;
            if (questionToken != null)
            {
                AdjustFlagsAndWidth(questionToken);
                this.questionToken = questionToken;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => classOrStructKeyword,
                1 => questionToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitClassOrStructConstraint(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitClassOrStructConstraint(this);
        }

        public ClassOrStructConstraintSyntax Update(SyntaxToken classOrStructKeyword, SyntaxToken questionToken)
        {
            if (classOrStructKeyword != ClassOrStructKeyword || questionToken != QuestionToken)
            {
                ClassOrStructConstraintSyntax classOrStructConstraintSyntax = SyntaxFactory.ClassOrStructConstraint(base.Kind, classOrStructKeyword, questionToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    classOrStructConstraintSyntax = classOrStructConstraintSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    classOrStructConstraintSyntax = classOrStructConstraintSyntax.WithAnnotationsGreen(annotations);
                }
                return classOrStructConstraintSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ClassOrStructConstraintSyntax(base.Kind, classOrStructKeyword, questionToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ClassOrStructConstraintSyntax(base.Kind, classOrStructKeyword, questionToken, GetDiagnostics(), annotations);
        }

        public ClassOrStructConstraintSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            classOrStructKeyword = node;
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                questionToken = syntaxToken;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(classOrStructKeyword);
            writer.WriteValue(questionToken);
        }

        static ClassOrStructConstraintSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ClassOrStructConstraintSyntax), (ObjectReader r) => new ClassOrStructConstraintSyntax(r));
        }
    }
}

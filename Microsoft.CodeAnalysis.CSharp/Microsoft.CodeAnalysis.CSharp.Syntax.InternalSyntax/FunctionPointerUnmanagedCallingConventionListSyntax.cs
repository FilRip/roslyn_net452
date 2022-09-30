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

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class FunctionPointerUnmanagedCallingConventionListSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken openBracketToken;

        internal readonly GreenNode? callingConventions;

        internal readonly SyntaxToken closeBracketToken;

        public SyntaxToken OpenBracketToken => openBracketToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> CallingConventions => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(callingConventions));

        public SyntaxToken CloseBracketToken => closeBracketToken;

        public FunctionPointerUnmanagedCallingConventionListSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? callingConventions, SyntaxToken closeBracketToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (callingConventions != null)
            {
                AdjustFlagsAndWidth(callingConventions);
                this.callingConventions = callingConventions;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public FunctionPointerUnmanagedCallingConventionListSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? callingConventions, SyntaxToken closeBracketToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (callingConventions != null)
            {
                AdjustFlagsAndWidth(callingConventions);
                this.callingConventions = callingConventions;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public FunctionPointerUnmanagedCallingConventionListSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? callingConventions, SyntaxToken closeBracketToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (callingConventions != null)
            {
                AdjustFlagsAndWidth(callingConventions);
                this.callingConventions = callingConventions;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openBracketToken,
                1 => callingConventions,
                2 => closeBracketToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionListSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFunctionPointerUnmanagedCallingConventionList(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitFunctionPointerUnmanagedCallingConventionList(this);
        }

        public FunctionPointerUnmanagedCallingConventionListSyntax Update(SyntaxToken openBracketToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> callingConventions, SyntaxToken closeBracketToken)
        {
            if (openBracketToken == OpenBracketToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> right = CallingConventions;
                if (!(callingConventions != right) && closeBracketToken == CloseBracketToken)
                {
                    return this;
                }
            }
            FunctionPointerUnmanagedCallingConventionListSyntax functionPointerUnmanagedCallingConventionListSyntax = SyntaxFactory.FunctionPointerUnmanagedCallingConventionList(openBracketToken, callingConventions, closeBracketToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                functionPointerUnmanagedCallingConventionListSyntax = functionPointerUnmanagedCallingConventionListSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                functionPointerUnmanagedCallingConventionListSyntax = functionPointerUnmanagedCallingConventionListSyntax.WithAnnotationsGreen(annotations);
            }
            return functionPointerUnmanagedCallingConventionListSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new FunctionPointerUnmanagedCallingConventionListSyntax(base.Kind, openBracketToken, callingConventions, closeBracketToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new FunctionPointerUnmanagedCallingConventionListSyntax(base.Kind, openBracketToken, callingConventions, closeBracketToken, GetDiagnostics(), annotations);
        }

        public FunctionPointerUnmanagedCallingConventionListSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openBracketToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                callingConventions = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            closeBracketToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openBracketToken);
            writer.WriteValue(callingConventions);
            writer.WriteValue(closeBracketToken);
        }

        static FunctionPointerUnmanagedCallingConventionListSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(FunctionPointerUnmanagedCallingConventionListSyntax), (ObjectReader r) => new FunctionPointerUnmanagedCallingConventionListSyntax(r));
        }
    }
}

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
    public sealed class FunctionPointerTypeSyntax : TypeSyntax
    {
        internal readonly SyntaxToken delegateKeyword;

        internal readonly SyntaxToken asteriskToken;

        internal readonly FunctionPointerCallingConventionSyntax? callingConvention;

        internal readonly FunctionPointerParameterListSyntax parameterList;

        public SyntaxToken DelegateKeyword => delegateKeyword;

        public SyntaxToken AsteriskToken => asteriskToken;

        public FunctionPointerCallingConventionSyntax? CallingConvention => callingConvention;

        public FunctionPointerParameterListSyntax ParameterList => parameterList;

        public FunctionPointerTypeSyntax(SyntaxKind kind, SyntaxToken delegateKeyword, SyntaxToken asteriskToken, FunctionPointerCallingConventionSyntax? callingConvention, FunctionPointerParameterListSyntax parameterList, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(delegateKeyword);
            this.delegateKeyword = delegateKeyword;
            AdjustFlagsAndWidth(asteriskToken);
            this.asteriskToken = asteriskToken;
            if (callingConvention != null)
            {
                AdjustFlagsAndWidth(callingConvention);
                this.callingConvention = callingConvention;
            }
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
        }

        public FunctionPointerTypeSyntax(SyntaxKind kind, SyntaxToken delegateKeyword, SyntaxToken asteriskToken, FunctionPointerCallingConventionSyntax? callingConvention, FunctionPointerParameterListSyntax parameterList, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(delegateKeyword);
            this.delegateKeyword = delegateKeyword;
            AdjustFlagsAndWidth(asteriskToken);
            this.asteriskToken = asteriskToken;
            if (callingConvention != null)
            {
                AdjustFlagsAndWidth(callingConvention);
                this.callingConvention = callingConvention;
            }
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
        }

        public FunctionPointerTypeSyntax(SyntaxKind kind, SyntaxToken delegateKeyword, SyntaxToken asteriskToken, FunctionPointerCallingConventionSyntax? callingConvention, FunctionPointerParameterListSyntax parameterList)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(delegateKeyword);
            this.delegateKeyword = delegateKeyword;
            AdjustFlagsAndWidth(asteriskToken);
            this.asteriskToken = asteriskToken;
            if (callingConvention != null)
            {
                AdjustFlagsAndWidth(callingConvention);
                this.callingConvention = callingConvention;
            }
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => delegateKeyword,
                1 => asteriskToken,
                2 => callingConvention,
                3 => parameterList,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerTypeSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFunctionPointerType(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitFunctionPointerType(this);
        }

        public FunctionPointerTypeSyntax Update(SyntaxToken delegateKeyword, SyntaxToken asteriskToken, FunctionPointerCallingConventionSyntax callingConvention, FunctionPointerParameterListSyntax parameterList)
        {
            if (delegateKeyword != DelegateKeyword || asteriskToken != AsteriskToken || callingConvention != CallingConvention || parameterList != ParameterList)
            {
                FunctionPointerTypeSyntax functionPointerTypeSyntax = SyntaxFactory.FunctionPointerType(delegateKeyword, asteriskToken, callingConvention, parameterList);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    functionPointerTypeSyntax = functionPointerTypeSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    functionPointerTypeSyntax = functionPointerTypeSyntax.WithAnnotationsGreen(annotations);
                }
                return functionPointerTypeSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new FunctionPointerTypeSyntax(base.Kind, delegateKeyword, asteriskToken, callingConvention, parameterList, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new FunctionPointerTypeSyntax(base.Kind, delegateKeyword, asteriskToken, callingConvention, parameterList, GetDiagnostics(), annotations);
        }

        public FunctionPointerTypeSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            delegateKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            asteriskToken = node2;
            FunctionPointerCallingConventionSyntax functionPointerCallingConventionSyntax = (FunctionPointerCallingConventionSyntax)reader.ReadValue();
            if (functionPointerCallingConventionSyntax != null)
            {
                AdjustFlagsAndWidth(functionPointerCallingConventionSyntax);
                callingConvention = functionPointerCallingConventionSyntax;
            }
            FunctionPointerParameterListSyntax node3 = (FunctionPointerParameterListSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            parameterList = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(delegateKeyword);
            writer.WriteValue(asteriskToken);
            writer.WriteValue(callingConvention);
            writer.WriteValue(parameterList);
        }

        static FunctionPointerTypeSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(FunctionPointerTypeSyntax), (ObjectReader r) => new FunctionPointerTypeSyntax(r));
        }
    }
}

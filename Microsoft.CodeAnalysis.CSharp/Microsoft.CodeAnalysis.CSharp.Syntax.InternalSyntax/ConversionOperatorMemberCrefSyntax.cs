using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ConversionOperatorMemberCrefSyntax : MemberCrefSyntax
    {
        internal readonly SyntaxToken implicitOrExplicitKeyword;

        internal readonly SyntaxToken operatorKeyword;

        internal readonly TypeSyntax type;

        internal readonly CrefParameterListSyntax? parameters;

        public SyntaxToken ImplicitOrExplicitKeyword => implicitOrExplicitKeyword;

        public SyntaxToken OperatorKeyword => operatorKeyword;

        public TypeSyntax Type => type;

        public CrefParameterListSyntax? Parameters => parameters;

        public ConversionOperatorMemberCrefSyntax(SyntaxKind kind, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, CrefParameterListSyntax? parameters, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(implicitOrExplicitKeyword);
            this.implicitOrExplicitKeyword = implicitOrExplicitKeyword;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
        }

        public ConversionOperatorMemberCrefSyntax(SyntaxKind kind, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, CrefParameterListSyntax? parameters, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(implicitOrExplicitKeyword);
            this.implicitOrExplicitKeyword = implicitOrExplicitKeyword;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
        }

        public ConversionOperatorMemberCrefSyntax(SyntaxKind kind, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, CrefParameterListSyntax? parameters)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(implicitOrExplicitKeyword);
            this.implicitOrExplicitKeyword = implicitOrExplicitKeyword;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
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
                0 => implicitOrExplicitKeyword,
                1 => operatorKeyword,
                2 => type,
                3 => parameters,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorMemberCrefSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConversionOperatorMemberCref(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitConversionOperatorMemberCref(this);
        }

        public ConversionOperatorMemberCrefSyntax Update(SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, CrefParameterListSyntax parameters)
        {
            if (implicitOrExplicitKeyword != ImplicitOrExplicitKeyword || operatorKeyword != OperatorKeyword || type != Type || parameters != Parameters)
            {
                ConversionOperatorMemberCrefSyntax conversionOperatorMemberCrefSyntax = SyntaxFactory.ConversionOperatorMemberCref(implicitOrExplicitKeyword, operatorKeyword, type, parameters);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    conversionOperatorMemberCrefSyntax = conversionOperatorMemberCrefSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    conversionOperatorMemberCrefSyntax = conversionOperatorMemberCrefSyntax.WithAnnotationsGreen(annotations);
                }
                return conversionOperatorMemberCrefSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ConversionOperatorMemberCrefSyntax(base.Kind, implicitOrExplicitKeyword, operatorKeyword, type, parameters, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ConversionOperatorMemberCrefSyntax(base.Kind, implicitOrExplicitKeyword, operatorKeyword, type, parameters, GetDiagnostics(), annotations);
        }

        public ConversionOperatorMemberCrefSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            implicitOrExplicitKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            operatorKeyword = node2;
            TypeSyntax node3 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            type = node3;
            CrefParameterListSyntax crefParameterListSyntax = (CrefParameterListSyntax)reader.ReadValue();
            if (crefParameterListSyntax != null)
            {
                AdjustFlagsAndWidth(crefParameterListSyntax);
                parameters = crefParameterListSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(implicitOrExplicitKeyword);
            writer.WriteValue(operatorKeyword);
            writer.WriteValue(type);
            writer.WriteValue(parameters);
        }

        static ConversionOperatorMemberCrefSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ConversionOperatorMemberCrefSyntax), (ObjectReader r) => new ConversionOperatorMemberCrefSyntax(r));
        }
    }
}

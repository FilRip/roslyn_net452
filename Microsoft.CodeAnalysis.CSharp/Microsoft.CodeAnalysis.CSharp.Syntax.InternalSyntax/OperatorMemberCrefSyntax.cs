using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class OperatorMemberCrefSyntax : MemberCrefSyntax
    {
        internal readonly SyntaxToken operatorKeyword;

        internal readonly SyntaxToken operatorToken;

        internal readonly CrefParameterListSyntax? parameters;

        public SyntaxToken OperatorKeyword => operatorKeyword;

        public SyntaxToken OperatorToken => operatorToken;

        public CrefParameterListSyntax? Parameters => parameters;

        public OperatorMemberCrefSyntax(SyntaxKind kind, SyntaxToken operatorKeyword, SyntaxToken operatorToken, CrefParameterListSyntax? parameters, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
        }

        public OperatorMemberCrefSyntax(SyntaxKind kind, SyntaxToken operatorKeyword, SyntaxToken operatorToken, CrefParameterListSyntax? parameters, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
        }

        public OperatorMemberCrefSyntax(SyntaxKind kind, SyntaxToken operatorKeyword, SyntaxToken operatorToken, CrefParameterListSyntax? parameters)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
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
                0 => operatorKeyword,
                1 => operatorToken,
                2 => parameters,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.OperatorMemberCrefSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitOperatorMemberCref(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitOperatorMemberCref(this);
        }

        public OperatorMemberCrefSyntax Update(SyntaxToken operatorKeyword, SyntaxToken operatorToken, CrefParameterListSyntax parameters)
        {
            if (operatorKeyword != OperatorKeyword || operatorToken != OperatorToken || parameters != Parameters)
            {
                OperatorMemberCrefSyntax operatorMemberCrefSyntax = SyntaxFactory.OperatorMemberCref(operatorKeyword, operatorToken, parameters);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    operatorMemberCrefSyntax = operatorMemberCrefSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    operatorMemberCrefSyntax = operatorMemberCrefSyntax.WithAnnotationsGreen(annotations);
                }
                return operatorMemberCrefSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new OperatorMemberCrefSyntax(base.Kind, operatorKeyword, operatorToken, parameters, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new OperatorMemberCrefSyntax(base.Kind, operatorKeyword, operatorToken, parameters, GetDiagnostics(), annotations);
        }

        public OperatorMemberCrefSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            operatorKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            operatorToken = node2;
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
            writer.WriteValue(operatorKeyword);
            writer.WriteValue(operatorToken);
            writer.WriteValue(parameters);
        }

        static OperatorMemberCrefSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(OperatorMemberCrefSyntax), (ObjectReader r) => new OperatorMemberCrefSyntax(r));
        }
    }
}

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class NameMemberCrefSyntax : MemberCrefSyntax
    {
        internal readonly TypeSyntax name;

        internal readonly CrefParameterListSyntax? parameters;

        public TypeSyntax Name => name;

        public CrefParameterListSyntax? Parameters => parameters;

        public NameMemberCrefSyntax(SyntaxKind kind, TypeSyntax name, CrefParameterListSyntax? parameters, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
        }

        public NameMemberCrefSyntax(SyntaxKind kind, TypeSyntax name, CrefParameterListSyntax? parameters, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
        }

        public NameMemberCrefSyntax(SyntaxKind kind, TypeSyntax name, CrefParameterListSyntax? parameters)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(name);
            this.name = name;
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
                0 => name,
                1 => parameters,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.NameMemberCrefSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNameMemberCref(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitNameMemberCref(this);
        }

        public NameMemberCrefSyntax Update(TypeSyntax name, CrefParameterListSyntax parameters)
        {
            if (name != Name || parameters != Parameters)
            {
                NameMemberCrefSyntax nameMemberCrefSyntax = SyntaxFactory.NameMemberCref(name, parameters);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    nameMemberCrefSyntax = nameMemberCrefSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    nameMemberCrefSyntax = nameMemberCrefSyntax.WithAnnotationsGreen(annotations);
                }
                return nameMemberCrefSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new NameMemberCrefSyntax(base.Kind, name, parameters, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new NameMemberCrefSyntax(base.Kind, name, parameters, GetDiagnostics(), annotations);
        }

        public NameMemberCrefSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            TypeSyntax node = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            name = node;
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
            writer.WriteValue(name);
            writer.WriteValue(parameters);
        }

        static NameMemberCrefSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(NameMemberCrefSyntax), (ObjectReader r) => new NameMemberCrefSyntax(r));
        }
    }
}

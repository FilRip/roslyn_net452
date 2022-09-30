#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class NameMemberCrefSyntax : MemberCrefSyntax
    {
        private TypeSyntax? name;

        private CrefParameterListSyntax? parameters;

        public TypeSyntax Name => GetRedAtZero(ref name);

        public CrefParameterListSyntax? Parameters => GetRed(ref parameters, 1);

        internal NameMemberCrefSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref name),
                1 => GetRed(ref parameters, 1),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => name,
                1 => parameters,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNameMemberCref(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNameMemberCref(this);

        public NameMemberCrefSyntax Update(TypeSyntax name, CrefParameterListSyntax? parameters)
        {
            if (name != Name || parameters != Parameters)
            {
                NameMemberCrefSyntax nameMemberCrefSyntax = SyntaxFactory.NameMemberCref(name, parameters);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return nameMemberCrefSyntax;
                }
                return nameMemberCrefSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public NameMemberCrefSyntax WithName(TypeSyntax name)
        {
            return Update(name, Parameters);
        }

        public NameMemberCrefSyntax WithParameters(CrefParameterListSyntax? parameters)
        {
            return Update(Name, parameters);
        }

        public NameMemberCrefSyntax AddParametersParameters(params CrefParameterSyntax[] items)
        {
            CrefParameterListSyntax crefParameterListSyntax = Parameters ?? SyntaxFactory.CrefParameterList();
            return WithParameters(crefParameterListSyntax.WithParameters(crefParameterListSyntax.Parameters.AddRange(items)));
        }
    }
}

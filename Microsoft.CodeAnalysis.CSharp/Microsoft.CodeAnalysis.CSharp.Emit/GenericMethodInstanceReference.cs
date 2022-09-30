using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    internal sealed class GenericMethodInstanceReference : MethodReference, IGenericMethodInstanceReference, IMethodReference, ISignature, ITypeMemberReference, IReference, INamedEntity
    {
        public override IGenericMethodInstanceReference AsGenericMethodInstanceReference => this;

        public GenericMethodInstanceReference(MethodSymbol underlyingMethod)
            : base(underlyingMethod)
        {
        }

        public override void Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        IEnumerable<ITypeReference> IGenericMethodInstanceReference.GetGenericArguments(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = UnderlyingMethod.TypeArgumentsWithAnnotations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return moduleBeingBuilt.Translate(enumerator.Current.Type, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
            }
        }

        IMethodReference IGenericMethodInstanceReference.GetGenericMethod(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(UnderlyingMethod.OriginalDefinition, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics, null, needDeclaration: true);
        }
    }
}

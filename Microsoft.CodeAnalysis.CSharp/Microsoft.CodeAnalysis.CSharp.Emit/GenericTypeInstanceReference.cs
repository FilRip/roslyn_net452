using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    internal abstract class GenericTypeInstanceReference : NamedTypeReference, IGenericTypeInstanceReference, ITypeReference, IReference
    {
        public GenericTypeInstanceReference(NamedTypeSymbol underlyingNamedType)
            : base(underlyingNamedType)
        {
        }

        public sealed override void Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        ImmutableArray<ITypeReference> IGenericTypeInstanceReference.GetGenericArguments(EmitContext context)
        {
            PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
            ArrayBuilder<ITypeReference> instance = ArrayBuilder<ITypeReference>.GetInstance();
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = UnderlyingNamedType.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                instance.Add(pEModuleBuilder.Translate(enumerator.Current.Type, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics));
            }
            return instance.ToImmutableAndFree();
        }

        INamedTypeReference IGenericTypeInstanceReference.GetGenericType(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(UnderlyingNamedType.OriginalDefinition, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: false, needDeclaration: true);
        }
    }
}

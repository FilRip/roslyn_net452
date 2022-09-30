using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class SpecializedGenericNestedTypeInstanceReference : SpecializedNestedTypeReference, IGenericTypeInstanceReference
	{
		public override IGenericTypeInstanceReference AsGenericTypeInstanceReference => this;

		public override INamespaceTypeReference AsNamespaceTypeReference => null;

		public override INestedTypeReference AsNestedTypeReference => this;

		public override ISpecializedNestedTypeReference AsSpecializedNestedTypeReference => null;

		public SpecializedGenericNestedTypeInstanceReference(NamedTypeSymbol underlyingNamedType)
			: base(underlyingNamedType)
		{
		}

		public override void Dispatch(MetadataVisitor visitor)
		{
			visitor.Visit((IGenericTypeInstanceReference)this);
		}

		private ImmutableArray<ITypeReference> IGenericTypeInstanceReferenceGetGenericArguments(EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			ArrayBuilder<ITypeReference> instance = ArrayBuilder<ITypeReference>.GetInstance();
			ImmutableArray<TypeSymbol>.Enumerator enumerator = m_UnderlyingNamedType.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol current = enumerator.Current;
				instance.Add(pEModuleBuilder.Translate(current, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics));
			}
			return instance.ToImmutableAndFree();
		}

		ImmutableArray<ITypeReference> IGenericTypeInstanceReference.GetGenericArguments(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IGenericTypeInstanceReferenceGetGenericArguments
			return this.IGenericTypeInstanceReferenceGetGenericArguments(context);
		}

		private INamedTypeReference IGenericTypeInstanceReferenceGetGenericType(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(m_UnderlyingNamedType.OriginalDefinition, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: false, needDeclaration: true);
		}

		INamedTypeReference IGenericTypeInstanceReference.GetGenericType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IGenericTypeInstanceReferenceGetGenericType
			return this.IGenericTypeInstanceReferenceGetGenericType(context);
		}
	}
}

using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal abstract class NamedTypeReference : INamedTypeReference
	{
		protected readonly NamedTypeSymbol m_UnderlyingNamedType;

		private ushort INamedTypeReferenceGenericParameterCount => (ushort)m_UnderlyingNamedType.Arity;

		private bool INamedTypeReferenceMangleName => m_UnderlyingNamedType.MangleName;

		private string INamedEntityName => m_UnderlyingNamedType.Name;

		private bool ITypeReferenceIsEnum => m_UnderlyingNamedType.TypeKind == TypeKind.Enum;

		private bool ITypeReferenceIsValueType => m_UnderlyingNamedType.IsValueType;

		private Microsoft.Cci.PrimitiveTypeCode ITypeReferenceTypeCode => Microsoft.Cci.PrimitiveTypeCode.NotPrimitive;

		private TypeDefinitionHandle ITypeReferenceTypeDef => default(TypeDefinitionHandle);

		private IGenericMethodParameterReference ITypeReferenceAsGenericMethodParameterReference => null;

		public abstract IGenericTypeInstanceReference AsGenericTypeInstanceReference { get; }

		private IGenericTypeParameterReference ITypeReferenceAsGenericTypeParameterReference => null;

		public abstract INamespaceTypeReference AsNamespaceTypeReference { get; }

		public abstract INestedTypeReference AsNestedTypeReference { get; }

		public abstract ISpecializedNestedTypeReference AsSpecializedNestedTypeReference { get; }

		public NamedTypeReference(NamedTypeSymbol underlyingNamedType)
		{
			m_UnderlyingNamedType = underlyingNamedType;
		}

		private ITypeDefinition ITypeReferenceGetResolvedType(EmitContext context)
		{
			return null;
		}

		ITypeDefinition ITypeReference.GetResolvedType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceGetResolvedType
			return this.ITypeReferenceGetResolvedType(context);
		}

		private INamespaceTypeDefinition ITypeReferenceAsNamespaceTypeDefinition(EmitContext context)
		{
			return null;
		}

		INamespaceTypeDefinition ITypeReference.AsNamespaceTypeDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceAsNamespaceTypeDefinition
			return this.ITypeReferenceAsNamespaceTypeDefinition(context);
		}

		private INestedTypeDefinition ITypeReferenceAsNestedTypeDefinition(EmitContext context)
		{
			return null;
		}

		INestedTypeDefinition ITypeReference.AsNestedTypeDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceAsNestedTypeDefinition
			return this.ITypeReferenceAsNestedTypeDefinition(context);
		}

		private ITypeDefinition ITypeReferenceAsTypeDefinition(EmitContext context)
		{
			return null;
		}

		ITypeDefinition ITypeReference.AsTypeDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceAsTypeDefinition
			return this.ITypeReferenceAsTypeDefinition(context);
		}

		public override string ToString()
		{
			return m_UnderlyingNamedType.ToString();
		}

		private IEnumerable<ICustomAttribute> IReferenceAttributes(EmitContext context)
		{
			return SpecializedCollections.EmptyEnumerable<ICustomAttribute>();
		}

		IEnumerable<ICustomAttribute> IReference.GetAttributes(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IReferenceAttributes
			return this.IReferenceAttributes(context);
		}

		public abstract void Dispatch(MetadataVisitor visitor);

		void IReference.Dispatch(MetadataVisitor visitor)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Dispatch
			this.Dispatch(visitor);
		}

		private IDefinition IReferenceAsDefinition(EmitContext context)
		{
			return null;
		}

		IDefinition IReference.AsDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IReferenceAsDefinition
			return this.IReferenceAsDefinition(context);
		}

		private ISymbolInternal IReferenceGetInternalSymbol()
		{
			return m_UnderlyingNamedType;
		}

		ISymbolInternal IReference.GetInternalSymbol()
		{
			//ILSpy generated this explicit interface implementation from .override directive in IReferenceGetInternalSymbol
			return this.IReferenceGetInternalSymbol();
		}

		public sealed override bool Equals(object obj)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public sealed override int GetHashCode()
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}

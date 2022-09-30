using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal abstract class TypeMemberReference : ITypeMemberReference
	{
		protected abstract Symbol UnderlyingSymbol { get; }

		private string INamedEntityName => UnderlyingSymbol.MetadataName;

		public virtual ITypeReference GetContainingType(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(UnderlyingSymbol.ContainingType, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetContainingType
			return this.GetContainingType(context);
		}

		public override string ToString()
		{
			return UnderlyingSymbol.ToDisplayString(SymbolDisplayFormat.ILVisualizationFormat);
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
			return UnderlyingSymbol;
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

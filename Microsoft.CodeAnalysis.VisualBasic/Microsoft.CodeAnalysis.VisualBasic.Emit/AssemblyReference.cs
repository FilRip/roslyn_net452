using System;
using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class AssemblyReference : IAssemblyReference
	{
		private readonly AssemblySymbol _targetAssembly;

		public AssemblyIdentity Identity => _targetAssembly.Identity;

		public Version AssemblyVersionPattern => _targetAssembly.AssemblyVersionPattern;

		private string INamedEntityName => Identity.Name;

		public AssemblyReference(AssemblySymbol assemblySymbol)
		{
			_targetAssembly = assemblySymbol;
		}

		private void IReferenceDispatch(MetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		void IReference.Dispatch(MetadataVisitor visitor)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IReferenceDispatch
			this.IReferenceDispatch(visitor);
		}

		private IAssemblyReference IModuleReferenceGetContainingAssembly(EmitContext context)
		{
			return this;
		}

		IAssemblyReference IModuleReference.GetContainingAssembly(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IModuleReferenceGetContainingAssembly
			return this.IModuleReferenceGetContainingAssembly(context);
		}

		public override string ToString()
		{
			return _targetAssembly.ToString();
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
			return null;
		}

		ISymbolInternal IReference.GetInternalSymbol()
		{
			//ILSpy generated this explicit interface implementation from .override directive in IReferenceGetInternalSymbol
			return this.IReferenceGetInternalSymbol();
		}
	}
}

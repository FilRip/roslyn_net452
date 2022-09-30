using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class ModuleReference : IModuleReference, IFileReference
	{
		private readonly PEModuleBuilder _moduleBeingBuilt;

		private readonly ModuleSymbol _underlyingModule;

		private string INamedEntityName => _underlyingModule.Name;

		private bool IFileReferenceHasMetadata => true;

		private string IFileReferenceFileName => _underlyingModule.Name;

		internal ModuleReference(PEModuleBuilder moduleBeingBuilt, ModuleSymbol underlyingModule)
		{
			_moduleBeingBuilt = moduleBeingBuilt;
			_underlyingModule = underlyingModule;
		}

		private void IReferenceDispatch(MetadataVisitor visitor)
		{
			visitor.Visit((IModuleReference)this);
		}

		void IReference.Dispatch(MetadataVisitor visitor)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IReferenceDispatch
			this.IReferenceDispatch(visitor);
		}

		private ImmutableArray<byte> IFileReferenceGetHashValue(AssemblyHashAlgorithm algorithmId)
		{
			return _underlyingModule.GetHash(algorithmId);
		}

		ImmutableArray<byte> IFileReference.GetHashValue(AssemblyHashAlgorithm algorithmId)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IFileReferenceGetHashValue
			return this.IFileReferenceGetHashValue(algorithmId);
		}

		private IAssemblyReference IModuleReferenceGetContainingAssembly(EmitContext context)
		{
			if (_moduleBeingBuilt.OutputKind.IsNetModule() && (object)_moduleBeingBuilt.SourceModule.ContainingAssembly == _underlyingModule.ContainingAssembly)
			{
				return null;
			}
			return _moduleBeingBuilt.Translate(_underlyingModule.ContainingAssembly, context.Diagnostics);
		}

		IAssemblyReference IModuleReference.GetContainingAssembly(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IModuleReferenceGetContainingAssembly
			return this.IModuleReferenceGetContainingAssembly(context);
		}

		public override string ToString()
		{
			return _underlyingModule.ToString();
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

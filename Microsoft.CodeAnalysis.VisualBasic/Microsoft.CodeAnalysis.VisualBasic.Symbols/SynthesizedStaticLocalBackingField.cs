using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedStaticLocalBackingField : SynthesizedFieldSymbol, IContextualNamedEntity
	{
		private MetadataWriter _metadataWriter;

		private string _nameToEmit;

		public readonly bool IsValueField;

		private readonly bool _reportErrorForLongNames;

		public override string MetadataName => _nameToEmit;

		internal override bool IsContextualNamedEntity => true;

		internal new LocalSymbol ImplicitlyDefinedBy => (LocalSymbol)_implicitlyDefinedBy;

		internal override bool HasRuntimeSpecialName => false;

		private void IContextualNamedEntity_AssociateWithMetadataWriter(MetadataWriter metadataWriter)
		{
			((SynthesizedStaticLocalBackingField)base.AdaptedFieldSymbol).AssociateWithMetadataWriter(metadataWriter);
		}

		void IContextualNamedEntity.AssociateWithMetadataWriter(MetadataWriter metadataWriter)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IContextualNamedEntity_AssociateWithMetadataWriter
			this.IContextualNamedEntity_AssociateWithMetadataWriter(metadataWriter);
		}

		internal void AssociateWithMetadataWriter(MetadataWriter metadataWriter)
		{
			Interlocked.CompareExchange(ref _metadataWriter, metadataWriter, null);
			if (_nameToEmit == null)
			{
				MethodSymbol methodSymbol = (MethodSymbol)ImplicitlyDefinedBy.ContainingSymbol;
				string methodSignature = GeneratedNames.MakeSignatureString(metadataWriter.GetMethodSignature(methodSymbol.GetCciAdapter()));
				_nameToEmit = GeneratedNames.MakeStaticLocalFieldName(methodSymbol.Name, methodSignature, Name);
			}
		}

		public SynthesizedStaticLocalBackingField(LocalSymbol implicitlyDefinedBy, bool isValueField, bool reportErrorForLongNames)
			: base(implicitlyDefinedBy.ContainingType, implicitlyDefinedBy, isValueField ? implicitlyDefinedBy.Type : implicitlyDefinedBy.DeclaringCompilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_CompilerServices_StaticLocalInitFlag), isValueField ? implicitlyDefinedBy.Name : (implicitlyDefinedBy.Name + "$Init"), Accessibility.Private, isReadOnly: false, implicitlyDefinedBy.ContainingSymbol.IsShared, isSpecialNameAndRuntimeSpecial: true)
		{
			IsValueField = isValueField;
			_reportErrorForLongNames = reportErrorForLongNames;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
		}
	}
}

using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class SpecializedFieldReference : TypeMemberReference, ISpecializedFieldReference, IContextualNamedEntity
	{
		private readonly FieldSymbol _underlyingField;

		protected override Symbol UnderlyingSymbol => _underlyingField;

		private IFieldReference ISpecializedFieldReferenceUnspecializedVersion => _underlyingField.OriginalDefinition.GetCciAdapter();

		private ISpecializedFieldReference IFieldReferenceAsSpecializedFieldReference => this;

		private bool IsContextualNamedEntity => _underlyingField.IsContextualNamedEntity;

		public SpecializedFieldReference(FieldSymbol underlyingField)
		{
			_underlyingField = underlyingField;
		}

		public override void Dispatch(MetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		private ITypeReference IFieldReferenceGetType(EmitContext context)
		{
			ImmutableArray<CustomModifier> customModifiers = _underlyingField.CustomModifiers;
			ITypeReference typeReference = ((PEModuleBuilder)context.Module).Translate(_underlyingField.Type, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
			if (customModifiers.Length == 0)
			{
				return typeReference;
			}
			return new ModifiedTypeReference(typeReference, customModifiers.As<ICustomModifier>());
		}

		ITypeReference IFieldReference.GetType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IFieldReferenceGetType
			return this.IFieldReferenceGetType(context);
		}

		private IFieldDefinition IFieldReferenceGetResolvedField(EmitContext context)
		{
			return null;
		}

		IFieldDefinition IFieldReference.GetResolvedField(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IFieldReferenceGetResolvedField
			return this.IFieldReferenceGetResolvedField(context);
		}

		private void AssociateWithMetadataWriter(MetadataWriter metadataWriter)
		{
			((SynthesizedStaticLocalBackingField)_underlyingField).AssociateWithMetadataWriter(metadataWriter);
		}

		void IContextualNamedEntity.AssociateWithMetadataWriter(MetadataWriter metadataWriter)
		{
			//ILSpy generated this explicit interface implementation from .override directive in AssociateWithMetadataWriter
			this.AssociateWithMetadataWriter(metadataWriter);
		}
	}
}

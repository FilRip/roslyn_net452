Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class SpecializedFieldReference
		Inherits TypeMemberReference
		Implements ISpecializedFieldReference, IContextualNamedEntity
		Private ReadOnly _underlyingField As FieldSymbol

		ReadOnly Property IFieldReferenceAsSpecializedFieldReference As ISpecializedFieldReference
			Get
				Return Me
			End Get
		End Property

		ReadOnly Property IsContextualNamedEntity As Boolean
			Get
				Return Me._underlyingField.IsContextualNamedEntity
			End Get
		End Property

		ReadOnly Property ISpecializedFieldReferenceUnspecializedVersion As IFieldReference Implements ISpecializedFieldReference.UnspecializedVersion
			Get
				Return Me._underlyingField.OriginalDefinition.GetCciAdapter()
			End Get
		End Property

		Protected Overrides ReadOnly Property UnderlyingSymbol As Symbol
			Get
				Return Me._underlyingField
			End Get
		End Property

		Public Sub New(ByVal underlyingField As FieldSymbol)
			MyBase.New()
			Me._underlyingField = underlyingField
		End Sub

		Private Sub AssociateWithMetadataWriter(ByVal metadataWriter As Microsoft.Cci.MetadataWriter) Implements IContextualNamedEntity.AssociateWithMetadataWriter
			DirectCast(Me._underlyingField, SynthesizedStaticLocalBackingField).AssociateWithMetadataWriter(metadataWriter)
		End Sub

		Public Overrides Sub Dispatch(ByVal visitor As MetadataVisitor)
			visitor.Visit(Me)
		End Sub

		Private Function IFieldReferenceGetResolvedField(ByVal context As EmitContext) As IFieldDefinition
			Return Nothing
		End Function

		Private Function IFieldReferenceGetType(ByVal context As EmitContext) As ITypeReference
			Dim modifiedTypeReference As ITypeReference
			Dim customModifiers As ImmutableArray(Of CustomModifier) = Me._underlyingField.CustomModifiers
			Dim typeReference As ITypeReference = DirectCast(context.[Module], PEModuleBuilder).Translate(Me._underlyingField.Type, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
			If (customModifiers.Length <> 0) Then
				modifiedTypeReference = New Microsoft.Cci.ModifiedTypeReference(typeReference, customModifiers.[As](Of ICustomModifier)())
			Else
				modifiedTypeReference = typeReference
			End If
			Return modifiedTypeReference
		End Function
	End Class
End Namespace
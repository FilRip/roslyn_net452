Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedStaticLocalBackingField
		Inherits SynthesizedFieldSymbol
		Implements IContextualNamedEntity
		Private _metadataWriter As MetadataWriter

		Private _nameToEmit As String

		Public ReadOnly IsValueField As Boolean

		Private ReadOnly _reportErrorForLongNames As Boolean

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Shadows ReadOnly Property ImplicitlyDefinedBy As LocalSymbol
			Get
				Return DirectCast(Me._implicitlyDefinedBy, LocalSymbol)
			End Get
		End Property

		Friend Overrides ReadOnly Property IsContextualNamedEntity As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._nameToEmit
			End Get
		End Property

		Public Sub New(ByVal implicitlyDefinedBy As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal isValueField As Boolean, ByVal reportErrorForLongNames As Boolean)
			MyBase.New(implicitlyDefinedBy.ContainingType, implicitlyDefinedBy, If(isValueField, implicitlyDefinedBy.Type, implicitlyDefinedBy.DeclaringCompilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_CompilerServices_StaticLocalInitFlag)), If(isValueField, implicitlyDefinedBy.Name, [String].Concat(implicitlyDefinedBy.Name, "$Init")), Accessibility.[Private], False, implicitlyDefinedBy.ContainingSymbol.IsShared, True)
			Me.IsValueField = isValueField
			Me._reportErrorForLongNames = reportErrorForLongNames
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
		End Sub

		Friend Sub AssociateWithMetadataWriter(ByVal metadataWriter As Microsoft.Cci.MetadataWriter)
			Interlocked.CompareExchange(Of Microsoft.Cci.MetadataWriter)(Me._metadataWriter, metadataWriter, Nothing)
			If (Me._nameToEmit Is Nothing) Then
				Dim containingSymbol As MethodSymbol = DirectCast(Me.ImplicitlyDefinedBy.ContainingSymbol, MethodSymbol)
				Dim str As String = GeneratedNames.MakeSignatureString(metadataWriter.GetMethodSignature(containingSymbol.GetCciAdapter()))
				Me._nameToEmit = GeneratedNames.MakeStaticLocalFieldName(containingSymbol.Name, str, Me.Name)
			End If
		End Sub

		Private Sub IContextualNamedEntity_AssociateWithMetadataWriter(ByVal metadataWriter As Microsoft.Cci.MetadataWriter) Implements IContextualNamedEntity.AssociateWithMetadataWriter
			DirectCast(MyBase.AdaptedFieldSymbol, SynthesizedStaticLocalBackingField).AssociateWithMetadataWriter(metadataWriter)
		End Sub
	End Class
End Namespace
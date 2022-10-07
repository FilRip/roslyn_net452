Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend NotInheritable Class RetargetingFieldSymbol
		Inherits FieldSymbol
		Private ReadOnly _retargetingModule As RetargetingModuleSymbol

		Private ReadOnly _underlyingField As FieldSymbol

		Private _lazyCustomModifiers As ImmutableArray(Of CustomModifier)

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyCachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me._underlyingField.AssociatedSymbol
				If (symbol Is Nothing) Then
					Return Nothing
				End If
				Return Me.RetargetingTranslator.Retarget(symbol)
			End Get
		End Property

		Public Overrides ReadOnly Property ConstantValue As Object
			Get
				Return Me._underlyingField.ConstantValue
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._retargetingModule.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me._retargetingModule
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingField.ContainingSymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.RetargetingTranslator.RetargetModifiers(Me._underlyingField.CustomModifiers, Me._lazyCustomModifiers)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._underlyingField.DeclaredAccessibility
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._underlyingField.DeclaringSyntaxReferences
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return Me._underlyingField.HasRuntimeSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._underlyingField.HasSpecialName
			End Get
		End Property

		Public Overrides ReadOnly Property IsConst As Boolean
			Get
				Return Me._underlyingField.IsConst
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._underlyingField.IsImplicitlyDeclared
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMarshalledExplicitly As Boolean
			Get
				Return Me._underlyingField.IsMarshalledExplicitly
			End Get
		End Property

		Friend Overrides ReadOnly Property IsNotSerialized As Boolean
			Get
				Return Me._underlyingField.IsNotSerialized
			End Get
		End Property

		Public Overrides ReadOnly Property IsReadOnly As Boolean
			Get
				Return Me._underlyingField.IsReadOnly
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me._underlyingField.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingField.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				Return Me._underlyingField.MarshallingDescriptor
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me.RetargetingTranslator.Retarget(Me.UnderlyingField.MarshallingInformation)
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._underlyingField.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingField.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me._underlyingField.ObsoleteAttributeData
			End Get
		End Property

		Public ReadOnly Property RetargetingModule As RetargetingModuleSymbol
			Get
				Return Me._retargetingModule
			End Get
		End Property

		Private ReadOnly Property RetargetingTranslator As RetargetingModuleSymbol.RetargetingSymbolTranslator
			Get
				Return Me._retargetingModule.RetargetingTranslator
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingField.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)
			Get
				Return Me._underlyingField.TypeLayoutOffset
			End Get
		End Property

		Public ReadOnly Property UnderlyingField As FieldSymbol
			Get
				Return Me._underlyingField
			End Get
		End Property

		Public Sub New(ByVal retargetingModule As RetargetingModuleSymbol, ByVal underlyingField As FieldSymbol)
			MyBase.New()
			Me._lazyCachedUseSiteInfo = CachedUseSiteInfo(Of AssemblySymbol).Uninitialized
			If (TypeOf underlyingField Is RetargetingFieldSymbol) Then
				Throw New ArgumentException()
			End If
			Me._retargetingModule = retargetingModule
			Me._underlyingField = underlyingField
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.GetRetargetedAttributes(Me._underlyingField, Me._lazyCustomAttributes, False)
		End Function

		Friend Overrides Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
			Return Me._underlyingField.GetConstantValue(inProgress)
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.RetargetAttributes(Me._underlyingField.GetCustomAttributesToEmit(compilationState))
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingField.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim primaryDependency As AssemblySymbol = MyBase.PrimaryDependency
			If (Not Me._lazyCachedUseSiteInfo.IsInitialized) Then
				Me._lazyCachedUseSiteInfo.Initialize(primaryDependency, MyBase.CalculateUseSiteInfo())
			End If
			Return Me._lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency)
		End Function
	End Class
End Namespace
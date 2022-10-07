Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend MustInherit Class RetargetingParameterSymbol
		Inherits ParameterSymbol
		Private ReadOnly _underlyingParameter As ParameterSymbol

		Private _lazyCustomModifiers As Microsoft.CodeAnalysis.CustomModifiersTuple

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me.RetargetingModule.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me.RetargetingModule
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingParameter.ContainingSymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.CustomModifiersTuple.TypeCustomModifiers
			End Get
		End Property

		Private ReadOnly Property CustomModifiersTuple As Microsoft.CodeAnalysis.CustomModifiersTuple
			Get
				Return Me.RetargetingTranslator.RetargetModifiers(Me._underlyingParameter.CustomModifiers, Me._underlyingParameter.RefCustomModifiers, Me._lazyCustomModifiers)
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._underlyingParameter.DeclaringSyntaxReferences
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				Return Me._underlyingParameter(inProgress)
			End Get
		End Property

		Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return Me._underlyingParameter.HasExplicitDefaultValue
			End Get
		End Property

		Friend Overrides ReadOnly Property HasMetadataConstantValue As Boolean
			Get
				Return Me._underlyingParameter.HasMetadataConstantValue
			End Get
		End Property

		Friend Overrides ReadOnly Property HasOptionCompare As Boolean
			Get
				Return Me._underlyingParameter.HasOptionCompare
			End Get
		End Property

		Public Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return Me._underlyingParameter.IsByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
			Get
				Return Me._underlyingParameter.IsCallerFilePath
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
			Get
				Return Me._underlyingParameter.IsCallerLineNumber
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
			Get
				Return Me._underlyingParameter.IsCallerMemberName
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return Me._underlyingParameter.IsExplicitByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
			Get
				Return Me._underlyingParameter.IsIDispatchConstant
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._underlyingParameter.IsImplicitlyDeclared
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
			Get
				Return Me._underlyingParameter.IsIUnknownConstant
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMarshalledExplicitly As Boolean
			Get
				Return Me._underlyingParameter.IsMarshalledExplicitly
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataIn As Boolean
			Get
				Return Me._underlyingParameter.IsMetadataIn
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataOptional As Boolean
			Get
				Return Me._underlyingParameter.IsMetadataOptional
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataOut As Boolean
			Get
				Return Me._underlyingParameter.IsMetadataOut
			End Get
		End Property

		Public Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return Me._underlyingParameter.IsOptional
			End Get
		End Property

		Public Overrides ReadOnly Property IsParamArray As Boolean
			Get
				Return Me._underlyingParameter.IsParamArray
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingParameter.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				Return Me._underlyingParameter.MarshallingDescriptor
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me.RetargetingTranslator.Retarget(Me.UnderlyingParameter.MarshallingInformation)
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._underlyingParameter.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingParameter.Name
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._underlyingParameter.Ordinal
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.CustomModifiersTuple.RefCustomModifiers
			End Get
		End Property

		Protected MustOverride ReadOnly Property RetargetingModule As RetargetingModuleSymbol

		Private ReadOnly Property RetargetingTranslator As RetargetingModuleSymbol.RetargetingSymbolTranslator
			Get
				Return Me.RetargetingModule.RetargetingTranslator
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingParameter.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
			End Get
		End Property

		Public ReadOnly Property UnderlyingParameter As ParameterSymbol
			Get
				Return Me._underlyingParameter
			End Get
		End Property

		Protected Sub New(ByVal underlyingParameter As ParameterSymbol)
			MyBase.New()
			If (TypeOf underlyingParameter Is RetargetingParameterSymbol) Then
				Throw New ArgumentException()
			End If
			Me._underlyingParameter = underlyingParameter
		End Sub

		Public Shared Function CreateMethodParameter(ByVal retargetingMethod As RetargetingMethodSymbol, ByVal underlyingParameter As ParameterSymbol) As RetargetingParameterSymbol
			Return New RetargetingParameterSymbol.RetargetingMethodParameterSymbol(retargetingMethod, underlyingParameter)
		End Function

		Public Shared Function CreatePropertyParameter(ByVal retargetingProperty As RetargetingPropertySymbol, ByVal underlyingParameter As ParameterSymbol) As RetargetingParameterSymbol
			Return New RetargetingParameterSymbol.RetargetingPropertyParameterSymbol(retargetingProperty, underlyingParameter)
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.GetRetargetedAttributes(Me._underlyingParameter, Me._lazyCustomAttributes, False)
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.RetargetAttributes(Me._underlyingParameter.GetCustomAttributesToEmit(compilationState))
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Private NotInheritable Class RetargetingMethodParameterSymbol
			Inherits RetargetingParameterSymbol
			Private ReadOnly _retargetingMethod As RetargetingMethodSymbol

			Protected Overrides ReadOnly Property RetargetingModule As RetargetingModuleSymbol
				Get
					Return Me._retargetingMethod.RetargetingModule
				End Get
			End Property

			Public Sub New(ByVal retargetingMethod As RetargetingMethodSymbol, ByVal underlyingParameter As ParameterSymbol)
				MyBase.New(underlyingParameter)
				Me._retargetingMethod = retargetingMethod
			End Sub
		End Class

		Private NotInheritable Class RetargetingPropertyParameterSymbol
			Inherits RetargetingParameterSymbol
			Private ReadOnly _retargetingProperty As RetargetingPropertySymbol

			Protected Overrides ReadOnly Property RetargetingModule As RetargetingModuleSymbol
				Get
					Return Me._retargetingProperty.RetargetingModule
				End Get
			End Property

			Public Sub New(ByVal retargetingProperty As RetargetingPropertySymbol, ByVal underlyingParameter As ParameterSymbol)
				MyBase.New(underlyingParameter)
				Me._retargetingProperty = retargetingProperty
			End Sub
		End Class
	End Class
End Namespace
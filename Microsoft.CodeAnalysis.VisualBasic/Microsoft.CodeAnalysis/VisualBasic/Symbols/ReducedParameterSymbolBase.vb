Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class ReducedParameterSymbolBase
		Inherits ParameterSymbol
		Protected ReadOnly m_CurriedFromParameter As ParameterSymbol

		Public Overrides ReadOnly Property ContainingSymbol As Symbol

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.m_CurriedFromParameter.CustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me.m_CurriedFromParameter.DeclaringSyntaxReferences
			End Get
		End Property

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				Return Me.m_CurriedFromParameter(inProgress)
			End Get
		End Property

		Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				Return Me.m_CurriedFromParameter.HasExplicitDefaultValue
			End Get
		End Property

		Friend Overrides ReadOnly Property HasOptionCompare As Boolean
			Get
				Return Me.m_CurriedFromParameter.HasOptionCompare
			End Get
		End Property

		Public Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsCallerFilePath
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsCallerLineNumber
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsCallerMemberName
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsExplicitByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsIDispatchConstant
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsImplicitlyDeclared
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsIUnknownConstant
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMetadataIn As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsMetadataIn
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMetadataOut As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsMetadataOut
			End Get
		End Property

		Public Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsOptional
			End Get
		End Property

		Public Overrides ReadOnly Property IsParamArray As Boolean
			Get
				Return Me.m_CurriedFromParameter.IsParamArray
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me.m_CurriedFromParameter.Locations
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me.m_CurriedFromParameter.MarshallingInformation
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me.m_CurriedFromParameter.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me.m_CurriedFromParameter.Name
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me.m_CurriedFromParameter.Ordinal - 1
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.m_CurriedFromParameter.RefCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol

		Protected Sub New(ByVal curriedFromParameter As ParameterSymbol)
			MyBase.New()
			Me.m_CurriedFromParameter = curriedFromParameter
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (Me <> obj) Then
				Dim reducedParameterSymbolBase As Microsoft.CodeAnalysis.VisualBasic.Symbols.ReducedParameterSymbolBase = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.ReducedParameterSymbolBase)
				flag = If(reducedParameterSymbolBase Is Nothing OrElse Not reducedParameterSymbolBase.m_CurriedFromParameter.Equals(Me.m_CurriedFromParameter), False, reducedParameterSymbolBase.ContainingSymbol.Equals(Me.ContainingSymbol))
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.m_CurriedFromParameter.GetAttributes()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me.m_CurriedFromParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Me.m_CurriedFromParameter.GetHashCode(), Me.ContainingSymbol.GetHashCode())
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Return Me.m_CurriedFromParameter.GetUseSiteInfo()
		End Function
	End Class
End Namespace
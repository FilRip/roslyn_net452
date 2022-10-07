Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class WrappedParameterSymbol
		Inherits ParameterSymbol
		Protected _underlyingParameter As ParameterSymbol

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._underlyingParameter.CustomModifiers
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

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me._underlyingParameter.MarshallingInformation
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingType As UnmanagedType
			Get
				Return Me._underlyingParameter.MarshallingType
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
				Return Me._underlyingParameter.RefCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._underlyingParameter.Type
			End Get
		End Property

		Public ReadOnly Property UnderlyingParameter As ParameterSymbol
			Get
				Return Me._underlyingParameter
			End Get
		End Property

		Protected Sub New(ByVal underlyingParameter As ParameterSymbol)
			MyBase.New()
			Me._underlyingParameter = underlyingParameter
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			Me._underlyingParameter.AddSynthesizedAttributes(compilationState, attributes)
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me._underlyingParameter.GetAttributes()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function
	End Class
End Namespace
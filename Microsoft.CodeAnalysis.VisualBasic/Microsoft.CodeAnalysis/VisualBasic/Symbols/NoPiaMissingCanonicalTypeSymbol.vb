Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class NoPiaMissingCanonicalTypeSymbol
		Inherits ErrorTypeSymbol
		Private ReadOnly _embeddingAssembly As AssemblySymbol

		Private ReadOnly _guid As String

		Private ReadOnly _scope As String

		Private ReadOnly _identifier As String

		Private ReadOnly _fullTypeName As String

		Public ReadOnly Property EmbeddingAssembly As AssemblySymbol
			Get
				Return Me._embeddingAssembly
			End Get
		End Property

		Friend Overrides ReadOnly Property ErrorInfo As DiagnosticInfo
			Get
				Return ErrorFactory.ErrorInfo(ERRID.ERR_AbsentReferenceToPIA1, New [Object]() { Me._fullTypeName })
			End Get
		End Property

		Public ReadOnly Property FullTypeName As String
			Get
				Return Me._fullTypeName
			End Get
		End Property

		Public ReadOnly Property Guid As String
			Get
				Return Me._guid
			End Get
		End Property

		Public ReadOnly Property Identifier As String
			Get
				Return Me._identifier
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._fullTypeName
			End Get
		End Property

		Public ReadOnly Property Scope As String
			Get
				Return Me._scope
			End Get
		End Property

		Public Sub New(ByVal embeddingAssembly As AssemblySymbol, ByVal fullTypeName As String, ByVal guid As String, ByVal scope As String, ByVal identifier As String)
			MyBase.New()
			Me._fullTypeName = fullTypeName
			Me._embeddingAssembly = embeddingAssembly
			Me._guid = guid
			Me._scope = scope
			Me._identifier = identifier
		End Sub

		Public Overrides Function Equals(ByVal obj As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			Return obj = Me
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return RuntimeHelpers.GetHashCode(Me)
		End Function
	End Class
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourceTypeParameterOnMethodSymbol
		Inherits SourceTypeParameterSymbol
		Private ReadOnly _container As SourceMemberMethodSymbol

		Private ReadOnly _syntaxRef As SyntaxReference

		Protected Overrides ReadOnly Property ContainerTypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return Me._container.TypeParameters
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._container
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Me._syntaxRef)
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray.Create(Of Location)(SourceTypeParameterSymbol.GetSymbolLocation(Me._syntaxRef))
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameterKind As Microsoft.CodeAnalysis.TypeParameterKind
			Get
				Return Microsoft.CodeAnalysis.TypeParameterKind.Method
			End Get
		End Property

		Public Overrides ReadOnly Property Variance As VarianceKind
			Get
				Return VarianceKind.None
			End Get
		End Property

		Public Sub New(ByVal container As SourceMemberMethodSymbol, ByVal ordinal As Integer, ByVal name As String, ByVal syntaxRef As SyntaxReference)
			MyBase.New(ordinal, name)
			Me._container = container
			Me._syntaxRef = syntaxRef
		End Sub

		Protected Overrides Function GetDeclaredConstraints(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of TypeParameterConstraint)
			Dim syntax As TypeParameterSyntax = DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), TypeParameterSyntax)
			Return Me._container.BindTypeParameterConstraints(syntax, diagnostics)
		End Function

		Protected Overrides Function ReportRedundantConstraints() As Boolean
			Dim flag As Boolean
			If (Not Me._container.IsOverrides) Then
				flag = If(Me._container.DeclaredAccessibility <> Accessibility.[Private] OrElse Not Me._container.HasExplicitInterfaceImplementations(), True, False)
			Else
				flag = False
			End If
			Return flag
		End Function
	End Class
End Namespace
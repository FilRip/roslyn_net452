Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class DecimalLiteralTokenSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
		Friend ReadOnly _typeSuffix As TypeCharacter

		Friend ReadOnly _value As [Decimal]

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend NotOverridable Overrides ReadOnly Property ObjectValue As Object
			Get
				Return Me.Value
			End Get
		End Property

		Friend ReadOnly Property TypeSuffix As TypeCharacter
			Get
				Return Me._typeSuffix
			End Get
		End Property

		Friend ReadOnly Property Value As [Decimal]
			Get
				Return Me._value
			End Get
		End Property

		Shared Sub New()
			DecimalLiteralTokenSyntax.CreateInstance = Function(o As ObjectReader) New DecimalLiteralTokenSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(DecimalLiteralTokenSyntax), Function(r As ObjectReader) New DecimalLiteralTokenSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal typeSuffix As TypeCharacter, ByVal value As [Decimal])
			MyBase.New(kind, text, leadingTrivia, trailingTrivia)
			Me._typeSuffix = typeSuffix
			Me._value = value
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal typeSuffix As TypeCharacter, ByVal value As [Decimal], ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, text, leadingTrivia, trailingTrivia)
			MyBase.SetFactoryContext(context)
			Me._typeSuffix = typeSuffix
			Me._value = value
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal typeSuffix As TypeCharacter, ByVal value As [Decimal])
			MyBase.New(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
			Me._typeSuffix = typeSuffix
			Me._value = value
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Me._typeSuffix = reader.ReadInt32()
			Me._value = Microsoft.VisualBasic.CompilerServices.Conversions.ToDecimal(reader.ReadValue())
		End Sub

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New DecimalLiteralTokenSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia(), Me._typeSuffix, Me._value)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New DecimalLiteralTokenSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia(), Me._typeSuffix, Me._value)
		End Function

		Public Overrides Function WithLeadingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New DecimalLiteralTokenSyntax(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, trivia, MyBase.GetTrailingTrivia(), Me._typeSuffix, Me._value)
		End Function

		Public Overrides Function WithTrailingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New DecimalLiteralTokenSyntax(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), trivia, Me._typeSuffix, Me._value)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteInt32(CInt(Me._typeSuffix))
			writer.WriteValue(Me._value)
		End Sub
	End Class
End Namespace
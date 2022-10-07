Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class LiteralExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Get
				Return Me._token
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			MyBase.New(kind)
			MyBase._slotCount = 1
			MyBase.AdjustFlagsAndWidth(token)
			Me._token = token
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 1
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(token)
			Me._token = token
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 1
			MyBase.AdjustFlagsAndWidth(token)
			Me._token = token
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 1
			Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
			If (syntaxToken IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(syntaxToken)
				Me._token = syntaxToken
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitLiteralExpression(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			If (i <> 0) Then
				greenNode = Nothing
			Else
				greenNode = Me._token
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._token)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._token)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._token, IObjectWritable))
		End Sub
	End Class
End Namespace
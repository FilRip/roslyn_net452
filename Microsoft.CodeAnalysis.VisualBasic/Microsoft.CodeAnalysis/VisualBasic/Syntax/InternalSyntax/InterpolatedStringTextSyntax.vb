Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class InterpolatedStringTextSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax
		Friend ReadOnly _textToken As InterpolatedStringTextTokenSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property TextToken As InterpolatedStringTextTokenSyntax
			Get
				Return Me._textToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal textToken As InterpolatedStringTextTokenSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 1
			MyBase.AdjustFlagsAndWidth(textToken)
			Me._textToken = textToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal textToken As InterpolatedStringTextTokenSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 1
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(textToken)
			Me._textToken = textToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal textToken As InterpolatedStringTextTokenSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 1
			MyBase.AdjustFlagsAndWidth(textToken)
			Me._textToken = textToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 1
			Dim interpolatedStringTextTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextTokenSyntax)
			If (interpolatedStringTextTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(interpolatedStringTextTokenSyntax)
				Me._textToken = interpolatedStringTextTokenSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitInterpolatedStringText(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterpolatedStringTextSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			If (i <> 0) Then
				greenNode = Nothing
			Else
				greenNode = Me._textToken
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._textToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._textToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._textToken, IObjectWritable))
		End Sub
	End Class
End Namespace
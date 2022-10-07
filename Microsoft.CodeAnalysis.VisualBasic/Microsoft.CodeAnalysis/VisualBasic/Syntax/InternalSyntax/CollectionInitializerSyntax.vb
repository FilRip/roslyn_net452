Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class CollectionInitializerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _openBraceToken As PunctuationSyntax

		Friend ReadOnly _initializers As GreenNode

		Friend ReadOnly _closeBraceToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property CloseBraceToken As PunctuationSyntax
			Get
				Return Me._closeBraceToken
			End Get
		End Property

		Friend ReadOnly Property Initializers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)(Me._initializers))
			End Get
		End Property

		Friend ReadOnly Property OpenBraceToken As PunctuationSyntax
			Get
				Return Me._openBraceToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal openBraceToken As PunctuationSyntax, ByVal initializers As GreenNode, ByVal closeBraceToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			If (initializers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(initializers)
				Me._initializers = initializers
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal openBraceToken As PunctuationSyntax, ByVal initializers As GreenNode, ByVal closeBraceToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			If (initializers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(initializers)
				Me._initializers = initializers
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openBraceToken As PunctuationSyntax, ByVal initializers As GreenNode, ByVal closeBraceToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			If (initializers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(initializers)
				Me._initializers = initializers
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._openBraceToken = punctuationSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._initializers = greenNode
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._closeBraceToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitCollectionInitializer(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._openBraceToken
					Exit Select
				Case 1
					greenNode = Me._initializers
					Exit Select
				Case 2
					greenNode = Me._closeBraceToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._openBraceToken, Me._initializers, Me._closeBraceToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._openBraceToken, Me._initializers, Me._closeBraceToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._openBraceToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._initializers, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeBraceToken, IObjectWritable))
		End Sub
	End Class
End Namespace
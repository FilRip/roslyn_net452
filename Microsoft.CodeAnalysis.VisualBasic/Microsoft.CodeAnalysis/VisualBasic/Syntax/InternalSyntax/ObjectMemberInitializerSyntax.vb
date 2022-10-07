Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ObjectMemberInitializerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationInitializerSyntax
		Friend ReadOnly _withKeyword As KeywordSyntax

		Friend ReadOnly _openBraceToken As PunctuationSyntax

		Friend ReadOnly _initializers As GreenNode

		Friend ReadOnly _closeBraceToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property CloseBraceToken As PunctuationSyntax
			Get
				Return Me._closeBraceToken
			End Get
		End Property

		Friend ReadOnly Property Initializers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax)(Me._initializers))
			End Get
		End Property

		Friend ReadOnly Property OpenBraceToken As PunctuationSyntax
			Get
				Return Me._openBraceToken
			End Get
		End Property

		Friend ReadOnly Property WithKeyword As KeywordSyntax
			Get
				Return Me._withKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal withKeyword As KeywordSyntax, ByVal openBraceToken As PunctuationSyntax, ByVal initializers As GreenNode, ByVal closeBraceToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(withKeyword)
			Me._withKeyword = withKeyword
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			If (initializers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(initializers)
				Me._initializers = initializers
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal withKeyword As KeywordSyntax, ByVal openBraceToken As PunctuationSyntax, ByVal initializers As GreenNode, ByVal closeBraceToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(withKeyword)
			Me._withKeyword = withKeyword
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			If (initializers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(initializers)
				Me._initializers = initializers
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal withKeyword As KeywordSyntax, ByVal openBraceToken As PunctuationSyntax, ByVal initializers As GreenNode, ByVal closeBraceToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(withKeyword)
			Me._withKeyword = withKeyword
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
			MyBase._slotCount = 4
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._withKeyword = keywordSyntax
			End If
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
			Return visitor.VisitObjectMemberInitializer(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._withKeyword
					Exit Select
				Case 1
					greenNode = Me._openBraceToken
					Exit Select
				Case 2
					greenNode = Me._initializers
					Exit Select
				Case 3
					greenNode = Me._closeBraceToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._withKeyword, Me._openBraceToken, Me._initializers, Me._closeBraceToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._withKeyword, Me._openBraceToken, Me._initializers, Me._closeBraceToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._withKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._openBraceToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._initializers, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeBraceToken, IObjectWritable))
		End Sub
	End Class
End Namespace
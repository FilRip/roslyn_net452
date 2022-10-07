Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlMemberAccessExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _token1 As PunctuationSyntax

		Friend ReadOnly _token2 As PunctuationSyntax

		Friend ReadOnly _token3 As PunctuationSyntax

		Friend ReadOnly _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._base
			End Get
		End Property

		Friend ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Get
				Return Me._name
			End Get
		End Property

		Friend ReadOnly Property Token1 As PunctuationSyntax
			Get
				Return Me._token1
			End Get
		End Property

		Friend ReadOnly Property Token2 As PunctuationSyntax
			Get
				Return Me._token2
			End Get
		End Property

		Friend ReadOnly Property Token3 As PunctuationSyntax
			Get
				Return Me._token3
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 5
			If (base IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(base)
				Me._base = base
			End If
			MyBase.AdjustFlagsAndWidth(token1)
			Me._token1 = token1
			If (token2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(token2)
				Me._token2 = token2
			End If
			If (token3 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(token3)
				Me._token3 = token3
			End If
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			If (base IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(base)
				Me._base = base
			End If
			MyBase.AdjustFlagsAndWidth(token1)
			Me._token1 = token1
			If (token2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(token2)
				Me._token2 = token2
			End If
			If (token3 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(token3)
				Me._token3 = token3
			End If
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal base As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal token1 As PunctuationSyntax, ByVal token2 As PunctuationSyntax, ByVal token3 As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 5
			If (base IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(base)
				Me._base = base
			End If
			MyBase.AdjustFlagsAndWidth(token1)
			Me._token1 = token1
			If (token2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(token2)
				Me._token2 = token2
			End If
			If (token3 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(token3)
				Me._token3 = token3
			End If
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._base = expressionSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._token1 = punctuationSyntax
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._token2 = punctuationSyntax1
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax2)
				Me._token3 = punctuationSyntax2
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (xmlNodeSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlNodeSyntax)
				Me._name = xmlNodeSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlMemberAccessExpression(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._base
					Exit Select
				Case 1
					greenNode = Me._token1
					Exit Select
				Case 2
					greenNode = Me._token2
					Exit Select
				Case 3
					greenNode = Me._token3
					Exit Select
				Case 4
					greenNode = Me._name
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._base, Me._token1, Me._token2, Me._token3, Me._name)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._base, Me._token1, Me._token2, Me._token3, Me._name)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._base, IObjectWritable))
			writer.WriteValue(DirectCast(Me._token1, IObjectWritable))
			writer.WriteValue(DirectCast(Me._token2, IObjectWritable))
			writer.WriteValue(DirectCast(Me._token3, IObjectWritable))
			writer.WriteValue(DirectCast(Me._name, IObjectWritable))
		End Sub
	End Class
End Namespace
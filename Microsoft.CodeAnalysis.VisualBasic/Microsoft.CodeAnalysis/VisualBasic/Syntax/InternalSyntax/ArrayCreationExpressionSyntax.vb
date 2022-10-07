Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ArrayCreationExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax
		Friend ReadOnly _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax

		Friend ReadOnly _arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax

		Friend ReadOnly _rankSpecifiers As GreenNode

		Friend ReadOnly _initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property ArrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax
			Get
				Return Me._arrayBounds
			End Get
		End Property

		Friend ReadOnly Property Initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax
			Get
				Return Me._initializer
			End Get
		End Property

		Friend ReadOnly Property RankSpecifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)(Me._rankSpecifiers)
			End Get
		End Property

		Friend ReadOnly Property Type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Get
				Return Me._type
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal newKeyword As KeywordSyntax, ByVal attributeLists As GreenNode, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal rankSpecifiers As GreenNode, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)
			MyBase.New(kind, newKeyword, attributeLists)
			MyBase._slotCount = 6
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
			If (arrayBounds IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arrayBounds)
				Me._arrayBounds = arrayBounds
			End If
			If (rankSpecifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(rankSpecifiers)
				Me._rankSpecifiers = rankSpecifiers
			End If
			MyBase.AdjustFlagsAndWidth(initializer)
			Me._initializer = initializer
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal newKeyword As KeywordSyntax, ByVal attributeLists As GreenNode, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal rankSpecifiers As GreenNode, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, newKeyword, attributeLists)
			MyBase._slotCount = 6
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
			If (arrayBounds IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arrayBounds)
				Me._arrayBounds = arrayBounds
			End If
			If (rankSpecifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(rankSpecifiers)
				Me._rankSpecifiers = rankSpecifiers
			End If
			MyBase.AdjustFlagsAndWidth(initializer)
			Me._initializer = initializer
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal newKeyword As KeywordSyntax, ByVal attributeLists As GreenNode, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal rankSpecifiers As GreenNode, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)
			MyBase.New(kind, errors, annotations, newKeyword, attributeLists)
			MyBase._slotCount = 6
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
			If (arrayBounds IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arrayBounds)
				Me._arrayBounds = arrayBounds
			End If
			If (rankSpecifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(rankSpecifiers)
				Me._rankSpecifiers = rankSpecifiers
			End If
			MyBase.AdjustFlagsAndWidth(initializer)
			Me._initializer = initializer
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 6
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (typeSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeSyntax)
				Me._type = typeSyntax
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			If (argumentListSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(argumentListSyntax)
				Me._arrayBounds = argumentListSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._rankSpecifiers = greenNode
			End If
			Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)
			If (collectionInitializerSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(collectionInitializerSyntax)
				Me._initializer = collectionInitializerSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitArrayCreationExpression(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayCreationExpressionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._newKeyword
					Exit Select
				Case 1
					greenNode = Me._attributeLists
					Exit Select
				Case 2
					greenNode = Me._type
					Exit Select
				Case 3
					greenNode = Me._arrayBounds
					Exit Select
				Case 4
					greenNode = Me._rankSpecifiers
					Exit Select
				Case 5
					greenNode = Me._initializer
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._newKeyword, Me._attributeLists, Me._type, Me._arrayBounds, Me._rankSpecifiers, Me._initializer)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._newKeyword, Me._attributeLists, Me._type, Me._arrayBounds, Me._rankSpecifiers, Me._initializer)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._type, IObjectWritable))
			writer.WriteValue(DirectCast(Me._arrayBounds, IObjectWritable))
			writer.WriteValue(DirectCast(Me._rankSpecifiers, IObjectWritable))
			writer.WriteValue(DirectCast(Me._initializer, IObjectWritable))
		End Sub
	End Class
End Namespace
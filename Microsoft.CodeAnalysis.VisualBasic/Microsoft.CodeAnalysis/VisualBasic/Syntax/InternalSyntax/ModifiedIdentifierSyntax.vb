Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ModifiedIdentifierSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _identifier As IdentifierTokenSyntax

		Friend ReadOnly _nullable As PunctuationSyntax

		Friend ReadOnly _arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax

		Friend ReadOnly _arrayRankSpecifiers As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property ArrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax
			Get
				Return Me._arrayBounds
			End Get
		End Property

		Friend ReadOnly Property ArrayRankSpecifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax)(Me._arrayRankSpecifiers)
			End Get
		End Property

		Friend ReadOnly Property Identifier As IdentifierTokenSyntax
			Get
				Return Me._identifier
			End Get
		End Property

		Friend ReadOnly Property Nullable As PunctuationSyntax
			Get
				Return Me._nullable
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal identifier As IdentifierTokenSyntax, ByVal nullable As PunctuationSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal arrayRankSpecifiers As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (nullable IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nullable)
				Me._nullable = nullable
			End If
			If (arrayBounds IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arrayBounds)
				Me._arrayBounds = arrayBounds
			End If
			If (arrayRankSpecifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arrayRankSpecifiers)
				Me._arrayRankSpecifiers = arrayRankSpecifiers
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal identifier As IdentifierTokenSyntax, ByVal nullable As PunctuationSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal arrayRankSpecifiers As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (nullable IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nullable)
				Me._nullable = nullable
			End If
			If (arrayBounds IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arrayBounds)
				Me._arrayBounds = arrayBounds
			End If
			If (arrayRankSpecifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arrayRankSpecifiers)
				Me._arrayRankSpecifiers = arrayRankSpecifiers
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal identifier As IdentifierTokenSyntax, ByVal nullable As PunctuationSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax, ByVal arrayRankSpecifiers As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (nullable IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nullable)
				Me._nullable = nullable
			End If
			If (arrayBounds IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arrayBounds)
				Me._arrayBounds = arrayBounds
			End If
			If (arrayRankSpecifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(arrayRankSpecifiers)
				Me._arrayRankSpecifiers = arrayRankSpecifiers
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 4
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (identifierTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(identifierTokenSyntax)
				Me._identifier = identifierTokenSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._nullable = punctuationSyntax
			End If
			Dim argumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)
			If (argumentListSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(argumentListSyntax)
				Me._arrayBounds = argumentListSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._arrayRankSpecifiers = greenNode
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitModifiedIdentifier(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._identifier
					Exit Select
				Case 1
					greenNode = Me._nullable
					Exit Select
				Case 2
					greenNode = Me._arrayBounds
					Exit Select
				Case 3
					greenNode = Me._arrayRankSpecifiers
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._identifier, Me._nullable, Me._arrayBounds, Me._arrayRankSpecifiers)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._identifier, Me._nullable, Me._arrayBounds, Me._arrayRankSpecifiers)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._identifier, IObjectWritable))
			writer.WriteValue(DirectCast(Me._nullable, IObjectWritable))
			writer.WriteValue(DirectCast(Me._arrayBounds, IObjectWritable))
			writer.WriteValue(DirectCast(Me._arrayRankSpecifiers, IObjectWritable))
		End Sub
	End Class
End Namespace
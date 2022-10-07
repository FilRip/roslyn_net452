Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class VariableDeclaratorSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _names As GreenNode

		Friend ReadOnly _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax

		Friend ReadOnly _initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax
			Get
				Return Me._asClause
			End Get
		End Property

		Friend ReadOnly Property Initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax
			Get
				Return Me._initializer
			End Get
		End Property

		Friend ReadOnly Property Names As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)(Me._names))
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal names As GreenNode, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			If (names IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(names)
				Me._names = names
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If (initializer IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(initializer)
				Me._initializer = initializer
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal names As GreenNode, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			If (names IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(names)
				Me._names = names
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If (initializer IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(initializer)
				Me._initializer = initializer
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal names As GreenNode, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			If (names IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(names)
				Me._names = names
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If (initializer IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(initializer)
				Me._initializer = initializer
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._names = greenNode
			End If
			Dim asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax)
			If (asClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClauseSyntax)
				Me._asClause = asClauseSyntax
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			If (equalsValueSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(equalsValueSyntax)
				Me._initializer = equalsValueSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitVariableDeclarator(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._names
					Exit Select
				Case 1
					greenNode = Me._asClause
					Exit Select
				Case 2
					greenNode = Me._initializer
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._names, Me._asClause, Me._initializer)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._names, Me._asClause, Me._initializer)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._names, IObjectWritable))
			writer.WriteValue(DirectCast(Me._asClause, IObjectWritable))
			writer.WriteValue(DirectCast(Me._initializer, IObjectWritable))
		End Sub
	End Class
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class CompilationUnitSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _options As GreenNode

		Friend ReadOnly _imports As GreenNode

		Friend ReadOnly _attributes As GreenNode

		Friend ReadOnly _members As GreenNode

		Friend ReadOnly _endOfFileToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax)(Me._attributes)
			End Get
		End Property

		Friend ReadOnly Property EndOfFileToken As PunctuationSyntax
			Get
				Return Me._endOfFileToken
			End Get
		End Property

		Friend ReadOnly Property [Imports] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax)(Me._imports)
			End Get
		End Property

		Friend ReadOnly Property Members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Me._members)
			End Get
		End Property

		Friend ReadOnly Property Options As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)(Me._options)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal options As GreenNode, ByVal [imports] As GreenNode, ByVal attributes As GreenNode, ByVal members As GreenNode, ByVal endOfFileToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 5
			If (options IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(options)
				Me._options = options
			End If
			If ([imports] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([imports])
				Me._imports = [imports]
			End If
			If (attributes IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributes)
				Me._attributes = attributes
			End If
			If (members IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(members)
				Me._members = members
			End If
			MyBase.AdjustFlagsAndWidth(endOfFileToken)
			Me._endOfFileToken = endOfFileToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal options As GreenNode, ByVal [imports] As GreenNode, ByVal attributes As GreenNode, ByVal members As GreenNode, ByVal endOfFileToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			If (options IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(options)
				Me._options = options
			End If
			If ([imports] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([imports])
				Me._imports = [imports]
			End If
			If (attributes IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributes)
				Me._attributes = attributes
			End If
			If (members IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(members)
				Me._members = members
			End If
			MyBase.AdjustFlagsAndWidth(endOfFileToken)
			Me._endOfFileToken = endOfFileToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal options As GreenNode, ByVal [imports] As GreenNode, ByVal attributes As GreenNode, ByVal members As GreenNode, ByVal endOfFileToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 5
			If (options IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(options)
				Me._options = options
			End If
			If ([imports] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([imports])
				Me._imports = [imports]
			End If
			If (attributes IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributes)
				Me._attributes = attributes
			End If
			If (members IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(members)
				Me._members = members
			End If
			MyBase.AdjustFlagsAndWidth(endOfFileToken)
			Me._endOfFileToken = endOfFileToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._options = greenNode
			End If
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
				Me._imports = greenNode1
			End If
			Dim greenNode2 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode2)
				Me._attributes = greenNode2
			End If
			Dim greenNode3 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode3 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode3)
				Me._members = greenNode3
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._endOfFileToken = punctuationSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitCompilationUnit(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._options
					Exit Select
				Case 1
					greenNode = Me._imports
					Exit Select
				Case 2
					greenNode = Me._attributes
					Exit Select
				Case 3
					greenNode = Me._members
					Exit Select
				Case 4
					greenNode = Me._endOfFileToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._options, Me._imports, Me._attributes, Me._members, Me._endOfFileToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._options, Me._imports, Me._attributes, Me._members, Me._endOfFileToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._options, IObjectWritable))
			writer.WriteValue(DirectCast(Me._imports, IObjectWritable))
			writer.WriteValue(DirectCast(Me._attributes, IObjectWritable))
			writer.WriteValue(DirectCast(Me._members, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endOfFileToken, IObjectWritable))
		End Sub
	End Class
End Namespace
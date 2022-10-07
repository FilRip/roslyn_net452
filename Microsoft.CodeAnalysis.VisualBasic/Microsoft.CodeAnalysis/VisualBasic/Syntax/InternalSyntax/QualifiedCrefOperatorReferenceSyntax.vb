Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class QualifiedCrefOperatorReferenceSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax
		Friend ReadOnly _left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax

		Friend ReadOnly _dotToken As PunctuationSyntax

		Friend ReadOnly _right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property DotToken As PunctuationSyntax
			Get
				Return Me._dotToken
			End Get
		End Property

		Friend ReadOnly Property Left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax
			Get
				Return Me._left
			End Get
		End Property

		Friend ReadOnly Property Right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax
			Get
				Return Me._right
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax, ByVal dotToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(left)
			Me._left = left
			MyBase.AdjustFlagsAndWidth(dotToken)
			Me._dotToken = dotToken
			MyBase.AdjustFlagsAndWidth(right)
			Me._right = right
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax, ByVal dotToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(left)
			Me._left = left
			MyBase.AdjustFlagsAndWidth(dotToken)
			Me._dotToken = dotToken
			MyBase.AdjustFlagsAndWidth(right)
			Me._right = right
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal left As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax, ByVal dotToken As PunctuationSyntax, ByVal right As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(left)
			Me._left = left
			MyBase.AdjustFlagsAndWidth(dotToken)
			Me._dotToken = dotToken
			MyBase.AdjustFlagsAndWidth(right)
			Me._right = right
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim nameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)
			If (nameSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nameSyntax)
				Me._left = nameSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._dotToken = punctuationSyntax
			End If
			Dim crefOperatorReferenceSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)
			If (crefOperatorReferenceSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(crefOperatorReferenceSyntax)
				Me._right = crefOperatorReferenceSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitQualifiedCrefOperatorReference(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedCrefOperatorReferenceSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._left
					Exit Select
				Case 1
					greenNode = Me._dotToken
					Exit Select
				Case 2
					greenNode = Me._right
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._left, Me._dotToken, Me._right)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._left, Me._dotToken, Me._right)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._left, IObjectWritable))
			writer.WriteValue(DirectCast(Me._dotToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._right, IObjectWritable))
		End Sub
	End Class
End Namespace
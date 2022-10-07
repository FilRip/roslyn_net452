Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class CrefReferenceSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax

		Friend ReadOnly _signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax

		Friend ReadOnly _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax
			Get
				Return Me._asClause
			End Get
		End Property

		Friend ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Get
				Return Me._name
			End Get
		End Property

		Friend ReadOnly Property Signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax
			Get
				Return Me._signature
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			If (signature IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(signature)
				Me._signature = signature
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			If (signature IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(signature)
				Me._signature = signature
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal signature As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			If (signature IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(signature)
				Me._signature = signature
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (typeSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeSyntax)
				Me._name = typeSyntax
			End If
			Dim crefSignatureSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax)
			If (crefSignatureSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(crefSignatureSyntax)
				Me._signature = crefSignatureSyntax
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (simpleAsClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(simpleAsClauseSyntax)
				Me._asClause = simpleAsClauseSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitCrefReference(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._name
					Exit Select
				Case 1
					greenNode = Me._signature
					Exit Select
				Case 2
					greenNode = Me._asClause
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._name, Me._signature, Me._asClause)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._name, Me._signature, Me._asClause)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._name, IObjectWritable))
			writer.WriteValue(DirectCast(Me._signature, IObjectWritable))
			writer.WriteValue(DirectCast(Me._asClause, IObjectWritable))
		End Sub
	End Class
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlDocumentSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
		Friend ReadOnly _declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax

		Friend ReadOnly _precedingMisc As GreenNode

		Friend ReadOnly _root As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax

		Friend ReadOnly _followingMisc As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax
			Get
				Return Me._declaration
			End Get
		End Property

		Friend ReadOnly Property FollowingMisc As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(Me._followingMisc)
			End Get
		End Property

		Friend ReadOnly Property PrecedingMisc As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(Me._precedingMisc)
			End Get
		End Property

		Friend ReadOnly Property Root As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Get
				Return Me._root
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax, ByVal precedingMisc As GreenNode, ByVal root As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal followingMisc As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(declaration)
			Me._declaration = declaration
			If (precedingMisc IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(precedingMisc)
				Me._precedingMisc = precedingMisc
			End If
			MyBase.AdjustFlagsAndWidth(root)
			Me._root = root
			If (followingMisc IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(followingMisc)
				Me._followingMisc = followingMisc
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax, ByVal precedingMisc As GreenNode, ByVal root As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal followingMisc As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(declaration)
			Me._declaration = declaration
			If (precedingMisc IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(precedingMisc)
				Me._precedingMisc = precedingMisc
			End If
			MyBase.AdjustFlagsAndWidth(root)
			Me._root = root
			If (followingMisc IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(followingMisc)
				Me._followingMisc = followingMisc
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax, ByVal precedingMisc As GreenNode, ByVal root As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal followingMisc As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(declaration)
			Me._declaration = declaration
			If (precedingMisc IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(precedingMisc)
				Me._precedingMisc = precedingMisc
			End If
			MyBase.AdjustFlagsAndWidth(root)
			Me._root = root
			If (followingMisc IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(followingMisc)
				Me._followingMisc = followingMisc
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 4
			Dim xmlDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)
			If (xmlDeclarationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlDeclarationSyntax)
				Me._declaration = xmlDeclarationSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._precedingMisc = greenNode
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (xmlNodeSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlNodeSyntax)
				Me._root = xmlNodeSyntax
			End If
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
				Me._followingMisc = greenNode1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlDocument(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._declaration
					Exit Select
				Case 1
					greenNode = Me._precedingMisc
					Exit Select
				Case 2
					greenNode = Me._root
					Exit Select
				Case 3
					greenNode = Me._followingMisc
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._declaration, Me._precedingMisc, Me._root, Me._followingMisc)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._declaration, Me._precedingMisc, Me._root, Me._followingMisc)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._declaration, IObjectWritable))
			writer.WriteValue(DirectCast(Me._precedingMisc, IObjectWritable))
			writer.WriteValue(DirectCast(Me._root, IObjectWritable))
			writer.WriteValue(DirectCast(Me._followingMisc, IObjectWritable))
		End Sub
	End Class
End Namespace
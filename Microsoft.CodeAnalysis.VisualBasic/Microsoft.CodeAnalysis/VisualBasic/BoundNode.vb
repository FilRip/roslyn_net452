Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Operations
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundNode
		Implements IBoundNodeWithIOperationChildren
		Private ReadOnly _kind As BoundKind

		Private _attributes As BoundNode.BoundNodeAttributes

		Private ReadOnly _syntax As SyntaxNode

		Protected Overridable ReadOnly Property Children As ImmutableArray(Of BoundNode) Implements IBoundNodeWithIOperationChildren.Children
			Get
				Return ImmutableArray(Of BoundNode).Empty
			End Get
		End Property

		Public ReadOnly Property HasErrors As Boolean
			Get
				Return CInt((Me._attributes And BoundNode.BoundNodeAttributes.HasErrors)) <> 0
			End Get
		End Property

		Public ReadOnly Property IBoundNodeWithIOperationChildren_Children As ImmutableArray(Of BoundNode) Implements IBoundNodeWithIOperationChildren.Children
			Get
				Return Me.Children
			End Get
		End Property

		Public ReadOnly Property Kind As BoundKind
			Get
				Return Me._kind
			End Get
		End Property

		Public ReadOnly Property Syntax As SyntaxNode
			Get
				Return Me._syntax
			End Get
		End Property

		Public ReadOnly Property SyntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Get
				Return DirectCast(Me._syntax.SyntaxTree, VisualBasicSyntaxTree)
			End Get
		End Property

		Public ReadOnly Property WasCompilerGenerated As Boolean
			Get
				Return CInt((Me._attributes And BoundNode.BoundNodeAttributes.WasCompilerGenerated)) <> 0
			End Get
		End Property

		Public Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode)
			MyBase.New()
			Me._kind = kind
			Me._syntax = syntax
		End Sub

		Public Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal hasErrors As Boolean)
			MyClass.New(kind, syntax)
			If (hasErrors) Then
				Me._attributes = BoundNode.BoundNodeAttributes.HasErrors
			End If
		End Sub

		Public Overridable Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Protected Sub CopyAttributes(ByVal node As BoundNode)
			If (node.WasCompilerGenerated) Then
				Me.SetWasCompilerGenerated()
			End If
		End Sub

		Public Sub SetWasCompilerGenerated()
			Me._attributes = Me._attributes Or BoundNode.BoundNodeAttributes.WasCompilerGenerated
		End Sub

		<Conditional("DEBUG")>
		Private Shared Sub ValidateLocationInformation(ByVal kind As BoundKind, ByVal syntax As SyntaxNode)
		End Sub

		<Flags>
		Private Enum BoundNodeAttributes As Byte
			HasErrors = 1
			WasCompilerGenerated = 2
		End Enum
	End Class
End Namespace
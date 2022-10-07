Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundAddRemoveHandlerStatement
		Inherits BoundStatement
		Private ReadOnly _EventAccess As BoundExpression

		Private ReadOnly _Handler As BoundExpression

		Public ReadOnly Property EventAccess As BoundExpression
			Get
				Return Me._EventAccess
			End Get
		End Property

		Public ReadOnly Property Handler As BoundExpression
			Get
				Return Me._Handler
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal eventAccess As BoundExpression, ByVal handler As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(kind, syntax, hasErrors)
			Me._EventAccess = eventAccess
			Me._Handler = handler
		End Sub
	End Class
End Namespace
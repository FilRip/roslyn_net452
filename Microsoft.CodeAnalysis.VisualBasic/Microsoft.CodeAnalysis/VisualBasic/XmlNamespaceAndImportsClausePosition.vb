Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure XmlNamespaceAndImportsClausePosition
		Public ReadOnly XmlNamespace As String

		Public ReadOnly ImportsClausePosition As Integer

		Public Sub New(ByVal xmlNamespace As String, ByVal importsClausePosition As Integer)
			Me = New XmlNamespaceAndImportsClausePosition() With
			{
				.XmlNamespace = xmlNamespace,
				.ImportsClausePosition = importsClausePosition
			}
		End Sub
	End Structure
End Namespace
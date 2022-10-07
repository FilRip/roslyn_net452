Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class XmlNamespaceImportsBinder
		Inherits Binder
		Private ReadOnly _namespaces As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition)

		Friend Overrides ReadOnly Property HasImportedXmlNamespaces As Boolean
			Get
				Return True
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal namespaces As IReadOnlyDictionary(Of String, XmlNamespaceAndImportsClausePosition))
			MyBase.New(containingBinder)
			Me._namespaces = namespaces
		End Sub

		Friend Overrides Function LookupXmlNamespace(ByVal prefix As String, ByVal ignoreXmlNodes As Boolean, <Out> ByRef [namespace] As String, <Out> ByRef fromImports As Boolean) As Boolean
			Dim flag As Boolean
			Dim xmlNamespaceAndImportsClausePosition As Microsoft.CodeAnalysis.VisualBasic.XmlNamespaceAndImportsClausePosition = New Microsoft.CodeAnalysis.VisualBasic.XmlNamespaceAndImportsClausePosition()
			If (Not Me._namespaces.TryGetValue(prefix, xmlNamespaceAndImportsClausePosition)) Then
				flag = MyBase.LookupXmlNamespace(prefix, ignoreXmlNodes, [namespace], fromImports)
			Else
				[namespace] = xmlNamespaceAndImportsClausePosition.XmlNamespace
				MyBase.Compilation.MarkImportDirectiveAsUsed(MyBase.SyntaxTree, xmlNamespaceAndImportsClausePosition.ImportsClausePosition)
				fromImports = True
				flag = True
			End If
			Return flag
		End Function
	End Class
End Namespace
Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class XmlElementBinder
		Inherits Binder
		Private ReadOnly _namespaces As Dictionary(Of String, String)

		Public Sub New(ByVal containingBinder As Binder, ByVal namespaces As Dictionary(Of String, String))
			MyBase.New(containingBinder)
			Me._namespaces = namespaces
		End Sub

		Friend Overrides Sub GetInScopeXmlNamespaces(ByVal builder As ArrayBuilder(Of KeyValuePair(Of String, String)))
			builder.AddRange(Me._namespaces)
			MyBase.ContainingBinder.GetInScopeXmlNamespaces(builder)
		End Sub

		Friend Overrides Function LookupXmlNamespace(ByVal prefix As String, ByVal ignoreXmlNodes As Boolean, <Out> ByRef [namespace] As String, <Out> ByRef fromImports As Boolean) As Boolean
			Dim flag As Boolean
			If (ignoreXmlNodes OrElse Not Me._namespaces.TryGetValue(prefix, [namespace])) Then
				flag = MyBase.LookupXmlNamespace(prefix, ignoreXmlNodes, [namespace], fromImports)
			Else
				fromImports = False
				flag = True
			End If
			Return flag
		End Function
	End Class
End Namespace
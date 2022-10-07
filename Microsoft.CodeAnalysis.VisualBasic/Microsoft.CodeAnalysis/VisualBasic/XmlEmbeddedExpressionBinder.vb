Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class XmlEmbeddedExpressionBinder
		Inherits Binder
		Public Sub New(ByVal containingBinder As Binder)
			MyBase.New(containingBinder)
		End Sub

		Friend Overrides Sub GetInScopeXmlNamespaces(ByVal builder As ArrayBuilder(Of KeyValuePair(Of String, String)))
		End Sub

		Friend Overrides Function LookupXmlNamespace(ByVal prefix As String, ByVal ignoreXmlNodes As Boolean, <Out> ByRef [namespace] As String, <Out> ByRef fromImports As Boolean) As Boolean
			Return MyBase.LookupXmlNamespace(prefix, True, [namespace], fromImports)
		End Function
	End Class
End Namespace
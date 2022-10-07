Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class XmlRootElementBinder
		Inherits Binder
		Public Sub New(ByVal containingBinder As Binder)
			MyBase.New(containingBinder)
		End Sub

		Friend Overrides Sub GetInScopeXmlNamespaces(ByVal builder As ArrayBuilder(Of KeyValuePair(Of String, String)))
		End Sub
	End Class
End Namespace
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlContainerRewriterInfo
		Public ReadOnly IsRoot As Boolean

		Public ReadOnly Placeholder As BoundRValuePlaceholder

		Public ReadOnly ObjectCreation As BoundExpression

		Public ReadOnly XmlnsAttributesPlaceholder As BoundRValuePlaceholder

		Public ReadOnly XmlnsAttributes As BoundExpression

		Public ReadOnly PrefixesPlaceholder As BoundRValuePlaceholder

		Public ReadOnly NamespacesPlaceholder As BoundRValuePlaceholder

		Public ReadOnly ImportedNamespaces As ImmutableArray(Of KeyValuePair(Of String, String))

		Public ReadOnly InScopeXmlNamespaces As ImmutableArray(Of KeyValuePair(Of String, String))

		Public ReadOnly SideEffects As ImmutableArray(Of BoundExpression)

		Public ReadOnly HasErrors As Boolean

		Public Sub New(ByVal objectCreation As BoundExpression)
			MyBase.New()
			Me.ObjectCreation = objectCreation
			Me.SideEffects = ImmutableArray(Of BoundExpression).Empty
			Me.HasErrors = objectCreation.HasErrors
		End Sub

		Public Sub New(ByVal isRoot As Boolean, ByVal placeholder As BoundRValuePlaceholder, ByVal objectCreation As BoundExpression, ByVal xmlnsAttributesPlaceholder As BoundRValuePlaceholder, ByVal xmlnsAttributes As BoundExpression, ByVal prefixesPlaceholder As BoundRValuePlaceholder, ByVal namespacesPlaceholder As BoundRValuePlaceholder, ByVal importedNamespaces As ImmutableArray(Of KeyValuePair(Of String, String)), ByVal inScopeXmlNamespaces As ImmutableArray(Of KeyValuePair(Of String, String)), ByVal sideEffects As ImmutableArray(Of BoundExpression))
			MyBase.New()
			Dim flag As Boolean
			Dim hasErrors As Func(Of BoundExpression, Boolean)
			Me.IsRoot = isRoot
			Me.Placeholder = placeholder
			Me.ObjectCreation = objectCreation
			Me.XmlnsAttributesPlaceholder = xmlnsAttributesPlaceholder
			Me.XmlnsAttributes = xmlnsAttributes
			Me.PrefixesPlaceholder = prefixesPlaceholder
			Me.NamespacesPlaceholder = namespacesPlaceholder
			Me.ImportedNamespaces = importedNamespaces
			Me.InScopeXmlNamespaces = inScopeXmlNamespaces
			Me.SideEffects = sideEffects
			If (objectCreation.HasErrors) Then
				flag = True
			Else
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = sideEffects
				If (BoundXmlContainerRewriterInfo._Closure$__.$I1-0 Is Nothing) Then
					hasErrors = Function(s As BoundExpression) s.HasErrors
					BoundXmlContainerRewriterInfo._Closure$__.$I1-0 = hasErrors
				Else
					hasErrors = BoundXmlContainerRewriterInfo._Closure$__.$I1-0
				End If
				flag = boundExpressions.Any(hasErrors)
			End If
			Me.HasErrors = flag
		End Sub
	End Class
End Namespace
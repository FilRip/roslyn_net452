Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Module XmlContextExtensions
		<Extension>
		Friend Function MatchEndElement(ByVal this As List(Of XmlContext), ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax) As Integer
			Dim num As Integer
			Dim i As Integer
			Dim count As Integer = this.Count - 1
			If (name IsNot Nothing) Then
				For i = count To 0 Step -1
					Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = this(i).StartElement.Name
					If (xmlNodeSyntax.Kind = SyntaxKind.XmlName) Then
						Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(xmlNodeSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
						If (EmbeddedOperators.CompareString(xmlNameSyntax.LocalName.Text, name.LocalName.Text, False) = 0) Then
							Dim prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax = xmlNameSyntax.Prefix
							Dim xmlPrefixSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax = name.Prefix
							If (prefix = xmlPrefixSyntax OrElse prefix IsNot Nothing AndAlso xmlPrefixSyntax IsNot Nothing AndAlso EmbeddedOperators.CompareString(prefix.Name.Text, xmlPrefixSyntax.Name.Text, False) = 0) Then
								Exit For
							End If
						End If
					End If
				Next

				num = i
			Else
				num = count
			End If
			Return num
		End Function

		<Extension>
		Friend Function Peek(ByVal this As List(Of XmlContext), Optional ByVal i As Integer = 0) As XmlContext
			Return this(this.Count - 1 - i)
		End Function

		<Extension>
		Friend Function Pop(ByVal this As List(Of XmlContext)) As XmlContext
			Dim count As Integer = this.Count - 1
			Dim item As XmlContext = this(count)
			this.RemoveAt(count)
			Return item
		End Function

		<Extension>
		Friend Sub Push(ByVal this As List(Of XmlContext), ByVal context As XmlContext)
			this.Add(context)
		End Sub
	End Module
End Namespace
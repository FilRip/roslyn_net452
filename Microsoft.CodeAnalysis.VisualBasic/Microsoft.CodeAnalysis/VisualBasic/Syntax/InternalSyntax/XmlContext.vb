Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Structure XmlContext
		Private ReadOnly _start As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax

		Private ReadOnly _content As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)

		Private ReadOnly _pool As SyntaxListPool

		Public ReadOnly Property StartElement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax
			Get
				Return Me._start
			End Get
		End Property

		Public Sub New(ByVal pool As SyntaxListPool, ByVal start As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax)
			Me = New XmlContext() With
			{
				._pool = pool,
				._start = start,
				._content = Me._pool.Allocate(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)()
			}
		End Sub

		Public Sub Add(ByVal xml As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			Me._content.Add(xml)
		End Sub

		Public Function CreateElement(ByVal endElement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me._content.ToList()
			Me._pool.Free(Me._content)
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlElement(Me._start, list, endElement)
		End Function

		Public Function CreateElement(ByVal endElement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax, ByVal diagnostic As DiagnosticInfo) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Dim list As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) = Me._content.ToList()
			Me._pool.Free(Me._content)
			Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.XmlElement(DirectCast(Me._start.AddError(diagnostic), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax), list, endElement)
		End Function
	End Structure
End Namespace
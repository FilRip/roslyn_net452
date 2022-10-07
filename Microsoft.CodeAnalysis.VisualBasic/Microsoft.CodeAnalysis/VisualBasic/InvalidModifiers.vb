Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Reflection

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module InvalidModifiers
		Public InvalidModifiersInNotInheritableClass As SyntaxKind()

		Public InvalidModifiersInNotInheritableOtherPartialClass As SyntaxKind()

		Public InvalidModifiersInModule As SyntaxKind()

		Public InvalidModifiersInInterface As SyntaxKind()

		Public InvalidModifiersIfShared As SyntaxKind()

		Public InvalidModifiersIfDefault As SyntaxKind()

		Public InvalidAsyncIterator As SyntaxKind()

		Sub New()
			InvalidModifiers.InvalidModifiersInNotInheritableClass = New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("2040620F9B6CF72996AAA7814F9B1F83D28A1A3E1E71BB6234C2D6012BDA5197").FieldHandle }
			InvalidModifiers.InvalidModifiersInNotInheritableOtherPartialClass = New SyntaxKind() { SyntaxKind.MustOverrideKeyword }
			InvalidModifiers.InvalidModifiersInModule = New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("5EC683CAD27E4B7797B6666AAA0E902CB195956B394AA63C954965FED9A88A47").FieldHandle }
			InvalidModifiers.InvalidModifiersInInterface = New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("0837912FE8CE66D4FACB7BEBF39DE592659235BE0E02C784106FF5568F74E431").FieldHandle }
			InvalidModifiers.InvalidModifiersIfShared = New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("BA70097A1E850DDC9EBA7182BAE25DE4856BADC0505E149B5506D080402B2628").FieldHandle }
			InvalidModifiers.InvalidModifiersIfDefault = New SyntaxKind() { SyntaxKind.PrivateKeyword }
			InvalidModifiers.InvalidAsyncIterator = New SyntaxKind() { SyntaxKind.AsyncKeyword, SyntaxKind.IteratorKeyword }
		End Sub
	End Module
End Namespace
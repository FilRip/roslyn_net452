Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module MethodKindExtensions
		<Extension>
		Friend Function TryGetAccessorDisplayName(ByVal kind As MethodKind) As String
			Dim text As String
			Select Case kind
				Case MethodKind.EventAdd
					text = SyntaxFacts.GetText(SyntaxKind.AddHandlerKeyword)
					Exit Select
				Case MethodKind.EventRaise
					text = SyntaxFacts.GetText(SyntaxKind.RaiseEventKeyword)
					Exit Select
				Case MethodKind.EventRemove
					text = SyntaxFacts.GetText(SyntaxKind.RemoveHandlerKeyword)
					Exit Select
				Case MethodKind.ExplicitInterfaceImplementation
				Case MethodKind.UserDefinedOperator
				Case MethodKind.Ordinary
				Label0:
					text = Nothing
					Exit Select
				Case MethodKind.PropertyGet
					text = SyntaxFacts.GetText(SyntaxKind.GetKeyword)
					Exit Select
				Case MethodKind.PropertySet
					text = SyntaxFacts.GetText(SyntaxKind.SetKeyword)
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return text
		End Function
	End Module
End Namespace
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure FlowAnalysisInfo
		Public ReadOnly Compilation As VisualBasicCompilation

		Public ReadOnly Symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol

		Public ReadOnly Node As BoundNode

		Public Sub New(ByVal _compilation As VisualBasicCompilation, ByVal _symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal _node As BoundNode)
			Me = New FlowAnalysisInfo() With
			{
				.Compilation = _compilation,
				.Symbol = _symbol,
				.Node = _node
			}
		End Sub
	End Structure
End Namespace
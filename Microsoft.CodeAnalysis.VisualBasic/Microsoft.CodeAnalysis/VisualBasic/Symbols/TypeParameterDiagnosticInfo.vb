Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Structure TypeParameterDiagnosticInfo
		Public ReadOnly TypeParameter As TypeParameterSymbol

		Public ReadOnly Constraint As TypeParameterConstraint

		Public ReadOnly UseSiteInfo As UseSiteInfo(Of AssemblySymbol)

		Public Sub New(ByVal typeParameter As TypeParameterSymbol, ByVal useSiteInfo As UseSiteInfo(Of AssemblySymbol))
			Me = New TypeParameterDiagnosticInfo() With
			{
				.TypeParameter = typeParameter,
				.UseSiteInfo = useSiteInfo
			}
		End Sub

		Public Sub New(ByVal typeParameter As TypeParameterSymbol, ByVal diagnostic As DiagnosticInfo)
			MyClass.New(typeParameter, New UseSiteInfo(Of AssemblySymbol)(diagnostic))
		End Sub

		Public Sub New(ByVal typeParameter As TypeParameterSymbol, ByVal constraint As TypeParameterConstraint, ByVal diagnostic As DiagnosticInfo)
			MyClass.New(typeParameter, diagnostic)
			Me.Constraint = constraint
		End Sub
	End Structure
End Namespace
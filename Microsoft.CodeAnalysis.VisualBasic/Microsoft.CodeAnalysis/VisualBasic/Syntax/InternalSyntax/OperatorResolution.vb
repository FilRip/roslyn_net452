Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Module OperatorResolution
		Private ReadOnly s_table As Byte(,,)

		Sub New()
			OperatorResolution.s_table = New Byte(,,) { { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 11, 9, 11, 11, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 9, 9, 11, 11, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 11, 11, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 11, 11, 11, 11, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 13, 13, 12, 13, 12, 13, 14, 15, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 13, 13, 13, 13, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 15, 15, 14, 15, 14, 15, 14, 15, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 17, 17, 16, 17, 16, 17, 16, 17, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 20, 0, 20, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 20, 20, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 20, 20, 20, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } }, { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 11, 9, 11, 11, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 9, 9, 11, 11, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 11, 11, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 11, 11, 11, 11, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 13, 13, 12, 13, 12, 13, 14, 15, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 13, 13, 13, 13, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 15, 15, 14, 15, 14, 15, 14, 15, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 17, 17, 16, 17, 16, 17, 16, 17, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1 } }, { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 17, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 17, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 17, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 17, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 17, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 17, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 17, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 17, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 17, 18, 19, 0, 0, 19, 1 }, { 0, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1 } }, { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1 } }, { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 11, 9, 11, 11, 13, 13, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 9, 9, 11, 11, 13, 13, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 11, 11, 10, 11, 12, 13, 14, 15, 16, 15, 15, 15, 0, 0, 15, 1 }, { 0, 11, 11, 11, 11, 13, 13, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 13, 13, 12, 13, 12, 13, 14, 15, 16, 15, 15, 15, 0, 0, 15, 1 }, { 0, 13, 13, 13, 13, 13, 13, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 14, 15, 14, 15, 14, 15, 16, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 16, 15, 16, 15, 16, 15, 16, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1 } }, { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 1 }, { 0, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 1 }, { 0, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 1 }, { 0, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 1 }, { 0, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 1 }, { 0, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 1 }, { 0, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 1 }, { 0, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } }, { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 7, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1 } }, { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 7, 9, 11, 11, 13, 13, 15, 15, 15, 15, 15, 15, 0, 0, 7, 1 }, { 0, 9, 9, 11, 11, 13, 13, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 11, 11, 10, 11, 12, 13, 14, 15, 16, 15, 15, 15, 0, 0, 15, 1 }, { 0, 11, 11, 11, 11, 13, 13, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 13, 13, 12, 13, 12, 13, 14, 15, 16, 15, 15, 15, 0, 0, 15, 1 }, { 0, 13, 13, 13, 13, 13, 13, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 14, 15, 14, 15, 14, 15, 16, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 16, 15, 16, 15, 16, 15, 16, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 7, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 15, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1 } }, { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 7, 9, 11, 11, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 7, 1 }, { 0, 9, 9, 11, 11, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 11, 11, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 11, 11, 11, 11, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 13, 13, 12, 13, 12, 13, 14, 15, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 13, 13, 13, 13, 13, 13, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 15, 15, 14, 15, 14, 15, 14, 15, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 15, 15, 15, 15, 15, 15, 15, 15, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 17, 17, 16, 17, 16, 17, 16, 17, 16, 17, 18, 19, 0, 0, 19, 1 }, { 0, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 18, 19, 0, 0, 19, 1 }, { 0, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 19, 0, 0, 19, 1 }, { 0, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 0, 0, 19, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 33, 0, 33, 1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 20, 1 }, { 0, 7, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 19, 33, 20, 20, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } }, { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 1 }, { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } } }
		End Sub

		Friend Function LookupInOperatorTables(ByVal opcode As SyntaxKind, ByVal left As SpecialType, ByVal right As SpecialType) As SpecialType
			Dim tableKind As OperatorResolution.TableKind
			Select Case opcode
				Case SyntaxKind.AddExpression
					tableKind = OperatorResolution.TableKind.Addition
					Exit Select
				Case SyntaxKind.SubtractExpression
				Case SyntaxKind.MultiplyExpression
				Case SyntaxKind.ModuloExpression
					tableKind = OperatorResolution.TableKind.SubtractionMultiplicationModulo
					Exit Select
				Case SyntaxKind.DivideExpression
					tableKind = OperatorResolution.TableKind.Division
					Exit Select
				Case SyntaxKind.IntegerDivideExpression
					tableKind = OperatorResolution.TableKind.IntegerDivision
					Exit Select
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.RaiseEventStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.DirectCastExpression
				Case SyntaxKind.List Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.OptionStatement Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.InheritsStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.CharacterLiteralExpression Or SyntaxKind.TrueLiteralExpression Or SyntaxKind.NothingLiteralExpression Or SyntaxKind.ParenthesizedExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.DirectCastExpression Or SyntaxKind.TryCastExpression
				Case SyntaxKind.IsExpression
				Case SyntaxKind.IsNotExpression
					Throw ExceptionUtilities.UnexpectedValue(opcode)
				Case SyntaxKind.ExponentiateExpression
					tableKind = OperatorResolution.TableKind.Power
					Exit Select
				Case SyntaxKind.LeftShiftExpression
				Case SyntaxKind.RightShiftExpression
					tableKind = OperatorResolution.TableKind.Shift
					Exit Select
				Case SyntaxKind.ConcatenateExpression
				Case SyntaxKind.LikeExpression
					tableKind = OperatorResolution.TableKind.ConcatenationLike
					Exit Select
				Case SyntaxKind.EqualsExpression
				Case SyntaxKind.NotEqualsExpression
				Case SyntaxKind.LessThanExpression
				Case SyntaxKind.LessThanOrEqualExpression
				Case SyntaxKind.GreaterThanOrEqualExpression
				Case SyntaxKind.GreaterThanExpression
					tableKind = OperatorResolution.TableKind.Relational
					Exit Select
				Case SyntaxKind.OrExpression
				Case SyntaxKind.ExclusiveOrExpression
				Case SyntaxKind.AndExpression
					tableKind = OperatorResolution.TableKind.Bitwise
					Exit Select
				Case SyntaxKind.OrElseExpression
				Case SyntaxKind.AndAlsoExpression
					tableKind = OperatorResolution.TableKind.Logical
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(opcode)
			End Select
			Return DirectCast(CSByte(OperatorResolution.s_table(CInt(tableKind), OperatorResolution.TypeCodeToIndex(left), OperatorResolution.TypeCodeToIndex(right))), SpecialType)
		End Function

		Private Function TypeCodeToIndex(ByVal specialType As Microsoft.CodeAnalysis.SpecialType) As Integer
			Dim num As Integer
			Dim specialType1 As Microsoft.CodeAnalysis.SpecialType = specialType
			Select Case specialType1
				Case Microsoft.CodeAnalysis.SpecialType.None
					num = 0
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Object
					num = 16
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Enum
				Case Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate
				Case Microsoft.CodeAnalysis.SpecialType.System_Delegate
				Case Microsoft.CodeAnalysis.SpecialType.System_ValueType
				Case Microsoft.CodeAnalysis.SpecialType.System_Void
					Throw ExceptionUtilities.UnexpectedValue(specialType)
				Case Microsoft.CodeAnalysis.SpecialType.System_Boolean
					num = 1
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Char
					num = 14
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_SByte
					num = 2
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Byte
					num = 3
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Int16
					num = 4
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt16
					num = 5
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Int32
					num = 6
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt32
					num = 7
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Int64
					num = 8
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_UInt64
					num = 9
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Decimal
					num = 10
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Single
					num = 11
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_Double
					num = 12
					Exit Select
				Case Microsoft.CodeAnalysis.SpecialType.System_String
					num = 15
					Exit Select
				Case Else
					If (specialType1 = Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
						num = 13
						Exit Select
					Else
						Throw ExceptionUtilities.UnexpectedValue(specialType)
					End If
			End Select
			Return num
		End Function

		Private Enum TableKind
			Addition
			SubtractionMultiplicationModulo
			Division
			Power
			IntegerDivision
			Shift
			Logical
			Bitwise
			Relational
			ConcatenationLike
		End Enum
	End Module
End Namespace
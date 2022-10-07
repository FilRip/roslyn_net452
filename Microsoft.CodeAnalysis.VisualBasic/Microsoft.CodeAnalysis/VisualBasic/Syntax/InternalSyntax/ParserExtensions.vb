Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Module ParserExtensions
		<Extension>
		Friend Function Any(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal this As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of T), ByVal ParamArray kinds As SyntaxKind()) As Boolean
			Dim flag As Boolean
			Dim length As Integer = CInt(kinds.Length) - 1
			Dim num As Integer = 0
			While True
				If (num > length) Then
					flag = False
					Exit While
				ElseIf (Not this.Any(CInt(kinds(num)))) Then
					num = num + 1
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Friend Function AnyAndOnly(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal this As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of T), ByVal ParamArray kinds As SyntaxKind()) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean = False
			Dim count As Integer = this.Count - 1
			Dim num As Integer = 0
			While True
				If (num <= count) Then
					flag1 = kinds.Contains(this(num).Kind)
					If (flag1) Then
						num = num + 1
					Else
						flag = False
						Exit While
					End If
				Else
					flag = flag1
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Friend Function ContainsDiagnostics(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal this As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of T)) As Boolean
			Dim flag As Boolean
			Dim count As Integer = this.Count - 1
			Dim num As Integer = 0
			While True
				If (num > count) Then
					flag = False
					Exit While
				ElseIf (Not this(num).ContainsDiagnostics) Then
					num = num + 1
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Friend Function ContainsDiagnostics(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(ByVal this As SyntaxListBuilder(Of T)) As Boolean
			Dim flag As Boolean
			Dim count As Integer = this.Count - 1
			Dim num As Integer = 0
			While True
				If (num > count) Then
					flag = False
					Exit While
				ElseIf (Not this(num).ContainsDiagnostics) Then
					num = num + 1
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function
	End Module
End Namespace
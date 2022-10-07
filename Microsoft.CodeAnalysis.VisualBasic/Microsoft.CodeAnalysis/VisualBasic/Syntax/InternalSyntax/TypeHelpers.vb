Imports Microsoft.CodeAnalysis
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class TypeHelpers
		Private Sub New()
			MyBase.New()
		End Sub

		Friend Shared Function UncheckedCLng(ByVal v As CConst) As Long
			Dim num As Long
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = v.SpecialType
			If (specialType.IsIntegralType()) Then
				num = Microsoft.VisualBasic.CompilerServices.Conversions.ToLong(v.ValueAsObject)
			ElseIf (specialType <> Microsoft.CodeAnalysis.SpecialType.System_Char) Then
				If (specialType <> Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
					Throw ExceptionUtilities.UnexpectedValue(specialType)
				End If
				num = Microsoft.VisualBasic.CompilerServices.Conversions.ToDate(v.ValueAsObject).ToBinary()
			Else
				num = CLng(Microsoft.VisualBasic.CompilerServices.Conversions.ToChar(v.ValueAsObject))
			End If
			Return num
		End Function

		Friend Shared Function VarDecAdd(ByVal pdecLeft As [Decimal], ByVal pdecRight As [Decimal], ByRef pdecResult As [Decimal]) As Boolean
			Dim flag As Boolean
			Try
				pdecResult = [Decimal].Add(pdecLeft, pdecRight)
			Catch overflowException As System.OverflowException
				ProjectData.SetProjectError(overflowException)
				flag = True
				ProjectData.ClearProjectError()
				Return flag
			End Try
			flag = False
			Return flag
		End Function

		Friend Shared Function VarDecDiv(ByVal pdecLeft As [Decimal], ByVal pdecRight As [Decimal], ByRef pdecResult As [Decimal]) As Boolean
			Dim flag As Boolean
			Try
				pdecResult = [Decimal].Divide(pdecLeft, pdecRight)
			Catch overflowException As System.OverflowException
				ProjectData.SetProjectError(overflowException)
				flag = True
				ProjectData.ClearProjectError()
				Return flag
			End Try
			flag = False
			Return flag
		End Function

		Friend Shared Function VarDecMul(ByVal pdecLeft As [Decimal], ByVal pdecRight As [Decimal], ByRef pdecResult As [Decimal]) As Boolean
			Dim flag As Boolean
			Try
				pdecResult = [Decimal].Multiply(pdecLeft, pdecRight)
			Catch overflowException As System.OverflowException
				ProjectData.SetProjectError(overflowException)
				flag = True
				ProjectData.ClearProjectError()
				Return flag
			End Try
			flag = False
			Return flag
		End Function

		Friend Shared Function VarDecSub(ByVal pdecLeft As [Decimal], ByVal pdecRight As [Decimal], ByRef pdecResult As [Decimal]) As Boolean
			Dim flag As Boolean
			Try
				pdecResult = [Decimal].Subtract(pdecLeft, pdecRight)
			Catch overflowException As System.OverflowException
				ProjectData.SetProjectError(overflowException)
				flag = True
				ProjectData.ClearProjectError()
				Return flag
			End Try
			flag = False
			Return flag
		End Function
	End Class
End Namespace
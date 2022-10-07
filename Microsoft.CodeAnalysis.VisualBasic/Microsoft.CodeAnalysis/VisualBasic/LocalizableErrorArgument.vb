Imports System
Imports System.Globalization

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Structure LocalizableErrorArgument
		Implements IFormattable
		Private ReadOnly _id As ERRID

		Friend Sub New(ByVal id As ERRID)
			Me = New LocalizableErrorArgument() With
			{
				._id = id
			}
		End Sub

		Public Overrides Function ToString() As String
			Return Me.ToString_IFormattable(Nothing, Nothing)
		End Function

		Public Function ToString_IFormattable(ByVal format As String, ByVal formatProvider As IFormatProvider) As String Implements IFormattable.ToString
			Return ErrorFactory.IdToString(Me._id, DirectCast(formatProvider, CultureInfo))
		End Function
	End Structure
End Namespace
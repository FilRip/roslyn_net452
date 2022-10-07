Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.Diagnostics.VisualBasic
	<DiagnosticAnalyzer("Visual Basic", New String() {  })>
	Friend Class VisualBasicCompilerDiagnosticAnalyzer
		Inherits CompilerDiagnosticAnalyzer
		Friend Overrides ReadOnly Property MessageProvider As CommonMessageProvider
			Get
				Return Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
			End Get
		End Property

		Public Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function GetSupportedErrorCodes() As ImmutableArray(Of Integer)
			Dim enumerator As IEnumerator = Nothing
			Dim values As Array = [Enum].GetValues(GetType(ERRID))
			Dim nums As ImmutableArray(Of Integer).Builder = ImmutableArray.CreateBuilder(Of Integer)()
			Try
				enumerator = values.GetEnumerator()
				While enumerator.MoveNext()
					Dim [integer] As Integer = Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(enumerator.Current)
					If ([integer] = 31091 OrElse [integer] = 35000 OrElse [integer] = 36597 OrElse [integer] <= 0 OrElse [integer] >= 42600) Then
						Continue While
					End If
					nums.Add([integer])
				End While
			Finally
				If (TypeOf enumerator Is IDisposable) Then
					TryCast(enumerator, IDisposable).Dispose()
				End If
			End Try
			Return nums.ToImmutable()
		End Function
	End Class
End Namespace
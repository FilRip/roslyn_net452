Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class CommandLineDiagnosticFormatter
		Inherits VisualBasicDiagnosticFormatter
		Private ReadOnly _baseDirectory As String

		Private ReadOnly _getAdditionalTextFiles As Func(Of ImmutableArray(Of AdditionalTextFile))

		Friend Sub New(ByVal baseDirectory As String, ByVal getAdditionalTextFiles As Func(Of ImmutableArray(Of AdditionalTextFile)))
			MyBase.New()
			Me._baseDirectory = baseDirectory
			Me._getAdditionalTextFiles = getAdditionalTextFiles
		End Sub

		Public Overrides Function Format(ByVal diagnostic As Microsoft.CodeAnalysis.Diagnostic, Optional ByVal formatter As IFormatProvider = Nothing) As String
			Dim str As String
			Dim sourceText As Microsoft.CodeAnalysis.Text.SourceText = Nothing
			Dim diagnosticSpanAndFileText As Nullable(Of TextSpan) = Me.GetDiagnosticSpanAndFileText(diagnostic, sourceText)
			If (Not diagnosticSpanAndFileText.HasValue OrElse sourceText Is Nothing OrElse sourceText.Length < diagnosticSpanAndFileText.Value.[End]) Then
				If (diagnostic.Location <> Location.None) Then
					diagnostic = diagnostic.WithLocation(Location.None)
				End If
				str = [String].Concat("vbc : ", MyBase.Format(diagnostic, formatter))
			Else
				Dim str1 As String = MyBase.Format(diagnostic, formatter)
				Dim stringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder()
				stringBuilder.AppendLine(str1)
				Dim value As TextSpan = diagnosticSpanAndFileText.Value
				Dim start As Integer = value.Start
				Dim [end] As Integer = value.[End]
				Dim num As Integer = sourceText.Lines.IndexOf(start)
				Dim item As TextLine = sourceText.Lines(num)
				If (value.IsEmpty AndAlso item.Start = [end] AndAlso num > 0) Then
					num = num - 1
					item = sourceText.Lines(num)
				End If
				While item.Start < [end]
					stringBuilder.AppendLine()
					stringBuilder.AppendLine(item.ToString().Replace("	", "    "))
					Dim num1 As Integer = Math.Min(start, item.Start)
					Dim num2 As Integer = Math.Min(item.[End], start) - 1
					Dim num3 As Integer = num1
					Do
						If (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(sourceText(num3)), "	", False) <> 0) Then
							stringBuilder.Append(" ")
						Else
							stringBuilder.Append(Strings.ChrW(32), 4)
						End If
						num3 = num3 + 1
					Loop While num3 <= num2
					If (Not value.IsEmpty) Then
						Dim num4 As Integer = Math.Max(start, item.Start)
						Dim num5 As Integer = Math.Min(If([end] = start, [end], [end] - 1), item.[End] - 1)
						For i As Integer = num4 To num5
							If (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(sourceText(i)), "	", False) <> 0) Then
								stringBuilder.Append("~")
							Else
								stringBuilder.Append("~"C, 4)
							End If
						Next

					Else
						stringBuilder.Append("~")
					End If
					Dim num6 As Integer = Math.Min([end], item.[End])
					Dim end1 As Integer = item.[End] - 1
					Dim num7 As Integer = num6
					Do
						If (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(sourceText(num7)), "	", False) <> 0) Then
							stringBuilder.Append(" ")
						Else
							stringBuilder.Append(Strings.ChrW(32), 4)
						End If
						num7 = num7 + 1
					Loop While num7 <= end1
					num = num + 1
					If (num >= sourceText.Lines.Count) Then
						Exit While
					End If
					item = sourceText.Lines(num)
				End While
				str = stringBuilder.ToString()
			End If
			Return str
		End Function

		Friend Overrides Function FormatSourcePath(ByVal path As String, ByVal basePath As String, ByVal formatter As IFormatProvider) As String
			Return If(FileUtilities.NormalizeRelativePath(path, basePath, Me._baseDirectory), path)
		End Function

		Private Function GetDiagnosticSpanAndFileText(ByVal diagnostic As Microsoft.CodeAnalysis.Diagnostic, <Out> ByRef text As SourceText) As Nullable(Of TextSpan)
			Dim nullable As Nullable(Of TextSpan)
			Dim cancellationToken As System.Threading.CancellationToken
			If (Not diagnostic.Location.IsInSource) Then
				If (diagnostic.Location.Kind = LocationKind.ExternalFile) Then
					Dim path As String = diagnostic.Location.GetLineSpan().Path
					If (path IsNot Nothing) Then
						Dim enumerator As ImmutableArray(Of AdditionalTextFile).Enumerator = Me._getAdditionalTextFiles().GetEnumerator()
						While enumerator.MoveNext()
							Dim current As AdditionalTextFile = enumerator.Current
							If (Not path.Equals(current.Path)) Then
								Continue While
							End If
							Try
								cancellationToken = New System.Threading.CancellationToken()
								text = current.GetText(cancellationToken)
							Catch exception As System.Exception
								ProjectData.SetProjectError(exception)
								text = Nothing
								ProjectData.ClearProjectError()
							End Try
							nullable = New Nullable(Of TextSpan)(diagnostic.Location.SourceSpan)
							Return nullable
						End While
					End If
				End If
				text = Nothing
				nullable = Nothing
			Else
				Dim sourceTree As SyntaxTree = diagnostic.Location.SourceTree
				cancellationToken = New System.Threading.CancellationToken()
				text = sourceTree.GetText(cancellationToken)
				nullable = New Nullable(Of TextSpan)(diagnostic.Location.SourceSpan)
			End If
			Return nullable
		End Function
	End Class
End Namespace
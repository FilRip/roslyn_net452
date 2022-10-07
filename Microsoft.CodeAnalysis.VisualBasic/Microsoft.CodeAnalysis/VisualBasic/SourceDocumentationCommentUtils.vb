Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module SourceDocumentationCommentUtils
		Friend Function GetAndCacheDocumentationComment(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal preferredCulture As CultureInfo, ByVal expandIncludes As Boolean, ByRef lazyXmlText As String, ByVal cancellationToken As System.Threading.CancellationToken) As String
			If (lazyXmlText Is Nothing) Then
				Dim documentationCommentForSymbol As String = SourceDocumentationCommentUtils.GetDocumentationCommentForSymbol(symbol, preferredCulture, expandIncludes, cancellationToken)
				Interlocked.CompareExchange(Of String)(lazyXmlText, documentationCommentForSymbol, Nothing)
			End If
			Return lazyXmlText
		End Function

		Friend Function GetDocumentationCommentForSymbol(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal preferredCulture As CultureInfo, ByVal expandIncludes As Boolean, ByVal cancellationToken As System.Threading.CancellationToken) As String
			Return VisualBasicCompilation.DocumentationCommentCompiler.GetDocumentationCommentXml(symbol, expandIncludes, preferredCulture, cancellationToken)
		End Function
	End Module
End Namespace
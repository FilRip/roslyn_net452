Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module PEDocumentationCommentUtils
		Friend Function GetDocumentationComment(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal containingPEModule As PEModuleSymbol, ByVal preferredCulture As CultureInfo, ByVal cancellationToken As System.Threading.CancellationToken, ByRef lazyDocComment As Tuple(Of CultureInfo, String)) As String
			Dim str As String
			If (lazyDocComment Is Nothing) Then
				Interlocked.CompareExchange(Of Tuple(Of CultureInfo, String))(lazyDocComment, Tuple.Create(Of CultureInfo, String)(preferredCulture, containingPEModule.DocumentationProvider.GetDocumentationForSymbol(symbol.GetDocumentationCommentId(), preferredCulture, cancellationToken)), Nothing)
			End If
			str = If(Not [Object].Equals(lazyDocComment.Item1, preferredCulture), containingPEModule.DocumentationProvider.GetDocumentationForSymbol(symbol.GetDocumentationCommentId(), preferredCulture, cancellationToken), lazyDocComment.Item2)
			Return str
		End Function
	End Module
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module VisualBasicFileSystemExtensions
		<Extension>
		Public Function Emit(ByVal compilation As VisualBasicCompilation, ByVal outputPath As String, Optional ByVal pdbPath As String = Nothing, Optional ByVal xmlDocPath As String = Nothing, Optional ByVal win32ResourcesPath As String = Nothing, Optional ByVal manifestResources As IEnumerable(Of ResourceDescription) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As EmitResult
			Return FileSystemExtensions.Emit(compilation, outputPath, pdbPath, xmlDocPath, win32ResourcesPath, manifestResources, cancellationToken)
		End Function
	End Module
End Namespace
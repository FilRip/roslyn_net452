Imports Microsoft.CodeAnalysis
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module CRC32
		Private ReadOnly s_CRC32_LOOKUP_TABLE As UInteger()

		Private Const s_CRC32_poly As UInteger = 3988292384

		Private ReadOnly s_encoding As UnicodeEncoding

		Sub New()
			CRC32.s_CRC32_LOOKUP_TABLE = CRC32.InitCrc32Table()
			CRC32.s_encoding = New UnicodeEncoding(False, False)
		End Sub

		Private Function CalcEntry(ByVal crc As UInteger) As UInteger
			' 
			' Current member / type: System.UInt32 Microsoft.CodeAnalysis.VisualBasic.Symbols.CRC32::CalcEntry(System.UInt32)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.UInt32 CalcEntry(System.UInt32)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Function ComputeCRC32(ByVal names As String()) As UInteger
			Dim num As UInteger = -1
			Dim strArray As String() = names
			Dim num1 As Integer = 0
			Do
				Dim str As String = strArray(num1)
				num = CRC32.Crc32Update(num, CRC32.s_encoding.GetBytes(CaseInsensitiveComparison.ToLower(str)))
				num1 = num1 + 1
			Loop While num1 < CInt(strArray.Length)
			Return num
		End Function

		Private Function Crc32Update(ByVal crc32 As UInteger, ByVal bytes As Byte()) As UInteger
			Dim numArray As Byte() = bytes
			Dim num As Integer = 0
			Do
				Dim num1 As Byte = numArray(num)
				crc32 = Microsoft.CodeAnalysis.VisualBasic.Symbols.CRC32.s_CRC32_LOOKUP_TABLE(CByte(crc32) Xor num1) Xor crc32 >> 8
				num = num + 1
			Loop While num < CInt(numArray.Length)
			Return crc32
		End Function

		Private Function InitCrc32Table() As UInteger()
			Dim numArray(255) As UInt32
			Dim num As UInteger = 0
			Do
				numArray(num) = CRC32.CalcEntry(num)
				num = num + 1
			Loop While num <= 255
			Return numArray
		End Function
	End Module
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class SyntaxTrivia
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Private ReadOnly _text As String

		Public Overrides ReadOnly Property IsTrivia As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property ShouldReuseInSerialization As Boolean
			Get
				Dim flag As Boolean
				Dim kind As SyntaxKind = MyBase.Kind
				flag = If(CUShort(kind) - CUShort(SyntaxKind.WhitespaceTrivia) <= CUShort(SyntaxKind.EmptyStatement) OrElse CUShort(kind) - CUShort(SyntaxKind.LineContinuationTrivia) <= CUShort(SyntaxKind.List), True, False)
				Return flag
			End Get
		End Property

		Friend ReadOnly Property Text As String
			Get
				Return Me._text
			End Get
		End Property

		Shared Sub New()
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String)
			MyBase.New(kind, errors, annotations, text.Length)
			Me._text = text
			If (text.Length > 0) Then
				MyBase.SetFlags(GreenNode.NodeFlags.IsNotMissing)
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal text As String, ByVal context As ISyntaxFactoryContext)
			MyClass.New(kind, text)
			MyBase.SetFactoryContext(context)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal text As String)
			MyBase.New(kind, text.Length)
			Me._text = text
			If (text.Length > 0) Then
				MyBase.SetFlags(GreenNode.NodeFlags.IsNotMissing)
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Me._text = reader.ReadString()
			MyBase.FullWidth = Me._text.Length
			If (Me.Text.Length > 0) Then
				MyBase.SetFlags(GreenNode.NodeFlags.IsNotMissing)
			End If
		End Sub

		Public NotOverridable Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitSyntaxTrivia(Me)
		End Function

		Friend NotOverridable Overrides Sub AddSyntaxErrors(ByVal accumulatedErrors As List(Of DiagnosticInfo))
			If (MyBase.GetDiagnostics() IsNot Nothing) Then
				accumulatedErrors.AddRange(MyBase.GetDiagnostics())
			End If
		End Sub

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal position As Integer) As SyntaxNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend NotOverridable Overrides Function GetLeadingTrivia() As GreenNode
			Return Nothing
		End Function

		Public NotOverridable Overrides Function GetLeadingTriviaWidth() As Integer
			Return 0
		End Function

		Friend NotOverridable Overrides Function GetSlot(ByVal index As Integer) As GreenNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend NotOverridable Overrides Function GetTrailingTrivia() As GreenNode
			Return Nothing
		End Function

		Public NotOverridable Overrides Function GetTrailingTriviaWidth() As Integer
			Return 0
		End Function

		Public Overrides Function IsEquivalentTo(ByVal other As GreenNode) As Boolean
			Dim flag As Boolean
			If (MyBase.IsEquivalentTo(other)) Then
				Dim syntaxTrivium As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia = DirectCast(other, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)
				flag = [String].Equals(Me.Text, syntaxTrivium.Text, StringComparison.Ordinal)
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Shared Narrowing Operator CType(ByVal trivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia) As Microsoft.CodeAnalysis.SyntaxTrivia
			' 
			' Current member / type: Microsoft.CodeAnalysis.SyntaxTrivia Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia::op_Explicit(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.SyntaxTrivia op_Explicit(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia)
			' 
			' The unary opperator AddressReference is not supported in VisualBasic
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\DetermineNotSupportedVBCodeStep.cs:ligne 22
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Operator

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me.Text)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me.Text)
		End Function

		Public NotOverridable Overrides Function ToFullString() As String
			Return Me._text
		End Function

		Public Overrides Function ToString() As String
			Return Me._text
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteString(Me._text)
		End Sub

		Protected Overrides Sub WriteTriviaTo(ByVal writer As TextWriter)
			writer.Write(Me.Text)
		End Sub
	End Class
End Namespace
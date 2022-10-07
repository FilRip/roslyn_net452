Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SourceFileBinder
		Inherits Binder
		Private ReadOnly _sourceFile As SourceFile

		Public Overrides ReadOnly Property OptionCompareText As Boolean
			Get
				Dim flag As Boolean
				flag = If(Not Me._sourceFile.OptionCompareText.HasValue, Me.m_containingBinder.OptionCompareText, Me._sourceFile.OptionCompareText.Value)
				Return flag
			End Get
		End Property

		Public Overrides ReadOnly Property OptionExplicit As Boolean
			Get
				Dim flag As Boolean
				flag = If(Not Me._sourceFile.OptionExplicit.HasValue, Me.m_containingBinder.OptionExplicit, Me._sourceFile.OptionExplicit.Value)
				Return flag
			End Get
		End Property

		Public Overrides ReadOnly Property OptionInfer As Boolean
			Get
				Dim flag As Boolean
				flag = If(Not Me._sourceFile.OptionInfer.HasValue, Me.m_containingBinder.OptionInfer, Me._sourceFile.OptionInfer.Value)
				Return flag
			End Get
		End Property

		Public Overrides ReadOnly Property OptionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict
			Get
				Dim optionStrict1 As Microsoft.CodeAnalysis.VisualBasic.OptionStrict
				If (Not Me._sourceFile.OptionStrict.HasValue) Then
					optionStrict1 = Me.m_containingBinder.OptionStrict
				Else
					optionStrict1 = If(Me._sourceFile.OptionStrict.Value, Microsoft.CodeAnalysis.VisualBasic.OptionStrict.[On], Microsoft.CodeAnalysis.VisualBasic.OptionStrict.Off)
				End If
				Return optionStrict1
			End Get
		End Property

		Public Overrides ReadOnly Property QuickAttributeChecker As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker
			Get
				Return Me._sourceFile.QuickAttributeChecker
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal sourceFile As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFile, ByVal tree As Microsoft.CodeAnalysis.SyntaxTree)
			MyBase.New(containingBinder, tree)
			Me._sourceFile = sourceFile
		End Sub

		Public Overrides Function GetSyntaxReference(ByVal node As VisualBasicSyntaxNode) As SyntaxReference
			Return MyBase.SyntaxTree.GetReference(node)
		End Function
	End Class
End Namespace
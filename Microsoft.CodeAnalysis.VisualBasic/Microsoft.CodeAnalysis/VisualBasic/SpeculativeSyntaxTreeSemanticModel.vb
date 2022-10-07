Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class SpeculativeSyntaxTreeSemanticModel
		Inherits SyntaxTreeSemanticModel
		Private ReadOnly _parentSemanticModel As SyntaxTreeSemanticModel

		Private ReadOnly _root As ExpressionSyntax

		Private ReadOnly _rootBinder As Binder

		Private ReadOnly _position As Integer

		Private ReadOnly _bindingOption As SpeculativeBindingOption

		Public Overrides ReadOnly Property IsSpeculativeSemanticModel As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property OriginalPositionForSpeculation As Integer
			Get
				Return Me._position
			End Get
		End Property

		Public Overrides ReadOnly Property ParentModel As SemanticModel
			Get
				Return Me._parentSemanticModel
			End Get
		End Property

		Friend Overrides ReadOnly Property Root As SyntaxNode
			Get
				Return Me._root
			End Get
		End Property

		Public Overrides ReadOnly Property SyntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Get
				Return Me._root.SyntaxTree
			End Get
		End Property

		Private Sub New(ByVal parentSemanticModel As SyntaxTreeSemanticModel, ByVal root As ExpressionSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal position As Integer, ByVal bindingOption As SpeculativeBindingOption)
			MyBase.New(parentSemanticModel.Compilation, DirectCast(parentSemanticModel.Compilation.SourceModule, SourceModuleSymbol), root.SyntaxTree, False)
			Me._parentSemanticModel = parentSemanticModel
			Me._root = root
			Me._rootBinder = binder
			Me._position = position
			Me._bindingOption = bindingOption
		End Sub

		Friend Overrides Function Bind(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal node As SyntaxNode, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundNode
			Return Me._parentSemanticModel.Bind(binder, node, diagnostics)
		End Function

		Public Shared Function Create(ByVal parentSemanticModel As SyntaxTreeSemanticModel, ByVal root As ExpressionSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal position As Integer, ByVal bindingOption As SpeculativeBindingOption) As SpeculativeSyntaxTreeSemanticModel
			Return New SpeculativeSyntaxTreeSemanticModel(parentSemanticModel, root, binder, position, bindingOption)
		End Function

		Public Overrides Function GetDeclarationDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Throw New NotSupportedException()
		End Function

		Public Overrides Function GetDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Throw New NotSupportedException()
		End Function

		Friend Overrides Function GetEnclosingBinder(ByVal position As Integer) As Binder
			Return Me._rootBinder
		End Function

		Friend Overrides Function GetExpressionConstantValue(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ConstantValue
			Return Me._parentSemanticModel.GetExpressionConstantValue(node, cancellationToken)
		End Function

		Friend Overrides Function GetExpressionMemberGroup(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Symbol)
			Return Me._parentSemanticModel.GetExpressionMemberGroup(node, cancellationToken)
		End Function

		Friend Overrides Function GetExpressionSymbolInfo(ByVal node As ExpressionSyntax, ByVal options As VBSemanticModel.SymbolInfoOptions, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As SymbolInfo
			' 
			' Current member / type: Microsoft.CodeAnalysis.SymbolInfo Microsoft.CodeAnalysis.VisualBasic.SpeculativeSyntaxTreeSemanticModel::GetExpressionSymbolInfo(Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax,Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel/SymbolInfoOptions,System.Threading.CancellationToken)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.SymbolInfo GetExpressionSymbolInfo(Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax,Microsoft.CodeAnalysis.VisualBasic.VBSemanticModel/SymbolInfoOptions,System.Threading.CancellationToken)
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

		Friend Overrides Function GetExpressionTypeInfo(ByVal node As ExpressionSyntax, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As VisualBasicTypeInfo
			Return Me._parentSemanticModel.GetSpeculativeTypeInfoWorker(Me._position, node, Me.GetSpeculativeBindingOption(node))
		End Function

		Private Function GetSpeculativeBindingOption(ByVal node As ExpressionSyntax) As Microsoft.CodeAnalysis.SpeculativeBindingOption
			Dim speculativeBindingOption As Microsoft.CodeAnalysis.SpeculativeBindingOption
			speculativeBindingOption = If(Not SyntaxFacts.IsInNamespaceOrTypeContext(node), Me._bindingOption, Microsoft.CodeAnalysis.SpeculativeBindingOption.BindAsTypeOrNamespace)
			Return speculativeBindingOption
		End Function

		Public Overrides Function GetSyntaxDiagnostics(Optional ByVal span As Nullable(Of TextSpan) = Nothing, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As ImmutableArray(Of Diagnostic)
			Throw New NotSupportedException()
		End Function
	End Class
End Namespace
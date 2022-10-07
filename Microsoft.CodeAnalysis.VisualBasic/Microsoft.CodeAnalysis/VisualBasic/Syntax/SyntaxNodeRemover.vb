Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public Class SyntaxNodeRemover
		Public Sub New()
			MyBase.New()
		End Sub

		Friend Shared Function RemoveNodes(Of TRoot As Microsoft.CodeAnalysis.SyntaxNode)(ByVal root As TRoot, ByVal nodes As IEnumerable(Of Microsoft.CodeAnalysis.SyntaxNode), ByVal options As SyntaxRemoveOptions) As TRoot
			Dim tRoot1 As TRoot
			If (CInt(nodes.ToArray().Length) <> 0) Then
				Dim syntaxRemover As SyntaxNodeRemover.SyntaxRemover = New SyntaxNodeRemover.SyntaxRemover(nodes.ToArray(), options)
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = syntaxRemover.Visit(DirectCast(root, Microsoft.CodeAnalysis.SyntaxNode))
				Dim residualTrivia As SyntaxTriviaList = syntaxRemover.ResidualTrivia
				If (residualTrivia.Count > 0) Then
					syntaxNode = syntaxNode.WithTrailingTrivia(DirectCast(syntaxNode.GetTrailingTrivia(), IEnumerable(Of SyntaxTrivia)).Concat(DirectCast(residualTrivia, IEnumerable(Of SyntaxTrivia))))
				End If
				tRoot1 = DirectCast(syntaxNode, TRoot)
			Else
				tRoot1 = root
			End If
			Return tRoot1
		End Function

		Private Class SyntaxRemover
			Inherits VisualBasicSyntaxRewriter
			Private ReadOnly _nodesToRemove As HashSet(Of SyntaxNode)

			Private ReadOnly _options As SyntaxRemoveOptions

			Private ReadOnly _searchSpan As TextSpan

			Private ReadOnly _residualTrivia As SyntaxTriviaListBuilder

			Private _directivesToKeep As HashSet(Of SyntaxNode)

			Friend ReadOnly Property ResidualTrivia As SyntaxTriviaList
				Get
					Dim syntaxTriviaLists As SyntaxTriviaList
					syntaxTriviaLists = If(Me._residualTrivia Is Nothing, New SyntaxTriviaList(), Me._residualTrivia.ToList())
					Return syntaxTriviaLists
				End Get
			End Property

			Public Sub New(ByVal nodes As SyntaxNode(), ByVal options As SyntaxRemoveOptions)
				' 
				' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNodeRemover/SyntaxRemover::.ctor(Microsoft.CodeAnalysis.SyntaxNode[],Microsoft.CodeAnalysis.SyntaxRemoveOptions)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Void .ctor(Microsoft.CodeAnalysis.SyntaxNode[],Microsoft.CodeAnalysis.SyntaxRemoveOptions)
				' 
				' L'index était hors limites. Il ne doit pas être négatif et doit être inférieur à la taille de la collection.
				' Nom du paramètre : index
				'    à System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
				'    à System.Collections.Generic.List`1.get_Item(Int32 index)
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CodePatterns\InitializationPattern.cs:ligne 109
				'    à ..( , Int32& , Statement& , Int32& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CodePatterns\InitializationPattern.cs:ligne 33
				'    à ..( , IEnumerable`1 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\DetermineCtorInvocationStep.cs:ligne 107
				'    à ..( , IEnumerable`1 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\DetermineCtorInvocationStep.cs:ligne 89
				'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\DetermineCtorInvocationStep.cs:ligne 62
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\DetermineCtorInvocationStep.cs:ligne 34
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
				'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
				'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Sub

			Private Sub AddDirectives(ByVal node As SyntaxNode, ByVal span As TextSpan)
				' 
				' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNodeRemover/SyntaxRemover::AddDirectives(Microsoft.CodeAnalysis.SyntaxNode,Microsoft.CodeAnalysis.Text.TextSpan)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Void AddDirectives(Microsoft.CodeAnalysis.SyntaxNode,Microsoft.CodeAnalysis.Text.TextSpan)
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

			End Sub

			Private Sub AddEndOfLine()
				If (Me._residualTrivia.Count = 0 OrElse Not SyntaxNodeRemover.SyntaxRemover.IsEndOfLine(Me._residualTrivia(Me._residualTrivia.Count - 1))) Then
					Me._residualTrivia.Add(SyntaxFactory.CarriageReturnLineFeed)
				End If
			End Sub

			Private Sub AddResidualTrivia(ByVal trivia As SyntaxTriviaList, Optional ByVal requiresNewLine As Boolean = False)
				If (requiresNewLine) Then
					Me.AddEndOfLine()
				End If
				Me._residualTrivia.Add(trivia)
			End Sub

			Private Sub AddTrivia(ByVal node As SyntaxNode)
				' 
				' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNodeRemover/SyntaxRemover::AddTrivia(Microsoft.CodeAnalysis.SyntaxNode)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Void AddTrivia(Microsoft.CodeAnalysis.SyntaxNode)
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

			End Sub

			Private Sub AddTrivia(ByVal token As SyntaxToken, ByVal node As SyntaxNode)
				' 
				' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNodeRemover/SyntaxRemover::AddTrivia(Microsoft.CodeAnalysis.SyntaxToken,Microsoft.CodeAnalysis.SyntaxNode)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Void AddTrivia(Microsoft.CodeAnalysis.SyntaxToken,Microsoft.CodeAnalysis.SyntaxNode)
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

			End Sub

			Private Sub AddTrivia(ByVal node As SyntaxNode, ByVal token As SyntaxToken)
				' 
				' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNodeRemover/SyntaxRemover::AddTrivia(Microsoft.CodeAnalysis.SyntaxNode,Microsoft.CodeAnalysis.SyntaxToken)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Void AddTrivia(Microsoft.CodeAnalysis.SyntaxNode,Microsoft.CodeAnalysis.SyntaxToken)
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

			End Sub

			Private Shared Function ComputeTotalSpan(ByVal nodes As SyntaxNode()) As Microsoft.CodeAnalysis.Text.TextSpan
				Dim fullSpan As Microsoft.CodeAnalysis.Text.TextSpan = nodes(0).FullSpan
				Dim start As Integer = fullSpan.Start
				Dim [end] As Integer = fullSpan.[End]
				Dim num As Integer = 1
				Do
					Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan = nodes(num).FullSpan
					start = Math.Min(start, textSpan.Start)
					[end] = Math.Max([end], textSpan.[End])
					num = num + 1
				Loop While num < CInt(nodes.Length)
				Return New Microsoft.CodeAnalysis.Text.TextSpan(start, [end] - start)
			End Function

			Private Function GetRemovedSpan(ByVal span As TextSpan, ByVal fullSpan As TextSpan) As TextSpan
				' 
				' Current member / type: Microsoft.CodeAnalysis.Text.TextSpan Microsoft.CodeAnalysis.VisualBasic.Syntax.SyntaxNodeRemover/SyntaxRemover::GetRemovedSpan(Microsoft.CodeAnalysis.Text.TextSpan,Microsoft.CodeAnalysis.Text.TextSpan)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: Microsoft.CodeAnalysis.Text.TextSpan GetRemovedSpan(Microsoft.CodeAnalysis.Text.TextSpan,Microsoft.CodeAnalysis.Text.TextSpan)
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

			Private Shared Function HasEndOfLine(ByVal trivia As SyntaxTriviaList) As Boolean
				Dim func As Func(Of SyntaxTrivia, Boolean)
				Dim syntaxTrivias As IEnumerable(Of SyntaxTrivia) = DirectCast(trivia, IEnumerable(Of SyntaxTrivia))
				If (SyntaxNodeRemover.SyntaxRemover._Closure$__.$I12-0 Is Nothing) Then
					func = Function(t As SyntaxTrivia) SyntaxNodeRemover.SyntaxRemover.IsEndOfLine(t)
					SyntaxNodeRemover.SyntaxRemover._Closure$__.$I12-0 = func
				Else
					func = SyntaxNodeRemover.SyntaxRemover._Closure$__.$I12-0
				End If
				Return syntaxTrivias.Any(func)
			End Function

			Private Shared Function HasRelatedDirectives(ByVal directive As DirectiveTriviaSyntax) As Boolean
				Dim flag As Boolean
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = directive.Kind()
				flag = If(CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IfDirectiveTrivia) <= 4 OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRegionDirectiveTrivia, True, False)
				Return flag
			End Function

			Private Shared Function IsEndOfLine(ByVal trivia As SyntaxTrivia) As Boolean
				If (trivia.Kind() = SyntaxKind.EndOfLineTrivia OrElse trivia.Kind() = SyntaxKind.CommentTrivia) Then
					Return True
				End If
				Return trivia.IsDirective
			End Function

			Private Function IsForRemoval(ByVal node As SyntaxNode) As Boolean
				Return Me._nodesToRemove.Contains(node)
			End Function

			Private Function ShouldVisit(ByVal node As SyntaxNode) As Boolean
				If (node.FullSpan.IntersectsWith(Me._searchSpan)) Then
					Return True
				End If
				If (Me._residualTrivia Is Nothing) Then
					Return False
				End If
				Return Me._residualTrivia.Count > 0
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.SyntaxNode) As Microsoft.CodeAnalysis.SyntaxNode
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node
				If (node IsNot Nothing) Then
					If (Me.IsForRemoval(node)) Then
						Me.AddTrivia(node)
						syntaxNode = Nothing
					ElseIf (Me.ShouldVisit(node)) Then
						syntaxNode = MyBase.Visit(node)
					End If
				End If
				Return syntaxNode
			End Function

			Public Overrides Function VisitList(Of TNode As SyntaxNode)(ByVal list As SeparatedSyntaxList(Of TNode)) As SeparatedSyntaxList(Of TNode)
				Dim tNodes As SeparatedSyntaxList(Of TNode)
				Dim flag As Boolean
				Dim flag1 As Boolean
				Dim item As Microsoft.CodeAnalysis.SyntaxNodeOrToken
				Dim withSeparators As SyntaxNodeOrTokenList = list.GetWithSeparators()
				Dim flag2 As Boolean = False
				Dim syntaxNodeOrTokenListBuilder As Microsoft.CodeAnalysis.Syntax.SyntaxNodeOrTokenListBuilder = Nothing
				Dim count As Integer = withSeparators.Count
				Dim num As Integer = count - 1
				Dim num1 As Integer = 0
				Do
					Dim syntaxNodeOrToken As Microsoft.CodeAnalysis.SyntaxNodeOrToken = withSeparators(num1)
					Dim syntaxNodeOrToken1 As Microsoft.CodeAnalysis.SyntaxNodeOrToken = New Microsoft.CodeAnalysis.SyntaxNodeOrToken()
					If (Not syntaxNodeOrToken.IsToken) Then
						Dim tNode1 As TNode = DirectCast(syntaxNodeOrToken.AsNode(), TNode)
						If (Not Me.IsForRemoval(DirectCast(tNode1, SyntaxNode))) Then
							syntaxNodeOrToken1 = DirectCast(Me.VisitListElement(Of TNode)(tNode1), SyntaxNode)
						Else
							If (syntaxNodeOrTokenListBuilder Is Nothing) Then
								syntaxNodeOrTokenListBuilder = New Microsoft.CodeAnalysis.Syntax.SyntaxNodeOrTokenListBuilder(count)
								syntaxNodeOrTokenListBuilder.Add(withSeparators, 0, num1)
							End If
							CommonSyntaxNodeRemover.GetSeparatorInfo(withSeparators, num1, 730, flag, flag1)
							If (Not flag1 AndAlso syntaxNodeOrTokenListBuilder.Count > 0 AndAlso syntaxNodeOrTokenListBuilder(syntaxNodeOrTokenListBuilder.Count - 1).IsToken) Then
								item = syntaxNodeOrTokenListBuilder(syntaxNodeOrTokenListBuilder.Count - 1)
								Me.AddTrivia(item.AsToken(), DirectCast(tNode1, SyntaxNode))
								syntaxNodeOrTokenListBuilder.RemoveLast()
							ElseIf (Not flag) Then
								Me.AddTrivia(DirectCast(tNode1, SyntaxNode))
							Else
								item = withSeparators(num1 + 1)
								Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = item.AsToken()
								Me.AddTrivia(DirectCast(tNode1, SyntaxNode), syntaxToken)
								flag2 = True
							End If
							syntaxNodeOrToken1 = New Microsoft.CodeAnalysis.SyntaxNodeOrToken()
						End If
					ElseIf (Not flag2) Then
						syntaxNodeOrToken1 = Me.VisitListSeparator(syntaxNodeOrToken.AsToken())
					Else
						flag2 = False
						syntaxNodeOrToken1 = New Microsoft.CodeAnalysis.SyntaxNodeOrToken()
					End If
					If (syntaxNodeOrToken <> syntaxNodeOrToken1 AndAlso syntaxNodeOrTokenListBuilder Is Nothing) Then
						syntaxNodeOrTokenListBuilder = New Microsoft.CodeAnalysis.Syntax.SyntaxNodeOrTokenListBuilder(count)
						syntaxNodeOrTokenListBuilder.Add(withSeparators, 0, num1)
					End If
					If (syntaxNodeOrTokenListBuilder IsNot Nothing AndAlso Not syntaxNodeOrToken1.IsKind(SyntaxKind.None)) Then
						syntaxNodeOrTokenListBuilder.Add(syntaxNodeOrToken1)
					End If
					num1 = num1 + 1
				Loop While num1 <= num
				tNodes = If(syntaxNodeOrTokenListBuilder Is Nothing, list, syntaxNodeOrTokenListBuilder.ToList().AsSeparatedList(Of TNode)())
				Return tNodes
			End Function

			Public Overrides Function VisitToken(ByVal token As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.SyntaxToken
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = token
				If (Me.VisitIntoStructuredTrivia) Then
					syntaxToken = MyBase.VisitToken(token)
				End If
				If (syntaxToken.Kind() <> SyntaxKind.None AndAlso Me._residualTrivia IsNot Nothing AndAlso Me._residualTrivia.Count > 0) Then
					Dim syntaxTriviaListBuilder As Microsoft.CodeAnalysis.Syntax.SyntaxTriviaListBuilder = Me._residualTrivia
					Dim leadingTrivia As SyntaxTriviaList = syntaxToken.LeadingTrivia
					syntaxTriviaListBuilder.Add(leadingTrivia)
					syntaxToken = syntaxToken.WithLeadingTrivia(Me._residualTrivia.ToList())
					Me._residualTrivia.Clear()
				End If
				Return syntaxToken
			End Function
		End Class
	End Class
End Namespace
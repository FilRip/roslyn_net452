Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class SyntaxToken
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Private ReadOnly _text As String

		Private ReadOnly _trailingTriviaOrTriviaInfo As Object

		Private ReadOnly Property _leadingTriviaWidth As Integer
			Get
				Dim triviaInfo As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo = TryCast(Me._trailingTriviaOrTriviaInfo, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo)
				Return If(triviaInfo Is Nothing, 0, triviaInfo._leadingTrivia.FullWidth)
			End Get
		End Property

		Friend ReadOnly Property IsEndOfLine As Boolean
			Get
				If (MyBase.Kind = SyntaxKind.StatementTerminatorToken) Then
					Return True
				End If
				Return MyBase.Kind = SyntaxKind.EndOfFileToken
			End Get
		End Property

		Friend ReadOnly Property IsEndOfParse As Boolean
			Get
				Return MyBase.Kind = SyntaxKind.EndOfFileToken
			End Get
		End Property

		Friend Overridable ReadOnly Property IsKeyword As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsToken As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overridable ReadOnly Property ObjectValue As Object
			Get
				Return Me.ValueText
			End Get
		End Property

		Friend Overrides ReadOnly Property ShouldReuseInSerialization As Boolean
			Get
				If (Not MyBase.ShouldReuseInSerialization) Then
					Return False
				End If
				Return MyBase.FullWidth < 42
			End Get
		End Property

		Friend ReadOnly Property Text As String
			Get
				Return Me._text
			End Get
		End Property

		Friend Overridable ReadOnly Property ValueText As String
			Get
				Return Me.Text
			End Get
		End Property

		Protected Sub New(ByVal kind As SyntaxKind, ByVal text As String, ByVal precedingTrivia As GreenNode, ByVal followingTrivia As GreenNode)
			MyBase.New(kind, text.Length)
			MyBase.SetFlags(GreenNode.NodeFlags.IsNotMissing)
			Me._text = text
			If (followingTrivia IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(followingTrivia)
				Me._trailingTriviaOrTriviaInfo = followingTrivia
			End If
			If (precedingTrivia IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(precedingTrivia)
				Me._trailingTriviaOrTriviaInfo = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.Create(precedingTrivia, DirectCast(Me._trailingTriviaOrTriviaInfo, GreenNode))
			End If
			Me.ClearFlagIfMissing()
		End Sub

		Protected Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal text As String, ByVal precedingTrivia As GreenNode, ByVal followingTrivia As GreenNode)
			MyBase.New(kind, errors, text.Length)
			MyBase.SetFlags(GreenNode.NodeFlags.IsNotMissing)
			Me._text = text
			If (followingTrivia IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(followingTrivia)
				Me._trailingTriviaOrTriviaInfo = followingTrivia
			End If
			If (precedingTrivia IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(precedingTrivia)
				Me._trailingTriviaOrTriviaInfo = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.Create(precedingTrivia, DirectCast(Me._trailingTriviaOrTriviaInfo, GreenNode))
			End If
			Me.ClearFlagIfMissing()
		End Sub

		Protected Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String, ByVal precedingTrivia As GreenNode, ByVal followingTrivia As GreenNode)
			MyBase.New(kind, errors, annotations, text.Length)
			MyBase.SetFlags(GreenNode.NodeFlags.IsNotMissing)
			Me._text = text
			If (followingTrivia IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(followingTrivia)
				Me._trailingTriviaOrTriviaInfo = followingTrivia
			End If
			If (precedingTrivia IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(precedingTrivia)
				Me._trailingTriviaOrTriviaInfo = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.Create(precedingTrivia, DirectCast(Me._trailingTriviaOrTriviaInfo, GreenNode))
			End If
			Me.ClearFlagIfMissing()
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			MyBase.SetFlags(Microsoft.CodeAnalysis.GreenNode.NodeFlags.IsNotMissing)
			Me._text = reader.ReadString()
			MyBase.FullWidth = Me._text.Length
			Me._trailingTriviaOrTriviaInfo = RuntimeHelpers.GetObjectValue(reader.ReadValue())
			Dim triviaInfo As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo = TryCast(Me._trailingTriviaOrTriviaInfo, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo)
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = If(triviaInfo IsNot Nothing, triviaInfo._trailingTrivia, TryCast(Me._trailingTriviaOrTriviaInfo, Microsoft.CodeAnalysis.GreenNode))
			If (triviaInfo IsNot Nothing) Then
				greenNode = triviaInfo._leadingTrivia
			Else
				greenNode = Nothing
			End If
			Dim greenNode2 As Microsoft.CodeAnalysis.GreenNode = greenNode
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
			End If
			If (greenNode2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode2)
			End If
			Me.ClearFlagIfMissing()
		End Sub

		Public NotOverridable Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitSyntaxToken(Me)
		End Function

		Public Shared Function AddLeadingTrivia(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(ByVal token As T, ByVal newTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As T
			Dim t1 As T
			Dim node As GreenNode
			If (newTrivia.Node IsNot Nothing) Then
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(token.GetLeadingTrivia())
				If (syntaxList.Node IsNot Nothing) Then
					Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).Create()
					syntaxListBuilder.AddRange(newTrivia)
					syntaxListBuilder.AddRange(syntaxList)
					node = syntaxListBuilder.ToList().Node
				Else
					node = newTrivia.Node
				End If
				t1 = DirectCast(token.WithLeadingTrivia(node), T)
			Else
				t1 = token
			End If
			Return t1
		End Function

		Friend NotOverridable Overrides Sub AddSyntaxErrors(ByVal accumulatedErrors As List(Of DiagnosticInfo))
			If (MyBase.GetDiagnostics() IsNot Nothing) Then
				accumulatedErrors.AddRange(MyBase.GetDiagnostics())
			End If
			Dim leadingTrivia As GreenNode = Me.GetLeadingTrivia()
			If (leadingTrivia IsNot Nothing) Then
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(leadingTrivia)
				Dim count As Integer = syntaxList.Count - 1
				For i As Integer = 0 To count
					DirectCast(syntaxList.ItemUntyped(i), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).AddSyntaxErrors(accumulatedErrors)
				Next

			End If
			Dim trailingTrivia As GreenNode = Me.GetTrailingTrivia()
			If (trailingTrivia IsNot Nothing) Then
				Dim syntaxList1 As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(trailingTrivia)
				Dim num As Integer = syntaxList1.Count - 1
				For j As Integer = 0 To num
					DirectCast(syntaxList1.ItemUntyped(j), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).AddSyntaxErrors(accumulatedErrors)
				Next

			End If
		End Sub

		Public Shared Function AddTrailingTrivia(Of T As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)(ByVal token As T, ByVal newTrivia As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)) As T
			Dim t1 As T
			Dim node As GreenNode
			If (newTrivia.Node IsNot Nothing) Then
				Dim syntaxList As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)(token.GetTrailingTrivia())
				If (syntaxList.Node IsNot Nothing) Then
					Dim syntaxListBuilder As SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) = SyntaxListBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode).Create()
					syntaxListBuilder.AddRange(syntaxList)
					syntaxListBuilder.AddRange(newTrivia)
					node = syntaxListBuilder.ToList().Node
				Else
					node = newTrivia.Node
				End If
				t1 = DirectCast(token.WithTrailingTrivia(node), T)
			Else
				t1 = token
			End If
			Return t1
		End Function

		Private Sub ClearFlagIfMissing()
			If (Me.Text.Length = 0 AndAlso MyBase.Kind <> SyntaxKind.EndOfFileToken AndAlso MyBase.Kind <> SyntaxKind.EmptyToken) Then
				MyBase.ClearFlags(GreenNode.NodeFlags.IsNotMissing)
			End If
		End Sub

		Friend Shared Function Create(ByVal kind As SyntaxKind, Optional ByVal leading As GreenNode = Nothing, Optional ByVal trailing As GreenNode = Nothing, Optional ByVal text As String = Nothing) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Dim str As String = If(text Is Nothing, SyntaxFacts.GetText(kind), text)
			If (kind >= SyntaxKind.AddHandlerKeyword) Then
				If (kind <= SyntaxKind.YieldKeyword OrElse kind = SyntaxKind.NameOfKeyword) Then
					keywordSyntax = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax(kind, str, leading, trailing)
				Else
					If (kind > SyntaxKind.EndOfXmlToken AndAlso kind <> SyntaxKind.EndOfInterpolatedStringToken AndAlso kind <> SyntaxKind.DollarSignDoubleQuoteToken) Then
						Throw ExceptionUtilities.UnexpectedValue(kind)
					End If
					keywordSyntax = New PunctuationSyntax(kind, str, leading, trailing)
				End If
				Return keywordSyntax
			End If
			Throw ExceptionUtilities.UnexpectedValue(kind)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal position As Integer) As SyntaxNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend NotOverridable Overrides Function GetLeadingTrivia() As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim triviaInfo As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo = TryCast(Me._trailingTriviaOrTriviaInfo, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo)
			If (triviaInfo Is Nothing) Then
				greenNode = Nothing
			Else
				greenNode = triviaInfo._leadingTrivia
			End If
			Return greenNode
		End Function

		Public NotOverridable Overrides Function GetLeadingTriviaWidth() As Integer
			Return Me._leadingTriviaWidth
		End Function

		Friend NotOverridable Overrides Function GetSlot(ByVal index As Integer) As GreenNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend NotOverridable Overrides Function GetTrailingTrivia() As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = TryCast(Me._trailingTriviaOrTriviaInfo, Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 Is Nothing) Then
				Dim triviaInfo As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo = TryCast(Me._trailingTriviaOrTriviaInfo, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo)
				If (triviaInfo Is Nothing) Then
					greenNode = Nothing
				Else
					greenNode = triviaInfo._trailingTrivia
				End If
			Else
				greenNode = greenNode1
			End If
			Return greenNode
		End Function

		Public NotOverridable Overrides Function GetTrailingTriviaWidth() As Integer
			Return MyBase.FullWidth - Me._text.Length - Me._leadingTriviaWidth
		End Function

		Public Overrides Function GetValue() As Object
			Return Me.ObjectValue
		End Function

		Public Overrides Function GetValueText() As String
			Return Me.ValueText
		End Function

		Public Function IsBinaryOperator() As Boolean
			Dim flag As Boolean
			Dim kind As SyntaxKind = MyBase.Kind
			If (kind <= SyntaxKind.LikeKeyword) Then
				If (kind > SyntaxKind.IsKeyword) Then
					If (kind = SyntaxKind.IsNotKeyword OrElse kind = SyntaxKind.LikeKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				Else
					If (CUShort(kind) - CUShort(SyntaxKind.AndKeyword) <= CUShort(SyntaxKind.List) OrElse kind = SyntaxKind.IsKeyword) Then
						flag = True
						Return flag
					End If
					flag = False
					Return flag
				End If
			ElseIf (kind <= SyntaxKind.OrElseKeyword) Then
				If (kind = SyntaxKind.ModKeyword OrElse CUShort(kind) - CUShort(SyntaxKind.OrKeyword) <= CUShort(SyntaxKind.List)) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (kind <> SyntaxKind.XorKeyword) Then
				Select Case kind
					Case SyntaxKind.AmpersandToken
					Case SyntaxKind.AsteriskToken
					Case SyntaxKind.PlusToken
					Case SyntaxKind.MinusToken
					Case SyntaxKind.SlashToken
					Case SyntaxKind.LessThanToken
					Case SyntaxKind.LessThanEqualsToken
					Case SyntaxKind.LessThanGreaterThanToken
					Case SyntaxKind.EqualsToken
					Case SyntaxKind.GreaterThanToken
					Case SyntaxKind.GreaterThanEqualsToken
					Case SyntaxKind.BackslashToken
					Case SyntaxKind.CaretToken
						Exit Select
					Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndIfStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndWithStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.EndSubStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndSetStatement Or SyntaxKind.EndPropertyStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.EndAddHandlerStatement Or SyntaxKind.EndRemoveHandlerStatement Or SyntaxKind.EndRaiseEventStatement Or SyntaxKind.EndWhileStatement Or SyntaxKind.EndTryStatement Or SyntaxKind.EndSyncLockStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NamespaceBlock Or SyntaxKind.NamespaceStatement Or SyntaxKind.ModuleBlock Or SyntaxKind.StructureBlock Or SyntaxKind.InterfaceBlock Or SyntaxKind.ClassBlock Or SyntaxKind.EnumBlock Or SyntaxKind.InheritsStatement Or SyntaxKind.ImplementsStatement Or SyntaxKind.ModuleStatement Or SyntaxKind.StructureStatement Or SyntaxKind.InterfaceStatement Or SyntaxKind.ClassStatement Or SyntaxKind.EnumStatement Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameter Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.TypeParameterMultipleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.TypeConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.SubBlock Or SyntaxKind.FunctionBlock Or SyntaxKind.ConstructorBlock Or SyntaxKind.OperatorBlock Or SyntaxKind.GetAccessorBlock Or SyntaxKind.SetAccessorBlock Or SyntaxKind.AddHandlerAccessorBlock Or SyntaxKind.RemoveHandlerAccessorBlock Or SyntaxKind.RaiseEventAccessorBlock Or SyntaxKind.PropertyBlock Or SyntaxKind.EventBlock Or SyntaxKind.ParameterList Or SyntaxKind.SubStatement Or SyntaxKind.FunctionStatement Or SyntaxKind.SubNewStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.DelegateFunctionStatement Or SyntaxKind.EventStatement Or SyntaxKind.OperatorStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.AddHandlerAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement Or SyntaxKind.RaiseEventAccessorStatement Or SyntaxKind.ImplementsClause Or SyntaxKind.HandlesClause Or SyntaxKind.KeywordEventContainer Or SyntaxKind.WithEventsEventContainer Or SyntaxKind.WithEventsPropertyEventContainer Or SyntaxKind.HandlesClauseItem Or SyntaxKind.IncompleteMember Or SyntaxKind.FieldDeclaration Or SyntaxKind.VariableDeclarator Or SyntaxKind.SimpleAsClause Or SyntaxKind.AsNewClause Or SyntaxKind.ObjectMemberInitializer Or SyntaxKind.ObjectCollectionInitializer Or SyntaxKind.InferredFieldInitializer Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.ObjectKeyword Or SyntaxKind.OfKeyword Or SyntaxKind.OnKeyword Or SyntaxKind.OperatorKeyword Or SyntaxKind.OptionKeyword Or SyntaxKind.OptionalKeyword Or SyntaxKind.OrKeyword Or SyntaxKind.OrElseKeyword Or SyntaxKind.OverloadsKeyword Or SyntaxKind.OverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PartialKeyword Or SyntaxKind.PrivateKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.PublicKeyword Or SyntaxKind.RaiseEventKeyword Or SyntaxKind.ReadOnlyKeyword Or SyntaxKind.ReDimKeyword Or SyntaxKind.REMKeyword Or SyntaxKind.RemoveHandlerKeyword Or SyntaxKind.ResumeKeyword Or SyntaxKind.ReturnKeyword Or SyntaxKind.SByteKeyword Or SyntaxKind.SelectKeyword Or SyntaxKind.SetKeyword Or SyntaxKind.ShadowsKeyword Or SyntaxKind.SharedKeyword Or SyntaxKind.ShortKeyword Or SyntaxKind.SingleKeyword Or SyntaxKind.StaticKeyword Or SyntaxKind.StepKeyword Or SyntaxKind.StopKeyword Or SyntaxKind.StringKeyword Or SyntaxKind.StructureKeyword Or SyntaxKind.SubKeyword Or SyntaxKind.SyncLockKeyword Or SyntaxKind.ThenKeyword Or SyntaxKind.ThrowKeyword Or SyntaxKind.ToKeyword Or SyntaxKind.TrueKeyword Or SyntaxKind.TryKeyword Or SyntaxKind.TryCastKeyword Or SyntaxKind.TypeOfKeyword Or SyntaxKind.UIntegerKeyword Or SyntaxKind.ULongKeyword Or SyntaxKind.UShortKeyword Or SyntaxKind.UsingKeyword Or SyntaxKind.WhenKeyword Or SyntaxKind.WhileKeyword Or SyntaxKind.WideningKeyword Or SyntaxKind.WithKeyword Or SyntaxKind.WithEventsKeyword Or SyntaxKind.WriteOnlyKeyword Or SyntaxKind.XorKeyword Or SyntaxKind.EndIfKeyword Or SyntaxKind.GosubKeyword Or SyntaxKind.VariantKeyword Or SyntaxKind.WendKeyword Or SyntaxKind.AggregateKeyword Or SyntaxKind.AllKeyword Or SyntaxKind.AnsiKeyword Or SyntaxKind.AscendingKeyword Or SyntaxKind.AssemblyKeyword Or SyntaxKind.AutoKeyword Or SyntaxKind.BinaryKeyword Or SyntaxKind.ByKeyword Or SyntaxKind.CompareKeyword Or SyntaxKind.CustomKeyword Or SyntaxKind.DescendingKeyword Or SyntaxKind.DisableKeyword Or SyntaxKind.DistinctKeyword Or SyntaxKind.EnableKeyword Or SyntaxKind.EqualsKeyword Or SyntaxKind.ExplicitKeyword Or SyntaxKind.ExternalSourceKeyword Or SyntaxKind.ExternalChecksumKeyword Or SyntaxKind.FromKeyword Or SyntaxKind.GroupKeyword Or SyntaxKind.InferKeyword Or SyntaxKind.IntoKeyword Or SyntaxKind.IsFalseKeyword Or SyntaxKind.IsTrueKeyword Or SyntaxKind.JoinKeyword Or SyntaxKind.KeyKeyword Or SyntaxKind.MidKeyword Or SyntaxKind.OffKeyword Or SyntaxKind.OrderKeyword Or SyntaxKind.OutKeyword Or SyntaxKind.PreserveKeyword Or SyntaxKind.RegionKeyword Or SyntaxKind.SkipKeyword Or SyntaxKind.StrictKeyword Or SyntaxKind.TakeKeyword Or SyntaxKind.TextKeyword Or SyntaxKind.UnicodeKeyword Or SyntaxKind.UntilKeyword Or SyntaxKind.WarningKeyword Or SyntaxKind.WhereKeyword Or SyntaxKind.TypeKeyword Or SyntaxKind.XmlKeyword Or SyntaxKind.AsyncKeyword Or SyntaxKind.AwaitKeyword Or SyntaxKind.IteratorKeyword Or SyntaxKind.YieldKeyword Or SyntaxKind.ExclamationToken Or SyntaxKind.AtToken Or SyntaxKind.CommaToken Or SyntaxKind.HashToken Or SyntaxKind.AmpersandToken
					Case SyntaxKind.NamedFieldInitializer Or SyntaxKind.NotKeyword
					Case SyntaxKind.SingleQuoteToken
					Case SyntaxKind.OpenParenToken
					Case SyntaxKind.CloseParenToken
					Case SyntaxKind.OpenBraceToken
					Case SyntaxKind.CloseBraceToken
					Case SyntaxKind.SemicolonToken
					Case SyntaxKind.DotToken
					Case SyntaxKind.ColonToken
					Case SyntaxKind.EndFunctionStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.Parameter Or SyntaxKind.UsingBlock Or SyntaxKind.LabelStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.OpenBraceToken Or SyntaxKind.EqualsToken
					Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndFunctionStatement Or SyntaxKind.EndGetStatement Or SyntaxKind.EndOperatorStatement Or SyntaxKind.EndEventStatement Or SyntaxKind.NamedFieldInitializer Or SyntaxKind.EqualsValue Or SyntaxKind.Parameter Or SyntaxKind.ModifiedIdentifier Or SyntaxKind.UsingBlock Or SyntaxKind.SyncLockBlock Or SyntaxKind.LabelStatement Or SyntaxKind.GoToStatement Or SyntaxKind.NotKeyword Or SyntaxKind.NothingKeyword Or SyntaxKind.NotInheritableKeyword Or SyntaxKind.NotOverridableKeyword Or SyntaxKind.OverridesKeyword Or SyntaxKind.ParamArrayKeyword Or SyntaxKind.PropertyKeyword Or SyntaxKind.ProtectedKeyword Or SyntaxKind.SingleQuoteToken Or SyntaxKind.OpenBraceToken Or SyntaxKind.CloseBraceToken Or SyntaxKind.EqualsToken Or SyntaxKind.GreaterThanToken
						flag = False
						Return flag
					Case Else
						If (CUShort(kind) - CUShort(SyntaxKind.LessThanLessThanToken) > CUShort(SyntaxKind.List)) Then
							flag = False
							Return flag
						Else
							Exit Select
						End If
				End Select
			End If
			flag = True
			Return flag
		End Function

		Public Overrides Function IsEquivalentTo(ByVal other As GreenNode) As Boolean
			Dim flag As Boolean
			If (MyBase.IsEquivalentTo(other)) Then
				Dim syntaxToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken = DirectCast(other, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)
				If (Not [String].Equals(Me.Text, syntaxToken.Text, StringComparison.Ordinal)) Then
					flag = False
				ElseIf (MyBase.HasLeadingTrivia <> syntaxToken.HasLeadingTrivia OrElse MyBase.HasTrailingTrivia <> syntaxToken.HasTrailingTrivia) Then
					flag = False
				ElseIf (Not MyBase.HasLeadingTrivia OrElse Me.GetLeadingTrivia().IsEquivalentTo(syntaxToken.GetLeadingTrivia())) Then
					flag = If(Not MyBase.HasTrailingTrivia OrElse Me.GetTrailingTrivia().IsEquivalentTo(syntaxToken.GetTrailingTrivia()), True, False)
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Shared Narrowing Operator CType(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Nothing, token, 0, 0)
		End Operator

		Public Overrides Function ToString() As String
			Return Me._text
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteString(Me._text)
			writer.WriteValue(RuntimeHelpers.GetObjectValue(Me._trailingTriviaOrTriviaInfo))
		End Sub

		Protected Overrides Sub WriteTokenTo(ByVal writer As TextWriter, ByVal leading As Boolean, ByVal trailing As Boolean)
			If (leading) Then
				Dim leadingTrivia As GreenNode = Me.GetLeadingTrivia()
				If (leadingTrivia IsNot Nothing) Then
					leadingTrivia.WriteTo(writer, True, True)
				End If
			End If
			writer.Write(Me.Text)
			If (trailing) Then
				Dim trailingTrivia As GreenNode = Me.GetTrailingTrivia()
				If (trailingTrivia IsNot Nothing) Then
					trailingTrivia.WriteTo(writer, True, True)
				End If
			End If
		End Sub

		Friend Class TriviaInfo
			Implements IObjectWritable
			Private Const s_maximumCachedTriviaWidth As Integer = 40

			Private Const s_triviaInfoCacheSize As Integer = 64

			Private ReadOnly Shared s_triviaKeyHasher As Func(Of GreenNode, Integer)

			Private ReadOnly Shared s_triviaKeyEquality As Func(Of GreenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo, Boolean)

			Private ReadOnly Shared s_triviaInfoCache As CachingFactory(Of GreenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo)

			Public _leadingTrivia As GreenNode

			Public _trailingTrivia As GreenNode

			ReadOnly Property IObjectWritable_ShouldReuseInSerialization As Boolean Implements IObjectWritable.ShouldReuseInSerialization
				Get
					Return Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.ShouldCacheTriviaInfo(Me._leadingTrivia, Me._trailingTrivia)
				End Get
			End Property

			Shared Sub New()
				Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.s_triviaKeyHasher = Function(key As GreenNode) Hash.Combine(Of String)(key.ToFullString(), CShort(key.RawKind))
				Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.s_triviaKeyEquality = Function(key As GreenNode, value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo)
					If (key = value._leadingTrivia) Then
						Return True
					End If
					If (key.RawKind <> value._leadingTrivia.RawKind OrElse key.FullWidth <> value._leadingTrivia.FullWidth) Then
						Return False
					End If
					Return EmbeddedOperators.CompareString(key.ToFullString(), value._leadingTrivia.ToFullString(), False) = 0
				End Function
				Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.s_triviaInfoCache = New CachingFactory(Of GreenNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo)(64, Nothing, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.s_triviaKeyHasher, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.s_triviaKeyEquality)
				ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo(r))
			End Sub

			Private Sub New(ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode)
				MyBase.New()
				Me._leadingTrivia = leadingTrivia
				Me._trailingTrivia = trailingTrivia
			End Sub

			Public Sub New(ByVal reader As ObjectReader)
				MyBase.New()
				Me._leadingTrivia = DirectCast(reader.ReadValue(), GreenNode)
				Me._trailingTrivia = DirectCast(reader.ReadValue(), GreenNode)
			End Sub

			Public Shared Function Create(ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo
				Dim triviaInfo As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo
				If (Not Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.ShouldCacheTriviaInfo(leadingTrivia, trailingTrivia)) Then
					triviaInfo = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo(leadingTrivia, trailingTrivia)
				Else
					Dim triviaInfo1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo = Nothing
					SyncLock Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.s_triviaInfoCache
						If (Not Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.s_triviaInfoCache.TryGetValue(leadingTrivia, triviaInfo1)) Then
							triviaInfo1 = New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo(leadingTrivia, trailingTrivia)
							Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken.TriviaInfo.s_triviaInfoCache.Add(leadingTrivia, triviaInfo1)
						End If
					End SyncLock
					triviaInfo = triviaInfo1
				End If
				Return triviaInfo
			End Function

			Private Shared Function ShouldCacheTriviaInfo(ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode) As Boolean
				Dim flag As Boolean
				If (trailingTrivia IsNot Nothing) Then
					flag = If(leadingTrivia.RawKind <> 729 OrElse leadingTrivia.Flags <> GreenNode.NodeFlags.IsNotMissing OrElse trailingTrivia.RawKind <> 729 OrElse trailingTrivia.Flags <> GreenNode.NodeFlags.IsNotMissing OrElse trailingTrivia.FullWidth <> 1 OrElse EmbeddedOperators.CompareString(trailingTrivia.ToFullString(), " ", False) <> 0, False, leadingTrivia.FullWidth <= 40)
				Else
					flag = If(leadingTrivia.RawKind <> 734 OrElse leadingTrivia.Flags <> GreenNode.NodeFlags.IsNotMissing, False, leadingTrivia.FullWidth <= 40)
				End If
				Return flag
			End Function

			Public Sub WriteTo(ByVal writer As ObjectWriter) Implements IObjectWritable.WriteTo
				writer.WriteValue(DirectCast(Me._leadingTrivia, IObjectWritable))
				writer.WriteValue(DirectCast(Me._trailingTrivia, IObjectWritable))
			End Sub
		End Class
	End Class
End Namespace
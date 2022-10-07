Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class InitializerSemanticModel
		Inherits MemberSemanticModel
		Private Sub New(ByVal root As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, Optional ByVal containingSemanticModelOpt As SyntaxTreeSemanticModel = Nothing, Optional ByVal parentSemanticModelOpt As SyntaxTreeSemanticModel = Nothing, Optional ByVal speculatedPosition As Integer = 0, Optional ByVal ignoreAccessibility As Boolean = False)
			MyBase.New(root, binder, containingSemanticModelOpt, parentSemanticModelOpt, speculatedPosition, ignoreAccessibility)
		End Sub

		Friend Overrides Function Bind(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal node As SyntaxNode, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue) Then
					boundNode1 = Me.BindInitializer(binder, node, diagnostics)
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter) Then
					Dim parameterSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)
					boundNode1 = Me.BindInitializer(binder, parameterSyntax.[Default], diagnostics)
				End If
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration) Then
				Dim enumMemberDeclarationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax)
				boundNode1 = Me.BindInitializer(binder, enumMemberDeclarationSyntax.Initializer, diagnostics)
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement) Then
				Dim declarationSyntax As PropertyStatementSyntax = DirectCast(DirectCast(MyBase.MemberSymbol, SourcePropertySymbol).DeclarationSyntax, PropertyStatementSyntax)
				Dim asClause As VisualBasicSyntaxNode = declarationSyntax.AsClause
				If (asClause Is Nothing OrElse asClause.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause) Then
					asClause = declarationSyntax.Initializer
				End If
				boundNode1 = Me.BindInitializer(binder, asClause, diagnostics)
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration) Then
				If (MyBase.MemberSymbol.Kind <> SymbolKind.Field) Then
					Dim parent As VariableDeclaratorSyntax = DirectCast(DirectCast(DirectCast(MyBase.MemberSymbol, SourcePropertySymbol).Syntax, ModifiedIdentifierSyntax).Parent, VariableDeclaratorSyntax)
					Dim initializer As VisualBasicSyntaxNode = parent.AsClause
					If (initializer Is Nothing OrElse initializer.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause) Then
						initializer = parent.Initializer
					End If
					boundNode1 = Me.BindInitializer(binder, initializer, diagnostics)
				Else
					Dim memberSymbol As SourceFieldSymbol = DirectCast(MyBase.MemberSymbol, SourceFieldSymbol)
					boundNode1 = Me.BindInitializer(binder, memberSymbol.EqualsValueOrAsNewInitOpt, diagnostics)
				End If
			End If
			boundNode = If(boundNode1 Is Nothing, MyBase.Bind(binder, node, diagnostics), boundNode1)
			Return boundNode
		End Function

		Private Function BindInitializer(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal initializer As SyntaxNode, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundFieldInitializer As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
			Dim kind As SymbolKind = MyBase.MemberSymbol.Kind
			If (kind = SymbolKind.Field) Then
				If (Not TypeOf MyBase.MemberSymbol Is Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEnumConstantSymbol) Then
					Dim memberSymbol As SourceFieldSymbol = DirectCast(MyBase.MemberSymbol, SourceFieldSymbol)
					Dim instance As ArrayBuilder(Of BoundInitializer) = ArrayBuilder(Of BoundInitializer).GetInstance()
					If (initializer Is Nothing) Then
						binder.BindArrayFieldImplicitInitializer(memberSymbol, instance, diagnostics)
					Else
						Dim fieldSymbols As ImmutableArray(Of FieldSymbol) = ImmutableArray.CreateRange(Of FieldSymbol)(Me.GetInitializedFieldsOrProperties(binder).Cast(Of FieldSymbol)())
						binder.BindFieldInitializer(fieldSymbols, initializer, instance, diagnostics, True)
					End If
					boundNode = instance.First()
					instance.Free()
				ElseIf (initializer.Kind() = SyntaxKind.EqualsValue) Then
					Dim sourceEnumConstantSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEnumConstantSymbol = DirectCast(MyBase.MemberSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEnumConstantSymbol)
					constantValue = Nothing
					boundNode = binder.BindFieldAndEnumConstantInitializer(sourceEnumConstantSymbol, DirectCast(initializer, EqualsValueSyntax), True, diagnostics, constantValue)
				End If
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(boundNode, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				If (boundExpression Is Nothing) Then
					boundFieldInitializer = boundNode
					Return boundFieldInitializer
				End If
				boundFieldInitializer = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldInitializer(initializer, ImmutableArray.Create(Of FieldSymbol)(DirectCast(MyBase.MemberSymbol, FieldSymbol)), Nothing, boundExpression, False)
				Return boundFieldInitializer
			ElseIf (kind <> SymbolKind.Parameter) Then
				If (kind <> SymbolKind.[Property]) Then
					Throw ExceptionUtilities.UnexpectedValue(MyBase.MemberSymbol.Kind)
				End If
				Dim propertySymbols As ImmutableArray(Of PropertySymbol) = ImmutableArray.CreateRange(Of PropertySymbol)(Me.GetInitializedFieldsOrProperties(binder).Cast(Of PropertySymbol)())
				Dim boundInitializers As ArrayBuilder(Of BoundInitializer) = ArrayBuilder(Of BoundInitializer).GetInstance()
				binder.BindPropertyInitializer(propertySymbols, initializer, boundInitializers, diagnostics)
				boundNode = boundInitializers.First()
				boundInitializers.Free()
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(boundNode, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				If (boundExpression1 Is Nothing) Then
					boundFieldInitializer = boundNode
					Return boundFieldInitializer
				End If
				boundFieldInitializer = New BoundPropertyInitializer(initializer, propertySymbols, Nothing, boundExpression1, False)
				Return boundFieldInitializer
			ElseIf (initializer.Kind() = SyntaxKind.EqualsValue) Then
				Dim containingMember As SourceComplexParameterSymbol = DirectCast(MyBase.RootBinder.ContainingMember, SourceComplexParameterSymbol)
				constantValue = Nothing
				boundNode = binder.BindParameterDefaultValue(containingMember.Type, DirectCast(initializer, EqualsValueSyntax), diagnostics, constantValue)
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(boundNode, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				If (boundExpression2 Is Nothing) Then
					boundFieldInitializer = boundNode
					Return boundFieldInitializer
				End If
				boundFieldInitializer = New BoundParameterEqualsValue(initializer, containingMember, boundExpression2, False)
				Return boundFieldInitializer
			End If
			boundFieldInitializer = boundNode
			Return boundFieldInitializer
		End Function

		Friend Shared Function Create(ByVal containingSemanticModel As SyntaxTreeSemanticModel, ByVal binder As DeclarationInitializerBinder, Optional ByVal ignoreAccessibility As Boolean = False) As InitializerSemanticModel
			Return New InitializerSemanticModel(binder.Root, binder, containingSemanticModel, Nothing, 0, ignoreAccessibility)
		End Function

		Friend Shared Function CreateSpeculative(ByVal parentSemanticModel As SyntaxTreeSemanticModel, ByVal root As EqualsValueSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal position As Integer) As InitializerSemanticModel
			Return New InitializerSemanticModel(root, binder, Nothing, parentSemanticModel, position, False)
		End Function

		Friend Overrides Function GetBoundRoot() As BoundNode
			Dim root As SyntaxNode = MyBase.Root
			If (root.Kind() = SyntaxKind.FieldDeclaration) Then
				Dim containingMember As SourceFieldSymbol = TryCast(MyBase.RootBinder.ContainingMember, SourceFieldSymbol)
				If (containingMember Is Nothing) Then
					Dim parent As VariableDeclaratorSyntax = DirectCast(DirectCast(TryCast(MyBase.RootBinder.ContainingMember, SourcePropertySymbol).Syntax, ModifiedIdentifierSyntax).Parent, VariableDeclaratorSyntax)
					Dim asClause As VisualBasicSyntaxNode = parent.AsClause
					If (asClause Is Nothing OrElse asClause.Kind() <> SyntaxKind.AsNewClause) Then
						asClause = parent.Initializer
					End If
					If (asClause IsNot Nothing) Then
						root = asClause
					End If
				Else
					root = If(containingMember.EqualsValueOrAsNewInitOpt, containingMember.Syntax)
				End If
			ElseIf (root.Kind() = SyntaxKind.PropertyStatement) Then
				Dim propertyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax = DirectCast(root, Microsoft.CodeAnalysis.VisualBasic.Syntax.PropertyStatementSyntax)
				Dim initializer As VisualBasicSyntaxNode = propertyStatementSyntax.AsClause
				If (initializer Is Nothing OrElse initializer.Kind() <> SyntaxKind.AsNewClause) Then
					initializer = propertyStatementSyntax.Initializer
				End If
				If (initializer IsNot Nothing) Then
					root = initializer
				End If
			End If
			Return MyBase.GetUpperBoundNode(root)
		End Function

		Private Function GetInitializedFieldsOrProperties(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As IEnumerable(Of Symbol)
			Return New InitializerSemanticModel.VB$StateMachine_4_GetInitializedFieldsOrProperties(-2) With
			{
				.$VB$Me = Me,
				.$P_binder = binder
			}
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal initializer As EqualsValueSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			Dim flag As Boolean
			Dim enclosingBinder As Binder = MyBase.GetEnclosingBinder(position)
			If (enclosingBinder IsNot Nothing) Then
				enclosingBinder = SpeculativeBinder.Create(enclosingBinder)
				speculativeModel = InitializerSemanticModel.CreateSpeculative(parentModel, initializer, enclosingBinder, position)
				flag = True
			Else
				speculativeModel = Nothing
				flag = False
			End If
			Return flag
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal statement As ExecutableStatementSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			speculativeModel = Nothing
			Return False
		End Function

		Friend Overrides Function TryGetSpeculativeSemanticModelForMethodBodyCore(ByVal parentModel As SyntaxTreeSemanticModel, ByVal position As Integer, ByVal body As MethodBlockBaseSyntax, <Out> ByRef speculativeModel As SemanticModel) As Boolean
			speculativeModel = Nothing
			Return False
		End Function
	End Class
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class LocalSymbol
		Inherits Symbol
		Implements ILocalSymbol, ILocalSymbolInternal
		Friend ReadOnly Shared UseBeforeDeclarationResultType As ErrorTypeSymbol

		Private ReadOnly _container As Symbol

		Private _lazyType As TypeSymbol

		Friend Overridable ReadOnly Property CanScheduleToStack As Boolean
			Get
				If (Me.IsConst) Then
					Return False
				End If
				Return Not Me.IsCatch
			End Get
		End Property

		Public ReadOnly Property ConstantValue As Object Implements ILocalSymbol.ConstantValue
			Get
				Dim obj As Object
				If (Me.IsConst) Then
					Dim constantValue1 As Microsoft.CodeAnalysis.ConstantValue = Me.GetConstantValue(Nothing)
					obj = If(constantValue1 Is Nothing, Nothing, constantValue1.Value)
				Else
					obj = Nothing
				End If
				Return obj
			End Get
		End Property

		Friend ReadOnly Property ConstHasType As Boolean
			Get
				Return CObj(Me._lazyType) <> CObj(Nothing)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._container
			End Get
		End Property

		Friend MustOverride ReadOnly Property DeclarationKind As LocalDeclarationKind

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.NotApplicable
			End Get
		End Property

		Public ReadOnly Property HasConstantValue As Boolean Implements ILocalSymbol.HasConstantValue
			Get
				Dim flag As Boolean
				flag = If(Me.IsConst, CObj(Me.GetConstantValue(Nothing)) <> CObj(Nothing), False)
				Return flag
			End Get
		End Property

		Friend Overridable ReadOnly Property HasInferredType As Boolean
			Get
				Return False
			End Get
		End Property

		Friend MustOverride ReadOnly Property IdentifierLocation As Location

		Friend MustOverride ReadOnly Property IdentifierToken As SyntaxToken

		ReadOnly Property ILocalSymbol_IsConst As Boolean Implements ILocalSymbol.IsConst
			Get
				Return Me.IsConst
			End Get
		End Property

		ReadOnly Property ILocalSymbol_IsFixed As Boolean Implements ILocalSymbol.IsFixed
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ILocalSymbol_NullableAnnotation As Microsoft.CodeAnalysis.NullableAnnotation Implements ILocalSymbol.NullableAnnotation
			Get
				Return Microsoft.CodeAnalysis.NullableAnnotation.None
			End Get
		End Property

		ReadOnly Property ILocalSymbol_Type As ITypeSymbol Implements ILocalSymbol.Type
			Get
				Return Me.Type
			End Get
		End Property

		ReadOnly Property ILocalSymbolInternal_IsImportedFromMetadata As Boolean Implements ILocalSymbolInternal.IsImportedFromMetadata
			Get
				Return Me.IsImportedFromMetadata
			End Get
		End Property

		ReadOnly Property ILocalSymbolInternal_SynthesizedKind As SynthesizedLocalKind Implements ILocalSymbolInternal.SynthesizedKind
			Get
				Return Me.SynthesizedKind
			End Get
		End Property

		Friend Overridable ReadOnly Property IsByRef As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property IsCatch As Boolean
			Get
				Return Me.DeclarationKind = LocalDeclarationKind.[Catch]
			End Get
		End Property

		Friend ReadOnly Property IsCompilerGenerated As Boolean
			Get
				Return Me.DeclarationKind = LocalDeclarationKind.None
			End Get
		End Property

		Public ReadOnly Property IsConst As Boolean
			Get
				Return Me.DeclarationKind = LocalDeclarationKind.Constant
			End Get
		End Property

		Public ReadOnly Property IsFor As Boolean
			Get
				Return Me.DeclarationKind = LocalDeclarationKind.[For]
			End Get
		End Property

		Public ReadOnly Property IsForEach As Boolean
			Get
				Return Me.DeclarationKind = LocalDeclarationKind.ForEach
			End Get
		End Property

		Public MustOverride ReadOnly Property IsFunctionValue As Boolean Implements ILocalSymbol.IsFunctionValue

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me.DeclarationKind = LocalDeclarationKind.ImplicitVariable
			End Get
		End Property

		Friend Overridable ReadOnly Property IsImportedFromMetadata As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overridable ReadOnly Property IsPinned As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overridable ReadOnly Property IsReadOnly As Boolean
			Get
				If (Me.IsUsing) Then
					Return True
				End If
				Return Me.IsConst
			End Get
		End Property

		Public ReadOnly Property IsRef As Boolean Implements ILocalSymbol.IsRef
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property IsStatic As Boolean
			Get
				Return Me.DeclarationKind = LocalDeclarationKind.[Static]
			End Get
		End Property

		Public ReadOnly Property IsUsing As Boolean
			Get
				Return Me.DeclarationKind = LocalDeclarationKind.[Using]
			End Get
		End Property

		Protected Overrides ReadOnly Property ISymbol_IsStatic As Boolean
			Get
				Return Me.IsStatic
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.Local
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray.Create(Of Location)(Me.IdentifierLocation)
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public ReadOnly Property RefKind As Microsoft.CodeAnalysis.RefKind Implements ILocalSymbol.RefKind
			Get
				Return Microsoft.CodeAnalysis.RefKind.None
			End Get
		End Property

		Friend MustOverride ReadOnly Property SynthesizedKind As SynthesizedLocalKind

		Public Overridable ReadOnly Property Type As TypeSymbol
			Get
				If (Me._lazyType Is Nothing) Then
					Interlocked.CompareExchange(Of TypeSymbol)(Me._lazyType, Me.ComputeType(Nothing), Nothing)
				End If
				Return Me._lazyType
			End Get
		End Property

		Shared Sub New()
			LocalSymbol.UseBeforeDeclarationResultType = New ErrorTypeSymbol()
		End Sub

		Friend Sub New(ByVal container As Symbol, ByVal type As TypeSymbol)
			MyBase.New()
			Me._container = container
			Me._lazyType = type
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitLocal(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitLocal(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitLocal(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitLocal(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitLocal(Me)
		End Function

		Friend Overridable Function ComputeType(Optional ByVal containingBinder As Binder = Nothing) As TypeSymbol
			Return Me._lazyType
		End Function

		Friend Shared Function Create(ByVal container As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal modifiedIdentifierOpt As ModifiedIdentifierSyntax, ByVal asClauseOpt As AsClauseSyntax, ByVal initializerOpt As EqualsValueSyntax, ByVal declarationKind As LocalDeclarationKind) As LocalSymbol
			Return New LocalSymbol.VariableLocalSymbol(container, binder, declaringIdentifier, modifiedIdentifierOpt, asClauseOpt, initializerOpt, declarationKind)
		End Function

		Friend Shared Function Create(ByVal container As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal declarationKind As LocalDeclarationKind, ByVal type As TypeSymbol) As LocalSymbol
			Return New LocalSymbol.SourceLocalSymbol(container, binder, declaringIdentifier, declarationKind, type)
		End Function

		Friend Shared Function Create(ByVal container As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal declarationKind As LocalDeclarationKind, ByVal type As TypeSymbol, ByVal name As String) As LocalSymbol
			Return New LocalSymbol.SourceLocalSymbolWithNonstandardName(container, binder, declaringIdentifier, declarationKind, type, name)
		End Function

		Friend Shared Function Create(ByVal originalVariable As LocalSymbol, ByVal type As TypeSymbol) As LocalSymbol
			Return New LocalSymbol.TypeSubstitutedLocalSymbol(originalVariable, type)
		End Function

		Friend Shared Function CreateInferredForEach(ByVal container As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal expression As ExpressionSyntax) As LocalSymbol
			Return New LocalSymbol.InferredForEachLocalSymbol(container, binder, declaringIdentifier, expression)
		End Function

		Friend Shared Function CreateInferredForFromTo(ByVal container As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal fromValue As ExpressionSyntax, ByVal toValue As ExpressionSyntax, ByVal stepClauseOpt As ForStepClauseSyntax) As LocalSymbol
			Return New LocalSymbol.InferredForFromToLocalSymbol(container, binder, declaringIdentifier, fromValue, toValue, stepClauseOpt)
		End Function

		Friend Overridable Function GetConstantExpression(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As BoundExpression
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overridable Function GetConstantValue(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.ConstantValue
			Return Nothing
		End Function

		Friend Overridable Function GetConstantValueDiagnostics(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend MustOverride Function GetDeclaratorSyntax() As SyntaxNode

		Private Function ILocalSymbolInternal_GetDeclaratorSyntax() As SyntaxNode Implements ILocalSymbolInternal.GetDeclaratorSyntax
			Return Me.GetDeclaratorSyntax()
		End Function

		Public Sub SetType(ByVal type As TypeSymbol)
			If (Me._lazyType Is Nothing) Then
				Interlocked.CompareExchange(Of TypeSymbol)(Me._lazyType, type, Nothing)
			End If
		End Sub

		Private NotInheritable Class InferredForEachLocalSymbol
			Inherits LocalSymbol.SourceLocalSymbol
			Private ReadOnly _collectionExpressionSyntax As ExpressionSyntax

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return Symbol.GetDeclaringSyntaxReferenceHelper(Of ForEachStatementSyntax)(Me.Locations)
				End Get
			End Property

			Friend Overrides ReadOnly Property HasInferredType As Boolean
				Get
					Return True
				End Get
			End Property

			Public Sub New(ByVal container As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal collectionExpressionSyntax As ExpressionSyntax)
				MyBase.New(container, binder, declaringIdentifier, LocalDeclarationKind.ForEach, Nothing)
				Me._collectionExpressionSyntax = collectionExpressionSyntax
			End Sub

			Friend Overrides Function ComputeTypeInternal(ByVal localBinder As Binder) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
				Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
				Dim flag As Boolean = False
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundLValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundLValuePlaceholder = Nothing
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = Nothing
				Dim flag1 As Boolean = False
				Dim flag2 As Boolean = False
				Return localBinder.InferForEachVariableType(Me, Me._collectionExpressionSyntax, boundExpression, typeSymbol, typeSymbol1, flag, boundExpression1, boundLValuePlaceholder, boundExpression2, boundExpression3, boundRValuePlaceholder, flag1, flag2, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
			End Function
		End Class

		Private NotInheritable Class InferredForFromToLocalSymbol
			Inherits LocalSymbol.SourceLocalSymbol
			Private ReadOnly _fromValue As ExpressionSyntax

			Private ReadOnly _toValue As ExpressionSyntax

			Private ReadOnly _stepClauseOpt As ForStepClauseSyntax

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return Symbol.GetDeclaringSyntaxReferenceHelper(Of ForStatementSyntax)(Me.Locations)
				End Get
			End Property

			Friend Overrides ReadOnly Property HasInferredType As Boolean
				Get
					Return True
				End Get
			End Property

			Public Sub New(ByVal container As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal fromValue As ExpressionSyntax, ByVal toValue As ExpressionSyntax, ByVal stepClauseOpt As ForStepClauseSyntax)
				MyBase.New(container, binder, declaringIdentifier, LocalDeclarationKind.[For], Nothing)
				Me._fromValue = fromValue
				Me._toValue = toValue
				Me._stepClauseOpt = stepClauseOpt
			End Sub

			Friend Overrides Function ComputeType(Optional ByVal containingBinder As Binder = Nothing) As TypeSymbol
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Return (If(containingBinder, Me._binder)).InferForFromToVariableType(Me, Me._fromValue, Me._toValue, Me._stepClauseOpt, boundExpression, boundExpression1, boundExpression2, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
			End Function
		End Class

		Private Class SourceLocalSymbol
			Inherits LocalSymbol
			Private ReadOnly _declarationKind As LocalDeclarationKind

			Protected ReadOnly _identifierToken As SyntaxToken

			Protected ReadOnly _binder As Binder

			Friend Overrides ReadOnly Property DeclarationKind As LocalDeclarationKind
				Get
					Return Me._declarationKind
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Dim empty As ImmutableArray(Of SyntaxReference)
					If (Me.DeclarationKind <> LocalDeclarationKind.FunctionValue) Then
						Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me._identifierToken
						empty = ImmutableArray.Create(Of SyntaxReference)(syntaxToken.Parent.GetReference())
					Else
						empty = ImmutableArray(Of SyntaxReference).Empty
					End If
					Return empty
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property IdentifierLocation As Location
				Get
					Return Me._identifierToken.GetLocation()
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property IdentifierToken As SyntaxToken
				Get
					Return Me._identifierToken
				End Get
			End Property

			Public Overrides ReadOnly Property IsFunctionValue As Boolean
				Get
					Return Me._declarationKind = LocalDeclarationKind.FunctionValue
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Return Me._identifierToken.GetIdentifierText()
				End Get
			End Property

			Friend Overrides ReadOnly Property SynthesizedKind As SynthesizedLocalKind
				Get
					Return SynthesizedLocalKind.UserDefined
				End Get
			End Property

			Public Sub New(ByVal containingSymbol As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal identifierToken As SyntaxToken, ByVal declarationKind As LocalDeclarationKind, ByVal type As TypeSymbol)
				MyBase.New(containingSymbol, type)
				Me._identifierToken = identifierToken
				Me._declarationKind = declarationKind
				Me._binder = binder
			End Sub

			Friend Overrides Function ComputeType(Optional ByVal containingBinder As Binder = Nothing) As TypeSymbol
				containingBinder = If(containingBinder, Me._binder)
				Return Me.ComputeTypeInternal(If(containingBinder, Me._binder))
			End Function

			Friend Overridable Function ComputeTypeInternal(ByVal containingBinder As Binder) As TypeSymbol
				Return Me._lazyType
			End Function

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim flag As Boolean
				If (Me <> obj) Then
					Dim sourceLocalSymbol As LocalSymbol.SourceLocalSymbol = TryCast(obj, LocalSymbol.SourceLocalSymbol)
					flag = If(sourceLocalSymbol Is Nothing OrElse Not sourceLocalSymbol._identifierToken.Equals(Me._identifierToken) OrElse Not [Object].Equals(sourceLocalSymbol._container, Me._container), False, [String].Equals(sourceLocalSymbol.Name, Me.Name))
				Else
					flag = True
				End If
				Return flag
			End Function

			Friend Overrides Function GetDeclaratorSyntax() As SyntaxNode
				Dim blockStatement As SyntaxNode
				Dim parent As SyntaxNode
				Dim func As Func(Of AccessorBlockSyntax, Boolean)
				Dim func1 As Func(Of AccessorBlockSyntax, Boolean)
				Select Case Me.DeclarationKind
					Case LocalDeclarationKind.Variable
					Case LocalDeclarationKind.Constant
					Case LocalDeclarationKind.[Static]
					Case LocalDeclarationKind.[Using]
						parent = Me._identifierToken.Parent
						Exit Select
					Case LocalDeclarationKind.ImplicitVariable
						parent = Me._identifierToken.Parent
						Exit Select
					Case LocalDeclarationKind.[Catch]
						parent = Me._identifierToken.Parent.Parent
						Exit Select
					Case LocalDeclarationKind.[For]
						parent = Me._identifierToken.Parent
						If (parent.IsKind(SyntaxKind.ModifiedIdentifier)) Then
							Exit Select
						End If
						parent = parent.Parent
						Exit Select
					Case LocalDeclarationKind.ForEach
						parent = Me._identifierToken.Parent
						If (parent.IsKind(SyntaxKind.ModifiedIdentifier)) Then
							Exit Select
						End If
						parent = parent.Parent
						Exit Select
					Case LocalDeclarationKind.FunctionValue
						parent = Me._identifierToken.Parent
						If (Not parent.IsKind(SyntaxKind.PropertyStatement)) Then
							If (Not parent.IsKind(SyntaxKind.EventStatement)) Then
								Exit Select
							End If
							Dim accessors As IEnumerable(Of AccessorBlockSyntax) = DirectCast(DirectCast(parent.Parent, EventBlockSyntax).Accessors, IEnumerable(Of AccessorBlockSyntax))
							If (LocalSymbol.SourceLocalSymbol._Closure$__.$I12-1 Is Nothing) Then
								func = Function(a As AccessorBlockSyntax) a.IsKind(SyntaxKind.AddHandlerAccessorBlock)
								LocalSymbol.SourceLocalSymbol._Closure$__.$I12-1 = func
							Else
								func = LocalSymbol.SourceLocalSymbol._Closure$__.$I12-1
							End If
							blockStatement = accessors.Where(func).[Single]().BlockStatement
							Return blockStatement
						Else
							Dim accessorBlockSyntaxes As IEnumerable(Of AccessorBlockSyntax) = DirectCast(DirectCast(parent.Parent, PropertyBlockSyntax).Accessors, IEnumerable(Of AccessorBlockSyntax))
							If (LocalSymbol.SourceLocalSymbol._Closure$__.$I12-0 Is Nothing) Then
								func1 = Function(a As AccessorBlockSyntax) a.IsKind(SyntaxKind.GetAccessorBlock)
								LocalSymbol.SourceLocalSymbol._Closure$__.$I12-0 = func1
							Else
								func1 = LocalSymbol.SourceLocalSymbol._Closure$__.$I12-0
							End If
							blockStatement = accessorBlockSyntaxes.Where(func1).[Single]().BlockStatement
							Return blockStatement
						End If
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(Me.DeclarationKind)
				End Select
				blockStatement = parent
				Return blockStatement
			End Function

			Public Overrides Function GetHashCode() As Integer
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = Me._identifierToken
				Return Hash.Combine(syntaxToken.GetHashCode(), Me._container.GetHashCode())
			End Function
		End Class

		Private NotInheritable Class SourceLocalSymbolWithNonstandardName
			Inherits LocalSymbol.SourceLocalSymbol
			Private ReadOnly _name As String

			Public Overrides ReadOnly Property Name As String
				Get
					Return Me._name
				End Get
			End Property

			Public Sub New(ByVal container As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal declarationKind As LocalDeclarationKind, ByVal type As TypeSymbol, ByVal name As String)
				MyBase.New(container, binder, declaringIdentifier, declarationKind, type)
				Me._name = name
			End Sub
		End Class

		Private NotInheritable Class TypeSubstitutedLocalSymbol
			Inherits LocalSymbol
			Private ReadOnly _originalVariable As LocalSymbol

			Friend Overrides ReadOnly Property DeclarationKind As LocalDeclarationKind
				Get
					Return Me._originalVariable.DeclarationKind
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return Me._originalVariable.DeclaringSyntaxReferences
				End Get
			End Property

			Friend Overrides ReadOnly Property IdentifierLocation As Location
				Get
					Return Me._originalVariable.IdentifierLocation
				End Get
			End Property

			Friend Overrides ReadOnly Property IdentifierToken As SyntaxToken
				Get
					Return Me._originalVariable.IdentifierToken
				End Get
			End Property

			Friend Overrides ReadOnly Property IsByRef As Boolean
				Get
					Return Me._originalVariable.IsByRef
				End Get
			End Property

			Public Overrides ReadOnly Property IsFunctionValue As Boolean
				Get
					Return Me._originalVariable.IsFunctionValue
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return Me._originalVariable.Locations
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Return Me._originalVariable.Name
				End Get
			End Property

			Friend Overrides ReadOnly Property SynthesizedKind As SynthesizedLocalKind
				Get
					Return Me._originalVariable.SynthesizedKind
				End Get
			End Property

			Public Sub New(ByVal originalVariable As LocalSymbol, ByVal type As TypeSymbol)
				MyBase.New(originalVariable._container, type)
				Me._originalVariable = originalVariable
			End Sub

			Friend Overrides Function GetConstantValue(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.ConstantValue
				Return Me._originalVariable.GetConstantValue(binder)
			End Function

			Friend Overrides Function GetConstantValueDiagnostics(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
				Return Me._originalVariable.GetConstantValueDiagnostics(binder)
			End Function

			Friend Overrides Function GetDeclaratorSyntax() As SyntaxNode
				Return Me._originalVariable.GetDeclaratorSyntax()
			End Function
		End Class

		Private NotInheritable Class VariableLocalSymbol
			Inherits LocalSymbol.SourceLocalSymbol
			Private ReadOnly _modifiedIdentifierOpt As ModifiedIdentifierSyntax

			Private ReadOnly _asClauseOpt As AsClauseSyntax

			Private ReadOnly _initializerOpt As EqualsValueSyntax

			Private _evaluatedConstant As LocalSymbol.VariableLocalSymbol.EvaluatedConstantInfo

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Dim empty As ImmutableArray(Of SyntaxReference)
					Dim declarationKind As LocalDeclarationKind = Me.DeclarationKind
					If (declarationKind = LocalDeclarationKind.None OrElse declarationKind = LocalDeclarationKind.FunctionValue) Then
						empty = ImmutableArray(Of SyntaxReference).Empty
					ElseIf (Me._modifiedIdentifierOpt Is Nothing) Then
						empty = ImmutableArray(Of SyntaxReference).Empty
					Else
						empty = ImmutableArray.Create(Of SyntaxReference)(Me._modifiedIdentifierOpt.GetReference())
					End If
					Return empty
				End Get
			End Property

			Public Sub New(ByVal container As Symbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal modifiedIdentifierOpt As ModifiedIdentifierSyntax, ByVal asClauseOpt As AsClauseSyntax, ByVal initializerOpt As EqualsValueSyntax, ByVal declarationKind As LocalDeclarationKind)
				MyBase.New(container, binder, declaringIdentifier, declarationKind, Nothing)
				Me._modifiedIdentifierOpt = modifiedIdentifierOpt
				Me._asClauseOpt = asClauseOpt
				Me._initializerOpt = initializerOpt
			End Sub

			Friend Overrides Function ComputeTypeInternal(ByVal localBinder As Binder) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Return localBinder.ComputeVariableType(Me, Me._modifiedIdentifierOpt, Me._asClauseOpt, Me._initializerOpt, boundExpression, typeSymbol, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
			End Function

			Friend Overrides Function GetConstantExpression(ByVal localBinder As Binder) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim expression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				If (Not MyBase.IsConst) Then
					Throw ExceptionUtilities.Unreachable
				End If
				If (Me._evaluatedConstant IsNot Nothing) Then
					expression = Me._evaluatedConstant.Expression
				Else
					Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag()
					Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Nothing
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = localBinder.BindLocalConstantInitializer(Me, Me._lazyType, Me._modifiedIdentifierOpt, Me._initializerOpt, bindingDiagnosticBag, constantValue)
					Me.SetConstantExpression(boundExpression.Type, constantValue, boundExpression, bindingDiagnosticBag)
					expression = boundExpression
				End If
				Return expression
			End Function

			Friend Overrides Function GetConstantValue(ByVal containingBinder As Binder) As Microsoft.CodeAnalysis.ConstantValue
				If (MyBase.IsConst AndAlso Me._evaluatedConstant Is Nothing) Then
					Me.GetConstantExpression(If(containingBinder, Me._binder))
				End If
				If (Me._evaluatedConstant Is Nothing) Then
					Return Nothing
				End If
				Return Me._evaluatedConstant.Value
			End Function

			Friend Overrides Function GetConstantValueDiagnostics(ByVal containingBinder As Binder) As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
				Me.GetConstantValue(containingBinder)
				If (Me._evaluatedConstant Is Nothing) Then
					Return Nothing
				End If
				Return Me._evaluatedConstant.Diagnostics
			End Function

			Private Sub SetConstantExpression(ByVal type As TypeSymbol, ByVal constantValue As Microsoft.CodeAnalysis.ConstantValue, ByVal expression As BoundExpression, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				If (Me._evaluatedConstant Is Nothing) Then
					Interlocked.CompareExchange(Of LocalSymbol.VariableLocalSymbol.EvaluatedConstantInfo)(Me._evaluatedConstant, New LocalSymbol.VariableLocalSymbol.EvaluatedConstantInfo(constantValue, type, expression, diagnostics), Nothing)
				End If
			End Sub

			Private NotInheritable Class EvaluatedConstantInfo
				Inherits EvaluatedConstant
				Public ReadOnly Expression As BoundExpression

				Public ReadOnly Diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

				Public Sub New(ByVal value As Microsoft.CodeAnalysis.ConstantValue, ByVal type As TypeSymbol, ByVal expression As BoundExpression, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
					MyBase.New(value, type)
					Me.Expression = expression
					Me.Diagnostics = diagnostics
				End Sub
			End Class
		End Class
	End Class
End Namespace
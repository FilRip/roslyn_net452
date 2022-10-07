Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BackstopBinder
		Inherits Binder
		Private ReadOnly Shared s_defaultXmlNamespaces As Dictionary(Of String, String)

		Public Overrides ReadOnly Property AdditionalContainingMembers As ImmutableArray(Of Symbol)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property AllImplicitVariableDeclarationsAreHandled As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property BindingLocation As Microsoft.CodeAnalysis.VisualBasic.BindingLocation
			Get
				Return Microsoft.CodeAnalysis.VisualBasic.BindingLocation.None
			End Get
		End Property

		Public Overrides ReadOnly Property CheckOverflow As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property ConstantFieldsInProgress As Microsoft.CodeAnalysis.VisualBasic.ConstantFieldsInProgress
			Get
				Return Microsoft.CodeAnalysis.VisualBasic.ConstantFieldsInProgress.Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingMember As Symbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingNamespaceOrType As NamespaceOrTypeSymbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultParametersInProgress As SymbolsInProgress(Of ParameterSymbol)
			Get
				Return SymbolsInProgress(Of ParameterSymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property HasImportedXmlNamespaces As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property ImplicitlyDeclaredVariables As ImmutableArray(Of LocalSymbol)
			Get
				Return ImmutableArray(Of LocalSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ImplicitlyTypedLocalsBeingBound As ConsList(Of LocalSymbol)
			Get
				Return ConsList(Of LocalSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ImplicitVariableDeclarationAllowed As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsDefaultInstancePropertyAllowed As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsInQuery As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Protected Overrides ReadOnly Property IsInsideChainedConstructorCallArguments As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsSemanticModelBinder As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property OptionCompareText As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property OptionExplicit As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property OptionInfer As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property OptionStrict As Microsoft.CodeAnalysis.VisualBasic.OptionStrict
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property QuickAttributeChecker As Microsoft.CodeAnalysis.VisualBasic.Symbols.QuickAttributeChecker
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property SuppressCallerInfo As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property SuppressObsoleteDiagnostics As Boolean
			Get
				Return False
			End Get
		End Property

		Shared Sub New()
			BackstopBinder.s_defaultXmlNamespaces = New Dictionary(Of String, String)() From
			{
				{ "", "" },
				{ "xml", "http://www.w3.org/XML/1998/namespace" },
				{ "xmlns", "http://www.w3.org/2000/xmlns/" }
			}
		End Sub

		Public Sub New()
			MyBase.New(Nothing)
		End Sub

		Friend Overrides Function BinderSpecificLookupOptions(ByVal options As LookupOptions) As LookupOptions
			Return options
		End Function

		Friend Overrides Function BindInsideCrefAttributeValue(ByVal name As TypeSyntax, ByVal preserveAliases As Boolean, ByVal diagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Return ImmutableArray(Of Symbol).Empty
		End Function

		Friend Overrides Function BindInsideCrefAttributeValue(ByVal reference As CrefReferenceSyntax, ByVal preserveAliases As Boolean, ByVal diagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Return ImmutableArray(Of Symbol).Empty
		End Function

		Friend Overrides Function BindXmlNameAttributeValue(ByVal identifier As IdentifierNameSyntax, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Return ImmutableArray(Of Symbol).Empty
		End Function

		Public Overrides Function CheckAccessibility(ByVal sym As Symbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal accessThroughType As TypeSymbol = Nothing, Optional ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = Nothing) As AccessCheckResult
			Throw ExceptionUtilities.Unreachable
		End Function

		Protected Overrides Function CreateBoundWithBlock(ByVal node As WithBlockSyntax, ByVal boundBlockBinder As Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundStatement
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function DeclareImplicitLocalVariable(ByVal nameSyntax As IdentifierNameSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As LocalSymbol
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Sub DisallowFurtherImplicitVariableDeclaration(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
		End Sub

		Public Overrides Function GetBinder(ByVal node As SyntaxNode) As Binder
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function GetBinder(ByVal stmtList As SyntaxList(Of StatementSyntax)) As Binder
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function GetContinueLabel(ByVal continueSyntaxKind As SyntaxKind) As LabelSymbol
			Return Nothing
		End Function

		Public Overrides Function GetExitLabel(ByVal exitSyntaxKind As SyntaxKind) As LabelSymbol
			Return Nothing
		End Function

		Friend Overrides Sub GetInScopeXmlNamespaces(ByVal builder As ArrayBuilder(Of KeyValuePair(Of String, String)))
			Throw ExceptionUtilities.Unreachable
		End Sub

		Public Overrides Function GetLocalForFunctionValue() As LocalSymbol
			Return Nothing
		End Function

		Public Overrides Function GetReturnLabel() As LabelSymbol
			Return Nothing
		End Function

		Public Overrides Function GetSyntaxReference(ByVal node As VisualBasicSyntaxNode) As SyntaxReference
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetWithStatementPlaceholderSubstitute(ByVal placeholder As BoundValuePlaceholderBase) As BoundExpression
			Return Nothing
		End Function

		Public Overrides Function IsUnboundTypeAllowed(ByVal syntax As GenericNameSyntax) As Boolean
			Return False
		End Function

		Friend Overrides Function LookupLabelByNameToken(ByVal labelName As SyntaxToken) As LabelSymbol
			Return Nothing
		End Function

		Friend Overrides Function LookupXmlNamespace(ByVal prefix As String, ByVal ignoreXmlNodes As Boolean, <Out> ByRef [namespace] As String, <Out> ByRef fromImports As Boolean) As Boolean
			fromImports = False
			Return BackstopBinder.s_defaultXmlNamespaces.TryGetValue(prefix, [namespace])
		End Function

		Protected Overrides Function TryBindOmittedLeftForConditionalAccess(ByVal node As ConditionalAccessExpressionSyntax, ByVal accessingBinder As Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Return Nothing
		End Function

		Protected Overrides Function TryBindOmittedLeftForDictionaryAccess(ByVal node As MemberAccessExpressionSyntax, ByVal accessingBinder As Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Return Nothing
		End Function

		Protected Friend Overrides Function TryBindOmittedLeftForMemberAccess(ByVal node As MemberAccessExpressionSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal accessingBinder As Binder, <Out> ByRef wholeMemberAccessExpressionBound As Boolean) As BoundExpression
			Return Nothing
		End Function

		Protected Friend Overrides Function TryBindOmittedLeftForXmlMemberAccess(ByVal node As XmlMemberAccessExpressionSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal accessingBinder As Binder) As BoundExpression
			Return Nothing
		End Function

		Protected Overrides Function TryGetConditionalAccessReceiver(ByVal node As ConditionalAccessExpressionSyntax) As BoundExpression
			Return Nothing
		End Function
	End Class
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SourceDelegateMethodSymbol
		Inherits SourceMethodSymbol
		Private _parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _returnType As TypeSymbol

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.Constructor
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Return MethodImplAttributes.CodeTypeMask
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				Return True
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property MayBeReducibleExtensionMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property OverriddenMembers As OverriddenMembersResult(Of MethodSymbol)
			Get
				Return OverriddenMembersResult(Of MethodSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._returnType
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return ImmutableArray(Of TypeParameterSymbol).Empty
			End Get
		End Property

		Protected Sub New(ByVal delegateType As NamedTypeSymbol, ByVal syntax As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal flags As SourceMemberFlags, ByVal returnType As TypeSymbol)
			MyBase.New(delegateType, flags, binder.GetSyntaxReference(syntax), delegateType.Locations)
			Me._returnType = returnType
		End Sub

		Private Shared Function BindReturnType(ByVal syntax As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As TypeSymbol
			Dim specialType As TypeSymbol
			If (syntax.Kind() <> SyntaxKind.DelegateFunctionStatement) Then
				specialType = binder.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void, syntax, diagnostics)
			Else
				Dim delegateStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.DelegateStatementSyntax)
				Dim getErrorInfoERRStrictDisallowsImplicitProc As Func(Of DiagnosticInfo) = Nothing
				If (binder.OptionStrict = OptionStrict.[On]) Then
					getErrorInfoERRStrictDisallowsImplicitProc = ErrorFactory.GetErrorInfo_ERR_StrictDisallowsImplicitProc
				ElseIf (binder.OptionStrict = OptionStrict.Custom) Then
					getErrorInfoERRStrictDisallowsImplicitProc = ErrorFactory.GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinFunction
				End If
				Dim asClause As SimpleAsClauseSyntax = delegateStatementSyntax.AsClause
				specialType = binder.DecodeIdentifierType(delegateStatementSyntax.Identifier, asClause, getErrorInfoERRStrictDisallowsImplicitProc, diagnostics)
			End If
			Return specialType
		End Function

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return ImmutableArray(Of String).Empty
		End Function

		Protected NotOverridable Overrides Function GetAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Return New OneOrMany(Of SyntaxList(Of AttributeListSyntax))()
		End Function

		Public NotOverridable Overrides Function GetDllImportData() As DllImportData
			Return Nothing
		End Function

		Protected Sub InitializeParameters(ByVal parameters As ImmutableArray(Of ParameterSymbol))
			Me._parameters = parameters
		End Sub

		Friend Shared Sub MakeDelegateMembers(ByVal delegateType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal syntax As VisualBasicSyntaxNode, ByVal parameterListOpt As ParameterListSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <Out> ByRef constructor As MethodSymbol, <Out> ByRef beginInvoke As MethodSymbol, <Out> ByRef endInvoke As MethodSymbol, <Out> ByRef invoke As MethodSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = SourceDelegateMethodSymbol.BindReturnType(syntax, binder, diagnostics)
			Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = binder.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void, syntax, diagnostics)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = binder.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_IAsyncResult, syntax, diagnostics)
			Dim specialType1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = binder.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object, syntax, diagnostics)
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = binder.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_IntPtr, syntax, diagnostics)
			Dim specialType2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = binder.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_AsyncCallback, syntax, diagnostics)
			Dim invokeMethod As SourceDelegateMethodSymbol.InvokeMethod = New SourceDelegateMethodSymbol.InvokeMethod(delegateType, typeSymbol, syntax, binder, parameterListOpt, diagnostics)
			invoke = invokeMethod
			constructor = New SourceDelegateMethodSymbol.Constructor(delegateType, specialType, specialType1, namedTypeSymbol1, syntax, binder)
			If (delegateType.IsCompilationOutputWinMdObj()) Then
				beginInvoke = Nothing
				endInvoke = Nothing
				Return
			End If
			beginInvoke = New SourceDelegateMethodSymbol.BeginInvokeMethod(invokeMethod, namedTypeSymbol, specialType1, specialType2, syntax, binder)
			endInvoke = New SourceDelegateMethodSymbol.EndInvokeMethod(invokeMethod, namedTypeSymbol, syntax, binder)
		End Sub

		Private NotInheritable Class BeginInvokeMethod
			Inherits SourceDelegateMethodSymbol
			Public Overrides ReadOnly Property Name As String
				Get
					Return "BeginInvoke"
				End Get
			End Property

			Public Sub New(ByVal invoke As SourceDelegateMethodSymbol.InvokeMethod, ByVal iAsyncResultType As TypeSymbol, ByVal objectType As TypeSymbol, ByVal asyncCallbackType As TypeSymbol, ByVal syntax As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
				MyBase.New(invoke.ContainingType, syntax, binder, SourceMemberFlags.AccessibilityFriend Or SourceMemberFlags.AccessibilityPrivateProtected Or SourceMemberFlags.AccessibilityPublic Or SourceMemberFlags.[Overridable] Or SourceMemberFlags.InferredFieldType Or SourceMemberFlags.MethodKindOrdinary Or SourceMemberFlags.MethodKindConversion, iAsyncResultType)
				Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance()
				Dim num As Integer = 0
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = invoke.Parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					instance.Add(New SourceClonedParameterSymbol(DirectCast(current, SourceParameterSymbol), Me, num))
					num = num + 1
				End While
				instance.Add(New SynthesizedParameterSymbol(Me, asyncCallbackType, num, False, "DelegateCallback"))
				num = num + 1
				instance.Add(New SynthesizedParameterSymbol(Me, objectType, num, False, "DelegateAsyncState"))
				MyBase.InitializeParameters(instance.ToImmutableAndFree())
			End Sub

			Protected Overrides Function GetReturnTypeAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
				Return New OneOrMany(Of SyntaxList(Of AttributeListSyntax))()
			End Function
		End Class

		Private NotInheritable Class Constructor
			Inherits SourceDelegateMethodSymbol
			Public Overrides ReadOnly Property Name As String
				Get
					Return ".ctor"
				End Get
			End Property

			Public Sub New(ByVal delegateType As NamedTypeSymbol, ByVal voidType As TypeSymbol, ByVal objectType As TypeSymbol, ByVal intPtrType As TypeSymbol, ByVal syntax As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
				MyBase.New(delegateType, syntax, binder, SourceMemberFlags.AccessibilityFriend Or SourceMemberFlags.AccessibilityPrivateProtected Or SourceMemberFlags.AccessibilityPublic Or SourceMemberFlags.[Dim] Or SourceMemberFlags.[Static] Or SourceMemberFlags.MethodIsSub Or SourceMemberFlags.MethodKindConstructor, voidType)
				MyBase.InitializeParameters(ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSymbol(Me, objectType, 0, False, "TargetObject"), New SynthesizedParameterSymbol(Me, intPtrType, 1, False, "TargetMethod")))
			End Sub

			Protected Overrides Function GetReturnTypeAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
				Return New OneOrMany(Of SyntaxList(Of AttributeListSyntax))()
			End Function
		End Class

		Private NotInheritable Class EndInvokeMethod
			Inherits SourceDelegateMethodSymbol
			Public Overrides ReadOnly Property Name As String
				Get
					Return "EndInvoke"
				End Get
			End Property

			Public Sub New(ByVal invoke As SourceDelegateMethodSymbol.InvokeMethod, ByVal iAsyncResultType As TypeSymbol, ByVal syntax As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
				MyBase.New(invoke.ContainingType, syntax, binder, DirectCast((1342179334 Or If(invoke.ReturnType.SpecialType = SpecialType.System_Void, 33554432, 0)), SourceMemberFlags), invoke.ReturnType)
				Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance()
				Dim num As Integer = 0
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = invoke.Parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					If (Not current.IsByRef) Then
						Continue While
					End If
					instance.Add(New SourceClonedParameterSymbol(DirectCast(current, SourceParameterSymbol), Me, num))
					num = num + 1
				End While
				instance.Add(New SynthesizedParameterSymbol(Me, iAsyncResultType, instance.Count, False, "DelegateAsyncResult"))
				MyBase.InitializeParameters(instance.ToImmutableAndFree())
			End Sub

			Protected Overrides Function GetReturnTypeAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
				Return New OneOrMany(Of SyntaxList(Of AttributeListSyntax))()
			End Function
		End Class

		Private NotInheritable Class InvokeMethod
			Inherits SourceDelegateMethodSymbol
			Public Overrides ReadOnly Property Name As String
				Get
					Return "Invoke"
				End Get
			End Property

			Public Sub New(ByVal delegateType As NamedTypeSymbol, ByVal returnType As TypeSymbol, ByVal syntax As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal parameterListOpt As ParameterListSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				MyBase.New(delegateType, syntax, binder, DirectCast((402655238 Or If(returnType.SpecialType = SpecialType.System_Void, 33554432, 0)), SourceMemberFlags), returnType)
				MyBase.InitializeParameters(binder.DecodeParameterListOfDelegateDeclaration(Me, parameterListOpt, diagnostics))
			End Sub
		End Class
	End Class
End Namespace
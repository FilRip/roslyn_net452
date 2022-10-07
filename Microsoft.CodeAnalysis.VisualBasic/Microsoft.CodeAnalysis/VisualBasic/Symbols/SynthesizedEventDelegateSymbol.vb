Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedEventDelegateSymbol
		Inherits InstanceTypeSymbol
		Private ReadOnly _eventName As String

		Private ReadOnly _name As String

		Private ReadOnly _containingType As NamedTypeSymbol

		Private ReadOnly _syntaxRef As SyntaxReference

		Private _lazyMembers As ImmutableArray(Of Symbol)

		Private _lazyEventSymbol As EventSymbol

		Private _reportedAllDeclarationErrors As Integer

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				If (Me._lazyEventSymbol Is Nothing) Then
					Dim members As ImmutableArray(Of Symbol) = Me._containingType.GetMembers(Me._eventName)
					Dim enumerator As ImmutableArray(Of Symbol).Enumerator = members.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As SourceEventSymbol = TryCast(enumerator.Current, SourceEventSymbol)
						If (current Is Nothing) Then
							Continue While
						End If
						Dim syntax As SyntaxNode = current.SyntaxReference.GetSyntax(New CancellationToken())
						If (syntax Is Nothing OrElse syntax <> Me.EventSyntax) Then
							Continue While
						End If
						Me._lazyEventSymbol = current
					End While
				End If
				Return Me._lazyEventSymbol
			End Get
		End Property

		Friend Overrides ReadOnly Property CoClassType As TypeSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me.AssociatedSymbol.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultPropertyName As String
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				Return Me.ContainingType.EmbeddedSymbolKind
			End Get
		End Property

		Private ReadOnly Property EventSyntax As EventStatementSyntax
			Get
				Return DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), EventStatementSyntax)
			End Get
		End Property

		Friend Overrides ReadOnly Property HasCodeAnalysisEmbeddedAttribute As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property ImplicitlyDefinedBy(ByVal membersInProgress As Dictionary(Of String, ArrayBuilder(Of Symbol))) As Symbol
			Get
				Dim associatedSymbol As Symbol
				If (membersInProgress IsNot Nothing) Then
					Dim item As ArrayBuilder(Of Symbol) = membersInProgress(Me._eventName)
					Dim sourceEventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol = Nothing
					If (item IsNot Nothing) Then
						Dim enumerator As ArrayBuilder(Of Symbol).Enumerator = item.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol = TryCast(enumerator.Current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol)
							If (current Is Nothing) Then
								Continue While
							End If
							Dim syntax As SyntaxNode = current.SyntaxReference.GetSyntax(New CancellationToken())
							If (syntax Is Nothing OrElse syntax <> Me.EventSyntax) Then
								Continue While
							End If
							sourceEventSymbol = current
						End While
					End If
					associatedSymbol = sourceEventSymbol
				Else
					associatedSymbol = Me.AssociatedSymbol
				End If
				Return associatedSymbol
			End Get
		End Property

		Friend Overrides ReadOnly Property IsComImport As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExtensibleInterfaceNoUseSiteDiagnostics As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustInherit As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotInheritable As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property Layout As TypeLayout
			Get
				Return New TypeLayout()
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray.Create(Of Location)(Me._syntaxRef.GetLocation())
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
			Get
				Return MyBase.DefaultMarshallingCharSet
			End Get
		End Property

		Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
			Get
				Dim name As Func(Of Symbol, String)
				Dim members As ImmutableArray(Of Symbol) = Me.GetMembers()
				If (SynthesizedEventDelegateSymbol._Closure$__.$I48-0 Is Nothing) Then
					name = Function(member As Symbol) member.Name
					SynthesizedEventDelegateSymbol._Closure$__.$I48-0 = name
				Else
					name = SynthesizedEventDelegateSymbol._Closure$__.$I48-0
				End If
				Return New HashSet(Of String)(members.[Select](Of String)(name))
			End Get
		End Property

		Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return Me.AssociatedSymbol.ShadowsExplicitly
			End Get
		End Property

		Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Microsoft.CodeAnalysis.TypeKind.[Delegate]
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return ImmutableArray(Of TypeParameterSymbol).Empty
			End Get
		End Property

		Friend Sub New(ByVal syntaxRef As SyntaxReference, ByVal containingSymbol As NamedTypeSymbol)
			MyBase.New()
			Me._reportedAllDeclarationErrors = 0
			Me._containingType = containingSymbol
			Me._syntaxRef = syntaxRef
			Me._eventName = Me.EventSyntax.Identifier.ValueText
			Me._name = [String].Concat(Me._eventName, "EventHandler")
		End Sub

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			If (Me._reportedAllDeclarationErrors = 0) Then
				Me.GetMembers()
				cancellationToken.ThrowIfCancellationRequested()
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				Me.DelegateInvokeMethod.GenerateDeclarationErrors(cancellationToken)
				Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._containingType
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
				Do
					If (Not containingType.IsInterfaceType()) Then
						Exit Do
					End If
					If (containingType.TypeParameters.HaveVariance()) Then
						namedTypeSymbol = containingType
					End If
					containingType = containingType.ContainingType
				Loop While containingType IsNot Nothing
				If (namedTypeSymbol IsNot Nothing) Then
					Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_VariancePreventsSynthesizedEvents2, New [Object]() { CustomSymbolDisplayFormatter.QualifiedName(namedTypeSymbol), Me.AssociatedSymbol.Name })
					Dim locations As ImmutableArray(Of Location) = Me.Locations
					instance.Add(New VBDiagnostic(diagnosticInfo, locations(0), False))
				End If
				DirectCast(Me.ContainingModule, SourceModuleSymbol).AtomicStoreIntegerAndDiagnostics(Me._reportedAllDeclarationErrors, 1, 0, instance)
				instance.Free()
			End If
		End Sub

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return ImmutableArray(Of String).Empty
		End Function

		Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
			Return SpecializedCollections.EmptyEnumerable(Of FieldSymbol)()
		End Function

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return New LexicalSortKey(Me._syntaxRef, Me.DeclaringCompilation)
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Dim symbols As ImmutableArray(Of Symbol)
			Dim symbols1 As ImmutableArray(Of Symbol)
			If (Me._lazyMembers.IsDefault) Then
				Dim containingModule As SourceModuleSymbol = DirectCast(Me.ContainingModule, SourceModuleSymbol)
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(containingModule, Me._syntaxRef.SyntaxTree, Me.ContainingType)
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				Dim eventSyntax As EventStatementSyntax = Me.EventSyntax
				Dim parameterList As ParameterListSyntax = eventSyntax.ParameterList
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				Dim methodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				Dim methodSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				SourceDelegateMethodSymbol.MakeDelegateMembers(Me, Me.EventSyntax, eventSyntax.ParameterList, binder, methodSymbol, methodSymbol1, methodSymbol2, methodSymbol3, instance)
				symbols1 = If(methodSymbol1 Is Nothing OrElse methodSymbol2 Is Nothing, ImmutableArray.Create(Of Symbol)(methodSymbol, methodSymbol3), ImmutableArray.Create(Of Symbol)(methodSymbol, methodSymbol1, methodSymbol2, methodSymbol3))
				containingModule.AtomicStoreArrayAndDiagnostics(Of Symbol)(Me._lazyMembers, symbols1, instance)
				instance.Free()
				symbols = Me._lazyMembers
			Else
				symbols = Me._lazyMembers
			End If
			Return symbols
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Return Me.GetMembers().Where(Function(m As Symbol) CaseInsensitiveComparison.Equals(m.Name, name)).AsImmutable()
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
			Return SpecializedCollections.EmptyEnumerable(Of PropertySymbol)()
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Me.MakeDeclaredBase(New BasesBeingResolved(), diagnostics)
		End Function

		Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Me._containingType.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate)
		End Function

		Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function
	End Class
End Namespace
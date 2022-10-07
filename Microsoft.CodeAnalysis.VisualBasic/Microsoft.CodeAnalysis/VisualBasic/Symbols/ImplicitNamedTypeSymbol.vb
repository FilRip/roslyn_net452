Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class ImplicitNamedTypeSymbol
		Inherits SourceMemberContainerTypeSymbol
		Friend Overrides ReadOnly Property CoClassType As TypeSymbol
			Get
				Return Nothing
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

		Friend ReadOnly Property HasStructLayoutAttribute As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
			Get
				Return False
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
				If (MyBase.IsImplicitClass) Then
					Return True
				End If
				Return MyBase.IsScriptClass
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

		Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
			Get
				Return MyBase.DefaultMarshallingCharSet
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return ImmutableArray(Of TypeParameterSymbol).Empty
			End Get
		End Property

		Friend Sub New(ByVal declaration As MergedTypeDeclaration, ByVal containingSymbol As NamespaceOrTypeSymbol, ByVal containingModule As SourceModuleSymbol)
			MyBase.New(declaration, containingSymbol, containingModule)
		End Sub

		Protected Overrides Sub AddDeclaredNonTypeMembers(ByVal membersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim enumerator As ImmutableArray(Of SyntaxReference).Enumerator = MyBase.SyntaxReferences.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As SyntaxReference = enumerator.Current
				Dim visualBasicSyntax As VisualBasicSyntaxNode = current.GetVisualBasicSyntax(New CancellationToken())
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(MyBase.ContainingSourceModule, current.SyntaxTree, Me)
				Dim fieldOrPropertyInitializers As ArrayBuilder(Of FieldOrPropertyInitializer) = Nothing
				Dim fieldOrPropertyInitializers1 As ArrayBuilder(Of FieldOrPropertyInitializer) = Nothing
				Dim isImplicitClass As Boolean = MyBase.IsImplicitClass
				Dim enumerator1 As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax).Enumerator = If(visualBasicSyntax.Kind() = SyntaxKind.CompilationUnit, DirectCast(visualBasicSyntax, CompilationUnitSyntax).Members, DirectCast(visualBasicSyntax, NamespaceBlockSyntax).Members).GetEnumerator()
				While enumerator1.MoveNext()
					Dim statementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax = enumerator1.Current
					Dim flag As Boolean = If(Not isImplicitClass, False, Not statementSyntax.HasErrors)
					MyBase.AddMember(statementSyntax, binder, diagnostics, membersBuilder, fieldOrPropertyInitializers, fieldOrPropertyInitializers1, flag)
				End While
				Dim membersAndInitializersBuilder As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder = membersBuilder
				Dim staticInitializers As ArrayBuilder(Of ImmutableArray(Of FieldOrPropertyInitializer)) = membersAndInitializersBuilder.StaticInitializers
				SourceMemberContainerTypeSymbol.AddInitializers(staticInitializers, fieldOrPropertyInitializers)
				membersAndInitializersBuilder.StaticInitializers = staticInitializers
				Dim membersAndInitializersBuilder1 As SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder = membersBuilder
				staticInitializers = membersAndInitializersBuilder1.InstanceInitializers
				SourceMemberContainerTypeSymbol.AddInitializers(staticInitializers, fieldOrPropertyInitializers1)
				membersAndInitializersBuilder1.InstanceInitializers = staticInitializers
			End While
		End Sub

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return ImmutableArray(Of String).Empty
		End Function

		Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
			Return AttributeUsageInfo.Null
		End Function

		Protected Overrides Function GetInheritsOrImplementsLocation(ByVal base As NamedTypeSymbol, ByVal getInherits As Boolean) As Location
			Return NoLocation.Singleton
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Return Nothing
		End Function

		Friend Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
			Return SpecializedCollections.EmptyEnumerable(Of PropertySymbol)()
		End Function

		Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Return Me.GetDeclaredBase(New BasesBeingResolved())
		End Function

		Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Dim specialType As NamedTypeSymbol = Me.DeclaringCompilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = specialType.GetUseSiteInfo()
			Dim locations As ImmutableArray(Of Location) = MyBase.Locations
			diagnostics.Add(useSiteInfo, locations(0))
			If (Me.TypeKind <> Microsoft.CodeAnalysis.TypeKind.Submission) Then
				Return specialType
			End If
			Return Nothing
		End Function

		Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function
	End Class
End Namespace
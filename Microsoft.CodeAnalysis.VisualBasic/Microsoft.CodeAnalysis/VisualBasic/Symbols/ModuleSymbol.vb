Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Reflection.PortableExecutable
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class ModuleSymbol
		Inherits Symbol
		Implements IModuleSymbol, IModuleSymbolInternal
		Friend MustOverride ReadOnly Property Bit32Required As Boolean

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return DirectCast(Me.ContainingSymbol, AssemblySymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.NotApplicable
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend MustOverride ReadOnly Property DefaultMarshallingCharSet As Nullable(Of CharSet)

		Public MustOverride ReadOnly Property GlobalNamespace As NamespaceSymbol

		Friend MustOverride ReadOnly Property HasAssemblyCompilationRelaxationsAttribute As Boolean

		Friend MustOverride ReadOnly Property HasAssemblyRuntimeCompatibilityAttribute As Boolean

		Friend MustOverride ReadOnly Property HasUnifiedReferences As Boolean

		ReadOnly Property IModuleSymbol_GlobalNamespace As INamespaceSymbol Implements IModuleSymbol.GlobalNamespace
			Get
				Return Me.GlobalNamespace
			End Get
		End Property

		ReadOnly Property IModuleSymbol_ReferencedAssemblySymbols As ImmutableArray(Of IAssemblySymbol) Implements IModuleSymbol.ReferencedAssemblySymbols
			Get
				Return ImmutableArray(Of IAssemblySymbol).CastUp(Of AssemblySymbol)(Me.ReferencedAssemblySymbols)
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsMissing As Boolean

		Public NotOverridable Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.NetModule
			End Get
		End Property

		Friend MustOverride ReadOnly Property Machine As System.Reflection.PortableExecutable.Machine

		Friend MustOverride ReadOnly Property MightContainExtensionMethods As Boolean

		Friend MustOverride ReadOnly Property NamespaceNames As ICollection(Of String)

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend MustOverride ReadOnly Property Ordinal As Integer

		Public ReadOnly Property ReferencedAssemblies As ImmutableArray(Of AssemblyIdentity) Implements IModuleSymbol.ReferencedAssemblies
			Get
				Return Me.GetReferencedAssemblies()
			End Get
		End Property

		Public ReadOnly Property ReferencedAssemblySymbols As ImmutableArray(Of AssemblySymbol)
			Get
				Return Me.GetReferencedAssemblySymbols()
			End Get
		End Property

		Friend MustOverride ReadOnly Property TypeNames As ICollection(Of String)

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitModule(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitModule(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitModule(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitModule(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitModule(Me)
		End Function

		Friend Overridable Function GetHash(ByVal algorithmId As AssemblyHashAlgorithm) As ImmutableArray(Of Byte)
			Throw ExceptionUtilities.Unreachable
		End Function

		Public MustOverride Function GetMetadata() As ModuleMetadata Implements IModuleSymbol.GetMetadata

		Public Function GetModuleNamespace(ByVal namespaceSymbol As INamespaceSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			Dim nestedNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
			If (namespaceSymbol Is Nothing) Then
				Throw New ArgumentNullException("namespaceSymbol")
			End If
			Dim namespaceSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = TryCast(namespaceSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
			Dim extent As NamespaceExtent = namespaceSymbol1.Extent
			If (CObj(namespaceSymbol1) <> CObj(Nothing) And extent.Kind = NamespaceKind.[Module] And (namespaceSymbol1.ContainingModule = Me)) Then
				nestedNamespace = namespaceSymbol1
			ElseIf (Not (namespaceSymbol.IsGlobalNamespace Or namespaceSymbol.ContainingNamespace Is Nothing)) Then
				Dim moduleNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Me.GetModuleNamespace(namespaceSymbol.ContainingNamespace)
				If (moduleNamespace Is Nothing) Then
					nestedNamespace = Nothing
				Else
					nestedNamespace = moduleNamespace.GetNestedNamespace(namespaceSymbol.Name)
				End If
			Else
				nestedNamespace = Me.GlobalNamespace
			End If
			Return nestedNamespace
		End Function

		Friend MustOverride Function GetReferencedAssemblies() As ImmutableArray(Of AssemblyIdentity)

		Friend Function GetReferencedAssemblySymbol(ByVal referencedAssemblyIndex As Integer) As AssemblySymbol
			Dim item As AssemblySymbol
			Dim referencedAssemblySymbols As ImmutableArray(Of AssemblySymbol) = Me.GetReferencedAssemblySymbols()
			If (referencedAssemblyIndex >= referencedAssemblySymbols.Length) Then
				Dim containingAssembly As AssemblySymbol = Me.ContainingAssembly
				If (CObj(containingAssembly) <> CObj(containingAssembly.CorLibrary)) Then
					Throw New ArgumentOutOfRangeException("referencedAssemblyIndex")
				End If
				item = Nothing
			Else
				item = referencedAssemblySymbols(referencedAssemblyIndex)
			End If
			Return item
		End Function

		Friend MustOverride Function GetReferencedAssemblySymbols() As ImmutableArray(Of AssemblySymbol)

		Friend MustOverride Function GetUnificationUseSiteErrorInfo(ByVal dependentType As TypeSymbol) As DiagnosticInfo

		Private Function IModuleSymbol_GetModuleNamespace(ByVal namespaceSymbol As INamespaceSymbol) As INamespaceSymbol Implements IModuleSymbol.GetModuleNamespace
			Return Me.GetModuleNamespace(namespaceSymbol)
		End Function

		Friend MustOverride Function LookupTopLevelMetadataType(ByRef emittedName As MetadataTypeName) As NamedTypeSymbol

		Friend MustOverride Sub SetReferences(ByVal moduleReferences As ModuleReferences(Of AssemblySymbol), Optional ByVal originatingSourceAssemblyDebugOnly As SourceAssemblySymbol = Nothing)
	End Class
End Namespace
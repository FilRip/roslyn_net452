Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection.Metadata

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class PEGlobalNamespaceSymbol
		Inherits PENamespaceSymbol
		Private ReadOnly _moduleSymbol As PEModuleSymbol

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._moduleSymbol.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me._moduleSymbol
			End Get
		End Property

		Friend Overrides ReadOnly Property ContainingPEModule As PEModuleSymbol
			Get
				Return Me._moduleSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._moduleSymbol
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property IsGlobalNamespace As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return [String].Empty
			End Get
		End Property

		Friend Sub New(ByVal moduleSymbol As PEModuleSymbol)
			MyBase.New()
			Me._moduleSymbol = moduleSymbol
		End Sub

		Protected Overrides Sub EnsureAllMembersLoaded()
			Dim groupings As IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle))
			If (Me.m_lazyTypes Is Nothing OrElse Me.m_lazyMembers Is Nothing) Then
				Try
					groupings = Me._moduleSymbol.[Module].GroupTypesByNamespaceOrThrow(CaseInsensitiveComparison.Comparer)
				Catch badImageFormatException As System.BadImageFormatException
					ProjectData.SetProjectError(badImageFormatException)
					groupings = SpecializedCollections.EmptyEnumerable(Of IGrouping(Of String, TypeDefinitionHandle))()
					ProjectData.ClearProjectError()
				End Try
				MyBase.LoadAllMembers(groupings)
			End If
		End Sub
	End Class
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class PENestedNamespaceSymbol
		Inherits PENamespaceSymbol
		Friend ReadOnly m_ContainingNamespaceSymbol As PENamespaceSymbol

		Protected ReadOnly m_Name As String

		Private _typesByNS As IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle))

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me.ContainingPEModule.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me.m_ContainingNamespaceSymbol.ContainingPEModule
			End Get
		End Property

		Friend Overrides ReadOnly Property ContainingPEModule As PEModuleSymbol
			Get
				Return Me.m_ContainingNamespaceSymbol.ContainingPEModule
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.m_ContainingNamespaceSymbol
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property IsGlobalNamespace As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me.m_Name
			End Get
		End Property

		Friend Sub New(ByVal name As String, ByVal containingNamespace As PENamespaceSymbol, ByVal typesByNS As IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle)))
			MyBase.New()
			Me.m_ContainingNamespaceSymbol = containingNamespace
			Me.m_Name = name
			Me._typesByNS = typesByNS
		End Sub

		Protected Overrides Sub EnsureAllMembersLoaded()
			Dim groupings As IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle)) = Me._typesByNS
			If (Me.m_lazyTypes Is Nothing OrElse Me.m_lazyMembers Is Nothing) Then
				MyBase.LoadAllMembers(groupings)
				Interlocked.Exchange(Of IEnumerable(Of IGrouping(Of String, TypeDefinitionHandle)))(Me._typesByNS, Nothing)
			End If
		End Sub

		Protected Overrides Function GetDeclaredAccessibilityOfMostAccessibleDescendantType() As Microsoft.CodeAnalysis.Accessibility
			Dim declaredAccessibilityOfMostAccessibleDescendantType As Microsoft.CodeAnalysis.Accessibility
			Dim enumerator As IEnumerator(Of IGrouping(Of String, System.Reflection.Metadata.TypeDefinitionHandle)) = Nothing
			Dim enumerator1 As IEnumerator(Of System.Reflection.Metadata.TypeDefinitionHandle) = Nothing
			Dim typeDefFlagsOrThrow As TypeAttributes = 0
			Dim groupings As IEnumerable(Of IGrouping(Of String, System.Reflection.Metadata.TypeDefinitionHandle)) = Me._typesByNS
			If (groupings Is Nothing OrElse Me.m_lazyTypes IsNot Nothing) Then
				declaredAccessibilityOfMostAccessibleDescendantType = MyBase.GetDeclaredAccessibilityOfMostAccessibleDescendantType()
			Else
				Dim [module] As PEModule = Me.ContainingPEModule.[Module]
				Using accessibility As Microsoft.CodeAnalysis.Accessibility = Microsoft.CodeAnalysis.Accessibility.NotApplicable
					enumerator = groupings.GetEnumerator()
					While enumerator.MoveNext()
						Using current As IGrouping(Of String, System.Reflection.Metadata.TypeDefinitionHandle) = enumerator.Current
							enumerator1 = current.GetEnumerator()
							While enumerator1.MoveNext()
								Dim typeDefinitionHandle As System.Reflection.Metadata.TypeDefinitionHandle = enumerator1.Current
								Try
									typeDefFlagsOrThrow = [module].GetTypeDefFlagsOrThrow(typeDefinitionHandle)
								Catch badImageFormatException As System.BadImageFormatException
									ProjectData.SetProjectError(badImageFormatException)
									ProjectData.ClearProjectError()
								End Try
								Dim typeAttribute As TypeAttributes = typeDefFlagsOrThrow And TypeAttributes.VisibilityMask
								If (typeAttribute = TypeAttributes.NotPublic) Then
									accessibility = Microsoft.CodeAnalysis.Accessibility.Internal
								Else
									If (typeAttribute <> TypeAttributes.[Public]) Then
										Continue While
									End If
									declaredAccessibilityOfMostAccessibleDescendantType = Microsoft.CodeAnalysis.Accessibility.[Public]
									Return declaredAccessibilityOfMostAccessibleDescendantType
								End If
							End While
						End Using
					End While
				End Using
				declaredAccessibilityOfMostAccessibleDescendantType = accessibility
			End If
			Return declaredAccessibilityOfMostAccessibleDescendantType
		End Function
	End Class
End Namespace
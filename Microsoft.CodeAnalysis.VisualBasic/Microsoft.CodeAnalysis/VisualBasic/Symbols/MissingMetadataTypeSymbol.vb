Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	<DebuggerDisplay("{GetDebuggerDisplay(), nq}")>
	Friend MustInherit Class MissingMetadataTypeSymbol
		Inherits InstanceErrorTypeSymbol
		Protected ReadOnly m_Name As String

		Protected ReadOnly m_MangleName As Boolean

		Friend Overrides ReadOnly Property ErrorInfo As DiagnosticInfo
			Get
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
				Dim obj As Object
				Dim containingAssembly As AssemblySymbol = Me.ContainingAssembly
				If (Not containingAssembly.IsMissing) Then
					Dim containingModule As ModuleSymbol = Me.ContainingModule
					diagnosticInfo = If(Not containingModule.IsMissing, ErrorFactory.ErrorInfo(ERRID.ERR_TypeRefResolutionError3, New [Object]() { Me, containingModule.Name }), ErrorFactory.ErrorInfo(ERRID.ERR_UnreferencedModule3, New [Object]() { containingModule.Name, Me }))
				Else
					If (Me.SpecialType <> Microsoft.CodeAnalysis.SpecialType.None) Then
						obj = CustomSymbolDisplayFormatter.DefaultErrorFormat(Me)
					Else
						obj = Me
					End If
					Dim objectValue As Object = RuntimeHelpers.GetObjectValue(obj)
					diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.ERR_UnreferencedAssembly3, New [Object]() { containingAssembly.Identity, objectValue })
				End If
				Return diagnosticInfo
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return Me.m_MangleName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me.m_Name
			End Get
		End Property

		Private Sub New(ByVal name As String, ByVal arity As Integer, ByVal mangleName As Boolean)
			MyBase.New(arity)
			Me.m_Name = name
			Me.m_MangleName = If(Not mangleName, False, arity > 0)
		End Sub

		Friend NotInheritable Class Nested
			Inherits MissingMetadataTypeSymbol
			Private ReadOnly _containingType As NamedTypeSymbol

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._containingType
				End Get
			End Property

			Public Overrides ReadOnly Property SpecialType As Microsoft.CodeAnalysis.SpecialType
				Get
					Return Microsoft.CodeAnalysis.SpecialType.None
				End Get
			End Property

			Public Sub New(ByVal containingType As NamedTypeSymbol, ByVal name As String, ByVal arity As Integer, ByVal mangleName As Boolean)
				MyBase.New(name, arity, mangleName)
				Me._containingType = containingType
			End Sub

			Public Sub New(ByVal containingType As NamedTypeSymbol, ByRef emittedName As MetadataTypeName)
				MyClass.New(containingType, emittedName, If(emittedName.ForcedArity = -1, True, emittedName.ForcedArity = emittedName.InferredArity))
			End Sub

			Private Sub New(ByVal containingType As NamedTypeSymbol, ByRef emittedName As MetadataTypeName, ByVal mangleName As Boolean)
				MyClass.New(containingType, If(mangleName, emittedName.UnmangledTypeName, emittedName.TypeName), If(mangleName, emittedName.InferredArity, emittedName.ForcedArity), mangleName)
			End Sub

			Private Function GetDebuggerDisplay() As String
				Dim str As String = [String].Concat(Me._containingType.ToString(), ".", Me.Name)
				If (Me._arity > 0) Then
					str = [String].Concat(str, "(Of ", New [String](","C, Me._arity - 1), ")")
				End If
				Return [String].Concat(str, "[missing]")
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Hash.Combine(Of NamedTypeSymbol)(Me._containingType, Hash.Combine(Of String)(Me.MetadataName, MyBase.Arity))
			End Function

			Protected Overrides Function SpecializedEquals(ByVal obj As InstanceErrorTypeSymbol) As Boolean
				Dim nested As MissingMetadataTypeSymbol.Nested = TryCast(obj, MissingMetadataTypeSymbol.Nested)
				If (nested Is Nothing OrElse Not [String].Equals(Me.MetadataName, nested.MetadataName, StringComparison.Ordinal) OrElse MyBase.Arity <> nested.Arity) Then
					Return False
				End If
				Return Me._containingType.Equals(nested._containingType)
			End Function
		End Class

		<DebuggerDisplay("{GetDebuggerDisplay(), nq}")>
		Friend Class TopLevel
			Inherits MissingMetadataTypeSymbol
			Private ReadOnly _namespaceName As String

			Private ReadOnly _containingModule As ModuleSymbol

			Private _lazyContainingNamespace As NamespaceSymbol

			Private _lazyTypeId As Integer

			Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
				Get
					Return Me._containingModule.ContainingAssembly
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
				Get
					Return Me._containingModule
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					If (Me._lazyContainingNamespace Is Nothing) Then
						Dim globalNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Me._containingModule.GlobalNamespace
						If (Me._namespaceName.Length > 0) Then
							Dim strs As ImmutableArray(Of String) = MetadataHelpers.SplitQualifiedName(Me._namespaceName)
							Dim length As Integer = strs.Length - 1
							Dim num As Integer = 0
							While True
								If (num <= length) Then
									Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Nothing
									Dim item As String = strs(num)
									Dim enumerator As ImmutableArray(Of Symbol).Enumerator = globalNamespace.GetMembers(item).GetEnumerator()
									While enumerator.MoveNext()
										Dim current As NamespaceOrTypeSymbol = DirectCast(enumerator.Current, NamespaceOrTypeSymbol)
										If (current.Kind <> SymbolKind.[Namespace] OrElse Not [String].Equals(current.Name, item, StringComparison.Ordinal)) Then
											Continue While
										End If
										namespaceSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
										Exit While
									End While
									If (namespaceSymbol Is Nothing) Then
										Exit While
									End If
									globalNamespace = namespaceSymbol
									num = num + 1
								Else
									Exit While
								End If
							End While
							While num < strs.Length
								globalNamespace = New MissingNamespaceSymbol(globalNamespace, strs(num))
								num = num + 1
							End While
						End If
						Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)(Me._lazyContainingNamespace, globalNamespace, Nothing)
					End If
					Return Me._lazyContainingNamespace
				End Get
			End Property

			Public ReadOnly Property NamespaceName As String
				Get
					Return Me._namespaceName
				End Get
			End Property

			Public Overrides ReadOnly Property SpecialType As Microsoft.CodeAnalysis.SpecialType
				Get
					If (Me._lazyTypeId = -1) Then
						Dim typeFromMetadataName As Microsoft.CodeAnalysis.SpecialType = Microsoft.CodeAnalysis.SpecialType.None
						Dim containingAssembly As AssemblySymbol = Me._containingModule.ContainingAssembly
						If ((MyBase.Arity = 0 OrElse Me.MangleName) AndAlso containingAssembly IsNot Nothing AndAlso CObj(containingAssembly) = CObj(containingAssembly.CorLibrary) AndAlso Me._containingModule.Ordinal = 0) Then
							typeFromMetadataName = SpecialTypes.GetTypeFromMetadataName(MetadataHelpers.BuildQualifiedName(Me._namespaceName, Me.MetadataName))
						End If
						Interlocked.CompareExchange(Me._lazyTypeId, CInt(typeFromMetadataName), -1)
					End If
					Return DirectCast(CSByte(Me._lazyTypeId), Microsoft.CodeAnalysis.SpecialType)
				End Get
			End Property

			Public Sub New(ByVal [module] As ModuleSymbol, ByVal [namespace] As String, ByVal name As String, ByVal arity As Integer, ByVal mangleName As Boolean)
				MyBase.New(name, arity, mangleName)
				Me._lazyTypeId = -1
				Me._namespaceName = [namespace]
				Me._containingModule = [module]
			End Sub

			Public Sub New(ByVal [module] As ModuleSymbol, ByRef fullname As MetadataTypeName, Optional ByVal typeId As Microsoft.CodeAnalysis.SpecialType = -1)
				MyClass.New([module], fullname, If(fullname.ForcedArity = -1, True, fullname.ForcedArity = fullname.InferredArity))
				Me._lazyTypeId = CInt(typeId)
			End Sub

			Private Sub New(ByVal [module] As ModuleSymbol, ByRef fullname As MetadataTypeName, ByVal mangleName As Boolean)
				MyClass.New([module], fullname.NamespaceName, If(mangleName, fullname.UnmangledTypeName, fullname.TypeName), If(mangleName, fullname.InferredArity, fullname.ForcedArity), mangleName)
			End Sub

			Private Function GetDebuggerDisplay() As String
				Dim str As String = MetadataHelpers.BuildQualifiedName(Me._namespaceName, Me.m_Name)
				If (Me._arity > 0) Then
					str = [String].Concat(str, "(Of ", New [String](","C, Me._arity - 1), ")")
				End If
				Return [String].Concat(str, "[missing]")
			End Function

			Friend Overrides Function GetEmittedNamespaceName() As String
				Return Me._namespaceName
			End Function

			Public NotOverridable Overrides Function GetHashCode() As Integer
				Return Hash.Combine(Of ModuleSymbol)(Me._containingModule, Hash.Combine(Of String)(Me.MetadataName, Hash.Combine(Of String)(Me._namespaceName, MyBase.Arity)))
			End Function

			Protected NotOverridable Overrides Function SpecializedEquals(ByVal obj As InstanceErrorTypeSymbol) As Boolean
				Dim topLevel As MissingMetadataTypeSymbol.TopLevel = TryCast(obj, MissingMetadataTypeSymbol.TopLevel)
				If (topLevel Is Nothing OrElse Not [String].Equals(Me.MetadataName, topLevel.MetadataName, StringComparison.Ordinal) OrElse MyBase.Arity <> topLevel.Arity OrElse Not [String].Equals(Me._namespaceName, topLevel._namespaceName, StringComparison.Ordinal)) Then
					Return False
				End If
				Return Me._containingModule.Equals(topLevel._containingModule)
			End Function
		End Class

		Friend Class TopLevelWithCustomErrorInfo
			Inherits MissingMetadataTypeSymbol.TopLevel
			Private ReadOnly _errorInfo As DiagnosticInfo

			Friend Overrides ReadOnly Property ErrorInfo As DiagnosticInfo
				Get
					Return Me._errorInfo
				End Get
			End Property

			Public Sub New(ByVal moduleSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ModuleSymbol, ByRef emittedName As MetadataTypeName, ByVal errorInfo As DiagnosticInfo, Optional ByVal typeId As Microsoft.CodeAnalysis.SpecialType = -1)
				MyBase.New(moduleSymbol, emittedName, typeId)
				Me._errorInfo = errorInfo
			End Sub

			Public Sub New(ByVal moduleSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ModuleSymbol, ByRef emittedName As MetadataTypeName, ByVal delayedErrorInfo As Func(Of MissingMetadataTypeSymbol.TopLevelWithCustomErrorInfo, DiagnosticInfo))
				MyBase.New(moduleSymbol, emittedName, Microsoft.CodeAnalysis.SpecialType.System_Object Or Microsoft.CodeAnalysis.SpecialType.System_Enum Or Microsoft.CodeAnalysis.SpecialType.System_MulticastDelegate Or Microsoft.CodeAnalysis.SpecialType.System_Delegate Or Microsoft.CodeAnalysis.SpecialType.System_ValueType Or Microsoft.CodeAnalysis.SpecialType.System_Void Or Microsoft.CodeAnalysis.SpecialType.System_Boolean Or Microsoft.CodeAnalysis.SpecialType.System_Char Or Microsoft.CodeAnalysis.SpecialType.System_SByte Or Microsoft.CodeAnalysis.SpecialType.System_Byte Or Microsoft.CodeAnalysis.SpecialType.System_Int16 Or Microsoft.CodeAnalysis.SpecialType.System_UInt16 Or Microsoft.CodeAnalysis.SpecialType.System_Int32 Or Microsoft.CodeAnalysis.SpecialType.System_UInt32 Or Microsoft.CodeAnalysis.SpecialType.System_Int64 Or Microsoft.CodeAnalysis.SpecialType.System_UInt64 Or Microsoft.CodeAnalysis.SpecialType.System_Decimal Or Microsoft.CodeAnalysis.SpecialType.System_Single Or Microsoft.CodeAnalysis.SpecialType.System_Double Or Microsoft.CodeAnalysis.SpecialType.System_String Or Microsoft.CodeAnalysis.SpecialType.System_IntPtr Or Microsoft.CodeAnalysis.SpecialType.System_UIntPtr Or Microsoft.CodeAnalysis.SpecialType.System_Array Or Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerable Or Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerable_T Or Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IList_T Or Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_ICollection_T Or Microsoft.CodeAnalysis.SpecialType.System_Collections_IEnumerator Or Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerator_T Or Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IReadOnlyList_T Or Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or Microsoft.CodeAnalysis.SpecialType.System_Nullable_T Or Microsoft.CodeAnalysis.SpecialType.System_DateTime Or Microsoft.CodeAnalysis.SpecialType.System_Runtime_CompilerServices_IsVolatile Or Microsoft.CodeAnalysis.SpecialType.System_IDisposable Or Microsoft.CodeAnalysis.SpecialType.System_TypedReference Or Microsoft.CodeAnalysis.SpecialType.System_ArgIterator Or Microsoft.CodeAnalysis.SpecialType.System_RuntimeArgumentHandle Or Microsoft.CodeAnalysis.SpecialType.System_RuntimeFieldHandle Or Microsoft.CodeAnalysis.SpecialType.System_RuntimeMethodHandle Or Microsoft.CodeAnalysis.SpecialType.System_RuntimeTypeHandle Or Microsoft.CodeAnalysis.SpecialType.System_IAsyncResult Or Microsoft.CodeAnalysis.SpecialType.System_AsyncCallback Or Microsoft.CodeAnalysis.SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or Microsoft.CodeAnalysis.SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or Microsoft.CodeAnalysis.SpecialType.Count)
				Me._errorInfo = delayedErrorInfo(Me)
			End Sub
		End Class
	End Class
End Namespace
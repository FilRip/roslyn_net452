Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.RuntimeMembers
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class AnonymousTypeManager
		Inherits CommonAnonymousTypeManager
		Public ReadOnly Compilation As VisualBasicCompilation

		Private _concurrentTypesCache As ConcurrentDictionary(Of String, AnonymousTypeManager.AnonymousTypeTemplateSymbol)

		Private _concurrentDelegatesCache As ConcurrentDictionary(Of String, AnonymousTypeManager.AnonymousDelegateTemplateSymbol)

		Friend ReadOnly Property AllCreatedTemplates As ImmutableArray(Of NamedTypeSymbol)
			Get
				Dim instance As ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol) = ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol).GetInstance()
				Me.GetAllCreatedTemplates(instance)
				Return StaticCast(Of NamedTypeSymbol).From(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol)(instance.ToImmutableAndFree())
			End Get
		End Property

		Private ReadOnly Property AnonymousDelegateTemplates As ConcurrentDictionary(Of String, AnonymousTypeManager.AnonymousDelegateTemplateSymbol)
			Get
				Dim anonymousTypeManager As ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousDelegateTemplateSymbol)
				Dim strs As ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousDelegateTemplateSymbol)
				If (Me._concurrentDelegatesCache Is Nothing) Then
					Dim previousSubmission As VisualBasicCompilation = Me.Compilation.PreviousSubmission
					If (previousSubmission Is Nothing) Then
						anonymousTypeManager = Nothing
					Else
						anonymousTypeManager = previousSubmission.AnonymousTypeManager._concurrentDelegatesCache
					End If
					Dim strs1 As ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousDelegateTemplateSymbol) = anonymousTypeManager
					strs = If(strs1 Is Nothing, New ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousDelegateTemplateSymbol)(), New ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousDelegateTemplateSymbol)(strs1))
					Interlocked.CompareExchange(Of ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousDelegateTemplateSymbol))(Me._concurrentDelegatesCache, strs, Nothing)
				End If
				Return Me._concurrentDelegatesCache
			End Get
		End Property

		Private ReadOnly Property AnonymousTypeTemplates As ConcurrentDictionary(Of String, AnonymousTypeManager.AnonymousTypeTemplateSymbol)
			Get
				Dim anonymousTypeManager As ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousTypeTemplateSymbol)
				Dim strs As ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousTypeTemplateSymbol)
				If (Me._concurrentTypesCache Is Nothing) Then
					Dim previousSubmission As VisualBasicCompilation = Me.Compilation.PreviousSubmission
					If (previousSubmission Is Nothing) Then
						anonymousTypeManager = Nothing
					Else
						anonymousTypeManager = previousSubmission.AnonymousTypeManager._concurrentTypesCache
					End If
					Dim strs1 As ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousTypeTemplateSymbol) = anonymousTypeManager
					strs = If(strs1 Is Nothing, New ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousTypeTemplateSymbol)(), New ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousTypeTemplateSymbol)(strs1))
					Interlocked.CompareExchange(Of ConcurrentDictionary(Of String, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeManager.AnonymousTypeTemplateSymbol))(Me._concurrentTypesCache, strs, Nothing)
				End If
				Return Me._concurrentTypesCache
			End Get
		End Property

		Public ReadOnly Property ContainingModule As SourceModuleSymbol
			Get
				Return DirectCast(Me.Compilation.SourceModule, SourceModuleSymbol)
			End Get
		End Property

		Public ReadOnly Property System_AsyncCallback As NamedTypeSymbol
			Get
				Return Me.Compilation.GetSpecialType(SpecialType.System_AsyncCallback)
			End Get
		End Property

		Public ReadOnly Property System_Boolean As NamedTypeSymbol
			Get
				Return Me.Compilation.GetSpecialType(SpecialType.System_Boolean)
			End Get
		End Property

		Public ReadOnly Property System_Diagnostics_DebuggerDisplayAttribute__ctor As MethodSymbol
			Get
				Return DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__ctor), MethodSymbol)
			End Get
		End Property

		Public ReadOnly Property System_Diagnostics_DebuggerDisplayAttribute__Type As PropertySymbol
			Get
				Return DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__Type), PropertySymbol)
			End Get
		End Property

		Public ReadOnly Property System_IAsyncResult As NamedTypeSymbol
			Get
				Return Me.Compilation.GetSpecialType(SpecialType.System_IAsyncResult)
			End Get
		End Property

		Public ReadOnly Property System_IEquatable_T As NamedTypeSymbol
			Get
				Return Me.Compilation.GetWellKnownType(WellKnownType.System_IEquatable_T)
			End Get
		End Property

		Public ReadOnly Property System_IEquatable_T_Equals As MethodSymbol
			Get
				Return DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_IEquatable_T__Equals), MethodSymbol)
			End Get
		End Property

		Public ReadOnly Property System_Int32 As NamedTypeSymbol
			Get
				Return Me.Compilation.GetSpecialType(SpecialType.System_Int32)
			End Get
		End Property

		Public ReadOnly Property System_IntPtr As NamedTypeSymbol
			Get
				Return Me.Compilation.GetSpecialType(SpecialType.System_IntPtr)
			End Get
		End Property

		Public ReadOnly Property System_MulticastDelegate As NamedTypeSymbol
			Get
				Return Me.Compilation.GetSpecialType(SpecialType.System_MulticastDelegate)
			End Get
		End Property

		Public ReadOnly Property System_Object As NamedTypeSymbol
			Get
				Return Me.Compilation.GetSpecialType(SpecialType.System_Object)
			End Get
		End Property

		Public ReadOnly Property System_Object__Equals As MethodSymbol
			Get
				Return DirectCast(Me.ContainingModule.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Object__Equals), MethodSymbol)
			End Get
		End Property

		Public ReadOnly Property System_Object__GetHashCode As MethodSymbol
			Get
				Return DirectCast(Me.ContainingModule.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Object__GetHashCode), MethodSymbol)
			End Get
		End Property

		Public ReadOnly Property System_Object__ToString As MethodSymbol
			Get
				Return DirectCast(Me.ContainingModule.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Object__ToString), MethodSymbol)
			End Get
		End Property

		Public ReadOnly Property System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor As MethodSymbol
			Get
				Return DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor), MethodSymbol)
			End Get
		End Property

		Public ReadOnly Property System_String As NamedTypeSymbol
			Get
				Return Me.Compilation.GetSpecialType(SpecialType.System_String)
			End Get
		End Property

		Public ReadOnly Property System_String__Format_IFormatProvider As MethodSymbol
			Get
				Return DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_String__Format_IFormatProvider), MethodSymbol)
			End Get
		End Property

		Public ReadOnly Property System_Void As NamedTypeSymbol
			Get
				Return Me.Compilation.GetSpecialType(SpecialType.System_Void)
			End Get
		End Property

		Public Sub New(ByVal compilation As VisualBasicCompilation)
			MyBase.New()
			Me._concurrentTypesCache = Nothing
			Me._concurrentDelegatesCache = Nothing
			Me.Compilation = compilation
		End Sub

		Private Sub AddFromCache(Of T As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol)(ByVal builder As ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol), ByVal cache As ConcurrentDictionary(Of String, T))
			Dim enumerator As IEnumerator(Of T) = Nothing
			If (cache IsNot Nothing) Then
				Try
					enumerator = cache.Values.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As T = enumerator.Current
						If (current.Manager <> Me) Then
							Continue While
						End If
						builder.Add(DirectCast(current, AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol))
					End While
				Finally
					If (enumerator IsNot Nothing) Then
						enumerator.Dispose()
					End If
				End Try
			End If
		End Sub

		Public Sub AssignTemplatesNamesAndCompile(ByVal compiler As MethodCompiler, ByVal moduleBeingBuilt As PEModuleBuilder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim variable As AnonymousTypeManager._Closure$__22-0 = Nothing
			Dim empty As String
			Dim name As Func(Of AnonymousTypeKeyField, String)
			Dim isKey As Func(Of AnonymousTypeKeyField, Boolean)
			Dim enumerator As ImmutableArray(Of AnonymousTypeKey).Enumerator = moduleBeingBuilt.GetPreviousAnonymousTypes().GetEnumerator()
			While enumerator.MoveNext()
				variable = New AnonymousTypeManager._Closure$__22-0(variable) With
				{
					.$VB$Me = Me,
					.$VB$Local_key = enumerator.Current
				}
				Dim fields As ImmutableArray(Of AnonymousTypeKeyField) = variable.$VB$Local_key.Fields
				If (AnonymousTypeManager._Closure$__.$I22-0 Is Nothing) Then
					name = Function(f As AnonymousTypeKeyField) f.Name
					AnonymousTypeManager._Closure$__.$I22-0 = name
				Else
					name = AnonymousTypeManager._Closure$__.$I22-0
				End If
				If (AnonymousTypeManager._Closure$__.$I22-1 Is Nothing) Then
					isKey = Function(f As AnonymousTypeKeyField) f.IsKey
					AnonymousTypeManager._Closure$__.$I22-1 = isKey
				Else
					isKey = AnonymousTypeManager._Closure$__.$I22-1
				End If
				Dim str As String = AnonymousTypeDescriptor.ComputeKey(Of AnonymousTypeKeyField)(fields, name, isKey)
				If (Not variable.$VB$Local_key.IsDelegate) Then
					Me.AnonymousTypeTemplates.GetOrAdd(str, Function(k As String) New AnonymousTypeManager.AnonymousTypeTemplateSymbol(Me.$VB$Me, AnonymousTypeManager.CreatePlaceholderTypeDescriptor(Me.$VB$Local_key)))
				Else
					Me.AnonymousDelegateTemplates.GetOrAdd(str, Function(k As String) AnonymousTypeManager.AnonymousDelegateTemplateSymbol.Create(Me.$VB$Me, AnonymousTypeManager.CreatePlaceholderTypeDescriptor(Me.$VB$Local_key)))
				End If
			End While
			Dim instance As ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol) = ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol).GetInstance()
			Me.GetAllCreatedTemplates(instance)
			If (Not MyBase.AreTemplatesSealed) Then
				If (moduleBeingBuilt.OutputKind <> OutputKind.NetModule) Then
					empty = [String].Empty
				Else
					empty = moduleBeingBuilt.Name
					Dim defaultExtension As String = EnumBounds.GetDefaultExtension(OutputKind.NetModule)
					If (empty.EndsWith(defaultExtension, StringComparison.OrdinalIgnoreCase)) Then
						empty = empty.Substring(0, empty.Length - defaultExtension.Length)
					End If
					empty = [String].Concat("<", MetadataHelpers.MangleForTypeNameIfNeeded(empty), ">")
				End If
				Dim nextAnonymousTypeIndex As Integer = moduleBeingBuilt.GetNextAnonymousTypeIndex(False)
				Dim num As Integer = moduleBeingBuilt.GetNextAnonymousTypeIndex(True)
				Dim enumerator1 As ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol).Enumerator = instance.GetEnumerator()
				While enumerator1.MoveNext()
					Dim current As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol = enumerator1.Current
					Dim str1 As String = Nothing
					Dim num1 As Integer = 0
					If (Not moduleBeingBuilt.TryGetAnonymousTypeName(current, str1, num1)) Then
						Dim typeKind As Microsoft.CodeAnalysis.TypeKind = current.TypeKind
						If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Class]) Then
							num1 = nextAnonymousTypeIndex
							nextAnonymousTypeIndex = nextAnonymousTypeIndex + 1
						Else
							If (typeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
								Throw ExceptionUtilities.UnexpectedValue(current.TypeKind)
							End If
							num1 = num
							num = num + 1
						End If
						Dim submissionSlotIndex As Integer = Me.Compilation.GetSubmissionSlotIndex()
						str1 = GeneratedNames.MakeAnonymousTypeTemplateName(current.GeneratedNamePrefix, num1, submissionSlotIndex, empty)
					End If
					current.NameAndIndex = New AnonymousTypeManager.NameAndIndex(str1, num1)
				End While
				MyBase.SealTemplates()
			End If
			If (instance.Count > 0 AndAlso Not Me.CheckAndReportMissingSymbols(instance, diagnostics)) Then
				Dim enumerator2 As ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol).Enumerator = instance.GetEnumerator()
				While enumerator2.MoveNext()
					enumerator2.Current.Accept(compiler)
				End While
			End If
			instance.Free()
		End Sub

		Private Function CheckAndReportMissingSymbols(ByVal anonymousTypes As ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			Dim enumerator As ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol).Enumerator = anonymousTypes.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol = enumerator.Current
				Dim typeKind As Microsoft.CodeAnalysis.TypeKind = current.TypeKind
				If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Class]) Then
					flag = True
					If (Not DirectCast(current, AnonymousTypeManager.AnonymousTypeTemplateSymbol).HasAtLeastOneKeyField) Then
						Continue While
					End If
					flag2 = True
					If (Not flag1) Then
						Continue While
					End If
					Exit While
				Else
					If (typeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
						Throw ExceptionUtilities.UnexpectedValue(current.TypeKind)
					End If
					flag1 = True
					If (Not flag2) Then
						Continue While
					End If
					Exit While
				End If
			End While
			If (Not flag AndAlso Not flag1) Then
				Return True
			End If
			Return Me.ReportMissingOrErroneousSymbols(diagnostics, flag, flag1, flag2)
		End Function

		<Conditional("DEBUG")>
		Private Sub CheckSourceLocationSeen(ByVal anonymous As AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol)
		End Sub

		Private Function ConstructAnonymousDelegateImplementationSymbol(ByVal anonymous As AnonymousTypeManager.AnonymousDelegatePublicSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim typeDescriptor As AnonymousTypeDescriptor = anonymous.TypeDescriptor
			Dim parameters As ImmutableArray(Of AnonymousTypeField) = typeDescriptor.Parameters
			Dim orAdd As AnonymousTypeManager.AnonymousDelegateTemplateSymbol = Nothing
			Dim key As String = typeDescriptor.Key
			If (Not Me.AnonymousDelegateTemplates.TryGetValue(key, orAdd)) Then
				orAdd = Me.AnonymousDelegateTemplates.GetOrAdd(key, AnonymousTypeManager.AnonymousDelegateTemplateSymbol.Create(Me, typeDescriptor))
			End If
			If (orAdd.Manager = Me) Then
				orAdd.AdjustMetadataNames(typeDescriptor)
			End If
			If (orAdd.Arity <> 0) Then
				Dim type(orAdd.Arity - 1 + 1 - 1) As TypeSymbol
				Dim arity As Integer = orAdd.Arity - 1
				Dim num As Integer = 0
				Do
					type(num) = parameters(num).Type
					num = num + 1
				Loop While num <= arity
				namedTypeSymbol = orAdd.Construct(type)
			Else
				namedTypeSymbol = orAdd
			End If
			Return namedTypeSymbol
		End Function

		Public Function ConstructAnonymousDelegateSymbol(ByVal delegateDescriptor As AnonymousTypeDescriptor) As AnonymousTypeManager.AnonymousDelegatePublicSymbol
			Return New AnonymousTypeManager.AnonymousDelegatePublicSymbol(Me, delegateDescriptor)
		End Function

		Private Function ConstructAnonymousTypeImplementationSymbol(ByVal anonymous As AnonymousTypeManager.AnonymousTypePublicSymbol) As NamedTypeSymbol
			Dim type As Func(Of AnonymousTypeField, TypeSymbol)
			Dim typeDescriptor As AnonymousTypeDescriptor = anonymous.TypeDescriptor
			Dim orAdd As AnonymousTypeManager.AnonymousTypeTemplateSymbol = Nothing
			Dim key As String = typeDescriptor.Key
			If (Not Me.AnonymousTypeTemplates.TryGetValue(key, orAdd)) Then
				orAdd = Me.AnonymousTypeTemplates.GetOrAdd(key, New AnonymousTypeManager.AnonymousTypeTemplateSymbol(Me, typeDescriptor))
			End If
			If (orAdd.Manager = Me) Then
				orAdd.AdjustMetadataNames(typeDescriptor)
			End If
			Dim fields As ImmutableArray(Of AnonymousTypeField) = typeDescriptor.Fields
			If (AnonymousTypeManager._Closure$__.$I18-0 Is Nothing) Then
				type = Function(f As AnonymousTypeField) f.Type
				AnonymousTypeManager._Closure$__.$I18-0 = type
			Else
				type = AnonymousTypeManager._Closure$__.$I18-0
			End If
			Return orAdd.Construct(Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of AnonymousTypeField, TypeSymbol)(fields, type))
		End Function

		Public Function ConstructAnonymousTypeSymbol(ByVal typeDescr As AnonymousTypeDescriptor) As AnonymousTypeManager.AnonymousTypePublicSymbol
			Return New AnonymousTypeManager.AnonymousTypePublicSymbol(Me, typeDescr)
		End Function

		Private Shared Function CreatePlaceholderTypeDescriptor(ByVal key As AnonymousTypeKey) As AnonymousTypeDescriptor
			Dim anonymousTypeField As Func(Of AnonymousTypeKeyField, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeField)
			Dim fields As ImmutableArray(Of AnonymousTypeKeyField) = key.Fields
			If (AnonymousTypeManager._Closure$__.$I21-0 Is Nothing) Then
				anonymousTypeField = Function(f As AnonymousTypeKeyField) New Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeField(f.Name, Location.None, f.IsKey)
				AnonymousTypeManager._Closure$__.$I21-0 = anonymousTypeField
			Else
				anonymousTypeField = AnonymousTypeManager._Closure$__.$I21-0
			End If
			Dim anonymousTypeFields As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeField) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of AnonymousTypeKeyField, Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeField)(fields, anonymousTypeField)
			Return New AnonymousTypeDescriptor(anonymousTypeFields, Location.None, True)
		End Function

		Private Sub GetAllCreatedTemplates(ByVal builder As ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol))
			Me.AddFromCache(Of AnonymousTypeManager.AnonymousTypeTemplateSymbol)(builder, Me._concurrentTypesCache)
			Me.AddFromCache(Of AnonymousTypeManager.AnonymousDelegateTemplateSymbol)(builder, Me._concurrentDelegatesCache)
			If (builder.Any()) Then
				builder.Sort(New AnonymousTypeManager.AnonymousTypeComparer(Me.Compilation))
			End If
		End Sub

		Friend Function GetAnonymousTypeMap() As IReadOnlyDictionary(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKey, Microsoft.CodeAnalysis.Emit.AnonymousTypeValue)
			Dim anonymousTypeKeys As Dictionary(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKey, Microsoft.CodeAnalysis.Emit.AnonymousTypeValue) = New Dictionary(Of Microsoft.CodeAnalysis.Emit.AnonymousTypeKey, Microsoft.CodeAnalysis.Emit.AnonymousTypeValue)()
			Dim instance As ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol) = ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol).GetInstance()
			Me.GetAllCreatedTemplates(instance)
			Dim enumerator As ArrayBuilder(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol).Enumerator = instance.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol = enumerator.Current
				Dim nameAndIndex As AnonymousTypeManager.NameAndIndex = current.NameAndIndex
				Dim anonymousTypeKey As Microsoft.CodeAnalysis.Emit.AnonymousTypeKey = current.GetAnonymousTypeKey()
				Dim anonymousTypeValue As Microsoft.CodeAnalysis.Emit.AnonymousTypeValue = New Microsoft.CodeAnalysis.Emit.AnonymousTypeValue(nameAndIndex.Name, nameAndIndex.Index, current.GetCciAdapter())
				anonymousTypeKeys.Add(anonymousTypeKey, anonymousTypeValue)
			End While
			instance.Free()
			Return anonymousTypeKeys
		End Function

		Private Shared Sub ReportErrorOnSpecialMember(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal member As SpecialMember, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByRef hasError As Boolean, ByVal embedVBCore As Boolean)
			If (symbol IsNot Nothing) Then
				AnonymousTypeManager.ReportErrorOnSymbol(symbol, diagnostics, hasError)
				Return
			End If
			Dim descriptor As MemberDescriptor = SpecialMembers.GetDescriptor(member)
			Dim diagnosticForMissingRuntimeHelper As DiagnosticInfo = MissingRuntimeMemberDiagnosticHelper.GetDiagnosticForMissingRuntimeHelper(descriptor.DeclaringTypeMetadataName, descriptor.Name, embedVBCore)
			diagnostics.Add(diagnosticForMissingRuntimeHelper, NoLocation.Singleton)
			hasError = True
		End Sub

		Private Shared Sub ReportErrorOnSymbol(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByRef hasError As Boolean)
			If (symbol IsNot Nothing AndAlso diagnostics.Add(symbol.GetUseSiteInfo(), NoLocation.Singleton)) Then
				hasError = True
			End If
		End Sub

		Private Shared Sub ReportErrorOnWellKnownMember(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal member As WellKnownMember, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByRef hasError As Boolean, ByVal embedVBCore As Boolean)
			If (symbol IsNot Nothing) Then
				AnonymousTypeManager.ReportErrorOnSymbol(symbol, diagnostics, hasError)
				AnonymousTypeManager.ReportErrorOnSymbol(symbol.ContainingType, diagnostics, hasError)
				Return
			End If
			Dim descriptor As MemberDescriptor = WellKnownMembers.GetDescriptor(member)
			Dim diagnosticForMissingRuntimeHelper As DiagnosticInfo = MissingRuntimeMemberDiagnosticHelper.GetDiagnosticForMissingRuntimeHelper(descriptor.DeclaringTypeMetadataName, descriptor.Name, embedVBCore)
			diagnostics.Add(diagnosticForMissingRuntimeHelper, NoLocation.Singleton)
			hasError = True
		End Sub

		Public Function ReportMissingOrErroneousSymbols(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal hasClass As Boolean, ByVal hasDelegate As Boolean, ByVal hasKeys As Boolean) As Boolean
			Dim flag As Boolean = False
			AnonymousTypeManager.ReportErrorOnSymbol(Me.System_Object, diagnostics, flag)
			AnonymousTypeManager.ReportErrorOnSymbol(Me.System_Void, diagnostics, flag)
			Dim embedVbCoreRuntime As Boolean = Me.Compilation.Options.EmbedVbCoreRuntime
			If (hasDelegate) Then
				AnonymousTypeManager.ReportErrorOnSymbol(Me.System_IntPtr, diagnostics, flag)
				AnonymousTypeManager.ReportErrorOnSymbol(Me.System_IAsyncResult, diagnostics, flag)
				AnonymousTypeManager.ReportErrorOnSymbol(Me.System_AsyncCallback, diagnostics, flag)
				AnonymousTypeManager.ReportErrorOnSymbol(Me.System_MulticastDelegate, diagnostics, flag)
			End If
			If (hasClass) Then
				AnonymousTypeManager.ReportErrorOnSymbol(Me.System_Int32, diagnostics, flag)
				AnonymousTypeManager.ReportErrorOnSymbol(Me.System_String, diagnostics, flag)
				AnonymousTypeManager.ReportErrorOnSpecialMember(Me.System_Object__ToString, SpecialMember.System_Object__ToString, diagnostics, flag, embedVbCoreRuntime)
				AnonymousTypeManager.ReportErrorOnWellKnownMember(Me.System_String__Format_IFormatProvider, WellKnownMember.System_String__Format_IFormatProvider, diagnostics, flag, embedVbCoreRuntime)
				If (hasKeys) Then
					AnonymousTypeManager.ReportErrorOnSymbol(Me.System_Boolean, diagnostics, flag)
					AnonymousTypeManager.ReportErrorOnSpecialMember(Me.System_Object__GetHashCode, SpecialMember.System_Object__GetHashCode, diagnostics, flag, embedVbCoreRuntime)
					AnonymousTypeManager.ReportErrorOnSpecialMember(Me.System_Object__Equals, SpecialMember.System_Object__Equals, diagnostics, flag, embedVbCoreRuntime)
					AnonymousTypeManager.ReportErrorOnSymbol(Me.System_IEquatable_T, diagnostics, flag)
					AnonymousTypeManager.ReportErrorOnSymbol(Me.System_IEquatable_T_Equals, diagnostics, flag)
				End If
			End If
			Return flag
		End Function

		Friend Shared Function TranslateAnonymousTypeMethodSymbol(ByVal method As MethodSymbol) As MethodSymbol
			Return DirectCast(method.ContainingType, AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol).MapMethodToImplementationSymbol(method)
		End Function

		Friend Shared Function TranslateAnonymousTypeSymbol(ByVal type As NamedTypeSymbol) As NamedTypeSymbol
			Return DirectCast(type, AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol).MapToImplementationSymbol()
		End Function

		Friend NotInheritable Class AnonymousDelegatePublicSymbol
			Inherits AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol
			Private ReadOnly _members As ImmutableArray(Of SynthesizedDelegateMethodSymbol)

			Public Overrides ReadOnly Property DelegateInvokeMethod As MethodSymbol
				Get
					Return Me._members(Me._members.Length - 1)
				End Get
			End Property

			Friend Overrides ReadOnly Property IsInterface As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
				Get
					Return Microsoft.CodeAnalysis.TypeKind.[Delegate]
				End Get
			End Property

			Public Sub New(ByVal manager As AnonymousTypeManager, ByVal typeDescr As AnonymousTypeDescriptor)
				MyBase.New(manager, typeDescr)
				Dim synthesizedDelegateMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol
				Dim synthesizedDelegateMethodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol
				Dim systemVoid As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
				Dim parameters As ImmutableArray(Of AnonymousTypeField) = typeDescr.Parameters
				If (parameters.IsSubDescription()) Then
					systemVoid = manager.System_Void
				Else
					systemVoid = parameters.Last().Type
				End If
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = systemVoid
				Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance(parameters.Length + 1)
				Dim synthesizedDelegateMethodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol("Invoke", Me, SourceMemberFlags.[Overridable] Or SourceMemberFlags.[Static] Or SourceMemberFlags.MethodKindConstructor Or SourceMemberFlags.MethodKindDelegateInvoke Or SourceMemberFlags.MethodKindConversion, typeSymbol)
				Dim length As Integer = parameters.Length - 2
				Dim num As Integer = 0
				Do
					instance.Add(AnonymousTypeManager.AnonymousDelegatePublicSymbol.ParameterFromField(synthesizedDelegateMethodSymbol2, parameters(num), num))
					num = num + 1
				Loop While num <= length
				synthesizedDelegateMethodSymbol2.SetParameters(instance.ToImmutable())
				instance.Clear()
				Dim synthesizedDelegateMethodSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol(".ctor", Me, SourceMemberFlags.[Static], manager.System_Void)
				synthesizedDelegateMethodSymbol3.SetParameters(ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSymbol(synthesizedDelegateMethodSymbol3, manager.System_Object, 0, False, "TargetObject"), New SynthesizedParameterSymbol(synthesizedDelegateMethodSymbol3, manager.System_IntPtr, 1, False, "TargetMethod")))
				If (Me.IsCompilationOutputWinMdObj()) Then
					synthesizedDelegateMethodSymbol = Nothing
					synthesizedDelegateMethodSymbol1 = Nothing
					instance.Free()
					Me._members = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol)(synthesizedDelegateMethodSymbol3, synthesizedDelegateMethodSymbol2)
					Return
				End If
				synthesizedDelegateMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol("BeginInvoke", Me, SourceMemberFlags.[Overridable] Or SourceMemberFlags.InferredFieldType Or SourceMemberFlags.MethodKindOrdinary Or SourceMemberFlags.MethodKindConversion, manager.System_IAsyncResult)
				Dim length1 As Integer = parameters.Length - 2
				num = 0
				Do
					instance.Add(AnonymousTypeManager.AnonymousDelegatePublicSymbol.ParameterFromField(synthesizedDelegateMethodSymbol, parameters(num), num))
					num = num + 1
				Loop While num <= length1
				instance.Add(New SynthesizedParameterSymbol(synthesizedDelegateMethodSymbol, manager.System_AsyncCallback, num, False, "DelegateCallback"))
				num = num + 1
				instance.Add(New SynthesizedParameterSymbol(synthesizedDelegateMethodSymbol, manager.System_Object, num, False, "DelegateAsyncState"))
				synthesizedDelegateMethodSymbol.SetParameters(instance.ToImmutable())
				instance.Clear()
				synthesizedDelegateMethodSymbol1 = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol("EndInvoke", Me, SourceMemberFlags.[Overridable] Or SourceMemberFlags.InferredFieldType Or SourceMemberFlags.MethodKindOrdinary Or SourceMemberFlags.MethodKindConversion, typeSymbol)
				Dim num1 As Integer = 0
				Dim length2 As Integer = parameters.Length - 2
				num = 0
				Do
					If (parameters(num).IsByRef) Then
						instance.Add(AnonymousTypeManager.AnonymousDelegatePublicSymbol.ParameterFromField(synthesizedDelegateMethodSymbol1, parameters(num), num1))
						num1 = num1 + 1
					End If
					num = num + 1
				Loop While num <= length2
				instance.Add(New SynthesizedParameterSymbol(synthesizedDelegateMethodSymbol1, manager.System_IAsyncResult, num1, False, "DelegateAsyncResult"))
				synthesizedDelegateMethodSymbol1.SetParameters(instance.ToImmutableAndFree())
				Me._members = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol)(synthesizedDelegateMethodSymbol3, synthesizedDelegateMethodSymbol, synthesizedDelegateMethodSymbol1, synthesizedDelegateMethodSymbol2)
			End Sub

			Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
				Return StaticCast(Of Symbol).From(Of SynthesizedDelegateMethodSymbol)(Me._members)
			End Function

			Friend Overrides Function InternalSubstituteTypeParameters(ByVal substitution As TypeSubstitution) As TypeWithModifiers
				Dim typeWithModifier As TypeWithModifiers
				Dim anonymousTypeDescriptor As Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeDescriptor = New Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeDescriptor()
				typeWithModifier = If(Me.TypeDescriptor.SubstituteTypeParametersIfNeeded(substitution, anonymousTypeDescriptor), New TypeWithModifiers(Me.Manager.ConstructAnonymousDelegateSymbol(anonymousTypeDescriptor)), New TypeWithModifiers(Me))
				Return typeWithModifier
			End Function

			Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
				Return Me.Manager.System_MulticastDelegate
			End Function

			Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
				Return ImmutableArray(Of NamedTypeSymbol).Empty
			End Function

			Public Overrides Function MapToImplementationSymbol() As NamedTypeSymbol
				Return Me.Manager.ConstructAnonymousDelegateImplementationSymbol(Me)
			End Function

			Private Shared Function ParameterFromField(ByVal container As SynthesizedDelegateMethodSymbol, ByVal field As AnonymousTypeField, ByVal ordinal As Integer) As ParameterSymbol
				Return New SynthesizedParameterWithLocationSymbol(container, field.Type, ordinal, field.IsByRef, field.Name, field.Location)
			End Function
		End Class

		Private Class AnonymousDelegateTemplateSymbol
			Inherits AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol
			Private Const s_ctorIndex As Integer = 0

			Private Const s_beginInvokeIndex As Integer = 1

			Private Const s_endInvokeIndex As Integer = 2

			Private Const s_invokeIndex As Integer = 3

			Protected ReadOnly TypeDescr As AnonymousTypeDescriptor

			Private ReadOnly _members As ImmutableArray(Of SynthesizedDelegateMethodSymbol)

			Public Overrides ReadOnly Property DelegateInvokeMethod As MethodSymbol
				Get
					Return Me._members(Me._members.Length - 1)
				End Get
			End Property

			Friend Overrides ReadOnly Property GeneratedNamePrefix As String
				Get
					Return "VB$AnonymousDelegate_"
				End Get
			End Property

			Friend Overrides ReadOnly Property IsInterface As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
				Get
					Return Microsoft.CodeAnalysis.TypeKind.[Delegate]
				End Get
			End Property

			Public Sub New(ByVal manager As AnonymousTypeManager, ByVal typeDescr As AnonymousTypeDescriptor)
				MyBase.New(manager, typeDescr)
				Dim synthesizedDelegateMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol
				Dim synthesizedDelegateMethodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol
				Dim systemVoid As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
				Me.TypeDescr = typeDescr
				Dim parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeField) = typeDescr.Parameters
				If (parameters.IsSubDescription()) Then
					systemVoid = manager.System_Void
				Else
					systemVoid = Me.TypeParameters.Last()
				End If
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = systemVoid
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).GetInstance(parameters.Length + 1)
				Dim synthesizedDelegateMethodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol("Invoke", Me, SourceMemberFlags.[Overridable] Or SourceMemberFlags.[Static] Or SourceMemberFlags.MethodKindConstructor Or SourceMemberFlags.MethodKindDelegateInvoke Or SourceMemberFlags.MethodKindConversion, typeSymbol)
				Dim length As Integer = parameters.Length - 2
				Dim num As Integer = 0
				Do
					Dim item As TypeParameterSymbol = Me.TypeParameters(num)
					Dim anonymousTypeField As Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeField = parameters(num)
					instance.Add(New AnonymousTypeManager.AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol2, item, num, anonymousTypeField.IsByRef, parameters(num).Name, num))
					num = num + 1
				Loop While num <= length
				synthesizedDelegateMethodSymbol2.SetParameters(instance.ToImmutable())
				instance.Clear()
				Dim synthesizedDelegateMethodSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol(".ctor", Me, SourceMemberFlags.[Static], manager.System_Void)
				synthesizedDelegateMethodSymbol3.SetParameters(ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)(New AnonymousTypeManager.AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol3, manager.System_Object, 0, False, "TargetObject", -1), New AnonymousTypeManager.AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol3, manager.System_IntPtr, 1, False, "TargetMethod", -1)))
				If (Not Me.IsCompilationOutputWinMdObj()) Then
					synthesizedDelegateMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol("BeginInvoke", Me, SourceMemberFlags.[Overridable] Or SourceMemberFlags.InferredFieldType Or SourceMemberFlags.MethodKindOrdinary Or SourceMemberFlags.MethodKindConversion, manager.System_IAsyncResult)
					Dim parameterCount As Integer = synthesizedDelegateMethodSymbol2.ParameterCount - 1
					num = 0
					Do
						Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = synthesizedDelegateMethodSymbol2.Parameters(num)
						instance.Add(New AnonymousTypeManager.AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol, parameterSymbol.Type, num, parameterSymbol.IsByRef, parameterSymbol.Name, num))
						num = num + 1
					Loop While num <= parameterCount
					instance.Add(New AnonymousTypeManager.AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol, manager.System_AsyncCallback, num, False, "DelegateCallback", -1))
					num = num + 1
					instance.Add(New AnonymousTypeManager.AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol, manager.System_Object, num, False, "DelegateAsyncState", -1))
					synthesizedDelegateMethodSymbol.SetParameters(instance.ToImmutable())
					instance.Clear()
					synthesizedDelegateMethodSymbol1 = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol("EndInvoke", Me, SourceMemberFlags.[Overridable] Or SourceMemberFlags.InferredFieldType Or SourceMemberFlags.MethodKindOrdinary Or SourceMemberFlags.MethodKindConversion, typeSymbol)
					Dim num1 As Integer = 0
					Dim parameterCount1 As Integer = synthesizedDelegateMethodSymbol2.ParameterCount - 1
					num = 0
					Do
						Dim item1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = synthesizedDelegateMethodSymbol2.Parameters(num)
						If (item1.IsByRef) Then
							instance.Add(New AnonymousTypeManager.AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol1, item1.Type, num1, item1.IsByRef, item1.Name, num))
							num1 = num1 + 1
						End If
						num = num + 1
					Loop While num <= parameterCount1
					instance.Add(New AnonymousTypeManager.AnonymousTypeOrDelegateParameterSymbol(synthesizedDelegateMethodSymbol1, manager.System_IAsyncResult, num1, False, "DelegateAsyncResult", -1))
					synthesizedDelegateMethodSymbol1.SetParameters(instance.ToImmutable())
					Me._members = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol)(synthesizedDelegateMethodSymbol3, synthesizedDelegateMethodSymbol, synthesizedDelegateMethodSymbol1, synthesizedDelegateMethodSymbol2)
				Else
					synthesizedDelegateMethodSymbol = Nothing
					synthesizedDelegateMethodSymbol1 = Nothing
					Me._members = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedDelegateMethodSymbol)(synthesizedDelegateMethodSymbol3, synthesizedDelegateMethodSymbol2)
				End If
				instance.Free()
			End Sub

			Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
				MyBase.AddSynthesizedAttributes(compilationState, attributes)
				Dim compilation As VisualBasicCompilation = Me.Manager.Compilation
				Dim typedConstants As ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant) = New ImmutableArray(Of Microsoft.CodeAnalysis.TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
				Dim typedConstant As Microsoft.CodeAnalysis.TypedConstant = New Microsoft.CodeAnalysis.TypedConstant(Me.Manager.System_String, TypedConstantKind.Primitive, "<generated method>")
				Symbol.AddSynthesizedAttribute(attributes, Me.Manager.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__ctor, ImmutableArray.Create(Of Microsoft.CodeAnalysis.TypedConstant)(typedConstant), ImmutableArray.Create(Of KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant))(New KeyValuePair(Of WellKnownMember, Microsoft.CodeAnalysis.TypedConstant)(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__Type, typedConstant)), False))
			End Sub

			Friend Shared Function Create(ByVal manager As AnonymousTypeManager, ByVal typeDescr As AnonymousTypeDescriptor) As AnonymousTypeManager.AnonymousDelegateTemplateSymbol
				Dim parameters As ImmutableArray(Of AnonymousTypeField) = typeDescr.Parameters
				If (parameters.Length = 1 AndAlso parameters.IsSubDescription()) Then
					Return New AnonymousTypeManager.NonGenericAnonymousDelegateSymbol(manager, typeDescr)
				End If
				Return New AnonymousTypeManager.AnonymousDelegateTemplateSymbol(manager, typeDescr)
			End Function

			Friend Overrides Function GetAnonymousTypeKey() As AnonymousTypeKey
				Dim anonymousTypeKeyField As Func(Of AnonymousTypeField, Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField)
				Dim parameters As ImmutableArray(Of AnonymousTypeField) = Me.TypeDescr.Parameters
				If (AnonymousTypeManager.AnonymousDelegateTemplateSymbol._Closure$__.$I8-0 Is Nothing) Then
					anonymousTypeKeyField = Function(p As AnonymousTypeField) New Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField(p.Name, p.IsByRef, True)
					AnonymousTypeManager.AnonymousDelegateTemplateSymbol._Closure$__.$I8-0 = anonymousTypeKeyField
				Else
					anonymousTypeKeyField = AnonymousTypeManager.AnonymousDelegateTemplateSymbol._Closure$__.$I8-0
				End If
				Return New AnonymousTypeKey(Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of AnonymousTypeField, Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField)(parameters, anonymousTypeKeyField), True)
			End Function

			Friend NotOverridable Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
				Return SpecializedCollections.EmptyEnumerable(Of FieldSymbol)()
			End Function

			Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
				Return StaticCast(Of Symbol).From(Of SynthesizedDelegateMethodSymbol)(Me._members)
			End Function

			Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
				Return Me.Manager.System_MulticastDelegate
			End Function

			Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
				Return ImmutableArray(Of NamedTypeSymbol).Empty
			End Function
		End Class

		Private NotInheritable Class AnonymousType_IEquatable_EqualsMethodSymbol
			Inherits SynthesizedRegularMethodBase
			Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

			Private ReadOnly _interfaceMethod As ImmutableArray(Of MethodSymbol)

			Private ReadOnly Property AnonymousType As AnonymousTypeManager.AnonymousTypeTemplateSymbol
				Get
					Return DirectCast(Me.m_containingType, AnonymousTypeManager.AnonymousTypeTemplateSymbol)
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
				Get
					Return Accessibility.[Public]
				End Get
			End Property

			Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
				Get
					Return Me._interfaceMethod
				End Get
			End Property

			Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsNotOverridable As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsOverloads As Boolean
				Get
					Return True
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

			Public Overrides ReadOnly Property IsSub As Boolean
				Get
					Return False
				End Get
			End Property

			Friend Overrides ReadOnly Property ParameterCount As Integer
				Get
					Return 1
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me.AnonymousType.Manager.System_Boolean
				End Get
			End Property

			Public Sub New(ByVal container As AnonymousTypeManager.AnonymousTypeTemplateSymbol, ByVal interfaceMethod As MethodSymbol)
				MyBase.New(VisualBasicSyntaxTree.Dummy.GetRoot(New CancellationToken()), container, "Equals", False)
				Me._parameters = ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSimpleSymbol(Me, container, 0, "val"))
				Me._interfaceMethod = ImmutableArray.Create(Of MethodSymbol)(interfaceMethod)
			End Sub

			Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
				MyBase.AddSynthesizedAttributes(compilationState, attributes)
				Dim compilation As VisualBasicCompilation = DirectCast(MyBase.ContainingType, AnonymousTypeManager.AnonymousTypeTemplateSymbol).Manager.Compilation
				Symbol.AddSynthesizedAttribute(attributes, compilation.SynthesizeDebuggerHiddenAttribute())
			End Sub

			Private Function BuildAndAlso(ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal booleanType As TypeSymbol) As BoundExpression
				Return (New BoundBinaryOperator(MyBase.Syntax, BinaryOperatorKind.[AndAlso], left, right, False, booleanType, False)).MakeCompilerGenerated()
			End Function

			Private Function BuildBoxedFieldAccess(ByVal receiver As BoundExpression, ByVal field As FieldSymbol) As BoundExpression
				Return (New BoundDirectCast(MyBase.Syntax, (New BoundFieldAccess(MyBase.Syntax, receiver, field, False, field.Type, False)).MakeCompilerGenerated(), ConversionKind.WideningTypeParameter, Me.AnonymousType.Manager.System_Object, False)).MakeCompilerGenerated()
			End Function

			Private Function BuildConditionForField(ByVal [property] As AnonymousTypeManager.AnonymousTypePropertySymbol, ByVal boundMe As BoundMeReference, ByVal boundOther As BoundParameter, ByVal boundNothing As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal localMyFieldBoxed As LocalSymbol, ByVal localOtherFieldBoxed As LocalSymbol, ByVal booleanType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim associatedField As FieldSymbol = [property].AssociatedField
				Dim syntax As SyntaxNode = MyBase.Syntax
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, localMyFieldBoxed, False, localMyFieldBoxed.Type)).MakeCompilerGenerated()
				Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, localOtherFieldBoxed, False, localOtherFieldBoxed.Type)).MakeCompilerGenerated()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.BuildAndAlso(Me.BuildIsCheck(boundLocal, boundNothing, booleanType, True), Me.BuildIsCheck(boundLocal1, boundNothing, booleanType, True), booleanType)
				Dim systemObject_Equals As MethodSymbol = Me.AnonymousType.Manager.System_Object__Equals
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLocal1)
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundCall(syntax, systemObject_Equals, Nothing, boundLocal, boundExpressions, Nothing, booleanType, False, False, bitVector)).MakeCompilerGenerated()
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundTernaryConditionalExpression(syntax, boundExpression, boundExpression1, Me.BuildIsCheck(boundLocal, boundLocal1, booleanType, False), Nothing, booleanType, False)).MakeCompilerGenerated()
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundAssignmentOperator(syntax, New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, localMyFieldBoxed, True, localMyFieldBoxed.Type), Me.BuildBoxedFieldAccess(boundMe, associatedField), True, localMyFieldBoxed.Type, False)).MakeCompilerGenerated()
				Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundAssignmentOperator(syntax, New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, localOtherFieldBoxed, True, localOtherFieldBoxed.Type), Me.BuildBoxedFieldAccess(boundOther, associatedField), True, localOtherFieldBoxed.Type, False)).MakeCompilerGenerated()
				Return (New BoundSequence(syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression3, boundExpression4), boundExpression2, boundExpression2.Type, False)).MakeCompilerGenerated()
			End Function

			Private Function BuildConditionsForFields(ByVal boundMe As BoundMeReference, ByVal boundOther As BoundParameter, ByVal boundNothing As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal localMyFieldBoxed As LocalSymbol, ByVal localOtherFieldBoxed As LocalSymbol, ByVal booleanType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim enumerator As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertySymbol).Enumerator = Me.AnonymousType.Properties.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AnonymousTypeManager.AnonymousTypePropertySymbol = enumerator.Current
					If (Not current.IsReadOnly) Then
						Continue While
					End If
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.BuildConditionForField(current, boundMe, boundOther, boundNothing, localMyFieldBoxed, localOtherFieldBoxed, booleanType)
					boundExpression = If(boundExpression Is Nothing, boundExpression1, Me.BuildAndAlso(boundExpression, boundExpression1, booleanType))
				End While
				Return boundExpression
			End Function

			Private Function BuildIsCheck(ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal booleanType As TypeSymbol, Optional ByVal reverse As Boolean = False) As BoundExpression
				Return (New BoundBinaryOperator(MyBase.Syntax, If(reverse, BinaryOperatorKind.[IsNot], BinaryOperatorKind.[Is]), left, right, False, booleanType, False)).MakeCompilerGenerated()
			End Function

			Private Function BuildOrElse(ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal booleanType As TypeSymbol) As BoundExpression
				Return (New BoundBinaryOperator(MyBase.Syntax, BinaryOperatorKind.[OrElse], left, right, False, booleanType, False)).MakeCompilerGenerated()
			End Function

			Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
				methodBodyBinder = Nothing
				Dim syntax As SyntaxNode = MyBase.Syntax
				Dim systemObject As TypeSymbol = Me.AnonymousType.Manager.System_Object
				Dim systemBoolean As TypeSymbol = Me.AnonymousType.Manager.System_Boolean
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me, systemObject, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me, systemObject, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, Me.AnonymousType)
				Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = Me._parameters
				Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(syntax, parameterSymbols(0), False, Me.AnonymousType)
				Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = (New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.[Nothing], systemObject)).MakeCompilerGenerated()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.BuildConditionsForFields(boundMeReference, boundParameter, boundLiteral, synthesizedLocal, localSymbol, systemBoolean)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.BuildIsCheck((New BoundDirectCast(syntax, boundParameter, ConversionKind.WideningReference, systemObject, False)).MakeCompilerGenerated(), boundLiteral, systemBoolean, True)
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.BuildAndAlso(boundExpression1, boundExpression, systemBoolean)
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.BuildIsCheck(boundMeReference, boundParameter, systemBoolean, False)
				boundExpression2 = Me.BuildOrElse(boundExpression3, boundExpression2, systemBoolean)
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				Return New BoundBlock(syntax, statementSyntaxes, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)(synthesizedLocal, localSymbol), ImmutableArray.Create(Of BoundStatement)((New BoundReturnStatement(syntax, boundExpression2, Nothing, Nothing, False)).MakeCompilerGenerated()), False)
			End Function
		End Class

		Private NotInheritable Class AnonymousTypeComparer
			Implements IComparer(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol)
			Private ReadOnly _compilation As VisualBasicCompilation

			Friend Sub New(ByVal compilation As VisualBasicCompilation)
				MyBase.New()
				Me._compilation = compilation
			End Sub

			Public Function Compare(ByVal x As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol, ByVal y As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol) As Integer Implements IComparer(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol).Compare
				Dim num As Integer
				If (CObj(x) <> CObj(y)) Then
					Dim num1 As Integer = Me.CompareLocations(x.SmallestLocation, y.SmallestLocation)
					If (num1 = 0) Then
						num1 = x.TypeDescriptorKey.CompareTo(y.TypeDescriptorKey)
					End If
					num = num1
				Else
					num = 0
				End If
				Return num
			End Function

			Private Function CompareLocations(ByVal x As Location, ByVal y As Location) As Integer
				Dim num As Integer
				If (CObj(x) = CObj(y)) Then
					num = 0
				ElseIf (x <> Location.None) Then
					num = If(y <> Location.None, Me._compilation.CompareSourceLocations(x, y), 1)
				Else
					num = -1
				End If
				Return num
			End Function
		End Class

		Private NotInheritable Class AnonymousTypeConstructorSymbol
			Inherits SynthesizedConstructorBase
			Private _parameters As ImmutableArray(Of ParameterSymbol)

			Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
				Get
					Return False
				End Get
			End Property

			Friend Overrides ReadOnly Property ParameterCount As Integer
				Get
					Return Me._parameters.Length
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Public Sub New(ByVal container As AnonymousTypeManager.AnonymousTypeTemplateSymbol)
				MyBase.New(VisualBasicSyntaxTree.DummyReference, container, False, Nothing, Nothing)
				Dim length As Integer = container.Properties.Length
				Dim anonymousTypeOrDelegateParameterSymbol(length - 1 + 1 - 1) As ParameterSymbol
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					Dim item As PropertySymbol = container.Properties(num1)
					anonymousTypeOrDelegateParameterSymbol(num1) = New AnonymousTypeManager.AnonymousTypeOrDelegateParameterSymbol(Me, item.Type, num1, False, item.Name, num1)
					num1 = num1 + 1
				Loop While num1 <= num
				Me._parameters = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(anonymousTypeOrDelegateParameterSymbol)
			End Sub

			Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
				MyBase.AddSynthesizedAttributes(compilationState, attributes)
				Dim compilation As VisualBasicCompilation = DirectCast(MyBase.ContainingType, AnonymousTypeManager.AnonymousTypeTemplateSymbol).Manager.Compilation
				Symbol.AddSynthesizedAttribute(attributes, compilation.SynthesizeDebuggerHiddenAttribute())
			End Sub

			Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
				methodBodyBinder = Nothing
				Dim syntax As SyntaxNode = MyBase.Syntax
				Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
				Dim containingType As AnonymousTypeManager.AnonymousTypeTemplateSymbol = DirectCast(MyBase.ContainingType, AnonymousTypeManager.AnonymousTypeTemplateSymbol)
				Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = (New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, containingType)).MakeCompilerGenerated()
				Dim parameterCount As Integer = Me.ParameterCount - 1
				Dim num As Integer = 0
				Do
					Dim item As AnonymousTypeManager.AnonymousTypePropertySymbol = containingType.Properties(num)
					Dim type As TypeSymbol = item.Type
					Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = (New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntax, boundMeReference, item.AssociatedField, True, type, False)).MakeCompilerGenerated()
					Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = (New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(syntax, Me._parameters(num), False, type)).MakeCompilerGenerated()
					Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = (New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, boundFieldAccess, boundParameter, False, type, False)).MakeCompilerGenerated()
					instance.Add((New BoundExpressionStatement(syntax, boundAssignmentOperator, False)).MakeCompilerGenerated())
					num = num + 1
				Loop While num <= parameterCount
				instance.Add((New BoundReturnStatement(syntax, Nothing, Nothing, Nothing, False)).MakeCompilerGenerated())
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				Return (New BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, instance.ToImmutableAndFree(), False)).MakeCompilerGenerated()
			End Function
		End Class

		Private NotInheritable Class AnonymousTypeEqualsMethodSymbol
			Inherits SynthesizedRegularMethodBase
			Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

			Private ReadOnly _iEquatableEqualsMethod As MethodSymbol

			Private ReadOnly Property AnonymousType As AnonymousTypeManager.AnonymousTypeTemplateSymbol
				Get
					Return DirectCast(Me.m_containingType, AnonymousTypeManager.AnonymousTypeTemplateSymbol)
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
				Get
					Return Accessibility.[Public]
				End Get
			End Property

			Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsOverloads As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property IsOverridable As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsOverrides As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property IsSub As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
				Get
					Return Me.AnonymousType.Manager.System_Object__Equals
				End Get
			End Property

			Friend Overrides ReadOnly Property ParameterCount As Integer
				Get
					Return 1
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me.AnonymousType.Manager.System_Boolean
				End Get
			End Property

			Public Sub New(ByVal container As AnonymousTypeManager.AnonymousTypeTemplateSymbol, ByVal iEquatableEqualsMethod As MethodSymbol)
				MyBase.New(VisualBasicSyntaxTree.Dummy.GetRoot(New CancellationToken()), container, "Equals", False)
				Me._parameters = ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSimpleSymbol(Me, container.Manager.System_Object, 0, "obj"))
				Me._iEquatableEqualsMethod = iEquatableEqualsMethod
			End Sub

			Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
				MyBase.AddSynthesizedAttributes(compilationState, attributes)
				Dim compilation As VisualBasicCompilation = DirectCast(MyBase.ContainingType, AnonymousTypeManager.AnonymousTypeTemplateSymbol).Manager.Compilation
				Symbol.AddSynthesizedAttribute(attributes, compilation.SynthesizeDebuggerHiddenAttribute())
			End Sub

			Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
				methodBodyBinder = Nothing
				Dim syntax As SyntaxNode = MyBase.Syntax
				Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = (New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, Me.AnonymousType)).MakeCompilerGenerated()
				Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = Me._parameters
				Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = (New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(syntax, parameterSymbols(0), False, Me.AnonymousType.Manager.System_Object)).MakeCompilerGenerated()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundTryCast(syntax, boundParameter, ConversionKind.NarrowingReference, Me.AnonymousType, False)).MakeCompilerGenerated()
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._iEquatableEqualsMethod
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression)
				Dim systemBoolean As NamedTypeSymbol = Me.AnonymousType.Manager.System_Boolean
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundCall(syntax, methodSymbol, Nothing, boundMeReference, boundExpressions, Nothing, systemBoolean, False, False, bitVector)).MakeCompilerGenerated()
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				Return New BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)((New BoundReturnStatement(syntax, boundExpression1, Nothing, Nothing, False)).MakeCompilerGenerated()), False)
			End Function
		End Class

		Private NotInheritable Class AnonymousTypeGetHashCodeMethodSymbol
			Inherits SynthesizedRegularMethodBase
			Private ReadOnly Property AnonymousType As AnonymousTypeManager.AnonymousTypeTemplateSymbol
				Get
					Return DirectCast(Me.m_containingType, AnonymousTypeManager.AnonymousTypeTemplateSymbol)
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
				Get
					Return Accessibility.[Public]
				End Get
			End Property

			Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsOverrides As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property IsSub As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
				Get
					Return Me.AnonymousType.Manager.System_Object__GetHashCode
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me.AnonymousType.Manager.System_Int32
				End Get
			End Property

			Public Sub New(ByVal container As AnonymousTypeManager.AnonymousTypeTemplateSymbol)
				MyBase.New(VisualBasicSyntaxTree.Dummy.GetRoot(New CancellationToken()), container, "GetHashCode", False)
			End Sub

			Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
				MyBase.AddSynthesizedAttributes(compilationState, attributes)
				Dim compilation As VisualBasicCompilation = DirectCast(MyBase.ContainingType, AnonymousTypeManager.AnonymousTypeTemplateSymbol).Manager.Compilation
				Symbol.AddSynthesizedAttribute(attributes, compilation.SynthesizeDebuggerHiddenAttribute())
			End Sub

			Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
				methodBodyBinder = Nothing
				Dim syntax As SyntaxNode = MyBase.Syntax
				Dim systemObject As TypeSymbol = Me.AnonymousType.Manager.System_Object
				Dim systemObject_GetHashCode As MethodSymbol = Me.AnonymousType.Manager.System_Object__GetHashCode
				Dim systemInt32 As TypeSymbol = Me.AnonymousType.Manager.System_Int32
				Dim systemBoolean As TypeSymbol = Me.AnonymousType.Manager.System_Boolean
				Dim properties As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertySymbol) = Me.AnonymousType.Properties
				Dim name(properties.Length - 1 + 1 - 1) As [String]
				Dim length As Integer = properties.Length - 1
				Dim num As Integer = 0
				Do
					name(num) = properties(num).Name
					num = num + 1
				Loop While num <= length
				Dim num1 As Integer = CInt(CRC32.ComputeCRC32(name))
				Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = (New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, Me.AnonymousType)).MakeCompilerGenerated()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				boundExpression = (New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.Create(num1), systemInt32)).MakeCompilerGenerated()
				Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = (New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.Create(-1521134295), systemInt32)).MakeCompilerGenerated()
				Dim boundLiteral1 As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = (New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.Create(0), systemInt32)).MakeCompilerGenerated()
				Dim boundLiteral2 As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = (New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.[Nothing], systemObject)).MakeCompilerGenerated()
				Dim enumerator As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertySymbol).Enumerator = Me.AnonymousType.Properties.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AnonymousTypeManager.AnonymousTypePropertySymbol = enumerator.Current
					If (Not current.IsReadOnly) Then
						Continue While
					End If
					boundExpression = (New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(syntax, BinaryOperatorKind.Multiply, boundExpression, boundLiteral, False, systemInt32, False)).MakeCompilerGenerated()
					Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = (New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(syntax, BinaryOperatorKind.[Is], (New BoundDirectCast(syntax, (New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntax, boundMeReference, current.AssociatedField, False, current.Type, False)).MakeCompilerGenerated(), ConversionKind.WideningTypeParameter, systemObject, False)).MakeCompilerGenerated(), boundLiteral2, False, systemBoolean, False)).MakeCompilerGenerated()
					Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = (New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntax, boundMeReference, current.AssociatedField, False, current.Type, False)).MakeCompilerGenerated()
					Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
					Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = (New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, systemObject_GetHashCode, Nothing, boundFieldAccess, empty, Nothing, systemInt32, False, False, bitVector)).MakeCompilerGenerated()
					Dim boundTernaryConditionalExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression = (New Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression(syntax, boundBinaryOperator, boundLiteral1, boundCall, Nothing, systemInt32, False)).MakeCompilerGenerated()
					boundExpression = (New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(syntax, BinaryOperatorKind.Add, boundExpression, boundTernaryConditionalExpression, False, systemInt32, False)).MakeCompilerGenerated()
				End While
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				Return New BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)((New BoundReturnStatement(syntax, boundExpression, Nothing, Nothing, False)).MakeCompilerGenerated()), False)
			End Function
		End Class

		Private NotInheritable Class AnonymousTypeOrDelegateParameterSymbol
			Inherits SynthesizedParameterSymbol
			Public ReadOnly CorrespondingInvokeParameterOrProperty As Integer

			Public Overrides ReadOnly Property MetadataName As String
				Get
					Dim str As String
					str = If(Me.CorrespondingInvokeParameterOrProperty = -1, MyBase.MetadataName, DirectCast(Me._container.ContainingSymbol, AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol).GetAdjustedName(Me.CorrespondingInvokeParameterOrProperty))
					Return str
				End Get
			End Property

			Public Sub New(ByVal container As MethodSymbol, ByVal type As TypeSymbol, ByVal ordinal As Integer, ByVal isByRef As Boolean, ByVal name As String, Optional ByVal correspondingInvokeParameterOrProperty As Integer = -1)
				MyBase.New(container, type, ordinal, isByRef, name)
				Me.CorrespondingInvokeParameterOrProperty = correspondingInvokeParameterOrProperty
			End Sub
		End Class

		Friend MustInherit Class AnonymousTypeOrDelegatePublicSymbol
			Inherits InstanceTypeSymbol
			Public ReadOnly Manager As AnonymousTypeManager

			Public ReadOnly TypeDescriptor As AnonymousTypeDescriptor

			Public Overrides ReadOnly Property Arity As Integer
				Get
					Return 0
				End Get
			End Property

			Friend Overrides ReadOnly Property CoClassType As TypeSymbol
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me.Manager.ContainingModule.GlobalNamespace
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
				Get
					Return Accessibility.Internal
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

			Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
				Get
					Return False
				End Get
			End Property

			Friend Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsAnonymousType As Boolean
				Get
					Return True
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

			Public NotOverridable Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
				Get
					Return Me.TypeDescriptor.IsImplicitlyDeclared
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

			Public NotOverridable Overrides ReadOnly Property IsSerializable As Boolean
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
					Return ImmutableArray.Create(Of Location)(Me.TypeDescriptor.Location)
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property MangleName As Boolean
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
					If (AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol._Closure$__.$I39-0 Is Nothing) Then
						name = Function(member As Symbol) member.Name
						AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol._Closure$__.$I39-0 = name
					Else
						name = AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol._Closure$__.$I39-0
					End If
					Return New HashSet(Of String)(members.[Select](Of String)(name))
				End Get
			End Property

			Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
				Get
					Return False
				End Get
			End Property

			Public NotOverridable Overrides ReadOnly Property Name As String
				Get
					Return [String].Empty
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As ObsoleteAttributeData
				Get
					Return Nothing
				End Get
			End Property

			Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return ImmutableArray(Of TypeParameterSymbol).Empty
				End Get
			End Property

			Protected Sub New(ByVal manager As AnonymousTypeManager, ByVal typeDescr As AnonymousTypeDescriptor)
				MyBase.New()
				Me.Manager = manager
				Me.TypeDescriptor = typeDescr
			End Sub

			Public NotOverridable Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
				Return Me.Equals(TryCast(other, AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol), comparison)
			End Function

			Public Function Equals(ByVal other As AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol, ByVal comparison As TypeCompareKind) As Boolean
				Dim flag As Boolean
				If (CObj(Me) <> CObj(other)) Then
					flag = If(other Is Nothing OrElse Me.TypeKind <> other.TypeKind, False, Me.TypeDescriptor.Equals(other.TypeDescriptor, comparison))
				Else
					flag = True
				End If
				Return flag
			End Function

			Private Shared Function FindMethodInTypeProvided(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal type As NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
				Dim memberForDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
				If (Not type.IsDefinition) Then
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol.FindMethodInTypeProvided(method, type.OriginalDefinition)
					memberForDefinition = DirectCast(DirectCast(type, SubstitutedNamedType).GetMemberForDefinition(methodSymbol), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				Else
					Dim num As Integer = 0
					Dim members As ImmutableArray(Of Symbol) = method.ContainingType.GetMembers()
					Dim enumerator As ImmutableArray(Of Symbol).Enumerator = members.GetEnumerator()
					While enumerator.MoveNext() AndAlso enumerator.Current <> method
						num = num + 1
					End While
					members = type.GetMembers()
					memberForDefinition = DirectCast(members(num), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				End If
				Return memberForDefinition
			End Function

			Public Function FindSubstitutedMethodSymbol(ByVal method As MethodSymbol) As MethodSymbol
				Return AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol.FindMethodInTypeProvided(method, Me)
			End Function

			Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
				Throw ExceptionUtilities.Unreachable
			End Sub

			Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
				Return ImmutableArray(Of String).Empty
			End Function

			Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend NotOverridable Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
				Throw ExceptionUtilities.Unreachable
			End Function

			Public NotOverridable Overrides Function GetHashCode() As Integer
				Dim typeDescriptor As AnonymousTypeDescriptor = Me.TypeDescriptor
				Return Hash.Combine(typeDescriptor.GetHashCode(), CInt(Me.TypeKind))
			End Function

			Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
				Return ImmutableArray.CreateRange(Of Symbol)(Me.GetMembers().Where(Function(member As Symbol) CaseInsensitiveComparison.Equals(member.Name, name)))
			End Function

			Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend NotOverridable Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
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

			Friend Overrides MustOverride Function InternalSubstituteTypeParameters(ByVal substitution As TypeSubstitution) As TypeWithModifiers

			Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
				Return Me.MakeAcyclicBaseType(diagnostics)
			End Function

			Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
				Return Me.MakeAcyclicInterfaces(diagnostics)
			End Function

			Public Function MapMethodToImplementationSymbol(ByVal method As MethodSymbol) As MethodSymbol
				Return AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol.FindMethodInTypeProvided(method, Me.MapToImplementationSymbol())
			End Function

			Public MustOverride Function MapToImplementationSymbol() As NamedTypeSymbol
		End Class

		Friend MustInherit Class AnonymousTypeOrDelegateTemplateSymbol
			Inherits InstanceTypeSymbol
			Public ReadOnly Manager As AnonymousTypeManager

			Private _nameAndIndex As AnonymousTypeManager.NameAndIndex

			Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

			Private _adjustedPropertyNames As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol.LocationAndNames

			Friend ReadOnly TypeDescriptorKey As String

			Public Overrides ReadOnly Property Arity As Integer
				Get
					Return Me._typeParameters.Length
				End Get
			End Property

			Friend Overrides ReadOnly Property CoClassType As TypeSymbol
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me.Manager.ContainingModule.GlobalNamespace
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
				Get
					Return Accessibility.Internal
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

			Friend MustOverride ReadOnly Property GeneratedNamePrefix As String

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

			Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
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
					Return True
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

			Public NotOverridable Overrides ReadOnly Property IsSerializable As Boolean
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
					Return ImmutableArray(Of Location).Empty
				End Get
			End Property

			Friend Overrides ReadOnly Property MangleName As Boolean
				Get
					Return Me._typeParameters.Length > 0
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
					If (AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol._Closure$__.$I56-0 Is Nothing) Then
						name = Function(member As Symbol) member.Name
						AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol._Closure$__.$I56-0 = name
					Else
						name = AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol._Closure$__.$I56-0
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
					Return Me._nameAndIndex.Name
				End Get
			End Property

			Friend Property NameAndIndex As AnonymousTypeManager.NameAndIndex
				Get
					Return Me._nameAndIndex
				End Get
				Set(ByVal value As AnonymousTypeManager.NameAndIndex)
					Interlocked.CompareExchange(Of AnonymousTypeManager.NameAndIndex)(Me._nameAndIndex, value, Nothing)
				End Set
			End Property

			Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As ObsoleteAttributeData
				Get
					Return Nothing
				End Get
			End Property

			Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
				Get
					Return False
				End Get
			End Property

			Public ReadOnly Property SmallestLocation As Location
				Get
					Return Me._adjustedPropertyNames.Location
				End Get
			End Property

			Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return Me._typeParameters
				End Get
			End Property

			Protected Sub New(ByVal manager As AnonymousTypeManager, ByVal typeDescr As AnonymousTypeDescriptor)
				MyBase.New()
				Me.Manager = manager
				Me.TypeDescriptorKey = typeDescr.Key
				Me._adjustedPropertyNames = New AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol.LocationAndNames(typeDescr)
				Dim length As Integer = typeDescr.Fields.Length
				If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate] AndAlso typeDescr.Fields.IsSubDescription()) Then
					length = length - 1
				End If
				If (length = 0) Then
					Me._typeParameters = ImmutableArray(Of TypeParameterSymbol).Empty
					Return
				End If
				Dim anonymousTypeOrDelegateTypeParameterSymbol(length - 1 + 1 - 1) As TypeParameterSymbol
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					anonymousTypeOrDelegateTypeParameterSymbol(num1) = New AnonymousTypeManager.AnonymousTypeOrDelegateTypeParameterSymbol(Me, num1)
					num1 = num1 + 1
				Loop While num1 <= num
				Me._typeParameters = anonymousTypeOrDelegateTypeParameterSymbol.AsImmutable(Of TypeParameterSymbol)()
			End Sub

			Friend Sub AdjustMetadataNames(ByVal typeDescr As AnonymousTypeDescriptor)
				Dim locationAndName As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol.LocationAndNames
				Dim locationAndName1 As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol.LocationAndNames
				Dim location As Microsoft.CodeAnalysis.Location = typeDescr.Location
				Do
					locationAndName = Me._adjustedPropertyNames
					If (locationAndName IsNot Nothing AndAlso Me.Manager.Compilation.CompareSourceLocations(locationAndName.Location, location) <= 0) Then
						Exit Do
					End If
					locationAndName1 = New AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol.LocationAndNames(typeDescr)
				Loop While Interlocked.CompareExchange(Of AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol.LocationAndNames)(Me._adjustedPropertyNames, locationAndName1, locationAndName) <> locationAndName
			End Sub

			Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
				Throw ExceptionUtilities.Unreachable
			End Sub

			Friend Function GetAdjustedName(ByVal index As Integer) As String
				Return Me._adjustedPropertyNames.Names(index)
			End Function

			Friend MustOverride Function GetAnonymousTypeKey() As AnonymousTypeKey

			Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
				Return ImmutableArray(Of String).Empty
			End Function

			Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
				Return ImmutableArray.CreateRange(Of Symbol)(Me.GetMembers().Where(Function(member As Symbol) CaseInsensitiveComparison.Equals(member.Name, name)))
			End Function

			Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend NotOverridable Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
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

			Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
				Return Me.MakeAcyclicBaseType(diagnostics)
			End Function

			Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
				Return Me.MakeAcyclicInterfaces(diagnostics)
			End Function

			Private NotInheritable Class LocationAndNames
				Public ReadOnly Location As Location

				Public ReadOnly Names As ImmutableArray(Of String)

				Public Sub New(ByVal typeDescr As AnonymousTypeDescriptor)
					MyBase.New()
					Dim name As Func(Of AnonymousTypeField, String)
					Me.Location = typeDescr.Location
					Dim fields As ImmutableArray(Of AnonymousTypeField) = typeDescr.Fields
					If (AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol.LocationAndNames._Closure$__.$I2-0 Is Nothing) Then
						name = Function(d As AnonymousTypeField) d.Name
						AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol.LocationAndNames._Closure$__.$I2-0 = name
					Else
						name = AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol.LocationAndNames._Closure$__.$I2-0
					End If
					Me.Names = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of AnonymousTypeField, String)(fields, name)
				End Sub
			End Class
		End Class

		Private NotInheritable Class AnonymousTypeOrDelegateTypeParameterSymbol
			Inherits TypeParameterSymbol
			Private ReadOnly _container As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol

			Private ReadOnly _ordinal As Integer

			Friend Overrides ReadOnly Property ConstraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
				Get
					Return ImmutableArray(Of TypeSymbol).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._container
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return ImmutableArray(Of SyntaxReference).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property HasConstructorConstraint As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property HasReferenceTypeConstraint As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property HasValueTypeConstraint As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return ImmutableArray(Of Location).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Dim str As String
					If (Me._container.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
						str = [String].Concat("T", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(Me.Ordinal))
					Else
						str = If(Me._container.DelegateInvokeMethod.IsSub OrElse Me.Ordinal < Me._container.Arity - 1, [String].Concat("TArg", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(Me.Ordinal)), "TResult")
					End If
					Return str
				End Get
			End Property

			Public Overrides ReadOnly Property Ordinal As Integer
				Get
					Return Me._ordinal
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameterKind As TypeParameterKind
				Get
					Return TypeParameterKind.Type
				End Get
			End Property

			Public Overrides ReadOnly Property Variance As VarianceKind
				Get
					Return VarianceKind.None
				End Get
			End Property

			Public Sub New(ByVal container As AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol, ByVal ordinal As Integer)
				MyBase.New()
				Me._container = container
				Me._ordinal = ordinal
			End Sub

			Friend Overrides Sub EnsureAllConstraintsAreResolved()
			End Sub

			Public Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
				Return other = Me
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return RuntimeHelpers.GetHashCode(Me)
			End Function
		End Class

		Private MustInherit Class AnonymousTypePropertyAccessorPublicSymbol
			Inherits SynthesizedPropertyAccessorBase(Of PropertySymbol)
			Private ReadOnly _returnType As TypeSymbol

			Friend NotOverridable Overrides ReadOnly Property BackingFieldSymbol As FieldSymbol
				Get
					Throw ExceptionUtilities.Unreachable
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me._returnType
				End Get
			End Property

			Public Sub New(ByVal [property] As PropertySymbol, ByVal returnType As TypeSymbol)
				MyBase.New([property].ContainingType, [property])
				Me._returnType = returnType
			End Sub

			Friend NotOverridable Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
				Throw ExceptionUtilities.Unreachable
			End Function
		End Class

		Private MustInherit Class AnonymousTypePropertyAccessorSymbol
			Inherits SynthesizedPropertyAccessorBase(Of PropertySymbol)
			Private ReadOnly _returnType As TypeSymbol

			Friend NotOverridable Overrides ReadOnly Property BackingFieldSymbol As FieldSymbol
				Get
					Return DirectCast(Me.m_propertyOrEvent, AnonymousTypeManager.AnonymousTypePropertySymbol).AssociatedField
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me._returnType
				End Get
			End Property

			Public Sub New(ByVal [property] As PropertySymbol, ByVal returnType As TypeSymbol)
				MyBase.New([property].ContainingType, [property])
				Me._returnType = returnType
			End Sub

			Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
				MyBase.AddSynthesizedAttributes(compilationState, attributes)
			End Sub

			Friend NotOverridable Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
				Throw ExceptionUtilities.Unreachable
			End Function

			Protected Overrides Function GenerateMetadataName() As String
				Return Binder.GetAccessorName(Me.m_propertyOrEvent.MetadataName, Me.MethodKind, Me.IsCompilationOutputWinMdObj())
			End Function
		End Class

		Private NotInheritable Class AnonymousTypePropertyBackingFieldSymbol
			Inherits SynthesizedBackingFieldBase(Of PropertySymbol)
			Public Overrides ReadOnly Property IsReadOnly As Boolean
				Get
					Return Me._propertyOrEvent.IsReadOnly
				End Get
			End Property

			Public Overrides ReadOnly Property MetadataName As String
				Get
					Return [String].Concat("$", Me._propertyOrEvent.MetadataName)
				End Get
			End Property

			Public Overrides ReadOnly Property Type As TypeSymbol
				Get
					Return Me._propertyOrEvent.Type
				End Get
			End Property

			Public Sub New(ByVal [property] As PropertySymbol)
				MyBase.New([property], [String].Concat("$", [property].Name), False)
			End Sub
		End Class

		Private NotInheritable Class AnonymousTypePropertyGetAccessorPublicSymbol
			Inherits AnonymousTypeManager.AnonymousTypePropertyAccessorPublicSymbol
			Public Overrides ReadOnly Property IsSub As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property MethodKind As MethodKind
				Get
					Return MethodKind.PropertyGet
				End Get
			End Property

			Public Sub New(ByVal [property] As PropertySymbol)
				MyBase.New([property], [property].Type)
			End Sub
		End Class

		Private NotInheritable Class AnonymousTypePropertyGetAccessorSymbol
			Inherits AnonymousTypeManager.AnonymousTypePropertyAccessorSymbol
			Public Overrides ReadOnly Property IsSub As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property MethodKind As MethodKind
				Get
					Return MethodKind.PropertyGet
				End Get
			End Property

			Public Sub New(ByVal [property] As PropertySymbol)
				MyBase.New([property], [property].Type)
			End Sub
		End Class

		Friend NotInheritable Class AnonymousTypePropertyPublicSymbol
			Inherits SynthesizedPropertyBase
			Private ReadOnly _container As AnonymousTypeManager.AnonymousTypePublicSymbol

			Private ReadOnly _getMethod As MethodSymbol

			Private ReadOnly _setMethod As MethodSymbol

			Friend ReadOnly PropertyIndex As Integer

			Friend ReadOnly Property AnonymousType As AnonymousTypeManager.AnonymousTypePublicSymbol
				Get
					Return Me._container
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._container
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
				Get
					Return Me._container
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return Symbol.GetDeclaringSyntaxReferenceHelper(Of FieldInitializerSyntax)(Me.Locations)
				End Get
			End Property

			Public Overrides ReadOnly Property GetMethod As MethodSymbol
				Get
					Return Me._getMethod
				End Get
			End Property

			Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
				Get
					Return Me.ContainingType.IsImplicitlyDeclared
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Dim fields As ImmutableArray(Of AnonymousTypeField) = Me._container.TypeDescriptor.Fields
					Return ImmutableArray.Create(Of Location)(fields(Me.PropertyIndex).Location)
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Return Me._container.TypeDescriptor.Fields(Me.PropertyIndex).Name
				End Get
			End Property

			Public Overrides ReadOnly Property SetMethod As MethodSymbol
				Get
					Return Me._setMethod
				End Get
			End Property

			Public Overrides ReadOnly Property Type As TypeSymbol
				Get
					Return Me._container.TypeDescriptor.Fields(Me.PropertyIndex).Type
				End Get
			End Property

			Public Sub New(ByVal container As AnonymousTypeManager.AnonymousTypePublicSymbol, ByVal index As Integer)
				MyBase.New()
				Me._container = container
				Me.PropertyIndex = index
				Me._getMethod = New AnonymousTypeManager.AnonymousTypePropertyGetAccessorPublicSymbol(Me)
				If (Not container.TypeDescriptor.Fields(index).IsKey) Then
					Me._setMethod = New AnonymousTypeManager.AnonymousTypePropertySetAccessorPublicSymbol(Me, container.Manager.System_Void)
				End If
			End Sub

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim flag As Boolean
				If (obj Is Nothing) Then
					flag = False
				ElseIf (obj <> Me) Then
					Dim anonymousTypePropertyPublicSymbol As AnonymousTypeManager.AnonymousTypePropertyPublicSymbol = TryCast(obj, AnonymousTypeManager.AnonymousTypePropertyPublicSymbol)
					If (anonymousTypePropertyPublicSymbol IsNot Nothing) Then
						flag = If(anonymousTypePropertyPublicSymbol Is Nothing OrElse Not CaseInsensitiveComparison.Equals(anonymousTypePropertyPublicSymbol.Name, Me.Name), False, anonymousTypePropertyPublicSymbol.ContainingType.Equals(Me.ContainingType))
					Else
						flag = False
					End If
				Else
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Hash.Combine(Me.ContainingType.GetHashCode(), CaseInsensitiveComparison.GetHashCode(Me.Name))
			End Function
		End Class

		Private NotInheritable Class AnonymousTypePropertySetAccessorPublicSymbol
			Inherits AnonymousTypeManager.AnonymousTypePropertyAccessorPublicSymbol
			Private _parameters As ImmutableArray(Of ParameterSymbol)

			Public Overrides ReadOnly Property IsSub As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property MethodKind As MethodKind
				Get
					Return MethodKind.PropertySet
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Public Sub New(ByVal [property] As PropertySymbol, ByVal voidTypeSymbol As TypeSymbol)
				MyBase.New([property], voidTypeSymbol)
				Me._parameters = ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSymbol(Me, Me.m_propertyOrEvent.Type, 0, False, "Value"))
			End Sub
		End Class

		Private NotInheritable Class AnonymousTypePropertySetAccessorSymbol
			Inherits AnonymousTypeManager.AnonymousTypePropertyAccessorSymbol
			Private _parameters As ImmutableArray(Of ParameterSymbol)

			Public Overrides ReadOnly Property IsSub As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property MethodKind As MethodKind
				Get
					Return MethodKind.PropertySet
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Public Sub New(ByVal [property] As PropertySymbol, ByVal voidTypeSymbol As TypeSymbol)
				MyBase.New([property], voidTypeSymbol)
				Me._parameters = ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSymbol(Me, Me.m_propertyOrEvent.Type, 0, False, "Value"))
			End Sub
		End Class

		Private NotInheritable Class AnonymousTypePropertySymbol
			Inherits PropertySymbol
			Private ReadOnly _containingType As AnonymousTypeManager.AnonymousTypeTemplateSymbol

			Private ReadOnly _type As TypeSymbol

			Private ReadOnly _name As String

			Private ReadOnly _getMethod As MethodSymbol

			Private ReadOnly _setMethod As MethodSymbol

			Private ReadOnly _backingField As FieldSymbol

			Friend ReadOnly PropertyIndex As Integer

			Friend ReadOnly Property AnonymousType As AnonymousTypeManager.AnonymousTypeTemplateSymbol
				Get
					Return Me._containingType
				End Get
			End Property

			Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
				Get
					Return Me._backingField
				End Get
			End Property

			Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
				Get
					Return Microsoft.Cci.CallingConvention.HasThis
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
					Return Accessibility.[Public]
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return ImmutableArray(Of SyntaxReference).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
				Get
					Return ImmutableArray(Of PropertySymbol).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property GetMethod As MethodSymbol
				Get
					Return Me._getMethod
				End Get
			End Property

			Friend Overrides ReadOnly Property HasSpecialName As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsDefault As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property IsMustOverride As Boolean
				Get
					Return False
				End Get
			End Property

			Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsNotOverridable As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsOverloads As Boolean
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

			Public Overrides ReadOnly Property IsShared As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return ImmutableArray(Of Location).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property MetadataName As String
				Get
					Return Me.AnonymousType.GetAdjustedName(Me.PropertyIndex)
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Return Me._name
				End Get
			End Property

			Friend Overrides ReadOnly Property ObsoleteAttributeData As ObsoleteAttributeData
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return ImmutableArray(Of ParameterSymbol).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return ImmutableArray(Of CustomModifier).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnsByRef As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property SetMethod As MethodSymbol
				Get
					Return Me._setMethod
				End Get
			End Property

			Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property Type As TypeSymbol
				Get
					Return Me._type
				End Get
			End Property

			Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return ImmutableArray(Of CustomModifier).Empty
				End Get
			End Property

			Public Sub New(ByVal container As AnonymousTypeManager.AnonymousTypeTemplateSymbol, ByVal field As AnonymousTypeField, ByVal index As Integer, ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
				MyBase.New()
				Me._containingType = container
				Me._type = typeSymbol
				Me._name = field.Name
				Me.PropertyIndex = index
				Me._getMethod = New AnonymousTypeManager.AnonymousTypePropertyGetAccessorSymbol(Me)
				If (Not field.IsKey) Then
					Me._setMethod = New AnonymousTypeManager.AnonymousTypePropertySetAccessorSymbol(Me, container.Manager.System_Void)
				End If
				Me._backingField = New AnonymousTypeManager.AnonymousTypePropertyBackingFieldSymbol(Me)
			End Sub
		End Class

		Friend NotInheritable Class AnonymousTypePublicSymbol
			Inherits AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol
			Private ReadOnly _properties As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertyPublicSymbol)

			Private ReadOnly _members As ImmutableArray(Of Symbol)

			Private ReadOnly _interfaces As ImmutableArray(Of NamedTypeSymbol)

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return Symbol.GetDeclaringSyntaxReferenceHelper(Of AnonymousObjectCreationExpressionSyntax)(Me.Locations)
				End Get
			End Property

			Friend Overrides ReadOnly Property IsInterface As Boolean
				Get
					Return False
				End Get
			End Property

			Public ReadOnly Property Properties As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertyPublicSymbol)
				Get
					Return Me._properties
				End Get
			End Property

			Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
				Get
					Return Microsoft.CodeAnalysis.TypeKind.[Class]
				End Get
			End Property

			Public Sub New(ByVal manager As AnonymousTypeManager, ByVal typeDescr As AnonymousTypeDescriptor)
				MyBase.New(manager, typeDescr)
				Dim length As Integer = typeDescr.Fields.Length
				Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				Dim symbols As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				Dim anonymousTypePropertyPublicSymbolArray(length - 1 + 1 - 1) As AnonymousTypeManager.AnonymousTypePropertyPublicSymbol
				Dim flag As Boolean = False
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					If (typeDescr.Fields(num1).IsKey) Then
						flag = True
					End If
					Dim anonymousTypePropertyPublicSymbol As AnonymousTypeManager.AnonymousTypePropertyPublicSymbol = New AnonymousTypeManager.AnonymousTypePropertyPublicSymbol(Me, num1)
					anonymousTypePropertyPublicSymbolArray(num1) = anonymousTypePropertyPublicSymbol
					symbols.Add(anonymousTypePropertyPublicSymbol)
					instance.Add(anonymousTypePropertyPublicSymbol.GetMethod)
					If (anonymousTypePropertyPublicSymbol.SetMethod IsNot Nothing) Then
						instance.Add(anonymousTypePropertyPublicSymbol.SetMethod)
					End If
					num1 = num1 + 1
				Loop While num1 <= num
				Me._properties = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of AnonymousTypeManager.AnonymousTypePropertyPublicSymbol)(anonymousTypePropertyPublicSymbolArray)
				instance.Add(Me.CreateConstructorSymbol())
				instance.Add(Me.CreateToStringMethod())
				If (Not flag OrElse Me.Manager.System_IEquatable_T_Equals Is Nothing) Then
					Me._interfaces = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Empty
				Else
					instance.Add(Me.CreateGetHashCodeMethod())
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Manager.System_IEquatable_T.Construct(ImmutableArray.Create(Of TypeSymbol)(Me))
					Me._interfaces = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(namedTypeSymbol)
					Dim memberForDefinition As Symbol = DirectCast(namedTypeSymbol, SubstitutedNamedType).GetMemberForDefinition(Me.Manager.System_IEquatable_T_Equals)
					instance.Add(Me.CreateIEquatableEqualsMethod(DirectCast(memberForDefinition, MethodSymbol)))
					instance.Add(Me.CreateEqualsMethod())
				End If
				instance.AddRange(symbols)
				symbols.Free()
				Me._members = instance.ToImmutableAndFree()
			End Sub

			Private Function CreateConstructorSymbol() As MethodSymbol
				Dim synthesizedSimpleConstructorSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSimpleConstructorSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSimpleConstructorSymbol(Me)
				Dim length As Integer = Me._properties.Length
				Dim synthesizedParameterSimpleSymbol(length - 1 + 1 - 1) As ParameterSymbol
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					Dim item As PropertySymbol = Me._properties(num1)
					synthesizedParameterSimpleSymbol(num1) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedParameterSimpleSymbol(synthesizedSimpleConstructorSymbol, item.Type, num1, item.Name)
					num1 = num1 + 1
				Loop While num1 <= num
				synthesizedSimpleConstructorSymbol.SetParameters(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(synthesizedParameterSimpleSymbol))
				Return synthesizedSimpleConstructorSymbol
			End Function

			Private Function CreateEqualsMethod() As MethodSymbol
				Dim synthesizedSimpleMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSimpleMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSimpleMethodSymbol(Me, "Equals", Me.Manager.System_Boolean, Me.Manager.System_Object__Equals, Nothing, True)
				synthesizedSimpleMethodSymbol.SetParameters(ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSimpleSymbol(synthesizedSimpleMethodSymbol, Me.Manager.System_Object, 0, "obj")))
				Return synthesizedSimpleMethodSymbol
			End Function

			Private Function CreateGetHashCodeMethod() As MethodSymbol
				Dim synthesizedSimpleMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSimpleMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSimpleMethodSymbol(Me, "GetHashCode", Me.Manager.System_Int32, Me.Manager.System_Object__GetHashCode, Nothing, False)
				synthesizedSimpleMethodSymbol.SetParameters(ImmutableArray(Of ParameterSymbol).Empty)
				Return synthesizedSimpleMethodSymbol
			End Function

			Private Function CreateIEquatableEqualsMethod(ByVal iEquatableEquals As MethodSymbol) As MethodSymbol
				Dim synthesizedSimpleMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSimpleMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSimpleMethodSymbol(Me, "Equals", Me.Manager.System_Boolean, Nothing, iEquatableEquals, True)
				synthesizedSimpleMethodSymbol.SetParameters(ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSimpleSymbol(synthesizedSimpleMethodSymbol, Me, 0, "val")))
				Return synthesizedSimpleMethodSymbol
			End Function

			Private Function CreateToStringMethod() As MethodSymbol
				Dim synthesizedSimpleMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSimpleMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedSimpleMethodSymbol(Me, "ToString", Me.Manager.System_String, Me.Manager.System_Object__ToString, Nothing, False)
				synthesizedSimpleMethodSymbol.SetParameters(ImmutableArray(Of ParameterSymbol).Empty)
				Return synthesizedSimpleMethodSymbol
			End Function

			Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
				Return Me._members
			End Function

			Friend Overrides Function InternalSubstituteTypeParameters(ByVal substitution As TypeSubstitution) As TypeWithModifiers
				Dim typeWithModifier As TypeWithModifiers
				Dim anonymousTypeDescriptor As Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeDescriptor = New Microsoft.CodeAnalysis.VisualBasic.Symbols.AnonymousTypeDescriptor()
				typeWithModifier = If(Me.TypeDescriptor.SubstituteTypeParametersIfNeeded(substitution, anonymousTypeDescriptor), New TypeWithModifiers(Me.Manager.ConstructAnonymousTypeSymbol(anonymousTypeDescriptor)), New TypeWithModifiers(Me))
				Return typeWithModifier
			End Function

			Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
				Return Me.Manager.System_Object
			End Function

			Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
				Return Me._interfaces
			End Function

			Public Overrides Function MapToImplementationSymbol() As NamedTypeSymbol
				Return Me.Manager.ConstructAnonymousTypeImplementationSymbol(Me)
			End Function
		End Class

		Private NotInheritable Class AnonymousTypeTemplateSymbol
			Inherits AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol
			Private ReadOnly _properties As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertySymbol)

			Private ReadOnly _members As ImmutableArray(Of Symbol)

			Private ReadOnly _interfaces As ImmutableArray(Of NamedTypeSymbol)

			Friend ReadOnly HasAtLeastOneKeyField As Boolean

			Friend Overrides ReadOnly Property GeneratedNamePrefix As String
				Get
					Return "VB$AnonymousType_"
				End Get
			End Property

			Friend Overrides ReadOnly Property IsInterface As Boolean
				Get
					Return False
				End Get
			End Property

			Public ReadOnly Property Properties As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertySymbol)
				Get
					Return Me._properties
				End Get
			End Property

			Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
				Get
					Return Microsoft.CodeAnalysis.TypeKind.[Class]
				End Get
			End Property

			Public Sub New(ByVal manager As AnonymousTypeManager, ByVal typeDescr As AnonymousTypeDescriptor)
				MyBase.New(manager, typeDescr)
				Dim length As Integer = typeDescr.Fields.Length
				Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				Dim symbols As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
				Dim anonymousTypePropertySymbolArray(length - 1 + 1 - 1) As AnonymousTypeManager.AnonymousTypePropertySymbol
				Me.HasAtLeastOneKeyField = False
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					Dim item As AnonymousTypeField = typeDescr.Fields(num1)
					If (item.IsKey) Then
						Me.HasAtLeastOneKeyField = True
					End If
					Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = Me.TypeParameters
					Dim anonymousTypePropertySymbol As AnonymousTypeManager.AnonymousTypePropertySymbol = New AnonymousTypeManager.AnonymousTypePropertySymbol(Me, item, num1, typeParameters(num1))
					anonymousTypePropertySymbolArray(num1) = anonymousTypePropertySymbol
					symbols.Add(anonymousTypePropertySymbol)
					instance.Add(anonymousTypePropertySymbol.GetMethod)
					If (anonymousTypePropertySymbol.SetMethod IsNot Nothing) Then
						instance.Add(anonymousTypePropertySymbol.SetMethod)
					End If
					symbols.Add(anonymousTypePropertySymbol.AssociatedField)
					num1 = num1 + 1
				Loop While num1 <= num
				Me._properties = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of AnonymousTypeManager.AnonymousTypePropertySymbol)(anonymousTypePropertySymbolArray)
				instance.Add(New AnonymousTypeManager.AnonymousTypeConstructorSymbol(Me))
				instance.Add(New AnonymousTypeManager.AnonymousTypeToStringMethodSymbol(Me))
				If (Not Me.HasAtLeastOneKeyField OrElse Me.Manager.System_IEquatable_T_Equals Is Nothing) Then
					Me._interfaces = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Empty
				Else
					instance.Add(New AnonymousTypeManager.AnonymousTypeGetHashCodeMethodSymbol(Me))
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Manager.System_IEquatable_T.Construct(ImmutableArray.Create(Of TypeSymbol)(Me))
					Me._interfaces = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(namedTypeSymbol)
					Dim memberForDefinition As Symbol = DirectCast(namedTypeSymbol, SubstitutedNamedType).GetMemberForDefinition(Me.Manager.System_IEquatable_T_Equals)
					Dim anonymousTypeIEquatableEqualsMethodSymbol As MethodSymbol = New AnonymousTypeManager.AnonymousType_IEquatable_EqualsMethodSymbol(Me, DirectCast(memberForDefinition, MethodSymbol))
					instance.Add(anonymousTypeIEquatableEqualsMethodSymbol)
					instance.Add(New AnonymousTypeManager.AnonymousTypeEqualsMethodSymbol(Me, anonymousTypeIEquatableEqualsMethodSymbol))
				End If
				instance.AddRange(symbols)
				symbols.Free()
				Me._members = instance.ToImmutableAndFree()
			End Sub

			Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
				MyBase.AddSynthesizedAttributes(compilationState, attributes)
				Dim compilation As VisualBasicCompilation = Me.Manager.Compilation
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
				Symbol.AddSynthesizedAttribute(attributes, Me.SynthesizeDebuggerDisplayAttribute())
			End Sub

			Friend Overrides Function GetAnonymousTypeKey() As AnonymousTypeKey
				Dim anonymousTypeKeyField As Func(Of AnonymousTypeManager.AnonymousTypePropertySymbol, Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField)
				Dim anonymousTypePropertySymbols As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertySymbol) = Me._properties
				If (AnonymousTypeManager.AnonymousTypeTemplateSymbol._Closure$__.$I5-0 Is Nothing) Then
					anonymousTypeKeyField = Function(p As AnonymousTypeManager.AnonymousTypePropertySymbol) New Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField(p.Name, p.IsReadOnly, True)
					AnonymousTypeManager.AnonymousTypeTemplateSymbol._Closure$__.$I5-0 = anonymousTypeKeyField
				Else
					anonymousTypeKeyField = AnonymousTypeManager.AnonymousTypeTemplateSymbol._Closure$__.$I5-0
				End If
				Return New AnonymousTypeKey(Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of AnonymousTypeManager.AnonymousTypePropertySymbol, Microsoft.CodeAnalysis.Emit.AnonymousTypeKeyField)(anonymousTypePropertySymbols, anonymousTypeKeyField), False)
			End Function

			Friend Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
				Return New AnonymousTypeManager.AnonymousTypeTemplateSymbol.VB$StateMachine_11_GetFieldsToEmit(-2) With
				{
					.$VB$Me = Me
				}
			End Function

			Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
				Return Me._members
			End Function

			Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
				Return Me.Manager.System_Object
			End Function

			Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
				Return Me._interfaces
			End Function

			Private Function SynthesizeDebuggerDisplayAttribute() As SynthesizedAttributeData
				Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
				Dim builder As StringBuilder = instance.Builder
				Dim properties As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertySymbol) = Me.Properties
				Dim num As Integer = Math.Min(properties.Length, 4)
				Dim num1 As Integer = num - 1
				Dim num2 As Integer = 0
				Do
					properties = Me.Properties
					Dim name As String = properties(num2).Name
					If (num2 > 0) Then
						builder.Append(", ")
					End If
					builder.Append(name)
					builder.Append("={")
					builder.Append(name)
					builder.Append("}")
					num2 = num2 + 1
				Loop While num2 <= num1
				If (Me.Properties.Length > num) Then
					builder.Append(", ...")
				End If
				Return Me.Manager.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__ctor, ImmutableArray.Create(Of TypedConstant)(New TypedConstant(Me.Manager.System_String, TypedConstantKind.Primitive, instance.ToStringAndFree())), New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))(), False)
			End Function
		End Class

		Private NotInheritable Class AnonymousTypeToStringMethodSymbol
			Inherits SynthesizedRegularMethodBase
			Private ReadOnly Property AnonymousType As AnonymousTypeManager.AnonymousTypeTemplateSymbol
				Get
					Return DirectCast(Me.m_containingType, AnonymousTypeManager.AnonymousTypeTemplateSymbol)
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
				Get
					Return Accessibility.[Public]
				End Get
			End Property

			Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsOverrides As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property IsSub As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
				Get
					Return Me.AnonymousType.Manager.System_Object__ToString
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me.AnonymousType.Manager.System_String
				End Get
			End Property

			Public Sub New(ByVal container As AnonymousTypeManager.AnonymousTypeTemplateSymbol)
				MyBase.New(VisualBasicSyntaxTree.Dummy.GetRoot(New CancellationToken()), container, "ToString", False)
			End Sub

			Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
				MyBase.AddSynthesizedAttributes(compilationState, attributes)
				Dim compilation As VisualBasicCompilation = DirectCast(MyBase.ContainingType, AnonymousTypeManager.AnonymousTypeTemplateSymbol).Manager.Compilation
				Symbol.AddSynthesizedAttribute(attributes, compilation.SynthesizeDebuggerHiddenAttribute())
			End Sub

			Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
				methodBodyBinder = Nothing
				Dim syntax As SyntaxNode = MyBase.Syntax
				Dim systemObject As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.AnonymousType.Manager.System_Object
				Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.ReturnType
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.AnonymousType.Manager.Compilation.CreateArrayTypeSymbol(systemObject, 1)
				Dim properties As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertySymbol) = Me.AnonymousType.Properties
				Dim length As Integer = properties.Length
				Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = (New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, Me.AnonymousType)).MakeCompilerGenerated()
				Dim boundExpressionArray(length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
				instance.Builder.Append("{{ ")
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					properties = Me.AnonymousType.Properties
					Dim item As AnonymousTypeManager.AnonymousTypePropertySymbol = properties(num1)
					instance.Builder.AppendFormat(If(num1 = 0, "{0} = {{{1}}}", ", {0} = {{{1}}}"), CObj(item.MetadataName), num1)
					boundExpressionArray(num1) = (New BoundDirectCast(syntax, (New BoundFieldAccess(syntax, boundMeReference, item.AssociatedField, False, item.Type, False)).MakeCompilerGenerated(), ConversionKind.WideningTypeParameter, systemObject, False)).MakeCompilerGenerated()
					num1 = num1 + 1
				Loop While num1 <= num
				instance.Builder.Append(" }}")
				Dim stringAndFree As String = instance.ToStringAndFree()
				Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = (New Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization(syntax, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray), typeSymbol, False)).MakeCompilerGenerated()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundArrayCreation(syntax, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)((New BoundLiteral(syntax, ConstantValue.Create(length), Me.AnonymousType.Manager.System_Int32)).MakeCompilerGenerated()), boundArrayInitialization, typeSymbol, False)).MakeCompilerGenerated()
				Dim systemString_FormatIFormatProvider As MethodSymbol = Me.AnonymousType.Manager.System_String__Format_IFormatProvider
				Dim [nothing] As ConstantValue = ConstantValue.[Nothing]
				Dim parameters As ImmutableArray(Of ParameterSymbol) = systemString_FormatIFormatProvider.Parameters
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)((New BoundLiteral(syntax, [nothing], parameters(0).Type)).MakeCompilerGenerated(), (New BoundLiteral(syntax, ConstantValue.Create(stringAndFree), returnType)).MakeCompilerGenerated(), boundExpression)
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundCall(syntax, systemString_FormatIFormatProvider, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)).MakeCompilerGenerated()
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				Return New BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)((New BoundReturnStatement(syntax, boundExpression1, Nothing, Nothing, False)).MakeCompilerGenerated()), False)
			End Function
		End Class

		Friend NotInheritable Class NameAndIndex
			Public ReadOnly Name As String

			Public ReadOnly Index As Integer

			Public Sub New(ByVal name As String, ByVal index As Integer)
				MyBase.New()
				Me.Name = name
				Me.Index = index
			End Sub
		End Class

		Private NotInheritable Class NonGenericAnonymousDelegateSymbol
			Inherits AnonymousTypeManager.AnonymousDelegateTemplateSymbol
			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return ImmutableArray.Create(Of Location)(Me.TypeDescr.Location)
				End Get
			End Property

			Public Sub New(ByVal manager As AnonymousTypeManager, ByVal typeDescr As AnonymousTypeDescriptor)
				MyBase.New(manager, typeDescr)
			End Sub

			Public Overrides Function Equals(ByVal obj As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
				Dim flag As Boolean
				If (obj <> Me) Then
					Dim nonGenericAnonymousDelegateSymbol As AnonymousTypeManager.NonGenericAnonymousDelegateSymbol = TryCast(obj, AnonymousTypeManager.NonGenericAnonymousDelegateSymbol)
					flag = If(nonGenericAnonymousDelegateSymbol Is Nothing, False, nonGenericAnonymousDelegateSymbol.Manager = Me.Manager)
				Else
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Me.Manager.GetHashCode()
			End Function
		End Class
	End Class
End Namespace
Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class EventSymbol
		Inherits Symbol
		Implements IEventDefinition, IEventSymbol
		Friend ReadOnly Property AdaptedEventSymbol As EventSymbol
			Get
				Return Me
			End Get
		End Property

		Public MustOverride ReadOnly Property AddMethod As MethodSymbol

		Friend MustOverride ReadOnly Property AssociatedField As FieldSymbol

		Friend Overridable ReadOnly Property DelegateParameters As ImmutableArray(Of ParameterSymbol)
			Get
				Dim empty As ImmutableArray(Of ParameterSymbol)
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.DelegateInvokeMethod()
				If (methodSymbol Is Nothing) Then
					empty = ImmutableArray(Of ParameterSymbol).Empty
				Else
					empty = methodSymbol.Parameters
				End If
				Return empty
			End Get
		End Property

		Friend ReadOnly Property DelegateReturnType As TypeSymbol
			Get
				Dim specialType As TypeSymbol
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.DelegateInvokeMethod()
				If (methodSymbol Is Nothing) Then
					specialType = Me.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void)
				Else
					specialType = methodSymbol.ReturnType
				End If
				Return specialType
			End Get
		End Property

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				Return Me.ContainingSymbol.EmbeddedSymbolKind
			End Get
		End Property

		Public MustOverride ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of EventSymbol)

		Friend ReadOnly Property HasAssociatedField As Boolean
			Get
				Return CObj(Me.AssociatedField) <> CObj(Nothing)
			End Get
		End Property

		Friend Overridable ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend MustOverride ReadOnly Property HasSpecialName As Boolean

		Public NotOverridable Overrides ReadOnly Property HasUnsupportedMetadata As Boolean
			Get
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = Me.GetUseSiteInfo().DiagnosticInfo
				If (diagnosticInfo Is Nothing) Then
					Return False
				End If
				If (diagnosticInfo.Code = 30649) Then
					Return True
				End If
				Return diagnosticInfo.Code = 37223
			End Get
		End Property

		Protected Overrides ReadOnly Property HighestPriorityUseSiteError As Integer
			Get
				Return 30649
			End Get
		End Property

		ReadOnly Property IEventDefinitionAdder As IMethodReference Implements IEventDefinition.Adder
			Get
				Return Me.AdaptedEventSymbol.AddMethod.GetCciAdapter()
			End Get
		End Property

		ReadOnly Property IEventDefinitionCaller As IMethodReference Implements IEventDefinition.Caller
			Get
				Dim raiseMethod As MethodSymbol = Me.AdaptedEventSymbol.RaiseMethod
				If (raiseMethod IsNot Nothing) Then
					Return raiseMethod.GetCciAdapter()
				End If
				Return Nothing
			End Get
		End Property

		ReadOnly Property IEventDefinitionContainingTypeDefinition As ITypeDefinition
			Get
				Return Me.AdaptedEventSymbol.ContainingType.GetCciAdapter()
			End Get
		End Property

		ReadOnly Property IEventDefinitionIsRuntimeSpecial As Boolean Implements IEventDefinition.IsRuntimeSpecial
			Get
				Return Me.AdaptedEventSymbol.HasRuntimeSpecialName
			End Get
		End Property

		ReadOnly Property IEventDefinitionIsSpecialName As Boolean Implements IEventDefinition.IsSpecialName
			Get
				Return Me.AdaptedEventSymbol.HasSpecialName
			End Get
		End Property

		ReadOnly Property IEventDefinitionName As String
			Get
				Return Me.AdaptedEventSymbol.MetadataName
			End Get
		End Property

		ReadOnly Property IEventDefinitionRemover As IMethodReference Implements IEventDefinition.Remover
			Get
				Return Me.AdaptedEventSymbol.RemoveMethod.GetCciAdapter()
			End Get
		End Property

		ReadOnly Property IEventDefinitionVisibility As TypeMemberVisibility
			Get
				Return PEModuleBuilder.MemberVisibility(Me.AdaptedEventSymbol)
			End Get
		End Property

		ReadOnly Property IEventSymbol_AddMethod As IMethodSymbol Implements IEventSymbol.AddMethod
			Get
				Return Me.AddMethod
			End Get
		End Property

		ReadOnly Property IEventSymbol_ExplicitInterfaceImplementations As ImmutableArray(Of IEventSymbol) Implements IEventSymbol.ExplicitInterfaceImplementations
			Get
				Return StaticCast(Of IEventSymbol).From(Of EventSymbol)(Me.ExplicitInterfaceImplementations)
			End Get
		End Property

		ReadOnly Property IEventSymbol_NullableAnnotation As Microsoft.CodeAnalysis.NullableAnnotation Implements IEventSymbol.NullableAnnotation
			Get
				Return Microsoft.CodeAnalysis.NullableAnnotation.None
			End Get
		End Property

		ReadOnly Property IEventSymbol_OriginalDefinition As IEventSymbol Implements IEventSymbol.OriginalDefinition
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		ReadOnly Property IEventSymbol_OverriddenEvent As IEventSymbol Implements IEventSymbol.OverriddenEvent
			Get
				Return Me.OverriddenEvent
			End Get
		End Property

		ReadOnly Property IEventSymbol_RaiseMethod As IMethodSymbol Implements IEventSymbol.RaiseMethod
			Get
				Return Me.RaiseMethod
			End Get
		End Property

		ReadOnly Property IEventSymbol_RemoveMethod As IMethodSymbol Implements IEventSymbol.RemoveMethod
			Get
				Return Me.RemoveMethod
			End Get
		End Property

		ReadOnly Property IEventSymbol_Type As ITypeSymbol Implements IEventSymbol.Type
			Get
				Return Me.Type
			End Get
		End Property

		Friend Overridable ReadOnly Property IsDirectlyExcludedFromCodeCoverage As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overridable ReadOnly Property IsExplicitInterfaceImplementation As Boolean
			Get
				Return System.Linq.ImmutableArrayExtensions.Any(Of EventSymbol)(Me.ExplicitInterfaceImplementations)
			End Get
		End Property

		Public Overridable ReadOnly Property IsTupleEvent As Boolean
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property IsWindowsRuntimeEvent As Boolean Implements IEventSymbol.IsWindowsRuntimeEvent

		Public NotOverridable Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.[Event]
			End Get
		End Property

		Public Shadows Overridable ReadOnly Property OriginalDefinition As EventSymbol
			Get
				Return Me
			End Get
		End Property

		Protected NotOverridable Overrides ReadOnly Property OriginalSymbolDefinition As Symbol
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		Public ReadOnly Property OverriddenEvent As EventSymbol
			Get
				Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol
				If (Not Me.IsOverrides) Then
					eventSymbol = Nothing
				Else
					eventSymbol = If(Not MyBase.IsDefinition, OverriddenMembersResult(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol).GetOverriddenMember(Me, Me.OriginalDefinition.OverriddenEvent), Me.OverriddenOrHiddenMembers.OverriddenMember)
				End If
				Return eventSymbol
			End Get
		End Property

		Friend Overridable ReadOnly Property OverriddenOrHiddenMembers As OverriddenMembersResult(Of EventSymbol)
			Get
				Return OverrideHidingHelper(Of EventSymbol).MakeOverriddenMembers(Me)
			End Get
		End Property

		Public MustOverride ReadOnly Property RaiseMethod As MethodSymbol

		Public MustOverride ReadOnly Property RemoveMethod As MethodSymbol

		Public Overridable ReadOnly Property TupleUnderlyingEvent As EventSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property Type As TypeSymbol

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal argument As TArgument) As TResult
			Return visitor.VisitEvent(Me, argument)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitEvent(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitEvent(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitEvent(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitEvent(Me)
		End Function

		Friend Function CalculateUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = MyBase.MergeUseSiteInfo(New UseSiteInfo(Of AssemblySymbol)(MyBase.PrimaryDependency), MyBase.DeriveUseSiteInfoFromType(Me.Type))
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo.DiagnosticInfo
			If (diagnosticInfo IsNot Nothing) Then
				Select Case diagnosticInfo.Code
					Case 30649
						If (Not diagnosticInfo.Arguments(0).Equals([String].Empty)) Then
							Exit Select
						End If
						useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(Me) }))
						Exit Select
					Case 30652
						useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_UnreferencedAssemblyEvent3, New [Object]() { diagnosticInfo.Arguments(0), Me }))
						Exit Select
					Case 30653
						useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_UnreferencedModuleEvent3, New [Object]() { diagnosticInfo.Arguments(0), Me }))
						Exit Select
				End Select
			ElseIf (Me.ContainingModule.HasUnifiedReferences) Then
				Dim typeSymbols As HashSet(Of TypeSymbol) = Nothing
				diagnosticInfo = Me.Type.GetUnificationUseSiteDiagnosticRecursive(Me, typeSymbols)
				If (diagnosticInfo IsNot Nothing) Then
					useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(diagnosticInfo)
				End If
			End If
			Return useSiteInfo
		End Function

		Private Function DelegateInvokeMethod() As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim type As NamedTypeSymbol = TryCast(Me.Type, NamedTypeSymbol)
			If (type Is Nothing OrElse type.TypeKind <> TypeKind.[Delegate]) Then
				methodSymbol = Nothing
			Else
				methodSymbol = type.DelegateInvokeMethod
			End If
			Return methodSymbol
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
			If (eventSymbol Is Nothing) Then
				flag = False
			ElseIf (CObj(Me) <> CObj(eventSymbol)) Then
				flag = If(Not TypeSymbol.Equals(Me.ContainingType, eventSymbol.ContainingType, TypeCompareKind.ConsiderEverything), False, CObj(Me.OriginalDefinition) = CObj(eventSymbol.OriginalDefinition))
			Else
				flag = True
			End If
			Return flag
		End Function

		Friend Shadows Function GetCciAdapter() As EventSymbol
			Return Me
		End Function

		Public Function GetFieldAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Dim associatedField As FieldSymbol = Me.AssociatedField
			If (associatedField Is Nothing) Then
				Return ImmutableArray(Of VisualBasicAttributeData).Empty
			End If
			Return associatedField.GetAttributes()
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim num As Integer = Hash.Combine(Of NamedTypeSymbol)(Me.ContainingType, 1)
			Return Hash.Combine(Of String)(Me.Name, num)
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			useSiteInfo = If(Not MyBase.IsDefinition, Me.OriginalDefinition.GetUseSiteInfo(), New UseSiteInfo(Of AssemblySymbol)(MyBase.PrimaryDependency))
			Return useSiteInfo
		End Function

		Private Function IEventDefinitionAccessors(ByVal context As EmitContext) As IEnumerable(Of IMethodReference) Implements IEventDefinition.GetAccessors
			Return New EventSymbol.VB$StateMachine_0_IEventDefinitionAccessors(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Private Function IEventDefinitionGetType(ByVal context As EmitContext) As ITypeReference Implements IEventDefinition.[GetType]
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.AdaptedEventSymbol.Type, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
		End Function

		Friend Overrides Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Return Me
		End Function

		Friend Overrides Sub IReferenceDispatch(ByVal visitor As MetadataVisitor)
			visitor.Visit(Me)
		End Sub

		Private Function ITypeMemberReferenceGetContainingType(ByVal context As EmitContext) As ITypeReference
			Return Me.AdaptedEventSymbol.ContainingType.GetCciAdapter()
		End Function
	End Class
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend NotInheritable Class RetargetingEventSymbol
		Inherits EventSymbol
		Private ReadOnly _retargetingModule As RetargetingModuleSymbol

		Private ReadOnly _underlyingEvent As EventSymbol

		Private _lazyCustomModifiers As ImmutableArray(Of CustomModifier)

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyExplicitInterfaceImplementations As ImmutableArray(Of EventSymbol)

		Private _lazyCachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

		Public Overrides ReadOnly Property AddMethod As MethodSymbol
			Get
				If (Me._underlyingEvent.AddMethod Is Nothing) Then
					Return Nothing
				End If
				Return Me.RetargetingTranslator.Retarget(Me._underlyingEvent.AddMethod)
			End Get
		End Property

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				If (Me._underlyingEvent.AssociatedField Is Nothing) Then
					Return Nothing
				End If
				Return Me.RetargetingTranslator.Retarget(Me._underlyingEvent.AssociatedField)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingEvent.ContainingSymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._underlyingEvent.DeclaredAccessibility
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._underlyingEvent.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of EventSymbol)
			Get
				If (Me._lazyExplicitInterfaceImplementations.IsDefault) Then
					Dim eventSymbols As ImmutableArray(Of EventSymbol) = Me.RetargetExplicitInterfaceImplementations()
					Dim eventSymbols1 As ImmutableArray(Of EventSymbol) = New ImmutableArray(Of EventSymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of EventSymbol)(Me._lazyExplicitInterfaceImplementations, eventSymbols, eventSymbols1)
				End If
				Return Me._lazyExplicitInterfaceImplementations
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return Me._underlyingEvent.HasRuntimeSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._underlyingEvent.HasSpecialName
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return Me._underlyingEvent.IsMustOverride
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return Me._underlyingEvent.IsNotOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return Me._underlyingEvent.IsOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return Me._underlyingEvent.IsOverrides
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me._underlyingEvent.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property IsWindowsRuntimeEvent As Boolean
			Get
				Return Me._underlyingEvent.IsWindowsRuntimeEvent
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingEvent.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._underlyingEvent.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingEvent.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me._underlyingEvent.ObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property RaiseMethod As MethodSymbol
			Get
				If (Me._underlyingEvent.RaiseMethod Is Nothing) Then
					Return Nothing
				End If
				Return Me.RetargetingTranslator.Retarget(Me._underlyingEvent.RaiseMethod)
			End Get
		End Property

		Public Overrides ReadOnly Property RemoveMethod As MethodSymbol
			Get
				If (Me._underlyingEvent.RemoveMethod Is Nothing) Then
					Return Nothing
				End If
				Return Me.RetargetingTranslator.Retarget(Me._underlyingEvent.RemoveMethod)
			End Get
		End Property

		Public ReadOnly Property RetargetingModule As RetargetingModuleSymbol
			Get
				Return Me._retargetingModule
			End Get
		End Property

		Private ReadOnly Property RetargetingTranslator As RetargetingModuleSymbol.RetargetingSymbolTranslator
			Get
				Return Me._retargetingModule.RetargetingTranslator
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingEvent.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
			End Get
		End Property

		Public ReadOnly Property UnderlyingEvent As EventSymbol
			Get
				Return Me._underlyingEvent
			End Get
		End Property

		Public Sub New(ByVal retargetingModule As RetargetingModuleSymbol, ByVal underlyingEvent As EventSymbol)
			MyBase.New()
			Me._lazyCachedUseSiteInfo = CachedUseSiteInfo(Of AssemblySymbol).Uninitialized
			If (TypeOf underlyingEvent Is RetargetingEventSymbol) Then
				Throw New ArgumentException()
			End If
			Me._retargetingModule = retargetingModule
			Me._underlyingEvent = underlyingEvent
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.GetRetargetedAttributes(Me._underlyingEvent, Me._lazyCustomAttributes, False)
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.RetargetAttributes(Me._underlyingEvent.GetCustomAttributesToEmit(compilationState))
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingEvent.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim primaryDependency As AssemblySymbol = MyBase.PrimaryDependency
			If (Not Me._lazyCachedUseSiteInfo.IsInitialized) Then
				Me._lazyCachedUseSiteInfo.Initialize(primaryDependency, MyBase.CalculateUseSiteInfo())
			End If
			Return Me._lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency)
		End Function

		Private Function RetargetExplicitInterfaceImplementations() As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
			Dim explicitInterfaceImplementations As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol) = Me.UnderlyingEvent.ExplicitInterfaceImplementations
			If (Not explicitInterfaceImplementations.IsEmpty) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol).GetInstance()
				Dim length As Integer = explicitInterfaceImplementations.Length - 1
				Dim num As Integer = 0
				Do
					Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = Me.RetargetingModule.RetargetingTranslator.RetargetImplementedEvent(explicitInterfaceImplementations(num))
					If (eventSymbol IsNot Nothing) Then
						instance.Add(eventSymbol)
					End If
					num = num + 1
				Loop While num <= length
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = explicitInterfaceImplementations
			End If
			Return immutableAndFree
		End Function
	End Class
End Namespace
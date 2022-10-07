Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class PEEventSymbol
		Inherits EventSymbol
		Private ReadOnly _name As String

		Private ReadOnly _flags As EventAttributes

		Private ReadOnly _containingType As PENamedTypeSymbol

		Private ReadOnly _handle As EventDefinitionHandle

		Private ReadOnly _eventType As TypeSymbol

		Private ReadOnly _addMethod As PEMethodSymbol

		Private ReadOnly _removeMethod As PEMethodSymbol

		Private ReadOnly _raiseMethod As PEMethodSymbol

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyDocComment As Tuple(Of CultureInfo, String)

		Private _lazyCachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

		Private _lazyObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData

		Private Const s_unsetAccessibility As Integer = -1

		Private _lazyDeclaredAccessibility As Integer

		Public Overrides ReadOnly Property AddMethod As MethodSymbol
			Get
				Return Me._addMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
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
				If (Me._lazyDeclaredAccessibility = -1) Then
					Dim declaredAccessibilityFromAccessors As Accessibility = PEPropertyOrEventHelpers.GetDeclaredAccessibilityFromAccessors(Me.AddMethod, Me.RemoveMethod)
					Interlocked.CompareExchange(Me._lazyDeclaredAccessibility, CInt(declaredAccessibilityFromAccessors), -1)
				End If
				Return DirectCast(Me._lazyDeclaredAccessibility, Accessibility)
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend ReadOnly Property EventFlags As EventAttributes
			Get
				Return Me._flags
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of EventSymbol)
			Get
				Dim immutableAndFree As ImmutableArray(Of EventSymbol)
				Dim enumerator As IEnumerator(Of EventSymbol) = Nothing
				If (Me.AddMethod.ExplicitInterfaceImplementations.Length <> 0 OrElse Me.RemoveMethod.ExplicitInterfaceImplementations.Length <> 0) Then
					Dim eventsForExplicitlyImplementedAccessor As ISet(Of EventSymbol) = PEPropertyOrEventHelpers.GetEventsForExplicitlyImplementedAccessor(Me.AddMethod)
					eventsForExplicitlyImplementedAccessor.IntersectWith(PEPropertyOrEventHelpers.GetEventsForExplicitlyImplementedAccessor(Me.RemoveMethod))
					Using instance As ArrayBuilder(Of EventSymbol) = ArrayBuilder(Of EventSymbol).GetInstance()
						enumerator = eventsForExplicitlyImplementedAccessor.GetEnumerator()
						While enumerator.MoveNext()
							instance.Add(enumerator.Current)
						End While
					End Using
					immutableAndFree = instance.ToImmutableAndFree()
				Else
					immutableAndFree = ImmutableArray(Of EventSymbol).Empty
				End If
				Return immutableAndFree
			End Get
		End Property

		Friend ReadOnly Property Handle As EventDefinitionHandle
			Get
				Return Me._handle
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return (Me._flags And EventAttributes.ReservedMask) <> EventAttributes.None
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return (Me._flags And EventAttributes.SpecialName) <> EventAttributes.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Dim addMethod As MethodSymbol = Me.AddMethod
				If (addMethod Is Nothing) Then
					Return False
				End If
				Return addMethod.IsMustOverride
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Dim addMethod As MethodSymbol = Me.AddMethod
				If (addMethod Is Nothing) Then
					Return False
				End If
				Return addMethod.IsNotOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Dim addMethod As MethodSymbol = Me.AddMethod
				If (addMethod Is Nothing) Then
					Return False
				End If
				Return addMethod.IsOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Dim addMethod As MethodSymbol = Me.AddMethod
				If (addMethod Is Nothing) Then
					Return False
				End If
				Return addMethod.IsOverrides
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Dim addMethod As MethodSymbol = Me.AddMethod
				If (addMethod Is Nothing) Then
					Return True
				End If
				Return addMethod.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property IsWindowsRuntimeEvent As Boolean
			Get
				Dim eventRegistrationTokenType As NamedTypeSymbol = DirectCast(Me.ContainingModule, PEModuleSymbol).GetEventRegistrationTokenType()
				If (Not TypeSymbol.Equals(Me._addMethod.ReturnType, eventRegistrationTokenType, TypeCompareKind.ConsiderEverything) OrElse Me._addMethod.ParameterCount <> 1 OrElse Me._removeMethod.ParameterCount <> 1) Then
					Return False
				End If
				Dim parameters As ImmutableArray(Of ParameterSymbol) = Me._removeMethod.Parameters
				Return TypeSymbol.Equals(parameters(0).Type, eventRegistrationTokenType, TypeCompareKind.ConsiderEverything)
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._containingType.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				ObsoleteAttributeHelpers.InitializeObsoleteDataFromMetadata(Me._lazyObsoleteAttributeData, Me._handle, DirectCast(Me.ContainingModule, PEModuleSymbol))
				Return Me._lazyObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property RaiseMethod As MethodSymbol
			Get
				Return Me._raiseMethod
			End Get
		End Property

		Public Overrides ReadOnly Property RemoveMethod As MethodSymbol
			Get
				Return Me._removeMethod
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._eventType
			End Get
		End Property

		Friend Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingType As PENamedTypeSymbol, ByVal handle As EventDefinitionHandle, ByVal addMethod As PEMethodSymbol, ByVal removeMethod As PEMethodSymbol, ByVal raiseMethod As PEMethodSymbol)
			MyBase.New()
			Dim entityHandle As System.Reflection.Metadata.EntityHandle = New System.Reflection.Metadata.EntityHandle()
			Me._lazyCachedUseSiteInfo = CachedUseSiteInfo(Of AssemblySymbol).Uninitialized
			Me._lazyObsoleteAttributeData = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized
			Me._lazyDeclaredAccessibility = -1
			Me._containingType = containingType
			Dim [module] As PEModule = moduleSymbol.[Module]
			Try
				[module].GetEventDefPropsOrThrow(handle, Me._name, Me._flags, entityHandle)
			Catch badImageFormatException1 As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException1)
				Dim badImageFormatException As System.BadImageFormatException = badImageFormatException1
				If (Me._name Is Nothing) Then
					Me._name = [String].Empty
				End If
				Dim objArray() As [Object] = { Me }
				Me._lazyCachedUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedEvent1, objArray))
				If (entityHandle.IsNil) Then
					Me._eventType = New UnsupportedMetadataTypeSymbol(badImageFormatException)
				End If
				ProjectData.ClearProjectError()
			End Try
			Me._addMethod = addMethod
			Me._removeMethod = removeMethod
			Me._raiseMethod = raiseMethod
			Me._handle = handle
			If (Me._eventType Is Nothing) Then
				Me._eventType = (New MetadataDecoder(moduleSymbol, containingType)).GetTypeOfToken(entityHandle)
				Me._eventType = TupleTypeDecoder.DecodeTupleTypesIfApplicable(Me._eventType, handle, moduleSymbol)
			End If
			If (Me._addMethod IsNot Nothing) Then
				Me._addMethod.SetAssociatedEvent(Me, MethodKind.EventAdd)
			End If
			If (Me._removeMethod IsNot Nothing) Then
				Me._removeMethod.SetAssociatedEvent(Me, MethodKind.EventRemove)
			End If
			If (Me._raiseMethod IsNot Nothing) Then
				Me._raiseMethod.SetAssociatedEvent(Me, MethodKind.EventRaise)
			End If
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			If (Me._lazyCustomAttributes.IsDefault) Then
				DirectCast(Me.ContainingModule, PEModuleSymbol).LoadCustomAttributes(Me._handle, Me._lazyCustomAttributes)
			End If
			Return Me._lazyCustomAttributes
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return DirectCast(Me.GetAttributes(), IEnumerable(Of VisualBasicAttributeData))
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return PEDocumentationCommentUtils.GetDocumentationComment(Me, Me._containingType.ContainingPEModule, preferredCulture, cancellationToken, Me._lazyDocComment)
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim primaryDependency As AssemblySymbol = MyBase.PrimaryDependency
			If (Not Me._lazyCachedUseSiteInfo.IsInitialized) Then
				Me._lazyCachedUseSiteInfo.Initialize(primaryDependency, MyBase.CalculateUseSiteInfo())
			End If
			Return Me._lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency)
		End Function
	End Class
End Namespace
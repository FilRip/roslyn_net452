Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection.Metadata
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class PEAttributeData
		Inherits VisualBasicAttributeData
		Private ReadOnly _decoder As MetadataDecoder

		Private ReadOnly _handle As CustomAttributeHandle

		Private _attributeClass As NamedTypeSymbol

		Private _attributeConstructor As MethodSymbol

		Private _lazyConstructorArguments As TypedConstant()

		Private _lazyNamedArguments As KeyValuePair(Of String, TypedConstant)()

		Private _lazyHasErrors As ThreeState

		Public Overrides ReadOnly Property ApplicationSyntaxReference As SyntaxReference
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property AttributeClass As NamedTypeSymbol
			Get
				If (Me._attributeClass Is Nothing) Then
					Me.EnsureClassAndConstructorSymbols()
				End If
				Return Me._attributeClass
			End Get
		End Property

		Public Overrides ReadOnly Property AttributeConstructor As MethodSymbol
			Get
				If (Me._attributeConstructor Is Nothing) Then
					Me.EnsureClassAndConstructorSymbols()
				End If
				Return Me._attributeConstructor
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonConstructorArguments As ImmutableArray(Of TypedConstant)
			Get
				If (Me._lazyConstructorArguments Is Nothing) Then
					Me.EnsureLazyMembersAreLoaded()
				End If
				Return ImmutableArrayExtensions.AsImmutableOrNull(Of TypedConstant)(Me._lazyConstructorArguments)
			End Get
		End Property

		Protected Overrides ReadOnly Property CommonNamedArguments As ImmutableArray(Of KeyValuePair(Of String, TypedConstant))
			Get
				If (Me._lazyNamedArguments Is Nothing) Then
					Me.EnsureLazyMembersAreLoaded()
				End If
				Return ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of String, TypedConstant))(Me._lazyNamedArguments)
			End Get
		End Property

		Friend Overrides ReadOnly Property HasErrors As Boolean
			Get
				If (Me._lazyHasErrors = ThreeState.Unknown) Then
					Me.EnsureClassAndConstructorSymbols()
					Me.EnsureLazyMembersAreLoaded()
					If (Me._lazyHasErrors = ThreeState.Unknown) Then
						Me._lazyHasErrors = ThreeState.[False]
					End If
				End If
				Return Me._lazyHasErrors.Value()
			End Get
		End Property

		Friend Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal handle As CustomAttributeHandle)
			MyBase.New()
			Me._lazyHasErrors = ThreeState.Unknown
			Me._decoder = New MetadataDecoder(moduleSymbol)
			Me._handle = handle
		End Sub

		Private Sub EnsureClassAndConstructorSymbols()
			If (Me._attributeClass Is Nothing) Then
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				If (Not Me._decoder.GetCustomAttribute(Me._handle, typeSymbol, methodSymbol) OrElse typeSymbol Is Nothing) Then
					Interlocked.CompareExchange(Of NamedTypeSymbol)(Me._attributeClass, ErrorTypeSymbol.UnknownResultType, Nothing)
					Me._lazyHasErrors = ThreeState.[True]
					Return
				End If
				If (typeSymbol.IsErrorType() OrElse methodSymbol Is Nothing) Then
					Me._lazyHasErrors = ThreeState.[True]
				End If
				Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(Me._attributeConstructor, methodSymbol, Nothing)
				Interlocked.CompareExchange(Of NamedTypeSymbol)(Me._attributeClass, DirectCast(typeSymbol, NamedTypeSymbol), Nothing)
			End If
		End Sub

		Private Sub EnsureLazyMembersAreLoaded()
			If (Me._lazyConstructorArguments Is Nothing) Then
				Dim typedConstantArray As TypedConstant() = Nothing
				Dim keyValuePairArray As KeyValuePair(Of String, TypedConstant)() = Nothing
				If (Not Me._decoder.GetCustomAttribute(Me._handle, typedConstantArray, keyValuePairArray)) Then
					Me._lazyHasErrors = ThreeState.[True]
				End If
				Interlocked.CompareExchange(Of KeyValuePair(Of String, TypedConstant)())(Me._lazyNamedArguments, keyValuePairArray, Nothing)
				Interlocked.CompareExchange(Of TypedConstant())(Me._lazyConstructorArguments, typedConstantArray, Nothing)
			End If
		End Sub

		Friend Overrides Function GetTargetAttributeSignatureIndex(ByVal targetSymbol As Symbol, ByVal description As AttributeDescription) As Integer
			Return Me._decoder.GetTargetAttributeSignatureIndex(Me._handle, description)
		End Function

		Friend Overrides Function IsTargetAttribute(ByVal namespaceName As String, ByVal typeName As String, Optional ByVal ignoreCase As Boolean = False) As Boolean
			Return Me._decoder.IsTargetAttribute(Me._handle, namespaceName, typeName, ignoreCase)
		End Function
	End Class
End Namespace
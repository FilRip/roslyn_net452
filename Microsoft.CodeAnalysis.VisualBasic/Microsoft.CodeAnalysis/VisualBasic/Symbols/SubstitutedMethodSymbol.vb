Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SubstitutedMethodSymbol
		Inherits MethodSymbol
		Private _propertyOrEventSymbolOpt As Symbol

		Private ReadOnly _lazyOverriddenMethods As OverriddenMembersResult(Of MethodSymbol)

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me.OriginalDefinition.Arity
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Me._propertyOrEventSymbolOpt
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me.OriginalDefinition.CallingConvention
			End Get
		End Property

		Friend Overrides ReadOnly Property CanConstruct As Boolean

		Public Overrides ReadOnly Property ConstructedFrom As MethodSymbol

		Public Overrides ReadOnly Property ContainingSymbol As Symbol

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me.OriginalDefinition.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me.OriginalDefinition.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return ImplementsHelper.SubstituteExplicitInterfaceImplementations(Of MethodSymbol)(Me.OriginalDefinition.ExplicitInterfaceImplementations, Me.TypeSubstitution)
			End Get
		End Property

		Friend Overrides ReadOnly Property FixedTypeParameters As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol))
			Get
				Return Me.OriginalDefinition.FixedTypeParameters
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return Me.OriginalDefinition.GenerateDebugInfo
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return Me.OriginalDefinition.HasDeclarativeSecurity
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me.OriginalDefinition.HasSpecialName
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Return Me.OriginalDefinition.ImplementationAttributes
			End Get
		End Property

		Public Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return Me.OriginalDefinition.IsAsync
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return Me.OriginalDefinition.IsExtensionMethod
			End Get
		End Property

		Public Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				Return Me.OriginalDefinition.IsExternalMethod
			End Get
		End Property

		Public Overrides ReadOnly Property IsGenericMethod As Boolean
			Get
				Return Me.OriginalDefinition.IsGenericMethod
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me.OriginalDefinition.IsImplicitlyDeclared
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsInitOnly As Boolean
			Get
				Return Me.OriginalDefinition.IsInitOnly
			End Get
		End Property

		Public Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return Me.OriginalDefinition.IsIterator
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
			Get
				Return Me.OriginalDefinition.IsMethodKindBasedOnSyntax
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return Me.OriginalDefinition.IsMustOverride
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return Me.OriginalDefinition.IsNotOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return Me.OriginalDefinition.IsOverloads
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return Me.OriginalDefinition.IsOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return Me.OriginalDefinition.IsOverrides
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me.OriginalDefinition.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me.OriginalDefinition.IsSub
			End Get
		End Property

		Public Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return Me.OriginalDefinition.IsVararg
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me.OriginalDefinition.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MayBeReducibleExtensionMethod As Boolean
			Get
				Return Me.OriginalDefinition.MayBeReducibleExtensionMethod
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property MetadataName As String
			Get
				Return Me.OriginalDefinition.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Me.OriginalDefinition.MethodKind
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me.OriginalDefinition.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me.OriginalDefinition.ObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property OriginalDefinition As MethodSymbol

		Friend NotOverridable Overrides ReadOnly Property ParameterCount As Integer
			Get
				Return Me.OriginalDefinition.ParameterCount
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)

		Friend Overrides ReadOnly Property Proximity As Integer
			Get
				Return Me.OriginalDefinition.Proximity
			End Get
		End Property

		Public Overrides ReadOnly Property ReceiverType As TypeSymbol
			Get
				Dim containingType As TypeSymbol
				If (Not Me.OriginalDefinition.IsReducedExtensionMethod) Then
					containingType = Me.ContainingType
				Else
					containingType = Me.OriginalDefinition.ReceiverType
				End If
				Return containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ReducedFrom As MethodSymbol
			Get
				Return Me.OriginalDefinition.ReducedFrom
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.TypeSubstitution.SubstituteCustomModifiers(Me.OriginalDefinition.RefCustomModifiers)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return Me.OriginalDefinition.ReturnsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me.OriginalDefinition.ReturnType.InternalSubstituteTypeParameters(Me.TypeSubstitution).Type
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.TypeSubstitution.SubstituteCustomModifiers(Me.OriginalDefinition.ReturnType, Me.OriginalDefinition.ReturnTypeCustomModifiers)
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me.OriginalDefinition.ReturnTypeMarshallingInformation
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Public MustOverride ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

		Protected Sub New()
			MyBase.New()
		End Sub

		Friend NotOverridable Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function CallsAreOmitted(ByVal atNode As SyntaxNodeOrToken, ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree) As Boolean
			Return Me.OriginalDefinition.CallsAreOmitted(atNode, syntaxTree)
		End Function

		Public Overrides MustOverride Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As MethodSymbol

		Public Overrides MustOverride Function Equals(ByVal obj As Object) As Boolean

		Private Function EqualsWithNoRegardToTypeArguments(Of T As SubstitutedMethodSymbol)(ByVal other As T) As Boolean
			Dim flag As Boolean
			If (other Is Nothing) Then
				flag = False
			ElseIf (Me.OriginalDefinition.Equals(other.OriginalDefinition)) Then
				Dim containingType As NamedTypeSymbol = Me.ContainingType
				flag = If(Me.ContainingType.Equals(other.ContainingType), True, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return Me.OriginalDefinition.GetAppliedConditionalSymbols()
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.OriginalDefinition.GetAttributes()
		End Function

		Public NotOverridable Overrides Function GetDllImportData() As DllImportData
			Return Me.OriginalDefinition.GetDllImportData()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me.OriginalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer = Me.OriginalDefinition.GetHashCode()
			Return Hash.Combine(Of NamedTypeSymbol)(Me.ContainingType, hashCode)
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Return Me.OriginalDefinition.GetSecurityInformation()
		End Function

		Public Overrides Function GetTypeInferredDuringReduction(ByVal reducedFromTypeParameter As TypeParameterSymbol) As TypeSymbol
			Return Me.OriginalDefinition.GetTypeInferredDuringReduction(reducedFromTypeParameter)
		End Function

		Friend NotOverridable Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Return Me.OriginalDefinition.IsMetadataNewSlot(ignoreInterfaceImplementationChanges)
		End Function

		Friend Function SetAssociatedPropertyOrEvent(ByVal propertyOrEventSymbol As Symbol) As Boolean
			Dim flag As Boolean
			If (Me._propertyOrEventSymbolOpt IsNot Nothing) Then
				flag = False
			Else
				Me._propertyOrEventSymbolOpt = propertyOrEventSymbol
				flag = True
			End If
			Return flag
		End Function

		Protected Overridable Function SubstituteParameters() As ImmutableArray(Of ParameterSymbol)
			Dim empty As ImmutableArray(Of ParameterSymbol)
			Dim parameters As ImmutableArray(Of ParameterSymbol) = Me.OriginalDefinition.Parameters
			Dim length As Integer = parameters.Length
			If (length <> 0) Then
				Dim parameterSymbolArray(length - 1 + 1 - 1) As ParameterSymbol
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					parameterSymbolArray(num1) = SubstitutedParameterSymbol.CreateMethodParameter(Me, parameters(num1))
					num1 = num1 + 1
				Loop While num1 <= num
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(parameterSymbolArray)
			Else
				empty = ImmutableArray(Of ParameterSymbol).Empty
			End If
			Return empty
		End Function

		Friend NotOverridable Overrides Function TryGetMeParameter(<Out> ByRef meParameter As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) As Boolean
			Dim flag As Boolean
			Dim meParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol
			Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Nothing
			If (Me.OriginalDefinition.TryGetMeParameter(parameterSymbol)) Then
				If (parameterSymbol IsNot Nothing) Then
					meParameterSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.MeParameterSymbol(Me)
				Else
					meParameterSymbol = Nothing
				End If
				meParameter = meParameterSymbol
				flag = True
			Else
				meParameter = Nothing
				flag = False
			End If
			Return flag
		End Function

		Public MustInherit Class ConstructedMethod
			Inherits SubstitutedMethodSymbol
			Protected ReadOnly _substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

			Protected ReadOnly _typeArguments As ImmutableArray(Of TypeSymbol)

			Friend Overrides ReadOnly Property CanConstruct As Boolean
				Get
					Return False
				End Get
			End Property

			Public NotOverridable Overrides ReadOnly Property OriginalDefinition As MethodSymbol
				Get
					Return DirectCast(Me._substitution.TargetGenericDefinition, MethodSymbol)
				End Get
			End Property

			Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
				Get
					Return Me._typeArguments
				End Get
			End Property

			Public Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
				Get
					Return Me._substitution
				End Get
			End Property

			Protected Sub New(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal typeArguments As ImmutableArray(Of TypeSymbol))
				MyBase.New()
				Me._substitution = substitution
				Me._typeArguments = typeArguments
			End Sub

			Public Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As MethodSymbol
				Throw New InvalidOperationException()
			End Function

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim flag As Boolean
				If (obj <> Me) Then
					Dim constructedMethod As SubstitutedMethodSymbol.ConstructedMethod = TryCast(obj, SubstitutedMethodSymbol.ConstructedMethod)
					If (MyBase.EqualsWithNoRegardToTypeArguments(Of SubstitutedMethodSymbol.ConstructedMethod)(constructedMethod)) Then
						Dim typeArguments As ImmutableArray(Of TypeSymbol) = Me.TypeArguments
						Dim typeSymbols As ImmutableArray(Of TypeSymbol) = constructedMethod.TypeArguments
						Dim length As Integer = typeArguments.Length - 1
						Dim num As Integer = 0
						While num <= length
							If (typeArguments(num).Equals(typeSymbols(num))) Then
								num = num + 1
							Else
								flag = False
								Return flag
							End If
						End While
						flag = True
					Else
						flag = False
					End If
				Else
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function GetHashCode() As Integer
				Dim hashCode As Integer = MyBase.GetHashCode()
				Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = Me.TypeArguments.GetEnumerator()
				While enumerator.MoveNext()
					hashCode = Hash.Combine(Of TypeSymbol)(enumerator.Current, hashCode)
				End While
				Return hashCode
			End Function
		End Class

		Public NotInheritable Class ConstructedNotSpecializedGenericMethod
			Inherits SubstitutedMethodSymbol.ConstructedMethod
			Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

			Friend Overrides ReadOnly Property CallsiteReducedFromMethod As MethodSymbol
				Get
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
					Dim reducedFrom As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.ReducedFrom
					If (reducedFrom Is Nothing) Then
						methodSymbol = Nothing
					ElseIf (Me.Arity <> reducedFrom.Arity) Then
						Dim value(reducedFrom.Arity - 1 + 1 - 1) As TypeSymbol
						Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol)).Enumerator = Me.FixedTypeParameters.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As KeyValuePair(Of TypeParameterSymbol, TypeSymbol) = enumerator.Current
							value(current.Key.Ordinal) = current.Value
						End While
						Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = Me.TypeParameters
						Dim typeArguments As ImmutableArray(Of TypeSymbol) = Me.TypeArguments
						Dim length As Integer = typeArguments.Length - 1
						Dim num As Integer = 0
						Do
							value(typeParameters(num).ReducedFrom.Ordinal) = typeArguments(num)
							num = num + 1
						Loop While num <= length
						methodSymbol = reducedFrom.Construct(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(value))
					Else
						methodSymbol = reducedFrom.Construct(Me.TypeArguments)
					End If
					Return methodSymbol
				End Get
			End Property

			Public Overrides ReadOnly Property ConstructedFrom As MethodSymbol
				Get
					Return MyBase.OriginalDefinition
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return MyBase.OriginalDefinition.ContainingSymbol
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return MyBase.OriginalDefinition.TypeParameters
				End Get
			End Property

			Public Sub New(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal typeArguments As ImmutableArray(Of TypeSymbol))
				MyBase.New(substitution, typeArguments)
				Me._parameters = Me.SubstituteParameters()
			End Sub
		End Class

		Public NotInheritable Class ConstructedSpecializedGenericMethod
			Inherits SubstitutedMethodSymbol.ConstructedMethod
			Private ReadOnly _constructedFrom As SubstitutedMethodSymbol.SpecializedGenericMethod

			Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

			Public Overrides ReadOnly Property ConstructedFrom As MethodSymbol
				Get
					Return Me._constructedFrom
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._constructedFrom.ContainingSymbol
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return Me._constructedFrom.TypeParameters
				End Get
			End Property

			Public Sub New(ByVal constructedFrom As SubstitutedMethodSymbol.SpecializedGenericMethod, ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal typeArguments As ImmutableArray(Of TypeSymbol))
				MyBase.New(substitution, typeArguments)
				Me._constructedFrom = constructedFrom
				Me._parameters = Me.SubstituteParameters()
			End Sub
		End Class

		Public NotInheritable Class SpecializedGenericMethod
			Inherits SubstitutedMethodSymbol.SpecializedMethod
			Private ReadOnly _substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

			Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

			Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

			Friend Overrides ReadOnly Property CanConstruct As Boolean
				Get
					Dim flag As Boolean
					Dim containingType As NamedTypeSymbol = Me._container
					While True
						If (containingType.Arity <= 0) Then
							containingType = containingType.ContainingType
							If (containingType Is Nothing OrElse containingType.IsDefinition) Then
								flag = True
								Exit While
							End If
						ElseIf (CObj(containingType.ConstructedFrom) <> CObj(containingType)) Then
							flag = True
							Exit While
						Else
							flag = False
							Exit While
						End If
					End While
					Return flag
				End Get
			End Property

			Public Overrides ReadOnly Property OriginalDefinition As MethodSymbol
				Get
					Return DirectCast(Me._substitution.TargetGenericDefinition, MethodSymbol)
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
				Get
					Return StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me._typeParameters)
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return Me._typeParameters
				End Get
			End Property

			Public Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
				Get
					Return Me._substitution
				End Get
			End Property

			Private Sub New(ByVal container As SubstitutedNamedType, ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal typeParameters As ImmutableArray(Of SubstitutedTypeParameterSymbol))
				MyBase.New(container)
				Me._substitution = substitution
				Me._typeParameters = StaticCast(Of TypeParameterSymbol).From(Of SubstitutedTypeParameterSymbol)(typeParameters)
				Dim enumerator As ImmutableArray(Of SubstitutedTypeParameterSymbol).Enumerator = typeParameters.GetEnumerator()
				While enumerator.MoveNext()
					enumerator.Current.SetContainingSymbol(Me)
				End While
				Me._parameters = Me.SubstituteParameters()
			End Sub

			Public Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As MethodSymbol
				Dim constructedSpecializedGenericMethod As MethodSymbol
				MyBase.CheckCanConstructAndTypeArguments(typeArguments)
				typeArguments = typeArguments.TransformToCanonicalFormFor(Me)
				If (Not typeArguments.IsDefault) Then
					Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(Me._substitution.Parent, Me._substitution.TargetGenericDefinition, typeArguments, True)
					constructedSpecializedGenericMethod = New SubstitutedMethodSymbol.ConstructedSpecializedGenericMethod(Me, typeSubstitution, typeArguments)
				Else
					constructedSpecializedGenericMethod = Me
				End If
				Return constructedSpecializedGenericMethod
			End Function

			Public Shared Function Create(ByVal container As SubstitutedNamedType, ByVal originalDefinition As MethodSymbol) As SubstitutedMethodSymbol.SpecializedGenericMethod
				Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = originalDefinition.TypeParameters
				Dim substitutedTypeParameterSymbol(typeParameters.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol
				Dim length As Integer = typeParameters.Length - 1
				Dim num As Integer = 0
				Do
					substitutedTypeParameterSymbol(num) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol(typeParameters(num))
					num = num + 1
				Loop While num <= length
				Dim substitutedTypeParameterSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedTypeParameterSymbol)(substitutedTypeParameterSymbol)
				Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.CreateForAlphaRename(container.TypeSubstitution, substitutedTypeParameterSymbols)
				Return New SubstitutedMethodSymbol.SpecializedGenericMethod(container, typeSubstitution, substitutedTypeParameterSymbols)
			End Function

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If (obj = Me) Then
					Return True
				End If
				Return MyBase.EqualsWithNoRegardToTypeArguments(Of SubstitutedMethodSymbol.SpecializedGenericMethod)(TryCast(obj, SubstitutedMethodSymbol.SpecializedGenericMethod))
			End Function
		End Class

		Public MustInherit Class SpecializedMethod
			Inherits SubstitutedMethodSymbol
			Protected ReadOnly _container As SubstitutedNamedType

			Public Overrides ReadOnly Property ConstructedFrom As MethodSymbol
				Get
					Return Me
				End Get
			End Property

			Public Overrides ReadOnly Property ContainingSymbol As Symbol
				Get
					Return Me._container
				End Get
			End Property

			Public Overrides ReadOnly Property OriginalDefinition As MethodSymbol

			Protected Sub New(ByVal container As SubstitutedNamedType)
				MyBase.New()
				Me._container = container
			End Sub
		End Class

		Public Class SpecializedNonGenericMethod
			Inherits SubstitutedMethodSymbol.SpecializedMethod
			Private ReadOnly _originalDefinition As MethodSymbol

			Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

			Friend Overrides ReadOnly Property CanConstruct As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property OriginalDefinition As MethodSymbol
				Get
					Return Me._originalDefinition
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
				Get
					Return ImmutableArray(Of TypeSymbol).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return ImmutableArray(Of TypeParameterSymbol).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
				Get
					Return Me._container.TypeSubstitution
				End Get
			End Property

			Public Sub New(ByVal container As SubstitutedNamedType, ByVal originalDefinition As MethodSymbol)
				MyBase.New(container)
				Me._originalDefinition = originalDefinition
				Me._parameters = Me.SubstituteParameters()
			End Sub

			Public Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As MethodSymbol
				Throw New InvalidOperationException()
			End Function

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If (obj = Me) Then
					Return True
				End If
				Return MyBase.EqualsWithNoRegardToTypeArguments(Of SubstitutedMethodSymbol.SpecializedNonGenericMethod)(TryCast(obj, SubstitutedMethodSymbol.SpecializedNonGenericMethod))
			End Function
		End Class
	End Class
End Namespace
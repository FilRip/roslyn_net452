Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class InstanceErrorTypeSymbol
		Inherits ErrorTypeSymbol
		Protected ReadOnly _arity As Integer

		Private _lazyTypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Public NotOverridable Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._arity
			End Get
		End Property

		Friend Overrides ReadOnly Property CanConstruct As Boolean
			Get
				Return Me._arity > 0
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Get
				Return StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				If (Me._lazyTypeParameters.IsDefault) Then
					Dim errorTypeParameterSymbol(Me._arity - 1 + 1 - 1) As TypeParameterSymbol
					Dim num As Integer = Me._arity - 1
					Dim num1 As Integer = 0
					Do
						errorTypeParameterSymbol(num1) = New InstanceErrorTypeSymbol.ErrorTypeParameterSymbol(Me, num1)
						num1 = num1 + 1
					Loop While num1 <= num
					Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeParameterSymbol)(errorTypeParameterSymbol)
					Dim typeParameterSymbols1 As ImmutableArray(Of TypeParameterSymbol) = New ImmutableArray(Of TypeParameterSymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of TypeParameterSymbol)(Me._lazyTypeParameters, typeParameterSymbols, typeParameterSymbols1)
				End If
				Return Me._lazyTypeParameters
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Get
				Return Nothing
			End Get
		End Property

		Friend Sub New(ByVal arity As Integer)
			MyBase.New()
			Me._arity = arity
			If (arity = 0) Then
				Me._lazyTypeParameters = ImmutableArray(Of TypeParameterSymbol).Empty
			End If
		End Sub

		Public NotOverridable Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As NamedTypeSymbol
			Dim substitutedErrorType As NamedTypeSymbol
			MyBase.CheckCanConstructAndTypeArguments(typeArguments)
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(Me, Me.TypeParameters, typeArguments, True)
			If (typeSubstitution IsNot Nothing) Then
				substitutedErrorType = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedErrorType(Me.ContainingSymbol, Me, typeSubstitution)
			Else
				substitutedErrorType = Me
			End If
			Return substitutedErrorType
		End Function

		Public NotOverridable Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.InstanceErrorTypeSymbol::Equals(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.TypeCompareKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean Equals(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.TypeCompareKind)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Overrides MustOverride Function GetHashCode() As Integer

		Public NotOverridable Overrides Function GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)
			Return MyBase.GetEmptyTypeArgumentCustomModifiers(ordinal)
		End Function

		Friend NotOverridable Overrides Function InternalSubstituteTypeParameters(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
			Return New TypeWithModifiers(Me.InternalSubstituteTypeParametersInInstanceErrorTypeSymbol(substitution))
		End Function

		Private Function InternalSubstituteTypeParametersInInstanceErrorTypeSymbol(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim substitutedErrorType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (substitution IsNot Nothing) Then
				substitution = substitution.GetSubstitutionForGenericDefinitionOrContainers(Me)
			End If
			If (substitution IsNot Nothing) Then
				Dim containingSymbol As Symbol = Me.ContainingSymbol
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol IsNot Nothing) Then
					Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(namedTypeSymbol.InternalSubstituteTypeParameters(substitution).AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (substitution.TargetGenericDefinition <> Me) Then
						substitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(Me, namedTypeSymbol1.TypeSubstitution, Nothing)
					End If
					substitutedErrorType = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedErrorType(namedTypeSymbol1, Me, substitution)
				Else
					substitutedErrorType = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedErrorType(containingSymbol, Me, substitution)
				End If
			Else
				substitutedErrorType = Me
			End If
			Return substitutedErrorType
		End Function

		Protected MustOverride Function SpecializedEquals(ByVal other As InstanceErrorTypeSymbol) As Boolean

		Private NotInheritable Class ErrorTypeParameterSymbol
			Inherits TypeParameterSymbol
			Private ReadOnly _container As InstanceErrorTypeSymbol

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

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return ImmutableArray(Of Location).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Return [String].Empty
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

			Public Sub New(ByVal container As InstanceErrorTypeSymbol, ByVal ordinal As Integer)
				MyBase.New()
				Me._container = container
				Me._ordinal = ordinal
			End Sub

			Friend Overrides Sub EnsureAllConstraintsAreResolved()
			End Sub

			Public Overrides Function Equals(ByVal obj As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
				Dim flag As Boolean
				If (obj Is Nothing) Then
					flag = False
				ElseIf (obj <> Me) Then
					Dim errorTypeParameterSymbol As InstanceErrorTypeSymbol.ErrorTypeParameterSymbol = TryCast(obj, InstanceErrorTypeSymbol.ErrorTypeParameterSymbol)
					flag = If(errorTypeParameterSymbol Is Nothing OrElse errorTypeParameterSymbol._ordinal <> Me._ordinal, False, errorTypeParameterSymbol._container.Equals(Me._container, comparison))
				Else
					flag = True
				End If
				Return flag
			End Function

			Friend Overrides Function GetConstraints() As ImmutableArray(Of TypeParameterConstraint)
				Return ImmutableArray(Of TypeParameterConstraint).Empty
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Hash.Combine(Me._container.GetHashCode(), Me._ordinal)
			End Function
		End Class
	End Class
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SubstitutedErrorType
		Inherits ErrorTypeSymbol
		Private ReadOnly _fullInstanceType As InstanceErrorTypeSymbol

		Private ReadOnly _substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

		Private ReadOnly _container As Symbol

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._fullInstanceType.Arity
			End Get
		End Property

		Friend Overrides ReadOnly Property CanConstruct As Boolean
			Get
				If (Me.Arity <= 0) Then
					Return False
				End If
				Return Me.IdentitySubstitutionOnMyTypeParameters
			End Get
		End Property

		Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
			Get
				Dim substitutedErrorType As NamedTypeSymbol
				If (Me.ConstructedFromItself) Then
					substitutedErrorType = Me
				ElseIf (Me.ContainingSymbol Is Nothing OrElse Me.ContainingSymbol.IsDefinition) Then
					substitutedErrorType = Me._fullInstanceType
				Else
					Dim parent As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Me._substitution.Parent
					parent = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(Me._fullInstanceType, parent, Nothing)
					substitutedErrorType = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedErrorType(Me.ContainingSymbol, Me._fullInstanceType, parent)
				End If
				Return substitutedErrorType
			End Get
		End Property

		Private ReadOnly Property ConstructedFromItself As Boolean
			Get
				If (Me._fullInstanceType.Arity = 0) Then
					Return True
				End If
				Return Me.IdentitySubstitutionOnMyTypeParameters
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._fullInstanceType.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._container
			End Get
		End Property

		Friend Overrides ReadOnly Property ErrorInfo As DiagnosticInfo
			Get
				Return Me._fullInstanceType.ErrorInfo
			End Get
		End Property

		Friend Overrides ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean
			Get
				Dim flag As Boolean
				flag = If(Not Me.IdentitySubstitutionOnMyTypeParameters, Me._substitution.HasTypeArgumentsCustomModifiersFor(Me._fullInstanceType), False)
				Return flag
			End Get
		End Property

		Private ReadOnly Property IdentitySubstitutionOnMyTypeParameters As Boolean
			Get
				Return Me._substitution.Pairs.Length = 0
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._fullInstanceType.IsImplicitlyDeclared
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return Me._fullInstanceType.MangleName
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._fullInstanceType.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._fullInstanceType.Name
			End Get
		End Property

		Public Overrides ReadOnly Property OriginalDefinition As NamedTypeSymbol
			Get
				Return Me._fullInstanceType
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Get
				Dim typeArgumentsFor As ImmutableArray(Of TypeSymbol)
				If (Not Me.IdentitySubstitutionOnMyTypeParameters) Then
					Dim flag As Boolean = False
					typeArgumentsFor = Me._substitution.GetTypeArgumentsFor(Me._fullInstanceType, flag)
				Else
					typeArgumentsFor = StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
				End If
				Return typeArgumentsFor
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return Me._fullInstanceType.TypeParameters
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Get
				Return Me._substitution
			End Get
		End Property

		Public Sub New(ByVal container As Symbol, ByVal fullInstanceType As InstanceErrorTypeSymbol, ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution)
			MyBase.New()
			Me._container = container
			Me._fullInstanceType = fullInstanceType
			Me._substitution = substitution
		End Sub

		Public Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			MyBase.CheckCanConstructAndTypeArguments(typeArguments)
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(Me._fullInstanceType, Me._fullInstanceType.TypeParameters, typeArguments, True)
			namedTypeSymbol = If(typeSubstitution IsNot Nothing, New SubstitutedErrorType(Me._container, Me._fullInstanceType, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(Me._fullInstanceType, Me._substitution.Parent, typeSubstitution)), Me)
			Return namedTypeSymbol
		End Function

		Public Overrides Function Equals(ByVal obj As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedErrorType::Equals(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.TypeCompareKind)
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

		Public Overrides Function GetHashCode() As Integer
			Dim num As Integer
			Dim hashCode As Integer = Me._fullInstanceType.GetHashCode()
			If (Not Me._substitution.WasConstructedForModifiers()) Then
				hashCode = Hash.Combine(Of NamedTypeSymbol)(Me.ContainingType, hashCode)
				If (Not Me.ConstructedFromItself) Then
					Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = Me.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator()
					While enumerator.MoveNext()
						hashCode = Hash.Combine(Of TypeSymbol)(enumerator.Current, hashCode)
					End While
				End If
				num = hashCode
			Else
				num = hashCode
			End If
			Return num
		End Function

		Public Overrides Function GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)
			Dim typeArgumentsCustomModifiersFor As ImmutableArray(Of CustomModifier)
			If (Not Me.IdentitySubstitutionOnMyTypeParameters) Then
				Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Me._substitution
				Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = Me._fullInstanceType.TypeParameters
				typeArgumentsCustomModifiersFor = typeSubstitution.GetTypeArgumentsCustomModifiersFor(typeParameters(ordinal))
			Else
				typeArgumentsCustomModifiersFor = MyBase.GetEmptyTypeArgumentCustomModifiers(ordinal)
			End If
			Return typeArgumentsCustomModifiersFor
		End Function

		Friend Overrides Function InternalSubstituteTypeParameters(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
			Return New TypeWithModifiers(Me.InternalSubstituteTypeParametersInSubstitutedErrorType(additionalSubstitution))
		End Function

		Private Function InternalSubstituteTypeParametersInSubstitutedErrorType(ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (additionalSubstitution IsNot Nothing) Then
				Dim containingSymbol As Symbol = Me.ContainingSymbol
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol1 IsNot Nothing) Then
					Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(namedTypeSymbol1.InternalSubstituteTypeParameters(additionalSubstitution).AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.AdjustForConstruct(namedTypeSymbol2.TypeSubstitution, Me._substitution, additionalSubstitution)
					If (typeSubstitution IsNot Nothing) Then
						namedTypeSymbol = If(CObj(namedTypeSymbol2) <> CObj(namedTypeSymbol1) OrElse typeSubstitution <> Me._substitution, New SubstitutedErrorType(namedTypeSymbol2, Me._fullInstanceType, typeSubstitution), Me)
					Else
						namedTypeSymbol = Me._fullInstanceType
					End If
				Else
					Dim typeSubstitution1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.AdjustForConstruct(Nothing, Me._substitution, additionalSubstitution)
					If (typeSubstitution1 IsNot Nothing) Then
						namedTypeSymbol = If(typeSubstitution1 <> Me._substitution, New SubstitutedErrorType(containingSymbol, Me._fullInstanceType, typeSubstitution1), Me)
					Else
						namedTypeSymbol = Me._fullInstanceType
					End If
				End If
			Else
				namedTypeSymbol = Me
			End If
			Return namedTypeSymbol
		End Function
	End Class
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class InstanceTypeSymbol
		Inherits NamedTypeSymbol
		Friend Overrides ReadOnly Property CanConstruct As Boolean
			Get
				Return Me.Arity > 0
			End Get
		End Property

		Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
			Get
				Return Me
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Get
				Dim empty As ImmutableArray(Of TypeSymbol)
				If (Me.Arity <= 0) Then
					empty = ImmutableArray(Of TypeSymbol).Empty
				Else
					empty = StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
				End If
				Return empty
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Get
				Return Nothing
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
		End Sub

		Protected Function CalculateUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)(MyBase.PrimaryDependency)
			Dim useSiteInfo2 As UseSiteInfo(Of AssemblySymbol) = useSiteInfo1.AdjustDiagnosticInfo(Me.DeriveUseSiteErrorInfoFromBase())
			If (useSiteInfo2.DiagnosticInfo Is Nothing) Then
				If (Me.ContainingModule.HasUnifiedReferences) Then
					Dim typeSymbols As HashSet(Of TypeSymbol) = Nothing
					Dim unificationUseSiteDiagnosticRecursive As DiagnosticInfo = Me.GetUnificationUseSiteDiagnosticRecursive(Me, typeSymbols)
					If (unificationUseSiteDiagnosticRecursive IsNot Nothing) Then
						useSiteInfo2 = New UseSiteInfo(Of AssemblySymbol)(unificationUseSiteDiagnosticRecursive)
					End If
				End If
				useSiteInfo = useSiteInfo2
			Else
				useSiteInfo = useSiteInfo2
			End If
			Return useSiteInfo
		End Function

		Public NotOverridable Overrides Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As NamedTypeSymbol
			Dim constructedInstanceType As NamedTypeSymbol
			MyBase.CheckCanConstructAndTypeArguments(typeArguments)
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(Me, Me.TypeParameters, typeArguments, True)
			If (typeSubstitution IsNot Nothing) Then
				constructedInstanceType = New SubstitutedNamedType.ConstructedInstanceType(typeSubstitution)
			Else
				constructedInstanceType = Me
			End If
			Return constructedInstanceType
		End Function

		Private Function DeriveUseSiteErrorInfoFromBase() As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = MyBase.BaseTypeNoUseSiteDiagnostics
			While True
				If (baseTypeNoUseSiteDiagnostics Is Nothing) Then
					diagnosticInfo = Nothing
					Exit While
				ElseIf (Not baseTypeNoUseSiteDiagnostics.IsErrorType() OrElse Not TypeOf baseTypeNoUseSiteDiagnostics Is NoPiaIllegalGenericInstantiationSymbol) Then
					baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
				Else
					diagnosticInfo = baseTypeNoUseSiteDiagnostics.GetUseSiteInfo().DiagnosticInfo
					Exit While
				End If
			End While
			Return diagnosticInfo
		End Function

		Public Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.InstanceTypeSymbol::Equals(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.TypeCompareKind)
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
			Return RuntimeHelpers.GetHashCode(Me)
		End Function

		Public NotOverridable Overrides Function GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)
			Return MyBase.GetEmptyTypeArgumentCustomModifiers(ordinal)
		End Function

		Friend NotOverridable Overrides Function GetUnificationUseSiteDiagnosticRecursive(ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			If (Me.MarkCheckedIfNecessary(checkedTypes)) Then
				Dim unificationUseSiteErrorInfo As Microsoft.CodeAnalysis.DiagnosticInfo = owner.ContainingModule.GetUnificationUseSiteErrorInfo(Me)
				If (unificationUseSiteErrorInfo Is Nothing) Then
					Dim containingType As NamedTypeSymbol = Me.ContainingType
					If (containingType IsNot Nothing) Then
						unificationUseSiteErrorInfo = containingType.GetUnificationUseSiteDiagnosticRecursive(owner, checkedTypes)
						If (unificationUseSiteErrorInfo Is Nothing) Then
							GoTo Label1
						End If
						diagnosticInfo = unificationUseSiteErrorInfo
						Return diagnosticInfo
					End If
				Label1:
					Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = MyBase.BaseTypeNoUseSiteDiagnostics
					If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
						unificationUseSiteErrorInfo = baseTypeNoUseSiteDiagnostics.GetUnificationUseSiteDiagnosticRecursive(owner, checkedTypes)
						If (unificationUseSiteErrorInfo Is Nothing) Then
							GoTo Label2
						End If
						diagnosticInfo = unificationUseSiteErrorInfo
						Return diagnosticInfo
					End If
				Label2:
					Dim unificationUseSiteDiagnosticRecursive As Microsoft.CodeAnalysis.DiagnosticInfo = If(Symbol.GetUnificationUseSiteDiagnosticRecursive(Of NamedTypeSymbol)(MyBase.InterfacesNoUseSiteDiagnostics, owner, checkedTypes), Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.TypeParameters, owner, checkedTypes))
					diagnosticInfo = unificationUseSiteDiagnosticRecursive
				Else
					diagnosticInfo = unificationUseSiteErrorInfo
				End If
			Else
				diagnosticInfo = Nothing
			End If
			Return diagnosticInfo
		End Function

		Friend Overrides Function InternalSubstituteTypeParameters(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As TypeWithModifiers
			Return New TypeWithModifiers(Me.SubstituteTypeParametersInNamedType(substitution))
		End Function

		Private Function SubstituteTypeParametersInNamedType(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim constructedInstanceType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim typeWithModifier As TypeWithModifiers
			If (substitution IsNot Nothing) Then
				substitution = substitution.GetSubstitutionForGenericDefinitionOrContainers(Me)
			End If
			If (substitution IsNot Nothing) Then
				If (substitution.TargetGenericDefinition <> Me) Then
					typeWithModifier = Me.ContainingType.InternalSubstituteTypeParameters(substitution)
					namedTypeSymbol = DirectCast(typeWithModifier.AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				Else
					If (substitution.Parent Is Nothing) Then
						constructedInstanceType = New SubstitutedNamedType.ConstructedInstanceType(substitution)
						Return constructedInstanceType
					End If
					typeWithModifier = Me.ContainingType.InternalSubstituteTypeParameters(substitution.Parent)
					namedTypeSymbol = DirectCast(typeWithModifier.AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				End If
				If (Me.Arity <> 0) Then
					Dim specializedGenericType As SubstitutedNamedType.SpecializedGenericType = SubstitutedNamedType.SpecializedGenericType.Create(namedTypeSymbol, Me)
					If (substitution.TargetGenericDefinition <> Me) Then
						constructedInstanceType = specializedGenericType
					Else
						constructedInstanceType = New SubstitutedNamedType.ConstructedSpecializedGenericType(specializedGenericType, substitution)
					End If
				Else
					constructedInstanceType = SubstitutedNamedType.SpecializedNonGenericType.Create(namedTypeSymbol, Me, substitution)
				End If
			Else
				constructedInstanceType = Me
			End If
			Return constructedInstanceType
		End Function
	End Class
End Namespace
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Reflection.Metadata

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class PETypeParameterSymbol
		Inherits SubstitutableTypeParameterSymbol
		Private ReadOnly _containingSymbol As Symbol

		Private ReadOnly _handle As GenericParameterHandle

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private ReadOnly _name As String

		Private ReadOnly _ordinal As UShort

		Private ReadOnly _flags As GenericParameterAttributes

		Private _lazyConstraintTypes As ImmutableArray(Of TypeSymbol)

		Private _lazyCachedBoundsUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

		Friend Overrides ReadOnly Property ConstraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Get
				Me.EnsureAllConstraintsAreResolved()
				Return Me._lazyConstraintTypes
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._containingSymbol.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingSymbol
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

		Friend ReadOnly Property Handle As GenericParameterHandle
			Get
				Return Me._handle
			End Get
		End Property

		Public Overrides ReadOnly Property HasConstructorConstraint As Boolean
			Get
				Return (Me._flags And GenericParameterAttributes.DefaultConstructorConstraint) <> GenericParameterAttributes.None
			End Get
		End Property

		Public Overrides ReadOnly Property HasReferenceTypeConstraint As Boolean
			Get
				Return (Me._flags And GenericParameterAttributes.ReferenceTypeConstraint) <> GenericParameterAttributes.None
			End Get
		End Property

		Public Overrides ReadOnly Property HasValueTypeConstraint As Boolean
			Get
				Return (Me._flags And GenericParameterAttributes.NotNullableValueTypeConstraint) <> GenericParameterAttributes.None
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._containingSymbol.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Overrides ReadOnly Property Ordinal As Integer
			Get
				Return Me._ordinal
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameterKind As Microsoft.CodeAnalysis.TypeParameterKind
			Get
				If (Me.ContainingSymbol.Kind <> SymbolKind.Method) Then
					Return Microsoft.CodeAnalysis.TypeParameterKind.Type
				End If
				Return Microsoft.CodeAnalysis.TypeParameterKind.Method
			End Get
		End Property

		Public Overrides ReadOnly Property Variance As VarianceKind
			Get
				Return DirectCast(CShort((Me._flags And GenericParameterAttributes.VarianceMask)), VarianceKind)
			End Get
		End Property

		Friend Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal definingNamedType As PENamedTypeSymbol, ByVal ordinal As UShort, ByVal handle As GenericParameterHandle)
			MyClass.New(moduleSymbol, DirectCast(definingNamedType, Symbol), ordinal, handle)
		End Sub

		Friend Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal definingMethod As PEMethodSymbol, ByVal ordinal As UShort, ByVal handle As GenericParameterHandle)
			MyClass.New(moduleSymbol, DirectCast(definingMethod, Symbol), ordinal, handle)
		End Sub

		Private Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal definingSymbol As Symbol, ByVal ordinal As UShort, ByVal handle As GenericParameterHandle)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PETypeParameterSymbol::.ctor(Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEModuleSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbol,System.UInt16,System.Reflection.Metadata.GenericParameterHandle)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void .ctor(Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEModuleSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbol,System.UInt16,System.Reflection.Metadata.GenericParameterHandle)
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

		End Sub

		Friend Overrides Sub EnsureAllConstraintsAreResolved()
			If (Me._lazyConstraintTypes.IsDefault) Then
				TypeParameterSymbol.EnsureAllConstraintsAreResolved(If(Me._containingSymbol.Kind = SymbolKind.Method, DirectCast(Me._containingSymbol, PEMethodSymbol).TypeParameters, DirectCast(Me._containingSymbol, PENamedTypeSymbol).TypeParameters))
			End If
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			If (Me._lazyCustomAttributes.IsDefault) Then
				DirectCast(Me.ContainingModule, PEModuleSymbol).LoadCustomAttributes(Me._handle, Me._lazyCustomAttributes)
			End If
			Return Me._lazyCustomAttributes
		End Function

		Friend Overrides Function GetConstraintsUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Me.EnsureAllConstraintsAreResolved()
			Return Me._lazyCachedBoundsUseSiteInfo.ToUseSiteInfo(MyBase.PrimaryDependency)
		End Function

		Private Function GetDeclaredConstraints() As ImmutableArray(Of TypeParameterConstraint)
			' 
			' Current member / type: System.Collections.Immutable.ImmutableArray`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterConstraint> Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PETypeParameterSymbol::GetDeclaredConstraints()
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterConstraint> GetDeclaredConstraints()
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

		Friend Overrides Sub ResolveConstraints(ByVal inProgress As ConsList(Of TypeParameterSymbol))
			If (Me._lazyConstraintTypes.IsDefault) Then
				Dim instance As ArrayBuilder(Of TypeParameterDiagnosticInfo) = ArrayBuilder(Of TypeParameterDiagnosticInfo).GetInstance()
				Dim num As Integer = If(Me._containingSymbol.Kind <> SymbolKind.Method, False, DirectCast(Me._containingSymbol, MethodSymbol).IsOverrides)
				Dim typeParameterConstraints As ImmutableArray(Of TypeParameterConstraint) = Me.RemoveDirectConstraintConflicts(Me.GetDeclaredConstraints(), inProgress.Prepend(Me), DirectConstraintConflictKind.None, instance)
				Dim primaryDependency As AssemblySymbol = MyBase.PrimaryDependency
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)(primaryDependency)
				Dim enumerator As ArrayBuilder(Of TypeParameterDiagnosticInfo).Enumerator = instance.GetEnumerator()
				Do
					If (Not enumerator.MoveNext()) Then
						Exit Do
					End If
					useSiteInfo = MyBase.MergeUseSiteInfo(useSiteInfo, enumerator.Current.UseSiteInfo)
				Loop While useSiteInfo.DiagnosticInfo Is Nothing
				instance.Free()
				Me._lazyCachedBoundsUseSiteInfo.InterlockedCompareExchange(primaryDependency, useSiteInfo)
				ImmutableInterlocked.InterlockedInitialize(Of TypeSymbol)(Me._lazyConstraintTypes, TypeParameterSymbol.GetConstraintTypesOnly(typeParameterConstraints))
			End If
		End Sub
	End Class
End Namespace
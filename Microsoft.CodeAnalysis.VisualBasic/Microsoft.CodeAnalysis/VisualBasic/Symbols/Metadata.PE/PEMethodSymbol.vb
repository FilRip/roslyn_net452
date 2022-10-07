Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class PEMethodSymbol
		Inherits MethodSymbol
		Private ReadOnly _handle As MethodDefinitionHandle

		Private ReadOnly _name As String

		Private ReadOnly _implFlags As UShort

		Private ReadOnly _flags As UShort

		Private ReadOnly _containingType As PENamedTypeSymbol

		Private _associatedPropertyOrEventOpt As Symbol

		Private _packedFlags As PEMethodSymbol.PackedFlags

		Private _lazyTypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private _lazyExplicitMethodImplementations As ImmutableArray(Of MethodSymbol)

		Private _uncommonFields As PEMethodSymbol.UncommonFields

		Private _lazySignature As PEMethodSymbol.SignatureData

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Dim length As Integer
				If (Not Me._lazyTypeParameters.IsDefault) Then
					length = Me._lazyTypeParameters.Length
				Else
					Try
						Dim num As Integer = 0
						Dim num1 As Integer = 0
						MetadataDecoder(Of PEModuleSymbol, TypeSymbol, MethodSymbol, FieldSymbol, Symbol).GetSignatureCountsOrThrow(Me._containingType.ContainingPEModule.[Module], Me._handle, num, num1)
						length = num1
					Catch badImageFormatException As System.BadImageFormatException
						ProjectData.SetProjectError(badImageFormatException)
						length = Me.TypeParameters.Length
						ProjectData.ClearProjectError()
					End Try
				End If
				Return length
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Me._associatedPropertyOrEventOpt
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return DirectCast(Me.Signature.Header.RawValue, Microsoft.Cci.CallingConvention)
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
				' 
				' Current member / type: Microsoft.CodeAnalysis.Accessibility Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::get_DeclaredAccessibility()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: Microsoft.CodeAnalysis.Accessibility get_DeclaredAccessibility()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 84
				'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 73
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 27
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 198
				'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 79
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
				'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
				'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
				'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 44
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

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

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Dim methodSymbols As ImmutableArray(Of MethodSymbol)
				If (Me._lazyExplicitMethodImplementations.IsDefault) Then
					Dim explicitlyOverriddenMethods As ImmutableArray(Of MethodSymbol) = (New MetadataDecoder(Me._containingType.ContainingPEModule, Me._containingType)).GetExplicitlyOverriddenMethods(Me._containingType.Handle, Me._handle, Me.ContainingType)
					Dim flag As Boolean = False
					Dim enumerator As ImmutableArray(Of MethodSymbol).Enumerator = explicitlyOverriddenMethods.GetEnumerator()
					While enumerator.MoveNext()
						If (enumerator.Current.ContainingType.IsInterface) Then
							Continue While
						End If
						flag = True
						Exit While
					End While
					Dim immutableAndFree As ImmutableArray(Of MethodSymbol) = explicitlyOverriddenMethods
					If (flag) Then
						Dim instance As ArrayBuilder(Of MethodSymbol) = ArrayBuilder(Of MethodSymbol).GetInstance()
						Dim enumerator1 As ImmutableArray(Of MethodSymbol).Enumerator = explicitlyOverriddenMethods.GetEnumerator()
						While enumerator1.MoveNext()
							Dim current As MethodSymbol = enumerator1.Current
							If (Not current.ContainingType.IsInterface) Then
								Continue While
							End If
							instance.Add(current)
						End While
						immutableAndFree = instance.ToImmutableAndFree()
					End If
					methodSymbols = InterlockedOperations.Initialize(Of MethodSymbol)(Me._lazyExplicitMethodImplementations, immutableAndFree)
				Else
					methodSymbols = Me._lazyExplicitMethodImplementations
				End If
				Return methodSymbols
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Friend ReadOnly Property Handle As MethodDefinitionHandle
			Get
				Return Me._handle
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return ' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::HasRuntimeSpecialName()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean HasRuntimeSpecialName()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3501
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 72
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1528
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 769
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1286
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1284
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 2085
				'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 453
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com


		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return ' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::HasSpecialName()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean HasSpecialName()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3501
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 72
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1528
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 769
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1286
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1284
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 2085
				'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 453
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com


		Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Return DirectCast(Me._implFlags, MethodImplAttributes)
			End Get
		End Property

		Friend Overrides ReadOnly Property IsAccessCheckedOnOverride As Boolean
			Get
				Return ' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::IsAccessCheckedOnOverride()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean IsAccessCheckedOnOverride()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3501
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 72
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1528
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 769
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1286
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1284
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 2085
				'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 453
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com


		Public Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				If (Not Me._packedFlags.IsExtensionMethodPopulated) Then
					Dim flag As Boolean = False
					If (Me.IsShared AndAlso Me.ParameterCount > 0 AndAlso Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.Ordinary AndAlso Me._containingType.MightContainExtensionMethods AndAlso Me._containingType.ContainingPEModule.[Module].HasExtensionAttribute(Me.Handle, True) AndAlso MyBase.ValidateGenericConstraintsOnExtensionMethodDefinition()) Then
						Dim item As ParameterSymbol = Me.Parameters(0)
						flag = If(item.IsOptional, False, Not item.IsParamArray)
					End If
					Me._packedFlags.InitializeIsExtensionMethod(flag)
				End If
				Return Me._packedFlags.IsExtensionMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExternal As Boolean
			Get
				If (Me.IsExternalMethod) Then
					Return True
				End If
				Return ' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::IsExternal()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean IsExternal()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3501
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 72
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1528
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 769
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1286
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1284
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 2085
				'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 453
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com


		Public Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::get_IsExternalMethod()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean get_IsExternalMethod()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Public Overrides ReadOnly Property IsGenericMethod As Boolean
			Get
				Return Me.Arity > 0
			End Get
		End Property

		Friend Overrides ReadOnly Property IsHiddenBySignature As Boolean
			Get
				Return ' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::IsHiddenBySignature()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean IsHiddenBySignature()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3501
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 72
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1528
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 769
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1286
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1284
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 2085
				'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 453
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com


		Public Overrides ReadOnly Property IsInitOnly As Boolean
			Get
				Dim flag As Boolean
				If (Not Me._packedFlags.IsInitOnlyPopulated) Then
					flag = If(Me.IsShared OrElse Me.MethodKind <> Microsoft.CodeAnalysis.MethodKind.PropertySet, False, CustomModifierUtils.HasIsExternalInitModifier(Me.ReturnTypeCustomModifiers))
					Me._packedFlags.InitializeIsInitOnly(flag)
				End If
				Return Me._packedFlags.IsInitOnly
			End Get
		End Property

		Public Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataFinal As Boolean
			Get
				Return ' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::IsMetadataFinal()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean IsMetadataFinal()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3501
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 72
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1528
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 769
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1286
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1284
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 2085
				'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 453
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com


		Friend Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::get_IsMustOverride()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean get_IsMustOverride()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::get_IsNotOverridable()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean get_IsNotOverridable()
				' 
				' File d'attente vide.
				'    à System.ThrowHelper.ThrowInvalidOperationException(ExceptionResource resource)
				'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.(ICollection`1 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 525
				'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 445
				'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 363
				'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 307
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 86
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::get_IsOverloads()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean get_IsOverloads()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Dim num As Integer = Me._flags And 1376
				If (num = 320) Then
					Return True
				End If
				If (Me._containingType.IsInterface OrElse num <> 64) Then
					Return False
				End If
				Return Me._containingType.BaseTypeNoUseSiteDiagnostics Is Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::get_IsOverrides()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean get_IsOverrides()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return ' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::IsShared()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean IsShared()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3501
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 72
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1528
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(PropertyDefinition , Boolean ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 769
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1286
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
				'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1284
				'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.Write(PropertyDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 2085
				'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 453
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com


		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me.ReturnType.SpecialType = SpecialType.System_Void
			End Get
		End Property

		Public Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return Me.Signature.Header.CallingConvention = SignatureCallingConvention.VarArgs
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return StaticCast(Of Location).From(Of MetadataLocation)(Me._containingType.ContainingPEModule.MetadataLocation)
			End Get
		End Property

		Friend ReadOnly Property MethodFlags As MethodAttributes
			Get
				Return DirectCast(Me._flags, MethodAttributes)
			End Get
		End Property

		Friend ReadOnly Property MethodImplFlags As MethodImplAttributes
			Get
				Return DirectCast(Me._implFlags, MethodImplAttributes)
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				If (Not Me._packedFlags.MethodKindIsPopulated) Then
					Me._packedFlags.InitializeMethodKind(Me.ComputeMethodKind())
				End If
				Return Me._packedFlags.MethodKind
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Dim obsoleteAttributeDatum As Microsoft.CodeAnalysis.ObsoleteAttributeData
				If (Me._packedFlags.IsObsoleteAttributePopulated) Then
					Dim uncommonField As PEMethodSymbol.UncommonFields = Me._uncommonFields
					If (uncommonField IsNot Nothing) Then
						Dim obsoleteAttributeDatum1 As Microsoft.CodeAnalysis.ObsoleteAttributeData = uncommonField._lazyObsoleteAttributeData
						obsoleteAttributeDatum = If(obsoleteAttributeDatum1 = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized, InterlockedOperations.Initialize(Of Microsoft.CodeAnalysis.ObsoleteAttributeData)(uncommonField._lazyObsoleteAttributeData, Nothing, Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized), obsoleteAttributeDatum1)
					Else
						obsoleteAttributeDatum = Nothing
					End If
				Else
					Dim obsoleteDataFromMetadata As Microsoft.CodeAnalysis.ObsoleteAttributeData = ObsoleteAttributeHelpers.GetObsoleteDataFromMetadata(Me._handle, DirectCast(Me.ContainingModule, PEModuleSymbol))
					If (obsoleteDataFromMetadata IsNot Nothing) Then
						obsoleteDataFromMetadata = InterlockedOperations.Initialize(Of Microsoft.CodeAnalysis.ObsoleteAttributeData)(Me.AccessUncommonFields()._lazyObsoleteAttributeData, obsoleteDataFromMetadata, Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized)
					End If
					Me._packedFlags.SetIsObsoleteAttributePopulated()
					obsoleteAttributeDatum = obsoleteDataFromMetadata
				End If
				Return obsoleteAttributeDatum
			End Get
		End Property

		Friend Overrides ReadOnly Property ParameterCount As Integer
			Get
				Dim length As Integer
				If (Me._lazySignature IsNot Nothing) Then
					length = Me._lazySignature.Parameters.Length
				Else
					Try
						Dim num As Integer = 0
						Dim num1 As Integer = 0
						MetadataDecoder(Of PEModuleSymbol, TypeSymbol, MethodSymbol, FieldSymbol, Symbol).GetSignatureCountsOrThrow(Me._containingType.ContainingPEModule.[Module], Me._handle, num, num1)
						length = num
					Catch badImageFormatException As System.BadImageFormatException
						ProjectData.SetProjectError(badImageFormatException)
						length = Me.Parameters.Length
						ProjectData.ClearProjectError()
					End Try
				End If
				Return length
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me.Signature.Parameters
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.Signature.ReturnParam.RefCustomModifiers
			End Get
		End Property

		Friend ReadOnly Property ReturnParam As PEParameterSymbol
			Get
				Return Me.Signature.ReturnParam
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return Me.Signature.ReturnParam.IsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me.Signature.ReturnParam.Type
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.Signature.ReturnParam.CustomModifiers
			End Get
		End Property

		Friend Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me._lazySignature.ReturnParam.MarshallingInformation
			End Get
		End Property

		Friend Overrides ReadOnly Property ReturnValueIsMarshalledExplicitly As Boolean
			Get
				Return Me._lazySignature.ReturnParam.IsMarshalledExplicitly
			End Get
		End Property

		Friend Overrides ReadOnly Property ReturnValueMarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				Return Me._lazySignature.ReturnParam.MarshallingDescriptor
			End Get
		End Property

		Private ReadOnly Property Signature As PEMethodSymbol.SignatureData
			Get
				Return If(Me._lazySignature, Me.LoadSignature())
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Dim empty As ImmutableArray(Of TypeSymbol)
				If (Not Me.IsGenericMethod) Then
					empty = ImmutableArray(Of TypeSymbol).Empty
				Else
					empty = StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
				End If
				Return empty
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = Nothing
				Me.EnsureTypeParametersAreLoaded(diagnosticInfo)
				Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol) = Me.EnsureTypeParametersAreLoaded(diagnosticInfo)
				If (diagnosticInfo IsNot Nothing) Then
					Me.InitializeUseSiteInfo(New UseSiteInfo(Of AssemblySymbol)(diagnosticInfo))
				End If
				Return typeParameterSymbols
			End Get
		End Property

		Friend Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingType As PENamedTypeSymbol, ByVal handle As MethodDefinitionHandle)
			MyBase.New()
			Dim methodImplAttribute As MethodImplAttributes
			Dim methodAttribute As MethodAttributes
			Dim num As Integer
			Me._handle = handle
			Me._containingType = containingType
			Try
				moduleSymbol.[Module].GetMethodDefPropsOrThrow(handle, Me._name, methodImplAttribute, methodAttribute, num)
				Me._implFlags = CUShort(methodImplAttribute)
				Me._flags = CUShort(methodAttribute)
			Catch badImageFormatException As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException)
				If (Me._name Is Nothing) Then
					Me._name = [String].Empty
				End If
				Me.InitializeUseSiteInfo(New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedMethod1, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(Me) })))
				ProjectData.ClearProjectError()
			End Try
		End Sub

		Private Function AccessUncommonFields() As PEMethodSymbol.UncommonFields
			Return If(Me._uncommonFields, InterlockedOperations.Initialize(Of PEMethodSymbol.UncommonFields)(Me._uncommonFields, Me.CreateUncommonFields()))
		End Function

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Sub CheckUnmanagedCallersOnly(ByRef errorInfo As DiagnosticInfo)
			If ((errorInfo Is Nothing OrElse errorInfo.Code <> 30657) AndAlso DirectCast(Me.ContainingModule, PEModuleSymbol).[Module].FindTargetAttribute(Me._handle, AttributeDescription.UnmanagedCallersOnlyAttribute).HasValue) Then
				errorInfo = ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedMethod1, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(Me) })
			End If
		End Sub

		Private Function ComputeMethodKind() As Microsoft.CodeAnalysis.MethodKind
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind
			Dim name As String = Me.Name
			If (Me.HasSpecialName) Then
				If (Not name.StartsWith(".", StringComparison.Ordinal)) Then
					If (Not Me.IsShared OrElse Me.DeclaredAccessibility <> Accessibility.[Public] OrElse Me.IsSub OrElse Me.Arity <> 0) Then
						methodKind = If(Me.IsShared OrElse Not [String].Equals(name, "Invoke", StringComparison.Ordinal) OrElse Me._containingType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate], Microsoft.CodeAnalysis.MethodKind.Ordinary, Microsoft.CodeAnalysis.MethodKind.DelegateInvoke)
						Return methodKind
					End If
					Dim operatorInfo As OverloadResolution.OperatorInfo = OverloadResolution.GetOperatorInfo(name)
					If (operatorInfo.ParamCount = 0 OrElse Not OverloadResolution.ValidateOverloadedOperator(Me, operatorInfo)) Then
						methodKind = Microsoft.CodeAnalysis.MethodKind.Ordinary
						Return methodKind
					Else
						methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(operatorInfo)
						Return methodKind
					End If
				Else
					If (' 
					' Current member / type: Microsoft.CodeAnalysis.MethodKind Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::ComputeMethodKind()
					' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
					' 
					' Product version: 2019.1.118.0
					' Exception in: Microsoft.CodeAnalysis.MethodKind ComputeMethodKind()
					' 
					' La référence d'objet n'est pas définie à une instance d'un objet.
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3490
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 102
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 119
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(IfStatement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 108
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 78
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
					'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
					'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1916
					'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1841
					'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 447
					' 
					' mailto: JustDecompilePublicFeedback@telerik.com


		Private Function ComputeMethodKindForPotentialOperatorOrConversion(ByVal opInfo As OverloadResolution.OperatorInfo) As Microsoft.CodeAnalysis.MethodKind
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind
			Dim unaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind
			If (Not opInfo.IsUnary) Then
				Select Case opInfo.BinaryOperatorKind
					Case BinaryOperatorKind.Add
					Case BinaryOperatorKind.Concatenate
					Case BinaryOperatorKind.[Like]
					Case BinaryOperatorKind.Equals
					Case BinaryOperatorKind.NotEquals
					Case BinaryOperatorKind.LessThanOrEqual
					Case BinaryOperatorKind.GreaterThanOrEqual
					Case BinaryOperatorKind.LessThan
					Case BinaryOperatorKind.GreaterThan
					Case BinaryOperatorKind.Subtract
					Case BinaryOperatorKind.Multiply
					Case BinaryOperatorKind.Power
					Case BinaryOperatorKind.Divide
					Case BinaryOperatorKind.Modulo
					Case BinaryOperatorKind.IntegerDivide
					Case BinaryOperatorKind.[Xor]
						methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, Nothing, False)
						Exit Select
					Case BinaryOperatorKind.LeftShift
						If (Not CaseInsensitiveComparison.Equals(Me.Name, "op_LeftShift")) Then
							methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, "op_LeftShift", False)
							Exit Select
						Else
							methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, Nothing, False)
							Exit Select
						End If
					Case BinaryOperatorKind.RightShift
						If (Not CaseInsensitiveComparison.Equals(Me.Name, "op_RightShift")) Then
							methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, "op_RightShift", False)
							Exit Select
						Else
							methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, Nothing, False)
							Exit Select
						End If
					Case BinaryOperatorKind.[Or]
						If (Not CaseInsensitiveComparison.Equals(Me.Name, "op_BitwiseOr")) Then
							methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, "op_BitwiseOr", False)
							Exit Select
						Else
							methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, Nothing, False)
							Exit Select
						End If
					Case BinaryOperatorKind.[OrElse]
						Throw ExceptionUtilities.UnexpectedValue(opInfo.BinaryOperatorKind)
					Case BinaryOperatorKind.[And]
						If (Not CaseInsensitiveComparison.Equals(Me.Name, "op_BitwiseAnd")) Then
							methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, "op_BitwiseAnd", False)
							Exit Select
						Else
							methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, Nothing, False)
							Exit Select
						End If
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(opInfo.BinaryOperatorKind)
				End Select
			Else
				unaryOperatorKind = opInfo.UnaryOperatorKind
				If (unaryOperatorKind > Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Implicit) Then
					If (unaryOperatorKind <> Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Explicit) Then
						GoTo Label3
					End If
					methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.Conversion, "op_Implicit", True)
					Return methodKind
				Else
					If (CInt(unaryOperatorKind) - CInt(Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Plus) <= CInt(Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Plus)) Then
						GoTo Label4
					End If
					If (unaryOperatorKind <> Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.[Not]) Then
						If (unaryOperatorKind <> Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Implicit) Then
							Throw ExceptionUtilities.UnexpectedValue(opInfo.UnaryOperatorKind)
						End If
						methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.Conversion, "op_Explicit", True)
						Return methodKind
					ElseIf (Not CaseInsensitiveComparison.Equals(Me.Name, "op_OnesComplement")) Then
						methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, "op_OnesComplement", False)
						Return methodKind
					Else
						methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, Nothing, False)
						Return methodKind
					End If
				End If
			Label4:
				methodKind = Me.ComputeMethodKindForPotentialOperatorOrConversion(opInfo, Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator, Nothing, False)
			End If
			Return methodKind
		Label3:
			If (unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsTrue OrElse unaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsFalse) Then
				GoTo Label4
			End If
			Throw ExceptionUtilities.UnexpectedValue(opInfo.UnaryOperatorKind)
		End Function

		Private Function ComputeMethodKindForPotentialOperatorOrConversion(ByVal opInfo As OverloadResolution.OperatorInfo, ByVal potentialMethodKind As Microsoft.CodeAnalysis.MethodKind, ByVal additionalNameOpt As String, ByVal adjustContendersOfAdditionalName As Boolean) As Microsoft.CodeAnalysis.MethodKind
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind = potentialMethodKind
			Dim parameters As ImmutableArray(Of ParameterSymbol) = Me.Parameters
			Dim returnType As TypeSymbol = Me.ReturnType
			Dim num As Integer = If(additionalNameOpt Is Nothing, 0, 1)
			Dim num1 As Integer = 0
			Do
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me._containingType.GetMembers(If(num1 = 0, Me.Name, additionalNameOpt)).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current = Me OrElse current.Kind <> SymbolKind.Method) Then
						Continue While
					End If
					Dim pEMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol)
					If (pEMethodSymbol Is Nothing OrElse Not pEMethodSymbol.IsPotentialOperatorOrConversion(opInfo)) Then
						Continue While
					End If
					If (pEMethodSymbol._packedFlags.MethodKindIsPopulated) Then
						Dim methodKind1 As Microsoft.CodeAnalysis.MethodKind = pEMethodSymbol._packedFlags.MethodKind
						If (methodKind1 <> Microsoft.CodeAnalysis.MethodKind.Ordinary AndAlso (methodKind1 <> potentialMethodKind OrElse num1 = 0 OrElse adjustContendersOfAdditionalName)) Then
							Continue While
						End If
					End If
					If (potentialMethodKind = Microsoft.CodeAnalysis.MethodKind.Conversion AndAlso Not returnType.IsSameTypeIgnoringAll(pEMethodSymbol.ReturnType)) Then
						Continue While
					End If
					Dim length As Integer = parameters.Length - 1
					Dim num2 As Integer = 0
					While num2 <= length AndAlso parameters(num2).Type.IsSameTypeIgnoringAll(pEMethodSymbol.Parameters(num2).Type)
						num2 = num2 + 1
					End While
					If (num2 < parameters.Length) Then
						Continue While
					End If
					methodKind = Microsoft.CodeAnalysis.MethodKind.Ordinary
					If (num1 <> 0 AndAlso Not adjustContendersOfAdditionalName) Then
						Continue While
					End If
					pEMethodSymbol._packedFlags.InitializeMethodKind(Microsoft.CodeAnalysis.MethodKind.Ordinary)
				End While
				num1 = num1 + 1
			Loop While num1 <= num
			Return methodKind
		End Function

		Private Function CreateUncommonFields() As PEMethodSymbol.UncommonFields
			Dim uncommonField As PEMethodSymbol.UncommonFields = New PEMethodSymbol.UncommonFields()
			If (Not Me._packedFlags.IsObsoleteAttributePopulated) Then
				uncommonField._lazyObsoleteAttributeData = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized
			End If
			If (Me._packedFlags.IsCustomAttributesPopulated) Then
				uncommonField._lazyCustomAttributes = ImmutableArray(Of VisualBasicAttributeData).Empty
			End If
			If (Me._packedFlags.IsConditionalPopulated) Then
				uncommonField._lazyConditionalAttributeSymbols = ImmutableArray(Of String).Empty
			End If
			Return uncommonField
		End Function

		Private Function EnsureTypeParametersAreLoaded(ByRef errorInfo As DiagnosticInfo) As ImmutableArray(Of TypeParameterSymbol)
			Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol)
			Dim typeParameterSymbols1 As ImmutableArray(Of TypeParameterSymbol) = Me._lazyTypeParameters
			typeParameterSymbols = If(typeParameterSymbols1.IsDefault, InterlockedOperations.Initialize(Of TypeParameterSymbol)(Me._lazyTypeParameters, Me.LoadTypeParameters(errorInfo)), typeParameterSymbols1)
			Return typeParameterSymbols
		End Function

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Dim empty As ImmutableArray(Of String)
			If (Me._packedFlags.IsConditionalPopulated) Then
				Dim uncommonField As PEMethodSymbol.UncommonFields = Me._uncommonFields
				If (uncommonField IsNot Nothing) Then
					Dim strs As ImmutableArray(Of String) = uncommonField._lazyConditionalAttributeSymbols
					empty = If(strs.IsDefault, InterlockedOperations.Initialize(Of String)(uncommonField._lazyConditionalAttributeSymbols, ImmutableArray(Of String).Empty), strs)
				Else
					empty = ImmutableArray(Of String).Empty
				End If
			Else
				Dim conditionalAttributeValues As ImmutableArray(Of String) = Me._containingType.ContainingPEModule.[Module].GetConditionalAttributeValues(Me._handle)
				If (Not conditionalAttributeValues.IsEmpty) Then
					conditionalAttributeValues = InterlockedOperations.Initialize(Of String)(Me.AccessUncommonFields()._lazyConditionalAttributeSymbols, conditionalAttributeValues)
				End If
				Me._packedFlags.SetIsConditionalAttributePopulated()
				empty = conditionalAttributeValues
			End If
			Return empty
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Dim empty As ImmutableArray(Of VisualBasicAttributeData)
			If (Me._packedFlags.IsCustomAttributesPopulated) Then
				Dim uncommonField As PEMethodSymbol.UncommonFields = Me._uncommonFields
				If (uncommonField IsNot Nothing) Then
					Dim visualBasicAttributeDatas As ImmutableArray(Of VisualBasicAttributeData) = uncommonField._lazyCustomAttributes
					empty = If(visualBasicAttributeDatas.IsDefault, InterlockedOperations.Initialize(Of VisualBasicAttributeData)(uncommonField._lazyCustomAttributes, ImmutableArray(Of VisualBasicAttributeData).Empty), visualBasicAttributeDatas)
				Else
					empty = ImmutableArray(Of VisualBasicAttributeData).Empty
				End If
			Else
				Dim visualBasicAttributeDatas1 As ImmutableArray(Of VisualBasicAttributeData) = New ImmutableArray(Of VisualBasicAttributeData)()
				DirectCast(Me.ContainingModule, PEModuleSymbol).LoadCustomAttributes(Me.Handle, visualBasicAttributeDatas1)
				If (Not visualBasicAttributeDatas1.IsEmpty) Then
					visualBasicAttributeDatas1 = InterlockedOperations.Initialize(Of VisualBasicAttributeData)(Me.AccessUncommonFields()._lazyCustomAttributes, visualBasicAttributeDatas1)
				End If
				Me._packedFlags.SetIsCustomAttributesPopulated()
				empty = visualBasicAttributeDatas1
			End If
			Return empty
		End Function

		Private Function GetCachedUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim cachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)
			Dim cachedUseSiteInfo1 As CachedUseSiteInfo(Of AssemblySymbol)
			Dim uncommonField As PEMethodSymbol.UncommonFields = Me._uncommonFields
			If (uncommonField IsNot Nothing) Then
				cachedUseSiteInfo1 = uncommonField._lazyCachedUseSiteInfo
			Else
				cachedUseSiteInfo = New CachedUseSiteInfo(Of AssemblySymbol)()
				cachedUseSiteInfo1 = cachedUseSiteInfo
			End If
			cachedUseSiteInfo = cachedUseSiteInfo1
			Return cachedUseSiteInfo.ToUseSiteInfo(MyBase.PrimaryDependency)
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return DirectCast(Me.GetAttributes(), IEnumerable(Of VisualBasicAttributeData))
		End Function

		Public Overrides Function GetDllImportData() As DllImportData
			' 
			' Current member / type: Microsoft.CodeAnalysis.DllImportData Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::GetDllImportData()
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.DllImportData GetDllImportData()
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

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return PEDocumentationCommentUtils.GetDocumentationComment(Me, Me._containingType.ContainingPEModule, preferredCulture, cancellationToken, Me.AccessUncommonFields()._lazyDocComment)
		End Function

		Public Overrides Function GetReturnTypeAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.Signature.ReturnParam.GetAttributes()
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim cachedUseSiteInfo As UseSiteInfo(Of AssemblySymbol)
			If (Me._packedFlags.IsUseSiteDiagnosticPopulated) Then
				cachedUseSiteInfo = Me.GetCachedUseSiteInfo()
			Else
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = MyBase.CalculateUseSiteInfo()
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo.DiagnosticInfo
				Me.EnsureTypeParametersAreLoaded(diagnosticInfo)
				Me.CheckUnmanagedCallersOnly(diagnosticInfo)
				cachedUseSiteInfo = Me.InitializeUseSiteInfo(useSiteInfo.AdjustDiagnosticInfo(diagnosticInfo))
			End If
			Return cachedUseSiteInfo
		End Function

		Private Function InitializeUseSiteInfo(ByVal useSiteInfo As UseSiteInfo(Of AssemblySymbol)) As UseSiteInfo(Of AssemblySymbol)
			Dim cachedUseSiteInfo As UseSiteInfo(Of AssemblySymbol)
			If (Not Me._packedFlags.IsUseSiteDiagnosticPopulated) Then
				If (useSiteInfo.DiagnosticInfo IsNot Nothing OrElse Not useSiteInfo.SecondaryDependencies.IsNullOrEmpty()) Then
					useSiteInfo = Me.AccessUncommonFields()._lazyCachedUseSiteInfo.InterlockedInitialize(MyBase.PrimaryDependency, useSiteInfo)
				End If
				Me._packedFlags.SetIsUseSiteDiagnosticPopulated()
				cachedUseSiteInfo = useSiteInfo
			Else
				cachedUseSiteInfo = Me.GetCachedUseSiteInfo()
			End If
			Return cachedUseSiteInfo
		End Function

		Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Return ' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::IsMetadataNewSlot(System.Boolean)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean IsMetadataNewSlot(System.Boolean)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3501
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 72
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2183
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 84
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1916
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1841
			'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 447
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com


		Friend Overrides Function IsParameterlessConstructor() As Boolean
			Dim flag As Boolean
			If (Me._packedFlags.MethodKindIsPopulated) Then
				flag = If(Me._packedFlags.MethodKind <> Microsoft.CodeAnalysis.MethodKind.Constructor, False, Me.ParameterCount = 0)
			ElseIf (' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEMethodSymbol::IsParameterlessConstructor()
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean IsParameterlessConstructor()
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 1019
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 946
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 797
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 998
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 797
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 998
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 797
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 998
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 797
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 998
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 141
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Expression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 3490
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 127
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 81
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(IEnumerable ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2177
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2139
			'    à Telerik.JustDecompiler.Languages.VisualBasic.VisualBasicWriter.(Action , String ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\VisualBasic\VisualBasicWriter.cs:ligne 455
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 2137
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:ligne 69
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.<>n__0(ICodeNode )
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(Action ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1130
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1084
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(Statement ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1060
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1916
			'    à Telerik.JustDecompiler.Languages.BaseImperativeLanguageWriter.Write(MethodDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseImperativeLanguageWriter.cs:ligne 1841
			'    à ..WriteInternal(IMemberDefinition ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Languages\BaseLanguageWriter.cs:ligne 447
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com


		Private Function IsPotentialOperatorOrConversion(ByVal opInfo As OverloadResolution.OperatorInfo) As Boolean
			If (Not Me.HasSpecialName OrElse Not Me.IsShared OrElse Me.DeclaredAccessibility <> Accessibility.[Public] OrElse Me.IsSub OrElse Me.Arity <> 0) Then
				Return False
			End If
			Return Me.ParameterCount = opInfo.ParamCount
		End Function

		Private Function LoadSignature() As PEMethodSymbol.SignatureData
			Dim signatureHeader As System.Reflection.Metadata.SignatureHeader
			Dim empty As ImmutableArray(Of ParameterSymbol)
			Dim flag As Boolean
			Dim containingPEModule As PEModuleSymbol = Me._containingType.ContainingPEModule
			Dim badImageFormatException As System.BadImageFormatException = Nothing
			Dim signatureForMethod As ParamInfo(Of TypeSymbol)() = (New MetadataDecoder(containingPEModule, Me)).GetSignatureForMethod(Me._handle, signatureHeader, badImageFormatException, True)
			If (Not signatureHeader.IsGeneric AndAlso Me._lazyTypeParameters.IsDefault) Then
				ImmutableInterlocked.InterlockedInitialize(Of TypeParameterSymbol)(Me._lazyTypeParameters, ImmutableArray(Of TypeParameterSymbol).Empty)
			End If
			Dim length As Integer = CInt(signatureForMethod.Length) - 1
			Dim flag1 As Boolean = False
			If (length <= 0) Then
				empty = ImmutableArray(Of ParameterSymbol).Empty
			Else
				Dim parameterSymbols As ImmutableArray(Of ParameterSymbol).Builder = ImmutableArray.CreateBuilder(Of ParameterSymbol)(length)
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					parameterSymbols.Add(Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol.Create(containingPEModule, Me, num1, signatureForMethod(num1 + 1), flag))
					If (flag) Then
						flag1 = True
					End If
					num1 = num1 + 1
				Loop While num1 <= num
				empty = parameterSymbols.ToImmutable()
			End If
			Dim pEParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol.Create(containingPEModule, Me, 0, signatureForMethod(0), flag)
			If (badImageFormatException IsNot Nothing OrElse flag1 OrElse flag) Then
				Me.InitializeUseSiteInfo(New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedMethod1, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(Me) })))
			End If
			Dim signatureDatum As PEMethodSymbol.SignatureData = New PEMethodSymbol.SignatureData(signatureHeader, empty, pEParameterSymbol)
			Return InterlockedOperations.Initialize(Of PEMethodSymbol.SignatureData)(Me._lazySignature, signatureDatum)
		End Function

		Private Function LoadTypeParameters(ByRef errorInfo As DiagnosticInfo) As ImmutableArray(Of TypeParameterSymbol)
			Dim immutable As ImmutableArray(Of TypeParameterSymbol)
			Try
				Dim containingPEModule As PEModuleSymbol = Me._containingType.ContainingPEModule
				Dim genericParametersForMethodOrThrow As GenericParameterHandleCollection = containingPEModule.[Module].GetGenericParametersForMethodOrThrow(Me._handle)
				If (genericParametersForMethodOrThrow.Count <> 0) Then
					Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol).Builder = ImmutableArray.CreateBuilder(Of TypeParameterSymbol)(genericParametersForMethodOrThrow.Count)
					Dim count As Integer = genericParametersForMethodOrThrow.Count - 1
					Dim num As Integer = 0
					Do
						typeParameterSymbols.Add(New PETypeParameterSymbol(containingPEModule, Me, CUShort(num), genericParametersForMethodOrThrow(num)))
						num = num + 1
					Loop While num <= count
					immutable = typeParameterSymbols.ToImmutable()
				Else
					immutable = ImmutableArray(Of TypeParameterSymbol).Empty
				End If
			Catch badImageFormatException As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException)
				errorInfo = ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedMethod1, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(Me) })
				immutable = ImmutableArray(Of TypeParameterSymbol).Empty
				ProjectData.ClearProjectError()
			End Try
			Return immutable
		End Function

		Friend Function SetAssociatedEvent(ByVal eventSymbol As PEEventSymbol, ByVal methodKind As Microsoft.CodeAnalysis.MethodKind) As Boolean
			Return Me.SetAssociatedPropertyOrEvent(eventSymbol, methodKind)
		End Function

		Friend Function SetAssociatedProperty(ByVal propertySymbol As PEPropertySymbol, ByVal methodKind As Microsoft.CodeAnalysis.MethodKind) As Boolean
			Return Me.SetAssociatedPropertyOrEvent(propertySymbol, methodKind)
		End Function

		Private Function SetAssociatedPropertyOrEvent(ByVal propertyOrEventSymbol As Symbol, ByVal methodKind As Microsoft.CodeAnalysis.MethodKind) As Boolean
			Dim flag As Boolean
			If (Me._associatedPropertyOrEventOpt IsNot Nothing) Then
				flag = False
			Else
				Me._associatedPropertyOrEventOpt = propertyOrEventSymbol
				Me._packedFlags.MethodKind = methodKind
				flag = True
			End If
			Return flag
		End Function

		Friend Overrides Function TryGetMeParameter(<Out> ByRef meParameter As ParameterSymbol) As Boolean
			Dim obj As Object
			If (Not Me.IsShared) Then
				Dim uncommonField As PEMethodSymbol.UncommonFields = Me._uncommonFields
				If (uncommonField IsNot Nothing) Then
					obj = uncommonField._lazyMeParameter
				Else
					obj = Nothing
				End If
				If (obj Is Nothing) Then
					obj = InterlockedOperations.Initialize(Of ParameterSymbol)(Me.AccessUncommonFields()._lazyMeParameter, New MeParameterSymbol(Me))
				End If
				meParameter = obj
			Else
				meParameter = Nothing
			End If
			Return True
		End Function

		Private Structure PackedFlags
			Private _bits As Integer

			Private Const s_methodKindOffset As Integer = 0

			Private Const s_methodKindMask As Integer = 31

			Private Const s_methodKindIsPopulatedBit As Integer = 32

			Private Const s_isExtensionMethodBit As Integer = 64

			Private Const s_isExtensionMethodIsPopulatedBit As Integer = 128

			Private Const s_isObsoleteAttributePopulatedBit As Integer = 256

			Private Const s_isCustomAttributesPopulatedBit As Integer = 512

			Private Const s_isUseSiteDiagnosticPopulatedBit As Integer = 1024

			Private Const s_isConditionalAttributePopulatedBit As Integer = 2048

			Private Const s_isInitOnlyBit As Integer = 4096

			Private Const s_isInitOnlyPopulatedBit As Integer = 8192

			Public ReadOnly Property IsConditionalPopulated As Boolean
				Get
					Return (Me._bits And 2048) <> 0
				End Get
			End Property

			Public ReadOnly Property IsCustomAttributesPopulated As Boolean
				Get
					Return (Me._bits And 512) <> 0
				End Get
			End Property

			Public ReadOnly Property IsExtensionMethod As Boolean
				Get
					Return (Me._bits And 64) <> 0
				End Get
			End Property

			Public ReadOnly Property IsExtensionMethodPopulated As Boolean
				Get
					Return (Me._bits And 128) <> 0
				End Get
			End Property

			Public ReadOnly Property IsInitOnly As Boolean
				Get
					Return (Me._bits And 4096) <> 0
				End Get
			End Property

			Public ReadOnly Property IsInitOnlyPopulated As Boolean
				Get
					Return (Me._bits And 8192) <> 0
				End Get
			End Property

			Public ReadOnly Property IsObsoleteAttributePopulated As Boolean
				Get
					Return (Me._bits And 256) <> 0
				End Get
			End Property

			Public ReadOnly Property IsUseSiteDiagnosticPopulated As Boolean
				Get
					Return (Me._bits And 1024) <> 0
				End Get
			End Property

			Public Property MethodKind As Microsoft.CodeAnalysis.MethodKind
				Get
					Return DirectCast((Me._bits >> 0 And 31), Microsoft.CodeAnalysis.MethodKind)
				End Get
				Set(ByVal value As Microsoft.CodeAnalysis.MethodKind)
					Me._bits = Me._bits And -32 Or CInt(value) << CInt(Microsoft.CodeAnalysis.MethodKind.AnonymousFunction) Or 32
				End Set
			End Property

			Public ReadOnly Property MethodKindIsPopulated As Boolean
				Get
					Return (Me._bits And 32) <> 0
				End Get
			End Property

			Private Shared Function BitsAreUnsetOrSame(ByVal bits As Integer, ByVal mask As Integer) As Boolean
				If ((bits And mask) = 0) Then
					Return True
				End If
				Return (bits And mask) = mask
			End Function

			Public Sub InitializeIsExtensionMethod(ByVal isExtensionMethod As Boolean)
				Dim num As Integer = If(isExtensionMethod, 64, 0) Or 128
				ThreadSafeFlagOperations.[Set](Me._bits, num)
			End Sub

			Public Sub InitializeIsInitOnly(ByVal isInitOnly As Boolean)
				Dim num As Integer = If(isInitOnly, 4096, 0) Or 8192
				ThreadSafeFlagOperations.[Set](Me._bits, num)
			End Sub

			Public Sub InitializeMethodKind(ByVal methodKind As Microsoft.CodeAnalysis.MethodKind)
				Dim num As Integer = CInt((methodKind And (Microsoft.CodeAnalysis.MethodKind.Constructor Or Microsoft.CodeAnalysis.MethodKind.Conversion Or Microsoft.CodeAnalysis.MethodKind.DelegateInvoke Or Microsoft.CodeAnalysis.MethodKind.Destructor Or Microsoft.CodeAnalysis.MethodKind.EventAdd Or Microsoft.CodeAnalysis.MethodKind.EventRaise Or Microsoft.CodeAnalysis.MethodKind.EventRemove Or Microsoft.CodeAnalysis.MethodKind.ExplicitInterfaceImplementation Or Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator Or Microsoft.CodeAnalysis.MethodKind.Ordinary Or Microsoft.CodeAnalysis.MethodKind.PropertyGet Or Microsoft.CodeAnalysis.MethodKind.PropertySet Or Microsoft.CodeAnalysis.MethodKind.ReducedExtension Or Microsoft.CodeAnalysis.MethodKind.StaticConstructor Or Microsoft.CodeAnalysis.MethodKind.SharedConstructor Or Microsoft.CodeAnalysis.MethodKind.BuiltinOperator Or Microsoft.CodeAnalysis.MethodKind.DeclareMethod Or Microsoft.CodeAnalysis.MethodKind.LocalFunction Or Microsoft.CodeAnalysis.MethodKind.FunctionPointerSignature))) << CInt(Microsoft.CodeAnalysis.MethodKind.AnonymousFunction) Or 32
				ThreadSafeFlagOperations.[Set](Me._bits, num)
			End Sub

			Public Sub SetIsConditionalAttributePopulated()
				ThreadSafeFlagOperations.[Set](Me._bits, 2048)
			End Sub

			Public Sub SetIsCustomAttributesPopulated()
				ThreadSafeFlagOperations.[Set](Me._bits, 512)
			End Sub

			Public Sub SetIsObsoleteAttributePopulated()
				ThreadSafeFlagOperations.[Set](Me._bits, 256)
			End Sub

			Public Sub SetIsUseSiteDiagnosticPopulated()
				ThreadSafeFlagOperations.[Set](Me._bits, 1024)
			End Sub
		End Structure

		Private Class SignatureData
			Public ReadOnly Header As SignatureHeader

			Public ReadOnly Parameters As ImmutableArray(Of ParameterSymbol)

			Public ReadOnly ReturnParam As PEParameterSymbol

			Public Sub New(ByVal signatureHeader As System.Reflection.Metadata.SignatureHeader, ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal returnParam As PEParameterSymbol)
				MyBase.New()
				Me.Header = signatureHeader
				Me.Parameters = parameters
				Me.ReturnParam = returnParam
			End Sub
		End Class

		Private NotInheritable Class UncommonFields
			Public _lazyMeParameter As ParameterSymbol

			Public _lazyDocComment As Tuple(Of CultureInfo, String)

			Public _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

			Public _lazyConditionalAttributeSymbols As ImmutableArray(Of String)

			Public _lazyObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData

			Public _lazyCachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

			Public Sub New()
				MyBase.New()
			End Sub
		End Class
	End Class
End Namespace
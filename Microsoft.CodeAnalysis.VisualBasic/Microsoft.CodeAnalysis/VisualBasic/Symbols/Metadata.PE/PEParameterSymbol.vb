Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend Class PEParameterSymbol
		Inherits ParameterSymbol
		Private ReadOnly _containingSymbol As Symbol

		Private ReadOnly _name As String

		Private ReadOnly _type As TypeSymbol

		Private ReadOnly _handle As ParameterHandle

		Private ReadOnly _flags As ParameterAttributes

		Private ReadOnly _ordinal As UShort

		Private ReadOnly _packed As Byte

		Private Const s_isByRefMask As Integer = 1

		Private Const s_hasNameInMetadataMask As Integer = 2

		Private Const s_hasOptionCompareMask As Integer = 4

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyDefaultValue As ConstantValue

		Private _lazyHasIDispatchConstantAttribute As ThreeState

		Private _lazyHasIUnknownConstantAttribute As ThreeState

		Private _lazyHasCallerLineNumberAttribute As ThreeState

		Private _lazyHasCallerMemberNameAttribute As ThreeState

		Private _lazyHasCallerFilePathAttribute As ThreeState

		Private _lazyIsParamArray As ThreeState

		Private _lazyHiddenAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
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

		Friend Overrides ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue
			Get
				' 
				' Current member / type: Microsoft.CodeAnalysis.ConstantValue Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol::get_ExplicitDefaultConstantValue(Microsoft.CodeAnalysis.VisualBasic.SymbolsInProgress`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol>)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: Microsoft.CodeAnalysis.ConstantValue get_ExplicitDefaultConstantValue(Microsoft.CodeAnalysis.VisualBasic.SymbolsInProgress<Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol>)
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

		Friend ReadOnly Property Handle As ParameterHandle
			Get
				Return Me._handle
			End Get
		End Property

		Public Overrides ReadOnly Property HasExplicitDefaultValue As Boolean
			Get
				If (Not Me.IsOptional) Then
					Return False
				End If
				Return CObj(MyBase.ExplicitDefaultConstantValue) <> CObj(Nothing)
			End Get
		End Property

		Friend Overrides ReadOnly Property HasMetadataConstantValue As Boolean
			Get
				Return (Me._flags And ParameterAttributes.HasDefault) <> ParameterAttributes.None
			End Get
		End Property

		Private ReadOnly Property HasNameInMetadata As Boolean
			Get
				Return ' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol::HasNameInMetadata()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean HasNameInMetadata()
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


		Friend Overrides ReadOnly Property HasOptionCompare As Boolean
			Get
				Return ' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol::HasOptionCompare()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean HasOptionCompare()
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


		Public Overrides ReadOnly Property IsByRef As Boolean
			Get
				Return ' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol::IsByRef()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean IsByRef()
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


		Friend Overrides ReadOnly Property IsCallerFilePath As Boolean
			Get
				If (Me._lazyHasCallerFilePathAttribute = ThreeState.Unknown) Then
					Me._lazyHasCallerFilePathAttribute = Me.PEModule.HasAttribute(Me._handle, AttributeDescription.CallerFilePathAttribute).ToThreeState()
				End If
				Return Me._lazyHasCallerFilePathAttribute.Value()
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerLineNumber As Boolean
			Get
				If (Me._lazyHasCallerLineNumberAttribute = ThreeState.Unknown) Then
					Me._lazyHasCallerLineNumberAttribute = Me.PEModule.HasAttribute(Me._handle, AttributeDescription.CallerLineNumberAttribute).ToThreeState()
				End If
				Return Me._lazyHasCallerLineNumberAttribute.Value()
			End Get
		End Property

		Friend Overrides ReadOnly Property IsCallerMemberName As Boolean
			Get
				If (Me._lazyHasCallerMemberNameAttribute = ThreeState.Unknown) Then
					Me._lazyHasCallerMemberNameAttribute = Me.PEModule.HasAttribute(Me._handle, AttributeDescription.CallerMemberNameAttribute).ToThreeState()
				End If
				Return Me._lazyHasCallerMemberNameAttribute.Value()
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExplicitByRef As Boolean
			Get
				Return Me.IsByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
			Get
				If (Me._lazyHasIDispatchConstantAttribute = ThreeState.Unknown) Then
					Me._lazyHasIDispatchConstantAttribute = Me.PEModule.HasAttribute(Me._handle, AttributeDescription.IDispatchConstantAttribute).ToThreeState()
				End If
				Return Me._lazyHasIDispatchConstantAttribute.Value()
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
			Get
				If (Me._lazyHasIUnknownConstantAttribute = ThreeState.Unknown) Then
					Me._lazyHasIUnknownConstantAttribute = Me.PEModule.HasAttribute(Me._handle, AttributeDescription.IUnknownConstantAttribute).ToThreeState()
				End If
				Return Me._lazyHasIUnknownConstantAttribute.Value()
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMarshalledExplicitly As Boolean
			Get
				Return (Me._flags And ParameterAttributes.HasFieldMarshal) <> ParameterAttributes.None
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataIn As Boolean
			Get
				Return (Me._flags And ParameterAttributes.[In]) <> ParameterAttributes.None
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataOptional As Boolean
			Get
				Return (Me._flags And ParameterAttributes.[Optional]) <> ParameterAttributes.None
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataOut As Boolean
			Get
				Return (Me._flags And ParameterAttributes.Out) <> ParameterAttributes.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsOptional As Boolean
			Get
				Return (Me._flags And ParameterAttributes.[Optional]) <> ParameterAttributes.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsParamArray As Boolean
			Get
				If (Not Me._lazyIsParamArray.HasValue()) Then
					Me._lazyIsParamArray = Me.PEModule.HasParamsAttribute(Me._handle).ToThreeState()
				End If
				Return Me._lazyIsParamArray.Value()
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._containingSymbol.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				' 
				' Current member / type: System.Collections.Immutable.ImmutableArray`1<System.Byte> Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol::get_MarshallingDescriptor()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Collections.Immutable.ImmutableArray<System.Byte> get_MarshallingDescriptor()
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

		Friend Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingType As UnmanagedType
			Get
				' 
				' Current member / type: System.Runtime.InteropServices.UnmanagedType Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEParameterSymbol::get_MarshallingType()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Runtime.InteropServices.UnmanagedType get_MarshallingType()
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

		Public Overrides ReadOnly Property MetadataName As String
			Get
				If (Not Me.HasNameInMetadata) Then
					Return [String].Empty
				End If
				Return Me._name
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

		Friend ReadOnly Property ParamFlags As ParameterAttributes
			Get
				Return Me._flags
			End Get
		End Property

		Private ReadOnly Property PEModule As Microsoft.CodeAnalysis.PEModule
			Get
				Return DirectCast(Me._containingSymbol.ContainingModule, PEModuleSymbol).[Module]
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Private Sub New(ByVal containingSymbol As Symbol, ByVal name As String, ByVal isByRef As Boolean, ByVal type As TypeSymbol, ByVal handle As ParameterHandle, ByVal flags As ParameterAttributes, ByVal isParamArray As Boolean, ByVal hasOptionCompare As Boolean, ByVal ordinal As Integer, ByVal defaultValue As ConstantValue)
			MyBase.New()
			Dim flag As Boolean
			Me._lazyDefaultValue = ConstantValue.Unset
			Me._lazyHasIDispatchConstantAttribute = ThreeState.Unknown
			Me._lazyHasIUnknownConstantAttribute = ThreeState.Unknown
			Me._lazyHasCallerLineNumberAttribute = ThreeState.Unknown
			Me._lazyHasCallerMemberNameAttribute = ThreeState.Unknown
			Me._lazyHasCallerFilePathAttribute = ThreeState.Unknown
			Me._containingSymbol = containingSymbol
			Me._name = PEParameterSymbol.EnsureParameterNameNotEmpty(name, flag)
			Me._type = TupleTypeDecoder.DecodeTupleTypesIfApplicable(type, handle, DirectCast(containingSymbol.ContainingModule, PEModuleSymbol))
			Me._handle = handle
			Me._ordinal = CUShort(ordinal)
			Me._flags = flags
			Me._lazyIsParamArray = isParamArray.ToThreeState()
			Me._lazyDefaultValue = defaultValue
			Me._packed = PEParameterSymbol.Pack(isByRef, flag, hasOptionCompare)
		End Sub

		Private Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingSymbol As Symbol, ByVal ordinal As Integer, ByVal isByRef As Boolean, ByVal type As TypeSymbol, ByVal handle As ParameterHandle, <Out> ByRef isBad As Boolean)
			MyBase.New()
			Dim flag As Boolean
			Me._lazyDefaultValue = ConstantValue.Unset
			Me._lazyHasIDispatchConstantAttribute = ThreeState.Unknown
			Me._lazyHasIUnknownConstantAttribute = ThreeState.Unknown
			Me._lazyHasCallerLineNumberAttribute = ThreeState.Unknown
			Me._lazyHasCallerMemberNameAttribute = ThreeState.Unknown
			Me._lazyHasCallerFilePathAttribute = ThreeState.Unknown
			isBad = False
			Me._containingSymbol = containingSymbol
			Me._type = TupleTypeDecoder.DecodeTupleTypesIfApplicable(type, handle, moduleSymbol)
			Me._ordinal = CUShort(ordinal)
			Me._handle = handle
			Dim flag1 As Boolean = False
			If (Not handle.IsNil) Then
				Try
					moduleSymbol.[Module].GetParamPropsOrThrow(handle, Me._name, Me._flags)
				Catch badImageFormatException As System.BadImageFormatException
					ProjectData.SetProjectError(badImageFormatException)
					isBad = True
					ProjectData.ClearProjectError()
				End Try
				flag1 = moduleSymbol.[Module].HasAttribute(handle, AttributeDescription.OptionCompareAttribute)
			Else
				Me._lazyCustomAttributes = ImmutableArray(Of VisualBasicAttributeData).Empty
				Me._lazyHiddenAttributes = ImmutableArray(Of VisualBasicAttributeData).Empty
				Me._lazyHasIDispatchConstantAttribute = ThreeState.[False]
				Me._lazyHasIUnknownConstantAttribute = ThreeState.[False]
				Me._lazyDefaultValue = Nothing
				Me._lazyHasCallerLineNumberAttribute = ThreeState.[False]
				Me._lazyHasCallerMemberNameAttribute = ThreeState.[False]
				Me._lazyHasCallerFilePathAttribute = ThreeState.[False]
				Me._lazyIsParamArray = ThreeState.[False]
			End If
			Me._name = PEParameterSymbol.EnsureParameterNameNotEmpty(Me._name, flag)
			Me._packed = PEParameterSymbol.Pack(isByRef, flag, flag1)
		End Sub

		Friend Shared Function Create(ByVal moduleSymbol As PEModuleSymbol, ByVal containingSymbol As PEMethodSymbol, ByVal ordinal As Integer, ByRef parameter As ParamInfo(Of TypeSymbol), <Out> ByRef isBad As Boolean) As PEParameterSymbol
			Return PEParameterSymbol.Create(moduleSymbol, containingSymbol, ordinal, parameter.IsByRef, parameter.RefCustomModifiers, parameter.Type, parameter.Handle, parameter.CustomModifiers, isBad)
		End Function

		Friend Shared Function Create(ByVal containingSymbol As Symbol, ByVal name As String, ByVal isByRef As Boolean, ByVal refCustomModifiers As ImmutableArray(Of CustomModifier), ByVal type As TypeSymbol, ByVal handle As ParameterHandle, ByVal flags As ParameterAttributes, ByVal isParamArray As Boolean, ByVal hasOptionCompare As Boolean, ByVal ordinal As Integer, ByVal defaultValue As ConstantValue, ByVal customModifiers As ImmutableArray(Of CustomModifier)) As PEParameterSymbol
			Dim pEParameterSymbolWithCustomModifier As PEParameterSymbol
			If (Not customModifiers.IsEmpty OrElse Not refCustomModifiers.IsEmpty) Then
				pEParameterSymbolWithCustomModifier = New PEParameterSymbol.PEParameterSymbolWithCustomModifiers(containingSymbol, name, isByRef, refCustomModifiers, type, handle, flags, isParamArray, hasOptionCompare, ordinal, defaultValue, customModifiers)
			Else
				pEParameterSymbolWithCustomModifier = New PEParameterSymbol(containingSymbol, name, isByRef, type, handle, flags, isParamArray, hasOptionCompare, ordinal, defaultValue)
			End If
			Return pEParameterSymbolWithCustomModifier
		End Function

		Private Shared Function Create(ByVal moduleSymbol As PEModuleSymbol, ByVal containingSymbol As Symbol, ByVal ordinal As Integer, ByVal isByRef As Boolean, ByVal refCustomModifiers As ImmutableArray(Of ModifierInfo(Of TypeSymbol)), ByVal type As TypeSymbol, ByVal handle As ParameterHandle, ByVal customModifiers As ImmutableArray(Of ModifierInfo(Of TypeSymbol)), <Out> ByRef isBad As Boolean) As PEParameterSymbol
			Dim pEParameterSymbolWithCustomModifier As PEParameterSymbol
			If (Not customModifiers.IsDefaultOrEmpty OrElse Not refCustomModifiers.IsDefaultOrEmpty) Then
				pEParameterSymbolWithCustomModifier = New PEParameterSymbol.PEParameterSymbolWithCustomModifiers(moduleSymbol, containingSymbol, ordinal, isByRef, refCustomModifiers, type, handle, customModifiers, isBad)
			Else
				pEParameterSymbolWithCustomModifier = New PEParameterSymbol(moduleSymbol, containingSymbol, ordinal, isByRef, type, handle, isBad)
			End If
			Return pEParameterSymbolWithCustomModifier
		End Function

		Private Shared Function EnsureParameterNameNotEmpty(ByVal name As String, <Out> ByRef hasNameInMetadata As Boolean) As String
			hasNameInMetadata = Not [String].IsNullOrEmpty(name)
			If (Not hasNameInMetadata) Then
				Return "Param"
			End If
			Return name
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Dim customAttributeHandle As System.Reflection.Metadata.CustomAttributeHandle
			Dim customAttributeHandle1 As System.Reflection.Metadata.CustomAttributeHandle
			If (Me._lazyCustomAttributes.IsDefault) Then
				Dim containingModule As PEModuleSymbol = DirectCast(Me._containingSymbol.ContainingModule, PEModuleSymbol)
				Dim flag As Boolean = If(Not Me._lazyIsParamArray.HasValue(), True, Me._lazyIsParamArray.Value())
				Dim explicitDefaultConstantValue As ConstantValue = MyBase.ExplicitDefaultConstantValue
				Dim dateTimeConstantAttribute As AttributeDescription = New AttributeDescription()
				If (explicitDefaultConstantValue IsNot Nothing) Then
					If (explicitDefaultConstantValue.Discriminator = ConstantValueTypeDiscriminator.DateTime) Then
						dateTimeConstantAttribute = AttributeDescription.DateTimeConstantAttribute
					ElseIf (explicitDefaultConstantValue.Discriminator = ConstantValueTypeDiscriminator.[Decimal]) Then
						dateTimeConstantAttribute = AttributeDescription.DecimalConstantAttribute
					End If
				End If
				If (flag OrElse dateTimeConstantAttribute.Signatures IsNot Nothing) Then
					Dim customAttributesForToken As ImmutableArray(Of VisualBasicAttributeData) = containingModule.GetCustomAttributesForToken(Me._handle, customAttributeHandle, If(flag, AttributeDescription.ParamArrayAttribute, New AttributeDescription()), customAttributeHandle1, dateTimeConstantAttribute)
					If (Not customAttributeHandle.IsNil OrElse Not customAttributeHandle1.IsNil) Then
						Dim instance As ArrayBuilder(Of VisualBasicAttributeData) = ArrayBuilder(Of VisualBasicAttributeData).GetInstance()
						If (Not customAttributeHandle.IsNil) Then
							instance.Add(New PEAttributeData(containingModule, customAttributeHandle))
						End If
						If (Not customAttributeHandle1.IsNil) Then
							instance.Add(New PEAttributeData(containingModule, customAttributeHandle1))
						End If
						ImmutableInterlocked.InterlockedInitialize(Of VisualBasicAttributeData)(Me._lazyHiddenAttributes, instance.ToImmutableAndFree())
					Else
						ImmutableInterlocked.InterlockedInitialize(Of VisualBasicAttributeData)(Me._lazyHiddenAttributes, ImmutableArray(Of VisualBasicAttributeData).Empty)
					End If
					If (Not Me._lazyIsParamArray.HasValue()) Then
						Me._lazyIsParamArray = (Not customAttributeHandle.IsNil).ToThreeState()
					End If
					ImmutableInterlocked.InterlockedInitialize(Of VisualBasicAttributeData)(Me._lazyCustomAttributes, customAttributesForToken)
				Else
					ImmutableInterlocked.InterlockedInitialize(Of VisualBasicAttributeData)(Me._lazyHiddenAttributes, ImmutableArray(Of VisualBasicAttributeData).Empty)
					containingModule.LoadCustomAttributes(Me._handle, Me._lazyCustomAttributes)
				End If
			End If
			Return Me._lazyCustomAttributes
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return New PEParameterSymbol.VB$StateMachine_46_GetCustomAttributesToEmit(-2) With
			{
				.$VB$Me = Me,
				.$P_compilationState = compilationState
			}
		End Function

		Private Shared Function Pack(ByVal isByRef As Boolean, ByVal hasNameInMetadata As Boolean, ByVal hasOptionCompare As Boolean) As Byte
			Dim num As Integer
			num = If(isByRef, 1, 0)
			Dim num1 As Integer = If(hasNameInMetadata, 2, 0)
			Return CByte((num Or num1 Or If(hasOptionCompare, 4, 0)))
		End Function

		Private NotInheritable Class PEParameterSymbolWithCustomModifiers
			Inherits PEParameterSymbol
			Private ReadOnly _customModifiers As ImmutableArray(Of CustomModifier)

			Private ReadOnly _refCustomModifiers As ImmutableArray(Of CustomModifier)

			Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._customModifiers
				End Get
			End Property

			Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._refCustomModifiers
				End Get
			End Property

			Public Sub New(ByVal containingSymbol As Symbol, ByVal name As String, ByVal isByRef As Boolean, ByVal refCustomModifiers As ImmutableArray(Of CustomModifier), ByVal type As TypeSymbol, ByVal handle As ParameterHandle, ByVal flags As ParameterAttributes, ByVal isParamArray As Boolean, ByVal hasOptionCompare As Boolean, ByVal ordinal As Integer, ByVal defaultValue As ConstantValue, ByVal customModifiers As ImmutableArray(Of CustomModifier))
				MyBase.New(containingSymbol, name, isByRef, type, handle, flags, isParamArray, hasOptionCompare, ordinal, defaultValue)
				Me._customModifiers = customModifiers
				Me._refCustomModifiers = refCustomModifiers
			End Sub

			Public Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingSymbol As Symbol, ByVal ordinal As Integer, ByVal isByRef As Boolean, ByVal refCustomModifiers As ImmutableArray(Of ModifierInfo(Of TypeSymbol)), ByVal type As TypeSymbol, ByVal handle As ParameterHandle, ByVal customModifiers As ImmutableArray(Of ModifierInfo(Of TypeSymbol)), <Out> ByRef isBad As Boolean)
				MyBase.New(moduleSymbol, containingSymbol, ordinal, isByRef, type, handle, isBad)
				Me._customModifiers = VisualBasicCustomModifier.Convert(customModifiers)
				Me._refCustomModifiers = VisualBasicCustomModifier.Convert(refCustomModifiers)
			End Sub
		End Class
	End Class
End Namespace
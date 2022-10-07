Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourceDeclareMethodSymbol
		Inherits SourceNonPropertyAccessorMethodSymbol
		Private ReadOnly _name As String

		Private _lazyMetadataName As String

		Private ReadOnly _quickAttributes As QuickAttributes

		Private ReadOnly _platformInvokeInfo As DllImportData

		Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Return MethodImplAttributes.PreserveSig
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return If(Not Me.MayBeReducibleExtensionMethod, False, MyBase.IsExtensionMethod)
			End Get
		End Property

		Public Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property MayBeReducibleExtensionMethod As Boolean
			Get
				Return (Me._quickAttributes And QuickAttributes.Extension) <> QuickAttributes.None
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				If (Me._lazyMetadataName Is Nothing) Then
					OverloadingHelper.SetMetadataNameForAllOverloads(Me._name, SymbolKind.Method, Me.m_containingType)
				End If
				Return Me._lazyMetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				' 
				' Current member / type: Microsoft.CodeAnalysis.ObsoleteAttributeData Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceDeclareMethodSymbol::get_ObsoleteAttributeData()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: Microsoft.CodeAnalysis.ObsoleteAttributeData get_ObsoleteAttributeData()
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

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return ImmutableArray(Of TypeParameterSymbol).Empty
			End Get
		End Property

		Public Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal name As String, ByVal flags As SourceMemberFlags, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal syntax As MethodBaseSyntax, ByVal platformInvokeInfo As DllImportData)
			MyBase.New(container, flags, binder.GetSyntaxReference(syntax), New ImmutableArray(Of Location)())
			Me._platformInvokeInfo = platformInvokeInfo
			Me._name = name
			Me._quickAttributes = binder.QuickAttributeChecker.CheckAttributes(syntax.AttributeLists)
			If (Me.ContainingType.TypeKind <> TypeKind.[Module]) Then
				Me._quickAttributes = Me._quickAttributes And (QuickAttributes.Obsolete Or QuickAttributes.MyGroupCollection Or QuickAttributes.TypeIdentifier)
			End If
		End Sub

		Public Overrides Function GetDllImportData() As DllImportData
			Return Me._platformInvokeInfo
		End Function

		Friend Overrides Sub SetMetadataName(ByVal metadataName As String)
			Interlocked.CompareExchange(Of String)(Me._lazyMetadataName, metadataName, Nothing)
		End Sub
	End Class
End Namespace
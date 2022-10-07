Imports Microsoft.CodeAnalysis
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
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class PEFieldSymbol
		Inherits FieldSymbol
		Private ReadOnly _handle As FieldDefinitionHandle

		Private ReadOnly _name As String

		Private ReadOnly _flags As FieldAttributes

		Private ReadOnly _containingType As PENamedTypeSymbol

		Private _lazyType As TypeSymbol

		Private _lazyCustomModifiers As ImmutableArray(Of CustomModifier)

		Private _lazyConstantValue As Microsoft.CodeAnalysis.ConstantValue

		Private _lazyDocComment As Tuple(Of CultureInfo, String)

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyCachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

		Private _lazyObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
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

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Me.EnsureSignatureIsLoaded()
				Return Me._lazyCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				' 
				' Current member / type: Microsoft.CodeAnalysis.Accessibility Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEFieldSymbol::get_DeclaredAccessibility()
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

		Friend ReadOnly Property FieldFlags As FieldAttributes
			Get
				Return Me._flags
			End Get
		End Property

		Friend ReadOnly Property Handle As FieldDefinitionHandle
			Get
				Return Me._handle
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return (Me._flags And FieldAttributes.RTSpecialName) <> FieldAttributes.PrivateScope
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return (Me._flags And FieldAttributes.SpecialName) <> FieldAttributes.PrivateScope
			End Get
		End Property

		Public Overrides ReadOnly Property IsConst As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEFieldSymbol::get_IsConst()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean get_IsConst()
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

		Friend Overrides ReadOnly Property IsMarshalledExplicitly As Boolean
			Get
				Return (Me._flags And FieldAttributes.HasFieldMarshal) <> FieldAttributes.PrivateScope
			End Get
		End Property

		Friend Overrides ReadOnly Property IsNotSerialized As Boolean
			Get
				Return (Me._flags And FieldAttributes.NotSerialized) <> FieldAttributes.PrivateScope
			End Get
		End Property

		Public Overrides ReadOnly Property IsReadOnly As Boolean
			Get
				Return (Me._flags And FieldAttributes.InitOnly) <> FieldAttributes.PrivateScope
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return (Me._flags And FieldAttributes.[Static]) <> FieldAttributes.PrivateScope
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return StaticCast(Of Location).From(Of MetadataLocation)(Me._containingType.ContainingPEModule.MetadataLocation)
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				' 
				' Current member / type: System.Collections.Immutable.ImmutableArray`1<System.Byte> Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEFieldSymbol::get_MarshallingDescriptor()
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
				' Current member / type: System.Runtime.InteropServices.UnmanagedType Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEFieldSymbol::get_MarshallingType()
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

		Private ReadOnly Property PEModule As Microsoft.CodeAnalysis.PEModule
			Get
				Return DirectCast(Me.ContainingModule, PEModuleSymbol).[Module]
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Me.EnsureSignatureIsLoaded()
				Return Me._lazyType
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)
			Get
				Return Me.PEModule.GetFieldOffset(Me._handle)
			End Get
		End Property

		Friend Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingType As PENamedTypeSymbol, ByVal handle As FieldDefinitionHandle)
			MyBase.New()
			Me._lazyConstantValue = Microsoft.CodeAnalysis.ConstantValue.Unset
			Me._lazyCachedUseSiteInfo = CachedUseSiteInfo(Of AssemblySymbol).Uninitialized
			Me._lazyObsoleteAttributeData = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized
			Me._handle = handle
			Me._containingType = containingType
			Try
				moduleSymbol.[Module].GetFieldDefPropsOrThrow(handle, Me._name, Me._flags)
			Catch badImageFormatException As System.BadImageFormatException
				ProjectData.SetProjectError(badImageFormatException)
				If (Me._name Is Nothing) Then
					Me._name = [String].Empty
				End If
				Dim objArray() As [Object] = { Me }
				Me._lazyCachedUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedField1, objArray))
				ProjectData.ClearProjectError()
			End Try
		End Sub

		Private Sub EnsureSignatureIsLoaded()
			If (Me._lazyType Is Nothing) Then
				Dim containingPEModule As PEModuleSymbol = Me._containingType.ContainingPEModule
				Dim modifierInfos As ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)) = New ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol))()
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = (New MetadataDecoder(containingPEModule, Me._containingType)).DecodeFieldSignature(Me._handle, modifierInfos)
				typeSymbol = TupleTypeDecoder.DecodeTupleTypesIfApplicable(typeSymbol, Me._handle, containingPEModule)
				Dim customModifiers As ImmutableArray(Of CustomModifier) = VisualBasicCustomModifier.Convert(modifierInfos)
				Dim customModifiers1 As ImmutableArray(Of CustomModifier) = New ImmutableArray(Of CustomModifier)()
				ImmutableInterlocked.InterlockedCompareExchange(Of CustomModifier)(Me._lazyCustomModifiers, customModifiers, customModifiers1)
				Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(Me._lazyType, typeSymbol, Nothing)
			End If
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Dim customAttributeHandle As System.Reflection.Metadata.CustomAttributeHandle
			If (Me._lazyCustomAttributes.IsDefault) Then
				Dim containingModule As PEModuleSymbol = DirectCast(Me.ContainingModule, PEModuleSymbol)
				Dim constantAttributeDescription As Microsoft.CodeAnalysis.AttributeDescription = Me.GetConstantAttributeDescription()
				If (constantAttributeDescription.Signatures Is Nothing) Then
					containingModule.LoadCustomAttributes(Me._handle, Me._lazyCustomAttributes)
				Else
					Dim entityHandle As System.Reflection.Metadata.EntityHandle = Me._handle
					Dim customAttributeHandle1 As System.Reflection.Metadata.CustomAttributeHandle = New System.Reflection.Metadata.CustomAttributeHandle()
					Dim attributeDescription As Microsoft.CodeAnalysis.AttributeDescription = New Microsoft.CodeAnalysis.AttributeDescription()
					Dim customAttributesForToken As ImmutableArray(Of VisualBasicAttributeData) = containingModule.GetCustomAttributesForToken(entityHandle, customAttributeHandle, constantAttributeDescription, customAttributeHandle1, attributeDescription)
					ImmutableInterlocked.InterlockedInitialize(Of VisualBasicAttributeData)(Me._lazyCustomAttributes, customAttributesForToken)
				End If
			End If
			Return Me._lazyCustomAttributes
		End Function

		Private Function GetConstantAttributeDescription() As AttributeDescription
			Dim dateTimeConstantAttribute As AttributeDescription
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			If (Me.Type.SpecialType = SpecialType.System_DateTime) Then
				constantValue = Me.GetConstantValue(ConstantFieldsInProgress.Empty)
				If (constantValue Is Nothing OrElse constantValue.Discriminator <> ConstantValueTypeDiscriminator.DateTime) Then
					dateTimeConstantAttribute = New AttributeDescription()
					Return dateTimeConstantAttribute
				End If
				dateTimeConstantAttribute = AttributeDescription.DateTimeConstantAttribute
				Return dateTimeConstantAttribute
			ElseIf (Me.Type.SpecialType = SpecialType.System_Decimal) Then
				constantValue = Me.GetConstantValue(ConstantFieldsInProgress.Empty)
				If (constantValue Is Nothing OrElse constantValue.Discriminator <> ConstantValueTypeDiscriminator.[Decimal]) Then
					dateTimeConstantAttribute = New AttributeDescription()
					Return dateTimeConstantAttribute
				End If
				dateTimeConstantAttribute = AttributeDescription.DecimalConstantAttribute
				Return dateTimeConstantAttribute
			End If
			dateTimeConstantAttribute = New AttributeDescription()
			Return dateTimeConstantAttribute
		End Function

		Friend Overrides Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue
			' 
			' Current member / type: Microsoft.CodeAnalysis.ConstantValue Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.PEFieldSymbol::GetConstantValue(Microsoft.CodeAnalysis.VisualBasic.ConstantFieldsInProgress)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.ConstantValue GetConstantValue(Microsoft.CodeAnalysis.VisualBasic.ConstantFieldsInProgress)
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

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return New PEFieldSymbol.VB$StateMachine_26_GetCustomAttributesToEmit(-2) With
			{
				.$VB$Me = Me,
				.$P_compilationState = compilationState
			}
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return PEDocumentationCommentUtils.GetDocumentationComment(Me, Me._containingType.ContainingPEModule, preferredCulture, cancellationToken, Me._lazyDocComment)
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim primaryDependency As AssemblySymbol = MyBase.PrimaryDependency
			If (Not Me._lazyCachedUseSiteInfo.IsInitialized) Then
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = MyBase.CalculateUseSiteInfo()
				If (useSiteInfo.DiagnosticInfo Is Nothing) Then
					Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Me.GetConstantValue(ConstantFieldsInProgress.Empty)
					If (constantValue IsNot Nothing AndAlso constantValue.IsBad) Then
						useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(New DiagnosticInfo(MessageProvider.Instance, 30799, New [Object]() { Me.ContainingType, Me.Name }))
					End If
				End If
				Me._lazyCachedUseSiteInfo.Initialize(primaryDependency, useSiteInfo)
			End If
			Return Me._lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency)
		End Function
	End Class
End Namespace
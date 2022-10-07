Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class FieldSymbol
		Inherits Symbol
		Implements IFieldReference, IFieldDefinition, ITypeMemberReference, ITypeDefinitionMember, ISpecializedFieldReference, IFieldSymbol, IFieldSymbolInternal
		Friend ReadOnly Property AdaptedFieldSymbol As FieldSymbol
			Get
				Return Me
			End Get
		End Property

		Public MustOverride ReadOnly Property AssociatedSymbol As Symbol

		Public Overridable ReadOnly Property ConstantValue As Object
			Get
				Dim obj As Object
				If (Me.IsConst) Then
					Dim constantValue1 As Microsoft.CodeAnalysis.ConstantValue = Me.GetConstantValue(ConstantFieldsInProgress.Empty)
					obj = If(constantValue1 IsNot Nothing, constantValue1.Value, Nothing)
				Else
					obj = Nothing
				End If
				Return obj
			End Get
		End Property

		Public Overridable ReadOnly Property CorrespondingTupleField As FieldSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				Return Me.ContainingSymbol.EmbeddedSymbolKind
			End Get
		End Property

		Public Overridable ReadOnly Property HasConstantValue As Boolean
			Get
				Dim flag As Boolean
				If (Me.IsConst) Then
					Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Me.GetConstantValue(ConstantFieldsInProgress.Empty)
					flag = If(constantValue Is Nothing, False, Not constantValue.IsBad)
				Else
					flag = False
				End If
				Return flag
			End Get
		End Property

		Friend Overridable ReadOnly Property HasDeclaredType As Boolean
			Get
				Return True
			End Get
		End Property

		Friend MustOverride ReadOnly Property HasRuntimeSpecialName As Boolean

		Friend MustOverride ReadOnly Property HasSpecialName As Boolean

		Public NotOverridable Overrides ReadOnly Property HasUnsupportedMetadata As Boolean
			Get
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = Me.GetUseSiteInfo().DiagnosticInfo
				If (diagnosticInfo Is Nothing) Then
					Return False
				End If
				Return diagnosticInfo.Code = 30656
			End Get
		End Property

		Protected Overrides ReadOnly Property HighestPriorityUseSiteError As Integer
			Get
				Return 30656
			End Get
		End Property

		ReadOnly Property IFieldDefinitionFieldMapping As ImmutableArray(Of Byte) Implements IFieldDefinition.MappedData
			Get
				Return New ImmutableArray(Of Byte)()
			End Get
		End Property

		ReadOnly Property IFieldDefinitionIsCompileTimeConstant As Boolean Implements IFieldDefinition.IsCompileTimeConstant
			Get
				Return If(Not Me.AdaptedFieldSymbol.IsMetadataConstant, False, True)
			End Get
		End Property

		ReadOnly Property IFieldDefinitionIsMarshalledExplicitly As Boolean Implements IFieldDefinition.IsMarshalledExplicitly
			Get
				Return Me.AdaptedFieldSymbol.IsMarshalledExplicitly
			End Get
		End Property

		ReadOnly Property IFieldDefinitionIsNotSerialized As Boolean Implements IFieldDefinition.IsNotSerialized
			Get
				Return Me.AdaptedFieldSymbol.IsNotSerialized
			End Get
		End Property

		ReadOnly Property IFieldDefinitionIsReadOnly As Boolean Implements IFieldDefinition.IsReadOnly
			Get
				If (Me.AdaptedFieldSymbol.IsReadOnly) Then
					Return True
				End If
				Return Me.AdaptedFieldSymbol.IsConstButNotMetadataConstant
			End Get
		End Property

		ReadOnly Property IFieldDefinitionIsRuntimeSpecial As Boolean Implements IFieldDefinition.IsRuntimeSpecial
			Get
				Return Me.AdaptedFieldSymbol.HasRuntimeSpecialName
			End Get
		End Property

		ReadOnly Property IFieldDefinitionIsSpecialName As Boolean Implements IFieldDefinition.IsSpecialName
			Get
				Return Me.AdaptedFieldSymbol.HasSpecialName
			End Get
		End Property

		ReadOnly Property IFieldDefinitionIsStatic As Boolean Implements IFieldDefinition.IsStatic
			Get
				Return Me.AdaptedFieldSymbol.IsShared
			End Get
		End Property

		ReadOnly Property IFieldDefinitionMarshallingDescriptor As ImmutableArray(Of Byte) Implements IFieldDefinition.MarshallingDescriptor
			Get
				Return Me.AdaptedFieldSymbol.MarshallingDescriptor
			End Get
		End Property

		ReadOnly Property IFieldDefinitionMarshallingInformation As IMarshallingInformation Implements IFieldDefinition.MarshallingInformation
			Get
				Return Me.AdaptedFieldSymbol.MarshallingInformation
			End Get
		End Property

		ReadOnly Property IFieldDefinitionOffset As Integer Implements IFieldDefinition.Offset
			Get
				Return Me.AdaptedFieldSymbol.TypeLayoutOffset.GetValueOrDefault()
			End Get
		End Property

		ReadOnly Property IFieldReference_IsContextualNamedEntity As Boolean Implements IFieldReference.IsContextualNamedEntity
			Get
				Return Me.AdaptedFieldSymbol.IsContextualNamedEntity
			End Get
		End Property

		ReadOnly Property IFieldReferenceAsSpecializedFieldReference As ISpecializedFieldReference Implements IFieldReference.AsSpecializedFieldReference
			Get
				Dim specializedFieldReference As ISpecializedFieldReference
				If (Me.AdaptedFieldSymbol.IsDefinition) Then
					specializedFieldReference = Nothing
				Else
					specializedFieldReference = Me
				End If
				Return specializedFieldReference
			End Get
		End Property

		ReadOnly Property IFieldSymbol_AssociatedSymbol As ISymbol Implements IFieldSymbol.AssociatedSymbol
			Get
				Return Me.AssociatedSymbol
			End Get
		End Property

		ReadOnly Property IFieldSymbol_ConstantValue As Object Implements IFieldSymbol.ConstantValue
			Get
				Return Me.ConstantValue
			End Get
		End Property

		ReadOnly Property IFieldSymbol_CorrespondingTupleField As IFieldSymbol Implements IFieldSymbol.CorrespondingTupleField
			Get
				Return Me.CorrespondingTupleField
			End Get
		End Property

		ReadOnly Property IFieldSymbol_CustomModifiers As ImmutableArray(Of CustomModifier) Implements IFieldSymbol.CustomModifiers
			Get
				Return Me.CustomModifiers
			End Get
		End Property

		ReadOnly Property IFieldSymbol_HasConstantValue As Boolean Implements IFieldSymbol.HasConstantValue
			Get
				Return Me.HasConstantValue
			End Get
		End Property

		ReadOnly Property IFieldSymbol_HasExplicitTupleElementName As Boolean Implements IFieldSymbol.IsExplicitlyNamedTupleElement
			Get
				Return Not Me.IsImplicitlyDeclared
			End Get
		End Property

		ReadOnly Property IFieldSymbol_IsConst As Boolean Implements IFieldSymbol.IsConst
			Get
				Return Me.IsConst
			End Get
		End Property

		ReadOnly Property IFieldSymbol_IsFixedSizeBuffer As Boolean Implements IFieldSymbol.IsFixedSizeBuffer
			Get
				Return False
			End Get
		End Property

		ReadOnly Property IFieldSymbol_IsVolatile As Boolean Implements IFieldSymbol.IsVolatile, IFieldSymbolInternal.IsVolatile
			Get
				Return False
			End Get
		End Property

		ReadOnly Property IFieldSymbol_NullableAnnotation As Microsoft.CodeAnalysis.NullableAnnotation Implements IFieldSymbol.NullableAnnotation
			Get
				Return Microsoft.CodeAnalysis.NullableAnnotation.None
			End Get
		End Property

		ReadOnly Property IFieldSymbol_OriginalDefinition As IFieldSymbol Implements IFieldSymbol.OriginalDefinition
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		ReadOnly Property IFieldSymbol_Type As ITypeSymbol Implements IFieldSymbol.Type
			Get
				Return Me.Type
			End Get
		End Property

		ReadOnly Property INamedEntityName As String
			Get
				Return Me.AdaptedFieldSymbol.MetadataName
			End Get
		End Property

		Friend Overridable ReadOnly Property IsCapturedFrame As Boolean
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property IsConst As Boolean

		Friend ReadOnly Property IsConstButNotMetadataConstant As Boolean
			Get
				Dim flag As Boolean
				If (Not Me.IsConst) Then
					flag = False
				Else
					Dim specialType As Microsoft.CodeAnalysis.SpecialType = Me.Type.SpecialType
					flag = If(specialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime, True, specialType = Microsoft.CodeAnalysis.SpecialType.System_Decimal)
				End If
				Return flag
			End Get
		End Property

		Friend Overridable ReadOnly Property IsContextualNamedEntity As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overridable ReadOnly Property IsDefaultTupleElement As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overridable ReadOnly Property IsMarshalledExplicitly As Boolean
			Get
				Return Me.MarshallingInformation IsNot Nothing
			End Get
		End Property

		Friend ReadOnly Property IsMetadataConstant As Boolean
			Get
				Dim flag As Boolean
				If (Not Me.IsConst) Then
					flag = False
				Else
					Dim specialType As Microsoft.CodeAnalysis.SpecialType = Me.Type.SpecialType
					flag = If(specialType = Microsoft.CodeAnalysis.SpecialType.System_DateTime, False, specialType <> Microsoft.CodeAnalysis.SpecialType.System_Decimal)
				End If
				Return flag
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsNotSerialized As Boolean

		Public NotOverridable Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ISpecializedFieldReferenceUnspecializedVersion As IFieldReference Implements ISpecializedFieldReference.UnspecializedVersion
			Get
				Return Me.AdaptedFieldSymbol.OriginalDefinition.GetCciAdapter()
			End Get
		End Property

		Public MustOverride ReadOnly Property IsReadOnly As Boolean Implements IFieldSymbol.IsReadOnly

		Public Overridable ReadOnly Property IsTupleField As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overridable ReadOnly Property IsVirtualTupleField As Boolean
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ITypeDefinitionMemberContainingTypeDefinition As ITypeDefinition Implements ITypeDefinitionMember.ContainingTypeDefinition
			Get
				Return Me.AdaptedFieldSymbol.ContainingType.GetCciAdapter()
			End Get
		End Property

		ReadOnly Property ITypeDefinitionMemberVisibility As TypeMemberVisibility Implements ITypeDefinitionMember.Visibility
			Get
				Return PEModuleBuilder.MemberVisibility(Me.AdaptedFieldSymbol)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.Field
			End Get
		End Property

		Friend Overridable ReadOnly Property MarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				Return New ImmutableArray(Of Byte)()
			End Get
		End Property

		Friend MustOverride ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData

		Friend Overridable ReadOnly Property MarshallingType As UnmanagedType
			Get
				Dim marshallingInformation As MarshalPseudoCustomAttributeData = Me.MarshallingInformation
				If (marshallingInformation Is Nothing) Then
					Return 0
				End If
				Return marshallingInformation.UnmanagedType
			End Get
		End Property

		Friend Overridable ReadOnly Property MeParameter As ParameterSymbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Shadows Overridable ReadOnly Property OriginalDefinition As FieldSymbol
			Get
				Return Me
			End Get
		End Property

		Protected NotOverridable Overrides ReadOnly Property OriginalSymbolDefinition As Symbol
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		Public Overridable ReadOnly Property TupleElementIndex As Integer
			Get
				Return -1
			End Get
		End Property

		Public Overridable ReadOnly Property TupleUnderlyingField As FieldSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property Type As TypeSymbol

		Friend MustOverride ReadOnly Property TypeLayoutOffset As Nullable(Of Integer)

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitField(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitField(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitField(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitField(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitField(Me)
		End Function

		Friend Function AsMember(ByVal newOwner As NamedTypeSymbol) As FieldSymbol
			If (TypeSymbol.Equals(newOwner, Me.ContainingType, TypeCompareKind.ConsiderEverything)) Then
				Return Me
			End If
			Return DirectCast(DirectCast(newOwner, SubstitutedNamedType).GetMemberForDefinition(Me), FieldSymbol)
		End Function

		Friend Function CalculateUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			Dim code As Boolean
			Dim flag As Boolean
			Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = MyBase.MergeUseSiteInfo(New UseSiteInfo(Of AssemblySymbol)(MyBase.PrimaryDependency), MyBase.DeriveUseSiteInfoFromType(Me.Type))
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo1.DiagnosticInfo
			If (diagnosticInfo IsNot Nothing) Then
				code = diagnosticInfo.Code = 30656
			Else
				code = False
			End If
			If (Not code) Then
				Dim useSiteInfo2 As UseSiteInfo(Of AssemblySymbol) = MyBase.DeriveUseSiteInfoFromCustomModifiers(Me.CustomModifiers, False)
				Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo2.DiagnosticInfo
				If (diagnosticInfo1 IsNot Nothing) Then
					flag = diagnosticInfo1.Code = 30656
				Else
					flag = False
				End If
				If (Not flag) Then
					useSiteInfo1 = MyBase.MergeUseSiteInfo(useSiteInfo1, useSiteInfo2)
					If (useSiteInfo1.DiagnosticInfo Is Nothing AndAlso Me.ContainingModule.HasUnifiedReferences) Then
						Dim typeSymbols As HashSet(Of TypeSymbol) = Nothing
						Dim unificationUseSiteDiagnosticRecursive As Microsoft.CodeAnalysis.DiagnosticInfo = If(Me.Type.GetUnificationUseSiteDiagnosticRecursive(Me, typeSymbols), Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.CustomModifiers, Me, typeSymbols))
						If (unificationUseSiteDiagnosticRecursive IsNot Nothing) Then
							useSiteInfo1 = New UseSiteInfo(Of AssemblySymbol)(unificationUseSiteDiagnosticRecursive)
						End If
					End If
					useSiteInfo = useSiteInfo1
				Else
					useSiteInfo = useSiteInfo2
				End If
			Else
				useSiteInfo = useSiteInfo1
			End If
			Return useSiteInfo
		End Function

		Friend Shadows Function GetCciAdapter() As FieldSymbol
			Return Me
		End Function

		Friend MustOverride Function GetConstantValue(ByVal inProgress As ConstantFieldsInProgress) As Microsoft.CodeAnalysis.ConstantValue

		Friend Overridable Function GetInferredType(ByVal inProgress As ConstantFieldsInProgress) As TypeSymbol
			Return Me.Type
		End Function

		Friend Function GetMetadataConstantValue(ByVal context As EmitContext) As Microsoft.CodeAnalysis.CodeGen.MetadataConstant
			Dim metadataConstant As Microsoft.CodeAnalysis.CodeGen.MetadataConstant
			If (Not Me.AdaptedFieldSymbol.IsMetadataConstant) Then
				metadataConstant = Nothing
			Else
				metadataConstant = DirectCast(context.[Module], PEModuleBuilder).CreateConstant(Me.AdaptedFieldSymbol.Type, RuntimeHelpers.GetObjectValue(Me.AdaptedFieldSymbol.ConstantValue), DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
			End If
			Return metadataConstant
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			useSiteInfo = If(Not MyBase.IsDefinition, Me.OriginalDefinition.GetUseSiteInfo(), New UseSiteInfo(Of AssemblySymbol)(MyBase.PrimaryDependency))
			Return useSiteInfo
		End Function

		Private Function IFieldDefinition_GetCompileTimeValue(ByVal context As EmitContext) As MetadataConstant Implements IFieldDefinition.GetCompileTimeValue
			Return Me.GetMetadataConstantValue(context)
		End Function

		Private Function IFieldReferenceGetResolvedField(ByVal context As EmitContext) As IFieldDefinition Implements IFieldReference.GetResolvedField
			Return Me.ResolvedFieldImpl(DirectCast(context.[Module], PEModuleBuilder))
		End Function

		Private Function IFieldReferenceGetType(ByVal context As EmitContext) As ITypeReference Implements IFieldReference.[GetType]
			Dim modifiedTypeReference As ITypeReference
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim customModifiers As ImmutableArray(Of CustomModifier) = Me.AdaptedFieldSymbol.CustomModifiers
			Dim typeReference As ITypeReference = [module].Translate(Me.AdaptedFieldSymbol.Type, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
			If (customModifiers.Length <> 0) Then
				modifiedTypeReference = New Microsoft.Cci.ModifiedTypeReference(typeReference, customModifiers.[As](Of ICustomModifier)())
			Else
				modifiedTypeReference = typeReference
			End If
			Return modifiedTypeReference
		End Function

		Friend NotOverridable Overrides Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Return Me.ResolvedFieldImpl(DirectCast(context.[Module], PEModuleBuilder))
		End Function

		Friend NotOverridable Overrides Sub IReferenceDispatch(ByVal visitor As MetadataVisitor)
			If (Not Me.AdaptedFieldSymbol.IsDefinition) Then
				visitor.Visit(DirectCast(Me, IFieldReference))
				Return
			End If
			If (Me.AdaptedFieldSymbol.ContainingModule = DirectCast(visitor.Context.[Module], PEModuleBuilder).SourceModule) Then
				visitor.Visit(Me)
				Return
			End If
			visitor.Visit(DirectCast(Me, IFieldReference))
		End Sub

		Private Function ITypeMemberReferenceGetContainingType(ByVal context As EmitContext) As ITypeReference Implements ITypeMemberReference.GetContainingType
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.AdaptedFieldSymbol.ContainingType, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, Me.AdaptedFieldSymbol.IsDefinition)
		End Function

		Private Function ResolvedFieldImpl(ByVal moduleBeingBuilt As PEModuleBuilder) As IFieldDefinition
			Dim fieldDefinition As IFieldDefinition
			If (Not Me.AdaptedFieldSymbol.IsDefinition OrElse Not (Me.AdaptedFieldSymbol.ContainingModule = moduleBeingBuilt.SourceModule)) Then
				fieldDefinition = Nothing
			Else
				fieldDefinition = Me
			End If
			Return fieldDefinition
		End Function
	End Class
End Namespace
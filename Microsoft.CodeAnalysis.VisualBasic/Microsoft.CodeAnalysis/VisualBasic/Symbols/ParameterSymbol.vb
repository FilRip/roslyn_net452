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
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class ParameterSymbol
		Inherits Symbol
		Implements IParameterTypeInformation, IParameterDefinition, IParameterSymbol, IParameterSymbolInternal
		Friend ReadOnly Property AdaptedParameterSymbol As ParameterSymbol
			Get
				Return Me
			End Get
		End Property

		Public MustOverride ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)

		Public NotOverridable Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.NotApplicable
			End Get
		End Property

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				Return Me.ContainingSymbol.EmbeddedSymbolKind
			End Get
		End Property

		Friend MustOverride ReadOnly Property ExplicitDefaultConstantValue(ByVal inProgress As SymbolsInProgress(Of ParameterSymbol)) As ConstantValue

		Friend ReadOnly Property ExplicitDefaultConstantValue As ConstantValue
			Get
				Return Me(SymbolsInProgress(Of ParameterSymbol).Empty)
			End Get
		End Property

		Public ReadOnly Property ExplicitDefaultValue As Object
			Get
				If (Not Me.HasExplicitDefaultValue) Then
					Throw New InvalidOperationException()
				End If
				Return Me.ExplicitDefaultConstantValue.Value
			End Get
		End Property

		Public MustOverride ReadOnly Property HasExplicitDefaultValue As Boolean Implements IParameterSymbol.HasExplicitDefaultValue

		Friend Overridable ReadOnly Property HasMetadataConstantValue As Boolean
			Get
				Dim flag As Boolean
				If (Not Me.HasExplicitDefaultValue) Then
					flag = False
				Else
					Dim explicitDefaultConstantValue As ConstantValue = Me.ExplicitDefaultConstantValue
					flag = If(explicitDefaultConstantValue.Discriminator = ConstantValueTypeDiscriminator.DateTime, False, explicitDefaultConstantValue.Discriminator <> ConstantValueTypeDiscriminator.[Decimal])
				End If
				Return flag
			End Get
		End Property

		Friend MustOverride ReadOnly Property HasOptionCompare As Boolean

		Public NotOverridable Overrides ReadOnly Property HasUnsupportedMetadata As Boolean
			Get
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = MyBase.DeriveUseSiteInfoFromParameter(Me, Me.HighestPriorityUseSiteError).DiagnosticInfo
				If (diagnosticInfo Is Nothing) Then
					Return False
				End If
				Return diagnosticInfo.Code = 30649
			End Get
		End Property

		Protected Overrides ReadOnly Property HighestPriorityUseSiteError As Integer
			Get
				Return 30649
			End Get
		End Property

		ReadOnly Property INamedEntityName As String
			Get
				Return Me.AdaptedParameterSymbol.MetadataName
			End Get
		End Property

		ReadOnly Property IParameterDefinition_HasDefaultValue As Boolean Implements IParameterDefinition.HasDefaultValue
			Get
				Return Me.AdaptedParameterSymbol.HasMetadataConstantValue
			End Get
		End Property

		ReadOnly Property IParameterDefinitionIsIn As Boolean Implements IParameterDefinition.IsIn
			Get
				Return Me.AdaptedParameterSymbol.IsMetadataIn
			End Get
		End Property

		ReadOnly Property IParameterDefinitionIsMarshalledExplicitly As Boolean Implements IParameterDefinition.IsMarshalledExplicitly
			Get
				Return Me.AdaptedParameterSymbol.IsMarshalledExplicitly
			End Get
		End Property

		ReadOnly Property IParameterDefinitionIsOptional As Boolean Implements IParameterDefinition.IsOptional
			Get
				Return Me.AdaptedParameterSymbol.IsMetadataOptional
			End Get
		End Property

		ReadOnly Property IParameterDefinitionIsOut As Boolean Implements IParameterDefinition.IsOut
			Get
				Return Me.AdaptedParameterSymbol.IsMetadataOut
			End Get
		End Property

		ReadOnly Property IParameterDefinitionMarshallingDescriptor As ImmutableArray(Of Byte) Implements IParameterDefinition.MarshallingDescriptor
			Get
				Return Me.AdaptedParameterSymbol.MarshallingDescriptor
			End Get
		End Property

		ReadOnly Property IParameterDefinitionMarshallingInformation As IMarshallingInformation Implements IParameterDefinition.MarshallingInformation
			Get
				Return Me.AdaptedParameterSymbol.MarshallingInformation
			End Get
		End Property

		ReadOnly Property IParameterListEntryIndex As UShort
			Get
				Return CUShort(Me.AdaptedParameterSymbol.Ordinal)
			End Get
		End Property

		ReadOnly Property IParameterSymbol_CustomModifiers As ImmutableArray(Of CustomModifier) Implements IParameterSymbol.CustomModifiers
			Get
				Return Me.CustomModifiers
			End Get
		End Property

		ReadOnly Property IParameterSymbol_DefaultValue As Object Implements IParameterSymbol.ExplicitDefaultValue
			Get
				Return Me.ExplicitDefaultValue
			End Get
		End Property

		ReadOnly Property IParameterSymbol_IsDiscard As Boolean Implements IParameterSymbol.IsDiscard
			Get
				Return False
			End Get
		End Property

		ReadOnly Property IParameterSymbol_IsOptional As Boolean Implements IParameterSymbol.IsOptional
			Get
				Return Me.IsOptional
			End Get
		End Property

		ReadOnly Property IParameterSymbol_IsThis As Boolean Implements IParameterSymbol.IsThis
			Get
				Return Me.IsMe
			End Get
		End Property

		ReadOnly Property IParameterSymbol_NullableAnnotation As Microsoft.CodeAnalysis.NullableAnnotation Implements IParameterSymbol.NullableAnnotation
			Get
				Return Microsoft.CodeAnalysis.NullableAnnotation.None
			End Get
		End Property

		ReadOnly Property IParameterSymbol_Ordinal As Integer Implements IParameterSymbol.Ordinal
			Get
				Return Me.Ordinal
			End Get
		End Property

		ReadOnly Property IParameterSymbol_OriginalDefinition As IParameterSymbol Implements IParameterSymbol.OriginalDefinition
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		ReadOnly Property IParameterSymbol_RefCustomModifiers As ImmutableArray(Of CustomModifier) Implements IParameterSymbol.RefCustomModifiers
			Get
				Return Me.RefCustomModifiers
			End Get
		End Property

		ReadOnly Property IParameterSymbol_RefKind As Microsoft.CodeAnalysis.RefKind Implements IParameterSymbol.RefKind
			Get
				If (Not Me.IsByRef) Then
					Return Microsoft.CodeAnalysis.RefKind.None
				End If
				Return Microsoft.CodeAnalysis.RefKind.Ref
			End Get
		End Property

		ReadOnly Property IParameterSymbol_Type As ITypeSymbol Implements IParameterSymbol.Type
			Get
				Return Me.Type
			End Get
		End Property

		ReadOnly Property IParameterTypeInformationCustomModifiers As ImmutableArray(Of ICustomModifier) Implements IParameterTypeInformation.CustomModifiers
			Get
				Return Me.AdaptedParameterSymbol.CustomModifiers.[As](Of ICustomModifier)()
			End Get
		End Property

		ReadOnly Property IParameterTypeInformationIsByReference As Boolean Implements IParameterTypeInformation.IsByReference
			Get
				Return Me.AdaptedParameterSymbol.IsByRef
			End Get
		End Property

		ReadOnly Property IParameterTypeInformationRefCustomModifiers As ImmutableArray(Of ICustomModifier) Implements IParameterTypeInformation.RefCustomModifiers
			Get
				Return Me.AdaptedParameterSymbol.RefCustomModifiers.[As](Of ICustomModifier)()
			End Get
		End Property

		Public MustOverride ReadOnly Property IsByRef As Boolean

		Friend MustOverride ReadOnly Property IsCallerFilePath As Boolean

		Friend MustOverride ReadOnly Property IsCallerLineNumber As Boolean

		Friend MustOverride ReadOnly Property IsCallerMemberName As Boolean

		Friend MustOverride ReadOnly Property IsExplicitByRef As Boolean

		Friend MustOverride ReadOnly Property IsIDispatchConstant As Boolean

		Friend MustOverride ReadOnly Property IsIUnknownConstant As Boolean

		Friend ReadOnly Property IsMarshalAsObject As Boolean
			Get
				Dim flag As Boolean
				Dim marshallingType As UnmanagedType = Me.MarshallingType
				flag = If(CInt(marshallingType) - CInt(UnmanagedType.IUnknown) <= 1 OrElse marshallingType = UnmanagedType.[Interface], True, False)
				Return flag
			End Get
		End Property

		Friend Overridable ReadOnly Property IsMarshalledExplicitly As Boolean
			Get
				Return Me.MarshallingInformation IsNot Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property IsMe As Boolean
			Get
				Return False
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsMetadataIn As Boolean

		Friend Overridable ReadOnly Property IsMetadataOptional As Boolean
			Get
				If (Me.IsOptional) Then
					Return True
				End If
				Return Me.GetAttributes().Any(Function(a As VisualBasicAttributeData) a.IsTargetAttribute(Me, AttributeDescription.OptionalAttribute))
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsMetadataOut As Boolean

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

		Public MustOverride ReadOnly Property IsOptional As Boolean

		Friend ReadOnly Property IsOut As Boolean
			Get
				If (Not Me.IsByRef OrElse Not Me.IsMetadataOut) Then
					Return False
				End If
				Return Not Me.IsMetadataIn
			End Get
		End Property

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

		Public MustOverride ReadOnly Property IsParamArray As Boolean Implements IParameterSymbol.IsParams

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.Parameter
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

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property Ordinal As Integer

		Public Shadows Overridable ReadOnly Property OriginalDefinition As ParameterSymbol
			Get
				Return Me
			End Get
		End Property

		Protected NotOverridable Overrides ReadOnly Property OriginalSymbolDefinition As Symbol
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		Public MustOverride ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)

		Public MustOverride ReadOnly Property Type As TypeSymbol

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitParameter(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitParameter(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitParameter(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitParameter(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitParameter(Me)
		End Function

		Friend Overridable Function ChangeOwner(ByVal newContainingSymbol As Symbol) As ParameterSymbol
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Shadows Function GetCciAdapter() As ParameterSymbol
			Return Me
		End Function

		Friend Function GetMetadataConstantValue(ByVal context As EmitContext) As Microsoft.CodeAnalysis.CodeGen.MetadataConstant
			Dim metadataConstant As Microsoft.CodeAnalysis.CodeGen.MetadataConstant
			If (Not Me.AdaptedParameterSymbol.HasMetadataConstantValue) Then
				metadataConstant = Nothing
			Else
				metadataConstant = DirectCast(context.[Module], PEModuleBuilder).CreateConstant(Me.AdaptedParameterSymbol.Type, RuntimeHelpers.GetObjectValue(Me.AdaptedParameterSymbol.ExplicitDefaultConstantValue.Value), DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
			End If
			Return metadataConstant
		End Function

		Private Function IParameterDefinition_GetDefaultValue(ByVal context As EmitContext) As MetadataConstant Implements IParameterDefinition.GetDefaultValue
			Return Me.GetMetadataConstantValue(context)
		End Function

		Private Function IParameterTypeInformationGetType(ByVal context As EmitContext) As ITypeReference Implements IParameterTypeInformation.[GetType]
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim type As TypeSymbol = Me.AdaptedParameterSymbol.Type
			Return [module].Translate(type, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
		End Function

		Friend NotOverridable Overrides Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Dim definition As IDefinition
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			If (Not Me.AdaptedParameterSymbol.IsDefinition OrElse Not (Me.AdaptedParameterSymbol.ContainingModule = [module].SourceModule)) Then
				definition = Nothing
			Else
				definition = Me
			End If
			Return definition
		End Function

		Friend NotOverridable Overrides Sub IReferenceDispatch(ByVal visitor As MetadataVisitor)
			If (Not Me.AdaptedParameterSymbol.IsDefinition) Then
				visitor.Visit(DirectCast(Me, IParameterTypeInformation))
				Return
			End If
			If (Me.AdaptedParameterSymbol.ContainingModule = DirectCast(visitor.Context.[Module], PEModuleBuilder).SourceModule) Then
				visitor.Visit(Me)
				Return
			End If
			visitor.Visit(DirectCast(Me, IParameterTypeInformation))
		End Sub
	End Class
End Namespace
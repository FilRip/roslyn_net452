Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SourceParameterSymbol
		Inherits SourceParameterSymbolBase
		Implements IAttributeTargetSymbol
		Private ReadOnly _location As Microsoft.CodeAnalysis.Location

		Private ReadOnly _name As String

		Private ReadOnly _type As TypeSymbol

		Friend MustOverride ReadOnly Property AttributeDeclarationList As SyntaxList(Of AttributeListSyntax)

		Public Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Dim declaringSyntaxReferenceHelper As ImmutableArray(Of SyntaxReference)
				If (Not Me.IsImplicitlyDeclared) Then
					declaringSyntaxReferenceHelper = Symbol.GetDeclaringSyntaxReferenceHelper(Of ParameterSyntax)(Me.Locations)
				Else
					declaringSyntaxReferenceHelper = ImmutableArray(Of SyntaxReference).Empty
				End If
				Return declaringSyntaxReferenceHelper
			End Get
		End Property

		Public ReadOnly Property DefaultAttributeLocation As AttributeLocation Implements IAttributeTargetSymbol.DefaultAttributeLocation
			Get
				Return AttributeLocation.Parameter
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasDefaultValueAttribute As Boolean
			Get
				Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.DefaultParameterValue <> ConstantValue.Unset
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasOptionCompare As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasParamArrayAttribute As Boolean
			Get
				Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.HasParamArrayAttribute
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsByRef As Boolean
			Get
				Dim flag As Boolean
				If (Me.IsExplicitByRef) Then
					flag = True
				ElseIf (Not Me.Type.IsStringType() OrElse MyBase.ContainingSymbol.Kind <> SymbolKind.Method OrElse DirectCast(MyBase.ContainingSymbol, MethodSymbol).MethodKind <> MethodKind.DeclareMethod) Then
					flag = False
				Else
					Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
					flag = If(earlyDecodedWellKnownAttributeData Is Nothing, True, Not earlyDecodedWellKnownAttributeData.HasMarshalAsAttribute)
				End If
				Return flag
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIDispatchConstant As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Dim matchingPropertyParameter As Boolean
				Dim methodKind As Boolean
				Dim nullable As Nullable(Of Boolean)
				If (Not MyBase.ContainingSymbol.IsImplicitlyDeclared) Then
					matchingPropertyParameter = CObj(Me.GetMatchingPropertyParameter()) <> CObj(Nothing)
				Else
					Dim containingSymbol As MethodSymbol = TryCast(MyBase.ContainingSymbol, MethodSymbol)
					If (containingSymbol IsNot Nothing) Then
						methodKind = containingSymbol.MethodKind = Microsoft.CodeAnalysis.MethodKind.DelegateInvoke
					Else
						methodKind = False
					End If
					If (methodKind) Then
						Dim associatedSymbol As Symbol = Me.ContainingType.AssociatedSymbol
						If (associatedSymbol IsNot Nothing) Then
							nullable = New Nullable(Of Boolean)(associatedSymbol.IsImplicitlyDeclared)
						Else
							nullable = Nothing
						End If
						Dim nullable1 As Nullable(Of Boolean) = nullable
						nullable1 = If(nullable1.HasValue, New Nullable(Of Boolean)(Not nullable1.GetValueOrDefault()), nullable1)
						If (Not nullable1.GetValueOrDefault()) Then
							GoTo Label1
						End If
						matchingPropertyParameter = False
						Return matchingPropertyParameter
					End If
				Label1:
					matchingPropertyParameter = True
				End If
				Return matchingPropertyParameter
			End Get
		End Property

		Friend Overrides ReadOnly Property IsIUnknownConstant As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMetadataIn As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonParameterWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasInAttribute
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMetadataOut As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonParameterWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasOutAttribute
			End Get
		End Property

		Friend ReadOnly Property Location As Microsoft.CodeAnalysis.Location
			Get
				Return Me._location
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Locations As ImmutableArray(Of Microsoft.CodeAnalysis.Location)
			Get
				Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.Location)
				If (Me._location Is Nothing) Then
					empty = ImmutableArray(Of Microsoft.CodeAnalysis.Location).Empty
				Else
					empty = ImmutableArray.Create(Of Microsoft.CodeAnalysis.Location)(Me._location)
				End If
				Return empty
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property MarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Dim marshalPseudoCustomAttributeDatum As MarshalPseudoCustomAttributeData
				Dim decodedWellKnownAttributeData As CommonParameterWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					If (Me.Type.IsStringType()) Then
						Dim containingSymbol As Symbol = MyBase.ContainingSymbol
						If (containingSymbol.Kind = SymbolKind.Method) Then
							Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							If (methodSymbol.MethodKind <> MethodKind.DeclareMethod) Then
								GoTo Label1
							End If
							Dim marshalPseudoCustomAttributeDatum1 As MarshalPseudoCustomAttributeData = New MarshalPseudoCustomAttributeData()
							If (Not Me.IsExplicitByRef) Then
								marshalPseudoCustomAttributeDatum1.SetMarshalAsSimpleType(UnmanagedType.VBByRefStr)
							Else
								Dim dllImportData As Microsoft.CodeAnalysis.DllImportData = methodSymbol.GetDllImportData()
								Select Case dllImportData.CharacterSet
									Case CharSet.None
									Case CharSet.Ansi
										marshalPseudoCustomAttributeDatum1.SetMarshalAsSimpleType(UnmanagedType.AnsiBStr)
										marshalPseudoCustomAttributeDatum = marshalPseudoCustomAttributeDatum1
										Return marshalPseudoCustomAttributeDatum
									Case CharSet.Unicode
										marshalPseudoCustomAttributeDatum1.SetMarshalAsSimpleType(UnmanagedType.BStr)
										marshalPseudoCustomAttributeDatum = marshalPseudoCustomAttributeDatum1
										Return marshalPseudoCustomAttributeDatum
									Case CharSet.Auto
										marshalPseudoCustomAttributeDatum1.SetMarshalAsSimpleType(UnmanagedType.TBStr)
										marshalPseudoCustomAttributeDatum = marshalPseudoCustomAttributeDatum1
										Return marshalPseudoCustomAttributeDatum
								End Select
								Throw ExceptionUtilities.UnexpectedValue(dllImportData.CharacterSet)
							End If
							marshalPseudoCustomAttributeDatum = marshalPseudoCustomAttributeDatum1
							Return marshalPseudoCustomAttributeDatum
						End If
					End If
				Label1:
					marshalPseudoCustomAttributeDatum = Nothing
				Else
					marshalPseudoCustomAttributeDatum = decodedWellKnownAttributeData.MarshallingInformation
				End If
				Return marshalPseudoCustomAttributeDatum
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)

		Public NotOverridable Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Friend Sub New(ByVal container As Symbol, ByVal name As String, ByVal ordinal As Integer, ByVal type As TypeSymbol, ByVal location As Microsoft.CodeAnalysis.Location)
			MyBase.New(container, ordinal)
			Me._name = name
			Me._type = type
			Me._location = location
		End Sub

		Private Sub DecodeDefaultParameterValueAttribute(ByVal description As AttributeDescription, ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			Dim attribute As VisualBasicAttributeData = arguments.Attribute
			Dim diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Me.DecodeDefaultParameterValueAttribute(description, attribute)
			If (Not constantValue.IsBad) Then
				Me.VerifyParamDefaultValueMatchesAttributeIfAny(constantValue, arguments.AttributeSyntaxOpt, diagnostics)
			End If
		End Sub

		Private Function DecodeDefaultParameterValueAttribute(ByVal description As AttributeDescription, ByVal attribute As VisualBasicAttributeData) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			If (Not description.Equals(AttributeDescription.DefaultParameterValueAttribute)) Then
				constantValue = If(Not description.Equals(AttributeDescription.DecimalConstantAttribute), attribute.DecodeDateTimeConstantValue(), attribute.DecodeDecimalConstantValue())
			Else
				constantValue = Me.DecodeDefaultParameterValueAttribute(attribute)
			End If
			Return constantValue
		End Function

		Private Function DecodeDefaultParameterValueAttribute(ByVal attribute As VisualBasicAttributeData) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim item As TypedConstant = attribute.CommonConstructorArguments(0)
			Dim discriminator As ConstantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValue.GetDiscriminator(If(item.Kind = TypedConstantKind.[Enum], DirectCast(item.TypeInternal, NamedTypeSymbol).EnumUnderlyingType.SpecialType, item.TypeInternal.SpecialType))
			If (discriminator <> ConstantValueTypeDiscriminator.Bad) Then
				constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(RuntimeHelpers.GetObjectValue(item.ValueInternal), discriminator)
			Else
				constantValue = If(item.Kind = TypedConstantKind.Array OrElse item.ValueInternal IsNot Nothing OrElse Not Me.Type.IsReferenceType, Microsoft.CodeAnalysis.ConstantValue.Bad, Microsoft.CodeAnalysis.ConstantValue.Null)
			End If
			Return constantValue
		End Function

		Friend Overrides Sub DecodeWellKnownAttribute(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			Dim attribute As VisualBasicAttributeData = arguments.Attribute
			If (attribute.IsTargetAttribute(Me, AttributeDescription.TupleElementNamesAttribute)) Then
				DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag).Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt.Location)
			End If
			If (attribute.IsTargetAttribute(Me, AttributeDescription.DefaultParameterValueAttribute)) Then
				Me.DecodeDefaultParameterValueAttribute(AttributeDescription.DefaultParameterValueAttribute, arguments)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.DecimalConstantAttribute)) Then
				Me.DecodeDefaultParameterValueAttribute(AttributeDescription.DecimalConstantAttribute, arguments)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.DateTimeConstantAttribute)) Then
				Me.DecodeDefaultParameterValueAttribute(AttributeDescription.DateTimeConstantAttribute, arguments)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.InAttribute)) Then
				arguments.GetOrCreateData(Of CommonParameterWellKnownAttributeData)().HasInAttribute = True
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.OutAttribute)) Then
				arguments.GetOrCreateData(Of CommonParameterWellKnownAttributeData)().HasOutAttribute = True
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.MarshalAsAttribute)) Then
				MarshalAsAttributeDecoder(Of CommonParameterWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation).Decode(arguments, AttributeTargets.Parameter, MessageProvider.Instance)
			End If
			MyBase.DecodeWellKnownAttribute(arguments)
		End Sub

		Private Function EarlyDecodeAttributeForDefaultParameterValue(ByVal description As AttributeDescription, ByRef arguments As EarlyDecodeWellKnownAttributeArguments(Of EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation)) As VisualBasicAttributeData
			Dim bad As ConstantValue
			Dim flag As Boolean = False
			Dim attribute As SourceAttributeData = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, flag)
			If (Not attribute.HasErrors) Then
				bad = Me.DecodeDefaultParameterValueAttribute(description, attribute)
			Else
				bad = ConstantValue.Bad
				flag = True
			End If
			Dim orCreateData As ParameterEarlyWellKnownAttributeData = arguments.GetOrCreateData(Of ParameterEarlyWellKnownAttributeData)()
			If (orCreateData.DefaultParameterValue = ConstantValue.Unset) Then
				orCreateData.DefaultParameterValue = bad
			End If
			If (Not flag) Then
				Return attribute
			End If
			Return Nothing
		End Function

		Friend Overrides Function EarlyDecodeWellKnownAttribute(ByRef arguments As EarlyDecodeWellKnownAttributeArguments(Of EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation)) As VisualBasicAttributeData
			Dim visualBasicAttributeDatum As VisualBasicAttributeData
			Dim visualBasicAttributeDatum1 As VisualBasicAttributeData
			Dim visualBasicAttributeDatum2 As VisualBasicAttributeData
			Dim containingSymbol As Symbol = MyBase.ContainingSymbol
			If (containingSymbol.Kind <> SymbolKind.Method OrElse DirectCast(containingSymbol, MethodSymbol).MethodKind <> Microsoft.CodeAnalysis.MethodKind.DeclareMethod OrElse Not VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.MarshalAsAttribute)) Then
				Dim flag As Boolean = False
				Dim kind As SymbolKind = containingSymbol.Kind
				If (kind = SymbolKind.Method) Then
					Dim methodKind As Microsoft.CodeAnalysis.MethodKind = DirectCast(containingSymbol, MethodSymbol).MethodKind
					If (methodKind <> Microsoft.CodeAnalysis.MethodKind.Conversion) Then
						Select Case methodKind
							Case Microsoft.CodeAnalysis.MethodKind.EventAdd
							Case Microsoft.CodeAnalysis.MethodKind.EventRemove
							Case Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator
								Exit Select
							Case Else
								flag = True
								Exit Select
						End Select
					End If
				ElseIf (kind = SymbolKind.[Property]) Then
					flag = True
				End If
				If (flag AndAlso VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ParamArrayAttribute)) Then
					Dim flag1 As Boolean = False
					Dim attribute As SourceAttributeData = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, flag1)
					If (attribute.HasErrors) Then
						visualBasicAttributeDatum = Nothing
					Else
						arguments.GetOrCreateData(Of ParameterEarlyWellKnownAttributeData)().HasParamArrayAttribute = True
						If (Not flag1) Then
							visualBasicAttributeDatum1 = attribute
						Else
							visualBasicAttributeDatum1 = Nothing
						End If
						visualBasicAttributeDatum = visualBasicAttributeDatum1
					End If
				ElseIf (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.DefaultParameterValueAttribute)) Then
					visualBasicAttributeDatum = Me.EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription.DefaultParameterValueAttribute, arguments)
				ElseIf (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.DecimalConstantAttribute)) Then
					visualBasicAttributeDatum = Me.EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription.DecimalConstantAttribute, arguments)
				ElseIf (Not VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.DateTimeConstantAttribute)) Then
					If (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CallerLineNumberAttribute)) Then
						arguments.GetOrCreateData(Of ParameterEarlyWellKnownAttributeData)().HasCallerLineNumberAttribute = True
					ElseIf (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CallerFilePathAttribute)) Then
						arguments.GetOrCreateData(Of ParameterEarlyWellKnownAttributeData)().HasCallerFilePathAttribute = True
					ElseIf (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CallerMemberNameAttribute)) Then
						arguments.GetOrCreateData(Of ParameterEarlyWellKnownAttributeData)().HasCallerMemberNameAttribute = True
					End If
					visualBasicAttributeDatum = MyBase.EarlyDecodeWellKnownAttribute(arguments)
				Else
					visualBasicAttributeDatum = Me.EarlyDecodeAttributeForDefaultParameterValue(AttributeDescription.DateTimeConstantAttribute, arguments)
				End If
			Else
				Dim flag2 As Boolean = False
				Dim sourceAttributeDatum As SourceAttributeData = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, flag2)
				If (sourceAttributeDatum.HasErrors) Then
					visualBasicAttributeDatum = Nothing
				Else
					arguments.GetOrCreateData(Of ParameterEarlyWellKnownAttributeData)().HasMarshalAsAttribute = True
					If (Not flag2) Then
						visualBasicAttributeDatum2 = sourceAttributeDatum
					Else
						visualBasicAttributeDatum2 = Nothing
					End If
					visualBasicAttributeDatum = visualBasicAttributeDatum2
				End If
			End If
			Return visualBasicAttributeDatum
		End Function

		Public NotOverridable Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.GetAttributesBag().Attributes
		End Function

		Friend MustOverride Function GetAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)

		Friend MustOverride Function GetDecodedWellKnownAttributeData() As CommonParameterWellKnownAttributeData

		Friend MustOverride Function GetEarlyDecodedWellKnownAttributeData() As ParameterEarlyWellKnownAttributeData

		Private Function GetMatchingPropertyParameter() As ParameterSymbol
			Dim item As ParameterSymbol
			Dim containingSymbol As MethodSymbol = TryCast(MyBase.ContainingSymbol, MethodSymbol)
			If (containingSymbol IsNot Nothing AndAlso containingSymbol.IsAccessor()) Then
				Dim associatedSymbol As PropertySymbol = TryCast(containingSymbol.AssociatedSymbol, PropertySymbol)
				If (associatedSymbol Is Nothing OrElse MyBase.Ordinal >= associatedSymbol.ParameterCount) Then
					item = Nothing
					Return item
				End If
				item = associatedSymbol.Parameters(MyBase.Ordinal)
				Return item
			End If
			item = Nothing
			Return item
		End Function

		Protected Sub VerifyParamDefaultValueMatchesAttributeIfAny(ByVal value As ConstantValue, ByVal syntax As VisualBasicSyntaxNode, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim earlyDecodedWellKnownAttributeData As ParameterEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
			If (earlyDecodedWellKnownAttributeData IsNot Nothing) Then
				Dim defaultParameterValue As ConstantValue = earlyDecodedWellKnownAttributeData.DefaultParameterValue
				If (defaultParameterValue <> ConstantValue.Unset AndAlso value <> defaultParameterValue) Then
					Binder.ReportDiagnostic(diagnostics, syntax, ERRID.ERR_ParamDefaultValueDiffersFromAttribute)
				End If
			End If
		End Sub
	End Class
End Namespace
Imports System
Imports System.Globalization
Imports System.Resources
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class VBResources
		Private Shared s_resourceManager As System.Resources.ResourceManager

		Friend ReadOnly Shared Property AggregateSyntaxNotWithinSyntaxTree As String
			Get
				Return VBResources.GetResourceString("AggregateSyntaxNotWithinSyntaxTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property AnonymousObjectCreationExpressionSyntaxNotWithinTree As String
			Get
				Return VBResources.GetResourceString("AnonymousObjectCreationExpressionSyntaxNotWithinTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property AssociatedTypeDoesNotHaveTypeParameters As String
			Get
				Return VBResources.GetResourceString("AssociatedTypeDoesNotHaveTypeParameters", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property CannotAddCompilerSpecialTree As String
			Get
				Return VBResources.GetResourceString("CannotAddCompilerSpecialTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property CannotRemoveCompilerSpecialTree As String
			Get
				Return VBResources.GetResourceString("CannotRemoveCompilerSpecialTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property CantReferenceCompilationFromTypes As String
			Get
				Return VBResources.GetResourceString("CantReferenceCompilationFromTypes", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ChainingSpeculativeModelIsNotSupported As String
			Get
				Return VBResources.GetResourceString("ChainingSpeculativeModelIsNotSupported", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property CompilationVisualBasic As String
			Get
				Return VBResources.GetResourceString("CompilationVisualBasic", Nothing)
			End Get
		End Property

		Friend Shared Property Culture As CultureInfo

		Friend ReadOnly Shared Property DeclarationSyntaxNotWithinSyntaxTree As String
			Get
				Return VBResources.GetResourceString("DeclarationSyntaxNotWithinSyntaxTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property DeclarationSyntaxNotWithinTree As String
			Get
				Return VBResources.GetResourceString("DeclarationSyntaxNotWithinTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ElementsCannotBeNull As String
			Get
				Return VBResources.GetResourceString("ElementsCannotBeNull", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AbsentReferenceToPIA1 As String
			Get
				Return VBResources.GetResourceString("ERR_AbsentReferenceToPIA1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AccessMismatch6 As String
			Get
				Return VBResources.GetResourceString("ERR_AccessMismatch6", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AccessMismatchImplementedEvent4 As String
			Get
				Return VBResources.GetResourceString("ERR_AccessMismatchImplementedEvent4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AccessMismatchImplementedEvent6 As String
			Get
				Return VBResources.GetResourceString("ERR_AccessMismatchImplementedEvent6", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AccessMismatchOutsideAssembly4 As String
			Get
				Return VBResources.GetResourceString("ERR_AccessMismatchOutsideAssembly4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AddOrRemoveHandlerEvent As String
			Get
				Return VBResources.GetResourceString("ERR_AddOrRemoveHandlerEvent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AddParamWrongForWinRT As String
			Get
				Return VBResources.GetResourceString("ERR_AddParamWrongForWinRT", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AddRemoveParamNotEventType As String
			Get
				Return VBResources.GetResourceString("ERR_AddRemoveParamNotEventType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AddressOfInSelectCaseExpr As String
			Get
				Return VBResources.GetResourceString("ERR_AddressOfInSelectCaseExpr", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AddressOfNotCreatableDelegate1 As String
			Get
				Return VBResources.GetResourceString("ERR_AddressOfNotCreatableDelegate1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AddressOfNotDelegate1 As String
			Get
				Return VBResources.GetResourceString("ERR_AddressOfNotDelegate1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AddressOfNullableMethod As String
			Get
				Return VBResources.GetResourceString("ERR_AddressOfNullableMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AddressOfOperandNotMethod As String
			Get
				Return VBResources.GetResourceString("ERR_AddressOfOperandNotMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AggrInitInvalidForObject As String
			Get
				Return VBResources.GetResourceString("ERR_AggrInitInvalidForObject", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AgnosticToMachineModule As String
			Get
				Return VBResources.GetResourceString("ERR_AgnosticToMachineModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousAcrossInterfaces3 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousAcrossInterfaces3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousCastConversion2 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousCastConversion2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousDelegateBinding2 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousDelegateBinding2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousImplements3 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousImplements3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousImplementsMember3 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousImplementsMember3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousInImports2 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousInImports2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousInModules2 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousInModules2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousInNamespace2 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousInNamespace2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousInNamespaces2 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousInNamespaces2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousInUnnamedNamespace1 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousInUnnamedNamespace1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousOverrides3 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousOverrides3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AmbiguousWidestType3 As String
			Get
				Return VBResources.GetResourceString("ERR_AmbiguousWidestType3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AnonTypeFieldXMLNameInference As String
			Get
				Return VBResources.GetResourceString("ERR_AnonTypeFieldXMLNameInference", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AnonymousTypeDisallowsTypeChar As String
			Get
				Return VBResources.GetResourceString("ERR_AnonymousTypeDisallowsTypeChar", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AnonymousTypeExpectedIdentifier As String
			Get
				Return VBResources.GetResourceString("ERR_AnonymousTypeExpectedIdentifier", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AnonymousTypeFieldNameInference As String
			Get
				Return VBResources.GetResourceString("ERR_AnonymousTypeFieldNameInference", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AnonymousTypeNameWithoutPeriod As String
			Get
				Return VBResources.GetResourceString("ERR_AnonymousTypeNameWithoutPeriod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AnonymousTypeNeedField As String
			Get
				Return VBResources.GetResourceString("ERR_AnonymousTypeNeedField", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AnonymousTypePropertyOutOfOrder1 As String
			Get
				Return VBResources.GetResourceString("ERR_AnonymousTypePropertyOutOfOrder1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArgumentCopyBackNarrowing3 As String
			Get
				Return VBResources.GetResourceString("ERR_ArgumentCopyBackNarrowing3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArgumentNarrowing2 As String
			Get
				Return VBResources.GetResourceString("ERR_ArgumentNarrowing2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArgumentNarrowing3 As String
			Get
				Return VBResources.GetResourceString("ERR_ArgumentNarrowing3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArgumentRequired As String
			Get
				Return VBResources.GetResourceString("ERR_ArgumentRequired", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArgumentSyntax As String
			Get
				Return VBResources.GetResourceString("ERR_ArgumentSyntax", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArrayInitForNonArray2 As String
			Get
				Return VBResources.GetResourceString("ERR_ArrayInitForNonArray2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArrayInitializerForNonConstDim As String
			Get
				Return VBResources.GetResourceString("ERR_ArrayInitializerForNonConstDim", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArrayInitializerTooFewDimensions As String
			Get
				Return VBResources.GetResourceString("ERR_ArrayInitializerTooFewDimensions", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArrayInitializerTooManyDimensions As String
			Get
				Return VBResources.GetResourceString("ERR_ArrayInitializerTooManyDimensions", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArrayInitInStruct As String
			Get
				Return VBResources.GetResourceString("ERR_ArrayInitInStruct", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArrayInitNoType As String
			Get
				Return VBResources.GetResourceString("ERR_ArrayInitNoType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArrayInitNoTypeObjectDisallowed As String
			Get
				Return VBResources.GetResourceString("ERR_ArrayInitNoTypeObjectDisallowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArrayInitTooManyTypesObjectDisallowed As String
			Get
				Return VBResources.GetResourceString("ERR_ArrayInitTooManyTypesObjectDisallowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArrayOfRawGenericInvalid As String
			Get
				Return VBResources.GetResourceString("ERR_ArrayOfRawGenericInvalid", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ArrayRankLimit As String
			Get
				Return VBResources.GetResourceString("ERR_ArrayRankLimit", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AsNewArray As String
			Get
				Return VBResources.GetResourceString("ERR_AsNewArray", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AssignmentInitOnly As String
			Get
				Return VBResources.GetResourceString("ERR_AssignmentInitOnly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AsyncSubMain As String
			Get
				Return VBResources.GetResourceString("ERR_AsyncSubMain", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AttrAssignmentNotFieldOrProp1 As String
			Get
				Return VBResources.GetResourceString("ERR_AttrAssignmentNotFieldOrProp1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AttrCannotBeGenerics As String
			Get
				Return VBResources.GetResourceString("ERR_AttrCannotBeGenerics", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AttributeCannotBeAbstract As String
			Get
				Return VBResources.GetResourceString("ERR_AttributeCannotBeAbstract", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AttributeMustBeClassNotStruct1 As String
			Get
				Return VBResources.GetResourceString("ERR_AttributeMustBeClassNotStruct1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AttributeMustInheritSysAttr As String
			Get
				Return VBResources.GetResourceString("ERR_AttributeMustInheritSysAttr", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AttributeOnLambdaReturnType As String
			Get
				Return VBResources.GetResourceString("ERR_AttributeOnLambdaReturnType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AttributeOrder As String
			Get
				Return VBResources.GetResourceString("ERR_AttributeOrder", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AttributeParameterRequired1 As String
			Get
				Return VBResources.GetResourceString("ERR_AttributeParameterRequired1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AttributeParameterRequired2 As String
			Get
				Return VBResources.GetResourceString("ERR_AttributeParameterRequired2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AttributeStmtWrongOrder As String
			Get
				Return VBResources.GetResourceString("ERR_AttributeStmtWrongOrder", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AutoPropertyCantBeWriteOnly As String
			Get
				Return VBResources.GetResourceString("ERR_AutoPropertyCantBeWriteOnly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AutoPropertyCantHaveParams As String
			Get
				Return VBResources.GetResourceString("ERR_AutoPropertyCantHaveParams", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_AutoPropertyInitializedInStructure As String
			Get
				Return VBResources.GetResourceString("ERR_AutoPropertyInitializedInStructure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAnonymousTypeForExprTree As String
			Get
				Return VBResources.GetResourceString("ERR_BadAnonymousTypeForExprTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAssemblyName As String
			Get
				Return VBResources.GetResourceString("ERR_BadAssemblyName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAsyncByRefParam As String
			Get
				Return VBResources.GetResourceString("ERR_BadAsyncByRefParam", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAsyncInQuery As String
			Get
				Return VBResources.GetResourceString("ERR_BadAsyncInQuery", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAsyncReturn As String
			Get
				Return VBResources.GetResourceString("ERR_BadAsyncReturn", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAsyncReturnOperand1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAsyncReturnOperand1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttribute1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttribute1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttributeConstructor1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttributeConstructor1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttributeConstructor2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttributeConstructor2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttributeNonPublicConstructor As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttributeNonPublicConstructor", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttributeNonPublicContType2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttributeNonPublicContType2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttributeNonPublicProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttributeNonPublicProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttributeNonPublicType1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttributeNonPublicType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttributePropertyType1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttributePropertyType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttributeReadOnlyProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttributeReadOnlyProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttributeSharedProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttributeSharedProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAttributeUuid2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadAttributeUuid2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAwaitInNonAsyncLambda As String
			Get
				Return VBResources.GetResourceString("ERR_BadAwaitInNonAsyncLambda", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAwaitInNonAsyncMethod As String
			Get
				Return VBResources.GetResourceString("ERR_BadAwaitInNonAsyncMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAwaitInNonAsyncVoidMethod As String
			Get
				Return VBResources.GetResourceString("ERR_BadAwaitInNonAsyncVoidMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAwaitInTryHandler As String
			Get
				Return VBResources.GetResourceString("ERR_BadAwaitInTryHandler", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAwaitNothing As String
			Get
				Return VBResources.GetResourceString("ERR_BadAwaitNothing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadAwaitNotInAsyncMethodOrLambda As String
			Get
				Return VBResources.GetResourceString("ERR_BadAwaitNotInAsyncMethodOrLambda", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadCCExpression As String
			Get
				Return VBResources.GetResourceString("ERR_BadCCExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadChecksumAlgorithm As String
			Get
				Return VBResources.GetResourceString("ERR_BadChecksumAlgorithm", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadClassFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadClassFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadCodepage As String
			Get
				Return VBResources.GetResourceString("ERR_BadCodepage", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadConditionalWithRef As String
			Get
				Return VBResources.GetResourceString("ERR_BadConditionalWithRef", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadConstFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadConstFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadConstraintSyntax As String
			Get
				Return VBResources.GetResourceString("ERR_BadConstraintSyntax", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadDeclareFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadDeclareFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadDelegateFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadDelegateFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadDimFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadDimFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadDocumentationMode As String
			Get
				Return VBResources.GetResourceString("ERR_BadDocumentationMode", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadEmptyEnum1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadEmptyEnum1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadEnumFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadEnumFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadEventFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadEventFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadFlagsInNotInheritableClass1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadFlagsInNotInheritableClass1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadFlagsOnNew1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadFlagsOnNew1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadFlagsOnNewOverloads As String
			Get
				Return VBResources.GetResourceString("ERR_BadFlagsOnNewOverloads", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadFlagsOnSharedMeth1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadFlagsOnSharedMeth1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadFlagsOnSharedProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadFlagsOnSharedProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadFlagsOnStdModuleProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadFlagsOnStdModuleProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadFlagsWithDefault1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadFlagsWithDefault1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadGenericParamForNewConstraint2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadGenericParamForNewConstraint2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadGetAwaiterMethod1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadGetAwaiterMethod1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadImplementsType As String
			Get
				Return VBResources.GetResourceString("ERR_BadImplementsType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadInstanceMemberAccess As String
			Get
				Return VBResources.GetResourceString("ERR_BadInstanceMemberAccess", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadInterfaceClassSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadInterfaceClassSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadInterfaceDelegateSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadInterfaceDelegateSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadInterfaceEnumSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadInterfaceEnumSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadInterfaceFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadInterfaceFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadInterfaceInterfaceSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadInterfaceInterfaceSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadInterfaceMethodFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadInterfaceMethodFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadInterfaceOrderOnInherits As String
			Get
				Return VBResources.GetResourceString("ERR_BadInterfaceOrderOnInherits", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadInterfacePropertyFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadInterfacePropertyFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadInterfaceStructSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadInterfaceStructSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadIsCompletedOnCompletedGetResult2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadIsCompletedOnCompletedGetResult2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadIteratorByRefParam As String
			Get
				Return VBResources.GetResourceString("ERR_BadIteratorByRefParam", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadIteratorExpressionLambda As String
			Get
				Return VBResources.GetResourceString("ERR_BadIteratorExpressionLambda", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadIteratorReturn As String
			Get
				Return VBResources.GetResourceString("ERR_BadIteratorReturn", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadLanguageVersion As String
			Get
				Return VBResources.GetResourceString("ERR_BadLanguageVersion", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadLocalConstFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadLocalConstFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadLocalDimFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadLocalDimFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadMetaDataReference1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadMetaDataReference1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadMethodFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadMethodFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadModuleFile1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadModuleFile1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadModuleFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadModuleFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadModuleName As String
			Get
				Return VBResources.GetResourceString("ERR_BadModuleName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadNamespaceName1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadNamespaceName1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadNonTrailingNamedArgument As String
			Get
				Return VBResources.GetResourceString("ERR_BadNonTrailingNamedArgument", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadNullTypeInCCExpression As String
			Get
				Return VBResources.GetResourceString("ERR_BadNullTypeInCCExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadOperatorFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadOperatorFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadOverloadCandidates2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadOverloadCandidates2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadOverrideAccess2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadOverrideAccess2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadPdbData As String
			Get
				Return VBResources.GetResourceString("ERR_BadPdbData", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadPropertyAccessorFlags As String
			Get
				Return VBResources.GetResourceString("ERR_BadPropertyAccessorFlags", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadPropertyAccessorFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadPropertyAccessorFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadPropertyAccessorFlags2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadPropertyAccessorFlags2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadPropertyAccessorFlags3 As String
			Get
				Return VBResources.GetResourceString("ERR_BadPropertyAccessorFlags3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadPropertyAccessorFlagsRestrict As String
			Get
				Return VBResources.GetResourceString("ERR_BadPropertyAccessorFlagsRestrict", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadPropertyFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadPropertyFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadRecordFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadRecordFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadRefLib1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadRefLib1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadResumableAccessReturnVariable As String
			Get
				Return VBResources.GetResourceString("ERR_BadResumableAccessReturnVariable", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadReturnValueInIterator As String
			Get
				Return VBResources.GetResourceString("ERR_BadReturnValueInIterator", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadSourceCodeKind As String
			Get
				Return VBResources.GetResourceString("ERR_BadSourceCodeKind", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadSpecifierCombo2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadSpecifierCombo2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadStaticInitializerInResumable As String
			Get
				Return VBResources.GetResourceString("ERR_BadStaticInitializerInResumable", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadStaticLocalInGenericMethod As String
			Get
				Return VBResources.GetResourceString("ERR_BadStaticLocalInGenericMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadStaticLocalInStruct As String
			Get
				Return VBResources.GetResourceString("ERR_BadStaticLocalInStruct", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadSwitchValue As String
			Get
				Return VBResources.GetResourceString("ERR_BadSwitchValue", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadTypeArgForRefConstraint2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadTypeArgForRefConstraint2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadTypeArgForStructConstraint2 As String
			Get
				Return VBResources.GetResourceString("ERR_BadTypeArgForStructConstraint2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadTypeArgForStructConstraintNull As String
			Get
				Return VBResources.GetResourceString("ERR_BadTypeArgForStructConstraintNull", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadTypeInCCExpression As String
			Get
				Return VBResources.GetResourceString("ERR_BadTypeInCCExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadUseOfVoid As String
			Get
				Return VBResources.GetResourceString("ERR_BadUseOfVoid", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadWithEventsFlags1 As String
			Get
				Return VBResources.GetResourceString("ERR_BadWithEventsFlags1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadWithRef As String
			Get
				Return VBResources.GetResourceString("ERR_BadWithRef", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadYieldInNonIteratorMethod As String
			Get
				Return VBResources.GetResourceString("ERR_BadYieldInNonIteratorMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BadYieldInTryHandler As String
			Get
				Return VBResources.GetResourceString("ERR_BadYieldInTryHandler", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BaseMismatchForPartialClass3 As String
			Get
				Return VBResources.GetResourceString("ERR_BaseMismatchForPartialClass3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BaseOnlyClassesMustBeExplicit2 As String
			Get
				Return VBResources.GetResourceString("ERR_BaseOnlyClassesMustBeExplicit2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BaseTypeReferences2 As String
			Get
				Return VBResources.GetResourceString("ERR_BaseTypeReferences2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BaseUnifiesWithInterfaces3 As String
			Get
				Return VBResources.GetResourceString("ERR_BaseUnifiesWithInterfaces3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BinaryFile As String
			Get
				Return VBResources.GetResourceString("ERR_BinaryFile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BinaryOperands3 As String
			Get
				Return VBResources.GetResourceString("ERR_BinaryOperands3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BinaryOperandsForXml4 As String
			Get
				Return VBResources.GetResourceString("ERR_BinaryOperandsForXml4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BinaryParamMustBeContainingType1 As String
			Get
				Return VBResources.GetResourceString("ERR_BinaryParamMustBeContainingType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BlockLocalShadowing1 As String
			Get
				Return VBResources.GetResourceString("ERR_BlockLocalShadowing1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BogusWithinLineIf As String
			Get
				Return VBResources.GetResourceString("ERR_BogusWithinLineIf", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_BranchOutOfFinally As String
			Get
				Return VBResources.GetResourceString("ERR_BranchOutOfFinally", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ByRefIllegal1 As String
			Get
				Return VBResources.GetResourceString("ERR_ByRefIllegal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ByRefParamInExpressionTree As String
			Get
				Return VBResources.GetResourceString("ERR_ByRefParamInExpressionTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotBeMadeNullable1 As String
			Get
				Return VBResources.GetResourceString("ERR_CannotBeMadeNullable1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotCallEvent1 As String
			Get
				Return VBResources.GetResourceString("ERR_CannotCallEvent1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotConvertValue2 As String
			Get
				Return VBResources.GetResourceString("ERR_CannotConvertValue2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotEmbedInterfaceWithGeneric As String
			Get
				Return VBResources.GetResourceString("ERR_CannotEmbedInterfaceWithGeneric", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotEmbedWithoutPdb As String
			Get
				Return VBResources.GetResourceString("ERR_CannotEmbedWithoutPdb", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotGotoNonScopeBlocksWithClosure As String
			Get
				Return VBResources.GetResourceString("ERR_CannotGotoNonScopeBlocksWithClosure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotInferNullableForVariable1 As String
			Get
				Return VBResources.GetResourceString("ERR_CannotInferNullableForVariable1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotLiftAnonymousType1 As String
			Get
				Return VBResources.GetResourceString("ERR_CannotLiftAnonymousType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotLiftByRefParamLambda1 As String
			Get
				Return VBResources.GetResourceString("ERR_CannotLiftByRefParamLambda1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotLiftByRefParamQuery1 As String
			Get
				Return VBResources.GetResourceString("ERR_CannotLiftByRefParamQuery1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotLiftRestrictedTypeLambda As String
			Get
				Return VBResources.GetResourceString("ERR_CannotLiftRestrictedTypeLambda", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotLiftRestrictedTypeQuery As String
			Get
				Return VBResources.GetResourceString("ERR_CannotLiftRestrictedTypeQuery", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotLiftRestrictedTypeResumable1 As String
			Get
				Return VBResources.GetResourceString("ERR_CannotLiftRestrictedTypeResumable1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotLiftStructureMeLambda As String
			Get
				Return VBResources.GetResourceString("ERR_CannotLiftStructureMeLambda", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotLiftStructureMeQuery As String
			Get
				Return VBResources.GetResourceString("ERR_CannotLiftStructureMeQuery", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotLinkClassWithNoPIA1 As String
			Get
				Return VBResources.GetResourceString("ERR_CannotLinkClassWithNoPIA1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotOverrideInAccessibleMember As String
			Get
				Return VBResources.GetResourceString("ERR_CannotOverrideInAccessibleMember", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotUseGenericTypeAcrossAssemblyBoundaries As String
			Get
				Return VBResources.GetResourceString("ERR_CannotUseGenericTypeAcrossAssemblyBoundaries", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CannotUseOnErrorGotoWithClosure As String
			Get
				Return VBResources.GetResourceString("ERR_CannotUseOnErrorGotoWithClosure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantAssignToConst As String
			Get
				Return VBResources.GetResourceString("ERR_CantAssignToConst", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantAwaitAsyncSub1 As String
			Get
				Return VBResources.GetResourceString("ERR_CantAwaitAsyncSub1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantCallIIF As String
			Get
				Return VBResources.GetResourceString("ERR_CantCallIIF", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantCombineInitializers As String
			Get
				Return VBResources.GetResourceString("ERR_CantCombineInitializers", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantHaveWin32ResAndManifest As String
			Get
				Return VBResources.GetResourceString("ERR_CantHaveWin32ResAndManifest", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantOpenFileWrite As String
			Get
				Return VBResources.GetResourceString("ERR_CantOpenFileWrite", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantOverride4 As String
			Get
				Return VBResources.GetResourceString("ERR_CantOverride4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantOverrideConstructor As String
			Get
				Return VBResources.GetResourceString("ERR_CantOverrideConstructor", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantOverrideNotOverridable2 As String
			Get
				Return VBResources.GetResourceString("ERR_CantOverrideNotOverridable2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantRaiseBaseEvent As String
			Get
				Return VBResources.GetResourceString("ERR_CantRaiseBaseEvent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantReadRulesetFile As String
			Get
				Return VBResources.GetResourceString("ERR_CantReadRulesetFile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantReferToMyGroupInsideGroupType1 As String
			Get
				Return VBResources.GetResourceString("ERR_CantReferToMyGroupInsideGroupType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantShadowAMustOverride1 As String
			Get
				Return VBResources.GetResourceString("ERR_CantShadowAMustOverride1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantSpecifyArrayAndNullableOnBoth As String
			Get
				Return VBResources.GetResourceString("ERR_CantSpecifyArrayAndNullableOnBoth", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantSpecifyArraysOnBoth As String
			Get
				Return VBResources.GetResourceString("ERR_CantSpecifyArraysOnBoth", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantSpecifyAsNewAndNullable As String
			Get
				Return VBResources.GetResourceString("ERR_CantSpecifyAsNewAndNullable", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantSpecifyNullableOnBoth As String
			Get
				Return VBResources.GetResourceString("ERR_CantSpecifyNullableOnBoth", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantSpecifyParamsOnLambdaParamNoType As String
			Get
				Return VBResources.GetResourceString("ERR_CantSpecifyParamsOnLambdaParamNoType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantSpecifyTypeCharacterOnIIF As String
			Get
				Return VBResources.GetResourceString("ERR_CantSpecifyTypeCharacterOnIIF", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantThrowNonException As String
			Get
				Return VBResources.GetResourceString("ERR_CantThrowNonException", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CantUseRequiredAttribute As String
			Get
				Return VBResources.GetResourceString("ERR_CantUseRequiredAttribute", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CaseAfterCaseElse As String
			Get
				Return VBResources.GetResourceString("ERR_CaseAfterCaseElse", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CaseElseNoSelect As String
			Get
				Return VBResources.GetResourceString("ERR_CaseElseNoSelect", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CaseNoSelect As String
			Get
				Return VBResources.GetResourceString("ERR_CaseNoSelect", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CatchAfterFinally As String
			Get
				Return VBResources.GetResourceString("ERR_CatchAfterFinally", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CatchNoMatchingTry As String
			Get
				Return VBResources.GetResourceString("ERR_CatchNoMatchingTry", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CatchNotException1 As String
			Get
				Return VBResources.GetResourceString("ERR_CatchNotException1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CatchVariableNotLocal1 As String
			Get
				Return VBResources.GetResourceString("ERR_CatchVariableNotLocal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CharToIntegralTypeMismatch1 As String
			Get
				Return VBResources.GetResourceString("ERR_CharToIntegralTypeMismatch1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CircularBaseDependencies4 As String
			Get
				Return VBResources.GetResourceString("ERR_CircularBaseDependencies4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CircularEvaluation1 As String
			Get
				Return VBResources.GetResourceString("ERR_CircularEvaluation1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CircularInference1 As String
			Get
				Return VBResources.GetResourceString("ERR_CircularInference1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ClashWithReservedEnumMember1 As String
			Get
				Return VBResources.GetResourceString("ERR_ClashWithReservedEnumMember1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ClassConstraintNotInheritable1 As String
			Get
				Return VBResources.GetResourceString("ERR_ClassConstraintNotInheritable1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ClassInheritsBaseUnifiesWithInterfaces3 As String
			Get
				Return VBResources.GetResourceString("ERR_ClassInheritsBaseUnifiesWithInterfaces3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ClassInheritsInterfaceBaseUnifiesWithBase4 As String
			Get
				Return VBResources.GetResourceString("ERR_ClassInheritsInterfaceBaseUnifiesWithBase4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ClassInheritsInterfaceUnifiesWithBase3 As String
			Get
				Return VBResources.GetResourceString("ERR_ClassInheritsInterfaceUnifiesWithBase3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ClassNotExpression1 As String
			Get
				Return VBResources.GetResourceString("ERR_ClassNotExpression1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CmdOptionConflictsSource As String
			Get
				Return VBResources.GetResourceString("ERR_CmdOptionConflictsSource", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CoClassMissing2 As String
			Get
				Return VBResources.GetResourceString("ERR_CoClassMissing2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CollisionWithPublicTypeInModule As String
			Get
				Return VBResources.GetResourceString("ERR_CollisionWithPublicTypeInModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ComClassAndReservedAttribute1 As String
			Get
				Return VBResources.GetResourceString("ERR_ComClassAndReservedAttribute1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ComClassCantBeAbstract0 As String
			Get
				Return VBResources.GetResourceString("ERR_ComClassCantBeAbstract0", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ComClassDuplicateGuids1 As String
			Get
				Return VBResources.GetResourceString("ERR_ComClassDuplicateGuids1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ComClassGenericMethod As String
			Get
				Return VBResources.GetResourceString("ERR_ComClassGenericMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ComClassOnGeneric As String
			Get
				Return VBResources.GetResourceString("ERR_ComClassOnGeneric", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ComClassRequiresPublicClass1 As String
			Get
				Return VBResources.GetResourceString("ERR_ComClassRequiresPublicClass1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ComClassRequiresPublicClass2 As String
			Get
				Return VBResources.GetResourceString("ERR_ComClassRequiresPublicClass2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ComClassReservedDispId1 As String
			Get
				Return VBResources.GetResourceString("ERR_ComClassReservedDispId1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ComClassReservedDispIdZero1 As String
			Get
				Return VBResources.GetResourceString("ERR_ComClassReservedDispIdZero1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CommentsAfterLineContinuationNotAvailable1 As String
			Get
				Return VBResources.GetResourceString("ERR_CommentsAfterLineContinuationNotAvailable1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConditionalCompilationConstantNotValid As String
			Get
				Return VBResources.GetResourceString("ERR_ConditionalCompilationConstantNotValid", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConditionOperatorRequired3 As String
			Get
				Return VBResources.GetResourceString("ERR_ConditionOperatorRequired3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConflictDefaultPropertyAttribute As String
			Get
				Return VBResources.GetResourceString("ERR_ConflictDefaultPropertyAttribute", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConflictingDirectConstraints3 As String
			Get
				Return VBResources.GetResourceString("ERR_ConflictingDirectConstraints3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConflictingMachineModule As String
			Get
				Return VBResources.GetResourceString("ERR_ConflictingMachineModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConflictingManifestSwitches As String
			Get
				Return VBResources.GetResourceString("ERR_ConflictingManifestSwitches", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstantStringTooLong As String
			Get
				Return VBResources.GetResourceString("ERR_ConstantStringTooLong", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstantWithNoValue As String
			Get
				Return VBResources.GetResourceString("ERR_ConstantWithNoValue", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstAsNonConstant As String
			Get
				Return VBResources.GetResourceString("ERR_ConstAsNonConstant", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstNotClassInterfaceOrTypeParam1 As String
			Get
				Return VBResources.GetResourceString("ERR_ConstNotClassInterfaceOrTypeParam1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstraintAlreadyExists1 As String
			Get
				Return VBResources.GetResourceString("ERR_ConstraintAlreadyExists1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstraintClashDirectIndirect3 As String
			Get
				Return VBResources.GetResourceString("ERR_ConstraintClashDirectIndirect3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstraintClashIndirectDirect3 As String
			Get
				Return VBResources.GetResourceString("ERR_ConstraintClashIndirectDirect3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstraintClashIndirectIndirect4 As String
			Get
				Return VBResources.GetResourceString("ERR_ConstraintClashIndirectIndirect4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstraintCycle2 As String
			Get
				Return VBResources.GetResourceString("ERR_ConstraintCycle2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstraintCycleLink2 As String
			Get
				Return VBResources.GetResourceString("ERR_ConstraintCycleLink2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstraintIsRestrictedType1 As String
			Get
				Return VBResources.GetResourceString("ERR_ConstraintIsRestrictedType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstructorAsync As String
			Get
				Return VBResources.GetResourceString("ERR_ConstructorAsync", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstructorCannotBeDeclaredPartial As String
			Get
				Return VBResources.GetResourceString("ERR_ConstructorCannotBeDeclaredPartial", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstructorFunction As String
			Get
				Return VBResources.GetResourceString("ERR_ConstructorFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConstructorNotFound1 As String
			Get
				Return VBResources.GetResourceString("ERR_ConstructorNotFound1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ContinueDoNotWithinDo As String
			Get
				Return VBResources.GetResourceString("ERR_ContinueDoNotWithinDo", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ContinueForNotWithinFor As String
			Get
				Return VBResources.GetResourceString("ERR_ContinueForNotWithinFor", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ContinueWhileNotWithinWhile As String
			Get
				Return VBResources.GetResourceString("ERR_ContinueWhileNotWithinWhile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConversionFromBaseType As String
			Get
				Return VBResources.GetResourceString("ERR_ConversionFromBaseType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConversionFromDerivedType As String
			Get
				Return VBResources.GetResourceString("ERR_ConversionFromDerivedType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConversionFromInterfaceType As String
			Get
				Return VBResources.GetResourceString("ERR_ConversionFromInterfaceType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConversionFromObject As String
			Get
				Return VBResources.GetResourceString("ERR_ConversionFromObject", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConversionToBaseType As String
			Get
				Return VBResources.GetResourceString("ERR_ConversionToBaseType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConversionToDerivedType As String
			Get
				Return VBResources.GetResourceString("ERR_ConversionToDerivedType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConversionToInterfaceType As String
			Get
				Return VBResources.GetResourceString("ERR_ConversionToInterfaceType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConversionToObject As String
			Get
				Return VBResources.GetResourceString("ERR_ConversionToObject", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConversionToSameType As String
			Get
				Return VBResources.GetResourceString("ERR_ConversionToSameType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConvertArrayMismatch4 As String
			Get
				Return VBResources.GetResourceString("ERR_ConvertArrayMismatch4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConvertArrayRankMismatch2 As String
			Get
				Return VBResources.GetResourceString("ERR_ConvertArrayRankMismatch2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConvertObjectArrayMismatch3 As String
			Get
				Return VBResources.GetResourceString("ERR_ConvertObjectArrayMismatch3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConvMustBeWideningOrNarrowing As String
			Get
				Return VBResources.GetResourceString("ERR_ConvMustBeWideningOrNarrowing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ConvParamMustBeContainingType1 As String
			Get
				Return VBResources.GetResourceString("ERR_ConvParamMustBeContainingType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CopyBackTypeMismatch3 As String
			Get
				Return VBResources.GetResourceString("ERR_CopyBackTypeMismatch3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CryptoHashFailed As String
			Get
				Return VBResources.GetResourceString("ERR_CryptoHashFailed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CustomEventInvInInterface As String
			Get
				Return VBResources.GetResourceString("ERR_CustomEventInvInInterface", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_CustomEventRequiresAs As String
			Get
				Return VBResources.GetResourceString("ERR_CustomEventRequiresAs", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DateToDoubleConversion As String
			Get
				Return VBResources.GetResourceString("ERR_DateToDoubleConversion", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DebugEntryPointNotSourceMethodDefinition As String
			Get
				Return VBResources.GetResourceString("ERR_DebugEntryPointNotSourceMethodDefinition", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DeclaresCantBeInGeneric As String
			Get
				Return VBResources.GetResourceString("ERR_DeclaresCantBeInGeneric", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DefaultEventNotFound1 As String
			Get
				Return VBResources.GetResourceString("ERR_DefaultEventNotFound1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DefaultInterfaceImplementationInNoPIAType As String
			Get
				Return VBResources.GetResourceString("ERR_DefaultInterfaceImplementationInNoPIAType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DefaultMemberNotProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_DefaultMemberNotProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DefaultMissingFromProperty2 As String
			Get
				Return VBResources.GetResourceString("ERR_DefaultMissingFromProperty2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DefaultPropertyAmbiguousAcrossInterfaces4 As String
			Get
				Return VBResources.GetResourceString("ERR_DefaultPropertyAmbiguousAcrossInterfaces4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DefaultPropertyWithNoParams As String
			Get
				Return VBResources.GetResourceString("ERR_DefaultPropertyWithNoParams", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DefaultValueForNonOptionalParam As String
			Get
				Return VBResources.GetResourceString("ERR_DefaultValueForNonOptionalParam", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DelegateBindingFailure3 As String
			Get
				Return VBResources.GetResourceString("ERR_DelegateBindingFailure3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DelegateBindingIncompatible2 As String
			Get
				Return VBResources.GetResourceString("ERR_DelegateBindingIncompatible2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DelegateBindingIncompatible3 As String
			Get
				Return VBResources.GetResourceString("ERR_DelegateBindingIncompatible3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DelegateBindingMismatch As String
			Get
				Return VBResources.GetResourceString("ERR_DelegateBindingMismatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DelegateBindingMismatchStrictOff2 As String
			Get
				Return VBResources.GetResourceString("ERR_DelegateBindingMismatchStrictOff2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DelegateBindingMismatchStrictOff3 As String
			Get
				Return VBResources.GetResourceString("ERR_DelegateBindingMismatchStrictOff3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DelegateBindingTypeInferenceFails As String
			Get
				Return VBResources.GetResourceString("ERR_DelegateBindingTypeInferenceFails", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DelegateCantHandleEvents As String
			Get
				Return VBResources.GetResourceString("ERR_DelegateCantHandleEvents", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DelegateCantImplement As String
			Get
				Return VBResources.GetResourceString("ERR_DelegateCantImplement", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DelegateNoInvoke1 As String
			Get
				Return VBResources.GetResourceString("ERR_DelegateNoInvoke1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DllImportNotLegalOnDeclare As String
			Get
				Return VBResources.GetResourceString("ERR_DllImportNotLegalOnDeclare", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DllImportNotLegalOnEventMethod As String
			Get
				Return VBResources.GetResourceString("ERR_DllImportNotLegalOnEventMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DllImportNotLegalOnGetOrSet As String
			Get
				Return VBResources.GetResourceString("ERR_DllImportNotLegalOnGetOrSet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DllImportOnGenericSubOrFunction As String
			Get
				Return VBResources.GetResourceString("ERR_DllImportOnGenericSubOrFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DllImportOnInstanceMethod As String
			Get
				Return VBResources.GetResourceString("ERR_DllImportOnInstanceMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DllImportOnInterfaceMethod As String
			Get
				Return VBResources.GetResourceString("ERR_DllImportOnInterfaceMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DllImportOnNonEmptySubOrFunction As String
			Get
				Return VBResources.GetResourceString("ERR_DllImportOnNonEmptySubOrFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DllImportOnResumableMethod As String
			Get
				Return VBResources.GetResourceString("ERR_DllImportOnResumableMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DocFileGen As String
			Get
				Return VBResources.GetResourceString("ERR_DocFileGen", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DoesntImplementAwaitInterface2 As String
			Get
				Return VBResources.GetResourceString("ERR_DoesntImplementAwaitInterface2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DoubleToDateConversion As String
			Get
				Return VBResources.GetResourceString("ERR_DoubleToDateConversion", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DTDNotSupported As String
			Get
				Return VBResources.GetResourceString("ERR_DTDNotSupported", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateAccessCategoryUsed As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateAccessCategoryUsed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateAddHandlerDef As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateAddHandlerDef", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateAggrMemberInit1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateAggrMemberInit1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateAnonTypeMemberName1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateAnonTypeMemberName1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateConversionCategoryUsed As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateConversionCategoryUsed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateDefaultProps1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateDefaultProps1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateImport1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateImport1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateInInherits1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateInInherits1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateLocals1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateLocals1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateLocalStatic1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateLocalStatic1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateLocalTypes3 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateLocalTypes3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateModifierCategoryUsed As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateModifierCategoryUsed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateNamedImportAlias1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateNamedImportAlias1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateOption1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateOption1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateParameterSpecifier As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateParameterSpecifier", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateParamName1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateParamName1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicatePrefix As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicatePrefix", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateProcDef1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateProcDef1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateProcDefWithDifferentTupleNames2 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateProcDefWithDifferentTupleNames2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicatePropertyGet As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicatePropertyGet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicatePropertySet As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicatePropertySet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateRaiseEventDef As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateRaiseEventDef", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateRawGenericTypeImport1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateRawGenericTypeImport1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateReference2 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateReference2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateReferenceStrong As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateReferenceStrong", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateRemoveHandlerDef As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateRemoveHandlerDef", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateResourceFileName1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateResourceFileName1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateResourceName1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateResourceName1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateSpecifier As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateSpecifier", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateTypeParamName1 As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateTypeParamName1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateWriteabilityCategoryUsed As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateWriteabilityCategoryUsed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_DuplicateXmlAttribute As String
			Get
				Return VBResources.GetResourceString("ERR_DuplicateXmlAttribute", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ElseIfNoMatchingIf As String
			Get
				Return VBResources.GetResourceString("ERR_ElseIfNoMatchingIf", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ElseNoMatchingIf As String
			Get
				Return VBResources.GetResourceString("ERR_ElseNoMatchingIf", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EmbeddedExpression As String
			Get
				Return VBResources.GetResourceString("ERR_EmbeddedExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EmptyAggregateInitializer As String
			Get
				Return VBResources.GetResourceString("ERR_EmptyAggregateInitializer", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EncNoPIAReference As String
			Get
				Return VBResources.GetResourceString("ERR_EncNoPIAReference", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EncodinglessSyntaxTree As String
			Get
				Return VBResources.GetResourceString("ERR_EncodinglessSyntaxTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EncReferenceToAddedMember As String
			Get
				Return VBResources.GetResourceString("ERR_EncReferenceToAddedMember", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EncUpdateFailedMissingAttribute As String
			Get
				Return VBResources.GetResourceString("ERR_EncUpdateFailedMissingAttribute", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndClassNoClass As String
			Get
				Return VBResources.GetResourceString("ERR_EndClassNoClass", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndDisallowedInDllProjects As String
			Get
				Return VBResources.GetResourceString("ERR_EndDisallowedInDllProjects", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndExternalSource As String
			Get
				Return VBResources.GetResourceString("ERR_EndExternalSource", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndFunctionExpected As String
			Get
				Return VBResources.GetResourceString("ERR_EndFunctionExpected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndIfNoMatchingIf As String
			Get
				Return VBResources.GetResourceString("ERR_EndIfNoMatchingIf", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndModuleNoModule As String
			Get
				Return VBResources.GetResourceString("ERR_EndModuleNoModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndNamespaceNoNamespace As String
			Get
				Return VBResources.GetResourceString("ERR_EndNamespaceNoNamespace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndOperatorExpected As String
			Get
				Return VBResources.GetResourceString("ERR_EndOperatorExpected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndOperatorNotAtLineStart As String
			Get
				Return VBResources.GetResourceString("ERR_EndOperatorNotAtLineStart", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndProp As String
			Get
				Return VBResources.GetResourceString("ERR_EndProp", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndRegionNoRegion As String
			Get
				Return VBResources.GetResourceString("ERR_EndRegionNoRegion", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndSelectNoSelect As String
			Get
				Return VBResources.GetResourceString("ERR_EndSelectNoSelect", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndStructureNoStructure As String
			Get
				Return VBResources.GetResourceString("ERR_EndStructureNoStructure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndSubExpected As String
			Get
				Return VBResources.GetResourceString("ERR_EndSubExpected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndSyncLockNoSyncLock As String
			Get
				Return VBResources.GetResourceString("ERR_EndSyncLockNoSyncLock", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndTryNoTry As String
			Get
				Return VBResources.GetResourceString("ERR_EndTryNoTry", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndUsingWithoutUsing As String
			Get
				Return VBResources.GetResourceString("ERR_EndUsingWithoutUsing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndWhileNoWhile As String
			Get
				Return VBResources.GetResourceString("ERR_EndWhileNoWhile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EndWithWithoutWith As String
			Get
				Return VBResources.GetResourceString("ERR_EndWithWithoutWith", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EnumNotExpression1 As String
			Get
				Return VBResources.GetResourceString("ERR_EnumNotExpression1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EqualsOperandIsBad As String
			Get
				Return VBResources.GetResourceString("ERR_EqualsOperandIsBad", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EqualsTypeMismatch As String
			Get
				Return VBResources.GetResourceString("ERR_EqualsTypeMismatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ErrorCreatingWin32ResourceFile As String
			Get
				Return VBResources.GetResourceString("ERR_ErrorCreatingWin32ResourceFile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventAddRemoveByrefParamIllegal As String
			Get
				Return VBResources.GetResourceString("ERR_EventAddRemoveByrefParamIllegal", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventAddRemoveHasOnlyOneParam As String
			Get
				Return VBResources.GetResourceString("ERR_EventAddRemoveHasOnlyOneParam", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventDelegatesCantBeFunctions As String
			Get
				Return VBResources.GetResourceString("ERR_EventDelegatesCantBeFunctions", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventHandlerSignatureIncompatible2 As String
			Get
				Return VBResources.GetResourceString("ERR_EventHandlerSignatureIncompatible2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventImplMismatch5 As String
			Get
				Return VBResources.GetResourceString("ERR_EventImplMismatch5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventImplRemoveHandlerParamWrong As String
			Get
				Return VBResources.GetResourceString("ERR_EventImplRemoveHandlerParamWrong", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventMethodOptionalParamIllegal1 As String
			Get
				Return VBResources.GetResourceString("ERR_EventMethodOptionalParamIllegal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventNoPIANoBackingMember As String
			Get
				Return VBResources.GetResourceString("ERR_EventNoPIANoBackingMember", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventNotFound1 As String
			Get
				Return VBResources.GetResourceString("ERR_EventNotFound1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventsCantBeFunctions As String
			Get
				Return VBResources.GetResourceString("ERR_EventsCantBeFunctions", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventSourceIsArray As String
			Get
				Return VBResources.GetResourceString("ERR_EventSourceIsArray", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_EventTypeNotDelegate As String
			Get
				Return VBResources.GetResourceString("ERR_EventTypeNotDelegate", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExecutableAsDeclaration As String
			Get
				Return VBResources.GetResourceString("ERR_ExecutableAsDeclaration", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExitDoNotWithinDo As String
			Get
				Return VBResources.GetResourceString("ERR_ExitDoNotWithinDo", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExitEventMemberNotInvalid As String
			Get
				Return VBResources.GetResourceString("ERR_ExitEventMemberNotInvalid", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExitForNotWithinFor As String
			Get
				Return VBResources.GetResourceString("ERR_ExitForNotWithinFor", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExitFuncOfSub As String
			Get
				Return VBResources.GetResourceString("ERR_ExitFuncOfSub", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExitOperatorNotValid As String
			Get
				Return VBResources.GetResourceString("ERR_ExitOperatorNotValid", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExitPropNot As String
			Get
				Return VBResources.GetResourceString("ERR_ExitPropNot", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExitSelectNotWithinSelect As String
			Get
				Return VBResources.GetResourceString("ERR_ExitSelectNotWithinSelect", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExitSubOfFunc As String
			Get
				Return VBResources.GetResourceString("ERR_ExitSubOfFunc", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExitTryNotWithinTry As String
			Get
				Return VBResources.GetResourceString("ERR_ExitTryNotWithinTry", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExitWhileNotWithinWhile As String
			Get
				Return VBResources.GetResourceString("ERR_ExitWhileNotWithinWhile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedAnd As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedAnd", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedArray1 As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedArray1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedAs As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedAs", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedAssignmentOperator As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedAssignmentOperator", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedAssignmentOperatorInInit As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedAssignmentOperatorInInit", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedBy As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedBy", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedCase As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedCase", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedComma As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedComma", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedConditionalDirective As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedConditionalDirective", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedContinueKind As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedContinueKind", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedDeclaration As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedDeclaration", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedDiv As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedDiv", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedDot As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedDot", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedDotAfterGlobalNameSpace As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedDotAfterGlobalNameSpace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedDotAfterMyBase As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedDotAfterMyBase", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedDotAfterMyClass As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedDotAfterMyClass", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndClass As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndClass", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndExternalSource As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndExternalSource", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndIf As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndIf", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndModule As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndNamespace As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndNamespace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndOfExpression As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndOfExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndRegion As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndRegion", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndSelect As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndSelect", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndStructure As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndStructure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndSyncLock As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndSyncLock", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndTry As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndTry", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndUsing As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndUsing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndWhile As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndWhile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEndWith As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEndWith", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEOS As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEOS", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEQ As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEQ", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedEquals As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedEquals", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedExitKind As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedExitKind", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedExpression As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedForOptionStmt As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedForOptionStmt", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedFrom As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedFrom", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedGreater As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedGreater", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedIdentifier As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedIdentifier", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedIdentifierOrGroup As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedIdentifierOrGroup", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedIn As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedIn", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedInOrEq As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedInOrEq", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedIntLiteral As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedIntLiteral", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedInto As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedInto", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedJoin As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedJoin", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedLbrace As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedLbrace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedLoop As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedLoop", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedLparen As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedLparen", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedLT As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedLT", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedMinus As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedMinus", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedNamedArgument As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedNamedArgument", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedNamedArgumentInAttributeList As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedNamedArgumentInAttributeList", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedNext As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedNext", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedOn As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedOn", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedOptional As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedOptional", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedOptionCompare As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedOptionCompare", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedProcedure As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedProcedure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedQualifiedNameInInit As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedQualifiedNameInInit", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedQueryableSource As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedQueryableSource", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedQuote As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedQuote", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedRbrace As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedRbrace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedRelational As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedRelational", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedResumeOrGoto As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedResumeOrGoto", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedRparen As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedRparen", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedSColon As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedSColon", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedSingleScript As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedSingleScript", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedSpecifier As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedSpecifier", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedSQuote As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedSQuote", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedStringLiteral As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedStringLiteral", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedSubFunction As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedSubFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedSubOrFunction As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedSubOrFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedWarningKeyword As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedWarningKeyword", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedXmlBeginEmbedded As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedXmlBeginEmbedded", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedXmlEndCData As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedXmlEndCData", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedXmlEndComment As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedXmlEndComment", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedXmlEndEmbedded As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedXmlEndEmbedded", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedXmlEndPI As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedXmlEndPI", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedXmlName As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedXmlName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedXmlns As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedXmlns", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpectedXmlWhiteSpace As String
			Get
				Return VBResources.GetResourceString("ERR_ExpectedXmlWhiteSpace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExplicitTupleElementNamesAttribute As String
			Get
				Return VBResources.GetResourceString("ERR_ExplicitTupleElementNamesAttribute", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExportedTypeConflictsWithDeclaration As String
			Get
				Return VBResources.GetResourceString("ERR_ExportedTypeConflictsWithDeclaration", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExportedTypesConflict As String
			Get
				Return VBResources.GetResourceString("ERR_ExportedTypesConflict", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpressionDoesntHaveName As String
			Get
				Return VBResources.GetResourceString("ERR_ExpressionDoesntHaveName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpressionOverflow1 As String
			Get
				Return VBResources.GetResourceString("ERR_ExpressionOverflow1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExpressionTreeNotSupported As String
			Get
				Return VBResources.GetResourceString("ERR_ExpressionTreeNotSupported", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExprTreeNoLateBind As String
			Get
				Return VBResources.GetResourceString("ERR_ExprTreeNoLateBind", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExprTreeNoMultiDimArrayCreation As String
			Get
				Return VBResources.GetResourceString("ERR_ExprTreeNoMultiDimArrayCreation", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtensionAttributeInvalid As String
			Get
				Return VBResources.GetResourceString("ERR_ExtensionAttributeInvalid", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtensionMethodCannotBeLateBound As String
			Get
				Return VBResources.GetResourceString("ERR_ExtensionMethodCannotBeLateBound", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtensionMethodNoParams As String
			Get
				Return VBResources.GetResourceString("ERR_ExtensionMethodNoParams", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtensionMethodNotInModule As String
			Get
				Return VBResources.GetResourceString("ERR_ExtensionMethodNotInModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtensionMethodOptionalFirstArg As String
			Get
				Return VBResources.GetResourceString("ERR_ExtensionMethodOptionalFirstArg", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtensionMethodOverloadCandidate2 As String
			Get
				Return VBResources.GetResourceString("ERR_ExtensionMethodOverloadCandidate2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtensionMethodOverloadCandidate3 As String
			Get
				Return VBResources.GetResourceString("ERR_ExtensionMethodOverloadCandidate3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtensionMethodParamArrayFirstArg As String
			Get
				Return VBResources.GetResourceString("ERR_ExtensionMethodParamArrayFirstArg", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtensionMethodUncallable1 As String
			Get
				Return VBResources.GetResourceString("ERR_ExtensionMethodUncallable1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtensionOnlyAllowedOnModuleSubOrFunction As String
			Get
				Return VBResources.GetResourceString("ERR_ExtensionOnlyAllowedOnModuleSubOrFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtraNextVariable As String
			Get
				Return VBResources.GetResourceString("ERR_ExtraNextVariable", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ExtraSpecifiers As String
			Get
				Return VBResources.GetResourceString("ERR_ExtraSpecifiers", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FailureSigningAssembly As String
			Get
				Return VBResources.GetResourceString("ERR_FailureSigningAssembly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FieldHasMultipleDistinctConstantValues As String
			Get
				Return VBResources.GetResourceString("ERR_FieldHasMultipleDistinctConstantValues", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FieldOfValueFieldOfMarshalByRef3 As String
			Get
				Return VBResources.GetResourceString("ERR_FieldOfValueFieldOfMarshalByRef3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FileAttributeNotAssemblyOrModule As String
			Get
				Return VBResources.GetResourceString("ERR_FileAttributeNotAssemblyOrModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FileNotFound As String
			Get
				Return VBResources.GetResourceString("ERR_FileNotFound", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FinallyAfterFinally As String
			Get
				Return VBResources.GetResourceString("ERR_FinallyAfterFinally", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FinallyNoMatchingTry As String
			Get
				Return VBResources.GetResourceString("ERR_FinallyNoMatchingTry", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ForCtlVarArraySizesSpecified As String
			Get
				Return VBResources.GetResourceString("ERR_ForCtlVarArraySizesSpecified", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ForEachAmbiguousIEnumerable1 As String
			Get
				Return VBResources.GetResourceString("ERR_ForEachAmbiguousIEnumerable1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ForEachCollectionDesignPattern1 As String
			Get
				Return VBResources.GetResourceString("ERR_ForEachCollectionDesignPattern1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ForIndexInUse1 As String
			Get
				Return VBResources.GetResourceString("ERR_ForIndexInUse1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ForLoopOperatorRequired2 As String
			Get
				Return VBResources.GetResourceString("ERR_ForLoopOperatorRequired2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ForLoopType1 As String
			Get
				Return VBResources.GetResourceString("ERR_ForLoopType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ForwardedTypeConflictsWithDeclaration As String
			Get
				Return VBResources.GetResourceString("ERR_ForwardedTypeConflictsWithDeclaration", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ForwardedTypeConflictsWithExportedType As String
			Get
				Return VBResources.GetResourceString("ERR_ForwardedTypeConflictsWithExportedType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ForwardedTypesConflict As String
			Get
				Return VBResources.GetResourceString("ERR_ForwardedTypesConflict", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ForwardedTypeUnavailable3 As String
			Get
				Return VBResources.GetResourceString("ERR_ForwardedTypeUnavailable3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FriendAssemblyBadAccessOverride2 As String
			Get
				Return VBResources.GetResourceString("ERR_FriendAssemblyBadAccessOverride2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FriendAssemblyBadArguments As String
			Get
				Return VBResources.GetResourceString("ERR_FriendAssemblyBadArguments", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FriendAssemblyNameInvalid As String
			Get
				Return VBResources.GetResourceString("ERR_FriendAssemblyNameInvalid", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FriendAssemblyStrongNameRequired As String
			Get
				Return VBResources.GetResourceString("ERR_FriendAssemblyStrongNameRequired", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FriendRefNotEqualToThis As String
			Get
				Return VBResources.GetResourceString("ERR_FriendRefNotEqualToThis", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FriendRefSigningMismatch As String
			Get
				Return VBResources.GetResourceString("ERR_FriendRefSigningMismatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FullWidthAsXmlDelimiter As String
			Get
				Return VBResources.GetResourceString("ERR_FullWidthAsXmlDelimiter", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_FunctionResultCannotBeIndexed1 As String
			Get
				Return VBResources.GetResourceString("ERR_FunctionResultCannotBeIndexed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GeneralProjectImportsError3 As String
			Get
				Return VBResources.GetResourceString("ERR_GeneralProjectImportsError3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GenericArgsOnAttributeSpecifier As String
			Get
				Return VBResources.GetResourceString("ERR_GenericArgsOnAttributeSpecifier", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GenericClassCannotInheritAttr As String
			Get
				Return VBResources.GetResourceString("ERR_GenericClassCannotInheritAttr", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GenericConstraintNotSatisfied2 As String
			Get
				Return VBResources.GetResourceString("ERR_GenericConstraintNotSatisfied2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GenericParamBase2 As String
			Get
				Return VBResources.GetResourceString("ERR_GenericParamBase2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GenericParamsOnInvalidMember As String
			Get
				Return VBResources.GetResourceString("ERR_GenericParamsOnInvalidMember", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GenericSubMainsFound1 As String
			Get
				Return VBResources.GetResourceString("ERR_GenericSubMainsFound1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GotoIntoFor As String
			Get
				Return VBResources.GetResourceString("ERR_GotoIntoFor", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GotoIntoSyncLock As String
			Get
				Return VBResources.GetResourceString("ERR_GotoIntoSyncLock", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GotoIntoTryHandler As String
			Get
				Return VBResources.GetResourceString("ERR_GotoIntoTryHandler", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GotoIntoUsing As String
			Get
				Return VBResources.GetResourceString("ERR_GotoIntoUsing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_GotoIntoWith As String
			Get
				Return VBResources.GetResourceString("ERR_GotoIntoWith", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_HandlesInvalidOnGenericMethod As String
			Get
				Return VBResources.GetResourceString("ERR_HandlesInvalidOnGenericMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_HandlesSyntaxInClass As String
			Get
				Return VBResources.GetResourceString("ERR_HandlesSyntaxInClass", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_HandlesSyntaxInModule As String
			Get
				Return VBResources.GetResourceString("ERR_HandlesSyntaxInModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IconFileAndWin32ResFile As String
			Get
				Return VBResources.GetResourceString("ERR_IconFileAndWin32ResFile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IdentityDirectCastForFloat As String
			Get
				Return VBResources.GetResourceString("ERR_IdentityDirectCastForFloat", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IdentNotMemberOfInterface4 As String
			Get
				Return VBResources.GetResourceString("ERR_IdentNotMemberOfInterface4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IfNoType As String
			Get
				Return VBResources.GetResourceString("ERR_IfNoType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IfNoTypeObjectDisallowed As String
			Get
				Return VBResources.GetResourceString("ERR_IfNoTypeObjectDisallowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IfTooManyTypesObjectDisallowed As String
			Get
				Return VBResources.GetResourceString("ERR_IfTooManyTypesObjectDisallowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalAttributeInXmlDecl As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalAttributeInXmlDecl", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalBaseTypeReferences3 As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalBaseTypeReferences3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalCallOrIndex As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalCallOrIndex", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalChar As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalChar", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalCharConstant As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalCharConstant", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalCondTypeInIIF As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalCondTypeInIIF", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalDefaultNamespace As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalDefaultNamespace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalOperandInIIFConversion As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalOperandInIIFConversion", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalOperandInIIFConversion2 As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalOperandInIIFConversion2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalOperandInIIFCount As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalOperandInIIFCount", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalOperandInIIFName As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalOperandInIIFName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalProcessingInstructionName As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalProcessingInstructionName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalXmlCommentChar As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalXmlCommentChar", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalXmlNameChar As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalXmlNameChar", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalXmlnsPrefix As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalXmlnsPrefix", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalXmlStartNameChar As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalXmlStartNameChar", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IllegalXmlWhiteSpace As String
			Get
				Return VBResources.GetResourceString("ERR_IllegalXmlWhiteSpace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ImplementationMustBePrivate2 As String
			Get
				Return VBResources.GetResourceString("ERR_ImplementationMustBePrivate2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ImplementingInterfaceWithDifferentTupleNames5 As String
			Get
				Return VBResources.GetResourceString("ERR_ImplementingInterfaceWithDifferentTupleNames5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ImplementsGenericParam As String
			Get
				Return VBResources.GetResourceString("ERR_ImplementsGenericParam", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ImplementsOnNew As String
			Get
				Return VBResources.GetResourceString("ERR_ImplementsOnNew", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ImplementsStmtWrongOrder As String
			Get
				Return VBResources.GetResourceString("ERR_ImplementsStmtWrongOrder", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ImplementsWithConstraintMismatch3 As String
			Get
				Return VBResources.GetResourceString("ERR_ImplementsWithConstraintMismatch3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ImportAliasConflictsWithType2 As String
			Get
				Return VBResources.GetResourceString("ERR_ImportAliasConflictsWithType2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ImportsMustBeFirst As String
			Get
				Return VBResources.GetResourceString("ERR_ImportsMustBeFirst", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InAccessibleCoClass3 As String
			Get
				Return VBResources.GetResourceString("ERR_InAccessibleCoClass3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InaccessibleMember3 As String
			Get
				Return VBResources.GetResourceString("ERR_InaccessibleMember3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InAccessibleOverridingMethod5 As String
			Get
				Return VBResources.GetResourceString("ERR_InAccessibleOverridingMethod5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InaccessibleReturnTypeOfMember2 As String
			Get
				Return VBResources.GetResourceString("ERR_InaccessibleReturnTypeOfMember2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InaccessibleSymbol2 As String
			Get
				Return VBResources.GetResourceString("ERR_InaccessibleSymbol2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IndexedNotArrayOrProc As String
			Get
				Return VBResources.GetResourceString("ERR_IndexedNotArrayOrProc", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IndirectUnreferencedAssembly4 As String
			Get
				Return VBResources.GetResourceString("ERR_IndirectUnreferencedAssembly4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InferringNonArrayType1 As String
			Get
				Return VBResources.GetResourceString("ERR_InferringNonArrayType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritanceAccessMismatch5 As String
			Get
				Return VBResources.GetResourceString("ERR_InheritanceAccessMismatch5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritanceAccessMismatchOutside3 As String
			Get
				Return VBResources.GetResourceString("ERR_InheritanceAccessMismatchOutside3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritanceCycle1 As String
			Get
				Return VBResources.GetResourceString("ERR_InheritanceCycle1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritanceCycleInImportedType1 As String
			Get
				Return VBResources.GetResourceString("ERR_InheritanceCycleInImportedType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritsFrom2 As String
			Get
				Return VBResources.GetResourceString("ERR_InheritsFrom2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritsFromCantInherit3 As String
			Get
				Return VBResources.GetResourceString("ERR_InheritsFromCantInherit3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritsFromNonClass As String
			Get
				Return VBResources.GetResourceString("ERR_InheritsFromNonClass", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritsFromNonInterface As String
			Get
				Return VBResources.GetResourceString("ERR_InheritsFromNonInterface", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritsFromRestrictedType1 As String
			Get
				Return VBResources.GetResourceString("ERR_InheritsFromRestrictedType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritsStmtWrongOrder As String
			Get
				Return VBResources.GetResourceString("ERR_InheritsStmtWrongOrder", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritsTypeArgAccessMismatch7 As String
			Get
				Return VBResources.GetResourceString("ERR_InheritsTypeArgAccessMismatch7", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InheritsTypeArgAccessMismatchOutside5 As String
			Get
				Return VBResources.GetResourceString("ERR_InheritsTypeArgAccessMismatchOutside5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InitializedExpandedProperty As String
			Get
				Return VBResources.GetResourceString("ERR_InitializedExpandedProperty", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InitializerExpected As String
			Get
				Return VBResources.GetResourceString("ERR_InitializerExpected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InitializerInStruct As String
			Get
				Return VBResources.GetResourceString("ERR_InitializerInStruct", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InitializerTooFewElements1 As String
			Get
				Return VBResources.GetResourceString("ERR_InitializerTooFewElements1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InitializerTooManyElements1 As String
			Get
				Return VBResources.GetResourceString("ERR_InitializerTooManyElements1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InitWithExplicitArraySizes As String
			Get
				Return VBResources.GetResourceString("ERR_InitWithExplicitArraySizes", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InitWithMultipleDeclarators As String
			Get
				Return VBResources.GetResourceString("ERR_InitWithMultipleDeclarators", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InReferencedAssembly As String
			Get
				Return VBResources.GetResourceString("ERR_InReferencedAssembly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IntegralToCharTypeMismatch1 As String
			Get
				Return VBResources.GetResourceString("ERR_IntegralToCharTypeMismatch1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceBaseUnifiesWithBase4 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceBaseUnifiesWithBase4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceCantUseEventSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceCantUseEventSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceCycle1 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceCycle1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceEventCantUse1 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceEventCantUse1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceImplementedTwice1 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceImplementedTwice1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceImplementedTwiceWithDifferentTupleNames2 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceImplementedTwiceWithDifferentTupleNames2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceImplementedTwiceWithDifferentTupleNames3 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceImplementedTwiceWithDifferentTupleNames3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceImplementedTwiceWithDifferentTupleNames4 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceImplementedTwiceWithDifferentTupleNames4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceImplementedTwiceWithDifferentTupleNamesReverse3 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceImplementedTwiceWithDifferentTupleNamesReverse3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceInheritedTwiceWithDifferentTupleNames2 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceInheritedTwiceWithDifferentTupleNames2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceInheritedTwiceWithDifferentTupleNames3 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceInheritedTwiceWithDifferentTupleNames3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceInheritedTwiceWithDifferentTupleNames4 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceInheritedTwiceWithDifferentTupleNames4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceInheritedTwiceWithDifferentTupleNamesReverse3 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceInheritedTwiceWithDifferentTupleNamesReverse3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceMemberSyntax As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceMemberSyntax", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceNoDefault1 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceNoDefault1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceNotExpression1 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceNotExpression1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceNotImplemented1 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceNotImplemented1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfacePossiblyImplTwice2 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfacePossiblyImplTwice2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceUnifiesWithBase3 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceUnifiesWithBase3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterfaceUnifiesWithInterface2 As String
			Get
				Return VBResources.GetResourceString("ERR_InterfaceUnifiesWithInterface2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InteropMethodWithBody1 As String
			Get
				Return VBResources.GetResourceString("ERR_InteropMethodWithBody1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterpolatedStringFactoryError As String
			Get
				Return VBResources.GetResourceString("ERR_InterpolatedStringFactoryError", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterpolationAlignmentOutOfRange As String
			Get
				Return VBResources.GetResourceString("ERR_InterpolationAlignmentOutOfRange", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InterpolationFormatWhitespace As String
			Get
				Return VBResources.GetResourceString("ERR_InterpolationFormatWhitespace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidAssemblyAttribute1 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidAssemblyAttribute1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidAssemblyCulture As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidAssemblyCulture", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidAssemblyCultureForExe As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidAssemblyCultureForExe", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidAssemblyName As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidAssemblyName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidAsyncIteratorModifiers As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidAsyncIteratorModifiers", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidAttributeUsage2 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidAttributeUsage2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidAttributeUsageOnAccessor As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidAttributeUsageOnAccessor", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidAttributeValue1 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidAttributeValue1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidAttributeValue2 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidAttributeValue2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidCoClass1 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidCoClass1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidConstructorCall As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidConstructorCall", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidDate As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidDate", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidDebugInfo As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidDebugInfo", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidDebugInformationFormat As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidDebugInformationFormat", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndAddHandler As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndAddHandler", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndEnum As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndEnum", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndEvent As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndEvent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndFunction As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndGet As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndGet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndInterface As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndInterface", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndOperator As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndOperator", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndProperty As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndProperty", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndRaiseEvent As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndRaiseEvent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndRemoveHandler As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndRemoveHandler", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndSet As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndSet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEndSub As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEndSub", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidEnumBase As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidEnumBase", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidFileAlignment As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidFileAlignment", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidFormatForGuidForOption As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidFormatForGuidForOption", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidFormatSpecifier As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidFormatSpecifier", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidHandles As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidHandles", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidHashAlgorithmName As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidHashAlgorithmName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidImplements As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidImplements", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidImplicitMeReference As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidImplicitMeReference", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidImplicitVar As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidImplicitVar", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidInNamespace As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidInNamespace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidInstrumentationKind As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidInstrumentationKind", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidLambdaModifier As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidLambdaModifier", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidLiteralExponent As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidLiteralExponent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidMe As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidMe", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidMeReference As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidMeReference", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidModuleAttribute1 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidModuleAttribute1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidMultipleAttributeUsage1 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidMultipleAttributeUsage1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidMultipleAttributeUsageInNetModule2 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidMultipleAttributeUsageInNetModule2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidNameOfSubExpression As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidNameOfSubExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidNewInType As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidNewInType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidNonSerializedUsage As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidNonSerializedUsage", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidOptionalParameterUsage1 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidOptionalParameterUsage1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidOptionCompare As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidOptionCompare", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidOptionExplicit As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidOptionExplicit", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidOptionInfer As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidOptionInfer", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidOptionStrict As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidOptionStrict", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidOptionStrictCustom As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidOptionStrictCustom", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidOutputName As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidOutputName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidOverrideDueToReturn2 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidOverrideDueToReturn2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidParameterSyntax As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidParameterSyntax", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidPathMap As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidPathMap", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidPreprocessorConstantType As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidPreprocessorConstantType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidSignaturePublicKey As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidSignaturePublicKey", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidSpecifierOnNonConversion1 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidSpecifierOnNonConversion1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidStructMemberNoPIA1 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidStructMemberNoPIA1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InValidSubMainsFound1 As String
			Get
				Return VBResources.GetResourceString("ERR_InValidSubMainsFound1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidSubsystemVersion As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidSubsystemVersion", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidSwitchValue As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidSwitchValue", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidTypeForAliasesImport2 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidTypeForAliasesImport2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidUseOfCustomModifier As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidUseOfCustomModifier", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidUseOfKeyword As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidUseOfKeyword", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidVersionFormat As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidVersionFormat", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvalidVersionFormat2 As String
			Get
				Return VBResources.GetResourceString("ERR_InvalidVersionFormat2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvInsideBlock As String
			Get
				Return VBResources.GetResourceString("ERR_InvInsideBlock", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvInsideEndsEnum As String
			Get
				Return VBResources.GetResourceString("ERR_InvInsideEndsEnum", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvInsideEndsEvent As String
			Get
				Return VBResources.GetResourceString("ERR_InvInsideEndsEvent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvInsideEndsInterface As String
			Get
				Return VBResources.GetResourceString("ERR_InvInsideEndsInterface", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvInsideEndsProc As String
			Get
				Return VBResources.GetResourceString("ERR_InvInsideEndsProc", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvInsideEndsProperty As String
			Get
				Return VBResources.GetResourceString("ERR_InvInsideEndsProperty", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvInsideEnum As String
			Get
				Return VBResources.GetResourceString("ERR_InvInsideEnum", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvInsideInterface As String
			Get
				Return VBResources.GetResourceString("ERR_InvInsideInterface", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvInsideProc As String
			Get
				Return VBResources.GetResourceString("ERR_InvInsideProc", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_InvOutsideProc As String
			Get
				Return VBResources.GetResourceString("ERR_InvOutsideProc", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IsNestedIn2 As String
			Get
				Return VBResources.GetResourceString("ERR_IsNestedIn2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IsNotOperatorGenericParam1 As String
			Get
				Return VBResources.GetResourceString("ERR_IsNotOperatorGenericParam1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IsNotOperatorNullable1 As String
			Get
				Return VBResources.GetResourceString("ERR_IsNotOperatorNullable1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IsNotOpRequiresReferenceTypes1 As String
			Get
				Return VBResources.GetResourceString("ERR_IsNotOpRequiresReferenceTypes1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IsOperatorGenericParam1 As String
			Get
				Return VBResources.GetResourceString("ERR_IsOperatorGenericParam1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IsOperatorNullable1 As String
			Get
				Return VBResources.GetResourceString("ERR_IsOperatorNullable1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IsOperatorRequiresReferenceTypes1 As String
			Get
				Return VBResources.GetResourceString("ERR_IsOperatorRequiresReferenceTypes1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IterationVariableShadowLocal1 As String
			Get
				Return VBResources.GetResourceString("ERR_IterationVariableShadowLocal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_IterationVariableShadowLocal2 As String
			Get
				Return VBResources.GetResourceString("ERR_IterationVariableShadowLocal2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_KeywordNotAllowedInScript As String
			Get
				Return VBResources.GetResourceString("ERR_KeywordNotAllowedInScript", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LabelNotDefined1 As String
			Get
				Return VBResources.GetResourceString("ERR_LabelNotDefined1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LambdaBindingMismatch1 As String
			Get
				Return VBResources.GetResourceString("ERR_LambdaBindingMismatch1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LambdaBindingMismatch2 As String
			Get
				Return VBResources.GetResourceString("ERR_LambdaBindingMismatch2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LambdaInSelectCaseExpr As String
			Get
				Return VBResources.GetResourceString("ERR_LambdaInSelectCaseExpr", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LambdaNotCreatableDelegate1 As String
			Get
				Return VBResources.GetResourceString("ERR_LambdaNotCreatableDelegate1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LambdaNotDelegate1 As String
			Get
				Return VBResources.GetResourceString("ERR_LambdaNotDelegate1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LambdaNoType As String
			Get
				Return VBResources.GetResourceString("ERR_LambdaNoType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LambdaNoTypeObjectDisallowed As String
			Get
				Return VBResources.GetResourceString("ERR_LambdaNoTypeObjectDisallowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LambdaParamShadowLocal1 As String
			Get
				Return VBResources.GetResourceString("ERR_LambdaParamShadowLocal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LambdasCannotHaveAttributes As String
			Get
				Return VBResources.GetResourceString("ERR_LambdasCannotHaveAttributes", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LambdaTooManyTypesObjectDisallowed As String
			Get
				Return VBResources.GetResourceString("ERR_LambdaTooManyTypesObjectDisallowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LanguageVersion As String
			Get
				Return VBResources.GetResourceString("ERR_LanguageVersion", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LateBoundOverloadInterfaceCall1 As String
			Get
				Return VBResources.GetResourceString("ERR_LateBoundOverloadInterfaceCall1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LbBadElseif As String
			Get
				Return VBResources.GetResourceString("ERR_LbBadElseif", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LbElseifAfterElse As String
			Get
				Return VBResources.GetResourceString("ERR_LbElseifAfterElse", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LbElseNoMatchingIf As String
			Get
				Return VBResources.GetResourceString("ERR_LbElseNoMatchingIf", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LbExpectedEndIf As String
			Get
				Return VBResources.GetResourceString("ERR_LbExpectedEndIf", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LbNoMatchingIf As String
			Get
				Return VBResources.GetResourceString("ERR_LbNoMatchingIf", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LibAnycpu32bitPreferredConflict As String
			Get
				Return VBResources.GetResourceString("ERR_LibAnycpu32bitPreferredConflict", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LibNotFound As String
			Get
				Return VBResources.GetResourceString("ERR_LibNotFound", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LineContWithCommentOrNoPrecSpace As String
			Get
				Return VBResources.GetResourceString("ERR_LineContWithCommentOrNoPrecSpace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LinkedNetmoduleMetadataMustProvideFullPEImage As String
			Get
				Return VBResources.GetResourceString("ERR_LinkedNetmoduleMetadataMustProvideFullPEImage", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LiteralExpected As String
			Get
				Return VBResources.GetResourceString("ERR_LiteralExpected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LocalNamedSameAsParam1 As String
			Get
				Return VBResources.GetResourceString("ERR_LocalNamedSameAsParam1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LocalNamedSameAsParamInLambda1 As String
			Get
				Return VBResources.GetResourceString("ERR_LocalNamedSameAsParamInLambda1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LocalSameAsFunc As String
			Get
				Return VBResources.GetResourceString("ERR_LocalSameAsFunc", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LocalsCannotHaveAttributes As String
			Get
				Return VBResources.GetResourceString("ERR_LocalsCannotHaveAttributes", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LocalTypeNameClash2 As String
			Get
				Return VBResources.GetResourceString("ERR_LocalTypeNameClash2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LoopControlMustNotAwait As String
			Get
				Return VBResources.GetResourceString("ERR_LoopControlMustNotAwait", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LoopControlMustNotBeProperty As String
			Get
				Return VBResources.GetResourceString("ERR_LoopControlMustNotBeProperty", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LoopDoubleCondition As String
			Get
				Return VBResources.GetResourceString("ERR_LoopDoubleCondition", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LoopNoMatchingDo As String
			Get
				Return VBResources.GetResourceString("ERR_LoopNoMatchingDo", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_LValueRequired As String
			Get
				Return VBResources.GetResourceString("ERR_LValueRequired", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MarshalUnmanagedTypeNotValidForFields As String
			Get
				Return VBResources.GetResourceString("ERR_MarshalUnmanagedTypeNotValidForFields", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MarshalUnmanagedTypeOnlyValidForFields As String
			Get
				Return VBResources.GetResourceString("ERR_MarshalUnmanagedTypeOnlyValidForFields", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MatchingOperatorExpected2 As String
			Get
				Return VBResources.GetResourceString("ERR_MatchingOperatorExpected2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MaximumNumberOfErrors As String
			Get
				Return VBResources.GetResourceString("ERR_MaximumNumberOfErrors", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MemberClashesWithSynth6 As String
			Get
				Return VBResources.GetResourceString("ERR_MemberClashesWithSynth6", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MemberConflictWithSynth4 As String
			Get
				Return VBResources.GetResourceString("ERR_MemberConflictWithSynth4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_Merge_conflict_marker_encountered As String
			Get
				Return VBResources.GetResourceString("ERR_Merge_conflict_marker_encountered", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MetaDataIsNotAssembly As String
			Get
				Return VBResources.GetResourceString("ERR_MetaDataIsNotAssembly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MetaDataIsNotModule As String
			Get
				Return VBResources.GetResourceString("ERR_MetaDataIsNotModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MetadataMembersAmbiguous3 As String
			Get
				Return VBResources.GetResourceString("ERR_MetadataMembersAmbiguous3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MetadataReferencesNotSupported As String
			Get
				Return VBResources.GetResourceString("ERR_MetadataReferencesNotSupported", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MethodAlreadyImplemented2 As String
			Get
				Return VBResources.GetResourceString("ERR_MethodAlreadyImplemented2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MethodBodyNotAtLineStart As String
			Get
				Return VBResources.GetResourceString("ERR_MethodBodyNotAtLineStart", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MethodMustBeFirstStatementOnLine As String
			Get
				Return VBResources.GetResourceString("ERR_MethodMustBeFirstStatementOnLine", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MethodTypeArgsUnexpected As String
			Get
				Return VBResources.GetResourceString("ERR_MethodTypeArgsUnexpected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MismatchedXmlEndTag As String
			Get
				Return VBResources.GetResourceString("ERR_MismatchedXmlEndTag", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingAddHandlerDef1 As String
			Get
				Return VBResources.GetResourceString("ERR_MissingAddHandlerDef1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingEndAddHandler As String
			Get
				Return VBResources.GetResourceString("ERR_MissingEndAddHandler", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingEndBrack As String
			Get
				Return VBResources.GetResourceString("ERR_MissingEndBrack", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingEndEnum As String
			Get
				Return VBResources.GetResourceString("ERR_MissingEndEnum", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingEndEvent As String
			Get
				Return VBResources.GetResourceString("ERR_MissingEndEvent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingEndGet As String
			Get
				Return VBResources.GetResourceString("ERR_MissingEndGet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingEndInterface As String
			Get
				Return VBResources.GetResourceString("ERR_MissingEndInterface", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingEndRaiseEvent As String
			Get
				Return VBResources.GetResourceString("ERR_MissingEndRaiseEvent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingEndRemoveHandler As String
			Get
				Return VBResources.GetResourceString("ERR_MissingEndRemoveHandler", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingEndSet As String
			Get
				Return VBResources.GetResourceString("ERR_MissingEndSet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingGuidForOption As String
			Get
				Return VBResources.GetResourceString("ERR_MissingGuidForOption", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingIsInTypeOf As String
			Get
				Return VBResources.GetResourceString("ERR_MissingIsInTypeOf", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingLibInDeclare As String
			Get
				Return VBResources.GetResourceString("ERR_MissingLibInDeclare", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingNetModuleReference As String
			Get
				Return VBResources.GetResourceString("ERR_MissingNetModuleReference", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingNext As String
			Get
				Return VBResources.GetResourceString("ERR_MissingNext", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingRaiseEventDef1 As String
			Get
				Return VBResources.GetResourceString("ERR_MissingRaiseEventDef1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingRemoveHandlerDef1 As String
			Get
				Return VBResources.GetResourceString("ERR_MissingRemoveHandlerDef1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingRuntimeHelper As String
			Get
				Return VBResources.GetResourceString("ERR_MissingRuntimeHelper", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingSubscript As String
			Get
				Return VBResources.GetResourceString("ERR_MissingSubscript", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingValuesForArraysInApplAttrs As String
			Get
				Return VBResources.GetResourceString("ERR_MissingValuesForArraysInApplAttrs", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingVersionInXmlDecl As String
			Get
				Return VBResources.GetResourceString("ERR_MissingVersionInXmlDecl", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MissingXmlEndTag As String
			Get
				Return VBResources.GetResourceString("ERR_MissingXmlEndTag", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MixingWinRTAndNETEvents As String
			Get
				Return VBResources.GetResourceString("ERR_MixingWinRTAndNETEvents", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleAsType1 As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleAsType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleCantImplement As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleCantImplement", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleCantInherit As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleCantInherit", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleCantUseDLLDeclareSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleCantUseDLLDeclareSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleCantUseEventSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleCantUseEventSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleCantUseMethodSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleCantUseMethodSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleCantUseTypeSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleCantUseTypeSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleCantUseVariableSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleCantUseVariableSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleEmitFailure As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleEmitFailure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleMemberCantImplement As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleMemberCantImplement", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModuleNotAtNamespace As String
			Get
				Return VBResources.GetResourceString("ERR_ModuleNotAtNamespace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ModulesCannotBeGeneric As String
			Get
				Return VBResources.GetResourceString("ERR_ModulesCannotBeGeneric", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MoreThanOneValidMainWasFound2 As String
			Get
				Return VBResources.GetResourceString("ERR_MoreThanOneValidMainWasFound2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultilineLambdaMissingFunction As String
			Get
				Return VBResources.GetResourceString("ERR_MultilineLambdaMissingFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultilineLambdaMissingSub As String
			Get
				Return VBResources.GetResourceString("ERR_MultilineLambdaMissingSub", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultilineLambdasCannotContainOnError As String
			Get
				Return VBResources.GetResourceString("ERR_MultilineLambdasCannotContainOnError", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultipleAnalyzerConfigsInSameDir As String
			Get
				Return VBResources.GetResourceString("ERR_MultipleAnalyzerConfigsInSameDir", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultipleClassConstraints1 As String
			Get
				Return VBResources.GetResourceString("ERR_MultipleClassConstraints1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultipleEventImplMismatch3 As String
			Get
				Return VBResources.GetResourceString("ERR_MultipleEventImplMismatch3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultipleExtends As String
			Get
				Return VBResources.GetResourceString("ERR_MultipleExtends", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultipleNewConstraints As String
			Get
				Return VBResources.GetResourceString("ERR_MultipleNewConstraints", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultipleOptionalParameterSpecifiers As String
			Get
				Return VBResources.GetResourceString("ERR_MultipleOptionalParameterSpecifiers", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultipleParameterSpecifiers As String
			Get
				Return VBResources.GetResourceString("ERR_MultipleParameterSpecifiers", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultipleReferenceConstraints As String
			Get
				Return VBResources.GetResourceString("ERR_MultipleReferenceConstraints", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultipleValueConstraints As String
			Get
				Return VBResources.GetResourceString("ERR_MultipleValueConstraints", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultiplyDefined1 As String
			Get
				Return VBResources.GetResourceString("ERR_MultiplyDefined1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultiplyDefinedEnumMember2 As String
			Get
				Return VBResources.GetResourceString("ERR_MultiplyDefinedEnumMember2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MultiplyDefinedType3 As String
			Get
				Return VBResources.GetResourceString("ERR_MultiplyDefinedType3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MustBeInCatchToRethrow As String
			Get
				Return VBResources.GetResourceString("ERR_MustBeInCatchToRethrow", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MustBeOverloads2 As String
			Get
				Return VBResources.GetResourceString("ERR_MustBeOverloads2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MustInheritEventNotOverridden As String
			Get
				Return VBResources.GetResourceString("ERR_MustInheritEventNotOverridden", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MustInheritForNewConstraint2 As String
			Get
				Return VBResources.GetResourceString("ERR_MustInheritForNewConstraint2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MustOverOnNotInheritPartClsMem1 As String
			Get
				Return VBResources.GetResourceString("ERR_MustOverOnNotInheritPartClsMem1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MustOverridesInClass1 As String
			Get
				Return VBResources.GetResourceString("ERR_MustOverridesInClass1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MustShadow2 As String
			Get
				Return VBResources.GetResourceString("ERR_MustShadow2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MutuallyExclusiveOptions As String
			Get
				Return VBResources.GetResourceString("ERR_MutuallyExclusiveOptions", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MyBaseAbstractCall1 As String
			Get
				Return VBResources.GetResourceString("ERR_MyBaseAbstractCall1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MyClassAbstractCall1 As String
			Get
				Return VBResources.GetResourceString("ERR_MyClassAbstractCall1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MyClassNotInClass As String
			Get
				Return VBResources.GetResourceString("ERR_MyClassNotInClass", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_MyGroupCollectionAttributeCycle As String
			Get
				Return VBResources.GetResourceString("ERR_MyGroupCollectionAttributeCycle", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedArgAlsoOmitted1 As String
			Get
				Return VBResources.GetResourceString("ERR_NamedArgAlsoOmitted1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedArgAlsoOmitted2 As String
			Get
				Return VBResources.GetResourceString("ERR_NamedArgAlsoOmitted2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedArgAlsoOmitted3 As String
			Get
				Return VBResources.GetResourceString("ERR_NamedArgAlsoOmitted3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedArgumentSpecificationBeforeFixedArgumentInLateboundInvocation As String
			Get
				Return VBResources.GetResourceString("ERR_NamedArgumentSpecificationBeforeFixedArgumentInLateboundInvocation", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedArgUsedTwice1 As String
			Get
				Return VBResources.GetResourceString("ERR_NamedArgUsedTwice1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedArgUsedTwice2 As String
			Get
				Return VBResources.GetResourceString("ERR_NamedArgUsedTwice2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedArgUsedTwice3 As String
			Get
				Return VBResources.GetResourceString("ERR_NamedArgUsedTwice3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedParamArrayArgument As String
			Get
				Return VBResources.GetResourceString("ERR_NamedParamArrayArgument", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedParamNotFound1 As String
			Get
				Return VBResources.GetResourceString("ERR_NamedParamNotFound1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedParamNotFound2 As String
			Get
				Return VBResources.GetResourceString("ERR_NamedParamNotFound2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedParamNotFound3 As String
			Get
				Return VBResources.GetResourceString("ERR_NamedParamNotFound3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamedSubscript As String
			Get
				Return VBResources.GetResourceString("ERR_NamedSubscript", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NameNotDeclared1 As String
			Get
				Return VBResources.GetResourceString("ERR_NameNotDeclared1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NameNotEvent2 As String
			Get
				Return VBResources.GetResourceString("ERR_NameNotEvent2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NameNotMember2 As String
			Get
				Return VBResources.GetResourceString("ERR_NameNotMember2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NameNotMemberOfAnonymousType2 As String
			Get
				Return VBResources.GetResourceString("ERR_NameNotMemberOfAnonymousType2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NameSameAsMethodTypeParam1 As String
			Get
				Return VBResources.GetResourceString("ERR_NameSameAsMethodTypeParam1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamespaceNotAllowedInScript As String
			Get
				Return VBResources.GetResourceString("ERR_NamespaceNotAllowedInScript", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamespaceNotAtNamespace As String
			Get
				Return VBResources.GetResourceString("ERR_NamespaceNotAtNamespace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NamespaceNotExpression1 As String
			Get
				Return VBResources.GetResourceString("ERR_NamespaceNotExpression1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NarrowingConversionCollection2 As String
			Get
				Return VBResources.GetResourceString("ERR_NarrowingConversionCollection2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NarrowingConversionDisallowed2 As String
			Get
				Return VBResources.GetResourceString("ERR_NarrowingConversionDisallowed2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NeedModule As String
			Get
				Return VBResources.GetResourceString("ERR_NeedModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NegativeArraySize As String
			Get
				Return VBResources.GetResourceString("ERR_NegativeArraySize", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NestedBase2 As String
			Get
				Return VBResources.GetResourceString("ERR_NestedBase2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NestedExternalSource As String
			Get
				Return VBResources.GetResourceString("ERR_NestedExternalSource", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NestedFunctionArgumentNarrowing3 As String
			Get
				Return VBResources.GetResourceString("ERR_NestedFunctionArgumentNarrowing3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NestedGlobalNamespace As String
			Get
				Return VBResources.GetResourceString("ERR_NestedGlobalNamespace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NestedInteropType As String
			Get
				Return VBResources.GetResourceString("ERR_NestedInteropType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NestedTypeInInheritsClause2 As String
			Get
				Return VBResources.GetResourceString("ERR_NestedTypeInInheritsClause2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NestingViolatesCLS1 As String
			Get
				Return VBResources.GetResourceString("ERR_NestingViolatesCLS1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NetModuleNameMismatch As String
			Get
				Return VBResources.GetResourceString("ERR_NetModuleNameMismatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NetModuleNameMustBeUnique As String
			Get
				Return VBResources.GetResourceString("ERR_NetModuleNameMustBeUnique", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NewAndValueConstraintsCombined As String
			Get
				Return VBResources.GetResourceString("ERR_NewAndValueConstraintsCombined", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NewArgsDisallowedForTypeParam As String
			Get
				Return VBResources.GetResourceString("ERR_NewArgsDisallowedForTypeParam", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NewCannotHandleEvents As String
			Get
				Return VBResources.GetResourceString("ERR_NewCannotHandleEvents", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NewIfNullOnGenericParam As String
			Get
				Return VBResources.GetResourceString("ERR_NewIfNullOnGenericParam", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NewIfNullOnNonClass As String
			Get
				Return VBResources.GetResourceString("ERR_NewIfNullOnNonClass", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NewInInterface As String
			Get
				Return VBResources.GetResourceString("ERR_NewInInterface", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NewInStruct As String
			Get
				Return VBResources.GetResourceString("ERR_NewInStruct", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NewOnAbstractClass As String
			Get
				Return VBResources.GetResourceString("ERR_NewOnAbstractClass", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NewWithTupleTypeSyntax As String
			Get
				Return VBResources.GetResourceString("ERR_NewWithTupleTypeSyntax", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NextForMismatch1 As String
			Get
				Return VBResources.GetResourceString("ERR_NextForMismatch1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NextNoMatchingFor As String
			Get
				Return VBResources.GetResourceString("ERR_NextNoMatchingFor", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoAccessibleConstructorOnBase As String
			Get
				Return VBResources.GetResourceString("ERR_NoAccessibleConstructorOnBase", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoAccessibleGet As String
			Get
				Return VBResources.GetResourceString("ERR_NoAccessibleGet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoAccessibleSet As String
			Get
				Return VBResources.GetResourceString("ERR_NoAccessibleSet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoAddMethod1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoAddMethod1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoArgumentCountOverloadCandidates1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoArgumentCountOverloadCandidates1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoCallableOverloadCandidates2 As String
			Get
				Return VBResources.GetResourceString("ERR_NoCallableOverloadCandidates2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoConstituentArraySizes As String
			Get
				Return VBResources.GetResourceString("ERR_NoConstituentArraySizes", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoConstructorOnBase2 As String
			Get
				Return VBResources.GetResourceString("ERR_NoConstructorOnBase2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoDefaultNotExtend1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoDefaultNotExtend1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoDirectDelegateConstruction1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoDirectDelegateConstruction1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoExplicitArraySizes As String
			Get
				Return VBResources.GetResourceString("ERR_NoExplicitArraySizes", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoGetProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoGetProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoGlobalExpectedIdentifier As String
			Get
				Return VBResources.GetResourceString("ERR_NoGlobalExpectedIdentifier", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoGlobalInHandles As String
			Get
				Return VBResources.GetResourceString("ERR_NoGlobalInHandles", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoMostSpecificOverload2 As String
			Get
				Return VBResources.GetResourceString("ERR_NoMostSpecificOverload2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoNetModuleOutputWhenRefOutOrRefOnly As String
			Get
				Return VBResources.GetResourceString("ERR_NoNetModuleOutputWhenRefOutOrRefOnly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NonFieldPropertyAggrMemberInit1 As String
			Get
				Return VBResources.GetResourceString("ERR_NonFieldPropertyAggrMemberInit1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NonNamespaceOrClassOnImport2 As String
			Get
				Return VBResources.GetResourceString("ERR_NonNamespaceOrClassOnImport2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoNonIndexProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoNonIndexProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoNonNarrowingOverloadCandidates2 As String
			Get
				Return VBResources.GetResourceString("ERR_NoNonNarrowingOverloadCandidates2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoNonObsoleteConstructorOnBase3 As String
			Get
				Return VBResources.GetResourceString("ERR_NoNonObsoleteConstructorOnBase3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoNonObsoleteConstructorOnBase4 As String
			Get
				Return VBResources.GetResourceString("ERR_NoNonObsoleteConstructorOnBase4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoPartialMethodInAddressOf1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoPartialMethodInAddressOf1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoPIAAttributeMissing2 As String
			Get
				Return VBResources.GetResourceString("ERR_NoPIAAttributeMissing2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoRefOutWhenRefOnly As String
			Get
				Return VBResources.GetResourceString("ERR_NoRefOutWhenRefOnly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoResponseFile As String
			Get
				Return VBResources.GetResourceString("ERR_NoResponseFile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoSetProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoSetProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoSources As String
			Get
				Return VBResources.GetResourceString("ERR_NoSources", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoSourcesOut As String
			Get
				Return VBResources.GetResourceString("ERR_NoSourcesOut", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoSuitableNewForNewConstraint2 As String
			Get
				Return VBResources.GetResourceString("ERR_NoSuitableNewForNewConstraint2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoSuitableWidestType1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoSuitableWidestType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NotACollection1 As String
			Get
				Return VBResources.GetResourceString("ERR_NotACollection1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NotMostSpecificOverload As String
			Get
				Return VBResources.GetResourceString("ERR_NotMostSpecificOverload", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NotOverridableRequiresOverrides As String
			Get
				Return VBResources.GetResourceString("ERR_NotOverridableRequiresOverrides", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoTypeArgumentCountOverloadCand1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoTypeArgumentCountOverloadCand1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoTypecharInAlias As String
			Get
				Return VBResources.GetResourceString("ERR_NoTypecharInAlias", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoTypecharInLabel As String
			Get
				Return VBResources.GetResourceString("ERR_NoTypecharInLabel", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoUniqueConstructorOnBase2 As String
			Get
				Return VBResources.GetResourceString("ERR_NoUniqueConstructorOnBase2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoViableOverloadCandidates1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoViableOverloadCandidates1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoWithEventsVarOnHandlesList As String
			Get
				Return VBResources.GetResourceString("ERR_NoWithEventsVarOnHandlesList", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoXmlAxesLateBinding As String
			Get
				Return VBResources.GetResourceString("ERR_NoXmlAxesLateBinding", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NoZeroCountArgumentInitCandidates1 As String
			Get
				Return VBResources.GetResourceString("ERR_NoZeroCountArgumentInitCandidates1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NullableCharNotSupported As String
			Get
				Return VBResources.GetResourceString("ERR_NullableCharNotSupported", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NullableDisallowedForStructConstr1 As String
			Get
				Return VBResources.GetResourceString("ERR_NullableDisallowedForStructConstr1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NullableImplicit As String
			Get
				Return VBResources.GetResourceString("ERR_NullableImplicit", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NullableParameterMustSpecifyType As String
			Get
				Return VBResources.GetResourceString("ERR_NullableParameterMustSpecifyType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NullableTypeInferenceNotSupported As String
			Get
				Return VBResources.GetResourceString("ERR_NullableTypeInferenceNotSupported", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_NullPropagatingOpInExpressionTree As String
			Get
				Return VBResources.GetResourceString("ERR_NullPropagatingOpInExpressionTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObjectInitializerRequiresFieldName As String
			Get
				Return VBResources.GetResourceString("ERR_ObjectInitializerRequiresFieldName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObjectReferenceNotSupplied As String
			Get
				Return VBResources.GetResourceString("ERR_ObjectReferenceNotSupplied", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteArgumentsNeedParens As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteArgumentsNeedParens", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteAsAny As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteAsAny", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteEndIf As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteEndIf", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteExponent As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteExponent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteGetStatement As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteGetStatement", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteGosub As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteGosub", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteInvalidOnEventMember As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteInvalidOnEventMember", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteLetSetNotNeeded As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteLetSetNotNeeded", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteLineNumbersAreLabels As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteLineNumbersAreLabels", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteObjectNotVariant As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteObjectNotVariant", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteOnGotoGosub As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteOnGotoGosub", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteOptionalWithoutValue As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteOptionalWithoutValue", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoletePropertyGetLetSet As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoletePropertyGetLetSet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteRedimAs As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteRedimAs", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteStructureNotType As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteStructureNotType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ObsoleteWhileWend As String
			Get
				Return VBResources.GetResourceString("ERR_ObsoleteWhileWend", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OfExpected As String
			Get
				Return VBResources.GetResourceString("ERR_OfExpected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OmittedArgument1 As String
			Get
				Return VBResources.GetResourceString("ERR_OmittedArgument1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OmittedArgument2 As String
			Get
				Return VBResources.GetResourceString("ERR_OmittedArgument2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OmittedArgument3 As String
			Get
				Return VBResources.GetResourceString("ERR_OmittedArgument3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OmittedParamArrayArgument As String
			Get
				Return VBResources.GetResourceString("ERR_OmittedParamArrayArgument", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OneOrTwoParametersRequired1 As String
			Get
				Return VBResources.GetResourceString("ERR_OneOrTwoParametersRequired1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OneParameterRequired1 As String
			Get
				Return VBResources.GetResourceString("ERR_OneParameterRequired1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OnErrorInSyncLock As String
			Get
				Return VBResources.GetResourceString("ERR_OnErrorInSyncLock", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OnErrorInUsing As String
			Get
				Return VBResources.GetResourceString("ERR_OnErrorInUsing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OnlyNullLowerBound As String
			Get
				Return VBResources.GetResourceString("ERR_OnlyNullLowerBound", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OnlyOneAccessorForGetSet As String
			Get
				Return VBResources.GetResourceString("ERR_OnlyOneAccessorForGetSet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OnlyOneImplementingMethodAllowed3 As String
			Get
				Return VBResources.GetResourceString("ERR_OnlyOneImplementingMethodAllowed3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OnlyOnePartialMethodAllowed2 As String
			Get
				Return VBResources.GetResourceString("ERR_OnlyOnePartialMethodAllowed2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OnlyPrivatePartialMethods1 As String
			Get
				Return VBResources.GetResourceString("ERR_OnlyPrivatePartialMethods1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OpenTypeDisallowed As String
			Get
				Return VBResources.GetResourceString("ERR_OpenTypeDisallowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OperatorDeclaredInModule As String
			Get
				Return VBResources.GetResourceString("ERR_OperatorDeclaredInModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OperatorMustBePublic As String
			Get
				Return VBResources.GetResourceString("ERR_OperatorMustBePublic", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OperatorMustBeShared As String
			Get
				Return VBResources.GetResourceString("ERR_OperatorMustBeShared", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OperatorNotOverloadable As String
			Get
				Return VBResources.GetResourceString("ERR_OperatorNotOverloadable", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OperatorRequiresBoolReturnType1 As String
			Get
				Return VBResources.GetResourceString("ERR_OperatorRequiresBoolReturnType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OperatorRequiresIntegerParameter1 As String
			Get
				Return VBResources.GetResourceString("ERR_OperatorRequiresIntegerParameter1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OptionalIllegal1 As String
			Get
				Return VBResources.GetResourceString("ERR_OptionalIllegal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OptionalsCantBeStructGenericParams As String
			Get
				Return VBResources.GetResourceString("ERR_OptionalsCantBeStructGenericParams", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OptionMustBeAbsolutePath As String
			Get
				Return VBResources.GetResourceString("ERR_OptionMustBeAbsolutePath", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OptionStmtWrongOrder As String
			Get
				Return VBResources.GetResourceString("ERR_OptionStmtWrongOrder", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_Overflow As String
			Get
				Return VBResources.GetResourceString("ERR_Overflow", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverloadCandidate1 As String
			Get
				Return VBResources.GetResourceString("ERR_OverloadCandidate1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverloadCandidate2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverloadCandidate2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverloadingPropertyKind2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverloadingPropertyKind2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverloadsModifierInModule As String
			Get
				Return VBResources.GetResourceString("ERR_OverloadsModifierInModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverloadWithArrayVsParamArray2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverloadWithArrayVsParamArray2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverloadWithByref2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverloadWithByref2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverloadWithDefault2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverloadWithDefault2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverloadWithOptional2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverloadWithOptional2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverloadWithReturnType2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverloadWithReturnType2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverriddenCandidate1 As String
			Get
				Return VBResources.GetResourceString("ERR_OverriddenCandidate1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverrideNotNeeded3 As String
			Get
				Return VBResources.GetResourceString("ERR_OverrideNotNeeded3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverridesImpliesOverridable As String
			Get
				Return VBResources.GetResourceString("ERR_OverridesImpliesOverridable", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverrideWithArrayVsParamArray2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverrideWithArrayVsParamArray2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverrideWithByref2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverrideWithByref2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverrideWithConstraintMismatch2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverrideWithConstraintMismatch2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverrideWithDefault2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverrideWithDefault2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverrideWithOptional2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverrideWithOptional2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverrideWithOptionalTypes2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverrideWithOptionalTypes2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverridingInitOnlyProperty As String
			Get
				Return VBResources.GetResourceString("ERR_OverridingInitOnlyProperty", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_OverridingPropertyKind2 As String
			Get
				Return VBResources.GetResourceString("ERR_OverridingPropertyKind2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamArrayArgumentMismatch As String
			Get
				Return VBResources.GetResourceString("ERR_ParamArrayArgumentMismatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamArrayIllegal1 As String
			Get
				Return VBResources.GetResourceString("ERR_ParamArrayIllegal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamArrayMustBeByVal As String
			Get
				Return VBResources.GetResourceString("ERR_ParamArrayMustBeByVal", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamArrayMustBeLast As String
			Get
				Return VBResources.GetResourceString("ERR_ParamArrayMustBeLast", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamArrayNotArray As String
			Get
				Return VBResources.GetResourceString("ERR_ParamArrayNotArray", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamArrayRank As String
			Get
				Return VBResources.GetResourceString("ERR_ParamArrayRank", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamArrayWithOptArgs As String
			Get
				Return VBResources.GetResourceString("ERR_ParamArrayWithOptArgs", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamArrayWrongType As String
			Get
				Return VBResources.GetResourceString("ERR_ParamArrayWrongType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamDefaultValueDiffersFromAttribute As String
			Get
				Return VBResources.GetResourceString("ERR_ParamDefaultValueDiffersFromAttribute", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParameterizedPropertyInAggrInit1 As String
			Get
				Return VBResources.GetResourceString("ERR_ParameterizedPropertyInAggrInit1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParameterNotValidForType As String
			Get
				Return VBResources.GetResourceString("ERR_ParameterNotValidForType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamNameFunctionNameCollision As String
			Get
				Return VBResources.GetResourceString("ERR_ParamNameFunctionNameCollision", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ParamTypingInconsistency As String
			Get
				Return VBResources.GetResourceString("ERR_ParamTypingInconsistency", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialDeclarationImplements1 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialDeclarationImplements1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialMethodDefaultParameterValueMismatch2 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialMethodDefaultParameterValueMismatch2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialMethodGenericConstraints2 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialMethodGenericConstraints2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialMethodMustBeEmpty As String
			Get
				Return VBResources.GetResourceString("ERR_PartialMethodMustBeEmpty", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialMethodParamArrayMismatch2 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialMethodParamArrayMismatch2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialMethodParamNamesMustMatch3 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialMethodParamNamesMustMatch3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialMethodsMustBePrivate As String
			Get
				Return VBResources.GetResourceString("ERR_PartialMethodsMustBePrivate", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialMethodsMustBeSub1 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialMethodsMustBeSub1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialMethodsMustNotBeAsync1 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialMethodsMustNotBeAsync1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialMethodTypeParamNameMismatch3 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialMethodTypeParamNameMismatch3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialTypeAccessMismatch3 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialTypeAccessMismatch3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialTypeBadMustInherit1 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialTypeBadMustInherit1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialTypeConstraintMismatch1 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialTypeConstraintMismatch1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PartialTypeTypeParamNameMismatch3 As String
			Get
				Return VBResources.GetResourceString("ERR_PartialTypeTypeParamNameMismatch3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PDBWritingFailed As String
			Get
				Return VBResources.GetResourceString("ERR_PDBWritingFailed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PermissionSetAttributeFileReadError As String
			Get
				Return VBResources.GetResourceString("ERR_PermissionSetAttributeFileReadError", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PermissionSetAttributeInvalidFile As String
			Get
				Return VBResources.GetResourceString("ERR_PermissionSetAttributeInvalidFile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PeWritingFailure As String
			Get
				Return VBResources.GetResourceString("ERR_PeWritingFailure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PIAHasNoAssemblyGuid1 As String
			Get
				Return VBResources.GetResourceString("ERR_PIAHasNoAssemblyGuid1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PIAHasNoTypeLibAttribute1 As String
			Get
				Return VBResources.GetResourceString("ERR_PIAHasNoTypeLibAttribute1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PlatformDoesntSupport As String
			Get
				Return VBResources.GetResourceString("ERR_PlatformDoesntSupport", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PredefinedValueTupleTypeMustBeStruct As String
			Get
				Return VBResources.GetResourceString("ERR_PredefinedValueTupleTypeMustBeStruct", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PrincipalPermissionInvalidAction As String
			Get
				Return VBResources.GetResourceString("ERR_PrincipalPermissionInvalidAction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PrivateTypeOutsideType As String
			Get
				Return VBResources.GetResourceString("ERR_PrivateTypeOutsideType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PropertyAccessIgnored As String
			Get
				Return VBResources.GetResourceString("ERR_PropertyAccessIgnored", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PropertyDoesntImplementAllAccessors As String
			Get
				Return VBResources.GetResourceString("ERR_PropertyDoesntImplementAllAccessors", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PropertyDoesntImplementInitOnly As String
			Get
				Return VBResources.GetResourceString("ERR_PropertyDoesntImplementInitOnly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PropertyNameConflictInMyCollection As String
			Get
				Return VBResources.GetResourceString("ERR_PropertyNameConflictInMyCollection", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PropertyOrFieldNotDefined1 As String
			Get
				Return VBResources.GetResourceString("ERR_PropertyOrFieldNotDefined1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PropertySetParamCollisionWithValue As String
			Get
				Return VBResources.GetResourceString("ERR_PropertySetParamCollisionWithValue", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PropMustHaveGetSet As String
			Get
				Return VBResources.GetResourceString("ERR_PropMustHaveGetSet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ProtectedTypeOutsideClass As String
			Get
				Return VBResources.GetResourceString("ERR_ProtectedTypeOutsideClass", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PublicKeyContainerFailure As String
			Get
				Return VBResources.GetResourceString("ERR_PublicKeyContainerFailure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PublicKeyFileFailure As String
			Get
				Return VBResources.GetResourceString("ERR_PublicKeyFileFailure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PublicSignNetModule As String
			Get
				Return VBResources.GetResourceString("ERR_PublicSignNetModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_PublicSignNoKey As String
			Get
				Return VBResources.GetResourceString("ERR_PublicSignNoKey", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QualifiedNameNotAllowed As String
			Get
				Return VBResources.GetResourceString("ERR_QualifiedNameNotAllowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QualNotObjectRecord1 As String
			Get
				Return VBResources.GetResourceString("ERR_QualNotObjectRecord1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QueryAnonTypeFieldXMLNameInference As String
			Get
				Return VBResources.GetResourceString("ERR_QueryAnonTypeFieldXMLNameInference", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QueryAnonymousTypeDisallowsTypeChar As String
			Get
				Return VBResources.GetResourceString("ERR_QueryAnonymousTypeDisallowsTypeChar", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QueryAnonymousTypeFieldNameInference As String
			Get
				Return VBResources.GetResourceString("ERR_QueryAnonymousTypeFieldNameInference", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QueryDuplicateAnonTypeMemberName1 As String
			Get
				Return VBResources.GetResourceString("ERR_QueryDuplicateAnonTypeMemberName1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QueryInvalidControlVariableName1 As String
			Get
				Return VBResources.GetResourceString("ERR_QueryInvalidControlVariableName1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QueryNameNotDeclared As String
			Get
				Return VBResources.GetResourceString("ERR_QueryNameNotDeclared", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QueryOperatorNotFound As String
			Get
				Return VBResources.GetResourceString("ERR_QueryOperatorNotFound", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QueryStrictDisallowImplicitObject As String
			Get
				Return VBResources.GetResourceString("ERR_QueryStrictDisallowImplicitObject", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_QuotedEmbeddedExpression As String
			Get
				Return VBResources.GetResourceString("ERR_QuotedEmbeddedExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RaiseEventShapeMismatch1 As String
			Get
				Return VBResources.GetResourceString("ERR_RaiseEventShapeMismatch1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReAbstractionInNoPIAType As String
			Get
				Return VBResources.GetResourceString("ERR_ReAbstractionInNoPIAType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReadOnlyAssignment As String
			Get
				Return VBResources.GetResourceString("ERR_ReadOnlyAssignment", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReadOnlyHasNoGet As String
			Get
				Return VBResources.GetResourceString("ERR_ReadOnlyHasNoGet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReadOnlyHasSet As String
			Get
				Return VBResources.GetResourceString("ERR_ReadOnlyHasSet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReadOnlyInClosure As String
			Get
				Return VBResources.GetResourceString("ERR_ReadOnlyInClosure", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReadOnlyNoAccessorFlag As String
			Get
				Return VBResources.GetResourceString("ERR_ReadOnlyNoAccessorFlag", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReadOnlyProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_ReadOnlyProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RecordCycle2 As String
			Get
				Return VBResources.GetResourceString("ERR_RecordCycle2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RecordEmbeds2 As String
			Get
				Return VBResources.GetResourceString("ERR_RecordEmbeds2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RedimNoSizes As String
			Get
				Return VBResources.GetResourceString("ERR_RedimNoSizes", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RedimRankMismatch As String
			Get
				Return VBResources.GetResourceString("ERR_RedimRankMismatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RefAndClassTypeConstrCombined As String
			Get
				Return VBResources.GetResourceString("ERR_RefAndClassTypeConstrCombined", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RefAndValueConstraintsCombined As String
			Get
				Return VBResources.GetResourceString("ERR_RefAndValueConstraintsCombined", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReferenceComparison3 As String
			Get
				Return VBResources.GetResourceString("ERR_ReferenceComparison3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReferenceDirectiveOnlyAllowedInScripts As String
			Get
				Return VBResources.GetResourceString("ERR_ReferenceDirectiveOnlyAllowedInScripts", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RefReturningCallInExpressionTree As String
			Get
				Return VBResources.GetResourceString("ERR_RefReturningCallInExpressionTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReImplementingWinRTInterface4 As String
			Get
				Return VBResources.GetResourceString("ERR_ReImplementingWinRTInterface4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReImplementingWinRTInterface5 As String
			Get
				Return VBResources.GetResourceString("ERR_ReImplementingWinRTInterface5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RemoveParamWrongForWinRT As String
			Get
				Return VBResources.GetResourceString("ERR_RemoveParamWrongForWinRT", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RequiredAttributeConstConversion2 As String
			Get
				Return VBResources.GetResourceString("ERR_RequiredAttributeConstConversion2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RequiredConstConversion2 As String
			Get
				Return VBResources.GetResourceString("ERR_RequiredConstConversion2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RequiredConstExpr As String
			Get
				Return VBResources.GetResourceString("ERR_RequiredConstExpr", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RequiredNewCall2 As String
			Get
				Return VBResources.GetResourceString("ERR_RequiredNewCall2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RequiredNewCallTooMany2 As String
			Get
				Return VBResources.GetResourceString("ERR_RequiredNewCallTooMany2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RequiredNonObsoleteNewCall3 As String
			Get
				Return VBResources.GetResourceString("ERR_RequiredNonObsoleteNewCall3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RequiredNonObsoleteNewCall4 As String
			Get
				Return VBResources.GetResourceString("ERR_RequiredNonObsoleteNewCall4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReservedAssemblyName As String
			Get
				Return VBResources.GetResourceString("ERR_ReservedAssemblyName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReservedXmlNamespace As String
			Get
				Return VBResources.GetResourceString("ERR_ReservedXmlNamespace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReservedXmlPrefix As String
			Get
				Return VBResources.GetResourceString("ERR_ReservedXmlPrefix", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ResourceInModule As String
			Get
				Return VBResources.GetResourceString("ERR_ResourceInModule", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RestrictedAccess As String
			Get
				Return VBResources.GetResourceString("ERR_RestrictedAccess", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RestrictedConversion1 As String
			Get
				Return VBResources.GetResourceString("ERR_RestrictedConversion1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RestrictedResumableType1 As String
			Get
				Return VBResources.GetResourceString("ERR_RestrictedResumableType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RestrictedType1 As String
			Get
				Return VBResources.GetResourceString("ERR_RestrictedType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ResumableLambdaInExpressionTree As String
			Get
				Return VBResources.GetResourceString("ERR_ResumableLambdaInExpressionTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ResumablesCannotContainOnError As String
			Get
				Return VBResources.GetResourceString("ERR_ResumablesCannotContainOnError", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReturnFromNonFunction As String
			Get
				Return VBResources.GetResourceString("ERR_ReturnFromNonFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReturnFromNonGenericTaskAsync As String
			Get
				Return VBResources.GetResourceString("ERR_ReturnFromNonGenericTaskAsync", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ReturnWithoutValue As String
			Get
				Return VBResources.GetResourceString("ERR_ReturnWithoutValue", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RuntimeDoesNotSupportDefaultInterfaceImplementation As String
			Get
				Return VBResources.GetResourceString("ERR_RuntimeDoesNotSupportDefaultInterfaceImplementation", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RuntimeDoesNotSupportProtectedAccessForInterfaceMember As String
			Get
				Return VBResources.GetResourceString("ERR_RuntimeDoesNotSupportProtectedAccessForInterfaceMember", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_RuntimeMemberNotFound2 As String
			Get
				Return VBResources.GetResourceString("ERR_RuntimeMemberNotFound2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SecurityAttributeInvalidAction As String
			Get
				Return VBResources.GetResourceString("ERR_SecurityAttributeInvalidAction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SecurityAttributeInvalidActionAssembly As String
			Get
				Return VBResources.GetResourceString("ERR_SecurityAttributeInvalidActionAssembly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SecurityAttributeInvalidActionTypeOrMethod As String
			Get
				Return VBResources.GetResourceString("ERR_SecurityAttributeInvalidActionTypeOrMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SecurityAttributeInvalidTarget As String
			Get
				Return VBResources.GetResourceString("ERR_SecurityAttributeInvalidTarget", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SecurityAttributeMissingAction As String
			Get
				Return VBResources.GetResourceString("ERR_SecurityAttributeMissingAction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SecurityCriticalAsync As String
			Get
				Return VBResources.GetResourceString("ERR_SecurityCriticalAsync", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SecurityCriticalAsyncInClassOrStruct As String
			Get
				Return VBResources.GetResourceString("ERR_SecurityCriticalAsyncInClassOrStruct", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SetHasOnlyOneParam As String
			Get
				Return VBResources.GetResourceString("ERR_SetHasOnlyOneParam", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SetHasToBeByVal1 As String
			Get
				Return VBResources.GetResourceString("ERR_SetHasToBeByVal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SetValueNotPropertyType As String
			Get
				Return VBResources.GetResourceString("ERR_SetValueNotPropertyType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ShadowingGenericParamWithMember1 As String
			Get
				Return VBResources.GetResourceString("ERR_ShadowingGenericParamWithMember1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ShadowingTypeOutsideClass1 As String
			Get
				Return VBResources.GetResourceString("ERR_ShadowingTypeOutsideClass1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SharedConstructorIllegalSpec1 As String
			Get
				Return VBResources.GetResourceString("ERR_SharedConstructorIllegalSpec1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SharedConstructorWithParams As String
			Get
				Return VBResources.GetResourceString("ERR_SharedConstructorWithParams", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SharedEventNeedsHandlerInTheSameType As String
			Get
				Return VBResources.GetResourceString("ERR_SharedEventNeedsHandlerInTheSameType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SharedEventNeedsSharedHandler As String
			Get
				Return VBResources.GetResourceString("ERR_SharedEventNeedsSharedHandler", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SharedMemberAggrMemberInit1 As String
			Get
				Return VBResources.GetResourceString("ERR_SharedMemberAggrMemberInit1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SharedOnProcThatImpl As String
			Get
				Return VBResources.GetResourceString("ERR_SharedOnProcThatImpl", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SharedStructMemberCannotSpecifyNew As String
			Get
				Return VBResources.GetResourceString("ERR_SharedStructMemberCannotSpecifyNew", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SignButNoPrivateKey As String
			Get
				Return VBResources.GetResourceString("ERR_SignButNoPrivateKey", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SourceInterfaceMustBeInterface As String
			Get
				Return VBResources.GetResourceString("ERR_SourceInterfaceMustBeInterface", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SourceLinkRequiresPdb As String
			Get
				Return VBResources.GetResourceString("ERR_SourceLinkRequiresPdb", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SpecifiersInvalidOnInheritsImplOpt As String
			Get
				Return VBResources.GetResourceString("ERR_SpecifiersInvalidOnInheritsImplOpt", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SpecifiersInvOnEventMethod As String
			Get
				Return VBResources.GetResourceString("ERR_SpecifiersInvOnEventMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StandaloneAttribute As String
			Get
				Return VBResources.GetResourceString("ERR_StandaloneAttribute", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StartAttributeValue As String
			Get
				Return VBResources.GetResourceString("ERR_StartAttributeValue", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StartupCodeNotFound1 As String
			Get
				Return VBResources.GetResourceString("ERR_StartupCodeNotFound1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StatementLambdaInExpressionTree As String
			Get
				Return VBResources.GetResourceString("ERR_StatementLambdaInExpressionTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_STAThreadAndMTAThread0 As String
			Get
				Return VBResources.GetResourceString("ERR_STAThreadAndMTAThread0", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StaticInLambda As String
			Get
				Return VBResources.GetResourceString("ERR_StaticInLambda", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StdInOptionProvidedButConsoleInputIsNotRedirected As String
			Get
				Return VBResources.GetResourceString("ERR_StdInOptionProvidedButConsoleInputIsNotRedirected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StrictArgumentCopyBackNarrowing3 As String
			Get
				Return VBResources.GetResourceString("ERR_StrictArgumentCopyBackNarrowing3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StrictDisallowImplicitObject As String
			Get
				Return VBResources.GetResourceString("ERR_StrictDisallowImplicitObject", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StrictDisallowImplicitObjectLambda As String
			Get
				Return VBResources.GetResourceString("ERR_StrictDisallowImplicitObjectLambda", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StrictDisallowsImplicitArgs As String
			Get
				Return VBResources.GetResourceString("ERR_StrictDisallowsImplicitArgs", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StrictDisallowsImplicitProc As String
			Get
				Return VBResources.GetResourceString("ERR_StrictDisallowsImplicitProc", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StrictDisallowsLateBinding As String
			Get
				Return VBResources.GetResourceString("ERR_StrictDisallowsLateBinding", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StrictDisallowsObjectComparison1 As String
			Get
				Return VBResources.GetResourceString("ERR_StrictDisallowsObjectComparison1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StrictDisallowsObjectOperand1 As String
			Get
				Return VBResources.GetResourceString("ERR_StrictDisallowsObjectOperand1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StructCantInherit As String
			Get
				Return VBResources.GetResourceString("ERR_StructCantInherit", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StructCantUseDLLDeclareSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_StructCantUseDLLDeclareSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StructCantUseVarSpecifier1 As String
			Get
				Return VBResources.GetResourceString("ERR_StructCantUseVarSpecifier1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StructLayoutAttributeNotAllowed As String
			Get
				Return VBResources.GetResourceString("ERR_StructLayoutAttributeNotAllowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StructsCannotHandleEvents As String
			Get
				Return VBResources.GetResourceString("ERR_StructsCannotHandleEvents", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StructureCantUseProtected As String
			Get
				Return VBResources.GetResourceString("ERR_StructureCantUseProtected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StructureNoDefault1 As String
			Get
				Return VBResources.GetResourceString("ERR_StructureNoDefault1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_StructureNotExpression1 As String
			Get
				Return VBResources.GetResourceString("ERR_StructureNotExpression1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SubDisallowsStatement As String
			Get
				Return VBResources.GetResourceString("ERR_SubDisallowsStatement", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SubNewCycle1 As String
			Get
				Return VBResources.GetResourceString("ERR_SubNewCycle1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SubNewCycle2 As String
			Get
				Return VBResources.GetResourceString("ERR_SubNewCycle2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SubRequiresParenthesesBang As String
			Get
				Return VBResources.GetResourceString("ERR_SubRequiresParenthesesBang", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SubRequiresParenthesesDot As String
			Get
				Return VBResources.GetResourceString("ERR_SubRequiresParenthesesDot", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SubRequiresParenthesesLParen As String
			Get
				Return VBResources.GetResourceString("ERR_SubRequiresParenthesesLParen", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SubRequiresSingleStatement As String
			Get
				Return VBResources.GetResourceString("ERR_SubRequiresSingleStatement", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SwitchNeedsBool As String
			Get
				Return VBResources.GetResourceString("ERR_SwitchNeedsBool", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SxSIndirectRefHigherThanDirectRef3 As String
			Get
				Return VBResources.GetResourceString("ERR_SxSIndirectRefHigherThanDirectRef3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SynchronizedAsyncMethod As String
			Get
				Return VBResources.GetResourceString("ERR_SynchronizedAsyncMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SyncLockRequiresReferenceType1 As String
			Get
				Return VBResources.GetResourceString("ERR_SyncLockRequiresReferenceType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_Syntax As String
			Get
				Return VBResources.GetResourceString("ERR_Syntax", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SyntaxInCastOp As String
			Get
				Return VBResources.GetResourceString("ERR_SyntaxInCastOp", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SynthMemberClashesWithMember5 As String
			Get
				Return VBResources.GetResourceString("ERR_SynthMemberClashesWithMember5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SynthMemberClashesWithSynth7 As String
			Get
				Return VBResources.GetResourceString("ERR_SynthMemberClashesWithSynth7", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SynthMemberShadowsMustOverride5 As String
			Get
				Return VBResources.GetResourceString("ERR_SynthMemberShadowsMustOverride5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_SyntMemberShadowsGenericParam3 As String
			Get
				Return VBResources.GetResourceString("ERR_SyntMemberShadowsGenericParam3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooFewGenericArguments1 As String
			Get
				Return VBResources.GetResourceString("ERR_TooFewGenericArguments1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooFewGenericArguments2 As String
			Get
				Return VBResources.GetResourceString("ERR_TooFewGenericArguments2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooFewIndices As String
			Get
				Return VBResources.GetResourceString("ERR_TooFewIndices", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooLongMetadataName As String
			Get
				Return VBResources.GetResourceString("ERR_TooLongMetadataName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooLongOrComplexExpression As String
			Get
				Return VBResources.GetResourceString("ERR_TooLongOrComplexExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooManyArgs As String
			Get
				Return VBResources.GetResourceString("ERR_TooManyArgs", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooManyArgs1 As String
			Get
				Return VBResources.GetResourceString("ERR_TooManyArgs1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooManyArgs2 As String
			Get
				Return VBResources.GetResourceString("ERR_TooManyArgs2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooManyGenericArguments1 As String
			Get
				Return VBResources.GetResourceString("ERR_TooManyGenericArguments1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooManyGenericArguments2 As String
			Get
				Return VBResources.GetResourceString("ERR_TooManyGenericArguments2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooManyIndices As String
			Get
				Return VBResources.GetResourceString("ERR_TooManyIndices", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TooManyUserStrings As String
			Get
				Return VBResources.GetResourceString("ERR_TooManyUserStrings", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TryAndOnErrorDoNotMix As String
			Get
				Return VBResources.GetResourceString("ERR_TryAndOnErrorDoNotMix", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TryCastOfUnconstrainedTypeParam1 As String
			Get
				Return VBResources.GetResourceString("ERR_TryCastOfUnconstrainedTypeParam1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TryCastOfValueType1 As String
			Get
				Return VBResources.GetResourceString("ERR_TryCastOfValueType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TryWithoutCatchOrFinally As String
			Get
				Return VBResources.GetResourceString("ERR_TryWithoutCatchOrFinally", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TupleDuplicateElementName As String
			Get
				Return VBResources.GetResourceString("ERR_TupleDuplicateElementName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TupleElementNamesAttributeMissing As String
			Get
				Return VBResources.GetResourceString("ERR_TupleElementNamesAttributeMissing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TupleInferredNamesNotAvailable As String
			Get
				Return VBResources.GetResourceString("ERR_TupleInferredNamesNotAvailable", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TupleLiteralDisallowsTypeChar As String
			Get
				Return VBResources.GetResourceString("ERR_TupleLiteralDisallowsTypeChar", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TupleReservedElementName As String
			Get
				Return VBResources.GetResourceString("ERR_TupleReservedElementName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TupleReservedElementNameAnyPosition As String
			Get
				Return VBResources.GetResourceString("ERR_TupleReservedElementNameAnyPosition", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TupleTooFewElements As String
			Get
				Return VBResources.GetResourceString("ERR_TupleTooFewElements", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TwoParametersRequired1 As String
			Get
				Return VBResources.GetResourceString("ERR_TwoParametersRequired1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeArgsUnexpected As String
			Get
				Return VBResources.GetResourceString("ERR_TypeArgsUnexpected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypecharNoMatch2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypecharNoMatch2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypecharNotallowed As String
			Get
				Return VBResources.GetResourceString("ERR_TypecharNotallowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeCharOnAggregation As String
			Get
				Return VBResources.GetResourceString("ERR_TypeCharOnAggregation", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeCharOnGenericParam As String
			Get
				Return VBResources.GetResourceString("ERR_TypeCharOnGenericParam", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeCharOnSub As String
			Get
				Return VBResources.GetResourceString("ERR_TypeCharOnSub", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeCharWithType1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeCharWithType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeClashesWithVbCoreType4 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeClashesWithVbCoreType4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeConflict6 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeConflict6", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeDisallowsAttributes As String
			Get
				Return VBResources.GetResourceString("ERR_TypeDisallowsAttributes", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeDisallowsDescendants As String
			Get
				Return VBResources.GetResourceString("ERR_TypeDisallowsDescendants", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeDisallowsElements As String
			Get
				Return VBResources.GetResourceString("ERR_TypeDisallowsElements", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeForwardedToMultipleAssemblies As String
			Get
				Return VBResources.GetResourceString("ERR_TypeForwardedToMultipleAssemblies", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeFwdCycle2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeFwdCycle2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceArrayRankMismatch1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceArrayRankMismatch1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailure1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailure1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailure2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailure2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailure3 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailure3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureAmbiguous1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureAmbiguous1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureAmbiguous2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureAmbiguous2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureAmbiguous3 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureAmbiguous3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoBest1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoBest1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoBest2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoBest2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoBest3 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoBest3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoExplicit1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoExplicit1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoExplicit2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoExplicit2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoExplicit3 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoExplicit3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoExplicitAmbiguous1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoExplicitAmbiguous1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoExplicitAmbiguous2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoExplicitAmbiguous2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoExplicitAmbiguous3 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoExplicitAmbiguous3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoExplicitNoBest1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoExplicitNoBest1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoExplicitNoBest2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoExplicitNoBest2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInferenceFailureNoExplicitNoBest3 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInferenceFailureNoExplicitNoBest3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeInItsInheritsClause1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeInItsInheritsClause1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeMismatch2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeMismatch2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeMismatchForXml3 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeMismatchForXml3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeNotExpression1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeNotExpression1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeOfExprAlwaysFalse2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeOfExprAlwaysFalse2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeOfRequiresReferenceType1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeOfRequiresReferenceType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeOrMemberNotGeneric1 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeOrMemberNotGeneric1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeOrMemberNotGeneric2 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeOrMemberNotGeneric2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeParamMissingAsCommaOrRParen As String
			Get
				Return VBResources.GetResourceString("ERR_TypeParamMissingAsCommaOrRParen", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeParamMissingCommaOrRParen As String
			Get
				Return VBResources.GetResourceString("ERR_TypeParamMissingCommaOrRParen", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeParamNameFunctionNameCollision As String
			Get
				Return VBResources.GetResourceString("ERR_TypeParamNameFunctionNameCollision", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeParamQualifierDisallowed As String
			Get
				Return VBResources.GetResourceString("ERR_TypeParamQualifierDisallowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeParamWithStructConstAsConst As String
			Get
				Return VBResources.GetResourceString("ERR_TypeParamWithStructConstAsConst", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_TypeRefResolutionError3 As String
			Get
				Return VBResources.GetResourceString("ERR_TypeRefResolutionError3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnableToCreateTempFile As String
			Get
				Return VBResources.GetResourceString("ERR_UnableToCreateTempFile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnableToOpenResourceFile1 As String
			Get
				Return VBResources.GetResourceString("ERR_UnableToOpenResourceFile1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnableToReadUacManifest2 As String
			Get
				Return VBResources.GetResourceString("ERR_UnableToReadUacManifest2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnacceptableForLoopOperator2 As String
			Get
				Return VBResources.GetResourceString("ERR_UnacceptableForLoopOperator2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnacceptableForLoopRelOperator2 As String
			Get
				Return VBResources.GetResourceString("ERR_UnacceptableForLoopRelOperator2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnacceptableLogicalOperator3 As String
			Get
				Return VBResources.GetResourceString("ERR_UnacceptableLogicalOperator3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnaryOperand2 As String
			Get
				Return VBResources.GetResourceString("ERR_UnaryOperand2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnaryParamMustBeContainingType1 As String
			Get
				Return VBResources.GetResourceString("ERR_UnaryParamMustBeContainingType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnboundTypeParam1 As String
			Get
				Return VBResources.GetResourceString("ERR_UnboundTypeParam1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnboundTypeParam2 As String
			Get
				Return VBResources.GetResourceString("ERR_UnboundTypeParam2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnboundTypeParam3 As String
			Get
				Return VBResources.GetResourceString("ERR_UnboundTypeParam3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UndefinedType1 As String
			Get
				Return VBResources.GetResourceString("ERR_UndefinedType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UndefinedTypeOrNamespace1 As String
			Get
				Return VBResources.GetResourceString("ERR_UndefinedTypeOrNamespace1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UndefinedXmlPrefix As String
			Get
				Return VBResources.GetResourceString("ERR_UndefinedXmlPrefix", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnexpectedExpressionStatement As String
			Get
				Return VBResources.GetResourceString("ERR_UnexpectedExpressionStatement", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnexpectedGroup As String
			Get
				Return VBResources.GetResourceString("ERR_UnexpectedGroup", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnimplementedMember3 As String
			Get
				Return VBResources.GetResourceString("ERR_UnimplementedMember3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnimplementedMustOverride As String
			Get
				Return VBResources.GetResourceString("ERR_UnimplementedMustOverride", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnknownOperator As String
			Get
				Return VBResources.GetResourceString("ERR_UnknownOperator", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnmanagedCallersOnlyNotSupported As String
			Get
				Return VBResources.GetResourceString("ERR_UnmanagedCallersOnlyNotSupported", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnrecognizedEnd As String
			Get
				Return VBResources.GetResourceString("ERR_UnrecognizedEnd", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnrecognizedType As String
			Get
				Return VBResources.GetResourceString("ERR_UnrecognizedType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnrecognizedTypeKeyword As String
			Get
				Return VBResources.GetResourceString("ERR_UnrecognizedTypeKeyword", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnrecognizedTypeOrWith As String
			Get
				Return VBResources.GetResourceString("ERR_UnrecognizedTypeOrWith", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnreferencedAssembly3 As String
			Get
				Return VBResources.GetResourceString("ERR_UnreferencedAssembly3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnreferencedAssemblyEvent3 As String
			Get
				Return VBResources.GetResourceString("ERR_UnreferencedAssemblyEvent3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnreferencedModule3 As String
			Get
				Return VBResources.GetResourceString("ERR_UnreferencedModule3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnreferencedModuleEvent3 As String
			Get
				Return VBResources.GetResourceString("ERR_UnreferencedModuleEvent3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnsupportedConstant2 As String
			Get
				Return VBResources.GetResourceString("ERR_UnsupportedConstant2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnsupportedEvent1 As String
			Get
				Return VBResources.GetResourceString("ERR_UnsupportedEvent1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnsupportedField1 As String
			Get
				Return VBResources.GetResourceString("ERR_UnsupportedField1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnsupportedMethod1 As String
			Get
				Return VBResources.GetResourceString("ERR_UnsupportedMethod1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnsupportedModule1 As String
			Get
				Return VBResources.GetResourceString("ERR_UnsupportedModule1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnsupportedProperty1 As String
			Get
				Return VBResources.GetResourceString("ERR_UnsupportedProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnsupportedType1 As String
			Get
				Return VBResources.GetResourceString("ERR_UnsupportedType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UnterminatedStringLiteral As String
			Get
				Return VBResources.GetResourceString("ERR_UnterminatedStringLiteral", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UseOfKeywordFromModule1 As String
			Get
				Return VBResources.GetResourceString("ERR_UseOfKeywordFromModule1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UseOfKeywordFromStructure1 As String
			Get
				Return VBResources.GetResourceString("ERR_UseOfKeywordFromStructure1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UseOfKeywordNotInInstanceMethod1 As String
			Get
				Return VBResources.GetResourceString("ERR_UseOfKeywordNotInInstanceMethod1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UseOfLocalBeforeDeclaration1 As String
			Get
				Return VBResources.GetResourceString("ERR_UseOfLocalBeforeDeclaration1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UseOfObsoletePropertyAccessor2 As String
			Get
				Return VBResources.GetResourceString("ERR_UseOfObsoletePropertyAccessor2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UseOfObsoletePropertyAccessor3 As String
			Get
				Return VBResources.GetResourceString("ERR_UseOfObsoletePropertyAccessor3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UseOfObsoleteSymbol2 As String
			Get
				Return VBResources.GetResourceString("ERR_UseOfObsoleteSymbol2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UseOfObsoleteSymbolNoMessage1 As String
			Get
				Return VBResources.GetResourceString("ERR_UseOfObsoleteSymbolNoMessage1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UsingRequiresDisposePattern As String
			Get
				Return VBResources.GetResourceString("ERR_UsingRequiresDisposePattern", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UsingResourceVarCantBeArray As String
			Get
				Return VBResources.GetResourceString("ERR_UsingResourceVarCantBeArray", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_UsingResourceVarNeedsInitializer As String
			Get
				Return VBResources.GetResourceString("ERR_UsingResourceVarNeedsInitializer", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ValueAndClassTypeConstrCombined As String
			Get
				Return VBResources.GetResourceString("ERR_ValueAndClassTypeConstrCombined", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ValueTupleResolutionAmbiguous3 As String
			Get
				Return VBResources.GetResourceString("ERR_ValueTupleResolutionAmbiguous3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ValueTupleTypeRefResolutionError1 As String
			Get
				Return VBResources.GetResourceString("ERR_ValueTupleTypeRefResolutionError1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceConversionFailedIn6 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceConversionFailedIn6", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceConversionFailedOut6 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceConversionFailedOut6", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceConversionFailedTryIn4 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceConversionFailedTryIn4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceConversionFailedTryOut4 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceConversionFailedTryOut4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceDisallowedHere As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceDisallowedHere", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceIEnumerableSuggestion3 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceIEnumerableSuggestion3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceInByRefDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceInByRefDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceInNullableDisallowed2 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceInNullableDisallowed2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceInParamDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceInParamDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceInParamDisallowedForGeneric3 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceInParamDisallowedForGeneric3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceInParamDisallowedHere2 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceInParamDisallowedHere2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceInParamDisallowedHereForGeneric4 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceInParamDisallowedHereForGeneric4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceInPropertyDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceInPropertyDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceInReadOnlyPropertyDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceInReadOnlyPropertyDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceInReturnDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceInReturnDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceInterfaceNesting As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceInterfaceNesting", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceOutByRefDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceOutByRefDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceOutByValDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceOutByValDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceOutConstraintDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceOutConstraintDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceOutNullableDisallowed2 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceOutNullableDisallowed2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceOutParamDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceOutParamDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceOutParamDisallowedForGeneric3 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceOutParamDisallowedForGeneric3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceOutParamDisallowedHere2 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceOutParamDisallowedHere2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceOutParamDisallowedHereForGeneric4 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceOutParamDisallowedHereForGeneric4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceOutPropertyDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceOutPropertyDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceOutWriteOnlyPropertyDisallowed1 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceOutWriteOnlyPropertyDisallowed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VariancePreventsSynthesizedEvents2 As String
			Get
				Return VBResources.GetResourceString("ERR_VariancePreventsSynthesizedEvents2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceTypeDisallowed2 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceTypeDisallowed2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceTypeDisallowedForGeneric4 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceTypeDisallowedForGeneric4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceTypeDisallowedHere3 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceTypeDisallowedHere3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VarianceTypeDisallowedHereForGeneric5 As String
			Get
				Return VBResources.GetResourceString("ERR_VarianceTypeDisallowedHereForGeneric5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VBCoreNetModuleConflict As String
			Get
				Return VBResources.GetResourceString("ERR_VBCoreNetModuleConflict", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VersionMustBeFirstInXmlDecl As String
			Get
				Return VBResources.GetResourceString("ERR_VersionMustBeFirstInXmlDecl", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VoidArrayDisallowed As String
			Get
				Return VBResources.GetResourceString("ERR_VoidArrayDisallowed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_VoidValue As String
			Get
				Return VBResources.GetResourceString("ERR_VoidValue", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_WinRTEventWithoutDelegate As String
			Get
				Return VBResources.GetResourceString("ERR_WinRTEventWithoutDelegate", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_WithEventsAsStruct As String
			Get
				Return VBResources.GetResourceString("ERR_WithEventsAsStruct", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_WithEventsRequiresClass As String
			Get
				Return VBResources.GetResourceString("ERR_WithEventsRequiresClass", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_WriteOnlyHasGet As String
			Get
				Return VBResources.GetResourceString("ERR_WriteOnlyHasGet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_WriteOnlyHasNoWrite As String
			Get
				Return VBResources.GetResourceString("ERR_WriteOnlyHasNoWrite", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_WriteOnlyNoAccessorFlag As String
			Get
				Return VBResources.GetResourceString("ERR_WriteOnlyNoAccessorFlag", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_XmlEndCDataNotAllowedInContent As String
			Get
				Return VBResources.GetResourceString("ERR_XmlEndCDataNotAllowedInContent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_XmlEndElementNoMatchingStart As String
			Get
				Return VBResources.GetResourceString("ERR_XmlEndElementNoMatchingStart", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_XmlEntityReference As String
			Get
				Return VBResources.GetResourceString("ERR_XmlEntityReference", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_XmlFeaturesNotAvailable As String
			Get
				Return VBResources.GetResourceString("ERR_XmlFeaturesNotAvailable", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_XmlPrefixNotExpression As String
			Get
				Return VBResources.GetResourceString("ERR_XmlPrefixNotExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ERR_ZeroDivide As String
			Get
				Return VBResources.GetResourceString("ERR_ZeroDivide", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_ArrayLiterals As String
			Get
				Return VBResources.GetResourceString("FEATURE_ArrayLiterals", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_AsyncExpressions As String
			Get
				Return VBResources.GetResourceString("FEATURE_AsyncExpressions", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_AutoProperties As String
			Get
				Return VBResources.GetResourceString("FEATURE_AutoProperties", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_BinaryLiterals As String
			Get
				Return VBResources.GetResourceString("FEATURE_BinaryLiterals", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_CObjInAttributeArguments As String
			Get
				Return VBResources.GetResourceString("FEATURE_CObjInAttributeArguments", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_CoContraVariance As String
			Get
				Return VBResources.GetResourceString("FEATURE_CoContraVariance", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_CollectionInitializers As String
			Get
				Return VBResources.GetResourceString("FEATURE_CollectionInitializers", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_CommentsAfterLineContinuation As String
			Get
				Return VBResources.GetResourceString("FEATURE_CommentsAfterLineContinuation", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_DigitSeparators As String
			Get
				Return VBResources.GetResourceString("FEATURE_DigitSeparators", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_GlobalNamespace As String
			Get
				Return VBResources.GetResourceString("FEATURE_GlobalNamespace", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_ImplementingReadonlyOrWriteonlyPropertyWithReadwrite As String
			Get
				Return VBResources.GetResourceString("FEATURE_ImplementingReadonlyOrWriteonlyPropertyWithReadwrite", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_InitOnlySettersUsage As String
			Get
				Return VBResources.GetResourceString("FEATURE_InitOnlySettersUsage", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_InterpolatedStrings As String
			Get
				Return VBResources.GetResourceString("FEATURE_InterpolatedStrings", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_Iterators As String
			Get
				Return VBResources.GetResourceString("FEATURE_Iterators", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_LeadingDigitSeparator As String
			Get
				Return VBResources.GetResourceString("FEATURE_LeadingDigitSeparator", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_LineContinuation As String
			Get
				Return VBResources.GetResourceString("FEATURE_LineContinuation", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_LineContinuationComments As String
			Get
				Return VBResources.GetResourceString("FEATURE_LineContinuationComments", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_MultilineStringLiterals As String
			Get
				Return VBResources.GetResourceString("FEATURE_MultilineStringLiterals", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_NameOfExpressions As String
			Get
				Return VBResources.GetResourceString("FEATURE_NameOfExpressions", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_NullPropagatingOperator As String
			Get
				Return VBResources.GetResourceString("FEATURE_NullPropagatingOperator", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_PartialInterfaces As String
			Get
				Return VBResources.GetResourceString("FEATURE_PartialInterfaces", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_PartialModules As String
			Get
				Return VBResources.GetResourceString("FEATURE_PartialModules", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_PrivateProtected As String
			Get
				Return VBResources.GetResourceString("FEATURE_PrivateProtected", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_ReadonlyAutoProperties As String
			Get
				Return VBResources.GetResourceString("FEATURE_ReadonlyAutoProperties", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_RegionsEverywhere As String
			Get
				Return VBResources.GetResourceString("FEATURE_RegionsEverywhere", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_StatementLambdas As String
			Get
				Return VBResources.GetResourceString("FEATURE_StatementLambdas", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_SubLambdas As String
			Get
				Return VBResources.GetResourceString("FEATURE_SubLambdas", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_Tuples As String
			Get
				Return VBResources.GetResourceString("FEATURE_Tuples", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_TypeOfIsNot As String
			Get
				Return VBResources.GetResourceString("FEATURE_TypeOfIsNot", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_UnconstrainedTypeParameterInConditional As String
			Get
				Return VBResources.GetResourceString("FEATURE_UnconstrainedTypeParameterInConditional", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_WarningDirectives As String
			Get
				Return VBResources.GetResourceString("FEATURE_WarningDirectives", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FEATURE_YearFirstDateLiterals As String
			Get
				Return VBResources.GetResourceString("FEATURE_YearFirstDateLiterals", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FieldInitializerSyntaxNotWithinSyntaxTree As String
			Get
				Return VBResources.GetResourceString("FieldInitializerSyntaxNotWithinSyntaxTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FTL_InvalidInputFileName As String
			Get
				Return VBResources.GetResourceString("FTL_InvalidInputFileName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property FunctionSyntaxNotWithinSyntaxTree As String
			Get
				Return VBResources.GetResourceString("FunctionSyntaxNotWithinSyntaxTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property HDN_UnusedImportClause As String
			Get
				Return VBResources.GetResourceString("HDN_UnusedImportClause", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property HDN_UnusedImportClause_Title As String
			Get
				Return VBResources.GetResourceString("HDN_UnusedImportClause_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property HDN_UnusedImportStatement As String
			Get
				Return VBResources.GetResourceString("HDN_UnusedImportStatement", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property HDN_UnusedImportStatement_Title As String
			Get
				Return VBResources.GetResourceString("HDN_UnusedImportStatement_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IArgumentIsNotVisualBasicArgument As String
			Get
				Return VBResources.GetResourceString("IArgumentIsNotVisualBasicArgument", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ICompoundAssignmentOperationIsNotVisualBasicCompoundAssignment As String
			Get
				Return VBResources.GetResourceString("ICompoundAssignmentOperationIsNotVisualBasicCompoundAssignment", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IConversionExpressionIsNotVisualBasicConversion As String
			Get
				Return VBResources.GetResourceString("IConversionExpressionIsNotVisualBasicConversion", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IdentifierSyntaxNotWithinSyntaxTree As String
			Get
				Return VBResources.GetResourceString("IdentifierSyntaxNotWithinSyntaxTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_FunctionReturnType As String
			Get
				Return VBResources.GetResourceString("IDS_FunctionReturnType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_LangVersions As String
			Get
				Return VBResources.GetResourceString("IDS_LangVersions", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_LogoLine1 As String
			Get
				Return VBResources.GetResourceString("IDS_LogoLine1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_LogoLine2 As String
			Get
				Return VBResources.GetResourceString("IDS_LogoLine2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_MSG_ADDLINKREFERENCE As String
			Get
				Return VBResources.GetResourceString("IDS_MSG_ADDLINKREFERENCE", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_MSG_ADDMODULE As String
			Get
				Return VBResources.GetResourceString("IDS_MSG_ADDMODULE", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_MSG_ADDREFERENCE As String
			Get
				Return VBResources.GetResourceString("IDS_MSG_ADDREFERENCE", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_ProjectSettingsLocationName As String
			Get
				Return VBResources.GetResourceString("IDS_ProjectSettingsLocationName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_TheSystemCannotFindThePathSpecified As String
			Get
				Return VBResources.GetResourceString("IDS_TheSystemCannotFindThePathSpecified", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_ToolName As String
			Get
				Return VBResources.GetResourceString("IDS_ToolName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property IDS_VBCHelp As String
			Get
				Return VBResources.GetResourceString("IDS_VBCHelp", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property INF_UnableToLoadSomeTypesInAnalyzer As String
			Get
				Return VBResources.GetResourceString("INF_UnableToLoadSomeTypesInAnalyzer", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property INF_UnableToLoadSomeTypesInAnalyzer_Title As String
			Get
				Return VBResources.GetResourceString("INF_UnableToLoadSomeTypesInAnalyzer_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property LocationMustBeProvided As String
			Get
				Return VBResources.GetResourceString("LocationMustBeProvided", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property NodeIsNotWithinSyntaxTree As String
			Get
				Return VBResources.GetResourceString("NodeIsNotWithinSyntaxTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property NoNoneSearchCriteria As String
			Get
				Return VBResources.GetResourceString("NoNoneSearchCriteria", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property NotAVbSymbol As String
			Get
				Return VBResources.GetResourceString("NotAVbSymbol", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property NotWithinTree As String
			Get
				Return VBResources.GetResourceString("NotWithinTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property NumberOfTypeParametersAndArgumentsMustMatch As String
			Get
				Return VBResources.GetResourceString("NumberOfTypeParametersAndArgumentsMustMatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property PositionIsNotWithinSyntax As String
			Get
				Return VBResources.GetResourceString("PositionIsNotWithinSyntax", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property PositionNotWithinTree As String
			Get
				Return VBResources.GetResourceString("PositionNotWithinTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property PositionOfTypeParameterTooLarge As String
			Get
				Return VBResources.GetResourceString("PositionOfTypeParameterTooLarge", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property PropertiesCanNotHaveTypeArguments As String
			Get
				Return VBResources.GetResourceString("PropertiesCanNotHaveTypeArguments", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property RangeVariableSyntaxNotWithinSyntaxTree As String
			Get
				Return VBResources.GetResourceString("RangeVariableSyntaxNotWithinSyntaxTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ResourceManager As System.Resources.ResourceManager
			Get
				If (VBResources.s_resourceManager Is Nothing) Then
					VBResources.s_resourceManager = New System.Resources.ResourceManager(GetType(VBResources))
				End If
				Return VBResources.s_resourceManager
			End Get
		End Property

		Friend ReadOnly Shared Property SemanticModelMustBeProvided As String
			Get
				Return VBResources.GetResourceString("SemanticModelMustBeProvided", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property SpeculatedSyntaxNodeCannotBelongToCurrentCompilation As String
			Get
				Return VBResources.GetResourceString("SpeculatedSyntaxNodeCannotBelongToCurrentCompilation", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property StatementOrExpressionIsNotAValidType As String
			Get
				Return VBResources.GetResourceString("StatementOrExpressionIsNotAValidType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property SubmissionCanHaveAtMostOneSyntaxTree As String
			Get
				Return VBResources.GetResourceString("SubmissionCanHaveAtMostOneSyntaxTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property SyntaxTreeAlreadyPresent As String
			Get
				Return VBResources.GetResourceString("SyntaxTreeAlreadyPresent", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property SyntaxTreeIsNotASubmission As String
			Get
				Return VBResources.GetResourceString("SyntaxTreeIsNotASubmission", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property SyntaxTreeNotFoundToRemove As String
			Get
				Return VBResources.GetResourceString("SyntaxTreeNotFoundToRemove", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ThereAreNoFunctionPointerTypesInVB As String
			Get
				Return VBResources.GetResourceString("ThereAreNoFunctionPointerTypesInVB", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ThereAreNoNativeIntegerTypesInVB As String
			Get
				Return VBResources.GetResourceString("ThereAreNoNativeIntegerTypesInVB", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ThereAreNoPointerTypesInVB As String
			Get
				Return VBResources.GetResourceString("ThereAreNoPointerTypesInVB", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property ThereIsNoDynamicTypeInVB As String
			Get
				Return VBResources.GetResourceString("ThereIsNoDynamicTypeInVB", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property TreeMustHaveARootNodeWithCompilationUnit As String
			Get
				Return VBResources.GetResourceString("TreeMustHaveARootNodeWithCompilationUnit", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property Trees0 As String
			Get
				Return VBResources.GetResourceString("Trees0", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property TreesMustHaveRootNode As String
			Get
				Return VBResources.GetResourceString("TreesMustHaveRootNode", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property TypeArgumentCannotBeNothing As String
			Get
				Return VBResources.GetResourceString("TypeArgumentCannotBeNothing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property TypeParameterNotWithinTree As String
			Get
				Return VBResources.GetResourceString("TypeParameterNotWithinTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property VariableSyntaxNotWithinSyntaxTree As String
			Get
				Return VBResources.GetResourceString("VariableSyntaxNotWithinSyntaxTree", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AmbiguousCastConversion2 As String
			Get
				Return VBResources.GetResourceString("WRN_AmbiguousCastConversion2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AmbiguousCastConversion2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_AmbiguousCastConversion2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AnalyzerCannotBeCreated As String
			Get
				Return VBResources.GetResourceString("WRN_AnalyzerCannotBeCreated", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AnalyzerCannotBeCreated_Title As String
			Get
				Return VBResources.GetResourceString("WRN_AnalyzerCannotBeCreated_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AnalyzerReferencesFramework As String
			Get
				Return VBResources.GetResourceString("WRN_AnalyzerReferencesFramework", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AnalyzerReferencesFramework_Title As String
			Get
				Return VBResources.GetResourceString("WRN_AnalyzerReferencesFramework_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ArrayInitNoTypeObjectAssumed As String
			Get
				Return VBResources.GetResourceString("WRN_ArrayInitNoTypeObjectAssumed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ArrayInitNoTypeObjectAssumed_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ArrayInitNoTypeObjectAssumed_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ArrayInitTooManyTypesObjectAssumed As String
			Get
				Return VBResources.GetResourceString("WRN_ArrayInitTooManyTypesObjectAssumed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ArrayInitTooManyTypesObjectAssumed_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ArrayInitTooManyTypesObjectAssumed_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ArrayOverloadsNonCLS2 As String
			Get
				Return VBResources.GetResourceString("WRN_ArrayOverloadsNonCLS2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ArrayOverloadsNonCLS2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ArrayOverloadsNonCLS2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AssemblyAttributeFromModuleIsOverridden As String
			Get
				Return VBResources.GetResourceString("WRN_AssemblyAttributeFromModuleIsOverridden", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AssemblyAttributeFromModuleIsOverridden_Title As String
			Get
				Return VBResources.GetResourceString("WRN_AssemblyAttributeFromModuleIsOverridden_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AssemblyGeneration0 As String
			Get
				Return VBResources.GetResourceString("WRN_AssemblyGeneration0", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AssemblyGeneration0_Title As String
			Get
				Return VBResources.GetResourceString("WRN_AssemblyGeneration0_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AssemblyGeneration1 As String
			Get
				Return VBResources.GetResourceString("WRN_AssemblyGeneration1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AssemblyGeneration1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_AssemblyGeneration1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AsyncLacksAwaits As String
			Get
				Return VBResources.GetResourceString("WRN_AsyncLacksAwaits", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AsyncLacksAwaits_Title As String
			Get
				Return VBResources.GetResourceString("WRN_AsyncLacksAwaits_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AsyncSubCouldBeFunction As String
			Get
				Return VBResources.GetResourceString("WRN_AsyncSubCouldBeFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AsyncSubCouldBeFunction_Title As String
			Get
				Return VBResources.GetResourceString("WRN_AsyncSubCouldBeFunction_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AttributeIgnoredWhenPublicSigning As String
			Get
				Return VBResources.GetResourceString("WRN_AttributeIgnoredWhenPublicSigning", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AttributeIgnoredWhenPublicSigning_Title As String
			Get
				Return VBResources.GetResourceString("WRN_AttributeIgnoredWhenPublicSigning_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AttributeNotSupportedInVB As String
			Get
				Return VBResources.GetResourceString("WRN_AttributeNotSupportedInVB", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_AttributeNotSupportedInVB_Title As String
			Get
				Return VBResources.GetResourceString("WRN_AttributeNotSupportedInVB_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_BadChecksumValExtChecksum As String
			Get
				Return VBResources.GetResourceString("WRN_BadChecksumValExtChecksum", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_BadChecksumValExtChecksum_Title As String
			Get
				Return VBResources.GetResourceString("WRN_BadChecksumValExtChecksum_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_BadGUIDFormatExtChecksum As String
			Get
				Return VBResources.GetResourceString("WRN_BadGUIDFormatExtChecksum", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_BadGUIDFormatExtChecksum_Title As String
			Get
				Return VBResources.GetResourceString("WRN_BadGUIDFormatExtChecksum_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_BadSwitch As String
			Get
				Return VBResources.GetResourceString("WRN_BadSwitch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_BadSwitch_Title As String
			Get
				Return VBResources.GetResourceString("WRN_BadSwitch_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_BadUILang As String
			Get
				Return VBResources.GetResourceString("WRN_BadUILang", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_BadUILang_Title As String
			Get
				Return VBResources.GetResourceString("WRN_BadUILang_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_BaseClassNotCLSCompliant2 As String
			Get
				Return VBResources.GetResourceString("WRN_BaseClassNotCLSCompliant2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_BaseClassNotCLSCompliant2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_BaseClassNotCLSCompliant2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_CannotFindStandardLibrary1 As String
			Get
				Return VBResources.GetResourceString("WRN_CannotFindStandardLibrary1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_CannotFindStandardLibrary1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_CannotFindStandardLibrary1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_CLSAttrInvalidOnGetSet As String
			Get
				Return VBResources.GetResourceString("WRN_CLSAttrInvalidOnGetSet", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_CLSAttrInvalidOnGetSet_Title As String
			Get
				Return VBResources.GetResourceString("WRN_CLSAttrInvalidOnGetSet_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_CLSEventMethodInNonCLSType3 As String
			Get
				Return VBResources.GetResourceString("WRN_CLSEventMethodInNonCLSType3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_CLSEventMethodInNonCLSType3_Title As String
			Get
				Return VBResources.GetResourceString("WRN_CLSEventMethodInNonCLSType3_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_CLSMemberInNonCLSType3 As String
			Get
				Return VBResources.GetResourceString("WRN_CLSMemberInNonCLSType3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_CLSMemberInNonCLSType3_Title As String
			Get
				Return VBResources.GetResourceString("WRN_CLSMemberInNonCLSType3_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ComClassInterfaceShadows5 As String
			Get
				Return VBResources.GetResourceString("WRN_ComClassInterfaceShadows5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ComClassInterfaceShadows5_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ComClassInterfaceShadows5_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ComClassNoMembers1 As String
			Get
				Return VBResources.GetResourceString("WRN_ComClassNoMembers1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ComClassNoMembers1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ComClassNoMembers1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ComClassPropertySetObject1 As String
			Get
				Return VBResources.GetResourceString("WRN_ComClassPropertySetObject1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ComClassPropertySetObject1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ComClassPropertySetObject1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ConditionalNotValidOnFunction As String
			Get
				Return VBResources.GetResourceString("WRN_ConditionalNotValidOnFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ConditionalNotValidOnFunction_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ConditionalNotValidOnFunction_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ConflictingMachineAssembly As String
			Get
				Return VBResources.GetResourceString("WRN_ConflictingMachineAssembly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ConflictingMachineAssembly_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ConflictingMachineAssembly_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ConstraintsFailedForInferredArgs2 As String
			Get
				Return VBResources.GetResourceString("WRN_ConstraintsFailedForInferredArgs2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ConstraintsFailedForInferredArgs2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ConstraintsFailedForInferredArgs2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DebuggerHiddenIgnoredOnProperties As String
			Get
				Return VBResources.GetResourceString("WRN_DebuggerHiddenIgnoredOnProperties", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DebuggerHiddenIgnoredOnProperties_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DebuggerHiddenIgnoredOnProperties_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValFuncRef1 As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValFuncRef1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValFuncRef1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValFuncRef1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValFuncVal1 As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValFuncVal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValFuncVal1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValFuncVal1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValOpRef1 As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValOpRef1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValOpRef1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValOpRef1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValOpVal1 As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValOpVal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValOpVal1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValOpVal1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValPropRef1 As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValPropRef1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValPropRef1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValPropRef1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValPropVal1 As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValPropVal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValPropVal1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValPropVal1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValWinRtEventVal1 As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValWinRtEventVal1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgNoRetValWinRtEventVal1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgNoRetValWinRtEventVal1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgUseNullRef As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgUseNullRef", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgUseNullRef_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgUseNullRef_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgUseNullRefByRef As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgUseNullRefByRef", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgUseNullRefByRef_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgUseNullRefByRef_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgUseNullRefByRefStr As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgUseNullRefByRefStr", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgUseNullRefByRefStr_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgUseNullRefByRefStr_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgUseNullRefStr As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgUseNullRefStr", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefAsgUseNullRefStr_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefAsgUseNullRefStr_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefaultnessShadowed4 As String
			Get
				Return VBResources.GetResourceString("WRN_DefaultnessShadowed4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DefaultnessShadowed4_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DefaultnessShadowed4_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DelaySignButNoKey As String
			Get
				Return VBResources.GetResourceString("WRN_DelaySignButNoKey", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DelaySignButNoKey_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DelaySignButNoKey_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DuplicateCatch As String
			Get
				Return VBResources.GetResourceString("WRN_DuplicateCatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_DuplicateCatch_Title As String
			Get
				Return VBResources.GetResourceString("WRN_DuplicateCatch_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_EmptyPrefixAndXmlnsLocalName As String
			Get
				Return VBResources.GetResourceString("WRN_EmptyPrefixAndXmlnsLocalName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_EmptyPrefixAndXmlnsLocalName_Title As String
			Get
				Return VBResources.GetResourceString("WRN_EmptyPrefixAndXmlnsLocalName_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_EnumUnderlyingTypeNotCLS1 As String
			Get
				Return VBResources.GetResourceString("WRN_EnumUnderlyingTypeNotCLS1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_EnumUnderlyingTypeNotCLS1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_EnumUnderlyingTypeNotCLS1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_EqualToLiteralNothing As String
			Get
				Return VBResources.GetResourceString("WRN_EqualToLiteralNothing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_EqualToLiteralNothing_Title As String
			Get
				Return VBResources.GetResourceString("WRN_EqualToLiteralNothing_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_EventDelegateTypeNotCLSCompliant2 As String
			Get
				Return VBResources.GetResourceString("WRN_EventDelegateTypeNotCLSCompliant2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_EventDelegateTypeNotCLSCompliant2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_EventDelegateTypeNotCLSCompliant2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ExpectedInitComponentCall2 As String
			Get
				Return VBResources.GetResourceString("WRN_ExpectedInitComponentCall2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ExpectedInitComponentCall2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ExpectedInitComponentCall2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_Experimental As String
			Get
				Return VBResources.GetResourceString("WRN_Experimental", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_Experimental_Title As String
			Get
				Return VBResources.GetResourceString("WRN_Experimental_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_FieldNotCLSCompliant1 As String
			Get
				Return VBResources.GetResourceString("WRN_FieldNotCLSCompliant1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_FieldNotCLSCompliant1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_FieldNotCLSCompliant1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_FileAlreadyIncluded As String
			Get
				Return VBResources.GetResourceString("WRN_FileAlreadyIncluded", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_FileAlreadyIncluded_Title As String
			Get
				Return VBResources.GetResourceString("WRN_FileAlreadyIncluded_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_GeneratorFailedDuringGeneration As String
			Get
				Return VBResources.GetResourceString("WRN_GeneratorFailedDuringGeneration", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_GeneratorFailedDuringGeneration_Description As String
			Get
				Return VBResources.GetResourceString("WRN_GeneratorFailedDuringGeneration_Description", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_GeneratorFailedDuringGeneration_Title As String
			Get
				Return VBResources.GetResourceString("WRN_GeneratorFailedDuringGeneration_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_GeneratorFailedDuringInitialization As String
			Get
				Return VBResources.GetResourceString("WRN_GeneratorFailedDuringInitialization", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_GeneratorFailedDuringInitialization_Description As String
			Get
				Return VBResources.GetResourceString("WRN_GeneratorFailedDuringInitialization_Description", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_GeneratorFailedDuringInitialization_Title As String
			Get
				Return VBResources.GetResourceString("WRN_GeneratorFailedDuringInitialization_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_GenericConstraintNotCLSCompliant1 As String
			Get
				Return VBResources.GetResourceString("WRN_GenericConstraintNotCLSCompliant1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_GenericConstraintNotCLSCompliant1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_GenericConstraintNotCLSCompliant1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_IfNoTypeObjectAssumed As String
			Get
				Return VBResources.GetResourceString("WRN_IfNoTypeObjectAssumed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_IfNoTypeObjectAssumed_Title As String
			Get
				Return VBResources.GetResourceString("WRN_IfNoTypeObjectAssumed_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_IfTooManyTypesObjectAssumed As String
			Get
				Return VBResources.GetResourceString("WRN_IfTooManyTypesObjectAssumed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_IfTooManyTypesObjectAssumed_Title As String
			Get
				Return VBResources.GetResourceString("WRN_IfTooManyTypesObjectAssumed_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_IgnoreModuleManifest As String
			Get
				Return VBResources.GetResourceString("WRN_IgnoreModuleManifest", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_IgnoreModuleManifest_Title As String
			Get
				Return VBResources.GetResourceString("WRN_IgnoreModuleManifest_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ImplicitConversion2 As String
			Get
				Return VBResources.GetResourceString("WRN_ImplicitConversion2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ImplicitConversion2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ImplicitConversion2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ImplicitConversionCopyBack As String
			Get
				Return VBResources.GetResourceString("WRN_ImplicitConversionCopyBack", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ImplicitConversionCopyBack_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ImplicitConversionCopyBack_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ImplicitConversionSubst1 As String
			Get
				Return VBResources.GetResourceString("WRN_ImplicitConversionSubst1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ImplicitConversionSubst1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ImplicitConversionSubst1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_IndirectRefToLinkedAssembly2 As String
			Get
				Return VBResources.GetResourceString("WRN_IndirectRefToLinkedAssembly2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_IndirectRefToLinkedAssembly2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_IndirectRefToLinkedAssembly2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InheritedInterfaceNotCLSCompliant2 As String
			Get
				Return VBResources.GetResourceString("WRN_InheritedInterfaceNotCLSCompliant2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InheritedInterfaceNotCLSCompliant2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_InheritedInterfaceNotCLSCompliant2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InterfaceConversion2 As String
			Get
				Return VBResources.GetResourceString("WRN_InterfaceConversion2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InterfaceConversion2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_InterfaceConversion2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InvalidAssemblyName As String
			Get
				Return VBResources.GetResourceString("WRN_InvalidAssemblyName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InvalidAssemblyName_Title As String
			Get
				Return VBResources.GetResourceString("WRN_InvalidAssemblyName_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InvalidOverrideDueToTupleNames2 As String
			Get
				Return VBResources.GetResourceString("WRN_InvalidOverrideDueToTupleNames2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InvalidOverrideDueToTupleNames2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_InvalidOverrideDueToTupleNames2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InvalidVersionFormat As String
			Get
				Return VBResources.GetResourceString("WRN_InvalidVersionFormat", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InvalidVersionFormat_Title As String
			Get
				Return VBResources.GetResourceString("WRN_InvalidVersionFormat_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InvalidWarningId As String
			Get
				Return VBResources.GetResourceString("WRN_InvalidWarningId", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_InvalidWarningId_Title As String
			Get
				Return VBResources.GetResourceString("WRN_InvalidWarningId_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LambdaNoTypeObjectAssumed As String
			Get
				Return VBResources.GetResourceString("WRN_LambdaNoTypeObjectAssumed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LambdaNoTypeObjectAssumed_Title As String
			Get
				Return VBResources.GetResourceString("WRN_LambdaNoTypeObjectAssumed_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LambdaPassedToRemoveHandler As String
			Get
				Return VBResources.GetResourceString("WRN_LambdaPassedToRemoveHandler", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LambdaPassedToRemoveHandler_Title As String
			Get
				Return VBResources.GetResourceString("WRN_LambdaPassedToRemoveHandler_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LambdaTooManyTypesObjectAssumed As String
			Get
				Return VBResources.GetResourceString("WRN_LambdaTooManyTypesObjectAssumed", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LambdaTooManyTypesObjectAssumed_Title As String
			Get
				Return VBResources.GetResourceString("WRN_LambdaTooManyTypesObjectAssumed_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LateBindingResolution As String
			Get
				Return VBResources.GetResourceString("WRN_LateBindingResolution", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LateBindingResolution_Title As String
			Get
				Return VBResources.GetResourceString("WRN_LateBindingResolution_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LiftControlVariableLambda As String
			Get
				Return VBResources.GetResourceString("WRN_LiftControlVariableLambda", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LiftControlVariableLambda_Title As String
			Get
				Return VBResources.GetResourceString("WRN_LiftControlVariableLambda_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LiftControlVariableQuery As String
			Get
				Return VBResources.GetResourceString("WRN_LiftControlVariableQuery", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_LiftControlVariableQuery_Title As String
			Get
				Return VBResources.GetResourceString("WRN_LiftControlVariableQuery_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MainIgnored As String
			Get
				Return VBResources.GetResourceString("WRN_MainIgnored", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MainIgnored_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MainIgnored_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MemberShadowsSynthMember6 As String
			Get
				Return VBResources.GetResourceString("WRN_MemberShadowsSynthMember6", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MemberShadowsSynthMember6_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MemberShadowsSynthMember6_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MissingAsClauseinFunction As String
			Get
				Return VBResources.GetResourceString("WRN_MissingAsClauseinFunction", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MissingAsClauseinFunction_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MissingAsClauseinFunction_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MissingAsClauseinOperator As String
			Get
				Return VBResources.GetResourceString("WRN_MissingAsClauseinOperator", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MissingAsClauseinOperator_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MissingAsClauseinOperator_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MissingAsClauseinProperty As String
			Get
				Return VBResources.GetResourceString("WRN_MissingAsClauseinProperty", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MissingAsClauseinProperty_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MissingAsClauseinProperty_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MissingAsClauseinVarDecl As String
			Get
				Return VBResources.GetResourceString("WRN_MissingAsClauseinVarDecl", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MissingAsClauseinVarDecl_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MissingAsClauseinVarDecl_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MultipleDeclFileExtChecksum As String
			Get
				Return VBResources.GetResourceString("WRN_MultipleDeclFileExtChecksum", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MultipleDeclFileExtChecksum_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MultipleDeclFileExtChecksum_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MustOverloadBase4 As String
			Get
				Return VBResources.GetResourceString("WRN_MustOverloadBase4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MustOverloadBase4_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MustOverloadBase4_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MustOverride2 As String
			Get
				Return VBResources.GetResourceString("WRN_MustOverride2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MustOverride2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MustOverride2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MustShadowOnMultipleInheritance2 As String
			Get
				Return VBResources.GetResourceString("WRN_MustShadowOnMultipleInheritance2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MustShadowOnMultipleInheritance2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MustShadowOnMultipleInheritance2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MutableGenericStructureInUsing As String
			Get
				Return VBResources.GetResourceString("WRN_MutableGenericStructureInUsing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MutableGenericStructureInUsing_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MutableGenericStructureInUsing_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MutableStructureInUsing As String
			Get
				Return VBResources.GetResourceString("WRN_MutableStructureInUsing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_MutableStructureInUsing_Title As String
			Get
				Return VBResources.GetResourceString("WRN_MutableStructureInUsing_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NameNotCLSCompliant1 As String
			Get
				Return VBResources.GetResourceString("WRN_NameNotCLSCompliant1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NameNotCLSCompliant1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_NameNotCLSCompliant1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NamespaceCaseMismatch3 As String
			Get
				Return VBResources.GetResourceString("WRN_NamespaceCaseMismatch3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NamespaceCaseMismatch3_Title As String
			Get
				Return VBResources.GetResourceString("WRN_NamespaceCaseMismatch3_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NoAnalyzerInAssembly As String
			Get
				Return VBResources.GetResourceString("WRN_NoAnalyzerInAssembly", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NoAnalyzerInAssembly_Title As String
			Get
				Return VBResources.GetResourceString("WRN_NoAnalyzerInAssembly_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NoConfigInResponseFile As String
			Get
				Return VBResources.GetResourceString("WRN_NoConfigInResponseFile", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NoConfigInResponseFile_Title As String
			Get
				Return VBResources.GetResourceString("WRN_NoConfigInResponseFile_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NonCLSMemberInCLSInterface1 As String
			Get
				Return VBResources.GetResourceString("WRN_NonCLSMemberInCLSInterface1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NonCLSMemberInCLSInterface1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_NonCLSMemberInCLSInterface1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NonCLSMustOverrideInCLSType1 As String
			Get
				Return VBResources.GetResourceString("WRN_NonCLSMustOverrideInCLSType1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NonCLSMustOverrideInCLSType1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_NonCLSMustOverrideInCLSType1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NoNonObsoleteConstructorOnBase3 As String
			Get
				Return VBResources.GetResourceString("WRN_NoNonObsoleteConstructorOnBase3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NoNonObsoleteConstructorOnBase3_Title As String
			Get
				Return VBResources.GetResourceString("WRN_NoNonObsoleteConstructorOnBase3_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NoNonObsoleteConstructorOnBase4 As String
			Get
				Return VBResources.GetResourceString("WRN_NoNonObsoleteConstructorOnBase4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NoNonObsoleteConstructorOnBase4_Title As String
			Get
				Return VBResources.GetResourceString("WRN_NoNonObsoleteConstructorOnBase4_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NotEqualToLiteralNothing As String
			Get
				Return VBResources.GetResourceString("WRN_NotEqualToLiteralNothing", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_NotEqualToLiteralNothing_Title As String
			Get
				Return VBResources.GetResourceString("WRN_NotEqualToLiteralNothing_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectAssumed1 As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectAssumed1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectAssumed1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectAssumed1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectAssumedProperty1 As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectAssumedProperty1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectAssumedProperty1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectAssumedProperty1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectAssumedVar1 As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectAssumedVar1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectAssumedVar1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectAssumedVar1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectMath1 As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectMath1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectMath1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectMath1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectMath1Not As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectMath1Not", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectMath1Not_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectMath1Not_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectMath2 As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectMath2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectMath2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectMath2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectMathSelectCase As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectMathSelectCase", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObjectMathSelectCase_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ObjectMathSelectCase_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObsoleteIdentityDirectCastForValueType As String
			Get
				Return VBResources.GetResourceString("WRN_ObsoleteIdentityDirectCastForValueType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ObsoleteIdentityDirectCastForValueType_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ObsoleteIdentityDirectCastForValueType_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_OptionalValueNotCLSCompliant1 As String
			Get
				Return VBResources.GetResourceString("WRN_OptionalValueNotCLSCompliant1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_OptionalValueNotCLSCompliant1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_OptionalValueNotCLSCompliant1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_OverlappingCatch As String
			Get
				Return VBResources.GetResourceString("WRN_OverlappingCatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_OverlappingCatch_Title As String
			Get
				Return VBResources.GetResourceString("WRN_OverlappingCatch_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_OverrideType5 As String
			Get
				Return VBResources.GetResourceString("WRN_OverrideType5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_OverrideType5_Title As String
			Get
				Return VBResources.GetResourceString("WRN_OverrideType5_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ParamNotCLSCompliant1 As String
			Get
				Return VBResources.GetResourceString("WRN_ParamNotCLSCompliant1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ParamNotCLSCompliant1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ParamNotCLSCompliant1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_PdbLocalNameTooLong As String
			Get
				Return VBResources.GetResourceString("WRN_PdbLocalNameTooLong", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_PdbLocalNameTooLong_Title As String
			Get
				Return VBResources.GetResourceString("WRN_PdbLocalNameTooLong_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_PdbUsingNameTooLong As String
			Get
				Return VBResources.GetResourceString("WRN_PdbUsingNameTooLong", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_PdbUsingNameTooLong_Title As String
			Get
				Return VBResources.GetResourceString("WRN_PdbUsingNameTooLong_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_PrefixAndXmlnsLocalName As String
			Get
				Return VBResources.GetResourceString("WRN_PrefixAndXmlnsLocalName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_PrefixAndXmlnsLocalName_Title As String
			Get
				Return VBResources.GetResourceString("WRN_PrefixAndXmlnsLocalName_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ProcTypeNotCLSCompliant1 As String
			Get
				Return VBResources.GetResourceString("WRN_ProcTypeNotCLSCompliant1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ProcTypeNotCLSCompliant1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ProcTypeNotCLSCompliant1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_QueryMissingAsClauseinVarDecl As String
			Get
				Return VBResources.GetResourceString("WRN_QueryMissingAsClauseinVarDecl", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_QueryMissingAsClauseinVarDecl_Title As String
			Get
				Return VBResources.GetResourceString("WRN_QueryMissingAsClauseinVarDecl_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RecursiveAddHandlerCall As String
			Get
				Return VBResources.GetResourceString("WRN_RecursiveAddHandlerCall", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RecursiveAddHandlerCall_Title As String
			Get
				Return VBResources.GetResourceString("WRN_RecursiveAddHandlerCall_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RecursiveOperatorCall As String
			Get
				Return VBResources.GetResourceString("WRN_RecursiveOperatorCall", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RecursiveOperatorCall_Title As String
			Get
				Return VBResources.GetResourceString("WRN_RecursiveOperatorCall_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RecursivePropertyCall As String
			Get
				Return VBResources.GetResourceString("WRN_RecursivePropertyCall", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RecursivePropertyCall_Title As String
			Get
				Return VBResources.GetResourceString("WRN_RecursivePropertyCall_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RefCultureMismatch As String
			Get
				Return VBResources.GetResourceString("WRN_RefCultureMismatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RefCultureMismatch_Title As String
			Get
				Return VBResources.GetResourceString("WRN_RefCultureMismatch_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ReferencedAssemblyDoesNotHaveStrongName As String
			Get
				Return VBResources.GetResourceString("WRN_ReferencedAssemblyDoesNotHaveStrongName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ReferencedAssemblyDoesNotHaveStrongName_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ReferencedAssemblyDoesNotHaveStrongName_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RelDelegatePassedToRemoveHandler As String
			Get
				Return VBResources.GetResourceString("WRN_RelDelegatePassedToRemoveHandler", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RelDelegatePassedToRemoveHandler_Title As String
			Get
				Return VBResources.GetResourceString("WRN_RelDelegatePassedToRemoveHandler_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RequiredNonObsoleteNewCall3 As String
			Get
				Return VBResources.GetResourceString("WRN_RequiredNonObsoleteNewCall3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RequiredNonObsoleteNewCall3_Title As String
			Get
				Return VBResources.GetResourceString("WRN_RequiredNonObsoleteNewCall3_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RequiredNonObsoleteNewCall4 As String
			Get
				Return VBResources.GetResourceString("WRN_RequiredNonObsoleteNewCall4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RequiredNonObsoleteNewCall4_Title As String
			Get
				Return VBResources.GetResourceString("WRN_RequiredNonObsoleteNewCall4_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ReturnTypeAttributeOnWriteOnlyProperty As String
			Get
				Return VBResources.GetResourceString("WRN_ReturnTypeAttributeOnWriteOnlyProperty", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ReturnTypeAttributeOnWriteOnlyProperty_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ReturnTypeAttributeOnWriteOnlyProperty_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RootNamespaceNotCLSCompliant1 As String
			Get
				Return VBResources.GetResourceString("WRN_RootNamespaceNotCLSCompliant1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RootNamespaceNotCLSCompliant1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_RootNamespaceNotCLSCompliant1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RootNamespaceNotCLSCompliant2 As String
			Get
				Return VBResources.GetResourceString("WRN_RootNamespaceNotCLSCompliant2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_RootNamespaceNotCLSCompliant2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_RootNamespaceNotCLSCompliant2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_SelectCaseInvalidRange As String
			Get
				Return VBResources.GetResourceString("WRN_SelectCaseInvalidRange", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_SelectCaseInvalidRange_Title As String
			Get
				Return VBResources.GetResourceString("WRN_SelectCaseInvalidRange_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ShadowingGenericParamWithParam1 As String
			Get
				Return VBResources.GetResourceString("WRN_ShadowingGenericParamWithParam1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_ShadowingGenericParamWithParam1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_ShadowingGenericParamWithParam1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_SharedMemberThroughInstance As String
			Get
				Return VBResources.GetResourceString("WRN_SharedMemberThroughInstance", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_SharedMemberThroughInstance_Title As String
			Get
				Return VBResources.GetResourceString("WRN_SharedMemberThroughInstance_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_StaticLocalNoInference As String
			Get
				Return VBResources.GetResourceString("WRN_StaticLocalNoInference", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_StaticLocalNoInference_Title As String
			Get
				Return VBResources.GetResourceString("WRN_StaticLocalNoInference_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_SynthMemberShadowsMember5 As String
			Get
				Return VBResources.GetResourceString("WRN_SynthMemberShadowsMember5", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_SynthMemberShadowsMember5_Title As String
			Get
				Return VBResources.GetResourceString("WRN_SynthMemberShadowsMember5_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_SynthMemberShadowsSynthMember7 As String
			Get
				Return VBResources.GetResourceString("WRN_SynthMemberShadowsSynthMember7", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_SynthMemberShadowsSynthMember7_Title As String
			Get
				Return VBResources.GetResourceString("WRN_SynthMemberShadowsSynthMember7_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_TupleLiteralNameMismatch As String
			Get
				Return VBResources.GetResourceString("WRN_TupleLiteralNameMismatch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_TupleLiteralNameMismatch_Title As String
			Get
				Return VBResources.GetResourceString("WRN_TupleLiteralNameMismatch_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_TypeConflictButMerged6 As String
			Get
				Return VBResources.GetResourceString("WRN_TypeConflictButMerged6", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_TypeConflictButMerged6_Title As String
			Get
				Return VBResources.GetResourceString("WRN_TypeConflictButMerged6_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_TypeInferenceAssumed3 As String
			Get
				Return VBResources.GetResourceString("WRN_TypeInferenceAssumed3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_TypeInferenceAssumed3_Title As String
			Get
				Return VBResources.GetResourceString("WRN_TypeInferenceAssumed3_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_TypeNotCLSCompliant1 As String
			Get
				Return VBResources.GetResourceString("WRN_TypeNotCLSCompliant1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_TypeNotCLSCompliant1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_TypeNotCLSCompliant1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnableToLoadAnalyzer As String
			Get
				Return VBResources.GetResourceString("WRN_UnableToLoadAnalyzer", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnableToLoadAnalyzer_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UnableToLoadAnalyzer_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UndefinedOrEmptyNamespaceOrClass1 As String
			Get
				Return VBResources.GetResourceString("WRN_UndefinedOrEmptyNamespaceOrClass1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UndefinedOrEmptyNamespaceOrClass1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UndefinedOrEmptyNamespaceOrClass1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UndefinedOrEmptyProjectNamespaceOrClass1 As String
			Get
				Return VBResources.GetResourceString("WRN_UndefinedOrEmptyProjectNamespaceOrClass1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UndefinedOrEmptyProjectNamespaceOrClass1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UndefinedOrEmptyProjectNamespaceOrClass1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnimplementedCommandLineSwitch As String
			Get
				Return VBResources.GetResourceString("WRN_UnimplementedCommandLineSwitch", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnimplementedCommandLineSwitch_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UnimplementedCommandLineSwitch_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnobservedAwaitableDelegate As String
			Get
				Return VBResources.GetResourceString("WRN_UnobservedAwaitableDelegate", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnobservedAwaitableDelegate_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UnobservedAwaitableDelegate_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnobservedAwaitableExpression As String
			Get
				Return VBResources.GetResourceString("WRN_UnobservedAwaitableExpression", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnobservedAwaitableExpression_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UnobservedAwaitableExpression_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnreachableCode As String
			Get
				Return VBResources.GetResourceString("WRN_UnreachableCode", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnreachableCode_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UnreachableCode_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnusedLocal As String
			Get
				Return VBResources.GetResourceString("WRN_UnusedLocal", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnusedLocal_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UnusedLocal_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnusedLocalConst As String
			Get
				Return VBResources.GetResourceString("WRN_UnusedLocalConst", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UnusedLocalConst_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UnusedLocalConst_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseOfObsoletePropertyAccessor2 As String
			Get
				Return VBResources.GetResourceString("WRN_UseOfObsoletePropertyAccessor2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseOfObsoletePropertyAccessor2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UseOfObsoletePropertyAccessor2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseOfObsoletePropertyAccessor3 As String
			Get
				Return VBResources.GetResourceString("WRN_UseOfObsoletePropertyAccessor3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseOfObsoletePropertyAccessor3_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UseOfObsoletePropertyAccessor3_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseOfObsoleteSymbol2 As String
			Get
				Return VBResources.GetResourceString("WRN_UseOfObsoleteSymbol2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseOfObsoleteSymbol2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UseOfObsoleteSymbol2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseOfObsoleteSymbolNoMessage1 As String
			Get
				Return VBResources.GetResourceString("WRN_UseOfObsoleteSymbolNoMessage1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseOfObsoleteSymbolNoMessage1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UseOfObsoleteSymbolNoMessage1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseSwitchInsteadOfAttribute As String
			Get
				Return VBResources.GetResourceString("WRN_UseSwitchInsteadOfAttribute", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseSwitchInsteadOfAttribute_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UseSwitchInsteadOfAttribute_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseValueForXmlExpression3 As String
			Get
				Return VBResources.GetResourceString("WRN_UseValueForXmlExpression3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_UseValueForXmlExpression3_Title As String
			Get
				Return VBResources.GetResourceString("WRN_UseValueForXmlExpression3_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceConversionFailedIn6 As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceConversionFailedIn6", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceConversionFailedIn6_Title As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceConversionFailedIn6_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceConversionFailedOut6 As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceConversionFailedOut6", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceConversionFailedOut6_Title As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceConversionFailedOut6_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceConversionFailedTryIn4 As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceConversionFailedTryIn4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceConversionFailedTryIn4_Title As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceConversionFailedTryIn4_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceConversionFailedTryOut4 As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceConversionFailedTryOut4", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceConversionFailedTryOut4_Title As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceConversionFailedTryOut4_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceDeclarationAmbiguous3 As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceDeclarationAmbiguous3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceDeclarationAmbiguous3_Title As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceDeclarationAmbiguous3_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceIEnumerableSuggestion3 As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceIEnumerableSuggestion3", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_VarianceIEnumerableSuggestion3_Title As String
			Get
				Return VBResources.GetResourceString("WRN_VarianceIEnumerableSuggestion3_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLCannotWriteToXMLDocFile2 As String
			Get
				Return VBResources.GetResourceString("WRN_XMLCannotWriteToXMLDocFile2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLCannotWriteToXMLDocFile2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLCannotWriteToXMLDocFile2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocBadFormedXML As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocBadFormedXML", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocBadFormedXML_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocBadFormedXML_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocBadGenericParamTag2 As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocBadGenericParamTag2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocBadGenericParamTag2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocBadGenericParamTag2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocBadParamTag2 As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocBadParamTag2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocBadParamTag2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocBadParamTag2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocBadXMLLine As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocBadXMLLine", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocBadXMLLine_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocBadXMLLine_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocCrefAttributeNotFound1 As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocCrefAttributeNotFound1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocCrefAttributeNotFound1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocCrefAttributeNotFound1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocCrefToTypeParameter As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocCrefToTypeParameter", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocCrefToTypeParameter_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocCrefToTypeParameter_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocDuplicateXMLNode1 As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocDuplicateXMLNode1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocDuplicateXMLNode1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocDuplicateXMLNode1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocExceptionTagWithoutCRef As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocExceptionTagWithoutCRef", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocExceptionTagWithoutCRef_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocExceptionTagWithoutCRef_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocGenericParamTagWithoutName As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocGenericParamTagWithoutName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocGenericParamTagWithoutName_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocGenericParamTagWithoutName_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocIllegalTagOnElement2 As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocIllegalTagOnElement2", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocIllegalTagOnElement2_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocIllegalTagOnElement2_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocInsideMethod As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocInsideMethod", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocInsideMethod_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocInsideMethod_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocInvalidXMLFragment As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocInvalidXMLFragment", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocInvalidXMLFragment_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocInvalidXMLFragment_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocMoreThanOneCommentBlock As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocMoreThanOneCommentBlock", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocMoreThanOneCommentBlock_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocMoreThanOneCommentBlock_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocNotFirstOnLine As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocNotFirstOnLine", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocNotFirstOnLine_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocNotFirstOnLine_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocOnAPartialType As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocOnAPartialType", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocOnAPartialType_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocOnAPartialType_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocParamTagWithoutName As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocParamTagWithoutName", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocParamTagWithoutName_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocParamTagWithoutName_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocParseError1 As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocParseError1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocParseError1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocParseError1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocReturnsOnADeclareSub As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocReturnsOnADeclareSub", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocReturnsOnADeclareSub_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocReturnsOnADeclareSub_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocReturnsOnWriteOnlyProperty As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocReturnsOnWriteOnlyProperty", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocReturnsOnWriteOnlyProperty_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocReturnsOnWriteOnlyProperty_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocStartTagWithNoEndTag As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocStartTagWithNoEndTag", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocStartTagWithNoEndTag_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocStartTagWithNoEndTag_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocWithoutLanguageElement As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocWithoutLanguageElement", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLDocWithoutLanguageElement_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLDocWithoutLanguageElement_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLMissingFileOrPathAttribute1 As String
			Get
				Return VBResources.GetResourceString("WRN_XMLMissingFileOrPathAttribute1", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WRN_XMLMissingFileOrPathAttribute1_Title As String
			Get
				Return VBResources.GetResourceString("WRN_XMLMissingFileOrPathAttribute1_Title", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WrongNumberOfTypeArguments As String
			Get
				Return VBResources.GetResourceString("WrongNumberOfTypeArguments", Nothing)
			End Get
		End Property

		Friend ReadOnly Shared Property WrongSemanticModelType As String
			Get
				Return VBResources.GetResourceString("WrongSemanticModelType", Nothing)
			End Get
		End Property

		Private Sub New()
			MyBase.New()
		End Sub

		<MethodImpl(256)>
		Friend Shared Function GetResourceString(ByVal resourceKey As String, Optional ByVal defaultValue As String = Nothing) As String
			Return VBResources.ResourceManager.GetString(resourceKey, VBResources.Culture)
		End Function
	End Class
End Namespace
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class CSharpResources
    {
        private static ResourceManager s_resourceManager;

        internal static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new ResourceManager(typeof(CSharpResources)));

        internal static CultureInfo Culture { get; set; }

        internal static string IDS_NULL => GetResourceString("IDS_NULL");

        internal static string IDS_ThrowExpression => GetResourceString("IDS_ThrowExpression");

        internal static string IDS_FeatureSwitchExpression => GetResourceString("IDS_FeatureSwitchExpression");

        internal static string IDS_FeatureLocalFunctionAttributes => GetResourceString("IDS_FeatureLocalFunctionAttributes");

        internal static string IDS_FeatureExternLocalFunctions => GetResourceString("IDS_FeatureExternLocalFunctions");

        internal static string IDS_RELATEDERROR => GetResourceString("IDS_RELATEDERROR");

        internal static string IDS_RELATEDWARNING => GetResourceString("IDS_RELATEDWARNING");

        internal static string IDS_XMLIGNORED => GetResourceString("IDS_XMLIGNORED");

        internal static string IDS_XMLIGNORED2 => GetResourceString("IDS_XMLIGNORED2");

        internal static string IDS_XMLFAILEDINCLUDE => GetResourceString("IDS_XMLFAILEDINCLUDE");

        internal static string IDS_XMLBADINCLUDE => GetResourceString("IDS_XMLBADINCLUDE");

        internal static string IDS_XMLNOINCLUDE => GetResourceString("IDS_XMLNOINCLUDE");

        internal static string IDS_XMLMISSINGINCLUDEFILE => GetResourceString("IDS_XMLMISSINGINCLUDEFILE");

        internal static string IDS_XMLMISSINGINCLUDEPATH => GetResourceString("IDS_XMLMISSINGINCLUDEPATH");

        internal static string IDS_GlobalNamespace => GetResourceString("IDS_GlobalNamespace");

        internal static string IDS_FeatureGenerics => GetResourceString("IDS_FeatureGenerics");

        internal static string IDS_FeatureAnonDelegates => GetResourceString("IDS_FeatureAnonDelegates");

        internal static string IDS_FeatureModuleAttrLoc => GetResourceString("IDS_FeatureModuleAttrLoc");

        internal static string IDS_FeatureGlobalNamespace => GetResourceString("IDS_FeatureGlobalNamespace");

        internal static string IDS_FeatureFixedBuffer => GetResourceString("IDS_FeatureFixedBuffer");

        internal static string IDS_FeaturePragma => GetResourceString("IDS_FeaturePragma");

        internal static string IDS_FeatureStaticClasses => GetResourceString("IDS_FeatureStaticClasses");

        internal static string IDS_FeatureReadOnlyStructs => GetResourceString("IDS_FeatureReadOnlyStructs");

        internal static string IDS_FeaturePartialTypes => GetResourceString("IDS_FeaturePartialTypes");

        internal static string IDS_FeatureAsync => GetResourceString("IDS_FeatureAsync");

        internal static string IDS_FeatureSwitchOnBool => GetResourceString("IDS_FeatureSwitchOnBool");

        internal static string IDS_MethodGroup => GetResourceString("IDS_MethodGroup");

        internal static string IDS_AnonMethod => GetResourceString("IDS_AnonMethod");

        internal static string IDS_Lambda => GetResourceString("IDS_Lambda");

        internal static string IDS_Collection => GetResourceString("IDS_Collection");

        internal static string IDS_Disposable => GetResourceString("IDS_Disposable");

        internal static string IDS_FeaturePropertyAccessorMods => GetResourceString("IDS_FeaturePropertyAccessorMods");

        internal static string IDS_FeatureExternAlias => GetResourceString("IDS_FeatureExternAlias");

        internal static string IDS_FeatureIterators => GetResourceString("IDS_FeatureIterators");

        internal static string IDS_FeatureDefault => GetResourceString("IDS_FeatureDefault");

        internal static string IDS_FeatureAsyncStreams => GetResourceString("IDS_FeatureAsyncStreams");

        internal static string IDS_FeatureUnmanagedConstructedTypes => GetResourceString("IDS_FeatureUnmanagedConstructedTypes");

        internal static string IDS_FeatureReadOnlyMembers => GetResourceString("IDS_FeatureReadOnlyMembers");

        internal static string IDS_FeatureDefaultLiteral => GetResourceString("IDS_FeatureDefaultLiteral");

        internal static string IDS_FeaturePrivateProtected => GetResourceString("IDS_FeaturePrivateProtected");

        internal static string IDS_FeatureTupleEquality => GetResourceString("IDS_FeatureTupleEquality");

        internal static string IDS_FeatureNullable => GetResourceString("IDS_FeatureNullable");

        internal static string IDS_FeaturePatternMatching => GetResourceString("IDS_FeaturePatternMatching");

        internal static string IDS_FeatureExpressionBodiedAccessor => GetResourceString("IDS_FeatureExpressionBodiedAccessor");

        internal static string IDS_FeatureExpressionBodiedDeOrConstructor => GetResourceString("IDS_FeatureExpressionBodiedDeOrConstructor");

        internal static string IDS_FeatureThrowExpression => GetResourceString("IDS_FeatureThrowExpression");

        internal static string IDS_FeatureImplicitArray => GetResourceString("IDS_FeatureImplicitArray");

        internal static string IDS_FeatureImplicitLocal => GetResourceString("IDS_FeatureImplicitLocal");

        internal static string IDS_FeatureAnonymousTypes => GetResourceString("IDS_FeatureAnonymousTypes");

        internal static string IDS_FeatureAutoImplementedProperties => GetResourceString("IDS_FeatureAutoImplementedProperties");

        internal static string IDS_FeatureReadonlyAutoImplementedProperties => GetResourceString("IDS_FeatureReadonlyAutoImplementedProperties");

        internal static string IDS_FeatureObjectInitializer => GetResourceString("IDS_FeatureObjectInitializer");

        internal static string IDS_FeatureCollectionInitializer => GetResourceString("IDS_FeatureCollectionInitializer");

        internal static string IDS_FeatureQueryExpression => GetResourceString("IDS_FeatureQueryExpression");

        internal static string IDS_FeatureExtensionMethod => GetResourceString("IDS_FeatureExtensionMethod");

        internal static string IDS_FeaturePartialMethod => GetResourceString("IDS_FeaturePartialMethod");

        internal static string IDS_SK_METHOD => GetResourceString("IDS_SK_METHOD");

        internal static string IDS_SK_TYPE => GetResourceString("IDS_SK_TYPE");

        internal static string IDS_SK_NAMESPACE => GetResourceString("IDS_SK_NAMESPACE");

        internal static string IDS_SK_FIELD => GetResourceString("IDS_SK_FIELD");

        internal static string IDS_SK_PROPERTY => GetResourceString("IDS_SK_PROPERTY");

        internal static string IDS_SK_UNKNOWN => GetResourceString("IDS_SK_UNKNOWN");

        internal static string IDS_SK_VARIABLE => GetResourceString("IDS_SK_VARIABLE");

        internal static string IDS_SK_LABEL => GetResourceString("IDS_SK_LABEL");

        internal static string IDS_SK_EVENT => GetResourceString("IDS_SK_EVENT");

        internal static string IDS_SK_TYVAR => GetResourceString("IDS_SK_TYVAR");

        internal static string IDS_SK_ALIAS => GetResourceString("IDS_SK_ALIAS");

        internal static string IDS_SK_EXTERNALIAS => GetResourceString("IDS_SK_EXTERNALIAS");

        internal static string IDS_SK_CONSTRUCTOR => GetResourceString("IDS_SK_CONSTRUCTOR");

        internal static string IDS_FOREACHLOCAL => GetResourceString("IDS_FOREACHLOCAL");

        internal static string IDS_FIXEDLOCAL => GetResourceString("IDS_FIXEDLOCAL");

        internal static string IDS_USINGLOCAL => GetResourceString("IDS_USINGLOCAL");

        internal static string IDS_Contravariant => GetResourceString("IDS_Contravariant");

        internal static string IDS_Contravariantly => GetResourceString("IDS_Contravariantly");

        internal static string IDS_Covariant => GetResourceString("IDS_Covariant");

        internal static string IDS_Covariantly => GetResourceString("IDS_Covariantly");

        internal static string IDS_Invariantly => GetResourceString("IDS_Invariantly");

        internal static string IDS_FeatureDynamic => GetResourceString("IDS_FeatureDynamic");

        internal static string IDS_FeatureNamedArgument => GetResourceString("IDS_FeatureNamedArgument");

        internal static string IDS_FeatureOptionalParameter => GetResourceString("IDS_FeatureOptionalParameter");

        internal static string IDS_FeatureExceptionFilter => GetResourceString("IDS_FeatureExceptionFilter");

        internal static string IDS_FeatureTypeVariance => GetResourceString("IDS_FeatureTypeVariance");

        internal static string IDS_Parameter => GetResourceString("IDS_Parameter");

        internal static string IDS_Return => GetResourceString("IDS_Return");

        internal static string XML_InvalidToken => GetResourceString("XML_InvalidToken");

        internal static string XML_IncorrectComment => GetResourceString("XML_IncorrectComment");

        internal static string XML_InvalidCharEntity => GetResourceString("XML_InvalidCharEntity");

        internal static string XML_ExpectedEndOfTag => GetResourceString("XML_ExpectedEndOfTag");

        internal static string XML_ExpectedIdentifier => GetResourceString("XML_ExpectedIdentifier");

        internal static string XML_InvalidUnicodeChar => GetResourceString("XML_InvalidUnicodeChar");

        internal static string XML_InvalidWhitespace => GetResourceString("XML_InvalidWhitespace");

        internal static string XML_LessThanInAttributeValue => GetResourceString("XML_LessThanInAttributeValue");

        internal static string XML_MissingEqualsAttribute => GetResourceString("XML_MissingEqualsAttribute");

        internal static string XML_RefUndefinedEntity_1 => GetResourceString("XML_RefUndefinedEntity_1");

        internal static string XML_StringLiteralNoStartQuote => GetResourceString("XML_StringLiteralNoStartQuote");

        internal static string XML_StringLiteralNoEndQuote => GetResourceString("XML_StringLiteralNoEndQuote");

        internal static string XML_StringLiteralNonAsciiQuote => GetResourceString("XML_StringLiteralNonAsciiQuote");

        internal static string XML_EndTagNotExpected => GetResourceString("XML_EndTagNotExpected");

        internal static string XML_ElementTypeMatch => GetResourceString("XML_ElementTypeMatch");

        internal static string XML_EndTagExpected => GetResourceString("XML_EndTagExpected");

        internal static string XML_WhitespaceMissing => GetResourceString("XML_WhitespaceMissing");

        internal static string XML_ExpectedEndOfXml => GetResourceString("XML_ExpectedEndOfXml");

        internal static string XML_CDataEndTagNotAllowed => GetResourceString("XML_CDataEndTagNotAllowed");

        internal static string XML_DuplicateAttribute => GetResourceString("XML_DuplicateAttribute");

        internal static string ERR_NoMetadataFile => GetResourceString("ERR_NoMetadataFile");

        internal static string ERR_MetadataReferencesNotSupported => GetResourceString("ERR_MetadataReferencesNotSupported");

        internal static string FTL_MetadataCantOpenFile => GetResourceString("FTL_MetadataCantOpenFile");

        internal static string ERR_NoTypeDef => GetResourceString("ERR_NoTypeDef");

        internal static string ERR_NoTypeDefFromModule => GetResourceString("ERR_NoTypeDefFromModule");

        internal static string ERR_OutputWriteFailed => GetResourceString("ERR_OutputWriteFailed");

        internal static string ERR_MultipleEntryPoints => GetResourceString("ERR_MultipleEntryPoints");

        internal static string ERR_BadBinaryOps => GetResourceString("ERR_BadBinaryOps");

        internal static string ERR_AmbigBinaryOpsOnUnconstrainedDefault => GetResourceString("ERR_AmbigBinaryOpsOnUnconstrainedDefault");

        internal static string ERR_IntDivByZero => GetResourceString("ERR_IntDivByZero");

        internal static string ERR_BadIndexLHS => GetResourceString("ERR_BadIndexLHS");

        internal static string ERR_BadIndexCount => GetResourceString("ERR_BadIndexCount");

        internal static string ERR_BadUnaryOp => GetResourceString("ERR_BadUnaryOp");

        internal static string ERR_BadOpOnNullOrDefaultOrNew => GetResourceString("ERR_BadOpOnNullOrDefaultOrNew");

        internal static string ERR_ThisInStaticMeth => GetResourceString("ERR_ThisInStaticMeth");

        internal static string ERR_ThisInBadContext => GetResourceString("ERR_ThisInBadContext");

        internal static string ERR_OmittedTypeArgument => GetResourceString("ERR_OmittedTypeArgument");

        internal static string WRN_InvalidMainSig => GetResourceString("WRN_InvalidMainSig");

        internal static string WRN_InvalidMainSig_Title => GetResourceString("WRN_InvalidMainSig_Title");

        internal static string ERR_NoImplicitConv => GetResourceString("ERR_NoImplicitConv");

        internal static string ERR_NoExplicitConv => GetResourceString("ERR_NoExplicitConv");

        internal static string ERR_ConstOutOfRange => GetResourceString("ERR_ConstOutOfRange");

        internal static string ERR_AmbigBinaryOps => GetResourceString("ERR_AmbigBinaryOps");

        internal static string ERR_AmbigBinaryOpsOnDefault => GetResourceString("ERR_AmbigBinaryOpsOnDefault");

        internal static string ERR_AmbigUnaryOp => GetResourceString("ERR_AmbigUnaryOp");

        internal static string ERR_InAttrOnOutParam => GetResourceString("ERR_InAttrOnOutParam");

        internal static string ERR_ValueCantBeNull => GetResourceString("ERR_ValueCantBeNull");

        internal static string ERR_NoExplicitBuiltinConv => GetResourceString("ERR_NoExplicitBuiltinConv");

        internal static string FTL_DebugEmitFailure => GetResourceString("FTL_DebugEmitFailure");

        internal static string ERR_BadVisReturnType => GetResourceString("ERR_BadVisReturnType");

        internal static string ERR_BadVisParamType => GetResourceString("ERR_BadVisParamType");

        internal static string ERR_BadVisFieldType => GetResourceString("ERR_BadVisFieldType");

        internal static string ERR_BadVisPropertyType => GetResourceString("ERR_BadVisPropertyType");

        internal static string ERR_BadVisIndexerReturn => GetResourceString("ERR_BadVisIndexerReturn");

        internal static string ERR_BadVisIndexerParam => GetResourceString("ERR_BadVisIndexerParam");

        internal static string ERR_BadVisOpReturn => GetResourceString("ERR_BadVisOpReturn");

        internal static string ERR_BadVisOpParam => GetResourceString("ERR_BadVisOpParam");

        internal static string ERR_BadVisDelegateReturn => GetResourceString("ERR_BadVisDelegateReturn");

        internal static string ERR_BadVisDelegateParam => GetResourceString("ERR_BadVisDelegateParam");

        internal static string ERR_BadVisBaseClass => GetResourceString("ERR_BadVisBaseClass");

        internal static string ERR_BadVisBaseInterface => GetResourceString("ERR_BadVisBaseInterface");

        internal static string ERR_EventNeedsBothAccessors => GetResourceString("ERR_EventNeedsBothAccessors");

        internal static string ERR_AbstractEventHasAccessors => GetResourceString("ERR_AbstractEventHasAccessors");

        internal static string ERR_EventNotDelegate => GetResourceString("ERR_EventNotDelegate");

        internal static string WRN_UnreferencedEvent => GetResourceString("WRN_UnreferencedEvent");

        internal static string WRN_UnreferencedEvent_Title => GetResourceString("WRN_UnreferencedEvent_Title");

        internal static string ERR_InterfaceEventInitializer => GetResourceString("ERR_InterfaceEventInitializer");

        internal static string ERR_BadEventUsage => GetResourceString("ERR_BadEventUsage");

        internal static string ERR_ExplicitEventFieldImpl => GetResourceString("ERR_ExplicitEventFieldImpl");

        internal static string ERR_CantOverrideNonEvent => GetResourceString("ERR_CantOverrideNonEvent");

        internal static string ERR_AddRemoveMustHaveBody => GetResourceString("ERR_AddRemoveMustHaveBody");

        internal static string ERR_AbstractEventInitializer => GetResourceString("ERR_AbstractEventInitializer");

        internal static string ERR_ReservedAssemblyName => GetResourceString("ERR_ReservedAssemblyName");

        internal static string ERR_ReservedEnumerator => GetResourceString("ERR_ReservedEnumerator");

        internal static string ERR_AsMustHaveReferenceType => GetResourceString("ERR_AsMustHaveReferenceType");

        internal static string WRN_LowercaseEllSuffix => GetResourceString("WRN_LowercaseEllSuffix");

        internal static string WRN_LowercaseEllSuffix_Title => GetResourceString("WRN_LowercaseEllSuffix_Title");

        internal static string ERR_BadEventUsageNoField => GetResourceString("ERR_BadEventUsageNoField");

        internal static string ERR_ConstraintOnlyAllowedOnGenericDecl => GetResourceString("ERR_ConstraintOnlyAllowedOnGenericDecl");

        internal static string ERR_TypeParamMustBeIdentifier => GetResourceString("ERR_TypeParamMustBeIdentifier");

        internal static string ERR_MemberReserved => GetResourceString("ERR_MemberReserved");

        internal static string ERR_DuplicateParamName => GetResourceString("ERR_DuplicateParamName");

        internal static string ERR_DuplicateNameInNS => GetResourceString("ERR_DuplicateNameInNS");

        internal static string ERR_DuplicateNameInClass => GetResourceString("ERR_DuplicateNameInClass");

        internal static string ERR_NameNotInContext => GetResourceString("ERR_NameNotInContext");

        internal static string ERR_NameNotInContextPossibleMissingReference => GetResourceString("ERR_NameNotInContextPossibleMissingReference");

        internal static string ERR_AmbigContext => GetResourceString("ERR_AmbigContext");

        internal static string WRN_DuplicateUsing => GetResourceString("WRN_DuplicateUsing");

        internal static string WRN_DuplicateUsing_Title => GetResourceString("WRN_DuplicateUsing_Title");

        internal static string ERR_BadMemberFlag => GetResourceString("ERR_BadMemberFlag");

        internal static string ERR_BadInitAccessor => GetResourceString("ERR_BadInitAccessor");

        internal static string ERR_BadMemberProtection => GetResourceString("ERR_BadMemberProtection");

        internal static string WRN_NewRequired => GetResourceString("WRN_NewRequired");

        internal static string WRN_NewRequired_Title => GetResourceString("WRN_NewRequired_Title");

        internal static string WRN_NewRequired_Description => GetResourceString("WRN_NewRequired_Description");

        internal static string WRN_NewNotRequired => GetResourceString("WRN_NewNotRequired");

        internal static string WRN_NewNotRequired_Title => GetResourceString("WRN_NewNotRequired_Title");

        internal static string ERR_CircConstValue => GetResourceString("ERR_CircConstValue");

        internal static string ERR_MemberAlreadyExists => GetResourceString("ERR_MemberAlreadyExists");

        internal static string ERR_StaticNotVirtual => GetResourceString("ERR_StaticNotVirtual");

        internal static string ERR_OverrideNotNew => GetResourceString("ERR_OverrideNotNew");

        internal static string WRN_NewOrOverrideExpected => GetResourceString("WRN_NewOrOverrideExpected");

        internal static string WRN_NewOrOverrideExpected_Title => GetResourceString("WRN_NewOrOverrideExpected_Title");

        internal static string ERR_OverrideNotExpected => GetResourceString("ERR_OverrideNotExpected");

        internal static string ERR_NamespaceUnexpected => GetResourceString("ERR_NamespaceUnexpected");

        internal static string ERR_NoSuchMember => GetResourceString("ERR_NoSuchMember");

        internal static string ERR_BadSKknown => GetResourceString("ERR_BadSKknown");

        internal static string ERR_BadSKunknown => GetResourceString("ERR_BadSKunknown");

        internal static string ERR_ObjectRequired => GetResourceString("ERR_ObjectRequired");

        internal static string ERR_AmbigCall => GetResourceString("ERR_AmbigCall");

        internal static string ERR_BadAccess => GetResourceString("ERR_BadAccess");

        internal static string ERR_MethDelegateMismatch => GetResourceString("ERR_MethDelegateMismatch");

        internal static string ERR_RetObjectRequired => GetResourceString("ERR_RetObjectRequired");

        internal static string ERR_RetNoObjectRequired => GetResourceString("ERR_RetNoObjectRequired");

        internal static string ERR_LocalDuplicate => GetResourceString("ERR_LocalDuplicate");

        internal static string ERR_AssgLvalueExpected => GetResourceString("ERR_AssgLvalueExpected");

        internal static string ERR_StaticConstParam => GetResourceString("ERR_StaticConstParam");

        internal static string ERR_NotConstantExpression => GetResourceString("ERR_NotConstantExpression");

        internal static string ERR_NotNullConstRefField => GetResourceString("ERR_NotNullConstRefField");

        internal static string ERR_LocalIllegallyOverrides => GetResourceString("ERR_LocalIllegallyOverrides");

        internal static string ERR_BadUsingNamespace => GetResourceString("ERR_BadUsingNamespace");

        internal static string ERR_BadUsingType => GetResourceString("ERR_BadUsingType");

        internal static string ERR_NoAliasHere => GetResourceString("ERR_NoAliasHere");

        internal static string ERR_NoBreakOrCont => GetResourceString("ERR_NoBreakOrCont");

        internal static string ERR_DuplicateLabel => GetResourceString("ERR_DuplicateLabel");

        internal static string ERR_NoConstructors => GetResourceString("ERR_NoConstructors");

        internal static string ERR_NoNewAbstract => GetResourceString("ERR_NoNewAbstract");

        internal static string ERR_ConstValueRequired => GetResourceString("ERR_ConstValueRequired");

        internal static string ERR_CircularBase => GetResourceString("ERR_CircularBase");

        internal static string ERR_BadDelegateConstructor => GetResourceString("ERR_BadDelegateConstructor");

        internal static string ERR_MethodNameExpected => GetResourceString("ERR_MethodNameExpected");

        internal static string ERR_ConstantExpected => GetResourceString("ERR_ConstantExpected");

        internal static string ERR_V6SwitchGoverningTypeValueExpected => GetResourceString("ERR_V6SwitchGoverningTypeValueExpected");

        internal static string ERR_IntegralTypeValueExpected => GetResourceString("ERR_IntegralTypeValueExpected");

        internal static string ERR_DuplicateCaseLabel => GetResourceString("ERR_DuplicateCaseLabel");

        internal static string ERR_InvalidGotoCase => GetResourceString("ERR_InvalidGotoCase");

        internal static string ERR_PropertyLacksGet => GetResourceString("ERR_PropertyLacksGet");

        internal static string ERR_BadExceptionType => GetResourceString("ERR_BadExceptionType");

        internal static string ERR_BadEmptyThrow => GetResourceString("ERR_BadEmptyThrow");

        internal static string ERR_BadFinallyLeave => GetResourceString("ERR_BadFinallyLeave");

        internal static string ERR_LabelShadow => GetResourceString("ERR_LabelShadow");

        internal static string ERR_LabelNotFound => GetResourceString("ERR_LabelNotFound");

        internal static string ERR_UnreachableCatch => GetResourceString("ERR_UnreachableCatch");

        internal static string WRN_FilterIsConstantTrue => GetResourceString("WRN_FilterIsConstantTrue");

        internal static string WRN_FilterIsConstantTrue_Title => GetResourceString("WRN_FilterIsConstantTrue_Title");

        internal static string ERR_ReturnExpected => GetResourceString("ERR_ReturnExpected");

        internal static string WRN_UnreachableCode => GetResourceString("WRN_UnreachableCode");

        internal static string WRN_UnreachableCode_Title => GetResourceString("WRN_UnreachableCode_Title");

        internal static string ERR_SwitchFallThrough => GetResourceString("ERR_SwitchFallThrough");

        internal static string WRN_UnreferencedLabel => GetResourceString("WRN_UnreferencedLabel");

        internal static string WRN_UnreferencedLabel_Title => GetResourceString("WRN_UnreferencedLabel_Title");

        internal static string ERR_UseDefViolation => GetResourceString("ERR_UseDefViolation");

        internal static string WRN_UseDefViolation => GetResourceString("WRN_UseDefViolation");

        internal static string WRN_UseDefViolation_Title => GetResourceString("WRN_UseDefViolation_Title");

        internal static string WRN_UnreferencedVar => GetResourceString("WRN_UnreferencedVar");

        internal static string WRN_UnreferencedVar_Title => GetResourceString("WRN_UnreferencedVar_Title");

        internal static string WRN_UnreferencedField => GetResourceString("WRN_UnreferencedField");

        internal static string WRN_UnreferencedField_Title => GetResourceString("WRN_UnreferencedField_Title");

        internal static string ERR_UseDefViolationField => GetResourceString("ERR_UseDefViolationField");

        internal static string WRN_UseDefViolationField => GetResourceString("WRN_UseDefViolationField");

        internal static string WRN_UseDefViolationField_Title => GetResourceString("WRN_UseDefViolationField_Title");

        internal static string ERR_UseDefViolationProperty => GetResourceString("ERR_UseDefViolationProperty");

        internal static string WRN_UseDefViolationProperty => GetResourceString("WRN_UseDefViolationProperty");

        internal static string WRN_UseDefViolationProperty_Title => GetResourceString("WRN_UseDefViolationProperty_Title");

        internal static string ERR_UnassignedThis => GetResourceString("ERR_UnassignedThis");

        internal static string WRN_UnassignedThis => GetResourceString("WRN_UnassignedThis");

        internal static string WRN_UnassignedThis_Title => GetResourceString("WRN_UnassignedThis_Title");

        internal static string ERR_AmbigQM => GetResourceString("ERR_AmbigQM");

        internal static string ERR_InvalidQM => GetResourceString("ERR_InvalidQM");

        internal static string ERR_NoBaseClass => GetResourceString("ERR_NoBaseClass");

        internal static string ERR_BaseIllegal => GetResourceString("ERR_BaseIllegal");

        internal static string ERR_ObjectProhibited => GetResourceString("ERR_ObjectProhibited");

        internal static string ERR_ParamUnassigned => GetResourceString("ERR_ParamUnassigned");

        internal static string WRN_ParamUnassigned => GetResourceString("WRN_ParamUnassigned");

        internal static string WRN_ParamUnassigned_Title => GetResourceString("WRN_ParamUnassigned_Title");

        internal static string ERR_InvalidArray => GetResourceString("ERR_InvalidArray");

        internal static string ERR_ExternHasBody => GetResourceString("ERR_ExternHasBody");

        internal static string ERR_ExternHasConstructorInitializer => GetResourceString("ERR_ExternHasConstructorInitializer");

        internal static string ERR_AbstractAndExtern => GetResourceString("ERR_AbstractAndExtern");

        internal static string ERR_BadAttributeParamType => GetResourceString("ERR_BadAttributeParamType");

        internal static string ERR_BadAttributeArgument => GetResourceString("ERR_BadAttributeArgument");

        internal static string ERR_BadAttributeParamDefaultArgument => GetResourceString("ERR_BadAttributeParamDefaultArgument");

        internal static string WRN_IsAlwaysTrue => GetResourceString("WRN_IsAlwaysTrue");

        internal static string WRN_IsAlwaysTrue_Title => GetResourceString("WRN_IsAlwaysTrue_Title");

        internal static string WRN_IsAlwaysFalse => GetResourceString("WRN_IsAlwaysFalse");

        internal static string WRN_IsAlwaysFalse_Title => GetResourceString("WRN_IsAlwaysFalse_Title");

        internal static string ERR_LockNeedsReference => GetResourceString("ERR_LockNeedsReference");

        internal static string ERR_NullNotValid => GetResourceString("ERR_NullNotValid");

        internal static string ERR_DefaultLiteralNotValid => GetResourceString("ERR_DefaultLiteralNotValid");

        internal static string ERR_UseDefViolationThis => GetResourceString("ERR_UseDefViolationThis");

        internal static string WRN_UseDefViolationThis => GetResourceString("WRN_UseDefViolationThis");

        internal static string WRN_UseDefViolationThis_Title => GetResourceString("WRN_UseDefViolationThis_Title");

        internal static string ERR_ArgsInvalid => GetResourceString("ERR_ArgsInvalid");

        internal static string ERR_PtrExpected => GetResourceString("ERR_PtrExpected");

        internal static string ERR_PtrIndexSingle => GetResourceString("ERR_PtrIndexSingle");

        internal static string WRN_ByRefNonAgileField => GetResourceString("WRN_ByRefNonAgileField");

        internal static string WRN_ByRefNonAgileField_Title => GetResourceString("WRN_ByRefNonAgileField_Title");

        internal static string ERR_AssgReadonlyStatic => GetResourceString("ERR_AssgReadonlyStatic");

        internal static string ERR_RefReadonlyStatic => GetResourceString("ERR_RefReadonlyStatic");

        internal static string ERR_AssgReadonlyProp => GetResourceString("ERR_AssgReadonlyProp");

        internal static string ERR_IllegalStatement => GetResourceString("ERR_IllegalStatement");

        internal static string ERR_BadGetEnumerator => GetResourceString("ERR_BadGetEnumerator");

        internal static string ERR_BadGetAsyncEnumerator => GetResourceString("ERR_BadGetAsyncEnumerator");

        internal static string ERR_TooManyLocals => GetResourceString("ERR_TooManyLocals");

        internal static string ERR_AbstractBaseCall => GetResourceString("ERR_AbstractBaseCall");

        internal static string ERR_RefProperty => GetResourceString("ERR_RefProperty");

        internal static string ERR_ManagedAddr => GetResourceString("ERR_ManagedAddr");

        internal static string ERR_BadFixedInitType => GetResourceString("ERR_BadFixedInitType");

        internal static string ERR_FixedMustInit => GetResourceString("ERR_FixedMustInit");

        internal static string ERR_InvalidAddrOp => GetResourceString("ERR_InvalidAddrOp");

        internal static string ERR_FixedNeeded => GetResourceString("ERR_FixedNeeded");

        internal static string ERR_FixedNotNeeded => GetResourceString("ERR_FixedNotNeeded");

        internal static string ERR_ExprCannotBeFixed => GetResourceString("ERR_ExprCannotBeFixed");

        internal static string ERR_UnsafeNeeded => GetResourceString("ERR_UnsafeNeeded");

        internal static string ERR_OpTFRetType => GetResourceString("ERR_OpTFRetType");

        internal static string ERR_OperatorNeedsMatch => GetResourceString("ERR_OperatorNeedsMatch");

        internal static string ERR_BadBoolOp => GetResourceString("ERR_BadBoolOp");

        internal static string ERR_MustHaveOpTF => GetResourceString("ERR_MustHaveOpTF");

        internal static string WRN_UnreferencedVarAssg => GetResourceString("WRN_UnreferencedVarAssg");

        internal static string WRN_UnreferencedVarAssg_Title => GetResourceString("WRN_UnreferencedVarAssg_Title");

        internal static string ERR_CheckedOverflow => GetResourceString("ERR_CheckedOverflow");

        internal static string ERR_ConstOutOfRangeChecked => GetResourceString("ERR_ConstOutOfRangeChecked");

        internal static string ERR_BadVarargs => GetResourceString("ERR_BadVarargs");

        internal static string ERR_ParamsMustBeArray => GetResourceString("ERR_ParamsMustBeArray");

        internal static string ERR_IllegalArglist => GetResourceString("ERR_IllegalArglist");

        internal static string ERR_IllegalUnsafe => GetResourceString("ERR_IllegalUnsafe");

        internal static string ERR_AmbigMember => GetResourceString("ERR_AmbigMember");

        internal static string ERR_BadForeachDecl => GetResourceString("ERR_BadForeachDecl");

        internal static string ERR_ParamsLast => GetResourceString("ERR_ParamsLast");

        internal static string ERR_SizeofUnsafe => GetResourceString("ERR_SizeofUnsafe");

        internal static string ERR_DottedTypeNameNotFoundInNS => GetResourceString("ERR_DottedTypeNameNotFoundInNS");

        internal static string ERR_FieldInitRefNonstatic => GetResourceString("ERR_FieldInitRefNonstatic");

        internal static string ERR_SealedNonOverride => GetResourceString("ERR_SealedNonOverride");

        internal static string ERR_CantOverrideSealed => GetResourceString("ERR_CantOverrideSealed");

        internal static string ERR_VoidError => GetResourceString("ERR_VoidError");

        internal static string ERR_ConditionalOnOverride => GetResourceString("ERR_ConditionalOnOverride");

        internal static string ERR_ConditionalOnLocalFunction => GetResourceString("ERR_ConditionalOnLocalFunction");

        internal static string ERR_PointerInAsOrIs => GetResourceString("ERR_PointerInAsOrIs");

        internal static string ERR_CallingFinalizeDeprecated => GetResourceString("ERR_CallingFinalizeDeprecated");

        internal static string ERR_SingleTypeNameNotFound => GetResourceString("ERR_SingleTypeNameNotFound");

        internal static string ERR_NegativeStackAllocSize => GetResourceString("ERR_NegativeStackAllocSize");

        internal static string ERR_NegativeArraySize => GetResourceString("ERR_NegativeArraySize");

        internal static string ERR_OverrideFinalizeDeprecated => GetResourceString("ERR_OverrideFinalizeDeprecated");

        internal static string ERR_CallingBaseFinalizeDeprecated => GetResourceString("ERR_CallingBaseFinalizeDeprecated");

        internal static string WRN_NegativeArrayIndex => GetResourceString("WRN_NegativeArrayIndex");

        internal static string WRN_NegativeArrayIndex_Title => GetResourceString("WRN_NegativeArrayIndex_Title");

        internal static string WRN_BadRefCompareLeft => GetResourceString("WRN_BadRefCompareLeft");

        internal static string WRN_BadRefCompareLeft_Title => GetResourceString("WRN_BadRefCompareLeft_Title");

        internal static string WRN_BadRefCompareRight => GetResourceString("WRN_BadRefCompareRight");

        internal static string WRN_BadRefCompareRight_Title => GetResourceString("WRN_BadRefCompareRight_Title");

        internal static string ERR_BadCastInFixed => GetResourceString("ERR_BadCastInFixed");

        internal static string ERR_StackallocInCatchFinally => GetResourceString("ERR_StackallocInCatchFinally");

        internal static string ERR_VarargsLast => GetResourceString("ERR_VarargsLast");

        internal static string ERR_MissingPartial => GetResourceString("ERR_MissingPartial");

        internal static string ERR_PartialTypeKindConflict => GetResourceString("ERR_PartialTypeKindConflict");

        internal static string ERR_PartialModifierConflict => GetResourceString("ERR_PartialModifierConflict");

        internal static string ERR_PartialMultipleBases => GetResourceString("ERR_PartialMultipleBases");

        internal static string ERR_PartialWrongTypeParams => GetResourceString("ERR_PartialWrongTypeParams");

        internal static string ERR_PartialWrongConstraints => GetResourceString("ERR_PartialWrongConstraints");

        internal static string ERR_NoImplicitConvCast => GetResourceString("ERR_NoImplicitConvCast");

        internal static string ERR_PartialMisplaced => GetResourceString("ERR_PartialMisplaced");

        internal static string ERR_ImportedCircularBase => GetResourceString("ERR_ImportedCircularBase");

        internal static string ERR_UseDefViolationOut => GetResourceString("ERR_UseDefViolationOut");

        internal static string WRN_UseDefViolationOut => GetResourceString("WRN_UseDefViolationOut");

        internal static string WRN_UseDefViolationOut_Title => GetResourceString("WRN_UseDefViolationOut_Title");

        internal static string ERR_ArraySizeInDeclaration => GetResourceString("ERR_ArraySizeInDeclaration");

        internal static string ERR_InaccessibleGetter => GetResourceString("ERR_InaccessibleGetter");

        internal static string ERR_InaccessibleSetter => GetResourceString("ERR_InaccessibleSetter");

        internal static string ERR_InvalidPropertyAccessMod => GetResourceString("ERR_InvalidPropertyAccessMod");

        internal static string ERR_DuplicatePropertyAccessMods => GetResourceString("ERR_DuplicatePropertyAccessMods");

        internal static string ERR_AccessModMissingAccessor => GetResourceString("ERR_AccessModMissingAccessor");

        internal static string ERR_UnimplementedInterfaceAccessor => GetResourceString("ERR_UnimplementedInterfaceAccessor");

        internal static string WRN_PatternIsAmbiguous => GetResourceString("WRN_PatternIsAmbiguous");

        internal static string WRN_PatternIsAmbiguous_Title => GetResourceString("WRN_PatternIsAmbiguous_Title");

        internal static string WRN_PatternNotPublicOrNotInstance => GetResourceString("WRN_PatternNotPublicOrNotInstance");

        internal static string WRN_PatternNotPublicOrNotInstance_Title => GetResourceString("WRN_PatternNotPublicOrNotInstance_Title");

        internal static string WRN_PatternBadSignature => GetResourceString("WRN_PatternBadSignature");

        internal static string WRN_PatternBadSignature_Title => GetResourceString("WRN_PatternBadSignature_Title");

        internal static string ERR_FriendRefNotEqualToThis => GetResourceString("ERR_FriendRefNotEqualToThis");

        internal static string ERR_FriendRefSigningMismatch => GetResourceString("ERR_FriendRefSigningMismatch");

        internal static string WRN_SequentialOnPartialClass => GetResourceString("WRN_SequentialOnPartialClass");

        internal static string WRN_SequentialOnPartialClass_Title => GetResourceString("WRN_SequentialOnPartialClass_Title");

        internal static string ERR_BadConstType => GetResourceString("ERR_BadConstType");

        internal static string ERR_NoNewTyvar => GetResourceString("ERR_NoNewTyvar");

        internal static string ERR_BadArity => GetResourceString("ERR_BadArity");

        internal static string ERR_BadTypeArgument => GetResourceString("ERR_BadTypeArgument");

        internal static string ERR_TypeArgsNotAllowed => GetResourceString("ERR_TypeArgsNotAllowed");

        internal static string ERR_HasNoTypeVars => GetResourceString("ERR_HasNoTypeVars");

        internal static string ERR_NewConstraintNotSatisfied => GetResourceString("ERR_NewConstraintNotSatisfied");

        internal static string ERR_GenericConstraintNotSatisfiedRefType => GetResourceString("ERR_GenericConstraintNotSatisfiedRefType");

        internal static string ERR_GenericConstraintNotSatisfiedNullableEnum => GetResourceString("ERR_GenericConstraintNotSatisfiedNullableEnum");

        internal static string ERR_GenericConstraintNotSatisfiedNullableInterface => GetResourceString("ERR_GenericConstraintNotSatisfiedNullableInterface");

        internal static string ERR_GenericConstraintNotSatisfiedTyVar => GetResourceString("ERR_GenericConstraintNotSatisfiedTyVar");

        internal static string ERR_GenericConstraintNotSatisfiedValType => GetResourceString("ERR_GenericConstraintNotSatisfiedValType");

        internal static string ERR_DuplicateGeneratedName => GetResourceString("ERR_DuplicateGeneratedName");

        internal static string ERR_GlobalSingleTypeNameNotFound => GetResourceString("ERR_GlobalSingleTypeNameNotFound");

        internal static string ERR_NewBoundMustBeLast => GetResourceString("ERR_NewBoundMustBeLast");

        internal static string WRN_MainCantBeGeneric => GetResourceString("WRN_MainCantBeGeneric");

        internal static string WRN_MainCantBeGeneric_Title => GetResourceString("WRN_MainCantBeGeneric_Title");

        internal static string ERR_TypeVarCantBeNull => GetResourceString("ERR_TypeVarCantBeNull");

        internal static string ERR_AttributeCantBeGeneric => GetResourceString("ERR_AttributeCantBeGeneric");

        internal static string ERR_DuplicateBound => GetResourceString("ERR_DuplicateBound");

        internal static string ERR_ClassBoundNotFirst => GetResourceString("ERR_ClassBoundNotFirst");

        internal static string ERR_BadRetType => GetResourceString("ERR_BadRetType");

        internal static string ERR_DelegateRefMismatch => GetResourceString("ERR_DelegateRefMismatch");

        internal static string ERR_DuplicateConstraintClause => GetResourceString("ERR_DuplicateConstraintClause");

        internal static string ERR_CantInferMethTypeArgs => GetResourceString("ERR_CantInferMethTypeArgs");

        internal static string ERR_LocalSameNameAsTypeParam => GetResourceString("ERR_LocalSameNameAsTypeParam");

        internal static string ERR_AsWithTypeVar => GetResourceString("ERR_AsWithTypeVar");

        internal static string WRN_UnreferencedFieldAssg => GetResourceString("WRN_UnreferencedFieldAssg");

        internal static string WRN_UnreferencedFieldAssg_Title => GetResourceString("WRN_UnreferencedFieldAssg_Title");

        internal static string ERR_BadIndexerNameAttr => GetResourceString("ERR_BadIndexerNameAttr");

        internal static string ERR_AttrArgWithTypeVars => GetResourceString("ERR_AttrArgWithTypeVars");

        internal static string ERR_NewTyvarWithArgs => GetResourceString("ERR_NewTyvarWithArgs");

        internal static string ERR_AbstractSealedStatic => GetResourceString("ERR_AbstractSealedStatic");

        internal static string WRN_AmbiguousXMLReference => GetResourceString("WRN_AmbiguousXMLReference");

        internal static string WRN_AmbiguousXMLReference_Title => GetResourceString("WRN_AmbiguousXMLReference_Title");

        internal static string WRN_VolatileByRef => GetResourceString("WRN_VolatileByRef");

        internal static string WRN_VolatileByRef_Title => GetResourceString("WRN_VolatileByRef_Title");

        internal static string WRN_VolatileByRef_Description => GetResourceString("WRN_VolatileByRef_Description");

        internal static string ERR_ComImportWithImpl => GetResourceString("ERR_ComImportWithImpl");

        internal static string ERR_ComImportWithBase => GetResourceString("ERR_ComImportWithBase");

        internal static string ERR_ImplBadConstraints => GetResourceString("ERR_ImplBadConstraints");

        internal static string ERR_ImplBadTupleNames => GetResourceString("ERR_ImplBadTupleNames");

        internal static string ERR_DottedTypeNameNotFoundInAgg => GetResourceString("ERR_DottedTypeNameNotFoundInAgg");

        internal static string ERR_MethGrpToNonDel => GetResourceString("ERR_MethGrpToNonDel");

        internal static string ERR_BadExternAlias => GetResourceString("ERR_BadExternAlias");

        internal static string ERR_ColColWithTypeAlias => GetResourceString("ERR_ColColWithTypeAlias");

        internal static string ERR_AliasNotFound => GetResourceString("ERR_AliasNotFound");

        internal static string ERR_SameFullNameAggAgg => GetResourceString("ERR_SameFullNameAggAgg");

        internal static string ERR_SameFullNameNsAgg => GetResourceString("ERR_SameFullNameNsAgg");

        internal static string WRN_SameFullNameThisNsAgg => GetResourceString("WRN_SameFullNameThisNsAgg");

        internal static string WRN_SameFullNameThisNsAgg_Title => GetResourceString("WRN_SameFullNameThisNsAgg_Title");

        internal static string WRN_SameFullNameThisAggAgg => GetResourceString("WRN_SameFullNameThisAggAgg");

        internal static string WRN_SameFullNameThisAggAgg_Title => GetResourceString("WRN_SameFullNameThisAggAgg_Title");

        internal static string WRN_SameFullNameThisAggNs => GetResourceString("WRN_SameFullNameThisAggNs");

        internal static string WRN_SameFullNameThisAggNs_Title => GetResourceString("WRN_SameFullNameThisAggNs_Title");

        internal static string ERR_SameFullNameThisAggThisNs => GetResourceString("ERR_SameFullNameThisAggThisNs");

        internal static string ERR_ExternAfterElements => GetResourceString("ERR_ExternAfterElements");

        internal static string WRN_GlobalAliasDefn => GetResourceString("WRN_GlobalAliasDefn");

        internal static string WRN_GlobalAliasDefn_Title => GetResourceString("WRN_GlobalAliasDefn_Title");

        internal static string ERR_SealedStaticClass => GetResourceString("ERR_SealedStaticClass");

        internal static string ERR_PrivateAbstractAccessor => GetResourceString("ERR_PrivateAbstractAccessor");

        internal static string ERR_ValueExpected => GetResourceString("ERR_ValueExpected");

        internal static string ERR_UnboxNotLValue => GetResourceString("ERR_UnboxNotLValue");

        internal static string ERR_AnonMethGrpInForEach => GetResourceString("ERR_AnonMethGrpInForEach");

        internal static string ERR_BadIncDecRetType => GetResourceString("ERR_BadIncDecRetType");

        internal static string ERR_TypeConstraintsMustBeUniqueAndFirst => GetResourceString("ERR_TypeConstraintsMustBeUniqueAndFirst");

        internal static string ERR_RefValBoundWithClass => GetResourceString("ERR_RefValBoundWithClass");

        internal static string ERR_UnmanagedBoundWithClass => GetResourceString("ERR_UnmanagedBoundWithClass");

        internal static string ERR_NewBoundWithVal => GetResourceString("ERR_NewBoundWithVal");

        internal static string ERR_RefConstraintNotSatisfied => GetResourceString("ERR_RefConstraintNotSatisfied");

        internal static string ERR_ValConstraintNotSatisfied => GetResourceString("ERR_ValConstraintNotSatisfied");

        internal static string ERR_CircularConstraint => GetResourceString("ERR_CircularConstraint");

        internal static string ERR_BaseConstraintConflict => GetResourceString("ERR_BaseConstraintConflict");

        internal static string ERR_ConWithValCon => GetResourceString("ERR_ConWithValCon");

        internal static string ERR_AmbigUDConv => GetResourceString("ERR_AmbigUDConv");

        internal static string WRN_AlwaysNull => GetResourceString("WRN_AlwaysNull");

        internal static string WRN_AlwaysNull_Title => GetResourceString("WRN_AlwaysNull_Title");

        internal static string ERR_RefReturnThis => GetResourceString("ERR_RefReturnThis");

        internal static string ERR_AttributeCtorInParameter => GetResourceString("ERR_AttributeCtorInParameter");

        internal static string ERR_OverrideWithConstraints => GetResourceString("ERR_OverrideWithConstraints");

        internal static string ERR_AmbigOverride => GetResourceString("ERR_AmbigOverride");

        internal static string ERR_DecConstError => GetResourceString("ERR_DecConstError");

        internal static string WRN_CmpAlwaysFalse => GetResourceString("WRN_CmpAlwaysFalse");

        internal static string WRN_CmpAlwaysFalse_Title => GetResourceString("WRN_CmpAlwaysFalse_Title");

        internal static string WRN_FinalizeMethod => GetResourceString("WRN_FinalizeMethod");

        internal static string WRN_FinalizeMethod_Title => GetResourceString("WRN_FinalizeMethod_Title");

        internal static string WRN_FinalizeMethod_Description => GetResourceString("WRN_FinalizeMethod_Description");

        internal static string ERR_ExplicitImplParams => GetResourceString("ERR_ExplicitImplParams");

        internal static string WRN_GotoCaseShouldConvert => GetResourceString("WRN_GotoCaseShouldConvert");

        internal static string WRN_GotoCaseShouldConvert_Title => GetResourceString("WRN_GotoCaseShouldConvert_Title");

        internal static string ERR_MethodImplementingAccessor => GetResourceString("ERR_MethodImplementingAccessor");

        internal static string WRN_NubExprIsConstBool => GetResourceString("WRN_NubExprIsConstBool");

        internal static string WRN_NubExprIsConstBool_Title => GetResourceString("WRN_NubExprIsConstBool_Title");

        internal static string WRN_NubExprIsConstBool2 => GetResourceString("WRN_NubExprIsConstBool2");

        internal static string WRN_NubExprIsConstBool2_Title => GetResourceString("WRN_NubExprIsConstBool2_Title");

        internal static string WRN_ExplicitImplCollision => GetResourceString("WRN_ExplicitImplCollision");

        internal static string WRN_ExplicitImplCollision_Title => GetResourceString("WRN_ExplicitImplCollision_Title");

        internal static string ERR_AbstractHasBody => GetResourceString("ERR_AbstractHasBody");

        internal static string ERR_ConcreteMissingBody => GetResourceString("ERR_ConcreteMissingBody");

        internal static string ERR_AbstractAndSealed => GetResourceString("ERR_AbstractAndSealed");

        internal static string ERR_AbstractNotVirtual => GetResourceString("ERR_AbstractNotVirtual");

        internal static string ERR_StaticConstant => GetResourceString("ERR_StaticConstant");

        internal static string ERR_CantOverrideNonFunction => GetResourceString("ERR_CantOverrideNonFunction");

        internal static string ERR_CantOverrideNonVirtual => GetResourceString("ERR_CantOverrideNonVirtual");

        internal static string ERR_CantChangeAccessOnOverride => GetResourceString("ERR_CantChangeAccessOnOverride");

        internal static string ERR_CantChangeTupleNamesOnOverride => GetResourceString("ERR_CantChangeTupleNamesOnOverride");

        internal static string ERR_CantChangeReturnTypeOnOverride => GetResourceString("ERR_CantChangeReturnTypeOnOverride");

        internal static string ERR_CantDeriveFromSealedType => GetResourceString("ERR_CantDeriveFromSealedType");

        internal static string ERR_AbstractInConcreteClass => GetResourceString("ERR_AbstractInConcreteClass");

        internal static string ERR_StaticConstructorWithExplicitConstructorCall => GetResourceString("ERR_StaticConstructorWithExplicitConstructorCall");

        internal static string ERR_StaticConstructorWithAccessModifiers => GetResourceString("ERR_StaticConstructorWithAccessModifiers");

        internal static string ERR_RecursiveConstructorCall => GetResourceString("ERR_RecursiveConstructorCall");

        internal static string ERR_IndirectRecursiveConstructorCall => GetResourceString("ERR_IndirectRecursiveConstructorCall");

        internal static string ERR_ObjectCallingBaseConstructor => GetResourceString("ERR_ObjectCallingBaseConstructor");

        internal static string ERR_PredefinedTypeNotFound => GetResourceString("ERR_PredefinedTypeNotFound");

        internal static string ERR_PredefinedValueTupleTypeNotFound => GetResourceString("ERR_PredefinedValueTupleTypeNotFound");

        internal static string ERR_PredefinedValueTupleTypeAmbiguous3 => GetResourceString("ERR_PredefinedValueTupleTypeAmbiguous3");

        internal static string ERR_StructWithBaseConstructorCall => GetResourceString("ERR_StructWithBaseConstructorCall");

        internal static string ERR_StructLayoutCycle => GetResourceString("ERR_StructLayoutCycle");

        internal static string ERR_InterfacesCantContainFields => GetResourceString("ERR_InterfacesCantContainFields");

        internal static string ERR_InterfacesCantContainConstructors => GetResourceString("ERR_InterfacesCantContainConstructors");

        internal static string ERR_NonInterfaceInInterfaceList => GetResourceString("ERR_NonInterfaceInInterfaceList");

        internal static string ERR_DuplicateInterfaceInBaseList => GetResourceString("ERR_DuplicateInterfaceInBaseList");

        internal static string ERR_DuplicateInterfaceWithTupleNamesInBaseList => GetResourceString("ERR_DuplicateInterfaceWithTupleNamesInBaseList");

        internal static string ERR_DuplicateInterfaceWithDifferencesInBaseList => GetResourceString("ERR_DuplicateInterfaceWithDifferencesInBaseList");

        internal static string ERR_CycleInInterfaceInheritance => GetResourceString("ERR_CycleInInterfaceInheritance");

        internal static string ERR_HidingAbstractMethod => GetResourceString("ERR_HidingAbstractMethod");

        internal static string ERR_UnimplementedAbstractMethod => GetResourceString("ERR_UnimplementedAbstractMethod");

        internal static string ERR_UnimplementedInterfaceMember => GetResourceString("ERR_UnimplementedInterfaceMember");

        internal static string ERR_ObjectCantHaveBases => GetResourceString("ERR_ObjectCantHaveBases");

        internal static string ERR_ExplicitInterfaceImplementationNotInterface => GetResourceString("ERR_ExplicitInterfaceImplementationNotInterface");

        internal static string ERR_InterfaceMemberNotFound => GetResourceString("ERR_InterfaceMemberNotFound");

        internal static string ERR_ClassDoesntImplementInterface => GetResourceString("ERR_ClassDoesntImplementInterface");

        internal static string ERR_ExplicitInterfaceImplementationInNonClassOrStruct => GetResourceString("ERR_ExplicitInterfaceImplementationInNonClassOrStruct");

        internal static string ERR_MemberNameSameAsType => GetResourceString("ERR_MemberNameSameAsType");

        internal static string ERR_EnumeratorOverflow => GetResourceString("ERR_EnumeratorOverflow");

        internal static string ERR_CantOverrideNonProperty => GetResourceString("ERR_CantOverrideNonProperty");

        internal static string ERR_NoGetToOverride => GetResourceString("ERR_NoGetToOverride");

        internal static string ERR_NoSetToOverride => GetResourceString("ERR_NoSetToOverride");

        internal static string ERR_PropertyCantHaveVoidType => GetResourceString("ERR_PropertyCantHaveVoidType");

        internal static string ERR_PropertyWithNoAccessors => GetResourceString("ERR_PropertyWithNoAccessors");

        internal static string ERR_CantUseVoidInArglist => GetResourceString("ERR_CantUseVoidInArglist");

        internal static string ERR_NewVirtualInSealed => GetResourceString("ERR_NewVirtualInSealed");

        internal static string ERR_ExplicitPropertyAddingAccessor => GetResourceString("ERR_ExplicitPropertyAddingAccessor");

        internal static string ERR_ExplicitPropertyMismatchInitOnly => GetResourceString("ERR_ExplicitPropertyMismatchInitOnly");

        internal static string ERR_ExplicitPropertyMissingAccessor => GetResourceString("ERR_ExplicitPropertyMissingAccessor");

        internal static string ERR_ConversionWithInterface => GetResourceString("ERR_ConversionWithInterface");

        internal static string ERR_ConversionWithBase => GetResourceString("ERR_ConversionWithBase");

        internal static string ERR_ConversionWithDerived => GetResourceString("ERR_ConversionWithDerived");

        internal static string ERR_IdentityConversion => GetResourceString("ERR_IdentityConversion");

        internal static string ERR_ConversionNotInvolvingContainedType => GetResourceString("ERR_ConversionNotInvolvingContainedType");

        internal static string ERR_DuplicateConversionInClass => GetResourceString("ERR_DuplicateConversionInClass");

        internal static string ERR_OperatorsMustBeStatic => GetResourceString("ERR_OperatorsMustBeStatic");

        internal static string ERR_BadIncDecSignature => GetResourceString("ERR_BadIncDecSignature");

        internal static string ERR_BadUnaryOperatorSignature => GetResourceString("ERR_BadUnaryOperatorSignature");

        internal static string ERR_BadBinaryOperatorSignature => GetResourceString("ERR_BadBinaryOperatorSignature");

        internal static string ERR_BadShiftOperatorSignature => GetResourceString("ERR_BadShiftOperatorSignature");

        internal static string ERR_InterfacesCantContainConversionOrEqualityOperators => GetResourceString("ERR_InterfacesCantContainConversionOrEqualityOperators");

        internal static string ERR_StructsCantContainDefaultConstructor => GetResourceString("ERR_StructsCantContainDefaultConstructor");

        internal static string ERR_EnumsCantContainDefaultConstructor => GetResourceString("ERR_EnumsCantContainDefaultConstructor");

        internal static string ERR_CantOverrideBogusMethod => GetResourceString("ERR_CantOverrideBogusMethod");

        internal static string ERR_BindToBogus => GetResourceString("ERR_BindToBogus");

        internal static string ERR_CantCallSpecialMethod => GetResourceString("ERR_CantCallSpecialMethod");

        internal static string ERR_BadTypeReference => GetResourceString("ERR_BadTypeReference");

        internal static string ERR_FieldInitializerInStruct => GetResourceString("ERR_FieldInitializerInStruct");

        internal static string ERR_BadDestructorName => GetResourceString("ERR_BadDestructorName");

        internal static string ERR_OnlyClassesCanContainDestructors => GetResourceString("ERR_OnlyClassesCanContainDestructors");

        internal static string ERR_ConflictAliasAndMember => GetResourceString("ERR_ConflictAliasAndMember");

        internal static string ERR_ConflictingAliasAndDefinition => GetResourceString("ERR_ConflictingAliasAndDefinition");

        internal static string ERR_ConditionalOnSpecialMethod => GetResourceString("ERR_ConditionalOnSpecialMethod");

        internal static string ERR_ConditionalMustReturnVoid => GetResourceString("ERR_ConditionalMustReturnVoid");

        internal static string ERR_DuplicateAttribute => GetResourceString("ERR_DuplicateAttribute");

        internal static string ERR_DuplicateAttributeInNetModule => GetResourceString("ERR_DuplicateAttributeInNetModule");

        internal static string ERR_ConditionalOnInterfaceMethod => GetResourceString("ERR_ConditionalOnInterfaceMethod");

        internal static string ERR_OperatorCantReturnVoid => GetResourceString("ERR_OperatorCantReturnVoid");

        internal static string ERR_BadDynamicConversion => GetResourceString("ERR_BadDynamicConversion");

        internal static string ERR_InvalidAttributeArgument => GetResourceString("ERR_InvalidAttributeArgument");

        internal static string ERR_ParameterNotValidForType => GetResourceString("ERR_ParameterNotValidForType");

        internal static string ERR_AttributeParameterRequired1 => GetResourceString("ERR_AttributeParameterRequired1");

        internal static string ERR_AttributeParameterRequired2 => GetResourceString("ERR_AttributeParameterRequired2");

        internal static string ERR_MarshalUnmanagedTypeNotValidForFields => GetResourceString("ERR_MarshalUnmanagedTypeNotValidForFields");

        internal static string ERR_MarshalUnmanagedTypeOnlyValidForFields => GetResourceString("ERR_MarshalUnmanagedTypeOnlyValidForFields");

        internal static string ERR_AttributeOnBadSymbolType => GetResourceString("ERR_AttributeOnBadSymbolType");

        internal static string ERR_FloatOverflow => GetResourceString("ERR_FloatOverflow");

        internal static string ERR_ComImportWithoutUuidAttribute => GetResourceString("ERR_ComImportWithoutUuidAttribute");

        internal static string ERR_InvalidNamedArgument => GetResourceString("ERR_InvalidNamedArgument");

        internal static string ERR_DllImportOnInvalidMethod => GetResourceString("ERR_DllImportOnInvalidMethod");

        internal static string ERR_EncUpdateFailedMissingAttribute => GetResourceString("ERR_EncUpdateFailedMissingAttribute");

        internal static string ERR_DllImportOnGenericMethod => GetResourceString("ERR_DllImportOnGenericMethod");

        internal static string ERR_FieldCantBeRefAny => GetResourceString("ERR_FieldCantBeRefAny");

        internal static string ERR_FieldAutoPropCantBeByRefLike => GetResourceString("ERR_FieldAutoPropCantBeByRefLike");

        internal static string ERR_ArrayElementCantBeRefAny => GetResourceString("ERR_ArrayElementCantBeRefAny");

        internal static string WRN_DeprecatedSymbol => GetResourceString("WRN_DeprecatedSymbol");

        internal static string WRN_DeprecatedSymbol_Title => GetResourceString("WRN_DeprecatedSymbol_Title");

        internal static string ERR_NotAnAttributeClass => GetResourceString("ERR_NotAnAttributeClass");

        internal static string ERR_BadNamedAttributeArgument => GetResourceString("ERR_BadNamedAttributeArgument");

        internal static string WRN_DeprecatedSymbolStr => GetResourceString("WRN_DeprecatedSymbolStr");

        internal static string WRN_DeprecatedSymbolStr_Title => GetResourceString("WRN_DeprecatedSymbolStr_Title");

        internal static string ERR_DeprecatedSymbolStr => GetResourceString("ERR_DeprecatedSymbolStr");

        internal static string ERR_IndexerCantHaveVoidType => GetResourceString("ERR_IndexerCantHaveVoidType");

        internal static string ERR_VirtualPrivate => GetResourceString("ERR_VirtualPrivate");

        internal static string ERR_ArrayInitToNonArrayType => GetResourceString("ERR_ArrayInitToNonArrayType");

        internal static string ERR_ArrayInitInBadPlace => GetResourceString("ERR_ArrayInitInBadPlace");

        internal static string ERR_MissingStructOffset => GetResourceString("ERR_MissingStructOffset");

        internal static string WRN_ExternMethodNoImplementation => GetResourceString("WRN_ExternMethodNoImplementation");

        internal static string WRN_ExternMethodNoImplementation_Title => GetResourceString("WRN_ExternMethodNoImplementation_Title");

        internal static string WRN_ProtectedInSealed => GetResourceString("WRN_ProtectedInSealed");

        internal static string WRN_ProtectedInSealed_Title => GetResourceString("WRN_ProtectedInSealed_Title");

        internal static string ERR_InterfaceImplementedByConditional => GetResourceString("ERR_InterfaceImplementedByConditional");

        internal static string ERR_InterfaceImplementedImplicitlyByVariadic => GetResourceString("ERR_InterfaceImplementedImplicitlyByVariadic");

        internal static string ERR_IllegalRefParam => GetResourceString("ERR_IllegalRefParam");

        internal static string ERR_BadArgumentToAttribute => GetResourceString("ERR_BadArgumentToAttribute");

        internal static string ERR_StructOffsetOnBadStruct => GetResourceString("ERR_StructOffsetOnBadStruct");

        internal static string ERR_StructOffsetOnBadField => GetResourceString("ERR_StructOffsetOnBadField");

        internal static string ERR_AttributeUsageOnNonAttributeClass => GetResourceString("ERR_AttributeUsageOnNonAttributeClass");

        internal static string WRN_PossibleMistakenNullStatement => GetResourceString("WRN_PossibleMistakenNullStatement");

        internal static string WRN_PossibleMistakenNullStatement_Title => GetResourceString("WRN_PossibleMistakenNullStatement_Title");

        internal static string ERR_DuplicateNamedAttributeArgument => GetResourceString("ERR_DuplicateNamedAttributeArgument");

        internal static string ERR_DeriveFromEnumOrValueType => GetResourceString("ERR_DeriveFromEnumOrValueType");

        internal static string ERR_DefaultMemberOnIndexedType => GetResourceString("ERR_DefaultMemberOnIndexedType");

        internal static string ERR_BogusType => GetResourceString("ERR_BogusType");

        internal static string WRN_UnassignedInternalField => GetResourceString("WRN_UnassignedInternalField");

        internal static string WRN_UnassignedInternalField_Title => GetResourceString("WRN_UnassignedInternalField_Title");

        internal static string ERR_CStyleArray => GetResourceString("ERR_CStyleArray");

        internal static string WRN_VacuousIntegralComp => GetResourceString("WRN_VacuousIntegralComp");

        internal static string WRN_VacuousIntegralComp_Title => GetResourceString("WRN_VacuousIntegralComp_Title");

        internal static string ERR_AbstractAttributeClass => GetResourceString("ERR_AbstractAttributeClass");

        internal static string ERR_BadNamedAttributeArgumentType => GetResourceString("ERR_BadNamedAttributeArgumentType");

        internal static string ERR_MissingPredefinedMember => GetResourceString("ERR_MissingPredefinedMember");

        internal static string WRN_AttributeLocationOnBadDeclaration => GetResourceString("WRN_AttributeLocationOnBadDeclaration");

        internal static string WRN_AttributeLocationOnBadDeclaration_Title => GetResourceString("WRN_AttributeLocationOnBadDeclaration_Title");

        internal static string WRN_InvalidAttributeLocation => GetResourceString("WRN_InvalidAttributeLocation");

        internal static string WRN_InvalidAttributeLocation_Title => GetResourceString("WRN_InvalidAttributeLocation_Title");

        internal static string WRN_EqualsWithoutGetHashCode => GetResourceString("WRN_EqualsWithoutGetHashCode");

        internal static string WRN_EqualsWithoutGetHashCode_Title => GetResourceString("WRN_EqualsWithoutGetHashCode_Title");

        internal static string WRN_EqualityOpWithoutEquals => GetResourceString("WRN_EqualityOpWithoutEquals");

        internal static string WRN_EqualityOpWithoutEquals_Title => GetResourceString("WRN_EqualityOpWithoutEquals_Title");

        internal static string WRN_EqualityOpWithoutGetHashCode => GetResourceString("WRN_EqualityOpWithoutGetHashCode");

        internal static string WRN_EqualityOpWithoutGetHashCode_Title => GetResourceString("WRN_EqualityOpWithoutGetHashCode_Title");

        internal static string ERR_OutAttrOnRefParam => GetResourceString("ERR_OutAttrOnRefParam");

        internal static string ERR_OverloadRefKind => GetResourceString("ERR_OverloadRefKind");

        internal static string ERR_LiteralDoubleCast => GetResourceString("ERR_LiteralDoubleCast");

        internal static string WRN_IncorrectBooleanAssg => GetResourceString("WRN_IncorrectBooleanAssg");

        internal static string WRN_IncorrectBooleanAssg_Title => GetResourceString("WRN_IncorrectBooleanAssg_Title");

        internal static string ERR_ProtectedInStruct => GetResourceString("ERR_ProtectedInStruct");

        internal static string ERR_InconsistentIndexerNames => GetResourceString("ERR_InconsistentIndexerNames");

        internal static string ERR_ComImportWithUserCtor => GetResourceString("ERR_ComImportWithUserCtor");

        internal static string ERR_FieldCantHaveVoidType => GetResourceString("ERR_FieldCantHaveVoidType");

        internal static string WRN_NonObsoleteOverridingObsolete => GetResourceString("WRN_NonObsoleteOverridingObsolete");

        internal static string WRN_NonObsoleteOverridingObsolete_Title => GetResourceString("WRN_NonObsoleteOverridingObsolete_Title");

        internal static string ERR_SystemVoid => GetResourceString("ERR_SystemVoid");

        internal static string ERR_ExplicitParamArray => GetResourceString("ERR_ExplicitParamArray");

        internal static string WRN_BitwiseOrSignExtend => GetResourceString("WRN_BitwiseOrSignExtend");

        internal static string WRN_BitwiseOrSignExtend_Title => GetResourceString("WRN_BitwiseOrSignExtend_Title");

        internal static string WRN_BitwiseOrSignExtend_Description => GetResourceString("WRN_BitwiseOrSignExtend_Description");

        internal static string ERR_VolatileStruct => GetResourceString("ERR_VolatileStruct");

        internal static string ERR_VolatileAndReadonly => GetResourceString("ERR_VolatileAndReadonly");

        internal static string ERR_AbstractField => GetResourceString("ERR_AbstractField");

        internal static string ERR_BogusExplicitImpl => GetResourceString("ERR_BogusExplicitImpl");

        internal static string ERR_ExplicitMethodImplAccessor => GetResourceString("ERR_ExplicitMethodImplAccessor");

        internal static string WRN_CoClassWithoutComImport => GetResourceString("WRN_CoClassWithoutComImport");

        internal static string WRN_CoClassWithoutComImport_Title => GetResourceString("WRN_CoClassWithoutComImport_Title");

        internal static string ERR_ConditionalWithOutParam => GetResourceString("ERR_ConditionalWithOutParam");

        internal static string ERR_AccessorImplementingMethod => GetResourceString("ERR_AccessorImplementingMethod");

        internal static string ERR_AliasQualAsExpression => GetResourceString("ERR_AliasQualAsExpression");

        internal static string ERR_DerivingFromATyVar => GetResourceString("ERR_DerivingFromATyVar");

        internal static string ERR_DuplicateTypeParameter => GetResourceString("ERR_DuplicateTypeParameter");

        internal static string WRN_TypeParameterSameAsOuterTypeParameter => GetResourceString("WRN_TypeParameterSameAsOuterTypeParameter");

        internal static string WRN_TypeParameterSameAsOuterTypeParameter_Title => GetResourceString("WRN_TypeParameterSameAsOuterTypeParameter_Title");

        internal static string WRN_TypeParameterSameAsOuterMethodTypeParameter => GetResourceString("WRN_TypeParameterSameAsOuterMethodTypeParameter");

        internal static string WRN_TypeParameterSameAsOuterMethodTypeParameter_Title => GetResourceString("WRN_TypeParameterSameAsOuterMethodTypeParameter_Title");

        internal static string ERR_TypeVariableSameAsParent => GetResourceString("ERR_TypeVariableSameAsParent");

        internal static string ERR_UnifyingInterfaceInstantiations => GetResourceString("ERR_UnifyingInterfaceInstantiations");

        internal static string ERR_GenericDerivingFromAttribute => GetResourceString("ERR_GenericDerivingFromAttribute");

        internal static string ERR_TyVarNotFoundInConstraint => GetResourceString("ERR_TyVarNotFoundInConstraint");

        internal static string ERR_BadBoundType => GetResourceString("ERR_BadBoundType");

        internal static string ERR_SpecialTypeAsBound => GetResourceString("ERR_SpecialTypeAsBound");

        internal static string ERR_BadVisBound => GetResourceString("ERR_BadVisBound");

        internal static string ERR_LookupInTypeVariable => GetResourceString("ERR_LookupInTypeVariable");

        internal static string ERR_BadConstraintType => GetResourceString("ERR_BadConstraintType");

        internal static string ERR_InstanceMemberInStaticClass => GetResourceString("ERR_InstanceMemberInStaticClass");

        internal static string ERR_StaticBaseClass => GetResourceString("ERR_StaticBaseClass");

        internal static string ERR_ConstructorInStaticClass => GetResourceString("ERR_ConstructorInStaticClass");

        internal static string ERR_DestructorInStaticClass => GetResourceString("ERR_DestructorInStaticClass");

        internal static string ERR_InstantiatingStaticClass => GetResourceString("ERR_InstantiatingStaticClass");

        internal static string ERR_StaticDerivedFromNonObject => GetResourceString("ERR_StaticDerivedFromNonObject");

        internal static string ERR_StaticClassInterfaceImpl => GetResourceString("ERR_StaticClassInterfaceImpl");

        internal static string ERR_RefStructInterfaceImpl => GetResourceString("ERR_RefStructInterfaceImpl");

        internal static string ERR_OperatorInStaticClass => GetResourceString("ERR_OperatorInStaticClass");

        internal static string ERR_ConvertToStaticClass => GetResourceString("ERR_ConvertToStaticClass");

        internal static string ERR_ConstraintIsStaticClass => GetResourceString("ERR_ConstraintIsStaticClass");

        internal static string ERR_GenericArgIsStaticClass => GetResourceString("ERR_GenericArgIsStaticClass");

        internal static string ERR_ArrayOfStaticClass => GetResourceString("ERR_ArrayOfStaticClass");

        internal static string ERR_IndexerInStaticClass => GetResourceString("ERR_IndexerInStaticClass");

        internal static string ERR_ParameterIsStaticClass => GetResourceString("ERR_ParameterIsStaticClass");

        internal static string WRN_ParameterIsStaticClass => GetResourceString("WRN_ParameterIsStaticClass");

        internal static string WRN_ParameterIsStaticClass_Title => GetResourceString("WRN_ParameterIsStaticClass_Title");

        internal static string ERR_ReturnTypeIsStaticClass => GetResourceString("ERR_ReturnTypeIsStaticClass");

        internal static string WRN_ReturnTypeIsStaticClass => GetResourceString("WRN_ReturnTypeIsStaticClass");

        internal static string WRN_ReturnTypeIsStaticClass_Title => GetResourceString("WRN_ReturnTypeIsStaticClass_Title");

        internal static string ERR_VarDeclIsStaticClass => GetResourceString("ERR_VarDeclIsStaticClass");

        internal static string ERR_BadEmptyThrowInFinally => GetResourceString("ERR_BadEmptyThrowInFinally");

        internal static string ERR_InvalidSpecifier => GetResourceString("ERR_InvalidSpecifier");

        internal static string WRN_AssignmentToLockOrDispose => GetResourceString("WRN_AssignmentToLockOrDispose");

        internal static string WRN_AssignmentToLockOrDispose_Title => GetResourceString("WRN_AssignmentToLockOrDispose_Title");

        internal static string ERR_ForwardedTypeInThisAssembly => GetResourceString("ERR_ForwardedTypeInThisAssembly");

        internal static string ERR_ForwardedTypeIsNested => GetResourceString("ERR_ForwardedTypeIsNested");

        internal static string ERR_CycleInTypeForwarder => GetResourceString("ERR_CycleInTypeForwarder");

        internal static string ERR_AssemblyNameOnNonModule => GetResourceString("ERR_AssemblyNameOnNonModule");

        internal static string ERR_InvalidAssemblyName => GetResourceString("ERR_InvalidAssemblyName");

        internal static string ERR_InvalidFwdType => GetResourceString("ERR_InvalidFwdType");

        internal static string ERR_CloseUnimplementedInterfaceMemberStatic => GetResourceString("ERR_CloseUnimplementedInterfaceMemberStatic");

        internal static string ERR_CloseUnimplementedInterfaceMemberNotPublic => GetResourceString("ERR_CloseUnimplementedInterfaceMemberNotPublic");

        internal static string ERR_CloseUnimplementedInterfaceMemberWrongReturnType => GetResourceString("ERR_CloseUnimplementedInterfaceMemberWrongReturnType");

        internal static string ERR_DuplicateTypeForwarder => GetResourceString("ERR_DuplicateTypeForwarder");

        internal static string ERR_ExpectedSelectOrGroup => GetResourceString("ERR_ExpectedSelectOrGroup");

        internal static string ERR_ExpectedContextualKeywordOn => GetResourceString("ERR_ExpectedContextualKeywordOn");

        internal static string ERR_ExpectedContextualKeywordEquals => GetResourceString("ERR_ExpectedContextualKeywordEquals");

        internal static string ERR_ExpectedContextualKeywordBy => GetResourceString("ERR_ExpectedContextualKeywordBy");

        internal static string ERR_InvalidAnonymousTypeMemberDeclarator => GetResourceString("ERR_InvalidAnonymousTypeMemberDeclarator");

        internal static string ERR_InvalidInitializerElementInitializer => GetResourceString("ERR_InvalidInitializerElementInitializer");

        internal static string ERR_InconsistentLambdaParameterUsage => GetResourceString("ERR_InconsistentLambdaParameterUsage");

        internal static string ERR_PartialMethodInvalidModifier => GetResourceString("ERR_PartialMethodInvalidModifier");

        internal static string ERR_PartialMethodOnlyInPartialClass => GetResourceString("ERR_PartialMethodOnlyInPartialClass");

        internal static string ERR_PartialMethodNotExplicit => GetResourceString("ERR_PartialMethodNotExplicit");

        internal static string ERR_PartialMethodExtensionDifference => GetResourceString("ERR_PartialMethodExtensionDifference");

        internal static string ERR_PartialMethodOnlyOneLatent => GetResourceString("ERR_PartialMethodOnlyOneLatent");

        internal static string ERR_PartialMethodOnlyOneActual => GetResourceString("ERR_PartialMethodOnlyOneActual");

        internal static string ERR_PartialMethodParamsDifference => GetResourceString("ERR_PartialMethodParamsDifference");

        internal static string ERR_PartialMethodMustHaveLatent => GetResourceString("ERR_PartialMethodMustHaveLatent");

        internal static string ERR_PartialMethodInconsistentTupleNames => GetResourceString("ERR_PartialMethodInconsistentTupleNames");

        internal static string ERR_PartialMethodInconsistentConstraints => GetResourceString("ERR_PartialMethodInconsistentConstraints");

        internal static string ERR_PartialMethodToDelegate => GetResourceString("ERR_PartialMethodToDelegate");

        internal static string ERR_PartialMethodStaticDifference => GetResourceString("ERR_PartialMethodStaticDifference");

        internal static string ERR_PartialMethodUnsafeDifference => GetResourceString("ERR_PartialMethodUnsafeDifference");

        internal static string ERR_PartialMethodInExpressionTree => GetResourceString("ERR_PartialMethodInExpressionTree");

        internal static string WRN_ObsoleteOverridingNonObsolete => GetResourceString("WRN_ObsoleteOverridingNonObsolete");

        internal static string WRN_ObsoleteOverridingNonObsolete_Title => GetResourceString("WRN_ObsoleteOverridingNonObsolete_Title");

        internal static string WRN_DebugFullNameTooLong => GetResourceString("WRN_DebugFullNameTooLong");

        internal static string WRN_DebugFullNameTooLong_Title => GetResourceString("WRN_DebugFullNameTooLong_Title");

        internal static string ERR_ImplicitlyTypedVariableAssignedBadValue => GetResourceString("ERR_ImplicitlyTypedVariableAssignedBadValue");

        internal static string ERR_ImplicitlyTypedVariableWithNoInitializer => GetResourceString("ERR_ImplicitlyTypedVariableWithNoInitializer");

        internal static string ERR_ImplicitlyTypedVariableMultipleDeclarator => GetResourceString("ERR_ImplicitlyTypedVariableMultipleDeclarator");

        internal static string ERR_ImplicitlyTypedVariableAssignedArrayInitializer => GetResourceString("ERR_ImplicitlyTypedVariableAssignedArrayInitializer");

        internal static string ERR_ImplicitlyTypedLocalCannotBeFixed => GetResourceString("ERR_ImplicitlyTypedLocalCannotBeFixed");

        internal static string ERR_ImplicitlyTypedVariableCannotBeConst => GetResourceString("ERR_ImplicitlyTypedVariableCannotBeConst");

        internal static string WRN_ExternCtorNoImplementation => GetResourceString("WRN_ExternCtorNoImplementation");

        internal static string WRN_ExternCtorNoImplementation_Title => GetResourceString("WRN_ExternCtorNoImplementation_Title");

        internal static string ERR_TypeVarNotFound => GetResourceString("ERR_TypeVarNotFound");

        internal static string ERR_ImplicitlyTypedArrayNoBestType => GetResourceString("ERR_ImplicitlyTypedArrayNoBestType");

        internal static string ERR_AnonymousTypePropertyAssignedBadValue => GetResourceString("ERR_AnonymousTypePropertyAssignedBadValue");

        internal static string ERR_ExpressionTreeContainsBaseAccess => GetResourceString("ERR_ExpressionTreeContainsBaseAccess");

        internal static string ERR_ExpressionTreeContainsTupleBinOp => GetResourceString("ERR_ExpressionTreeContainsTupleBinOp");

        internal static string ERR_ExpressionTreeContainsAssignment => GetResourceString("ERR_ExpressionTreeContainsAssignment");

        internal static string ERR_AnonymousTypeDuplicatePropertyName => GetResourceString("ERR_AnonymousTypeDuplicatePropertyName");

        internal static string ERR_StatementLambdaToExpressionTree => GetResourceString("ERR_StatementLambdaToExpressionTree");

        internal static string ERR_ExpressionTreeMustHaveDelegate => GetResourceString("ERR_ExpressionTreeMustHaveDelegate");

        internal static string ERR_AnonymousTypeNotAvailable => GetResourceString("ERR_AnonymousTypeNotAvailable");

        internal static string ERR_LambdaInIsAs => GetResourceString("ERR_LambdaInIsAs");

        internal static string ERR_TypelessTupleInAs => GetResourceString("ERR_TypelessTupleInAs");

        internal static string ERR_ExpressionTreeContainsMultiDimensionalArrayInitializer => GetResourceString("ERR_ExpressionTreeContainsMultiDimensionalArrayInitializer");

        internal static string ERR_MissingArgument => GetResourceString("ERR_MissingArgument");

        internal static string ERR_VariableUsedBeforeDeclaration => GetResourceString("ERR_VariableUsedBeforeDeclaration");

        internal static string ERR_RecursivelyTypedVariable => GetResourceString("ERR_RecursivelyTypedVariable");

        internal static string ERR_UnassignedThisAutoProperty => GetResourceString("ERR_UnassignedThisAutoProperty");

        internal static string WRN_UnassignedThisAutoProperty => GetResourceString("WRN_UnassignedThisAutoProperty");

        internal static string WRN_UnassignedThisAutoProperty_Title => GetResourceString("WRN_UnassignedThisAutoProperty_Title");

        internal static string ERR_VariableUsedBeforeDeclarationAndHidesField => GetResourceString("ERR_VariableUsedBeforeDeclarationAndHidesField");

        internal static string ERR_ExpressionTreeContainsBadCoalesce => GetResourceString("ERR_ExpressionTreeContainsBadCoalesce");

        internal static string ERR_IdentifierExpected => GetResourceString("ERR_IdentifierExpected");

        internal static string ERR_SemicolonExpected => GetResourceString("ERR_SemicolonExpected");

        internal static string ERR_SyntaxError => GetResourceString("ERR_SyntaxError");

        internal static string ERR_DuplicateModifier => GetResourceString("ERR_DuplicateModifier");

        internal static string ERR_DuplicateAccessor => GetResourceString("ERR_DuplicateAccessor");

        internal static string ERR_IntegralTypeExpected => GetResourceString("ERR_IntegralTypeExpected");

        internal static string ERR_IllegalEscape => GetResourceString("ERR_IllegalEscape");

        internal static string ERR_NewlineInConst => GetResourceString("ERR_NewlineInConst");

        internal static string ERR_EmptyCharConst => GetResourceString("ERR_EmptyCharConst");

        internal static string ERR_TooManyCharsInConst => GetResourceString("ERR_TooManyCharsInConst");

        internal static string ERR_InvalidNumber => GetResourceString("ERR_InvalidNumber");

        internal static string ERR_GetOrSetExpected => GetResourceString("ERR_GetOrSetExpected");

        internal static string ERR_ClassTypeExpected => GetResourceString("ERR_ClassTypeExpected");

        internal static string ERR_NamedArgumentExpected => GetResourceString("ERR_NamedArgumentExpected");

        internal static string ERR_TooManyCatches => GetResourceString("ERR_TooManyCatches");

        internal static string ERR_ThisOrBaseExpected => GetResourceString("ERR_ThisOrBaseExpected");

        internal static string ERR_OvlUnaryOperatorExpected => GetResourceString("ERR_OvlUnaryOperatorExpected");

        internal static string ERR_OvlBinaryOperatorExpected => GetResourceString("ERR_OvlBinaryOperatorExpected");

        internal static string ERR_IntOverflow => GetResourceString("ERR_IntOverflow");

        internal static string ERR_EOFExpected => GetResourceString("ERR_EOFExpected");

        internal static string ERR_GlobalDefinitionOrStatementExpected => GetResourceString("ERR_GlobalDefinitionOrStatementExpected");

        internal static string ERR_BadEmbeddedStmt => GetResourceString("ERR_BadEmbeddedStmt");

        internal static string ERR_PPDirectiveExpected => GetResourceString("ERR_PPDirectiveExpected");

        internal static string ERR_EndOfPPLineExpected => GetResourceString("ERR_EndOfPPLineExpected");

        internal static string ERR_CloseParenExpected => GetResourceString("ERR_CloseParenExpected");

        internal static string ERR_EndifDirectiveExpected => GetResourceString("ERR_EndifDirectiveExpected");

        internal static string ERR_UnexpectedDirective => GetResourceString("ERR_UnexpectedDirective");

        internal static string ERR_ErrorDirective => GetResourceString("ERR_ErrorDirective");

        internal static string WRN_WarningDirective => GetResourceString("WRN_WarningDirective");

        internal static string WRN_WarningDirective_Title => GetResourceString("WRN_WarningDirective_Title");

        internal static string ERR_TypeExpected => GetResourceString("ERR_TypeExpected");

        internal static string ERR_PPDefFollowsToken => GetResourceString("ERR_PPDefFollowsToken");

        internal static string ERR_PPReferenceFollowsToken => GetResourceString("ERR_PPReferenceFollowsToken");

        internal static string ERR_OpenEndedComment => GetResourceString("ERR_OpenEndedComment");

        internal static string ERR_Merge_conflict_marker_encountered => GetResourceString("ERR_Merge_conflict_marker_encountered");

        internal static string ERR_NoRefOutWhenRefOnly => GetResourceString("ERR_NoRefOutWhenRefOnly");

        internal static string ERR_NoNetModuleOutputWhenRefOutOrRefOnly => GetResourceString("ERR_NoNetModuleOutputWhenRefOutOrRefOnly");

        internal static string ERR_OvlOperatorExpected => GetResourceString("ERR_OvlOperatorExpected");

        internal static string ERR_EndRegionDirectiveExpected => GetResourceString("ERR_EndRegionDirectiveExpected");

        internal static string ERR_UnterminatedStringLit => GetResourceString("ERR_UnterminatedStringLit");

        internal static string ERR_BadDirectivePlacement => GetResourceString("ERR_BadDirectivePlacement");

        internal static string ERR_IdentifierExpectedKW => GetResourceString("ERR_IdentifierExpectedKW");

        internal static string ERR_SemiOrLBraceExpected => GetResourceString("ERR_SemiOrLBraceExpected");

        internal static string ERR_MultiTypeInDeclaration => GetResourceString("ERR_MultiTypeInDeclaration");

        internal static string ERR_AddOrRemoveExpected => GetResourceString("ERR_AddOrRemoveExpected");

        internal static string ERR_UnexpectedCharacter => GetResourceString("ERR_UnexpectedCharacter");

        internal static string ERR_UnexpectedToken => GetResourceString("ERR_UnexpectedToken");

        internal static string ERR_ProtectedInStatic => GetResourceString("ERR_ProtectedInStatic");

        internal static string WRN_UnreachableGeneralCatch => GetResourceString("WRN_UnreachableGeneralCatch");

        internal static string WRN_UnreachableGeneralCatch_Title => GetResourceString("WRN_UnreachableGeneralCatch_Title");

        internal static string WRN_UnreachableGeneralCatch_Description => GetResourceString("WRN_UnreachableGeneralCatch_Description");

        internal static string ERR_IncrementLvalueExpected => GetResourceString("ERR_IncrementLvalueExpected");

        internal static string ERR_NoSuchMemberOrExtension => GetResourceString("ERR_NoSuchMemberOrExtension");

        internal static string ERR_NoSuchMemberOrExtensionNeedUsing => GetResourceString("ERR_NoSuchMemberOrExtensionNeedUsing");

        internal static string ERR_BadThisParam => GetResourceString("ERR_BadThisParam");

        internal static string ERR_BadParameterModifiers => GetResourceString("ERR_BadParameterModifiers");

        internal static string ERR_BadTypeforThis => GetResourceString("ERR_BadTypeforThis");

        internal static string ERR_BadParamModThis => GetResourceString("ERR_BadParamModThis");

        internal static string ERR_BadExtensionMeth => GetResourceString("ERR_BadExtensionMeth");

        internal static string ERR_BadExtensionAgg => GetResourceString("ERR_BadExtensionAgg");

        internal static string ERR_DupParamMod => GetResourceString("ERR_DupParamMod");

        internal static string ERR_ExtensionMethodsDecl => GetResourceString("ERR_ExtensionMethodsDecl");

        internal static string ERR_ExtensionAttrNotFound => GetResourceString("ERR_ExtensionAttrNotFound");

        internal static string ERR_ExplicitExtension => GetResourceString("ERR_ExplicitExtension");

        internal static string ERR_ExplicitDynamicAttr => GetResourceString("ERR_ExplicitDynamicAttr");

        internal static string ERR_NoDynamicPhantomOnBaseCtor => GetResourceString("ERR_NoDynamicPhantomOnBaseCtor");

        internal static string ERR_ValueTypeExtDelegate => GetResourceString("ERR_ValueTypeExtDelegate");

        internal static string ERR_BadArgCount => GetResourceString("ERR_BadArgCount");

        internal static string ERR_BadArgType => GetResourceString("ERR_BadArgType");

        internal static string ERR_NoSourceFile => GetResourceString("ERR_NoSourceFile");

        internal static string ERR_CantRefResource => GetResourceString("ERR_CantRefResource");

        internal static string ERR_ResourceNotUnique => GetResourceString("ERR_ResourceNotUnique");

        internal static string ERR_ResourceFileNameNotUnique => GetResourceString("ERR_ResourceFileNameNotUnique");

        internal static string ERR_ImportNonAssembly => GetResourceString("ERR_ImportNonAssembly");

        internal static string ERR_RefLvalueExpected => GetResourceString("ERR_RefLvalueExpected");

        internal static string ERR_BaseInStaticMeth => GetResourceString("ERR_BaseInStaticMeth");

        internal static string ERR_BaseInBadContext => GetResourceString("ERR_BaseInBadContext");

        internal static string ERR_RbraceExpected => GetResourceString("ERR_RbraceExpected");

        internal static string ERR_LbraceExpected => GetResourceString("ERR_LbraceExpected");

        internal static string ERR_InExpected => GetResourceString("ERR_InExpected");

        internal static string ERR_InvalidPreprocExpr => GetResourceString("ERR_InvalidPreprocExpr");

        internal static string ERR_InvalidMemberDecl => GetResourceString("ERR_InvalidMemberDecl");

        internal static string ERR_MemberNeedsType => GetResourceString("ERR_MemberNeedsType");

        internal static string ERR_BadBaseType => GetResourceString("ERR_BadBaseType");

        internal static string WRN_EmptySwitch => GetResourceString("WRN_EmptySwitch");

        internal static string WRN_EmptySwitch_Title => GetResourceString("WRN_EmptySwitch_Title");

        internal static string ERR_ExpectedEndTry => GetResourceString("ERR_ExpectedEndTry");

        internal static string ERR_InvalidExprTerm => GetResourceString("ERR_InvalidExprTerm");

        internal static string ERR_BadNewExpr => GetResourceString("ERR_BadNewExpr");

        internal static string ERR_NoNamespacePrivate => GetResourceString("ERR_NoNamespacePrivate");

        internal static string ERR_BadVarDecl => GetResourceString("ERR_BadVarDecl");

        internal static string ERR_UsingAfterElements => GetResourceString("ERR_UsingAfterElements");

        internal static string ERR_BadBinOpArgs => GetResourceString("ERR_BadBinOpArgs");

        internal static string ERR_BadUnOpArgs => GetResourceString("ERR_BadUnOpArgs");

        internal static string ERR_NoVoidParameter => GetResourceString("ERR_NoVoidParameter");

        internal static string ERR_DuplicateAlias => GetResourceString("ERR_DuplicateAlias");

        internal static string ERR_BadProtectedAccess => GetResourceString("ERR_BadProtectedAccess");

        internal static string ERR_AddModuleAssembly => GetResourceString("ERR_AddModuleAssembly");

        internal static string ERR_BindToBogusProp2 => GetResourceString("ERR_BindToBogusProp2");

        internal static string ERR_BindToBogusProp1 => GetResourceString("ERR_BindToBogusProp1");

        internal static string ERR_NoVoidHere => GetResourceString("ERR_NoVoidHere");

        internal static string ERR_IndexerNeedsParam => GetResourceString("ERR_IndexerNeedsParam");

        internal static string ERR_BadArraySyntax => GetResourceString("ERR_BadArraySyntax");

        internal static string ERR_BadOperatorSyntax => GetResourceString("ERR_BadOperatorSyntax");

        internal static string ERR_MainClassNotFound => GetResourceString("ERR_MainClassNotFound");

        internal static string ERR_MainClassNotClass => GetResourceString("ERR_MainClassNotClass");

        internal static string ERR_NoMainInClass => GetResourceString("ERR_NoMainInClass");

        internal static string ERR_MainClassIsImport => GetResourceString("ERR_MainClassIsImport");

        internal static string ERR_OutputNeedsName => GetResourceString("ERR_OutputNeedsName");

        internal static string ERR_NoOutputDirectory => GetResourceString("ERR_NoOutputDirectory");

        internal static string ERR_CantHaveWin32ResAndManifest => GetResourceString("ERR_CantHaveWin32ResAndManifest");

        internal static string ERR_CantHaveWin32ResAndIcon => GetResourceString("ERR_CantHaveWin32ResAndIcon");

        internal static string ERR_CantReadResource => GetResourceString("ERR_CantReadResource");

        internal static string ERR_DocFileGen => GetResourceString("ERR_DocFileGen");

        internal static string WRN_XMLParseError => GetResourceString("WRN_XMLParseError");

        internal static string WRN_XMLParseError_Title => GetResourceString("WRN_XMLParseError_Title");

        internal static string WRN_DuplicateParamTag => GetResourceString("WRN_DuplicateParamTag");

        internal static string WRN_DuplicateParamTag_Title => GetResourceString("WRN_DuplicateParamTag_Title");

        internal static string WRN_UnmatchedParamTag => GetResourceString("WRN_UnmatchedParamTag");

        internal static string WRN_UnmatchedParamTag_Title => GetResourceString("WRN_UnmatchedParamTag_Title");

        internal static string WRN_UnmatchedParamRefTag => GetResourceString("WRN_UnmatchedParamRefTag");

        internal static string WRN_UnmatchedParamRefTag_Title => GetResourceString("WRN_UnmatchedParamRefTag_Title");

        internal static string WRN_MissingParamTag => GetResourceString("WRN_MissingParamTag");

        internal static string WRN_MissingParamTag_Title => GetResourceString("WRN_MissingParamTag_Title");

        internal static string WRN_BadXMLRef => GetResourceString("WRN_BadXMLRef");

        internal static string WRN_BadXMLRef_Title => GetResourceString("WRN_BadXMLRef_Title");

        internal static string ERR_BadStackAllocExpr => GetResourceString("ERR_BadStackAllocExpr");

        internal static string ERR_InvalidLineNumber => GetResourceString("ERR_InvalidLineNumber");

        internal static string ERR_MissingPPFile => GetResourceString("ERR_MissingPPFile");

        internal static string ERR_ExpectedPPFile => GetResourceString("ERR_ExpectedPPFile");

        internal static string ERR_ReferenceDirectiveOnlyAllowedInScripts => GetResourceString("ERR_ReferenceDirectiveOnlyAllowedInScripts");

        internal static string ERR_ForEachMissingMember => GetResourceString("ERR_ForEachMissingMember");

        internal static string ERR_AwaitForEachMissingMember => GetResourceString("ERR_AwaitForEachMissingMember");

        internal static string ERR_ForEachMissingMemberWrongAsync => GetResourceString("ERR_ForEachMissingMemberWrongAsync");

        internal static string ERR_AwaitForEachMissingMemberWrongAsync => GetResourceString("ERR_AwaitForEachMissingMemberWrongAsync");

        internal static string ERR_PossibleAsyncIteratorWithoutYield => GetResourceString("ERR_PossibleAsyncIteratorWithoutYield");

        internal static string ERR_PossibleAsyncIteratorWithoutYieldOrAwait => GetResourceString("ERR_PossibleAsyncIteratorWithoutYieldOrAwait");

        internal static string ERR_StaticLocalFunctionCannotCaptureVariable => GetResourceString("ERR_StaticLocalFunctionCannotCaptureVariable");

        internal static string ERR_StaticLocalFunctionCannotCaptureThis => GetResourceString("ERR_StaticLocalFunctionCannotCaptureThis");

        internal static string WRN_BadXMLRefParamType => GetResourceString("WRN_BadXMLRefParamType");

        internal static string WRN_BadXMLRefParamType_Title => GetResourceString("WRN_BadXMLRefParamType_Title");

        internal static string WRN_BadXMLRefReturnType => GetResourceString("WRN_BadXMLRefReturnType");

        internal static string WRN_BadXMLRefReturnType_Title => GetResourceString("WRN_BadXMLRefReturnType_Title");

        internal static string ERR_BadWin32Res => GetResourceString("ERR_BadWin32Res");

        internal static string WRN_BadXMLRefSyntax => GetResourceString("WRN_BadXMLRefSyntax");

        internal static string WRN_BadXMLRefSyntax_Title => GetResourceString("WRN_BadXMLRefSyntax_Title");

        internal static string ERR_BadModifierLocation => GetResourceString("ERR_BadModifierLocation");

        internal static string ERR_MissingArraySize => GetResourceString("ERR_MissingArraySize");

        internal static string WRN_UnprocessedXMLComment => GetResourceString("WRN_UnprocessedXMLComment");

        internal static string WRN_UnprocessedXMLComment_Title => GetResourceString("WRN_UnprocessedXMLComment_Title");

        internal static string WRN_FailedInclude => GetResourceString("WRN_FailedInclude");

        internal static string WRN_FailedInclude_Title => GetResourceString("WRN_FailedInclude_Title");

        internal static string WRN_InvalidInclude => GetResourceString("WRN_InvalidInclude");

        internal static string WRN_InvalidInclude_Title => GetResourceString("WRN_InvalidInclude_Title");

        internal static string WRN_MissingXMLComment => GetResourceString("WRN_MissingXMLComment");

        internal static string WRN_MissingXMLComment_Title => GetResourceString("WRN_MissingXMLComment_Title");

        internal static string WRN_MissingXMLComment_Description => GetResourceString("WRN_MissingXMLComment_Description");

        internal static string WRN_XMLParseIncludeError => GetResourceString("WRN_XMLParseIncludeError");

        internal static string WRN_XMLParseIncludeError_Title => GetResourceString("WRN_XMLParseIncludeError_Title");

        internal static string ERR_BadDelArgCount => GetResourceString("ERR_BadDelArgCount");

        internal static string ERR_UnexpectedSemicolon => GetResourceString("ERR_UnexpectedSemicolon");

        internal static string ERR_MethodReturnCantBeRefAny => GetResourceString("ERR_MethodReturnCantBeRefAny");

        internal static string ERR_CompileCancelled => GetResourceString("ERR_CompileCancelled");

        internal static string ERR_MethodArgCantBeRefAny => GetResourceString("ERR_MethodArgCantBeRefAny");

        internal static string ERR_AssgReadonlyLocal => GetResourceString("ERR_AssgReadonlyLocal");

        internal static string ERR_RefReadonlyLocal => GetResourceString("ERR_RefReadonlyLocal");

        internal static string ERR_CantUseRequiredAttribute => GetResourceString("ERR_CantUseRequiredAttribute");

        internal static string ERR_NoModifiersOnAccessor => GetResourceString("ERR_NoModifiersOnAccessor");

        internal static string ERR_ParamsCantBeWithModifier => GetResourceString("ERR_ParamsCantBeWithModifier");

        internal static string ERR_ReturnNotLValue => GetResourceString("ERR_ReturnNotLValue");

        internal static string ERR_MissingCoClass => GetResourceString("ERR_MissingCoClass");

        internal static string ERR_AmbiguousAttribute => GetResourceString("ERR_AmbiguousAttribute");

        internal static string ERR_BadArgExtraRef => GetResourceString("ERR_BadArgExtraRef");

        internal static string WRN_CmdOptionConflictsSource => GetResourceString("WRN_CmdOptionConflictsSource");

        internal static string WRN_CmdOptionConflictsSource_Title => GetResourceString("WRN_CmdOptionConflictsSource_Title");

        internal static string WRN_CmdOptionConflictsSource_Description => GetResourceString("WRN_CmdOptionConflictsSource_Description");

        internal static string ERR_BadCompatMode => GetResourceString("ERR_BadCompatMode");

        internal static string ERR_DelegateOnConditional => GetResourceString("ERR_DelegateOnConditional");

        internal static string ERR_CantMakeTempFile => GetResourceString("ERR_CantMakeTempFile");

        internal static string ERR_BadArgRef => GetResourceString("ERR_BadArgRef");

        internal static string ERR_YieldInAnonMeth => GetResourceString("ERR_YieldInAnonMeth");

        internal static string ERR_ReturnInIterator => GetResourceString("ERR_ReturnInIterator");

        internal static string ERR_BadIteratorArgType => GetResourceString("ERR_BadIteratorArgType");

        internal static string ERR_BadIteratorReturn => GetResourceString("ERR_BadIteratorReturn");

        internal static string ERR_BadYieldInFinally => GetResourceString("ERR_BadYieldInFinally");

        internal static string ERR_IteratorMustBeAsync => GetResourceString("ERR_IteratorMustBeAsync");

        internal static string ERR_BadYieldInTryOfCatch => GetResourceString("ERR_BadYieldInTryOfCatch");

        internal static string ERR_EmptyYield => GetResourceString("ERR_EmptyYield");

        internal static string ERR_AnonDelegateCantUse => GetResourceString("ERR_AnonDelegateCantUse");

        internal static string ERR_IllegalInnerUnsafe => GetResourceString("ERR_IllegalInnerUnsafe");

        internal static string ERR_BadYieldInCatch => GetResourceString("ERR_BadYieldInCatch");

        internal static string ERR_BadDelegateLeave => GetResourceString("ERR_BadDelegateLeave");

        internal static string ERR_IllegalSuppression => GetResourceString("ERR_IllegalSuppression");

        internal static string WRN_IllegalPragma => GetResourceString("WRN_IllegalPragma");

        internal static string WRN_IllegalPragma_Title => GetResourceString("WRN_IllegalPragma_Title");

        internal static string WRN_IllegalPPWarning => GetResourceString("WRN_IllegalPPWarning");

        internal static string WRN_IllegalPPWarning_Title => GetResourceString("WRN_IllegalPPWarning_Title");

        internal static string WRN_BadRestoreNumber => GetResourceString("WRN_BadRestoreNumber");

        internal static string WRN_BadRestoreNumber_Title => GetResourceString("WRN_BadRestoreNumber_Title");

        internal static string ERR_VarargsIterator => GetResourceString("ERR_VarargsIterator");

        internal static string ERR_UnsafeIteratorArgType => GetResourceString("ERR_UnsafeIteratorArgType");

        internal static string ERR_BadCoClassSig => GetResourceString("ERR_BadCoClassSig");

        internal static string ERR_MultipleIEnumOfT => GetResourceString("ERR_MultipleIEnumOfT");

        internal static string ERR_MultipleIAsyncEnumOfT => GetResourceString("ERR_MultipleIAsyncEnumOfT");

        internal static string ERR_FixedDimsRequired => GetResourceString("ERR_FixedDimsRequired");

        internal static string ERR_FixedNotInStruct => GetResourceString("ERR_FixedNotInStruct");

        internal static string ERR_AnonymousReturnExpected => GetResourceString("ERR_AnonymousReturnExpected");

        internal static string WRN_NonECMAFeature => GetResourceString("WRN_NonECMAFeature");

        internal static string WRN_NonECMAFeature_Title => GetResourceString("WRN_NonECMAFeature_Title");

        internal static string ERR_ExpectedVerbatimLiteral => GetResourceString("ERR_ExpectedVerbatimLiteral");

        internal static string ERR_RefReadonly => GetResourceString("ERR_RefReadonly");

        internal static string ERR_RefReadonly2 => GetResourceString("ERR_RefReadonly2");

        internal static string ERR_AssgReadonly => GetResourceString("ERR_AssgReadonly");

        internal static string ERR_AssgReadonly2 => GetResourceString("ERR_AssgReadonly2");

        internal static string ERR_RefReadonlyNotField => GetResourceString("ERR_RefReadonlyNotField");

        internal static string ERR_RefReadonlyNotField2 => GetResourceString("ERR_RefReadonlyNotField2");

        internal static string ERR_AssignReadonlyNotField => GetResourceString("ERR_AssignReadonlyNotField");

        internal static string ERR_AssignReadonlyNotField2 => GetResourceString("ERR_AssignReadonlyNotField2");

        internal static string ERR_RefReturnReadonlyNotField => GetResourceString("ERR_RefReturnReadonlyNotField");

        internal static string ERR_RefReturnReadonlyNotField2 => GetResourceString("ERR_RefReturnReadonlyNotField2");

        internal static string ERR_AssgReadonlyStatic2 => GetResourceString("ERR_AssgReadonlyStatic2");

        internal static string ERR_RefReadonlyStatic2 => GetResourceString("ERR_RefReadonlyStatic2");

        internal static string ERR_AssgReadonlyLocal2Cause => GetResourceString("ERR_AssgReadonlyLocal2Cause");

        internal static string ERR_RefReadonlyLocal2Cause => GetResourceString("ERR_RefReadonlyLocal2Cause");

        internal static string ERR_AssgReadonlyLocalCause => GetResourceString("ERR_AssgReadonlyLocalCause");

        internal static string ERR_RefReadonlyLocalCause => GetResourceString("ERR_RefReadonlyLocalCause");

        internal static string WRN_ErrorOverride => GetResourceString("WRN_ErrorOverride");

        internal static string WRN_ErrorOverride_Title => GetResourceString("WRN_ErrorOverride_Title");

        internal static string WRN_ErrorOverride_Description => GetResourceString("WRN_ErrorOverride_Description");

        internal static string ERR_AnonMethToNonDel => GetResourceString("ERR_AnonMethToNonDel");

        internal static string ERR_CantConvAnonMethParams => GetResourceString("ERR_CantConvAnonMethParams");

        internal static string ERR_CantConvAnonMethReturns => GetResourceString("ERR_CantConvAnonMethReturns");

        internal static string ERR_BadAsyncReturnExpression => GetResourceString("ERR_BadAsyncReturnExpression");

        internal static string ERR_CantConvAsyncAnonFuncReturns => GetResourceString("ERR_CantConvAsyncAnonFuncReturns");

        internal static string ERR_IllegalFixedType => GetResourceString("ERR_IllegalFixedType");

        internal static string ERR_FixedOverflow => GetResourceString("ERR_FixedOverflow");

        internal static string ERR_InvalidFixedArraySize => GetResourceString("ERR_InvalidFixedArraySize");

        internal static string ERR_FixedBufferNotFixed => GetResourceString("ERR_FixedBufferNotFixed");

        internal static string ERR_AttributeNotOnAccessor => GetResourceString("ERR_AttributeNotOnAccessor");

        internal static string WRN_InvalidSearchPathDir => GetResourceString("WRN_InvalidSearchPathDir");

        internal static string WRN_InvalidSearchPathDir_Title => GetResourceString("WRN_InvalidSearchPathDir_Title");

        internal static string ERR_IllegalVarArgs => GetResourceString("ERR_IllegalVarArgs");

        internal static string ERR_IllegalParams => GetResourceString("ERR_IllegalParams");

        internal static string ERR_BadModifiersOnNamespace => GetResourceString("ERR_BadModifiersOnNamespace");

        internal static string ERR_BadPlatformType => GetResourceString("ERR_BadPlatformType");

        internal static string ERR_ThisStructNotInAnonMeth => GetResourceString("ERR_ThisStructNotInAnonMeth");

        internal static string ERR_NoConvToIDisp => GetResourceString("ERR_NoConvToIDisp");

        internal static string ERR_NoConvToIDispWrongAsync => GetResourceString("ERR_NoConvToIDispWrongAsync");

        internal static string ERR_NoConvToIAsyncDisp => GetResourceString("ERR_NoConvToIAsyncDisp");

        internal static string ERR_NoConvToIAsyncDispWrongAsync => GetResourceString("ERR_NoConvToIAsyncDispWrongAsync");

        internal static string ERR_BadParamRef => GetResourceString("ERR_BadParamRef");

        internal static string ERR_BadParamExtraRef => GetResourceString("ERR_BadParamExtraRef");

        internal static string ERR_BadParamType => GetResourceString("ERR_BadParamType");

        internal static string ERR_BadExternIdentifier => GetResourceString("ERR_BadExternIdentifier");

        internal static string ERR_AliasMissingFile => GetResourceString("ERR_AliasMissingFile");

        internal static string ERR_GlobalExternAlias => GetResourceString("ERR_GlobalExternAlias");

        internal static string ERR_MissingTypeInSource => GetResourceString("ERR_MissingTypeInSource");

        internal static string ERR_MissingTypeInAssembly => GetResourceString("ERR_MissingTypeInAssembly");

        internal static string WRN_MultiplePredefTypes => GetResourceString("WRN_MultiplePredefTypes");

        internal static string WRN_MultiplePredefTypes_Title => GetResourceString("WRN_MultiplePredefTypes_Title");

        internal static string WRN_MultiplePredefTypes_Description => GetResourceString("WRN_MultiplePredefTypes_Description");

        internal static string ERR_LocalCantBeFixedAndHoisted => GetResourceString("ERR_LocalCantBeFixedAndHoisted");

        internal static string WRN_TooManyLinesForDebugger => GetResourceString("WRN_TooManyLinesForDebugger");

        internal static string WRN_TooManyLinesForDebugger_Title => GetResourceString("WRN_TooManyLinesForDebugger_Title");

        internal static string ERR_CantConvAnonMethNoParams => GetResourceString("ERR_CantConvAnonMethNoParams");

        internal static string ERR_ConditionalOnNonAttributeClass => GetResourceString("ERR_ConditionalOnNonAttributeClass");

        internal static string WRN_CallOnNonAgileField => GetResourceString("WRN_CallOnNonAgileField");

        internal static string WRN_CallOnNonAgileField_Title => GetResourceString("WRN_CallOnNonAgileField_Title");

        internal static string WRN_CallOnNonAgileField_Description => GetResourceString("WRN_CallOnNonAgileField_Description");

        internal static string WRN_BadWarningNumber => GetResourceString("WRN_BadWarningNumber");

        internal static string WRN_BadWarningNumber_Title => GetResourceString("WRN_BadWarningNumber_Title");

        internal static string WRN_BadWarningNumber_Description => GetResourceString("WRN_BadWarningNumber_Description");

        internal static string WRN_InvalidNumber => GetResourceString("WRN_InvalidNumber");

        internal static string WRN_InvalidNumber_Title => GetResourceString("WRN_InvalidNumber_Title");

        internal static string WRN_FileNameTooLong => GetResourceString("WRN_FileNameTooLong");

        internal static string WRN_FileNameTooLong_Title => GetResourceString("WRN_FileNameTooLong_Title");

        internal static string WRN_IllegalPPChecksum => GetResourceString("WRN_IllegalPPChecksum");

        internal static string WRN_IllegalPPChecksum_Title => GetResourceString("WRN_IllegalPPChecksum_Title");

        internal static string WRN_EndOfPPLineExpected => GetResourceString("WRN_EndOfPPLineExpected");

        internal static string WRN_EndOfPPLineExpected_Title => GetResourceString("WRN_EndOfPPLineExpected_Title");

        internal static string WRN_ConflictingChecksum => GetResourceString("WRN_ConflictingChecksum");

        internal static string WRN_ConflictingChecksum_Title => GetResourceString("WRN_ConflictingChecksum_Title");

        internal static string WRN_InvalidAssemblyName => GetResourceString("WRN_InvalidAssemblyName");

        internal static string WRN_InvalidAssemblyName_Title => GetResourceString("WRN_InvalidAssemblyName_Title");

        internal static string WRN_InvalidAssemblyName_Description => GetResourceString("WRN_InvalidAssemblyName_Description");

        internal static string WRN_UnifyReferenceMajMin => GetResourceString("WRN_UnifyReferenceMajMin");

        internal static string WRN_UnifyReferenceMajMin_Title => GetResourceString("WRN_UnifyReferenceMajMin_Title");

        internal static string WRN_UnifyReferenceMajMin_Description => GetResourceString("WRN_UnifyReferenceMajMin_Description");

        internal static string WRN_UnifyReferenceBldRev => GetResourceString("WRN_UnifyReferenceBldRev");

        internal static string WRN_UnifyReferenceBldRev_Title => GetResourceString("WRN_UnifyReferenceBldRev_Title");

        internal static string WRN_UnifyReferenceBldRev_Description => GetResourceString("WRN_UnifyReferenceBldRev_Description");

        internal static string ERR_DuplicateImport => GetResourceString("ERR_DuplicateImport");

        internal static string ERR_DuplicateImportSimple => GetResourceString("ERR_DuplicateImportSimple");

        internal static string ERR_AssemblyMatchBadVersion => GetResourceString("ERR_AssemblyMatchBadVersion");

        internal static string ERR_FixedNeedsLvalue => GetResourceString("ERR_FixedNeedsLvalue");

        internal static string WRN_DuplicateTypeParamTag => GetResourceString("WRN_DuplicateTypeParamTag");

        internal static string WRN_DuplicateTypeParamTag_Title => GetResourceString("WRN_DuplicateTypeParamTag_Title");

        internal static string WRN_UnmatchedTypeParamTag => GetResourceString("WRN_UnmatchedTypeParamTag");

        internal static string WRN_UnmatchedTypeParamTag_Title => GetResourceString("WRN_UnmatchedTypeParamTag_Title");

        internal static string WRN_UnmatchedTypeParamRefTag => GetResourceString("WRN_UnmatchedTypeParamRefTag");

        internal static string WRN_UnmatchedTypeParamRefTag_Title => GetResourceString("WRN_UnmatchedTypeParamRefTag_Title");

        internal static string WRN_MissingTypeParamTag => GetResourceString("WRN_MissingTypeParamTag");

        internal static string WRN_MissingTypeParamTag_Title => GetResourceString("WRN_MissingTypeParamTag_Title");

        internal static string ERR_CantChangeTypeOnOverride => GetResourceString("ERR_CantChangeTypeOnOverride");

        internal static string ERR_DoNotUseFixedBufferAttr => GetResourceString("ERR_DoNotUseFixedBufferAttr");

        internal static string ERR_DoNotUseFixedBufferAttrOnProperty => GetResourceString("ERR_DoNotUseFixedBufferAttrOnProperty");

        internal static string WRN_AssignmentToSelf => GetResourceString("WRN_AssignmentToSelf");

        internal static string WRN_AssignmentToSelf_Title => GetResourceString("WRN_AssignmentToSelf_Title");

        internal static string WRN_ComparisonToSelf => GetResourceString("WRN_ComparisonToSelf");

        internal static string WRN_ComparisonToSelf_Title => GetResourceString("WRN_ComparisonToSelf_Title");

        internal static string ERR_CantOpenWin32Res => GetResourceString("ERR_CantOpenWin32Res");

        internal static string WRN_DotOnDefault => GetResourceString("WRN_DotOnDefault");

        internal static string WRN_DotOnDefault_Title => GetResourceString("WRN_DotOnDefault_Title");

        internal static string ERR_NoMultipleInheritance => GetResourceString("ERR_NoMultipleInheritance");

        internal static string ERR_BaseClassMustBeFirst => GetResourceString("ERR_BaseClassMustBeFirst");

        internal static string WRN_BadXMLRefTypeVar => GetResourceString("WRN_BadXMLRefTypeVar");

        internal static string WRN_BadXMLRefTypeVar_Title => GetResourceString("WRN_BadXMLRefTypeVar_Title");

        internal static string ERR_FriendAssemblyBadArgs => GetResourceString("ERR_FriendAssemblyBadArgs");

        internal static string ERR_FriendAssemblySNReq => GetResourceString("ERR_FriendAssemblySNReq");

        internal static string ERR_DelegateOnNullable => GetResourceString("ERR_DelegateOnNullable");

        internal static string ERR_BadCtorArgCount => GetResourceString("ERR_BadCtorArgCount");

        internal static string ERR_GlobalAttributesNotFirst => GetResourceString("ERR_GlobalAttributesNotFirst");

        internal static string ERR_ExpressionExpected => GetResourceString("ERR_ExpressionExpected");

        internal static string ERR_InvalidSubsystemVersion => GetResourceString("ERR_InvalidSubsystemVersion");

        internal static string ERR_InteropMethodWithBody => GetResourceString("ERR_InteropMethodWithBody");

        internal static string ERR_BadWarningLevel => GetResourceString("ERR_BadWarningLevel");

        internal static string ERR_BadDebugType => GetResourceString("ERR_BadDebugType");

        internal static string ERR_BadResourceVis => GetResourceString("ERR_BadResourceVis");

        internal static string ERR_DefaultValueTypeMustMatch => GetResourceString("ERR_DefaultValueTypeMustMatch");

        internal static string ERR_DefaultValueBadValueType => GetResourceString("ERR_DefaultValueBadValueType");

        internal static string ERR_MemberAlreadyInitialized => GetResourceString("ERR_MemberAlreadyInitialized");

        internal static string ERR_MemberCannotBeInitialized => GetResourceString("ERR_MemberCannotBeInitialized");

        internal static string ERR_StaticMemberInObjectInitializer => GetResourceString("ERR_StaticMemberInObjectInitializer");

        internal static string ERR_ReadonlyValueTypeInObjectInitializer => GetResourceString("ERR_ReadonlyValueTypeInObjectInitializer");

        internal static string ERR_ValueTypePropertyInObjectInitializer => GetResourceString("ERR_ValueTypePropertyInObjectInitializer");

        internal static string ERR_UnsafeTypeInObjectCreation => GetResourceString("ERR_UnsafeTypeInObjectCreation");

        internal static string ERR_EmptyElementInitializer => GetResourceString("ERR_EmptyElementInitializer");

        internal static string ERR_InitializerAddHasWrongSignature => GetResourceString("ERR_InitializerAddHasWrongSignature");

        internal static string ERR_CollectionInitRequiresIEnumerable => GetResourceString("ERR_CollectionInitRequiresIEnumerable");

        internal static string ERR_CantSetWin32Manifest => GetResourceString("ERR_CantSetWin32Manifest");

        internal static string WRN_CantHaveManifestForModule => GetResourceString("WRN_CantHaveManifestForModule");

        internal static string WRN_CantHaveManifestForModule_Title => GetResourceString("WRN_CantHaveManifestForModule_Title");

        internal static string ERR_BadInstanceArgType => GetResourceString("ERR_BadInstanceArgType");

        internal static string ERR_QueryDuplicateRangeVariable => GetResourceString("ERR_QueryDuplicateRangeVariable");

        internal static string ERR_QueryRangeVariableOverrides => GetResourceString("ERR_QueryRangeVariableOverrides");

        internal static string ERR_QueryRangeVariableAssignedBadValue => GetResourceString("ERR_QueryRangeVariableAssignedBadValue");

        internal static string ERR_QueryNoProviderCastable => GetResourceString("ERR_QueryNoProviderCastable");

        internal static string ERR_QueryNoProviderStandard => GetResourceString("ERR_QueryNoProviderStandard");

        internal static string ERR_QueryNoProvider => GetResourceString("ERR_QueryNoProvider");

        internal static string ERR_QueryOuterKey => GetResourceString("ERR_QueryOuterKey");

        internal static string ERR_QueryInnerKey => GetResourceString("ERR_QueryInnerKey");

        internal static string ERR_QueryOutRefRangeVariable => GetResourceString("ERR_QueryOutRefRangeVariable");

        internal static string ERR_QueryMultipleProviders => GetResourceString("ERR_QueryMultipleProviders");

        internal static string ERR_QueryTypeInferenceFailedMulti => GetResourceString("ERR_QueryTypeInferenceFailedMulti");

        internal static string ERR_QueryTypeInferenceFailed => GetResourceString("ERR_QueryTypeInferenceFailed");

        internal static string ERR_QueryTypeInferenceFailedSelectMany => GetResourceString("ERR_QueryTypeInferenceFailedSelectMany");

        internal static string ERR_ExpressionTreeContainsPointerOp => GetResourceString("ERR_ExpressionTreeContainsPointerOp");

        internal static string ERR_ExpressionTreeContainsAnonymousMethod => GetResourceString("ERR_ExpressionTreeContainsAnonymousMethod");

        internal static string ERR_AnonymousMethodToExpressionTree => GetResourceString("ERR_AnonymousMethodToExpressionTree");

        internal static string ERR_QueryRangeVariableReadOnly => GetResourceString("ERR_QueryRangeVariableReadOnly");

        internal static string ERR_QueryRangeVariableSameAsTypeParam => GetResourceString("ERR_QueryRangeVariableSameAsTypeParam");

        internal static string ERR_TypeVarNotFoundRangeVariable => GetResourceString("ERR_TypeVarNotFoundRangeVariable");

        internal static string ERR_BadArgTypesForCollectionAdd => GetResourceString("ERR_BadArgTypesForCollectionAdd");

        internal static string ERR_ByRefParameterInExpressionTree => GetResourceString("ERR_ByRefParameterInExpressionTree");

        internal static string ERR_VarArgsInExpressionTree => GetResourceString("ERR_VarArgsInExpressionTree");

        internal static string ERR_MemGroupInExpressionTree => GetResourceString("ERR_MemGroupInExpressionTree");

        internal static string ERR_InitializerAddHasParamModifiers => GetResourceString("ERR_InitializerAddHasParamModifiers");

        internal static string ERR_NonInvocableMemberCalled => GetResourceString("ERR_NonInvocableMemberCalled");

        internal static string WRN_MultipleRuntimeImplementationMatches => GetResourceString("WRN_MultipleRuntimeImplementationMatches");

        internal static string WRN_MultipleRuntimeImplementationMatches_Title => GetResourceString("WRN_MultipleRuntimeImplementationMatches_Title");

        internal static string WRN_MultipleRuntimeImplementationMatches_Description => GetResourceString("WRN_MultipleRuntimeImplementationMatches_Description");

        internal static string WRN_MultipleRuntimeOverrideMatches => GetResourceString("WRN_MultipleRuntimeOverrideMatches");

        internal static string WRN_MultipleRuntimeOverrideMatches_Title => GetResourceString("WRN_MultipleRuntimeOverrideMatches_Title");

        internal static string ERR_ObjectOrCollectionInitializerWithDelegateCreation => GetResourceString("ERR_ObjectOrCollectionInitializerWithDelegateCreation");

        internal static string ERR_InvalidConstantDeclarationType => GetResourceString("ERR_InvalidConstantDeclarationType");

        internal static string ERR_FileNotFound => GetResourceString("ERR_FileNotFound");

        internal static string WRN_FileAlreadyIncluded => GetResourceString("WRN_FileAlreadyIncluded");

        internal static string WRN_FileAlreadyIncluded_Title => GetResourceString("WRN_FileAlreadyIncluded_Title");

        internal static string ERR_NoFileSpec => GetResourceString("ERR_NoFileSpec");

        internal static string ERR_SwitchNeedsString => GetResourceString("ERR_SwitchNeedsString");

        internal static string ERR_BadSwitch => GetResourceString("ERR_BadSwitch");

        internal static string WRN_NoSources => GetResourceString("WRN_NoSources");

        internal static string WRN_NoSources_Title => GetResourceString("WRN_NoSources_Title");

        internal static string ERR_ExpectedSingleScript => GetResourceString("ERR_ExpectedSingleScript");

        internal static string ERR_OpenResponseFile => GetResourceString("ERR_OpenResponseFile");

        internal static string ERR_CantOpenFileWrite => GetResourceString("ERR_CantOpenFileWrite");

        internal static string ERR_BadBaseNumber => GetResourceString("ERR_BadBaseNumber");

        internal static string ERR_BinaryFile => GetResourceString("ERR_BinaryFile");

        internal static string FTL_BadCodepage => GetResourceString("FTL_BadCodepage");

        internal static string FTL_BadChecksumAlgorithm => GetResourceString("FTL_BadChecksumAlgorithm");

        internal static string ERR_NoMainOnDLL => GetResourceString("ERR_NoMainOnDLL");

        internal static string FTL_InvalidTarget => GetResourceString("FTL_InvalidTarget");

        internal static string FTL_InvalidInputFileName => GetResourceString("FTL_InvalidInputFileName");

        internal static string WRN_NoConfigNotOnCommandLine => GetResourceString("WRN_NoConfigNotOnCommandLine");

        internal static string WRN_NoConfigNotOnCommandLine_Title => GetResourceString("WRN_NoConfigNotOnCommandLine_Title");

        internal static string ERR_InvalidFileAlignment => GetResourceString("ERR_InvalidFileAlignment");

        internal static string ERR_InvalidOutputName => GetResourceString("ERR_InvalidOutputName");

        internal static string ERR_InvalidDebugInformationFormat => GetResourceString("ERR_InvalidDebugInformationFormat");

        internal static string ERR_LegacyObjectIdSyntax => GetResourceString("ERR_LegacyObjectIdSyntax");

        internal static string WRN_DefineIdentifierRequired => GetResourceString("WRN_DefineIdentifierRequired");

        internal static string WRN_DefineIdentifierRequired_Title => GetResourceString("WRN_DefineIdentifierRequired_Title");

        internal static string FTL_OutputFileExists => GetResourceString("FTL_OutputFileExists");

        internal static string ERR_OneAliasPerReference => GetResourceString("ERR_OneAliasPerReference");

        internal static string ERR_SwitchNeedsNumber => GetResourceString("ERR_SwitchNeedsNumber");

        internal static string ERR_MissingDebugSwitch => GetResourceString("ERR_MissingDebugSwitch");

        internal static string ERR_ComRefCallInExpressionTree => GetResourceString("ERR_ComRefCallInExpressionTree");

        internal static string ERR_InvalidFormatForGuidForOption => GetResourceString("ERR_InvalidFormatForGuidForOption");

        internal static string ERR_MissingGuidForOption => GetResourceString("ERR_MissingGuidForOption");

        internal static string WRN_CLS_NoVarArgs => GetResourceString("WRN_CLS_NoVarArgs");

        internal static string WRN_CLS_NoVarArgs_Title => GetResourceString("WRN_CLS_NoVarArgs_Title");

        internal static string WRN_CLS_BadArgType => GetResourceString("WRN_CLS_BadArgType");

        internal static string WRN_CLS_BadArgType_Title => GetResourceString("WRN_CLS_BadArgType_Title");

        internal static string WRN_CLS_BadReturnType => GetResourceString("WRN_CLS_BadReturnType");

        internal static string WRN_CLS_BadReturnType_Title => GetResourceString("WRN_CLS_BadReturnType_Title");

        internal static string WRN_CLS_BadFieldPropType => GetResourceString("WRN_CLS_BadFieldPropType");

        internal static string WRN_CLS_BadFieldPropType_Title => GetResourceString("WRN_CLS_BadFieldPropType_Title");

        internal static string WRN_CLS_BadFieldPropType_Description => GetResourceString("WRN_CLS_BadFieldPropType_Description");

        internal static string WRN_CLS_BadIdentifierCase => GetResourceString("WRN_CLS_BadIdentifierCase");

        internal static string WRN_CLS_BadIdentifierCase_Title => GetResourceString("WRN_CLS_BadIdentifierCase_Title");

        internal static string WRN_CLS_OverloadRefOut => GetResourceString("WRN_CLS_OverloadRefOut");

        internal static string WRN_CLS_OverloadRefOut_Title => GetResourceString("WRN_CLS_OverloadRefOut_Title");

        internal static string WRN_CLS_OverloadUnnamed => GetResourceString("WRN_CLS_OverloadUnnamed");

        internal static string WRN_CLS_OverloadUnnamed_Title => GetResourceString("WRN_CLS_OverloadUnnamed_Title");

        internal static string WRN_CLS_OverloadUnnamed_Description => GetResourceString("WRN_CLS_OverloadUnnamed_Description");

        internal static string WRN_CLS_BadIdentifier => GetResourceString("WRN_CLS_BadIdentifier");

        internal static string WRN_CLS_BadIdentifier_Title => GetResourceString("WRN_CLS_BadIdentifier_Title");

        internal static string WRN_CLS_BadBase => GetResourceString("WRN_CLS_BadBase");

        internal static string WRN_CLS_BadBase_Title => GetResourceString("WRN_CLS_BadBase_Title");

        internal static string WRN_CLS_BadBase_Description => GetResourceString("WRN_CLS_BadBase_Description");

        internal static string WRN_CLS_BadInterfaceMember => GetResourceString("WRN_CLS_BadInterfaceMember");

        internal static string WRN_CLS_BadInterfaceMember_Title => GetResourceString("WRN_CLS_BadInterfaceMember_Title");

        internal static string WRN_CLS_NoAbstractMembers => GetResourceString("WRN_CLS_NoAbstractMembers");

        internal static string WRN_CLS_NoAbstractMembers_Title => GetResourceString("WRN_CLS_NoAbstractMembers_Title");

        internal static string WRN_CLS_NotOnModules => GetResourceString("WRN_CLS_NotOnModules");

        internal static string WRN_CLS_NotOnModules_Title => GetResourceString("WRN_CLS_NotOnModules_Title");

        internal static string WRN_CLS_ModuleMissingCLS => GetResourceString("WRN_CLS_ModuleMissingCLS");

        internal static string WRN_CLS_ModuleMissingCLS_Title => GetResourceString("WRN_CLS_ModuleMissingCLS_Title");

        internal static string WRN_CLS_AssemblyNotCLS => GetResourceString("WRN_CLS_AssemblyNotCLS");

        internal static string WRN_CLS_AssemblyNotCLS_Title => GetResourceString("WRN_CLS_AssemblyNotCLS_Title");

        internal static string WRN_CLS_BadAttributeType => GetResourceString("WRN_CLS_BadAttributeType");

        internal static string WRN_CLS_BadAttributeType_Title => GetResourceString("WRN_CLS_BadAttributeType_Title");

        internal static string WRN_CLS_ArrayArgumentToAttribute => GetResourceString("WRN_CLS_ArrayArgumentToAttribute");

        internal static string WRN_CLS_ArrayArgumentToAttribute_Title => GetResourceString("WRN_CLS_ArrayArgumentToAttribute_Title");

        internal static string WRN_CLS_NotOnModules2 => GetResourceString("WRN_CLS_NotOnModules2");

        internal static string WRN_CLS_NotOnModules2_Title => GetResourceString("WRN_CLS_NotOnModules2_Title");

        internal static string WRN_CLS_IllegalTrueInFalse => GetResourceString("WRN_CLS_IllegalTrueInFalse");

        internal static string WRN_CLS_IllegalTrueInFalse_Title => GetResourceString("WRN_CLS_IllegalTrueInFalse_Title");

        internal static string WRN_CLS_MeaninglessOnPrivateType => GetResourceString("WRN_CLS_MeaninglessOnPrivateType");

        internal static string WRN_CLS_MeaninglessOnPrivateType_Title => GetResourceString("WRN_CLS_MeaninglessOnPrivateType_Title");

        internal static string WRN_CLS_AssemblyNotCLS2 => GetResourceString("WRN_CLS_AssemblyNotCLS2");

        internal static string WRN_CLS_AssemblyNotCLS2_Title => GetResourceString("WRN_CLS_AssemblyNotCLS2_Title");

        internal static string WRN_CLS_MeaninglessOnParam => GetResourceString("WRN_CLS_MeaninglessOnParam");

        internal static string WRN_CLS_MeaninglessOnParam_Title => GetResourceString("WRN_CLS_MeaninglessOnParam_Title");

        internal static string WRN_CLS_MeaninglessOnReturn => GetResourceString("WRN_CLS_MeaninglessOnReturn");

        internal static string WRN_CLS_MeaninglessOnReturn_Title => GetResourceString("WRN_CLS_MeaninglessOnReturn_Title");

        internal static string WRN_CLS_BadTypeVar => GetResourceString("WRN_CLS_BadTypeVar");

        internal static string WRN_CLS_BadTypeVar_Title => GetResourceString("WRN_CLS_BadTypeVar_Title");

        internal static string WRN_CLS_VolatileField => GetResourceString("WRN_CLS_VolatileField");

        internal static string WRN_CLS_VolatileField_Title => GetResourceString("WRN_CLS_VolatileField_Title");

        internal static string WRN_CLS_BadInterface => GetResourceString("WRN_CLS_BadInterface");

        internal static string WRN_CLS_BadInterface_Title => GetResourceString("WRN_CLS_BadInterface_Title");

        internal static string ERR_BadAwaitArg => GetResourceString("ERR_BadAwaitArg");

        internal static string ERR_BadAwaitArgIntrinsic => GetResourceString("ERR_BadAwaitArgIntrinsic");

        internal static string ERR_BadAwaiterPattern => GetResourceString("ERR_BadAwaiterPattern");

        internal static string ERR_BadAwaitArg_NeedSystem => GetResourceString("ERR_BadAwaitArg_NeedSystem");

        internal static string ERR_BadAwaitArgVoidCall => GetResourceString("ERR_BadAwaitArgVoidCall");

        internal static string ERR_BadAwaitAsIdentifier => GetResourceString("ERR_BadAwaitAsIdentifier");

        internal static string ERR_DoesntImplementAwaitInterface => GetResourceString("ERR_DoesntImplementAwaitInterface");

        internal static string ERR_TaskRetNoObjectRequired => GetResourceString("ERR_TaskRetNoObjectRequired");

        internal static string ERR_BadAsyncReturn => GetResourceString("ERR_BadAsyncReturn");

        internal static string ERR_CantReturnVoid => GetResourceString("ERR_CantReturnVoid");

        internal static string ERR_VarargsAsync => GetResourceString("ERR_VarargsAsync");

        internal static string ERR_ByRefTypeAndAwait => GetResourceString("ERR_ByRefTypeAndAwait");

        internal static string ERR_UnsafeAsyncArgType => GetResourceString("ERR_UnsafeAsyncArgType");

        internal static string ERR_BadAsyncArgType => GetResourceString("ERR_BadAsyncArgType");

        internal static string ERR_BadAwaitWithoutAsync => GetResourceString("ERR_BadAwaitWithoutAsync");

        internal static string ERR_BadAwaitWithoutAsyncLambda => GetResourceString("ERR_BadAwaitWithoutAsyncLambda");

        internal static string ERR_BadAwaitWithoutAsyncMethod => GetResourceString("ERR_BadAwaitWithoutAsyncMethod");

        internal static string ERR_BadAwaitWithoutVoidAsyncMethod => GetResourceString("ERR_BadAwaitWithoutVoidAsyncMethod");

        internal static string ERR_BadAwaitInFinally => GetResourceString("ERR_BadAwaitInFinally");

        internal static string ERR_BadAwaitInCatch => GetResourceString("ERR_BadAwaitInCatch");

        internal static string ERR_BadAwaitInCatchFilter => GetResourceString("ERR_BadAwaitInCatchFilter");

        internal static string ERR_BadAwaitInLock => GetResourceString("ERR_BadAwaitInLock");

        internal static string ERR_BadAwaitInStaticVariableInitializer => GetResourceString("ERR_BadAwaitInStaticVariableInitializer");

        internal static string ERR_AwaitInUnsafeContext => GetResourceString("ERR_AwaitInUnsafeContext");

        internal static string ERR_BadAsyncLacksBody => GetResourceString("ERR_BadAsyncLacksBody");

        internal static string ERR_BadSpecialByRefLocal => GetResourceString("ERR_BadSpecialByRefLocal");

        internal static string ERR_BadSpecialByRefIterator => GetResourceString("ERR_BadSpecialByRefIterator");

        internal static string ERR_SecurityCriticalOrSecuritySafeCriticalOnAsync => GetResourceString("ERR_SecurityCriticalOrSecuritySafeCriticalOnAsync");

        internal static string ERR_SecurityCriticalOrSecuritySafeCriticalOnAsyncInClassOrStruct => GetResourceString("ERR_SecurityCriticalOrSecuritySafeCriticalOnAsyncInClassOrStruct");

        internal static string ERR_BadAwaitInQuery => GetResourceString("ERR_BadAwaitInQuery");

        internal static string WRN_AsyncLacksAwaits => GetResourceString("WRN_AsyncLacksAwaits");

        internal static string WRN_AsyncLacksAwaits_Title => GetResourceString("WRN_AsyncLacksAwaits_Title");

        internal static string WRN_UnobservedAwaitableExpression => GetResourceString("WRN_UnobservedAwaitableExpression");

        internal static string WRN_UnobservedAwaitableExpression_Title => GetResourceString("WRN_UnobservedAwaitableExpression_Title");

        internal static string WRN_UnobservedAwaitableExpression_Description => GetResourceString("WRN_UnobservedAwaitableExpression_Description");

        internal static string ERR_SynchronizedAsyncMethod => GetResourceString("ERR_SynchronizedAsyncMethod");

        internal static string ERR_NoConversionForCallerLineNumberParam => GetResourceString("ERR_NoConversionForCallerLineNumberParam");

        internal static string ERR_NoConversionForCallerFilePathParam => GetResourceString("ERR_NoConversionForCallerFilePathParam");

        internal static string ERR_NoConversionForCallerMemberNameParam => GetResourceString("ERR_NoConversionForCallerMemberNameParam");

        internal static string ERR_BadCallerLineNumberParamWithoutDefaultValue => GetResourceString("ERR_BadCallerLineNumberParamWithoutDefaultValue");

        internal static string ERR_BadCallerFilePathParamWithoutDefaultValue => GetResourceString("ERR_BadCallerFilePathParamWithoutDefaultValue");

        internal static string ERR_BadCallerMemberNameParamWithoutDefaultValue => GetResourceString("ERR_BadCallerMemberNameParamWithoutDefaultValue");

        internal static string WRN_CallerLineNumberParamForUnconsumedLocation => GetResourceString("WRN_CallerLineNumberParamForUnconsumedLocation");

        internal static string WRN_CallerLineNumberParamForUnconsumedLocation_Title => GetResourceString("WRN_CallerLineNumberParamForUnconsumedLocation_Title");

        internal static string WRN_CallerFilePathParamForUnconsumedLocation => GetResourceString("WRN_CallerFilePathParamForUnconsumedLocation");

        internal static string WRN_CallerFilePathParamForUnconsumedLocation_Title => GetResourceString("WRN_CallerFilePathParamForUnconsumedLocation_Title");

        internal static string WRN_CallerMemberNameParamForUnconsumedLocation => GetResourceString("WRN_CallerMemberNameParamForUnconsumedLocation");

        internal static string WRN_CallerMemberNameParamForUnconsumedLocation_Title => GetResourceString("WRN_CallerMemberNameParamForUnconsumedLocation_Title");

        internal static string ERR_NoEntryPoint => GetResourceString("ERR_NoEntryPoint");

        internal static string ERR_ArrayInitializerIncorrectLength => GetResourceString("ERR_ArrayInitializerIncorrectLength");

        internal static string ERR_ArrayInitializerExpected => GetResourceString("ERR_ArrayInitializerExpected");

        internal static string ERR_IllegalVarianceSyntax => GetResourceString("ERR_IllegalVarianceSyntax");

        internal static string ERR_UnexpectedAliasedName => GetResourceString("ERR_UnexpectedAliasedName");

        internal static string ERR_UnexpectedGenericName => GetResourceString("ERR_UnexpectedGenericName");

        internal static string ERR_UnexpectedUnboundGenericName => GetResourceString("ERR_UnexpectedUnboundGenericName");

        internal static string ERR_GlobalStatement => GetResourceString("ERR_GlobalStatement");

        internal static string ERR_NamedArgumentForArray => GetResourceString("ERR_NamedArgumentForArray");

        internal static string ERR_NotYetImplementedInRoslyn => GetResourceString("ERR_NotYetImplementedInRoslyn");

        internal static string ERR_DefaultValueNotAllowed => GetResourceString("ERR_DefaultValueNotAllowed");

        internal static string ERR_CantOpenIcon => GetResourceString("ERR_CantOpenIcon");

        internal static string ERR_CantOpenWin32Manifest => GetResourceString("ERR_CantOpenWin32Manifest");

        internal static string ERR_ErrorBuildingWin32Resources => GetResourceString("ERR_ErrorBuildingWin32Resources");

        internal static string ERR_DefaultValueBeforeRequiredValue => GetResourceString("ERR_DefaultValueBeforeRequiredValue");

        internal static string ERR_ExplicitImplCollisionOnRefOut => GetResourceString("ERR_ExplicitImplCollisionOnRefOut");

        internal static string ERR_PartialWrongTypeParamsVariance => GetResourceString("ERR_PartialWrongTypeParamsVariance");

        internal static string ERR_UnexpectedVariance => GetResourceString("ERR_UnexpectedVariance");

        internal static string ERR_UnexpectedVarianceStaticMember => GetResourceString("ERR_UnexpectedVarianceStaticMember");

        internal static string ERR_DeriveFromDynamic => GetResourceString("ERR_DeriveFromDynamic");

        internal static string ERR_DeriveFromConstructedDynamic => GetResourceString("ERR_DeriveFromConstructedDynamic");

        internal static string ERR_DynamicTypeAsBound => GetResourceString("ERR_DynamicTypeAsBound");

        internal static string ERR_ConstructedDynamicTypeAsBound => GetResourceString("ERR_ConstructedDynamicTypeAsBound");

        internal static string ERR_DynamicRequiredTypesMissing => GetResourceString("ERR_DynamicRequiredTypesMissing");

        internal static string ERR_MetadataNameTooLong => GetResourceString("ERR_MetadataNameTooLong");

        internal static string ERR_AttributesNotAllowed => GetResourceString("ERR_AttributesNotAllowed");

        internal static string ERR_AttributesRequireParenthesizedLambdaExpression => GetResourceString("ERR_AttributesRequireParenthesizedLambdaExpression");

        internal static string ERR_ExternAliasNotAllowed => GetResourceString("ERR_ExternAliasNotAllowed");

        internal static string WRN_IsDynamicIsConfusing => GetResourceString("WRN_IsDynamicIsConfusing");

        internal static string WRN_IsDynamicIsConfusing_Title => GetResourceString("WRN_IsDynamicIsConfusing_Title");

        internal static string ERR_YieldNotAllowedInScript => GetResourceString("ERR_YieldNotAllowedInScript");

        internal static string ERR_NamespaceNotAllowedInScript => GetResourceString("ERR_NamespaceNotAllowedInScript");

        internal static string ERR_GlobalAttributesNotAllowed => GetResourceString("ERR_GlobalAttributesNotAllowed");

        internal static string ERR_InvalidDelegateType => GetResourceString("ERR_InvalidDelegateType");

        internal static string WRN_MainIgnored => GetResourceString("WRN_MainIgnored");

        internal static string WRN_MainIgnored_Title => GetResourceString("WRN_MainIgnored_Title");

        internal static string WRN_StaticInAsOrIs => GetResourceString("WRN_StaticInAsOrIs");

        internal static string WRN_StaticInAsOrIs_Title => GetResourceString("WRN_StaticInAsOrIs_Title");

        internal static string ERR_BadVisEventType => GetResourceString("ERR_BadVisEventType");

        internal static string ERR_NamedArgumentSpecificationBeforeFixedArgument => GetResourceString("ERR_NamedArgumentSpecificationBeforeFixedArgument");

        internal static string ERR_NamedArgumentSpecificationBeforeFixedArgumentInDynamicInvocation => GetResourceString("ERR_NamedArgumentSpecificationBeforeFixedArgumentInDynamicInvocation");

        internal static string ERR_BadNamedArgument => GetResourceString("ERR_BadNamedArgument");

        internal static string ERR_BadNamedArgumentForDelegateInvoke => GetResourceString("ERR_BadNamedArgumentForDelegateInvoke");

        internal static string ERR_DuplicateNamedArgument => GetResourceString("ERR_DuplicateNamedArgument");

        internal static string ERR_NamedArgumentUsedInPositional => GetResourceString("ERR_NamedArgumentUsedInPositional");

        internal static string ERR_BadNonTrailingNamedArgument => GetResourceString("ERR_BadNonTrailingNamedArgument");

        internal static string ERR_DefaultValueUsedWithAttributes => GetResourceString("ERR_DefaultValueUsedWithAttributes");

        internal static string ERR_DefaultValueMustBeConstant => GetResourceString("ERR_DefaultValueMustBeConstant");

        internal static string ERR_RefOutDefaultValue => GetResourceString("ERR_RefOutDefaultValue");

        internal static string ERR_DefaultValueForExtensionParameter => GetResourceString("ERR_DefaultValueForExtensionParameter");

        internal static string ERR_DefaultValueForParamsParameter => GetResourceString("ERR_DefaultValueForParamsParameter");

        internal static string ERR_NoConversionForDefaultParam => GetResourceString("ERR_NoConversionForDefaultParam");

        internal static string ERR_NoConversionForNubDefaultParam => GetResourceString("ERR_NoConversionForNubDefaultParam");

        internal static string ERR_NotNullRefDefaultParameter => GetResourceString("ERR_NotNullRefDefaultParameter");

        internal static string WRN_DefaultValueForUnconsumedLocation => GetResourceString("WRN_DefaultValueForUnconsumedLocation");

        internal static string WRN_DefaultValueForUnconsumedLocation_Title => GetResourceString("WRN_DefaultValueForUnconsumedLocation_Title");

        internal static string ERR_PublicKeyFileFailure => GetResourceString("ERR_PublicKeyFileFailure");

        internal static string ERR_PublicKeyContainerFailure => GetResourceString("ERR_PublicKeyContainerFailure");

        internal static string ERR_BadDynamicTypeof => GetResourceString("ERR_BadDynamicTypeof");

        internal static string ERR_BadNullableTypeof => GetResourceString("ERR_BadNullableTypeof");

        internal static string ERR_ExpressionTreeContainsDynamicOperation => GetResourceString("ERR_ExpressionTreeContainsDynamicOperation");

        internal static string ERR_BadAsyncExpressionTree => GetResourceString("ERR_BadAsyncExpressionTree");

        internal static string ERR_DynamicAttributeMissing => GetResourceString("ERR_DynamicAttributeMissing");

        internal static string ERR_CannotPassNullForFriendAssembly => GetResourceString("ERR_CannotPassNullForFriendAssembly");

        internal static string ERR_SignButNoPrivateKey => GetResourceString("ERR_SignButNoPrivateKey");

        internal static string ERR_PublicSignButNoKey => GetResourceString("ERR_PublicSignButNoKey");

        internal static string ERR_PublicSignNetModule => GetResourceString("ERR_PublicSignNetModule");

        internal static string WRN_DelaySignButNoKey => GetResourceString("WRN_DelaySignButNoKey");

        internal static string WRN_DelaySignButNoKey_Title => GetResourceString("WRN_DelaySignButNoKey_Title");

        internal static string ERR_InvalidVersionFormat => GetResourceString("ERR_InvalidVersionFormat");

        internal static string ERR_InvalidVersionFormatDeterministic => GetResourceString("ERR_InvalidVersionFormatDeterministic");

        internal static string ERR_InvalidVersionFormat2 => GetResourceString("ERR_InvalidVersionFormat2");

        internal static string WRN_InvalidVersionFormat => GetResourceString("WRN_InvalidVersionFormat");

        internal static string WRN_InvalidVersionFormat_Title => GetResourceString("WRN_InvalidVersionFormat_Title");

        internal static string ERR_InvalidAssemblyCultureForExe => GetResourceString("ERR_InvalidAssemblyCultureForExe");

        internal static string ERR_NoCorrespondingArgument => GetResourceString("ERR_NoCorrespondingArgument");

        internal static string WRN_UnimplementedCommandLineSwitch => GetResourceString("WRN_UnimplementedCommandLineSwitch");

        internal static string WRN_UnimplementedCommandLineSwitch_Title => GetResourceString("WRN_UnimplementedCommandLineSwitch_Title");

        internal static string ERR_ModuleEmitFailure => GetResourceString("ERR_ModuleEmitFailure");

        internal static string ERR_FixedLocalInLambda => GetResourceString("ERR_FixedLocalInLambda");

        internal static string ERR_ExpressionTreeContainsNamedArgument => GetResourceString("ERR_ExpressionTreeContainsNamedArgument");

        internal static string ERR_ExpressionTreeContainsOptionalArgument => GetResourceString("ERR_ExpressionTreeContainsOptionalArgument");

        internal static string ERR_ExpressionTreeContainsIndexedProperty => GetResourceString("ERR_ExpressionTreeContainsIndexedProperty");

        internal static string ERR_IndexedPropertyRequiresParams => GetResourceString("ERR_IndexedPropertyRequiresParams");

        internal static string ERR_IndexedPropertyMustHaveAllOptionalParams => GetResourceString("ERR_IndexedPropertyMustHaveAllOptionalParams");

        internal static string ERR_SpecialByRefInLambda => GetResourceString("ERR_SpecialByRefInLambda");

        internal static string ERR_SecurityAttributeMissingAction => GetResourceString("ERR_SecurityAttributeMissingAction");

        internal static string ERR_SecurityAttributeInvalidAction => GetResourceString("ERR_SecurityAttributeInvalidAction");

        internal static string ERR_SecurityAttributeInvalidActionAssembly => GetResourceString("ERR_SecurityAttributeInvalidActionAssembly");

        internal static string ERR_SecurityAttributeInvalidActionTypeOrMethod => GetResourceString("ERR_SecurityAttributeInvalidActionTypeOrMethod");

        internal static string ERR_PrincipalPermissionInvalidAction => GetResourceString("ERR_PrincipalPermissionInvalidAction");

        internal static string ERR_FeatureNotValidInExpressionTree => GetResourceString("ERR_FeatureNotValidInExpressionTree");

        internal static string ERR_PermissionSetAttributeInvalidFile => GetResourceString("ERR_PermissionSetAttributeInvalidFile");

        internal static string ERR_PermissionSetAttributeFileReadError => GetResourceString("ERR_PermissionSetAttributeFileReadError");

        internal static string ERR_GlobalSingleTypeNameNotFoundFwd => GetResourceString("ERR_GlobalSingleTypeNameNotFoundFwd");

        internal static string ERR_DottedTypeNameNotFoundInNSFwd => GetResourceString("ERR_DottedTypeNameNotFoundInNSFwd");

        internal static string ERR_SingleTypeNameNotFoundFwd => GetResourceString("ERR_SingleTypeNameNotFoundFwd");

        internal static string ERR_AssemblySpecifiedForLinkAndRef => GetResourceString("ERR_AssemblySpecifiedForLinkAndRef");

        internal static string WRN_DeprecatedCollectionInitAdd => GetResourceString("WRN_DeprecatedCollectionInitAdd");

        internal static string WRN_DeprecatedCollectionInitAdd_Title => GetResourceString("WRN_DeprecatedCollectionInitAdd_Title");

        internal static string WRN_DeprecatedCollectionInitAddStr => GetResourceString("WRN_DeprecatedCollectionInitAddStr");

        internal static string WRN_DeprecatedCollectionInitAddStr_Title => GetResourceString("WRN_DeprecatedCollectionInitAddStr_Title");

        internal static string ERR_DeprecatedCollectionInitAddStr => GetResourceString("ERR_DeprecatedCollectionInitAddStr");

        internal static string ERR_IteratorInInteractive => GetResourceString("ERR_IteratorInInteractive");

        internal static string ERR_SecurityAttributeInvalidTarget => GetResourceString("ERR_SecurityAttributeInvalidTarget");

        internal static string ERR_BadDynamicMethodArg => GetResourceString("ERR_BadDynamicMethodArg");

        internal static string ERR_BadDynamicMethodArgLambda => GetResourceString("ERR_BadDynamicMethodArgLambda");

        internal static string ERR_BadDynamicMethodArgMemgrp => GetResourceString("ERR_BadDynamicMethodArgMemgrp");

        internal static string ERR_NoDynamicPhantomOnBase => GetResourceString("ERR_NoDynamicPhantomOnBase");

        internal static string ERR_BadDynamicQuery => GetResourceString("ERR_BadDynamicQuery");

        internal static string ERR_NoDynamicPhantomOnBaseIndexer => GetResourceString("ERR_NoDynamicPhantomOnBaseIndexer");

        internal static string WRN_DynamicDispatchToConditionalMethod => GetResourceString("WRN_DynamicDispatchToConditionalMethod");

        internal static string WRN_DynamicDispatchToConditionalMethod_Title => GetResourceString("WRN_DynamicDispatchToConditionalMethod_Title");

        internal static string ERR_BadArgTypeDynamicExtension => GetResourceString("ERR_BadArgTypeDynamicExtension");

        internal static string WRN_CallerFilePathPreferredOverCallerMemberName => GetResourceString("WRN_CallerFilePathPreferredOverCallerMemberName");

        internal static string WRN_CallerFilePathPreferredOverCallerMemberName_Title => GetResourceString("WRN_CallerFilePathPreferredOverCallerMemberName_Title");

        internal static string WRN_CallerLineNumberPreferredOverCallerMemberName => GetResourceString("WRN_CallerLineNumberPreferredOverCallerMemberName");

        internal static string WRN_CallerLineNumberPreferredOverCallerMemberName_Title => GetResourceString("WRN_CallerLineNumberPreferredOverCallerMemberName_Title");

        internal static string WRN_CallerLineNumberPreferredOverCallerFilePath => GetResourceString("WRN_CallerLineNumberPreferredOverCallerFilePath");

        internal static string WRN_CallerLineNumberPreferredOverCallerFilePath_Title => GetResourceString("WRN_CallerLineNumberPreferredOverCallerFilePath_Title");

        internal static string ERR_InvalidDynamicCondition => GetResourceString("ERR_InvalidDynamicCondition");

        internal static string ERR_MixingWinRTEventWithRegular => GetResourceString("ERR_MixingWinRTEventWithRegular");

        internal static string WRN_CA2000_DisposeObjectsBeforeLosingScope1 => GetResourceString("WRN_CA2000_DisposeObjectsBeforeLosingScope1");

        internal static string WRN_CA2000_DisposeObjectsBeforeLosingScope1_Title => GetResourceString("WRN_CA2000_DisposeObjectsBeforeLosingScope1_Title");

        internal static string WRN_CA2000_DisposeObjectsBeforeLosingScope2 => GetResourceString("WRN_CA2000_DisposeObjectsBeforeLosingScope2");

        internal static string WRN_CA2000_DisposeObjectsBeforeLosingScope2_Title => GetResourceString("WRN_CA2000_DisposeObjectsBeforeLosingScope2_Title");

        internal static string WRN_CA2202_DoNotDisposeObjectsMultipleTimes => GetResourceString("WRN_CA2202_DoNotDisposeObjectsMultipleTimes");

        internal static string WRN_CA2202_DoNotDisposeObjectsMultipleTimes_Title => GetResourceString("WRN_CA2202_DoNotDisposeObjectsMultipleTimes_Title");

        internal static string ERR_NewCoClassOnLink => GetResourceString("ERR_NewCoClassOnLink");

        internal static string ERR_NoPIANestedType => GetResourceString("ERR_NoPIANestedType");

        internal static string ERR_GenericsUsedInNoPIAType => GetResourceString("ERR_GenericsUsedInNoPIAType");

        internal static string ERR_InteropStructContainsMethods => GetResourceString("ERR_InteropStructContainsMethods");

        internal static string ERR_WinRtEventPassedByRef => GetResourceString("ERR_WinRtEventPassedByRef");

        internal static string ERR_MissingMethodOnSourceInterface => GetResourceString("ERR_MissingMethodOnSourceInterface");

        internal static string ERR_MissingSourceInterface => GetResourceString("ERR_MissingSourceInterface");

        internal static string ERR_InteropTypeMissingAttribute => GetResourceString("ERR_InteropTypeMissingAttribute");

        internal static string ERR_NoPIAAssemblyMissingAttribute => GetResourceString("ERR_NoPIAAssemblyMissingAttribute");

        internal static string ERR_NoPIAAssemblyMissingAttributes => GetResourceString("ERR_NoPIAAssemblyMissingAttributes");

        internal static string ERR_InteropTypesWithSameNameAndGuid => GetResourceString("ERR_InteropTypesWithSameNameAndGuid");

        internal static string ERR_LocalTypeNameClash => GetResourceString("ERR_LocalTypeNameClash");

        internal static string WRN_ReferencedAssemblyReferencesLinkedPIA => GetResourceString("WRN_ReferencedAssemblyReferencesLinkedPIA");

        internal static string WRN_ReferencedAssemblyReferencesLinkedPIA_Title => GetResourceString("WRN_ReferencedAssemblyReferencesLinkedPIA_Title");

        internal static string WRN_ReferencedAssemblyReferencesLinkedPIA_Description => GetResourceString("WRN_ReferencedAssemblyReferencesLinkedPIA_Description");

        internal static string ERR_GenericsUsedAcrossAssemblies => GetResourceString("ERR_GenericsUsedAcrossAssemblies");

        internal static string ERR_NoCanonicalView => GetResourceString("ERR_NoCanonicalView");

        internal static string ERR_NetModuleNameMismatch => GetResourceString("ERR_NetModuleNameMismatch");

        internal static string ERR_BadModuleName => GetResourceString("ERR_BadModuleName");

        internal static string ERR_BadCompilationOptionValue => GetResourceString("ERR_BadCompilationOptionValue");

        internal static string ERR_BadAppConfigPath => GetResourceString("ERR_BadAppConfigPath");

        internal static string WRN_AssemblyAttributeFromModuleIsOverridden => GetResourceString("WRN_AssemblyAttributeFromModuleIsOverridden");

        internal static string WRN_AssemblyAttributeFromModuleIsOverridden_Title => GetResourceString("WRN_AssemblyAttributeFromModuleIsOverridden_Title");

        internal static string ERR_CmdOptionConflictsSource => GetResourceString("ERR_CmdOptionConflictsSource");

        internal static string ERR_FixedBufferTooManyDimensions => GetResourceString("ERR_FixedBufferTooManyDimensions");

        internal static string WRN_ReferencedAssemblyDoesNotHaveStrongName => GetResourceString("WRN_ReferencedAssemblyDoesNotHaveStrongName");

        internal static string WRN_ReferencedAssemblyDoesNotHaveStrongName_Title => GetResourceString("WRN_ReferencedAssemblyDoesNotHaveStrongName_Title");

        internal static string ERR_InvalidSignaturePublicKey => GetResourceString("ERR_InvalidSignaturePublicKey");

        internal static string ERR_ExportedTypeConflictsWithDeclaration => GetResourceString("ERR_ExportedTypeConflictsWithDeclaration");

        internal static string ERR_ExportedTypesConflict => GetResourceString("ERR_ExportedTypesConflict");

        internal static string ERR_ForwardedTypeConflictsWithDeclaration => GetResourceString("ERR_ForwardedTypeConflictsWithDeclaration");

        internal static string ERR_ForwardedTypesConflict => GetResourceString("ERR_ForwardedTypesConflict");

        internal static string ERR_ForwardedTypeConflictsWithExportedType => GetResourceString("ERR_ForwardedTypeConflictsWithExportedType");

        internal static string WRN_RefCultureMismatch => GetResourceString("WRN_RefCultureMismatch");

        internal static string WRN_RefCultureMismatch_Title => GetResourceString("WRN_RefCultureMismatch_Title");

        internal static string ERR_AgnosticToMachineModule => GetResourceString("ERR_AgnosticToMachineModule");

        internal static string ERR_ConflictingMachineModule => GetResourceString("ERR_ConflictingMachineModule");

        internal static string WRN_ConflictingMachineAssembly => GetResourceString("WRN_ConflictingMachineAssembly");

        internal static string WRN_ConflictingMachineAssembly_Title => GetResourceString("WRN_ConflictingMachineAssembly_Title");

        internal static string ERR_CryptoHashFailed => GetResourceString("ERR_CryptoHashFailed");

        internal static string ERR_MissingNetModuleReference => GetResourceString("ERR_MissingNetModuleReference");

        internal static string ERR_NetModuleNameMustBeUnique => GetResourceString("ERR_NetModuleNameMustBeUnique");

        internal static string ERR_CantReadConfigFile => GetResourceString("ERR_CantReadConfigFile");

        internal static string ERR_EncNoPIAReference => GetResourceString("ERR_EncNoPIAReference");

        internal static string ERR_EncReferenceToAddedMember => GetResourceString("ERR_EncReferenceToAddedMember");

        internal static string ERR_MutuallyExclusiveOptions => GetResourceString("ERR_MutuallyExclusiveOptions");

        internal static string ERR_LinkedNetmoduleMetadataMustProvideFullPEImage => GetResourceString("ERR_LinkedNetmoduleMetadataMustProvideFullPEImage");

        internal static string ERR_BadPrefer32OnLib => GetResourceString("ERR_BadPrefer32OnLib");

        internal static string IDS_PathList => GetResourceString("IDS_PathList");

        internal static string IDS_Text => GetResourceString("IDS_Text");

        internal static string IDS_FeatureNullPropagatingOperator => GetResourceString("IDS_FeatureNullPropagatingOperator");

        internal static string IDS_FeatureExpressionBodiedMethod => GetResourceString("IDS_FeatureExpressionBodiedMethod");

        internal static string IDS_FeatureExpressionBodiedProperty => GetResourceString("IDS_FeatureExpressionBodiedProperty");

        internal static string IDS_FeatureExpressionBodiedIndexer => GetResourceString("IDS_FeatureExpressionBodiedIndexer");

        internal static string IDS_FeatureAutoPropertyInitializer => GetResourceString("IDS_FeatureAutoPropertyInitializer");

        internal static string IDS_Namespace1 => GetResourceString("IDS_Namespace1");

        internal static string IDS_FeatureRefLocalsReturns => GetResourceString("IDS_FeatureRefLocalsReturns");

        internal static string IDS_FeatureReadOnlyReferences => GetResourceString("IDS_FeatureReadOnlyReferences");

        internal static string IDS_FeatureRefStructs => GetResourceString("IDS_FeatureRefStructs");

        internal static string IDS_FeatureRefConditional => GetResourceString("IDS_FeatureRefConditional");

        internal static string IDS_FeatureRefReassignment => GetResourceString("IDS_FeatureRefReassignment");

        internal static string IDS_FeatureRefFor => GetResourceString("IDS_FeatureRefFor");

        internal static string IDS_FeatureRefForEach => GetResourceString("IDS_FeatureRefForEach");

        internal static string IDS_FeatureExtensibleFixedStatement => GetResourceString("IDS_FeatureExtensibleFixedStatement");

        internal static string CompilationC => GetResourceString("CompilationC");

        internal static string SyntaxNodeIsNotWithinSynt => GetResourceString("SyntaxNodeIsNotWithinSynt");

        internal static string LocationMustBeProvided => GetResourceString("LocationMustBeProvided");

        internal static string SyntaxTreeSemanticModelMust => GetResourceString("SyntaxTreeSemanticModelMust");

        internal static string CantReferenceCompilationOf => GetResourceString("CantReferenceCompilationOf");

        internal static string SyntaxTreeAlreadyPresent => GetResourceString("SyntaxTreeAlreadyPresent");

        internal static string SubmissionCanOnlyInclude => GetResourceString("SubmissionCanOnlyInclude");

        internal static string SubmissionCanHaveAtMostOne => GetResourceString("SubmissionCanHaveAtMostOne");

        internal static string SyntaxTreeNotFoundToRemove => GetResourceString("SyntaxTreeNotFoundToRemove");

        internal static string TreeMustHaveARootNodeWith => GetResourceString("TreeMustHaveARootNodeWith");

        internal static string TypeArgumentCannotBeNull => GetResourceString("TypeArgumentCannotBeNull");

        internal static string WrongNumberOfTypeArguments => GetResourceString("WrongNumberOfTypeArguments");

        internal static string NameConflictForName => GetResourceString("NameConflictForName");

        internal static string LookupOptionsHasInvalidCombo => GetResourceString("LookupOptionsHasInvalidCombo");

        internal static string ItemsMustBeNonEmpty => GetResourceString("ItemsMustBeNonEmpty");

        internal static string UseVerbatimIdentifier => GetResourceString("UseVerbatimIdentifier");

        internal static string UseLiteralForTokens => GetResourceString("UseLiteralForTokens");

        internal static string UseLiteralForNumeric => GetResourceString("UseLiteralForNumeric");

        internal static string ThisMethodCanOnlyBeUsedToCreateTokens => GetResourceString("ThisMethodCanOnlyBeUsedToCreateTokens");

        internal static string GenericParameterDefinition => GetResourceString("GenericParameterDefinition");

        internal static string InvalidGetDeclarationNameMultipleDeclarators => GetResourceString("InvalidGetDeclarationNameMultipleDeclarators");

        internal static string TreeNotPartOfCompilation => GetResourceString("TreeNotPartOfCompilation");

        internal static string PositionIsNotWithinSyntax => GetResourceString("PositionIsNotWithinSyntax");

        internal static string WRN_BadUILang => GetResourceString("WRN_BadUILang");

        internal static string WRN_BadUILang_Title => GetResourceString("WRN_BadUILang_Title");

        internal static string ERR_UnsupportedTransparentIdentifierAccess => GetResourceString("ERR_UnsupportedTransparentIdentifierAccess");

        internal static string ERR_ParamDefaultValueDiffersFromAttribute => GetResourceString("ERR_ParamDefaultValueDiffersFromAttribute");

        internal static string ERR_FieldHasMultipleDistinctConstantValues => GetResourceString("ERR_FieldHasMultipleDistinctConstantValues");

        internal static string WRN_UnqualifiedNestedTypeInCref => GetResourceString("WRN_UnqualifiedNestedTypeInCref");

        internal static string WRN_UnqualifiedNestedTypeInCref_Title => GetResourceString("WRN_UnqualifiedNestedTypeInCref_Title");

        internal static string NotACSharpSymbol => GetResourceString("NotACSharpSymbol");

        internal static string HDN_UnusedUsingDirective => GetResourceString("HDN_UnusedUsingDirective");

        internal static string HDN_UnusedExternAlias => GetResourceString("HDN_UnusedExternAlias");

        internal static string ElementsCannotBeNull => GetResourceString("ElementsCannotBeNull");

        internal static string IDS_LIB_ENV => GetResourceString("IDS_LIB_ENV");

        internal static string IDS_LIB_OPTION => GetResourceString("IDS_LIB_OPTION");

        internal static string IDS_REFERENCEPATH_OPTION => GetResourceString("IDS_REFERENCEPATH_OPTION");

        internal static string IDS_DirectoryDoesNotExist => GetResourceString("IDS_DirectoryDoesNotExist");

        internal static string IDS_DirectoryHasInvalidPath => GetResourceString("IDS_DirectoryHasInvalidPath");

        internal static string WRN_NoRuntimeMetadataVersion => GetResourceString("WRN_NoRuntimeMetadataVersion");

        internal static string WRN_NoRuntimeMetadataVersion_Title => GetResourceString("WRN_NoRuntimeMetadataVersion_Title");

        internal static string WrongSemanticModelType => GetResourceString("WrongSemanticModelType");

        internal static string IDS_FeatureLambda => GetResourceString("IDS_FeatureLambda");

        internal static string ERR_FeatureNotAvailableInVersion1 => GetResourceString("ERR_FeatureNotAvailableInVersion1");

        internal static string ERR_FeatureNotAvailableInVersion2 => GetResourceString("ERR_FeatureNotAvailableInVersion2");

        internal static string ERR_FeatureNotAvailableInVersion3 => GetResourceString("ERR_FeatureNotAvailableInVersion3");

        internal static string ERR_FeatureNotAvailableInVersion4 => GetResourceString("ERR_FeatureNotAvailableInVersion4");

        internal static string ERR_FeatureNotAvailableInVersion5 => GetResourceString("ERR_FeatureNotAvailableInVersion5");

        internal static string ERR_FeatureNotAvailableInVersion6 => GetResourceString("ERR_FeatureNotAvailableInVersion6");

        internal static string ERR_FeatureNotAvailableInVersion7 => GetResourceString("ERR_FeatureNotAvailableInVersion7");

        internal static string ERR_FeatureIsExperimental => GetResourceString("ERR_FeatureIsExperimental");

        internal static string IDS_VersionExperimental => GetResourceString("IDS_VersionExperimental");

        internal static string PositionNotWithinTree => GetResourceString("PositionNotWithinTree");

        internal static string SpeculatedSyntaxNodeCannotBelongToCurrentCompilation => GetResourceString("SpeculatedSyntaxNodeCannotBelongToCurrentCompilation");

        internal static string ChainingSpeculativeModelIsNotSupported => GetResourceString("ChainingSpeculativeModelIsNotSupported");

        internal static string IDS_ToolName => GetResourceString("IDS_ToolName");

        internal static string IDS_LogoLine1 => GetResourceString("IDS_LogoLine1");

        internal static string IDS_LogoLine2 => GetResourceString("IDS_LogoLine2");

        internal static string IDS_LangVersions => GetResourceString("IDS_LangVersions");

        internal static string IDS_CSCHelp => GetResourceString("IDS_CSCHelp");

        internal static string ERR_ComImportWithInitializers => GetResourceString("ERR_ComImportWithInitializers");

        internal static string WRN_PdbLocalNameTooLong => GetResourceString("WRN_PdbLocalNameTooLong");

        internal static string WRN_PdbLocalNameTooLong_Title => GetResourceString("WRN_PdbLocalNameTooLong_Title");

        internal static string ERR_RetNoObjectRequiredLambda => GetResourceString("ERR_RetNoObjectRequiredLambda");

        internal static string ERR_TaskRetNoObjectRequiredLambda => GetResourceString("ERR_TaskRetNoObjectRequiredLambda");

        internal static string WRN_AnalyzerCannotBeCreated => GetResourceString("WRN_AnalyzerCannotBeCreated");

        internal static string WRN_AnalyzerCannotBeCreated_Title => GetResourceString("WRN_AnalyzerCannotBeCreated_Title");

        internal static string WRN_NoAnalyzerInAssembly => GetResourceString("WRN_NoAnalyzerInAssembly");

        internal static string WRN_NoAnalyzerInAssembly_Title => GetResourceString("WRN_NoAnalyzerInAssembly_Title");

        internal static string WRN_UnableToLoadAnalyzer => GetResourceString("WRN_UnableToLoadAnalyzer");

        internal static string WRN_UnableToLoadAnalyzer_Title => GetResourceString("WRN_UnableToLoadAnalyzer_Title");

        internal static string INF_UnableToLoadSomeTypesInAnalyzer => GetResourceString("INF_UnableToLoadSomeTypesInAnalyzer");

        internal static string ERR_CantReadRulesetFile => GetResourceString("ERR_CantReadRulesetFile");

        internal static string ERR_BadPdbData => GetResourceString("ERR_BadPdbData");

        internal static string IDS_OperationCausedStackOverflow => GetResourceString("IDS_OperationCausedStackOverflow");

        internal static string WRN_IdentifierOrNumericLiteralExpected => GetResourceString("WRN_IdentifierOrNumericLiteralExpected");

        internal static string WRN_IdentifierOrNumericLiteralExpected_Title => GetResourceString("WRN_IdentifierOrNumericLiteralExpected_Title");

        internal static string ERR_InitializerOnNonAutoProperty => GetResourceString("ERR_InitializerOnNonAutoProperty");

        internal static string ERR_InstancePropertyInitializerInInterface => GetResourceString("ERR_InstancePropertyInitializerInInterface");

        internal static string ERR_AutoPropertyMustHaveGetAccessor => GetResourceString("ERR_AutoPropertyMustHaveGetAccessor");

        internal static string ERR_AutoPropertyMustOverrideSet => GetResourceString("ERR_AutoPropertyMustOverrideSet");

        internal static string ERR_InitializerInStructWithoutExplicitConstructor => GetResourceString("ERR_InitializerInStructWithoutExplicitConstructor");

        internal static string ERR_EncodinglessSyntaxTree => GetResourceString("ERR_EncodinglessSyntaxTree");

        internal static string ERR_BlockBodyAndExpressionBody => GetResourceString("ERR_BlockBodyAndExpressionBody");

        internal static string ERR_SwitchFallOut => GetResourceString("ERR_SwitchFallOut");

        internal static string ERR_UnexpectedBoundGenericName => GetResourceString("ERR_UnexpectedBoundGenericName");

        internal static string ERR_NullPropagatingOpInExpressionTree => GetResourceString("ERR_NullPropagatingOpInExpressionTree");

        internal static string ERR_DictionaryInitializerInExpressionTree => GetResourceString("ERR_DictionaryInitializerInExpressionTree");

        internal static string ERR_ExtensionCollectionElementInitializerInExpressionTree => GetResourceString("ERR_ExtensionCollectionElementInitializerInExpressionTree");

        internal static string IDS_FeatureNameof => GetResourceString("IDS_FeatureNameof");

        internal static string IDS_FeatureDictionaryInitializer => GetResourceString("IDS_FeatureDictionaryInitializer");

        internal static string ERR_UnclosedExpressionHole => GetResourceString("ERR_UnclosedExpressionHole");

        internal static string ERR_SingleLineCommentInExpressionHole => GetResourceString("ERR_SingleLineCommentInExpressionHole");

        internal static string ERR_InsufficientStack => GetResourceString("ERR_InsufficientStack");

        internal static string ERR_ExpressionHasNoName => GetResourceString("ERR_ExpressionHasNoName");

        internal static string ERR_SubexpressionNotInNameof => GetResourceString("ERR_SubexpressionNotInNameof");

        internal static string ERR_AliasQualifiedNameNotAnExpression => GetResourceString("ERR_AliasQualifiedNameNotAnExpression");

        internal static string ERR_NameofMethodGroupWithTypeParameters => GetResourceString("ERR_NameofMethodGroupWithTypeParameters");

        internal static string NoNoneSearchCriteria => GetResourceString("NoNoneSearchCriteria");

        internal static string ERR_InvalidAssemblyCulture => GetResourceString("ERR_InvalidAssemblyCulture");

        internal static string IDS_FeatureUsingStatic => GetResourceString("IDS_FeatureUsingStatic");

        internal static string IDS_FeatureInterpolatedStrings => GetResourceString("IDS_FeatureInterpolatedStrings");

        internal static string IDS_FeatureAltInterpolatedVerbatimStrings => GetResourceString("IDS_FeatureAltInterpolatedVerbatimStrings");

        internal static string IDS_AwaitInCatchAndFinally => GetResourceString("IDS_AwaitInCatchAndFinally");

        internal static string IDS_FeatureBinaryLiteral => GetResourceString("IDS_FeatureBinaryLiteral");

        internal static string IDS_FeatureDigitSeparator => GetResourceString("IDS_FeatureDigitSeparator");

        internal static string IDS_FeatureLocalFunctions => GetResourceString("IDS_FeatureLocalFunctions");

        internal static string ERR_UnescapedCurly => GetResourceString("ERR_UnescapedCurly");

        internal static string ERR_EscapedCurly => GetResourceString("ERR_EscapedCurly");

        internal static string ERR_TrailingWhitespaceInFormatSpecifier => GetResourceString("ERR_TrailingWhitespaceInFormatSpecifier");

        internal static string ERR_EmptyFormatSpecifier => GetResourceString("ERR_EmptyFormatSpecifier");

        internal static string ERR_ErrorInReferencedAssembly => GetResourceString("ERR_ErrorInReferencedAssembly");

        internal static string ERR_ExpressionOrDeclarationExpected => GetResourceString("ERR_ExpressionOrDeclarationExpected");

        internal static string ERR_NameofExtensionMethod => GetResourceString("ERR_NameofExtensionMethod");

        internal static string WRN_AlignmentMagnitude => GetResourceString("WRN_AlignmentMagnitude");

        internal static string HDN_UnusedExternAlias_Title => GetResourceString("HDN_UnusedExternAlias_Title");

        internal static string HDN_UnusedUsingDirective_Title => GetResourceString("HDN_UnusedUsingDirective_Title");

        internal static string INF_UnableToLoadSomeTypesInAnalyzer_Title => GetResourceString("INF_UnableToLoadSomeTypesInAnalyzer_Title");

        internal static string WRN_AlignmentMagnitude_Title => GetResourceString("WRN_AlignmentMagnitude_Title");

        internal static string ERR_ConstantStringTooLong => GetResourceString("ERR_ConstantStringTooLong");

        internal static string ERR_TupleTooFewElements => GetResourceString("ERR_TupleTooFewElements");

        internal static string ERR_DebugEntryPointNotSourceMethodDefinition => GetResourceString("ERR_DebugEntryPointNotSourceMethodDefinition");

        internal static string ERR_LoadDirectiveOnlyAllowedInScripts => GetResourceString("ERR_LoadDirectiveOnlyAllowedInScripts");

        internal static string ERR_PPLoadFollowsToken => GetResourceString("ERR_PPLoadFollowsToken");

        internal static string CouldNotFindFile => GetResourceString("CouldNotFindFile");

        internal static string SyntaxTreeFromLoadNoRemoveReplace => GetResourceString("SyntaxTreeFromLoadNoRemoveReplace");

        internal static string ERR_SourceFileReferencesNotSupported => GetResourceString("ERR_SourceFileReferencesNotSupported");

        internal static string ERR_InvalidPathMap => GetResourceString("ERR_InvalidPathMap");

        internal static string ERR_InvalidReal => GetResourceString("ERR_InvalidReal");

        internal static string ERR_AutoPropertyCannotBeRefReturning => GetResourceString("ERR_AutoPropertyCannotBeRefReturning");

        internal static string ERR_RefPropertyMustHaveGetAccessor => GetResourceString("ERR_RefPropertyMustHaveGetAccessor");

        internal static string ERR_RefPropertyCannotHaveSetAccessor => GetResourceString("ERR_RefPropertyCannotHaveSetAccessor");

        internal static string ERR_CantChangeRefReturnOnOverride => GetResourceString("ERR_CantChangeRefReturnOnOverride");

        internal static string ERR_CantChangeInitOnlyOnOverride => GetResourceString("ERR_CantChangeInitOnlyOnOverride");

        internal static string ERR_MustNotHaveRefReturn => GetResourceString("ERR_MustNotHaveRefReturn");

        internal static string ERR_MustHaveRefReturn => GetResourceString("ERR_MustHaveRefReturn");

        internal static string ERR_RefReturnMustHaveIdentityConversion => GetResourceString("ERR_RefReturnMustHaveIdentityConversion");

        internal static string ERR_CloseUnimplementedInterfaceMemberWrongRefReturn => GetResourceString("ERR_CloseUnimplementedInterfaceMemberWrongRefReturn");

        internal static string ERR_CloseUnimplementedInterfaceMemberWrongInitOnly => GetResourceString("ERR_CloseUnimplementedInterfaceMemberWrongInitOnly");

        internal static string ERR_BadIteratorReturnRef => GetResourceString("ERR_BadIteratorReturnRef");

        internal static string ERR_BadRefReturnExpressionTree => GetResourceString("ERR_BadRefReturnExpressionTree");

        internal static string ERR_RefReturningCallInExpressionTree => GetResourceString("ERR_RefReturningCallInExpressionTree");

        internal static string ERR_RefReturnLvalueExpected => GetResourceString("ERR_RefReturnLvalueExpected");

        internal static string ERR_RefReturnNonreturnableLocal => GetResourceString("ERR_RefReturnNonreturnableLocal");

        internal static string ERR_RefReturnNonreturnableLocal2 => GetResourceString("ERR_RefReturnNonreturnableLocal2");

        internal static string ERR_RefReturnReadonlyLocal => GetResourceString("ERR_RefReturnReadonlyLocal");

        internal static string ERR_RefReturnRangeVariable => GetResourceString("ERR_RefReturnRangeVariable");

        internal static string ERR_RefReturnReadonlyLocalCause => GetResourceString("ERR_RefReturnReadonlyLocalCause");

        internal static string ERR_RefReturnReadonlyLocal2Cause => GetResourceString("ERR_RefReturnReadonlyLocal2Cause");

        internal static string ERR_RefReturnReadonly => GetResourceString("ERR_RefReturnReadonly");

        internal static string ERR_RefReturnReadonlyStatic => GetResourceString("ERR_RefReturnReadonlyStatic");

        internal static string ERR_RefReturnReadonly2 => GetResourceString("ERR_RefReturnReadonly2");

        internal static string ERR_RefReturnReadonlyStatic2 => GetResourceString("ERR_RefReturnReadonlyStatic2");

        internal static string ERR_RefReturnParameter => GetResourceString("ERR_RefReturnParameter");

        internal static string ERR_RefReturnParameter2 => GetResourceString("ERR_RefReturnParameter2");

        internal static string ERR_RefReturnLocal => GetResourceString("ERR_RefReturnLocal");

        internal static string ERR_RefReturnLocal2 => GetResourceString("ERR_RefReturnLocal2");

        internal static string ERR_RefReturnStructThis => GetResourceString("ERR_RefReturnStructThis");

        internal static string ERR_EscapeOther => GetResourceString("ERR_EscapeOther");

        internal static string ERR_EscapeLocal => GetResourceString("ERR_EscapeLocal");

        internal static string ERR_EscapeCall => GetResourceString("ERR_EscapeCall");

        internal static string ERR_EscapeCall2 => GetResourceString("ERR_EscapeCall2");

        internal static string ERR_CallArgMixing => GetResourceString("ERR_CallArgMixing");

        internal static string ERR_MismatchedRefEscapeInTernary => GetResourceString("ERR_MismatchedRefEscapeInTernary");

        internal static string ERR_EscapeStackAlloc => GetResourceString("ERR_EscapeStackAlloc");

        internal static string ERR_InitializeByValueVariableWithReference => GetResourceString("ERR_InitializeByValueVariableWithReference");

        internal static string ERR_InitializeByReferenceVariableWithValue => GetResourceString("ERR_InitializeByReferenceVariableWithValue");

        internal static string ERR_RefAssignmentMustHaveIdentityConversion => GetResourceString("ERR_RefAssignmentMustHaveIdentityConversion");

        internal static string ERR_ByReferenceVariableMustBeInitialized => GetResourceString("ERR_ByReferenceVariableMustBeInitialized");

        internal static string ERR_AnonDelegateCantUseLocal => GetResourceString("ERR_AnonDelegateCantUseLocal");

        internal static string ERR_BadIteratorLocalType => GetResourceString("ERR_BadIteratorLocalType");

        internal static string ERR_BadAsyncLocalType => GetResourceString("ERR_BadAsyncLocalType");

        internal static string ERR_RefReturningCallAndAwait => GetResourceString("ERR_RefReturningCallAndAwait");

        internal static string ERR_RefConditionalAndAwait => GetResourceString("ERR_RefConditionalAndAwait");

        internal static string ERR_RefConditionalNeedsTwoRefs => GetResourceString("ERR_RefConditionalNeedsTwoRefs");

        internal static string ERR_RefConditionalDifferentTypes => GetResourceString("ERR_RefConditionalDifferentTypes");

        internal static string ERR_ExpressionTreeContainsLocalFunction => GetResourceString("ERR_ExpressionTreeContainsLocalFunction");

        internal static string ERR_DynamicLocalFunctionParamsParameter => GetResourceString("ERR_DynamicLocalFunctionParamsParameter");

        internal static string SyntaxTreeIsNotASubmission => GetResourceString("SyntaxTreeIsNotASubmission");

        internal static string ERR_TooManyUserStrings => GetResourceString("ERR_TooManyUserStrings");

        internal static string ERR_PatternNullableType => GetResourceString("ERR_PatternNullableType");

        internal static string ERR_IsNullableType => GetResourceString("ERR_IsNullableType");

        internal static string ERR_AsNullableType => GetResourceString("ERR_AsNullableType");

        internal static string ERR_BadPatternExpression => GetResourceString("ERR_BadPatternExpression");

        internal static string ERR_PeWritingFailure => GetResourceString("ERR_PeWritingFailure");

        internal static string ERR_TupleDuplicateElementName => GetResourceString("ERR_TupleDuplicateElementName");

        internal static string ERR_TupleReservedElementName => GetResourceString("ERR_TupleReservedElementName");

        internal static string ERR_TupleReservedElementNameAnyPosition => GetResourceString("ERR_TupleReservedElementNameAnyPosition");

        internal static string ERR_PredefinedTypeMemberNotFoundInAssembly => GetResourceString("ERR_PredefinedTypeMemberNotFoundInAssembly");

        internal static string IDS_FeatureTuples => GetResourceString("IDS_FeatureTuples");

        internal static string ERR_MissingDeconstruct => GetResourceString("ERR_MissingDeconstruct");

        internal static string ERR_DeconstructRequiresExpression => GetResourceString("ERR_DeconstructRequiresExpression");

        internal static string ERR_SwitchExpressionValueExpected => GetResourceString("ERR_SwitchExpressionValueExpected");

        internal static string ERR_SwitchCaseSubsumed => GetResourceString("ERR_SwitchCaseSubsumed");

        internal static string ERR_StdInOptionProvidedButConsoleInputIsNotRedirected => GetResourceString("ERR_StdInOptionProvidedButConsoleInputIsNotRedirected");

        internal static string ERR_SwitchArmSubsumed => GetResourceString("ERR_SwitchArmSubsumed");

        internal static string ERR_PatternWrongType => GetResourceString("ERR_PatternWrongType");

        internal static string ERR_ConstantPatternVsOpenType => GetResourceString("ERR_ConstantPatternVsOpenType");

        internal static string WRN_AttributeIgnoredWhenPublicSigning => GetResourceString("WRN_AttributeIgnoredWhenPublicSigning");

        internal static string WRN_AttributeIgnoredWhenPublicSigning_Title => GetResourceString("WRN_AttributeIgnoredWhenPublicSigning_Title");

        internal static string ERR_OptionMustBeAbsolutePath => GetResourceString("ERR_OptionMustBeAbsolutePath");

        internal static string ERR_ConversionNotTupleCompatible => GetResourceString("ERR_ConversionNotTupleCompatible");

        internal static string IDS_FeatureOutVar => GetResourceString("IDS_FeatureOutVar");

        internal static string ERR_ImplicitlyTypedOutVariableUsedInTheSameArgumentList => GetResourceString("ERR_ImplicitlyTypedOutVariableUsedInTheSameArgumentList");

        internal static string ERR_TypeInferenceFailedForImplicitlyTypedOutVariable => GetResourceString("ERR_TypeInferenceFailedForImplicitlyTypedOutVariable");

        internal static string ERR_TypeInferenceFailedForImplicitlyTypedDeconstructionVariable => GetResourceString("ERR_TypeInferenceFailedForImplicitlyTypedDeconstructionVariable");

        internal static string ERR_DiscardTypeInferenceFailed => GetResourceString("ERR_DiscardTypeInferenceFailed");

        internal static string ERR_DeconstructWrongCardinality => GetResourceString("ERR_DeconstructWrongCardinality");

        internal static string ERR_CannotDeconstructDynamic => GetResourceString("ERR_CannotDeconstructDynamic");

        internal static string ERR_DeconstructTooFewElements => GetResourceString("ERR_DeconstructTooFewElements");

        internal static string WRN_TupleLiteralNameMismatch => GetResourceString("WRN_TupleLiteralNameMismatch");

        internal static string WRN_TupleLiteralNameMismatch_Title => GetResourceString("WRN_TupleLiteralNameMismatch_Title");

        internal static string WRN_TupleBinopLiteralNameMismatch => GetResourceString("WRN_TupleBinopLiteralNameMismatch");

        internal static string WRN_TupleBinopLiteralNameMismatch_Title => GetResourceString("WRN_TupleBinopLiteralNameMismatch_Title");

        internal static string ERR_PredefinedValueTupleTypeMustBeStruct => GetResourceString("ERR_PredefinedValueTupleTypeMustBeStruct");

        internal static string ERR_NewWithTupleTypeSyntax => GetResourceString("ERR_NewWithTupleTypeSyntax");

        internal static string ERR_DeconstructionVarFormDisallowsSpecificType => GetResourceString("ERR_DeconstructionVarFormDisallowsSpecificType");

        internal static string ERR_TupleElementNamesAttributeMissing => GetResourceString("ERR_TupleElementNamesAttributeMissing");

        internal static string ERR_ExplicitTupleElementNamesAttribute => GetResourceString("ERR_ExplicitTupleElementNamesAttribute");

        internal static string ERR_ExpressionTreeContainsOutVariable => GetResourceString("ERR_ExpressionTreeContainsOutVariable");

        internal static string ERR_ExpressionTreeContainsDiscard => GetResourceString("ERR_ExpressionTreeContainsDiscard");

        internal static string ERR_ExpressionTreeContainsIsMatch => GetResourceString("ERR_ExpressionTreeContainsIsMatch");

        internal static string ERR_ExpressionTreeContainsTupleLiteral => GetResourceString("ERR_ExpressionTreeContainsTupleLiteral");

        internal static string ERR_ExpressionTreeContainsTupleConversion => GetResourceString("ERR_ExpressionTreeContainsTupleConversion");

        internal static string ERR_SourceLinkRequiresPdb => GetResourceString("ERR_SourceLinkRequiresPdb");

        internal static string ERR_CannotEmbedWithoutPdb => GetResourceString("ERR_CannotEmbedWithoutPdb");

        internal static string ERR_InvalidInstrumentationKind => GetResourceString("ERR_InvalidInstrumentationKind");

        internal static string ERR_InvalidHashAlgorithmName => GetResourceString("ERR_InvalidHashAlgorithmName");

        internal static string ERR_VarInvocationLvalueReserved => GetResourceString("ERR_VarInvocationLvalueReserved");

        internal static string ERR_SemiOrLBraceOrArrowExpected => GetResourceString("ERR_SemiOrLBraceOrArrowExpected");

        internal static string ERR_ThrowMisplaced => GetResourceString("ERR_ThrowMisplaced");

        internal static string ERR_DeclarationExpressionNotPermitted => GetResourceString("ERR_DeclarationExpressionNotPermitted");

        internal static string ERR_MustDeclareForeachIteration => GetResourceString("ERR_MustDeclareForeachIteration");

        internal static string ERR_TupleElementNamesInDeconstruction => GetResourceString("ERR_TupleElementNamesInDeconstruction");

        internal static string ERR_PossibleBadNegCast => GetResourceString("ERR_PossibleBadNegCast");

        internal static string ERR_ExpressionTreeContainsThrowExpression => GetResourceString("ERR_ExpressionTreeContainsThrowExpression");

        internal static string ERR_ExpressionTreeContainsWithExpression => GetResourceString("ERR_ExpressionTreeContainsWithExpression");

        internal static string ERR_BadAssemblyName => GetResourceString("ERR_BadAssemblyName");

        internal static string ERR_BadAsyncMethodBuilderTaskProperty => GetResourceString("ERR_BadAsyncMethodBuilderTaskProperty");

        internal static string ERR_AttributesInLocalFuncDecl => GetResourceString("ERR_AttributesInLocalFuncDecl");

        internal static string ERR_TypeForwardedToMultipleAssemblies => GetResourceString("ERR_TypeForwardedToMultipleAssemblies");

        internal static string ERR_PatternDynamicType => GetResourceString("ERR_PatternDynamicType");

        internal static string ERR_BadDocumentationMode => GetResourceString("ERR_BadDocumentationMode");

        internal static string ERR_BadSourceCodeKind => GetResourceString("ERR_BadSourceCodeKind");

        internal static string ERR_BadLanguageVersion => GetResourceString("ERR_BadLanguageVersion");

        internal static string ERR_InvalidPreprocessingSymbol => GetResourceString("ERR_InvalidPreprocessingSymbol");

        internal static string ERR_FeatureNotAvailableInVersion7_1 => GetResourceString("ERR_FeatureNotAvailableInVersion7_1");

        internal static string ERR_FeatureNotAvailableInVersion7_2 => GetResourceString("ERR_FeatureNotAvailableInVersion7_2");

        internal static string ERR_FeatureNotAvailableInVersion7_3 => GetResourceString("ERR_FeatureNotAvailableInVersion7_3");

        internal static string ERR_FeatureNotAvailableInVersion8 => GetResourceString("ERR_FeatureNotAvailableInVersion8");

        internal static string ERR_LanguageVersionCannotHaveLeadingZeroes => GetResourceString("ERR_LanguageVersionCannotHaveLeadingZeroes");

        internal static string ERR_VoidAssignment => GetResourceString("ERR_VoidAssignment");

        internal static string WRN_Experimental => GetResourceString("WRN_Experimental");

        internal static string WRN_Experimental_Title => GetResourceString("WRN_Experimental_Title");

        internal static string ERR_CompilerAndLanguageVersion => GetResourceString("ERR_CompilerAndLanguageVersion");

        internal static string IDS_FeatureAsyncMain => GetResourceString("IDS_FeatureAsyncMain");

        internal static string ERR_TupleInferredNamesNotAvailable => GetResourceString("ERR_TupleInferredNamesNotAvailable");

        internal static string ERR_AltInterpolatedVerbatimStringsNotAvailable => GetResourceString("ERR_AltInterpolatedVerbatimStringsNotAvailable");

        internal static string WRN_AttributesOnBackingFieldsNotAvailable => GetResourceString("WRN_AttributesOnBackingFieldsNotAvailable");

        internal static string WRN_AttributesOnBackingFieldsNotAvailable_Title => GetResourceString("WRN_AttributesOnBackingFieldsNotAvailable_Title");

        internal static string ERR_VoidInTuple => GetResourceString("ERR_VoidInTuple");

        internal static string IDS_FeatureNullableReferenceTypes => GetResourceString("IDS_FeatureNullableReferenceTypes");

        internal static string IDS_FeaturePragmaWarningEnable => GetResourceString("IDS_FeaturePragmaWarningEnable");

        internal static string WRN_ConvertingNullableToNonNullable => GetResourceString("WRN_ConvertingNullableToNonNullable");

        internal static string WRN_ConvertingNullableToNonNullable_Title => GetResourceString("WRN_ConvertingNullableToNonNullable_Title");

        internal static string WRN_NullReferenceAssignment => GetResourceString("WRN_NullReferenceAssignment");

        internal static string WRN_NullReferenceAssignment_Title => GetResourceString("WRN_NullReferenceAssignment_Title");

        internal static string WRN_NullReferenceReceiver => GetResourceString("WRN_NullReferenceReceiver");

        internal static string WRN_NullReferenceReceiver_Title => GetResourceString("WRN_NullReferenceReceiver_Title");

        internal static string WRN_NullReferenceReturn => GetResourceString("WRN_NullReferenceReturn");

        internal static string WRN_NullReferenceReturn_Title => GetResourceString("WRN_NullReferenceReturn_Title");

        internal static string WRN_NullReferenceArgument => GetResourceString("WRN_NullReferenceArgument");

        internal static string WRN_NullReferenceArgument_Title => GetResourceString("WRN_NullReferenceArgument_Title");

        internal static string WRN_ThrowPossibleNull => GetResourceString("WRN_ThrowPossibleNull");

        internal static string WRN_ThrowPossibleNull_Title => GetResourceString("WRN_ThrowPossibleNull_Title");

        internal static string WRN_UnboxPossibleNull => GetResourceString("WRN_UnboxPossibleNull");

        internal static string WRN_UnboxPossibleNull_Title => GetResourceString("WRN_UnboxPossibleNull_Title");

        internal static string WRN_NullabilityMismatchInTypeOnOverride => GetResourceString("WRN_NullabilityMismatchInTypeOnOverride");

        internal static string WRN_NullabilityMismatchInTypeOnOverride_Title => GetResourceString("WRN_NullabilityMismatchInTypeOnOverride_Title");

        internal static string WRN_NullabilityMismatchInReturnTypeOnOverride => GetResourceString("WRN_NullabilityMismatchInReturnTypeOnOverride");

        internal static string WRN_NullabilityMismatchInReturnTypeOnOverride_Title => GetResourceString("WRN_NullabilityMismatchInReturnTypeOnOverride_Title");

        internal static string WRN_TopLevelNullabilityMismatchInReturnTypeOnOverride => GetResourceString("WRN_TopLevelNullabilityMismatchInReturnTypeOnOverride");

        internal static string WRN_TopLevelNullabilityMismatchInReturnTypeOnOverride_Title => GetResourceString("WRN_TopLevelNullabilityMismatchInReturnTypeOnOverride_Title");

        internal static string WRN_NullabilityMismatchInParameterTypeOnOverride => GetResourceString("WRN_NullabilityMismatchInParameterTypeOnOverride");

        internal static string WRN_NullabilityMismatchInParameterTypeOnOverride_Title => GetResourceString("WRN_NullabilityMismatchInParameterTypeOnOverride_Title");

        internal static string WRN_TopLevelNullabilityMismatchInParameterTypeOnOverride => GetResourceString("WRN_TopLevelNullabilityMismatchInParameterTypeOnOverride");

        internal static string WRN_TopLevelNullabilityMismatchInParameterTypeOnOverride_Title => GetResourceString("WRN_TopLevelNullabilityMismatchInParameterTypeOnOverride_Title");

        internal static string WRN_NullabilityMismatchInParameterTypeOnPartial => GetResourceString("WRN_NullabilityMismatchInParameterTypeOnPartial");

        internal static string WRN_NullabilityMismatchInParameterTypeOnPartial_Title => GetResourceString("WRN_NullabilityMismatchInParameterTypeOnPartial_Title");

        internal static string WRN_NullabilityMismatchInReturnTypeOnPartial => GetResourceString("WRN_NullabilityMismatchInReturnTypeOnPartial");

        internal static string WRN_NullabilityMismatchInReturnTypeOnPartial_Title => GetResourceString("WRN_NullabilityMismatchInReturnTypeOnPartial_Title");

        internal static string WRN_NullabilityMismatchInTypeOnImplicitImplementation => GetResourceString("WRN_NullabilityMismatchInTypeOnImplicitImplementation");

        internal static string WRN_NullabilityMismatchInTypeOnImplicitImplementation_Title => GetResourceString("WRN_NullabilityMismatchInTypeOnImplicitImplementation_Title");

        internal static string WRN_NullabilityMismatchInReturnTypeOnImplicitImplementation => GetResourceString("WRN_NullabilityMismatchInReturnTypeOnImplicitImplementation");

        internal static string WRN_NullabilityMismatchInReturnTypeOnImplicitImplementation_Title => GetResourceString("WRN_NullabilityMismatchInReturnTypeOnImplicitImplementation_Title");

        internal static string WRN_NullabilityMismatchInParameterTypeOnImplicitImplementation => GetResourceString("WRN_NullabilityMismatchInParameterTypeOnImplicitImplementation");

        internal static string WRN_NullabilityMismatchInParameterTypeOnImplicitImplementation_Title => GetResourceString("WRN_NullabilityMismatchInParameterTypeOnImplicitImplementation_Title");

        internal static string WRN_TopLevelNullabilityMismatchInReturnTypeOnImplicitImplementation => GetResourceString("WRN_TopLevelNullabilityMismatchInReturnTypeOnImplicitImplementation");

        internal static string WRN_TopLevelNullabilityMismatchInReturnTypeOnImplicitImplementation_Title => GetResourceString("WRN_TopLevelNullabilityMismatchInReturnTypeOnImplicitImplementation_Title");

        internal static string WRN_TopLevelNullabilityMismatchInParameterTypeOnImplicitImplementation => GetResourceString("WRN_TopLevelNullabilityMismatchInParameterTypeOnImplicitImplementation");

        internal static string WRN_TopLevelNullabilityMismatchInParameterTypeOnImplicitImplementation_Title => GetResourceString("WRN_TopLevelNullabilityMismatchInParameterTypeOnImplicitImplementation_Title");

        internal static string WRN_NullabilityMismatchInTypeOnExplicitImplementation => GetResourceString("WRN_NullabilityMismatchInTypeOnExplicitImplementation");

        internal static string WRN_NullabilityMismatchInTypeOnExplicitImplementation_Title => GetResourceString("WRN_NullabilityMismatchInTypeOnExplicitImplementation_Title");

        internal static string WRN_NullabilityMismatchInReturnTypeOnExplicitImplementation => GetResourceString("WRN_NullabilityMismatchInReturnTypeOnExplicitImplementation");

        internal static string WRN_NullabilityMismatchInReturnTypeOnExplicitImplementation_Title => GetResourceString("WRN_NullabilityMismatchInReturnTypeOnExplicitImplementation_Title");

        internal static string WRN_NullabilityMismatchInParameterTypeOnExplicitImplementation => GetResourceString("WRN_NullabilityMismatchInParameterTypeOnExplicitImplementation");

        internal static string WRN_NullabilityMismatchInParameterTypeOnExplicitImplementation_Title => GetResourceString("WRN_NullabilityMismatchInParameterTypeOnExplicitImplementation_Title");

        internal static string WRN_TopLevelNullabilityMismatchInReturnTypeOnExplicitImplementation => GetResourceString("WRN_TopLevelNullabilityMismatchInReturnTypeOnExplicitImplementation");

        internal static string WRN_TopLevelNullabilityMismatchInReturnTypeOnExplicitImplementation_Title => GetResourceString("WRN_TopLevelNullabilityMismatchInReturnTypeOnExplicitImplementation_Title");

        internal static string WRN_TopLevelNullabilityMismatchInParameterTypeOnExplicitImplementation => GetResourceString("WRN_TopLevelNullabilityMismatchInParameterTypeOnExplicitImplementation");

        internal static string WRN_TopLevelNullabilityMismatchInParameterTypeOnExplicitImplementation_Title => GetResourceString("WRN_TopLevelNullabilityMismatchInParameterTypeOnExplicitImplementation_Title");

        internal static string WRN_UninitializedNonNullableField => GetResourceString("WRN_UninitializedNonNullableField");

        internal static string WRN_UninitializedNonNullableField_Title => GetResourceString("WRN_UninitializedNonNullableField_Title");

        internal static string WRN_NullabilityMismatchInAssignment => GetResourceString("WRN_NullabilityMismatchInAssignment");

        internal static string WRN_NullabilityMismatchInAssignment_Title => GetResourceString("WRN_NullabilityMismatchInAssignment_Title");

        internal static string WRN_ImplicitCopyInReadOnlyMember => GetResourceString("WRN_ImplicitCopyInReadOnlyMember");

        internal static string WRN_ImplicitCopyInReadOnlyMember_Title => GetResourceString("WRN_ImplicitCopyInReadOnlyMember_Title");

        internal static string ERR_StaticMemberCantBeReadOnly => GetResourceString("ERR_StaticMemberCantBeReadOnly");

        internal static string ERR_AutoSetterCantBeReadOnly => GetResourceString("ERR_AutoSetterCantBeReadOnly");

        internal static string ERR_AutoPropertyWithSetterCantBeReadOnly => GetResourceString("ERR_AutoPropertyWithSetterCantBeReadOnly");

        internal static string ERR_InvalidPropertyReadOnlyMods => GetResourceString("ERR_InvalidPropertyReadOnlyMods");

        internal static string ERR_DuplicatePropertyReadOnlyMods => GetResourceString("ERR_DuplicatePropertyReadOnlyMods");

        internal static string ERR_FieldLikeEventCantBeReadOnly => GetResourceString("ERR_FieldLikeEventCantBeReadOnly");

        internal static string ERR_PartialMethodReadOnlyDifference => GetResourceString("ERR_PartialMethodReadOnlyDifference");

        internal static string ERR_ReadOnlyModMissingAccessor => GetResourceString("ERR_ReadOnlyModMissingAccessor");

        internal static string WRN_NullabilityMismatchInArgument => GetResourceString("WRN_NullabilityMismatchInArgument");

        internal static string WRN_NullabilityMismatchInArgument_Title => GetResourceString("WRN_NullabilityMismatchInArgument_Title");

        internal static string WRN_NullabilityMismatchInArgumentForOutput => GetResourceString("WRN_NullabilityMismatchInArgumentForOutput");

        internal static string WRN_NullabilityMismatchInArgumentForOutput_Title => GetResourceString("WRN_NullabilityMismatchInArgumentForOutput_Title");

        internal static string WRN_DisallowNullAttributeForbidsMaybeNullAssignment => GetResourceString("WRN_DisallowNullAttributeForbidsMaybeNullAssignment");

        internal static string WRN_DisallowNullAttributeForbidsMaybeNullAssignment_Title => GetResourceString("WRN_DisallowNullAttributeForbidsMaybeNullAssignment_Title");

        internal static string WRN_ParameterConditionallyDisallowsNull => GetResourceString("WRN_ParameterConditionallyDisallowsNull");

        internal static string WRN_ParameterConditionallyDisallowsNull_Title => GetResourceString("WRN_ParameterConditionallyDisallowsNull_Title");

        internal static string WRN_ParameterDisallowsNull => GetResourceString("WRN_ParameterDisallowsNull");

        internal static string WRN_ParameterDisallowsNull_Title => GetResourceString("WRN_ParameterDisallowsNull_Title");

        internal static string WRN_ParameterNotNullIfNotNull => GetResourceString("WRN_ParameterNotNullIfNotNull");

        internal static string WRN_ParameterNotNullIfNotNull_Title => GetResourceString("WRN_ParameterNotNullIfNotNull_Title");

        internal static string WRN_ReturnNotNullIfNotNull => GetResourceString("WRN_ReturnNotNullIfNotNull");

        internal static string WRN_ReturnNotNullIfNotNull_Title => GetResourceString("WRN_ReturnNotNullIfNotNull_Title");

        internal static string WRN_MemberNotNull => GetResourceString("WRN_MemberNotNull");

        internal static string WRN_MemberNotNull_Title => GetResourceString("WRN_MemberNotNull_Title");

        internal static string WRN_MemberNotNullBadMember => GetResourceString("WRN_MemberNotNullBadMember");

        internal static string WRN_MemberNotNullBadMember_Title => GetResourceString("WRN_MemberNotNullBadMember_Title");

        internal static string WRN_MemberNotNullWhen => GetResourceString("WRN_MemberNotNullWhen");

        internal static string WRN_MemberNotNullWhen_Title => GetResourceString("WRN_MemberNotNullWhen_Title");

        internal static string WRN_ShouldNotReturn => GetResourceString("WRN_ShouldNotReturn");

        internal static string WRN_ShouldNotReturn_Title => GetResourceString("WRN_ShouldNotReturn_Title");

        internal static string WRN_DoesNotReturnMismatch => GetResourceString("WRN_DoesNotReturnMismatch");

        internal static string WRN_DoesNotReturnMismatch_Title => GetResourceString("WRN_DoesNotReturnMismatch_Title");

        internal static string WRN_NullabilityMismatchInReturnTypeOfTargetDelegate => GetResourceString("WRN_NullabilityMismatchInReturnTypeOfTargetDelegate");

        internal static string WRN_NullabilityMismatchInReturnTypeOfTargetDelegate_Title => GetResourceString("WRN_NullabilityMismatchInReturnTypeOfTargetDelegate_Title");

        internal static string WRN_NullabilityMismatchInParameterTypeOfTargetDelegate => GetResourceString("WRN_NullabilityMismatchInParameterTypeOfTargetDelegate");

        internal static string WRN_NullabilityMismatchInParameterTypeOfTargetDelegate_Title => GetResourceString("WRN_NullabilityMismatchInParameterTypeOfTargetDelegate_Title");

        internal static string WRN_NullAsNonNullable => GetResourceString("WRN_NullAsNonNullable");

        internal static string WRN_NullAsNonNullable_Title => GetResourceString("WRN_NullAsNonNullable_Title");

        internal static string ERR_AnnotationDisallowedInObjectCreation => GetResourceString("ERR_AnnotationDisallowedInObjectCreation");

        internal static string WRN_NullableValueTypeMayBeNull => GetResourceString("WRN_NullableValueTypeMayBeNull");

        internal static string WRN_NullableValueTypeMayBeNull_Title => GetResourceString("WRN_NullableValueTypeMayBeNull_Title");

        internal static string WRN_NullabilityMismatchInTypeParameterConstraint => GetResourceString("WRN_NullabilityMismatchInTypeParameterConstraint");

        internal static string WRN_NullabilityMismatchInTypeParameterConstraint_Title => GetResourceString("WRN_NullabilityMismatchInTypeParameterConstraint_Title");

        internal static string WRN_MissingNonNullTypesContextForAnnotation => GetResourceString("WRN_MissingNonNullTypesContextForAnnotation");

        internal static string WRN_MissingNonNullTypesContextForAnnotation_Title => GetResourceString("WRN_MissingNonNullTypesContextForAnnotation_Title");

        internal static string ERR_ExplicitNullableAttribute => GetResourceString("ERR_ExplicitNullableAttribute");

        internal static string ERR_NullableUnconstrainedTypeParameter => GetResourceString("ERR_NullableUnconstrainedTypeParameter");

        internal static string ERR_NullableOptionNotAvailable => GetResourceString("ERR_NullableOptionNotAvailable");

        internal static string ERR_NonTaskMainCantBeAsync => GetResourceString("ERR_NonTaskMainCantBeAsync");

        internal static string ERR_PatternWrongGenericTypeInVersion => GetResourceString("ERR_PatternWrongGenericTypeInVersion");

        internal static string WRN_UnreferencedLocalFunction => GetResourceString("WRN_UnreferencedLocalFunction");

        internal static string WRN_UnreferencedLocalFunction_Title => GetResourceString("WRN_UnreferencedLocalFunction_Title");

        internal static string ERR_LocalFunctionMissingBody => GetResourceString("ERR_LocalFunctionMissingBody");

        internal static string ERR_InvalidDebugInfo => GetResourceString("ERR_InvalidDebugInfo");

        internal static string IConversionExpressionIsNotCSharpConversion => GetResourceString("IConversionExpressionIsNotCSharpConversion");

        internal static string ERR_DynamicLocalFunctionTypeParameter => GetResourceString("ERR_DynamicLocalFunctionTypeParameter");

        internal static string IDS_FeatureLeadingDigitSeparator => GetResourceString("IDS_FeatureLeadingDigitSeparator");

        internal static string ERR_ExplicitReservedAttr => GetResourceString("ERR_ExplicitReservedAttr");

        internal static string ERR_TypeReserved => GetResourceString("ERR_TypeReserved");

        internal static string ERR_InExtensionMustBeValueType => GetResourceString("ERR_InExtensionMustBeValueType");

        internal static string ERR_FieldsInRoStruct => GetResourceString("ERR_FieldsInRoStruct");

        internal static string ERR_AutoPropsInRoStruct => GetResourceString("ERR_AutoPropsInRoStruct");

        internal static string ERR_FieldlikeEventsInRoStruct => GetResourceString("ERR_FieldlikeEventsInRoStruct");

        internal static string IDS_FeatureRefExtensionMethods => GetResourceString("IDS_FeatureRefExtensionMethods");

        internal static string ERR_StackAllocConversionNotPossible => GetResourceString("ERR_StackAllocConversionNotPossible");

        internal static string ERR_RefExtensionMustBeValueTypeOrConstrainedToOne => GetResourceString("ERR_RefExtensionMustBeValueTypeOrConstrainedToOne");

        internal static string ERR_OutAttrOnInParam => GetResourceString("ERR_OutAttrOnInParam");

        internal static string ICompoundAssignmentOperationIsNotCSharpCompoundAssignment => GetResourceString("ICompoundAssignmentOperationIsNotCSharpCompoundAssignment");

        internal static string WRN_FilterIsConstantFalse => GetResourceString("WRN_FilterIsConstantFalse");

        internal static string WRN_FilterIsConstantFalse_Title => GetResourceString("WRN_FilterIsConstantFalse_Title");

        internal static string WRN_FilterIsConstantFalseRedundantTryCatch => GetResourceString("WRN_FilterIsConstantFalseRedundantTryCatch");

        internal static string WRN_FilterIsConstantFalseRedundantTryCatch_Title => GetResourceString("WRN_FilterIsConstantFalseRedundantTryCatch_Title");

        internal static string ERR_ConditionalInInterpolation => GetResourceString("ERR_ConditionalInInterpolation");

        internal static string ERR_InDynamicMethodArg => GetResourceString("ERR_InDynamicMethodArg");

        internal static string ERR_TupleSizesMismatchForBinOps => GetResourceString("ERR_TupleSizesMismatchForBinOps");

        internal static string ERR_RefLocalOrParamExpected => GetResourceString("ERR_RefLocalOrParamExpected");

        internal static string ERR_RefAssignNarrower => GetResourceString("ERR_RefAssignNarrower");

        internal static string IDS_FeatureEnumGenericTypeConstraint => GetResourceString("IDS_FeatureEnumGenericTypeConstraint");

        internal static string IDS_FeatureDelegateGenericTypeConstraint => GetResourceString("IDS_FeatureDelegateGenericTypeConstraint");

        internal static string IDS_FeatureUnmanagedGenericTypeConstraint => GetResourceString("IDS_FeatureUnmanagedGenericTypeConstraint");

        internal static string ERR_NewBoundWithUnmanaged => GetResourceString("ERR_NewBoundWithUnmanaged");

        internal static string ERR_UnmanagedConstraintNotSatisfied => GetResourceString("ERR_UnmanagedConstraintNotSatisfied");

        internal static string ERR_ConWithUnmanagedCon => GetResourceString("ERR_ConWithUnmanagedCon");

        internal static string IDS_FeatureStackAllocInitializer => GetResourceString("IDS_FeatureStackAllocInitializer");

        internal static string ERR_InvalidStackAllocArray => GetResourceString("ERR_InvalidStackAllocArray");

        internal static string IDS_FeatureExpressionVariablesInQueriesAndInitializers => GetResourceString("IDS_FeatureExpressionVariablesInQueriesAndInitializers");

        internal static string ERR_MissingPattern => GetResourceString("ERR_MissingPattern");

        internal static string IDS_FeatureRecursivePatterns => GetResourceString("IDS_FeatureRecursivePatterns");

        internal static string IDS_FeatureNullPointerConstantPattern => GetResourceString("IDS_FeatureNullPointerConstantPattern");

        internal static string IDS_FeatureDefaultTypeParameterConstraint => GetResourceString("IDS_FeatureDefaultTypeParameterConstraint");

        internal static string ERR_WrongNumberOfSubpatterns => GetResourceString("ERR_WrongNumberOfSubpatterns");

        internal static string ERR_PropertyPatternNameMissing => GetResourceString("ERR_PropertyPatternNameMissing");

        internal static string ERR_DefaultPattern => GetResourceString("ERR_DefaultPattern");

        internal static string ERR_SwitchExpressionNoBestType => GetResourceString("ERR_SwitchExpressionNoBestType");

        internal static string ERR_DefaultLiteralNoTargetType => GetResourceString("ERR_DefaultLiteralNoTargetType");

        internal static string ERR_CannotInferDelegateType => GetResourceString("ERR_CannotInferDelegateType");

        internal static string ERR_SingleElementPositionalPatternRequiresDisambiguation => GetResourceString("ERR_SingleElementPositionalPatternRequiresDisambiguation");

        internal static string ERR_VarMayNotBindToType => GetResourceString("ERR_VarMayNotBindToType");

        internal static string WRN_SwitchExpressionNotExhaustive => GetResourceString("WRN_SwitchExpressionNotExhaustive");

        internal static string WRN_SwitchExpressionNotExhaustive_Title => GetResourceString("WRN_SwitchExpressionNotExhaustive_Title");

        internal static string WRN_SwitchExpressionNotExhaustiveWithWhen => GetResourceString("WRN_SwitchExpressionNotExhaustiveWithWhen");

        internal static string WRN_SwitchExpressionNotExhaustiveWithWhen_Title => GetResourceString("WRN_SwitchExpressionNotExhaustiveWithWhen_Title");

        internal static string WRN_SwitchExpressionNotExhaustiveWithUnnamedEnumValue => GetResourceString("WRN_SwitchExpressionNotExhaustiveWithUnnamedEnumValue");

        internal static string WRN_SwitchExpressionNotExhaustiveWithUnnamedEnumValue_Title => GetResourceString("WRN_SwitchExpressionNotExhaustiveWithUnnamedEnumValue_Title");

        internal static string WRN_CaseConstantNamedUnderscore => GetResourceString("WRN_CaseConstantNamedUnderscore");

        internal static string WRN_CaseConstantNamedUnderscore_Title => GetResourceString("WRN_CaseConstantNamedUnderscore_Title");

        internal static string WRN_IsTypeNamedUnderscore => GetResourceString("WRN_IsTypeNamedUnderscore");

        internal static string WRN_IsTypeNamedUnderscore_Title => GetResourceString("WRN_IsTypeNamedUnderscore_Title");

        internal static string ERR_ExpressionTreeContainsSwitchExpression => GetResourceString("ERR_ExpressionTreeContainsSwitchExpression");

        internal static string ERR_InvalidObjectCreation => GetResourceString("ERR_InvalidObjectCreation");

        internal static string IDS_FeatureIndexingMovableFixedBuffers => GetResourceString("IDS_FeatureIndexingMovableFixedBuffers");

        internal static string ERR_CantUseInOrOutInArglist => GetResourceString("ERR_CantUseInOrOutInArglist");

        internal static string SyntaxTreeNotFound => GetResourceString("SyntaxTreeNotFound");

        internal static string ERR_OutVariableCannotBeByRef => GetResourceString("ERR_OutVariableCannotBeByRef");

        internal static string ERR_MultipleAnalyzerConfigsInSameDir => GetResourceString("ERR_MultipleAnalyzerConfigsInSameDir");

        internal static string IDS_FeatureCoalesceAssignmentExpression => GetResourceString("IDS_FeatureCoalesceAssignmentExpression");

        internal static string CannotCreateConstructedFromConstructed => GetResourceString("CannotCreateConstructedFromConstructed");

        internal static string CannotCreateConstructedFromNongeneric => GetResourceString("CannotCreateConstructedFromNongeneric");

        internal static string IDS_FeatureUnconstrainedTypeParameterInNullCoalescingOperator => GetResourceString("IDS_FeatureUnconstrainedTypeParameterInNullCoalescingOperator");

        internal static string WRN_NullabilityMismatchInConstraintsOnImplicitImplementation => GetResourceString("WRN_NullabilityMismatchInConstraintsOnImplicitImplementation");

        internal static string WRN_NullabilityMismatchInConstraintsOnImplicitImplementation_Title => GetResourceString("WRN_NullabilityMismatchInConstraintsOnImplicitImplementation_Title");

        internal static string WRN_NullabilityMismatchInTypeParameterReferenceTypeConstraint => GetResourceString("WRN_NullabilityMismatchInTypeParameterReferenceTypeConstraint");

        internal static string WRN_NullabilityMismatchInTypeParameterReferenceTypeConstraint_Title => GetResourceString("WRN_NullabilityMismatchInTypeParameterReferenceTypeConstraint_Title");

        internal static string ERR_TripleDotNotAllowed => GetResourceString("ERR_TripleDotNotAllowed");

        internal static string IDS_FeatureIndexOperator => GetResourceString("IDS_FeatureIndexOperator");

        internal static string IDS_FeatureRangeOperator => GetResourceString("IDS_FeatureRangeOperator");

        internal static string IDS_FeatureStaticLocalFunctions => GetResourceString("IDS_FeatureStaticLocalFunctions");

        internal static string IDS_FeatureNameShadowingInNestedFunctions => GetResourceString("IDS_FeatureNameShadowingInNestedFunctions");

        internal static string IDS_FeatureLambdaDiscardParameters => GetResourceString("IDS_FeatureLambdaDiscardParameters");

        internal static string IDS_FeatureMemberNotNull => GetResourceString("IDS_FeatureMemberNotNull");

        internal static string IDS_FeatureNativeInt => GetResourceString("IDS_FeatureNativeInt");

        internal static string ERR_BadDynamicAwaitForEach => GetResourceString("ERR_BadDynamicAwaitForEach");

        internal static string ERR_NullableDirectiveQualifierExpected => GetResourceString("ERR_NullableDirectiveQualifierExpected");

        internal static string ERR_NullableDirectiveTargetExpected => GetResourceString("ERR_NullableDirectiveTargetExpected");

        internal static string WRN_MissingNonNullTypesContextForAnnotationInGeneratedCode => GetResourceString("WRN_MissingNonNullTypesContextForAnnotationInGeneratedCode");

        internal static string WRN_MissingNonNullTypesContextForAnnotationInGeneratedCode_Title => GetResourceString("WRN_MissingNonNullTypesContextForAnnotationInGeneratedCode_Title");

        internal static string WRN_NullReferenceInitializer => GetResourceString("WRN_NullReferenceInitializer");

        internal static string WRN_NullReferenceInitializer_Title => GetResourceString("WRN_NullReferenceInitializer_Title");

        internal static string ERR_ExpressionTreeCantContainRefStruct => GetResourceString("ERR_ExpressionTreeCantContainRefStruct");

        internal static string ERR_ElseCannotStartStatement => GetResourceString("ERR_ElseCannotStartStatement");

        internal static string ERR_ExpressionTreeCantContainNullCoalescingAssignment => GetResourceString("ERR_ExpressionTreeCantContainNullCoalescingAssignment");

        internal static string ERR_BadNullableContextOption => GetResourceString("ERR_BadNullableContextOption");

        internal static string ERR_SwitchGoverningExpressionRequiresParens => GetResourceString("ERR_SwitchGoverningExpressionRequiresParens");

        internal static string ERR_TupleElementNameMismatch => GetResourceString("ERR_TupleElementNameMismatch");

        internal static string ERR_DeconstructParameterNameMismatch => GetResourceString("ERR_DeconstructParameterNameMismatch");

        internal static string ERR_IsPatternImpossible => GetResourceString("ERR_IsPatternImpossible");

        internal static string WRN_IsPatternAlways => GetResourceString("WRN_IsPatternAlways");

        internal static string WRN_IsPatternAlways_Title => GetResourceString("WRN_IsPatternAlways_Title");

        internal static string WRN_GivenExpressionNeverMatchesPattern => GetResourceString("WRN_GivenExpressionNeverMatchesPattern");

        internal static string WRN_GivenExpressionNeverMatchesPattern_Title => GetResourceString("WRN_GivenExpressionNeverMatchesPattern_Title");

        internal static string WRN_GivenExpressionAlwaysMatchesConstant => GetResourceString("WRN_GivenExpressionAlwaysMatchesConstant");

        internal static string WRN_GivenExpressionAlwaysMatchesConstant_Title => GetResourceString("WRN_GivenExpressionAlwaysMatchesConstant_Title");

        internal static string WRN_GivenExpressionAlwaysMatchesPattern => GetResourceString("WRN_GivenExpressionAlwaysMatchesPattern");

        internal static string WRN_GivenExpressionAlwaysMatchesPattern_Title => GetResourceString("WRN_GivenExpressionAlwaysMatchesPattern_Title");

        internal static string ERR_FeatureNotAvailableInVersion8_0 => GetResourceString("ERR_FeatureNotAvailableInVersion8_0");

        internal static string ERR_PointerTypeInPatternMatching => GetResourceString("ERR_PointerTypeInPatternMatching");

        internal static string ERR_ArgumentNameInITuplePattern => GetResourceString("ERR_ArgumentNameInITuplePattern");

        internal static string ERR_DiscardPatternInSwitchStatement => GetResourceString("ERR_DiscardPatternInSwitchStatement");

        internal static string WRN_NullabilityMismatchInExplicitlyImplementedInterface => GetResourceString("WRN_NullabilityMismatchInExplicitlyImplementedInterface");

        internal static string WRN_NullabilityMismatchInExplicitlyImplementedInterface_Title => GetResourceString("WRN_NullabilityMismatchInExplicitlyImplementedInterface_Title");

        internal static string WRN_NullabilityMismatchInInterfaceImplementedByBase => GetResourceString("WRN_NullabilityMismatchInInterfaceImplementedByBase");

        internal static string WRN_NullabilityMismatchInInterfaceImplementedByBase_Title => GetResourceString("WRN_NullabilityMismatchInInterfaceImplementedByBase_Title");

        internal static string WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList => GetResourceString("WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList");

        internal static string WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList_Title => GetResourceString("WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList_Title");

        internal static string ERR_DuplicateExplicitImpl => GetResourceString("ERR_DuplicateExplicitImpl");

        internal static string ERR_UsingVarInSwitchCase => GetResourceString("ERR_UsingVarInSwitchCase");

        internal static string ERR_GoToForwardJumpOverUsingVar => GetResourceString("ERR_GoToForwardJumpOverUsingVar");

        internal static string ERR_GoToBackwardJumpOverUsingVar => GetResourceString("ERR_GoToBackwardJumpOverUsingVar");

        internal static string IDS_FeatureUsingDeclarations => GetResourceString("IDS_FeatureUsingDeclarations");

        internal static string ERR_FeatureInPreview => GetResourceString("ERR_FeatureInPreview");

        internal static string IDS_DefaultInterfaceImplementation => GetResourceString("IDS_DefaultInterfaceImplementation");

        internal static string ERR_RuntimeDoesNotSupportDefaultInterfaceImplementation => GetResourceString("ERR_RuntimeDoesNotSupportDefaultInterfaceImplementation");

        internal static string ERR_RuntimeDoesNotSupportDefaultInterfaceImplementationForMember => GetResourceString("ERR_RuntimeDoesNotSupportDefaultInterfaceImplementationForMember");

        internal static string ERR_InvalidModifierForLanguageVersion => GetResourceString("ERR_InvalidModifierForLanguageVersion");

        internal static string ERR_ImplicitImplementationOfNonPublicInterfaceMember => GetResourceString("ERR_ImplicitImplementationOfNonPublicInterfaceMember");

        internal static string ERR_MostSpecificImplementationIsNotFound => GetResourceString("ERR_MostSpecificImplementationIsNotFound");

        internal static string ERR_LanguageVersionDoesNotSupportDefaultInterfaceImplementationForMember => GetResourceString("ERR_LanguageVersionDoesNotSupportDefaultInterfaceImplementationForMember");

        internal static string ERR_RuntimeDoesNotSupportProtectedAccessForInterfaceMember => GetResourceString("ERR_RuntimeDoesNotSupportProtectedAccessForInterfaceMember");

        internal static string ERR_DefaultInterfaceImplementationInNoPIAType => GetResourceString("ERR_DefaultInterfaceImplementationInNoPIAType");

        internal static string WRN_SwitchExpressionNotExhaustiveForNull => GetResourceString("WRN_SwitchExpressionNotExhaustiveForNull");

        internal static string WRN_SwitchExpressionNotExhaustiveForNull_Title => GetResourceString("WRN_SwitchExpressionNotExhaustiveForNull_Title");

        internal static string WRN_SwitchExpressionNotExhaustiveForNullWithWhen => GetResourceString("WRN_SwitchExpressionNotExhaustiveForNullWithWhen");

        internal static string WRN_SwitchExpressionNotExhaustiveForNullWithWhen_Title => GetResourceString("WRN_SwitchExpressionNotExhaustiveForNullWithWhen_Title");

        internal static string ERR_AttributeNotOnEventAccessor => GetResourceString("ERR_AttributeNotOnEventAccessor");

        internal static string IDS_FeatureObsoleteOnPropertyAccessor => GetResourceString("IDS_FeatureObsoleteOnPropertyAccessor");

        internal static string WRN_UnconsumedEnumeratorCancellationAttributeUsage => GetResourceString("WRN_UnconsumedEnumeratorCancellationAttributeUsage");

        internal static string WRN_UnconsumedEnumeratorCancellationAttributeUsage_Title => GetResourceString("WRN_UnconsumedEnumeratorCancellationAttributeUsage_Title");

        internal static string WRN_UndecoratedCancellationTokenParameter => GetResourceString("WRN_UndecoratedCancellationTokenParameter");

        internal static string WRN_UndecoratedCancellationTokenParameter_Title => GetResourceString("WRN_UndecoratedCancellationTokenParameter_Title");

        internal static string ERR_MultipleEnumeratorCancellationAttributes => GetResourceString("ERR_MultipleEnumeratorCancellationAttributes");

        internal static string ERR_OverrideRefConstraintNotSatisfied => GetResourceString("ERR_OverrideRefConstraintNotSatisfied");

        internal static string ERR_OverrideValConstraintNotSatisfied => GetResourceString("ERR_OverrideValConstraintNotSatisfied");

        internal static string ERR_OverrideDefaultConstraintNotSatisfied => GetResourceString("ERR_OverrideDefaultConstraintNotSatisfied");

        internal static string ERR_DefaultConstraintOverrideOnly => GetResourceString("ERR_DefaultConstraintOverrideOnly");

        internal static string IDS_OverrideWithConstraints => GetResourceString("IDS_OverrideWithConstraints");

        internal static string WRN_NullabilityMismatchInConstraintsOnPartialImplementation => GetResourceString("WRN_NullabilityMismatchInConstraintsOnPartialImplementation");

        internal static string WRN_NullabilityMismatchInConstraintsOnPartialImplementation_Title => GetResourceString("WRN_NullabilityMismatchInConstraintsOnPartialImplementation_Title");

        internal static string IDS_FeatureNestedStackalloc => GetResourceString("IDS_FeatureNestedStackalloc");

        internal static string WRN_NullabilityMismatchInTypeParameterNotNullConstraint => GetResourceString("WRN_NullabilityMismatchInTypeParameterNotNullConstraint");

        internal static string WRN_NullabilityMismatchInTypeParameterNotNullConstraint_Title => GetResourceString("WRN_NullabilityMismatchInTypeParameterNotNullConstraint_Title");

        internal static string IDS_FeatureNotNullGenericTypeConstraint => GetResourceString("IDS_FeatureNotNullGenericTypeConstraint");

        internal static string ERR_DuplicateNullSuppression => GetResourceString("ERR_DuplicateNullSuppression");

        internal static string ERR_ReAbstractionInNoPIAType => GetResourceString("ERR_ReAbstractionInNoPIAType");

        internal static string ERR_BadSwitchValue => GetResourceString("ERR_BadSwitchValue");

        internal static string IDS_FeatureFunctionPointers => GetResourceString("IDS_FeatureFunctionPointers");

        internal static string IDS_AddressOfMethodGroup => GetResourceString("IDS_AddressOfMethodGroup");

        internal static string ERR_InvalidFunctionPointerCallingConvention => GetResourceString("ERR_InvalidFunctionPointerCallingConvention");

        internal static string ERR_TypeNotFound => GetResourceString("ERR_TypeNotFound");

        internal static string ERR_TypeMustBePublic => GetResourceString("ERR_TypeMustBePublic");

        internal static string WRN_SyncAndAsyncEntryPoints => GetResourceString("WRN_SyncAndAsyncEntryPoints");

        internal static string ERR_InternalError => GetResourceString("ERR_InternalError");

        internal static string IDS_FeatureStaticAnonymousFunction => GetResourceString("IDS_FeatureStaticAnonymousFunction");

        internal static string ERR_StaticAnonymousFunctionCannotCaptureThis => GetResourceString("ERR_StaticAnonymousFunctionCannotCaptureThis");

        internal static string ERR_StaticAnonymousFunctionCannotCaptureVariable => GetResourceString("ERR_StaticAnonymousFunctionCannotCaptureVariable");

        internal static string IDS_FeatureAsyncUsing => GetResourceString("IDS_FeatureAsyncUsing");

        internal static string IDS_FeatureParenthesizedPattern => GetResourceString("IDS_FeatureParenthesizedPattern");

        internal static string IDS_FeatureOrPattern => GetResourceString("IDS_FeatureOrPattern");

        internal static string IDS_FeatureAndPattern => GetResourceString("IDS_FeatureAndPattern");

        internal static string IDS_FeatureNotPattern => GetResourceString("IDS_FeatureNotPattern");

        internal static string IDS_FeatureTypePattern => GetResourceString("IDS_FeatureTypePattern");

        internal static string IDS_FeatureRelationalPattern => GetResourceString("IDS_FeatureRelationalPattern");

        internal static string ERR_VarianceInterfaceNesting => GetResourceString("ERR_VarianceInterfaceNesting");

        internal static string ERR_ExternEventInitializer => GetResourceString("ERR_ExternEventInitializer");

        internal static string ERR_ImplicitIndexIndexerWithName => GetResourceString("ERR_ImplicitIndexIndexerWithName");

        internal static string ERR_ImplicitRangeIndexerWithName => GetResourceString("ERR_ImplicitRangeIndexerWithName");

        internal static string ERR_ImplicitObjectCreationIllegalTargetType => GetResourceString("ERR_ImplicitObjectCreationIllegalTargetType");

        internal static string ERR_ImplicitObjectCreationNotValid => GetResourceString("ERR_ImplicitObjectCreationNotValid");

        internal static string ERR_ImplicitObjectCreationNoTargetType => GetResourceString("ERR_ImplicitObjectCreationNoTargetType");

        internal static string IDS_FeatureImplicitObjectCreation => GetResourceString("IDS_FeatureImplicitObjectCreation");

        internal static string ERR_ExpressionTreeContainsPatternIndexOrRangeIndexer => GetResourceString("ERR_ExpressionTreeContainsPatternIndexOrRangeIndexer");

        internal static string ERR_ExpressionTreeContainsFromEndIndexExpression => GetResourceString("ERR_ExpressionTreeContainsFromEndIndexExpression");

        internal static string ERR_ExpressionTreeContainsRangeExpression => GetResourceString("ERR_ExpressionTreeContainsRangeExpression");

        internal static string WRN_GeneratorFailedDuringGeneration => GetResourceString("WRN_GeneratorFailedDuringGeneration");

        internal static string WRN_GeneratorFailedDuringInitialization => GetResourceString("WRN_GeneratorFailedDuringInitialization");

        internal static string WRN_GeneratorFailedDuringGeneration_Title => GetResourceString("WRN_GeneratorFailedDuringGeneration_Title");

        internal static string WRN_GeneratorFailedDuringInitialization_Title => GetResourceString("WRN_GeneratorFailedDuringInitialization_Title");

        internal static string WRN_GeneratorFailedDuringGeneration_Description => GetResourceString("WRN_GeneratorFailedDuringGeneration_Description");

        internal static string WRN_GeneratorFailedDuringInitialization_Description => GetResourceString("WRN_GeneratorFailedDuringInitialization_Description");

        internal static string IDS_FeatureRecords => GetResourceString("IDS_FeatureRecords");

        internal static string IDS_FeatureInitOnlySetters => GetResourceString("IDS_FeatureInitOnlySetters");

        internal static string ERR_InvalidWithReceiverType => GetResourceString("ERR_InvalidWithReceiverType");

        internal static string ERR_CannotClone => GetResourceString("ERR_CannotClone");

        internal static string ERR_AssignmentInitOnly => GetResourceString("ERR_AssignmentInitOnly");

        internal static string ERR_DesignatorBeneathPatternCombinator => GetResourceString("ERR_DesignatorBeneathPatternCombinator");

        internal static string ERR_UnsupportedTypeForRelationalPattern => GetResourceString("ERR_UnsupportedTypeForRelationalPattern");

        internal static string ERR_RelationalPatternWithNaN => GetResourceString("ERR_RelationalPatternWithNaN");

        internal static string IDS_FeatureExtendedPartialMethods => GetResourceString("IDS_FeatureExtendedPartialMethods");

        internal static string IDS_FeatureConstantInterpolatedStrings => GetResourceString("IDS_FeatureConstantInterpolatedStrings");

        internal static string ERR_PartialMethodWithNonVoidReturnMustHaveAccessMods => GetResourceString("ERR_PartialMethodWithNonVoidReturnMustHaveAccessMods");

        internal static string ERR_PartialMethodWithOutParamMustHaveAccessMods => GetResourceString("ERR_PartialMethodWithOutParamMustHaveAccessMods");

        internal static string ERR_PartialMethodWithAccessibilityModsMustHaveImplementation => GetResourceString("ERR_PartialMethodWithAccessibilityModsMustHaveImplementation");

        internal static string ERR_PartialMethodWithExtendedModMustHaveAccessMods => GetResourceString("ERR_PartialMethodWithExtendedModMustHaveAccessMods");

        internal static string ERR_PartialMethodAccessibilityDifference => GetResourceString("ERR_PartialMethodAccessibilityDifference");

        internal static string ERR_PartialMethodExtendedModDifference => GetResourceString("ERR_PartialMethodExtendedModDifference");

        internal static string ERR_PartialMethodReturnTypeDifference => GetResourceString("ERR_PartialMethodReturnTypeDifference");

        internal static string ERR_PartialMethodRefReturnDifference => GetResourceString("ERR_PartialMethodRefReturnDifference");

        internal static string IDS_TopLevelStatements => GetResourceString("IDS_TopLevelStatements");

        internal static string ERR_SimpleProgramLocalIsReferencedOutsideOfTopLevelStatement => GetResourceString("ERR_SimpleProgramLocalIsReferencedOutsideOfTopLevelStatement");

        internal static string ERR_SimpleProgramMultipleUnitsWithTopLevelStatements => GetResourceString("ERR_SimpleProgramMultipleUnitsWithTopLevelStatements");

        internal static string ERR_TopLevelStatementAfterNamespaceOrType => GetResourceString("ERR_TopLevelStatementAfterNamespaceOrType");

        internal static string ERR_SimpleProgramDisallowsMainType => GetResourceString("ERR_SimpleProgramDisallowsMainType");

        internal static string ERR_SimpleProgramNotAnExecutable => GetResourceString("ERR_SimpleProgramNotAnExecutable");

        internal static string ERR_InvalidFuncPointerReturnTypeModifier => GetResourceString("ERR_InvalidFuncPointerReturnTypeModifier");

        internal static string ERR_DupReturnTypeMod => GetResourceString("ERR_DupReturnTypeMod");

        internal static string ERR_BadFuncPointerParamModifier => GetResourceString("ERR_BadFuncPointerParamModifier");

        internal static string ERR_BadFuncPointerArgCount => GetResourceString("ERR_BadFuncPointerArgCount");

        internal static string ERR_MethFuncPtrMismatch => GetResourceString("ERR_MethFuncPtrMismatch");

        internal static string ERR_FuncPtrRefMismatch => GetResourceString("ERR_FuncPtrRefMismatch");

        internal static string ERR_FuncPtrMethMustBeStatic => GetResourceString("ERR_FuncPtrMethMustBeStatic");

        internal static string ERR_AddressOfMethodGroupInExpressionTree => GetResourceString("ERR_AddressOfMethodGroupInExpressionTree");

        internal static string ERR_WrongFuncPtrCallingConvention => GetResourceString("ERR_WrongFuncPtrCallingConvention");

        internal static string ERR_MissingAddressOf => GetResourceString("ERR_MissingAddressOf");

        internal static string ERR_CannotUseReducedExtensionMethodInAddressOf => GetResourceString("ERR_CannotUseReducedExtensionMethodInAddressOf");

        internal static string ERR_CannotUseFunctionPointerAsFixedLocal => GetResourceString("ERR_CannotUseFunctionPointerAsFixedLocal");

        internal static string ERR_UnsupportedCallingConvention => GetResourceString("ERR_UnsupportedCallingConvention");

        internal static string ERR_RuntimeDoesNotSupportUnmanagedDefaultCallConv => GetResourceString("ERR_RuntimeDoesNotSupportUnmanagedDefaultCallConv");

        internal static string NotSameNumberParameterTypesAndRefKinds => GetResourceString("NotSameNumberParameterTypesAndRefKinds");

        internal static string OutIsNotValidForReturn => GetResourceString("OutIsNotValidForReturn");

        internal static string CallingConventionTypesRequireUnmanaged => GetResourceString("CallingConventionTypesRequireUnmanaged");

        internal static string CallingConventionTypeIsInvalid => GetResourceString("CallingConventionTypeIsInvalid");

        internal static string ERR_CannotConvertAddressOfToDelegate => GetResourceString("ERR_CannotConvertAddressOfToDelegate");

        internal static string ERR_AddressOfToNonFunctionPointer => GetResourceString("ERR_AddressOfToNonFunctionPointer");

        internal static string ERR_CannotSpecifyManagedWithUnmanagedSpecifiers => GetResourceString("ERR_CannotSpecifyManagedWithUnmanagedSpecifiers");

        internal static string ERR_FeatureNotAvailableInVersion9 => GetResourceString("ERR_FeatureNotAvailableInVersion9");

        internal static string ERR_UnexpectedArgumentList => GetResourceString("ERR_UnexpectedArgumentList");

        internal static string ERR_UnexpectedOrMissingConstructorInitializerInRecord => GetResourceString("ERR_UnexpectedOrMissingConstructorInitializerInRecord");

        internal static string ERR_MultipleRecordParameterLists => GetResourceString("ERR_MultipleRecordParameterLists");

        internal static string ERR_BadRecordBase => GetResourceString("ERR_BadRecordBase");

        internal static string ERR_BadInheritanceFromRecord => GetResourceString("ERR_BadInheritanceFromRecord");

        internal static string ERR_BadRecordMemberForPositionalParameter => GetResourceString("ERR_BadRecordMemberForPositionalParameter");

        internal static string ERR_NoCopyConstructorInBaseType => GetResourceString("ERR_NoCopyConstructorInBaseType");

        internal static string ERR_CopyConstructorMustInvokeBaseCopyConstructor => GetResourceString("ERR_CopyConstructorMustInvokeBaseCopyConstructor");

        internal static string IDS_FeatureTargetTypedConditional => GetResourceString("IDS_FeatureTargetTypedConditional");

        internal static string ERR_DoesNotOverrideMethodFromObject => GetResourceString("ERR_DoesNotOverrideMethodFromObject");

        internal static string IDS_FeatureCovariantReturnsForOverrides => GetResourceString("IDS_FeatureCovariantReturnsForOverrides");

        internal static string ERR_RuntimeDoesNotSupportCovariantReturnsOfClasses => GetResourceString("ERR_RuntimeDoesNotSupportCovariantReturnsOfClasses");

        internal static string ERR_RuntimeDoesNotSupportCovariantPropertiesOfClasses => GetResourceString("ERR_RuntimeDoesNotSupportCovariantPropertiesOfClasses");

        internal static string ERR_SealedAPIInRecord => GetResourceString("ERR_SealedAPIInRecord");

        internal static string ERR_DoesNotOverrideBaseMethod => GetResourceString("ERR_DoesNotOverrideBaseMethod");

        internal static string WRN_ConstOutOfRangeChecked => GetResourceString("WRN_ConstOutOfRangeChecked");

        internal static string WRN_ConstOutOfRangeChecked_Title => GetResourceString("WRN_ConstOutOfRangeChecked_Title");

        internal static string ERR_CloneDisallowedInRecord => GetResourceString("ERR_CloneDisallowedInRecord");

        internal static string WRN_RecordNamedDisallowed => GetResourceString("WRN_RecordNamedDisallowed");

        internal static string WRN_RecordNamedDisallowed_Title => GetResourceString("WRN_RecordNamedDisallowed_Title");

        internal static string ERR_NotOverridableAPIInRecord => GetResourceString("ERR_NotOverridableAPIInRecord");

        internal static string ERR_NonPublicAPIInRecord => GetResourceString("ERR_NonPublicAPIInRecord");

        internal static string ERR_SignatureMismatchInRecord => GetResourceString("ERR_SignatureMismatchInRecord");

        internal static string ERR_NonProtectedAPIInRecord => GetResourceString("ERR_NonProtectedAPIInRecord");

        internal static string ERR_DoesNotOverrideBaseEqualityContract => GetResourceString("ERR_DoesNotOverrideBaseEqualityContract");

        internal static string ERR_StaticAPIInRecord => GetResourceString("ERR_StaticAPIInRecord");

        internal static string ERR_CopyConstructorWrongAccessibility => GetResourceString("ERR_CopyConstructorWrongAccessibility");

        internal static string ERR_NonPrivateAPIInRecord => GetResourceString("ERR_NonPrivateAPIInRecord");

        internal static string WRN_PrecedenceInversion => GetResourceString("WRN_PrecedenceInversion");

        internal static string WRN_PrecedenceInversion_Title => GetResourceString("WRN_PrecedenceInversion_Title");

        internal static string IDS_FeatureModuleInitializers => GetResourceString("IDS_FeatureModuleInitializers");

        internal static string ERR_ModuleInitializerMethodMustBeAccessibleOutsideTopLevelType => GetResourceString("ERR_ModuleInitializerMethodMustBeAccessibleOutsideTopLevelType");

        internal static string ERR_ModuleInitializerMethodMustBeStaticParameterlessVoid => GetResourceString("ERR_ModuleInitializerMethodMustBeStaticParameterlessVoid");

        internal static string ERR_ModuleInitializerMethodAndContainingTypesMustNotBeGeneric => GetResourceString("ERR_ModuleInitializerMethodAndContainingTypesMustNotBeGeneric");

        internal static string ERR_ModuleInitializerMethodMustBeOrdinary => GetResourceString("ERR_ModuleInitializerMethodMustBeOrdinary");

        internal static string IDS_FeatureExtensionGetAsyncEnumerator => GetResourceString("IDS_FeatureExtensionGetAsyncEnumerator");

        internal static string IDS_FeatureExtensionGetEnumerator => GetResourceString("IDS_FeatureExtensionGetEnumerator");

        internal static string ERR_UnmanagedCallersOnlyRequiresStatic => GetResourceString("ERR_UnmanagedCallersOnlyRequiresStatic");

        internal static string ERR_InvalidUnmanagedCallersOnlyCallConv => GetResourceString("ERR_InvalidUnmanagedCallersOnlyCallConv");

        internal static string ERR_CannotUseManagedTypeInUnmanagedCallersOnly => GetResourceString("ERR_CannotUseManagedTypeInUnmanagedCallersOnly");

        internal static string ERR_UnmanagedCallersOnlyMethodOrTypeCannotBeGeneric => GetResourceString("ERR_UnmanagedCallersOnlyMethodOrTypeCannotBeGeneric");

        internal static string ERR_UnmanagedCallersOnlyMethodsCannotBeCalledDirectly => GetResourceString("ERR_UnmanagedCallersOnlyMethodsCannotBeCalledDirectly");

        internal static string ERR_UnmanagedCallersOnlyMethodsCannotBeConvertedToDelegate => GetResourceString("ERR_UnmanagedCallersOnlyMethodsCannotBeConvertedToDelegate");

        internal static string ERR_EntryPointCannotBeUnmanagedCallersOnly => GetResourceString("ERR_EntryPointCannotBeUnmanagedCallersOnly");

        internal static string ERR_ModuleInitializerCannotBeUnmanagedCallersOnly => GetResourceString("ERR_ModuleInitializerCannotBeUnmanagedCallersOnly");

        internal static string WRN_RecordEqualsWithoutGetHashCode => GetResourceString("WRN_RecordEqualsWithoutGetHashCode");

        internal static string WRN_RecordEqualsWithoutGetHashCode_Title => GetResourceString("WRN_RecordEqualsWithoutGetHashCode_Title");

        internal static string ERR_InitCannotBeReadonly => GetResourceString("ERR_InitCannotBeReadonly");

        internal static string IDS_FeatureDiscards => GetResourceString("IDS_FeatureDiscards");

        internal static string IDS_FeatureMixedDeclarationsAndExpressionsInDeconstruction => GetResourceString("IDS_FeatureMixedDeclarationsAndExpressionsInDeconstruction");

        internal static string IDS_FeatureRecordStructs => GetResourceString("IDS_FeatureRecordStructs");

        internal static string IDS_FeatureWithOnStructs => GetResourceString("IDS_FeatureWithOnStructs");

        internal static string IDS_FeaturePositionalFieldsInRecords => GetResourceString("IDS_FeaturePositionalFieldsInRecords");

        internal static string IDS_FeatureVarianceSafetyForStaticInterfaceMembers => GetResourceString("IDS_FeatureVarianceSafetyForStaticInterfaceMembers");

        internal static string ERR_EqualityContractRequiresGetter => GetResourceString("ERR_EqualityContractRequiresGetter");

        internal static string WRN_AnalyzerReferencesFramework => GetResourceString("WRN_AnalyzerReferencesFramework");

        internal static string WRN_AnalyzerReferencesFramework_Title => GetResourceString("WRN_AnalyzerReferencesFramework_Title");

        internal static string ERR_BadFieldTypeInRecord => GetResourceString("ERR_BadFieldTypeInRecord");

        internal static string ERR_FunctionPointersCannotBeCalledWithNamedArguments => GetResourceString("ERR_FunctionPointersCannotBeCalledWithNamedArguments");

        internal static string WRN_UnreadRecordParameter => GetResourceString("WRN_UnreadRecordParameter");

        internal static string WRN_UnreadRecordParameter_Title => GetResourceString("WRN_UnreadRecordParameter_Title");

        internal static string ERR_RecordAmbigCtor => GetResourceString("ERR_RecordAmbigCtor");

        internal static string IDS_FeatureLambdaAttributes => GetResourceString("IDS_FeatureLambdaAttributes");

        internal static string IDS_FeatureInferredDelegateType => GetResourceString("IDS_FeatureInferredDelegateType");

        internal static string WRN_DoNotCompareFunctionPointers => GetResourceString("WRN_DoNotCompareFunctionPointers");

        internal static string WRN_DoNotCompareFunctionPointers_Title => GetResourceString("WRN_DoNotCompareFunctionPointers_Title");

        internal static string ERR_FunctionPointerTypesInAttributeNotSupported => GetResourceString("ERR_FunctionPointerTypesInAttributeNotSupported");

        internal static string IDS_FeatureSealedToStringInRecord => GetResourceString("IDS_FeatureSealedToStringInRecord");

        internal static string ERR_InheritingFromRecordWithSealedToString => GetResourceString("ERR_InheritingFromRecordWithSealedToString");

        internal static string ERR_HiddenPositionalMember => GetResourceString("ERR_HiddenPositionalMember");

        internal static string IDS_FeatureGlobalUsing => GetResourceString("IDS_FeatureGlobalUsing");

        internal static string ERR_GlobalUsingInNamespace => GetResourceString("ERR_GlobalUsingInNamespace");

        internal static string ERR_GlobalUsingOutOfOrder => GetResourceString("ERR_GlobalUsingOutOfOrder");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetResourceString(string resourceKey, string defaultValue = null)
        {
            return ResourceManager.GetString(resourceKey, Culture);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class SourceMethodSymbolWithAttributes : SourceMethodSymbol, IAttributeTargetSymbol
    {
        private CustomAttributesBag<CSharpAttributeData> _lazyCustomAttributesBag;

        private CustomAttributesBag<CSharpAttributeData> _lazyReturnTypeCustomAttributesBag;

        protected readonly SyntaxReference syntaxReferenceOpt;

        internal virtual Binder? SignatureBinder => null;

        internal virtual Binder? ParameterBinder => null;

        internal SyntaxReference SyntaxRef => syntaxReferenceOpt;

        internal virtual CSharpSyntaxNode SyntaxNode
        {
            get
            {
                if (syntaxReferenceOpt != null)
                {
                    return (CSharpSyntaxNode)syntaxReferenceOpt.GetSyntax();
                }
                return null;
            }
        }

        internal SyntaxTree SyntaxTree
        {
            get
            {
                if (syntaxReferenceOpt != null)
                {
                    return syntaxReferenceOpt.SyntaxTree;
                }
                return null;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                if (syntaxReferenceOpt != null)
                {
                    return ImmutableArray.Create(syntaxReferenceOpt);
                }
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        public override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => DecodeReturnTypeAnnotationAttributes(GetDecodedReturnTypeWellKnownAttributeData());

        public override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => GetDecodedReturnTypeWellKnownAttributeData()?.NotNullIfParameterNotNull ?? ImmutableHashSet<string>.Empty;

        protected virtual SourceMemberMethodSymbol BoundAttributesSource => null;

        protected virtual IAttributeTargetSymbol AttributeOwner => this;

        IAttributeTargetSymbol IAttributeTargetSymbol.AttributesOwner => AttributeOwner;

        AttributeLocation IAttributeTargetSymbol.DefaultAttributeLocation => AttributeLocation.Method;

        AttributeLocation IAttributeTargetSymbol.AllowedAttributeLocations
        {
            get
            {
                switch (MethodKind)
                {
                    case MethodKind.Constructor:
                    case MethodKind.Destructor:
                    case MethodKind.StaticConstructor:
                        return AttributeLocation.Method;
                    case MethodKind.EventAdd:
                    case MethodKind.EventRemove:
                    case MethodKind.PropertySet:
                        return AttributeLocation.Method | AttributeLocation.Parameter | AttributeLocation.Return;
                    default:
                        return AttributeLocation.Method | AttributeLocation.Return;
                }
            }
        }

        public override bool AreLocalsZeroed
        {
            get
            {
                MethodWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
                if (decodedWellKnownAttributeData == null || !decodedWellKnownAttributeData.HasSkipLocalsInitAttribute)
                {
                    return base.AreContainingSymbolLocalsZeroed;
                }
                return false;
            }
        }

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                if (ContainingSymbol is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol && !sourceMemberContainerTypeSymbol.AnyMemberHasAttributes)
                {
                    return null;
                }
                CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
                if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
                {
                    return ((MethodEarlyWellKnownAttributeData)lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData)?.ObsoleteAttributeData;
                }
                if (syntaxReferenceOpt == null)
                {
                    return null;
                }
                return ObsoleteAttributeData.Uninitialized;
            }
        }

        internal override ImmutableArray<string> NotNullMembers => GetDecodedWellKnownAttributeData()?.NotNullMembers ?? ImmutableArray<string>.Empty;

        internal override ImmutableArray<string> NotNullWhenTrueMembers => GetDecodedWellKnownAttributeData()?.NotNullWhenTrueMembers ?? ImmutableArray<string>.Empty;

        internal override ImmutableArray<string> NotNullWhenFalseMembers => GetDecodedWellKnownAttributeData()?.NotNullWhenFalseMembers ?? ImmutableArray<string>.Empty;

        public override FlowAnalysisAnnotations FlowAnalysisAnnotations => DecodeFlowAnalysisAttributes(GetDecodedWellKnownAttributeData());

        public sealed override bool HidesBaseMethodsByName => false;

        internal sealed override bool HasRuntimeSpecialName
        {
            get
            {
                if (!base.HasRuntimeSpecialName)
                {
                    return IsVtableGapInterfaceMethod();
                }
                return true;
            }
        }

        internal override bool HasSpecialName
        {
            get
            {
                switch (MethodKind)
                {
                    case MethodKind.Constructor:
                    case MethodKind.Conversion:
                    case MethodKind.EventAdd:
                    case MethodKind.EventRemove:
                    case MethodKind.UserDefinedOperator:
                    case MethodKind.PropertyGet:
                    case MethodKind.PropertySet:
                    case MethodKind.StaticConstructor:
                        return true;
                    default:
                        if (IsVtableGapInterfaceMethod())
                        {
                            return true;
                        }
                        return GetDecodedWellKnownAttributeData()?.HasSpecialNameAttribute ?? false;
                }
            }
        }

        internal sealed override bool IsDirectlyExcludedFromCodeCoverage => GetDecodedWellKnownAttributeData()?.HasExcludeFromCodeCoverageAttribute ?? false;

        internal override bool RequiresSecurityObject => GetDecodedWellKnownAttributeData()?.HasDynamicSecurityMethodAttribute ?? false;

        internal override bool HasDeclarativeSecurity => GetDecodedWellKnownAttributeData()?.HasDeclarativeSecurity ?? false;

        internal override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation => GetDecodedReturnTypeWellKnownAttributeData()?.MarshallingInformation;

        internal override MethodImplAttributes ImplementationAttributes
        {
            get
            {
                MethodImplAttributes methodImplAttributes = GetDecodedWellKnownAttributeData()?.MethodImplAttributes ?? MethodImplAttributes.IL;
                if (ContainingType.IsComImport && MethodKind == MethodKind.Constructor)
                {
                    methodImplAttributes |= (MethodImplAttributes)4099;
                }
                return methodImplAttributes;
            }
        }

        protected SourceMethodSymbolWithAttributes(SyntaxReference syntaxReferenceOpt)
        {
            this.syntaxReferenceOpt = syntaxReferenceOpt;
        }

        protected CSharpSyntaxNode? GetInMethodSyntaxNode()
        {
            CSharpSyntaxNode syntaxNode = SyntaxNode;
            if (!(syntaxNode is ConstructorDeclarationSyntax constructorDeclarationSyntax))
            {
                if (!(syntaxNode is BaseMethodDeclarationSyntax baseMethodDeclarationSyntax))
                {
                    if (!(syntaxNode is AccessorDeclarationSyntax accessorDeclarationSyntax))
                    {
                        if (!(syntaxNode is ArrowExpressionClauseSyntax result))
                        {
                            if (!(syntaxNode is LocalFunctionStatementSyntax localFunctionStatementSyntax))
                            {
                                if (!(syntaxNode is CompilationUnitSyntax))
                                {
                                    if (syntaxNode is RecordDeclarationSyntax result2)
                                    {
                                        return result2;
                                    }
                                }
                                else if (this is SynthesizedSimpleProgramEntryPointSymbol synthesizedSimpleProgramEntryPointSymbol)
                                {
                                    return (CSharpSyntaxNode)synthesizedSimpleProgramEntryPointSymbol.ReturnTypeSyntax;
                                }
                                return null;
                            }
                            return (CSharpSyntaxNode?)(localFunctionStatementSyntax.Body ?? ((object)localFunctionStatementSyntax.ExpressionBody));
                        }
                        return result;
                    }
                    return (CSharpSyntaxNode?)(accessorDeclarationSyntax.Body ?? ((object)accessorDeclarationSyntax.ExpressionBody));
                }
                return (CSharpSyntaxNode?)(baseMethodDeclarationSyntax.Body ?? ((object)baseMethodDeclarationSyntax.ExpressionBody));
            }
            return (CSharpSyntaxNode?)(constructorDeclarationSyntax.Initializer ?? constructorDeclarationSyntax.Body ?? ((object)constructorDeclarationSyntax.ExpressionBody));
        }

        internal virtual OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(default(SyntaxList<AttributeListSyntax>));
        }

        internal virtual OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
        {
            return GetAttributeDeclarations();
        }

        internal MethodEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (MethodEarlyWellKnownAttributeData)customAttributesBag.EarlyDecodedWellKnownAttributeData;
        }

        protected MethodWellKnownAttributeData GetDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (MethodWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
        }

        internal ReturnTypeWellKnownAttributeData GetDecodedReturnTypeWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyReturnTypeCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetReturnTypeAttributesBag();
            }
            return (ReturnTypeWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
        }

        private CustomAttributesBag<CSharpAttributeData> GetAttributesBag()
        {
            CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
            if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsSealed)
            {
                return lazyCustomAttributesBag;
            }
            return GetAttributesBag(ref _lazyCustomAttributesBag, forReturnType: false);
        }

        private CustomAttributesBag<CSharpAttributeData> GetReturnTypeAttributesBag()
        {
            CustomAttributesBag<CSharpAttributeData> lazyReturnTypeCustomAttributesBag = _lazyReturnTypeCustomAttributesBag;
            if (lazyReturnTypeCustomAttributesBag != null && lazyReturnTypeCustomAttributesBag.IsSealed)
            {
                return lazyReturnTypeCustomAttributesBag;
            }
            return GetAttributesBag(ref _lazyReturnTypeCustomAttributesBag, forReturnType: true);
        }

        private CustomAttributesBag<CSharpAttributeData> GetAttributesBag(ref CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag, bool forReturnType)
        {
            SourceMemberMethodSymbol boundAttributesSource = BoundAttributesSource;
            bool flag;
            if ((object)boundAttributesSource != null)
            {
                CustomAttributesBag<CSharpAttributeData> value = (forReturnType ? boundAttributesSource.GetReturnTypeAttributesBag() : boundAttributesSource.GetAttributesBag());
                flag = Interlocked.CompareExchange(ref lazyCustomAttributesBag, value, null) == null;
            }
            else
            {
                OneOrMany<SyntaxList<AttributeListSyntax>> attributesSyntaxLists;
                AttributeLocation symbolPart;
                if (!forReturnType)
                {
                    attributesSyntaxLists = GetAttributeDeclarations();
                    symbolPart = AttributeLocation.None;
                }
                else
                {
                    attributesSyntaxLists = GetReturnTypeAttributeDeclarations();
                    symbolPart = AttributeLocation.Return;
                }
                flag = LoadAndValidateAttributes(attributesSyntaxLists, ref lazyCustomAttributesBag, symbolPart, earlyDecodingOnly: false, SignatureBinder);
            }
            if (flag)
            {
                NoteAttributesComplete(forReturnType);
            }
            return lazyCustomAttributesBag;
        }

        protected abstract void NoteAttributesComplete(bool forReturnType);

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return GetAttributesBag().Attributes;
        }

        public override ImmutableArray<CSharpAttributeData> GetReturnTypeAttributes()
        {
            return GetReturnTypeAttributesBag().Attributes;
        }

        internal sealed override CSharpAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
        {
            if (arguments.SymbolPart == AttributeLocation.None)
            {
                if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ConditionalAttribute))
                {
                    CSharpAttributeData attribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out bool generatedDiagnostics);
                    if (!attribute.HasErrors)
                    {
                        string constructorArgument = attribute.GetConstructorArgument<string>(0, SpecialType.System_String);
                        arguments.GetOrCreateData<MethodEarlyWellKnownAttributeData>().AddConditionalSymbol(constructorArgument);
                        if (!generatedDiagnostics)
                        {
                            return attribute;
                        }
                    }
                    return null;
                }
                if (Symbol.EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out var attributeData, out var obsoleteData))
                {
                    if (obsoleteData != null)
                    {
                        arguments.GetOrCreateData<MethodEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
                    }
                    return attributeData;
                }
                if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.UnmanagedCallersOnlyAttribute))
                {
                    arguments.GetOrCreateData<MethodEarlyWellKnownAttributeData>().UnmanagedCallersOnlyAttributePresent = true;
                    return null;
                }
            }
            return base.EarlyDecodeWellKnownAttribute(ref arguments);
        }

        internal sealed override UnmanagedCallersOnlyAttributeData? GetUnmanagedCallersOnlyAttributeData(bool forceComplete)
        {
            if (syntaxReferenceOpt == null)
            {
                return null;
            }
            if (forceComplete)
            {
                GetAttributes();
            }
            CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
            if (lazyCustomAttributesBag == null || !lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
            {
                return UnmanagedCallersOnlyAttributeData.Uninitialized;
            }
            if (lazyCustomAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                return ((MethodWellKnownAttributeData)lazyCustomAttributesBag.DecodedWellKnownAttributeData)?.UnmanagedCallersOnlyAttributeData;
            }
            MethodEarlyWellKnownAttributeData obj = (MethodEarlyWellKnownAttributeData)lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData;
            if (obj == null || !obj.UnmanagedCallersOnlyAttributePresent)
            {
                return null;
            }
            return UnmanagedCallersOnlyAttributeData.AttributePresentDataNotBound;
        }

        internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return GetEarlyDecodedWellKnownAttributeData()?.ConditionalSymbols ?? ImmutableArray<string>.Empty;
        }

        internal sealed override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            if (arguments.SymbolPart == AttributeLocation.None)
            {
                DecodeWellKnownAttributeAppliedToMethod(ref arguments);
            }
            else
            {
                DecodeWellKnownAttributeAppliedToReturnValue(ref arguments);
            }
        }

        private void DecodeWellKnownAttributeAppliedToMethod(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            CSharpAttributeData attribute = arguments.Attribute;
            BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
            if (attribute.IsTargetAttribute(this, AttributeDescription.PreserveSigAttribute))
            {
                arguments.GetOrCreateData<MethodWellKnownAttributeData>().SetPreserveSignature(arguments.Index);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.MethodImplAttribute))
            {
                AttributeData.DecodeMethodImplAttribute<MethodWellKnownAttributeData, AttributeSyntax, CSharpAttributeData, AttributeLocation>(ref arguments, MessageProvider.Instance);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.DllImportAttribute))
            {
                DecodeDllImportAttribute(ref arguments);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SpecialNameAttribute))
            {
                arguments.GetOrCreateData<MethodWellKnownAttributeData>().HasSpecialNameAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ExcludeFromCodeCoverageAttribute))
            {
                arguments.GetOrCreateData<MethodWellKnownAttributeData>().HasExcludeFromCodeCoverageAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ConditionalAttribute))
            {
                ValidateConditionalAttribute(attribute, arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SuppressUnmanagedCodeSecurityAttribute))
            {
                arguments.GetOrCreateData<MethodWellKnownAttributeData>().HasSuppressUnmanagedCodeSecurityAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.DynamicSecurityMethodAttribute))
            {
                arguments.GetOrCreateData<MethodWellKnownAttributeData>().HasDynamicSecurityMethodAttribute = true;
            }
            else
            {
                if (VerifyObsoleteAttributeAppliedToMethod(ref arguments, AttributeDescription.ObsoleteAttribute) || VerifyObsoleteAttributeAppliedToMethod(ref arguments, AttributeDescription.DeprecatedAttribute) || ReportExplicitUseOfReservedAttributes(in arguments, ReservedAttributes.IsReadOnlyAttribute | ReservedAttributes.IsUnmanagedAttribute | ReservedAttributes.IsByRefLikeAttribute | ReservedAttributes.NullableContextAttribute | ReservedAttributes.CaseSensitiveExtensionAttribute))
                {
                    return;
                }
                if (attribute.IsTargetAttribute(this, AttributeDescription.SecurityCriticalAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.SecuritySafeCriticalAttribute))
                {
                    if (IsAsync)
                    {
                        bindingDiagnosticBag.Add(ErrorCode.ERR_SecurityCriticalOrSecuritySafeCriticalOnAsync, arguments.AttributeSyntaxOpt!.Location, arguments.AttributeSyntaxOpt!.GetErrorDisplayName());
                    }
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.SkipLocalsInitAttribute))
                {
                    CSharpAttributeData.DecodeSkipLocalsInitAttribute<MethodWellKnownAttributeData>(DeclaringCompilation, ref arguments);
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.DoesNotReturnAttribute))
                {
                    arguments.GetOrCreateData<MethodWellKnownAttributeData>().HasDoesNotReturnAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.MemberNotNullAttribute))
                {
                    MessageID.IDS_FeatureMemberNotNull.CheckFeatureAvailability(bindingDiagnosticBag, arguments.AttributeSyntaxOpt);
                    CSharpAttributeData.DecodeMemberNotNullAttribute<MethodWellKnownAttributeData>(ContainingType, ref arguments);
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.MemberNotNullWhenAttribute))
                {
                    MessageID.IDS_FeatureMemberNotNull.CheckFeatureAvailability(bindingDiagnosticBag, arguments.AttributeSyntaxOpt);
                    CSharpAttributeData.DecodeMemberNotNullWhenAttribute<MethodWellKnownAttributeData>(ContainingType, ref arguments);
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.ModuleInitializerAttribute))
                {
                    MessageID.IDS_FeatureModuleInitializers.CheckFeatureAvailability(bindingDiagnosticBag, arguments.AttributeSyntaxOpt);
                    DecodeModuleInitializerAttribute(arguments);
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.UnmanagedCallersOnlyAttribute))
                {
                    DecodeUnmanagedCallersOnlyAttribute(ref arguments);
                }
                else
                {
                    CSharpCompilation declaringCompilation = DeclaringCompilation;
                    if (attribute.IsSecurityAttribute(declaringCompilation))
                    {
                        attribute.DecodeSecurityAttribute<MethodWellKnownAttributeData>(this, declaringCompilation, ref arguments);
                    }
                }
            }
        }

        private static FlowAnalysisAnnotations DecodeFlowAnalysisAttributes(MethodWellKnownAttributeData attributeData)
        {
            if (attributeData == null || !attributeData.HasDoesNotReturnAttribute)
            {
                return FlowAnalysisAnnotations.None;
            }
            return FlowAnalysisAnnotations.DoesNotReturn;
        }

        private bool VerifyObsoleteAttributeAppliedToMethod(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments, AttributeDescription description)
        {
            if (arguments.Attribute.IsTargetAttribute(this, description))
            {
                if (this.IsAccessor())
                {
                    BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
                    if (this is SourceEventAccessorSymbol)
                    {
                        AttributeUsageInfo attributeUsageInfo = arguments.Attribute.AttributeClass!.GetAttributeUsageInfo();
                        bindingDiagnosticBag.Add(ErrorCode.ERR_AttributeNotOnEventAccessor, arguments.AttributeSyntaxOpt!.Name.Location, description.FullName, attributeUsageInfo.GetValidTargetsErrorArgument());
                    }
                    else
                    {
                        MessageID.IDS_FeatureObsoleteOnPropertyAccessor.CheckFeatureAvailability(bindingDiagnosticBag, arguments.AttributeSyntaxOpt);
                    }
                }
                return true;
            }
            return false;
        }

        private void ValidateConditionalAttribute(CSharpAttributeData attribute, AttributeSyntax node, BindingDiagnosticBag diagnostics)
        {
            if (this.IsAccessor())
            {
                AttributeUsageInfo attributeUsageInfo = attribute.AttributeClass!.GetAttributeUsageInfo();
                diagnostics.Add(ErrorCode.ERR_AttributeNotOnAccessor, node.Name.Location, node.GetErrorDisplayName(), attributeUsageInfo.GetValidTargetsErrorArgument());
                return;
            }
            if (ContainingType.IsInterfaceType())
            {
                diagnostics.Add(ErrorCode.ERR_ConditionalOnInterfaceMethod, node.Location);
                return;
            }
            if (IsOverride)
            {
                diagnostics.Add(ErrorCode.ERR_ConditionalOnOverride, node.Location, this);
                return;
            }
            if (!base.CanBeReferencedByName || MethodKind == MethodKind.Destructor)
            {
                diagnostics.Add(ErrorCode.ERR_ConditionalOnSpecialMethod, node.Location, this);
                return;
            }
            if (!ReturnsVoid)
            {
                diagnostics.Add(ErrorCode.ERR_ConditionalMustReturnVoid, node.Location, this);
                return;
            }
            if (HasAnyOutParameter())
            {
                diagnostics.Add(ErrorCode.ERR_ConditionalWithOutParam, node.Location, this);
                return;
            }
            if ((object)this != null && MethodKind == MethodKind.LocalFunction && !IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_ConditionalOnLocalFunction, node.Location, this);
                return;
            }
            string constructorArgument = attribute.GetConstructorArgument<string>(0, SpecialType.System_String);
            if (constructorArgument == null || !SyntaxFacts.IsValidIdentifier(constructorArgument))
            {
                CSharpSyntaxNode attributeArgumentSyntax = attribute.GetAttributeArgumentSyntax(0, node);
                diagnostics.Add(ErrorCode.ERR_BadArgumentToAttribute, attributeArgumentSyntax.Location, node.GetErrorDisplayName());
            }
        }

        private bool HasAnyOutParameter()
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.RefKind == RefKind.Out)
                {
                    return true;
                }
            }
            return false;
        }

        private void DecodeWellKnownAttributeAppliedToReturnValue(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            CSharpAttributeData attribute = arguments.Attribute;
            _ = (BindingDiagnosticBag)arguments.Diagnostics;
            if (attribute.IsTargetAttribute(this, AttributeDescription.MarshalAsAttribute))
            {
                MarshalAsAttributeDecoder<ReturnTypeWellKnownAttributeData, AttributeSyntax, CSharpAttributeData, AttributeLocation>.Decode(ref arguments, AttributeTargets.ReturnValue, MessageProvider.Instance);
            }
            else if (!ReportExplicitUseOfReservedAttributes(in arguments, ReservedAttributes.DynamicAttribute | ReservedAttributes.IsReadOnlyAttribute | ReservedAttributes.IsUnmanagedAttribute | ReservedAttributes.IsByRefLikeAttribute | ReservedAttributes.TupleElementNamesAttribute | ReservedAttributes.NullableAttribute | ReservedAttributes.NativeIntegerAttribute))
            {
                if (attribute.IsTargetAttribute(this, AttributeDescription.MaybeNullAttribute))
                {
                    arguments.GetOrCreateData<ReturnTypeWellKnownAttributeData>().HasMaybeNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.NotNullAttribute))
                {
                    arguments.GetOrCreateData<ReturnTypeWellKnownAttributeData>().HasNotNullAttribute = true;
                }
                else if (attribute.IsTargetAttribute(this, AttributeDescription.NotNullIfNotNullAttribute))
                {
                    arguments.GetOrCreateData<ReturnTypeWellKnownAttributeData>().AddNotNullIfParameterNotNull(attribute.DecodeNotNullIfNotNullAttribute());
                }
            }
        }

#nullable enable
        private void DecodeDllImportAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            var attribute = arguments.Attribute;
            var diagnostics = (BindingDiagnosticBag)arguments.Diagnostics;
            bool hasErrors = false;

            var implementationPart = this.PartialImplementationPart ?? this;
            if (!implementationPart.IsExtern || !implementationPart.IsStatic)
            {
                diagnostics.Add(ErrorCode.ERR_DllImportOnInvalidMethod, arguments.AttributeSyntaxOpt.Name.Location);
                hasErrors = true;
            }

            var isAnyNestedMethodGeneric = false;
            for (MethodSymbol? current = this; current is object; current = current.ContainingSymbol as MethodSymbol)
            {
                if (current.IsGenericMethod)
                {
                    isAnyNestedMethodGeneric = true;
                    break;
                }
            }

            if (isAnyNestedMethodGeneric || ContainingType?.IsGenericType == true)
            {
                diagnostics.Add(ErrorCode.ERR_DllImportOnGenericMethod, arguments.AttributeSyntaxOpt.Name.Location);
                hasErrors = true;
            }

            string? moduleName = attribute.GetConstructorArgument<string>(0, SpecialType.System_String);
            if (!MetadataHelpers.IsValidMetadataIdentifier(moduleName))
            {
                // Dev10 reports CS0647: "Error emitting attribute ..."
                CSharpSyntaxNode attributeArgumentSyntax = attribute.GetAttributeArgumentSyntax(0, arguments.AttributeSyntaxOpt);
                diagnostics.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntax.Location, arguments.AttributeSyntaxOpt.GetErrorDisplayName());
                hasErrors = true;
                moduleName = null;
            }

            // Default value of charset is inherited from the module (only if specified).
            // This might be different from ContainingType.DefaultMarshallingCharSet. If the charset is not specified on module
            // ContainingType.DefaultMarshallingCharSet would be Ansi (the class is emitted with "Ansi" charset metadata flag) 
            // while the charset in P/Invoke metadata should be "None".
            CharSet charSet = this.GetEffectiveDefaultMarshallingCharSet() ?? Cci.Constants.CharSet_None;

            string? importName = null;
            bool preserveSig = true;
            System.Runtime.InteropServices.CallingConvention callingConvention = System.Runtime.InteropServices.CallingConvention.Winapi;
            bool setLastError = false;
            bool exactSpelling = false;  // C#: ExactSpelling=false for any charset
            bool? bestFitMapping = null;
            bool? throwOnUnmappable = null;

            int position = 1;
            foreach (var namedArg in attribute.CommonNamedArguments)
            {
                switch (namedArg.Key)
                {
                    case "EntryPoint":
                        importName = namedArg.Value.ValueInternal as string;
                        if (!MetadataHelpers.IsValidMetadataIdentifier(importName))
                        {
                            // Dev10 reports CS0647: "Error emitting attribute ..."
                            diagnostics.Add(ErrorCode.ERR_InvalidNamedArgument, arguments.AttributeSyntaxOpt.ArgumentList.Arguments[position].Location, namedArg.Key);
                            hasErrors = true;
                            importName = null;
                        }

                        break;

                    case "CharSet":
                        // invalid values will be ignored
                        charSet = namedArg.Value.DecodeValue<CharSet>(SpecialType.System_Enum);
                        break;

                    case "SetLastError":
                        // invalid values will be ignored
                        setLastError = namedArg.Value.DecodeValue<bool>(SpecialType.System_Boolean);
                        break;

                    case "ExactSpelling":
                        // invalid values will be ignored
                        exactSpelling = namedArg.Value.DecodeValue<bool>(SpecialType.System_Boolean);
                        break;

                    case "PreserveSig":
                        preserveSig = namedArg.Value.DecodeValue<bool>(SpecialType.System_Boolean);
                        break;

                    case "CallingConvention":
                        // invalid values will be ignored
                        callingConvention = namedArg.Value.DecodeValue<System.Runtime.InteropServices.CallingConvention>(SpecialType.System_Enum);
                        break;

                    case "BestFitMapping":
                        bestFitMapping = namedArg.Value.DecodeValue<bool>(SpecialType.System_Boolean);
                        break;

                    case "ThrowOnUnmappableChar":
                        throwOnUnmappable = namedArg.Value.DecodeValue<bool>(SpecialType.System_Boolean);
                        break;
                }

                position++;
            }

            if (!hasErrors)
            {
                arguments.GetOrCreateData<MethodWellKnownAttributeData>().SetDllImport(
                    arguments.Index,
                    moduleName,
                    importName ?? Name,
                    DllImportData.MakeFlags(
                        exactSpelling,
                        charSet,
                        setLastError,
                        callingConvention,
                        bestFitMapping,
                        throwOnUnmappable),
                    preserveSig);
            }
        }

        private void DecodeModuleInitializerAttribute(DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
            if (MethodKind != MethodKind.Ordinary)
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_ModuleInitializerMethodMustBeOrdinary, arguments.AttributeSyntaxOpt!.Location);
                return;
            }
            bool flag = false;
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(bindingDiagnosticBag, ContainingAssembly);
            if (!AccessCheck.IsSymbolAccessible(this, ContainingAssembly, ref useSiteInfo))
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_ModuleInitializerMethodMustBeAccessibleOutsideTopLevelType, arguments.AttributeSyntaxOpt!.Location, Name);
                flag = true;
            }
            bindingDiagnosticBag.Add(arguments.AttributeSyntaxOpt, useSiteInfo);
            if (!IsStatic || ParameterCount > 0 || !ReturnsVoid)
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_ModuleInitializerMethodMustBeStaticParameterlessVoid, arguments.AttributeSyntaxOpt!.Location, Name);
                flag = true;
            }
            if (IsGenericMethod || ContainingType.IsGenericType)
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_ModuleInitializerMethodAndContainingTypesMustNotBeGeneric, arguments.AttributeSyntaxOpt!.Location, Name);
                flag = true;
            }
            if (_lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData is MethodEarlyWellKnownAttributeData methodEarlyWellKnownAttributeData && methodEarlyWellKnownAttributeData.UnmanagedCallersOnlyAttributePresent)
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_ModuleInitializerCannotBeUnmanagedCallersOnly, arguments.AttributeSyntaxOpt!.Location);
                flag = true;
            }
            if (!flag && !CallsAreOmitted(arguments.AttributeSyntaxOpt!.SyntaxTree))
            {
                DeclaringCompilation.AddModuleInitializerMethod(this);
            }
        }

        private void DecodeUnmanagedCallersOnlyAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            BindingDiagnosticBag diagnostics2 = (BindingDiagnosticBag)arguments.Diagnostics;
            arguments.GetOrCreateData<MethodWellKnownAttributeData>().UnmanagedCallersOnlyAttributeData = DecodeUnmanagedCallersOnlyAttributeData(this, arguments.Attribute, arguments.AttributeSyntaxOpt!.Location, diagnostics2);
            CheckAndReportValidUnmanagedCallersOnlyTarget(arguments.AttributeSyntaxOpt!.Name.Location, diagnostics2);
            CSharpSyntaxNode cSharpSyntaxNode = this.ExtractReturnTypeSyntax();
            if (cSharpSyntaxNode != CSharpSyntaxTree.Dummy.GetRoot())
            {
                checkAndReportManagedTypes(base.ReturnType, cSharpSyntaxNode, isParam: false, diagnostics2);
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ParameterSymbol current = enumerator.Current;
                    checkAndReportManagedTypes(current.Type, current.GetNonNullSyntaxNode(), isParam: true, diagnostics2);
                }
            }
            static void checkAndReportManagedTypes(TypeSymbol type, SyntaxNode syntax, bool isParam, BindingDiagnosticBag diagnostics)
            {
                switch (type.ManagedKindNoUseSiteDiagnostics)
                {
                    case ManagedKind.Unmanaged:
                    case ManagedKind.UnmanagedWithGenerics:
                        break;
                    case ManagedKind.Managed:
                        diagnostics.Add(ErrorCode.ERR_CannotUseManagedTypeInUnmanagedCallersOnly, syntax.Location, type, (isParam ? MessageID.IDS_Parameter : MessageID.IDS_Return).Localize());
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(type.ManagedKindNoUseSiteDiagnostics);
                }
            }
            static UnmanagedCallersOnlyAttributeData DecodeUnmanagedCallersOnlyAttributeData(SourceMethodSymbolWithAttributes @this, CSharpAttributeData attribute, Location location, BindingDiagnosticBag diagnostics)
            {
                ImmutableHashSet<INamedTypeSymbolInternal> callingConventionTypes = null;
                if (!attribute.CommonNamedArguments.IsDefaultOrEmpty)
                {
                    NamedTypeSymbol wellKnownType = @this.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Type);
                    ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator2 = attribute.CommonNamedArguments.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        KeyValuePairUtil.Deconstruct(enumerator2.Current, out var key, out var value);
                        string text = key;
                        TypedConstant value2 = value;
                        bool isField = attribute.AttributeClass!.GetMembers(text).Any((Symbol m, NamedTypeSymbol systemType) => m is FieldSymbol fieldSymbol && fieldSymbol.Type is ArrayTypeSymbol arrayTypeSymbol && arrayTypeSymbol.ElementType is NamedTypeSymbol namedTypeSymbol && namedTypeSymbol.Equals(systemType, TypeCompareKind.ConsiderEverything), wellKnownType);
                        (bool, ImmutableHashSet<INamedTypeSymbolInternal>) tuple = MethodSymbol.TryDecodeUnmanagedCallersOnlyCallConvsField(text, value2, isField, location, diagnostics);
                        if (tuple.Item1)
                        {
                            callingConventionTypes = tuple.Item2;
                        }
                    }
                }
                return UnmanagedCallersOnlyAttributeData.Create(callingConventionTypes);
            }
        }

        internal sealed override void PostDecodeWellKnownAttributes(ImmutableArray<CSharpAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
        {
            if (symbolPart != AttributeLocation.Return)
            {
                if (ContainingSymbol is NamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsComImport && namedTypeSymbol.TypeKind == TypeKind.Class)
                {
                    MethodKind methodKind = MethodKind;
                    if (methodKind == MethodKind.Constructor || methodKind == MethodKind.StaticConstructor)
                    {
                        if (!IsImplicitlyDeclared)
                        {
                            diagnostics.Add(ErrorCode.ERR_ComImportWithUserCtor, Locations[0]);
                        }
                    }
                    else if (!IsAbstract && !IsExtern)
                    {
                        diagnostics.Add(ErrorCode.ERR_ComImportWithImpl, Locations[0], this, ContainingType);
                    }
                }
                if (IsExtern && !IsAbstract && !this.IsPartialMethod() && GetInMethodSyntaxNode() == null && boundAttributes.IsEmpty && !ContainingType.IsComImport)
                {
                    ErrorCode code = ((MethodKind == MethodKind.Constructor || MethodKind == MethodKind.StaticConstructor) ? ErrorCode.WRN_ExternCtorNoImplementation : ErrorCode.WRN_ExternMethodNoImplementation);
                    diagnostics.Add(code, Locations[0], this);
                }
            }
            base.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData);
        }

        protected void AsyncMethodChecks(BindingDiagnosticBag diagnostics)
        {
            if (!IsAsync)
            {
                return;
            }
            Location location = Locations[0];
            bool flag = false;
            if (RefKind != 0)
            {
                CSharpSyntaxNode syntaxNode = SyntaxNode;
                TypeSyntax returnTypeSyntax;
                if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax)
                {
                    TypeSyntax returnType = methodDeclarationSyntax.ReturnType;
                    returnTypeSyntax = returnType;
                }
                else
                {
                    if (!(syntaxNode is LocalFunctionStatementSyntax localFunctionStatementSyntax))
                    {
                        throw ExceptionUtilities.UnexpectedValue(syntaxNode);
                    }
                    TypeSyntax returnType2 = localFunctionStatementSyntax.ReturnType;
                    returnTypeSyntax = returnType2;
                }
                SourceMethodSymbol.ReportBadRefToken(returnTypeSyntax, diagnostics);
                flag = true;
            }
            else if (base.ReturnType.IsBadAsyncReturn(DeclaringCompilation))
            {
                diagnostics.Add(ErrorCode.ERR_BadAsyncReturn, location);
                flag = true;
            }
            NamedTypeSymbol containingType = ContainingType;
            while ((object)containingType != null)
            {
                if (containingType is SourceNamedTypeSymbol sourceNamedTypeSymbol && sourceNamedTypeSymbol.HasSecurityCriticalAttributes)
                {
                    diagnostics.Add(ErrorCode.ERR_SecurityCriticalOrSecuritySafeCriticalOnAsyncInClassOrStruct, location);
                    flag = true;
                    break;
                }
                containingType = containingType.ContainingType;
            }
            if ((ImplementationAttributes & MethodImplAttributes.Synchronized) != 0)
            {
                diagnostics.Add(ErrorCode.ERR_SynchronizedAsyncMethod, location);
                flag = true;
            }
            if (!flag)
            {
                ReportAsyncParameterErrors(diagnostics, location);
            }
            NamedTypeSymbol wellKnownType = DeclaringCompilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T);
            if (base.ReturnType.OriginalDefinition.Equals(wellKnownType) && GetInMethodSyntaxNode() != null)
            {
                NamedTypeSymbol cancellationTokenType = DeclaringCompilation.GetWellKnownType(WellKnownType.System_Threading_CancellationToken);
                int num = Parameters.Count((ParameterSymbol p) => p.IsSourceParameterWithEnumeratorCancellationAttribute());
                if (num == 0 && base.ParameterTypesWithAnnotations.Any((TypeWithAnnotations p) => p.Type.Equals(cancellationTokenType)))
                {
                    diagnostics.Add(ErrorCode.WRN_UndecoratedCancellationTokenParameter, location, this);
                }
                if (num > 1)
                {
                    diagnostics.Add(ErrorCode.ERR_MultipleEnumeratorCancellationAttributes, location);
                }
            }
        }

        private static FlowAnalysisAnnotations DecodeReturnTypeAnnotationAttributes(ReturnTypeWellKnownAttributeData attributeData)
        {
            FlowAnalysisAnnotations flowAnalysisAnnotations = FlowAnalysisAnnotations.None;
            if (attributeData != null)
            {
                if (attributeData.HasMaybeNullAttribute)
                {
                    flowAnalysisAnnotations |= FlowAnalysisAnnotations.MaybeNull;
                }
                if (attributeData.HasNotNullAttribute)
                {
                    flowAnalysisAnnotations |= FlowAnalysisAnnotations.NotNull;
                }
            }
            return flowAnalysisAnnotations;
        }

        private bool IsVtableGapInterfaceMethod()
        {
            if (ContainingType.IsInterface)
            {
                return ModuleExtensions.GetVTableGapSize(MetadataName) > 0;
            }
            return false;
        }

        internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            CustomAttributesBag<CSharpAttributeData> attributesBag = GetAttributesBag();
            MethodWellKnownAttributeData methodWellKnownAttributeData = (MethodWellKnownAttributeData)attributesBag.DecodedWellKnownAttributeData;
            if (methodWellKnownAttributeData != null)
            {
                SecurityWellKnownAttributeData securityInformation = methodWellKnownAttributeData.SecurityInformation;
                if (securityInformation != null)
                {
                    return securityInformation.GetSecurityAttributes(attributesBag.Attributes);
                }
            }
            return SpecializedCollections.EmptyEnumerable<SecurityAttribute>();
        }

        public override DllImportData GetDllImportData()
        {
            return GetDecodedWellKnownAttributeData()?.DllImportPlatformInvokeData;
        }
    }
}

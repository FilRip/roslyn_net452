using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class CSharpAttributeData : AttributeData, ICustomAttribute
    {
        private ThreeState _lazyIsSecurityAttribute;

        int ICustomAttribute.ArgumentCount => CommonConstructorArguments.Length;

        ushort ICustomAttribute.NamedArgumentCount => (ushort)CommonNamedArguments.Length;

        bool ICustomAttribute.AllowMultiple => AttributeClass!.GetAttributeUsageInfo().AllowMultiple;

        public new abstract NamedTypeSymbol? AttributeClass { get; }

        public new abstract MethodSymbol? AttributeConstructor { get; }

        public new abstract SyntaxReference? ApplicationSyntaxReference { get; }

        [MemberNotNullWhen(true, new string[] { "AttributeClass", "AttributeConstructor" })]
        public override bool HasErrors
        {
            [MemberNotNullWhen(true, new string[] { "AttributeClass", "AttributeConstructor" })]
            get
            {
                return base.HasErrors;
            }
        }

        public new IEnumerable<TypedConstant> ConstructorArguments => CommonConstructorArguments;

        public new IEnumerable<KeyValuePair<string, TypedConstant>> NamedArguments => CommonNamedArguments;

        protected override INamedTypeSymbol? CommonAttributeClass => AttributeClass.GetPublicSymbol();

        protected override IMethodSymbol? CommonAttributeConstructor => AttributeConstructor.GetPublicSymbol();

        protected override SyntaxReference? CommonApplicationSyntaxReference => ApplicationSyntaxReference;

        ImmutableArray<IMetadataExpression> ICustomAttribute.GetArguments(EmitContext context)
        {
            ImmutableArray<TypedConstant> commonConstructorArguments = CommonConstructorArguments;
            if (commonConstructorArguments.IsEmpty)
            {
                return ImmutableArray<IMetadataExpression>.Empty;
            }
            ArrayBuilder<IMetadataExpression> instance = ArrayBuilder<IMetadataExpression>.GetInstance();
            ImmutableArray<TypedConstant>.Enumerator enumerator = commonConstructorArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypedConstant current = enumerator.Current;
                instance.Add(CreateMetadataExpression(current, context));
            }
            return instance.ToImmutableAndFree();
        }

        IMethodReference ICustomAttribute.Constructor(EmitContext context, bool reportDiagnostics)
        {
            if (AttributeConstructor.IsDefaultValueTypeConstructor())
            {
                if (reportDiagnostics)
                {
                    context.Diagnostics.Add(ErrorCode.ERR_NotAnAttributeClass, context.SyntaxNode?.Location ?? NoLocation.Singleton, AttributeClass);
                }
                return null;
            }
            return ((PEModuleBuilder)context.Module).Translate(AttributeConstructor, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }

        ImmutableArray<IMetadataNamedArgument> ICustomAttribute.GetNamedArguments(EmitContext context)
        {
            ImmutableArray<KeyValuePair<string, TypedConstant>> commonNamedArguments = CommonNamedArguments;
            if (commonNamedArguments.IsEmpty)
            {
                return ImmutableArray<IMetadataNamedArgument>.Empty;
            }
            ArrayBuilder<IMetadataNamedArgument> instance = ArrayBuilder<IMetadataNamedArgument>.GetInstance();
            ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator = commonNamedArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, TypedConstant> current = enumerator.Current;
                instance.Add(CreateMetadataNamedArgument(current.Key, current.Value, context));
            }
            return instance.ToImmutableAndFree();
        }

        ITypeReference ICustomAttribute.GetType(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(AttributeClass, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }

        private IMetadataExpression CreateMetadataExpression(TypedConstant argument, EmitContext context)
        {
            if (argument.IsNull)
            {
                return CreateMetadataConstant(argument.TypeInternal, null, context);
            }
            return argument.Kind switch
            {
                TypedConstantKind.Array => CreateMetadataArray(argument, context),
                TypedConstantKind.Type => CreateType(argument, context),
                _ => CreateMetadataConstant(argument.TypeInternal, argument.ValueInternal, context),
            };
        }

        private MetadataCreateArray CreateMetadataArray(TypedConstant argument, EmitContext context)
        {
            ImmutableArray<TypedConstant> values = argument.Values;
            IArrayTypeReference arrayTypeReference = ((PEModuleBuilder)context.Module).Translate((ArrayTypeSymbol)argument.TypeInternal);
            if (values.Length == 0)
            {
                return new MetadataCreateArray(arrayTypeReference, arrayTypeReference.GetElementType(context), ImmutableArray<IMetadataExpression>.Empty);
            }
            IMetadataExpression[] array = new IMetadataExpression[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = CreateMetadataExpression(values[i], context);
            }
            return new MetadataCreateArray(arrayTypeReference, arrayTypeReference.GetElementType(context), array.AsImmutableOrNull());
        }

        private static MetadataTypeOf CreateType(TypedConstant argument, EmitContext context)
        {
            PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
            CSharpSyntaxNode syntaxNodeOpt = (CSharpSyntaxNode)context.SyntaxNode;
            DiagnosticBag diagnostics = context.Diagnostics;
            return new MetadataTypeOf(pEModuleBuilder.Translate((TypeSymbol)argument.ValueInternal, syntaxNodeOpt, diagnostics), pEModuleBuilder.Translate((TypeSymbol)argument.TypeInternal, syntaxNodeOpt, diagnostics));
        }

        private static MetadataConstant CreateMetadataConstant(ITypeSymbolInternal type, object value, EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).CreateConstant((TypeSymbol)type, value, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }

        private IMetadataNamedArgument CreateMetadataNamedArgument(string name, TypedConstant argument, EmitContext context)
        {
            Symbol symbol = LookupName(name);
            IMetadataExpression value = CreateMetadataExpression(argument, context);
            TypeSymbol symbol2 = ((!(symbol is FieldSymbol fieldSymbol)) ? ((PropertySymbol)symbol).Type : fieldSymbol.Type);
            PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
            return new MetadataNamedArgument(symbol, pEModuleBuilder.Translate(symbol2, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics), value);
        }

        private Symbol LookupName(string name)
        {
            NamedTypeSymbol namedTypeSymbol = AttributeClass;
            while ((object)namedTypeSymbol != null)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = namedTypeSymbol.GetMembers(name).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.DeclaredAccessibility == Accessibility.Public)
                    {
                        return current;
                    }
                }
                namedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
            }
            return null;
        }

        internal virtual bool IsTargetAttribute(string namespaceName, string typeName)
        {
            if (!AttributeClass!.Name.Equals(typeName))
            {
                return false;
            }
            if (AttributeClass.IsErrorType() && !(AttributeClass is MissingMetadataTypeSymbol))
            {
                return false;
            }
            return AttributeClass.HasNameQualifier(namespaceName);
        }

        internal bool IsTargetAttribute(Symbol targetSymbol, AttributeDescription description)
        {
            return GetTargetAttributeSignatureIndex(targetSymbol, description) != -1;
        }

        internal abstract int GetTargetAttributeSignatureIndex(Symbol targetSymbol, AttributeDescription description);

        internal static bool IsTargetEarlyAttribute(NamedTypeSymbol attributeType, AttributeSyntax attributeSyntax, AttributeDescription description)
        {
            int attributeArgCount = ((attributeSyntax.ArgumentList != null) ? attributeSyntax.ArgumentList!.Arguments.Count((AttributeArgumentSyntax arg) => arg.NameEquals == null) : 0);
            return AttributeData.IsTargetEarlyAttribute(attributeType, attributeArgCount, description);
        }

        internal bool IsSecurityAttribute(CSharpCompilation compilation)
        {
            if (_lazyIsSecurityAttribute == ThreeState.Unknown)
            {
                NamedTypeSymbol wellKnownType = compilation.GetWellKnownType(WellKnownType.System_Security_Permissions_SecurityAttribute);
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                _lazyIsSecurityAttribute = AttributeClass!.IsDerivedFrom(wellKnownType, TypeCompareKind.ConsiderEverything, ref useSiteInfo).ToThreeState();
            }
            return _lazyIsSecurityAttribute.Value();
        }

        public override string? ToString()
        {
            if ((object)AttributeClass != null)
            {
                string text = AttributeClass!.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);
                if (!CommonConstructorArguments.Any() & !CommonNamedArguments.Any())
                {
                    return text;
                }
                PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                StringBuilder builder = instance.Builder;
                builder.Append(text);
                builder.Append("(");
                bool flag = true;
                ImmutableArray<TypedConstant>.Enumerator enumerator = CommonConstructorArguments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypedConstant current = enumerator.Current;
                    if (!flag)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(current.ToCSharpString());
                    flag = false;
                }
                ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator2 = CommonNamedArguments.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    KeyValuePair<string, TypedConstant> current2 = enumerator2.Current;
                    if (!flag)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(current2.Key);
                    builder.Append(" = ");
                    builder.Append(current2.Value.ToCSharpString());
                    flag = false;
                }
                builder.Append(")");
                return instance.ToStringAndFree();
            }
            return base.ToString();
        }

        internal void DecodeSecurityAttribute<T>(Symbol targetSymbol, CSharpCompilation compilation, ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments) where T : WellKnownAttributeData, ISecurityAttributeTarget, new()
        {
            DeclarativeSecurityAction action = DecodeSecurityAttributeAction(targetSymbol, compilation, arguments.AttributeSyntaxOpt, out bool hasErrors, (BindingDiagnosticBag)arguments.Diagnostics);
            if (hasErrors)
            {
                return;
            }
            SecurityWellKnownAttributeData orCreateData = arguments.GetOrCreateData<T>().GetOrCreateData();
            orCreateData.SetSecurityAttribute(arguments.Index, action, arguments.AttributesCount);
            if (IsTargetAttribute(targetSymbol, AttributeDescription.PermissionSetAttribute))
            {
                string text = DecodePermissionSetAttribute(compilation, arguments.AttributeSyntaxOpt, (BindingDiagnosticBag)arguments.Diagnostics);
                if (text != null)
                {
                    orCreateData.SetPathForPermissionSetAttributeFixup(arguments.Index, text, arguments.AttributesCount);
                }
            }
        }

        internal static void DecodeSkipLocalsInitAttribute<T>(CSharpCompilation compilation, ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments) where T : WellKnownAttributeData, ISkipLocalsInitAttributeTarget, new()
        {
            arguments.GetOrCreateData<T>().HasSkipLocalsInitAttribute = true;
            if (!compilation.Options.AllowUnsafe)
            {
                ((BindingDiagnosticBag)arguments.Diagnostics).Add(ErrorCode.ERR_IllegalUnsafe, arguments.AttributeSyntaxOpt!.Location);
            }
        }

        internal static void DecodeMemberNotNullAttribute<T>(TypeSymbol type, ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments) where T : WellKnownAttributeData, IMemberNotNullAttributeTarget, new()
        {
            TypedConstant typedConstant = arguments.Attribute.CommonConstructorArguments[0];
            if (typedConstant.IsNull)
            {
                return;
            }
            if (typedConstant.Kind != TypedConstantKind.Array)
            {
                string text = typedConstant.DecodeValue<string>(SpecialType.System_String);
                if (text != null)
                {
                    arguments.GetOrCreateData<T>().AddNotNullMember(text);
                    ReportBadNotNullMemberIfNeeded(type, arguments, text);
                }
                return;
            }
            ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
            ImmutableArray<TypedConstant>.Enumerator enumerator = typedConstant.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string text2 = enumerator.Current.DecodeValue<string>(SpecialType.System_String);
                if (text2 != null)
                {
                    instance.Add(text2);
                    ReportBadNotNullMemberIfNeeded(type, arguments, text2);
                }
            }
            arguments.GetOrCreateData<T>().AddNotNullMember(instance);
            instance.Free();
        }

        private static void ReportBadNotNullMemberIfNeeded(TypeSymbol type, DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments, string memberName)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = type.GetMembers(memberName).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Field || current.Kind == SymbolKind.Property)
                {
                    return;
                }
            }
            ((BindingDiagnosticBag)arguments.Diagnostics).Add(ErrorCode.WRN_MemberNotNullBadMember, arguments.AttributeSyntaxOpt!.Location, memberName);
        }

        internal static void DecodeMemberNotNullWhenAttribute<T>(TypeSymbol type, ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments) where T : WellKnownAttributeData, IMemberNotNullAttributeTarget, new()
        {
            TypedConstant typedConstant = arguments.Attribute.CommonConstructorArguments[1];
            if (typedConstant.IsNull)
            {
                return;
            }
            bool sense = arguments.Attribute.CommonConstructorArguments[0].DecodeValue<bool>(SpecialType.System_Boolean);
            if (typedConstant.Kind != TypedConstantKind.Array)
            {
                string text = typedConstant.DecodeValue<string>(SpecialType.System_String);
                if (text != null)
                {
                    arguments.GetOrCreateData<T>().AddNotNullWhenMember(sense, text);
                    ReportBadNotNullMemberIfNeeded(type, arguments, text);
                }
                return;
            }
            ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
            ImmutableArray<TypedConstant>.Enumerator enumerator = typedConstant.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string text2 = enumerator.Current.DecodeValue<string>(SpecialType.System_String);
                if (text2 != null)
                {
                    instance.Add(text2);
                    ReportBadNotNullMemberIfNeeded(type, arguments, text2);
                }
            }
            arguments.GetOrCreateData<T>().AddNotNullWhenMember(sense, instance);
            instance.Free();
        }

        private DeclarativeSecurityAction DecodeSecurityAttributeAction(Symbol targetSymbol, CSharpCompilation compilation, AttributeSyntax? nodeOpt, out bool hasErrors, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<TypedConstant> commonConstructorArguments = CommonConstructorArguments;
            if (!commonConstructorArguments.Any())
            {
                if (IsTargetAttribute(targetSymbol, AttributeDescription.HostProtectionAttribute))
                {
                    hasErrors = false;
                    return DeclarativeSecurityAction.LinkDemand;
                }
            }
            else
            {
                TypedConstant typedValue = commonConstructorArguments.First();
                TypeSymbol typeSymbol = (TypeSymbol)typedValue.TypeInternal;
                if ((object)typeSymbol != null && typeSymbol.Equals(compilation.GetWellKnownType(WellKnownType.System_Security_Permissions_SecurityAction)))
                {
                    return DecodeSecurityAction(typedValue, targetSymbol, nodeOpt, diagnostics, out hasErrors);
                }
            }
            diagnostics.Add(ErrorCode.ERR_SecurityAttributeMissingAction, (nodeOpt != null) ? nodeOpt!.Name.Location : NoLocation.Singleton);
            hasErrors = true;
            return DeclarativeSecurityAction.None;
        }

        private DeclarativeSecurityAction DecodeSecurityAction(TypedConstant typedValue, Symbol targetSymbol, AttributeSyntax? nodeOpt, BindingDiagnosticBag diagnostics, out bool hasErrors)
        {
            int num = (int)typedValue.ValueInternal;
            bool flag;
            switch (num)
            {
                case 6:
                case 7:
                    if (IsTargetAttribute(targetSymbol, AttributeDescription.PrincipalPermissionAttribute))
                    {
                        Location securityAttributeActionSyntaxLocation2 = GetSecurityAttributeActionSyntaxLocation(nodeOpt, typedValue, out object displayString2);
                        diagnostics.Add(ErrorCode.ERR_PrincipalPermissionInvalidAction, securityAttributeActionSyntaxLocation2, displayString2);
                        hasErrors = true;
                        return DeclarativeSecurityAction.None;
                    }
                    flag = false;
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    flag = false;
                    break;
                case 8:
                case 9:
                case 10:
                    flag = true;
                    break;
                default:
                    {
                        Location securityAttributeActionSyntaxLocation = GetSecurityAttributeActionSyntaxLocation(nodeOpt, typedValue, out object displayString);
                        diagnostics.Add(ErrorCode.ERR_SecurityAttributeInvalidAction, securityAttributeActionSyntaxLocation, (nodeOpt != null) ? nodeOpt!.GetErrorDisplayName() : "", displayString);
                        hasErrors = true;
                        return DeclarativeSecurityAction.None;
                    }
            }
            if (flag)
            {
                if (targetSymbol.Kind == SymbolKind.NamedType || targetSymbol.Kind == SymbolKind.Method)
                {
                    Location securityAttributeActionSyntaxLocation3 = GetSecurityAttributeActionSyntaxLocation(nodeOpt, typedValue, out object displayString3);
                    diagnostics.Add(ErrorCode.ERR_SecurityAttributeInvalidActionTypeOrMethod, securityAttributeActionSyntaxLocation3, displayString3);
                    hasErrors = true;
                    return DeclarativeSecurityAction.None;
                }
            }
            else if (targetSymbol.Kind == SymbolKind.Assembly)
            {
                Location securityAttributeActionSyntaxLocation4 = GetSecurityAttributeActionSyntaxLocation(nodeOpt, typedValue, out object displayString4);
                diagnostics.Add(ErrorCode.ERR_SecurityAttributeInvalidActionAssembly, securityAttributeActionSyntaxLocation4, displayString4);
                hasErrors = true;
                return DeclarativeSecurityAction.None;
            }
            hasErrors = false;
            return (DeclarativeSecurityAction)num;
        }

        private static Location GetSecurityAttributeActionSyntaxLocation(AttributeSyntax? nodeOpt, TypedConstant typedValue, out object displayString)
        {
            if (nodeOpt == null)
            {
                displayString = "";
                return NoLocation.Singleton;
            }

            var argList = nodeOpt.ArgumentList;
            if (argList == null || argList.Arguments.IsEmpty())
            {
                // Optional SecurityAction parameter with default value.
                displayString = $"{typedValue.ValueInternal}"; // FilRip : Remove FormattableString cast
                return nodeOpt.Location;
            }

            AttributeArgumentSyntax argSyntax = argList.Arguments[0];
            displayString = argSyntax.ToString();
            return argSyntax.Location;
        }

        private string? DecodePermissionSetAttribute(CSharpCompilation compilation, AttributeSyntax? nodeOpt, BindingDiagnosticBag diagnostics)
        {
            string text = null;
            ImmutableArray<KeyValuePair<string, TypedConstant>> commonNamedArguments = CommonNamedArguments;
            if (commonNamedArguments.Length == 1)
            {
                KeyValuePair<string, TypedConstant> keyValuePair = commonNamedArguments[0];
                NamedTypeSymbol attributeClass = AttributeClass;
                string text2 = "File";
                string propName = "Hex";
                if (keyValuePair.Key == text2 && PermissionSetAttributeTypeHasRequiredProperty(attributeClass, text2))
                {
                    string text3 = (string)keyValuePair.Value.ValueInternal;
                    XmlReferenceResolver xmlReferenceResolver = compilation.Options.XmlReferenceResolver;
                    text = ((xmlReferenceResolver != null && text3 != null) ? xmlReferenceResolver.ResolveReference(text3, null) : null);
                    if (text == null)
                    {
                        Location location = nodeOpt?.GetNamedArgumentSyntax(text2)?.Location ?? NoLocation.Singleton;
                        diagnostics.Add(ErrorCode.ERR_PermissionSetAttributeInvalidFile, location, text3 ?? "<null>", text2);
                    }
                    else if (!PermissionSetAttributeTypeHasRequiredProperty(attributeClass, propName))
                    {
                        return null;
                    }
                }
            }
            return text;
        }

        private static bool PermissionSetAttributeTypeHasRequiredProperty(NamedTypeSymbol permissionSetType, string propName)
        {
            ImmutableArray<Symbol> members = permissionSetType.GetMembers(propName);
            if (members.Length == 1 && members[0].Kind == SymbolKind.Property)
            {
                PropertySymbol propertySymbol = (PropertySymbol)members[0];
                if (propertySymbol.TypeWithAnnotations.HasType && propertySymbol.Type.SpecialType == SpecialType.System_String && propertySymbol.DeclaredAccessibility == Accessibility.Public && propertySymbol.GetMemberArity() == 0 && (object)propertySymbol.SetMethod != null && propertySymbol.SetMethod.DeclaredAccessibility == Accessibility.Public)
                {
                    return true;
                }
            }
            return false;
        }

        internal void DecodeClassInterfaceAttribute(AttributeSyntax? nodeOpt, BindingDiagnosticBag diagnostics)
        {
            TypedConstant typedConstant = CommonConstructorArguments[0];
            ClassInterfaceType classInterfaceType = ((typedConstant.Kind == TypedConstantKind.Enum) ? typedConstant.DecodeValue<ClassInterfaceType>(SpecialType.System_Enum) : ((ClassInterfaceType)typedConstant.DecodeValue<short>(SpecialType.System_Int16)));
            if ((uint)classInterfaceType > 2u)
            {
                Location attributeArgumentSyntaxLocation = this.GetAttributeArgumentSyntaxLocation(0, nodeOpt);
                diagnostics.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntaxLocation, (nodeOpt != null) ? nodeOpt!.GetErrorDisplayName() : "");
            }
        }

        internal void DecodeInterfaceTypeAttribute(AttributeSyntax node, BindingDiagnosticBag diagnostics)
        {
            TypedConstant typedConstant = CommonConstructorArguments[0];
            ComInterfaceType comInterfaceType = ((typedConstant.Kind == TypedConstantKind.Enum) ? typedConstant.DecodeValue<ComInterfaceType>(SpecialType.System_Enum) : ((ComInterfaceType)typedConstant.DecodeValue<short>(SpecialType.System_Int16)));
            if ((uint)comInterfaceType > 3u)
            {
                CSharpSyntaxNode attributeArgumentSyntax = this.GetAttributeArgumentSyntax(0, node);
                diagnostics.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntax.Location, node.GetErrorDisplayName());
            }
        }

        internal string DecodeGuidAttribute(AttributeSyntax? nodeOpt, BindingDiagnosticBag diagnostics)
        {
            string text = (string)CommonConstructorArguments[0].ValueInternal;
            if (!Guid.TryParseExact(text, "D", out var _))
            {
                Location attributeArgumentSyntaxLocation = this.GetAttributeArgumentSyntaxLocation(0, nodeOpt);
                diagnostics.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntaxLocation, (nodeOpt != null) ? nodeOpt!.GetErrorDisplayName() : "");
                text = string.Empty;
            }
            return text;
        }

        protected sealed override bool IsStringProperty(string memberName)
        {
            if ((object)AttributeClass != null)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = AttributeClass!.GetMembers(memberName).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is PropertySymbol propertySymbol)
                    {
                        TypeSymbol type = propertySymbol.Type;
                        if ((object)type != null && type.SpecialType == SpecialType.System_String)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal bool ShouldEmitAttribute(Symbol target, bool isReturnType, bool emittingAssemblyAttributesInNetModule)
        {
            if (HasErrors)
            {
                throw ExceptionUtilities.Unreachable;
            }
            if (IsConditionallyOmitted)
            {
                return false;
            }
            switch (target.Kind)
            {
                case SymbolKind.Assembly:
                    if ((!emittingAssemblyAttributesInNetModule && (IsTargetAttribute(target, AttributeDescription.AssemblyCultureAttribute) || IsTargetAttribute(target, AttributeDescription.AssemblyVersionAttribute) || IsTargetAttribute(target, AttributeDescription.AssemblyFlagsAttribute) || IsTargetAttribute(target, AttributeDescription.AssemblyAlgorithmIdAttribute))) || IsTargetAttribute(target, AttributeDescription.TypeForwardedToAttribute) || IsSecurityAttribute(target.DeclaringCompilation))
                    {
                        return false;
                    }
                    break;
                case SymbolKind.Event:
                    if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute))
                    {
                        return false;
                    }
                    break;
                case SymbolKind.Field:
                    if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) || IsTargetAttribute(target, AttributeDescription.NonSerializedAttribute) || IsTargetAttribute(target, AttributeDescription.FieldOffsetAttribute) || IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute))
                    {
                        return false;
                    }
                    break;
                case SymbolKind.Method:
                    if (isReturnType)
                    {
                        if (IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute))
                        {
                            return false;
                        }
                    }
                    else if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) || IsTargetAttribute(target, AttributeDescription.MethodImplAttribute) || IsTargetAttribute(target, AttributeDescription.DllImportAttribute) || IsTargetAttribute(target, AttributeDescription.PreserveSigAttribute) || IsTargetAttribute(target, AttributeDescription.DynamicSecurityMethodAttribute) || IsSecurityAttribute(target.DeclaringCompilation))
                    {
                        return false;
                    }
                    break;
                case SymbolKind.NamedType:
                    if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) || IsTargetAttribute(target, AttributeDescription.ComImportAttribute) || IsTargetAttribute(target, AttributeDescription.SerializableAttribute) || IsTargetAttribute(target, AttributeDescription.StructLayoutAttribute) || IsTargetAttribute(target, AttributeDescription.WindowsRuntimeImportAttribute) || IsSecurityAttribute(target.DeclaringCompilation))
                    {
                        return false;
                    }
                    break;
                case SymbolKind.Parameter:
                    if (IsTargetAttribute(target, AttributeDescription.OptionalAttribute) || IsTargetAttribute(target, AttributeDescription.DefaultParameterValueAttribute) || IsTargetAttribute(target, AttributeDescription.InAttribute) || IsTargetAttribute(target, AttributeDescription.OutAttribute) || IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute))
                    {
                        return false;
                    }
                    break;
                case SymbolKind.Property:
                    if (IsTargetAttribute(target, AttributeDescription.IndexerNameAttribute) || IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) || IsTargetAttribute(target, AttributeDescription.DisallowNullAttribute) || IsTargetAttribute(target, AttributeDescription.AllowNullAttribute) || IsTargetAttribute(target, AttributeDescription.MaybeNullAttribute) || IsTargetAttribute(target, AttributeDescription.NotNullAttribute))
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }
    }
}

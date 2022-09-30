using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public class SourceAttributeData : CSharpAttributeData
    {
        private readonly NamedTypeSymbol _attributeClass;

        private readonly MethodSymbol? _attributeConstructor;

        private readonly ImmutableArray<TypedConstant> _constructorArguments;

        private readonly ImmutableArray<int> _constructorArgumentsSourceIndices;

        private readonly ImmutableArray<KeyValuePair<string, TypedConstant>> _namedArguments;

        private readonly bool _isConditionallyOmitted;

        private readonly bool _hasErrors;

        private readonly SyntaxReference? _applicationNode;

        public override NamedTypeSymbol AttributeClass => _attributeClass;

        public override MethodSymbol? AttributeConstructor => _attributeConstructor;

        public override SyntaxReference? ApplicationSyntaxReference => _applicationNode;

        internal ImmutableArray<int> ConstructorArgumentsSourceIndices => _constructorArgumentsSourceIndices;

        public override bool IsConditionallyOmitted => _isConditionallyOmitted;

        [MemberNotNullWhen(true, new string[] { "AttributeClass", "AttributeConstructor" })]
        public override bool HasErrors
        {
            [MemberNotNullWhen(true, new string[] { "AttributeClass", "AttributeConstructor" })]
            get
            {
                return _hasErrors;
            }
        }

        public override ImmutableArray<TypedConstant> CommonConstructorArguments => _constructorArguments;

        public override ImmutableArray<KeyValuePair<string, TypedConstant>> CommonNamedArguments => _namedArguments;

        internal SourceAttributeData(SyntaxReference? applicationNode, NamedTypeSymbol attributeClass, MethodSymbol? attributeConstructor, ImmutableArray<TypedConstant> constructorArguments, ImmutableArray<int> constructorArgumentsSourceIndices, ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments, bool hasErrors, bool isConditionallyOmitted)
        {
            _attributeClass = attributeClass;
            _attributeConstructor = attributeConstructor;
            _constructorArguments = constructorArguments;
            _constructorArgumentsSourceIndices = constructorArgumentsSourceIndices;
            _namedArguments = namedArguments;
            _isConditionallyOmitted = isConditionallyOmitted;
            _hasErrors = hasErrors;
            _applicationNode = applicationNode;
        }

        internal SourceAttributeData(SyntaxReference applicationNode, NamedTypeSymbol attributeClass, MethodSymbol? attributeConstructor, bool hasErrors)
            : this(applicationNode, attributeClass, attributeConstructor, ImmutableArray<TypedConstant>.Empty, default(ImmutableArray<int>), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty, hasErrors, isConditionallyOmitted: false)
        {
        }

        internal CSharpSyntaxNode GetAttributeArgumentSyntax(int parameterIndex, AttributeSyntax attributeSyntax)
        {
            if (_constructorArgumentsSourceIndices.IsDefault)
            {
                return attributeSyntax.ArgumentList!.Arguments[parameterIndex];
            }
            int num = _constructorArgumentsSourceIndices[parameterIndex];
            if (num == -1)
            {
                return attributeSyntax.Name;
            }
            return attributeSyntax.ArgumentList!.Arguments[num];
        }

        internal SourceAttributeData WithOmittedCondition(bool isConditionallyOmitted)
        {
            if (IsConditionallyOmitted == isConditionallyOmitted)
            {
                return this;
            }
            return new SourceAttributeData(ApplicationSyntaxReference, AttributeClass, AttributeConstructor, CommonConstructorArguments, ConstructorArgumentsSourceIndices, CommonNamedArguments, HasErrors, isConditionallyOmitted);
        }

        internal override int GetTargetAttributeSignatureIndex(Symbol targetSymbol, AttributeDescription description)
        {
            Symbol targetSymbol2 = targetSymbol;
            if (!IsTargetAttribute(description.Namespace, description.Name))
            {
                return -1;
            }
            MethodSymbol attributeConstructor = AttributeConstructor;
            if ((object)attributeConstructor == null)
            {
                return -1;
            }
            TypeSymbol lazySystemType2 = null;
            ImmutableArray<ParameterSymbol> parameters2 = attributeConstructor.Parameters;
            for (int i = 0; i < description.Signatures.Length; i++)
            {
                byte[] targetSignature2 = description.Signatures[i];
                if (matches(targetSignature2, parameters2, ref lazySystemType2))
                {
                    return i;
                }
            }
            return -1;
            bool matches(byte[] targetSignature, ImmutableArray<ParameterSymbol> parameters, ref TypeSymbol? lazySystemType)
            {
                if (targetSignature[0] != 32)
                {
                    return false;
                }
                if (targetSignature[1] != parameters.Length)
                {
                    return false;
                }
                if (targetSignature[2] != 1)
                {
                    return false;
                }
                int num = 0;
                for (int j = 3; j < targetSignature.Length; j++)
                {
                    if (num >= parameters.Length)
                    {
                        return false;
                    }
                    TypeSymbol type = parameters[num].Type;
                    SpecialType specialType = type.SpecialType;
                    byte b = targetSignature[j];
                    switch (b)
                    {
                        case 64:
                            {
                                j++;
                                if (type.Kind != SymbolKind.NamedType && type.Kind != SymbolKind.ErrorType)
                                {
                                    return false;
                                }
                                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
                                AttributeDescription.TypeHandleTargetInfo typeHandleTargetInfo = AttributeDescription.TypeHandleTargets[targetSignature[j]];
                                if (!string.Equals(namedTypeSymbol.MetadataName, typeHandleTargetInfo.Name, StringComparison.Ordinal) || !namedTypeSymbol.HasNameQualifier(typeHandleTargetInfo.Namespace))
                                {
                                    return false;
                                }
                                b = (byte)typeHandleTargetInfo.Underlying;
                                if (type.IsEnumType())
                                {
                                    specialType = type.GetEnumUnderlyingType()!.SpecialType;
                                }
                                break;
                            }
                        default:
                            if (type.IsArray())
                            {
                                if (targetSignature[j - 1] != 29)
                                {
                                    return false;
                                }
                                specialType = ((ArrayTypeSymbol)type).ElementType.SpecialType;
                            }
                            break;
                        case 29:
                            break;
                    }
                    switch (b)
                    {
                        case 2:
                            if (specialType != SpecialType.System_Boolean)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 3:
                            if (specialType != SpecialType.System_Char)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 4:
                            if (specialType != SpecialType.System_SByte)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 5:
                            if (specialType != SpecialType.System_Byte)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 6:
                            if (specialType != SpecialType.System_Int16)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 7:
                            if (specialType != SpecialType.System_UInt16)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 8:
                            if (specialType != SpecialType.System_Int32)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 9:
                            if (specialType != SpecialType.System_UInt32)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 10:
                            if (specialType != SpecialType.System_Int64)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 11:
                            if (specialType != SpecialType.System_UInt64)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 12:
                            if (specialType != SpecialType.System_Single)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 13:
                            if (specialType != SpecialType.System_Double)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 14:
                            if (specialType != SpecialType.System_String)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 28:
                            if (specialType != SpecialType.System_Object)
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 80:
                            if ((object)lazySystemType == null)
                            {
                                lazySystemType = GetSystemType(targetSymbol2);
                            }
                            if (!TypeSymbol.Equals(type, lazySystemType, TypeCompareKind.ConsiderEverything))
                            {
                                return false;
                            }
                            num++;
                            break;
                        case 29:
                            if (!type.IsArray())
                            {
                                return false;
                            }
                            break;
                        default:
                            return false;
                    }
                }
                return true;
            }
        }

        internal virtual TypeSymbol GetSystemType(Symbol targetSymbol)
        {
            return targetSymbol.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Type);
        }
    }
}

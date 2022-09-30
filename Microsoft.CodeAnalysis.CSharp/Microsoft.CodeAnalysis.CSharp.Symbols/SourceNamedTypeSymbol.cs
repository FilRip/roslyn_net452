using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceNamedTypeSymbol : SourceMemberContainerTypeSymbol, IAttributeTargetSymbol
    {
        private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

        private ImmutableArray<ImmutableArray<TypeWithAnnotations>> _lazyTypeParameterConstraintTypes;

        private ImmutableArray<TypeParameterConstraintKind> _lazyTypeParameterConstraintKinds;

        private CustomAttributesBag<CSharpAttributeData> _lazyCustomAttributesBag;

        private string _lazyDocComment;

        private string _lazyExpandedDocComment;

        private ThreeState _lazyIsExplicitDefinitionOfNoPiaLocalType;

        private Tuple<NamedTypeSymbol, ImmutableArray<NamedTypeSymbol>> _lazyDeclaredBases;

        private NamedTypeSymbol _lazyBaseType = ErrorTypeSymbol.UnknownResultType;

        private ImmutableArray<NamedTypeSymbol> _lazyInterfaces;

        private SynthesizedEnumValueFieldSymbol _lazyEnumValueField;

        private NamedTypeSymbol _lazyEnumUnderlyingType = ErrorTypeSymbol.UnknownResultType;

        internal sealed override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics => GetTypeParametersAsTypeArguments();

        public override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get
            {
                if (_lazyTypeParameters.IsDefault)
                {
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                    if (ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameters, MakeTypeParameters(instance)))
                    {
                        AddDeclarationDiagnostics(instance);
                    }
                    instance.Free();
                }
                return _lazyTypeParameters;
            }
        }

        IAttributeTargetSymbol IAttributeTargetSymbol.AttributesOwner => this;

        AttributeLocation IAttributeTargetSymbol.DefaultAttributeLocation => AttributeLocation.Type;

        AttributeLocation IAttributeTargetSymbol.AllowedAttributeLocations
        {
            get
            {
                return TypeKind switch
                {
                    TypeKind.Delegate => AttributeLocation.Type | AttributeLocation.Return,
                    TypeKind.Enum or TypeKind.Interface => AttributeLocation.Type,
                    TypeKind.Class or TypeKind.Struct => AttributeLocation.Type,
                    _ => AttributeLocation.None,
                };
            }
        }

        internal override ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
                if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
                {
                    return ((CommonTypeEarlyWellKnownAttributeData)lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData)?.ObsoleteAttributeData;
                }
                ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.HasAnyAttributes)
                    {
                        return ObsoleteAttributeData.Uninitialized;
                    }
                }
                return null;
            }
        }

        internal override bool IsExplicitDefinitionOfNoPiaLocalType
        {
            get
            {
                if (_lazyIsExplicitDefinitionOfNoPiaLocalType == ThreeState.Unknown)
                {
                    CheckPresenceOfTypeIdentifierAttribute();
                    if (_lazyIsExplicitDefinitionOfNoPiaLocalType == ThreeState.Unknown)
                    {
                        _lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.False;
                    }
                }
                return _lazyIsExplicitDefinitionOfNoPiaLocalType == ThreeState.True;
            }
        }

        internal override bool IsComImport => GetEarlyDecodedWellKnownAttributeData()?.HasComImportAttribute ?? false;

        internal override NamedTypeSymbol ComImportCoClass => GetDecodedWellKnownAttributeData()?.ComImportCoClass;

        internal override bool HasSpecialName => GetDecodedWellKnownAttributeData()?.HasSpecialNameAttribute ?? false;

        internal override bool HasCodeAnalysisEmbeddedAttribute => GetEarlyDecodedWellKnownAttributeData()?.HasCodeAnalysisEmbeddedAttribute ?? false;

        internal sealed override bool ShouldAddWinRTMembers => false;

        internal sealed override bool IsWindowsRuntimeImport => GetDecodedWellKnownAttributeData()?.HasWindowsRuntimeImportAttribute ?? false;

        public sealed override bool IsSerializable => GetDecodedWellKnownAttributeData()?.HasSerializableAttribute ?? false;

        public sealed override bool AreLocalsZeroed
        {
            get
            {
                TypeWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
                if (decodedWellKnownAttributeData == null || !decodedWellKnownAttributeData.HasSkipLocalsInitAttribute)
                {
                    return ContainingType?.AreLocalsZeroed ?? ContainingModule.AreLocalsZeroed;
                }
                return false;
            }
        }

        internal override bool IsDirectlyExcludedFromCodeCoverage => GetDecodedWellKnownAttributeData()?.HasExcludeFromCodeCoverageAttribute ?? false;

        internal sealed override TypeLayout Layout
        {
            get
            {
                TypeWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
                if (decodedWellKnownAttributeData != null && decodedWellKnownAttributeData.HasStructLayoutAttribute)
                {
                    return decodedWellKnownAttributeData.Layout;
                }
                if (TypeKind == TypeKind.Struct)
                {
                    return new TypeLayout(LayoutKind.Sequential, (!HasInstanceFields()) ? 1 : 0, 0);
                }
                return default;
            }
        }

        internal bool HasStructLayoutAttribute => GetDecodedWellKnownAttributeData()?.HasStructLayoutAttribute ?? false;

        internal override CharSet MarshallingCharSet
        {
            get
            {
                TypeWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
                if (decodedWellKnownAttributeData == null || !decodedWellKnownAttributeData.HasStructLayoutAttribute)
                {
                    return base.DefaultMarshallingCharSet;
                }
                return decodedWellKnownAttributeData.MarshallingCharSet;
            }
        }

        internal sealed override bool HasDeclarativeSecurity => GetDecodedWellKnownAttributeData()?.HasDeclarativeSecurity ?? false;

        internal bool HasSecurityCriticalAttributes => GetDecodedWellKnownAttributeData()?.HasSecurityCriticalAttributes ?? false;

        internal override NamedTypeSymbol NativeIntegerUnderlyingType => null;

        internal sealed override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics
        {
            get
            {
                if ((object)_lazyBaseType == ErrorTypeSymbol.UnknownResultType)
                {
                    if ((object)ContainingType != null)
                    {
                        _ = ContainingType!.BaseTypeNoUseSiteDiagnostics;
                    }
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                    NamedTypeSymbol value = MakeAcyclicBaseType(instance);
                    if ((object)Interlocked.CompareExchange(ref _lazyBaseType, value, ErrorTypeSymbol.UnknownResultType) == ErrorTypeSymbol.UnknownResultType)
                    {
                        AddDeclarationDiagnostics(instance);
                    }
                    instance.Free();
                }
                return _lazyBaseType;
            }
        }

        public override NamedTypeSymbol EnumUnderlyingType
        {
            get
            {
                if ((object)_lazyEnumUnderlyingType == ErrorTypeSymbol.UnknownResultType)
                {
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                    if ((object)Interlocked.CompareExchange(ref _lazyEnumUnderlyingType, GetEnumUnderlyingType(instance), ErrorTypeSymbol.UnknownResultType) == ErrorTypeSymbol.UnknownResultType)
                    {
                        AddDeclarationDiagnostics(instance);
                        state.NotePartComplete(CompletionPart.EnumUnderlyingType);
                    }
                    instance.Free();
                }
                return _lazyEnumUnderlyingType;
            }
        }

        internal FieldSymbol EnumValueField
        {
            get
            {
                if (TypeKind != TypeKind.Enum)
                {
                    return null;
                }
                if (_lazyEnumValueField is null)
                {
                    Interlocked.CompareExchange(ref _lazyEnumValueField, new SynthesizedEnumValueFieldSymbol(this), null);
                }
                return _lazyEnumValueField;
            }
        }

        protected override Location GetCorrespondingBaseListLocation(NamedTypeSymbol @base)
        {
            Location location = null;
            ImmutableArray<SyntaxReference>.Enumerator enumerator = base.SyntaxReferences.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BaseListSyntax baseList = ((TypeDeclarationSyntax)enumerator.Current.GetSyntax()).BaseList;
                if (baseList == null)
                {
                    continue;
                }
                SeparatedSyntaxList<BaseTypeSyntax> types = baseList.Types;
                Binder binder = DeclaringCompilation.GetBinder(baseList);
                binder = binder.WithAdditionalFlagsAndContainingMemberOrLambda(BinderFlags.SuppressConstraintChecks, this);
                if ((object)location == null)
                {
                    location = types[0].Type.GetLocation();
                }
                SeparatedSyntaxList<BaseTypeSyntax>.Enumerator enumerator2 = types.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    TypeSyntax type = enumerator2.Current.Type;
                    if (TypeSymbol.Equals(binder.BindType(type, BindingDiagnosticBag.Discarded).Type, @base, TypeCompareKind.ConsiderEverything))
                    {
                        return type.GetLocation();
                    }
                }
            }
            return location;
        }

        internal SourceNamedTypeSymbol(NamespaceOrTypeSymbol containingSymbol, MergedTypeDeclaration declaration, BindingDiagnosticBag diagnostics, TupleExtraData tupleData = null)
            : base(containingSymbol, declaration, diagnostics, tupleData)
        {
            DeclarationKind kind = declaration.Kind;
            if (kind - 1 > DeclarationKind.Enum)
            {
                _ = kind - 10;
                _ = 1;
            }
            if (containingSymbol.Kind == SymbolKind.NamedType)
            {
                _lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.False;
            }
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            return new SourceNamedTypeSymbol(ContainingType, declaration, BindingDiagnosticBag.Discarded, newData);
        }

        private static SyntaxToken GetName(CSharpSyntaxNode node)
        {
            return node.Kind() switch
            {
                SyntaxKind.EnumDeclaration => ((EnumDeclarationSyntax)node).Identifier,
                SyntaxKind.DelegateDeclaration => ((DelegateDeclarationSyntax)node).Identifier,
                SyntaxKind.ClassDeclaration or SyntaxKind.StructDeclaration or SyntaxKind.InterfaceDeclaration or SyntaxKind.RecordDeclaration or SyntaxKind.RecordStructDeclaration => ((BaseTypeDeclarationSyntax)node).Identifier,
                _ => default(SyntaxToken),
            };
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, expandIncludes, ref expandIncludes ? ref _lazyExpandedDocComment : ref _lazyDocComment);
        }

        private ImmutableArray<TypeParameterSymbol> MakeTypeParameters(BindingDiagnosticBag diagnostics)
        {
            if (declaration.Arity == 0)
            {
                return ImmutableArray<TypeParameterSymbol>.Empty;
            }
            bool flag = false;
            string[] array = new string[declaration.Arity];
            string[] array2 = new string[declaration.Arity];
            List<List<TypeParameterBuilder>> list = new();
            ImmutableArray<SyntaxReference>.Enumerator enumerator = base.SyntaxReferences.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxReference current = enumerator.Current;
                CSharpSyntaxNode cSharpSyntaxNode = (CSharpSyntaxNode)current.GetSyntax();
                SyntaxTree syntaxTree = current.SyntaxTree;
                SyntaxKind syntaxKind = cSharpSyntaxNode.Kind();
                TypeParameterListSyntax typeParameterList = syntaxKind switch
                {
                    SyntaxKind.ClassDeclaration or SyntaxKind.StructDeclaration or SyntaxKind.InterfaceDeclaration or SyntaxKind.RecordDeclaration or SyntaxKind.RecordStructDeclaration => ((TypeDeclarationSyntax)cSharpSyntaxNode).TypeParameterList,
                    SyntaxKind.DelegateDeclaration => ((DelegateDeclarationSyntax)cSharpSyntaxNode).TypeParameterList,
                    _ => throw ExceptionUtilities.UnexpectedValue(cSharpSyntaxNode.Kind()),
                };
                bool flag2 = syntaxKind == SyntaxKind.InterfaceDeclaration || syntaxKind == SyntaxKind.DelegateDeclaration;
                List<TypeParameterBuilder> list2 = new();
                list.Add(list2);
                int num = 0;
                SeparatedSyntaxList<TypeParameterSyntax>.Enumerator enumerator2 = typeParameterList.Parameters.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    TypeParameterSyntax current2 = enumerator2.Current;
                    if (current2.VarianceKeyword.Kind() != 0 && !flag2)
                    {
                        diagnostics.Add(ErrorCode.ERR_IllegalVarianceSyntax, current2.VarianceKeyword.GetLocation());
                    }
                    string text = array[num];
                    SyntaxToken token = current2.Identifier;
                    SourceLocation location = new(in token);
                    string text2 = array2[num];
                    SourceMemberContainerTypeSymbol.ReportTypeNamedRecord(current2.Identifier.Text, DeclaringCompilation, diagnostics.DiagnosticBag, location);
                    if (text == null)
                    {
                        text = (array[num] = current2.Identifier.ValueText);
                        text2 = (array2[num] = current2.VarianceKeyword.ValueText);
                        int num2 = 0;
                        while (true)
                        {
                            if (num2 < num)
                            {
                                if (text == array[num2])
                                {
                                    flag = true;
                                    diagnostics.Add(ErrorCode.ERR_DuplicateTypeParameter, location, text);
                                    break;
                                }
                                num2++;
                                continue;
                            }
                            if ((object)ContainingType != null)
                            {
                                TypeParameterSymbol typeParameterSymbol = ContainingType.FindEnclosingTypeParameter(text);
                                if ((object)typeParameterSymbol != null)
                                {
                                    diagnostics.Add(ErrorCode.WRN_TypeParameterSameAsOuterTypeParameter, location, text, typeParameterSymbol.ContainingType);
                                }
                            }
                            break;
                        }
                    }
                    else if (!flag)
                    {
                        if (text2 != current2.VarianceKeyword.ValueText)
                        {
                            flag = true;
                            diagnostics.Add(ErrorCode.ERR_PartialWrongTypeParamsVariance, declaration.NameLocations.First(), this);
                        }
                        else if (text != current2.Identifier.ValueText)
                        {
                            flag = true;
                            diagnostics.Add(ErrorCode.ERR_PartialWrongTypeParams, declaration.NameLocations.First(), this);
                        }
                    }
                    list2.Add(new TypeParameterBuilder(syntaxTree.GetReference(current2), this, location));
                    num++;
                }
            }
            return list.Transpose().Select((IList<TypeParameterBuilder> builders, int i) => builders[0].MakeSymbol(i, builders, diagnostics)).AsImmutable();
        }

        internal ImmutableArray<TypeWithAnnotations> GetTypeParameterConstraintTypes(int ordinal)
        {
            ImmutableArray<ImmutableArray<TypeWithAnnotations>> typeParameterConstraintTypes = GetTypeParameterConstraintTypes();
            if (typeParameterConstraintTypes.Length <= 0)
            {
                return ImmutableArray<TypeWithAnnotations>.Empty;
            }
            return typeParameterConstraintTypes[ordinal];
        }

        private ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            ImmutableArray<ImmutableArray<TypeWithAnnotations>> lazyTypeParameterConstraintTypes = _lazyTypeParameterConstraintTypes;
            if (lazyTypeParameterConstraintTypes.IsDefault)
            {
                GetTypeParameterConstraintKinds();
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                if (ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameterConstraintTypes, MakeTypeParameterConstraintTypes(instance)))
                {
                    AddDeclarationDiagnostics(instance);
                }
                instance.Free();
                lazyTypeParameterConstraintTypes = _lazyTypeParameterConstraintTypes;
            }
            return lazyTypeParameterConstraintTypes;
        }

        internal TypeParameterConstraintKind GetTypeParameterConstraintKind(int ordinal)
        {
            ImmutableArray<TypeParameterConstraintKind> typeParameterConstraintKinds = GetTypeParameterConstraintKinds();
            if (typeParameterConstraintKinds.Length <= 0)
            {
                return TypeParameterConstraintKind.None;
            }
            return typeParameterConstraintKinds[ordinal];
        }

        private ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            ImmutableArray<TypeParameterConstraintKind> lazyTypeParameterConstraintKinds = _lazyTypeParameterConstraintKinds;
            if (lazyTypeParameterConstraintKinds.IsDefault)
            {
                ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameterConstraintKinds, MakeTypeParameterConstraintKinds());
                lazyTypeParameterConstraintKinds = _lazyTypeParameterConstraintKinds;
            }
            return lazyTypeParameterConstraintKinds;
        }

        private ImmutableArray<ImmutableArray<TypeWithAnnotations>> MakeTypeParameterConstraintTypes(BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<TypeParameterSymbol> typeParameters = TypeParameters;
            ImmutableArray<TypeParameterConstraintClause> immutableArray = ImmutableArray<TypeParameterConstraintClause>.Empty;
            if (typeParameters.Length > 0)
            {
                bool flag = SkipPartialDeclarationsWithoutConstraintClauses();
                ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>> arrayBuilder = null;
                ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxReference syntaxReference = enumerator.Current.SyntaxReference;
                    SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = GetConstraintClauses((CSharpSyntaxNode)syntaxReference.GetSyntax(), out TypeParameterListSyntax typeParameterList);
                    if (!flag || constraintClauses.Count != 0)
                    {
                        BinderFactory binderFactory = DeclaringCompilation.GetBinderFactory(syntaxReference.SyntaxTree);
                        ImmutableArray<TypeParameterConstraintClause> immutableArray2 = ((constraintClauses.Count != 0) ? binderFactory.GetBinder(constraintClauses[0]).WithContainingMemberOrLambda(this).WithAdditionalFlags(BinderFlags.SuppressConstraintChecks | BinderFlags.GenericConstraintsClause)
                            .BindTypeParameterConstraintClauses(this, typeParameters, typeParameterList, constraintClauses, diagnostics, performOnlyCycleSafeValidation: false) : binderFactory.GetBinder(typeParameterList.Parameters[0]).GetDefaultTypeParameterConstraintClauses(typeParameterList));
                        if (immutableArray.Length == 0)
                        {
                            immutableArray = immutableArray2;
                        }
                        else
                        {
                            (arrayBuilder ?? (arrayBuilder = ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>>.GetInstance())).Add(immutableArray2);
                        }
                    }
                }
                immutableArray = MergeConstraintTypesForPartialDeclarations(immutableArray, arrayBuilder, diagnostics);
                if (immutableArray.All((TypeParameterConstraintClause clause) => clause.ConstraintTypes.IsEmpty))
                {
                    immutableArray = ImmutableArray<TypeParameterConstraintClause>.Empty;
                }
                arrayBuilder?.Free();
            }
            return immutableArray.SelectAsArray((TypeParameterConstraintClause clause) => clause.ConstraintTypes);
        }

        private bool SkipPartialDeclarationsWithoutConstraintClauses()
        {
            ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (GetConstraintClauses((CSharpSyntaxNode)enumerator.Current.SyntaxReference.GetSyntax(), out var _).Count != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private ImmutableArray<TypeParameterConstraintKind> MakeTypeParameterConstraintKinds()
        {
            ImmutableArray<TypeParameterSymbol> typeParameters = TypeParameters;
            ImmutableArray<TypeParameterConstraintClause> immutableArray = ImmutableArray<TypeParameterConstraintClause>.Empty;
            if (typeParameters.Length > 0)
            {
                bool flag = SkipPartialDeclarationsWithoutConstraintClauses();
                ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>> arrayBuilder = null;
                ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxReference syntaxReference = enumerator.Current.SyntaxReference;
                    SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = GetConstraintClauses((CSharpSyntaxNode)syntaxReference.GetSyntax(), out TypeParameterListSyntax typeParameterList);
                    if (!flag || constraintClauses.Count != 0)
                    {
                        BinderFactory binderFactory = DeclaringCompilation.GetBinderFactory(syntaxReference.SyntaxTree);
                        ImmutableArray<TypeParameterConstraintClause> immutableArray2 = ((constraintClauses.Count != 0) ? binderFactory.GetBinder(constraintClauses[0]).WithContainingMemberOrLambda(this).WithAdditionalFlags(BinderFlags.SuppressConstraintChecks | BinderFlags.GenericConstraintsClause | BinderFlags.SuppressTypeArgumentBinding)
                            .BindTypeParameterConstraintClauses(this, typeParameters, typeParameterList, constraintClauses, BindingDiagnosticBag.Discarded, performOnlyCycleSafeValidation: true) : binderFactory.GetBinder(typeParameterList.Parameters[0]).GetDefaultTypeParameterConstraintClauses(typeParameterList));
                        if (immutableArray.Length == 0)
                        {
                            immutableArray = immutableArray2;
                        }
                        else
                        {
                            (arrayBuilder ?? (arrayBuilder = ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>>.GetInstance())).Add(immutableArray2);
                        }
                    }
                }
                immutableArray = MergeConstraintKindsForPartialDeclarations(immutableArray, arrayBuilder);
                immutableArray = ConstraintsHelper.AdjustConstraintKindsBasedOnConstraintTypes(this, typeParameters, immutableArray);
                if (immutableArray.All((TypeParameterConstraintClause clause) => clause.Constraints == TypeParameterConstraintKind.None))
                {
                    immutableArray = ImmutableArray<TypeParameterConstraintClause>.Empty;
                }
                arrayBuilder?.Free();
            }
            return immutableArray.SelectAsArray((TypeParameterConstraintClause clause) => clause.Constraints);
        }

        private static SyntaxList<TypeParameterConstraintClauseSyntax> GetConstraintClauses(CSharpSyntaxNode node, out TypeParameterListSyntax typeParameterList)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    {
                        TypeDeclarationSyntax typeDeclarationSyntax = (TypeDeclarationSyntax)node;
                        typeParameterList = typeDeclarationSyntax.TypeParameterList;
                        return typeDeclarationSyntax.ConstraintClauses;
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        DelegateDeclarationSyntax delegateDeclarationSyntax = (DelegateDeclarationSyntax)node;
                        typeParameterList = delegateDeclarationSyntax.TypeParameterList;
                        return delegateDeclarationSyntax.ConstraintClauses;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(node.Kind());
            }
        }

        private ImmutableArray<TypeParameterConstraintClause> MergeConstraintTypesForPartialDeclarations(ImmutableArray<TypeParameterConstraintClause> constraintClauses, ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>> otherPartialClauses, BindingDiagnosticBag diagnostics)
        {
            if (otherPartialClauses == null)
            {
                return constraintClauses;
            }
            ArrayBuilder<TypeParameterConstraintClause> arrayBuilder = null;
            ImmutableArray<TypeParameterSymbol> typeParameters = TypeParameters;
            int length = typeParameters.Length;
            for (int i = 0; i < length; i++)
            {
                TypeParameterConstraintClause typeParameterConstraintClause = constraintClauses[i];
                ImmutableArray<TypeWithAnnotations> constraintTypes2 = typeParameterConstraintClause.ConstraintTypes;
                ArrayBuilder<TypeWithAnnotations> mergedConstraintTypes2 = null;
                SmallDictionary<TypeWithAnnotations, int> originalConstraintTypesMap2 = null;
                bool flag = (GetTypeParameterConstraintKind(i) & TypeParameterConstraintKind.PartialMismatch) != 0;
                ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>>.Enumerator enumerator = otherPartialClauses.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!mergeConstraints(constraintTypes2, ref originalConstraintTypesMap2, ref mergedConstraintTypes2, enumerator.Current[i]))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    diagnostics.Add(ErrorCode.ERR_PartialWrongConstraints, Locations[0], this, typeParameters[i]);
                }
                if (mergedConstraintTypes2 != null)
                {
                    if (arrayBuilder == null)
                    {
                        arrayBuilder = ArrayBuilder<TypeParameterConstraintClause>.GetInstance(constraintClauses.Length);
                        arrayBuilder.AddRange(constraintClauses);
                    }
                    arrayBuilder[i] = TypeParameterConstraintClause.Create(typeParameterConstraintClause.Constraints, mergedConstraintTypes2?.ToImmutableAndFree() ?? constraintTypes2);
                }
            }
            if (arrayBuilder != null)
            {
                constraintClauses = arrayBuilder.ToImmutableAndFree();
            }
            return constraintClauses;
            static bool mergeConstraints(ImmutableArray<TypeWithAnnotations> originalConstraintTypes, ref SmallDictionary<TypeWithAnnotations, int> originalConstraintTypesMap, ref ArrayBuilder<TypeWithAnnotations> mergedConstraintTypes, TypeParameterConstraintClause clause)
            {
                bool result = true;
                if (originalConstraintTypes.Length == 0)
                {
                    if (clause.ConstraintTypes.Length == 0)
                    {
                        return result;
                    }
                    return false;
                }
                if (clause.ConstraintTypes.Length == 0)
                {
                    return false;
                }
                if (originalConstraintTypesMap == null)
                {
                    originalConstraintTypesMap = toDictionary(originalConstraintTypes, TypeWithAnnotations.EqualsComparer.IgnoreNullableModifiersForReferenceTypesComparer);
                }
                SmallDictionary<TypeWithAnnotations, int> smallDictionary2 = toDictionary(clause.ConstraintTypes, originalConstraintTypesMap.Comparer);
                SmallDictionary<TypeWithAnnotations, int>.ValueCollection.Enumerator enumerator2 = originalConstraintTypesMap.Values.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    int current = enumerator2.Current;
                    TypeWithAnnotations key = mergedConstraintTypes?[current] ?? originalConstraintTypes[current];
                    if (!smallDictionary2.TryGetValue(key, out var value))
                    {
                        result = false;
                    }
                    else
                    {
                        TypeWithAnnotations other = clause.ConstraintTypes[value];
                        if (!key.Equals(other, TypeCompareKind.ObliviousNullableModifierMatchesAny))
                        {
                            result = false;
                        }
                        else if (!key.Equals(other, TypeCompareKind.ConsiderEverything))
                        {
                            if (mergedConstraintTypes == null)
                            {
                                mergedConstraintTypes = ArrayBuilder<TypeWithAnnotations>.GetInstance(originalConstraintTypes.Length);
                                mergedConstraintTypes.AddRange(originalConstraintTypes);
                            }
                            mergedConstraintTypes[current] = key.MergeEquivalentTypes(other, VarianceKind.None);
                        }
                    }
                }
                SmallDictionary<TypeWithAnnotations, int>.KeyCollection.Enumerator enumerator3 = smallDictionary2.Keys.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    TypeWithAnnotations current2 = enumerator3.Current;
                    if (!originalConstraintTypesMap.ContainsKey(current2))
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
            static SmallDictionary<TypeWithAnnotations, int> toDictionary(ImmutableArray<TypeWithAnnotations> constraintTypes, IEqualityComparer<TypeWithAnnotations> comparer)
            {
                SmallDictionary<TypeWithAnnotations, int> smallDictionary = new(comparer);
                for (int num = constraintTypes.Length - 1; num >= 0; num--)
                {
                    smallDictionary[constraintTypes[num]] = num;
                }
                return smallDictionary;
            }
        }

        private ImmutableArray<TypeParameterConstraintClause> MergeConstraintKindsForPartialDeclarations(ImmutableArray<TypeParameterConstraintClause> constraintClauses, ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>> otherPartialClauses)
        {
            if (otherPartialClauses == null)
            {
                return constraintClauses;
            }
            ArrayBuilder<TypeParameterConstraintClause> arrayBuilder = null;
            int length = TypeParameters.Length;
            for (int i = 0; i < length; i++)
            {
                TypeParameterConstraintClause typeParameterConstraintClause = constraintClauses[i];
                TypeParameterConstraintKind mergedKind2 = typeParameterConstraintClause.Constraints;
                ImmutableArray<TypeWithAnnotations> constraintTypes = typeParameterConstraintClause.ConstraintTypes;
                ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>>.Enumerator enumerator = otherPartialClauses.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    mergeConstraints(ref mergedKind2, constraintTypes, enumerator.Current[i]);
                }
                if (typeParameterConstraintClause.Constraints != mergedKind2)
                {
                    if (arrayBuilder == null)
                    {
                        arrayBuilder = ArrayBuilder<TypeParameterConstraintClause>.GetInstance(constraintClauses.Length);
                        arrayBuilder.AddRange(constraintClauses);
                    }
                    arrayBuilder[i] = TypeParameterConstraintClause.Create(mergedKind2, constraintTypes);
                }
            }
            if (arrayBuilder != null)
            {
                constraintClauses = arrayBuilder.ToImmutableAndFree();
            }
            return constraintClauses;
            static void mergeConstraints(ref TypeParameterConstraintKind mergedKind, ImmutableArray<TypeWithAnnotations> originalConstraintTypes, TypeParameterConstraintClause clause)
            {
                if ((mergedKind & (TypeParameterConstraintKind.AllNonNullableKinds | TypeParameterConstraintKind.NotNull)) != (clause.Constraints & (TypeParameterConstraintKind.AllNonNullableKinds | TypeParameterConstraintKind.NotNull)))
                {
                    mergedKind |= TypeParameterConstraintKind.PartialMismatch;
                }
                if ((mergedKind & TypeParameterConstraintKind.ReferenceType) != 0 && (clause.Constraints & TypeParameterConstraintKind.ReferenceType) != 0)
                {
                    TypeParameterConstraintKind typeParameterConstraintKind = mergedKind & TypeParameterConstraintKind.AllReferenceTypeKinds;
                    TypeParameterConstraintKind typeParameterConstraintKind2 = clause.Constraints & TypeParameterConstraintKind.AllReferenceTypeKinds;
                    if (typeParameterConstraintKind != typeParameterConstraintKind2)
                    {
                        if (typeParameterConstraintKind == TypeParameterConstraintKind.ReferenceType)
                        {
                            mergedKind = (mergedKind & ~TypeParameterConstraintKind.AllReferenceTypeKinds) | typeParameterConstraintKind2;
                        }
                        else if (typeParameterConstraintKind2 != TypeParameterConstraintKind.ReferenceType)
                        {
                            mergedKind |= TypeParameterConstraintKind.PartialMismatch;
                        }
                    }
                }
                if (originalConstraintTypes.Length == 0 && clause.ConstraintTypes.Length == 0 && ((mergedKind | clause.Constraints) & ~(TypeParameterConstraintKind.Constructor | TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType)) == 0 && (mergedKind & TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType) != 0 && (clause.Constraints & TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType) == 0)
                {
                    mergedKind &= ~TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType;
                }
            }
        }

        internal ImmutableArray<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return declaration.GetAttributeDeclarations();
        }

        private CustomAttributesBag<CSharpAttributeData> GetAttributesBag()
        {
            CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
            if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsSealed)
            {
                return lazyCustomAttributesBag;
            }
            if (LoadAndValidateAttributes(OneOrMany.Create(GetAttributeDeclarations()), ref _lazyCustomAttributesBag))
            {
                state.NotePartComplete(CompletionPart.Attributes);
            }
            return _lazyCustomAttributesBag;
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return GetAttributesBag().Attributes;
        }

        private TypeWellKnownAttributeData GetDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (TypeWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
        }

        internal CommonTypeEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData()
        {
            CustomAttributesBag<CSharpAttributeData> customAttributesBag = _lazyCustomAttributesBag;
            if (customAttributesBag == null || !customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
            {
                customAttributesBag = GetAttributesBag();
            }
            return (CommonTypeEarlyWellKnownAttributeData)customAttributesBag.EarlyDecodedWellKnownAttributeData;
        }

        internal override CSharpAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
        {
            bool generatedDiagnostics;
            CSharpAttributeData attribute;
            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ComImportAttribute))
            {
                attribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
                if (!attribute.HasErrors)
                {
                    arguments.GetOrCreateData<CommonTypeEarlyWellKnownAttributeData>().HasComImportAttribute = true;
                    if (!generatedDiagnostics)
                    {
                        return attribute;
                    }
                }
                return null;
            }
            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CodeAnalysisEmbeddedAttribute))
            {
                attribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
                if (!attribute.HasErrors)
                {
                    arguments.GetOrCreateData<CommonTypeEarlyWellKnownAttributeData>().HasCodeAnalysisEmbeddedAttribute = true;
                    if (!generatedDiagnostics)
                    {
                        return attribute;
                    }
                }
                return null;
            }
            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ConditionalAttribute))
            {
                attribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
                if (!attribute.HasErrors)
                {
                    string constructorArgument = attribute.GetConstructorArgument<string>(0, SpecialType.System_String);
                    arguments.GetOrCreateData<CommonTypeEarlyWellKnownAttributeData>().AddConditionalSymbol(constructorArgument);
                    if (!generatedDiagnostics)
                    {
                        return attribute;
                    }
                }
                return null;
            }
            if (Symbol.EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out attribute, out var obsoleteData))
            {
                if (obsoleteData != null)
                {
                    arguments.GetOrCreateData<CommonTypeEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
                }
                return attribute;
            }
            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.AttributeUsageAttribute))
            {
                attribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
                if (!attribute.HasErrors)
                {
                    AttributeUsageInfo attributeUsageInfo = DecodeAttributeUsageAttribute(attribute, arguments.AttributeSyntax, diagnose: false);
                    if (!attributeUsageInfo.IsNull)
                    {
                        CommonTypeEarlyWellKnownAttributeData orCreateData = arguments.GetOrCreateData<CommonTypeEarlyWellKnownAttributeData>();
                        if (orCreateData.AttributeUsageInfo.IsNull)
                        {
                            orCreateData.AttributeUsageInfo = attributeUsageInfo;
                        }
                        if (!generatedDiagnostics)
                        {
                            return attribute;
                        }
                    }
                }
                return null;
            }
            return base.EarlyDecodeWellKnownAttribute(ref arguments);
        }

        internal override AttributeUsageInfo GetAttributeUsageInfo()
        {
            CommonTypeEarlyWellKnownAttributeData earlyDecodedWellKnownAttributeData = GetEarlyDecodedWellKnownAttributeData();
            if (earlyDecodedWellKnownAttributeData != null && !earlyDecodedWellKnownAttributeData.AttributeUsageInfo.IsNull)
            {
                return earlyDecodedWellKnownAttributeData.AttributeUsageInfo;
            }
            if ((object)BaseTypeNoUseSiteDiagnostics == null)
            {
                return AttributeUsageInfo.Default;
            }
            return BaseTypeNoUseSiteDiagnostics.GetAttributeUsageInfo();
        }

        internal sealed override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
            CSharpAttributeData attribute = arguments.Attribute;
            if (attribute.IsTargetAttribute(this, AttributeDescription.AttributeUsageAttribute))
            {
                DecodeAttributeUsageAttribute(attribute, arguments.AttributeSyntaxOpt, diagnose: true, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.DefaultMemberAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().HasDefaultMemberAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.CoClassAttribute))
            {
                DecodeCoClassAttribute(ref arguments);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ConditionalAttribute))
            {
                ValidateConditionalAttribute(attribute, arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.GuidAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().GuidString = attribute.DecodeGuidAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SpecialNameAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().HasSpecialNameAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SerializableAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().HasSerializableAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ExcludeFromCodeCoverageAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().HasExcludeFromCodeCoverageAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.StructLayoutAttribute))
            {
                AttributeData.DecodeStructLayoutAttribute<TypeWellKnownAttributeData, AttributeSyntax, CSharpAttributeData, AttributeLocation>(ref arguments, base.DefaultMarshallingCharSet, 0, MessageProvider.Instance);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SuppressUnmanagedCodeSecurityAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().HasSuppressUnmanagedCodeSecurityAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ClassInterfaceAttribute))
            {
                attribute.DecodeClassInterfaceAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.InterfaceTypeAttribute))
            {
                attribute.DecodeInterfaceTypeAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.WindowsRuntimeImportAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().HasWindowsRuntimeImportAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.RequiredAttributeAttribute))
            {
                bindingDiagnosticBag.Add(ErrorCode.ERR_CantUseRequiredAttribute, arguments.AttributeSyntaxOpt!.Name.Location);
            }
            else
            {
                if (ReportExplicitUseOfReservedAttributes(in arguments, ReservedAttributes.DynamicAttribute | ReservedAttributes.IsReadOnlyAttribute | ReservedAttributes.IsUnmanagedAttribute | ReservedAttributes.IsByRefLikeAttribute | ReservedAttributes.TupleElementNamesAttribute | ReservedAttributes.NullableAttribute | ReservedAttributes.NullableContextAttribute | ReservedAttributes.NativeIntegerAttribute | ReservedAttributes.CaseSensitiveExtensionAttribute))
                {
                    return;
                }
                if (attribute.IsTargetAttribute(this, AttributeDescription.SecurityCriticalAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.SecuritySafeCriticalAttribute))
                {
                    arguments.GetOrCreateData<TypeWellKnownAttributeData>().HasSecurityCriticalAttributes = true;
                    return;
                }
                if (attribute.IsTargetAttribute(this, AttributeDescription.SkipLocalsInitAttribute))
                {
                    CSharpAttributeData.DecodeSkipLocalsInitAttribute<TypeWellKnownAttributeData>(DeclaringCompilation, ref arguments);
                    return;
                }
                if (_lazyIsExplicitDefinitionOfNoPiaLocalType == ThreeState.Unknown && attribute.IsTargetAttribute(this, AttributeDescription.TypeIdentifierAttribute))
                {
                    _lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.True;
                    return;
                }
                CSharpCompilation declaringCompilation = DeclaringCompilation;
                if (attribute.IsSecurityAttribute(declaringCompilation))
                {
                    attribute.DecodeSecurityAttribute<TypeWellKnownAttributeData>(this, declaringCompilation, ref arguments);
                }
            }
        }

        private void CheckPresenceOfTypeIdentifierAttribute()
        {
            CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
            if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                return;
            }
            ImmutableArray<SyntaxList<AttributeListSyntax>>.Enumerator enumerator = GetAttributeDeclarations().GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxList<AttributeListSyntax> current = enumerator.Current;
                _ = current.Node!.SyntaxTree;
                QuickAttributeChecker quickAttributeChecker = DeclaringCompilation.GetBinderFactory(current.Node!.SyntaxTree).GetBinder(current.Node).QuickAttributeChecker;
                SyntaxList<AttributeListSyntax>.Enumerator enumerator2 = current.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    SeparatedSyntaxList<AttributeSyntax>.Enumerator enumerator3 = enumerator2.Current.Attributes.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        AttributeSyntax current2 = enumerator3.Current;
                        if (quickAttributeChecker.IsPossibleMatch(current2, QuickAttributes.TypeIdentifier))
                        {
                            GetAttributes();
                            return;
                        }
                    }
                }
            }
        }

        private AttributeUsageInfo DecodeAttributeUsageAttribute(CSharpAttributeData attribute, AttributeSyntax node, bool diagnose, BindingDiagnosticBag diagnosticsOpt = null)
        {
            if (!DeclaringCompilation.IsAttributeType(this))
            {
                if (diagnose)
                {
                    diagnosticsOpt.Add(ErrorCode.ERR_AttributeUsageOnNonAttributeClass, node.Name.Location, node.GetErrorDisplayName());
                }
                return AttributeUsageInfo.Null;
            }
            AttributeUsageInfo result = attribute.DecodeAttributeUsageAttribute();
            if (!result.HasValidAttributeTargets)
            {
                if (diagnose)
                {
                    CSharpSyntaxNode attributeArgumentSyntax = attribute.GetAttributeArgumentSyntax(0, node);
                    diagnosticsOpt.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntax.Location, node.GetErrorDisplayName());
                }
                return AttributeUsageInfo.Null;
            }
            return result;
        }

        private void DecodeCoClassAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            CSharpAttributeData attribute = arguments.Attribute;
            if (this.IsInterfaceType() && (!arguments.HasDecodedData || (object)((TypeWellKnownAttributeData)arguments.DecodedData).ComImportCoClass == null) && attribute.CommonConstructorArguments[0].ValueInternal is NamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeKind == TypeKind.Class)
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().ComImportCoClass = namedTypeSymbol;
            }
        }

        private void ValidateConditionalAttribute(CSharpAttributeData attribute, AttributeSyntax node, BindingDiagnosticBag diagnostics)
        {
            if (!DeclaringCompilation.IsAttributeType(this))
            {
                diagnostics.Add(ErrorCode.ERR_ConditionalOnNonAttributeClass, node.Location, node.GetErrorDisplayName());
                return;
            }
            string constructorArgument = attribute.GetConstructorArgument<string>(0, SpecialType.System_String);
            if (constructorArgument == null || !SyntaxFacts.IsValidIdentifier(constructorArgument))
            {
                CSharpSyntaxNode attributeArgumentSyntax = attribute.GetAttributeArgumentSyntax(0, node);
                diagnostics.Add(ErrorCode.ERR_BadArgumentToAttribute, attributeArgumentSyntax.Location, node.GetErrorDisplayName());
            }
        }

        private bool HasInstanceFields()
        {
            foreach (FieldSymbol item in GetFieldsToEmit())
            {
                if (!item.IsStatic)
                {
                    return true;
                }
            }
            return false;
        }

        internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            CustomAttributesBag<CSharpAttributeData> attributesBag = GetAttributesBag();
            TypeWellKnownAttributeData typeWellKnownAttributeData = (TypeWellKnownAttributeData)attributesBag.DecodedWellKnownAttributeData;
            if (typeWellKnownAttributeData != null)
            {
                SecurityWellKnownAttributeData securityInformation = typeWellKnownAttributeData.SecurityInformation;
                if (securityInformation != null)
                {
                    return securityInformation.GetSecurityAttributes(attributesBag.Attributes);
                }
            }
            return null;
        }

        internal override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return GetEarlyDecodedWellKnownAttributeData()?.ConditionalSymbols ?? ImmutableArray<string>.Empty;
        }

        internal override void PostDecodeWellKnownAttributes(ImmutableArray<CSharpAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
        {
            TypeWellKnownAttributeData typeWellKnownAttributeData = (TypeWellKnownAttributeData)decodedData;
            if (IsComImport)
            {
                if (typeWellKnownAttributeData == null || typeWellKnownAttributeData.GuidString == null)
                {
                    int index = boundAttributes.IndexOfAttribute(this, AttributeDescription.ComImportAttribute);
                    diagnostics.Add(ErrorCode.ERR_ComImportWithoutUuidAttribute, allAttributeSyntaxNodes[index].Name.Location, Name);
                }
                if (TypeKind == TypeKind.Class)
                {
                    NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
                    if ((object)baseTypeNoUseSiteDiagnostics != null && baseTypeNoUseSiteDiagnostics.SpecialType != SpecialType.System_Object)
                    {
                        diagnostics.Add(ErrorCode.ERR_ComImportWithBase, Locations[0], Name);
                    }
                    ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> staticInitializers = base.StaticInitializers;
                    if (!staticInitializers.IsDefaultOrEmpty)
                    {
                        ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Enumerator enumerator = staticInitializers.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ImmutableArray<FieldOrPropertyInitializer>.Enumerator enumerator2 = enumerator.Current.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                FieldOrPropertyInitializer current = enumerator2.Current;
                                if (!current.FieldOpt.IsMetadataConstant)
                                {
                                    diagnostics.Add(ErrorCode.ERR_ComImportWithInitializers, current.Syntax.GetLocation(), Name);
                                }
                            }
                        }
                    }
                    staticInitializers = base.InstanceInitializers;
                    if (!staticInitializers.IsDefaultOrEmpty)
                    {
                        ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Enumerator enumerator = staticInitializers.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ImmutableArray<FieldOrPropertyInitializer>.Enumerator enumerator2 = enumerator.Current.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                diagnostics.Add(ErrorCode.ERR_ComImportWithInitializers, enumerator2.Current.Syntax.GetLocation(), Name);
                            }
                        }
                    }
                }
            }
            else if ((object)ComImportCoClass != null)
            {
                int index2 = boundAttributes.IndexOfAttribute(this, AttributeDescription.CoClassAttribute);
                diagnostics.Add(ErrorCode.WRN_CoClassWithoutComImport, allAttributeSyntaxNodes[index2].Location, Name);
            }
            if (typeWellKnownAttributeData != null && typeWellKnownAttributeData.HasDefaultMemberAttribute && base.Indexers.Any())
            {
                int index3 = boundAttributes.IndexOfAttribute(this, AttributeDescription.DefaultMemberAttribute);
                diagnostics.Add(ErrorCode.ERR_DefaultMemberOnIndexedType, allAttributeSyntaxNodes[index3].Name.Location);
            }
            base.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData);
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            if (base.ContainsExtensionMethods)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_ExtensionAttribute__ctor));
            }
            if (IsRefLikeType)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeIsByRefLikeAttribute(this));
                if (ObsoleteAttributeData == null && !this.IsRestrictedType(ignoreSpanLikeTypes: true))
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_ObsoleteAttribute__ctor, ImmutableArray.Create(new TypedConstant(declaringCompilation.GetSpecialType(SpecialType.System_String), TypedConstantKind.Primitive, "Types with embedded references are not supported in this version of your compiler."), new TypedConstant(declaringCompilation.GetSpecialType(SpecialType.System_Boolean), TypedConstantKind.Primitive, true)), default(ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>>), isOptionalUse: true));
                }
            }
            if (IsReadOnly)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeIsReadOnlyAttribute(this));
            }
            if (base.Indexers.Any())
            {
                string metadataName = base.Indexers.First().MetadataName;
                TypedConstant item = new(declaringCompilation.GetSpecialType(SpecialType.System_String), TypedConstantKind.Primitive, metadataName);
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor, ImmutableArray.Create(item)));
            }
        }

        internal override NamedTypeSymbol AsNativeInteger()
        {
            return ContainingAssembly.GetNativeIntegerType(this);
        }

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            if (!(t2 is NativeIntegerTypeSymbol nativeIntegerTypeSymbol))
            {
                return base.Equals(t2, comparison);
            }
            return nativeIntegerTypeSymbol.Equals(this, comparison);
        }

        internal sealed override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved)
        {
            if (_lazyInterfaces.IsDefault)
            {
                if (basesBeingResolved != null && basesBeingResolved.ContainsReference(OriginalDefinition))
                {
                    return ImmutableArray<NamedTypeSymbol>.Empty;
                }
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                ImmutableArray<NamedTypeSymbol> value = MakeAcyclicInterfaces(basesBeingResolved, instance);
                if (ImmutableInterlocked.InterlockedCompareExchange(ref _lazyInterfaces, value, default(ImmutableArray<NamedTypeSymbol>)).IsDefault)
                {
                    AddDeclarationDiagnostics(instance);
                }
                instance.Free();
            }
            return _lazyInterfaces;
        }

        protected override void CheckBase(BindingDiagnosticBag diagnostics)
        {
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
            if ((object)baseTypeNoUseSiteDiagnostics == null)
            {
                return;
            }
            Location location = null;
            bool flag = baseTypeNoUseSiteDiagnostics.ContainsErrorType();
            if (!flag)
            {
                location = FindBaseRefSyntax(baseTypeNoUseSiteDiagnostics);
            }
            if (base.IsGenericType && !flag && DeclaringCompilation.IsAttributeType(baseTypeNoUseSiteDiagnostics))
            {
                diagnostics.Add(ErrorCode.ERR_GenericDerivingFromAttribute, location, baseTypeNoUseSiteDiagnostics);
            }
            SingleTypeDeclaration singleTypeDeclaration = FirstDeclarationWithExplicitBases();
            if (singleTypeDeclaration != null)
            {
                TypeConversions conversions = new(ContainingAssembly.CorLibrary);
                SourceLocation nameLocation = singleTypeDeclaration.NameLocation;
                baseTypeNoUseSiteDiagnostics.CheckAllConstraints(DeclaringCompilation, conversions, nameLocation, diagnostics);
            }
            if (!this.IsClassType() || baseTypeNoUseSiteDiagnostics.IsObjectType() || flag)
            {
                return;
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new(diagnostics, ContainingAssembly);
            if (declaration.Kind == DeclarationKind.Record)
            {
                if ((object)SynthesizedRecordClone.FindValidCloneMethod(baseTypeNoUseSiteDiagnostics, ref useSiteInfo) == null)
                {
                    diagnostics.Add(ErrorCode.ERR_BadRecordBase, location);
                }
            }
            else if ((object)SynthesizedRecordClone.FindValidCloneMethod(baseTypeNoUseSiteDiagnostics, ref useSiteInfo) != null)
            {
                diagnostics.Add(ErrorCode.ERR_BadInheritanceFromRecord, location);
            }
            diagnostics.Add(location, useSiteInfo);
        }

        protected override void CheckInterfaces(BindingDiagnosticBag diagnostics)
        {
            MultiDictionary<NamedTypeSymbol, NamedTypeSymbol> interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics = base.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics;
            if (interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.IsEmpty)
            {
                return;
            }
            SingleTypeDeclaration singleTypeDeclaration = FirstDeclarationWithExplicitBases();
            if (singleTypeDeclaration == null)
            {
                return;
            }
            TypeConversions conversions = new(ContainingAssembly.CorLibrary);
            SourceLocation nameLocation = singleTypeDeclaration.NameLocation;
            foreach (KeyValuePair<NamedTypeSymbol, MultiDictionary<NamedTypeSymbol, NamedTypeSymbol>.ValueSet> item in interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics)
            {
                MultiDictionary<NamedTypeSymbol, NamedTypeSymbol>.ValueSet value = item.Value;
                foreach (NamedTypeSymbol item2 in value)
                {
                    item2.CheckAllConstraints(DeclaringCompilation, conversions, nameLocation, diagnostics);
                }
                if (value.Count <= 1)
                {
                    continue;
                }
                NamedTypeSymbol key = item.Key;
                foreach (NamedTypeSymbol item3 in value)
                {
                    if ((object)key == item3)
                    {
                        continue;
                    }
                    if (key.Equals(item3, TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
                    {
                        if (!key.Equals(item3, TypeCompareKind.ObliviousNullableModifierMatchesAny))
                        {
                            diagnostics.Add(ErrorCode.WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList, nameLocation, item3, this);
                        }
                    }
                    else if (key.Equals(item3, TypeCompareKind.IgnoreTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
                    {
                        diagnostics.Add(ErrorCode.ERR_DuplicateInterfaceWithTupleNamesInBaseList, nameLocation, item3, key, this);
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.ERR_DuplicateInterfaceWithDifferencesInBaseList, nameLocation, item3, key, this);
                    }
                }
            }
        }

        private SourceLocation FindBaseRefSyntax(NamedTypeSymbol baseSym)
        {
            ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BaseListSyntax baseListOpt = GetBaseListOpt(enumerator.Current);
                if (baseListOpt == null)
                {
                    continue;
                }
                Binder binder = DeclaringCompilation.GetBinder(baseListOpt);
                binder = binder.WithAdditionalFlagsAndContainingMemberOrLambda(BinderFlags.SuppressConstraintChecks, this);
                SeparatedSyntaxList<BaseTypeSyntax>.Enumerator enumerator2 = baseListOpt.Types.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    TypeSyntax type = enumerator2.Current.Type;
                    TypeSymbol type2 = binder.BindType(type, BindingDiagnosticBag.Discarded).Type;
                    if (baseSym.Equals(type2))
                    {
                        return new SourceLocation(type);
                    }
                }
            }
            return null;
        }

        private SingleTypeDeclaration FirstDeclarationWithExplicitBases()
        {
            ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SingleTypeDeclaration current = enumerator.Current;
                if (GetBaseListOpt(current) != null)
                {
                    return current;
                }
            }
            return null;
        }

        internal Tuple<NamedTypeSymbol, ImmutableArray<NamedTypeSymbol>> GetDeclaredBases(ConsList<TypeSymbol> basesBeingResolved)
        {
            if (_lazyDeclaredBases == null)
            {
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                if (Interlocked.CompareExchange(ref _lazyDeclaredBases, MakeDeclaredBases(basesBeingResolved, instance), null) == null)
                {
                    AddDeclarationDiagnostics(instance);
                }
                instance.Free();
            }
            return _lazyDeclaredBases;
        }

        internal override NamedTypeSymbol GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved)
        {
            return GetDeclaredBases(basesBeingResolved).Item1;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved)
        {
            return GetDeclaredBases(basesBeingResolved).Item2;
        }

        private Tuple<NamedTypeSymbol, ImmutableArray<NamedTypeSymbol>> MakeDeclaredBases(ConsList<TypeSymbol> basesBeingResolved, BindingDiagnosticBag diagnostics)
        {
            if (TypeKind == TypeKind.Enum)
            {
                return new Tuple<NamedTypeSymbol, ImmutableArray<NamedTypeSymbol>>(null, ImmutableArray<NamedTypeSymbol>.Empty);
            }
            bool flag = false;
            ConsList<TypeSymbol> newBasesBeingResolved = basesBeingResolved.Prepend(OriginalDefinition);
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            NamedTypeSymbol namedTypeSymbol = null;
            SourceLocation location = null;
            PooledDictionary<NamedTypeSymbol, SourceLocation> pooledSymbolDictionaryInstance = SpecializedSymbolCollections.GetPooledSymbolDictionaryInstance<NamedTypeSymbol, SourceLocation>();
            ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SingleTypeDeclaration current = enumerator.Current;
                Tuple<NamedTypeSymbol, ImmutableArray<NamedTypeSymbol>> tuple = MakeOneDeclaredBases(newBasesBeingResolved, current, diagnostics);
                if (tuple == null)
                {
                    continue;
                }
                NamedTypeSymbol item = tuple.Item1;
                ImmutableArray<NamedTypeSymbol> immutableArray = tuple.Item2;
                if (!flag)
                {
                    if ((object)namedTypeSymbol == null)
                    {
                        namedTypeSymbol = item;
                        location = current.NameLocation;
                    }
                    else if (namedTypeSymbol.TypeKind == TypeKind.Error && (object)item != null)
                    {
                        immutableArray = immutableArray.Add(namedTypeSymbol);
                        namedTypeSymbol = item;
                        location = current.NameLocation;
                    }
                    else if ((object)item != null && !TypeSymbol.Equals(item, namedTypeSymbol, TypeCompareKind.ConsiderEverything) && item.TypeKind != TypeKind.Error)
                    {
                        CSDiagnosticInfo errorInfo = diagnostics.Add(ErrorCode.ERR_PartialMultipleBases, Locations[0], this);
                        namedTypeSymbol = new ExtendedErrorTypeSymbol(namedTypeSymbol, LookupResultKind.Ambiguous, errorInfo);
                        location = current.NameLocation;
                        flag = true;
                    }
                }
                ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = immutableArray.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    NamedTypeSymbol current2 = enumerator2.Current;
                    if (!pooledSymbolDictionaryInstance.ContainsKey(current2))
                    {
                        instance.Add(current2);
                        pooledSymbolDictionaryInstance.Add(current2, current.NameLocation);
                    }
                }
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new(diagnostics, ContainingAssembly);
            DeclarationKind kind = declaration.Kind;
            if (kind == DeclarationKind.Record || kind == DeclarationKind.RecordStruct)
            {
                NamedTypeSymbol namedTypeSymbol2 = DeclaringCompilation.GetWellKnownType(WellKnownType.System_IEquatable_T).Construct(this);
                if (instance.IndexOf(namedTypeSymbol2, SymbolEqualityComparer.AllIgnoreOptions) < 0)
                {
                    instance.Add(namedTypeSymbol2);
                    namedTypeSymbol2.AddUseSiteInfo(ref useSiteInfo);
                }
            }
            if ((object)namedTypeSymbol != null)
            {
                if (namedTypeSymbol.IsStatic)
                {
                    diagnostics.Add(ErrorCode.ERR_StaticBaseClass, location, namedTypeSymbol, this);
                }
                if (!this.IsNoMoreVisibleThan(namedTypeSymbol, ref useSiteInfo))
                {
                    diagnostics.Add(ErrorCode.ERR_BadVisBaseClass, location, this, namedTypeSymbol);
                }
            }
            ImmutableArray<NamedTypeSymbol> item2 = instance.ToImmutableAndFree();
            if (DeclaredAccessibility != Accessibility.Private && IsInterface)
            {
                ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = item2.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    NamedTypeSymbol current3 = enumerator2.Current;
                    if (!current3.IsAtLeastAsVisibleAs(this, ref useSiteInfo))
                    {
                        diagnostics.Add(ErrorCode.ERR_BadVisBaseInterface, pooledSymbolDictionaryInstance[current3], this, current3);
                    }
                }
            }
            pooledSymbolDictionaryInstance.Free();
            diagnostics.Add(Locations[0], useSiteInfo);
            return new Tuple<NamedTypeSymbol, ImmutableArray<NamedTypeSymbol>>(namedTypeSymbol, item2);
        }

        private static BaseListSyntax GetBaseListOpt(SingleTypeDeclaration decl)
        {
            if (decl.HasBaseDeclarations)
            {
                return ((BaseTypeDeclarationSyntax)decl.SyntaxReference.GetSyntax()).BaseList;
            }
            return null;
        }

        private Tuple<NamedTypeSymbol, ImmutableArray<NamedTypeSymbol>> MakeOneDeclaredBases(ConsList<TypeSymbol> newBasesBeingResolved, SingleTypeDeclaration decl, BindingDiagnosticBag diagnostics)
        {
            BaseListSyntax baseListOpt = GetBaseListOpt(decl);
            if (baseListOpt == null)
            {
                return null;
            }
            NamedTypeSymbol namedTypeSymbol = null;
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            Binder binder = DeclaringCompilation.GetBinder(baseListOpt);
            binder = binder.WithAdditionalFlagsAndContainingMemberOrLambda(BinderFlags.SuppressConstraintChecks, this);
            int num = -1;
            SeparatedSyntaxList<BaseTypeSyntax>.Enumerator enumerator = baseListOpt.Types.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BaseTypeSyntax current = enumerator.Current;
                num++;
                TypeSyntax type = current.Type;
                if (type.Kind() != SyntaxKind.PredefinedType && !SyntaxFacts.IsName(type.Kind()))
                {
                    diagnostics.Add(ErrorCode.ERR_BadBaseType, type.GetLocation());
                }
                SourceLocation location = new(type);
                TypeSymbol type2;
                if (num == 0 && TypeKind == TypeKind.Class)
                {
                    type2 = binder.BindType(type, diagnostics, newBasesBeingResolved).Type;
                    SpecialType specialType = type2.SpecialType;
                    if (IsRestrictedBaseType(specialType) && (SpecialType != SpecialType.System_Enum || specialType != SpecialType.System_ValueType) && (SpecialType != SpecialType.System_MulticastDelegate || specialType != SpecialType.System_Delegate) && (specialType != SpecialType.System_Array || !(ContainingAssembly.CorLibrary == ContainingAssembly)))
                    {
                        diagnostics.Add(ErrorCode.ERR_DeriveFromEnumOrValueType, location, this, type2);
                        continue;
                    }
                    if (type2.IsSealed && !IsStatic)
                    {
                        diagnostics.Add(ErrorCode.ERR_CantDeriveFromSealedType, location, this, type2);
                        continue;
                    }
                    bool flag = false;
                    if (type2.TypeKind == TypeKind.Error)
                    {
                        flag = true;
                        if (type2.GetNonErrorTypeKindGuess() == TypeKind.Interface)
                        {
                            flag = false;
                        }
                    }
                    if ((type2.TypeKind == TypeKind.Class || type2.TypeKind == TypeKind.Delegate || type2.TypeKind == TypeKind.Struct || flag) && (object)namedTypeSymbol == null)
                    {
                        namedTypeSymbol = (NamedTypeSymbol)type2;
                        if (IsStatic && namedTypeSymbol.SpecialType != SpecialType.System_Object)
                        {
                            CSDiagnosticInfo errorInfo = diagnostics.Add(ErrorCode.ERR_StaticDerivedFromNonObject, location, this, namedTypeSymbol);
                            namedTypeSymbol = new ExtendedErrorTypeSymbol(namedTypeSymbol, LookupResultKind.NotReferencable, errorInfo);
                        }
                        checkPrimaryConstructorBaseType(current, namedTypeSymbol);
                        continue;
                    }
                }
                else
                {
                    type2 = binder.BindType(type, diagnostics, newBasesBeingResolved).Type;
                }
                if (num == 0)
                {
                    checkPrimaryConstructorBaseType(current, type2);
                }
                switch (type2.TypeKind)
                {
                    case TypeKind.Interface:
                        {
                            ArrayBuilder<NamedTypeSymbol>.Enumerator enumerator2 = instance.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                NamedTypeSymbol current2 = enumerator2.Current;
                                if (current2.Equals(type2, TypeCompareKind.ConsiderEverything))
                                {
                                    diagnostics.Add(ErrorCode.ERR_DuplicateInterfaceInBaseList, location, type2);
                                }
                                else if (current2.Equals(type2, TypeCompareKind.ObliviousNullableModifierMatchesAny))
                                {
                                    diagnostics.Add(ErrorCode.WRN_DuplicateInterfaceWithNullabilityMismatchInBaseList, location, type2, this);
                                }
                            }
                            if (IsStatic)
                            {
                                diagnostics.Add(ErrorCode.ERR_StaticClassInterfaceImpl, location, this, type2);
                            }
                            if (IsRefLikeType)
                            {
                                diagnostics.Add(ErrorCode.ERR_RefStructInterfaceImpl, location, this, type2);
                            }
                            if (type2.ContainsDynamic())
                            {
                                diagnostics.Add(ErrorCode.ERR_DeriveFromConstructedDynamic, location, this, type2);
                            }
                            instance.Add((NamedTypeSymbol)type2);
                            continue;
                        }
                    case TypeKind.Class:
                        if (TypeKind == TypeKind.Class)
                        {
                            if ((object)namedTypeSymbol == null)
                            {
                                namedTypeSymbol = (NamedTypeSymbol)type2;
                                diagnostics.Add(ErrorCode.ERR_BaseClassMustBeFirst, location, type2);
                            }
                            else
                            {
                                diagnostics.Add(ErrorCode.ERR_NoMultipleInheritance, location, this, namedTypeSymbol, type2);
                            }
                            continue;
                        }
                        break;
                    case TypeKind.TypeParameter:
                        diagnostics.Add(ErrorCode.ERR_DerivingFromATyVar, location, type2);
                        continue;
                    case TypeKind.Error:
                        instance.Add((NamedTypeSymbol)type2);
                        continue;
                    case TypeKind.Dynamic:
                        diagnostics.Add(ErrorCode.ERR_DeriveFromDynamic, location, this);
                        continue;
                    case TypeKind.Submission:
                        throw ExceptionUtilities.UnexpectedValue(type2.TypeKind);
                }
                diagnostics.Add(ErrorCode.ERR_NonInterfaceInInterfaceList, location, type2);
            }
            if (SpecialType == SpecialType.System_Object && ((object)namedTypeSymbol != null || instance.Count != 0))
            {
                SyntaxToken token = GetName(baseListOpt.Parent);
                diagnostics.Add(ErrorCode.ERR_ObjectCantHaveBases, new SourceLocation(in token));
            }
            return new Tuple<NamedTypeSymbol, ImmutableArray<NamedTypeSymbol>>(namedTypeSymbol, instance.ToImmutableAndFree());
            void checkPrimaryConstructorBaseType(BaseTypeSyntax baseTypeSyntax, TypeSymbol baseType)
            {
                if (baseTypeSyntax is PrimaryConstructorBaseTypeSyntax primaryConstructorBaseTypeSyntax && (!IsRecord || TypeKind != TypeKind.Class || baseType.TypeKind == TypeKind.Interface || ((RecordDeclarationSyntax)decl.SyntaxReference.GetSyntax()).ParameterList == null))
                {
                    diagnostics.Add(ErrorCode.ERR_UnexpectedArgumentList, primaryConstructorBaseTypeSyntax.ArgumentList.Location);
                }
            }
        }

        private static bool IsRestrictedBaseType(SpecialType specialType)
        {
            if ((uint)(specialType - 2) <= 3u || specialType == SpecialType.System_Array)
            {
                return true;
            }
            return false;
        }

        private ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(ConsList<TypeSymbol> basesBeingResolved, BindingDiagnosticBag diagnostics)
        {
            TypeKind typeKind = TypeKind;
            if (typeKind == TypeKind.Enum)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }
            ImmutableArray<NamedTypeSymbol> declaredInterfaces = GetDeclaredInterfaces(basesBeingResolved);
            bool flag = typeKind == TypeKind.Interface;
            ArrayBuilder<NamedTypeSymbol> arrayBuilder = (flag ? ArrayBuilder<NamedTypeSymbol>.GetInstance() : null);
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = declaredInterfaces.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (flag)
                {
                    if (BaseTypeAnalysis.TypeDependsOn(current, this))
                    {
                        arrayBuilder.Add(new ExtendedErrorTypeSymbol(current, LookupResultKind.NotReferencable, diagnostics.Add(ErrorCode.ERR_CycleInInterfaceInheritance, Locations[0], this, current)));
                        continue;
                    }
                    arrayBuilder.Add(current);
                }
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new(diagnostics, ContainingAssembly);
                if (current.DeclaringCompilation != DeclaringCompilation)
                {
                    current.AddUseSiteInfo(ref useSiteInfo);
                    ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = current.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        NamedTypeSymbol current2 = enumerator2.Current;
                        if (current2.DeclaringCompilation != DeclaringCompilation)
                        {
                            current2.AddUseSiteInfo(ref useSiteInfo);
                        }
                    }
                }
                diagnostics.Add(Locations[0], useSiteInfo);
            }
            if (!flag)
            {
                return declaredInterfaces;
            }
            return arrayBuilder.ToImmutableAndFree();
        }

        private NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
        {
            TypeKind typeKind = TypeKind;
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            NamedTypeSymbol namedTypeSymbol = ((typeKind != TypeKind.Enum) ? GetDeclaredBaseType(null) : declaringCompilation.GetSpecialType(SpecialType.System_Enum));
            if (namedTypeSymbol is null)
            {
                switch (typeKind)
                {
                    case TypeKind.Class:
                        if (SpecialType == SpecialType.System_Object)
                        {
                            return null;
                        }
                        namedTypeSymbol = declaringCompilation.GetSpecialType(SpecialType.System_Object);
                        break;
                    case TypeKind.Struct:
                        namedTypeSymbol = declaringCompilation.GetSpecialType(SpecialType.System_ValueType);
                        break;
                    case TypeKind.Interface:
                        return null;
                    case TypeKind.Delegate:
                        namedTypeSymbol = declaringCompilation.GetSpecialType(SpecialType.System_MulticastDelegate);
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(typeKind);
                }
            }
            if (BaseTypeAnalysis.TypeDependsOn(namedTypeSymbol, this))
            {
                return new ExtendedErrorTypeSymbol(namedTypeSymbol, LookupResultKind.NotReferencable, diagnostics.Add(ErrorCode.ERR_CircularBase, Locations[0], namedTypeSymbol, this));
            }
            SetKnownToHaveNoDeclaredBaseCycles();
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new(diagnostics, ContainingAssembly);
            NamedTypeSymbol namedTypeSymbol2 = namedTypeSymbol;
            while (namedTypeSymbol2.DeclaringCompilation != DeclaringCompilation)
            {
                namedTypeSymbol2.AddUseSiteInfo(ref useSiteInfo);
                namedTypeSymbol2 = namedTypeSymbol2.BaseTypeNoUseSiteDiagnostics;
                if (namedTypeSymbol2 is null)
                {
                    break;
                }
            }
            diagnostics.Add(useSiteInfo.Diagnostics.IsNullOrEmpty() ? Location.None : (FindBaseRefSyntax(namedTypeSymbol) ?? Locations[0]), useSiteInfo);
            return namedTypeSymbol;
        }

        private NamedTypeSymbol GetEnumUnderlyingType(BindingDiagnosticBag diagnostics)
        {
            if (TypeKind != TypeKind.Enum)
            {
                return null;
            }
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            BaseListSyntax baseListOpt = GetBaseListOpt(declaration.Declarations[0]);
            if (baseListOpt != null)
            {
                SeparatedSyntaxList<BaseTypeSyntax> types = baseListOpt.Types;
                if (types.Count > 0)
                {
                    TypeSyntax type = types[0].Type;
                    TypeSymbol typeSymbol = declaringCompilation.GetBinder(baseListOpt).BindType(type, diagnostics).Type;
                    if (!typeSymbol.SpecialType.IsValidEnumUnderlyingType())
                    {
                        diagnostics.Add(ErrorCode.ERR_IntegralTypeExpected, type.Location);
                        typeSymbol = declaringCompilation.GetSpecialType(SpecialType.System_Int32);
                    }
                    return (NamedTypeSymbol)typeSymbol;
                }
            }
            NamedTypeSymbol specialType = declaringCompilation.GetSpecialType(SpecialType.System_Int32);
            Binder.ReportUseSite(specialType, diagnostics, Locations[0]);
            return specialType;
        }
    }
}

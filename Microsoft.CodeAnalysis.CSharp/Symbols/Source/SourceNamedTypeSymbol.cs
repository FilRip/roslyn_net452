// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    // This is a type symbol associated with a type definition in source code.
    // That is, for a generic type C<T> this is the instance type C<T>.  
    internal sealed partial class SourceNamedTypeSymbol : SourceMemberContainerTypeSymbol, IAttributeTargetSymbol
    {
        private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

        /// <summary>
        /// A collection of type parameter constraint types, populated when
        /// constraint types for the first type parameter are requested.
        /// </summary>
        private ImmutableArray<ImmutableArray<TypeWithAnnotations>> _lazyTypeParameterConstraintTypes;

        /// <summary>
        /// A collection of type parameter constraint kinds, populated when
        /// constraint kinds for the first type parameter are requested.
        /// </summary>
        private ImmutableArray<TypeParameterConstraintKind> _lazyTypeParameterConstraintKinds;

        private CustomAttributesBag<CSharpAttributeData> _lazyCustomAttributesBag;

        private string _lazyDocComment;
        private string _lazyExpandedDocComment;

        private ThreeState _lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.Unknown;

        protected override Location GetCorrespondingBaseListLocation(NamedTypeSymbol @base)
        {
            Location backupLocation = null;

            foreach (SyntaxReference part in SyntaxReferences)
            {
                TypeDeclarationSyntax typeBlock = (TypeDeclarationSyntax)part.GetSyntax();
                BaseListSyntax bases = typeBlock.BaseList;
                if (bases == null)
                {
                    continue;
                }
                SeparatedSyntaxList<BaseTypeSyntax> inheritedTypeDecls = bases.Types;

                var baseBinder = this.DeclaringCompilation.GetBinder(bases);
                baseBinder = baseBinder.WithAdditionalFlagsAndContainingMemberOrLambda(EBinder.SuppressConstraintChecks, this);

                backupLocation ??= inheritedTypeDecls[0].Type.GetLocation();

                foreach (TypeSyntax t in inheritedTypeDecls.Select(baseTypeSyntax => baseTypeSyntax.Type))
                {
                    TypeSymbol bt = baseBinder.BindType(t, BindingDiagnosticBag.Discarded).Type;

                    if (Equals(bt, @base, TypeCompareKind.ConsiderEverything2))
                    {
                        return t.GetLocation();
                    }
                }
            }

            return backupLocation;
        }

        internal SourceNamedTypeSymbol(NamespaceOrTypeSymbol containingSymbol, MergedTypeDeclaration declaration, BindingDiagnosticBag diagnostics, TupleExtraData tupleData = null)
            : base(containingSymbol, declaration, diagnostics, tupleData)
        {
            if (containingSymbol.Kind == SymbolKind.NamedType)
            {
                // Nested types are never unified.
                _lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.False;
            }
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            return new SourceNamedTypeSymbol(ContainingType, declaration, BindingDiagnosticBag.Discarded, newData);
        }

        #region Syntax

        private static SyntaxToken GetName(CSharpSyntaxNode node)
        {
            return node.Kind() switch
            {
                SyntaxKind.EnumDeclaration => ((EnumDeclarationSyntax)node).Identifier,
                SyntaxKind.DelegateDeclaration => ((DelegateDeclarationSyntax)node).Identifier,
                SyntaxKind.ClassDeclaration or SyntaxKind.InterfaceDeclaration or SyntaxKind.StructDeclaration or SyntaxKind.RecordDeclaration or SyntaxKind.RecordStructDeclaration => ((BaseTypeDeclarationSyntax)node).Identifier,
                _ => default,
            };
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default)
        {
            ref var lazyDocComment = ref expandIncludes ? ref _lazyExpandedDocComment : ref _lazyDocComment;
            return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, expandIncludes, ref lazyDocComment);
        }

        #endregion

        #region Type Parameters

        private ImmutableArray<TypeParameterSymbol> MakeTypeParameters(BindingDiagnosticBag diagnostics)
        {
            if (declaration.Arity == 0)
            {
                return ImmutableArray<TypeParameterSymbol>.Empty;
            }

            var typeParameterMismatchReported = false;
            var typeParameterNames = new string[declaration.Arity];
            var typeParameterVarianceKeywords = new string[declaration.Arity];
            var parameterBuilders1 = new List<List<TypeParameterBuilder>>();

            foreach (var syntaxRef in this.SyntaxReferences)
            {
                var typeDecl = (CSharpSyntaxNode)syntaxRef.GetSyntax();
                var syntaxTree = syntaxRef.SyntaxTree;
                SyntaxKind typeKind = typeDecl.Kind();
                TypeParameterListSyntax tpl = typeKind switch
                {
                    SyntaxKind.ClassDeclaration or SyntaxKind.StructDeclaration or SyntaxKind.InterfaceDeclaration or SyntaxKind.RecordDeclaration or SyntaxKind.RecordStructDeclaration => ((TypeDeclarationSyntax)typeDecl).TypeParameterList,
                    SyntaxKind.DelegateDeclaration => ((DelegateDeclarationSyntax)typeDecl).TypeParameterList,
                    _ => throw ExceptionUtilities.UnexpectedValue(typeDecl.Kind()),// there is no such thing as a generic enum, so code should never reach here.
                };
                bool isInterfaceOrDelegate = typeKind == SyntaxKind.InterfaceDeclaration || typeKind == SyntaxKind.DelegateDeclaration;
                var parameterBuilder = new List<TypeParameterBuilder>();
                parameterBuilders1.Add(parameterBuilder);
                int i = 0;
                bool next;
                foreach (var tp in tpl.Parameters)
                {
                    next = false;
                    if (tp.VarianceKeyword.Kind() != SyntaxKind.None &&
                        !isInterfaceOrDelegate)
                    {
                        diagnostics.Add(ErrorCode.ERR_IllegalVarianceSyntax, tp.VarianceKeyword.GetLocation());
                    }

                    var name = typeParameterNames[i];
                    var location = new SourceLocation(tp.Identifier);
                    var varianceKind = typeParameterVarianceKeywords[i];

                    ReportTypeNamedRecord(tp.Identifier.Text, this.DeclaringCompilation, diagnostics.DiagnosticBag, location);

                    if (name == null)
                    {
                        name = typeParameterNames[i] = tp.Identifier.ValueText;
                        varianceKind = typeParameterVarianceKeywords[i] = tp.VarianceKeyword.ValueText;
                        for (int j = 0; j < i; j++)
                        {
                            if (name == typeParameterNames[j])
                            {
                                typeParameterMismatchReported = true;
                                diagnostics.Add(ErrorCode.ERR_DuplicateTypeParameter, location, name);
                                next = true;
                            }
                        }

                        if (!next && ContainingType is not null)
                        {
                            var tpEnclosing = ContainingType.FindEnclosingTypeParameter(name);
                            if (tpEnclosing is not null)
                            {
                                // Type parameter '{0}' has the same name as the type parameter from outer type '{1}'
                                diagnostics.Add(ErrorCode.WRN_TypeParameterSameAsOuterTypeParameter, location, name, tpEnclosing.ContainingType);
                            }
                        }
                    }
                    else if (!typeParameterMismatchReported)
                    {
                        // Note: the "this", below, refers to the name of the current class, which includes its type
                        // parameter names.  But the type parameter names have not been computed yet.  Therefore, we
                        // take advantage of the fact that "this" won't undergo "ToString()" until later, when the
                        // diagnostic is printed, by which time the type parameters field will have been filled in.
                        if (varianceKind != tp.VarianceKeyword.ValueText)
                        {
                            // Dev10 reports CS1067, even if names also don't match
                            typeParameterMismatchReported = true;
                            diagnostics.Add(
                                ErrorCode.ERR_PartialWrongTypeParamsVariance,
                                declaration.NameLocations.First(),
                                this); // see comment above
                        }
                        else if (name != tp.Identifier.ValueText)
                        {
                            typeParameterMismatchReported = true;
                            diagnostics.Add(
                                ErrorCode.ERR_PartialWrongTypeParams,
                                declaration.NameLocations.First(),
                                this); // see comment above
                        }
                    }
                    parameterBuilder.Add(new TypeParameterBuilder(syntaxTree.GetReference(tp), this, location));
                    i++;
                }
            }

            var parameterBuilders2 = parameterBuilders1.Transpose(); // type arguments are positional
            var parameters = parameterBuilders2.Select((builders, i) => builders[0].MakeSymbol(i, builders, diagnostics));
            return parameters.AsImmutable();
        }

        /// <summary>
        /// Returns the constraint types for the given type parameter.
        /// </summary>
        internal ImmutableArray<TypeWithAnnotations> GetTypeParameterConstraintTypes(int ordinal)
        {
            var constraintTypes = GetTypeParameterConstraintTypes();
            return (constraintTypes.Length > 0) ? constraintTypes[ordinal] : ImmutableArray<TypeWithAnnotations>.Empty;
        }

        private ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            var constraintTypes = _lazyTypeParameterConstraintTypes;
            if (constraintTypes.IsDefault)
            {
                GetTypeParameterConstraintKinds();

                var diagnostics = BindingDiagnosticBag.GetInstance();
                if (ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameterConstraintTypes, MakeTypeParameterConstraintTypes(diagnostics)))
                {
                    this.AddDeclarationDiagnostics(diagnostics);
                }
                diagnostics.Free();
                constraintTypes = _lazyTypeParameterConstraintTypes;
            }

            return constraintTypes;
        }

        /// <summary>
        /// Returns the constraint kind for the given type parameter.
        /// </summary>
        internal TypeParameterConstraintKind GetTypeParameterConstraintKind(int ordinal)
        {
            var constraintKinds = GetTypeParameterConstraintKinds();
            return (constraintKinds.Length > 0) ? constraintKinds[ordinal] : TypeParameterConstraintKind.None;
        }

        private ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            var constraintKinds = _lazyTypeParameterConstraintKinds;
            if (constraintKinds.IsDefault)
            {
                ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameterConstraintKinds, MakeTypeParameterConstraintKinds());
                constraintKinds = _lazyTypeParameterConstraintKinds;
            }

            return constraintKinds;
        }

        private ImmutableArray<ImmutableArray<TypeWithAnnotations>> MakeTypeParameterConstraintTypes(BindingDiagnosticBag diagnostics)
        {
            var typeParameters = this.TypeParameters;
            var results = ImmutableArray<TypeParameterConstraintClause>.Empty;

            int arity = typeParameters.Length;
            if (arity > 0)
            {
                bool skipPartialDeclarationsWithoutConstraintClauses = SkipPartialDeclarationsWithoutConstraintClauses();
                ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>> otherPartialClauses = null;

                foreach (SyntaxReference syntaxRef in declaration.Declarations.Select(decl => decl.SyntaxReference))
                {
                    var constraintClauses = GetConstraintClauses((CSharpSyntaxNode)syntaxRef.GetSyntax(), out TypeParameterListSyntax typeParameterList);

                    if (skipPartialDeclarationsWithoutConstraintClauses && constraintClauses.Count == 0)
                    {
                        continue;
                    }

                    var binderFactory = this.DeclaringCompilation.GetBinderFactory(syntaxRef.SyntaxTree);
                    Binder binder;
                    ImmutableArray<TypeParameterConstraintClause> constraints;

                    if (constraintClauses.Count == 0)
                    {
                        binder = binderFactory.GetBinder(typeParameterList.Parameters[0]);

                        constraints = binder.GetDefaultTypeParameterConstraintClauses(typeParameterList);
                    }
                    else
                    {
                        binder = binderFactory.GetBinder(constraintClauses[0]);

                        // Wrap binder from factory in a generic constraints specific binder 
                        // to avoid checking constraints when binding type names.
                        binder = binder.WithContainingMemberOrLambda(this).WithAdditionalFlags(EBinder.GenericConstraintsClause | EBinder.SuppressConstraintChecks);

                        constraints = binder.BindTypeParameterConstraintClauses(this, typeParameters, typeParameterList, constraintClauses, diagnostics, performOnlyCycleSafeValidation: false);
                    }


                    if (results.Length == 0)
                    {
                        results = constraints;
                    }
                    else
                    {
                        (otherPartialClauses ??= ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>>.GetInstance()).Add(constraints);
                    }
                }

                results = MergeConstraintTypesForPartialDeclarations(results, otherPartialClauses, diagnostics);

                if (results.All(clause => clause.ConstraintTypes.IsEmpty))
                {
                    results = ImmutableArray<TypeParameterConstraintClause>.Empty;
                }

                otherPartialClauses?.Free();
            }

            return results.SelectAsArray(clause => clause.ConstraintTypes);
        }

        private bool SkipPartialDeclarationsWithoutConstraintClauses()
        {
            foreach (var decl in declaration.Declarations)
            {
                if (GetConstraintClauses((CSharpSyntaxNode)decl.SyntaxReference.GetSyntax(), out _).Count != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private ImmutableArray<TypeParameterConstraintKind> MakeTypeParameterConstraintKinds()
        {
            var typeParameters = this.TypeParameters;
            var results = ImmutableArray<TypeParameterConstraintClause>.Empty;

            int arity = typeParameters.Length;
            if (arity > 0)
            {
                bool skipPartialDeclarationsWithoutConstraintClauses = SkipPartialDeclarationsWithoutConstraintClauses();
                ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>> otherPartialClauses = null;

                foreach (var syntaxRef in declaration.Declarations.Select(decl => decl.SyntaxReference))
                {
                    var constraintClauses = GetConstraintClauses((CSharpSyntaxNode)syntaxRef.GetSyntax(), out TypeParameterListSyntax typeParameterList);

                    if (skipPartialDeclarationsWithoutConstraintClauses && constraintClauses.Count == 0)
                    {
                        continue;
                    }

                    var binderFactory = this.DeclaringCompilation.GetBinderFactory(syntaxRef.SyntaxTree);
                    Binder binder;
                    ImmutableArray<TypeParameterConstraintClause> constraints;

                    if (constraintClauses.Count == 0)
                    {
                        binder = binderFactory.GetBinder(typeParameterList.Parameters[0]);
                        constraints = binder.GetDefaultTypeParameterConstraintClauses(typeParameterList);
                    }
                    else
                    {
                        binder = binderFactory.GetBinder(constraintClauses[0]);

                        // Wrap binder from factory in a generic constraints specific binder 
                        // to avoid checking constraints when binding type names.
                        // Also, suppress type argument binding in constraint types, this helps to avoid cycles while we figure out constraint kinds. 
                        binder = binder.WithContainingMemberOrLambda(this).WithAdditionalFlags(EBinder.GenericConstraintsClause | EBinder.SuppressConstraintChecks | EBinder.SuppressTypeArgumentBinding);

                        // We will recompute this diagnostics more accurately later, when binding without BinderFlags.SuppressTypeArgumentBinding  
                        constraints = binder.BindTypeParameterConstraintClauses(this, typeParameters, typeParameterList, constraintClauses, BindingDiagnosticBag.Discarded, performOnlyCycleSafeValidation: true);
                    }


                    if (results.Length == 0)
                    {
                        results = constraints;
                    }
                    else
                    {
                        (otherPartialClauses ??= ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>>.GetInstance()).Add(constraints);
                    }
                }

                results = MergeConstraintKindsForPartialDeclarations(results, otherPartialClauses);
                results = ConstraintsHelper.AdjustConstraintKindsBasedOnConstraintTypes(/*this, */typeParameters, results);

                if (results.All(clause => clause.Constraints == TypeParameterConstraintKind.None))
                {
                    results = ImmutableArray<TypeParameterConstraintClause>.Empty;
                }

                otherPartialClauses?.Free();
            }

            return results.SelectAsArray(clause => clause.Constraints);
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
                    var typeDeclaration = (TypeDeclarationSyntax)node;
                    typeParameterList = typeDeclaration.TypeParameterList;
                    return typeDeclaration.ConstraintClauses;
                case SyntaxKind.DelegateDeclaration:
                    var delegateDeclaration = (DelegateDeclarationSyntax)node;
                    typeParameterList = delegateDeclaration.TypeParameterList;
                    return delegateDeclaration.ConstraintClauses;
                default:
                    throw ExceptionUtilities.UnexpectedValue(node.Kind());
            }
        }

        /// <summary>
        /// Note, only nullability aspects are merged if possible, other mismatches are treated as failures.
        /// </summary>
        private ImmutableArray<TypeParameterConstraintClause> MergeConstraintTypesForPartialDeclarations(ImmutableArray<TypeParameterConstraintClause> constraintClauses,
                                                                                                         ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>> otherPartialClauses,
                                                                                                         BindingDiagnosticBag diagnostics)
        {
            if (otherPartialClauses == null)
            {
                return constraintClauses;
            }

            ArrayBuilder<TypeParameterConstraintClause> builder = null;
            var typeParameters = TypeParameters;
            int arity = typeParameters.Length;


            for (int i = 0; i < arity; i++)
            {
                var constraint = constraintClauses[i];

                ImmutableArray<TypeWithAnnotations> originalConstraintTypes = constraint.ConstraintTypes;
                ArrayBuilder<TypeWithAnnotations> mergedConstraintTypes = null;
                SmallDictionary<TypeWithAnnotations, int> originalConstraintTypesMap = null;

                // Constraints defined on multiple partial declarations.
                // Report any mismatched constraints.
                bool report = (GetTypeParameterConstraintKind(i) & TypeParameterConstraintKind.PartialMismatch) != 0;
                foreach (ImmutableArray<TypeParameterConstraintClause> otherPartialConstraints in otherPartialClauses)
                {
                    if (!mergeConstraints(originalConstraintTypes, ref originalConstraintTypesMap, ref mergedConstraintTypes, otherPartialConstraints[i]))
                    {
                        report = true;
                    }
                }

                if (report)
                {
                    // "Partial declarations of '{0}' have inconsistent constraints for type parameter '{1}'"
                    diagnostics.Add(ErrorCode.ERR_PartialWrongConstraints, Locations[0], this, typeParameters[i]);
                }

                if (mergedConstraintTypes != null)
                {
#if DEBUG

                    for (int j = 0; j < originalConstraintTypes.Length; j++)
                    {
                        // Nothing to do
                    }
#endif
                    if (builder == null)
                    {
                        builder = ArrayBuilder<TypeParameterConstraintClause>.GetInstance(constraintClauses.Length);
                        builder.AddRange(constraintClauses);
                    }

                    builder[i] = TypeParameterConstraintClause.Create(constraint.Constraints,
                                                                      mergedConstraintTypes?.ToImmutableAndFree() ?? originalConstraintTypes);
                }
            }

            if (builder != null)
            {
                constraintClauses = builder.ToImmutableAndFree();
            }

            return constraintClauses;

            static bool mergeConstraints(ImmutableArray<TypeWithAnnotations> originalConstraintTypes,
                                         ref SmallDictionary<TypeWithAnnotations, int> originalConstraintTypesMap, ref ArrayBuilder<TypeWithAnnotations> mergedConstraintTypes,
                                         TypeParameterConstraintClause clause)
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
                else if (clause.ConstraintTypes.Length == 0)
                {
                    return false;
                }

                originalConstraintTypesMap ??= toDictionary(originalConstraintTypes,
                                                            TypeWithAnnotations.EqualsComparer.IgnoreNullableModifiersForReferenceTypesComparer);
                SmallDictionary<TypeWithAnnotations, int> clauseConstraintTypesMap = toDictionary(clause.ConstraintTypes, originalConstraintTypesMap.Comparer);

                foreach (int index1 in originalConstraintTypesMap.Values)
                {
                    TypeWithAnnotations constraintType1 = mergedConstraintTypes?[index1] ?? originalConstraintTypes[index1];

                    if (!clauseConstraintTypesMap.TryGetValue(constraintType1, out int index2))
                    {
                        // No matching type
                        result = false;
                        continue;
                    }

                    TypeWithAnnotations constraintType2 = clause.ConstraintTypes[index2];

                    if (!constraintType1.Equals(constraintType2, TypeCompareKind.ObliviousNullableModifierMatchesAny))
                    {
                        // Nullability mismatch that doesn't involve oblivious
                        result = false;
                        continue;
                    }

                    if (!constraintType1.Equals(constraintType2, TypeCompareKind.ConsiderEverything))
                    {
                        // Mismatch with oblivious, merge
                        if (mergedConstraintTypes == null)
                        {
                            mergedConstraintTypes = ArrayBuilder<TypeWithAnnotations>.GetInstance(originalConstraintTypes.Length);
                            mergedConstraintTypes.AddRange(originalConstraintTypes);
                        }

                        mergedConstraintTypes[index1] = constraintType1.MergeEquivalentTypes(constraintType2, VarianceKind.None);
                    }
                }

                foreach (var constraintType in clauseConstraintTypesMap.Keys)
                {
                    if (!originalConstraintTypesMap.ContainsKey(constraintType))
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            }

            static SmallDictionary<TypeWithAnnotations, int> toDictionary(ImmutableArray<TypeWithAnnotations> constraintTypes, IEqualityComparer<TypeWithAnnotations> comparer)
            {
                var result = new SmallDictionary<TypeWithAnnotations, int>(comparer);

                for (int i = constraintTypes.Length - 1; i >= 0; i--)
                {
                    result[constraintTypes[i]] = i; // Use the first type among the duplicates as the source of the nullable information
                }

                return result;
            }
        }

        /// <summary>
        /// Note, only nullability aspects are merged if possible, other mismatches are treated as failures.
        /// </summary>
        private ImmutableArray<TypeParameterConstraintClause> MergeConstraintKindsForPartialDeclarations(ImmutableArray<TypeParameterConstraintClause> constraintClauses,
                                                                                                         ArrayBuilder<ImmutableArray<TypeParameterConstraintClause>> otherPartialClauses)
        {
            if (otherPartialClauses == null)
            {
                return constraintClauses;
            }

            ArrayBuilder<TypeParameterConstraintClause> builder = null;
            var typeParameters = TypeParameters;
            int arity = typeParameters.Length;


            for (int i = 0; i < arity; i++)
            {
                var constraint = constraintClauses[i];

                TypeParameterConstraintKind mergedKind = constraint.Constraints;
                ImmutableArray<TypeWithAnnotations> originalConstraintTypes = constraint.ConstraintTypes;

                foreach (ImmutableArray<TypeParameterConstraintClause> otherPartialConstraints in otherPartialClauses)
                {
                    mergeConstraints(ref mergedKind, originalConstraintTypes, otherPartialConstraints[i]);
                }

                if (constraint.Constraints != mergedKind)
                {
                    if (builder == null)
                    {
                        builder = ArrayBuilder<TypeParameterConstraintClause>.GetInstance(constraintClauses.Length);
                        builder.AddRange(constraintClauses);
                    }

                    builder[i] = TypeParameterConstraintClause.Create(mergedKind, originalConstraintTypes);
                }
            }

            if (builder != null)
            {
                constraintClauses = builder.ToImmutableAndFree();
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
                    // Try merging nullability of a 'class' constraint
                    TypeParameterConstraintKind clause1Constraints = mergedKind & TypeParameterConstraintKind.AllReferenceTypeKinds;
                    TypeParameterConstraintKind clause2Constraints = clause.Constraints & TypeParameterConstraintKind.AllReferenceTypeKinds;
                    if (clause1Constraints != clause2Constraints)
                    {
                        if (clause1Constraints == TypeParameterConstraintKind.ReferenceType) // Oblivious
                        {
                            // Take nullability from clause2
                            mergedKind = (mergedKind & (~TypeParameterConstraintKind.AllReferenceTypeKinds)) | clause2Constraints;
                        }
                        else if (clause2Constraints != TypeParameterConstraintKind.ReferenceType)
                        {
                            // Neither nullability is oblivious and they do not match. Cannot merge.
                            mergedKind |= TypeParameterConstraintKind.PartialMismatch;
                        }
                    }
                }

                if (originalConstraintTypes.Length == 0 && clause.ConstraintTypes.Length == 0 &&
                    (((mergedKind | clause.Constraints) & ~(TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType | TypeParameterConstraintKind.Constructor)) == 0 &&
                    (mergedKind & TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType) != 0 && // 'object~'
                    (clause.Constraints & TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType) == 0))   // 'object?' )
                {
                    // Try merging nullability of implied 'object' constraint
                    // Merged value is 'object?'
                    mergedKind &= ~TypeParameterConstraintKind.ObliviousNullabilityIfReferenceType;
                }
            }
        }

        internal sealed override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics
        {
            get
            {
                return GetTypeParametersAsTypeArguments();
            }
        }

        public override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get
            {
                if (_lazyTypeParameters.IsDefault)
                {
                    var diagnostics = BindingDiagnosticBag.GetInstance();
                    if (ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameters, MakeTypeParameters(diagnostics)))
                    {
                        AddDeclarationDiagnostics(diagnostics);
                    }

                    diagnostics.Free();
                }

                return _lazyTypeParameters;
            }
        }

        #endregion

        #region Attributes

        internal ImmutableArray<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return declaration.GetAttributeDeclarations();
        }

        IAttributeTargetSymbol IAttributeTargetSymbol.AttributesOwner
        {
            get { return this; }
        }

        AttributeLocation IAttributeTargetSymbol.DefaultAttributeLocation
        {
            get { return AttributeLocation.Type; }
        }

        AttributeLocation IAttributeTargetSymbol.AllowedAttributeLocations
        {
            get
            {
                return TypeKind switch
                {
                    TypeKind.Delegate => AttributeLocation.Type | AttributeLocation.Return,
                    TypeKind.Enum or TypeKind.Interface => AttributeLocation.Type,
                    TypeKind.Struct or TypeKind.Class => AttributeLocation.Type,
                    _ => AttributeLocation.None,
                };
            }
        }

        /// <summary>
        /// Returns a bag of applied custom attributes and data decoded from well-known attributes. Returns null if there are no attributes applied on the symbol.
        /// </summary>
        /// <remarks>
        /// Forces binding and decoding of attributes.
        /// </remarks>
        private CustomAttributesBag<CSharpAttributeData> GetAttributesBag()
        {
            var bag = _lazyCustomAttributesBag;
            if (bag != null && bag.IsSealed)
            {
                return bag;
            }

            if (LoadAndValidateAttributes(OneOrMany.Create(this.GetAttributeDeclarations()), ref _lazyCustomAttributesBag))
            {
                state.NotePartComplete(CompletionPart.Attributes);
            }

            return _lazyCustomAttributesBag;
        }

        /// <summary>
        /// Gets the attributes applied on this symbol.
        /// Returns an empty array if there are no attributes.
        /// </summary>
        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return this.GetAttributesBag().Attributes;
        }

        /// <summary>
        /// Returns data decoded from well-known attributes applied to the symbol or null if there are no applied attributes.
        /// </summary>
        /// <remarks>
        /// Forces binding and decoding of attributes.
        /// </remarks>
        private TypeWellKnownAttributeData GetDecodedWellKnownAttributeData()
        {
            var attributesBag = _lazyCustomAttributesBag;
            if (attributesBag == null || !attributesBag.IsDecodedWellKnownAttributeDataComputed)
            {
                attributesBag = this.GetAttributesBag();
            }

            return (TypeWellKnownAttributeData)attributesBag.DecodedWellKnownAttributeData;
        }

        /// <summary>
        /// Returns data decoded from special early bound well-known attributes applied to the symbol or null if there are no applied attributes.
        /// </summary>
        /// <remarks>
        /// Forces binding and decoding of attributes.
        /// </remarks>
        internal CommonTypeEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData()
        {
            var attributesBag = _lazyCustomAttributesBag;
            if (attributesBag == null || !attributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
            {
                attributesBag = this.GetAttributesBag();
            }

            return (CommonTypeEarlyWellKnownAttributeData)attributesBag.EarlyDecodedWellKnownAttributeData;
        }

        internal override CSharpAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
        {
            bool hasAnyDiagnostics;
            CSharpAttributeData boundAttribute;

            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ComImportAttribute))
            {
                boundAttribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out hasAnyDiagnostics);
                if (!boundAttribute.HasErrors)
                {
                    arguments.GetOrCreateData<CommonTypeEarlyWellKnownAttributeData>().HasComImportAttribute = true;
                    if (!hasAnyDiagnostics)
                    {
                        return boundAttribute;
                    }
                }

                return null;
            }

            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CodeAnalysisEmbeddedAttribute))
            {
                boundAttribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out hasAnyDiagnostics);
                if (!boundAttribute.HasErrors)
                {
                    arguments.GetOrCreateData<CommonTypeEarlyWellKnownAttributeData>().HasCodeAnalysisEmbeddedAttribute = true;
                    if (!hasAnyDiagnostics)
                    {
                        return boundAttribute;
                    }
                }

                return null;
            }

            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ConditionalAttribute))
            {
                boundAttribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out hasAnyDiagnostics);
                if (!boundAttribute.HasErrors)
                {
                    string name = boundAttribute.GetConstructorArgument<string>(0, SpecialType.System_String);
                    arguments.GetOrCreateData<CommonTypeEarlyWellKnownAttributeData>().AddConditionalSymbol(name);
                    if (!hasAnyDiagnostics)
                    {
                        return boundAttribute;
                    }
                }

                return null;
            }

            if (EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out boundAttribute, out ObsoleteAttributeData obsoleteData))
            {
                if (obsoleteData != null)
                {
                    arguments.GetOrCreateData<CommonTypeEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
                }

                return boundAttribute;
            }

            if (CSharpAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.AttributeUsageAttribute))
            {
                boundAttribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out hasAnyDiagnostics);
                if (!boundAttribute.HasErrors)
                {
                    AttributeUsageInfo info = this.DecodeAttributeUsageAttribute(boundAttribute, arguments.AttributeSyntax, diagnose: false);
                    if (!info.IsNull)
                    {
                        var typeData = arguments.GetOrCreateData<CommonTypeEarlyWellKnownAttributeData>();
                        if (typeData.AttributeUsageInfo.IsNull)
                        {
                            typeData.AttributeUsageInfo = info;
                        }

                        if (!hasAnyDiagnostics)
                        {
                            return boundAttribute;
                        }
                    }
                }

                return null;
            }

            return base.EarlyDecodeWellKnownAttribute(ref arguments);
        }

        internal override AttributeUsageInfo GetAttributeUsageInfo()
        {

            CommonTypeEarlyWellKnownAttributeData data = this.GetEarlyDecodedWellKnownAttributeData();
            if (data != null && !data.AttributeUsageInfo.IsNull)
            {
                return data.AttributeUsageInfo;
            }

            return (this.BaseTypeNoUseSiteDiagnostics is not null) ? this.BaseTypeNoUseSiteDiagnostics.GetAttributeUsageInfo() : AttributeUsageInfo.Default;
        }

        /// <summary>
        /// Returns data decoded from Obsolete attribute or null if there is no Obsolete attribute.
        /// This property returns ObsoleteAttributeData.Uninitialized if attribute arguments haven't been decoded yet.
        /// </summary>
        internal override ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                var lazyCustomAttributesBag = _lazyCustomAttributesBag;
                if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
                {
                    var data = (CommonTypeEarlyWellKnownAttributeData)lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData;
                    return data?.ObsoleteAttributeData;
                }

                foreach (var decl in this.declaration.Declarations)
                {
                    if (decl.HasAnyAttributes)
                    {
                        return ObsoleteAttributeData.Uninitialized;
                    }
                }

                return null;
            }
        }

        internal sealed override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            var diagnostics = (BindingDiagnosticBag)arguments.Diagnostics;

            var attribute = arguments.Attribute;

            if (attribute.IsTargetAttribute(this, AttributeDescription.AttributeUsageAttribute))
            {
                DecodeAttributeUsageAttribute(attribute, arguments.AttributeSyntaxOpt, diagnose: true, diagnosticsOpt: diagnostics);
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
                ValidateConditionalAttribute(attribute, arguments.AttributeSyntaxOpt, diagnostics);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.GuidAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().GuidString = attribute.DecodeGuidAttribute(arguments.AttributeSyntaxOpt, diagnostics);
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
                AttributeData.DecodeStructLayoutAttribute<TypeWellKnownAttributeData, AttributeSyntax, CSharpAttributeData, AttributeLocation>(
                    ref arguments, this.DefaultMarshallingCharSet, defaultAutoLayoutSize: 0, messageProvider: MessageProvider.Instance);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SuppressUnmanagedCodeSecurityAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().HasSuppressUnmanagedCodeSecurityAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.ClassInterfaceAttribute))
            {
                attribute.DecodeClassInterfaceAttribute(arguments.AttributeSyntaxOpt, diagnostics);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.InterfaceTypeAttribute))
            {
                attribute.DecodeInterfaceTypeAttribute(arguments.AttributeSyntaxOpt, diagnostics);
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.WindowsRuntimeImportAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().HasWindowsRuntimeImportAttribute = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.RequiredAttributeAttribute))
            {
                // CS1608: The Required attribute is not permitted on C# types
                diagnostics.Add(ErrorCode.ERR_CantUseRequiredAttribute, arguments.AttributeSyntaxOpt.Name.Location);
            }
            else if (ReportExplicitUseOfReservedAttributes(in arguments,
                ReservedAttributes.DynamicAttribute | ReservedAttributes.IsReadOnlyAttribute | ReservedAttributes.IsUnmanagedAttribute | ReservedAttributes.IsByRefLikeAttribute | ReservedAttributes.TupleElementNamesAttribute | ReservedAttributes.NullableAttribute | ReservedAttributes.NullableContextAttribute | ReservedAttributes.NativeIntegerAttribute | ReservedAttributes.CaseSensitiveExtensionAttribute))
            {
                // Nothing to do
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SecurityCriticalAttribute)
                || attribute.IsTargetAttribute(this, AttributeDescription.SecuritySafeCriticalAttribute))
            {
                arguments.GetOrCreateData<TypeWellKnownAttributeData>().HasSecurityCriticalAttributes = true;
            }
            else if (attribute.IsTargetAttribute(this, AttributeDescription.SkipLocalsInitAttribute))
            {
                CSharpAttributeData.DecodeSkipLocalsInitAttribute<TypeWellKnownAttributeData>(DeclaringCompilation, ref arguments);
            }
            else if (_lazyIsExplicitDefinitionOfNoPiaLocalType == ThreeState.Unknown && attribute.IsTargetAttribute(this, AttributeDescription.TypeIdentifierAttribute))
            {
                _lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.True;
            }
            else
            {
                var compilation = this.DeclaringCompilation;
                if (attribute.IsSecurityAttribute(compilation))
                {
                    attribute.DecodeSecurityAttribute<TypeWellKnownAttributeData>(this, compilation, ref arguments);
                }
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

        private void CheckPresenceOfTypeIdentifierAttribute()
        {
            // Have we already decoded well-known attributes?
            if (_lazyCustomAttributesBag?.IsDecodedWellKnownAttributeDataComputed == true)
            {
                return;
            }

            // We want this function to be as cheap as possible, it is called for every top level type
            // and we don't want to bind attributes attached to the declaration unless there is a chance
            // that one of them is TypeIdentifier attribute.
            ImmutableArray<SyntaxList<AttributeListSyntax>> attributeLists = GetAttributeDeclarations();

            foreach (SyntaxList<AttributeListSyntax> list in attributeLists)
            {
                _ = list.Node.SyntaxTree;
                QuickAttributeChecker checker = this.DeclaringCompilation.GetBinderFactory(list.Node.SyntaxTree).GetBinder(list.Node).QuickAttributeChecker;

                foreach (AttributeListSyntax attrList in list)
                {
                    if (attrList.Attributes.Any(attr => checker.IsPossibleMatch(attr, QuickAttributes.TypeIdentifier)))
                    {
                        // This attribute syntax might be an application of TypeIdentifierAttribute.
                        // Let's bind it.
                        // For simplicity we bind all attributes.
                        GetAttributes();
                        return;
                    }
                }
            }
        }

        // Process the specified AttributeUsage attribute on the given ownerSymbol
        private AttributeUsageInfo DecodeAttributeUsageAttribute(CSharpAttributeData attribute, AttributeSyntax node, bool diagnose, BindingDiagnosticBag diagnosticsOpt = null)
        {
            // AttributeUsage can only be specified for attribute classes
            if (!this.DeclaringCompilation.IsAttributeType(this))
            {
                if (diagnose)
                {
                    diagnosticsOpt.Add(ErrorCode.ERR_AttributeUsageOnNonAttributeClass, node.Name.Location, node.GetErrorDisplayName());
                }

                return AttributeUsageInfo.Null;
            }
            else
            {
                AttributeUsageInfo info = attribute.DecodeAttributeUsageAttribute();

                // Validate first ctor argument for AttributeUsage specification is a valid AttributeTargets enum member
                if (!info.HasValidAttributeTargets)
                {
                    if (diagnose)
                    {
                        // invalid attribute target
                        CSharpSyntaxNode attributeArgumentSyntax = attribute.GetAttributeArgumentSyntax(0, node);
                        diagnosticsOpt.Add(ErrorCode.ERR_InvalidAttributeArgument, attributeArgumentSyntax.Location, node.GetErrorDisplayName());
                    }

                    return AttributeUsageInfo.Null;
                }

                return info;
            }
        }

        private void DecodeCoClassAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
            var attribute = arguments.Attribute;

            if (this.IsInterfaceType() && (!arguments.HasDecodedData || ((TypeWellKnownAttributeData)arguments.DecodedData).ComImportCoClass is null))
            {
                TypedConstant argument = attribute.CommonConstructorArguments[0];

                if (argument.ValueInternal is NamedTypeSymbol coClassType && coClassType.TypeKind == TypeKind.Class)
                {
                    arguments.GetOrCreateData<TypeWellKnownAttributeData>().ComImportCoClass = coClassType;
                }
            }
        }

        internal override bool IsComImport
        {
            get
            {
                CommonTypeEarlyWellKnownAttributeData data = this.GetEarlyDecodedWellKnownAttributeData();
                return data != null && data.HasComImportAttribute;
            }
        }

        internal override NamedTypeSymbol ComImportCoClass
        {
            get
            {
                TypeWellKnownAttributeData data = this.GetDecodedWellKnownAttributeData();
                return data?.ComImportCoClass;
            }
        }

        private void ValidateConditionalAttribute(CSharpAttributeData attribute, AttributeSyntax node, BindingDiagnosticBag diagnostics)
        {

            if (!this.DeclaringCompilation.IsAttributeType(this))
            {
                // CS1689: Attribute '{0}' is only valid on methods or attribute classes
                diagnostics.Add(ErrorCode.ERR_ConditionalOnNonAttributeClass, node.Location, node.GetErrorDisplayName());
            }
            else
            {
                string name = attribute.GetConstructorArgument<string>(0, SpecialType.System_String);

                if (name == null || !SyntaxFacts.IsValidIdentifier(name))
                {
                    // CS0633: The argument to the '{0}' attribute must be a valid identifier
                    CSharpSyntaxNode attributeArgumentSyntax = attribute.GetAttributeArgumentSyntax(0, node);
                    diagnostics.Add(ErrorCode.ERR_BadArgumentToAttribute, attributeArgumentSyntax.Location, node.GetErrorDisplayName());
                }
            }
        }

        internal override bool HasSpecialName
        {
            get
            {
                var data = GetDecodedWellKnownAttributeData();
                return data != null && data.HasSpecialNameAttribute;
            }
        }

        internal override bool HasCodeAnalysisEmbeddedAttribute
        {
            get
            {
                var data = GetEarlyDecodedWellKnownAttributeData();
                return data != null && data.HasCodeAnalysisEmbeddedAttribute;
            }
        }

        internal sealed override bool ShouldAddWinRTMembers
        {
            get { return false; }
        }

        internal sealed override bool IsWindowsRuntimeImport
        {
            get
            {
                TypeWellKnownAttributeData data = this.GetDecodedWellKnownAttributeData();
                return data != null && data.HasWindowsRuntimeImportAttribute;
            }
        }

        public sealed override bool IsSerializable
        {
            get
            {
                var data = this.GetDecodedWellKnownAttributeData();
                return data != null && data.HasSerializableAttribute;
            }
        }

        public sealed override bool AreLocalsZeroed
        {
            get
            {
                var data = this.GetDecodedWellKnownAttributeData();
                return data?.HasSkipLocalsInitAttribute != true && (ContainingType?.AreLocalsZeroed ?? ContainingModule.AreLocalsZeroed);
            }
        }

        internal override bool IsDirectlyExcludedFromCodeCoverage =>
            GetDecodedWellKnownAttributeData()?.HasExcludeFromCodeCoverageAttribute == true;

        private bool HasInstanceFields()
        {
            var fields = this.GetFieldsToEmit();
            foreach (var field in fields)
            {
                if (!field.IsStatic)
                {
                    return true;
                }
            }

            return false;
        }

        internal sealed override TypeLayout Layout
        {
            get
            {
                var data = GetDecodedWellKnownAttributeData();
                if (data != null && data.HasStructLayoutAttribute)
                {
                    return data.Layout;
                }

                if (this.TypeKind == TypeKind.Struct)
                {
                    // CLI spec 22.37.16:
                    // "A ValueType shall have a non-zero size - either by defining at least one field, or by providing a non-zero ClassSize"
                    // 
                    // Dev11 compiler sets the value to 1 for structs with no instance fields and no size specified.
                    // It does not change the size value if it was explicitly specified to be 0, nor does it report an error.
                    return new TypeLayout(LayoutKind.Sequential, this.HasInstanceFields() ? 0 : 1, alignment: 0);
                }

                return default;
            }
        }

        internal bool HasStructLayoutAttribute
        {
            get
            {
                var data = GetDecodedWellKnownAttributeData();
                return data != null && data.HasStructLayoutAttribute;
            }
        }

        internal override CharSet MarshallingCharSet
        {
            get
            {
                var data = GetDecodedWellKnownAttributeData();
                return (data != null && data.HasStructLayoutAttribute) ? data.MarshallingCharSet : DefaultMarshallingCharSet;
            }
        }

        internal sealed override bool HasDeclarativeSecurity
        {
            get
            {
                var data = this.GetDecodedWellKnownAttributeData();
                return data != null && data.HasDeclarativeSecurity;
            }
        }

        internal bool HasSecurityCriticalAttributes
        {
            get
            {
                var data = this.GetDecodedWellKnownAttributeData();
                return data != null && data.HasSecurityCriticalAttributes;
            }
        }

        internal sealed override IEnumerable<Microsoft.Cci.SecurityAttribute> GetSecurityInformation()
        {
            var attributesBag = this.GetAttributesBag();
            var wellKnownData = (TypeWellKnownAttributeData)attributesBag.DecodedWellKnownAttributeData;
            if (wellKnownData != null)
            {
                SecurityWellKnownAttributeData securityData = wellKnownData.SecurityInformation;
                if (securityData != null)
                {
                    return securityData.GetSecurityAttributes(attributesBag.Attributes);
                }
            }

            return null;
        }

        internal override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            var data = GetEarlyDecodedWellKnownAttributeData();
            return data != null ? data.ConditionalSymbols : ImmutableArray<string>.Empty;
        }

        internal override void PostDecodeWellKnownAttributes(ImmutableArray<CSharpAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
        {

            var data = (TypeWellKnownAttributeData)decodedData;

            if (this.IsComImport)
            {

                // Symbol with ComImportAttribute must have a GuidAttribute
                if (data == null || data.GuidString == null)
                {
                    int index = boundAttributes.IndexOfAttribute(this, AttributeDescription.ComImportAttribute);
                    diagnostics.Add(ErrorCode.ERR_ComImportWithoutUuidAttribute, allAttributeSyntaxNodes[index].Name.Location, this.Name);
                }

                if (this.TypeKind == TypeKind.Class)
                {
                    var baseType = this.BaseTypeNoUseSiteDiagnostics;
                    if (baseType is not null && baseType.SpecialType != SpecialType.System_Object)
                    {
                        // CS0424: '{0}': a class with the ComImport attribute cannot specify a base class
                        diagnostics.Add(ErrorCode.ERR_ComImportWithBase, this.Locations[0], this.Name);
                    }

                    var initializers = this.StaticInitializers;
                    if (!initializers.IsDefaultOrEmpty)
                    {
                        foreach (var initializerGroup in initializers)
                        {
                            foreach (var singleInitializer in initializerGroup)
                            {
                                if (!singleInitializer.FieldOpt.IsMetadataConstant)
                                {
                                    // CS8028: '{0}': a class with the ComImport attribute cannot specify field initializers.
                                    diagnostics.Add(ErrorCode.ERR_ComImportWithInitializers, singleInitializer.Syntax.GetLocation(), this.Name);
                                }
                            }
                        }
                    }

                    initializers = this.InstanceInitializers;
                    if (!initializers.IsDefaultOrEmpty)
                    {
                        foreach (var initializerGroup in initializers)
                        {
                            foreach (var singleInitializer in initializerGroup)
                            {
                                // CS8028: '{0}': a class with the ComImport attribute cannot specify field initializers.
                                diagnostics.Add(ErrorCode.ERR_ComImportWithInitializers, singleInitializer.Syntax.GetLocation(), this.Name);
                            }
                        }
                    }
                }
            }
            else if (this.ComImportCoClass is not null)
            {

                // Symbol with CoClassAttribute must have a ComImportAttribute
                int index = boundAttributes.IndexOfAttribute(this, AttributeDescription.CoClassAttribute);
                diagnostics.Add(ErrorCode.WRN_CoClassWithoutComImport, allAttributeSyntaxNodes[index].Location, this.Name);
            }

            // Report ERR_DefaultMemberOnIndexedType if type has a default member attribute and has indexers.
            if (data != null && data.HasDefaultMemberAttribute && this.Indexers.Any())
            {

                int index = boundAttributes.IndexOfAttribute(this, AttributeDescription.DefaultMemberAttribute);
                diagnostics.Add(ErrorCode.ERR_DefaultMemberOnIndexedType, allAttributeSyntaxNodes[index].Name.Location);
            }

            base.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData);
        }

        /// <remarks>
        /// These won't be returned by GetAttributes on source methods, but they
        /// will be returned by GetAttributes on metadata symbols.
        /// </remarks>
        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);

            CSharpCompilation compilation = this.DeclaringCompilation;

            if (this.ContainsExtensionMethods)
            {
                // No need to check if [Extension] attribute was explicitly set since
                // we'll issue CS1112 error in those cases and won't generate IL.
                AddSynthesizedAttribute(ref attributes, compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_ExtensionAttribute__ctor));
            }

            if (this.IsRefLikeType)
            {
                AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeIsByRefLikeAttribute(this));

                var obsoleteData = ObsoleteAttributeData;

                // If user specified an Obsolete attribute, we cannot emit ours.
                // NB: we do not check the kind of deprecation. 
                //     we will not emit Obsolete even if Deprecated or Experimental was used.
                //     we do not want to get into a scenario where different kinds of deprecation are combined together.
                //
                if (obsoleteData == null && !this.IsRestrictedType(ignoreSpanLikeTypes: true))
                {
                    AddSynthesizedAttribute(ref attributes, compilation.TrySynthesizeAttribute(WellKnownMember.System_ObsoleteAttribute__ctor,
                        ImmutableArray.Create(
                            new TypedConstant(compilation.GetSpecialType(SpecialType.System_String), TypedConstantKind.Primitive, PEModule.ByRefLikeMarker), // message
                            new TypedConstant(compilation.GetSpecialType(SpecialType.System_Boolean), TypedConstantKind.Primitive, true)), // error=true
                        isOptionalUse: true));
                }
            }

            if (this.IsReadOnly)
            {
                AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeIsReadOnlyAttribute(this));
            }

            if (this.Indexers.Any())
            {
                string defaultMemberName = this.Indexers.First().MetadataName; // UNDONE: IndexerNameAttribute
                var defaultMemberNameConstant = new TypedConstant(compilation.GetSpecialType(SpecialType.System_String), TypedConstantKind.Primitive, defaultMemberName);

                AddSynthesizedAttribute(ref attributes, compilation.TrySynthesizeAttribute(
                    WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor,
                    ImmutableArray.Create(defaultMemberNameConstant)));
            }
        }

        #endregion

        internal override NamedTypeSymbol AsNativeInteger()
        {

            return ContainingAssembly.GetNativeIntegerType(this);
        }

        internal override NamedTypeSymbol NativeIntegerUnderlyingType => null;

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            return t2 is NativeIntegerTypeSymbol nativeInteger ?
                nativeInteger.Equals(this, comparison) :
                base.Equals(t2, comparison);
        }
    }
}

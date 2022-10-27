// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class BoundExpression
    {
        internal BoundExpression WithSuppression(bool suppress = true)
        {
            if (this.IsSuppressed == suppress)
            {
                return this;
            }

            // There is no scenario where suppression goes away

            var result = (BoundExpression)MemberwiseClone();
            result.IsSuppressed = suppress;
            return result;
        }

        internal BoundExpression WithWasConverted()
        {
#if DEBUG
            // We track the WasConverted flag for locals and parameters only, as many other
            // kinds of bound nodes have special behavior that prevents this from working for them.
            // Also we want to minimize the GC pressure, even in Debug, and we have excellent
            // test coverage for locals and parameters.
            if ((Kind != BoundKind.Local && Kind != BoundKind.Parameter) || this.WasConverted)
                return this;

            var result = (BoundExpression)MemberwiseClone();
            result.WasConverted = true;
            return result;
#else
            return this;
#endif
        }

        internal new BoundExpression WithHasErrors()
        {
            return (BoundExpression)base.WithHasErrors();
        }

        internal bool NeedsToBeConverted()
        {
            switch (Kind)
            {
                case BoundKind.TupleLiteral:
                case BoundKind.UnconvertedSwitchExpression:
                case BoundKind.UnconvertedObjectCreationExpression:
                case BoundKind.UnconvertedConditionalOperator:
                case BoundKind.DefaultLiteral:
                case BoundKind.UnconvertedInterpolatedString:
                    return true;
                case BoundKind.StackAllocArrayCreation:
                    // A BoundStackAllocArrayCreation is given a null type when it is in a
                    // syntactic context where it could be either a pointer or a span, and
                    // in that case it requires conversion to one or the other.
                    return this.Type is null;
#if DEBUG
                case BoundKind.Local when !WasConverted:
                case BoundKind.Parameter when !WasConverted:
                    return !WasCompilerGenerated;
#endif
                default:
                    return false;
            }
        }

        public virtual ConstantValue? ConstantValue
        {
            get
            {
                return null;
            }
        }

        public virtual Symbol? ExpressionSymbol
        {
            get
            {
                return null;
            }
        }

        // Indicates any problems with lookup/symbol binding that should be reported via GetSemanticInfo.
        public virtual LookupResultKind ResultKind
        {
            get
            {
                return LookupResultKind.Viable;
            }
        }

        /// <summary>
        /// Returns true if calls and delegate invocations with this
        /// expression as the receiver should be non-virtual calls.
        /// </summary>
        public virtual bool SuppressVirtualCalls
        {
            get
            {
                return false;
            }
        }

        public new NullabilityInfo TopLevelNullability
        {
            get => base.TopLevelNullability;
            set => base.TopLevelNullability = value;
        }

        public CodeAnalysis.ITypeSymbol? GetPublicTypeSymbol()
            => Type?.GetITypeSymbol(TopLevelNullability.FlowState.ToAnnotation());
    }

    public partial class BoundPassByCopy
    {
        public override ConstantValue? ConstantValue
        {
            get
            {
                return null;
            }
        }

        public override Symbol? ExpressionSymbol
        {
            get
            {
                return Expression.ExpressionSymbol;
            }
        }
    }

    public partial class BoundCall
    {
        public override Symbol ExpressionSymbol
        {
            get
            {
                return this.Method;
            }
        }
    }

    public partial class BoundTypeExpression
    {
        public override Symbol ExpressionSymbol
        {
            get { return this.AliasOpt ?? (Symbol)this.Type; }
        }

        public override LookupResultKind ResultKind
        {
            get
            {
                ErrorTypeSymbol? errorType = this.Type.OriginalDefinition as ErrorTypeSymbol;
                if (errorType is { })
                    return errorType.ResultKind;
                else
                    return LookupResultKind.Viable;
            }
        }
    }

    public partial class BoundNamespaceExpression
    {
        public override Symbol ExpressionSymbol
        {
            get { return this.AliasOpt ?? (Symbol)this.NamespaceSymbol; }
        }
    }

    public partial class BoundLocal
    {
        public override ConstantValue? ConstantValue
        {
            get { return this.ConstantValueOpt; }
        }

        public override Symbol ExpressionSymbol
        {
            get { return this.LocalSymbol; }
        }

        public BoundLocal(SyntaxNode syntax, LocalSymbol localSymbol, ConstantValue? constantValueOpt, TypeSymbol type, bool hasErrors = false)
            : this(syntax, localSymbol, BoundLocalDeclarationKind.None, constantValueOpt, false, type, hasErrors)
        {
        }

        public BoundLocal Update(LocalSymbol localSymbol, ConstantValue? constantValueOpt, TypeSymbol type)
        {
            return this.Update(localSymbol, this.DeclarationKind, constantValueOpt, this.IsNullableUnknown, type);
        }
    }

    public partial class BoundFieldAccess
    {
        public override ConstantValue? ConstantValue
        {
            get { return this.ConstantValueOpt; }
        }

        public override Symbol? ExpressionSymbol
        {
            get { return this.FieldSymbol; }
        }
    }

    public partial class BoundPropertyAccess
    {
        public override Symbol? ExpressionSymbol
        {
            get { return this.PropertySymbol; }
        }
    }

    public partial class BoundIndexerAccess
    {
        public override Symbol? ExpressionSymbol
        {
            get { return this.Indexer; }
        }

        public override LookupResultKind ResultKind
        {
            get
            {
                return !this.OriginalIndexersOpt.IsDefault ? LookupResultKind.OverloadResolutionFailure : base.ResultKind;
            }
        }
    }

    public partial class BoundDynamicIndexerAccess
    {
        internal string? TryGetIndexedPropertyName()
        {
            foreach (var indexer in ApplicableIndexers)
            {
                if (!indexer.IsIndexer && indexer.IsIndexedProperty)
                {
                    return indexer.Name;
                }
            }

            return null;
        }
    }

    public partial class BoundEventAccess
    {
        public override Symbol ExpressionSymbol
        {
            get { return this.EventSymbol; }
        }
    }

    public partial class BoundParameter
    {
        public override Symbol ExpressionSymbol
        {
            get { return this.ParameterSymbol; }
        }
    }

    public partial class BoundBinaryOperator
    {
        public override ConstantValue? ConstantValue
        {
            get { return this.ConstantValueOpt; }
        }

        public override Symbol? ExpressionSymbol
        {
            get { return this.MethodOpt; }
        }
    }

    public partial class BoundInterpolatedStringBase
    {
        public sealed override ConstantValue? ConstantValue
        {
            get { return this.ConstantValueOpt; }
        }
    }

    public partial class BoundUserDefinedConditionalLogicalOperator
    {
        public override Symbol ExpressionSymbol
        {
            get { return this.LogicalOperator; }
        }
    }

    public partial class BoundUnaryOperator
    {
        public override ConstantValue? ConstantValue
        {
            get { return this.ConstantValueOpt; }
        }

        public override Symbol? ExpressionSymbol
        {
            get { return this.MethodOpt; }
        }
    }

    public partial class BoundIncrementOperator
    {
        public override Symbol? ExpressionSymbol
        {
            get { return this.MethodOpt; }
        }
    }

    public partial class BoundCompoundAssignmentOperator
    {
        public override Symbol? ExpressionSymbol
        {
            get { return this.Operator.Method; }
        }
    }

    public partial class BoundLiteral
    {
        public override ConstantValue? ConstantValue
        {
            get { return this.ConstantValueOpt; }
        }
    }

    public partial class BoundConversion
    {
        public override ConstantValue? ConstantValue
        {
            get { return this.ConstantValueOpt; }
        }

        public ConversionKind ConversionKind
        {
            get { return this.Conversion.Kind; }
        }

        public bool IsExtensionMethod
        {
            get { return this.Conversion.IsExtensionMethod; }
        }

        public MethodSymbol? SymbolOpt
        {
            get { return this.Conversion.Method; }
        }

        public override Symbol? ExpressionSymbol
        {
            get { return this.SymbolOpt; }
        }

        public override bool SuppressVirtualCalls
        {
            get { return this.IsBaseConversion; }
        }

        public BoundConversion UpdateOperand(BoundExpression operand)
        {
            return this.Update(operand: operand, this.Conversion, this.IsBaseConversion, this.Checked, this.ExplicitCastInCode, this.ConstantValue, this.ConversionGroupOpt, this.OriginalUserDefinedConversionsOpt, this.Type);
        }

        /// <summary>
        /// Returns true when conversion itself (not the operand) may have side-effects
        /// A typical side-effect of a conversion is an exception when conversion is unsuccessful.
        /// </summary>
        /// <returns></returns>
        internal bool ConversionHasSideEffects()
        {
            // only some intrinsic conversions are side effect free
            // the only side effect of an intrinsic conversion is a throw when we fail to convert.
            // and some intrinsic conversion always succeed
            return this.ConversionKind switch
            {
                ConversionKind.Identity or ConversionKind.ImplicitNumeric or ConversionKind.ImplicitEnumeration or ConversionKind.ImplicitReference or ConversionKind.Boxing => false,
                // unchecked numeric conversion does not throw
                ConversionKind.ExplicitNumeric => this.Checked,
                _ => true,
            };
        }
    }

    public partial class BoundObjectCreationExpression
    {
        public override ConstantValue? ConstantValue
        {
            get { return this.ConstantValueOpt; }
        }

        public override Symbol ExpressionSymbol
        {
            get { return this.Constructor; }
        }

        /// <summary>
        /// Build an object creation expression without performing any rewriting
        /// </summary>
        internal BoundObjectCreationExpression UpdateArgumentsAndInitializer(
            ImmutableArray<BoundExpression> newArguments,
            ImmutableArray<RefKind> newRefKinds,
            BoundObjectInitializerExpressionBase? newInitializerExpression,
            TypeSymbol? changeTypeOpt = null)
        {
            return Update(
                constructor: Constructor,
                arguments: newArguments,
                argumentNamesOpt: default,
                argumentRefKindsOpt: newRefKinds,
                expanded: false,
                argsToParamsOpt: default,
                defaultArguments: default,
                constantValueOpt: ConstantValueOpt,
                initializerExpressionOpt: newInitializerExpression,
                type: changeTypeOpt ?? Type);
        }
    }

    public partial class BoundAnonymousObjectCreationExpression
    {
        public override Symbol ExpressionSymbol
        {
            get { return this.Constructor; }
        }
    }

    public partial class BoundAnonymousPropertyDeclaration
    {
        public override Symbol ExpressionSymbol
        {
            get { return this.Property; }
        }
    }

    public partial class BoundLambda
    {
        public override Symbol ExpressionSymbol
        {
            get { return this.Symbol; }
        }
    }

    public partial class BoundAttribute
    {
        public override Symbol? ExpressionSymbol
        {
            get { return this.Constructor; }
        }
    }

    public partial class BoundDefaultLiteral
    {
        public override ConstantValue? ConstantValue
        {
            get { return null; }
        }
    }

    public partial class BoundConditionalOperator
    {
        public override ConstantValue? ConstantValue
        {
            get
            {
                return this.ConstantValueOpt;
            }
        }

        public bool IsDynamic
        {
            get
            {
                // IsTrue dynamic operator is invoked at runtime if the condition is of the type dynamic.
                // The type of the operator itself is Boolean, so we need to check its kind.
                return this.Condition.Kind == BoundKind.UnaryOperator && ((BoundUnaryOperator)this.Condition).OperatorKind.IsDynamic();
            }
        }
    }

    public partial class BoundUnconvertedConditionalOperator
    {
        public override ConstantValue? ConstantValue
        {
            get
            {
                return this.ConstantValueOpt;
            }
        }
    }

    public partial class BoundSizeOfOperator
    {
        public override ConstantValue? ConstantValue
        {
            get
            {
                return this.ConstantValueOpt;
            }
        }
    }

    public partial class BoundRangeVariable
    {
        public override Symbol ExpressionSymbol
        {
            get
            {
                return this.RangeVariableSymbol;
            }
        }
    }

    public partial class BoundLabel
    {
        public override Symbol ExpressionSymbol
        {
            get
            {
                return this.Label;
            }
        }
    }

    public partial class BoundObjectInitializerMember
    {
        public override Symbol? ExpressionSymbol
        {
            get
            {
                return this.MemberSymbol;
            }
        }
    }

    public partial class BoundCollectionElementInitializer
    {
        public override Symbol ExpressionSymbol
        {
            get
            {
                return this.AddMethod;
            }
        }
    }

    public partial class BoundBaseReference
    {
        public override bool SuppressVirtualCalls
        {
            get { return true; }
        }
    }

    public partial class BoundNameOfOperator
    {
        public override ConstantValue ConstantValue
        {
            get
            {
                return this.ConstantValueOpt;
            }
        }
    }

    // NOTE: this type exists in order to hide the presence of {Value,Type}Expression inside of a
    //       BoundTypeOrValueExpression from the bound tree generator, which would otherwise generate
    //       a constructor that may spuriously set hasErrors to true if either field had errors.
    //       A BoundTypeOrValueExpression should never have errors if it is present in the tree.
    public struct BoundTypeOrValueData : IEquatable<BoundTypeOrValueData>
    {
        public Symbol ValueSymbol { get; }
        public BoundExpression ValueExpression { get; }
        public BindingDiagnosticBag ValueDiagnostics { get; }
        public BoundExpression TypeExpression { get; }
        public BindingDiagnosticBag TypeDiagnostics { get; }

        public BoundTypeOrValueData(Symbol valueSymbol, BoundExpression valueExpression, BindingDiagnosticBag valueDiagnostics, BoundExpression typeExpression, BindingDiagnosticBag typeDiagnostics)
        {

            this.ValueSymbol = valueSymbol;
            this.ValueExpression = valueExpression;
            this.ValueDiagnostics = valueDiagnostics;
            this.TypeExpression = typeExpression;
            this.TypeDiagnostics = typeDiagnostics;
        }

        // operator==, operator!=, GetHashCode, and Equals are needed by the generated bound tree.

        public static bool operator ==(BoundTypeOrValueData a, BoundTypeOrValueData b)
        {
            return a.ValueSymbol == (object)b.ValueSymbol &&
                a.ValueExpression == b.ValueExpression &&
                a.ValueDiagnostics == b.ValueDiagnostics &&
                a.TypeExpression == b.TypeExpression &&
                a.TypeDiagnostics == b.TypeDiagnostics;
        }

        public static bool operator !=(BoundTypeOrValueData a, BoundTypeOrValueData b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            return obj is BoundTypeOrValueData data && data == this;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(ValueSymbol.GetHashCode(),
                Hash.Combine(ValueExpression.GetHashCode(),
                Hash.Combine(ValueDiagnostics.GetHashCode(),
                Hash.Combine(TypeExpression.GetHashCode(), TypeDiagnostics.GetHashCode()))));
        }

        bool System.IEquatable<BoundTypeOrValueData>.Equals(BoundTypeOrValueData b)
        {
            return b == this;
        }
    }

    public partial class BoundTupleExpression
    {
        /// <summary>
        /// Applies action to all the nested elements of this tuple.
        /// </summary>
        internal void VisitAllElements<T>(Action<BoundExpression, T> action, T args)
        {
            foreach (var argument in this.Arguments)
            {
                if (argument.Kind == BoundKind.TupleLiteral)
                {
                    ((BoundTupleExpression)argument).VisitAllElements(action, args);
                }
                else
                {
                    action(argument, args);
                }
            }
        }
    }
}

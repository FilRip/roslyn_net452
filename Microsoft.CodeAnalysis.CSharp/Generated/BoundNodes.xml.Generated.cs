#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public enum BoundKind : byte
    {
        FieldEqualsValue,
        PropertyEqualsValue,
        ParameterEqualsValue,
        GlobalStatementInitializer,
        DeconstructValuePlaceholder,
        TupleOperandPlaceholder,
        AwaitableValuePlaceholder,
        DisposableValuePlaceholder,
        ObjectOrCollectionValuePlaceholder,
        Dup,
        PassByCopy,
        BadExpression,
        BadStatement,
        ExtractedFinallyBlock,
        TypeExpression,
        TypeOrValueExpression,
        NamespaceExpression,
        UnaryOperator,
        IncrementOperator,
        AddressOfOperator,
        UnconvertedAddressOfOperator,
        FunctionPointerLoad,
        PointerIndirectionOperator,
        PointerElementAccess,
        FunctionPointerInvocation,
        RefTypeOperator,
        MakeRefOperator,
        RefValueOperator,
        FromEndIndexExpression,
        RangeExpression,
        BinaryOperator,
        TupleBinaryOperator,
        UserDefinedConditionalLogicalOperator,
        CompoundAssignmentOperator,
        AssignmentOperator,
        DeconstructionAssignmentOperator,
        NullCoalescingOperator,
        NullCoalescingAssignmentOperator,
        UnconvertedConditionalOperator,
        ConditionalOperator,
        ArrayAccess,
        ArrayLength,
        AwaitableInfo,
        AwaitExpression,
        TypeOfOperator,
        MethodDefIndex,
        MaximumMethodDefIndex,
        InstrumentationPayloadRoot,
        ModuleVersionId,
        ModuleVersionIdString,
        SourceDocumentIndex,
        MethodInfo,
        FieldInfo,
        DefaultLiteral,
        DefaultExpression,
        IsOperator,
        AsOperator,
        SizeOfOperator,
        Conversion,
        ReadOnlySpanFromArray,
        ArgList,
        ArgListOperator,
        FixedLocalCollectionInitializer,
        SequencePoint,
        SequencePointWithSpan,
        SavePreviousSequencePoint,
        RestorePreviousSequencePoint,
        StepThroughSequencePoint,
        Block,
        Scope,
        StateMachineScope,
        LocalDeclaration,
        MultipleLocalDeclarations,
        UsingLocalDeclarations,
        LocalFunctionStatement,
        NoOpStatement,
        ReturnStatement,
        YieldReturnStatement,
        YieldBreakStatement,
        ThrowStatement,
        ExpressionStatement,
        BreakStatement,
        ContinueStatement,
        SwitchStatement,
        SwitchDispatch,
        IfStatement,
        DoStatement,
        WhileStatement,
        ForStatement,
        ForEachStatement,
        ForEachDeconstructStep,
        UsingStatement,
        FixedStatement,
        LockStatement,
        TryStatement,
        CatchBlock,
        Literal,
        ThisReference,
        PreviousSubmissionReference,
        HostObjectMemberReference,
        BaseReference,
        Local,
        PseudoVariable,
        RangeVariable,
        Parameter,
        LabelStatement,
        GotoStatement,
        LabeledStatement,
        Label,
        StatementList,
        ConditionalGoto,
        SwitchExpressionArm,
        UnconvertedSwitchExpression,
        ConvertedSwitchExpression,
        DecisionDag,
        EvaluationDecisionDagNode,
        TestDecisionDagNode,
        WhenDecisionDagNode,
        LeafDecisionDagNode,
        DagTemp,
        DagTypeTest,
        DagNonNullTest,
        DagExplicitNullTest,
        DagValueTest,
        DagRelationalTest,
        DagDeconstructEvaluation,
        DagTypeEvaluation,
        DagFieldEvaluation,
        DagPropertyEvaluation,
        DagIndexEvaluation,
        SwitchSection,
        SwitchLabel,
        SequencePointExpression,
        Sequence,
        SpillSequence,
        DynamicMemberAccess,
        DynamicInvocation,
        ConditionalAccess,
        LoweredConditionalAccess,
        ConditionalReceiver,
        ComplexConditionalReceiver,
        MethodGroup,
        PropertyGroup,
        Call,
        EventAssignmentOperator,
        Attribute,
        UnconvertedObjectCreationExpression,
        ObjectCreationExpression,
        TupleLiteral,
        ConvertedTupleLiteral,
        DynamicObjectCreationExpression,
        NoPiaObjectCreationExpression,
        ObjectInitializerExpression,
        ObjectInitializerMember,
        DynamicObjectInitializerMember,
        CollectionInitializerExpression,
        CollectionElementInitializer,
        DynamicCollectionElementInitializer,
        ImplicitReceiver,
        AnonymousObjectCreationExpression,
        AnonymousPropertyDeclaration,
        NewT,
        DelegateCreationExpression,
        ArrayCreation,
        ArrayInitialization,
        StackAllocArrayCreation,
        ConvertedStackAllocExpression,
        FieldAccess,
        HoistedFieldAccess,
        PropertyAccess,
        EventAccess,
        IndexerAccess,
        IndexOrRangePatternIndexerAccess,
        DynamicIndexerAccess,
        Lambda,
        UnboundLambda,
        QueryClause,
        TypeOrInstanceInitializers,
        NameOfOperator,
        UnconvertedInterpolatedString,
        InterpolatedString,
        StringInsert,
        IsPatternExpression,
        ConstantPattern,
        DiscardPattern,
        DeclarationPattern,
        RecursivePattern,
        ITuplePattern,
        Subpattern,
        TypePattern,
        BinaryPattern,
        NegatedPattern,
        RelationalPattern,
        DiscardExpression,
        ThrowExpression,
        OutVariablePendingInference,
        DeconstructionVariablePendingInference,
        OutDeconstructVarPendingInference,
        NonConstructorMethodBody,
        ConstructorMethodBody,
        ExpressionWithNullability,
        WithExpression,
    }

    public abstract partial class BoundInitializer : BoundNode
    {
        protected BoundInitializer(BoundKind kind, SyntaxNode syntax, bool hasErrors)
            : base(kind, syntax, hasErrors)
        {
        }

        protected BoundInitializer(BoundKind kind, SyntaxNode syntax)
            : base(kind, syntax)
        {
        }

    }

    public abstract partial class BoundEqualsValue : BoundInitializer
    {
        protected BoundEqualsValue(BoundKind kind, SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpression value, bool hasErrors = false)
            : base(kind, syntax, hasErrors)
        {
            this.Locals = locals;
            this.Value = value;
        }


        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundExpression Value { get; }
    }

    public sealed partial class BoundFieldEqualsValue : BoundEqualsValue
    {
        public BoundFieldEqualsValue(SyntaxNode syntax, FieldSymbol field, ImmutableArray<LocalSymbol> locals, BoundExpression value, bool hasErrors = false)
            : base(BoundKind.FieldEqualsValue, syntax, locals, value, hasErrors || value.HasErrors())
        {
            this.Field = field;
        }


        public FieldSymbol Field { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitFieldEqualsValue(this);

        public BoundFieldEqualsValue Update(FieldSymbol field, ImmutableArray<LocalSymbol> locals, BoundExpression value)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(field, this.Field) || locals != this.Locals || value != this.Value)
            {
                var result = new BoundFieldEqualsValue(this.Syntax, field, locals, value, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundPropertyEqualsValue : BoundEqualsValue
    {
        public BoundPropertyEqualsValue(SyntaxNode syntax, PropertySymbol property, ImmutableArray<LocalSymbol> locals, BoundExpression value, bool hasErrors = false)
            : base(BoundKind.PropertyEqualsValue, syntax, locals, value, hasErrors || value.HasErrors())
        {
            this.Property = property;
        }


        public PropertySymbol Property { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitPropertyEqualsValue(this);

        public BoundPropertyEqualsValue Update(PropertySymbol property, ImmutableArray<LocalSymbol> locals, BoundExpression value)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(property, this.Property) || locals != this.Locals || value != this.Value)
            {
                var result = new BoundPropertyEqualsValue(this.Syntax, property, locals, value, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundParameterEqualsValue : BoundEqualsValue
    {
        public BoundParameterEqualsValue(SyntaxNode syntax, ParameterSymbol parameter, ImmutableArray<LocalSymbol> locals, BoundExpression value, bool hasErrors = false)
            : base(BoundKind.ParameterEqualsValue, syntax, locals, value, hasErrors || value.HasErrors())
        {
            this.Parameter = parameter;
        }


        public ParameterSymbol Parameter { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitParameterEqualsValue(this);

        public BoundParameterEqualsValue Update(ParameterSymbol parameter, ImmutableArray<LocalSymbol> locals, BoundExpression value)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(parameter, this.Parameter) || locals != this.Locals || value != this.Value)
            {
                var result = new BoundParameterEqualsValue(this.Syntax, parameter, locals, value, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundGlobalStatementInitializer : BoundInitializer
    {
        public BoundGlobalStatementInitializer(SyntaxNode syntax, BoundStatement statement, bool hasErrors = false)
            : base(BoundKind.GlobalStatementInitializer, syntax, hasErrors || statement.HasErrors())
        {
            this.Statement = statement;
        }


        public BoundStatement Statement { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitGlobalStatementInitializer(this);

        public BoundGlobalStatementInitializer Update(BoundStatement statement)
        {
            if (statement != this.Statement)
            {
                var result = new BoundGlobalStatementInitializer(this.Syntax, statement, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundExpression : BoundNode
    {
        protected BoundExpression(BoundKind kind, SyntaxNode syntax, TypeSymbol? type, bool hasErrors)
            : base(kind, syntax, hasErrors)
        {
            this.Type = type;
        }

        protected BoundExpression(BoundKind kind, SyntaxNode syntax, TypeSymbol? type)
            : base(kind, syntax)
        {
            this.Type = type;
        }


        public TypeSymbol? Type { get; }
    }

    public abstract partial class BoundValuePlaceholderBase : BoundExpression
    {
        protected BoundValuePlaceholderBase(BoundKind kind, SyntaxNode syntax, TypeSymbol? type, bool hasErrors)
            : base(kind, syntax, type, hasErrors)
        {
        }

        protected BoundValuePlaceholderBase(BoundKind kind, SyntaxNode syntax, TypeSymbol? type)
            : base(kind, syntax, type)
        {
        }

    }

    public sealed partial class BoundDeconstructValuePlaceholder : BoundValuePlaceholderBase
    {
        public BoundDeconstructValuePlaceholder(SyntaxNode syntax, uint valEscape, TypeSymbol type, bool hasErrors)
            : base(BoundKind.DeconstructValuePlaceholder, syntax, type, hasErrors)
        {
            this.ValEscape = valEscape;
        }

        public BoundDeconstructValuePlaceholder(SyntaxNode syntax, uint valEscape, TypeSymbol type)
            : base(BoundKind.DeconstructValuePlaceholder, syntax, type)
        {
            this.ValEscape = valEscape;
        }


        public new TypeSymbol Type => base.Type!;

        public uint ValEscape { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDeconstructValuePlaceholder(this);

        public BoundDeconstructValuePlaceholder Update(uint valEscape, TypeSymbol type)
        {
            if (valEscape != this.ValEscape || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDeconstructValuePlaceholder(this.Syntax, valEscape, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundTupleOperandPlaceholder : BoundValuePlaceholderBase
    {
        public BoundTupleOperandPlaceholder(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.TupleOperandPlaceholder, syntax, type, hasErrors)
        {
        }

        public BoundTupleOperandPlaceholder(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.TupleOperandPlaceholder, syntax, type)
        {
        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitTupleOperandPlaceholder(this);

        public BoundTupleOperandPlaceholder Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundTupleOperandPlaceholder(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundAwaitableValuePlaceholder : BoundValuePlaceholderBase
    {
        public BoundAwaitableValuePlaceholder(SyntaxNode syntax, uint valEscape, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.AwaitableValuePlaceholder, syntax, type, hasErrors)
        {
            this.ValEscape = valEscape;
        }

        public BoundAwaitableValuePlaceholder(SyntaxNode syntax, uint valEscape, TypeSymbol? type)
            : base(BoundKind.AwaitableValuePlaceholder, syntax, type)
        {
            this.ValEscape = valEscape;
        }


        public new TypeSymbol? Type => base.Type;

        public uint ValEscape { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitAwaitableValuePlaceholder(this);

        public BoundAwaitableValuePlaceholder Update(uint valEscape, TypeSymbol? type)
        {
            if (valEscape != this.ValEscape || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundAwaitableValuePlaceholder(this.Syntax, valEscape, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDisposableValuePlaceholder : BoundValuePlaceholderBase
    {
        public BoundDisposableValuePlaceholder(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.DisposableValuePlaceholder, syntax, type, hasErrors)
        {
        }

        public BoundDisposableValuePlaceholder(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.DisposableValuePlaceholder, syntax, type)
        {
        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDisposableValuePlaceholder(this);

        public BoundDisposableValuePlaceholder Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDisposableValuePlaceholder(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundObjectOrCollectionValuePlaceholder : BoundValuePlaceholderBase
    {
        public BoundObjectOrCollectionValuePlaceholder(SyntaxNode syntax, bool isNewInstance, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ObjectOrCollectionValuePlaceholder, syntax, type, hasErrors)
        {
            this.IsNewInstance = isNewInstance;
        }

        public BoundObjectOrCollectionValuePlaceholder(SyntaxNode syntax, bool isNewInstance, TypeSymbol type)
            : base(BoundKind.ObjectOrCollectionValuePlaceholder, syntax, type)
        {
            this.IsNewInstance = isNewInstance;
        }


        public new TypeSymbol Type => base.Type!;

        public bool IsNewInstance { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitObjectOrCollectionValuePlaceholder(this);

        public BoundObjectOrCollectionValuePlaceholder Update(bool isNewInstance, TypeSymbol type)
        {
            if (isNewInstance != this.IsNewInstance || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundObjectOrCollectionValuePlaceholder(this.Syntax, isNewInstance, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDup : BoundExpression
    {
        public BoundDup(SyntaxNode syntax, RefKind refKind, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.Dup, syntax, type, hasErrors)
        {
            this.RefKind = refKind;
        }

        public BoundDup(SyntaxNode syntax, RefKind refKind, TypeSymbol? type)
            : base(BoundKind.Dup, syntax, type)
        {
            this.RefKind = refKind;
        }


        public RefKind RefKind { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDup(this);

        public BoundDup Update(RefKind refKind, TypeSymbol? type)
        {
            if (refKind != this.RefKind || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDup(this.Syntax, refKind, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundPassByCopy : BoundExpression
    {
        public BoundPassByCopy(SyntaxNode syntax, BoundExpression expression, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.PassByCopy, syntax, type, hasErrors || expression.HasErrors())
        {
            this.Expression = expression;
        }


        public BoundExpression Expression { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitPassByCopy(this);

        public BoundPassByCopy Update(BoundExpression expression, TypeSymbol? type)
        {
            if (expression != this.Expression || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundPassByCopy(this.Syntax, expression, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundBadExpression : BoundExpression
    {
        public BoundBadExpression(SyntaxNode syntax, LookupResultKind resultKind, ImmutableArray<Symbol?> symbols, ImmutableArray<BoundExpression> childBoundNodes, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.BadExpression, syntax, type, hasErrors || childBoundNodes.HasErrors())
        {
            this._ResultKind = resultKind;
            this.Symbols = symbols;
            this.ChildBoundNodes = childBoundNodes;
        }


        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }

        public ImmutableArray<Symbol?> Symbols { get; }

        public ImmutableArray<BoundExpression> ChildBoundNodes { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitBadExpression(this);

        public BoundBadExpression Update(LookupResultKind resultKind, ImmutableArray<Symbol?> symbols, ImmutableArray<BoundExpression> childBoundNodes, TypeSymbol? type)
        {
            if (resultKind != this.ResultKind || symbols != this.Symbols || childBoundNodes != this.ChildBoundNodes || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundBadExpression(this.Syntax, resultKind, symbols, childBoundNodes, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundBadStatement : BoundStatement
    {
        public BoundBadStatement(SyntaxNode syntax, ImmutableArray<BoundNode> childBoundNodes, bool hasErrors = false)
            : base(BoundKind.BadStatement, syntax, hasErrors || childBoundNodes.HasErrors())
        {
            this.ChildBoundNodes = childBoundNodes;
        }


        public ImmutableArray<BoundNode> ChildBoundNodes { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitBadStatement(this);

        public BoundBadStatement Update(ImmutableArray<BoundNode> childBoundNodes)
        {
            if (childBoundNodes != this.ChildBoundNodes)
            {
                var result = new BoundBadStatement(this.Syntax, childBoundNodes, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundExtractedFinallyBlock : BoundStatement
    {
        public BoundExtractedFinallyBlock(SyntaxNode syntax, BoundBlock finallyBlock, bool hasErrors = false)
            : base(BoundKind.ExtractedFinallyBlock, syntax, hasErrors || finallyBlock.HasErrors())
        {
            this.FinallyBlock = finallyBlock;
        }


        public BoundBlock FinallyBlock { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitExtractedFinallyBlock(this);

        public BoundExtractedFinallyBlock Update(BoundBlock finallyBlock)
        {
            if (finallyBlock != this.FinallyBlock)
            {
                var result = new BoundExtractedFinallyBlock(this.Syntax, finallyBlock, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundTypeExpression : BoundExpression
    {
        public BoundTypeExpression(SyntaxNode syntax, AliasSymbol? aliasOpt, BoundTypeExpression? boundContainingTypeOpt, ImmutableArray<BoundExpression> boundDimensionsOpt, TypeWithAnnotations typeWithAnnotations, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.TypeExpression, syntax, type, hasErrors || boundContainingTypeOpt.HasErrors() || boundDimensionsOpt.HasErrors())
        {
            this.AliasOpt = aliasOpt;
            this.BoundContainingTypeOpt = boundContainingTypeOpt;
            this.BoundDimensionsOpt = boundDimensionsOpt;
            this.TypeWithAnnotations = typeWithAnnotations;
        }


        public AliasSymbol? AliasOpt { get; }

        public BoundTypeExpression? BoundContainingTypeOpt { get; }

        public ImmutableArray<BoundExpression> BoundDimensionsOpt { get; }

        public new TypeSymbol Type => base.Type!;

        public TypeWithAnnotations TypeWithAnnotations { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitTypeExpression(this);

        public BoundTypeExpression Update(AliasSymbol? aliasOpt, BoundTypeExpression? boundContainingTypeOpt, ImmutableArray<BoundExpression> boundDimensionsOpt, TypeWithAnnotations typeWithAnnotations, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(aliasOpt, this.AliasOpt) || boundContainingTypeOpt != this.BoundContainingTypeOpt || boundDimensionsOpt != this.BoundDimensionsOpt || typeWithAnnotations != this.TypeWithAnnotations || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundTypeExpression(this.Syntax, aliasOpt, boundContainingTypeOpt, boundDimensionsOpt, typeWithAnnotations, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundTypeOrValueExpression : BoundExpression
    {
        public BoundTypeOrValueExpression(SyntaxNode syntax, BoundTypeOrValueData data, TypeSymbol type, bool hasErrors)
            : base(BoundKind.TypeOrValueExpression, syntax, type, hasErrors)
        {
            this.Data = data;
        }

        public BoundTypeOrValueExpression(SyntaxNode syntax, BoundTypeOrValueData data, TypeSymbol type)
            : base(BoundKind.TypeOrValueExpression, syntax, type)
        {
            this.Data = data;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundTypeOrValueData Data { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitTypeOrValueExpression(this);

        public BoundTypeOrValueExpression Update(BoundTypeOrValueData data, TypeSymbol type)
        {
            if (data != this.Data || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundTypeOrValueExpression(this.Syntax, data, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundNamespaceExpression : BoundExpression
    {
        public BoundNamespaceExpression(SyntaxNode syntax, NamespaceSymbol namespaceSymbol, AliasSymbol? aliasOpt, bool hasErrors)
            : base(BoundKind.NamespaceExpression, syntax, null, hasErrors)
        {
            this.NamespaceSymbol = namespaceSymbol;
            this.AliasOpt = aliasOpt;
        }

        public BoundNamespaceExpression(SyntaxNode syntax, NamespaceSymbol namespaceSymbol, AliasSymbol? aliasOpt)
            : base(BoundKind.NamespaceExpression, syntax, null)
        {
            this.NamespaceSymbol = namespaceSymbol;
            this.AliasOpt = aliasOpt;
        }


        public new TypeSymbol? Type => base.Type;

        public NamespaceSymbol NamespaceSymbol { get; }

        public AliasSymbol? AliasOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitNamespaceExpression(this);

        public BoundNamespaceExpression Update(NamespaceSymbol namespaceSymbol, AliasSymbol? aliasOpt)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(namespaceSymbol, this.NamespaceSymbol) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(aliasOpt, this.AliasOpt))
            {
                var result = new BoundNamespaceExpression(this.Syntax, namespaceSymbol, aliasOpt, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundUnaryOperator : BoundExpression
    {
        public BoundUnaryOperator(SyntaxNode syntax, UnaryOperatorKind operatorKind, BoundExpression operand, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.UnaryOperator, syntax, type, hasErrors || operand.HasErrors())
        {
            this.OperatorKind = operatorKind;
            this.Operand = operand;
            this.ConstantValueOpt = constantValueOpt;
            this.MethodOpt = methodOpt;
            this._ResultKind = resultKind;
            this.OriginalUserDefinedOperatorsOpt = originalUserDefinedOperatorsOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public UnaryOperatorKind OperatorKind { get; }

        public BoundExpression Operand { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public MethodSymbol? MethodOpt { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }

        public ImmutableArray<MethodSymbol> OriginalUserDefinedOperatorsOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitUnaryOperator(this);

        public BoundUnaryOperator Update(UnaryOperatorKind operatorKind, BoundExpression operand, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type)
        {
            if (operatorKind != this.OperatorKind || operand != this.Operand || constantValueOpt != this.ConstantValueOpt || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, this.MethodOpt) || resultKind != this.ResultKind || originalUserDefinedOperatorsOpt != this.OriginalUserDefinedOperatorsOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundUnaryOperator(this.Syntax, operatorKind, operand, constantValueOpt, methodOpt, resultKind, originalUserDefinedOperatorsOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundIncrementOperator : BoundExpression
    {
        public BoundIncrementOperator(SyntaxNode syntax, UnaryOperatorKind operatorKind, BoundExpression operand, MethodSymbol? methodOpt, Conversion operandConversion, Conversion resultConversion, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.IncrementOperator, syntax, type, hasErrors || operand.HasErrors())
        {
            this.OperatorKind = operatorKind;
            this.Operand = operand;
            this.MethodOpt = methodOpt;
            this.OperandConversion = operandConversion;
            this.ResultConversion = resultConversion;
            this._ResultKind = resultKind;
            this.OriginalUserDefinedOperatorsOpt = originalUserDefinedOperatorsOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public UnaryOperatorKind OperatorKind { get; }

        public BoundExpression Operand { get; }

        public MethodSymbol? MethodOpt { get; }

        public Conversion OperandConversion { get; }

        public Conversion ResultConversion { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }

        public ImmutableArray<MethodSymbol> OriginalUserDefinedOperatorsOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitIncrementOperator(this);

        public BoundIncrementOperator Update(UnaryOperatorKind operatorKind, BoundExpression operand, MethodSymbol? methodOpt, Conversion operandConversion, Conversion resultConversion, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type)
        {
            if (operatorKind != this.OperatorKind || operand != this.Operand || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, this.MethodOpt) || operandConversion != this.OperandConversion || resultConversion != this.ResultConversion || resultKind != this.ResultKind || originalUserDefinedOperatorsOpt != this.OriginalUserDefinedOperatorsOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundIncrementOperator(this.Syntax, operatorKind, operand, methodOpt, operandConversion, resultConversion, resultKind, originalUserDefinedOperatorsOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundAddressOfOperator : BoundExpression
    {
        public BoundAddressOfOperator(SyntaxNode syntax, BoundExpression operand, bool isManaged, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.AddressOfOperator, syntax, type, hasErrors || operand.HasErrors())
        {


            this.Operand = operand;
            this.IsManaged = isManaged;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Operand { get; }

        public bool IsManaged { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitAddressOfOperator(this);

        public BoundAddressOfOperator Update(BoundExpression operand, bool isManaged, TypeSymbol type)
        {
            if (operand != this.Operand || isManaged != this.IsManaged || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundAddressOfOperator(this.Syntax, operand, isManaged, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundUnconvertedAddressOfOperator : BoundExpression
    {
        public BoundUnconvertedAddressOfOperator(SyntaxNode syntax, BoundMethodGroup operand, bool hasErrors = false)
            : base(BoundKind.UnconvertedAddressOfOperator, syntax, null, hasErrors || operand.HasErrors())
        {


            this.Operand = operand;
        }


        public BoundMethodGroup Operand { get; }

        public new TypeSymbol? Type => base.Type;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitUnconvertedAddressOfOperator(this);

        public BoundUnconvertedAddressOfOperator Update(BoundMethodGroup operand)
        {
            if (operand != this.Operand)
            {
                var result = new BoundUnconvertedAddressOfOperator(this.Syntax, operand, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundFunctionPointerLoad : BoundExpression
    {
        public BoundFunctionPointerLoad(SyntaxNode syntax, MethodSymbol targetMethod, TypeSymbol type, bool hasErrors)
            : base(BoundKind.FunctionPointerLoad, syntax, type, hasErrors)
        {


            this.TargetMethod = targetMethod;
        }

        public BoundFunctionPointerLoad(SyntaxNode syntax, MethodSymbol targetMethod, TypeSymbol type)
            : base(BoundKind.FunctionPointerLoad, syntax, type)
        {


            this.TargetMethod = targetMethod;
        }


        public MethodSymbol TargetMethod { get; }

        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitFunctionPointerLoad(this);

        public BoundFunctionPointerLoad Update(MethodSymbol targetMethod, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(targetMethod, this.TargetMethod) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundFunctionPointerLoad(this.Syntax, targetMethod, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundPointerIndirectionOperator : BoundExpression
    {
        public BoundPointerIndirectionOperator(SyntaxNode syntax, BoundExpression operand, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.PointerIndirectionOperator, syntax, type, hasErrors || operand.HasErrors())
        {


            this.Operand = operand;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Operand { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitPointerIndirectionOperator(this);

        public BoundPointerIndirectionOperator Update(BoundExpression operand, TypeSymbol type)
        {
            if (operand != this.Operand || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundPointerIndirectionOperator(this.Syntax, operand, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundPointerElementAccess : BoundExpression
    {
        public BoundPointerElementAccess(SyntaxNode syntax, BoundExpression expression, BoundExpression index, bool @checked, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.PointerElementAccess, syntax, type, hasErrors || expression.HasErrors() || index.HasErrors())
        {


            this.Expression = expression;
            this.Index = index;
            this.Checked = @checked;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Expression { get; }

        public BoundExpression Index { get; }

        public bool Checked { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitPointerElementAccess(this);

        public BoundPointerElementAccess Update(BoundExpression expression, BoundExpression index, bool @checked, TypeSymbol type)
        {
            if (expression != this.Expression || index != this.Index || @checked != this.Checked || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundPointerElementAccess(this.Syntax, expression, index, @checked, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundFunctionPointerInvocation : BoundExpression
    {
        public BoundFunctionPointerInvocation(SyntaxNode syntax, BoundExpression invokedExpression, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.FunctionPointerInvocation, syntax, type, hasErrors || invokedExpression.HasErrors() || arguments.HasErrors())
        {


            this.InvokedExpression = invokedExpression;
            this.Arguments = arguments;
            this.ArgumentRefKindsOpt = argumentRefKindsOpt;
            this._ResultKind = resultKind;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression InvokedExpression { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitFunctionPointerInvocation(this);

        public BoundFunctionPointerInvocation Update(BoundExpression invokedExpression, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt, LookupResultKind resultKind, TypeSymbol type)
        {
            if (invokedExpression != this.InvokedExpression || arguments != this.Arguments || argumentRefKindsOpt != this.ArgumentRefKindsOpt || resultKind != this.ResultKind || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundFunctionPointerInvocation(this.Syntax, invokedExpression, arguments, argumentRefKindsOpt, resultKind, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundRefTypeOperator : BoundExpression
    {
        public BoundRefTypeOperator(SyntaxNode syntax, BoundExpression operand, MethodSymbol? getTypeFromHandle, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.RefTypeOperator, syntax, type, hasErrors || operand.HasErrors())
        {


            this.Operand = operand;
            this.GetTypeFromHandle = getTypeFromHandle;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Operand { get; }

        public MethodSymbol? GetTypeFromHandle { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitRefTypeOperator(this);

        public BoundRefTypeOperator Update(BoundExpression operand, MethodSymbol? getTypeFromHandle, TypeSymbol type)
        {
            if (operand != this.Operand || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getTypeFromHandle, this.GetTypeFromHandle) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundRefTypeOperator(this.Syntax, operand, getTypeFromHandle, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundMakeRefOperator : BoundExpression
    {
        public BoundMakeRefOperator(SyntaxNode syntax, BoundExpression operand, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.MakeRefOperator, syntax, type, hasErrors || operand.HasErrors())
        {


            this.Operand = operand;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Operand { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitMakeRefOperator(this);

        public BoundMakeRefOperator Update(BoundExpression operand, TypeSymbol type)
        {
            if (operand != this.Operand || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundMakeRefOperator(this.Syntax, operand, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundRefValueOperator : BoundExpression
    {
        public BoundRefValueOperator(SyntaxNode syntax, NullableAnnotation nullableAnnotation, BoundExpression operand, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.RefValueOperator, syntax, type, hasErrors || operand.HasErrors())
        {


            this.NullableAnnotation = nullableAnnotation;
            this.Operand = operand;
        }


        public new TypeSymbol Type => base.Type!;

        public NullableAnnotation NullableAnnotation { get; }

        public BoundExpression Operand { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitRefValueOperator(this);

        public BoundRefValueOperator Update(NullableAnnotation nullableAnnotation, BoundExpression operand, TypeSymbol type)
        {
            if (nullableAnnotation != this.NullableAnnotation || operand != this.Operand || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundRefValueOperator(this.Syntax, nullableAnnotation, operand, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundFromEndIndexExpression : BoundExpression
    {
        public BoundFromEndIndexExpression(SyntaxNode syntax, BoundExpression operand, MethodSymbol? methodOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.FromEndIndexExpression, syntax, type, hasErrors || operand.HasErrors())
        {


            this.Operand = operand;
            this.MethodOpt = methodOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Operand { get; }

        public MethodSymbol? MethodOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitFromEndIndexExpression(this);

        public BoundFromEndIndexExpression Update(BoundExpression operand, MethodSymbol? methodOpt, TypeSymbol type)
        {
            if (operand != this.Operand || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, this.MethodOpt) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundFromEndIndexExpression(this.Syntax, operand, methodOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundRangeExpression : BoundExpression
    {
        public BoundRangeExpression(SyntaxNode syntax, BoundExpression? leftOperandOpt, BoundExpression? rightOperandOpt, MethodSymbol? methodOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.RangeExpression, syntax, type, hasErrors || leftOperandOpt.HasErrors() || rightOperandOpt.HasErrors())
        {


            this.LeftOperandOpt = leftOperandOpt;
            this.RightOperandOpt = rightOperandOpt;
            this.MethodOpt = methodOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression? LeftOperandOpt { get; }

        public BoundExpression? RightOperandOpt { get; }

        public MethodSymbol? MethodOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitRangeExpression(this);

        public BoundRangeExpression Update(BoundExpression? leftOperandOpt, BoundExpression? rightOperandOpt, MethodSymbol? methodOpt, TypeSymbol type)
        {
            if (leftOperandOpt != this.LeftOperandOpt || rightOperandOpt != this.RightOperandOpt || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, this.MethodOpt) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundRangeExpression(this.Syntax, leftOperandOpt, rightOperandOpt, methodOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundBinaryOperatorBase : BoundExpression
    {
        protected BoundBinaryOperatorBase(BoundKind kind, SyntaxNode syntax, BoundExpression left, BoundExpression right, TypeSymbol type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {


            this.Left = left;
            this.Right = right;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Left { get; }

        public BoundExpression Right { get; }
    }

    public sealed partial class BoundBinaryOperator : BoundBinaryOperatorBase
    {
        public BoundBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, BoundExpression left, BoundExpression right, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.BinaryOperator, syntax, left, right, type, hasErrors || left.HasErrors() || right.HasErrors())
        {


            this.OperatorKind = operatorKind;
            this.ConstantValueOpt = constantValueOpt;
            this.MethodOpt = methodOpt;
            this._ResultKind = resultKind;
            this.OriginalUserDefinedOperatorsOpt = originalUserDefinedOperatorsOpt;
        }


        public BinaryOperatorKind OperatorKind { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public MethodSymbol? MethodOpt { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }

        public ImmutableArray<MethodSymbol> OriginalUserDefinedOperatorsOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitBinaryOperator(this);

        public BoundBinaryOperator Update(BinaryOperatorKind operatorKind, ConstantValue? constantValueOpt, MethodSymbol? methodOpt, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, BoundExpression left, BoundExpression right, TypeSymbol type)
        {
            if (operatorKind != this.OperatorKind || constantValueOpt != this.ConstantValueOpt || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, this.MethodOpt) || resultKind != this.ResultKind || originalUserDefinedOperatorsOpt != this.OriginalUserDefinedOperatorsOpt || left != this.Left || right != this.Right || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundBinaryOperator(this.Syntax, operatorKind, constantValueOpt, methodOpt, resultKind, originalUserDefinedOperatorsOpt, left, right, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundTupleBinaryOperator : BoundExpression
    {
        public BoundTupleBinaryOperator(SyntaxNode syntax, BoundExpression left, BoundExpression right, BinaryOperatorKind operatorKind, TupleBinaryOperatorInfo.Multiple operators, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.TupleBinaryOperator, syntax, type, hasErrors || left.HasErrors() || right.HasErrors())
        {


            this.Left = left;
            this.Right = right;
            this.OperatorKind = operatorKind;
            this.Operators = operators;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Left { get; }

        public BoundExpression Right { get; }

        public BinaryOperatorKind OperatorKind { get; }

        public TupleBinaryOperatorInfo.Multiple Operators { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitTupleBinaryOperator(this);

        public BoundTupleBinaryOperator Update(BoundExpression left, BoundExpression right, BinaryOperatorKind operatorKind, TupleBinaryOperatorInfo.Multiple operators, TypeSymbol type)
        {
            if (left != this.Left || right != this.Right || operatorKind != this.OperatorKind || operators != this.Operators || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundTupleBinaryOperator(this.Syntax, left, right, operatorKind, operators, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundUserDefinedConditionalLogicalOperator : BoundBinaryOperatorBase
    {
        public BoundUserDefinedConditionalLogicalOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, MethodSymbol logicalOperator, MethodSymbol trueOperator, MethodSymbol falseOperator, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, BoundExpression left, BoundExpression right, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.UserDefinedConditionalLogicalOperator, syntax, left, right, type, hasErrors || left.HasErrors() || right.HasErrors())
        {


            this.OperatorKind = operatorKind;
            this.LogicalOperator = logicalOperator;
            this.TrueOperator = trueOperator;
            this.FalseOperator = falseOperator;
            this._ResultKind = resultKind;
            this.OriginalUserDefinedOperatorsOpt = originalUserDefinedOperatorsOpt;
        }


        public BinaryOperatorKind OperatorKind { get; }

        public MethodSymbol LogicalOperator { get; }

        public MethodSymbol TrueOperator { get; }

        public MethodSymbol FalseOperator { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }

        public ImmutableArray<MethodSymbol> OriginalUserDefinedOperatorsOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitUserDefinedConditionalLogicalOperator(this);

        public BoundUserDefinedConditionalLogicalOperator Update(BinaryOperatorKind operatorKind, MethodSymbol logicalOperator, MethodSymbol trueOperator, MethodSymbol falseOperator, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, BoundExpression left, BoundExpression right, TypeSymbol type)
        {
            if (operatorKind != this.OperatorKind || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(logicalOperator, this.LogicalOperator) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(trueOperator, this.TrueOperator) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(falseOperator, this.FalseOperator) || resultKind != this.ResultKind || originalUserDefinedOperatorsOpt != this.OriginalUserDefinedOperatorsOpt || left != this.Left || right != this.Right || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundUserDefinedConditionalLogicalOperator(this.Syntax, operatorKind, logicalOperator, trueOperator, falseOperator, resultKind, originalUserDefinedOperatorsOpt, left, right, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundCompoundAssignmentOperator : BoundExpression
    {
        public BoundCompoundAssignmentOperator(SyntaxNode syntax, BinaryOperatorSignature @operator, BoundExpression left, BoundExpression right, Conversion leftConversion, Conversion finalConversion, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.CompoundAssignmentOperator, syntax, type, hasErrors || left.HasErrors() || right.HasErrors())
        {


            this.Operator = @operator;
            this.Left = left;
            this.Right = right;
            this.LeftConversion = leftConversion;
            this.FinalConversion = finalConversion;
            this._ResultKind = resultKind;
            this.OriginalUserDefinedOperatorsOpt = originalUserDefinedOperatorsOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public BinaryOperatorSignature Operator { get; }

        public BoundExpression Left { get; }

        public BoundExpression Right { get; }

        public Conversion LeftConversion { get; }

        public Conversion FinalConversion { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }

        public ImmutableArray<MethodSymbol> OriginalUserDefinedOperatorsOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitCompoundAssignmentOperator(this);

        public BoundCompoundAssignmentOperator Update(BinaryOperatorSignature @operator, BoundExpression left, BoundExpression right, Conversion leftConversion, Conversion finalConversion, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt, TypeSymbol type)
        {
            if (@operator != this.Operator || left != this.Left || right != this.Right || leftConversion != this.LeftConversion || finalConversion != this.FinalConversion || resultKind != this.ResultKind || originalUserDefinedOperatorsOpt != this.OriginalUserDefinedOperatorsOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundCompoundAssignmentOperator(this.Syntax, @operator, left, right, leftConversion, finalConversion, resultKind, originalUserDefinedOperatorsOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundAssignmentOperator : BoundExpression
    {
        public BoundAssignmentOperator(SyntaxNode syntax, BoundExpression left, BoundExpression right, bool isRef, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.AssignmentOperator, syntax, type, hasErrors || left.HasErrors() || right.HasErrors())
        {


            this.Left = left;
            this.Right = right;
            this.IsRef = isRef;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Left { get; }

        public BoundExpression Right { get; }

        public bool IsRef { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitAssignmentOperator(this);

        public BoundAssignmentOperator Update(BoundExpression left, BoundExpression right, bool isRef, TypeSymbol type)
        {
            if (left != this.Left || right != this.Right || isRef != this.IsRef || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundAssignmentOperator(this.Syntax, left, right, isRef, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDeconstructionAssignmentOperator : BoundExpression
    {
        public BoundDeconstructionAssignmentOperator(SyntaxNode syntax, BoundTupleExpression left, BoundConversion right, bool isUsed, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DeconstructionAssignmentOperator, syntax, type, hasErrors || left.HasErrors() || right.HasErrors())
        {
            this.Left = left;
            this.Right = right;
            this.IsUsed = isUsed;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundTupleExpression Left { get; }

        public BoundConversion Right { get; }

        public bool IsUsed { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDeconstructionAssignmentOperator(this);

        public BoundDeconstructionAssignmentOperator Update(BoundTupleExpression left, BoundConversion right, bool isUsed, TypeSymbol type)
        {
            if (left != this.Left || right != this.Right || isUsed != this.IsUsed || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDeconstructionAssignmentOperator(this.Syntax, left, right, isUsed, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundNullCoalescingOperator : BoundExpression
    {
        public BoundNullCoalescingOperator(SyntaxNode syntax, BoundExpression leftOperand, BoundExpression rightOperand, Conversion leftConversion, BoundNullCoalescingOperatorResultKind operatorResultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.NullCoalescingOperator, syntax, type, hasErrors || leftOperand.HasErrors() || rightOperand.HasErrors())
        {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
            this.LeftConversion = leftConversion;
            this.OperatorResultKind = operatorResultKind;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression LeftOperand { get; }

        public BoundExpression RightOperand { get; }

        public Conversion LeftConversion { get; }

        public BoundNullCoalescingOperatorResultKind OperatorResultKind { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitNullCoalescingOperator(this);

        public BoundNullCoalescingOperator Update(BoundExpression leftOperand, BoundExpression rightOperand, Conversion leftConversion, BoundNullCoalescingOperatorResultKind operatorResultKind, TypeSymbol type)
        {
            if (leftOperand != this.LeftOperand || rightOperand != this.RightOperand || leftConversion != this.LeftConversion || operatorResultKind != this.OperatorResultKind || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundNullCoalescingOperator(this.Syntax, leftOperand, rightOperand, leftConversion, operatorResultKind, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundNullCoalescingAssignmentOperator : BoundExpression
    {
        public BoundNullCoalescingAssignmentOperator(SyntaxNode syntax, BoundExpression leftOperand, BoundExpression rightOperand, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.NullCoalescingAssignmentOperator, syntax, type, hasErrors || leftOperand.HasErrors() || rightOperand.HasErrors())
        {


            this.LeftOperand = leftOperand;
            this.RightOperand = rightOperand;
        }


        public BoundExpression LeftOperand { get; }

        public BoundExpression RightOperand { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitNullCoalescingAssignmentOperator(this);

        public BoundNullCoalescingAssignmentOperator Update(BoundExpression leftOperand, BoundExpression rightOperand, TypeSymbol? type)
        {
            if (leftOperand != this.LeftOperand || rightOperand != this.RightOperand || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundNullCoalescingAssignmentOperator(this.Syntax, leftOperand, rightOperand, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundUnconvertedConditionalOperator : BoundExpression
    {
        public BoundUnconvertedConditionalOperator(SyntaxNode syntax, BoundExpression condition, BoundExpression consequence, BoundExpression alternative, ConstantValue? constantValueOpt, ErrorCode noCommonTypeError, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.UnconvertedConditionalOperator, syntax, type, hasErrors || condition.HasErrors() || consequence.HasErrors() || alternative.HasErrors())
        {


            this.Condition = condition;
            this.Consequence = consequence;
            this.Alternative = alternative;
            this.ConstantValueOpt = constantValueOpt;
            this.NoCommonTypeError = noCommonTypeError;
        }


        public BoundExpression Condition { get; }

        public BoundExpression Consequence { get; }

        public BoundExpression Alternative { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public ErrorCode NoCommonTypeError { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitUnconvertedConditionalOperator(this);

        public BoundUnconvertedConditionalOperator Update(BoundExpression condition, BoundExpression consequence, BoundExpression alternative, ConstantValue? constantValueOpt, ErrorCode noCommonTypeError, TypeSymbol? type)
        {
            if (condition != this.Condition || consequence != this.Consequence || alternative != this.Alternative || constantValueOpt != this.ConstantValueOpt || noCommonTypeError != this.NoCommonTypeError || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundUnconvertedConditionalOperator(this.Syntax, condition, consequence, alternative, constantValueOpt, noCommonTypeError, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundConditionalOperator : BoundExpression
    {
        public BoundConditionalOperator(SyntaxNode syntax, bool isRef, BoundExpression condition, BoundExpression consequence, BoundExpression alternative, ConstantValue? constantValueOpt, TypeSymbol? naturalTypeOpt, bool wasTargetTyped, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ConditionalOperator, syntax, type, hasErrors || condition.HasErrors() || consequence.HasErrors() || alternative.HasErrors())
        {


            this.IsRef = isRef;
            this.Condition = condition;
            this.Consequence = consequence;
            this.Alternative = alternative;
            this.ConstantValueOpt = constantValueOpt;
            this.NaturalTypeOpt = naturalTypeOpt;
            this.WasTargetTyped = wasTargetTyped;
        }


        public new TypeSymbol Type => base.Type!;

        public bool IsRef { get; }

        public BoundExpression Condition { get; }

        public BoundExpression Consequence { get; }

        public BoundExpression Alternative { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public TypeSymbol? NaturalTypeOpt { get; }

        public bool WasTargetTyped { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitConditionalOperator(this);

        public BoundConditionalOperator Update(bool isRef, BoundExpression condition, BoundExpression consequence, BoundExpression alternative, ConstantValue? constantValueOpt, TypeSymbol? naturalTypeOpt, bool wasTargetTyped, TypeSymbol type)
        {
            if (isRef != this.IsRef || condition != this.Condition || consequence != this.Consequence || alternative != this.Alternative || constantValueOpt != this.ConstantValueOpt || !TypeSymbol.Equals(naturalTypeOpt, this.NaturalTypeOpt, TypeCompareKind.ConsiderEverything) || wasTargetTyped != this.WasTargetTyped || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundConditionalOperator(this.Syntax, isRef, condition, consequence, alternative, constantValueOpt, naturalTypeOpt, wasTargetTyped, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundArrayAccess : BoundExpression
    {
        public BoundArrayAccess(SyntaxNode syntax, BoundExpression expression, ImmutableArray<BoundExpression> indices, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ArrayAccess, syntax, type, hasErrors || expression.HasErrors() || indices.HasErrors())
        {


            this.Expression = expression;
            this.Indices = indices;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Expression { get; }

        public ImmutableArray<BoundExpression> Indices { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitArrayAccess(this);

        public BoundArrayAccess Update(BoundExpression expression, ImmutableArray<BoundExpression> indices, TypeSymbol type)
        {
            if (expression != this.Expression || indices != this.Indices || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundArrayAccess(this.Syntax, expression, indices, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundArrayLength : BoundExpression
    {
        public BoundArrayLength(SyntaxNode syntax, BoundExpression expression, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ArrayLength, syntax, type, hasErrors || expression.HasErrors())
        {


            this.Expression = expression;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Expression { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitArrayLength(this);

        public BoundArrayLength Update(BoundExpression expression, TypeSymbol type)
        {
            if (expression != this.Expression || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundArrayLength(this.Syntax, expression, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundAwaitableInfo : BoundNode
    {
        public BoundAwaitableInfo(SyntaxNode syntax, BoundAwaitableValuePlaceholder? awaitableInstancePlaceholder, bool isDynamic, BoundExpression? getAwaiter, PropertySymbol? isCompleted, MethodSymbol? getResult, bool hasErrors = false)
            : base(BoundKind.AwaitableInfo, syntax, hasErrors || awaitableInstancePlaceholder.HasErrors() || getAwaiter.HasErrors())
        {
            this.AwaitableInstancePlaceholder = awaitableInstancePlaceholder;
            this.IsDynamic = isDynamic;
            this.GetAwaiter = getAwaiter;
            this.IsCompleted = isCompleted;
            this.GetResult = getResult;
        }


        public BoundAwaitableValuePlaceholder? AwaitableInstancePlaceholder { get; }

        public bool IsDynamic { get; }

        public BoundExpression? GetAwaiter { get; }

        public PropertySymbol? IsCompleted { get; }

        public MethodSymbol? GetResult { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitAwaitableInfo(this);

        public BoundAwaitableInfo Update(BoundAwaitableValuePlaceholder? awaitableInstancePlaceholder, bool isDynamic, BoundExpression? getAwaiter, PropertySymbol? isCompleted, MethodSymbol? getResult)
        {
            if (awaitableInstancePlaceholder != this.AwaitableInstancePlaceholder || isDynamic != this.IsDynamic || getAwaiter != this.GetAwaiter || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(isCompleted, this.IsCompleted) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getResult, this.GetResult))
            {
                var result = new BoundAwaitableInfo(this.Syntax, awaitableInstancePlaceholder, isDynamic, getAwaiter, isCompleted, getResult, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundAwaitExpression : BoundExpression
    {
        public BoundAwaitExpression(SyntaxNode syntax, BoundExpression expression, BoundAwaitableInfo awaitableInfo, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.AwaitExpression, syntax, type, hasErrors || expression.HasErrors() || awaitableInfo.HasErrors())
        {


            this.Expression = expression;
            this.AwaitableInfo = awaitableInfo;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Expression { get; }

        public BoundAwaitableInfo AwaitableInfo { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitAwaitExpression(this);

        public BoundAwaitExpression Update(BoundExpression expression, BoundAwaitableInfo awaitableInfo, TypeSymbol type)
        {
            if (expression != this.Expression || awaitableInfo != this.AwaitableInfo || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundAwaitExpression(this.Syntax, expression, awaitableInfo, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundTypeOf : BoundExpression
    {
        protected BoundTypeOf(BoundKind kind, SyntaxNode syntax, MethodSymbol? getTypeFromHandle, TypeSymbol type, bool hasErrors)
            : base(kind, syntax, type, hasErrors)
        {


            this.GetTypeFromHandle = getTypeFromHandle;
        }

        protected BoundTypeOf(BoundKind kind, SyntaxNode syntax, MethodSymbol? getTypeFromHandle, TypeSymbol type)
            : base(kind, syntax, type)
        {


            this.GetTypeFromHandle = getTypeFromHandle;
        }


        public new TypeSymbol Type => base.Type!;

        public MethodSymbol? GetTypeFromHandle { get; }
    }

    public sealed partial class BoundTypeOfOperator : BoundTypeOf
    {
        public BoundTypeOfOperator(SyntaxNode syntax, BoundTypeExpression sourceType, MethodSymbol? getTypeFromHandle, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.TypeOfOperator, syntax, getTypeFromHandle, type, hasErrors || sourceType.HasErrors())
        {


            this.SourceType = sourceType;
        }


        public BoundTypeExpression SourceType { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitTypeOfOperator(this);

        public BoundTypeOfOperator Update(BoundTypeExpression sourceType, MethodSymbol? getTypeFromHandle, TypeSymbol type)
        {
            if (sourceType != this.SourceType || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getTypeFromHandle, this.GetTypeFromHandle) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundTypeOfOperator(this.Syntax, sourceType, getTypeFromHandle, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundMethodDefIndex : BoundExpression
    {
        public BoundMethodDefIndex(SyntaxNode syntax, MethodSymbol method, TypeSymbol type, bool hasErrors)
            : base(BoundKind.MethodDefIndex, syntax, type, hasErrors)
        {


            this.Method = method;
        }

        public BoundMethodDefIndex(SyntaxNode syntax, MethodSymbol method, TypeSymbol type)
            : base(BoundKind.MethodDefIndex, syntax, type)
        {


            this.Method = method;
        }


        public new TypeSymbol Type => base.Type!;

        public MethodSymbol Method { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitMethodDefIndex(this);

        public BoundMethodDefIndex Update(MethodSymbol method, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(method, this.Method) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundMethodDefIndex(this.Syntax, method, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundMaximumMethodDefIndex : BoundExpression
    {
        public BoundMaximumMethodDefIndex(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.MaximumMethodDefIndex, syntax, type, hasErrors)
        {


        }

        public BoundMaximumMethodDefIndex(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.MaximumMethodDefIndex, syntax, type)
        {


        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitMaximumMethodDefIndex(this);

        public BoundMaximumMethodDefIndex Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundMaximumMethodDefIndex(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundInstrumentationPayloadRoot : BoundExpression
    {
        public BoundInstrumentationPayloadRoot(SyntaxNode syntax, int analysisKind, TypeSymbol type, bool hasErrors)
            : base(BoundKind.InstrumentationPayloadRoot, syntax, type, hasErrors)
        {


            this.AnalysisKind = analysisKind;
        }

        public BoundInstrumentationPayloadRoot(SyntaxNode syntax, int analysisKind, TypeSymbol type)
            : base(BoundKind.InstrumentationPayloadRoot, syntax, type)
        {


            this.AnalysisKind = analysisKind;
        }


        public int AnalysisKind { get; }

        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitInstrumentationPayloadRoot(this);

        public BoundInstrumentationPayloadRoot Update(int analysisKind, TypeSymbol type)
        {
            if (analysisKind != this.AnalysisKind || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundInstrumentationPayloadRoot(this.Syntax, analysisKind, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundModuleVersionId : BoundExpression
    {
        public BoundModuleVersionId(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ModuleVersionId, syntax, type, hasErrors)
        {


        }

        public BoundModuleVersionId(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.ModuleVersionId, syntax, type)
        {


        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitModuleVersionId(this);

        public BoundModuleVersionId Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundModuleVersionId(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundModuleVersionIdString : BoundExpression
    {
        public BoundModuleVersionIdString(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ModuleVersionIdString, syntax, type, hasErrors)
        {


        }

        public BoundModuleVersionIdString(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.ModuleVersionIdString, syntax, type)
        {


        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitModuleVersionIdString(this);

        public BoundModuleVersionIdString Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundModuleVersionIdString(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSourceDocumentIndex : BoundExpression
    {
        public BoundSourceDocumentIndex(SyntaxNode syntax, Cci.DebugSourceDocument document, TypeSymbol type, bool hasErrors)
            : base(BoundKind.SourceDocumentIndex, syntax, type, hasErrors)
        {


            this.Document = document;
        }

        public BoundSourceDocumentIndex(SyntaxNode syntax, Cci.DebugSourceDocument document, TypeSymbol type)
            : base(BoundKind.SourceDocumentIndex, syntax, type)
        {


            this.Document = document;
        }


        public new TypeSymbol Type => base.Type!;

        public Cci.DebugSourceDocument Document { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSourceDocumentIndex(this);

        public BoundSourceDocumentIndex Update(Cci.DebugSourceDocument document, TypeSymbol type)
        {
            if (document != this.Document || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundSourceDocumentIndex(this.Syntax, document, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundMethodInfo : BoundExpression
    {
        public BoundMethodInfo(SyntaxNode syntax, MethodSymbol method, MethodSymbol? getMethodFromHandle, TypeSymbol type, bool hasErrors)
            : base(BoundKind.MethodInfo, syntax, type, hasErrors)
        {


            this.Method = method;
            this.GetMethodFromHandle = getMethodFromHandle;
        }

        public BoundMethodInfo(SyntaxNode syntax, MethodSymbol method, MethodSymbol? getMethodFromHandle, TypeSymbol type)
            : base(BoundKind.MethodInfo, syntax, type)
        {


            this.Method = method;
            this.GetMethodFromHandle = getMethodFromHandle;
        }


        public new TypeSymbol Type => base.Type!;

        public MethodSymbol Method { get; }

        public MethodSymbol? GetMethodFromHandle { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitMethodInfo(this);

        public BoundMethodInfo Update(MethodSymbol method, MethodSymbol? getMethodFromHandle, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(method, this.Method) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getMethodFromHandle, this.GetMethodFromHandle) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundMethodInfo(this.Syntax, method, getMethodFromHandle, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundFieldInfo : BoundExpression
    {
        public BoundFieldInfo(SyntaxNode syntax, FieldSymbol field, MethodSymbol? getFieldFromHandle, TypeSymbol type, bool hasErrors)
            : base(BoundKind.FieldInfo, syntax, type, hasErrors)
        {


            this.Field = field;
            this.GetFieldFromHandle = getFieldFromHandle;
        }

        public BoundFieldInfo(SyntaxNode syntax, FieldSymbol field, MethodSymbol? getFieldFromHandle, TypeSymbol type)
            : base(BoundKind.FieldInfo, syntax, type)
        {


            this.Field = field;
            this.GetFieldFromHandle = getFieldFromHandle;
        }


        public new TypeSymbol Type => base.Type!;

        public FieldSymbol Field { get; }

        public MethodSymbol? GetFieldFromHandle { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitFieldInfo(this);

        public BoundFieldInfo Update(FieldSymbol field, MethodSymbol? getFieldFromHandle, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(field, this.Field) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getFieldFromHandle, this.GetFieldFromHandle) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundFieldInfo(this.Syntax, field, getFieldFromHandle, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDefaultLiteral : BoundExpression
    {
        public BoundDefaultLiteral(SyntaxNode syntax, bool hasErrors)
            : base(BoundKind.DefaultLiteral, syntax, null, hasErrors)
        {
        }

        public BoundDefaultLiteral(SyntaxNode syntax)
            : base(BoundKind.DefaultLiteral, syntax, null)
        {
        }


        public new TypeSymbol? Type => base.Type;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDefaultLiteral(this);

        public BoundDefaultLiteral Update()
        {
            return this;
        }
    }

    public sealed partial class BoundDefaultExpression : BoundExpression
    {
        public BoundDefaultExpression(SyntaxNode syntax, BoundTypeExpression? targetType, ConstantValue? constantValueOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DefaultExpression, syntax, type, hasErrors || targetType.HasErrors())
        {


            this.TargetType = targetType;
            this.ConstantValueOpt = constantValueOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundTypeExpression? TargetType { get; }

        public ConstantValue? ConstantValueOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDefaultExpression(this);

        public BoundDefaultExpression Update(BoundTypeExpression? targetType, ConstantValue? constantValueOpt, TypeSymbol type)
        {
            if (targetType != this.TargetType || constantValueOpt != this.ConstantValueOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDefaultExpression(this.Syntax, targetType, constantValueOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundIsOperator : BoundExpression
    {
        public BoundIsOperator(SyntaxNode syntax, BoundExpression operand, BoundTypeExpression targetType, Conversion conversion, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.IsOperator, syntax, type, hasErrors || operand.HasErrors() || targetType.HasErrors())
        {


            this.Operand = operand;
            this.TargetType = targetType;
            this.Conversion = conversion;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Operand { get; }

        public BoundTypeExpression TargetType { get; }

        public Conversion Conversion { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitIsOperator(this);

        public BoundIsOperator Update(BoundExpression operand, BoundTypeExpression targetType, Conversion conversion, TypeSymbol type)
        {
            if (operand != this.Operand || targetType != this.TargetType || conversion != this.Conversion || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundIsOperator(this.Syntax, operand, targetType, conversion, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundAsOperator : BoundExpression
    {
        public BoundAsOperator(SyntaxNode syntax, BoundExpression operand, BoundTypeExpression targetType, Conversion conversion, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.AsOperator, syntax, type, hasErrors || operand.HasErrors() || targetType.HasErrors())
        {


            this.Operand = operand;
            this.TargetType = targetType;
            this.Conversion = conversion;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Operand { get; }

        public BoundTypeExpression TargetType { get; }

        public Conversion Conversion { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitAsOperator(this);

        public BoundAsOperator Update(BoundExpression operand, BoundTypeExpression targetType, Conversion conversion, TypeSymbol type)
        {
            if (operand != this.Operand || targetType != this.TargetType || conversion != this.Conversion || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundAsOperator(this.Syntax, operand, targetType, conversion, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSizeOfOperator : BoundExpression
    {
        public BoundSizeOfOperator(SyntaxNode syntax, BoundTypeExpression sourceType, ConstantValue? constantValueOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.SizeOfOperator, syntax, type, hasErrors || sourceType.HasErrors())
        {


            this.SourceType = sourceType;
            this.ConstantValueOpt = constantValueOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundTypeExpression SourceType { get; }

        public ConstantValue? ConstantValueOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSizeOfOperator(this);

        public BoundSizeOfOperator Update(BoundTypeExpression sourceType, ConstantValue? constantValueOpt, TypeSymbol type)
        {
            if (sourceType != this.SourceType || constantValueOpt != this.ConstantValueOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundSizeOfOperator(this.Syntax, sourceType, constantValueOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundConversion : BoundExpression
    {
        public BoundConversion(SyntaxNode syntax, BoundExpression operand, Conversion conversion, bool isBaseConversion, bool @checked, bool explicitCastInCode, ConstantValue? constantValueOpt, ConversionGroup? conversionGroupOpt, ImmutableArray<MethodSymbol> originalUserDefinedConversionsOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.Conversion, syntax, type, hasErrors || operand.HasErrors())
        {
            this.Operand = operand;
            this.Conversion = conversion;
            this.IsBaseConversion = isBaseConversion;
            this.Checked = @checked;
            this.ExplicitCastInCode = explicitCastInCode;
            this.ConstantValueOpt = constantValueOpt;
            this.ConversionGroupOpt = conversionGroupOpt;
            this.OriginalUserDefinedConversionsOpt = originalUserDefinedConversionsOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Operand { get; }

        public Conversion Conversion { get; }

        public bool IsBaseConversion { get; }

        public bool Checked { get; }

        public bool ExplicitCastInCode { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public ConversionGroup? ConversionGroupOpt { get; }

        public ImmutableArray<MethodSymbol> OriginalUserDefinedConversionsOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitConversion(this);

        public BoundConversion Update(BoundExpression operand, Conversion conversion, bool isBaseConversion, bool @checked, bool explicitCastInCode, ConstantValue? constantValueOpt, ConversionGroup? conversionGroupOpt, ImmutableArray<MethodSymbol> originalUserDefinedConversionsOpt, TypeSymbol type)
        {
            if (operand != this.Operand || conversion != this.Conversion || isBaseConversion != this.IsBaseConversion || @checked != this.Checked || explicitCastInCode != this.ExplicitCastInCode || constantValueOpt != this.ConstantValueOpt || conversionGroupOpt != this.ConversionGroupOpt || originalUserDefinedConversionsOpt != this.OriginalUserDefinedConversionsOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundConversion(this.Syntax, operand, conversion, isBaseConversion, @checked, explicitCastInCode, constantValueOpt, conversionGroupOpt, originalUserDefinedConversionsOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundReadOnlySpanFromArray : BoundExpression
    {
        public BoundReadOnlySpanFromArray(SyntaxNode syntax, BoundExpression operand, MethodSymbol conversionMethod, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ReadOnlySpanFromArray, syntax, type, hasErrors || operand.HasErrors())
        {


            this.Operand = operand;
            this.ConversionMethod = conversionMethod;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Operand { get; }

        public MethodSymbol ConversionMethod { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitReadOnlySpanFromArray(this);

        public BoundReadOnlySpanFromArray Update(BoundExpression operand, MethodSymbol conversionMethod, TypeSymbol type)
        {
            if (operand != this.Operand || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(conversionMethod, this.ConversionMethod) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundReadOnlySpanFromArray(this.Syntax, operand, conversionMethod, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundArgList : BoundExpression
    {
        public BoundArgList(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ArgList, syntax, type, hasErrors)
        {


        }

        public BoundArgList(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.ArgList, syntax, type)
        {


        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitArgList(this);

        public BoundArgList Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundArgList(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundArgListOperator : BoundExpression
    {
        public BoundArgListOperator(SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.ArgListOperator, syntax, type, hasErrors || arguments.HasErrors())
        {


            this.Arguments = arguments;
            this.ArgumentRefKindsOpt = argumentRefKindsOpt;
        }


        public new TypeSymbol? Type => base.Type;

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitArgListOperator(this);

        public BoundArgListOperator Update(ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt, TypeSymbol? type)
        {
            if (arguments != this.Arguments || argumentRefKindsOpt != this.ArgumentRefKindsOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundArgListOperator(this.Syntax, arguments, argumentRefKindsOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundFixedLocalCollectionInitializer : BoundExpression
    {
        public BoundFixedLocalCollectionInitializer(SyntaxNode syntax, TypeSymbol elementPointerType, Conversion elementPointerTypeConversion, BoundExpression expression, MethodSymbol? getPinnableOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.FixedLocalCollectionInitializer, syntax, type, hasErrors || expression.HasErrors())
        {


            this.ElementPointerType = elementPointerType;
            this.ElementPointerTypeConversion = elementPointerTypeConversion;
            this.Expression = expression;
            this.GetPinnableOpt = getPinnableOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public TypeSymbol ElementPointerType { get; }

        public Conversion ElementPointerTypeConversion { get; }

        public BoundExpression Expression { get; }

        public MethodSymbol? GetPinnableOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitFixedLocalCollectionInitializer(this);

        public BoundFixedLocalCollectionInitializer Update(TypeSymbol elementPointerType, Conversion elementPointerTypeConversion, BoundExpression expression, MethodSymbol? getPinnableOpt, TypeSymbol type)
        {
            if (!TypeSymbol.Equals(elementPointerType, this.ElementPointerType, TypeCompareKind.ConsiderEverything) || elementPointerTypeConversion != this.ElementPointerTypeConversion || expression != this.Expression || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getPinnableOpt, this.GetPinnableOpt) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundFixedLocalCollectionInitializer(this.Syntax, elementPointerType, elementPointerTypeConversion, expression, getPinnableOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundStatement : BoundNode
    {
        protected BoundStatement(BoundKind kind, SyntaxNode syntax, bool hasErrors)
            : base(kind, syntax, hasErrors)
        {
        }

        protected BoundStatement(BoundKind kind, SyntaxNode syntax)
            : base(kind, syntax)
        {
        }

    }

    public sealed partial class BoundSequencePoint : BoundStatement
    {
        public BoundSequencePoint(SyntaxNode syntax, BoundStatement? statementOpt, bool hasErrors = false)
            : base(BoundKind.SequencePoint, syntax, hasErrors || statementOpt.HasErrors())
        {
            this.StatementOpt = statementOpt;
        }


        public BoundStatement? StatementOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSequencePoint(this);

        public BoundSequencePoint Update(BoundStatement? statementOpt)
        {
            if (statementOpt != this.StatementOpt)
            {
                var result = new BoundSequencePoint(this.Syntax, statementOpt, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSequencePointWithSpan : BoundStatement
    {
        public BoundSequencePointWithSpan(SyntaxNode syntax, BoundStatement? statementOpt, TextSpan span, bool hasErrors = false)
            : base(BoundKind.SequencePointWithSpan, syntax, hasErrors || statementOpt.HasErrors())
        {
            this.StatementOpt = statementOpt;
            this.Span = span;
        }


        public BoundStatement? StatementOpt { get; }

        public TextSpan Span { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSequencePointWithSpan(this);

        public BoundSequencePointWithSpan Update(BoundStatement? statementOpt, TextSpan span)
        {
            if (statementOpt != this.StatementOpt || span != this.Span)
            {
                var result = new BoundSequencePointWithSpan(this.Syntax, statementOpt, span, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSavePreviousSequencePoint : BoundStatement
    {
        public BoundSavePreviousSequencePoint(SyntaxNode syntax, object identifier, bool hasErrors)
            : base(BoundKind.SavePreviousSequencePoint, syntax, hasErrors)
        {


            this.Identifier = identifier;
        }

        public BoundSavePreviousSequencePoint(SyntaxNode syntax, object identifier)
            : base(BoundKind.SavePreviousSequencePoint, syntax)
        {


            this.Identifier = identifier;
        }


        public object Identifier { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSavePreviousSequencePoint(this);

        public BoundSavePreviousSequencePoint Update(object identifier)
        {
            if (identifier != this.Identifier)
            {
                var result = new BoundSavePreviousSequencePoint(this.Syntax, identifier, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundRestorePreviousSequencePoint : BoundStatement
    {
        public BoundRestorePreviousSequencePoint(SyntaxNode syntax, object identifier, bool hasErrors)
            : base(BoundKind.RestorePreviousSequencePoint, syntax, hasErrors)
        {


            this.Identifier = identifier;
        }

        public BoundRestorePreviousSequencePoint(SyntaxNode syntax, object identifier)
            : base(BoundKind.RestorePreviousSequencePoint, syntax)
        {


            this.Identifier = identifier;
        }


        public object Identifier { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitRestorePreviousSequencePoint(this);

        public BoundRestorePreviousSequencePoint Update(object identifier)
        {
            if (identifier != this.Identifier)
            {
                var result = new BoundRestorePreviousSequencePoint(this.Syntax, identifier, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundStepThroughSequencePoint : BoundStatement
    {
        public BoundStepThroughSequencePoint(SyntaxNode syntax, TextSpan span, bool hasErrors)
            : base(BoundKind.StepThroughSequencePoint, syntax, hasErrors)
        {
            this.Span = span;
        }

        public BoundStepThroughSequencePoint(SyntaxNode syntax, TextSpan span)
            : base(BoundKind.StepThroughSequencePoint, syntax)
        {
            this.Span = span;
        }


        public TextSpan Span { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitStepThroughSequencePoint(this);

        public BoundStepThroughSequencePoint Update(TextSpan span)
        {
            if (span != this.Span)
            {
                var result = new BoundStepThroughSequencePoint(this.Syntax, span, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundBlock : BoundStatementList
    {
        public BoundBlock(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<LocalFunctionSymbol> localFunctions, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(BoundKind.Block, syntax, statements, hasErrors || statements.HasErrors())
        {


            this.Locals = locals;
            this.LocalFunctions = localFunctions;
        }


        public ImmutableArray<LocalSymbol> Locals { get; }

        public ImmutableArray<LocalFunctionSymbol> LocalFunctions { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitBlock(this);

        public BoundBlock Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<LocalFunctionSymbol> localFunctions, ImmutableArray<BoundStatement> statements)
        {
            if (locals != this.Locals || localFunctions != this.LocalFunctions || statements != this.Statements)
            {
                var result = new BoundBlock(this.Syntax, locals, localFunctions, statements, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundScope : BoundStatementList
    {
        public BoundScope(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(BoundKind.Scope, syntax, statements, hasErrors || statements.HasErrors())
        {


            this.Locals = locals;
        }


        public ImmutableArray<LocalSymbol> Locals { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitScope(this);

        public BoundScope Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> statements)
        {
            if (locals != this.Locals || statements != this.Statements)
            {
                var result = new BoundScope(this.Syntax, locals, statements, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundStateMachineScope : BoundStatement
    {
        public BoundStateMachineScope(SyntaxNode syntax, ImmutableArray<StateMachineFieldSymbol> fields, BoundStatement statement, bool hasErrors = false)
            : base(BoundKind.StateMachineScope, syntax, hasErrors || statement.HasErrors())
        {


            this.Fields = fields;
            this.Statement = statement;
        }


        public ImmutableArray<StateMachineFieldSymbol> Fields { get; }

        public BoundStatement Statement { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitStateMachineScope(this);

        public BoundStateMachineScope Update(ImmutableArray<StateMachineFieldSymbol> fields, BoundStatement statement)
        {
            if (fields != this.Fields || statement != this.Statement)
            {
                var result = new BoundStateMachineScope(this.Syntax, fields, statement, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLocalDeclaration : BoundStatement
    {
        public BoundLocalDeclaration(SyntaxNode syntax, LocalSymbol localSymbol, BoundTypeExpression? declaredTypeOpt, BoundExpression? initializerOpt, ImmutableArray<BoundExpression> argumentsOpt, bool inferredType, bool hasErrors = false)
            : base(BoundKind.LocalDeclaration, syntax, hasErrors || declaredTypeOpt.HasErrors() || initializerOpt.HasErrors() || argumentsOpt.HasErrors())
        {


            this.LocalSymbol = localSymbol;
            this.DeclaredTypeOpt = declaredTypeOpt;
            this.InitializerOpt = initializerOpt;
            this.ArgumentsOpt = argumentsOpt;
            this.InferredType = inferredType;
        }


        public LocalSymbol LocalSymbol { get; }

        public BoundTypeExpression? DeclaredTypeOpt { get; }

        public BoundExpression? InitializerOpt { get; }

        public ImmutableArray<BoundExpression> ArgumentsOpt { get; }

        public bool InferredType { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLocalDeclaration(this);

        public BoundLocalDeclaration Update(LocalSymbol localSymbol, BoundTypeExpression? declaredTypeOpt, BoundExpression? initializerOpt, ImmutableArray<BoundExpression> argumentsOpt, bool inferredType)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(localSymbol, this.LocalSymbol) || declaredTypeOpt != this.DeclaredTypeOpt || initializerOpt != this.InitializerOpt || argumentsOpt != this.ArgumentsOpt || inferredType != this.InferredType)
            {
                var result = new BoundLocalDeclaration(this.Syntax, localSymbol, declaredTypeOpt, initializerOpt, argumentsOpt, inferredType, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundMultipleLocalDeclarationsBase : BoundStatement
    {
        protected BoundMultipleLocalDeclarationsBase(BoundKind kind, SyntaxNode syntax, ImmutableArray<BoundLocalDeclaration> localDeclarations, bool hasErrors = false)
            : base(kind, syntax, hasErrors)
        {


            this.LocalDeclarations = localDeclarations;
        }


        public ImmutableArray<BoundLocalDeclaration> LocalDeclarations { get; }
    }

    public sealed partial class BoundMultipleLocalDeclarations : BoundMultipleLocalDeclarationsBase
    {
        public BoundMultipleLocalDeclarations(SyntaxNode syntax, ImmutableArray<BoundLocalDeclaration> localDeclarations, bool hasErrors = false)
            : base(BoundKind.MultipleLocalDeclarations, syntax, localDeclarations, hasErrors || localDeclarations.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitMultipleLocalDeclarations(this);

        public BoundMultipleLocalDeclarations Update(ImmutableArray<BoundLocalDeclaration> localDeclarations)
        {
            if (localDeclarations != this.LocalDeclarations)
            {
                var result = new BoundMultipleLocalDeclarations(this.Syntax, localDeclarations, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundUsingLocalDeclarations : BoundMultipleLocalDeclarationsBase
    {
        public BoundUsingLocalDeclarations(SyntaxNode syntax, MethodArgumentInfo? patternDisposeInfoOpt, Conversion iDisposableConversion, BoundAwaitableInfo? awaitOpt, ImmutableArray<BoundLocalDeclaration> localDeclarations, bool hasErrors = false)
            : base(BoundKind.UsingLocalDeclarations, syntax, localDeclarations, hasErrors || awaitOpt.HasErrors() || localDeclarations.HasErrors())
        {


            this.PatternDisposeInfoOpt = patternDisposeInfoOpt;
            this.IDisposableConversion = iDisposableConversion;
            this.AwaitOpt = awaitOpt;
        }


        public MethodArgumentInfo? PatternDisposeInfoOpt { get; }

        public Conversion IDisposableConversion { get; }

        public BoundAwaitableInfo? AwaitOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitUsingLocalDeclarations(this);

        public BoundUsingLocalDeclarations Update(MethodArgumentInfo? patternDisposeInfoOpt, Conversion iDisposableConversion, BoundAwaitableInfo? awaitOpt, ImmutableArray<BoundLocalDeclaration> localDeclarations)
        {
            if (patternDisposeInfoOpt != this.PatternDisposeInfoOpt || iDisposableConversion != this.IDisposableConversion || awaitOpt != this.AwaitOpt || localDeclarations != this.LocalDeclarations)
            {
                var result = new BoundUsingLocalDeclarations(this.Syntax, patternDisposeInfoOpt, iDisposableConversion, awaitOpt, localDeclarations, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLocalFunctionStatement : BoundStatement
    {
        public BoundLocalFunctionStatement(SyntaxNode syntax, LocalFunctionSymbol symbol, BoundBlock? blockBody, BoundBlock? expressionBody, bool hasErrors = false)
            : base(BoundKind.LocalFunctionStatement, syntax, hasErrors || blockBody.HasErrors() || expressionBody.HasErrors())
        {


            this.Symbol = symbol;
            this.BlockBody = blockBody;
            this.ExpressionBody = expressionBody;
        }


        public LocalFunctionSymbol Symbol { get; }

        public BoundBlock? BlockBody { get; }

        public BoundBlock? ExpressionBody { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLocalFunctionStatement(this);

        public BoundLocalFunctionStatement Update(LocalFunctionSymbol symbol, BoundBlock? blockBody, BoundBlock? expressionBody)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(symbol, this.Symbol) || blockBody != this.BlockBody || expressionBody != this.ExpressionBody)
            {
                var result = new BoundLocalFunctionStatement(this.Syntax, symbol, blockBody, expressionBody, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundNoOpStatement : BoundStatement
    {
        public BoundNoOpStatement(SyntaxNode syntax, NoOpStatementFlavor flavor, bool hasErrors)
            : base(BoundKind.NoOpStatement, syntax, hasErrors)
        {
            this.Flavor = flavor;
        }

        public BoundNoOpStatement(SyntaxNode syntax, NoOpStatementFlavor flavor)
            : base(BoundKind.NoOpStatement, syntax)
        {
            this.Flavor = flavor;
        }


        public NoOpStatementFlavor Flavor { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitNoOpStatement(this);

        public BoundNoOpStatement Update(NoOpStatementFlavor flavor)
        {
            if (flavor != this.Flavor)
            {
                var result = new BoundNoOpStatement(this.Syntax, flavor, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundReturnStatement : BoundStatement
    {
        public BoundReturnStatement(SyntaxNode syntax, RefKind refKind, BoundExpression? expressionOpt, bool hasErrors = false)
            : base(BoundKind.ReturnStatement, syntax, hasErrors || expressionOpt.HasErrors())
        {
            this.RefKind = refKind;
            this.ExpressionOpt = expressionOpt;
        }


        public RefKind RefKind { get; }

        public BoundExpression? ExpressionOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitReturnStatement(this);

        public BoundReturnStatement Update(RefKind refKind, BoundExpression? expressionOpt)
        {
            if (refKind != this.RefKind || expressionOpt != this.ExpressionOpt)
            {
                var result = new BoundReturnStatement(this.Syntax, refKind, expressionOpt, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundYieldReturnStatement : BoundStatement
    {
        public BoundYieldReturnStatement(SyntaxNode syntax, BoundExpression expression, bool hasErrors = false)
            : base(BoundKind.YieldReturnStatement, syntax, hasErrors || expression.HasErrors())
        {


            this.Expression = expression;
        }


        public BoundExpression Expression { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitYieldReturnStatement(this);

        public BoundYieldReturnStatement Update(BoundExpression expression)
        {
            if (expression != this.Expression)
            {
                var result = new BoundYieldReturnStatement(this.Syntax, expression, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundYieldBreakStatement : BoundStatement
    {
        public BoundYieldBreakStatement(SyntaxNode syntax, bool hasErrors)
            : base(BoundKind.YieldBreakStatement, syntax, hasErrors)
        {
        }

        public BoundYieldBreakStatement(SyntaxNode syntax)
            : base(BoundKind.YieldBreakStatement, syntax)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitYieldBreakStatement(this);
    }

    public sealed partial class BoundThrowStatement : BoundStatement
    {
        public BoundThrowStatement(SyntaxNode syntax, BoundExpression? expressionOpt, bool hasErrors = false)
            : base(BoundKind.ThrowStatement, syntax, hasErrors || expressionOpt.HasErrors())
        {
            this.ExpressionOpt = expressionOpt;
        }


        public BoundExpression? ExpressionOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitThrowStatement(this);

        public BoundThrowStatement Update(BoundExpression? expressionOpt)
        {
            if (expressionOpt != this.ExpressionOpt)
            {
                var result = new BoundThrowStatement(this.Syntax, expressionOpt, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(SyntaxNode syntax, BoundExpression expression, bool hasErrors = false)
            : base(BoundKind.ExpressionStatement, syntax, hasErrors || expression.HasErrors())
        {


            this.Expression = expression;
        }


        public BoundExpression Expression { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitExpressionStatement(this);

        public BoundExpressionStatement Update(BoundExpression expression)
        {
            if (expression != this.Expression)
            {
                var result = new BoundExpressionStatement(this.Syntax, expression, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundBreakStatement : BoundStatement
    {
        public BoundBreakStatement(SyntaxNode syntax, GeneratedLabelSymbol label, bool hasErrors)
            : base(BoundKind.BreakStatement, syntax, hasErrors)
        {


            this.Label = label;
        }

        public BoundBreakStatement(SyntaxNode syntax, GeneratedLabelSymbol label)
            : base(BoundKind.BreakStatement, syntax)
        {


            this.Label = label;
        }


        public GeneratedLabelSymbol Label { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitBreakStatement(this);

        public BoundBreakStatement Update(GeneratedLabelSymbol label)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, this.Label))
            {
                var result = new BoundBreakStatement(this.Syntax, label, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundContinueStatement : BoundStatement
    {
        public BoundContinueStatement(SyntaxNode syntax, GeneratedLabelSymbol label, bool hasErrors)
            : base(BoundKind.ContinueStatement, syntax, hasErrors)
        {


            this.Label = label;
        }

        public BoundContinueStatement(SyntaxNode syntax, GeneratedLabelSymbol label)
            : base(BoundKind.ContinueStatement, syntax)
        {


            this.Label = label;
        }


        public GeneratedLabelSymbol Label { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitContinueStatement(this);

        public BoundContinueStatement Update(GeneratedLabelSymbol label)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, this.Label))
            {
                var result = new BoundContinueStatement(this.Syntax, label, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSwitchStatement : BoundStatement
    {
        public BoundSwitchStatement(SyntaxNode syntax, BoundExpression expression, ImmutableArray<LocalSymbol> innerLocals, ImmutableArray<LocalFunctionSymbol> innerLocalFunctions, ImmutableArray<BoundSwitchSection> switchSections, BoundDecisionDag decisionDag, BoundSwitchLabel? defaultLabel, GeneratedLabelSymbol breakLabel, bool hasErrors = false)
            : base(BoundKind.SwitchStatement, syntax, hasErrors || expression.HasErrors() || switchSections.HasErrors() || decisionDag.HasErrors() || defaultLabel.HasErrors())
        {


            this.Expression = expression;
            this.InnerLocals = innerLocals;
            this.InnerLocalFunctions = innerLocalFunctions;
            this.SwitchSections = switchSections;
            this.DecisionDag = decisionDag;
            this.DefaultLabel = defaultLabel;
            this.BreakLabel = breakLabel;
        }


        public BoundExpression Expression { get; }

        public ImmutableArray<LocalSymbol> InnerLocals { get; }

        public ImmutableArray<LocalFunctionSymbol> InnerLocalFunctions { get; }

        public ImmutableArray<BoundSwitchSection> SwitchSections { get; }

        public BoundDecisionDag DecisionDag { get; }

        public BoundSwitchLabel? DefaultLabel { get; }

        public GeneratedLabelSymbol BreakLabel { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSwitchStatement(this);

        public BoundSwitchStatement Update(BoundExpression expression, ImmutableArray<LocalSymbol> innerLocals, ImmutableArray<LocalFunctionSymbol> innerLocalFunctions, ImmutableArray<BoundSwitchSection> switchSections, BoundDecisionDag decisionDag, BoundSwitchLabel? defaultLabel, GeneratedLabelSymbol breakLabel)
        {
            if (expression != this.Expression || innerLocals != this.InnerLocals || innerLocalFunctions != this.InnerLocalFunctions || switchSections != this.SwitchSections || decisionDag != this.DecisionDag || defaultLabel != this.DefaultLabel || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(breakLabel, this.BreakLabel))
            {
                var result = new BoundSwitchStatement(this.Syntax, expression, innerLocals, innerLocalFunctions, switchSections, decisionDag, defaultLabel, breakLabel, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSwitchDispatch : BoundStatement
    {
        public BoundSwitchDispatch(SyntaxNode syntax, BoundExpression expression, ImmutableArray<(ConstantValue value, LabelSymbol label)> cases, LabelSymbol defaultLabel, MethodSymbol? equalityMethod, bool hasErrors = false)
            : base(BoundKind.SwitchDispatch, syntax, hasErrors || expression.HasErrors())
        {


            this.Expression = expression;
            this.Cases = cases;
            this.DefaultLabel = defaultLabel;
            this.EqualityMethod = equalityMethod;
        }


        public BoundExpression Expression { get; }

        public ImmutableArray<(ConstantValue value, LabelSymbol label)> Cases { get; }

        public LabelSymbol DefaultLabel { get; }

        public MethodSymbol? EqualityMethod { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSwitchDispatch(this);

        public BoundSwitchDispatch Update(BoundExpression expression, ImmutableArray<(ConstantValue value, LabelSymbol label)> cases, LabelSymbol defaultLabel, MethodSymbol? equalityMethod)
        {
            if (expression != this.Expression || cases != this.Cases || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(defaultLabel, this.DefaultLabel) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(equalityMethod, this.EqualityMethod))
            {
                var result = new BoundSwitchDispatch(this.Syntax, expression, cases, defaultLabel, equalityMethod, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(SyntaxNode syntax, BoundExpression condition, BoundStatement consequence, BoundStatement? alternativeOpt, bool hasErrors = false)
            : base(BoundKind.IfStatement, syntax, hasErrors || condition.HasErrors() || consequence.HasErrors() || alternativeOpt.HasErrors())
        {


            this.Condition = condition;
            this.Consequence = consequence;
            this.AlternativeOpt = alternativeOpt;
        }


        public BoundExpression Condition { get; }

        public BoundStatement Consequence { get; }

        public BoundStatement? AlternativeOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitIfStatement(this);

        public BoundIfStatement Update(BoundExpression condition, BoundStatement consequence, BoundStatement? alternativeOpt)
        {
            if (condition != this.Condition || consequence != this.Consequence || alternativeOpt != this.AlternativeOpt)
            {
                var result = new BoundIfStatement(this.Syntax, condition, consequence, alternativeOpt, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundLoopStatement : BoundStatement
    {
        protected BoundLoopStatement(BoundKind kind, SyntaxNode syntax, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors)
            : base(kind, syntax, hasErrors)
        {


            this.BreakLabel = breakLabel;
            this.ContinueLabel = continueLabel;
        }

        protected BoundLoopStatement(BoundKind kind, SyntaxNode syntax, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel)
            : base(kind, syntax)
        {


            this.BreakLabel = breakLabel;
            this.ContinueLabel = continueLabel;
        }


        public GeneratedLabelSymbol BreakLabel { get; }

        public GeneratedLabelSymbol ContinueLabel { get; }
    }

    public abstract partial class BoundConditionalLoopStatement : BoundLoopStatement
    {
        protected BoundConditionalLoopStatement(BoundKind kind, SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpression condition, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors = false)
            : base(kind, syntax, breakLabel, continueLabel, hasErrors)
        {


            this.Locals = locals;
            this.Condition = condition;
            this.Body = body;
        }


        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundExpression Condition { get; }

        public BoundStatement Body { get; }
    }

    public sealed partial class BoundDoStatement : BoundConditionalLoopStatement
    {
        public BoundDoStatement(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpression condition, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors = false)
            : base(BoundKind.DoStatement, syntax, locals, condition, body, breakLabel, continueLabel, hasErrors || condition.HasErrors() || body.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDoStatement(this);

        public BoundDoStatement Update(ImmutableArray<LocalSymbol> locals, BoundExpression condition, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel)
        {
            if (locals != this.Locals || condition != this.Condition || body != this.Body || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(breakLabel, this.BreakLabel) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(continueLabel, this.ContinueLabel))
            {
                var result = new BoundDoStatement(this.Syntax, locals, condition, body, breakLabel, continueLabel, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundWhileStatement : BoundConditionalLoopStatement
    {
        public BoundWhileStatement(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpression condition, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors = false)
            : base(BoundKind.WhileStatement, syntax, locals, condition, body, breakLabel, continueLabel, hasErrors || condition.HasErrors() || body.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitWhileStatement(this);

        public BoundWhileStatement Update(ImmutableArray<LocalSymbol> locals, BoundExpression condition, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel)
        {
            if (locals != this.Locals || condition != this.Condition || body != this.Body || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(breakLabel, this.BreakLabel) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(continueLabel, this.ContinueLabel))
            {
                var result = new BoundWhileStatement(this.Syntax, locals, condition, body, breakLabel, continueLabel, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundForStatement : BoundLoopStatement
    {
        public BoundForStatement(SyntaxNode syntax, ImmutableArray<LocalSymbol> outerLocals, BoundStatement? initializer, ImmutableArray<LocalSymbol> innerLocals, BoundExpression? condition, BoundStatement? increment, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors = false)
            : base(BoundKind.ForStatement, syntax, breakLabel, continueLabel, hasErrors || initializer.HasErrors() || condition.HasErrors() || increment.HasErrors() || body.HasErrors())
        {


            this.OuterLocals = outerLocals;
            this.Initializer = initializer;
            this.InnerLocals = innerLocals;
            this.Condition = condition;
            this.Increment = increment;
            this.Body = body;
        }


        public ImmutableArray<LocalSymbol> OuterLocals { get; }

        public BoundStatement? Initializer { get; }

        public ImmutableArray<LocalSymbol> InnerLocals { get; }

        public BoundExpression? Condition { get; }

        public BoundStatement? Increment { get; }

        public BoundStatement Body { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitForStatement(this);

        public BoundForStatement Update(ImmutableArray<LocalSymbol> outerLocals, BoundStatement? initializer, ImmutableArray<LocalSymbol> innerLocals, BoundExpression? condition, BoundStatement? increment, BoundStatement body, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel)
        {
            if (outerLocals != this.OuterLocals || initializer != this.Initializer || innerLocals != this.InnerLocals || condition != this.Condition || increment != this.Increment || body != this.Body || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(breakLabel, this.BreakLabel) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(continueLabel, this.ContinueLabel))
            {
                var result = new BoundForStatement(this.Syntax, outerLocals, initializer, innerLocals, condition, increment, body, breakLabel, continueLabel, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundForEachStatement : BoundLoopStatement
    {
        public BoundForEachStatement(SyntaxNode syntax, ForEachEnumeratorInfo? enumeratorInfoOpt, Conversion elementConversion, BoundTypeExpression iterationVariableType, ImmutableArray<LocalSymbol> iterationVariables, BoundExpression? iterationErrorExpressionOpt, BoundExpression expression, BoundForEachDeconstructStep? deconstructionOpt, BoundAwaitableInfo? awaitOpt, BoundStatement body, bool @checked, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel, bool hasErrors = false)
            : base(BoundKind.ForEachStatement, syntax, breakLabel, continueLabel, hasErrors || iterationVariableType.HasErrors() || iterationErrorExpressionOpt.HasErrors() || expression.HasErrors() || deconstructionOpt.HasErrors() || awaitOpt.HasErrors() || body.HasErrors())
        {


            this.EnumeratorInfoOpt = enumeratorInfoOpt;
            this.ElementConversion = elementConversion;
            this.IterationVariableType = iterationVariableType;
            this.IterationVariables = iterationVariables;
            this.IterationErrorExpressionOpt = iterationErrorExpressionOpt;
            this.Expression = expression;
            this.DeconstructionOpt = deconstructionOpt;
            this.AwaitOpt = awaitOpt;
            this.Body = body;
            this.Checked = @checked;
        }


        public ForEachEnumeratorInfo? EnumeratorInfoOpt { get; }

        public Conversion ElementConversion { get; }

        public BoundTypeExpression IterationVariableType { get; }

        public ImmutableArray<LocalSymbol> IterationVariables { get; }

        public BoundExpression? IterationErrorExpressionOpt { get; }

        public BoundExpression Expression { get; }

        public BoundForEachDeconstructStep? DeconstructionOpt { get; }

        public BoundAwaitableInfo? AwaitOpt { get; }

        public BoundStatement Body { get; }

        public bool Checked { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitForEachStatement(this);

        public BoundForEachStatement Update(ForEachEnumeratorInfo? enumeratorInfoOpt, Conversion elementConversion, BoundTypeExpression iterationVariableType, ImmutableArray<LocalSymbol> iterationVariables, BoundExpression? iterationErrorExpressionOpt, BoundExpression expression, BoundForEachDeconstructStep? deconstructionOpt, BoundAwaitableInfo? awaitOpt, BoundStatement body, bool @checked, GeneratedLabelSymbol breakLabel, GeneratedLabelSymbol continueLabel)
        {
            if (enumeratorInfoOpt != this.EnumeratorInfoOpt || elementConversion != this.ElementConversion || iterationVariableType != this.IterationVariableType || iterationVariables != this.IterationVariables || iterationErrorExpressionOpt != this.IterationErrorExpressionOpt || expression != this.Expression || deconstructionOpt != this.DeconstructionOpt || awaitOpt != this.AwaitOpt || body != this.Body || @checked != this.Checked || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(breakLabel, this.BreakLabel) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(continueLabel, this.ContinueLabel))
            {
                var result = new BoundForEachStatement(this.Syntax, enumeratorInfoOpt, elementConversion, iterationVariableType, iterationVariables, iterationErrorExpressionOpt, expression, deconstructionOpt, awaitOpt, body, @checked, breakLabel, continueLabel, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundForEachDeconstructStep : BoundNode
    {
        public BoundForEachDeconstructStep(SyntaxNode syntax, BoundDeconstructionAssignmentOperator deconstructionAssignment, BoundDeconstructValuePlaceholder targetPlaceholder, bool hasErrors = false)
            : base(BoundKind.ForEachDeconstructStep, syntax, hasErrors || deconstructionAssignment.HasErrors() || targetPlaceholder.HasErrors())
        {


            this.DeconstructionAssignment = deconstructionAssignment;
            this.TargetPlaceholder = targetPlaceholder;
        }


        public BoundDeconstructionAssignmentOperator DeconstructionAssignment { get; }

        public BoundDeconstructValuePlaceholder TargetPlaceholder { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitForEachDeconstructStep(this);

        public BoundForEachDeconstructStep Update(BoundDeconstructionAssignmentOperator deconstructionAssignment, BoundDeconstructValuePlaceholder targetPlaceholder)
        {
            if (deconstructionAssignment != this.DeconstructionAssignment || targetPlaceholder != this.TargetPlaceholder)
            {
                var result = new BoundForEachDeconstructStep(this.Syntax, deconstructionAssignment, targetPlaceholder, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundUsingStatement : BoundStatement
    {
        public BoundUsingStatement(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundMultipleLocalDeclarations? declarationsOpt, BoundExpression? expressionOpt, Conversion iDisposableConversion, BoundStatement body, BoundAwaitableInfo? awaitOpt, MethodArgumentInfo? patternDisposeInfoOpt, bool hasErrors = false)
            : base(BoundKind.UsingStatement, syntax, hasErrors || declarationsOpt.HasErrors() || expressionOpt.HasErrors() || body.HasErrors() || awaitOpt.HasErrors())
        {


            this.Locals = locals;
            this.DeclarationsOpt = declarationsOpt;
            this.ExpressionOpt = expressionOpt;
            this.IDisposableConversion = iDisposableConversion;
            this.Body = body;
            this.AwaitOpt = awaitOpt;
            this.PatternDisposeInfoOpt = patternDisposeInfoOpt;
        }


        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundMultipleLocalDeclarations? DeclarationsOpt { get; }

        public BoundExpression? ExpressionOpt { get; }

        public Conversion IDisposableConversion { get; }

        public BoundStatement Body { get; }

        public BoundAwaitableInfo? AwaitOpt { get; }

        public MethodArgumentInfo? PatternDisposeInfoOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitUsingStatement(this);

        public BoundUsingStatement Update(ImmutableArray<LocalSymbol> locals, BoundMultipleLocalDeclarations? declarationsOpt, BoundExpression? expressionOpt, Conversion iDisposableConversion, BoundStatement body, BoundAwaitableInfo? awaitOpt, MethodArgumentInfo? patternDisposeInfoOpt)
        {
            if (locals != this.Locals || declarationsOpt != this.DeclarationsOpt || expressionOpt != this.ExpressionOpt || iDisposableConversion != this.IDisposableConversion || body != this.Body || awaitOpt != this.AwaitOpt || patternDisposeInfoOpt != this.PatternDisposeInfoOpt)
            {
                var result = new BoundUsingStatement(this.Syntax, locals, declarationsOpt, expressionOpt, iDisposableConversion, body, awaitOpt, patternDisposeInfoOpt, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundFixedStatement : BoundStatement
    {
        public BoundFixedStatement(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundMultipleLocalDeclarations declarations, BoundStatement body, bool hasErrors = false)
            : base(BoundKind.FixedStatement, syntax, hasErrors || declarations.HasErrors() || body.HasErrors())
        {


            this.Locals = locals;
            this.Declarations = declarations;
            this.Body = body;
        }


        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundMultipleLocalDeclarations Declarations { get; }

        public BoundStatement Body { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitFixedStatement(this);

        public BoundFixedStatement Update(ImmutableArray<LocalSymbol> locals, BoundMultipleLocalDeclarations declarations, BoundStatement body)
        {
            if (locals != this.Locals || declarations != this.Declarations || body != this.Body)
            {
                var result = new BoundFixedStatement(this.Syntax, locals, declarations, body, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLockStatement : BoundStatement
    {
        public BoundLockStatement(SyntaxNode syntax, BoundExpression argument, BoundStatement body, bool hasErrors = false)
            : base(BoundKind.LockStatement, syntax, hasErrors || argument.HasErrors() || body.HasErrors())
        {


            this.Argument = argument;
            this.Body = body;
        }


        public BoundExpression Argument { get; }

        public BoundStatement Body { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLockStatement(this);

        public BoundLockStatement Update(BoundExpression argument, BoundStatement body)
        {
            if (argument != this.Argument || body != this.Body)
            {
                var result = new BoundLockStatement(this.Syntax, argument, body, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundTryStatement : BoundStatement
    {
        public BoundTryStatement(SyntaxNode syntax, BoundBlock tryBlock, ImmutableArray<BoundCatchBlock> catchBlocks, BoundBlock? finallyBlockOpt, LabelSymbol? finallyLabelOpt, bool preferFaultHandler, bool hasErrors = false)
            : base(BoundKind.TryStatement, syntax, hasErrors || tryBlock.HasErrors() || catchBlocks.HasErrors() || finallyBlockOpt.HasErrors())
        {


            this.TryBlock = tryBlock;
            this.CatchBlocks = catchBlocks;
            this.FinallyBlockOpt = finallyBlockOpt;
            this.FinallyLabelOpt = finallyLabelOpt;
            this.PreferFaultHandler = preferFaultHandler;
        }


        public BoundBlock TryBlock { get; }

        public ImmutableArray<BoundCatchBlock> CatchBlocks { get; }

        public BoundBlock? FinallyBlockOpt { get; }

        public LabelSymbol? FinallyLabelOpt { get; }

        public bool PreferFaultHandler { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitTryStatement(this);

        public BoundTryStatement Update(BoundBlock tryBlock, ImmutableArray<BoundCatchBlock> catchBlocks, BoundBlock? finallyBlockOpt, LabelSymbol? finallyLabelOpt, bool preferFaultHandler)
        {
            if (tryBlock != this.TryBlock || catchBlocks != this.CatchBlocks || finallyBlockOpt != this.FinallyBlockOpt || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(finallyLabelOpt, this.FinallyLabelOpt) || preferFaultHandler != this.PreferFaultHandler)
            {
                var result = new BoundTryStatement(this.Syntax, tryBlock, catchBlocks, finallyBlockOpt, finallyLabelOpt, preferFaultHandler, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundCatchBlock : BoundNode
    {
        public BoundCatchBlock(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpression? exceptionSourceOpt, TypeSymbol? exceptionTypeOpt, BoundStatementList? exceptionFilterPrologueOpt, BoundExpression? exceptionFilterOpt, BoundBlock body, bool isSynthesizedAsyncCatchAll, bool hasErrors = false)
            : base(BoundKind.CatchBlock, syntax, hasErrors || exceptionSourceOpt.HasErrors() || exceptionFilterPrologueOpt.HasErrors() || exceptionFilterOpt.HasErrors() || body.HasErrors())
        {


            this.Locals = locals;
            this.ExceptionSourceOpt = exceptionSourceOpt;
            this.ExceptionTypeOpt = exceptionTypeOpt;
            this.ExceptionFilterPrologueOpt = exceptionFilterPrologueOpt;
            this.ExceptionFilterOpt = exceptionFilterOpt;
            this.Body = body;
            this.IsSynthesizedAsyncCatchAll = isSynthesizedAsyncCatchAll;
        }


        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundExpression? ExceptionSourceOpt { get; }

        public TypeSymbol? ExceptionTypeOpt { get; }

        public BoundStatementList? ExceptionFilterPrologueOpt { get; }

        public BoundExpression? ExceptionFilterOpt { get; }

        public BoundBlock Body { get; }

        public bool IsSynthesizedAsyncCatchAll { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitCatchBlock(this);

        public BoundCatchBlock Update(ImmutableArray<LocalSymbol> locals, BoundExpression? exceptionSourceOpt, TypeSymbol? exceptionTypeOpt, BoundStatementList? exceptionFilterPrologueOpt, BoundExpression? exceptionFilterOpt, BoundBlock body, bool isSynthesizedAsyncCatchAll)
        {
            if (locals != this.Locals || exceptionSourceOpt != this.ExceptionSourceOpt || !TypeSymbol.Equals(exceptionTypeOpt, this.ExceptionTypeOpt, TypeCompareKind.ConsiderEverything) || exceptionFilterPrologueOpt != this.ExceptionFilterPrologueOpt || exceptionFilterOpt != this.ExceptionFilterOpt || body != this.Body || isSynthesizedAsyncCatchAll != this.IsSynthesizedAsyncCatchAll)
            {
                var result = new BoundCatchBlock(this.Syntax, locals, exceptionSourceOpt, exceptionTypeOpt, exceptionFilterPrologueOpt, exceptionFilterOpt, body, isSynthesizedAsyncCatchAll, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLiteral : BoundExpression
    {
        public BoundLiteral(SyntaxNode syntax, ConstantValue? constantValueOpt, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.Literal, syntax, type, hasErrors)
        {
            this.ConstantValueOpt = constantValueOpt;
        }

        public BoundLiteral(SyntaxNode syntax, ConstantValue? constantValueOpt, TypeSymbol? type)
            : base(BoundKind.Literal, syntax, type)
        {
            this.ConstantValueOpt = constantValueOpt;
        }


        public ConstantValue? ConstantValueOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLiteral(this);

        public BoundLiteral Update(ConstantValue? constantValueOpt, TypeSymbol? type)
        {
            if (constantValueOpt != this.ConstantValueOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundLiteral(this.Syntax, constantValueOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundThisReference : BoundExpression
    {
        public BoundThisReference(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ThisReference, syntax, type, hasErrors)
        {


        }

        public BoundThisReference(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.ThisReference, syntax, type)
        {


        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitThisReference(this);

        public BoundThisReference Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundThisReference(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundPreviousSubmissionReference : BoundExpression
    {
        public BoundPreviousSubmissionReference(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.PreviousSubmissionReference, syntax, type, hasErrors)
        {


        }

        public BoundPreviousSubmissionReference(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.PreviousSubmissionReference, syntax, type)
        {


        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitPreviousSubmissionReference(this);

        public BoundPreviousSubmissionReference Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundPreviousSubmissionReference(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundHostObjectMemberReference : BoundExpression
    {
        public BoundHostObjectMemberReference(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.HostObjectMemberReference, syntax, type, hasErrors)
        {


        }

        public BoundHostObjectMemberReference(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.HostObjectMemberReference, syntax, type)
        {


        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitHostObjectMemberReference(this);

        public BoundHostObjectMemberReference Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundHostObjectMemberReference(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundBaseReference : BoundExpression
    {
        public BoundBaseReference(SyntaxNode syntax, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.BaseReference, syntax, type, hasErrors)
        {
        }

        public BoundBaseReference(SyntaxNode syntax, TypeSymbol? type)
            : base(BoundKind.BaseReference, syntax, type)
        {
        }


        public new TypeSymbol? Type => base.Type;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitBaseReference(this);

        public BoundBaseReference Update(TypeSymbol? type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundBaseReference(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLocal : BoundExpression
    {
        public BoundLocal(SyntaxNode syntax, LocalSymbol localSymbol, BoundLocalDeclarationKind declarationKind, ConstantValue? constantValueOpt, bool isNullableUnknown, TypeSymbol type, bool hasErrors)
            : base(BoundKind.Local, syntax, type, hasErrors)
        {


            this.LocalSymbol = localSymbol;
            this.DeclarationKind = declarationKind;
            this.ConstantValueOpt = constantValueOpt;
            this.IsNullableUnknown = isNullableUnknown;
        }

        public BoundLocal(SyntaxNode syntax, LocalSymbol localSymbol, BoundLocalDeclarationKind declarationKind, ConstantValue? constantValueOpt, bool isNullableUnknown, TypeSymbol type)
            : base(BoundKind.Local, syntax, type)
        {


            this.LocalSymbol = localSymbol;
            this.DeclarationKind = declarationKind;
            this.ConstantValueOpt = constantValueOpt;
            this.IsNullableUnknown = isNullableUnknown;
        }


        public new TypeSymbol Type => base.Type!;

        public LocalSymbol LocalSymbol { get; }

        public BoundLocalDeclarationKind DeclarationKind { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public bool IsNullableUnknown { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLocal(this);

        public BoundLocal Update(LocalSymbol localSymbol, BoundLocalDeclarationKind declarationKind, ConstantValue? constantValueOpt, bool isNullableUnknown, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(localSymbol, this.LocalSymbol) || declarationKind != this.DeclarationKind || constantValueOpt != this.ConstantValueOpt || isNullableUnknown != this.IsNullableUnknown || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundLocal(this.Syntax, localSymbol, declarationKind, constantValueOpt, isNullableUnknown, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundPseudoVariable : BoundExpression
    {
        public BoundPseudoVariable(SyntaxNode syntax, LocalSymbol localSymbol, PseudoVariableExpressions emitExpressions, TypeSymbol type, bool hasErrors)
            : base(BoundKind.PseudoVariable, syntax, type, hasErrors)
        {


            this.LocalSymbol = localSymbol;
            this.EmitExpressions = emitExpressions;
        }

        public BoundPseudoVariable(SyntaxNode syntax, LocalSymbol localSymbol, PseudoVariableExpressions emitExpressions, TypeSymbol type)
            : base(BoundKind.PseudoVariable, syntax, type)
        {


            this.LocalSymbol = localSymbol;
            this.EmitExpressions = emitExpressions;
        }


        public new TypeSymbol Type => base.Type!;

        public LocalSymbol LocalSymbol { get; }

        public PseudoVariableExpressions EmitExpressions { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitPseudoVariable(this);

        public BoundPseudoVariable Update(LocalSymbol localSymbol, PseudoVariableExpressions emitExpressions, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(localSymbol, this.LocalSymbol) || emitExpressions != this.EmitExpressions || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundPseudoVariable(this.Syntax, localSymbol, emitExpressions, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundRangeVariable : BoundExpression
    {
        public BoundRangeVariable(SyntaxNode syntax, RangeVariableSymbol rangeVariableSymbol, BoundExpression value, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.RangeVariable, syntax, type, hasErrors || value.HasErrors())
        {


            this.RangeVariableSymbol = rangeVariableSymbol;
            this.Value = value;
        }


        public new TypeSymbol Type => base.Type!;

        public RangeVariableSymbol RangeVariableSymbol { get; }

        public BoundExpression Value { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitRangeVariable(this);

        public BoundRangeVariable Update(RangeVariableSymbol rangeVariableSymbol, BoundExpression value, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(rangeVariableSymbol, this.RangeVariableSymbol) || value != this.Value || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundRangeVariable(this.Syntax, rangeVariableSymbol, value, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundParameter : BoundExpression
    {
        public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, TypeSymbol type, bool hasErrors)
            : base(BoundKind.Parameter, syntax, type, hasErrors)
        {


            this.ParameterSymbol = parameterSymbol;
        }

        public BoundParameter(SyntaxNode syntax, ParameterSymbol parameterSymbol, TypeSymbol type)
            : base(BoundKind.Parameter, syntax, type)
        {


            this.ParameterSymbol = parameterSymbol;
        }


        public new TypeSymbol Type => base.Type!;

        public ParameterSymbol ParameterSymbol { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitParameter(this);

        public BoundParameter Update(ParameterSymbol parameterSymbol, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(parameterSymbol, this.ParameterSymbol) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundParameter(this.Syntax, parameterSymbol, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(SyntaxNode syntax, LabelSymbol label, bool hasErrors)
            : base(BoundKind.LabelStatement, syntax, hasErrors)
        {


            this.Label = label;
        }

        public BoundLabelStatement(SyntaxNode syntax, LabelSymbol label)
            : base(BoundKind.LabelStatement, syntax)
        {


            this.Label = label;
        }


        public LabelSymbol Label { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLabelStatement(this);

        public BoundLabelStatement Update(LabelSymbol label)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, this.Label))
            {
                var result = new BoundLabelStatement(this.Syntax, label, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundGotoStatement : BoundStatement
    {
        public BoundGotoStatement(SyntaxNode syntax, LabelSymbol label, BoundExpression? caseExpressionOpt, BoundLabel? labelExpressionOpt, bool hasErrors = false)
            : base(BoundKind.GotoStatement, syntax, hasErrors || caseExpressionOpt.HasErrors() || labelExpressionOpt.HasErrors())
        {


            this.Label = label;
            this.CaseExpressionOpt = caseExpressionOpt;
            this.LabelExpressionOpt = labelExpressionOpt;
        }


        public LabelSymbol Label { get; }

        public BoundExpression? CaseExpressionOpt { get; }

        public BoundLabel? LabelExpressionOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitGotoStatement(this);

        public BoundGotoStatement Update(LabelSymbol label, BoundExpression? caseExpressionOpt, BoundLabel? labelExpressionOpt)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, this.Label) || caseExpressionOpt != this.CaseExpressionOpt || labelExpressionOpt != this.LabelExpressionOpt)
            {
                var result = new BoundGotoStatement(this.Syntax, label, caseExpressionOpt, labelExpressionOpt, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLabeledStatement : BoundStatement
    {
        public BoundLabeledStatement(SyntaxNode syntax, LabelSymbol label, BoundStatement body, bool hasErrors = false)
            : base(BoundKind.LabeledStatement, syntax, hasErrors || body.HasErrors())
        {


            this.Label = label;
            this.Body = body;
        }


        public LabelSymbol Label { get; }

        public BoundStatement Body { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLabeledStatement(this);

        public BoundLabeledStatement Update(LabelSymbol label, BoundStatement body)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, this.Label) || body != this.Body)
            {
                var result = new BoundLabeledStatement(this.Syntax, label, body, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLabel : BoundExpression
    {
        public BoundLabel(SyntaxNode syntax, LabelSymbol label, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.Label, syntax, type, hasErrors)
        {


            this.Label = label;
        }

        public BoundLabel(SyntaxNode syntax, LabelSymbol label, TypeSymbol? type)
            : base(BoundKind.Label, syntax, type)
        {


            this.Label = label;
        }


        public LabelSymbol Label { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLabel(this);

        public BoundLabel Update(LabelSymbol label, TypeSymbol? type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, this.Label) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundLabel(this.Syntax, label, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public partial class BoundStatementList : BoundStatement
    {
        protected BoundStatementList(BoundKind kind, SyntaxNode syntax, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(kind, syntax, hasErrors)
        {


            this.Statements = statements;
        }

        public BoundStatementList(SyntaxNode syntax, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(BoundKind.StatementList, syntax, hasErrors || statements.HasErrors())
        {


            this.Statements = statements;
        }


        public ImmutableArray<BoundStatement> Statements { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitStatementList(this);

        public BoundStatementList Update(ImmutableArray<BoundStatement> statements)
        {
            if (statements != this.Statements)
            {
                var result = new BoundStatementList(this.Syntax, statements, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundConditionalGoto : BoundStatement
    {
        public BoundConditionalGoto(SyntaxNode syntax, BoundExpression condition, bool jumpIfTrue, LabelSymbol label, bool hasErrors = false)
            : base(BoundKind.ConditionalGoto, syntax, hasErrors || condition.HasErrors())
        {


            this.Condition = condition;
            this.JumpIfTrue = jumpIfTrue;
            this.Label = label;
        }


        public BoundExpression Condition { get; }

        public bool JumpIfTrue { get; }

        public LabelSymbol Label { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitConditionalGoto(this);

        public BoundConditionalGoto Update(BoundExpression condition, bool jumpIfTrue, LabelSymbol label)
        {
            if (condition != this.Condition || jumpIfTrue != this.JumpIfTrue || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, this.Label))
            {
                var result = new BoundConditionalGoto(this.Syntax, condition, jumpIfTrue, label, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundSwitchExpression : BoundExpression
    {
        protected BoundSwitchExpression(BoundKind kind, SyntaxNode syntax, BoundExpression expression, ImmutableArray<BoundSwitchExpressionArm> switchArms, BoundDecisionDag decisionDag, LabelSymbol? defaultLabel, bool reportedNotExhaustive, TypeSymbol? type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {


            this.Expression = expression;
            this.SwitchArms = switchArms;
            this.DecisionDag = decisionDag;
            this.DefaultLabel = defaultLabel;
            this.ReportedNotExhaustive = reportedNotExhaustive;
        }


        public BoundExpression Expression { get; }

        public ImmutableArray<BoundSwitchExpressionArm> SwitchArms { get; }

        public BoundDecisionDag DecisionDag { get; }

        public LabelSymbol? DefaultLabel { get; }

        public bool ReportedNotExhaustive { get; }
    }

    public sealed partial class BoundSwitchExpressionArm : BoundNode
    {
        public BoundSwitchExpressionArm(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundPattern pattern, BoundExpression? whenClause, BoundExpression value, LabelSymbol label, bool hasErrors = false)
            : base(BoundKind.SwitchExpressionArm, syntax, hasErrors || pattern.HasErrors() || whenClause.HasErrors() || value.HasErrors())
        {


            this.Locals = locals;
            this.Pattern = pattern;
            this.WhenClause = whenClause;
            this.Value = value;
            this.Label = label;
        }


        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundPattern Pattern { get; }

        public BoundExpression? WhenClause { get; }

        public BoundExpression Value { get; }

        public LabelSymbol Label { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSwitchExpressionArm(this);

        public BoundSwitchExpressionArm Update(ImmutableArray<LocalSymbol> locals, BoundPattern pattern, BoundExpression? whenClause, BoundExpression value, LabelSymbol label)
        {
            if (locals != this.Locals || pattern != this.Pattern || whenClause != this.WhenClause || value != this.Value || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, this.Label))
            {
                var result = new BoundSwitchExpressionArm(this.Syntax, locals, pattern, whenClause, value, label, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundUnconvertedSwitchExpression : BoundSwitchExpression
    {
        public BoundUnconvertedSwitchExpression(SyntaxNode syntax, BoundExpression expression, ImmutableArray<BoundSwitchExpressionArm> switchArms, BoundDecisionDag decisionDag, LabelSymbol? defaultLabel, bool reportedNotExhaustive, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.UnconvertedSwitchExpression, syntax, expression, switchArms, decisionDag, defaultLabel, reportedNotExhaustive, type, hasErrors || expression.HasErrors() || switchArms.HasErrors() || decisionDag.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitUnconvertedSwitchExpression(this);

        public BoundUnconvertedSwitchExpression Update(BoundExpression expression, ImmutableArray<BoundSwitchExpressionArm> switchArms, BoundDecisionDag decisionDag, LabelSymbol? defaultLabel, bool reportedNotExhaustive, TypeSymbol? type)
        {
            if (expression != this.Expression || switchArms != this.SwitchArms || decisionDag != this.DecisionDag || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(defaultLabel, this.DefaultLabel) || reportedNotExhaustive != this.ReportedNotExhaustive || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundUnconvertedSwitchExpression(this.Syntax, expression, switchArms, decisionDag, defaultLabel, reportedNotExhaustive, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundConvertedSwitchExpression : BoundSwitchExpression
    {
        public BoundConvertedSwitchExpression(SyntaxNode syntax, TypeSymbol? naturalTypeOpt, bool wasTargetTyped, Conversion conversion, BoundExpression expression, ImmutableArray<BoundSwitchExpressionArm> switchArms, BoundDecisionDag decisionDag, LabelSymbol? defaultLabel, bool reportedNotExhaustive, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ConvertedSwitchExpression, syntax, expression, switchArms, decisionDag, defaultLabel, reportedNotExhaustive, type, hasErrors || expression.HasErrors() || switchArms.HasErrors() || decisionDag.HasErrors())
        {


            this.NaturalTypeOpt = naturalTypeOpt;
            this.WasTargetTyped = wasTargetTyped;
            this.Conversion = conversion;
        }


        public new TypeSymbol Type => base.Type!;

        public TypeSymbol? NaturalTypeOpt { get; }

        public bool WasTargetTyped { get; }

        public Conversion Conversion { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitConvertedSwitchExpression(this);

        public BoundConvertedSwitchExpression Update(TypeSymbol? naturalTypeOpt, bool wasTargetTyped, Conversion conversion, BoundExpression expression, ImmutableArray<BoundSwitchExpressionArm> switchArms, BoundDecisionDag decisionDag, LabelSymbol? defaultLabel, bool reportedNotExhaustive, TypeSymbol type)
        {
            if (!TypeSymbol.Equals(naturalTypeOpt, this.NaturalTypeOpt, TypeCompareKind.ConsiderEverything) || wasTargetTyped != this.WasTargetTyped || conversion != this.Conversion || expression != this.Expression || switchArms != this.SwitchArms || decisionDag != this.DecisionDag || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(defaultLabel, this.DefaultLabel) || reportedNotExhaustive != this.ReportedNotExhaustive || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundConvertedSwitchExpression(this.Syntax, naturalTypeOpt, wasTargetTyped, conversion, expression, switchArms, decisionDag, defaultLabel, reportedNotExhaustive, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDecisionDag : BoundNode
    {
        public BoundDecisionDag(SyntaxNode syntax, BoundDecisionDagNode rootNode, bool hasErrors = false)
            : base(BoundKind.DecisionDag, syntax, hasErrors || rootNode.HasErrors())
        {


            this.RootNode = rootNode;
        }


        public BoundDecisionDagNode RootNode { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDecisionDag(this);

        public BoundDecisionDag Update(BoundDecisionDagNode rootNode)
        {
            if (rootNode != this.RootNode)
            {
                var result = new BoundDecisionDag(this.Syntax, rootNode, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundDecisionDagNode : BoundNode
    {
        protected BoundDecisionDagNode(BoundKind kind, SyntaxNode syntax, bool hasErrors)
            : base(kind, syntax, hasErrors)
        {
        }

        protected BoundDecisionDagNode(BoundKind kind, SyntaxNode syntax)
            : base(kind, syntax)
        {
        }

    }

    public sealed partial class BoundEvaluationDecisionDagNode : BoundDecisionDagNode
    {
        public BoundEvaluationDecisionDagNode(SyntaxNode syntax, BoundDagEvaluation evaluation, BoundDecisionDagNode next, bool hasErrors = false)
            : base(BoundKind.EvaluationDecisionDagNode, syntax, hasErrors || evaluation.HasErrors() || next.HasErrors())
        {


            this.Evaluation = evaluation;
            this.Next = next;
        }


        public BoundDagEvaluation Evaluation { get; }

        public BoundDecisionDagNode Next { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitEvaluationDecisionDagNode(this);

        public BoundEvaluationDecisionDagNode Update(BoundDagEvaluation evaluation, BoundDecisionDagNode next)
        {
            if (evaluation != this.Evaluation || next != this.Next)
            {
                var result = new BoundEvaluationDecisionDagNode(this.Syntax, evaluation, next, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundTestDecisionDagNode : BoundDecisionDagNode
    {
        public BoundTestDecisionDagNode(SyntaxNode syntax, BoundDagTest test, BoundDecisionDagNode whenTrue, BoundDecisionDagNode whenFalse, bool hasErrors = false)
            : base(BoundKind.TestDecisionDagNode, syntax, hasErrors || test.HasErrors() || whenTrue.HasErrors() || whenFalse.HasErrors())
        {


            this.Test = test;
            this.WhenTrue = whenTrue;
            this.WhenFalse = whenFalse;
        }


        public BoundDagTest Test { get; }

        public BoundDecisionDagNode WhenTrue { get; }

        public BoundDecisionDagNode WhenFalse { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitTestDecisionDagNode(this);

        public BoundTestDecisionDagNode Update(BoundDagTest test, BoundDecisionDagNode whenTrue, BoundDecisionDagNode whenFalse)
        {
            if (test != this.Test || whenTrue != this.WhenTrue || whenFalse != this.WhenFalse)
            {
                var result = new BoundTestDecisionDagNode(this.Syntax, test, whenTrue, whenFalse, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundWhenDecisionDagNode : BoundDecisionDagNode
    {
        public BoundWhenDecisionDagNode(SyntaxNode syntax, ImmutableArray<BoundPatternBinding> bindings, BoundExpression? whenExpression, BoundDecisionDagNode whenTrue, BoundDecisionDagNode? whenFalse, bool hasErrors = false)
            : base(BoundKind.WhenDecisionDagNode, syntax, hasErrors || whenExpression.HasErrors() || whenTrue.HasErrors() || whenFalse.HasErrors())
        {


            this.Bindings = bindings;
            this.WhenExpression = whenExpression;
            this.WhenTrue = whenTrue;
            this.WhenFalse = whenFalse;
        }


        public ImmutableArray<BoundPatternBinding> Bindings { get; }

        public BoundExpression? WhenExpression { get; }

        public BoundDecisionDagNode WhenTrue { get; }

        public BoundDecisionDagNode? WhenFalse { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitWhenDecisionDagNode(this);

        public BoundWhenDecisionDagNode Update(ImmutableArray<BoundPatternBinding> bindings, BoundExpression? whenExpression, BoundDecisionDagNode whenTrue, BoundDecisionDagNode? whenFalse)
        {
            if (bindings != this.Bindings || whenExpression != this.WhenExpression || whenTrue != this.WhenTrue || whenFalse != this.WhenFalse)
            {
                var result = new BoundWhenDecisionDagNode(this.Syntax, bindings, whenExpression, whenTrue, whenFalse, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLeafDecisionDagNode : BoundDecisionDagNode
    {
        public BoundLeafDecisionDagNode(SyntaxNode syntax, LabelSymbol label, bool hasErrors)
            : base(BoundKind.LeafDecisionDagNode, syntax, hasErrors)
        {


            this.Label = label;
        }

        public BoundLeafDecisionDagNode(SyntaxNode syntax, LabelSymbol label)
            : base(BoundKind.LeafDecisionDagNode, syntax)
        {


            this.Label = label;
        }


        public LabelSymbol Label { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLeafDecisionDagNode(this);

        public BoundLeafDecisionDagNode Update(LabelSymbol label)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, this.Label))
            {
                var result = new BoundLeafDecisionDagNode(this.Syntax, label, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundDagTest : BoundNode
    {
        protected BoundDagTest(BoundKind kind, SyntaxNode syntax, BoundDagTemp input, bool hasErrors = false)
            : base(kind, syntax, hasErrors)
        {


            this.Input = input;
        }


        public BoundDagTemp Input { get; }
    }

    public sealed partial class BoundDagTemp : BoundNode
    {
        public BoundDagTemp(SyntaxNode syntax, TypeSymbol type, BoundDagEvaluation? source, int index, bool hasErrors = false)
            : base(BoundKind.DagTemp, syntax, hasErrors || source.HasErrors())
        {


            this.Type = type;
            this.Source = source;
            this.Index = index;
        }


        public TypeSymbol Type { get; }

        public BoundDagEvaluation? Source { get; }

        public int Index { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagTemp(this);

        public BoundDagTemp Update(TypeSymbol type, BoundDagEvaluation? source, int index)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything) || source != this.Source || index != this.Index)
            {
                var result = new BoundDagTemp(this.Syntax, type, source, index, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDagTypeTest : BoundDagTest
    {
        public BoundDagTypeTest(SyntaxNode syntax, TypeSymbol type, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagTypeTest, syntax, input, hasErrors || input.HasErrors())
        {


            this.Type = type;
        }


        public TypeSymbol Type { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagTypeTest(this);

        public BoundDagTypeTest Update(TypeSymbol type, BoundDagTemp input)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything) || input != this.Input)
            {
                var result = new BoundDagTypeTest(this.Syntax, type, input, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDagNonNullTest : BoundDagTest
    {
        public BoundDagNonNullTest(SyntaxNode syntax, bool isExplicitTest, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagNonNullTest, syntax, input, hasErrors || input.HasErrors())
        {


            this.IsExplicitTest = isExplicitTest;
        }


        public bool IsExplicitTest { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagNonNullTest(this);

        public BoundDagNonNullTest Update(bool isExplicitTest, BoundDagTemp input)
        {
            if (isExplicitTest != this.IsExplicitTest || input != this.Input)
            {
                var result = new BoundDagNonNullTest(this.Syntax, isExplicitTest, input, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDagExplicitNullTest : BoundDagTest
    {
        public BoundDagExplicitNullTest(SyntaxNode syntax, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagExplicitNullTest, syntax, input, hasErrors || input.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagExplicitNullTest(this);

        public BoundDagExplicitNullTest Update(BoundDagTemp input)
        {
            if (input != this.Input)
            {
                var result = new BoundDagExplicitNullTest(this.Syntax, input, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDagValueTest : BoundDagTest
    {
        public BoundDagValueTest(SyntaxNode syntax, ConstantValue value, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagValueTest, syntax, input, hasErrors || input.HasErrors())
        {


            this.Value = value;
        }


        public ConstantValue Value { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagValueTest(this);

        public BoundDagValueTest Update(ConstantValue value, BoundDagTemp input)
        {
            if (value != this.Value || input != this.Input)
            {
                var result = new BoundDagValueTest(this.Syntax, value, input, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDagRelationalTest : BoundDagTest
    {
        public BoundDagRelationalTest(SyntaxNode syntax, BinaryOperatorKind operatorKind, ConstantValue value, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagRelationalTest, syntax, input, hasErrors || input.HasErrors())
        {


            this.OperatorKind = operatorKind;
            this.Value = value;
        }


        public BinaryOperatorKind OperatorKind { get; }

        public ConstantValue Value { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagRelationalTest(this);

        public BoundDagRelationalTest Update(BinaryOperatorKind operatorKind, ConstantValue value, BoundDagTemp input)
        {
            if (operatorKind != this.OperatorKind || value != this.Value || input != this.Input)
            {
                var result = new BoundDagRelationalTest(this.Syntax, operatorKind, value, input, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundDagEvaluation : BoundDagTest
    {
        protected BoundDagEvaluation(BoundKind kind, SyntaxNode syntax, BoundDagTemp input, bool hasErrors = false)
            : base(kind, syntax, input, hasErrors)
        {


        }

    }

    public sealed partial class BoundDagDeconstructEvaluation : BoundDagEvaluation
    {
        public BoundDagDeconstructEvaluation(SyntaxNode syntax, MethodSymbol deconstructMethod, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagDeconstructEvaluation, syntax, input, hasErrors || input.HasErrors())
        {


            this.DeconstructMethod = deconstructMethod;
        }


        public MethodSymbol DeconstructMethod { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagDeconstructEvaluation(this);

        public BoundDagDeconstructEvaluation Update(MethodSymbol deconstructMethod, BoundDagTemp input)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(deconstructMethod, this.DeconstructMethod) || input != this.Input)
            {
                var result = new BoundDagDeconstructEvaluation(this.Syntax, deconstructMethod, input, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDagTypeEvaluation : BoundDagEvaluation
    {
        public BoundDagTypeEvaluation(SyntaxNode syntax, TypeSymbol type, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagTypeEvaluation, syntax, input, hasErrors || input.HasErrors())
        {


            this.Type = type;
        }


        public TypeSymbol Type { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagTypeEvaluation(this);

        public BoundDagTypeEvaluation Update(TypeSymbol type, BoundDagTemp input)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything) || input != this.Input)
            {
                var result = new BoundDagTypeEvaluation(this.Syntax, type, input, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDagFieldEvaluation : BoundDagEvaluation
    {
        public BoundDagFieldEvaluation(SyntaxNode syntax, FieldSymbol field, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagFieldEvaluation, syntax, input, hasErrors || input.HasErrors())
        {


            this.Field = field;
        }


        public FieldSymbol Field { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagFieldEvaluation(this);

        public BoundDagFieldEvaluation Update(FieldSymbol field, BoundDagTemp input)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(field, this.Field) || input != this.Input)
            {
                var result = new BoundDagFieldEvaluation(this.Syntax, field, input, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDagPropertyEvaluation : BoundDagEvaluation
    {
        public BoundDagPropertyEvaluation(SyntaxNode syntax, PropertySymbol property, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagPropertyEvaluation, syntax, input, hasErrors || input.HasErrors())
        {


            this.Property = property;
        }


        public PropertySymbol Property { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagPropertyEvaluation(this);

        public BoundDagPropertyEvaluation Update(PropertySymbol property, BoundDagTemp input)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(property, this.Property) || input != this.Input)
            {
                var result = new BoundDagPropertyEvaluation(this.Syntax, property, input, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDagIndexEvaluation : BoundDagEvaluation
    {
        public BoundDagIndexEvaluation(SyntaxNode syntax, PropertySymbol property, int index, BoundDagTemp input, bool hasErrors = false)
            : base(BoundKind.DagIndexEvaluation, syntax, input, hasErrors || input.HasErrors())
        {


            this.Property = property;
            this.Index = index;
        }


        public PropertySymbol Property { get; }

        public int Index { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDagIndexEvaluation(this);

        public BoundDagIndexEvaluation Update(PropertySymbol property, int index, BoundDagTemp input)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(property, this.Property) || index != this.Index || input != this.Input)
            {
                var result = new BoundDagIndexEvaluation(this.Syntax, property, index, input, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSwitchSection : BoundStatementList
    {
        public BoundSwitchSection(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundSwitchLabel> switchLabels, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(BoundKind.SwitchSection, syntax, statements, hasErrors || switchLabels.HasErrors() || statements.HasErrors())
        {


            this.Locals = locals;
            this.SwitchLabels = switchLabels;
        }


        public ImmutableArray<LocalSymbol> Locals { get; }

        public ImmutableArray<BoundSwitchLabel> SwitchLabels { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSwitchSection(this);

        public BoundSwitchSection Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundSwitchLabel> switchLabels, ImmutableArray<BoundStatement> statements)
        {
            if (locals != this.Locals || switchLabels != this.SwitchLabels || statements != this.Statements)
            {
                var result = new BoundSwitchSection(this.Syntax, locals, switchLabels, statements, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSwitchLabel : BoundNode
    {
        public BoundSwitchLabel(SyntaxNode syntax, LabelSymbol label, BoundPattern pattern, BoundExpression? whenClause, bool hasErrors = false)
            : base(BoundKind.SwitchLabel, syntax, hasErrors || pattern.HasErrors() || whenClause.HasErrors())
        {


            this.Label = label;
            this.Pattern = pattern;
            this.WhenClause = whenClause;
        }


        public LabelSymbol Label { get; }

        public BoundPattern Pattern { get; }

        public BoundExpression? WhenClause { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSwitchLabel(this);

        public BoundSwitchLabel Update(LabelSymbol label, BoundPattern pattern, BoundExpression? whenClause)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, this.Label) || pattern != this.Pattern || whenClause != this.WhenClause)
            {
                var result = new BoundSwitchLabel(this.Syntax, label, pattern, whenClause, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundMethodOrPropertyGroup : BoundExpression
    {
        protected BoundMethodOrPropertyGroup(BoundKind kind, SyntaxNode syntax, BoundExpression? receiverOpt, LookupResultKind resultKind, bool hasErrors = false)
            : base(kind, syntax, null, hasErrors)
        {
            this.ReceiverOpt = receiverOpt;
            this._ResultKind = resultKind;
        }


        public new TypeSymbol? Type => base.Type;

        public BoundExpression? ReceiverOpt { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }
    }

    public sealed partial class BoundSequencePointExpression : BoundExpression
    {
        public BoundSequencePointExpression(SyntaxNode syntax, BoundExpression expression, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.SequencePointExpression, syntax, type, hasErrors || expression.HasErrors())
        {


            this.Expression = expression;
        }


        public BoundExpression Expression { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSequencePointExpression(this);

        public BoundSequencePointExpression Update(BoundExpression expression, TypeSymbol? type)
        {
            if (expression != this.Expression || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundSequencePointExpression(this.Syntax, expression, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSequence : BoundExpression
    {
        public BoundSequence(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> sideEffects, BoundExpression value, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.Sequence, syntax, type, hasErrors || sideEffects.HasErrors() || value.HasErrors())
        {


            this.Locals = locals;
            this.SideEffects = sideEffects;
            this.Value = value;
        }


        public new TypeSymbol Type => base.Type!;

        public ImmutableArray<LocalSymbol> Locals { get; }

        public ImmutableArray<BoundExpression> SideEffects { get; }

        public BoundExpression Value { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSequence(this);

        public BoundSequence Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> sideEffects, BoundExpression value, TypeSymbol type)
        {
            if (locals != this.Locals || sideEffects != this.SideEffects || value != this.Value || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundSequence(this.Syntax, locals, sideEffects, value, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSpillSequence : BoundExpression
    {
        public BoundSpillSequence(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> sideEffects, BoundExpression value, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.SpillSequence, syntax, type, hasErrors || sideEffects.HasErrors() || value.HasErrors())
        {


            this.Locals = locals;
            this.SideEffects = sideEffects;
            this.Value = value;
        }


        public new TypeSymbol Type => base.Type!;

        public ImmutableArray<LocalSymbol> Locals { get; }

        public ImmutableArray<BoundStatement> SideEffects { get; }

        public BoundExpression Value { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSpillSequence(this);

        public BoundSpillSequence Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> sideEffects, BoundExpression value, TypeSymbol type)
        {
            if (locals != this.Locals || sideEffects != this.SideEffects || value != this.Value || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundSpillSequence(this.Syntax, locals, sideEffects, value, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDynamicMemberAccess : BoundExpression
    {
        public BoundDynamicMemberAccess(SyntaxNode syntax, BoundExpression receiver, ImmutableArray<TypeWithAnnotations> typeArgumentsOpt, string name, bool invoked, bool indexed, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DynamicMemberAccess, syntax, type, hasErrors || receiver.HasErrors())
        {


            this.Receiver = receiver;
            this.TypeArgumentsOpt = typeArgumentsOpt;
            this.Name = name;
            this.Invoked = invoked;
            this.Indexed = indexed;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Receiver { get; }

        public ImmutableArray<TypeWithAnnotations> TypeArgumentsOpt { get; }

        public string Name { get; }

        public bool Invoked { get; }

        public bool Indexed { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDynamicMemberAccess(this);

        public BoundDynamicMemberAccess Update(BoundExpression receiver, ImmutableArray<TypeWithAnnotations> typeArgumentsOpt, string name, bool invoked, bool indexed, TypeSymbol type)
        {
            if (receiver != this.Receiver || typeArgumentsOpt != this.TypeArgumentsOpt || name != this.Name || invoked != this.Invoked || indexed != this.Indexed || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDynamicMemberAccess(this.Syntax, receiver, typeArgumentsOpt, name, invoked, indexed, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundDynamicInvocableBase : BoundExpression
    {
        protected BoundDynamicInvocableBase(BoundKind kind, SyntaxNode syntax, BoundExpression expression, ImmutableArray<BoundExpression> arguments, TypeSymbol? type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {


            this.Expression = expression;
            this.Arguments = arguments;
        }


        public BoundExpression Expression { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }
    }

    public sealed partial class BoundDynamicInvocation : BoundDynamicInvocableBase
    {
        public BoundDynamicInvocation(SyntaxNode syntax, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<MethodSymbol> applicableMethods, BoundExpression expression, ImmutableArray<BoundExpression> arguments, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DynamicInvocation, syntax, expression, arguments, type, hasErrors || expression.HasErrors() || arguments.HasErrors())
        {


            this.ArgumentNamesOpt = argumentNamesOpt;
            this.ArgumentRefKindsOpt = argumentRefKindsOpt;
            this.ApplicableMethods = applicableMethods;
        }


        public new TypeSymbol Type => base.Type!;

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public ImmutableArray<MethodSymbol> ApplicableMethods { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDynamicInvocation(this);

        public BoundDynamicInvocation Update(ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<MethodSymbol> applicableMethods, BoundExpression expression, ImmutableArray<BoundExpression> arguments, TypeSymbol type)
        {
            if (argumentNamesOpt != this.ArgumentNamesOpt || argumentRefKindsOpt != this.ArgumentRefKindsOpt || applicableMethods != this.ApplicableMethods || expression != this.Expression || arguments != this.Arguments || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDynamicInvocation(this.Syntax, argumentNamesOpt, argumentRefKindsOpt, applicableMethods, expression, arguments, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundConditionalAccess : BoundExpression
    {
        public BoundConditionalAccess(SyntaxNode syntax, BoundExpression receiver, BoundExpression accessExpression, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ConditionalAccess, syntax, type, hasErrors || receiver.HasErrors() || accessExpression.HasErrors())
        {


            this.Receiver = receiver;
            this.AccessExpression = accessExpression;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Receiver { get; }

        public BoundExpression AccessExpression { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitConditionalAccess(this);

        public BoundConditionalAccess Update(BoundExpression receiver, BoundExpression accessExpression, TypeSymbol type)
        {
            if (receiver != this.Receiver || accessExpression != this.AccessExpression || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundConditionalAccess(this.Syntax, receiver, accessExpression, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLoweredConditionalAccess : BoundExpression
    {
        public BoundLoweredConditionalAccess(SyntaxNode syntax, BoundExpression receiver, MethodSymbol? hasValueMethodOpt, BoundExpression whenNotNull, BoundExpression? whenNullOpt, int id, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.LoweredConditionalAccess, syntax, type, hasErrors || receiver.HasErrors() || whenNotNull.HasErrors() || whenNullOpt.HasErrors())
        {


            this.Receiver = receiver;
            this.HasValueMethodOpt = hasValueMethodOpt;
            this.WhenNotNull = whenNotNull;
            this.WhenNullOpt = whenNullOpt;
            this.Id = id;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Receiver { get; }

        public MethodSymbol? HasValueMethodOpt { get; }

        public BoundExpression WhenNotNull { get; }

        public BoundExpression? WhenNullOpt { get; }

        public int Id { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLoweredConditionalAccess(this);

        public BoundLoweredConditionalAccess Update(BoundExpression receiver, MethodSymbol? hasValueMethodOpt, BoundExpression whenNotNull, BoundExpression? whenNullOpt, int id, TypeSymbol type)
        {
            if (receiver != this.Receiver || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(hasValueMethodOpt, this.HasValueMethodOpt) || whenNotNull != this.WhenNotNull || whenNullOpt != this.WhenNullOpt || id != this.Id || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundLoweredConditionalAccess(this.Syntax, receiver, hasValueMethodOpt, whenNotNull, whenNullOpt, id, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundConditionalReceiver : BoundExpression
    {
        public BoundConditionalReceiver(SyntaxNode syntax, int id, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ConditionalReceiver, syntax, type, hasErrors)
        {


            this.Id = id;
        }

        public BoundConditionalReceiver(SyntaxNode syntax, int id, TypeSymbol type)
            : base(BoundKind.ConditionalReceiver, syntax, type)
        {


            this.Id = id;
        }


        public new TypeSymbol Type => base.Type!;

        public int Id { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitConditionalReceiver(this);

        public BoundConditionalReceiver Update(int id, TypeSymbol type)
        {
            if (id != this.Id || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundConditionalReceiver(this.Syntax, id, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundComplexConditionalReceiver : BoundExpression
    {
        public BoundComplexConditionalReceiver(SyntaxNode syntax, BoundExpression valueTypeReceiver, BoundExpression referenceTypeReceiver, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ComplexConditionalReceiver, syntax, type, hasErrors || valueTypeReceiver.HasErrors() || referenceTypeReceiver.HasErrors())
        {


            this.ValueTypeReceiver = valueTypeReceiver;
            this.ReferenceTypeReceiver = referenceTypeReceiver;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression ValueTypeReceiver { get; }

        public BoundExpression ReferenceTypeReceiver { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitComplexConditionalReceiver(this);

        public BoundComplexConditionalReceiver Update(BoundExpression valueTypeReceiver, BoundExpression referenceTypeReceiver, TypeSymbol type)
        {
            if (valueTypeReceiver != this.ValueTypeReceiver || referenceTypeReceiver != this.ReferenceTypeReceiver || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundComplexConditionalReceiver(this.Syntax, valueTypeReceiver, referenceTypeReceiver, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundMethodGroup : BoundMethodOrPropertyGroup
    {
        public BoundMethodGroup(SyntaxNode syntax, ImmutableArray<TypeWithAnnotations> typeArgumentsOpt, string name, ImmutableArray<MethodSymbol> methods, Symbol? lookupSymbolOpt, DiagnosticInfo? lookupError, BoundMethodGroupFlags? flags, BoundExpression? receiverOpt, LookupResultKind resultKind, bool hasErrors = false)
            : base(BoundKind.MethodGroup, syntax, receiverOpt, resultKind, hasErrors || receiverOpt.HasErrors())
        {


            this.TypeArgumentsOpt = typeArgumentsOpt;
            this.Name = name;
            this.Methods = methods;
            this.LookupSymbolOpt = lookupSymbolOpt;
            this.LookupError = lookupError;
            this.Flags = flags;
        }


        public ImmutableArray<TypeWithAnnotations> TypeArgumentsOpt { get; }

        public string Name { get; }

        public ImmutableArray<MethodSymbol> Methods { get; }

        public Symbol? LookupSymbolOpt { get; }

        public DiagnosticInfo? LookupError { get; }

        public BoundMethodGroupFlags? Flags { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitMethodGroup(this);

        public BoundMethodGroup Update(ImmutableArray<TypeWithAnnotations> typeArgumentsOpt, string name, ImmutableArray<MethodSymbol> methods, Symbol? lookupSymbolOpt, DiagnosticInfo? lookupError, BoundMethodGroupFlags? flags, BoundExpression? receiverOpt, LookupResultKind resultKind)
        {
            if (typeArgumentsOpt != this.TypeArgumentsOpt || name != this.Name || methods != this.Methods || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(lookupSymbolOpt, this.LookupSymbolOpt) || lookupError != this.LookupError || flags != this.Flags || receiverOpt != this.ReceiverOpt || resultKind != this.ResultKind)
            {
                var result = new BoundMethodGroup(this.Syntax, typeArgumentsOpt, name, methods, lookupSymbolOpt, lookupError, flags, receiverOpt, resultKind, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundPropertyGroup : BoundMethodOrPropertyGroup
    {
        public BoundPropertyGroup(SyntaxNode syntax, ImmutableArray<PropertySymbol> properties, BoundExpression? receiverOpt, LookupResultKind resultKind, bool hasErrors = false)
            : base(BoundKind.PropertyGroup, syntax, receiverOpt, resultKind, hasErrors || receiverOpt.HasErrors())
        {


            this.Properties = properties;
        }


        public ImmutableArray<PropertySymbol> Properties { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitPropertyGroup(this);

        public BoundPropertyGroup Update(ImmutableArray<PropertySymbol> properties, BoundExpression? receiverOpt, LookupResultKind resultKind)
        {
            if (properties != this.Properties || receiverOpt != this.ReceiverOpt || resultKind != this.ResultKind)
            {
                var result = new BoundPropertyGroup(this.Syntax, properties, receiverOpt, resultKind, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundCall : BoundExpression
    {
        public BoundCall(SyntaxNode syntax, BoundExpression? receiverOpt, MethodSymbol method, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool isDelegateCall, bool expanded, bool invokedAsExtensionMethod, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalMethodsOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.Call, syntax, type, hasErrors || receiverOpt.HasErrors() || arguments.HasErrors())
        {


            this.ReceiverOpt = receiverOpt;
            this.Method = method;
            this.Arguments = arguments;
            this.ArgumentNamesOpt = argumentNamesOpt;
            this.ArgumentRefKindsOpt = argumentRefKindsOpt;
            this.IsDelegateCall = isDelegateCall;
            this.Expanded = expanded;
            this.InvokedAsExtensionMethod = invokedAsExtensionMethod;
            this.ArgsToParamsOpt = argsToParamsOpt;
            this.DefaultArguments = defaultArguments;
            this._ResultKind = resultKind;
            this.OriginalMethodsOpt = originalMethodsOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression? ReceiverOpt { get; }

        public MethodSymbol Method { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public bool IsDelegateCall { get; }

        public bool Expanded { get; }

        public bool InvokedAsExtensionMethod { get; }

        public ImmutableArray<int> ArgsToParamsOpt { get; }

        public BitVector DefaultArguments { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }

        public ImmutableArray<MethodSymbol> OriginalMethodsOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitCall(this);

        public BoundCall Update(BoundExpression? receiverOpt, MethodSymbol method, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool isDelegateCall, bool expanded, bool invokedAsExtensionMethod, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalMethodsOpt, TypeSymbol type)
        {
            if (receiverOpt != this.ReceiverOpt || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(method, this.Method) || arguments != this.Arguments || argumentNamesOpt != this.ArgumentNamesOpt || argumentRefKindsOpt != this.ArgumentRefKindsOpt || isDelegateCall != this.IsDelegateCall || expanded != this.Expanded || invokedAsExtensionMethod != this.InvokedAsExtensionMethod || argsToParamsOpt != this.ArgsToParamsOpt || defaultArguments != this.DefaultArguments || resultKind != this.ResultKind || originalMethodsOpt != this.OriginalMethodsOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundCall(this.Syntax, receiverOpt, method, arguments, argumentNamesOpt, argumentRefKindsOpt, isDelegateCall, expanded, invokedAsExtensionMethod, argsToParamsOpt, defaultArguments, resultKind, originalMethodsOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundEventAssignmentOperator : BoundExpression
    {
        public BoundEventAssignmentOperator(SyntaxNode syntax, EventSymbol @event, bool isAddition, bool isDynamic, BoundExpression? receiverOpt, BoundExpression argument, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.EventAssignmentOperator, syntax, type, hasErrors || receiverOpt.HasErrors() || argument.HasErrors())
        {


            this.Event = @event;
            this.IsAddition = isAddition;
            this.IsDynamic = isDynamic;
            this.ReceiverOpt = receiverOpt;
            this.Argument = argument;
        }


        public new TypeSymbol Type => base.Type!;

        public EventSymbol Event { get; }

        public bool IsAddition { get; }

        public bool IsDynamic { get; }

        public BoundExpression? ReceiverOpt { get; }

        public BoundExpression Argument { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitEventAssignmentOperator(this);

        public BoundEventAssignmentOperator Update(EventSymbol @event, bool isAddition, bool isDynamic, BoundExpression? receiverOpt, BoundExpression argument, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(@event, this.Event) || isAddition != this.IsAddition || isDynamic != this.IsDynamic || receiverOpt != this.ReceiverOpt || argument != this.Argument || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundEventAssignmentOperator(this.Syntax, @event, isAddition, isDynamic, receiverOpt, argument, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundAttribute : BoundExpression
    {
        public BoundAttribute(SyntaxNode syntax, MethodSymbol? constructor, ImmutableArray<BoundExpression> constructorArguments, ImmutableArray<string> constructorArgumentNamesOpt, ImmutableArray<int> constructorArgumentsToParamsOpt, bool constructorExpanded, ImmutableArray<BoundAssignmentOperator> namedArguments, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.Attribute, syntax, type, hasErrors || constructorArguments.HasErrors() || namedArguments.HasErrors())
        {


            this.Constructor = constructor;
            this.ConstructorArguments = constructorArguments;
            this.ConstructorArgumentNamesOpt = constructorArgumentNamesOpt;
            this.ConstructorArgumentsToParamsOpt = constructorArgumentsToParamsOpt;
            this.ConstructorExpanded = constructorExpanded;
            this.NamedArguments = namedArguments;
            this._ResultKind = resultKind;
        }


        public new TypeSymbol Type => base.Type!;

        public MethodSymbol? Constructor { get; }

        public ImmutableArray<BoundExpression> ConstructorArguments { get; }

        public ImmutableArray<string> ConstructorArgumentNamesOpt { get; }

        public ImmutableArray<int> ConstructorArgumentsToParamsOpt { get; }

        public bool ConstructorExpanded { get; }

        public ImmutableArray<BoundAssignmentOperator> NamedArguments { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitAttribute(this);

        public BoundAttribute Update(MethodSymbol? constructor, ImmutableArray<BoundExpression> constructorArguments, ImmutableArray<string> constructorArgumentNamesOpt, ImmutableArray<int> constructorArgumentsToParamsOpt, bool constructorExpanded, ImmutableArray<BoundAssignmentOperator> namedArguments, LookupResultKind resultKind, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(constructor, this.Constructor) || constructorArguments != this.ConstructorArguments || constructorArgumentNamesOpt != this.ConstructorArgumentNamesOpt || constructorArgumentsToParamsOpt != this.ConstructorArgumentsToParamsOpt || constructorExpanded != this.ConstructorExpanded || namedArguments != this.NamedArguments || resultKind != this.ResultKind || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundAttribute(this.Syntax, constructor, constructorArguments, constructorArgumentNamesOpt, constructorArgumentsToParamsOpt, constructorExpanded, namedArguments, resultKind, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundUnconvertedObjectCreationExpression : BoundExpression
    {
        public BoundUnconvertedObjectCreationExpression(SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, ImmutableArray<IdentifierNameSyntax> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, InitializerExpressionSyntax? initializerOpt, bool hasErrors = false)
            : base(BoundKind.UnconvertedObjectCreationExpression, syntax, null, hasErrors || arguments.HasErrors())
        {
            this.Arguments = arguments;
            this.ArgumentNamesOpt = argumentNamesOpt;
            this.ArgumentRefKindsOpt = argumentRefKindsOpt;
            this.InitializerOpt = initializerOpt;
        }


        public new TypeSymbol? Type => base.Type;

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<IdentifierNameSyntax> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public InitializerExpressionSyntax? InitializerOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitUnconvertedObjectCreationExpression(this);

        public BoundUnconvertedObjectCreationExpression Update(ImmutableArray<BoundExpression> arguments, ImmutableArray<IdentifierNameSyntax> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, InitializerExpressionSyntax? initializerOpt)
        {
            if (arguments != this.Arguments || argumentNamesOpt != this.ArgumentNamesOpt || argumentRefKindsOpt != this.ArgumentRefKindsOpt || initializerOpt != this.InitializerOpt)
            {
                var result = new BoundUnconvertedObjectCreationExpression(this.Syntax, arguments, argumentNamesOpt, argumentRefKindsOpt, initializerOpt, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundObjectCreationExpression : BoundExpression
    {
        public BoundObjectCreationExpression(SyntaxNode syntax, MethodSymbol constructor, ImmutableArray<MethodSymbol> constructorsGroup, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ConstantValue? constantValueOpt, BoundObjectInitializerExpressionBase? initializerExpressionOpt, bool wasTargetTyped, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ObjectCreationExpression, syntax, type, hasErrors || arguments.HasErrors() || initializerExpressionOpt.HasErrors())
        {


            this.Constructor = constructor;
            this.ConstructorsGroup = constructorsGroup;
            this.Arguments = arguments;
            this.ArgumentNamesOpt = argumentNamesOpt;
            this.ArgumentRefKindsOpt = argumentRefKindsOpt;
            this.Expanded = expanded;
            this.ArgsToParamsOpt = argsToParamsOpt;
            this.DefaultArguments = defaultArguments;
            this.ConstantValueOpt = constantValueOpt;
            this.InitializerExpressionOpt = initializerExpressionOpt;
            this.WasTargetTyped = wasTargetTyped;
        }


        public new TypeSymbol Type => base.Type!;

        public MethodSymbol Constructor { get; }

        public ImmutableArray<MethodSymbol> ConstructorsGroup { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public bool Expanded { get; }

        public ImmutableArray<int> ArgsToParamsOpt { get; }

        public BitVector DefaultArguments { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public BoundObjectInitializerExpressionBase? InitializerExpressionOpt { get; }

        public bool WasTargetTyped { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitObjectCreationExpression(this);

        public BoundObjectCreationExpression Update(MethodSymbol constructor, ImmutableArray<MethodSymbol> constructorsGroup, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ConstantValue? constantValueOpt, BoundObjectInitializerExpressionBase? initializerExpressionOpt, bool wasTargetTyped, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(constructor, this.Constructor) || constructorsGroup != this.ConstructorsGroup || arguments != this.Arguments || argumentNamesOpt != this.ArgumentNamesOpt || argumentRefKindsOpt != this.ArgumentRefKindsOpt || expanded != this.Expanded || argsToParamsOpt != this.ArgsToParamsOpt || defaultArguments != this.DefaultArguments || constantValueOpt != this.ConstantValueOpt || initializerExpressionOpt != this.InitializerExpressionOpt || wasTargetTyped != this.WasTargetTyped || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundObjectCreationExpression(this.Syntax, constructor, constructorsGroup, arguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, constantValueOpt, initializerExpressionOpt, wasTargetTyped, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundTupleExpression : BoundExpression
    {
        protected BoundTupleExpression(BoundKind kind, SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, ImmutableArray<string?> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, TypeSymbol? type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {


            this.Arguments = arguments;
            this.ArgumentNamesOpt = argumentNamesOpt;
            this.InferredNamesOpt = inferredNamesOpt;
        }


        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string?> ArgumentNamesOpt { get; }

        public ImmutableArray<bool> InferredNamesOpt { get; }
    }

    public sealed partial class BoundTupleLiteral : BoundTupleExpression
    {
        public BoundTupleLiteral(SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, ImmutableArray<string?> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.TupleLiteral, syntax, arguments, argumentNamesOpt, inferredNamesOpt, type, hasErrors || arguments.HasErrors())
        {


        }


        public new TypeSymbol? Type => base.Type;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitTupleLiteral(this);

        public BoundTupleLiteral Update(ImmutableArray<BoundExpression> arguments, ImmutableArray<string?> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, TypeSymbol? type)
        {
            if (arguments != this.Arguments || argumentNamesOpt != this.ArgumentNamesOpt || inferredNamesOpt != this.InferredNamesOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundTupleLiteral(this.Syntax, arguments, argumentNamesOpt, inferredNamesOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundConvertedTupleLiteral : BoundTupleExpression
    {
        public BoundConvertedTupleLiteral(SyntaxNode syntax, BoundTupleLiteral? sourceTuple, bool wasTargetTyped, ImmutableArray<BoundExpression> arguments, ImmutableArray<string?> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.ConvertedTupleLiteral, syntax, arguments, argumentNamesOpt, inferredNamesOpt, type, hasErrors || sourceTuple.HasErrors() || arguments.HasErrors())
        {


            this.SourceTuple = sourceTuple;
            this.WasTargetTyped = wasTargetTyped;
        }


        public BoundTupleLiteral? SourceTuple { get; }

        public bool WasTargetTyped { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitConvertedTupleLiteral(this);

        public BoundConvertedTupleLiteral Update(BoundTupleLiteral? sourceTuple, bool wasTargetTyped, ImmutableArray<BoundExpression> arguments, ImmutableArray<string?> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, TypeSymbol? type)
        {
            if (sourceTuple != this.SourceTuple || wasTargetTyped != this.WasTargetTyped || arguments != this.Arguments || argumentNamesOpt != this.ArgumentNamesOpt || inferredNamesOpt != this.InferredNamesOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundConvertedTupleLiteral(this.Syntax, sourceTuple, wasTargetTyped, arguments, argumentNamesOpt, inferredNamesOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDynamicObjectCreationExpression : BoundExpression
    {
        public BoundDynamicObjectCreationExpression(SyntaxNode syntax, string name, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, BoundObjectInitializerExpressionBase? initializerExpressionOpt, ImmutableArray<MethodSymbol> applicableMethods, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DynamicObjectCreationExpression, syntax, type, hasErrors || arguments.HasErrors() || initializerExpressionOpt.HasErrors())
        {


            this.Name = name;
            this.Arguments = arguments;
            this.ArgumentNamesOpt = argumentNamesOpt;
            this.ArgumentRefKindsOpt = argumentRefKindsOpt;
            this.InitializerExpressionOpt = initializerExpressionOpt;
            this.ApplicableMethods = applicableMethods;
        }


        public new TypeSymbol Type => base.Type!;

        public string Name { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public BoundObjectInitializerExpressionBase? InitializerExpressionOpt { get; }

        public ImmutableArray<MethodSymbol> ApplicableMethods { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDynamicObjectCreationExpression(this);

        public BoundDynamicObjectCreationExpression Update(string name, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, BoundObjectInitializerExpressionBase? initializerExpressionOpt, ImmutableArray<MethodSymbol> applicableMethods, TypeSymbol type)
        {
            if (name != this.Name || arguments != this.Arguments || argumentNamesOpt != this.ArgumentNamesOpt || argumentRefKindsOpt != this.ArgumentRefKindsOpt || initializerExpressionOpt != this.InitializerExpressionOpt || applicableMethods != this.ApplicableMethods || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDynamicObjectCreationExpression(this.Syntax, name, arguments, argumentNamesOpt, argumentRefKindsOpt, initializerExpressionOpt, applicableMethods, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundNoPiaObjectCreationExpression : BoundExpression
    {
        public BoundNoPiaObjectCreationExpression(SyntaxNode syntax, string? guidString, BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.NoPiaObjectCreationExpression, syntax, type, hasErrors || initializerExpressionOpt.HasErrors())
        {


            this.GuidString = guidString;
            this.InitializerExpressionOpt = initializerExpressionOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public string? GuidString { get; }

        public BoundObjectInitializerExpressionBase? InitializerExpressionOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitNoPiaObjectCreationExpression(this);

        public BoundNoPiaObjectCreationExpression Update(string? guidString, BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type)
        {
            if (guidString != this.GuidString || initializerExpressionOpt != this.InitializerExpressionOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundNoPiaObjectCreationExpression(this.Syntax, guidString, initializerExpressionOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundObjectInitializerExpressionBase : BoundExpression
    {
        protected BoundObjectInitializerExpressionBase(BoundKind kind, SyntaxNode syntax, BoundObjectOrCollectionValuePlaceholder placeholder, ImmutableArray<BoundExpression> initializers, TypeSymbol type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {


            this.Placeholder = placeholder;
            this.Initializers = initializers;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundObjectOrCollectionValuePlaceholder Placeholder { get; }

        public ImmutableArray<BoundExpression> Initializers { get; }
    }

    public sealed partial class BoundObjectInitializerExpression : BoundObjectInitializerExpressionBase
    {
        public BoundObjectInitializerExpression(SyntaxNode syntax, BoundObjectOrCollectionValuePlaceholder placeholder, ImmutableArray<BoundExpression> initializers, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ObjectInitializerExpression, syntax, placeholder, initializers, type, hasErrors || placeholder.HasErrors() || initializers.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitObjectInitializerExpression(this);

        public BoundObjectInitializerExpression Update(BoundObjectOrCollectionValuePlaceholder placeholder, ImmutableArray<BoundExpression> initializers, TypeSymbol type)
        {
            if (placeholder != this.Placeholder || initializers != this.Initializers || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundObjectInitializerExpression(this.Syntax, placeholder, initializers, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundObjectInitializerMember : BoundExpression
    {
        public BoundObjectInitializerMember(SyntaxNode syntax, Symbol? memberSymbol, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, LookupResultKind resultKind, TypeSymbol receiverType, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ObjectInitializerMember, syntax, type, hasErrors || arguments.HasErrors())
        {


            this.MemberSymbol = memberSymbol;
            this.Arguments = arguments;
            this.ArgumentNamesOpt = argumentNamesOpt;
            this.ArgumentRefKindsOpt = argumentRefKindsOpt;
            this.Expanded = expanded;
            this.ArgsToParamsOpt = argsToParamsOpt;
            this.DefaultArguments = defaultArguments;
            this._ResultKind = resultKind;
            this.ReceiverType = receiverType;
        }


        public new TypeSymbol Type => base.Type!;

        public Symbol? MemberSymbol { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public bool Expanded { get; }

        public ImmutableArray<int> ArgsToParamsOpt { get; }

        public BitVector DefaultArguments { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }

        public TypeSymbol ReceiverType { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitObjectInitializerMember(this);

        public BoundObjectInitializerMember Update(Symbol? memberSymbol, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, LookupResultKind resultKind, TypeSymbol receiverType, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(memberSymbol, this.MemberSymbol) || arguments != this.Arguments || argumentNamesOpt != this.ArgumentNamesOpt || argumentRefKindsOpt != this.ArgumentRefKindsOpt || expanded != this.Expanded || argsToParamsOpt != this.ArgsToParamsOpt || defaultArguments != this.DefaultArguments || resultKind != this.ResultKind || !TypeSymbol.Equals(receiverType, this.ReceiverType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundObjectInitializerMember(this.Syntax, memberSymbol, arguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, resultKind, receiverType, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDynamicObjectInitializerMember : BoundExpression
    {
        public BoundDynamicObjectInitializerMember(SyntaxNode syntax, string memberName, TypeSymbol receiverType, TypeSymbol type, bool hasErrors)
            : base(BoundKind.DynamicObjectInitializerMember, syntax, type, hasErrors)
        {


            this.MemberName = memberName;
            this.ReceiverType = receiverType;
        }

        public BoundDynamicObjectInitializerMember(SyntaxNode syntax, string memberName, TypeSymbol receiverType, TypeSymbol type)
            : base(BoundKind.DynamicObjectInitializerMember, syntax, type)
        {


            this.MemberName = memberName;
            this.ReceiverType = receiverType;
        }


        public new TypeSymbol Type => base.Type!;

        public string MemberName { get; }

        public TypeSymbol ReceiverType { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDynamicObjectInitializerMember(this);

        public BoundDynamicObjectInitializerMember Update(string memberName, TypeSymbol receiverType, TypeSymbol type)
        {
            if (memberName != this.MemberName || !TypeSymbol.Equals(receiverType, this.ReceiverType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDynamicObjectInitializerMember(this.Syntax, memberName, receiverType, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundCollectionInitializerExpression : BoundObjectInitializerExpressionBase
    {
        public BoundCollectionInitializerExpression(SyntaxNode syntax, BoundObjectOrCollectionValuePlaceholder placeholder, ImmutableArray<BoundExpression> initializers, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.CollectionInitializerExpression, syntax, placeholder, initializers, type, hasErrors || placeholder.HasErrors() || initializers.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitCollectionInitializerExpression(this);

        public BoundCollectionInitializerExpression Update(BoundObjectOrCollectionValuePlaceholder placeholder, ImmutableArray<BoundExpression> initializers, TypeSymbol type)
        {
            if (placeholder != this.Placeholder || initializers != this.Initializers || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundCollectionInitializerExpression(this.Syntax, placeholder, initializers, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundCollectionElementInitializer : BoundExpression
    {
        public BoundCollectionElementInitializer(SyntaxNode syntax, MethodSymbol addMethod, ImmutableArray<BoundExpression> arguments, BoundExpression? implicitReceiverOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, bool invokedAsExtensionMethod, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.CollectionElementInitializer, syntax, type, hasErrors || arguments.HasErrors() || implicitReceiverOpt.HasErrors())
        {


            this.AddMethod = addMethod;
            this.Arguments = arguments;
            this.ImplicitReceiverOpt = implicitReceiverOpt;
            this.Expanded = expanded;
            this.ArgsToParamsOpt = argsToParamsOpt;
            this.DefaultArguments = defaultArguments;
            this.InvokedAsExtensionMethod = invokedAsExtensionMethod;
            this._ResultKind = resultKind;
        }


        public new TypeSymbol Type => base.Type!;

        public MethodSymbol AddMethod { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public BoundExpression? ImplicitReceiverOpt { get; }

        public bool Expanded { get; }

        public ImmutableArray<int> ArgsToParamsOpt { get; }

        public BitVector DefaultArguments { get; }

        public bool InvokedAsExtensionMethod { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitCollectionElementInitializer(this);

        public BoundCollectionElementInitializer Update(MethodSymbol addMethod, ImmutableArray<BoundExpression> arguments, BoundExpression? implicitReceiverOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, bool invokedAsExtensionMethod, LookupResultKind resultKind, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(addMethod, this.AddMethod) || arguments != this.Arguments || implicitReceiverOpt != this.ImplicitReceiverOpt || expanded != this.Expanded || argsToParamsOpt != this.ArgsToParamsOpt || defaultArguments != this.DefaultArguments || invokedAsExtensionMethod != this.InvokedAsExtensionMethod || resultKind != this.ResultKind || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundCollectionElementInitializer(this.Syntax, addMethod, arguments, implicitReceiverOpt, expanded, argsToParamsOpt, defaultArguments, invokedAsExtensionMethod, resultKind, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDynamicCollectionElementInitializer : BoundDynamicInvocableBase
    {
        public BoundDynamicCollectionElementInitializer(SyntaxNode syntax, ImmutableArray<MethodSymbol> applicableMethods, BoundExpression expression, ImmutableArray<BoundExpression> arguments, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DynamicCollectionElementInitializer, syntax, expression, arguments, type, hasErrors || expression.HasErrors() || arguments.HasErrors())
        {


            this.ApplicableMethods = applicableMethods;
        }


        public new TypeSymbol Type => base.Type!;

        public ImmutableArray<MethodSymbol> ApplicableMethods { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDynamicCollectionElementInitializer(this);

        public BoundDynamicCollectionElementInitializer Update(ImmutableArray<MethodSymbol> applicableMethods, BoundExpression expression, ImmutableArray<BoundExpression> arguments, TypeSymbol type)
        {
            if (applicableMethods != this.ApplicableMethods || expression != this.Expression || arguments != this.Arguments || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDynamicCollectionElementInitializer(this.Syntax, applicableMethods, expression, arguments, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundImplicitReceiver : BoundExpression
    {
        public BoundImplicitReceiver(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ImplicitReceiver, syntax, type, hasErrors)
        {


        }

        public BoundImplicitReceiver(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.ImplicitReceiver, syntax, type)
        {


        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitImplicitReceiver(this);

        public BoundImplicitReceiver Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundImplicitReceiver(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundAnonymousObjectCreationExpression : BoundExpression
    {
        public BoundAnonymousObjectCreationExpression(SyntaxNode syntax, MethodSymbol constructor, ImmutableArray<BoundExpression> arguments, ImmutableArray<BoundAnonymousPropertyDeclaration> declarations, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.AnonymousObjectCreationExpression, syntax, type, hasErrors || arguments.HasErrors() || declarations.HasErrors())
        {


            this.Constructor = constructor;
            this.Arguments = arguments;
            this.Declarations = declarations;
        }


        public new TypeSymbol Type => base.Type!;

        public MethodSymbol Constructor { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<BoundAnonymousPropertyDeclaration> Declarations { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitAnonymousObjectCreationExpression(this);

        public BoundAnonymousObjectCreationExpression Update(MethodSymbol constructor, ImmutableArray<BoundExpression> arguments, ImmutableArray<BoundAnonymousPropertyDeclaration> declarations, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(constructor, this.Constructor) || arguments != this.Arguments || declarations != this.Declarations || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundAnonymousObjectCreationExpression(this.Syntax, constructor, arguments, declarations, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundAnonymousPropertyDeclaration : BoundExpression
    {
        public BoundAnonymousPropertyDeclaration(SyntaxNode syntax, PropertySymbol property, TypeSymbol type, bool hasErrors)
            : base(BoundKind.AnonymousPropertyDeclaration, syntax, type, hasErrors)
        {


            this.Property = property;
        }

        public BoundAnonymousPropertyDeclaration(SyntaxNode syntax, PropertySymbol property, TypeSymbol type)
            : base(BoundKind.AnonymousPropertyDeclaration, syntax, type)
        {


            this.Property = property;
        }


        public new TypeSymbol Type => base.Type!;

        public PropertySymbol Property { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitAnonymousPropertyDeclaration(this);

        public BoundAnonymousPropertyDeclaration Update(PropertySymbol property, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(property, this.Property) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundAnonymousPropertyDeclaration(this.Syntax, property, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundNewT : BoundExpression
    {
        public BoundNewT(SyntaxNode syntax, BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.NewT, syntax, type, hasErrors || initializerExpressionOpt.HasErrors())
        {


            this.InitializerExpressionOpt = initializerExpressionOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundObjectInitializerExpressionBase? InitializerExpressionOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitNewT(this);

        public BoundNewT Update(BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type)
        {
            if (initializerExpressionOpt != this.InitializerExpressionOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundNewT(this.Syntax, initializerExpressionOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDelegateCreationExpression : BoundExpression
    {
        public BoundDelegateCreationExpression(SyntaxNode syntax, BoundExpression argument, MethodSymbol? methodOpt, bool isExtensionMethod, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DelegateCreationExpression, syntax, type, hasErrors || argument.HasErrors())
        {


            this.Argument = argument;
            this.MethodOpt = methodOpt;
            this.IsExtensionMethod = isExtensionMethod;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Argument { get; }

        public MethodSymbol? MethodOpt { get; }

        public bool IsExtensionMethod { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDelegateCreationExpression(this);

        public BoundDelegateCreationExpression Update(BoundExpression argument, MethodSymbol? methodOpt, bool isExtensionMethod, TypeSymbol type)
        {
            if (argument != this.Argument || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, this.MethodOpt) || isExtensionMethod != this.IsExtensionMethod || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDelegateCreationExpression(this.Syntax, argument, methodOpt, isExtensionMethod, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundArrayCreation : BoundExpression
    {
        public BoundArrayCreation(SyntaxNode syntax, ImmutableArray<BoundExpression> bounds, BoundArrayInitialization? initializerOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ArrayCreation, syntax, type, hasErrors || bounds.HasErrors() || initializerOpt.HasErrors())
        {
            this.Bounds = bounds;
            this.InitializerOpt = initializerOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public ImmutableArray<BoundExpression> Bounds { get; }

        public BoundArrayInitialization? InitializerOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitArrayCreation(this);

        public BoundArrayCreation Update(ImmutableArray<BoundExpression> bounds, BoundArrayInitialization? initializerOpt, TypeSymbol type)
        {
            if (bounds != this.Bounds || initializerOpt != this.InitializerOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundArrayCreation(this.Syntax, bounds, initializerOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundArrayInitialization : BoundExpression
    {
        public BoundArrayInitialization(SyntaxNode syntax, ImmutableArray<BoundExpression> initializers, bool hasErrors = false)
            : base(BoundKind.ArrayInitialization, syntax, null, hasErrors || initializers.HasErrors())
        {


            this.Initializers = initializers;
        }


        public new TypeSymbol? Type => base.Type;

        public ImmutableArray<BoundExpression> Initializers { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitArrayInitialization(this);

        public BoundArrayInitialization Update(ImmutableArray<BoundExpression> initializers)
        {
            if (initializers != this.Initializers)
            {
                var result = new BoundArrayInitialization(this.Syntax, initializers, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundStackAllocArrayCreationBase : BoundExpression
    {
        protected BoundStackAllocArrayCreationBase(BoundKind kind, SyntaxNode syntax, TypeSymbol elementType, BoundExpression count, BoundArrayInitialization? initializerOpt, TypeSymbol? type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {


            this.ElementType = elementType;
            this.Count = count;
            this.InitializerOpt = initializerOpt;
        }


        public TypeSymbol ElementType { get; }

        public BoundExpression Count { get; }

        public BoundArrayInitialization? InitializerOpt { get; }
    }

    public sealed partial class BoundStackAllocArrayCreation : BoundStackAllocArrayCreationBase
    {
        public BoundStackAllocArrayCreation(SyntaxNode syntax, TypeSymbol elementType, BoundExpression count, BoundArrayInitialization? initializerOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.StackAllocArrayCreation, syntax, elementType, count, initializerOpt, type, hasErrors || count.HasErrors() || initializerOpt.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitStackAllocArrayCreation(this);

        public BoundStackAllocArrayCreation Update(TypeSymbol elementType, BoundExpression count, BoundArrayInitialization? initializerOpt, TypeSymbol? type)
        {
            if (!TypeSymbol.Equals(elementType, this.ElementType, TypeCompareKind.ConsiderEverything) || count != this.Count || initializerOpt != this.InitializerOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundStackAllocArrayCreation(this.Syntax, elementType, count, initializerOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundConvertedStackAllocExpression : BoundStackAllocArrayCreationBase
    {
        public BoundConvertedStackAllocExpression(SyntaxNode syntax, TypeSymbol elementType, BoundExpression count, BoundArrayInitialization? initializerOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ConvertedStackAllocExpression, syntax, elementType, count, initializerOpt, type, hasErrors || count.HasErrors() || initializerOpt.HasErrors())
        {


        }


        public new TypeSymbol Type => base.Type!;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitConvertedStackAllocExpression(this);

        public BoundConvertedStackAllocExpression Update(TypeSymbol elementType, BoundExpression count, BoundArrayInitialization? initializerOpt, TypeSymbol type)
        {
            if (!TypeSymbol.Equals(elementType, this.ElementType, TypeCompareKind.ConsiderEverything) || count != this.Count || initializerOpt != this.InitializerOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundConvertedStackAllocExpression(this.Syntax, elementType, count, initializerOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundFieldAccess : BoundExpression
    {
        public BoundFieldAccess(SyntaxNode syntax, BoundExpression? receiverOpt, FieldSymbol fieldSymbol, ConstantValue? constantValueOpt, LookupResultKind resultKind, bool isByValue, bool isDeclaration, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.FieldAccess, syntax, type, hasErrors || receiverOpt.HasErrors())
        {


            this.ReceiverOpt = receiverOpt;
            this.FieldSymbol = fieldSymbol;
            this.ConstantValueOpt = constantValueOpt;
            this._ResultKind = resultKind;
            this.IsByValue = isByValue;
            this.IsDeclaration = isDeclaration;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression? ReceiverOpt { get; }

        public FieldSymbol FieldSymbol { get; }

        public ConstantValue? ConstantValueOpt { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }

        public bool IsByValue { get; }

        public bool IsDeclaration { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitFieldAccess(this);

        public BoundFieldAccess Update(BoundExpression? receiverOpt, FieldSymbol fieldSymbol, ConstantValue? constantValueOpt, LookupResultKind resultKind, bool isByValue, bool isDeclaration, TypeSymbol type)
        {
            if (receiverOpt != this.ReceiverOpt || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(fieldSymbol, this.FieldSymbol) || constantValueOpt != this.ConstantValueOpt || resultKind != this.ResultKind || isByValue != this.IsByValue || isDeclaration != this.IsDeclaration || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundFieldAccess(this.Syntax, receiverOpt, fieldSymbol, constantValueOpt, resultKind, isByValue, isDeclaration, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundHoistedFieldAccess : BoundExpression
    {
        public BoundHoistedFieldAccess(SyntaxNode syntax, FieldSymbol fieldSymbol, TypeSymbol type, bool hasErrors)
            : base(BoundKind.HoistedFieldAccess, syntax, type, hasErrors)
        {


            this.FieldSymbol = fieldSymbol;
        }

        public BoundHoistedFieldAccess(SyntaxNode syntax, FieldSymbol fieldSymbol, TypeSymbol type)
            : base(BoundKind.HoistedFieldAccess, syntax, type)
        {


            this.FieldSymbol = fieldSymbol;
        }


        public new TypeSymbol Type => base.Type!;

        public FieldSymbol FieldSymbol { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitHoistedFieldAccess(this);

        public BoundHoistedFieldAccess Update(FieldSymbol fieldSymbol, TypeSymbol type)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(fieldSymbol, this.FieldSymbol) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundHoistedFieldAccess(this.Syntax, fieldSymbol, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundPropertyAccess : BoundExpression
    {
        public BoundPropertyAccess(SyntaxNode syntax, BoundExpression? receiverOpt, PropertySymbol propertySymbol, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.PropertyAccess, syntax, type, hasErrors || receiverOpt.HasErrors())
        {


            this.ReceiverOpt = receiverOpt;
            this.PropertySymbol = propertySymbol;
            this._ResultKind = resultKind;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression? ReceiverOpt { get; }

        public PropertySymbol PropertySymbol { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitPropertyAccess(this);

        public BoundPropertyAccess Update(BoundExpression? receiverOpt, PropertySymbol propertySymbol, LookupResultKind resultKind, TypeSymbol type)
        {
            if (receiverOpt != this.ReceiverOpt || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(propertySymbol, this.PropertySymbol) || resultKind != this.ResultKind || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundPropertyAccess(this.Syntax, receiverOpt, propertySymbol, resultKind, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundEventAccess : BoundExpression
    {
        public BoundEventAccess(SyntaxNode syntax, BoundExpression? receiverOpt, EventSymbol eventSymbol, bool isUsableAsField, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.EventAccess, syntax, type, hasErrors || receiverOpt.HasErrors())
        {


            this.ReceiverOpt = receiverOpt;
            this.EventSymbol = eventSymbol;
            this.IsUsableAsField = isUsableAsField;
            this._ResultKind = resultKind;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression? ReceiverOpt { get; }

        public EventSymbol EventSymbol { get; }

        public bool IsUsableAsField { get; }

        private readonly LookupResultKind _ResultKind;
        public override LookupResultKind ResultKind { get { return _ResultKind; } }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitEventAccess(this);

        public BoundEventAccess Update(BoundExpression? receiverOpt, EventSymbol eventSymbol, bool isUsableAsField, LookupResultKind resultKind, TypeSymbol type)
        {
            if (receiverOpt != this.ReceiverOpt || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(eventSymbol, this.EventSymbol) || isUsableAsField != this.IsUsableAsField || resultKind != this.ResultKind || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundEventAccess(this.Syntax, receiverOpt, eventSymbol, isUsableAsField, resultKind, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundIndexerAccess : BoundExpression
    {
        public BoundIndexerAccess(SyntaxNode syntax, BoundExpression? receiverOpt, PropertySymbol indexer, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ImmutableArray<PropertySymbol> originalIndexersOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.IndexerAccess, syntax, type, hasErrors || receiverOpt.HasErrors() || arguments.HasErrors())
        {


            this.ReceiverOpt = receiverOpt;
            this.Indexer = indexer;
            this.Arguments = arguments;
            this.ArgumentNamesOpt = argumentNamesOpt;
            this.ArgumentRefKindsOpt = argumentRefKindsOpt;
            this.Expanded = expanded;
            this.ArgsToParamsOpt = argsToParamsOpt;
            this.DefaultArguments = defaultArguments;
            this.OriginalIndexersOpt = originalIndexersOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression? ReceiverOpt { get; }

        public PropertySymbol Indexer { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public bool Expanded { get; }

        public ImmutableArray<int> ArgsToParamsOpt { get; }

        public BitVector DefaultArguments { get; }

        public ImmutableArray<PropertySymbol> OriginalIndexersOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitIndexerAccess(this);

        public BoundIndexerAccess Update(BoundExpression? receiverOpt, PropertySymbol indexer, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ImmutableArray<PropertySymbol> originalIndexersOpt, TypeSymbol type)
        {
            if (receiverOpt != this.ReceiverOpt || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(indexer, this.Indexer) || arguments != this.Arguments || argumentNamesOpt != this.ArgumentNamesOpt || argumentRefKindsOpt != this.ArgumentRefKindsOpt || expanded != this.Expanded || argsToParamsOpt != this.ArgsToParamsOpt || defaultArguments != this.DefaultArguments || originalIndexersOpt != this.OriginalIndexersOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundIndexerAccess(this.Syntax, receiverOpt, indexer, arguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, originalIndexersOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundIndexOrRangePatternIndexerAccess : BoundExpression
    {
        public BoundIndexOrRangePatternIndexerAccess(SyntaxNode syntax, BoundExpression receiver, PropertySymbol lengthOrCountProperty, Symbol patternSymbol, BoundExpression argument, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.IndexOrRangePatternIndexerAccess, syntax, type, hasErrors || receiver.HasErrors() || argument.HasErrors())
        {


            this.Receiver = receiver;
            this.LengthOrCountProperty = lengthOrCountProperty;
            this.PatternSymbol = patternSymbol;
            this.Argument = argument;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Receiver { get; }

        public PropertySymbol LengthOrCountProperty { get; }

        public Symbol PatternSymbol { get; }

        public BoundExpression Argument { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitIndexOrRangePatternIndexerAccess(this);

        public BoundIndexOrRangePatternIndexerAccess Update(BoundExpression receiver, PropertySymbol lengthOrCountProperty, Symbol patternSymbol, BoundExpression argument, TypeSymbol type)
        {
            if (receiver != this.Receiver || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(lengthOrCountProperty, this.LengthOrCountProperty) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(patternSymbol, this.PatternSymbol) || argument != this.Argument || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundIndexOrRangePatternIndexerAccess(this.Syntax, receiver, lengthOrCountProperty, patternSymbol, argument, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDynamicIndexerAccess : BoundExpression
    {
        public BoundDynamicIndexerAccess(SyntaxNode syntax, BoundExpression receiver, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<PropertySymbol> applicableIndexers, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DynamicIndexerAccess, syntax, type, hasErrors || receiver.HasErrors() || arguments.HasErrors())
        {


            this.Receiver = receiver;
            this.Arguments = arguments;
            this.ArgumentNamesOpt = argumentNamesOpt;
            this.ArgumentRefKindsOpt = argumentRefKindsOpt;
            this.ApplicableIndexers = applicableIndexers;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Receiver { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public ImmutableArray<PropertySymbol> ApplicableIndexers { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDynamicIndexerAccess(this);

        public BoundDynamicIndexerAccess Update(BoundExpression receiver, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<PropertySymbol> applicableIndexers, TypeSymbol type)
        {
            if (receiver != this.Receiver || arguments != this.Arguments || argumentNamesOpt != this.ArgumentNamesOpt || argumentRefKindsOpt != this.ArgumentRefKindsOpt || applicableIndexers != this.ApplicableIndexers || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDynamicIndexerAccess(this.Syntax, receiver, arguments, argumentNamesOpt, argumentRefKindsOpt, applicableIndexers, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundLambda : BoundExpression
    {
        public BoundLambda(SyntaxNode syntax, UnboundLambda unboundLambda, LambdaSymbol symbol, BoundBlock body, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics, Binder binder, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.Lambda, syntax, type, hasErrors || unboundLambda.HasErrors() || body.HasErrors())
        {


            this.UnboundLambda = unboundLambda;
            this.Symbol = symbol;
            this.Body = body;
            this.Diagnostics = diagnostics;
            this.Binder = binder;
        }


        public UnboundLambda UnboundLambda { get; }

        public LambdaSymbol Symbol { get; }

        public new TypeSymbol? Type => base.Type;

        public BoundBlock Body { get; }

        public ImmutableBindingDiagnostic<AssemblySymbol> Diagnostics { get; }

        public Binder Binder { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitLambda(this);

        public BoundLambda Update(UnboundLambda unboundLambda, LambdaSymbol symbol, BoundBlock body, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics, Binder binder, TypeSymbol? type)
        {
            if (unboundLambda != this.UnboundLambda || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(symbol, this.Symbol) || body != this.Body || diagnostics != this.Diagnostics || binder != this.Binder || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundLambda(this.Syntax, unboundLambda, symbol, body, diagnostics, binder, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class UnboundLambda : BoundExpression
    {
        public UnboundLambda(SyntaxNode syntax, UnboundLambdaState data, Boolean withDependencies, bool hasErrors)
            : base(BoundKind.UnboundLambda, syntax, null, hasErrors)
        {


            this.Data = data;
            this.WithDependencies = withDependencies;
        }

        public UnboundLambda(SyntaxNode syntax, UnboundLambdaState data, Boolean withDependencies)
            : base(BoundKind.UnboundLambda, syntax, null)
        {


            this.Data = data;
            this.WithDependencies = withDependencies;
        }


        public new TypeSymbol? Type => base.Type;

        public UnboundLambdaState Data { get; }

        public Boolean WithDependencies { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitUnboundLambda(this);

        public UnboundLambda Update(UnboundLambdaState data, Boolean withDependencies)
        {
            if (data != this.Data || withDependencies != this.WithDependencies)
            {
                var result = new UnboundLambda(this.Syntax, data, withDependencies, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundQueryClause : BoundExpression
    {
        public BoundQueryClause(SyntaxNode syntax, BoundExpression value, RangeVariableSymbol? definedSymbol, BoundExpression? operation, BoundExpression? cast, Binder binder, BoundExpression? unoptimizedForm, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.QueryClause, syntax, type, hasErrors || value.HasErrors() || operation.HasErrors() || cast.HasErrors() || unoptimizedForm.HasErrors())
        {


            this.Value = value;
            this.DefinedSymbol = definedSymbol;
            this.Operation = operation;
            this.Cast = cast;
            this.Binder = binder;
            this.UnoptimizedForm = unoptimizedForm;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Value { get; }

        public RangeVariableSymbol? DefinedSymbol { get; }

        public BoundExpression? Operation { get; }

        public BoundExpression? Cast { get; }

        public Binder Binder { get; }

        public BoundExpression? UnoptimizedForm { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitQueryClause(this);

        public BoundQueryClause Update(BoundExpression value, RangeVariableSymbol? definedSymbol, BoundExpression? operation, BoundExpression? cast, Binder binder, BoundExpression? unoptimizedForm, TypeSymbol type)
        {
            if (value != this.Value || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(definedSymbol, this.DefinedSymbol) || operation != this.Operation || cast != this.Cast || binder != this.Binder || unoptimizedForm != this.UnoptimizedForm || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundQueryClause(this.Syntax, value, definedSymbol, operation, cast, binder, unoptimizedForm, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundTypeOrInstanceInitializers : BoundStatementList
    {
        public BoundTypeOrInstanceInitializers(SyntaxNode syntax, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
            : base(BoundKind.TypeOrInstanceInitializers, syntax, statements, hasErrors || statements.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitTypeOrInstanceInitializers(this);

        public new BoundTypeOrInstanceInitializers Update(ImmutableArray<BoundStatement> statements)
        {
            if (statements != this.Statements)
            {
                var result = new BoundTypeOrInstanceInitializers(this.Syntax, statements, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundNameOfOperator : BoundExpression
    {
        public BoundNameOfOperator(SyntaxNode syntax, BoundExpression argument, ConstantValue constantValueOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.NameOfOperator, syntax, type, hasErrors || argument.HasErrors())
        {


            this.Argument = argument;
            this.ConstantValueOpt = constantValueOpt;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Argument { get; }

        public ConstantValue ConstantValueOpt { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitNameOfOperator(this);

        public BoundNameOfOperator Update(BoundExpression argument, ConstantValue constantValueOpt, TypeSymbol type)
        {
            if (argument != this.Argument || constantValueOpt != this.ConstantValueOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundNameOfOperator(this.Syntax, argument, constantValueOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundInterpolatedStringBase : BoundExpression
    {
        protected BoundInterpolatedStringBase(BoundKind kind, SyntaxNode syntax, ImmutableArray<BoundExpression> parts, ConstantValue? constantValueOpt, TypeSymbol? type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {


            this.Parts = parts;
            this.ConstantValueOpt = constantValueOpt;
        }


        public ImmutableArray<BoundExpression> Parts { get; }

        public ConstantValue? ConstantValueOpt { get; }
    }

    public sealed partial class BoundUnconvertedInterpolatedString : BoundInterpolatedStringBase
    {
        public BoundUnconvertedInterpolatedString(SyntaxNode syntax, ImmutableArray<BoundExpression> parts, ConstantValue? constantValueOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.UnconvertedInterpolatedString, syntax, parts, constantValueOpt, type, hasErrors || parts.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitUnconvertedInterpolatedString(this);

        public BoundUnconvertedInterpolatedString Update(ImmutableArray<BoundExpression> parts, ConstantValue? constantValueOpt, TypeSymbol? type)
        {
            if (parts != this.Parts || constantValueOpt != this.ConstantValueOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundUnconvertedInterpolatedString(this.Syntax, parts, constantValueOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundInterpolatedString : BoundInterpolatedStringBase
    {
        public BoundInterpolatedString(SyntaxNode syntax, ImmutableArray<BoundExpression> parts, ConstantValue? constantValueOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.InterpolatedString, syntax, parts, constantValueOpt, type, hasErrors || parts.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitInterpolatedString(this);

        public BoundInterpolatedString Update(ImmutableArray<BoundExpression> parts, ConstantValue? constantValueOpt, TypeSymbol? type)
        {
            if (parts != this.Parts || constantValueOpt != this.ConstantValueOpt || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundInterpolatedString(this.Syntax, parts, constantValueOpt, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundStringInsert : BoundExpression
    {
        public BoundStringInsert(SyntaxNode syntax, BoundExpression value, BoundExpression? alignment, BoundLiteral? format, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.StringInsert, syntax, type, hasErrors || value.HasErrors() || alignment.HasErrors() || format.HasErrors())
        {


            this.Value = value;
            this.Alignment = alignment;
            this.Format = format;
        }


        public BoundExpression Value { get; }

        public BoundExpression? Alignment { get; }

        public BoundLiteral? Format { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitStringInsert(this);

        public BoundStringInsert Update(BoundExpression value, BoundExpression? alignment, BoundLiteral? format, TypeSymbol? type)
        {
            if (value != this.Value || alignment != this.Alignment || format != this.Format || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundStringInsert(this.Syntax, value, alignment, format, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundIsPatternExpression : BoundExpression
    {
        public BoundIsPatternExpression(SyntaxNode syntax, BoundExpression expression, BoundPattern pattern, bool isNegated, BoundDecisionDag decisionDag, LabelSymbol whenTrueLabel, LabelSymbol whenFalseLabel, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.IsPatternExpression, syntax, type, hasErrors || expression.HasErrors() || pattern.HasErrors() || decisionDag.HasErrors())
        {


            this.Expression = expression;
            this.Pattern = pattern;
            this.IsNegated = isNegated;
            this.DecisionDag = decisionDag;
            this.WhenTrueLabel = whenTrueLabel;
            this.WhenFalseLabel = whenFalseLabel;
        }


        public BoundExpression Expression { get; }

        public BoundPattern Pattern { get; }

        public bool IsNegated { get; }

        public BoundDecisionDag DecisionDag { get; }

        public LabelSymbol WhenTrueLabel { get; }

        public LabelSymbol WhenFalseLabel { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitIsPatternExpression(this);

        public BoundIsPatternExpression Update(BoundExpression expression, BoundPattern pattern, bool isNegated, BoundDecisionDag decisionDag, LabelSymbol whenTrueLabel, LabelSymbol whenFalseLabel, TypeSymbol? type)
        {
            if (expression != this.Expression || pattern != this.Pattern || isNegated != this.IsNegated || decisionDag != this.DecisionDag || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(whenTrueLabel, this.WhenTrueLabel) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(whenFalseLabel, this.WhenFalseLabel) || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundIsPatternExpression(this.Syntax, expression, pattern, isNegated, decisionDag, whenTrueLabel, whenFalseLabel, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class BoundPattern : BoundNode
    {
        protected BoundPattern(BoundKind kind, SyntaxNode syntax, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors)
            : base(kind, syntax, hasErrors)
        {


            this.InputType = inputType;
            this.NarrowedType = narrowedType;
        }

        protected BoundPattern(BoundKind kind, SyntaxNode syntax, TypeSymbol inputType, TypeSymbol narrowedType)
            : base(kind, syntax)
        {


            this.InputType = inputType;
            this.NarrowedType = narrowedType;
        }


        public TypeSymbol InputType { get; }

        public TypeSymbol NarrowedType { get; }
    }

    public sealed partial class BoundConstantPattern : BoundPattern
    {
        public BoundConstantPattern(SyntaxNode syntax, BoundExpression value, ConstantValue constantValue, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.ConstantPattern, syntax, inputType, narrowedType, hasErrors || value.HasErrors())
        {


            this.Value = value;
            this.ConstantValue = constantValue;
        }


        public BoundExpression Value { get; }

        public ConstantValue ConstantValue { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitConstantPattern(this);

        public BoundConstantPattern Update(BoundExpression value, ConstantValue constantValue, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (value != this.Value || constantValue != this.ConstantValue || !TypeSymbol.Equals(inputType, this.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, this.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundConstantPattern(this.Syntax, value, constantValue, inputType, narrowedType, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDiscardPattern : BoundPattern
    {
        public BoundDiscardPattern(SyntaxNode syntax, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors)
            : base(BoundKind.DiscardPattern, syntax, inputType, narrowedType, hasErrors)
        {


        }

        public BoundDiscardPattern(SyntaxNode syntax, TypeSymbol inputType, TypeSymbol narrowedType)
            : base(BoundKind.DiscardPattern, syntax, inputType, narrowedType)
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDiscardPattern(this);

        public BoundDiscardPattern Update(TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (!TypeSymbol.Equals(inputType, this.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, this.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDiscardPattern(this.Syntax, inputType, narrowedType, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDeclarationPattern : BoundPattern
    {
        public BoundDeclarationPattern(SyntaxNode syntax, Symbol? variable, BoundExpression? variableAccess, BoundTypeExpression declaredType, bool isVar, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.DeclarationPattern, syntax, inputType, narrowedType, hasErrors || variableAccess.HasErrors() || declaredType.HasErrors())
        {


            this.Variable = variable;
            this.VariableAccess = variableAccess;
            this.DeclaredType = declaredType;
            this.IsVar = isVar;
        }


        public Symbol? Variable { get; }

        public BoundExpression? VariableAccess { get; }

        public BoundTypeExpression DeclaredType { get; }

        public bool IsVar { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDeclarationPattern(this);

        public BoundDeclarationPattern Update(Symbol? variable, BoundExpression? variableAccess, BoundTypeExpression declaredType, bool isVar, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(variable, this.Variable) || variableAccess != this.VariableAccess || declaredType != this.DeclaredType || isVar != this.IsVar || !TypeSymbol.Equals(inputType, this.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, this.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDeclarationPattern(this.Syntax, variable, variableAccess, declaredType, isVar, inputType, narrowedType, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundRecursivePattern : BoundPattern
    {
        public BoundRecursivePattern(SyntaxNode syntax, BoundTypeExpression? declaredType, MethodSymbol? deconstructMethod, ImmutableArray<BoundSubpattern> deconstruction, ImmutableArray<BoundSubpattern> properties, Symbol? variable, BoundExpression? variableAccess, bool isExplicitNotNullTest, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.RecursivePattern, syntax, inputType, narrowedType, hasErrors || declaredType.HasErrors() || deconstruction.HasErrors() || properties.HasErrors() || variableAccess.HasErrors())
        {


            this.DeclaredType = declaredType;
            this.DeconstructMethod = deconstructMethod;
            this.Deconstruction = deconstruction;
            this.Properties = properties;
            this.Variable = variable;
            this.VariableAccess = variableAccess;
            this.IsExplicitNotNullTest = isExplicitNotNullTest;
        }


        public BoundTypeExpression? DeclaredType { get; }

        public MethodSymbol? DeconstructMethod { get; }

        public ImmutableArray<BoundSubpattern> Deconstruction { get; }

        public ImmutableArray<BoundSubpattern> Properties { get; }

        public Symbol? Variable { get; }

        public BoundExpression? VariableAccess { get; }

        public bool IsExplicitNotNullTest { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitRecursivePattern(this);

        public BoundRecursivePattern Update(BoundTypeExpression? declaredType, MethodSymbol? deconstructMethod, ImmutableArray<BoundSubpattern> deconstruction, ImmutableArray<BoundSubpattern> properties, Symbol? variable, BoundExpression? variableAccess, bool isExplicitNotNullTest, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (declaredType != this.DeclaredType || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(deconstructMethod, this.DeconstructMethod) || deconstruction != this.Deconstruction || properties != this.Properties || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(variable, this.Variable) || variableAccess != this.VariableAccess || isExplicitNotNullTest != this.IsExplicitNotNullTest || !TypeSymbol.Equals(inputType, this.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, this.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundRecursivePattern(this.Syntax, declaredType, deconstructMethod, deconstruction, properties, variable, variableAccess, isExplicitNotNullTest, inputType, narrowedType, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundITuplePattern : BoundPattern
    {
        public BoundITuplePattern(SyntaxNode syntax, MethodSymbol getLengthMethod, MethodSymbol getItemMethod, ImmutableArray<BoundSubpattern> subpatterns, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.ITuplePattern, syntax, inputType, narrowedType, hasErrors || subpatterns.HasErrors())
        {


            this.GetLengthMethod = getLengthMethod;
            this.GetItemMethod = getItemMethod;
            this.Subpatterns = subpatterns;
        }


        public MethodSymbol GetLengthMethod { get; }

        public MethodSymbol GetItemMethod { get; }

        public ImmutableArray<BoundSubpattern> Subpatterns { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitITuplePattern(this);

        public BoundITuplePattern Update(MethodSymbol getLengthMethod, MethodSymbol getItemMethod, ImmutableArray<BoundSubpattern> subpatterns, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getLengthMethod, this.GetLengthMethod) || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getItemMethod, this.GetItemMethod) || subpatterns != this.Subpatterns || !TypeSymbol.Equals(inputType, this.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, this.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundITuplePattern(this.Syntax, getLengthMethod, getItemMethod, subpatterns, inputType, narrowedType, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundSubpattern : BoundNode
    {
        public BoundSubpattern(SyntaxNode syntax, Symbol? symbol, BoundPattern pattern, bool hasErrors = false)
            : base(BoundKind.Subpattern, syntax, hasErrors || pattern.HasErrors())
        {


            this.Symbol = symbol;
            this.Pattern = pattern;
        }


        public Symbol? Symbol { get; }

        public BoundPattern Pattern { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitSubpattern(this);

        public BoundSubpattern Update(Symbol? symbol, BoundPattern pattern)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(symbol, this.Symbol) || pattern != this.Pattern)
            {
                var result = new BoundSubpattern(this.Syntax, symbol, pattern, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundTypePattern : BoundPattern
    {
        public BoundTypePattern(SyntaxNode syntax, BoundTypeExpression declaredType, bool isExplicitNotNullTest, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.TypePattern, syntax, inputType, narrowedType, hasErrors || declaredType.HasErrors())
        {


            this.DeclaredType = declaredType;
            this.IsExplicitNotNullTest = isExplicitNotNullTest;
        }


        public BoundTypeExpression DeclaredType { get; }

        public bool IsExplicitNotNullTest { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitTypePattern(this);

        public BoundTypePattern Update(BoundTypeExpression declaredType, bool isExplicitNotNullTest, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (declaredType != this.DeclaredType || isExplicitNotNullTest != this.IsExplicitNotNullTest || !TypeSymbol.Equals(inputType, this.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, this.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundTypePattern(this.Syntax, declaredType, isExplicitNotNullTest, inputType, narrowedType, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundBinaryPattern : BoundPattern
    {
        public BoundBinaryPattern(SyntaxNode syntax, bool disjunction, BoundPattern left, BoundPattern right, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.BinaryPattern, syntax, inputType, narrowedType, hasErrors || left.HasErrors() || right.HasErrors())
        {


            this.Disjunction = disjunction;
            this.Left = left;
            this.Right = right;
        }


        public bool Disjunction { get; }

        public BoundPattern Left { get; }

        public BoundPattern Right { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitBinaryPattern(this);

        public BoundBinaryPattern Update(bool disjunction, BoundPattern left, BoundPattern right, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (disjunction != this.Disjunction || left != this.Left || right != this.Right || !TypeSymbol.Equals(inputType, this.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, this.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundBinaryPattern(this.Syntax, disjunction, left, right, inputType, narrowedType, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundNegatedPattern : BoundPattern
    {
        public BoundNegatedPattern(SyntaxNode syntax, BoundPattern negated, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.NegatedPattern, syntax, inputType, narrowedType, hasErrors || negated.HasErrors())
        {


            this.Negated = negated;
        }


        public BoundPattern Negated { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitNegatedPattern(this);

        public BoundNegatedPattern Update(BoundPattern negated, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (negated != this.Negated || !TypeSymbol.Equals(inputType, this.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, this.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundNegatedPattern(this.Syntax, negated, inputType, narrowedType, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundRelationalPattern : BoundPattern
    {
        public BoundRelationalPattern(SyntaxNode syntax, BinaryOperatorKind relation, BoundExpression value, ConstantValue constantValue, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.RelationalPattern, syntax, inputType, narrowedType, hasErrors || value.HasErrors())
        {


            this.Relation = relation;
            this.Value = value;
            this.ConstantValue = constantValue;
        }


        public BinaryOperatorKind Relation { get; }

        public BoundExpression Value { get; }

        public ConstantValue ConstantValue { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitRelationalPattern(this);

        public BoundRelationalPattern Update(BinaryOperatorKind relation, BoundExpression value, ConstantValue constantValue, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (relation != this.Relation || value != this.Value || constantValue != this.ConstantValue || !TypeSymbol.Equals(inputType, this.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, this.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundRelationalPattern(this.Syntax, relation, value, constantValue, inputType, narrowedType, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundDiscardExpression : BoundExpression
    {
        public BoundDiscardExpression(SyntaxNode syntax, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.DiscardExpression, syntax, type, hasErrors)
        {
        }

        public BoundDiscardExpression(SyntaxNode syntax, TypeSymbol? type)
            : base(BoundKind.DiscardExpression, syntax, type)
        {
        }


        public new TypeSymbol? Type => base.Type;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDiscardExpression(this);

        public BoundDiscardExpression Update(TypeSymbol? type)
        {
            if (!TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundDiscardExpression(this.Syntax, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundThrowExpression : BoundExpression
    {
        public BoundThrowExpression(SyntaxNode syntax, BoundExpression expression, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.ThrowExpression, syntax, type, hasErrors || expression.HasErrors())
        {


            this.Expression = expression;
        }


        public BoundExpression Expression { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitThrowExpression(this);

        public BoundThrowExpression Update(BoundExpression expression, TypeSymbol? type)
        {
            if (expression != this.Expression || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundThrowExpression(this.Syntax, expression, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public abstract partial class VariablePendingInference : BoundExpression
    {
        protected VariablePendingInference(BoundKind kind, SyntaxNode syntax, Symbol variableSymbol, BoundExpression? receiverOpt, bool hasErrors = false)
            : base(kind, syntax, null, hasErrors)
        {


            this.VariableSymbol = variableSymbol;
            this.ReceiverOpt = receiverOpt;
        }


        public new TypeSymbol? Type => base.Type;

        public Symbol VariableSymbol { get; }

        public BoundExpression? ReceiverOpt { get; }
    }

    public sealed partial class OutVariablePendingInference : VariablePendingInference
    {
        public OutVariablePendingInference(SyntaxNode syntax, Symbol variableSymbol, BoundExpression? receiverOpt, bool hasErrors = false)
            : base(BoundKind.OutVariablePendingInference, syntax, variableSymbol, receiverOpt, hasErrors || receiverOpt.HasErrors())
        {


        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitOutVariablePendingInference(this);

        public OutVariablePendingInference Update(Symbol variableSymbol, BoundExpression? receiverOpt)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(variableSymbol, this.VariableSymbol) || receiverOpt != this.ReceiverOpt)
            {
                var result = new OutVariablePendingInference(this.Syntax, variableSymbol, receiverOpt, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class DeconstructionVariablePendingInference : VariablePendingInference
    {
        public DeconstructionVariablePendingInference(SyntaxNode syntax, Symbol variableSymbol, BoundExpression? receiverOpt, bool hasErrors = false)
            : base(BoundKind.DeconstructionVariablePendingInference, syntax, variableSymbol, receiverOpt, hasErrors || receiverOpt.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitDeconstructionVariablePendingInference(this);

        public DeconstructionVariablePendingInference Update(Symbol variableSymbol, BoundExpression? receiverOpt)
        {
            if (!Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(variableSymbol, this.VariableSymbol) || receiverOpt != this.ReceiverOpt)
            {
                var result = new DeconstructionVariablePendingInference(this.Syntax, variableSymbol, receiverOpt, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class OutDeconstructVarPendingInference : BoundExpression
    {
        public OutDeconstructVarPendingInference(SyntaxNode syntax, bool hasErrors)
            : base(BoundKind.OutDeconstructVarPendingInference, syntax, null, hasErrors)
        {
        }

        public OutDeconstructVarPendingInference(SyntaxNode syntax)
            : base(BoundKind.OutDeconstructVarPendingInference, syntax, null)
        {
        }


        public new TypeSymbol? Type => base.Type;
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitOutDeconstructVarPendingInference(this);

        public OutDeconstructVarPendingInference Update()
        {
            return this;
        }
    }

    public abstract partial class BoundMethodBodyBase : BoundNode
    {
        protected BoundMethodBodyBase(BoundKind kind, SyntaxNode syntax, BoundBlock? blockBody, BoundBlock? expressionBody, bool hasErrors = false)
            : base(kind, syntax, hasErrors)
        {
            this.BlockBody = blockBody;
            this.ExpressionBody = expressionBody;
        }

        public BoundBlock? BlockBody { get; }

        public BoundBlock? ExpressionBody { get; }
    }

    public sealed partial class BoundNonConstructorMethodBody : BoundMethodBodyBase
    {
        public BoundNonConstructorMethodBody(SyntaxNode syntax, BoundBlock? blockBody, BoundBlock? expressionBody, bool hasErrors = false)
            : base(BoundKind.NonConstructorMethodBody, syntax, blockBody, expressionBody, hasErrors || blockBody.HasErrors() || expressionBody.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitNonConstructorMethodBody(this);

        public BoundNonConstructorMethodBody Update(BoundBlock? blockBody, BoundBlock? expressionBody)
        {
            if (blockBody != this.BlockBody || expressionBody != this.ExpressionBody)
            {
                var result = new BoundNonConstructorMethodBody(this.Syntax, blockBody, expressionBody, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundConstructorMethodBody : BoundMethodBodyBase
    {
        public BoundConstructorMethodBody(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, BoundExpressionStatement? initializer, BoundBlock? blockBody, BoundBlock? expressionBody, bool hasErrors = false)
            : base(BoundKind.ConstructorMethodBody, syntax, blockBody, expressionBody, hasErrors || initializer.HasErrors() || blockBody.HasErrors() || expressionBody.HasErrors())
        {


            this.Locals = locals;
            this.Initializer = initializer;
        }


        public ImmutableArray<LocalSymbol> Locals { get; }

        public BoundExpressionStatement? Initializer { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitConstructorMethodBody(this);

        public BoundConstructorMethodBody Update(ImmutableArray<LocalSymbol> locals, BoundExpressionStatement? initializer, BoundBlock? blockBody, BoundBlock? expressionBody)
        {
            if (locals != this.Locals || initializer != this.Initializer || blockBody != this.BlockBody || expressionBody != this.ExpressionBody)
            {
                var result = new BoundConstructorMethodBody(this.Syntax, locals, initializer, blockBody, expressionBody, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundExpressionWithNullability : BoundExpression
    {
        public BoundExpressionWithNullability(SyntaxNode syntax, BoundExpression expression, NullableAnnotation nullableAnnotation, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.ExpressionWithNullability, syntax, type, hasErrors || expression.HasErrors())
        {


            this.Expression = expression;
            this.NullableAnnotation = nullableAnnotation;
        }


        public BoundExpression Expression { get; }

        public new TypeSymbol? Type => base.Type;

        public NullableAnnotation NullableAnnotation { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitExpressionWithNullability(this);

        public BoundExpressionWithNullability Update(BoundExpression expression, NullableAnnotation nullableAnnotation, TypeSymbol? type)
        {
            if (expression != this.Expression || nullableAnnotation != this.NullableAnnotation || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundExpressionWithNullability(this.Syntax, expression, nullableAnnotation, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    public sealed partial class BoundWithExpression : BoundExpression
    {
        public BoundWithExpression(SyntaxNode syntax, BoundExpression receiver, MethodSymbol? cloneMethod, BoundObjectInitializerExpressionBase initializerExpression, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.WithExpression, syntax, type, hasErrors || receiver.HasErrors() || initializerExpression.HasErrors())
        {


            this.Receiver = receiver;
            this.CloneMethod = cloneMethod;
            this.InitializerExpression = initializerExpression;
        }


        public new TypeSymbol Type => base.Type!;

        public BoundExpression Receiver { get; }

        public MethodSymbol? CloneMethod { get; }

        public BoundObjectInitializerExpressionBase InitializerExpression { get; }
        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor) => visitor.VisitWithExpression(this);

        public BoundWithExpression Update(BoundExpression receiver, MethodSymbol? cloneMethod, BoundObjectInitializerExpressionBase initializerExpression, TypeSymbol type)
        {
            if (receiver != this.Receiver || !Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(cloneMethod, this.CloneMethod) || initializerExpression != this.InitializerExpression || !TypeSymbol.Equals(type, this.Type, TypeCompareKind.ConsiderEverything))
            {
                var result = new BoundWithExpression(this.Syntax, receiver, cloneMethod, initializerExpression, type, this.HasErrors);
                result.CopyAttributes(this);
                return result;
            }
            return this;
        }
    }

    internal abstract partial class BoundTreeVisitor<A, R>
    {

        [MethodImpl(MethodImplOptions.NoInlining), DebuggerStepThrough]
        internal R VisitInternal(BoundNode node, A arg)
        {
            switch (node.Kind)
            {
                case BoundKind.FieldEqualsValue:
                    return VisitFieldEqualsValue((BoundFieldEqualsValue)node, arg);
                case BoundKind.PropertyEqualsValue:
                    return VisitPropertyEqualsValue((BoundPropertyEqualsValue)node, arg);
                case BoundKind.ParameterEqualsValue:
                    return VisitParameterEqualsValue((BoundParameterEqualsValue)node, arg);
                case BoundKind.GlobalStatementInitializer:
                    return VisitGlobalStatementInitializer((BoundGlobalStatementInitializer)node, arg);
                case BoundKind.DeconstructValuePlaceholder:
                    return VisitDeconstructValuePlaceholder((BoundDeconstructValuePlaceholder)node, arg);
                case BoundKind.TupleOperandPlaceholder:
                    return VisitTupleOperandPlaceholder((BoundTupleOperandPlaceholder)node, arg);
                case BoundKind.AwaitableValuePlaceholder:
                    return VisitAwaitableValuePlaceholder((BoundAwaitableValuePlaceholder)node, arg);
                case BoundKind.DisposableValuePlaceholder:
                    return VisitDisposableValuePlaceholder((BoundDisposableValuePlaceholder)node, arg);
                case BoundKind.ObjectOrCollectionValuePlaceholder:
                    return VisitObjectOrCollectionValuePlaceholder((BoundObjectOrCollectionValuePlaceholder)node, arg);
                case BoundKind.Dup:
                    return VisitDup((BoundDup)node, arg);
                case BoundKind.PassByCopy:
                    return VisitPassByCopy((BoundPassByCopy)node, arg);
                case BoundKind.BadExpression:
                    return VisitBadExpression((BoundBadExpression)node, arg);
                case BoundKind.BadStatement:
                    return VisitBadStatement((BoundBadStatement)node, arg);
                case BoundKind.ExtractedFinallyBlock:
                    return VisitExtractedFinallyBlock((BoundExtractedFinallyBlock)node, arg);
                case BoundKind.TypeExpression:
                    return VisitTypeExpression((BoundTypeExpression)node, arg);
                case BoundKind.TypeOrValueExpression:
                    return VisitTypeOrValueExpression((BoundTypeOrValueExpression)node, arg);
                case BoundKind.NamespaceExpression:
                    return VisitNamespaceExpression((BoundNamespaceExpression)node, arg);
                case BoundKind.UnaryOperator:
                    return VisitUnaryOperator((BoundUnaryOperator)node, arg);
                case BoundKind.IncrementOperator:
                    return VisitIncrementOperator((BoundIncrementOperator)node, arg);
                case BoundKind.AddressOfOperator:
                    return VisitAddressOfOperator((BoundAddressOfOperator)node, arg);
                case BoundKind.UnconvertedAddressOfOperator:
                    return VisitUnconvertedAddressOfOperator((BoundUnconvertedAddressOfOperator)node, arg);
                case BoundKind.FunctionPointerLoad:
                    return VisitFunctionPointerLoad((BoundFunctionPointerLoad)node, arg);
                case BoundKind.PointerIndirectionOperator:
                    return VisitPointerIndirectionOperator((BoundPointerIndirectionOperator)node, arg);
                case BoundKind.PointerElementAccess:
                    return VisitPointerElementAccess((BoundPointerElementAccess)node, arg);
                case BoundKind.FunctionPointerInvocation:
                    return VisitFunctionPointerInvocation((BoundFunctionPointerInvocation)node, arg);
                case BoundKind.RefTypeOperator:
                    return VisitRefTypeOperator((BoundRefTypeOperator)node, arg);
                case BoundKind.MakeRefOperator:
                    return VisitMakeRefOperator((BoundMakeRefOperator)node, arg);
                case BoundKind.RefValueOperator:
                    return VisitRefValueOperator((BoundRefValueOperator)node, arg);
                case BoundKind.FromEndIndexExpression:
                    return VisitFromEndIndexExpression((BoundFromEndIndexExpression)node, arg);
                case BoundKind.RangeExpression:
                    return VisitRangeExpression((BoundRangeExpression)node, arg);
                case BoundKind.BinaryOperator:
                    return VisitBinaryOperator((BoundBinaryOperator)node, arg);
                case BoundKind.TupleBinaryOperator:
                    return VisitTupleBinaryOperator((BoundTupleBinaryOperator)node, arg);
                case BoundKind.UserDefinedConditionalLogicalOperator:
                    return VisitUserDefinedConditionalLogicalOperator((BoundUserDefinedConditionalLogicalOperator)node, arg);
                case BoundKind.CompoundAssignmentOperator:
                    return VisitCompoundAssignmentOperator((BoundCompoundAssignmentOperator)node, arg);
                case BoundKind.AssignmentOperator:
                    return VisitAssignmentOperator((BoundAssignmentOperator)node, arg);
                case BoundKind.DeconstructionAssignmentOperator:
                    return VisitDeconstructionAssignmentOperator((BoundDeconstructionAssignmentOperator)node, arg);
                case BoundKind.NullCoalescingOperator:
                    return VisitNullCoalescingOperator((BoundNullCoalescingOperator)node, arg);
                case BoundKind.NullCoalescingAssignmentOperator:
                    return VisitNullCoalescingAssignmentOperator((BoundNullCoalescingAssignmentOperator)node, arg);
                case BoundKind.UnconvertedConditionalOperator:
                    return VisitUnconvertedConditionalOperator((BoundUnconvertedConditionalOperator)node, arg);
                case BoundKind.ConditionalOperator:
                    return VisitConditionalOperator((BoundConditionalOperator)node, arg);
                case BoundKind.ArrayAccess:
                    return VisitArrayAccess((BoundArrayAccess)node, arg);
                case BoundKind.ArrayLength:
                    return VisitArrayLength((BoundArrayLength)node, arg);
                case BoundKind.AwaitableInfo:
                    return VisitAwaitableInfo((BoundAwaitableInfo)node, arg);
                case BoundKind.AwaitExpression:
                    return VisitAwaitExpression((BoundAwaitExpression)node, arg);
                case BoundKind.TypeOfOperator:
                    return VisitTypeOfOperator((BoundTypeOfOperator)node, arg);
                case BoundKind.MethodDefIndex:
                    return VisitMethodDefIndex((BoundMethodDefIndex)node, arg);
                case BoundKind.MaximumMethodDefIndex:
                    return VisitMaximumMethodDefIndex((BoundMaximumMethodDefIndex)node, arg);
                case BoundKind.InstrumentationPayloadRoot:
                    return VisitInstrumentationPayloadRoot((BoundInstrumentationPayloadRoot)node, arg);
                case BoundKind.ModuleVersionId:
                    return VisitModuleVersionId((BoundModuleVersionId)node, arg);
                case BoundKind.ModuleVersionIdString:
                    return VisitModuleVersionIdString((BoundModuleVersionIdString)node, arg);
                case BoundKind.SourceDocumentIndex:
                    return VisitSourceDocumentIndex((BoundSourceDocumentIndex)node, arg);
                case BoundKind.MethodInfo:
                    return VisitMethodInfo((BoundMethodInfo)node, arg);
                case BoundKind.FieldInfo:
                    return VisitFieldInfo((BoundFieldInfo)node, arg);
                case BoundKind.DefaultLiteral:
                    return VisitDefaultLiteral((BoundDefaultLiteral)node, arg);
                case BoundKind.DefaultExpression:
                    return VisitDefaultExpression((BoundDefaultExpression)node, arg);
                case BoundKind.IsOperator:
                    return VisitIsOperator((BoundIsOperator)node, arg);
                case BoundKind.AsOperator:
                    return VisitAsOperator((BoundAsOperator)node, arg);
                case BoundKind.SizeOfOperator:
                    return VisitSizeOfOperator((BoundSizeOfOperator)node, arg);
                case BoundKind.Conversion:
                    return VisitConversion((BoundConversion)node, arg);
                case BoundKind.ReadOnlySpanFromArray:
                    return VisitReadOnlySpanFromArray((BoundReadOnlySpanFromArray)node, arg);
                case BoundKind.ArgList:
                    return VisitArgList((BoundArgList)node, arg);
                case BoundKind.ArgListOperator:
                    return VisitArgListOperator((BoundArgListOperator)node, arg);
                case BoundKind.FixedLocalCollectionInitializer:
                    return VisitFixedLocalCollectionInitializer((BoundFixedLocalCollectionInitializer)node, arg);
                case BoundKind.SequencePoint:
                    return VisitSequencePoint((BoundSequencePoint)node, arg);
                case BoundKind.SequencePointWithSpan:
                    return VisitSequencePointWithSpan((BoundSequencePointWithSpan)node, arg);
                case BoundKind.SavePreviousSequencePoint:
                    return VisitSavePreviousSequencePoint((BoundSavePreviousSequencePoint)node, arg);
                case BoundKind.RestorePreviousSequencePoint:
                    return VisitRestorePreviousSequencePoint((BoundRestorePreviousSequencePoint)node, arg);
                case BoundKind.StepThroughSequencePoint:
                    return VisitStepThroughSequencePoint((BoundStepThroughSequencePoint)node, arg);
                case BoundKind.Block:
                    return VisitBlock((BoundBlock)node, arg);
                case BoundKind.Scope:
                    return VisitScope((BoundScope)node, arg);
                case BoundKind.StateMachineScope:
                    return VisitStateMachineScope((BoundStateMachineScope)node, arg);
                case BoundKind.LocalDeclaration:
                    return VisitLocalDeclaration((BoundLocalDeclaration)node, arg);
                case BoundKind.MultipleLocalDeclarations:
                    return VisitMultipleLocalDeclarations((BoundMultipleLocalDeclarations)node, arg);
                case BoundKind.UsingLocalDeclarations:
                    return VisitUsingLocalDeclarations((BoundUsingLocalDeclarations)node, arg);
                case BoundKind.LocalFunctionStatement:
                    return VisitLocalFunctionStatement((BoundLocalFunctionStatement)node, arg);
                case BoundKind.NoOpStatement:
                    return VisitNoOpStatement((BoundNoOpStatement)node, arg);
                case BoundKind.ReturnStatement:
                    return VisitReturnStatement((BoundReturnStatement)node, arg);
                case BoundKind.YieldReturnStatement:
                    return VisitYieldReturnStatement((BoundYieldReturnStatement)node, arg);
                case BoundKind.YieldBreakStatement:
                    return VisitYieldBreakStatement((BoundYieldBreakStatement)node, arg);
                case BoundKind.ThrowStatement:
                    return VisitThrowStatement((BoundThrowStatement)node, arg);
                case BoundKind.ExpressionStatement:
                    return VisitExpressionStatement((BoundExpressionStatement)node, arg);
                case BoundKind.BreakStatement:
                    return VisitBreakStatement((BoundBreakStatement)node, arg);
                case BoundKind.ContinueStatement:
                    return VisitContinueStatement((BoundContinueStatement)node, arg);
                case BoundKind.SwitchStatement:
                    return VisitSwitchStatement((BoundSwitchStatement)node, arg);
                case BoundKind.SwitchDispatch:
                    return VisitSwitchDispatch((BoundSwitchDispatch)node, arg);
                case BoundKind.IfStatement:
                    return VisitIfStatement((BoundIfStatement)node, arg);
                case BoundKind.DoStatement:
                    return VisitDoStatement((BoundDoStatement)node, arg);
                case BoundKind.WhileStatement:
                    return VisitWhileStatement((BoundWhileStatement)node, arg);
                case BoundKind.ForStatement:
                    return VisitForStatement((BoundForStatement)node, arg);
                case BoundKind.ForEachStatement:
                    return VisitForEachStatement((BoundForEachStatement)node, arg);
                case BoundKind.ForEachDeconstructStep:
                    return VisitForEachDeconstructStep((BoundForEachDeconstructStep)node, arg);
                case BoundKind.UsingStatement:
                    return VisitUsingStatement((BoundUsingStatement)node, arg);
                case BoundKind.FixedStatement:
                    return VisitFixedStatement((BoundFixedStatement)node, arg);
                case BoundKind.LockStatement:
                    return VisitLockStatement((BoundLockStatement)node, arg);
                case BoundKind.TryStatement:
                    return VisitTryStatement((BoundTryStatement)node, arg);
                case BoundKind.CatchBlock:
                    return VisitCatchBlock((BoundCatchBlock)node, arg);
                case BoundKind.Literal:
                    return VisitLiteral((BoundLiteral)node, arg);
                case BoundKind.ThisReference:
                    return VisitThisReference((BoundThisReference)node, arg);
                case BoundKind.PreviousSubmissionReference:
                    return VisitPreviousSubmissionReference((BoundPreviousSubmissionReference)node, arg);
                case BoundKind.HostObjectMemberReference:
                    return VisitHostObjectMemberReference((BoundHostObjectMemberReference)node, arg);
                case BoundKind.BaseReference:
                    return VisitBaseReference((BoundBaseReference)node, arg);
                case BoundKind.Local:
                    return VisitLocal((BoundLocal)node, arg);
                case BoundKind.PseudoVariable:
                    return VisitPseudoVariable((BoundPseudoVariable)node, arg);
                case BoundKind.RangeVariable:
                    return VisitRangeVariable((BoundRangeVariable)node, arg);
                case BoundKind.Parameter:
                    return VisitParameter((BoundParameter)node, arg);
                case BoundKind.LabelStatement:
                    return VisitLabelStatement((BoundLabelStatement)node, arg);
                case BoundKind.GotoStatement:
                    return VisitGotoStatement((BoundGotoStatement)node, arg);
                case BoundKind.LabeledStatement:
                    return VisitLabeledStatement((BoundLabeledStatement)node, arg);
                case BoundKind.Label:
                    return VisitLabel((BoundLabel)node, arg);
                case BoundKind.StatementList:
                    return VisitStatementList((BoundStatementList)node, arg);
                case BoundKind.ConditionalGoto:
                    return VisitConditionalGoto((BoundConditionalGoto)node, arg);
                case BoundKind.SwitchExpressionArm:
                    return VisitSwitchExpressionArm((BoundSwitchExpressionArm)node, arg);
                case BoundKind.UnconvertedSwitchExpression:
                    return VisitUnconvertedSwitchExpression((BoundUnconvertedSwitchExpression)node, arg);
                case BoundKind.ConvertedSwitchExpression:
                    return VisitConvertedSwitchExpression((BoundConvertedSwitchExpression)node, arg);
                case BoundKind.DecisionDag:
                    return VisitDecisionDag((BoundDecisionDag)node, arg);
                case BoundKind.EvaluationDecisionDagNode:
                    return VisitEvaluationDecisionDagNode((BoundEvaluationDecisionDagNode)node, arg);
                case BoundKind.TestDecisionDagNode:
                    return VisitTestDecisionDagNode((BoundTestDecisionDagNode)node, arg);
                case BoundKind.WhenDecisionDagNode:
                    return VisitWhenDecisionDagNode((BoundWhenDecisionDagNode)node, arg);
                case BoundKind.LeafDecisionDagNode:
                    return VisitLeafDecisionDagNode((BoundLeafDecisionDagNode)node, arg);
                case BoundKind.DagTemp:
                    return VisitDagTemp((BoundDagTemp)node, arg);
                case BoundKind.DagTypeTest:
                    return VisitDagTypeTest((BoundDagTypeTest)node, arg);
                case BoundKind.DagNonNullTest:
                    return VisitDagNonNullTest((BoundDagNonNullTest)node, arg);
                case BoundKind.DagExplicitNullTest:
                    return VisitDagExplicitNullTest((BoundDagExplicitNullTest)node, arg);
                case BoundKind.DagValueTest:
                    return VisitDagValueTest((BoundDagValueTest)node, arg);
                case BoundKind.DagRelationalTest:
                    return VisitDagRelationalTest((BoundDagRelationalTest)node, arg);
                case BoundKind.DagDeconstructEvaluation:
                    return VisitDagDeconstructEvaluation((BoundDagDeconstructEvaluation)node, arg);
                case BoundKind.DagTypeEvaluation:
                    return VisitDagTypeEvaluation((BoundDagTypeEvaluation)node, arg);
                case BoundKind.DagFieldEvaluation:
                    return VisitDagFieldEvaluation((BoundDagFieldEvaluation)node, arg);
                case BoundKind.DagPropertyEvaluation:
                    return VisitDagPropertyEvaluation((BoundDagPropertyEvaluation)node, arg);
                case BoundKind.DagIndexEvaluation:
                    return VisitDagIndexEvaluation((BoundDagIndexEvaluation)node, arg);
                case BoundKind.SwitchSection:
                    return VisitSwitchSection((BoundSwitchSection)node, arg);
                case BoundKind.SwitchLabel:
                    return VisitSwitchLabel((BoundSwitchLabel)node, arg);
                case BoundKind.SequencePointExpression:
                    return VisitSequencePointExpression((BoundSequencePointExpression)node, arg);
                case BoundKind.Sequence:
                    return VisitSequence((BoundSequence)node, arg);
                case BoundKind.SpillSequence:
                    return VisitSpillSequence((BoundSpillSequence)node, arg);
                case BoundKind.DynamicMemberAccess:
                    return VisitDynamicMemberAccess((BoundDynamicMemberAccess)node, arg);
                case BoundKind.DynamicInvocation:
                    return VisitDynamicInvocation((BoundDynamicInvocation)node, arg);
                case BoundKind.ConditionalAccess:
                    return VisitConditionalAccess((BoundConditionalAccess)node, arg);
                case BoundKind.LoweredConditionalAccess:
                    return VisitLoweredConditionalAccess((BoundLoweredConditionalAccess)node, arg);
                case BoundKind.ConditionalReceiver:
                    return VisitConditionalReceiver((BoundConditionalReceiver)node, arg);
                case BoundKind.ComplexConditionalReceiver:
                    return VisitComplexConditionalReceiver((BoundComplexConditionalReceiver)node, arg);
                case BoundKind.MethodGroup:
                    return VisitMethodGroup((BoundMethodGroup)node, arg);
                case BoundKind.PropertyGroup:
                    return VisitPropertyGroup((BoundPropertyGroup)node, arg);
                case BoundKind.Call:
                    return VisitCall((BoundCall)node, arg);
                case BoundKind.EventAssignmentOperator:
                    return VisitEventAssignmentOperator((BoundEventAssignmentOperator)node, arg);
                case BoundKind.Attribute:
                    return VisitAttribute((BoundAttribute)node, arg);
                case BoundKind.UnconvertedObjectCreationExpression:
                    return VisitUnconvertedObjectCreationExpression((BoundUnconvertedObjectCreationExpression)node, arg);
                case BoundKind.ObjectCreationExpression:
                    return VisitObjectCreationExpression((BoundObjectCreationExpression)node, arg);
                case BoundKind.TupleLiteral:
                    return VisitTupleLiteral((BoundTupleLiteral)node, arg);
                case BoundKind.ConvertedTupleLiteral:
                    return VisitConvertedTupleLiteral((BoundConvertedTupleLiteral)node, arg);
                case BoundKind.DynamicObjectCreationExpression:
                    return VisitDynamicObjectCreationExpression((BoundDynamicObjectCreationExpression)node, arg);
                case BoundKind.NoPiaObjectCreationExpression:
                    return VisitNoPiaObjectCreationExpression((BoundNoPiaObjectCreationExpression)node, arg);
                case BoundKind.ObjectInitializerExpression:
                    return VisitObjectInitializerExpression((BoundObjectInitializerExpression)node, arg);
                case BoundKind.ObjectInitializerMember:
                    return VisitObjectInitializerMember((BoundObjectInitializerMember)node, arg);
                case BoundKind.DynamicObjectInitializerMember:
                    return VisitDynamicObjectInitializerMember((BoundDynamicObjectInitializerMember)node, arg);
                case BoundKind.CollectionInitializerExpression:
                    return VisitCollectionInitializerExpression((BoundCollectionInitializerExpression)node, arg);
                case BoundKind.CollectionElementInitializer:
                    return VisitCollectionElementInitializer((BoundCollectionElementInitializer)node, arg);
                case BoundKind.DynamicCollectionElementInitializer:
                    return VisitDynamicCollectionElementInitializer((BoundDynamicCollectionElementInitializer)node, arg);
                case BoundKind.ImplicitReceiver:
                    return VisitImplicitReceiver((BoundImplicitReceiver)node, arg);
                case BoundKind.AnonymousObjectCreationExpression:
                    return VisitAnonymousObjectCreationExpression((BoundAnonymousObjectCreationExpression)node, arg);
                case BoundKind.AnonymousPropertyDeclaration:
                    return VisitAnonymousPropertyDeclaration((BoundAnonymousPropertyDeclaration)node, arg);
                case BoundKind.NewT:
                    return VisitNewT((BoundNewT)node, arg);
                case BoundKind.DelegateCreationExpression:
                    return VisitDelegateCreationExpression((BoundDelegateCreationExpression)node, arg);
                case BoundKind.ArrayCreation:
                    return VisitArrayCreation((BoundArrayCreation)node, arg);
                case BoundKind.ArrayInitialization:
                    return VisitArrayInitialization((BoundArrayInitialization)node, arg);
                case BoundKind.StackAllocArrayCreation:
                    return VisitStackAllocArrayCreation((BoundStackAllocArrayCreation)node, arg);
                case BoundKind.ConvertedStackAllocExpression:
                    return VisitConvertedStackAllocExpression((BoundConvertedStackAllocExpression)node, arg);
                case BoundKind.FieldAccess:
                    return VisitFieldAccess((BoundFieldAccess)node, arg);
                case BoundKind.HoistedFieldAccess:
                    return VisitHoistedFieldAccess((BoundHoistedFieldAccess)node, arg);
                case BoundKind.PropertyAccess:
                    return VisitPropertyAccess((BoundPropertyAccess)node, arg);
                case BoundKind.EventAccess:
                    return VisitEventAccess((BoundEventAccess)node, arg);
                case BoundKind.IndexerAccess:
                    return VisitIndexerAccess((BoundIndexerAccess)node, arg);
                case BoundKind.IndexOrRangePatternIndexerAccess:
                    return VisitIndexOrRangePatternIndexerAccess((BoundIndexOrRangePatternIndexerAccess)node, arg);
                case BoundKind.DynamicIndexerAccess:
                    return VisitDynamicIndexerAccess((BoundDynamicIndexerAccess)node, arg);
                case BoundKind.Lambda:
                    return VisitLambda((BoundLambda)node, arg);
                case BoundKind.UnboundLambda:
                    return VisitUnboundLambda((UnboundLambda)node, arg);
                case BoundKind.QueryClause:
                    return VisitQueryClause((BoundQueryClause)node, arg);
                case BoundKind.TypeOrInstanceInitializers:
                    return VisitTypeOrInstanceInitializers((BoundTypeOrInstanceInitializers)node, arg);
                case BoundKind.NameOfOperator:
                    return VisitNameOfOperator((BoundNameOfOperator)node, arg);
                case BoundKind.UnconvertedInterpolatedString:
                    return VisitUnconvertedInterpolatedString((BoundUnconvertedInterpolatedString)node, arg);
                case BoundKind.InterpolatedString:
                    return VisitInterpolatedString((BoundInterpolatedString)node, arg);
                case BoundKind.StringInsert:
                    return VisitStringInsert((BoundStringInsert)node, arg);
                case BoundKind.IsPatternExpression:
                    return VisitIsPatternExpression((BoundIsPatternExpression)node, arg);
                case BoundKind.ConstantPattern:
                    return VisitConstantPattern((BoundConstantPattern)node, arg);
                case BoundKind.DiscardPattern:
                    return VisitDiscardPattern((BoundDiscardPattern)node, arg);
                case BoundKind.DeclarationPattern:
                    return VisitDeclarationPattern((BoundDeclarationPattern)node, arg);
                case BoundKind.RecursivePattern:
                    return VisitRecursivePattern((BoundRecursivePattern)node, arg);
                case BoundKind.ITuplePattern:
                    return VisitITuplePattern((BoundITuplePattern)node, arg);
                case BoundKind.Subpattern:
                    return VisitSubpattern((BoundSubpattern)node, arg);
                case BoundKind.TypePattern:
                    return VisitTypePattern((BoundTypePattern)node, arg);
                case BoundKind.BinaryPattern:
                    return VisitBinaryPattern((BoundBinaryPattern)node, arg);
                case BoundKind.NegatedPattern:
                    return VisitNegatedPattern((BoundNegatedPattern)node, arg);
                case BoundKind.RelationalPattern:
                    return VisitRelationalPattern((BoundRelationalPattern)node, arg);
                case BoundKind.DiscardExpression:
                    return VisitDiscardExpression((BoundDiscardExpression)node, arg);
                case BoundKind.ThrowExpression:
                    return VisitThrowExpression((BoundThrowExpression)node, arg);
                case BoundKind.OutVariablePendingInference:
                    return VisitOutVariablePendingInference((OutVariablePendingInference)node, arg);
                case BoundKind.DeconstructionVariablePendingInference:
                    return VisitDeconstructionVariablePendingInference((DeconstructionVariablePendingInference)node, arg);
                case BoundKind.OutDeconstructVarPendingInference:
                    return VisitOutDeconstructVarPendingInference((OutDeconstructVarPendingInference)node, arg);
                case BoundKind.NonConstructorMethodBody:
                    return VisitNonConstructorMethodBody((BoundNonConstructorMethodBody)node, arg);
                case BoundKind.ConstructorMethodBody:
                    return VisitConstructorMethodBody((BoundConstructorMethodBody)node, arg);
                case BoundKind.ExpressionWithNullability:
                    return VisitExpressionWithNullability((BoundExpressionWithNullability)node, arg);
                case BoundKind.WithExpression:
                    return VisitWithExpression((BoundWithExpression)node, arg);
            }

            return default(R)!;
        }
    }

    internal abstract partial class BoundTreeVisitor<A, R>
    {
        public virtual R VisitFieldEqualsValue(BoundFieldEqualsValue node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitPropertyEqualsValue(BoundPropertyEqualsValue node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitParameterEqualsValue(BoundParameterEqualsValue node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDisposableValuePlaceholder(BoundDisposableValuePlaceholder node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDup(BoundDup node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitPassByCopy(BoundPassByCopy node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitBadExpression(BoundBadExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitBadStatement(BoundBadStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitExtractedFinallyBlock(BoundExtractedFinallyBlock node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitTypeExpression(BoundTypeExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitTypeOrValueExpression(BoundTypeOrValueExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitNamespaceExpression(BoundNamespaceExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitUnaryOperator(BoundUnaryOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitIncrementOperator(BoundIncrementOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitAddressOfOperator(BoundAddressOfOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitFunctionPointerLoad(BoundFunctionPointerLoad node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitPointerElementAccess(BoundPointerElementAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitRefTypeOperator(BoundRefTypeOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitMakeRefOperator(BoundMakeRefOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitRefValueOperator(BoundRefValueOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitFromEndIndexExpression(BoundFromEndIndexExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitRangeExpression(BoundRangeExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitBinaryOperator(BoundBinaryOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitTupleBinaryOperator(BoundTupleBinaryOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitAssignmentOperator(BoundAssignmentOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitNullCoalescingOperator(BoundNullCoalescingOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitConditionalOperator(BoundConditionalOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitArrayAccess(BoundArrayAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitArrayLength(BoundArrayLength node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitAwaitableInfo(BoundAwaitableInfo node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitAwaitExpression(BoundAwaitExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitTypeOfOperator(BoundTypeOfOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitMethodDefIndex(BoundMethodDefIndex node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitModuleVersionId(BoundModuleVersionId node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitModuleVersionIdString(BoundModuleVersionIdString node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSourceDocumentIndex(BoundSourceDocumentIndex node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitMethodInfo(BoundMethodInfo node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitFieldInfo(BoundFieldInfo node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDefaultLiteral(BoundDefaultLiteral node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDefaultExpression(BoundDefaultExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitIsOperator(BoundIsOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitAsOperator(BoundAsOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSizeOfOperator(BoundSizeOfOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitConversion(BoundConversion node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitArgList(BoundArgList node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitArgListOperator(BoundArgListOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSequencePoint(BoundSequencePoint node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSequencePointWithSpan(BoundSequencePointWithSpan node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSavePreviousSequencePoint(BoundSavePreviousSequencePoint node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitRestorePreviousSequencePoint(BoundRestorePreviousSequencePoint node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitStepThroughSequencePoint(BoundStepThroughSequencePoint node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitBlock(BoundBlock node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitScope(BoundScope node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitStateMachineScope(BoundStateMachineScope node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLocalDeclaration(BoundLocalDeclaration node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLocalFunctionStatement(BoundLocalFunctionStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitNoOpStatement(BoundNoOpStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitReturnStatement(BoundReturnStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitYieldReturnStatement(BoundYieldReturnStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitYieldBreakStatement(BoundYieldBreakStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitThrowStatement(BoundThrowStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitExpressionStatement(BoundExpressionStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitBreakStatement(BoundBreakStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitContinueStatement(BoundContinueStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSwitchStatement(BoundSwitchStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSwitchDispatch(BoundSwitchDispatch node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitIfStatement(BoundIfStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDoStatement(BoundDoStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitWhileStatement(BoundWhileStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitForStatement(BoundForStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitForEachStatement(BoundForEachStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitForEachDeconstructStep(BoundForEachDeconstructStep node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitUsingStatement(BoundUsingStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitFixedStatement(BoundFixedStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLockStatement(BoundLockStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitTryStatement(BoundTryStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitCatchBlock(BoundCatchBlock node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLiteral(BoundLiteral node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitThisReference(BoundThisReference node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitHostObjectMemberReference(BoundHostObjectMemberReference node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitBaseReference(BoundBaseReference node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLocal(BoundLocal node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitPseudoVariable(BoundPseudoVariable node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitRangeVariable(BoundRangeVariable node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitParameter(BoundParameter node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLabelStatement(BoundLabelStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitGotoStatement(BoundGotoStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLabeledStatement(BoundLabeledStatement node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLabel(BoundLabel node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitStatementList(BoundStatementList node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitConditionalGoto(BoundConditionalGoto node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSwitchExpressionArm(BoundSwitchExpressionArm node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDecisionDag(BoundDecisionDag node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitEvaluationDecisionDagNode(BoundEvaluationDecisionDagNode node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitTestDecisionDagNode(BoundTestDecisionDagNode node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitWhenDecisionDagNode(BoundWhenDecisionDagNode node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLeafDecisionDagNode(BoundLeafDecisionDagNode node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagTemp(BoundDagTemp node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagTypeTest(BoundDagTypeTest node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagNonNullTest(BoundDagNonNullTest node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagExplicitNullTest(BoundDagExplicitNullTest node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagValueTest(BoundDagValueTest node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagRelationalTest(BoundDagRelationalTest node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagDeconstructEvaluation(BoundDagDeconstructEvaluation node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagTypeEvaluation(BoundDagTypeEvaluation node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagFieldEvaluation(BoundDagFieldEvaluation node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagPropertyEvaluation(BoundDagPropertyEvaluation node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDagIndexEvaluation(BoundDagIndexEvaluation node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSwitchSection(BoundSwitchSection node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSwitchLabel(BoundSwitchLabel node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSequencePointExpression(BoundSequencePointExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSequence(BoundSequence node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSpillSequence(BoundSpillSequence node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDynamicMemberAccess(BoundDynamicMemberAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDynamicInvocation(BoundDynamicInvocation node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitConditionalAccess(BoundConditionalAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitConditionalReceiver(BoundConditionalReceiver node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitMethodGroup(BoundMethodGroup node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitPropertyGroup(BoundPropertyGroup node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitCall(BoundCall node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitEventAssignmentOperator(BoundEventAssignmentOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitAttribute(BoundAttribute node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitObjectCreationExpression(BoundObjectCreationExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitTupleLiteral(BoundTupleLiteral node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitObjectInitializerExpression(BoundObjectInitializerExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitObjectInitializerMember(BoundObjectInitializerMember node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitCollectionElementInitializer(BoundCollectionElementInitializer node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitImplicitReceiver(BoundImplicitReceiver node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitNewT(BoundNewT node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDelegateCreationExpression(BoundDelegateCreationExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitArrayCreation(BoundArrayCreation node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitArrayInitialization(BoundArrayInitialization node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitFieldAccess(BoundFieldAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitHoistedFieldAccess(BoundHoistedFieldAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitPropertyAccess(BoundPropertyAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitEventAccess(BoundEventAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitIndexerAccess(BoundIndexerAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitLambda(BoundLambda node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitUnboundLambda(UnboundLambda node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitQueryClause(BoundQueryClause node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitNameOfOperator(BoundNameOfOperator node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitInterpolatedString(BoundInterpolatedString node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitStringInsert(BoundStringInsert node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitIsPatternExpression(BoundIsPatternExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitConstantPattern(BoundConstantPattern node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDiscardPattern(BoundDiscardPattern node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDeclarationPattern(BoundDeclarationPattern node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitRecursivePattern(BoundRecursivePattern node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitITuplePattern(BoundITuplePattern node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitSubpattern(BoundSubpattern node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitTypePattern(BoundTypePattern node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitBinaryPattern(BoundBinaryPattern node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitNegatedPattern(BoundNegatedPattern node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitRelationalPattern(BoundRelationalPattern node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDiscardExpression(BoundDiscardExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitThrowExpression(BoundThrowExpression node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitOutVariablePendingInference(OutVariablePendingInference node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitConstructorMethodBody(BoundConstructorMethodBody node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitExpressionWithNullability(BoundExpressionWithNullability node, A arg) => this.DefaultVisit(node, arg);
        public virtual R VisitWithExpression(BoundWithExpression node, A arg) => this.DefaultVisit(node, arg);
    }

    public abstract partial class BoundTreeVisitor
    {
        public virtual BoundNode? VisitFieldEqualsValue(BoundFieldEqualsValue node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitPropertyEqualsValue(BoundPropertyEqualsValue node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitParameterEqualsValue(BoundParameterEqualsValue node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDisposableValuePlaceholder(BoundDisposableValuePlaceholder node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDup(BoundDup node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitPassByCopy(BoundPassByCopy node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitBadExpression(BoundBadExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitBadStatement(BoundBadStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitExtractedFinallyBlock(BoundExtractedFinallyBlock node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitTypeExpression(BoundTypeExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitTypeOrValueExpression(BoundTypeOrValueExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitNamespaceExpression(BoundNamespaceExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitUnaryOperator(BoundUnaryOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitIncrementOperator(BoundIncrementOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitAddressOfOperator(BoundAddressOfOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitFunctionPointerLoad(BoundFunctionPointerLoad node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitPointerElementAccess(BoundPointerElementAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitRefTypeOperator(BoundRefTypeOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitMakeRefOperator(BoundMakeRefOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitRefValueOperator(BoundRefValueOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitFromEndIndexExpression(BoundFromEndIndexExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitRangeExpression(BoundRangeExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitBinaryOperator(BoundBinaryOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitTupleBinaryOperator(BoundTupleBinaryOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitAssignmentOperator(BoundAssignmentOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitNullCoalescingOperator(BoundNullCoalescingOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitConditionalOperator(BoundConditionalOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitArrayAccess(BoundArrayAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitArrayLength(BoundArrayLength node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitAwaitableInfo(BoundAwaitableInfo node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitAwaitExpression(BoundAwaitExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitTypeOfOperator(BoundTypeOfOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitMethodDefIndex(BoundMethodDefIndex node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitModuleVersionId(BoundModuleVersionId node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitModuleVersionIdString(BoundModuleVersionIdString node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSourceDocumentIndex(BoundSourceDocumentIndex node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitMethodInfo(BoundMethodInfo node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitFieldInfo(BoundFieldInfo node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDefaultLiteral(BoundDefaultLiteral node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDefaultExpression(BoundDefaultExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitIsOperator(BoundIsOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitAsOperator(BoundAsOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSizeOfOperator(BoundSizeOfOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitConversion(BoundConversion node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitArgList(BoundArgList node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitArgListOperator(BoundArgListOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSequencePoint(BoundSequencePoint node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSequencePointWithSpan(BoundSequencePointWithSpan node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSavePreviousSequencePoint(BoundSavePreviousSequencePoint node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitRestorePreviousSequencePoint(BoundRestorePreviousSequencePoint node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitStepThroughSequencePoint(BoundStepThroughSequencePoint node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitBlock(BoundBlock node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitScope(BoundScope node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitStateMachineScope(BoundStateMachineScope node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLocalDeclaration(BoundLocalDeclaration node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLocalFunctionStatement(BoundLocalFunctionStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitNoOpStatement(BoundNoOpStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitReturnStatement(BoundReturnStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitYieldReturnStatement(BoundYieldReturnStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitYieldBreakStatement(BoundYieldBreakStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitThrowStatement(BoundThrowStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitExpressionStatement(BoundExpressionStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitBreakStatement(BoundBreakStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitContinueStatement(BoundContinueStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSwitchStatement(BoundSwitchStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSwitchDispatch(BoundSwitchDispatch node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitIfStatement(BoundIfStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDoStatement(BoundDoStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitWhileStatement(BoundWhileStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitForStatement(BoundForStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitForEachStatement(BoundForEachStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitForEachDeconstructStep(BoundForEachDeconstructStep node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitUsingStatement(BoundUsingStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitFixedStatement(BoundFixedStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLockStatement(BoundLockStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitTryStatement(BoundTryStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitCatchBlock(BoundCatchBlock node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLiteral(BoundLiteral node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitThisReference(BoundThisReference node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitHostObjectMemberReference(BoundHostObjectMemberReference node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitBaseReference(BoundBaseReference node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLocal(BoundLocal node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitPseudoVariable(BoundPseudoVariable node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitRangeVariable(BoundRangeVariable node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitParameter(BoundParameter node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLabelStatement(BoundLabelStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitGotoStatement(BoundGotoStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLabeledStatement(BoundLabeledStatement node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLabel(BoundLabel node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitStatementList(BoundStatementList node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitConditionalGoto(BoundConditionalGoto node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSwitchExpressionArm(BoundSwitchExpressionArm node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDecisionDag(BoundDecisionDag node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitEvaluationDecisionDagNode(BoundEvaluationDecisionDagNode node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitTestDecisionDagNode(BoundTestDecisionDagNode node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitWhenDecisionDagNode(BoundWhenDecisionDagNode node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLeafDecisionDagNode(BoundLeafDecisionDagNode node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagTemp(BoundDagTemp node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagTypeTest(BoundDagTypeTest node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagNonNullTest(BoundDagNonNullTest node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagExplicitNullTest(BoundDagExplicitNullTest node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagValueTest(BoundDagValueTest node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagRelationalTest(BoundDagRelationalTest node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagDeconstructEvaluation(BoundDagDeconstructEvaluation node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagTypeEvaluation(BoundDagTypeEvaluation node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagFieldEvaluation(BoundDagFieldEvaluation node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagPropertyEvaluation(BoundDagPropertyEvaluation node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDagIndexEvaluation(BoundDagIndexEvaluation node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSwitchSection(BoundSwitchSection node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSwitchLabel(BoundSwitchLabel node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSequencePointExpression(BoundSequencePointExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSequence(BoundSequence node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSpillSequence(BoundSpillSequence node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDynamicMemberAccess(BoundDynamicMemberAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDynamicInvocation(BoundDynamicInvocation node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitConditionalAccess(BoundConditionalAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitConditionalReceiver(BoundConditionalReceiver node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitMethodGroup(BoundMethodGroup node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitPropertyGroup(BoundPropertyGroup node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitCall(BoundCall node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitEventAssignmentOperator(BoundEventAssignmentOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitAttribute(BoundAttribute node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitObjectCreationExpression(BoundObjectCreationExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitTupleLiteral(BoundTupleLiteral node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitObjectInitializerExpression(BoundObjectInitializerExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitObjectInitializerMember(BoundObjectInitializerMember node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitCollectionElementInitializer(BoundCollectionElementInitializer node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitImplicitReceiver(BoundImplicitReceiver node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitNewT(BoundNewT node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDelegateCreationExpression(BoundDelegateCreationExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitArrayCreation(BoundArrayCreation node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitArrayInitialization(BoundArrayInitialization node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitFieldAccess(BoundFieldAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitHoistedFieldAccess(BoundHoistedFieldAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitPropertyAccess(BoundPropertyAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitEventAccess(BoundEventAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitIndexerAccess(BoundIndexerAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitLambda(BoundLambda node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitUnboundLambda(UnboundLambda node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitQueryClause(BoundQueryClause node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitNameOfOperator(BoundNameOfOperator node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitInterpolatedString(BoundInterpolatedString node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitStringInsert(BoundStringInsert node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitIsPatternExpression(BoundIsPatternExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitConstantPattern(BoundConstantPattern node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDiscardPattern(BoundDiscardPattern node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDeclarationPattern(BoundDeclarationPattern node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitRecursivePattern(BoundRecursivePattern node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitITuplePattern(BoundITuplePattern node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitSubpattern(BoundSubpattern node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitTypePattern(BoundTypePattern node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitBinaryPattern(BoundBinaryPattern node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitNegatedPattern(BoundNegatedPattern node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitRelationalPattern(BoundRelationalPattern node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDiscardExpression(BoundDiscardExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitThrowExpression(BoundThrowExpression node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitOutVariablePendingInference(OutVariablePendingInference node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitConstructorMethodBody(BoundConstructorMethodBody node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitExpressionWithNullability(BoundExpressionWithNullability node) => this.DefaultVisit(node);
        public virtual BoundNode? VisitWithExpression(BoundWithExpression node) => this.DefaultVisit(node);
    }

    internal abstract partial class BoundTreeWalker : BoundTreeVisitor
    {
        public override BoundNode? VisitFieldEqualsValue(BoundFieldEqualsValue node)
        {
            this.Visit(node.Value);
            return null;
        }
        public override BoundNode? VisitPropertyEqualsValue(BoundPropertyEqualsValue node)
        {
            this.Visit(node.Value);
            return null;
        }
        public override BoundNode? VisitParameterEqualsValue(BoundParameterEqualsValue node)
        {
            this.Visit(node.Value);
            return null;
        }
        public override BoundNode? VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node)
        {
            this.Visit(node.Statement);
            return null;
        }
        public override BoundNode? VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node) => null;
        public override BoundNode? VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node) => null;
        public override BoundNode? VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node) => null;
        public override BoundNode? VisitDisposableValuePlaceholder(BoundDisposableValuePlaceholder node) => null;
        public override BoundNode? VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node) => null;
        public override BoundNode? VisitDup(BoundDup node) => null;
        public override BoundNode? VisitPassByCopy(BoundPassByCopy node)
        {
            this.Visit(node.Expression);
            return null;
        }
        public override BoundNode? VisitBadExpression(BoundBadExpression node)
        {
            this.VisitList(node.ChildBoundNodes);
            return null;
        }
        public override BoundNode? VisitBadStatement(BoundBadStatement node)
        {
            this.VisitList(node.ChildBoundNodes);
            return null;
        }
        public override BoundNode? VisitExtractedFinallyBlock(BoundExtractedFinallyBlock node)
        {
            this.Visit(node.FinallyBlock);
            return null;
        }
        public override BoundNode? VisitTypeExpression(BoundTypeExpression node)
        {
            this.Visit(node.BoundContainingTypeOpt);
            this.VisitList(node.BoundDimensionsOpt);
            return null;
        }
        public override BoundNode? VisitTypeOrValueExpression(BoundTypeOrValueExpression node) => null;
        public override BoundNode? VisitNamespaceExpression(BoundNamespaceExpression node) => null;
        public override BoundNode? VisitUnaryOperator(BoundUnaryOperator node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitIncrementOperator(BoundIncrementOperator node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitAddressOfOperator(BoundAddressOfOperator node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitFunctionPointerLoad(BoundFunctionPointerLoad node) => null;
        public override BoundNode? VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitPointerElementAccess(BoundPointerElementAccess node)
        {
            this.Visit(node.Expression);
            this.Visit(node.Index);
            return null;
        }
        public override BoundNode? VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node)
        {
            this.Visit(node.InvokedExpression);
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitRefTypeOperator(BoundRefTypeOperator node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitMakeRefOperator(BoundMakeRefOperator node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitRefValueOperator(BoundRefValueOperator node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitFromEndIndexExpression(BoundFromEndIndexExpression node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitRangeExpression(BoundRangeExpression node)
        {
            this.Visit(node.LeftOperandOpt);
            this.Visit(node.RightOperandOpt);
            return null;
        }
        public override BoundNode? VisitBinaryOperator(BoundBinaryOperator node)
        {
            this.Visit(node.Left);
            this.Visit(node.Right);
            return null;
        }
        public override BoundNode? VisitTupleBinaryOperator(BoundTupleBinaryOperator node)
        {
            this.Visit(node.Left);
            this.Visit(node.Right);
            return null;
        }
        public override BoundNode? VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node)
        {
            this.Visit(node.Left);
            this.Visit(node.Right);
            return null;
        }
        public override BoundNode? VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            this.Visit(node.Left);
            this.Visit(node.Right);
            return null;
        }
        public override BoundNode? VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            this.Visit(node.Left);
            this.Visit(node.Right);
            return null;
        }
        public override BoundNode? VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            this.Visit(node.Left);
            this.Visit(node.Right);
            return null;
        }
        public override BoundNode? VisitNullCoalescingOperator(BoundNullCoalescingOperator node)
        {
            this.Visit(node.LeftOperand);
            this.Visit(node.RightOperand);
            return null;
        }
        public override BoundNode? VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node)
        {
            this.Visit(node.LeftOperand);
            this.Visit(node.RightOperand);
            return null;
        }
        public override BoundNode? VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node)
        {
            this.Visit(node.Condition);
            this.Visit(node.Consequence);
            this.Visit(node.Alternative);
            return null;
        }
        public override BoundNode? VisitConditionalOperator(BoundConditionalOperator node)
        {
            this.Visit(node.Condition);
            this.Visit(node.Consequence);
            this.Visit(node.Alternative);
            return null;
        }
        public override BoundNode? VisitArrayAccess(BoundArrayAccess node)
        {
            this.Visit(node.Expression);
            this.VisitList(node.Indices);
            return null;
        }
        public override BoundNode? VisitArrayLength(BoundArrayLength node)
        {
            this.Visit(node.Expression);
            return null;
        }
        public override BoundNode? VisitAwaitableInfo(BoundAwaitableInfo node)
        {
            this.Visit(node.AwaitableInstancePlaceholder);
            this.Visit(node.GetAwaiter);
            return null;
        }
        public override BoundNode? VisitAwaitExpression(BoundAwaitExpression node)
        {
            this.Visit(node.Expression);
            this.Visit(node.AwaitableInfo);
            return null;
        }
        public override BoundNode? VisitTypeOfOperator(BoundTypeOfOperator node)
        {
            this.Visit(node.SourceType);
            return null;
        }
        public override BoundNode? VisitMethodDefIndex(BoundMethodDefIndex node) => null;
        public override BoundNode? VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node) => null;
        public override BoundNode? VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node) => null;
        public override BoundNode? VisitModuleVersionId(BoundModuleVersionId node) => null;
        public override BoundNode? VisitModuleVersionIdString(BoundModuleVersionIdString node) => null;
        public override BoundNode? VisitSourceDocumentIndex(BoundSourceDocumentIndex node) => null;
        public override BoundNode? VisitMethodInfo(BoundMethodInfo node) => null;
        public override BoundNode? VisitFieldInfo(BoundFieldInfo node) => null;
        public override BoundNode? VisitDefaultLiteral(BoundDefaultLiteral node) => null;
        public override BoundNode? VisitDefaultExpression(BoundDefaultExpression node) => null;
        public override BoundNode? VisitIsOperator(BoundIsOperator node)
        {
            this.Visit(node.Operand);
            this.Visit(node.TargetType);
            return null;
        }
        public override BoundNode? VisitAsOperator(BoundAsOperator node)
        {
            this.Visit(node.Operand);
            this.Visit(node.TargetType);
            return null;
        }
        public override BoundNode? VisitSizeOfOperator(BoundSizeOfOperator node)
        {
            this.Visit(node.SourceType);
            return null;
        }
        public override BoundNode? VisitConversion(BoundConversion node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node)
        {
            this.Visit(node.Operand);
            return null;
        }
        public override BoundNode? VisitArgList(BoundArgList node) => null;
        public override BoundNode? VisitArgListOperator(BoundArgListOperator node)
        {
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node)
        {
            this.Visit(node.Expression);
            return null;
        }
        public override BoundNode? VisitSequencePoint(BoundSequencePoint node)
        {
            this.Visit(node.StatementOpt);
            return null;
        }
        public override BoundNode? VisitSequencePointWithSpan(BoundSequencePointWithSpan node)
        {
            this.Visit(node.StatementOpt);
            return null;
        }
        public override BoundNode? VisitSavePreviousSequencePoint(BoundSavePreviousSequencePoint node) => null;
        public override BoundNode? VisitRestorePreviousSequencePoint(BoundRestorePreviousSequencePoint node) => null;
        public override BoundNode? VisitStepThroughSequencePoint(BoundStepThroughSequencePoint node) => null;
        public override BoundNode? VisitBlock(BoundBlock node)
        {
            this.VisitList(node.Statements);
            return null;
        }
        public override BoundNode? VisitScope(BoundScope node)
        {
            this.VisitList(node.Statements);
            return null;
        }
        public override BoundNode? VisitStateMachineScope(BoundStateMachineScope node)
        {
            this.Visit(node.Statement);
            return null;
        }
        public override BoundNode? VisitLocalDeclaration(BoundLocalDeclaration node)
        {
            this.Visit(node.DeclaredTypeOpt);
            this.Visit(node.InitializerOpt);
            this.VisitList(node.ArgumentsOpt);
            return null;
        }
        public override BoundNode? VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node)
        {
            this.VisitList(node.LocalDeclarations);
            return null;
        }
        public override BoundNode? VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node)
        {
            this.Visit(node.AwaitOpt);
            this.VisitList(node.LocalDeclarations);
            return null;
        }
        public override BoundNode? VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            this.Visit(node.BlockBody);
            this.Visit(node.ExpressionBody);
            return null;
        }
        public override BoundNode? VisitNoOpStatement(BoundNoOpStatement node) => null;
        public override BoundNode? VisitReturnStatement(BoundReturnStatement node)
        {
            this.Visit(node.ExpressionOpt);
            return null;
        }
        public override BoundNode? VisitYieldReturnStatement(BoundYieldReturnStatement node)
        {
            this.Visit(node.Expression);
            return null;
        }
        public override BoundNode? VisitYieldBreakStatement(BoundYieldBreakStatement node) => null;
        public override BoundNode? VisitThrowStatement(BoundThrowStatement node)
        {
            this.Visit(node.ExpressionOpt);
            return null;
        }
        public override BoundNode? VisitExpressionStatement(BoundExpressionStatement node)
        {
            this.Visit(node.Expression);
            return null;
        }
        public override BoundNode? VisitBreakStatement(BoundBreakStatement node) => null;
        public override BoundNode? VisitContinueStatement(BoundContinueStatement node) => null;
        public override BoundNode? VisitSwitchStatement(BoundSwitchStatement node)
        {
            this.Visit(node.Expression);
            this.VisitList(node.SwitchSections);
            this.Visit(node.DefaultLabel);
            return null;
        }
        public override BoundNode? VisitSwitchDispatch(BoundSwitchDispatch node)
        {
            this.Visit(node.Expression);
            return null;
        }
        public override BoundNode? VisitIfStatement(BoundIfStatement node)
        {
            this.Visit(node.Condition);
            this.Visit(node.Consequence);
            this.Visit(node.AlternativeOpt);
            return null;
        }
        public override BoundNode? VisitDoStatement(BoundDoStatement node)
        {
            this.Visit(node.Condition);
            this.Visit(node.Body);
            return null;
        }
        public override BoundNode? VisitWhileStatement(BoundWhileStatement node)
        {
            this.Visit(node.Condition);
            this.Visit(node.Body);
            return null;
        }
        public override BoundNode? VisitForStatement(BoundForStatement node)
        {
            this.Visit(node.Initializer);
            this.Visit(node.Condition);
            this.Visit(node.Increment);
            this.Visit(node.Body);
            return null;
        }
        public override BoundNode? VisitForEachStatement(BoundForEachStatement node)
        {
            this.Visit(node.IterationVariableType);
            this.Visit(node.IterationErrorExpressionOpt);
            this.Visit(node.Expression);
            this.Visit(node.DeconstructionOpt);
            this.Visit(node.AwaitOpt);
            this.Visit(node.Body);
            return null;
        }
        public override BoundNode? VisitForEachDeconstructStep(BoundForEachDeconstructStep node)
        {
            this.Visit(node.DeconstructionAssignment);
            this.Visit(node.TargetPlaceholder);
            return null;
        }
        public override BoundNode? VisitUsingStatement(BoundUsingStatement node)
        {
            this.Visit(node.DeclarationsOpt);
            this.Visit(node.ExpressionOpt);
            this.Visit(node.Body);
            this.Visit(node.AwaitOpt);
            return null;
        }
        public override BoundNode? VisitFixedStatement(BoundFixedStatement node)
        {
            this.Visit(node.Declarations);
            this.Visit(node.Body);
            return null;
        }
        public override BoundNode? VisitLockStatement(BoundLockStatement node)
        {
            this.Visit(node.Argument);
            this.Visit(node.Body);
            return null;
        }
        public override BoundNode? VisitTryStatement(BoundTryStatement node)
        {
            this.Visit(node.TryBlock);
            this.VisitList(node.CatchBlocks);
            this.Visit(node.FinallyBlockOpt);
            return null;
        }
        public override BoundNode? VisitCatchBlock(BoundCatchBlock node)
        {
            this.Visit(node.ExceptionSourceOpt);
            this.Visit(node.ExceptionFilterPrologueOpt);
            this.Visit(node.ExceptionFilterOpt);
            this.Visit(node.Body);
            return null;
        }
        public override BoundNode? VisitLiteral(BoundLiteral node) => null;
        public override BoundNode? VisitThisReference(BoundThisReference node) => null;
        public override BoundNode? VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node) => null;
        public override BoundNode? VisitHostObjectMemberReference(BoundHostObjectMemberReference node) => null;
        public override BoundNode? VisitBaseReference(BoundBaseReference node) => null;
        public override BoundNode? VisitLocal(BoundLocal node) => null;
        public override BoundNode? VisitPseudoVariable(BoundPseudoVariable node) => null;
        public override BoundNode? VisitRangeVariable(BoundRangeVariable node)
        {
            this.Visit(node.Value);
            return null;
        }
        public override BoundNode? VisitParameter(BoundParameter node) => null;
        public override BoundNode? VisitLabelStatement(BoundLabelStatement node) => null;
        public override BoundNode? VisitGotoStatement(BoundGotoStatement node)
        {
            this.Visit(node.CaseExpressionOpt);
            this.Visit(node.LabelExpressionOpt);
            return null;
        }
        public override BoundNode? VisitLabeledStatement(BoundLabeledStatement node)
        {
            this.Visit(node.Body);
            return null;
        }
        public override BoundNode? VisitLabel(BoundLabel node) => null;
        public override BoundNode? VisitStatementList(BoundStatementList node)
        {
            this.VisitList(node.Statements);
            return null;
        }
        public override BoundNode? VisitConditionalGoto(BoundConditionalGoto node)
        {
            this.Visit(node.Condition);
            return null;
        }
        public override BoundNode? VisitSwitchExpressionArm(BoundSwitchExpressionArm node)
        {
            this.Visit(node.Pattern);
            this.Visit(node.WhenClause);
            this.Visit(node.Value);
            return null;
        }
        public override BoundNode? VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node)
        {
            this.Visit(node.Expression);
            this.VisitList(node.SwitchArms);
            return null;
        }
        public override BoundNode? VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node)
        {
            this.Visit(node.Expression);
            this.VisitList(node.SwitchArms);
            return null;
        }
        public override BoundNode? VisitDecisionDag(BoundDecisionDag node)
        {
            this.Visit(node.RootNode);
            return null;
        }
        public override BoundNode? VisitEvaluationDecisionDagNode(BoundEvaluationDecisionDagNode node)
        {
            this.Visit(node.Evaluation);
            this.Visit(node.Next);
            return null;
        }
        public override BoundNode? VisitTestDecisionDagNode(BoundTestDecisionDagNode node)
        {
            this.Visit(node.Test);
            this.Visit(node.WhenTrue);
            this.Visit(node.WhenFalse);
            return null;
        }
        public override BoundNode? VisitWhenDecisionDagNode(BoundWhenDecisionDagNode node)
        {
            this.Visit(node.WhenExpression);
            this.Visit(node.WhenTrue);
            this.Visit(node.WhenFalse);
            return null;
        }
        public override BoundNode? VisitLeafDecisionDagNode(BoundLeafDecisionDagNode node) => null;
        public override BoundNode? VisitDagTemp(BoundDagTemp node)
        {
            this.Visit(node.Source);
            return null;
        }
        public override BoundNode? VisitDagTypeTest(BoundDagTypeTest node)
        {
            this.Visit(node.Input);
            return null;
        }
        public override BoundNode? VisitDagNonNullTest(BoundDagNonNullTest node)
        {
            this.Visit(node.Input);
            return null;
        }
        public override BoundNode? VisitDagExplicitNullTest(BoundDagExplicitNullTest node)
        {
            this.Visit(node.Input);
            return null;
        }
        public override BoundNode? VisitDagValueTest(BoundDagValueTest node)
        {
            this.Visit(node.Input);
            return null;
        }
        public override BoundNode? VisitDagRelationalTest(BoundDagRelationalTest node)
        {
            this.Visit(node.Input);
            return null;
        }
        public override BoundNode? VisitDagDeconstructEvaluation(BoundDagDeconstructEvaluation node)
        {
            this.Visit(node.Input);
            return null;
        }
        public override BoundNode? VisitDagTypeEvaluation(BoundDagTypeEvaluation node)
        {
            this.Visit(node.Input);
            return null;
        }
        public override BoundNode? VisitDagFieldEvaluation(BoundDagFieldEvaluation node)
        {
            this.Visit(node.Input);
            return null;
        }
        public override BoundNode? VisitDagPropertyEvaluation(BoundDagPropertyEvaluation node)
        {
            this.Visit(node.Input);
            return null;
        }
        public override BoundNode? VisitDagIndexEvaluation(BoundDagIndexEvaluation node)
        {
            this.Visit(node.Input);
            return null;
        }
        public override BoundNode? VisitSwitchSection(BoundSwitchSection node)
        {
            this.VisitList(node.SwitchLabels);
            this.VisitList(node.Statements);
            return null;
        }
        public override BoundNode? VisitSwitchLabel(BoundSwitchLabel node)
        {
            this.Visit(node.Pattern);
            this.Visit(node.WhenClause);
            return null;
        }
        public override BoundNode? VisitSequencePointExpression(BoundSequencePointExpression node)
        {
            this.Visit(node.Expression);
            return null;
        }
        public override BoundNode? VisitSequence(BoundSequence node)
        {
            this.VisitList(node.SideEffects);
            this.Visit(node.Value);
            return null;
        }
        public override BoundNode? VisitSpillSequence(BoundSpillSequence node)
        {
            this.VisitList(node.SideEffects);
            this.Visit(node.Value);
            return null;
        }
        public override BoundNode? VisitDynamicMemberAccess(BoundDynamicMemberAccess node)
        {
            this.Visit(node.Receiver);
            return null;
        }
        public override BoundNode? VisitDynamicInvocation(BoundDynamicInvocation node)
        {
            this.Visit(node.Expression);
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitConditionalAccess(BoundConditionalAccess node)
        {
            this.Visit(node.Receiver);
            this.Visit(node.AccessExpression);
            return null;
        }
        public override BoundNode? VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
        {
            this.Visit(node.Receiver);
            this.Visit(node.WhenNotNull);
            this.Visit(node.WhenNullOpt);
            return null;
        }
        public override BoundNode? VisitConditionalReceiver(BoundConditionalReceiver node) => null;
        public override BoundNode? VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node)
        {
            this.Visit(node.ValueTypeReceiver);
            this.Visit(node.ReferenceTypeReceiver);
            return null;
        }
        public override BoundNode? VisitMethodGroup(BoundMethodGroup node)
        {
            this.Visit(node.ReceiverOpt);
            return null;
        }
        public override BoundNode? VisitPropertyGroup(BoundPropertyGroup node)
        {
            this.Visit(node.ReceiverOpt);
            return null;
        }
        public override BoundNode? VisitCall(BoundCall node)
        {
            this.Visit(node.ReceiverOpt);
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            this.Visit(node.ReceiverOpt);
            this.Visit(node.Argument);
            return null;
        }
        public override BoundNode? VisitAttribute(BoundAttribute node)
        {
            this.VisitList(node.ConstructorArguments);
            this.VisitList(node.NamedArguments);
            return null;
        }
        public override BoundNode? VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node)
        {
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            this.VisitList(node.Arguments);
            this.Visit(node.InitializerExpressionOpt);
            return null;
        }
        public override BoundNode? VisitTupleLiteral(BoundTupleLiteral node)
        {
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
        {
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
        {
            this.VisitList(node.Arguments);
            this.Visit(node.InitializerExpressionOpt);
            return null;
        }
        public override BoundNode? VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
        {
            this.Visit(node.InitializerExpressionOpt);
            return null;
        }
        public override BoundNode? VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
        {
            this.Visit(node.Placeholder);
            this.VisitList(node.Initializers);
            return null;
        }
        public override BoundNode? VisitObjectInitializerMember(BoundObjectInitializerMember node)
        {
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node) => null;
        public override BoundNode? VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
        {
            this.Visit(node.Placeholder);
            this.VisitList(node.Initializers);
            return null;
        }
        public override BoundNode? VisitCollectionElementInitializer(BoundCollectionElementInitializer node)
        {
            this.VisitList(node.Arguments);
            this.Visit(node.ImplicitReceiverOpt);
            return null;
        }
        public override BoundNode? VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node)
        {
            this.Visit(node.Expression);
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitImplicitReceiver(BoundImplicitReceiver node) => null;
        public override BoundNode? VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node)
        {
            this.VisitList(node.Arguments);
            this.VisitList(node.Declarations);
            return null;
        }
        public override BoundNode? VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node) => null;
        public override BoundNode? VisitNewT(BoundNewT node)
        {
            this.Visit(node.InitializerExpressionOpt);
            return null;
        }
        public override BoundNode? VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            this.Visit(node.Argument);
            return null;
        }
        public override BoundNode? VisitArrayCreation(BoundArrayCreation node)
        {
            this.VisitList(node.Bounds);
            this.Visit(node.InitializerOpt);
            return null;
        }
        public override BoundNode? VisitArrayInitialization(BoundArrayInitialization node)
        {
            this.VisitList(node.Initializers);
            return null;
        }
        public override BoundNode? VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node)
        {
            this.Visit(node.Count);
            this.Visit(node.InitializerOpt);
            return null;
        }
        public override BoundNode? VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node)
        {
            this.Visit(node.Count);
            this.Visit(node.InitializerOpt);
            return null;
        }
        public override BoundNode? VisitFieldAccess(BoundFieldAccess node)
        {
            this.Visit(node.ReceiverOpt);
            return null;
        }
        public override BoundNode? VisitHoistedFieldAccess(BoundHoistedFieldAccess node) => null;
        public override BoundNode? VisitPropertyAccess(BoundPropertyAccess node)
        {
            this.Visit(node.ReceiverOpt);
            return null;
        }
        public override BoundNode? VisitEventAccess(BoundEventAccess node)
        {
            this.Visit(node.ReceiverOpt);
            return null;
        }
        public override BoundNode? VisitIndexerAccess(BoundIndexerAccess node)
        {
            this.Visit(node.ReceiverOpt);
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node)
        {
            this.Visit(node.Receiver);
            this.Visit(node.Argument);
            return null;
        }
        public override BoundNode? VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node)
        {
            this.Visit(node.Receiver);
            this.VisitList(node.Arguments);
            return null;
        }
        public override BoundNode? VisitLambda(BoundLambda node)
        {
            this.Visit(node.Body);
            return null;
        }
        public override BoundNode? VisitUnboundLambda(UnboundLambda node) => null;
        public override BoundNode? VisitQueryClause(BoundQueryClause node)
        {
            this.Visit(node.Value);
            return null;
        }
        public override BoundNode? VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node)
        {
            this.VisitList(node.Statements);
            return null;
        }
        public override BoundNode? VisitNameOfOperator(BoundNameOfOperator node)
        {
            this.Visit(node.Argument);
            return null;
        }
        public override BoundNode? VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node)
        {
            this.VisitList(node.Parts);
            return null;
        }
        public override BoundNode? VisitInterpolatedString(BoundInterpolatedString node)
        {
            this.VisitList(node.Parts);
            return null;
        }
        public override BoundNode? VisitStringInsert(BoundStringInsert node)
        {
            this.Visit(node.Value);
            this.Visit(node.Alignment);
            this.Visit(node.Format);
            return null;
        }
        public override BoundNode? VisitIsPatternExpression(BoundIsPatternExpression node)
        {
            this.Visit(node.Expression);
            this.Visit(node.Pattern);
            return null;
        }
        public override BoundNode? VisitConstantPattern(BoundConstantPattern node)
        {
            this.Visit(node.Value);
            return null;
        }
        public override BoundNode? VisitDiscardPattern(BoundDiscardPattern node) => null;
        public override BoundNode? VisitDeclarationPattern(BoundDeclarationPattern node)
        {
            this.Visit(node.VariableAccess);
            this.Visit(node.DeclaredType);
            return null;
        }
        public override BoundNode? VisitRecursivePattern(BoundRecursivePattern node)
        {
            this.Visit(node.DeclaredType);
            this.VisitList(node.Deconstruction);
            this.VisitList(node.Properties);
            this.Visit(node.VariableAccess);
            return null;
        }
        public override BoundNode? VisitITuplePattern(BoundITuplePattern node)
        {
            this.VisitList(node.Subpatterns);
            return null;
        }
        public override BoundNode? VisitSubpattern(BoundSubpattern node)
        {
            this.Visit(node.Pattern);
            return null;
        }
        public override BoundNode? VisitTypePattern(BoundTypePattern node)
        {
            this.Visit(node.DeclaredType);
            return null;
        }
        public override BoundNode? VisitBinaryPattern(BoundBinaryPattern node)
        {
            this.Visit(node.Left);
            this.Visit(node.Right);
            return null;
        }
        public override BoundNode? VisitNegatedPattern(BoundNegatedPattern node)
        {
            this.Visit(node.Negated);
            return null;
        }
        public override BoundNode? VisitRelationalPattern(BoundRelationalPattern node)
        {
            this.Visit(node.Value);
            return null;
        }
        public override BoundNode? VisitDiscardExpression(BoundDiscardExpression node) => null;
        public override BoundNode? VisitThrowExpression(BoundThrowExpression node)
        {
            this.Visit(node.Expression);
            return null;
        }
        public override BoundNode? VisitOutVariablePendingInference(OutVariablePendingInference node)
        {
            this.Visit(node.ReceiverOpt);
            return null;
        }
        public override BoundNode? VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node)
        {
            this.Visit(node.ReceiverOpt);
            return null;
        }
        public override BoundNode? VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node) => null;
        public override BoundNode? VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node)
        {
            this.Visit(node.BlockBody);
            this.Visit(node.ExpressionBody);
            return null;
        }
        public override BoundNode? VisitConstructorMethodBody(BoundConstructorMethodBody node)
        {
            this.Visit(node.Initializer);
            this.Visit(node.BlockBody);
            this.Visit(node.ExpressionBody);
            return null;
        }
        public override BoundNode? VisitExpressionWithNullability(BoundExpressionWithNullability node)
        {
            this.Visit(node.Expression);
            return null;
        }
        public override BoundNode? VisitWithExpression(BoundWithExpression node)
        {
            this.Visit(node.Receiver);
            this.Visit(node.InitializerExpression);
            return null;
        }
    }

    internal abstract partial class BoundTreeRewriter : BoundTreeVisitor
    {
        public override BoundNode? VisitFieldEqualsValue(BoundFieldEqualsValue node)
        {
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            return node.Update(node.Field, node.Locals, value);
        }
        public override BoundNode? VisitPropertyEqualsValue(BoundPropertyEqualsValue node)
        {
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            return node.Update(node.Property, node.Locals, value);
        }
        public override BoundNode? VisitParameterEqualsValue(BoundParameterEqualsValue node)
        {
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            return node.Update(node.Parameter, node.Locals, value);
        }
        public override BoundNode? VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node)
        {
            BoundStatement statement = (BoundStatement)this.Visit(node.Statement);
            return node.Update(statement);
        }
        public override BoundNode? VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.ValEscape, type);
        }
        public override BoundNode? VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.ValEscape, type);
        }
        public override BoundNode? VisitDisposableValuePlaceholder(BoundDisposableValuePlaceholder node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.IsNewInstance, type);
        }
        public override BoundNode? VisitDup(BoundDup node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.RefKind, type);
        }
        public override BoundNode? VisitPassByCopy(BoundPassByCopy node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(expression, type);
        }
        public override BoundNode? VisitBadExpression(BoundBadExpression node)
        {
            ImmutableArray<BoundExpression> childBoundNodes = this.VisitList(node.ChildBoundNodes);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.ResultKind, node.Symbols, childBoundNodes, type);
        }
        public override BoundNode? VisitBadStatement(BoundBadStatement node)
        {
            ImmutableArray<BoundNode> childBoundNodes = this.VisitList(node.ChildBoundNodes);
            return node.Update(childBoundNodes);
        }
        public override BoundNode? VisitExtractedFinallyBlock(BoundExtractedFinallyBlock node)
        {
            BoundBlock finallyBlock = (BoundBlock)this.Visit(node.FinallyBlock);
            return node.Update(finallyBlock);
        }
        public override BoundNode? VisitTypeExpression(BoundTypeExpression node)
        {
            BoundTypeExpression? boundContainingTypeOpt = (BoundTypeExpression?)this.Visit(node.BoundContainingTypeOpt);
            ImmutableArray<BoundExpression> boundDimensionsOpt = this.VisitList(node.BoundDimensionsOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.AliasOpt, boundContainingTypeOpt, boundDimensionsOpt, node.TypeWithAnnotations, type);
        }
        public override BoundNode? VisitTypeOrValueExpression(BoundTypeOrValueExpression node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Data, type);
        }
        public override BoundNode? VisitNamespaceExpression(BoundNamespaceExpression node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.NamespaceSymbol, node.AliasOpt);
        }
        public override BoundNode? VisitUnaryOperator(BoundUnaryOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.OperatorKind, operand, node.ConstantValueOpt, node.MethodOpt, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, type);
        }
        public override BoundNode? VisitIncrementOperator(BoundIncrementOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.OperatorKind, operand, node.MethodOpt, node.OperandConversion, node.ResultConversion, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, type);
        }
        public override BoundNode? VisitAddressOfOperator(BoundAddressOfOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(operand, node.IsManaged, type);
        }
        public override BoundNode? VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node)
        {
            BoundMethodGroup operand = (BoundMethodGroup)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(operand);
        }
        public override BoundNode? VisitFunctionPointerLoad(BoundFunctionPointerLoad node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.TargetMethod, type);
        }
        public override BoundNode? VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(operand, type);
        }
        public override BoundNode? VisitPointerElementAccess(BoundPointerElementAccess node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundExpression index = (BoundExpression)this.Visit(node.Index);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(expression, index, node.Checked, type);
        }
        public override BoundNode? VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node)
        {
            BoundExpression invokedExpression = (BoundExpression)this.Visit(node.InvokedExpression);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(invokedExpression, arguments, node.ArgumentRefKindsOpt, node.ResultKind, type);
        }
        public override BoundNode? VisitRefTypeOperator(BoundRefTypeOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(operand, node.GetTypeFromHandle, type);
        }
        public override BoundNode? VisitMakeRefOperator(BoundMakeRefOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(operand, type);
        }
        public override BoundNode? VisitRefValueOperator(BoundRefValueOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.NullableAnnotation, operand, type);
        }
        public override BoundNode? VisitFromEndIndexExpression(BoundFromEndIndexExpression node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(operand, node.MethodOpt, type);
        }
        public override BoundNode? VisitRangeExpression(BoundRangeExpression node)
        {
            BoundExpression? leftOperandOpt = (BoundExpression?)this.Visit(node.LeftOperandOpt);
            BoundExpression? rightOperandOpt = (BoundExpression?)this.Visit(node.RightOperandOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(leftOperandOpt, rightOperandOpt, node.MethodOpt, type);
        }
        public override BoundNode? VisitBinaryOperator(BoundBinaryOperator node)
        {
            BoundExpression left = (BoundExpression)this.Visit(node.Left);
            BoundExpression right = (BoundExpression)this.Visit(node.Right);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.OperatorKind, node.ConstantValueOpt, node.MethodOpt, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, left, right, type);
        }
        public override BoundNode? VisitTupleBinaryOperator(BoundTupleBinaryOperator node)
        {
            BoundExpression left = (BoundExpression)this.Visit(node.Left);
            BoundExpression right = (BoundExpression)this.Visit(node.Right);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(left, right, node.OperatorKind, node.Operators, type);
        }
        public override BoundNode? VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node)
        {
            BoundExpression left = (BoundExpression)this.Visit(node.Left);
            BoundExpression right = (BoundExpression)this.Visit(node.Right);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.OperatorKind, node.LogicalOperator, node.TrueOperator, node.FalseOperator, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, left, right, type);
        }
        public override BoundNode? VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            BoundExpression left = (BoundExpression)this.Visit(node.Left);
            BoundExpression right = (BoundExpression)this.Visit(node.Right);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Operator, left, right, node.LeftConversion, node.FinalConversion, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, type);
        }
        public override BoundNode? VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            BoundExpression left = (BoundExpression)this.Visit(node.Left);
            BoundExpression right = (BoundExpression)this.Visit(node.Right);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(left, right, node.IsRef, type);
        }
        public override BoundNode? VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            BoundTupleExpression left = (BoundTupleExpression)this.Visit(node.Left);
            BoundConversion right = (BoundConversion)this.Visit(node.Right);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(left, right, node.IsUsed, type);
        }
        public override BoundNode? VisitNullCoalescingOperator(BoundNullCoalescingOperator node)
        {
            BoundExpression leftOperand = (BoundExpression)this.Visit(node.LeftOperand);
            BoundExpression rightOperand = (BoundExpression)this.Visit(node.RightOperand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(leftOperand, rightOperand, node.LeftConversion, node.OperatorResultKind, type);
        }
        public override BoundNode? VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node)
        {
            BoundExpression leftOperand = (BoundExpression)this.Visit(node.LeftOperand);
            BoundExpression rightOperand = (BoundExpression)this.Visit(node.RightOperand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(leftOperand, rightOperand, type);
        }
        public override BoundNode? VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node)
        {
            BoundExpression condition = (BoundExpression)this.Visit(node.Condition);
            BoundExpression consequence = (BoundExpression)this.Visit(node.Consequence);
            BoundExpression alternative = (BoundExpression)this.Visit(node.Alternative);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(condition, consequence, alternative, node.ConstantValueOpt, node.NoCommonTypeError, type);
        }
        public override BoundNode? VisitConditionalOperator(BoundConditionalOperator node)
        {
            BoundExpression condition = (BoundExpression)this.Visit(node.Condition);
            BoundExpression consequence = (BoundExpression)this.Visit(node.Consequence);
            BoundExpression alternative = (BoundExpression)this.Visit(node.Alternative);
            TypeSymbol? naturalTypeOpt = this.VisitType(node.NaturalTypeOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.IsRef, condition, consequence, alternative, node.ConstantValueOpt, naturalTypeOpt, node.WasTargetTyped, type);
        }
        public override BoundNode? VisitArrayAccess(BoundArrayAccess node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundExpression> indices = this.VisitList(node.Indices);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(expression, indices, type);
        }
        public override BoundNode? VisitArrayLength(BoundArrayLength node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(expression, type);
        }
        public override BoundNode? VisitAwaitableInfo(BoundAwaitableInfo node)
        {
            BoundAwaitableValuePlaceholder? awaitableInstancePlaceholder = (BoundAwaitableValuePlaceholder?)this.Visit(node.AwaitableInstancePlaceholder);
            BoundExpression? getAwaiter = (BoundExpression?)this.Visit(node.GetAwaiter);
            return node.Update(awaitableInstancePlaceholder, node.IsDynamic, getAwaiter, node.IsCompleted, node.GetResult);
        }
        public override BoundNode? VisitAwaitExpression(BoundAwaitExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundAwaitableInfo awaitableInfo = (BoundAwaitableInfo)this.Visit(node.AwaitableInfo);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(expression, awaitableInfo, type);
        }
        public override BoundNode? VisitTypeOfOperator(BoundTypeOfOperator node)
        {
            BoundTypeExpression sourceType = (BoundTypeExpression)this.Visit(node.SourceType);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(sourceType, node.GetTypeFromHandle, type);
        }
        public override BoundNode? VisitMethodDefIndex(BoundMethodDefIndex node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Method, type);
        }
        public override BoundNode? VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.AnalysisKind, type);
        }
        public override BoundNode? VisitModuleVersionId(BoundModuleVersionId node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitModuleVersionIdString(BoundModuleVersionIdString node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitSourceDocumentIndex(BoundSourceDocumentIndex node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Document, type);
        }
        public override BoundNode? VisitMethodInfo(BoundMethodInfo node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Method, node.GetMethodFromHandle, type);
        }
        public override BoundNode? VisitFieldInfo(BoundFieldInfo node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Field, node.GetFieldFromHandle, type);
        }
        public override BoundNode? VisitDefaultLiteral(BoundDefaultLiteral node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update();
        }
        public override BoundNode? VisitDefaultExpression(BoundDefaultExpression node)
        {
            BoundTypeExpression? targetType = node.TargetType;
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(targetType, node.ConstantValueOpt, type);
        }
        public override BoundNode? VisitIsOperator(BoundIsOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundTypeExpression targetType = (BoundTypeExpression)this.Visit(node.TargetType);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(operand, targetType, node.Conversion, type);
        }
        public override BoundNode? VisitAsOperator(BoundAsOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundTypeExpression targetType = (BoundTypeExpression)this.Visit(node.TargetType);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(operand, targetType, node.Conversion, type);
        }
        public override BoundNode? VisitSizeOfOperator(BoundSizeOfOperator node)
        {
            BoundTypeExpression sourceType = (BoundTypeExpression)this.Visit(node.SourceType);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(sourceType, node.ConstantValueOpt, type);
        }
        public override BoundNode? VisitConversion(BoundConversion node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(operand, node.Conversion, node.IsBaseConversion, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ConversionGroupOpt, node.OriginalUserDefinedConversionsOpt, type);
        }
        public override BoundNode? VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(operand, node.ConversionMethod, type);
        }
        public override BoundNode? VisitArgList(BoundArgList node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitArgListOperator(BoundArgListOperator node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(arguments, node.ArgumentRefKindsOpt, type);
        }
        public override BoundNode? VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            TypeSymbol? elementPointerType = this.VisitType(node.ElementPointerType);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(elementPointerType, node.ElementPointerTypeConversion, expression, node.GetPinnableOpt, type);
        }
        public override BoundNode? VisitSequencePoint(BoundSequencePoint node)
        {
            BoundStatement? statementOpt = (BoundStatement?)this.Visit(node.StatementOpt);
            return node.Update(statementOpt);
        }
        public override BoundNode? VisitSequencePointWithSpan(BoundSequencePointWithSpan node)
        {
            BoundStatement? statementOpt = (BoundStatement?)this.Visit(node.StatementOpt);
            return node.Update(statementOpt, node.Span);
        }
        public override BoundNode? VisitSavePreviousSequencePoint(BoundSavePreviousSequencePoint node) => node;
        public override BoundNode? VisitRestorePreviousSequencePoint(BoundRestorePreviousSequencePoint node) => node;
        public override BoundNode? VisitStepThroughSequencePoint(BoundStepThroughSequencePoint node) => node;
        public override BoundNode? VisitBlock(BoundBlock node)
        {
            ImmutableArray<BoundStatement> statements = this.VisitList(node.Statements);
            return node.Update(node.Locals, node.LocalFunctions, statements);
        }
        public override BoundNode? VisitScope(BoundScope node)
        {
            ImmutableArray<BoundStatement> statements = this.VisitList(node.Statements);
            return node.Update(node.Locals, statements);
        }
        public override BoundNode? VisitStateMachineScope(BoundStateMachineScope node)
        {
            BoundStatement statement = (BoundStatement)this.Visit(node.Statement);
            return node.Update(node.Fields, statement);
        }
        public override BoundNode? VisitLocalDeclaration(BoundLocalDeclaration node)
        {
            BoundTypeExpression? declaredTypeOpt = (BoundTypeExpression?)this.Visit(node.DeclaredTypeOpt);
            BoundExpression? initializerOpt = (BoundExpression?)this.Visit(node.InitializerOpt);
            ImmutableArray<BoundExpression> argumentsOpt = this.VisitList(node.ArgumentsOpt);
            return node.Update(node.LocalSymbol, declaredTypeOpt, initializerOpt, argumentsOpt, node.InferredType);
        }
        public override BoundNode? VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node)
        {
            ImmutableArray<BoundLocalDeclaration> localDeclarations = this.VisitList(node.LocalDeclarations);
            return node.Update(localDeclarations);
        }
        public override BoundNode? VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node)
        {
            BoundAwaitableInfo? awaitOpt = (BoundAwaitableInfo?)this.Visit(node.AwaitOpt);
            ImmutableArray<BoundLocalDeclaration> localDeclarations = this.VisitList(node.LocalDeclarations);
            return node.Update(node.PatternDisposeInfoOpt, node.IDisposableConversion, awaitOpt, localDeclarations);
        }
        public override BoundNode? VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            BoundBlock? blockBody = (BoundBlock?)this.Visit(node.BlockBody);
            BoundBlock? expressionBody = (BoundBlock?)this.Visit(node.ExpressionBody);
            return node.Update(node.Symbol, blockBody, expressionBody);
        }
        public override BoundNode? VisitNoOpStatement(BoundNoOpStatement node) => node;
        public override BoundNode? VisitReturnStatement(BoundReturnStatement node)
        {
            BoundExpression? expressionOpt = (BoundExpression?)this.Visit(node.ExpressionOpt);
            return node.Update(node.RefKind, expressionOpt);
        }
        public override BoundNode? VisitYieldReturnStatement(BoundYieldReturnStatement node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            return node.Update(expression);
        }
        public override BoundNode? VisitYieldBreakStatement(BoundYieldBreakStatement node) => node;
        public override BoundNode? VisitThrowStatement(BoundThrowStatement node)
        {
            BoundExpression? expressionOpt = (BoundExpression?)this.Visit(node.ExpressionOpt);
            return node.Update(expressionOpt);
        }
        public override BoundNode? VisitExpressionStatement(BoundExpressionStatement node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            return node.Update(expression);
        }
        public override BoundNode? VisitBreakStatement(BoundBreakStatement node) => node;
        public override BoundNode? VisitContinueStatement(BoundContinueStatement node) => node;
        public override BoundNode? VisitSwitchStatement(BoundSwitchStatement node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundSwitchSection> switchSections = this.VisitList(node.SwitchSections);
            BoundDecisionDag decisionDag = node.DecisionDag;
            BoundSwitchLabel? defaultLabel = (BoundSwitchLabel?)this.Visit(node.DefaultLabel);
            return node.Update(expression, node.InnerLocals, node.InnerLocalFunctions, switchSections, decisionDag, defaultLabel, node.BreakLabel);
        }
        public override BoundNode? VisitSwitchDispatch(BoundSwitchDispatch node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            return node.Update(expression, node.Cases, node.DefaultLabel, node.EqualityMethod);
        }
        public override BoundNode? VisitIfStatement(BoundIfStatement node)
        {
            BoundExpression condition = (BoundExpression)this.Visit(node.Condition);
            BoundStatement consequence = (BoundStatement)this.Visit(node.Consequence);
            BoundStatement? alternativeOpt = (BoundStatement?)this.Visit(node.AlternativeOpt);
            return node.Update(condition, consequence, alternativeOpt);
        }
        public override BoundNode? VisitDoStatement(BoundDoStatement node)
        {
            BoundExpression condition = (BoundExpression)this.Visit(node.Condition);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(node.Locals, condition, body, node.BreakLabel, node.ContinueLabel);
        }
        public override BoundNode? VisitWhileStatement(BoundWhileStatement node)
        {
            BoundExpression condition = (BoundExpression)this.Visit(node.Condition);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(node.Locals, condition, body, node.BreakLabel, node.ContinueLabel);
        }
        public override BoundNode? VisitForStatement(BoundForStatement node)
        {
            BoundStatement? initializer = (BoundStatement?)this.Visit(node.Initializer);
            BoundExpression? condition = (BoundExpression?)this.Visit(node.Condition);
            BoundStatement? increment = (BoundStatement?)this.Visit(node.Increment);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(node.OuterLocals, initializer, node.InnerLocals, condition, increment, body, node.BreakLabel, node.ContinueLabel);
        }
        public override BoundNode? VisitForEachStatement(BoundForEachStatement node)
        {
            BoundTypeExpression iterationVariableType = (BoundTypeExpression)this.Visit(node.IterationVariableType);
            BoundExpression? iterationErrorExpressionOpt = (BoundExpression?)this.Visit(node.IterationErrorExpressionOpt);
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundForEachDeconstructStep? deconstructionOpt = (BoundForEachDeconstructStep?)this.Visit(node.DeconstructionOpt);
            BoundAwaitableInfo? awaitOpt = (BoundAwaitableInfo?)this.Visit(node.AwaitOpt);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(node.EnumeratorInfoOpt, node.ElementConversion, iterationVariableType, node.IterationVariables, iterationErrorExpressionOpt, expression, deconstructionOpt, awaitOpt, body, node.Checked, node.BreakLabel, node.ContinueLabel);
        }
        public override BoundNode? VisitForEachDeconstructStep(BoundForEachDeconstructStep node)
        {
            BoundDeconstructionAssignmentOperator deconstructionAssignment = (BoundDeconstructionAssignmentOperator)this.Visit(node.DeconstructionAssignment);
            BoundDeconstructValuePlaceholder targetPlaceholder = (BoundDeconstructValuePlaceholder)this.Visit(node.TargetPlaceholder);
            return node.Update(deconstructionAssignment, targetPlaceholder);
        }
        public override BoundNode? VisitUsingStatement(BoundUsingStatement node)
        {
            BoundMultipleLocalDeclarations? declarationsOpt = (BoundMultipleLocalDeclarations?)this.Visit(node.DeclarationsOpt);
            BoundExpression? expressionOpt = (BoundExpression?)this.Visit(node.ExpressionOpt);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            BoundAwaitableInfo? awaitOpt = (BoundAwaitableInfo?)this.Visit(node.AwaitOpt);
            return node.Update(node.Locals, declarationsOpt, expressionOpt, node.IDisposableConversion, body, awaitOpt, node.PatternDisposeInfoOpt);
        }
        public override BoundNode? VisitFixedStatement(BoundFixedStatement node)
        {
            BoundMultipleLocalDeclarations declarations = (BoundMultipleLocalDeclarations)this.Visit(node.Declarations);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(node.Locals, declarations, body);
        }
        public override BoundNode? VisitLockStatement(BoundLockStatement node)
        {
            BoundExpression argument = (BoundExpression)this.Visit(node.Argument);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(argument, body);
        }
        public override BoundNode? VisitTryStatement(BoundTryStatement node)
        {
            BoundBlock tryBlock = (BoundBlock)this.Visit(node.TryBlock);
            ImmutableArray<BoundCatchBlock> catchBlocks = this.VisitList(node.CatchBlocks);
            BoundBlock? finallyBlockOpt = (BoundBlock?)this.Visit(node.FinallyBlockOpt);
            return node.Update(tryBlock, catchBlocks, finallyBlockOpt, node.FinallyLabelOpt, node.PreferFaultHandler);
        }
        public override BoundNode? VisitCatchBlock(BoundCatchBlock node)
        {
            BoundExpression? exceptionSourceOpt = (BoundExpression?)this.Visit(node.ExceptionSourceOpt);
            BoundStatementList? exceptionFilterPrologueOpt = (BoundStatementList?)this.Visit(node.ExceptionFilterPrologueOpt);
            BoundExpression? exceptionFilterOpt = (BoundExpression?)this.Visit(node.ExceptionFilterOpt);
            BoundBlock body = (BoundBlock)this.Visit(node.Body);
            TypeSymbol? exceptionTypeOpt = this.VisitType(node.ExceptionTypeOpt);
            return node.Update(node.Locals, exceptionSourceOpt, exceptionTypeOpt, exceptionFilterPrologueOpt, exceptionFilterOpt, body, node.IsSynthesizedAsyncCatchAll);
        }
        public override BoundNode? VisitLiteral(BoundLiteral node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.ConstantValueOpt, type);
        }
        public override BoundNode? VisitThisReference(BoundThisReference node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitBaseReference(BoundBaseReference node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitLocal(BoundLocal node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.LocalSymbol, node.DeclarationKind, node.ConstantValueOpt, node.IsNullableUnknown, type);
        }
        public override BoundNode? VisitPseudoVariable(BoundPseudoVariable node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.LocalSymbol, node.EmitExpressions, type);
        }
        public override BoundNode? VisitRangeVariable(BoundRangeVariable node)
        {
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.RangeVariableSymbol, value, type);
        }
        public override BoundNode? VisitParameter(BoundParameter node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.ParameterSymbol, type);
        }
        public override BoundNode? VisitLabelStatement(BoundLabelStatement node) => node;
        public override BoundNode? VisitGotoStatement(BoundGotoStatement node)
        {
            BoundExpression? caseExpressionOpt = (BoundExpression?)this.Visit(node.CaseExpressionOpt);
            BoundLabel? labelExpressionOpt = (BoundLabel?)this.Visit(node.LabelExpressionOpt);
            return node.Update(node.Label, caseExpressionOpt, labelExpressionOpt);
        }
        public override BoundNode? VisitLabeledStatement(BoundLabeledStatement node)
        {
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(node.Label, body);
        }
        public override BoundNode? VisitLabel(BoundLabel node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Label, type);
        }
        public override BoundNode? VisitStatementList(BoundStatementList node)
        {
            ImmutableArray<BoundStatement> statements = this.VisitList(node.Statements);
            return node.Update(statements);
        }
        public override BoundNode? VisitConditionalGoto(BoundConditionalGoto node)
        {
            BoundExpression condition = (BoundExpression)this.Visit(node.Condition);
            return node.Update(condition, node.JumpIfTrue, node.Label);
        }
        public override BoundNode? VisitSwitchExpressionArm(BoundSwitchExpressionArm node)
        {
            BoundPattern pattern = (BoundPattern)this.Visit(node.Pattern);
            BoundExpression? whenClause = (BoundExpression?)this.Visit(node.WhenClause);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            return node.Update(node.Locals, pattern, whenClause, value, node.Label);
        }
        public override BoundNode? VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundSwitchExpressionArm> switchArms = this.VisitList(node.SwitchArms);
            BoundDecisionDag decisionDag = node.DecisionDag;
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(expression, switchArms, decisionDag, node.DefaultLabel, node.ReportedNotExhaustive, type);
        }
        public override BoundNode? VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundSwitchExpressionArm> switchArms = this.VisitList(node.SwitchArms);
            BoundDecisionDag decisionDag = node.DecisionDag;
            TypeSymbol? naturalTypeOpt = this.VisitType(node.NaturalTypeOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(naturalTypeOpt, node.WasTargetTyped, node.Conversion, expression, switchArms, decisionDag, node.DefaultLabel, node.ReportedNotExhaustive, type);
        }
        public override BoundNode? VisitDecisionDag(BoundDecisionDag node)
        {
            BoundDecisionDagNode rootNode = (BoundDecisionDagNode)this.Visit(node.RootNode);
            return node.Update(rootNode);
        }
        public override BoundNode? VisitEvaluationDecisionDagNode(BoundEvaluationDecisionDagNode node)
        {
            BoundDagEvaluation evaluation = (BoundDagEvaluation)this.Visit(node.Evaluation);
            BoundDecisionDagNode next = (BoundDecisionDagNode)this.Visit(node.Next);
            return node.Update(evaluation, next);
        }
        public override BoundNode? VisitTestDecisionDagNode(BoundTestDecisionDagNode node)
        {
            BoundDagTest test = (BoundDagTest)this.Visit(node.Test);
            BoundDecisionDagNode whenTrue = (BoundDecisionDagNode)this.Visit(node.WhenTrue);
            BoundDecisionDagNode whenFalse = (BoundDecisionDagNode)this.Visit(node.WhenFalse);
            return node.Update(test, whenTrue, whenFalse);
        }
        public override BoundNode? VisitWhenDecisionDagNode(BoundWhenDecisionDagNode node)
        {
            BoundExpression? whenExpression = (BoundExpression?)this.Visit(node.WhenExpression);
            BoundDecisionDagNode whenTrue = (BoundDecisionDagNode)this.Visit(node.WhenTrue);
            BoundDecisionDagNode? whenFalse = (BoundDecisionDagNode?)this.Visit(node.WhenFalse);
            return node.Update(node.Bindings, whenExpression, whenTrue, whenFalse);
        }
        public override BoundNode? VisitLeafDecisionDagNode(BoundLeafDecisionDagNode node) => node;
        public override BoundNode? VisitDagTemp(BoundDagTemp node)
        {
            BoundDagEvaluation? source = (BoundDagEvaluation?)this.Visit(node.Source);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type, source, node.Index);
        }
        public override BoundNode? VisitDagTypeTest(BoundDagTypeTest node)
        {
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type, input);
        }
        public override BoundNode? VisitDagNonNullTest(BoundDagNonNullTest node)
        {
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(node.IsExplicitTest, input);
        }
        public override BoundNode? VisitDagExplicitNullTest(BoundDagExplicitNullTest node)
        {
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(input);
        }
        public override BoundNode? VisitDagValueTest(BoundDagValueTest node)
        {
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(node.Value, input);
        }
        public override BoundNode? VisitDagRelationalTest(BoundDagRelationalTest node)
        {
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(node.OperatorKind, node.Value, input);
        }
        public override BoundNode? VisitDagDeconstructEvaluation(BoundDagDeconstructEvaluation node)
        {
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(node.DeconstructMethod, input);
        }
        public override BoundNode? VisitDagTypeEvaluation(BoundDagTypeEvaluation node)
        {
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type, input);
        }
        public override BoundNode? VisitDagFieldEvaluation(BoundDagFieldEvaluation node)
        {
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(node.Field, input);
        }
        public override BoundNode? VisitDagPropertyEvaluation(BoundDagPropertyEvaluation node)
        {
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(node.Property, input);
        }
        public override BoundNode? VisitDagIndexEvaluation(BoundDagIndexEvaluation node)
        {
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(node.Property, node.Index, input);
        }
        public override BoundNode? VisitSwitchSection(BoundSwitchSection node)
        {
            ImmutableArray<BoundSwitchLabel> switchLabels = this.VisitList(node.SwitchLabels);
            ImmutableArray<BoundStatement> statements = this.VisitList(node.Statements);
            return node.Update(node.Locals, switchLabels, statements);
        }
        public override BoundNode? VisitSwitchLabel(BoundSwitchLabel node)
        {
            BoundPattern pattern = (BoundPattern)this.Visit(node.Pattern);
            BoundExpression? whenClause = (BoundExpression?)this.Visit(node.WhenClause);
            return node.Update(node.Label, pattern, whenClause);
        }
        public override BoundNode? VisitSequencePointExpression(BoundSequencePointExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(expression, type);
        }
        public override BoundNode? VisitSequence(BoundSequence node)
        {
            ImmutableArray<BoundExpression> sideEffects = this.VisitList(node.SideEffects);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Locals, sideEffects, value, type);
        }
        public override BoundNode? VisitSpillSequence(BoundSpillSequence node)
        {
            ImmutableArray<BoundStatement> sideEffects = this.VisitList(node.SideEffects);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Locals, sideEffects, value, type);
        }
        public override BoundNode? VisitDynamicMemberAccess(BoundDynamicMemberAccess node)
        {
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiver, node.TypeArgumentsOpt, node.Name, node.Invoked, node.Indexed, type);
        }
        public override BoundNode? VisitDynamicInvocation(BoundDynamicInvocation node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.ApplicableMethods, expression, arguments, type);
        }
        public override BoundNode? VisitConditionalAccess(BoundConditionalAccess node)
        {
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            BoundExpression accessExpression = (BoundExpression)this.Visit(node.AccessExpression);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiver, accessExpression, type);
        }
        public override BoundNode? VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
        {
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            BoundExpression whenNotNull = (BoundExpression)this.Visit(node.WhenNotNull);
            BoundExpression? whenNullOpt = (BoundExpression?)this.Visit(node.WhenNullOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiver, node.HasValueMethodOpt, whenNotNull, whenNullOpt, node.Id, type);
        }
        public override BoundNode? VisitConditionalReceiver(BoundConditionalReceiver node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Id, type);
        }
        public override BoundNode? VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node)
        {
            BoundExpression valueTypeReceiver = (BoundExpression)this.Visit(node.ValueTypeReceiver);
            BoundExpression referenceTypeReceiver = (BoundExpression)this.Visit(node.ReferenceTypeReceiver);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(valueTypeReceiver, referenceTypeReceiver, type);
        }
        public override BoundNode? VisitMethodGroup(BoundMethodGroup node)
        {
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.TypeArgumentsOpt, node.Name, node.Methods, node.LookupSymbolOpt, node.LookupError, node.Flags, receiverOpt, node.ResultKind);
        }
        public override BoundNode? VisitPropertyGroup(BoundPropertyGroup node)
        {
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Properties, receiverOpt, node.ResultKind);
        }
        public override BoundNode? VisitCall(BoundCall node)
        {
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiverOpt, node.Method, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.IsDelegateCall, node.Expanded, node.InvokedAsExtensionMethod, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, node.OriginalMethodsOpt, type);
        }
        public override BoundNode? VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            BoundExpression argument = (BoundExpression)this.Visit(node.Argument);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Event, node.IsAddition, node.IsDynamic, receiverOpt, argument, type);
        }
        public override BoundNode? VisitAttribute(BoundAttribute node)
        {
            ImmutableArray<BoundExpression> constructorArguments = this.VisitList(node.ConstructorArguments);
            ImmutableArray<BoundAssignmentOperator> namedArguments = this.VisitList(node.NamedArguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Constructor, constructorArguments, node.ConstructorArgumentNamesOpt, node.ConstructorArgumentsToParamsOpt, node.ConstructorExpanded, namedArguments, node.ResultKind, type);
        }
        public override BoundNode? VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.InitializerOpt);
        }
        public override BoundNode? VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundObjectInitializerExpressionBase? initializerExpressionOpt = (BoundObjectInitializerExpressionBase?)this.Visit(node.InitializerExpressionOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Constructor, node.ConstructorsGroup, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ConstantValueOpt, initializerExpressionOpt, node.WasTargetTyped, type);
        }
        public override BoundNode? VisitTupleLiteral(BoundTupleLiteral node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(arguments, node.ArgumentNamesOpt, node.InferredNamesOpt, type);
        }
        public override BoundNode? VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
        {
            BoundTupleLiteral? sourceTuple = node.SourceTuple;
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(sourceTuple, node.WasTargetTyped, arguments, node.ArgumentNamesOpt, node.InferredNamesOpt, type);
        }
        public override BoundNode? VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundObjectInitializerExpressionBase? initializerExpressionOpt = (BoundObjectInitializerExpressionBase?)this.Visit(node.InitializerExpressionOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Name, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, initializerExpressionOpt, node.ApplicableMethods, type);
        }
        public override BoundNode? VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
        {
            BoundObjectInitializerExpressionBase? initializerExpressionOpt = (BoundObjectInitializerExpressionBase?)this.Visit(node.InitializerExpressionOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.GuidString, initializerExpressionOpt, type);
        }
        public override BoundNode? VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
        {
            BoundObjectOrCollectionValuePlaceholder placeholder = (BoundObjectOrCollectionValuePlaceholder)this.Visit(node.Placeholder);
            ImmutableArray<BoundExpression> initializers = this.VisitList(node.Initializers);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(placeholder, initializers, type);
        }
        public override BoundNode? VisitObjectInitializerMember(BoundObjectInitializerMember node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? receiverType = this.VisitType(node.ReceiverType);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.MemberSymbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, receiverType, type);
        }
        public override BoundNode? VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node)
        {
            TypeSymbol? receiverType = this.VisitType(node.ReceiverType);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.MemberName, receiverType, type);
        }
        public override BoundNode? VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
        {
            BoundObjectOrCollectionValuePlaceholder placeholder = (BoundObjectOrCollectionValuePlaceholder)this.Visit(node.Placeholder);
            ImmutableArray<BoundExpression> initializers = this.VisitList(node.Initializers);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(placeholder, initializers, type);
        }
        public override BoundNode? VisitCollectionElementInitializer(BoundCollectionElementInitializer node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundExpression? implicitReceiverOpt = (BoundExpression?)this.Visit(node.ImplicitReceiverOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.AddMethod, arguments, implicitReceiverOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.InvokedAsExtensionMethod, node.ResultKind, type);
        }
        public override BoundNode? VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.ApplicableMethods, expression, arguments, type);
        }
        public override BoundNode? VisitImplicitReceiver(BoundImplicitReceiver node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            ImmutableArray<BoundAnonymousPropertyDeclaration> declarations = this.VisitList(node.Declarations);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Constructor, arguments, declarations, type);
        }
        public override BoundNode? VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Property, type);
        }
        public override BoundNode? VisitNewT(BoundNewT node)
        {
            BoundObjectInitializerExpressionBase? initializerExpressionOpt = (BoundObjectInitializerExpressionBase?)this.Visit(node.InitializerExpressionOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(initializerExpressionOpt, type);
        }
        public override BoundNode? VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            BoundExpression argument = (BoundExpression)this.Visit(node.Argument);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(argument, node.MethodOpt, node.IsExtensionMethod, type);
        }
        public override BoundNode? VisitArrayCreation(BoundArrayCreation node)
        {
            ImmutableArray<BoundExpression> bounds = this.VisitList(node.Bounds);
            BoundArrayInitialization? initializerOpt = (BoundArrayInitialization?)this.Visit(node.InitializerOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(bounds, initializerOpt, type);
        }
        public override BoundNode? VisitArrayInitialization(BoundArrayInitialization node)
        {
            ImmutableArray<BoundExpression> initializers = this.VisitList(node.Initializers);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(initializers);
        }
        public override BoundNode? VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node)
        {
            BoundExpression count = (BoundExpression)this.Visit(node.Count);
            BoundArrayInitialization? initializerOpt = (BoundArrayInitialization?)this.Visit(node.InitializerOpt);
            TypeSymbol? elementType = this.VisitType(node.ElementType);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(elementType, count, initializerOpt, type);
        }
        public override BoundNode? VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node)
        {
            BoundExpression count = (BoundExpression)this.Visit(node.Count);
            BoundArrayInitialization? initializerOpt = (BoundArrayInitialization?)this.Visit(node.InitializerOpt);
            TypeSymbol? elementType = this.VisitType(node.ElementType);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(elementType, count, initializerOpt, type);
        }
        public override BoundNode? VisitFieldAccess(BoundFieldAccess node)
        {
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiverOpt, node.FieldSymbol, node.ConstantValueOpt, node.ResultKind, node.IsByValue, node.IsDeclaration, type);
        }
        public override BoundNode? VisitHoistedFieldAccess(BoundHoistedFieldAccess node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.FieldSymbol, type);
        }
        public override BoundNode? VisitPropertyAccess(BoundPropertyAccess node)
        {
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiverOpt, node.PropertySymbol, node.ResultKind, type);
        }
        public override BoundNode? VisitEventAccess(BoundEventAccess node)
        {
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiverOpt, node.EventSymbol, node.IsUsableAsField, node.ResultKind, type);
        }
        public override BoundNode? VisitIndexerAccess(BoundIndexerAccess node)
        {
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiverOpt, node.Indexer, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.OriginalIndexersOpt, type);
        }
        public override BoundNode? VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node)
        {
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            BoundExpression argument = (BoundExpression)this.Visit(node.Argument);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiver, node.LengthOrCountProperty, node.PatternSymbol, argument, type);
        }
        public override BoundNode? VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node)
        {
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiver, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.ApplicableIndexers, type);
        }
        public override BoundNode? VisitLambda(BoundLambda node)
        {
            UnboundLambda unboundLambda = node.UnboundLambda;
            BoundBlock body = (BoundBlock)this.Visit(node.Body);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(unboundLambda, node.Symbol, body, node.Diagnostics, node.Binder, type);
        }
        public override BoundNode? VisitUnboundLambda(UnboundLambda node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.Data, node.WithDependencies);
        }
        public override BoundNode? VisitQueryClause(BoundQueryClause node)
        {
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            BoundExpression? operation = node.Operation;
            BoundExpression? cast = node.Cast;
            BoundExpression? unoptimizedForm = node.UnoptimizedForm;
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(value, node.DefinedSymbol, operation, cast, node.Binder, unoptimizedForm, type);
        }
        public override BoundNode? VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node)
        {
            ImmutableArray<BoundStatement> statements = this.VisitList(node.Statements);
            return node.Update(statements);
        }
        public override BoundNode? VisitNameOfOperator(BoundNameOfOperator node)
        {
            BoundExpression argument = (BoundExpression)this.Visit(node.Argument);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(argument, node.ConstantValueOpt, type);
        }
        public override BoundNode? VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node)
        {
            ImmutableArray<BoundExpression> parts = this.VisitList(node.Parts);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(parts, node.ConstantValueOpt, type);
        }
        public override BoundNode? VisitInterpolatedString(BoundInterpolatedString node)
        {
            ImmutableArray<BoundExpression> parts = this.VisitList(node.Parts);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(parts, node.ConstantValueOpt, type);
        }
        public override BoundNode? VisitStringInsert(BoundStringInsert node)
        {
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            BoundExpression? alignment = (BoundExpression?)this.Visit(node.Alignment);
            BoundLiteral? format = (BoundLiteral?)this.Visit(node.Format);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(value, alignment, format, type);
        }
        public override BoundNode? VisitIsPatternExpression(BoundIsPatternExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundPattern pattern = (BoundPattern)this.Visit(node.Pattern);
            BoundDecisionDag decisionDag = node.DecisionDag;
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(expression, pattern, node.IsNegated, decisionDag, node.WhenTrueLabel, node.WhenFalseLabel, type);
        }
        public override BoundNode? VisitConstantPattern(BoundConstantPattern node)
        {
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            TypeSymbol? inputType = this.VisitType(node.InputType);
            TypeSymbol? narrowedType = this.VisitType(node.NarrowedType);
            return node.Update(value, node.ConstantValue, inputType, narrowedType);
        }
        public override BoundNode? VisitDiscardPattern(BoundDiscardPattern node)
        {
            TypeSymbol? inputType = this.VisitType(node.InputType);
            TypeSymbol? narrowedType = this.VisitType(node.NarrowedType);
            return node.Update(inputType, narrowedType);
        }
        public override BoundNode? VisitDeclarationPattern(BoundDeclarationPattern node)
        {
            BoundExpression? variableAccess = (BoundExpression?)this.Visit(node.VariableAccess);
            BoundTypeExpression declaredType = (BoundTypeExpression)this.Visit(node.DeclaredType);
            TypeSymbol? inputType = this.VisitType(node.InputType);
            TypeSymbol? narrowedType = this.VisitType(node.NarrowedType);
            return node.Update(node.Variable, variableAccess, declaredType, node.IsVar, inputType, narrowedType);
        }
        public override BoundNode? VisitRecursivePattern(BoundRecursivePattern node)
        {
            BoundTypeExpression? declaredType = (BoundTypeExpression?)this.Visit(node.DeclaredType);
            ImmutableArray<BoundSubpattern> deconstruction = this.VisitList(node.Deconstruction);
            ImmutableArray<BoundSubpattern> properties = this.VisitList(node.Properties);
            BoundExpression? variableAccess = (BoundExpression?)this.Visit(node.VariableAccess);
            TypeSymbol? inputType = this.VisitType(node.InputType);
            TypeSymbol? narrowedType = this.VisitType(node.NarrowedType);
            return node.Update(declaredType, node.DeconstructMethod, deconstruction, properties, node.Variable, variableAccess, node.IsExplicitNotNullTest, inputType, narrowedType);
        }
        public override BoundNode? VisitITuplePattern(BoundITuplePattern node)
        {
            ImmutableArray<BoundSubpattern> subpatterns = this.VisitList(node.Subpatterns);
            TypeSymbol? inputType = this.VisitType(node.InputType);
            TypeSymbol? narrowedType = this.VisitType(node.NarrowedType);
            return node.Update(node.GetLengthMethod, node.GetItemMethod, subpatterns, inputType, narrowedType);
        }
        public override BoundNode? VisitSubpattern(BoundSubpattern node)
        {
            BoundPattern pattern = (BoundPattern)this.Visit(node.Pattern);
            return node.Update(node.Symbol, pattern);
        }
        public override BoundNode? VisitTypePattern(BoundTypePattern node)
        {
            BoundTypeExpression declaredType = (BoundTypeExpression)this.Visit(node.DeclaredType);
            TypeSymbol? inputType = this.VisitType(node.InputType);
            TypeSymbol? narrowedType = this.VisitType(node.NarrowedType);
            return node.Update(declaredType, node.IsExplicitNotNullTest, inputType, narrowedType);
        }
        public override BoundNode? VisitBinaryPattern(BoundBinaryPattern node)
        {
            BoundPattern left = (BoundPattern)this.Visit(node.Left);
            BoundPattern right = (BoundPattern)this.Visit(node.Right);
            TypeSymbol? inputType = this.VisitType(node.InputType);
            TypeSymbol? narrowedType = this.VisitType(node.NarrowedType);
            return node.Update(node.Disjunction, left, right, inputType, narrowedType);
        }
        public override BoundNode? VisitNegatedPattern(BoundNegatedPattern node)
        {
            BoundPattern negated = (BoundPattern)this.Visit(node.Negated);
            TypeSymbol? inputType = this.VisitType(node.InputType);
            TypeSymbol? narrowedType = this.VisitType(node.NarrowedType);
            return node.Update(negated, inputType, narrowedType);
        }
        public override BoundNode? VisitRelationalPattern(BoundRelationalPattern node)
        {
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            TypeSymbol? inputType = this.VisitType(node.InputType);
            TypeSymbol? narrowedType = this.VisitType(node.NarrowedType);
            return node.Update(node.Relation, value, node.ConstantValue, inputType, narrowedType);
        }
        public override BoundNode? VisitDiscardExpression(BoundDiscardExpression node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(type);
        }
        public override BoundNode? VisitThrowExpression(BoundThrowExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(expression, type);
        }
        public override BoundNode? VisitOutVariablePendingInference(OutVariablePendingInference node)
        {
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.VariableSymbol, receiverOpt);
        }
        public override BoundNode? VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node)
        {
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(node.VariableSymbol, receiverOpt);
        }
        public override BoundNode? VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node)
        {
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update();
        }
        public override BoundNode? VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node)
        {
            BoundBlock? blockBody = (BoundBlock?)this.Visit(node.BlockBody);
            BoundBlock? expressionBody = (BoundBlock?)this.Visit(node.ExpressionBody);
            return node.Update(blockBody, expressionBody);
        }
        public override BoundNode? VisitConstructorMethodBody(BoundConstructorMethodBody node)
        {
            BoundExpressionStatement? initializer = (BoundExpressionStatement?)this.Visit(node.Initializer);
            BoundBlock? blockBody = (BoundBlock?)this.Visit(node.BlockBody);
            BoundBlock? expressionBody = (BoundBlock?)this.Visit(node.ExpressionBody);
            return node.Update(node.Locals, initializer, blockBody, expressionBody);
        }
        public override BoundNode? VisitExpressionWithNullability(BoundExpressionWithNullability node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(expression, node.NullableAnnotation, type);
        }
        public override BoundNode? VisitWithExpression(BoundWithExpression node)
        {
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            BoundObjectInitializerExpressionBase initializerExpression = (BoundObjectInitializerExpressionBase)this.Visit(node.InitializerExpression);
            TypeSymbol? type = this.VisitType(node.Type);
            return node.Update(receiver, node.CloneMethod, initializerExpression, type);
        }
    }

    internal sealed partial class NullabilityRewriter : BoundTreeRewriter
    {
        private readonly ImmutableDictionary<BoundExpression, (NullabilityInfo Info, TypeSymbol? Type)> _updatedNullabilities;
        private readonly NullableWalker.SnapshotManager? _snapshotManager;
        private readonly ImmutableDictionary<Symbol, Symbol>.Builder _remappedSymbols;

        public NullabilityRewriter(ImmutableDictionary<BoundExpression, (NullabilityInfo Info, TypeSymbol? Type)> updatedNullabilities, NullableWalker.SnapshotManager? snapshotManager, ImmutableDictionary<Symbol, Symbol>.Builder remappedSymbols)
        {
            _updatedNullabilities = updatedNullabilities;
            _snapshotManager = snapshotManager;
            _remappedSymbols = remappedSymbols;
        }

        public override BoundNode? VisitFieldEqualsValue(BoundFieldEqualsValue node)
        {
            FieldSymbol field = GetUpdatedSymbol(node, node.Field);
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            return node.Update(field, locals, value);
        }

        public override BoundNode? VisitPropertyEqualsValue(BoundPropertyEqualsValue node)
        {
            PropertySymbol property = GetUpdatedSymbol(node, node.Property);
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            return node.Update(property, locals, value);
        }

        public override BoundNode? VisitParameterEqualsValue(BoundParameterEqualsValue node)
        {
            ParameterSymbol parameter = GetUpdatedSymbol(node, node.Parameter);
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            return node.Update(parameter, locals, value);
        }

        public override BoundNode? VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundDeconstructValuePlaceholder updatedNode = node.Update(node.ValEscape, infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundTupleOperandPlaceholder updatedNode = node.Update(infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundAwaitableValuePlaceholder updatedNode = node.Update(node.ValEscape, infoAndType.Type);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitDisposableValuePlaceholder(BoundDisposableValuePlaceholder node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundDisposableValuePlaceholder updatedNode = node.Update(infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundObjectOrCollectionValuePlaceholder updatedNode = node.Update(node.IsNewInstance, infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitDup(BoundDup node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundDup updatedNode = node.Update(node.RefKind, infoAndType.Type);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitPassByCopy(BoundPassByCopy node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundPassByCopy updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(expression, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(expression, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitBadExpression(BoundBadExpression node)
        {
            ImmutableArray<Symbol?> symbols = GetUpdatedArray(node, node.Symbols);
            ImmutableArray<BoundExpression> childBoundNodes = this.VisitList(node.ChildBoundNodes);
            BoundBadExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.ResultKind, node.Symbols, childBoundNodes, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.ResultKind, node.Symbols, childBoundNodes, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitTypeExpression(BoundTypeExpression node)
        {
            BoundTypeExpression? boundContainingTypeOpt = (BoundTypeExpression?)this.Visit(node.BoundContainingTypeOpt);
            ImmutableArray<BoundExpression> boundDimensionsOpt = this.VisitList(node.BoundDimensionsOpt);
            BoundTypeExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.AliasOpt, boundContainingTypeOpt, boundDimensionsOpt, node.TypeWithAnnotations, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.AliasOpt, boundContainingTypeOpt, boundDimensionsOpt, node.TypeWithAnnotations, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitTypeOrValueExpression(BoundTypeOrValueExpression node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundTypeOrValueExpression updatedNode = node.Update(node.Data, infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitNamespaceExpression(BoundNamespaceExpression node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundNamespaceExpression updatedNode = node.Update(node.NamespaceSymbol, node.AliasOpt);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitUnaryOperator(BoundUnaryOperator node)
        {
            MethodSymbol? methodOpt = GetUpdatedSymbol(node, node.MethodOpt);
            ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt = GetUpdatedArray(node, node.OriginalUserDefinedOperatorsOpt);
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundUnaryOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.OperatorKind, operand, node.ConstantValueOpt, methodOpt, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.OperatorKind, operand, node.ConstantValueOpt, methodOpt, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitIncrementOperator(BoundIncrementOperator node)
        {
            MethodSymbol? methodOpt = GetUpdatedSymbol(node, node.MethodOpt);
            ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt = GetUpdatedArray(node, node.OriginalUserDefinedOperatorsOpt);
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundIncrementOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.OperatorKind, operand, methodOpt, node.OperandConversion, node.ResultConversion, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.OperatorKind, operand, methodOpt, node.OperandConversion, node.ResultConversion, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitAddressOfOperator(BoundAddressOfOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundAddressOfOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(operand, node.IsManaged, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(operand, node.IsManaged, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node)
        {
            BoundMethodGroup operand = (BoundMethodGroup)this.Visit(node.Operand);
            BoundUnconvertedAddressOfOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(operand);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(operand);
            }
            return updatedNode;
        }

        public override BoundNode? VisitFunctionPointerLoad(BoundFunctionPointerLoad node)
        {
            MethodSymbol targetMethod = GetUpdatedSymbol(node, node.TargetMethod);
            BoundFunctionPointerLoad updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(targetMethod, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(targetMethod, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundPointerIndirectionOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(operand, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(operand, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitPointerElementAccess(BoundPointerElementAccess node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundExpression index = (BoundExpression)this.Visit(node.Index);
            BoundPointerElementAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(expression, index, node.Checked, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(expression, index, node.Checked, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node)
        {
            BoundExpression invokedExpression = (BoundExpression)this.Visit(node.InvokedExpression);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundFunctionPointerInvocation updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(invokedExpression, arguments, node.ArgumentRefKindsOpt, node.ResultKind, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(invokedExpression, arguments, node.ArgumentRefKindsOpt, node.ResultKind, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitRefTypeOperator(BoundRefTypeOperator node)
        {
            MethodSymbol? getTypeFromHandle = GetUpdatedSymbol(node, node.GetTypeFromHandle);
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundRefTypeOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(operand, getTypeFromHandle, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(operand, getTypeFromHandle, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitMakeRefOperator(BoundMakeRefOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundMakeRefOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(operand, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(operand, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitRefValueOperator(BoundRefValueOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundRefValueOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.NullableAnnotation, operand, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.NullableAnnotation, operand, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitFromEndIndexExpression(BoundFromEndIndexExpression node)
        {
            MethodSymbol? methodOpt = GetUpdatedSymbol(node, node.MethodOpt);
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundFromEndIndexExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(operand, methodOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(operand, methodOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitRangeExpression(BoundRangeExpression node)
        {
            MethodSymbol? methodOpt = GetUpdatedSymbol(node, node.MethodOpt);
            BoundExpression? leftOperandOpt = (BoundExpression?)this.Visit(node.LeftOperandOpt);
            BoundExpression? rightOperandOpt = (BoundExpression?)this.Visit(node.RightOperandOpt);
            BoundRangeExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(leftOperandOpt, rightOperandOpt, methodOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(leftOperandOpt, rightOperandOpt, methodOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitTupleBinaryOperator(BoundTupleBinaryOperator node)
        {
            BoundExpression left = (BoundExpression)this.Visit(node.Left);
            BoundExpression right = (BoundExpression)this.Visit(node.Right);
            BoundTupleBinaryOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(left, right, node.OperatorKind, node.Operators, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(left, right, node.OperatorKind, node.Operators, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            ImmutableArray<MethodSymbol> originalUserDefinedOperatorsOpt = GetUpdatedArray(node, node.OriginalUserDefinedOperatorsOpt);
            BoundExpression left = (BoundExpression)this.Visit(node.Left);
            BoundExpression right = (BoundExpression)this.Visit(node.Right);
            BoundCompoundAssignmentOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.Operator, left, right, node.LeftConversion, node.FinalConversion, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.Operator, left, right, node.LeftConversion, node.FinalConversion, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            BoundExpression left = (BoundExpression)this.Visit(node.Left);
            BoundExpression right = (BoundExpression)this.Visit(node.Right);
            BoundAssignmentOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(left, right, node.IsRef, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(left, right, node.IsRef, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            BoundTupleExpression left = (BoundTupleExpression)this.Visit(node.Left);
            BoundConversion right = (BoundConversion)this.Visit(node.Right);
            BoundDeconstructionAssignmentOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(left, right, node.IsUsed, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(left, right, node.IsUsed, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitNullCoalescingOperator(BoundNullCoalescingOperator node)
        {
            BoundExpression leftOperand = (BoundExpression)this.Visit(node.LeftOperand);
            BoundExpression rightOperand = (BoundExpression)this.Visit(node.RightOperand);
            BoundNullCoalescingOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(leftOperand, rightOperand, node.LeftConversion, node.OperatorResultKind, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(leftOperand, rightOperand, node.LeftConversion, node.OperatorResultKind, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node)
        {
            BoundExpression leftOperand = (BoundExpression)this.Visit(node.LeftOperand);
            BoundExpression rightOperand = (BoundExpression)this.Visit(node.RightOperand);
            BoundNullCoalescingAssignmentOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(leftOperand, rightOperand, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(leftOperand, rightOperand, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node)
        {
            BoundExpression condition = (BoundExpression)this.Visit(node.Condition);
            BoundExpression consequence = (BoundExpression)this.Visit(node.Consequence);
            BoundExpression alternative = (BoundExpression)this.Visit(node.Alternative);
            BoundUnconvertedConditionalOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(condition, consequence, alternative, node.ConstantValueOpt, node.NoCommonTypeError, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(condition, consequence, alternative, node.ConstantValueOpt, node.NoCommonTypeError, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitConditionalOperator(BoundConditionalOperator node)
        {
            TypeSymbol? naturalTypeOpt = GetUpdatedSymbol(node, node.NaturalTypeOpt);
            BoundExpression condition = (BoundExpression)this.Visit(node.Condition);
            BoundExpression consequence = (BoundExpression)this.Visit(node.Consequence);
            BoundExpression alternative = (BoundExpression)this.Visit(node.Alternative);
            BoundConditionalOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.IsRef, condition, consequence, alternative, node.ConstantValueOpt, naturalTypeOpt, node.WasTargetTyped, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.IsRef, condition, consequence, alternative, node.ConstantValueOpt, naturalTypeOpt, node.WasTargetTyped, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitArrayAccess(BoundArrayAccess node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundExpression> indices = this.VisitList(node.Indices);
            BoundArrayAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(expression, indices, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(expression, indices, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitArrayLength(BoundArrayLength node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundArrayLength updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(expression, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(expression, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitAwaitableInfo(BoundAwaitableInfo node)
        {
            PropertySymbol? isCompleted = GetUpdatedSymbol(node, node.IsCompleted);
            MethodSymbol? getResult = GetUpdatedSymbol(node, node.GetResult);
            BoundAwaitableValuePlaceholder? awaitableInstancePlaceholder = (BoundAwaitableValuePlaceholder?)this.Visit(node.AwaitableInstancePlaceholder);
            BoundExpression? getAwaiter = (BoundExpression?)this.Visit(node.GetAwaiter);
            return node.Update(awaitableInstancePlaceholder, node.IsDynamic, getAwaiter, isCompleted, getResult);
        }

        public override BoundNode? VisitAwaitExpression(BoundAwaitExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundAwaitableInfo awaitableInfo = (BoundAwaitableInfo)this.Visit(node.AwaitableInfo);
            BoundAwaitExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(expression, awaitableInfo, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(expression, awaitableInfo, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitTypeOfOperator(BoundTypeOfOperator node)
        {
            MethodSymbol? getTypeFromHandle = GetUpdatedSymbol(node, node.GetTypeFromHandle);
            BoundTypeExpression sourceType = (BoundTypeExpression)this.Visit(node.SourceType);
            BoundTypeOfOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(sourceType, getTypeFromHandle, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(sourceType, getTypeFromHandle, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitMethodDefIndex(BoundMethodDefIndex node)
        {
            MethodSymbol method = GetUpdatedSymbol(node, node.Method);
            BoundMethodDefIndex updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(method, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(method, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundMaximumMethodDefIndex updatedNode = node.Update(infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundInstrumentationPayloadRoot updatedNode = node.Update(node.AnalysisKind, infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitModuleVersionId(BoundModuleVersionId node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundModuleVersionId updatedNode = node.Update(infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitModuleVersionIdString(BoundModuleVersionIdString node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundModuleVersionIdString updatedNode = node.Update(infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitSourceDocumentIndex(BoundSourceDocumentIndex node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundSourceDocumentIndex updatedNode = node.Update(node.Document, infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitMethodInfo(BoundMethodInfo node)
        {
            MethodSymbol method = GetUpdatedSymbol(node, node.Method);
            MethodSymbol? getMethodFromHandle = GetUpdatedSymbol(node, node.GetMethodFromHandle);
            BoundMethodInfo updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(method, getMethodFromHandle, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(method, getMethodFromHandle, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitFieldInfo(BoundFieldInfo node)
        {
            FieldSymbol field = GetUpdatedSymbol(node, node.Field);
            MethodSymbol? getFieldFromHandle = GetUpdatedSymbol(node, node.GetFieldFromHandle);
            BoundFieldInfo updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(field, getFieldFromHandle, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(field, getFieldFromHandle, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDefaultLiteral(BoundDefaultLiteral node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundDefaultLiteral updatedNode = node.Update();
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitDefaultExpression(BoundDefaultExpression node)
        {
            BoundTypeExpression? targetType = node.TargetType;
            BoundDefaultExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(targetType, node.ConstantValueOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(targetType, node.ConstantValueOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitIsOperator(BoundIsOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundTypeExpression targetType = (BoundTypeExpression)this.Visit(node.TargetType);
            BoundIsOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(operand, targetType, node.Conversion, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(operand, targetType, node.Conversion, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitAsOperator(BoundAsOperator node)
        {
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundTypeExpression targetType = (BoundTypeExpression)this.Visit(node.TargetType);
            BoundAsOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(operand, targetType, node.Conversion, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(operand, targetType, node.Conversion, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitSizeOfOperator(BoundSizeOfOperator node)
        {
            BoundTypeExpression sourceType = (BoundTypeExpression)this.Visit(node.SourceType);
            BoundSizeOfOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(sourceType, node.ConstantValueOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(sourceType, node.ConstantValueOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitConversion(BoundConversion node)
        {
            ImmutableArray<MethodSymbol> originalUserDefinedConversionsOpt = GetUpdatedArray(node, node.OriginalUserDefinedConversionsOpt);
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundConversion updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(operand, node.Conversion, node.IsBaseConversion, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ConversionGroupOpt, node.OriginalUserDefinedConversionsOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(operand, node.Conversion, node.IsBaseConversion, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ConversionGroupOpt, node.OriginalUserDefinedConversionsOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node)
        {
            MethodSymbol conversionMethod = GetUpdatedSymbol(node, node.ConversionMethod);
            BoundExpression operand = (BoundExpression)this.Visit(node.Operand);
            BoundReadOnlySpanFromArray updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(operand, conversionMethod, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(operand, conversionMethod, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitArgList(BoundArgList node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundArgList updatedNode = node.Update(infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitArgListOperator(BoundArgListOperator node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundArgListOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(arguments, node.ArgumentRefKindsOpt, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(arguments, node.ArgumentRefKindsOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node)
        {
            TypeSymbol elementPointerType = GetUpdatedSymbol(node, node.ElementPointerType);
            MethodSymbol? getPinnableOpt = GetUpdatedSymbol(node, node.GetPinnableOpt);
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundFixedLocalCollectionInitializer updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(elementPointerType, node.ElementPointerTypeConversion, expression, getPinnableOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(elementPointerType, node.ElementPointerTypeConversion, expression, getPinnableOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitBlock(BoundBlock node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            ImmutableArray<LocalFunctionSymbol> localFunctions = GetUpdatedArray(node, node.LocalFunctions);
            ImmutableArray<BoundStatement> statements = this.VisitList(node.Statements);
            return node.Update(locals, localFunctions, statements);
        }

        public override BoundNode? VisitScope(BoundScope node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            ImmutableArray<BoundStatement> statements = this.VisitList(node.Statements);
            return node.Update(locals, statements);
        }

        public override BoundNode? VisitStateMachineScope(BoundStateMachineScope node)
        {
            ImmutableArray<StateMachineFieldSymbol> fields = GetUpdatedArray(node, node.Fields);
            BoundStatement statement = (BoundStatement)this.Visit(node.Statement);
            return node.Update(fields, statement);
        }

        public override BoundNode? VisitLocalDeclaration(BoundLocalDeclaration node)
        {
            LocalSymbol localSymbol = GetUpdatedSymbol(node, node.LocalSymbol);
            BoundTypeExpression? declaredTypeOpt = (BoundTypeExpression?)this.Visit(node.DeclaredTypeOpt);
            BoundExpression? initializerOpt = (BoundExpression?)this.Visit(node.InitializerOpt);
            ImmutableArray<BoundExpression> argumentsOpt = this.VisitList(node.ArgumentsOpt);
            return node.Update(localSymbol, declaredTypeOpt, initializerOpt, argumentsOpt, node.InferredType);
        }

        public override BoundNode? VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            LocalFunctionSymbol symbol = GetUpdatedSymbol(node, node.Symbol);
            BoundBlock? blockBody = (BoundBlock?)this.Visit(node.BlockBody);
            BoundBlock? expressionBody = (BoundBlock?)this.Visit(node.ExpressionBody);
            return node.Update(symbol, blockBody, expressionBody);
        }

        public override BoundNode? VisitSwitchStatement(BoundSwitchStatement node)
        {
            ImmutableArray<LocalSymbol> innerLocals = GetUpdatedArray(node, node.InnerLocals);
            ImmutableArray<LocalFunctionSymbol> innerLocalFunctions = GetUpdatedArray(node, node.InnerLocalFunctions);
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundSwitchSection> switchSections = this.VisitList(node.SwitchSections);
            BoundDecisionDag decisionDag = node.DecisionDag;
            BoundSwitchLabel? defaultLabel = (BoundSwitchLabel?)this.Visit(node.DefaultLabel);
            return node.Update(expression, innerLocals, innerLocalFunctions, switchSections, decisionDag, defaultLabel, node.BreakLabel);
        }

        public override BoundNode? VisitSwitchDispatch(BoundSwitchDispatch node)
        {
            MethodSymbol? equalityMethod = GetUpdatedSymbol(node, node.EqualityMethod);
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            return node.Update(expression, node.Cases, node.DefaultLabel, equalityMethod);
        }

        public override BoundNode? VisitDoStatement(BoundDoStatement node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            BoundExpression condition = (BoundExpression)this.Visit(node.Condition);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(locals, condition, body, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode? VisitWhileStatement(BoundWhileStatement node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            BoundExpression condition = (BoundExpression)this.Visit(node.Condition);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(locals, condition, body, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode? VisitForStatement(BoundForStatement node)
        {
            ImmutableArray<LocalSymbol> outerLocals = GetUpdatedArray(node, node.OuterLocals);
            ImmutableArray<LocalSymbol> innerLocals = GetUpdatedArray(node, node.InnerLocals);
            BoundStatement? initializer = (BoundStatement?)this.Visit(node.Initializer);
            BoundExpression? condition = (BoundExpression?)this.Visit(node.Condition);
            BoundStatement? increment = (BoundStatement?)this.Visit(node.Increment);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(outerLocals, initializer, innerLocals, condition, increment, body, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode? VisitForEachStatement(BoundForEachStatement node)
        {
            ImmutableArray<LocalSymbol> iterationVariables = GetUpdatedArray(node, node.IterationVariables);
            BoundTypeExpression iterationVariableType = (BoundTypeExpression)this.Visit(node.IterationVariableType);
            BoundExpression? iterationErrorExpressionOpt = (BoundExpression?)this.Visit(node.IterationErrorExpressionOpt);
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundForEachDeconstructStep? deconstructionOpt = (BoundForEachDeconstructStep?)this.Visit(node.DeconstructionOpt);
            BoundAwaitableInfo? awaitOpt = (BoundAwaitableInfo?)this.Visit(node.AwaitOpt);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(node.EnumeratorInfoOpt, node.ElementConversion, iterationVariableType, iterationVariables, iterationErrorExpressionOpt, expression, deconstructionOpt, awaitOpt, body, node.Checked, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode? VisitUsingStatement(BoundUsingStatement node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            BoundMultipleLocalDeclarations? declarationsOpt = (BoundMultipleLocalDeclarations?)this.Visit(node.DeclarationsOpt);
            BoundExpression? expressionOpt = (BoundExpression?)this.Visit(node.ExpressionOpt);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            BoundAwaitableInfo? awaitOpt = (BoundAwaitableInfo?)this.Visit(node.AwaitOpt);
            return node.Update(locals, declarationsOpt, expressionOpt, node.IDisposableConversion, body, awaitOpt, node.PatternDisposeInfoOpt);
        }

        public override BoundNode? VisitFixedStatement(BoundFixedStatement node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            BoundMultipleLocalDeclarations declarations = (BoundMultipleLocalDeclarations)this.Visit(node.Declarations);
            BoundStatement body = (BoundStatement)this.Visit(node.Body);
            return node.Update(locals, declarations, body);
        }

        public override BoundNode? VisitCatchBlock(BoundCatchBlock node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            TypeSymbol? exceptionTypeOpt = GetUpdatedSymbol(node, node.ExceptionTypeOpt);
            BoundExpression? exceptionSourceOpt = (BoundExpression?)this.Visit(node.ExceptionSourceOpt);
            BoundStatementList? exceptionFilterPrologueOpt = (BoundStatementList?)this.Visit(node.ExceptionFilterPrologueOpt);
            BoundExpression? exceptionFilterOpt = (BoundExpression?)this.Visit(node.ExceptionFilterOpt);
            BoundBlock body = (BoundBlock)this.Visit(node.Body);
            return node.Update(locals, exceptionSourceOpt, exceptionTypeOpt, exceptionFilterPrologueOpt, exceptionFilterOpt, body, node.IsSynthesizedAsyncCatchAll);
        }

        public override BoundNode? VisitLiteral(BoundLiteral node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundLiteral updatedNode = node.Update(node.ConstantValueOpt, infoAndType.Type);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitThisReference(BoundThisReference node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundThisReference updatedNode = node.Update(infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundPreviousSubmissionReference updatedNode = node.Update(infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundHostObjectMemberReference updatedNode = node.Update(infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitBaseReference(BoundBaseReference node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundBaseReference updatedNode = node.Update(infoAndType.Type);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitLocal(BoundLocal node)
        {
            LocalSymbol localSymbol = GetUpdatedSymbol(node, node.LocalSymbol);
            BoundLocal updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(localSymbol, node.DeclarationKind, node.ConstantValueOpt, node.IsNullableUnknown, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(localSymbol, node.DeclarationKind, node.ConstantValueOpt, node.IsNullableUnknown, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitPseudoVariable(BoundPseudoVariable node)
        {
            LocalSymbol localSymbol = GetUpdatedSymbol(node, node.LocalSymbol);
            BoundPseudoVariable updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(localSymbol, node.EmitExpressions, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(localSymbol, node.EmitExpressions, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitRangeVariable(BoundRangeVariable node)
        {
            RangeVariableSymbol rangeVariableSymbol = GetUpdatedSymbol(node, node.RangeVariableSymbol);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            BoundRangeVariable updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(rangeVariableSymbol, value, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(rangeVariableSymbol, value, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitParameter(BoundParameter node)
        {
            ParameterSymbol parameterSymbol = GetUpdatedSymbol(node, node.ParameterSymbol);
            BoundParameter updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(parameterSymbol, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(parameterSymbol, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitLabel(BoundLabel node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundLabel updatedNode = node.Update(node.Label, infoAndType.Type);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitSwitchExpressionArm(BoundSwitchExpressionArm node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            BoundPattern pattern = (BoundPattern)this.Visit(node.Pattern);
            BoundExpression? whenClause = (BoundExpression?)this.Visit(node.WhenClause);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            return node.Update(locals, pattern, whenClause, value, node.Label);
        }

        public override BoundNode? VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundSwitchExpressionArm> switchArms = this.VisitList(node.SwitchArms);
            BoundDecisionDag decisionDag = node.DecisionDag;
            BoundUnconvertedSwitchExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(expression, switchArms, decisionDag, node.DefaultLabel, node.ReportedNotExhaustive, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(expression, switchArms, decisionDag, node.DefaultLabel, node.ReportedNotExhaustive, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node)
        {
            TypeSymbol? naturalTypeOpt = GetUpdatedSymbol(node, node.NaturalTypeOpt);
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundSwitchExpressionArm> switchArms = this.VisitList(node.SwitchArms);
            BoundDecisionDag decisionDag = node.DecisionDag;
            BoundConvertedSwitchExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(naturalTypeOpt, node.WasTargetTyped, node.Conversion, expression, switchArms, decisionDag, node.DefaultLabel, node.ReportedNotExhaustive, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(naturalTypeOpt, node.WasTargetTyped, node.Conversion, expression, switchArms, decisionDag, node.DefaultLabel, node.ReportedNotExhaustive, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDagDeconstructEvaluation(BoundDagDeconstructEvaluation node)
        {
            MethodSymbol deconstructMethod = GetUpdatedSymbol(node, node.DeconstructMethod);
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(deconstructMethod, input);
        }

        public override BoundNode? VisitDagFieldEvaluation(BoundDagFieldEvaluation node)
        {
            FieldSymbol field = GetUpdatedSymbol(node, node.Field);
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(field, input);
        }

        public override BoundNode? VisitDagPropertyEvaluation(BoundDagPropertyEvaluation node)
        {
            PropertySymbol property = GetUpdatedSymbol(node, node.Property);
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(property, input);
        }

        public override BoundNode? VisitDagIndexEvaluation(BoundDagIndexEvaluation node)
        {
            PropertySymbol property = GetUpdatedSymbol(node, node.Property);
            BoundDagTemp input = (BoundDagTemp)this.Visit(node.Input);
            return node.Update(property, node.Index, input);
        }

        public override BoundNode? VisitSwitchSection(BoundSwitchSection node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            ImmutableArray<BoundSwitchLabel> switchLabels = this.VisitList(node.SwitchLabels);
            ImmutableArray<BoundStatement> statements = this.VisitList(node.Statements);
            return node.Update(locals, switchLabels, statements);
        }

        public override BoundNode? VisitSequencePointExpression(BoundSequencePointExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundSequencePointExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(expression, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(expression, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitSequence(BoundSequence node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            ImmutableArray<BoundExpression> sideEffects = this.VisitList(node.SideEffects);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            BoundSequence updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(locals, sideEffects, value, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(locals, sideEffects, value, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitSpillSequence(BoundSpillSequence node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            ImmutableArray<BoundStatement> sideEffects = this.VisitList(node.SideEffects);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            BoundSpillSequence updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(locals, sideEffects, value, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(locals, sideEffects, value, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDynamicMemberAccess(BoundDynamicMemberAccess node)
        {
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            BoundDynamicMemberAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiver, node.TypeArgumentsOpt, node.Name, node.Invoked, node.Indexed, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiver, node.TypeArgumentsOpt, node.Name, node.Invoked, node.Indexed, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDynamicInvocation(BoundDynamicInvocation node)
        {
            ImmutableArray<MethodSymbol> applicableMethods = GetUpdatedArray(node, node.ApplicableMethods);
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundDynamicInvocation updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, applicableMethods, expression, arguments, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, applicableMethods, expression, arguments, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitConditionalAccess(BoundConditionalAccess node)
        {
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            BoundExpression accessExpression = (BoundExpression)this.Visit(node.AccessExpression);
            BoundConditionalAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiver, accessExpression, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiver, accessExpression, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
        {
            MethodSymbol? hasValueMethodOpt = GetUpdatedSymbol(node, node.HasValueMethodOpt);
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            BoundExpression whenNotNull = (BoundExpression)this.Visit(node.WhenNotNull);
            BoundExpression? whenNullOpt = (BoundExpression?)this.Visit(node.WhenNullOpt);
            BoundLoweredConditionalAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiver, hasValueMethodOpt, whenNotNull, whenNullOpt, node.Id, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiver, hasValueMethodOpt, whenNotNull, whenNullOpt, node.Id, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitConditionalReceiver(BoundConditionalReceiver node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundConditionalReceiver updatedNode = node.Update(node.Id, infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node)
        {
            BoundExpression valueTypeReceiver = (BoundExpression)this.Visit(node.ValueTypeReceiver);
            BoundExpression referenceTypeReceiver = (BoundExpression)this.Visit(node.ReferenceTypeReceiver);
            BoundComplexConditionalReceiver updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(valueTypeReceiver, referenceTypeReceiver, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(valueTypeReceiver, referenceTypeReceiver, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitMethodGroup(BoundMethodGroup node)
        {
            ImmutableArray<MethodSymbol> methods = GetUpdatedArray(node, node.Methods);
            Symbol? lookupSymbolOpt = GetUpdatedSymbol(node, node.LookupSymbolOpt);
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            BoundMethodGroup updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.TypeArgumentsOpt, node.Name, methods, lookupSymbolOpt, node.LookupError, node.Flags, receiverOpt, node.ResultKind);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.TypeArgumentsOpt, node.Name, methods, lookupSymbolOpt, node.LookupError, node.Flags, receiverOpt, node.ResultKind);
            }
            return updatedNode;
        }

        public override BoundNode? VisitPropertyGroup(BoundPropertyGroup node)
        {
            ImmutableArray<PropertySymbol> properties = GetUpdatedArray(node, node.Properties);
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            BoundPropertyGroup updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(properties, receiverOpt, node.ResultKind);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(properties, receiverOpt, node.ResultKind);
            }
            return updatedNode;
        }

        public override BoundNode? VisitCall(BoundCall node)
        {
            MethodSymbol method = GetUpdatedSymbol(node, node.Method);
            ImmutableArray<MethodSymbol> originalMethodsOpt = GetUpdatedArray(node, node.OriginalMethodsOpt);
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundCall updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiverOpt, method, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.IsDelegateCall, node.Expanded, node.InvokedAsExtensionMethod, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, node.OriginalMethodsOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiverOpt, method, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.IsDelegateCall, node.Expanded, node.InvokedAsExtensionMethod, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, node.OriginalMethodsOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            EventSymbol @event = GetUpdatedSymbol(node, node.Event);
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            BoundExpression argument = (BoundExpression)this.Visit(node.Argument);
            BoundEventAssignmentOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(@event, node.IsAddition, node.IsDynamic, receiverOpt, argument, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(@event, node.IsAddition, node.IsDynamic, receiverOpt, argument, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitAttribute(BoundAttribute node)
        {
            MethodSymbol? constructor = GetUpdatedSymbol(node, node.Constructor);
            ImmutableArray<BoundExpression> constructorArguments = this.VisitList(node.ConstructorArguments);
            ImmutableArray<BoundAssignmentOperator> namedArguments = this.VisitList(node.NamedArguments);
            BoundAttribute updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(constructor, constructorArguments, node.ConstructorArgumentNamesOpt, node.ConstructorArgumentsToParamsOpt, node.ConstructorExpanded, namedArguments, node.ResultKind, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(constructor, constructorArguments, node.ConstructorArgumentNamesOpt, node.ConstructorArgumentsToParamsOpt, node.ConstructorExpanded, namedArguments, node.ResultKind, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundUnconvertedObjectCreationExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.InitializerOpt);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.InitializerOpt);
            }
            return updatedNode;
        }

        public override BoundNode? VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            MethodSymbol constructor = GetUpdatedSymbol(node, node.Constructor);
            ImmutableArray<MethodSymbol> constructorsGroup = GetUpdatedArray(node, node.ConstructorsGroup);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundObjectInitializerExpressionBase? initializerExpressionOpt = (BoundObjectInitializerExpressionBase?)this.Visit(node.InitializerExpressionOpt);
            BoundObjectCreationExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(constructor, constructorsGroup, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ConstantValueOpt, initializerExpressionOpt, node.WasTargetTyped, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(constructor, constructorsGroup, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ConstantValueOpt, initializerExpressionOpt, node.WasTargetTyped, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitTupleLiteral(BoundTupleLiteral node)
        {
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundTupleLiteral updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(arguments, node.ArgumentNamesOpt, node.InferredNamesOpt, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(arguments, node.ArgumentNamesOpt, node.InferredNamesOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
        {
            BoundTupleLiteral? sourceTuple = (BoundTupleLiteral?)this.Visit(node.SourceTuple);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundConvertedTupleLiteral updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(sourceTuple, node.WasTargetTyped, arguments, node.ArgumentNamesOpt, node.InferredNamesOpt, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(sourceTuple, node.WasTargetTyped, arguments, node.ArgumentNamesOpt, node.InferredNamesOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
        {
            ImmutableArray<MethodSymbol> applicableMethods = GetUpdatedArray(node, node.ApplicableMethods);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundObjectInitializerExpressionBase? initializerExpressionOpt = (BoundObjectInitializerExpressionBase?)this.Visit(node.InitializerExpressionOpt);
            BoundDynamicObjectCreationExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.Name, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, initializerExpressionOpt, applicableMethods, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.Name, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, initializerExpressionOpt, applicableMethods, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
        {
            BoundObjectInitializerExpressionBase? initializerExpressionOpt = (BoundObjectInitializerExpressionBase?)this.Visit(node.InitializerExpressionOpt);
            BoundNoPiaObjectCreationExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.GuidString, initializerExpressionOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.GuidString, initializerExpressionOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
        {
            BoundObjectOrCollectionValuePlaceholder placeholder = (BoundObjectOrCollectionValuePlaceholder)this.Visit(node.Placeholder);
            ImmutableArray<BoundExpression> initializers = this.VisitList(node.Initializers);
            BoundObjectInitializerExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(placeholder, initializers, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(placeholder, initializers, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitObjectInitializerMember(BoundObjectInitializerMember node)
        {
            Symbol? memberSymbol = GetUpdatedSymbol(node, node.MemberSymbol);
            TypeSymbol receiverType = GetUpdatedSymbol(node, node.ReceiverType);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundObjectInitializerMember updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(memberSymbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, receiverType, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(memberSymbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, receiverType, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node)
        {
            TypeSymbol receiverType = GetUpdatedSymbol(node, node.ReceiverType);
            BoundDynamicObjectInitializerMember updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(node.MemberName, receiverType, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(node.MemberName, receiverType, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
        {
            BoundObjectOrCollectionValuePlaceholder placeholder = (BoundObjectOrCollectionValuePlaceholder)this.Visit(node.Placeholder);
            ImmutableArray<BoundExpression> initializers = this.VisitList(node.Initializers);
            BoundCollectionInitializerExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(placeholder, initializers, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(placeholder, initializers, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitCollectionElementInitializer(BoundCollectionElementInitializer node)
        {
            MethodSymbol addMethod = GetUpdatedSymbol(node, node.AddMethod);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundExpression? implicitReceiverOpt = (BoundExpression?)this.Visit(node.ImplicitReceiverOpt);
            BoundCollectionElementInitializer updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(addMethod, arguments, implicitReceiverOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.InvokedAsExtensionMethod, node.ResultKind, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(addMethod, arguments, implicitReceiverOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.InvokedAsExtensionMethod, node.ResultKind, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node)
        {
            ImmutableArray<MethodSymbol> applicableMethods = GetUpdatedArray(node, node.ApplicableMethods);
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundDynamicCollectionElementInitializer updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(applicableMethods, expression, arguments, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(applicableMethods, expression, arguments, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitImplicitReceiver(BoundImplicitReceiver node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundImplicitReceiver updatedNode = node.Update(infoAndType.Type!);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node)
        {
            MethodSymbol constructor = GetUpdatedSymbol(node, node.Constructor);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            ImmutableArray<BoundAnonymousPropertyDeclaration> declarations = this.VisitList(node.Declarations);
            BoundAnonymousObjectCreationExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(constructor, arguments, declarations, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(constructor, arguments, declarations, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node)
        {
            PropertySymbol property = GetUpdatedSymbol(node, node.Property);
            BoundAnonymousPropertyDeclaration updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(property, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(property, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitNewT(BoundNewT node)
        {
            BoundObjectInitializerExpressionBase? initializerExpressionOpt = (BoundObjectInitializerExpressionBase?)this.Visit(node.InitializerExpressionOpt);
            BoundNewT updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(initializerExpressionOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(initializerExpressionOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            MethodSymbol? methodOpt = GetUpdatedSymbol(node, node.MethodOpt);
            BoundExpression argument = (BoundExpression)this.Visit(node.Argument);
            BoundDelegateCreationExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(argument, methodOpt, node.IsExtensionMethod, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(argument, methodOpt, node.IsExtensionMethod, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitArrayCreation(BoundArrayCreation node)
        {
            ImmutableArray<BoundExpression> bounds = this.VisitList(node.Bounds);
            BoundArrayInitialization? initializerOpt = (BoundArrayInitialization?)this.Visit(node.InitializerOpt);
            BoundArrayCreation updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(bounds, initializerOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(bounds, initializerOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitArrayInitialization(BoundArrayInitialization node)
        {
            ImmutableArray<BoundExpression> initializers = this.VisitList(node.Initializers);
            BoundArrayInitialization updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(initializers);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(initializers);
            }
            return updatedNode;
        }

        public override BoundNode? VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node)
        {
            TypeSymbol elementType = GetUpdatedSymbol(node, node.ElementType);
            BoundExpression count = (BoundExpression)this.Visit(node.Count);
            BoundArrayInitialization? initializerOpt = (BoundArrayInitialization?)this.Visit(node.InitializerOpt);
            BoundStackAllocArrayCreation updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(elementType, count, initializerOpt, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(elementType, count, initializerOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node)
        {
            TypeSymbol elementType = GetUpdatedSymbol(node, node.ElementType);
            BoundExpression count = (BoundExpression)this.Visit(node.Count);
            BoundArrayInitialization? initializerOpt = (BoundArrayInitialization?)this.Visit(node.InitializerOpt);
            BoundConvertedStackAllocExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(elementType, count, initializerOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(elementType, count, initializerOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitFieldAccess(BoundFieldAccess node)
        {
            FieldSymbol fieldSymbol = GetUpdatedSymbol(node, node.FieldSymbol);
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            BoundFieldAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiverOpt, fieldSymbol, node.ConstantValueOpt, node.ResultKind, node.IsByValue, node.IsDeclaration, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiverOpt, fieldSymbol, node.ConstantValueOpt, node.ResultKind, node.IsByValue, node.IsDeclaration, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitHoistedFieldAccess(BoundHoistedFieldAccess node)
        {
            FieldSymbol fieldSymbol = GetUpdatedSymbol(node, node.FieldSymbol);
            BoundHoistedFieldAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(fieldSymbol, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(fieldSymbol, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitPropertyAccess(BoundPropertyAccess node)
        {
            PropertySymbol propertySymbol = GetUpdatedSymbol(node, node.PropertySymbol);
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            BoundPropertyAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiverOpt, propertySymbol, node.ResultKind, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiverOpt, propertySymbol, node.ResultKind, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitEventAccess(BoundEventAccess node)
        {
            EventSymbol eventSymbol = GetUpdatedSymbol(node, node.EventSymbol);
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            BoundEventAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiverOpt, eventSymbol, node.IsUsableAsField, node.ResultKind, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiverOpt, eventSymbol, node.IsUsableAsField, node.ResultKind, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitIndexerAccess(BoundIndexerAccess node)
        {
            PropertySymbol indexer = GetUpdatedSymbol(node, node.Indexer);
            ImmutableArray<PropertySymbol> originalIndexersOpt = GetUpdatedArray(node, node.OriginalIndexersOpt);
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundIndexerAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiverOpt, indexer, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.OriginalIndexersOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiverOpt, indexer, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.OriginalIndexersOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node)
        {
            PropertySymbol lengthOrCountProperty = GetUpdatedSymbol(node, node.LengthOrCountProperty);
            Symbol patternSymbol = GetUpdatedSymbol(node, node.PatternSymbol);
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            BoundExpression argument = (BoundExpression)this.Visit(node.Argument);
            BoundIndexOrRangePatternIndexerAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiver, lengthOrCountProperty, patternSymbol, argument, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiver, lengthOrCountProperty, patternSymbol, argument, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node)
        {
            ImmutableArray<PropertySymbol> applicableIndexers = GetUpdatedArray(node, node.ApplicableIndexers);
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            ImmutableArray<BoundExpression> arguments = this.VisitList(node.Arguments);
            BoundDynamicIndexerAccess updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiver, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, applicableIndexers, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiver, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, applicableIndexers, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitLambda(BoundLambda node)
        {
            LambdaSymbol symbol = GetUpdatedSymbol(node, node.Symbol);
            UnboundLambda unboundLambda = node.UnboundLambda;
            BoundBlock body = (BoundBlock)this.Visit(node.Body);
            BoundLambda updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(unboundLambda, symbol, body, node.Diagnostics, node.Binder, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(unboundLambda, symbol, body, node.Diagnostics, node.Binder, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitUnboundLambda(UnboundLambda node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            UnboundLambda updatedNode = node.Update(node.Data, node.WithDependencies);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitQueryClause(BoundQueryClause node)
        {
            RangeVariableSymbol? definedSymbol = GetUpdatedSymbol(node, node.DefinedSymbol);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            BoundExpression? operation = node.Operation;
            BoundExpression? cast = node.Cast;
            BoundExpression? unoptimizedForm = node.UnoptimizedForm;
            BoundQueryClause updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(value, definedSymbol, operation, cast, node.Binder, unoptimizedForm, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(value, definedSymbol, operation, cast, node.Binder, unoptimizedForm, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitNameOfOperator(BoundNameOfOperator node)
        {
            BoundExpression argument = (BoundExpression)this.Visit(node.Argument);
            BoundNameOfOperator updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(argument, node.ConstantValueOpt, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(argument, node.ConstantValueOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node)
        {
            ImmutableArray<BoundExpression> parts = this.VisitList(node.Parts);
            BoundUnconvertedInterpolatedString updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(parts, node.ConstantValueOpt, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(parts, node.ConstantValueOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitInterpolatedString(BoundInterpolatedString node)
        {
            ImmutableArray<BoundExpression> parts = this.VisitList(node.Parts);
            BoundInterpolatedString updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(parts, node.ConstantValueOpt, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(parts, node.ConstantValueOpt, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitStringInsert(BoundStringInsert node)
        {
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            BoundExpression? alignment = (BoundExpression?)this.Visit(node.Alignment);
            BoundLiteral? format = (BoundLiteral?)this.Visit(node.Format);
            BoundStringInsert updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(value, alignment, format, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(value, alignment, format, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitIsPatternExpression(BoundIsPatternExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundPattern pattern = (BoundPattern)this.Visit(node.Pattern);
            BoundDecisionDag decisionDag = node.DecisionDag;
            BoundIsPatternExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(expression, pattern, node.IsNegated, decisionDag, node.WhenTrueLabel, node.WhenFalseLabel, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(expression, pattern, node.IsNegated, decisionDag, node.WhenTrueLabel, node.WhenFalseLabel, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitConstantPattern(BoundConstantPattern node)
        {
            TypeSymbol inputType = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol narrowedType = GetUpdatedSymbol(node, node.NarrowedType);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            return node.Update(value, node.ConstantValue, inputType, narrowedType);
        }

        public override BoundNode? VisitDiscardPattern(BoundDiscardPattern node)
        {
            TypeSymbol inputType = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol narrowedType = GetUpdatedSymbol(node, node.NarrowedType);
            return node.Update(inputType, narrowedType);
        }

        public override BoundNode? VisitDeclarationPattern(BoundDeclarationPattern node)
        {
            Symbol? variable = GetUpdatedSymbol(node, node.Variable);
            TypeSymbol inputType = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol narrowedType = GetUpdatedSymbol(node, node.NarrowedType);
            BoundExpression? variableAccess = (BoundExpression?)this.Visit(node.VariableAccess);
            BoundTypeExpression declaredType = (BoundTypeExpression)this.Visit(node.DeclaredType);
            return node.Update(variable, variableAccess, declaredType, node.IsVar, inputType, narrowedType);
        }

        public override BoundNode? VisitRecursivePattern(BoundRecursivePattern node)
        {
            MethodSymbol? deconstructMethod = GetUpdatedSymbol(node, node.DeconstructMethod);
            Symbol? variable = GetUpdatedSymbol(node, node.Variable);
            TypeSymbol inputType = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol narrowedType = GetUpdatedSymbol(node, node.NarrowedType);
            BoundTypeExpression? declaredType = (BoundTypeExpression?)this.Visit(node.DeclaredType);
            ImmutableArray<BoundSubpattern> deconstruction = this.VisitList(node.Deconstruction);
            ImmutableArray<BoundSubpattern> properties = this.VisitList(node.Properties);
            BoundExpression? variableAccess = (BoundExpression?)this.Visit(node.VariableAccess);
            return node.Update(declaredType, deconstructMethod, deconstruction, properties, variable, variableAccess, node.IsExplicitNotNullTest, inputType, narrowedType);
        }

        public override BoundNode? VisitITuplePattern(BoundITuplePattern node)
        {
            MethodSymbol getLengthMethod = GetUpdatedSymbol(node, node.GetLengthMethod);
            MethodSymbol getItemMethod = GetUpdatedSymbol(node, node.GetItemMethod);
            TypeSymbol inputType = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol narrowedType = GetUpdatedSymbol(node, node.NarrowedType);
            ImmutableArray<BoundSubpattern> subpatterns = this.VisitList(node.Subpatterns);
            return node.Update(getLengthMethod, getItemMethod, subpatterns, inputType, narrowedType);
        }

        public override BoundNode? VisitSubpattern(BoundSubpattern node)
        {
            Symbol? symbol = GetUpdatedSymbol(node, node.Symbol);
            BoundPattern pattern = (BoundPattern)this.Visit(node.Pattern);
            return node.Update(symbol, pattern);
        }

        public override BoundNode? VisitTypePattern(BoundTypePattern node)
        {
            TypeSymbol inputType = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol narrowedType = GetUpdatedSymbol(node, node.NarrowedType);
            BoundTypeExpression declaredType = (BoundTypeExpression)this.Visit(node.DeclaredType);
            return node.Update(declaredType, node.IsExplicitNotNullTest, inputType, narrowedType);
        }

        public override BoundNode? VisitBinaryPattern(BoundBinaryPattern node)
        {
            TypeSymbol inputType = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol narrowedType = GetUpdatedSymbol(node, node.NarrowedType);
            BoundPattern left = (BoundPattern)this.Visit(node.Left);
            BoundPattern right = (BoundPattern)this.Visit(node.Right);
            return node.Update(node.Disjunction, left, right, inputType, narrowedType);
        }

        public override BoundNode? VisitNegatedPattern(BoundNegatedPattern node)
        {
            TypeSymbol inputType = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol narrowedType = GetUpdatedSymbol(node, node.NarrowedType);
            BoundPattern negated = (BoundPattern)this.Visit(node.Negated);
            return node.Update(negated, inputType, narrowedType);
        }

        public override BoundNode? VisitRelationalPattern(BoundRelationalPattern node)
        {
            TypeSymbol inputType = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol narrowedType = GetUpdatedSymbol(node, node.NarrowedType);
            BoundExpression value = (BoundExpression)this.Visit(node.Value);
            return node.Update(node.Relation, value, node.ConstantValue, inputType, narrowedType);
        }

        public override BoundNode? VisitDiscardExpression(BoundDiscardExpression node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            BoundDiscardExpression updatedNode = node.Update(infoAndType.Type);
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitThrowExpression(BoundThrowExpression node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundThrowExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(expression, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(expression, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitOutVariablePendingInference(OutVariablePendingInference node)
        {
            Symbol variableSymbol = GetUpdatedSymbol(node, node.VariableSymbol);
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            OutVariablePendingInference updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(variableSymbol, receiverOpt);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(variableSymbol, receiverOpt);
            }
            return updatedNode;
        }

        public override BoundNode? VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node)
        {
            Symbol variableSymbol = GetUpdatedSymbol(node, node.VariableSymbol);
            BoundExpression? receiverOpt = (BoundExpression?)this.Visit(node.ReceiverOpt);
            DeconstructionVariablePendingInference updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(variableSymbol, receiverOpt);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(variableSymbol, receiverOpt);
            }
            return updatedNode;
        }

        public override BoundNode? VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                return node;
            }

            OutDeconstructVarPendingInference updatedNode = node.Update();
            updatedNode.TopLevelNullability = infoAndType.Info;
            return updatedNode;
        }

        public override BoundNode? VisitConstructorMethodBody(BoundConstructorMethodBody node)
        {
            ImmutableArray<LocalSymbol> locals = GetUpdatedArray(node, node.Locals);
            BoundExpressionStatement? initializer = (BoundExpressionStatement?)this.Visit(node.Initializer);
            BoundBlock? blockBody = (BoundBlock?)this.Visit(node.BlockBody);
            BoundBlock? expressionBody = (BoundBlock?)this.Visit(node.ExpressionBody);
            return node.Update(locals, initializer, blockBody, expressionBody);
        }

        public override BoundNode? VisitExpressionWithNullability(BoundExpressionWithNullability node)
        {
            BoundExpression expression = (BoundExpression)this.Visit(node.Expression);
            BoundExpressionWithNullability updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(expression, node.NullableAnnotation, infoAndType.Type);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(expression, node.NullableAnnotation, node.Type);
            }
            return updatedNode;
        }

        public override BoundNode? VisitWithExpression(BoundWithExpression node)
        {
            MethodSymbol? cloneMethod = GetUpdatedSymbol(node, node.CloneMethod);
            BoundExpression receiver = (BoundExpression)this.Visit(node.Receiver);
            BoundObjectInitializerExpressionBase initializerExpression = (BoundObjectInitializerExpressionBase)this.Visit(node.InitializerExpression);
            BoundWithExpression updatedNode;

            if (_updatedNullabilities.TryGetValue(node, out (NullabilityInfo Info, TypeSymbol? Type) infoAndType))
            {
                updatedNode = node.Update(receiver, cloneMethod, initializerExpression, infoAndType.Type!);
                updatedNode.TopLevelNullability = infoAndType.Info;
            }
            else
            {
                updatedNode = node.Update(receiver, cloneMethod, initializerExpression, node.Type);
            }
            return updatedNode;
        }
    }

    internal sealed class BoundTreeDumperNodeProducer : BoundTreeVisitor<object?, TreeDumperNode>
    {
        private BoundTreeDumperNodeProducer()
        {
        }
        public static TreeDumperNode MakeTree(BoundNode node) => (new BoundTreeDumperNodeProducer()).Visit(node, null);
        public override TreeDumperNode VisitFieldEqualsValue(BoundFieldEqualsValue node, object? arg) => new TreeDumperNode("fieldEqualsValue", null, new TreeDumperNode[]
        {
            new TreeDumperNode("field", node.Field, null),
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitPropertyEqualsValue(BoundPropertyEqualsValue node, object? arg) => new TreeDumperNode("propertyEqualsValue", null, new TreeDumperNode[]
        {
            new TreeDumperNode("property", node.Property, null),
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitParameterEqualsValue(BoundParameterEqualsValue node, object? arg) => new TreeDumperNode("parameterEqualsValue", null, new TreeDumperNode[]
        {
            new TreeDumperNode("parameter", node.Parameter, null),
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node, object? arg) => new TreeDumperNode("globalStatementInitializer", null, new TreeDumperNode[]
        {
            new TreeDumperNode("statement", null, new TreeDumperNode[] { Visit(node.Statement, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node, object? arg) => new TreeDumperNode("deconstructValuePlaceholder", null, new TreeDumperNode[]
        {
            new TreeDumperNode("valEscape", node.ValEscape, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node, object? arg) => new TreeDumperNode("tupleOperandPlaceholder", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node, object? arg) => new TreeDumperNode("awaitableValuePlaceholder", null, new TreeDumperNode[]
        {
            new TreeDumperNode("valEscape", node.ValEscape, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDisposableValuePlaceholder(BoundDisposableValuePlaceholder node, object? arg) => new TreeDumperNode("disposableValuePlaceholder", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node, object? arg) => new TreeDumperNode("objectOrCollectionValuePlaceholder", null, new TreeDumperNode[]
        {
            new TreeDumperNode("isNewInstance", node.IsNewInstance, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDup(BoundDup node, object? arg) => new TreeDumperNode("dup", null, new TreeDumperNode[]
        {
            new TreeDumperNode("refKind", node.RefKind, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitPassByCopy(BoundPassByCopy node, object? arg) => new TreeDumperNode("passByCopy", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitBadExpression(BoundBadExpression node, object? arg) => new TreeDumperNode("badExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("symbols", node.Symbols, null),
            new TreeDumperNode("childBoundNodes", null, from x in node.ChildBoundNodes select Visit(x, null)),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitBadStatement(BoundBadStatement node, object? arg) => new TreeDumperNode("badStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("childBoundNodes", null, from x in node.ChildBoundNodes select Visit(x, null)),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitExtractedFinallyBlock(BoundExtractedFinallyBlock node, object? arg) => new TreeDumperNode("extractedFinallyBlock", null, new TreeDumperNode[]
        {
            new TreeDumperNode("finallyBlock", null, new TreeDumperNode[] { Visit(node.FinallyBlock, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitTypeExpression(BoundTypeExpression node, object? arg) => new TreeDumperNode("typeExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("aliasOpt", node.AliasOpt, null),
            new TreeDumperNode("boundContainingTypeOpt", null, new TreeDumperNode[] { Visit(node.BoundContainingTypeOpt, null) }),
            new TreeDumperNode("boundDimensionsOpt", null, node.BoundDimensionsOpt.IsDefault ? new TreeDumperNode[0] : from x in node.BoundDimensionsOpt select Visit(x, null)),
            new TreeDumperNode("typeWithAnnotations", node.TypeWithAnnotations, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitTypeOrValueExpression(BoundTypeOrValueExpression node, object? arg) => new TreeDumperNode("typeOrValueExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("data", node.Data, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitNamespaceExpression(BoundNamespaceExpression node, object? arg) => new TreeDumperNode("namespaceExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("namespaceSymbol", node.NamespaceSymbol, null),
            new TreeDumperNode("aliasOpt", node.AliasOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitUnaryOperator(BoundUnaryOperator node, object? arg) => new TreeDumperNode("unaryOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operatorKind", node.OperatorKind, null),
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("methodOpt", node.MethodOpt, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("originalUserDefinedOperatorsOpt", node.OriginalUserDefinedOperatorsOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitIncrementOperator(BoundIncrementOperator node, object? arg) => new TreeDumperNode("incrementOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operatorKind", node.OperatorKind, null),
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("methodOpt", node.MethodOpt, null),
            new TreeDumperNode("operandConversion", node.OperandConversion, null),
            new TreeDumperNode("resultConversion", node.ResultConversion, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("originalUserDefinedOperatorsOpt", node.OriginalUserDefinedOperatorsOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitAddressOfOperator(BoundAddressOfOperator node, object? arg) => new TreeDumperNode("addressOfOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("isManaged", node.IsManaged, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node, object? arg) => new TreeDumperNode("unconvertedAddressOfOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitFunctionPointerLoad(BoundFunctionPointerLoad node, object? arg) => new TreeDumperNode("functionPointerLoad", null, new TreeDumperNode[]
        {
            new TreeDumperNode("targetMethod", node.TargetMethod, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node, object? arg) => new TreeDumperNode("pointerIndirectionOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitPointerElementAccess(BoundPointerElementAccess node, object? arg) => new TreeDumperNode("pointerElementAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("index", null, new TreeDumperNode[] { Visit(node.Index, null) }),
            new TreeDumperNode("@checked", node.Checked, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node, object? arg) => new TreeDumperNode("functionPointerInvocation", null, new TreeDumperNode[]
        {
            new TreeDumperNode("invokedExpression", null, new TreeDumperNode[] { Visit(node.InvokedExpression, null) }),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitRefTypeOperator(BoundRefTypeOperator node, object? arg) => new TreeDumperNode("refTypeOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("getTypeFromHandle", node.GetTypeFromHandle, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitMakeRefOperator(BoundMakeRefOperator node, object? arg) => new TreeDumperNode("makeRefOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitRefValueOperator(BoundRefValueOperator node, object? arg) => new TreeDumperNode("refValueOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("nullableAnnotation", node.NullableAnnotation, null),
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitFromEndIndexExpression(BoundFromEndIndexExpression node, object? arg) => new TreeDumperNode("fromEndIndexExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("methodOpt", node.MethodOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitRangeExpression(BoundRangeExpression node, object? arg) => new TreeDumperNode("rangeExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("leftOperandOpt", null, new TreeDumperNode[] { Visit(node.LeftOperandOpt, null) }),
            new TreeDumperNode("rightOperandOpt", null, new TreeDumperNode[] { Visit(node.RightOperandOpt, null) }),
            new TreeDumperNode("methodOpt", node.MethodOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitBinaryOperator(BoundBinaryOperator node, object? arg) => new TreeDumperNode("binaryOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operatorKind", node.OperatorKind, null),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("methodOpt", node.MethodOpt, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("originalUserDefinedOperatorsOpt", node.OriginalUserDefinedOperatorsOpt, null),
            new TreeDumperNode("left", null, new TreeDumperNode[] { Visit(node.Left, null) }),
            new TreeDumperNode("right", null, new TreeDumperNode[] { Visit(node.Right, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitTupleBinaryOperator(BoundTupleBinaryOperator node, object? arg) => new TreeDumperNode("tupleBinaryOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("left", null, new TreeDumperNode[] { Visit(node.Left, null) }),
            new TreeDumperNode("right", null, new TreeDumperNode[] { Visit(node.Right, null) }),
            new TreeDumperNode("operatorKind", node.OperatorKind, null),
            new TreeDumperNode("operators", node.Operators, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node, object? arg) => new TreeDumperNode("userDefinedConditionalLogicalOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operatorKind", node.OperatorKind, null),
            new TreeDumperNode("logicalOperator", node.LogicalOperator, null),
            new TreeDumperNode("trueOperator", node.TrueOperator, null),
            new TreeDumperNode("falseOperator", node.FalseOperator, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("originalUserDefinedOperatorsOpt", node.OriginalUserDefinedOperatorsOpt, null),
            new TreeDumperNode("left", null, new TreeDumperNode[] { Visit(node.Left, null) }),
            new TreeDumperNode("right", null, new TreeDumperNode[] { Visit(node.Right, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node, object? arg) => new TreeDumperNode("compoundAssignmentOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("@operator", node.Operator, null),
            new TreeDumperNode("left", null, new TreeDumperNode[] { Visit(node.Left, null) }),
            new TreeDumperNode("right", null, new TreeDumperNode[] { Visit(node.Right, null) }),
            new TreeDumperNode("leftConversion", node.LeftConversion, null),
            new TreeDumperNode("finalConversion", node.FinalConversion, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("originalUserDefinedOperatorsOpt", node.OriginalUserDefinedOperatorsOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitAssignmentOperator(BoundAssignmentOperator node, object? arg) => new TreeDumperNode("assignmentOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("left", null, new TreeDumperNode[] { Visit(node.Left, null) }),
            new TreeDumperNode("right", null, new TreeDumperNode[] { Visit(node.Right, null) }),
            new TreeDumperNode("isRef", node.IsRef, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node, object? arg) => new TreeDumperNode("deconstructionAssignmentOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("left", null, new TreeDumperNode[] { Visit(node.Left, null) }),
            new TreeDumperNode("right", null, new TreeDumperNode[] { Visit(node.Right, null) }),
            new TreeDumperNode("isUsed", node.IsUsed, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitNullCoalescingOperator(BoundNullCoalescingOperator node, object? arg) => new TreeDumperNode("nullCoalescingOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("leftOperand", null, new TreeDumperNode[] { Visit(node.LeftOperand, null) }),
            new TreeDumperNode("rightOperand", null, new TreeDumperNode[] { Visit(node.RightOperand, null) }),
            new TreeDumperNode("leftConversion", node.LeftConversion, null),
            new TreeDumperNode("operatorResultKind", node.OperatorResultKind, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node, object? arg) => new TreeDumperNode("nullCoalescingAssignmentOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("leftOperand", null, new TreeDumperNode[] { Visit(node.LeftOperand, null) }),
            new TreeDumperNode("rightOperand", null, new TreeDumperNode[] { Visit(node.RightOperand, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node, object? arg) => new TreeDumperNode("unconvertedConditionalOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("condition", null, new TreeDumperNode[] { Visit(node.Condition, null) }),
            new TreeDumperNode("consequence", null, new TreeDumperNode[] { Visit(node.Consequence, null) }),
            new TreeDumperNode("alternative", null, new TreeDumperNode[] { Visit(node.Alternative, null) }),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("noCommonTypeError", node.NoCommonTypeError, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitConditionalOperator(BoundConditionalOperator node, object? arg) => new TreeDumperNode("conditionalOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("isRef", node.IsRef, null),
            new TreeDumperNode("condition", null, new TreeDumperNode[] { Visit(node.Condition, null) }),
            new TreeDumperNode("consequence", null, new TreeDumperNode[] { Visit(node.Consequence, null) }),
            new TreeDumperNode("alternative", null, new TreeDumperNode[] { Visit(node.Alternative, null) }),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("naturalTypeOpt", node.NaturalTypeOpt, null),
            new TreeDumperNode("wasTargetTyped", node.WasTargetTyped, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitArrayAccess(BoundArrayAccess node, object? arg) => new TreeDumperNode("arrayAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("indices", null, from x in node.Indices select Visit(x, null)),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitArrayLength(BoundArrayLength node, object? arg) => new TreeDumperNode("arrayLength", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitAwaitableInfo(BoundAwaitableInfo node, object? arg) => new TreeDumperNode("awaitableInfo", null, new TreeDumperNode[]
        {
            new TreeDumperNode("awaitableInstancePlaceholder", null, new TreeDumperNode[] { Visit(node.AwaitableInstancePlaceholder, null) }),
            new TreeDumperNode("isDynamic", node.IsDynamic, null),
            new TreeDumperNode("getAwaiter", null, new TreeDumperNode[] { Visit(node.GetAwaiter, null) }),
            new TreeDumperNode("isCompleted", node.IsCompleted, null),
            new TreeDumperNode("getResult", node.GetResult, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitAwaitExpression(BoundAwaitExpression node, object? arg) => new TreeDumperNode("awaitExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("awaitableInfo", null, new TreeDumperNode[] { Visit(node.AwaitableInfo, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitTypeOfOperator(BoundTypeOfOperator node, object? arg) => new TreeDumperNode("typeOfOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("sourceType", null, new TreeDumperNode[] { Visit(node.SourceType, null) }),
            new TreeDumperNode("getTypeFromHandle", node.GetTypeFromHandle, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitMethodDefIndex(BoundMethodDefIndex node, object? arg) => new TreeDumperNode("methodDefIndex", null, new TreeDumperNode[]
        {
            new TreeDumperNode("method", node.Method, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node, object? arg) => new TreeDumperNode("maximumMethodDefIndex", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node, object? arg) => new TreeDumperNode("instrumentationPayloadRoot", null, new TreeDumperNode[]
        {
            new TreeDumperNode("analysisKind", node.AnalysisKind, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitModuleVersionId(BoundModuleVersionId node, object? arg) => new TreeDumperNode("moduleVersionId", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitModuleVersionIdString(BoundModuleVersionIdString node, object? arg) => new TreeDumperNode("moduleVersionIdString", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSourceDocumentIndex(BoundSourceDocumentIndex node, object? arg) => new TreeDumperNode("sourceDocumentIndex", null, new TreeDumperNode[]
        {
            new TreeDumperNode("document", node.Document, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitMethodInfo(BoundMethodInfo node, object? arg) => new TreeDumperNode("methodInfo", null, new TreeDumperNode[]
        {
            new TreeDumperNode("method", node.Method, null),
            new TreeDumperNode("getMethodFromHandle", node.GetMethodFromHandle, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitFieldInfo(BoundFieldInfo node, object? arg) => new TreeDumperNode("fieldInfo", null, new TreeDumperNode[]
        {
            new TreeDumperNode("field", node.Field, null),
            new TreeDumperNode("getFieldFromHandle", node.GetFieldFromHandle, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDefaultLiteral(BoundDefaultLiteral node, object? arg) => new TreeDumperNode("defaultLiteral", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDefaultExpression(BoundDefaultExpression node, object? arg) => new TreeDumperNode("defaultExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("targetType", null, new TreeDumperNode[] { Visit(node.TargetType, null) }),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitIsOperator(BoundIsOperator node, object? arg) => new TreeDumperNode("isOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("targetType", null, new TreeDumperNode[] { Visit(node.TargetType, null) }),
            new TreeDumperNode("conversion", node.Conversion, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitAsOperator(BoundAsOperator node, object? arg) => new TreeDumperNode("asOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("targetType", null, new TreeDumperNode[] { Visit(node.TargetType, null) }),
            new TreeDumperNode("conversion", node.Conversion, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSizeOfOperator(BoundSizeOfOperator node, object? arg) => new TreeDumperNode("sizeOfOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("sourceType", null, new TreeDumperNode[] { Visit(node.SourceType, null) }),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitConversion(BoundConversion node, object? arg) => new TreeDumperNode("conversion", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("conversion", node.Conversion, null),
            new TreeDumperNode("isBaseConversion", node.IsBaseConversion, null),
            new TreeDumperNode("@checked", node.Checked, null),
            new TreeDumperNode("explicitCastInCode", node.ExplicitCastInCode, null),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("conversionGroupOpt", node.ConversionGroupOpt, null),
            new TreeDumperNode("originalUserDefinedConversionsOpt", node.OriginalUserDefinedConversionsOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node, object? arg) => new TreeDumperNode("readOnlySpanFromArray", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operand", null, new TreeDumperNode[] { Visit(node.Operand, null) }),
            new TreeDumperNode("conversionMethod", node.ConversionMethod, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitArgList(BoundArgList node, object? arg) => new TreeDumperNode("argList", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitArgListOperator(BoundArgListOperator node, object? arg) => new TreeDumperNode("argListOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node, object? arg) => new TreeDumperNode("fixedLocalCollectionInitializer", null, new TreeDumperNode[]
        {
            new TreeDumperNode("elementPointerType", node.ElementPointerType, null),
            new TreeDumperNode("elementPointerTypeConversion", node.ElementPointerTypeConversion, null),
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("getPinnableOpt", node.GetPinnableOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSequencePoint(BoundSequencePoint node, object? arg) => new TreeDumperNode("sequencePoint", null, new TreeDumperNode[]
        {
            new TreeDumperNode("statementOpt", null, new TreeDumperNode[] { Visit(node.StatementOpt, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSequencePointWithSpan(BoundSequencePointWithSpan node, object? arg) => new TreeDumperNode("sequencePointWithSpan", null, new TreeDumperNode[]
        {
            new TreeDumperNode("statementOpt", null, new TreeDumperNode[] { Visit(node.StatementOpt, null) }),
            new TreeDumperNode("span", node.Span, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSavePreviousSequencePoint(BoundSavePreviousSequencePoint node, object? arg) => new TreeDumperNode("savePreviousSequencePoint", null, new TreeDumperNode[]
        {
            new TreeDumperNode("identifier", node.Identifier, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitRestorePreviousSequencePoint(BoundRestorePreviousSequencePoint node, object? arg) => new TreeDumperNode("restorePreviousSequencePoint", null, new TreeDumperNode[]
        {
            new TreeDumperNode("identifier", node.Identifier, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitStepThroughSequencePoint(BoundStepThroughSequencePoint node, object? arg) => new TreeDumperNode("stepThroughSequencePoint", null, new TreeDumperNode[]
        {
            new TreeDumperNode("span", node.Span, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitBlock(BoundBlock node, object? arg) => new TreeDumperNode("block", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("localFunctions", node.LocalFunctions, null),
            new TreeDumperNode("statements", null, from x in node.Statements select Visit(x, null)),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitScope(BoundScope node, object? arg) => new TreeDumperNode("scope", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("statements", null, from x in node.Statements select Visit(x, null)),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitStateMachineScope(BoundStateMachineScope node, object? arg) => new TreeDumperNode("stateMachineScope", null, new TreeDumperNode[]
        {
            new TreeDumperNode("fields", node.Fields, null),
            new TreeDumperNode("statement", null, new TreeDumperNode[] { Visit(node.Statement, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLocalDeclaration(BoundLocalDeclaration node, object? arg) => new TreeDumperNode("localDeclaration", null, new TreeDumperNode[]
        {
            new TreeDumperNode("localSymbol", node.LocalSymbol, null),
            new TreeDumperNode("declaredTypeOpt", null, new TreeDumperNode[] { Visit(node.DeclaredTypeOpt, null) }),
            new TreeDumperNode("initializerOpt", null, new TreeDumperNode[] { Visit(node.InitializerOpt, null) }),
            new TreeDumperNode("argumentsOpt", null, node.ArgumentsOpt.IsDefault ? new TreeDumperNode[0] : from x in node.ArgumentsOpt select Visit(x, null)),
            new TreeDumperNode("inferredType", node.InferredType, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node, object? arg) => new TreeDumperNode("multipleLocalDeclarations", null, new TreeDumperNode[]
        {
            new TreeDumperNode("localDeclarations", null, from x in node.LocalDeclarations select Visit(x, null)),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node, object? arg) => new TreeDumperNode("usingLocalDeclarations", null, new TreeDumperNode[]
        {
            new TreeDumperNode("patternDisposeInfoOpt", node.PatternDisposeInfoOpt, null),
            new TreeDumperNode("iDisposableConversion", node.IDisposableConversion, null),
            new TreeDumperNode("awaitOpt", null, new TreeDumperNode[] { Visit(node.AwaitOpt, null) }),
            new TreeDumperNode("localDeclarations", null, from x in node.LocalDeclarations select Visit(x, null)),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLocalFunctionStatement(BoundLocalFunctionStatement node, object? arg) => new TreeDumperNode("localFunctionStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("symbol", node.Symbol, null),
            new TreeDumperNode("blockBody", null, new TreeDumperNode[] { Visit(node.BlockBody, null) }),
            new TreeDumperNode("expressionBody", null, new TreeDumperNode[] { Visit(node.ExpressionBody, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitNoOpStatement(BoundNoOpStatement node, object? arg) => new TreeDumperNode("noOpStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("flavor", node.Flavor, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitReturnStatement(BoundReturnStatement node, object? arg) => new TreeDumperNode("returnStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("refKind", node.RefKind, null),
            new TreeDumperNode("expressionOpt", null, new TreeDumperNode[] { Visit(node.ExpressionOpt, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitYieldReturnStatement(BoundYieldReturnStatement node, object? arg) => new TreeDumperNode("yieldReturnStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitYieldBreakStatement(BoundYieldBreakStatement node, object? arg) => new TreeDumperNode("yieldBreakStatement", null, new TreeDumperNode[0]
        );
        public override TreeDumperNode VisitThrowStatement(BoundThrowStatement node, object? arg) => new TreeDumperNode("throwStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expressionOpt", null, new TreeDumperNode[] { Visit(node.ExpressionOpt, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitExpressionStatement(BoundExpressionStatement node, object? arg) => new TreeDumperNode("expressionStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitBreakStatement(BoundBreakStatement node, object? arg) => new TreeDumperNode("breakStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("label", node.Label, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitContinueStatement(BoundContinueStatement node, object? arg) => new TreeDumperNode("continueStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("label", node.Label, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSwitchStatement(BoundSwitchStatement node, object? arg) => new TreeDumperNode("switchStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("innerLocals", node.InnerLocals, null),
            new TreeDumperNode("innerLocalFunctions", node.InnerLocalFunctions, null),
            new TreeDumperNode("switchSections", null, from x in node.SwitchSections select Visit(x, null)),
            new TreeDumperNode("decisionDag", null, new TreeDumperNode[] { Visit(node.DecisionDag, null) }),
            new TreeDumperNode("defaultLabel", null, new TreeDumperNode[] { Visit(node.DefaultLabel, null) }),
            new TreeDumperNode("breakLabel", node.BreakLabel, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSwitchDispatch(BoundSwitchDispatch node, object? arg) => new TreeDumperNode("switchDispatch", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("cases", node.Cases, null),
            new TreeDumperNode("defaultLabel", node.DefaultLabel, null),
            new TreeDumperNode("equalityMethod", node.EqualityMethod, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitIfStatement(BoundIfStatement node, object? arg) => new TreeDumperNode("ifStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("condition", null, new TreeDumperNode[] { Visit(node.Condition, null) }),
            new TreeDumperNode("consequence", null, new TreeDumperNode[] { Visit(node.Consequence, null) }),
            new TreeDumperNode("alternativeOpt", null, new TreeDumperNode[] { Visit(node.AlternativeOpt, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDoStatement(BoundDoStatement node, object? arg) => new TreeDumperNode("doStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("condition", null, new TreeDumperNode[] { Visit(node.Condition, null) }),
            new TreeDumperNode("body", null, new TreeDumperNode[] { Visit(node.Body, null) }),
            new TreeDumperNode("breakLabel", node.BreakLabel, null),
            new TreeDumperNode("continueLabel", node.ContinueLabel, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitWhileStatement(BoundWhileStatement node, object? arg) => new TreeDumperNode("whileStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("condition", null, new TreeDumperNode[] { Visit(node.Condition, null) }),
            new TreeDumperNode("body", null, new TreeDumperNode[] { Visit(node.Body, null) }),
            new TreeDumperNode("breakLabel", node.BreakLabel, null),
            new TreeDumperNode("continueLabel", node.ContinueLabel, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitForStatement(BoundForStatement node, object? arg) => new TreeDumperNode("forStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("outerLocals", node.OuterLocals, null),
            new TreeDumperNode("initializer", null, new TreeDumperNode[] { Visit(node.Initializer, null) }),
            new TreeDumperNode("innerLocals", node.InnerLocals, null),
            new TreeDumperNode("condition", null, new TreeDumperNode[] { Visit(node.Condition, null) }),
            new TreeDumperNode("increment", null, new TreeDumperNode[] { Visit(node.Increment, null) }),
            new TreeDumperNode("body", null, new TreeDumperNode[] { Visit(node.Body, null) }),
            new TreeDumperNode("breakLabel", node.BreakLabel, null),
            new TreeDumperNode("continueLabel", node.ContinueLabel, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitForEachStatement(BoundForEachStatement node, object? arg) => new TreeDumperNode("forEachStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("enumeratorInfoOpt", node.EnumeratorInfoOpt, null),
            new TreeDumperNode("elementConversion", node.ElementConversion, null),
            new TreeDumperNode("iterationVariableType", null, new TreeDumperNode[] { Visit(node.IterationVariableType, null) }),
            new TreeDumperNode("iterationVariables", node.IterationVariables, null),
            new TreeDumperNode("iterationErrorExpressionOpt", null, new TreeDumperNode[] { Visit(node.IterationErrorExpressionOpt, null) }),
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("deconstructionOpt", null, new TreeDumperNode[] { Visit(node.DeconstructionOpt, null) }),
            new TreeDumperNode("awaitOpt", null, new TreeDumperNode[] { Visit(node.AwaitOpt, null) }),
            new TreeDumperNode("body", null, new TreeDumperNode[] { Visit(node.Body, null) }),
            new TreeDumperNode("@checked", node.Checked, null),
            new TreeDumperNode("breakLabel", node.BreakLabel, null),
            new TreeDumperNode("continueLabel", node.ContinueLabel, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitForEachDeconstructStep(BoundForEachDeconstructStep node, object? arg) => new TreeDumperNode("forEachDeconstructStep", null, new TreeDumperNode[]
        {
            new TreeDumperNode("deconstructionAssignment", null, new TreeDumperNode[] { Visit(node.DeconstructionAssignment, null) }),
            new TreeDumperNode("targetPlaceholder", null, new TreeDumperNode[] { Visit(node.TargetPlaceholder, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitUsingStatement(BoundUsingStatement node, object? arg) => new TreeDumperNode("usingStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("declarationsOpt", null, new TreeDumperNode[] { Visit(node.DeclarationsOpt, null) }),
            new TreeDumperNode("expressionOpt", null, new TreeDumperNode[] { Visit(node.ExpressionOpt, null) }),
            new TreeDumperNode("iDisposableConversion", node.IDisposableConversion, null),
            new TreeDumperNode("body", null, new TreeDumperNode[] { Visit(node.Body, null) }),
            new TreeDumperNode("awaitOpt", null, new TreeDumperNode[] { Visit(node.AwaitOpt, null) }),
            new TreeDumperNode("patternDisposeInfoOpt", node.PatternDisposeInfoOpt, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitFixedStatement(BoundFixedStatement node, object? arg) => new TreeDumperNode("fixedStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("declarations", null, new TreeDumperNode[] { Visit(node.Declarations, null) }),
            new TreeDumperNode("body", null, new TreeDumperNode[] { Visit(node.Body, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLockStatement(BoundLockStatement node, object? arg) => new TreeDumperNode("lockStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("argument", null, new TreeDumperNode[] { Visit(node.Argument, null) }),
            new TreeDumperNode("body", null, new TreeDumperNode[] { Visit(node.Body, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitTryStatement(BoundTryStatement node, object? arg) => new TreeDumperNode("tryStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("tryBlock", null, new TreeDumperNode[] { Visit(node.TryBlock, null) }),
            new TreeDumperNode("catchBlocks", null, from x in node.CatchBlocks select Visit(x, null)),
            new TreeDumperNode("finallyBlockOpt", null, new TreeDumperNode[] { Visit(node.FinallyBlockOpt, null) }),
            new TreeDumperNode("finallyLabelOpt", node.FinallyLabelOpt, null),
            new TreeDumperNode("preferFaultHandler", node.PreferFaultHandler, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitCatchBlock(BoundCatchBlock node, object? arg) => new TreeDumperNode("catchBlock", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("exceptionSourceOpt", null, new TreeDumperNode[] { Visit(node.ExceptionSourceOpt, null) }),
            new TreeDumperNode("exceptionTypeOpt", node.ExceptionTypeOpt, null),
            new TreeDumperNode("exceptionFilterPrologueOpt", null, new TreeDumperNode[] { Visit(node.ExceptionFilterPrologueOpt, null) }),
            new TreeDumperNode("exceptionFilterOpt", null, new TreeDumperNode[] { Visit(node.ExceptionFilterOpt, null) }),
            new TreeDumperNode("body", null, new TreeDumperNode[] { Visit(node.Body, null) }),
            new TreeDumperNode("isSynthesizedAsyncCatchAll", node.IsSynthesizedAsyncCatchAll, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLiteral(BoundLiteral node, object? arg) => new TreeDumperNode("literal", null, new TreeDumperNode[]
        {
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitThisReference(BoundThisReference node, object? arg) => new TreeDumperNode("thisReference", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node, object? arg) => new TreeDumperNode("previousSubmissionReference", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitHostObjectMemberReference(BoundHostObjectMemberReference node, object? arg) => new TreeDumperNode("hostObjectMemberReference", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitBaseReference(BoundBaseReference node, object? arg) => new TreeDumperNode("baseReference", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLocal(BoundLocal node, object? arg) => new TreeDumperNode("local", null, new TreeDumperNode[]
        {
            new TreeDumperNode("localSymbol", node.LocalSymbol, null),
            new TreeDumperNode("declarationKind", node.DeclarationKind, null),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("isNullableUnknown", node.IsNullableUnknown, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitPseudoVariable(BoundPseudoVariable node, object? arg) => new TreeDumperNode("pseudoVariable", null, new TreeDumperNode[]
        {
            new TreeDumperNode("localSymbol", node.LocalSymbol, null),
            new TreeDumperNode("emitExpressions", node.EmitExpressions, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitRangeVariable(BoundRangeVariable node, object? arg) => new TreeDumperNode("rangeVariable", null, new TreeDumperNode[]
        {
            new TreeDumperNode("rangeVariableSymbol", node.RangeVariableSymbol, null),
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitParameter(BoundParameter node, object? arg) => new TreeDumperNode("parameter", null, new TreeDumperNode[]
        {
            new TreeDumperNode("parameterSymbol", node.ParameterSymbol, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLabelStatement(BoundLabelStatement node, object? arg) => new TreeDumperNode("labelStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("label", node.Label, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitGotoStatement(BoundGotoStatement node, object? arg) => new TreeDumperNode("gotoStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("label", node.Label, null),
            new TreeDumperNode("caseExpressionOpt", null, new TreeDumperNode[] { Visit(node.CaseExpressionOpt, null) }),
            new TreeDumperNode("labelExpressionOpt", null, new TreeDumperNode[] { Visit(node.LabelExpressionOpt, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLabeledStatement(BoundLabeledStatement node, object? arg) => new TreeDumperNode("labeledStatement", null, new TreeDumperNode[]
        {
            new TreeDumperNode("label", node.Label, null),
            new TreeDumperNode("body", null, new TreeDumperNode[] { Visit(node.Body, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLabel(BoundLabel node, object? arg) => new TreeDumperNode("label", null, new TreeDumperNode[]
        {
            new TreeDumperNode("label", node.Label, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitStatementList(BoundStatementList node, object? arg) => new TreeDumperNode("statementList", null, new TreeDumperNode[]
        {
            new TreeDumperNode("statements", null, from x in node.Statements select Visit(x, null)),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitConditionalGoto(BoundConditionalGoto node, object? arg) => new TreeDumperNode("conditionalGoto", null, new TreeDumperNode[]
        {
            new TreeDumperNode("condition", null, new TreeDumperNode[] { Visit(node.Condition, null) }),
            new TreeDumperNode("jumpIfTrue", node.JumpIfTrue, null),
            new TreeDumperNode("label", node.Label, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSwitchExpressionArm(BoundSwitchExpressionArm node, object? arg) => new TreeDumperNode("switchExpressionArm", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("pattern", null, new TreeDumperNode[] { Visit(node.Pattern, null) }),
            new TreeDumperNode("whenClause", null, new TreeDumperNode[] { Visit(node.WhenClause, null) }),
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("label", node.Label, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node, object? arg) => new TreeDumperNode("unconvertedSwitchExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("switchArms", null, from x in node.SwitchArms select Visit(x, null)),
            new TreeDumperNode("decisionDag", null, new TreeDumperNode[] { Visit(node.DecisionDag, null) }),
            new TreeDumperNode("defaultLabel", node.DefaultLabel, null),
            new TreeDumperNode("reportedNotExhaustive", node.ReportedNotExhaustive, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node, object? arg) => new TreeDumperNode("convertedSwitchExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("naturalTypeOpt", node.NaturalTypeOpt, null),
            new TreeDumperNode("wasTargetTyped", node.WasTargetTyped, null),
            new TreeDumperNode("conversion", node.Conversion, null),
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("switchArms", null, from x in node.SwitchArms select Visit(x, null)),
            new TreeDumperNode("decisionDag", null, new TreeDumperNode[] { Visit(node.DecisionDag, null) }),
            new TreeDumperNode("defaultLabel", node.DefaultLabel, null),
            new TreeDumperNode("reportedNotExhaustive", node.ReportedNotExhaustive, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDecisionDag(BoundDecisionDag node, object? arg) => new TreeDumperNode("decisionDag", null, new TreeDumperNode[]
        {
            new TreeDumperNode("rootNode", null, new TreeDumperNode[] { Visit(node.RootNode, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitEvaluationDecisionDagNode(BoundEvaluationDecisionDagNode node, object? arg) => new TreeDumperNode("evaluationDecisionDagNode", null, new TreeDumperNode[]
        {
            new TreeDumperNode("evaluation", null, new TreeDumperNode[] { Visit(node.Evaluation, null) }),
            new TreeDumperNode("next", null, new TreeDumperNode[] { Visit(node.Next, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitTestDecisionDagNode(BoundTestDecisionDagNode node, object? arg) => new TreeDumperNode("testDecisionDagNode", null, new TreeDumperNode[]
        {
            new TreeDumperNode("test", null, new TreeDumperNode[] { Visit(node.Test, null) }),
            new TreeDumperNode("whenTrue", null, new TreeDumperNode[] { Visit(node.WhenTrue, null) }),
            new TreeDumperNode("whenFalse", null, new TreeDumperNode[] { Visit(node.WhenFalse, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitWhenDecisionDagNode(BoundWhenDecisionDagNode node, object? arg) => new TreeDumperNode("whenDecisionDagNode", null, new TreeDumperNode[]
        {
            new TreeDumperNode("bindings", node.Bindings, null),
            new TreeDumperNode("whenExpression", null, new TreeDumperNode[] { Visit(node.WhenExpression, null) }),
            new TreeDumperNode("whenTrue", null, new TreeDumperNode[] { Visit(node.WhenTrue, null) }),
            new TreeDumperNode("whenFalse", null, new TreeDumperNode[] { Visit(node.WhenFalse, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLeafDecisionDagNode(BoundLeafDecisionDagNode node, object? arg) => new TreeDumperNode("leafDecisionDagNode", null, new TreeDumperNode[]
        {
            new TreeDumperNode("label", node.Label, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagTemp(BoundDagTemp node, object? arg) => new TreeDumperNode("dagTemp", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("source", null, new TreeDumperNode[] { Visit(node.Source, null) }),
            new TreeDumperNode("index", node.Index, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagTypeTest(BoundDagTypeTest node, object? arg) => new TreeDumperNode("dagTypeTest", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("input", null, new TreeDumperNode[] { Visit(node.Input, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagNonNullTest(BoundDagNonNullTest node, object? arg) => new TreeDumperNode("dagNonNullTest", null, new TreeDumperNode[]
        {
            new TreeDumperNode("isExplicitTest", node.IsExplicitTest, null),
            new TreeDumperNode("input", null, new TreeDumperNode[] { Visit(node.Input, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagExplicitNullTest(BoundDagExplicitNullTest node, object? arg) => new TreeDumperNode("dagExplicitNullTest", null, new TreeDumperNode[]
        {
            new TreeDumperNode("input", null, new TreeDumperNode[] { Visit(node.Input, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagValueTest(BoundDagValueTest node, object? arg) => new TreeDumperNode("dagValueTest", null, new TreeDumperNode[]
        {
            new TreeDumperNode("value", node.Value, null),
            new TreeDumperNode("input", null, new TreeDumperNode[] { Visit(node.Input, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagRelationalTest(BoundDagRelationalTest node, object? arg) => new TreeDumperNode("dagRelationalTest", null, new TreeDumperNode[]
        {
            new TreeDumperNode("operatorKind", node.OperatorKind, null),
            new TreeDumperNode("value", node.Value, null),
            new TreeDumperNode("input", null, new TreeDumperNode[] { Visit(node.Input, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagDeconstructEvaluation(BoundDagDeconstructEvaluation node, object? arg) => new TreeDumperNode("dagDeconstructEvaluation", null, new TreeDumperNode[]
        {
            new TreeDumperNode("deconstructMethod", node.DeconstructMethod, null),
            new TreeDumperNode("input", null, new TreeDumperNode[] { Visit(node.Input, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagTypeEvaluation(BoundDagTypeEvaluation node, object? arg) => new TreeDumperNode("dagTypeEvaluation", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("input", null, new TreeDumperNode[] { Visit(node.Input, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagFieldEvaluation(BoundDagFieldEvaluation node, object? arg) => new TreeDumperNode("dagFieldEvaluation", null, new TreeDumperNode[]
        {
            new TreeDumperNode("field", node.Field, null),
            new TreeDumperNode("input", null, new TreeDumperNode[] { Visit(node.Input, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagPropertyEvaluation(BoundDagPropertyEvaluation node, object? arg) => new TreeDumperNode("dagPropertyEvaluation", null, new TreeDumperNode[]
        {
            new TreeDumperNode("property", node.Property, null),
            new TreeDumperNode("input", null, new TreeDumperNode[] { Visit(node.Input, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDagIndexEvaluation(BoundDagIndexEvaluation node, object? arg) => new TreeDumperNode("dagIndexEvaluation", null, new TreeDumperNode[]
        {
            new TreeDumperNode("property", node.Property, null),
            new TreeDumperNode("index", node.Index, null),
            new TreeDumperNode("input", null, new TreeDumperNode[] { Visit(node.Input, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSwitchSection(BoundSwitchSection node, object? arg) => new TreeDumperNode("switchSection", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("switchLabels", null, from x in node.SwitchLabels select Visit(x, null)),
            new TreeDumperNode("statements", null, from x in node.Statements select Visit(x, null)),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSwitchLabel(BoundSwitchLabel node, object? arg) => new TreeDumperNode("switchLabel", null, new TreeDumperNode[]
        {
            new TreeDumperNode("label", node.Label, null),
            new TreeDumperNode("pattern", null, new TreeDumperNode[] { Visit(node.Pattern, null) }),
            new TreeDumperNode("whenClause", null, new TreeDumperNode[] { Visit(node.WhenClause, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSequencePointExpression(BoundSequencePointExpression node, object? arg) => new TreeDumperNode("sequencePointExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSequence(BoundSequence node, object? arg) => new TreeDumperNode("sequence", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("sideEffects", null, from x in node.SideEffects select Visit(x, null)),
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSpillSequence(BoundSpillSequence node, object? arg) => new TreeDumperNode("spillSequence", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("sideEffects", null, from x in node.SideEffects select Visit(x, null)),
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDynamicMemberAccess(BoundDynamicMemberAccess node, object? arg) => new TreeDumperNode("dynamicMemberAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiver", null, new TreeDumperNode[] { Visit(node.Receiver, null) }),
            new TreeDumperNode("typeArgumentsOpt", node.TypeArgumentsOpt, null),
            new TreeDumperNode("name", node.Name, null),
            new TreeDumperNode("invoked", node.Invoked, null),
            new TreeDumperNode("indexed", node.Indexed, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDynamicInvocation(BoundDynamicInvocation node, object? arg) => new TreeDumperNode("dynamicInvocation", null, new TreeDumperNode[]
        {
            new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
            new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
            new TreeDumperNode("applicableMethods", node.ApplicableMethods, null),
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitConditionalAccess(BoundConditionalAccess node, object? arg) => new TreeDumperNode("conditionalAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiver", null, new TreeDumperNode[] { Visit(node.Receiver, null) }),
            new TreeDumperNode("accessExpression", null, new TreeDumperNode[] { Visit(node.AccessExpression, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node, object? arg) => new TreeDumperNode("loweredConditionalAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiver", null, new TreeDumperNode[] { Visit(node.Receiver, null) }),
            new TreeDumperNode("hasValueMethodOpt", node.HasValueMethodOpt, null),
            new TreeDumperNode("whenNotNull", null, new TreeDumperNode[] { Visit(node.WhenNotNull, null) }),
            new TreeDumperNode("whenNullOpt", null, new TreeDumperNode[] { Visit(node.WhenNullOpt, null) }),
            new TreeDumperNode("id", node.Id, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitConditionalReceiver(BoundConditionalReceiver node, object? arg) => new TreeDumperNode("conditionalReceiver", null, new TreeDumperNode[]
        {
            new TreeDumperNode("id", node.Id, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node, object? arg) => new TreeDumperNode("complexConditionalReceiver", null, new TreeDumperNode[]
        {
            new TreeDumperNode("valueTypeReceiver", null, new TreeDumperNode[] { Visit(node.ValueTypeReceiver, null) }),
            new TreeDumperNode("referenceTypeReceiver", null, new TreeDumperNode[] { Visit(node.ReferenceTypeReceiver, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitMethodGroup(BoundMethodGroup node, object? arg) => new TreeDumperNode("methodGroup", null, new TreeDumperNode[]
        {
            new TreeDumperNode("typeArgumentsOpt", node.TypeArgumentsOpt, null),
            new TreeDumperNode("name", node.Name, null),
            new TreeDumperNode("methods", node.Methods, null),
            new TreeDumperNode("lookupSymbolOpt", node.LookupSymbolOpt, null),
            new TreeDumperNode("lookupError", node.LookupError, null),
            new TreeDumperNode("flags", node.Flags, null),
            new TreeDumperNode("receiverOpt", null, new TreeDumperNode[] { Visit(node.ReceiverOpt, null) }),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitPropertyGroup(BoundPropertyGroup node, object? arg) => new TreeDumperNode("propertyGroup", null, new TreeDumperNode[]
        {
            new TreeDumperNode("properties", node.Properties, null),
            new TreeDumperNode("receiverOpt", null, new TreeDumperNode[] { Visit(node.ReceiverOpt, null) }),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitCall(BoundCall node, object? arg) => new TreeDumperNode("call", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiverOpt", null, new TreeDumperNode[] { Visit(node.ReceiverOpt, null) }),
            new TreeDumperNode("method", node.Method, null),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
            new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
            new TreeDumperNode("isDelegateCall", node.IsDelegateCall, null),
            new TreeDumperNode("expanded", node.Expanded, null),
            new TreeDumperNode("invokedAsExtensionMethod", node.InvokedAsExtensionMethod, null),
            new TreeDumperNode("argsToParamsOpt", node.ArgsToParamsOpt, null),
            new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("originalMethodsOpt", node.OriginalMethodsOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitEventAssignmentOperator(BoundEventAssignmentOperator node, object? arg) => new TreeDumperNode("eventAssignmentOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("@event", node.Event, null),
            new TreeDumperNode("isAddition", node.IsAddition, null),
            new TreeDumperNode("isDynamic", node.IsDynamic, null),
            new TreeDumperNode("receiverOpt", null, new TreeDumperNode[] { Visit(node.ReceiverOpt, null) }),
            new TreeDumperNode("argument", null, new TreeDumperNode[] { Visit(node.Argument, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitAttribute(BoundAttribute node, object? arg) => new TreeDumperNode("attribute", null, new TreeDumperNode[]
        {
            new TreeDumperNode("constructor", node.Constructor, null),
            new TreeDumperNode("constructorArguments", null, from x in node.ConstructorArguments select Visit(x, null)),
            new TreeDumperNode("constructorArgumentNamesOpt", node.ConstructorArgumentNamesOpt, null),
            new TreeDumperNode("constructorArgumentsToParamsOpt", node.ConstructorArgumentsToParamsOpt, null),
            new TreeDumperNode("constructorExpanded", node.ConstructorExpanded, null),
            new TreeDumperNode("namedArguments", null, from x in node.NamedArguments select Visit(x, null)),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node, object? arg) => new TreeDumperNode("unconvertedObjectCreationExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
            new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
            new TreeDumperNode("initializerOpt", node.InitializerOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitObjectCreationExpression(BoundObjectCreationExpression node, object? arg) => new TreeDumperNode("objectCreationExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("constructor", node.Constructor, null),
            new TreeDumperNode("constructorsGroup", node.ConstructorsGroup, null),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
            new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
            new TreeDumperNode("expanded", node.Expanded, null),
            new TreeDumperNode("argsToParamsOpt", node.ArgsToParamsOpt, null),
            new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("initializerExpressionOpt", null, new TreeDumperNode[] { Visit(node.InitializerExpressionOpt, null) }),
            new TreeDumperNode("wasTargetTyped", node.WasTargetTyped, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitTupleLiteral(BoundTupleLiteral node, object? arg) => new TreeDumperNode("tupleLiteral", null, new TreeDumperNode[]
        {
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
            new TreeDumperNode("inferredNamesOpt", node.InferredNamesOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node, object? arg) => new TreeDumperNode("convertedTupleLiteral", null, new TreeDumperNode[]
        {
            new TreeDumperNode("sourceTuple", null, new TreeDumperNode[] { Visit(node.SourceTuple, null) }),
            new TreeDumperNode("wasTargetTyped", node.WasTargetTyped, null),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
            new TreeDumperNode("inferredNamesOpt", node.InferredNamesOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node, object? arg) => new TreeDumperNode("dynamicObjectCreationExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("name", node.Name, null),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
            new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
            new TreeDumperNode("initializerExpressionOpt", null, new TreeDumperNode[] { Visit(node.InitializerExpressionOpt, null) }),
            new TreeDumperNode("applicableMethods", node.ApplicableMethods, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node, object? arg) => new TreeDumperNode("noPiaObjectCreationExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("guidString", node.GuidString, null),
            new TreeDumperNode("initializerExpressionOpt", null, new TreeDumperNode[] { Visit(node.InitializerExpressionOpt, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node, object? arg) => new TreeDumperNode("objectInitializerExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("placeholder", null, new TreeDumperNode[] { Visit(node.Placeholder, null) }),
            new TreeDumperNode("initializers", null, from x in node.Initializers select Visit(x, null)),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitObjectInitializerMember(BoundObjectInitializerMember node, object? arg) => new TreeDumperNode("objectInitializerMember", null, new TreeDumperNode[]
        {
            new TreeDumperNode("memberSymbol", node.MemberSymbol, null),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
            new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
            new TreeDumperNode("expanded", node.Expanded, null),
            new TreeDumperNode("argsToParamsOpt", node.ArgsToParamsOpt, null),
            new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("receiverType", node.ReceiverType, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node, object? arg) => new TreeDumperNode("dynamicObjectInitializerMember", null, new TreeDumperNode[]
        {
            new TreeDumperNode("memberName", node.MemberName, null),
            new TreeDumperNode("receiverType", node.ReceiverType, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node, object? arg) => new TreeDumperNode("collectionInitializerExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("placeholder", null, new TreeDumperNode[] { Visit(node.Placeholder, null) }),
            new TreeDumperNode("initializers", null, from x in node.Initializers select Visit(x, null)),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitCollectionElementInitializer(BoundCollectionElementInitializer node, object? arg) => new TreeDumperNode("collectionElementInitializer", null, new TreeDumperNode[]
        {
            new TreeDumperNode("addMethod", node.AddMethod, null),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("implicitReceiverOpt", null, new TreeDumperNode[] { Visit(node.ImplicitReceiverOpt, null) }),
            new TreeDumperNode("expanded", node.Expanded, null),
            new TreeDumperNode("argsToParamsOpt", node.ArgsToParamsOpt, null),
            new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
            new TreeDumperNode("invokedAsExtensionMethod", node.InvokedAsExtensionMethod, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node, object? arg) => new TreeDumperNode("dynamicCollectionElementInitializer", null, new TreeDumperNode[]
        {
            new TreeDumperNode("applicableMethods", node.ApplicableMethods, null),
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitImplicitReceiver(BoundImplicitReceiver node, object? arg) => new TreeDumperNode("implicitReceiver", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node, object? arg) => new TreeDumperNode("anonymousObjectCreationExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("constructor", node.Constructor, null),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("declarations", null, from x in node.Declarations select Visit(x, null)),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node, object? arg) => new TreeDumperNode("anonymousPropertyDeclaration", null, new TreeDumperNode[]
        {
            new TreeDumperNode("property", node.Property, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitNewT(BoundNewT node, object? arg) => new TreeDumperNode("newT", null, new TreeDumperNode[]
        {
            new TreeDumperNode("initializerExpressionOpt", null, new TreeDumperNode[] { Visit(node.InitializerExpressionOpt, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node, object? arg) => new TreeDumperNode("delegateCreationExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("argument", null, new TreeDumperNode[] { Visit(node.Argument, null) }),
            new TreeDumperNode("methodOpt", node.MethodOpt, null),
            new TreeDumperNode("isExtensionMethod", node.IsExtensionMethod, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitArrayCreation(BoundArrayCreation node, object? arg) => new TreeDumperNode("arrayCreation", null, new TreeDumperNode[]
        {
            new TreeDumperNode("bounds", null, from x in node.Bounds select Visit(x, null)),
            new TreeDumperNode("initializerOpt", null, new TreeDumperNode[] { Visit(node.InitializerOpt, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitArrayInitialization(BoundArrayInitialization node, object? arg) => new TreeDumperNode("arrayInitialization", null, new TreeDumperNode[]
        {
            new TreeDumperNode("initializers", null, from x in node.Initializers select Visit(x, null)),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node, object? arg) => new TreeDumperNode("stackAllocArrayCreation", null, new TreeDumperNode[]
        {
            new TreeDumperNode("elementType", node.ElementType, null),
            new TreeDumperNode("count", null, new TreeDumperNode[] { Visit(node.Count, null) }),
            new TreeDumperNode("initializerOpt", null, new TreeDumperNode[] { Visit(node.InitializerOpt, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node, object? arg) => new TreeDumperNode("convertedStackAllocExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("elementType", node.ElementType, null),
            new TreeDumperNode("count", null, new TreeDumperNode[] { Visit(node.Count, null) }),
            new TreeDumperNode("initializerOpt", null, new TreeDumperNode[] { Visit(node.InitializerOpt, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitFieldAccess(BoundFieldAccess node, object? arg) => new TreeDumperNode("fieldAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiverOpt", null, new TreeDumperNode[] { Visit(node.ReceiverOpt, null) }),
            new TreeDumperNode("fieldSymbol", node.FieldSymbol, null),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("isByValue", node.IsByValue, null),
            new TreeDumperNode("isDeclaration", node.IsDeclaration, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitHoistedFieldAccess(BoundHoistedFieldAccess node, object? arg) => new TreeDumperNode("hoistedFieldAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("fieldSymbol", node.FieldSymbol, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitPropertyAccess(BoundPropertyAccess node, object? arg) => new TreeDumperNode("propertyAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiverOpt", null, new TreeDumperNode[] { Visit(node.ReceiverOpt, null) }),
            new TreeDumperNode("propertySymbol", node.PropertySymbol, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitEventAccess(BoundEventAccess node, object? arg) => new TreeDumperNode("eventAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiverOpt", null, new TreeDumperNode[] { Visit(node.ReceiverOpt, null) }),
            new TreeDumperNode("eventSymbol", node.EventSymbol, null),
            new TreeDumperNode("isUsableAsField", node.IsUsableAsField, null),
            new TreeDumperNode("resultKind", node.ResultKind, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitIndexerAccess(BoundIndexerAccess node, object? arg) => new TreeDumperNode("indexerAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiverOpt", null, new TreeDumperNode[] { Visit(node.ReceiverOpt, null) }),
            new TreeDumperNode("indexer", node.Indexer, null),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
            new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
            new TreeDumperNode("expanded", node.Expanded, null),
            new TreeDumperNode("argsToParamsOpt", node.ArgsToParamsOpt, null),
            new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
            new TreeDumperNode("originalIndexersOpt", node.OriginalIndexersOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node, object? arg) => new TreeDumperNode("indexOrRangePatternIndexerAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiver", null, new TreeDumperNode[] { Visit(node.Receiver, null) }),
            new TreeDumperNode("lengthOrCountProperty", node.LengthOrCountProperty, null),
            new TreeDumperNode("patternSymbol", node.PatternSymbol, null),
            new TreeDumperNode("argument", null, new TreeDumperNode[] { Visit(node.Argument, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node, object? arg) => new TreeDumperNode("dynamicIndexerAccess", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiver", null, new TreeDumperNode[] { Visit(node.Receiver, null) }),
            new TreeDumperNode("arguments", null, from x in node.Arguments select Visit(x, null)),
            new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
            new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
            new TreeDumperNode("applicableIndexers", node.ApplicableIndexers, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitLambda(BoundLambda node, object? arg) => new TreeDumperNode("lambda", null, new TreeDumperNode[]
        {
            new TreeDumperNode("unboundLambda", null, new TreeDumperNode[] { Visit(node.UnboundLambda, null) }),
            new TreeDumperNode("symbol", node.Symbol, null),
            new TreeDumperNode("body", null, new TreeDumperNode[] { Visit(node.Body, null) }),
            new TreeDumperNode("diagnostics", node.Diagnostics, null),
            new TreeDumperNode("binder", node.Binder, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitUnboundLambda(UnboundLambda node, object? arg) => new TreeDumperNode("unboundLambda", null, new TreeDumperNode[]
        {
            new TreeDumperNode("data", node.Data, null),
            new TreeDumperNode("withDependencies", node.WithDependencies, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitQueryClause(BoundQueryClause node, object? arg) => new TreeDumperNode("queryClause", null, new TreeDumperNode[]
        {
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("definedSymbol", node.DefinedSymbol, null),
            new TreeDumperNode("operation", null, new TreeDumperNode[] { Visit(node.Operation, null) }),
            new TreeDumperNode("cast", null, new TreeDumperNode[] { Visit(node.Cast, null) }),
            new TreeDumperNode("binder", node.Binder, null),
            new TreeDumperNode("unoptimizedForm", null, new TreeDumperNode[] { Visit(node.UnoptimizedForm, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node, object? arg) => new TreeDumperNode("typeOrInstanceInitializers", null, new TreeDumperNode[]
        {
            new TreeDumperNode("statements", null, from x in node.Statements select Visit(x, null)),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitNameOfOperator(BoundNameOfOperator node, object? arg) => new TreeDumperNode("nameOfOperator", null, new TreeDumperNode[]
        {
            new TreeDumperNode("argument", null, new TreeDumperNode[] { Visit(node.Argument, null) }),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node, object? arg) => new TreeDumperNode("unconvertedInterpolatedString", null, new TreeDumperNode[]
        {
            new TreeDumperNode("parts", null, from x in node.Parts select Visit(x, null)),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitInterpolatedString(BoundInterpolatedString node, object? arg) => new TreeDumperNode("interpolatedString", null, new TreeDumperNode[]
        {
            new TreeDumperNode("parts", null, from x in node.Parts select Visit(x, null)),
            new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitStringInsert(BoundStringInsert node, object? arg) => new TreeDumperNode("stringInsert", null, new TreeDumperNode[]
        {
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("alignment", null, new TreeDumperNode[] { Visit(node.Alignment, null) }),
            new TreeDumperNode("format", null, new TreeDumperNode[] { Visit(node.Format, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitIsPatternExpression(BoundIsPatternExpression node, object? arg) => new TreeDumperNode("isPatternExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("pattern", null, new TreeDumperNode[] { Visit(node.Pattern, null) }),
            new TreeDumperNode("isNegated", node.IsNegated, null),
            new TreeDumperNode("decisionDag", null, new TreeDumperNode[] { Visit(node.DecisionDag, null) }),
            new TreeDumperNode("whenTrueLabel", node.WhenTrueLabel, null),
            new TreeDumperNode("whenFalseLabel", node.WhenFalseLabel, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitConstantPattern(BoundConstantPattern node, object? arg) => new TreeDumperNode("constantPattern", null, new TreeDumperNode[]
        {
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("constantValue", node.ConstantValue, null),
            new TreeDumperNode("inputType", node.InputType, null),
            new TreeDumperNode("narrowedType", node.NarrowedType, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDiscardPattern(BoundDiscardPattern node, object? arg) => new TreeDumperNode("discardPattern", null, new TreeDumperNode[]
        {
            new TreeDumperNode("inputType", node.InputType, null),
            new TreeDumperNode("narrowedType", node.NarrowedType, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDeclarationPattern(BoundDeclarationPattern node, object? arg) => new TreeDumperNode("declarationPattern", null, new TreeDumperNode[]
        {
            new TreeDumperNode("variable", node.Variable, null),
            new TreeDumperNode("variableAccess", null, new TreeDumperNode[] { Visit(node.VariableAccess, null) }),
            new TreeDumperNode("declaredType", null, new TreeDumperNode[] { Visit(node.DeclaredType, null) }),
            new TreeDumperNode("isVar", node.IsVar, null),
            new TreeDumperNode("inputType", node.InputType, null),
            new TreeDumperNode("narrowedType", node.NarrowedType, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitRecursivePattern(BoundRecursivePattern node, object? arg) => new TreeDumperNode("recursivePattern", null, new TreeDumperNode[]
        {
            new TreeDumperNode("declaredType", null, new TreeDumperNode[] { Visit(node.DeclaredType, null) }),
            new TreeDumperNode("deconstructMethod", node.DeconstructMethod, null),
            new TreeDumperNode("deconstruction", null, node.Deconstruction.IsDefault ? new TreeDumperNode[0] : from x in node.Deconstruction select Visit(x, null)),
            new TreeDumperNode("properties", null, node.Properties.IsDefault ? new TreeDumperNode[0] : from x in node.Properties select Visit(x, null)),
            new TreeDumperNode("variable", node.Variable, null),
            new TreeDumperNode("variableAccess", null, new TreeDumperNode[] { Visit(node.VariableAccess, null) }),
            new TreeDumperNode("isExplicitNotNullTest", node.IsExplicitNotNullTest, null),
            new TreeDumperNode("inputType", node.InputType, null),
            new TreeDumperNode("narrowedType", node.NarrowedType, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitITuplePattern(BoundITuplePattern node, object? arg) => new TreeDumperNode("iTuplePattern", null, new TreeDumperNode[]
        {
            new TreeDumperNode("getLengthMethod", node.GetLengthMethod, null),
            new TreeDumperNode("getItemMethod", node.GetItemMethod, null),
            new TreeDumperNode("subpatterns", null, from x in node.Subpatterns select Visit(x, null)),
            new TreeDumperNode("inputType", node.InputType, null),
            new TreeDumperNode("narrowedType", node.NarrowedType, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitSubpattern(BoundSubpattern node, object? arg) => new TreeDumperNode("subpattern", null, new TreeDumperNode[]
        {
            new TreeDumperNode("symbol", node.Symbol, null),
            new TreeDumperNode("pattern", null, new TreeDumperNode[] { Visit(node.Pattern, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitTypePattern(BoundTypePattern node, object? arg) => new TreeDumperNode("typePattern", null, new TreeDumperNode[]
        {
            new TreeDumperNode("declaredType", null, new TreeDumperNode[] { Visit(node.DeclaredType, null) }),
            new TreeDumperNode("isExplicitNotNullTest", node.IsExplicitNotNullTest, null),
            new TreeDumperNode("inputType", node.InputType, null),
            new TreeDumperNode("narrowedType", node.NarrowedType, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitBinaryPattern(BoundBinaryPattern node, object? arg) => new TreeDumperNode("binaryPattern", null, new TreeDumperNode[]
        {
            new TreeDumperNode("disjunction", node.Disjunction, null),
            new TreeDumperNode("left", null, new TreeDumperNode[] { Visit(node.Left, null) }),
            new TreeDumperNode("right", null, new TreeDumperNode[] { Visit(node.Right, null) }),
            new TreeDumperNode("inputType", node.InputType, null),
            new TreeDumperNode("narrowedType", node.NarrowedType, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitNegatedPattern(BoundNegatedPattern node, object? arg) => new TreeDumperNode("negatedPattern", null, new TreeDumperNode[]
        {
            new TreeDumperNode("negated", null, new TreeDumperNode[] { Visit(node.Negated, null) }),
            new TreeDumperNode("inputType", node.InputType, null),
            new TreeDumperNode("narrowedType", node.NarrowedType, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitRelationalPattern(BoundRelationalPattern node, object? arg) => new TreeDumperNode("relationalPattern", null, new TreeDumperNode[]
        {
            new TreeDumperNode("relation", node.Relation, null),
            new TreeDumperNode("value", null, new TreeDumperNode[] { Visit(node.Value, null) }),
            new TreeDumperNode("constantValue", node.ConstantValue, null),
            new TreeDumperNode("inputType", node.InputType, null),
            new TreeDumperNode("narrowedType", node.NarrowedType, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDiscardExpression(BoundDiscardExpression node, object? arg) => new TreeDumperNode("discardExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitThrowExpression(BoundThrowExpression node, object? arg) => new TreeDumperNode("throwExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitOutVariablePendingInference(OutVariablePendingInference node, object? arg) => new TreeDumperNode("outVariablePendingInference", null, new TreeDumperNode[]
        {
            new TreeDumperNode("variableSymbol", node.VariableSymbol, null),
            new TreeDumperNode("receiverOpt", null, new TreeDumperNode[] { Visit(node.ReceiverOpt, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node, object? arg) => new TreeDumperNode("deconstructionVariablePendingInference", null, new TreeDumperNode[]
        {
            new TreeDumperNode("variableSymbol", node.VariableSymbol, null),
            new TreeDumperNode("receiverOpt", null, new TreeDumperNode[] { Visit(node.ReceiverOpt, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node, object? arg) => new TreeDumperNode("outDeconstructVarPendingInference", null, new TreeDumperNode[]
        {
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node, object? arg) => new TreeDumperNode("nonConstructorMethodBody", null, new TreeDumperNode[]
        {
            new TreeDumperNode("blockBody", null, new TreeDumperNode[] { Visit(node.BlockBody, null) }),
            new TreeDumperNode("expressionBody", null, new TreeDumperNode[] { Visit(node.ExpressionBody, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitConstructorMethodBody(BoundConstructorMethodBody node, object? arg) => new TreeDumperNode("constructorMethodBody", null, new TreeDumperNode[]
        {
            new TreeDumperNode("locals", node.Locals, null),
            new TreeDumperNode("initializer", null, new TreeDumperNode[] { Visit(node.Initializer, null) }),
            new TreeDumperNode("blockBody", null, new TreeDumperNode[] { Visit(node.BlockBody, null) }),
            new TreeDumperNode("expressionBody", null, new TreeDumperNode[] { Visit(node.ExpressionBody, null) }),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitExpressionWithNullability(BoundExpressionWithNullability node, object? arg) => new TreeDumperNode("expressionWithNullability", null, new TreeDumperNode[]
        {
            new TreeDumperNode("expression", null, new TreeDumperNode[] { Visit(node.Expression, null) }),
            new TreeDumperNode("nullableAnnotation", node.NullableAnnotation, null),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
        public override TreeDumperNode VisitWithExpression(BoundWithExpression node, object? arg) => new TreeDumperNode("withExpression", null, new TreeDumperNode[]
        {
            new TreeDumperNode("receiver", null, new TreeDumperNode[] { Visit(node.Receiver, null) }),
            new TreeDumperNode("cloneMethod", node.CloneMethod, null),
            new TreeDumperNode("initializerExpression", null, new TreeDumperNode[] { Visit(node.InitializerExpression, null) }),
            new TreeDumperNode("type", node.Type, null),
            new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
            new TreeDumperNode("hasErrors", node.HasErrors, null)
        }
        );
    }
}

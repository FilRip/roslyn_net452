using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class SyntheticBoundNodeFactory
    {
        public class MissingPredefinedMember : Exception
        {
            public Diagnostic Diagnostic { get; }

            public MissingPredefinedMember(Diagnostic error)
                : base(error.ToString())
            {
                Diagnostic = error;
            }
        }

        private sealed class SyntheticBinderImpl : BuckStopsHereBinder
        {
            private readonly SyntheticBoundNodeFactory _factory;

            internal override Symbol? ContainingMemberOrLambda => _factory.CurrentFunction;

            internal SyntheticBinderImpl(SyntheticBoundNodeFactory factory)
                : base(factory.Compilation)
            {
                _factory = factory;
            }

            internal override bool IsAccessibleHelper(Symbol symbol, TypeSymbol accessThroughType, out bool failedThroughTypeCheck, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved)
            {
                return AccessCheck.IsSymbolAccessible(symbol, _factory.CurrentType, accessThroughType, out failedThroughTypeCheck, ref useSiteInfo, basesBeingResolved);
            }
        }

        internal class SyntheticSwitchSection
        {
            public readonly ImmutableArray<int> Values;

            public readonly ImmutableArray<BoundStatement> Statements;

            public SyntheticSwitchSection(ImmutableArray<int> Values, ImmutableArray<BoundStatement> Statements)
            {
                this.Values = Values;
                this.Statements = Statements;
            }
        }

        private NamedTypeSymbol? _currentType;

        private MethodSymbol? _currentFunction;

        private MethodSymbol? _topLevelMethod;

        private Binder? _binder;

        public CSharpCompilation Compilation => CompilationState.Compilation;

        public SyntaxNode Syntax { get; set; }

        public PEModuleBuilder? ModuleBuilderOpt => CompilationState.ModuleBuilderOpt;

        public BindingDiagnosticBag Diagnostics { get; }

        public TypeCompilationState CompilationState { get; }

        public NamedTypeSymbol? CurrentType
        {
            get
            {
                return _currentType;
            }
            set
            {
                _currentType = value;
            }
        }

        public MethodSymbol? CurrentFunction
        {
            get
            {
                return _currentFunction;
            }
            set
            {
                _currentFunction = value;
                if ((object)value != null && value!.MethodKind != 0 && value!.MethodKind != MethodKind.LocalFunction)
                {
                    _topLevelMethod = value;
                    _currentType = value!.ContainingType;
                }
            }
        }

        public MethodSymbol? TopLevelMethod
        {
            get
            {
                return _topLevelMethod;
            }
            private set
            {
                _topLevelMethod = value;
            }
        }

        internal BoundExpression MakeInvocationExpression(BinderFlags flags, SyntaxNode node, BoundExpression receiver, string methodName, ImmutableArray<BoundExpression> args, BindingDiagnosticBag diagnostics, ImmutableArray<TypeSymbol> typeArgs = default(ImmutableArray<TypeSymbol>), bool allowUnexpandedForm = true)
        {
            if (_binder == null || _binder!.Flags != flags)
            {
                _binder = new SyntheticBinderImpl(this).WithFlags(flags);
            }
            Binder? binder = _binder;
            ImmutableArray<TypeWithAnnotations> typeArgs2 = (typeArgs.IsDefault ? default(ImmutableArray<TypeWithAnnotations>) : typeArgs.SelectAsArray((TypeSymbol t) => TypeWithAnnotations.Create(t)));
            bool allowUnexpandedForm2 = allowUnexpandedForm;
            return binder!.MakeInvocationExpression(node, receiver, methodName, args, diagnostics, default(SeparatedSyntaxList<TypeSyntax>), typeArgs2, null, allowFieldsAndProperties: false, allowUnexpandedForm2);
        }

        public SyntheticBoundNodeFactory(MethodSymbol topLevelMethod, SyntaxNode node, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
            : this(topLevelMethod, topLevelMethod.ContainingType, node, compilationState, diagnostics)
        {
        }

        public SyntheticBoundNodeFactory(MethodSymbol? topLevelMethodOpt, NamedTypeSymbol? currentClassOpt, SyntaxNode node, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            CompilationState = compilationState;
            CurrentType = currentClassOpt;
            TopLevelMethod = topLevelMethodOpt;
            CurrentFunction = topLevelMethodOpt;
            Syntax = node;
            Diagnostics = diagnostics;
        }

        [Conditional("DEBUG")]
        private void CheckCurrentType()
        {
            _ = CurrentType;
        }

        public void AddNestedType(NamedTypeSymbol nestedType)
        {
            ModuleBuilderOpt!.AddSynthesizedDefinition(CurrentType, nestedType.GetCciAdapter());
        }

        public void OpenNestedType(NamedTypeSymbol nestedType)
        {
            AddNestedType(nestedType);
            CurrentFunction = null;
            TopLevelMethod = null;
            CurrentType = nestedType;
        }

        public BoundHoistedFieldAccess HoistedField(FieldSymbol field)
        {
            return new BoundHoistedFieldAccess(Syntax, field, field.Type);
        }

        public StateMachineFieldSymbol StateMachineField(TypeWithAnnotations type, string name, bool isPublic = false, bool isThis = false)
        {
            StateMachineFieldSymbol stateMachineFieldSymbol = new StateMachineFieldSymbol(CurrentType, type, name, isPublic, isThis);
            AddField(CurrentType, stateMachineFieldSymbol);
            return stateMachineFieldSymbol;
        }

        public StateMachineFieldSymbol StateMachineField(TypeSymbol type, string name, bool isPublic = false, bool isThis = false)
        {
            StateMachineFieldSymbol stateMachineFieldSymbol = new StateMachineFieldSymbol(CurrentType, TypeWithAnnotations.Create(type), name, isPublic, isThis);
            AddField(CurrentType, stateMachineFieldSymbol);
            return stateMachineFieldSymbol;
        }

        public StateMachineFieldSymbol StateMachineField(TypeSymbol type, string name, SynthesizedLocalKind synthesizedKind, int slotIndex)
        {
            StateMachineFieldSymbol stateMachineFieldSymbol = new StateMachineFieldSymbol(CurrentType, type, name, synthesizedKind, slotIndex, isPublic: false);
            AddField(CurrentType, stateMachineFieldSymbol);
            return stateMachineFieldSymbol;
        }

        public StateMachineFieldSymbol StateMachineField(TypeSymbol type, string name, LocalSlotDebugInfo slotDebugInfo, int slotIndex)
        {
            StateMachineFieldSymbol stateMachineFieldSymbol = new StateMachineFieldSymbol(CurrentType, type, name, slotDebugInfo, slotIndex, isPublic: false);
            AddField(CurrentType, stateMachineFieldSymbol);
            return stateMachineFieldSymbol;
        }

        public void AddField(NamedTypeSymbol containingType, FieldSymbol field)
        {
            ModuleBuilderOpt!.AddSynthesizedDefinition(containingType, field.GetCciAdapter());
        }

        public GeneratedLabelSymbol GenerateLabel(string prefix)
        {
            return new GeneratedLabelSymbol(prefix);
        }

        public BoundThisReference This()
        {
            return new BoundThisReference(Syntax, CurrentFunction!.ThisParameter.Type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression This(LocalSymbol thisTempOpt)
        {
            if (!(thisTempOpt != null))
            {
                return This();
            }
            return Local(thisTempOpt);
        }

        public BoundBaseReference Base(NamedTypeSymbol baseType)
        {
            return new BoundBaseReference(Syntax, baseType)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundBadExpression BadExpression(TypeSymbol type)
        {
            return new BoundBadExpression(Syntax, LookupResultKind.Empty, ImmutableArray<Symbol>.Empty, ImmutableArray<BoundExpression>.Empty, type, hasErrors: true);
        }

        public BoundParameter Parameter(ParameterSymbol p)
        {
            return new BoundParameter(Syntax, p, p.Type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundFieldAccess Field(BoundExpression? receiver, FieldSymbol f)
        {
            return new BoundFieldAccess(Syntax, receiver, f, null, LookupResultKind.Viable, f.Type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundFieldAccess InstanceField(FieldSymbol f)
        {
            return Field(This(), f);
        }

        public BoundExpression Property(WellKnownMember member)
        {
            return Property(null, member);
        }

        public BoundExpression Property(BoundExpression? receiverOpt, WellKnownMember member)
        {
            PropertySymbol propertySymbol = (PropertySymbol)WellKnownMember(member);
            Binder.ReportUseSite(propertySymbol, Diagnostics, Syntax);
            return Property(receiverOpt, propertySymbol);
        }

        public BoundExpression Property(BoundExpression? receiverOpt, PropertySymbol property)
        {
            return Call(receiverOpt, property.GetMethod);
        }

        public BoundExpression Indexer(BoundExpression? receiverOpt, PropertySymbol property, BoundExpression arg0)
        {
            return Call(receiverOpt, property.GetMethod, arg0);
        }

        public NamedTypeSymbol SpecialType(SpecialType st)
        {
            NamedTypeSymbol specialType = Compilation.GetSpecialType(st);
            Binder.ReportUseSite(specialType, Diagnostics, Syntax);
            return specialType;
        }

        public ArrayTypeSymbol WellKnownArrayType(WellKnownType elementType)
        {
            return Compilation.CreateArrayTypeSymbol(WellKnownType(elementType));
        }

        public NamedTypeSymbol WellKnownType(WellKnownType wt)
        {
            NamedTypeSymbol wellKnownType = Compilation.GetWellKnownType(wt);
            Binder.ReportUseSite(wellKnownType, Diagnostics, Syntax);
            return wellKnownType;
        }

        public Symbol? WellKnownMember(WellKnownMember wm, bool isOptional)
        {
            Symbol wellKnownTypeMember = Binder.GetWellKnownTypeMember(Compilation, wm, Diagnostics, null, Syntax, isOptional: true);
            if ((object)wellKnownTypeMember == null && !isOptional)
            {
                MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(wm);
                throw new MissingPredefinedMember(new CSDiagnostic(new CSDiagnosticInfo(ErrorCode.ERR_MissingPredefinedMember, descriptor.DeclaringTypeMetadataName, descriptor.Name), Syntax.Location));
            }
            return wellKnownTypeMember;
        }

        public Symbol WellKnownMember(WellKnownMember wm)
        {
            return WellKnownMember(wm, isOptional: false);
        }

        public MethodSymbol? WellKnownMethod(WellKnownMember wm, bool isOptional)
        {
            return (MethodSymbol)WellKnownMember(wm, isOptional);
        }

        public MethodSymbol WellKnownMethod(WellKnownMember wm)
        {
            return (MethodSymbol)WellKnownMember(wm, isOptional: false);
        }

        public Symbol SpecialMember(SpecialMember sm)
        {
            Symbol specialTypeMember = Compilation.GetSpecialTypeMember(sm);
            if ((object)specialTypeMember == null)
            {
                MemberDescriptor descriptor = SpecialMembers.GetDescriptor(sm);
                throw new MissingPredefinedMember(new CSDiagnostic(new CSDiagnosticInfo(ErrorCode.ERR_MissingPredefinedMember, descriptor.DeclaringTypeMetadataName, descriptor.Name), Syntax.Location));
            }
            Binder.ReportUseSite(specialTypeMember, Diagnostics, Syntax);
            return specialTypeMember;
        }

        public MethodSymbol SpecialMethod(SpecialMember sm)
        {
            return (MethodSymbol)SpecialMember(sm);
        }

        public PropertySymbol SpecialProperty(SpecialMember sm)
        {
            return (PropertySymbol)SpecialMember(sm);
        }

        public BoundExpressionStatement Assignment(BoundExpression left, BoundExpression right, bool isRef = false)
        {
            return ExpressionStatement(AssignmentExpression(left, right, isRef));
        }

        public BoundExpressionStatement ExpressionStatement(BoundExpression expr)
        {
            return new BoundExpressionStatement(Syntax, expr)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundAssignmentOperator AssignmentExpression(BoundExpression left, BoundExpression right, bool isRef = false)
        {
            return new BoundAssignmentOperator(Syntax, left, right, left.Type, isRef)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundBlock Block()
        {
            return Block(ImmutableArray<BoundStatement>.Empty);
        }

        public BoundBlock Block(ImmutableArray<BoundStatement> statements)
        {
            return Block(ImmutableArray<LocalSymbol>.Empty, statements);
        }

        public BoundBlock Block(params BoundStatement[] statements)
        {
            return Block(ImmutableArray.Create(statements));
        }

        public BoundBlock Block(ImmutableArray<LocalSymbol> locals, params BoundStatement[] statements)
        {
            return Block(locals, ImmutableArray.Create(statements));
        }

        public BoundBlock Block(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> statements)
        {
            return new BoundBlock(Syntax, locals, statements)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundBlock Block(ImmutableArray<LocalSymbol> locals, ImmutableArray<LocalFunctionSymbol> localFunctions, params BoundStatement[] statements)
        {
            return Block(locals, localFunctions, ImmutableArray.Create(statements));
        }

        public BoundBlock Block(ImmutableArray<LocalSymbol> locals, ImmutableArray<LocalFunctionSymbol> localFunctions, ImmutableArray<BoundStatement> statements)
        {
            return new BoundBlock(Syntax, locals, localFunctions, statements)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExtractedFinallyBlock ExtractedFinallyBlock(BoundBlock finallyBlock)
        {
            return new BoundExtractedFinallyBlock(Syntax, finallyBlock)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundStatementList StatementList()
        {
            return StatementList(ImmutableArray<BoundStatement>.Empty);
        }

        public BoundStatementList StatementList(ImmutableArray<BoundStatement> statements)
        {
            return new BoundStatementList(Syntax, statements)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundStatementList StatementList(BoundStatement first, BoundStatement second)
        {
            return new BoundStatementList(Syntax, ImmutableArray.Create(first, second))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundReturnStatement Return(BoundExpression? expression = null)
        {
            if (expression != null)
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                Conversion conversion = Compilation.Conversions.ClassifyConversionFromType(expression!.Type, CurrentFunction!.ReturnType, ref useSiteInfo);
                if (conversion.Kind != ConversionKind.Identity)
                {
                    expression = BoundConversion.Synthesized(Syntax, expression, conversion, @checked: false, explicitCastInCode: false, null, null, CurrentFunction!.ReturnType);
                }
            }
            return new BoundReturnStatement(Syntax, CurrentFunction!.RefKind, expression)
            {
                WasCompilerGenerated = true
            };
        }

        public void CloseMethod(BoundStatement body)
        {
            if (body.Kind != BoundKind.Block)
            {
                body = Block(body);
            }
            CompilationState.AddSynthesizedMethod(CurrentFunction, body);
            CurrentFunction = null;
        }

        public LocalSymbol SynthesizedLocal(TypeSymbol type, SyntaxNode? syntax = null, bool isPinned = false, RefKind refKind = RefKind.None, SynthesizedLocalKind kind = SynthesizedLocalKind.LoweringTemp)
        {
            return new SynthesizedLocal(CurrentFunction, TypeWithAnnotations.Create(type), kind, syntax, isPinned, refKind);
        }

        public ParameterSymbol SynthesizedParameter(TypeSymbol type, string name, MethodSymbol? container = null, int ordinal = 0)
        {
            return SynthesizedParameterSymbol.Create(container, TypeWithAnnotations.Create(type), ordinal, RefKind.None, name);
        }

        public BoundBinaryOperator Binary(BinaryOperatorKind kind, TypeSymbol type, BoundExpression left, BoundExpression right)
        {
            return new BoundBinaryOperator(Syntax, kind, null, null, LookupResultKind.Viable, left, right, type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundAsOperator As(BoundExpression operand, TypeSymbol type)
        {
            return new BoundAsOperator(Syntax, operand, Type(type), Conversion.ExplicitReference, type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundIsOperator Is(BoundExpression operand, TypeSymbol type)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            Conversion conversion = Compilation.Conversions.ClassifyBuiltInConversion(operand.Type, type, ref useSiteInfo);
            return new BoundIsOperator(Syntax, operand, Type(type), conversion, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundBinaryOperator LogicalAnd(BoundExpression left, BoundExpression right)
        {
            return Binary(BinaryOperatorKind.LogicalBoolAnd, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
        }

        public BoundBinaryOperator LogicalOr(BoundExpression left, BoundExpression right)
        {
            return Binary(BinaryOperatorKind.LogicalBoolOr, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
        }

        public BoundBinaryOperator IntEqual(BoundExpression left, BoundExpression right)
        {
            return Binary(BinaryOperatorKind.IntEqual, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
        }

        public BoundBinaryOperator ObjectEqual(BoundExpression left, BoundExpression right)
        {
            return Binary(BinaryOperatorKind.ObjectEqual, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
        }

        public BoundBinaryOperator ObjectNotEqual(BoundExpression left, BoundExpression right)
        {
            return Binary(BinaryOperatorKind.ObjectNotEqual, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
        }

        public BoundBinaryOperator IntNotEqual(BoundExpression left, BoundExpression right)
        {
            return Binary(BinaryOperatorKind.IntNotEqual, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
        }

        public BoundBinaryOperator IntLessThan(BoundExpression left, BoundExpression right)
        {
            return Binary(BinaryOperatorKind.IntLessThan, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
        }

        public BoundBinaryOperator IntGreaterThanOrEqual(BoundExpression left, BoundExpression right)
        {
            return Binary(BinaryOperatorKind.IntGreaterThanOrEqual, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
        }

        public BoundBinaryOperator IntSubtract(BoundExpression left, BoundExpression right)
        {
            return Binary(BinaryOperatorKind.IntSubtraction, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32), left, right);
        }

        public BoundLiteral Literal(int value)
        {
            return new BoundLiteral(Syntax, ConstantValue.Create(value), SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundLiteral Literal(uint value)
        {
            return new BoundLiteral(Syntax, ConstantValue.Create(value), SpecialType(Microsoft.CodeAnalysis.SpecialType.System_UInt32))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundLiteral Literal(ConstantValue value, TypeSymbol type)
        {
            return new BoundLiteral(Syntax, value, type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundObjectCreationExpression New(NamedTypeSymbol type, params BoundExpression[] args)
        {
            BoundExpression[] args2 = args;
            MethodSymbol ctor = type.InstanceConstructors.Single((MethodSymbol c) => c.ParameterCount == args2.Length);
            return New(ctor, args2);
        }

        public BoundObjectCreationExpression New(MethodSymbol ctor, params BoundExpression[] args)
        {
            return New(ctor, args.ToImmutableArray());
        }

        public BoundObjectCreationExpression New(MethodSymbol ctor, ImmutableArray<BoundExpression> args)
        {
            return new BoundObjectCreationExpression(Syntax, ctor, args)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundObjectCreationExpression New(WellKnownMember wm, ImmutableArray<BoundExpression> args)
        {
            MethodSymbol constructor = WellKnownMethod(wm);
            return new BoundObjectCreationExpression(Syntax, constructor, args)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression MakeIsNotANumberTest(BoundExpression input)
        {
            TypeSymbol type = input.Type;
            if ((object)type != null)
            {
                switch (type.SpecialType)
                {
                    case Microsoft.CodeAnalysis.SpecialType.System_Double:
                        return StaticCall(Microsoft.CodeAnalysis.SpecialMember.System_Double__IsNaN, input);
                    case Microsoft.CodeAnalysis.SpecialType.System_Single:
                        return StaticCall(Microsoft.CodeAnalysis.SpecialMember.System_Single__IsNaN, input);
                }
            }
            throw ExceptionUtilities.UnexpectedValue(input.Type);
        }

        public BoundExpression InstanceCall(BoundExpression receiver, string name, BoundExpression arg)
        {
            return MakeInvocationExpression(BinderFlags.None, Syntax, receiver, name, ImmutableArray.Create(arg), Diagnostics);
        }

        public BoundExpression InstanceCall(BoundExpression receiver, string name)
        {
            return MakeInvocationExpression(BinderFlags.None, Syntax, receiver, name, ImmutableArray<BoundExpression>.Empty, Diagnostics);
        }

        public BoundExpression StaticCall(TypeSymbol receiver, string name, params BoundExpression[] args)
        {
            return MakeInvocationExpression(BinderFlags.None, Syntax, Type(receiver), name, args.ToImmutableArray(), Diagnostics);
        }

        public BoundExpression StaticCall(TypeSymbol receiver, string name, ImmutableArray<BoundExpression> args, bool allowUnexpandedForm)
        {
            SyntaxNode syntax = Syntax;
            BoundTypeExpression receiver2 = Type(receiver);
            BindingDiagnosticBag diagnostics = Diagnostics;
            bool allowUnexpandedForm2 = allowUnexpandedForm;
            return MakeInvocationExpression(BinderFlags.None, syntax, receiver2, name, args, diagnostics, default(ImmutableArray<TypeSymbol>), allowUnexpandedForm2);
        }

        public BoundExpression StaticCall(BinderFlags flags, TypeSymbol receiver, string name, ImmutableArray<TypeSymbol> typeArgs, params BoundExpression[] args)
        {
            return MakeInvocationExpression(flags, Syntax, Type(receiver), name, args.ToImmutableArray(), Diagnostics, typeArgs);
        }

        public BoundExpression StaticCall(TypeSymbol receiver, MethodSymbol method, params BoundExpression[] args)
        {
            if ((object)method == null)
            {
                return new BoundBadExpression(Syntax, LookupResultKind.Empty, ImmutableArray<Symbol>.Empty, args.AsImmutable(), receiver);
            }
            return Call(null, method, args);
        }

        public BoundExpression StaticCall(MethodSymbol method, ImmutableArray<BoundExpression> args)
        {
            return Call(null, method, args);
        }

        public BoundExpression StaticCall(WellKnownMember method, params BoundExpression[] args)
        {
            MethodSymbol methodSymbol = WellKnownMethod(method);
            Binder.ReportUseSite(methodSymbol, Diagnostics, Syntax);
            return Call(null, methodSymbol, args);
        }

        public BoundExpression StaticCall(SpecialMember method, params BoundExpression[] args)
        {
            MethodSymbol methodSymbol = SpecialMethod(method);
            Binder.ReportUseSite(methodSymbol, Diagnostics, Syntax);
            return Call(null, methodSymbol, args);
        }

        public BoundCall Call(BoundExpression? receiver, MethodSymbol method)
        {
            return Call(receiver, method, ImmutableArray<BoundExpression>.Empty);
        }

        public BoundCall Call(BoundExpression? receiver, MethodSymbol method, BoundExpression arg0)
        {
            return Call(receiver, method, ImmutableArray.Create(arg0));
        }

        public BoundCall Call(BoundExpression? receiver, MethodSymbol method, BoundExpression arg0, BoundExpression arg1)
        {
            return Call(receiver, method, ImmutableArray.Create(arg0, arg1));
        }

        public BoundCall Call(BoundExpression? receiver, MethodSymbol method, params BoundExpression[] args)
        {
            return Call(receiver, method, ImmutableArray.Create(args));
        }

        public BoundCall Call(BoundExpression? receiver, WellKnownMember method, BoundExpression arg0)
        {
            return Call(receiver, WellKnownMethod(method), ImmutableArray.Create(arg0));
        }

        public BoundCall Call(BoundExpression? receiver, MethodSymbol method, ImmutableArray<BoundExpression> args)
        {
            return new BoundCall(Syntax, receiver, method, args, default(ImmutableArray<string>), method.ParameterRefKinds, isDelegateCall: false, expanded: false, invokedAsExtensionMethod: false, default(ImmutableArray<int>), default(BitVector), LookupResultKind.Viable, method.ReturnType, method.OriginalDefinition is ErrorMethodSymbol)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundCall Call(BoundExpression? receiver, MethodSymbol method, ImmutableArray<RefKind> refKinds, ImmutableArray<BoundExpression> args)
        {
            return new BoundCall(Syntax, receiver, method, args, default(ImmutableArray<string>), refKinds, isDelegateCall: false, expanded: false, invokedAsExtensionMethod: false, ImmutableArray<int>.Empty, default(BitVector), LookupResultKind.Viable, method.ReturnType)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression Conditional(BoundExpression condition, BoundExpression consequence, BoundExpression alternative, TypeSymbol type)
        {
            return new BoundConditionalOperator(Syntax, isRef: false, condition, consequence, alternative, null, type, wasTargetTyped: false, type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression ComplexConditionalReceiver(BoundExpression valueTypeReceiver, BoundExpression referenceTypeReceiver)
        {
            return new BoundComplexConditionalReceiver(Syntax, valueTypeReceiver, referenceTypeReceiver, valueTypeReceiver.Type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression Coalesce(BoundExpression left, BoundExpression right)
        {
            return new BoundNullCoalescingOperator(Syntax, left, right, Conversion.Identity, BoundNullCoalescingOperatorResultKind.LeftType, left.Type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundStatement If(BoundExpression condition, BoundStatement thenClause, BoundStatement? elseClauseOpt = null)
        {
            return If(condition, ImmutableArray<LocalSymbol>.Empty, thenClause, elseClauseOpt);
        }

        public BoundStatement ConditionalGoto(BoundExpression condition, LabelSymbol label, bool jumpIfTrue)
        {
            return new BoundConditionalGoto(Syntax, condition, jumpIfTrue, label)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundStatement If(BoundExpression condition, ImmutableArray<LocalSymbol> locals, BoundStatement thenClause, BoundStatement? elseClauseOpt = null)
        {
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            GeneratedLabelSymbol label = new GeneratedLabelSymbol("afterif");
            if (elseClauseOpt != null)
            {
                GeneratedLabelSymbol label2 = new GeneratedLabelSymbol("alternative");
                instance.Add(ConditionalGoto(condition, label2, jumpIfTrue: false));
                instance.Add(thenClause);
                instance.Add(Goto(label));
                if (!locals.IsDefaultOrEmpty)
                {
                    BoundBlock item = Block(locals, instance.ToImmutable());
                    instance.Clear();
                    instance.Add(item);
                }
                instance.Add(Label(label2));
                instance.Add(elseClauseOpt);
            }
            else
            {
                instance.Add(ConditionalGoto(condition, label, jumpIfTrue: false));
                instance.Add(thenClause);
                if (!locals.IsDefaultOrEmpty)
                {
                    BoundBlock item2 = Block(locals, instance.ToImmutable());
                    instance.Clear();
                    instance.Add(item2);
                }
            }
            instance.Add(Label(label));
            return Block(instance.ToImmutableAndFree());
        }

        public BoundThrowStatement Throw(BoundExpression e)
        {
            return new BoundThrowStatement(Syntax, e)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundLocal Local(LocalSymbol local)
        {
            return new BoundLocal(Syntax, local, null, local.Type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression MakeSequence(LocalSymbol temp, params BoundExpression[] parts)
        {
            return MakeSequence(ImmutableArray.Create(temp), parts);
        }

        public BoundExpression MakeSequence(params BoundExpression[] parts)
        {
            return MakeSequence(ImmutableArray<LocalSymbol>.Empty, parts);
        }

        public BoundExpression MakeSequence(ImmutableArray<LocalSymbol> locals, params BoundExpression[] parts)
        {
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (LocalRewriter.ReadIsSideeffecting(parts[i]))
                {
                    instance.Add(parts[i]);
                }
            }
            BoundExpression result = parts[^1];
            if (locals.IsDefaultOrEmpty && instance.Count == 0)
            {
                instance.Free();
                return result;
            }
            return Sequence(locals, instance.ToImmutableAndFree(), result);
        }

        public BoundSequence Sequence(BoundExpression[] sideEffects, BoundExpression result, TypeSymbol? type = null)
        {
            TypeSymbol type2 = type ?? result.Type;
            return new BoundSequence(Syntax, ImmutableArray<LocalSymbol>.Empty, sideEffects.AsImmutableOrNull(), result, type2)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression Sequence(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> sideEffects, BoundExpression result)
        {
            if (!locals.IsDefaultOrEmpty || !sideEffects.IsDefaultOrEmpty)
            {
                return new BoundSequence(Syntax, locals, sideEffects, result, result.Type)
                {
                    WasCompilerGenerated = true
                };
            }
            return result;
        }

        public BoundSpillSequence SpillSequence(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> sideEffects, BoundExpression result)
        {
            return new BoundSpillSequence(Syntax, locals, sideEffects, result, result.Type)
            {
                WasCompilerGenerated = true
            };
        }

        public SyntheticSwitchSection SwitchSection(int value, params BoundStatement[] statements)
        {
            return new SyntheticSwitchSection(ImmutableArray.Create(value), ImmutableArray.Create(statements));
        }

        public SyntheticSwitchSection SwitchSection(List<int> values, params BoundStatement[] statements)
        {
            return new SyntheticSwitchSection(ImmutableArray.CreateRange(values), ImmutableArray.Create(statements));
        }

        public BoundStatement Switch(BoundExpression ex, ImmutableArray<SyntheticSwitchSection> sections)
        {
            if (sections.Length == 0)
            {
                return ExpressionStatement(ex);
            }
            GeneratedLabelSymbol generatedLabelSymbol = new GeneratedLabelSymbol("break");
            ArrayBuilder<(ConstantValue, LabelSymbol)> instance = ArrayBuilder<(ConstantValue, LabelSymbol)>.GetInstance();
            ArrayBuilder<BoundStatement> instance2 = ArrayBuilder<BoundStatement>.GetInstance();
            instance2.Add(null);
            ImmutableArray<SyntheticSwitchSection>.Enumerator enumerator = sections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntheticSwitchSection current = enumerator.Current;
                LabelSymbol labelSymbol = new GeneratedLabelSymbol("case " + current.Values[0]);
                instance2.Add(Label(labelSymbol));
                instance2.AddRange(current.Statements);
                ImmutableArray<int>.Enumerator enumerator2 = current.Values.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    int current2 = enumerator2.Current;
                    instance.Add((ConstantValue.Create(current2), labelSymbol));
                }
            }
            instance2.Add(Label(generatedLabelSymbol));
            instance2[0] = new BoundSwitchDispatch(Syntax, ex, instance.ToImmutableAndFree(), generatedLabelSymbol, null)
            {
                WasCompilerGenerated = true
            };
            return Block(instance2.ToImmutableAndFree());
        }

        [Conditional("DEBUG")]
        private static void CheckSwitchSections(ImmutableArray<SyntheticSwitchSection> sections)
        {
            HashSet<int> hashSet = new HashSet<int>();
            ImmutableArray<SyntheticSwitchSection>.Enumerator enumerator = sections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ImmutableArray<int>.Enumerator enumerator2 = enumerator.Current.Values.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    int current = enumerator2.Current;
                    hashSet.Add(current);
                }
            }
        }

        public BoundGotoStatement Goto(LabelSymbol label)
        {
            return new BoundGotoStatement(Syntax, label)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundLabelStatement Label(LabelSymbol label)
        {
            return new BoundLabelStatement(Syntax, label)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundLiteral Literal(bool value)
        {
            return new BoundLiteral(Syntax, ConstantValue.Create(value), SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundLiteral Literal(string? value)
        {
            ConstantValue stringConst = ConstantValue.Create(value);
            return StringLiteral(stringConst);
        }

        public BoundLiteral StringLiteral(ConstantValue stringConst)
        {
            return new BoundLiteral(Syntax, stringConst, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_String))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundLiteral StringLiteral(string stringValue)
        {
            return StringLiteral(ConstantValue.Create(stringValue));
        }

        public BoundArrayLength ArrayLength(BoundExpression array)
        {
            return new BoundArrayLength(Syntax, array, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32));
        }

        public BoundArrayAccess ArrayAccessFirstElement(BoundExpression array)
        {
            ImmutableArray<BoundExpression> indices = ArrayBuilder<BoundExpression>.GetInstance(((ArrayTypeSymbol)array.Type).Rank, Literal(0)).ToImmutableAndFree();
            return ArrayAccess(array, indices);
        }

        public BoundArrayAccess ArrayAccess(BoundExpression array, params BoundExpression[] indices)
        {
            return ArrayAccess(array, indices.AsImmutableOrNull());
        }

        public BoundArrayAccess ArrayAccess(BoundExpression array, ImmutableArray<BoundExpression> indices)
        {
            return new BoundArrayAccess(Syntax, array, indices, ((ArrayTypeSymbol)array.Type).ElementType);
        }

        public BoundStatement BaseInitialization()
        {
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = CurrentFunction!.ThisParameter.Type.BaseTypeNoUseSiteDiagnostics;
            MethodSymbol method = baseTypeNoUseSiteDiagnostics.InstanceConstructors.Single((MethodSymbol c) => c.ParameterCount == 0);
            return new BoundExpressionStatement(Syntax, Call(Base(baseTypeNoUseSiteDiagnostics), method))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundStatement SequencePoint(SyntaxNode syntax, BoundStatement statement)
        {
            return new BoundSequencePoint(syntax, statement);
        }

        public BoundStatement SequencePointWithSpan(CSharpSyntaxNode syntax, TextSpan span, BoundStatement statement)
        {
            return new BoundSequencePointWithSpan(syntax, statement, span);
        }

        public BoundStatement HiddenSequencePoint(BoundStatement? statementOpt = null)
        {
            return BoundSequencePoint.CreateHidden(statementOpt);
        }

        public BoundStatement ThrowNull()
        {
            return Throw(Null(Binder.GetWellKnownType(Compilation, Microsoft.CodeAnalysis.WellKnownType.System_Exception, Diagnostics, Syntax.Location)));
        }

        public BoundExpression ThrowExpression(BoundExpression thrown, TypeSymbol type)
        {
            return new BoundThrowExpression(thrown.Syntax, thrown, type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression Null(TypeSymbol type)
        {
            return Null(type, Syntax);
        }

        public static BoundExpression Null(TypeSymbol type, SyntaxNode syntax)
        {
            BoundExpression boundExpression = new BoundLiteral(syntax, ConstantValue.Null, type)
            {
                WasCompilerGenerated = true
            };
            if (!type.IsPointerOrFunctionPointer())
            {
                return boundExpression;
            }
            return BoundConversion.SynthesizedNonUserDefined(syntax, boundExpression, Conversion.NullToPointer, type);
        }

        public BoundTypeExpression Type(TypeSymbol type)
        {
            return new BoundTypeExpression(Syntax, null, type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression Typeof(WellKnownType type)
        {
            return Typeof(WellKnownType(type));
        }

        public BoundExpression Typeof(TypeSymbol type)
        {
            return new BoundTypeOfOperator(Syntax, Type(type), WellKnownMethod(Microsoft.CodeAnalysis.WellKnownMember.System_Type__GetTypeFromHandle), WellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Type))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression Typeof(TypeWithAnnotations type)
        {
            return Typeof(type.Type);
        }

        public ImmutableArray<BoundExpression> TypeOfs(ImmutableArray<TypeWithAnnotations> typeArguments)
        {
            return typeArguments.SelectAsArray(Typeof);
        }

        public BoundExpression TypeofDynamicOperationContextType()
        {
            return Typeof(CompilationState.DynamicOperationContextType);
        }

        public BoundExpression Sizeof(TypeSymbol type)
        {
            return new BoundSizeOfOperator(Syntax, Type(type), Binder.GetConstantSizeOf(type), SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32))
            {
                WasCompilerGenerated = true
            };
        }

        internal BoundExpression ConstructorInfo(MethodSymbol ctor)
        {
            return new BoundMethodInfo(Syntax, ctor, GetMethodFromHandleMethod(ctor.ContainingType), WellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Reflection_ConstructorInfo))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression MethodDefIndex(MethodSymbol method)
        {
            return new BoundMethodDefIndex(Syntax, method, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression ModuleVersionId()
        {
            return new BoundModuleVersionId(Syntax, WellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Guid))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression ModuleVersionIdString()
        {
            return new BoundModuleVersionIdString(Syntax, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_String))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression InstrumentationPayloadRoot(int analysisKind, TypeSymbol payloadType)
        {
            return new BoundInstrumentationPayloadRoot(Syntax, analysisKind, payloadType)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression MaximumMethodDefIndex()
        {
            return new BoundMaximumMethodDefIndex(Syntax, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression SourceDocumentIndex(DebugSourceDocument document)
        {
            return new BoundSourceDocumentIndex(Syntax, document, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression MethodInfo(MethodSymbol method)
        {
            if (!method.ContainingType.IsValueType || !CodeGenerator.MayUseCallForStructMethod(method))
            {
                method = method.GetConstructedLeastOverriddenMethod(CompilationState.Type, requireSameReturnType: true);
            }
            return new BoundMethodInfo(Syntax, method, GetMethodFromHandleMethod(method.ContainingType), WellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Reflection_MethodInfo))
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression FieldInfo(FieldSymbol field)
        {
            return new BoundFieldInfo(Syntax, field, GetFieldFromHandleMethod(field.ContainingType), WellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Reflection_FieldInfo))
            {
                WasCompilerGenerated = true
            };
        }

        private MethodSymbol GetMethodFromHandleMethod(NamedTypeSymbol methodContainer)
        {
            return WellKnownMethod((methodContainer.AllTypeArgumentCount() == 0 && !methodContainer.IsAnonymousType) ? Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle : Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle2);
        }

        private MethodSymbol GetFieldFromHandleMethod(NamedTypeSymbol fieldContainer)
        {
            return WellKnownMethod((fieldContainer.AllTypeArgumentCount() == 0) ? Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle : Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle2);
        }

        public BoundExpression Convert(TypeSymbol type, BoundExpression arg)
        {
            if (TypeSymbol.Equals(type, arg.Type, TypeCompareKind.ConsiderEverything))
            {
                return arg;
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            Conversion conversion = Compilation.Conversions.ClassifyConversionFromExpression(arg, type, ref useSiteInfo);
            return Convert(type, arg, conversion);
        }

        public BoundExpression Convert(TypeSymbol type, BoundExpression arg, Conversion conversion, bool isChecked = false)
        {
            if ((object)conversion.Method != null && !TypeSymbol.Equals(conversion.Method!.Parameters[0].Type, arg.Type, TypeCompareKind.ConsiderEverything))
            {
                arg = Convert(conversion.Method!.Parameters[0].Type, arg);
            }
            if (conversion.Kind == ConversionKind.ImplicitReference && arg.IsLiteralNull())
            {
                return Null(type);
            }
            if (conversion.Kind == ConversionKind.ExplicitNullable && arg.Type.IsNullableType() && arg.Type.GetNullableUnderlyingType().Equals(type, TypeCompareKind.AllIgnoreOptions))
            {
                return Call(arg, SpecialMethod(Microsoft.CodeAnalysis.SpecialMember.System_Nullable_T_get_Value).AsMember((NamedTypeSymbol)arg.Type));
            }
            return new BoundConversion(Syntax, arg, conversion, isChecked, explicitCastInCode: true, null, null, type)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundExpression ArrayOrEmpty(TypeSymbol elementType, BoundExpression[] elements)
        {
            return ArrayOrEmpty(elementType, elements.AsImmutable());
        }

        public BoundExpression ArrayOrEmpty(TypeSymbol elementType, ImmutableArray<BoundExpression> elements)
        {
            if (elements.Length == 0)
            {
                MethodSymbol methodSymbol = WellKnownMethod(Microsoft.CodeAnalysis.WellKnownMember.System_Array__Empty, isOptional: true);
                if ((object)methodSymbol != null)
                {
                    methodSymbol = methodSymbol.Construct(ImmutableArray.Create(elementType));
                    return Call(null, methodSymbol);
                }
            }
            return Array(elementType, elements);
        }

        public BoundExpression Array(TypeSymbol elementType, ImmutableArray<BoundExpression> elements)
        {
            return new BoundArrayCreation(Syntax, ImmutableArray.Create((BoundExpression)Literal(elements.Length)), new BoundArrayInitialization(Syntax, elements)
            {
                WasCompilerGenerated = true
            }, Compilation.CreateArrayTypeSymbol(elementType));
        }

        public BoundExpression Array(TypeSymbol elementType, BoundExpression length)
        {
            return new BoundArrayCreation(Syntax, ImmutableArray.Create(length), null, Compilation.CreateArrayTypeSymbol(elementType))
            {
                WasCompilerGenerated = true
            };
        }

        internal BoundExpression Default(TypeSymbol type)
        {
            return Default(type, Syntax);
        }

        internal static BoundExpression Default(TypeSymbol type, SyntaxNode syntax)
        {
            return new BoundDefaultExpression(syntax, type)
            {
                WasCompilerGenerated = true
            };
        }

        internal BoundStatement Try(BoundBlock tryBlock, ImmutableArray<BoundCatchBlock> catchBlocks, BoundBlock? finallyBlock = null, LabelSymbol? finallyLabel = null)
        {
            return new BoundTryStatement(Syntax, tryBlock, catchBlocks, finallyBlock, finallyLabel)
            {
                WasCompilerGenerated = true
            };
        }

        internal ImmutableArray<BoundCatchBlock> CatchBlocks(params BoundCatchBlock[] catchBlocks)
        {
            return catchBlocks.AsImmutableOrNull();
        }

        internal BoundCatchBlock Catch(LocalSymbol local, BoundBlock block)
        {
            BoundLocal boundLocal = Local(local);
            return new BoundCatchBlock(Syntax, ImmutableArray.Create(local), boundLocal, boundLocal.Type, null, null, block, isSynthesizedAsyncCatchAll: false);
        }

        internal BoundCatchBlock Catch(BoundExpression source, BoundBlock block)
        {
            return new BoundCatchBlock(Syntax, ImmutableArray<LocalSymbol>.Empty, source, source.Type, null, null, block, isSynthesizedAsyncCatchAll: false);
        }

        internal BoundTryStatement Fault(BoundBlock tryBlock, BoundBlock faultBlock)
        {
            return new BoundTryStatement(Syntax, tryBlock, ImmutableArray<BoundCatchBlock>.Empty, faultBlock, null, preferFaultHandler: true);
        }

        internal BoundExpression NullOrDefault(TypeSymbol typeSymbol)
        {
            return NullOrDefault(typeSymbol, Syntax);
        }

        internal static BoundExpression NullOrDefault(TypeSymbol typeSymbol, SyntaxNode syntax)
        {
            if (!typeSymbol.IsReferenceType)
            {
                return Default(typeSymbol, syntax);
            }
            return Null(typeSymbol, syntax);
        }

        internal BoundExpression Not(BoundExpression expression)
        {
            return new BoundUnaryOperator(expression.Syntax, UnaryOperatorKind.BoolLogicalNegation, expression, null, null, LookupResultKind.Viable, expression.Type);
        }

        public BoundLocal StoreToTemp(BoundExpression argument, out BoundAssignmentOperator store, RefKind refKind = RefKind.None, SynthesizedLocalKind kind = SynthesizedLocalKind.LoweringTemp, SyntaxNode? syntaxOpt = null)
        {
            MethodSymbol currentFunction = CurrentFunction;
            switch (refKind)
            {
                case RefKind.Out:
                    refKind = RefKind.Ref;
                    break;
                case RefKind.In:
                    if (!Binder.HasHome(argument, Binder.AddressKind.ReadOnly, currentFunction, Compilation.IsPeVerifyCompatEnabled, null))
                    {
                        refKind = RefKind.None;
                    }
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(refKind);
                case RefKind.None:
                case RefKind.Ref:
                case (RefKind)4:
                    break;
            }
            SyntaxNode syntax = argument.Syntax;
            TypeSymbol type = argument.Type;
            BoundLocal boundLocal = new BoundLocal(syntax, new SynthesizedLocal(currentFunction, TypeWithAnnotations.Create(type), kind, syntaxOpt ?? (kind.IsLongLived() ? syntax : null), isPinned: false, refKind), null, type);
            store = new BoundAssignmentOperator(syntax, boundLocal, argument, refKind != RefKind.None, type);
            return boundLocal;
        }

        internal BoundStatement NoOp(NoOpStatementFlavor noOpStatementFlavor)
        {
            return new BoundNoOpStatement(Syntax, noOpStatementFlavor);
        }

        internal BoundLocal MakeTempForDiscard(BoundDiscardExpression node, ArrayBuilder<LocalSymbol> temps)
        {
            BoundLocal result = MakeTempForDiscard(node, out LocalSymbol temp);
            temps.Add(temp);
            return result;
        }

        internal BoundLocal MakeTempForDiscard(BoundDiscardExpression node, out LocalSymbol temp)
        {
            temp = new SynthesizedLocal(CurrentFunction, TypeWithAnnotations.Create(node.Type), SynthesizedLocalKind.LoweringTemp);
            return new BoundLocal(node.Syntax, temp, null, node.Type)
            {
                WasCompilerGenerated = true
            };
        }

        internal ImmutableArray<BoundExpression> MakeTempsForDiscardArguments(ImmutableArray<BoundExpression> arguments, ArrayBuilder<LocalSymbol> builder)
        {
            if (arguments.Any((BoundExpression a) => a.Kind == BoundKind.DiscardExpression))
            {
                arguments = arguments.SelectAsArray((BoundExpression arg, (SyntheticBoundNodeFactory factory, ArrayBuilder<LocalSymbol> builder) t) => (arg.Kind != BoundKind.DiscardExpression) ? arg : t.factory.MakeTempForDiscard((BoundDiscardExpression)arg, t.builder), (this, builder));
            }
            return arguments;
        }
    }
}

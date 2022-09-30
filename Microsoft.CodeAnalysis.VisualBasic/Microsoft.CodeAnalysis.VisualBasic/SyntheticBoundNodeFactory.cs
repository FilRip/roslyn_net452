using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SyntheticBoundNodeFactory
	{
		private NamedTypeSymbol _currentClass;

		private SyntaxNode _syntax;

		public readonly BindingDiagnosticBag Diagnostics;

		public readonly MethodSymbol TopLevelMethod;

		public readonly TypeCompilationState CompilationState;

		public MethodSymbol CurrentMethod { get; set; }

		public NamedTypeSymbol CurrentType => _currentClass;

		public VisualBasicCompilation Compilation => CompilationState.Compilation;

		public SyntaxNode Syntax
		{
			get
			{
				return _syntax;
			}
			set
			{
				_syntax = value;
			}
		}

		private PEModuleBuilder EmitModule
		{
			get
			{
				if (CompilationState == null)
				{
					return null;
				}
				return CompilationState.ModuleBuilderOpt;
			}
		}

		public SyntheticBoundNodeFactory(MethodSymbol topLevelMethod, MethodSymbol currentMethod, SyntaxNode node, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
			: this(topLevelMethod, currentMethod, null, node, compilationState, diagnostics)
		{
		}

		public SyntheticBoundNodeFactory(MethodSymbol topLevelMethod, MethodSymbol currentMethod, NamedTypeSymbol currentClass, SyntaxNode node, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
		{
			CompilationState = compilationState;
			CurrentMethod = currentMethod;
			TopLevelMethod = topLevelMethod;
			_currentClass = currentClass;
			_syntax = node;
			Diagnostics = diagnostics;
		}

		public void AddNestedType(NamedTypeSymbol nestedType)
		{
			EmitModule?.AddSynthesizedDefinition(_currentClass, nestedType.GetCciAdapter());
		}

		public void OpenNestedType(NamedTypeSymbol nestedType)
		{
			AddNestedType(nestedType);
			_currentClass = nestedType;
			CurrentMethod = null;
		}

		public void AddField(NamedTypeSymbol containingType, FieldSymbol field)
		{
			EmitModule?.AddSynthesizedDefinition(containingType, field.GetCciAdapter());
		}

		public void AddMethod(NamedTypeSymbol containingType, MethodSymbol method)
		{
			EmitModule?.AddSynthesizedDefinition(containingType, method.GetCciAdapter());
		}

		public void AddProperty(NamedTypeSymbol containingType, PropertySymbol prop)
		{
			EmitModule?.AddSynthesizedDefinition(containingType, prop.GetCciAdapter());
		}

		public SynthesizedFieldSymbol StateMachineField(TypeSymbol type, Symbol implicitlyDefinedBy, string name, Accessibility accessibility = Accessibility.Private)
		{
			StateMachineFieldSymbol stateMachineFieldSymbol = new StateMachineFieldSymbol(CurrentType, implicitlyDefinedBy, type, name, accessibility);
			AddField(CurrentType, stateMachineFieldSymbol);
			return stateMachineFieldSymbol;
		}

		public SynthesizedFieldSymbol StateMachineField(TypeSymbol type, Symbol implicitlyDefinedBy, string name, SynthesizedLocalKind synthesizedKind, int slotIndex, Accessibility accessibility = Accessibility.Private)
		{
			StateMachineFieldSymbol stateMachineFieldSymbol = new StateMachineFieldSymbol(CurrentType, implicitlyDefinedBy, type, name, synthesizedKind, slotIndex, accessibility);
			AddField(CurrentType, stateMachineFieldSymbol);
			return stateMachineFieldSymbol;
		}

		public SynthesizedFieldSymbol StateMachineField(TypeSymbol type, Symbol implicitlyDefinedBy, string name, LocalSlotDebugInfo slotDebugInfo, int slotIndex, Accessibility accessibility = Accessibility.Private)
		{
			StateMachineFieldSymbol stateMachineFieldSymbol = new StateMachineFieldSymbol(CurrentType, implicitlyDefinedBy, type, name, slotDebugInfo, slotIndex, accessibility);
			AddField(CurrentType, stateMachineFieldSymbol);
			return stateMachineFieldSymbol;
		}

		public GeneratedLabelSymbol GenerateLabel(string prefix)
		{
			return new GeneratedLabelSymbol(prefix);
		}

		public BoundMeReference Me()
		{
			BoundMeReference boundMeReference = new BoundMeReference(_syntax, CurrentMethod.MeParameter.Type);
			boundMeReference.SetWasCompilerGenerated();
			return boundMeReference;
		}

		public BoundExpression ReferenceOrByrefMe()
		{
			BoundExpression obj = (CurrentMethod.MeParameter.Type.IsReferenceType ? ((BoundExpression)Me()) : ((BoundExpression)new BoundValueTypeMeReference(_syntax, CurrentMethod.MeParameter.Type)));
			obj.SetWasCompilerGenerated();
			return obj;
		}

		public BoundMyBaseReference Base()
		{
			BoundMyBaseReference boundMyBaseReference = new BoundMyBaseReference(_syntax, CurrentMethod.MeParameter.Type.BaseTypeNoUseSiteDiagnostics);
			boundMyBaseReference.SetWasCompilerGenerated();
			return boundMyBaseReference;
		}

		public BoundParameter Parameter(ParameterSymbol p)
		{
			BoundParameter boundParameter = new BoundParameter(_syntax, p, p.Type);
			boundParameter.SetWasCompilerGenerated();
			return boundParameter;
		}

		public BoundFieldAccess Field(BoundExpression receiver, FieldSymbol f, bool isLValue)
		{
			BoundFieldAccess boundFieldAccess = new BoundFieldAccess(_syntax, receiver, f, isLValue, f.Type);
			boundFieldAccess.SetWasCompilerGenerated();
			return boundFieldAccess;
		}

		public BoundExpression Property(WellKnownMember member)
		{
			PropertySymbol propertySymbol = WellKnownMember<PropertySymbol>(member);
			return Call(null, propertySymbol.GetMethod);
		}

		public BoundExpression Property(BoundExpression receiver, WellKnownMember member)
		{
			PropertySymbol propertySymbol = WellKnownMember<PropertySymbol>(member);
			return Call(receiver, propertySymbol.GetMethod);
		}

		public BoundExpression Property(BoundExpression receiver, string name)
		{
			PropertySymbol propertySymbol = receiver.Type.GetMembers(name).OfType<PropertySymbol>().Single();
			return Call(receiver, propertySymbol.GetMethod);
		}

		public BoundExpression Property(NamedTypeSymbol receiver, string name)
		{
			PropertySymbol propertySymbol = receiver.GetMembers(name).OfType<PropertySymbol>().Single();
			return Call(null, propertySymbol.GetMethod);
		}

		public NamedTypeSymbol SpecialType(SpecialType st)
		{
			return Binder.GetSpecialType(Compilation, st, _syntax, Diagnostics);
		}

		public NamedTypeSymbol NullableOf(TypeSymbol type)
		{
			NamedTypeSymbol namedTypeSymbol = SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Nullable_T);
			if (TypeSymbolExtensions.IsErrorType(namedTypeSymbol))
			{
				return namedTypeSymbol;
			}
			return namedTypeSymbol.Construct(ImmutableArray.Create(type));
		}

		public NamedTypeSymbol WellKnownType(WellKnownType wt)
		{
			return Binder.GetWellKnownType(Compilation, wt, _syntax, Diagnostics);
		}

		public T WellKnownMember<T>(WellKnownMember wm, bool isOptional = false) where T : Symbol
		{
			UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			T result = (T)Binder.GetWellKnownTypeMember(Compilation, wm, out useSiteInfo);
			if (useSiteInfo.DiagnosticInfo != null && isOptional)
			{
				result = null;
			}
			else
			{
				Diagnostics.Add(useSiteInfo, _syntax);
			}
			return result;
		}

		public Symbol SpecialMember(SpecialMember sm)
		{
			Symbol specialTypeMember = Compilation.GetSpecialTypeMember(sm);
			UseSiteInfo<AssemblySymbol> useSiteInfo;
			if ((object)specialTypeMember == null)
			{
				MemberDescriptor descriptor = SpecialMembers.GetDescriptor(sm);
				useSiteInfo = new UseSiteInfo<AssemblySymbol>(MissingRuntimeMemberDiagnosticHelper.GetDiagnosticForMissingRuntimeHelper(descriptor.DeclaringTypeMetadataName, descriptor.Name, CompilationState.Compilation.Options.EmbedVbCoreRuntime));
			}
			else
			{
				useSiteInfo = Binder.GetUseSiteInfoForMemberAndContainingType(specialTypeMember);
			}
			Diagnostics.Add(useSiteInfo, _syntax);
			return specialTypeMember;
		}

		public BoundExpressionStatement Assignment(BoundExpression left, BoundExpression right)
		{
			return ExpressionStatement(AssignmentExpression(left, right));
		}

		public BoundExpressionStatement ExpressionStatement(BoundExpression expr)
		{
			BoundExpressionStatement boundExpressionStatement = new BoundExpressionStatement(_syntax, expr);
			boundExpressionStatement.SetWasCompilerGenerated();
			return boundExpressionStatement;
		}

		public BoundAssignmentOperator AssignmentExpression(BoundExpression left, BoundExpression right)
		{
			BoundAssignmentOperator boundAssignmentOperator = new BoundAssignmentOperator(_syntax, left, right, suppressObjectClone: true);
			boundAssignmentOperator.SetWasCompilerGenerated();
			return boundAssignmentOperator;
		}

		public BoundReferenceAssignment ReferenceAssignment(LocalSymbol byRefLocal, BoundExpression lValue)
		{
			BoundReferenceAssignment boundReferenceAssignment = new BoundReferenceAssignment(_syntax, Local(byRefLocal, isLValue: true), lValue, isLValue: true, lValue.Type);
			boundReferenceAssignment.SetWasCompilerGenerated();
			return boundReferenceAssignment;
		}

		public BoundBlock Block(ImmutableArray<BoundStatement> statements)
		{
			return Block(ImmutableArray<LocalSymbol>.Empty, statements);
		}

		public BoundBlock Block(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> statements)
		{
			BoundBlock boundBlock = new BoundBlock(_syntax, default(SyntaxList<StatementSyntax>), locals, statements);
			boundBlock.SetWasCompilerGenerated();
			return boundBlock;
		}

		public BoundBlock Block()
		{
			return Block(ImmutableArray<BoundStatement>.Empty);
		}

		public BoundBlock Block(params BoundStatement[] statements)
		{
			return Block(ImmutableArray.Create(statements));
		}

		public BoundBlock Block(ImmutableArray<LocalSymbol> locals, params BoundStatement[] statements)
		{
			return Block(locals, ImmutableArray.Create(statements));
		}

		public BoundStatementList StatementList()
		{
			return StatementList(ImmutableArray<BoundStatement>.Empty);
		}

		public BoundStatementList StatementList(ImmutableArray<BoundStatement> statements)
		{
			BoundStatementList boundStatementList = new BoundStatementList(Syntax, statements);
			boundStatementList.SetWasCompilerGenerated();
			return boundStatementList;
		}

		public BoundStatementList StatementList(BoundStatement first, BoundStatement second)
		{
			BoundStatementList boundStatementList = new BoundStatementList(Syntax, ImmutableArray.Create(first, second));
			boundStatementList.SetWasCompilerGenerated();
			return boundStatementList;
		}

		public BoundReturnStatement Return(BoundExpression expression = null)
		{
			if (expression != null)
			{
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(Diagnostics, Compilation.Assembly);
				ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(expression.Type, CurrentMethod.ReturnType, ref useSiteInfo);
				Diagnostics.Add(expression, useSiteInfo);
				if (!Conversions.IsIdentityConversion(conversionKind))
				{
					expression = new BoundDirectCast(Syntax, expression, conversionKind, CurrentMethod.ReturnType);
				}
			}
			BoundReturnStatement boundReturnStatement = new BoundReturnStatement(_syntax, expression, null, null);
			boundReturnStatement.SetWasCompilerGenerated();
			return boundReturnStatement;
		}

		public LocalSymbol SynthesizedLocal(TypeSymbol type, SynthesizedLocalKind kind = SynthesizedLocalKind.LoweringTemp, SyntaxNode syntax = null)
		{
			return new SynthesizedLocal(CurrentMethod, type, kind, syntax);
		}

		public ParameterSymbol SynthesizedParameter(TypeSymbol type, string name, MethodSymbol container = null, int ordinal = 0)
		{
			return new SynthesizedParameterSymbol(container, type, ordinal, isByRef: false, name);
		}

		public BoundBinaryOperator LogicalAndAlso(BoundExpression left, BoundExpression right)
		{
			return Binary(BinaryOperatorKind.AndAlso, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
		}

		public BoundBinaryOperator LogicalOrElse(BoundExpression left, BoundExpression right)
		{
			return Binary(BinaryOperatorKind.OrElse, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
		}

		public BoundBinaryOperator IntEqual(BoundExpression left, BoundExpression right)
		{
			return Binary(BinaryOperatorKind.Equals, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
		}

		public BoundBinaryOperator IntLessThan(BoundExpression left, BoundExpression right)
		{
			return Binary(BinaryOperatorKind.LessThan, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
		}

		public BoundLiteral Literal(bool value)
		{
			BoundLiteral boundLiteral = new BoundLiteral(_syntax, ConstantValue.Create(value), SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean));
			boundLiteral.SetWasCompilerGenerated();
			return boundLiteral;
		}

		public BoundLiteral Literal(int value)
		{
			BoundLiteral boundLiteral = new BoundLiteral(_syntax, ConstantValue.Create(value), SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32));
			boundLiteral.SetWasCompilerGenerated();
			return boundLiteral;
		}

		public BoundExpression BadExpression(params BoundExpression[] subExpressions)
		{
			BoundBadExpression boundBadExpression = new BoundBadExpression(_syntax, LookupResultKind.Empty, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(subExpressions), ErrorTypeSymbol.UnknownResultType, hasErrors: true);
			boundBadExpression.SetWasCompilerGenerated();
			return boundBadExpression;
		}

		public BoundObjectCreationExpression New(NamedTypeSymbol type)
		{
			MethodSymbol ctor = type.InstanceConstructors.Single((MethodSymbol c) => c.ParameterCount == 0);
			return New(ctor);
		}

		public BoundObjectCreationExpression New(MethodSymbol ctor, params BoundExpression[] args)
		{
			BoundObjectCreationExpression boundObjectCreationExpression = new BoundObjectCreationExpression(_syntax, ctor, ImmutableArray.Create(args), null, ctor.ContainingType);
			boundObjectCreationExpression.SetWasCompilerGenerated();
			return boundObjectCreationExpression;
		}

		public BoundObjectCreationExpression New(MethodSymbol ctor)
		{
			BoundObjectCreationExpression boundObjectCreationExpression = new BoundObjectCreationExpression(_syntax, ctor, ImmutableArray<BoundExpression>.Empty, null, ctor.ContainingType);
			boundObjectCreationExpression.SetWasCompilerGenerated();
			return boundObjectCreationExpression;
		}

		public BoundCall Call(BoundExpression receiver, MethodSymbol method)
		{
			return Call(receiver, method, ImmutableArray<BoundExpression>.Empty);
		}

		public BoundCall Call(BoundExpression receiver, MethodSymbol method, params BoundExpression[] args)
		{
			return Call(receiver, method, ImmutableArray.Create(args));
		}

		public BoundCall Call(BoundExpression receiver, MethodSymbol method, ImmutableArray<BoundExpression> args)
		{
			BoundCall boundCall = new BoundCall(Syntax, method, null, receiver, args, null, method.ReturnType, suppressObjectClone: true);
			boundCall.SetWasCompilerGenerated();
			return boundCall;
		}

		public BoundStatement If(BoundExpression condition, BoundStatement thenClause, BoundStatement elseClause)
		{
			GeneratedLabelSymbol generatedLabelSymbol = new GeneratedLabelSymbol("afterif");
			GeneratedLabelSymbol generatedLabelSymbol2 = new GeneratedLabelSymbol("alternative");
			BoundConditionalGoto boundConditionalGoto = new BoundConditionalGoto(_syntax, condition, jumpIfTrue: false, generatedLabelSymbol2);
			boundConditionalGoto.SetWasCompilerGenerated();
			return Block(boundConditionalGoto, thenClause, Goto(generatedLabelSymbol), Label(generatedLabelSymbol2), elseClause, Label(generatedLabelSymbol));
		}

		public BoundTernaryConditionalExpression TernaryConditionalExpression(BoundExpression condition, BoundExpression ifTrue, BoundExpression ifFalse)
		{
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundTernaryConditionalExpression(Syntax, condition, ifTrue, ifFalse, null, ifTrue.Type));
		}

		public BoundTryCast TryCast(BoundExpression expression, TypeSymbol type)
		{
			SyntaxNode syntax = Syntax;
			TypeSymbol type2 = expression.Type;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			return new BoundTryCast(syntax, expression, Conversions.ClassifyTryCastConversion(type2, type, ref useSiteInfo), type);
		}

		public BoundDirectCast DirectCast(BoundExpression expression, TypeSymbol type)
		{
			SyntaxNode syntax = Syntax;
			int conversionKind;
			if (!BoundExpressionExtensions.IsNothingLiteral(expression))
			{
				TypeSymbol type2 = expression.Type;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				conversionKind = (int)Conversions.ClassifyDirectCastConversion(type2, type, ref useSiteInfo);
			}
			else
			{
				conversionKind = 2049;
			}
			return new BoundDirectCast(syntax, expression, (ConversionKind)conversionKind, type);
		}

		public BoundStatement If(BoundExpression condition, BoundStatement thenClause)
		{
			return If(condition, thenClause, Block());
		}

		public BoundThrowStatement Throw(BoundExpression e = null)
		{
			BoundThrowStatement boundThrowStatement = new BoundThrowStatement(_syntax, e);
			boundThrowStatement.SetWasCompilerGenerated();
			return boundThrowStatement;
		}

		public BoundLocal Local(LocalSymbol localSym, bool isLValue)
		{
			BoundLocal boundLocal = new BoundLocal(_syntax, localSym, isLValue, localSym.Type);
			boundLocal.SetWasCompilerGenerated();
			return boundLocal;
		}

		public BoundExpression Sequence(ImmutableArray<LocalSymbol> temps, params BoundExpression[] parts)
		{
			BoundExpression[] array = new BoundExpression[parts.Length - 1 - 1 + 1];
			int num = parts.Length - 1 - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = parts[i];
			}
			BoundExpression result = parts[^1];
			return Sequence(temps, array.AsImmutableOrNull(), result);
		}

		public BoundExpression Sequence(LocalSymbol temp, params BoundExpression[] parts)
		{
			return Sequence(ImmutableArray.Create(temp), parts);
		}

		public BoundExpression Sequence(params BoundExpression[] parts)
		{
			return Sequence(ImmutableArray<LocalSymbol>.Empty, parts);
		}

		public BoundExpression Sequence(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> sideEffects, BoundExpression result)
		{
			BoundSequence boundSequence = new BoundSequence(_syntax, locals, sideEffects, result, result.Type);
			boundSequence.SetWasCompilerGenerated();
			return boundSequence;
		}

		public BoundStatement Select(BoundExpression ex, IEnumerable<BoundCaseBlock> sections)
		{
			ImmutableArray<BoundCaseBlock> caseBlocks = ImmutableArray.CreateRange(sections);
			if (caseBlocks.Length == 0)
			{
				return ExpressionStatement(ex);
			}
			GeneratedLabelSymbol exitLabel = new GeneratedLabelSymbol("break");
			BoundSelectStatement boundSelectStatement = new BoundSelectStatement(_syntax, ExpressionStatement(ex), null, caseBlocks, recommendSwitchTable: true, exitLabel);
			boundSelectStatement.SetWasCompilerGenerated();
			return boundSelectStatement;
		}

		[Conditional("DEBUG")]
		private void CheckSwitchSections(ImmutableArray<BoundCaseBlock> sections)
		{
			HashSet<int> hashSet = new HashSet<int>();
			ImmutableArray<BoundCaseBlock>.Enumerator enumerator = sections.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ImmutableArray<BoundCaseClause>.Enumerator enumerator2 = enumerator.Current.CaseStatement.CaseClauses.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					int int32Value = ((BoundSimpleCaseClause)enumerator2.Current).ValueOpt.ConstantValueOpt.Int32Value;
					hashSet.Add(int32Value);
				}
			}
		}

		public BoundCaseBlock SwitchSection(List<int> values, params BoundStatement[] statements)
		{
			ArrayBuilder<BoundCaseClause> instance = ArrayBuilder<BoundCaseClause>.GetInstance();
			foreach (int value in values)
			{
				BoundSimpleCaseClause boundSimpleCaseClause = new BoundSimpleCaseClause(_syntax, Literal(value), null);
				boundSimpleCaseClause.SetWasCompilerGenerated();
				instance.Add(boundSimpleCaseClause);
			}
			BoundCaseStatement boundCaseStatement = new BoundCaseStatement(_syntax, instance.ToImmutableAndFree(), null);
			boundCaseStatement.SetWasCompilerGenerated();
			BoundCaseBlock boundCaseBlock = new BoundCaseBlock(_syntax, boundCaseStatement, Block(ImmutableArray.Create(statements)));
			boundCaseBlock.SetWasCompilerGenerated();
			return boundCaseBlock;
		}

		public BoundGotoStatement Goto(LabelSymbol label, bool setWasCompilerGenerated = true)
		{
			BoundGotoStatement boundGotoStatement = new BoundGotoStatement(_syntax, label, null);
			if (setWasCompilerGenerated)
			{
				boundGotoStatement.SetWasCompilerGenerated();
			}
			return boundGotoStatement;
		}

		public BoundLabelStatement Label(LabelSymbol labelSym)
		{
			BoundLabelStatement boundLabelStatement = new BoundLabelStatement(_syntax, labelSym);
			boundLabelStatement.SetWasCompilerGenerated();
			return boundLabelStatement;
		}

		public BoundLiteral Literal(string value)
		{
			BoundLiteral boundLiteral = new BoundLiteral(_syntax, ConstantValue.Create(value), SpecialType(Microsoft.CodeAnalysis.SpecialType.System_String));
			boundLiteral.SetWasCompilerGenerated();
			return boundLiteral;
		}

		public BoundLiteral StringLiteral(ConstantValue value)
		{
			BoundLiteral boundLiteral = new BoundLiteral(_syntax, value, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_String));
			boundLiteral.SetWasCompilerGenerated();
			return boundLiteral;
		}

		public BoundArrayAccess ArrayAccess(BoundExpression array, bool isLValue, params BoundExpression[] indices)
		{
			return ArrayAccess(array, isLValue, indices.AsImmutableOrNull());
		}

		public BoundArrayAccess ArrayAccess(BoundExpression array, bool isLValue, ImmutableArray<BoundExpression> indices)
		{
			BoundArrayAccess boundArrayAccess = new BoundArrayAccess(_syntax, array, indices, isLValue, ((ArrayTypeSymbol)array.Type).ElementType);
			boundArrayAccess.SetWasCompilerGenerated();
			return boundArrayAccess;
		}

		public BoundStatement BaseInitialization(params BoundExpression[] args)
		{
			MethodSymbol method = CurrentMethod.MeParameter.Type.BaseTypeNoUseSiteDiagnostics.InstanceConstructors.Single((MethodSymbol c) => c.ParameterCount == args.Length);
			BoundExpressionStatement boundExpressionStatement = new BoundExpressionStatement(_syntax, Call(Base(), method, args));
			boundExpressionStatement.SetWasCompilerGenerated();
			return boundExpressionStatement;
		}

		public static BoundStatement HiddenSequencePoint(BoundStatement statementOpt = null)
		{
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundSequencePoint(null, statementOpt));
		}

		public BoundExpression Null()
		{
			BoundLiteral boundLiteral = new BoundLiteral(_syntax, ConstantValue.Null, null);
			boundLiteral.SetWasCompilerGenerated();
			return boundLiteral;
		}

		public BoundExpression Null(TypeSymbol type)
		{
			if (!TypeSymbolExtensions.IsTypeParameter(type) && type.IsReferenceType)
			{
				BoundLiteral boundLiteral = new BoundLiteral(_syntax, ConstantValue.Null, type);
				boundLiteral.SetWasCompilerGenerated();
				return boundLiteral;
			}
			BoundExpression boundExpression = new BoundLiteral(_syntax, ConstantValue.Null, null);
			boundExpression.SetWasCompilerGenerated();
			return Convert(type, boundExpression);
		}

		public BoundTypeExpression Type(TypeSymbol typeSym)
		{
			BoundTypeExpression boundTypeExpression = new BoundTypeExpression(_syntax, typeSym);
			boundTypeExpression.SetWasCompilerGenerated();
			return boundTypeExpression;
		}

		public BoundExpression Typeof(WellKnownType type)
		{
			return Typeof(WellKnownType(type));
		}

		public BoundExpression Typeof(TypeSymbol typeSym)
		{
			BoundGetType boundGetType = new BoundGetType(_syntax, Type(typeSym), WellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Type));
			boundGetType.SetWasCompilerGenerated();
			return boundGetType;
		}

		public BoundTypeArguments TypeArguments(ImmutableArray<TypeSymbol> typeArgs)
		{
			BoundTypeArguments boundTypeArguments = new BoundTypeArguments(_syntax, typeArgs);
			boundTypeArguments.SetWasCompilerGenerated();
			return boundTypeArguments;
		}

		public BoundExpression MethodInfo(WellKnownMember meth)
		{
			MethodSymbol methodSymbol = WellKnownMember<MethodSymbol>(meth);
			if ((object)methodSymbol == null)
			{
				return BadExpression();
			}
			return MethodInfo(methodSymbol);
		}

		public BoundExpression MethodInfo(SpecialMember meth)
		{
			MethodSymbol methodSymbol = (MethodSymbol)SpecialMember(meth);
			if ((object)methodSymbol == null)
			{
				return BadExpression();
			}
			return MethodInfo(methodSymbol);
		}

		public BoundExpression MethodInfo(MethodSymbol method)
		{
			BoundMethodInfo boundMethodInfo = new BoundMethodInfo(Syntax, method, WellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Reflection_MethodInfo));
			WellKnownMember<MethodSymbol>(method.ContainingType.IsGenericType ? Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle2 : Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle);
			boundMethodInfo.SetWasCompilerGenerated();
			return boundMethodInfo;
		}

		public BoundExpression ConstructorInfo(WellKnownMember meth)
		{
			MethodSymbol methodSymbol = WellKnownMember<MethodSymbol>(meth);
			if ((object)methodSymbol == null)
			{
				return BadExpression();
			}
			return ConstructorInfo(methodSymbol);
		}

		public BoundExpression ConstructorInfo(SpecialMember meth)
		{
			MethodSymbol methodSymbol = (MethodSymbol)SpecialMember(meth);
			if ((object)methodSymbol == null)
			{
				return BadExpression();
			}
			return ConstructorInfo(methodSymbol);
		}

		public BoundExpression ConstructorInfo(MethodSymbol meth)
		{
			BoundMethodInfo boundMethodInfo = new BoundMethodInfo(Syntax, meth, WellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Reflection_ConstructorInfo));
			boundMethodInfo.SetWasCompilerGenerated();
			return boundMethodInfo;
		}

		public BoundExpression FieldInfo(FieldSymbol field)
		{
			BoundFieldInfo boundFieldInfo = new BoundFieldInfo(_syntax, field, WellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Reflection_FieldInfo));
			WellKnownMember<MethodSymbol>(field.ContainingType.IsGenericType ? Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle2 : Microsoft.CodeAnalysis.WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle);
			boundFieldInfo.SetWasCompilerGenerated();
			return boundFieldInfo;
		}

		public BoundExpression MethodDefIndex(MethodSymbol method)
		{
			BoundMethodDefIndex boundMethodDefIndex = new BoundMethodDefIndex(Syntax, method, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32));
			boundMethodDefIndex.SetWasCompilerGenerated();
			return boundMethodDefIndex;
		}

		public BoundExpression MaximumMethodDefIndex()
		{
			BoundMaximumMethodDefIndex boundMaximumMethodDefIndex = new BoundMaximumMethodDefIndex(Syntax, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32));
			boundMaximumMethodDefIndex.SetWasCompilerGenerated();
			return boundMaximumMethodDefIndex;
		}

		public BoundExpression ModuleVersionId(bool isLValue)
		{
			BoundModuleVersionId boundModuleVersionId = new BoundModuleVersionId(Syntax, isLValue, WellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Guid));
			boundModuleVersionId.SetWasCompilerGenerated();
			return boundModuleVersionId;
		}

		public BoundExpression ModuleVersionIdString()
		{
			BoundModuleVersionIdString boundModuleVersionIdString = new BoundModuleVersionIdString(Syntax, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_String));
			boundModuleVersionIdString.SetWasCompilerGenerated();
			return boundModuleVersionIdString;
		}

		public BoundExpression InstrumentationPayloadRoot(int analysisKind, TypeSymbol payloadType, bool isLValue)
		{
			BoundInstrumentationPayloadRoot boundInstrumentationPayloadRoot = new BoundInstrumentationPayloadRoot(Syntax, analysisKind, isLValue, payloadType);
			boundInstrumentationPayloadRoot.SetWasCompilerGenerated();
			return boundInstrumentationPayloadRoot;
		}

		public BoundExpression SourceDocumentIndex(DebugSourceDocument document)
		{
			BoundSourceDocumentIndex boundSourceDocumentIndex = new BoundSourceDocumentIndex(Syntax, document, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32));
			boundSourceDocumentIndex.SetWasCompilerGenerated();
			return boundSourceDocumentIndex;
		}

		public BoundConversion Convert(TypeSymbol type, BoundExpression arg, bool isChecked = false)
		{
			if (BoundExpressionExtensions.IsNothingLiteral(arg))
			{
				return Convert(type, arg, ConversionKind.WideningNothingLiteral, isChecked);
			}
			if (TypeSymbolExtensions.IsErrorType(type) || TypeSymbolExtensions.IsErrorType(arg.Type))
			{
				return Convert(type, arg, ConversionKind.WideningReference, isChecked);
			}
			TypeSymbol type2 = arg.Type;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			return Convert(type, arg, Conversions.ClassifyConversion(type2, type, ref useSiteInfo).Key, isChecked);
		}

		public BoundConversion Convert(TypeSymbol type, BoundExpression arg, ConversionKind convKind, bool isChecked = false)
		{
			BoundConversion boundConversion = new BoundConversion(_syntax, arg, convKind, isChecked, explicitCastInCode: true, null, type);
			boundConversion.SetWasCompilerGenerated();
			return boundConversion;
		}

		public BoundExpression Array(TypeSymbol elementType, params BoundExpression[] elements)
		{
			return Array(elementType, elements.AsImmutableOrNull());
		}

		public BoundExpression Array(TypeSymbol elementType, ImmutableArray<BoundExpression> elements)
		{
			ArrayTypeSymbol type = Compilation.CreateArrayTypeSymbol(elementType);
			BoundArrayInitialization boundArrayInitialization = new BoundArrayInitialization(_syntax, elements, type);
			boundArrayInitialization.SetWasCompilerGenerated();
			return new BoundArrayCreation(_syntax, ImmutableArray.Create((BoundExpression)Literal(elements.Length)), boundArrayInitialization, type);
		}

		public BoundExpression Array(TypeSymbol elementType, ImmutableArray<BoundExpression> bounds, ImmutableArray<BoundExpression> elements)
		{
			ArrayTypeSymbol type = Compilation.CreateArrayTypeSymbol(elementType);
			BoundArrayInitialization boundArrayInitialization = ((!elements.IsDefaultOrEmpty) ? new BoundArrayInitialization(_syntax, elements, type) : null);
			boundArrayInitialization?.SetWasCompilerGenerated();
			BoundArrayCreation boundArrayCreation = new BoundArrayCreation(_syntax, bounds, boundArrayInitialization, type);
			boundArrayCreation.SetWasCompilerGenerated();
			return boundArrayCreation;
		}

		public BoundTernaryConditionalExpression Conditional(BoundExpression condition, BoundExpression consequence, BoundExpression alternative, TypeSymbol type)
		{
			return new BoundTernaryConditionalExpression(Syntax, condition, consequence, alternative, null, type);
		}

		public BoundBinaryConditionalExpression BinaryConditional(BoundExpression left, BoundExpression right)
		{
			return new BoundBinaryConditionalExpression(Syntax, left, null, null, right, null, left.Type);
		}

		public BoundBinaryOperator Binary(BinaryOperatorKind kind, TypeSymbol type, BoundExpression left, BoundExpression right)
		{
			BoundBinaryOperator boundBinaryOperator = new BoundBinaryOperator(Syntax, kind, left, right, @checked: false, type);
			boundBinaryOperator.SetWasCompilerGenerated();
			return boundBinaryOperator;
		}

		public BoundBinaryOperator ObjectReferenceEqual(BoundExpression left, BoundExpression right)
		{
			BoundBinaryOperator boundBinaryOperator = Binary(BinaryOperatorKind.Is, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), left, right);
			boundBinaryOperator.SetWasCompilerGenerated();
			return boundBinaryOperator;
		}

		public BoundBinaryOperator ReferenceIsNothing(BoundExpression operand)
		{
			BoundBinaryOperator boundBinaryOperator = Binary(BinaryOperatorKind.Is, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), operand, Null(operand.Type));
			boundBinaryOperator.SetWasCompilerGenerated();
			return boundBinaryOperator;
		}

		public BoundBinaryOperator ReferenceIsNotNothing(BoundExpression operand)
		{
			BoundBinaryOperator boundBinaryOperator = Binary(BinaryOperatorKind.IsNot, SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), operand, Null(operand.Type));
			boundBinaryOperator.SetWasCompilerGenerated();
			return boundBinaryOperator;
		}

		public BoundExpression Not(BoundExpression expression)
		{
			return new BoundUnaryOperator(expression.Syntax, UnaryOperatorKind.Not, expression, @checked: false, expression.Type);
		}

		public BoundStatement Try(BoundBlock tryBlock, ImmutableArray<BoundCatchBlock> catchBlocks, BoundBlock finallyBlock = null, LabelSymbol exitLabel = null)
		{
			return new BoundTryStatement(Syntax, tryBlock, catchBlocks, finallyBlock, exitLabel);
		}

		public ImmutableArray<BoundCatchBlock> CatchBlocks(params BoundCatchBlock[] blocks)
		{
			return blocks.AsImmutableOrNull();
		}

		public BoundCatchBlock Catch(LocalSymbol local, BoundBlock block, bool isSynthesizedAsyncCatchAll = false)
		{
			MethodSymbol methodSymbol = WellKnownMember<MethodSymbol>(Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError);
			MethodSymbol methodSymbol2 = WellKnownMember<MethodSymbol>(Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError);
			return new BoundCatchBlock(Syntax, local, Local(local, isLValue: false), null, null, block, isSynthesizedAsyncCatchAll, (object)methodSymbol == null || (object)methodSymbol2 == null);
		}

		public BoundStatement SequencePoint(SyntaxNode syntax, BoundStatement statement)
		{
			return new BoundSequencePoint(syntax, statement);
		}

		public BoundStatement SequencePoint(SyntaxNode syntax)
		{
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundSequencePoint(syntax, null));
		}

		public BoundStatement SequencePointWithSpan(SyntaxNode syntax, TextSpan textSpan, BoundStatement boundStatement)
		{
			return new BoundSequencePointWithSpan(syntax, boundStatement, textSpan);
		}

		public BoundStatement NoOp(NoOpStatementFlavor flavor = NoOpStatementFlavor.Default)
		{
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundNoOpStatement(Syntax, flavor));
		}

		public void CloseMethod(BoundStatement body)
		{
			if (body.Kind != BoundKind.Block)
			{
				body = Block(body);
			}
			CompilationState.AddSynthesizedMethod(CurrentMethod, body);
			CurrentMethod = null;
		}

		public BoundSpillSequence SpillSequence(ImmutableArray<LocalSymbol> locals, ImmutableArray<FieldSymbol> fields, ImmutableArray<BoundStatement> statements, BoundExpression valueOpt)
		{
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundSpillSequence(Syntax, locals, fields, statements, valueOpt, (valueOpt == null) ? SpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void) : valueOpt.Type));
		}
	}
}

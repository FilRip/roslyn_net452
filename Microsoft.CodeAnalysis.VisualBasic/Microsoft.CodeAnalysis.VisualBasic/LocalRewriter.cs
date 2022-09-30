using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class LocalRewriter : BoundTreeRewriterWithStackGuard
	{
		[Flags]
		internal enum RewritingFlags : byte
		{
			Default = 0,
			AllowSequencePoints = 1,
			AllowEndOfMethodReturnWithExpression = 2,
			AllowCatchWithErrorLineNumberReference = 4,
			AllowOmissionOfConditionalCalls = 8
		}

		private sealed class LocalVariableSubstituter : BoundTreeRewriterWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
		{
			private readonly LocalSymbol _original;

			private readonly LocalSymbol _replacement;

			private bool _replacedNode;

			private bool ReplacedNode => _replacedNode;

			public static BoundNode Replace(BoundNode node, LocalSymbol original, LocalSymbol replacement, int recursionDepth, ref bool replacedNode)
			{
				LocalVariableSubstituter localVariableSubstituter = new LocalVariableSubstituter(original, replacement, recursionDepth);
				BoundNode result = localVariableSubstituter.Visit(node);
				replacedNode = localVariableSubstituter.ReplacedNode;
				return result;
			}

			private LocalVariableSubstituter(LocalSymbol original, LocalSymbol replacement, int recursionDepth)
				: base(recursionDepth)
			{
				_replacedNode = false;
				_original = original;
				_replacement = replacement;
			}

			public override BoundNode VisitLocal(BoundLocal node)
			{
				if ((object)node.LocalSymbol == _original)
				{
					_replacedNode = true;
					return node.Update(_replacement, node.IsLValue, node.Type);
				}
				return node;
			}
		}

		private struct UnstructuredExceptionHandlingState
		{
			public BoundUnstructuredExceptionHandlingStatement Context;

			public ArrayBuilder<BoundGotoStatement> ExceptionHandlers;

			public ArrayBuilder<BoundGotoStatement> ResumeTargets;

			public int OnErrorResumeNextCount;

			public LocalSymbol ActiveHandlerTemporary;

			public LocalSymbol ResumeTargetTemporary;

			public LocalSymbol CurrentStatementTemporary;

			public LabelSymbol ResumeNextLabel;

			public LabelSymbol ResumeLabel;
		}

		private struct UnstructuredExceptionHandlingContext
		{
			public BoundUnstructuredExceptionHandlingStatement Context;
		}

		private struct XmlLiteralFixupData
		{
			public struct LocalWithInitialization
			{
				public readonly LocalSymbol Local;

				public readonly BoundExpression Initialization;

				public LocalWithInitialization(LocalSymbol local, BoundExpression initialization)
				{
					this = default(LocalWithInitialization);
					Local = local;
					Initialization = initialization;
				}
			}

			private ArrayBuilder<LocalWithInitialization> _locals;

			public bool IsEmpty => _locals == null;

			public void AddLocal(LocalSymbol local, BoundExpression initialization)
			{
				if (_locals == null)
				{
					_locals = ArrayBuilder<LocalWithInitialization>.GetInstance();
				}
				_locals.Add(new LocalWithInitialization(local, initialization));
			}

			public ImmutableArray<LocalWithInitialization> MaterializeAndFree()
			{
				ImmutableArray<LocalWithInitialization> result = _locals.ToImmutableAndFree();
				_locals = null;
				return result;
			}
		}

		private readonly MethodSymbol _topMethod;

		private readonly PEModuleBuilder _emitModule;

		private readonly TypeCompilationState _compilationState;

		private readonly SynthesizedSubmissionFields _previousSubmissionFields;

		private readonly BindingDiagnosticBag _diagnostics;

		private readonly Instrumenter _instrumenterOpt;

		private ISet<Symbol> _symbolsCapturedWithoutCopyCtor;

		private MethodSymbol _currentMethodOrLambda;

		private Dictionary<RangeVariableSymbol, BoundExpression> _rangeVariableMap;

		private Dictionary<BoundValuePlaceholderBase, BoundExpression> _placeholderReplacementMapDoNotUseDirectly;

		private bool _hasLambdas;

		private bool _inExpressionLambda;

		private Dictionary<LocalSymbol, KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField>> _staticLocalMap;

		private XmlLiteralFixupData _xmlFixupData;

		private ImmutableArray<KeyValuePair<string, string>> _xmlImportedNamespaces;

		private UnstructuredExceptionHandlingState _unstructuredExceptionHandling;

		private LocalSymbol _currentLineTemporary;

		private bool _instrumentTopLevelNonCompilerGeneratedExpressionsInQuery;

		private int _conditionalAccessReceiverPlaceholderId;

		private readonly RewritingFlags _flags;

		private const int s_activeHandler_None = 0;

		private const int s_activeHandler_ResumeNext = 1;

		private const int s_activeHandler_FirstNonReservedOnErrorGotoIndex = 2;

		private const int s_activeHandler_FirstOnErrorResumeNextIndex = -2;

		private readonly Dictionary<TypeSymbol, bool> _valueTypesCleanUpCache;

		private BoundExpression PlaceholderReplacement => _placeholderReplacementMapDoNotUseDirectly[placeholder];

		public bool OptimizationLevelIsDebug => Compilation.Options.OptimizationLevel == OptimizationLevel.Debug;

		private VisualBasicCompilation Compilation => _topMethod.DeclaringCompilation;

		private SourceAssemblySymbol ContainingAssembly => (SourceAssemblySymbol)_topMethod.ContainingAssembly;

		private bool Instrument
		{
			get
			{
				if (_instrumenterOpt != null)
				{
					return !_inExpressionLambda;
				}
				return false;
			}
		}

		private bool Instrument
		{
			get
			{
				if (this.Instrument && rewritten != null && !original.WasCompilerGenerated)
				{
					return original.Syntax != null;
				}
				return false;
			}
		}

		private bool Instrument
		{
			get
			{
				if (this.Instrument && !original.WasCompilerGenerated)
				{
					return original.Syntax != null;
				}
				return false;
			}
		}

		[Conditional("DEBUG")]
		private static void AssertPlaceholderReplacement(BoundValuePlaceholderBase placeholder, BoundExpression value)
		{
			if (placeholder.IsLValue)
			{
				_ = value.Kind;
				_ = 108;
			}
		}

		private void AddPlaceholderReplacement(BoundValuePlaceholderBase placeholder, BoundExpression value)
		{
			if (_placeholderReplacementMapDoNotUseDirectly == null)
			{
				_placeholderReplacementMapDoNotUseDirectly = new Dictionary<BoundValuePlaceholderBase, BoundExpression>();
			}
			_placeholderReplacementMapDoNotUseDirectly.Add(placeholder, value);
		}

		private void UpdatePlaceholderReplacement(BoundValuePlaceholderBase placeholder, BoundExpression value)
		{
			_placeholderReplacementMapDoNotUseDirectly[placeholder] = value;
		}

		private void RemovePlaceholderReplacement(BoundValuePlaceholderBase placeholder)
		{
			_placeholderReplacementMapDoNotUseDirectly.Remove(placeholder);
		}

		private LocalRewriter(MethodSymbol topMethod, MethodSymbol currentMethod, TypeCompilationState compilationState, SynthesizedSubmissionFields previousSubmissionFields, BindingDiagnosticBag diagnostics, RewritingFlags flags, Instrumenter instrumenterOpt, int recursionDepth)
			: base(recursionDepth)
		{
			_xmlFixupData = default(XmlLiteralFixupData);
			_valueTypesCleanUpCache = new Dictionary<TypeSymbol, bool>();
			_topMethod = topMethod;
			_currentMethodOrLambda = currentMethod;
			_emitModule = compilationState.ModuleBuilderOpt;
			_compilationState = compilationState;
			_previousSubmissionFields = previousSubmissionFields;
			_diagnostics = diagnostics;
			_flags = flags;
			_instrumenterOpt = instrumenterOpt;
		}

		private static BoundNode RewriteNode(BoundNode node, MethodSymbol topMethod, MethodSymbol currentMethod, TypeCompilationState compilationState, SynthesizedSubmissionFields previousSubmissionFields, BindingDiagnosticBag diagnostics, [In][Out] ref HashSet<BoundNode> rewrittenNodes, out bool hasLambdas, out ISet<Symbol> symbolsCapturedWithoutCtor, RewritingFlags flags, Instrumenter instrumenterOpt, int recursionDepth)
		{
			LocalRewriter localRewriter = new LocalRewriter(topMethod, currentMethod, compilationState, previousSubmissionFields, diagnostics, flags, instrumenterOpt, recursionDepth);
			BoundNode boundNode = localRewriter.Visit(node);
			if (!localRewriter._xmlFixupData.IsEmpty)
			{
				boundNode = InsertXmlLiteralsPreamble(boundNode, localRewriter._xmlFixupData.MaterializeAndFree());
			}
			hasLambdas = localRewriter._hasLambdas;
			symbolsCapturedWithoutCtor = localRewriter._symbolsCapturedWithoutCopyCtor;
			return boundNode;
		}

		private static BoundBlock InsertXmlLiteralsPreamble(BoundNode node, ImmutableArray<XmlLiteralFixupData.LocalWithInitialization> fixups)
		{
			int length = fixups.Length;
			LocalSymbol[] array = new LocalSymbol[length - 1 + 1];
			BoundStatement[] array2 = new BoundStatement[length + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				XmlLiteralFixupData.LocalWithInitialization localWithInitialization = fixups[i];
				array[i] = localWithInitialization.Local;
				BoundExpression initialization = localWithInitialization.Initialization;
				array2[i] = new BoundExpressionStatement(initialization.Syntax, initialization);
			}
			array2[length] = (BoundStatement)node;
			return new BoundBlock(node.Syntax, default(SyntaxList<StatementSyntax>), array.AsImmutable(), array2.AsImmutableOrNull());
		}

		public static BoundBlock Rewrite(BoundBlock node, MethodSymbol topMethod, TypeCompilationState compilationState, SynthesizedSubmissionFields previousSubmissionFields, BindingDiagnosticBag diagnostics, out HashSet<BoundNode> rewrittenNodes, out bool hasLambdas, out ISet<Symbol> symbolsCapturedWithoutCopyCtor, RewritingFlags flags, Instrumenter instrumenterOpt, MethodSymbol currentMethod)
		{
			return (BoundBlock)RewriteNode(node, topMethod, currentMethod ?? topMethod, compilationState, previousSubmissionFields, diagnostics, ref rewrittenNodes, out hasLambdas, out symbolsCapturedWithoutCopyCtor, flags, instrumenterOpt, 0);
		}

		public static BoundExpression RewriteExpressionTree(BoundExpression node, MethodSymbol method, TypeCompilationState compilationState, SynthesizedSubmissionFields previousSubmissionFields, BindingDiagnosticBag diagnostics, HashSet<BoundNode> rewrittenNodes, int recursionDepth)
		{
			bool hasLambdas = false;
			ISet<Symbol> symbolsCapturedWithoutCtor = SpecializedCollections.EmptySet<Symbol>();
			return (BoundExpression)RewriteNode(node, method, method, compilationState, previousSubmissionFields, diagnostics, ref rewrittenNodes, out hasLambdas, out symbolsCapturedWithoutCtor, RewritingFlags.Default, null, recursionDepth);
		}

		public override BoundNode Visit(BoundNode node)
		{
			if (node is BoundExpression node2)
			{
				return VisitExpression(node2);
			}
			return base.Visit(node);
		}

		private BoundExpression VisitExpression(BoundExpression node)
		{
			ConstantValue constantValueOpt = node.ConstantValueOpt;
			int num;
			if (_instrumentTopLevelNonCompilerGeneratedExpressionsInQuery && this.Instrument && !node.WasCompilerGenerated && VisualBasicExtensions.Kind(node.Syntax) != SyntaxKind.GroupAggregation)
			{
				if (VisualBasicExtensions.Kind(node.Syntax) != SyntaxKind.SimpleAsClause || VisualBasicExtensions.Kind(node.Syntax.Parent) != SyntaxKind.CollectionRangeVariable)
				{
					num = ((node.Syntax is ExpressionSyntax) ? 1 : 0);
					if (num == 0)
					{
						goto IL_0075;
					}
				}
				else
				{
					num = 1;
				}
				_instrumentTopLevelNonCompilerGeneratedExpressionsInQuery = false;
			}
			else
			{
				num = 0;
			}
			goto IL_0075;
			IL_0075:
			BoundExpression boundExpression = (((object)constantValueOpt == null) ? VisitExpressionWithStackGuard(node) : RewriteConstant(node, constantValueOpt));
			if (num != 0)
			{
				_instrumentTopLevelNonCompilerGeneratedExpressionsInQuery = true;
				boundExpression = _instrumenterOpt.InstrumentTopLevelExpressionInQuery(node, boundExpression);
			}
			return boundExpression;
		}

		private static BoundStatement Concat(BoundStatement statement, BoundStatement additionOpt)
		{
			if (additionOpt == null)
			{
				return statement;
			}
			if (!(statement is BoundBlock boundBlock))
			{
				return new BoundStatementList(statements: new BoundStatement[2] { statement, additionOpt }.AsImmutableOrNull(), syntax: statement.Syntax);
			}
			BoundStatement[] array = new BoundStatement[boundBlock.Statements.Length + 1];
			int num = boundBlock.Statements.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = boundBlock.Statements[i];
			}
			array[boundBlock.Statements.Length] = additionOpt;
			return boundBlock.Update(boundBlock.StatementListSyntax, boundBlock.Locals, array.AsImmutableOrNull());
		}

		private static BoundBlock AppendToBlock(BoundBlock block, BoundStatement additionOpt)
		{
			if (additionOpt == null)
			{
				return block;
			}
			BoundStatement[] array = new BoundStatement[block.Statements.Length + 1];
			int num = block.Statements.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = block.Statements[i];
			}
			array[block.Statements.Length] = additionOpt;
			return block.Update(block.StatementListSyntax, block.Locals, array.AsImmutableOrNull());
		}

		private static BoundStatement PrependWithPrologue(BoundStatement statement, BoundStatement prologueOpt)
		{
			if (prologueOpt == null)
			{
				return statement;
			}
			return new BoundStatementList(statement.Syntax, ImmutableArray.Create(prologueOpt, statement));
		}

		private static BoundBlock PrependWithPrologue(BoundBlock block, BoundStatement prologueOpt)
		{
			if (prologueOpt == null)
			{
				return block;
			}
			return new BoundBlock(block.Syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(prologueOpt, block));
		}

		public override BoundNode VisitSequencePointWithSpan(BoundSequencePointWithSpan node)
		{
			return node.Update((BoundStatement)Visit(node.StatementOpt), node.Span);
		}

		public override BoundNode VisitSequencePoint(BoundSequencePoint node)
		{
			return node.Update((BoundStatement)Visit(node.StatementOpt));
		}

		public override BoundNode VisitBadExpression(BoundBadExpression node)
		{
			throw ExceptionUtilities.Unreachable;
		}

		private BoundExpression RewriteReceiverArgumentsAndGenerateAccessorCall(SyntaxNode syntax, MethodSymbol methodSymbol, BoundExpression receiverOpt, ImmutableArray<BoundExpression> arguments, ConstantValue constantValueOpt, bool isLValue, bool suppressObjectClone, TypeSymbol type)
		{
			UpdateMethodAndArgumentsIfReducedFromMethod(ref methodSymbol, ref receiverOpt, ref arguments);
			ImmutableArray<SynthesizedLocal> temporaries = default(ImmutableArray<SynthesizedLocal>);
			ImmutableArray<BoundExpression> copyBack = default(ImmutableArray<BoundExpression>);
			receiverOpt = VisitExpressionNode(receiverOpt);
			arguments = RewriteCallArguments(arguments, methodSymbol.Parameters, out temporaries, out copyBack, suppressObjectClone: false);
			BoundExpression boundExpression = new BoundCall(syntax, methodSymbol, null, receiverOpt, arguments, constantValueOpt, isLValue, suppressObjectClone, type);
			if (!temporaries.IsDefault)
			{
				boundExpression = ((!methodSymbol.IsSub) ? new BoundSequence(syntax, StaticCast<LocalSymbol>.From(temporaries), ImmutableArray<BoundExpression>.Empty, boundExpression, boundExpression.Type) : new BoundSequence(syntax, StaticCast<LocalSymbol>.From(temporaries), ImmutableArray.Create(boundExpression), null, boundExpression.Type));
			}
			return boundExpression;
		}

		private static LabelSymbol GenerateLabel(string baseName)
		{
			return new GeneratedLabelSymbol(baseName);
		}

		public override BoundNode VisitRValuePlaceholder(BoundRValuePlaceholder node)
		{
			return this.get_PlaceholderReplacement((BoundValuePlaceholderBase)node);
		}

		public override BoundNode VisitLValuePlaceholder(BoundLValuePlaceholder node)
		{
			return this.get_PlaceholderReplacement((BoundValuePlaceholderBase)node);
		}

		public override BoundNode VisitCompoundAssignmentTargetPlaceholder(BoundCompoundAssignmentTargetPlaceholder node)
		{
			return this.get_PlaceholderReplacement((BoundValuePlaceholderBase)node);
		}

		public override BoundNode VisitByRefArgumentPlaceholder(BoundByRefArgumentPlaceholder node)
		{
			return this.get_PlaceholderReplacement((BoundValuePlaceholderBase)node);
		}

		public override BoundNode VisitLValueToRValueWrapper(BoundLValueToRValueWrapper node)
		{
			return VisitExpressionNode(node.UnderlyingLValue).MakeRValue();
		}

		private NamedTypeSymbol GetSpecialType(SpecialType specialType)
		{
			return _topMethod.ContainingAssembly.GetSpecialType(specialType);
		}

		private NamedTypeSymbol GetSpecialTypeWithUseSiteDiagnostics(SpecialType specialType, SyntaxNode syntax)
		{
			NamedTypeSymbol specialType2 = _topMethod.ContainingAssembly.GetSpecialType(specialType);
			Binder.ReportUseSite(useSiteInfo: Binder.GetUseSiteInfoForSpecialType(specialType2), diagBag: _diagnostics, syntax: syntax);
			return specialType2;
		}

		private Symbol GetSpecialTypeMember(SpecialMember specialMember)
		{
			return _topMethod.ContainingAssembly.GetSpecialTypeMember(specialMember);
		}

		private bool ReportMissingOrBadRuntimeHelper(BoundNode node, SpecialMember specialMember, Symbol memberSymbol)
		{
			return ReportMissingOrBadRuntimeHelper(node, specialMember, memberSymbol, _diagnostics, _compilationState.Compilation.Options.EmbedVbCoreRuntime);
		}

		internal static bool ReportMissingOrBadRuntimeHelper(BoundNode node, SpecialMember specialMember, Symbol memberSymbol, BindingDiagnosticBag diagnostics, bool embedVBCoreRuntime = false)
		{
			if ((object)memberSymbol == null)
			{
				ReportMissingRuntimeHelper(node, specialMember, diagnostics, embedVBCoreRuntime);
				return true;
			}
			return ReportUseSite(node, Binder.GetUseSiteInfoForMemberAndContainingType(memberSymbol), diagnostics);
		}

		private static void ReportMissingRuntimeHelper(BoundNode node, SpecialMember specialMember, BindingDiagnosticBag diagnostics, bool embedVBCoreRuntime = false)
		{
			MemberDescriptor descriptor = SpecialMembers.GetDescriptor(specialMember);
			string declaringTypeMetadataName = descriptor.DeclaringTypeMetadataName;
			string name = descriptor.Name;
			ReportMissingRuntimeHelper(node, declaringTypeMetadataName, name, diagnostics, embedVBCoreRuntime);
		}

		private bool ReportMissingOrBadRuntimeHelper(BoundNode node, WellKnownMember wellKnownMember, Symbol memberSymbol)
		{
			return ReportMissingOrBadRuntimeHelper(node, wellKnownMember, memberSymbol, _diagnostics, _compilationState.Compilation.Options.EmbedVbCoreRuntime);
		}

		internal static bool ReportMissingOrBadRuntimeHelper(BoundNode node, WellKnownMember wellKnownMember, Symbol memberSymbol, BindingDiagnosticBag diagnostics, bool embedVBCoreRuntime)
		{
			if ((object)memberSymbol == null)
			{
				ReportMissingRuntimeHelper(node, wellKnownMember, diagnostics, embedVBCoreRuntime);
				return true;
			}
			return ReportUseSite(node, Binder.GetUseSiteInfoForMemberAndContainingType(memberSymbol), diagnostics);
		}

		private static void ReportMissingRuntimeHelper(BoundNode node, WellKnownMember wellKnownMember, BindingDiagnosticBag diagnostics, bool embedVBCoreRuntime)
		{
			MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(wellKnownMember);
			string declaringTypeMetadataName = descriptor.DeclaringTypeMetadataName;
			string name = descriptor.Name;
			ReportMissingRuntimeHelper(node, declaringTypeMetadataName, name, diagnostics, embedVBCoreRuntime);
		}

		private static void ReportMissingRuntimeHelper(BoundNode node, string typeName, string memberName, BindingDiagnosticBag diagnostics, bool embedVBCoreRuntime)
		{
			if (memberName.Equals(".ctor") || memberName.Equals(".cctor"))
			{
				memberName = "New";
			}
			DiagnosticInfo diagnosticForMissingRuntimeHelper = MissingRuntimeMemberDiagnosticHelper.GetDiagnosticForMissingRuntimeHelper(typeName, memberName, embedVBCoreRuntime);
			ReportDiagnostic(node, diagnosticForMissingRuntimeHelper, diagnostics);
		}

		private static void ReportDiagnostic(BoundNode node, DiagnosticInfo diagnostic, BindingDiagnosticBag diagnostics)
		{
			diagnostics.Add(new VBDiagnostic(diagnostic, node.Syntax.GetLocation()));
		}

		private static bool ReportUseSite(BoundNode node, UseSiteInfo<AssemblySymbol> useSiteInfo, BindingDiagnosticBag diagnostics)
		{
			return diagnostics.Add(useSiteInfo, node.Syntax.GetLocation());
		}

		private void ReportBadType(BoundNode node, TypeSymbol typeSymbol)
		{
			ReportUseSite(node, typeSymbol.GetUseSiteInfo(), _diagnostics);
		}

		public override BoundNode VisitMethodGroup(BoundMethodGroup node)
		{
			return null;
		}

		public override BoundNode VisitParenthesized(BoundParenthesized node)
		{
			BoundExpression boundExpression = VisitExpressionNode(node.Expression);
			if (boundExpression.IsLValue)
			{
				boundExpression = boundExpression.MakeRValue();
			}
			return boundExpression;
		}

		private static BoundExpression CacheToLocalIfNotConst(Symbol container, BoundExpression value, ArrayBuilder<LocalSymbol> locals, ArrayBuilder<BoundExpression> expressions, SynthesizedLocalKind kind, StatementSyntax syntaxOpt)
		{
			ConstantValue constantValueOpt = value.ConstantValueOpt;
			if ((object)constantValueOpt != null)
			{
				if (!TypeSymbolExtensions.IsDecimalType(value.Type))
				{
					return value;
				}
				decimal decimalValue = constantValueOpt.DecimalValue;
				if (decimal.Compare(decimalValue, -1m) == 0 || decimal.Compare(decimalValue, 0m) == 0 || decimal.Compare(decimalValue, 1m) == 0)
				{
					return value;
				}
			}
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(container, value.Type, kind, syntaxOpt);
			locals.Add(synthesizedLocal);
			BoundLocal boundLocal = new BoundLocal(value.Syntax, synthesizedLocal, synthesizedLocal.Type);
			BoundAssignmentOperator item = BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(value.Syntax, boundLocal, value, suppressObjectClone: true, boundLocal.Type));
			expressions.Add(item);
			return boundLocal.MakeRValue();
		}

		internal static BoundExpression GenerateSequenceValueSideEffects(Symbol container, BoundExpression value, ImmutableArray<LocalSymbol> temporaries, ImmutableArray<BoundExpression> sideEffects)
		{
			SyntaxNode syntax = value.Syntax;
			TypeSymbol type = value.Type;
			ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
			if (!temporaries.IsEmpty)
			{
				instance.AddRange(temporaries);
			}
			ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
			BoundExpression valueOpt;
			if (type.SpecialType == SpecialType.System_Void)
			{
				instance2.Add(value);
				valueOpt = null;
			}
			else
			{
				valueOpt = CacheToLocalIfNotConst(container, value, instance, instance2, SynthesizedLocalKind.LoweringTemp, null);
			}
			if (!sideEffects.IsDefaultOrEmpty)
			{
				instance2.AddRange(sideEffects);
			}
			return new BoundSequence(syntax, instance.ToImmutableAndFree(), instance2.ToImmutableAndFree(), valueOpt, type);
		}

		private BoundExpression VisitExpressionNode(BoundExpression expression)
		{
			return (BoundExpression)Visit(expression);
		}

		public override BoundNode VisitAwaitOperator(BoundAwaitOperator node)
		{
			if (!_inExpressionLambda)
			{
				BoundLValuePlaceholder awaiterInstancePlaceholder = node.AwaiterInstancePlaceholder;
				BoundRValuePlaceholder awaitableInstancePlaceholder = node.AwaitableInstancePlaceholder;
				AddPlaceholderReplacement(awaiterInstancePlaceholder, awaiterInstancePlaceholder);
				AddPlaceholderReplacement(awaitableInstancePlaceholder, awaitableInstancePlaceholder);
				BoundNode result = base.VisitAwaitOperator(node);
				RemovePlaceholderReplacement(awaiterInstancePlaceholder);
				RemovePlaceholderReplacement(awaitableInstancePlaceholder);
				return result;
			}
			return node;
		}

		public override BoundNode VisitStopStatement(BoundStopStatement node)
		{
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			MethodSymbol methodSymbol = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.System_Diagnostics_Debugger__Break);
			BoundStatement boundStatement = node;
			if ((object)methodSymbol != null)
			{
				boundStatement = BoundExpressionExtensions.ToStatement(new BoundCall(syntheticBoundNodeFactory.Syntax, methodSymbol, null, null, ImmutableArray<BoundExpression>.Empty, null, isLValue: false, suppressObjectClone: true, methodSymbol.ReturnType));
			}
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, canThrow: true);
			}
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentStopStatement(node, boundStatement);
			}
			return boundStatement;
		}

		public override BoundNode VisitEndStatement(BoundEndStatement node)
		{
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			MethodSymbol methodSymbol = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__EndApp);
			BoundStatement boundStatement = node;
			if ((object)methodSymbol != null)
			{
				boundStatement = BoundExpressionExtensions.ToStatement(syntheticBoundNodeFactory.Call(null, methodSymbol, ImmutableArray<BoundExpression>.Empty));
			}
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentEndStatement(node, boundStatement);
			}
			return boundStatement;
		}

		public override BoundNode VisitGetType(BoundGetType node)
		{
			BoundGetType boundGetType = (BoundGetType)base.VisitGetType(node);
			MethodSymbol result = null;
			if (!TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.System_Type__GetTypeFromHandle, node.Syntax))
			{
				return new BoundGetType(boundGetType.Syntax, boundGetType.SourceType, boundGetType.Type, hasErrors: true);
			}
			return boundGetType;
		}

		public override BoundNode VisitArrayCreation(BoundArrayCreation node)
		{
			return base.VisitArrayCreation(node.Update(node.IsParamArrayArgument, node.Bounds, node.InitializerOpt, null, ConversionKind.DelegateRelaxationLevelNone, node.Type));
		}

		public override BoundNode VisitAddHandlerStatement(BoundAddHandlerStatement node)
		{
			BoundStatement boundStatement = RewriteAddRemoveHandler(node);
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentAddHandlerStatement(node, boundStatement);
			}
			return boundStatement;
		}

		public override BoundNode VisitRemoveHandlerStatement(BoundRemoveHandlerStatement node)
		{
			BoundStatement boundStatement = RewriteAddRemoveHandler(node);
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentRemoveHandlerStatement(node, boundStatement);
			}
			return boundStatement;
		}

		private BoundStatement RewriteAddRemoveHandler(BoundAddRemoveHandlerStatement node)
		{
			BoundEventAccess boundEventAccess = UnwrapEventAccess(node.EventAccess);
			EventSymbol eventSymbol = boundEventAccess.EventSymbol;
			UnstructuredExceptionHandlingContext saved = LeaveUnstructuredExceptionHandlingContext(node);
			BoundStatement boundStatement = ((!eventSymbol.IsWindowsRuntimeEvent) ? MakeEventAccessorCall(node, boundEventAccess, (node.Kind == BoundKind.AddHandlerStatement) ? eventSymbol.AddMethod : eventSymbol.RemoveMethod) : RewriteWinRtEvent(node, boundEventAccess, node.Kind == BoundKind.AddHandlerStatement));
			RestoreUnstructuredExceptionHandlingContext(node, saved);
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, canThrow: true);
			}
			return boundStatement;
		}

		private BoundStatement RewriteWinRtEvent(BoundAddRemoveHandlerStatement node, BoundEventAccess unwrappedEventAccess, bool isAddition)
		{
			SyntaxNode syntax = node.Syntax;
			EventSymbol eventSymbol = unwrappedEventAccess.EventSymbol;
			BoundExpression eventAccessReceiver = GetEventAccessReceiver(unwrappedEventAccess);
			BoundExpression boundExpression = VisitExpressionNode(node.Handler);
			BoundAssignmentOperator expression = null;
			BoundLocal boundLocal = null;
			if (!eventSymbol.IsShared && EventReceiverNeedsTemp(eventAccessReceiver))
			{
				TypeSymbol type = eventAccessReceiver.Type;
				boundLocal = new BoundLocal(syntax, new SynthesizedLocal(_currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp), type);
				expression = new BoundAssignmentOperator(syntax, boundLocal, GenerateObjectCloneIfNeeded(unwrappedEventAccess.ReceiverOpt, eventAccessReceiver.MakeRValue()), suppressObjectClone: true);
			}
			NamedTypeSymbol wellKnownType = Compilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken);
			Compilation.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal);
			NamedTypeSymbol wellKnownType2 = Compilation.GetWellKnownType(WellKnownType.System_Action_T);
			TypeSymbol type2 = eventSymbol.Type;
			wellKnownType2 = wellKnownType2.Construct(wellKnownType);
			BoundExpression receiverOpt = (boundLocal ?? eventAccessReceiver ?? BoundNodeExtensions.MakeCompilerGenerated(new BoundTypeExpression(syntax, type2))).MakeRValue();
			BoundDelegateCreationExpression boundDelegateCreationExpression = new BoundDelegateCreationExpression(syntax, receiverOpt, eventSymbol.RemoveMethod, null, null, null, wellKnownType2);
			WellKnownMember memberId;
			ImmutableArray<BoundExpression> arguments;
			if (isAddition)
			{
				NamedTypeSymbol wellKnownType3 = Compilation.GetWellKnownType(WellKnownType.System_Func_T2);
				wellKnownType3 = wellKnownType3.Construct(type2, wellKnownType);
				BoundDelegateCreationExpression item = new BoundDelegateCreationExpression(syntax, receiverOpt, eventSymbol.AddMethod, null, null, null, wellKnownType3);
				memberId = WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__AddEventHandler_T;
				arguments = ImmutableArray.Create(item, boundDelegateCreationExpression, boundExpression);
			}
			else
			{
				memberId = WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveEventHandler_T;
				arguments = ImmutableArray.Create(boundDelegateCreationExpression, boundExpression);
			}
			MethodSymbol result = null;
			if (!TryGetWellknownMember<MethodSymbol>(out result, memberId, syntax))
			{
				return new BoundExpressionStatement(syntax, new BoundBadExpression(syntax, LookupResultKind.Empty, ImmutableArray.Create((Symbol)eventSymbol), ImmutableArray<BoundExpression>.Empty, ErrorTypeSymbol.UnknownResultType, hasErrors: true));
			}
			result = result.Construct(type2);
			BoundExpression expression2 = new BoundCall(syntax, result, null, null, arguments, null, result.ReturnType, suppressObjectClone: true);
			if (boundLocal == null)
			{
				return new BoundExpressionStatement(syntax, expression2);
			}
			return new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray.Create(boundLocal.LocalSymbol), ImmutableArray.Create((BoundStatement)new BoundExpressionStatement(syntax, expression), (BoundStatement)new BoundExpressionStatement(syntax, expression2)));
		}

		private static bool EventReceiverNeedsTemp(BoundExpression expression)
		{
			switch (expression.Kind)
			{
			case BoundKind.MeReference:
			case BoundKind.MyBaseReference:
			case BoundKind.MyClassReference:
				return false;
			case BoundKind.Literal:
				return (object)expression.Type != null && !expression.Type.SpecialType.IsClrInteger();
			case BoundKind.Local:
			case BoundKind.Parameter:
				return false;
			default:
				return true;
			}
		}

		public override BoundNode VisitEventAccess(BoundEventAccess node)
		{
			return base.VisitEventAccess(node);
		}

		private BoundStatement MakeEventAccessorCall(BoundAddRemoveHandlerStatement node, BoundEventAccess unwrappedEventAccess, MethodSymbol accessorSymbol)
		{
			BoundExpression eventAccessReceiver = GetEventAccessReceiver(unwrappedEventAccess);
			BoundExpression boundExpression = VisitExpressionNode(node.Handler);
			EventSymbol eventSymbol = unwrappedEventAccess.EventSymbol;
			BoundExpression boundExpression2 = null;
			if (eventAccessReceiver != null && eventSymbol.ContainingAssembly.IsLinked && TypeSymbolExtensions.IsInterfaceType(eventSymbol.ContainingType))
			{
				NamedTypeSymbol containingType = eventSymbol.ContainingType;
				ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = containingType.GetAttributes().GetEnumerator();
				while (enumerator.MoveNext())
				{
					VisualBasicAttributeData current = enumerator.Current;
					if (current.IsTargetAttribute(containingType, AttributeDescription.ComEventInterfaceAttribute) && current.CommonConstructorArguments.Length == 2)
					{
						boundExpression2 = RewriteNoPiaAddRemoveHandler(node, eventAccessReceiver, eventSymbol, boundExpression);
						break;
					}
				}
			}
			if (boundExpression2 == null)
			{
				boundExpression2 = new BoundCall(node.Syntax, accessorSymbol, null, eventAccessReceiver, ImmutableArray.Create(boundExpression), null, accessorSymbol.ReturnType);
			}
			return new BoundExpressionStatement(node.Syntax, boundExpression2);
		}

		private BoundEventAccess UnwrapEventAccess(BoundExpression node)
		{
			if (node.Kind == BoundKind.EventAccess)
			{
				return (BoundEventAccess)node;
			}
			return UnwrapEventAccess(((BoundParenthesized)node).Expression);
		}

		private BoundExpression GetEventAccessReceiver(BoundEventAccess unwrappedEventAccess)
		{
			if (unwrappedEventAccess.ReceiverOpt == null)
			{
				return null;
			}
			BoundExpression boundExpression = VisitExpressionNode(unwrappedEventAccess.ReceiverOpt);
			return unwrappedEventAccess.EventSymbol.IsShared ? null : boundExpression;
		}

		private BoundExpression RewriteNoPiaAddRemoveHandler(BoundAddRemoveHandlerStatement node, BoundExpression receiver, EventSymbol @event, BoundExpression handler)
		{
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			BoundExpression boundExpression = null;
			MethodSymbol methodSymbol = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__ctor);
			if ((object)methodSymbol != null)
			{
				MethodSymbol methodSymbol2 = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>((node.Kind == BoundKind.AddHandlerStatement) ? WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__AddEventHandler : WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__RemoveEventHandler);
				if ((object)methodSymbol2 != null)
				{
					BoundObjectCreationExpression receiver2 = syntheticBoundNodeFactory.New(methodSymbol, syntheticBoundNodeFactory.Typeof(@event.ContainingType), syntheticBoundNodeFactory.Literal(@event.MetadataName));
					boundExpression = syntheticBoundNodeFactory.Call(receiver2, methodSymbol2, Convert(syntheticBoundNodeFactory, methodSymbol2.Parameters[0].Type, receiver.MakeRValue()), Convert(syntheticBoundNodeFactory, methodSymbol2.Parameters[1].Type, handler));
				}
			}
			if (_emitModule != null)
			{
				_emitModule.EmbeddedTypesManagerOpt.EmbedEventIfNeedTo(@event.GetCciAdapter(), node.Syntax, _diagnostics.DiagnosticBag, isUsedForComAwareEventBinding: true);
			}
			if (boundExpression != null)
			{
				return boundExpression;
			}
			return new BoundBadExpression(node.Syntax, LookupResultKind.NotCreatable, ImmutableArray.Create((Symbol)@event), ImmutableArray.Create(receiver, handler), ErrorTypeSymbol.UnknownResultType, hasErrors: true);
		}

		private BoundExpression Convert(SyntheticBoundNodeFactory factory, TypeSymbol type, BoundExpression expr)
		{
			return TransformRewrittenConversion(factory.Convert(type, expr));
		}

		public override BoundNode VisitAnonymousTypeCreationExpression(BoundAnonymousTypeCreationExpression node)
		{
			int length = node.Arguments.Length;
			BoundExpression[] array = new BoundExpression[length - 1 + 1];
			ArrayBuilder<LocalSymbol> arrayBuilder = null;
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = node.Arguments[i];
				LocalSymbol localSymbol = ((node.BinderOpt != null) ? node.BinderOpt.GetAnonymousTypePropertyLocal(i) : null);
				if ((object)localSymbol != null)
				{
					if (arrayBuilder == null)
					{
						arrayBuilder = ArrayBuilder<LocalSymbol>.GetInstance();
					}
					arrayBuilder.Add(localSymbol);
					BoundLocal left = new BoundLocal(array[i].Syntax, localSymbol, isLValue: true, localSymbol.Type);
					array[i] = new BoundAssignmentOperator(array[i].Syntax, left, array[i], suppressObjectClone: true, localSymbol.Type);
				}
				array[i] = VisitExpression(array[i]);
			}
			BoundExpression boundExpression = new BoundObjectCreationExpression(node.Syntax, ((NamedTypeSymbol)node.Type).InstanceConstructors[0], array.AsImmutableOrNull(), null, node.Type);
			if (arrayBuilder != null)
			{
				boundExpression = new BoundSequence(node.Syntax, arrayBuilder.ToImmutableAndFree(), ImmutableArray<BoundExpression>.Empty, boundExpression, node.Type);
			}
			return boundExpression;
		}

		public override BoundNode VisitAnonymousTypePropertyAccess(BoundAnonymousTypePropertyAccess node)
		{
			LocalSymbol anonymousTypePropertyLocal = node.Binder.GetAnonymousTypePropertyLocal(node.PropertyIndex);
			return new BoundLocal(node.Syntax, anonymousTypePropertyLocal, isLValue: false, VisitType(anonymousTypePropertyLocal.Type));
		}

		public override BoundNode VisitAnonymousTypeFieldInitializer(BoundAnonymousTypeFieldInitializer node)
		{
			return Visit(node.Value);
		}

		public override BoundNode VisitAsNewLocalDeclarations(BoundAsNewLocalDeclarations node)
		{
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			BoundObjectInitializerExpression boundObjectInitializerFromInitializer = GetBoundObjectInitializerFromInitializer(node.Initializer);
			ImmutableArray<BoundLocalDeclaration> localDeclarations = node.LocalDeclarations;
			int num = localDeclarations.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				BoundLocalDeclaration boundLocalDeclaration = localDeclarations[i];
				BoundNode boundNode = null;
				LocalSymbol localSymbol = boundLocalDeclaration.LocalSymbol;
				KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField> staticLocalBackingFields = default(KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField>);
				if (localSymbol.IsStatic)
				{
					staticLocalBackingFields = CreateBackingFieldsForStaticLocal(localSymbol, hasInitializer: true);
				}
				if (boundObjectInitializerFromInitializer != null)
				{
					BoundExpression right = node.Initializer;
					BoundWithLValueExpressionPlaceholder boundWithLValueExpressionPlaceholder = boundObjectInitializerFromInitializer.PlaceholderOpt;
					if (i > 0)
					{
						AsNewClauseSyntax asNewClauseSyntax = (AsNewClauseSyntax)((VariableDeclaratorSyntax)node.Syntax).AsClause;
						ObjectCreationExpressionSyntax objectCreationExpressionSyntax = (ObjectCreationExpressionSyntax)asNewClauseSyntax.NewExpression;
						TypeSymbol type = localSymbol.Type;
						boundWithLValueExpressionPlaceholder = new BoundWithLValueExpressionPlaceholder(asNewClauseSyntax, type);
						boundWithLValueExpressionPlaceholder.SetWasCompilerGenerated();
						right = boundObjectInitializerFromInitializer.Binder.BindObjectCreationExpression(SyntaxExtensions.Type(asNewClauseSyntax), objectCreationExpressionSyntax.ArgumentList, type, objectCreationExpressionSyntax, BindingDiagnosticBag.Discarded, boundWithLValueExpressionPlaceholder);
					}
					if (!boundObjectInitializerFromInitializer.CreateTemporaryLocalForInitialization)
					{
						AddPlaceholderReplacement(boundWithLValueExpressionPlaceholder, VisitExpressionNode(new BoundLocal(boundLocalDeclaration.Syntax, localSymbol, localSymbol.Type)));
					}
					boundNode = VisitAndGenerateObjectCloneIfNeeded(right);
					if (!boundObjectInitializerFromInitializer.CreateTemporaryLocalForInitialization)
					{
						RemovePlaceholderReplacement(boundWithLValueExpressionPlaceholder);
					}
				}
				else
				{
					boundNode = VisitAndGenerateObjectCloneIfNeeded(node.Initializer);
				}
				BoundStatement item = RewriteLocalDeclarationAsInitializer(boundLocalDeclaration, (BoundExpression)boundNode, staticLocalBackingFields, boundObjectInitializerFromInitializer?.CreateTemporaryLocalForInitialization ?? true);
				instance.Add(item);
			}
			return new BoundStatementList(node.Syntax, instance.ToImmutableAndFree());
		}

		private static BoundObjectInitializerExpression GetBoundObjectInitializerFromInitializer(BoundExpression initializer)
		{
			if (initializer != null && (initializer.Kind == BoundKind.ObjectCreationExpression || initializer.Kind == BoundKind.NewT))
			{
				return ((BoundObjectCreationExpressionBase)initializer).InitializerOpt as BoundObjectInitializerExpression;
			}
			return null;
		}

		public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
		{
			BoundExpression left = node.Left;
			if (BoundExpressionExtensions.IsLateBound(left))
			{
				return RewriteLateBoundAssignment(node);
			}
			if (node.Right.Kind == BoundKind.MidResult && left.IsLValue)
			{
				return RewriteTrivialMidAssignment(node);
			}
			BoundExpression boundExpression = (IsPropertyAssignment(node) ? left : null);
			if (boundExpression == null && node.LeftOnTheRightOpt == null)
			{
				return VisitAssignmentOperatorSimple(node);
			}
			ImmutableArray<SynthesizedLocal> from = ImmutableArray<SynthesizedLocal>.Empty;
			BoundExpression value;
			if (node.LeftOnTheRightOpt != null)
			{
				value = ((boundExpression == null) ? left : BoundExpressionExtensions.SetAccessKind(boundExpression, PropertyAccessKind.Unknown));
				ArrayBuilder<SynthesizedLocal> instance = ArrayBuilder<SynthesizedLocal>.GetInstance();
				UseTwiceRewriter.Result result = UseTwiceRewriter.UseTwice(_currentMethodOrLambda, value, instance);
				from = instance.ToImmutableAndFree();
				BoundExpression expression;
				if (boundExpression != null)
				{
					boundExpression = BoundExpressionExtensions.SetAccessKind(result.First, PropertyAccessKind.Set);
					value = boundExpression;
					expression = BoundExpressionExtensions.SetAccessKind(result.Second, PropertyAccessKind.Get);
				}
				else
				{
					value = result.First;
					expression = result.Second.MakeRValue();
				}
				AddPlaceholderReplacement(node.LeftOnTheRightOpt, VisitExpressionNode(expression));
			}
			else
			{
				value = left;
			}
			BoundExpression boundExpression2 = ((boundExpression == null) ? node.Update(VisitExpressionNode(value), null, VisitAndGenerateObjectCloneIfNeeded(node.Right, node.SuppressObjectClone), suppressObjectClone: true, node.Type) : RewritePropertyAssignmentAsSetCall(node, boundExpression));
			if (from.Length > 0)
			{
				boundExpression2 = ((!TypeSymbolExtensions.IsVoidType(boundExpression2.Type)) ? new BoundSequence(node.Syntax, StaticCast<LocalSymbol>.From(from), ImmutableArray<BoundExpression>.Empty, boundExpression2, boundExpression2.Type) : new BoundSequence(node.Syntax, StaticCast<LocalSymbol>.From(from), ImmutableArray.Create(boundExpression2), null, boundExpression2.Type));
			}
			if (node.LeftOnTheRightOpt != null)
			{
				RemovePlaceholderReplacement(node.LeftOnTheRightOpt);
			}
			return boundExpression2;
		}

		private static bool IsPropertyAssignment(BoundAssignmentOperator node)
		{
			return node.Left.Kind switch
			{
				BoundKind.PropertyAccess => !((BoundPropertyAccess)node.Left).PropertySymbol.ReturnsByRef, 
				BoundKind.XmlMemberAccess => true, 
				_ => false, 
			};
		}

		private BoundExpression VisitAssignmentOperatorSimple(BoundAssignmentOperator node)
		{
			return node.Update(VisitAssignmentLeftExpression(node), null, VisitAndGenerateObjectCloneIfNeeded(node.Right, node.SuppressObjectClone), suppressObjectClone: true, node.Type);
		}

		private BoundExpression VisitAssignmentLeftExpression(BoundAssignmentOperator node)
		{
			BoundExpression left = node.Left;
			if (left.Kind == BoundKind.FieldAccess)
			{
				BoundFieldAccess boundFieldAccess = (BoundFieldAccess)left;
				if (boundFieldAccess.IsConstant)
				{
					return (BoundExpression)base.VisitFieldAccess(boundFieldAccess);
				}
			}
			return VisitExpressionNode(left);
		}

		private BoundExpression RewritePropertyAssignmentAsSetCall(BoundAssignmentOperator node, BoundExpression setNode)
		{
			return setNode.Kind switch
			{
				BoundKind.XmlMemberAccess => RewritePropertyAssignmentAsSetCall(node, ((BoundXmlMemberAccess)setNode).MemberAccess), 
				BoundKind.PropertyAccess => RewritePropertyAssignmentAsSetCall(node, (BoundPropertyAccess)setNode), 
				_ => throw ExceptionUtilities.UnexpectedValue(setNode.Kind), 
			};
		}

		[Conditional("DEBUG")]
		private static void AssertIsWriteableFromMember(BoundPropertyAccess node, Symbol fromMember)
		{
			_ = node.ReceiverOpt;
			_ = (SourcePropertySymbol)node.PropertySymbol;
			_ = node.PropertySymbol.IsShared;
		}

		private BoundExpression RewritePropertyAssignmentAsSetCall(BoundAssignmentOperator node, BoundPropertyAccess setNode)
		{
			PropertySymbol propertySymbol = setNode.PropertySymbol;
			MethodSymbol mostDerivedSetMethod = propertySymbol.GetMostDerivedSetMethod();
			if ((object)mostDerivedSetMethod == null)
			{
				FieldSymbol associatedField = propertySymbol.AssociatedField;
				BoundExpression receiverOpt = VisitExpressionNode(setNode.ReceiverOpt);
				BoundFieldAccess left = new BoundFieldAccess(setNode.Syntax, receiverOpt, associatedField, isLValue: true, associatedField.Type);
				BoundExpression right = VisitExpression(node.Right);
				return new BoundAssignmentOperator(node.Syntax, left, right, node.SuppressObjectClone, node.Type);
			}
			return RewriteReceiverArgumentsAndGenerateAccessorCall(node.Syntax, mostDerivedSetMethod, setNode.ReceiverOpt, setNode.Arguments.Concat(ImmutableArray.Create(node.Right)), node.ConstantValueOpt, isLValue: false, suppressObjectClone: false, mostDerivedSetMethod.ReturnType);
		}

		private BoundNode RewriteLateBoundAssignment(BoundAssignmentOperator node)
		{
			BoundExpression boundExpression = node.Left;
			ImmutableArray<SynthesizedLocal> from = ImmutableArray<SynthesizedLocal>.Empty;
			if (node.LeftOnTheRightOpt != null)
			{
				boundExpression = BoundExpressionExtensions.SetLateBoundAccessKind(boundExpression, LateBoundAccessKind.Unknown);
				ArrayBuilder<SynthesizedLocal> instance = ArrayBuilder<SynthesizedLocal>.GetInstance();
				UseTwiceRewriter.Result result = UseTwiceRewriter.UseTwice(_currentMethodOrLambda, boundExpression, instance);
				from = instance.ToImmutableAndFree();
				boundExpression = BoundExpressionExtensions.SetLateBoundAccessKind(result.First, LateBoundAccessKind.Set);
				BoundExpression expression = BoundExpressionExtensions.SetLateBoundAccessKind(result.Second, LateBoundAccessKind.Get);
				AddPlaceholderReplacement(node.LeftOnTheRightOpt, VisitExpressionNode(expression));
			}
			BoundExpression assignmentValue = VisitExpressionNode(node.Right);
			if (node.LeftOnTheRightOpt != null)
			{
				RemovePlaceholderReplacement(node.LeftOnTheRightOpt);
			}
			BoundExpression boundExpression2;
			if (boundExpression.Kind == BoundKind.LateMemberAccess)
			{
				boundExpression2 = LateSet(node.Syntax, (BoundLateMemberAccess)base.VisitLateMemberAccess((BoundLateMemberAccess)boundExpression), assignmentValue, default(ImmutableArray<BoundExpression>), default(ImmutableArray<string>), isCopyBack: false);
			}
			else
			{
				BoundLateInvocation boundLateInvocation = (BoundLateInvocation)boundExpression;
				if (boundLateInvocation.Member.Kind == BoundKind.LateMemberAccess)
				{
					boundExpression2 = LateSet(node.Syntax, (BoundLateMemberAccess)base.VisitLateMemberAccess((BoundLateMemberAccess)boundLateInvocation.Member), assignmentValue, VisitList(boundLateInvocation.ArgumentsOpt), boundLateInvocation.ArgumentNamesOpt, isCopyBack: false);
				}
				else
				{
					boundLateInvocation = boundLateInvocation.Update(VisitExpressionNode(boundLateInvocation.Member), VisitList(boundLateInvocation.ArgumentsOpt), boundLateInvocation.ArgumentNamesOpt, boundLateInvocation.AccessKind, boundLateInvocation.MethodOrPropertyGroupOpt, boundLateInvocation.Type);
					boundExpression2 = LateIndexSet(node.Syntax, boundLateInvocation, assignmentValue, isCopyBack: false);
				}
			}
			if (from.Length > 0)
			{
				boundExpression2 = new BoundSequence(node.Syntax, StaticCast<LocalSymbol>.From(from), ImmutableArray.Create(boundExpression2), null, boundExpression2.Type);
			}
			return boundExpression2;
		}

		private BoundExpression VisitAndGenerateObjectCloneIfNeeded(BoundExpression right, bool suppressObjectClone = false)
		{
			if (!suppressObjectClone && !right.HasErrors)
			{
				return GenerateObjectCloneIfNeeded(right, VisitExpression(right));
			}
			return VisitExpression(right);
		}

		private BoundExpression GenerateObjectCloneIfNeeded(BoundExpression generatedExpression)
		{
			return GenerateObjectCloneIfNeeded(generatedExpression, generatedExpression);
		}

		private BoundExpression GenerateObjectCloneIfNeeded(BoundExpression expression, BoundExpression rewrittenExpression)
		{
			if (expression.HasErrors || rewrittenExpression.HasErrors || _inExpressionLambda)
			{
				return rewrittenExpression;
			}
			BoundExpression boundExpression = rewrittenExpression;
			if (!boundExpression.HasErrors && TypeSymbolExtensions.IsObjectType(boundExpression.Type) && !ContainingAssembly.IsVbRuntime)
			{
				BoundExpression boundExpression2 = expression;
				while (true)
				{
					if (boundExpression2.IsConstant)
					{
						return boundExpression;
					}
					switch (boundExpression2.Kind)
					{
					case BoundKind.BinaryOperator:
					{
						BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)boundExpression2;
						if ((boundBinaryOperator.OperatorKind & BinaryOperatorKind.UserDefined) == 0)
						{
							BinaryOperatorKind binaryOperatorKind = boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask;
							if ((uint)(binaryOperatorKind - 1) <= 1u || (uint)(binaryOperatorKind - 10) <= 12u)
							{
								return boundExpression;
							}
						}
						break;
					}
					case BoundKind.UnaryOperator:
					{
						BoundUnaryOperator boundUnaryOperator = (BoundUnaryOperator)boundExpression2;
						if ((boundUnaryOperator.OperatorKind & UnaryOperatorKind.UserDefined) == 0)
						{
							UnaryOperatorKind unaryOperatorKind = boundUnaryOperator.OperatorKind & UnaryOperatorKind.Not;
							if ((uint)(unaryOperatorKind - 1) <= 2u)
							{
								return boundExpression;
							}
						}
						break;
					}
					case BoundKind.Conversion:
					case BoundKind.DirectCast:
					case BoundKind.TryCast:
					{
						ConversionKind conversionKind;
						if (boundExpression2.Kind == BoundKind.DirectCast)
						{
							BoundDirectCast obj = (BoundDirectCast)boundExpression2;
							conversionKind = obj.ConversionKind;
							boundExpression2 = obj.Operand;
						}
						else if (boundExpression2.Kind == BoundKind.TryCast)
						{
							BoundTryCast obj2 = (BoundTryCast)boundExpression2;
							conversionKind = obj2.ConversionKind;
							boundExpression2 = obj2.Operand;
						}
						else
						{
							BoundConversion obj3 = (BoundConversion)boundExpression2;
							conversionKind = obj3.ConversionKind;
							boundExpression2 = obj3.Operand;
						}
						if (Conversions.IsIdentityConversion(conversionKind))
						{
							continue;
						}
						return boundExpression;
					}
					case BoundKind.Parenthesized:
						boundExpression2 = ((BoundParenthesized)boundExpression2).Expression;
						continue;
					case BoundKind.XmlEmbeddedExpression:
						boundExpression2 = ((BoundXmlEmbeddedExpression)boundExpression2).Expression;
						continue;
					}
					break;
				}
				MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__GetObjectValueObject);
				if (!ReportMissingOrBadRuntimeHelper(boundExpression2, WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__GetObjectValueObject, methodSymbol))
				{
					boundExpression = new BoundCall(expression.Syntax, methodSymbol, null, null, ImmutableArray.Create(boundExpression), null, methodSymbol.ReturnType);
				}
			}
			return boundExpression;
		}

		private BoundExpression RewriteTrivialMidAssignment(BoundAssignmentOperator node)
		{
			BoundMidResult boundMidResult = (BoundMidResult)node.Right;
			MethodSymbol methodSymbol = null;
			methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_StringType__MidStmtStr);
			if (ReportMissingOrBadRuntimeHelper(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_StringType__MidStmtStr, methodSymbol))
			{
				return boundMidResult.Update(VisitExpressionNode(node.Left), VisitExpressionNode(boundMidResult.Start), VisitExpressionNode(boundMidResult.LengthOpt), VisitExpressionNode(boundMidResult.Source), node.Type);
			}
			ImmutableArray<SynthesizedLocal> temporaries = default(ImmutableArray<SynthesizedLocal>);
			ImmutableArray<BoundExpression> copyBack = default(ImmutableArray<BoundExpression>);
			return new BoundCall(node.Syntax, methodSymbol, null, null, RewriteCallArguments(ImmutableArray.Create(node.Left, boundMidResult.Start, boundMidResult.LengthOpt ?? new BoundLiteral(node.Syntax, ConstantValue.Create(int.MaxValue), boundMidResult.Start.Type), boundMidResult.Source), methodSymbol.Parameters, out temporaries, out copyBack, suppressObjectClone: false), null, methodSymbol.ReturnType);
		}

		public override BoundNode VisitMidResult(BoundMidResult node)
		{
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, node.Type, SynthesizedLocalKind.LoweringTemp);
			BoundLocal boundLocal = new BoundLocal(node.Syntax, synthesizedLocal, node.Type);
			BoundCompoundAssignmentTargetPlaceholder boundCompoundAssignmentTargetPlaceholder = new BoundCompoundAssignmentTargetPlaceholder(node.Syntax, node.Type);
			return new BoundSequence(node.Syntax, ImmutableArray.Create((LocalSymbol)synthesizedLocal), ImmutableArray.Create(new BoundAssignmentOperator(node.Syntax, boundLocal, VisitExpressionNode(node.Original), suppressObjectClone: true), RewriteTrivialMidAssignment(new BoundAssignmentOperator(node.Syntax, boundLocal, boundCompoundAssignmentTargetPlaceholder, node.Update(boundCompoundAssignmentTargetPlaceholder, node.Start, node.LengthOpt, node.Source, node.Type), suppressObjectClone: false))), boundLocal.MakeRValue(), node.Type);
		}

		public override BoundNode VisitUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node)
		{
			if (_inExpressionLambda)
			{
				return node.Update(node.OperatorKind, (BoundExpression)Visit(node.UnderlyingExpression), node.Checked, node.Type);
			}
			if ((node.OperatorKind & BinaryOperatorKind.Lifted) != 0)
			{
				return RewriteLiftedUserDefinedBinaryOperator(node);
			}
			return Visit(node.UnderlyingExpression);
		}

		public override BoundNode VisitUserDefinedShortCircuitingOperator(BoundUserDefinedShortCircuitingOperator node)
		{
			if (_inExpressionLambda)
			{
				BoundRValuePlaceholder leftOperandPlaceholder = node.LeftOperandPlaceholder;
				BoundExpression leftOperand = node.LeftOperand;
				if (leftOperandPlaceholder != null)
				{
					AddPlaceholderReplacement(leftOperandPlaceholder, VisitExpression(leftOperand));
				}
				BoundUserDefinedBinaryOperator bitwiseOperator = (BoundUserDefinedBinaryOperator)VisitExpression(node.BitwiseOperator);
				if (leftOperandPlaceholder != null)
				{
					RemovePlaceholderReplacement(leftOperandPlaceholder);
				}
				return node.Update(node.LeftOperand, node.LeftOperandPlaceholder, node.LeftTest, bitwiseOperator, node.Type);
			}
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, node.LeftOperand.Type, SynthesizedLocalKind.LoweringTemp);
			BoundLocal boundLocal = new BoundLocal(node.Syntax, synthesizedLocal, isLValue: true, synthesizedLocal.Type);
			AddPlaceholderReplacement(node.LeftOperandPlaceholder, new BoundAssignmentOperator(node.Syntax, boundLocal, VisitExpressionNode(node.LeftOperand), suppressObjectClone: true, synthesizedLocal.Type));
			BoundExpression condition = VisitExpressionNode(node.LeftTest);
			boundLocal = boundLocal.MakeRValue();
			UpdatePlaceholderReplacement(node.LeftOperandPlaceholder, boundLocal);
			BoundExpression whenFalse = VisitExpressionNode(node.BitwiseOperator);
			RemovePlaceholderReplacement(node.LeftOperandPlaceholder);
			return new BoundSequence(node.Syntax, ImmutableArray.Create((LocalSymbol)synthesizedLocal), ImmutableArray<BoundExpression>.Empty, MakeTernaryConditionalExpression(node.Syntax, condition, boundLocal, whenFalse), synthesizedLocal.Type);
		}

		public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
		{
			bool flag = (node.OperatorKind & BinaryOperatorKind.OptimizableForConditionalBranch) != 0;
			bool optimizeForConditionalBranch = flag;
			BoundExpression leftOperand = GetLeftOperand(node, ref optimizeForConditionalBranch);
			if (leftOperand.Kind != BoundKind.BinaryOperator)
			{
				return RewriteBinaryOperatorSimple(node, flag);
			}
			ArrayBuilder<(BoundBinaryOperator, bool)> instance = ArrayBuilder<(BoundBinaryOperator, bool)>.GetInstance();
			instance.Push((node, flag));
			BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)leftOperand;
			while (true)
			{
				if (optimizeForConditionalBranch)
				{
					BinaryOperatorKind binaryOperatorKind = boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask;
					if (binaryOperatorKind != BinaryOperatorKind.OrElse && binaryOperatorKind != BinaryOperatorKind.AndAlso)
					{
						optimizeForConditionalBranch = false;
					}
				}
				instance.Push((boundBinaryOperator, optimizeForConditionalBranch));
				leftOperand = GetLeftOperand(boundBinaryOperator, ref optimizeForConditionalBranch);
				if (leftOperand.Kind != BoundKind.BinaryOperator)
				{
					break;
				}
				boundBinaryOperator = (BoundBinaryOperator)leftOperand;
			}
			BoundExpression boundExpression = VisitExpressionNode(leftOperand);
			do
			{
				(BoundBinaryOperator, bool) tuple = instance.Pop();
				boundBinaryOperator = tuple.Item1;
				BoundExpression right = VisitExpression(GetRightOperand(boundBinaryOperator, tuple.Item2));
				boundExpression = (((boundBinaryOperator.OperatorKind & BinaryOperatorKind.Lifted) == 0) ? TransformRewrittenBinaryOperator(boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, boundExpression, right, boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, VisitType(boundBinaryOperator.Type))) : FinishRewriteOfLiftedIntrinsicBinaryOperator(boundBinaryOperator, boundExpression, right, tuple.Item2));
			}
			while (boundBinaryOperator != node);
			instance.Free();
			return boundExpression;
		}

		private static BoundExpression GetLeftOperand(BoundBinaryOperator binary, ref bool optimizeForConditionalBranch)
		{
			if (optimizeForConditionalBranch && (binary.OperatorKind & BinaryOperatorKind.OpMask) != BinaryOperatorKind.OrElse)
			{
				optimizeForConditionalBranch = false;
			}
			return BoundExpressionExtensions.GetMostEnclosedParenthesizedExpression(binary.Left);
		}

		private static BoundExpression GetRightOperand(BoundBinaryOperator binary, bool adjustIfOptimizableForConditionalBranch)
		{
			if (adjustIfOptimizableForConditionalBranch)
			{
				BoundExpression right = binary.Right;
				bool optimizableForConditionalBranch = false;
				return AdjustIfOptimizableForConditionalBranch(right, out optimizableForConditionalBranch);
			}
			return binary.Right;
		}

		private BoundNode RewriteBinaryOperatorSimple(BoundBinaryOperator node, bool optimizeForConditionalBranch)
		{
			if ((node.OperatorKind & BinaryOperatorKind.Lifted) != 0)
			{
				return RewriteLiftedIntrinsicBinaryOperatorSimple(node, optimizeForConditionalBranch);
			}
			return TransformRewrittenBinaryOperator((BoundBinaryOperator)base.VisitBinaryOperator(node));
		}

		private BoundExpression ReplaceMyGroupCollectionPropertyGetWithUnderlyingField(BoundExpression operand)
		{
			if (operand.HasErrors)
			{
				return operand;
			}
			switch (operand.Kind)
			{
			case BoundKind.DirectCast:
			{
				BoundDirectCast boundDirectCast = (BoundDirectCast)operand;
				return boundDirectCast.Update(ReplaceMyGroupCollectionPropertyGetWithUnderlyingField(boundDirectCast.Operand), boundDirectCast.ConversionKind, boundDirectCast.SuppressVirtualCalls, boundDirectCast.ConstantValueOpt, boundDirectCast.RelaxationLambdaOpt, boundDirectCast.Type);
			}
			case BoundKind.Conversion:
			{
				BoundConversion boundConversion = (BoundConversion)operand;
				return boundConversion.Update(ReplaceMyGroupCollectionPropertyGetWithUnderlyingField(boundConversion.Operand), boundConversion.ConversionKind, boundConversion.Checked, boundConversion.ExplicitCastInCode, boundConversion.ConstantValueOpt, boundConversion.ExtendedInfoOpt, boundConversion.Type);
			}
			case BoundKind.Call:
			{
				BoundCall boundCall = (BoundCall)operand;
				if (boundCall.Method.MethodKind == MethodKind.PropertyGet && (object)boundCall.Method.AssociatedSymbol != null && boundCall.Method.AssociatedSymbol.IsMyGroupCollectionProperty)
				{
					return new BoundFieldAccess(boundCall.Syntax, boundCall.ReceiverOpt, ((PropertySymbol)boundCall.Method.AssociatedSymbol).AssociatedField, isLValue: false, boundCall.Type);
				}
				break;
			}
			case BoundKind.PropertyAccess:
			{
				BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)operand;
				if (boundPropertyAccess.AccessKind == PropertyAccessKind.Get && boundPropertyAccess.PropertySymbol.IsMyGroupCollectionProperty)
				{
					return new BoundFieldAccess(boundPropertyAccess.Syntax, boundPropertyAccess.ReceiverOpt, boundPropertyAccess.PropertySymbol.AssociatedField, isLValue: false, boundPropertyAccess.Type);
				}
				break;
			}
			}
			return operand;
		}

		private BoundExpression TransformRewrittenBinaryOperator(BoundBinaryOperator node)
		{
			switch (node.OperatorKind & BinaryOperatorKind.OpMask)
			{
			case BinaryOperatorKind.Is:
			case BinaryOperatorKind.IsNot:
				node = node.Update(node.OperatorKind, ReplaceMyGroupCollectionPropertyGetWithUnderlyingField(node.Left), ReplaceMyGroupCollectionPropertyGetWithUnderlyingField(node.Right), node.Checked, node.ConstantValueOpt, node.Type);
				if (((object)node.Left.Type != null && TypeSymbolExtensions.IsNullableType(node.Left.Type)) || ((object)node.Right.Type != null && TypeSymbolExtensions.IsNullableType(node.Right.Type)))
				{
					return RewriteNullableIsOrIsNotOperator(node);
				}
				break;
			case BinaryOperatorKind.Concatenate:
				if (TypeSymbolExtensions.IsObjectType(node.Type))
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConcatenateObjectObjectObject);
				}
				return RewriteConcatenateOperator(node);
			case BinaryOperatorKind.Like:
				if (TypeSymbolExtensions.IsObjectType(node.Left.Type))
				{
					return RewriteLikeOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_LikeOperator__LikeObjectObjectObjectCompareMethod);
				}
				return RewriteLikeOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_LikeOperator__LikeStringStringStringCompareMethod);
			case BinaryOperatorKind.Equals:
			{
				TypeSymbol type5 = node.Left.Type;
				if (TypeSymbolExtensions.IsObjectType(node.Type) || (_inExpressionLambda && TypeSymbolExtensions.IsObjectType(type5)))
				{
					return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectEqualObjectObjectBoolean);
				}
				if (TypeSymbolExtensions.IsBooleanType(node.Type))
				{
					if (TypeSymbolExtensions.IsObjectType(type5))
					{
						return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectEqualObjectObjectBoolean);
					}
					if (TypeSymbolExtensions.IsStringType(type5))
					{
						return RewriteStringComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDecimalType(type5))
					{
						return RewriteDecimalComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDateTimeType(type5))
					{
						return RewriteDateComparisonOperator(node);
					}
				}
				break;
			}
			case BinaryOperatorKind.NotEquals:
			{
				TypeSymbol type = node.Left.Type;
				if (TypeSymbolExtensions.IsObjectType(node.Type) || (_inExpressionLambda && TypeSymbolExtensions.IsObjectType(type)))
				{
					return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectNotEqualObjectObjectBoolean);
				}
				if (TypeSymbolExtensions.IsBooleanType(node.Type))
				{
					if (TypeSymbolExtensions.IsObjectType(type))
					{
						return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectNotEqualObjectObjectBoolean);
					}
					if (TypeSymbolExtensions.IsStringType(type))
					{
						return RewriteStringComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDecimalType(type))
					{
						return RewriteDecimalComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDateTimeType(type))
					{
						return RewriteDateComparisonOperator(node);
					}
				}
				break;
			}
			case BinaryOperatorKind.LessThanOrEqual:
			{
				TypeSymbol type6 = node.Left.Type;
				if (TypeSymbolExtensions.IsObjectType(node.Type) || (_inExpressionLambda && TypeSymbolExtensions.IsObjectType(type6)))
				{
					return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectLessEqualObjectObjectBoolean);
				}
				if (TypeSymbolExtensions.IsBooleanType(node.Type))
				{
					if (TypeSymbolExtensions.IsObjectType(type6))
					{
						return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectLessEqualObjectObjectBoolean);
					}
					if (TypeSymbolExtensions.IsStringType(type6))
					{
						return RewriteStringComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDecimalType(type6))
					{
						return RewriteDecimalComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDateTimeType(type6))
					{
						return RewriteDateComparisonOperator(node);
					}
				}
				break;
			}
			case BinaryOperatorKind.GreaterThanOrEqual:
			{
				TypeSymbol type2 = node.Left.Type;
				if (TypeSymbolExtensions.IsObjectType(node.Type) || (_inExpressionLambda && TypeSymbolExtensions.IsObjectType(type2)))
				{
					return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectGreaterEqualObjectObjectBoolean);
				}
				if (TypeSymbolExtensions.IsBooleanType(node.Type))
				{
					if (TypeSymbolExtensions.IsObjectType(type2))
					{
						return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectGreaterEqualObjectObjectBoolean);
					}
					if (TypeSymbolExtensions.IsStringType(type2))
					{
						return RewriteStringComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDecimalType(type2))
					{
						return RewriteDecimalComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDateTimeType(type2))
					{
						return RewriteDateComparisonOperator(node);
					}
				}
				break;
			}
			case BinaryOperatorKind.LessThan:
			{
				TypeSymbol type4 = node.Left.Type;
				if (TypeSymbolExtensions.IsObjectType(node.Type) || (_inExpressionLambda && TypeSymbolExtensions.IsObjectType(type4)))
				{
					return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectLessObjectObjectBoolean);
				}
				if (TypeSymbolExtensions.IsBooleanType(node.Type))
				{
					if (TypeSymbolExtensions.IsObjectType(type4))
					{
						return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectLessObjectObjectBoolean);
					}
					if (TypeSymbolExtensions.IsStringType(type4))
					{
						return RewriteStringComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDecimalType(type4))
					{
						return RewriteDecimalComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDateTimeType(type4))
					{
						return RewriteDateComparisonOperator(node);
					}
				}
				break;
			}
			case BinaryOperatorKind.GreaterThan:
			{
				TypeSymbol type3 = node.Left.Type;
				if (TypeSymbolExtensions.IsObjectType(node.Type) || (_inExpressionLambda && TypeSymbolExtensions.IsObjectType(type3)))
				{
					return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareObjectGreaterObjectObjectBoolean);
				}
				if (TypeSymbolExtensions.IsBooleanType(node.Type))
				{
					if (TypeSymbolExtensions.IsObjectType(type3))
					{
						return RewriteObjectComparisonOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ConditionalCompareObjectGreaterObjectObjectBoolean);
					}
					if (TypeSymbolExtensions.IsStringType(type3))
					{
						return RewriteStringComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDecimalType(type3))
					{
						return RewriteDecimalComparisonOperator(node);
					}
					if (TypeSymbolExtensions.IsDateTimeType(type3))
					{
						return RewriteDateComparisonOperator(node);
					}
				}
				break;
			}
			case BinaryOperatorKind.Add:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__AddObjectObjectObject);
				}
				if (TypeSymbolExtensions.IsDecimalType(node.Type))
				{
					return RewriteDecimalBinaryOperator(node, SpecialMember.System_Decimal__AddDecimalDecimal);
				}
				break;
			case BinaryOperatorKind.Subtract:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__SubtractObjectObjectObject);
				}
				if (TypeSymbolExtensions.IsDecimalType(node.Type))
				{
					return RewriteDecimalBinaryOperator(node, SpecialMember.System_Decimal__SubtractDecimalDecimal);
				}
				break;
			case BinaryOperatorKind.Multiply:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__MultiplyObjectObjectObject);
				}
				if (TypeSymbolExtensions.IsDecimalType(node.Type))
				{
					return RewriteDecimalBinaryOperator(node, SpecialMember.System_Decimal__MultiplyDecimalDecimal);
				}
				break;
			case BinaryOperatorKind.Modulo:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ModObjectObjectObject);
				}
				if (TypeSymbolExtensions.IsDecimalType(node.Type))
				{
					return RewriteDecimalBinaryOperator(node, SpecialMember.System_Decimal__RemainderDecimalDecimal);
				}
				break;
			case BinaryOperatorKind.Divide:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__DivideObjectObjectObject);
				}
				if (TypeSymbolExtensions.IsDecimalType(node.Type))
				{
					return RewriteDecimalBinaryOperator(node, SpecialMember.System_Decimal__DivideDecimalDecimal);
				}
				break;
			case BinaryOperatorKind.IntegerDivide:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__IntDivideObjectObjectObject);
				}
				break;
			case BinaryOperatorKind.Power:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__ExponentObjectObjectObject);
				}
				return RewritePowOperator(node);
			case BinaryOperatorKind.LeftShift:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__LeftShiftObjectObjectObject);
				}
				break;
			case BinaryOperatorKind.RightShift:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__RightShiftObjectObjectObject);
				}
				break;
			case BinaryOperatorKind.OrElse:
			case BinaryOperatorKind.AndAlso:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectShortCircuitOperator(node);
				}
				break;
			case BinaryOperatorKind.Xor:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__XorObjectObjectObject);
				}
				break;
			case BinaryOperatorKind.Or:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__OrObjectObjectObject);
				}
				break;
			case BinaryOperatorKind.And:
				if (TypeSymbolExtensions.IsObjectType(node.Type) && !_inExpressionLambda)
				{
					return RewriteObjectBinaryOperator(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__AndObjectObjectObject);
				}
				break;
			}
			return node;
		}

		private BoundExpression RewriteDateComparisonOperator(BoundBinaryOperator node)
		{
			if (_inExpressionLambda)
			{
				return node;
			}
			BoundExpression result = node;
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			if (TypeSymbolExtensions.IsDateTimeType(left.Type) && TypeSymbolExtensions.IsDateTimeType(right.Type))
			{
				MethodSymbol methodSymbol = (MethodSymbol)ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_DateTime__CompareDateTimeDateTime);
				if (!ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_DateTime__CompareDateTimeDateTime, methodSymbol))
				{
					BoundCall left2 = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(left, right), null, methodSymbol.ReturnType);
					result = new BoundBinaryOperator(node.Syntax, node.OperatorKind & BinaryOperatorKind.OpMask, left2, new BoundLiteral(node.Syntax, ConstantValue.Create(0), methodSymbol.ReturnType), @checked: false, node.Type);
				}
			}
			return result;
		}

		private BoundExpression RewriteDecimalComparisonOperator(BoundBinaryOperator node)
		{
			if (_inExpressionLambda)
			{
				return node;
			}
			BoundExpression result = node;
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			if (TypeSymbolExtensions.IsDecimalType(left.Type) && TypeSymbolExtensions.IsDecimalType(right.Type))
			{
				MethodSymbol methodSymbol = (MethodSymbol)ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__CompareDecimalDecimal);
				if (!ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_Decimal__CompareDecimalDecimal, methodSymbol))
				{
					BoundCall left2 = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(left, right), null, methodSymbol.ReturnType);
					result = new BoundBinaryOperator(node.Syntax, node.OperatorKind & BinaryOperatorKind.OpMask, left2, new BoundLiteral(node.Syntax, ConstantValue.Create(0), methodSymbol.ReturnType), @checked: false, node.Type);
				}
			}
			return result;
		}

		private BoundExpression RewriteObjectShortCircuitOperator(BoundBinaryOperator node)
		{
			BoundExpression result = node;
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			if (TypeSymbolExtensions.IsObjectType(left.Type) && TypeSymbolExtensions.IsObjectType(right.Type))
			{
				BoundExpression boundExpression = left;
				BoundExpression boundExpression2 = right;
				if (boundExpression.Kind == BoundKind.DirectCast)
				{
					BoundDirectCast boundDirectCast = (BoundDirectCast)boundExpression;
					if (TypeSymbolExtensions.IsBooleanType(boundDirectCast.Operand.Type))
					{
						boundExpression = boundDirectCast.Operand;
					}
				}
				if (boundExpression2.Kind == BoundKind.DirectCast)
				{
					BoundDirectCast boundDirectCast2 = (BoundDirectCast)boundExpression2;
					if (TypeSymbolExtensions.IsBooleanType(boundDirectCast2.Operand.Type))
					{
						boundExpression2 = boundDirectCast2.Operand;
					}
				}
				if (boundExpression == left || boundExpression2 == right)
				{
					MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanObject);
					if (!ReportMissingOrBadRuntimeHelper(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanObject, methodSymbol))
					{
						if (boundExpression == left)
						{
							boundExpression = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(boundExpression), null, methodSymbol.ReturnType);
						}
						if (boundExpression2 == right)
						{
							boundExpression2 = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(boundExpression2), null, methodSymbol.ReturnType);
						}
					}
				}
				if (boundExpression != left && boundExpression2 != right)
				{
					BoundBinaryOperator operand = new BoundBinaryOperator(node.Syntax, node.OperatorKind & BinaryOperatorKind.OpMask, boundExpression, boundExpression2, @checked: false, boundExpression.Type);
					result = new BoundDirectCast(node.Syntax, operand, ConversionKind.WideningValue, node.Type);
				}
				return result;
			}
			throw ExceptionUtilities.Unreachable;
		}

		private BoundExpression RewritePowOperator(BoundBinaryOperator node)
		{
			if (_inExpressionLambda)
			{
				return node;
			}
			BoundExpression result = node;
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			if (TypeSymbolExtensions.IsDoubleType(node.Type) && TypeSymbolExtensions.IsDoubleType(left.Type) && TypeSymbolExtensions.IsDoubleType(right.Type))
			{
				MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__PowDoubleDouble);
				if (!ReportMissingOrBadRuntimeHelper(node, WellKnownMember.System_Math__PowDoubleDouble, methodSymbol))
				{
					result = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(left, right), null, methodSymbol.ReturnType);
				}
			}
			return result;
		}

		private BoundExpression RewriteDecimalBinaryOperator(BoundBinaryOperator node, SpecialMember member)
		{
			if (_inExpressionLambda)
			{
				return node;
			}
			BoundExpression result = node;
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			if (TypeSymbolExtensions.IsDecimalType(left.Type) && TypeSymbolExtensions.IsDecimalType(right.Type))
			{
				MethodSymbol methodSymbol = (MethodSymbol)ContainingAssembly.GetSpecialTypeMember(member);
				if (!ReportMissingOrBadRuntimeHelper(node, member, methodSymbol))
				{
					result = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(left, right), null, methodSymbol.ReturnType);
				}
			}
			return result;
		}

		private BoundExpression RewriteStringComparisonOperator(BoundBinaryOperator node)
		{
			BoundExpression result = node;
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			bool value = (node.OperatorKind & BinaryOperatorKind.CompareText) != 0;
			NamedTypeSymbol wellKnownType = Compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators);
			WellKnownMember wellKnownMember = ((TypeSymbolExtensions.IsErrorType(wellKnownType) && wellKnownType is MissingMetadataTypeSymbol) ? WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareStringStringStringBoolean : WellKnownMember.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators__CompareStringStringStringBoolean);
			MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(wellKnownMember);
			if (!ReportMissingOrBadRuntimeHelper(node, wellKnownMember, methodSymbol))
			{
				BoundCall left2 = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(left, right, new BoundLiteral(node.Syntax, ConstantValue.Create(value), methodSymbol.Parameters[2].Type)), null, methodSymbol.ReturnType);
				result = new BoundBinaryOperator(node.Syntax, node.OperatorKind & BinaryOperatorKind.OpMask, left2, new BoundLiteral(node.Syntax, ConstantValue.Create(0), methodSymbol.ReturnType), @checked: false, node.Type);
			}
			return result;
		}

		private BoundExpression RewriteObjectComparisonOperator(BoundBinaryOperator node, WellKnownMember member)
		{
			BoundExpression boundExpression = node;
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			bool value = (node.OperatorKind & BinaryOperatorKind.CompareText) != 0;
			MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(member);
			if (!ReportMissingOrBadRuntimeHelper(node, member, methodSymbol))
			{
				boundExpression = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(left, right, new BoundLiteral(node.Syntax, ConstantValue.Create(value), methodSymbol.Parameters[2].Type)), null, methodSymbol.ReturnType, suppressObjectClone: true);
				if (_inExpressionLambda && TypeSymbolExtensions.IsObjectType(methodSymbol.ReturnType) && TypeSymbolExtensions.IsBooleanType(node.Type))
				{
					boundExpression = new BoundConversion(node.Syntax, boundExpression, ConversionKind.NarrowingBoolean, node.Checked, explicitCastInCode: false, node.Type);
				}
			}
			return boundExpression;
		}

		private BoundExpression RewriteLikeOperator(BoundBinaryOperator node, WellKnownMember member)
		{
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			BoundExpression result = node;
			bool flag = (node.OperatorKind & BinaryOperatorKind.CompareText) != 0;
			MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(member);
			if (!ReportMissingOrBadRuntimeHelper(node, member, methodSymbol))
			{
				result = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(left, right, new BoundLiteral(node.Syntax, ConstantValue.Create(flag ? 1 : 0), methodSymbol.Parameters[2].Type)), null, methodSymbol.ReturnType, suppressObjectClone: true);
			}
			return result;
		}

		private BoundExpression RewriteObjectBinaryOperator(BoundBinaryOperator node, WellKnownMember member)
		{
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			BoundExpression result = node;
			MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(member);
			if (!ReportMissingOrBadRuntimeHelper(node, member, methodSymbol))
			{
				result = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(left, right), null, methodSymbol.ReturnType, suppressObjectClone: true);
			}
			return result;
		}

		private BoundNode RewriteLiftedIntrinsicBinaryOperatorSimple(BoundBinaryOperator node, bool optimizeForConditionalBranch)
		{
			BoundExpression left = VisitExpressionNode(node.Left);
			BoundExpression right = VisitExpressionNode(GetRightOperand(node, optimizeForConditionalBranch));
			return FinishRewriteOfLiftedIntrinsicBinaryOperator(node, left, right, optimizeForConditionalBranch);
		}

		private BoundExpression FinishRewriteOfLiftedIntrinsicBinaryOperator(BoundBinaryOperator node, BoundExpression left, BoundExpression right, bool optimizeForConditionalBranch)
		{
			bool flag = HasValue(left);
			bool flag2 = HasValue(right);
			bool flag3 = HasNoValue(left);
			bool flag4 = HasNoValue(right);
			if (optimizeForConditionalBranch && TypeSymbolExtensions.IsNullableOfBoolean(node.Type) && TypeSymbolExtensions.IsNullableOfBoolean(left.Type) && TypeSymbolExtensions.IsNullableOfBoolean(right.Type) && (flag || !_inExpressionLambda || (node.OperatorKind & BinaryOperatorKind.OpMask) == BinaryOperatorKind.OrElse))
			{
				return RewriteAndOptimizeLiftedIntrinsicLogicalShortCircuitingOperator(node, left, right, flag3, flag, flag4, flag2);
			}
			if (_inExpressionLambda)
			{
				return node.Update(node.OperatorKind, left, right, node.Checked, node.ConstantValueOpt, node.Type);
			}
			if (flag3 && flag4)
			{
				return NullableNull(left, node.Type);
			}
			if (flag && flag2)
			{
				BoundExpression expr = ApplyUnliftedBinaryOp(node, NullableValueOrDefault(left), NullableValueOrDefault(right));
				return WrapInNullable(expr, node.Type);
			}
			if (TypeSymbolExtensions.IsNullableOfBoolean(node.Left.Type))
			{
				BinaryOperatorKind binaryOperatorKind = node.OperatorKind & BinaryOperatorKind.OpMask;
				if ((uint)(binaryOperatorKind - 19) <= 3u)
				{
					return RewriteLiftedBooleanBinaryOperator(node, left, right, flag3, flag4, flag, flag2);
				}
			}
			if (flag3 || flag4)
			{
				BoundExpression first = (flag3 ? right : left);
				BoundExpression second = NullableNull(flag3 ? left : right, node.Type);
				return MakeSequence(first, second);
			}
			if (flag2)
			{
				BoundExpression whenNotNull = null;
				BoundExpression whenNull = null;
				if (IsConditionalAccess(left, out whenNotNull, out whenNull))
				{
					BoundExpression boundExpression = NullableValueOrDefault(right);
					if ((boundExpression.IsConstant || boundExpression.Kind == BoundKind.Local || boundExpression.Kind == BoundKind.Parameter) && HasValue(whenNotNull) && HasNoValue(whenNull))
					{
						return UpdateConditionalAccess(left, WrapInNullable(ApplyUnliftedBinaryOp(node, NullableValueOrDefault(whenNotNull), boundExpression), node.Type), NullableNull(whenNull, node.Type));
					}
				}
			}
			ArrayBuilder<LocalSymbol> temps = null;
			ArrayBuilder<BoundExpression> inits = null;
			BoundExpression hasValueExpr = null;
			BoundExpression hasValueExpr2 = null;
			BoundExpression left2 = ProcessNullableOperand(left, out hasValueExpr, ref temps, ref inits, RightCantChangeLeftLocal(left, right), flag);
			BoundExpression right2 = ProcessNullableOperand(right, out hasValueExpr2, ref temps, ref inits, doNotCaptureLocals: true, flag2);
			BoundExpression boundExpression2 = null;
			BoundExpression condition = MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.And, hasValueExpr, hasValueExpr2);
			BoundExpression expr2 = ApplyUnliftedBinaryOp(node, left2, right2);
			boundExpression2 = MakeTernaryConditionalExpression(node.Syntax, condition, WrapInNullable(expr2, node.Type), NullableNull(node.Syntax, node.Type));
			if (temps != null)
			{
				boundExpression2 = new BoundSequence(node.Syntax, temps.ToImmutableAndFree(), inits.ToImmutableAndFree(), boundExpression2, boundExpression2.Type);
			}
			return boundExpression2;
		}

		private BoundExpression RewriteAndOptimizeLiftedIntrinsicLogicalShortCircuitingOperator(BoundBinaryOperator node, BoundExpression left, BoundExpression right, bool leftHasNoValue, bool leftHasValue, bool rightHasNoValue, bool rightHasValue)
		{
			BoundExpression boundExpression = null;
			if (!_inExpressionLambda)
			{
				if (leftHasNoValue && rightHasNoValue)
				{
					return NullableNull(left, node.Type);
				}
				if ((node.OperatorKind & BinaryOperatorKind.OpMask) == BinaryOperatorKind.OrElse)
				{
					if (leftHasNoValue)
					{
						return right;
					}
					if (rightHasNoValue)
					{
						return left;
					}
				}
				else if (leftHasNoValue)
				{
					boundExpression = EvaluateOperandAndReturnFalse(node, right, rightHasValue);
				}
				else if (rightHasNoValue)
				{
					boundExpression = EvaluateOperandAndReturnFalse(node, left, leftHasValue);
				}
				else if (!leftHasValue)
				{
					SynthesizedLocal temp = null;
					BoundExpression init = null;
					BoundExpression expr = CaptureNullableIfNeeded(left, out temp, out init, RightCantChangeLeftLocal(left, right));
					boundExpression = MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.AndAlso, MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.OrElse, new BoundUnaryOperator(node.Syntax, UnaryOperatorKind.Not, NullableHasValue(expr), @checked: false, TypeSymbolExtensions.GetNullableUnderlyingType(node.Type)), NullableValueOrDefault(expr)), MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.AndAlso, NullableValueOrDefault(right), NullableHasValue(expr)));
					if ((object)temp != null)
					{
						boundExpression = new BoundSequence(node.Syntax, ImmutableArray.Create((LocalSymbol)temp), ImmutableArray.Create(init), boundExpression, boundExpression.Type);
					}
				}
			}
			if (boundExpression == null)
			{
				boundExpression = ApplyUnliftedBinaryOp(node, NullableValueOrDefault(left, leftHasValue), NullableValueOrDefault(right, rightHasValue));
			}
			return WrapInNullable(boundExpression, node.Type);
		}

		private BoundExpression EvaluateOperandAndReturnFalse(BoundBinaryOperator node, BoundExpression operand, bool operandHasValue)
		{
			BoundLiteral boundLiteral = new BoundLiteral(node.Syntax, ConstantValue.False, TypeSymbolExtensions.GetNullableUnderlyingType(node.Type));
			return new BoundSequence(node.Syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(operandHasValue ? NullableValueOrDefault(operand) : operand), boundLiteral, boundLiteral.Type);
		}

		private BoundExpression NullableValueOrDefault(BoundExpression operand, bool operandHasValue)
		{
			if (!_inExpressionLambda || operandHasValue)
			{
				return NullableValueOrDefault(operand);
			}
			return new BoundNullableIsTrueOperator(operand.Syntax, operand, TypeSymbolExtensions.GetNullableUnderlyingType(operand.Type));
		}

		private BoundExpression RewriteLiftedBooleanBinaryOperator(BoundBinaryOperator node, BoundExpression left, BoundExpression right, bool leftHasNoValue, bool rightHasNoValue, bool leftHasValue, bool rightHasValue)
		{
			TypeSymbol type = node.Type;
			TypeSymbol nullableUnderlyingType = TypeSymbolExtensions.GetNullableUnderlyingType(type);
			BinaryOperatorKind binaryOperatorKind = node.OperatorKind & BinaryOperatorKind.OpMask;
			bool flag = binaryOperatorKind == BinaryOperatorKind.OrElse || binaryOperatorKind == BinaryOperatorKind.Or;
			if (leftHasNoValue || rightHasNoValue)
			{
				BoundExpression boundExpression;
				BoundExpression candidateNullExpression;
				bool flag2;
				if (rightHasNoValue)
				{
					boundExpression = left;
					candidateNullExpression = right;
					flag2 = leftHasValue;
				}
				else
				{
					boundExpression = right;
					candidateNullExpression = left;
					flag2 = rightHasValue;
				}
				if (flag2)
				{
					SyntaxNode syntax = boundExpression.Syntax;
					BoundExpression condition = NullableValueOrDefault(boundExpression);
					return MakeTernaryConditionalExpression(node.Syntax, condition, flag ? NullableTrue(syntax, type) : NullableNull(candidateNullExpression, type), flag ? NullableNull(candidateNullExpression, type) : NullableFalse(syntax, type));
				}
				SynthesizedLocal temp = null;
				BoundExpression init = null;
				BoundExpression boundExpression2 = CaptureNullableIfNeeded(boundExpression, out temp, out init, doNotCaptureLocals: true);
				SyntaxNode syntax2 = boundExpression.Syntax;
				NullableValueOrDefault(boundExpression2);
				BoundExpression condition2 = (flag ? NullableValueOrDefault(boundExpression2) : MakeBooleanBinaryExpression(syntax2, BinaryOperatorKind.And, NullableHasValue(boundExpression2), new BoundUnaryOperator(syntax2, UnaryOperatorKind.Not, NullableValueOrDefault(boundExpression2), @checked: false, nullableUnderlyingType)));
				BoundExpression boundExpression3 = MakeTernaryConditionalExpression(node.Syntax, condition2, boundExpression2, NullableNull(candidateNullExpression, type));
				if ((object)temp != null)
				{
					boundExpression3 = new BoundSequence(node.Syntax, ImmutableArray.Create((LocalSymbol)temp), ImmutableArray.Create(init), boundExpression3, boundExpression3.Type);
				}
				return boundExpression3;
			}
			bool flag3 = binaryOperatorKind == BinaryOperatorKind.AndAlso || binaryOperatorKind == BinaryOperatorKind.OrElse;
			SynthesizedLocal temp2 = null;
			BoundExpression init2 = null;
			BoundExpression boundExpression4 = left;
			if (!leftHasValue)
			{
				boundExpression4 = CaptureNullableIfNeeded(left, out temp2, out init2, RightCantChangeLeftLocal(left, right));
			}
			SynthesizedLocal temp3 = null;
			BoundExpression init3 = null;
			BoundExpression boundExpression5 = right;
			if (!rightHasValue && (!leftHasValue || !flag3))
			{
				boundExpression5 = CaptureNullableIfNeeded(boundExpression5, out temp3, out init3, doNotCaptureLocals: true);
			}
			BoundExpression boundExpression6 = (leftHasValue ? boundExpression5 : boundExpression4);
			BoundExpression boundExpression7 = NullableOfBooleanValue(node.Syntax, flag, type);
			BoundExpression boundExpression8 = MakeTernaryConditionalExpression(node.Syntax, leftHasValue ? NullableValueOrDefault(boundExpression4) : NullableValueOrDefault(boundExpression5), flag ? boundExpression7 : boundExpression6, flag ? boundExpression6 : boundExpression7);
			if (!leftHasValue)
			{
				if (!rightHasValue)
				{
					BoundExpression expr;
					if (flag3)
					{
						expr = init3 ?? boundExpression5;
						init3 = null;
					}
					else
					{
						expr = boundExpression5;
					}
					boundExpression8 = MakeTernaryConditionalExpression(node.Syntax, NullableHasValue(expr), boundExpression8, NullableNull(node.Syntax, type));
				}
				if (!rightHasValue || flag3)
				{
					BoundExpression boundExpression9 = NullableValueOrDefault(boundExpression4);
					BoundExpression expr2;
					if (init3 != null || init2 == null)
					{
						expr2 = boundExpression4;
					}
					else
					{
						expr2 = init2;
						init2 = null;
					}
					boundExpression8 = MakeTernaryConditionalExpression(node.Syntax, MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.AndAlso, NullableHasValue(expr2), flag ? boundExpression9 : new BoundUnaryOperator(node.Syntax, UnaryOperatorKind.Not, boundExpression9, @checked: false, nullableUnderlyingType)), NullableOfBooleanValue(node.Syntax, flag, type), boundExpression8);
				}
			}
			if ((object)temp2 != null || (object)temp3 != null)
			{
				ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
				ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
				if ((object)temp2 != null)
				{
					instance.Add(temp2);
					if (init2 != null)
					{
						instance2.Add(init2);
					}
				}
				if ((object)temp3 != null)
				{
					instance.Add(temp3);
					if (init3 != null)
					{
						instance2.Add(init3);
					}
				}
				boundExpression8 = new BoundSequence(node.Syntax, instance.ToImmutableAndFree(), instance2.ToImmutableAndFree(), boundExpression8, boundExpression8.Type);
			}
			return boundExpression8;
		}

		private BoundExpression RewriteNullableIsOrIsNotOperator(BoundBinaryOperator node)
		{
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			if (_inExpressionLambda)
			{
				return node;
			}
			return RewriteNullableIsOrIsNotOperator((node.OperatorKind & BinaryOperatorKind.OpMask) == BinaryOperatorKind.Is, BoundExpressionExtensions.IsNothingLiteral(left) ? right : left, node.Type);
		}

		private BoundExpression RewriteNullableIsOrIsNotOperator(bool isIs, BoundExpression operand, TypeSymbol resultType)
		{
			if (HasNoValue(operand))
			{
				return new BoundLiteral(operand.Syntax, isIs ? ConstantValue.True : ConstantValue.False, resultType);
			}
			if (HasValue(operand))
			{
				return MakeSequence(operand, new BoundLiteral(operand.Syntax, isIs ? ConstantValue.False : ConstantValue.True, resultType));
			}
			BoundExpression whenNotNull = null;
			BoundExpression whenNull = null;
			if (IsConditionalAccess(operand, out whenNotNull, out whenNull) && HasNoValue(whenNull))
			{
				return UpdateConditionalAccess(operand, RewriteNullableIsOrIsNotOperator(isIs, whenNotNull, resultType), RewriteNullableIsOrIsNotOperator(isIs, whenNull, resultType));
			}
			BoundExpression boundExpression = NullableHasValue(operand);
			if (isIs)
			{
				boundExpression = new BoundUnaryOperator(boundExpression.Syntax, UnaryOperatorKind.Not, boundExpression, @checked: false, resultType);
			}
			return boundExpression;
		}

		private BoundNode RewriteLiftedUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node)
		{
			BoundExpression boundExpression = VisitExpressionNode(node.Left);
			BoundExpression boundExpression2 = VisitExpressionNode(node.Right);
			BoundCall call = node.Call;
			TypeSymbol type = call.Type;
			BoundExpression boundExpression3 = NullableNull(node.Syntax, type);
			bool flag = HasNoValue(boundExpression);
			bool flag2 = HasNoValue(boundExpression2);
			if (flag && flag2)
			{
				return boundExpression3;
			}
			if (flag || flag2)
			{
				return MakeSequence(flag ? boundExpression2 : boundExpression, boundExpression3);
			}
			ArrayBuilder<LocalSymbol> temps = null;
			ArrayBuilder<BoundExpression> inits = null;
			bool num = HasValue(boundExpression);
			bool flag3 = HasValue(boundExpression2);
			BoundExpression hasValueExpr = null;
			BoundExpression boundExpression4;
			BoundExpression item;
			if (num)
			{
				boundExpression4 = NullableValueOrDefault(boundExpression);
				if (flag3)
				{
					item = NullableValueOrDefault(boundExpression2);
				}
				else
				{
					boundExpression4 = CaptureNullableIfNeeded(boundExpression4, ref temps, ref inits, doNotCaptureLocals: true);
					item = ProcessNullableOperand(boundExpression2, out hasValueExpr, ref temps, ref inits, doNotCaptureLocals: true);
				}
			}
			else if (flag3)
			{
				boundExpression4 = ProcessNullableOperand(boundExpression, out hasValueExpr, ref temps, ref inits, doNotCaptureLocals: true);
				item = NullableValueOrDefault(boundExpression2);
				item = CaptureNullableIfNeeded(item, ref temps, ref inits, doNotCaptureLocals: true);
			}
			else
			{
				BoundExpression hasValueExpr2 = null;
				BoundExpression hasValueExpr3 = null;
				boundExpression4 = ProcessNullableOperand(boundExpression, out hasValueExpr2, ref temps, ref inits, doNotCaptureLocals: true);
				item = ProcessNullableOperand(boundExpression2, out hasValueExpr3, ref temps, ref inits, doNotCaptureLocals: true);
				hasValueExpr = MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.And, hasValueExpr2, hasValueExpr3);
			}
			BoundExpression boundExpression5 = call.Update(call.Method, null, call.ReceiverOpt, ImmutableArray.Create(boundExpression4, item), default(BitVector), call.ConstantValueOpt, call.IsLValue, call.SuppressObjectClone, call.Method.ReturnType);
			if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(boundExpression5.Type, type))
			{
				boundExpression5 = WrapInNullable(boundExpression5, type);
			}
			if (num && flag3)
			{
				return boundExpression5;
			}
			BoundExpression boundExpression6 = MakeTernaryConditionalExpression(node.Syntax, hasValueExpr, boundExpression5, boundExpression3);
			if (temps != null)
			{
				boundExpression6 = new BoundSequence(node.Syntax, temps.ToImmutableAndFree(), inits.ToImmutableAndFree(), boundExpression6, boundExpression6.Type);
			}
			return boundExpression6;
		}

		private BoundExpression ApplyUnliftedBinaryOp(BoundBinaryOperator originalOperator, BoundExpression left, BoundExpression right)
		{
			BinaryOperatorKind binaryOpKind = originalOperator.OperatorKind & ~BinaryOperatorKind.Lifted;
			return MakeBinaryExpression(originalOperator.Syntax, binaryOpKind, left, right, originalOperator.Checked, TypeSymbolExtensions.GetNullableUnderlyingType(originalOperator.Type));
		}

		public override BoundNode VisitBlock(BoundBlock node)
		{
			BoundBlock trueOriginal = node;
			if (!node.Locals.IsEmpty)
			{
				ArrayBuilder<LocalSymbol> arrayBuilder = null;
				int num = node.Locals.Length - 1;
				int i;
				for (i = 0; i <= num; i++)
				{
					if (node.Locals[i].IsStatic)
					{
						arrayBuilder = ArrayBuilder<LocalSymbol>.GetInstance();
						break;
					}
				}
				if (arrayBuilder != null)
				{
					arrayBuilder.AddRange(node.Locals, i);
					int num2 = i + 1;
					int num3 = node.Locals.Length - 1;
					for (i = num2; i <= num3; i++)
					{
						if (!node.Locals[i].IsStatic)
						{
							arrayBuilder.Add(node.Locals[i]);
						}
					}
					node = node.Update(node.StatementListSyntax, arrayBuilder.ToImmutableAndFree(), node.Statements);
				}
			}
			if (this.Instrument)
			{
				ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
				ImmutableArray<BoundStatement>.Enumerator enumerator = node.Statements.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundStatement current = enumerator.Current;
					if (Visit(current) is BoundStatement item)
					{
						instance.Add(item);
					}
				}
				LocalSymbol synthesizedLocal = null;
				BoundStatement boundStatement = _instrumenterOpt.CreateBlockPrologue(trueOriginal, node, ref synthesizedLocal);
				if (boundStatement != null)
				{
					instance.Insert(0, boundStatement);
				}
				return new BoundBlock(node.Syntax, node.StatementListSyntax, ((object)synthesizedLocal == null) ? node.Locals : node.Locals.Add(synthesizedLocal), instance.ToImmutableAndFree());
			}
			return base.VisitBlock(node);
		}

		public override BoundNode VisitCall(BoundCall node)
		{
			BoundExpression receiver = node.ReceiverOpt;
			MethodSymbol method = node.Method;
			ImmutableArray<BoundExpression> arguments = node.Arguments;
			if ((object)method == Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Strings__AscWCharInt32))
			{
				return new BoundConversion(node.Syntax, VisitExpressionNode(arguments[0]), ConversionKind.WideningNumeric, @checked: false, explicitCastInCode: true, node.Type);
			}
			WellKnownMember wellKnownMember = WellKnownMember.Count;
			if ((object)method == Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Interaction__CallByName))
			{
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__CallByName;
			}
			else if ((object)method.ContainingSymbol == Compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_Information))
			{
				if ((object)method == Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Information__IsNumeric))
				{
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__IsNumeric;
				}
				else if ((object)method == Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Information__SystemTypeName))
				{
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__SystemTypeName;
				}
				else if ((object)method == Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Information__TypeName))
				{
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__TypeName;
				}
				else if ((object)method == Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Information__VbTypeName))
				{
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__VbTypeName;
				}
			}
			if (wellKnownMember != WellKnownMember.Count)
			{
				MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(wellKnownMember);
				if ((object)methodSymbol != null && !ReportMissingOrBadRuntimeHelper(node, wellKnownMember, methodSymbol))
				{
					method = methodSymbol;
				}
			}
			UpdateMethodAndArgumentsIfReducedFromMethod(ref method, ref receiver, ref arguments);
			ImmutableArray<SynthesizedLocal> temporaries = default(ImmutableArray<SynthesizedLocal>);
			ImmutableArray<BoundExpression> copyBack = default(ImmutableArray<BoundExpression>);
			bool suppressObjectClone = node.SuppressObjectClone || (object)method == Compilation.GetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__GetObjectValueObject);
			receiver = VisitExpressionNode(receiver);
			node = node.Update(method, null, receiver, RewriteCallArguments(arguments, method.Parameters, out temporaries, out copyBack, suppressObjectClone), node.DefaultArguments, null, node.IsLValue, suppressObjectClone: true, node.Type);
			if (!copyBack.IsDefault)
			{
				return GenerateSequenceValueSideEffects(_currentMethodOrLambda, node, StaticCast<LocalSymbol>.From(temporaries), copyBack);
			}
			if (!temporaries.IsDefault)
			{
				if (method.IsSub)
				{
					return new BoundSequence(node.Syntax, StaticCast<LocalSymbol>.From(temporaries), ImmutableArray.Create((BoundExpression)node), null, node.Type);
				}
				return new BoundSequence(node.Syntax, StaticCast<LocalSymbol>.From(temporaries), ImmutableArray<BoundExpression>.Empty, node, node.Type);
			}
			return node;
		}

		private static void UpdateMethodAndArgumentsIfReducedFromMethod(ref MethodSymbol method, ref BoundExpression receiver, ref ImmutableArray<BoundExpression> arguments)
		{
			if (receiver == null)
			{
				return;
			}
			MethodSymbol callsiteReducedFromMethod = method.CallsiteReducedFromMethod;
			if ((object)callsiteReducedFromMethod != null)
			{
				if (arguments.IsEmpty)
				{
					arguments = ImmutableArray.Create(receiver);
				}
				else
				{
					BoundExpression[] array = new BoundExpression[arguments.Length + 1];
					array[0] = receiver;
					arguments.CopyTo(array, 1);
					arguments = array.AsImmutableOrNull();
				}
				receiver = null;
				method = callsiteReducedFromMethod;
			}
		}

		public override BoundNode VisitByRefArgumentWithCopyBack(BoundByRefArgumentWithCopyBack node)
		{
			throw ExceptionUtilities.Unreachable;
		}

		private ImmutableArray<BoundExpression> RewriteCallArguments(ImmutableArray<BoundExpression> arguments, ImmutableArray<ParameterSymbol> parameters, out ImmutableArray<SynthesizedLocal> temporaries, out ImmutableArray<BoundExpression> copyBack, bool suppressObjectClone)
		{
			temporaries = default(ImmutableArray<SynthesizedLocal>);
			copyBack = default(ImmutableArray<BoundExpression>);
			if (arguments.IsEmpty)
			{
				return arguments;
			}
			ArrayBuilder<SynthesizedLocal> tempsArray = null;
			ArrayBuilder<BoundExpression> copyBackArray = null;
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			bool flag = false;
			int num = 0;
			ImmutableArray<BoundExpression>.Enumerator enumerator = arguments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				BoundExpression boundExpression;
				if (current.Kind == BoundKind.ByRefArgumentWithCopyBack)
				{
					boundExpression = RewriteByRefArgumentWithCopyBack((BoundByRefArgumentWithCopyBack)current, ref tempsArray, ref copyBackArray);
				}
				else
				{
					boundExpression = VisitExpressionNode(current);
					if (parameters[num].IsByRef && !current.IsLValue && !_inExpressionLambda)
					{
						boundExpression = PassArgAsTempClone(current, boundExpression, ref tempsArray);
					}
				}
				if (!suppressObjectClone && (!parameters[num].IsByRef || !boundExpression.IsLValue))
				{
					boundExpression = GenerateObjectCloneIfNeeded(current, boundExpression);
				}
				if (boundExpression != current)
				{
					flag = true;
				}
				instance.Add(boundExpression);
				num++;
			}
			if (flag)
			{
				arguments = instance.ToImmutable();
			}
			instance.Free();
			if (tempsArray != null)
			{
				temporaries = tempsArray.ToImmutableAndFree();
			}
			if (copyBackArray != null)
			{
				copyBackArray.ReverseContents();
				copyBack = copyBackArray.ToImmutableAndFree();
			}
			return arguments;
		}

		private BoundExpression PassArgAsTempClone(BoundExpression argument, BoundExpression rewrittenArgument, ref ArrayBuilder<SynthesizedLocal> tempsArray)
		{
			if (tempsArray == null)
			{
				tempsArray = ArrayBuilder<SynthesizedLocal>.GetInstance();
			}
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, rewrittenArgument.Type, SynthesizedLocalKind.LoweringTemp);
			tempsArray.Add(synthesizedLocal);
			BoundLocal boundLocal = new BoundLocal(rewrittenArgument.Syntax, synthesizedLocal, synthesizedLocal.Type);
			BoundExpression item = new BoundAssignmentOperator(rewrittenArgument.Syntax, boundLocal, GenerateObjectCloneIfNeeded(argument, rewrittenArgument), suppressObjectClone: true, rewrittenArgument.Type);
			return new BoundSequence(rewrittenArgument.Syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(item), boundLocal, rewrittenArgument.Type);
		}

		private BoundExpression RewriteByRefArgumentWithCopyBack(BoundByRefArgumentWithCopyBack argument, ref ArrayBuilder<SynthesizedLocal> tempsArray, ref ArrayBuilder<BoundExpression> copyBackArray)
		{
			BoundExpression boundExpression = argument.OriginalArgument;
			if (BoundExpressionExtensions.IsPropertyOrXmlPropertyAccess(boundExpression))
			{
				boundExpression = BoundExpressionExtensions.SetAccessKind(boundExpression, PropertyAccessKind.Unknown);
			}
			else if (BoundExpressionExtensions.IsLateBound(boundExpression))
			{
				boundExpression = BoundExpressionExtensions.SetLateBoundAccessKind(boundExpression, LateBoundAccessKind.Unknown);
			}
			if (_inExpressionLambda)
			{
				if (BoundExpressionExtensions.IsPropertyOrXmlPropertyAccess(boundExpression))
				{
					boundExpression = BoundExpressionExtensions.SetAccessKind(boundExpression, PropertyAccessKind.Get);
				}
				else if (BoundExpressionExtensions.IsLateBound(boundExpression))
				{
					boundExpression = BoundExpressionExtensions.SetLateBoundAccessKind(boundExpression, LateBoundAccessKind.Get);
				}
				if (boundExpression.IsLValue)
				{
					boundExpression = boundExpression.MakeRValue();
				}
				AddPlaceholderReplacement(argument.InPlaceholder, VisitExpressionNode(boundExpression));
				BoundExpression result = VisitExpression(argument.InConversion);
				RemovePlaceholderReplacement(argument.InPlaceholder);
				return result;
			}
			if (tempsArray == null)
			{
				tempsArray = ArrayBuilder<SynthesizedLocal>.GetInstance();
			}
			if (copyBackArray == null)
			{
				copyBackArray = ArrayBuilder<BoundExpression>.GetInstance();
			}
			UseTwiceRewriter.Result result2 = UseTwiceRewriter.UseTwice(_currentMethodOrLambda, boundExpression, tempsArray);
			BoundExpression expression;
			BoundExpression boundExpression2;
			if (BoundExpressionExtensions.IsPropertyOrXmlPropertyAccess(boundExpression))
			{
				expression = BoundExpressionExtensions.SetAccessKind(result2.First, PropertyAccessKind.Get).MakeRValue();
				boundExpression2 = BoundExpressionExtensions.SetAccessKind(result2.Second, BoundExpressionExtensions.IsPropertyReturnsByRef(boundExpression) ? PropertyAccessKind.Get : PropertyAccessKind.Set);
			}
			else if (BoundExpressionExtensions.IsLateBound(boundExpression))
			{
				expression = BoundExpressionExtensions.SetLateBoundAccessKind(result2.First, LateBoundAccessKind.Get);
				boundExpression2 = BoundExpressionExtensions.SetLateBoundAccessKind(result2.Second, LateBoundAccessKind.Set);
			}
			else
			{
				expression = result2.First.MakeRValue();
				boundExpression2 = result2.Second;
			}
			AddPlaceholderReplacement(argument.InPlaceholder, VisitExpressionNode(expression));
			BoundExpression right = VisitAndGenerateObjectCloneIfNeeded(argument.InConversion);
			RemovePlaceholderReplacement(argument.InPlaceholder);
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, argument.Type, SynthesizedLocalKind.LoweringTemp);
			tempsArray.Add(synthesizedLocal);
			BoundLocal boundLocal = new BoundLocal(argument.Syntax, synthesizedLocal, synthesizedLocal.Type);
			BoundExpression item = new BoundAssignmentOperator(argument.Syntax, boundLocal, right, suppressObjectClone: true, argument.Type);
			AddPlaceholderReplacement(argument.OutPlaceholder, boundLocal.MakeRValue());
			BoundExpression item2;
			if (!BoundExpressionExtensions.IsLateBound(boundExpression))
			{
				item2 = (BoundExpression)VisitAssignmentOperator(new BoundAssignmentOperator(argument.Syntax, boundExpression2, argument.OutConversion, suppressObjectClone: false));
				RemovePlaceholderReplacement(argument.OutPlaceholder);
			}
			else
			{
				BoundExpression assignmentValue = VisitExpressionNode(argument.OutConversion);
				RemovePlaceholderReplacement(argument.OutPlaceholder);
				if (boundExpression2.Kind == BoundKind.LateMemberAccess)
				{
					item2 = LateSet(boundExpression2.Syntax, (BoundLateMemberAccess)base.VisitLateMemberAccess((BoundLateMemberAccess)boundExpression2), assignmentValue, default(ImmutableArray<BoundExpression>), default(ImmutableArray<string>), isCopyBack: true);
				}
				else
				{
					BoundLateInvocation boundLateInvocation = (BoundLateInvocation)boundExpression2;
					if (boundLateInvocation.Member.Kind == BoundKind.LateMemberAccess)
					{
						item2 = LateSet(boundLateInvocation.Syntax, (BoundLateMemberAccess)base.VisitLateMemberAccess((BoundLateMemberAccess)boundLateInvocation.Member), assignmentValue, VisitList(boundLateInvocation.ArgumentsOpt), boundLateInvocation.ArgumentNamesOpt, isCopyBack: true);
					}
					else
					{
						boundLateInvocation = boundLateInvocation.Update(VisitExpressionNode(boundLateInvocation.Member), VisitList(boundLateInvocation.ArgumentsOpt), boundLateInvocation.ArgumentNamesOpt, boundLateInvocation.AccessKind, boundLateInvocation.MethodOrPropertyGroupOpt, boundLateInvocation.Type);
						item2 = LateIndexSet(boundLateInvocation.Syntax, boundLateInvocation, assignmentValue, isCopyBack: true);
					}
				}
			}
			copyBackArray.Add(item2);
			return new BoundSequence(argument.Syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(item), boundLocal, argument.Type);
		}

		private static bool ShouldCaptureConditionalAccessReceiver(BoundExpression receiver)
		{
			return receiver.Kind switch
			{
				BoundKind.MeReference => false, 
				BoundKind.Parameter => ((BoundParameter)receiver).ParameterSymbol.IsByRef, 
				BoundKind.Local => ((BoundLocal)receiver).LocalSymbol.IsByRef, 
				_ => !BoundExpressionExtensions.IsDefaultValue(receiver), 
			};
		}

		public override BoundNode VisitConditionalAccess(BoundConditionalAccess node)
		{
			BoundExpression boundExpression = VisitExpressionNode(node.Receiver);
			TypeSymbol type = boundExpression.Type;
			int placeholderId = 0;
			LocalSymbol localSymbol = null;
			BoundExpression boundExpression2 = null;
			bool flag = true;
			bool flag2 = true;
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			BoundExpression receiverOrCondition;
			BoundExpression value;
			bool captureReceiver;
			if (TypeSymbolExtensions.IsNullableType(type))
			{
				if (HasNoValue(boundExpression))
				{
					receiverOrCondition = null;
					flag = false;
					value = null;
				}
				else if (HasValue(boundExpression))
				{
					receiverOrCondition = null;
					flag2 = false;
					value = NullableValueOrDefault(boundExpression);
				}
				else
				{
					BoundExpression expr;
					if (ShouldCaptureConditionalAccessReceiver(boundExpression))
					{
						localSymbol = new SynthesizedLocal(_currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp);
						boundExpression2 = syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(localSymbol, isLValue: true), boundExpression.MakeRValue());
						expr = syntheticBoundNodeFactory.Local(localSymbol, isLValue: true);
						value = syntheticBoundNodeFactory.Local(localSymbol, isLValue: true);
					}
					else
					{
						expr = boundExpression;
						value = boundExpression;
					}
					receiverOrCondition = NullableHasValue(expr);
					value = NullableValueOrDefault(value);
				}
				captureReceiver = false;
			}
			else if (boundExpression.IsConstant)
			{
				receiverOrCondition = null;
				captureReceiver = false;
				if (boundExpression.ConstantValueOpt.IsNothing)
				{
					value = null;
					flag = false;
				}
				else
				{
					value = boundExpression.MakeRValue();
					flag2 = false;
				}
			}
			else
			{
				receiverOrCondition = boundExpression;
				captureReceiver = ShouldCaptureConditionalAccessReceiver(boundExpression);
				_conditionalAccessReceiverPlaceholderId++;
				placeholderId = _conditionalAccessReceiverPlaceholderId;
				value = new BoundConditionalAccessReceiverPlaceholder(node.Placeholder.Syntax, placeholderId, node.Placeholder.Type);
			}
			TypeSymbol type2 = node.AccessExpression.Type;
			BoundExpression boundExpression3;
			if (flag)
			{
				AddPlaceholderReplacement(node.Placeholder, value);
				boundExpression3 = VisitExpressionNode(node.AccessExpression);
				RemovePlaceholderReplacement(node.Placeholder);
			}
			else
			{
				boundExpression3 = null;
			}
			BoundExpression boundExpression4;
			if (TypeSymbolExtensions.IsVoidType(node.Type))
			{
				boundExpression4 = null;
			}
			else
			{
				if (flag && !TypeSymbolExtensions.IsNullableType(type2) && type2.IsValueType)
				{
					boundExpression3 = WrapInNullable(boundExpression3, node.Type);
				}
				boundExpression4 = ((!flag2) ? null : (TypeSymbolExtensions.IsNullableType(node.Type) ? NullableNull(node.Syntax, node.Type) : syntheticBoundNodeFactory.Null(node.Type)));
			}
			BoundExpression boundExpression5 = (flag ? ((!flag2) ? boundExpression3 : new BoundLoweredConditionalAccess(node.Syntax, receiverOrCondition, captureReceiver, placeholderId, boundExpression3, boundExpression4, node.Type)) : ((boundExpression4 == null) ? new BoundSequence(node.Syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray<BoundExpression>.Empty, null, node.Type) : boundExpression4));
			if ((object)localSymbol != null)
			{
				boundExpression5 = ((!TypeSymbolExtensions.IsVoidType(boundExpression5.Type)) ? new BoundSequence(node.Syntax, ImmutableArray.Create(localSymbol), ImmutableArray.Create(boundExpression2), boundExpression5, boundExpression5.Type) : new BoundSequence(node.Syntax, ImmutableArray.Create(localSymbol), ImmutableArray.Create(boundExpression2, boundExpression5), null, boundExpression5.Type));
			}
			return boundExpression5;
		}

		private static bool IsConditionalAccess(BoundExpression operand, out BoundExpression whenNotNull, out BoundExpression whenNull)
		{
			if (operand.Kind == BoundKind.Sequence)
			{
				BoundSequence boundSequence = (BoundSequence)operand;
				if (boundSequence.ValueOpt == null)
				{
					whenNotNull = null;
					whenNull = null;
					return false;
				}
				operand = boundSequence.ValueOpt;
			}
			if (operand.Kind == BoundKind.LoweredConditionalAccess)
			{
				BoundLoweredConditionalAccess boundLoweredConditionalAccess = (BoundLoweredConditionalAccess)operand;
				whenNotNull = boundLoweredConditionalAccess.WhenNotNull;
				whenNull = boundLoweredConditionalAccess.WhenNullOpt;
				return true;
			}
			whenNotNull = null;
			whenNull = null;
			return false;
		}

		private static BoundExpression UpdateConditionalAccess(BoundExpression operand, BoundExpression whenNotNull, BoundExpression whenNull)
		{
			BoundSequence boundSequence;
			if (operand.Kind == BoundKind.Sequence)
			{
				boundSequence = (BoundSequence)operand;
				operand = boundSequence.ValueOpt;
			}
			else
			{
				boundSequence = null;
			}
			BoundLoweredConditionalAccess boundLoweredConditionalAccess = (BoundLoweredConditionalAccess)operand;
			operand = boundLoweredConditionalAccess.Update(boundLoweredConditionalAccess.ReceiverOrCondition, boundLoweredConditionalAccess.CaptureReceiver, boundLoweredConditionalAccess.PlaceholderId, whenNotNull, whenNull, whenNotNull.Type);
			if (boundSequence == null)
			{
				return operand;
			}
			return boundSequence.Update(boundSequence.Locals, boundSequence.SideEffects, operand, operand.Type);
		}

		public override BoundNode VisitBinaryConditionalExpression(BoundBinaryConditionalExpression node)
		{
			if (_inExpressionLambda)
			{
				return RewriteBinaryConditionalExpressionInExpressionLambda(node);
			}
			if ((object)node.TestExpression.Type != null && TypeSymbolExtensions.IsNullableType(node.TestExpression.Type))
			{
				return RewriteNullableBinaryConditionalExpression(node);
			}
			BoundExpression convertedTestExpression = node.ConvertedTestExpression;
			if (convertedTestExpression == null)
			{
				return TransformReferenceOrUnconstrainedRewrittenBinaryConditionalExpression(base.VisitBinaryConditionalExpression(node));
			}
			if (convertedTestExpression.Kind == BoundKind.Conversion)
			{
				ConversionKind conversionKind = ((BoundConversion)node.ConvertedTestExpression).ConversionKind;
				if (Conversions.IsWideningConversion(conversionKind) && Conversions.IsCLRPredefinedConversion(conversionKind) && (conversionKind & ConversionKind.TypeParameter) == 0)
				{
					return TransformReferenceOrUnconstrainedRewrittenBinaryConditionalExpression(node.Update(VisitExpressionNode(node.TestExpression), null, null, VisitExpressionNode(node.ElseExpression), node.ConstantValueOpt, node.Type));
				}
			}
			BoundValuePlaceholderBase testExpressionPlaceholder = node.TestExpressionPlaceholder;
			BoundExpression boundExpression = VisitExpressionNode(node.TestExpression);
			TypeSymbol type = boundExpression.Type;
			SynthesizedLocal synthesizedLocal = null;
			BoundExpression boundExpression2 = null;
			BoundKind kind = boundExpression.Kind;
			if (kind == BoundKind.Literal || kind == BoundKind.Local || kind == BoundKind.Parameter)
			{
				boundExpression2 = boundExpression;
			}
			else
			{
				synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp);
				boundExpression2 = new BoundLocal(boundExpression.Syntax, synthesizedLocal, isLValue: false, type);
			}
			if (testExpressionPlaceholder != null)
			{
				AddPlaceholderReplacement(testExpressionPlaceholder, boundExpression2);
			}
			BoundExpression whenTrue = VisitExpressionNode(node.ConvertedTestExpression);
			if (testExpressionPlaceholder != null)
			{
				RemovePlaceholderReplacement(testExpressionPlaceholder);
			}
			BoundExpression condition = boundExpression2;
			if ((object)synthesizedLocal != null)
			{
				condition = new BoundAssignmentOperator(boundExpression.Syntax, new BoundLocal(boundExpression.Syntax, synthesizedLocal, isLValue: true, type), boundExpression, suppressObjectClone: true, type);
			}
			BoundExpression boundExpression3 = TransformRewrittenTernaryConditionalExpression(new BoundTernaryConditionalExpression(node.Syntax, condition, whenTrue, VisitExpressionNode(node.ElseExpression), node.ConstantValueOpt, node.Type));
			if ((object)synthesizedLocal == null)
			{
				return boundExpression3;
			}
			return new BoundSequence(boundExpression3.Syntax, ImmutableArray.Create((LocalSymbol)synthesizedLocal), ImmutableArray<BoundExpression>.Empty, boundExpression3, boundExpression3.Type);
		}

		private BoundExpression RewriteBinaryConditionalExpressionInExpressionLambda(BoundBinaryConditionalExpression node)
		{
			BoundExpression testExpression = node.TestExpression;
			TypeSymbol type = testExpression.Type;
			BoundExpression boundExpression = VisitExpression(testExpression);
			BoundExpression convertedTestExpression = null;
			BoundExpression convertedTestExpression2 = node.ConvertedTestExpression;
			if (convertedTestExpression2 == null)
			{
				if (!TypeSymbolExtensions.IsNullableOfBoolean(type) && TypeSymbolExtensions.IsNullableType(type))
				{
					convertedTestExpression = new BoundConversion(boundExpression.Syntax, boundExpression, ConversionKind.WideningNullable, @checked: false, explicitCastInCode: false, TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type));
				}
			}
			else
			{
				convertedTestExpression = VisitExpressionNode(convertedTestExpression2, node.TestExpressionPlaceholder, TypeSymbolExtensions.IsNullableType(type) ? new BoundConversion(boundExpression.Syntax, boundExpression, ConversionKind.WideningNullable, @checked: false, explicitCastInCode: false, TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type)) : boundExpression);
			}
			BoundExpression elseExpression = VisitExpression(node.ElseExpression);
			return node.Update(boundExpression, convertedTestExpression, null, elseExpression, node.ConstantValueOpt, node.Type);
		}

		private static BoundNode TransformReferenceOrUnconstrainedRewrittenBinaryConditionalExpression(BoundNode node)
		{
			if (node.Kind == BoundKind.BinaryConditionalExpression)
			{
				return TransformReferenceOrUnconstrainedRewrittenBinaryConditionalExpression((BoundBinaryConditionalExpression)node);
			}
			return node;
		}

		private static BoundExpression TransformReferenceOrUnconstrainedRewrittenBinaryConditionalExpression(BoundBinaryConditionalExpression node)
		{
			if (node.HasErrors)
			{
				return node;
			}
			BoundExpression testExpression = node.TestExpression;
			BoundExpression elseExpression = node.ElseExpression;
			if (testExpression.IsConstant && TypeSymbol.Equals(testExpression.Type, elseExpression.Type, TypeCompareKind.ConsiderEverything))
			{
				if (testExpression.ConstantValueOpt.IsNothing)
				{
					return node.ElseExpression;
				}
				return testExpression;
			}
			return node;
		}

		private BoundNode RewriteNullableBinaryConditionalExpression(BoundBinaryConditionalExpression node)
		{
			BoundExpression boundExpression = VisitExpressionNode(node.TestExpression);
			if (HasValue(boundExpression))
			{
				return MakeResultFromNonNullLeft(boundExpression, node.ConvertedTestExpression, node.TestExpressionPlaceholder);
			}
			BoundExpression boundExpression2 = VisitExpressionNode(node.ElseExpression);
			if (HasNoValue(boundExpression))
			{
				return boundExpression2;
			}
			BoundExpression whenNotNull = null;
			BoundExpression whenNull = null;
			if (IsConditionalAccess(boundExpression, out whenNotNull, out whenNull) && HasNoValue(whenNull) && HasValue(whenNotNull))
			{
				return UpdateConditionalAccess(boundExpression, MakeResultFromNonNullLeft(whenNotNull, node.ConvertedTestExpression, node.TestExpressionPlaceholder), boundExpression2);
			}
			if (TypeSymbolExtensions.IsNullableType(boundExpression.Type) && BoundExpressionExtensions.IsDefaultValue(boundExpression2) && TypeSymbolExtensions.IsSameTypeIgnoringAll(boundExpression2.Type, TypeSymbolExtensions.GetNullableUnderlyingType(boundExpression.Type)))
			{
				return NullableValueOrDefault(boundExpression);
			}
			SynthesizedLocal temp = null;
			BoundExpression init = null;
			BoundExpression boundExpression3 = CaptureNullableIfNeeded(boundExpression, out temp, out init, doNotCaptureLocals: true);
			BoundExpression condition = NullableHasValue(init ?? boundExpression3);
			BoundExpression boundExpression4 = MakeTernaryConditionalExpression(whenTrue: (node.ConvertedTestExpression == null) ? NullableValueOrDefault(boundExpression3) : ((!TypeSymbolExtensions.IsSameTypeIgnoringAll(boundExpression3.Type, node.ConvertedTestExpression.Type)) ? VisitExpressionNode(node.ConvertedTestExpression, node.TestExpressionPlaceholder, NullableValueOrDefault(boundExpression3)) : boundExpression3), syntax: node.Syntax, condition: condition, whenFalse: boundExpression2);
			if ((object)temp != null)
			{
				boundExpression4 = new BoundSequence(node.Syntax, ImmutableArray.Create((LocalSymbol)temp), ImmutableArray<BoundExpression>.Empty, boundExpression4, boundExpression4.Type);
			}
			return boundExpression4;
		}

		private BoundExpression MakeResultFromNonNullLeft(BoundExpression rewrittenLeft, BoundExpression convertedTestExpression, BoundRValuePlaceholder testExpressionPlaceholder)
		{
			if (convertedTestExpression == null)
			{
				return NullableValueOrDefault(rewrittenLeft);
			}
			if (TypeSymbolExtensions.IsSameTypeIgnoringAll(rewrittenLeft.Type, convertedTestExpression.Type))
			{
				return rewrittenLeft;
			}
			return VisitExpressionNode(convertedTestExpression, testExpressionPlaceholder, NullableValueOrDefault(rewrittenLeft));
		}

		private BoundExpression VisitExpressionNode(BoundExpression node, BoundValuePlaceholderBase placeholder, BoundExpression placeholderSubstitute)
		{
			if (placeholder != null)
			{
				AddPlaceholderReplacement(placeholder, placeholderSubstitute);
			}
			BoundExpression result = VisitExpressionNode(node);
			if (placeholder != null)
			{
				RemovePlaceholderReplacement(placeholder);
			}
			return result;
		}

		public override BoundNode VisitTernaryConditionalExpression(BoundTernaryConditionalExpression node)
		{
			return TransformRewrittenTernaryConditionalExpression((BoundTernaryConditionalExpression)base.VisitTernaryConditionalExpression(node));
		}

		private static BoundExpression TransformRewrittenTernaryConditionalExpression(BoundTernaryConditionalExpression node)
		{
			if (node.Condition.IsConstant && node.WhenTrue.IsConstant && node.WhenFalse.IsConstant)
			{
				return (node.Condition.ConstantValueOpt.IsBoolean ? node.Condition.ConstantValueOpt.BooleanValue : node.Condition.ConstantValueOpt.IsString) ? node.WhenTrue : node.WhenFalse;
			}
			return node;
		}

		private BoundExpression RewriteConstant(BoundExpression node, ConstantValue constantValue)
		{
			if (!_inExpressionLambda && !node.HasErrors)
			{
				if (constantValue.Discriminator == ConstantValueTypeDiscriminator.Decimal)
				{
					return RewriteDecimalConstant(node, constantValue, _topMethod, _diagnostics);
				}
				if (constantValue.Discriminator == ConstantValueTypeDiscriminator.DateTime)
				{
					return RewriteDateConstant(node, constantValue, _topMethod, _diagnostics);
				}
			}
			return (node.Kind == BoundKind.Literal) ? node : new BoundLiteral(node.Syntax, constantValue, node.Type, constantValue.IsBad);
		}

		private static BoundExpression RewriteDecimalConstant(BoundExpression node, ConstantValue nodeValue, MethodSymbol currentMethod, BindingDiagnosticBag diagnostics)
		{
			AssemblySymbol containingAssembly = currentMethod.ContainingAssembly;
			nodeValue.DecimalValue.GetBits(out var isNegative, out var scale, out var low, out var mid, out var high);
			if (scale == 0 && (long)high == 0L && (long)mid == 0L)
			{
				if (currentMethod.MethodKind != MethodKind.StaticConstructor || currentMethod.ContainingType.SpecialType != SpecialType.System_Decimal)
				{
					Symbol symbol = null;
					if ((long)low == 0L)
					{
						symbol = containingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__Zero);
					}
					else if ((ulong)low == 1)
					{
						symbol = ((!isNegative) ? containingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__One) : containingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__MinusOne));
					}
					if ((object)symbol != null)
					{
						UseSiteInfo<AssemblySymbol> useSiteInfoForMemberAndContainingType = Binder.GetUseSiteInfoForMemberAndContainingType(symbol);
						if (useSiteInfoForMemberAndContainingType.DiagnosticInfo == null)
						{
							FieldSymbol fieldSymbol = (FieldSymbol)symbol;
							diagnostics.AddDependencies(useSiteInfoForMemberAndContainingType);
							return new BoundFieldAccess(node.Syntax, null, fieldSymbol, isLValue: false, fieldSymbol.Type);
						}
					}
				}
				long num = low;
				if (isNegative)
				{
					num = -num;
				}
				MethodSymbol methodSymbol = (MethodSymbol)containingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__CtorInt64);
				if ((object)methodSymbol != null)
				{
					UseSiteInfo<AssemblySymbol> useSiteInfoForMemberAndContainingType2 = Binder.GetUseSiteInfoForMemberAndContainingType(methodSymbol);
					if (useSiteInfoForMemberAndContainingType2.DiagnosticInfo == null)
					{
						diagnostics.AddDependencies(useSiteInfoForMemberAndContainingType2);
						return new BoundObjectCreationExpression(node.Syntax, methodSymbol, new BoundExpression[1]
						{
							new BoundLiteral(node.Syntax, ConstantValue.Create(num), methodSymbol.Parameters[0].Type)
						}.AsImmutableOrNull(), null, node.Type);
					}
				}
			}
			MethodSymbol methodSymbol2 = null;
			methodSymbol2 = (MethodSymbol)containingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__CtorInt32Int32Int32BooleanByte);
			if (!ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_Decimal__CtorInt32Int32Int32BooleanByte, methodSymbol2, diagnostics))
			{
				return new BoundObjectCreationExpression(node.Syntax, methodSymbol2, ImmutableArray.Create(new BoundExpression[5]
				{
					new BoundLiteral(node.Syntax, ConstantValue.Create(CompileTimeCalculations.UncheckedCInt(low)), methodSymbol2.Parameters[0].Type),
					new BoundLiteral(node.Syntax, ConstantValue.Create(CompileTimeCalculations.UncheckedCInt(mid)), methodSymbol2.Parameters[1].Type),
					new BoundLiteral(node.Syntax, ConstantValue.Create(CompileTimeCalculations.UncheckedCInt(high)), methodSymbol2.Parameters[2].Type),
					new BoundLiteral(node.Syntax, ConstantValue.Create(isNegative), methodSymbol2.Parameters[3].Type),
					new BoundLiteral(node.Syntax, ConstantValue.Create(scale), methodSymbol2.Parameters[4].Type)
				}), null, node.Type);
			}
			return node;
		}

		private static BoundExpression RewriteDateConstant(BoundExpression node, ConstantValue nodeValue, MethodSymbol currentMethod, BindingDiagnosticBag diagnostics)
		{
			AssemblySymbol containingAssembly = currentMethod.ContainingAssembly;
			DateTime dateTimeValue = nodeValue.DateTimeValue;
			if (DateTime.Compare(dateTimeValue, DateTime.MinValue) == 0 && (currentMethod.MethodKind != MethodKind.StaticConstructor || currentMethod.ContainingType.SpecialType != SpecialType.System_DateTime))
			{
				FieldSymbol fieldSymbol = (FieldSymbol)containingAssembly.GetSpecialTypeMember(SpecialMember.System_DateTime__MinValue);
				if ((object)fieldSymbol != null)
				{
					UseSiteInfo<AssemblySymbol> useSiteInfoForMemberAndContainingType = Binder.GetUseSiteInfoForMemberAndContainingType(fieldSymbol);
					if (useSiteInfoForMemberAndContainingType.DiagnosticInfo == null)
					{
						diagnostics.AddDependencies(useSiteInfoForMemberAndContainingType);
						return new BoundFieldAccess(node.Syntax, null, fieldSymbol, isLValue: false, fieldSymbol.Type);
					}
				}
			}
			MethodSymbol methodSymbol = (MethodSymbol)containingAssembly.GetSpecialTypeMember(SpecialMember.System_DateTime__CtorInt64);
			if (!ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_DateTime__CtorInt64, methodSymbol, diagnostics))
			{
				return new BoundObjectCreationExpression(node.Syntax, methodSymbol, new BoundExpression[1]
				{
					new BoundLiteral(node.Syntax, ConstantValue.Create(dateTimeValue.Ticks), methodSymbol.Parameters[0].Type)
				}.AsImmutableOrNull(), null, node.Type);
			}
			return node;
		}

		public override BoundNode VisitContinueStatement(BoundContinueStatement node)
		{
			BoundStatement boundStatement = new BoundGotoStatement(node.Syntax, node.Label, null);
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement = Concat(RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax), boundStatement);
			}
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentContinueStatement(node, boundStatement);
			}
			return boundStatement;
		}

		public override BoundNode VisitConversion(BoundConversion node)
		{
			if (!_inExpressionLambda && Conversions.IsIdentityConversion(node.ConversionKind))
			{
				BoundExpression boundExpression = (BoundExpression)Visit(node.Operand);
				if (node.ExplicitCastInCode && IsFloatingPointExpressionOfUnknownPrecision(boundExpression))
				{
					boundExpression = node.Update(boundExpression, ConversionKind.Identity, @checked: false, explicitCastInCode: true, node.ConstantValueOpt, node.ExtendedInfoOpt, node.Type);
				}
				return boundExpression;
			}
			if (node.Operand.Kind == BoundKind.UserDefinedConversion)
			{
				if (_inExpressionLambda)
				{
					return node.Update((BoundExpression)Visit(node.Operand), node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ExtendedInfoOpt, node.Type);
				}
				if ((node.ConversionKind & ConversionKind.Nullable) != 0)
				{
					return RewriteNullableUserDefinedConversion((BoundUserDefinedConversion)node.Operand);
				}
				return Visit(((BoundUserDefinedConversion)node.Operand).UnderlyingExpression);
			}
			if ((((object)node.Type != null && TypeSymbolExtensions.IsNullableType(node.Type)) || ((object)node.Operand.Type != null && TypeSymbolExtensions.IsNullableType(node.Operand.Type))) && !_inExpressionLambda)
			{
				return RewriteNullableConversion(node);
			}
			if ((node.ConversionKind & ConversionKind.AnonymousDelegate) != 0)
			{
				return RewriteAnonymousDelegateConversion(node);
			}
			if (!node.HasErrors && TypeSymbolExtensions.IsBooleanType(node.Type) && TypeSymbolExtensions.IsObjectType(node.Operand.Type))
			{
				BoundNode boundNode = node.Operand;
				while (boundNode.Kind == BoundKind.Parenthesized)
				{
					boundNode = ((BoundParenthesized)boundNode).Expression;
				}
				if (boundNode.Kind == BoundKind.BinaryOperator)
				{
					BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)boundNode;
					BinaryOperatorKind binaryOperatorKind = boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask;
					if ((uint)(binaryOperatorKind - 4) <= 5u)
					{
						return Visit(boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, boundBinaryOperator.Left, boundBinaryOperator.Right, boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, node.Type));
					}
				}
			}
			bool inExpressionLambda = _inExpressionLambda;
			if ((node.ConversionKind & (ConversionKind.Lambda | ConversionKind.ConvertedToExpressionTree)) == (ConversionKind.Lambda | ConversionKind.ConvertedToExpressionTree))
			{
				_inExpressionLambda = true;
			}
			BoundNode boundNode2;
			if (node.ExtendedInfoOpt != null && node.ExtendedInfoOpt.Kind == BoundKind.RelaxationLambda)
			{
				boundNode2 = RewriteLambdaRelaxationConversion(node);
			}
			else if ((node.ConversionKind & ConversionKind.InterpolatedString) == ConversionKind.InterpolatedString)
			{
				boundNode2 = RewriteInterpolatedStringConversion(node);
			}
			else if ((node.ConversionKind & (ConversionKind.Nullable | ConversionKind.Tuple)) == ConversionKind.Tuple)
			{
				boundNode2 = RewriteTupleConversion(node);
			}
			else
			{
				boundNode2 = base.VisitConversion(node);
				if (boundNode2.Kind == BoundKind.Conversion)
				{
					boundNode2 = TransformRewrittenConversion((BoundConversion)boundNode2);
				}
			}
			_inExpressionLambda = inExpressionLambda;
			return boundNode2;
		}

		private static bool IsFloatingPointExpressionOfUnknownPrecision(BoundExpression rewrittenNode)
		{
			if (rewrittenNode == null)
			{
				return false;
			}
			SpecialType specialType = rewrittenNode.Type.SpecialType;
			if (specialType != SpecialType.System_Double && specialType != SpecialType.System_Single)
			{
				return false;
			}
			switch (rewrittenNode.Kind)
			{
			case BoundKind.Sequence:
				return IsFloatingPointExpressionOfUnknownPrecision(((BoundSequence)rewrittenNode).ValueOpt);
			case BoundKind.Conversion:
			{
				BoundConversion boundConversion = (BoundConversion)rewrittenNode;
				return boundConversion.ConversionKind == ConversionKind.Identity && !boundConversion.ExplicitCastInCode;
			}
			default:
				return true;
			}
		}

		private BoundExpression RewriteTupleConversion(BoundConversion node)
		{
			SyntaxNode syntax = node.Syntax;
			BoundExpression rewrittenOperand = VisitExpression(node.Operand);
			NamedTypeSymbol destinationType = (NamedTypeSymbol)VisitType(node.Type);
			return MakeTupleConversion(syntax, rewrittenOperand, destinationType, (BoundConvertedTupleElements)node.ExtendedInfoOpt);
		}

		private BoundExpression MakeTupleConversion(SyntaxNode syntax, BoundExpression rewrittenOperand, TypeSymbol destinationType, BoundConvertedTupleElements convertedElements)
		{
			if (TypeSymbolExtensions.IsSameTypeIgnoringAll(destinationType, rewrittenOperand.Type))
			{
				return rewrittenOperand;
			}
			int length = TypeSymbolExtensions.GetElementTypesOfTupleOrCompatible(destinationType).Length;
			TypeSymbol type = rewrittenOperand.Type;
			TupleTypeSymbol tupleTypeSymbol = ((!type.IsTupleType) ? TupleTypeSymbol.Create((NamedTypeSymbol)type) : ((TupleTypeSymbol)type));
			ImmutableArray<FieldSymbol> tupleElements = tupleTypeSymbol.TupleElements;
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(length);
			BoundExpression init = null;
			SynthesizedLocal temp = null;
			BoundExpression rewrittenReceiver = CaptureOperand(rewrittenOperand, out temp, out init);
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, syntax, _compilationState, _diagnostics);
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				FieldSymbol fieldSymbol = tupleElements[i];
				UseSiteInfo<AssemblySymbol> useSiteInfo = fieldSymbol.CalculateUseSiteInfo();
				ReportUseSite(rewrittenOperand, useSiteInfo, _diagnostics);
				BoundExpression value = MakeTupleFieldAccess(syntax, fieldSymbol, rewrittenReceiver, null, isLValue: false);
				AddPlaceholderReplacement(convertedElements.ElementPlaceholders[i], value);
				instance.Add(VisitExpression(convertedElements.ConvertedElements[i]));
				RemovePlaceholderReplacement(convertedElements.ElementPlaceholders[i]);
			}
			BoundExpression boundExpression = MakeTupleCreationExpression(syntax, (NamedTypeSymbol)destinationType, instance.ToImmutableAndFree());
			return syntheticBoundNodeFactory.Sequence(temp, init, boundExpression);
		}

		private BoundNode RewriteLambdaRelaxationConversion(BoundConversion node)
		{
			BoundLambda lambda = ((BoundRelaxationLambda)node.ExtendedInfoOpt).Lambda;
			if (_inExpressionLambda && NoParameterRelaxation(node.Operand, lambda.LambdaSymbol))
			{
				BoundNode boundNode = base.VisitConversion(node.Update(node.Operand, node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, null, node.Type));
				return TransformRewrittenConversion((BoundConversion)boundNode);
			}
			return node.Update(VisitExpressionNode(lambda), node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, null, node.Type);
		}

		private BoundNode RewriteLambdaRelaxationConversion(BoundDirectCast node)
		{
			if (_inExpressionLambda && NoParameterRelaxation(node.Operand, node.RelaxationLambdaOpt.LambdaSymbol))
			{
				return base.VisitDirectCast(node.Update(node.Operand, node.ConversionKind, node.SuppressVirtualCalls, node.ConstantValueOpt, null, node.Type));
			}
			return node.Update(VisitExpressionNode(node.RelaxationLambdaOpt), node.ConversionKind, node.SuppressVirtualCalls, node.ConstantValueOpt, null, node.Type);
		}

		private BoundNode RewriteLambdaRelaxationConversion(BoundTryCast node)
		{
			if (_inExpressionLambda && NoParameterRelaxation(node.Operand, node.RelaxationLambdaOpt.LambdaSymbol))
			{
				return base.VisitTryCast(node.Update(node.Operand, node.ConversionKind, node.ConstantValueOpt, null, node.Type));
			}
			return node.Update(VisitExpressionNode(node.RelaxationLambdaOpt), node.ConversionKind, node.ConstantValueOpt, null, node.Type);
		}

		private static bool NoParameterRelaxation(BoundExpression from, LambdaSymbol toLambda)
		{
			LambdaSymbol lambdaSymbol = (from as BoundLambda)?.LambdaSymbol;
			if ((object)lambdaSymbol != null && !lambdaSymbol.IsSub && toLambda.IsSub)
			{
				return MethodSignatureComparer.HaveSameParameterTypes(lambdaSymbol.Parameters, null, toLambda.Parameters, null, considerByRef: true, considerCustomModifiers: false, considerTupleNames: false);
			}
			return false;
		}

		private BoundNode RewriteAnonymousDelegateConversion(BoundConversion node)
		{
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			if (BoundExpressionExtensions.IsDefaultValueConstant(node.Operand))
			{
				return syntheticBoundNodeFactory.Null(node.Type);
			}
			BoundLambda relaxationLambdaOpt;
			BoundRValuePlaceholder relaxationReceiverPlaceholderOpt;
			if (node.ExtendedInfoOpt != null)
			{
				BoundRelaxationLambda obj = (BoundRelaxationLambda)node.ExtendedInfoOpt;
				relaxationLambdaOpt = obj.Lambda;
				relaxationReceiverPlaceholderOpt = obj.ReceiverPlaceholderOpt;
			}
			else
			{
				relaxationLambdaOpt = null;
				relaxationReceiverPlaceholderOpt = null;
			}
			if (!_inExpressionLambda && CouldPossiblyBeNothing(syntheticBoundNodeFactory, node.Operand))
			{
				LocalSymbol localSymbol = syntheticBoundNodeFactory.SynthesizedLocal(node.Operand.Type);
				BoundBinaryOperator condition = syntheticBoundNodeFactory.ReferenceIsNothing(syntheticBoundNodeFactory.Local(localSymbol, isLValue: false));
				BoundExpression ifTrue = syntheticBoundNodeFactory.Null(node.Type);
				BoundDelegateCreationExpression ifFalse = new BoundDelegateCreationExpression(node.Syntax, syntheticBoundNodeFactory.Local(localSymbol, isLValue: false), ((NamedTypeSymbol)node.Operand.Type).DelegateInvokeMethod, relaxationLambdaOpt, relaxationReceiverPlaceholderOpt, null, node.Type);
				BoundExpression node2 = syntheticBoundNodeFactory.TernaryConditionalExpression(condition, ifTrue, ifFalse);
				return syntheticBoundNodeFactory.Sequence(localSymbol, syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(localSymbol, isLValue: true), VisitExpression(node.Operand)), VisitExpression(node2));
			}
			BoundDelegateCreationExpression node3 = new BoundDelegateCreationExpression(node.Syntax, node.Operand, ((NamedTypeSymbol)node.Operand.Type).DelegateInvokeMethod, relaxationLambdaOpt, relaxationReceiverPlaceholderOpt, null, node.Type);
			return VisitExpression(node3);
		}

		private bool CouldPossiblyBeNothing(SyntheticBoundNodeFactory F, BoundExpression node)
		{
			switch (node.Kind)
			{
			case BoundKind.TernaryConditionalExpression:
			{
				BoundTernaryConditionalExpression boundTernaryConditionalExpression = (BoundTernaryConditionalExpression)node;
				return CouldPossiblyBeNothing(F, boundTernaryConditionalExpression.WhenTrue) || CouldPossiblyBeNothing(F, boundTernaryConditionalExpression.WhenFalse);
			}
			case BoundKind.Conversion:
			{
				BoundConversion boundConversion = (BoundConversion)node;
				return CouldPossiblyBeNothing(F, boundConversion.Operand);
			}
			case BoundKind.Lambda:
				return false;
			case BoundKind.Call:
			{
				BoundCall boundCall = (BoundCall)node;
				return !(boundCall.Method == F.WellKnownMember<MethodSymbol>(WellKnownMember.System_Delegate__CreateDelegate, isOptional: true)) && !(boundCall.Method == F.WellKnownMember<MethodSymbol>(WellKnownMember.System_Delegate__CreateDelegate4, isOptional: true)) && !(boundCall.Method == F.WellKnownMember<MethodSymbol>(WellKnownMember.System_Reflection_MethodInfo__CreateDelegate, isOptional: true));
			}
			default:
				return true;
			}
		}

		private BoundExpression RewriteNullableConversion(BoundConversion node)
		{
			BoundExpression boundExpression = (BoundExpression)Visit(node.Operand);
			if (Conversions.IsIdentityConversion(node.ConversionKind))
			{
				return boundExpression;
			}
			return RewriteNullableConversion(node, boundExpression);
		}

		private BoundExpression RewriteNullableConversion(BoundConversion node, BoundExpression rewrittenOperand)
		{
			TypeSymbol type = node.Type;
			TypeSymbol type2 = rewrittenOperand.Type;
			if ((object)rewrittenOperand.ConstantValueOpt == ConstantValue.Nothing)
			{
				return NullableNull(rewrittenOperand.Syntax, type);
			}
			if (type2.IsReferenceType || type.IsReferenceType)
			{
				if (TypeSymbolExtensions.IsStringType(type))
				{
					rewrittenOperand = NullableValue(rewrittenOperand);
				}
				else
				{
					if (TypeSymbolExtensions.IsStringType(type2))
					{
						TypeSymbol nullableUnderlyingType = TypeSymbolExtensions.GetNullableUnderlyingType(type);
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
						ConversionKind key = Conversions.ClassifyConversion(rewrittenOperand.Type, nullableUnderlyingType, ref useSiteInfo).Key;
						_diagnostics.Add(node, useSiteInfo);
						return WrapInNullable(TransformRewrittenConversion(node.Update(rewrittenOperand, key, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ExtendedInfoOpt, TypeSymbolExtensions.GetNullableUnderlyingType(type))), type);
					}
					if (TypeSymbolExtensions.IsNullableType(type2))
					{
						if (HasNoValue(rewrittenOperand))
						{
							return new BoundDirectCast(node.Syntax, MakeNullLiteral(rewrittenOperand.Syntax, type), ConversionKind.WideningNothingLiteral, type);
						}
						if (HasValue(rewrittenOperand))
						{
							BoundExpression boundExpression = NullableValueOrDefault(rewrittenOperand);
							CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = GetNewCompoundUseSiteInfo();
							ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(boundExpression.Type, type, ref useSiteInfo2);
							_diagnostics.Add(node, useSiteInfo2);
							return new BoundDirectCast(node.Syntax, boundExpression, conversionKind, type);
						}
					}
				}
				return TransformRewrittenConversion(node.Update(rewrittenOperand, node.ConversionKind & ~ConversionKind.Nullable, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ExtendedInfoOpt, type));
			}
			BoundExpression boundExpression2 = rewrittenOperand;
			BoundExpression hasValueExpr = null;
			ArrayBuilder<LocalSymbol> temps = null;
			ArrayBuilder<BoundExpression> inits = null;
			if (TypeSymbolExtensions.IsNullableType(type2))
			{
				if (TypeSymbolExtensions.IsNullableType(type))
				{
					if (HasValue(rewrittenOperand))
					{
						boundExpression2 = NullableValueOrDefault(rewrittenOperand);
					}
					else
					{
						if (HasNoValue(rewrittenOperand))
						{
							return NullableNull(boundExpression2.Syntax, type);
						}
						BoundExpression whenNotNull = null;
						BoundExpression whenNull = null;
						if (IsConditionalAccess(rewrittenOperand, out whenNotNull, out whenNull) && HasValue(whenNotNull) && HasNoValue(whenNull))
						{
							return UpdateConditionalAccess(rewrittenOperand, FinishRewriteNullableConversion(node, type, NullableValueOrDefault(whenNotNull), null, null, null), NullableNull(boundExpression2.Syntax, type));
						}
						boundExpression2 = ProcessNullableOperand(rewrittenOperand, out hasValueExpr, ref temps, ref inits, doNotCaptureLocals: true);
					}
				}
				else
				{
					boundExpression2 = NullableValue(rewrittenOperand);
				}
			}
			return FinishRewriteNullableConversion(node, type, boundExpression2, hasValueExpr, temps, inits);
		}

		private CompoundUseSiteInfo<AssemblySymbol> GetNewCompoundUseSiteInfo()
		{
			return new CompoundUseSiteInfo<AssemblySymbol>(_diagnostics, Compilation.Assembly);
		}

		private BoundExpression FinishRewriteNullableConversion(BoundConversion node, TypeSymbol resultType, BoundExpression operand, BoundExpression operandHasValue, ArrayBuilder<LocalSymbol> temps, ArrayBuilder<BoundExpression> inits)
		{
			TypeSymbol nullableUnderlyingTypeOrSelf = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(resultType);
			if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(operand.Type, nullableUnderlyingTypeOrSelf))
			{
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
				ConversionKind key = Conversions.ClassifyConversion(operand.Type, nullableUnderlyingTypeOrSelf, ref useSiteInfo).Key;
				bool integerOverflow = false;
				ConstantValue constantValue = Conversions.TryFoldConstantConversion(operand, nullableUnderlyingTypeOrSelf, ref integerOverflow);
				if ((object)constantValue != null && !constantValue.IsBad)
				{
					operand = RewriteConstant(new BoundLiteral(node.Syntax, constantValue, nullableUnderlyingTypeOrSelf), constantValue);
				}
				else
				{
					_diagnostics.Add(node, useSiteInfo);
					operand = (((key & ConversionKind.Tuple) == 0) ? TransformRewrittenConversion(new BoundConversion(node.Syntax, operand, key, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ExtendedInfoOpt, nullableUnderlyingTypeOrSelf)) : MakeTupleConversion(node.Syntax, operand, nullableUnderlyingTypeOrSelf, (BoundConvertedTupleElements)node.ExtendedInfoOpt));
				}
			}
			if (TypeSymbolExtensions.IsNullableType(resultType))
			{
				operand = WrapInNullable(operand, resultType);
				if (operandHasValue != null)
				{
					operand = MakeTernaryConditionalExpression(node.Syntax, operandHasValue, operand, NullableNull(operand.Syntax, resultType));
					if (temps != null)
					{
						operand = new BoundSequence(operand.Syntax, temps.ToImmutableAndFree(), inits.ToImmutableAndFree(), operand, operand.Type);
					}
				}
			}
			return operand;
		}

		private BoundExpression RewriteNullableReferenceConversion(BoundConversion node, BoundExpression rewrittenOperand)
		{
			TypeSymbol type = node.Type;
			TypeSymbol type2 = rewrittenOperand.Type;
			if (TypeSymbolExtensions.IsStringType(type2))
			{
				TypeSymbol nullableUnderlyingType = TypeSymbolExtensions.GetNullableUnderlyingType(type);
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
				ConversionKind key = Conversions.ClassifyConversion(type2, nullableUnderlyingType, ref useSiteInfo).Key;
				_diagnostics.Add(node, useSiteInfo);
				return WrapInNullable(TransformRewrittenConversion(node.Update(rewrittenOperand, key, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ExtendedInfoOpt, TypeSymbolExtensions.GetNullableUnderlyingType(type))), type);
			}
			if (TypeSymbolExtensions.IsStringType(type))
			{
				rewrittenOperand = NullableValue(rewrittenOperand);
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = GetNewCompoundUseSiteInfo();
				Conversions.ClassifyDirectCastConversion(rewrittenOperand.Type, type, ref useSiteInfo2);
				_diagnostics.Add(node, useSiteInfo2);
				return TransformRewrittenConversion(node.Update(rewrittenOperand, node.ConversionKind & ~ConversionKind.Nullable, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ExtendedInfoOpt, type));
			}
			if (TypeSymbolExtensions.IsNullableType(type2))
			{
				if (HasNoValue(rewrittenOperand))
				{
					return new BoundDirectCast(node.Syntax, MakeNullLiteral(rewrittenOperand.Syntax, type), ConversionKind.WideningNothingLiteral, type);
				}
				if (HasValue(rewrittenOperand))
				{
					BoundExpression boundExpression = NullableValueOrDefault(rewrittenOperand);
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo3 = GetNewCompoundUseSiteInfo();
					ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(boundExpression.Type, type, ref useSiteInfo3);
					_diagnostics.Add(node, useSiteInfo3);
					return new BoundDirectCast(node.Syntax, boundExpression, conversionKind, type);
				}
			}
			if (TypeSymbolExtensions.IsNullableType(type))
			{
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo4 = GetNewCompoundUseSiteInfo();
				ConversionKind conversionKind2 = Conversions.ClassifyDirectCastConversion(rewrittenOperand.Type, type, ref useSiteInfo4);
				_diagnostics.Add(node, useSiteInfo4);
				return new BoundDirectCast(node.Syntax, rewrittenOperand, conversionKind2, type);
			}
			throw ExceptionUtilities.Unreachable;
		}

		private BoundNode RewriteNullableUserDefinedConversion(BoundUserDefinedConversion node)
		{
			BoundExpression operand = node.Operand;
			BoundConversion inConversionOpt = node.InConversionOpt;
			BoundCall call = node.Call;
			BoundConversion outConversionOpt = node.OutConversionOpt;
			TypeSymbol type = outConversionOpt.Type;
			BoundExpression boundExpression = VisitExpressionNode(operand);
			BoundExpression boundExpression2 = NullableNull(node.Syntax, type);
			if (HasNoValue(boundExpression))
			{
				return boundExpression2;
			}
			bool num = HasValue(boundExpression);
			BoundExpression condition = null;
			SynthesizedLocal temp = null;
			BoundExpression rewrittenOperand;
			if (num)
			{
				rewrittenOperand = boundExpression;
			}
			else
			{
				BoundExpression init = null;
				BoundExpression boundExpression3 = CaptureNullableIfNeeded(boundExpression, out temp, out init, doNotCaptureLocals: true);
				condition = NullableHasValue(init ?? boundExpression3);
				rewrittenOperand = WrapInNullable(NullableValueOrDefault(boundExpression3), boundExpression3.Type);
			}
			BoundExpression rewrittenOperand2 = call.Update(arguments: ImmutableArray.Create(RewriteNullableConversion(inConversionOpt, rewrittenOperand)), method: call.Method, methodGroupOpt: null, receiverOpt: call.ReceiverOpt, defaultArguments: default(BitVector), constantValueOpt: call.ConstantValueOpt, isLValue: call.IsLValue, suppressObjectClone: call.SuppressObjectClone, type: call.Type);
			rewrittenOperand2 = RewriteNullableConversion(outConversionOpt, rewrittenOperand2);
			if (num)
			{
				return rewrittenOperand2;
			}
			BoundExpression boundExpression4 = MakeTernaryConditionalExpression(node.Syntax, condition, rewrittenOperand2, boundExpression2);
			if ((object)temp != null)
			{
				boundExpression4 = new BoundSequence(node.Syntax, ImmutableArray.Create((LocalSymbol)temp), ImmutableArray<BoundExpression>.Empty, boundExpression4, boundExpression4.Type);
			}
			return boundExpression4;
		}

		private BoundExpression TransformRewrittenConversion(BoundConversion rewrittenConversion)
		{
			if (rewrittenConversion.HasErrors || _inExpressionLambda)
			{
				return rewrittenConversion;
			}
			BoundExpression result = rewrittenConversion;
			TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(rewrittenConversion.Type);
			BoundExpression operand = rewrittenConversion.Operand;
			if (BoundExpressionExtensions.IsNothingLiteral(operand))
			{
				if (TypeSymbolExtensions.IsTypeParameter(enumUnderlyingTypeOrSelf) || enumUnderlyingTypeOrSelf.IsReferenceType)
				{
					result = RewriteAsDirectCast(rewrittenConversion);
				}
			}
			else
			{
				if (operand.Kind == BoundKind.Lambda)
				{
					return rewrittenConversion;
				}
				TypeSymbol enumUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(operand.Type);
				if (TypeSymbolExtensions.IsFloatingType(enumUnderlyingTypeOrSelf2) && TypeSymbolExtensions.IsIntegralType(enumUnderlyingTypeOrSelf))
				{
					result = RewriteFloatingToIntegralConversion(rewrittenConversion, enumUnderlyingTypeOrSelf2, enumUnderlyingTypeOrSelf);
				}
				else if (TypeSymbolExtensions.IsDecimalType(enumUnderlyingTypeOrSelf2) && (TypeSymbolExtensions.IsBooleanType(enumUnderlyingTypeOrSelf) || TypeSymbolExtensions.IsIntegralType(enumUnderlyingTypeOrSelf) || TypeSymbolExtensions.IsFloatingType(enumUnderlyingTypeOrSelf)))
				{
					result = RewriteDecimalToNumericOrBooleanConversion(rewrittenConversion, enumUnderlyingTypeOrSelf2, enumUnderlyingTypeOrSelf);
				}
				else if (TypeSymbolExtensions.IsDecimalType(enumUnderlyingTypeOrSelf) && (TypeSymbolExtensions.IsBooleanType(enumUnderlyingTypeOrSelf2) || TypeSymbolExtensions.IsIntegralType(enumUnderlyingTypeOrSelf2) || TypeSymbolExtensions.IsFloatingType(enumUnderlyingTypeOrSelf2)))
				{
					result = RewriteNumericOrBooleanToDecimalConversion(rewrittenConversion, enumUnderlyingTypeOrSelf2, enumUnderlyingTypeOrSelf);
				}
				else if (!TypeSymbolExtensions.IsNullableType(enumUnderlyingTypeOrSelf2) && !TypeSymbolExtensions.IsNullableType(enumUnderlyingTypeOrSelf))
				{
					if (TypeSymbolExtensions.IsObjectType(enumUnderlyingTypeOrSelf2) && (TypeSymbolExtensions.IsTypeParameter(enumUnderlyingTypeOrSelf) || TypeSymbolExtensions.IsIntrinsicType(enumUnderlyingTypeOrSelf)))
					{
						result = RewriteFromObjectConversion(rewrittenConversion, enumUnderlyingTypeOrSelf2, enumUnderlyingTypeOrSelf);
					}
					else if (TypeSymbolExtensions.IsTypeParameter(enumUnderlyingTypeOrSelf2))
					{
						result = RewriteAsDirectCast(rewrittenConversion);
					}
					else if (TypeSymbolExtensions.IsTypeParameter(enumUnderlyingTypeOrSelf))
					{
						result = RewriteAsDirectCast(rewrittenConversion);
					}
					else if (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf2) && (TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf) || TypeSymbolExtensions.IsIntrinsicValueType(enumUnderlyingTypeOrSelf)))
					{
						result = RewriteFromStringConversion(rewrittenConversion, enumUnderlyingTypeOrSelf2, enumUnderlyingTypeOrSelf);
					}
					else if (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf) && (TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf2) || TypeSymbolExtensions.IsIntrinsicValueType(enumUnderlyingTypeOrSelf2)))
					{
						result = RewriteToStringConversion(rewrittenConversion, enumUnderlyingTypeOrSelf2, enumUnderlyingTypeOrSelf);
					}
					else if (enumUnderlyingTypeOrSelf2.IsReferenceType && TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf))
					{
						result = RewriteReferenceTypeToCharArrayRankOneConversion(rewrittenConversion, enumUnderlyingTypeOrSelf2, enumUnderlyingTypeOrSelf);
					}
					else if (enumUnderlyingTypeOrSelf.IsReferenceType)
					{
						result = RewriteAsDirectCast(rewrittenConversion);
					}
					else if (enumUnderlyingTypeOrSelf2.IsReferenceType && TypeSymbolExtensions.IsIntrinsicValueType(enumUnderlyingTypeOrSelf))
					{
						result = RewriteFromObjectConversion(rewrittenConversion, Compilation.GetSpecialType(SpecialType.System_Object), enumUnderlyingTypeOrSelf);
					}
				}
			}
			return result;
		}

		private BoundExpression RewriteReferenceTypeToCharArrayRankOneConversion(BoundConversion node, TypeSymbol typeFrom, TypeSymbol typeTo)
		{
			BoundExpression result = node;
			MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneObject);
			if (!ReportMissingOrBadRuntimeHelper(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneObject, methodSymbol))
			{
				BoundExpression boundExpression = node.Operand;
				if (!TypeSymbolExtensions.IsObjectType(boundExpression.Type))
				{
					TypeSymbol type = methodSymbol.Parameters[0].Type;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
					boundExpression = new BoundDirectCast(boundExpression.Syntax, boundExpression, Conversions.ClassifyDirectCastConversion(boundExpression.Type, type, ref useSiteInfo), type);
					_diagnostics.Add(node, useSiteInfo);
				}
				result = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(boundExpression), null, methodSymbol.ReturnType);
			}
			return result;
		}

		private static BoundExpression RewriteAsDirectCast(BoundConversion node)
		{
			return new BoundDirectCast(node.Syntax, node.Operand, node.ConversionKind, node.Type);
		}

		private BoundExpression RewriteFromObjectConversion(BoundConversion node, TypeSymbol typeFrom, TypeSymbol underlyingTypeTo)
		{
			BoundExpression boundExpression = node;
			WellKnownMember wellKnownMember = WellKnownMember.Count;
			switch (underlyingTypeTo.SpecialType)
			{
			case SpecialType.System_Boolean:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanObject;
				break;
			case SpecialType.System_SByte:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSByteObject;
				break;
			case SpecialType.System_Byte:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToByteObject;
				break;
			case SpecialType.System_Int16:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToShortObject;
				break;
			case SpecialType.System_UInt16:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUShortObject;
				break;
			case SpecialType.System_Int32:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToIntegerObject;
				break;
			case SpecialType.System_UInt32:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUIntegerObject;
				break;
			case SpecialType.System_Int64:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToLongObject;
				break;
			case SpecialType.System_UInt64:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToULongObject;
				break;
			case SpecialType.System_Single:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSingleObject;
				break;
			case SpecialType.System_Double:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDoubleObject;
				break;
			case SpecialType.System_Decimal:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalObject;
				break;
			case SpecialType.System_DateTime:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDateObject;
				break;
			case SpecialType.System_Char:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharObject;
				break;
			case SpecialType.System_String:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringObject;
				break;
			default:
				if (TypeSymbolExtensions.IsTypeParameter(underlyingTypeTo))
				{
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToGenericParameter_T_Object;
				}
				break;
			}
			if (wellKnownMember != WellKnownMember.Count)
			{
				MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(wellKnownMember);
				if (!ReportMissingOrBadRuntimeHelper(node, wellKnownMember, methodSymbol))
				{
					if (wellKnownMember == WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToGenericParameter_T_Object)
					{
						methodSymbol = methodSymbol.Construct(underlyingTypeTo);
					}
					BoundExpression boundExpression2 = node.Operand;
					if (!TypeSymbolExtensions.IsObjectType(boundExpression2.Type))
					{
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
						boundExpression2 = new BoundDirectCast(boundExpression2.Syntax, boundExpression2, Conversions.ClassifyDirectCastConversion(boundExpression2.Type, typeFrom, ref useSiteInfo), typeFrom);
						_diagnostics.Add(node, useSiteInfo);
					}
					boundExpression = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(boundExpression2), null, methodSymbol.ReturnType);
					TypeSymbol type = node.Type;
					if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(type, methodSymbol.ReturnType))
					{
						ConversionKind conversionKind = ConversionKind.NarrowingNumeric | ConversionKind.InvolvesEnumTypeConversions;
						boundExpression = new BoundConversion(node.Syntax, boundExpression, conversionKind, node.Checked, node.ExplicitCastInCode, type);
					}
				}
			}
			return boundExpression;
		}

		private BoundExpression RewriteToStringConversion(BoundConversion node, TypeSymbol underlyingTypeFrom, TypeSymbol typeTo)
		{
			BoundExpression result = node;
			MethodSymbol methodSymbol = null;
			if (TypeSymbolExtensions.IsCharSZArray(underlyingTypeFrom))
			{
				methodSymbol = (MethodSymbol)ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_String__CtorSZArrayChar);
				if (ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_String__CtorSZArrayChar, methodSymbol))
				{
					methodSymbol = null;
				}
			}
			else
			{
				WellKnownMember wellKnownMember = WellKnownMember.Count;
				switch (underlyingTypeFrom.SpecialType)
				{
				case SpecialType.System_Boolean:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringBoolean;
					break;
				case SpecialType.System_SByte:
				case SpecialType.System_Int16:
				case SpecialType.System_Int32:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringInt32;
					break;
				case SpecialType.System_Byte:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringByte;
					break;
				case SpecialType.System_UInt16:
				case SpecialType.System_UInt32:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringUInt32;
					break;
				case SpecialType.System_Int64:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringInt64;
					break;
				case SpecialType.System_UInt64:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringUInt64;
					break;
				case SpecialType.System_Single:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringSingle;
					break;
				case SpecialType.System_Double:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDouble;
					break;
				case SpecialType.System_Decimal:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDecimal;
					break;
				case SpecialType.System_DateTime:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDateTime;
					break;
				case SpecialType.System_Char:
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringChar;
					break;
				}
				if (wellKnownMember != WellKnownMember.Count)
				{
					methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(wellKnownMember);
					if (ReportMissingOrBadRuntimeHelper(node, wellKnownMember, methodSymbol))
					{
						methodSymbol = null;
					}
				}
			}
			if ((object)methodSymbol != null)
			{
				BoundExpression boundExpression = node.Operand;
				TypeSymbol type = boundExpression.Type;
				if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(type, methodSymbol.Parameters[0].Type))
				{
					boundExpression = new BoundConversion(conversionKind: (!TypeSymbolExtensions.IsEnumType(type)) ? ConversionKind.WideningNumeric : (ConversionKind.WideningNumeric | ConversionKind.InvolvesEnumTypeConversions), syntax: node.Syntax, operand: boundExpression, @checked: node.Checked, explicitCastInCode: node.ExplicitCastInCode, type: methodSymbol.Parameters[0].Type);
				}
				result = ((methodSymbol.MethodKind != MethodKind.Constructor) ? ((BoundExpression)new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(boundExpression), null, methodSymbol.ReturnType)) : ((BoundExpression)new BoundObjectCreationExpression(node.Syntax, methodSymbol, ImmutableArray.Create(boundExpression), null, typeTo)));
			}
			return result;
		}

		private BoundExpression RewriteFromStringConversion(BoundConversion node, TypeSymbol typeFrom, TypeSymbol underlyingTypeTo)
		{
			BoundExpression boundExpression = node;
			WellKnownMember wellKnownMember = WellKnownMember.Count;
			switch (underlyingTypeTo.SpecialType)
			{
			case SpecialType.System_Boolean:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanString;
				break;
			case SpecialType.System_SByte:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSByteString;
				break;
			case SpecialType.System_Byte:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToByteString;
				break;
			case SpecialType.System_Int16:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToShortString;
				break;
			case SpecialType.System_UInt16:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUShortString;
				break;
			case SpecialType.System_Int32:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToIntegerString;
				break;
			case SpecialType.System_UInt32:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUIntegerString;
				break;
			case SpecialType.System_Int64:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToLongString;
				break;
			case SpecialType.System_UInt64:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToULongString;
				break;
			case SpecialType.System_Single:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSingleString;
				break;
			case SpecialType.System_Double:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDoubleString;
				break;
			case SpecialType.System_Decimal:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalString;
				break;
			case SpecialType.System_DateTime:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDateString;
				break;
			case SpecialType.System_Char:
				wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharString;
				break;
			default:
				if (TypeSymbolExtensions.IsCharSZArray(underlyingTypeTo))
				{
					wellKnownMember = WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneString;
				}
				break;
			}
			if (wellKnownMember != WellKnownMember.Count)
			{
				MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(wellKnownMember);
				if (!ReportMissingOrBadRuntimeHelper(node, wellKnownMember, methodSymbol))
				{
					BoundExpression operand = node.Operand;
					boundExpression = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(operand), null, methodSymbol.ReturnType);
					TypeSymbol type = node.Type;
					if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(type, methodSymbol.ReturnType))
					{
						ConversionKind conversionKind = ConversionKind.NarrowingNumeric | ConversionKind.InvolvesEnumTypeConversions;
						boundExpression = new BoundConversion(node.Syntax, boundExpression, conversionKind, node.Checked, node.ExplicitCastInCode, type);
					}
				}
			}
			return boundExpression;
		}

		private BoundExpression RewriteNumericOrBooleanToDecimalConversion(BoundConversion node, TypeSymbol underlyingTypeFrom, TypeSymbol typeTo)
		{
			BoundExpression result = node;
			MethodSymbol methodSymbol;
			if (TypeSymbolExtensions.IsBooleanType(underlyingTypeFrom))
			{
				methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalBoolean);
				if (ReportMissingOrBadRuntimeHelper(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalBoolean, methodSymbol))
				{
					methodSymbol = null;
				}
			}
			else
			{
				SpecialMember specialMember;
				switch (underlyingTypeFrom.SpecialType)
				{
				case SpecialType.System_SByte:
				case SpecialType.System_Byte:
				case SpecialType.System_Int16:
				case SpecialType.System_UInt16:
				case SpecialType.System_Int32:
					specialMember = SpecialMember.System_Decimal__CtorInt32;
					break;
				case SpecialType.System_UInt32:
					specialMember = SpecialMember.System_Decimal__CtorUInt32;
					break;
				case SpecialType.System_Int64:
					specialMember = SpecialMember.System_Decimal__CtorInt64;
					break;
				case SpecialType.System_UInt64:
					specialMember = SpecialMember.System_Decimal__CtorUInt64;
					break;
				case SpecialType.System_Single:
					specialMember = SpecialMember.System_Decimal__CtorSingle;
					break;
				case SpecialType.System_Double:
					specialMember = SpecialMember.System_Decimal__CtorDouble;
					break;
				default:
					return result;
				}
				methodSymbol = (MethodSymbol)ContainingAssembly.GetSpecialTypeMember(specialMember);
				if (ReportMissingOrBadRuntimeHelper(node, specialMember, methodSymbol))
				{
					methodSymbol = null;
				}
			}
			if ((object)methodSymbol != null)
			{
				BoundExpression boundExpression = node.Operand;
				TypeSymbol type = boundExpression.Type;
				if ((object)type != methodSymbol.Parameters[0].Type)
				{
					boundExpression = new BoundConversion(conversionKind: (!TypeSymbolExtensions.IsEnumType(type)) ? ConversionKind.WideningNumeric : (ConversionKind.WideningNumeric | ConversionKind.InvolvesEnumTypeConversions), syntax: node.Syntax, operand: boundExpression, @checked: node.Checked, explicitCastInCode: node.ExplicitCastInCode, type: methodSymbol.Parameters[0].Type);
				}
				result = ((methodSymbol.MethodKind != MethodKind.Constructor) ? ((BoundExpression)new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(boundExpression), null, methodSymbol.ReturnType)) : ((BoundExpression)new BoundObjectCreationExpression(node.Syntax, methodSymbol, ImmutableArray.Create(boundExpression), null, typeTo)));
			}
			return result;
		}

		private BoundExpression RewriteDecimalToNumericOrBooleanConversion(BoundConversion node, TypeSymbol typeFrom, TypeSymbol underlyingTypeTo)
		{
			BoundExpression boundExpression = node;
			WellKnownMember wellKnownMember;
			switch (underlyingTypeTo.SpecialType)
			{
			case SpecialType.System_Boolean:
				wellKnownMember = WellKnownMember.System_Convert__ToBooleanDecimal;
				break;
			case SpecialType.System_SByte:
				wellKnownMember = WellKnownMember.System_Convert__ToSByteDecimal;
				break;
			case SpecialType.System_Byte:
				wellKnownMember = WellKnownMember.System_Convert__ToByteDecimal;
				break;
			case SpecialType.System_Int16:
				wellKnownMember = WellKnownMember.System_Convert__ToInt16Decimal;
				break;
			case SpecialType.System_UInt16:
				wellKnownMember = WellKnownMember.System_Convert__ToUInt16Decimal;
				break;
			case SpecialType.System_Int32:
				wellKnownMember = WellKnownMember.System_Convert__ToInt32Decimal;
				break;
			case SpecialType.System_UInt32:
				wellKnownMember = WellKnownMember.System_Convert__ToUInt32Decimal;
				break;
			case SpecialType.System_Int64:
				wellKnownMember = WellKnownMember.System_Convert__ToInt64Decimal;
				break;
			case SpecialType.System_UInt64:
				wellKnownMember = WellKnownMember.System_Convert__ToUInt64Decimal;
				break;
			case SpecialType.System_Single:
				wellKnownMember = WellKnownMember.System_Convert__ToSingleDecimal;
				break;
			case SpecialType.System_Double:
				wellKnownMember = WellKnownMember.System_Convert__ToDoubleDecimal;
				break;
			default:
				return boundExpression;
			}
			MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(wellKnownMember);
			if (!ReportMissingOrBadRuntimeHelper(node, wellKnownMember, methodSymbol))
			{
				BoundExpression operand = node.Operand;
				boundExpression = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(operand), null, methodSymbol.ReturnType);
				TypeSymbol type = node.Type;
				if ((object)type != methodSymbol.ReturnType)
				{
					ConversionKind conversionKind = ConversionKind.NarrowingNumeric | ConversionKind.InvolvesEnumTypeConversions;
					boundExpression = new BoundConversion(node.Syntax, boundExpression, conversionKind, node.Checked, node.ExplicitCastInCode, type);
				}
			}
			return boundExpression;
		}

		private BoundExpression RewriteFloatingToIntegralConversion(BoundConversion node, TypeSymbol typeFrom, TypeSymbol underlyingTypeTo)
		{
			BoundExpression result = node;
			BoundExpression boundExpression = node.Operand;
			if (boundExpression.Kind == BoundKind.Call)
			{
				BoundCall boundCall = (BoundCall)boundExpression;
				if (IsFloatingTruncation(boundCall))
				{
					return new BoundConversion(node.Syntax, boundCall.Arguments[0], node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.Type);
				}
				if (ReturnsWholeNumberDouble(boundCall))
				{
					return node;
				}
			}
			MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__RoundDouble);
			if (!ReportMissingOrBadRuntimeHelper(node, WellKnownMember.System_Math__RoundDouble, methodSymbol))
			{
				if ((object)typeFrom != methodSymbol.Parameters[0].Type)
				{
					boundExpression = new BoundConversion(node.Syntax, boundExpression, ConversionKind.WideningNumeric, node.Checked, node.ExplicitCastInCode, methodSymbol.Parameters[0].Type);
				}
				BoundCall operand = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(boundExpression), null, methodSymbol.ReturnType);
				result = new BoundConversion(node.Syntax, operand, node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.Type);
			}
			return result;
		}

		private bool ReturnsWholeNumberDouble(BoundCall node)
		{
			string name = node.Method.Name;
			if ("Ceiling".Equals(name))
			{
				return node.Method == Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__CeilingDouble);
			}
			if ("Floor".Equals(name))
			{
				return node.Method == Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__FloorDouble);
			}
			if ("Round".Equals(name))
			{
				return node.Method == Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__RoundDouble);
			}
			if ("Int".Equals(name))
			{
				return node.Type.SpecialType switch
				{
					SpecialType.System_Single => node.Method == Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Conversion__IntSingle), 
					SpecialType.System_Double => node.Method == Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Conversion__IntDouble), 
					_ => false, 
				};
			}
			return false;
		}

		private bool IsFloatingTruncation(BoundCall node)
		{
			string name = node.Method.Name;
			if ("Fix".Equals(name))
			{
				return node.Type.SpecialType switch
				{
					SpecialType.System_Single => node.Method == Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Conversion__FixSingle), 
					SpecialType.System_Double => node.Method == Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Conversion__FixDouble), 
					_ => false, 
				};
			}
			if ("Truncate".Equals(name))
			{
				return node.Method == Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__TruncateDouble);
			}
			return false;
		}

		public override BoundNode VisitDirectCast(BoundDirectCast node)
		{
			if (!_inExpressionLambda && Conversions.IsIdentityConversion(node.ConversionKind))
			{
				return VisitExpressionNode(node.Operand);
			}
			bool inExpressionLambda = _inExpressionLambda;
			if ((node.ConversionKind & (ConversionKind.Lambda | ConversionKind.ConvertedToExpressionTree)) == (ConversionKind.Lambda | ConversionKind.ConvertedToExpressionTree))
			{
				_inExpressionLambda = true;
			}
			BoundNode result = ((node.RelaxationLambdaOpt != null) ? RewriteLambdaRelaxationConversion(node) : base.VisitDirectCast(node));
			_inExpressionLambda = inExpressionLambda;
			return result;
		}

		public override BoundNode VisitTryCast(BoundTryCast node)
		{
			if (!_inExpressionLambda && Conversions.IsIdentityConversion(node.ConversionKind))
			{
				return Visit(node.Operand);
			}
			bool inExpressionLambda = _inExpressionLambda;
			if ((node.ConversionKind & (ConversionKind.Lambda | ConversionKind.ConvertedToExpressionTree)) == (ConversionKind.Lambda | ConversionKind.ConvertedToExpressionTree))
			{
				_inExpressionLambda = true;
			}
			BoundNode boundNode;
			if (node.RelaxationLambdaOpt == null)
			{
				boundNode = null;
				if (Conversions.IsWideningConversion(node.ConversionKind) && !Conversions.IsIdentityConversion(node.ConversionKind))
				{
					BoundExpression operand = node.Operand;
					if (operand.Kind != BoundKind.Lambda)
					{
						TypeSymbol type = operand.Type;
						TypeSymbol type2 = node.Type;
						if (!TypeSymbolExtensions.IsTypeParameter(type2) && type2.IsReferenceType && !TypeSymbolExtensions.IsTypeParameter(type) && type.IsReferenceType)
						{
							boundNode = new BoundDirectCast(node.Syntax, (BoundExpression)Visit(operand), node.ConversionKind, type2);
						}
					}
				}
				if (boundNode == null)
				{
					boundNode = base.VisitTryCast(node);
				}
			}
			else
			{
				boundNode = RewriteLambdaRelaxationConversion(node);
			}
			_inExpressionLambda = inExpressionLambda;
			return boundNode;
		}

		public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
		{
			if (node.RelaxationLambdaOpt == null)
			{
				return base.VisitDelegateCreationExpression(node);
			}
			BoundRValuePlaceholder relaxationReceiverPlaceholderOpt = node.RelaxationReceiverPlaceholderOpt;
			SynthesizedLocal synthesizedLocal = null;
			if (relaxationReceiverPlaceholderOpt != null)
			{
				if (_inExpressionLambda)
				{
					AddPlaceholderReplacement(relaxationReceiverPlaceholderOpt, VisitExpression(node.ReceiverOpt));
				}
				else
				{
					synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, relaxationReceiverPlaceholderOpt.Type, SynthesizedLocalKind.DelegateRelaxationReceiver, relaxationReceiverPlaceholderOpt.Syntax);
					BoundLocal value = new BoundLocal(relaxationReceiverPlaceholderOpt.Syntax, synthesizedLocal, synthesizedLocal.Type).MakeRValue();
					AddPlaceholderReplacement(relaxationReceiverPlaceholderOpt, value);
				}
			}
			BoundLambda boundLambda = (BoundLambda)Visit(node.RelaxationLambdaOpt);
			if (relaxationReceiverPlaceholderOpt != null)
			{
				RemovePlaceholderReplacement(relaxationReceiverPlaceholderOpt);
			}
			BoundExpression boundExpression = new BoundConversion(boundLambda.Syntax, boundLambda, ConversionKind.Widening | ConversionKind.Lambda, @checked: false, explicitCastInCode: false, node.Type, node.HasErrors);
			if ((object)synthesizedLocal != null)
			{
				BoundExpression boundExpression2 = VisitExpressionNode(node.ReceiverOpt);
				BoundAssignmentOperator item = new BoundAssignmentOperator(boundExpression2.Syntax, new BoundLocal(boundExpression2.Syntax, synthesizedLocal, synthesizedLocal.Type), boundExpression2.MakeRValue(), suppressObjectClone: true, synthesizedLocal.Type);
				boundExpression = new BoundSequence(boundExpression.Syntax, ImmutableArray.Create((LocalSymbol)synthesizedLocal), ImmutableArray.Create((BoundExpression)item), boundExpression, boundExpression.Type);
			}
			return boundExpression;
		}

		public override BoundNode VisitDimStatement(BoundDimStatement node)
		{
			ArrayBuilder<BoundStatement> arrayBuilder = null;
			ImmutableArray<BoundLocalDeclarationBase>.Enumerator enumerator = node.LocalDeclarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundLocalDeclarationBase current = enumerator.Current;
				BoundNode boundNode = Visit(current);
				if (boundNode != null)
				{
					if (arrayBuilder == null)
					{
						arrayBuilder = ArrayBuilder<BoundStatement>.GetInstance();
					}
					arrayBuilder.Add((BoundStatement)boundNode);
				}
			}
			if (arrayBuilder != null)
			{
				return new BoundStatementList(node.Syntax, arrayBuilder.ToImmutableAndFree());
			}
			return null;
		}

		public override BoundNode VisitDoLoopStatement(BoundDoLoopStatement node)
		{
			if (node.ConditionOpt != null)
			{
				if (node.ConditionIsTop)
				{
					return VisitTopConditionLoop(node);
				}
				return VisitBottomConditionLoop(node);
			}
			return VisitInfiniteLoop(node);
		}

		private BoundNode VisitTopConditionLoop(BoundDoLoopStatement node)
		{
			bool num = ShouldGenerateUnstructuredExceptionHandlingResumeCode(node);
			BoundLabelStatement loopResumeLabelOpt = null;
			ImmutableArray<BoundStatement> conditionResumeTargetOpt = default(ImmutableArray<BoundStatement>);
			if (num)
			{
				loopResumeLabelOpt = RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax);
				conditionResumeTargetOpt = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, canThrow: true);
			}
			BoundStatement rewrittenBody = (BoundStatement)Visit(node.Body);
			BoundLabelStatement afterBodyResumeTargetOpt = null;
			if (num)
			{
				afterBodyResumeTargetOpt = RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax);
			}
			_ = (DoLoopBlockSyntax)node.Syntax;
			return RewriteWhileStatement(node, VisitExpressionNode(node.ConditionOpt), rewrittenBody, node.ContinueLabel, node.ExitLabel, !node.ConditionIsUntil, loopResumeLabelOpt, conditionResumeTargetOpt, afterBodyResumeTargetOpt);
		}

		private BoundNode VisitBottomConditionLoop(BoundDoLoopStatement node)
		{
			DoLoopBlockSyntax doLoopBlockSyntax = (DoLoopBlockSyntax)node.Syntax;
			bool num = ShouldGenerateUnstructuredExceptionHandlingResumeCode(node);
			BoundLabelStatement boundLabelStatement = null;
			if (num)
			{
				boundLabelStatement = RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(doLoopBlockSyntax.DoStatement);
			}
			LabelSymbol label = GenerateLabel("start");
			BoundStatement boundStatement = new BoundLabelStatement(doLoopBlockSyntax.DoStatement, label);
			if (boundLabelStatement != null)
			{
				boundStatement = Concat(boundLabelStatement, boundStatement);
			}
			BoundStatement boundStatement2 = (BoundStatement)Visit(node.Body);
			bool flag = this.get_Instrument((BoundNode)node);
			if (flag && doLoopBlockSyntax.LoopStatement != null)
			{
				boundStatement2 = Concat(boundStatement2, _instrumenterOpt.InstrumentDoLoopEpilogue(node, null));
			}
			ImmutableArray<BoundStatement> immutableArray = default(ImmutableArray<BoundStatement>);
			if (num)
			{
				immutableArray = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, canThrow: true);
			}
			BoundExpression boundExpression = VisitExpressionNode(node.ConditionOpt);
			if (boundExpression != null && flag)
			{
				boundExpression = _instrumenterOpt.InstrumentDoLoopStatementCondition(node, boundExpression, _currentMethodOrLambda);
			}
			BoundStatement boundStatement3 = new BoundConditionalGoto(doLoopBlockSyntax.DoStatement, boundExpression, !node.ConditionIsUntil, label);
			if (!immutableArray.IsDefaultOrEmpty)
			{
				boundStatement3 = new BoundStatementList(boundStatement3.Syntax, immutableArray.Add(boundStatement3));
			}
			if (flag)
			{
				return new BoundStatementList(node.Syntax, ImmutableArray.Create<BoundStatement>(boundStatement, _instrumenterOpt.InstrumentDoLoopStatementEntryOrConditionalGotoStart(node, null), boundStatement2, new BoundLabelStatement(doLoopBlockSyntax.DoStatement, node.ContinueLabel), boundStatement3, new BoundLabelStatement(doLoopBlockSyntax.DoStatement, node.ExitLabel)));
			}
			return new BoundStatementList(node.Syntax, ImmutableArray.Create<BoundStatement>(boundStatement, boundStatement2, new BoundLabelStatement(node.Syntax, node.ContinueLabel), boundStatement3, new BoundLabelStatement(node.Syntax, node.ExitLabel)));
		}

		private BoundNode VisitInfiniteLoop(BoundDoLoopStatement node)
		{
			DoLoopBlockSyntax doLoopBlockSyntax = (DoLoopBlockSyntax)node.Syntax;
			bool num = ShouldGenerateUnstructuredExceptionHandlingResumeCode(node);
			BoundLabelStatement boundLabelStatement = null;
			if (num)
			{
				boundLabelStatement = RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(doLoopBlockSyntax.DoStatement);
			}
			LabelSymbol label = GenerateLabel("start");
			BoundStatement boundStatement = new BoundLabelStatement(doLoopBlockSyntax.DoStatement, label);
			if (boundLabelStatement != null)
			{
				boundStatement = Concat(boundLabelStatement, boundStatement);
			}
			BoundStatement statement = (BoundStatement)Visit(node.Body);
			BoundStatement boundStatement2 = null;
			if (num)
			{
				boundStatement2 = RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(doLoopBlockSyntax);
			}
			bool num2 = this.get_Instrument((BoundNode)node);
			if (num2 && doLoopBlockSyntax.LoopStatement != null)
			{
				boundStatement2 = _instrumenterOpt.InstrumentDoLoopEpilogue(node, boundStatement2);
			}
			statement = Concat(statement, boundStatement2);
			if (num2)
			{
				return new BoundStatementList(doLoopBlockSyntax, ImmutableArray.Create<BoundStatement>(boundStatement, _instrumenterOpt.InstrumentDoLoopStatementEntryOrConditionalGotoStart(node, null), statement, new BoundLabelStatement(doLoopBlockSyntax.DoStatement, node.ContinueLabel), new BoundGotoStatement(doLoopBlockSyntax.DoStatement, label, null), new BoundLabelStatement(doLoopBlockSyntax.DoStatement, node.ExitLabel)));
			}
			return new BoundStatementList(doLoopBlockSyntax, ImmutableArray.Create<BoundStatement>(boundStatement, statement, new BoundLabelStatement(node.Syntax, node.ContinueLabel), new BoundGotoStatement(node.Syntax, label, null), new BoundLabelStatement(node.Syntax, node.ExitLabel)));
		}

		public override BoundNode VisitEraseStatement(BoundEraseStatement node)
		{
			if (node.Clauses.Length == 1)
			{
				BoundAssignmentOperator boundAssignmentOperator = node.Clauses[0];
				return Visit(new BoundExpressionStatement(boundAssignmentOperator.Syntax, boundAssignmentOperator));
			}
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			ImmutableArray<BoundAssignmentOperator>.Enumerator enumerator = node.Clauses.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundAssignmentOperator current = enumerator.Current;
				instance.Add((BoundStatement)Visit(new BoundExpressionStatement(current.Syntax, current)));
			}
			return new BoundStatementList(node.Syntax, instance.ToImmutableAndFree());
		}

		public override BoundNode VisitExitStatement(BoundExitStatement node)
		{
			BoundStatement boundStatement = new BoundGotoStatement(node.Syntax, node.Label, null);
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement = Concat(RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax), boundStatement);
			}
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentExitStatement(node, boundStatement);
			}
			return boundStatement;
		}

		public override BoundNode VisitExpressionStatement(BoundExpressionStatement node)
		{
			if (IsOmittedBoundCall(node.Expression))
			{
				return null;
			}
			BoundStatement boundStatement = (BoundStatement)base.VisitExpressionStatement(node);
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, canThrow: true);
			}
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentExpressionStatement(node, boundStatement);
			}
			return boundStatement;
		}

		private bool IsOmittedBoundCall(BoundExpression expression)
		{
			if ((_flags & RewritingFlags.AllowOmissionOfConditionalCalls) == RewritingFlags.AllowOmissionOfConditionalCalls)
			{
				switch (expression.Kind)
				{
				case BoundKind.ConditionalAccess:
					return IsOmittedBoundCall(((BoundConditionalAccess)expression).AccessExpression);
				case BoundKind.Call:
					return ((BoundCall)expression).Method.CallsAreOmitted(expression.Syntax, expression.SyntaxTree);
				}
			}
			return false;
		}

		public override BoundNode VisitFieldAccess(BoundFieldAccess node)
		{
			BoundExpression boundExpression = (node.FieldSymbol.IsShared ? null : VisitExpressionNode(node.ReceiverOpt));
			if (node.FieldSymbol.IsTupleField)
			{
				return MakeTupleFieldAccess(node.Syntax, node.FieldSymbol, boundExpression, node.ConstantValueOpt, node.IsLValue);
			}
			return node.Update(boundExpression, node.FieldSymbol, node.IsLValue, node.SuppressVirtualCalls, null, node.Type);
		}

		private BoundExpression MakeTupleFieldAccess(SyntaxNode syntax, FieldSymbol tupleField, BoundExpression rewrittenReceiver, ConstantValue constantValueOpt, bool isLValue)
		{
			NamedTypeSymbol tupleUnderlyingType = tupleField.ContainingType.TupleUnderlyingType;
			FieldSymbol tupleUnderlyingField = tupleField.TupleUnderlyingField;
			if ((object)tupleUnderlyingField == null)
			{
				return MakeBadFieldAccess(syntax, tupleField, rewrittenReceiver);
			}
			if (!TypeSymbol.Equals(tupleUnderlyingField.ContainingType, tupleUnderlyingType, TypeCompareKind.ConsiderEverything))
			{
				WellKnownMember tupleTypeMember = TupleTypeSymbol.GetTupleTypeMember(8, 8);
				FieldSymbol fieldSymbol = (FieldSymbol)TupleTypeSymbol.GetWellKnownMemberInType(tupleUnderlyingType.OriginalDefinition, tupleTypeMember, _diagnostics, syntax);
				if ((object)fieldSymbol == null)
				{
					return MakeBadFieldAccess(syntax, tupleField, rewrittenReceiver);
				}
				do
				{
					FieldSymbol fieldSymbol2 = fieldSymbol.AsMember(tupleUnderlyingType);
					rewrittenReceiver = new BoundFieldAccess(syntax, rewrittenReceiver, fieldSymbol2, isLValue, fieldSymbol2.Type);
					tupleUnderlyingType = tupleUnderlyingType.TypeArgumentsNoUseSiteDiagnostics[7].TupleUnderlyingType;
				}
				while (!TypeSymbol.Equals(tupleUnderlyingField.ContainingType, tupleUnderlyingType, TypeCompareKind.ConsiderEverything));
			}
			return new BoundFieldAccess(syntax, rewrittenReceiver, tupleUnderlyingField, isLValue, tupleUnderlyingField.Type);
		}

		private static BoundBadExpression MakeBadFieldAccess(SyntaxNode syntax, FieldSymbol tupleField, BoundExpression rewrittenReceiver)
		{
			return new BoundBadExpression(syntax, LookupResultKind.Empty, ImmutableArray.Create((Symbol)tupleField), ImmutableArray.Create(rewrittenReceiver), tupleField.Type, hasErrors: true);
		}

		public override BoundNode VisitFieldInitializer(BoundFieldInitializer node)
		{
			return VisitFieldOrPropertyInitializer(node, ImmutableArray<Symbol>.CastUp(node.InitializedFields));
		}

		public override BoundNode VisitPropertyInitializer(BoundPropertyInitializer node)
		{
			return VisitFieldOrPropertyInitializer(node, ImmutableArray<Symbol>.CastUp(node.InitializedProperties));
		}

		private BoundNode VisitFieldOrPropertyInitializer(BoundFieldOrPropertyInitializer node, ImmutableArray<Symbol> initializedSymbols)
		{
			SyntaxNode syntax = node.Syntax;
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance(initializedSymbols.Length);
			BoundExpression boundExpression = null;
			if (!initializedSymbols.First().IsShared)
			{
				boundExpression = new BoundMeReference(syntax, _currentMethodOrLambda.ContainingType);
				boundExpression.SetWasCompilerGenerated();
			}
			BoundObjectInitializerExpression boundObjectInitializerExpression = null;
			bool flag = true;
			if (node.InitialValue.Kind == BoundKind.ObjectCreationExpression || node.InitialValue.Kind == BoundKind.NewT)
			{
				BoundObjectCreationExpressionBase boundObjectCreationExpressionBase = (BoundObjectCreationExpressionBase)node.InitialValue;
				if (boundObjectCreationExpressionBase.InitializerOpt != null && boundObjectCreationExpressionBase.InitializerOpt.Kind == BoundKind.ObjectInitializerExpression)
				{
					boundObjectInitializerExpression = (BoundObjectInitializerExpression)boundObjectCreationExpressionBase.InitializerOpt;
					flag = boundObjectInitializerExpression.CreateTemporaryLocalForInitialization;
				}
			}
			bool flag2 = this.get_Instrument((BoundNode)node);
			int num = initializedSymbols.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				Symbol symbol = initializedSymbols[i];
				BoundExpression boundExpression2;
				if (initializedSymbols.Length > 1)
				{
					if (symbol.Kind == SymbolKind.Field)
					{
						FieldSymbol fieldSymbol = (FieldSymbol)symbol;
						boundExpression2 = new BoundFieldAccess(syntax, boundExpression, fieldSymbol, isLValue: true, fieldSymbol.Type);
					}
					else
					{
						PropertySymbol propertySymbol = (PropertySymbol)symbol;
						boundExpression2 = new BoundPropertyAccess(syntax, propertySymbol, null, PropertyAccessKind.Set, propertySymbol.HasSet, boundExpression, ImmutableArray<BoundExpression>.Empty);
					}
				}
				else
				{
					boundExpression2 = node.MemberAccessExpressionOpt;
				}
				BoundStatement boundStatement;
				if (!flag)
				{
					AddPlaceholderReplacement(boundObjectInitializerExpression.PlaceholderOpt, boundExpression2);
					boundStatement = BoundExpressionExtensions.ToStatement(VisitExpressionNode(node.InitialValue));
					RemovePlaceholderReplacement(boundObjectInitializerExpression.PlaceholderOpt);
				}
				else
				{
					boundStatement = BoundExpressionExtensions.ToStatement(VisitExpression(new BoundAssignmentOperator(syntax, boundExpression2, node.InitialValue, suppressObjectClone: false)));
				}
				if (flag2)
				{
					boundStatement = _instrumenterOpt.InstrumentFieldOrPropertyInitializer(node, boundStatement, i, flag);
				}
				instance.Add(boundStatement);
			}
			return new BoundStatementList(node.Syntax, instance.ToImmutableAndFree());
		}

		public override BoundNode VisitForEachStatement(BoundForEachStatement node)
		{
			ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
			ArrayBuilder<BoundStatement> instance2 = ArrayBuilder<BoundStatement>.GetInstance();
			BoundExpression boundExpression;
			if (node.Collection.Kind == BoundKind.Conversion)
			{
				BoundConversion obj = (BoundConversion)node.Collection;
				BoundExpression operand = obj.Operand;
				boundExpression = ((obj.ExplicitCastInCode || BoundExpressionExtensions.IsNothingLiteral(operand) || (!TypeSymbolExtensions.IsArrayType(operand.Type) && !TypeSymbolExtensions.IsStringType(operand.Type))) ? node.Collection : operand);
			}
			else
			{
				boundExpression = node.Collection;
			}
			TypeSymbol type = boundExpression.Type;
			BoundExpression boundExpression2 = boundExpression;
			if ((object)node.DeclaredOrInferredLocalOpt != null)
			{
				SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, node.ControlVariable.Type, SynthesizedLocalKind.LoweringTemp);
				new BoundLocal(node.Syntax, synthesizedLocal, node.ControlVariable.Type);
				bool replacedNode = false;
				boundExpression2 = (BoundExpression)LocalVariableSubstituter.Replace(boundExpression, node.DeclaredOrInferredLocalOpt, synthesizedLocal, base.RecursionDepth, ref replacedNode);
				if (replacedNode)
				{
					instance.Add(synthesizedLocal);
					if (_symbolsCapturedWithoutCopyCtor == null)
					{
						_symbolsCapturedWithoutCopyCtor = new HashSet<Symbol>();
					}
					_symbolsCapturedWithoutCopyCtor.Add(synthesizedLocal);
				}
			}
			if (node.EnumeratorInfo.CollectionPlaceholder != null)
			{
				AddPlaceholderReplacement(node.EnumeratorInfo.CollectionPlaceholder, VisitExpressionNode(boundExpression2).MakeRValue());
			}
			if (TypeSymbolExtensions.IsArrayType(type) && ((ArrayTypeSymbol)type).IsSZArray)
			{
				RewriteForEachArrayOrString(node, instance2, instance, isArray: true, boundExpression2);
			}
			else if (TypeSymbolExtensions.IsStringType(type))
			{
				RewriteForEachArrayOrString(node, instance2, instance, isArray: false, boundExpression2);
			}
			else if (!node.Collection.HasErrors)
			{
				RewriteForEachIEnumerable(node, instance2, instance);
			}
			if (node.EnumeratorInfo.CollectionPlaceholder != null)
			{
				RemovePlaceholderReplacement(node.EnumeratorInfo.CollectionPlaceholder);
			}
			return new BoundBlock(node.Syntax, default(SyntaxList<StatementSyntax>), instance.ToImmutableAndFree(), instance2.ToImmutableAndFree());
		}

		private void RewriteForEachArrayOrString(BoundForEachStatement node, ArrayBuilder<BoundStatement> statements, ArrayBuilder<LocalSymbol> locals, bool isArray, BoundExpression collectionExpression)
		{
			ForEachBlockSyntax forEachBlockSyntax = (ForEachBlockSyntax)node.Syntax;
			bool flag = ShouldGenerateUnstructuredExceptionHandlingResumeCode(node);
			ImmutableArray<BoundStatement> immutableArray = default(ImmutableArray<BoundStatement>);
			if (flag)
			{
				immutableArray = RegisterUnstructuredExceptionHandlingResumeTarget(forEachBlockSyntax, canThrow: true);
			}
			_ = node.ControlVariable.Type;
			ForEachEnumeratorInfo enumeratorInfo = node.EnumeratorInfo;
			if (collectionExpression.Kind == BoundKind.Conversion)
			{
				BoundConversion boundConversion = (BoundConversion)collectionExpression;
				if (!boundConversion.ExplicitCastInCode && TypeSymbolExtensions.IsArrayType(boundConversion.Operand.Type))
				{
					collectionExpression = boundConversion.Operand;
				}
			}
			TypeSymbol type = collectionExpression.Type;
			BoundLocal boundLocal = null;
			BoundStatement boundStatement = CreateLocalAndAssignment(forEachBlockSyntax.ForEachStatement, collectionExpression.MakeRValue(), out boundLocal, locals, SynthesizedLocalKind.ForEachArray);
			if (!immutableArray.IsDefaultOrEmpty)
			{
				boundStatement = new BoundStatementList(boundStatement.Syntax, immutableArray.Add(boundStatement));
			}
			if (this.get_Instrument((BoundNode)node))
			{
				boundStatement = _instrumenterOpt.InstrumentForEachLoopInitialization(node, boundStatement);
			}
			statements.Add(boundStatement);
			BoundLocal boundLocal2 = null;
			NamedTypeSymbol specialTypeWithUseSiteDiagnostics = GetSpecialTypeWithUseSiteDiagnostics(SpecialType.System_Int32, forEachBlockSyntax);
			BoundStatement item = CreateLocalAndAssignment(forEachBlockSyntax.ForEachStatement, new BoundLiteral(forEachBlockSyntax, ConstantValue.Default(SpecialType.System_Int32), specialTypeWithUseSiteDiagnostics), out boundLocal2, locals, SynthesizedLocalKind.ForEachArrayIndex);
			statements.Add(item);
			BoundExpression boundExpression = null;
			if (isArray)
			{
				boundExpression = new BoundArrayLength(forEachBlockSyntax, boundLocal, specialTypeWithUseSiteDiagnostics);
			}
			else
			{
				Symbol specialTypeMember = GetSpecialTypeMember(SpecialMember.System_String__Length);
				boundExpression = new BoundCall(forEachBlockSyntax, (MethodSymbol)specialTypeMember, null, boundLocal, ImmutableArray<BoundExpression>.Empty, null, specialTypeWithUseSiteDiagnostics);
			}
			BoundExpression value;
			if (isArray)
			{
				TypeSymbol elementType = ((ArrayTypeSymbol)type).ElementType;
				value = new BoundArrayAccess(forEachBlockSyntax, boundLocal.MakeRValue(), ImmutableArray.Create((BoundExpression)boundLocal2.MakeRValue()), isLValue: false, elementType);
			}
			else
			{
				TypeSymbol elementType = GetSpecialType(SpecialType.System_Char);
				MethodSymbol result = null;
				value = ((!TryGetSpecialMember<MethodSymbol>(out result, SpecialMember.System_String__Chars, forEachBlockSyntax)) ? ((BoundExpression)new BoundBadExpression(forEachBlockSyntax, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create((BoundExpression)boundLocal2.MakeRValue()), elementType, hasErrors: true)) : ((BoundExpression)new BoundCall(forEachBlockSyntax, result, null, boundLocal, ImmutableArray.Create((BoundExpression)boundLocal2.MakeRValue()), null, elementType)));
			}
			if (enumeratorInfo.CurrentPlaceholder != null)
			{
				AddPlaceholderReplacement(enumeratorInfo.CurrentPlaceholder, value);
			}
			BoundStatement boundStatement2 = BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(forEachBlockSyntax, node.ControlVariable, enumeratorInfo.CurrentConversion, suppressObjectClone: false, node.ControlVariable.Type));
			boundStatement2.SetWasCompilerGenerated();
			boundStatement2 = (BoundStatement)Visit(boundStatement2);
			BoundStatement incrementAssignment = CreateIndexIncrement(forEachBlockSyntax, boundLocal2);
			BoundStatementList boundStatementList = CreateLoweredWhileStatements(node, boundExpression, boundLocal2, boundStatement2, incrementAssignment, flag);
			statements.AddRange(boundStatementList.Statements);
			if (enumeratorInfo.CurrentPlaceholder != null)
			{
				RemovePlaceholderReplacement(enumeratorInfo.CurrentPlaceholder);
			}
		}

		private BoundStatement CreateLocalAndAssignment(StatementSyntax syntaxNode, BoundExpression initExpression, out BoundLocal boundLocal, ArrayBuilder<LocalSymbol> locals, SynthesizedLocalKind kind)
		{
			TypeSymbol type = initExpression.Type;
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, type, kind, syntaxNode);
			locals.Add(synthesizedLocal);
			boundLocal = new BoundLocal(syntaxNode, synthesizedLocal, type);
			BoundExpressionStatement boundExpressionStatement = BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(syntaxNode, boundLocal, VisitAndGenerateObjectCloneIfNeeded(initExpression), suppressObjectClone: true, type));
			boundExpressionStatement.SetWasCompilerGenerated();
			return boundExpressionStatement;
		}

		private BoundStatement CreateIndexIncrement(VisualBasicSyntaxNode syntaxNode, BoundLocal boundIndex)
		{
			TypeSymbol type = boundIndex.Type;
			BoundBinaryOperator right = new BoundBinaryOperator(syntaxNode, BinaryOperatorKind.Add, boundIndex.MakeRValue(), new BoundLiteral(syntaxNode, ConstantValue.Create(1), type), @checked: true, type);
			BoundStatement node = BoundNodeExtensions.MakeCompilerGenerated(BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(syntaxNode, boundIndex, right, suppressObjectClone: false, type)));
			node = (BoundStatement)Visit(node);
			if (this.Instrument)
			{
				node = SyntheticBoundNodeFactory.HiddenSequencePoint(node);
			}
			return node;
		}

		private BoundStatementList CreateLoweredWhileStatements(BoundForEachStatement forEachStatement, BoundExpression limit, BoundLocal index, BoundStatement currentAssignment, BoundStatement incrementAssignment, bool generateUnstructuredExceptionHandlingResumeCode)
		{
			BoundStatement item = (BoundStatement)Visit(forEachStatement.Body);
			SyntaxNode syntax = forEachStatement.Syntax;
			BoundStatement boundStatement = null;
			if (generateUnstructuredExceptionHandlingResumeCode)
			{
				boundStatement = new BoundStatementList(syntax, RegisterUnstructuredExceptionHandlingResumeTarget(syntax, canThrow: true));
			}
			if (this.get_Instrument((BoundNode)forEachStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentForEachLoopEpilogue(forEachStatement, boundStatement);
			}
			if (boundStatement != null)
			{
				incrementAssignment = new BoundStatementList(syntax, ImmutableArray.Create(boundStatement, incrementAssignment));
			}
			ImmutableArray<BoundStatement> statements = ImmutableArray.Create(currentAssignment, item, new BoundLabelStatement(syntax, forEachStatement.ContinueLabel), incrementAssignment);
			BoundBlock rewrittenBody = new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ((object)forEachStatement.DeclaredOrInferredLocalOpt != null) ? ImmutableArray.Create(forEachStatement.DeclaredOrInferredLocalOpt) : ImmutableArray<LocalSymbol>.Empty, statements);
			NamedTypeSymbol specialTypeWithUseSiteDiagnostics = GetSpecialTypeWithUseSiteDiagnostics(SpecialType.System_Boolean, syntax);
			BoundExpression expression = TransformRewrittenBinaryOperator(new BoundBinaryOperator(syntax, BinaryOperatorKind.LessThan, index.MakeRValue(), limit, @checked: false, specialTypeWithUseSiteDiagnostics));
			return (BoundStatementList)RewriteWhileStatement(forEachStatement, VisitExpressionNode(expression), rewrittenBody, new GeneratedLabelSymbol("postIncrement"), forEachStatement.ExitLabel);
		}

		private void RewriteForEachIEnumerable(BoundForEachStatement node, ArrayBuilder<BoundStatement> statements, ArrayBuilder<LocalSymbol> locals)
		{
			ForEachBlockSyntax forEachBlockSyntax = (ForEachBlockSyntax)node.Syntax;
			ForEachEnumeratorInfo enumeratorInfo = node.EnumeratorInfo;
			bool flag = enumeratorInfo.NeedToDispose && !InsideValidUnstructuredExceptionHandlingOnErrorContext();
			UnstructuredExceptionHandlingContext saved = default(UnstructuredExceptionHandlingContext);
			if (flag)
			{
				saved = LeaveUnstructuredExceptionHandlingContext(node);
			}
			bool flag2 = ShouldGenerateUnstructuredExceptionHandlingResumeCode(node);
			ImmutableArray<BoundStatement> immutableArray = default(ImmutableArray<BoundStatement>);
			if (flag2)
			{
				immutableArray = RegisterUnstructuredExceptionHandlingResumeTarget(forEachBlockSyntax, canThrow: true);
			}
			BoundLocal boundLocal = null;
			BoundStatement boundStatement = CreateLocalAndAssignment(forEachBlockSyntax.ForEachStatement, enumeratorInfo.GetEnumerator, out boundLocal, locals, SynthesizedLocalKind.ForEachEnumerator);
			if (!immutableArray.IsDefaultOrEmpty)
			{
				boundStatement = new BoundStatementList(boundStatement.Syntax, immutableArray.Add(boundStatement));
			}
			if (this.get_Instrument((BoundNode)node))
			{
				boundStatement = _instrumenterOpt.InstrumentForEachLoopInitialization(node, boundStatement);
			}
			AddPlaceholderReplacement(enumeratorInfo.EnumeratorPlaceholder, boundLocal);
			if (enumeratorInfo.CurrentPlaceholder != null)
			{
				AddPlaceholderReplacement(enumeratorInfo.CurrentPlaceholder, VisitExpressionNode(enumeratorInfo.Current));
			}
			BoundExpressionStatement boundExpressionStatement = BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(forEachBlockSyntax, node.ControlVariable, enumeratorInfo.CurrentConversion, suppressObjectClone: false, node.ControlVariable.Type));
			boundExpressionStatement.SetWasCompilerGenerated();
			ImmutableArray<BoundStatement> statements2 = ImmutableArray.Create((BoundStatement)Visit(boundExpressionStatement), (BoundStatement)Visit(node.Body));
			BoundBlock block = new BoundBlock(forEachBlockSyntax, default(SyntaxList<StatementSyntax>), ((object)node.DeclaredOrInferredLocalOpt != null) ? ImmutableArray.Create(node.DeclaredOrInferredLocalOpt) : ImmutableArray<LocalSymbol>.Empty, statements2);
			BoundStatement boundStatement2 = new BoundLabelStatement(forEachBlockSyntax, node.ContinueLabel);
			if (flag2)
			{
				boundStatement2 = Concat(boundStatement2, new BoundStatementList(forEachBlockSyntax, RegisterUnstructuredExceptionHandlingResumeTarget(forEachBlockSyntax, canThrow: true)));
			}
			if (this.get_Instrument((BoundNode)node))
			{
				boundStatement2 = _instrumenterOpt.InstrumentForEachLoopEpilogue(node, boundStatement2);
			}
			block = AppendToBlock(block, boundStatement2);
			BoundStatementList boundStatementList = (BoundStatementList)RewriteWhileStatement(node, VisitExpressionNode(enumeratorInfo.MoveNext), block, new GeneratedLabelSymbol("MoveNextLabel"), node.ExitLabel);
			if (enumeratorInfo.CurrentPlaceholder != null)
			{
				RemovePlaceholderReplacement(enumeratorInfo.CurrentPlaceholder);
			}
			if (enumeratorInfo.NeedToDispose)
			{
				BoundStatement item = GenerateDisposeCallForForeachAndUsing(node.Syntax, boundLocal, VisitExpressionNode(enumeratorInfo.DisposeCondition), enumeratorInfo.IsOrInheritsFromOrImplementsIDisposable, VisitExpressionNode(enumeratorInfo.DisposeCast));
				if (!flag)
				{
					statements.Add(boundStatement);
					statements.Add(boundStatementList);
					if (flag2)
					{
						RegisterUnstructuredExceptionHandlingResumeTarget(forEachBlockSyntax, canThrow: true, statements);
					}
					statements.Add(item);
				}
				else
				{
					BoundTryStatement boundTryStatement = new BoundTryStatement(forEachBlockSyntax, new BoundBlock(forEachBlockSyntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(boundStatement, boundStatementList)), ImmutableArray<BoundCatchBlock>.Empty, new BoundBlock(forEachBlockSyntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(item)), null);
					boundTryStatement.SetWasCompilerGenerated();
					statements.Add(boundTryStatement);
				}
			}
			else
			{
				statements.Add(boundStatement);
				statements.AddRange(boundStatementList);
			}
			if (flag)
			{
				RestoreUnstructuredExceptionHandlingContext(node, saved);
			}
			RemovePlaceholderReplacement(enumeratorInfo.EnumeratorPlaceholder);
		}

		public BoundStatement GenerateDisposeCallForForeachAndUsing(SyntaxNode syntaxNode, BoundLocal rewrittenBoundLocal, BoundExpression rewrittenCondition, bool IsOrInheritsFromOrImplementsIDisposable, BoundExpression rewrittenDisposeConversion)
		{
			MethodSymbol result = null;
			if (!TryGetSpecialMember<MethodSymbol>(out result, SpecialMember.System_IDisposable__Dispose, syntaxNode))
			{
				return BoundExpressionExtensions.ToStatement(new BoundBadExpression(syntaxNode, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, (rewrittenCondition != null) ? ImmutableArray.Create(rewrittenBoundLocal, rewrittenCondition) : ImmutableArray.Create((BoundExpression)rewrittenBoundLocal), ErrorTypeSymbol.UnknownResultType, hasErrors: true));
			}
			NamedTypeSymbol specialTypeWithUseSiteDiagnostics = GetSpecialTypeWithUseSiteDiagnostics(SpecialType.System_Void, syntaxNode);
			TypeSymbol type = rewrittenBoundLocal.Type;
			BoundStatement boundStatement;
			if (IsOrInheritsFromOrImplementsIDisposable && (type.IsValueType || TypeSymbolExtensions.IsTypeParameter(type)))
			{
				boundStatement = BoundExpressionExtensions.ToStatement(new BoundCall(syntaxNode, result, null, rewrittenBoundLocal, ImmutableArray<BoundExpression>.Empty, null, specialTypeWithUseSiteDiagnostics));
				boundStatement.SetWasCompilerGenerated();
				if (type.IsValueType)
				{
					return boundStatement;
				}
			}
			else
			{
				boundStatement = BoundExpressionExtensions.ToStatement(new BoundCall(syntaxNode, result, null, rewrittenDisposeConversion, ImmutableArray<BoundExpression>.Empty, null, specialTypeWithUseSiteDiagnostics));
				boundStatement.SetWasCompilerGenerated();
			}
			return RewriteIfStatement(syntaxNode, rewrittenCondition, boundStatement, null, null);
		}

		public override BoundNode VisitForToStatement(BoundForToStatement node)
		{
			BoundExpression boundExpression = VisitExpressionNode(node.ControlVariable);
			bool num = TypeSymbolExtensions.IsObjectType(boundExpression.Type);
			BoundExpression rewrittenInitialValue = VisitExpressionNode(node.InitialValue);
			BoundExpression rewrittenLimit = VisitExpressionNode(node.LimitValue);
			BoundExpression rewrittenStep = VisitExpressionNode(node.StepValue);
			if (!num)
			{
				return FinishNonObjectForLoop(node, boundExpression, rewrittenInitialValue, rewrittenLimit, rewrittenStep);
			}
			return FinishObjectForLoop(node, boundExpression, rewrittenInitialValue, rewrittenLimit, rewrittenStep);
		}

		private BoundBlock FinishNonObjectForLoop(BoundForToStatement forStatement, BoundExpression rewrittenControlVariable, BoundExpression rewrittenInitialValue, BoundExpression rewrittenLimit, BoundExpression rewrittenStep)
		{
			ForBlockSyntax forBlockSyntax = (ForBlockSyntax)forStatement.Syntax;
			bool num = ShouldGenerateUnstructuredExceptionHandlingResumeCode(forStatement);
			ImmutableArray<BoundStatement> immutableArray = default(ImmutableArray<BoundStatement>);
			if (num)
			{
				immutableArray = RegisterUnstructuredExceptionHandlingResumeTarget(forBlockSyntax, canThrow: true);
			}
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			bool flag = WillDoAtLeastOneIteration(rewrittenInitialValue, rewrittenLimit, rewrittenStep);
			ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
			rewrittenInitialValue = CacheToLocalIfNotConst(_currentMethodOrLambda, rewrittenInitialValue, instance2, instance, SynthesizedLocalKind.ForInitialValue, forBlockSyntax);
			rewrittenLimit = CacheToLocalIfNotConst(_currentMethodOrLambda, rewrittenLimit, instance2, instance, SynthesizedLocalKind.ForLimit, forBlockSyntax);
			rewrittenStep = CacheToLocalIfNotConst(_currentMethodOrLambda, rewrittenStep, instance2, instance, SynthesizedLocalKind.ForStep, forBlockSyntax);
			SynthesizedLocal synthesizedLocal = null;
			if (forStatement.OperatorsOpt != null)
			{
				AddPlaceholderReplacement(forStatement.OperatorsOpt.LeftOperandPlaceholder, rewrittenStep);
				AddPlaceholderReplacement(forStatement.OperatorsOpt.RightOperandPlaceholder, rewrittenStep);
				UpdatePlaceholderReplacement(value: VisitExpressionNode(forStatement.OperatorsOpt.Subtraction), placeholder: forStatement.OperatorsOpt.RightOperandPlaceholder);
				BoundExpression boundExpression = VisitExpressionNode(forStatement.OperatorsOpt.GreaterThanOrEqual);
				synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, boundExpression.Type, SynthesizedLocalKind.ForDirection, forBlockSyntax);
				instance2.Add(synthesizedLocal);
				instance.Add(new BoundAssignmentOperator(forStatement.OperatorsOpt.Syntax, new BoundLocal(forStatement.OperatorsOpt.Syntax, synthesizedLocal, synthesizedLocal.Type), boundExpression, suppressObjectClone: true, synthesizedLocal.Type));
				RemovePlaceholderReplacement(forStatement.OperatorsOpt.LeftOperandPlaceholder);
				RemovePlaceholderReplacement(forStatement.OperatorsOpt.RightOperandPlaceholder);
			}
			else if ((object)rewrittenStep.ConstantValueOpt == null && !TypeSymbolExtensions.IsSignedIntegralType(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(rewrittenStep.Type)) && !TypeSymbolExtensions.IsUnsignedIntegralType(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(rewrittenStep.Type)))
			{
				BoundExpression boundExpression2 = rewrittenStep;
				BoundExpression boundExpression3 = null;
				if (TypeSymbolExtensions.IsNullableType(boundExpression2.Type))
				{
					boundExpression3 = NullableHasValue(boundExpression2);
					boundExpression2 = NullableValueOrDefault(boundExpression2);
				}
				if (!TypeSymbolExtensions.IsNumericType(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(boundExpression2.Type)))
				{
					throw ExceptionUtilities.Unreachable;
				}
				TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(boundExpression2.Type);
				BoundExpression boundExpression4 = new BoundLiteral(rewrittenStep.Syntax, ConstantValue.Default(enumUnderlyingTypeOrSelf.SpecialType), boundExpression2.Type);
				if (TypeSymbolExtensions.IsDecimalType(enumUnderlyingTypeOrSelf))
				{
					boundExpression4 = RewriteDecimalConstant(boundExpression4, boundExpression4.ConstantValueOpt, _topMethod, _diagnostics);
				}
				BoundExpression boundExpression5 = TransformRewrittenBinaryOperator(new BoundBinaryOperator(rewrittenStep.Syntax, BinaryOperatorKind.GreaterThanOrEqual, boundExpression2, boundExpression4, @checked: false, GetSpecialType(SpecialType.System_Boolean)));
				if (boundExpression3 != null)
				{
					boundExpression5 = MakeBooleanBinaryExpression(boundExpression5.Syntax, BinaryOperatorKind.AndAlso, boundExpression3, boundExpression5);
				}
				synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, boundExpression5.Type, SynthesizedLocalKind.ForDirection, forBlockSyntax);
				instance2.Add(synthesizedLocal);
				instance.Add(new BoundAssignmentOperator(boundExpression5.Syntax, new BoundLocal(boundExpression5.Syntax, synthesizedLocal, synthesizedLocal.Type), boundExpression5, suppressObjectClone: true, synthesizedLocal.Type));
			}
			if (instance.Count > 0)
			{
				rewrittenInitialValue = new BoundSequence(rewrittenInitialValue.Syntax, ImmutableArray<LocalSymbol>.Empty, instance.ToImmutable(), rewrittenInitialValue, rewrittenInitialValue.Type);
			}
			instance.Free();
			BoundStatement boundStatement = new BoundExpressionStatement(rewrittenInitialValue.Syntax, new BoundAssignmentOperator(rewrittenInitialValue.Syntax, rewrittenControlVariable, rewrittenInitialValue, suppressObjectClone: true, rewrittenInitialValue.Type));
			if (!immutableArray.IsDefaultOrEmpty)
			{
				boundStatement = new BoundStatementList(boundStatement.Syntax, immutableArray.Add(boundStatement));
			}
			bool flag2 = this.get_Instrument((BoundNode)forStatement);
			if (flag2)
			{
				boundStatement = _instrumenterOpt.InstrumentForLoopInitialization(forStatement, boundStatement);
			}
			BoundStatement item = (BoundStatement)Visit(forStatement.Body);
			BoundStatement boundStatement2 = RewriteForLoopIncrement(rewrittenControlVariable, rewrittenStep, forStatement.Checked, forStatement.OperatorsOpt);
			if (num)
			{
				boundStatement2 = RegisterUnstructuredExceptionHandlingResumeTarget(forBlockSyntax, boundStatement2, canThrow: true);
			}
			if (flag2)
			{
				boundStatement2 = _instrumenterOpt.InstrumentForLoopIncrement(forStatement, boundStatement2);
			}
			BoundExpression condition = RewriteForLoopCondition(rewrittenControlVariable, rewrittenLimit, rewrittenStep, forStatement.OperatorsOpt, synthesizedLocal);
			LabelSymbol label = GenerateLabel("start");
			BoundStatement boundStatement3 = new BoundConditionalGoto(forBlockSyntax, condition, jumpIfTrue: true, label);
			GeneratedLabelSymbol generatedLabelSymbol = null;
			BoundStatement boundStatement4 = null;
			if (!flag)
			{
				generatedLabelSymbol = new GeneratedLabelSymbol("PostIncrement");
				new BoundLabelStatement(forBlockSyntax, generatedLabelSymbol);
				boundStatement4 = new BoundGotoStatement(forBlockSyntax, generatedLabelSymbol, null);
				if (flag2)
				{
					boundStatement4 = SyntheticBoundNodeFactory.HiddenSequencePoint(boundStatement4);
				}
			}
			ArrayBuilder<BoundStatement> instance3 = ArrayBuilder<BoundStatement>.GetInstance();
			instance3.Add(boundStatement);
			if (boundStatement4 != null)
			{
				instance3.Add(boundStatement4);
			}
			instance3.Add(new BoundLabelStatement(forBlockSyntax, label));
			instance3.Add(item);
			instance3.Add(new BoundLabelStatement(forBlockSyntax, forStatement.ContinueLabel));
			instance3.Add(boundStatement2);
			if ((object)generatedLabelSymbol != null)
			{
				BoundStatement boundStatement5 = new BoundLabelStatement(forBlockSyntax, generatedLabelSymbol);
				if (flag2)
				{
					boundStatement4 = SyntheticBoundNodeFactory.HiddenSequencePoint(boundStatement5);
				}
				instance3.Add(boundStatement5);
			}
			if (flag2)
			{
				boundStatement3 = SyntheticBoundNodeFactory.HiddenSequencePoint(boundStatement3);
			}
			instance3.Add(boundStatement3);
			instance3.Add(new BoundLabelStatement(forBlockSyntax, forStatement.ExitLabel));
			LocalSymbol declaredOrInferredLocalOpt = forStatement.DeclaredOrInferredLocalOpt;
			if ((object)declaredOrInferredLocalOpt != null)
			{
				instance2.Add(declaredOrInferredLocalOpt);
			}
			return new BoundBlock(forBlockSyntax, default(SyntaxList<StatementSyntax>), instance2.ToImmutableAndFree(), instance3.ToImmutableAndFree());
		}

		private static bool WillDoAtLeastOneIteration(BoundExpression rewrittenInitialValue, BoundExpression rewrittenLimit, BoundExpression rewrittenStep)
		{
			ConstantValue constantValueOpt = rewrittenInitialValue.ConstantValueOpt;
			if ((object)constantValueOpt == null)
			{
				return false;
			}
			ConstantValue constantValueOpt2 = rewrittenLimit.ConstantValueOpt;
			if ((object)constantValueOpt2 == null)
			{
				return false;
			}
			ConstantValue constantValueOpt3 = rewrittenStep.ConstantValueOpt;
			bool flag;
			if ((object)constantValueOpt3 == null)
			{
				if (!TypeSymbolExtensions.IsUnsignedIntegralType(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(rewrittenStep.Type)))
				{
					return false;
				}
				flag = false;
			}
			else
			{
				flag = constantValueOpt3.IsNegativeNumeric;
			}
			if (constantValueOpt.IsUnsigned)
			{
				ulong uInt64Value = constantValueOpt.UInt64Value;
				ulong uInt64Value2 = constantValueOpt2.UInt64Value;
				return flag ? (uInt64Value >= uInt64Value2) : (uInt64Value <= uInt64Value2);
			}
			if (constantValueOpt.IsIntegral)
			{
				long int64Value = constantValueOpt.Int64Value;
				long int64Value2 = constantValueOpt2.Int64Value;
				return flag ? (int64Value >= int64Value2) : (int64Value <= int64Value2);
			}
			if (constantValueOpt.IsDecimal)
			{
				decimal decimalValue = constantValueOpt.DecimalValue;
				decimal decimalValue2 = constantValueOpt2.DecimalValue;
				return flag ? (decimal.Compare(decimalValue, decimalValue2) >= 0) : (decimal.Compare(decimalValue, decimalValue2) <= 0);
			}
			double doubleValue = constantValueOpt.DoubleValue;
			double doubleValue2 = constantValueOpt2.DoubleValue;
			return flag ? (doubleValue >= doubleValue2) : (doubleValue <= doubleValue2);
		}

		private BoundBlock FinishObjectForLoop(BoundForToStatement forStatement, BoundExpression rewrittenControlVariable, BoundExpression rewrittenInitialValue, BoundExpression rewrittenLimit, BoundExpression rewrittenStep)
		{
			ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
			ForBlockSyntax forBlockSyntax = (ForBlockSyntax)forStatement.Syntax;
			bool flag = ShouldGenerateUnstructuredExceptionHandlingResumeCode(forStatement);
			TypeSymbol type = rewrittenControlVariable.Type;
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, type, SynthesizedLocalKind.ForInitialValue, forBlockSyntax);
			instance.Add(synthesizedLocal);
			BoundLocal boundLocal = new BoundLocal(forBlockSyntax, synthesizedLocal, isLValue: true, synthesizedLocal.Type);
			ImmutableArray<BoundExpression> immutableArray = ImmutableArray.Create<BoundExpression>(rewrittenControlVariable.MakeRValue(), rewrittenInitialValue, rewrittenLimit, rewrittenStep, boundLocal, rewrittenControlVariable);
			MethodSymbol result = null;
			BoundExpression boundExpression = ((!TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl__ForLoopInitObj, forBlockSyntax)) ? ((BoundExpression)new BoundBadExpression(rewrittenLimit.Syntax, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, immutableArray, Compilation.GetSpecialType(SpecialType.System_Boolean), hasErrors: true)) : ((BoundExpression)new BoundCall(rewrittenLimit.Syntax, result, null, null, immutableArray, null, Compilation.GetSpecialType(SpecialType.System_Boolean), suppressObjectClone: true)));
			bool num = this.get_Instrument((BoundNode)forStatement);
			if (num)
			{
				boundExpression = _instrumenterOpt.InstrumentObjectForLoopInitCondition(forStatement, boundExpression, _currentMethodOrLambda);
			}
			BoundStatement boundStatement = new BoundConditionalGoto(forBlockSyntax, boundExpression, jumpIfTrue: false, forStatement.ExitLabel);
			if (flag)
			{
				boundStatement = RegisterUnstructuredExceptionHandlingResumeTarget(forBlockSyntax, boundStatement, canThrow: true);
			}
			if (num)
			{
				boundStatement = _instrumenterOpt.InstrumentForLoopInitialization(forStatement, boundStatement);
			}
			BoundStatement boundStatement2 = (BoundStatement)Visit(forStatement.Body);
			immutableArray = ImmutableArray.Create(rewrittenControlVariable.MakeRValue(), boundLocal.MakeRValue(), rewrittenControlVariable);
			MethodSymbol result2 = null;
			BoundExpression boundExpression2 = ((!TryGetWellknownMember<MethodSymbol>(out result2, WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl__ForNextCheckObj, forBlockSyntax)) ? ((BoundExpression)new BoundBadExpression(rewrittenLimit.Syntax, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, immutableArray, Compilation.GetSpecialType(SpecialType.System_Boolean), hasErrors: true)) : ((BoundExpression)new BoundCall(rewrittenLimit.Syntax, result2, null, null, immutableArray, null, Compilation.GetSpecialType(SpecialType.System_Boolean), suppressObjectClone: true)));
			LabelSymbol label = GenerateLabel("start");
			if (num)
			{
				boundExpression2 = _instrumenterOpt.InstrumentObjectForLoopCondition(forStatement, boundExpression2, _currentMethodOrLambda);
			}
			BoundStatement boundStatement3 = new BoundConditionalGoto(forBlockSyntax, boundExpression2, jumpIfTrue: true, label);
			if (flag)
			{
				boundStatement3 = RegisterUnstructuredExceptionHandlingResumeTarget(forBlockSyntax, boundStatement3, canThrow: true);
			}
			if (num)
			{
				boundStatement3 = _instrumenterOpt.InstrumentForLoopIncrement(forStatement, boundStatement3);
			}
			BoundStatement boundStatement4 = new BoundLabelStatement(forBlockSyntax, forStatement.ContinueLabel);
			if (num)
			{
				boundStatement4 = SyntheticBoundNodeFactory.HiddenSequencePoint(boundStatement4);
				boundStatement3 = SyntheticBoundNodeFactory.HiddenSequencePoint(boundStatement3);
			}
			ImmutableArray<BoundStatement> statements = ImmutableArray.Create<BoundStatement>(boundStatement, new BoundLabelStatement(forBlockSyntax, label), boundStatement2, boundStatement4, boundStatement3, new BoundLabelStatement(forBlockSyntax, forStatement.ExitLabel));
			LocalSymbol declaredOrInferredLocalOpt = forStatement.DeclaredOrInferredLocalOpt;
			if ((object)declaredOrInferredLocalOpt != null)
			{
				instance.Add(declaredOrInferredLocalOpt);
			}
			return new BoundBlock(forBlockSyntax, default(SyntaxList<StatementSyntax>), instance.ToImmutableAndFree(), statements);
		}

		private BoundStatement RewriteForLoopIncrement(BoundExpression controlVariable, BoundExpression stepValue, bool isChecked, BoundForToUserDefinedOperators operatorsOpt)
		{
			BoundExpression boundExpression2;
			if (operatorsOpt == null)
			{
				BoundExpression boundExpression = controlVariable;
				BoundExpression condition = null;
				if (TypeSymbolExtensions.IsNullableType(controlVariable.Type))
				{
					condition = MakeBooleanBinaryExpression(controlVariable.Syntax, BinaryOperatorKind.And, NullableHasValue(stepValue), NullableHasValue(controlVariable));
					boundExpression = NullableValueOrDefault(controlVariable);
					stepValue = NullableValueOrDefault(stepValue);
				}
				boundExpression2 = TransformRewrittenBinaryOperator(new BoundBinaryOperator(stepValue.Syntax, BinaryOperatorKind.Add, boundExpression.MakeRValue(), stepValue, isChecked, boundExpression.Type));
				if (TypeSymbolExtensions.IsNullableType(controlVariable.Type))
				{
					boundExpression2 = MakeTernaryConditionalExpression(boundExpression2.Syntax, condition, WrapInNullable(boundExpression2, controlVariable.Type), NullableNull(controlVariable.Syntax, controlVariable.Type));
				}
			}
			else
			{
				AddPlaceholderReplacement(operatorsOpt.LeftOperandPlaceholder, controlVariable.MakeRValue());
				AddPlaceholderReplacement(operatorsOpt.RightOperandPlaceholder, stepValue);
				boundExpression2 = VisitExpressionNode(operatorsOpt.Addition);
				RemovePlaceholderReplacement(operatorsOpt.LeftOperandPlaceholder);
				RemovePlaceholderReplacement(operatorsOpt.RightOperandPlaceholder);
			}
			return new BoundExpressionStatement(stepValue.Syntax, new BoundAssignmentOperator(stepValue.Syntax, controlVariable, boundExpression2, suppressObjectClone: true, controlVariable.Type));
		}

		private BoundExpression NegateIfStepNegative(BoundExpression value, BoundExpression step)
		{
			NamedTypeSymbol primitiveType = step.Type.ContainingAssembly.GetPrimitiveType(PrimitiveTypeCode.Int32);
			int value2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(step.Type).SpecialType.VBForToShiftBits();
			BoundLiteral right = new BoundLiteral(value.Syntax, ConstantValue.Create(value2), primitiveType);
			BoundExpression left = TransformRewrittenBinaryOperator(new BoundBinaryOperator(value.Syntax, BinaryOperatorKind.RightShift, step, right, @checked: false, step.Type));
			return TransformRewrittenBinaryOperator(new BoundBinaryOperator(value.Syntax, BinaryOperatorKind.Xor, left, value, @checked: false, value.Type));
		}

		private BoundExpression RewriteForLoopCondition(BoundExpression controlVariable, BoundExpression limit, BoundExpression stepValue, BoundForToUserDefinedOperators operatorsOpt, SynthesizedLocal positiveFlag)
		{
			if (operatorsOpt != null)
			{
				AddPlaceholderReplacement(operatorsOpt.LeftOperandPlaceholder, controlVariable.MakeRValue());
				AddPlaceholderReplacement(operatorsOpt.RightOperandPlaceholder, limit);
				BoundExpression result = MakeTernaryConditionalExpression(operatorsOpt.Syntax, new BoundLocal(operatorsOpt.Syntax, positiveFlag, isLValue: false, positiveFlag.Type), VisitExpressionNode(operatorsOpt.LessThanOrEqual), VisitExpressionNode(operatorsOpt.GreaterThanOrEqual));
				RemovePlaceholderReplacement(operatorsOpt.LeftOperandPlaceholder);
				RemovePlaceholderReplacement(operatorsOpt.RightOperandPlaceholder);
				return result;
			}
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Boolean);
			if (TypeSymbolExtensions.IsUnsignedIntegralType(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(stepValue.Type)))
			{
				return TransformRewrittenBinaryOperator(new BoundBinaryOperator(limit.Syntax, BinaryOperatorKind.LessThanOrEqual, controlVariable.MakeRValue(), limit, @checked: false, specialType));
			}
			ConstantValue constantValueOpt = stepValue.ConstantValueOpt;
			if ((object)constantValueOpt != null)
			{
				BinaryOperatorKind operatorKind;
				if (constantValueOpt.IsNegativeNumeric)
				{
					operatorKind = BinaryOperatorKind.GreaterThanOrEqual;
				}
				else
				{
					if (!constantValueOpt.IsNumeric)
					{
						throw ExceptionUtilities.UnexpectedValue(constantValueOpt);
					}
					operatorKind = BinaryOperatorKind.LessThanOrEqual;
				}
				return TransformRewrittenBinaryOperator(new BoundBinaryOperator(limit.Syntax, operatorKind, controlVariable.MakeRValue(), limit, @checked: false, specialType));
			}
			if (TypeSymbolExtensions.IsSignedIntegralType(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(stepValue.Type)))
			{
				return TransformRewrittenBinaryOperator(new BoundBinaryOperator(stepValue.Syntax, BinaryOperatorKind.LessThanOrEqual, NegateIfStepNegative(controlVariable.MakeRValue(), stepValue), NegateIfStepNegative(limit, stepValue), @checked: false, specialType));
			}
			BoundExpression boundExpression = null;
			if (TypeSymbolExtensions.IsNullableType(controlVariable.Type))
			{
				boundExpression = MakeBooleanBinaryExpression(controlVariable.Syntax, BinaryOperatorKind.And, NullableHasValue(limit), NullableHasValue(controlVariable));
				controlVariable = NullableValueOrDefault(controlVariable);
				limit = NullableValueOrDefault(limit);
			}
			if ((object)positiveFlag != null)
			{
				BoundExpression whenTrue = TransformRewrittenBinaryOperator(new BoundBinaryOperator(limit.Syntax, BinaryOperatorKind.LessThanOrEqual, controlVariable.MakeRValue(), limit, @checked: false, specialType));
				BoundExpression whenFalse = TransformRewrittenBinaryOperator(new BoundBinaryOperator(limit.Syntax, BinaryOperatorKind.GreaterThanOrEqual, controlVariable.MakeRValue(), limit, @checked: false, specialType));
				BoundExpression condition = new BoundLocal(limit.Syntax, positiveFlag, isLValue: false, positiveFlag.Type);
				BoundExpression boundExpression2 = MakeTernaryConditionalExpression(limit.Syntax, condition, whenTrue, whenFalse);
				if (boundExpression != null)
				{
					boundExpression2 = MakeBooleanBinaryExpression(boundExpression2.Syntax, BinaryOperatorKind.AndAlso, boundExpression, boundExpression2);
				}
				return boundExpression2;
			}
			throw ExceptionUtilities.Unreachable;
		}

		public override BoundNode VisitGotoStatement(BoundGotoStatement node)
		{
			if (node.LabelExpressionOpt != null)
			{
				node = node.Update(node.Label, null);
			}
			BoundStatement boundStatement = (BoundStatement)base.VisitGotoStatement(node);
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement = Concat(RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax), boundStatement);
			}
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentGotoStatement(node, boundStatement);
			}
			return boundStatement;
		}

		public override BoundNode VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
		{
			SyntaxNode syntax = node.Syntax;
			FieldSymbol hostObjectField = _previousSubmissionFields.GetHostObjectField();
			BoundMeReference receiverOpt = new BoundMeReference(syntax, _topMethod.ContainingType);
			return new BoundFieldAccess(syntax, receiverOpt, hostObjectField, isLValue: false, hostObjectField.Type);
		}

		public override BoundNode VisitIfStatement(BoundIfStatement node)
		{
			SyntaxNode syntax = node.Syntax;
			bool flag = ShouldGenerateUnstructuredExceptionHandlingResumeCode(node);
			ImmutableArray<BoundStatement> unstructuredExceptionHandlingResumeTarget = default(ImmutableArray<BoundStatement>);
			if (flag)
			{
				unstructuredExceptionHandlingResumeTarget = RegisterUnstructuredExceptionHandlingResumeTarget(syntax, canThrow: true);
			}
			BoundExpression rewrittenCondition = VisitExpressionNode(node.Condition);
			BoundStatement boundStatement = (BoundStatement)Visit(node.Consequence);
			bool flag2 = node.AlternativeOpt != null;
			bool flag3 = this.get_Instrument((BoundNode)node);
			if (flag3)
			{
				BoundStatement epilogueOpt = null;
				switch (VisualBasicExtensions.Kind(syntax))
				{
				case SyntaxKind.MultiLineIfBlock:
				case SyntaxKind.ElseIfBlock:
					if (flag && (OptimizationLevelIsDebug || flag2))
					{
						epilogueOpt = RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(boundStatement.Syntax);
					}
					flag2 = false;
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(syntax));
				case SyntaxKind.SingleLineIfStatement:
					break;
				}
				boundStatement = Concat(boundStatement, _instrumenterOpt.InstrumentIfStatementConsequenceEpilogue(node, epilogueOpt));
			}
			if (flag && flag2)
			{
				boundStatement = Concat(boundStatement, RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(boundStatement.Syntax));
			}
			BoundStatement boundStatement2 = (BoundStatement)Visit(node.AlternativeOpt);
			if (flag3 && boundStatement2 != null)
			{
				if (VisualBasicExtensions.Kind(syntax) != SyntaxKind.SingleLineIfStatement)
				{
					if (node.AlternativeOpt.Syntax is ElseBlockSyntax)
					{
						BoundStatement epilogueOpt2 = null;
						if (flag && OptimizationLevelIsDebug)
						{
							epilogueOpt2 = RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(boundStatement2.Syntax);
						}
						boundStatement2 = Concat(boundStatement2, _instrumenterOpt.InstrumentIfStatementAlternativeEpilogue(node, epilogueOpt2));
						boundStatement2 = PrependWithPrologue(boundStatement2, _instrumenterOpt.CreateIfStatementAlternativePrologue(node));
					}
				}
				else if (node.AlternativeOpt.Syntax is SingleLineElseClauseSyntax)
				{
					boundStatement2 = PrependWithPrologue(boundStatement2, _instrumenterOpt.CreateIfStatementAlternativePrologue(node));
				}
			}
			if (flag3)
			{
				rewrittenCondition = _instrumenterOpt.InstrumentIfStatementCondition(node, rewrittenCondition, _currentMethodOrLambda);
			}
			return RewriteIfStatement(node.Syntax, rewrittenCondition, boundStatement, boundStatement2, flag3 ? node : null, unstructuredExceptionHandlingResumeTarget);
		}

		private BoundStatement RewriteIfStatement(SyntaxNode syntaxNode, BoundExpression rewrittenCondition, BoundStatement rewrittenConsequence, BoundStatement rewrittenAlternative, BoundStatement instrumentationTargetOpt, ImmutableArray<BoundStatement> unstructuredExceptionHandlingResumeTarget = default(ImmutableArray<BoundStatement>))
		{
			LabelSymbol label = GenerateLabel("afterif");
			BoundStatement boundStatement = new BoundLabelStatement(syntaxNode, label);
			if (rewrittenAlternative == null)
			{
				BoundStatement boundStatement2 = new BoundConditionalGoto(syntaxNode, rewrittenCondition, jumpIfTrue: false, label);
				if (!unstructuredExceptionHandlingResumeTarget.IsDefaultOrEmpty)
				{
					boundStatement2 = new BoundStatementList(boundStatement2.Syntax, unstructuredExceptionHandlingResumeTarget.Add(boundStatement2));
				}
				if (instrumentationTargetOpt != null)
				{
					switch (VisualBasicExtensions.Kind(instrumentationTargetOpt.Syntax))
					{
					case SyntaxKind.SingleLineIfStatement:
					case SyntaxKind.MultiLineIfBlock:
					case SyntaxKind.ElseIfBlock:
						boundStatement2 = _instrumenterOpt.InstrumentIfStatementConditionalGoto((BoundIfStatement)instrumentationTargetOpt, boundStatement2);
						break;
					case SyntaxKind.CaseBlock:
						boundStatement2 = _instrumenterOpt.InstrumentCaseBlockConditionalGoto((BoundCaseBlock)instrumentationTargetOpt, boundStatement2);
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(instrumentationTargetOpt.Syntax));
					}
					boundStatement = ((VisualBasicExtensions.Kind(instrumentationTargetOpt.Syntax) != SyntaxKind.MultiLineIfBlock) ? SyntheticBoundNodeFactory.HiddenSequencePoint(boundStatement) : _instrumenterOpt.InstrumentIfStatementAfterIfStatement((BoundIfStatement)instrumentationTargetOpt, boundStatement));
				}
				return new BoundStatementList(syntaxNode, ImmutableArray.Create(boundStatement2, rewrittenConsequence, boundStatement));
			}
			LabelSymbol label2 = GenerateLabel("alternative");
			BoundStatement boundStatement3 = new BoundConditionalGoto(syntaxNode, rewrittenCondition, jumpIfTrue: false, label2);
			if (!unstructuredExceptionHandlingResumeTarget.IsDefaultOrEmpty)
			{
				boundStatement3 = new BoundStatementList(boundStatement3.Syntax, unstructuredExceptionHandlingResumeTarget.Add(boundStatement3));
			}
			if (instrumentationTargetOpt != null)
			{
				switch (VisualBasicExtensions.Kind(instrumentationTargetOpt.Syntax))
				{
				case SyntaxKind.SingleLineIfStatement:
				case SyntaxKind.MultiLineIfBlock:
				case SyntaxKind.ElseIfBlock:
					boundStatement3 = _instrumenterOpt.InstrumentIfStatementConditionalGoto((BoundIfStatement)instrumentationTargetOpt, boundStatement3);
					break;
				case SyntaxKind.CaseBlock:
					boundStatement3 = _instrumenterOpt.InstrumentCaseBlockConditionalGoto((BoundCaseBlock)instrumentationTargetOpt, boundStatement3);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(instrumentationTargetOpt.Syntax));
				}
			}
			return new BoundStatementList(syntaxNode, ImmutableArray.Create<BoundStatement>(boundStatement3, rewrittenConsequence, new BoundGotoStatement(syntaxNode, label, null), new BoundLabelStatement(syntaxNode, label2), rewrittenAlternative, boundStatement));
		}

		public override BoundNode VisitInterpolatedStringExpression(BoundInterpolatedStringExpression node)
		{
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			if (node.IsEmpty)
			{
				return syntheticBoundNodeFactory.StringLiteral(ConstantValue.Create(string.Empty));
			}
			if (!node.HasInterpolations)
			{
				string stringValue = ((BoundLiteral)node.Contents[0]).Value.StringValue;
				return syntheticBoundNodeFactory.StringLiteral(ConstantValue.Create(stringValue.Replace("{{", "{").Replace("}}", "}")));
			}
			return InvokeInterpolatedStringFactory(node, node.Type, "Format", node.Type, syntheticBoundNodeFactory);
		}

		private BoundExpression RewriteInterpolatedStringConversion(BoundConversion conversion)
		{
			_ = conversion.Type;
			BoundInterpolatedStringExpression boundInterpolatedStringExpression = (BoundInterpolatedStringExpression)conversion.Operand;
			Binder binder = boundInterpolatedStringExpression.Binder;
			return InvokeInterpolatedStringFactory(boundInterpolatedStringExpression, binder.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_FormattableStringFactory, conversion.Syntax, _diagnostics), "Create", conversion.Type, new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, boundInterpolatedStringExpression.Syntax, _compilationState, _diagnostics));
		}

		private BoundExpression InvokeInterpolatedStringFactory(BoundInterpolatedStringExpression node, TypeSymbol factoryType, string factoryMethodName, TypeSymbol targetType, SyntheticBoundNodeFactory factory)
		{
			bool flag = false;
			if (!TypeSymbolExtensions.IsErrorType(factoryType))
			{
				Binder binder = node.Binder;
				LookupResult instance = LookupResult.GetInstance();
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
				binder.LookupMember(instance, factoryType, factoryMethodName, 0, LookupOptions.MustNotBeInstance | LookupOptions.AllMethodsOfAnyArity | LookupOptions.MethodsOnly, ref useSiteInfo);
				_diagnostics.Add(node, useSiteInfo);
				if (instance.Kind == LookupResultKind.Inaccessible)
				{
					flag = true;
				}
				else if (!instance.IsGood)
				{
					instance.Free();
					goto IL_0283;
				}
				BoundMethodGroup group = BoundNodeExtensions.MakeCompilerGenerated(new BoundMethodGroup(node.Syntax, null, instance.Symbols.ToDowncastedImmutable<MethodSymbol>(), instance.Kind, null, QualificationKind.QualifiedViaTypeName));
				instance.Free();
				PooledStringBuilder instance2 = PooledStringBuilder.GetInstance();
				ArrayBuilder<BoundExpression> instance3 = ArrayBuilder<BoundExpression>.GetInstance();
				int num = -1;
				instance3.Add(null);
				ImmutableArray<BoundNode>.Enumerator enumerator = node.Contents.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundNode current = enumerator.Current;
					switch (current.Kind)
					{
					case BoundKind.Literal:
						instance2.Builder.Append(((BoundLiteral)current).Value.StringValue);
						break;
					case BoundKind.Interpolation:
					{
						num++;
						BoundInterpolation boundInterpolation = (BoundInterpolation)current;
						StringBuilder builder = instance2.Builder;
						builder.Append('{');
						builder.Append(num.ToString(CultureInfo.InvariantCulture));
						if (boundInterpolation.AlignmentOpt != null)
						{
							builder.Append(',');
							builder.Append(boundInterpolation.AlignmentOpt.ConstantValueOpt.Int64Value.ToString(CultureInfo.InvariantCulture));
						}
						if (boundInterpolation.FormatStringOpt != null)
						{
							builder.Append(":");
							builder.Append(boundInterpolation.FormatStringOpt.Value.StringValue);
						}
						builder.Append('}');
						builder = null;
						instance3.Add(boundInterpolation.Expression);
						break;
					}
					default:
						throw ExceptionUtilities.Unreachable;
					}
				}
				instance3[0] = BoundNodeExtensions.MakeCompilerGenerated(factory.StringLiteral(ConstantValue.Create(instance2.ToStringAndFree())));
				BoundExpression boundExpression = BoundNodeExtensions.MakeCompilerGenerated(binder.MakeRValue(binder.BindInvocationExpression(node.Syntax, node.Syntax, TypeCharacter.None, group, instance3.ToImmutableAndFree(), default(ImmutableArray<string>), _diagnostics, null, allowConstructorCall: false, suppressAbstractCallDiagnostics: false, isDefaultMemberAccess: false, null, forceExpandedForm: true), _diagnostics));
				if (!boundExpression.Type.Equals(targetType))
				{
					boundExpression = BoundNodeExtensions.MakeCompilerGenerated(binder.ApplyImplicitConversion(node.Syntax, targetType, boundExpression, _diagnostics));
				}
				if (!flag && !boundExpression.HasErrors)
				{
					return VisitExpression(boundExpression);
				}
			}
			goto IL_0283;
			IL_0283:
			ReportDiagnostic(node, ErrorFactory.ErrorInfo(ERRID.ERR_InterpolatedStringFactoryError, factoryType.Name, factoryMethodName), _diagnostics);
			return factory.Convert(targetType, factory.BadExpression((BoundExpression)base.VisitInterpolatedStringExpression(node)));
		}

		public override BoundNode VisitLabelStatement(BoundLabelStatement node)
		{
			BoundStatement boundStatement = (BoundStatement)base.VisitLabelStatement(node);
			if ((object)_currentLineTemporary != null && (object)_currentMethodOrLambda == _topMethod && !node.WasCompilerGenerated && VisualBasicExtensions.Kind(node.Syntax) == SyntaxKind.LabelStatement)
			{
				LabelStatementSyntax labelStatementSyntax = (LabelStatementSyntax)node.Syntax;
				if (VisualBasicExtensions.Kind(labelStatementSyntax.LabelToken) == SyntaxKind.IntegerLiteralToken)
				{
					int result = 0;
					int.TryParse(labelStatementSyntax.LabelToken.ValueText, NumberStyles.None, CultureInfo.InvariantCulture, out result);
					BoundStatement boundStatement2 = BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(node.Syntax, new BoundLocal(node.Syntax, _currentLineTemporary, _currentLineTemporary.Type), new BoundLiteral(node.Syntax, ConstantValue.Create(result), _currentLineTemporary.Type), suppressObjectClone: true));
					if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
					{
						boundStatement2 = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement2, canThrow: false);
					}
					boundStatement = new BoundStatementList(node.Syntax, ImmutableArray.Create(boundStatement, boundStatement2));
				}
			}
			if (node.Label.IsFromCompilation(_compilationState.Compilation) && this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentLabelStatement(node, boundStatement);
			}
			return boundStatement;
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			_hasLambdas = true;
			MethodSymbol currentMethodOrLambda = _currentMethodOrLambda;
			_currentMethodOrLambda = node.LambdaSymbol;
			BoundNode result = base.VisitLambda(node);
			_currentMethodOrLambda = currentMethodOrLambda;
			return result;
		}

		public override BoundNode VisitLateAddressOfOperator(BoundLateAddressOfOperator node)
		{
			if (_inExpressionLambda)
			{
				return base.VisitLateAddressOfOperator(node);
			}
			NamedTypeSymbol targetType = (NamedTypeSymbol)node.Type;
			BoundExpression expression = BuildDelegateRelaxationLambda(node.Syntax, targetType, node.MemberAccess, node.Binder, _diagnostics);
			return VisitExpressionNode(expression);
		}

		private static BoundExpression BuildDelegateRelaxationLambda(SyntaxNode syntaxNode, NamedTypeSymbol targetType, BoundLateMemberAccess boundMember, Binder binder, BindingDiagnosticBag diagnostics)
		{
			MethodSymbol delegateInvokeMethod = targetType.DelegateInvokeMethod;
			TypeSymbol returnType = delegateInvokeMethod.ReturnType;
			ImmutableArray<ParameterSymbol> parameters = delegateInvokeMethod.Parameters;
			int length = parameters.Length;
			BoundLambdaParameterSymbol[] array = new BoundLambdaParameterSymbol[length - 1 + 1];
			Location location = syntaxNode.GetLocation();
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				ParameterSymbol parameterSymbol = parameters[i];
				array[i] = new BoundLambdaParameterSymbol(GeneratedNames.MakeDelegateRelaxationParameterName(i), parameterSymbol.Ordinal, parameterSymbol.Type, parameterSymbol.IsByRef, syntaxNode, location);
			}
			SynthesizedLambdaSymbol synthesizedLambdaSymbol = new SynthesizedLambdaSymbol(SynthesizedLambdaKind.LateBoundAddressOfLambda, syntaxNode, array.AsImmutableOrNull(), returnType, binder);
			BoundExpression[] array2 = new BoundExpression[length - 1 + 1];
			int num2 = array.Length - 1;
			for (int j = 0; j <= num2; j++)
			{
				BoundLambdaParameterSymbol boundLambdaParameterSymbol = array[j];
				BoundParameter boundParameter = new BoundParameter(syntaxNode, boundLambdaParameterSymbol, boundLambdaParameterSymbol.Type);
				boundParameter.SetWasCompilerGenerated();
				array2[j] = boundParameter;
			}
			LambdaBodyBinder lambdaBodyBinder = new LambdaBodyBinder(synthesizedLambdaSymbol, binder);
			BoundExpression boundExpression = lambdaBodyBinder.BindLateBoundInvocation(syntaxNode, null, boundMember, array2.AsImmutableOrNull(), default(ImmutableArray<string>), diagnostics, suppressLateBindingResolutionDiagnostics: true);
			boundExpression.SetWasCompilerGenerated();
			ImmutableArray<BoundStatement> immutableArray = default(ImmutableArray<BoundStatement>);
			if (synthesizedLambdaSymbol.IsSub)
			{
				if (BoundExpressionExtensions.IsLateBound(boundExpression))
				{
					boundExpression = BoundExpressionExtensions.SetLateBoundAccessKind(boundExpression, LateBoundAccessKind.Call);
				}
				BoundStatement[] array3 = new BoundStatement[2];
				BoundStatement boundStatement = new BoundExpressionStatement(syntaxNode, boundExpression);
				boundStatement.SetWasCompilerGenerated();
				array3[0] = boundStatement;
				boundStatement = new BoundReturnStatement(syntaxNode, null, null, null);
				boundStatement.SetWasCompilerGenerated();
				array3[1] = boundStatement;
				immutableArray = array3.AsImmutableOrNull();
			}
			else
			{
				boundExpression = lambdaBodyBinder.ApplyImplicitConversion(syntaxNode, returnType, boundExpression, diagnostics);
				BoundReturnStatement boundReturnStatement = new BoundReturnStatement(syntaxNode, boundExpression, null, null);
				boundReturnStatement.SetWasCompilerGenerated();
				immutableArray = ImmutableArray.Create((BoundStatement)boundReturnStatement);
			}
			BoundBlock boundBlock = new BoundBlock(syntaxNode, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, immutableArray);
			boundBlock.SetWasCompilerGenerated();
			BoundLambda boundLambda = new BoundLambda(syntaxNode, synthesizedLambdaSymbol, boundBlock, ImmutableBindingDiagnostic<AssemblySymbol>.Empty, null, ConversionKind.DelegateRelaxationLevelWidening, MethodConversionKind.Identity);
			boundLambda.SetWasCompilerGenerated();
			return new BoundDirectCast(syntaxNode, boundLambda, ConversionKind.DelegateRelaxationLevelWidening, targetType);
		}

		private BoundExpression LateMakeReceiverArgument(SyntaxNode node, BoundExpression rewrittenReceiver, TypeSymbol objectType)
		{
			if (rewrittenReceiver == null)
			{
				return MakeNullLiteral(node, objectType);
			}
			if (!TypeSymbolExtensions.IsObjectType(rewrittenReceiver.Type))
			{
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
				ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(rewrittenReceiver.Type, objectType, ref useSiteInfo);
				((BindingDiagnosticBag<AssemblySymbol>)_diagnostics).Add(node, useSiteInfo);
				rewrittenReceiver = new BoundDirectCast(node, rewrittenReceiver, conversionKind, objectType);
			}
			return rewrittenReceiver;
		}

		private BoundExpression LateMakeContainerArgument(SyntaxNode node, BoundExpression receiver, TypeSymbol containerType, TypeSymbol typeType)
		{
			if (receiver != null)
			{
				return MakeNullLiteral(node, typeType);
			}
			return MakeGetTypeExpression(node, containerType, typeType);
		}

		private BoundExpression LateMakeTypeArgumentArrayArgument(SyntaxNode node, BoundTypeArguments arguments, TypeSymbol typeArrayType)
		{
			if (arguments == null)
			{
				return MakeNullLiteral(node, typeArrayType);
			}
			return MakeArrayOfGetTypeExpressions(node, arguments.Arguments, typeArrayType);
		}

		private BoundExpression LateMakeCopyBackArray(SyntaxNode node, ImmutableArray<bool> flags, TypeSymbol booleanArrayType)
		{
			TypeSymbol elementType = ((ArrayTypeSymbol)booleanArrayType).ElementType;
			if (flags.IsDefaultOrEmpty)
			{
				return MakeNullLiteral(node, booleanArrayType);
			}
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Int32);
			BoundExpression item = new BoundLiteral(node, ConstantValue.Create(flags.Length), specialType);
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			ImmutableArray<bool>.Enumerator enumerator = flags.GetEnumerator();
			while (enumerator.MoveNext())
			{
				bool current = enumerator.Current;
				instance.Add(MakeBooleanLiteral(node, current, elementType));
			}
			BoundArrayInitialization initializerOpt = new BoundArrayInitialization(node, instance.ToImmutableAndFree(), null);
			return new BoundArrayCreation(node, ImmutableArray.Create(item), initializerOpt, booleanArrayType);
		}

		private BoundExpression LateMakeArgumentArrayArgument(SyntaxNode node, ImmutableArray<BoundExpression> rewrittenArguments, ImmutableArray<string> argumentNames, TypeSymbol objectArrayType)
		{
			if (argumentNames.IsDefaultOrEmpty)
			{
				return LateMakeArgumentArrayArgumentNoNamed(node, rewrittenArguments, objectArrayType);
			}
			int num = 0;
			ImmutableArray<string>.Enumerator enumerator = argumentNames.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					num++;
				}
			}
			int num2 = rewrittenArguments.Length - num;
			TypeSymbol elementType = ((ArrayTypeSymbol)objectArrayType).ElementType;
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Int32);
			BoundExpression item = new BoundLiteral(node, ConstantValue.Create(rewrittenArguments.Length), specialType);
			BoundArrayCreation boundArrayCreation = new BoundArrayCreation(node, ImmutableArray.Create(item), null, objectArrayType);
			LocalSymbol localSymbol = new SynthesizedLocal(_currentMethodOrLambda, boundArrayCreation.Type, SynthesizedLocalKind.LoweringTemp);
			BoundLocal boundLocal = new BoundLocal(node, localSymbol, localSymbol.Type);
			BoundAssignmentOperator item2 = new BoundAssignmentOperator(node, boundLocal, boundArrayCreation, suppressObjectClone: true);
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			instance.Add(item2);
			boundLocal = boundLocal.MakeRValue();
			int num3 = rewrittenArguments.Length - 1;
			for (int i = 0; i <= num3; i++)
			{
				BoundExpression boundExpression = rewrittenArguments[i];
				boundExpression = boundExpression.MakeRValue();
				if (!TypeSymbolExtensions.IsObjectType(boundExpression.Type))
				{
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
					ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(boundExpression.Type, elementType, ref useSiteInfo);
					((BindingDiagnosticBag<AssemblySymbol>)_diagnostics).Add(node, useSiteInfo);
					boundExpression = new BoundDirectCast(node, boundExpression, conversionKind, elementType);
				}
				int value = ((i < num2) ? (num + i) : (i - num2));
				ImmutableArray<BoundExpression> indices = ImmutableArray.Create((BoundExpression)new BoundLiteral(node, ConstantValue.Create(value), specialType));
				BoundExpression left = new BoundArrayAccess(node, boundLocal, indices, elementType);
				BoundAssignmentOperator item3 = new BoundAssignmentOperator(node, left, boundExpression, suppressObjectClone: true);
				instance.Add(item3);
			}
			return new BoundSequence(node, ImmutableArray.Create(localSymbol), instance.ToImmutableAndFree(), boundLocal, boundLocal.Type);
		}

		private BoundExpression LateMakeSetArgumentArrayArgument(SyntaxNode node, BoundExpression rewrittenValue, ImmutableArray<BoundExpression> rewrittenArguments, ImmutableArray<string> argumentNames, TypeSymbol objectArrayType)
		{
			TypeSymbol elementType = ((ArrayTypeSymbol)objectArrayType).ElementType;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
			if (!TypeSymbolExtensions.IsObjectType(rewrittenValue.Type))
			{
				ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(rewrittenValue.Type, elementType, ref useSiteInfo);
				((BindingDiagnosticBag<AssemblySymbol>)_diagnostics).Add(node, useSiteInfo);
				rewrittenValue = new BoundDirectCast(node, rewrittenValue, conversionKind, elementType);
			}
			if (argumentNames.IsDefaultOrEmpty)
			{
				if (rewrittenArguments.IsDefaultOrEmpty)
				{
					rewrittenArguments = ImmutableArray.Create(rewrittenValue);
				}
				else if (argumentNames.IsDefaultOrEmpty)
				{
					rewrittenArguments = rewrittenArguments.Add(rewrittenValue);
				}
				return LateMakeArgumentArrayArgumentNoNamed(node, rewrittenArguments, objectArrayType);
			}
			int num = 0;
			ImmutableArray<string>.Enumerator enumerator = argumentNames.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					num++;
				}
			}
			int num2 = rewrittenArguments.Length - num;
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Int32);
			BoundExpression item = new BoundLiteral(node, ConstantValue.Create(rewrittenArguments.Length + 1), specialType);
			BoundArrayCreation boundArrayCreation = new BoundArrayCreation(node, ImmutableArray.Create(item), null, objectArrayType);
			LocalSymbol localSymbol = new SynthesizedLocal(_currentMethodOrLambda, boundArrayCreation.Type, SynthesizedLocalKind.LoweringTemp);
			BoundLocal boundLocal = new BoundLocal(node, localSymbol, localSymbol.Type);
			BoundAssignmentOperator item2 = new BoundAssignmentOperator(node, boundLocal, boundArrayCreation, suppressObjectClone: true);
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			instance.Add(item2);
			boundLocal = boundLocal.MakeRValue();
			int num3 = rewrittenArguments.Length - 1;
			for (int i = 0; i <= num3; i++)
			{
				BoundExpression boundExpression = rewrittenArguments[i];
				boundExpression = boundExpression.MakeRValue();
				if (!TypeSymbolExtensions.IsObjectType(boundExpression.Type))
				{
					ConversionKind conversionKind2 = Conversions.ClassifyDirectCastConversion(boundExpression.Type, elementType, ref useSiteInfo);
					_diagnostics.Add(boundExpression, useSiteInfo);
					boundExpression = new BoundDirectCast(node, boundExpression, conversionKind2, elementType);
				}
				int index = ((i < num2) ? (num + i) : (i - num2));
				BoundExpression item3 = LateAssignToArrayElement(node, boundLocal, index, boundExpression, specialType);
				instance.Add(item3);
			}
			if (!TypeSymbolExtensions.IsObjectType(rewrittenValue.Type))
			{
				ConversionKind conversionKind3 = Conversions.ClassifyDirectCastConversion(rewrittenValue.Type, elementType, ref useSiteInfo);
				_diagnostics.Add(rewrittenValue, useSiteInfo);
				rewrittenValue = new BoundDirectCast(node, rewrittenValue, conversionKind3, elementType);
			}
			BoundExpression item4 = LateAssignToArrayElement(node, boundLocal, rewrittenArguments.Length, rewrittenValue, specialType);
			instance.Add(item4);
			return new BoundSequence(node, ImmutableArray.Create(localSymbol), instance.ToImmutableAndFree(), boundLocal, boundLocal.Type);
		}

		private static BoundExpression LateAssignToArrayElement(SyntaxNode node, BoundExpression arrayRef, int index, BoundExpression value, TypeSymbol intType)
		{
			BoundExpression item = new BoundLiteral(node, ConstantValue.Create(index), intType);
			BoundExpression left = new BoundArrayAccess(node, arrayRef, ImmutableArray.Create(item), value.Type);
			return new BoundAssignmentOperator(node, left, value, suppressObjectClone: true);
		}

		private BoundExpression LateMakeArgumentArrayArgumentNoNamed(SyntaxNode node, ImmutableArray<BoundExpression> rewrittenArguments, TypeSymbol objectArrayType)
		{
			TypeSymbol elementType = ((ArrayTypeSymbol)objectArrayType).ElementType;
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Int32);
			if (rewrittenArguments.IsDefaultOrEmpty)
			{
				BoundExpression item = new BoundLiteral(node, ConstantValue.Default(ConstantValueTypeDiscriminator.Int32), specialType);
				return new BoundArrayCreation(node, ImmutableArray.Create(item), null, objectArrayType);
			}
			BoundExpression item2 = new BoundLiteral(node, ConstantValue.Create(rewrittenArguments.Length), specialType);
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			ImmutableArray<BoundExpression>.Enumerator enumerator = rewrittenArguments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				current = current.MakeRValue();
				if (!TypeSymbolExtensions.IsObjectType(current.Type))
				{
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
					ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(current.Type, elementType, ref useSiteInfo);
					_diagnostics.Add(current, useSiteInfo);
					current = new BoundDirectCast(node, current, conversionKind, elementType);
				}
				instance.Add(current);
			}
			BoundArrayInitialization initializerOpt = new BoundArrayInitialization(node, instance.ToImmutableAndFree(), null);
			return new BoundArrayCreation(node, ImmutableArray.Create(item2), initializerOpt, objectArrayType);
		}

		private BoundExpression LateMakeArgumentNameArrayArgument(SyntaxNode node, ImmutableArray<string> argumentNames, TypeSymbol stringArrayType)
		{
			TypeSymbol elementType = ((ArrayTypeSymbol)stringArrayType).ElementType;
			if (argumentNames.IsDefaultOrEmpty)
			{
				return MakeNullLiteral(node, stringArrayType);
			}
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			ImmutableArray<string>.Enumerator enumerator = argumentNames.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				if (current != null)
				{
					instance.Add(MakeStringLiteral(node, current, elementType));
				}
			}
			BoundArrayInitialization boundArrayInitialization = new BoundArrayInitialization(node, instance.ToImmutableAndFree(), null);
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Int32);
			BoundExpression item = new BoundLiteral(node, ConstantValue.Create(boundArrayInitialization.Initializers.Length), specialType);
			return new BoundArrayCreation(node, ImmutableArray.Create(item), boundArrayInitialization, stringArrayType);
		}

		private BoundExpression LateMakeConditionalCopyback(BoundExpression assignmentTarget, BoundExpression valueArrayRef, BoundExpression copyBackArrayRef, int argNum)
		{
			SyntaxNode syntax = assignmentTarget.Syntax;
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Int32);
			BoundExpression item = new BoundLiteral(syntax, ConstantValue.Create(argNum), specialType);
			ImmutableArray<BoundExpression> indices = ImmutableArray.Create(item);
			TypeSymbol elementType = ((ArrayTypeSymbol)copyBackArrayRef.Type).ElementType;
			BoundExpression condition = new BoundArrayAccess(syntax, copyBackArrayRef, ImmutableArray.Create(item), elementType).MakeRValue();
			TypeSymbol elementType2 = ((ArrayTypeSymbol)valueArrayRef.Type).ElementType;
			BoundExpression boundExpression = new BoundArrayAccess(syntax, valueArrayRef, indices, elementType2).MakeRValue();
			TypeSymbol type = assignmentTarget.Type;
			if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(type, elementType2))
			{
				MethodSymbol result = null;
				if (TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ChangeType, syntax))
				{
					BoundGetType item2 = new BoundGetType(syntax, new BoundTypeExpression(syntax, type), result.Parameters[1].Type);
					boundExpression = new BoundCall(syntax, result, null, null, ImmutableArray.Create(boundExpression, item2), null, elementType2);
				}
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(elementType2, type, ref useSiteInfo);
				boundExpression = new BoundDirectCast(syntax, boundExpression, conversionKind, type);
			}
			BoundExpression whenFalse = new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray<BoundExpression>.Empty, null, GetSpecialType(SpecialType.System_Void));
			BoundExpression whenTrue = LateMakeCopyback(syntax, assignmentTarget, boundExpression);
			BoundExpression expression = MakeTernaryConditionalExpression(syntax, condition, whenTrue, whenFalse);
			return VisitExpressionNode(expression);
		}

		private BoundExpression LateMakeCopyback(SyntaxNode syntax, BoundExpression assignmentTarget, BoundExpression convertedValue)
		{
			if (assignmentTarget.Kind == BoundKind.LateMemberAccess)
			{
				BoundLateMemberAccess memberAccess = (BoundLateMemberAccess)assignmentTarget;
				return LateSet(syntax, memberAccess, convertedValue, default(ImmutableArray<BoundExpression>), default(ImmutableArray<string>), isCopyBack: true);
			}
			if (assignmentTarget.Kind == BoundKind.LateInvocation)
			{
				BoundLateInvocation boundLateInvocation = (BoundLateInvocation)assignmentTarget;
				if (boundLateInvocation.Member.Kind == BoundKind.LateMemberAccess)
				{
					BoundLateMemberAccess memberAccess2 = (BoundLateMemberAccess)boundLateInvocation.Member;
					return LateSet(syntax, memberAccess2, convertedValue, boundLateInvocation.ArgumentsOpt, boundLateInvocation.ArgumentNamesOpt, isCopyBack: true);
				}
				return LateIndexSet(syntax, boundLateInvocation, convertedValue, isCopyBack: true);
			}
			BoundExpression item = new BoundAssignmentOperator(syntax, assignmentTarget, GenerateObjectCloneIfNeeded(convertedValue), suppressObjectClone: true);
			return new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(item), null, GetSpecialType(SpecialType.System_Void));
		}

		private BoundExpression LateIndexGet(BoundLateInvocation node, BoundExpression receiverExpr, ImmutableArray<BoundExpression> argExpressions)
		{
			SyntaxNode syntax = node.Syntax;
			MethodSymbol result = null;
			if (!TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateIndexGet, syntax))
			{
				return node;
			}
			BoundExpression item = LateMakeReceiverArgument(syntax, receiverExpr.MakeRValue(), result.Parameters[0].Type);
			BoundExpression item2 = LateMakeArgumentArrayArgument(node.Syntax, argExpressions, node.ArgumentNamesOpt, result.Parameters[1].Type);
			BoundExpression item3 = LateMakeArgumentNameArrayArgument(syntax, node.ArgumentNamesOpt, result.Parameters[2].Type);
			ImmutableArray<BoundExpression> arguments = ImmutableArray.Create(item, item2, item3);
			return new BoundCall(syntax, result, null, null, arguments, null, result.ReturnType, suppressObjectClone: true);
		}

		private BoundExpression LateSet(SyntaxNode syntax, BoundLateMemberAccess memberAccess, BoundExpression assignmentValue, ImmutableArray<BoundExpression> argExpressions, ImmutableArray<string> argNames, bool isCopyBack)
		{
			bool flag = memberAccess.ReceiverOpt != null && !memberAccess.ReceiverOpt.IsLValue;
			MethodSymbol result = null;
			bool flag2 = isCopyBack || flag;
			if (flag2)
			{
				if (!TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateSetComplex, syntax))
				{
					return new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundExpression)memberAccess), null, GetSpecialType(SpecialType.System_Void));
				}
			}
			else if (!TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateSet, syntax))
			{
				return new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundExpression)memberAccess), null, GetSpecialType(SpecialType.System_Void));
			}
			BoundExpression boundExpression = LateMakeReceiverArgument(syntax, (memberAccess.ReceiverOpt != null) ? memberAccess.ReceiverOpt.MakeRValue() : null, result.Parameters[0].Type);
			BoundExpression boundExpression2 = LateMakeContainerArgument(syntax, memberAccess.ReceiverOpt, memberAccess.ContainerTypeOpt, result.Parameters[1].Type);
			BoundLiteral boundLiteral = MakeStringLiteral(syntax, memberAccess.NameOpt, result.Parameters[2].Type);
			BoundExpression boundExpression3 = LateMakeSetArgumentArrayArgument(syntax, assignmentValue, argExpressions, argNames, result.Parameters[3].Type);
			BoundExpression boundExpression4 = LateMakeArgumentNameArrayArgument(syntax, argNames, result.Parameters[4].Type);
			BoundExpression boundExpression5 = LateMakeTypeArgumentArrayArgument(syntax, memberAccess.TypeArgumentsOpt, result.Parameters[5].Type);
			ImmutableArray<BoundExpression> arguments;
			if (!flag2)
			{
				arguments = ImmutableArray.Create<BoundExpression>(boundExpression, boundExpression2, boundLiteral, boundExpression3, boundExpression4, boundExpression5);
			}
			else
			{
				BoundExpression boundExpression6 = MakeBooleanLiteral(syntax, isCopyBack, result.Parameters[6].Type);
				BoundExpression boundExpression7 = MakeBooleanLiteral(syntax, flag, result.Parameters[7].Type);
				arguments = ImmutableArray.Create<BoundExpression>(boundExpression, boundExpression2, boundLiteral, boundExpression3, boundExpression4, boundExpression5, boundExpression6, boundExpression7);
			}
			return new BoundCall(syntax, result, null, null, arguments, null, result.ReturnType, suppressObjectClone: true);
		}

		private BoundExpression LateIndexSet(SyntaxNode syntax, BoundLateInvocation invocation, BoundExpression assignmentValue, bool isCopyBack)
		{
			bool flag = invocation.Member != null && !invocation.Member.IsLValue;
			MethodSymbol result = null;
			bool flag2 = isCopyBack || flag;
			if (flag2)
			{
				if (!TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateIndexSetComplex, syntax))
				{
					return new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundExpression)invocation), null, GetSpecialType(SpecialType.System_Void));
				}
			}
			else if (!TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateIndexSet, syntax))
			{
				return new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundExpression)invocation), null, GetSpecialType(SpecialType.System_Void));
			}
			BoundExpression boundExpression = LateMakeReceiverArgument(syntax, invocation.Member.MakeRValue(), result.Parameters[0].Type);
			BoundExpression boundExpression2 = LateMakeSetArgumentArrayArgument(syntax, assignmentValue.MakeRValue(), invocation.ArgumentsOpt, invocation.ArgumentNamesOpt, result.Parameters[1].Type);
			BoundExpression boundExpression3 = LateMakeArgumentNameArrayArgument(syntax, invocation.ArgumentNamesOpt, result.Parameters[2].Type);
			ImmutableArray<BoundExpression> arguments;
			if (!flag2)
			{
				arguments = ImmutableArray.Create(boundExpression, boundExpression2, boundExpression3);
			}
			else
			{
				BoundExpression boundExpression4 = MakeBooleanLiteral(syntax, isCopyBack, result.Parameters[3].Type);
				BoundExpression boundExpression5 = MakeBooleanLiteral(syntax, flag, result.Parameters[4].Type);
				arguments = ImmutableArray.Create<BoundExpression>(boundExpression, boundExpression2, boundExpression3, boundExpression4, boundExpression5);
			}
			return new BoundCall(syntax, result, null, null, arguments, null, result.ReturnType, suppressObjectClone: true);
		}

		private BoundExpression LateCallOrGet(BoundLateMemberAccess memberAccess, BoundExpression receiverExpression, ImmutableArray<BoundExpression> argExpressions, ImmutableArray<BoundExpression> assignmentArguments, ImmutableArray<string> argNames, bool useLateCall)
		{
			SyntaxNode syntax = memberAccess.Syntax;
			MethodSymbol result = null;
			if (useLateCall)
			{
				if (!TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateCall, syntax))
				{
					return memberAccess;
				}
			}
			else if (!TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateGet, syntax))
			{
				return memberAccess;
			}
			SynthesizedLocal synthesizedLocal = null;
			BoundLocal valueArrayRef = null;
			SynthesizedLocal synthesizedLocal2 = null;
			BoundLocal copyBackArrayRef = null;
			BoundExpression boundExpression = LateMakeArgumentArrayArgument(syntax, argExpressions, argNames, result.Parameters[3].Type);
			BoundExpression boundExpression2 = LateMakeCopyBackArray(syntax, default(ImmutableArray<bool>), result.Parameters[6].Type);
			ArrayBuilder<BoundExpression> arrayBuilder = null;
			if (!assignmentArguments.IsDefaultOrEmpty)
			{
				int num = 0;
				if (!argNames.IsDefaultOrEmpty)
				{
					ImmutableArray<string>.Enumerator enumerator = argNames.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (enumerator.Current != null)
						{
							num++;
						}
					}
				}
				int num2 = assignmentArguments.Length - num;
				bool[] array = null;
				int num3 = assignmentArguments.Length - 1;
				for (int i = 0; i <= num3; i++)
				{
					BoundExpression boundExpression3 = assignmentArguments[i];
					if (BoundExpressionExtensions.IsSupportingAssignment(boundExpression3))
					{
						if ((object)synthesizedLocal2 == null)
						{
							synthesizedLocal2 = new SynthesizedLocal(_currentMethodOrLambda, boundExpression2.Type, SynthesizedLocalKind.LoweringTemp);
							copyBackArrayRef = new BoundLocal(syntax, synthesizedLocal2, synthesizedLocal2.Type).MakeRValue();
							synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, boundExpression.Type, SynthesizedLocalKind.LoweringTemp);
							valueArrayRef = new BoundLocal(syntax, synthesizedLocal, synthesizedLocal.Type);
							boundExpression = new BoundAssignmentOperator(syntax, valueArrayRef, boundExpression, suppressObjectClone: true).MakeRValue();
							valueArrayRef = valueArrayRef.MakeRValue();
							arrayBuilder = ArrayBuilder<BoundExpression>.GetInstance(assignmentArguments.Length);
							array = new bool[assignmentArguments.Length - 1 + 1];
						}
						int num4 = ((i < num2) ? (num + i) : (i - num2));
						array[num4] = true;
						arrayBuilder.Add(LateMakeConditionalCopyback(boundExpression3, valueArrayRef, copyBackArrayRef, num4));
					}
				}
				if ((object)synthesizedLocal2 != null)
				{
					boundExpression2 = new BoundAssignmentOperator(syntax, new BoundLocal(syntax, synthesizedLocal2, synthesizedLocal2.Type), LateMakeCopyBackArray(syntax, array.AsImmutableOrNull(), synthesizedLocal2.Type), suppressObjectClone: true).MakeRValue();
				}
			}
			BoundExpression rewrittenReceiver = receiverExpression?.MakeRValue();
			BoundExpression boundExpression4 = LateMakeReceiverArgument(syntax, rewrittenReceiver, result.Parameters[0].Type);
			BoundExpression boundExpression5 = LateMakeContainerArgument(syntax, receiverExpression, memberAccess.ContainerTypeOpt, result.Parameters[1].Type);
			BoundLiteral boundLiteral = MakeStringLiteral(syntax, memberAccess.NameOpt, result.Parameters[2].Type);
			BoundExpression boundExpression6 = boundExpression;
			BoundExpression boundExpression7 = LateMakeArgumentNameArrayArgument(syntax, argNames, result.Parameters[4].Type);
			BoundExpression boundExpression8 = LateMakeTypeArgumentArrayArgument(syntax, memberAccess.TypeArgumentsOpt, result.Parameters[5].Type);
			BoundExpression boundExpression9 = boundExpression2;
			ImmutableArray<BoundExpression> arguments = ImmutableArray.Create<BoundExpression>(boundExpression4, boundExpression5, boundLiteral, boundExpression6, boundExpression7, boundExpression8, boundExpression9);
			if (useLateCall)
			{
				BoundExpression item = MakeBooleanLiteral(syntax, value: true, result.Parameters[7].Type);
				arguments = arguments.Add(item);
			}
			BoundExpression boundExpression10 = new BoundCall(syntax, result, null, null, arguments, null, result.ReturnType, suppressObjectClone: true);
			if ((object)synthesizedLocal2 != null)
			{
				SynthesizedLocal synthesizedLocal3 = new SynthesizedLocal(_currentMethodOrLambda, boundExpression10.Type, SynthesizedLocalKind.LoweringTemp);
				BoundLocal boundLocal = new BoundLocal(syntax, synthesizedLocal3, synthesizedLocal3.Type);
				BoundAssignmentOperator item2 = new BoundAssignmentOperator(syntax, boundLocal, boundExpression10, suppressObjectClone: true);
				boundExpression10 = new BoundSequence(syntax, ImmutableArray.Create((LocalSymbol)synthesizedLocal, (LocalSymbol)synthesizedLocal2, (LocalSymbol)synthesizedLocal3), ImmutableArray.Create((BoundExpression)item2).Concat(arrayBuilder.ToImmutableAndFree()), boundLocal.MakeRValue(), boundLocal.Type);
			}
			return boundExpression10;
		}

		private void LateCaptureArgsComplex(ref ArrayBuilder<SynthesizedLocal> temps, ref ImmutableArray<BoundExpression> arguments, out ImmutableArray<BoundExpression> writeTargets)
		{
			MethodSymbol currentMethodOrLambda = _currentMethodOrLambda;
			if (temps == null)
			{
				temps = ArrayBuilder<SynthesizedLocal>.GetInstance();
			}
			if (arguments.IsDefaultOrEmpty)
			{
				return;
			}
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
			ImmutableArray<BoundExpression>.Enumerator enumerator = arguments.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression boundExpression = enumerator.Current;
				BoundExpression item;
				if (!BoundExpressionExtensions.IsSupportingAssignment(boundExpression))
				{
					item = null;
				}
				else
				{
					BoundLateBoundArgumentSupportingAssignmentWithCapture boundLateBoundArgumentSupportingAssignmentWithCapture = null;
					if (boundExpression.Kind == BoundKind.LateBoundArgumentSupportingAssignmentWithCapture)
					{
						boundLateBoundArgumentSupportingAssignmentWithCapture = (BoundLateBoundArgumentSupportingAssignmentWithCapture)boundExpression;
						boundExpression = boundLateBoundArgumentSupportingAssignmentWithCapture.OriginalArgument;
					}
					UseTwiceRewriter.Result result = UseTwiceRewriter.UseTwice(currentMethodOrLambda, boundExpression, temps);
					if (BoundExpressionExtensions.IsPropertyOrXmlPropertyAccess(boundExpression))
					{
						boundExpression = BoundExpressionExtensions.SetAccessKind(result.First, PropertyAccessKind.Get);
						item = BoundExpressionExtensions.SetAccessKind(result.Second, PropertyAccessKind.Set);
					}
					else if (BoundExpressionExtensions.IsLateBound(boundExpression))
					{
						boundExpression = BoundExpressionExtensions.SetLateBoundAccessKind(result.First, LateBoundAccessKind.Get);
						item = BoundExpressionExtensions.SetLateBoundAccessKind(result.Second, LateBoundAccessKind.Set);
					}
					else
					{
						boundExpression = result.First.MakeRValue();
						item = result.Second;
					}
					if (boundLateBoundArgumentSupportingAssignmentWithCapture != null)
					{
						boundExpression = new BoundAssignmentOperator(boundLateBoundArgumentSupportingAssignmentWithCapture.Syntax, new BoundLocal(boundLateBoundArgumentSupportingAssignmentWithCapture.Syntax, boundLateBoundArgumentSupportingAssignmentWithCapture.LocalSymbol, boundLateBoundArgumentSupportingAssignmentWithCapture.LocalSymbol.Type), boundExpression, suppressObjectClone: true, boundLateBoundArgumentSupportingAssignmentWithCapture.Type);
					}
				}
				instance.Add(VisitExpressionNode(boundExpression));
				instance2.Add(item);
			}
			arguments = instance.ToImmutableAndFree();
			writeTargets = instance2.ToImmutableAndFree();
		}

		private static BoundLiteral MakeStringLiteral(SyntaxNode node, string value, TypeSymbol stringType)
		{
			if (value == null)
			{
				return MakeNullLiteral(node, stringType);
			}
			return new BoundLiteral(node, ConstantValue.Create(value), stringType);
		}

		private static BoundLiteral MakeBooleanLiteral(SyntaxNode node, bool value, TypeSymbol booleanType)
		{
			return new BoundLiteral(node, ConstantValue.Create(value), booleanType);
		}

		private static BoundGetType MakeGetTypeExpression(SyntaxNode node, TypeSymbol type, TypeSymbol typeType)
		{
			BoundTypeExpression sourceType = new BoundTypeExpression(node, type);
			return new BoundGetType(node, sourceType, typeType);
		}

		private BoundArrayCreation MakeArrayOfGetTypeExpressions(SyntaxNode node, ImmutableArray<TypeSymbol> types, TypeSymbol typeArrayType)
		{
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Int32);
			BoundExpression item = new BoundLiteral(node, ConstantValue.Create(types.Length), specialType);
			TypeSymbol elementType = ((ArrayTypeSymbol)typeArrayType).ElementType;
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			ImmutableArray<TypeSymbol>.Enumerator enumerator = types.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol current = enumerator.Current;
				instance.Add(MakeGetTypeExpression(node, current, elementType));
			}
			BoundArrayInitialization initializerOpt = new BoundArrayInitialization(node, instance.ToImmutableAndFree(), null);
			return new BoundArrayCreation(node, ImmutableArray.Create(item), initializerOpt, typeArrayType);
		}

		private bool TryGetWellknownMember<T>(out T result, WellKnownMember memberId, SyntaxNode syntax, bool isOptional = false) where T : Symbol
		{
			result = null;
			UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			Symbol wellKnownTypeMember = Binder.GetWellKnownTypeMember(Compilation, memberId, out useSiteInfo);
			if (useSiteInfo.DiagnosticInfo != null)
			{
				if (!isOptional)
				{
					Binder.ReportUseSite(_diagnostics, syntax.GetLocation(), useSiteInfo);
				}
				return false;
			}
			_diagnostics.AddDependencies(useSiteInfo);
			result = (T)wellKnownTypeMember;
			return true;
		}

		private bool TryGetSpecialMember<T>(out T result, SpecialMember memberId, SyntaxNode syntax) where T : Symbol
		{
			result = null;
			UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			Symbol specialTypeMember = Binder.GetSpecialTypeMember(_topMethod.ContainingAssembly, memberId, out useSiteInfo);
			if (Binder.ReportUseSite(_diagnostics, syntax.GetLocation(), useSiteInfo))
			{
				return false;
			}
			result = (T)specialTypeMember;
			return true;
		}

		public override BoundNode VisitLateInvocation(BoundLateInvocation node)
		{
			if (_inExpressionLambda)
			{
				return base.VisitLateInvocation(node);
			}
			if (node.Member.Kind == BoundKind.LateMemberAccess)
			{
				BoundLateMemberAccess boundLateMemberAccess = (BoundLateMemberAccess)node.Member;
				return RewriteLateBoundMemberInvocation(boundLateMemberAccess, boundLateMemberAccess.ReceiverOpt, node.ArgumentsOpt, node.ArgumentNamesOpt, node.AccessKind == LateBoundAccessKind.Call);
			}
			return RewriteLateBoundIndexInvocation(node, node.Member, node.ArgumentsOpt);
		}

		private BoundExpression RewriteLateBoundIndexInvocation(BoundLateInvocation invocation, BoundExpression receiverExpression, ImmutableArray<BoundExpression> argExpressions)
		{
			BoundExpression receiverExpr = VisitExpressionNode(invocation.Member);
			ImmutableArray<BoundExpression> argExpressions2 = VisitList(argExpressions);
			return LateIndexGet(invocation, receiverExpr, argExpressions2);
		}

		private BoundExpression RewriteLateBoundMemberInvocation(BoundLateMemberAccess memberAccess, BoundExpression receiverExpression, ImmutableArray<BoundExpression> argExpressions, ImmutableArray<string> argNames, bool useLateCall)
		{
			ArrayBuilder<SynthesizedLocal> temps = null;
			BoundExpression receiverExpression2 = VisitExpressionNode(receiverExpression);
			ImmutableArray<BoundExpression> writeTargets = default(ImmutableArray<BoundExpression>);
			LateCaptureArgsComplex(ref temps, ref argExpressions, out writeTargets);
			BoundExpression boundExpression = LateCallOrGet(memberAccess, receiverExpression2, argExpressions, writeTargets, argNames, useLateCall);
			ImmutableArray<SynthesizedLocal> immutableArray = default(ImmutableArray<SynthesizedLocal>);
			if (temps != null)
			{
				immutableArray = temps.ToImmutableAndFree();
				if (!immutableArray.IsEmpty)
				{
					boundExpression = new BoundSequence(memberAccess.Syntax, StaticCast<LocalSymbol>.From(immutableArray), ImmutableArray<BoundExpression>.Empty, boundExpression, boundExpression.Type);
				}
			}
			return boundExpression;
		}

		public override BoundNode VisitLateMemberAccess(BoundLateMemberAccess memberAccess)
		{
			if (_inExpressionLambda)
			{
				return base.VisitLateMemberAccess(memberAccess);
			}
			BoundExpression receiverExpression = VisitExpressionNode(memberAccess.ReceiverOpt);
			return LateCallOrGet(memberAccess, receiverExpression, default(ImmutableArray<BoundExpression>), default(ImmutableArray<BoundExpression>), default(ImmutableArray<string>), memberAccess.AccessKind == LateBoundAccessKind.Call);
		}

		public override BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
		{
			LocalSymbol localSymbol = node.LocalSymbol;
			KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField> staticLocalBackingFields = default(KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField>);
			BoundExpression initializerOpt = node.InitializerOpt;
			bool flag = initializerOpt != null;
			BoundStatement result = null;
			if (localSymbol.IsStatic)
			{
				staticLocalBackingFields = CreateBackingFieldsForStaticLocal(localSymbol, flag);
			}
			if (flag)
			{
				BoundWithLValueExpressionPlaceholder boundWithLValueExpressionPlaceholder = null;
				if (initializerOpt.Kind == BoundKind.ObjectCreationExpression || initializerOpt.Kind == BoundKind.NewT)
				{
					BoundObjectCreationExpressionBase boundObjectCreationExpressionBase = (BoundObjectCreationExpressionBase)initializerOpt;
					if (boundObjectCreationExpressionBase.InitializerOpt != null && boundObjectCreationExpressionBase.InitializerOpt.Kind == BoundKind.ObjectInitializerExpression)
					{
						BoundObjectInitializerExpression boundObjectInitializerExpression = (BoundObjectInitializerExpression)boundObjectCreationExpressionBase.InitializerOpt;
						if (!boundObjectInitializerExpression.CreateTemporaryLocalForInitialization)
						{
							boundWithLValueExpressionPlaceholder = boundObjectInitializerExpression.PlaceholderOpt;
							AddPlaceholderReplacement(boundWithLValueExpressionPlaceholder, VisitExpressionNode(new BoundLocal(node.Syntax, localSymbol, localSymbol.Type)));
						}
					}
				}
				if (!localSymbol.IsConst)
				{
					BoundExpression rewrittenInitializer = VisitAndGenerateObjectCloneIfNeeded(initializerOpt);
					result = RewriteLocalDeclarationAsInitializer(node, rewrittenInitializer, staticLocalBackingFields, boundWithLValueExpressionPlaceholder == null);
				}
				if (boundWithLValueExpressionPlaceholder != null)
				{
					RemovePlaceholderReplacement(boundWithLValueExpressionPlaceholder);
				}
			}
			return result;
		}

		private BoundStatement RewriteLocalDeclarationAsInitializer(BoundLocalDeclaration node, BoundExpression rewrittenInitializer, KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField> staticLocalBackingFields, bool objectInitializerNeedsTemporary = true)
		{
			UnstructuredExceptionHandlingContext saved = LeaveUnstructuredExceptionHandlingContext(node);
			BoundStatement boundStatement = (objectInitializerNeedsTemporary ? new BoundExpressionStatement(rewrittenInitializer.Syntax, new BoundAssignmentOperator(rewrittenInitializer.Syntax, VisitExpressionNode(new BoundLocal(node.Syntax, node.LocalSymbol, node.LocalSymbol.Type)), rewrittenInitializer, suppressObjectClone: true, node.LocalSymbol.Type)) : new BoundExpressionStatement(rewrittenInitializer.Syntax, rewrittenInitializer));
			if (node.LocalSymbol.IsStatic)
			{
				boundStatement = EnforceStaticLocalInitializationSemantics(staticLocalBackingFields, boundStatement);
			}
			RestoreUnstructuredExceptionHandlingContext(node, saved);
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, canThrow: true);
			}
			if (this.get_Instrument((BoundNode)node))
			{
				boundStatement = _instrumenterOpt.InstrumentLocalInitialization(node, boundStatement);
			}
			return boundStatement;
		}

		private KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField> CreateBackingFieldsForStaticLocal(LocalSymbol localSymbol, bool hasInitializer)
		{
			if (_staticLocalMap == null)
			{
				_staticLocalMap = new Dictionary<LocalSymbol, KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField>>(ReferenceEqualityComparer.Instance);
			}
			KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField> keyValuePair = new KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField>(new SynthesizedStaticLocalBackingField(localSymbol, isValueField: true, !hasInitializer), hasInitializer ? new SynthesizedStaticLocalBackingField(localSymbol, isValueField: false, reportErrorForLongNames: true) : null);
			if (_emitModule != null)
			{
				_emitModule.AddSynthesizedDefinition(_topMethod.ContainingType, keyValuePair.Key.GetCciAdapter());
				if ((object)keyValuePair.Value != null)
				{
					_emitModule.AddSynthesizedDefinition(_topMethod.ContainingType, keyValuePair.Value.GetCciAdapter());
				}
			}
			_staticLocalMap.Add(localSymbol, keyValuePair);
			return keyValuePair;
		}

		public override BoundNode VisitLocal(BoundLocal node)
		{
			if (node.LocalSymbol.IsStatic)
			{
				SynthesizedStaticLocalBackingField key = _staticLocalMap[node.LocalSymbol].Key;
				return new BoundFieldAccess(node.Syntax, _topMethod.IsShared ? null : new BoundMeReference(node.Syntax, _topMethod.ContainingType), key, node.IsLValue, key.Type);
			}
			return base.VisitLocal(node);
		}

		private BoundStatement EnforceStaticLocalInitializationSemantics(KeyValuePair<SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField> staticLocalBackingFields, BoundStatement rewrittenInitialization)
		{
			SyntaxNode syntax = rewrittenInitialization.Syntax;
			NamedTypeSymbol specialTypeWithUseSiteDiagnostics = GetSpecialTypeWithUseSiteDiagnostics(SpecialType.System_Object, syntax);
			NamedTypeSymbol specialTypeWithUseSiteDiagnostics2 = GetSpecialTypeWithUseSiteDiagnostics(SpecialType.System_Boolean, syntax);
			MethodSymbol result = null;
			MethodSymbol result2 = null;
			FieldSymbol result3 = null;
			MethodSymbol result4 = null;
			if ((TypeSymbolExtensions.IsErrorType(specialTypeWithUseSiteDiagnostics) || TypeSymbolExtensions.IsErrorType(specialTypeWithUseSiteDiagnostics2)) | !TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_StaticLocalInitFlag__ctor, syntax) | !TryGetWellknownMember<MethodSymbol>(out result2, WellKnownMember.System_Threading_Interlocked__CompareExchange_T, syntax) | !TryGetWellknownMember<FieldSymbol>(out result3, WellKnownMember.Microsoft_VisualBasic_CompilerServices_StaticLocalInitFlag__State, syntax) | !TryGetWellknownMember<MethodSymbol>(out result4, WellKnownMember.Microsoft_VisualBasic_CompilerServices_IncompleteInitialization__ctor, syntax))
			{
				return rewrittenInitialization;
			}
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			BoundFieldAccess boundFieldAccess = new BoundFieldAccess(syntax, _topMethod.IsShared ? null : new BoundMeReference(syntax, _topMethod.ContainingType), staticLocalBackingFields.Value, isLValue: true, staticLocalBackingFields.Value.Type);
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
			BoundDirectCast boundDirectCast = new BoundDirectCast(syntax, boundFieldAccess.MakeRValue(), Conversions.ClassifyDirectCastConversion(boundFieldAccess.Type, specialTypeWithUseSiteDiagnostics, ref useSiteInfo), specialTypeWithUseSiteDiagnostics);
			((BindingDiagnosticBag<AssemblySymbol>)_diagnostics).Add(syntax, useSiteInfo);
			BoundBinaryOperator rewrittenCondition = new BoundBinaryOperator(syntax, BinaryOperatorKind.Is, boundDirectCast, new BoundLiteral(syntax, ConstantValue.Nothing, specialTypeWithUseSiteDiagnostics), @checked: false, specialTypeWithUseSiteDiagnostics2);
			BoundObjectCreationExpression item = new BoundObjectCreationExpression(syntax, result, ImmutableArray<BoundExpression>.Empty, null, boundFieldAccess.Type);
			BoundCall node = new BoundCall(syntax, result2.Construct(boundFieldAccess.Type), null, null, ImmutableArray.Create<BoundExpression>(boundFieldAccess, item, new BoundLiteral(syntax, ConstantValue.Nothing, boundFieldAccess.Type)), null, boundFieldAccess.Type);
			BoundStatement item2 = RewriteIfStatement(syntax, rewrittenCondition, BoundExpressionExtensions.ToStatement(node), null, null);
			instance.Add(item2);
			BoundLocal boundLockTakenLocal = null;
			BoundStatement boundLockTakenInitialization = null;
			BoundStatement boundStatement = GenerateMonitorEnter(syntax, boundDirectCast, out boundLockTakenLocal, out boundLockTakenInitialization);
			BoundFieldAccess boundFieldAccess2 = new BoundFieldAccess(syntax, boundFieldAccess, result3, isLValue: true, result3.Type);
			BoundLiteral right = new BoundLiteral(syntax, ConstantValue.Create((short)2), boundFieldAccess2.Type);
			BoundBinaryOperator rewrittenCondition2 = new BoundBinaryOperator(syntax, BinaryOperatorKind.Equals, boundFieldAccess2.MakeRValue(), new BoundLiteral(syntax, ConstantValue.Default(ConstantValueTypeDiscriminator.Int16), boundFieldAccess2.Type), @checked: false, specialTypeWithUseSiteDiagnostics2);
			BoundExpressionStatement item3 = BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(syntax, boundFieldAccess2, right, suppressObjectClone: true));
			BoundBinaryOperator rewrittenCondition3 = new BoundBinaryOperator(syntax, BinaryOperatorKind.Equals, boundFieldAccess2.MakeRValue(), right, @checked: false, specialTypeWithUseSiteDiagnostics2);
			BoundThrowStatement rewrittenConsequence = new BoundThrowStatement(syntax, new BoundObjectCreationExpression(syntax, result4, ImmutableArray<BoundExpression>.Empty, null, result4.ContainingType));
			BoundStatement boundStatement2 = RewriteIfStatement(syntax, rewrittenCondition2, new BoundStatementList(syntax, ImmutableArray.Create(item3, rewrittenInitialization)), RewriteIfStatement(syntax, rewrittenCondition3, rewrittenConsequence, null, null), null);
			ImmutableArray<LocalSymbol> locals;
			ImmutableArray<BoundStatement> statements;
			if (boundLockTakenLocal != null)
			{
				locals = ImmutableArray.Create(boundLockTakenLocal.LocalSymbol);
				instance.Add(boundLockTakenInitialization);
				statements = ImmutableArray.Create(boundStatement, boundStatement2);
			}
			else
			{
				locals = ImmutableArray<LocalSymbol>.Empty;
				instance.Add(boundStatement);
				statements = ImmutableArray.Create(boundStatement2);
			}
			BoundBlock tryBlock = new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, statements);
			BoundExpressionStatement item4 = BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(syntax, boundFieldAccess2, new BoundLiteral(syntax, ConstantValue.Create((short)1), boundFieldAccess2.Type), suppressObjectClone: true));
			BoundStatement item5 = GenerateMonitorExit(syntax, boundDirectCast, boundLockTakenLocal);
			BoundBlock finallyBlockOpt = new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(item4, item5));
			BoundTryStatement item6 = new BoundTryStatement(syntax, tryBlock, ImmutableArray<BoundCatchBlock>.Empty, finallyBlockOpt, null);
			instance.Add(item6);
			return new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), locals, instance.ToImmutableAndFree());
		}

		private BoundExpression WrapInNullable(BoundExpression expr, TypeSymbol nullableType)
		{
			MethodSymbol nullableMethod = GetNullableMethod(expr.Syntax, nullableType, SpecialMember.System_Nullable_T__ctor);
			if ((object)nullableMethod != null)
			{
				return new BoundObjectCreationExpression(expr.Syntax, nullableMethod, ImmutableArray.Create(expr), null, nullableType);
			}
			return new BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(expr), nullableType, hasErrors: true);
		}

		private BoundExpression ProcessNullableOperand(BoundExpression operand, out BoundExpression hasValueExpr, ref ArrayBuilder<LocalSymbol> temps, ref ArrayBuilder<BoundExpression> inits, bool doNotCaptureLocals)
		{
			return ProcessNullableOperand(operand, out hasValueExpr, ref temps, ref inits, doNotCaptureLocals, HasValue(operand));
		}

		private BoundExpression ProcessNullableOperand(BoundExpression operand, out BoundExpression hasValueExpr, ref ArrayBuilder<LocalSymbol> temps, ref ArrayBuilder<BoundExpression> inits, bool doNotCaptureLocals, bool operandHasValue)
		{
			if (operandHasValue)
			{
				operand = NullableValueOrDefault(operand);
			}
			BoundExpression boundExpression = CaptureNullableIfNeeded(operand, ref temps, ref inits, doNotCaptureLocals);
			if (operandHasValue)
			{
				hasValueExpr = new BoundLiteral(operand.Syntax, ConstantValue.True, GetSpecialType(SpecialType.System_Boolean));
				return boundExpression;
			}
			hasValueExpr = NullableHasValue(boundExpression);
			return NullableValueOrDefault(boundExpression);
		}

		private static bool RightCantChangeLeftLocal(BoundExpression left, BoundExpression right)
		{
			if (right.Kind != BoundKind.Local)
			{
				return right.Kind == BoundKind.Parameter;
			}
			return true;
		}

		private BoundExpression CaptureNullableIfNeeded(BoundExpression operand, out SynthesizedLocal temp, out BoundExpression init, bool doNotCaptureLocals)
		{
			temp = null;
			init = null;
			if (operand.IsConstant)
			{
				return operand;
			}
			if (doNotCaptureLocals)
			{
				if (operand.Kind == BoundKind.Local && !((BoundLocal)operand).LocalSymbol.IsByRef)
				{
					return operand;
				}
				if (operand.Kind == BoundKind.Parameter && !((BoundParameter)operand).ParameterSymbol.IsByRef)
				{
					return operand;
				}
			}
			return CaptureOperand(operand, out temp, out init);
		}

		private BoundExpression CaptureOperand(BoundExpression operand, out SynthesizedLocal temp, out BoundExpression init)
		{
			temp = new SynthesizedLocal(_currentMethodOrLambda, operand.Type, SynthesizedLocalKind.LoweringTemp);
			BoundLocal boundLocal = new BoundLocal(operand.Syntax, temp, isLValue: true, temp.Type);
			init = new BoundAssignmentOperator(operand.Syntax, boundLocal, operand, suppressObjectClone: true, operand.Type);
			return boundLocal.MakeRValue();
		}

		private BoundExpression CaptureNullableIfNeeded(BoundExpression operand, [In][Out] ref ArrayBuilder<LocalSymbol> temps, [In][Out] ref ArrayBuilder<BoundExpression> inits, bool doNotCaptureLocals)
		{
			SynthesizedLocal temp = null;
			BoundExpression init = null;
			BoundExpression result = CaptureNullableIfNeeded(operand, out temp, out init, doNotCaptureLocals);
			if ((object)temp != null)
			{
				temps = temps ?? ArrayBuilder<LocalSymbol>.GetInstance();
				temps.Add(temp);
				inits = inits ?? ArrayBuilder<BoundExpression>.GetInstance();
				inits.Add(init);
			}
			return result;
		}

		private BoundExpression NullableValueOrDefault(BoundExpression expr)
		{
			switch (expr.Kind)
			{
			case BoundKind.ObjectCreationExpression:
			{
				BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)expr;
				if (boundObjectCreationExpression.Arguments.Length == 1)
				{
					return boundObjectCreationExpression.Arguments[0];
				}
				break;
			}
			case BoundKind.Conversion:
			{
				BoundConversion boundConversion = (BoundConversion)expr;
				if (IsConversionFromUnderlyingToNullable(boundConversion))
				{
					return boundConversion.Operand;
				}
				break;
			}
			default:
				if (!_inExpressionLambda && TypeSymbolExtensions.IsNullableOfBoolean(expr.Type))
				{
					BoundExpression whenNotNull = null;
					BoundExpression whenNull = null;
					if (IsConditionalAccess(expr, out whenNotNull, out whenNull) && HasNoValue(whenNull))
					{
						return UpdateConditionalAccess(expr, NullableValueOrDefault(whenNotNull), new BoundLiteral(expr.Syntax, ConstantValue.False, TypeSymbolExtensions.GetNullableUnderlyingType(expr.Type)));
					}
				}
				break;
			}
			MethodSymbol nullableMethod = GetNullableMethod(expr.Syntax, expr.Type, SpecialMember.System_Nullable_T_GetValueOrDefault);
			if ((object)nullableMethod != null)
			{
				return new BoundCall(expr.Syntax, nullableMethod, null, expr, ImmutableArray<BoundExpression>.Empty, null, isLValue: false, suppressObjectClone: true, nullableMethod.ReturnType);
			}
			return new BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(expr), TypeSymbolExtensions.GetNullableUnderlyingType(expr.Type), hasErrors: true);
		}

		private static bool IsConversionFromUnderlyingToNullable(BoundConversion conversion)
		{
			if ((conversion.ConversionKind & (ConversionKind.WideningNullable | ConversionKind.UserDefined)) == ConversionKind.WideningNullable)
			{
				return TypeSymbolExtensions.GetNullableUnderlyingType(conversion.Type).Equals(conversion.Operand.Type, TypeCompareKind.AllIgnoreOptionsForVB);
			}
			return false;
		}

		private BoundExpression NullableValue(BoundExpression expr)
		{
			if (HasValue(expr))
			{
				return NullableValueOrDefault(expr);
			}
			MethodSymbol nullableMethod = GetNullableMethod(expr.Syntax, expr.Type, SpecialMember.System_Nullable_T_get_Value);
			if ((object)nullableMethod != null)
			{
				return new BoundCall(expr.Syntax, nullableMethod, null, expr, ImmutableArray<BoundExpression>.Empty, null, isLValue: false, suppressObjectClone: true, nullableMethod.ReturnType);
			}
			return new BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(expr), TypeSymbolExtensions.GetNullableUnderlyingType(expr.Type), hasErrors: true);
		}

		private BoundExpression NullableHasValue(BoundExpression expr)
		{
			MethodSymbol nullableMethod = GetNullableMethod(expr.Syntax, expr.Type, SpecialMember.System_Nullable_T_get_HasValue);
			if ((object)nullableMethod != null)
			{
				return new BoundCall(expr.Syntax, nullableMethod, null, expr, ImmutableArray<BoundExpression>.Empty, null, isLValue: false, suppressObjectClone: true, nullableMethod.ReturnType);
			}
			return new BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(expr), Compilation.GetSpecialType(SpecialType.System_Boolean), hasErrors: true);
		}

		private static BoundExpression NullableNull(SyntaxNode syntax, TypeSymbol nullableType)
		{
			return new BoundObjectCreationExpression(syntax, null, ImmutableArray<BoundExpression>.Empty, null, nullableType);
		}

		private static BoundExpression NullableNull(BoundExpression candidateNullExpression, TypeSymbol type)
		{
			if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(type, candidateNullExpression.Type) || candidateNullExpression.Kind != BoundKind.ObjectCreationExpression)
			{
				return NullableNull(candidateNullExpression.Syntax, type);
			}
			return candidateNullExpression;
		}

		private BoundExpression NullableFalse(SyntaxNode syntax, TypeSymbol nullableOfBoolean)
		{
			TypeSymbol nullableUnderlyingType = TypeSymbolExtensions.GetNullableUnderlyingType(nullableOfBoolean);
			return WrapInNullable(new BoundLiteral(syntax, ConstantValue.False, nullableUnderlyingType), nullableOfBoolean);
		}

		private BoundExpression NullableTrue(SyntaxNode syntax, TypeSymbol nullableOfBoolean)
		{
			TypeSymbol nullableUnderlyingType = TypeSymbolExtensions.GetNullableUnderlyingType(nullableOfBoolean);
			return WrapInNullable(new BoundLiteral(syntax, ConstantValue.True, nullableUnderlyingType), nullableOfBoolean);
		}

		private MethodSymbol GetNullableMethod(SyntaxNode syntax, TypeSymbol nullableType, SpecialMember member)
		{
			MethodSymbol result = null;
			if (TryGetSpecialMember<MethodSymbol>(out result, member, syntax))
			{
				return (MethodSymbol)((SubstitutedNamedType)nullableType).GetMemberForDefinition(result);
			}
			return null;
		}

		private BoundExpression NullableOfBooleanValue(SyntaxNode syntax, bool isTrue, TypeSymbol nullableOfBoolean)
		{
			if (isTrue)
			{
				return NullableTrue(syntax, nullableOfBoolean);
			}
			return NullableFalse(syntax, nullableOfBoolean);
		}

		private static bool HasNoValue(BoundExpression expr)
		{
			if (expr.Kind == BoundKind.ObjectCreationExpression)
			{
				return ((BoundObjectCreationExpression)expr).Arguments.Length == 0;
			}
			return false;
		}

		private static bool HasValue(BoundExpression expr)
		{
			switch (expr.Kind)
			{
			case BoundKind.ObjectCreationExpression:
				return ((BoundObjectCreationExpression)expr).Arguments.Length != 0;
			case BoundKind.Conversion:
				if (IsConversionFromUnderlyingToNullable((BoundConversion)expr))
				{
					return true;
				}
				break;
			}
			return false;
		}

		private BoundExpression MakeBinaryExpression(SyntaxNode syntax, BinaryOperatorKind binaryOpKind, BoundExpression left, BoundExpression right, bool isChecked, TypeSymbol resultType)
		{
			bool integerOverflow = false;
			bool divideByZero = false;
			bool lengthOutOfLimit = false;
			ConstantValue constantValue = OverloadResolution.TryFoldConstantBinaryOperator(binaryOpKind, left, right, resultType, ref integerOverflow, ref divideByZero, ref lengthOutOfLimit);
			if ((object)constantValue != null && !divideByZero && !(integerOverflow && isChecked) && !lengthOutOfLimit)
			{
				return new BoundLiteral(syntax, constantValue, resultType);
			}
			switch (binaryOpKind)
			{
			case BinaryOperatorKind.Subtract:
				if (BoundExpressionExtensions.IsDefaultValueConstant(right))
				{
					return left;
				}
				break;
			case BinaryOperatorKind.Add:
			case BinaryOperatorKind.Or:
			case BinaryOperatorKind.OrElse:
				if (BoundExpressionExtensions.IsDefaultValueConstant(left))
				{
					return right;
				}
				if (BoundExpressionExtensions.IsDefaultValueConstant(right))
				{
					return left;
				}
				if (BoundExpressionExtensions.IsTrueConstant(left))
				{
					return MakeSequence(right, left);
				}
				if (BoundExpressionExtensions.IsTrueConstant(right))
				{
					return MakeSequence(left, right);
				}
				break;
			case BinaryOperatorKind.Multiply:
			case BinaryOperatorKind.And:
			case BinaryOperatorKind.AndAlso:
				if (BoundExpressionExtensions.IsDefaultValueConstant(left))
				{
					return MakeSequence(right, left);
				}
				if (BoundExpressionExtensions.IsDefaultValueConstant(right))
				{
					return MakeSequence(left, right);
				}
				if (BoundExpressionExtensions.IsTrueConstant(left))
				{
					return right;
				}
				if (BoundExpressionExtensions.IsTrueConstant(right))
				{
					return left;
				}
				break;
			case BinaryOperatorKind.Equals:
				if (BoundExpressionExtensions.IsTrueConstant(left))
				{
					return right;
				}
				if (BoundExpressionExtensions.IsTrueConstant(right))
				{
					return left;
				}
				break;
			case BinaryOperatorKind.NotEquals:
				if (BoundExpressionExtensions.IsFalseConstant(left))
				{
					return right;
				}
				if (BoundExpressionExtensions.IsFalseConstant(right))
				{
					return left;
				}
				break;
			}
			return TransformRewrittenBinaryOperator(new BoundBinaryOperator(syntax, binaryOpKind, left, right, isChecked, resultType));
		}

		private BoundExpression MakeBooleanBinaryExpression(SyntaxNode syntax, BinaryOperatorKind binaryOpKind, BoundExpression left, BoundExpression right)
		{
			return MakeBinaryExpression(syntax, binaryOpKind, left, right, isChecked: false, left.Type);
		}

		private static BoundLiteral MakeNullLiteral(SyntaxNode syntax, TypeSymbol type)
		{
			return new BoundLiteral(syntax, ConstantValue.Nothing, type);
		}

		private static BoundExpression MakeSequence(BoundExpression first, BoundExpression second)
		{
			return MakeSequence(second.Syntax, first, second);
		}

		private static BoundExpression MakeSequence(SyntaxNode syntax, BoundExpression first, BoundExpression second)
		{
			BoundExpression sideeffects = GetSideeffects(first);
			if (sideeffects == null)
			{
				return second;
			}
			return new BoundSequence(syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(sideeffects), second, second.Type);
		}

		private BoundExpression MakeTernaryConditionalExpression(SyntaxNode syntax, BoundExpression condition, BoundExpression whenTrue, BoundExpression whenFalse)
		{
			ConstantValue constantValueOpt = condition.ConstantValueOpt;
			if ((object)constantValueOpt != null)
			{
				return MakeSequence(syntax, condition, ((object)constantValueOpt == ConstantValue.True) ? whenTrue : whenFalse);
			}
			return TransformRewrittenTernaryConditionalExpression(new BoundTernaryConditionalExpression(syntax, condition, whenTrue, whenFalse, null, whenTrue.Type));
		}

		private static BoundExpression GetSideeffects(BoundExpression operand)
		{
			if (operand.IsConstant)
			{
				return null;
			}
			switch (operand.Kind)
			{
			case BoundKind.Local:
			case BoundKind.Parameter:
				return null;
			case BoundKind.ObjectCreationExpression:
				if (TypeSymbolExtensions.IsNullableType(operand.Type))
				{
					ImmutableArray<BoundExpression> arguments = ((BoundObjectCreationExpression)operand).Arguments;
					if (arguments.Length == 0)
					{
						return null;
					}
					return GetSideeffects(arguments[0]);
				}
				break;
			}
			return operand;
		}

		public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
		{
			BoundObjectInitializerExpressionBase initializerOpt = node.InitializerOpt;
			node = node.Update(node.ConstructorOpt, node.Arguments, node.DefaultArguments, null, node.Type);
			MethodSymbol constructorOpt = node.ConstructorOpt;
			BoundExpression boundExpression = node;
			if ((object)constructorOpt != null)
			{
				ImmutableArray<SynthesizedLocal> temporaries = default(ImmutableArray<SynthesizedLocal>);
				ImmutableArray<BoundExpression> copyBack = default(ImmutableArray<BoundExpression>);
				boundExpression = node.Update(constructorOpt, RewriteCallArguments(node.Arguments, constructorOpt.Parameters, out temporaries, out copyBack, suppressObjectClone: false), node.DefaultArguments, null, constructorOpt.ContainingType);
				if (!temporaries.IsDefault)
				{
					boundExpression = GenerateSequenceValueSideEffects(_currentMethodOrLambda, boundExpression, StaticCast<LocalSymbol>.From(temporaries), copyBack);
				}
				if (TypeSymbolExtensions.IsInterfaceType(node.Type))
				{
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
					ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(boundExpression.Type, node.Type, ref useSiteInfo);
					_diagnostics.Add(boundExpression, useSiteInfo);
					boundExpression = new BoundDirectCast(node.Syntax, boundExpression, conversionKind, node.Type);
				}
			}
			if (initializerOpt != null)
			{
				return VisitObjectCreationInitializer(initializerOpt, node, boundExpression);
			}
			return boundExpression;
		}

		public override BoundNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
		{
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			MethodSymbol methodSymbol = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.System_Guid__ctor);
			BoundExpression boundExpression = (((object)methodSymbol == null) ? ((BoundExpression)new BoundBadExpression(node.Syntax, LookupResultKind.NotCreatable, ImmutableArray<Symbol>.Empty, ImmutableArray<BoundExpression>.Empty, ErrorTypeSymbol.UnknownResultType, hasErrors: true)) : ((BoundExpression)syntheticBoundNodeFactory.New(methodSymbol, syntheticBoundNodeFactory.Literal(node.GuidString))));
			MethodSymbol methodSymbol2 = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.System_Runtime_InteropServices_Marshal__GetTypeFromCLSID, isOptional: true) ?? syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.System_Type__GetTypeFromCLSID);
			BoundExpression boundExpression2 = (((object)methodSymbol2 == null) ? ((BoundExpression)new BoundBadExpression(node.Syntax, LookupResultKind.OverloadResolutionFailure, ImmutableArray<Symbol>.Empty, ImmutableArray<BoundExpression>.Empty, ErrorTypeSymbol.UnknownResultType, hasErrors: true)) : ((BoundExpression)syntheticBoundNodeFactory.Call(null, methodSymbol2, boundExpression)));
			MethodSymbol methodSymbol3 = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.System_Activator__CreateInstance);
			BoundExpression boundExpression3;
			if ((object)methodSymbol3 != null && !TypeSymbolExtensions.IsErrorType(methodSymbol3.ReturnType))
			{
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
				ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(methodSymbol3.ReturnType, node.Type, ref useSiteInfo);
				_diagnostics.Add(node, useSiteInfo);
				boundExpression3 = new BoundDirectCast(node.Syntax, syntheticBoundNodeFactory.Call(null, methodSymbol3, boundExpression2), conversionKind, node.Type);
			}
			else
			{
				boundExpression3 = new BoundBadExpression(node.Syntax, LookupResultKind.OverloadResolutionFailure, ImmutableArray<Symbol>.Empty, ImmutableArray<BoundExpression>.Empty, node.Type, hasErrors: true);
			}
			if (node.InitializerOpt == null || node.InitializerOpt.HasErrors)
			{
				return boundExpression3;
			}
			return VisitObjectCreationInitializer(node.InitializerOpt, node, boundExpression3);
		}

		private BoundNode VisitObjectCreationInitializer(BoundObjectInitializerExpressionBase objectInitializer, BoundExpression objectCreationExpression, BoundExpression rewrittenObjectCreationExpression)
		{
			if (objectInitializer.Kind == BoundKind.CollectionInitializerExpression)
			{
				return RewriteCollectionInitializerExpression((BoundCollectionInitializerExpression)objectInitializer, objectCreationExpression, rewrittenObjectCreationExpression);
			}
			return RewriteObjectInitializerExpression((BoundObjectInitializerExpression)objectInitializer, objectCreationExpression, rewrittenObjectCreationExpression);
		}

		public override BoundNode VisitNewT(BoundNewT node)
		{
			if (_inExpressionLambda)
			{
				if (node.InitializerOpt != null)
				{
					return VisitObjectCreationInitializer(node.InitializerOpt, node, node);
				}
				return node;
			}
			SyntaxNode syntax = node.Syntax;
			TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)node.Type;
			MethodSymbol result = null;
			BoundExpression boundExpression;
			if (TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.System_Activator__CreateInstance_T, syntax))
			{
				result = result.Construct(ImmutableArray.Create((TypeSymbol)typeParameterSymbol));
				boundExpression = new BoundCall(syntax, result, null, null, ImmutableArray<BoundExpression>.Empty, null, isLValue: false, suppressObjectClone: false, typeParameterSymbol);
			}
			else
			{
				boundExpression = new BoundBadExpression(syntax, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, ImmutableArray<BoundExpression>.Empty, typeParameterSymbol, hasErrors: true);
			}
			if (node.InitializerOpt != null)
			{
				return VisitObjectCreationInitializer(node.InitializerOpt, boundExpression, boundExpression);
			}
			return boundExpression;
		}

		public BoundNode RewriteCollectionInitializerExpression(BoundCollectionInitializerExpression node, BoundExpression objectCreationExpression, BoundExpression rewrittenObjectCreationExpression)
		{
			TypeSymbol type = node.Type;
			SyntaxNode syntax = node.Syntax;
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			LocalSymbol localSymbol;
			BoundLocal boundLocal;
			BoundWithLValueExpressionPlaceholder boundWithLValueExpressionPlaceholder;
			if (_inExpressionLambda)
			{
				localSymbol = null;
				boundLocal = null;
				boundWithLValueExpressionPlaceholder = new BoundWithLValueExpressionPlaceholder(node.PlaceholderOpt.Syntax, node.PlaceholderOpt.Type);
				AddPlaceholderReplacement(node.PlaceholderOpt, boundWithLValueExpressionPlaceholder);
			}
			else
			{
				localSymbol = new SynthesizedLocal(_currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp);
				boundLocal = new BoundLocal(syntax, localSymbol, type);
				BoundAssignmentOperator item = new BoundAssignmentOperator(syntax, boundLocal, GenerateObjectCloneIfNeeded(objectCreationExpression, rewrittenObjectCreationExpression), suppressObjectClone: true, type);
				instance.Add(item);
				boundWithLValueExpressionPlaceholder = null;
				AddPlaceholderReplacement(node.PlaceholderOpt, boundLocal);
			}
			int num = node.Initializers.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				BoundExpression expression = node.Initializers[i];
				if (!IsOmittedBoundCall(expression))
				{
					instance.Add(VisitExpressionNode(expression));
				}
			}
			RemovePlaceholderReplacement(node.PlaceholderOpt);
			if (_inExpressionLambda)
			{
				return ReplaceObjectOrCollectionInitializer(rewrittenObjectCreationExpression, node.Update(boundWithLValueExpressionPlaceholder, instance.ToImmutableAndFree(), node.Type));
			}
			return new BoundSequence(syntax, ImmutableArray.Create(localSymbol), instance.ToImmutableAndFree(), boundLocal.MakeRValue(), type);
		}

		public BoundNode RewriteObjectInitializerExpression(BoundObjectInitializerExpression node, BoundExpression objectCreationExpression, BoundExpression rewrittenObjectCreationExpression)
		{
			TypeSymbol type = node.Type;
			int length = node.Initializers.Length;
			SyntaxNode syntax = node.Syntax;
			TypeSymbol type2;
			ImmutableArray<LocalSymbol> locals;
			BoundExpression boundExpression;
			BoundExpression valueOpt;
			if (node.CreateTemporaryLocalForInitialization)
			{
				LocalSymbol localSymbol = new SynthesizedLocal(_currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp);
				type2 = type;
				locals = ImmutableArray.Create(localSymbol);
				boundExpression = (_inExpressionLambda ? ((BoundExpression)node.PlaceholderOpt) : ((BoundExpression)new BoundLocal(syntax, localSymbol, type)));
				valueOpt = boundExpression.MakeRValue();
				AddPlaceholderReplacement(node.PlaceholderOpt, boundExpression);
			}
			else
			{
				boundExpression = this.get_PlaceholderReplacement((BoundValuePlaceholderBase)node.PlaceholderOpt);
				type2 = GetSpecialType(SpecialType.System_Void);
				locals = ImmutableArray<LocalSymbol>.Empty;
				valueOpt = null;
			}
			BoundExpression[] array = new BoundExpression[length + 1];
			array[0] = new BoundAssignmentOperator(syntax, boundExpression, GenerateObjectCloneIfNeeded(objectCreationExpression, rewrittenObjectCreationExpression), suppressObjectClone: true, type);
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (_inExpressionLambda)
				{
					BoundAssignmentOperator boundAssignmentOperator = (BoundAssignmentOperator)node.Initializers[i];
					array[i + 1] = boundAssignmentOperator.Update(boundAssignmentOperator.Left, boundAssignmentOperator.LeftOnTheRightOpt, VisitExpressionNode(boundAssignmentOperator.Right), suppressObjectClone: true, boundAssignmentOperator.Type);
				}
				else
				{
					array[i + 1] = VisitExpressionNode(node.Initializers[i]);
				}
			}
			if (node.CreateTemporaryLocalForInitialization)
			{
				RemovePlaceholderReplacement(node.PlaceholderOpt);
			}
			if (_inExpressionLambda)
			{
				BoundExpression[] array2 = new BoundExpression[length - 1 + 1];
				int num2 = length - 1;
				for (int j = 0; j <= num2; j++)
				{
					array2[j] = array[j + 1];
				}
				return ReplaceObjectOrCollectionInitializer(rewrittenObjectCreationExpression, node.Update(node.CreateTemporaryLocalForInitialization, node.Binder, node.PlaceholderOpt, array2.AsImmutableOrNull(), node.Type));
			}
			return new BoundSequence(syntax, locals, array.AsImmutableOrNull(), valueOpt, type2);
		}

		private BoundExpression ReplaceObjectOrCollectionInitializer(BoundExpression rewrittenObjectCreationExpression, BoundObjectInitializerExpressionBase rewrittenInitializer)
		{
			switch (rewrittenObjectCreationExpression.Kind)
			{
			case BoundKind.ObjectCreationExpression:
			{
				BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)rewrittenObjectCreationExpression;
				return boundObjectCreationExpression.Update(boundObjectCreationExpression.ConstructorOpt, boundObjectCreationExpression.Arguments, boundObjectCreationExpression.DefaultArguments, rewrittenInitializer, boundObjectCreationExpression.Type);
			}
			case BoundKind.NewT:
			{
				BoundNewT boundNewT = (BoundNewT)rewrittenObjectCreationExpression;
				return boundNewT.Update(rewrittenInitializer, boundNewT.Type);
			}
			case BoundKind.Sequence:
			{
				BoundSequence boundSequence = (BoundSequence)rewrittenObjectCreationExpression;
				return boundSequence.Update(boundSequence.Locals, boundSequence.SideEffects, ReplaceObjectOrCollectionInitializer(boundSequence.ValueOpt, rewrittenInitializer), boundSequence.Type);
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(rewrittenObjectCreationExpression.Kind);
			}
		}

		public override BoundNode VisitOmittedArgument(BoundOmittedArgument node)
		{
			FieldSymbol result = null;
			if (!TryGetWellknownMember<FieldSymbol>(out result, WellKnownMember.System_Reflection_Missing__Value, node.Syntax))
			{
				return node;
			}
			BoundFieldAccess boundFieldAccess = new BoundFieldAccess(node.Syntax, null, result, isLValue: false, result.Type);
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
			ConversionKind conversionKind = Conversions.ClassifyDirectCastConversion(boundFieldAccess.Type, node.Type, ref useSiteInfo);
			_diagnostics.Add(node, useSiteInfo);
			return new BoundDirectCast(node.Syntax, boundFieldAccess, conversionKind, node.Type);
		}

		public override BoundNode VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
		{
			ImplicitNamedTypeSymbol previousSubmissionType = (ImplicitNamedTypeSymbol)node.Type;
			SyntaxNode syntax = node.Syntax;
			FieldSymbol orMakeField = _previousSubmissionFields.GetOrMakeField(previousSubmissionType);
			BoundMeReference receiverOpt = new BoundMeReference(syntax, _topMethod.ContainingType);
			return new BoundFieldAccess(syntax, receiverOpt, orMakeField, isLValue: false, orMakeField.Type);
		}

		public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
		{
			BoundExpression receiverOpt = node.ReceiverOpt;
			if (receiverOpt != null && TypeSymbolExtensions.IsArrayType(receiverOpt.Type) && ((ArrayTypeSymbol)receiverOpt.Type).IsSZArray && ((object)node.PropertySymbol == GetSpecialTypeMember(SpecialMember.System_Array__Length) || (object)node.PropertySymbol == GetSpecialTypeMember(SpecialMember.System_Array__LongLength)))
			{
				return new BoundArrayLength(node.Syntax, VisitExpressionNode(receiverOpt), node.Type);
			}
			PropertySymbol propertySymbol = node.PropertySymbol;
			bool flag = receiverOpt != null && (BoundExpressionExtensions.IsMyClassReference(receiverOpt) || BoundExpressionExtensions.IsMyBaseReference(receiverOpt));
			if (_inExpressionLambda && propertySymbol.ParameterCount == 0 && (object)propertySymbol.ReducedFrom == null && !flag)
			{
				return base.VisitPropertyAccess(node);
			}
			MethodSymbol mostDerivedGetMethod = propertySymbol.GetMostDerivedGetMethod();
			return RewriteReceiverArgumentsAndGenerateAccessorCall(node.Syntax, mostDerivedGetMethod, receiverOpt, node.Arguments, node.ConstantValueOpt, node.IsLValue, suppressObjectClone: false, mostDerivedGetMethod.ReturnType);
		}

		public override BoundNode VisitQueryExpression(BoundQueryExpression node)
		{
			return Visit(node.LastOperator);
		}

		public override BoundNode VisitQueryClause(BoundQueryClause node)
		{
			return Visit(node.UnderlyingExpression);
		}

		public override BoundNode VisitOrdering(BoundOrdering node)
		{
			return Visit(node.UnderlyingExpression);
		}

		public override BoundNode VisitRangeVariableAssignment(BoundRangeVariableAssignment node)
		{
			return Visit(node.Value);
		}

		public override BoundNode VisitGroupAggregation(BoundGroupAggregation node)
		{
			return Visit(node.Group);
		}

		public override BoundNode VisitQueryLambda(BoundQueryLambda node)
		{
			MethodSymbol currentMethodOrLambda = _currentMethodOrLambda;
			_currentMethodOrLambda = node.LambdaSymbol;
			PopulateRangeVariableMapForQueryLambdaRewrite(node, ref _rangeVariableMap, _inExpressionLambda);
			bool instrumentTopLevelNonCompilerGeneratedExpressionsInQuery = _instrumentTopLevelNonCompilerGeneratedExpressionsInQuery;
			SynthesizedLambdaKind synthesizedKind = node.LambdaSymbol.SynthesizedKind;
			bool flag = synthesizedKind == SynthesizedLambdaKind.AggregateQueryLambda || synthesizedKind == SynthesizedLambdaKind.LetVariableQueryLambda;
			_instrumentTopLevelNonCompilerGeneratedExpressionsInQuery = !flag;
			BoundStatement boundStatement = CreateReturnStatementForQueryLambdaBody(VisitExpressionNode(node.Expression), node);
			if (flag && this.Instrument)
			{
				boundStatement = _instrumenterOpt.InstrumentQueryLambdaBody(node, boundStatement);
			}
			RemoveRangeVariables(node, _rangeVariableMap);
			_instrumentTopLevelNonCompilerGeneratedExpressionsInQuery = instrumentTopLevelNonCompilerGeneratedExpressionsInQuery;
			_hasLambdas = true;
			BoundLambda result = RewriteQueryLambda(boundStatement, node);
			_currentMethodOrLambda = currentMethodOrLambda;
			return result;
		}

		internal static void PopulateRangeVariableMapForQueryLambdaRewrite(BoundQueryLambda node, ref Dictionary<RangeVariableSymbol, BoundExpression> rangeVariableMap, bool inExpressionLambda)
		{
			ImmutableArray<RangeVariableSymbol> rangeVariables = node.RangeVariables;
			if (rangeVariables.Length <= 0)
			{
				return;
			}
			if (rangeVariableMap == null)
			{
				rangeVariableMap = new Dictionary<RangeVariableSymbol, BoundExpression>();
			}
			int firstUnmappedRangeVariable = 0;
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = node.LambdaSymbol.Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				string name = current.Name;
				bool flag = name.StartsWith("$", StringComparison.Ordinal);
				if (flag && string.Equals(name, "$VB$ItAnonymous", StringComparison.Ordinal))
				{
					continue;
				}
				BoundParameter boundParameter = new BoundParameter(node.Syntax, current, isLValue: false, current.Type);
				if (flag && IsCompoundVariableName(name))
				{
					if (TypeSymbolExtensions.IsErrorType(current.Type))
					{
						break;
					}
					PopulateRangeVariableMapForAnonymousType(node.Syntax, BoundNodeExtensions.MakeCompilerGenerated(boundParameter), rangeVariables, ref firstUnmappedRangeVariable, rangeVariableMap, inExpressionLambda);
				}
				else
				{
					rangeVariableMap.Add(rangeVariables[firstUnmappedRangeVariable], boundParameter);
					firstUnmappedRangeVariable++;
				}
			}
		}

		private static void PopulateRangeVariableMapForAnonymousType(SyntaxNode syntax, BoundExpression anonymousTypeInstance, ImmutableArray<RangeVariableSymbol> rangeVariables, ref int firstUnmappedRangeVariable, Dictionary<RangeVariableSymbol, BoundExpression> rangeVariableMap, bool inExpressionLambda)
		{
			ImmutableArray<AnonymousTypeManager.AnonymousTypePropertyPublicSymbol>.Enumerator enumerator = ((AnonymousTypeManager.AnonymousTypePublicSymbol)anonymousTypeInstance.Type).Properties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PropertySymbol current = enumerator.Current;
				BoundExpression boundExpression = null;
				if (inExpressionLambda)
				{
					boundExpression = new BoundPropertyAccess(syntax, current, null, PropertyAccessKind.Get, isWriteable: false, isLValue: false, anonymousTypeInstance, ImmutableArray<BoundExpression>.Empty, BitVector.Null, current.Type);
				}
				else
				{
					MethodSymbol getMethod = current.GetMethod;
					boundExpression = new BoundCall(syntax, getMethod, null, anonymousTypeInstance, ImmutableArray<BoundExpression>.Empty, null, getMethod.ReturnType);
				}
				string name = current.Name;
				if (name.StartsWith("$", StringComparison.Ordinal) && IsCompoundVariableName(name))
				{
					PopulateRangeVariableMapForAnonymousType(syntax, BoundNodeExtensions.MakeCompilerGenerated(boundExpression), rangeVariables, ref firstUnmappedRangeVariable, rangeVariableMap, inExpressionLambda);
					continue;
				}
				rangeVariableMap.Add(rangeVariables[firstUnmappedRangeVariable], boundExpression);
				firstUnmappedRangeVariable++;
			}
		}

		private static bool IsCompoundVariableName(string name)
		{
			if (!name.Equals("$VB$It", StringComparison.Ordinal) && !name.Equals("$VB$It1", StringComparison.Ordinal))
			{
				return name.Equals("$VB$It2", StringComparison.Ordinal);
			}
			return true;
		}

		internal static BoundStatement CreateReturnStatementForQueryLambdaBody(BoundExpression rewrittenBody, BoundQueryLambda originalNode, bool hasErrors = false)
		{
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundReturnStatement(originalNode.Syntax, rewrittenBody, null, null, hasErrors));
		}

		internal static void RemoveRangeVariables(BoundQueryLambda originalNode, Dictionary<RangeVariableSymbol, BoundExpression> rangeVariableMap)
		{
			ImmutableArray<RangeVariableSymbol>.Enumerator enumerator = originalNode.RangeVariables.GetEnumerator();
			while (enumerator.MoveNext())
			{
				RangeVariableSymbol current = enumerator.Current;
				rangeVariableMap.Remove(current);
			}
		}

		internal static BoundLambda RewriteQueryLambda(BoundStatement rewrittenBody, BoundQueryLambda originalNode)
		{
			BoundBlock body = BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(originalNode.Syntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(rewrittenBody)));
			BoundLambda boundLambda = new BoundLambda(originalNode.Syntax, originalNode.LambdaSymbol, body, ImmutableBindingDiagnostic<AssemblySymbol>.Empty, null, ConversionKind.DelegateRelaxationLevelNone, MethodConversionKind.Identity);
			BoundNodeExtensions.MakeCompilerGenerated(boundLambda);
			return boundLambda;
		}

		public override BoundNode VisitRangeVariable(BoundRangeVariable node)
		{
			return _rangeVariableMap[node.RangeVariable];
		}

		public override BoundNode VisitQueryableSource(BoundQueryableSource node)
		{
			return Visit(node.Source);
		}

		public override BoundNode VisitQuerySource(BoundQuerySource node)
		{
			return Visit(node.Expression);
		}

		public override BoundNode VisitToQueryableCollectionConversion(BoundToQueryableCollectionConversion node)
		{
			return Visit(node.ConversionCall);
		}

		public override BoundNode VisitAggregateClause(BoundAggregateClause node)
		{
			if (node.CapturedGroupOpt != null)
			{
				SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, node.CapturedGroupOpt.Type, SynthesizedLocalKind.LoweringTemp);
				AddPlaceholderReplacement(node.GroupPlaceholderOpt, new BoundLocal(node.Syntax, synthesizedLocal, isLValue: false, synthesizedLocal.Type));
				BoundSequence result = new BoundSequence(node.Syntax, ImmutableArray.Create((LocalSymbol)synthesizedLocal), ImmutableArray.Create((BoundExpression)new BoundAssignmentOperator(node.Syntax, new BoundLocal(node.Syntax, synthesizedLocal, isLValue: true, synthesizedLocal.Type), VisitExpressionNode(node.CapturedGroupOpt), suppressObjectClone: true, synthesizedLocal.Type)), VisitExpressionNode(node.UnderlyingExpression), node.Type);
				RemovePlaceholderReplacement(node.GroupPlaceholderOpt);
				return result;
			}
			return Visit(node.UnderlyingExpression);
		}

		public override BoundNode VisitRaiseEventStatement(BoundRaiseEventStatement node)
		{
			SyntaxNode syntax = node.Syntax;
			UnstructuredExceptionHandlingContext saved = LeaveUnstructuredExceptionHandlingContext(node);
			BoundCall boundCall = (BoundCall)node.EventInvocation;
			BoundExpression boundExpression = boundCall.ReceiverOpt;
			BoundStatement boundStatement;
			if (boundExpression == null || BoundExpressionExtensions.IsMeReference(boundExpression))
			{
				boundStatement = new BoundExpressionStatement(syntax, VisitExpressionNode(boundCall));
			}
			else
			{
				if (node.EventSymbol.IsWindowsRuntimeEvent)
				{
					boundExpression = GetWindowsRuntimeEventReceiver(syntax, boundExpression);
				}
				LocalSymbol localSymbol = new SynthesizedLocal(_currentMethodOrLambda, boundExpression.Type, SynthesizedLocalKind.LoweringTemp);
				BoundLocal boundLocal = BoundNodeExtensions.MakeCompilerGenerated(new BoundLocal(syntax, localSymbol, localSymbol.Type));
				BoundExpressionStatement item = BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(syntax, new BoundAssignmentOperator(syntax, boundLocal, boundExpression, suppressObjectClone: true, boundExpression.Type)));
				boundCall = boundCall.Update(boundCall.Method, boundCall.MethodGroupOpt, boundLocal, boundCall.Arguments, boundCall.DefaultArguments, boundCall.ConstantValueOpt, boundCall.IsLValue, boundCall.SuppressObjectClone, boundCall.Type);
				BoundExpressionStatement item2 = new BoundExpressionStatement(syntax, VisitExpressionNode(boundCall));
				BoundBinaryOperator condition = BoundNodeExtensions.MakeCompilerGenerated(new BoundBinaryOperator(syntax, BinaryOperatorKind.Is, boundLocal.MakeRValue(), new BoundLiteral(syntax, ConstantValue.Nothing, Compilation.GetSpecialType(SpecialType.System_Object)), @checked: false, Compilation.GetSpecialType(SpecialType.System_Boolean)));
				GeneratedLabelSymbol label = new GeneratedLabelSymbol("skipEventRaise");
				BoundConditionalGoto item3 = BoundNodeExtensions.MakeCompilerGenerated(new BoundConditionalGoto(syntax, condition, jumpIfTrue: true, label));
				boundStatement = new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), ImmutableArray.Create(localSymbol), ImmutableArray.Create<BoundStatement>(item, item3, item2, new BoundLabelStatement(syntax, label)));
			}
			RestoreUnstructuredExceptionHandlingContext(node, saved);
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, canThrow: true);
			}
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentRaiseEventStatement(node, boundStatement);
			}
			return boundStatement;
		}

		private BoundExpression GetWindowsRuntimeEventReceiver(SyntaxNode syntax, BoundExpression rewrittenReceiver)
		{
			NamedTypeSymbol type = (NamedTypeSymbol)rewrittenReceiver.Type;
			MethodSymbol origMember = (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__GetOrCreateEventRegistrationTokenTable);
			origMember = SymbolExtensions.AsMember(origMember, type);
			PropertySymbol result = null;
			if (TryGetWellknownMember<PropertySymbol>(out result, WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__InvocationList, syntax))
			{
				MethodSymbol getMethod = result.GetMethod;
				if ((object)getMethod != null)
				{
					getMethod = SymbolExtensions.AsMember(getMethod, type);
					BoundCall receiverOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(syntax, origMember, null, null, ImmutableArray.Create(rewrittenReceiver), null, isLValue: false, suppressObjectClone: false, origMember.ReturnType));
					return BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(syntax, getMethod, null, receiverOpt, ImmutableArray<BoundExpression>.Empty, null, isLValue: false, suppressObjectClone: false, getMethod.ReturnType));
				}
				MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__InvocationList);
				string accessorName = Binder.GetAccessorName(result.Name, MethodKind.PropertyGet, isWinMd: false);
				DiagnosticInfo diagnosticForMissingRuntimeHelper = MissingRuntimeMemberDiagnosticHelper.GetDiagnosticForMissingRuntimeHelper(descriptor.DeclaringTypeMetadataName, accessorName, _compilationState.Compilation.Options.EmbedVbCoreRuntime);
				_diagnostics.Add(diagnosticForMissingRuntimeHelper, syntax.GetLocation());
			}
			return new BoundBadExpression(syntax, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(rewrittenReceiver), ErrorTypeSymbol.UnknownResultType, hasErrors: true);
		}

		public override BoundNode VisitRedimStatement(BoundRedimStatement node)
		{
			if (node.Clauses.Length == 1)
			{
				return Visit(node.Clauses[0]);
			}
			BoundStatement[] array = new BoundStatement[node.Clauses.Length - 1 + 1];
			int num = node.Clauses.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = (BoundStatement)Visit(node.Clauses[i]);
			}
			return new BoundStatementList(node.Syntax, array.AsImmutableOrNull());
		}

		public override BoundNode VisitRedimClause(BoundRedimClause node)
		{
			BoundExpression boundExpression = new BoundArrayCreation(node.Syntax, node.Indices, null, node.ArrayTypeOpt);
			ArrayBuilder<SynthesizedLocal> arrayBuilder = null;
			BoundExpression boundExpression2 = node.Operand;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
			MethodSymbol result = null;
			if (node.Preserve && TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Utils__CopyArray, node.Syntax))
			{
				arrayBuilder = ArrayBuilder<SynthesizedLocal>.GetInstance();
				UseTwiceRewriter.Result result2 = UseTwiceRewriter.UseTwice(_currentMethodOrLambda, boundExpression2, arrayBuilder);
				boundExpression2 = result2.First;
				BoundExpression second = result2.Second;
				second = ((second.Kind == BoundKind.PropertyAccess) ? ((BoundPropertyAccess)second).SetAccessKind(PropertyAccessKind.Get) : ((!BoundExpressionExtensions.IsLateBound(second)) ? second.MakeRValue() : BoundExpressionExtensions.SetLateBoundAccessKind(second, LateBoundAccessKind.Get)));
				TypeSymbol type = result.Parameters[0].Type;
				second = new BoundDirectCast(node.Syntax, second, Conversions.ClassifyDirectCastConversion(second.Type, type, ref useSiteInfo), type);
				boundExpression = new BoundDirectCast(node.Syntax, boundExpression, Conversions.ClassifyDirectCastConversion(boundExpression.Type, type, ref useSiteInfo), type);
				boundExpression = new BoundCall(node.Syntax, result, null, null, ImmutableArray.Create(second, boundExpression), null, type);
			}
			boundExpression = new BoundDirectCast(node.Syntax, boundExpression, Conversions.ClassifyDirectCastConversion(boundExpression.Type, boundExpression2.Type, ref useSiteInfo), boundExpression2.Type);
			_diagnostics.Add(node, useSiteInfo);
			if (boundExpression2.Kind == BoundKind.PropertyAccess)
			{
				boundExpression2 = ((BoundPropertyAccess)boundExpression2).SetAccessKind(PropertyAccessKind.Set);
			}
			else if (BoundExpressionExtensions.IsLateBound(boundExpression2))
			{
				boundExpression2 = BoundExpressionExtensions.SetLateBoundAccessKind(boundExpression2, LateBoundAccessKind.Set);
			}
			BoundExpression boundExpression3 = new BoundAssignmentOperator(node.Syntax, boundExpression2, boundExpression, suppressObjectClone: true);
			if (arrayBuilder != null)
			{
				if (arrayBuilder.Count > 0)
				{
					boundExpression3 = new BoundSequence(node.Syntax, StaticCast<LocalSymbol>.From(arrayBuilder.ToImmutableAndFree()), ImmutableArray.Create(boundExpression3), null, TypeSymbolExtensions.IsVoidType(boundExpression3.Type) ? boundExpression3.Type : Compilation.GetSpecialType(SpecialType.System_Void));
				}
				else
				{
					arrayBuilder.Free();
				}
			}
			return Visit(new BoundExpressionStatement(node.Syntax, boundExpression3));
		}

		public override BoundNode VisitReturnStatement(BoundReturnStatement node)
		{
			BoundStatement boundStatement = RewriteReturnStatement(node);
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, node.ExpressionOpt != null);
			}
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement) || (node.ExpressionOpt != null && this.get_Instrument((BoundNode)node.ExpressionOpt)))
			{
				boundStatement = _instrumenterOpt.InstrumentReturnStatement(node, boundStatement);
			}
			return boundStatement;
		}

		private BoundStatement RewriteReturnStatement(BoundReturnStatement node)
		{
			node = (BoundReturnStatement)base.VisitReturnStatement(node);
			if (_inExpressionLambda)
			{
				node = node.Update(node.ExpressionOpt, null, null);
			}
			else if (!node.IsEndOfMethodReturn())
			{
				if (node.ExpressionOpt == null)
				{
					return new BoundGotoStatement(node.Syntax, node.ExitLabelOpt, null);
				}
				LocalSymbol functionLocalOpt = node.FunctionLocalOpt;
				if ((object)functionLocalOpt != null)
				{
					if (_currentMethodOrLambda.IsAsync)
					{
						return node;
					}
					BoundLocal left = new BoundLocal(node.Syntax, functionLocalOpt, functionLocalOpt.Type);
					SyntaxNode syntax = node.Syntax;
					BoundStatement item = new BoundExpressionStatement(syntax, new BoundAssignmentOperator(syntax, left, node.ExpressionOpt, suppressObjectClone: true, functionLocalOpt.Type));
					BoundStatement item2 = new BoundGotoStatement(syntax, node.ExitLabelOpt, null);
					return new BoundStatementList(syntax, ImmutableArray.Create(item, item2));
				}
			}
			else if (_currentMethodOrLambda.IsAsync && (_flags & RewritingFlags.AllowEndOfMethodReturnWithExpression) == 0)
			{
				node = node.Update(null, null, null);
			}
			return node;
		}

		public override BoundNode VisitSelectStatement(BoundSelectStatement node)
		{
			return RewriteSelectStatement(node, node.Syntax, node.ExpressionStatement, node.ExprPlaceholderOpt, node.CaseBlocks, node.RecommendSwitchTable, node.ExitLabel);
		}

		protected BoundNode RewriteSelectStatement(BoundSelectStatement node, SyntaxNode syntaxNode, BoundExpressionStatement selectExpressionStmt, BoundRValuePlaceholder exprPlaceholderOpt, ImmutableArray<BoundCaseBlock> caseBlocks, bool recommendSwitchTable, LabelSymbol exitLabel)
		{
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			bool num = this.get_Instrument((BoundNode)node);
			if (num)
			{
				BoundStatement boundStatement = _instrumenterOpt.CreateSelectStatementPrologue(node);
				if (boundStatement != null)
				{
					instance.Add(boundStatement);
				}
			}
			BoundExpression rewrittenSelectExpression = null;
			ImmutableArray<LocalSymbol> tempLocals = default(ImmutableArray<LocalSymbol>);
			BoundLabelStatement endSelectResumeLabel = null;
			bool generateUnstructuredExceptionHandlingResumeCode = ShouldGenerateUnstructuredExceptionHandlingResumeCode(node);
			BoundExpressionStatement boundExpressionStatement = RewriteSelectExpression(generateUnstructuredExceptionHandlingResumeCode, selectExpressionStmt, out rewrittenSelectExpression, out tempLocals, instance, caseBlocks, recommendSwitchTable, out endSelectResumeLabel);
			if (exprPlaceholderOpt != null)
			{
				AddPlaceholderReplacement(exprPlaceholderOpt, rewrittenSelectExpression);
			}
			if (!caseBlocks.Any())
			{
				instance.Add(boundExpressionStatement);
			}
			else if (recommendSwitchTable)
			{
				if (TypeSymbolExtensions.IsStringType(rewrittenSelectExpression.Type))
				{
					EnsureStringHashFunction(node);
				}
				instance.Add(node.Update(boundExpressionStatement, exprPlaceholderOpt, VisitList(caseBlocks), recommendSwitchTable: true, exitLabel));
			}
			else
			{
				LocalSymbol lazyConditionalBranchLocal = null;
				instance.Add(RewriteCaseBlocksRecursive(node, generateUnstructuredExceptionHandlingResumeCode, caseBlocks, 0, ref lazyConditionalBranchLocal));
				if ((object)lazyConditionalBranchLocal != null)
				{
					tempLocals = tempLocals.Add(lazyConditionalBranchLocal);
				}
				instance.Add(new BoundLabelStatement(syntaxNode, exitLabel));
			}
			if (exprPlaceholderOpt != null)
			{
				RemovePlaceholderReplacement(exprPlaceholderOpt);
			}
			BoundStatement boundStatement2 = endSelectResumeLabel;
			if (num)
			{
				boundStatement2 = _instrumenterOpt.InstrumentSelectStatementEpilogue(node, boundStatement2);
			}
			if (boundStatement2 != null)
			{
				instance.Add(boundStatement2);
			}
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundBlock(syntaxNode, default(SyntaxList<StatementSyntax>), tempLocals, instance.ToImmutableAndFree()));
		}

		private void EnsureStringHashFunction(BoundSelectStatement node)
		{
			BoundExpression expression = node.ExpressionStatement.Expression;
			NamedTypeSymbol wellKnownType = Compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators);
			WellKnownMember wellKnownMember = ((TypeSymbolExtensions.IsErrorType(wellKnownType) && wellKnownType is MissingMetadataTypeSymbol) ? WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareStringStringStringBoolean : WellKnownMember.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators__CompareStringStringStringBoolean);
			MethodSymbol memberSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(wellKnownMember);
			ReportMissingOrBadRuntimeHelper(expression, wellKnownMember, memberSymbol);
			MethodSymbol memberSymbol2 = (MethodSymbol)ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_String__Chars);
			ReportMissingOrBadRuntimeHelper(expression, SpecialMember.System_String__Chars, memberSymbol2);
			ReportBadType(expression, Compilation.GetSpecialType(SpecialType.System_Int32));
			ReportBadType(expression, Compilation.GetSpecialType(SpecialType.System_UInt32));
			ReportBadType(expression, Compilation.GetSpecialType(SpecialType.System_String));
			if (_emitModule != null && ShouldGenerateHashTableSwitch(_emitModule, node))
			{
				PrivateImplementationDetails privateImplClass = _emitModule.GetPrivateImplClass(node.Syntax, _diagnostics.DiagnosticBag);
				if (privateImplClass.GetMethod("ComputeStringHash") == null)
				{
					SynthesizedStringSwitchHashMethod synthesizedStringSwitchHashMethod = new SynthesizedStringSwitchHashMethod(_emitModule.SourceModule, privateImplClass);
					privateImplClass.TryAddSynthesizedMethod(synthesizedStringSwitchHashMethod.GetCciAdapter());
				}
			}
		}

		private BoundExpressionStatement RewriteSelectExpression(bool generateUnstructuredExceptionHandlingResumeCode, BoundExpressionStatement selectExpressionStmt, out BoundExpression rewrittenSelectExpression, out ImmutableArray<LocalSymbol> tempLocals, ArrayBuilder<BoundStatement> statementBuilder, ImmutableArray<BoundCaseBlock> caseBlocks, bool recommendSwitchTable, out BoundLabelStatement endSelectResumeLabel)
		{
			SyntaxNode syntax = selectExpressionStmt.Syntax;
			if (generateUnstructuredExceptionHandlingResumeCode)
			{
				RegisterUnstructuredExceptionHandlingResumeTarget(syntax, canThrow: true, statementBuilder);
				endSelectResumeLabel = RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(syntax);
			}
			else
			{
				endSelectResumeLabel = null;
			}
			rewrittenSelectExpression = VisitExpressionNode(selectExpressionStmt.Expression);
			if (caseBlocks.Any() && (!recommendSwitchTable || rewrittenSelectExpression.Kind != BoundKind.Local))
			{
				TypeSymbol type = rewrittenSelectExpression.Type;
				SelectStatementSyntax selectStatement = ((SelectBlockSyntax)syntax.Parent).SelectStatement;
				SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_currentMethodOrLambda, type, SynthesizedLocalKind.SelectCaseValue, selectStatement);
				tempLocals = ImmutableArray.Create((LocalSymbol)synthesizedLocal);
				BoundLocal boundLocal = new BoundLocal(rewrittenSelectExpression.Syntax, synthesizedLocal, type);
				statementBuilder.Add(BoundNodeExtensions.MakeCompilerGenerated(BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(syntax, boundLocal, rewrittenSelectExpression, suppressObjectClone: true, type))));
				rewrittenSelectExpression = boundLocal.MakeRValue();
			}
			else
			{
				tempLocals = ImmutableArray<LocalSymbol>.Empty;
			}
			return selectExpressionStmt.Update(rewrittenSelectExpression);
		}

		private BoundStatement RewriteCaseBlocksRecursive(BoundSelectStatement selectStatement, bool generateUnstructuredExceptionHandlingResumeCode, ImmutableArray<BoundCaseBlock> caseBlocks, int startFrom, ref LocalSymbol lazyConditionalBranchLocal)
		{
			if (startFrom == caseBlocks.Length)
			{
				return null;
			}
			BoundCaseBlock boundCaseBlock = caseBlocks[startFrom];
			ImmutableArray<BoundStatement> unstructuredExceptionHandlingResumeTarget = default(ImmutableArray<BoundStatement>);
			BoundExpression boundExpression = RewriteCaseStatement(generateUnstructuredExceptionHandlingResumeCode, boundCaseBlock.CaseStatement, out unstructuredExceptionHandlingResumeTarget);
			BoundBlock boundBlock = (BoundBlock)VisitBlock(boundCaseBlock.Body);
			if (generateUnstructuredExceptionHandlingResumeCode && startFrom < caseBlocks.Length - 1)
			{
				boundBlock = AppendToBlock(boundBlock, RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(boundCaseBlock.Syntax));
			}
			bool flag = this.get_Instrument((BoundNode)selectStatement);
			BoundStatement result;
			if (boundExpression == null)
			{
				result = ((!flag) ? boundBlock : _instrumenterOpt.InstrumentCaseElseBlock(boundCaseBlock, boundBlock));
			}
			else
			{
				if (flag)
				{
					boundExpression = _instrumenterOpt.InstrumentSelectStatementCaseCondition(selectStatement, boundExpression, _currentMethodOrLambda, ref lazyConditionalBranchLocal);
				}
				result = RewriteIfStatement(boundCaseBlock.Syntax, boundExpression, boundBlock, RewriteCaseBlocksRecursive(selectStatement, generateUnstructuredExceptionHandlingResumeCode, caseBlocks, startFrom + 1, ref lazyConditionalBranchLocal), flag ? boundCaseBlock : null, unstructuredExceptionHandlingResumeTarget);
			}
			return result;
		}

		private BoundExpression RewriteCaseStatement(bool generateUnstructuredExceptionHandlingResumeCode, BoundCaseStatement node, out ImmutableArray<BoundStatement> unstructuredExceptionHandlingResumeTarget)
		{
			if (node.CaseClauses.Any())
			{
				unstructuredExceptionHandlingResumeTarget = (generateUnstructuredExceptionHandlingResumeCode ? RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, canThrow: true) : default(ImmutableArray<BoundStatement>));
				return VisitExpressionNode(node.ConditionOpt);
			}
			unstructuredExceptionHandlingResumeTarget = default(ImmutableArray<BoundStatement>);
			return null;
		}

		private static bool ShouldGenerateHashTableSwitch(PEModuleBuilder module, BoundSelectStatement node)
		{
			if (!module.SupportsPrivateImplClass)
			{
				return false;
			}
			HashSet<ConstantValue> hashSet = new HashSet<ConstantValue>();
			ImmutableArray<BoundCaseBlock>.Enumerator enumerator = node.CaseBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ImmutableArray<BoundCaseClause>.Enumerator enumerator2 = enumerator.Current.CaseStatement.CaseClauses.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					BoundCaseClause current = enumerator2.Current;
					ConstantValue constantValue = null;
					hashSet.Add(current.Kind switch
					{
						BoundKind.SimpleCaseClause => ((BoundSimpleCaseClause)current).ValueOpt.ConstantValueOpt, 
						BoundKind.RelationalCaseClause => ((BoundRelationalCaseClause)current).ValueOpt.ConstantValueOpt, 
						_ => throw ExceptionUtilities.UnexpectedValue(current.Kind), 
					});
				}
			}
			return SwitchStringJumpTableEmitter.ShouldGenerateHashTableSwitch(module, hashSet.Count);
		}

		public override BoundNode VisitCaseBlock(BoundCaseBlock node)
		{
			BoundCaseBlock obj = (BoundCaseBlock)base.VisitCaseBlock(node);
			BoundBlock boundBlock = obj.Body;
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundBlock = AppendToBlock(boundBlock, RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax));
			}
			return obj.Update(obj.CaseStatement, boundBlock);
		}

		private BoundExpression RewriteConcatenateOperator(BoundBinaryOperator node)
		{
			SyntaxNode syntax = node.Syntax;
			BoundExpression left = node.Left;
			BoundExpression right = node.Right;
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, syntax, _compilationState, _diagnostics);
			BoundExpression boundExpression = TryFoldTwoConcatOperands(syntheticBoundNodeFactory, left, right);
			if (boundExpression != null)
			{
				return boundExpression;
			}
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
			FlattenConcatArg(left, instance);
			FlattenConcatArg(right, instance2);
			if (instance.Any() && instance2.Any())
			{
				boundExpression = TryFoldTwoConcatOperands(syntheticBoundNodeFactory, instance.Last(), instance2.First());
				if (boundExpression != null)
				{
					instance2[0] = boundExpression;
					instance.RemoveLast();
				}
			}
			instance.AddRange(instance2);
			instance2.Free();
			switch (instance.Count)
			{
			case 0:
				instance.Free();
				return syntheticBoundNodeFactory.StringLiteral(ConstantValue.Create(""));
			case 1:
			{
				BoundExpression result = instance[0];
				instance.Free();
				return result;
			}
			case 2:
			{
				BoundExpression loweredLeft = instance[0];
				BoundExpression loweredRight = instance[1];
				instance.Free();
				return RewriteStringConcatenationTwoExprs(node, syntheticBoundNodeFactory, loweredLeft, loweredRight);
			}
			case 3:
			{
				BoundExpression loweredFirst2 = instance[0];
				BoundExpression loweredSecond2 = instance[1];
				BoundExpression loweredThird2 = instance[2];
				instance.Free();
				return RewriteStringConcatenationThreeExprs(node, syntheticBoundNodeFactory, loweredFirst2, loweredSecond2, loweredThird2);
			}
			case 4:
			{
				BoundExpression loweredFirst = instance[0];
				BoundExpression loweredSecond = instance[1];
				BoundExpression loweredThird = instance[2];
				BoundExpression loweredFourth = instance[3];
				instance.Free();
				return RewriteStringConcatenationFourExprs(node, syntheticBoundNodeFactory, loweredFirst, loweredSecond, loweredThird, loweredFourth);
			}
			default:
				return RewriteStringConcatenationManyExprs(node, syntheticBoundNodeFactory, instance.ToImmutableAndFree());
			}
		}

		private void FlattenConcatArg(BoundExpression lowered, ArrayBuilder<BoundExpression> flattened)
		{
			switch (lowered.Kind)
			{
			case BoundKind.Call:
			{
				BoundCall boundCall = (BoundCall)lowered;
				MethodSymbol method = boundCall.Method;
				if (!method.IsShared || method.ContainingType.SpecialType != SpecialType.System_String)
				{
					break;
				}
				if ((object)method == Compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringString) || (object)method == Compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringString) || (object)method == Compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringStringString))
				{
					flattened.AddRange(boundCall.Arguments);
					return;
				}
				if ((object)method == Compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringArray) && boundCall.Arguments[0] is BoundArrayCreation boundArrayCreation)
				{
					BoundArrayInitialization initializerOpt = boundArrayCreation.InitializerOpt;
					if (initializerOpt != null)
					{
						flattened.AddRange(initializerOpt.Initializers);
						return;
					}
				}
				break;
			}
			case BoundKind.BinaryConditionalExpression:
			{
				BoundBinaryConditionalExpression boundBinaryConditionalExpression = (BoundBinaryConditionalExpression)lowered;
				if (boundBinaryConditionalExpression.ConvertedTestExpression == null)
				{
					BoundExpression elseExpression = boundBinaryConditionalExpression.ElseExpression;
					if ((object)elseExpression.ConstantValueOpt != null && EmbeddedOperators.CompareString(elseExpression.ConstantValueOpt.StringValue, "", TextCompare: false) == 0)
					{
						flattened.AddRange(boundBinaryConditionalExpression.TestExpression);
						return;
					}
				}
				break;
			}
			}
			flattened.Add(lowered);
		}

		private BoundExpression TryFoldTwoConcatOperands(SyntheticBoundNodeFactory factory, BoundExpression loweredLeft, BoundExpression loweredRight)
		{
			ConstantValue constantValueOpt = loweredLeft.ConstantValueOpt;
			ConstantValue constantValueOpt2 = loweredRight.ConstantValueOpt;
			if ((object)constantValueOpt != null && (object)constantValueOpt2 != null)
			{
				ConstantValue constantValue = TryFoldTwoConcatConsts(constantValueOpt, constantValueOpt2);
				if ((object)constantValue != null)
				{
					return factory.StringLiteral(constantValue);
				}
			}
			if (IsNullOrEmptyStringConstant(loweredLeft))
			{
				if (IsNullOrEmptyStringConstant(loweredRight))
				{
					return factory.Literal("");
				}
				if (!_inExpressionLambda)
				{
					return RewriteStringConcatenationOneExpr(factory, loweredRight);
				}
			}
			else if (!_inExpressionLambda && IsNullOrEmptyStringConstant(loweredRight))
			{
				return RewriteStringConcatenationOneExpr(factory, loweredLeft);
			}
			return null;
		}

		private static bool IsNullOrEmptyStringConstant(BoundExpression operand)
		{
			if ((object)operand.ConstantValueOpt == null || !string.IsNullOrEmpty(operand.ConstantValueOpt.StringValue))
			{
				return BoundExpressionExtensions.IsDefaultValueConstant(operand);
			}
			return true;
		}

		private static ConstantValue TryFoldTwoConcatConsts(ConstantValue leftConst, ConstantValue rightConst)
		{
			string stringValue = leftConst.StringValue;
			string stringValue2 = rightConst.StringValue;
			if (!leftConst.IsDefaultValue && !rightConst.IsDefaultValue && stringValue.Length + stringValue2.Length < 0)
			{
				return null;
			}
			return ConstantValue.Create(stringValue + stringValue2);
		}

		private static BoundExpression RewriteStringConcatenationOneExpr(SyntheticBoundNodeFactory factory, BoundExpression loweredOperand)
		{
			return factory.BinaryConditional(loweredOperand, factory.Literal(""));
		}

		private BoundExpression RewriteStringConcatenationTwoExprs(BoundExpression node, SyntheticBoundNodeFactory factory, BoundExpression loweredLeft, BoundExpression loweredRight)
		{
			MethodSymbol methodSymbol = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_String__ConcatStringString);
			if (!ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_String__ConcatStringString, methodSymbol))
			{
				return factory.Call(null, methodSymbol, loweredLeft, loweredRight);
			}
			return node;
		}

		private BoundExpression RewriteStringConcatenationThreeExprs(BoundExpression node, SyntheticBoundNodeFactory factory, BoundExpression loweredFirst, BoundExpression loweredSecond, BoundExpression loweredThird)
		{
			MethodSymbol methodSymbol = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringString);
			if (!ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_String__ConcatStringStringString, methodSymbol))
			{
				return factory.Call(null, methodSymbol, loweredFirst, loweredSecond, loweredThird);
			}
			return node;
		}

		private BoundExpression RewriteStringConcatenationFourExprs(BoundExpression node, SyntheticBoundNodeFactory factory, BoundExpression loweredFirst, BoundExpression loweredSecond, BoundExpression loweredThird, BoundExpression loweredFourth)
		{
			MethodSymbol methodSymbol = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringStringString);
			if (!ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_String__ConcatStringStringStringString, methodSymbol))
			{
				return factory.Call(null, methodSymbol, loweredFirst, loweredSecond, loweredThird, loweredFourth);
			}
			return node;
		}

		private BoundExpression RewriteStringConcatenationManyExprs(BoundExpression node, SyntheticBoundNodeFactory factory, ImmutableArray<BoundExpression> loweredArgs)
		{
			MethodSymbol methodSymbol = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_String__ConcatStringArray);
			if (!ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_String__ConcatStringArray, methodSymbol))
			{
				BoundExpression item = factory.Array(node.Type, loweredArgs);
				return factory.Call(null, methodSymbol, ImmutableArray.Create(item));
			}
			return node;
		}

		public override BoundNode VisitSyncLockStatement(BoundSyncLockStatement node)
		{
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			SyncLockBlockSyntax syncLockBlockSyntax = (SyncLockBlockSyntax)node.Syntax;
			BoundExpression boundExpression = VisitExpressionNode(node.LockExpression);
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Object);
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo();
			ConversionKind key = Conversions.ClassifyConversion(boundExpression.Type, specialType, ref useSiteInfo).Key;
			_diagnostics.Add(node, useSiteInfo);
			if (!Conversions.IsIdentityConversion(key))
			{
				bool integerOverflow = default(bool);
				ConstantValue constantValueOpt = Conversions.TryFoldConstantConversion(boundExpression, specialType, ref integerOverflow);
				boundExpression = TransformRewrittenConversion(new BoundConversion(node.LockExpression.Syntax, boundExpression, key, @checked: false, explicitCastInCode: false, constantValueOpt, specialType));
			}
			LocalSymbol localSymbol = new SynthesizedLocal(_currentMethodOrLambda, specialType, SynthesizedLocalKind.Lock, syncLockBlockSyntax.SyncLockStatement);
			BoundLocal boundLocal = new BoundLocal(syncLockBlockSyntax, localSymbol, specialType);
			bool num = this.get_Instrument((BoundNode)node);
			if (num)
			{
				BoundStatement boundStatement = _instrumenterOpt.CreateSyncLockStatementPrologue(node);
				if (boundStatement != null)
				{
					instance.Add(boundStatement);
				}
			}
			BoundStatement boundStatement2 = BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(syncLockBlockSyntax, boundLocal, boundExpression, suppressObjectClone: true, specialType));
			boundLocal = boundLocal.MakeRValue();
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement2 = RegisterUnstructuredExceptionHandlingResumeTarget(syncLockBlockSyntax, boundStatement2, canThrow: true);
			}
			UnstructuredExceptionHandlingContext saved = LeaveUnstructuredExceptionHandlingContext(node);
			if (num)
			{
				boundStatement2 = _instrumenterOpt.InstrumentSyncLockObjectCapture(node, boundStatement2);
			}
			instance.Add(boundStatement2);
			MethodSymbol result = null;
			if (TypeSymbolExtensions.IsObjectType(node.LockExpression.Type) && TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl__CheckForSyncLockOnValueType, syncLockBlockSyntax, isOptional: true))
			{
				BoundExpressionStatement boundExpressionStatement = BoundExpressionExtensions.ToStatement(new BoundCall(syncLockBlockSyntax, result, null, null, ImmutableArray.Create((BoundExpression)boundLocal), null, result.ReturnType, suppressObjectClone: true));
				boundExpressionStatement.SetWasCompilerGenerated();
				instance.Add(boundExpressionStatement);
			}
			BoundLocal boundLockTakenLocal = null;
			BoundStatement boundLockTakenInitialization = null;
			BoundStatement boundStatement3 = GenerateMonitorEnter(node.LockExpression.Syntax, boundLocal, out boundLockTakenLocal, out boundLockTakenInitialization);
			ImmutableArray<LocalSymbol> locals;
			ImmutableArray<BoundStatement> statements;
			if (boundLockTakenLocal != null)
			{
				locals = ImmutableArray.Create(localSymbol, boundLockTakenLocal.LocalSymbol);
				instance.Add(boundLockTakenInitialization);
				statements = ImmutableArray.Create(boundStatement3, (BoundBlock)Visit(node.Body));
			}
			else
			{
				locals = ImmutableArray.Create(localSymbol);
				instance.Add(boundStatement3);
				statements = ImmutableArray.Create((BoundStatement)(BoundBlock)Visit(node.Body));
			}
			BoundBlock tryBlock = new BoundBlock(syncLockBlockSyntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, statements);
			BoundBlock boundBlock = new BoundBlock(statements: ImmutableArray.Create(GenerateMonitorExit(syncLockBlockSyntax, boundLocal, boundLockTakenLocal)), syntax: syncLockBlockSyntax, statementListSyntax: default(SyntaxList<StatementSyntax>), locals: ImmutableArray<LocalSymbol>.Empty);
			if (num)
			{
				boundBlock = (BoundBlock)Concat(boundBlock, _instrumenterOpt.CreateSyncLockExitDueToExceptionEpilogue(node));
			}
			BoundStatement item = RewriteTryStatement(syncLockBlockSyntax, tryBlock, ImmutableArray<BoundCatchBlock>.Empty, boundBlock, null);
			instance.Add(item);
			if (num)
			{
				BoundStatement boundStatement4 = _instrumenterOpt.CreateSyncLockExitNormallyEpilogue(node);
				if (boundStatement4 != null)
				{
					instance.Add(boundStatement4);
				}
			}
			RestoreUnstructuredExceptionHandlingContext(node, saved);
			return new BoundBlock(syncLockBlockSyntax, default(SyntaxList<StatementSyntax>), locals, instance.ToImmutableAndFree());
		}

		private BoundStatement GenerateMonitorEnter(SyntaxNode syntaxNode, BoundExpression boundLockObject, out BoundLocal boundLockTakenLocal, out BoundStatement boundLockTakenInitialization)
		{
			boundLockTakenLocal = null;
			boundLockTakenInitialization = null;
			MethodSymbol result = null;
			ImmutableArray<BoundExpression> immutableArray;
			if (TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.System_Threading_Monitor__Enter2, syntaxNode, isOptional: true))
			{
				LocalSymbol localSymbol = ((VisualBasicExtensions.Kind(syntaxNode.Parent) != SyntaxKind.SyncLockStatement) ? new SynthesizedLocal(_currentMethodOrLambda, result.Parameters[1].Type, SynthesizedLocalKind.LoweringTemp) : new SynthesizedLocal(_currentMethodOrLambda, result.Parameters[1].Type, SynthesizedLocalKind.LockTaken, (SyncLockStatementSyntax)syntaxNode.Parent));
				boundLockTakenLocal = new BoundLocal(syntaxNode, localSymbol, localSymbol.Type);
				boundLockTakenInitialization = BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(syntaxNode, boundLockTakenLocal, new BoundLiteral(syntaxNode, ConstantValue.False, boundLockTakenLocal.Type), suppressObjectClone: true, boundLockTakenLocal.Type));
				boundLockTakenInitialization.SetWasCompilerGenerated();
				immutableArray = ImmutableArray.Create(boundLockObject, boundLockTakenLocal);
				boundLockTakenLocal = boundLockTakenLocal.MakeRValue();
			}
			else
			{
				TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.System_Threading_Monitor__Enter, syntaxNode);
				immutableArray = ImmutableArray.Create(boundLockObject);
			}
			if ((object)result != null)
			{
				BoundExpressionStatement boundExpressionStatement = BoundExpressionExtensions.ToStatement(new BoundCall(syntaxNode, result, null, null, immutableArray, null, result.ReturnType, suppressObjectClone: true));
				boundExpressionStatement.SetWasCompilerGenerated();
				return boundExpressionStatement;
			}
			return BoundExpressionExtensions.ToStatement(new BoundBadExpression(syntaxNode, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, immutableArray, ErrorTypeSymbol.UnknownResultType, hasErrors: true));
		}

		private BoundStatement GenerateMonitorExit(SyntaxNode syntaxNode, BoundExpression boundLockObject, BoundLocal boundLockTakenLocal)
		{
			MethodSymbol result = null;
			BoundExpression node = ((!TryGetWellknownMember<MethodSymbol>(out result, WellKnownMember.System_Threading_Monitor__Exit, syntaxNode)) ? ((BoundExpression)new BoundBadExpression(syntaxNode, LookupResultKind.NotReferencable, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(boundLockObject), ErrorTypeSymbol.UnknownResultType, hasErrors: true)) : ((BoundExpression)new BoundCall(syntaxNode, result, null, null, ImmutableArray.Create(boundLockObject), null, result.ReturnType, suppressObjectClone: true)));
			BoundExpressionStatement boundExpressionStatement = BoundExpressionExtensions.ToStatement(node);
			boundExpressionStatement.SetWasCompilerGenerated();
			if (boundLockTakenLocal != null)
			{
				BoundBinaryOperator rewrittenCondition = new BoundBinaryOperator(syntaxNode, BinaryOperatorKind.Equals, boundLockTakenLocal, new BoundLiteral(syntaxNode, ConstantValue.True, boundLockTakenLocal.Type), @checked: false, boundLockTakenLocal.Type);
				return RewriteIfStatement(syntaxNode, rewrittenCondition, boundExpressionStatement, null, null);
			}
			return boundExpressionStatement;
		}

		public override BoundNode VisitThrowStatement(BoundThrowStatement node)
		{
			BoundExpression boundExpression = node.ExpressionOpt;
			if (boundExpression != null)
			{
				boundExpression = VisitExpressionNode(boundExpression);
				if (boundExpression.Type.SpecialType == SpecialType.System_Int32)
				{
					MethodSymbol methodSymbol = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics).WellKnownMember<MethodSymbol>(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__CreateProjectError);
					if ((object)methodSymbol != null)
					{
						boundExpression = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(boundExpression), null, methodSymbol.ReturnType);
					}
				}
			}
			BoundStatement boundStatement = node.Update(boundExpression);
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				boundStatement = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, canThrow: true);
			}
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentThrowStatement(node, boundStatement);
			}
			return boundStatement;
		}

		public override BoundNode VisitTryStatement(BoundTryStatement node)
		{
			BoundBlock tryBlock = RewriteTryBlock(node);
			ImmutableArray<BoundCatchBlock> catchBlocks = VisitList(node.CatchBlocks);
			BoundBlock finallyBlockOpt = RewriteFinallyBlock(node);
			BoundStatement boundStatement = RewriteTryStatement(node.Syntax, tryBlock, catchBlocks, finallyBlockOpt, node.ExitLabelOpt);
			if (this.get_Instrument((BoundNode)node) && node.Syntax is TryBlockSyntax)
			{
				boundStatement = _instrumenterOpt.InstrumentTryStatement(node, boundStatement);
			}
			return boundStatement;
		}

		private static bool HasSideEffects(BoundStatement statement)
		{
			if (statement == null)
			{
				return false;
			}
			switch (statement.Kind)
			{
			case BoundKind.NoOpStatement:
				return false;
			case BoundKind.Block:
			{
				ImmutableArray<BoundStatement>.Enumerator enumerator = ((BoundBlock)statement).Statements.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (HasSideEffects(enumerator.Current))
					{
						return true;
					}
				}
				return false;
			}
			case BoundKind.SequencePoint:
				return HasSideEffects(((BoundSequencePoint)statement).StatementOpt);
			case BoundKind.SequencePointWithSpan:
				return HasSideEffects(((BoundSequencePointWithSpan)statement).StatementOpt);
			default:
				return true;
			}
		}

		public BoundStatement RewriteTryStatement(SyntaxNode syntaxNode, BoundBlock tryBlock, ImmutableArray<BoundCatchBlock> catchBlocks, BoundBlock finallyBlockOpt, LabelSymbol exitLabelOpt)
		{
			if (!OptimizationLevelIsDebug)
			{
				if (!HasSideEffects(tryBlock))
				{
					catchBlocks = ImmutableArray<BoundCatchBlock>.Empty;
				}
				if (!HasSideEffects(finallyBlockOpt))
				{
					finallyBlockOpt = null;
				}
				if (catchBlocks.IsDefaultOrEmpty && finallyBlockOpt == null)
				{
					if ((object)exitLabelOpt == null)
					{
						return tryBlock;
					}
					return new BoundStatementList(syntaxNode, ImmutableArray.Create((BoundStatement)tryBlock, (BoundStatement)new BoundLabelStatement(syntaxNode, exitLabelOpt)));
				}
			}
			BoundStatement result = new BoundTryStatement(syntaxNode, tryBlock, catchBlocks, finallyBlockOpt, exitLabelOpt);
			ImmutableArray<BoundCatchBlock>.Enumerator enumerator = catchBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundCatchBlock current = enumerator.Current;
				ReportErrorsOnCatchBlockHelpers(current);
			}
			return result;
		}

		private BoundBlock RewriteFinallyBlock(BoundTryStatement tryStatement)
		{
			BoundBlock finallyBlockOpt = tryStatement.FinallyBlockOpt;
			if (finallyBlockOpt == null)
			{
				return finallyBlockOpt;
			}
			BoundBlock boundBlock = (BoundBlock)Visit(finallyBlockOpt);
			if (this.get_Instrument((BoundNode)tryStatement) && finallyBlockOpt.Syntax is FinallyBlockSyntax)
			{
				boundBlock = PrependWithPrologue(boundBlock, _instrumenterOpt.CreateFinallyBlockPrologue(tryStatement));
			}
			return boundBlock;
		}

		private BoundBlock RewriteTryBlock(BoundTryStatement tryStatement)
		{
			BoundBlock tryBlock = tryStatement.TryBlock;
			BoundBlock boundBlock = (BoundBlock)Visit(tryBlock);
			if (this.get_Instrument((BoundNode)tryStatement) && tryBlock.Syntax is TryBlockSyntax)
			{
				boundBlock = PrependWithPrologue(boundBlock, _instrumenterOpt.CreateTryBlockPrologue(tryStatement));
			}
			return boundBlock;
		}

		public override BoundNode VisitCatchBlock(BoundCatchBlock node)
		{
			BoundExpression exceptionSourceOpt = VisitExpressionNode(node.ExceptionSourceOpt);
			BoundExpression boundExpression = VisitExpressionNode(node.ExceptionFilterOpt);
			BoundBlock boundBlock = (BoundBlock)Visit(node.Body);
			if (this.get_Instrument((BoundNode)node) && node.Syntax is CatchBlockSyntax)
			{
				if (boundExpression != null)
				{
					boundExpression = _instrumenterOpt.InstrumentCatchBlockFilter(node, boundExpression, _currentMethodOrLambda);
				}
				else
				{
					boundBlock = PrependWithPrologue(boundBlock, _instrumenterOpt.CreateCatchBlockPrologue(node));
				}
			}
			BoundExpression errorLineNumberOpt = null;
			if (node.ErrorLineNumberOpt != null)
			{
				errorLineNumberOpt = VisitExpressionNode(node.ErrorLineNumberOpt);
			}
			else if ((object)_currentLineTemporary != null && (object)_currentMethodOrLambda == _topMethod)
			{
				errorLineNumberOpt = new BoundLocal(node.Syntax, _currentLineTemporary, isLValue: false, _currentLineTemporary.Type);
			}
			return node.Update(node.LocalOpt, exceptionSourceOpt, errorLineNumberOpt, boundExpression, boundBlock, node.IsSynthesizedAsyncCatchAll);
		}

		private void ReportErrorsOnCatchBlockHelpers(BoundCatchBlock node)
		{
			WellKnownMember wellKnownMember = ((node.ErrorLineNumberOpt == null) ? WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError : WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError_Int32);
			MethodSymbol memberSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(wellKnownMember);
			ReportMissingOrBadRuntimeHelper(node, wellKnownMember, memberSymbol);
			if (node.ExceptionFilterOpt == null || node.ExceptionFilterOpt.Kind != BoundKind.UnstructuredExceptionHandlingCatchFilter)
			{
				MethodSymbol memberSymbol2 = (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError);
				ReportMissingOrBadRuntimeHelper(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError, memberSymbol2);
			}
		}

		public override BoundNode VisitTupleLiteral(BoundTupleLiteral node)
		{
			return VisitTupleExpression(node);
		}

		public override BoundNode VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
		{
			return VisitTupleExpression(node);
		}

		private BoundNode VisitTupleExpression(BoundTupleExpression node)
		{
			ImmutableArray<BoundExpression> rewrittenArguments = VisitList(node.Arguments);
			return RewriteTupleCreationExpression(node, rewrittenArguments);
		}

		private BoundExpression RewriteTupleCreationExpression(BoundTupleExpression node, ImmutableArray<BoundExpression> rewrittenArguments)
		{
			return MakeTupleCreationExpression(node.Syntax, (NamedTypeSymbol)node.Type, rewrittenArguments);
		}

		private BoundExpression MakeTupleCreationExpression(SyntaxNode syntax, NamedTypeSymbol type, ImmutableArray<BoundExpression> rewrittenArguments)
		{
			NamedTypeSymbol underlyingTupleType = type.TupleUnderlyingType ?? type;
			ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
			TupleTypeSymbol.GetUnderlyingTypeChain(underlyingTupleType, instance);
			try
			{
				NamedTypeSymbol namedTypeSymbol = instance.Pop();
				ImmutableArray<BoundExpression> arguments = ImmutableArray.Create(rewrittenArguments, instance.Count * 7, namedTypeSymbol.Arity);
				MethodSymbol methodSymbol = (MethodSymbol)TupleTypeSymbol.GetWellKnownMemberInType(namedTypeSymbol.OriginalDefinition, TupleTypeSymbol.GetTupleCtor(namedTypeSymbol.Arity), _diagnostics, syntax);
				if ((object)methodSymbol == null)
				{
					return new BoundBadExpression(syntax, LookupResultKind.Empty, ImmutableArray<Symbol>.Empty, rewrittenArguments, type, hasErrors: true);
				}
				MethodSymbol constructorOpt = SymbolExtensions.AsMember(methodSymbol, namedTypeSymbol);
				BoundObjectCreationExpression boundObjectCreationExpression = new BoundObjectCreationExpression(syntax, constructorOpt, arguments, null, namedTypeSymbol);
				if (instance.Count > 0)
				{
					MethodSymbol methodSymbol2 = (MethodSymbol)TupleTypeSymbol.GetWellKnownMemberInType(instance.Peek().OriginalDefinition, TupleTypeSymbol.GetTupleCtor(8), _diagnostics, syntax);
					if ((object)methodSymbol2 == null)
					{
						return new BoundBadExpression(syntax, LookupResultKind.Empty, ImmutableArray<Symbol>.Empty, rewrittenArguments, type, hasErrors: true);
					}
					do
					{
						ImmutableArray<BoundExpression> arguments2 = ImmutableArray.Create(rewrittenArguments, (instance.Count - 1) * 7, 7).Add(boundObjectCreationExpression);
						MethodSymbol methodSymbol3 = SymbolExtensions.AsMember(methodSymbol2, instance.Pop());
						boundObjectCreationExpression = new BoundObjectCreationExpression(syntax, methodSymbol3, arguments2, null, methodSymbol3.ContainingType);
					}
					while (instance.Count > 0);
				}
				return boundObjectCreationExpression.Update(boundObjectCreationExpression.ConstructorOpt, null, boundObjectCreationExpression.Arguments, boundObjectCreationExpression.DefaultArguments, boundObjectCreationExpression.InitializerOpt, type);
			}
			finally
			{
				instance.Free();
			}
		}

		public override BoundNode VisitNullableIsTrueOperator(BoundNullableIsTrueOperator node)
		{
			bool optimizableForConditionalBranch = false;
			BoundExpression boundExpression = VisitExpression(AdjustIfOptimizableForConditionalBranch(node.Operand, out optimizableForConditionalBranch));
			if (optimizableForConditionalBranch && HasValue(boundExpression))
			{
				return NullableValueOrDefault(boundExpression);
			}
			if (_inExpressionLambda)
			{
				return node.Update(boundExpression, node.Type);
			}
			if (HasNoValue(boundExpression))
			{
				return new BoundLiteral(node.Syntax, ConstantValue.False, node.Type);
			}
			return NullableValueOrDefault(boundExpression);
		}

		private static BoundExpression AdjustIfOptimizableForConditionalBranch(BoundExpression operand, out bool optimizableForConditionalBranch)
		{
			optimizableForConditionalBranch = false;
			BoundExpression boundExpression = operand;
			while (true)
			{
				switch (boundExpression.Kind)
				{
				case BoundKind.BinaryOperator:
				{
					BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)boundExpression;
					if ((boundBinaryOperator.OperatorKind & BinaryOperatorKind.IsOperandOfConditionalBranch) != 0)
					{
						BinaryOperatorKind binaryOperatorKind = boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask;
						if (binaryOperatorKind == BinaryOperatorKind.OrElse || binaryOperatorKind == BinaryOperatorKind.AndAlso)
						{
							optimizableForConditionalBranch = true;
							return boundBinaryOperator.Update(boundBinaryOperator.OperatorKind | BinaryOperatorKind.OptimizableForConditionalBranch, boundBinaryOperator.Left, boundBinaryOperator.Right, boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, boundBinaryOperator.Type);
						}
					}
					return operand;
				}
				case BoundKind.Parenthesized:
					break;
				default:
					return operand;
				}
				boundExpression = ((BoundParenthesized)boundExpression).Expression;
			}
		}

		public override BoundNode VisitUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node)
		{
			if (_inExpressionLambda)
			{
				return node.Update(node.OperatorKind, VisitExpression(node.UnderlyingExpression), node.Type);
			}
			if ((node.OperatorKind & UnaryOperatorKind.Lifted) != 0)
			{
				return RewriteLiftedUserDefinedUnaryOperator(node);
			}
			return Visit(node.UnderlyingExpression);
		}

		public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
		{
			if ((node.OperatorKind & UnaryOperatorKind.Lifted) == 0 || _inExpressionLambda)
			{
				BoundNode boundNode = base.VisitUnaryOperator(node);
				if (boundNode.Kind == BoundKind.UnaryOperator)
				{
					boundNode = RewriteUnaryOperator((BoundUnaryOperator)boundNode);
				}
				return boundNode;
			}
			return RewriteLiftedUnaryOperator(node);
		}

		private BoundExpression RewriteUnaryOperator(BoundUnaryOperator node)
		{
			BoundExpression result = node;
			UnaryOperatorKind operatorKind = node.OperatorKind;
			if (!node.HasErrors && (operatorKind & UnaryOperatorKind.Lifted) == 0 && operatorKind != UnaryOperatorKind.Error && !_inExpressionLambda)
			{
				TypeSymbol type = node.Type;
				if (TypeSymbolExtensions.IsObjectType(type))
				{
					result = RewriteObjectUnaryOperator(node);
				}
				else if (TypeSymbolExtensions.IsDecimalType(type))
				{
					result = RewriteDecimalUnaryOperator(node);
				}
			}
			return result;
		}

		private BoundExpression RewriteObjectUnaryOperator(BoundUnaryOperator node)
		{
			BoundExpression result = node;
			WellKnownMember wellKnownMember = (node.OperatorKind & UnaryOperatorKind.Not) switch
			{
				UnaryOperatorKind.Plus => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__PlusObjectObject, 
				UnaryOperatorKind.Minus => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__NegateObjectObject, 
				_ => WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__NotObjectObject, 
			};
			MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(wellKnownMember);
			if (!ReportMissingOrBadRuntimeHelper(node, wellKnownMember, methodSymbol))
			{
				result = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(node.Operand), null, methodSymbol.ReturnType);
			}
			return result;
		}

		private BoundExpression RewriteDecimalUnaryOperator(BoundUnaryOperator node)
		{
			BoundExpression result = node;
			if ((node.OperatorKind & UnaryOperatorKind.Not) == UnaryOperatorKind.Plus)
			{
				result = node.Operand;
			}
			else
			{
				MethodSymbol methodSymbol = (MethodSymbol)ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__NegateDecimal);
				if (!ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_Decimal__NegateDecimal, methodSymbol))
				{
					result = new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create(node.Operand), null, methodSymbol.ReturnType);
				}
			}
			return result;
		}

		private BoundNode RewriteLiftedUnaryOperator(BoundUnaryOperator node)
		{
			BoundExpression boundExpression = VisitExpressionNode(node.Operand);
			if (HasNoValue(boundExpression))
			{
				return NullableNull(boundExpression, node.Type);
			}
			if (HasValue(boundExpression))
			{
				BoundExpression expr = ApplyUnliftedUnaryOp(node, NullableValueOrDefault(boundExpression));
				return WrapInNullable(expr, node.Type);
			}
			SynthesizedLocal temp = null;
			BoundExpression init = null;
			BoundExpression boundExpression2 = CaptureNullableIfNeeded(boundExpression, out temp, out init, doNotCaptureLocals: true);
			BoundExpression expr2 = ApplyUnliftedUnaryOp(node, NullableValueOrDefault(boundExpression2));
			BoundExpression boundExpression3 = MakeTernaryConditionalExpression(node.Syntax, NullableHasValue(boundExpression2), WrapInNullable(expr2, node.Type), boundExpression2);
			if ((object)temp != null)
			{
				boundExpression3 = new BoundSequence(node.Syntax, ImmutableArray.Create((LocalSymbol)temp), ImmutableArray.Create(init), boundExpression3, boundExpression3.Type);
			}
			return boundExpression3;
		}

		private BoundExpression RewriteLiftedUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node)
		{
			BoundExpression boundExpression = VisitExpressionNode(node.Operand);
			BoundCall call = node.Call;
			TypeSymbol type = call.Type;
			BoundExpression boundExpression2 = NullableNull(node.Syntax, type);
			if (HasNoValue(boundExpression))
			{
				return boundExpression2;
			}
			ArrayBuilder<LocalSymbol> temps = null;
			ArrayBuilder<BoundExpression> inits = null;
			bool num = HasValue(boundExpression);
			BoundExpression hasValueExpr = null;
			BoundExpression boundExpression3 = call.Update(arguments: ImmutableArray.Create((!num) ? ProcessNullableOperand(boundExpression, out hasValueExpr, ref temps, ref inits, doNotCaptureLocals: true) : NullableValueOrDefault(boundExpression)), method: call.Method, methodGroupOpt: null, receiverOpt: call.ReceiverOpt, defaultArguments: default(BitVector), constantValueOpt: call.ConstantValueOpt, isLValue: call.IsLValue, suppressObjectClone: call.SuppressObjectClone, type: call.Method.ReturnType);
			if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(boundExpression3.Type, type))
			{
				boundExpression3 = WrapInNullable(boundExpression3, type);
			}
			if (num)
			{
				return boundExpression3;
			}
			BoundExpression condition = hasValueExpr;
			BoundExpression boundExpression4 = MakeTernaryConditionalExpression(node.Syntax, condition, boundExpression3, boundExpression2);
			if (temps != null)
			{
				boundExpression4 = new BoundSequence(node.Syntax, temps.ToImmutableAndFree(), inits.ToImmutableAndFree(), boundExpression4, boundExpression4.Type);
			}
			return boundExpression4;
		}

		private BoundExpression ApplyUnliftedUnaryOp(BoundUnaryOperator originalOperator, BoundExpression operandValue)
		{
			UnaryOperatorKind operatorKind = originalOperator.OperatorKind & ~UnaryOperatorKind.Lifted;
			return RewriteUnaryOperator(new BoundUnaryOperator(originalOperator.Syntax, operatorKind, operandValue, originalOperator.Checked, TypeSymbolExtensions.GetNullableUnderlyingType(originalOperator.Type)));
		}

		public override BoundNode VisitUnstructuredExceptionHandlingStatement(BoundUnstructuredExceptionHandlingStatement node)
		{
			if (!node.TrackLineNumber)
			{
				return RewriteUnstructuredExceptionHandlingStatementIntoBlock(node);
			}
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			NamedTypeSymbol type = syntheticBoundNodeFactory.SpecialType(SpecialType.System_Int32);
			_currentLineTemporary = new SynthesizedLocal(_topMethod, type, SynthesizedLocalKind.OnErrorCurrentLine, (StatementSyntax)syntheticBoundNodeFactory.Syntax);
			BoundBlock boundBlock = ((!node.ContainsOnError && !node.ContainsResume) ? ((BoundBlock)VisitBlock(node.Body)) : RewriteUnstructuredExceptionHandlingStatementIntoBlock(node));
			boundBlock = boundBlock.Update(boundBlock.StatementListSyntax, boundBlock.Locals.IsEmpty ? ImmutableArray.Create(_currentLineTemporary) : boundBlock.Locals.Add(_currentLineTemporary), boundBlock.Statements);
			_currentLineTemporary = null;
			return boundBlock;
		}

		private BoundBlock RewriteUnstructuredExceptionHandlingStatementIntoBlock(BoundUnstructuredExceptionHandlingStatement node)
		{
			ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
			_unstructuredExceptionHandling.Context = node;
			_unstructuredExceptionHandling.ExceptionHandlers = ArrayBuilder<BoundGotoStatement>.GetInstance();
			_unstructuredExceptionHandling.OnErrorResumeNextCount = 0;
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			NamedTypeSymbol type = syntheticBoundNodeFactory.SpecialType(SpecialType.System_Int32);
			NamedTypeSymbol type2 = syntheticBoundNodeFactory.SpecialType(SpecialType.System_Boolean);
			_unstructuredExceptionHandling.ActiveHandlerTemporary = new SynthesizedLocal(_topMethod, type, SynthesizedLocalKind.OnErrorActiveHandler, (StatementSyntax)syntheticBoundNodeFactory.Syntax);
			instance.Add(_unstructuredExceptionHandling.ActiveHandlerTemporary);
			_unstructuredExceptionHandling.ResumeTargetTemporary = new SynthesizedLocal(_topMethod, type, SynthesizedLocalKind.OnErrorResumeTarget, (StatementSyntax)syntheticBoundNodeFactory.Syntax);
			instance.Add(_unstructuredExceptionHandling.ResumeTargetTemporary);
			if (node.ResumeWithoutLabelOpt != null)
			{
				_unstructuredExceptionHandling.CurrentStatementTemporary = new SynthesizedLocal(_topMethod, type, SynthesizedLocalKind.OnErrorCurrentStatement, (StatementSyntax)syntheticBoundNodeFactory.Syntax);
				instance.Add(_unstructuredExceptionHandling.CurrentStatementTemporary);
				_unstructuredExceptionHandling.ResumeNextLabel = new GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_ResumeNext");
				_unstructuredExceptionHandling.ResumeLabel = new GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_Resume");
				_unstructuredExceptionHandling.ResumeTargets = ArrayBuilder<BoundGotoStatement>.GetInstance();
			}
			ArrayBuilder<BoundStatement> instance2 = ArrayBuilder<BoundStatement>.GetInstance();
			instance2.Add((BoundBlock)Visit(node.Body));
			if (this.Instrument)
			{
				instance2.Add(SyntheticBoundNodeFactory.HiddenSequencePoint());
			}
			if ((object)_unstructuredExceptionHandling.CurrentStatementTemporary != null)
			{
				RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, canThrow: false, instance2);
			}
			GeneratedLabelSymbol generatedLabelSymbol = new GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_Done");
			instance2.Add(syntheticBoundNodeFactory.Goto(generatedLabelSymbol));
			GeneratedLabelSymbol generatedLabelSymbol2 = new GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_OnErrorFailure");
			if (node.ResumeWithoutLabelOpt != null)
			{
				GeneratedLabelSymbol generatedLabelSymbol3 = new GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_ResumeSwitchFallThrough");
				BoundGotoStatement[] array = new BoundGotoStatement[1 + _unstructuredExceptionHandling.ResumeTargets.Count - 1 + 1];
				array[0] = syntheticBoundNodeFactory.Goto(generatedLabelSymbol3);
				int num = _unstructuredExceptionHandling.ResumeTargets.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					array[i + 1] = _unstructuredExceptionHandling.ResumeTargets[i];
				}
				instance2.Add(new BoundUnstructuredExceptionResumeSwitch(node.Syntax, syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ResumeTargetTemporary, isLValue: false), syntheticBoundNodeFactory.Label(_unstructuredExceptionHandling.ResumeLabel), syntheticBoundNodeFactory.Label(_unstructuredExceptionHandling.ResumeNextLabel), array.AsImmutableOrNull()));
				instance2.Add(syntheticBoundNodeFactory.Label(generatedLabelSymbol3));
				instance2.Add(syntheticBoundNodeFactory.Goto(generatedLabelSymbol2));
			}
			GeneratedLabelSymbol generatedLabelSymbol4 = new GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_OnError");
			instance2.Add(syntheticBoundNodeFactory.Label(generatedLabelSymbol4));
			instance2.Add(BoundExpressionExtensions.ToStatement(syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ResumeTargetTemporary, isLValue: true), ((object)_unstructuredExceptionHandling.CurrentStatementTemporary == null) ? ((BoundExpression)syntheticBoundNodeFactory.Literal(-1)) : ((BoundExpression)syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.CurrentStatementTemporary, isLValue: false)))));
			GeneratedLabelSymbol generatedLabelSymbol5 = new GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_OnErrorSwitchFallThrough");
			BoundGotoStatement[] array2 = new BoundGotoStatement[2 + _unstructuredExceptionHandling.ExceptionHandlers.Count - 1 + 1];
			array2[0] = syntheticBoundNodeFactory.Goto(generatedLabelSymbol5);
			array2[1] = syntheticBoundNodeFactory.Goto((node.ResumeWithoutLabelOpt != null) ? _unstructuredExceptionHandling.ResumeNextLabel : generatedLabelSymbol5);
			int num2 = _unstructuredExceptionHandling.ExceptionHandlers.Count - 1;
			for (int j = 0; j <= num2; j++)
			{
				array2[2 + j] = _unstructuredExceptionHandling.ExceptionHandlers[j];
			}
			instance2.Add(new BoundUnstructuredExceptionOnErrorSwitch(node.Syntax, (node.ResumeWithoutLabelOpt != null && OptimizationLevelIsDebug) ? ((BoundExpression)syntheticBoundNodeFactory.Conditional(syntheticBoundNodeFactory.Binary(BinaryOperatorKind.GreaterThan, type2, syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ActiveHandlerTemporary, isLValue: false), syntheticBoundNodeFactory.Literal(-2)), syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ActiveHandlerTemporary, isLValue: false), syntheticBoundNodeFactory.Literal(1), type)) : ((BoundExpression)syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ActiveHandlerTemporary, isLValue: false)), array2.AsImmutableOrNull()));
			instance2.Add(syntheticBoundNodeFactory.Label(generatedLabelSymbol5));
			instance2.Add(syntheticBoundNodeFactory.Goto(generatedLabelSymbol2));
			BoundBlock tryBlock = syntheticBoundNodeFactory.Block(instance2.ToImmutable());
			instance2.Clear();
			instance2.Add(RewriteTryStatement(node.Syntax, tryBlock, ImmutableArray.Create(new BoundCatchBlock(node.Syntax, null, null, ((object)_currentLineTemporary != null) ? new BoundLocal(node.Syntax, _currentLineTemporary, isLValue: false, _currentLineTemporary.Type) : null, new BoundUnstructuredExceptionHandlingCatchFilter(node.Syntax, syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ActiveHandlerTemporary, isLValue: false), syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ResumeTargetTemporary, isLValue: false), type2), syntheticBoundNodeFactory.Block(ImmutableArray.Create((BoundStatement)syntheticBoundNodeFactory.Goto(generatedLabelSymbol4))), isSynthesizedAsyncCatchAll: false)), null, null));
			instance2.Add(syntheticBoundNodeFactory.Label(generatedLabelSymbol2));
			MethodSymbol methodSymbol = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__CreateProjectError);
			if ((object)methodSymbol != null)
			{
				instance2.Add(syntheticBoundNodeFactory.Throw(new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray.Create((BoundExpression)syntheticBoundNodeFactory.Literal(-2146828237)), null, methodSymbol.ReturnType)));
			}
			instance2.Add(syntheticBoundNodeFactory.Label(generatedLabelSymbol));
			MethodSymbol methodSymbol2 = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError);
			if ((object)methodSymbol2 != null)
			{
				instance2.Add(RewriteIfStatement(node.Syntax, syntheticBoundNodeFactory.Binary(BinaryOperatorKind.NotEquals, type2, syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ResumeTargetTemporary, isLValue: false), syntheticBoundNodeFactory.Literal(0)), BoundExpressionExtensions.ToStatement(new BoundCall(node.Syntax, methodSymbol2, null, null, ImmutableArray<BoundExpression>.Empty, null, methodSymbol2.ReturnType)), null, null));
			}
			_unstructuredExceptionHandling.Context = null;
			_unstructuredExceptionHandling.ExceptionHandlers.Free();
			_unstructuredExceptionHandling.ExceptionHandlers = null;
			if (_unstructuredExceptionHandling.ResumeTargets != null)
			{
				_unstructuredExceptionHandling.ResumeTargets.Free();
				_unstructuredExceptionHandling.ResumeTargets = null;
			}
			_unstructuredExceptionHandling.ActiveHandlerTemporary = null;
			_unstructuredExceptionHandling.ResumeTargetTemporary = null;
			_unstructuredExceptionHandling.CurrentStatementTemporary = null;
			_unstructuredExceptionHandling.ResumeNextLabel = null;
			_unstructuredExceptionHandling.ResumeLabel = null;
			_unstructuredExceptionHandling.OnErrorResumeNextCount = 0;
			return syntheticBoundNodeFactory.Block(instance.ToImmutableAndFree(), instance2.ToImmutableAndFree());
		}

		public override BoundNode VisitOnErrorStatement(BoundOnErrorStatement node)
		{
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, canThrow: false, instance);
			}
			MethodSymbol methodSymbol = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError);
			if ((object)methodSymbol != null)
			{
				instance.Add(BoundExpressionExtensions.ToStatement(new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray<BoundExpression>.Empty, null, methodSymbol.ReturnType)));
			}
			int value;
			switch (node.OnErrorKind)
			{
			case OnErrorStatementKind.GoToMinusOne:
				instance.Add(BoundExpressionExtensions.ToStatement(syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ResumeTargetTemporary, isLValue: true), syntheticBoundNodeFactory.Literal(0))));
				break;
			case OnErrorStatementKind.GoToLabel:
				value = 2 + _unstructuredExceptionHandling.ExceptionHandlers.Count;
				_unstructuredExceptionHandling.ExceptionHandlers.Add(syntheticBoundNodeFactory.Goto(node.LabelOpt, setWasCompilerGenerated: false));
				goto IL_0138;
			case OnErrorStatementKind.ResumeNext:
				value = ((!OptimizationLevelIsDebug) ? 1 : (-2 - _unstructuredExceptionHandling.OnErrorResumeNextCount));
				_unstructuredExceptionHandling.OnErrorResumeNextCount++;
				goto IL_0138;
			default:
				{
					value = 0;
					goto IL_0138;
				}
				IL_0138:
				instance.Add(BoundExpressionExtensions.ToStatement(syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ActiveHandlerTemporary, isLValue: true), syntheticBoundNodeFactory.Literal(value))));
				break;
			}
			BoundStatement boundStatement = new BoundStatementList(node.Syntax, instance.ToImmutableAndFree());
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentOnErrorStatement(node, boundStatement);
			}
			return boundStatement;
		}

		public override BoundNode VisitResumeStatement(BoundResumeStatement node)
		{
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, node.Syntax, _compilationState, _diagnostics);
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			bool flag = ShouldGenerateUnstructuredExceptionHandlingResumeCode(node);
			if (flag)
			{
				RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, canThrow: true, instance);
			}
			MethodSymbol methodSymbol = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError);
			if ((object)methodSymbol != null)
			{
				instance.Add(BoundExpressionExtensions.ToStatement(new BoundCall(node.Syntax, methodSymbol, null, null, ImmutableArray<BoundExpression>.Empty, null, methodSymbol.ReturnType)));
			}
			MethodSymbol methodSymbol2 = syntheticBoundNodeFactory.WellKnownMember<MethodSymbol>(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__CreateProjectError);
			if ((object)methodSymbol2 != null)
			{
				instance.Add(RewriteIfStatement(node.Syntax, syntheticBoundNodeFactory.Binary(BinaryOperatorKind.Equals, syntheticBoundNodeFactory.SpecialType(SpecialType.System_Boolean), syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ResumeTargetTemporary, isLValue: false), syntheticBoundNodeFactory.Literal(0)), syntheticBoundNodeFactory.Throw(new BoundCall(node.Syntax, methodSymbol2, null, null, ImmutableArray.Create((BoundExpression)syntheticBoundNodeFactory.Literal(-2146828268)), null, methodSymbol2.ReturnType)), null, null));
			}
			switch (node.ResumeKind)
			{
			case ResumeStatementKind.Label:
				instance.Add(BoundExpressionExtensions.ToStatement(syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.ResumeTargetTemporary, isLValue: true), syntheticBoundNodeFactory.Literal(0))));
				if (flag)
				{
					RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, canThrow: false, instance);
				}
				instance.Add(syntheticBoundNodeFactory.Goto(node.LabelOpt, setWasCompilerGenerated: false));
				break;
			case ResumeStatementKind.Next:
				instance.Add(syntheticBoundNodeFactory.Goto(_unstructuredExceptionHandling.ResumeNextLabel));
				break;
			default:
				instance.Add(syntheticBoundNodeFactory.Goto(_unstructuredExceptionHandling.ResumeLabel));
				break;
			}
			BoundStatement boundStatement = new BoundStatementList(node.Syntax, instance.ToImmutableAndFree());
			if (this.get_Instrument((BoundNode)node, (BoundNode)boundStatement))
			{
				boundStatement = _instrumenterOpt.InstrumentResumeStatement(node, boundStatement);
			}
			return boundStatement;
		}

		private BoundLabelStatement AddResumeTargetLabel(SyntaxNode syntax)
		{
			GeneratedUnstructuredExceptionHandlingResumeLabel label = new GeneratedUnstructuredExceptionHandlingResumeLabel(_unstructuredExceptionHandling.Context.ResumeWithoutLabelOpt);
			_unstructuredExceptionHandling.ResumeTargets.Add(new BoundGotoStatement(syntax, label, null));
			return new BoundLabelStatement(syntax, label);
		}

		private void AddResumeTargetLabelAndUpdateCurrentStatementTemporary(SyntaxNode syntax, bool canThrow, ArrayBuilder<BoundStatement> statements)
		{
			statements.Add(AddResumeTargetLabel(syntax));
			if (canThrow)
			{
				SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_topMethod, _currentMethodOrLambda, syntax, _compilationState, _diagnostics);
				statements.Add(BoundExpressionExtensions.ToStatement(syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(_unstructuredExceptionHandling.CurrentStatementTemporary, isLValue: true), syntheticBoundNodeFactory.Literal(_unstructuredExceptionHandling.ResumeTargets.Count))));
			}
		}

		private bool ShouldGenerateUnstructuredExceptionHandlingResumeCode(BoundStatement statement)
		{
			if (statement.WasCompilerGenerated)
			{
				return false;
			}
			if (!InsideValidUnstructuredExceptionHandlingResumeContext())
			{
				return false;
			}
			if (!(statement.Syntax is StatementSyntax) && (statement.Syntax.Parent == null || VisualBasicExtensions.Kind(statement.Syntax.Parent) != SyntaxKind.EraseStatement))
			{
				switch (VisualBasicExtensions.Kind(statement.Syntax))
				{
				case SyntaxKind.ElseIfBlock:
					if (statement.Kind != BoundKind.IfStatement)
					{
						return false;
					}
					break;
				case SyntaxKind.CaseBlock:
				case SyntaxKind.CaseElseBlock:
					if (statement.Kind != BoundKind.CaseBlock)
					{
						return false;
					}
					break;
				case SyntaxKind.ModifiedIdentifier:
					if (statement.Kind != BoundKind.LocalDeclaration || statement.Syntax.Parent == null || VisualBasicExtensions.Kind(statement.Syntax.Parent) != SyntaxKind.VariableDeclarator || statement.Syntax.Parent!.Parent == null || VisualBasicExtensions.Kind(statement.Syntax.Parent!.Parent) != SyntaxKind.LocalDeclarationStatement)
					{
						return false;
					}
					break;
				case SyntaxKind.RedimClause:
					if (statement.Kind != BoundKind.ExpressionStatement || !(statement.Syntax.Parent is ReDimStatementSyntax))
					{
						return false;
					}
					break;
				default:
					return false;
				}
			}
			if (statement.Syntax is DeclarationStatementSyntax)
			{
				return false;
			}
			return true;
		}

		private UnstructuredExceptionHandlingContext LeaveUnstructuredExceptionHandlingContext(BoundNode node)
		{
			UnstructuredExceptionHandlingContext result = default(UnstructuredExceptionHandlingContext);
			result.Context = _unstructuredExceptionHandling.Context;
			_unstructuredExceptionHandling.Context = null;
			return result;
		}

		private void RestoreUnstructuredExceptionHandlingContext(BoundNode node, UnstructuredExceptionHandlingContext saved)
		{
			_unstructuredExceptionHandling.Context = saved.Context;
		}

		private bool InsideValidUnstructuredExceptionHandlingResumeContext()
		{
			if (_unstructuredExceptionHandling.Context != null && (object)_unstructuredExceptionHandling.CurrentStatementTemporary != null)
			{
				return (object)_currentMethodOrLambda == _topMethod;
			}
			return false;
		}

		private bool InsideValidUnstructuredExceptionHandlingOnErrorContext()
		{
			if ((object)_currentMethodOrLambda == _topMethod && _unstructuredExceptionHandling.Context != null)
			{
				return _unstructuredExceptionHandling.Context.ContainsOnError;
			}
			return false;
		}

		private void RegisterUnstructuredExceptionHandlingResumeTarget(SyntaxNode syntax, bool canThrow, ArrayBuilder<BoundStatement> statements)
		{
			AddResumeTargetLabelAndUpdateCurrentStatementTemporary(syntax, canThrow, statements);
		}

		private BoundStatement RegisterUnstructuredExceptionHandlingResumeTarget(SyntaxNode syntax, BoundStatement node, bool canThrow)
		{
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			AddResumeTargetLabelAndUpdateCurrentStatementTemporary(syntax, canThrow, instance);
			instance.Add(node);
			return new BoundStatementList(syntax, instance.ToImmutableAndFree());
		}

		private BoundLabelStatement RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(SyntaxNode syntax)
		{
			return AddResumeTargetLabel(syntax);
		}

		private ImmutableArray<BoundStatement> RegisterUnstructuredExceptionHandlingResumeTarget(SyntaxNode syntax, bool canThrow)
		{
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			AddResumeTargetLabelAndUpdateCurrentStatementTemporary(syntax, canThrow, instance);
			return instance.ToImmutableAndFree();
		}

		public override BoundNode VisitUsingStatement(BoundUsingStatement node)
		{
			UnstructuredExceptionHandlingContext saved = LeaveUnstructuredExceptionHandlingContext(node);
			UsingBlockSyntax usingBlockSyntax = (UsingBlockSyntax)node.Syntax;
			BoundBlock boundBlock = (BoundBlock)Visit(node.Body);
			ImmutableArray<LocalSymbol> locals = node.Locals;
			if (!node.ResourceList.IsDefault)
			{
				for (int i = node.ResourceList.Length - 1; i >= 0; i += -1)
				{
					BoundLocalDeclarationBase boundLocalDeclarationBase = node.ResourceList[i];
					(BoundRValuePlaceholder, BoundExpression, BoundExpression) placeholderInfo;
					if (boundLocalDeclarationBase.Kind == BoundKind.LocalDeclaration)
					{
						BoundLocalDeclaration boundLocalDeclaration = (BoundLocalDeclaration)boundLocalDeclarationBase;
						placeholderInfo = node.UsingInfo.PlaceholderInfo[boundLocalDeclaration.LocalSymbol.Type];
						boundBlock = RewriteSingleUsingToTryFinally(node, i, boundLocalDeclaration.LocalSymbol, boundLocalDeclaration.InitializerOpt, ref placeholderInfo, boundBlock);
						continue;
					}
					BoundAsNewLocalDeclarations boundAsNewLocalDeclarations = (BoundAsNewLocalDeclarations)boundLocalDeclarationBase;
					_ = boundAsNewLocalDeclarations.LocalDeclarations.Length;
					placeholderInfo = node.UsingInfo.PlaceholderInfo[boundAsNewLocalDeclarations.LocalDeclarations.First().LocalSymbol.Type];
					for (int j = boundAsNewLocalDeclarations.LocalDeclarations.Length - 1; j >= 0; j += -1)
					{
						boundBlock = RewriteSingleUsingToTryFinally(node, i, boundAsNewLocalDeclarations.LocalDeclarations[j].LocalSymbol, boundAsNewLocalDeclarations.Initializer, ref placeholderInfo, boundBlock);
					}
				}
			}
			else
			{
				BoundExpression resourceExpressionOpt = node.ResourceExpressionOpt;
				(BoundRValuePlaceholder, BoundExpression, BoundExpression) placeholderInfo = node.UsingInfo.PlaceholderInfo[resourceExpressionOpt.Type];
				LocalSymbol localSymbol = new SynthesizedLocal(_currentMethodOrLambda, resourceExpressionOpt.Type, SynthesizedLocalKind.Using, usingBlockSyntax.UsingStatement);
				boundBlock = RewriteSingleUsingToTryFinally(node, 0, localSymbol, resourceExpressionOpt, ref placeholderInfo, boundBlock);
				locals = locals.Add(localSymbol);
			}
			RestoreUnstructuredExceptionHandlingContext(node, saved);
			ImmutableArray<BoundStatement> immutableArray = boundBlock.Statements;
			if (ShouldGenerateUnstructuredExceptionHandlingResumeCode(node))
			{
				immutableArray = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, canThrow: true).Concat(immutableArray);
			}
			boundBlock = new BoundBlock(node.Syntax, boundBlock.StatementListSyntax, locals, immutableArray);
			BoundStatement boundStatement = null;
			if (this.get_Instrument((BoundNode)node))
			{
				boundStatement = _instrumenterOpt.CreateUsingStatementPrologue(node);
			}
			if (boundStatement != null)
			{
				return new BoundStatementList(node.UsingInfo.UsingStatementSyntax, ImmutableArray.Create(boundStatement, boundBlock));
			}
			return new BoundStatementList(node.UsingInfo.UsingStatementSyntax, ImmutableArray.Create((BoundStatement)boundBlock));
		}

		private BoundBlock RewriteSingleUsingToTryFinally(BoundUsingStatement node, int resourceIndex, LocalSymbol localSymbol, BoundExpression initializationExpression, ref (BoundRValuePlaceholder, BoundExpression, BoundExpression) placeholderInfo, BoundBlock currentBody)
		{
			UsingBlockSyntax usingBlockSyntax = (UsingBlockSyntax)node.Syntax;
			TypeSymbol type = localSymbol.Type;
			BoundLocal boundLocal = new BoundLocal(usingBlockSyntax, localSymbol, isLValue: true, type);
			BoundRValuePlaceholder item = placeholderInfo.Item1;
			BoundExpression item2 = placeholderInfo.Item2;
			BoundExpression item3 = placeholderInfo.Item3;
			AddPlaceholderReplacement(item, boundLocal.MakeRValue());
			BoundBlock tryBlock = (BoundBlock)Concat(currentBody, SyntheticBoundNodeFactory.HiddenSequencePoint());
			BoundStatement boundStatement = BoundExpressionExtensions.ToStatement(new BoundAssignmentOperator(usingBlockSyntax, boundLocal, VisitAndGenerateObjectCloneIfNeeded(initializationExpression, suppressObjectClone: true), suppressObjectClone: true, type));
			bool num = this.get_Instrument((BoundNode)node);
			if (num)
			{
				boundStatement = _instrumenterOpt.InstrumentUsingStatementResourceCapture(node, resourceIndex, boundStatement);
			}
			BoundStatement boundStatement2 = GenerateDisposeCallForForeachAndUsing(usingBlockSyntax, boundLocal, VisitExpressionNode(item3), IsOrInheritsFromOrImplementsIDisposable: true, VisitExpressionNode(item2));
			BoundStatement boundStatement3 = null;
			if (num)
			{
				boundStatement3 = _instrumenterOpt.CreateUsingStatementDisposePrologue(node);
			}
			BoundBlock finallyBlockOpt = new BoundBlock(statements: (boundStatement3 == null) ? ImmutableArray.Create(boundStatement2) : ImmutableArray.Create(boundStatement3, boundStatement2), syntax: usingBlockSyntax, statementListSyntax: default(SyntaxList<StatementSyntax>), locals: ImmutableArray<LocalSymbol>.Empty);
			BoundStatement item4 = RewriteTryStatement(usingBlockSyntax, tryBlock, ImmutableArray<BoundCatchBlock>.Empty, finallyBlockOpt, null);
			tryBlock = new BoundBlock(usingBlockSyntax, default(SyntaxList<StatementSyntax>), ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(boundStatement, item4));
			RemovePlaceholderReplacement(item);
			return tryBlock;
		}

		public override BoundNode VisitWhileStatement(BoundWhileStatement node)
		{
			bool num = ShouldGenerateUnstructuredExceptionHandlingResumeCode(node);
			BoundLabelStatement loopResumeLabelOpt = null;
			ImmutableArray<BoundStatement> conditionResumeTargetOpt = default(ImmutableArray<BoundStatement>);
			if (num)
			{
				loopResumeLabelOpt = RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax);
				conditionResumeTargetOpt = RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, canThrow: true);
			}
			BoundStatement rewrittenBody = (BoundStatement)Visit(node.Body);
			BoundLabelStatement afterBodyResumeTargetOpt = null;
			if (num)
			{
				afterBodyResumeTargetOpt = RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax);
			}
			return RewriteWhileStatement(node, VisitExpressionNode(node.Condition), rewrittenBody, node.ContinueLabel, node.ExitLabel, loopIfTrue: true, loopResumeLabelOpt, conditionResumeTargetOpt, afterBodyResumeTargetOpt);
		}

		protected BoundNode RewriteWhileStatement(BoundStatement statement, BoundExpression rewrittenCondition, BoundStatement rewrittenBody, LabelSymbol continueLabel, LabelSymbol exitLabel, bool loopIfTrue = true, BoundLabelStatement loopResumeLabelOpt = null, ImmutableArray<BoundStatement> conditionResumeTargetOpt = default(ImmutableArray<BoundStatement>), BoundStatement afterBodyResumeTargetOpt = null)
		{
			LabelSymbol label = GenerateLabel("start");
			SyntaxNode syntax = statement.Syntax;
			bool flag = this.get_Instrument((BoundNode)statement);
			if (flag)
			{
				switch (statement.Kind)
				{
				case BoundKind.WhileStatement:
					afterBodyResumeTargetOpt = _instrumenterOpt.InstrumentWhileEpilogue((BoundWhileStatement)statement, afterBodyResumeTargetOpt);
					break;
				case BoundKind.DoLoopStatement:
					afterBodyResumeTargetOpt = _instrumenterOpt.InstrumentDoLoopEpilogue((BoundDoLoopStatement)statement, afterBodyResumeTargetOpt);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(statement.Kind);
				case BoundKind.ForEachStatement:
					break;
				}
			}
			rewrittenBody = Concat(rewrittenBody, afterBodyResumeTargetOpt);
			if (rewrittenCondition != null && flag)
			{
				rewrittenCondition = statement.Kind switch
				{
					BoundKind.WhileStatement => _instrumenterOpt.InstrumentWhileStatementCondition((BoundWhileStatement)statement, rewrittenCondition, _currentMethodOrLambda), 
					BoundKind.DoLoopStatement => _instrumenterOpt.InstrumentDoLoopStatementCondition((BoundDoLoopStatement)statement, rewrittenCondition, _currentMethodOrLambda), 
					BoundKind.ForEachStatement => _instrumenterOpt.InstrumentForEachStatementCondition((BoundForEachStatement)statement, rewrittenCondition, _currentMethodOrLambda), 
					_ => throw ExceptionUtilities.UnexpectedValue(statement.Kind), 
				};
			}
			BoundStatement boundStatement = new BoundConditionalGoto(syntax, rewrittenCondition, loopIfTrue, label);
			if (!conditionResumeTargetOpt.IsDefaultOrEmpty)
			{
				boundStatement = new BoundStatementList(boundStatement.Syntax, conditionResumeTargetOpt.Add(boundStatement));
			}
			if (flag)
			{
				boundStatement = statement.Kind switch
				{
					BoundKind.WhileStatement => _instrumenterOpt.InstrumentWhileStatementConditionalGotoStart((BoundWhileStatement)statement, boundStatement), 
					BoundKind.DoLoopStatement => _instrumenterOpt.InstrumentDoLoopStatementEntryOrConditionalGotoStart((BoundDoLoopStatement)statement, boundStatement), 
					BoundKind.ForEachStatement => _instrumenterOpt.InstrumentForEachStatementConditionalGotoStart((BoundForEachStatement)statement, boundStatement), 
					_ => throw ExceptionUtilities.UnexpectedValue(statement.Kind), 
				};
			}
			BoundStatement boundStatement2 = new BoundGotoStatement(syntax, continueLabel, null);
			if (loopResumeLabelOpt != null)
			{
				boundStatement2 = Concat(loopResumeLabelOpt, boundStatement2);
			}
			if (flag)
			{
				boundStatement2 = SyntheticBoundNodeFactory.HiddenSequencePoint(boundStatement2);
			}
			return new BoundStatementList(syntax, ImmutableArray.Create<BoundStatement>(boundStatement2, new BoundLabelStatement(syntax, label), rewrittenBody, new BoundLabelStatement(syntax, continueLabel), boundStatement, new BoundLabelStatement(syntax, exitLabel)));
		}

		public override BoundNode VisitWithStatement(BoundWithStatement node)
		{
			if (node.HasErrors)
			{
				return node;
			}
			UnstructuredExceptionHandlingContext saved = LeaveUnstructuredExceptionHandlingContext(node);
			BoundExpression boundExpression = VisitExpressionNode(node.OriginalExpression);
			_ = boundExpression.Type;
			WithStatementSyntax withStatement = ((WithBlockSyntax)node.Syntax).WithStatement;
			bool doNotUseByRefLocal = _currentMethodOrLambda.IsIterator || _currentMethodOrLambda.IsAsync || node.Binder.ExpressionIsAccessedFromNestedLambda;
			WithExpressionRewriter.Result result = new WithExpressionRewriter(withStatement).AnalyzeWithExpression(_currentMethodOrLambda, boundExpression, doNotUseByRefLocal, null);
			RestoreUnstructuredExceptionHandlingContext(node, saved);
			return RewriteWithBlockStatements(node, ShouldGenerateUnstructuredExceptionHandlingResumeCode(node), result.Locals, result.Initializers, node.ExpressionPlaceholder, result.Expression);
		}

		private BoundBlock RewriteWithBlockStatements(BoundWithStatement node, bool generateUnstructuredExceptionHandlingResumeCode, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> initializers, BoundValuePlaceholderBase placeholder, BoundExpression replaceWith)
		{
			BoundBlock body = node.Body;
			SyntaxNode syntax = node.Syntax;
			ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
			bool flag = this.get_Instrument((BoundNode)node) && VisualBasicExtensions.Kind(syntax) == SyntaxKind.WithBlock;
			if (flag)
			{
				BoundStatement boundStatement = _instrumenterOpt.CreateWithStatementPrologue(node);
				if (boundStatement != null)
				{
					instance.Add(boundStatement);
				}
			}
			if (generateUnstructuredExceptionHandlingResumeCode)
			{
				RegisterUnstructuredExceptionHandlingResumeTarget(syntax, canThrow: true, instance);
			}
			ImmutableArray<BoundExpression>.Enumerator enumerator = initializers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				instance.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(syntax, current)));
			}
			AddPlaceholderReplacement(placeholder, replaceWith);
			instance.Add((BoundStatement)Visit(body));
			RemovePlaceholderReplacement(placeholder);
			if (flag)
			{
				BoundStatement boundStatement2 = _instrumenterOpt.CreateWithStatementEpilogue(node);
				if (boundStatement2 != null)
				{
					instance.Add(boundStatement2);
				}
			}
			if (generateUnstructuredExceptionHandlingResumeCode)
			{
				instance.Add(RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(syntax));
			}
			if (!node.Binder.ExpressionIsAccessedFromNestedLambda)
			{
				ImmutableArray<LocalSymbol>.Enumerator enumerator2 = locals.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					LocalSymbol current2 = enumerator2.Current;
					TypeSymbol type = current2.Type;
					if (!current2.IsByRef && LocalOrFieldNeedsToBeCleanedUp(type))
					{
						instance.Add(BoundNodeExtensions.MakeCompilerGenerated(new BoundExpressionStatement(syntax, VisitExpression(BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(syntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundLocal(syntax, current2, isLValue: true, type)), BoundNodeExtensions.MakeCompilerGenerated(new BoundConversion(syntax, BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Nothing, null)), ConversionKind.WideningNothingLiteral, @checked: false, explicitCastInCode: false, type)), suppressObjectClone: true, type))))));
					}
				}
			}
			return new BoundBlock(syntax, default(SyntaxList<StatementSyntax>), locals, instance.ToImmutableAndFree());
		}

		private bool LocalOrFieldNeedsToBeCleanedUp(TypeSymbol currentType)
		{
			if (currentType.IsReferenceType || TypeSymbolExtensions.IsTypeParameter(currentType))
			{
				return true;
			}
			if (TypeSymbolExtensions.IsIntrinsicOrEnumType(currentType))
			{
				return false;
			}
			if (_valueTypesCleanUpCache.TryGetValue(currentType, out var value))
			{
				return value;
			}
			_valueTypesCleanUpCache[currentType] = false;
			ImmutableArray<Symbol>.Enumerator enumerator = currentType.GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind == SymbolKind.Field)
				{
					TypeSymbol type = ((FieldSymbol)current).Type;
					if ((object)type != currentType && LocalOrFieldNeedsToBeCleanedUp(type))
					{
						_valueTypesCleanUpCache[currentType] = true;
						return true;
					}
				}
			}
			return false;
		}

		public override BoundNode VisitWithLValueExpressionPlaceholder(BoundWithLValueExpressionPlaceholder node)
		{
			return this.get_PlaceholderReplacement((BoundValuePlaceholderBase)node);
		}

		public override BoundNode VisitWithRValueExpressionPlaceholder(BoundWithRValueExpressionPlaceholder node)
		{
			return this.get_PlaceholderReplacement((BoundValuePlaceholderBase)node);
		}

		public override BoundNode VisitXmlComment(BoundXmlComment node)
		{
			return Visit(node.ObjectCreation);
		}

		public override BoundNode VisitXmlDocument(BoundXmlDocument node)
		{
			if (_inExpressionLambda && !node.HasErrors)
			{
				return VisitXmlContainerInExpressionLambda(node.RewriterInfo);
			}
			return VisitXmlContainer(node.RewriterInfo);
		}

		public override BoundNode VisitXmlDeclaration(BoundXmlDeclaration node)
		{
			return Visit(node.ObjectCreation);
		}

		public override BoundNode VisitXmlProcessingInstruction(BoundXmlProcessingInstruction node)
		{
			return Visit(node.ObjectCreation);
		}

		public override BoundNode VisitXmlAttribute(BoundXmlAttribute node)
		{
			return Visit(node.ObjectCreation);
		}

		public override BoundNode VisitXmlElement(BoundXmlElement node)
		{
			BoundXmlContainerRewriterInfo rewriterInfo = node.RewriterInfo;
			ImmutableArray<KeyValuePair<string, string>> xmlImportedNamespaces = _xmlImportedNamespaces;
			if (rewriterInfo.IsRoot)
			{
				_xmlImportedNamespaces = rewriterInfo.ImportedNamespaces;
			}
			BoundNode result = ((!_inExpressionLambda || node.HasErrors) ? VisitXmlContainer(rewriterInfo) : VisitXmlContainerInExpressionLambda(rewriterInfo));
			if (rewriterInfo.IsRoot)
			{
				_xmlImportedNamespaces = xmlImportedNamespaces;
			}
			return result;
		}

		public override BoundNode VisitXmlEmbeddedExpression(BoundXmlEmbeddedExpression node)
		{
			return Visit(node.Expression);
		}

		public override BoundNode VisitXmlMemberAccess(BoundXmlMemberAccess node)
		{
			return Visit(node.MemberAccess);
		}

		public override BoundNode VisitXmlName(BoundXmlName node)
		{
			return Visit(node.ObjectCreation);
		}

		public override BoundNode VisitXmlNamespace(BoundXmlNamespace node)
		{
			return Visit(node.ObjectCreation);
		}

		public override BoundNode VisitXmlCData(BoundXmlCData node)
		{
			return Visit(node.ObjectCreation);
		}

		private BoundExpression VisitXmlContainer(BoundXmlContainerRewriterInfo rewriterInfo)
		{
			BoundExpression boundExpression = VisitExpressionNode(rewriterInfo.ObjectCreation);
			if (rewriterInfo.SideEffects.Length == 0)
			{
				return boundExpression;
			}
			SyntaxNode syntax = boundExpression.Syntax;
			TypeSymbol type = boundExpression.Type;
			ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
			ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
			BoundLocal boundLocal = CreateTempLocal(syntax, type, boundExpression, instance2);
			instance.Add(boundLocal.LocalSymbol);
			AddPlaceholderReplacement(rewriterInfo.Placeholder, boundLocal);
			BoundLocal boundLocal2 = null;
			if (rewriterInfo.XmlnsAttributesPlaceholder != null)
			{
				boundLocal2 = CreateTempLocal(syntax, rewriterInfo.XmlnsAttributesPlaceholder.Type, VisitExpressionNode(rewriterInfo.XmlnsAttributes), instance2);
				instance.Add(boundLocal2.LocalSymbol);
				AddPlaceholderReplacement(rewriterInfo.XmlnsAttributesPlaceholder, boundLocal2);
			}
			if (rewriterInfo.PrefixesPlaceholder != null)
			{
				BoundExpression prefixes = null;
				BoundExpression namespaces = null;
				CreatePrefixesAndNamespacesArrays(rewriterInfo, syntax, out prefixes, out namespaces);
				BoundLocal boundLocal3 = CreateTempLocal(syntax, rewriterInfo.PrefixesPlaceholder.Type, prefixes, instance2);
				instance.Add(boundLocal3.LocalSymbol);
				AddPlaceholderReplacement(rewriterInfo.PrefixesPlaceholder, boundLocal3);
				BoundLocal boundLocal4 = CreateTempLocal(syntax, rewriterInfo.NamespacesPlaceholder.Type, namespaces, instance2);
				instance.Add(boundLocal4.LocalSymbol);
				AddPlaceholderReplacement(rewriterInfo.NamespacesPlaceholder, boundLocal4);
			}
			VisitList(rewriterInfo.SideEffects, instance2);
			if (rewriterInfo.PrefixesPlaceholder != null)
			{
				RemovePlaceholderReplacement(rewriterInfo.PrefixesPlaceholder);
				RemovePlaceholderReplacement(rewriterInfo.NamespacesPlaceholder);
			}
			if (rewriterInfo.XmlnsAttributesPlaceholder != null)
			{
				RemovePlaceholderReplacement(rewriterInfo.XmlnsAttributesPlaceholder);
			}
			RemovePlaceholderReplacement(rewriterInfo.Placeholder);
			return new BoundSequence(syntax, instance.ToImmutableAndFree(), instance2.ToImmutableAndFree(), boundLocal, type);
		}

		private BoundLocal CreateTempLocal(SyntaxNode syntax, TypeSymbol type, BoundExpression expr, ArrayBuilder<BoundExpression> sideEffects)
		{
			BoundLocal boundLocal = new BoundLocal(syntax, new SynthesizedLocal(_currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp), type);
			sideEffects.Add(new BoundAssignmentOperator(syntax, boundLocal, expr, suppressObjectClone: true, type));
			return boundLocal.MakeRValue();
		}

		private BoundExpression VisitXmlContainerInExpressionLambda(BoundXmlContainerRewriterInfo rewriterInfo)
		{
			ImmutableArray<BoundExpression> sideEffects = rewriterInfo.SideEffects;
			BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)rewriterInfo.ObjectCreation;
			if (sideEffects.Length == 0)
			{
				return VisitExpressionNode(boundObjectCreationExpression);
			}
			BoundExpression node = boundObjectCreationExpression.Arguments[0];
			MethodSymbol methodSymbol = null;
			if (boundObjectCreationExpression.Arguments.Length == 1)
			{
				methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember((sideEffects.Length == 1) ? WellKnownMember.System_Xml_Linq_XElement__ctor : WellKnownMember.System_Xml_Linq_XElement__ctor2);
				if (ReportMissingOrBadRuntimeHelper(boundObjectCreationExpression, WellKnownMember.System_Xml_Linq_XElement__ctor2, methodSymbol))
				{
					return VisitExpressionNode(boundObjectCreationExpression);
				}
			}
			else
			{
				methodSymbol = boundObjectCreationExpression.ConstructorOpt;
			}
			SyntaxNode syntax = boundObjectCreationExpression.Syntax;
			_ = boundObjectCreationExpression.Type;
			BoundLocal boundLocal = null;
			if (rewriterInfo.XmlnsAttributesPlaceholder != null)
			{
				boundLocal = CreateTempLocalInExpressionLambda(syntax, rewriterInfo.XmlnsAttributesPlaceholder.Type, VisitExpressionNode(rewriterInfo.XmlnsAttributes));
				AddPlaceholderReplacement(rewriterInfo.XmlnsAttributesPlaceholder, boundLocal);
			}
			if (rewriterInfo.PrefixesPlaceholder != null)
			{
				BoundExpression prefixes = null;
				BoundExpression namespaces = null;
				CreatePrefixesAndNamespacesArrays(rewriterInfo, syntax, out prefixes, out namespaces);
				BoundLocal value = CreateTempLocalInExpressionLambda(syntax, rewriterInfo.PrefixesPlaceholder.Type, prefixes);
				AddPlaceholderReplacement(rewriterInfo.PrefixesPlaceholder, value);
				BoundLocal value2 = CreateTempLocalInExpressionLambda(syntax, rewriterInfo.NamespacesPlaceholder.Type, namespaces);
				AddPlaceholderReplacement(rewriterInfo.NamespacesPlaceholder, value2);
			}
			BoundExpression[] array = new BoundExpression[sideEffects.Length - 1 + 1];
			int num = sideEffects.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				BoundCall boundCall = (BoundCall)sideEffects[i];
				array[i] = VisitExpressionNode(boundCall.Arguments[0]);
			}
			if (rewriterInfo.PrefixesPlaceholder != null)
			{
				RemovePlaceholderReplacement(rewriterInfo.PrefixesPlaceholder);
				RemovePlaceholderReplacement(rewriterInfo.NamespacesPlaceholder);
			}
			if (rewriterInfo.XmlnsAttributesPlaceholder != null)
			{
				RemovePlaceholderReplacement(rewriterInfo.XmlnsAttributesPlaceholder);
			}
			TypeSymbol type = methodSymbol.Parameters[1].Type;
			BoundExpression item;
			if (TypeSymbolExtensions.IsArrayType(type))
			{
				ArrayTypeSymbol type2 = (ArrayTypeSymbol)type;
				item = new BoundArrayCreation(boundObjectCreationExpression.Syntax, ImmutableArray.Create((BoundExpression)new BoundLiteral(boundObjectCreationExpression.Syntax, ConstantValue.Create(array.Length), GetSpecialType(SpecialType.System_Int32))), new BoundArrayInitialization(boundObjectCreationExpression.Syntax, array.AsImmutableOrNull(), type2), type2);
			}
			else
			{
				item = array[0];
			}
			return boundObjectCreationExpression.Update(methodSymbol, ImmutableArray.Create(VisitExpression(node), item), default(BitVector), null, boundObjectCreationExpression.Type);
		}

		private BoundLocal CreateTempLocalInExpressionLambda(SyntaxNode syntax, TypeSymbol type, BoundExpression expr)
		{
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(_topMethod, type, SynthesizedLocalKind.XmlInExpressionLambda, _currentMethodOrLambda.Syntax);
			BoundLocal boundLocal = new BoundLocal(syntax, synthesizedLocal, type);
			_xmlFixupData.AddLocal(synthesizedLocal, new BoundAssignmentOperator(syntax, boundLocal, expr, suppressObjectClone: true, type));
			return boundLocal.MakeRValue();
		}

		private void CreatePrefixesAndNamespacesArrays(BoundXmlContainerRewriterInfo rewriterInfo, SyntaxNode syntax, out BoundExpression prefixes, out BoundExpression namespaces)
		{
			ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
			ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
			ImmutableArray<KeyValuePair<string, string>>.Enumerator enumerator = _xmlImportedNamespaces.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, string> current = enumerator.Current;
				instance.Add(CreateCompilerGeneratedXmlnsPrefix(syntax, current.Key));
				instance2.Add(CreateCompilerGeneratedXmlNamespace(syntax, current.Value));
			}
			ImmutableArray<KeyValuePair<string, string>>.Enumerator enumerator2 = rewriterInfo.InScopeXmlNamespaces.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				KeyValuePair<string, string> current2 = enumerator2.Current;
				instance.Add(CreateCompilerGeneratedXmlnsPrefix(syntax, current2.Key));
				instance2.Add(CreateCompilerGeneratedXmlNamespace(syntax, current2.Value));
			}
			prefixes = VisitExpressionNode(CreateCompilerGeneratedArray(syntax, rewriterInfo.PrefixesPlaceholder.Type, instance.ToImmutableAndFree()));
			namespaces = VisitExpressionNode(CreateCompilerGeneratedArray(syntax, rewriterInfo.NamespacesPlaceholder.Type, instance2.ToImmutableAndFree()));
		}

		private BoundExpression BindXmlNamespace(SyntaxNode syntax, BoundExpression @namespace)
		{
			MethodSymbol methodSymbol = (MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_Xml_Linq_XNamespace__Get);
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundCall(syntax, methodSymbol, null, null, ImmutableArray.Create(@namespace), null, methodSymbol.ReturnType));
		}

		private BoundLiteral CreateStringLiteral(SyntaxNode syntax, string str)
		{
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Create(str), GetSpecialType(SpecialType.System_String)));
		}

		private BoundExpression CreateCompilerGeneratedXmlnsPrefix(SyntaxNode syntax, string prefix)
		{
			return CreateStringLiteral(syntax, (EmbeddedOperators.CompareString(prefix, "", TextCompare: false) == 0) ? "xmlns" : prefix);
		}

		private BoundExpression CreateCompilerGeneratedXmlNamespace(SyntaxNode syntax, string @namespace)
		{
			return BindXmlNamespace(syntax, CreateStringLiteral(syntax, @namespace));
		}

		private BoundExpression CreateCompilerGeneratedArray(SyntaxNode syntax, TypeSymbol arrayType, ImmutableArray<BoundExpression> items)
		{
			BoundExpression boundExpression;
			if (items.Length == 0)
			{
				boundExpression = new BoundLiteral(syntax, ConstantValue.Nothing, arrayType);
			}
			else
			{
				BoundLiteral item = BoundNodeExtensions.MakeCompilerGenerated(new BoundLiteral(syntax, ConstantValue.Create(items.Length), GetSpecialType(SpecialType.System_Int32)));
				BoundArrayInitialization initializerOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundArrayInitialization(syntax, items, arrayType));
				boundExpression = new BoundArrayCreation(syntax, ImmutableArray.Create((BoundExpression)item), initializerOpt, arrayType);
			}
			boundExpression.SetWasCompilerGenerated();
			return boundExpression;
		}
	}
}

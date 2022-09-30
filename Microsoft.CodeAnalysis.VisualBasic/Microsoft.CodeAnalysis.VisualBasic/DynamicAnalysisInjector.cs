using System.Collections.Immutable;
using System.Linq;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class DynamicAnalysisInjector : CompoundInstrumenter
	{
		private readonly MethodSymbol _method;

		private readonly BoundStatement _methodBody;

		private readonly MethodSymbol _createPayloadForMethodsSpanningSingleFile;

		private readonly MethodSymbol _createPayloadForMethodsSpanningMultipleFiles;

		private readonly ArrayBuilder<SourceSpan> _spansBuilder;

		private ImmutableArray<SourceSpan> _dynamicAnalysisSpans;

		private readonly BoundStatement _methodEntryInstrumentation;

		private readonly ArrayTypeSymbol _payloadType;

		private readonly LocalSymbol _methodPayload;

		private readonly BindingDiagnosticBag _diagnostics;

		private readonly DebugDocumentProvider _debugDocumentProvider;

		private readonly SyntheticBoundNodeFactory _methodBodyFactory;

		public ImmutableArray<SourceSpan> DynamicAnalysisSpans => _dynamicAnalysisSpans;

		public static DynamicAnalysisInjector TryCreate(MethodSymbol method, BoundStatement methodBody, SyntheticBoundNodeFactory methodBodyFactory, BindingDiagnosticBag diagnostics, DebugDocumentProvider debugDocumentProvider, Instrumenter previous)
		{
			if (method.IsImplicitlyDeclared && !SymbolExtensions.IsAnyConstructor(method))
			{
				return null;
			}
			if (!HasValidMappedLineSpan(methodBody.Syntax))
			{
				return null;
			}
			if (IsExcludedFromCodeCoverage(method))
			{
				return null;
			}
			MethodSymbol createPayloadOverload = GetCreatePayloadOverload(methodBodyFactory.Compilation, WellKnownMember.Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningSingleFile, methodBody.Syntax, diagnostics);
			MethodSymbol createPayloadOverload2 = GetCreatePayloadOverload(methodBodyFactory.Compilation, WellKnownMember.Microsoft_CodeAnalysis_Runtime_Instrumentation__CreatePayloadForMethodsSpanningMultipleFiles, methodBody.Syntax, diagnostics);
			if ((object)createPayloadOverload == null || (object)createPayloadOverload2 == null)
			{
				return null;
			}
			if (method.Equals(createPayloadOverload) || method.Equals(createPayloadOverload2))
			{
				return null;
			}
			return new DynamicAnalysisInjector(method, methodBody, methodBodyFactory, createPayloadOverload, createPayloadOverload2, diagnostics, debugDocumentProvider, previous);
		}

		private static bool HasValidMappedLineSpan(SyntaxNode syntax)
		{
			return syntax.GetLocation().GetMappedLineSpan().IsValid;
		}

		private DynamicAnalysisInjector(MethodSymbol method, BoundStatement methodBody, SyntheticBoundNodeFactory methodBodyFactory, MethodSymbol createPayloadForMethodsSpanningSingleFile, MethodSymbol createPayloadForMethodsSpanningMultipleFiles, BindingDiagnosticBag diagnostics, DebugDocumentProvider debugDocumentProvider, Instrumenter previous)
			: base(previous)
		{
			_dynamicAnalysisSpans = ImmutableArray<SourceSpan>.Empty;
			_createPayloadForMethodsSpanningSingleFile = createPayloadForMethodsSpanningSingleFile;
			_createPayloadForMethodsSpanningMultipleFiles = createPayloadForMethodsSpanningMultipleFiles;
			_method = method;
			_methodBody = methodBody;
			_spansBuilder = ArrayBuilder<SourceSpan>.GetInstance();
			TypeSymbol elementType = methodBodyFactory.SpecialType(SpecialType.System_Boolean);
			_payloadType = ArrayTypeSymbol.CreateVBArray(elementType, ImmutableArray<CustomModifier>.Empty, 1, methodBodyFactory.Compilation.Assembly);
			_methodPayload = methodBodyFactory.SynthesizedLocal(_payloadType, SynthesizedLocalKind.InstrumentationPayload, methodBody.Syntax);
			_diagnostics = diagnostics;
			_debugDocumentProvider = debugDocumentProvider;
			_methodBodyFactory = methodBodyFactory;
			SyntaxNode syntax = methodBody.Syntax;
			if (!method.IsImplicitlyDeclared)
			{
				_methodEntryInstrumentation = AddAnalysisPoint(syntax, SkipAttributes(syntax), methodBodyFactory);
			}
		}

		private static bool IsExcludedFromCodeCoverage(MethodSymbol method)
		{
			NamedTypeSymbol containingType = method.ContainingType;
			while ((object)containingType != null)
			{
				if (containingType.IsDirectlyExcludedFromCodeCoverage)
				{
					return true;
				}
				containingType = containingType.ContainingType;
			}
			Symbol symbol = SymbolExtensions.ContainingNonLambdaMember(method);
			if ((object)symbol != null && symbol.Kind == SymbolKind.Method)
			{
				method = (MethodSymbol)symbol;
				if (method.IsDirectlyExcludedFromCodeCoverage)
				{
					return true;
				}
				Symbol associatedSymbol = method.AssociatedSymbol;
				SymbolKind? symbolKind = associatedSymbol?.Kind;
				int? num = (int?)symbolKind;
				if ((num.HasValue ? new bool?(num.GetValueOrDefault() == 15) : null).GetValueOrDefault())
				{
					if (((PropertySymbol)associatedSymbol).IsDirectlyExcludedFromCodeCoverage)
					{
						return true;
					}
				}
				else
				{
					num = (int?)symbolKind;
					if ((num.HasValue ? new bool?(num.GetValueOrDefault() == 5) : null).GetValueOrDefault() && ((EventSymbol)associatedSymbol).IsDirectlyExcludedFromCodeCoverage)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static BoundExpressionStatement GetCreatePayloadStatement(ImmutableArray<SourceSpan> dynamicAnalysisSpans, SyntaxNode methodBodySyntax, LocalSymbol methodPayload, MethodSymbol createPayloadForMethodsSpanningSingleFile, MethodSymbol createPayloadForMethodsSpanningMultipleFiles, BoundExpression mvid, BoundExpression methodToken, BoundExpression payloadSlot, SyntheticBoundNodeFactory methodBodyFactory, DebugDocumentProvider debugDocumentProvider)
		{
			MethodSymbol method;
			BoundExpression boundExpression;
			if (dynamicAnalysisSpans.IsEmpty)
			{
				method = createPayloadForMethodsSpanningSingleFile;
				DebugSourceDocument sourceDocument = GetSourceDocument(debugDocumentProvider, methodBodySyntax);
				boundExpression = methodBodyFactory.SourceDocumentIndex(sourceDocument);
			}
			else
			{
				PooledHashSet<DebugSourceDocument> instance = PooledHashSet<DebugSourceDocument>.GetInstance();
				ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
				ImmutableArray<SourceSpan>.Enumerator enumerator = dynamicAnalysisSpans.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DebugSourceDocument document = enumerator.Current.Document;
					if (instance.Add(document))
					{
						instance2.Add(methodBodyFactory.SourceDocumentIndex(document));
					}
				}
				instance.Free();
				if (instance2.Count == 1)
				{
					method = createPayloadForMethodsSpanningSingleFile;
					boundExpression = instance2.Single();
				}
				else
				{
					method = createPayloadForMethodsSpanningMultipleFiles;
					boundExpression = methodBodyFactory.Array(methodBodyFactory.SpecialType(SpecialType.System_Int32), instance2.ToImmutable());
				}
				instance2.Free();
			}
			return methodBodyFactory.Assignment(methodBodyFactory.Local(methodPayload, isLValue: true), methodBodyFactory.Call(null, method, mvid, methodToken, boundExpression, payloadSlot, methodBodyFactory.Literal(dynamicAnalysisSpans.Length)));
		}

		public override BoundStatement CreateBlockPrologue(BoundBlock trueOriginal, BoundBlock original, ref LocalSymbol synthesizedLocal)
		{
			BoundStatement boundStatement = base.CreateBlockPrologue(trueOriginal, original, ref synthesizedLocal);
			if (_methodBody == trueOriginal)
			{
				_dynamicAnalysisSpans = _spansBuilder.ToImmutableAndFree();
				ArrayTypeSymbol payloadType = ArrayTypeSymbol.CreateVBArray(_payloadType, ImmutableArray<CustomModifier>.Empty, 1, _methodBodyFactory.Compilation.Assembly);
				BoundStatement item = _methodBodyFactory.Assignment(_methodBodyFactory.Local(_methodPayload, isLValue: true), _methodBodyFactory.ArrayAccess(_methodBodyFactory.InstrumentationPayloadRoot(0, payloadType, isLValue: false), isLValue: false, ImmutableArray.Create(_methodBodyFactory.MethodDefIndex(_method))));
				BoundExpression mvid = _methodBodyFactory.ModuleVersionId(isLValue: false);
				BoundExpression methodToken = _methodBodyFactory.MethodDefIndex(_method);
				BoundExpression payloadSlot = _methodBodyFactory.ArrayAccess(_methodBodyFactory.InstrumentationPayloadRoot(0, payloadType, isLValue: false), isLValue: true, ImmutableArray.Create(_methodBodyFactory.MethodDefIndex(_method)));
				BoundStatement createPayloadStatement = GetCreatePayloadStatement(_dynamicAnalysisSpans, _methodBody.Syntax, _methodPayload, _createPayloadForMethodsSpanningSingleFile, _createPayloadForMethodsSpanningMultipleFiles, mvid, methodToken, payloadSlot, _methodBodyFactory, _debugDocumentProvider);
				BoundExpression condition = _methodBodyFactory.Binary(BinaryOperatorKind.Equals, _methodBodyFactory.SpecialType(SpecialType.System_Boolean), _methodBodyFactory.Local(_methodPayload, isLValue: false), _methodBodyFactory.Null(_payloadType));
				BoundStatement item2 = _methodBodyFactory.If(condition, createPayloadStatement);
				synthesizedLocal = _methodPayload;
				ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance((boundStatement == null) ? 3 : 4);
				instance.Add(item);
				instance.Add(item2);
				if (_methodEntryInstrumentation != null)
				{
					instance.Add(_methodEntryInstrumentation);
				}
				if (boundStatement != null)
				{
					instance.Add(boundStatement);
				}
				return _methodBodyFactory.StatementList(instance.ToImmutableAndFree());
			}
			return boundStatement;
		}

		public override BoundStatement InstrumentExpressionStatement(BoundExpressionStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentExpressionStatement(original, rewritten));
		}

		public override BoundStatement InstrumentStopStatement(BoundStopStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentStopStatement(original, rewritten));
		}

		public override BoundStatement InstrumentEndStatement(BoundEndStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentEndStatement(original, rewritten));
		}

		public override BoundStatement InstrumentContinueStatement(BoundContinueStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentContinueStatement(original, rewritten));
		}

		public override BoundStatement InstrumentExitStatement(BoundExitStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentExitStatement(original, rewritten));
		}

		public override BoundStatement InstrumentGotoStatement(BoundGotoStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentGotoStatement(original, rewritten));
		}

		public override BoundStatement InstrumentRaiseEventStatement(BoundRaiseEventStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentRaiseEventStatement(original, rewritten));
		}

		public override BoundStatement InstrumentReturnStatement(BoundReturnStatement original, BoundStatement rewritten)
		{
			BoundStatement boundStatement = base.InstrumentReturnStatement(original, rewritten);
			if (!original.IsEndOfMethodReturn())
			{
				if (original.ExpressionOpt != null)
				{
					return CollectDynamicAnalysis(original, boundStatement);
				}
				return AddDynamicAnalysis(original, boundStatement);
			}
			return boundStatement;
		}

		public override BoundStatement InstrumentThrowStatement(BoundThrowStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentThrowStatement(original, rewritten));
		}

		public override BoundStatement InstrumentOnErrorStatement(BoundOnErrorStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentOnErrorStatement(original, rewritten));
		}

		public override BoundStatement InstrumentResumeStatement(BoundResumeStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentResumeStatement(original, rewritten));
		}

		public override BoundStatement InstrumentAddHandlerStatement(BoundAddHandlerStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentAddHandlerStatement(original, rewritten));
		}

		public override BoundStatement InstrumentRemoveHandlerStatement(BoundRemoveHandlerStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentRemoveHandlerStatement(original, rewritten));
		}

		public override BoundStatement InstrumentSyncLockObjectCapture(BoundSyncLockStatement original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentSyncLockObjectCapture(original, rewritten));
		}

		public override BoundStatement InstrumentWhileStatementConditionalGotoStart(BoundWhileStatement original, BoundStatement ifConditionGotoStart)
		{
			return AddDynamicAnalysis(original, base.InstrumentWhileStatementConditionalGotoStart(original, ifConditionGotoStart));
		}

		public override BoundStatement InstrumentDoLoopStatementEntryOrConditionalGotoStart(BoundDoLoopStatement original, BoundStatement ifConditionGotoStartOpt)
		{
			BoundStatement boundStatement = base.InstrumentDoLoopStatementEntryOrConditionalGotoStart(original, ifConditionGotoStartOpt);
			if (original.ConditionOpt != null)
			{
				return AddDynamicAnalysis(original, boundStatement);
			}
			return boundStatement;
		}

		public override BoundStatement InstrumentIfStatementConditionalGoto(BoundIfStatement original, BoundStatement condGoto)
		{
			return AddDynamicAnalysis(original, base.InstrumentIfStatementConditionalGoto(original, condGoto));
		}

		public override BoundStatement CreateSelectStatementPrologue(BoundSelectStatement original)
		{
			return AddDynamicAnalysis(original, base.CreateSelectStatementPrologue(original));
		}

		public override BoundStatement InstrumentFieldOrPropertyInitializer(BoundFieldOrPropertyInitializer original, BoundStatement rewritten, int symbolIndex, bool createTemporary)
		{
			return AddDynamicAnalysis(original, base.InstrumentFieldOrPropertyInitializer(original, rewritten, symbolIndex, createTemporary));
		}

		public override BoundStatement InstrumentForEachLoopInitialization(BoundForEachStatement original, BoundStatement initialization)
		{
			return AddDynamicAnalysis(original, base.InstrumentForEachLoopInitialization(original, initialization));
		}

		public override BoundStatement InstrumentForLoopInitialization(BoundForToStatement original, BoundStatement initialization)
		{
			return AddDynamicAnalysis(original, base.InstrumentForLoopInitialization(original, initialization));
		}

		public override BoundStatement InstrumentLocalInitialization(BoundLocalDeclaration original, BoundStatement rewritten)
		{
			return AddDynamicAnalysis(original, base.InstrumentLocalInitialization(original, rewritten));
		}

		public override BoundStatement CreateUsingStatementPrologue(BoundUsingStatement original)
		{
			return AddDynamicAnalysis(original, base.CreateUsingStatementPrologue(original));
		}

		public override BoundStatement CreateWithStatementPrologue(BoundWithStatement original)
		{
			return AddDynamicAnalysis(original, base.CreateWithStatementPrologue(original));
		}

		private BoundStatement AddDynamicAnalysis(BoundStatement original, BoundStatement rewritten)
		{
			if (!original.WasCompilerGenerated)
			{
				return CollectDynamicAnalysis(original, rewritten);
			}
			return rewritten;
		}

		private BoundStatement CollectDynamicAnalysis(BoundStatement original, BoundStatement rewritten)
		{
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_methodBodyFactory.TopLevelMethod, _method, original.Syntax, _methodBodyFactory.CompilationState, _diagnostics);
			BoundStatement boundStatement = AddAnalysisPoint(SyntaxForSpan(original), syntheticBoundNodeFactory);
			if (rewritten == null)
			{
				return boundStatement;
			}
			return syntheticBoundNodeFactory.StatementList(boundStatement, rewritten);
		}

		private static DebugSourceDocument GetSourceDocument(DebugDocumentProvider debugDocumentProvider, SyntaxNode syntax)
		{
			return GetSourceDocument(debugDocumentProvider, syntax, syntax.GetLocation().GetMappedLineSpan());
		}

		private static DebugSourceDocument GetSourceDocument(DebugDocumentProvider debugDocumentProvider, SyntaxNode syntax, FileLinePositionSpan span)
		{
			string text = span.Path;
			if (text.Length == 0)
			{
				text = syntax.SyntaxTree.FilePath;
			}
			return debugDocumentProvider(text, "");
		}

		private BoundStatement AddAnalysisPoint(SyntaxNode syntaxForSpan, TextSpan alternateSpan, SyntheticBoundNodeFactory statementFactory)
		{
			return AddAnalysisPoint(syntaxForSpan, syntaxForSpan.SyntaxTree.GetMappedLineSpan(alternateSpan), statementFactory);
		}

		private BoundStatement AddAnalysisPoint(SyntaxNode syntaxForSpan, SyntheticBoundNodeFactory statementFactory)
		{
			return AddAnalysisPoint(syntaxForSpan, syntaxForSpan.GetLocation().GetMappedLineSpan(), statementFactory);
		}

		private BoundStatement AddAnalysisPoint(SyntaxNode syntaxForSpan, FileLinePositionSpan span, SyntheticBoundNodeFactory statementFactory)
		{
			int count = _spansBuilder.Count;
			_spansBuilder.Add(new SourceSpan(GetSourceDocument(_debugDocumentProvider, syntaxForSpan, span), span.StartLinePosition.Line, span.StartLinePosition.Character, span.EndLinePosition.Line, span.EndLinePosition.Character));
			BoundArrayAccess left = statementFactory.ArrayAccess(statementFactory.Local(_methodPayload, isLValue: false), isLValue: true, ImmutableArray.Create((BoundExpression)statementFactory.Literal(count)));
			return statementFactory.Assignment(left, statementFactory.Literal(value: true));
		}

		private static SyntaxNode SyntaxForSpan(BoundStatement statement)
		{
			switch (statement.Kind)
			{
			case BoundKind.IfStatement:
				return ((BoundIfStatement)statement).Condition.Syntax;
			case BoundKind.WhileStatement:
				return ((BoundWhileStatement)statement).Condition.Syntax;
			case BoundKind.ForToStatement:
				return ((BoundForToStatement)statement).InitialValue.Syntax;
			case BoundKind.ForEachStatement:
				return ((BoundForEachStatement)statement).Collection.Syntax;
			case BoundKind.DoLoopStatement:
				return ((BoundDoLoopStatement)statement).ConditionOpt.Syntax;
			case BoundKind.UsingStatement:
			{
				BoundUsingStatement boundUsingStatement = (BoundUsingStatement)statement;
				return ((BoundNode)(((object)boundUsingStatement.ResourceExpressionOpt) ?? ((object)boundUsingStatement))).Syntax;
			}
			case BoundKind.SyncLockStatement:
				return ((BoundSyncLockStatement)statement).LockExpression.Syntax;
			case BoundKind.SelectStatement:
				return ((BoundSelectStatement)statement).ExpressionStatement.Expression.Syntax;
			case BoundKind.LocalDeclaration:
			{
				BoundExpression initializerOpt = ((BoundLocalDeclaration)statement).InitializerOpt;
				if (initializerOpt != null)
				{
					return initializerOpt.Syntax;
				}
				break;
			}
			case BoundKind.FieldInitializer:
			case BoundKind.PropertyInitializer:
				if (statement.Syntax is EqualsValueSyntax equalsValueSyntax)
				{
					return equalsValueSyntax.Value;
				}
				if (statement.Syntax is AsNewClauseSyntax asNewClauseSyntax)
				{
					return asNewClauseSyntax._newExpression;
				}
				break;
			}
			return statement.Syntax;
		}

		private static MethodSymbol GetCreatePayloadOverload(VisualBasicCompilation compilation, WellKnownMember overload, SyntaxNode syntax, BindingDiagnosticBag diagnostics)
		{
			return (MethodSymbol)Binder.GetWellKnownTypeMember(compilation, overload, syntax, diagnostics);
		}

		private static TextSpan SkipAttributes(SyntaxNode syntax)
		{
			switch (VisualBasicExtensions.Kind(syntax))
			{
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			{
				MethodStatementSyntax subOrFunctionStatement = ((MethodBlockSyntax)syntax).SubOrFunctionStatement;
				return SkipAttributes(syntax, subOrFunctionStatement.AttributeLists, subOrFunctionStatement.Modifiers, subOrFunctionStatement.SubOrFunctionKeyword);
			}
			case SyntaxKind.PropertyBlock:
			{
				PropertyStatementSyntax propertyStatement = ((PropertyBlockSyntax)syntax).PropertyStatement;
				return SkipAttributes(syntax, propertyStatement.AttributeLists, propertyStatement.Modifiers, propertyStatement.PropertyKeyword);
			}
			case SyntaxKind.GetAccessorBlock:
			case SyntaxKind.SetAccessorBlock:
			{
				AccessorStatementSyntax accessorStatement = ((AccessorBlockSyntax)syntax).AccessorStatement;
				return SkipAttributes(syntax, accessorStatement.AttributeLists, accessorStatement.Modifiers, accessorStatement.AccessorKeyword);
			}
			case SyntaxKind.ConstructorBlock:
			{
				SubNewStatementSyntax subNewStatement = ((ConstructorBlockSyntax)syntax).SubNewStatement;
				return SkipAttributes(syntax, subNewStatement.AttributeLists, subNewStatement.Modifiers, subNewStatement.SubKeyword);
			}
			case SyntaxKind.OperatorBlock:
			{
				OperatorStatementSyntax operatorStatement = ((OperatorBlockSyntax)syntax).OperatorStatement;
				return SkipAttributes(syntax, operatorStatement.AttributeLists, operatorStatement.Modifiers, operatorStatement.OperatorKeyword);
			}
			default:
				return syntax.Span;
			}
		}

		private static TextSpan SkipAttributes(SyntaxNode syntax, SyntaxList<AttributeListSyntax> attributes, SyntaxTokenList modifiers, SyntaxToken keyword)
		{
			TextSpan span = syntax.Span;
			if (attributes.Count > 0)
			{
				TextSpan textSpan = ((modifiers.Node != null) ? modifiers.Span : keyword.Span);
				return new TextSpan(textSpan.Start, span.Length - (textSpan.Start - span.Start));
			}
			return span;
		}
	}
}

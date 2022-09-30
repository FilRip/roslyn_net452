using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.CodeGen
{
	internal sealed class CodeGenerator
	{
		private enum AddressKind
		{
			Writeable,
			ReadOnly,
			Immutable
		}

		private enum ArrayInitializerStyle
		{
			Element,
			Block,
			Mixed
		}

		private struct IndexDesc
		{
			public readonly int Index;

			public readonly ImmutableArray<BoundExpression> Initializers;

			public IndexDesc(int Index, ImmutableArray<BoundExpression> Initializers)
			{
				this = default(IndexDesc);
				this.Index = Index;
				this.Initializers = Initializers;
			}
		}

		private class EmitCancelledException : Exception
		{
		}

		private enum UseKind
		{
			Unused,
			UsedAsValue,
			UsedAsAddress
		}

		private enum CallKind
		{
			Call,
			CallVirt,
			ConstrainedCallVirt
		}

		private enum ConstResKind
		{
			ConstFalse,
			ConstTrue,
			NotAConst
		}

		private class LabelFinder : StatementWalker
		{
			private readonly LabelSymbol _label;

			private bool _found;

			private LabelFinder(LabelSymbol label)
			{
				_found = false;
				_label = label;
			}

			public override BoundNode Visit(BoundNode node)
			{
				if (!_found && !(node is BoundExpression))
				{
					return base.Visit(node);
				}
				return null;
			}

			public override BoundNode VisitLabelStatement(BoundLabelStatement node)
			{
				if ((object)node.Label == _label)
				{
					_found = true;
				}
				return base.VisitLabelStatement(node);
			}

			public static bool NodeContainsLabel(BoundNode node, LabelSymbol label)
			{
				LabelFinder labelFinder = new LabelFinder(label);
				labelFinder.Visit(node);
				return labelFinder._found;
			}
		}

		private readonly MethodSymbol _method;

		private readonly BoundStatement _block;

		private readonly ILBuilder _builder;

		private readonly PEModuleBuilder _module;

		private readonly DiagnosticBag _diagnostics;

		private readonly ILEmitStyle _ilEmitStyle;

		private readonly bool _emitPdbSequencePoints;

		private readonly HashSet<LocalSymbol> _stackLocals;

		private int _tryNestingLevel;

		private BoundCatchBlock _currentCatchBlock;

		private readonly SynthesizedLocalOrdinalsDispenser _synthesizedLocalOrdinals;

		private int _uniqueNameId;

		private static readonly object s_returnLabel = RuntimeHelpers.GetObjectValue(new object());

		private bool _unhandledReturn;

		private bool _checkCallsForUnsafeJITOptimization;

		private int _asyncCatchHandlerOffset;

		private ArrayBuilder<int> _asyncYieldPoints;

		private ArrayBuilder<int> _asyncResumePoints;

		private int _recursionDepth;

		private static readonly ILOpCode[] s_compOpCodes = new ILOpCode[12]
		{
			ILOpCode.Clt,
			ILOpCode.Cgt,
			ILOpCode.Cgt,
			ILOpCode.Clt,
			ILOpCode.Clt_un,
			ILOpCode.Cgt_un,
			ILOpCode.Cgt_un,
			ILOpCode.Clt_un,
			ILOpCode.Clt,
			ILOpCode.Cgt_un,
			ILOpCode.Cgt,
			ILOpCode.Clt_un
		};

		private const int s_IL_OP_CODE_ROW_LENGTH = 4;

		private static readonly ILOpCode[] s_condJumpOpCodes = new ILOpCode[24]
		{
			ILOpCode.Blt,
			ILOpCode.Ble,
			ILOpCode.Bgt,
			ILOpCode.Bge,
			ILOpCode.Bge,
			ILOpCode.Bgt,
			ILOpCode.Ble,
			ILOpCode.Blt,
			ILOpCode.Blt_un,
			ILOpCode.Ble_un,
			ILOpCode.Bgt_un,
			ILOpCode.Bge_un,
			ILOpCode.Bge_un,
			ILOpCode.Bgt_un,
			ILOpCode.Ble_un,
			ILOpCode.Blt_un,
			ILOpCode.Blt,
			ILOpCode.Ble,
			ILOpCode.Bgt,
			ILOpCode.Bge,
			ILOpCode.Bge_un,
			ILOpCode.Bgt_un,
			ILOpCode.Ble_un,
			ILOpCode.Blt_un
		};

		public CodeGenerator(MethodSymbol method, BoundStatement boundBody, ILBuilder builder, PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics, OptimizationLevel optimizations, bool emittingPdb)
		{
			_stackLocals = null;
			_tryNestingLevel = 0;
			_currentCatchBlock = null;
			_synthesizedLocalOrdinals = new SynthesizedLocalOrdinalsDispenser();
			_asyncCatchHandlerOffset = -1;
			_asyncYieldPoints = null;
			_asyncResumePoints = null;
			_method = method;
			_block = boundBody;
			_builder = builder;
			_module = moduleBuilder;
			_diagnostics = diagnostics;
			if (!method.GenerateDebugInfo)
			{
				_ilEmitStyle = ILEmitStyle.Release;
			}
			else if (optimizations == OptimizationLevel.Debug)
			{
				_ilEmitStyle = ILEmitStyle.Debug;
			}
			else
			{
				_ilEmitStyle = (IsDebugPlus() ? ILEmitStyle.DebugFriendlyRelease : ILEmitStyle.Release);
			}
			_emitPdbSequencePoints = emittingPdb && method.GenerateDebugInfo;
			try
			{
				_block = Optimizer.Optimize(method, boundBody, _ilEmitStyle != ILEmitStyle.Release, out _stackLocals);
			}
			catch (BoundTreeVisitor.CancelledByStackGuardException ex)
			{
				ProjectData.SetProjectError(ex);
				BoundTreeVisitor.CancelledByStackGuardException ex2 = ex;
				ex2.AddAnError(diagnostics);
				_block = boundBody;
				ProjectData.ClearProjectError();
			}
			_checkCallsForUnsafeJITOptimization = (_method.ImplementationAttributes & (MethodImplAttributes)72) != (MethodImplAttributes)72;
		}

		private bool IsDebugPlus()
		{
			return _module.Compilation.Options.DebugPlusMode;
		}

		public void Generate()
		{
			GenerateImpl();
		}

		public void Generate(out int asyncCatchHandlerOffset, out ImmutableArray<int> asyncYieldPoints, out ImmutableArray<int> asyncResumePoints)
		{
			GenerateImpl();
			asyncCatchHandlerOffset = _builder.GetILOffsetFromMarker(_asyncCatchHandlerOffset);
			ArrayBuilder<int> asyncYieldPoints2 = _asyncYieldPoints;
			ArrayBuilder<int> asyncResumePoints2 = _asyncResumePoints;
			if (asyncYieldPoints2 == null)
			{
				asyncYieldPoints = ImmutableArray<int>.Empty;
				asyncResumePoints = ImmutableArray<int>.Empty;
				return;
			}
			ArrayBuilder<int> instance = ArrayBuilder<int>.GetInstance();
			ArrayBuilder<int> instance2 = ArrayBuilder<int>.GetInstance();
			int num = asyncYieldPoints2.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				int iLOffsetFromMarker = _builder.GetILOffsetFromMarker(asyncYieldPoints2[i]);
				int iLOffsetFromMarker2 = _builder.GetILOffsetFromMarker(asyncResumePoints2[i]);
				if (iLOffsetFromMarker > 0)
				{
					instance.Add(iLOffsetFromMarker);
					instance2.Add(iLOffsetFromMarker2);
				}
			}
			asyncYieldPoints = instance.ToImmutableAndFree();
			asyncResumePoints = instance2.ToImmutableAndFree();
			asyncYieldPoints2.Free();
			asyncResumePoints2.Free();
		}

		private void GenerateImpl()
		{
			SetInitialDebugDocument();
			if (_emitPdbSequencePoints && _method.IsImplicitlyDeclared)
			{
				_builder.DefineInitialHiddenSequencePoint();
			}
			try
			{
				EmitStatement(_block);
				if (_unhandledReturn)
				{
					HandleReturn();
				}
				if (!_diagnostics.HasAnyErrors())
				{
					_builder.Realize();
				}
			}
			catch (EmitCancelledException ex)
			{
				ProjectData.SetProjectError(ex);
				EmitCancelledException ex2 = ex;
				ProjectData.ClearProjectError();
			}
			_synthesizedLocalOrdinals.Free();
		}

		private void HandleReturn()
		{
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(s_returnLabel));
			_builder.EmitRet(isVoid: true);
			_unhandledReturn = false;
		}

		private void EmitFieldAccess(BoundFieldAccess fieldAccess)
		{
			FieldSymbol fieldSymbol = fieldAccess.FieldSymbol;
			if (!fieldSymbol.IsShared)
			{
				EmitExpression(fieldAccess.ReceiverOpt, used: true);
			}
			if (fieldSymbol.IsShared)
			{
				_builder.EmitOpCode(ILOpCode.Ldsfld);
			}
			else
			{
				_builder.EmitOpCode(ILOpCode.Ldfld);
			}
			EmitSymbolToken(fieldSymbol, fieldAccess.Syntax);
		}

		private bool IsStackLocal(LocalSymbol local)
		{
			if (_stackLocals != null)
			{
				return _stackLocals.Contains(local);
			}
			return false;
		}

		private void EmitLocalStore(BoundLocal local)
		{
			LocalDefinition local2 = GetLocal(local);
			_builder.EmitLocalStore(local2);
		}

		private void EmitSymbolToken(FieldSymbol symbol, SyntaxNode syntaxNode)
		{
			_builder.EmitToken(_module.Translate(symbol, syntaxNode, _diagnostics), syntaxNode, _diagnostics);
		}

		private void EmitSymbolToken(MethodSymbol symbol, SyntaxNode syntaxNode, bool encodeAsRawDefinitionToken = false)
		{
			_builder.EmitToken(_module.Translate(symbol, syntaxNode, _diagnostics, encodeAsRawDefinitionToken), syntaxNode, _diagnostics, encodeAsRawDefinitionToken);
		}

		private void EmitSymbolToken(TypeSymbol symbol, SyntaxNode syntaxNode)
		{
			_builder.EmitToken(_module.Translate(symbol, syntaxNode, _diagnostics), syntaxNode, _diagnostics);
		}

		private void EmitSequencePointExpression(BoundSequencePointExpression node, bool used)
		{
			SyntaxNode syntax = node.Syntax;
			if (_emitPdbSequencePoints)
			{
				if (syntax == null)
				{
					EmitHiddenSequencePoint();
				}
				else
				{
					EmitSequencePoint(syntax);
				}
			}
			EmitExpression(node.Expression, used: true);
			EmitPopIfUnused(used);
		}

		private void EmitSequencePointExpressionAddress(BoundSequencePointExpression node, AddressKind addressKind)
		{
			SyntaxNode syntax = node.Syntax;
			if (_emitPdbSequencePoints)
			{
				if (syntax == null)
				{
					EmitHiddenSequencePoint();
				}
				else
				{
					EmitSequencePoint(syntax);
				}
			}
			EmitAddress(node.Expression, addressKind);
		}

		private void EmitSequencePointStatement(BoundSequencePoint node)
		{
			SyntaxNode syntax = node.Syntax;
			if (_emitPdbSequencePoints)
			{
				if (syntax == null)
				{
					EmitHiddenSequencePoint();
				}
				else
				{
					EmitSequencePoint(syntax);
				}
			}
			BoundStatement statementOpt = node.StatementOpt;
			int num = 0;
			if (statementOpt != null)
			{
				num = EmitStatementAndCountInstructions(statementOpt);
			}
			if (num == 0 && syntax != null && _ilEmitStyle == ILEmitStyle.Debug)
			{
				_builder.EmitOpCode(ILOpCode.Nop);
			}
		}

		private void EmitSequencePointStatement(BoundSequencePointWithSpan node)
		{
			TextSpan span = node.Span;
			if (span != default(TextSpan) && _emitPdbSequencePoints)
			{
				EmitSequencePoint(node.SyntaxTree, span);
			}
			BoundStatement statementOpt = node.StatementOpt;
			int num = 0;
			if (statementOpt != null)
			{
				num = EmitStatementAndCountInstructions(statementOpt);
			}
			if (num == 0 && span != default(TextSpan) && _ilEmitStyle == ILEmitStyle.Debug)
			{
				_builder.EmitOpCode(ILOpCode.Nop);
			}
		}

		private void SetInitialDebugDocument()
		{
			SyntaxNode syntax = _method.Syntax;
			if (_emitPdbSequencePoints && syntax != null)
			{
				_builder.SetInitialDebugDocument(syntax.SyntaxTree);
			}
		}

		private void EmitHiddenSequencePoint()
		{
			_builder.DefineHiddenSequencePoint();
		}

		private void EmitSequencePoint(SyntaxNode syntax)
		{
			EmitSequencePoint(syntax.SyntaxTree, syntax.Span);
		}

		private TextSpan EmitSequencePoint(SyntaxTree tree, TextSpan span)
		{
			_builder.DefineSequencePoint(tree, span);
			return span;
		}

		private LocalDefinition EmitAddress(BoundExpression expression, AddressKind addressKind)
		{
			BoundKind kind = expression.Kind;
			LocalDefinition result = null;
			if (!AllowedToTakeRef(expression, addressKind))
			{
				return EmitAddressOfTempClone(expression);
			}
			switch (kind)
			{
			case BoundKind.Local:
			{
				BoundLocal boundLocal = (BoundLocal)expression;
				if (!IsStackLocal(boundLocal.LocalSymbol))
				{
					LocalDefinition local = GetLocal(boundLocal);
					_builder.EmitLocalAddress(local);
				}
				break;
			}
			case BoundKind.Dup:
				_builder.EmitOpCode(ILOpCode.Dup);
				break;
			case BoundKind.ReferenceAssignment:
				EmitReferenceAssignment((BoundReferenceAssignment)expression, used: true, needReference: true);
				break;
			case BoundKind.ComplexConditionalAccessReceiver:
				EmitComplexConditionalAccessReceiverAddress((BoundComplexConditionalAccessReceiver)expression);
				break;
			case BoundKind.Parameter:
				EmitParameterAddress((BoundParameter)expression);
				break;
			case BoundKind.FieldAccess:
				result = EmitFieldAddress((BoundFieldAccess)expression, addressKind);
				break;
			case BoundKind.ArrayAccess:
				EmitArrayElementAddress((BoundArrayAccess)expression, addressKind);
				break;
			case BoundKind.MeReference:
			case BoundKind.MyClassReference:
				_builder.EmitOpCode(ILOpCode.Ldarg_0);
				break;
			case BoundKind.ValueTypeMeReference:
				_builder.EmitOpCode(ILOpCode.Ldarg_0);
				break;
			case BoundKind.Sequence:
				result = EmitSequenceAddress((BoundSequence)expression, addressKind);
				break;
			case BoundKind.SequencePointExpression:
				EmitSequencePointExpressionAddress((BoundSequencePointExpression)expression, addressKind);
				break;
			case BoundKind.PseudoVariable:
				EmitPseudoVariableAddress((BoundPseudoVariable)expression);
				break;
			case BoundKind.Call:
			{
				BoundCall call = (BoundCall)expression;
				EmitCallExpression(call, UseKind.UsedAsAddress);
				break;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(kind);
			case BoundKind.Parenthesized:
			case BoundKind.MyBaseReference:
			case BoundKind.ConditionalAccessReceiverPlaceholder:
				break;
			}
			return result;
		}

		private void EmitPseudoVariableAddress(BoundPseudoVariable expression)
		{
			EmitExpression(expression.EmitExpressions.GetAddress(expression), used: true);
		}

		private LocalDefinition EmitAddressOfTempClone(BoundExpression expression)
		{
			EmitExpression(expression, used: true);
			LocalDefinition localDefinition = AllocateTemp(expression.Type, expression.Syntax);
			_builder.EmitLocalStore(localDefinition);
			_builder.EmitLocalAddress(localDefinition);
			return localDefinition;
		}

		private LocalDefinition EmitSequenceAddress(BoundSequence sequence, AddressKind addressKind)
		{
			bool flag = !sequence.Locals.IsEmpty;
			if (flag)
			{
				_builder.OpenLocalScope();
				ImmutableArray<LocalSymbol>.Enumerator enumerator = sequence.Locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					DefineLocal(current, sequence.Syntax);
				}
			}
			EmitSideEffects(sequence.SideEffects);
			LocalDefinition localDefinition = EmitAddress(sequence.ValueOpt, addressKind);
			LocalSymbol localSymbol = null;
			if (localDefinition == null)
			{
				BoundLocal boundLocal = DigForLocal(sequence.ValueOpt);
				if (boundLocal != null)
				{
					localSymbol = boundLocal.LocalSymbol;
				}
			}
			if (flag)
			{
				_builder.CloseLocalScope();
				ImmutableArray<LocalSymbol>.Enumerator enumerator2 = sequence.Locals.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					LocalSymbol current2 = enumerator2.Current;
					if ((object)current2 != localSymbol)
					{
						FreeLocal(current2);
					}
					else
					{
						localDefinition = GetLocal(localSymbol);
					}
				}
			}
			return localDefinition;
		}

		private BoundLocal DigForLocal(BoundExpression value)
		{
			switch (value.Kind)
			{
			case BoundKind.Local:
			{
				BoundLocal boundLocal = (BoundLocal)value;
				if (!boundLocal.LocalSymbol.IsByRef)
				{
					return boundLocal;
				}
				break;
			}
			case BoundKind.Sequence:
				return DigForLocal(((BoundSequence)value).ValueOpt);
			case BoundKind.FieldAccess:
			{
				BoundFieldAccess boundFieldAccess = (BoundFieldAccess)value;
				if (!boundFieldAccess.FieldSymbol.IsShared)
				{
					return DigForLocal(boundFieldAccess.ReceiverOpt);
				}
				break;
			}
			}
			return null;
		}

		private bool HasHome(BoundExpression expression)
		{
			switch (expression.Kind)
			{
			case BoundKind.Sequence:
			{
				BoundExpression valueOpt = ((BoundSequence)expression).ValueOpt;
				return valueOpt != null && HasHome(valueOpt);
			}
			case BoundKind.FieldAccess:
				return HasHome((BoundFieldAccess)expression);
			case BoundKind.ArrayAccess:
			case BoundKind.ReferenceAssignment:
			case BoundKind.MeReference:
			case BoundKind.MyBaseReference:
			case BoundKind.Parameter:
				return true;
			case BoundKind.Local:
			{
				LocalSymbol localSymbol = ((BoundLocal)expression).LocalSymbol;
				return !IsStackLocal(localSymbol) || localSymbol.IsByRef;
			}
			case BoundKind.Call:
				return ((BoundCall)expression).Method.ReturnsByRef;
			case BoundKind.Dup:
				return ((BoundDup)expression).IsReference;
			case BoundKind.ValueTypeMeReference:
				return true;
			default:
				return false;
			}
		}

		private bool HasHome(BoundFieldAccess fieldAccess)
		{
			FieldSymbol fieldSymbol = fieldAccess.FieldSymbol;
			if (fieldSymbol.IsConst && !fieldSymbol.IsConstButNotMetadataConstant)
			{
				return false;
			}
			if (!fieldSymbol.IsReadOnly)
			{
				return true;
			}
			if (!TypeSymbol.Equals(fieldSymbol.ContainingType, _method.ContainingType, TypeCompareKind.ConsiderEverything))
			{
				return false;
			}
			if (fieldSymbol.IsShared)
			{
				return _method.MethodKind == MethodKind.StaticConstructor;
			}
			return _method.MethodKind == MethodKind.Constructor && fieldAccess.ReceiverOpt.Kind == BoundKind.MeReference;
		}

		private bool AllowedToTakeRef(BoundExpression expression, AddressKind addressKind)
		{
			if (expression.Kind == BoundKind.ConditionalAccessReceiverPlaceholder || expression.Kind == BoundKind.ComplexConditionalAccessReceiver)
			{
				return addressKind == AddressKind.ReadOnly || addressKind == AddressKind.Immutable;
			}
			if (addressKind != AddressKind.Immutable)
			{
				switch (expression.Kind)
				{
				case BoundKind.Sequence:
				{
					BoundExpression valueOpt = ((BoundSequence)expression).ValueOpt;
					return valueOpt != null && AllowedToTakeRef(valueOpt, addressKind);
				}
				case BoundKind.FieldAccess:
					return AllowedToTakeRef((BoundFieldAccess)expression, addressKind);
				case BoundKind.Local:
					return AllowedToTakeRef((BoundLocal)expression, addressKind);
				case BoundKind.Parameter:
					return true;
				case BoundKind.PseudoVariable:
					return true;
				case BoundKind.Dup:
					return ((BoundDup)expression).IsReference;
				case BoundKind.MeReference:
				case BoundKind.MyClassReference:
					return addressKind != AddressKind.Writeable;
				}
			}
			return HasHome(expression);
		}

		private bool AllowedToTakeRef(BoundLocal boundLocal, AddressKind addressKind)
		{
			if (addressKind == AddressKind.Writeable && boundLocal.LocalSymbol.IsReadOnly && !boundLocal.IsLValue)
			{
				return false;
			}
			if (!HasHome(boundLocal))
			{
				return false;
			}
			if (boundLocal.IsConstant)
			{
				TypeSymbol type = boundLocal.Type;
				if (!TypeSymbolExtensions.IsDecimalType(type) && !TypeSymbolExtensions.IsDateTimeType(type))
				{
					return false;
				}
			}
			return true;
		}

		private bool AllowedToTakeRef(BoundFieldAccess fieldAccess, AddressKind addressKind)
		{
			if (addressKind != AddressKind.Immutable)
			{
				if (!HasHome(fieldAccess))
				{
					return false;
				}
				if (fieldAccess.FieldSymbol.ContainingType.IsValueType)
				{
					BoundExpression receiverOpt = fieldAccess.ReceiverOpt;
					if (receiverOpt != null && !AllowedToTakeRef(receiverOpt, AddressKind.ReadOnly))
					{
						if (!HasHome(receiverOpt))
						{
							return true;
						}
						return false;
					}
				}
			}
			return HasHome(fieldAccess);
		}

		private void EmitArrayElementAddress(BoundArrayAccess arrayAccess, AddressKind addressKind)
		{
			EmitExpression(arrayAccess.Expression, used: true);
			EmitExpressions(arrayAccess.Indices, used: true);
			TypeSymbol type = arrayAccess.Type;
			if (addressKind != 0 && TypeSymbolExtensions.IsTypeParameter(type))
			{
				_builder.EmitOpCode(ILOpCode.Readonly);
			}
			if (((ArrayTypeSymbol)arrayAccess.Expression.Type).IsSZArray)
			{
				_builder.EmitOpCode(ILOpCode.Ldelema);
				EmitSymbolToken(type, arrayAccess.Syntax);
			}
			else
			{
				_builder.EmitArrayElementAddress(_module.Translate((ArrayTypeSymbol)arrayAccess.Expression.Type), arrayAccess.Syntax, _diagnostics);
			}
		}

		private LocalDefinition EmitFieldAddress(BoundFieldAccess fieldAccess, AddressKind addressKind)
		{
			FieldSymbol fieldSymbol = fieldAccess.FieldSymbol;
			if (fieldAccess.FieldSymbol.IsShared)
			{
				EmitStaticFieldAddress(fieldSymbol, fieldAccess.Syntax);
				return null;
			}
			return EmitInstanceFieldAddress(fieldAccess, addressKind);
		}

		private void EmitStaticFieldAddress(FieldSymbol field, SyntaxNode syntaxNode)
		{
			_builder.EmitOpCode(ILOpCode.Ldsflda);
			EmitSymbolToken(field, syntaxNode);
		}

		private void EmitParameterAddress(BoundParameter parameter)
		{
			int argNumber = ParameterSlot(parameter);
			if (!parameter.ParameterSymbol.IsByRef)
			{
				_builder.EmitLoadArgumentAddrOpcode(argNumber);
			}
			else
			{
				_builder.EmitLoadArgumentOpcode(argNumber);
			}
		}

		private LocalDefinition EmitReceiverRef(BoundExpression receiver, bool isAccessConstrained, AddressKind addressKind)
		{
			TypeSymbol type = receiver.Type;
			if (TypeSymbolExtensions.IsVerifierReference(type))
			{
				EmitExpression(receiver, used: true);
				return null;
			}
			if (type.TypeKind == TypeKind.TypeParameter)
			{
				if (isAccessConstrained)
				{
					return EmitAddress(receiver, AddressKind.ReadOnly);
				}
				EmitExpression(receiver, used: true);
				if (receiver.Kind != BoundKind.ConditionalAccessReceiverPlaceholder)
				{
					EmitBox(type, receiver.Syntax);
				}
				return null;
			}
			return EmitAddress(receiver, addressKind);
		}

		private LocalDefinition EmitInstanceFieldAddress(BoundFieldAccess fieldAccess, AddressKind addressKind)
		{
			FieldSymbol fieldSymbol = fieldAccess.FieldSymbol;
			if (addressKind == AddressKind.Writeable && IsMeReceiver(fieldAccess.ReceiverOpt))
			{
				addressKind = AddressKind.ReadOnly;
			}
			LocalDefinition result = EmitReceiverRef(fieldAccess.ReceiverOpt, isAccessConstrained: false, addressKind);
			_builder.EmitOpCode(ILOpCode.Ldflda);
			EmitSymbolToken(fieldSymbol, fieldAccess.Syntax);
			return result;
		}

		private void EmitArrayInitializers(ArrayTypeSymbol arrayType, BoundArrayInitialization inits)
		{
			ImmutableArray<BoundExpression> initializers = inits.Initializers;
			ArrayInitializerStyle arrayInitializerStyle = ShouldEmitBlockInitializer(arrayType.ElementType, initializers);
			if (arrayInitializerStyle == ArrayInitializerStyle.Element)
			{
				EmitElementInitializers(arrayType, initializers, includeConstants: true);
				return;
			}
			_builder.EmitArrayBlockInitializer(GetRawData(initializers), inits.Syntax, _diagnostics);
			if (arrayInitializerStyle == ArrayInitializerStyle.Mixed)
			{
				EmitElementInitializers(arrayType, initializers, includeConstants: false);
			}
		}

		private void EmitElementInitializers(ArrayTypeSymbol arrayType, ImmutableArray<BoundExpression> inits, bool includeConstants)
		{
			if (!IsMultidimensionalInitializer(inits))
			{
				EmitOnedimensionalElementInitializers(arrayType, inits, includeConstants);
			}
			else
			{
				EmitMultidimensionalElementInitializers(arrayType, inits, includeConstants);
			}
		}

		private void EmitOnedimensionalElementInitializers(ArrayTypeSymbol arrayType, ImmutableArray<BoundExpression> inits, bool includeConstants)
		{
			int num = inits.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				BoundExpression boundExpression = inits[i];
				if (ShouldEmitInitExpression(includeConstants, boundExpression))
				{
					_builder.EmitOpCode(ILOpCode.Dup);
					_builder.EmitIntConstant(i);
					EmitExpression(boundExpression, used: true);
					EmitArrayElementStore(arrayType, boundExpression.Syntax);
				}
			}
		}

		private static bool ShouldEmitInitExpression(bool includeConstants, BoundExpression init)
		{
			if ((object)init.ConstantValueOpt != null)
			{
				if (includeConstants)
				{
					return !init.ConstantValueOpt.IsDefaultValue;
				}
				return false;
			}
			return true;
		}

		private void EmitMultidimensionalElementInitializers(ArrayTypeSymbol arrayType, ImmutableArray<BoundExpression> inits, bool includeConstants)
		{
			ArrayBuilder<IndexDesc> arrayBuilder = new ArrayBuilder<IndexDesc>();
			int num = inits.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				arrayBuilder.Push(new IndexDesc(i, ((BoundArrayInitialization)inits[i]).Initializers));
				EmitAllElementInitializersRecursive(arrayType, arrayBuilder, includeConstants);
			}
		}

		private void EmitAllElementInitializersRecursive(ArrayTypeSymbol arrayType, ArrayBuilder<IndexDesc> indices, bool includeConstants)
		{
			ImmutableArray<BoundExpression> initializers = indices.Peek().Initializers;
			if (IsMultidimensionalInitializer(initializers))
			{
				int num = initializers.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					indices.Push(new IndexDesc(i, ((BoundArrayInitialization)initializers[i]).Initializers));
					EmitAllElementInitializersRecursive(arrayType, indices, includeConstants);
				}
			}
			else
			{
				int num2 = initializers.Length - 1;
				for (int j = 0; j <= num2; j++)
				{
					BoundExpression boundExpression = initializers[j];
					if (ShouldEmitInitExpression(includeConstants, boundExpression))
					{
						_builder.EmitOpCode(ILOpCode.Dup);
						ArrayBuilder<IndexDesc>.Enumerator enumerator = indices.GetEnumerator();
						while (enumerator.MoveNext())
						{
							IndexDesc current = enumerator.Current;
							_builder.EmitIntConstant(current.Index);
						}
						_builder.EmitIntConstant(j);
						BoundExpression expression = initializers[j];
						EmitExpression(expression, used: true);
						EmitArrayElementStore(arrayType, boundExpression.Syntax);
					}
				}
			}
			indices.Pop();
		}

		private ConstantValue AsConstOrDefault(BoundExpression init)
		{
			ConstantValue constantValueOpt = init.ConstantValueOpt;
			if ((object)constantValueOpt != null)
			{
				return constantValueOpt;
			}
			return ConstantValue.Default(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(init.Type).SpecialType);
		}

		private ArrayInitializerStyle ShouldEmitBlockInitializer(TypeSymbol elementType, ImmutableArray<BoundExpression> inits)
		{
			if (!_module.SupportsPrivateImplClass)
			{
				return ArrayInitializerStyle.Element;
			}
			if (TypeSymbolExtensions.IsEnumType(elementType))
			{
				if (!_module.Compilation.EnableEnumArrayBlockInitialization)
				{
					return ArrayInitializerStyle.Element;
				}
				elementType = ((NamedTypeSymbol)elementType).EnumUnderlyingType;
			}
			if (elementType.SpecialType.IsBlittable())
			{
				if (_module.GetInitArrayHelper() == null)
				{
					return ArrayInitializerStyle.Element;
				}
				int initCount = 0;
				int constInits = 0;
				InitializerCountRecursive(inits, ref initCount, ref constInits);
				if (initCount > 2)
				{
					if (initCount == constInits)
					{
						return ArrayInitializerStyle.Block;
					}
					int num = Math.Max(3, initCount / 3);
					if (constInits >= num)
					{
						return ArrayInitializerStyle.Mixed;
					}
				}
			}
			return ArrayInitializerStyle.Element;
		}

		private void InitializerCountRecursive(ImmutableArray<BoundExpression> inits, ref int initCount, ref int constInits)
		{
			if (inits.Length == 0)
			{
				return;
			}
			ImmutableArray<BoundExpression>.Enumerator enumerator = inits.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				if (current is BoundArrayInitialization boundArrayInitialization)
				{
					InitializerCountRecursive(boundArrayInitialization.Initializers, ref initCount, ref constInits);
				}
				else if (!BoundExpressionExtensions.IsDefaultValue(current))
				{
					initCount++;
					if ((object)current.ConstantValueOpt != null)
					{
						constInits++;
					}
				}
			}
		}

		private ImmutableArray<byte> GetRawData(ImmutableArray<BoundExpression> initializers)
		{
			PooledBlobBuilder instance = PooledBlobBuilder.GetInstance(initializers.Length * 4);
			SerializeArrayRecursive(instance, initializers);
			ImmutableArray<byte> result = instance.ToImmutableArray();
			instance.Free();
			return result;
		}

		private void SerializeArrayRecursive(BlobBuilder bw, ImmutableArray<BoundExpression> inits)
		{
			if (inits.Length == 0)
			{
				return;
			}
			if (inits[0].Kind == BoundKind.ArrayInitialization)
			{
				ImmutableArray<BoundExpression>.Enumerator enumerator = inits.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundExpression current = enumerator.Current;
					SerializeArrayRecursive(bw, ((BoundArrayInitialization)current).Initializers);
				}
			}
			else
			{
				ImmutableArray<BoundExpression>.Enumerator enumerator2 = inits.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					BoundExpression current2 = enumerator2.Current;
					AsConstOrDefault(current2).Serialize(bw);
				}
			}
		}

		private bool IsMultidimensionalInitializer(ImmutableArray<BoundExpression> inits)
		{
			if (inits.Length != 0)
			{
				return inits[0].Kind == BoundKind.ArrayInitialization;
			}
			return false;
		}

		private static bool IsSimpleType(Microsoft.Cci.PrimitiveTypeCode type)
		{
			bool result = false;
			switch (type)
			{
			case Microsoft.Cci.PrimitiveTypeCode.Boolean:
			case Microsoft.Cci.PrimitiveTypeCode.Int8:
			case Microsoft.Cci.PrimitiveTypeCode.Float32:
			case Microsoft.Cci.PrimitiveTypeCode.Float64:
			case Microsoft.Cci.PrimitiveTypeCode.Int16:
			case Microsoft.Cci.PrimitiveTypeCode.Int32:
			case Microsoft.Cci.PrimitiveTypeCode.Int64:
			case Microsoft.Cci.PrimitiveTypeCode.UInt8:
			case Microsoft.Cci.PrimitiveTypeCode.UInt16:
			case Microsoft.Cci.PrimitiveTypeCode.UInt32:
			case Microsoft.Cci.PrimitiveTypeCode.UInt64:
				result = true;
				break;
			}
			return result;
		}

		private static bool IsIntegral(Microsoft.Cci.PrimitiveTypeCode type)
		{
			bool result = false;
			switch (type)
			{
			case Microsoft.Cci.PrimitiveTypeCode.Int8:
			case Microsoft.Cci.PrimitiveTypeCode.Int16:
			case Microsoft.Cci.PrimitiveTypeCode.Int32:
			case Microsoft.Cci.PrimitiveTypeCode.Int64:
			case Microsoft.Cci.PrimitiveTypeCode.UInt8:
			case Microsoft.Cci.PrimitiveTypeCode.UInt16:
			case Microsoft.Cci.PrimitiveTypeCode.UInt32:
			case Microsoft.Cci.PrimitiveTypeCode.UInt64:
				result = true;
				break;
			}
			return result;
		}

		private void EmitConvertIntrinsic(BoundConversion conversion, Microsoft.Cci.PrimitiveTypeCode underlyingFrom, Microsoft.Cci.PrimitiveTypeCode underlyingTo)
		{
			EmitExpression(conversion.Operand, used: true);
			if (underlyingFrom == underlyingTo && !conversion.ExplicitCastInCode && underlyingFrom != Microsoft.Cci.PrimitiveTypeCode.Float32 && underlyingFrom != Microsoft.Cci.PrimitiveTypeCode.Float64)
			{
				return;
			}
			if (underlyingTo == Microsoft.Cci.PrimitiveTypeCode.Boolean)
			{
				_builder.EmitConstantValue(ConstantValue.Default(underlyingFrom.GetConstantValueTypeDiscriminator()));
				if (underlyingFrom.IsFloatingPoint())
				{
					_builder.EmitOpCode(ILOpCode.Ceq);
					_builder.EmitOpCode(ILOpCode.Ldc_i4_0);
					_builder.EmitOpCode(ILOpCode.Ceq);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Cgt_un);
				}
				return;
			}
			switch (underlyingFrom)
			{
			case Microsoft.Cci.PrimitiveTypeCode.Boolean:
				_builder.EmitOpCode(ILOpCode.Ldc_i4_0);
				_builder.EmitOpCode(ILOpCode.Cgt_un);
				_builder.EmitOpCode(ILOpCode.Neg);
				if (underlyingTo != Microsoft.Cci.PrimitiveTypeCode.Int32)
				{
					_builder.EmitNumericConversion(Microsoft.Cci.PrimitiveTypeCode.Int32, underlyingTo, @checked: false);
				}
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Float32:
				if (!IsIntegral(underlyingTo))
				{
					break;
				}
				switch (conversion.Operand.Kind)
				{
				case BoundKind.BinaryOperator:
				{
					BinaryOperatorKind binaryOperatorKind = ((BoundBinaryOperator)conversion.Operand).OperatorKind & BinaryOperatorKind.OpMask;
					if (binaryOperatorKind == BinaryOperatorKind.Add || (uint)(binaryOperatorKind - 10) <= 4u)
					{
						_builder.EmitOpCode(ILOpCode.Conv_r4);
					}
					break;
				}
				case BoundKind.UnaryOperator:
				{
					UnaryOperatorKind unaryOperatorKind = ((BoundUnaryOperator)conversion.Operand).OperatorKind & UnaryOperatorKind.Not;
					if ((uint)(unaryOperatorKind - 1) <= 1u)
					{
						_builder.EmitOpCode(ILOpCode.Conv_r4);
					}
					break;
				}
				}
				break;
			}
			EmitConvertSimpleNumeric(conversion, underlyingFrom, underlyingTo, conversion.Checked);
		}

		private void EmitConvertSimpleNumeric(BoundConversion conversion, Microsoft.Cci.PrimitiveTypeCode typeFrom, Microsoft.Cci.PrimitiveTypeCode typeTo, bool @checked)
		{
			_builder.EmitNumericConversion(typeFrom, typeTo, @checked);
		}

		private void EmitConversionExpression(BoundConversion conversion, bool used)
		{
			if (!used && !ConversionHasSideEffects(conversion))
			{
				EmitExpression(conversion.Operand, used: false);
				return;
			}
			TypeSymbol type = conversion.Type;
			if (BoundExpressionExtensions.IsNothingLiteral(conversion.Operand))
			{
				if (used)
				{
					EmitLoadDefaultValueOfTypeFromNothingLiteral(type, used: true, conversion.Syntax);
				}
				return;
			}
			Microsoft.Cci.PrimitiveTypeCode primitiveTypeCode = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type).PrimitiveTypeCode;
			TypeSymbol type2 = conversion.Operand.Type;
			Microsoft.Cci.PrimitiveTypeCode primitiveTypeCode2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type2).PrimitiveTypeCode;
			if ((IsSimpleType(primitiveTypeCode2) && IsSimpleType(primitiveTypeCode)) || (primitiveTypeCode2 == Microsoft.Cci.PrimitiveTypeCode.Char && primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.Int32))
			{
				EmitConvertIntrinsic(conversion, primitiveTypeCode2, primitiveTypeCode);
			}
			else if (TypeSymbolExtensions.IsNullableType(type2))
			{
				EmitExpression(conversion.Operand, used: true);
				if ((conversion.ConversionKind & ConversionKind.Narrowing) != 0)
				{
					EmitBox(type2, conversion.Operand.Syntax);
					_builder.EmitOpCode(ILOpCode.Castclass);
					EmitSymbolToken(type, conversion.Syntax);
				}
				else if (used)
				{
					EmitBox(type2, conversion.Operand.Syntax);
				}
			}
			else if (TypeSymbolExtensions.IsNullableType(type))
			{
				EmitExpression(conversion.Operand, used: true);
				EmitUnboxAny(type, conversion.Syntax);
			}
			else
			{
				EmitExpression(conversion.Operand, used: true);
				if (!Conversions.IsIdentityConversion(conversion.ConversionKind))
				{
					GeneratedLabelSymbol label = new GeneratedLabelSymbol("unbox");
					GeneratedLabelSymbol label2 = new GeneratedLabelSymbol("result");
					_builder.EmitOpCode(ILOpCode.Dup);
					_builder.EmitBranch(ILOpCode.Brtrue_s, label);
					_builder.EmitOpCode(ILOpCode.Pop);
					MethodSymbol parameterlessValueTypeConstructor = GetParameterlessValueTypeConstructor((NamedTypeSymbol)type);
					if ((object)parameterlessValueTypeConstructor == null || SymbolExtensions.IsDefaultValueTypeConstructor(parameterlessValueTypeConstructor))
					{
						EmitInitObj(type, used: true, conversion.Syntax);
					}
					else
					{
						DiagnosticInfo diagnosticInfo = parameterlessValueTypeConstructor.GetUseSiteInfo().DiagnosticInfo;
						if (diagnosticInfo != null)
						{
							_diagnostics.Add(new VBDiagnostic(diagnosticInfo, conversion.Syntax.Location));
						}
						EmitNewObj(parameterlessValueTypeConstructor, ImmutableArray<BoundExpression>.Empty, used: true, conversion.Syntax);
					}
					_builder.EmitBranch(ILOpCode.Br_s, label2);
					_builder.MarkLabel(label);
					_builder.EmitOpCode(ILOpCode.Unbox_any);
					EmitSymbolToken(type, conversion.Syntax);
					_builder.MarkLabel(label2);
				}
			}
			EmitPopIfUnused(used);
		}

		private MethodSymbol GetParameterlessValueTypeConstructor(NamedTypeSymbol typeTo)
		{
			ImmutableArray<MethodSymbol>.Enumerator enumerator = typeTo.InstanceConstructors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MethodSymbol current = enumerator.Current;
				if (current.ParameterCount == 0)
				{
					NamedTypeSymbol containingType = _method.ContainingType;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					if (AccessCheck.IsSymbolAccessible(current, containingType, typeTo, ref useSiteInfo))
					{
						return current;
					}
					return null;
				}
			}
			throw ExceptionUtilities.Unreachable;
		}

		private bool IsUnboxingDirectCast(BoundDirectCast conversion)
		{
			TypeSymbol type = conversion.Type;
			TypeSymbol type2 = conversion.Operand.Type;
			if (!BoundExpressionExtensions.IsNothingLiteral(conversion.Operand) && !Conversions.IsIdentityConversion(conversion.ConversionKind) && !TypeSymbolExtensions.IsSameTypeIgnoringAll(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type2), TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type)) && !TypeSymbolExtensions.IsTypeParameter(type2) && !type2.IsValueType)
			{
				return !type.IsReferenceType;
			}
			return false;
		}

		private void EmitDirectCastExpression(BoundDirectCast conversion, bool used)
		{
			if (!used && !ConversionHasSideEffects(conversion))
			{
				EmitExpression(conversion.Operand, used: false);
				return;
			}
			if (BoundExpressionExtensions.IsNothingLiteral(conversion.Operand))
			{
				if (TypeSymbolExtensions.IsTypeParameter(conversion.Type))
				{
					EmitLoadDefaultValueOfTypeParameter(conversion.Type, used, conversion.Syntax);
					return;
				}
				EmitExpression(conversion.Operand, used: true);
				if (conversion.Type.IsValueType)
				{
					EmitUnboxAny(conversion.Type, conversion.Syntax);
				}
			}
			else
			{
				EmitExpression(conversion.Operand, used: true);
				if (!Conversions.IsIdentityConversion(conversion.ConversionKind))
				{
					TypeSymbol type = conversion.Type;
					TypeSymbol type2 = conversion.Operand.Type;
					if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type2), TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type)))
					{
						if (TypeSymbolExtensions.IsTypeParameter(type2))
						{
							EmitBox(type2, conversion.Operand.Syntax);
							if (type.SpecialType != SpecialType.System_Object)
							{
								if (TypeSymbolExtensions.IsTypeParameter(type))
								{
									_builder.EmitOpCode(ILOpCode.Unbox_any);
									EmitSymbolToken(type, conversion.Syntax);
								}
								else if (type.IsReferenceType)
								{
									_builder.EmitOpCode(ILOpCode.Castclass);
									EmitSymbolToken(type, conversion.Syntax);
								}
								else
								{
									EmitUnboxAny(type, conversion.Syntax);
								}
							}
						}
						else if (TypeSymbolExtensions.IsTypeParameter(type))
						{
							if (type2.IsValueType)
							{
								EmitBox(type2, conversion.Operand.Syntax);
							}
							_builder.EmitOpCode(ILOpCode.Unbox_any);
							EmitSymbolToken(type, conversion.Syntax);
						}
						else if (type2.IsValueType)
						{
							EmitBox(type2, conversion.Operand.Syntax);
							if (TypeSymbolExtensions.IsInterfaceType(type))
							{
								_builder.EmitOpCode(ILOpCode.Castclass);
								EmitSymbolToken(type, conversion.Syntax);
							}
						}
						else if (type.IsReferenceType)
						{
							bool flag = true;
							if (Conversions.IsWideningConversion(conversion.ConversionKind))
							{
								flag = false;
								if (TypeSymbolExtensions.IsArrayType(type2))
								{
									TypeSymbol elementType = ((ArrayTypeSymbol)type2).ElementType;
									if (TypeSymbolExtensions.IsArrayType(type) && (TypeSymbolExtensions.IsTypeParameter(elementType) || TypeSymbolExtensions.IsTypeParameter(((ArrayTypeSymbol)type).ElementType)))
									{
										flag = true;
									}
									else if (TypeSymbolExtensions.IsTypeParameter(elementType) && TypeSymbolExtensions.IsInterfaceType(type))
									{
										NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;
										if (namedTypeSymbol.Arity == 1 && !TypeSymbolExtensions.IsSameTypeIgnoringAll(namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[0], elementType))
										{
											flag = true;
										}
									}
								}
							}
							if (flag)
							{
								_builder.EmitOpCode(ILOpCode.Castclass);
								EmitSymbolToken(type, conversion.Syntax);
							}
						}
						else
						{
							EmitUnboxAny(type, conversion.Syntax);
						}
					}
				}
			}
			EmitPopIfUnused(used);
		}

		private bool ConversionHasSideEffects(BoundConversion conversion)
		{
			return true;
		}

		private bool ConversionHasSideEffects(BoundDirectCast conversion)
		{
			return true;
		}

		private bool ConversionHasSideEffects(BoundTryCast conversion)
		{
			return false;
		}

		private void EmitTryCastExpression(BoundTryCast conversion, bool used)
		{
			if (!used && !ConversionHasSideEffects(conversion))
			{
				EmitExpression(conversion.Operand, used: false);
				return;
			}
			if (BoundExpressionExtensions.IsNothingLiteral(conversion.Operand))
			{
				if (TypeSymbolExtensions.IsTypeParameter(conversion.Type))
				{
					EmitLoadDefaultValueOfTypeParameter(conversion.Type, used: true, conversion.Syntax);
				}
				else
				{
					EmitExpression(conversion.Operand, used: true);
				}
			}
			else
			{
				EmitExpression(conversion.Operand, used: true);
				if (!Conversions.IsIdentityConversion(conversion.ConversionKind))
				{
					TypeSymbol type = conversion.Type;
					TypeSymbol type2 = conversion.Operand.Type;
					if (type2.IsReferenceType || TypeSymbolExtensions.IsTypeParameter(type2) || TypeSymbolExtensions.IsTypeParameter(type))
					{
						if (!TypeSymbolExtensions.IsVerifierReference(type2))
						{
							EmitBox(type2, conversion.Operand.Syntax);
						}
						_builder.EmitOpCode(ILOpCode.Isinst);
						EmitSymbolToken(type, conversion.Syntax);
						if (!TypeSymbolExtensions.IsVerifierReference(type))
						{
							_builder.EmitOpCode(ILOpCode.Unbox_any);
							EmitSymbolToken(type, conversion.Syntax);
						}
					}
					else
					{
						EmitBox(type2, conversion.Operand.Syntax);
					}
				}
			}
			EmitPopIfUnused(used);
		}

		private void EmitExpression(BoundExpression expression, bool used)
		{
			if (expression == null)
			{
				return;
			}
			ConstantValue constantValueOpt = expression.ConstantValueOpt;
			if ((object)constantValueOpt != null)
			{
				if (!used)
				{
					return;
				}
				if (!constantValueOpt.IsDecimal && !constantValueOpt.IsDateTime)
				{
					EmitConstantExpression(expression.Type, constantValueOpt, used, expression.Syntax);
					return;
				}
			}
			_recursionDepth++;
			if (_recursionDepth > 1)
			{
				StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
				EmitExpressionCore(expression, used);
			}
			else
			{
				EmitExpressionCoreWithStackGuard(expression, used);
			}
			_recursionDepth--;
		}

		private void EmitExpressionCoreWithStackGuard(BoundExpression expression, bool used)
		{
			try
			{
				EmitExpressionCore(expression, used);
			}
			catch (InsufficientExecutionStackException ex)
			{
				ProjectData.SetProjectError(ex);
				InsufficientExecutionStackException ex2 = ex;
				DiagnosticBagExtensions.Add(_diagnostics, ERRID.ERR_TooLongOrComplexExpression, BoundTreeVisitor.CancelledByStackGuardException.GetTooLongOrComplexExpressionErrorLocation(expression));
				throw new EmitCancelledException();
			}
		}

		private void EmitExpressionCore(BoundExpression expression, bool used)
		{
			switch (expression.Kind)
			{
			case BoundKind.AssignmentOperator:
				EmitAssignmentExpression((BoundAssignmentOperator)expression, used);
				break;
			case BoundKind.Call:
				EmitCallExpression((BoundCall)expression, used ? UseKind.UsedAsValue : UseKind.Unused);
				break;
			case BoundKind.TernaryConditionalExpression:
				EmitTernaryConditionalExpression((BoundTernaryConditionalExpression)expression, used);
				break;
			case BoundKind.BinaryConditionalExpression:
				EmitBinaryConditionalExpression((BoundBinaryConditionalExpression)expression, used);
				break;
			case BoundKind.ObjectCreationExpression:
				EmitObjectCreationExpression((BoundObjectCreationExpression)expression, used);
				break;
			case BoundKind.ArrayCreation:
				EmitArrayCreationExpression((BoundArrayCreation)expression, used);
				break;
			case BoundKind.ArrayLength:
				EmitArrayLengthExpression((BoundArrayLength)expression, used);
				break;
			case BoundKind.Conversion:
				EmitConversionExpression((BoundConversion)expression, used);
				break;
			case BoundKind.DirectCast:
				EmitDirectCastExpression((BoundDirectCast)expression, used);
				break;
			case BoundKind.TryCast:
				EmitTryCastExpression((BoundTryCast)expression, used);
				break;
			case BoundKind.TypeOf:
				EmitTypeOfExpression((BoundTypeOf)expression, used);
				break;
			case BoundKind.Local:
				EmitLocalLoad((BoundLocal)expression, used);
				break;
			case BoundKind.Parameter:
				if (used)
				{
					EmitParameterLoad((BoundParameter)expression);
				}
				break;
			case BoundKind.Dup:
				EmitDupExpression((BoundDup)expression, used);
				break;
			case BoundKind.FieldAccess:
				EmitFieldLoad((BoundFieldAccess)expression, used);
				break;
			case BoundKind.ArrayAccess:
				EmitArrayElementLoad((BoundArrayAccess)expression, used);
				break;
			case BoundKind.MeReference:
			case BoundKind.MyClassReference:
				if (used)
				{
					EmitMeOrMyClassReferenceExpression(expression);
				}
				break;
			case BoundKind.MyBaseReference:
				if (used)
				{
					_builder.EmitOpCode(ILOpCode.Ldarg_0);
				}
				break;
			case BoundKind.Sequence:
				EmitSequenceExpression((BoundSequence)expression, used);
				break;
			case BoundKind.SequencePointExpression:
				EmitSequencePointExpression((BoundSequencePointExpression)expression, used);
				break;
			case BoundKind.UnaryOperator:
				EmitUnaryOperatorExpression((BoundUnaryOperator)expression, used);
				break;
			case BoundKind.BinaryOperator:
				EmitBinaryOperatorExpression((BoundBinaryOperator)expression, used);
				break;
			case BoundKind.DelegateCreationExpression:
				EmitDelegateCreationExpression((BoundDelegateCreationExpression)expression, used);
				break;
			case BoundKind.GetType:
				EmitGetType((BoundGetType)expression, used);
				break;
			case BoundKind.FieldInfo:
				EmitFieldInfoExpression((BoundFieldInfo)expression, used);
				break;
			case BoundKind.MethodInfo:
				EmitMethodInfoExpression((BoundMethodInfo)expression, used);
				break;
			case BoundKind.ReferenceAssignment:
				EmitReferenceAssignment((BoundReferenceAssignment)expression, used);
				break;
			case BoundKind.ValueTypeMeReference:
				throw ExceptionUtilities.UnexpectedValue(expression.Kind);
			case BoundKind.LoweredConditionalAccess:
				EmitConditionalAccess((BoundLoweredConditionalAccess)expression, used);
				break;
			case BoundKind.ConditionalAccessReceiverPlaceholder:
				EmitConditionalAccessReceiverPlaceholder((BoundConditionalAccessReceiverPlaceholder)expression, used);
				break;
			case BoundKind.ComplexConditionalAccessReceiver:
				EmitComplexConditionalAccessReceiver((BoundComplexConditionalAccessReceiver)expression, used);
				break;
			case BoundKind.PseudoVariable:
				EmitPseudoVariableValue((BoundPseudoVariable)expression, used);
				break;
			case BoundKind.ModuleVersionId:
				EmitModuleVersionIdLoad((BoundModuleVersionId)expression);
				break;
			case BoundKind.ModuleVersionIdString:
				EmitModuleVersionIdStringLoad((BoundModuleVersionIdString)expression);
				break;
			case BoundKind.InstrumentationPayloadRoot:
				EmitInstrumentationPayloadRootLoad((BoundInstrumentationPayloadRoot)expression);
				break;
			case BoundKind.MethodDefIndex:
				EmitMethodDefIndexExpression((BoundMethodDefIndex)expression);
				break;
			case BoundKind.MaximumMethodDefIndex:
				EmitMaximumMethodDefIndexExpression((BoundMaximumMethodDefIndex)expression);
				break;
			case BoundKind.SourceDocumentIndex:
				EmitSourceDocumentIndex((BoundSourceDocumentIndex)expression);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(expression.Kind);
			}
		}

		private void EmitConditionalAccessReceiverPlaceholder(BoundConditionalAccessReceiverPlaceholder expression, bool used)
		{
			if (used && !expression.Type.IsReferenceType)
			{
				EmitLoadIndirect(expression.Type, expression.Syntax);
			}
			EmitPopIfUnused(used);
		}

		private void EmitComplexConditionalAccessReceiver(BoundComplexConditionalAccessReceiver expression, bool used)
		{
			TypeSymbol type = expression.Type;
			object objectValue = RuntimeHelpers.GetObjectValue(new object());
			object objectValue2 = RuntimeHelpers.GetObjectValue(new object());
			EmitInitObj(type, used: true, expression.Syntax);
			EmitBox(type, expression.Syntax);
			_builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue));
			EmitExpression(expression.ReferenceTypeReceiver, used);
			_builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(objectValue2));
			_builder.AdjustStack(-1);
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue));
			EmitExpression(expression.ValueTypeReceiver, used);
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue2));
		}

		private void EmitConditionalAccess(BoundLoweredConditionalAccess conditional, bool used)
		{
			if (TypeSymbolExtensions.IsBooleanType(conditional.ReceiverOrCondition.Type))
			{
				object objectValue = RuntimeHelpers.GetObjectValue(new object());
				object lazyDest = RuntimeHelpers.GetObjectValue(new object());
				EmitCondBranch(conditional.ReceiverOrCondition, ref lazyDest, sense: true);
				if (conditional.WhenNullOpt != null)
				{
					EmitExpression(conditional.WhenNullOpt, used);
				}
				_builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(objectValue));
				if (used)
				{
					_builder.AdjustStack(-1);
				}
				_builder.MarkLabel(RuntimeHelpers.GetObjectValue(lazyDest));
				EmitExpression(conditional.WhenNotNull, used);
				_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue));
				return;
			}
			LocalDefinition localDefinition = null;
			LocalDefinition localDefinition2 = null;
			object objectValue2 = RuntimeHelpers.GetObjectValue(new object());
			object objectValue3 = RuntimeHelpers.GetObjectValue(new object());
			BoundExpression receiverOrCondition = conditional.ReceiverOrCondition;
			TypeSymbol type = receiverOrCondition.Type;
			int num;
			if (!conditional.CaptureReceiver)
			{
				if (type.IsReferenceType)
				{
					num = ((type.TypeKind == TypeKind.TypeParameter) ? 1 : 0);
					if (num != 0)
					{
						goto IL_00f7;
					}
				}
				else
				{
					num = 0;
				}
				EmitExpression(receiverOrCondition, used: true);
				if (!type.IsReferenceType)
				{
					EmitBox(type, receiverOrCondition.Syntax);
				}
				goto IL_0202;
			}
			num = 1;
			goto IL_00f7;
			IL_00f7:
			localDefinition = EmitReceiverRef(receiverOrCondition, !type.IsReferenceType, AddressKind.ReadOnly);
			if (!type.IsReferenceType)
			{
				if (localDefinition == null)
				{
					EmitInitObj(type, used: true, receiverOrCondition.Syntax);
					EmitBox(type, receiverOrCondition.Syntax);
					_builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue2));
					EmitLoadIndirect(type, receiverOrCondition.Syntax);
					localDefinition2 = AllocateTemp(type, receiverOrCondition.Syntax);
					_builder.EmitLocalStore(localDefinition2);
					_builder.EmitLocalAddress(localDefinition2);
					_builder.EmitLocalLoad(localDefinition2);
					EmitBox(type, receiverOrCondition.Syntax);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Dup);
					EmitLoadIndirect(type, receiverOrCondition.Syntax);
					EmitBox(type, receiverOrCondition.Syntax);
				}
			}
			else
			{
				_builder.EmitOpCode(ILOpCode.Dup);
			}
			goto IL_0202;
			IL_0202:
			_builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue2));
			if (num != 0)
			{
				_builder.EmitOpCode(ILOpCode.Pop);
			}
			if (conditional.WhenNullOpt != null)
			{
				EmitExpression(conditional.WhenNullOpt, used);
			}
			_builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(objectValue3));
			if (used)
			{
				_builder.AdjustStack(-1);
			}
			if (num != 0)
			{
				_builder.AdjustStack(1);
			}
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue2));
			if (num == 0)
			{
				localDefinition = EmitReceiverRef(receiverOrCondition, !type.IsReferenceType, AddressKind.ReadOnly);
			}
			EmitExpression(conditional.WhenNotNull, used);
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue3));
			if (localDefinition2 != null)
			{
				FreeTemp(localDefinition2);
			}
			if (localDefinition != null)
			{
				FreeTemp(localDefinition);
			}
		}

		private void EmitComplexConditionalAccessReceiverAddress(BoundComplexConditionalAccessReceiver expression)
		{
			TypeSymbol type = expression.Type;
			object objectValue = RuntimeHelpers.GetObjectValue(new object());
			object objectValue2 = RuntimeHelpers.GetObjectValue(new object());
			EmitInitObj(type, used: true, expression.Syntax);
			EmitBox(type, expression.Syntax);
			_builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue));
			EmitAddress(expression.ReferenceTypeReceiver, AddressKind.ReadOnly);
			_builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(objectValue2));
			_builder.AdjustStack(-1);
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue));
			EmitReceiverRef(expression.ValueTypeReceiver, isAccessConstrained: true, AddressKind.ReadOnly);
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue2));
		}

		private void EmitDelegateCreationExpression(BoundDelegateCreationExpression expression, bool used)
		{
			MethodSymbol method = expression.Method;
			EmitDelegateCreation(expression.ReceiverOpt, method, expression.Type, used, expression.Syntax);
		}

		private void EmitLocalLoad(BoundLocal local, bool used)
		{
			if (IsStackLocal(local.LocalSymbol))
			{
				EmitPopIfUnused(used);
			}
			else
			{
				if (!used)
				{
					return;
				}
				_builder.EmitLocalLoad(GetLocal(local));
			}
			if (used && local.LocalSymbol.IsByRef)
			{
				EmitLoadIndirect(local.Type, local.Syntax);
			}
		}

		private void EmitDelegateCreation(BoundExpression receiver, MethodSymbol method, TypeSymbol delegateType, bool used, SyntaxNode syntaxNode)
		{
			bool flag = receiver == null || method.IsShared;
			if (!used)
			{
				if (!flag)
				{
					EmitExpression(receiver, used: false);
				}
				return;
			}
			if (flag)
			{
				_builder.EmitNullConstant();
			}
			else
			{
				EmitExpression(receiver, used: true);
				if (!TypeSymbolExtensions.IsVerifierReference(receiver.Type))
				{
					EmitBox(receiver.Type, receiver.Syntax);
				}
			}
			if (SymbolExtensions.IsMetadataVirtual(method) && !TypeSymbolExtensions.IsDelegateType(method.ContainingType) && !receiver.SuppressVirtualCalls)
			{
				_builder.EmitOpCode(ILOpCode.Dup);
				_builder.EmitOpCode(ILOpCode.Ldvirtftn);
			}
			else
			{
				_builder.EmitOpCode(ILOpCode.Ldftn);
			}
			MethodSymbol methodSymbol = method.CallsiteReducedFromMethod ?? method;
			if (!flag && TypeSymbolExtensions.IsNullableType(methodSymbol.ContainingType))
			{
				methodSymbol = method.OverriddenMethod;
			}
			EmitSymbolToken(methodSymbol, syntaxNode);
			MethodSymbol symbol = (MethodSymbol)delegateType.GetMembers(".ctor").Single();
			_builder.EmitOpCode(ILOpCode.Newobj, -1);
			EmitSymbolToken(symbol, syntaxNode);
		}

		private void EmitMeOrMyClassReferenceExpression(BoundExpression thisRef)
		{
			TypeSymbol type = thisRef.Type;
			_builder.EmitOpCode(ILOpCode.Ldarg_0);
			if (type.IsValueType)
			{
				_builder.EmitOpCode(ILOpCode.Ldobj);
				EmitSymbolToken(thisRef.Type, thisRef.Syntax);
			}
		}

		private void EmitPseudoVariableValue(BoundPseudoVariable expression, bool used)
		{
			EmitExpression(expression.EmitExpressions.GetValue(expression, _diagnostics), used);
		}

		private void EmitSequenceExpression(BoundSequence sequence, bool used)
		{
			bool flag = !sequence.Locals.IsEmpty;
			if (flag)
			{
				_builder.OpenLocalScope();
				ImmutableArray<LocalSymbol>.Enumerator enumerator = sequence.Locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					DefineLocal(current, sequence.Syntax);
				}
			}
			EmitSideEffects(sequence.SideEffects);
			EmitExpression(sequence.ValueOpt, used);
			if (flag)
			{
				_builder.CloseLocalScope();
				ImmutableArray<LocalSymbol>.Enumerator enumerator2 = sequence.Locals.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					LocalSymbol current2 = enumerator2.Current;
					FreeLocal(current2);
				}
			}
		}

		private void EmitSideEffects(ImmutableArray<BoundExpression> sideEffects)
		{
			if (!sideEffects.IsDefaultOrEmpty)
			{
				ImmutableArray<BoundExpression>.Enumerator enumerator = sideEffects.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundExpression current = enumerator.Current;
					EmitExpression(current, used: false);
				}
			}
		}

		private void EmitExpressions(ImmutableArray<BoundExpression> expressions, bool used)
		{
			for (int i = 0; i < expressions.Length; i++)
			{
				BoundExpression expression = expressions[i];
				EmitExpression(expression, used);
			}
		}

		private void EmitArguments(ImmutableArray<BoundExpression> arguments, ImmutableArray<ParameterSymbol> parameters)
		{
			int num = arguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				BoundExpression expression = arguments[i];
				if (parameters[i].IsByRef)
				{
					EmitAddress(expression, AddressKind.Writeable);
				}
				else
				{
					EmitExpression(expression, used: true);
				}
			}
		}

		private void EmitArrayElementLoad(BoundArrayAccess arrayAccess, bool used)
		{
			EmitExpression(arrayAccess.Expression, used: true);
			EmitExpressions(arrayAccess.Indices, used: true);
			if (((ArrayTypeSymbol)arrayAccess.Expression.Type).IsSZArray)
			{
				TypeSymbol typeSymbol = arrayAccess.Type;
				if (TypeSymbolExtensions.IsEnumType(typeSymbol))
				{
					typeSymbol = ((NamedTypeSymbol)typeSymbol).EnumUnderlyingType;
				}
				switch (typeSymbol.PrimitiveTypeCode)
				{
				case Microsoft.Cci.PrimitiveTypeCode.Int8:
					_builder.EmitOpCode(ILOpCode.Ldelem_i1);
					break;
				case Microsoft.Cci.PrimitiveTypeCode.Boolean:
				case Microsoft.Cci.PrimitiveTypeCode.UInt8:
					_builder.EmitOpCode(ILOpCode.Ldelem_u1);
					break;
				case Microsoft.Cci.PrimitiveTypeCode.Int16:
					_builder.EmitOpCode(ILOpCode.Ldelem_i2);
					break;
				case Microsoft.Cci.PrimitiveTypeCode.Char:
				case Microsoft.Cci.PrimitiveTypeCode.UInt16:
					_builder.EmitOpCode(ILOpCode.Ldelem_u2);
					break;
				case Microsoft.Cci.PrimitiveTypeCode.Int32:
					_builder.EmitOpCode(ILOpCode.Ldelem_i4);
					break;
				case Microsoft.Cci.PrimitiveTypeCode.UInt32:
					_builder.EmitOpCode(ILOpCode.Ldelem_u4);
					break;
				case Microsoft.Cci.PrimitiveTypeCode.Int64:
				case Microsoft.Cci.PrimitiveTypeCode.UInt64:
					_builder.EmitOpCode(ILOpCode.Ldelem_i8);
					break;
				case Microsoft.Cci.PrimitiveTypeCode.IntPtr:
				case Microsoft.Cci.PrimitiveTypeCode.Pointer:
				case Microsoft.Cci.PrimitiveTypeCode.UIntPtr:
					_builder.EmitOpCode(ILOpCode.Ldelem_i);
					break;
				case Microsoft.Cci.PrimitiveTypeCode.Float32:
					_builder.EmitOpCode(ILOpCode.Ldelem_r4);
					break;
				case Microsoft.Cci.PrimitiveTypeCode.Float64:
					_builder.EmitOpCode(ILOpCode.Ldelem_r8);
					break;
				default:
					if (TypeSymbolExtensions.IsVerifierReference(typeSymbol))
					{
						_builder.EmitOpCode(ILOpCode.Ldelem_ref);
						break;
					}
					if (used)
					{
						_builder.EmitOpCode(ILOpCode.Ldelem);
					}
					else
					{
						if (typeSymbol.TypeKind == TypeKind.TypeParameter)
						{
							_builder.EmitOpCode(ILOpCode.Readonly);
						}
						_builder.EmitOpCode(ILOpCode.Ldelema);
					}
					EmitSymbolToken(typeSymbol, arrayAccess.Expression.Syntax);
					break;
				}
			}
			else
			{
				_builder.EmitArrayElementLoad(_module.Translate((ArrayTypeSymbol)arrayAccess.Expression.Type), arrayAccess.Expression.Syntax, _diagnostics);
			}
			EmitPopIfUnused(used);
		}

		private void EmitFieldLoad(BoundFieldAccess fieldAccess, bool used)
		{
			FieldSymbol fieldSymbol = fieldAccess.FieldSymbol;
			if (!used && !fieldSymbol.IsShared && TypeSymbolExtensions.IsVerifierValue(fieldAccess.ReceiverOpt.Type))
			{
				EmitExpression(fieldAccess.ReceiverOpt, used: false);
				return;
			}
			SpecialType specialType = fieldSymbol.Type.SpecialType;
			if (fieldSymbol.IsConst && specialType != SpecialType.System_Decimal && specialType != SpecialType.System_DateTime)
			{
				throw ExceptionUtilities.Unreachable;
			}
			if (fieldSymbol.IsShared)
			{
				EmitStaticFieldLoad(fieldSymbol, used, fieldAccess.Syntax);
			}
			else
			{
				EmitInstanceFieldLoad(fieldAccess, used);
			}
		}

		private void EmitDupExpression(BoundDup dupExpression, bool used)
		{
			if (!dupExpression.IsReference)
			{
				if (used)
				{
					_builder.EmitOpCode(ILOpCode.Dup);
				}
			}
			else
			{
				_builder.EmitOpCode(ILOpCode.Dup);
				EmitLoadIndirect(dupExpression.Type, dupExpression.Syntax);
				EmitPopIfUnused(used);
			}
		}

		private void EmitStaticFieldLoad(FieldSymbol field, bool used, SyntaxNode syntaxNode)
		{
			_builder.EmitOpCode(ILOpCode.Ldsfld);
			EmitSymbolToken(field, syntaxNode);
			EmitPopIfUnused(used);
		}

		private void EmitInstanceFieldLoad(BoundFieldAccess fieldAccess, bool used)
		{
			FieldSymbol fieldSymbol = fieldAccess.FieldSymbol;
			BoundExpression receiverOpt = fieldAccess.ReceiverOpt;
			if (FieldLoadMustUseRef(receiverOpt) || FieldLoadPrefersRef(receiverOpt))
			{
				if (!EmitFieldLoadReceiverAddress(receiverOpt))
				{
					EmitReceiverRef(receiverOpt, isAccessConstrained: false, AddressKind.Immutable);
				}
			}
			else
			{
				EmitExpression(receiverOpt, used: true);
			}
			_builder.EmitOpCode(ILOpCode.Ldfld);
			EmitSymbolToken(fieldSymbol, fieldAccess.Syntax);
			EmitPopIfUnused(used);
		}

		private bool EmitFieldLoadReceiverAddress(BoundExpression receiver)
		{
			if (receiver == null || receiver.Type.IsReferenceType)
			{
				return false;
			}
			if (receiver.Kind == BoundKind.DirectCast && IsUnboxingDirectCast((BoundDirectCast)receiver))
			{
				EmitExpression(((BoundDirectCast)receiver).Operand, used: true);
				_builder.EmitOpCode(ILOpCode.Unbox);
				EmitSymbolToken(receiver.Type, receiver.Syntax);
				return true;
			}
			if (receiver.Kind == BoundKind.FieldAccess)
			{
				BoundFieldAccess boundFieldAccess = (BoundFieldAccess)receiver;
				FieldSymbol fieldSymbol = boundFieldAccess.FieldSymbol;
				if (!fieldSymbol.IsShared && EmitFieldLoadReceiverAddress(boundFieldAccess.ReceiverOpt))
				{
					_builder.EmitOpCode(ILOpCode.Ldflda);
					EmitSymbolToken(fieldSymbol, boundFieldAccess.Syntax);
					return true;
				}
			}
			return false;
		}

		private bool FieldLoadPrefersRef(BoundExpression receiver)
		{
			if (!TypeSymbolExtensions.IsVerifierValue(receiver.Type))
			{
				return true;
			}
			if (receiver.Kind == BoundKind.DirectCast && IsUnboxingDirectCast((BoundDirectCast)receiver))
			{
				return true;
			}
			if (!HasHome(receiver))
			{
				return false;
			}
			switch (receiver.Kind)
			{
			case BoundKind.Parameter:
				return ((BoundParameter)receiver).ParameterSymbol.IsByRef;
			case BoundKind.Local:
				return ((BoundLocal)receiver).LocalSymbol.IsByRef;
			case BoundKind.Sequence:
				return FieldLoadPrefersRef(((BoundSequence)receiver).ValueOpt);
			case BoundKind.FieldAccess:
			{
				BoundFieldAccess boundFieldAccess = (BoundFieldAccess)receiver;
				return boundFieldAccess.FieldSymbol.IsShared || FieldLoadPrefersRef(boundFieldAccess.ReceiverOpt);
			}
			default:
				return true;
			}
		}

		internal static bool FieldLoadMustUseRef(BoundExpression expr)
		{
			if (TypeSymbolExtensions.IsEnumType(expr.Type))
			{
				return true;
			}
			return TypeSymbolExtensions.IsTypeParameter(expr.Type);
		}

		private int ParameterSlot(BoundParameter parameter)
		{
			ParameterSymbol parameterSymbol = parameter.ParameterSymbol;
			int num = parameterSymbol.Ordinal;
			if (!parameterSymbol.ContainingSymbol.IsShared)
			{
				num++;
			}
			return num;
		}

		private void EmitParameterLoad(BoundParameter parameter)
		{
			int argNumber = ParameterSlot(parameter);
			_builder.EmitLoadArgumentOpcode(argNumber);
			if (parameter.ParameterSymbol.IsByRef)
			{
				TypeSymbol type = parameter.ParameterSymbol.Type;
				EmitLoadIndirect(type, parameter.Syntax);
			}
		}

		private void EmitLoadIndirect(TypeSymbol type, SyntaxNode syntaxNode)
		{
			if (TypeSymbolExtensions.IsEnumType(type))
			{
				type = ((NamedTypeSymbol)type).EnumUnderlyingType;
			}
			switch (type.PrimitiveTypeCode)
			{
			case Microsoft.Cci.PrimitiveTypeCode.Int8:
				_builder.EmitOpCode(ILOpCode.Ldind_i1);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Boolean:
			case Microsoft.Cci.PrimitiveTypeCode.UInt8:
				_builder.EmitOpCode(ILOpCode.Ldind_u1);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Int16:
				_builder.EmitOpCode(ILOpCode.Ldind_i2);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Char:
			case Microsoft.Cci.PrimitiveTypeCode.UInt16:
				_builder.EmitOpCode(ILOpCode.Ldind_u2);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Int32:
				_builder.EmitOpCode(ILOpCode.Ldind_i4);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.UInt32:
				_builder.EmitOpCode(ILOpCode.Ldind_u4);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Int64:
			case Microsoft.Cci.PrimitiveTypeCode.UInt64:
				_builder.EmitOpCode(ILOpCode.Ldind_i8);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.IntPtr:
			case Microsoft.Cci.PrimitiveTypeCode.Pointer:
			case Microsoft.Cci.PrimitiveTypeCode.UIntPtr:
				_builder.EmitOpCode(ILOpCode.Ldind_i);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Float32:
				_builder.EmitOpCode(ILOpCode.Ldind_r4);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Float64:
				_builder.EmitOpCode(ILOpCode.Ldind_r8);
				return;
			}
			if (TypeSymbolExtensions.IsVerifierReference(type))
			{
				_builder.EmitOpCode(ILOpCode.Ldind_ref);
				return;
			}
			_builder.EmitOpCode(ILOpCode.Ldobj);
			EmitSymbolToken(type, syntaxNode);
		}

		private bool CanUseCallOnRefTypeReceiver(BoundExpression receiver)
		{
			if (TypeSymbolExtensions.IsTypeParameter(receiver.Type))
			{
				return false;
			}
			ConstantValue constantValueOpt = receiver.ConstantValueOpt;
			if ((object)constantValueOpt != null)
			{
				return !constantValueOpt.IsNothing;
			}
			switch (receiver.Kind)
			{
			case BoundKind.ArrayCreation:
				return true;
			case BoundKind.ObjectCreationExpression:
				return true;
			case BoundKind.DirectCast:
			{
				BoundExpression operand = ((BoundDirectCast)receiver).Operand;
				if (!TypeSymbolExtensions.IsVerifierReference(operand.Type))
				{
					return true;
				}
				return CanUseCallOnRefTypeReceiver(operand);
			}
			case BoundKind.MeReference:
			case BoundKind.MyBaseReference:
			case BoundKind.MyClassReference:
				return true;
			case BoundKind.AddressOfOperator:
			case BoundKind.DelegateCreationExpression:
				return true;
			case BoundKind.Sequence:
			{
				BoundExpression valueOpt = ((BoundSequence)receiver).ValueOpt;
				return valueOpt != null && CanUseCallOnRefTypeReceiver(valueOpt);
			}
			case BoundKind.AssignmentOperator:
			{
				BoundExpression right = ((BoundAssignmentOperator)receiver).Right;
				return CanUseCallOnRefTypeReceiver(right);
			}
			case BoundKind.GetType:
				return true;
			case BoundKind.FieldAccess:
				return ((BoundFieldAccess)receiver).FieldSymbol.IsCapturedFrame;
			case BoundKind.ConditionalAccessReceiverPlaceholder:
			case BoundKind.ComplexConditionalAccessReceiver:
				return true;
			default:
				return false;
			}
		}

		private bool IsMeReceiver(BoundExpression receiver)
		{
			switch (receiver.Kind)
			{
			case BoundKind.MeReference:
			case BoundKind.MyClassReference:
				return true;
			case BoundKind.Sequence:
			{
				BoundExpression valueOpt = ((BoundSequence)receiver).ValueOpt;
				return IsMeReceiver(valueOpt);
			}
			default:
				return false;
			}
		}

		private void EmitCallExpression(BoundCall call, UseKind useKind)
		{
			MethodSymbol methodSymbol = call.Method;
			BoundExpression receiverOpt = call.ReceiverOpt;
			if (SymbolExtensions.IsDefaultValueTypeConstructor(methodSymbol))
			{
				EmitInitObjOnTarget(receiverOpt);
				return;
			}
			ImmutableArray<BoundExpression> arguments = call.Arguments;
			int num = ((!methodSymbol.IsSub) ? 1 : 0) - arguments.Length;
			LocalDefinition temp = null;
			CallKind callKind;
			if (methodSymbol.IsShared)
			{
				callKind = CallKind.Call;
			}
			else
			{
				num--;
				TypeSymbol type = receiverOpt.Type;
				if (TypeSymbolExtensions.IsVerifierReference(type))
				{
					temp = EmitReceiverRef(receiverOpt, isAccessConstrained: false, AddressKind.ReadOnly);
					callKind = ((!receiverOpt.SuppressVirtualCalls && (SymbolExtensions.IsMetadataVirtual(methodSymbol) || !CanUseCallOnRefTypeReceiver(receiverOpt))) ? CallKind.CallVirt : CallKind.Call);
				}
				else if (TypeSymbolExtensions.IsVerifierValue(type))
				{
					NamedTypeSymbol containingType = methodSymbol.ContainingType;
					if (TypeSymbolExtensions.IsVerifierValue(containingType) && MayUseCallForStructMethod(methodSymbol))
					{
						temp = EmitReceiverRef(receiverOpt, isAccessConstrained: false, IsMeReceiver(receiverOpt) ? AddressKind.ReadOnly : AddressKind.Writeable);
						callKind = CallKind.Call;
					}
					else if (SymbolExtensions.IsMetadataVirtual(methodSymbol))
					{
						temp = EmitReceiverRef(receiverOpt, isAccessConstrained: true, AddressKind.ReadOnly);
						callKind = CallKind.ConstrainedCallVirt;
						if (TypeSymbolExtensions.IsVerifierValue(containingType))
						{
							while ((object)methodSymbol.OverriddenMethod != null)
							{
								methodSymbol = methodSymbol.OverriddenMethod;
							}
						}
					}
					else
					{
						EmitExpression(receiverOpt, used: true);
						EmitBox(type, receiverOpt.Syntax);
						callKind = CallKind.Call;
					}
				}
				else
				{
					callKind = ((type.IsReferenceType && (receiverOpt.Kind == BoundKind.ConditionalAccessReceiverPlaceholder || !AllowedToTakeRef(receiverOpt, AddressKind.ReadOnly))) ? CallKind.CallVirt : CallKind.ConstrainedCallVirt);
					temp = EmitReceiverRef(receiverOpt, callKind == CallKind.ConstrainedCallVirt, AddressKind.ReadOnly);
				}
			}
			if (callKind == CallKind.CallVirt && (object)methodSymbol.ContainingModule == _method.ContainingModule)
			{
				if (IsMeReceiver(receiverOpt) && methodSymbol.ContainingType.IsNotInheritable)
				{
					callKind = CallKind.Call;
				}
				else if (methodSymbol.IsMetadataFinal && CanUseCallOnRefTypeReceiver(receiverOpt))
				{
					callKind = CallKind.Call;
				}
			}
			EmitArguments(arguments, methodSymbol.Parameters);
			switch (callKind)
			{
			case CallKind.Call:
				_builder.EmitOpCode(ILOpCode.Call, num);
				break;
			case CallKind.CallVirt:
				_builder.EmitOpCode(ILOpCode.Callvirt, num);
				break;
			case CallKind.ConstrainedCallVirt:
				_builder.EmitOpCode(ILOpCode.Constrained);
				EmitSymbolToken(receiverOpt.Type, receiverOpt.Syntax);
				_builder.EmitOpCode(ILOpCode.Callvirt, num);
				break;
			}
			EmitSymbolToken(methodSymbol, call.Syntax);
			if (!methodSymbol.IsSub)
			{
				EmitPopIfUnused(useKind != UseKind.Unused);
			}
			else if (_ilEmitStyle == ILEmitStyle.Debug)
			{
				_builder.EmitOpCode(ILOpCode.Nop);
			}
			if (useKind == UseKind.UsedAsValue && methodSymbol.ReturnsByRef)
			{
				EmitLoadIndirect(methodSymbol.ReturnType, call.Syntax);
			}
			else
			{
				_ = 2;
			}
			FreeOptTemp(temp);
			if (!_checkCallsForUnsafeJITOptimization || !methodSymbol.IsDefinition)
			{
				return;
			}
			bool flag = false;
			if ((object)methodSymbol.ContainingSymbol == _module.Compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_ErrObject))
			{
				if (string.Equals(methodSymbol.Name, "Raise", StringComparison.Ordinal))
				{
					flag = true;
				}
			}
			else if ((object)methodSymbol.ContainingSymbol == _module.Compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_CompilerServices_ProjectData))
			{
				if (string.Equals(methodSymbol.Name, "EndApp", StringComparison.Ordinal))
				{
					flag = true;
				}
			}
			else if ((object)methodSymbol.ContainingSymbol == _module.Compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_ApplicationServices_ApplicationBase))
			{
				if (string.Equals(methodSymbol.Name, "Info", StringComparison.Ordinal))
				{
					flag = true;
				}
			}
			else if ((object)methodSymbol.ContainingSymbol == _module.Compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_ApplicationServices_WindowsFormsApplicationBase))
			{
				if (string.Equals(methodSymbol.Name, "Run", StringComparison.Ordinal))
				{
					flag = true;
				}
			}
			else if ((object)methodSymbol.ContainingSymbol == _module.Compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_FileSystem))
			{
				string name = methodSymbol.Name;
				switch (_003CPrivateImplementationDetails_003E.ComputeStringHash(name))
				{
				case 1315521844u:
					if (EmbeddedOperators.CompareString(name, "Dir", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 3477472001u:
					if (EmbeddedOperators.CompareString(name, "EOF", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 1039821500u:
					if (EmbeddedOperators.CompareString(name, "FileAttr", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 3475451097u:
					if (EmbeddedOperators.CompareString(name, "FileClose", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 1882556202u:
					if (EmbeddedOperators.CompareString(name, "FileCopy", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 3969019597u:
					if (EmbeddedOperators.CompareString(name, "FileGet", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 773332930u:
					if (EmbeddedOperators.CompareString(name, "FileGetObject", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 3669927119u:
					if (EmbeddedOperators.CompareString(name, "FileOpen", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 934190080u:
					if (EmbeddedOperators.CompareString(name, "FilePut", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 1451687479u:
					if (EmbeddedOperators.CompareString(name, "FilePutObject", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 3843894389u:
					if (EmbeddedOperators.CompareString(name, "FileWidth", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 2821449333u:
					if (EmbeddedOperators.CompareString(name, "FreeFile", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 2677268763u:
					if (EmbeddedOperators.CompareString(name, "Input", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 2277698462u:
					if (EmbeddedOperators.CompareString(name, "InputString", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 737074361u:
					if (EmbeddedOperators.CompareString(name, "Kill", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 86024517u:
					if (EmbeddedOperators.CompareString(name, "LineInput", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 2782757437u:
					if (EmbeddedOperators.CompareString(name, "Loc", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 1907633506u:
					if (EmbeddedOperators.CompareString(name, "Lock", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 3240872862u:
					if (EmbeddedOperators.CompareString(name, "LOF", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 3895594280u:
					if (EmbeddedOperators.CompareString(name, "Print", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 1233818802u:
					if (EmbeddedOperators.CompareString(name, "PrintLine", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 3355849203u:
					if (EmbeddedOperators.CompareString(name, "Rename", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 180921696u:
					if (EmbeddedOperators.CompareString(name, "Reset", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 3614577617u:
					if (EmbeddedOperators.CompareString(name, "Seek", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 3839124732u:
					if (EmbeddedOperators.CompareString(name, "SetAttr", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 2625214837u:
					if (EmbeddedOperators.CompareString(name, "Unlock", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 1717336572u:
					if (EmbeddedOperators.CompareString(name, "Write", TextCompare: false) != 0)
					{
						break;
					}
					goto IL_07dd;
				case 2063185886u:
					{
						if (EmbeddedOperators.CompareString(name, "WriteLine", TextCompare: false) != 0)
						{
							break;
						}
						goto IL_07dd;
					}
					IL_07dd:
					flag = true;
					break;
				}
			}
			if (flag)
			{
				_checkCallsForUnsafeJITOptimization = false;
				_module.SetDisableJITOptimization(_method);
			}
		}

		private bool MayUseCallForStructMethod(MethodSymbol method)
		{
			if (!SymbolExtensions.IsMetadataVirtual(method))
			{
				return true;
			}
			MethodSymbol overriddenMethod = method.OverriddenMethod;
			if ((object)overriddenMethod == null || overriddenMethod.IsMustOverride)
			{
				return true;
			}
			NamedTypeSymbol containingType = method.ContainingType;
			return TypeSymbolExtensions.IsIntrinsicType(containingType) || TypeSymbolExtensions.IsRestrictedType(containingType);
		}

		private void EmitTypeOfExpression(BoundTypeOf expression, bool used, bool optimize = false)
		{
			BoundExpression operand = expression.Operand;
			EmitExpression(operand, used: true);
			if (used)
			{
				_ = operand.Type;
				TypeSymbol targetType = expression.TargetType;
				_builder.EmitOpCode(ILOpCode.Isinst);
				EmitSymbolToken(targetType, expression.Syntax);
				if (!optimize)
				{
					_builder.EmitOpCode(ILOpCode.Ldnull);
					if (expression.IsTypeOfIsNotExpression)
					{
						_builder.EmitOpCode(ILOpCode.Ceq);
					}
					else
					{
						_builder.EmitOpCode(ILOpCode.Cgt_un);
					}
				}
			}
			EmitPopIfUnused(used);
		}

		private void EmitTernaryConditionalExpression(BoundTernaryConditionalExpression expr, bool used)
		{
			object lazyDest = RuntimeHelpers.GetObjectValue(new object());
			object objectValue = RuntimeHelpers.GetObjectValue(new object());
			EmitCondBranch(expr.Condition, ref lazyDest, sense: true);
			EmitExpression(expr.WhenFalse, used);
			TypeSymbol typeSymbol = StackMergeType(expr.WhenFalse);
			if (used && IsVarianceCast(expr.Type, typeSymbol))
			{
				EmitStaticCast(expr.Type, expr.Syntax);
				typeSymbol = expr.Type;
			}
			_builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(objectValue));
			if (used)
			{
				_builder.AdjustStack(-1);
			}
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(lazyDest));
			EmitExpression(expr.WhenTrue, used);
			if (used)
			{
				TypeSymbol typeSymbol2 = StackMergeType(expr.WhenTrue);
				if (IsVarianceCast(expr.Type, typeSymbol2))
				{
					EmitStaticCast(expr.Type, expr.Syntax);
					typeSymbol2 = expr.Type;
				}
				else if (TypeSymbolExtensions.IsInterfaceType(expr.Type) && !TypeSymbol.Equals(expr.Type, typeSymbol, TypeCompareKind.ConsiderEverything) && !TypeSymbol.Equals(expr.Type, typeSymbol2, TypeCompareKind.ConsiderEverything))
				{
					EmitStaticCast(expr.Type, expr.Syntax);
				}
			}
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue));
		}

		private void EmitBinaryConditionalExpression(BoundBinaryConditionalExpression expr, bool used)
		{
			EmitExpression(expr.TestExpression, used: true);
			TypeSymbol typeSymbol = StackMergeType(expr.TestExpression);
			if (used)
			{
				if (IsVarianceCast(expr.Type, typeSymbol))
				{
					EmitStaticCast(expr.Type, expr.Syntax);
					typeSymbol = expr.Type;
				}
				_builder.EmitOpCode(ILOpCode.Dup);
			}
			if (TypeSymbolExtensions.IsTypeParameter(expr.Type))
			{
				EmitBox(expr.Type, expr.TestExpression.Syntax);
			}
			object objectValue = RuntimeHelpers.GetObjectValue(new object());
			_builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue));
			if (used)
			{
				_builder.EmitOpCode(ILOpCode.Pop);
			}
			EmitExpression(expr.ElseExpression, used);
			if (used)
			{
				TypeSymbol typeSymbol2 = StackMergeType(expr.ElseExpression);
				if (IsVarianceCast(expr.Type, typeSymbol2))
				{
					EmitStaticCast(expr.Type, expr.Syntax);
					typeSymbol2 = expr.Type;
				}
				else if (TypeSymbolExtensions.IsInterfaceType(expr.Type) && !TypeSymbol.Equals(expr.Type, typeSymbol, TypeCompareKind.ConsiderEverything) && !TypeSymbol.Equals(expr.Type, typeSymbol2, TypeCompareKind.ConsiderEverything))
				{
					EmitStaticCast(expr.Type, expr.Syntax);
				}
			}
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue));
		}

		private TypeSymbol StackMergeType(BoundExpression expr)
		{
			if (!TypeSymbolExtensions.IsArrayType(expr.Type) && !TypeSymbolExtensions.IsInterfaceType(expr.Type) && !TypeSymbolExtensions.IsDelegateType(expr.Type))
			{
				return expr.Type;
			}
			switch (expr.Kind)
			{
			case BoundKind.DirectCast:
			{
				BoundDirectCast boundDirectCast = (BoundDirectCast)expr;
				if (Conversions.IsWideningConversion(boundDirectCast.ConversionKind))
				{
					return StackMergeType(boundDirectCast.Operand);
				}
				break;
			}
			case BoundKind.TryCast:
			{
				BoundTryCast boundTryCast = (BoundTryCast)expr;
				if (Conversions.IsWideningConversion(boundTryCast.ConversionKind))
				{
					return StackMergeType(boundTryCast.Operand);
				}
				break;
			}
			case BoundKind.AssignmentOperator:
			{
				BoundAssignmentOperator boundAssignmentOperator = (BoundAssignmentOperator)expr;
				return StackMergeType(boundAssignmentOperator.Right);
			}
			case BoundKind.Sequence:
			{
				BoundSequence boundSequence = (BoundSequence)expr;
				return StackMergeType(boundSequence.ValueOpt);
			}
			case BoundKind.Local:
			{
				BoundLocal boundLocal = (BoundLocal)expr;
				if (IsStackLocal(boundLocal.LocalSymbol))
				{
					return null;
				}
				break;
			}
			case BoundKind.Dup:
				return null;
			}
			return expr.Type;
		}

		private static bool IsVarianceCast(TypeSymbol toType, TypeSymbol fromType)
		{
			if (TypeSymbol.Equals(toType, fromType, TypeCompareKind.ConsiderEverything))
			{
				return false;
			}
			if ((object)fromType == null)
			{
				return true;
			}
			if (TypeSymbolExtensions.IsArrayType(toType))
			{
				return IsVarianceCast(((ArrayTypeSymbol)toType).ElementType, ((ArrayTypeSymbol)fromType).ElementType);
			}
			return (TypeSymbolExtensions.IsDelegateType(toType) && !TypeSymbol.Equals(toType, fromType, TypeCompareKind.ConsiderEverything)) || (TypeSymbolExtensions.IsInterfaceType(toType) && TypeSymbolExtensions.IsInterfaceType(fromType) && !fromType.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.ContainsKey((NamedTypeSymbol)toType));
		}

		private void EmitStaticCast(TypeSymbol toType, SyntaxNode syntax)
		{
			LocalDefinition localDefinition = AllocateTemp(toType, syntax);
			_builder.EmitLocalStore(localDefinition);
			_builder.EmitLocalLoad(localDefinition);
			FreeTemp(localDefinition);
		}

		private void EmitArrayCreationExpression(BoundArrayCreation expression, bool used)
		{
			ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)expression.Type;
			EmitExpressions(expression.Bounds, used: true);
			if (arrayTypeSymbol.IsSZArray)
			{
				_builder.EmitOpCode(ILOpCode.Newarr);
				EmitSymbolToken(arrayTypeSymbol.ElementType, expression.Syntax);
			}
			else
			{
				_builder.EmitArrayCreation(_module.Translate(arrayTypeSymbol), expression.Syntax, _diagnostics);
			}
			if (expression.InitializerOpt != null)
			{
				EmitArrayInitializers(arrayTypeSymbol, expression.InitializerOpt);
			}
			EmitPopIfUnused(used);
		}

		private void EmitArrayLengthExpression(BoundArrayLength expression, bool used)
		{
			EmitExpression(expression.Expression, used: true);
			_builder.EmitOpCode(ILOpCode.Ldlen);
			Microsoft.Cci.PrimitiveTypeCode primitiveTypeCode = expression.Type.PrimitiveTypeCode;
			Microsoft.Cci.PrimitiveTypeCode fromPredefTypeKind = (primitiveTypeCode.IsUnsigned() ? Microsoft.Cci.PrimitiveTypeCode.UIntPtr : Microsoft.Cci.PrimitiveTypeCode.IntPtr);
			_builder.EmitNumericConversion(fromPredefTypeKind, primitiveTypeCode, @checked: false);
			EmitPopIfUnused(used);
		}

		private void EmitObjectCreationExpression(BoundObjectCreationExpression expression, bool used)
		{
			if (BoundExpressionExtensions.IsDefaultValue(expression))
			{
				EmitInitObj(expression.Type, used, expression.Syntax);
				return;
			}
			_ = expression.ConstructorOpt;
			EmitNewObj(expression.ConstructorOpt, expression.Arguments, used, expression.Syntax);
		}

		private void EmitInitObj(TypeSymbol type, bool used, SyntaxNode syntaxNode)
		{
			if (used)
			{
				LocalDefinition localDefinition = AllocateTemp(type, syntaxNode);
				_builder.EmitLocalAddress(localDefinition);
				_builder.EmitOpCode(ILOpCode.Initobj);
				EmitSymbolToken(type, syntaxNode);
				_builder.EmitLocalLoad(localDefinition);
				FreeTemp(localDefinition);
			}
		}

		private void EmitNewObj(MethodSymbol constructor, ImmutableArray<BoundExpression> arguments, bool used, SyntaxNode syntaxNode)
		{
			EmitArguments(arguments, constructor.Parameters);
			_builder.EmitOpCode(ILOpCode.Newobj, ILOpCode.Newobj.StackPushCount() - arguments.Length);
			EmitSymbolToken(constructor, syntaxNode);
			EmitPopIfUnused(used);
		}

		private void EmitLoadDefaultValueOfTypeParameter(TypeSymbol type, bool used, SyntaxNode syntaxNode)
		{
			EmitLoadDefaultValueOfTypeFromNothingLiteral(type, used, syntaxNode);
		}

		private void EmitLoadDefaultValueOfTypeFromNothingLiteral(TypeSymbol type, bool used, SyntaxNode syntaxNode)
		{
			EmitInitObj(type, used, syntaxNode);
		}

		private void EmitStructConstructorCallOnTarget(MethodSymbol constructor, ImmutableArray<BoundExpression> arguments, BoundExpression target, VisualBasicSyntaxNode syntaxNode)
		{
			if (target.Kind == BoundKind.Local && IsStackLocal(((BoundLocal)target).LocalSymbol))
			{
				EmitNewObj(constructor, arguments, used: true, syntaxNode);
				return;
			}
			EmitAddress(target, AddressKind.Immutable);
			int stackAdjustment = -constructor.ParameterCount - 1;
			EmitArguments(arguments, constructor.Parameters);
			_builder.EmitOpCode(ILOpCode.Call, stackAdjustment);
			EmitSymbolToken(constructor, syntaxNode);
		}

		private void EmitInitObjOnTarget(BoundExpression target)
		{
			if (target.Kind == BoundKind.Local && IsStackLocal(((BoundLocal)target).LocalSymbol))
			{
				EmitInitObj(target.Type, used: true, target.Syntax);
				return;
			}
			EmitAddress(target, AddressKind.Immutable);
			_builder.EmitOpCode(ILOpCode.Initobj);
			EmitSymbolToken(target.Type, target.Syntax);
		}

		private void EmitConstantExpression(TypeSymbol type, ConstantValue constantValue, bool used, SyntaxNode syntaxNode)
		{
			if (used)
			{
				if ((object)type != null && type.TypeKind == TypeKind.TypeParameter && constantValue.IsNull)
				{
					EmitInitObj(type, used, syntaxNode);
				}
				else
				{
					_builder.EmitConstantValue(constantValue);
				}
			}
		}

		private void EmitConstantExpression(BoundExpression expression)
		{
			_builder.EmitConstantValue(expression.ConstantValueOpt);
		}

		private void EmitAssignmentExpression(BoundAssignmentOperator assignmentOperator, bool used)
		{
			if (!TryEmitAssignmentInPlace(assignmentOperator, used))
			{
				bool lhsUsesStack = EmitAssignmentPreamble(assignmentOperator.Left);
				EmitExpression(assignmentOperator.Right, used: true);
				LocalDefinition temp = EmitAssignmentDuplication(assignmentOperator, used, lhsUsesStack);
				EmitStore(assignmentOperator.Left);
				EmitAssignmentPostfix(temp);
			}
		}

		private bool TryEmitAssignmentInPlace(BoundAssignmentOperator assignmentOperator, bool used)
		{
			BoundExpression left = assignmentOperator.Left;
			if (used && !TargetIsNotOnHeap(left))
			{
				return false;
			}
			if (!SafeToGetWriteableReference(left))
			{
				return false;
			}
			BoundExpression right = assignmentOperator.Right;
			TypeSymbol type = right.Type;
			if (!TypeSymbolExtensions.IsTypeParameter(type) && (type.IsReferenceType || ((object)right.ConstantValueOpt != null && type.SpecialType != SpecialType.System_Decimal)))
			{
				return false;
			}
			if (BoundExpressionExtensions.IsDefaultValue(right))
			{
				InPlaceInit(left, used);
				return true;
			}
			if (right.Kind == BoundKind.ObjectCreationExpression && PartialCtorResultCannotEscape(left))
			{
				BoundObjectCreationExpression objCreation = (BoundObjectCreationExpression)right;
				InPlaceCtorCall(left, objCreation, used);
				return true;
			}
			return false;
		}

		private bool SafeToGetWriteableReference(BoundExpression left)
		{
			if (AllowedToTakeRef(left, AddressKind.Writeable))
			{
				if (left.Kind == BoundKind.ArrayAccess)
				{
					return left.Type.TypeKind != TypeKind.TypeParameter;
				}
				return true;
			}
			return false;
		}

		private void InPlaceInit(BoundExpression target, bool used)
		{
			EmitAddress(target, AddressKind.Writeable);
			_builder.EmitOpCode(ILOpCode.Initobj);
			EmitSymbolToken(target.Type, target.Syntax);
			if (used)
			{
				EmitExpression(target, used);
			}
		}

		private void InPlaceCtorCall(BoundExpression target, BoundObjectCreationExpression objCreation, bool used)
		{
			EmitAddress(target, AddressKind.Writeable);
			MethodSymbol constructorOpt = objCreation.ConstructorOpt;
			EmitArguments(objCreation.Arguments, constructorOpt.Parameters);
			int num = constructorOpt.ParameterCount + 1;
			_builder.EmitOpCode(ILOpCode.Call, -num);
			EmitSymbolToken(constructorOpt, objCreation.Syntax);
			if (used)
			{
				EmitExpression(target, used);
			}
		}

		private bool PartialCtorResultCannotEscape(BoundExpression left)
		{
			if (_tryNestingLevel == 0)
			{
				return TargetIsNotOnHeap(left);
			}
			return false;
		}

		private bool TargetIsNotOnHeap(BoundExpression left)
		{
			return left.Kind switch
			{
				BoundKind.Local => !((BoundLocal)left).LocalSymbol.IsByRef, 
				BoundKind.Parameter => !((BoundParameter)left).ParameterSymbol.IsByRef, 
				BoundKind.ReferenceAssignment => false, 
				_ => false, 
			};
		}

		private bool EmitAssignmentPreamble(BoundExpression assignmentTarget)
		{
			bool result = false;
			switch (assignmentTarget.Kind)
			{
			case BoundKind.Local:
			{
				BoundLocal boundLocal = (BoundLocal)assignmentTarget;
				if (boundLocal.LocalSymbol.IsByRef)
				{
					if (!IsStackLocal(boundLocal.LocalSymbol))
					{
						_builder.EmitLocalLoad(GetLocal(boundLocal));
					}
					result = true;
				}
				break;
			}
			case BoundKind.ReferenceAssignment:
				EmitReferenceAssignment((BoundReferenceAssignment)assignmentTarget, used: true, needReference: true);
				result = true;
				break;
			case BoundKind.FieldAccess:
			{
				BoundFieldAccess boundFieldAccess = (BoundFieldAccess)assignmentTarget;
				if (!boundFieldAccess.FieldSymbol.IsShared)
				{
					EmitReceiverRef(boundFieldAccess.ReceiverOpt, isAccessConstrained: false, AddressKind.ReadOnly);
					result = true;
				}
				break;
			}
			case BoundKind.Parameter:
			{
				BoundParameter boundParameter = (BoundParameter)assignmentTarget;
				if (boundParameter.ParameterSymbol.IsByRef)
				{
					_builder.EmitLoadArgumentOpcode(ParameterSlot(boundParameter));
					result = true;
				}
				break;
			}
			case BoundKind.ArrayAccess:
			{
				BoundArrayAccess boundArrayAccess = (BoundArrayAccess)assignmentTarget;
				EmitExpression(boundArrayAccess.Expression, used: true);
				EmitExpressions(boundArrayAccess.Indices, used: true);
				result = true;
				break;
			}
			case BoundKind.MeReference:
			{
				BoundMeReference expression = (BoundMeReference)assignmentTarget;
				EmitAddress(expression, AddressKind.Writeable);
				result = true;
				break;
			}
			case BoundKind.PseudoVariable:
				EmitPseudoVariableAddress((BoundPseudoVariable)assignmentTarget);
				result = true;
				break;
			case BoundKind.Sequence:
			{
				BoundSequence boundSequence = (BoundSequence)assignmentTarget;
				if (!boundSequence.Locals.IsEmpty)
				{
					_builder.OpenLocalScope();
					ImmutableArray<LocalSymbol>.Enumerator enumerator = boundSequence.Locals.GetEnumerator();
					while (enumerator.MoveNext())
					{
						LocalSymbol current = enumerator.Current;
						DefineLocal(current, boundSequence.Syntax);
					}
				}
				EmitSideEffects(boundSequence.SideEffects);
				result = EmitAssignmentPreamble(boundSequence.ValueOpt);
				break;
			}
			case BoundKind.Call:
			{
				BoundCall call = (BoundCall)assignmentTarget;
				EmitCallExpression(call, UseKind.UsedAsAddress);
				result = true;
				break;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(assignmentTarget.Kind);
			case BoundKind.InstrumentationPayloadRoot:
			case BoundKind.ModuleVersionId:
				break;
			}
			return result;
		}

		private LocalDefinition EmitAssignmentDuplication(BoundAssignmentOperator assignmentOperator, bool used, bool lhsUsesStack)
		{
			LocalDefinition localDefinition = null;
			if (used)
			{
				_builder.EmitOpCode(ILOpCode.Dup);
				if (lhsUsesStack)
				{
					localDefinition = AllocateTemp(assignmentOperator.Left.Type, assignmentOperator.Left.Syntax);
					_builder.EmitLocalStore(localDefinition);
				}
			}
			return localDefinition;
		}

		private void EmitAssignmentPostfix(LocalDefinition temp)
		{
			if (temp != null)
			{
				_builder.EmitLocalLoad(temp);
				FreeTemp(temp);
			}
		}

		private void EmitReferenceAssignment(BoundReferenceAssignment capture, bool used, bool needReference = false)
		{
			EmitAddress(capture.LValue, AddressKind.Writeable);
			if (used)
			{
				_builder.EmitOpCode(ILOpCode.Dup);
			}
			BoundLocal byRefLocal = capture.ByRefLocal;
			if (!IsStackLocal(byRefLocal.LocalSymbol))
			{
				LocalDefinition local = GetLocal(byRefLocal);
				_builder.EmitLocalStore(local);
			}
			if (used && !needReference)
			{
				EmitLoadIndirect(capture.Type, capture.Syntax);
			}
		}

		private void EmitStore(BoundExpression expression)
		{
			switch (expression.Kind)
			{
			case BoundKind.FieldAccess:
				EmitFieldStore((BoundFieldAccess)expression);
				break;
			case BoundKind.Local:
			{
				BoundLocal boundLocal = (BoundLocal)expression;
				if (boundLocal.LocalSymbol.IsByRef)
				{
					EmitStoreIndirect(boundLocal.LocalSymbol.Type, expression.Syntax);
				}
				else if (!IsStackLocal(boundLocal.LocalSymbol))
				{
					LocalDefinition local = GetLocal(boundLocal);
					_builder.EmitLocalStore(local);
				}
				break;
			}
			case BoundKind.ReferenceAssignment:
			case BoundKind.PseudoVariable:
				EmitStoreIndirect(expression.Type, expression.Syntax);
				break;
			case BoundKind.ArrayAccess:
			{
				ArrayTypeSymbol arrayType = (ArrayTypeSymbol)((BoundArrayAccess)expression).Expression.Type;
				EmitArrayElementStore(arrayType, expression.Syntax);
				break;
			}
			case BoundKind.MeReference:
				EmitMeStore((BoundMeReference)expression);
				break;
			case BoundKind.Parameter:
				EmitParameterStore((BoundParameter)expression);
				break;
			case BoundKind.Sequence:
			{
				BoundSequence boundSequence = (BoundSequence)expression;
				EmitStore(boundSequence.ValueOpt);
				if (!boundSequence.Locals.IsEmpty)
				{
					_builder.CloseLocalScope();
					ImmutableArray<LocalSymbol>.Enumerator enumerator = boundSequence.Locals.GetEnumerator();
					while (enumerator.MoveNext())
					{
						LocalSymbol current = enumerator.Current;
						FreeLocal(current);
					}
				}
				break;
			}
			case BoundKind.Call:
				EmitStoreIndirect(expression.Type, expression.Syntax);
				break;
			case BoundKind.ModuleVersionId:
				EmitModuleVersionIdStore((BoundModuleVersionId)expression);
				break;
			case BoundKind.InstrumentationPayloadRoot:
				EmitInstrumentationPayloadRootStore((BoundInstrumentationPayloadRoot)expression);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(expression.Kind);
			}
		}

		private void EmitMeStore(BoundMeReference thisRef)
		{
			_builder.EmitOpCode(ILOpCode.Stobj);
			EmitSymbolToken(thisRef.Type, thisRef.Syntax);
		}

		private void EmitArrayElementStore(ArrayTypeSymbol arrayType, SyntaxNode syntaxNode)
		{
			if (arrayType.IsSZArray)
			{
				EmitVectorElementStore(arrayType, syntaxNode);
			}
			else
			{
				_builder.EmitArrayElementStore(_module.Translate(arrayType), syntaxNode, _diagnostics);
			}
		}

		private void EmitVectorElementStore(ArrayTypeSymbol arrayType, SyntaxNode syntaxNode)
		{
			TypeSymbol typeSymbol = arrayType.ElementType;
			if (TypeSymbolExtensions.IsEnumType(typeSymbol))
			{
				typeSymbol = ((NamedTypeSymbol)typeSymbol).EnumUnderlyingType;
			}
			switch (typeSymbol.PrimitiveTypeCode)
			{
			case Microsoft.Cci.PrimitiveTypeCode.Boolean:
			case Microsoft.Cci.PrimitiveTypeCode.Int8:
			case Microsoft.Cci.PrimitiveTypeCode.UInt8:
				_builder.EmitOpCode(ILOpCode.Stelem_i1);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Char:
			case Microsoft.Cci.PrimitiveTypeCode.Int16:
			case Microsoft.Cci.PrimitiveTypeCode.UInt16:
				_builder.EmitOpCode(ILOpCode.Stelem_i2);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Int32:
			case Microsoft.Cci.PrimitiveTypeCode.UInt32:
				_builder.EmitOpCode(ILOpCode.Stelem_i4);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Int64:
			case Microsoft.Cci.PrimitiveTypeCode.UInt64:
				_builder.EmitOpCode(ILOpCode.Stelem_i8);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.IntPtr:
			case Microsoft.Cci.PrimitiveTypeCode.Pointer:
			case Microsoft.Cci.PrimitiveTypeCode.UIntPtr:
				_builder.EmitOpCode(ILOpCode.Stelem_i);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Float32:
				_builder.EmitOpCode(ILOpCode.Stelem_r4);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Float64:
				_builder.EmitOpCode(ILOpCode.Stelem_r8);
				return;
			}
			if (TypeSymbolExtensions.IsVerifierReference(typeSymbol))
			{
				_builder.EmitOpCode(ILOpCode.Stelem_ref);
				return;
			}
			_builder.EmitOpCode(ILOpCode.Stelem);
			EmitSymbolToken(typeSymbol, syntaxNode);
		}

		private void EmitFieldStore(BoundFieldAccess fieldAccess)
		{
			FieldSymbol fieldSymbol = fieldAccess.FieldSymbol;
			if (fieldSymbol.IsShared)
			{
				_builder.EmitOpCode(ILOpCode.Stsfld);
			}
			else
			{
				_builder.EmitOpCode(ILOpCode.Stfld);
			}
			EmitSymbolToken(fieldSymbol, fieldAccess.Syntax);
		}

		private void EmitParameterStore(BoundParameter parameter)
		{
			if (!parameter.ParameterSymbol.IsByRef)
			{
				int argNumber = ParameterSlot(parameter);
				_builder.EmitStoreArgumentOpcode(argNumber);
			}
			else
			{
				EmitStoreIndirect(parameter.ParameterSymbol.Type, parameter.Syntax);
			}
		}

		private void EmitStoreIndirect(TypeSymbol type, SyntaxNode syntaxNode)
		{
			if (TypeSymbolExtensions.IsEnumType(type))
			{
				type = ((NamedTypeSymbol)type).EnumUnderlyingType;
			}
			switch (type.PrimitiveTypeCode)
			{
			case Microsoft.Cci.PrimitiveTypeCode.Boolean:
			case Microsoft.Cci.PrimitiveTypeCode.Int8:
			case Microsoft.Cci.PrimitiveTypeCode.UInt8:
				_builder.EmitOpCode(ILOpCode.Stind_i1);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Char:
			case Microsoft.Cci.PrimitiveTypeCode.Int16:
			case Microsoft.Cci.PrimitiveTypeCode.UInt16:
				_builder.EmitOpCode(ILOpCode.Stind_i2);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Int32:
			case Microsoft.Cci.PrimitiveTypeCode.UInt32:
				_builder.EmitOpCode(ILOpCode.Stind_i4);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Int64:
			case Microsoft.Cci.PrimitiveTypeCode.UInt64:
				_builder.EmitOpCode(ILOpCode.Stind_i8);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.IntPtr:
			case Microsoft.Cci.PrimitiveTypeCode.Pointer:
			case Microsoft.Cci.PrimitiveTypeCode.UIntPtr:
				_builder.EmitOpCode(ILOpCode.Stind_i);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Float32:
				_builder.EmitOpCode(ILOpCode.Stind_r4);
				return;
			case Microsoft.Cci.PrimitiveTypeCode.Float64:
				_builder.EmitOpCode(ILOpCode.Stind_r8);
				return;
			}
			if (TypeSymbolExtensions.IsVerifierReference(type))
			{
				_builder.EmitOpCode(ILOpCode.Stind_ref);
				return;
			}
			_builder.EmitOpCode(ILOpCode.Stobj);
			EmitSymbolToken(type, syntaxNode);
		}

		private void EmitPopIfUnused(bool used)
		{
			if (!used)
			{
				_builder.EmitOpCode(ILOpCode.Pop);
			}
		}

		private void EmitGetType(BoundGetType boundTypeOfOperator, bool used)
		{
			TypeSymbol type = boundTypeOfOperator.SourceType.Type;
			_builder.EmitOpCode(ILOpCode.Ldtoken);
			EmitSymbolToken(type, boundTypeOfOperator.SourceType.Syntax);
			_builder.EmitOpCode(ILOpCode.Call, 0);
			MethodSymbol symbol = (MethodSymbol)_module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Type__GetTypeFromHandle);
			EmitSymbolToken(symbol, boundTypeOfOperator.Syntax);
			EmitPopIfUnused(used);
		}

		private void EmitFieldInfoExpression(BoundFieldInfo node, bool used)
		{
			_builder.EmitOpCode(ILOpCode.Ldtoken);
			EmitSymbolToken(node.Field, node.Syntax);
			MethodSymbol methodSymbol;
			if (!node.Field.ContainingType.IsGenericType)
			{
				_builder.EmitOpCode(ILOpCode.Call, 0);
				methodSymbol = (MethodSymbol)_module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle);
			}
			else
			{
				_builder.EmitOpCode(ILOpCode.Ldtoken);
				EmitSymbolToken(node.Field.ContainingType, node.Syntax);
				_builder.EmitOpCode(ILOpCode.Call, -1);
				methodSymbol = (MethodSymbol)_module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle2);
			}
			EmitSymbolToken(methodSymbol, node.Syntax);
			if (!TypeSymbol.Equals(node.Type, methodSymbol.ReturnType, TypeCompareKind.ConsiderEverything))
			{
				_builder.EmitOpCode(ILOpCode.Castclass);
				EmitSymbolToken(node.Type, node.Syntax);
			}
			EmitPopIfUnused(used);
		}

		private void EmitMethodInfoExpression(BoundMethodInfo node, bool used)
		{
			MethodSymbol methodSymbol = node.Method;
			if (methodSymbol.IsTupleMethod)
			{
				methodSymbol = methodSymbol.TupleUnderlyingMethod;
			}
			_builder.EmitOpCode(ILOpCode.Ldtoken);
			EmitSymbolToken(methodSymbol, node.Syntax);
			MethodSymbol methodSymbol2;
			if (!methodSymbol.ContainingType.IsGenericType && !methodSymbol.ContainingType.IsAnonymousType)
			{
				_builder.EmitOpCode(ILOpCode.Call, 0);
				methodSymbol2 = (MethodSymbol)_module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle);
			}
			else
			{
				_builder.EmitOpCode(ILOpCode.Ldtoken);
				EmitSymbolToken(methodSymbol.ContainingType, node.Syntax);
				_builder.EmitOpCode(ILOpCode.Call, -1);
				methodSymbol2 = (MethodSymbol)_module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle2);
			}
			EmitSymbolToken(methodSymbol2, node.Syntax);
			if (!TypeSymbol.Equals(node.Type, methodSymbol2.ReturnType, TypeCompareKind.ConsiderEverything))
			{
				_builder.EmitOpCode(ILOpCode.Castclass);
				EmitSymbolToken(node.Type, node.Syntax);
			}
			EmitPopIfUnused(used);
		}

		private void EmitBox(TypeSymbol type, SyntaxNode syntaxNode)
		{
			_builder.EmitOpCode(ILOpCode.Box);
			EmitSymbolToken(type, syntaxNode);
		}

		private void EmitUnboxAny(TypeSymbol type, SyntaxNode syntaxNode)
		{
			_builder.EmitOpCode(ILOpCode.Unbox_any);
			EmitSymbolToken(type, syntaxNode);
		}

		private void EmitMethodDefIndexExpression(BoundMethodDefIndex node)
		{
			_builder.EmitOpCode(ILOpCode.Ldtoken);
			MethodSymbol symbol = node.Method.PartialDefinitionPart ?? node.Method;
			EmitSymbolToken(symbol, node.Syntax, encodeAsRawDefinitionToken: true);
		}

		private void EmitMaximumMethodDefIndexExpression(BoundMaximumMethodDefIndex node)
		{
			_builder.EmitOpCode(ILOpCode.Ldtoken);
			_builder.EmitGreatestMethodToken();
		}

		private void EmitModuleVersionIdLoad(BoundModuleVersionId node)
		{
			_builder.EmitOpCode(ILOpCode.Ldsfld);
			EmitModuleVersionIdToken(node);
		}

		private void EmitModuleVersionIdStore(BoundModuleVersionId node)
		{
			_builder.EmitOpCode(ILOpCode.Stsfld);
			EmitModuleVersionIdToken(node);
		}

		private void EmitModuleVersionIdToken(BoundModuleVersionId node)
		{
			_builder.EmitToken(_module.GetModuleVersionId(_module.Translate(node.Type, node.Syntax, _diagnostics), node.Syntax, _diagnostics), node.Syntax, _diagnostics);
		}

		private void EmitModuleVersionIdStringLoad(BoundModuleVersionIdString node)
		{
			_builder.EmitOpCode(ILOpCode.Ldstr);
			_builder.EmitModuleVersionIdStringToken();
		}

		private void EmitInstrumentationPayloadRootLoad(BoundInstrumentationPayloadRoot node)
		{
			_builder.EmitOpCode(ILOpCode.Ldsfld);
			EmitInstrumentationPayloadRootToken(node);
		}

		private void EmitInstrumentationPayloadRootStore(BoundInstrumentationPayloadRoot node)
		{
			_builder.EmitOpCode(ILOpCode.Stsfld);
			EmitInstrumentationPayloadRootToken(node);
		}

		private void EmitInstrumentationPayloadRootToken(BoundInstrumentationPayloadRoot node)
		{
			_builder.EmitToken(_module.GetInstrumentationPayloadRoot(node.AnalysisKind, _module.Translate(node.Type, node.Syntax, _diagnostics), node.Syntax, _diagnostics), node.Syntax, _diagnostics);
		}

		private void EmitSourceDocumentIndex(BoundSourceDocumentIndex node)
		{
			_builder.EmitOpCode(ILOpCode.Ldtoken);
			_builder.EmitSourceDocumentIndexToken(node.Document);
		}

		private void EmitUnaryOperatorExpression(BoundUnaryOperator expression, bool used)
		{
			if (!used && !OperatorHasSideEffects(expression))
			{
				EmitExpression(expression.Operand, used: false);
				return;
			}
			Microsoft.Cci.PrimitiveTypeCode primitiveTypeCode2;
			int num;
			switch (expression.OperatorKind)
			{
			case UnaryOperatorKind.Minus:
				primitiveTypeCode2 = expression.Type.PrimitiveTypeCode;
				if (expression.Checked)
				{
					if (primitiveTypeCode2 != Microsoft.Cci.PrimitiveTypeCode.Int32)
					{
						num = ((primitiveTypeCode2 == Microsoft.Cci.PrimitiveTypeCode.Int64) ? 1 : 0);
						if (num == 0)
						{
							goto IL_007c;
						}
					}
					else
					{
						num = 1;
					}
					_builder.EmitOpCode(ILOpCode.Ldc_i4_0);
					if (primitiveTypeCode2 == Microsoft.Cci.PrimitiveTypeCode.Int64)
					{
						_builder.EmitOpCode(ILOpCode.Conv_i8);
					}
				}
				else
				{
					num = 0;
				}
				goto IL_007c;
			case UnaryOperatorKind.Not:
			{
				if (TypeSymbolExtensions.IsBooleanType(expression.Type))
				{
					EmitCondExpr(expression.Operand, sense: false);
					break;
				}
				EmitExpression(expression.Operand, used: true);
				_builder.EmitOpCode(ILOpCode.Not);
				Microsoft.Cci.PrimitiveTypeCode primitiveTypeCode = expression.Type.PrimitiveTypeCode;
				if (primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.UInt8 || primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.UInt16)
				{
					_builder.EmitNumericConversion(Microsoft.Cci.PrimitiveTypeCode.UInt32, primitiveTypeCode, @checked: false);
				}
				break;
			}
			case UnaryOperatorKind.Plus:
				EmitExpression(expression.Operand, used: true);
				break;
			default:
				{
					throw ExceptionUtilities.UnexpectedValue(expression.OperatorKind);
				}
				IL_007c:
				EmitExpression(expression.Operand, used: true);
				if (num != 0)
				{
					_builder.EmitOpCode(ILOpCode.Sub_ovf);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Neg);
				}
				DowncastResultOfArithmeticOperation(primitiveTypeCode2, expression.Checked);
				break;
			}
			EmitPopIfUnused(used);
		}

		private static bool OperatorHasSideEffects(BoundUnaryOperator expression)
		{
			if (expression.Checked && expression.OperatorKind == UnaryOperatorKind.Minus && TypeSymbolExtensions.IsIntegralType(expression.Type))
			{
				return true;
			}
			return false;
		}

		private void EmitBinaryOperatorExpression(BoundBinaryOperator expression, bool used)
		{
			BinaryOperatorKind binaryOperatorKind = expression.OperatorKind & BinaryOperatorKind.OpMask;
			bool flag = binaryOperatorKind == BinaryOperatorKind.AndAlso || binaryOperatorKind == BinaryOperatorKind.OrElse;
			if (!used && !flag && !OperatorHasSideEffects(expression))
			{
				EmitExpression(expression.Left, used: false);
				EmitExpression(expression.Right, used: false);
				return;
			}
			if (IsCondOperator(binaryOperatorKind))
			{
				EmitBinaryCondOperator(expression, sense: true);
			}
			else
			{
				EmitBinaryOperator(expression);
			}
			EmitPopIfUnused(used);
		}

		private bool IsCondOperator(BinaryOperatorKind operationKind)
		{
			BinaryOperatorKind binaryOperatorKind = operationKind & BinaryOperatorKind.OpMask;
			if ((uint)(binaryOperatorKind - 4) <= 5u || binaryOperatorKind == BinaryOperatorKind.OrElse || (uint)(binaryOperatorKind - 22) <= 2u)
			{
				return true;
			}
			return false;
		}

		private void EmitBinaryOperator(BoundBinaryOperator expression)
		{
			BoundExpression left = expression.Left;
			if (left.Kind != BoundKind.BinaryOperator || (object)left.ConstantValueOpt != null)
			{
				EmitBinaryOperatorSimple(expression);
				return;
			}
			BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)left;
			if (IsCondOperator(boundBinaryOperator.OperatorKind))
			{
				EmitBinaryOperatorSimple(expression);
				return;
			}
			ArrayBuilder<BoundBinaryOperator> instance = ArrayBuilder<BoundBinaryOperator>.GetInstance();
			instance.Push(expression);
			do
			{
				instance.Push(boundBinaryOperator);
				left = boundBinaryOperator.Left;
				if (left.Kind != BoundKind.BinaryOperator || (object)left.ConstantValueOpt != null)
				{
					break;
				}
				boundBinaryOperator = (BoundBinaryOperator)left;
			}
			while (!IsCondOperator(boundBinaryOperator.OperatorKind));
			EmitExpression(left, used: true);
			do
			{
				boundBinaryOperator = instance.Pop();
				EmitExpression(boundBinaryOperator.Right, used: true);
				switch (boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask)
				{
				case BinaryOperatorKind.And:
					_builder.EmitOpCode(ILOpCode.And);
					break;
				case BinaryOperatorKind.Xor:
					_builder.EmitOpCode(ILOpCode.Xor);
					break;
				case BinaryOperatorKind.Or:
					_builder.EmitOpCode(ILOpCode.Or);
					break;
				default:
					EmitBinaryArithOperatorInstructionAndDowncast(boundBinaryOperator);
					break;
				}
			}
			while (boundBinaryOperator != expression);
			instance.Free();
		}

		private void EmitBinaryOperatorSimple(BoundBinaryOperator expression)
		{
			switch (expression.OperatorKind & BinaryOperatorKind.OpMask)
			{
			case BinaryOperatorKind.And:
				EmitExpression(expression.Left, used: true);
				EmitExpression(expression.Right, used: true);
				_builder.EmitOpCode(ILOpCode.And);
				break;
			case BinaryOperatorKind.Xor:
				EmitExpression(expression.Left, used: true);
				EmitExpression(expression.Right, used: true);
				_builder.EmitOpCode(ILOpCode.Xor);
				break;
			case BinaryOperatorKind.Or:
				EmitExpression(expression.Left, used: true);
				EmitExpression(expression.Right, used: true);
				_builder.EmitOpCode(ILOpCode.Or);
				break;
			default:
				EmitBinaryArithOperator(expression);
				break;
			}
		}

		private bool OperatorHasSideEffects(BoundBinaryOperator expression)
		{
			switch (expression.OperatorKind & BinaryOperatorKind.OpMask)
			{
			case BinaryOperatorKind.Divide:
			case BinaryOperatorKind.Modulo:
			case BinaryOperatorKind.IntegerDivide:
				return true;
			case BinaryOperatorKind.Add:
			case BinaryOperatorKind.Subtract:
			case BinaryOperatorKind.Multiply:
				return expression.Checked && TypeSymbolExtensions.IsIntegralType(expression.Type);
			default:
				return false;
			}
		}

		private void EmitBinaryArithOperator(BoundBinaryOperator expression)
		{
			EmitExpression(expression.Left, used: true);
			EmitExpression(expression.Right, used: true);
			EmitBinaryArithOperatorInstructionAndDowncast(expression);
		}

		private void EmitBinaryArithOperatorInstructionAndDowncast(BoundBinaryOperator expression)
		{
			Microsoft.Cci.PrimitiveTypeCode primitiveTypeCode = expression.Type.PrimitiveTypeCode;
			BinaryOperatorKind binaryOperatorKind = expression.OperatorKind & BinaryOperatorKind.OpMask;
			switch (binaryOperatorKind)
			{
			case BinaryOperatorKind.Multiply:
				if (expression.Checked && (primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.Int32 || primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.Int64))
				{
					_builder.EmitOpCode(ILOpCode.Mul_ovf);
				}
				else if (expression.Checked && (primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.UInt32 || primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.UInt64))
				{
					_builder.EmitOpCode(ILOpCode.Mul_ovf_un);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Mul);
				}
				break;
			case BinaryOperatorKind.Modulo:
				if (primitiveTypeCode.IsUnsigned())
				{
					_builder.EmitOpCode(ILOpCode.Rem_un);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Rem);
				}
				break;
			case BinaryOperatorKind.Add:
				if (expression.Checked && (primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.Int32 || primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.Int64))
				{
					_builder.EmitOpCode(ILOpCode.Add_ovf);
				}
				else if (expression.Checked && (primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.UInt32 || primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.UInt64))
				{
					_builder.EmitOpCode(ILOpCode.Add_ovf_un);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Add);
				}
				break;
			case BinaryOperatorKind.Subtract:
				if (expression.Checked && (primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.Int32 || primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.Int64))
				{
					_builder.EmitOpCode(ILOpCode.Sub_ovf);
				}
				else if (expression.Checked && (primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.UInt32 || primitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.UInt64))
				{
					_builder.EmitOpCode(ILOpCode.Sub_ovf_un);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Sub);
				}
				break;
			case BinaryOperatorKind.Divide:
			case BinaryOperatorKind.IntegerDivide:
				if (primitiveTypeCode.IsUnsigned())
				{
					_builder.EmitOpCode(ILOpCode.Div_un);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Div);
				}
				break;
			case BinaryOperatorKind.LeftShift:
			{
				int shiftSizeMask2 = GetShiftSizeMask(expression.Left.Type);
				ConstantValue constantValueOpt2 = expression.Right.ConstantValueOpt;
				if ((object)constantValueOpt2 == null || constantValueOpt2.UInt32Value > shiftSizeMask2)
				{
					_builder.EmitConstantValue(ConstantValue.Create(shiftSizeMask2));
					_builder.EmitOpCode(ILOpCode.And);
				}
				_builder.EmitOpCode(ILOpCode.Shl);
				break;
			}
			case BinaryOperatorKind.RightShift:
			{
				int shiftSizeMask = GetShiftSizeMask(expression.Left.Type);
				ConstantValue constantValueOpt = expression.Right.ConstantValueOpt;
				if ((object)constantValueOpt == null || constantValueOpt.UInt32Value > shiftSizeMask)
				{
					_builder.EmitConstantValue(ConstantValue.Create(shiftSizeMask));
					_builder.EmitOpCode(ILOpCode.And);
				}
				if (primitiveTypeCode.IsUnsigned())
				{
					_builder.EmitOpCode(ILOpCode.Shr_un);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Shr);
				}
				break;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(binaryOperatorKind);
			}
			DowncastResultOfArithmeticOperation(primitiveTypeCode, expression.Checked && binaryOperatorKind != BinaryOperatorKind.LeftShift && binaryOperatorKind != BinaryOperatorKind.RightShift);
		}

		private void DowncastResultOfArithmeticOperation(Microsoft.Cci.PrimitiveTypeCode targetPrimitiveType, bool isChecked)
		{
			if (targetPrimitiveType == Microsoft.Cci.PrimitiveTypeCode.Int8 || targetPrimitiveType == Microsoft.Cci.PrimitiveTypeCode.UInt8 || targetPrimitiveType == Microsoft.Cci.PrimitiveTypeCode.Int16 || targetPrimitiveType == Microsoft.Cci.PrimitiveTypeCode.UInt16)
			{
				_builder.EmitNumericConversion(targetPrimitiveType.IsUnsigned() ? Microsoft.Cci.PrimitiveTypeCode.UInt32 : Microsoft.Cci.PrimitiveTypeCode.Int32, targetPrimitiveType, isChecked);
			}
		}

		public static int GetShiftSizeMask(TypeSymbol leftOperandType)
		{
			return Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetShiftSizeMask(TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(leftOperandType).SpecialType);
		}

		private void EmitShortCircuitingOperator(BoundBinaryOperator condition, bool sense, bool stopSense, bool stopValue)
		{
			object lazyDest = null;
			EmitCondBranch(condition.Left, ref lazyDest, stopSense);
			EmitCondExpr(condition.Right, sense);
			if (lazyDest != null)
			{
				object objectValue = RuntimeHelpers.GetObjectValue(new object());
				_builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(objectValue));
				_builder.AdjustStack(-1);
				_builder.MarkLabel(RuntimeHelpers.GetObjectValue(lazyDest));
				_builder.EmitBoolConstant(stopValue);
				_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue));
			}
		}

		private void EmitBinaryCondOperator(BoundBinaryOperator binOp, bool sense)
		{
			bool flag = sense;
			BinaryOperatorKind binaryOperatorKind = binOp.OperatorKind & BinaryOperatorKind.OpMask;
			TypeSymbol type = binOp.Left.Type;
			if ((object)type != null && TypeSymbolExtensions.IsBooleanType(type))
			{
				switch (binaryOperatorKind)
				{
				case BinaryOperatorKind.LessThan:
					binaryOperatorKind = BinaryOperatorKind.GreaterThan;
					break;
				case BinaryOperatorKind.LessThanOrEqual:
					binaryOperatorKind = BinaryOperatorKind.GreaterThanOrEqual;
					break;
				case BinaryOperatorKind.GreaterThan:
					binaryOperatorKind = BinaryOperatorKind.LessThan;
					break;
				case BinaryOperatorKind.GreaterThanOrEqual:
					binaryOperatorKind = BinaryOperatorKind.LessThanOrEqual;
					break;
				}
			}
			int num;
			switch (binaryOperatorKind)
			{
			case BinaryOperatorKind.OrElse:
				flag = !flag;
				goto case BinaryOperatorKind.AndAlso;
			case BinaryOperatorKind.AndAlso:
				if (!flag)
				{
					EmitShortCircuitingOperator(binOp, sense, sense, stopValue: true);
				}
				else
				{
					EmitShortCircuitingOperator(binOp, sense, !sense, stopValue: false);
				}
				return;
			case BinaryOperatorKind.NotEquals:
			case BinaryOperatorKind.IsNot:
				sense = !sense;
				goto case BinaryOperatorKind.Equals;
			case BinaryOperatorKind.Equals:
			case BinaryOperatorKind.Is:
			{
				ConstantValue constantValueOpt = binOp.Left.ConstantValueOpt;
				BoundExpression boundExpression = binOp.Right;
				if ((object)constantValueOpt == null)
				{
					constantValueOpt = boundExpression.ConstantValueOpt;
					boundExpression = binOp.Left;
				}
				if ((object)constantValueOpt != null)
				{
					if (constantValueOpt.IsDefaultValue)
					{
						if (!constantValueOpt.IsFloating)
						{
							if (sense)
							{
								EmitIsNullOrZero(boundExpression, constantValueOpt);
							}
							else
							{
								EmitIsNotNullOrZero(boundExpression, constantValueOpt);
							}
							return;
						}
					}
					else if (constantValueOpt.IsBoolean)
					{
						EmitExpression(boundExpression, used: true);
						EmitIsSense(sense);
						return;
					}
				}
				EmitBinaryCondOperatorHelper(ILOpCode.Ceq, binOp.Left, binOp.Right, sense);
				return;
			}
			case BinaryOperatorKind.Or:
				EmitBinaryCondOperatorHelper(ILOpCode.Or, binOp.Left, binOp.Right, sense);
				return;
			case BinaryOperatorKind.And:
				EmitBinaryCondOperatorHelper(ILOpCode.And, binOp.Left, binOp.Right, sense);
				return;
			case BinaryOperatorKind.Xor:
				if (sense)
				{
					EmitBinaryCondOperatorHelper(ILOpCode.Xor, binOp.Left, binOp.Right, sense: true);
				}
				else
				{
					EmitBinaryCondOperatorHelper(ILOpCode.Ceq, binOp.Left, binOp.Right, sense: true);
				}
				return;
			case BinaryOperatorKind.LessThan:
				num = 0;
				break;
			case BinaryOperatorKind.LessThanOrEqual:
				num = 1;
				sense = !sense;
				break;
			case BinaryOperatorKind.GreaterThan:
				num = 2;
				break;
			case BinaryOperatorKind.GreaterThanOrEqual:
				num = 3;
				sense = !sense;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(binaryOperatorKind);
			}
			if ((object)type != null)
			{
				if (TypeSymbolExtensions.IsUnsignedIntegralType(type))
				{
					num += 4;
				}
				else if (TypeSymbolExtensions.IsFloatingType(type))
				{
					num += 8;
				}
			}
			EmitBinaryCondOperatorHelper(s_compOpCodes[num], binOp.Left, binOp.Right, sense);
		}

		private void EmitIsNotNullOrZero(BoundExpression comparand, ConstantValue nullOrZero)
		{
			EmitExpression(comparand, used: true);
			TypeSymbol type = comparand.Type;
			if (type.IsReferenceType && !TypeSymbolExtensions.IsVerifierReference(type))
			{
				EmitBox(type, comparand.Syntax);
			}
			_builder.EmitConstantValue(nullOrZero);
			_builder.EmitOpCode(ILOpCode.Cgt_un);
		}

		private void EmitIsNullOrZero(BoundExpression comparand, ConstantValue nullOrZero)
		{
			EmitExpression(comparand, used: true);
			TypeSymbol type = comparand.Type;
			if (type.IsReferenceType && !TypeSymbolExtensions.IsVerifierReference(type))
			{
				EmitBox(type, comparand.Syntax);
			}
			_builder.EmitConstantValue(nullOrZero);
			_builder.EmitOpCode(ILOpCode.Ceq);
		}

		private void EmitBinaryCondOperatorHelper(ILOpCode opCode, BoundExpression left, BoundExpression right, bool sense)
		{
			EmitExpression(left, used: true);
			EmitExpression(right, used: true);
			_builder.EmitOpCode(opCode);
			EmitIsSense(sense);
		}

		private ConstResKind EmitCondExpr(BoundExpression condition, bool sense)
		{
			while (condition.Kind == BoundKind.UnaryOperator)
			{
				condition = ((BoundUnaryOperator)condition).Operand;
				sense = !sense;
			}
			if (_ilEmitStyle == ILEmitStyle.Release && condition.IsConstant)
			{
				bool booleanValue = condition.ConstantValueOpt.BooleanValue;
				_builder.EmitBoolConstant(booleanValue == sense);
				return (booleanValue == sense) ? ConstResKind.ConstTrue : ConstResKind.ConstFalse;
			}
			if (condition.Kind == BoundKind.BinaryOperator)
			{
				BoundBinaryOperator binOp = (BoundBinaryOperator)condition;
				EmitBinaryCondOperator(binOp, sense);
				return ConstResKind.NotAConst;
			}
			EmitExpression(condition, used: true);
			EmitIsSense(sense);
			return ConstResKind.NotAConst;
		}

		private void EmitIsSense(bool sense)
		{
			if (!sense)
			{
				_builder.EmitOpCode(ILOpCode.Ldc_i4_0);
				_builder.EmitOpCode(ILOpCode.Ceq);
			}
		}

		private void EmitStatement(BoundStatement statement)
		{
			switch (statement.Kind)
			{
			case BoundKind.Block:
				EmitBlock((BoundBlock)statement);
				break;
			case BoundKind.SequencePoint:
				EmitSequencePointStatement((BoundSequencePoint)statement);
				break;
			case BoundKind.SequencePointWithSpan:
				EmitSequencePointStatement((BoundSequencePointWithSpan)statement);
				break;
			case BoundKind.ExpressionStatement:
				EmitExpression(((BoundExpressionStatement)statement).Expression, used: false);
				break;
			case BoundKind.NoOpStatement:
				EmitNoOpStatement((BoundNoOpStatement)statement);
				break;
			case BoundKind.StatementList:
			{
				BoundStatementList boundStatementList = (BoundStatementList)statement;
				int num = boundStatementList.Statements.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					EmitStatement(boundStatementList.Statements[i]);
				}
				break;
			}
			case BoundKind.ReturnStatement:
				EmitReturnStatement((BoundReturnStatement)statement);
				break;
			case BoundKind.ThrowStatement:
				EmitThrowStatement((BoundThrowStatement)statement);
				break;
			case BoundKind.GotoStatement:
				EmitGotoStatement((BoundGotoStatement)statement);
				break;
			case BoundKind.LabelStatement:
				EmitLabelStatement((BoundLabelStatement)statement);
				break;
			case BoundKind.ConditionalGoto:
				EmitConditionalGoto((BoundConditionalGoto)statement);
				break;
			case BoundKind.TryStatement:
				EmitTryStatement((BoundTryStatement)statement);
				break;
			case BoundKind.SelectStatement:
				EmitSelectStatement((BoundSelectStatement)statement);
				break;
			case BoundKind.UnstructuredExceptionOnErrorSwitch:
				EmitUnstructuredExceptionOnErrorSwitch((BoundUnstructuredExceptionOnErrorSwitch)statement);
				break;
			case BoundKind.UnstructuredExceptionResumeSwitch:
				EmitUnstructuredExceptionResumeSwitch((BoundUnstructuredExceptionResumeSwitch)statement);
				break;
			case BoundKind.StateMachineScope:
				EmitStateMachineScope((BoundStateMachineScope)statement);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(statement.Kind);
			}
		}

		private int EmitStatementAndCountInstructions(BoundStatement statement)
		{
			int instructionsEmitted = _builder.InstructionsEmitted;
			EmitStatement(statement);
			return _builder.InstructionsEmitted - instructionsEmitted;
		}

		private void EmitNoOpStatement(BoundNoOpStatement statement)
		{
			switch (statement.Flavor)
			{
			case NoOpStatementFlavor.Default:
				if (_ilEmitStyle == ILEmitStyle.Debug)
				{
					_builder.EmitOpCode(ILOpCode.Nop);
				}
				break;
			case NoOpStatementFlavor.AwaitYieldPoint:
				if (_asyncYieldPoints == null)
				{
					_asyncYieldPoints = ArrayBuilder<int>.GetInstance();
					_asyncResumePoints = ArrayBuilder<int>.GetInstance();
				}
				_asyncYieldPoints.Add(_builder.AllocateILMarker());
				break;
			case NoOpStatementFlavor.AwaitResumePoint:
				_asyncResumePoints.Add(_builder.AllocateILMarker());
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(statement.Flavor);
			}
		}

		private void EmitTryStatement(BoundTryStatement statement, bool emitCatchesOnly = false)
		{
			bool num = !emitCatchesOnly && statement.CatchBlocks.Length > 0 && statement.FinallyBlockOpt != null;
			_builder.OpenLocalScope(ScopeType.TryCatchFinally);
			_builder.OpenLocalScope(ScopeType.Try);
			_tryNestingLevel++;
			if (num)
			{
				EmitTryStatement(statement, emitCatchesOnly: true);
			}
			else
			{
				EmitBlock(statement.TryBlock);
			}
			_tryNestingLevel--;
			_builder.CloseLocalScope();
			if (!num)
			{
				ImmutableArray<BoundCatchBlock>.Enumerator enumerator = statement.CatchBlocks.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundCatchBlock current = enumerator.Current;
					EmitCatchBlock(current);
				}
			}
			if (!emitCatchesOnly && statement.FinallyBlockOpt != null)
			{
				_builder.OpenLocalScope(ScopeType.Finally);
				EmitBlock(statement.FinallyBlockOpt);
				_builder.CloseLocalScope();
			}
			_builder.CloseLocalScope();
			if (!emitCatchesOnly && (object)statement.ExitLabelOpt != null)
			{
				_builder.MarkLabel(statement.ExitLabelOpt);
			}
		}

		private void EmitCatchBlock(BoundCatchBlock catchBlock)
		{
			BoundCatchBlock currentCatchBlock = _currentCatchBlock;
			_currentCatchBlock = catchBlock;
			object obj = null;
			BoundExpression boundExpression = catchBlock.ExceptionSourceOpt;
			ITypeReference typeReference = ((boundExpression == null) ? _module.Translate(_module.Compilation.GetWellKnownType(WellKnownType.System_Exception), catchBlock.Syntax, _diagnostics) : _module.Translate(boundExpression.Type, boundExpression.Syntax, _diagnostics));
			_builder.AdjustStack(1);
			if (catchBlock.ExceptionFilterOpt != null && catchBlock.ExceptionFilterOpt.Kind == BoundKind.UnstructuredExceptionHandlingCatchFilter)
			{
				BoundUnstructuredExceptionHandlingCatchFilter boundUnstructuredExceptionHandlingCatchFilter = (BoundUnstructuredExceptionHandlingCatchFilter)catchBlock.ExceptionFilterOpt;
				_builder.OpenLocalScope(ScopeType.Filter);
				_builder.EmitOpCode(ILOpCode.Isinst);
				_builder.EmitToken(typeReference, catchBlock.Syntax, _diagnostics);
				_builder.EmitOpCode(ILOpCode.Ldnull);
				_builder.EmitOpCode(ILOpCode.Cgt_un);
				EmitLocalLoad(boundUnstructuredExceptionHandlingCatchFilter.ActiveHandlerLocal, used: true);
				_builder.EmitIntConstant(0);
				_builder.EmitOpCode(ILOpCode.Cgt_un);
				_builder.EmitOpCode(ILOpCode.And);
				EmitLocalLoad(boundUnstructuredExceptionHandlingCatchFilter.ResumeTargetLocal, used: true);
				_builder.EmitIntConstant(0);
				_builder.EmitOpCode(ILOpCode.Ceq);
				_builder.EmitOpCode(ILOpCode.And);
				_builder.MarkFilterConditionEnd();
				_builder.EmitOpCode(ILOpCode.Castclass);
				_builder.EmitToken(typeReference, catchBlock.Syntax, _diagnostics);
				if (ShouldNoteProjectErrors())
				{
					EmitSetProjectError(catchBlock.Syntax, catchBlock.ErrorLineNumberOpt);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Pop);
				}
			}
			else
			{
				if (catchBlock.ExceptionFilterOpt == null)
				{
					_builder.OpenLocalScope(ScopeType.Catch, typeReference);
					if (catchBlock.IsSynthesizedAsyncCatchAll)
					{
						_asyncCatchHandlerOffset = _builder.AllocateILMarker();
					}
				}
				else
				{
					_builder.OpenLocalScope(ScopeType.Filter);
					object objectValue = RuntimeHelpers.GetObjectValue(new object());
					obj = RuntimeHelpers.GetObjectValue(new object());
					_builder.EmitOpCode(ILOpCode.Isinst);
					_builder.EmitToken(typeReference, catchBlock.Syntax, _diagnostics);
					_builder.EmitOpCode(ILOpCode.Dup);
					_builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue));
					_builder.EmitOpCode(ILOpCode.Pop);
					_builder.EmitIntConstant(0);
					_builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(obj));
					_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue));
				}
				LocalSymbol localOpt = catchBlock.LocalOpt;
				if ((object)localOpt != null)
				{
					ImmutableArray<SyntaxReference> declaringSyntaxReferences = localOpt.DeclaringSyntaxReferences;
					DefineLocal(localOpt, (!declaringSyntaxReferences.IsEmpty) ? ((VisualBasicSyntaxNode)declaringSyntaxReferences[0].GetSyntax()) : catchBlock.Syntax);
				}
				if (boundExpression != null)
				{
					if (ShouldNoteProjectErrors())
					{
						_builder.EmitOpCode(ILOpCode.Dup);
						EmitSetProjectError(catchBlock.Syntax, catchBlock.ErrorLineNumberOpt);
					}
					if (TypeSymbolExtensions.IsTypeParameter(boundExpression.Type))
					{
						_builder.EmitOpCode(ILOpCode.Unbox_any);
						EmitSymbolToken(boundExpression.Type, boundExpression.Syntax);
					}
					while (boundExpression.Kind == BoundKind.Sequence)
					{
						BoundSequence boundSequence = (BoundSequence)boundExpression;
						EmitSideEffects(boundSequence.SideEffects);
						if (boundSequence.ValueOpt == null)
						{
							break;
						}
						boundExpression = boundSequence.ValueOpt;
					}
					switch (boundExpression.Kind)
					{
					case BoundKind.Local:
						_builder.EmitLocalStore(GetLocal((BoundLocal)boundExpression));
						break;
					case BoundKind.Parameter:
					{
						BoundParameter boundParameter = (BoundParameter)boundExpression;
						if (boundParameter.ParameterSymbol.IsByRef)
						{
							LocalDefinition localDefinition = AllocateTemp(boundExpression.Type, boundExpression.Syntax);
							_builder.EmitLocalStore(localDefinition);
							_builder.EmitLoadArgumentOpcode(ParameterSlot(boundParameter));
							_builder.EmitLocalLoad(localDefinition);
							FreeTemp(localDefinition);
						}
						EmitParameterStore(boundParameter);
						break;
					}
					case BoundKind.FieldAccess:
					{
						BoundFieldAccess boundFieldAccess = (BoundFieldAccess)boundExpression;
						if (!boundFieldAccess.FieldSymbol.IsShared)
						{
							if (boundFieldAccess.FieldSymbol is StateMachineFieldSymbol stateMachineFieldSymbol && stateMachineFieldSymbol.SlotIndex >= 0)
							{
								DefineUserDefinedStateMachineHoistedLocal(stateMachineFieldSymbol);
							}
							LocalDefinition local = AllocateTemp(boundExpression.Type, boundExpression.Syntax);
							_builder.EmitLocalStore(local);
							BoundExpression receiverOpt = boundFieldAccess.ReceiverOpt;
							EmitReceiverRef(receiverOpt, isAccessConstrained: false, AddressKind.ReadOnly);
							_builder.EmitLocalLoad(local);
						}
						EmitFieldStore(boundFieldAccess);
						break;
					}
					default:
						throw ExceptionUtilities.UnexpectedValue(boundExpression.Kind);
					}
				}
				else if (ShouldNoteProjectErrors())
				{
					EmitSetProjectError(catchBlock.Syntax, catchBlock.ErrorLineNumberOpt);
				}
				else
				{
					_builder.EmitOpCode(ILOpCode.Pop);
				}
				if (catchBlock.ExceptionFilterOpt != null)
				{
					EmitCondExpr(catchBlock.ExceptionFilterOpt, sense: true);
					_builder.EmitIntConstant(0);
					_builder.EmitOpCode(ILOpCode.Cgt_un);
					_builder.MarkLabel(RuntimeHelpers.GetObjectValue(obj));
					_builder.MarkFilterConditionEnd();
					_builder.EmitOpCode(ILOpCode.Pop);
				}
			}
			EmitBlock(catchBlock.Body);
			if (ShouldNoteProjectErrors() && (catchBlock.ExceptionFilterOpt == null || catchBlock.ExceptionFilterOpt.Kind != BoundKind.UnstructuredExceptionHandlingCatchFilter))
			{
				EmitClearProjectError(catchBlock.Syntax);
			}
			_builder.CloseLocalScope();
			_currentCatchBlock = currentCatchBlock;
		}

		private bool ShouldNoteProjectErrors()
		{
			return !_module.SourceModule.ContainingSourceAssembly.IsVbRuntime;
		}

		private void EmitSetProjectError(SyntaxNode syntaxNode, BoundExpression errorLineNumberOpt)
		{
			MethodSymbol symbol;
			if (errorLineNumberOpt == null)
			{
				symbol = (MethodSymbol)_module.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError);
				_builder.EmitOpCode(ILOpCode.Call, -1);
			}
			else
			{
				symbol = (MethodSymbol)_module.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError_Int32);
				EmitExpression(errorLineNumberOpt, used: true);
				_builder.EmitOpCode(ILOpCode.Call, -2);
			}
			EmitSymbolToken(symbol, syntaxNode);
		}

		private void EmitClearProjectError(SyntaxNode syntaxNode)
		{
			MethodSymbol symbol = (MethodSymbol)_module.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError);
			_builder.EmitOpCode(ILOpCode.Call, 0);
			EmitSymbolToken(symbol, syntaxNode);
		}

		private void EmitConditionalGoto(BoundConditionalGoto boundConditionalGoto)
		{
			object lazyDest = boundConditionalGoto.Label;
			EmitCondBranch(boundConditionalGoto.Condition, ref lazyDest, boundConditionalGoto.JumpIfTrue);
		}

		private bool CanPassToBrfalse(TypeSymbol ts)
		{
			if (TypeSymbolExtensions.IsEnumType(ts))
			{
				return true;
			}
			switch (ts.PrimitiveTypeCode)
			{
			case Microsoft.Cci.PrimitiveTypeCode.Float32:
			case Microsoft.Cci.PrimitiveTypeCode.Float64:
				return false;
			case Microsoft.Cci.PrimitiveTypeCode.NotPrimitive:
				return ts.IsReferenceType;
			default:
				return true;
			}
		}

		private BoundExpression TryReduce(BoundBinaryOperator condition, ref bool sense)
		{
			BinaryOperatorKind binaryOperatorKind = condition.OperatorKind & BinaryOperatorKind.OpMask;
			ConstantValue constantValueOpt = condition.Left.ConstantValueOpt;
			BoundExpression boundExpression;
			if ((object)constantValueOpt != null)
			{
				boundExpression = condition.Right;
			}
			else
			{
				constantValueOpt = condition.Right.ConstantValueOpt;
				if ((object)constantValueOpt == null)
				{
					return null;
				}
				boundExpression = condition.Left;
			}
			TypeSymbol type = boundExpression.Type;
			if ((object)type != null && !CanPassToBrfalse(type))
			{
				return null;
			}
			bool num = (object)type != null && type.PrimitiveTypeCode == Microsoft.Cci.PrimitiveTypeCode.Boolean;
			bool isDefaultValue = constantValueOpt.IsDefaultValue;
			if (!num && !isDefaultValue)
			{
				return null;
			}
			if (isDefaultValue)
			{
				sense = !sense;
			}
			if (binaryOperatorKind == BinaryOperatorKind.NotEquals || binaryOperatorKind == BinaryOperatorKind.IsNot)
			{
				sense = !sense;
			}
			return boundExpression;
		}

		private ILOpCode CodeForJump(BoundBinaryOperator expression, bool sense, out ILOpCode revOpCode)
		{
			BinaryOperatorKind binaryOperatorKind = expression.OperatorKind & BinaryOperatorKind.OpMask;
			TypeSymbol type = expression.Left.Type;
			if ((object)type != null && TypeSymbolExtensions.IsBooleanType(type))
			{
				switch (binaryOperatorKind)
				{
				case BinaryOperatorKind.LessThan:
					binaryOperatorKind = BinaryOperatorKind.GreaterThan;
					break;
				case BinaryOperatorKind.LessThanOrEqual:
					binaryOperatorKind = BinaryOperatorKind.GreaterThanOrEqual;
					break;
				case BinaryOperatorKind.GreaterThan:
					binaryOperatorKind = BinaryOperatorKind.LessThan;
					break;
				case BinaryOperatorKind.GreaterThanOrEqual:
					binaryOperatorKind = BinaryOperatorKind.LessThanOrEqual;
					break;
				}
			}
			int num;
			switch (binaryOperatorKind)
			{
			case BinaryOperatorKind.IsNot:
				return sense ? ILOpCode.Bne_un : ILOpCode.Beq;
			case BinaryOperatorKind.Is:
				return sense ? ILOpCode.Beq : ILOpCode.Bne_un;
			case BinaryOperatorKind.Equals:
				return sense ? ILOpCode.Beq : ILOpCode.Bne_un;
			case BinaryOperatorKind.NotEquals:
				return sense ? ILOpCode.Bne_un : ILOpCode.Beq;
			case BinaryOperatorKind.LessThan:
				num = 0;
				break;
			case BinaryOperatorKind.LessThanOrEqual:
				num = 1;
				break;
			case BinaryOperatorKind.GreaterThan:
				num = 2;
				break;
			case BinaryOperatorKind.GreaterThanOrEqual:
				num = 3;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(binaryOperatorKind);
			}
			if ((object)type != null)
			{
				if (TypeSymbolExtensions.IsUnsignedIntegralType(type))
				{
					num += 8;
				}
				else if (TypeSymbolExtensions.IsFloatingType(type))
				{
					num += 16;
				}
			}
			int num2 = num;
			if (!sense)
			{
				num += 4;
			}
			else
			{
				num2 += 4;
			}
			revOpCode = s_condJumpOpCodes[num2];
			return s_condJumpOpCodes[num];
		}

		private void EmitCondBranch(BoundExpression condition, ref object lazyDest, bool sense)
		{
			_recursionDepth++;
			if (_recursionDepth > 1)
			{
				StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
				EmitCondBranchCore(condition, ref lazyDest, sense);
			}
			else
			{
				EmitCondBranchCoreWithStackGuard(condition, ref lazyDest, sense);
			}
			_recursionDepth--;
		}

		private void EmitCondBranchCoreWithStackGuard(BoundExpression condition, ref object lazyDest, bool sense)
		{
			try
			{
				EmitCondBranchCore(condition, ref lazyDest, sense);
			}
			catch (InsufficientExecutionStackException ex)
			{
				ProjectData.SetProjectError(ex);
				InsufficientExecutionStackException ex2 = ex;
				DiagnosticBagExtensions.Add(_diagnostics, ERRID.ERR_TooLongOrComplexExpression, BoundTreeVisitor.CancelledByStackGuardException.GetTooLongOrComplexExpressionErrorLocation(condition));
				throw new EmitCancelledException();
			}
		}

		private void EmitCondBranchCore(BoundExpression condition, ref object lazyDest, bool sense)
		{
			while (true)
			{
				ConstantValue constantValueOpt = condition.ConstantValueOpt;
				if ((object)constantValueOpt != null)
				{
					if (constantValueOpt.IsDefaultValue != sense)
					{
						lazyDest = RuntimeHelpers.GetObjectValue(lazyDest ?? new object());
						_builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(lazyDest));
					}
					break;
				}
				switch (condition.Kind)
				{
				case BoundKind.BinaryOperator:
				{
					BoundBinaryOperator boundBinaryOperator = (BoundBinaryOperator)condition;
					bool flag = sense;
					switch (boundBinaryOperator.OperatorKind & BinaryOperatorKind.OpMask)
					{
					case BinaryOperatorKind.OrElse:
						flag = !flag;
						goto case BinaryOperatorKind.AndAlso;
					case BinaryOperatorKind.AndAlso:
						if (flag)
						{
							object lazyDest2 = RuntimeHelpers.GetObjectValue(new object());
							EmitCondBranch(boundBinaryOperator.Left, ref lazyDest2, !sense);
							EmitCondBranch(boundBinaryOperator.Right, ref lazyDest, sense);
							if (lazyDest2 != null)
							{
								_builder.MarkLabel(RuntimeHelpers.GetObjectValue(lazyDest2));
							}
							return;
						}
						EmitCondBranch(boundBinaryOperator.Left, ref lazyDest, sense);
						condition = boundBinaryOperator.Right;
						goto end_IL_004d;
					case BinaryOperatorKind.Equals:
					case BinaryOperatorKind.NotEquals:
					case BinaryOperatorKind.Is:
					case BinaryOperatorKind.IsNot:
					{
						BoundExpression boundExpression = TryReduce(boundBinaryOperator, ref sense);
						if (boundExpression != null)
						{
							condition = boundExpression;
							goto end_IL_004d;
						}
						goto case BinaryOperatorKind.LessThanOrEqual;
					}
					case BinaryOperatorKind.LessThanOrEqual:
					case BinaryOperatorKind.GreaterThanOrEqual:
					case BinaryOperatorKind.LessThan:
					case BinaryOperatorKind.GreaterThan:
					{
						EmitExpression(boundBinaryOperator.Left, used: true);
						EmitExpression(boundBinaryOperator.Right, used: true);
						ILOpCode revOpCode;
						ILOpCode code = CodeForJump(boundBinaryOperator, sense, out revOpCode);
						lazyDest = RuntimeHelpers.GetObjectValue(lazyDest ?? new object());
						_builder.EmitBranch(code, RuntimeHelpers.GetObjectValue(lazyDest), revOpCode);
						return;
					}
					}
					goto default;
				}
				case BoundKind.UnaryOperator:
				{
					BoundUnaryOperator boundUnaryOperator = (BoundUnaryOperator)condition;
					if (boundUnaryOperator.OperatorKind == UnaryOperatorKind.Not)
					{
						sense = !sense;
						condition = boundUnaryOperator.Operand;
						break;
					}
					goto default;
				}
				case BoundKind.TypeOf:
				{
					BoundTypeOf boundTypeOf = (BoundTypeOf)condition;
					EmitTypeOfExpression(boundTypeOf, used: true, optimize: true);
					if (boundTypeOf.IsTypeOfIsNotExpression)
					{
						sense = !sense;
					}
					ILOpCode code = (sense ? ILOpCode.Brtrue : ILOpCode.Brfalse);
					lazyDest = RuntimeHelpers.GetObjectValue(lazyDest ?? new object());
					_builder.EmitBranch(code, RuntimeHelpers.GetObjectValue(lazyDest));
					return;
				}
				case BoundKind.Sequence:
				{
					BoundSequence sequence = (BoundSequence)condition;
					EmitSequenceCondBranch(sequence, ref lazyDest, sense);
					return;
				}
				default:
					{
						EmitExpression(condition, used: true);
						TypeSymbol type = condition.Type;
						if (!type.IsValueType && !TypeSymbolExtensions.IsVerifierReference(type))
						{
							EmitBox(type, condition.Syntax);
						}
						ILOpCode code = (sense ? ILOpCode.Brtrue : ILOpCode.Brfalse);
						lazyDest = RuntimeHelpers.GetObjectValue(lazyDest ?? new object());
						_builder.EmitBranch(code, RuntimeHelpers.GetObjectValue(lazyDest));
						return;
					}
					end_IL_004d:
					break;
				}
			}
		}

		[Conditional("DEBUG")]
		private void ValidateReferenceEqualityOperands(BoundBinaryOperator binOp)
		{
		}

		private void EmitSequenceCondBranch(BoundSequence sequence, ref object lazyDest, bool sense)
		{
			bool flag = !sequence.Locals.IsEmpty;
			if (flag)
			{
				_builder.OpenLocalScope();
				ImmutableArray<LocalSymbol>.Enumerator enumerator = sequence.Locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					DefineLocal(current, sequence.Syntax);
				}
			}
			EmitSideEffects(sequence.SideEffects);
			EmitCondBranch(sequence.ValueOpt, ref lazyDest, sense);
			if (flag)
			{
				_builder.CloseLocalScope();
				ImmutableArray<LocalSymbol>.Enumerator enumerator2 = sequence.Locals.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					LocalSymbol current2 = enumerator2.Current;
					FreeLocal(current2);
				}
			}
		}

		private void EmitLabelStatement(BoundLabelStatement boundLabelStatement)
		{
			_builder.MarkLabel(boundLabelStatement.Label);
		}

		private void EmitGotoStatement(BoundGotoStatement boundGotoStatement)
		{
			if (ShouldNoteProjectErrors() && _currentCatchBlock != null && (_currentCatchBlock.ExceptionFilterOpt == null || _currentCatchBlock.ExceptionFilterOpt.Kind != BoundKind.UnstructuredExceptionHandlingCatchFilter) && !LabelFinder.NodeContainsLabel(_currentCatchBlock, boundGotoStatement.Label))
			{
				EmitClearProjectError(boundGotoStatement.Syntax);
			}
			_builder.EmitBranch(ILOpCode.Br, boundGotoStatement.Label);
		}

		private void EmitReturnStatement(BoundReturnStatement boundReturnStatement)
		{
			EmitExpression(boundReturnStatement.ExpressionOpt, used: true);
			_builder.EmitRet(boundReturnStatement.ExpressionOpt == null);
		}

		private void EmitThrowStatement(BoundThrowStatement boundThrowStatement)
		{
			BoundExpression expressionOpt = boundThrowStatement.ExpressionOpt;
			if (expressionOpt != null)
			{
				EmitExpression(expressionOpt, used: true);
				TypeSymbol type = expressionOpt.Type;
				if ((object)type != null && type.TypeKind == TypeKind.TypeParameter)
				{
					EmitBox(type, expressionOpt.Syntax);
				}
			}
			_builder.EmitThrow(expressionOpt == null);
		}

		private void EmitSelectStatement(BoundSelectStatement boundSelectStatement)
		{
			BoundExpression expression = boundSelectStatement.ExpressionStatement.Expression;
			ImmutableArray<BoundCaseBlock> caseBlocks = boundSelectStatement.CaseBlocks;
			LabelSymbol exitLabel = boundSelectStatement.ExitLabel;
			LabelSymbol fallThroughLabel = exitLabel;
			ImmutableArray<GeneratedLabelSymbol> caseBlockLabels = CreateCaseBlockLabels(caseBlocks);
			KeyValuePair<ConstantValue, object>[] caseLabelsForEmitSwitchHeader = GetCaseLabelsForEmitSwitchHeader(caseBlocks, caseBlockLabels, ref fallThroughLabel);
			EmitSwitchTableHeader(expression, caseLabelsForEmitSwitchHeader, fallThroughLabel);
			EmitCaseBlocks(caseBlocks, caseBlockLabels, exitLabel);
			_builder.MarkLabel(exitLabel);
		}

		private ImmutableArray<GeneratedLabelSymbol> CreateCaseBlockLabels(ImmutableArray<BoundCaseBlock> caseBlocks)
		{
			ArrayBuilder<GeneratedLabelSymbol> instance = ArrayBuilder<GeneratedLabelSymbol>.GetInstance(caseBlocks.Length);
			int num = 0;
			ImmutableArray<BoundCaseBlock>.Enumerator enumerator = caseBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				_ = enumerator.Current;
				num++;
				instance.Add(new GeneratedLabelSymbol("Case Block " + num));
			}
			return instance.ToImmutableAndFree();
		}

		private KeyValuePair<ConstantValue, object>[] GetCaseLabelsForEmitSwitchHeader(ImmutableArray<BoundCaseBlock> caseBlocks, ImmutableArray<GeneratedLabelSymbol> caseBlockLabels, ref LabelSymbol fallThroughLabel)
		{
			ArrayBuilder<KeyValuePair<ConstantValue, object>> instance = ArrayBuilder<KeyValuePair<ConstantValue, object>>.GetInstance();
			HashSet<ConstantValue> hashSet = new HashSet<ConstantValue>(new SwitchConstantValueHelper.SwitchLabelsComparer());
			int num = 0;
			ImmutableArray<BoundCaseBlock>.Enumerator enumerator = caseBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundCaseBlock current = enumerator.Current;
				GeneratedLabelSymbol generatedLabelSymbol = caseBlockLabels[num];
				ImmutableArray<BoundCaseClause> caseClauses = current.CaseStatement.CaseClauses;
				if (caseClauses.Any())
				{
					ImmutableArray<BoundCaseClause>.Enumerator enumerator2 = caseClauses.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						BoundCaseClause current2 = enumerator2.Current;
						ConstantValue constantValue = current2.Kind switch
						{
							BoundKind.SimpleCaseClause => ((BoundSimpleCaseClause)current2).ValueOpt.ConstantValueOpt, 
							BoundKind.RelationalCaseClause => ((BoundRelationalCaseClause)current2).ValueOpt.ConstantValueOpt, 
							BoundKind.RangeCaseClause => throw ExceptionUtilities.UnexpectedValue(current2.Kind), 
							_ => throw ExceptionUtilities.UnexpectedValue(current2.Kind), 
						};
						if (!hashSet.Contains(constantValue))
						{
							instance.Add(new KeyValuePair<ConstantValue, object>(constantValue, generatedLabelSymbol));
							hashSet.Add(constantValue);
						}
					}
				}
				else
				{
					fallThroughLabel = generatedLabelSymbol;
				}
				num++;
			}
			return instance.ToArrayAndFree();
		}

		private void EmitSwitchTableHeader(BoundExpression selectExpression, KeyValuePair<ConstantValue, object>[] caseLabels, LabelSymbol fallThroughLabel)
		{
			if (!caseLabels.Any())
			{
				_builder.EmitBranch(ILOpCode.Br, fallThroughLabel);
				return;
			}
			TypeSymbol type = selectExpression.Type;
			LocalDefinition localDefinition = null;
			if (type.SpecialType != SpecialType.System_String)
			{
				if (selectExpression.Kind == BoundKind.Local && !((BoundLocal)selectExpression).LocalSymbol.IsByRef)
				{
					_builder.EmitIntegerSwitchJumpTable(caseLabels, fallThroughLabel, GetLocal((BoundLocal)selectExpression), TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type).PrimitiveTypeCode);
				}
				else if (selectExpression.Kind == BoundKind.Parameter && !((BoundParameter)selectExpression).ParameterSymbol.IsByRef)
				{
					_builder.EmitIntegerSwitchJumpTable(caseLabels, fallThroughLabel, ParameterSlot((BoundParameter)selectExpression), TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type).PrimitiveTypeCode);
				}
				else
				{
					EmitExpression(selectExpression, used: true);
					localDefinition = AllocateTemp(type, selectExpression.Syntax);
					_builder.EmitLocalStore(localDefinition);
					_builder.EmitIntegerSwitchJumpTable(caseLabels, fallThroughLabel, localDefinition, TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type).PrimitiveTypeCode);
				}
			}
			else if (selectExpression.Kind == BoundKind.Local && !((BoundLocal)selectExpression).LocalSymbol.IsByRef)
			{
				EmitStringSwitchJumpTable(caseLabels, fallThroughLabel, GetLocal((BoundLocal)selectExpression), selectExpression.Syntax);
			}
			else
			{
				EmitExpression(selectExpression, used: true);
				localDefinition = AllocateTemp(type, selectExpression.Syntax);
				_builder.EmitLocalStore(localDefinition);
				EmitStringSwitchJumpTable(caseLabels, fallThroughLabel, localDefinition, selectExpression.Syntax);
			}
			if (localDefinition != null)
			{
				FreeTemp(localDefinition);
			}
		}

		private void EmitStringSwitchJumpTable(KeyValuePair<ConstantValue, object>[] caseLabels, LabelSymbol fallThroughLabel, LocalDefinition key, SyntaxNode syntaxNode)
		{
			bool num = SwitchStringJumpTableEmitter.ShouldGenerateHashTableSwitch(_module, caseLabels.Length);
			LocalDefinition localDefinition = null;
			if (num)
			{
				IReference method = _module.GetPrivateImplClass(syntaxNode, _diagnostics).GetMethod("ComputeStringHash");
				_builder.EmitLocalLoad(key);
				_builder.EmitOpCode(ILOpCode.Call, 0);
				_builder.EmitToken(method, syntaxNode, _diagnostics);
				TypeSymbol type = (TypeSymbol)_module.GetSpecialType(SpecialType.System_UInt32, syntaxNode, _diagnostics).GetInternalSymbol();
				localDefinition = AllocateTemp(type, syntaxNode);
				_builder.EmitLocalStore(localDefinition);
			}
			NamedTypeSymbol wellKnownType = _module.Compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators);
			WellKnownMember member = ((TypeSymbolExtensions.IsErrorType(wellKnownType) && wellKnownType is MissingMetadataTypeSymbol) ? WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareStringStringStringBoolean : WellKnownMember.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators__CompareStringStringStringBoolean);
			MethodSymbol methodSymbol = (MethodSymbol)_module.Compilation.GetWellKnownTypeMember(member);
			IReference stringCompareMethodRef = _module.Translate(methodSymbol, syntaxNode, _diagnostics);
			SwitchStringJumpTableEmitter.EmitStringCompareAndBranch emitStringCondBranchDelegate = delegate(LocalOrParameter keyArg, ConstantValue stringConstant, object targetLabel)
			{
				EmitStringCompareAndBranch(keyArg, syntaxNode, stringConstant, RuntimeHelpers.GetObjectValue(targetLabel), stringCompareMethodRef);
			};
			_builder.EmitStringSwitchJumpTable(caseLabels, fallThroughLabel, key, localDefinition, emitStringCondBranchDelegate, SynthesizedStringSwitchHashMethod.ComputeStringHash);
			if (localDefinition != null)
			{
				FreeTemp(localDefinition);
			}
		}

		private void EmitStringCompareAndBranch(LocalOrParameter key, SyntaxNode syntaxNode, ConstantValue stringConstant, object targetLabel, IReference stringCompareMethodRef)
		{
			_builder.EmitLoad(key);
			_builder.EmitConstantValue(stringConstant);
			_builder.EmitConstantValue(ConstantValue.False);
			_builder.EmitOpCode(ILOpCode.Call, -2);
			_builder.EmitToken(stringCompareMethodRef, syntaxNode, _diagnostics);
			_builder.EmitBranch(ILOpCode.Brfalse, RuntimeHelpers.GetObjectValue(targetLabel), ILOpCode.Brtrue);
		}

		private void EmitCaseBlocks(ImmutableArray<BoundCaseBlock> caseBlocks, ImmutableArray<GeneratedLabelSymbol> caseBlockLabels, LabelSymbol exitLabel)
		{
			int num = 0;
			ImmutableArray<BoundCaseBlock>.Enumerator enumerator = caseBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundCaseBlock current = enumerator.Current;
				_builder.MarkLabel(caseBlockLabels[num]);
				num++;
				BoundCaseStatement caseStatement = current.CaseStatement;
				if (!caseStatement.WasCompilerGenerated)
				{
					if (_emitPdbSequencePoints)
					{
						EmitSequencePoint(caseStatement.Syntax);
					}
					if (_ilEmitStyle == ILEmitStyle.Debug)
					{
						_builder.EmitOpCode(ILOpCode.Nop);
					}
				}
				EmitBlock(current.Body);
				_builder.EmitBranch(ILOpCode.Br, exitLabel);
			}
		}

		private void EmitBlock(BoundBlock scope)
		{
			bool flag = !scope.Locals.IsEmpty;
			if (flag)
			{
				_builder.OpenLocalScope();
				ImmutableArray<LocalSymbol>.Enumerator enumerator = scope.Locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					ImmutableArray<SyntaxReference> declaringSyntaxReferences = current.DeclaringSyntaxReferences;
					DefineLocal(current, declaringSyntaxReferences.IsEmpty ? scope.Syntax : VisualBasicExtensions.GetVisualBasicSyntax(declaringSyntaxReferences[0]));
				}
			}
			ImmutableArray<BoundStatement>.Enumerator enumerator2 = scope.Statements.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				BoundStatement current2 = enumerator2.Current;
				EmitStatement(current2);
			}
			if (flag)
			{
				_builder.CloseLocalScope();
			}
		}

		private LocalDefinition DefineLocal(LocalSymbol local, SyntaxNode syntaxNode)
		{
			ImmutableArray<bool> empty = ImmutableArray<bool>.Empty;
			ImmutableArray<string> tupleElementNames = ((!local.IsCompilerGenerated && TypeSymbolExtensions.ContainsTupleNames(local.Type)) ? VisualBasicCompilation.TupleNamesEncoder.Encode(local.Type) : ImmutableArray<string>.Empty);
			if (local.HasConstantValue)
			{
				MetadataConstant compileTimeValue = _module.CreateConstant(local.Type, RuntimeHelpers.GetObjectValue(local.ConstantValue), syntaxNode, _diagnostics);
				LocalConstantDefinition localConstant = new LocalConstantDefinition(local.Name, local.Locations.FirstOrDefault() ?? Location.None, compileTimeValue, empty, tupleElementNames);
				_builder.AddLocalConstantToScope(localConstant);
				return null;
			}
			if (IsStackLocal(local))
			{
				return null;
			}
			ITypeReference typeReference = _module.Translate(local.Type, syntaxNode, _diagnostics);
			_module.GetFakeSymbolTokenForIL(typeReference, syntaxNode, _diagnostics);
			LocalSlotConstraints constraints = (local.IsByRef ? LocalSlotConstraints.ByRef : LocalSlotConstraints.None) | (local.IsPinned ? LocalSlotConstraints.Pinned : LocalSlotConstraints.None);
			LocalDebugId localId = default(LocalDebugId);
			string localDebugName = GetLocalDebugName(local, out localId);
			SynthesizedLocalKind synthesizedKind = local.SynthesizedKind;
			LocalDefinition localDefinition = _builder.LocalSlotManager.DeclareLocal(typeReference, local, localDebugName, synthesizedKind, localId, synthesizedKind.PdbAttributes(), constraints, empty, tupleElementNames, synthesizedKind.IsSlotReusable(_ilEmitStyle != ILEmitStyle.Release));
			if (localDefinition.Name != null)
			{
				_builder.AddLocalToScope(localDefinition);
			}
			return localDefinition;
		}

		private string GetLocalDebugName(LocalSymbol local, out LocalDebugId localId)
		{
			localId = LocalDebugId.None;
			if (local.IsImportedFromMetadata)
			{
				return local.Name;
			}
			if (local.DeclarationKind == LocalDeclarationKind.FunctionValue && _method is SynthesizedStateMachineMethod)
			{
				return null;
			}
			SynthesizedLocalKind synthesizedKind = local.SynthesizedKind;
			if (!synthesizedKind.IsLongLived())
			{
				return null;
			}
			if (_ilEmitStyle == ILEmitStyle.Debug)
			{
				SyntaxNode declaratorSyntax = local.GetDeclaratorSyntax();
				int syntaxOffset = _method.CalculateLocalSyntaxOffset(declaratorSyntax.SpanStart, declaratorSyntax.SyntaxTree);
				int ordinal = _synthesizedLocalOrdinals.AssignLocalOrdinal(synthesizedKind, syntaxOffset);
				localId = new LocalDebugId(syntaxOffset, ordinal);
			}
			if (local.Name != null)
			{
				return local.Name;
			}
			return GeneratedNames.MakeSynthesizedLocalName(synthesizedKind, ref _uniqueNameId);
		}

		private bool IsSlotReusable(LocalSymbol local)
		{
			return local.SynthesizedKind.IsSlotReusable(_ilEmitStyle != ILEmitStyle.Release);
		}

		private void FreeLocal(LocalSymbol local)
		{
			if (local.Name == null && IsSlotReusable(local) && !IsStackLocal(local))
			{
				_builder.LocalSlotManager.FreeLocal(local);
			}
		}

		private LocalDefinition GetLocal(BoundLocal localExpression)
		{
			LocalSymbol localSymbol = localExpression.LocalSymbol;
			return GetLocal(localSymbol);
		}

		private LocalDefinition GetLocal(LocalSymbol symbol)
		{
			return _builder.LocalSlotManager.GetLocal(symbol);
		}

		private LocalDefinition AllocateTemp(TypeSymbol type, SyntaxNode syntaxNode)
		{
			return _builder.LocalSlotManager.AllocateSlot(_module.Translate(type, syntaxNode, _diagnostics), LocalSlotConstraints.None);
		}

		private void FreeTemp(LocalDefinition temp)
		{
			_builder.LocalSlotManager.FreeSlot(temp);
		}

		private void FreeOptTemp(LocalDefinition temp)
		{
			if (temp != null)
			{
				FreeTemp(temp);
			}
		}

		private void EmitUnstructuredExceptionOnErrorSwitch(BoundUnstructuredExceptionOnErrorSwitch node)
		{
			EmitExpression(node.Value, used: true);
			EmitSwitch(node.Jumps);
		}

		private void EmitSwitch(ImmutableArray<BoundGotoStatement> jumps)
		{
			object[] array = new object[jumps.Length - 1 + 1];
			int num = jumps.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = jumps[i].Label;
			}
			_builder.EmitSwitch(array);
		}

		private void EmitStateMachineScope(BoundStateMachineScope scope)
		{
			_builder.OpenLocalScope(ScopeType.StateMachineVariable);
			ImmutableArray<FieldSymbol>.Enumerator enumerator = scope.Fields.GetEnumerator();
			while (enumerator.MoveNext())
			{
				FieldSymbol current = enumerator.Current;
				DefineUserDefinedStateMachineHoistedLocal((StateMachineFieldSymbol)current);
			}
			EmitStatement(scope.Statement);
			_builder.CloseLocalScope();
		}

		private void DefineUserDefinedStateMachineHoistedLocal(StateMachineFieldSymbol field)
		{
			if (_module.DebugInformationFormat == DebugInformationFormat.Pdb)
			{
				_builder.AddLocalToScope(new LocalDefinition(null, field.Name, null, field.SlotIndex, SynthesizedLocalKind.EmitterTemp, default(LocalDebugId), LocalVariableAttributes.None, LocalSlotConstraints.None, default(ImmutableArray<bool>), default(ImmutableArray<string>)));
			}
			else
			{
				_builder.DefineUserDefinedStateMachineHoistedLocal(field.SlotIndex);
			}
		}

		private void EmitUnstructuredExceptionResumeSwitch(BoundUnstructuredExceptionResumeSwitch node)
		{
			EmitLabelStatement(node.ResumeLabel);
			EmitExpression(node.ResumeTargetTemporary, used: true);
			object objectValue = RuntimeHelpers.GetObjectValue(new object());
			_builder.EmitBranch(ILOpCode.Br_s, RuntimeHelpers.GetObjectValue(objectValue));
			_builder.AdjustStack(-1);
			EmitLabelStatement(node.ResumeNextLabel);
			EmitExpression(node.ResumeTargetTemporary, used: true);
			_builder.EmitIntConstant(1);
			_builder.EmitOpCode(ILOpCode.Add);
			_builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue));
			_builder.EmitIntConstant(0);
			_builder.EmitLocalStore(GetLocal(node.ResumeTargetTemporary));
			EmitSwitch(node.Jumps);
		}
	}
}

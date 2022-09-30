using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class DataFlowPass : AbstractFlowPass<DataFlowPass.LocalState>
	{
		internal struct LocalState : AbstractLocalState
		{
			internal BitVector Assigned;

			public bool Reachable
			{
				get
				{
					if (Assigned.Capacity > 0)
					{
						return !IsAssigned(0);
					}
					return true;
				}
			}

			public bool FunctionAssignedValue => IsAssigned(1);

			internal LocalState(BitVector assigned)
			{
				this = default(LocalState);
				Assigned = assigned;
			}

			public LocalState Clone()
			{
				return new LocalState(Assigned.Clone());
			}

			LocalState AbstractLocalState.Clone()
			{
				//ILSpy generated this explicit interface implementation from .override directive in Clone
				return this.Clone();
			}

			public bool IsAssigned(int slot)
			{
				if (slot != -1 && !Assigned[0])
				{
					return Assigned[slot];
				}
				return true;
			}

			public void Assign(int slot)
			{
				if (slot != -1)
				{
					Assigned[slot] = true;
				}
			}

			public void Unassign(int slot)
			{
				if (slot != -1)
				{
					Assigned[slot] = false;
				}
			}
		}

		protected struct SlotCollection
		{
			private int _singleValue;

			private ArrayBuilder<int> _builder;

			public int Count
			{
				get
				{
					if (_builder != null)
					{
						return _builder.Count;
					}
					return (_singleValue != 0) ? 1 : 0;
				}
			}

			public int this[int index]
			{
				get
				{
					if (_builder != null)
					{
						return _builder[index];
					}
					return _singleValue;
				}
				set
				{
					if (_builder != null)
					{
						_builder[index] = value;
					}
					_singleValue = value;
				}
			}

			public void Append(int slot)
			{
				if (_builder != null)
				{
					_builder.Add(slot);
					return;
				}
				if (_singleValue == 0)
				{
					_singleValue = slot;
					return;
				}
				_builder = ArrayBuilder<int>.GetInstance();
				_builder.Add(_singleValue);
				_builder.Add(slot);
			}

			public void Free()
			{
				if (_builder != null)
				{
					_builder.Free();
					_builder = null;
				}
				_singleValue = 0;
			}
		}

		protected sealed class AmbiguousLocalsPseudoSymbol : LocalSymbol
		{
			public readonly ImmutableArray<LocalSymbol> Locals;

			internal override LocalDeclarationKind DeclarationKind => LocalDeclarationKind.AmbiguousLocals;

			internal override SynthesizedLocalKind SynthesizedKind => SynthesizedLocalKind.UserDefined;

			public override bool IsFunctionValue => false;

			public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

			internal override SyntaxToken IdentifierToken => default(SyntaxToken);

			internal override Location IdentifierLocation => NoLocation.Singleton;

			public override string Name => null;

			internal override bool IsByRef => false;

			private AmbiguousLocalsPseudoSymbol(Symbol container, TypeSymbol type, ImmutableArray<LocalSymbol> locals)
				: base(container, type)
			{
				Locals = locals;
			}

			internal static LocalSymbol Create(ImmutableArray<LocalSymbol> locals)
			{
				LocalSymbol localSymbol = locals[0];
				return new AmbiguousLocalsPseudoSymbol(localSymbol.ContainingSymbol, localSymbol.Type, locals);
			}

			internal override SyntaxNode GetDeclaratorSyntax()
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		protected struct VariableIdentifier : IEquatable<VariableIdentifier>
		{
			public static VariableIdentifier None = default(VariableIdentifier);

			public readonly Symbol Symbol;

			public readonly int ContainingSlot;

			public bool Exists => (object)Symbol != null;

			public VariableIdentifier(Symbol symbol, int containingSlot)
			{
				this = default(VariableIdentifier);
				Symbol = symbol;
				ContainingSlot = containingSlot;
			}

			public override int GetHashCode()
			{
				return Hash.Combine(Symbol.GetHashCode(), ContainingSlot.GetHashCode());
			}

			public bool Equals(VariableIdentifier obj)
			{
				if (Symbol.Equals(obj.Symbol))
				{
					return ContainingSlot == obj.ContainingSlot;
				}
				return false;
			}

			bool IEquatable<VariableIdentifier>.Equals(VariableIdentifier obj)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Equals
				return this.Equals(obj);
			}

			public override bool Equals(object obj)
			{
				return Equals(((VariableIdentifier?)obj).Value);
			}

			public static bool operator ==(VariableIdentifier left, VariableIdentifier right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(VariableIdentifier left, VariableIdentifier right)
			{
				return !left.Equals(right);
			}
		}

		public enum SlotKind
		{
			NotTracked = -1,
			Unreachable,
			FunctionValue,
			FirstAvailable
		}

		protected readonly HashSet<Symbol> initiallyAssignedVariables;

		private readonly bool _trackStructsWithIntrinsicTypedFields;

		private readonly HashSet<LocalSymbol> _unusedVariables;

		private readonly HashSet<Symbol> _writtenVariables;

		private readonly Dictionary<VariableIdentifier, int> _variableSlot;

		protected VariableIdentifier[] variableBySlot;

		protected int nextVariableSlot;

		private BitVector _alreadyReported;

		private bool _seenOnErrorOrResume;

		protected bool _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;

		private readonly Dictionary<NamedTypeSymbol, bool> _isEmptyStructType;

		private Dictionary<TypeSymbol, ImmutableArray<FieldSymbol>> _typeToMembersCache;

		private LocalState? _tryState;

		private static BitVector UnreachableBitsSet => BitVector.AllSet(1);

		protected virtual bool EnableBreakingFlowAnalysisFeatures => false;

		protected virtual bool ProcessCompilerGeneratedLocals => false;

		protected override bool SuppressRedimOperandRvalueOnPreserve => true;

		protected virtual bool IgnoreOutSemantics => true;

		protected override bool IntersectWith(ref LocalState self, ref LocalState other)
		{
			if (self.Reachable == other.Reachable)
			{
				if (self.Assigned.Capacity != other.Assigned.Capacity)
				{
					Normalize(ref self);
					Normalize(ref other);
				}
				return IntersectBitArrays(ref self.Assigned, other.Assigned);
			}
			if (!self.Reachable)
			{
				self = other.Clone();
				return true;
			}
			return false;
		}

		protected override void UnionWith(ref LocalState self, ref LocalState other)
		{
			if (self.Assigned.Capacity != other.Assigned.Capacity)
			{
				Normalize(ref self);
				Normalize(ref other);
			}
			int num = 0;
			do
			{
				if (other.Assigned[num])
				{
					self.Assigned[num] = true;
				}
				num++;
			}
			while (num <= 1);
			int num2 = self.Assigned.Capacity - 1;
			for (int i = 2; i <= num2; i++)
			{
				if (other.Assigned[i] && !self.Assigned[i])
				{
					SetSlotAssigned(i, ref self);
				}
			}
		}

		private static bool IntersectBitArrays(ref BitVector receiver, BitVector other)
		{
			if (other[0])
			{
				return false;
			}
			if (receiver[0])
			{
				receiver = other.Clone();
				return true;
			}
			return receiver.IntersectWith(in other);
		}

		private static void UnionBitArrays(ref BitVector receiver, BitVector other)
		{
			if (!receiver[0])
			{
				if (other[0])
				{
					receiver = UnreachableBitsSet;
				}
				else
				{
					receiver.UnionWith(in other);
				}
			}
		}

		protected void Normalize(ref LocalState _state)
		{
			int capacity = _state.Assigned.Capacity;
			_state.Assigned.EnsureCapacity(nextVariableSlot);
			int num = nextVariableSlot - 1;
			for (int i = capacity; i <= num; i++)
			{
				VariableIdentifier variableIdentifier = variableBySlot[i];
				if (variableIdentifier.ContainingSlot >= 2 && _state.Assigned[variableIdentifier.ContainingSlot])
				{
					_state.Assign(i);
				}
			}
		}

		internal DataFlowPass(FlowAnalysisInfo info, bool suppressConstExpressionsSupport, bool trackStructsWithIntrinsicTypedFields = false)
			: base(info, suppressConstExpressionsSupport)
		{
			_unusedVariables = new HashSet<LocalSymbol>();
			_writtenVariables = new HashSet<Symbol>();
			_variableSlot = new Dictionary<VariableIdentifier, int>();
			variableBySlot = new VariableIdentifier[11];
			nextVariableSlot = 2;
			_convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = false;
			_isEmptyStructType = new Dictionary<NamedTypeSymbol, bool>();
			_typeToMembersCache = null;
			initiallyAssignedVariables = null;
			_trackStructsWithIntrinsicTypedFields = trackStructsWithIntrinsicTypedFields;
		}

		internal DataFlowPass(FlowAnalysisInfo info, FlowAnalysisRegionInfo region, bool suppressConstExpressionsSupport, HashSet<Symbol> initiallyAssignedVariables = null, bool trackUnassignments = false, bool trackStructsWithIntrinsicTypedFields = false)
			: base(info, region, suppressConstExpressionsSupport, trackUnassignments)
		{
			_unusedVariables = new HashSet<LocalSymbol>();
			_writtenVariables = new HashSet<Symbol>();
			_variableSlot = new Dictionary<VariableIdentifier, int>();
			variableBySlot = new VariableIdentifier[11];
			nextVariableSlot = 2;
			_convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = false;
			_isEmptyStructType = new Dictionary<NamedTypeSymbol, bool>();
			_typeToMembersCache = null;
			this.initiallyAssignedVariables = initiallyAssignedVariables;
			_trackStructsWithIntrinsicTypedFields = trackStructsWithIntrinsicTypedFields;
		}

		protected override void InitForScan()
		{
			base.InitForScan();
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = base.MethodParameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				GetOrCreateSlot(current);
			}
			_alreadyReported = BitVector.Empty;
			EnterParameters(base.MethodParameters);
			ParameterSymbol meParameter = MeParameter;
			if ((object)meParameter != null)
			{
				GetOrCreateSlot(meParameter);
				EnterParameter(meParameter);
			}
			if (initiallyAssignedVariables == null)
			{
				return;
			}
			foreach (Symbol initiallyAssignedVariable in initiallyAssignedVariables)
			{
				SetSlotAssigned(GetOrCreateSlot(initiallyAssignedVariable));
			}
		}

		protected override bool Scan()
		{
			if (!base.Scan())
			{
				return false;
			}
			if (!_seenOnErrorOrResume)
			{
				foreach (LocalSymbol unusedVariable in _unusedVariables)
				{
					ReportUnused(unusedVariable);
				}
			}
			if (base.ShouldAnalyzeByRefParameters)
			{
				LeaveParameters(base.MethodParameters);
			}
			ParameterSymbol meParameter = MeParameter;
			if ((object)meParameter != null)
			{
				LeaveParameter(meParameter);
			}
			return true;
		}

		private void ReportUnused(LocalSymbol local)
		{
			if (local.IsFunctionValue || string.IsNullOrEmpty(local.Name))
			{
				return;
			}
			if (_writtenVariables.Contains(local))
			{
				if (local.IsConst)
				{
					DiagnosticBagExtensions.Add(diagnostics, ERRID.WRN_UnusedLocalConst, local.Locations[0], local.Name ?? "dummy");
				}
			}
			else
			{
				DiagnosticBagExtensions.Add(diagnostics, ERRID.WRN_UnusedLocal, local.Locations[0], local.Name ?? "dummy");
			}
		}

		protected virtual void ReportUnassignedByRefParameter(ParameterSymbol parameter)
		{
		}

		public static void Analyze(FlowAnalysisInfo info, DiagnosticBag diagnostics, bool suppressConstExpressionsSupport)
		{
			DataFlowPass dataFlowPass = new DataFlowPass(info, suppressConstExpressionsSupport);
			if (diagnostics != null)
			{
				dataFlowPass._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = true;
			}
			try
			{
				dataFlowPass.Analyze();
				diagnostics?.AddRange(dataFlowPass.diagnostics);
			}
			catch (CancelledByStackGuardException ex) when (((Func<bool>)delegate
			{
				// Could not convert BlockContainer to single expression
				ProjectData.SetProjectError(ex);
				return diagnostics != null;
			}).Invoke())
			{
				ex.AddAnError(diagnostics);
				ProjectData.ClearProjectError();
			}
			finally
			{
				dataFlowPass.Free();
			}
		}

		protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
		{
			return _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;
		}

		protected override void Free()
		{
			_alreadyReported = BitVector.Null;
			base.Free();
		}

		protected override string Dump(LocalState state)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[assigned ");
			AppendBitNames(state.Assigned, stringBuilder);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		protected void AppendBitNames(BitVector a, StringBuilder builder)
		{
			bool flag = false;
			foreach (int item in a.TrueBits())
			{
				if (flag)
				{
					builder.Append(", ");
				}
				flag = true;
				AppendBitName(item, builder);
			}
		}

		protected void AppendBitName(int bit, StringBuilder builder)
		{
			VariableIdentifier variableIdentifier = variableBySlot[bit];
			if (variableIdentifier.ContainingSlot > 0)
			{
				AppendBitName(variableIdentifier.ContainingSlot, builder);
				builder.Append(".");
			}
			builder.Append((bit == 0) ? "*" : variableIdentifier.Symbol.Name);
		}

		protected virtual void NoteRead(Symbol variable)
		{
			if ((object)variable != null && variable.Kind == SymbolKind.Local)
			{
				_unusedVariables.Remove((LocalSymbol)variable);
			}
		}

		protected virtual void NoteWrite(Symbol variable, BoundExpression value)
		{
			if ((object)variable != null)
			{
				_writtenVariables.Add(variable);
				LocalSymbol localSymbol = variable as LocalSymbol;
				if (value != null && (((object)localSymbol != null && !localSymbol.IsConst && localSymbol.Type.IsReferenceType) || value.HasErrors))
				{
					_unusedVariables.Remove(localSymbol);
				}
			}
		}

		protected Symbol GetNodeSymbol(BoundNode node)
		{
			while (node != null)
			{
				switch (node.Kind)
				{
				case BoundKind.FieldAccess:
				{
					BoundFieldAccess boundFieldAccess = (BoundFieldAccess)node;
					if (AbstractFlowPass<LocalState>.FieldAccessMayRequireTracking(boundFieldAccess))
					{
						node = boundFieldAccess.ReceiverOpt;
						break;
					}
					return null;
				}
				case BoundKind.PropertyAccess:
					node = ((BoundPropertyAccess)node).ReceiverOpt;
					break;
				case BoundKind.MeReference:
					return MeParameter;
				case BoundKind.Local:
					return ((BoundLocal)node).LocalSymbol;
				case BoundKind.RangeVariable:
					return ((BoundRangeVariable)node).RangeVariable;
				case BoundKind.Parameter:
					return ((BoundParameter)node).ParameterSymbol;
				case BoundKind.WithLValueExpressionPlaceholder:
				case BoundKind.WithRValueExpressionPlaceholder:
				{
					BoundExpression boundExpression = base.get_GetPlaceholderSubstitute((BoundValuePlaceholderBase)node);
					if (boundExpression != null)
					{
						return GetNodeSymbol(boundExpression);
					}
					return null;
				}
				case BoundKind.LocalDeclaration:
					return ((BoundLocalDeclaration)node).LocalSymbol;
				case BoundKind.ForToStatement:
				case BoundKind.ForEachStatement:
					return ((BoundForStatement)node).DeclaredOrInferredLocalOpt;
				case BoundKind.ByRefArgumentWithCopyBack:
					node = ((BoundByRefArgumentWithCopyBack)node).OriginalArgument;
					break;
				default:
					return null;
				}
			}
			return null;
		}

		protected virtual void NoteWrite(BoundExpression node, BoundExpression value)
		{
			Symbol nodeSymbol = GetNodeSymbol(node);
			if ((object)nodeSymbol == null)
			{
				return;
			}
			if (nodeSymbol.Kind == SymbolKind.Local && ((LocalSymbol)nodeSymbol).DeclarationKind == LocalDeclarationKind.AmbiguousLocals)
			{
				ImmutableArray<LocalSymbol>.Enumerator enumerator = ((AmbiguousLocalsPseudoSymbol)nodeSymbol).Locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					NoteWrite(current, value);
				}
			}
			else
			{
				NoteWrite(nodeSymbol, value);
			}
		}

		protected virtual void NoteRead(BoundFieldAccess fieldAccess)
		{
			Symbol nodeSymbol = GetNodeSymbol(fieldAccess);
			if ((object)nodeSymbol == null)
			{
				return;
			}
			if (nodeSymbol.Kind == SymbolKind.Local && ((LocalSymbol)nodeSymbol).DeclarationKind == LocalDeclarationKind.AmbiguousLocals)
			{
				ImmutableArray<LocalSymbol>.Enumerator enumerator = ((AmbiguousLocalsPseudoSymbol)nodeSymbol).Locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					NoteRead(current);
				}
			}
			else
			{
				NoteRead(nodeSymbol);
			}
		}

		protected int VariableSlot(Symbol symbol, int containingSlot = 0)
		{
			containingSlot = DescendThroughTupleRestFields(ref symbol, containingSlot, forceContainingSlotsToExist: false);
			if ((object)symbol == null)
			{
				return -1;
			}
			int value;
			return _variableSlot.TryGetValue(new VariableIdentifier(symbol, containingSlot), out value) ? value : (-1);
		}

		protected SlotCollection MakeSlotsForExpression(BoundExpression node)
		{
			SlotCollection result = default(SlotCollection);
			switch (node.Kind)
			{
			case BoundKind.MeReference:
			case BoundKind.MyBaseReference:
			case BoundKind.MyClassReference:
				if ((object)MeParameter != null)
				{
					result.Append(GetOrCreateSlot(MeParameter));
				}
				break;
			case BoundKind.Local:
			{
				LocalSymbol localSymbol = ((BoundLocal)node).LocalSymbol;
				if (localSymbol.DeclarationKind == LocalDeclarationKind.AmbiguousLocals)
				{
					ImmutableArray<LocalSymbol>.Enumerator enumerator = ((AmbiguousLocalsPseudoSymbol)localSymbol).Locals.GetEnumerator();
					while (enumerator.MoveNext())
					{
						LocalSymbol current = enumerator.Current;
						result.Append(GetOrCreateSlot(current));
					}
				}
				else
				{
					result.Append(GetOrCreateSlot(localSymbol));
				}
				break;
			}
			case BoundKind.RangeVariable:
				result.Append(GetOrCreateSlot(((BoundRangeVariable)node).RangeVariable));
				break;
			case BoundKind.Parameter:
				result.Append(GetOrCreateSlot(((BoundParameter)node).ParameterSymbol));
				break;
			case BoundKind.FieldAccess:
			{
				BoundFieldAccess boundFieldAccess = (BoundFieldAccess)node;
				FieldSymbol fieldSymbol = boundFieldAccess.FieldSymbol;
				BoundExpression receiverOpt = boundFieldAccess.ReceiverOpt;
				if (!fieldSymbol.IsShared && receiverOpt != null && receiverOpt.Kind != BoundKind.TypeExpression && (object)receiverOpt.Type != null && receiverOpt.Type.TypeKind == TypeKind.Struct)
				{
					result = MakeSlotsForExpression(receiverOpt);
					int num = result.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						result[i] = GetOrCreateSlot(boundFieldAccess.FieldSymbol, result[0]);
					}
				}
				break;
			}
			case BoundKind.WithLValueExpressionPlaceholder:
			case BoundKind.WithRValueExpressionPlaceholder:
			{
				BoundExpression boundExpression = base.get_GetPlaceholderSubstitute((BoundValuePlaceholderBase)node);
				if (boundExpression != null)
				{
					return MakeSlotsForExpression(boundExpression);
				}
				break;
			}
			}
			return result;
		}

		protected int GetOrCreateSlot(Symbol symbol, int containingSlot = 0)
		{
			containingSlot = DescendThroughTupleRestFields(ref symbol, containingSlot, forceContainingSlotsToExist: true);
			if (containingSlot == -1)
			{
				return -1;
			}
			VariableIdentifier variableIdentifier = new VariableIdentifier(symbol, containingSlot);
			if (!_variableSlot.TryGetValue(variableIdentifier, out var value))
			{
				if (symbol.Kind == SymbolKind.Local)
				{
					_unusedVariables.Add((LocalSymbol)symbol);
				}
				TypeSymbol variableType = GetVariableType(symbol);
				if (IsEmptyStructType(variableType))
				{
					return -1;
				}
				value = nextVariableSlot;
				nextVariableSlot++;
				_variableSlot.Add(variableIdentifier, value);
				if (value >= variableBySlot.Length)
				{
					Array.Resize(ref variableBySlot, value * 2);
				}
				variableBySlot[value] = variableIdentifier;
			}
			Normalize(ref State);
			return value;
		}

		private int DescendThroughTupleRestFields(ref Symbol symbol, int containingSlot, bool forceContainingSlotsToExist)
		{
			if (symbol is TupleFieldSymbol tupleFieldSymbol)
			{
				TypeSymbol typeSymbol = ((TupleTypeSymbol)symbol.ContainingType).UnderlyingNamedType;
				symbol = tupleFieldSymbol.TupleUnderlyingField;
				while (!TypeSymbol.Equals(typeSymbol, symbol.ContainingType, TypeCompareKind.ConsiderEverything))
				{
					if (!(typeSymbol.GetMembers("Rest").FirstOrDefault() is FieldSymbol fieldSymbol))
					{
						return -1;
					}
					if (forceContainingSlotsToExist)
					{
						containingSlot = GetOrCreateSlot(fieldSymbol, containingSlot);
					}
					else if (!_variableSlot.TryGetValue(new VariableIdentifier(fieldSymbol, containingSlot), out containingSlot))
					{
						return -1;
					}
					typeSymbol = TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(fieldSymbol.Type);
				}
			}
			return containingSlot;
		}

		protected virtual bool IsEmptyStructType(TypeSymbol type)
		{
			if (!(type is NamedTypeSymbol namedTypeSymbol))
			{
				return false;
			}
			if (!IsTrackableStructType(namedTypeSymbol))
			{
				return false;
			}
			bool value = false;
			if (_isEmptyStructType.TryGetValue(namedTypeSymbol, out value))
			{
				return value;
			}
			_isEmptyStructType[namedTypeSymbol] = true;
			ImmutableArray<FieldSymbol>.Enumerator enumerator = GetStructInstanceFields(namedTypeSymbol).GetEnumerator();
			while (enumerator.MoveNext())
			{
				FieldSymbol current = enumerator.Current;
				if (!IsEmptyStructType(current.Type))
				{
					_isEmptyStructType[namedTypeSymbol] = false;
					return false;
				}
			}
			_isEmptyStructType[namedTypeSymbol] = true;
			return true;
		}

		private static bool IsTrackableStructType(TypeSymbol symbol)
		{
			if (AbstractFlowPass<LocalState>.IsNonPrimitiveValueType(symbol))
			{
				return symbol.OriginalDefinition is NamedTypeSymbol namedTypeSymbol && !namedTypeSymbol.KnownCircularStruct;
			}
			return false;
		}

		private bool IsSlotAlreadyReported(TypeSymbol symbolType, int slot)
		{
			if (_alreadyReported[slot])
			{
				return true;
			}
			if (slot <= 1)
			{
				return false;
			}
			if (!IsTrackableStructType(symbolType))
			{
				return false;
			}
			ImmutableArray<FieldSymbol>.Enumerator enumerator = GetStructInstanceFields(symbolType).GetEnumerator();
			while (enumerator.MoveNext())
			{
				FieldSymbol current = enumerator.Current;
				int num = VariableSlot(current, slot);
				if (num != -1)
				{
					if (!IsSlotAlreadyReported(current.Type, num))
					{
						return false;
					}
					continue;
				}
				return false;
			}
			_alreadyReported[slot] = true;
			return true;
		}

		private void MarkSlotAsReported(TypeSymbol symbolType, int slot)
		{
			if (_alreadyReported[slot])
			{
				return;
			}
			_alreadyReported[slot] = true;
			if (slot <= 1 || !IsTrackableStructType(symbolType))
			{
				return;
			}
			ImmutableArray<FieldSymbol>.Enumerator enumerator = GetStructInstanceFields(symbolType).GetEnumerator();
			while (enumerator.MoveNext())
			{
				FieldSymbol current = enumerator.Current;
				int num = VariableSlot(current, slot);
				if (num != -1)
				{
					MarkSlotAsReported(current.Type, num);
				}
			}
		}

		private void SetSlotUnassigned(int slot)
		{
			if (slot >= State.Assigned.Capacity)
			{
				Normalize(ref State);
			}
			if (_tryState.HasValue)
			{
				LocalState state = _tryState.Value;
				SetSlotUnassigned(slot, ref state);
				_tryState = state;
			}
			SetSlotUnassigned(slot, ref State);
		}

		private void SetSlotUnassigned(int slot, ref LocalState state)
		{
			VariableIdentifier variableIdentifier = variableBySlot[slot];
			TypeSymbol variableType = GetVariableType(variableIdentifier.Symbol);
			state.Unassign(slot);
			if (IsTrackableStructType(variableType))
			{
				ImmutableArray<FieldSymbol>.Enumerator enumerator = GetStructInstanceFields(variableType).GetEnumerator();
				while (enumerator.MoveNext())
				{
					FieldSymbol current = enumerator.Current;
					int num = VariableSlot(current, slot);
					if (num >= 2)
					{
						SetSlotUnassigned(num, ref state);
					}
				}
			}
			while (variableIdentifier.ContainingSlot > 0)
			{
				int containingSlot = variableIdentifier.ContainingSlot;
				VariableIdentifier variableIdentifier2 = variableBySlot[containingSlot];
				Symbol symbol = variableIdentifier2.Symbol;
				if (state.IsAssigned(containingSlot))
				{
					state.Unassign(containingSlot);
					if (symbol.Kind == SymbolKind.Local && ((LocalSymbol)symbol).IsFunctionValue)
					{
						state.Unassign(1);
					}
					variableIdentifier = variableIdentifier2;
					continue;
				}
				break;
			}
		}

		private void SetSlotAssigned(int slot, ref LocalState state)
		{
			VariableIdentifier variableIdentifier = variableBySlot[slot];
			TypeSymbol variableType = GetVariableType(variableIdentifier.Symbol);
			if (slot >= State.Assigned.Capacity)
			{
				Normalize(ref State);
			}
			if (state.IsAssigned(slot))
			{
				return;
			}
			state.Assign(slot);
			if (IsTrackableStructType(variableType))
			{
				ImmutableArray<FieldSymbol>.Enumerator enumerator = GetStructInstanceFields(variableType).GetEnumerator();
				while (enumerator.MoveNext())
				{
					FieldSymbol current = enumerator.Current;
					int num = VariableSlot(current, slot);
					if (num >= 2)
					{
						SetSlotAssigned(num, ref state);
					}
				}
			}
			while (variableIdentifier.ContainingSlot > 0)
			{
				int containingSlot = variableIdentifier.ContainingSlot;
				VariableIdentifier variableIdentifier2 = variableBySlot[containingSlot];
				Symbol symbol = variableIdentifier2.Symbol;
				TypeSymbol variableType2 = GetVariableType(symbol);
				ImmutableArray<FieldSymbol>.Enumerator enumerator2 = GetStructInstanceFields(variableType2).GetEnumerator();
				while (enumerator2.MoveNext())
				{
					FieldSymbol current2 = enumerator2.Current;
					int orCreateSlot = GetOrCreateSlot(current2, containingSlot);
					if (orCreateSlot != -1 && !state.IsAssigned(orCreateSlot))
					{
						return;
					}
				}
				state.Assign(containingSlot);
				if (symbol.Kind == SymbolKind.Local && ((LocalSymbol)symbol).IsFunctionValue)
				{
					state.Assign(1);
				}
				variableIdentifier = variableIdentifier2;
			}
		}

		private void SetSlotAssigned(int slot)
		{
			SetSlotAssigned(slot, ref State);
		}

		private bool ShouldIgnoreStructField(FieldSymbol field)
		{
			if (field.IsShared)
			{
				return true;
			}
			if (_trackStructsWithIntrinsicTypedFields)
			{
				return false;
			}
			TypeSymbol type = field.Type;
			if (TypeSymbolExtensions.IsIntrinsicValueType(type))
			{
				return true;
			}
			if (TypeSymbolExtensions.IsTypeParameter(type) && !((TypeParameterSymbol)type).IsReferenceType)
			{
				return true;
			}
			if (field.DeclaredAccessibility != Accessibility.Private)
			{
				return false;
			}
			if (field.Dangerous_IsFromSomeCompilationIncludingRetargeting)
			{
				Symbol associatedSymbol = field.AssociatedSymbol;
				if ((object)associatedSymbol == null || associatedSymbol.Kind != SymbolKind.Event)
				{
					return false;
				}
			}
			if (field.ContainingType.IsGenericType)
			{
				return false;
			}
			return true;
		}

		private ImmutableArray<FieldSymbol> GetStructInstanceFields(TypeSymbol type)
		{
			ImmutableArray<FieldSymbol> value = default(ImmutableArray<FieldSymbol>);
			if (_typeToMembersCache == null || !_typeToMembersCache.TryGetValue(type, out value))
			{
				ArrayBuilder<FieldSymbol> instance = ArrayBuilder<FieldSymbol>.GetInstance();
				ImmutableArray<Symbol>.Enumerator enumerator = type.GetMembersUnordered().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.Kind == SymbolKind.Field)
					{
						FieldSymbol fieldSymbol = (FieldSymbol)current;
						if (!fieldSymbol.IsVirtualTupleField && !ShouldIgnoreStructField(fieldSymbol))
						{
							instance.Add(fieldSymbol);
						}
					}
				}
				value = instance.ToImmutableAndFree();
				if (_typeToMembersCache == null)
				{
					_typeToMembersCache = new Dictionary<TypeSymbol, ImmutableArray<FieldSymbol>>();
				}
				_typeToMembersCache.Add(type, value);
			}
			return value;
		}

		private static TypeSymbol GetVariableType(Symbol symbol)
		{
			return symbol.Kind switch
			{
				SymbolKind.Local => ((LocalSymbol)symbol).Type, 
				SymbolKind.RangeVariable => ((RangeVariableSymbol)symbol).Type, 
				SymbolKind.Field => ((FieldSymbol)symbol).Type, 
				SymbolKind.Parameter => ((ParameterSymbol)symbol).Type, 
				_ => throw ExceptionUtilities.UnexpectedValue(symbol.Kind), 
			};
		}

		protected void SetSlotState(int slot, bool assigned)
		{
			switch (slot)
			{
			case 1:
				if (assigned)
				{
					State.Assign(slot);
				}
				else
				{
					State.Unassign(slot);
				}
				break;
			default:
				if (assigned)
				{
					SetSlotAssigned(slot);
				}
				else
				{
					SetSlotUnassigned(slot);
				}
				break;
			case -1:
				break;
			}
		}

		protected void CheckAssigned(Symbol symbol, SyntaxNode node, ReadWriteContext rwContext = ReadWriteContext.None)
		{
			if ((object)symbol == null)
			{
				return;
			}
			LocalSymbol localSymbol = symbol as LocalSymbol;
			if ((object)localSymbol != null && localSymbol.IsCompilerGenerated && !ProcessCompilerGeneratedLocals)
			{
				return;
			}
			if ((object)localSymbol != null && localSymbol.DeclarationKind == LocalDeclarationKind.AmbiguousLocals)
			{
				AmbiguousLocalsPseudoSymbol ambiguousLocalsPseudoSymbol = (AmbiguousLocalsPseudoSymbol)localSymbol;
				VisitAmbiguousLocalSymbol(ambiguousLocalsPseudoSymbol);
				ImmutableArray<LocalSymbol>.Enumerator enumerator = ambiguousLocalsPseudoSymbol.Locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocalSymbol current = enumerator.Current;
					NoteRead(current);
				}
				return;
			}
			int orCreateSlot = GetOrCreateSlot(symbol);
			if (orCreateSlot >= State.Assigned.Capacity)
			{
				Normalize(ref State);
			}
			if (orCreateSlot >= 2 && State.Reachable && !State.IsAssigned(orCreateSlot))
			{
				ReportUnassigned(symbol, node, rwContext);
			}
			NoteRead(symbol);
		}

		private void CheckAssigned(BoundFieldAccess fieldAccess, SyntaxNode node, ReadWriteContext rwContext = ReadWriteContext.None)
		{
			int unassignedSlot = default(int);
			if (State.Reachable && !IsAssigned(fieldAccess, ref unassignedSlot))
			{
				ReportUnassigned(fieldAccess.FieldSymbol, node, rwContext, unassignedSlot, fieldAccess);
			}
			NoteRead(fieldAccess);
		}

		protected virtual void VisitAmbiguousLocalSymbol(AmbiguousLocalsPseudoSymbol ambiguous)
		{
		}

		protected override void VisitLvalue(BoundExpression node, bool dontLeaveRegion = false)
		{
			base.VisitLvalue(node, dontLeaveRegion: true);
			if (node.Kind == BoundKind.Local)
			{
				LocalSymbol localSymbol = ((BoundLocal)node).LocalSymbol;
				if (localSymbol.DeclarationKind == LocalDeclarationKind.AmbiguousLocals)
				{
					VisitAmbiguousLocalSymbol((AmbiguousLocalsPseudoSymbol)localSymbol);
				}
			}
			if (!dontLeaveRegion && node == _lastInRegion && base.IsInside)
			{
				LeaveRegion();
			}
		}

		private bool IsAssigned(BoundExpression node, ref int unassignedSlot)
		{
			unassignedSlot = -1;
			if (IsEmptyStructType(node.Type))
			{
				return true;
			}
			switch (node.Kind)
			{
			case BoundKind.MeReference:
				unassignedSlot = VariableSlot(MeParameter);
				break;
			case BoundKind.Local:
			{
				LocalSymbol localSymbol = ((BoundLocal)node).LocalSymbol;
				if (localSymbol.DeclarationKind != LocalDeclarationKind.AmbiguousLocals)
				{
					unassignedSlot = VariableSlot(localSymbol);
					break;
				}
				unassignedSlot = -1;
				VisitAmbiguousLocalSymbol((AmbiguousLocalsPseudoSymbol)localSymbol);
				break;
			}
			case BoundKind.RangeVariable:
				unassignedSlot = -1;
				return true;
			case BoundKind.Parameter:
				unassignedSlot = VariableSlot(((BoundParameter)node).ParameterSymbol);
				break;
			case BoundKind.FieldAccess:
			{
				BoundFieldAccess boundFieldAccess = (BoundFieldAccess)node;
				if (!AbstractFlowPass<LocalState>.FieldAccessMayRequireTracking(boundFieldAccess) || IsAssigned(boundFieldAccess.ReceiverOpt, ref unassignedSlot))
				{
					return true;
				}
				unassignedSlot = GetOrCreateSlot(boundFieldAccess.FieldSymbol, unassignedSlot);
				break;
			}
			case BoundKind.WithLValueExpressionPlaceholder:
			case BoundKind.WithRValueExpressionPlaceholder:
			{
				BoundExpression boundExpression = base.get_GetPlaceholderSubstitute((BoundValuePlaceholderBase)node);
				if (boundExpression != null)
				{
					return IsAssigned(boundExpression, ref unassignedSlot);
				}
				unassignedSlot = -1;
				break;
			}
			default:
				unassignedSlot = -1;
				break;
			}
			return State.IsAssigned(unassignedSlot);
		}

		private Location GetUnassignedSymbolFirstLocation(Symbol sym, BoundFieldAccess boundFieldAccess)
		{
			switch (sym.Kind)
			{
			case SymbolKind.Parameter:
				return null;
			case SymbolKind.RangeVariable:
				return null;
			case SymbolKind.Local:
			{
				ImmutableArray<Location> locations = sym.Locations;
				return locations.IsEmpty ? null : locations[0];
			}
			case SymbolKind.Field:
			{
				if (sym.IsShared)
				{
					return null;
				}
				BoundExpression receiverOpt = boundFieldAccess.ReceiverOpt;
				switch (receiverOpt.Kind)
				{
				case BoundKind.Local:
					return GetUnassignedSymbolFirstLocation(((BoundLocal)receiverOpt).LocalSymbol, null);
				case BoundKind.FieldAccess:
				{
					BoundFieldAccess boundFieldAccess2 = (BoundFieldAccess)receiverOpt;
					return GetUnassignedSymbolFirstLocation(boundFieldAccess2.FieldSymbol, boundFieldAccess2);
				}
				default:
					return null;
				}
			}
			default:
				return null;
			}
		}

		protected virtual void ReportUnassigned(Symbol sym, SyntaxNode node, ReadWriteContext rwContext, int slot = -1, BoundFieldAccess boundFieldAccess = null)
		{
			if (slot < 2)
			{
				slot = VariableSlot(sym);
			}
			if (slot >= _alreadyReported.Capacity)
			{
				_alreadyReported.EnsureCapacity(nextVariableSlot);
			}
			if (sym.Kind == SymbolKind.Parameter || sym.Kind == SymbolKind.RangeVariable)
			{
				return;
			}
			bool flag = false;
			TypeSymbol type;
			bool flag2;
			bool flag3;
			if (sym.Kind == SymbolKind.Local)
			{
				LocalSymbol obj = (LocalSymbol)sym;
				type = obj.Type;
				flag2 = obj.IsFunctionValue && EnableBreakingFlowAnalysisFeatures;
				flag3 = obj.IsStatic;
				flag = obj.IsImplicitlyDeclared;
			}
			else
			{
				flag2 = false;
				flag3 = false;
				type = ((FieldSymbol)sym).Type;
			}
			if (IsSlotAlreadyReported(type, slot))
			{
				return;
			}
			ERRID eRRID = ERRID.ERR_None;
			Location unassignedSymbolFirstLocation = GetUnassignedSymbolFirstLocation(sym, boundFieldAccess);
			if (!flag && (object)unassignedSymbolFirstLocation != null && unassignedSymbolFirstLocation.SourceSpan.Start >= node.SpanStart)
			{
				return;
			}
			if ((object)type != null && !flag3)
			{
				if (TypeSymbolExtensions.IsIntrinsicValueType(type) || type.IsReferenceType)
				{
					eRRID = ((!TypeSymbolExtensions.IsIntrinsicValueType(type) || flag2) ? ((rwContext == ReadWriteContext.ByRefArgument) ? ERRID.WRN_DefAsgUseNullRefByRef : ERRID.WRN_DefAsgUseNullRef) : ERRID.ERR_None);
				}
				else if (type.IsValueType)
				{
					eRRID = ((rwContext == ReadWriteContext.ByRefArgument) ? ERRID.WRN_DefAsgUseNullRefByRefStr : ERRID.WRN_DefAsgUseNullRefStr);
				}
				if (eRRID != 0)
				{
					DiagnosticBagExtensions.Add(diagnostics, eRRID, node.GetLocation(), sym.Name ?? "dummy");
				}
			}
			MarkSlotAsReported(type, slot);
		}

		private void CheckAssignedFunctionValue(LocalSymbol local, SyntaxNode node)
		{
			if (!State.FunctionAssignedValue && !_seenOnErrorOrResume)
			{
				TypeSymbol type = local.Type;
				if (EnableBreakingFlowAnalysisFeatures || !type.IsValueType || TypeSymbolExtensions.IsIntrinsicOrEnumType(type) || !IsEmptyStructType(type) || (base.MethodSymbol.MethodKind == MethodKind.EventAdd && ((EventSymbol)base.MethodSymbol.AssociatedSymbol).IsWindowsRuntimeEvent))
				{
					ReportUnassignedFunctionValue(local, node);
				}
			}
		}

		private void ReportUnassignedFunctionValue(LocalSymbol local, SyntaxNode node)
		{
			if (_alreadyReported[1])
			{
				return;
			}
			TypeSymbol typeSymbol = null;
			MethodSymbol methodSymbol = base.MethodSymbol;
			typeSymbol = methodSymbol.ReturnType;
			if ((object)typeSymbol != null && !methodSymbol.IsIterator && (!methodSymbol.IsAsync || !typeSymbol.Equals(compilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task))))
			{
				ERRID eRRID = ERRID.ERR_None;
				string text = GetFunctionLocalName(methodSymbol, local);
				typeSymbol = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(typeSymbol);
				if (TypeSymbolExtensions.IsIntrinsicValueType(typeSymbol))
				{
					switch (base.MethodSymbol.MethodKind)
					{
					case MethodKind.Conversion:
					case MethodKind.UserDefinedOperator:
						eRRID = ERRID.WRN_DefAsgNoRetValOpVal1;
						break;
					case MethodKind.PropertyGet:
						eRRID = ERRID.WRN_DefAsgNoRetValPropVal1;
						break;
					default:
						eRRID = ERRID.WRN_DefAsgNoRetValFuncVal1;
						break;
					}
				}
				else if (typeSymbol.IsReferenceType)
				{
					switch (base.MethodSymbol.MethodKind)
					{
					case MethodKind.Conversion:
					case MethodKind.UserDefinedOperator:
						eRRID = ERRID.WRN_DefAsgNoRetValOpRef1;
						break;
					case MethodKind.PropertyGet:
						eRRID = ERRID.WRN_DefAsgNoRetValPropRef1;
						break;
					default:
						eRRID = ERRID.WRN_DefAsgNoRetValFuncRef1;
						break;
					}
				}
				else if (typeSymbol.TypeKind != TypeKind.TypeParameter)
				{
					switch (base.MethodSymbol.MethodKind)
					{
					case MethodKind.PropertyGet:
						eRRID = ERRID.WRN_DefAsgNoRetValPropRef1;
						break;
					case MethodKind.EventAdd:
						eRRID = ERRID.WRN_DefAsgNoRetValWinRtEventVal1;
						text = base.MethodSymbol.AssociatedSymbol.Name;
						break;
					default:
						eRRID = ERRID.WRN_DefAsgNoRetValFuncRef1;
						break;
					case MethodKind.Conversion:
					case MethodKind.UserDefinedOperator:
						break;
					}
				}
				if (eRRID != 0)
				{
					DiagnosticBagExtensions.Add(diagnostics, eRRID, node.GetLocation(), text);
				}
			}
			_alreadyReported[1] = true;
		}

		private static string GetFunctionLocalName(MethodSymbol method, LocalSymbol local)
		{
			switch (method.MethodKind)
			{
			case MethodKind.AnonymousFunction:
				return "<anonymous method>";
			case MethodKind.Conversion:
			case MethodKind.UserDefinedOperator:
				return ((OperatorBlockSyntax)((SourceMemberMethodSymbol)local.ContainingSymbol).BlockSyntax).OperatorStatement.OperatorToken.Text;
			default:
				return local.Name ?? method.Name;
			}
		}

		protected virtual void Assign(BoundNode node, BoundExpression value, bool assigned = true)
		{
			switch (node.Kind)
			{
			case BoundKind.LocalDeclaration:
			{
				BoundLocalDeclaration boundLocalDeclaration = (BoundLocalDeclaration)node;
				LocalSymbol localSymbol = boundLocalDeclaration.LocalSymbol;
				int orCreateSlot = GetOrCreateSlot(localSymbol);
				bool assigned2 = assigned || !State.Reachable;
				SetSlotState(orCreateSlot, assigned2);
				if (assigned && (value != null || boundLocalDeclaration.InitializedByAsNew))
				{
					NoteWrite(localSymbol, value);
				}
				break;
			}
			case BoundKind.ForToStatement:
			case BoundKind.ForEachStatement:
			{
				LocalSymbol declaredOrInferredLocalOpt = ((BoundForStatement)node).DeclaredOrInferredLocalOpt;
				int orCreateSlot3 = GetOrCreateSlot(declaredOrInferredLocalOpt);
				bool assigned3 = assigned || !State.Reachable;
				SetSlotState(orCreateSlot3, assigned3);
				break;
			}
			case BoundKind.Local:
			{
				LocalSymbol localSymbol2 = ((BoundLocal)node).LocalSymbol;
				if (!localSymbol2.IsCompilerGenerated || ProcessCompilerGeneratedLocals)
				{
					int orCreateSlot4 = GetOrCreateSlot(localSymbol2);
					SetSlotState(orCreateSlot4, assigned);
					if (localSymbol2.IsFunctionValue)
					{
						SetSlotState(1, assigned);
					}
					if (assigned)
					{
						NoteWrite(localSymbol2, value);
					}
				}
				break;
			}
			case BoundKind.Parameter:
			{
				ParameterSymbol parameterSymbol = ((BoundParameter)node).ParameterSymbol;
				int orCreateSlot2 = GetOrCreateSlot(parameterSymbol);
				SetSlotState(orCreateSlot2, assigned);
				if (assigned)
				{
					NoteWrite(parameterSymbol, value);
				}
				break;
			}
			case BoundKind.MeReference:
			{
				int orCreateSlot5 = GetOrCreateSlot(MeParameter);
				SetSlotState(orCreateSlot5, assigned);
				if (assigned)
				{
					NoteWrite(MeParameter, value);
				}
				break;
			}
			case BoundKind.FieldAccess:
			case BoundKind.PropertyAccess:
			{
				BoundExpression node2 = (BoundExpression)node;
				SlotCollection slotCollection = MakeSlotsForExpression(node2);
				int num = slotCollection.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					SetSlotState(slotCollection[i], assigned);
				}
				slotCollection.Free();
				if (assigned)
				{
					NoteWrite(node2, value);
				}
				break;
			}
			case BoundKind.WithLValueExpressionPlaceholder:
			case BoundKind.WithRValueExpressionPlaceholder:
			{
				BoundExpression boundExpression = base.get_GetPlaceholderSubstitute((BoundValuePlaceholderBase)node);
				if (boundExpression != null)
				{
					Assign(boundExpression, value, assigned);
				}
				break;
			}
			case BoundKind.ByRefArgumentWithCopyBack:
				Assign(((BoundByRefArgumentWithCopyBack)node).OriginalArgument, value, assigned);
				break;
			}
		}

		protected override void VisitTryBlock(BoundStatement tryBlock, BoundTryStatement node, ref LocalState _tryState)
		{
			if (TrackUnassignments)
			{
				LocalState? tryState = this._tryState;
				this._tryState = AllBitsSet();
				base.VisitTryBlock(tryBlock, node, ref _tryState);
				LocalState other = this._tryState.Value;
				IntersectWith(ref _tryState, ref other);
				if (tryState.HasValue)
				{
					LocalState self = this._tryState.Value;
					other = tryState.Value;
					IntersectWith(ref self, ref other);
					this._tryState = self;
				}
				else
				{
					this._tryState = tryState;
				}
			}
			else
			{
				base.VisitTryBlock(tryBlock, node, ref _tryState);
			}
		}

		protected override void VisitCatchBlock(BoundCatchBlock catchBlock, ref LocalState finallyState)
		{
			if (TrackUnassignments)
			{
				LocalState? tryState = _tryState;
				_tryState = AllBitsSet();
				VisitCatchBlockInternal(catchBlock, ref finallyState);
				LocalState other = _tryState.Value;
				IntersectWith(ref finallyState, ref other);
				if (tryState.HasValue)
				{
					LocalState self = _tryState.Value;
					other = tryState.Value;
					IntersectWith(ref self, ref other);
					_tryState = self;
				}
				else
				{
					_tryState = tryState;
				}
			}
			else
			{
				VisitCatchBlockInternal(catchBlock, ref finallyState);
			}
		}

		private void VisitCatchBlockInternal(BoundCatchBlock catchBlock, ref LocalState finallyState)
		{
			LocalSymbol localOpt = catchBlock.LocalOpt;
			if ((object)localOpt != null)
			{
				GetOrCreateSlot(localOpt);
			}
			BoundExpression exceptionSourceOpt = catchBlock.ExceptionSourceOpt;
			if (exceptionSourceOpt != null)
			{
				Assign(exceptionSourceOpt, null);
			}
			base.VisitCatchBlock(catchBlock, ref finallyState);
		}

		protected override void VisitFinallyBlock(BoundStatement finallyBlock, ref LocalState unsetInFinally)
		{
			if (TrackUnassignments)
			{
				LocalState? tryState = _tryState;
				_tryState = AllBitsSet();
				base.VisitFinallyBlock(finallyBlock, ref unsetInFinally);
				LocalState other = _tryState.Value;
				IntersectWith(ref unsetInFinally, ref other);
				if (tryState.HasValue)
				{
					LocalState self = _tryState.Value;
					other = tryState.Value;
					IntersectWith(ref self, ref other);
					_tryState = self;
				}
				else
				{
					_tryState = tryState;
				}
			}
			else
			{
				base.VisitFinallyBlock(finallyBlock, ref unsetInFinally);
			}
		}

		protected override LocalState ReachableState()
		{
			return new LocalState(BitVector.Empty);
		}

		private void EnterParameters(ImmutableArray<ParameterSymbol> parameters)
		{
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				EnterParameter(current);
			}
		}

		protected virtual void EnterParameter(ParameterSymbol parameter)
		{
			int num = VariableSlot(parameter);
			if (num >= 2)
			{
				State.Assign(num);
			}
			NoteWrite(parameter, null);
		}

		private void LeaveParameters(ImmutableArray<ParameterSymbol> parameters)
		{
			if (State.Reachable)
			{
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					LeaveParameter(current);
				}
			}
		}

		private void LeaveParameter(ParameterSymbol parameter)
		{
			if (parameter.IsByRef)
			{
				int slot = VariableSlot(parameter);
				if (!State.IsAssigned(slot))
				{
					ReportUnassignedByRefParameter(parameter);
				}
				NoteRead(parameter);
			}
		}

		protected override LocalState UnreachableState()
		{
			return new LocalState(UnreachableBitsSet);
		}

		protected override LocalState AllBitsSet()
		{
			LocalState result = new LocalState(BitVector.AllSet(nextVariableSlot));
			result.Unassign(0);
			return result;
		}

		protected override void VisitLocalInReadWriteContext(BoundLocal node, ReadWriteContext rwContext)
		{
			base.VisitLocalInReadWriteContext(node, rwContext);
			CheckAssigned(node.LocalSymbol, node.Syntax, rwContext);
		}

		public override BoundNode VisitLocal(BoundLocal node)
		{
			CheckAssigned(node.LocalSymbol, node.Syntax);
			return null;
		}

		public override BoundNode VisitRangeVariable(BoundRangeVariable node)
		{
			if (!node.WasCompilerGenerated)
			{
				CheckAssigned(node.RangeVariable, node.Syntax);
			}
			return null;
		}

		protected bool ConsiderLocalInitiallyAssigned(LocalSymbol variable)
		{
			if (State.Reachable)
			{
				if (initiallyAssignedVariables != null)
				{
					return initiallyAssignedVariables.Contains(variable);
				}
				return false;
			}
			return true;
		}

		public override BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
		{
			BoundValuePlaceholderBase placeholder = null;
			bool flag = DeclaredVariableIsAlwaysAssignedBeforeInitializer(node.Syntax.Parent, node.InitializerOpt, out placeholder);
			LocalSymbol localSymbol = node.LocalSymbol;
			if (flag)
			{
				SetPlaceholderSubstitute(placeholder, new BoundLocal(node.Syntax, localSymbol, localSymbol.Type));
			}
			GetOrCreateSlot(localSymbol);
			Assign(node, node.InitializerOpt, ConsiderLocalInitiallyAssigned(localSymbol) || flag || TreatTheLocalAsAssignedWithinTheLambda(localSymbol, node.InitializerOpt));
			if (node.InitializerOpt != null || node.InitializedByAsNew)
			{
				base.VisitLocalDeclaration(node);
			}
			AssignLocalOnDeclaration(localSymbol, node);
			if (flag)
			{
				RemovePlaceholderSubstitute(placeholder);
			}
			return null;
		}

		protected virtual bool TreatTheLocalAsAssignedWithinTheLambda(LocalSymbol local, BoundExpression right)
		{
			return IsConvertedLambda(right);
		}

		private static bool IsConvertedLambda(BoundExpression value)
		{
			if (value == null)
			{
				return false;
			}
			while (true)
			{
				switch (value.Kind)
				{
				case BoundKind.Lambda:
					return true;
				case BoundKind.Conversion:
					value = ((BoundConversion)value).Operand;
					break;
				case BoundKind.DirectCast:
					value = ((BoundDirectCast)value).Operand;
					break;
				case BoundKind.TryCast:
					value = ((BoundTryCast)value).Operand;
					break;
				case BoundKind.Parenthesized:
					value = ((BoundParenthesized)value).Expression;
					break;
				default:
					return false;
				}
			}
		}

		internal virtual void AssignLocalOnDeclaration(LocalSymbol local, BoundLocalDeclaration node)
		{
			if (node.InitializerOpt != null || node.InitializedByAsNew)
			{
				Assign(node, node.InitializerOpt);
			}
		}

		public override BoundNode VisitBlock(BoundBlock node)
		{
			ImmutableArray<LocalSymbol>.Enumerator enumerator = node.Locals.GetEnumerator();
			while (enumerator.MoveNext())
			{
				LocalSymbol current = enumerator.Current;
				if (current.IsImplicitlyDeclared)
				{
					SetSlotState(GetOrCreateSlot(current), ConsiderLocalInitiallyAssigned(current));
				}
			}
			return base.VisitBlock(node);
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			SavedPending oldPending = SavePending();
			Symbol symbol = base.symbol;
			base.symbol = node.LambdaSymbol;
			LocalState self = State;
			SetState(self.Reachable ? self.Clone() : AllBitsSet());
			State.Assigned[1] = false;
			bool value = _alreadyReported[1];
			_alreadyReported[1] = false;
			EnterParameters(node.LambdaSymbol.Parameters);
			VisitBlock(node.Body);
			LeaveParameters(node.LambdaSymbol.Parameters);
			base.symbol = symbol;
			_alreadyReported[1] = value;
			State.Assigned[1] = true;
			RestorePending(oldPending);
			IntersectWith(ref self, ref State);
			SetState(self);
			return null;
		}

		public override BoundNode VisitQueryLambda(BoundQueryLambda node)
		{
			Symbol symbol = base.symbol;
			base.symbol = node.LambdaSymbol;
			LocalState self = State.Clone();
			VisitRvalue(node.Expression);
			base.symbol = symbol;
			IntersectWith(ref self, ref State);
			SetState(self);
			return null;
		}

		public override BoundNode VisitQueryExpression(BoundQueryExpression node)
		{
			VisitRvalue(node.LastOperator);
			return null;
		}

		public override BoundNode VisitQuerySource(BoundQuerySource node)
		{
			VisitRvalue(node.Expression);
			return null;
		}

		public override BoundNode VisitQueryableSource(BoundQueryableSource node)
		{
			VisitRvalue(node.Source);
			return null;
		}

		public override BoundNode VisitToQueryableCollectionConversion(BoundToQueryableCollectionConversion node)
		{
			VisitRvalue(node.ConversionCall);
			return null;
		}

		public override BoundNode VisitQueryClause(BoundQueryClause node)
		{
			VisitRvalue(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitAggregateClause(BoundAggregateClause node)
		{
			if (node.CapturedGroupOpt != null)
			{
				VisitRvalue(node.CapturedGroupOpt);
			}
			VisitRvalue(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitOrdering(BoundOrdering node)
		{
			VisitRvalue(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitRangeVariableAssignment(BoundRangeVariableAssignment node)
		{
			VisitRvalue(node.Value);
			return null;
		}

		public override BoundNode VisitMeReference(BoundMeReference node)
		{
			CheckAssigned(MeParameter, node.Syntax);
			return null;
		}

		public override BoundNode VisitParameter(BoundParameter node)
		{
			if (!node.WasCompilerGenerated)
			{
				CheckAssigned(node.ParameterSymbol, node.Syntax);
			}
			return null;
		}

		public override BoundNode VisitFieldAccess(BoundFieldAccess node)
		{
			BoundNode result = base.VisitFieldAccess(node);
			if (AbstractFlowPass<LocalState>.FieldAccessMayRequireTracking(node))
			{
				CheckAssigned(node, node.Syntax);
			}
			return result;
		}

		protected override void VisitFieldAccessInReadWriteContext(BoundFieldAccess node, ReadWriteContext rwContext)
		{
			base.VisitFieldAccessInReadWriteContext(node, rwContext);
			if (AbstractFlowPass<LocalState>.FieldAccessMayRequireTracking(node))
			{
				CheckAssigned(node, node.Syntax, rwContext);
			}
		}

		public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
		{
			BoundExpression left = node.Left;
			bool flag = false;
			if (left.Kind == BoundKind.Local)
			{
				flag = TreatTheLocalAsAssignedWithinTheLambda(((BoundLocal)left).LocalSymbol, node.Right);
			}
			if (flag)
			{
				Assign(left, node.Right);
			}
			base.VisitAssignmentOperator(node);
			if (!flag)
			{
				Assign(left, node.Right);
			}
			return null;
		}

		public override BoundNode VisitRedimClause(BoundRedimClause node)
		{
			base.VisitRedimClause(node);
			Assign(node.Operand, null);
			return null;
		}

		public override BoundNode VisitReferenceAssignment(BoundReferenceAssignment node)
		{
			base.VisitReferenceAssignment(node);
			Assign(node.ByRefLocal, node.LValue);
			return null;
		}

		protected override void VisitForControlInitialization(BoundForToStatement node)
		{
			base.VisitForControlInitialization(node);
			Assign(node.ControlVariable, node.InitialValue);
		}

		protected override void VisitForControlInitialization(BoundForEachStatement node)
		{
			base.VisitForControlInitialization(node);
			Assign(node.ControlVariable, null);
		}

		protected override void VisitForStatementVariableDeclaration(BoundForStatement node)
		{
			if ((object)node.DeclaredOrInferredLocalOpt != null)
			{
				GetOrCreateSlot(node.DeclaredOrInferredLocalOpt);
				Assign(node, null, ConsiderLocalInitiallyAssigned(node.DeclaredOrInferredLocalOpt));
			}
		}

		public override BoundNode VisitAnonymousTypeCreationExpression(BoundAnonymousTypeCreationExpression node)
		{
			base.VisitAnonymousTypeCreationExpression(node);
			return null;
		}

		public override BoundNode VisitWithStatement(BoundWithStatement node)
		{
			SetPlaceholderSubstitute(node.ExpressionPlaceholder, node.DraftPlaceholderSubstitute);
			base.VisitWithStatement(node);
			RemovePlaceholderSubstitute(node.ExpressionPlaceholder);
			return null;
		}

		public override BoundNode VisitUsingStatement(BoundUsingStatement node)
		{
			if (node.ResourceList.IsDefaultOrEmpty)
			{
				return base.VisitUsingStatement(node);
			}
			ImmutableArray<BoundLocalDeclarationBase>.Enumerator enumerator = node.ResourceList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundLocalDeclarationBase current = enumerator.Current;
				if (current.Kind == BoundKind.AsNewLocalDeclarations)
				{
					ImmutableArray<BoundLocalDeclaration>.Enumerator enumerator2 = ((BoundAsNewLocalDeclarations)current).LocalDeclarations.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						LocalSymbol localSymbol = enumerator2.Current.LocalSymbol;
						int orCreateSlot = GetOrCreateSlot(localSymbol);
						if (orCreateSlot >= 0)
						{
							SetSlotAssigned(orCreateSlot);
							NoteWrite(localSymbol, null);
						}
					}
				}
				else
				{
					LocalSymbol localSymbol2 = ((BoundLocalDeclaration)current).LocalSymbol;
					int orCreateSlot2 = GetOrCreateSlot(localSymbol2);
					if (orCreateSlot2 >= 0)
					{
						SetSlotAssigned(orCreateSlot2);
						NoteWrite(localSymbol2, null);
					}
				}
			}
			BoundNode result = base.VisitUsingStatement(node);
			ImmutableArray<BoundLocalDeclarationBase>.Enumerator enumerator3 = node.ResourceList.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				BoundLocalDeclarationBase current2 = enumerator3.Current;
				if (current2.Kind == BoundKind.AsNewLocalDeclarations)
				{
					ImmutableArray<BoundLocalDeclaration>.Enumerator enumerator4 = ((BoundAsNewLocalDeclarations)current2).LocalDeclarations.GetEnumerator();
					while (enumerator4.MoveNext())
					{
						BoundLocalDeclaration current3 = enumerator4.Current;
						NoteRead(current3.LocalSymbol);
					}
				}
				else
				{
					NoteRead(((BoundLocalDeclaration)current2).LocalSymbol);
				}
			}
			return result;
		}

		protected sealed override void VisitArgument(BoundExpression arg, ParameterSymbol p)
		{
			if (p.IsByRef)
			{
				if (p.IsOut & !IgnoreOutSemantics)
				{
					VisitLvalue(arg);
				}
				else
				{
					base.VisitRvalue(arg, ReadWriteContext.ByRefArgument, dontLeaveRegion: false);
				}
			}
			else
			{
				VisitRvalue(arg);
			}
		}

		protected override void WriteArgument(BoundExpression arg, bool isOut)
		{
			if (!isOut)
			{
				CheckAssignedFromArgumentWrite(arg, arg.Syntax);
			}
			Assign(arg, null);
			base.WriteArgument(arg, isOut);
		}

		protected void CheckAssignedFromArgumentWrite(BoundExpression expr, SyntaxNode node)
		{
			if (!State.Reachable)
			{
				return;
			}
			switch (expr.Kind)
			{
			case BoundKind.Local:
				CheckAssigned(((BoundLocal)expr).LocalSymbol, node);
				break;
			case BoundKind.Parameter:
				CheckAssigned(((BoundParameter)expr).ParameterSymbol, node);
				break;
			case BoundKind.FieldAccess:
			{
				BoundFieldAccess boundFieldAccess = (BoundFieldAccess)expr;
				_ = boundFieldAccess.FieldSymbol;
				if (AbstractFlowPass<LocalState>.FieldAccessMayRequireTracking(boundFieldAccess))
				{
					this.CheckAssigned(boundFieldAccess, node, ReadWriteContext.ByRefArgument);
				}
				break;
			}
			case BoundKind.EventAccess:
				throw ExceptionUtilities.UnexpectedValue(expr.Kind);
			case BoundKind.MeReference:
			case BoundKind.MyBaseReference:
			case BoundKind.MyClassReference:
				CheckAssigned(MeParameter, node);
				break;
			}
		}

		protected override void VisitLateBoundArgument(BoundExpression arg, bool isByRef)
		{
			if (isByRef)
			{
				base.VisitRvalue(arg, ReadWriteContext.ByRefArgument, dontLeaveRegion: false);
			}
			else
			{
				VisitRvalue(arg);
			}
		}

		public override BoundNode VisitReturnStatement(BoundReturnStatement node)
		{
			if (!node.IsEndOfMethodReturn())
			{
				SetSlotState(1, assigned: true);
			}
			else if (node.ExpressionOpt is BoundLocal boundLocal)
			{
				CheckAssignedFunctionValue(boundLocal.LocalSymbol, node.Syntax);
			}
			base.VisitReturnStatement(node);
			return null;
		}

		public override BoundNode VisitMyBaseReference(BoundMyBaseReference node)
		{
			CheckAssigned(MeParameter, node.Syntax);
			return null;
		}

		public override BoundNode VisitMyClassReference(BoundMyClassReference node)
		{
			CheckAssigned(MeParameter, node.Syntax);
			return null;
		}

		private bool DeclaredVariableIsAlwaysAssignedBeforeInitializer(SyntaxNode syntax, BoundExpression boundInitializer, out BoundValuePlaceholderBase placeholder)
		{
			placeholder = null;
			if (boundInitializer != null && (boundInitializer.Kind == BoundKind.ObjectCreationExpression || boundInitializer.Kind == BoundKind.NewT))
			{
				BoundObjectInitializerExpressionBase initializerOpt = ((BoundObjectCreationExpressionBase)boundInitializer).InitializerOpt;
				if (initializerOpt != null && initializerOpt.Kind == BoundKind.ObjectInitializerExpression)
				{
					BoundObjectInitializerExpression boundObjectInitializerExpression = (BoundObjectInitializerExpression)initializerOpt;
					if (syntax != null && VisualBasicExtensions.Kind(syntax) == SyntaxKind.VariableDeclarator)
					{
						VariableDeclaratorSyntax variableDeclaratorSyntax = (VariableDeclaratorSyntax)syntax;
						if (variableDeclaratorSyntax.AsClause != null && variableDeclaratorSyntax.AsClause.Kind() == SyntaxKind.AsNewClause)
						{
							placeholder = boundObjectInitializerExpression.PlaceholderOpt;
							return !boundObjectInitializerExpression.CreateTemporaryLocalForInitialization;
						}
					}
				}
			}
			return false;
		}

		public override BoundNode VisitAsNewLocalDeclarations(BoundAsNewLocalDeclarations node)
		{
			BoundValuePlaceholderBase placeholder = null;
			bool flag = DeclaredVariableIsAlwaysAssignedBeforeInitializer(node.Syntax, node.Initializer, out placeholder);
			ImmutableArray<BoundLocalDeclaration> localDeclarations = node.LocalDeclarations;
			if (localDeclarations.IsEmpty)
			{
				return null;
			}
			ImmutableArray<BoundLocalDeclaration>.Enumerator enumerator = localDeclarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundLocalDeclaration current = enumerator.Current;
				GetOrCreateSlot(current.LocalSymbol);
			}
			LocalSymbol localSymbol = CreateLocalSymbolForVariables(localDeclarations);
			BoundLocalDeclaration boundLocalDeclaration = localDeclarations[0];
			Assign(boundLocalDeclaration, node.Initializer, ConsiderLocalInitiallyAssigned(boundLocalDeclaration.LocalSymbol) || flag);
			if (flag)
			{
				SetPlaceholderSubstitute(placeholder, new BoundLocal(boundLocalDeclaration.Syntax, localSymbol, localSymbol.Type));
			}
			VisitRvalue(node.Initializer);
			Visit(boundLocalDeclaration);
			int num = localDeclarations.Length - 1;
			for (int i = 1; i <= num; i++)
			{
				boundLocalDeclaration = localDeclarations[i];
				Assign(boundLocalDeclaration, node.Initializer, ConsiderLocalInitiallyAssigned(boundLocalDeclaration.LocalSymbol) || flag);
				Visit(boundLocalDeclaration);
			}
			if (flag)
			{
				RemovePlaceholderSubstitute(placeholder);
			}
			return null;
		}

		protected virtual LocalSymbol CreateLocalSymbolForVariables(ImmutableArray<BoundLocalDeclaration> declarations)
		{
			return declarations[0].LocalSymbol;
		}

		protected override void VisitObjectCreationExpressionInitializer(BoundObjectInitializerExpressionBase node)
		{
			int num;
			if (node != null && node.Kind == BoundKind.ObjectInitializerExpression)
			{
				num = (((BoundObjectInitializerExpression)node).CreateTemporaryLocalForInitialization ? 1 : 0);
				if (num != 0)
				{
					SetPlaceholderSubstitute(((BoundObjectInitializerExpression)node).PlaceholderOpt, null);
				}
			}
			else
			{
				num = 0;
			}
			base.VisitObjectCreationExpressionInitializer(node);
			if (num != 0)
			{
				RemovePlaceholderSubstitute(((BoundObjectInitializerExpression)node).PlaceholderOpt);
			}
		}

		public override BoundNode VisitOnErrorStatement(BoundOnErrorStatement node)
		{
			_seenOnErrorOrResume = true;
			return base.VisitOnErrorStatement(node);
		}

		public override BoundNode VisitResumeStatement(BoundResumeStatement node)
		{
			_seenOnErrorOrResume = true;
			return base.VisitResumeStatement(node);
		}
	}
}

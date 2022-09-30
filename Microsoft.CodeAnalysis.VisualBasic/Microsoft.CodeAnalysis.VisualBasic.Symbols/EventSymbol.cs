using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class EventSymbol : Symbol, IEventDefinition, IEventSymbol
	{
		private IMethodReference IEventDefinitionAdder => AdaptedEventSymbol.AddMethod.GetCciAdapter();

		private IMethodReference IEventDefinitionRemover => AdaptedEventSymbol.RemoveMethod.GetCciAdapter();

		private bool IEventDefinitionIsRuntimeSpecial => AdaptedEventSymbol.HasRuntimeSpecialName;

		private bool IEventDefinitionIsSpecialName => AdaptedEventSymbol.HasSpecialName;

		private IMethodReference IEventDefinitionCaller => AdaptedEventSymbol.RaiseMethod?.GetCciAdapter();

		private ITypeDefinition IEventDefinitionContainingTypeDefinition => AdaptedEventSymbol.ContainingType.GetCciAdapter();

		private TypeMemberVisibility IEventDefinitionVisibility => PEModuleBuilder.MemberVisibility(AdaptedEventSymbol);

		private string IEventDefinitionName => AdaptedEventSymbol.MetadataName;

		internal EventSymbol AdaptedEventSymbol => this;

		internal virtual bool HasRuntimeSpecialName => false;

		public new virtual EventSymbol OriginalDefinition => this;

		protected sealed override Symbol OriginalSymbolDefinition => OriginalDefinition;

		public abstract bool IsWindowsRuntimeEvent { get; }

		public abstract TypeSymbol Type { get; }

		public abstract MethodSymbol AddMethod { get; }

		public abstract MethodSymbol RemoveMethod { get; }

		public abstract MethodSymbol RaiseMethod { get; }

		internal virtual bool IsDirectlyExcludedFromCodeCoverage => false;

		internal abstract bool HasSpecialName { get; }

		internal bool HasAssociatedField => (object)AssociatedField != null;

		internal abstract FieldSymbol AssociatedField { get; }

		public EventSymbol OverriddenEvent
		{
			get
			{
				if (IsOverrides)
				{
					if (base.IsDefinition)
					{
						return OverriddenOrHiddenMembers.OverriddenMember;
					}
					return OverriddenMembersResult<EventSymbol>.GetOverriddenMember(this, OriginalDefinition.OverriddenEvent);
				}
				return null;
			}
		}

		internal virtual OverriddenMembersResult<EventSymbol> OverriddenOrHiddenMembers => OverrideHidingHelper<EventSymbol>.MakeOverriddenMembers(this);

		internal virtual bool IsExplicitInterfaceImplementation => ExplicitInterfaceImplementations.Any();

		public abstract ImmutableArray<EventSymbol> ExplicitInterfaceImplementations { get; }

		public sealed override SymbolKind Kind => SymbolKind.Event;

		internal virtual ImmutableArray<ParameterSymbol> DelegateParameters => DelegateInvokeMethod()?.Parameters ?? ImmutableArray<ParameterSymbol>.Empty;

		internal TypeSymbol DelegateReturnType
		{
			get
			{
				MethodSymbol methodSymbol = DelegateInvokeMethod();
				if ((object)methodSymbol != null)
				{
					return methodSymbol.ReturnType;
				}
				return ContainingAssembly.GetSpecialType(SpecialType.System_Void);
			}
		}

		protected override int HighestPriorityUseSiteError => 30649;

		public sealed override bool HasUnsupportedMetadata
		{
			get
			{
				DiagnosticInfo diagnosticInfo = GetUseSiteInfo().DiagnosticInfo;
				if (diagnosticInfo != null)
				{
					if (diagnosticInfo.Code != 30649)
					{
						return diagnosticInfo.Code == 37223;
					}
					return true;
				}
				return false;
			}
		}

		internal override EmbeddedSymbolKind EmbeddedSymbolKind => ContainingSymbol.EmbeddedSymbolKind;

		public virtual bool IsTupleEvent => false;

		public virtual EventSymbol TupleUnderlyingEvent => null;

		private ITypeSymbol IEventSymbol_Type => Type;

		private NullableAnnotation IEventSymbol_NullableAnnotation => NullableAnnotation.None;

		private IMethodSymbol IEventSymbol_AddMethod => AddMethod;

		private IMethodSymbol IEventSymbol_RemoveMethod => RemoveMethod;

		private IMethodSymbol IEventSymbol_RaiseMethod => RaiseMethod;

		private IEventSymbol IEventSymbol_OriginalDefinition => OriginalDefinition;

		private IEventSymbol IEventSymbol_OverriddenEvent => OverriddenEvent;

		private ImmutableArray<IEventSymbol> IEventSymbol_ExplicitInterfaceImplementations => StaticCast<IEventSymbol>.From(ExplicitInterfaceImplementations);

		[IteratorStateMachine(typeof(VB_0024StateMachine_0_IEventDefinitionAccessors))]
		private IEnumerable<IMethodReference> IEventDefinitionAccessors(EmitContext context)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_0_IEventDefinitionAccessors(-2)
			{
				_0024VB_0024Me = this,
				_0024P_context = context
			};
		}

		IEnumerable<IMethodReference> IEventDefinition.GetAccessors(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IEventDefinitionAccessors
			return this.IEventDefinitionAccessors(context);
		}

		private ITypeReference IEventDefinitionGetType(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(AdaptedEventSymbol.Type, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		ITypeReference IEventDefinition.GetType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IEventDefinitionGetType
			return this.IEventDefinitionGetType(context);
		}

		private ITypeReference ITypeMemberReferenceGetContainingType(EmitContext context)
		{
			return AdaptedEventSymbol.ContainingType.GetCciAdapter();
		}

		ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeMemberReferenceGetContainingType
			return this.ITypeMemberReferenceGetContainingType(context);
		}

		internal override void IReferenceDispatch(MetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		internal override IDefinition IReferenceAsDefinition(EmitContext context)
		{
			return this;
		}

		internal new EventSymbol GetCciAdapter()
		{
			return this;
		}

		internal EventSymbol()
		{
		}

		public ImmutableArray<VisualBasicAttributeData> GetFieldAttributes()
		{
			return AssociatedField?.GetAttributes() ?? ImmutableArray<VisualBasicAttributeData>.Empty;
		}

		private MethodSymbol DelegateInvokeMethod()
		{
			if (Type is NamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeKind == TypeKind.Delegate)
			{
				return namedTypeSymbol.DelegateInvokeMethod;
			}
			return null;
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
		{
			return visitor.VisitEvent(this, argument);
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			if (base.IsDefinition)
			{
				return new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency);
			}
			return OriginalDefinition.GetUseSiteInfo();
		}

		internal UseSiteInfo<AssemblySymbol> CalculateUseSiteInfo()
		{
			UseSiteInfo<AssemblySymbol> result = MergeUseSiteInfo(new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency), DeriveUseSiteInfoFromType(Type));
			DiagnosticInfo diagnosticInfo = result.DiagnosticInfo;
			if (diagnosticInfo != null)
			{
				switch (diagnosticInfo.Code)
				{
				case 30652:
					result = new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnreferencedAssemblyEvent3, diagnosticInfo.Arguments[0], this));
					break;
				case 30653:
					result = new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnreferencedModuleEvent3, diagnosticInfo.Arguments[0], this));
					break;
				case 30649:
					if (diagnosticInfo.Arguments[0].Equals(string.Empty))
					{
						result = new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, CustomSymbolDisplayFormatter.ShortErrorName(this)));
					}
					break;
				}
			}
			else if (ContainingModule.HasUnifiedReferences)
			{
				TypeSymbol type = Type;
				HashSet<TypeSymbol> checkedTypes = null;
				diagnosticInfo = type.GetUnificationUseSiteDiagnosticRecursive(this, ref checkedTypes);
				if (diagnosticInfo != null)
				{
					result = new UseSiteInfo<AssemblySymbol>(diagnosticInfo);
				}
			}
			return result;
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitEvent(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitEvent(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitEvent(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitEvent(this);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is EventSymbol eventSymbol))
			{
				return false;
			}
			if ((object)this == eventSymbol)
			{
				return true;
			}
			return TypeSymbol.Equals(ContainingType, eventSymbol.ContainingType, TypeCompareKind.ConsiderEverything) && (object)OriginalDefinition == eventSymbol.OriginalDefinition;
		}

		public override int GetHashCode()
		{
			int currentKey = 1;
			currentKey = Hash.Combine(ContainingType, currentKey);
			return Hash.Combine(Name, currentKey);
		}
	}
}

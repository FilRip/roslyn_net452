using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class LocalSymbol : Symbol, ILocalSymbol, ILocalSymbolInternal
	{
		private class SourceLocalSymbol : LocalSymbol
		{
			private readonly LocalDeclarationKind _declarationKind;

			protected readonly SyntaxToken _identifierToken;

			protected readonly Binder _binder;

			internal override LocalDeclarationKind DeclarationKind => _declarationKind;

			public override bool IsFunctionValue => _declarationKind == LocalDeclarationKind.FunctionValue;

			internal override SynthesizedLocalKind SynthesizedKind => SynthesizedLocalKind.UserDefined;

			public override string Name => VisualBasicExtensions.GetIdentifierText(_identifierToken);

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
			{
				get
				{
					if (DeclarationKind == LocalDeclarationKind.FunctionValue)
					{
						return ImmutableArray<SyntaxReference>.Empty;
					}
					SyntaxToken identifierToken = _identifierToken;
					return ImmutableArray.Create(identifierToken.Parent!.GetReference());
				}
			}

			internal sealed override Location IdentifierLocation
			{
				get
				{
					SyntaxToken identifierToken = _identifierToken;
					return identifierToken.GetLocation();
				}
			}

			internal sealed override SyntaxToken IdentifierToken => _identifierToken;

			public SourceLocalSymbol(Symbol containingSymbol, Binder binder, SyntaxToken identifierToken, LocalDeclarationKind declarationKind, TypeSymbol type)
				: base(containingSymbol, type)
			{
				_identifierToken = identifierToken;
				_declarationKind = declarationKind;
				_binder = binder;
			}

			internal override SyntaxNode GetDeclaratorSyntax()
			{
				SyntaxNode parent;
				switch (DeclarationKind)
				{
				case LocalDeclarationKind.Variable:
				case LocalDeclarationKind.Constant:
				case LocalDeclarationKind.Static:
				case LocalDeclarationKind.Using:
				{
					SyntaxToken identifierToken = _identifierToken;
					parent = identifierToken.Parent;
					break;
				}
				case LocalDeclarationKind.ImplicitVariable:
				{
					SyntaxToken identifierToken = _identifierToken;
					parent = identifierToken.Parent;
					break;
				}
				case LocalDeclarationKind.FunctionValue:
				{
					SyntaxToken identifierToken = _identifierToken;
					parent = identifierToken.Parent;
					if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent, SyntaxKind.PropertyStatement))
					{
						return ((PropertyBlockSyntax)parent.Parent).Accessors.Where((AccessorBlockSyntax a) => Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(a, SyntaxKind.GetAccessorBlock)).Single().BlockStatement;
					}
					if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent, SyntaxKind.EventStatement))
					{
						return ((EventBlockSyntax)parent.Parent).Accessors.Where((AccessorBlockSyntax a) => Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(a, SyntaxKind.AddHandlerAccessorBlock)).Single().BlockStatement;
					}
					break;
				}
				case LocalDeclarationKind.Catch:
				{
					SyntaxToken identifierToken = _identifierToken;
					parent = identifierToken.Parent!.Parent;
					break;
				}
				case LocalDeclarationKind.For:
				{
					SyntaxToken identifierToken = _identifierToken;
					parent = identifierToken.Parent;
					if (!Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent, SyntaxKind.ModifiedIdentifier))
					{
						parent = parent.Parent;
					}
					break;
				}
				case LocalDeclarationKind.ForEach:
				{
					SyntaxToken identifierToken = _identifierToken;
					parent = identifierToken.Parent;
					if (!Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent, SyntaxKind.ModifiedIdentifier))
					{
						parent = parent.Parent;
					}
					break;
				}
				default:
					throw ExceptionUtilities.UnexpectedValue(DeclarationKind);
				}
				return parent;
			}

			internal override TypeSymbol ComputeType(Binder containingBinder = null)
			{
				containingBinder = containingBinder ?? _binder;
				return ComputeTypeInternal(containingBinder ?? _binder);
			}

			internal virtual TypeSymbol ComputeTypeInternal(Binder containingBinder)
			{
				return _lazyType;
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				int result;
				if (obj is SourceLocalSymbol sourceLocalSymbol)
				{
					SyntaxToken identifierToken = sourceLocalSymbol._identifierToken;
					if (identifierToken.Equals(_identifierToken) && object.Equals(sourceLocalSymbol._container, _container))
					{
						result = (string.Equals(sourceLocalSymbol.Name, Name) ? 1 : 0);
						goto IL_004f;
					}
				}
				result = 0;
				goto IL_004f;
				IL_004f:
				return (byte)result != 0;
			}

			public override int GetHashCode()
			{
				SyntaxToken identifierToken = _identifierToken;
				return Hash.Combine(identifierToken.GetHashCode(), _container.GetHashCode());
			}
		}

		private sealed class SourceLocalSymbolWithNonstandardName : SourceLocalSymbol
		{
			private readonly string _name;

			public override string Name => _name;

			public SourceLocalSymbolWithNonstandardName(Symbol container, Binder binder, SyntaxToken declaringIdentifier, LocalDeclarationKind declarationKind, TypeSymbol type, string name)
				: base(container, binder, declaringIdentifier, declarationKind, type)
			{
				_name = name;
			}
		}

		private sealed class InferredForEachLocalSymbol : SourceLocalSymbol
		{
			private readonly ExpressionSyntax _collectionExpressionSyntax;

			internal override bool HasInferredType => true;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<ForEachStatementSyntax>(Locations);

			public InferredForEachLocalSymbol(Symbol container, Binder binder, SyntaxToken declaringIdentifier, ExpressionSyntax collectionExpressionSyntax)
				: base(container, binder, declaringIdentifier, LocalDeclarationKind.ForEach, null)
			{
				_collectionExpressionSyntax = collectionExpressionSyntax;
			}

			internal override TypeSymbol ComputeTypeInternal(Binder localBinder)
			{
				ExpressionSyntax collectionExpressionSyntax = _collectionExpressionSyntax;
				BoundExpression collectionExpression = null;
				TypeSymbol currentType = null;
				TypeSymbol elementType = null;
				bool isEnumerable = false;
				BoundExpression boundGetEnumeratorCall = null;
				BoundLValuePlaceholder boundEnumeratorPlaceholder = null;
				BoundExpression boundMoveNextCall = null;
				BoundExpression boundCurrentAccess = null;
				BoundRValuePlaceholder collectionPlaceholder = null;
				bool needToDispose = false;
				bool isOrInheritsFromOrImplementsIDisposable = false;
				return localBinder.InferForEachVariableType(this, collectionExpressionSyntax, out collectionExpression, out currentType, out elementType, out isEnumerable, out boundGetEnumeratorCall, out boundEnumeratorPlaceholder, out boundMoveNextCall, out boundCurrentAccess, out collectionPlaceholder, out needToDispose, out isOrInheritsFromOrImplementsIDisposable, BindingDiagnosticBag.Discarded);
			}
		}

		private sealed class InferredForFromToLocalSymbol : SourceLocalSymbol
		{
			private readonly ExpressionSyntax _fromValue;

			private readonly ExpressionSyntax _toValue;

			private readonly ForStepClauseSyntax _stepClauseOpt;

			internal override bool HasInferredType => true;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<ForStatementSyntax>(Locations);

			public InferredForFromToLocalSymbol(Symbol container, Binder binder, SyntaxToken declaringIdentifier, ExpressionSyntax fromValue, ExpressionSyntax toValue, ForStepClauseSyntax stepClauseOpt)
				: base(container, binder, declaringIdentifier, LocalDeclarationKind.For, null)
			{
				_fromValue = fromValue;
				_toValue = toValue;
				_stepClauseOpt = stepClauseOpt;
			}

			internal override TypeSymbol ComputeType(Binder containingBinder = null)
			{
				BoundExpression fromValueExpression = null;
				BoundExpression toValueExpression = null;
				BoundExpression stepValueExpression = null;
				return (containingBinder ?? _binder).InferForFromToVariableType(this, _fromValue, _toValue, _stepClauseOpt, out fromValueExpression, out toValueExpression, out stepValueExpression, BindingDiagnosticBag.Discarded);
			}
		}

		private sealed class VariableLocalSymbol : SourceLocalSymbol
		{
			private sealed class EvaluatedConstantInfo : EvaluatedConstant
			{
				public readonly BoundExpression Expression;

				public readonly BindingDiagnosticBag Diagnostics;

				public EvaluatedConstantInfo(ConstantValue value, TypeSymbol type, BoundExpression expression, BindingDiagnosticBag diagnostics)
					: base(value, type)
				{
					Expression = expression;
					Diagnostics = diagnostics;
				}
			}

			private readonly ModifiedIdentifierSyntax _modifiedIdentifierOpt;

			private readonly AsClauseSyntax _asClauseOpt;

			private readonly EqualsValueSyntax _initializerOpt;

			private EvaluatedConstantInfo _evaluatedConstant;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
			{
				get
				{
					LocalDeclarationKind declarationKind = DeclarationKind;
					if (declarationKind == LocalDeclarationKind.None || declarationKind == LocalDeclarationKind.FunctionValue)
					{
						return ImmutableArray<SyntaxReference>.Empty;
					}
					if (_modifiedIdentifierOpt != null)
					{
						return ImmutableArray.Create(_modifiedIdentifierOpt.GetReference());
					}
					return ImmutableArray<SyntaxReference>.Empty;
				}
			}

			public VariableLocalSymbol(Symbol container, Binder binder, SyntaxToken declaringIdentifier, ModifiedIdentifierSyntax modifiedIdentifierOpt, AsClauseSyntax asClauseOpt, EqualsValueSyntax initializerOpt, LocalDeclarationKind declarationKind)
				: base(container, binder, declaringIdentifier, declarationKind, null)
			{
				_modifiedIdentifierOpt = modifiedIdentifierOpt;
				_asClauseOpt = asClauseOpt;
				_initializerOpt = initializerOpt;
			}

			internal override TypeSymbol ComputeTypeInternal(Binder localBinder)
			{
				TypeSymbol asClauseType = null;
				BoundExpression valueExpression = null;
				return localBinder.ComputeVariableType(this, _modifiedIdentifierOpt, _asClauseOpt, _initializerOpt, out valueExpression, out asClauseType, BindingDiagnosticBag.Discarded);
			}

			internal override BoundExpression GetConstantExpression(Binder localBinder)
			{
				if (base.IsConst)
				{
					if (_evaluatedConstant == null)
					{
						BindingDiagnosticBag diagnostics = new BindingDiagnosticBag();
						ConstantValue constValue = null;
						BoundExpression boundExpression = localBinder.BindLocalConstantInitializer(this, _lazyType, _modifiedIdentifierOpt, _initializerOpt, diagnostics, out constValue);
						SetConstantExpression(boundExpression.Type, constValue, boundExpression, diagnostics);
						return boundExpression;
					}
					return _evaluatedConstant.Expression;
				}
				throw ExceptionUtilities.Unreachable;
			}

			internal override ConstantValue GetConstantValue(Binder containingBinder)
			{
				if (base.IsConst && _evaluatedConstant == null)
				{
					Binder localBinder = containingBinder ?? _binder;
					GetConstantExpression(localBinder);
				}
				if (_evaluatedConstant == null)
				{
					return null;
				}
				return _evaluatedConstant.Value;
			}

			internal override BindingDiagnosticBag GetConstantValueDiagnostics(Binder containingBinder)
			{
				GetConstantValue(containingBinder);
				if (_evaluatedConstant == null)
				{
					return null;
				}
				return _evaluatedConstant.Diagnostics;
			}

			private void SetConstantExpression(TypeSymbol type, ConstantValue constantValue, BoundExpression expression, BindingDiagnosticBag diagnostics)
			{
				if (_evaluatedConstant == null)
				{
					Interlocked.CompareExchange(ref _evaluatedConstant, new EvaluatedConstantInfo(constantValue, type, expression, diagnostics), null);
				}
			}
		}

		private sealed class TypeSubstitutedLocalSymbol : LocalSymbol
		{
			private readonly LocalSymbol _originalVariable;

			internal override LocalDeclarationKind DeclarationKind => _originalVariable.DeclarationKind;

			internal override SynthesizedLocalKind SynthesizedKind => _originalVariable.SynthesizedKind;

			public override bool IsFunctionValue => _originalVariable.IsFunctionValue;

			public override string Name => _originalVariable.Name;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _originalVariable.DeclaringSyntaxReferences;

			public override ImmutableArray<Location> Locations => _originalVariable.Locations;

			internal override SyntaxToken IdentifierToken => _originalVariable.IdentifierToken;

			internal override Location IdentifierLocation => _originalVariable.IdentifierLocation;

			internal override bool IsByRef => _originalVariable.IsByRef;

			public TypeSubstitutedLocalSymbol(LocalSymbol originalVariable, TypeSymbol type)
				: base(originalVariable._container, type)
			{
				_originalVariable = originalVariable;
			}

			internal override ConstantValue GetConstantValue(Binder binder)
			{
				return _originalVariable.GetConstantValue(binder);
			}

			internal override BindingDiagnosticBag GetConstantValueDiagnostics(Binder binder)
			{
				return _originalVariable.GetConstantValueDiagnostics(binder);
			}

			internal override SyntaxNode GetDeclaratorSyntax()
			{
				return _originalVariable.GetDeclaratorSyntax();
			}
		}

		internal static readonly ErrorTypeSymbol UseBeforeDeclarationResultType = new ErrorTypeSymbol();

		private readonly Symbol _container;

		private TypeSymbol _lazyType;

		internal virtual bool IsImportedFromMetadata => false;

		internal abstract LocalDeclarationKind DeclarationKind { get; }

		internal abstract SynthesizedLocalKind SynthesizedKind { get; }

		public virtual TypeSymbol Type
		{
			get
			{
				if ((object)_lazyType == null)
				{
					Interlocked.CompareExchange(ref _lazyType, ComputeType(), null);
				}
				return _lazyType;
			}
		}

		internal bool ConstHasType => (object)_lazyType != null;

		internal virtual bool IsReadOnly
		{
			get
			{
				if (!IsUsing)
				{
					return IsConst;
				}
				return true;
			}
		}

		public abstract override string Name { get; }

		internal abstract SyntaxToken IdentifierToken { get; }

		public sealed override SymbolKind Kind => SymbolKind.Local;

		public override ImmutableArray<Location> Locations => ImmutableArray.Create(IdentifierLocation);

		internal abstract Location IdentifierLocation { get; }

		public override Symbol ContainingSymbol => _container;

		public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

		public override bool IsShared => false;

		public override bool IsOverridable => false;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverrides => false;

		public bool IsUsing => DeclarationKind == LocalDeclarationKind.Using;

		public bool IsCatch => DeclarationKind == LocalDeclarationKind.Catch;

		public bool IsConst => DeclarationKind == LocalDeclarationKind.Constant;

		private bool ILocalSymbol_IsFixed => false;

		internal virtual bool CanScheduleToStack
		{
			get
			{
				if (!IsConst)
				{
					return !IsCatch;
				}
				return false;
			}
		}

		public bool IsStatic => DeclarationKind == LocalDeclarationKind.Static;

		public bool IsFor => DeclarationKind == LocalDeclarationKind.For;

		public bool IsForEach => DeclarationKind == LocalDeclarationKind.ForEach;

		public bool IsRef => false;

		public RefKind RefKind => RefKind.None;

		public abstract bool IsFunctionValue { get; }

		internal bool IsCompilerGenerated => DeclarationKind == LocalDeclarationKind.None;

		public override bool IsImplicitlyDeclared => DeclarationKind == LocalDeclarationKind.ImplicitVariable;

		internal virtual bool IsByRef => false;

		internal virtual bool IsPinned => false;

		public bool HasConstantValue
		{
			get
			{
				if (!IsConst)
				{
					return false;
				}
				return (object)GetConstantValue(null) != null;
			}
		}

		public object ConstantValue
		{
			get
			{
				if (!IsConst)
				{
					return null;
				}
				return GetConstantValue(null)?.Value;
			}
		}

		internal virtual bool HasInferredType => false;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		private ITypeSymbol ILocalSymbol_Type => Type;

		private NullableAnnotation ILocalSymbol_NullableAnnotation => NullableAnnotation.None;

		private bool ILocalSymbol_IsConst => IsConst;

		protected override bool ISymbol_IsStatic => IsStatic;

		private bool ILocalSymbolInternal_IsImportedFromMetadata => IsImportedFromMetadata;

		private SynthesizedLocalKind ILocalSymbolInternal_SynthesizedKind => SynthesizedKind;

		internal static LocalSymbol Create(Symbol container, Binder binder, SyntaxToken declaringIdentifier, ModifiedIdentifierSyntax modifiedIdentifierOpt, AsClauseSyntax asClauseOpt, EqualsValueSyntax initializerOpt, LocalDeclarationKind declarationKind)
		{
			return new VariableLocalSymbol(container, binder, declaringIdentifier, modifiedIdentifierOpt, asClauseOpt, initializerOpt, declarationKind);
		}

		internal static LocalSymbol Create(Symbol container, Binder binder, SyntaxToken declaringIdentifier, LocalDeclarationKind declarationKind, TypeSymbol type)
		{
			return new SourceLocalSymbol(container, binder, declaringIdentifier, declarationKind, type);
		}

		internal static LocalSymbol Create(Symbol container, Binder binder, SyntaxToken declaringIdentifier, LocalDeclarationKind declarationKind, TypeSymbol type, string name)
		{
			return new SourceLocalSymbolWithNonstandardName(container, binder, declaringIdentifier, declarationKind, type, name);
		}

		internal static LocalSymbol Create(LocalSymbol originalVariable, TypeSymbol type)
		{
			return new TypeSubstitutedLocalSymbol(originalVariable, type);
		}

		internal static LocalSymbol CreateInferredForFromTo(Symbol container, Binder binder, SyntaxToken declaringIdentifier, ExpressionSyntax fromValue, ExpressionSyntax toValue, ForStepClauseSyntax stepClauseOpt)
		{
			return new InferredForFromToLocalSymbol(container, binder, declaringIdentifier, fromValue, toValue, stepClauseOpt);
		}

		internal static LocalSymbol CreateInferredForEach(Symbol container, Binder binder, SyntaxToken declaringIdentifier, ExpressionSyntax expression)
		{
			return new InferredForEachLocalSymbol(container, binder, declaringIdentifier, expression);
		}

		internal LocalSymbol(Symbol container, TypeSymbol type)
		{
			_container = container;
			_lazyType = type;
		}

		public void SetType(TypeSymbol type)
		{
			if ((object)_lazyType == null)
			{
				Interlocked.CompareExchange(ref _lazyType, type, null);
			}
		}

		internal virtual TypeSymbol ComputeType(Binder containingBinder = null)
		{
			return _lazyType;
		}

		internal abstract SyntaxNode GetDeclaratorSyntax();

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitLocal(this, arg);
		}

		internal virtual BindingDiagnosticBag GetConstantValueDiagnostics(Binder binder)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal virtual BoundExpression GetConstantExpression(Binder binder)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal virtual ConstantValue GetConstantValue(Binder binder)
		{
			return null;
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitLocal(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitLocal(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitLocal(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitLocal(this);
		}

		private SyntaxNode ILocalSymbolInternal_GetDeclaratorSyntax()
		{
			return GetDeclaratorSyntax();
		}

		SyntaxNode ILocalSymbolInternal.GetDeclaratorSyntax()
		{
			//ILSpy generated this explicit interface implementation from .override directive in ILocalSymbolInternal_GetDeclaratorSyntax
			return this.ILocalSymbolInternal_GetDeclaratorSyntax();
		}
	}
}

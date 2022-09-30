using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class ErrorTypeSymbol : NamedTypeSymbol, IErrorTypeSymbol
	{
		internal static readonly ErrorTypeSymbol UnknownResultType = new ErrorTypeSymbol();

		internal virtual DiagnosticInfo ErrorInfo => null;

		public override bool IsReferenceType => true;

		public override bool IsValueType => false;

		public override IEnumerable<string> MemberNames => SpecializedCollections.EmptyEnumerable<string>();

		public sealed override SymbolKind Kind => SymbolKind.ErrorType;

		public sealed override TypeKind TypeKind => TypeKind.Error;

		internal sealed override bool IsInterface => false;

		public override Symbol ContainingSymbol => null;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override int Arity => 0;

		public override string Name => string.Empty;

		internal override bool MangleName => false;

		internal sealed override bool HasSpecialName => false;

		public sealed override bool IsSerializable => false;

		internal override TypeLayout Layout => default(TypeLayout);

		internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

		internal override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics => ImmutableArray<TypeSymbol>.Empty;

		internal override bool HasTypeArgumentsCustomModifiers => false;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		public override NamedTypeSymbol ConstructedFrom => this;

		public sealed override Accessibility DeclaredAccessibility => Accessibility.Public;

		public sealed override bool IsMustInherit => false;

		public sealed override bool IsNotInheritable => false;

		public sealed override bool MightContainExtensionMethods => false;

		internal override bool HasCodeAnalysisEmbeddedAttribute => false;

		internal override bool HasVisualBasicEmbeddedAttribute => false;

		internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics => false;

		internal override bool IsWindowsRuntimeImport => false;

		internal override bool ShouldAddWinRTMembers => false;

		internal override bool IsComImport => false;

		internal override TypeSymbol CoClassType => null;

		internal override bool HasDeclarativeSecurity => false;

		internal override TypeSubstitution TypeSubstitution => null;

		internal override bool CanConstruct => false;

		internal override string DefaultPropertyName => null;

		internal NamedTypeSymbol NonErrorGuessType
		{
			get
			{
				ImmutableArray<Symbol> candidateSymbols = CandidateSymbols;
				if (candidateSymbols.Length == 1)
				{
					return candidateSymbols[0] as NamedTypeSymbol;
				}
				return null;
			}
		}

		internal virtual LookupResultKind ResultKind => LookupResultKind.Empty;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		public virtual ImmutableArray<Symbol> CandidateSymbols => ImmutableArray<Symbol>.Empty;

		public CandidateReason CandidateReason
		{
			get
			{
				if (CandidateSymbols.IsEmpty)
				{
					return CandidateReason.None;
				}
				return LookupResultKindExtensions.ToCandidateReason(ResultKind);
			}
		}

		public ImmutableArray<ISymbol> IErrorTypeSymbol_CandidateSymbols => StaticCast<ISymbol>.From(CandidateSymbols);

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			if (base.IsDefinition)
			{
				return new UseSiteInfo<AssemblySymbol>(ErrorInfo);
			}
			return base.GetUseSiteInfo();
		}

		internal override DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
		{
			return null;
		}

		internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return null;
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			return null;
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			return ImmutableArray<Symbol>.Empty;
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			return ImmutableArray<Symbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
		{
			return SpecializedCollections.EmptyEnumerable<FieldSymbol>();
		}

		public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
		{
			return GetEmptyTypeArgumentCustomModifiers(ordinal);
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitErrorType(this, arg);
		}

		internal ErrorTypeSymbol()
		{
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return ImmutableArray<string>.Empty;
		}

		internal override AttributeUsageInfo GetAttributeUsageInfo()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
		{
			return new TypeWithModifiers(this);
		}

		public override NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
		{
			throw new InvalidOperationException();
		}

		public override bool Equals(TypeSymbol obj, TypeCompareKind comparison)
		{
			return (object)this == obj;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal sealed override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
		{
			return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
		}
	}
}

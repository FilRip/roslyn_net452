using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedEventDelegateSymbol : InstanceTypeSymbol
	{
		private readonly string _eventName;

		private readonly string _name;

		private readonly NamedTypeSymbol _containingType;

		private readonly SyntaxReference _syntaxRef;

		private ImmutableArray<Symbol> _lazyMembers;

		private EventSymbol _lazyEventSymbol;

		private int _reportedAllDeclarationErrors;

		private EventStatementSyntax EventSyntax => (EventStatementSyntax)_syntaxRef.GetSyntax();

		public override Symbol AssociatedSymbol
		{
			get
			{
				if ((object)_lazyEventSymbol == null)
				{
					ImmutableArray<Symbol>.Enumerator enumerator = _containingType.GetMembers(_eventName).GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (enumerator.Current is SourceEventSymbol sourceEventSymbol)
						{
							SyntaxNode syntax = sourceEventSymbol.SyntaxReference.GetSyntax();
							if (syntax != null && syntax == EventSyntax)
							{
								_lazyEventSymbol = sourceEventSymbol;
							}
						}
					}
				}
				return _lazyEventSymbol;
			}
		}

		internal override Symbol ImplicitlyDefinedBy
		{
			get
			{
				if (membersInProgress == null)
				{
					return AssociatedSymbol;
				}
				ArrayBuilder<Symbol> arrayBuilder = membersInProgress[_eventName];
				SourceEventSymbol result = null;
				if (arrayBuilder != null)
				{
					ArrayBuilder<Symbol>.Enumerator enumerator = arrayBuilder.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (enumerator.Current is SourceEventSymbol sourceEventSymbol)
						{
							SyntaxNode syntax = sourceEventSymbol.SyntaxReference.GetSyntax();
							if (syntax != null && syntax == EventSyntax)
							{
								result = sourceEventSymbol;
							}
						}
					}
				}
				return result;
			}
		}

		public override int Arity => 0;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override Accessibility DeclaredAccessibility => AssociatedSymbol.DeclaredAccessibility;

		internal override string DefaultPropertyName => null;

		public override bool IsMustInherit => false;

		public override bool IsNotInheritable => true;

		internal override bool ShadowsExplicitly => AssociatedSymbol.ShadowsExplicitly;

		public override ImmutableArray<Location> Locations => ImmutableArray.Create(_syntaxRef.GetLocation());

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		internal override bool MangleName => false;

		public override IEnumerable<string> MemberNames => new HashSet<string>(from member in GetMembers()
			select member.Name);

		public override bool MightContainExtensionMethods => false;

		internal override bool HasCodeAnalysisEmbeddedAttribute => false;

		internal override bool HasVisualBasicEmbeddedAttribute => false;

		internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics => false;

		internal override bool IsWindowsRuntimeImport => false;

		internal override bool ShouldAddWinRTMembers => false;

		internal override bool IsComImport => false;

		internal override TypeSymbol CoClassType => null;

		internal override bool HasDeclarativeSecurity => false;

		public override string Name => _name;

		internal override bool HasSpecialName => false;

		public override bool IsSerializable => false;

		internal override TypeLayout Layout => default(TypeLayout);

		internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

		public override TypeKind TypeKind => TypeKind.Delegate;

		internal override bool IsInterface => false;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		public override bool IsImplicitlyDeclared => true;

		internal override EmbeddedSymbolKind EmbeddedSymbolKind => ContainingType.EmbeddedSymbolKind;

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal SynthesizedEventDelegateSymbol(SyntaxReference syntaxRef, NamedTypeSymbol containingSymbol)
		{
			_reportedAllDeclarationErrors = 0;
			_containingType = containingSymbol;
			_syntaxRef = syntaxRef;
			string text = (_eventName = EventSyntax.Identifier.ValueText);
			_name = _eventName + "EventHandler";
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			if (!_lazyMembers.IsDefault)
			{
				return _lazyMembers;
			}
			SourceModuleSymbol obj = (SourceModuleSymbol)ContainingModule;
			Binder binder = BinderBuilder.CreateBinderForType(obj, _syntaxRef.SyntaxTree, ContainingType);
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
			EventStatementSyntax eventSyntax = EventSyntax;
			_ = eventSyntax.ParameterList;
			MethodSymbol constructor = null;
			MethodSymbol beginInvoke = null;
			MethodSymbol endInvoke = null;
			MethodSymbol invoke = null;
			SourceDelegateMethodSymbol.MakeDelegateMembers(this, EventSyntax, eventSyntax.ParameterList, binder, out constructor, out beginInvoke, out endInvoke, out invoke, instance);
			obj.AtomicStoreArrayAndDiagnostics<Symbol>(value: ((object)beginInvoke != null && (object)endInvoke != null) ? ImmutableArray.Create((Symbol)constructor, (Symbol)beginInvoke, (Symbol)endInvoke, (Symbol)invoke) : ImmutableArray.Create((Symbol)constructor, (Symbol)invoke), variable: ref _lazyMembers, diagBag: instance);
			instance.Free();
			return _lazyMembers;
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			return (from m in GetMembers()
				where CaseInsensitiveComparison.Equals(m.Name, name)
				select m).AsImmutable();
		}

		internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
		{
			return SpecializedCollections.EmptyEnumerable<FieldSymbol>();
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

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return new LexicalSortKey(_syntaxRef, DeclaringCompilation);
		}

		internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			return MakeDeclaredBase(default(BasesBeingResolved), diagnostics);
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return _containingType.ContainingAssembly.GetSpecialType(SpecialType.System_MulticastDelegate);
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
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

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			if (_reportedAllDeclarationErrors != 0)
			{
				return;
			}
			GetMembers();
			cancellationToken.ThrowIfCancellationRequested();
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
			DelegateInvokeMethod.GenerateDeclarationErrors(cancellationToken);
			NamedTypeSymbol containingType = _containingType;
			NamedTypeSymbol namedTypeSymbol = null;
			while (TypeSymbolExtensions.IsInterfaceType(containingType))
			{
				if (NamedTypeSymbolExtensions.HaveVariance(containingType.TypeParameters))
				{
					namedTypeSymbol = containingType;
				}
				containingType = containingType.ContainingType;
				if ((object)containingType == null)
				{
					break;
				}
			}
			if ((object)namedTypeSymbol != null)
			{
				instance.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_VariancePreventsSynthesizedEvents2, CustomSymbolDisplayFormatter.QualifiedName(namedTypeSymbol), AssociatedSymbol.Name), Locations[0]));
			}
			((SourceModuleSymbol)ContainingModule).AtomicStoreIntegerAndDiagnostics(ref _reportedAllDeclarationErrors, 1, 0, instance);
			instance.Free();
		}

		internal override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
		{
			return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SourceMemberContainerTypeSymbol : InstanceTypeSymbol
	{
		[Flags]
		internal enum SourceTypeFlags : ushort
		{
			Private = 1,
			Protected = 3,
			Friend = 4,
			ProtectedFriend = 5,
			PrivateProtected = 2,
			Public = 6,
			AccessibilityMask = 7,
			Class = 0x20,
			Structure = 0xA0,
			Interface = 0x70,
			Enum = 0x50,
			Delegate = 0x30,
			Module = 0x80,
			Submission = 0xC0,
			TypeKindMask = 0xF0,
			TypeKindShift = 4,
			MustInherit = 0x100,
			NotInheritable = 0x200,
			Shadows = 0x400,
			Partial = 0x800
		}

		[Flags]
		protected enum StateFlags
		{
			FlattenedMembersIsSortedMask = 1,
			ReportedVarianceDiagnostics = 2,
			ReportedBaseClassConstraintsDiagnostics = 4,
			ReportedInterfacesConstraintsDiagnostics = 8
		}

		private enum VarianceContext
		{
			ByVal,
			ByRef,
			Return,
			Constraint,
			Nullable,
			ReadOnlyProperty,
			WriteOnlyProperty,
			Property,
			Complex
		}

		private struct VarianceDiagnosticsTargetTypeParameter
		{
			public readonly NamedTypeSymbol ConstructedType;

			private readonly int _typeParameterIndex;

			public TypeParameterSymbol TypeParameter => ConstructedType.TypeParameters[_typeParameterIndex];

			public VarianceDiagnosticsTargetTypeParameter(NamedTypeSymbol constructedType, int typeParameterIndex)
			{
				this = default(VarianceDiagnosticsTargetTypeParameter);
				ConstructedType = constructedType;
				_typeParameterIndex = typeParameterIndex;
			}
		}

		internal class MembersAndInitializers
		{
			internal readonly Dictionary<string, ImmutableArray<Symbol>> Members;

			internal readonly ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> StaticInitializers;

			internal readonly ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> InstanceInitializers;

			internal readonly int StaticInitializersSyntaxLength;

			internal readonly int InstanceInitializersSyntaxLength;

			internal MembersAndInitializers(Dictionary<string, ImmutableArray<Symbol>> members, ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> staticInitializers, ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> instanceInitializers, int staticInitializersSyntaxLength, int instanceInitializersSyntaxLength)
			{
				Members = members;
				StaticInitializers = staticInitializers;
				InstanceInitializers = instanceInitializers;
				StaticInitializersSyntaxLength = staticInitializersSyntaxLength;
				InstanceInitializersSyntaxLength = instanceInitializersSyntaxLength;
			}
		}

		internal sealed class MembersAndInitializersBuilder
		{
			internal readonly Dictionary<string, ArrayBuilder<Symbol>> Members;

			internal readonly ArrayBuilder<Symbol> DeferredMemberDiagnostic;

			internal int StaticSyntaxLength;

			internal int InstanceSyntaxLength;

			internal ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>> StaticInitializers { get; set; }

			internal ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>> InstanceInitializers { get; set; }

			public MembersAndInitializersBuilder()
			{
				Members = new Dictionary<string, ArrayBuilder<Symbol>>(CaseInsensitiveComparison.Comparer);
				DeferredMemberDiagnostic = ArrayBuilder<Symbol>.GetInstance();
				StaticSyntaxLength = 0;
				InstanceSyntaxLength = 0;
			}

			internal MembersAndInitializers ToReadOnlyAndFree()
			{
				DeferredMemberDiagnostic.Free();
				Dictionary<string, ImmutableArray<Symbol>> dictionary = new Dictionary<string, ImmutableArray<Symbol>>(CaseInsensitiveComparison.Comparer);
				foreach (ArrayBuilder<Symbol> value in Members.Values)
				{
					dictionary.Add(value[0].Name, value.ToImmutableAndFree());
				}
				return new MembersAndInitializers(dictionary, (StaticInitializers != null) ? StaticInitializers.ToImmutableAndFree() : default(ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>), (InstanceInitializers != null) ? InstanceInitializers.ToImmutableAndFree() : default(ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>), StaticSyntaxLength, InstanceSyntaxLength);
			}
		}

		private class StructureCircularityDetectionDataSet
		{
			public struct QueueElement
			{
				public readonly NamedTypeSymbol Type;

				public readonly ConsList<FieldSymbol> Path;

				public QueueElement(NamedTypeSymbol type, ConsList<FieldSymbol> path)
				{
					this = default(QueueElement);
					Type = type;
					Path = path;
				}
			}

			private static readonly ObjectPool<StructureCircularityDetectionDataSet> s_pool = new ObjectPool<StructureCircularityDetectionDataSet>(() => new StructureCircularityDetectionDataSet(), 32);

			public readonly HashSet<NamedTypeSymbol> ProcessedTypes;

			public readonly Queue<QueueElement> Queue;

			private StructureCircularityDetectionDataSet()
			{
				ProcessedTypes = new HashSet<NamedTypeSymbol>();
				Queue = new Queue<QueueElement>();
			}

			public static StructureCircularityDetectionDataSet GetInstance()
			{
				return s_pool.Allocate();
			}

			public void Free()
			{
				Queue.Clear();
				ProcessedTypes.Clear();
				s_pool.Free(this);
			}
		}

		private readonly SourceTypeFlags _flags;

		protected int m_lazyState;

		private readonly NamespaceOrTypeSymbol _containingSymbol;

		protected readonly SourceModuleSymbol m_containingModule;

		private readonly MergedTypeDeclaration _declaration;

		private readonly string _name;

		private string _defaultPropertyName;

		private MembersAndInitializers _lazyMembersAndInitializers;

		private static readonly Dictionary<string, ImmutableArray<NamedTypeSymbol>> s_emptyTypeMembers = new Dictionary<string, ImmutableArray<NamedTypeSymbol>>(CaseInsensitiveComparison.Comparer);

		private Dictionary<string, ImmutableArray<NamedTypeSymbol>> _lazyTypeMembers;

		private ImmutableArray<Symbol> _lazyMembersFlattened;

		private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

		private ThreeState _lazyEmitExtensionAttribute;

		private ThreeState _lazyContainsExtensionMethods;

		private ThreeState _lazyAnyMemberHasAttributes;

		private int _lazyStructureCycle;

		private LexicalSortKey _lazyLexicalSortKey;

		public override Symbol ContainingSymbol => _containingSymbol;

		public override NamedTypeSymbol ContainingType => _containingSymbol as NamedTypeSymbol;

		public override ModuleSymbol ContainingModule => m_containingModule;

		public SourceModuleSymbol ContainingSourceModule => m_containingModule;

		public override Accessibility DeclaredAccessibility => (Accessibility)(_flags & SourceTypeFlags.AccessibilityMask);

		public override bool IsMustInherit => (_flags & SourceTypeFlags.MustInherit) != 0;

		public override bool IsNotInheritable => (_flags & SourceTypeFlags.NotInheritable) != 0;

		internal override bool ShadowsExplicitly => (_flags & SourceTypeFlags.Shadows) != 0;

		public override TypeKind TypeKind => (TypeKind)(ushort)((uint)(_flags & SourceTypeFlags.TypeKindMask) >> 4);

		internal override bool IsInterface => TypeKind == TypeKind.Interface;

		internal bool IsPartial => (_flags & SourceTypeFlags.Partial) != 0;

		internal MergedTypeDeclaration TypeDeclaration => _declaration;

		public override bool IsImplicitlyDeclared => false;

		public sealed override bool IsScriptClass
		{
			get
			{
				DeclarationKind kind = _declaration.Declarations[0].Kind;
				if (kind != DeclarationKind.Script)
				{
					return kind == DeclarationKind.Submission;
				}
				return true;
			}
		}

		public sealed override bool IsImplicitClass => _declaration.Declarations[0].Kind == DeclarationKind.ImplicitClass;

		public sealed override int Arity => _declaration.Arity;

		internal DeclarationKind DeclarationKind => _declaration.Kind;

		public sealed override string Name => _name;

		internal sealed override bool MangleName => Arity > 0;

		public sealed override ImmutableArray<Location> Locations => _declaration.NameLocations;

		public ImmutableArray<SyntaxReference> SyntaxReferences => _declaration.SyntaxReferences;

		public sealed override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper(SyntaxReferences);

		internal override string DefaultPropertyName
		{
			get
			{
				if (TypeKind != TypeKind.Delegate)
				{
					GetMembersAndInitializers();
				}
				return _defaultPropertyName;
			}
		}

		private MembersAndInitializers MemberAndInitializerLookup => GetMembersAndInitializers();

		internal bool MembersHaveBeenCreated => _lazyMembersAndInitializers != null;

		internal override bool KnownCircularStruct
		{
			get
			{
				if (_lazyStructureCycle == 0)
				{
					if (!TypeSymbolExtensions.IsStructureType(this))
					{
						_lazyStructureCycle = 1;
					}
					else
					{
						BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
						bool flag = CheckStructureCircularity(instance);
						m_containingModule.AtomicStoreIntegerAndDiagnostics(ref _lazyStructureCycle, (!flag) ? 1 : 2, 0, instance);
						instance.Free();
					}
				}
				return _lazyStructureCycle == 2;
			}
		}

		public override IEnumerable<string> MemberNames => _declaration.MemberNames;

		public ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> StaticInitializers => MemberAndInitializerLookup.StaticInitializers;

		public ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> InstanceInitializers => MemberAndInitializerLookup.InstanceInitializers;

		public override bool MightContainExtensionMethods
		{
			get
			{
				if (_lazyContainsExtensionMethods == ThreeState.Unknown && (_containingSymbol.Kind != SymbolKind.Namespace || !NamedTypeSymbolExtensions.AllowsExtensionMethods(this) || !AnyMemberHasAttributes))
				{
					_lazyContainsExtensionMethods = ThreeState.False;
				}
				return _lazyContainsExtensionMethods != ThreeState.False;
			}
		}

		internal override MultiDictionary<Symbol, Symbol> ExplicitInterfaceImplementationMap
		{
			get
			{
				if (m_lazyExplicitInterfaceImplementationMap == null)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					MultiDictionary<Symbol, Symbol> value = MakeExplicitInterfaceImplementationMap(instance);
					OverrideHidingHelper.CheckHidingAndOverridingForType(this, instance);
					CheckForOverloadsErrors(instance);
					m_containingModule.AtomicStoreReferenceAndDiagnostics(ref m_lazyExplicitInterfaceImplementationMap, value, instance);
					instance.Free();
				}
				return m_lazyExplicitInterfaceImplementationMap;
			}
		}

		private bool EmitExtensionAttribute
		{
			get
			{
				if (_lazyEmitExtensionAttribute == ThreeState.Unknown)
				{
					BindAllMemberAttributes(default(CancellationToken));
				}
				return _lazyEmitExtensionAttribute == ThreeState.True;
			}
		}

		internal bool AnyMemberHasAttributes
		{
			get
			{
				if (!_lazyAnyMemberHasAttributes.HasValue())
				{
					_lazyAnyMemberHasAttributes = _declaration.AnyMemberHasAttributes.ToThreeState();
				}
				return _lazyAnyMemberHasAttributes.Value();
			}
		}

		protected SourceMemberContainerTypeSymbol(MergedTypeDeclaration declaration, NamespaceOrTypeSymbol containingSymbol, SourceModuleSymbol containingModule)
		{
			_lazyEmitExtensionAttribute = ThreeState.Unknown;
			_lazyContainsExtensionMethods = ThreeState.Unknown;
			_lazyAnyMemberHasAttributes = ThreeState.Unknown;
			_lazyStructureCycle = 0;
			_lazyLexicalSortKey = LexicalSortKey.NotInitialized;
			m_containingModule = containingModule;
			_containingSymbol = containingSymbol;
			_declaration = declaration;
			_name = GetBestName(declaration, containingModule.ContainingSourceAssembly.DeclaringCompilation);
			_flags = ComputeTypeFlags(declaration, containingSymbol.IsNamespace);
		}

		private static string GetBestName(MergedTypeDeclaration declaration, VisualBasicCompilation compilation)
		{
			ImmutableArray<SingleTypeDeclaration> declarations = declaration.Declarations;
			SingleTypeDeclaration singleTypeDeclaration = declarations[0];
			int num = declarations.Length - 1;
			for (int i = 1; i <= num; i++)
			{
				Location location = singleTypeDeclaration.Location;
				if ((object)compilation.FirstSourceLocation(location, declarations[i].Location) != location)
				{
					singleTypeDeclaration = declarations[i];
				}
			}
			return singleTypeDeclaration.Name;
		}

		private SourceTypeFlags ComputeTypeFlags(MergedTypeDeclaration declaration, bool isTopLevel)
		{
			DeclarationModifiers declarationModifiers = DeclarationModifiers.None;
			int num = declaration.Declarations.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				declarationModifiers |= declaration.Declarations[i].Modifiers;
			}
			DeclarationModifiers declarationModifiers2 = declarationModifiers;
			SourceTypeFlags sourceTypeFlags = (SourceTypeFlags)0;
			switch (declaration.Kind)
			{
			case DeclarationKind.Class:
				sourceTypeFlags = SourceTypeFlags.Class;
				if ((declarationModifiers2 & DeclarationModifiers.NotInheritable) != 0)
				{
					sourceTypeFlags |= SourceTypeFlags.NotInheritable;
				}
				else if ((declarationModifiers2 & DeclarationModifiers.MustInherit) != 0)
				{
					sourceTypeFlags |= SourceTypeFlags.MustInherit;
				}
				break;
			case DeclarationKind.Script:
			case DeclarationKind.ImplicitClass:
				sourceTypeFlags = SourceTypeFlags.Class | SourceTypeFlags.NotInheritable;
				break;
			case DeclarationKind.Submission:
				sourceTypeFlags = SourceTypeFlags.Submission | SourceTypeFlags.NotInheritable;
				break;
			case DeclarationKind.Structure:
				sourceTypeFlags = SourceTypeFlags.Structure | SourceTypeFlags.NotInheritable;
				break;
			case DeclarationKind.Interface:
				sourceTypeFlags = SourceTypeFlags.Interface | SourceTypeFlags.MustInherit;
				break;
			case DeclarationKind.Enum:
				sourceTypeFlags = SourceTypeFlags.Enum | SourceTypeFlags.NotInheritable;
				break;
			case DeclarationKind.Delegate:
			case DeclarationKind.EventSyntheticDelegate:
				sourceTypeFlags = SourceTypeFlags.Delegate | SourceTypeFlags.NotInheritable;
				break;
			case DeclarationKind.Module:
				sourceTypeFlags = SourceTypeFlags.Module | SourceTypeFlags.NotInheritable;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(declaration.Kind);
			}
			sourceTypeFlags = (isTopLevel ? (((declarationModifiers2 & DeclarationModifiers.Friend) != 0) ? (sourceTypeFlags | SourceTypeFlags.Friend) : (((declarationModifiers2 & DeclarationModifiers.Public) == 0) ? (sourceTypeFlags | SourceTypeFlags.Friend) : (sourceTypeFlags | SourceTypeFlags.Public))) : (((declarationModifiers2 & (DeclarationModifiers.Private | DeclarationModifiers.Protected)) == (DeclarationModifiers.Private | DeclarationModifiers.Protected)) ? (sourceTypeFlags | SourceTypeFlags.PrivateProtected) : (((declarationModifiers2 & DeclarationModifiers.Private) != 0) ? (sourceTypeFlags | SourceTypeFlags.Private) : (((declarationModifiers2 & (DeclarationModifiers.Protected | DeclarationModifiers.Friend)) == (DeclarationModifiers.Protected | DeclarationModifiers.Friend)) ? (sourceTypeFlags | SourceTypeFlags.ProtectedFriend) : (((declarationModifiers2 & DeclarationModifiers.Protected) != 0) ? (sourceTypeFlags | SourceTypeFlags.Protected) : (((declarationModifiers2 & DeclarationModifiers.Friend) == 0) ? (sourceTypeFlags | SourceTypeFlags.Public) : (sourceTypeFlags | SourceTypeFlags.Friend)))))));
			if ((declarationModifiers2 & DeclarationModifiers.Partial) != 0)
			{
				sourceTypeFlags |= SourceTypeFlags.Partial;
			}
			if ((declarationModifiers2 & DeclarationModifiers.Shadows) != 0)
			{
				sourceTypeFlags |= SourceTypeFlags.Shadows;
			}
			return sourceTypeFlags;
		}

		public static SourceMemberContainerTypeSymbol Create(MergedTypeDeclaration declaration, NamespaceOrTypeSymbol containingSymbol, SourceModuleSymbol containingModule)
		{
			EmbeddedSymbolKind embeddedKind = EmbeddedSymbolExtensions.GetEmbeddedKind(declaration.SyntaxReferences.First().SyntaxTree);
			if (embeddedKind != 0)
			{
				return new EmbeddedSymbolManager.EmbeddedNamedTypeSymbol(declaration, containingSymbol, containingModule, embeddedKind);
			}
			DeclarationKind kind = declaration.Kind;
			if (kind - 7 <= DeclarationKind.Interface)
			{
				return new ImplicitNamedTypeSymbol(declaration, containingSymbol, containingModule);
			}
			SourceNamedTypeSymbol sourceNamedTypeSymbol = new SourceNamedTypeSymbol(declaration, containingSymbol, containingModule);
			if (sourceNamedTypeSymbol.TypeKind == TypeKind.Module)
			{
				sourceNamedTypeSymbol.DeclaringCompilation.EmbeddedSymbolManager.RegisterModuleDeclaration();
			}
			return sourceNamedTypeSymbol;
		}

		private NamedTypeSymbol CreateNestedType(MergedTypeDeclaration declaration)
		{
			if (declaration.Kind == DeclarationKind.Delegate)
			{
				return new SourceNamedTypeSymbol(declaration, this, m_containingModule);
			}
			if (declaration.Kind == DeclarationKind.EventSyntheticDelegate)
			{
				return new SynthesizedEventDelegateSymbol(declaration.SyntaxReferences[0], this);
			}
			return Create(declaration, this, m_containingModule);
		}

		internal sealed override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			GenerateAllDeclarationErrorsImpl(cancellationToken);
		}

		protected virtual void GenerateAllDeclarationErrorsImpl(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			GetMembersAndInitializers();
			cancellationToken.ThrowIfCancellationRequested();
			ImmutableArray<Symbol>.Enumerator enumerator = GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind != SymbolKind.NamedType)
				{
					current.GenerateDeclarationErrors(cancellationToken);
				}
			}
			cancellationToken.ThrowIfCancellationRequested();
			_ = base.BaseTypeNoUseSiteDiagnostics;
			cancellationToken.ThrowIfCancellationRequested();
			_ = base.InterfacesNoUseSiteDiagnostics;
			cancellationToken.ThrowIfCancellationRequested();
			_ = ExplicitInterfaceImplementationMap;
			cancellationToken.ThrowIfCancellationRequested();
			ImmutableArray<TypeParameterSymbol> typeParameters = TypeParameters;
			if (!typeParameters.IsEmpty)
			{
				TypeParameterSymbol.EnsureAllConstraintsAreResolved(typeParameters);
			}
			cancellationToken.ThrowIfCancellationRequested();
			GetAttributes();
			cancellationToken.ThrowIfCancellationRequested();
			BindAllMemberAttributes(cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			GenerateVarianceDiagnostics();
		}

		private void GenerateVarianceDiagnostics()
		{
			if ((m_lazyState & 2) == 0)
			{
				DiagnosticBag diagnostics = null;
				ArrayBuilder<DiagnosticInfo> infosBuffer = null;
				switch (TypeKind)
				{
				case TypeKind.Interface:
					GenerateVarianceDiagnosticsForInterface(ref diagnostics, ref infosBuffer);
					break;
				case TypeKind.Delegate:
					GenerateVarianceDiagnosticsForDelegate(ref diagnostics, ref infosBuffer);
					break;
				case TypeKind.Class:
				case TypeKind.Enum:
				case TypeKind.Struct:
					ReportNestingIntoVariantInterface(ref diagnostics);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(TypeKind);
				case TypeKind.Module:
				case TypeKind.Submission:
					break;
				}
				m_containingModule.AtomicSetFlagAndStoreDiagnostics(ref m_lazyState, 2, 0, (diagnostics != null) ? new BindingDiagnosticBag(diagnostics) : null);
				diagnostics?.Free();
				infosBuffer?.Free();
			}
		}

		private void ReportNestingIntoVariantInterface([In][Out] ref DiagnosticBag diagnostics)
		{
			if (!_containingSymbol.IsType)
			{
				return;
			}
			NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)_containingSymbol;
			do
			{
				if (!TypeSymbolExtensions.IsInterfaceType(namedTypeSymbol))
				{
					namedTypeSymbol = null;
					break;
				}
				if (NamedTypeSymbolExtensions.HaveVariance(namedTypeSymbol.TypeParameters))
				{
					break;
				}
				namedTypeSymbol = namedTypeSymbol.ContainingType;
			}
			while ((object)namedTypeSymbol != null);
			if ((object)namedTypeSymbol != null)
			{
				if (diagnostics == null)
				{
					diagnostics = DiagnosticBag.GetInstance();
				}
				diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_VarianceInterfaceNesting), Locations[0]));
			}
		}

		private void GenerateVarianceDiagnosticsForInterface([In][Out] ref DiagnosticBag diagnostics, [In][Out] ref ArrayBuilder<DiagnosticInfo> infosBuffer)
		{
			if (!NamedTypeSymbolExtensions.HasVariance(this))
			{
				return;
			}
			foreach (ImmutableArray<Symbol> value in GetMembersAndInitializers().Members.Values)
			{
				ImmutableArray<Symbol>.Enumerator enumerator2 = value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current = enumerator2.Current;
					if (!current.IsImplicitlyDeclared)
					{
						switch (current.Kind)
						{
						case SymbolKind.Method:
							GenerateVarianceDiagnosticsForMethod((MethodSymbol)current, ref diagnostics, ref infosBuffer);
							break;
						case SymbolKind.Property:
							GenerateVarianceDiagnosticsForProperty((PropertySymbol)current, ref diagnostics, ref infosBuffer);
							break;
						case SymbolKind.Event:
							GenerateVarianceDiagnosticsForEvent((EventSymbol)current, ref diagnostics, ref infosBuffer);
							break;
						}
					}
				}
			}
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator3 = base.InterfacesNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				NamedTypeSymbol current2 = enumerator3.Current;
				if (!TypeSymbolExtensions.IsErrorType(current2))
				{
					GenerateVarianceDiagnosticsForType(current2, VarianceKind.Out, VarianceContext.Complex, ref infosBuffer);
					if (HaveDiagnostics(infosBuffer))
					{
						ReportDiagnostics(ref diagnostics, GetInheritsOrImplementsLocation(current2, getInherits: true), infosBuffer);
					}
				}
			}
		}

		private Location GetImplementsLocation(NamedTypeSymbol implementedInterface, ref NamedTypeSymbol directInterface)
		{
			directInterface = null;
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = base.InterfacesNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamedTypeSymbol current = enumerator.Current;
				if (TypeSymbol.Equals(current, implementedInterface, TypeCompareKind.ConsiderEverything))
				{
					directInterface = current;
					break;
				}
				if ((object)directInterface == null)
				{
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					if (TypeSymbolExtensions.ImplementsInterface(current, implementedInterface, null, ref useSiteInfo))
					{
						directInterface = current;
					}
				}
			}
			return GetInheritsOrImplementsLocation(directInterface, TypeSymbolExtensions.IsInterfaceType(this));
		}

		private Location GetImplementsLocation(NamedTypeSymbol implementedInterface)
		{
			NamedTypeSymbol directInterface = null;
			return GetImplementsLocation(implementedInterface, ref directInterface);
		}

		protected abstract Location GetInheritsOrImplementsLocation(NamedTypeSymbol @base, bool getInherits);

		private void GenerateVarianceDiagnosticsForDelegate([In][Out] ref DiagnosticBag diagnostics, [In][Out] ref ArrayBuilder<DiagnosticInfo> infosBuffer)
		{
			if (NamedTypeSymbolExtensions.HasVariance(this))
			{
				MethodSymbol delegateInvokeMethod = DelegateInvokeMethod;
				if ((object)delegateInvokeMethod != null)
				{
					GenerateVarianceDiagnosticsForMethod(delegateInvokeMethod, ref diagnostics, ref infosBuffer);
				}
			}
		}

		private static void ReportDiagnostics([In][Out] ref DiagnosticBag diagnostics, Location location, ArrayBuilder<DiagnosticInfo> infos)
		{
			if (diagnostics == null)
			{
				diagnostics = DiagnosticBag.GetInstance();
			}
			ArrayBuilder<DiagnosticInfo>.Enumerator enumerator = infos.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DiagnosticInfo current = enumerator.Current;
				DiagnosticBagExtensions.Add(diagnostics, current, location);
			}
			infos.Clear();
		}

		private static bool HaveDiagnostics(ArrayBuilder<DiagnosticInfo> diagnostics)
		{
			if (diagnostics != null)
			{
				return diagnostics.Count > 0;
			}
			return false;
		}

		private void GenerateVarianceDiagnosticsForType(TypeSymbol type, VarianceKind requiredVariance, VarianceContext context, [In][Out] ref ArrayBuilder<DiagnosticInfo> diagnostics)
		{
			GenerateVarianceDiagnosticsForTypeRecursively(type, requiredVariance, context, default(VarianceDiagnosticsTargetTypeParameter), 0, ref diagnostics);
		}

		private static void AppendVarianceDiagnosticInfo([In][Out] ref ArrayBuilder<DiagnosticInfo> diagnostics, DiagnosticInfo info)
		{
			if (diagnostics == null)
			{
				diagnostics = ArrayBuilder<DiagnosticInfo>.GetInstance();
			}
			diagnostics.Add(info);
		}

		private void GenerateVarianceDiagnosticsForTypeRecursively(TypeSymbol type, VarianceKind requiredVariance, VarianceContext context, VarianceDiagnosticsTargetTypeParameter typeParameterInfo, int constructionDepth, [In][Out] ref ArrayBuilder<DiagnosticInfo> diagnostics)
		{
			switch (type.Kind)
			{
			case SymbolKind.TypeParameter:
			{
				TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)type;
				if ((typeParameterSymbol.Variance != VarianceKind.Out || requiredVariance == VarianceKind.Out) && (typeParameterSymbol.Variance != VarianceKind.In || requiredVariance == VarianceKind.In))
				{
					break;
				}
				bool flag = typeParameterSymbol.Variance == VarianceKind.Out;
				switch (context)
				{
				case VarianceContext.ByVal:
					AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceOutByValDisallowed1, type.Name));
					break;
				case VarianceContext.ByRef:
					AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(flag ? ERRID.ERR_VarianceOutByRefDisallowed1 : ERRID.ERR_VarianceInByRefDisallowed1, type.Name));
					break;
				case VarianceContext.Return:
					AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceInReturnDisallowed1, type.Name));
					break;
				case VarianceContext.Constraint:
					AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceOutConstraintDisallowed1, type.Name));
					break;
				case VarianceContext.Nullable:
					AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(flag ? ERRID.ERR_VarianceOutNullableDisallowed2 : ERRID.ERR_VarianceInNullableDisallowed2, type.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType)));
					break;
				case VarianceContext.ReadOnlyProperty:
					AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceInReadOnlyPropertyDisallowed1, type.Name));
					break;
				case VarianceContext.WriteOnlyProperty:
					AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceOutWriteOnlyPropertyDisallowed1, type.Name));
					break;
				case VarianceContext.Property:
					AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(flag ? ERRID.ERR_VarianceOutPropertyDisallowed1 : ERRID.ERR_VarianceInPropertyDisallowed1, type.Name));
					break;
				case VarianceContext.Complex:
					if ((object)typeParameterInfo.ConstructedType == null)
					{
						AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(flag ? ERRID.ERR_VarianceOutParamDisallowed1 : ERRID.ERR_VarianceInParamDisallowed1, type.Name));
					}
					else if (constructionDepth <= 1)
					{
						if (typeParameterInfo.ConstructedType.Arity <= 1)
						{
							AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(flag ? ERRID.ERR_VarianceOutParamDisallowed1 : ERRID.ERR_VarianceInParamDisallowed1, type.Name));
							break;
						}
						AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(flag ? ERRID.ERR_VarianceOutParamDisallowedForGeneric3 : ERRID.ERR_VarianceInParamDisallowedForGeneric3, type.Name, typeParameterInfo.TypeParameter.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType.OriginalDefinition)));
					}
					else if (typeParameterInfo.ConstructedType.Arity <= 1)
					{
						AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(flag ? ERRID.ERR_VarianceOutParamDisallowedHere2 : ERRID.ERR_VarianceInParamDisallowedHere2, type.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType)));
					}
					else
					{
						AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(flag ? ERRID.ERR_VarianceOutParamDisallowedHereForGeneric4 : ERRID.ERR_VarianceInParamDisallowedHereForGeneric4, type.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType), typeParameterInfo.TypeParameter.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType.OriginalDefinition)));
					}
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(context);
				}
				break;
			}
			case SymbolKind.ArrayType:
				GenerateVarianceDiagnosticsForTypeRecursively(((ArrayTypeSymbol)type).ElementType, requiredVariance, context, typeParameterInfo, constructionDepth, ref diagnostics);
				break;
			case SymbolKind.NamedType:
			{
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(type);
				if (!namedTypeSymbol.IsGenericType)
				{
					break;
				}
				if (requiredVariance != VarianceKind.Out)
				{
					NamedTypeSymbol namedTypeSymbol2 = null;
					NamedTypeSymbol containingType = type.ContainingType;
					while ((object)containingType != null)
					{
						if (NamedTypeSymbolExtensions.HaveVariance(containingType.TypeParameters))
						{
							namedTypeSymbol2 = containingType.OriginalDefinition;
						}
						containingType = containingType.ContainingType;
					}
					NamedTypeSymbol namedTypeSymbol3 = null;
					containingType = this;
					do
					{
						if (NamedTypeSymbolExtensions.HaveVariance(containingType.TypeParameters))
						{
							namedTypeSymbol3 = containingType;
						}
						containingType = containingType.ContainingType;
					}
					while ((object)containingType != null);
					if ((object)namedTypeSymbol2 != null && (object)namedTypeSymbol2 == namedTypeSymbol3)
					{
						if ((object)typeParameterInfo.ConstructedType == null)
						{
							AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceTypeDisallowed2, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(type.OriginalDefinition), CustomSymbolDisplayFormatter.QualifiedName(namedTypeSymbol2)));
						}
						else if (constructionDepth <= 1)
						{
							if (typeParameterInfo.ConstructedType.Arity <= 1)
							{
								AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceTypeDisallowed2, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(type.OriginalDefinition), CustomSymbolDisplayFormatter.QualifiedName(namedTypeSymbol2)));
							}
							else
							{
								AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceTypeDisallowedForGeneric4, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(type.OriginalDefinition), CustomSymbolDisplayFormatter.QualifiedName(namedTypeSymbol2), typeParameterInfo.TypeParameter.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType.OriginalDefinition)));
							}
						}
						else if (typeParameterInfo.ConstructedType.Arity <= 1)
						{
							AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceTypeDisallowedHere3, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(type.OriginalDefinition), CustomSymbolDisplayFormatter.QualifiedName(namedTypeSymbol2), CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType)));
						}
						else
						{
							AppendVarianceDiagnosticInfo(ref diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_VarianceTypeDisallowedHereForGeneric5, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(type.OriginalDefinition), CustomSymbolDisplayFormatter.QualifiedName(namedTypeSymbol2), CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType), typeParameterInfo.TypeParameter.Name, CustomSymbolDisplayFormatter.QualifiedName(typeParameterInfo.ConstructedType.OriginalDefinition)));
						}
						break;
					}
				}
				if (TypeSymbolExtensions.IsNullableType(namedTypeSymbol))
				{
					if (namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[0].IsValueType)
					{
						GenerateVarianceDiagnosticsForTypeRecursively(namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[0], VarianceKind.None, VarianceContext.Nullable, new VarianceDiagnosticsTargetTypeParameter(namedTypeSymbol, 0), constructionDepth, ref diagnostics);
					}
					break;
				}
				do
				{
					int num = namedTypeSymbol.Arity - 1;
					for (int i = 0; i <= num; i++)
					{
						GenerateVarianceDiagnosticsForTypeRecursively(requiredVariance: requiredVariance switch
						{
							VarianceKind.In => namedTypeSymbol.TypeParameters[i].Variance switch
							{
								VarianceKind.In => VarianceKind.Out, 
								VarianceKind.Out => VarianceKind.In, 
								_ => VarianceKind.None, 
							}, 
							VarianceKind.Out => namedTypeSymbol.TypeParameters[i].Variance, 
							_ => VarianceKind.None, 
						}, type: namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[i], context: VarianceContext.Complex, typeParameterInfo: new VarianceDiagnosticsTargetTypeParameter(namedTypeSymbol, i), constructionDepth: constructionDepth + 1, diagnostics: ref diagnostics);
					}
					namedTypeSymbol = namedTypeSymbol.ContainingType;
				}
				while ((object)namedTypeSymbol != null);
				break;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(type.Kind);
			case SymbolKind.ErrorType:
				break;
			}
		}

		private void GenerateVarianceDiagnosticsForMethod(MethodSymbol method, [In][Out] ref DiagnosticBag diagnostics, [In][Out] ref ArrayBuilder<DiagnosticInfo> infosBuffer)
		{
			MethodKind methodKind = method.MethodKind;
			if (methodKind == MethodKind.EventAdd || methodKind == MethodKind.EventRemove || (uint)(methodKind - 11) <= 1u)
			{
				return;
			}
			GenerateVarianceDiagnosticsForParameters(method.Parameters, ref diagnostics, ref infosBuffer);
			GenerateVarianceDiagnosticsForType(method.ReturnType, VarianceKind.Out, VarianceContext.Return, ref infosBuffer);
			if (HaveDiagnostics(infosBuffer))
			{
				MethodBaseSyntax declaringSyntaxNode = SymbolExtensions.GetDeclaringSyntaxNode<MethodBaseSyntax>(method);
				if (declaringSyntaxNode == null && method.MethodKind == MethodKind.DelegateInvoke)
				{
					declaringSyntaxNode = SymbolExtensions.GetDeclaringSyntaxNode<MethodBaseSyntax>(method.ContainingType);
				}
				AsClauseSyntax asClauseSyntax = declaringSyntaxNode?.AsClauseInternal;
				Location location = ((asClauseSyntax == null) ? method.Locations[0] : SyntaxExtensions.Type(asClauseSyntax).GetLocation());
				ReportDiagnostics(ref diagnostics, location, infosBuffer);
			}
			GenerateVarianceDiagnosticsForConstraints(method.TypeParameters, ref diagnostics, ref infosBuffer);
		}

		private void GenerateVarianceDiagnosticsForParameters(ImmutableArray<ParameterSymbol> parameters, [In][Out] ref DiagnosticBag diagnostics, [In][Out] ref ArrayBuilder<DiagnosticInfo> infosBuffer)
		{
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				VarianceKind requiredVariance;
				VarianceContext context;
				if (current.IsByRef)
				{
					requiredVariance = VarianceKind.None;
					context = VarianceContext.ByRef;
				}
				else
				{
					requiredVariance = VarianceKind.In;
					context = VarianceContext.ByVal;
				}
				GenerateVarianceDiagnosticsForType(current.Type, requiredVariance, context, ref infosBuffer);
				if (HaveDiagnostics(infosBuffer))
				{
					ParameterSyntax declaringSyntaxNode = SymbolExtensions.GetDeclaringSyntaxNode<ParameterSyntax>(current);
					Location location = ((declaringSyntaxNode == null || declaringSyntaxNode.AsClause == null) ? current.Locations[0] : declaringSyntaxNode.AsClause.Type.GetLocation());
					ReportDiagnostics(ref diagnostics, location, infosBuffer);
				}
			}
		}

		private void GenerateVarianceDiagnosticsForConstraints(ImmutableArray<TypeParameterSymbol> parameters, [In][Out] ref DiagnosticBag diagnostics, [In][Out] ref ArrayBuilder<DiagnosticInfo> infosBuffer)
		{
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterSymbol current = enumerator.Current;
				ImmutableArray<TypeSymbol>.Enumerator enumerator2 = current.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TypeSymbol current2 = enumerator2.Current;
					GenerateVarianceDiagnosticsForType(current2, VarianceKind.In, VarianceContext.Constraint, ref infosBuffer);
					if (!HaveDiagnostics(infosBuffer))
					{
						continue;
					}
					Location location = current.Locations[0];
					ImmutableArray<TypeParameterConstraint>.Enumerator enumerator3 = current.GetConstraints().GetEnumerator();
					while (enumerator3.MoveNext())
					{
						TypeParameterConstraint current3 = enumerator3.Current;
						if ((object)current3.TypeConstraint != null && TypeSymbolExtensions.IsSameTypeIgnoringAll(current3.TypeConstraint, current2))
						{
							location = current3.LocationOpt;
							break;
						}
					}
					ReportDiagnostics(ref diagnostics, location, infosBuffer);
				}
			}
		}

		private void GenerateVarianceDiagnosticsForProperty(PropertySymbol property, [In][Out] ref DiagnosticBag diagnostics, [In][Out] ref ArrayBuilder<DiagnosticInfo> infosBuffer)
		{
			VarianceKind requiredVariance;
			VarianceContext context;
			if (property.IsReadOnly)
			{
				requiredVariance = VarianceKind.Out;
				context = VarianceContext.ReadOnlyProperty;
			}
			else if (property.IsWriteOnly)
			{
				requiredVariance = VarianceKind.In;
				context = VarianceContext.WriteOnlyProperty;
			}
			else
			{
				requiredVariance = VarianceKind.None;
				context = VarianceContext.Property;
			}
			GenerateVarianceDiagnosticsForType(property.Type, requiredVariance, context, ref infosBuffer);
			if (HaveDiagnostics(infosBuffer))
			{
				PropertyStatementSyntax declaringSyntaxNode = SymbolExtensions.GetDeclaringSyntaxNode<PropertyStatementSyntax>(property);
				Location location = ((declaringSyntaxNode == null || declaringSyntaxNode.AsClause == null) ? property.Locations[0] : SyntaxExtensions.Type(declaringSyntaxNode.AsClause).GetLocation());
				ReportDiagnostics(ref diagnostics, location, infosBuffer);
			}
			GenerateVarianceDiagnosticsForParameters(property.Parameters, ref diagnostics, ref infosBuffer);
		}

		private void GenerateVarianceDiagnosticsForEvent(EventSymbol @event, [In][Out] ref DiagnosticBag diagnostics, [In][Out] ref ArrayBuilder<DiagnosticInfo> infosBuffer)
		{
			TypeSymbol type = @event.Type;
			if (TypeSymbolExtensions.IsDelegateType(type) && (object)((Symbol)type).get_ImplicitlyDefinedBy((Dictionary<string, ArrayBuilder<Symbol>>)null) != @event)
			{
				GenerateVarianceDiagnosticsForType(type, VarianceKind.In, VarianceContext.Complex, ref infosBuffer);
				if (HaveDiagnostics(infosBuffer))
				{
					EventStatementSyntax declaringSyntaxNode = SymbolExtensions.GetDeclaringSyntaxNode<EventStatementSyntax>(@event);
					Location location = ((declaringSyntaxNode == null || declaringSyntaxNode.AsClause == null) ? @event.Locations[0] : declaringSyntaxNode.AsClause.Type.GetLocation());
					ReportDiagnostics(ref diagnostics, location, infosBuffer);
				}
			}
		}

		private void BindAllMemberAttributes(CancellationToken cancellationToken)
		{
			MembersAndInitializers memberAndInitializerLookup = MemberAndInitializerLookup;
			bool flag = false;
			foreach (ImmutableArray<Symbol> value in memberAndInitializerLookup.Members.Values)
			{
				ImmutableArray<Symbol>.Enumerator enumerator2 = value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current = enumerator2.Current;
					current.GetAttributes();
					if (!flag)
					{
						flag = current.Kind == SymbolKind.Method && ((MethodSymbol)current).IsExtensionMethod;
					}
					cancellationToken.ThrowIfCancellationRequested();
				}
			}
			if (flag)
			{
				m_containingModule.RecordPresenceOfExtensionMethods();
				_lazyContainsExtensionMethods = ThreeState.True;
				if (_lazyEmitExtensionAttribute == ThreeState.Unknown)
				{
					UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
					m_containingModule.ContainingSourceAssembly.DeclaringCompilation.GetExtensionAttributeConstructor(out useSiteInfo);
					if (useSiteInfo.DiagnosticInfo != null)
					{
						_lazyEmitExtensionAttribute = ThreeState.False;
						m_containingModule.ContainingSourceAssembly.AnErrorHasBeenReportedAboutExtensionAttribute();
					}
					else
					{
						_lazyEmitExtensionAttribute = ThreeState.True;
					}
				}
			}
			else
			{
				_lazyContainsExtensionMethods = ThreeState.False;
				_lazyEmitExtensionAttribute = ThreeState.False;
			}
		}

		internal override string GetEmittedNamespaceName()
		{
			if (_containingSymbol is SourceNamespaceSymbol sourceNamespaceSymbol && sourceNamespaceSymbol.HasMultipleSpellings)
			{
				Location location = DeclaringCompilation.FirstSourceLocation(Locations);
				return sourceNamespaceSymbol.GetDeclarationSpelling(location.SourceTree, location.SourceSpan.Start);
			}
			return null;
		}

		internal sealed override LexicalSortKey GetLexicalSortKey()
		{
			if (!_lazyLexicalSortKey.IsInitialized)
			{
				ref LexicalSortKey lazyLexicalSortKey = ref _lazyLexicalSortKey;
				LexicalSortKey other = _declaration.GetLexicalSortKey(DeclaringCompilation);
				lazyLexicalSortKey.SetFrom(ref other);
			}
			return _lazyLexicalSortKey;
		}

		internal static SourceNamedTypeSymbol FindSymbolFromSyntax(TypeStatementSyntax declarationSyntax, NamespaceOrTypeSymbol container, ModuleSymbol sourceModule)
		{
			string valueText = declarationSyntax.Identifier.ValueText;
			int arity = DeclarationTreeBuilder.GetArity(declarationSyntax.TypeParameterList);
			DeclarationKind kind = DeclarationTreeBuilder.GetKind(declarationSyntax.Kind());
			return FindSymbolInContainer(valueText, arity, kind, container, sourceModule);
		}

		internal static SourceNamedTypeSymbol FindSymbolFromSyntax(EnumStatementSyntax declarationSyntax, NamespaceOrTypeSymbol container, ModuleSymbol sourceModule)
		{
			string valueText = declarationSyntax.Identifier.ValueText;
			int childArity = 0;
			DeclarationKind kind = DeclarationTreeBuilder.GetKind(declarationSyntax.Kind());
			return FindSymbolInContainer(valueText, childArity, kind, container, sourceModule);
		}

		internal static SourceNamedTypeSymbol FindSymbolFromSyntax(DelegateStatementSyntax declarationSyntax, NamespaceOrTypeSymbol container, ModuleSymbol sourceModule)
		{
			string valueText = declarationSyntax.Identifier.ValueText;
			int arity = DeclarationTreeBuilder.GetArity(declarationSyntax.TypeParameterList);
			DeclarationKind childDeclKind = DeclarationKind.Delegate;
			return FindSymbolInContainer(valueText, arity, childDeclKind, container, sourceModule);
		}

		private static SourceNamedTypeSymbol FindSymbolInContainer(string childName, int childArity, DeclarationKind childDeclKind, NamespaceOrTypeSymbol container, ModuleSymbol sourceModule)
		{
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = container.GetTypeMembers(childName, childArity).GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is SourceNamedTypeSymbol sourceNamedTypeSymbol && (object)sourceNamedTypeSymbol.ContainingModule == sourceModule && sourceNamedTypeSymbol.DeclarationKind == childDeclKind)
				{
					return sourceNamedTypeSymbol;
				}
			}
			return null;
		}

		internal static void AddInitializer(ref ArrayBuilder<FieldOrPropertyInitializer> initializers, Func<int, FieldOrPropertyInitializer> computeInitializer, ref int aggregateSyntaxLength)
		{
			FieldOrPropertyInitializer item = computeInitializer(aggregateSyntaxLength);
			if (initializers == null)
			{
				initializers = ArrayBuilder<FieldOrPropertyInitializer>.GetInstance();
			}
			initializers.Add(item);
			if (!item.IsMetadataConstant)
			{
				aggregateSyntaxLength += item.Syntax.Span.Length;
			}
		}

		internal static void AddInitializers(ref ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>> allInitializers, ArrayBuilder<FieldOrPropertyInitializer> siblings)
		{
			if (siblings != null)
			{
				if (allInitializers == null)
				{
					allInitializers = new ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>>();
				}
				allInitializers.Add(siblings.ToImmutableAndFree());
			}
		}

		protected Dictionary<string, ImmutableArray<NamedTypeSymbol>> GetTypeMembersDictionary()
		{
			if (_lazyTypeMembers == null)
			{
				Interlocked.CompareExchange(ref _lazyTypeMembers, MakeTypeMembers(), null);
			}
			return _lazyTypeMembers;
		}

		private Dictionary<string, ImmutableArray<NamedTypeSymbol>> MakeTypeMembers()
		{
			ImmutableArray<MergedTypeDeclaration> children = _declaration.Children;
			if (children.IsEmpty)
			{
				return s_emptyTypeMembers;
			}
			return Roslyn.Utilities.EnumerableExtensions.ToDictionary(children.Select((MergedTypeDeclaration decl) => CreateNestedType(decl)), (NamedTypeSymbol decl) => decl.Name, CaseInsensitiveComparison.Comparer);
		}

		internal override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
		{
			return GetTypeMembersDictionary().Flatten();
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return GetTypeMembersDictionary().Flatten(LexicalOrderSymbolComparer.Instance);
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			ImmutableArray<NamedTypeSymbol> value = default(ImmutableArray<NamedTypeSymbol>);
			if (GetTypeMembersDictionary().TryGetValue(name, out value))
			{
				return value;
			}
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return GetTypeMembers(name).WhereAsArray((NamedTypeSymbol t, int arity_) => t.Arity == arity_, arity);
		}

		private MembersAndInitializers GetMembersAndInitializers()
		{
			if (_lazyMembersAndInitializers == null)
			{
				BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
				MembersAndInitializers value = BuildMembersAndInitializers(instance);
				m_containingModule.AtomicStoreReferenceAndDiagnostics(ref _lazyMembersAndInitializers, value, instance);
				instance.Free();
				_ = KnownCircularStruct;
			}
			return _lazyMembersAndInitializers;
		}

		private MembersAndInitializers BuildMembersAndInitializers(BindingDiagnosticBag diagBag)
		{
			Dictionary<string, ImmutableArray<NamedTypeSymbol>> typeMembersDictionary = GetTypeMembersDictionary();
			MembersAndInitializers membersAndInitializers = BuildNonTypeMembers(diagBag);
			_defaultPropertyName = DetermineDefaultPropertyName(membersAndInitializers.Members, diagBag);
			ProcessPartialMethodsIfAny(membersAndInitializers.Members, diagBag);
			foreach (ImmutableArray<NamedTypeSymbol> value2 in typeMembersDictionary.Values)
			{
				ImmutableArray<Symbol> value = default(ImmutableArray<Symbol>);
				string name = value2[0].Name;
				if (!membersAndInitializers.Members.TryGetValue(name, out value))
				{
					membersAndInitializers.Members.Add(name, StaticCast<Symbol>.From(value2));
				}
				else
				{
					membersAndInitializers.Members[name] = value.Concat(StaticCast<Symbol>.From(value2));
				}
			}
			return membersAndInitializers;
		}

		private HashSet<SourceMemberMethodSymbol> FindPartialMethodDeclarations(BindingDiagnosticBag diagnostics, Dictionary<string, ImmutableArray<Symbol>> members)
		{
			HashSet<SourceMemberMethodSymbol> hashSet = null;
			foreach (KeyValuePair<string, ImmutableArray<Symbol>> member in members)
			{
				ImmutableArray<Symbol>.Enumerator enumerator2 = member.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (!(enumerator2.Current is SourceMemberMethodSymbol sourceMemberMethodSymbol) || !sourceMemberMethodSymbol.IsPartial || sourceMemberMethodSymbol.MethodKind != MethodKind.Ordinary)
					{
						continue;
					}
					if (!sourceMemberMethodSymbol.IsSub)
					{
						diagnostics.Add(ERRID.ERR_PartialMethodsMustBeSub1, sourceMemberMethodSymbol.NonMergedLocation, sourceMemberMethodSymbol.Name);
						continue;
					}
					if (hashSet == null)
					{
						hashSet = new HashSet<SourceMemberMethodSymbol>(ReferenceEqualityComparer.Instance);
					}
					hashSet.Add(sourceMemberMethodSymbol);
				}
			}
			return hashSet;
		}

		private void ProcessPartialMethodsIfAny(Dictionary<string, ImmutableArray<Symbol>> members, BindingDiagnosticBag diagnostics)
		{
			HashSet<SourceMemberMethodSymbol> hashSet = FindPartialMethodDeclarations(diagnostics, members);
			if (hashSet == null)
			{
				return;
			}
			while (hashSet.Count > 0)
			{
				SourceMemberMethodSymbol sourceMemberMethodSymbol = hashSet.First();
				hashSet.Remove(sourceMemberMethodSymbol);
				SourceMemberMethodSymbol sourceMemberMethodSymbol2 = sourceMemberMethodSymbol;
				Location location = sourceMemberMethodSymbol2.NonMergedLocation;
				SourceMemberMethodSymbol sourceMemberMethodSymbol3 = null;
				Location location2 = null;
				ImmutableArray<Symbol> immutableArray = members[sourceMemberMethodSymbol.Name];
				ImmutableArray<Symbol>.Enumerator enumerator = immutableArray.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (!(enumerator.Current is SourceMemberMethodSymbol sourceMemberMethodSymbol4) || (object)sourceMemberMethodSymbol4 == sourceMemberMethodSymbol || sourceMemberMethodSymbol4.MethodKind != MethodKind.Ordinary || !ComparePartialMethodSignatures(sourceMemberMethodSymbol, sourceMemberMethodSymbol4))
					{
						continue;
					}
					Location nonMergedLocation = sourceMemberMethodSymbol4.NonMergedLocation;
					if (hashSet.Contains(sourceMemberMethodSymbol4))
					{
						hashSet.Remove(sourceMemberMethodSymbol4);
						bool flag = DeclaringCompilation.CompareSourceLocations(location, nonMergedLocation) < 0;
						SourceMemberMethodSymbol sourceMemberMethodSymbol5 = (flag ? sourceMemberMethodSymbol4 : sourceMemberMethodSymbol2);
						string name = sourceMemberMethodSymbol5.Name;
						diagnostics.Add(ERRID.ERR_OnlyOnePartialMethodAllowed2, flag ? nonMergedLocation : location, name, name);
						sourceMemberMethodSymbol5.SuppressDuplicateProcDefDiagnostics = true;
						if (!flag)
						{
							sourceMemberMethodSymbol2 = sourceMemberMethodSymbol4;
							location = nonMergedLocation;
						}
					}
					else
					{
						if (sourceMemberMethodSymbol4.IsPartial)
						{
							continue;
						}
						if ((object)sourceMemberMethodSymbol3 == null)
						{
							sourceMemberMethodSymbol3 = sourceMemberMethodSymbol4;
							location2 = nonMergedLocation;
							continue;
						}
						bool flag2 = DeclaringCompilation.CompareSourceLocations(location2, nonMergedLocation) < 0;
						SourceMemberMethodSymbol sourceMemberMethodSymbol6 = (flag2 ? sourceMemberMethodSymbol4 : sourceMemberMethodSymbol3);
						string name2 = sourceMemberMethodSymbol6.Name;
						diagnostics.Add(ERRID.ERR_OnlyOneImplementingMethodAllowed3, flag2 ? nonMergedLocation : location2, name2, name2, name2);
						sourceMemberMethodSymbol6.SuppressDuplicateProcDefDiagnostics = true;
						if (!flag2)
						{
							sourceMemberMethodSymbol3 = sourceMemberMethodSymbol4;
							location2 = nonMergedLocation;
						}
					}
				}
				if (sourceMemberMethodSymbol2.BlockSyntax != null && sourceMemberMethodSymbol2.BlockSyntax.Statements.Count > 0)
				{
					diagnostics.Add(ERRID.ERR_PartialMethodMustBeEmpty, location);
				}
				if ((object)sourceMemberMethodSymbol3 != null)
				{
					ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
					int num = immutableArray.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						Symbol symbol = immutableArray[i];
						if ((object)sourceMemberMethodSymbol3 != symbol)
						{
							instance.Add(symbol);
						}
					}
					members[sourceMemberMethodSymbol.Name] = instance.ToImmutableAndFree();
					SourceMemberMethodSymbol.InitializePartialMethodParts(sourceMemberMethodSymbol2, sourceMemberMethodSymbol3);
					ReportErrorsOnPartialMethodImplementation(sourceMemberMethodSymbol2, sourceMemberMethodSymbol3, location2, diagnostics);
				}
				else
				{
					SourceMemberMethodSymbol.InitializePartialMethodParts(sourceMemberMethodSymbol2, null);
				}
			}
		}

		private void ReportErrorsOnPartialMethodImplementation(SourceMethodSymbol partialMethod, SourceMethodSymbol implMethod, Location implMethodLocation, BindingDiagnosticBag diagnostics)
		{
			if (implMethod.DeclaredAccessibility != Accessibility.Private)
			{
				diagnostics.Add(ERRID.ERR_ImplementationMustBePrivate2, implMethodLocation, implMethod.Name, partialMethod.Name);
			}
			if (partialMethod.ParameterCount > 0)
			{
				ImmutableArray<ParameterSymbol> parameters = partialMethod.Parameters;
				ImmutableArray<ParameterSymbol> parameters2 = implMethod.Parameters;
				int num = parameters.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					ParameterSymbol parameterSymbol = parameters[i];
					ParameterSymbol parameterSymbol2 = parameters2[i];
					if (!CaseInsensitiveComparison.Equals(parameterSymbol.Name, parameterSymbol2.Name))
					{
						diagnostics.Add(ERRID.ERR_PartialMethodParamNamesMustMatch3, parameterSymbol2.Locations[0], parameterSymbol2.Name, parameterSymbol.Name, implMethod.Name);
					}
				}
			}
			if (implMethod.Arity <= 0)
			{
				return;
			}
			ImmutableArray<TypeParameterSymbol> typeParameters = partialMethod.TypeParameters;
			ImmutableArray<TypeParameterSymbol> typeParameters2 = implMethod.TypeParameters;
			int num2 = typeParameters.Length - 1;
			for (int j = 0; j <= num2; j++)
			{
				TypeParameterSymbol typeParameterSymbol = typeParameters[j];
				TypeParameterSymbol typeParameterSymbol2 = typeParameters2[j];
				if (!CaseInsensitiveComparison.Equals(typeParameterSymbol.Name, typeParameterSymbol2.Name))
				{
					diagnostics.Add(ERRID.ERR_PartialMethodTypeParamNameMismatch3, typeParameterSymbol2.Locations[0], typeParameterSymbol2.Name, typeParameterSymbol.Name, implMethod.Name);
				}
			}
			SymbolComparisonResults comparisons = (SymbolComparisonResults)12;
			if (MethodSignatureComparer.DetailedCompare(partialMethod, implMethod, comparisons) != 0)
			{
				diagnostics.Add(ERRID.ERR_PartialMethodGenericConstraints2, implMethodLocation, implMethod.Name, partialMethod.Name);
			}
		}

		private bool ComparePartialMethodSignatures(SourceMethodSymbol partialDeclaration, SourceMethodSymbol candidate)
		{
			SymbolComparisonResults comparisons = (SymbolComparisonResults)256999;
			if (MethodSignatureComparer.DetailedCompare(partialDeclaration, candidate, comparisons) != 0)
			{
				return false;
			}
			return partialDeclaration.IsShared == candidate.IsShared && partialDeclaration.IsOverrides == candidate.IsOverrides && partialDeclaration.IsMustOverride == candidate.IsMustOverride;
		}

		private bool CheckStructureCircularity(BindingDiagnosticBag diagnostics)
		{
			StructureCircularityDetectionDataSet instance = StructureCircularityDetectionDataSet.GetInstance();
			instance.Queue.Enqueue(new StructureCircularityDetectionDataSet.QueueElement(this, ConsList<FieldSymbol>.Empty));
			bool result = false;
			try
			{
				while (instance.Queue.Count > 0)
				{
					StructureCircularityDetectionDataSet.QueueElement queueElement = instance.Queue.Dequeue();
					if (!instance.ProcessedTypes.Add(queueElement.Type))
					{
						continue;
					}
					bool flag = false;
					ImmutableArray<Symbol>.Enumerator enumerator = queueElement.Type.GetMembers().GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (!(enumerator.Current is FieldSymbol fieldSymbol) || fieldSymbol.IsShared)
						{
							continue;
						}
						NamedTypeSymbol namedTypeSymbol = fieldSymbol.Type as NamedTypeSymbol;
						if ((object)namedTypeSymbol == null || !namedTypeSymbol.IsValueType || (!fieldSymbol.IsDefinition && fieldSymbol.Type.Equals(fieldSymbol.OriginalDefinition.Type)))
						{
							continue;
						}
						if (namedTypeSymbol.OriginalDefinition.Equals(this))
						{
							if (!flag)
							{
								ConsList<FieldSymbol> consList = new ConsList<FieldSymbol>(fieldSymbol, queueElement.Path);
								ArrayBuilder<DiagnosticInfo> instance2 = ArrayBuilder<DiagnosticInfo>.GetInstance();
								FieldSymbol fieldSymbol2 = null;
								while (!consList.IsEmpty())
								{
									fieldSymbol2 = consList.Head;
									instance2.Add(ErrorFactory.ErrorInfo(ERRID.ERR_RecordEmbeds2, fieldSymbol2.ContainingType, fieldSymbol2.Type, fieldSymbol2.Name));
									consList = consList.Tail;
								}
								instance2.ReverseContents();
								Symbol symbol = fieldSymbol2.AssociatedSymbol ?? fieldSymbol2;
								diagnostics.Add(ERRID.ERR_RecordCycle2, symbol.Locations[0], fieldSymbol2.ContainingType.Name, new CompoundDiagnosticInfo(instance2.ToArrayAndFree()));
								flag = true;
								result = true;
							}
						}
						else if (!instance.ProcessedTypes.Contains(namedTypeSymbol))
						{
							if (!namedTypeSymbol.IsDefinition)
							{
								instance.Queue.Enqueue(new StructureCircularityDetectionDataSet.QueueElement(namedTypeSymbol, new ConsList<FieldSymbol>(fieldSymbol, queueElement.Path)));
								namedTypeSymbol = namedTypeSymbol.OriginalDefinition;
							}
							if (DetectTypeCircularity_ShouldStepIntoType(namedTypeSymbol))
							{
								instance.Queue.Enqueue(new StructureCircularityDetectionDataSet.QueueElement(namedTypeSymbol, new ConsList<FieldSymbol>(fieldSymbol, queueElement.Path)));
							}
							else
							{
								instance.ProcessedTypes.Add(namedTypeSymbol);
							}
						}
					}
				}
				return result;
			}
			finally
			{
				instance.Free();
			}
		}

		internal bool DetectTypeCircularity_ShouldStepIntoType(NamedTypeSymbol typeToTest)
		{
			if ((object)typeToTest.ContainingModule == null || !typeToTest.ContainingModule.Equals(ContainingModule))
			{
				return true;
			}
			if (typeToTest.Locations.IsEmpty)
			{
				return true;
			}
			Location location = typeToTest.Locations[0];
			Location location2 = Locations[0];
			int num = DeclaringCompilation.CompareSourceLocations(location, location2);
			return num > 0 || (num == 0 && location.SourceSpan.Start >= location2.SourceSpan.Start);
		}

		private string DetermineDefaultPropertyName(Dictionary<string, ImmutableArray<Symbol>> membersByName, BindingDiagnosticBag diagBag)
		{
			string text = null;
			foreach (KeyValuePair<string, ImmutableArray<Symbol>> item in membersByName)
			{
				string key = item.Key;
				ImmutableArray<Symbol> value = item.Value;
				PropertySymbol propertySymbol = null;
				ImmutableArray<Symbol>.Enumerator enumerator2 = value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current2 = enumerator2.Current;
					if (current2.Kind != SymbolKind.Property)
					{
						continue;
					}
					PropertySymbol propertySymbol2 = (PropertySymbol)current2;
					if (!propertySymbol2.IsDefault)
					{
						continue;
					}
					if (text == null)
					{
						propertySymbol = propertySymbol2;
						text = key;
						if (!propertySymbol.ShadowsExplicitly)
						{
							CheckDefaultPropertyAgainstAllBases(this, text, propertySymbol2.Locations[0], diagBag);
						}
					}
					else
					{
						diagBag.Add(ERRID.ERR_DuplicateDefaultProps1, propertySymbol2.Locations[0], SymbolExtensions.GetKindText(this));
					}
					break;
				}
				if (text == null || EmbeddedOperators.CompareString(text, key, TextCompare: false) != 0)
				{
					continue;
				}
				ImmutableArray<Symbol>.Enumerator enumerator3 = value.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					Symbol current3 = enumerator3.Current;
					if (current3.Kind == SymbolKind.Property)
					{
						SourcePropertySymbol sourcePropertySymbol = (SourcePropertySymbol)current3;
						if (!sourcePropertySymbol.IsDefault)
						{
							diagBag.Add(ERRID.ERR_DefaultMissingFromProperty2, sourcePropertySymbol.Locations[0], propertySymbol, sourcePropertySymbol);
						}
					}
				}
			}
			return text;
		}

		private void CheckDefaultPropertyAgainstAllBases(NamedTypeSymbol namedType, string defaultPropertyName, Location location, BindingDiagnosticBag diagBag)
		{
			if (TypeSymbolExtensions.IsInterfaceType(namedType))
			{
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = namedType.InterfacesNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamedTypeSymbol current = enumerator.Current;
					CheckDefaultPropertyAgainstBase(defaultPropertyName, current, location, diagBag);
				}
			}
			else
			{
				CheckDefaultPropertyAgainstBase(defaultPropertyName, namedType.BaseTypeNoUseSiteDiagnostics, location, diagBag);
			}
		}

		private void CheckDefaultPropertyAgainstBase(string defaultPropertyName, NamedTypeSymbol baseType, Location location, BindingDiagnosticBag diagBag)
		{
			if ((object)baseType == null)
			{
				return;
			}
			string defaultPropertyName2 = baseType.DefaultPropertyName;
			if (defaultPropertyName2 != null)
			{
				if (!CaseInsensitiveComparison.Equals(defaultPropertyName, defaultPropertyName2))
				{
					diagBag.Add(ERRID.WRN_DefaultnessShadowed4, location, defaultPropertyName, defaultPropertyName2, SymbolExtensions.GetKindText(baseType), CustomSymbolDisplayFormatter.ShortErrorName(baseType));
				}
			}
			else
			{
				CheckDefaultPropertyAgainstAllBases(baseType, defaultPropertyName, location, diagBag);
			}
		}

		internal bool AnyInitializerToBeInjectedIntoConstructor(IEnumerable<ImmutableArray<FieldOrPropertyInitializer>> initializerSet, bool includingNonMetadataConstants)
		{
			if (initializerSet != null)
			{
				foreach (ImmutableArray<FieldOrPropertyInitializer> item in initializerSet)
				{
					ImmutableArray<FieldOrPropertyInitializer>.Enumerator enumerator2 = item.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						ImmutableArray<Symbol> fieldsOrProperties = enumerator2.Current.FieldsOrProperties;
						if (!fieldsOrProperties.IsDefault)
						{
							Symbol symbol = fieldsOrProperties.First();
							if (symbol.Kind == SymbolKind.Property)
							{
								return true;
							}
							FieldSymbol fieldSymbol = (FieldSymbol)symbol;
							if (!fieldSymbol.IsConst || (includingNonMetadataConstants && fieldSymbol.IsConstButNotMetadataConstant))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private void CheckForOverloadOverridesShadowsClashesInSameType(MembersAndInitializers membersAndInitializers, BindingDiagnosticBag diagBag)
		{
			bool shadowsExplicitly = default(bool);
			bool overloadsExplicitly = default(bool);
			bool overridesExplicitly = default(bool);
			foreach (KeyValuePair<string, ImmutableArray<Symbol>> member in membersAndInitializers.Members)
			{
				bool flag = true;
				bool flag2 = true;
				bool flag3 = false;
				bool flag4 = false;
				ImmutableArray<Symbol>.Enumerator enumerator2 = member.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current2 = enumerator2.Current;
					SymbolKind kind = current2.Kind;
					if (kind != SymbolKind.Method)
					{
						if (kind != SymbolKind.Property)
						{
							flag3 = false;
							flag4 = false;
							break;
						}
						if (!flag)
						{
							continue;
						}
						flag2 = false;
					}
					else
					{
						if (!flag2)
						{
							continue;
						}
						flag = false;
					}
					if (GetExplicitSymbolFlags(current2, ref shadowsExplicitly, ref overloadsExplicitly, ref overridesExplicitly))
					{
						if (shadowsExplicitly)
						{
							flag3 = true;
							break;
						}
						if (overloadsExplicitly || overridesExplicitly)
						{
							flag4 = true;
						}
					}
				}
				if (!flag3 && !flag4)
				{
					continue;
				}
				ImmutableArray<Symbol>.Enumerator enumerator3 = member.Value.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					Symbol current3 = enumerator3.Current;
					if (((current3.Kind != SymbolKind.Method || !flag2) && (!SymbolExtensions.IsPropertyAndNotWithEvents(current3) || !flag)) || !GetExplicitSymbolFlags(current3, ref shadowsExplicitly, ref overloadsExplicitly, ref overridesExplicitly))
					{
						continue;
					}
					if (flag3)
					{
						if (!shadowsExplicitly)
						{
							diagBag.Add(ERRID.ERR_MustShadow2, current3.Locations[0], SymbolExtensions.GetKindText(current3), current3.Name);
						}
					}
					else if (flag4 && !overridesExplicitly && !overloadsExplicitly)
					{
						diagBag.Add(ERRID.ERR_MustBeOverloads2, current3.Locations[0], SymbolExtensions.GetKindText(current3), current3.Name);
					}
				}
			}
		}

		private bool GetExplicitSymbolFlags(Symbol symbol, ref bool shadowsExplicitly, ref bool overloadsExplicitly, ref bool overridesExplicitly)
		{
			switch (symbol.Kind)
			{
			case SymbolKind.Method:
				if (!(symbol is SourceMethodSymbol sourceMethodSymbol))
				{
					return false;
				}
				shadowsExplicitly = sourceMethodSymbol.ShadowsExplicitly;
				overloadsExplicitly = sourceMethodSymbol.OverloadsExplicitly;
				overridesExplicitly = sourceMethodSymbol.OverridesExplicitly;
				return sourceMethodSymbol.MethodKind == MethodKind.Ordinary || sourceMethodSymbol.MethodKind == MethodKind.DeclareMethod;
			case SymbolKind.Property:
				if (!(symbol is SourcePropertySymbol sourcePropertySymbol))
				{
					return false;
				}
				shadowsExplicitly = sourcePropertySymbol.ShadowsExplicitly;
				overloadsExplicitly = sourcePropertySymbol.OverloadsExplicitly;
				overridesExplicitly = sourcePropertySymbol.OverridesExplicitly;
				return true;
			default:
				throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
			}
		}

		private MembersAndInitializers BuildNonTypeMembers(BindingDiagnosticBag diagnostics)
		{
			MembersAndInitializersBuilder membersAndInitializersBuilder = new MembersAndInitializersBuilder();
			AddDeclaredNonTypeMembers(membersAndInitializersBuilder, diagnostics);
			AddDefaultConstructorIfNeeded(membersAndInitializersBuilder, isShared: false, membersAndInitializersBuilder.InstanceInitializers, diagnostics);
			AddDefaultConstructorIfNeeded(membersAndInitializersBuilder, isShared: true, membersAndInitializersBuilder.StaticInitializers, diagnostics);
			AddWithEventsHookupConstructorsIfNeeded(membersAndInitializersBuilder, diagnostics);
			AddGroupClassMembersIfNeeded(membersAndInitializersBuilder, diagnostics);
			AddEntryPointIfNeeded(membersAndInitializersBuilder);
			CheckMemberDiagnostics(membersAndInitializersBuilder, diagnostics);
			MembersAndInitializers membersAndInitializers = membersAndInitializersBuilder.ToReadOnlyAndFree();
			CheckForOverloadOverridesShadowsClashesInSameType(membersAndInitializers, diagnostics);
			return membersAndInitializers;
		}

		protected virtual void AddEntryPointIfNeeded(MembersAndInitializersBuilder membersBuilder)
		{
		}

		protected abstract void AddDeclaredNonTypeMembers(MembersAndInitializersBuilder membersBuilder, BindingDiagnosticBag diagnostics);

		protected virtual void AddGroupClassMembersIfNeeded(MembersAndInitializersBuilder membersBuilder, BindingDiagnosticBag diagnostics)
		{
		}

		protected void AddMember(StatementSyntax memberSyntax, Binder binder, BindingDiagnosticBag diagBag, MembersAndInitializersBuilder members, ref ArrayBuilder<FieldOrPropertyInitializer> staticInitializers, ref ArrayBuilder<FieldOrPropertyInitializer> instanceInitializers, bool reportAsInvalid)
		{
			switch (memberSyntax.Kind())
			{
			case SyntaxKind.FieldDeclaration:
			{
				FieldDeclarationSyntax fieldDeclarationSyntax = (FieldDeclarationSyntax)memberSyntax;
				if (reportAsInvalid)
				{
					diagBag.Add(ERRID.ERR_InvalidInNamespace, fieldDeclarationSyntax.GetLocation());
				}
				SourceMemberFieldSymbol.Create(this, fieldDeclarationSyntax, binder, members, ref staticInitializers, ref instanceInitializers, diagBag);
				return;
			}
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			case SyntaxKind.ConstructorBlock:
			case SyntaxKind.OperatorBlock:
			{
				MethodBaseSyntax blockStatement = ((MethodBlockBaseSyntax)memberSyntax).BlockStatement;
				if (reportAsInvalid)
				{
					diagBag.Add(ERRID.ERR_InvalidInNamespace, blockStatement.GetLocation());
				}
				SourceMethodSymbol sourceMethodSymbol = CreateMethodMember(blockStatement, binder, diagBag.DiagnosticBag);
				if ((object)sourceMethodSymbol != null)
				{
					AddMember(sourceMethodSymbol, binder, members, omitDiagnostics: false);
				}
				return;
			}
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
			case SyntaxKind.SubNewStatement:
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
			case SyntaxKind.OperatorStatement:
			{
				MethodBaseSyntax methodBaseSyntax = (MethodBaseSyntax)memberSyntax;
				if (reportAsInvalid)
				{
					diagBag.Add(ERRID.ERR_InvalidInNamespace, methodBaseSyntax.GetLocation());
				}
				SourceMethodSymbol sourceMethodSymbol2 = CreateMethodMember((MethodBaseSyntax)memberSyntax, binder, diagBag.DiagnosticBag);
				if ((object)sourceMethodSymbol2 != null)
				{
					AddMember(sourceMethodSymbol2, binder, members, omitDiagnostics: false);
				}
				return;
			}
			case SyntaxKind.PropertyBlock:
			{
				PropertyBlockSyntax propertyBlockSyntax = (PropertyBlockSyntax)memberSyntax;
				if (reportAsInvalid)
				{
					diagBag.Add(ERRID.ERR_InvalidInNamespace, propertyBlockSyntax.PropertyStatement.GetLocation());
				}
				CreateProperty(propertyBlockSyntax.PropertyStatement, propertyBlockSyntax, binder, diagBag.DiagnosticBag, members, ref staticInitializers, ref instanceInitializers);
				return;
			}
			case SyntaxKind.PropertyStatement:
			{
				PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)memberSyntax;
				if (reportAsInvalid)
				{
					diagBag.Add(ERRID.ERR_InvalidInNamespace, propertyStatementSyntax.GetLocation());
				}
				CreateProperty(propertyStatementSyntax, null, binder, diagBag.DiagnosticBag, members, ref staticInitializers, ref instanceInitializers);
				return;
			}
			case SyntaxKind.EventStatement:
			{
				EventStatementSyntax syntax = (EventStatementSyntax)memberSyntax;
				CreateEvent(syntax, null, binder, diagBag.DiagnosticBag, members);
				return;
			}
			case SyntaxKind.EventBlock:
			{
				EventBlockSyntax eventBlockSyntax = (EventBlockSyntax)memberSyntax;
				CreateEvent(eventBlockSyntax.EventStatement, eventBlockSyntax, binder, diagBag.DiagnosticBag, members);
				return;
			}
			case SyntaxKind.LabelStatement:
				return;
			}
			if (memberSyntax.Kind() != SyntaxKind.EmptyStatement && !(memberSyntax is ExecutableStatementSyntax))
			{
				return;
			}
			if (binder.BindingTopLevelScriptCode)
			{
				VB_0024AnonymousDelegate_0<int, FieldOrPropertyInitializer> vB_0024AnonymousDelegate_ = (int precedingInitializersLength) => new FieldOrPropertyInitializer(binder.GetSyntaxReference(memberSyntax), precedingInitializersLength);
				VB_0024AnonymousDelegate_0<int, FieldOrPropertyInitializer> vB_0024AnonymousDelegate_2 = vB_0024AnonymousDelegate_;
				AddInitializer(ref instanceInitializers, (vB_0024AnonymousDelegate_2 == null) ? null : new Func<int, FieldOrPropertyInitializer>(vB_0024AnonymousDelegate_2.Invoke), ref members.InstanceSyntaxLength);
			}
			else if (reportAsInvalid)
			{
				diagBag.Add(ERRID.ERR_InvalidInNamespace, memberSyntax.GetLocation());
			}
		}

		private void CreateProperty(PropertyStatementSyntax syntax, PropertyBlockSyntax blockSyntaxOpt, Binder binder, DiagnosticBag diagBag, MembersAndInitializersBuilder members, ref ArrayBuilder<FieldOrPropertyInitializer> staticInitializers, ref ArrayBuilder<FieldOrPropertyInitializer> instanceInitializers)
		{
			SourcePropertySymbol sourcePropertySymbol = SourcePropertySymbol.Create(this, binder, syntax, blockSyntaxOpt, diagBag);
			AddPropertyAndAccessors(sourcePropertySymbol, binder, members);
			EqualsValueSyntax initializer = syntax.Initializer;
			AsClauseSyntax asClause = syntax.AsClause;
			VisualBasicSyntaxNode visualBasicSyntaxNode = ((asClause == null || asClause.Kind() != SyntaxKind.AsNewClause) ? ((VisualBasicSyntaxNode)initializer) : ((VisualBasicSyntaxNode)asClause));
			if (visualBasicSyntaxNode == null)
			{
				return;
			}
			SyntaxReference syntaxReference = binder.GetSyntaxReference(visualBasicSyntaxNode);
			VB_0024AnonymousDelegate_0<int, FieldOrPropertyInitializer> vB_0024AnonymousDelegate_ = (int precedingInitializersLength) => new FieldOrPropertyInitializer(sourcePropertySymbol, syntaxReference, precedingInitializersLength);
			VB_0024AnonymousDelegate_0<int, FieldOrPropertyInitializer> vB_0024AnonymousDelegate_2;
			if (sourcePropertySymbol.IsShared)
			{
				vB_0024AnonymousDelegate_2 = vB_0024AnonymousDelegate_;
				AddInitializer(ref staticInitializers, (vB_0024AnonymousDelegate_2 == null) ? null : new Func<int, FieldOrPropertyInitializer>(vB_0024AnonymousDelegate_2.Invoke), ref members.StaticSyntaxLength);
				return;
			}
			if (sourcePropertySymbol.IsAutoProperty && sourcePropertySymbol.ContainingType.TypeKind == TypeKind.Struct)
			{
				Binder.ReportDiagnostic(diagBag, syntax.Identifier, ERRID.ERR_AutoPropertyInitializedInStructure);
			}
			vB_0024AnonymousDelegate_2 = vB_0024AnonymousDelegate_;
			AddInitializer(ref instanceInitializers, (vB_0024AnonymousDelegate_2 == null) ? null : new Func<int, FieldOrPropertyInitializer>(vB_0024AnonymousDelegate_2.Invoke), ref members.InstanceSyntaxLength);
		}

		private void CreateEvent(EventStatementSyntax syntax, EventBlockSyntax blockSyntaxOpt, Binder binder, DiagnosticBag diagBag, MembersAndInitializersBuilder members)
		{
			SourceEventSymbol eventSymbol = new SourceEventSymbol(this, binder, syntax, blockSyntaxOpt, diagBag);
			AddEventAndAccessors(eventSymbol, binder, members);
		}

		private SourceMethodSymbol CreateMethodMember(MethodBaseSyntax methodBaseSyntax, Binder binder, DiagnosticBag diagBag)
		{
			switch (methodBaseSyntax.Kind())
			{
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
				return SourceMethodSymbol.CreateRegularMethod(this, (MethodStatementSyntax)methodBaseSyntax, binder, diagBag);
			case SyntaxKind.SubNewStatement:
				return SourceMethodSymbol.CreateConstructor(this, (SubNewStatementSyntax)methodBaseSyntax, binder, diagBag);
			case SyntaxKind.OperatorStatement:
				return SourceMethodSymbol.CreateOperator(this, (OperatorStatementSyntax)methodBaseSyntax, binder, diagBag);
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
				return SourceMethodSymbol.CreateDeclareMethod(this, (DeclareStatementSyntax)methodBaseSyntax, binder, diagBag);
			default:
				throw ExceptionUtilities.UnexpectedValue(methodBaseSyntax.Kind());
			}
		}

		private void AddDefaultConstructorIfNeeded(MembersAndInitializersBuilder members, bool isShared, ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>> initializers, BindingDiagnosticBag diagnostics)
		{
			if (TypeKind == TypeKind.Submission)
			{
				if (!isShared || AnyInitializerToBeInjectedIntoConstructor(initializers, includingNonMetadataConstants: false))
				{
					SyntaxReference syntaxReference = SyntaxReferences.Single();
					Binder binder = BinderBuilder.CreateBinderForType(m_containingModule, syntaxReference.SyntaxTree, this);
					SynthesizedSubmissionConstructorSymbol sym = new SynthesizedSubmissionConstructorSymbol(syntaxReference, this, isShared, binder, diagnostics);
					AddMember(sym, binder, members, omitDiagnostics: false);
				}
			}
			else if (TypeKind == TypeKind.Class || TypeKind == TypeKind.Struct || TypeKind == TypeKind.Enum || (TypeKind == TypeKind.Module && isShared))
			{
				bool flag = AnyInitializerToBeInjectedIntoConstructor(initializers, !isShared);
				if (isShared && !flag)
				{
					return;
				}
				bool isDebuggable = flag;
				EnsureCtor(members, isShared, isDebuggable, diagnostics);
			}
			if (!isShared && IsScriptClass)
			{
				SyntaxReference syntaxReference2 = SyntaxReferences.Single();
				SynthesizedInteractiveInitializerMethod synthesizedInteractiveInitializerMethod = new SynthesizedInteractiveInitializerMethod(syntaxReference2, this, diagnostics);
				AddSymbolToMembers(synthesizedInteractiveInitializerMethod, members.Members);
				SynthesizedEntryPointSymbol memberSymbol = SynthesizedEntryPointSymbol.Create(synthesizedInteractiveInitializerMethod, diagnostics);
				AddSymbolToMembers(memberSymbol, members.Members);
			}
		}

		private void EnsureCtor(MembersAndInitializersBuilder members, bool isShared, bool isDebuggable, BindingDiagnosticBag diagBag)
		{
			string key = (isShared ? ".cctor" : ".ctor");
			ArrayBuilder<Symbol> value = null;
			if (members.Members.TryGetValue(key, out value))
			{
				ArrayBuilder<Symbol>.Enumerator enumerator = value.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MethodSymbol methodSymbol = (MethodSymbol)enumerator.Current;
					if (methodSymbol.MethodKind == MethodKind.Constructor && methodSymbol.ParameterCount == 0)
					{
						return;
					}
				}
				if (TypeKind != TypeKind.Struct || isShared)
				{
					return;
				}
			}
			SyntaxReference syntaxReference = SyntaxReferences.First();
			Binder binder = BinderBuilder.CreateBinderForType(m_containingModule, syntaxReference.SyntaxTree, this);
			SynthesizedConstructorSymbol sym = new SynthesizedConstructorSymbol(syntaxReference, this, isShared, isDebuggable, binder, diagBag);
			AddMember(sym, binder, members, omitDiagnostics: false);
		}

		private void AddWithEventsHookupConstructorsIfNeeded(MembersAndInitializersBuilder members, BindingDiagnosticBag diagBag)
		{
			if (TypeKind == TypeKind.Submission || (TypeKind != TypeKind.Class && TypeKind != TypeKind.Module))
			{
				return;
			}
			ArrayBuilder<SourceMethodSymbol> arrayBuilder = null;
			foreach (ArrayBuilder<Symbol> value2 in members.Members.Values)
			{
				ArrayBuilder<Symbol>.Enumerator enumerator2 = value2.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current is SourceMethodSymbol sourceMethodSymbol && sourceMethodSymbol.HandlesEvents)
					{
						if (arrayBuilder == null)
						{
							arrayBuilder = ArrayBuilder<SourceMethodSymbol>.GetInstance();
						}
						arrayBuilder.Add(sourceMethodSymbol);
					}
				}
			}
			if (arrayBuilder == null)
			{
				return;
			}
			Binder binder = null;
			ArrayBuilder<SourceMethodSymbol>.Enumerator enumerator3 = arrayBuilder.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				SourceMethodSymbol current = enumerator3.Current;
				MethodStatementSyntax methodStatementSyntax = (MethodStatementSyntax)current.DeclarationSyntax;
				SeparatedSyntaxList<HandlesClauseItemSyntax>.Enumerator enumerator4 = methodStatementSyntax.HandlesClause.Events.GetEnumerator();
				while (enumerator4.MoveNext())
				{
					HandlesClauseItemSyntax current2 = enumerator4.Current;
					if (current2.EventContainer.Kind() != SyntaxKind.KeywordEventContainer)
					{
						continue;
					}
					if (!current.IsShared)
					{
						EnsureCtor(members, isShared: false, isDebuggable: false, diagBag);
						continue;
					}
					string valueText = current2.EventMember.Identifier.ValueText;
					EventSymbol eventSymbol = null;
					if (current2.EventContainer.Kind() != SyntaxKind.MyBaseKeyword)
					{
						ArrayBuilder<Symbol> value = null;
						if (members.Members.TryGetValue(valueText, out value) && value.Count == 1 && value[0].Kind == SymbolKind.Event)
						{
							eventSymbol = (EventSymbol)value[0];
						}
					}
					if ((object)eventSymbol == null)
					{
						binder = binder ?? BinderBuilder.CreateBinderForType(m_containingModule, methodStatementSyntax.SyntaxTree, this);
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagBag, m_containingModule.ContainingAssembly);
						NamedTypeSymbol baseTypeNoUseSiteDiagnostics = base.BaseTypeNoUseSiteDiagnostics;
						Binder binder2 = binder;
						LookupResultKind resultKind = LookupResultKind.Empty;
						eventSymbol = SourceMemberMethodSymbol.FindEvent(baseTypeNoUseSiteDiagnostics, binder2, valueText, isThroughMyBase: true, ref useSiteInfo, null, ref resultKind);
						((BindingDiagnosticBag<AssemblySymbol>)diagBag).Add((SyntaxNode)current2.EventMember, useSiteInfo);
					}
					if ((object)eventSymbol != null)
					{
						EnsureCtor(members, eventSymbol.IsShared, isDebuggable: false, diagBag);
					}
				}
			}
			arrayBuilder.Free();
		}

		private void AddPropertyAndAccessors(SourcePropertySymbol propertySymbol, Binder binder, MembersAndInitializersBuilder members)
		{
			AddMember(propertySymbol, binder, members, omitDiagnostics: false);
			if ((object)propertySymbol.GetMethod != null)
			{
				AddMember(propertySymbol.GetMethod, binder, members, omitDiagnostics: false);
			}
			if ((object)propertySymbol.SetMethod != null)
			{
				AddMember(propertySymbol.SetMethod, binder, members, omitDiagnostics: false);
			}
			if ((object)propertySymbol.AssociatedField != null)
			{
				AddMember(propertySymbol.AssociatedField, binder, members, omitDiagnostics: false);
			}
		}

		private void AddEventAndAccessors(SourceEventSymbol eventSymbol, Binder binder, MembersAndInitializersBuilder members)
		{
			AddMember(eventSymbol, binder, members, omitDiagnostics: false);
			if ((object)eventSymbol.AddMethod != null)
			{
				AddMember(eventSymbol.AddMethod, binder, members, omitDiagnostics: false);
			}
			if ((object)eventSymbol.RemoveMethod != null)
			{
				AddMember(eventSymbol.RemoveMethod, binder, members, omitDiagnostics: false);
			}
			if ((object)eventSymbol.RaiseMethod != null)
			{
				AddMember(eventSymbol.RaiseMethod, binder, members, omitDiagnostics: false);
			}
			if ((object)eventSymbol.AssociatedField != null)
			{
				AddMember(eventSymbol.AssociatedField, binder, members, omitDiagnostics: false);
			}
		}

		private void CheckMemberDiagnostics(MembersAndInitializersBuilder members, BindingDiagnosticBag diagBag)
		{
			if (Locations.Length > 1 && !IsPartial)
			{
				return;
			}
			ArrayBuilder<Symbol>.Enumerator enumerator = members.DeferredMemberDiagnostic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (!CheckIfMemberNameConflictsWithTypeMember(current, members, diagBag))
				{
					CheckIfMemberNameIsDuplicate(current, diagBag, members);
				}
				if (current.CanBeReferencedByName && SymbolExtensions.MatchesAnyName(TypeParameters, current.Name))
				{
					if (current.IsImplicitlyDeclared)
					{
						Symbol symbol = current.get_ImplicitlyDefinedBy(members.Members);
						Binder.ReportDiagnostic(diagBag, symbol.Locations[0], ERRID.ERR_SyntMemberShadowsGenericParam3, SymbolExtensions.GetKindText(symbol), symbol.Name, current.Name);
					}
					else
					{
						Binder.ReportDiagnostic(diagBag, current.Locations[0], ERRID.ERR_ShadowingGenericParamWithMember1, current.Name);
					}
				}
			}
		}

		internal void AddMember(Symbol sym, Binder binder, MembersAndInitializersBuilder members, bool omitDiagnostics)
		{
			if (!omitDiagnostics)
			{
				members.DeferredMemberDiagnostic.Add(sym);
			}
			AddSymbolToMembers(sym, members.Members);
		}

		internal void AddSymbolToMembers(Symbol memberSymbol, Dictionary<string, ArrayBuilder<Symbol>> members)
		{
			ArrayBuilder<Symbol> value = null;
			if (members.TryGetValue(memberSymbol.Name, out value))
			{
				value.Add(memberSymbol);
				return;
			}
			value = new ArrayBuilder<Symbol>();
			value.Add(memberSymbol);
			members[memberSymbol.Name] = value;
		}

		private bool CheckIfMemberNameConflictsWithTypeMember(Symbol sym, MembersAndInitializersBuilder members, BindingDiagnosticBag diagBag)
		{
			ImmutableArray<NamedTypeSymbol> typeMembers = GetTypeMembers(sym.Name);
			if (typeMembers.Length > 0)
			{
				NamedTypeSymbol namedTypeSymbol = typeMembers[0];
				if (!TypeSymbol.Equals(sym as TypeSymbol, namedTypeSymbol, TypeCompareKind.ConsiderEverything))
				{
					return CheckIfMemberNameIsDuplicate(sym, namedTypeSymbol, members, diagBag, includeKind: true);
				}
			}
			return false;
		}

		private bool CheckIfMemberNameIsDuplicate(Symbol sym, BindingDiagnosticBag diagBag, MembersAndInitializersBuilder members)
		{
			ArrayBuilder<Symbol> value = null;
			if (members.Members.TryGetValue(sym.Name, out value))
			{
				Symbol symbol = value[0];
				if (sym != symbol)
				{
					return CheckIfMemberNameIsDuplicate(sym, symbol, members, diagBag, includeKind: false);
				}
			}
			return false;
		}

		private bool CheckIfMemberNameIsDuplicate(Symbol firstSymbol, Symbol secondSymbol, MembersAndInitializersBuilder members, BindingDiagnosticBag diagBag, bool includeKind)
		{
			Symbol symbol = secondSymbol.get_ImplicitlyDefinedBy(members.Members);
			if ((object)symbol == null && SymbolExtensions.IsUserDefinedOperator(secondSymbol))
			{
				symbol = secondSymbol;
			}
			Symbol symbol2 = firstSymbol.get_ImplicitlyDefinedBy(members.Members);
			if ((object)symbol2 == null && SymbolExtensions.IsUserDefinedOperator(firstSymbol))
			{
				symbol2 = firstSymbol;
			}
			if ((object)symbol != null)
			{
				if ((object)symbol2 == null)
				{
					if (symbol is TypeSymbol type && TypeSymbolExtensions.IsEnumType(type))
					{
						return true;
					}
					Binder.ReportDiagnostic(diagBag, symbol.Locations[0], ERRID.ERR_SynthMemberClashesWithMember5, SymbolExtensions.GetKindText(symbol), OverrideHidingHelper.AssociatedSymbolName(symbol), secondSymbol.Name, SymbolExtensions.GetKindText(this), Name);
					return true;
				}
				if (!CaseInsensitiveComparison.Equals(symbol.Name, symbol2.Name))
				{
					Binder.ReportDiagnostic(diagBag, symbol.Locations[0], ERRID.ERR_SynthMemberClashesWithSynth7, SymbolExtensions.GetKindText(symbol), OverrideHidingHelper.AssociatedSymbolName(symbol), secondSymbol.Name, SymbolExtensions.GetKindText(symbol2), OverrideHidingHelper.AssociatedSymbolName(symbol2), SymbolExtensions.GetKindText(this), Name);
				}
			}
			else
			{
				if ((object)symbol2 != null)
				{
					Binder.ReportDiagnostic(diagBag, secondSymbol.Locations[0], ERRID.ERR_MemberClashesWithSynth6, SymbolExtensions.GetKindText(secondSymbol), secondSymbol.Name, SymbolExtensions.GetKindText(symbol2), OverrideHidingHelper.AssociatedSymbolName(symbol2), SymbolExtensions.GetKindText(this), Name);
					return true;
				}
				if ((firstSymbol.Kind != SymbolKind.Method && !SymbolExtensions.IsPropertyAndNotWithEvents(firstSymbol)) || firstSymbol.Kind != secondSymbol.Kind)
				{
					if (TypeSymbolExtensions.IsEnumType(this))
					{
						Binder.ReportDiagnostic(diagBag, firstSymbol.Locations[0], ERRID.ERR_MultiplyDefinedEnumMember2, firstSymbol.Name, SymbolExtensions.GetKindText(this));
					}
					else
					{
						Binder.ReportDiagnostic(diagBag, firstSymbol.Locations[0], ERRID.ERR_MultiplyDefinedType3, firstSymbol.Name, includeKind ? ((IFormattable)CustomSymbolDisplayFormatter.ErrorNameWithKind(secondSymbol)) : ((IFormattable)secondSymbol), SymbolExtensions.GetKindText(this));
					}
					return true;
				}
			}
			return false;
		}

		internal override ImmutableArray<Symbol> GetMembersUnordered()
		{
			if (_lazyMembersFlattened.IsDefault)
			{
				ImmutableArray<Symbol> value = MemberAndInitializerLookup.Members.Flatten();
				ImmutableInterlocked.InterlockedInitialize(ref _lazyMembersFlattened, value);
			}
			return _lazyMembersFlattened.ConditionallyDeOrder();
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			if (((uint)m_lazyState & (true ? 1u : 0u)) != 0)
			{
				return _lazyMembersFlattened;
			}
			ImmutableArray<Symbol> immutableArray = GetMembersUnordered();
			if (immutableArray.Length >= 2)
			{
				immutableArray = immutableArray.Sort(LexicalOrderSymbolComparer.Instance);
				ImmutableInterlocked.InterlockedExchange(ref _lazyMembersFlattened, immutableArray);
			}
			ThreadSafeFlagOperations.Set(ref m_lazyState, 1);
			return immutableArray;
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			MembersAndInitializers memberAndInitializerLookup = MemberAndInitializerLookup;
			ImmutableArray<Symbol> value = default(ImmutableArray<Symbol>);
			if (memberAndInitializerLookup.Members.TryGetValue(name, out value))
			{
				return value;
			}
			return ImmutableArray<Symbol>.Empty;
		}

		internal override ImmutableArray<Symbol> GetSimpleNonTypeMembers(string name)
		{
			if (_lazyMembersAndInitializers != null || MemberNames.Contains(name, CaseInsensitiveComparison.Comparer))
			{
				return GetMembers(name);
			}
			return ImmutableArray<Symbol>.Empty;
		}

		internal MethodSymbol CreateSharedConstructorsForConstFieldsIfRequired(Binder binder, BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> staticInitializers = MemberAndInitializerLookup.StaticInitializers;
			if (!staticInitializers.IsDefaultOrEmpty)
			{
				ImmutableArray<Symbol> value = default(ImmutableArray<Symbol>);
				if (!MemberAndInitializerLookup.Members.TryGetValue(".cctor", out value) && AnyInitializerToBeInjectedIntoConstructor(staticInitializers, includingNonMetadataConstants: true))
				{
					return new SynthesizedConstructorSymbol(SyntaxReferences.First(), this, isShared: true, isDebuggable: true, binder, diagnostics);
				}
			}
			return null;
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_156_GetFieldsToEmit))]
		internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_156_GetFieldsToEmit(-2)
			{
				_0024VB_0024Me = this
			};
		}

		internal int CalculateSyntaxOffsetInSynthesizedConstructor(int position, SyntaxTree tree, bool isShared)
		{
			if (IsScriptClass && !isShared)
			{
				int num = 0;
				ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = _declaration.Declarations.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxReference syntaxReference = enumerator.Current.SyntaxReference;
					if (tree == syntaxReference.SyntaxTree)
					{
						return num + position;
					}
					num += syntaxReference.Span.Length;
				}
				throw ExceptionUtilities.Unreachable;
			}
			int syntaxOffset = default(int);
			if (TryCalculateSyntaxOffsetOfPositionInInitializer(position, tree, isShared, ref syntaxOffset))
			{
				return syntaxOffset;
			}
			if (_declaration.Declarations.Length >= 1 && position == _declaration.Declarations[0].Location.SourceSpan.Start)
			{
				return 0;
			}
			throw ExceptionUtilities.Unreachable;
		}

		internal bool TryCalculateSyntaxOffsetOfPositionInInitializer(int position, SyntaxTree tree, bool isShared, ref int syntaxOffset)
		{
			MembersAndInitializers membersAndInitializers = GetMembersAndInitializers();
			ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> initializers = (isShared ? membersAndInitializers.StaticInitializers : membersAndInitializers.InstanceInitializers);
			ImmutableArray<FieldOrPropertyInitializer> initializersInSourceTree = GetInitializersInSourceTree(tree, initializers);
			int num = IndexOfInitializerContainingPosition(initializersInSourceTree, position);
			if (num < 0)
			{
				syntaxOffset = 0;
				return false;
			}
			int num2 = (isShared ? membersAndInitializers.StaticInitializersSyntaxLength : membersAndInitializers.InstanceInitializersSyntaxLength);
			int num3 = position - initializersInSourceTree[num].Syntax.Span.Start;
			int num4 = num2 - (initializersInSourceTree[num].PrecedingInitializersLength + num3);
			syntaxOffset = -num4;
			return true;
		}

		private static ImmutableArray<FieldOrPropertyInitializer> GetInitializersInSourceTree(SyntaxTree tree, ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> initializers)
		{
			ArrayBuilder<FieldOrPropertyInitializer> instance = ArrayBuilder<FieldOrPropertyInitializer>.GetInstance();
			ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Enumerator enumerator = initializers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ImmutableArray<FieldOrPropertyInitializer> current = enumerator.Current;
				if (current.First().Syntax.SyntaxTree == tree)
				{
					instance.AddRange(current);
				}
			}
			return instance.ToImmutableAndFree();
		}

		private static int IndexOfInitializerContainingPosition(ImmutableArray<FieldOrPropertyInitializer> initializers, int position)
		{
			int num = initializers.BinarySearch(position, (FieldOrPropertyInitializer initializer, int pos) => initializer.Syntax.Span.Start.CompareTo(pos));
			if (num >= 0)
			{
				return num;
			}
			num = ~num - 1;
			if (num >= 0 && initializers[num].Syntax.Span.Contains(position))
			{
				return num;
			}
			return -1;
		}

		internal override void BuildExtensionMethodsMap(Dictionary<string, ArrayBuilder<MethodSymbol>> map, NamespaceSymbol appendThrough)
		{
			if (MightContainExtensionMethods)
			{
				MembersAndInitializers memberAndInitializerLookup = MemberAndInitializerLookup;
				if (!appendThrough.BuildExtensionMethodsMap(map, memberAndInitializerLookup.Members))
				{
					_lazyContainsExtensionMethods = ThreeState.False;
				}
			}
		}

		internal override void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder, NamedTypeSymbol appendThrough)
		{
			if (MightContainExtensionMethods)
			{
				MembersAndInitializers memberAndInitializerLookup = MemberAndInitializerLookup;
				if (!appendThrough.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, memberAndInitializerLookup.Members))
				{
					_lazyContainsExtensionMethods = ThreeState.False;
				}
			}
		}

		private MultiDictionary<Symbol, Symbol> MakeExplicitInterfaceImplementationMap(BindingDiagnosticBag diagnostics)
		{
			if (TypeSymbolExtensions.IsClassType(this) || TypeSymbolExtensions.IsStructureType(this) || TypeSymbolExtensions.IsInterfaceType(this))
			{
				CheckInterfaceUnificationAndVariance(diagnostics);
			}
			if (TypeSymbolExtensions.IsClassType(this) || TypeSymbolExtensions.IsStructureType(this))
			{
				MultiDictionary<Symbol, Symbol> multiDictionary = new MultiDictionary<Symbol, Symbol>(ExplicitInterfaceImplementationTargetMemberEqualityComparer.Instance);
				ImmutableArray<Symbol>.Enumerator enumerator = GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					ImmutableArray<Symbol>.Enumerator enumerator2 = ImplementsHelper.GetExplicitInterfaceImplementations(current).GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Symbol current2 = enumerator2.Current;
						if (ShouldReportImplementationError(current2) && multiDictionary.ContainsKey(current2))
						{
							DiagnosticInfo info = ErrorFactory.ErrorInfo(ERRID.ERR_MethodAlreadyImplemented2, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(current2.ContainingType), CustomSymbolDisplayFormatter.ShortErrorName(current2));
							diagnostics.Add(new VBDiagnostic(info, ImplementsHelper.GetImplementingLocation(current, current2)));
						}
						multiDictionary.Add(current2, current);
					}
				}
				foreach (MultiDictionary<NamedTypeSymbol, NamedTypeSymbol>.ValueSet value in base.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Values)
				{
					foreach (NamedTypeSymbol item in value)
					{
						NamedTypeSymbol baseTypeNoUseSiteDiagnostics = base.BaseTypeNoUseSiteDiagnostics;
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
						if (TypeSymbolExtensions.ImplementsInterface(baseTypeNoUseSiteDiagnostics, item, null, ref useSiteInfo))
						{
							continue;
						}
						ImmutableArray<Symbol>.Enumerator enumerator5 = item.GetMembers().GetEnumerator();
						while (enumerator5.MoveNext())
						{
							Symbol current4 = enumerator5.Current;
							if (!SymbolExtensions.RequiresImplementation(current4))
							{
								continue;
							}
							MultiDictionary<Symbol, Symbol>.ValueSet valueSet = multiDictionary[current4];
							UseSiteInfo<AssemblySymbol> useSiteInfo2 = current4.GetUseSiteInfo();
							if (ShouldReportImplementationError(current4))
							{
								if (valueSet.Count == 0)
								{
									DiagnosticInfo info2 = useSiteInfo2.DiagnosticInfo ?? ErrorFactory.ErrorInfo(ERRID.ERR_UnimplementedMember3, TypeSymbolExtensions.IsStructureType(this) ? "Structure" : "Class", CustomSymbolDisplayFormatter.ShortErrorName(this), current4, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(item));
									diagnostics.Add(new VBDiagnostic(info2, GetImplementsLocation(item)));
								}
								else if (valueSet.Count == 1)
								{
									diagnostics.Add(useSiteInfo2, valueSet.Single().Locations[0]);
								}
							}
							else if (valueSet.Count == 1)
							{
								diagnostics.Add(useSiteInfo2, valueSet.Single().Locations[0]);
							}
						}
					}
				}
				if (multiDictionary.Count > 0)
				{
					return multiDictionary;
				}
				return TypeSymbol.EmptyExplicitImplementationMap;
			}
			return TypeSymbol.EmptyExplicitImplementationMap;
		}

		private bool ShouldReportImplementationError(Symbol interfaceMember)
		{
			if (interfaceMember.Kind == SymbolKind.Method && ((MethodSymbol)interfaceMember).MethodKind != MethodKind.Ordinary)
			{
				return false;
			}
			return true;
		}

		private void CheckForOverloadsErrors(BindingDiagnosticBag diagnostics)
		{
			TypeKind typeKind = TypeKind;
			HashSet<MethodSymbol> operatorsKnownToHavePair = null;
			if (typeKind != TypeKind.Class && typeKind != TypeKind.Interface && typeKind != TypeKind.Struct && typeKind != TypeKind.Module)
			{
				return;
			}
			Dictionary<string, ImmutableArray<Symbol>>.Enumerator enumerator = MemberAndInitializerLookup.Members.GetEnumerator();
			bool flag = typeKind != TypeKind.Module && typeKind != TypeKind.Interface;
			while (enumerator.MoveNext())
			{
				ImmutableArray<Symbol> value = enumerator.Current.Value;
				if (value.Length < 2)
				{
					if (flag && !CheckForOperatorOverloadingErrors(value, 0, enumerator, ref operatorsKnownToHavePair, diagnostics))
					{
					}
					continue;
				}
				SymbolKind[] array = new SymbolKind[2]
				{
					SymbolKind.Method,
					SymbolKind.Property
				};
				foreach (SymbolKind symbolKind in array)
				{
					int num = value.Length - 2;
					for (int j = 0; j <= num; j++)
					{
						Symbol symbol = value[j];
						if (symbol.Kind != symbolKind || SymbolExtensions.IsAccessor(symbol) || SymbolExtensions.IsWithEventsProperty(symbol) || (flag && symbolKind == SymbolKind.Method && CheckForOperatorOverloadingErrors(value, j, enumerator, ref operatorsKnownToHavePair, diagnostics)) || (symbol is SourceMemberMethodSymbol sourceMemberMethodSymbol && (MethodSymbolExtensions.IsUserDefinedOperator(sourceMemberMethodSymbol) || sourceMemberMethodSymbol.SuppressDuplicateProcDefDiagnostics)))
						{
							continue;
						}
						int num2 = j + 1;
						int num3 = value.Length - 1;
						for (int k = num2; k <= num3; k++)
						{
							Symbol symbol2 = value[k];
							if (symbol2.Kind == symbolKind && !SymbolExtensions.IsAccessor(symbol2) && !SymbolExtensions.IsWithEventsProperty(symbol2) && (!(symbol2 is SourceMemberMethodSymbol sourceMemberMethodSymbol2) || (!MethodSymbolExtensions.IsUserDefinedOperator(sourceMemberMethodSymbol2) && !sourceMemberMethodSymbol2.SuppressDuplicateProcDefDiagnostics)) && !symbol.IsImplicitlyDeclared && !symbol2.IsImplicitlyDeclared)
							{
								SymbolComparisonResults symbolComparisonResults = OverrideHidingHelper.DetailedSignatureCompare(symbol, symbol2, (SymbolComparisonResults)262119);
								if ((symbolComparisonResults & (SymbolComparisonResults)(-146619)) == 0)
								{
									ReportOverloadsErrors(symbolComparisonResults, symbol, symbol2, symbol.Locations[0], diagnostics);
									break;
								}
							}
						}
					}
					if (flag && symbolKind == SymbolKind.Method)
					{
						CheckForOperatorOverloadingErrors(value, value.Length - 1, enumerator, ref operatorsKnownToHavePair, diagnostics);
					}
				}
			}
		}

		private bool CheckForOperatorOverloadingErrors(ImmutableArray<Symbol> memberList, int memberIndex, Dictionary<string, ImmutableArray<Symbol>>.Enumerator membersEnumerator, [In][Out] ref HashSet<MethodSymbol> operatorsKnownToHavePair, BindingDiagnosticBag diagnostics)
		{
			Symbol symbol = memberList[memberIndex];
			if (symbol.Kind != SymbolKind.Method)
			{
				return false;
			}
			MethodSymbol methodSymbol = (MethodSymbol)symbol;
			SymbolComparisonResults symbolComparisonResults = (SymbolComparisonResults)(-146619);
			MethodKind methodKind = methodSymbol.MethodKind;
			switch (methodKind)
			{
			case MethodKind.Conversion:
				symbolComparisonResults |= SymbolComparisonResults.ReturnTypeMismatch;
				break;
			default:
				return false;
			case MethodKind.UserDefinedOperator:
				break;
			}
			OverloadResolution.OperatorInfo operatorInfo = OverloadResolution.GetOperatorInfo(methodSymbol.Name);
			if (!OverloadResolution.ValidateOverloadedOperator(methodSymbol, operatorInfo, diagnostics, ContainingAssembly))
			{
				return true;
			}
			if (IsConflictingOperatorOverloading(methodSymbol, symbolComparisonResults, memberList, memberIndex + 1, diagnostics))
			{
				return true;
			}
			if (methodKind == MethodKind.Conversion)
			{
				string key = (CaseInsensitiveComparison.Equals("op_Implicit", methodSymbol.Name) ? "op_Explicit" : "op_Implicit");
				ImmutableArray<Symbol> value = default(ImmutableArray<Symbol>);
				if (MemberAndInitializerLookup.Members.TryGetValue(key, out value))
				{
					while (membersEnumerator.MoveNext())
					{
						if (membersEnumerator.Current.Value == value)
						{
							if (!IsConflictingOperatorOverloading(methodSymbol, symbolComparisonResults, value, 0, diagnostics))
							{
								break;
							}
							return true;
						}
					}
				}
			}
			string text = null;
			if (operatorInfo.IsUnary)
			{
				switch (operatorInfo.UnaryOperatorKind)
				{
				case UnaryOperatorKind.IsTrue:
					text = "op_False";
					break;
				case UnaryOperatorKind.IsFalse:
					text = "op_True";
					break;
				}
			}
			else
			{
				switch (operatorInfo.BinaryOperatorKind)
				{
				case BinaryOperatorKind.Equals:
					text = "op_Inequality";
					break;
				case BinaryOperatorKind.NotEquals:
					text = "op_Equality";
					break;
				case BinaryOperatorKind.LessThan:
					text = "op_GreaterThan";
					break;
				case BinaryOperatorKind.GreaterThan:
					text = "op_LessThan";
					break;
				case BinaryOperatorKind.LessThanOrEqual:
					text = "op_GreaterThanOrEqual";
					break;
				case BinaryOperatorKind.GreaterThanOrEqual:
					text = "op_LessThanOrEqual";
					break;
				}
			}
			if (text != null && (operatorsKnownToHavePair == null || !operatorsKnownToHavePair.Contains(methodSymbol)))
			{
				ImmutableArray<Symbol> value2 = default(ImmutableArray<Symbol>);
				if (MemberAndInitializerLookup.Members.TryGetValue(text, out value2))
				{
					ImmutableArray<Symbol>.Enumerator enumerator = value2.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Symbol current = enumerator.Current;
						if (!SymbolExtensions.IsUserDefinedOperator(current))
						{
							continue;
						}
						MethodSymbol methodSymbol2 = (MethodSymbol)current;
						if ((MethodSignatureComparer.DetailedCompare(methodSymbol, methodSymbol2, (SymbolComparisonResults)262086) & (SymbolComparisonResults)(-146617)) == 0)
						{
							if (operatorsKnownToHavePair == null)
							{
								operatorsKnownToHavePair = new HashSet<MethodSymbol>(ReferenceEqualityComparer.Instance);
							}
							operatorsKnownToHavePair.Add(methodSymbol2);
							return true;
						}
					}
				}
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_MatchingOperatorExpected2, SyntaxFacts.GetText(OverloadResolution.GetOperatorTokenKind(text)), methodSymbol), methodSymbol.Locations[0]);
			}
			return true;
		}

		private bool IsConflictingOperatorOverloading(MethodSymbol method, SymbolComparisonResults significantDiff, ImmutableArray<Symbol> memberList, int memberIndex, BindingDiagnosticBag diagnostics)
		{
			int num = memberList.Length - 1;
			for (int i = memberIndex; i <= num; i++)
			{
				Symbol symbol = memberList[i];
				if (symbol.Kind != SymbolKind.Method)
				{
					continue;
				}
				MethodSymbol methodSymbol = (MethodSymbol)symbol;
				if (methodSymbol.MethodKind == method.MethodKind)
				{
					SymbolComparisonResults symbolComparisonResults = MethodSignatureComparer.DetailedCompare(method, methodSymbol, (SymbolComparisonResults)262086);
					if ((symbolComparisonResults & significantDiff) == 0)
					{
						ReportOverloadsErrors(symbolComparisonResults, method, methodSymbol, method.Locations[0], diagnostics);
						return true;
					}
				}
			}
			return false;
		}

		private void CheckInterfaceUnificationAndVariance(BindingDiagnosticBag diagnostics)
		{
			MultiDictionary<NamedTypeSymbol, NamedTypeSymbol> interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics = base.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics;
			if (interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.IsEmpty || (interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Count == 1 && interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Values.Single().Count == 1))
			{
				return;
			}
			if (SymbolExtensions.GetDeclaringSyntaxNode<VisualBasicSyntaxNode>(this) != null)
			{
				foreach (KeyValuePair<NamedTypeSymbol, MultiDictionary<NamedTypeSymbol, NamedTypeSymbol>.ValueSet> item in interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics)
				{
					if (item.Value.Count <= 1)
					{
						continue;
					}
					NamedTypeSymbol key = item.Key;
					foreach (NamedTypeSymbol item2 in item.Value)
					{
						if ((object)key != item2)
						{
							ReportDuplicateInterfaceWithDifferentTupleNames(diagnostics, item2, key);
						}
					}
				}
			}
			MultiDictionary<NamedTypeSymbol, NamedTypeSymbol> multiDictionary = new MultiDictionary<NamedTypeSymbol, NamedTypeSymbol>();
			foreach (NamedTypeSymbol key2 in interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Keys)
			{
				if (key2.IsGenericType)
				{
					multiDictionary.Add(key2.OriginalDefinition, key2);
				}
			}
			foreach (KeyValuePair<NamedTypeSymbol, MultiDictionary<NamedTypeSymbol, NamedTypeSymbol>.ValueSet> item3 in multiDictionary)
			{
				if (item3.Value.Count < 2)
				{
					continue;
				}
				int num = 0;
				foreach (NamedTypeSymbol item4 in item3.Value)
				{
					int num2 = 0;
					foreach (NamedTypeSymbol item5 in item3.Value)
					{
						if (num2 > num)
						{
							if (TypeUnification.CanUnify(this, item4, item5))
							{
								ReportInterfaceUnificationError(diagnostics, item4, item5);
							}
							else
							{
								CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
								if (VarianceAmbiguity.HasVarianceAmbiguity(this, item4, item5, ref useSiteInfo))
								{
									ReportVarianceAmbiguityWarning(diagnostics, item4, item5);
								}
							}
						}
						num2++;
					}
					num++;
				}
			}
		}

		private void ReportOverloadsErrors(SymbolComparisonResults comparisonResults, Symbol firstMember, Symbol secondMember, Location location, BindingDiagnosticBag diagnostics)
		{
			if (Locations.Length > 1 && !IsPartial)
			{
				return;
			}
			if (comparisonResults == (SymbolComparisonResults)0)
			{
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_DuplicateProcDef1, firstMember), location);
				return;
			}
			if ((comparisonResults & SymbolComparisonResults.TupleNamesMismatch) != 0)
			{
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_DuplicateProcDefWithDifferentTupleNames2, firstMember, secondMember), location);
			}
			if ((comparisonResults & SymbolComparisonResults.ParameterByrefMismatch) != 0)
			{
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_OverloadWithByref2, firstMember, secondMember), location);
			}
			if ((comparisonResults & SymbolComparisonResults.ReturnTypeMismatch) != 0)
			{
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_OverloadWithReturnType2, firstMember, secondMember), location);
			}
			if ((comparisonResults & SymbolComparisonResults.ParamArrayMismatch) != 0)
			{
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_OverloadWithArrayVsParamArray2, firstMember, secondMember), location);
			}
			if ((comparisonResults & SymbolComparisonResults.OptionalParameterMismatch) != 0 && (comparisonResults & SymbolComparisonResults.TotalParameterCountMismatch) == 0)
			{
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_OverloadWithOptional2, firstMember, secondMember), location);
			}
			if ((comparisonResults & SymbolComparisonResults.OptionalParameterValueMismatch) != 0)
			{
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_OverloadWithDefault2, firstMember, secondMember), location);
			}
			if ((comparisonResults & SymbolComparisonResults.PropertyAccessorMismatch) != 0)
			{
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_OverloadingPropertyKind2, firstMember, secondMember), location);
			}
		}

		private void ReportInterfaceUnificationError(BindingDiagnosticBag diagnostics, NamedTypeSymbol interface1, NamedTypeSymbol interface2)
		{
			if (GetImplementsLocation(interface1).SourceSpan.Start > GetImplementsLocation(interface2).SourceSpan.Start)
			{
				NamedTypeSymbol namedTypeSymbol = interface1;
				interface1 = interface2;
				interface2 = namedTypeSymbol;
			}
			NamedTypeSymbol directInterface = null;
			NamedTypeSymbol directInterface2 = null;
			GetImplementsLocation(interface1, ref directInterface);
			Location implementsLocation = GetImplementsLocation(interface2, ref directInterface2);
			bool flag = TypeSymbolExtensions.IsInterfaceType(this);
			DiagnosticInfo info = ((TypeSymbol.Equals(directInterface, interface1, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(directInterface2, interface2, TypeCompareKind.ConsiderEverything)) ? ErrorFactory.ErrorInfo(flag ? ERRID.ERR_InterfaceUnifiesWithInterface2 : ERRID.ERR_InterfacePossiblyImplTwice2, CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1)) : ((!TypeSymbol.Equals(directInterface, interface1, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(directInterface2, interface2, TypeCompareKind.ConsiderEverything)) ? ErrorFactory.ErrorInfo(flag ? ERRID.ERR_InterfaceUnifiesWithBase3 : ERRID.ERR_ClassInheritsInterfaceUnifiesWithBase3, CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(directInterface)) : ((!TypeSymbol.Equals(directInterface, interface1, TypeCompareKind.ConsiderEverything) || TypeSymbol.Equals(directInterface2, interface2, TypeCompareKind.ConsiderEverything)) ? ErrorFactory.ErrorInfo(flag ? ERRID.ERR_InterfaceBaseUnifiesWithBase4 : ERRID.ERR_ClassInheritsInterfaceBaseUnifiesWithBase4, CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(directInterface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(directInterface)) : ErrorFactory.ErrorInfo(flag ? ERRID.ERR_BaseUnifiesWithInterfaces3 : ERRID.ERR_ClassInheritsBaseUnifiesWithInterfaces3, CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(directInterface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1)))));
			diagnostics.Add(new VBDiagnostic(info, implementsLocation));
		}

		private void ReportVarianceAmbiguityWarning(BindingDiagnosticBag diagnostics, NamedTypeSymbol interface1, NamedTypeSymbol interface2)
		{
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
			bool flag = VarianceAmbiguity.HasVarianceAmbiguity(this, interface1, interface2, ref useSiteInfo);
			if (flag || !useSiteInfo.Diagnostics.IsNullOrEmpty())
			{
				if (GetImplementsLocation(interface1).SourceSpan.Start > GetImplementsLocation(interface2).SourceSpan.Start)
				{
					NamedTypeSymbol namedTypeSymbol = interface1;
					interface1 = interface2;
					interface2 = namedTypeSymbol;
				}
				NamedTypeSymbol directInterface = null;
				NamedTypeSymbol directInterface2 = null;
				GetImplementsLocation(interface1, ref directInterface);
				Location implementsLocation = GetImplementsLocation(interface2, ref directInterface2);
				if (!diagnostics.Add(implementsLocation, useSiteInfo) && flag)
				{
					DiagnosticInfo info = ErrorFactory.ErrorInfo(ERRID.WRN_VarianceDeclarationAmbiguous3, CustomSymbolDisplayFormatter.QualifiedName(directInterface2), CustomSymbolDisplayFormatter.QualifiedName(directInterface), CustomSymbolDisplayFormatter.ErrorNameWithKind(interface1.OriginalDefinition));
					diagnostics.Add(new VBDiagnostic(info, implementsLocation));
				}
			}
			else
			{
				diagnostics.AddDependencies(useSiteInfo);
			}
		}

		private void ReportDuplicateInterfaceWithDifferentTupleNames(BindingDiagnosticBag diagnostics, NamedTypeSymbol interface1, NamedTypeSymbol interface2)
		{
			if (GetImplementsLocation(interface1).SourceSpan.Start > GetImplementsLocation(interface2).SourceSpan.Start)
			{
				NamedTypeSymbol namedTypeSymbol = interface1;
				interface1 = interface2;
				interface2 = namedTypeSymbol;
			}
			NamedTypeSymbol directInterface = null;
			NamedTypeSymbol directInterface2 = null;
			GetImplementsLocation(interface1, ref directInterface);
			Location implementsLocation = GetImplementsLocation(interface2, ref directInterface2);
			DiagnosticInfo info = ((TypeSymbol.Equals(directInterface, interface1, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(directInterface2, interface2, TypeCompareKind.ConsiderEverything)) ? ErrorFactory.ErrorInfo(IsInterface ? ERRID.ERR_InterfaceInheritedTwiceWithDifferentTupleNames2 : ERRID.ERR_InterfaceImplementedTwiceWithDifferentTupleNames2, CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1)) : ((!TypeSymbol.Equals(directInterface, interface1, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(directInterface2, interface2, TypeCompareKind.ConsiderEverything)) ? ErrorFactory.ErrorInfo(IsInterface ? ERRID.ERR_InterfaceInheritedTwiceWithDifferentTupleNames3 : ERRID.ERR_InterfaceImplementedTwiceWithDifferentTupleNames3, CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(directInterface)) : ((!TypeSymbol.Equals(directInterface, interface1, TypeCompareKind.ConsiderEverything) || TypeSymbol.Equals(directInterface2, interface2, TypeCompareKind.ConsiderEverything)) ? ErrorFactory.ErrorInfo(IsInterface ? ERRID.ERR_InterfaceInheritedTwiceWithDifferentTupleNames4 : ERRID.ERR_InterfaceImplementedTwiceWithDifferentTupleNames4, CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(directInterface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(directInterface)) : ErrorFactory.ErrorInfo(IsInterface ? ERRID.ERR_InterfaceInheritedTwiceWithDifferentTupleNamesReverse3 : ERRID.ERR_InterfaceImplementedTwiceWithDifferentTupleNamesReverse3, CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(directInterface2), CustomSymbolDisplayFormatter.ShortNameWithTypeArgsAndContainingTypes(interface1)))));
			diagnostics.Add(new VBDiagnostic(info, implementsLocation));
		}

		protected void SuppressExtensionAttributeSynthesis()
		{
			_lazyEmitExtensionAttribute = ThreeState.False;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			if (EmitExtensionAttribute)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeExtensionAttribute());
			}
		}
	}
}

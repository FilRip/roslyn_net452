using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.PortableExecutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class AssemblySymbol : Symbol, IAssemblySymbol, IAssemblySymbolInternal
	{
		private AssemblySymbol _corLibrary;

		private static readonly char[] s_nestedTypeNameSeparators = new char[1] { '+' };

		internal AssemblySymbol CorLibrary => _corLibrary;

		private IAssemblySymbolInternal IAssemblySymbolInternal_CorLibrary => CorLibrary;

		public override string Name => Identity.Name;

		public virtual bool IsInteractive => false;

		public abstract AssemblyIdentity Identity { get; }

		public abstract Version AssemblyVersionPattern { get; }

		internal Machine Machine => Modules[0].Machine;

		internal bool Bit32Required => Modules[0].Bit32Required;

		public abstract ImmutableArray<ModuleSymbol> Modules { get; }

		public abstract NamespaceSymbol GlobalNamespace { get; }

		public sealed override SymbolKind Kind => SymbolKind.Assembly;

		public sealed override AssemblySymbol ContainingAssembly => null;

		internal abstract bool IsMissing { get; }

		public sealed override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

		public sealed override bool IsMustOverride => false;

		public sealed override bool IsNotOverridable => false;

		public sealed override bool IsOverridable => false;

		public sealed override bool IsOverrides => false;

		public sealed override bool IsShared => false;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public sealed override Symbol ContainingSymbol => null;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal virtual bool KeepLookingForDeclaredSpecialTypes
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal bool RuntimeSupportsDefaultInterfaceImplementation => RuntimeSupportsFeature(SpecialMember.System_Runtime_CompilerServices_RuntimeFeature__DefaultImplementationsOfInterfaces);

		internal abstract bool IsLinked { get; }

		public abstract ICollection<string> TypeNames { get; }

		public abstract ICollection<string> NamespaceNames { get; }

		internal NamedTypeSymbol ObjectType => GetSpecialType(SpecialType.System_Object);

		public abstract bool MightContainExtensionMethods { get; }

		internal abstract ImmutableArray<byte> PublicKey { get; }

		private INamespaceSymbol IAssemblySymbol_GlobalNamespace => GlobalNamespace;

		private IEnumerable<IModuleSymbol> IAssemblySymbol_Modules => Modules;

		internal void SetCorLibrary(AssemblySymbol corLibrary)
		{
			_corLibrary = corLibrary;
		}

		public abstract AssemblyMetadata GetMetadata();

		AssemblyMetadata IAssemblySymbol.GetMetadata()
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetMetadata
			return this.GetMetadata();
		}

		internal NamespaceSymbol GetAssemblyNamespace(NamespaceSymbol namespaceSymbol)
		{
			if (namespaceSymbol.IsGlobalNamespace)
			{
				return GlobalNamespace;
			}
			NamespaceSymbol containingNamespace = namespaceSymbol.ContainingNamespace;
			if ((object)containingNamespace == null)
			{
				return GlobalNamespace;
			}
			if (namespaceSymbol.Extent.Kind == NamespaceKind.Assembly && namespaceSymbol.ContainingAssembly == this)
			{
				return namespaceSymbol;
			}
			NamespaceSymbol assemblyNamespace = GetAssemblyNamespace(containingNamespace);
			if ((object)assemblyNamespace == containingNamespace)
			{
				return namespaceSymbol;
			}
			return assemblyNamespace?.GetNestedNamespace(namespaceSymbol.Name);
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitAssembly(this, arg);
		}

		internal AssemblySymbol()
		{
		}

		internal NamedTypeSymbol LookupTopLevelMetadataType(ref MetadataTypeName emittedName, bool digThroughForwardedTypes)
		{
			return LookupTopLevelMetadataTypeWithCycleDetection(ref emittedName, null, digThroughForwardedTypes);
		}

		internal abstract NamedTypeSymbol LookupTopLevelMetadataTypeWithCycleDetection(ref MetadataTypeName emittedName, ConsList<AssemblySymbol> visitedAssemblies, bool digThroughForwardedTypes);

		public NamedTypeSymbol ResolveForwardedType(string fullyQualifiedMetadataName)
		{
			if (fullyQualifiedMetadataName == null)
			{
				throw new ArgumentNullException("fullyQualifiedMetadataName");
			}
			MetadataTypeName emittedName = MetadataTypeName.FromFullName(fullyQualifiedMetadataName);
			return TryLookupForwardedMetadataType(ref emittedName, ignoreCase: false);
		}

		internal NamedTypeSymbol TryLookupForwardedMetadataType(ref MetadataTypeName emittedName, bool ignoreCase)
		{
			return TryLookupForwardedMetadataTypeWithCycleDetection(ref emittedName, null, ignoreCase);
		}

		internal virtual NamedTypeSymbol TryLookupForwardedMetadataTypeWithCycleDetection(ref MetadataTypeName emittedName, ConsList<AssemblySymbol> visitedAssemblies, bool ignoreCase)
		{
			return null;
		}

		internal ErrorTypeSymbol CreateCycleInTypeForwarderErrorTypeSymbol(ref MetadataTypeName emittedName)
		{
			DiagnosticInfo errorInfo = new DiagnosticInfo(MessageProvider.Instance, 31425, emittedName.FullName, this);
			return new MissingMetadataTypeSymbol.TopLevelWithCustomErrorInfo(Modules[0], ref emittedName, errorInfo);
		}

		internal ErrorTypeSymbol CreateMultipleForwardingErrorTypeSymbol(ref MetadataTypeName emittedName, ModuleSymbol forwardingModule, AssemblySymbol destination1, AssemblySymbol destination2)
		{
			DiagnosticInfo errorInfo = new DiagnosticInfo(MessageProvider.Instance, 37208, forwardingModule, this, emittedName.FullName, destination1, destination2);
			return new MissingMetadataTypeSymbol.TopLevelWithCustomErrorInfo(forwardingModule, ref emittedName, errorInfo);
		}

		internal abstract IEnumerable<NamedTypeSymbol> GetAllTopLevelForwardedTypes();

		internal abstract NamedTypeSymbol GetDeclaredSpecialType(SpecialType type);

		internal virtual void RegisterDeclaredSpecialType(NamedTypeSymbol corType)
		{
			throw ExceptionUtilities.Unreachable;
		}

		private bool RuntimeSupportsFeature(SpecialMember feature)
		{
			NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Runtime_CompilerServices_RuntimeFeature);
			if (TypeSymbolExtensions.IsClassType(specialType) && specialType.IsMetadataAbstract && specialType.IsMetadataSealed)
			{
				return (object)GetSpecialTypeMember(feature) != null;
			}
			return false;
		}

		internal abstract ImmutableArray<AssemblySymbol> GetNoPiaResolutionAssemblies();

		internal abstract void SetNoPiaResolutionAssemblies(ImmutableArray<AssemblySymbol> assemblies);

		internal abstract ImmutableArray<AssemblySymbol> GetLinkedReferencedAssemblies();

		internal abstract void SetLinkedReferencedAssemblies(ImmutableArray<AssemblySymbol> assemblies);

		internal virtual bool GetGuidString(ref string guidString)
		{
			return GetGuidStringDefaultImplementation(out guidString);
		}

		internal abstract IEnumerable<ImmutableArray<byte>> GetInternalsVisibleToPublicKeys(string simpleName);

		internal abstract bool AreInternalsVisibleToThisAssembly(AssemblySymbol other);

		internal NamedTypeSymbol GetSpecialType(SpecialType type)
		{
			if (type <= SpecialType.None || type > SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute)
			{
				throw new ArgumentOutOfRangeException("type", $"Unexpected SpecialType: '{(int)type}'.");
			}
			return CorLibrary.GetDeclaredSpecialType(type);
		}

		internal NamedTypeSymbol GetPrimitiveType(PrimitiveTypeCode type)
		{
			return GetSpecialType(SpecialTypes.GetTypeFromMetadataName(type));
		}

		public NamedTypeSymbol GetTypeByMetadataName(string fullyQualifiedMetadataName)
		{
			(AssemblySymbol, AssemblySymbol) conflicts = default((AssemblySymbol, AssemblySymbol));
			return GetTypeByMetadataName(fullyQualifiedMetadataName, includeReferences: false, isWellKnownType: false, out conflicts);
		}

		internal NamedTypeSymbol GetTypeByMetadataName(string metadataName, bool includeReferences, bool isWellKnownType, out (AssemblySymbol, AssemblySymbol) conflicts, bool useCLSCompliantNameArityEncoding = false, bool ignoreCorLibraryDuplicatedTypes = false)
		{
			if (metadataName == null)
			{
				throw new ArgumentNullException("metadataName");
			}
			NamedTypeSymbol namedTypeSymbol;
			if (metadataName.Contains("+"))
			{
				string[] array = metadataName.Split(s_nestedTypeNameSeparators);
				MetadataTypeName metadataName2 = MetadataTypeName.FromFullName(array[0], useCLSCompliantNameArityEncoding);
				namedTypeSymbol = GetTopLevelTypeByMetadataName(ref metadataName2, includeReferences, isWellKnownType, out conflicts);
				int num = 1;
				while ((object)namedTypeSymbol != null && !TypeSymbolExtensions.IsErrorType(namedTypeSymbol) && num < array.Length)
				{
					metadataName2 = MetadataTypeName.FromTypeName(array[num]);
					NamedTypeSymbol namedTypeSymbol2 = namedTypeSymbol.LookupMetadataType(ref metadataName2);
					namedTypeSymbol = ((!isWellKnownType || IsValidWellKnownType(namedTypeSymbol2)) ? namedTypeSymbol2 : null);
					num++;
				}
			}
			else
			{
				MetadataTypeName metadataName2 = MetadataTypeName.FromFullName(metadataName, useCLSCompliantNameArityEncoding);
				namedTypeSymbol = GetTopLevelTypeByMetadataName(ref metadataName2, includeReferences, isWellKnownType, out conflicts, ignoreCorLibraryDuplicatedTypes);
			}
			if ((object)namedTypeSymbol != null && !TypeSymbolExtensions.IsErrorType(namedTypeSymbol))
			{
				return namedTypeSymbol;
			}
			return null;
		}

		internal NamedTypeSymbol GetTopLevelTypeByMetadataName(ref MetadataTypeName metadataName, bool includeReferences, bool isWellKnownType, out (AssemblySymbol, AssemblySymbol) conflicts, bool ignoreCorLibraryDuplicatedTypes = false)
		{
			conflicts = default((AssemblySymbol, AssemblySymbol));
			NamedTypeSymbol namedTypeSymbol = LookupTopLevelMetadataType(ref metadataName, digThroughForwardedTypes: false);
			if (isWellKnownType && !IsValidWellKnownType(namedTypeSymbol))
			{
				namedTypeSymbol = null;
			}
			if (IsAcceptableMatchForGetTypeByNameAndArity(namedTypeSymbol))
			{
				return namedTypeSymbol;
			}
			namedTypeSymbol = null;
			if (!includeReferences)
			{
				return namedTypeSymbol;
			}
			bool flag = false;
			if ((object)CorLibrary != this && !CorLibrary.IsMissing && !ignoreCorLibraryDuplicatedTypes)
			{
				NamedTypeSymbol namedTypeSymbol2 = CorLibrary.LookupTopLevelMetadataType(ref metadataName, digThroughForwardedTypes: false);
				flag = true;
				if (IsValidCandidate(namedTypeSymbol2, isWellKnownType))
				{
					return namedTypeSymbol2;
				}
			}
			ImmutableArray<AssemblySymbol> referencedAssemblySymbols = Modules[0].GetReferencedAssemblySymbols();
			int num = referencedAssemblySymbols.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				AssemblySymbol assemblySymbol = referencedAssemblySymbols[i];
				if (flag && (object)assemblySymbol == CorLibrary)
				{
					continue;
				}
				NamedTypeSymbol namedTypeSymbol3 = assemblySymbol.LookupTopLevelMetadataType(ref metadataName, digThroughForwardedTypes: false);
				if (!IsValidCandidate(namedTypeSymbol3, isWellKnownType) || TypeSymbol.Equals(namedTypeSymbol3, namedTypeSymbol, TypeCompareKind.ConsiderEverything))
				{
					continue;
				}
				if ((object)namedTypeSymbol != null)
				{
					if (ignoreCorLibraryDuplicatedTypes)
					{
						if (IsInCorLib(namedTypeSymbol3))
						{
							continue;
						}
						if (IsInCorLib(namedTypeSymbol))
						{
							namedTypeSymbol = namedTypeSymbol3;
							continue;
						}
					}
					conflicts = (namedTypeSymbol.ContainingAssembly, namedTypeSymbol3.ContainingAssembly);
					return null;
				}
				namedTypeSymbol = namedTypeSymbol3;
			}
			return namedTypeSymbol;
		}

		private bool IsValidCandidate(NamedTypeSymbol candidate, bool isWellKnownType)
		{
			if ((!isWellKnownType || IsValidWellKnownType(candidate)) && IsAcceptableMatchForGetTypeByNameAndArity(candidate) && !SymbolExtensions.IsHiddenByVisualBasicEmbeddedAttribute(candidate))
			{
				return !SymbolExtensions.IsHiddenByCodeAnalysisEmbeddedAttribute(candidate);
			}
			return false;
		}

		private bool IsInCorLib(NamedTypeSymbol type)
		{
			return (object)type.ContainingAssembly == CorLibrary;
		}

		internal static bool IsAcceptableMatchForGetTypeByNameAndArity(NamedTypeSymbol candidate)
		{
			if ((object)candidate != null)
			{
				if (candidate.Kind == SymbolKind.ErrorType)
				{
					return !(candidate is MissingMetadataTypeSymbol);
				}
				return true;
			}
			return false;
		}

		internal bool IsValidWellKnownType(NamedTypeSymbol result)
		{
			if ((object)result == null || result.TypeKind == TypeKind.Error)
			{
				return false;
			}
			return result.DeclaredAccessibility == Accessibility.Public || Symbol.IsSymbolAccessible(result, this);
		}

		private bool IAssemblySymbol_GivesAccessTo(IAssemblySymbol assemblyWantingAccess)
		{
			if (object.Equals(this, assemblyWantingAccess))
			{
				return true;
			}
			IEnumerable<ImmutableArray<byte>> internalsVisibleToPublicKeys = GetInternalsVisibleToPublicKeys(assemblyWantingAccess.Name);
			if (internalsVisibleToPublicKeys.Any() && assemblyWantingAccess.IsNetModule())
			{
				return true;
			}
			foreach (ImmutableArray<byte> item in internalsVisibleToPublicKeys)
			{
				if (Identity.PerformIVTCheck(assemblyWantingAccess.Identity.PublicKey, item) == IVTConclusion.Match)
				{
					return true;
				}
			}
			return false;
		}

		bool IAssemblySymbol.GivesAccessTo(IAssemblySymbol assemblyWantingAccess)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IAssemblySymbol_GivesAccessTo
			return this.IAssemblySymbol_GivesAccessTo(assemblyWantingAccess);
		}

		private INamedTypeSymbol IAssemblySymbol_ResolveForwardedType(string metadataName)
		{
			return ResolveForwardedType(metadataName);
		}

		INamedTypeSymbol IAssemblySymbol.ResolveForwardedType(string metadataName)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IAssemblySymbol_ResolveForwardedType
			return this.IAssemblySymbol_ResolveForwardedType(metadataName);
		}

		private ImmutableArray<INamedTypeSymbol> IAssemblySymbol_GetForwardedTypes()
		{
			return ((IEnumerable<INamedTypeSymbol>)(from t in GetAllTopLevelForwardedTypes()
				orderby t.ToDisplayString(SymbolDisplayFormat.QualifiedNameArityFormat)
				select t)).AsImmutable();
		}

		ImmutableArray<INamedTypeSymbol> IAssemblySymbol.GetForwardedTypes()
		{
			//ILSpy generated this explicit interface implementation from .override directive in IAssemblySymbol_GetForwardedTypes
			return this.IAssemblySymbol_GetForwardedTypes();
		}

		private INamedTypeSymbol IAssemblySymbol_GetTypeByMetadataName(string metadataName)
		{
			return GetTypeByMetadataName(metadataName);
		}

		INamedTypeSymbol IAssemblySymbol.GetTypeByMetadataName(string metadataName)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IAssemblySymbol_GetTypeByMetadataName
			return this.IAssemblySymbol_GetTypeByMetadataName(metadataName);
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitAssembly(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitAssembly(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitAssembly(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitAssembly(this);
		}

		internal virtual Symbol GetSpecialTypeMember(SpecialMember member)
		{
			return CorLibrary.GetDeclaredSpecialTypeMember(member);
		}

		internal virtual Symbol GetDeclaredSpecialTypeMember(SpecialMember member)
		{
			return null;
		}
	}
}

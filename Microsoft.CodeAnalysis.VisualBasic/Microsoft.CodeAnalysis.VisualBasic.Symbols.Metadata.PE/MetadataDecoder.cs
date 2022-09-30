using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal class MetadataDecoder : MetadataDecoder<PEModuleSymbol, TypeSymbol, MethodSymbol, FieldSymbol, Symbol>
	{
		private readonly PENamedTypeSymbol _typeContextOpt;

		private readonly PEMethodSymbol _methodContextOpt;

		internal PEModuleSymbol ModuleSymbol => moduleSymbol;

		public MetadataDecoder(PEModuleSymbol moduleSymbol, PENamedTypeSymbol context)
			: this(moduleSymbol, context, null)
		{
		}

		public MetadataDecoder(PEModuleSymbol moduleSymbol, PEMethodSymbol context)
			: this(moduleSymbol, (PENamedTypeSymbol)context.ContainingType, context)
		{
		}

		public MetadataDecoder(PEModuleSymbol moduleSymbol)
			: this(moduleSymbol, null, null)
		{
		}

		private MetadataDecoder(PEModuleSymbol moduleSymbol, PENamedTypeSymbol typeContextOpt, PEMethodSymbol methodContextOpt)
			: base(moduleSymbol.Module, (moduleSymbol.ContainingAssembly is PEAssemblySymbol) ? moduleSymbol.ContainingAssembly.Identity : null, (SymbolFactory<PEModuleSymbol, TypeSymbol>)SymbolFactory.Instance, moduleSymbol)
		{
			_typeContextOpt = typeContextOpt;
			_methodContextOpt = methodContextOpt;
		}

		protected override TypeSymbol GetGenericMethodTypeParamSymbol(int position)
		{
			if ((object)_methodContextOpt == null)
			{
				return new UnsupportedMetadataTypeSymbol();
			}
			ImmutableArray<TypeParameterSymbol> typeParameters = _methodContextOpt.TypeParameters;
			if (typeParameters.Length <= position)
			{
				return new UnsupportedMetadataTypeSymbol();
			}
			return typeParameters[position];
		}

		protected override TypeSymbol GetGenericTypeParamSymbol(int position)
		{
			PENamedTypeSymbol pENamedTypeSymbol = _typeContextOpt;
			while ((object)pENamedTypeSymbol != null && pENamedTypeSymbol.MetadataArity - pENamedTypeSymbol.Arity > position)
			{
				pENamedTypeSymbol = pENamedTypeSymbol.ContainingSymbol as PENamedTypeSymbol;
			}
			if ((object)pENamedTypeSymbol == null || pENamedTypeSymbol.MetadataArity <= position)
			{
				return new UnsupportedMetadataTypeSymbol();
			}
			position -= pENamedTypeSymbol.MetadataArity - pENamedTypeSymbol.Arity;
			return pENamedTypeSymbol.TypeParameters[position];
		}

		protected override ConcurrentDictionary<TypeDefinitionHandle, TypeSymbol> GetTypeHandleToTypeMap()
		{
			return ModuleSymbol.TypeHandleToTypeMap;
		}

		protected override ConcurrentDictionary<TypeReferenceHandle, TypeSymbol> GetTypeRefHandleToTypeMap()
		{
			return ModuleSymbol.TypeRefHandleToTypeMap;
		}

		protected override TypeSymbol LookupNestedTypeDefSymbol(TypeSymbol container, ref MetadataTypeName emittedName)
		{
			return container.LookupMetadataType(ref emittedName);
		}

		protected override TypeSymbol LookupTopLevelTypeDefSymbol(int referencedAssemblyIndex, ref MetadataTypeName emittedName)
		{
			AssemblySymbol referencedAssemblySymbol = ModuleSymbol.GetReferencedAssemblySymbol(referencedAssemblyIndex);
			if ((object)referencedAssemblySymbol == null)
			{
				return new UnsupportedMetadataTypeSymbol();
			}
			try
			{
				return referencedAssemblySymbol.LookupTopLevelMetadataType(ref emittedName, digThroughForwardedTypes: true);
			}
			catch (Exception ex) when (((Func<bool>)delegate
			{
				// Could not convert BlockContainer to single expression
				ProjectData.SetProjectError(ex);
				return FatalError.ReportAndPropagate(ex);
			}).Invoke())
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		protected override TypeSymbol LookupTopLevelTypeDefSymbol(string moduleName, ref MetadataTypeName emittedName, out bool isNoPiaLocalType)
		{
			ImmutableArray<ModuleSymbol>.Enumerator enumerator = ModuleSymbol.ContainingAssembly.Modules.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ModuleSymbol current = enumerator.Current;
				if (string.Equals(current.Name, moduleName, StringComparison.OrdinalIgnoreCase))
				{
					if ((object)current == ModuleSymbol)
					{
						return ModuleSymbol.LookupTopLevelMetadataType(ref emittedName, out isNoPiaLocalType);
					}
					isNoPiaLocalType = false;
					return current.LookupTopLevelMetadataType(ref emittedName);
				}
			}
			isNoPiaLocalType = false;
			return new MissingMetadataTypeSymbol.TopLevel(new MissingModuleSymbolWithName(ModuleSymbol.ContainingAssembly, moduleName), ref emittedName, SpecialType.None);
		}

		protected override TypeSymbol LookupTopLevelTypeDefSymbol(ref MetadataTypeName emittedName, out bool isNoPiaLocalType)
		{
			return ModuleSymbol.LookupTopLevelMetadataType(ref emittedName, out isNoPiaLocalType);
		}

		protected override int GetIndexOfReferencedAssembly(AssemblyIdentity identity)
		{
			ImmutableArray<AssemblyIdentity> referencedAssemblies = ModuleSymbol.GetReferencedAssemblies();
			int num = referencedAssemblies.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (identity.Equals(referencedAssemblies[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool IsOrClosedOverATypeFromAssemblies(TypeSymbol @this, ImmutableArray<AssemblySymbol> assemblies)
		{
			switch (@this.Kind)
			{
			case SymbolKind.TypeParameter:
				return false;
			case SymbolKind.ArrayType:
				return IsOrClosedOverATypeFromAssemblies(((ArrayTypeSymbol)@this).ElementType, assemblies);
			case SymbolKind.ErrorType:
			case SymbolKind.NamedType:
			{
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)@this;
				AssemblySymbol containingAssembly = namedTypeSymbol.OriginalDefinition.ContainingAssembly;
				if ((object)containingAssembly != null)
				{
					int num = assemblies.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						if ((object)containingAssembly == assemblies[i])
						{
							return true;
						}
					}
				}
				do
				{
					if (namedTypeSymbol.IsTupleType)
					{
						return IsOrClosedOverATypeFromAssemblies(namedTypeSymbol.TupleUnderlyingType, assemblies);
					}
					ImmutableArray<TypeSymbol>.Enumerator enumerator = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (IsOrClosedOverATypeFromAssemblies(enumerator.Current, assemblies))
						{
							return true;
						}
					}
					namedTypeSymbol = namedTypeSymbol.ContainingType;
				}
				while ((object)namedTypeSymbol != null);
				return false;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(@this.Kind);
			}
		}

		protected override TypeSymbol SubstituteNoPiaLocalType(TypeDefinitionHandle typeDef, ref MetadataTypeName name, string interfaceGuid, string scope, string identifier)
		{
			TypeSymbol value;
			try
			{
				bool flag = Module.IsInterfaceOrThrow(typeDef);
				TypeSymbol baseType = null;
				if (!flag)
				{
					EntityHandle baseTypeOfTypeOrThrow = Module.GetBaseTypeOfTypeOrThrow(typeDef);
					if (!baseTypeOfTypeOrThrow.IsNil)
					{
						baseType = GetTypeOfToken(baseTypeOfTypeOrThrow);
					}
				}
				value = SubstituteNoPiaLocalType(ref name, flag, baseType, interfaceGuid, scope, identifier, ModuleSymbol.ContainingAssembly);
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException exception = ex;
				value = GetUnsupportedMetadataTypeSymbol(exception);
				ProjectData.ClearProjectError();
			}
			return GetTypeHandleToTypeMap().GetOrAdd(typeDef, value);
		}

		internal static NamedTypeSymbol SubstituteNoPiaLocalType(ref MetadataTypeName fullEmittedName, bool isInterface, TypeSymbol baseType, string interfaceGuid, string scope, string identifier, AssemblySymbol referringAssembly)
		{
			NamedTypeSymbol namedTypeSymbol = null;
			Guid result = default(Guid);
			bool flag = false;
			Guid result2 = default(Guid);
			bool flag2 = false;
			if (isInterface && interfaceGuid != null)
			{
				flag = Guid.TryParse(interfaceGuid, out result);
				if (flag)
				{
					scope = null;
					identifier = null;
				}
			}
			if (scope != null)
			{
				flag2 = Guid.TryParse(scope, out result2);
			}
			ImmutableArray<AssemblySymbol>.Enumerator enumerator = referringAssembly.GetNoPiaResolutionAssemblies().GetEnumerator();
			while (enumerator.MoveNext())
			{
				AssemblySymbol current = enumerator.Current;
				if ((object)current == referringAssembly)
				{
					continue;
				}
				NamedTypeSymbol namedTypeSymbol2 = current.LookupTopLevelMetadataType(ref fullEmittedName, digThroughForwardedTypes: false);
				if (namedTypeSymbol2.Kind == SymbolKind.ErrorType || (object)namedTypeSymbol2.ContainingAssembly != current || namedTypeSymbol2.DeclaredAccessibility != Accessibility.Public)
				{
					continue;
				}
				string guidString = null;
				bool flag3 = false;
				Guid result3 = default(Guid);
				switch (namedTypeSymbol2.TypeKind)
				{
				case TypeKind.Interface:
					if (!isInterface)
					{
						continue;
					}
					if (namedTypeSymbol2.GetGuidString(ref guidString) && guidString != null)
					{
						flag3 = Guid.TryParse(guidString, out result3);
					}
					break;
				case TypeKind.Delegate:
				case TypeKind.Enum:
				case TypeKind.Struct:
				{
					if (isInterface)
					{
						continue;
					}
					SpecialType specialType = namedTypeSymbol2.BaseTypeNoUseSiteDiagnostics?.SpecialType ?? SpecialType.None;
					if (specialType == SpecialType.None || specialType != (baseType?.SpecialType ?? SpecialType.None))
					{
						continue;
					}
					break;
				}
				default:
					continue;
				}
				if (flag || flag3)
				{
					if (!flag || !flag3 || result3 != result)
					{
						continue;
					}
				}
				else
				{
					if (!flag2 || identifier == null || !string.Equals(identifier, fullEmittedName.FullName, StringComparison.Ordinal))
					{
						continue;
					}
					flag3 = false;
					if (current.GetGuidString(ref guidString) && guidString != null)
					{
						flag3 = Guid.TryParse(guidString, out result3);
					}
					if (!flag3 || result2 != result3)
					{
						continue;
					}
				}
				if ((object)namedTypeSymbol != null)
				{
					namedTypeSymbol = new NoPiaAmbiguousCanonicalTypeSymbol(referringAssembly, namedTypeSymbol, namedTypeSymbol2);
					break;
				}
				namedTypeSymbol = namedTypeSymbol2;
			}
			if ((object)namedTypeSymbol == null)
			{
				namedTypeSymbol = new NoPiaMissingCanonicalTypeSymbol(referringAssembly, fullEmittedName.FullName, interfaceGuid, scope, identifier);
			}
			return namedTypeSymbol;
		}

		protected override MethodSymbol FindMethodSymbolInType(TypeSymbol typeSymbol, MethodDefinitionHandle targetMethodDef)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = typeSymbol.GetMembersUnordered().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is PEMethodSymbol pEMethodSymbol && pEMethodSymbol.Handle == targetMethodDef)
				{
					return pEMethodSymbol;
				}
			}
			return null;
		}

		protected override FieldSymbol FindFieldSymbolInType(TypeSymbol typeSymbol, FieldDefinitionHandle fieldDef)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = typeSymbol.GetMembersUnordered().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is PEFieldSymbol pEFieldSymbol && pEFieldSymbol.Handle == fieldDef)
				{
					return pEFieldSymbol;
				}
			}
			return null;
		}

		internal override Symbol GetSymbolForMemberRef(MemberReferenceHandle memberRef, TypeSymbol scope = null, bool methodsOnly = false)
		{
			TypeSymbol typeSymbol = GetMemberRefTypeSymbol(memberRef);
			if ((object)typeSymbol == null)
			{
				return null;
			}
			if ((object)scope != null && !TypeSymbol.Equals(typeSymbol, scope, TypeCompareKind.ConsiderEverything))
			{
				TypeSymbol superType = typeSymbol;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				if (!TypeSymbolExtensions.IsBaseTypeOrInterfaceOf(superType, scope, ref useSiteInfo))
				{
					return null;
				}
			}
			if (!typeSymbol.IsTupleCompatible())
			{
				typeSymbol = TupleTypeDecoder.DecodeTupleTypesIfApplicable(typeSymbol, default(ImmutableArray<string>));
			}
			return new MemberRefMetadataDecoder(ModuleSymbol, typeSymbol).FindMember(typeSymbol, memberRef, methodsOnly);
		}

		protected override void EnqueueTypeSymbolInterfacesAndBaseTypes(Queue<TypeDefinitionHandle> typeDefsToSearch, Queue<TypeSymbol> typeSymbolsToSearch, TypeSymbol typeSymbol)
		{
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = typeSymbol.InterfacesNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamedTypeSymbol current = enumerator.Current;
				EnqueueTypeSymbol(typeDefsToSearch, typeSymbolsToSearch, current);
			}
			EnqueueTypeSymbol(typeDefsToSearch, typeSymbolsToSearch, typeSymbol.BaseTypeNoUseSiteDiagnostics);
		}

		protected override void EnqueueTypeSymbol(Queue<TypeDefinitionHandle> typeDefsToSearch, Queue<TypeSymbol> typeSymbolsToSearch, TypeSymbol typeSymbol)
		{
			if ((object)typeSymbol != null)
			{
				if (typeSymbol is PENamedTypeSymbol pENamedTypeSymbol && (object)pENamedTypeSymbol.ContainingPEModule == ModuleSymbol)
				{
					typeDefsToSearch.Enqueue(pENamedTypeSymbol.Handle);
				}
				else
				{
					typeSymbolsToSearch.Enqueue(typeSymbol);
				}
			}
		}

		protected override MethodDefinitionHandle GetMethodHandle(MethodSymbol method)
		{
			if (method is PEMethodSymbol pEMethodSymbol && (object)pEMethodSymbol.ContainingModule == ModuleSymbol)
			{
				return pEMethodSymbol.Handle;
			}
			return default(MethodDefinitionHandle);
		}
	}
}

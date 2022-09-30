using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal sealed class RetargetingNamespaceSymbol : NamespaceSymbol
	{
		private readonly RetargetingModuleSymbol _retargetingModule;

		private readonly NamespaceSymbol _underlyingNamespace;

		private RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => _retargetingModule.RetargetingTranslator;

		public NamespaceSymbol UnderlyingNamespace => _underlyingNamespace;

		internal override NamespaceExtent Extent => new NamespaceExtent(_retargetingModule);

		public override Symbol ContainingSymbol => RetargetingTranslator.Retarget(_underlyingNamespace.ContainingSymbol);

		public override ImmutableArray<Location> Locations => _underlyingNamespace.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingNamespace.DeclaringSyntaxReferences;

		public override AssemblySymbol ContainingAssembly => _retargetingModule.ContainingAssembly;

		public override ModuleSymbol ContainingModule => _retargetingModule;

		public override bool IsGlobalNamespace => _underlyingNamespace.IsGlobalNamespace;

		public override string Name => _underlyingNamespace.Name;

		public override string MetadataName => _underlyingNamespace.MetadataName;

		internal override Accessibility DeclaredAccessibilityOfMostAccessibleDescendantType => _underlyingNamespace.DeclaredAccessibilityOfMostAccessibleDescendantType;

		internal override ImmutableArray<NamedTypeSymbol> TypesToCheckForExtensionMethods
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override VisualBasicCompilation DeclaringCompilation => null;

		public RetargetingNamespaceSymbol(RetargetingModuleSymbol retargetingModule, NamespaceSymbol underlyingNamespace)
		{
			if (underlyingNamespace is RetargetingNamespaceSymbol)
			{
				throw new ArgumentException();
			}
			_retargetingModule = retargetingModule;
			_underlyingNamespace = underlyingNamespace;
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			return RetargetMembers(_underlyingNamespace.GetMembers());
		}

		private ImmutableArray<Symbol> RetargetMembers(ImmutableArray<Symbol> underlyingMembers)
		{
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			ImmutableArray<Symbol>.Enumerator enumerator = underlyingMembers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind != SymbolKind.NamedType || !((NamedTypeSymbol)current).IsExplicitDefinitionOfNoPiaLocalType)
				{
					instance.Add(RetargetingTranslator.Retarget(current));
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal override ImmutableArray<Symbol> GetMembersUnordered()
		{
			return RetargetMembers(_underlyingNamespace.GetMembersUnordered());
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			return RetargetMembers(_underlyingNamespace.GetMembers(name));
		}

		internal override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
		{
			return RetargetTypeMembers(_underlyingNamespace.GetTypeMembersUnordered());
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return RetargetTypeMembers(_underlyingNamespace.GetTypeMembers());
		}

		private ImmutableArray<NamedTypeSymbol> RetargetTypeMembers(ImmutableArray<NamedTypeSymbol> underlyingMembers)
		{
			ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = underlyingMembers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamedTypeSymbol current = enumerator.Current;
				if (!current.IsExplicitDefinitionOfNoPiaLocalType)
				{
					instance.Add(RetargetingTranslator.Retarget(current, RetargetOptions.RetargetPrimitiveTypesByName));
				}
			}
			return instance.ToImmutableAndFree();
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			return RetargetTypeMembers(_underlyingNamespace.GetTypeMembers(name));
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return RetargetTypeMembers(_underlyingNamespace.GetTypeMembers(name, arity));
		}

		internal override NamedTypeSymbol LookupMetadataType(ref MetadataTypeName fullEmittedName)
		{
			NamedTypeSymbol namedTypeSymbol = _underlyingNamespace.LookupMetadataType(ref fullEmittedName);
			if (!TypeSymbolExtensions.IsErrorType(namedTypeSymbol) && namedTypeSymbol.IsExplicitDefinitionOfNoPiaLocalType)
			{
				return new MissingMetadataTypeSymbol.TopLevel(_retargetingModule, ref fullEmittedName);
			}
			return RetargetingTranslator.Retarget(namedTypeSymbol, RetargetOptions.RetargetPrimitiveTypesByName);
		}

		public override ImmutableArray<NamedTypeSymbol> GetModuleMembers()
		{
			return RetargetingTranslator.Retarget(_underlyingNamespace.GetModuleMembers());
		}

		public override ImmutableArray<NamedTypeSymbol> GetModuleMembers(string name)
		{
			return RetargetingTranslator.Retarget(_underlyingNamespace.GetModuleMembers(name));
		}

		protected override Accessibility GetDeclaredAccessibilityOfMostAccessibleDescendantType()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override void AppendProbableExtensionMethods(string name, ArrayBuilder<MethodSymbol> methods)
		{
			int count = methods.Count;
			_underlyingNamespace.AppendProbableExtensionMethods(name, methods);
			int num = methods.Count - 1;
			for (int i = count; i <= num; i++)
			{
				methods[i] = RetargetingTranslator.Retarget(methods[i]);
			}
		}

		internal override void BuildExtensionMethodsMap(Dictionary<string, ArrayBuilder<MethodSymbol>> map)
		{
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = _underlyingNamespace.TypesToCheckForExtensionMethods.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.BuildExtensionMethodsMap(map, this);
			}
		}

		internal override void GetExtensionMethods(ArrayBuilder<MethodSymbol> methods, string name)
		{
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = _underlyingNamespace.TypesToCheckForExtensionMethods.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.GetExtensionMethods(methods, this, name);
			}
		}

		internal override void BuildExtensionMethodsMapBucket(ArrayBuilder<MethodSymbol> bucket, MethodSymbol method)
		{
			bucket.Add(RetargetingTranslator.Retarget(method));
		}

		internal override void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			_underlyingNamespace.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, this);
		}

		internal override bool AddExtensionMethodLookupSymbolsInfoViabilityCheck(MethodSymbol method, LookupOptions options, LookupSymbolsInfo nameSet, Binder originalBinder)
		{
			return base.AddExtensionMethodLookupSymbolsInfoViabilityCheck(RetargetingTranslator.Retarget(method), options, nameSet, originalBinder);
		}

		internal override void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder, NamespaceSymbol appendThrough)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingNamespace.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}

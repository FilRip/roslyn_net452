using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class PEAttributeData : VisualBasicAttributeData
	{
		private readonly MetadataDecoder _decoder;

		private readonly CustomAttributeHandle _handle;

		private NamedTypeSymbol _attributeClass;

		private MethodSymbol _attributeConstructor;

		private TypedConstant[] _lazyConstructorArguments;

		private KeyValuePair<string, TypedConstant>[] _lazyNamedArguments;

		private ThreeState _lazyHasErrors;

		public override NamedTypeSymbol AttributeClass
		{
			get
			{
				if ((object)_attributeClass == null)
				{
					EnsureClassAndConstructorSymbols();
				}
				return _attributeClass;
			}
		}

		public override MethodSymbol AttributeConstructor
		{
			get
			{
				if ((object)_attributeConstructor == null)
				{
					EnsureClassAndConstructorSymbols();
				}
				return _attributeConstructor;
			}
		}

		public override SyntaxReference ApplicationSyntaxReference => null;

		protected internal override ImmutableArray<TypedConstant> CommonConstructorArguments
		{
			protected get
			{
				if (_lazyConstructorArguments == null)
				{
					EnsureLazyMembersAreLoaded();
				}
				return _lazyConstructorArguments.AsImmutableOrNull();
			}
		}

		protected internal override ImmutableArray<KeyValuePair<string, TypedConstant>> CommonNamedArguments
		{
			protected get
			{
				if (_lazyNamedArguments == null)
				{
					EnsureLazyMembersAreLoaded();
				}
				return _lazyNamedArguments.AsImmutableOrNull();
			}
		}

		internal override bool HasErrors
		{
			get
			{
				if (_lazyHasErrors == ThreeState.Unknown)
				{
					EnsureClassAndConstructorSymbols();
					EnsureLazyMembersAreLoaded();
					if (_lazyHasErrors == ThreeState.Unknown)
					{
						_lazyHasErrors = ThreeState.False;
					}
				}
				return _lazyHasErrors.Value();
			}
		}

		internal PEAttributeData(PEModuleSymbol moduleSymbol, CustomAttributeHandle handle)
		{
			_lazyHasErrors = ThreeState.Unknown;
			_decoder = new MetadataDecoder(moduleSymbol);
			_handle = handle;
		}

		internal override bool IsTargetAttribute(string namespaceName, string typeName, bool ignoreCase = false)
		{
			return _decoder.IsTargetAttribute(_handle, namespaceName, typeName, ignoreCase);
		}

		internal override int GetTargetAttributeSignatureIndex(Symbol targetSymbol, AttributeDescription description)
		{
			return _decoder.GetTargetAttributeSignatureIndex(_handle, description);
		}

		private void EnsureLazyMembersAreLoaded()
		{
			if (_lazyConstructorArguments == null)
			{
				TypedConstant[] positionalArgs = null;
				KeyValuePair<string, TypedConstant>[] namedArgs = null;
				if (!_decoder.GetCustomAttribute(_handle, out positionalArgs, out namedArgs))
				{
					_lazyHasErrors = ThreeState.True;
				}
				Interlocked.CompareExchange(ref _lazyNamedArguments, namedArgs, null);
				Interlocked.CompareExchange(ref _lazyConstructorArguments, positionalArgs, null);
			}
		}

		private void EnsureClassAndConstructorSymbols()
		{
			if ((object)_attributeClass != null)
			{
				return;
			}
			TypeSymbol attributeClass = null;
			MethodSymbol attributeCtor = null;
			if (!_decoder.GetCustomAttribute(_handle, out attributeClass, out attributeCtor) || (object)attributeClass == null)
			{
				Interlocked.CompareExchange(ref _attributeClass, ErrorTypeSymbol.UnknownResultType, null);
				_lazyHasErrors = ThreeState.True;
				return;
			}
			if (TypeSymbolExtensions.IsErrorType(attributeClass) || (object)attributeCtor == null)
			{
				_lazyHasErrors = ThreeState.True;
			}
			Interlocked.CompareExchange(ref _attributeConstructor, attributeCtor, null);
			Interlocked.CompareExchange(ref _attributeClass, (NamedTypeSymbol)attributeClass, null);
		}
	}
}

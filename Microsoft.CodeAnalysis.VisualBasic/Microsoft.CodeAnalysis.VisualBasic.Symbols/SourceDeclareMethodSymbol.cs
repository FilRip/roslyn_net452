using System.Collections.Immutable;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourceDeclareMethodSymbol : SourceNonPropertyAccessorMethodSymbol
	{
		private readonly string _name;

		private string _lazyMetadataName;

		private readonly QuickAttributes _quickAttributes;

		private readonly DllImportData _platformInvokeInfo;

		public override string Name => _name;

		public override string MetadataName
		{
			get
			{
				if (_lazyMetadataName == null)
				{
					OverloadingHelper.SetMetadataNameForAllOverloads(_name, SymbolKind.Method, m_containingType);
				}
				return _lazyMetadataName;
			}
		}

		public override bool IsExternalMethod => true;

		internal override bool MayBeReducibleExtensionMethod => (_quickAttributes & QuickAttributes.Extension) != 0;

		public override bool IsExtensionMethod
		{
			get
			{
				if (MayBeReducibleExtensionMethod)
				{
					return base.IsExtensionMethod;
				}
				return false;
			}
		}

		internal override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				if ((_quickAttributes & QuickAttributes.Obsolete) != 0)
				{
					return base.ObsoleteAttributeData;
				}
				return null;
			}
		}

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.PreserveSig;

		public SourceDeclareMethodSymbol(SourceMemberContainerTypeSymbol container, string name, SourceMemberFlags flags, Binder binder, MethodBaseSyntax syntax, DllImportData platformInvokeInfo)
			: base(container, flags, binder.GetSyntaxReference(syntax))
		{
			_platformInvokeInfo = platformInvokeInfo;
			_name = name;
			_quickAttributes = binder.QuickAttributeChecker.CheckAttributes(syntax.AttributeLists);
			if (ContainingType.TypeKind != TypeKind.Module)
			{
				_quickAttributes &= ~QuickAttributes.Extension;
			}
		}

		internal override void SetMetadataName(string metadataName)
		{
			Interlocked.CompareExchange(ref _lazyMetadataName, metadataName, null);
		}

		public override DllImportData GetDllImportData()
		{
			return _platformInvokeInfo;
		}
	}
}

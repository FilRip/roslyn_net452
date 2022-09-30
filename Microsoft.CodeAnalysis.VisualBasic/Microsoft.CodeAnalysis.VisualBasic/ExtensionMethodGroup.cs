using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ExtensionMethodGroup
	{
		private readonly Binder _lookupBinder;

		private readonly LookupOptions _lookupOptions;

		private readonly bool _withDependencies;

		private ImmutableArray<MethodSymbol> _lazyMethods;

		private IReadOnlyCollection<DiagnosticInfo> _lazyUseSiteDiagnostics;

		private IReadOnlyCollection<AssemblySymbol> _lazyUseSiteDependencies;

		public ExtensionMethodGroup(Binder lookupBinder, LookupOptions lookupOptions, bool withDependencies)
		{
			_lookupBinder = lookupBinder;
			_lookupOptions = lookupOptions;
			_withDependencies = withDependencies;
		}

		public ImmutableArray<MethodSymbol> LazyLookupAdditionalExtensionMethods(BoundMethodGroup group, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (_lazyMethods.IsDefault)
			{
				BoundExpression receiverOpt = group.ReceiverOpt;
				ImmutableArray<MethodSymbol> value = ImmutableArray<MethodSymbol>.Empty;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = (_withDependencies ? new CompoundUseSiteInfo<AssemblySymbol>(_lookupBinder.Compilation.Assembly) : CompoundUseSiteInfo<AssemblySymbol>.DiscardedDependencies);
				if (receiverOpt != null && (object)receiverOpt.Type != null)
				{
					LookupResult instance = LookupResult.GetInstance();
					_lookupBinder.LookupExtensionMethods(instance, receiverOpt.Type, group.Methods[0].Name, (group.TypeArgumentsOpt != null) ? group.TypeArgumentsOpt.Arguments.Length : 0, _lookupOptions, ref useSiteInfo2);
					if (instance.IsGood)
					{
						value = instance.Symbols.ToDowncastedImmutable<MethodSymbol>();
					}
					instance.Free();
				}
				Interlocked.CompareExchange(ref _lazyUseSiteDiagnostics, useSiteInfo2.Diagnostics, null);
				Interlocked.CompareExchange(ref _lazyUseSiteDependencies, useSiteInfo2.AccumulatesDependencies ? useSiteInfo2.Dependencies : null, null);
				ImmutableInterlocked.InterlockedCompareExchange(ref _lazyMethods, value, default(ImmutableArray<MethodSymbol>));
			}
			useSiteInfo.AddDiagnostics(Volatile.Read(ref _lazyUseSiteDiagnostics));
			useSiteInfo.AddDependencies(Volatile.Read(ref _lazyUseSiteDependencies));
			return _lazyMethods;
		}
	}
}

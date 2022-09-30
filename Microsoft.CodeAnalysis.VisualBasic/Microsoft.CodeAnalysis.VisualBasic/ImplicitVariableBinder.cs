using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ImplicitVariableBinder : Binder
	{
		private struct ShadowedVariableInfo
		{
			public readonly string Name;

			public readonly Location Location;

			public readonly ERRID ErrorId;

			public ShadowedVariableInfo(string name, Location location, ERRID errorId)
			{
				this = default(ShadowedVariableInfo);
				Name = name;
				Location = location;
				ErrorId = errorId;
			}
		}

		private readonly Symbol _containerOfLocals;

		private bool _frozen;

		private Dictionary<string, LocalSymbol> _implicitLocals;

		private MultiDictionary<string, ShadowedVariableInfo> _possiblyShadowingVariables;

		public override bool AllImplicitVariableDeclarationsAreHandled => _frozen;

		public override bool ImplicitVariableDeclarationAllowed => true;

		public override ImmutableArray<LocalSymbol> ImplicitlyDeclaredVariables
		{
			get
			{
				if (_implicitLocals == null)
				{
					return ImmutableArray<LocalSymbol>.Empty;
				}
				ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
				instance.AddRange(_implicitLocals.Values);
				return instance.ToImmutableAndFree();
			}
		}

		public ImplicitVariableBinder(Binder containingBinder, Symbol containerOfLocals)
			: base(containingBinder)
		{
			_containerOfLocals = containerOfLocals;
			_frozen = false;
		}

		internal override BoundExpression BindGroupAggregationExpression(GroupAggregationSyntax group, BindingDiagnosticBag diagnostics)
		{
			return base.ContainingBinder.BindGroupAggregationExpression(group, diagnostics);
		}

		internal override BoundExpression BindFunctionAggregationExpression(FunctionAggregationSyntax function, BindingDiagnosticBag diagnostics)
		{
			return base.ContainingBinder.BindFunctionAggregationExpression(function, diagnostics);
		}

		public override void DisallowFurtherImplicitVariableDeclaration(BindingDiagnosticBag diagnostics)
		{
			if (_frozen)
			{
				return;
			}
			_frozen = true;
			if (_implicitLocals == null || _possiblyShadowingVariables == null)
			{
				return;
			}
			foreach (string key in _implicitLocals.Keys)
			{
				foreach (ShadowedVariableInfo item in _possiblyShadowingVariables[key])
				{
					Binder.ReportDiagnostic(diagnostics, item.Location, item.ErrorId, item.Name);
				}
			}
		}

		public override LocalSymbol DeclareImplicitLocalVariable(IdentifierNameSyntax nameSyntax, BindingDiagnosticBag diagnostics)
		{
			SpecialType typeId = SpecialType.System_Object;
			if (VisualBasicExtensions.GetTypeCharacter(nameSyntax.Identifier) != 0)
			{
				string typeCharacterString = null;
				typeId = Binder.GetSpecialTypeForTypeCharacter(VisualBasicExtensions.GetTypeCharacter(nameSyntax.Identifier), ref typeCharacterString);
			}
			LocalSymbol localSymbol = LocalSymbol.Create(_containerOfLocals, this, nameSyntax.Identifier, LocalDeclarationKind.ImplicitVariable, GetSpecialType(typeId, nameSyntax, diagnostics));
			if (_implicitLocals == null)
			{
				_implicitLocals = new Dictionary<string, LocalSymbol>(CaseInsensitiveComparison.Comparer);
			}
			_implicitLocals.Add(nameSyntax.Identifier.ValueText, localSymbol);
			return localSymbol;
		}

		public void RememberPossibleShadowingVariable(string name, SyntaxNodeOrToken syntax, ERRID errorId)
		{
			if (_possiblyShadowingVariables == null)
			{
				_possiblyShadowingVariables = new MultiDictionary<string, ShadowedVariableInfo>(CaseInsensitiveComparison.Comparer);
			}
			_possiblyShadowingVariables.Add(name, new ShadowedVariableInfo(name, syntax.GetLocation(), errorId));
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			LocalSymbol value = null;
			if (_implicitLocals != null && (options & (LookupOptions.NamespacesOrTypesOnly | LookupOptions.LabelsOnly)) == 0 && _implicitLocals.TryGetValue(name, out value))
			{
				lookupResult.SetFrom(CheckViability(value, arity, options, null, ref useSiteInfo));
			}
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			if (_implicitLocals == null || (options & (LookupOptions.NamespacesOrTypesOnly | LookupOptions.LabelsOnly)) != 0)
			{
				return;
			}
			foreach (LocalSymbol value in _implicitLocals.Values)
			{
				if (originalBinder.CanAddLookupSymbolInfo(value, options, nameSet, null))
				{
					nameSet.AddSymbol(value, value.Name, 0);
				}
			}
		}
	}
}

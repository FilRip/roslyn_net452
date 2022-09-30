using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class LambdaBodyBinder : SubOrFunctionBodyBinder
	{
		private readonly LocalSymbol _functionValue;

		public LambdaBodyBinder(LambdaSymbol lambdaSymbol, Binder containingBinder)
			: base(lambdaSymbol, lambdaSymbol.Syntax, containingBinder)
		{
			_functionValue = CreateFunctionValueLocal(lambdaSymbol);
		}

		private static LocalSymbol CreateFunctionValueLocal(LambdaSymbol lambdaSymbol)
		{
			if (lambdaSymbol.IsImplicitlyDeclared || lambdaSymbol.IsSub)
			{
				return null;
			}
			LambdaHeaderSyntax subOrFunctionHeader = ((LambdaExpressionSyntax)lambdaSymbol.Syntax).SubOrFunctionHeader;
			return new SynthesizedLocal(lambdaSymbol, lambdaSymbol.ReturnType, SynthesizedLocalKind.FunctionReturnValue, subOrFunctionHeader);
		}

		public override LocalSymbol GetLocalForFunctionValue()
		{
			return _functionValue;
		}

		public override LabelSymbol GetContinueLabel(SyntaxKind continueSyntaxKind)
		{
			return null;
		}

		public override LabelSymbol GetExitLabel(SyntaxKind exitSyntaxKind)
		{
			return null;
		}

		public override LabelSymbol GetReturnLabel()
		{
			return null;
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			base.LookupInSingleBinder(lookupResult, name, arity, options, originalBinder, ref useSiteInfo);
			if ((options & LookupOptions.LabelsOnly) == LookupOptions.LabelsOnly && lookupResult.Kind == LookupResultKind.Empty)
			{
				lookupResult.SetFrom(SingleLookupResult.EmptyAndStopLookup);
			}
		}
	}
}

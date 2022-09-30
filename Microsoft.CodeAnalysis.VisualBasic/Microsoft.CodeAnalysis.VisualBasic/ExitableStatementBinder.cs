using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ExitableStatementBinder : BlockBaseBinder
	{
		private readonly LabelSymbol _continueLabel;

		private readonly SyntaxKind _continueKind;

		private readonly LabelSymbol _exitLabel;

		private readonly SyntaxKind _exitKind;

		internal override ImmutableArray<LocalSymbol> Locals => ImmutableArray<LocalSymbol>.Empty;

		public ExitableStatementBinder(Binder enclosing, SyntaxKind continueKind, SyntaxKind exitKind)
			: base(enclosing)
		{
			_continueKind = continueKind;
			if (continueKind != 0)
			{
				_continueLabel = new GeneratedLabelSymbol("continue");
			}
			_exitKind = exitKind;
			if (exitKind != 0)
			{
				_exitLabel = new GeneratedLabelSymbol("exit");
			}
		}

		public override LabelSymbol GetContinueLabel(SyntaxKind continueSyntaxKind)
		{
			if (_continueKind == continueSyntaxKind)
			{
				return _continueLabel;
			}
			return base.ContainingBinder.GetContinueLabel(continueSyntaxKind);
		}

		public override LabelSymbol GetExitLabel(SyntaxKind exitSyntaxKind)
		{
			if (_exitKind == exitSyntaxKind)
			{
				return _exitLabel;
			}
			return base.ContainingBinder.GetExitLabel(exitSyntaxKind);
		}

		public override LabelSymbol GetReturnLabel()
		{
			SyntaxKind exitKind = _exitKind;
			if (exitKind - 102 <= SyntaxKind.List || exitKind - 159 <= SyntaxKind.List || exitKind == SyntaxKind.ExitPropertyStatement)
			{
				return _exitLabel;
			}
			return base.ContainingBinder.GetReturnLabel();
		}
	}
}

using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundInstrumentationPayloadRoot : BoundExpression
	{
		private readonly int _AnalysisKind;

		private readonly bool _IsLValue;

		public int AnalysisKind => _AnalysisKind;

		public override bool IsLValue => _IsLValue;

		public BoundInstrumentationPayloadRoot(SyntaxNode syntax, int analysisKind, bool isLValue, TypeSymbol type, bool hasErrors)
			: base(BoundKind.InstrumentationPayloadRoot, syntax, type, hasErrors)
		{
			_AnalysisKind = analysisKind;
			_IsLValue = isLValue;
		}

		public BoundInstrumentationPayloadRoot(SyntaxNode syntax, int analysisKind, bool isLValue, TypeSymbol type)
			: base(BoundKind.InstrumentationPayloadRoot, syntax, type)
		{
			_AnalysisKind = analysisKind;
			_IsLValue = isLValue;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitInstrumentationPayloadRoot(this);
		}

		public BoundInstrumentationPayloadRoot Update(int analysisKind, bool isLValue, TypeSymbol type)
		{
			if (analysisKind != AnalysisKind || isLValue != IsLValue || (object)type != base.Type)
			{
				BoundInstrumentationPayloadRoot boundInstrumentationPayloadRoot = new BoundInstrumentationPayloadRoot(base.Syntax, analysisKind, isLValue, type, base.HasErrors);
				boundInstrumentationPayloadRoot.CopyAttributes(this);
				return boundInstrumentationPayloadRoot;
			}
			return this;
		}
	}
}

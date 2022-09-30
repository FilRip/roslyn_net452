using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class BadCConst : CConst
	{
		public override SpecialType SpecialType => SpecialType.None;

		public override object ValueAsObject => null;

		public BadCConst(ERRID id)
			: base(id)
		{
		}

		public BadCConst(ERRID id, params object[] args)
			: base(id, args)
		{
		}

		public override CConst WithError(ERRID id)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}

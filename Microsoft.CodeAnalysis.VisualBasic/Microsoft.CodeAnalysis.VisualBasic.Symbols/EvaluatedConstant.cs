namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class EvaluatedConstant
	{
		public static readonly EvaluatedConstant None = new EvaluatedConstant(null, null);

		public readonly ConstantValue Value;

		public readonly TypeSymbol Type;

		public EvaluatedConstant(ConstantValue value, TypeSymbol type)
		{
			Value = value;
			Type = type;
		}
	}
}

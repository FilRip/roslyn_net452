using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class LambdaExpressionSyntax : ExpressionSyntax
	{
		internal readonly LambdaHeaderSyntax _subOrFunctionHeader;

		internal LambdaHeaderSyntax SubOrFunctionHeader => _subOrFunctionHeader;

		internal LambdaExpressionSyntax(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader)
			: base(kind)
		{
			AdjustFlagsAndWidth(subOrFunctionHeader);
			_subOrFunctionHeader = subOrFunctionHeader;
		}

		internal LambdaExpressionSyntax(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			AdjustFlagsAndWidth(subOrFunctionHeader);
			_subOrFunctionHeader = subOrFunctionHeader;
		}

		internal LambdaExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, LambdaHeaderSyntax subOrFunctionHeader)
			: base(kind, errors, annotations)
		{
			AdjustFlagsAndWidth(subOrFunctionHeader);
			_subOrFunctionHeader = subOrFunctionHeader;
		}

		internal LambdaExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			LambdaHeaderSyntax lambdaHeaderSyntax = (LambdaHeaderSyntax)reader.ReadValue();
			if (lambdaHeaderSyntax != null)
			{
				AdjustFlagsAndWidth(lambdaHeaderSyntax);
				_subOrFunctionHeader = lambdaHeaderSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_subOrFunctionHeader);
		}
	}
}

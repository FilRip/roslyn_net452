using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class LiteralExpressionSyntax : ExpressionSyntax
	{
		internal readonly SyntaxToken _token;

		internal static Func<ObjectReader, object> CreateInstance;

		internal SyntaxToken Token => _token;

		internal LiteralExpressionSyntax(SyntaxKind kind, SyntaxToken token)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(token);
			_token = token;
		}

		internal LiteralExpressionSyntax(SyntaxKind kind, SyntaxToken token, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(token);
			_token = token;
		}

		internal LiteralExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxToken token)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(token);
			_token = token;
		}

		internal LiteralExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
			if (syntaxToken != null)
			{
				AdjustFlagsAndWidth(syntaxToken);
				_token = syntaxToken;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_token);
		}

		static LiteralExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new LiteralExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(LiteralExpressionSyntax), (ObjectReader r) => new LiteralExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _token;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new LiteralExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _token);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new LiteralExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _token);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitLiteralExpression(this);
		}
	}
}

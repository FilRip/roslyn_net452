using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SimpleCaseClauseSyntax : CaseClauseSyntax
	{
		internal readonly ExpressionSyntax _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Value => _value;

		internal SimpleCaseClauseSyntax(SyntaxKind kind, ExpressionSyntax value)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal SimpleCaseClauseSyntax(SyntaxKind kind, ExpressionSyntax value, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal SimpleCaseClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax value)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal SimpleCaseClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_value = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_value);
		}

		static SimpleCaseClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new SimpleCaseClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SimpleCaseClauseSyntax), (ObjectReader r) => new SimpleCaseClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleCaseClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _value;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SimpleCaseClauseSyntax(base.Kind, newErrors, GetAnnotations(), _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SimpleCaseClauseSyntax(base.Kind, GetDiagnostics(), annotations, _value);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSimpleCaseClause(this);
		}
	}
}

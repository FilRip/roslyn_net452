using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SimpleArgumentSyntax : ArgumentSyntax
	{
		internal readonly NameColonEqualsSyntax _nameColonEquals;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal NameColonEqualsSyntax NameColonEquals => _nameColonEquals;

		internal ExpressionSyntax Expression => _expression;

		internal SimpleArgumentSyntax(SyntaxKind kind, NameColonEqualsSyntax nameColonEquals, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 2;
			if (nameColonEquals != null)
			{
				AdjustFlagsAndWidth(nameColonEquals);
				_nameColonEquals = nameColonEquals;
			}
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal SimpleArgumentSyntax(SyntaxKind kind, NameColonEqualsSyntax nameColonEquals, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			if (nameColonEquals != null)
			{
				AdjustFlagsAndWidth(nameColonEquals);
				_nameColonEquals = nameColonEquals;
			}
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal SimpleArgumentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, NameColonEqualsSyntax nameColonEquals, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			if (nameColonEquals != null)
			{
				AdjustFlagsAndWidth(nameColonEquals);
				_nameColonEquals = nameColonEquals;
			}
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal SimpleArgumentSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			NameColonEqualsSyntax nameColonEqualsSyntax = (NameColonEqualsSyntax)reader.ReadValue();
			if (nameColonEqualsSyntax != null)
			{
				AdjustFlagsAndWidth(nameColonEqualsSyntax);
				_nameColonEquals = nameColonEqualsSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_nameColonEquals);
			writer.WriteValue(_expression);
		}

		static SimpleArgumentSyntax()
		{
			CreateInstance = (ObjectReader o) => new SimpleArgumentSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SimpleArgumentSyntax), (ObjectReader r) => new SimpleArgumentSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleArgumentSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _nameColonEquals, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SimpleArgumentSyntax(base.Kind, newErrors, GetAnnotations(), _nameColonEquals, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SimpleArgumentSyntax(base.Kind, GetDiagnostics(), annotations, _nameColonEquals, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSimpleArgument(this);
		}
	}
}

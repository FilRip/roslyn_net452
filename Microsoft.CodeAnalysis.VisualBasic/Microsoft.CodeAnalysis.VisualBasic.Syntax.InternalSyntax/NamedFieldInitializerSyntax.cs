using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class NamedFieldInitializerSyntax : FieldInitializerSyntax
	{
		internal readonly PunctuationSyntax _dotToken;

		internal readonly IdentifierNameSyntax _name;

		internal readonly PunctuationSyntax _equalsToken;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax DotToken => _dotToken;

		internal IdentifierNameSyntax Name => _name;

		internal PunctuationSyntax EqualsToken => _equalsToken;

		internal ExpressionSyntax Expression => _expression;

		internal NamedFieldInitializerSyntax(SyntaxKind kind, KeywordSyntax keyKeyword, PunctuationSyntax dotToken, IdentifierNameSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax expression)
			: base(kind, keyKeyword)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal NamedFieldInitializerSyntax(SyntaxKind kind, KeywordSyntax keyKeyword, PunctuationSyntax dotToken, IdentifierNameSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind, keyKeyword)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal NamedFieldInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyKeyword, PunctuationSyntax dotToken, IdentifierNameSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax expression)
			: base(kind, errors, annotations, keyKeyword)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal NamedFieldInitializerSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_dotToken = punctuationSyntax;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)reader.ReadValue();
			if (identifierNameSyntax != null)
			{
				AdjustFlagsAndWidth(identifierNameSyntax);
				_name = identifierNameSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_equalsToken = punctuationSyntax2;
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
			writer.WriteValue(_dotToken);
			writer.WriteValue(_name);
			writer.WriteValue(_equalsToken);
			writer.WriteValue(_expression);
		}

		static NamedFieldInitializerSyntax()
		{
			CreateInstance = (ObjectReader o) => new NamedFieldInitializerSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(NamedFieldInitializerSyntax), (ObjectReader r) => new NamedFieldInitializerSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedFieldInitializerSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _keyKeyword, 
				1 => _dotToken, 
				2 => _name, 
				3 => _equalsToken, 
				4 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new NamedFieldInitializerSyntax(base.Kind, newErrors, GetAnnotations(), _keyKeyword, _dotToken, _name, _equalsToken, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new NamedFieldInitializerSyntax(base.Kind, GetDiagnostics(), annotations, _keyKeyword, _dotToken, _name, _equalsToken, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitNamedFieldInitializer(this);
		}
	}
}

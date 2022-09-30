using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlMemberAccessExpressionSyntax : ExpressionSyntax
	{
		internal readonly ExpressionSyntax _base;

		internal readonly PunctuationSyntax _token1;

		internal readonly PunctuationSyntax _token2;

		internal readonly PunctuationSyntax _token3;

		internal readonly XmlNodeSyntax _name;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Base => _base;

		internal PunctuationSyntax Token1 => _token1;

		internal PunctuationSyntax Token2 => _token2;

		internal PunctuationSyntax Token3 => _token3;

		internal XmlNodeSyntax Name => _name;

		internal XmlMemberAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
			: base(kind)
		{
			base._slotCount = 5;
			if (@base != null)
			{
				AdjustFlagsAndWidth(@base);
				_base = @base;
			}
			AdjustFlagsAndWidth(token1);
			_token1 = token1;
			if (token2 != null)
			{
				AdjustFlagsAndWidth(token2);
				_token2 = token2;
			}
			if (token3 != null)
			{
				AdjustFlagsAndWidth(token3);
				_token3 = token3;
			}
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal XmlMemberAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			if (@base != null)
			{
				AdjustFlagsAndWidth(@base);
				_base = @base;
			}
			AdjustFlagsAndWidth(token1);
			_token1 = token1;
			if (token2 != null)
			{
				AdjustFlagsAndWidth(token2);
				_token2 = token2;
			}
			if (token3 != null)
			{
				AdjustFlagsAndWidth(token3);
				_token3 = token3;
			}
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal XmlMemberAccessExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
			if (@base != null)
			{
				AdjustFlagsAndWidth(@base);
				_base = @base;
			}
			AdjustFlagsAndWidth(token1);
			_token1 = token1;
			if (token2 != null)
			{
				AdjustFlagsAndWidth(token2);
				_token2 = token2;
			}
			if (token3 != null)
			{
				AdjustFlagsAndWidth(token3);
				_token3 = token3;
			}
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal XmlMemberAccessExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_base = expressionSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_token1 = punctuationSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_token2 = punctuationSyntax2;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax3 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax3);
				_token3 = punctuationSyntax3;
			}
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)reader.ReadValue();
			if (xmlNodeSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNodeSyntax);
				_name = xmlNodeSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_base);
			writer.WriteValue(_token1);
			writer.WriteValue(_token2);
			writer.WriteValue(_token3);
			writer.WriteValue(_name);
		}

		static XmlMemberAccessExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlMemberAccessExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlMemberAccessExpressionSyntax), (ObjectReader r) => new XmlMemberAccessExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _base, 
				1 => _token1, 
				2 => _token2, 
				3 => _token3, 
				4 => _name, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlMemberAccessExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _base, _token1, _token2, _token3, _name);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlMemberAccessExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _base, _token1, _token2, _token3, _name);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlMemberAccessExpression(this);
		}
	}
}

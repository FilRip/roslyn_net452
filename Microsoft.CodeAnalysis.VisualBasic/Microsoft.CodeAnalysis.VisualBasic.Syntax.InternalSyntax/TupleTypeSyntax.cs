using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TupleTypeSyntax : TypeSyntax
	{
		internal readonly PunctuationSyntax _openParenToken;

		internal readonly GreenNode _elements;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TupleElementSyntax> Elements => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TupleElementSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TupleElementSyntax>(_elements));

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal TupleTypeSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, GreenNode elements, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (elements != null)
			{
				AdjustFlagsAndWidth(elements);
				_elements = elements;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal TupleTypeSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, GreenNode elements, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (elements != null)
			{
				AdjustFlagsAndWidth(elements);
				_elements = elements;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal TupleTypeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, GreenNode elements, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (elements != null)
			{
				AdjustFlagsAndWidth(elements);
				_elements = elements;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal TupleTypeSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openParenToken = punctuationSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_elements = greenNode;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_closeParenToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_elements);
			writer.WriteValue(_closeParenToken);
		}

		static TupleTypeSyntax()
		{
			CreateInstance = (ObjectReader o) => new TupleTypeSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TupleTypeSyntax), (ObjectReader r) => new TupleTypeSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleTypeSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _openParenToken, 
				1 => _elements, 
				2 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new TupleTypeSyntax(base.Kind, newErrors, GetAnnotations(), _openParenToken, _elements, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TupleTypeSyntax(base.Kind, GetDiagnostics(), annotations, _openParenToken, _elements, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTupleType(this);
		}
	}
}

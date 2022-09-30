using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ArrayRankSpecifierSyntax : VisualBasicSyntaxNode
	{
		internal readonly PunctuationSyntax _openParenToken;

		internal readonly GreenNode _commaTokens;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<PunctuationSyntax> CommaTokens => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_commaTokens);

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal ArrayRankSpecifierSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, GreenNode commaTokens, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (commaTokens != null)
			{
				AdjustFlagsAndWidth(commaTokens);
				_commaTokens = commaTokens;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ArrayRankSpecifierSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, GreenNode commaTokens, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (commaTokens != null)
			{
				AdjustFlagsAndWidth(commaTokens);
				_commaTokens = commaTokens;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ArrayRankSpecifierSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, GreenNode commaTokens, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (commaTokens != null)
			{
				AdjustFlagsAndWidth(commaTokens);
				_commaTokens = commaTokens;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ArrayRankSpecifierSyntax(ObjectReader reader)
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
				_commaTokens = greenNode;
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
			writer.WriteValue(_commaTokens);
			writer.WriteValue(_closeParenToken);
		}

		static ArrayRankSpecifierSyntax()
		{
			CreateInstance = (ObjectReader o) => new ArrayRankSpecifierSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ArrayRankSpecifierSyntax), (ObjectReader r) => new ArrayRankSpecifierSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _openParenToken, 
				1 => _commaTokens, 
				2 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ArrayRankSpecifierSyntax(base.Kind, newErrors, GetAnnotations(), _openParenToken, _commaTokens, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ArrayRankSpecifierSyntax(base.Kind, GetDiagnostics(), annotations, _openParenToken, _commaTokens, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitArrayRankSpecifier(this);
		}
	}
}

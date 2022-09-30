using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CrefSignatureSyntax : VisualBasicSyntaxNode
	{
		internal readonly PunctuationSyntax _openParenToken;

		internal readonly GreenNode _argumentTypes;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CrefSignaturePartSyntax> ArgumentTypes => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CrefSignaturePartSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CrefSignaturePartSyntax>(_argumentTypes));

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal CrefSignatureSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, GreenNode argumentTypes, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (argumentTypes != null)
			{
				AdjustFlagsAndWidth(argumentTypes);
				_argumentTypes = argumentTypes;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal CrefSignatureSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, GreenNode argumentTypes, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (argumentTypes != null)
			{
				AdjustFlagsAndWidth(argumentTypes);
				_argumentTypes = argumentTypes;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal CrefSignatureSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, GreenNode argumentTypes, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (argumentTypes != null)
			{
				AdjustFlagsAndWidth(argumentTypes);
				_argumentTypes = argumentTypes;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal CrefSignatureSyntax(ObjectReader reader)
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
				_argumentTypes = greenNode;
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
			writer.WriteValue(_argumentTypes);
			writer.WriteValue(_closeParenToken);
		}

		static CrefSignatureSyntax()
		{
			CreateInstance = (ObjectReader o) => new CrefSignatureSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CrefSignatureSyntax), (ObjectReader r) => new CrefSignatureSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefSignatureSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _openParenToken, 
				1 => _argumentTypes, 
				2 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CrefSignatureSyntax(base.Kind, newErrors, GetAnnotations(), _openParenToken, _argumentTypes, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CrefSignatureSyntax(base.Kind, GetDiagnostics(), annotations, _openParenToken, _argumentTypes, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCrefSignature(this);
		}
	}
}

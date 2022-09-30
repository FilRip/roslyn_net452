using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ArgumentListSyntax : VisualBasicSyntaxNode
	{
		internal readonly PunctuationSyntax _openParenToken;

		internal readonly GreenNode _arguments;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> Arguments => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ArgumentSyntax>(_arguments));

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal ArgumentListSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, GreenNode arguments, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (arguments != null)
			{
				AdjustFlagsAndWidth(arguments);
				_arguments = arguments;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ArgumentListSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, GreenNode arguments, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (arguments != null)
			{
				AdjustFlagsAndWidth(arguments);
				_arguments = arguments;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ArgumentListSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, GreenNode arguments, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			if (arguments != null)
			{
				AdjustFlagsAndWidth(arguments);
				_arguments = arguments;
			}
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ArgumentListSyntax(ObjectReader reader)
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
				_arguments = greenNode;
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
			writer.WriteValue(_arguments);
			writer.WriteValue(_closeParenToken);
		}

		static ArgumentListSyntax()
		{
			CreateInstance = (ObjectReader o) => new ArgumentListSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ArgumentListSyntax), (ObjectReader r) => new ArgumentListSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _openParenToken, 
				1 => _arguments, 
				2 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ArgumentListSyntax(base.Kind, newErrors, GetAnnotations(), _openParenToken, _arguments, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ArgumentListSyntax(base.Kind, GetDiagnostics(), annotations, _openParenToken, _arguments, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitArgumentList(this);
		}
	}
}

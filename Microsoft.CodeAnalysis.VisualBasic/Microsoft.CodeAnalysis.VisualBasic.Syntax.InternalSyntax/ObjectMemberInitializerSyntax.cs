using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ObjectMemberInitializerSyntax : ObjectCreationInitializerSyntax
	{
		internal readonly KeywordSyntax _withKeyword;

		internal readonly PunctuationSyntax _openBraceToken;

		internal readonly GreenNode _initializers;

		internal readonly PunctuationSyntax _closeBraceToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax WithKeyword => _withKeyword;

		internal PunctuationSyntax OpenBraceToken => _openBraceToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<FieldInitializerSyntax> Initializers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<FieldInitializerSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<FieldInitializerSyntax>(_initializers));

		internal PunctuationSyntax CloseBraceToken => _closeBraceToken;

		internal ObjectMemberInitializerSyntax(SyntaxKind kind, KeywordSyntax withKeyword, PunctuationSyntax openBraceToken, GreenNode initializers, PunctuationSyntax closeBraceToken)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(withKeyword);
			_withKeyword = withKeyword;
			AdjustFlagsAndWidth(openBraceToken);
			_openBraceToken = openBraceToken;
			if (initializers != null)
			{
				AdjustFlagsAndWidth(initializers);
				_initializers = initializers;
			}
			AdjustFlagsAndWidth(closeBraceToken);
			_closeBraceToken = closeBraceToken;
		}

		internal ObjectMemberInitializerSyntax(SyntaxKind kind, KeywordSyntax withKeyword, PunctuationSyntax openBraceToken, GreenNode initializers, PunctuationSyntax closeBraceToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(withKeyword);
			_withKeyword = withKeyword;
			AdjustFlagsAndWidth(openBraceToken);
			_openBraceToken = openBraceToken;
			if (initializers != null)
			{
				AdjustFlagsAndWidth(initializers);
				_initializers = initializers;
			}
			AdjustFlagsAndWidth(closeBraceToken);
			_closeBraceToken = closeBraceToken;
		}

		internal ObjectMemberInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax withKeyword, PunctuationSyntax openBraceToken, GreenNode initializers, PunctuationSyntax closeBraceToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(withKeyword);
			_withKeyword = withKeyword;
			AdjustFlagsAndWidth(openBraceToken);
			_openBraceToken = openBraceToken;
			if (initializers != null)
			{
				AdjustFlagsAndWidth(initializers);
				_initializers = initializers;
			}
			AdjustFlagsAndWidth(closeBraceToken);
			_closeBraceToken = closeBraceToken;
		}

		internal ObjectMemberInitializerSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_withKeyword = keywordSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openBraceToken = punctuationSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_initializers = greenNode;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_closeBraceToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_withKeyword);
			writer.WriteValue(_openBraceToken);
			writer.WriteValue(_initializers);
			writer.WriteValue(_closeBraceToken);
		}

		static ObjectMemberInitializerSyntax()
		{
			CreateInstance = (ObjectReader o) => new ObjectMemberInitializerSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ObjectMemberInitializerSyntax), (ObjectReader r) => new ObjectMemberInitializerSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _withKeyword, 
				1 => _openBraceToken, 
				2 => _initializers, 
				3 => _closeBraceToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ObjectMemberInitializerSyntax(base.Kind, newErrors, GetAnnotations(), _withKeyword, _openBraceToken, _initializers, _closeBraceToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ObjectMemberInitializerSyntax(base.Kind, GetDiagnostics(), annotations, _withKeyword, _openBraceToken, _initializers, _closeBraceToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitObjectMemberInitializer(this);
		}
	}
}

using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CollectionInitializerSyntax : ExpressionSyntax
	{
		internal readonly PunctuationSyntax _openBraceToken;

		internal readonly GreenNode _initializers;

		internal readonly PunctuationSyntax _closeBraceToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax OpenBraceToken => _openBraceToken;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> Initializers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExpressionSyntax>(_initializers));

		internal PunctuationSyntax CloseBraceToken => _closeBraceToken;

		internal CollectionInitializerSyntax(SyntaxKind kind, PunctuationSyntax openBraceToken, GreenNode initializers, PunctuationSyntax closeBraceToken)
			: base(kind)
		{
			base._slotCount = 3;
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

		internal CollectionInitializerSyntax(SyntaxKind kind, PunctuationSyntax openBraceToken, GreenNode initializers, PunctuationSyntax closeBraceToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
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

		internal CollectionInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openBraceToken, GreenNode initializers, PunctuationSyntax closeBraceToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
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

		internal CollectionInitializerSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
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
			writer.WriteValue(_openBraceToken);
			writer.WriteValue(_initializers);
			writer.WriteValue(_closeBraceToken);
		}

		static CollectionInitializerSyntax()
		{
			CreateInstance = (ObjectReader o) => new CollectionInitializerSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CollectionInitializerSyntax), (ObjectReader r) => new CollectionInitializerSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _openBraceToken, 
				1 => _initializers, 
				2 => _closeBraceToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CollectionInitializerSyntax(base.Kind, newErrors, GetAnnotations(), _openBraceToken, _initializers, _closeBraceToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CollectionInitializerSyntax(base.Kind, GetDiagnostics(), annotations, _openBraceToken, _initializers, _closeBraceToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCollectionInitializer(this);
		}
	}
}

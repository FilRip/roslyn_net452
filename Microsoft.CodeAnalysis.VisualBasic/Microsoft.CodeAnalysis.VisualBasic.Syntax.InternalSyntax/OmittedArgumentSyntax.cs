using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class OmittedArgumentSyntax : ArgumentSyntax
	{
		internal readonly PunctuationSyntax _empty;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax Empty => _empty;

		internal OmittedArgumentSyntax(SyntaxKind kind, PunctuationSyntax empty)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(empty);
			_empty = empty;
		}

		internal OmittedArgumentSyntax(SyntaxKind kind, PunctuationSyntax empty, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(empty);
			_empty = empty;
		}

		internal OmittedArgumentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax empty)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(empty);
			_empty = empty;
		}

		internal OmittedArgumentSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_empty = punctuationSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_empty);
		}

		static OmittedArgumentSyntax()
		{
			CreateInstance = (ObjectReader o) => new OmittedArgumentSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(OmittedArgumentSyntax), (ObjectReader r) => new OmittedArgumentSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OmittedArgumentSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _empty;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new OmittedArgumentSyntax(base.Kind, newErrors, GetAnnotations(), _empty);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new OmittedArgumentSyntax(base.Kind, GetDiagnostics(), annotations, _empty);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitOmittedArgument(this);
		}
	}
}

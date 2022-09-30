using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class NameColonEqualsSyntax : VisualBasicSyntaxNode
	{
		internal readonly IdentifierNameSyntax _name;

		internal readonly PunctuationSyntax _colonEqualsToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal IdentifierNameSyntax Name => _name;

		internal PunctuationSyntax ColonEqualsToken => _colonEqualsToken;

		internal NameColonEqualsSyntax(SyntaxKind kind, IdentifierNameSyntax name, PunctuationSyntax colonEqualsToken)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(colonEqualsToken);
			_colonEqualsToken = colonEqualsToken;
		}

		internal NameColonEqualsSyntax(SyntaxKind kind, IdentifierNameSyntax name, PunctuationSyntax colonEqualsToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(colonEqualsToken);
			_colonEqualsToken = colonEqualsToken;
		}

		internal NameColonEqualsSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierNameSyntax name, PunctuationSyntax colonEqualsToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(colonEqualsToken);
			_colonEqualsToken = colonEqualsToken;
		}

		internal NameColonEqualsSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)reader.ReadValue();
			if (identifierNameSyntax != null)
			{
				AdjustFlagsAndWidth(identifierNameSyntax);
				_name = identifierNameSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_colonEqualsToken = punctuationSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_name);
			writer.WriteValue(_colonEqualsToken);
		}

		static NameColonEqualsSyntax()
		{
			CreateInstance = (ObjectReader o) => new NameColonEqualsSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(NameColonEqualsSyntax), (ObjectReader r) => new NameColonEqualsSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NameColonEqualsSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _name, 
				1 => _colonEqualsToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new NameColonEqualsSyntax(base.Kind, newErrors, GetAnnotations(), _name, _colonEqualsToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new NameColonEqualsSyntax(base.Kind, GetDiagnostics(), annotations, _name, _colonEqualsToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitNameColonEquals(this);
		}
	}
}

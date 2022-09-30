using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class QualifiedNameSyntax : NameSyntax
	{
		internal readonly NameSyntax _left;

		internal readonly PunctuationSyntax _dotToken;

		internal readonly SimpleNameSyntax _right;

		internal static Func<ObjectReader, object> CreateInstance;

		internal NameSyntax Left => _left;

		internal PunctuationSyntax DotToken => _dotToken;

		internal SimpleNameSyntax Right => _right;

		internal QualifiedNameSyntax(SyntaxKind kind, NameSyntax left, PunctuationSyntax dotToken, SimpleNameSyntax right)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(left);
			_left = left;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(right);
			_right = right;
		}

		internal QualifiedNameSyntax(SyntaxKind kind, NameSyntax left, PunctuationSyntax dotToken, SimpleNameSyntax right, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(left);
			_left = left;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(right);
			_right = right;
		}

		internal QualifiedNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, NameSyntax left, PunctuationSyntax dotToken, SimpleNameSyntax right)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(left);
			_left = left;
			AdjustFlagsAndWidth(dotToken);
			_dotToken = dotToken;
			AdjustFlagsAndWidth(right);
			_right = right;
		}

		internal QualifiedNameSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			NameSyntax nameSyntax = (NameSyntax)reader.ReadValue();
			if (nameSyntax != null)
			{
				AdjustFlagsAndWidth(nameSyntax);
				_left = nameSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_dotToken = punctuationSyntax;
			}
			SimpleNameSyntax simpleNameSyntax = (SimpleNameSyntax)reader.ReadValue();
			if (simpleNameSyntax != null)
			{
				AdjustFlagsAndWidth(simpleNameSyntax);
				_right = simpleNameSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_left);
			writer.WriteValue(_dotToken);
			writer.WriteValue(_right);
		}

		static QualifiedNameSyntax()
		{
			CreateInstance = (ObjectReader o) => new QualifiedNameSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(QualifiedNameSyntax), (ObjectReader r) => new QualifiedNameSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _left, 
				1 => _dotToken, 
				2 => _right, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new QualifiedNameSyntax(base.Kind, newErrors, GetAnnotations(), _left, _dotToken, _right);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new QualifiedNameSyntax(base.Kind, GetDiagnostics(), annotations, _left, _dotToken, _right);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitQualifiedName(this);
		}
	}
}

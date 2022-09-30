using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class OptionStatementSyntax : DeclarationStatementSyntax
	{
		internal readonly KeywordSyntax _optionKeyword;

		internal readonly KeywordSyntax _nameKeyword;

		internal readonly KeywordSyntax _valueKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax OptionKeyword => _optionKeyword;

		internal KeywordSyntax NameKeyword => _nameKeyword;

		internal KeywordSyntax ValueKeyword => _valueKeyword;

		internal OptionStatementSyntax(SyntaxKind kind, KeywordSyntax optionKeyword, KeywordSyntax nameKeyword, KeywordSyntax valueKeyword)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(optionKeyword);
			_optionKeyword = optionKeyword;
			AdjustFlagsAndWidth(nameKeyword);
			_nameKeyword = nameKeyword;
			if (valueKeyword != null)
			{
				AdjustFlagsAndWidth(valueKeyword);
				_valueKeyword = valueKeyword;
			}
		}

		internal OptionStatementSyntax(SyntaxKind kind, KeywordSyntax optionKeyword, KeywordSyntax nameKeyword, KeywordSyntax valueKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(optionKeyword);
			_optionKeyword = optionKeyword;
			AdjustFlagsAndWidth(nameKeyword);
			_nameKeyword = nameKeyword;
			if (valueKeyword != null)
			{
				AdjustFlagsAndWidth(valueKeyword);
				_valueKeyword = valueKeyword;
			}
		}

		internal OptionStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax optionKeyword, KeywordSyntax nameKeyword, KeywordSyntax valueKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(optionKeyword);
			_optionKeyword = optionKeyword;
			AdjustFlagsAndWidth(nameKeyword);
			_nameKeyword = nameKeyword;
			if (valueKeyword != null)
			{
				AdjustFlagsAndWidth(valueKeyword);
				_valueKeyword = valueKeyword;
			}
		}

		internal OptionStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_optionKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_nameKeyword = keywordSyntax2;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax3 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax3);
				_valueKeyword = keywordSyntax3;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_optionKeyword);
			writer.WriteValue(_nameKeyword);
			writer.WriteValue(_valueKeyword);
		}

		static OptionStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new OptionStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(OptionStatementSyntax), (ObjectReader r) => new OptionStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _optionKeyword, 
				1 => _nameKeyword, 
				2 => _valueKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new OptionStatementSyntax(base.Kind, newErrors, GetAnnotations(), _optionKeyword, _nameKeyword, _valueKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new OptionStatementSyntax(base.Kind, GetDiagnostics(), annotations, _optionKeyword, _nameKeyword, _valueKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitOptionStatement(this);
		}
	}
}

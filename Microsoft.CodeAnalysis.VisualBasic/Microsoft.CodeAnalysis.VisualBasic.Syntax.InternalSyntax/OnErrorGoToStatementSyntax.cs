using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class OnErrorGoToStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _onKeyword;

		internal readonly KeywordSyntax _errorKeyword;

		internal readonly KeywordSyntax _goToKeyword;

		internal readonly PunctuationSyntax _minus;

		internal readonly LabelSyntax _label;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax OnKeyword => _onKeyword;

		internal KeywordSyntax ErrorKeyword => _errorKeyword;

		internal KeywordSyntax GoToKeyword => _goToKeyword;

		internal PunctuationSyntax Minus => _minus;

		internal LabelSyntax Label => _label;

		internal OnErrorGoToStatementSyntax(SyntaxKind kind, KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
			: base(kind)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(onKeyword);
			_onKeyword = onKeyword;
			AdjustFlagsAndWidth(errorKeyword);
			_errorKeyword = errorKeyword;
			AdjustFlagsAndWidth(goToKeyword);
			_goToKeyword = goToKeyword;
			if (minus != null)
			{
				AdjustFlagsAndWidth(minus);
				_minus = minus;
			}
			AdjustFlagsAndWidth(label);
			_label = label;
		}

		internal OnErrorGoToStatementSyntax(SyntaxKind kind, KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(onKeyword);
			_onKeyword = onKeyword;
			AdjustFlagsAndWidth(errorKeyword);
			_errorKeyword = errorKeyword;
			AdjustFlagsAndWidth(goToKeyword);
			_goToKeyword = goToKeyword;
			if (minus != null)
			{
				AdjustFlagsAndWidth(minus);
				_minus = minus;
			}
			AdjustFlagsAndWidth(label);
			_label = label;
		}

		internal OnErrorGoToStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(onKeyword);
			_onKeyword = onKeyword;
			AdjustFlagsAndWidth(errorKeyword);
			_errorKeyword = errorKeyword;
			AdjustFlagsAndWidth(goToKeyword);
			_goToKeyword = goToKeyword;
			if (minus != null)
			{
				AdjustFlagsAndWidth(minus);
				_minus = minus;
			}
			AdjustFlagsAndWidth(label);
			_label = label;
		}

		internal OnErrorGoToStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_onKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_errorKeyword = keywordSyntax2;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax3 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax3);
				_goToKeyword = keywordSyntax3;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_minus = punctuationSyntax;
			}
			LabelSyntax labelSyntax = (LabelSyntax)reader.ReadValue();
			if (labelSyntax != null)
			{
				AdjustFlagsAndWidth(labelSyntax);
				_label = labelSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_onKeyword);
			writer.WriteValue(_errorKeyword);
			writer.WriteValue(_goToKeyword);
			writer.WriteValue(_minus);
			writer.WriteValue(_label);
		}

		static OnErrorGoToStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new OnErrorGoToStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(OnErrorGoToStatementSyntax), (ObjectReader r) => new OnErrorGoToStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _onKeyword, 
				1 => _errorKeyword, 
				2 => _goToKeyword, 
				3 => _minus, 
				4 => _label, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new OnErrorGoToStatementSyntax(base.Kind, newErrors, GetAnnotations(), _onKeyword, _errorKeyword, _goToKeyword, _minus, _label);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new OnErrorGoToStatementSyntax(base.Kind, GetDiagnostics(), annotations, _onKeyword, _errorKeyword, _goToKeyword, _minus, _label);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitOnErrorGoToStatement(this);
		}
	}
}

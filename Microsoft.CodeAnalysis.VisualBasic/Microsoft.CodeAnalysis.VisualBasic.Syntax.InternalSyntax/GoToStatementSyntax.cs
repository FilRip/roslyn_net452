using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class GoToStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _goToKeyword;

		internal readonly LabelSyntax _label;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax GoToKeyword => _goToKeyword;

		internal LabelSyntax Label => _label;

		internal GoToStatementSyntax(SyntaxKind kind, KeywordSyntax goToKeyword, LabelSyntax label)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(goToKeyword);
			_goToKeyword = goToKeyword;
			AdjustFlagsAndWidth(label);
			_label = label;
		}

		internal GoToStatementSyntax(SyntaxKind kind, KeywordSyntax goToKeyword, LabelSyntax label, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(goToKeyword);
			_goToKeyword = goToKeyword;
			AdjustFlagsAndWidth(label);
			_label = label;
		}

		internal GoToStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax goToKeyword, LabelSyntax label)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(goToKeyword);
			_goToKeyword = goToKeyword;
			AdjustFlagsAndWidth(label);
			_label = label;
		}

		internal GoToStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_goToKeyword = keywordSyntax;
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
			writer.WriteValue(_goToKeyword);
			writer.WriteValue(_label);
		}

		static GoToStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new GoToStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(GoToStatementSyntax), (ObjectReader r) => new GoToStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GoToStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _goToKeyword, 
				1 => _label, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new GoToStatementSyntax(base.Kind, newErrors, GetAnnotations(), _goToKeyword, _label);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new GoToStatementSyntax(base.Kind, GetDiagnostics(), annotations, _goToKeyword, _label);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitGoToStatement(this);
		}
	}
}

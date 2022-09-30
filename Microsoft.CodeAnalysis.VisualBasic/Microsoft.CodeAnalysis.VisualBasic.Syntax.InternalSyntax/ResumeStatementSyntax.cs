using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ResumeStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _resumeKeyword;

		internal readonly LabelSyntax _label;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ResumeKeyword => _resumeKeyword;

		internal LabelSyntax Label => _label;

		internal ResumeStatementSyntax(SyntaxKind kind, KeywordSyntax resumeKeyword, LabelSyntax label)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(resumeKeyword);
			_resumeKeyword = resumeKeyword;
			if (label != null)
			{
				AdjustFlagsAndWidth(label);
				_label = label;
			}
		}

		internal ResumeStatementSyntax(SyntaxKind kind, KeywordSyntax resumeKeyword, LabelSyntax label, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(resumeKeyword);
			_resumeKeyword = resumeKeyword;
			if (label != null)
			{
				AdjustFlagsAndWidth(label);
				_label = label;
			}
		}

		internal ResumeStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax resumeKeyword, LabelSyntax label)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(resumeKeyword);
			_resumeKeyword = resumeKeyword;
			if (label != null)
			{
				AdjustFlagsAndWidth(label);
				_label = label;
			}
		}

		internal ResumeStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_resumeKeyword = keywordSyntax;
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
			writer.WriteValue(_resumeKeyword);
			writer.WriteValue(_label);
		}

		static ResumeStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ResumeStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ResumeStatementSyntax), (ObjectReader r) => new ResumeStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _resumeKeyword, 
				1 => _label, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ResumeStatementSyntax(base.Kind, newErrors, GetAnnotations(), _resumeKeyword, _label);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ResumeStatementSyntax(base.Kind, GetDiagnostics(), annotations, _resumeKeyword, _label);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitResumeStatement(this);
		}
	}
}

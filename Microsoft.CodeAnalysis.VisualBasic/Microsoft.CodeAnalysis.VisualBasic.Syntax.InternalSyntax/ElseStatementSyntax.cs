using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ElseStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _elseKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ElseKeyword => _elseKeyword;

		internal ElseStatementSyntax(SyntaxKind kind, KeywordSyntax elseKeyword)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(elseKeyword);
			_elseKeyword = elseKeyword;
		}

		internal ElseStatementSyntax(SyntaxKind kind, KeywordSyntax elseKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(elseKeyword);
			_elseKeyword = elseKeyword;
		}

		internal ElseStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax elseKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(elseKeyword);
			_elseKeyword = elseKeyword;
		}

		internal ElseStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_elseKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_elseKeyword);
		}

		static ElseStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ElseStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ElseStatementSyntax), (ObjectReader r) => new ElseStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _elseKeyword;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ElseStatementSyntax(base.Kind, newErrors, GetAnnotations(), _elseKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ElseStatementSyntax(base.Kind, GetDiagnostics(), annotations, _elseKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitElseStatement(this);
		}
	}
}

using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class FinallyStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _finallyKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax FinallyKeyword => _finallyKeyword;

		internal FinallyStatementSyntax(SyntaxKind kind, KeywordSyntax finallyKeyword)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(finallyKeyword);
			_finallyKeyword = finallyKeyword;
		}

		internal FinallyStatementSyntax(SyntaxKind kind, KeywordSyntax finallyKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(finallyKeyword);
			_finallyKeyword = finallyKeyword;
		}

		internal FinallyStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax finallyKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(finallyKeyword);
			_finallyKeyword = finallyKeyword;
		}

		internal FinallyStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_finallyKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_finallyKeyword);
		}

		static FinallyStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new FinallyStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(FinallyStatementSyntax), (ObjectReader r) => new FinallyStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _finallyKeyword;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new FinallyStatementSyntax(base.Kind, newErrors, GetAnnotations(), _finallyKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new FinallyStatementSyntax(base.Kind, GetDiagnostics(), annotations, _finallyKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitFinallyStatement(this);
		}
	}
}

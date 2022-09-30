using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class LoopStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _loopKeyword;

		internal readonly WhileOrUntilClauseSyntax _whileOrUntilClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax LoopKeyword => _loopKeyword;

		internal WhileOrUntilClauseSyntax WhileOrUntilClause => _whileOrUntilClause;

		internal LoopStatementSyntax(SyntaxKind kind, KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(loopKeyword);
			_loopKeyword = loopKeyword;
			if (whileOrUntilClause != null)
			{
				AdjustFlagsAndWidth(whileOrUntilClause);
				_whileOrUntilClause = whileOrUntilClause;
			}
		}

		internal LoopStatementSyntax(SyntaxKind kind, KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(loopKeyword);
			_loopKeyword = loopKeyword;
			if (whileOrUntilClause != null)
			{
				AdjustFlagsAndWidth(whileOrUntilClause);
				_whileOrUntilClause = whileOrUntilClause;
			}
		}

		internal LoopStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(loopKeyword);
			_loopKeyword = loopKeyword;
			if (whileOrUntilClause != null)
			{
				AdjustFlagsAndWidth(whileOrUntilClause);
				_whileOrUntilClause = whileOrUntilClause;
			}
		}

		internal LoopStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_loopKeyword = keywordSyntax;
			}
			WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = (WhileOrUntilClauseSyntax)reader.ReadValue();
			if (whileOrUntilClauseSyntax != null)
			{
				AdjustFlagsAndWidth(whileOrUntilClauseSyntax);
				_whileOrUntilClause = whileOrUntilClauseSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_loopKeyword);
			writer.WriteValue(_whileOrUntilClause);
		}

		static LoopStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new LoopStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(LoopStatementSyntax), (ObjectReader r) => new LoopStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LoopStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _loopKeyword, 
				1 => _whileOrUntilClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new LoopStatementSyntax(base.Kind, newErrors, GetAnnotations(), _loopKeyword, _whileOrUntilClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new LoopStatementSyntax(base.Kind, GetDiagnostics(), annotations, _loopKeyword, _whileOrUntilClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitLoopStatement(this);
		}
	}
}

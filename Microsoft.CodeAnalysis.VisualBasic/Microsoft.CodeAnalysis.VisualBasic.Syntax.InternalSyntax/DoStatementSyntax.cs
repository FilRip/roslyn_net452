using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DoStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _doKeyword;

		internal readonly WhileOrUntilClauseSyntax _whileOrUntilClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax DoKeyword => _doKeyword;

		internal WhileOrUntilClauseSyntax WhileOrUntilClause => _whileOrUntilClause;

		internal DoStatementSyntax(SyntaxKind kind, KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(doKeyword);
			_doKeyword = doKeyword;
			if (whileOrUntilClause != null)
			{
				AdjustFlagsAndWidth(whileOrUntilClause);
				_whileOrUntilClause = whileOrUntilClause;
			}
		}

		internal DoStatementSyntax(SyntaxKind kind, KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(doKeyword);
			_doKeyword = doKeyword;
			if (whileOrUntilClause != null)
			{
				AdjustFlagsAndWidth(whileOrUntilClause);
				_whileOrUntilClause = whileOrUntilClause;
			}
		}

		internal DoStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(doKeyword);
			_doKeyword = doKeyword;
			if (whileOrUntilClause != null)
			{
				AdjustFlagsAndWidth(whileOrUntilClause);
				_whileOrUntilClause = whileOrUntilClause;
			}
		}

		internal DoStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_doKeyword = keywordSyntax;
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
			writer.WriteValue(_doKeyword);
			writer.WriteValue(_whileOrUntilClause);
		}

		static DoStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new DoStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(DoStatementSyntax), (ObjectReader r) => new DoStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _doKeyword, 
				1 => _whileOrUntilClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new DoStatementSyntax(base.Kind, newErrors, GetAnnotations(), _doKeyword, _whileOrUntilClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new DoStatementSyntax(base.Kind, GetDiagnostics(), annotations, _doKeyword, _whileOrUntilClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitDoStatement(this);
		}
	}
}

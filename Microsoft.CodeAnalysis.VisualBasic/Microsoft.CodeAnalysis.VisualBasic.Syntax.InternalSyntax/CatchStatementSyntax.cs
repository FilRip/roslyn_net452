using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CatchStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _catchKeyword;

		internal readonly IdentifierNameSyntax _identifierName;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal readonly CatchFilterClauseSyntax _whenClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax CatchKeyword => _catchKeyword;

		internal IdentifierNameSyntax IdentifierName => _identifierName;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal CatchFilterClauseSyntax WhenClause => _whenClause;

		internal CatchStatementSyntax(SyntaxKind kind, KeywordSyntax catchKeyword, IdentifierNameSyntax identifierName, SimpleAsClauseSyntax asClause, CatchFilterClauseSyntax whenClause)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(catchKeyword);
			_catchKeyword = catchKeyword;
			if (identifierName != null)
			{
				AdjustFlagsAndWidth(identifierName);
				_identifierName = identifierName;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (whenClause != null)
			{
				AdjustFlagsAndWidth(whenClause);
				_whenClause = whenClause;
			}
		}

		internal CatchStatementSyntax(SyntaxKind kind, KeywordSyntax catchKeyword, IdentifierNameSyntax identifierName, SimpleAsClauseSyntax asClause, CatchFilterClauseSyntax whenClause, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(catchKeyword);
			_catchKeyword = catchKeyword;
			if (identifierName != null)
			{
				AdjustFlagsAndWidth(identifierName);
				_identifierName = identifierName;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (whenClause != null)
			{
				AdjustFlagsAndWidth(whenClause);
				_whenClause = whenClause;
			}
		}

		internal CatchStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax catchKeyword, IdentifierNameSyntax identifierName, SimpleAsClauseSyntax asClause, CatchFilterClauseSyntax whenClause)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(catchKeyword);
			_catchKeyword = catchKeyword;
			if (identifierName != null)
			{
				AdjustFlagsAndWidth(identifierName);
				_identifierName = identifierName;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (whenClause != null)
			{
				AdjustFlagsAndWidth(whenClause);
				_whenClause = whenClause;
			}
		}

		internal CatchStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_catchKeyword = keywordSyntax;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)reader.ReadValue();
			if (identifierNameSyntax != null)
			{
				AdjustFlagsAndWidth(identifierNameSyntax);
				_identifierName = identifierNameSyntax;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)reader.ReadValue();
			if (simpleAsClauseSyntax != null)
			{
				AdjustFlagsAndWidth(simpleAsClauseSyntax);
				_asClause = simpleAsClauseSyntax;
			}
			CatchFilterClauseSyntax catchFilterClauseSyntax = (CatchFilterClauseSyntax)reader.ReadValue();
			if (catchFilterClauseSyntax != null)
			{
				AdjustFlagsAndWidth(catchFilterClauseSyntax);
				_whenClause = catchFilterClauseSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_catchKeyword);
			writer.WriteValue(_identifierName);
			writer.WriteValue(_asClause);
			writer.WriteValue(_whenClause);
		}

		static CatchStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new CatchStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CatchStatementSyntax), (ObjectReader r) => new CatchStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _catchKeyword, 
				1 => _identifierName, 
				2 => _asClause, 
				3 => _whenClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CatchStatementSyntax(base.Kind, newErrors, GetAnnotations(), _catchKeyword, _identifierName, _asClause, _whenClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CatchStatementSyntax(base.Kind, GetDiagnostics(), annotations, _catchKeyword, _identifierName, _asClause, _whenClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCatchStatement(this);
		}
	}
}

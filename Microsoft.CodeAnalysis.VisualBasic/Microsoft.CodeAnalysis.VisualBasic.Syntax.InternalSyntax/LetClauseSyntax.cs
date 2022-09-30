using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class LetClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _letKeyword;

		internal readonly GreenNode _variables;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax LetKeyword => _letKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> Variables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExpressionRangeVariableSyntax>(_variables));

		internal LetClauseSyntax(SyntaxKind kind, KeywordSyntax letKeyword, GreenNode variables)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(letKeyword);
			_letKeyword = letKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal LetClauseSyntax(SyntaxKind kind, KeywordSyntax letKeyword, GreenNode variables, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(letKeyword);
			_letKeyword = letKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal LetClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax letKeyword, GreenNode variables)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(letKeyword);
			_letKeyword = letKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal LetClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_letKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_variables = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_letKeyword);
			writer.WriteValue(_variables);
		}

		static LetClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new LetClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(LetClauseSyntax), (ObjectReader r) => new LetClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _letKeyword, 
				1 => _variables, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new LetClauseSyntax(base.Kind, newErrors, GetAnnotations(), _letKeyword, _variables);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new LetClauseSyntax(base.Kind, GetDiagnostics(), annotations, _letKeyword, _variables);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitLetClause(this);
		}
	}
}

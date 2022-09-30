using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SelectClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _selectKeyword;

		internal readonly GreenNode _variables;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax SelectKeyword => _selectKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax> Variables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionRangeVariableSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExpressionRangeVariableSyntax>(_variables));

		internal SelectClauseSyntax(SyntaxKind kind, KeywordSyntax selectKeyword, GreenNode variables)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(selectKeyword);
			_selectKeyword = selectKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal SelectClauseSyntax(SyntaxKind kind, KeywordSyntax selectKeyword, GreenNode variables, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(selectKeyword);
			_selectKeyword = selectKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal SelectClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax selectKeyword, GreenNode variables)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(selectKeyword);
			_selectKeyword = selectKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal SelectClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_selectKeyword = keywordSyntax;
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
			writer.WriteValue(_selectKeyword);
			writer.WriteValue(_variables);
		}

		static SelectClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new SelectClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SelectClauseSyntax), (ObjectReader r) => new SelectClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _selectKeyword, 
				1 => _variables, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SelectClauseSyntax(base.Kind, newErrors, GetAnnotations(), _selectKeyword, _variables);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SelectClauseSyntax(base.Kind, GetDiagnostics(), annotations, _selectKeyword, _variables);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSelectClause(this);
		}
	}
}

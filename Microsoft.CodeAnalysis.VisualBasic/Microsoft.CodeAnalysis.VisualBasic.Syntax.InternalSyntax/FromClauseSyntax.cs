using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class FromClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _fromKeyword;

		internal readonly GreenNode _variables;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax FromKeyword => _fromKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax> Variables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CollectionRangeVariableSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CollectionRangeVariableSyntax>(_variables));

		internal FromClauseSyntax(SyntaxKind kind, KeywordSyntax fromKeyword, GreenNode variables)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(fromKeyword);
			_fromKeyword = fromKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal FromClauseSyntax(SyntaxKind kind, KeywordSyntax fromKeyword, GreenNode variables, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(fromKeyword);
			_fromKeyword = fromKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal FromClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax fromKeyword, GreenNode variables)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(fromKeyword);
			_fromKeyword = fromKeyword;
			if (variables != null)
			{
				AdjustFlagsAndWidth(variables);
				_variables = variables;
			}
		}

		internal FromClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_fromKeyword = keywordSyntax;
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
			writer.WriteValue(_fromKeyword);
			writer.WriteValue(_variables);
		}

		static FromClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new FromClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(FromClauseSyntax), (ObjectReader r) => new FromClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _fromKeyword, 
				1 => _variables, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new FromClauseSyntax(base.Kind, newErrors, GetAnnotations(), _fromKeyword, _variables);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new FromClauseSyntax(base.Kind, GetDiagnostics(), annotations, _fromKeyword, _variables);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitFromClause(this);
		}
	}
}

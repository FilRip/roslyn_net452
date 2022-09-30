using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AsNewClauseSyntax : AsClauseSyntax
	{
		internal readonly NewExpressionSyntax _newExpression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal NewExpressionSyntax NewExpression => _newExpression;

		internal AsNewClauseSyntax(SyntaxKind kind, KeywordSyntax asKeyword, NewExpressionSyntax newExpression)
			: base(kind, asKeyword)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(newExpression);
			_newExpression = newExpression;
		}

		internal AsNewClauseSyntax(SyntaxKind kind, KeywordSyntax asKeyword, NewExpressionSyntax newExpression, ISyntaxFactoryContext context)
			: base(kind, asKeyword)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(newExpression);
			_newExpression = newExpression;
		}

		internal AsNewClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax asKeyword, NewExpressionSyntax newExpression)
			: base(kind, errors, annotations, asKeyword)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(newExpression);
			_newExpression = newExpression;
		}

		internal AsNewClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			NewExpressionSyntax newExpressionSyntax = (NewExpressionSyntax)reader.ReadValue();
			if (newExpressionSyntax != null)
			{
				AdjustFlagsAndWidth(newExpressionSyntax);
				_newExpression = newExpressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_newExpression);
		}

		static AsNewClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new AsNewClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AsNewClauseSyntax), (ObjectReader r) => new AsNewClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AsNewClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _asKeyword, 
				1 => _newExpression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AsNewClauseSyntax(base.Kind, newErrors, GetAnnotations(), _asKeyword, _newExpression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AsNewClauseSyntax(base.Kind, GetDiagnostics(), annotations, _asKeyword, _newExpression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAsNewClause(this);
		}
	}
}

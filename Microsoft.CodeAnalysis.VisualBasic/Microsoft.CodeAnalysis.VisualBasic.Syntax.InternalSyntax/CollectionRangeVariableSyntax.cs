using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CollectionRangeVariableSyntax : VisualBasicSyntaxNode
	{
		internal readonly ModifiedIdentifierSyntax _identifier;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal readonly KeywordSyntax _inKeyword;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ModifiedIdentifierSyntax Identifier => _identifier;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal KeywordSyntax InKeyword => _inKeyword;

		internal ExpressionSyntax Expression => _expression;

		internal CollectionRangeVariableSyntax(SyntaxKind kind, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, KeywordSyntax inKeyword, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			AdjustFlagsAndWidth(inKeyword);
			_inKeyword = inKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal CollectionRangeVariableSyntax(SyntaxKind kind, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, KeywordSyntax inKeyword, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			AdjustFlagsAndWidth(inKeyword);
			_inKeyword = inKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal CollectionRangeVariableSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, KeywordSyntax inKeyword, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			AdjustFlagsAndWidth(inKeyword);
			_inKeyword = inKeyword;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal CollectionRangeVariableSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = (ModifiedIdentifierSyntax)reader.ReadValue();
			if (modifiedIdentifierSyntax != null)
			{
				AdjustFlagsAndWidth(modifiedIdentifierSyntax);
				_identifier = modifiedIdentifierSyntax;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)reader.ReadValue();
			if (simpleAsClauseSyntax != null)
			{
				AdjustFlagsAndWidth(simpleAsClauseSyntax);
				_asClause = simpleAsClauseSyntax;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_inKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_identifier);
			writer.WriteValue(_asClause);
			writer.WriteValue(_inKeyword);
			writer.WriteValue(_expression);
		}

		static CollectionRangeVariableSyntax()
		{
			CreateInstance = (ObjectReader o) => new CollectionRangeVariableSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CollectionRangeVariableSyntax), (ObjectReader r) => new CollectionRangeVariableSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _identifier, 
				1 => _asClause, 
				2 => _inKeyword, 
				3 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CollectionRangeVariableSyntax(base.Kind, newErrors, GetAnnotations(), _identifier, _asClause, _inKeyword, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CollectionRangeVariableSyntax(base.Kind, GetDiagnostics(), annotations, _identifier, _asClause, _inKeyword, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCollectionRangeVariable(this);
		}
	}
}

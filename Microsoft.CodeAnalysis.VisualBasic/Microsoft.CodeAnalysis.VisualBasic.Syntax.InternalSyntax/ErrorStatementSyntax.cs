using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ErrorStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _errorKeyword;

		internal readonly ExpressionSyntax _errorNumber;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ErrorKeyword => _errorKeyword;

		internal ExpressionSyntax ErrorNumber => _errorNumber;

		internal ErrorStatementSyntax(SyntaxKind kind, KeywordSyntax errorKeyword, ExpressionSyntax errorNumber)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(errorKeyword);
			_errorKeyword = errorKeyword;
			AdjustFlagsAndWidth(errorNumber);
			_errorNumber = errorNumber;
		}

		internal ErrorStatementSyntax(SyntaxKind kind, KeywordSyntax errorKeyword, ExpressionSyntax errorNumber, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(errorKeyword);
			_errorKeyword = errorKeyword;
			AdjustFlagsAndWidth(errorNumber);
			_errorNumber = errorNumber;
		}

		internal ErrorStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax errorKeyword, ExpressionSyntax errorNumber)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(errorKeyword);
			_errorKeyword = errorKeyword;
			AdjustFlagsAndWidth(errorNumber);
			_errorNumber = errorNumber;
		}

		internal ErrorStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_errorKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_errorNumber = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_errorKeyword);
			writer.WriteValue(_errorNumber);
		}

		static ErrorStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ErrorStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ErrorStatementSyntax), (ObjectReader r) => new ErrorStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ErrorStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _errorKeyword, 
				1 => _errorNumber, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ErrorStatementSyntax(base.Kind, newErrors, GetAnnotations(), _errorKeyword, _errorNumber);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ErrorStatementSyntax(base.Kind, GetDiagnostics(), annotations, _errorKeyword, _errorNumber);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitErrorStatement(this);
		}
	}
}

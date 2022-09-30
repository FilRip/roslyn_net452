using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EraseStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _eraseKeyword;

		internal readonly GreenNode _expressions;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax EraseKeyword => _eraseKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> Expressions => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExpressionSyntax>(_expressions));

		internal EraseStatementSyntax(SyntaxKind kind, KeywordSyntax eraseKeyword, GreenNode expressions)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(eraseKeyword);
			_eraseKeyword = eraseKeyword;
			if (expressions != null)
			{
				AdjustFlagsAndWidth(expressions);
				_expressions = expressions;
			}
		}

		internal EraseStatementSyntax(SyntaxKind kind, KeywordSyntax eraseKeyword, GreenNode expressions, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(eraseKeyword);
			_eraseKeyword = eraseKeyword;
			if (expressions != null)
			{
				AdjustFlagsAndWidth(expressions);
				_expressions = expressions;
			}
		}

		internal EraseStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax eraseKeyword, GreenNode expressions)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(eraseKeyword);
			_eraseKeyword = eraseKeyword;
			if (expressions != null)
			{
				AdjustFlagsAndWidth(expressions);
				_expressions = expressions;
			}
		}

		internal EraseStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_eraseKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_expressions = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_eraseKeyword);
			writer.WriteValue(_expressions);
		}

		static EraseStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new EraseStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EraseStatementSyntax), (ObjectReader r) => new EraseStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EraseStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _eraseKeyword, 
				1 => _expressions, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EraseStatementSyntax(base.Kind, newErrors, GetAnnotations(), _eraseKeyword, _expressions);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EraseStatementSyntax(base.Kind, GetDiagnostics(), annotations, _eraseKeyword, _expressions);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEraseStatement(this);
		}
	}
}

using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class PartitionClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _skipOrTakeKeyword;

		internal readonly ExpressionSyntax _count;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax SkipOrTakeKeyword => _skipOrTakeKeyword;

		internal ExpressionSyntax Count => _count;

		internal PartitionClauseSyntax(SyntaxKind kind, KeywordSyntax skipOrTakeKeyword, ExpressionSyntax count)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(skipOrTakeKeyword);
			_skipOrTakeKeyword = skipOrTakeKeyword;
			AdjustFlagsAndWidth(count);
			_count = count;
		}

		internal PartitionClauseSyntax(SyntaxKind kind, KeywordSyntax skipOrTakeKeyword, ExpressionSyntax count, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(skipOrTakeKeyword);
			_skipOrTakeKeyword = skipOrTakeKeyword;
			AdjustFlagsAndWidth(count);
			_count = count;
		}

		internal PartitionClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax skipOrTakeKeyword, ExpressionSyntax count)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(skipOrTakeKeyword);
			_skipOrTakeKeyword = skipOrTakeKeyword;
			AdjustFlagsAndWidth(count);
			_count = count;
		}

		internal PartitionClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_skipOrTakeKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_count = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_skipOrTakeKeyword);
			writer.WriteValue(_count);
		}

		static PartitionClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new PartitionClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(PartitionClauseSyntax), (ObjectReader r) => new PartitionClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _skipOrTakeKeyword, 
				1 => _count, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new PartitionClauseSyntax(base.Kind, newErrors, GetAnnotations(), _skipOrTakeKeyword, _count);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new PartitionClauseSyntax(base.Kind, GetDiagnostics(), annotations, _skipOrTakeKeyword, _count);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitPartitionClause(this);
		}
	}
}

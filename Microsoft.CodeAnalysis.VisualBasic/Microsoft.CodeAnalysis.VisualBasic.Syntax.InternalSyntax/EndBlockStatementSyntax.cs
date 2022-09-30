using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EndBlockStatementSyntax : DeclarationStatementSyntax
	{
		internal readonly KeywordSyntax _endKeyword;

		internal readonly KeywordSyntax _blockKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax EndKeyword => _endKeyword;

		internal KeywordSyntax BlockKeyword => _blockKeyword;

		internal EndBlockStatementSyntax(SyntaxKind kind, KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(blockKeyword);
			_blockKeyword = blockKeyword;
		}

		internal EndBlockStatementSyntax(SyntaxKind kind, KeywordSyntax endKeyword, KeywordSyntax blockKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(blockKeyword);
			_blockKeyword = blockKeyword;
		}

		internal EndBlockStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(endKeyword);
			_endKeyword = endKeyword;
			AdjustFlagsAndWidth(blockKeyword);
			_blockKeyword = blockKeyword;
		}

		internal EndBlockStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_endKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_blockKeyword = keywordSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_endKeyword);
			writer.WriteValue(_blockKeyword);
		}

		static EndBlockStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new EndBlockStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EndBlockStatementSyntax), (ObjectReader r) => new EndBlockStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _endKeyword, 
				1 => _blockKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EndBlockStatementSyntax(base.Kind, newErrors, GetAnnotations(), _endKeyword, _blockKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EndBlockStatementSyntax(base.Kind, GetDiagnostics(), annotations, _endKeyword, _blockKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEndBlockStatement(this);
		}
	}
}

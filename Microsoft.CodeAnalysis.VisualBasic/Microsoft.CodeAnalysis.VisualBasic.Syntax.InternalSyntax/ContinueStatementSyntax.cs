using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ContinueStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _continueKeyword;

		internal readonly KeywordSyntax _blockKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ContinueKeyword => _continueKeyword;

		internal KeywordSyntax BlockKeyword => _blockKeyword;

		internal ContinueStatementSyntax(SyntaxKind kind, KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(continueKeyword);
			_continueKeyword = continueKeyword;
			AdjustFlagsAndWidth(blockKeyword);
			_blockKeyword = blockKeyword;
		}

		internal ContinueStatementSyntax(SyntaxKind kind, KeywordSyntax continueKeyword, KeywordSyntax blockKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(continueKeyword);
			_continueKeyword = continueKeyword;
			AdjustFlagsAndWidth(blockKeyword);
			_blockKeyword = blockKeyword;
		}

		internal ContinueStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(continueKeyword);
			_continueKeyword = continueKeyword;
			AdjustFlagsAndWidth(blockKeyword);
			_blockKeyword = blockKeyword;
		}

		internal ContinueStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_continueKeyword = keywordSyntax;
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
			writer.WriteValue(_continueKeyword);
			writer.WriteValue(_blockKeyword);
		}

		static ContinueStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ContinueStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ContinueStatementSyntax), (ObjectReader r) => new ContinueStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ContinueStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _continueKeyword, 
				1 => _blockKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ContinueStatementSyntax(base.Kind, newErrors, GetAnnotations(), _continueKeyword, _blockKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ContinueStatementSyntax(base.Kind, GetDiagnostics(), annotations, _continueKeyword, _blockKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitContinueStatement(this);
		}
	}
}

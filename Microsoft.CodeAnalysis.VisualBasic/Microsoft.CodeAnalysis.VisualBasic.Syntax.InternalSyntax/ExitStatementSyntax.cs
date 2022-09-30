using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ExitStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _exitKeyword;

		internal readonly KeywordSyntax _blockKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ExitKeyword => _exitKeyword;

		internal KeywordSyntax BlockKeyword => _blockKeyword;

		internal ExitStatementSyntax(SyntaxKind kind, KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(exitKeyword);
			_exitKeyword = exitKeyword;
			AdjustFlagsAndWidth(blockKeyword);
			_blockKeyword = blockKeyword;
		}

		internal ExitStatementSyntax(SyntaxKind kind, KeywordSyntax exitKeyword, KeywordSyntax blockKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(exitKeyword);
			_exitKeyword = exitKeyword;
			AdjustFlagsAndWidth(blockKeyword);
			_blockKeyword = blockKeyword;
		}

		internal ExitStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(exitKeyword);
			_exitKeyword = exitKeyword;
			AdjustFlagsAndWidth(blockKeyword);
			_blockKeyword = blockKeyword;
		}

		internal ExitStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_exitKeyword = keywordSyntax;
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
			writer.WriteValue(_exitKeyword);
			writer.WriteValue(_blockKeyword);
		}

		static ExitStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ExitStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ExitStatementSyntax), (ObjectReader r) => new ExitStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ExitStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _exitKeyword, 
				1 => _blockKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ExitStatementSyntax(base.Kind, newErrors, GetAnnotations(), _exitKeyword, _blockKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ExitStatementSyntax(base.Kind, GetDiagnostics(), annotations, _exitKeyword, _blockKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitExitStatement(this);
		}
	}
}

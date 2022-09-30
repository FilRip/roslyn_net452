using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class OnErrorResumeNextStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _onKeyword;

		internal readonly KeywordSyntax _errorKeyword;

		internal readonly KeywordSyntax _resumeKeyword;

		internal readonly KeywordSyntax _nextKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax OnKeyword => _onKeyword;

		internal KeywordSyntax ErrorKeyword => _errorKeyword;

		internal KeywordSyntax ResumeKeyword => _resumeKeyword;

		internal KeywordSyntax NextKeyword => _nextKeyword;

		internal OnErrorResumeNextStatementSyntax(SyntaxKind kind, KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax resumeKeyword, KeywordSyntax nextKeyword)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(onKeyword);
			_onKeyword = onKeyword;
			AdjustFlagsAndWidth(errorKeyword);
			_errorKeyword = errorKeyword;
			AdjustFlagsAndWidth(resumeKeyword);
			_resumeKeyword = resumeKeyword;
			AdjustFlagsAndWidth(nextKeyword);
			_nextKeyword = nextKeyword;
		}

		internal OnErrorResumeNextStatementSyntax(SyntaxKind kind, KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax resumeKeyword, KeywordSyntax nextKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(onKeyword);
			_onKeyword = onKeyword;
			AdjustFlagsAndWidth(errorKeyword);
			_errorKeyword = errorKeyword;
			AdjustFlagsAndWidth(resumeKeyword);
			_resumeKeyword = resumeKeyword;
			AdjustFlagsAndWidth(nextKeyword);
			_nextKeyword = nextKeyword;
		}

		internal OnErrorResumeNextStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax resumeKeyword, KeywordSyntax nextKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(onKeyword);
			_onKeyword = onKeyword;
			AdjustFlagsAndWidth(errorKeyword);
			_errorKeyword = errorKeyword;
			AdjustFlagsAndWidth(resumeKeyword);
			_resumeKeyword = resumeKeyword;
			AdjustFlagsAndWidth(nextKeyword);
			_nextKeyword = nextKeyword;
		}

		internal OnErrorResumeNextStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_onKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_errorKeyword = keywordSyntax2;
			}
			KeywordSyntax keywordSyntax3 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax3 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax3);
				_resumeKeyword = keywordSyntax3;
			}
			KeywordSyntax keywordSyntax4 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax4 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax4);
				_nextKeyword = keywordSyntax4;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_onKeyword);
			writer.WriteValue(_errorKeyword);
			writer.WriteValue(_resumeKeyword);
			writer.WriteValue(_nextKeyword);
		}

		static OnErrorResumeNextStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new OnErrorResumeNextStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(OnErrorResumeNextStatementSyntax), (ObjectReader r) => new OnErrorResumeNextStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _onKeyword, 
				1 => _errorKeyword, 
				2 => _resumeKeyword, 
				3 => _nextKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new OnErrorResumeNextStatementSyntax(base.Kind, newErrors, GetAnnotations(), _onKeyword, _errorKeyword, _resumeKeyword, _nextKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new OnErrorResumeNextStatementSyntax(base.Kind, GetDiagnostics(), annotations, _onKeyword, _errorKeyword, _resumeKeyword, _nextKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitOnErrorResumeNextStatement(this);
		}
	}
}

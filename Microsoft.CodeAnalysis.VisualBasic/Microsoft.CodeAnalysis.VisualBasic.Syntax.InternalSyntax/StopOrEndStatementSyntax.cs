using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class StopOrEndStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _stopOrEndKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax StopOrEndKeyword => _stopOrEndKeyword;

		internal StopOrEndStatementSyntax(SyntaxKind kind, KeywordSyntax stopOrEndKeyword)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(stopOrEndKeyword);
			_stopOrEndKeyword = stopOrEndKeyword;
		}

		internal StopOrEndStatementSyntax(SyntaxKind kind, KeywordSyntax stopOrEndKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(stopOrEndKeyword);
			_stopOrEndKeyword = stopOrEndKeyword;
		}

		internal StopOrEndStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax stopOrEndKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(stopOrEndKeyword);
			_stopOrEndKeyword = stopOrEndKeyword;
		}

		internal StopOrEndStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_stopOrEndKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_stopOrEndKeyword);
		}

		static StopOrEndStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new StopOrEndStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(StopOrEndStatementSyntax), (ObjectReader r) => new StopOrEndStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.StopOrEndStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _stopOrEndKeyword;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new StopOrEndStatementSyntax(base.Kind, newErrors, GetAnnotations(), _stopOrEndKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new StopOrEndStatementSyntax(base.Kind, GetDiagnostics(), annotations, _stopOrEndKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitStopOrEndStatement(this);
		}
	}
}

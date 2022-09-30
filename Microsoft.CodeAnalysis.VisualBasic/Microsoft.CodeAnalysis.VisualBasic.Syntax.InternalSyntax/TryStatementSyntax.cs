using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TryStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _tryKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax TryKeyword => _tryKeyword;

		internal TryStatementSyntax(SyntaxKind kind, KeywordSyntax tryKeyword)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(tryKeyword);
			_tryKeyword = tryKeyword;
		}

		internal TryStatementSyntax(SyntaxKind kind, KeywordSyntax tryKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(tryKeyword);
			_tryKeyword = tryKeyword;
		}

		internal TryStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax tryKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(tryKeyword);
			_tryKeyword = tryKeyword;
		}

		internal TryStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_tryKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_tryKeyword);
		}

		static TryStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new TryStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TryStatementSyntax), (ObjectReader r) => new TryStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _tryKeyword;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new TryStatementSyntax(base.Kind, newErrors, GetAnnotations(), _tryKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TryStatementSyntax(base.Kind, GetDiagnostics(), annotations, _tryKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTryStatement(this);
		}
	}
}

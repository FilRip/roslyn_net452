using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class KeywordEventContainerSyntax : EventContainerSyntax
	{
		internal readonly KeywordSyntax _keyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax Keyword => _keyword;

		internal KeywordEventContainerSyntax(SyntaxKind kind, KeywordSyntax keyword)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
		}

		internal KeywordEventContainerSyntax(SyntaxKind kind, KeywordSyntax keyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
		}

		internal KeywordEventContainerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
		}

		internal KeywordEventContainerSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_keyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_keyword);
		}

		static KeywordEventContainerSyntax()
		{
			CreateInstance = (ObjectReader o) => new KeywordEventContainerSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(KeywordEventContainerSyntax), (ObjectReader r) => new KeywordEventContainerSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.KeywordEventContainerSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _keyword;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new KeywordEventContainerSyntax(base.Kind, newErrors, GetAnnotations(), _keyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new KeywordEventContainerSyntax(base.Kind, GetDiagnostics(), annotations, _keyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitKeywordEventContainer(this);
		}
	}
}

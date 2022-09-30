using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class GlobalNameSyntax : NameSyntax
	{
		internal readonly KeywordSyntax _globalKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax GlobalKeyword => _globalKeyword;

		internal GlobalNameSyntax(SyntaxKind kind, KeywordSyntax globalKeyword)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(globalKeyword);
			_globalKeyword = globalKeyword;
		}

		internal GlobalNameSyntax(SyntaxKind kind, KeywordSyntax globalKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(globalKeyword);
			_globalKeyword = globalKeyword;
		}

		internal GlobalNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax globalKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(globalKeyword);
			_globalKeyword = globalKeyword;
		}

		internal GlobalNameSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_globalKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_globalKeyword);
		}

		static GlobalNameSyntax()
		{
			CreateInstance = (ObjectReader o) => new GlobalNameSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(GlobalNameSyntax), (ObjectReader r) => new GlobalNameSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.GlobalNameSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _globalKeyword;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new GlobalNameSyntax(base.Kind, newErrors, GetAnnotations(), _globalKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new GlobalNameSyntax(base.Kind, GetDiagnostics(), annotations, _globalKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitGlobalName(this);
		}
	}
}

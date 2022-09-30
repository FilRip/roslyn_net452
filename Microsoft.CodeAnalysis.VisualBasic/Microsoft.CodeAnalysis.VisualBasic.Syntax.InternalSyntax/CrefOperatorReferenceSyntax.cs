using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CrefOperatorReferenceSyntax : NameSyntax
	{
		internal readonly KeywordSyntax _operatorKeyword;

		internal readonly SyntaxToken _operatorToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax OperatorKeyword => _operatorKeyword;

		internal SyntaxToken OperatorToken => _operatorToken;

		internal CrefOperatorReferenceSyntax(SyntaxKind kind, KeywordSyntax operatorKeyword, SyntaxToken operatorToken)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(operatorKeyword);
			_operatorKeyword = operatorKeyword;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
		}

		internal CrefOperatorReferenceSyntax(SyntaxKind kind, KeywordSyntax operatorKeyword, SyntaxToken operatorToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(operatorKeyword);
			_operatorKeyword = operatorKeyword;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
		}

		internal CrefOperatorReferenceSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax operatorKeyword, SyntaxToken operatorToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(operatorKeyword);
			_operatorKeyword = operatorKeyword;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
		}

		internal CrefOperatorReferenceSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_operatorKeyword = keywordSyntax;
			}
			SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
			if (syntaxToken != null)
			{
				AdjustFlagsAndWidth(syntaxToken);
				_operatorToken = syntaxToken;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_operatorKeyword);
			writer.WriteValue(_operatorToken);
		}

		static CrefOperatorReferenceSyntax()
		{
			CreateInstance = (ObjectReader o) => new CrefOperatorReferenceSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CrefOperatorReferenceSyntax), (ObjectReader r) => new CrefOperatorReferenceSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefOperatorReferenceSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _operatorKeyword, 
				1 => _operatorToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CrefOperatorReferenceSyntax(base.Kind, newErrors, GetAnnotations(), _operatorKeyword, _operatorToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CrefOperatorReferenceSyntax(base.Kind, GetDiagnostics(), annotations, _operatorKeyword, _operatorToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCrefOperatorReference(this);
		}
	}
}

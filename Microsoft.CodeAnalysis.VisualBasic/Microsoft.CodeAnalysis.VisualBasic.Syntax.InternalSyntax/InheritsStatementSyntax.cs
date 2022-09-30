using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InheritsStatementSyntax : InheritsOrImplementsStatementSyntax
	{
		internal readonly KeywordSyntax _inheritsKeyword;

		internal readonly GreenNode _types;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax InheritsKeyword => _inheritsKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> Types => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeSyntax>(_types));

		internal InheritsStatementSyntax(SyntaxKind kind, KeywordSyntax inheritsKeyword, GreenNode types)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(inheritsKeyword);
			_inheritsKeyword = inheritsKeyword;
			if (types != null)
			{
				AdjustFlagsAndWidth(types);
				_types = types;
			}
		}

		internal InheritsStatementSyntax(SyntaxKind kind, KeywordSyntax inheritsKeyword, GreenNode types, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(inheritsKeyword);
			_inheritsKeyword = inheritsKeyword;
			if (types != null)
			{
				AdjustFlagsAndWidth(types);
				_types = types;
			}
		}

		internal InheritsStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax inheritsKeyword, GreenNode types)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(inheritsKeyword);
			_inheritsKeyword = inheritsKeyword;
			if (types != null)
			{
				AdjustFlagsAndWidth(types);
				_types = types;
			}
		}

		internal InheritsStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_inheritsKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_types = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_inheritsKeyword);
			writer.WriteValue(_types);
		}

		static InheritsStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new InheritsStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(InheritsStatementSyntax), (ObjectReader r) => new InheritsStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _inheritsKeyword, 
				1 => _types, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new InheritsStatementSyntax(base.Kind, newErrors, GetAnnotations(), _inheritsKeyword, _types);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new InheritsStatementSyntax(base.Kind, GetDiagnostics(), annotations, _inheritsKeyword, _types);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitInheritsStatement(this);
		}
	}
}

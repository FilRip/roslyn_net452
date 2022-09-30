using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ImplementsStatementSyntax : InheritsOrImplementsStatementSyntax
	{
		internal readonly KeywordSyntax _implementsKeyword;

		internal readonly GreenNode _types;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ImplementsKeyword => _implementsKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> Types => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeSyntax>(_types));

		internal ImplementsStatementSyntax(SyntaxKind kind, KeywordSyntax implementsKeyword, GreenNode types)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(implementsKeyword);
			_implementsKeyword = implementsKeyword;
			if (types != null)
			{
				AdjustFlagsAndWidth(types);
				_types = types;
			}
		}

		internal ImplementsStatementSyntax(SyntaxKind kind, KeywordSyntax implementsKeyword, GreenNode types, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(implementsKeyword);
			_implementsKeyword = implementsKeyword;
			if (types != null)
			{
				AdjustFlagsAndWidth(types);
				_types = types;
			}
		}

		internal ImplementsStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax implementsKeyword, GreenNode types)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(implementsKeyword);
			_implementsKeyword = implementsKeyword;
			if (types != null)
			{
				AdjustFlagsAndWidth(types);
				_types = types;
			}
		}

		internal ImplementsStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_implementsKeyword = keywordSyntax;
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
			writer.WriteValue(_implementsKeyword);
			writer.WriteValue(_types);
		}

		static ImplementsStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ImplementsStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ImplementsStatementSyntax), (ObjectReader r) => new ImplementsStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _implementsKeyword, 
				1 => _types, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ImplementsStatementSyntax(base.Kind, newErrors, GetAnnotations(), _implementsKeyword, _types);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ImplementsStatementSyntax(base.Kind, GetDiagnostics(), annotations, _implementsKeyword, _types);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitImplementsStatement(this);
		}
	}
}

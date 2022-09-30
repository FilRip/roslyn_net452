using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SimpleAsClauseSyntax : AsClauseSyntax
	{
		internal readonly GreenNode _attributeLists;

		internal readonly TypeSyntax _type;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(_attributeLists);

		internal TypeSyntax Type => _type;

		internal SimpleAsClauseSyntax(SyntaxKind kind, KeywordSyntax asKeyword, GreenNode attributeLists, TypeSyntax type)
			: base(kind, asKeyword)
		{
			base._slotCount = 3;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal SimpleAsClauseSyntax(SyntaxKind kind, KeywordSyntax asKeyword, GreenNode attributeLists, TypeSyntax type, ISyntaxFactoryContext context)
			: base(kind, asKeyword)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal SimpleAsClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax asKeyword, GreenNode attributeLists, TypeSyntax type)
			: base(kind, errors, annotations, asKeyword)
		{
			base._slotCount = 3;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			AdjustFlagsAndWidth(type);
			_type = type;
		}

		internal SimpleAsClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_attributeLists = greenNode;
			}
			TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
			if (typeSyntax != null)
			{
				AdjustFlagsAndWidth(typeSyntax);
				_type = typeSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_attributeLists);
			writer.WriteValue(_type);
		}

		static SimpleAsClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new SimpleAsClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SimpleAsClauseSyntax), (ObjectReader r) => new SimpleAsClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _asKeyword, 
				1 => _attributeLists, 
				2 => _type, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SimpleAsClauseSyntax(base.Kind, newErrors, GetAnnotations(), _asKeyword, _attributeLists, _type);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SimpleAsClauseSyntax(base.Kind, GetDiagnostics(), annotations, _asKeyword, _attributeLists, _type);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSimpleAsClause(this);
		}
	}
}

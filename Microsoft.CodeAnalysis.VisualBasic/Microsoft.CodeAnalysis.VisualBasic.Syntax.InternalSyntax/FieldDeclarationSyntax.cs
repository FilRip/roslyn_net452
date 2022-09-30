using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class FieldDeclarationSyntax : DeclarationStatementSyntax
	{
		internal readonly GreenNode _attributeLists;

		internal readonly GreenNode _modifiers;

		internal readonly GreenNode _declarators;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(_attributeLists);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_modifiers);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> Declarators => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VariableDeclaratorSyntax>(_declarators));

		internal FieldDeclarationSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, GreenNode declarators)
			: base(kind)
		{
			base._slotCount = 3;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			if (modifiers != null)
			{
				AdjustFlagsAndWidth(modifiers);
				_modifiers = modifiers;
			}
			if (declarators != null)
			{
				AdjustFlagsAndWidth(declarators);
				_declarators = declarators;
			}
		}

		internal FieldDeclarationSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, GreenNode declarators, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			if (modifiers != null)
			{
				AdjustFlagsAndWidth(modifiers);
				_modifiers = modifiers;
			}
			if (declarators != null)
			{
				AdjustFlagsAndWidth(declarators);
				_declarators = declarators;
			}
		}

		internal FieldDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, GreenNode declarators)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			if (modifiers != null)
			{
				AdjustFlagsAndWidth(modifiers);
				_modifiers = modifiers;
			}
			if (declarators != null)
			{
				AdjustFlagsAndWidth(declarators);
				_declarators = declarators;
			}
		}

		internal FieldDeclarationSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_attributeLists = greenNode;
			}
			GreenNode greenNode2 = (GreenNode)reader.ReadValue();
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
				_modifiers = greenNode2;
			}
			GreenNode greenNode3 = (GreenNode)reader.ReadValue();
			if (greenNode3 != null)
			{
				AdjustFlagsAndWidth(greenNode3);
				_declarators = greenNode3;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_attributeLists);
			writer.WriteValue(_modifiers);
			writer.WriteValue(_declarators);
		}

		static FieldDeclarationSyntax()
		{
			CreateInstance = (ObjectReader o) => new FieldDeclarationSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(FieldDeclarationSyntax), (ObjectReader r) => new FieldDeclarationSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldDeclarationSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _declarators, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new FieldDeclarationSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _declarators);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new FieldDeclarationSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _declarators);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitFieldDeclaration(this);
		}
	}
}

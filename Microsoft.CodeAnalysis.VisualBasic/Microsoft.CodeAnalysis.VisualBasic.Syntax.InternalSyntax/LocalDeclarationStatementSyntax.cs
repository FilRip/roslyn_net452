using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class LocalDeclarationStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly GreenNode _modifiers;

		internal readonly GreenNode _declarators;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_modifiers);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> Declarators => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VariableDeclaratorSyntax>(_declarators));

		internal LocalDeclarationStatementSyntax(SyntaxKind kind, GreenNode modifiers, GreenNode declarators)
			: base(kind)
		{
			base._slotCount = 2;
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

		internal LocalDeclarationStatementSyntax(SyntaxKind kind, GreenNode modifiers, GreenNode declarators, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
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

		internal LocalDeclarationStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode modifiers, GreenNode declarators)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
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

		internal LocalDeclarationStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_modifiers = greenNode;
			}
			GreenNode greenNode2 = (GreenNode)reader.ReadValue();
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
				_declarators = greenNode2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_modifiers);
			writer.WriteValue(_declarators);
		}

		static LocalDeclarationStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new LocalDeclarationStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(LocalDeclarationStatementSyntax), (ObjectReader r) => new LocalDeclarationStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _modifiers, 
				1 => _declarators, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new LocalDeclarationStatementSyntax(base.Kind, newErrors, GetAnnotations(), _modifiers, _declarators);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new LocalDeclarationStatementSyntax(base.Kind, GetDiagnostics(), annotations, _modifiers, _declarators);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitLocalDeclarationStatement(this);
		}
	}
}

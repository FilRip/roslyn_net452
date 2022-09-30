using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class VariableDeclaratorSyntax : VisualBasicSyntaxNode
	{
		internal readonly GreenNode _names;

		internal readonly AsClauseSyntax _asClause;

		internal readonly EqualsValueSyntax _initializer;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ModifiedIdentifierSyntax> Names => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ModifiedIdentifierSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ModifiedIdentifierSyntax>(_names));

		internal AsClauseSyntax AsClause => _asClause;

		internal EqualsValueSyntax Initializer => _initializer;

		internal VariableDeclaratorSyntax(SyntaxKind kind, GreenNode names, AsClauseSyntax asClause, EqualsValueSyntax initializer)
			: base(kind)
		{
			base._slotCount = 3;
			if (names != null)
			{
				AdjustFlagsAndWidth(names);
				_names = names;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
		}

		internal VariableDeclaratorSyntax(SyntaxKind kind, GreenNode names, AsClauseSyntax asClause, EqualsValueSyntax initializer, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (names != null)
			{
				AdjustFlagsAndWidth(names);
				_names = names;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
		}

		internal VariableDeclaratorSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode names, AsClauseSyntax asClause, EqualsValueSyntax initializer)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			if (names != null)
			{
				AdjustFlagsAndWidth(names);
				_names = names;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
		}

		internal VariableDeclaratorSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_names = greenNode;
			}
			AsClauseSyntax asClauseSyntax = (AsClauseSyntax)reader.ReadValue();
			if (asClauseSyntax != null)
			{
				AdjustFlagsAndWidth(asClauseSyntax);
				_asClause = asClauseSyntax;
			}
			EqualsValueSyntax equalsValueSyntax = (EqualsValueSyntax)reader.ReadValue();
			if (equalsValueSyntax != null)
			{
				AdjustFlagsAndWidth(equalsValueSyntax);
				_initializer = equalsValueSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_names);
			writer.WriteValue(_asClause);
			writer.WriteValue(_initializer);
		}

		static VariableDeclaratorSyntax()
		{
			CreateInstance = (ObjectReader o) => new VariableDeclaratorSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(VariableDeclaratorSyntax), (ObjectReader r) => new VariableDeclaratorSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _names, 
				1 => _asClause, 
				2 => _initializer, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new VariableDeclaratorSyntax(base.Kind, newErrors, GetAnnotations(), _names, _asClause, _initializer);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new VariableDeclaratorSyntax(base.Kind, GetDiagnostics(), annotations, _names, _asClause, _initializer);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitVariableDeclarator(this);
		}
	}
}

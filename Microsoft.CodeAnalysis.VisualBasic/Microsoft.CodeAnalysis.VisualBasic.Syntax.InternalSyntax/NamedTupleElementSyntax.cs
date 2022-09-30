using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class NamedTupleElementSyntax : TupleElementSyntax
	{
		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal NamedTupleElementSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, SimpleAsClauseSyntax asClause)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal NamedTupleElementSyntax(SyntaxKind kind, IdentifierTokenSyntax identifier, SimpleAsClauseSyntax asClause, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal NamedTupleElementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier, SimpleAsClauseSyntax asClause)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal NamedTupleElementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = (SimpleAsClauseSyntax)reader.ReadValue();
			if (simpleAsClauseSyntax != null)
			{
				AdjustFlagsAndWidth(simpleAsClauseSyntax);
				_asClause = simpleAsClauseSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_identifier);
			writer.WriteValue(_asClause);
		}

		static NamedTupleElementSyntax()
		{
			CreateInstance = (ObjectReader o) => new NamedTupleElementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(NamedTupleElementSyntax), (ObjectReader r) => new NamedTupleElementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedTupleElementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _identifier, 
				1 => _asClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new NamedTupleElementSyntax(base.Kind, newErrors, GetAnnotations(), _identifier, _asClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new NamedTupleElementSyntax(base.Kind, GetDiagnostics(), annotations, _identifier, _asClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitNamedTupleElement(this);
		}
	}
}

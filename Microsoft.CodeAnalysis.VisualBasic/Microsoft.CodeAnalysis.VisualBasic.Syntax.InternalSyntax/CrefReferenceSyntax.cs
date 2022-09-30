using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CrefReferenceSyntax : VisualBasicSyntaxNode
	{
		internal readonly TypeSyntax _name;

		internal readonly CrefSignatureSyntax _signature;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal TypeSyntax Name => _name;

		internal CrefSignatureSyntax Signature => _signature;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal CrefReferenceSyntax(SyntaxKind kind, TypeSyntax name, CrefSignatureSyntax signature, SimpleAsClauseSyntax asClause)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (signature != null)
			{
				AdjustFlagsAndWidth(signature);
				_signature = signature;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal CrefReferenceSyntax(SyntaxKind kind, TypeSyntax name, CrefSignatureSyntax signature, SimpleAsClauseSyntax asClause, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(name);
			_name = name;
			if (signature != null)
			{
				AdjustFlagsAndWidth(signature);
				_signature = signature;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal CrefReferenceSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TypeSyntax name, CrefSignatureSyntax signature, SimpleAsClauseSyntax asClause)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (signature != null)
			{
				AdjustFlagsAndWidth(signature);
				_signature = signature;
			}
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal CrefReferenceSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
			if (typeSyntax != null)
			{
				AdjustFlagsAndWidth(typeSyntax);
				_name = typeSyntax;
			}
			CrefSignatureSyntax crefSignatureSyntax = (CrefSignatureSyntax)reader.ReadValue();
			if (crefSignatureSyntax != null)
			{
				AdjustFlagsAndWidth(crefSignatureSyntax);
				_signature = crefSignatureSyntax;
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
			writer.WriteValue(_name);
			writer.WriteValue(_signature);
			writer.WriteValue(_asClause);
		}

		static CrefReferenceSyntax()
		{
			CreateInstance = (ObjectReader o) => new CrefReferenceSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CrefReferenceSyntax), (ObjectReader r) => new CrefReferenceSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CrefReferenceSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _name, 
				1 => _signature, 
				2 => _asClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CrefReferenceSyntax(base.Kind, newErrors, GetAnnotations(), _name, _signature, _asClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CrefReferenceSyntax(base.Kind, GetDiagnostics(), annotations, _name, _signature, _asClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCrefReference(this);
		}
	}
}

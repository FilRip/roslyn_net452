using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SimpleImportsClauseSyntax : ImportsClauseSyntax
	{
		internal readonly ImportAliasClauseSyntax _alias;

		internal readonly NameSyntax _name;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ImportAliasClauseSyntax Alias => _alias;

		internal NameSyntax Name => _name;

		internal SimpleImportsClauseSyntax(SyntaxKind kind, ImportAliasClauseSyntax alias, NameSyntax name)
			: base(kind)
		{
			base._slotCount = 2;
			if (alias != null)
			{
				AdjustFlagsAndWidth(alias);
				_alias = alias;
			}
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal SimpleImportsClauseSyntax(SyntaxKind kind, ImportAliasClauseSyntax alias, NameSyntax name, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			if (alias != null)
			{
				AdjustFlagsAndWidth(alias);
				_alias = alias;
			}
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal SimpleImportsClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ImportAliasClauseSyntax alias, NameSyntax name)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			if (alias != null)
			{
				AdjustFlagsAndWidth(alias);
				_alias = alias;
			}
			AdjustFlagsAndWidth(name);
			_name = name;
		}

		internal SimpleImportsClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			ImportAliasClauseSyntax importAliasClauseSyntax = (ImportAliasClauseSyntax)reader.ReadValue();
			if (importAliasClauseSyntax != null)
			{
				AdjustFlagsAndWidth(importAliasClauseSyntax);
				_alias = importAliasClauseSyntax;
			}
			NameSyntax nameSyntax = (NameSyntax)reader.ReadValue();
			if (nameSyntax != null)
			{
				AdjustFlagsAndWidth(nameSyntax);
				_name = nameSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_alias);
			writer.WriteValue(_name);
		}

		static SimpleImportsClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new SimpleImportsClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SimpleImportsClauseSyntax), (ObjectReader r) => new SimpleImportsClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleImportsClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _alias, 
				1 => _name, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SimpleImportsClauseSyntax(base.Kind, newErrors, GetAnnotations(), _alias, _name);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SimpleImportsClauseSyntax(base.Kind, GetDiagnostics(), annotations, _alias, _name);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSimpleImportsClause(this);
		}
	}
}

using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EnumMemberDeclarationSyntax : DeclarationStatementSyntax
	{
		internal readonly GreenNode _attributeLists;

		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly EqualsValueSyntax _initializer;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(_attributeLists);

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal EqualsValueSyntax Initializer => _initializer;

		internal EnumMemberDeclarationSyntax(SyntaxKind kind, GreenNode attributeLists, IdentifierTokenSyntax identifier, EqualsValueSyntax initializer)
			: base(kind)
		{
			base._slotCount = 3;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
		}

		internal EnumMemberDeclarationSyntax(SyntaxKind kind, GreenNode attributeLists, IdentifierTokenSyntax identifier, EqualsValueSyntax initializer, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
		}

		internal EnumMemberDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, IdentifierTokenSyntax identifier, EqualsValueSyntax initializer)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			if (attributeLists != null)
			{
				AdjustFlagsAndWidth(attributeLists);
				_attributeLists = attributeLists;
			}
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (initializer != null)
			{
				AdjustFlagsAndWidth(initializer);
				_initializer = initializer;
			}
		}

		internal EnumMemberDeclarationSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_attributeLists = greenNode;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
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
			writer.WriteValue(_attributeLists);
			writer.WriteValue(_identifier);
			writer.WriteValue(_initializer);
		}

		static EnumMemberDeclarationSyntax()
		{
			CreateInstance = (ObjectReader o) => new EnumMemberDeclarationSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EnumMemberDeclarationSyntax), (ObjectReader r) => new EnumMemberDeclarationSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumMemberDeclarationSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _identifier, 
				2 => _initializer, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EnumMemberDeclarationSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _identifier, _initializer);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EnumMemberDeclarationSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _identifier, _initializer);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEnumMemberDeclaration(this);
		}
	}
}

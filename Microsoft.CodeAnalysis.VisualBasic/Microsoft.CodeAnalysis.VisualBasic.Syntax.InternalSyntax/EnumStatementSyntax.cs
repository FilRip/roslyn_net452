using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EnumStatementSyntax : DeclarationStatementSyntax
	{
		internal readonly GreenNode _attributeLists;

		internal readonly GreenNode _modifiers;

		internal readonly KeywordSyntax _enumKeyword;

		internal readonly IdentifierTokenSyntax _identifier;

		internal readonly AsClauseSyntax _underlyingType;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(_attributeLists);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>(_modifiers);

		internal KeywordSyntax EnumKeyword => _enumKeyword;

		internal IdentifierTokenSyntax Identifier => _identifier;

		internal AsClauseSyntax UnderlyingType => _underlyingType;

		internal EnumStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax enumKeyword, IdentifierTokenSyntax identifier, AsClauseSyntax underlyingType)
			: base(kind)
		{
			base._slotCount = 5;
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
			AdjustFlagsAndWidth(enumKeyword);
			_enumKeyword = enumKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (underlyingType != null)
			{
				AdjustFlagsAndWidth(underlyingType);
				_underlyingType = underlyingType;
			}
		}

		internal EnumStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax enumKeyword, IdentifierTokenSyntax identifier, AsClauseSyntax underlyingType, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
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
			AdjustFlagsAndWidth(enumKeyword);
			_enumKeyword = enumKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (underlyingType != null)
			{
				AdjustFlagsAndWidth(underlyingType);
				_underlyingType = underlyingType;
			}
		}

		internal EnumStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax enumKeyword, IdentifierTokenSyntax identifier, AsClauseSyntax underlyingType)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
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
			AdjustFlagsAndWidth(enumKeyword);
			_enumKeyword = enumKeyword;
			AdjustFlagsAndWidth(identifier);
			_identifier = identifier;
			if (underlyingType != null)
			{
				AdjustFlagsAndWidth(underlyingType);
				_underlyingType = underlyingType;
			}
		}

		internal EnumStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
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
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_enumKeyword = keywordSyntax;
			}
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_identifier = identifierTokenSyntax;
			}
			AsClauseSyntax asClauseSyntax = (AsClauseSyntax)reader.ReadValue();
			if (asClauseSyntax != null)
			{
				AdjustFlagsAndWidth(asClauseSyntax);
				_underlyingType = asClauseSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_attributeLists);
			writer.WriteValue(_modifiers);
			writer.WriteValue(_enumKeyword);
			writer.WriteValue(_identifier);
			writer.WriteValue(_underlyingType);
		}

		static EnumStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new EnumStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EnumStatementSyntax), (ObjectReader r) => new EnumStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _enumKeyword, 
				3 => _identifier, 
				4 => _underlyingType, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EnumStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _enumKeyword, _identifier, _underlyingType);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EnumStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _enumKeyword, _identifier, _underlyingType);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEnumStatement(this);
		}
	}
}

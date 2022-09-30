using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class StructureStatementSyntax : TypeStatementSyntax
	{
		internal readonly KeywordSyntax _structureKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax StructureKeyword => _structureKeyword;

		internal StructureStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax structureKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: base(kind, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(structureKeyword);
			_structureKeyword = structureKeyword;
		}

		internal StructureStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax structureKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(structureKeyword);
			_structureKeyword = structureKeyword;
		}

		internal StructureStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax structureKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: base(kind, errors, annotations, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(structureKeyword);
			_structureKeyword = structureKeyword;
		}

		internal StructureStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_structureKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_structureKeyword);
		}

		static StructureStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new StructureStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(StructureStatementSyntax), (ObjectReader r) => new StructureStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _structureKeyword, 
				3 => _identifier, 
				4 => _typeParameterList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new StructureStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _structureKeyword, _identifier, _typeParameterList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new StructureStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _structureKeyword, _identifier, _typeParameterList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitStructureStatement(this);
		}
	}
}

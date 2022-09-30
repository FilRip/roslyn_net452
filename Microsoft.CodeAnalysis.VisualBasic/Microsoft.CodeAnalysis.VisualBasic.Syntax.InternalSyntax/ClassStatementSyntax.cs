using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ClassStatementSyntax : TypeStatementSyntax
	{
		internal readonly KeywordSyntax _classKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ClassKeyword => _classKeyword;

		internal ClassStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax classKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: base(kind, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(classKeyword);
			_classKeyword = classKeyword;
		}

		internal ClassStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax classKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(classKeyword);
			_classKeyword = classKeyword;
		}

		internal ClassStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax classKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: base(kind, errors, annotations, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(classKeyword);
			_classKeyword = classKeyword;
		}

		internal ClassStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_classKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_classKeyword);
		}

		static ClassStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ClassStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ClassStatementSyntax), (ObjectReader r) => new ClassStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _classKeyword, 
				3 => _identifier, 
				4 => _typeParameterList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ClassStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _classKeyword, _identifier, _typeParameterList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ClassStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _classKeyword, _identifier, _typeParameterList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitClassStatement(this);
		}
	}
}

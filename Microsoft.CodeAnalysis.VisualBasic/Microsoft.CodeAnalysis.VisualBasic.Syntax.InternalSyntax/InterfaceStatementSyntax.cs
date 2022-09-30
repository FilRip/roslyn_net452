using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InterfaceStatementSyntax : TypeStatementSyntax
	{
		internal readonly KeywordSyntax _interfaceKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax InterfaceKeyword => _interfaceKeyword;

		internal InterfaceStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax interfaceKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: base(kind, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(interfaceKeyword);
			_interfaceKeyword = interfaceKeyword;
		}

		internal InterfaceStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax interfaceKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(interfaceKeyword);
			_interfaceKeyword = interfaceKeyword;
		}

		internal InterfaceStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax interfaceKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: base(kind, errors, annotations, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(interfaceKeyword);
			_interfaceKeyword = interfaceKeyword;
		}

		internal InterfaceStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_interfaceKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_interfaceKeyword);
		}

		static InterfaceStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new InterfaceStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(InterfaceStatementSyntax), (ObjectReader r) => new InterfaceStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _interfaceKeyword, 
				3 => _identifier, 
				4 => _typeParameterList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new InterfaceStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _interfaceKeyword, _identifier, _typeParameterList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new InterfaceStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _interfaceKeyword, _identifier, _typeParameterList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitInterfaceStatement(this);
		}
	}
}

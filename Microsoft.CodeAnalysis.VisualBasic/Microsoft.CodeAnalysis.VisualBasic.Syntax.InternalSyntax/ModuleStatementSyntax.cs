using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ModuleStatementSyntax : TypeStatementSyntax
	{
		internal readonly KeywordSyntax _moduleKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax ModuleKeyword => _moduleKeyword;

		internal ModuleStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax moduleKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: base(kind, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(moduleKeyword);
			_moduleKeyword = moduleKeyword;
		}

		internal ModuleStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax moduleKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(moduleKeyword);
			_moduleKeyword = moduleKeyword;
		}

		internal ModuleStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax moduleKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: base(kind, errors, annotations, attributeLists, modifiers, identifier, typeParameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(moduleKeyword);
			_moduleKeyword = moduleKeyword;
		}

		internal ModuleStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_moduleKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_moduleKeyword);
		}

		static ModuleStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ModuleStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ModuleStatementSyntax), (ObjectReader r) => new ModuleStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _moduleKeyword, 
				3 => _identifier, 
				4 => _typeParameterList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ModuleStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _moduleKeyword, _identifier, _typeParameterList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ModuleStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _moduleKeyword, _identifier, _typeParameterList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitModuleStatement(this);
		}
	}
}

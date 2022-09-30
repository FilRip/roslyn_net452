using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class AccessorStatementSyntax : MethodBaseSyntax
	{
		internal readonly KeywordSyntax _accessorKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax AccessorKeyword => _accessorKeyword;

		internal AccessorStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(accessorKeyword);
			_accessorKeyword = accessorKeyword;
		}

		internal AccessorStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(accessorKeyword);
			_accessorKeyword = accessorKeyword;
		}

		internal AccessorStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
			: base(kind, errors, annotations, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(accessorKeyword);
			_accessorKeyword = accessorKeyword;
		}

		internal AccessorStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_accessorKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_accessorKeyword);
		}

		static AccessorStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new AccessorStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(AccessorStatementSyntax), (ObjectReader r) => new AccessorStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _accessorKeyword, 
				3 => _parameterList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new AccessorStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _accessorKeyword, _parameterList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new AccessorStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _accessorKeyword, _parameterList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitAccessorStatement(this);
		}
	}
}

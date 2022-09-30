using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SubNewStatementSyntax : MethodBaseSyntax
	{
		internal readonly KeywordSyntax _subKeyword;

		internal readonly KeywordSyntax _newKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax SubKeyword => _subKeyword;

		internal KeywordSyntax NewKeyword => _newKeyword;

		internal SubNewStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax subKeyword, KeywordSyntax newKeyword, ParameterListSyntax parameterList)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(subKeyword);
			_subKeyword = subKeyword;
			AdjustFlagsAndWidth(newKeyword);
			_newKeyword = newKeyword;
		}

		internal SubNewStatementSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax subKeyword, KeywordSyntax newKeyword, ParameterListSyntax parameterList, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(subKeyword);
			_subKeyword = subKeyword;
			AdjustFlagsAndWidth(newKeyword);
			_newKeyword = newKeyword;
		}

		internal SubNewStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax subKeyword, KeywordSyntax newKeyword, ParameterListSyntax parameterList)
			: base(kind, errors, annotations, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(subKeyword);
			_subKeyword = subKeyword;
			AdjustFlagsAndWidth(newKeyword);
			_newKeyword = newKeyword;
		}

		internal SubNewStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_subKeyword = keywordSyntax;
			}
			KeywordSyntax keywordSyntax2 = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax2 != null)
			{
				AdjustFlagsAndWidth(keywordSyntax2);
				_newKeyword = keywordSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_subKeyword);
			writer.WriteValue(_newKeyword);
		}

		static SubNewStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new SubNewStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SubNewStatementSyntax), (ObjectReader r) => new SubNewStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _subKeyword, 
				3 => _newKeyword, 
				4 => _parameterList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SubNewStatementSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _subKeyword, _newKeyword, _parameterList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SubNewStatementSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _subKeyword, _newKeyword, _parameterList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitSubNewStatement(this);
		}
	}
}

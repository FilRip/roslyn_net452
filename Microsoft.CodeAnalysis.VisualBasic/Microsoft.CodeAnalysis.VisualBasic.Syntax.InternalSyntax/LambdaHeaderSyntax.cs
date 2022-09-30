using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class LambdaHeaderSyntax : MethodBaseSyntax
	{
		internal readonly KeywordSyntax _subOrFunctionKeyword;

		internal readonly SimpleAsClauseSyntax _asClause;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax SubOrFunctionKeyword => _subOrFunctionKeyword;

		internal SimpleAsClauseSyntax AsClause => _asClause;

		internal LambdaHeaderSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(subOrFunctionKeyword);
			_subOrFunctionKeyword = subOrFunctionKeyword;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal LambdaHeaderSyntax(SyntaxKind kind, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ISyntaxFactoryContext context)
			: base(kind, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(subOrFunctionKeyword);
			_subOrFunctionKeyword = subOrFunctionKeyword;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal LambdaHeaderSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode attributeLists, GreenNode modifiers, KeywordSyntax subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: base(kind, errors, annotations, attributeLists, modifiers, parameterList)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(subOrFunctionKeyword);
			_subOrFunctionKeyword = subOrFunctionKeyword;
			if (asClause != null)
			{
				AdjustFlagsAndWidth(asClause);
				_asClause = asClause;
			}
		}

		internal LambdaHeaderSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_subOrFunctionKeyword = keywordSyntax;
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
			writer.WriteValue(_subOrFunctionKeyword);
			writer.WriteValue(_asClause);
		}

		static LambdaHeaderSyntax()
		{
			CreateInstance = (ObjectReader o) => new LambdaHeaderSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(LambdaHeaderSyntax), (ObjectReader r) => new LambdaHeaderSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				1 => _modifiers, 
				2 => _subOrFunctionKeyword, 
				3 => _parameterList, 
				4 => _asClause, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new LambdaHeaderSyntax(base.Kind, newErrors, GetAnnotations(), _attributeLists, _modifiers, _subOrFunctionKeyword, _parameterList, _asClause);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new LambdaHeaderSyntax(base.Kind, GetDiagnostics(), annotations, _attributeLists, _modifiers, _subOrFunctionKeyword, _parameterList, _asClause);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitLambdaHeader(this);
		}
	}
}

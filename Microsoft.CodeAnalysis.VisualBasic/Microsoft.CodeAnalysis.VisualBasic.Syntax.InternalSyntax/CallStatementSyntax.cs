using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CallStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _callKeyword;

		internal readonly ExpressionSyntax _invocation;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax CallKeyword => _callKeyword;

		internal ExpressionSyntax Invocation => _invocation;

		internal CallStatementSyntax(SyntaxKind kind, KeywordSyntax callKeyword, ExpressionSyntax invocation)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(callKeyword);
			_callKeyword = callKeyword;
			AdjustFlagsAndWidth(invocation);
			_invocation = invocation;
		}

		internal CallStatementSyntax(SyntaxKind kind, KeywordSyntax callKeyword, ExpressionSyntax invocation, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(callKeyword);
			_callKeyword = callKeyword;
			AdjustFlagsAndWidth(invocation);
			_invocation = invocation;
		}

		internal CallStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax callKeyword, ExpressionSyntax invocation)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(callKeyword);
			_callKeyword = callKeyword;
			AdjustFlagsAndWidth(invocation);
			_invocation = invocation;
		}

		internal CallStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_callKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_invocation = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_callKeyword);
			writer.WriteValue(_invocation);
		}

		static CallStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new CallStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CallStatementSyntax), (ObjectReader r) => new CallStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CallStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _callKeyword, 
				1 => _invocation, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CallStatementSyntax(base.Kind, newErrors, GetAnnotations(), _callKeyword, _invocation);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CallStatementSyntax(base.Kind, GetDiagnostics(), annotations, _callKeyword, _invocation);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCallStatement(this);
		}
	}
}

using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class FunctionAggregationSyntax : AggregationSyntax
	{
		internal readonly IdentifierTokenSyntax _functionName;

		internal readonly PunctuationSyntax _openParenToken;

		internal readonly ExpressionSyntax _argument;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal IdentifierTokenSyntax FunctionName => _functionName;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal ExpressionSyntax Argument => _argument;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal FunctionAggregationSyntax(SyntaxKind kind, IdentifierTokenSyntax functionName, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(functionName);
			_functionName = functionName;
			if (openParenToken != null)
			{
				AdjustFlagsAndWidth(openParenToken);
				_openParenToken = openParenToken;
			}
			if (argument != null)
			{
				AdjustFlagsAndWidth(argument);
				_argument = argument;
			}
			if (closeParenToken != null)
			{
				AdjustFlagsAndWidth(closeParenToken);
				_closeParenToken = closeParenToken;
			}
		}

		internal FunctionAggregationSyntax(SyntaxKind kind, IdentifierTokenSyntax functionName, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(functionName);
			_functionName = functionName;
			if (openParenToken != null)
			{
				AdjustFlagsAndWidth(openParenToken);
				_openParenToken = openParenToken;
			}
			if (argument != null)
			{
				AdjustFlagsAndWidth(argument);
				_argument = argument;
			}
			if (closeParenToken != null)
			{
				AdjustFlagsAndWidth(closeParenToken);
				_closeParenToken = closeParenToken;
			}
		}

		internal FunctionAggregationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax functionName, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(functionName);
			_functionName = functionName;
			if (openParenToken != null)
			{
				AdjustFlagsAndWidth(openParenToken);
				_openParenToken = openParenToken;
			}
			if (argument != null)
			{
				AdjustFlagsAndWidth(argument);
				_argument = argument;
			}
			if (closeParenToken != null)
			{
				AdjustFlagsAndWidth(closeParenToken);
				_closeParenToken = closeParenToken;
			}
		}

		internal FunctionAggregationSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			IdentifierTokenSyntax identifierTokenSyntax = (IdentifierTokenSyntax)reader.ReadValue();
			if (identifierTokenSyntax != null)
			{
				AdjustFlagsAndWidth(identifierTokenSyntax);
				_functionName = identifierTokenSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openParenToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_argument = expressionSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_closeParenToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_functionName);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_argument);
			writer.WriteValue(_closeParenToken);
		}

		static FunctionAggregationSyntax()
		{
			CreateInstance = (ObjectReader o) => new FunctionAggregationSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(FunctionAggregationSyntax), (ObjectReader r) => new FunctionAggregationSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.FunctionAggregationSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _functionName, 
				1 => _openParenToken, 
				2 => _argument, 
				3 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new FunctionAggregationSyntax(base.Kind, newErrors, GetAnnotations(), _functionName, _openParenToken, _argument, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new FunctionAggregationSyntax(base.Kind, GetDiagnostics(), annotations, _functionName, _openParenToken, _argument, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitFunctionAggregation(this);
		}
	}
}

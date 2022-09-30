using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundArrayCreation : BoundExpression
	{
		private readonly bool _IsParamArrayArgument;

		private readonly ImmutableArray<BoundExpression> _Bounds;

		private readonly BoundArrayInitialization _InitializerOpt;

		private readonly BoundArrayLiteral _ArrayLiteralOpt;

		private readonly ConversionKind _ArrayLiteralConversion;

		public bool IsParamArrayArgument => _IsParamArrayArgument;

		public ImmutableArray<BoundExpression> Bounds => _Bounds;

		public BoundArrayInitialization InitializerOpt => _InitializerOpt;

		public BoundArrayLiteral ArrayLiteralOpt => _ArrayLiteralOpt;

		public ConversionKind ArrayLiteralConversion => _ArrayLiteralConversion;

		public BoundArrayCreation(SyntaxNode syntax, ImmutableArray<BoundExpression> bounds, BoundArrayInitialization initializerOpt, TypeSymbol type, bool hasErrors = false)
			: this(syntax, isParamArrayArgument: false, bounds, initializerOpt, null, ConversionKind.DelegateRelaxationLevelNone, type, hasErrors)
		{
		}

		public BoundArrayCreation(SyntaxNode syntax, ImmutableArray<BoundExpression> bounds, BoundArrayInitialization initializerOpt, BoundArrayLiteral arrayLiteralOpt, ConversionKind arrayLiteralConversion, TypeSymbol type, bool hasErrors = false)
			: this(syntax, isParamArrayArgument: false, bounds, initializerOpt, arrayLiteralOpt, arrayLiteralConversion, type, hasErrors)
		{
		}

		public BoundArrayCreation(SyntaxNode syntax, bool isParamArrayArgument, ImmutableArray<BoundExpression> bounds, BoundArrayInitialization initializerOpt, BoundArrayLiteral arrayLiteralOpt, ConversionKind arrayLiteralConversion, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ArrayCreation, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(bounds) || BoundNodeExtensions.NonNullAndHasErrors(initializerOpt) || BoundNodeExtensions.NonNullAndHasErrors(arrayLiteralOpt))
		{
			_IsParamArrayArgument = isParamArrayArgument;
			_Bounds = bounds;
			_InitializerOpt = initializerOpt;
			_ArrayLiteralOpt = arrayLiteralOpt;
			_ArrayLiteralConversion = arrayLiteralConversion;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitArrayCreation(this);
		}

		public BoundArrayCreation Update(bool isParamArrayArgument, ImmutableArray<BoundExpression> bounds, BoundArrayInitialization initializerOpt, BoundArrayLiteral arrayLiteralOpt, ConversionKind arrayLiteralConversion, TypeSymbol type)
		{
			if (isParamArrayArgument != IsParamArrayArgument || bounds != Bounds || initializerOpt != InitializerOpt || arrayLiteralOpt != ArrayLiteralOpt || arrayLiteralConversion != ArrayLiteralConversion || (object)type != base.Type)
			{
				BoundArrayCreation boundArrayCreation = new BoundArrayCreation(base.Syntax, isParamArrayArgument, bounds, initializerOpt, arrayLiteralOpt, arrayLiteralConversion, type, base.HasErrors);
				boundArrayCreation.CopyAttributes(this);
				return boundArrayCreation;
			}
			return this;
		}
	}
}

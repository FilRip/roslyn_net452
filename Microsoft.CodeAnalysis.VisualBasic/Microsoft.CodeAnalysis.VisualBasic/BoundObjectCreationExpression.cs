using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundObjectCreationExpression : BoundObjectCreationExpressionBase
	{
		private readonly MethodSymbol _ConstructorOpt;

		private readonly BoundMethodGroup _MethodGroupOpt;

		private readonly ImmutableArray<BoundExpression> _Arguments;

		private readonly BitVector _DefaultArguments;

		public override Symbol ExpressionSymbol => ConstructorOpt;

		public MethodSymbol ConstructorOpt => _ConstructorOpt;

		public BoundMethodGroup MethodGroupOpt => _MethodGroupOpt;

		public ImmutableArray<BoundExpression> Arguments => _Arguments;

		public BitVector DefaultArguments => _DefaultArguments;

		public BoundObjectCreationExpression(SyntaxNode syntax, MethodSymbol constructorOpt, ImmutableArray<BoundExpression> arguments, BoundObjectInitializerExpressionBase initializerOpt, TypeSymbol type, bool hasErrors = false, BitVector defaultArguments = default(BitVector))
			: this(syntax, constructorOpt, null, arguments, defaultArguments, initializerOpt, type, hasErrors)
		{
		}

		public BoundObjectCreationExpression Update(MethodSymbol constructorOpt, ImmutableArray<BoundExpression> arguments, BitVector defaultArguments, BoundObjectInitializerExpressionBase initializerOpt, TypeSymbol type)
		{
			return Update(constructorOpt, null, arguments, defaultArguments, initializerOpt, type);
		}

		public BoundObjectCreationExpression(SyntaxNode syntax, MethodSymbol constructorOpt, BoundMethodGroup methodGroupOpt, ImmutableArray<BoundExpression> arguments, BitVector defaultArguments, BoundObjectInitializerExpressionBase initializerOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ObjectCreationExpression, syntax, initializerOpt, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(methodGroupOpt) || BoundNodeExtensions.NonNullAndHasErrors(arguments) || BoundNodeExtensions.NonNullAndHasErrors(initializerOpt))
		{
			_ConstructorOpt = constructorOpt;
			_MethodGroupOpt = methodGroupOpt;
			_Arguments = arguments;
			_DefaultArguments = defaultArguments;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitObjectCreationExpression(this);
		}

		public BoundObjectCreationExpression Update(MethodSymbol constructorOpt, BoundMethodGroup methodGroupOpt, ImmutableArray<BoundExpression> arguments, BitVector defaultArguments, BoundObjectInitializerExpressionBase initializerOpt, TypeSymbol type)
		{
			if ((object)constructorOpt != ConstructorOpt || methodGroupOpt != MethodGroupOpt || arguments != Arguments || defaultArguments != DefaultArguments || initializerOpt != base.InitializerOpt || (object)type != base.Type)
			{
				BoundObjectCreationExpression boundObjectCreationExpression = new BoundObjectCreationExpression(base.Syntax, constructorOpt, methodGroupOpt, arguments, defaultArguments, initializerOpt, type, base.HasErrors);
				boundObjectCreationExpression.CopyAttributes(this);
				return boundObjectCreationExpression;
			}
			return this;
		}
	}
}

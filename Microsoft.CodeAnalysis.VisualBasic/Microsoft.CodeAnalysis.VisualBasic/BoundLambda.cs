using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLambda : BoundExpression
	{
		private readonly LambdaSymbol _LambdaSymbol;

		private readonly BoundBlock _Body;

		private readonly ImmutableBindingDiagnostic<AssemblySymbol> _Diagnostics;

		private readonly LambdaBodyBinder _LambdaBinderOpt;

		private readonly ConversionKind _DelegateRelaxation;

		private readonly MethodConversionKind _MethodConversionKind;

		public bool IsSingleLine
		{
			get
			{
				SyntaxKind syntaxKind = VisualBasicExtensions.Kind(base.Syntax);
				if (syntaxKind != SyntaxKind.SingleLineFunctionLambdaExpression)
				{
					return syntaxKind == SyntaxKind.SingleLineSubLambdaExpression;
				}
				return true;
			}
		}

		public override Symbol ExpressionSymbol => LambdaSymbol;

		public LambdaSymbol LambdaSymbol => _LambdaSymbol;

		public BoundBlock Body => _Body;

		public ImmutableBindingDiagnostic<AssemblySymbol> Diagnostics => _Diagnostics;

		public LambdaBodyBinder LambdaBinderOpt => _LambdaBinderOpt;

		public ConversionKind DelegateRelaxation => _DelegateRelaxation;

		public MethodConversionKind MethodConversionKind => _MethodConversionKind;

		public BoundLambda(SyntaxNode syntax, LambdaSymbol lambdaSymbol, BoundBlock body, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics, LambdaBodyBinder lambdaBinderOpt, ConversionKind delegateRelaxation, MethodConversionKind methodConversionKind, bool hasErrors = false)
			: base(BoundKind.Lambda, syntax, null, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(body))
		{
			_LambdaSymbol = lambdaSymbol;
			_Body = body;
			_Diagnostics = diagnostics;
			_LambdaBinderOpt = lambdaBinderOpt;
			_DelegateRelaxation = delegateRelaxation;
			_MethodConversionKind = methodConversionKind;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLambda(this);
		}

		public BoundLambda Update(LambdaSymbol lambdaSymbol, BoundBlock body, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics, LambdaBodyBinder lambdaBinderOpt, ConversionKind delegateRelaxation, MethodConversionKind methodConversionKind)
		{
			if ((object)lambdaSymbol != LambdaSymbol || body != Body || diagnostics != Diagnostics || lambdaBinderOpt != LambdaBinderOpt || delegateRelaxation != DelegateRelaxation || methodConversionKind != MethodConversionKind)
			{
				BoundLambda boundLambda = new BoundLambda(base.Syntax, lambdaSymbol, body, diagnostics, lambdaBinderOpt, delegateRelaxation, methodConversionKind, base.HasErrors);
				boundLambda.CopyAttributes(this);
				return boundLambda;
			}
			return this;
		}
	}
}

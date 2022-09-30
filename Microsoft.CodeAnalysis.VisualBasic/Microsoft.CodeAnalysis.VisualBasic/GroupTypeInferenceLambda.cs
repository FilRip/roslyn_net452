using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class GroupTypeInferenceLambda : BoundExpression
	{
		private readonly Binder _Binder;

		private readonly ImmutableArray<ParameterSymbol> _Parameters;

		private readonly VisualBasicCompilation _Compilation;

		public Binder Binder => _Binder;

		public ImmutableArray<ParameterSymbol> Parameters => _Parameters;

		public VisualBasicCompilation Compilation => _Compilation;

		public TypeSymbol InferLambdaReturnType(ImmutableArray<ParameterSymbol> delegateParams)
		{
			if (delegateParams.Length != 2)
			{
				return null;
			}
			return Compilation.AnonymousTypeManager.ConstructAnonymousTypeSymbol(new AnonymousTypeDescriptor(ImmutableArray.Create(new AnonymousTypeField("$VB$ItAnonymous", delegateParams[1].Type, SyntaxNodeExtensions.QueryClauseKeywordOrRangeVariableIdentifier(base.Syntax).GetLocation(), isKeyOrByRef: true)), SyntaxNodeExtensions.QueryClauseKeywordOrRangeVariableIdentifier(base.Syntax).GetLocation(), isImplicitlyDeclared: true));
		}

		public GroupTypeInferenceLambda(SyntaxNode syntax, Binder binder, ImmutableArray<ParameterSymbol> parameters, VisualBasicCompilation compilation, bool hasErrors)
			: base(BoundKind.GroupTypeInferenceLambda, syntax, null, hasErrors)
		{
			_Binder = binder;
			_Parameters = parameters;
			_Compilation = compilation;
		}

		public GroupTypeInferenceLambda(SyntaxNode syntax, Binder binder, ImmutableArray<ParameterSymbol> parameters, VisualBasicCompilation compilation)
			: base(BoundKind.GroupTypeInferenceLambda, syntax, null)
		{
			_Binder = binder;
			_Parameters = parameters;
			_Compilation = compilation;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitGroupTypeInferenceLambda(this);
		}

		public GroupTypeInferenceLambda Update(Binder binder, ImmutableArray<ParameterSymbol> parameters, VisualBasicCompilation compilation)
		{
			if (binder != Binder || parameters != Parameters || compilation != Compilation)
			{
				GroupTypeInferenceLambda groupTypeInferenceLambda = new GroupTypeInferenceLambda(base.Syntax, binder, parameters, compilation, base.HasErrors);
				groupTypeInferenceLambda.CopyAttributes(this);
				return groupTypeInferenceLambda;
			}
			return this;
		}
	}
}

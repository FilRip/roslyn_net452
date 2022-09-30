using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLocalDeclaration : BoundLocalDeclarationBase, IBoundLocalDeclarations
	{
		private readonly LocalSymbol _LocalSymbol;

		private readonly BoundExpression _DeclarationInitializerOpt;

		private readonly BoundArrayCreation _IdentifierInitializerOpt;

		private readonly bool _InitializedByAsNew;

		public BoundExpression InitializerOpt => DeclarationInitializerOpt ?? IdentifierInitializerOpt;

		private ImmutableArray<BoundLocalDeclarationBase> IBoundLocalDeclarations_Declarations => ImmutableArray.Create((BoundLocalDeclarationBase)this);

		public LocalSymbol LocalSymbol => _LocalSymbol;

		public BoundExpression DeclarationInitializerOpt => _DeclarationInitializerOpt;

		public BoundArrayCreation IdentifierInitializerOpt => _IdentifierInitializerOpt;

		public bool InitializedByAsNew => _InitializedByAsNew;

		public BoundLocalDeclaration(SyntaxNode syntax, LocalSymbol localSymbol, BoundExpression initializerOpt)
			: this(syntax, localSymbol, initializerOpt, null, initializedByAsNew: false)
		{
		}

		public BoundLocalDeclaration(SyntaxNode syntax, LocalSymbol localSymbol, BoundExpression declarationInitializerOpt, BoundArrayCreation identifierInitializerOpt, bool initializedByAsNew, bool hasErrors = false)
			: base(BoundKind.LocalDeclaration, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(declarationInitializerOpt) || BoundNodeExtensions.NonNullAndHasErrors(identifierInitializerOpt))
		{
			_LocalSymbol = localSymbol;
			_DeclarationInitializerOpt = declarationInitializerOpt;
			_IdentifierInitializerOpt = identifierInitializerOpt;
			_InitializedByAsNew = initializedByAsNew;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLocalDeclaration(this);
		}

		public BoundLocalDeclaration Update(LocalSymbol localSymbol, BoundExpression declarationInitializerOpt, BoundArrayCreation identifierInitializerOpt, bool initializedByAsNew)
		{
			if ((object)localSymbol != LocalSymbol || declarationInitializerOpt != DeclarationInitializerOpt || identifierInitializerOpt != IdentifierInitializerOpt || initializedByAsNew != InitializedByAsNew)
			{
				BoundLocalDeclaration boundLocalDeclaration = new BoundLocalDeclaration(base.Syntax, localSymbol, declarationInitializerOpt, identifierInitializerOpt, initializedByAsNew, base.HasErrors);
				boundLocalDeclaration.CopyAttributes(this);
				return boundLocalDeclaration;
			}
			return this;
		}
	}
}

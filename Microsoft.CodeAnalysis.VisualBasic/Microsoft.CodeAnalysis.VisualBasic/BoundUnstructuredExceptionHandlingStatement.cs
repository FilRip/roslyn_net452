using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundUnstructuredExceptionHandlingStatement : BoundStatement
	{
		private readonly bool _ContainsOnError;

		private readonly bool _ContainsResume;

		private readonly StatementSyntax _ResumeWithoutLabelOpt;

		private readonly bool _TrackLineNumber;

		private readonly BoundBlock _Body;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Body);

		public bool ContainsOnError => _ContainsOnError;

		public bool ContainsResume => _ContainsResume;

		public StatementSyntax ResumeWithoutLabelOpt => _ResumeWithoutLabelOpt;

		public bool TrackLineNumber => _TrackLineNumber;

		public BoundBlock Body => _Body;

		public BoundUnstructuredExceptionHandlingStatement(SyntaxNode syntax, bool containsOnError, bool containsResume, StatementSyntax resumeWithoutLabelOpt, bool trackLineNumber, BoundBlock body, bool hasErrors = false)
			: base(BoundKind.UnstructuredExceptionHandlingStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(body))
		{
			_ContainsOnError = containsOnError;
			_ContainsResume = containsResume;
			_ResumeWithoutLabelOpt = resumeWithoutLabelOpt;
			_TrackLineNumber = trackLineNumber;
			_Body = body;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUnstructuredExceptionHandlingStatement(this);
		}

		public BoundUnstructuredExceptionHandlingStatement Update(bool containsOnError, bool containsResume, StatementSyntax resumeWithoutLabelOpt, bool trackLineNumber, BoundBlock body)
		{
			if (containsOnError != ContainsOnError || containsResume != ContainsResume || resumeWithoutLabelOpt != ResumeWithoutLabelOpt || trackLineNumber != TrackLineNumber || body != Body)
			{
				BoundUnstructuredExceptionHandlingStatement boundUnstructuredExceptionHandlingStatement = new BoundUnstructuredExceptionHandlingStatement(base.Syntax, containsOnError, containsResume, resumeWithoutLabelOpt, trackLineNumber, body, base.HasErrors);
				boundUnstructuredExceptionHandlingStatement.CopyAttributes(this);
				return boundUnstructuredExceptionHandlingStatement;
			}
			return this;
		}
	}
}

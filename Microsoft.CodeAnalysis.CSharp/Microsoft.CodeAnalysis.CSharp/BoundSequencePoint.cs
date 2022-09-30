using System.Diagnostics;

using Microsoft.CodeAnalysis.Text;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSequencePoint : BoundStatement
    {
        public BoundStatement? StatementOpt { get; }

        public static BoundStatement Create(SyntaxNode? syntax, TextSpan? part, BoundStatement statement, bool hasErrors = false)
        {
            if (part.HasValue)
            {
                return new BoundSequencePointWithSpan(syntax, statement, part.Value, hasErrors);
            }
            return new BoundSequencePoint(syntax, statement, hasErrors);
        }

        public static BoundStatement Create(SyntaxNode? syntax, BoundStatement? statementOpt, bool hasErrors = false, bool wasCompilerGenerated = false)
        {
            return new BoundSequencePoint(syntax, statementOpt, hasErrors)
            {
                WasCompilerGenerated = wasCompilerGenerated
            };
        }

        public static BoundStatement CreateHidden(BoundStatement? statementOpt = null, bool hasErrors = false)
        {
            return new BoundSequencePoint(null, statementOpt, hasErrors)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundSequencePoint(SyntaxNode syntax, BoundStatement? statementOpt, bool hasErrors = false)
            : base(BoundKind.SequencePoint, syntax, hasErrors || statementOpt.HasErrors())
        {
            StatementOpt = statementOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSequencePoint(this);
        }

        public BoundSequencePoint Update(BoundStatement? statementOpt)
        {
            if (statementOpt != StatementOpt)
            {
                BoundSequencePoint boundSequencePoint = new BoundSequencePoint(Syntax, statementOpt, base.HasErrors);
                boundSequencePoint.CopyAttributes(this);
                return boundSequencePoint;
            }
            return this;
        }
    }
}

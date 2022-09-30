using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundUnconvertedObjectCreationExpression : BoundExpression
    {
        public override object Display
        {
            get
            {
                ImmutableArray<BoundExpression> arguments = Arguments;
                if (arguments.Length == 0)
                {
                    return "new()";
                }
                PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                StringBuilder builder = instance.Builder;
                object[] array = new object[arguments.Length];
                builder.Append("new");
                builder.Append('(');
                builder.Append("{0}");
                array[0] = arguments[0].Display;
                for (int i = 1; i < arguments.Length; i++)
                {
                    builder.Append(", {" + i + "}");
                    array[i] = arguments[i].Display;
                }
                builder.Append(')');
                return instance.ToStringAndFree(); // FilRip : Remove FormattageStringFactory
            }
        }

        public new TypeSymbol? Type => base.Type;

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<IdentifierNameSyntax> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public InitializerExpressionSyntax? InitializerOpt { get; }

        public BoundUnconvertedObjectCreationExpression(SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, ImmutableArray<IdentifierNameSyntax> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, InitializerExpressionSyntax? initializerOpt, bool hasErrors = false)
            : base(BoundKind.UnconvertedObjectCreationExpression, syntax, null, hasErrors || arguments.HasErrors())
        {
            Arguments = arguments;
            ArgumentNamesOpt = argumentNamesOpt;
            ArgumentRefKindsOpt = argumentRefKindsOpt;
            InitializerOpt = initializerOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitUnconvertedObjectCreationExpression(this);
        }

        public BoundUnconvertedObjectCreationExpression Update(ImmutableArray<BoundExpression> arguments, ImmutableArray<IdentifierNameSyntax> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, InitializerExpressionSyntax? initializerOpt)
        {
            if (arguments != Arguments || argumentNamesOpt != ArgumentNamesOpt || argumentRefKindsOpt != ArgumentRefKindsOpt || initializerOpt != InitializerOpt)
            {
                BoundUnconvertedObjectCreationExpression boundUnconvertedObjectCreationExpression = new BoundUnconvertedObjectCreationExpression(Syntax, arguments, argumentNamesOpt, argumentRefKindsOpt, initializerOpt, base.HasErrors);
                boundUnconvertedObjectCreationExpression.CopyAttributes(this);
                return boundUnconvertedObjectCreationExpression;
            }
            return this;
        }
    }
}

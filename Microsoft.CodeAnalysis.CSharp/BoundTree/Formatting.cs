// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract partial class BoundExpression
    {
        /// <summary>
        /// Returns a serializable object that is used for displaying this expression in a diagnostic message.
        /// </summary>
        public virtual object Display
        {
            get
            {
                return this.Type;
            }
        }
    }

    public sealed partial class BoundArgListOperator
    {
        public override object Display
        {
            get { return "__arglist"; }
        }
    }

    public sealed partial class BoundLiteral
    {
        public override object Display
        {
            get { return ConstantValue?.IsNull == true ? MessageID.IDS_NULL.Localize() : base.Display; }
        }
    }

    public sealed partial class BoundLambda
    {
        public override object Display
        {
            get { return this.MessageID.Localize(); }
        }
    }

    public sealed partial class UnboundLambda
    {
        public override object Display
        {
            get { return this.MessageID.Localize(); }
        }
    }

    public sealed partial class BoundMethodGroup
    {
        public override object Display
        {
            get { return MessageID.IDS_MethodGroup.Localize(); }
        }
    }

    public sealed partial class BoundThrowExpression
    {
        public override object Display
        {
            get { return MessageID.IDS_ThrowExpression.Localize(); }
        }
    }

    public partial class BoundTupleExpression
    {
        public override object Display
        {
            get
            {
                var pooledBuilder = PooledStringBuilder.GetInstance();
                var builder = pooledBuilder.Builder;
                var arguments = this.Arguments;
                var argumentDisplays = new object[arguments.Length];

                builder.Append('(');
                builder.Append("{0}");
                argumentDisplays[0] = arguments[0].Display;

                for (int i = 1; i < arguments.Length; i++)
                {
                    builder.Append(", {" + i + "}");
                    argumentDisplays[i] = arguments[i].Display;
                }

                builder.Append(')');

                var format = pooledBuilder.ToStringAndFree();
                return string.Format(format, argumentDisplays);
            }
        }
    }

    public sealed partial class BoundPropertyGroup
    {
        public override object Display
        {
            get { throw ExceptionUtilities.Unreachable; }
        }
    }

    public partial class OutVariablePendingInference
    {
        public override object Display
        {
            get { return string.Empty; }
        }
    }

    public partial class OutDeconstructVarPendingInference
    {
        public override object Display
        {
            get { return string.Empty; }
        }
    }

#nullable enable

    public partial class BoundDiscardExpression
    {
        public override object Display
        {
            get { return (object?)this.Type ?? "_"; }
        }
    }

    public partial class DeconstructionVariablePendingInference
    {
        public override object Display
        {
            get { throw ExceptionUtilities.Unreachable; }
        }
    }

    public partial class BoundDefaultLiteral
    {
        public override object Display
        {
            get { return (object?)this.Type ?? "default"; }
        }
    }

    public partial class BoundStackAllocArrayCreation
    {
        public override object Display
            => (Type is null) ? string.Format("stackalloc {0}[{1}]", ElementType, Count.WasCompilerGenerated ? null : Count.Syntax.ToString()) : base.Display;
    }

    public partial class BoundUnconvertedSwitchExpression
    {
        public override object Display
            => (Type is null) ? MessageID.IDS_FeatureSwitchExpression.Localize() : base.Display;
    }

    public partial class BoundUnconvertedConditionalOperator
    {
        public override object Display
            => (Type is null) ? MessageID.IDS_FeatureTargetTypedConditional.Localize() : base.Display;
    }

    public partial class BoundPassByCopy
    {
        public override object Display => Expression.Display;
    }

    public partial class BoundUnconvertedAddressOfOperator
    {
        public override object Display => string.Format("&{0}", Operand.Display);
    }

    public partial class BoundUnconvertedObjectCreationExpression
    {
        public override object Display
        {
            get
            {
                var arguments = this.Arguments;
                if (arguments.Length == 0)
                {
                    return "new()";
                }

                var pooledBuilder = PooledStringBuilder.GetInstance();
                var builder = pooledBuilder.Builder;
                var argumentDisplays = new object[arguments.Length];

                builder.Append("new");
                builder.Append('(');
                builder.Append("{0}");
                argumentDisplays[0] = arguments[0].Display;

                for (int i = 1; i < arguments.Length; i++)
                {
                    builder.Append($", {{{i}}}");
                    argumentDisplays[i] = arguments[i].Display;
                }

                builder.Append(')');

                var format = pooledBuilder.ToStringAndFree();
                return string.Format(format, argumentDisplays);
            }
        }
    }
}

using System;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Operations;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public abstract class BoundNode : IBoundNodeWithIOperationChildren
    {
        [Flags()]
        private enum BoundNodeAttributes : short
        {
            HasErrors = 1,
            CompilerGenerated = 2,
            IsSuppressed = 4,
            TopLevelFlowStateMaybeNull = 8,
            TopLevelNotAnnotated = 0x10,
            TopLevelAnnotated = 0x20,
            TopLevelNone = 0x30,
            TopLevelAnnotationMask = 0x30,
            WasCompilerGeneratedIsChecked = 0x40,
            WasTopLevelNullabilityChecked = 0x80,
            WasConverted = 0x100,
            AttributesPreservedInClone = 0x107
        }

        private readonly BoundKind _kind;

        private BoundNodeAttributes _attributes;

        public readonly SyntaxNode Syntax;

        public bool HasAnyErrors
        {
            get
            {
                if (HasErrors || (Syntax != null && Syntax.HasErrors))
                {
                    return true;
                }
                BoundExpression obj = this as BoundExpression;
                if (obj == null)
                {
                    return false;
                }
                return obj.Type?.IsErrorType() == true;
            }
        }

        public bool HasErrors
        {
            get
            {
                return (_attributes & BoundNodeAttributes.HasErrors) != 0;
            }
            private set
            {
                if (value)
                {
                    _attributes |= BoundNodeAttributes.HasErrors;
                }
            }
        }

        public SyntaxTree? SyntaxTree => Syntax?.SyntaxTree;

        public bool WasCompilerGenerated
        {
            get
            {
                return (_attributes & BoundNodeAttributes.CompilerGenerated) != 0;
            }
            internal set
            {
                if (value)
                {
                    _attributes |= BoundNodeAttributes.CompilerGenerated;
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected NullabilityInfo TopLevelNullability
        {
            get
            {
                return TopLevelNullabilityCore;
            }
            set
            {
                _attributes &= ~(BoundNodeAttributes.TopLevelNone | BoundNodeAttributes.TopLevelFlowStateMaybeNull);
                BoundNodeAttributes attributes = _attributes;
                Microsoft.CodeAnalysis.NullableAnnotation annotation = value.Annotation;
                _attributes = attributes | (annotation switch
                {
                    Microsoft.CodeAnalysis.NullableAnnotation.Annotated => BoundNodeAttributes.TopLevelAnnotated,
                    Microsoft.CodeAnalysis.NullableAnnotation.NotAnnotated => BoundNodeAttributes.TopLevelNotAnnotated,
                    Microsoft.CodeAnalysis.NullableAnnotation.None => BoundNodeAttributes.TopLevelNone,
                    _ => throw ExceptionUtilities.UnexpectedValue(annotation),
                });
                switch (value.FlowState)
                {
                    case Microsoft.CodeAnalysis.NullableFlowState.MaybeNull:
                        _attributes |= BoundNodeAttributes.TopLevelFlowStateMaybeNull;
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(value.FlowState);
                    case Microsoft.CodeAnalysis.NullableFlowState.NotNull:
                        break;
                }
            }
        }

        private NullabilityInfo TopLevelNullabilityCore
        {
            get
            {
                if ((_attributes & BoundNodeAttributes.TopLevelNone) == 0)
                {
                    return default(NullabilityInfo);
                }
                BoundNodeAttributes boundNodeAttributes = _attributes & BoundNodeAttributes.TopLevelNone;
                int annotation = boundNodeAttributes switch
                {
                    BoundNodeAttributes.TopLevelAnnotated => 2,
                    BoundNodeAttributes.TopLevelNotAnnotated => 1,
                    BoundNodeAttributes.TopLevelNone => 0,
                    _ => throw ExceptionUtilities.UnexpectedValue(boundNodeAttributes),
                };
                Microsoft.CodeAnalysis.NullableFlowState flowState = (((_attributes & BoundNodeAttributes.TopLevelFlowStateMaybeNull) == 0) ? Microsoft.CodeAnalysis.NullableFlowState.NotNull : Microsoft.CodeAnalysis.NullableFlowState.MaybeNull);
                return new NullabilityInfo((Microsoft.CodeAnalysis.NullableAnnotation)annotation, flowState);
            }
        }

        public bool IsSuppressed
        {
            get
            {
                return (_attributes & BoundNodeAttributes.IsSuppressed) != 0;
            }
            protected set
            {
                if (value)
                {
                    _attributes |= BoundNodeAttributes.IsSuppressed;
                }
            }
        }

        public BoundKind Kind => _kind;

        ImmutableArray<BoundNode?> IBoundNodeWithIOperationChildren.Children => Children;

        protected virtual ImmutableArray<BoundNode?> Children => ImmutableArray<BoundNode>.Empty;

        protected new BoundNode MemberwiseClone()
        {
            BoundNode obj = (BoundNode)base.MemberwiseClone();
            obj._attributes &= BoundNodeAttributes.AttributesPreservedInClone;
            return obj;
        }

        protected BoundNode(BoundKind kind, SyntaxNode syntax)
        {
            _kind = kind;
            Syntax = syntax;
        }

        protected BoundNode(BoundKind kind, SyntaxNode syntax, bool hasErrors)
            : this(kind, syntax)
        {
            if (hasErrors)
            {
                _attributes = BoundNodeAttributes.HasErrors;
            }
        }

        protected void CopyAttributes(BoundNode original)
        {
            WasCompilerGenerated = original.WasCompilerGenerated;
            IsSuppressed = original.IsSuppressed;
        }

        public void ResetCompilerGenerated(bool newCompilerGenerated)
        {
            if (newCompilerGenerated)
            {
                _attributes |= BoundNodeAttributes.CompilerGenerated;
            }
            else
            {
                _attributes &= ~BoundNodeAttributes.CompilerGenerated;
            }
        }

        public virtual BoundNode? Accept(BoundTreeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        internal BoundNode WithHasErrors()
        {
            if (HasErrors)
            {
                return this;
            }
            BoundNode boundNode = MemberwiseClone();
            boundNode.HasErrors = true;
            return boundNode;
        }

        internal string GetDebuggerDisplay()
        {
            string text = GetType().Name;
            if (Syntax != null)
            {
                text = text + " " + Syntax.ToString();
            }
            return text;
        }

        [Conditional("DEBUG")]
        public void CheckLocalsDefined()
        {
        }
    }
}

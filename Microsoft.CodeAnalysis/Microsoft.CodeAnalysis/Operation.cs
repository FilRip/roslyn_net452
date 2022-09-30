using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class Operation : IOperation
    {
        [NonDefaultable]
        internal readonly struct Enumerable : IEnumerable<IOperation>, IEnumerable
        {
            private readonly Operation _operation;

            internal Enumerable(Operation operation)
            {
                _operation = operation;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(_operation);
            }

            public ImmutableArray<IOperation> ToImmutableArray()
            {
                Operation operation = _operation;
                if (operation is NoneOperation noneOperation)
                {
                    return noneOperation.Children;
                }
                if (operation is InvalidOperation invalidOperation)
                {
                    return invalidOperation.Children;
                }
                if (!GetEnumerator().MoveNext())
                {
                    return ImmutableArray<IOperation>.Empty;
                }
                ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance();
                using (Enumerator enumerator = GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        IOperation current = enumerator.Current;
                        instance.Add(current);
                    }
                }
                return instance.ToImmutableAndFree();
            }

            IEnumerator<IOperation> IEnumerable<IOperation>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [NonDefaultable]
        internal struct Enumerator : IEnumerator<IOperation>, IEnumerator, IDisposable
        {
            private readonly Operation _operation;

            private int _currentSlot;

            private int _currentIndex;

            public IOperation Current => _operation.GetCurrent(_currentSlot, _currentIndex);

            object? IEnumerator.Current => Current;

            public Enumerator(Operation operation)
            {
                _operation = operation;
                _currentSlot = -1;
                _currentIndex = -1;
            }

            public bool MoveNext()
            {
                bool result;
                (result, _currentSlot, _currentIndex) = _operation.MoveNext(_currentSlot, _currentIndex);
                return result;
            }

            void IEnumerator.Reset()
            {
                _currentSlot = -1;
                _currentIndex = -1;
            }

            void IDisposable.Dispose()
            {
            }
        }

        protected static readonly IOperation s_unset = new EmptyOperation(null, null, isImplicit: true);

        private readonly SemanticModel? _owningSemanticModelOpt;

        private IOperation? _parentDoNotAccessDirectly;

        private static readonly ObjectPool<Queue<IOperation>> s_queuePool = new ObjectPool<Queue<IOperation>>(() => new Queue<IOperation>(), 10);

        public IOperation? Parent => _parentDoNotAccessDirectly;

        public bool IsImplicit { get; }

        public abstract OperationKind Kind { get; }

        public SyntaxNode Syntax { get; }

        public abstract ITypeSymbol? Type { get; }

        public string Language => Syntax.Language;

        internal abstract ConstantValue? OperationConstantValue { get; }

        public Optional<object?> ConstantValue
        {
            get
            {
                if (OperationConstantValue == null || OperationConstantValue!.IsBad)
                {
                    return default(Optional<object>);
                }
                return new Optional<object>(OperationConstantValue!.Value);
            }
        }

        IEnumerable<IOperation> IOperation.Children => ChildOperations;

        internal Enumerable ChildOperations => new Enumerable(this);

        SemanticModel? IOperation.SemanticModel => _owningSemanticModelOpt?.ContainingModelOrSelf;

        internal SemanticModel? OwningSemanticModel => _owningSemanticModelOpt;

        protected Operation(SemanticModel? semanticModel, SyntaxNode syntax, bool isImplicit)
        {
            _owningSemanticModelOpt = semanticModel;
            Syntax = syntax;
            IsImplicit = isImplicit;
            _parentDoNotAccessDirectly = s_unset;
        }

        protected abstract IOperation GetCurrent(int slot, int index);

        protected abstract (bool hasNext, int nextSlot, int nextIndex) MoveNext(int previousSlot, int previousIndex);

        public abstract void Accept(OperationVisitor visitor);

        public abstract TResult Accept<TArgument, TResult>(OperationVisitor<TArgument, TResult> visitor, TArgument argument); // FilRip : Change return type from TResult? to TResult

        protected void SetParentOperation(IOperation? parent)
        {
            _parentDoNotAccessDirectly = parent;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("operation")]
        public static T? SetParentOperation<T>(T? operation, IOperation? parent) where T : IOperation
        {
            (operation as Operation)?.SetParentOperation(parent);
            return operation;
        }

        public static ImmutableArray<T> SetParentOperation<T>(ImmutableArray<T> operations, IOperation? parent) where T : IOperation
        {
            if (operations.Length == 0)
            {
                return operations;
            }
            ImmutableArray<T>.Enumerator enumerator = operations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SetParentOperation(enumerator.Current, parent);
            }
            return operations;
        }

        [Conditional("DEBUG")]
        internal static void VerifyParentOperation(IOperation? parent, IOperation child)
        {
        }

        [Conditional("DEBUG")]
        internal static void VerifyParentOperation<T>(IOperation? parent, ImmutableArray<T> children) where T : IOperation
        {
            ImmutableArray<T>.Enumerator enumerator = children.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _ = enumerator.Current;
            }
        }
    }
}

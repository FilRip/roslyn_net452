#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    public abstract class OperationWalker : OperationVisitor
    {
        private int _recursionDepth;

        private void VisitChildOperations(IOperation operation)
        {
            foreach (IOperation childOperation in ((Operation)operation).ChildOperations)
            {
                Visit(childOperation);
            }
        }

        public override void Visit(IOperation? operation)
        {
            if (operation != null)
            {
                _recursionDepth++;
                try
                {
                    StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
                    operation!.Accept(this);
                }
                finally
                {
                    _recursionDepth--;
                }
            }
        }

        public override void DefaultVisit(IOperation operation)
        {
            VisitChildOperations(operation);
        }

        internal override void VisitNoneOperation(IOperation operation)
        {
            VisitChildOperations(operation);
        }
    }
    public abstract class OperationWalker<TArgument> : OperationVisitor<TArgument, object>
    {
        private int _recursionDepth;

        private void VisitChildrenOperations(IOperation operation, TArgument argument)
        {
            foreach (IOperation childOperation in ((Operation)operation).ChildOperations)
            {
                Visit(childOperation, argument);
            }
        }

        public override object? Visit(IOperation? operation, TArgument argument)
        {
            if (operation != null)
            {
                _recursionDepth++;
                try
                {
                    StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
                    operation!.Accept(this, argument);
                }
                finally
                {
                    _recursionDepth--;
                }
            }
            return null;
        }

        public override object? DefaultVisit(IOperation operation, TArgument argument)
        {
            VisitChildrenOperations(operation, argument);
            return null;
        }

        internal override object? VisitNoneOperation(IOperation operation, TArgument argument)
        {
            VisitChildrenOperations(operation, argument);
            return null;
        }
    }
}

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class VariablesDeclaredWalker : AbstractRegionControlFlowPass
    {
        private HashSet<Symbol> _variablesDeclared = new HashSet<Symbol>();

        internal static IEnumerable<Symbol> Analyze(CSharpCompilation compilation, Symbol member, BoundNode node, BoundNode firstInRegion, BoundNode lastInRegion)
        {
            VariablesDeclaredWalker variablesDeclaredWalker = new VariablesDeclaredWalker(compilation, member, node, firstInRegion, lastInRegion);
            try
            {
                bool badRegion = false;
                variablesDeclaredWalker.Analyze(ref badRegion);
                IEnumerable<Symbol> result;
                if (!badRegion)
                {
                    IEnumerable<Symbol> variablesDeclared = variablesDeclaredWalker._variablesDeclared;
                    result = variablesDeclared;
                }
                else
                {
                    result = SpecializedCollections.EmptyEnumerable<Symbol>();
                }
                return result;
            }
            finally
            {
                variablesDeclaredWalker.Free();
            }
        }

        internal VariablesDeclaredWalker(CSharpCompilation compilation, Symbol member, BoundNode node, BoundNode firstInRegion, BoundNode lastInRegion)
            : base(compilation, member, node, firstInRegion, lastInRegion)
        {
        }

        protected override void Free()
        {
            base.Free();
            _variablesDeclared = null;
        }

        public override void VisitPattern(BoundPattern pattern)
        {
            base.VisitPattern(pattern);
            NoteDeclaredPatternVariables(pattern);
        }

        protected override void VisitSwitchSection(BoundSwitchSection node, bool isLastSection)
        {
            ImmutableArray<BoundSwitchLabel>.Enumerator enumerator = node.SwitchLabels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundSwitchLabel current = enumerator.Current;
                NoteDeclaredPatternVariables(current.Pattern);
            }
            base.VisitSwitchSection(node, isLastSection);
        }

        private void NoteDeclaredPatternVariables(BoundPattern pattern)
        {
            if (!base.IsInside)
            {
                return;
            }
            if (!(pattern is BoundDeclarationPattern boundDeclarationPattern))
            {
                if (pattern is BoundRecursivePattern boundRecursivePattern)
                {
                    Symbol? variable = boundRecursivePattern.Variable;
                    if ((object)variable != null && variable!.Kind == SymbolKind.Local)
                    {
                        _variablesDeclared.Add(boundRecursivePattern.Variable);
                    }
                }
            }
            else
            {
                Symbol? variable2 = boundDeclarationPattern.Variable;
                if ((object)variable2 != null && variable2!.Kind == SymbolKind.Local)
                {
                    _variablesDeclared.Add(boundDeclarationPattern.Variable);
                }
            }
        }

        public override BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
        {
            if (base.IsInside)
            {
                _variablesDeclared.Add(node.LocalSymbol);
            }
            return base.VisitLocalDeclaration(node);
        }

        public override BoundNode VisitLambda(BoundLambda node)
        {
            if (base.IsInside && !node.WasCompilerGenerated)
            {
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = node.Symbol.Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ParameterSymbol current = enumerator.Current;
                    _variablesDeclared.Add(current);
                }
            }
            return base.VisitLambda(node);
        }

        public override BoundNode VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            if (base.IsInside && !node.WasCompilerGenerated)
            {
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = node.Symbol.Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ParameterSymbol current = enumerator.Current;
                    _variablesDeclared.Add(current);
                }
            }
            return base.VisitLocalFunctionStatement(node);
        }

        public override void VisitForEachIterationVariables(BoundForEachStatement node)
        {
            if (!base.IsInside)
            {
                return;
            }
            BoundDeconstructionAssignmentOperator boundDeconstructionAssignmentOperator = node.DeconstructionOpt?.DeconstructionAssignment;
            if (boundDeconstructionAssignmentOperator == null)
            {
                _variablesDeclared.AddAll(node.IterationVariables);
                return;
            }
            boundDeconstructionAssignmentOperator.Left.VisitAllElements(delegate (BoundExpression x, VariablesDeclaredWalker self)
            {
                self.Visit(x);
            }, this);
        }

        protected override void VisitCatchBlock(BoundCatchBlock catchBlock, ref LocalState finallyState)
        {
            if (base.IsInside)
            {
                LocalSymbol localSymbol = catchBlock.Locals.FirstOrDefault();
                if ((object)localSymbol != null && localSymbol.DeclarationKind == LocalDeclarationKind.CatchVariable)
                {
                    _variablesDeclared.Add(localSymbol);
                }
            }
            base.VisitCatchBlock(catchBlock, ref finallyState);
        }

        public override BoundNode VisitQueryClause(BoundQueryClause node)
        {
            if (base.IsInside && (object)node.DefinedSymbol != null)
            {
                _variablesDeclared.Add(node.DefinedSymbol);
            }
            return base.VisitQueryClause(node);
        }

        protected override void VisitLvalue(BoundLocal node)
        {
            VisitLocal(node);
        }

        public override BoundNode VisitLocal(BoundLocal node)
        {
            if (base.IsInside && node.DeclarationKind != 0)
            {
                _variablesDeclared.Add(node.LocalSymbol);
            }
            return null;
        }
    }
}

using System.Collections.Generic;
using System.Linq;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

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
    internal sealed class BoundTreeDumperNodeProducer : BoundTreeVisitor<object, TreeDumperNode>
    {
        private BoundTreeDumperNodeProducer()
        {
        }

        public static TreeDumperNode MakeTree(BoundNode node)
        {
            return new BoundTreeDumperNodeProducer().Visit(node, null);
        }

        public override TreeDumperNode VisitFieldEqualsValue(BoundFieldEqualsValue node, object? arg)
        {
            return new TreeDumperNode("fieldEqualsValue", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("field", node.Field, null),
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitPropertyEqualsValue(BoundPropertyEqualsValue node, object? arg)
        {
            return new TreeDumperNode("propertyEqualsValue", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("property", node.Property, null),
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitParameterEqualsValue(BoundParameterEqualsValue node, object? arg)
        {
            return new TreeDumperNode("parameterEqualsValue", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("parameter", node.Parameter, null),
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node, object? arg)
        {
            return new TreeDumperNode("globalStatementInitializer", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("statement", null, new TreeDumperNode[1] { Visit(node.Statement, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node, object? arg)
        {
            return new TreeDumperNode("deconstructValuePlaceholder", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("valEscape", node.ValEscape, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node, object? arg)
        {
            return new TreeDumperNode("tupleOperandPlaceholder", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node, object? arg)
        {
            return new TreeDumperNode("awaitableValuePlaceholder", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("valEscape", node.ValEscape, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDisposableValuePlaceholder(BoundDisposableValuePlaceholder node, object? arg)
        {
            return new TreeDumperNode("disposableValuePlaceholder", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node, object? arg)
        {
            return new TreeDumperNode("objectOrCollectionValuePlaceholder", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("isNewInstance", node.IsNewInstance, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDup(BoundDup node, object? arg)
        {
            return new TreeDumperNode("dup", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("refKind", node.RefKind, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitPassByCopy(BoundPassByCopy node, object? arg)
        {
            return new TreeDumperNode("passByCopy", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitBadExpression(BoundBadExpression node, object? arg)
        {
            return new TreeDumperNode("badExpression", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("symbols", node.Symbols, null),
                new TreeDumperNode("childBoundNodes", null, node.ChildBoundNodes.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitBadStatement(BoundBadStatement node, object? arg)
        {
            return new TreeDumperNode("badStatement", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("childBoundNodes", null, node.ChildBoundNodes.Select((BoundNode x) => Visit(x, null))),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitExtractedFinallyBlock(BoundExtractedFinallyBlock node, object? arg)
        {
            return new TreeDumperNode("extractedFinallyBlock", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("finallyBlock", null, new TreeDumperNode[1] { Visit(node.FinallyBlock, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitTypeExpression(BoundTypeExpression node, object? arg)
        {
            TreeDumperNode[] obj = new TreeDumperNode[7]
            {
                new TreeDumperNode("aliasOpt", node.AliasOpt, null),
                new TreeDumperNode("boundContainingTypeOpt", null, new TreeDumperNode[1] { Visit(node.BoundContainingTypeOpt, null) }),
                null,
                null,
                null,
                null,
                null
            };
            IEnumerable<TreeDumperNode> children;
            if (!node.BoundDimensionsOpt.IsDefault)
            {
                children = node.BoundDimensionsOpt.Select((BoundExpression x) => Visit(x, null));
            }
            else
            {
                IEnumerable<TreeDumperNode> enumerable = new TreeDumperNode[0];
                children = enumerable;
            }
            obj[2] = new TreeDumperNode("boundDimensionsOpt", null, children);
            obj[3] = new TreeDumperNode("typeWithAnnotations", node.TypeWithAnnotations, null);
            obj[4] = new TreeDumperNode("type", node.Type, null);
            obj[5] = new TreeDumperNode("isSuppressed", node.IsSuppressed, null);
            obj[6] = new TreeDumperNode("hasErrors", node.HasErrors, null);
            return new TreeDumperNode("typeExpression", null, obj);
        }

        public override TreeDumperNode VisitTypeOrValueExpression(BoundTypeOrValueExpression node, object? arg)
        {
            return new TreeDumperNode("typeOrValueExpression", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("data", node.Data, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitNamespaceExpression(BoundNamespaceExpression node, object? arg)
        {
            return new TreeDumperNode("namespaceExpression", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("namespaceSymbol", node.NamespaceSymbol, null),
                new TreeDumperNode("aliasOpt", node.AliasOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitUnaryOperator(BoundUnaryOperator node, object? arg)
        {
            return new TreeDumperNode("unaryOperator", null, new TreeDumperNode[9]
            {
                new TreeDumperNode("operatorKind", node.OperatorKind, null),
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("methodOpt", node.MethodOpt, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("originalUserDefinedOperatorsOpt", node.OriginalUserDefinedOperatorsOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitIncrementOperator(BoundIncrementOperator node, object? arg)
        {
            return new TreeDumperNode("incrementOperator", null, new TreeDumperNode[10]
            {
                new TreeDumperNode("operatorKind", node.OperatorKind, null),
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("methodOpt", node.MethodOpt, null),
                new TreeDumperNode("operandConversion", node.OperandConversion, null),
                new TreeDumperNode("resultConversion", node.ResultConversion, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("originalUserDefinedOperatorsOpt", node.OriginalUserDefinedOperatorsOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitAddressOfOperator(BoundAddressOfOperator node, object? arg)
        {
            return new TreeDumperNode("addressOfOperator", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("isManaged", node.IsManaged, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node, object? arg)
        {
            return new TreeDumperNode("unconvertedAddressOfOperator", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitFunctionPointerLoad(BoundFunctionPointerLoad node, object? arg)
        {
            return new TreeDumperNode("functionPointerLoad", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("targetMethod", node.TargetMethod, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node, object? arg)
        {
            return new TreeDumperNode("pointerIndirectionOperator", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitPointerElementAccess(BoundPointerElementAccess node, object? arg)
        {
            return new TreeDumperNode("pointerElementAccess", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("index", null, new TreeDumperNode[1] { Visit(node.Index, null) }),
                new TreeDumperNode("@checked", node.Checked, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node, object? arg)
        {
            return new TreeDumperNode("functionPointerInvocation", null, new TreeDumperNode[7]
            {
                new TreeDumperNode("invokedExpression", null, new TreeDumperNode[1] { Visit(node.InvokedExpression, null) }),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitRefTypeOperator(BoundRefTypeOperator node, object? arg)
        {
            return new TreeDumperNode("refTypeOperator", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("getTypeFromHandle", node.GetTypeFromHandle, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitMakeRefOperator(BoundMakeRefOperator node, object? arg)
        {
            return new TreeDumperNode("makeRefOperator", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitRefValueOperator(BoundRefValueOperator node, object? arg)
        {
            return new TreeDumperNode("refValueOperator", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("nullableAnnotation", node.NullableAnnotation, null),
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitFromEndIndexExpression(BoundFromEndIndexExpression node, object? arg)
        {
            return new TreeDumperNode("fromEndIndexExpression", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("methodOpt", node.MethodOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitRangeExpression(BoundRangeExpression node, object? arg)
        {
            return new TreeDumperNode("rangeExpression", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("leftOperandOpt", null, new TreeDumperNode[1] { Visit(node.LeftOperandOpt, null) }),
                new TreeDumperNode("rightOperandOpt", null, new TreeDumperNode[1] { Visit(node.RightOperandOpt, null) }),
                new TreeDumperNode("methodOpt", node.MethodOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitBinaryOperator(BoundBinaryOperator node, object? arg)
        {
            return new TreeDumperNode("binaryOperator", null, new TreeDumperNode[10]
            {
                new TreeDumperNode("operatorKind", node.OperatorKind, null),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("methodOpt", node.MethodOpt, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("originalUserDefinedOperatorsOpt", node.OriginalUserDefinedOperatorsOpt, null),
                new TreeDumperNode("left", null, new TreeDumperNode[1] { Visit(node.Left, null) }),
                new TreeDumperNode("right", null, new TreeDumperNode[1] { Visit(node.Right, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitTupleBinaryOperator(BoundTupleBinaryOperator node, object? arg)
        {
            return new TreeDumperNode("tupleBinaryOperator", null, new TreeDumperNode[7]
            {
                new TreeDumperNode("left", null, new TreeDumperNode[1] { Visit(node.Left, null) }),
                new TreeDumperNode("right", null, new TreeDumperNode[1] { Visit(node.Right, null) }),
                new TreeDumperNode("operatorKind", node.OperatorKind, null),
                new TreeDumperNode("operators", node.Operators, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node, object? arg)
        {
            return new TreeDumperNode("userDefinedConditionalLogicalOperator", null, new TreeDumperNode[11]
            {
                new TreeDumperNode("operatorKind", node.OperatorKind, null),
                new TreeDumperNode("logicalOperator", node.LogicalOperator, null),
                new TreeDumperNode("trueOperator", node.TrueOperator, null),
                new TreeDumperNode("falseOperator", node.FalseOperator, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("originalUserDefinedOperatorsOpt", node.OriginalUserDefinedOperatorsOpt, null),
                new TreeDumperNode("left", null, new TreeDumperNode[1] { Visit(node.Left, null) }),
                new TreeDumperNode("right", null, new TreeDumperNode[1] { Visit(node.Right, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node, object? arg)
        {
            return new TreeDumperNode("compoundAssignmentOperator", null, new TreeDumperNode[10]
            {
                new TreeDumperNode("@operator", node.Operator, null),
                new TreeDumperNode("left", null, new TreeDumperNode[1] { Visit(node.Left, null) }),
                new TreeDumperNode("right", null, new TreeDumperNode[1] { Visit(node.Right, null) }),
                new TreeDumperNode("leftConversion", node.LeftConversion, null),
                new TreeDumperNode("finalConversion", node.FinalConversion, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("originalUserDefinedOperatorsOpt", node.OriginalUserDefinedOperatorsOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitAssignmentOperator(BoundAssignmentOperator node, object? arg)
        {
            return new TreeDumperNode("assignmentOperator", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("left", null, new TreeDumperNode[1] { Visit(node.Left, null) }),
                new TreeDumperNode("right", null, new TreeDumperNode[1] { Visit(node.Right, null) }),
                new TreeDumperNode("isRef", node.IsRef, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node, object? arg)
        {
            return new TreeDumperNode("deconstructionAssignmentOperator", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("left", null, new TreeDumperNode[1] { Visit(node.Left, null) }),
                new TreeDumperNode("right", null, new TreeDumperNode[1] { Visit(node.Right, null) }),
                new TreeDumperNode("isUsed", node.IsUsed, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitNullCoalescingOperator(BoundNullCoalescingOperator node, object? arg)
        {
            return new TreeDumperNode("nullCoalescingOperator", null, new TreeDumperNode[7]
            {
                new TreeDumperNode("leftOperand", null, new TreeDumperNode[1] { Visit(node.LeftOperand, null) }),
                new TreeDumperNode("rightOperand", null, new TreeDumperNode[1] { Visit(node.RightOperand, null) }),
                new TreeDumperNode("leftConversion", node.LeftConversion, null),
                new TreeDumperNode("operatorResultKind", node.OperatorResultKind, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node, object? arg)
        {
            return new TreeDumperNode("nullCoalescingAssignmentOperator", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("leftOperand", null, new TreeDumperNode[1] { Visit(node.LeftOperand, null) }),
                new TreeDumperNode("rightOperand", null, new TreeDumperNode[1] { Visit(node.RightOperand, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node, object? arg)
        {
            return new TreeDumperNode("unconvertedConditionalOperator", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
                new TreeDumperNode("consequence", null, new TreeDumperNode[1] { Visit(node.Consequence, null) }),
                new TreeDumperNode("alternative", null, new TreeDumperNode[1] { Visit(node.Alternative, null) }),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("noCommonTypeError", node.NoCommonTypeError, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitConditionalOperator(BoundConditionalOperator node, object? arg)
        {
            return new TreeDumperNode("conditionalOperator", null, new TreeDumperNode[10]
            {
                new TreeDumperNode("isRef", node.IsRef, null),
                new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
                new TreeDumperNode("consequence", null, new TreeDumperNode[1] { Visit(node.Consequence, null) }),
                new TreeDumperNode("alternative", null, new TreeDumperNode[1] { Visit(node.Alternative, null) }),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("naturalTypeOpt", node.NaturalTypeOpt, null),
                new TreeDumperNode("wasTargetTyped", node.WasTargetTyped, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitArrayAccess(BoundArrayAccess node, object? arg)
        {
            return new TreeDumperNode("arrayAccess", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("indices", null, node.Indices.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitArrayLength(BoundArrayLength node, object? arg)
        {
            return new TreeDumperNode("arrayLength", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitAwaitableInfo(BoundAwaitableInfo node, object? arg)
        {
            return new TreeDumperNode("awaitableInfo", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("awaitableInstancePlaceholder", null, new TreeDumperNode[1] { Visit(node.AwaitableInstancePlaceholder, null) }),
                new TreeDumperNode("isDynamic", node.IsDynamic, null),
                new TreeDumperNode("getAwaiter", null, new TreeDumperNode[1] { Visit(node.GetAwaiter, null) }),
                new TreeDumperNode("isCompleted", node.IsCompleted, null),
                new TreeDumperNode("getResult", node.GetResult, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitAwaitExpression(BoundAwaitExpression node, object? arg)
        {
            return new TreeDumperNode("awaitExpression", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("awaitableInfo", null, new TreeDumperNode[1] { Visit(node.AwaitableInfo, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitTypeOfOperator(BoundTypeOfOperator node, object? arg)
        {
            return new TreeDumperNode("typeOfOperator", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("sourceType", null, new TreeDumperNode[1] { Visit(node.SourceType, null) }),
                new TreeDumperNode("getTypeFromHandle", node.GetTypeFromHandle, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitMethodDefIndex(BoundMethodDefIndex node, object? arg)
        {
            return new TreeDumperNode("methodDefIndex", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("method", node.Method, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node, object? arg)
        {
            return new TreeDumperNode("maximumMethodDefIndex", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node, object? arg)
        {
            return new TreeDumperNode("instrumentationPayloadRoot", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("analysisKind", node.AnalysisKind, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitModuleVersionId(BoundModuleVersionId node, object? arg)
        {
            return new TreeDumperNode("moduleVersionId", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitModuleVersionIdString(BoundModuleVersionIdString node, object? arg)
        {
            return new TreeDumperNode("moduleVersionIdString", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSourceDocumentIndex(BoundSourceDocumentIndex node, object? arg)
        {
            return new TreeDumperNode("sourceDocumentIndex", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("document", node.Document, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitMethodInfo(BoundMethodInfo node, object? arg)
        {
            return new TreeDumperNode("methodInfo", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("method", node.Method, null),
                new TreeDumperNode("getMethodFromHandle", node.GetMethodFromHandle, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitFieldInfo(BoundFieldInfo node, object? arg)
        {
            return new TreeDumperNode("fieldInfo", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("field", node.Field, null),
                new TreeDumperNode("getFieldFromHandle", node.GetFieldFromHandle, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDefaultLiteral(BoundDefaultLiteral node, object? arg)
        {
            return new TreeDumperNode("defaultLiteral", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDefaultExpression(BoundDefaultExpression node, object? arg)
        {
            return new TreeDumperNode("defaultExpression", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("targetType", null, new TreeDumperNode[1] { Visit(node.TargetType, null) }),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitIsOperator(BoundIsOperator node, object? arg)
        {
            return new TreeDumperNode("isOperator", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("targetType", null, new TreeDumperNode[1] { Visit(node.TargetType, null) }),
                new TreeDumperNode("conversion", node.Conversion, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitAsOperator(BoundAsOperator node, object? arg)
        {
            return new TreeDumperNode("asOperator", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("targetType", null, new TreeDumperNode[1] { Visit(node.TargetType, null) }),
                new TreeDumperNode("conversion", node.Conversion, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSizeOfOperator(BoundSizeOfOperator node, object? arg)
        {
            return new TreeDumperNode("sizeOfOperator", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("sourceType", null, new TreeDumperNode[1] { Visit(node.SourceType, null) }),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitConversion(BoundConversion node, object? arg)
        {
            return new TreeDumperNode("conversion", null, new TreeDumperNode[11]
            {
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("conversion", node.Conversion, null),
                new TreeDumperNode("isBaseConversion", node.IsBaseConversion, null),
                new TreeDumperNode("@checked", node.Checked, null),
                new TreeDumperNode("explicitCastInCode", node.ExplicitCastInCode, null),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("conversionGroupOpt", node.ConversionGroupOpt, null),
                new TreeDumperNode("originalUserDefinedConversionsOpt", node.OriginalUserDefinedConversionsOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node, object? arg)
        {
            return new TreeDumperNode("readOnlySpanFromArray", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
                new TreeDumperNode("conversionMethod", node.ConversionMethod, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitArgList(BoundArgList node, object? arg)
        {
            return new TreeDumperNode("argList", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitArgListOperator(BoundArgListOperator node, object? arg)
        {
            return new TreeDumperNode("argListOperator", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node, object? arg)
        {
            return new TreeDumperNode("fixedLocalCollectionInitializer", null, new TreeDumperNode[7]
            {
                new TreeDumperNode("elementPointerType", node.ElementPointerType, null),
                new TreeDumperNode("elementPointerTypeConversion", node.ElementPointerTypeConversion, null),
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("getPinnableOpt", node.GetPinnableOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSequencePoint(BoundSequencePoint node, object? arg)
        {
            return new TreeDumperNode("sequencePoint", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("statementOpt", null, new TreeDumperNode[1] { Visit(node.StatementOpt, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSequencePointWithSpan(BoundSequencePointWithSpan node, object? arg)
        {
            return new TreeDumperNode("sequencePointWithSpan", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("statementOpt", null, new TreeDumperNode[1] { Visit(node.StatementOpt, null) }),
                new TreeDumperNode("span", node.Span, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSavePreviousSequencePoint(BoundSavePreviousSequencePoint node, object? arg)
        {
            return new TreeDumperNode("savePreviousSequencePoint", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("identifier", node.Identifier, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitRestorePreviousSequencePoint(BoundRestorePreviousSequencePoint node, object? arg)
        {
            return new TreeDumperNode("restorePreviousSequencePoint", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("identifier", node.Identifier, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitStepThroughSequencePoint(BoundStepThroughSequencePoint node, object? arg)
        {
            return new TreeDumperNode("stepThroughSequencePoint", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("span", node.Span, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitBlock(BoundBlock node, object? arg)
        {
            return new TreeDumperNode("block", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("localFunctions", node.LocalFunctions, null),
                new TreeDumperNode("statements", null, node.Statements.Select((BoundStatement x) => Visit(x, null))),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitScope(BoundScope node, object? arg)
        {
            return new TreeDumperNode("scope", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("statements", null, node.Statements.Select((BoundStatement x) => Visit(x, null))),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitStateMachineScope(BoundStateMachineScope node, object? arg)
        {
            return new TreeDumperNode("stateMachineScope", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("fields", node.Fields, null),
                new TreeDumperNode("statement", null, new TreeDumperNode[1] { Visit(node.Statement, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLocalDeclaration(BoundLocalDeclaration node, object? arg)
        {
            TreeDumperNode[] obj = new TreeDumperNode[6]
            {
                new TreeDumperNode("localSymbol", node.LocalSymbol, null),
                new TreeDumperNode("declaredTypeOpt", null, new TreeDumperNode[1] { Visit(node.DeclaredTypeOpt, null) }),
                new TreeDumperNode("initializerOpt", null, new TreeDumperNode[1] { Visit(node.InitializerOpt, null) }),
                null,
                null,
                null
            };
            IEnumerable<TreeDumperNode> children;
            if (!node.ArgumentsOpt.IsDefault)
            {
                children = node.ArgumentsOpt.Select((BoundExpression x) => Visit(x, null));
            }
            else
            {
                IEnumerable<TreeDumperNode> enumerable = new TreeDumperNode[0];
                children = enumerable;
            }
            obj[3] = new TreeDumperNode("argumentsOpt", null, children);
            obj[4] = new TreeDumperNode("inferredType", node.InferredType, null);
            obj[5] = new TreeDumperNode("hasErrors", node.HasErrors, null);
            return new TreeDumperNode("localDeclaration", null, obj);
        }

        public override TreeDumperNode VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node, object? arg)
        {
            return new TreeDumperNode("multipleLocalDeclarations", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("localDeclarations", null, node.LocalDeclarations.Select((BoundLocalDeclaration x) => Visit(x, null))),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node, object? arg)
        {
            return new TreeDumperNode("usingLocalDeclarations", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("patternDisposeInfoOpt", node.PatternDisposeInfoOpt, null),
                new TreeDumperNode("iDisposableConversion", node.IDisposableConversion, null),
                new TreeDumperNode("awaitOpt", null, new TreeDumperNode[1] { Visit(node.AwaitOpt, null) }),
                new TreeDumperNode("localDeclarations", null, node.LocalDeclarations.Select((BoundLocalDeclaration x) => Visit(x, null))),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLocalFunctionStatement(BoundLocalFunctionStatement node, object? arg)
        {
            return new TreeDumperNode("localFunctionStatement", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("symbol", node.Symbol, null),
                new TreeDumperNode("blockBody", null, new TreeDumperNode[1] { Visit(node.BlockBody, null) }),
                new TreeDumperNode("expressionBody", null, new TreeDumperNode[1] { Visit(node.ExpressionBody, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitNoOpStatement(BoundNoOpStatement node, object? arg)
        {
            return new TreeDumperNode("noOpStatement", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("flavor", node.Flavor, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitReturnStatement(BoundReturnStatement node, object? arg)
        {
            return new TreeDumperNode("returnStatement", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("refKind", node.RefKind, null),
                new TreeDumperNode("expressionOpt", null, new TreeDumperNode[1] { Visit(node.ExpressionOpt, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitYieldReturnStatement(BoundYieldReturnStatement node, object? arg)
        {
            return new TreeDumperNode("yieldReturnStatement", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitYieldBreakStatement(BoundYieldBreakStatement node, object? arg)
        {
            return new TreeDumperNode("yieldBreakStatement", null, new TreeDumperNode[0]);
        }

        public override TreeDumperNode VisitThrowStatement(BoundThrowStatement node, object? arg)
        {
            return new TreeDumperNode("throwStatement", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("expressionOpt", null, new TreeDumperNode[1] { Visit(node.ExpressionOpt, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitExpressionStatement(BoundExpressionStatement node, object? arg)
        {
            return new TreeDumperNode("expressionStatement", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitBreakStatement(BoundBreakStatement node, object? arg)
        {
            return new TreeDumperNode("breakStatement", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("label", node.Label, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitContinueStatement(BoundContinueStatement node, object? arg)
        {
            return new TreeDumperNode("continueStatement", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("label", node.Label, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSwitchStatement(BoundSwitchStatement node, object? arg)
        {
            return new TreeDumperNode("switchStatement", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("innerLocals", node.InnerLocals, null),
                new TreeDumperNode("innerLocalFunctions", node.InnerLocalFunctions, null),
                new TreeDumperNode("switchSections", null, node.SwitchSections.Select((BoundSwitchSection x) => Visit(x, null))),
                new TreeDumperNode("decisionDag", null, new TreeDumperNode[1] { Visit(node.DecisionDag, null) }),
                new TreeDumperNode("defaultLabel", null, new TreeDumperNode[1] { Visit(node.DefaultLabel, null) }),
                new TreeDumperNode("breakLabel", node.BreakLabel, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSwitchDispatch(BoundSwitchDispatch node, object? arg)
        {
            return new TreeDumperNode("switchDispatch", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("cases", node.Cases, null),
                new TreeDumperNode("defaultLabel", node.DefaultLabel, null),
                new TreeDumperNode("equalityMethod", node.EqualityMethod, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitIfStatement(BoundIfStatement node, object? arg)
        {
            return new TreeDumperNode("ifStatement", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
                new TreeDumperNode("consequence", null, new TreeDumperNode[1] { Visit(node.Consequence, null) }),
                new TreeDumperNode("alternativeOpt", null, new TreeDumperNode[1] { Visit(node.AlternativeOpt, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDoStatement(BoundDoStatement node, object? arg)
        {
            return new TreeDumperNode("doStatement", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
                new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
                new TreeDumperNode("breakLabel", node.BreakLabel, null),
                new TreeDumperNode("continueLabel", node.ContinueLabel, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitWhileStatement(BoundWhileStatement node, object? arg)
        {
            return new TreeDumperNode("whileStatement", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
                new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
                new TreeDumperNode("breakLabel", node.BreakLabel, null),
                new TreeDumperNode("continueLabel", node.ContinueLabel, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitForStatement(BoundForStatement node, object? arg)
        {
            return new TreeDumperNode("forStatement", null, new TreeDumperNode[9]
            {
                new TreeDumperNode("outerLocals", node.OuterLocals, null),
                new TreeDumperNode("initializer", null, new TreeDumperNode[1] { Visit(node.Initializer, null) }),
                new TreeDumperNode("innerLocals", node.InnerLocals, null),
                new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
                new TreeDumperNode("increment", null, new TreeDumperNode[1] { Visit(node.Increment, null) }),
                new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
                new TreeDumperNode("breakLabel", node.BreakLabel, null),
                new TreeDumperNode("continueLabel", node.ContinueLabel, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitForEachStatement(BoundForEachStatement node, object? arg)
        {
            return new TreeDumperNode("forEachStatement", null, new TreeDumperNode[13]
            {
                new TreeDumperNode("enumeratorInfoOpt", node.EnumeratorInfoOpt, null),
                new TreeDumperNode("elementConversion", node.ElementConversion, null),
                new TreeDumperNode("iterationVariableType", null, new TreeDumperNode[1] { Visit(node.IterationVariableType, null) }),
                new TreeDumperNode("iterationVariables", node.IterationVariables, null),
                new TreeDumperNode("iterationErrorExpressionOpt", null, new TreeDumperNode[1] { Visit(node.IterationErrorExpressionOpt, null) }),
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("deconstructionOpt", null, new TreeDumperNode[1] { Visit(node.DeconstructionOpt, null) }),
                new TreeDumperNode("awaitOpt", null, new TreeDumperNode[1] { Visit(node.AwaitOpt, null) }),
                new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
                new TreeDumperNode("@checked", node.Checked, null),
                new TreeDumperNode("breakLabel", node.BreakLabel, null),
                new TreeDumperNode("continueLabel", node.ContinueLabel, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitForEachDeconstructStep(BoundForEachDeconstructStep node, object? arg)
        {
            return new TreeDumperNode("forEachDeconstructStep", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("deconstructionAssignment", null, new TreeDumperNode[1] { Visit(node.DeconstructionAssignment, null) }),
                new TreeDumperNode("targetPlaceholder", null, new TreeDumperNode[1] { Visit(node.TargetPlaceholder, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitUsingStatement(BoundUsingStatement node, object? arg)
        {
            return new TreeDumperNode("usingStatement", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("declarationsOpt", null, new TreeDumperNode[1] { Visit(node.DeclarationsOpt, null) }),
                new TreeDumperNode("expressionOpt", null, new TreeDumperNode[1] { Visit(node.ExpressionOpt, null) }),
                new TreeDumperNode("iDisposableConversion", node.IDisposableConversion, null),
                new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
                new TreeDumperNode("awaitOpt", null, new TreeDumperNode[1] { Visit(node.AwaitOpt, null) }),
                new TreeDumperNode("patternDisposeInfoOpt", node.PatternDisposeInfoOpt, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitFixedStatement(BoundFixedStatement node, object? arg)
        {
            return new TreeDumperNode("fixedStatement", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("declarations", null, new TreeDumperNode[1] { Visit(node.Declarations, null) }),
                new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLockStatement(BoundLockStatement node, object? arg)
        {
            return new TreeDumperNode("lockStatement", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("argument", null, new TreeDumperNode[1] { Visit(node.Argument, null) }),
                new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitTryStatement(BoundTryStatement node, object? arg)
        {
            return new TreeDumperNode("tryStatement", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("tryBlock", null, new TreeDumperNode[1] { Visit(node.TryBlock, null) }),
                new TreeDumperNode("catchBlocks", null, node.CatchBlocks.Select((BoundCatchBlock x) => Visit(x, null))),
                new TreeDumperNode("finallyBlockOpt", null, new TreeDumperNode[1] { Visit(node.FinallyBlockOpt, null) }),
                new TreeDumperNode("finallyLabelOpt", node.FinallyLabelOpt, null),
                new TreeDumperNode("preferFaultHandler", node.PreferFaultHandler, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitCatchBlock(BoundCatchBlock node, object? arg)
        {
            return new TreeDumperNode("catchBlock", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("exceptionSourceOpt", null, new TreeDumperNode[1] { Visit(node.ExceptionSourceOpt, null) }),
                new TreeDumperNode("exceptionTypeOpt", node.ExceptionTypeOpt, null),
                new TreeDumperNode("exceptionFilterPrologueOpt", null, new TreeDumperNode[1] { Visit(node.ExceptionFilterPrologueOpt, null) }),
                new TreeDumperNode("exceptionFilterOpt", null, new TreeDumperNode[1] { Visit(node.ExceptionFilterOpt, null) }),
                new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
                new TreeDumperNode("isSynthesizedAsyncCatchAll", node.IsSynthesizedAsyncCatchAll, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLiteral(BoundLiteral node, object? arg)
        {
            return new TreeDumperNode("literal", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitThisReference(BoundThisReference node, object? arg)
        {
            return new TreeDumperNode("thisReference", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node, object? arg)
        {
            return new TreeDumperNode("previousSubmissionReference", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitHostObjectMemberReference(BoundHostObjectMemberReference node, object? arg)
        {
            return new TreeDumperNode("hostObjectMemberReference", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitBaseReference(BoundBaseReference node, object? arg)
        {
            return new TreeDumperNode("baseReference", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLocal(BoundLocal node, object? arg)
        {
            return new TreeDumperNode("local", null, new TreeDumperNode[7]
            {
                new TreeDumperNode("localSymbol", node.LocalSymbol, null),
                new TreeDumperNode("declarationKind", node.DeclarationKind, null),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("isNullableUnknown", node.IsNullableUnknown, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitPseudoVariable(BoundPseudoVariable node, object? arg)
        {
            return new TreeDumperNode("pseudoVariable", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("localSymbol", node.LocalSymbol, null),
                new TreeDumperNode("emitExpressions", node.EmitExpressions, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitRangeVariable(BoundRangeVariable node, object? arg)
        {
            return new TreeDumperNode("rangeVariable", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("rangeVariableSymbol", node.RangeVariableSymbol, null),
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitParameter(BoundParameter node, object? arg)
        {
            return new TreeDumperNode("parameter", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("parameterSymbol", node.ParameterSymbol, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLabelStatement(BoundLabelStatement node, object? arg)
        {
            return new TreeDumperNode("labelStatement", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("label", node.Label, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitGotoStatement(BoundGotoStatement node, object? arg)
        {
            return new TreeDumperNode("gotoStatement", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("label", node.Label, null),
                new TreeDumperNode("caseExpressionOpt", null, new TreeDumperNode[1] { Visit(node.CaseExpressionOpt, null) }),
                new TreeDumperNode("labelExpressionOpt", null, new TreeDumperNode[1] { Visit(node.LabelExpressionOpt, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLabeledStatement(BoundLabeledStatement node, object? arg)
        {
            return new TreeDumperNode("labeledStatement", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("label", node.Label, null),
                new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLabel(BoundLabel node, object? arg)
        {
            return new TreeDumperNode("label", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("label", node.Label, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitStatementList(BoundStatementList node, object? arg)
        {
            return new TreeDumperNode("statementList", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("statements", null, node.Statements.Select((BoundStatement x) => Visit(x, null))),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitConditionalGoto(BoundConditionalGoto node, object? arg)
        {
            return new TreeDumperNode("conditionalGoto", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
                new TreeDumperNode("jumpIfTrue", node.JumpIfTrue, null),
                new TreeDumperNode("label", node.Label, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSwitchExpressionArm(BoundSwitchExpressionArm node, object? arg)
        {
            return new TreeDumperNode("switchExpressionArm", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("pattern", null, new TreeDumperNode[1] { Visit(node.Pattern, null) }),
                new TreeDumperNode("whenClause", null, new TreeDumperNode[1] { Visit(node.WhenClause, null) }),
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("label", node.Label, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node, object? arg)
        {
            return new TreeDumperNode("unconvertedSwitchExpression", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("switchArms", null, node.SwitchArms.Select((BoundSwitchExpressionArm x) => Visit(x, null))),
                new TreeDumperNode("decisionDag", null, new TreeDumperNode[1] { Visit(node.DecisionDag, null) }),
                new TreeDumperNode("defaultLabel", node.DefaultLabel, null),
                new TreeDumperNode("reportedNotExhaustive", node.ReportedNotExhaustive, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node, object? arg)
        {
            return new TreeDumperNode("convertedSwitchExpression", null, new TreeDumperNode[11]
            {
                new TreeDumperNode("naturalTypeOpt", node.NaturalTypeOpt, null),
                new TreeDumperNode("wasTargetTyped", node.WasTargetTyped, null),
                new TreeDumperNode("conversion", node.Conversion, null),
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("switchArms", null, node.SwitchArms.Select((BoundSwitchExpressionArm x) => Visit(x, null))),
                new TreeDumperNode("decisionDag", null, new TreeDumperNode[1] { Visit(node.DecisionDag, null) }),
                new TreeDumperNode("defaultLabel", node.DefaultLabel, null),
                new TreeDumperNode("reportedNotExhaustive", node.ReportedNotExhaustive, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDecisionDag(BoundDecisionDag node, object? arg)
        {
            return new TreeDumperNode("decisionDag", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("rootNode", null, new TreeDumperNode[1] { Visit(node.RootNode, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitEvaluationDecisionDagNode(BoundEvaluationDecisionDagNode node, object? arg)
        {
            return new TreeDumperNode("evaluationDecisionDagNode", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("evaluation", null, new TreeDumperNode[1] { Visit(node.Evaluation, null) }),
                new TreeDumperNode("next", null, new TreeDumperNode[1] { Visit(node.Next, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitTestDecisionDagNode(BoundTestDecisionDagNode node, object? arg)
        {
            return new TreeDumperNode("testDecisionDagNode", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("test", null, new TreeDumperNode[1] { Visit(node.Test, null) }),
                new TreeDumperNode("whenTrue", null, new TreeDumperNode[1] { Visit(node.WhenTrue, null) }),
                new TreeDumperNode("whenFalse", null, new TreeDumperNode[1] { Visit(node.WhenFalse, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitWhenDecisionDagNode(BoundWhenDecisionDagNode node, object? arg)
        {
            return new TreeDumperNode("whenDecisionDagNode", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("bindings", node.Bindings, null),
                new TreeDumperNode("whenExpression", null, new TreeDumperNode[1] { Visit(node.WhenExpression, null) }),
                new TreeDumperNode("whenTrue", null, new TreeDumperNode[1] { Visit(node.WhenTrue, null) }),
                new TreeDumperNode("whenFalse", null, new TreeDumperNode[1] { Visit(node.WhenFalse, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLeafDecisionDagNode(BoundLeafDecisionDagNode node, object? arg)
        {
            return new TreeDumperNode("leafDecisionDagNode", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("label", node.Label, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagTemp(BoundDagTemp node, object? arg)
        {
            return new TreeDumperNode("dagTemp", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("source", null, new TreeDumperNode[1] { Visit(node.Source, null) }),
                new TreeDumperNode("index", node.Index, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagTypeTest(BoundDagTypeTest node, object? arg)
        {
            return new TreeDumperNode("dagTypeTest", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("input", null, new TreeDumperNode[1] { Visit(node.Input, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagNonNullTest(BoundDagNonNullTest node, object? arg)
        {
            return new TreeDumperNode("dagNonNullTest", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("isExplicitTest", node.IsExplicitTest, null),
                new TreeDumperNode("input", null, new TreeDumperNode[1] { Visit(node.Input, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagExplicitNullTest(BoundDagExplicitNullTest node, object? arg)
        {
            return new TreeDumperNode("dagExplicitNullTest", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("input", null, new TreeDumperNode[1] { Visit(node.Input, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagValueTest(BoundDagValueTest node, object? arg)
        {
            return new TreeDumperNode("dagValueTest", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("value", node.Value, null),
                new TreeDumperNode("input", null, new TreeDumperNode[1] { Visit(node.Input, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagRelationalTest(BoundDagRelationalTest node, object? arg)
        {
            return new TreeDumperNode("dagRelationalTest", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("operatorKind", node.OperatorKind, null),
                new TreeDumperNode("value", node.Value, null),
                new TreeDumperNode("input", null, new TreeDumperNode[1] { Visit(node.Input, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagDeconstructEvaluation(BoundDagDeconstructEvaluation node, object? arg)
        {
            return new TreeDumperNode("dagDeconstructEvaluation", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("deconstructMethod", node.DeconstructMethod, null),
                new TreeDumperNode("input", null, new TreeDumperNode[1] { Visit(node.Input, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagTypeEvaluation(BoundDagTypeEvaluation node, object? arg)
        {
            return new TreeDumperNode("dagTypeEvaluation", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("input", null, new TreeDumperNode[1] { Visit(node.Input, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagFieldEvaluation(BoundDagFieldEvaluation node, object? arg)
        {
            return new TreeDumperNode("dagFieldEvaluation", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("field", node.Field, null),
                new TreeDumperNode("input", null, new TreeDumperNode[1] { Visit(node.Input, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagPropertyEvaluation(BoundDagPropertyEvaluation node, object? arg)
        {
            return new TreeDumperNode("dagPropertyEvaluation", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("property", node.Property, null),
                new TreeDumperNode("input", null, new TreeDumperNode[1] { Visit(node.Input, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDagIndexEvaluation(BoundDagIndexEvaluation node, object? arg)
        {
            return new TreeDumperNode("dagIndexEvaluation", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("property", node.Property, null),
                new TreeDumperNode("index", node.Index, null),
                new TreeDumperNode("input", null, new TreeDumperNode[1] { Visit(node.Input, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSwitchSection(BoundSwitchSection node, object? arg)
        {
            return new TreeDumperNode("switchSection", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("switchLabels", null, node.SwitchLabels.Select((BoundSwitchLabel x) => Visit(x, null))),
                new TreeDumperNode("statements", null, node.Statements.Select((BoundStatement x) => Visit(x, null))),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSwitchLabel(BoundSwitchLabel node, object? arg)
        {
            return new TreeDumperNode("switchLabel", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("label", node.Label, null),
                new TreeDumperNode("pattern", null, new TreeDumperNode[1] { Visit(node.Pattern, null) }),
                new TreeDumperNode("whenClause", null, new TreeDumperNode[1] { Visit(node.WhenClause, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSequencePointExpression(BoundSequencePointExpression node, object? arg)
        {
            return new TreeDumperNode("sequencePointExpression", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSequence(BoundSequence node, object? arg)
        {
            return new TreeDumperNode("sequence", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("sideEffects", null, node.SideEffects.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSpillSequence(BoundSpillSequence node, object? arg)
        {
            return new TreeDumperNode("spillSequence", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("sideEffects", null, node.SideEffects.Select((BoundStatement x) => Visit(x, null))),
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDynamicMemberAccess(BoundDynamicMemberAccess node, object? arg)
        {
            return new TreeDumperNode("dynamicMemberAccess", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("receiver", null, new TreeDumperNode[1] { Visit(node.Receiver, null) }),
                new TreeDumperNode("typeArgumentsOpt", node.TypeArgumentsOpt, null),
                new TreeDumperNode("name", node.Name, null),
                new TreeDumperNode("invoked", node.Invoked, null),
                new TreeDumperNode("indexed", node.Indexed, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDynamicInvocation(BoundDynamicInvocation node, object? arg)
        {
            return new TreeDumperNode("dynamicInvocation", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
                new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
                new TreeDumperNode("applicableMethods", node.ApplicableMethods, null),
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitConditionalAccess(BoundConditionalAccess node, object? arg)
        {
            return new TreeDumperNode("conditionalAccess", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("receiver", null, new TreeDumperNode[1] { Visit(node.Receiver, null) }),
                new TreeDumperNode("accessExpression", null, new TreeDumperNode[1] { Visit(node.AccessExpression, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node, object? arg)
        {
            return new TreeDumperNode("loweredConditionalAccess", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("receiver", null, new TreeDumperNode[1] { Visit(node.Receiver, null) }),
                new TreeDumperNode("hasValueMethodOpt", node.HasValueMethodOpt, null),
                new TreeDumperNode("whenNotNull", null, new TreeDumperNode[1] { Visit(node.WhenNotNull, null) }),
                new TreeDumperNode("whenNullOpt", null, new TreeDumperNode[1] { Visit(node.WhenNullOpt, null) }),
                new TreeDumperNode("id", node.Id, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitConditionalReceiver(BoundConditionalReceiver node, object? arg)
        {
            return new TreeDumperNode("conditionalReceiver", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("id", node.Id, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node, object? arg)
        {
            return new TreeDumperNode("complexConditionalReceiver", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("valueTypeReceiver", null, new TreeDumperNode[1] { Visit(node.ValueTypeReceiver, null) }),
                new TreeDumperNode("referenceTypeReceiver", null, new TreeDumperNode[1] { Visit(node.ReferenceTypeReceiver, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitMethodGroup(BoundMethodGroup node, object? arg)
        {
            return new TreeDumperNode("methodGroup", null, new TreeDumperNode[11]
            {
                new TreeDumperNode("typeArgumentsOpt", node.TypeArgumentsOpt, null),
                new TreeDumperNode("name", node.Name, null),
                new TreeDumperNode("methods", node.Methods, null),
                new TreeDumperNode("lookupSymbolOpt", node.LookupSymbolOpt, null),
                new TreeDumperNode("lookupError", node.LookupError, null),
                new TreeDumperNode("flags", node.Flags, null),
                new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitPropertyGroup(BoundPropertyGroup node, object? arg)
        {
            return new TreeDumperNode("propertyGroup", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("properties", node.Properties, null),
                new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitCall(BoundCall node, object? arg)
        {
            return new TreeDumperNode("call", null, new TreeDumperNode[15]
            {
                new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
                new TreeDumperNode("method", node.Method, null),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
                new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
                new TreeDumperNode("isDelegateCall", node.IsDelegateCall, null),
                new TreeDumperNode("expanded", node.Expanded, null),
                new TreeDumperNode("invokedAsExtensionMethod", node.InvokedAsExtensionMethod, null),
                new TreeDumperNode("argsToParamsOpt", node.ArgsToParamsOpt, null),
                new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("originalMethodsOpt", node.OriginalMethodsOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitEventAssignmentOperator(BoundEventAssignmentOperator node, object? arg)
        {
            return new TreeDumperNode("eventAssignmentOperator", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("@event", node.Event, null),
                new TreeDumperNode("isAddition", node.IsAddition, null),
                new TreeDumperNode("isDynamic", node.IsDynamic, null),
                new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
                new TreeDumperNode("argument", null, new TreeDumperNode[1] { Visit(node.Argument, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitAttribute(BoundAttribute node, object? arg)
        {
            return new TreeDumperNode("attribute", null, new TreeDumperNode[10]
            {
                new TreeDumperNode("constructor", node.Constructor, null),
                new TreeDumperNode("constructorArguments", null, node.ConstructorArguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("constructorArgumentNamesOpt", node.ConstructorArgumentNamesOpt, null),
                new TreeDumperNode("constructorArgumentsToParamsOpt", node.ConstructorArgumentsToParamsOpt, null),
                new TreeDumperNode("constructorExpanded", node.ConstructorExpanded, null),
                new TreeDumperNode("namedArguments", null, node.NamedArguments.Select((BoundAssignmentOperator x) => Visit(x, null))),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node, object? arg)
        {
            return new TreeDumperNode("unconvertedObjectCreationExpression", null, new TreeDumperNode[7]
            {
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
                new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
                new TreeDumperNode("initializerOpt", node.InitializerOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitObjectCreationExpression(BoundObjectCreationExpression node, object? arg)
        {
            return new TreeDumperNode("objectCreationExpression", null, new TreeDumperNode[14]
            {
                new TreeDumperNode("constructor", node.Constructor, null),
                new TreeDumperNode("constructorsGroup", node.ConstructorsGroup, null),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
                new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
                new TreeDumperNode("expanded", node.Expanded, null),
                new TreeDumperNode("argsToParamsOpt", node.ArgsToParamsOpt, null),
                new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("initializerExpressionOpt", null, new TreeDumperNode[1] { Visit(node.InitializerExpressionOpt, null) }),
                new TreeDumperNode("wasTargetTyped", node.WasTargetTyped, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitTupleLiteral(BoundTupleLiteral node, object? arg)
        {
            return new TreeDumperNode("tupleLiteral", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
                new TreeDumperNode("inferredNamesOpt", node.InferredNamesOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node, object? arg)
        {
            return new TreeDumperNode("convertedTupleLiteral", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("sourceTuple", null, new TreeDumperNode[1] { Visit(node.SourceTuple, null) }),
                new TreeDumperNode("wasTargetTyped", node.WasTargetTyped, null),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
                new TreeDumperNode("inferredNamesOpt", node.InferredNamesOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node, object? arg)
        {
            return new TreeDumperNode("dynamicObjectCreationExpression", null, new TreeDumperNode[9]
            {
                new TreeDumperNode("name", node.Name, null),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
                new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
                new TreeDumperNode("initializerExpressionOpt", null, new TreeDumperNode[1] { Visit(node.InitializerExpressionOpt, null) }),
                new TreeDumperNode("applicableMethods", node.ApplicableMethods, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node, object? arg)
        {
            return new TreeDumperNode("noPiaObjectCreationExpression", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("guidString", node.GuidString, null),
                new TreeDumperNode("initializerExpressionOpt", null, new TreeDumperNode[1] { Visit(node.InitializerExpressionOpt, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node, object? arg)
        {
            return new TreeDumperNode("objectInitializerExpression", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("placeholder", null, new TreeDumperNode[1] { Visit(node.Placeholder, null) }),
                new TreeDumperNode("initializers", null, node.Initializers.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitObjectInitializerMember(BoundObjectInitializerMember node, object? arg)
        {
            return new TreeDumperNode("objectInitializerMember", null, new TreeDumperNode[12]
            {
                new TreeDumperNode("memberSymbol", node.MemberSymbol, null),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
                new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
                new TreeDumperNode("expanded", node.Expanded, null),
                new TreeDumperNode("argsToParamsOpt", node.ArgsToParamsOpt, null),
                new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("receiverType", node.ReceiverType, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node, object? arg)
        {
            return new TreeDumperNode("dynamicObjectInitializerMember", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("memberName", node.MemberName, null),
                new TreeDumperNode("receiverType", node.ReceiverType, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node, object? arg)
        {
            return new TreeDumperNode("collectionInitializerExpression", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("placeholder", null, new TreeDumperNode[1] { Visit(node.Placeholder, null) }),
                new TreeDumperNode("initializers", null, node.Initializers.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitCollectionElementInitializer(BoundCollectionElementInitializer node, object? arg)
        {
            return new TreeDumperNode("collectionElementInitializer", null, new TreeDumperNode[11]
            {
                new TreeDumperNode("addMethod", node.AddMethod, null),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("implicitReceiverOpt", null, new TreeDumperNode[1] { Visit(node.ImplicitReceiverOpt, null) }),
                new TreeDumperNode("expanded", node.Expanded, null),
                new TreeDumperNode("argsToParamsOpt", node.ArgsToParamsOpt, null),
                new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
                new TreeDumperNode("invokedAsExtensionMethod", node.InvokedAsExtensionMethod, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node, object? arg)
        {
            return new TreeDumperNode("dynamicCollectionElementInitializer", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("applicableMethods", node.ApplicableMethods, null),
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitImplicitReceiver(BoundImplicitReceiver node, object? arg)
        {
            return new TreeDumperNode("implicitReceiver", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node, object? arg)
        {
            return new TreeDumperNode("anonymousObjectCreationExpression", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("constructor", node.Constructor, null),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("declarations", null, node.Declarations.Select((BoundAnonymousPropertyDeclaration x) => Visit(x, null))),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node, object? arg)
        {
            return new TreeDumperNode("anonymousPropertyDeclaration", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("property", node.Property, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitNewT(BoundNewT node, object? arg)
        {
            return new TreeDumperNode("newT", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("initializerExpressionOpt", null, new TreeDumperNode[1] { Visit(node.InitializerExpressionOpt, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node, object? arg)
        {
            return new TreeDumperNode("delegateCreationExpression", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("argument", null, new TreeDumperNode[1] { Visit(node.Argument, null) }),
                new TreeDumperNode("methodOpt", node.MethodOpt, null),
                new TreeDumperNode("isExtensionMethod", node.IsExtensionMethod, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitArrayCreation(BoundArrayCreation node, object? arg)
        {
            return new TreeDumperNode("arrayCreation", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("bounds", null, node.Bounds.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("initializerOpt", null, new TreeDumperNode[1] { Visit(node.InitializerOpt, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitArrayInitialization(BoundArrayInitialization node, object? arg)
        {
            return new TreeDumperNode("arrayInitialization", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("initializers", null, node.Initializers.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node, object? arg)
        {
            return new TreeDumperNode("stackAllocArrayCreation", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("elementType", node.ElementType, null),
                new TreeDumperNode("count", null, new TreeDumperNode[1] { Visit(node.Count, null) }),
                new TreeDumperNode("initializerOpt", null, new TreeDumperNode[1] { Visit(node.InitializerOpt, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node, object? arg)
        {
            return new TreeDumperNode("convertedStackAllocExpression", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("elementType", node.ElementType, null),
                new TreeDumperNode("count", null, new TreeDumperNode[1] { Visit(node.Count, null) }),
                new TreeDumperNode("initializerOpt", null, new TreeDumperNode[1] { Visit(node.InitializerOpt, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitFieldAccess(BoundFieldAccess node, object? arg)
        {
            return new TreeDumperNode("fieldAccess", null, new TreeDumperNode[9]
            {
                new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
                new TreeDumperNode("fieldSymbol", node.FieldSymbol, null),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("isByValue", node.IsByValue, null),
                new TreeDumperNode("isDeclaration", node.IsDeclaration, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitHoistedFieldAccess(BoundHoistedFieldAccess node, object? arg)
        {
            return new TreeDumperNode("hoistedFieldAccess", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("fieldSymbol", node.FieldSymbol, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitPropertyAccess(BoundPropertyAccess node, object? arg)
        {
            return new TreeDumperNode("propertyAccess", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
                new TreeDumperNode("propertySymbol", node.PropertySymbol, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitEventAccess(BoundEventAccess node, object? arg)
        {
            return new TreeDumperNode("eventAccess", null, new TreeDumperNode[7]
            {
                new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
                new TreeDumperNode("eventSymbol", node.EventSymbol, null),
                new TreeDumperNode("isUsableAsField", node.IsUsableAsField, null),
                new TreeDumperNode("resultKind", node.ResultKind, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitIndexerAccess(BoundIndexerAccess node, object? arg)
        {
            return new TreeDumperNode("indexerAccess", null, new TreeDumperNode[12]
            {
                new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
                new TreeDumperNode("indexer", node.Indexer, null),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
                new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
                new TreeDumperNode("expanded", node.Expanded, null),
                new TreeDumperNode("argsToParamsOpt", node.ArgsToParamsOpt, null),
                new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
                new TreeDumperNode("originalIndexersOpt", node.OriginalIndexersOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node, object? arg)
        {
            return new TreeDumperNode("indexOrRangePatternIndexerAccess", null, new TreeDumperNode[7]
            {
                new TreeDumperNode("receiver", null, new TreeDumperNode[1] { Visit(node.Receiver, null) }),
                new TreeDumperNode("lengthOrCountProperty", node.LengthOrCountProperty, null),
                new TreeDumperNode("patternSymbol", node.PatternSymbol, null),
                new TreeDumperNode("argument", null, new TreeDumperNode[1] { Visit(node.Argument, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node, object? arg)
        {
            return new TreeDumperNode("dynamicIndexerAccess", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("receiver", null, new TreeDumperNode[1] { Visit(node.Receiver, null) }),
                new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
                new TreeDumperNode("argumentRefKindsOpt", node.ArgumentRefKindsOpt, null),
                new TreeDumperNode("applicableIndexers", node.ApplicableIndexers, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitLambda(BoundLambda node, object? arg)
        {
            return new TreeDumperNode("lambda", null, new TreeDumperNode[8]
            {
                new TreeDumperNode("unboundLambda", null, new TreeDumperNode[1] { Visit(node.UnboundLambda, null) }),
                new TreeDumperNode("symbol", node.Symbol, null),
                new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
                new TreeDumperNode("diagnostics", node.Diagnostics, null),
                new TreeDumperNode("binder", node.Binder, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitUnboundLambda(UnboundLambda node, object? arg)
        {
            return new TreeDumperNode("unboundLambda", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("data", node.Data, null),
                new TreeDumperNode("withDependencies", node.WithDependencies, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitQueryClause(BoundQueryClause node, object? arg)
        {
            return new TreeDumperNode("queryClause", null, new TreeDumperNode[9]
            {
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("definedSymbol", node.DefinedSymbol, null),
                new TreeDumperNode("operation", null, new TreeDumperNode[1] { Visit(node.Operation, null) }),
                new TreeDumperNode("cast", null, new TreeDumperNode[1] { Visit(node.Cast, null) }),
                new TreeDumperNode("binder", node.Binder, null),
                new TreeDumperNode("unoptimizedForm", null, new TreeDumperNode[1] { Visit(node.UnoptimizedForm, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node, object? arg)
        {
            return new TreeDumperNode("typeOrInstanceInitializers", null, new TreeDumperNode[2]
            {
                new TreeDumperNode("statements", null, node.Statements.Select((BoundStatement x) => Visit(x, null))),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitNameOfOperator(BoundNameOfOperator node, object? arg)
        {
            return new TreeDumperNode("nameOfOperator", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("argument", null, new TreeDumperNode[1] { Visit(node.Argument, null) }),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node, object? arg)
        {
            return new TreeDumperNode("unconvertedInterpolatedString", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("parts", null, node.Parts.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitInterpolatedString(BoundInterpolatedString node, object? arg)
        {
            return new TreeDumperNode("interpolatedString", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("parts", null, node.Parts.Select((BoundExpression x) => Visit(x, null))),
                new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitStringInsert(BoundStringInsert node, object? arg)
        {
            return new TreeDumperNode("stringInsert", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("alignment", null, new TreeDumperNode[1] { Visit(node.Alignment, null) }),
                new TreeDumperNode("format", null, new TreeDumperNode[1] { Visit(node.Format, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitIsPatternExpression(BoundIsPatternExpression node, object? arg)
        {
            return new TreeDumperNode("isPatternExpression", null, new TreeDumperNode[9]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("pattern", null, new TreeDumperNode[1] { Visit(node.Pattern, null) }),
                new TreeDumperNode("isNegated", node.IsNegated, null),
                new TreeDumperNode("decisionDag", null, new TreeDumperNode[1] { Visit(node.DecisionDag, null) }),
                new TreeDumperNode("whenTrueLabel", node.WhenTrueLabel, null),
                new TreeDumperNode("whenFalseLabel", node.WhenFalseLabel, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitConstantPattern(BoundConstantPattern node, object? arg)
        {
            return new TreeDumperNode("constantPattern", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("constantValue", node.ConstantValue, null),
                new TreeDumperNode("inputType", node.InputType, null),
                new TreeDumperNode("narrowedType", node.NarrowedType, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDiscardPattern(BoundDiscardPattern node, object? arg)
        {
            return new TreeDumperNode("discardPattern", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("inputType", node.InputType, null),
                new TreeDumperNode("narrowedType", node.NarrowedType, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDeclarationPattern(BoundDeclarationPattern node, object? arg)
        {
            return new TreeDumperNode("declarationPattern", null, new TreeDumperNode[7]
            {
                new TreeDumperNode("variable", node.Variable, null),
                new TreeDumperNode("variableAccess", null, new TreeDumperNode[1] { Visit(node.VariableAccess, null) }),
                new TreeDumperNode("declaredType", null, new TreeDumperNode[1] { Visit(node.DeclaredType, null) }),
                new TreeDumperNode("isVar", node.IsVar, null),
                new TreeDumperNode("inputType", node.InputType, null),
                new TreeDumperNode("narrowedType", node.NarrowedType, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitRecursivePattern(BoundRecursivePattern node, object? arg)
        {
            TreeDumperNode[] obj = new TreeDumperNode[10]
            {
                new TreeDumperNode("declaredType", null, new TreeDumperNode[1] { Visit(node.DeclaredType, null) }),
                new TreeDumperNode("deconstructMethod", node.DeconstructMethod, null),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            };
            IEnumerable<TreeDumperNode> children;
            if (!node.Deconstruction.IsDefault)
            {
                children = node.Deconstruction.Select((BoundSubpattern x) => Visit(x, null));
            }
            else
            {
                IEnumerable<TreeDumperNode> enumerable = new TreeDumperNode[0];
                children = enumerable;
            }
            obj[2] = new TreeDumperNode("deconstruction", null, children);
            IEnumerable<TreeDumperNode> children2;
            if (!node.Properties.IsDefault)
            {
                children2 = node.Properties.Select((BoundSubpattern x) => Visit(x, null));
            }
            else
            {
                IEnumerable<TreeDumperNode> enumerable = new TreeDumperNode[0];
                children2 = enumerable;
            }
            obj[3] = new TreeDumperNode("properties", null, children2);
            obj[4] = new TreeDumperNode("variable", node.Variable, null);
            obj[5] = new TreeDumperNode("variableAccess", null, new TreeDumperNode[1] { Visit(node.VariableAccess, null) });
            obj[6] = new TreeDumperNode("isExplicitNotNullTest", node.IsExplicitNotNullTest, null);
            obj[7] = new TreeDumperNode("inputType", node.InputType, null);
            obj[8] = new TreeDumperNode("narrowedType", node.NarrowedType, null);
            obj[9] = new TreeDumperNode("hasErrors", node.HasErrors, null);
            return new TreeDumperNode("recursivePattern", null, obj);
        }

        public override TreeDumperNode VisitITuplePattern(BoundITuplePattern node, object? arg)
        {
            return new TreeDumperNode("iTuplePattern", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("getLengthMethod", node.GetLengthMethod, null),
                new TreeDumperNode("getItemMethod", node.GetItemMethod, null),
                new TreeDumperNode("subpatterns", null, node.Subpatterns.Select((BoundSubpattern x) => Visit(x, null))),
                new TreeDumperNode("inputType", node.InputType, null),
                new TreeDumperNode("narrowedType", node.NarrowedType, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitSubpattern(BoundSubpattern node, object? arg)
        {
            return new TreeDumperNode("subpattern", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("symbol", node.Symbol, null),
                new TreeDumperNode("pattern", null, new TreeDumperNode[1] { Visit(node.Pattern, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitTypePattern(BoundTypePattern node, object? arg)
        {
            return new TreeDumperNode("typePattern", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("declaredType", null, new TreeDumperNode[1] { Visit(node.DeclaredType, null) }),
                new TreeDumperNode("isExplicitNotNullTest", node.IsExplicitNotNullTest, null),
                new TreeDumperNode("inputType", node.InputType, null),
                new TreeDumperNode("narrowedType", node.NarrowedType, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitBinaryPattern(BoundBinaryPattern node, object? arg)
        {
            return new TreeDumperNode("binaryPattern", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("disjunction", node.Disjunction, null),
                new TreeDumperNode("left", null, new TreeDumperNode[1] { Visit(node.Left, null) }),
                new TreeDumperNode("right", null, new TreeDumperNode[1] { Visit(node.Right, null) }),
                new TreeDumperNode("inputType", node.InputType, null),
                new TreeDumperNode("narrowedType", node.NarrowedType, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitNegatedPattern(BoundNegatedPattern node, object? arg)
        {
            return new TreeDumperNode("negatedPattern", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("negated", null, new TreeDumperNode[1] { Visit(node.Negated, null) }),
                new TreeDumperNode("inputType", node.InputType, null),
                new TreeDumperNode("narrowedType", node.NarrowedType, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitRelationalPattern(BoundRelationalPattern node, object? arg)
        {
            return new TreeDumperNode("relationalPattern", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("relation", node.Relation, null),
                new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
                new TreeDumperNode("constantValue", node.ConstantValue, null),
                new TreeDumperNode("inputType", node.InputType, null),
                new TreeDumperNode("narrowedType", node.NarrowedType, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDiscardExpression(BoundDiscardExpression node, object? arg)
        {
            return new TreeDumperNode("discardExpression", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitThrowExpression(BoundThrowExpression node, object? arg)
        {
            return new TreeDumperNode("throwExpression", null, new TreeDumperNode[4]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitOutVariablePendingInference(OutVariablePendingInference node, object? arg)
        {
            return new TreeDumperNode("outVariablePendingInference", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("variableSymbol", node.VariableSymbol, null),
                new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node, object? arg)
        {
            return new TreeDumperNode("deconstructionVariablePendingInference", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("variableSymbol", node.VariableSymbol, null),
                new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node, object? arg)
        {
            return new TreeDumperNode("outDeconstructVarPendingInference", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node, object? arg)
        {
            return new TreeDumperNode("nonConstructorMethodBody", null, new TreeDumperNode[3]
            {
                new TreeDumperNode("blockBody", null, new TreeDumperNode[1] { Visit(node.BlockBody, null) }),
                new TreeDumperNode("expressionBody", null, new TreeDumperNode[1] { Visit(node.ExpressionBody, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitConstructorMethodBody(BoundConstructorMethodBody node, object? arg)
        {
            return new TreeDumperNode("constructorMethodBody", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("locals", node.Locals, null),
                new TreeDumperNode("initializer", null, new TreeDumperNode[1] { Visit(node.Initializer, null) }),
                new TreeDumperNode("blockBody", null, new TreeDumperNode[1] { Visit(node.BlockBody, null) }),
                new TreeDumperNode("expressionBody", null, new TreeDumperNode[1] { Visit(node.ExpressionBody, null) }),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitExpressionWithNullability(BoundExpressionWithNullability node, object? arg)
        {
            return new TreeDumperNode("expressionWithNullability", null, new TreeDumperNode[5]
            {
                new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
                new TreeDumperNode("nullableAnnotation", node.NullableAnnotation, null),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }

        public override TreeDumperNode VisitWithExpression(BoundWithExpression node, object? arg)
        {
            return new TreeDumperNode("withExpression", null, new TreeDumperNode[6]
            {
                new TreeDumperNode("receiver", null, new TreeDumperNode[1] { Visit(node.Receiver, null) }),
                new TreeDumperNode("cloneMethod", node.CloneMethod, null),
                new TreeDumperNode("initializerExpression", null, new TreeDumperNode[1] { Visit(node.InitializerExpression, null) }),
                new TreeDumperNode("type", node.Type, null),
                new TreeDumperNode("isSuppressed", node.IsSuppressed, null),
                new TreeDumperNode("hasErrors", node.HasErrors, null)
            });
        }
    }
}

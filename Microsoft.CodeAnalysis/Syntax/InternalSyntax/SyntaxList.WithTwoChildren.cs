// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Syntax.InternalSyntax
{
    public partial class SyntaxList
    {
        public class WithTwoChildren : SyntaxList
        {
            static WithTwoChildren()
            {
                ObjectBinder.RegisterTypeReader(typeof(WithTwoChildren), r => new WithTwoChildren(r));
            }

            private readonly GreenNode _child0;
            private readonly GreenNode _child1;

            public WithTwoChildren(GreenNode child0, GreenNode child1)
            {
                this.SlotCount = 2;
                this.AdjustFlagsAndWidth(child0);
                _child0 = child0;
                this.AdjustFlagsAndWidth(child1);
                _child1 = child1;
            }

            public WithTwoChildren(DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations, GreenNode child0, GreenNode child1)
                : base(diagnostics, annotations)
            {
                this.SlotCount = 2;
                this.AdjustFlagsAndWidth(child0);
                _child0 = child0;
                this.AdjustFlagsAndWidth(child1);
                _child1 = child1;
            }

            public WithTwoChildren(ObjectReader reader)
                : base(reader)
            {
                this.SlotCount = 2;
                _child0 = (GreenNode)reader.ReadValue();
                this.AdjustFlagsAndWidth(_child0);
                _child1 = (GreenNode)reader.ReadValue();
                this.AdjustFlagsAndWidth(_child1);
            }

            public override void WriteTo(ObjectWriter writer)
            {
                base.WriteTo(writer);
                writer.WriteValue(_child0);
                writer.WriteValue(_child1);
            }

            public override GreenNode? GetSlot(int index)
            {
                return index switch
                {
                    0 => _child0,
                    1 => _child1,
                    _ => null,
                };
            }

            internal override void CopyTo(ArrayElement<GreenNode>[] array, int offset)
            {
                array[offset].Value = _child0;
                array[offset + 1].Value = _child1;
            }

            public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
            {
                return new Syntax.SyntaxList.WithTwoChildren(this, parent, position);
            }

            public override GreenNode SetDiagnostics(DiagnosticInfo[]? errors)
            {
                return new WithTwoChildren(errors, this.GetAnnotations(), _child0, _child1);
            }

            public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
            {
                return new WithTwoChildren(GetDiagnostics(), annotations, _child0, _child1);
            }
        }
    }
}

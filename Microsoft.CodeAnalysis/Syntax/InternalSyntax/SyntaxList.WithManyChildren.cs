// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Syntax.InternalSyntax
{
    public partial class SyntaxList
    {
        internal abstract class WithManyChildrenBase : SyntaxList
        {
            internal readonly ArrayElement<GreenNode>[] children;

            internal WithManyChildrenBase(ArrayElement<GreenNode>[] children)
            {
                this.children = children;
                this.InitializeChildren();
            }

            internal WithManyChildrenBase(DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations, ArrayElement<GreenNode>[] children)
                : base(diagnostics, annotations)
            {
                this.children = children;
                this.InitializeChildren();
            }

            private void InitializeChildren()
            {
                int n = children.Length;
                if (n < byte.MaxValue)
                {
                    this.SlotCount = (byte)n;
                }
                else
                {
                    this.SlotCount = byte.MaxValue;
                }

                for (int i = 0; i < children.Length; i++)
                {
                    this.AdjustFlagsAndWidth(children[i]);
                }
            }

            internal WithManyChildrenBase(ObjectReader reader)
                : base(reader)
            {
                var length = reader.ReadInt32();

                this.children = new ArrayElement<GreenNode>[length];
                for (var i = 0; i < length; i++)
                {
                    this.children[i].Value = (GreenNode)reader.ReadValue();
                }

                this.InitializeChildren();
            }

            public override void WriteTo(ObjectWriter writer)
            {
                base.WriteTo(writer);

                // PERF: Write the array out manually.Profiling shows that this is cheaper than converting to 
                // an array in order to use writer.WriteValue.
                writer.WriteInt32(this.children.Length);

                for (var i = 0; i < this.children.Length; i++)
                {
                    writer.WriteValue(this.children[i].Value);
                }
            }

            protected override int GetSlotCount()
            {
                return children.Length;
            }

            public override GreenNode GetSlot(int index)
            {
                return this.children[index];
            }

            internal override void CopyTo(ArrayElement<GreenNode>[] array, int offset)
            {
                Array.Copy(this.children, 0, array, offset, this.children.Length);
            }

            public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
            {
                var separated = this.SlotCount > 1 && HasNodeTokenPattern();
                if (parent != null && parent.ShouldCreateWeakList())
                {
                    return separated
                        ? new Syntax.SyntaxList.SeparatedWithManyWeakChildren(this, parent, position)
                        : new Syntax.SyntaxList.WithManyWeakChildren(this, parent, position);
                }
                else
                {
                    return separated
                        ? new Syntax.SyntaxList.SeparatedWithManyChildren(this, parent, position)
                        : new Syntax.SyntaxList.WithManyChildren(this, parent, position);
                }
            }

            private bool HasNodeTokenPattern()
            {
                for (int i = 0; i < this.SlotCount; i++)
                {
                    // even slots must not be tokens, odds slots must be tokens
                    if (this.GetSlot(i).IsToken == ((i & 1) == 0))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        internal sealed class WithManyChildren : WithManyChildrenBase
        {
            static WithManyChildren()
            {
                ObjectBinder.RegisterTypeReader(typeof(WithManyChildren), r => new WithManyChildren(r));
            }

            internal WithManyChildren(ArrayElement<GreenNode>[] children)
                : base(children)
            {
            }

            internal WithManyChildren(DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations, ArrayElement<GreenNode>[] children)
                : base(diagnostics, annotations, children)
            {
            }

            internal WithManyChildren(ObjectReader reader)
                : base(reader)
            {
            }

            public override GreenNode SetDiagnostics(DiagnosticInfo[]? errors)
            {
                return new WithManyChildren(errors, this.GetAnnotations(), children);
            }

            public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
            {
                return new WithManyChildren(GetDiagnostics(), annotations, children);
            }
        }
    }
}

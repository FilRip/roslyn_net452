using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class FunctionPointerCallingConventionSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken managedOrUnmanagedKeyword;

        internal readonly FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList;

        public SyntaxToken ManagedOrUnmanagedKeyword => managedOrUnmanagedKeyword;

        public FunctionPointerUnmanagedCallingConventionListSyntax? UnmanagedCallingConventionList => unmanagedCallingConventionList;

        public FunctionPointerCallingConventionSyntax(SyntaxKind kind, SyntaxToken managedOrUnmanagedKeyword, FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(managedOrUnmanagedKeyword);
            this.managedOrUnmanagedKeyword = managedOrUnmanagedKeyword;
            if (unmanagedCallingConventionList != null)
            {
                AdjustFlagsAndWidth(unmanagedCallingConventionList);
                this.unmanagedCallingConventionList = unmanagedCallingConventionList;
            }
        }

        public FunctionPointerCallingConventionSyntax(SyntaxKind kind, SyntaxToken managedOrUnmanagedKeyword, FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(managedOrUnmanagedKeyword);
            this.managedOrUnmanagedKeyword = managedOrUnmanagedKeyword;
            if (unmanagedCallingConventionList != null)
            {
                AdjustFlagsAndWidth(unmanagedCallingConventionList);
                this.unmanagedCallingConventionList = unmanagedCallingConventionList;
            }
        }

        public FunctionPointerCallingConventionSyntax(SyntaxKind kind, SyntaxToken managedOrUnmanagedKeyword, FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(managedOrUnmanagedKeyword);
            this.managedOrUnmanagedKeyword = managedOrUnmanagedKeyword;
            if (unmanagedCallingConventionList != null)
            {
                AdjustFlagsAndWidth(unmanagedCallingConventionList);
                this.unmanagedCallingConventionList = unmanagedCallingConventionList;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => managedOrUnmanagedKeyword,
                1 => unmanagedCallingConventionList,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerCallingConventionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFunctionPointerCallingConvention(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitFunctionPointerCallingConvention(this);
        }

        public FunctionPointerCallingConventionSyntax Update(SyntaxToken managedOrUnmanagedKeyword, FunctionPointerUnmanagedCallingConventionListSyntax unmanagedCallingConventionList)
        {
            if (managedOrUnmanagedKeyword != ManagedOrUnmanagedKeyword || unmanagedCallingConventionList != UnmanagedCallingConventionList)
            {
                FunctionPointerCallingConventionSyntax functionPointerCallingConventionSyntax = SyntaxFactory.FunctionPointerCallingConvention(managedOrUnmanagedKeyword, unmanagedCallingConventionList);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    functionPointerCallingConventionSyntax = functionPointerCallingConventionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    functionPointerCallingConventionSyntax = functionPointerCallingConventionSyntax.WithAnnotationsGreen(annotations);
                }
                return functionPointerCallingConventionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new FunctionPointerCallingConventionSyntax(base.Kind, managedOrUnmanagedKeyword, unmanagedCallingConventionList, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new FunctionPointerCallingConventionSyntax(base.Kind, managedOrUnmanagedKeyword, unmanagedCallingConventionList, GetDiagnostics(), annotations);
        }

        public FunctionPointerCallingConventionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            managedOrUnmanagedKeyword = node;
            FunctionPointerUnmanagedCallingConventionListSyntax functionPointerUnmanagedCallingConventionListSyntax = (FunctionPointerUnmanagedCallingConventionListSyntax)reader.ReadValue();
            if (functionPointerUnmanagedCallingConventionListSyntax != null)
            {
                AdjustFlagsAndWidth(functionPointerUnmanagedCallingConventionListSyntax);
                unmanagedCallingConventionList = functionPointerUnmanagedCallingConventionListSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(managedOrUnmanagedKeyword);
            writer.WriteValue(unmanagedCallingConventionList);
        }

        static FunctionPointerCallingConventionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(FunctionPointerCallingConventionSyntax), (ObjectReader r) => new FunctionPointerCallingConventionSyntax(r));
        }
    }
}

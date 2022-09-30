using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class FunctionPointerUnmanagedCallingConventionSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken name;

        public SyntaxToken Name => name;

        public FunctionPointerUnmanagedCallingConventionSyntax(SyntaxKind kind, SyntaxToken name, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public FunctionPointerUnmanagedCallingConventionSyntax(SyntaxKind kind, SyntaxToken name, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public FunctionPointerUnmanagedCallingConventionSyntax(SyntaxKind kind, SyntaxToken name)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(name);
            this.name = name;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return name;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.FunctionPointerUnmanagedCallingConventionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFunctionPointerUnmanagedCallingConvention(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitFunctionPointerUnmanagedCallingConvention(this);
        }

        public FunctionPointerUnmanagedCallingConventionSyntax Update(SyntaxToken name)
        {
            if (name != Name)
            {
                FunctionPointerUnmanagedCallingConventionSyntax functionPointerUnmanagedCallingConventionSyntax = SyntaxFactory.FunctionPointerUnmanagedCallingConvention(name);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    functionPointerUnmanagedCallingConventionSyntax = functionPointerUnmanagedCallingConventionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    functionPointerUnmanagedCallingConventionSyntax = functionPointerUnmanagedCallingConventionSyntax.WithAnnotationsGreen(annotations);
                }
                return functionPointerUnmanagedCallingConventionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new FunctionPointerUnmanagedCallingConventionSyntax(base.Kind, name, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new FunctionPointerUnmanagedCallingConventionSyntax(base.Kind, name, GetDiagnostics(), annotations);
        }

        public FunctionPointerUnmanagedCallingConventionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            name = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(name);
        }

        static FunctionPointerUnmanagedCallingConventionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(FunctionPointerUnmanagedCallingConventionSyntax), (ObjectReader r) => new FunctionPointerUnmanagedCallingConventionSyntax(r));
        }
    }
}

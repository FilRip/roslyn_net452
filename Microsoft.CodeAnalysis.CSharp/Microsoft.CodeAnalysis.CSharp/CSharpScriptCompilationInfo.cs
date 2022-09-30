using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class CSharpScriptCompilationInfo : ScriptCompilationInfo
    {
        public new CSharpCompilation? PreviousScriptCompilation { get; }

        public override Compilation? CommonPreviousScriptCompilation => PreviousScriptCompilation;

        public CSharpScriptCompilationInfo(CSharpCompilation? previousCompilationOpt, Type? returnType, Type? globalsType)
            : base(returnType, globalsType)
        {
            PreviousScriptCompilation = previousCompilationOpt;
        }

        public CSharpScriptCompilationInfo WithPreviousScriptCompilation(CSharpCompilation? compilation)
        {
            if (compilation != PreviousScriptCompilation)
            {
                return new CSharpScriptCompilationInfo(compilation, base.ReturnTypeOpt, base.GlobalsType);
            }
            return this;
        }

        public override ScriptCompilationInfo CommonWithPreviousScriptCompilation(Compilation? compilation)
        {
            return WithPreviousScriptCompilation((CSharpCompilation)compilation);
        }
    }
}

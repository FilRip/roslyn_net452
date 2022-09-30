using System;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class ScriptCompilationInfo
    {
        public Type? ReturnTypeOpt { get; }

        public Type ReturnType => ReturnTypeOpt ?? typeof(object);

        public Type? GlobalsType { get; }

        public Compilation? PreviousScriptCompilation => CommonPreviousScriptCompilation;

        public abstract Compilation? CommonPreviousScriptCompilation { get; }

        public ScriptCompilationInfo(Type? returnType, Type? globalsType)
        {
            ReturnTypeOpt = returnType;
            GlobalsType = globalsType;
        }

        public ScriptCompilationInfo WithPreviousScriptCompilation(Compilation? compilation)
        {
            return CommonWithPreviousScriptCompilation(compilation);
        }

        public abstract ScriptCompilationInfo CommonWithPreviousScriptCompilation(Compilation? compilation);
    }
}

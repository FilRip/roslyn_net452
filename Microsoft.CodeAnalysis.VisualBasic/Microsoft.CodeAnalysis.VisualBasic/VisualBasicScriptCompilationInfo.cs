using System;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class VisualBasicScriptCompilationInfo : ScriptCompilationInfo
	{
		public new VisualBasicCompilation PreviousScriptCompilation { get; }

		internal override Compilation CommonPreviousScriptCompilation => PreviousScriptCompilation;

		internal VisualBasicScriptCompilationInfo(VisualBasicCompilation previousCompilationOpt, Type returnType, Type globalsType)
			: base(returnType, globalsType)
		{
			PreviousScriptCompilation = previousCompilationOpt;
		}

		public VisualBasicScriptCompilationInfo WithPreviousScriptCompilation(VisualBasicCompilation compilation)
		{
			if (compilation != PreviousScriptCompilation)
			{
				return new VisualBasicScriptCompilationInfo(compilation, base.ReturnTypeOpt, base.GlobalsType);
			}
			return this;
		}

		internal override ScriptCompilationInfo CommonWithPreviousScriptCompilation(Compilation compilation)
		{
			return WithPreviousScriptCompilation((VisualBasicCompilation)compilation);
		}
	}
}

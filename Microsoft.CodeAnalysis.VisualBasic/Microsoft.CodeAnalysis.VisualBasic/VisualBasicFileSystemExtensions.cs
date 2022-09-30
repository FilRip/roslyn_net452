using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class VisualBasicFileSystemExtensions
	{
		public static EmitResult Emit(this VisualBasicCompilation compilation, string outputPath, string pdbPath = null, string xmlDocPath = null, string win32ResourcesPath = null, IEnumerable<ResourceDescription> manifestResources = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return compilation.Emit(outputPath, pdbPath, xmlDocPath, win32ResourcesPath, manifestResources, cancellationToken);
		}
	}
}

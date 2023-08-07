// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Scripting.Hosting
{
    internal static class CommandLineHelpers
    {
        // TODO (https://github.com/dotnet/roslyn/issues/5854): remove 
        public static ImmutableArray<string> GetImports(CommandLineArguments args)
        {
            return args.CompilationOptions.GetImports();
        }

        internal static ScriptOptions RemoveImportsAndReferences(this ScriptOptions options)
        {
#pragma warning disable S3878 // Arrays should not be created for params parameters
            return options.WithReferences(new MetadataReference[0] { }).WithImports(new string[0] { });
#pragma warning restore S3878 // Arrays should not be created for params parameters
        }
    }
}

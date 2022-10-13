// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeAnalysis.CommandLine.ExitingTraceListener
// Assembly: vbc, Version=3.11.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59BA59CE-D1C9-469A-AF98-699E22DB28ED
// Assembly location: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\vbc.exe

using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class ExitingTraceListener : TraceListener
    {
        internal ICompilerServerLogger Logger { get; }

        internal ExitingTraceListener(ICompilerServerLogger logger) => this.Logger = logger;

        public override void Write(string message) => this.Exit(message);

        public override void WriteLine(string message) => this.Exit(message);

        internal static void Install(ICompilerServerLogger logger)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new ExitingTraceListener(logger));
        }

        private void Exit(string originalMessage)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Debug.Assert failed with message: " + originalMessage);
            stringBuilder.AppendLine("Stack Trace");
            stringBuilder.AppendLine(new StackTrace().ToString());
            string message = stringBuilder.ToString();
            this.Logger.Log(message);
            Environment.FailFast(message);
        }
    }
}

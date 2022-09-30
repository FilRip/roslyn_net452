using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal sealed class AnalyzerInconsistencyBuildResponse : BuildResponse
	{
		public override ResponseType Type => ResponseType.AnalyzerInconsistency;

		public ReadOnlyCollection<string> ErrorMessages { get; }

		public AnalyzerInconsistencyBuildResponse(ReadOnlyCollection<string> errorMessages)
		{
			ErrorMessages = errorMessages;
		}

		protected override void AddResponseBody(BinaryWriter writer)
		{
			writer.Write(ErrorMessages.Count);
			foreach (string errorMessage in ErrorMessages)
			{
				BuildProtocolConstants.WriteLengthPrefixedString(writer, errorMessage);
			}
		}

		public static AnalyzerInconsistencyBuildResponse Create(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			List<string> list = new List<string>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(BuildProtocolConstants.ReadLengthPrefixedString(reader) ?? "");
			}
			return new AnalyzerInconsistencyBuildResponse(new ReadOnlyCollection<string>(list));
		}
	}
}

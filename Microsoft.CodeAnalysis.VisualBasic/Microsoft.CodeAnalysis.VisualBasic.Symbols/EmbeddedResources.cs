using System.IO;
using System.Reflection;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class EmbeddedResources
	{
		private static string s_embedded;

		private static string s_internalXmlHelper;

		private static string s_vbCoreSourceText;

		private static string s_vbMyTemplateText;

		public static string Embedded
		{
			get
			{
				if (s_embedded == null)
				{
					s_embedded = GetManifestResourceString("Embedded.vb");
				}
				return s_embedded;
			}
		}

		public static string InternalXmlHelper
		{
			get
			{
				if (s_internalXmlHelper == null)
				{
					s_internalXmlHelper = GetManifestResourceString("InternalXmlHelper.vb");
				}
				return s_internalXmlHelper;
			}
		}

		public static string VbCoreSourceText
		{
			get
			{
				if (s_vbCoreSourceText == null)
				{
					s_vbCoreSourceText = GetManifestResourceString("VbCoreSourceText.vb");
				}
				return s_vbCoreSourceText;
			}
		}

		public static string VbMyTemplateText
		{
			get
			{
				if (s_vbMyTemplateText == null)
				{
					s_vbMyTemplateText = GetManifestResourceString("VbMyTemplateText.vb");
				}
				return s_vbMyTemplateText;
			}
		}

		private static string GetManifestResourceString(string name)
		{
			using StreamReader streamReader = new StreamReader(typeof(EmbeddedResources).GetTypeInfo().Assembly.GetManifestResourceStream(name));
			return streamReader.ReadToEnd();
		}
	}
}

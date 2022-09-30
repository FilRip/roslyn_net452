using System.Reflection;

namespace System.Security.Cryptography
{
	internal static class CryptoConfigForwarder
	{
		private static readonly Func<string, object> s_createFromName = BindCreateFromName();

		private static Func<string, object> BindCreateFromName()
		{
			Type type = Type.GetType("System.Security.Cryptography.CryptoConfig, System.Security.Cryptography.Algorithms", throwOnError: true);
			MethodInfo method = type.GetMethod("CreateFromName", new Type[1] { typeof(string) });
			if (method == null)
			{
				throw new MissingMethodException(type.FullName, "CreateFromName");
			}
			return (Func<string, object>)method.CreateDelegate(typeof(Func<string, object>));
		}

		internal static object CreateFromName(string name)
		{
			return s_createFromName(name);
		}
	}
}

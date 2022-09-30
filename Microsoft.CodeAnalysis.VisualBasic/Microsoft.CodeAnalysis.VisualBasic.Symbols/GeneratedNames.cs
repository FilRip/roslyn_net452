using System;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class GeneratedNames
	{
		internal const char DotReplacementInTypeNames = '-';

		private const char s_methodNameSeparator = '_';

		private const char s_idSeparator = '-';

		private const char s_generationSeparator = '#';

		internal const string AnonymousTypeOrDelegateCommonPrefix = "VB$Anonymous";

		internal const string AnonymousTypeTemplateNamePrefix = "VB$AnonymousType_";

		internal const string AnonymousDelegateTemplateNamePrefix = "VB$AnonymousDelegate_";

		internal static GeneratedNameKind GetKind(string name)
		{
			if (name.StartsWith("$VB$Me", StringComparison.Ordinal))
			{
				return GeneratedNameKind.HoistedMeField;
			}
			if (name.StartsWith("$State", StringComparison.Ordinal))
			{
				return GeneratedNameKind.StateMachineStateField;
			}
			if (name.StartsWith("$STATIC$", StringComparison.Ordinal))
			{
				return GeneratedNameKind.StaticLocalField;
			}
			if (name.StartsWith("$S", StringComparison.Ordinal))
			{
				return GeneratedNameKind.HoistedSynthesizedLocalField;
			}
			if (name.StartsWith("$VB$Local_", StringComparison.Ordinal))
			{
				return GeneratedNameKind.HoistedUserVariableField;
			}
			if (name.StartsWith("$Current", StringComparison.Ordinal))
			{
				return GeneratedNameKind.IteratorCurrentField;
			}
			if (name.StartsWith("$InitialThreadId", StringComparison.Ordinal))
			{
				return GeneratedNameKind.IteratorInitialThreadIdField;
			}
			if (name.StartsWith("$P_", StringComparison.Ordinal))
			{
				return GeneratedNameKind.IteratorParameterProxyField;
			}
			if (name.StartsWith("$A", StringComparison.Ordinal))
			{
				return GeneratedNameKind.StateMachineAwaiterField;
			}
			if (name.StartsWith("$VB$ResumableLocal_", StringComparison.Ordinal))
			{
				return GeneratedNameKind.StateMachineHoistedUserVariableField;
			}
			if (name.StartsWith("VB$AnonymousType_", StringComparison.Ordinal))
			{
				return GeneratedNameKind.AnonymousType;
			}
			if (name.StartsWith("_Closure$__", StringComparison.Ordinal))
			{
				return GeneratedNameKind.LambdaDisplayClass;
			}
			if (name.Equals("$VB$It", StringComparison.Ordinal) || name.Equals("$VB$It1", StringComparison.Ordinal) || name.Equals("$VB$It2", StringComparison.Ordinal))
			{
				return GeneratedNameKind.TransparentIdentifier;
			}
			if (name.Equals("$VB$ItAnonymous", StringComparison.Ordinal))
			{
				return GeneratedNameKind.AnonymousTransparentIdentifier;
			}
			return GeneratedNameKind.None;
		}

		public static string MakeStateMachineTypeName(string methodName, int methodOrdinal, int generation)
		{
			return MakeMethodScopedSynthesizedName("VB$StateMachine_", methodOrdinal, generation, methodName, -1, -1, isTypeName: true);
		}

		public static string MakeStateMachineStateFieldName()
		{
			return "$State";
		}

		public static string MakeBaseMethodWrapperName(string methodName, bool isMyBase)
		{
			return "$VB$ClosureStub_" + methodName + (isMyBase ? "_MyBase" : "_MyClass");
		}

		public static string ReusableHoistedLocalFieldName(int number)
		{
			return "$U" + StringExtensions.GetNumeral(number);
		}

		public static string MakeStaticLambdaDisplayClassName(int methodOrdinal, int generation)
		{
			return MakeMethodScopedSynthesizedName("_Closure$__", methodOrdinal, generation);
		}

		internal static string MakeLambdaDisplayClassName(int methodOrdinal, int generation, int closureOrdinal, int closureGeneration, bool isDelegateRelaxation)
		{
			return MakeMethodScopedSynthesizedName(isDelegateRelaxation ? "_Closure$__R" : "_Closure$__", methodOrdinal, generation, null, closureOrdinal, closureGeneration, isTypeName: true);
		}

		internal static string MakeDisplayClassGenericParameterName(int parameterIndex)
		{
			return "$CLS" + StringExtensions.GetNumeral(parameterIndex);
		}

		internal static string MakeLambdaMethodName(int methodOrdinal, int generation, int lambdaOrdinal, int lambdaGeneration, SynthesizedLambdaKind lambdaKind)
		{
			return MakeMethodScopedSynthesizedName((lambdaKind == SynthesizedLambdaKind.DelegateRelaxationStub) ? "_Lambda$__R" : "_Lambda$__", methodOrdinal, generation, null, lambdaOrdinal, lambdaGeneration);
		}

		public static string MakeCachedFrameInstanceName()
		{
			return "$I";
		}

		internal static string MakeLambdaCacheFieldName(int methodOrdinal, int generation, int lambdaOrdinal, int lambdaGeneration, SynthesizedLambdaKind lambdaKind)
		{
			return MakeMethodScopedSynthesizedName((lambdaKind == SynthesizedLambdaKind.DelegateRelaxationStub) ? "$IR" : "$I", methodOrdinal, generation, null, lambdaOrdinal, lambdaGeneration);
		}

		internal static string MakeDelegateRelaxationParameterName(int parameterIndex)
		{
			return "a" + StringExtensions.GetNumeral(parameterIndex);
		}

		private static string MakeMethodScopedSynthesizedName(string prefix, int methodOrdinal, int methodGeneration, string methodNameOpt = null, int entityOrdinal = -1, int entityGeneration = -1, bool isTypeName = false)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			StringBuilder builder = instance.Builder;
			builder.Append(prefix);
			if (methodOrdinal >= 0)
			{
				builder.Append(methodOrdinal);
				if (methodGeneration > 0)
				{
					builder.Append('#');
					builder.Append(methodGeneration);
				}
			}
			if (entityOrdinal >= 0)
			{
				if (methodOrdinal >= 0)
				{
					builder.Append('-');
				}
				builder.Append(entityOrdinal);
				if (entityGeneration > 0)
				{
					builder.Append('#');
					builder.Append(entityGeneration);
				}
			}
			if (methodNameOpt != null)
			{
				builder.Append('_');
				builder.Append(methodNameOpt);
				if (isTypeName)
				{
					builder.Replace('.', '-');
				}
			}
			return instance.ToStringAndFree();
		}

		public static bool TryParseStateMachineTypeName(string stateMachineTypeName, out string methodName)
		{
			if (!stateMachineTypeName.StartsWith("VB$StateMachine_", StringComparison.Ordinal))
			{
				return false;
			}
			int length = "VB$StateMachine_".Length;
			int num = stateMachineTypeName.IndexOf('_', length);
			if (num < 0 || num == stateMachineTypeName.Length - 1)
			{
				return false;
			}
			methodName = stateMachineTypeName.Substring(num + 1);
			return true;
		}

		public static string MakeStateMachineBuilderFieldName()
		{
			return "$Builder";
		}

		public static string MakeIteratorCurrentFieldName()
		{
			return "$Current";
		}

		public static string MakeStateMachineAwaiterFieldName(int index)
		{
			return "$A" + StringExtensions.GetNumeral(index);
		}

		public static string MakeStateMachineParameterName(string paramName)
		{
			return "$VB$Local_" + paramName;
		}

		public static string MakeIteratorParameterProxyName(string paramName)
		{
			return "$P_" + paramName;
		}

		public static string MakeIteratorInitialThreadIdName()
		{
			return "$InitialThreadId";
		}

		public static bool TryParseHoistedUserVariableName(string proxyName, out string variableName)
		{
			variableName = null;
			int length = "$VB$Local_".Length;
			if (proxyName.Length <= length)
			{
				return false;
			}
			if (!proxyName.StartsWith("$VB$Local_", StringComparison.Ordinal))
			{
				return false;
			}
			variableName = proxyName.Substring(length);
			return true;
		}

		public static bool TryParseStateMachineHoistedUserVariableName(string proxyName, out string variableName, out int index)
		{
			variableName = null;
			index = 0;
			if (!proxyName.StartsWith("$VB$ResumableLocal_", StringComparison.Ordinal))
			{
				return false;
			}
			int length = "$VB$ResumableLocal_".Length;
			int num = proxyName.LastIndexOf('$');
			if (num <= length)
			{
				return false;
			}
			variableName = proxyName.Substring(length, num - length);
			return int.TryParse(proxyName.Substring(num + 1), NumberStyles.None, CultureInfo.InvariantCulture, out index);
		}

		public static string MakeStateMachineCapturedMeName()
		{
			return "$VB$Me";
		}

		public static string MakeStateMachineCapturedClosureMeName(string closureName)
		{
			return "$VB$NonLocal_" + closureName;
		}

		internal static string MakeAnonymousTypeTemplateName(string prefix, int index, int submissionSlotIndex, string moduleId)
		{
			if (submissionSlotIndex < 0)
			{
				return $"{prefix}{index}{moduleId}";
			}
			return $"{prefix}{submissionSlotIndex}_{index}{moduleId}";
		}

		internal static bool TryParseAnonymousTypeTemplateName(string prefix, string name, out int index)
		{
			if (name.StartsWith(prefix, StringComparison.Ordinal) && int.TryParse(name.Substring(prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out index))
			{
				return true;
			}
			index = -1;
			return false;
		}

		internal static string MakeSynthesizedLocalName(SynthesizedLocalKind kind, ref int uniqueId)
		{
			string result;
			switch (kind)
			{
			case SynthesizedLocalKind.LambdaDisplayClass:
				result = MakeLambdaDisplayClassStorageName(uniqueId);
				uniqueId++;
				break;
			case SynthesizedLocalKind.With:
				result = "$W" + StringExtensions.GetNumeral(uniqueId);
				uniqueId++;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		internal static string MakeLambdaDisplayClassStorageName(int uniqueId)
		{
			return "$VB$Closure_" + StringExtensions.GetNumeral(uniqueId);
		}

		internal static string MakeSignatureString(byte[] signature)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			foreach (byte b in signature)
			{
				instance.Builder.AppendFormat("{0:X}", b);
			}
			return instance.ToStringAndFree();
		}

		internal static string MakeStaticLocalFieldName(string methodName, string methodSignature, string localName)
		{
			return $"$STATIC${methodName}${methodSignature}${localName}";
		}

		internal static bool TryParseStaticLocalFieldName(string fieldName, out string methodName, out string methodSignature, out string localName)
		{
			if (fieldName.StartsWith("$STATIC$", StringComparison.Ordinal))
			{
				string[] array = fieldName.Split(new char[1] { '$' });
				if (array.Length == 5)
				{
					methodName = array[2];
					methodSignature = array[3];
					localName = array[4];
					return true;
				}
			}
			methodName = null;
			methodSignature = null;
			localName = null;
			return false;
		}

		internal static bool TryParseSlotIndex(string prefix, string fieldName, out int slotIndex)
		{
			if (fieldName.StartsWith(prefix, StringComparison.Ordinal) && int.TryParse(fieldName.Substring(prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out slotIndex))
			{
				return true;
			}
			slotIndex = -1;
			return false;
		}
	}
}

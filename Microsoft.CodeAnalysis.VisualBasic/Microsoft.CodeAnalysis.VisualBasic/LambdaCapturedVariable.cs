using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class LambdaCapturedVariable : SynthesizedFieldSymbol
	{
		private readonly bool _isMe;

		public override bool IsShared => IsConst;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override bool IsConst => false;

		internal override bool IsCapturedFrame => _isMe;

		internal LambdaCapturedVariable(LambdaFrame frame, Symbol captured, TypeSymbol type, string fieldName, bool isMeParameter)
			: base(frame, captured, type, fieldName, Accessibility.Public)
		{
			_isMe = isMeParameter;
		}

		public static LambdaCapturedVariable Create(LambdaFrame frame, Symbol captured, ref int uniqueId)
		{
			string capturedVariableFieldName = GetCapturedVariableFieldName(captured, ref uniqueId);
			TypeSymbol capturedVariableFieldType = GetCapturedVariableFieldType(frame, captured);
			return new LambdaCapturedVariable(frame, captured, capturedVariableFieldType, capturedVariableFieldName, IsMe(captured));
		}

		public static string GetCapturedVariableFieldName(Symbol captured, ref int uniqueId)
		{
			if (captured is LocalSymbol localSymbol && localSymbol.IsCompilerGenerated)
			{
				switch (localSymbol.SynthesizedKind)
				{
				case SynthesizedLocalKind.LambdaDisplayClass:
					uniqueId++;
					return "$VB$NonLocal_$VB$Closure_" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(uniqueId);
				case SynthesizedLocalKind.With:
					uniqueId++;
					return "$W" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(uniqueId);
				default:
					uniqueId++;
					return "$VB$NonLocal_" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(uniqueId);
				}
			}
			if (captured is ParameterSymbol parameterSymbol && parameterSymbol.IsMe)
			{
				return "$VB$Me";
			}
			return "$VB$Local_" + captured.Name;
		}

		public static TypeSymbol GetCapturedVariableFieldType(LambdaFrame frame, Symbol captured)
		{
			if (captured is LocalSymbol localSymbol)
			{
				if (localSymbol.Type.OriginalDefinition is LambdaFrame type)
				{
					return LambdaRewriter.ConstructFrameType(type, frame.TypeArgumentsNoUseSiteDiagnostics);
				}
				return localSymbol.Type.InternalSubstituteTypeParameters(frame.TypeMap).Type;
			}
			return ((ParameterSymbol)captured).Type.InternalSubstituteTypeParameters(frame.TypeMap).Type;
		}

		public static bool IsMe(Symbol captured)
		{
			if (captured is ParameterSymbol parameterSymbol)
			{
				return parameterSymbol.IsMe;
			}
			return false;
		}
	}
}

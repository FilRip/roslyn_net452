using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedStringSwitchHashMethod : SynthesizedGlobalMethodBase
	{
		private readonly ImmutableArray<ParameterSymbol> _parameters;

		private readonly TypeSymbol _returnType;

		internal override int ParameterCount => 1;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public override bool IsSub => false;

		public override bool ReturnsByRef => false;

		public override TypeSymbol ReturnType => _returnType;

		internal static uint ComputeStringHash(string text)
		{
			uint num = 2166136261u;
			if (EmbeddedOperators.CompareString(text, null, TextCompare: false) != 0)
			{
				for (int i = 0; i < text.Length; i++)
				{
					num = (uint)(((int)text[i] ^ num) * 16777619);
				}
			}
			return num;
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
		{
			SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, this, base.Syntax, compilationState, diagnostics);
			syntheticBoundNodeFactory.CurrentMethod = this;
			LocalSymbol localSymbol = syntheticBoundNodeFactory.SynthesizedLocal(ContainingAssembly.GetSpecialType(SpecialType.System_Int32));
			LocalSymbol localSymbol2 = syntheticBoundNodeFactory.SynthesizedLocal(ContainingAssembly.GetSpecialType(SpecialType.System_UInt32));
			LabelSymbol labelSymbol = syntheticBoundNodeFactory.GenerateLabel("again");
			LabelSymbol labelSymbol2 = syntheticBoundNodeFactory.GenerateLabel("start");
			ParameterSymbol parameterSymbol = Parameters[0];
			BoundExpression arg = syntheticBoundNodeFactory.Call(syntheticBoundNodeFactory.Parameter(parameterSymbol), (MethodSymbol)ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_String__Chars), syntheticBoundNodeFactory.Local(localSymbol, isLValue: false));
			arg = syntheticBoundNodeFactory.Convert(localSymbol.Type, arg, ConversionKind.WideningNumeric);
			arg = syntheticBoundNodeFactory.Convert(localSymbol2.Type, arg, ConversionKind.WideningNumeric);
			return syntheticBoundNodeFactory.Block(ImmutableArray.Create(localSymbol2, localSymbol), syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.Local(localSymbol2, isLValue: true), new BoundLiteral(base.Syntax, ConstantValue.Create(2166136261u), localSymbol2.Type)), syntheticBoundNodeFactory.If(syntheticBoundNodeFactory.Binary(BinaryOperatorKind.IsNot, ContainingAssembly.GetSpecialType(SpecialType.System_Boolean), syntheticBoundNodeFactory.Parameter(parameterSymbol).MakeRValue(), syntheticBoundNodeFactory.Null(parameterSymbol.Type)), syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.Local(localSymbol, isLValue: true), new BoundLiteral(base.Syntax, ConstantValue.Create(0), localSymbol.Type)), syntheticBoundNodeFactory.Goto(labelSymbol2), syntheticBoundNodeFactory.Label(labelSymbol), syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.Local(localSymbol2, isLValue: true), syntheticBoundNodeFactory.Binary(BinaryOperatorKind.Multiply, localSymbol2.Type, syntheticBoundNodeFactory.Binary(BinaryOperatorKind.Xor, localSymbol2.Type, arg, syntheticBoundNodeFactory.Local(localSymbol2, isLValue: false)), new BoundLiteral(base.Syntax, ConstantValue.Create(16777619u), localSymbol2.Type))), syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.Local(localSymbol, isLValue: true), syntheticBoundNodeFactory.Binary(BinaryOperatorKind.Add, localSymbol.Type, syntheticBoundNodeFactory.Local(localSymbol, isLValue: false), new BoundLiteral(base.Syntax, ConstantValue.Create(1), localSymbol.Type))), syntheticBoundNodeFactory.Label(labelSymbol2), syntheticBoundNodeFactory.If(syntheticBoundNodeFactory.Binary(BinaryOperatorKind.LessThan, ContainingAssembly.GetSpecialType(SpecialType.System_Boolean), syntheticBoundNodeFactory.Local(localSymbol, isLValue: false), syntheticBoundNodeFactory.Call(syntheticBoundNodeFactory.Parameter(parameterSymbol).MakeRValue(), (MethodSymbol)ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_String__Length))), syntheticBoundNodeFactory.Goto(labelSymbol)))), syntheticBoundNodeFactory.Return(syntheticBoundNodeFactory.Local(localSymbol2, isLValue: false)));
		}

		public SynthesizedStringSwitchHashMethod(SourceModuleSymbol container, PrivateImplementationDetails privateImplType)
			: base(container, "ComputeStringHash", privateImplType)
		{
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			_parameters = ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSimpleSymbol(this, declaringCompilation.GetSpecialType(SpecialType.System_String), 0, "s"));
			_returnType = declaringCompilation.GetSpecialType(SpecialType.System_UInt32);
		}
	}
}

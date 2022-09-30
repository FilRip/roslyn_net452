using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class ParameterTypeInformation : IParameterTypeInformation
	{
		private readonly ParameterSymbol _underlyingParameter;

		private ImmutableArray<ICustomModifier> IParameterTypeInformationCustomModifiers => _underlyingParameter.CustomModifiers.As<ICustomModifier>();

		private bool IParameterTypeInformationIsByReference => _underlyingParameter.IsByRef;

		private ImmutableArray<ICustomModifier> IParameterTypeInformationRefCustomModifiers => _underlyingParameter.RefCustomModifiers.As<ICustomModifier>();

		private ushort IParameterListEntryIndex => (ushort)_underlyingParameter.Ordinal;

		public ParameterTypeInformation(ParameterSymbol underlyingParameter)
		{
			_underlyingParameter = underlyingParameter;
		}

		private ITypeReference IParameterTypeInformationGetType(EmitContext context)
		{
			PEModuleBuilder obj = (PEModuleBuilder)context.Module;
			TypeSymbol type = _underlyingParameter.Type;
			return obj.Translate(type, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		ITypeReference IParameterTypeInformation.GetType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IParameterTypeInformationGetType
			return this.IParameterTypeInformationGetType(context);
		}

		public override bool Equals(object obj)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override int GetHashCode()
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}

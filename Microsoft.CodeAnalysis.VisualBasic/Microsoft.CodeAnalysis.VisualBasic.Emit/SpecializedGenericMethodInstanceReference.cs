using System.Collections.Generic;
using System.Linq;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class SpecializedGenericMethodInstanceReference : SpecializedMethodReference, IGenericMethodInstanceReference
	{
		private readonly SpecializedMethodReference _genericMethod;

		public override IGenericMethodInstanceReference AsGenericMethodInstanceReference => this;

		public SpecializedGenericMethodInstanceReference(MethodSymbol underlyingMethod)
			: base(underlyingMethod)
		{
			_genericMethod = new SpecializedMethodReference(underlyingMethod);
		}

		public IMethodReference GetGenericMethod(EmitContext context)
		{
			return _genericMethod;
		}

		IMethodReference IGenericMethodInstanceReference.GetGenericMethod(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetGenericMethod
			return this.GetGenericMethod(context);
		}

		public IEnumerable<ITypeReference> GetGenericArguments(EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			return m_UnderlyingMethod.TypeArguments.Select((TypeSymbol arg) => pEModuleBuilder.Translate(arg, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics));
		}

		IEnumerable<ITypeReference> IGenericMethodInstanceReference.GetGenericArguments(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetGenericArguments
			return this.GetGenericArguments(context);
		}

		public override void Dispatch(MetadataVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}

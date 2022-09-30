using System.Collections.Generic;
using System.Linq;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class GenericMethodInstanceReference : MethodReference, IGenericMethodInstanceReference
	{
		public override IGenericMethodInstanceReference AsGenericMethodInstanceReference => this;

		public GenericMethodInstanceReference(MethodSymbol underlyingMethod)
			: base(underlyingMethod)
		{
		}

		public override void Dispatch(MetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		private IEnumerable<ITypeReference> IGenericMethodInstanceReferenceGetGenericArguments(EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			return m_UnderlyingMethod.TypeArguments.Select((TypeSymbol arg) => pEModuleBuilder.Translate(arg, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics));
		}

		IEnumerable<ITypeReference> IGenericMethodInstanceReference.GetGenericArguments(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IGenericMethodInstanceReferenceGetGenericArguments
			return this.IGenericMethodInstanceReferenceGetGenericArguments(context);
		}

		private IMethodReference IGenericMethodInstanceReferenceGetGenericMethod(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(m_UnderlyingMethod.OriginalDefinition, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics, needDeclaration: true);
		}

		IMethodReference IGenericMethodInstanceReference.GetGenericMethod(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IGenericMethodInstanceReferenceGetGenericMethod
			return this.IGenericMethodInstanceReferenceGetGenericMethod(context);
		}
	}
}

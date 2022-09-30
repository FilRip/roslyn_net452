using Microsoft.Cci;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal class SpecializedMethodReference : MethodReference, ISpecializedMethodReference
	{
		private IMethodReference ISpecializedMethodReferenceUnspecializedVersion => m_UnderlyingMethod.OriginalDefinition.GetCciAdapter();

		public override ISpecializedMethodReference AsSpecializedMethodReference => this;

		public SpecializedMethodReference(MethodSymbol underlyingMethod)
			: base(underlyingMethod)
		{
		}

		public override void Dispatch(MetadataVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}

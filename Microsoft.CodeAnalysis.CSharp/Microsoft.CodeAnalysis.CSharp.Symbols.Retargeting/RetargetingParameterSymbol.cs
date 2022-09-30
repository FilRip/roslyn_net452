using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Emit;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Retargeting
{
    internal abstract class RetargetingParameterSymbol : WrappedParameterSymbol
    {
        private ImmutableArray<CustomModifier> _lazyRefCustomModifiers;

        private ImmutableArray<CSharpAttributeData> _lazyCustomAttributes;

        protected abstract RetargetingModuleSymbol RetargetingModule { get; }

        public sealed override TypeWithAnnotations TypeWithAnnotations => RetargetingModule.RetargetingTranslator.Retarget(_underlyingParameter.TypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode);

        public sealed override ImmutableArray<CustomModifier> RefCustomModifiers => RetargetingModule.RetargetingTranslator.RetargetModifiers(_underlyingParameter.RefCustomModifiers, ref _lazyRefCustomModifiers);

        public sealed override Symbol ContainingSymbol => RetargetingModule.RetargetingTranslator.Retarget(_underlyingParameter.ContainingSymbol);

        public sealed override AssemblySymbol ContainingAssembly => RetargetingModule.ContainingAssembly;

        internal sealed override ModuleSymbol ContainingModule => RetargetingModule;

        internal sealed override bool HasMetadataConstantValue => _underlyingParameter.HasMetadataConstantValue;

        internal sealed override bool IsMarshalledExplicitly => _underlyingParameter.IsMarshalledExplicitly;

        internal override MarshalPseudoCustomAttributeData MarshallingInformation => RetargetingModule.RetargetingTranslator.Retarget(_underlyingParameter.MarshallingInformation);

        internal override ImmutableArray<byte> MarshallingDescriptor => _underlyingParameter.MarshallingDescriptor;

        internal sealed override CSharpCompilation DeclaringCompilation => null;

        protected RetargetingParameterSymbol(ParameterSymbol underlyingParameter)
            : base(underlyingParameter)
        {
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return RetargetingModule.RetargetingTranslator.GetRetargetedAttributes(_underlyingParameter.GetAttributes(), ref _lazyCustomAttributes);
        }

        internal sealed override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            return RetargetingModule.RetargetingTranslator.RetargetAttributes(_underlyingParameter.GetCustomAttributesToEmit(moduleBuilder));
        }
    }
}

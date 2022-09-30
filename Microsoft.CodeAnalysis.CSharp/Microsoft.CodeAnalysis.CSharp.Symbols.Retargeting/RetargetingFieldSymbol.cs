using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Emit;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Retargeting
{
    internal sealed class RetargetingFieldSymbol : WrappedFieldSymbol
    {
        private readonly RetargetingModuleSymbol _retargetingModule;

        private ImmutableArray<CSharpAttributeData> _lazyCustomAttributes;

        private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;

        private RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => _retargetingModule.RetargetingTranslator;

        public RetargetingModuleSymbol RetargetingModule => _retargetingModule;

        public override Symbol ContainingSymbol => RetargetingTranslator.Retarget(_underlyingField.ContainingSymbol);

        public override AssemblySymbol ContainingAssembly => _retargetingModule.ContainingAssembly;

        internal override ModuleSymbol ContainingModule => _retargetingModule;

        internal override MarshalPseudoCustomAttributeData MarshallingInformation => RetargetingTranslator.Retarget(_underlyingField.MarshallingInformation);

        public override Symbol AssociatedSymbol
        {
            get
            {
                Symbol associatedSymbol = _underlyingField.AssociatedSymbol;
                if ((object)associatedSymbol != null)
                {
                    return RetargetingTranslator.Retarget(associatedSymbol);
                }
                return null;
            }
        }

        public override int TupleElementIndex => _underlyingField.TupleElementIndex;

        internal sealed override CSharpCompilation DeclaringCompilation => null;

        public RetargetingFieldSymbol(RetargetingModuleSymbol retargetingModule, FieldSymbol underlyingField)
            : base(underlyingField)
        {
            _retargetingModule = retargetingModule;
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return RetargetingTranslator.Retarget(_underlyingField.GetFieldType(fieldsBeingBound), RetargetOptions.RetargetPrimitiveTypesByTypeCode);
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return RetargetingTranslator.GetRetargetedAttributes(_underlyingField.GetAttributes(), ref _lazyCustomAttributes);
        }

        internal override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            return RetargetingTranslator.RetargetAttributes(_underlyingField.GetCustomAttributesToEmit(moduleBuilder));
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            if (!_lazyCachedUseSiteInfo.IsInitialized)
            {
                AssemblySymbol primaryDependency = base.PrimaryDependency;
                UseSiteInfo<AssemblySymbol> result = new UseSiteInfo<AssemblySymbol>(primaryDependency);
                CalculateUseSiteDiagnostic(ref result);
                _lazyCachedUseSiteInfo.Initialize(primaryDependency, result);
            }
            return _lazyCachedUseSiteInfo.ToUseSiteInfo(base.PrimaryDependency);
        }
    }
}

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class LazyMissingNonNullTypesContextDiagnosticInfo : LazyDiagnosticInfo
    {
        private readonly TypeWithAnnotations _type;

        private readonly DiagnosticInfo _info;

        private LazyMissingNonNullTypesContextDiagnosticInfo(TypeWithAnnotations type, DiagnosticInfo info)
        {
            _type = type;
            _info = info;
        }

        public static void AddAll(bool isNullableEnabled, bool isGeneratedCode, TypeWithAnnotations type, Location location, DiagnosticBag diagnostics)
        {
            ArrayBuilder<DiagnosticInfo> instance = ArrayBuilder<DiagnosticInfo>.GetInstance();
            GetRawDiagnosticInfos(isNullableEnabled, isGeneratedCode, (CSharpSyntaxTree)location.SourceTree, instance);
            ArrayBuilder<DiagnosticInfo>.Enumerator enumerator = instance.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticInfo current = enumerator.Current;
                diagnostics.Add(new LazyMissingNonNullTypesContextDiagnosticInfo(type, current), location);
            }
            instance.Free();
        }

        private static void GetRawDiagnosticInfos(bool isNullableEnabled, bool isGeneratedCode, CSharpSyntaxTree tree, ArrayBuilder<DiagnosticInfo> infos)
        {
            CSDiagnosticInfo featureAvailabilityDiagnosticInfo = MessageID.IDS_FeatureNullableReferenceTypes.GetFeatureAvailabilityDiagnosticInfo(tree.Options);
            if (featureAvailabilityDiagnosticInfo != null)
            {
                infos.Add(featureAvailabilityDiagnosticInfo);
            }
            if (!isNullableEnabled && (featureAvailabilityDiagnosticInfo == null || featureAvailabilityDiagnosticInfo.Severity != DiagnosticSeverity.Error))
            {
                ErrorCode code = (isGeneratedCode ? ErrorCode.WRN_MissingNonNullTypesContextForAnnotationInGeneratedCode : ErrorCode.WRN_MissingNonNullTypesContextForAnnotation);
                infos.Add(new CSDiagnosticInfo(code));
            }
        }

        private static bool IsNullableReference(TypeSymbol type)
        {
            if ((object)type != null)
            {
                if (!type.IsValueType)
                {
                    return !type.IsErrorType();
                }
                return false;
            }
            return true;
        }

        protected override DiagnosticInfo ResolveInfo()
        {
            if (!IsNullableReference(_type.Type))
            {
                return null;
            }
            return _info;
        }

        public static void ReportNullableReferenceTypesIfNeeded(bool isNullableEnabled, bool isGeneratedCode, TypeWithAnnotations type, Location location, DiagnosticBag diagnostics)
        {
            if (IsNullableReference(type.Type))
            {
                ReportNullableReferenceTypesIfNeeded(isNullableEnabled, isGeneratedCode, location, diagnostics);
            }
        }

        public static void ReportNullableReferenceTypesIfNeeded(bool isNullableEnabled, bool isGeneratedCode, Location location, DiagnosticBag diagnostics)
        {
            ArrayBuilder<DiagnosticInfo> instance = ArrayBuilder<DiagnosticInfo>.GetInstance();
            GetRawDiagnosticInfos(isNullableEnabled, isGeneratedCode, (CSharpSyntaxTree)location.SourceTree, instance);
            ArrayBuilder<DiagnosticInfo>.Enumerator enumerator = instance.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticInfo current = enumerator.Current;
                diagnostics.Add(current, location);
            }
            instance.Free();
        }
    }
}

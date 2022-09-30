using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal readonly struct TypeWithState
    {
        public readonly TypeSymbol? Type;

        public readonly NullableFlowState State;

        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, "Type")]
        public bool HasNullType
        {
            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, "Type")]
            get
            {
                return (object)Type == null;
            }
        }

        public bool MayBeNull => State == NullableFlowState.MaybeNull;

        public bool IsNotNull => State == NullableFlowState.NotNull;

        public static TypeWithState ForType(TypeSymbol? type)
        {
            return Create(type, NullableFlowState.MaybeDefault);
        }

        public static TypeWithState Create(TypeSymbol? type, NullableFlowState defaultState)
        {
            if (defaultState == NullableFlowState.MaybeDefault && ((object)type == null || type.IsTypeParameterDisallowingAnnotationInCSharp8()))
            {
                return new TypeWithState(type, defaultState);
            }
            NullableFlowState state = ((defaultState != 0 && ((object)type == null || type.CanContainNull())) ? NullableFlowState.MaybeNull : NullableFlowState.NotNull);
            return new TypeWithState(type, state);
        }

        public static TypeWithState Create(TypeWithAnnotations typeWithAnnotations, FlowAnalysisAnnotations annotations = FlowAnalysisAnnotations.None)
        {
            TypeSymbol type = typeWithAnnotations.Type;
            NullableFlowState defaultState;
            if (type.CanContainNull())
            {
                if ((annotations & FlowAnalysisAnnotations.MaybeNull) == FlowAnalysisAnnotations.MaybeNull)
                {
                    defaultState = NullableFlowState.MaybeDefault;
                }
                else
                {
                    if ((annotations & FlowAnalysisAnnotations.NotNull) != FlowAnalysisAnnotations.NotNull)
                    {
                        return typeWithAnnotations.ToTypeWithState();
                    }
                    defaultState = NullableFlowState.NotNull;
                }
            }
            else
            {
                defaultState = NullableFlowState.NotNull;
            }
            return Create(type, defaultState);
        }

        private TypeWithState(TypeSymbol? type, NullableFlowState state)
        {
            Type = type;
            State = state;
        }

        public string GetDebuggerDisplay()
        {
            return string.Format("{{Type:{0}, State:{1}{2}", Type?.GetDebuggerDisplay(), State, "}");
        }

        public override string ToString()
        {
            return GetDebuggerDisplay();
        }

        public TypeWithState WithNotNullState()
        {
            return new TypeWithState(Type, NullableFlowState.NotNull);
        }

        public TypeWithState WithSuppression(bool suppress)
        {
            if (!suppress)
            {
                return this;
            }
            return new TypeWithState(Type, NullableFlowState.NotNull);
        }

        public TypeWithAnnotations ToTypeWithAnnotations(CSharpCompilation compilation, bool asAnnotatedType = false)
        {
            TypeSymbol? type = Type;
            if ((object)type != null && type.IsTypeParameterDisallowingAnnotationInCSharp8())
            {
                TypeWithAnnotations result = TypeWithAnnotations.Create(Type, NullableAnnotation.NotAnnotated);
                if (!(State == NullableFlowState.MaybeDefault || asAnnotatedType))
                {
                    return result;
                }
                return result.SetIsAnnotated(compilation);
            }
            int num;
            if (!asAnnotatedType)
            {
                if (!State.IsNotNull())
                {
                    TypeSymbol? type2 = Type;
                    if ((object)type2 == null || type2.CanContainNull())
                    {
                        num = 2;
                        goto IL_0087;
                    }
                }
                num = 0;
            }
            else
            {
                TypeSymbol? type3 = Type;
                num = (((object)type3 == null || !type3!.IsValueType) ? 2 : 0);
            }
            goto IL_0087;
        IL_0087:
            NullableAnnotation nullableAnnotation = (NullableAnnotation)num;
            return TypeWithAnnotations.Create(Type, nullableAnnotation);
        }

        public TypeWithAnnotations ToAnnotatedTypeWithAnnotations(CSharpCompilation compilation)
        {
            return ToTypeWithAnnotations(compilation, asAnnotatedType: true);
        }
    }
}

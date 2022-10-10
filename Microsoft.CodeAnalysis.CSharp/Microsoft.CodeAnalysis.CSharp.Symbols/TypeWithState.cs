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
            if (Type?.IsTypeParameterDisallowingAnnotationInCSharp8() == true)
            {
                var type = TypeWithAnnotations.Create(Type, NullableAnnotation.NotAnnotated);
                return (State == NullableFlowState.MaybeDefault || asAnnotatedType) ? type.SetIsAnnotated(compilation) : type;
            }
            NullableAnnotation annotation = asAnnotatedType ?
                (Type?.IsValueType == true ? NullableAnnotation.NotAnnotated : NullableAnnotation.Annotated) :
                (State.IsNotNull() || Type?.CanContainNull() == false ? NullableAnnotation.NotAnnotated : NullableAnnotation.Annotated);
            return TypeWithAnnotations.Create(this.Type, annotation);
        }

        public TypeWithAnnotations ToAnnotatedTypeWithAnnotations(CSharpCompilation compilation)
        {
            return ToTypeWithAnnotations(compilation, asAnnotatedType: true);
        }
    }
}

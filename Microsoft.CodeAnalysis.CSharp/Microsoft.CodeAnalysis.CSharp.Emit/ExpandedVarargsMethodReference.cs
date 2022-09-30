using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    internal sealed class ExpandedVarargsMethodReference : IMethodReference, ISignature, ITypeMemberReference, IReference, INamedEntity, IGenericMethodInstanceReference, ISpecializedMethodReference
    {
        private readonly IMethodReference _underlyingMethod;

        private readonly ImmutableArray<IParameterTypeInformation> _argListParams;

        bool IMethodReference.AcceptsExtraArguments => _underlyingMethod.AcceptsExtraArguments;

        ushort IMethodReference.GenericParameterCount => _underlyingMethod.GenericParameterCount;

        bool IMethodReference.IsGeneric => _underlyingMethod.IsGeneric;

        ImmutableArray<IParameterTypeInformation> IMethodReference.ExtraParameters => _argListParams;

        IGenericMethodInstanceReference IMethodReference.AsGenericMethodInstanceReference
        {
            get
            {
                if (_underlyingMethod.AsGenericMethodInstanceReference == null)
                {
                    return null;
                }
                return this;
            }
        }

        ISpecializedMethodReference IMethodReference.AsSpecializedMethodReference
        {
            get
            {
                if (_underlyingMethod.AsSpecializedMethodReference == null)
                {
                    return null;
                }
                return this;
            }
        }

        CallingConvention ISignature.CallingConvention => _underlyingMethod.CallingConvention;

        ushort ISignature.ParameterCount => _underlyingMethod.ParameterCount;

        ImmutableArray<ICustomModifier> ISignature.ReturnValueCustomModifiers => _underlyingMethod.ReturnValueCustomModifiers;

        ImmutableArray<ICustomModifier> ISignature.RefCustomModifiers => _underlyingMethod.RefCustomModifiers;

        bool ISignature.ReturnValueIsByRef => _underlyingMethod.ReturnValueIsByRef;

        string INamedEntity.Name => _underlyingMethod.Name;

        IMethodReference ISpecializedMethodReference.UnspecializedVersion => new ExpandedVarargsMethodReference(_underlyingMethod.AsSpecializedMethodReference!.UnspecializedVersion, _argListParams);

        public ExpandedVarargsMethodReference(IMethodReference underlyingMethod, ImmutableArray<IParameterTypeInformation> argListParams)
        {
            _underlyingMethod = underlyingMethod;
            _argListParams = argListParams;
        }

        IMethodDefinition IMethodReference.GetResolvedMethod(EmitContext context)
        {
            return _underlyingMethod.GetResolvedMethod(context);
        }

        ImmutableArray<IParameterTypeInformation> ISignature.GetParameters(EmitContext context)
        {
            return _underlyingMethod.GetParameters(context);
        }

        ITypeReference ISignature.GetType(EmitContext context)
        {
            return _underlyingMethod.GetType(context);
        }

        ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
        {
            return _underlyingMethod.GetContainingType(context);
        }

        IEnumerable<ICustomAttribute> IReference.GetAttributes(EmitContext context)
        {
            return _underlyingMethod.GetAttributes(context);
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            if (((IMethodReference)this).AsGenericMethodInstanceReference != null)
            {
                visitor.Visit(this);
            }
            else if (((IMethodReference)this).AsSpecializedMethodReference != null)
            {
                visitor.Visit((IMethodReference)this);
            }
            else
            {
                visitor.Visit((IMethodReference)this);
            }
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            return null;
        }

        ISymbolInternal IReference.GetInternalSymbol()
        {
            return null;
        }

        IEnumerable<ITypeReference> IGenericMethodInstanceReference.GetGenericArguments(EmitContext context)
        {
            return _underlyingMethod.AsGenericMethodInstanceReference!.GetGenericArguments(context);
        }

        IMethodReference IGenericMethodInstanceReference.GetGenericMethod(EmitContext context)
        {
            return new ExpandedVarargsMethodReference(_underlyingMethod.AsGenericMethodInstanceReference!.GetGenericMethod(context), _argListParams);
        }

        public override string ToString()
        {
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            Append(instance, _underlyingMethod.GetInternalSymbol() ?? ((object)_underlyingMethod));
            instance.Builder.Append(" with __arglist( ");
            bool flag = true;
            ImmutableArray<IParameterTypeInformation>.Enumerator enumerator = _argListParams.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IParameterTypeInformation current = enumerator.Current;
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    instance.Builder.Append(", ");
                }
                if (current.IsByReference)
                {
                    instance.Builder.Append("ref ");
                }
                Append(instance, current.GetType(default(EmitContext)));
            }
            instance.Builder.Append(")");
            return instance.ToStringAndFree();
        }

        private static void Append(PooledStringBuilder result, object value)
        {
            ISymbol symbol = (value as ISymbolInternal)?.GetISymbol();
            if (symbol != null)
            {
                result.Builder.Append(symbol.ToDisplayString(SymbolDisplayFormat.ILVisualizationFormat));
            }
            else
            {
                result.Builder.Append(value);
            }
        }

        public sealed override bool Equals(object obj)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public sealed override int GetHashCode()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}

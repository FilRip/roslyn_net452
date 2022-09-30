using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public abstract class ReferenceIndexerBase : MetadataVisitor
    {
        private readonly HashSet<IReferenceOrISignature> _alreadySeen = new HashSet<IReferenceOrISignature>();

        private readonly HashSet<IReferenceOrISignature> _alreadyHasToken = new HashSet<IReferenceOrISignature>();

        protected bool typeReferenceNeedsToken;

        public ReferenceIndexerBase(EmitContext context)
            : base(context)
        {
        }

        public override void Visit(IAssemblyReference assemblyReference)
        {
            if (assemblyReference != Context.Module.GetContainingAssembly(Context))
            {
                RecordAssemblyReference(assemblyReference);
            }
        }

        protected abstract void RecordAssemblyReference(IAssemblyReference assemblyReference);

        public override void Visit(ICustomModifier customModifier)
        {
            typeReferenceNeedsToken = true;
            Visit(customModifier.GetModifier(Context));
        }

        public override void Visit(IEventDefinition eventDefinition)
        {
            typeReferenceNeedsToken = true;
            Visit(eventDefinition.GetType(Context));
        }

        public override void Visit(IFieldReference fieldReference)
        {
            if (_alreadySeen.Add(new IReferenceOrISignature(fieldReference)))
            {
                IUnitReference definingUnitReference = MetadataWriter.GetDefiningUnitReference(fieldReference.GetContainingType(Context), Context);
                if (definingUnitReference == null || definingUnitReference != Context.Module)
                {
                    Visit((ITypeMemberReference)fieldReference);
                    Visit(fieldReference.GetType(Context));
                    ReserveFieldToken(fieldReference);
                }
            }
        }

        protected abstract void ReserveFieldToken(IFieldReference fieldReference);

        public override void Visit(IFileReference fileReference)
        {
            RecordFileReference(fileReference);
        }

        protected abstract void RecordFileReference(IFileReference fileReference);

        public override void Visit(IGenericMethodInstanceReference genericMethodInstanceReference)
        {
            Visit(genericMethodInstanceReference.GetGenericArguments(Context));
            Visit(genericMethodInstanceReference.GetGenericMethod(Context));
        }

        public override void Visit(IGenericParameter genericParameter)
        {
            Visit(genericParameter.GetAttributes(Context));
            VisitTypeReferencesThatNeedTokens(genericParameter.GetConstraints(Context));
        }

        public override void Visit(IGenericTypeInstanceReference genericTypeInstanceReference)
        {
            INestedTypeReference asNestedTypeReference = genericTypeInstanceReference.AsNestedTypeReference;
            if (asNestedTypeReference != null)
            {
                ITypeReference containingType = asNestedTypeReference.GetContainingType(Context);
                if (containingType.AsGenericTypeInstanceReference != null || containingType.AsSpecializedNestedTypeReference != null)
                {
                    Visit(asNestedTypeReference.GetContainingType(Context));
                }
            }
            Visit(genericTypeInstanceReference.GetGenericType(Context));
            Visit(genericTypeInstanceReference.GetGenericArguments(Context));
        }

        public override void Visit(IMarshallingInformation marshallingInformation)
        {
        }

        public override void Visit(IMethodDefinition method)
        {
            base.Visit(method);
            ProcessMethodBody(method);
        }

        protected abstract void ProcessMethodBody(IMethodDefinition method);

        public override void Visit(IMethodReference methodReference)
        {
            IGenericMethodInstanceReference asGenericMethodInstanceReference = methodReference.AsGenericMethodInstanceReference;
            if (asGenericMethodInstanceReference != null)
            {
                Visit(asGenericMethodInstanceReference);
            }
            else
            {
                if (!_alreadySeen.Add(new IReferenceOrISignature(methodReference)))
                {
                    return;
                }
                IUnitReference definingUnitReference = MetadataWriter.GetDefiningUnitReference(methodReference.GetContainingType(Context), Context);
                if (definingUnitReference == null || definingUnitReference != Context.Module || methodReference.AcceptsExtraArguments)
                {
                    Visit((ITypeMemberReference)methodReference);
                    VisitSignature(methodReference.AsSpecializedMethodReference?.UnspecializedVersion ?? methodReference);
                    if (methodReference.AcceptsExtraArguments)
                    {
                        Visit(methodReference.ExtraParameters);
                    }
                    ReserveMethodToken(methodReference);
                }
            }
        }

        public void VisitSignature(ISignature signature)
        {
            Visit(signature.GetType(Context));
            Visit(signature.GetParameters(Context));
            Visit(signature.RefCustomModifiers);
            Visit(signature.ReturnValueCustomModifiers);
        }

        protected abstract void ReserveMethodToken(IMethodReference methodReference);

        public abstract override void Visit(CommonPEModuleBuilder module);

        public override void Visit(IModuleReference moduleReference)
        {
            if (moduleReference != Context.Module)
            {
                RecordModuleReference(moduleReference);
            }
        }

        protected abstract void RecordModuleReference(IModuleReference moduleReference);

        public abstract override void Visit(IPlatformInvokeInformation platformInvokeInformation);

        public override void Visit(INamespaceTypeReference namespaceTypeReference)
        {
            if (!typeReferenceNeedsToken && namespaceTypeReference.TypeCode != PrimitiveTypeCode.NotPrimitive)
            {
                return;
            }
            RecordTypeReference(namespaceTypeReference);
            IUnitReference unit = namespaceTypeReference.GetUnit(Context);
            if (unit is IAssemblyReference assemblyReference)
            {
                Visit(assemblyReference);
            }
            else if (unit is IModuleReference moduleReference)
            {
                IAssemblyReference containingAssembly = moduleReference.GetContainingAssembly(Context);
                if (containingAssembly != null && containingAssembly != Context.Module.GetContainingAssembly(Context))
                {
                    Visit(containingAssembly);
                }
                else
                {
                    Visit(moduleReference);
                }
            }
        }

        protected abstract void RecordTypeReference(ITypeReference typeReference);

        public override void Visit(INestedTypeReference nestedTypeReference)
        {
            if (typeReferenceNeedsToken || nestedTypeReference.AsSpecializedNestedTypeReference == null)
            {
                RecordTypeReference(nestedTypeReference);
            }
        }

        public override void Visit(IPropertyDefinition propertyDefinition)
        {
            Visit(propertyDefinition.RefCustomModifiers);
            Visit(propertyDefinition.ReturnValueCustomModifiers);
            Visit(propertyDefinition.GetType(Context));
            Visit(propertyDefinition.Parameters);
        }

        public override void Visit(ManagedResource resourceReference)
        {
            Visit(resourceReference.Attributes);
            IFileReference externalFile = resourceReference.ExternalFile;
            if (externalFile != null)
            {
                Visit(externalFile);
            }
        }

        public override void Visit(SecurityAttribute securityAttribute)
        {
            Visit(securityAttribute.Attribute);
        }

        public void VisitTypeDefinitionNoMembers(ITypeDefinition typeDefinition)
        {
            Visit(typeDefinition.GetAttributes(Context));
            ITypeReference baseClass = typeDefinition.GetBaseClass(Context);
            if (baseClass != null)
            {
                typeReferenceNeedsToken = true;
                Visit(baseClass);
            }
            Visit(typeDefinition.GetExplicitImplementationOverrides(Context));
            if (typeDefinition.HasDeclarativeSecurity)
            {
                Visit(typeDefinition.SecurityAttributes);
            }
            VisitTypeReferencesThatNeedTokens(typeDefinition.Interfaces(Context));
            if (typeDefinition.IsGeneric)
            {
                Visit(typeDefinition.GenericParameters);
            }
        }

        public override void Visit(ITypeDefinition typeDefinition)
        {
            VisitTypeDefinitionNoMembers(typeDefinition);
            Visit(typeDefinition.GetEvents(Context));
            Visit(typeDefinition.GetFields(Context));
            Visit(typeDefinition.GetMethods(Context));
            VisitNestedTypes(typeDefinition.GetNestedTypes(Context));
            Visit(typeDefinition.GetProperties(Context));
        }

        public void VisitTypeReferencesThatNeedTokens(IEnumerable<TypeReferenceWithAttributes> refsWithAttributes)
        {
            foreach (TypeReferenceWithAttributes refsWithAttribute in refsWithAttributes)
            {
                Visit(refsWithAttribute.Attributes);
                VisitTypeReferencesThatNeedTokens(refsWithAttribute.TypeRef);
            }
        }

        private void VisitTypeReferencesThatNeedTokens(ITypeReference typeReference)
        {
            typeReferenceNeedsToken = true;
            Visit(typeReference);
        }

        public override void Visit(ITypeMemberReference typeMemberReference)
        {
            RecordTypeMemberReference(typeMemberReference);
            typeReferenceNeedsToken = true;
            Visit(typeMemberReference.GetContainingType(Context));
        }

        protected abstract void RecordTypeMemberReference(ITypeMemberReference typeMemberReference);

        public override void Visit(IArrayTypeReference arrayTypeReference)
        {
            ITypeReference elementType = arrayTypeReference.GetElementType(Context);
            while (true)
            {
                if (!VisitTypeReference(elementType))
                {
                    return;
                }
                if (!(elementType is IArrayTypeReference))
                {
                    break;
                }
                elementType = ((IArrayTypeReference)elementType).GetElementType(Context);
            }
            DispatchAsReference(elementType);
        }

        public override void Visit(IPointerTypeReference pointerTypeReference)
        {
            ITypeReference targetType = pointerTypeReference.GetTargetType(Context);
            while (true)
            {
                if (!VisitTypeReference(targetType))
                {
                    return;
                }
                if (!(targetType is IPointerTypeReference))
                {
                    break;
                }
                targetType = ((IPointerTypeReference)targetType).GetTargetType(Context);
            }
            DispatchAsReference(targetType);
        }

        public override void Visit(ITypeReference typeReference)
        {
            if (VisitTypeReference(typeReference))
            {
                DispatchAsReference(typeReference);
            }
        }

        private bool VisitTypeReference(ITypeReference typeReference)
        {
            if (!_alreadySeen.Add(new IReferenceOrISignature(typeReference)))
            {
                if (!typeReferenceNeedsToken)
                {
                    return false;
                }
                typeReferenceNeedsToken = false;
                if (!_alreadyHasToken.Add(new IReferenceOrISignature(typeReference)))
                {
                    return false;
                }
                RecordTypeReference(typeReference);
                return false;
            }
            INestedTypeReference asNestedTypeReference = typeReference.AsNestedTypeReference;
            if (typeReferenceNeedsToken || asNestedTypeReference != null || (typeReference.TypeCode == PrimitiveTypeCode.NotPrimitive && typeReference.AsNamespaceTypeReference != null))
            {
                ISpecializedNestedTypeReference specializedNestedTypeReference = asNestedTypeReference?.AsSpecializedNestedTypeReference;
                if (specializedNestedTypeReference != null)
                {
                    INestedTypeReference unspecializedVersion = specializedNestedTypeReference.GetUnspecializedVersion(Context);
                    if (_alreadyHasToken.Add(new IReferenceOrISignature(unspecializedVersion)))
                    {
                        RecordTypeReference(unspecializedVersion);
                    }
                }
                if (typeReferenceNeedsToken && _alreadyHasToken.Add(new IReferenceOrISignature(typeReference)))
                {
                    RecordTypeReference(typeReference);
                }
                if (asNestedTypeReference != null)
                {
                    typeReferenceNeedsToken = typeReference.AsSpecializedNestedTypeReference == null;
                    Visit(asNestedTypeReference.GetContainingType(Context));
                }
            }
            typeReferenceNeedsToken = false;
            return true;
        }
    }
}

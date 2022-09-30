using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Emit;

using Roslyn.Utilities;

namespace Microsoft.Cci
{
    public abstract class ReferenceIndexer : ReferenceIndexerBase
    {
        protected readonly MetadataWriter metadataWriter;

        private readonly HashSet<IImportScope> _alreadySeenScopes = new HashSet<IImportScope>();

        public ReferenceIndexer(MetadataWriter metadataWriter)
            : base(metadataWriter.Context)
        {
            this.metadataWriter = metadataWriter;
        }

        public override void Visit(CommonPEModuleBuilder module)
        {
            Visit(module.GetSourceAssemblyAttributes(Context.IsRefAssembly));
            Visit(module.GetSourceAssemblySecurityAttributes());
            Visit(module.GetAssemblyReferences(Context));
            Visit(module.GetSourceModuleAttributes());
            Visit(module.GetTopLevelTypeDefinitions(Context));
            ImmutableArray<ExportedType>.Enumerator enumerator = module.GetExportedTypes(Context.Diagnostics).GetEnumerator();
            while (enumerator.MoveNext())
            {
                VisitExportedType(enumerator.Current.Type);
            }
            Visit(module.GetResources(Context));
            VisitImports(module.GetImports());
            Visit(module.GetFiles(Context));
        }

        private void VisitExportedType(ITypeReference exportedType)
        {
            IUnitReference definingUnitReference = MetadataWriter.GetDefiningUnitReference(exportedType, Context);
            if (definingUnitReference is IAssemblyReference assemblyReference)
            {
                Visit(assemblyReference);
                return;
            }
            IAssemblyReference containingAssembly = ((IModuleReference)definingUnitReference).GetContainingAssembly(Context);
            if (containingAssembly != null && containingAssembly != Context.Module.GetContainingAssembly(Context))
            {
                Visit(containingAssembly);
            }
        }

        public void VisitMethodBodyReference(IReference reference)
        {
            if (reference is ITypeReference typeReference)
            {
                typeReferenceNeedsToken = true;
                Visit(typeReference);
            }
            else if (reference is IFieldReference fieldReference)
            {
                if (fieldReference.IsContextualNamedEntity)
                {
                    ((IContextualNamedEntity)fieldReference).AssociateWithMetadataWriter(metadataWriter);
                }
                Visit(fieldReference);
            }
            else if (reference is IMethodReference methodReference)
            {
                Visit(methodReference);
            }
        }

        protected override void RecordAssemblyReference(IAssemblyReference assemblyReference)
        {
            metadataWriter.GetAssemblyReferenceHandle(assemblyReference);
        }

        protected override void ProcessMethodBody(IMethodDefinition method)
        {
            if (!method.HasBody() || metadataWriter.MetadataOnly)
            {
                return;
            }
            IMethodBody body = method.GetBody(Context);
            if (body != null)
            {
                Visit(body);
                IImportScope importScope = body.ImportScope;
                while (importScope != null && _alreadySeenScopes.Add(importScope))
                {
                    VisitImports(importScope.GetUsedNamespaces());
                    importScope = importScope.Parent;
                }
            }
            else if (!metadataWriter.MetadataOnly)
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private void VisitImports(ImmutableArray<UsedNamespaceOrType> imports)
        {
            ImmutableArray<UsedNamespaceOrType>.Enumerator enumerator = imports.GetEnumerator();
            while (enumerator.MoveNext())
            {
                UsedNamespaceOrType current = enumerator.Current;
                if (current.TargetAssemblyOpt != null)
                {
                    Visit(current.TargetAssemblyOpt);
                }
                if (current.TargetTypeOpt != null)
                {
                    typeReferenceNeedsToken = true;
                    Visit(current.TargetTypeOpt);
                }
            }
        }

        protected override void RecordTypeReference(ITypeReference typeReference)
        {
            metadataWriter.GetTypeHandle(typeReference);
        }

        protected override void RecordTypeMemberReference(ITypeMemberReference typeMemberReference)
        {
            metadataWriter.GetMemberReferenceHandle(typeMemberReference);
        }

        protected override void RecordFileReference(IFileReference fileReference)
        {
            metadataWriter.GetAssemblyFileHandle(fileReference);
        }

        protected override void ReserveMethodToken(IMethodReference methodReference)
        {
            metadataWriter.GetMethodHandle(methodReference);
        }

        protected override void ReserveFieldToken(IFieldReference fieldReference)
        {
            metadataWriter.GetFieldHandle(fieldReference);
        }

        protected override void RecordModuleReference(IModuleReference moduleReference)
        {
            metadataWriter.GetModuleReferenceHandle(moduleReference.Name);
        }

        public override void Visit(IPlatformInvokeInformation platformInvokeInformation)
        {
            metadataWriter.GetModuleReferenceHandle(platformInvokeInformation.ModuleName);
        }
    }
}

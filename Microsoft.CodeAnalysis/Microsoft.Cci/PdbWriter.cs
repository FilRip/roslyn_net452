using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.DiaSymReader;

using Roslyn.Utilities;

namespace Microsoft.Cci
{
    public sealed class PdbWriter : IDisposable
    {
        internal const uint Age = 1u;

        private readonly SourceHashAlgorithm _hashAlgorithmNameOpt;

        private readonly string _fileName;

        private readonly Func<ISymWriterMetadataProvider, SymUnmanagedWriter> _symWriterFactory;

        private readonly Dictionary<DebugSourceDocument, int> _documentIndex;

        private MetadataWriter _metadataWriter;

        private SymUnmanagedWriter _symWriter;

        private SymUnmanagedSequencePointsWriter _sequencePointsWriter;

        private readonly Dictionary<object, string> _qualifiedNameCache;

        private bool IsDeterministic
        {
            get
            {
                //HashAlgorithmName hashAlgorithmNameOpt = _hashAlgorithmNameOpt; // FilRip commented
                //return hashAlgorithmNameOpt.Name != null;
                return _hashAlgorithmNameOpt != SourceHashAlgorithm.None;
            }
        }

        private CommonPEModuleBuilder Module => Context.Module;

        private EmitContext Context => _metadataWriter.Context;

        public PdbWriter(string fileName, Func<ISymWriterMetadataProvider, SymUnmanagedWriter> symWriterFactory, SourceHashAlgorithm hashAlgorithmNameOpt)
        {
            _fileName = fileName;
            _symWriterFactory = symWriterFactory;
            _hashAlgorithmNameOpt = hashAlgorithmNameOpt;
            _documentIndex = new Dictionary<DebugSourceDocument, int>();
            _qualifiedNameCache = new Dictionary<object, string>(ReferenceEqualityComparer.Instance);
        }

        public void WriteTo(Stream stream)
        {
            _symWriter.WriteTo(stream);
        }

        public void Dispose()
        {
            _symWriter?.Dispose();
        }

        public void SerializeDebugInfo(IMethodBody methodBody, StandaloneSignatureHandle localSignatureHandleOpt, CustomDebugInfoWriter customDebugInfoWriter)
        {
            bool flag = methodBody.StateMachineTypeName != null;
            if (!flag && methodBody.SequencePoints.IsEmpty && methodBody.MethodDefinition != (Context.Module.DebugEntryPoint ?? Context.Module.PEEntryPoint))
            {
                return;
            }
            MethodDefinitionHandle methodDefinitionHandle = (MethodDefinitionHandle)_metadataWriter.GetMethodHandle(methodBody.MethodDefinition);
            int token = MetadataTokens.GetToken(methodDefinitionHandle);
            OpenMethod(token, methodBody.MethodDefinition);
            ImmutableArray<LocalScope> localScopes = methodBody.LocalScopes;
            if (localScopes.Length > 0)
            {
                DefineScopeLocals(localScopes[0], localSignatureHandleOpt);
            }
            if (!flag && methodBody.ImportScope != null)
            {
                if (customDebugInfoWriter.ShouldForwardNamespaceScopes(Context, methodBody, methodDefinitionHandle, out var forwardToMethod))
                {
                    if (forwardToMethod != null)
                    {
                        UsingNamespace("@" + MetadataTokens.GetToken(_metadataWriter.GetMethodHandle(forwardToMethod)), methodBody.MethodDefinition);
                    }
                }
                else
                {
                    DefineNamespaceScopes(methodBody);
                }
            }
            DefineLocalScopes(localScopes, localSignatureHandleOpt);
            EmitSequencePoints(methodBody.SequencePoints);
            if (methodBody.MoveNextBodyInfo is AsyncMoveNextBodyDebugInfo asyncMoveNextBodyDebugInfo)
            {
                SymUnmanagedWriter symWriter = _symWriter;
                int token2 = MetadataTokens.GetToken(_metadataWriter.GetMethodHandle(asyncMoveNextBodyDebugInfo.KickoffMethod));
                int catchHandlerOffset = asyncMoveNextBodyDebugInfo.CatchHandlerOffset;
                ImmutableArray<int> immutableArray = asyncMoveNextBodyDebugInfo.YieldOffsets;
                ReadOnlySpan<int> yieldOffsets = immutableArray.AsSpan();
                immutableArray = asyncMoveNextBodyDebugInfo.ResumeOffsets;
                symWriter.SetAsyncInfo(token, token2, catchHandlerOffset, yieldOffsets, immutableArray.AsSpan());
            }
            CompilationOptions options = Context.Module.CommonCompilation.Options;
            bool suppressNewCustomDebugInfo = options.OutputKind == OutputKind.WindowsRuntimeMetadata;
            bool emitEncInfo = options.EnableEditAndContinue && _metadataWriter.IsFullMetadata;
            byte[] array = customDebugInfoWriter.SerializeMethodDebugInfo(Context, methodBody, methodDefinitionHandle, emitEncInfo, suppressNewCustomDebugInfo, out bool emitExternNamespaces);
            if (array != null)
            {
                _symWriter.DefineCustomMetadata(array);
            }
            if (emitExternNamespaces)
            {
                DefineAssemblyReferenceAliases();
            }
            CloseMethod(methodBody.IL.Length);
        }

        private void DefineNamespaceScopes(IMethodBody methodBody)
        {
            CommonPEModuleBuilder module = Module;
            bool generateVisualBasicStylePdb = module.GenerateVisualBasicStylePdb;
            IMethodDefinition methodDefinition = methodBody.MethodDefinition;
            IImportScope importScope = methodBody.ImportScope;
            PooledHashSet<string> pooledHashSet = null;
            ImmutableArray<UsedNamespaceOrType>.Enumerator enumerator;
            if (!generateVisualBasicStylePdb)
            {
                for (IImportScope importScope2 = importScope; importScope2 != null; importScope2 = importScope2.Parent)
                {
                    enumerator = importScope2.GetUsedNamespaces().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        UsedNamespaceOrType current = enumerator.Current;
                        if (current.TargetNamespaceOpt == null && current.TargetTypeOpt == null)
                        {
                            if (pooledHashSet == null)
                            {
                                pooledHashSet = PooledHashSet<string>.GetInstance();
                            }
                            pooledHashSet.Add(current.AliasOpt);
                        }
                    }
                }
            }
            for (IImportScope importScope3 = importScope; importScope3 != null; importScope3 = importScope3.Parent)
            {
                enumerator = importScope3.GetUsedNamespaces().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    UsedNamespaceOrType current2 = enumerator.Current;
                    string text = TryEncodeImport(current2, pooledHashSet, isProjectLevel: false);
                    if (text != null)
                    {
                        UsingNamespace(text, methodDefinition);
                    }
                }
            }
            pooledHashSet?.Free();
            if (!generateVisualBasicStylePdb)
            {
                return;
            }
            string defaultNamespace = module.DefaultNamespace;
            if (!string.IsNullOrEmpty(defaultNamespace))
            {
                UsingNamespace("*" + defaultNamespace, module);
            }
            foreach (string item in module.LinkedAssembliesDebugInfo)
            {
                UsingNamespace("&" + item, module);
            }
            enumerator = module.GetImports().GetEnumerator();
            while (enumerator.MoveNext())
            {
                UsedNamespaceOrType current4 = enumerator.Current;
                string text2 = TryEncodeImport(current4, null, isProjectLevel: true);
                if (text2 != null)
                {
                    UsingNamespace(text2, methodDefinition);
                }
            }
            UsingNamespace(GetOrCreateSerializedNamespaceName(methodDefinition.ContainingNamespace), methodDefinition);
        }

        private void DefineAssemblyReferenceAliases()
        {
            ImmutableArray<AssemblyReferenceAlias>.Enumerator enumerator = Module.GetAssemblyReferenceAliases(Context).GetEnumerator();
            while (enumerator.MoveNext())
            {
                AssemblyReferenceAlias current = enumerator.Current;
                UsingNamespace("Z" + current.Name + " " + current.Assembly.Identity.GetDisplayName(), Module);
            }
        }

        private string TryEncodeImport(UsedNamespaceOrType import, HashSet<string> declaredExternAliasesOpt, bool isProjectLevel)
        {
            if (Module.GenerateVisualBasicStylePdb)
            {
                if (import.TargetTypeOpt != null)
                {
                    if (import.TargetTypeOpt.IsTypeSpecification())
                    {
                        return null;
                    }
                    string orCreateSerializedTypeName = GetOrCreateSerializedTypeName(import.TargetTypeOpt);
                    if (import.AliasOpt != null)
                    {
                        return (isProjectLevel ? "@PA:" : "@FA:") + import.AliasOpt + "=" + orCreateSerializedTypeName;
                    }
                    return (isProjectLevel ? "@PT:" : "@FT:") + orCreateSerializedTypeName;
                }
                if (import.TargetNamespaceOpt != null)
                {
                    string orCreateSerializedNamespaceName = GetOrCreateSerializedNamespaceName(import.TargetNamespaceOpt);
                    if (import.AliasOpt == null)
                    {
                        return (isProjectLevel ? "@P:" : "@F:") + orCreateSerializedNamespaceName;
                    }
                    return (isProjectLevel ? "@PA:" : "@FA:") + import.AliasOpt + "=" + orCreateSerializedNamespaceName;
                }
                return (isProjectLevel ? "@PX:" : "@FX:") + import.AliasOpt + "=" + import.TargetXmlNamespaceOpt;
            }
            if (import.TargetTypeOpt != null)
            {
                string orCreateSerializedTypeName2 = GetOrCreateSerializedTypeName(import.TargetTypeOpt);
                if (import.AliasOpt == null)
                {
                    return "T" + orCreateSerializedTypeName2;
                }
                return "A" + import.AliasOpt + " T" + orCreateSerializedTypeName2;
            }
            if (import.TargetNamespaceOpt != null)
            {
                string orCreateSerializedNamespaceName2 = GetOrCreateSerializedNamespaceName(import.TargetNamespaceOpt);
                if (import.AliasOpt != null)
                {
                    if (import.TargetAssemblyOpt == null)
                    {
                        return "A" + import.AliasOpt + " U" + orCreateSerializedNamespaceName2;
                    }
                    return "A" + import.AliasOpt + " E" + orCreateSerializedNamespaceName2 + " " + GetAssemblyReferenceAlias(import.TargetAssemblyOpt, declaredExternAliasesOpt);
                }
                if (import.TargetAssemblyOpt == null)
                {
                    return "U" + orCreateSerializedNamespaceName2;
                }
                return "E" + orCreateSerializedNamespaceName2 + " " + GetAssemblyReferenceAlias(import.TargetAssemblyOpt, declaredExternAliasesOpt);
            }
            return "X" + import.AliasOpt;
        }

        internal string GetOrCreateSerializedNamespaceName(INamespace @namespace)
        {
            if (!_qualifiedNameCache.TryGetValue(@namespace, out var value))
            {
                value = TypeNameSerializer.BuildQualifiedNamespaceName(@namespace);
                _qualifiedNameCache.Add(@namespace, value);
            }
            return value;
        }

        internal string GetOrCreateSerializedTypeName(ITypeReference typeReference)
        {
            if (!_qualifiedNameCache.TryGetValue(typeReference, out var value))
            {
                value = ((!Module.GenerateVisualBasicStylePdb) ? typeReference.GetSerializedTypeName(Context) : SerializeVisualBasicImportTypeReference(typeReference));
                _qualifiedNameCache.Add(typeReference, value);
            }
            return value;
        }

        private string SerializeVisualBasicImportTypeReference(ITypeReference typeReference)
        {
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            INestedTypeReference asNestedTypeReference = typeReference.AsNestedTypeReference;
            ArrayBuilder<string> arrayBuilder;
            if (asNestedTypeReference != null)
            {
                arrayBuilder = ArrayBuilder<string>.GetInstance();
                while (asNestedTypeReference != null)
                {
                    arrayBuilder.Add(asNestedTypeReference.Name);
                    typeReference = asNestedTypeReference.GetContainingType(_metadataWriter.Context);
                    asNestedTypeReference = typeReference.AsNestedTypeReference;
                }
            }
            else
            {
                arrayBuilder = null;
            }
            INamespaceTypeReference asNamespaceTypeReference = typeReference.AsNamespaceTypeReference;
            string namespaceName = asNamespaceTypeReference.NamespaceName;
            if (namespaceName.Length != 0)
            {
                instance.Builder.Append(namespaceName);
                instance.Builder.Append('.');
            }
            instance.Builder.Append(asNamespaceTypeReference.Name);
            if (arrayBuilder != null)
            {
                for (int num = arrayBuilder.Count - 1; num >= 0; num--)
                {
                    instance.Builder.Append('.');
                    instance.Builder.Append(arrayBuilder[num]);
                }
                arrayBuilder.Free();
            }
            return instance.ToStringAndFree();
        }

        private string GetAssemblyReferenceAlias(IAssemblyReference assembly, HashSet<string> declaredExternAliases)
        {
            ImmutableArray<AssemblyReferenceAlias>.Enumerator enumerator = _metadataWriter.Context.Module.GetAssemblyReferenceAliases(_metadataWriter.Context).GetEnumerator();
            while (enumerator.MoveNext())
            {
                AssemblyReferenceAlias current = enumerator.Current;
                if (assembly == current.Assembly && declaredExternAliases.Contains(current.Name))
                {
                    return current.Name;
                }
            }
            throw ExceptionUtilities.Unreachable;
        }

        private void DefineLocalScopes(ImmutableArray<LocalScope> scopes, StandaloneSignatureHandle localSignatureHandleOpt)
        {
            bool generateVisualBasicStylePdb = Module.GenerateVisualBasicStylePdb;
            ArrayBuilder<LocalScope> instance = ArrayBuilder<LocalScope>.GetInstance();
            for (int i = 1; i < scopes.Length; i++)
            {
                LocalScope localScope = scopes[i];
                while (instance.Count > 0)
                {
                    LocalScope localScope2 = instance.Last();
                    if (localScope.StartOffset < localScope2.StartOffset + localScope2.Length)
                    {
                        break;
                    }
                    instance.RemoveLast();
                    _symWriter.CloseScope(generateVisualBasicStylePdb ? (localScope2.EndOffset - 1) : localScope2.EndOffset);
                }
                instance.Add(localScope);
                _symWriter.OpenScope(localScope.StartOffset);
                DefineScopeLocals(localScope, localSignatureHandleOpt);
            }
            for (int num = instance.Count - 1; num >= 0; num--)
            {
                LocalScope localScope3 = instance[num];
                _symWriter.CloseScope(generateVisualBasicStylePdb ? (localScope3.EndOffset - 1) : localScope3.EndOffset);
            }
            instance.Free();
        }

        private void DefineScopeLocals(LocalScope currentScope, StandaloneSignatureHandle localSignatureHandleOpt)
        {
            ImmutableArray<ILocalDefinition>.Enumerator enumerator = currentScope.Constants.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ILocalDefinition current = enumerator.Current;
                StandaloneSignatureHandle standaloneSignatureHandle = _metadataWriter.SerializeLocalConstantStandAloneSignature(current);
                if (!_metadataWriter.IsLocalNameTooLong(current))
                {
                    _symWriter.DefineLocalConstant(current.Name, current.CompileTimeValue.Value, MetadataTokens.GetToken(standaloneSignatureHandle));
                }
            }
            enumerator = currentScope.Variables.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ILocalDefinition current2 = enumerator.Current;
                if (!_metadataWriter.IsLocalNameTooLong(current2))
                {
                    _symWriter.DefineLocalVariable(current2.SlotIndex, current2.Name, (int)current2.PdbAttributes, (!localSignatureHandleOpt.IsNil) ? MetadataTokens.GetToken(localSignatureHandleOpt) : 0);
                }
            }
        }

        public void SetMetadataEmitter(MetadataWriter metadataWriter)
        {
            SymUnmanagedWriterCreationOptions options = (IsDeterministic ? SymUnmanagedWriterCreationOptions.Deterministic : SymUnmanagedWriterCreationOptions.UseComRegistry) | SymUnmanagedWriterCreationOptions.UseAlternativeLoadPath;
            SymWriterMetadataProvider symWriterMetadataProvider = new SymWriterMetadataProvider(metadataWriter);
            SymUnmanagedWriter symUnmanagedWriter;
            try
            {
                symUnmanagedWriter = ((_symWriterFactory != null) ? _symWriterFactory(symWriterMetadataProvider) : SymUnmanagedWriterFactory.CreateWriter(symWriterMetadataProvider, options));
            }
            catch (DllNotFoundException ex)
            {
                throw new SymUnmanagedWriterException(ex.Message);
            }
            catch (SymUnmanagedWriterException ex2) when (ex2.InnerException is NotSupportedException)
            {
                throw new SymUnmanagedWriterException(string.Format(IsDeterministic ? CodeAnalysisResources.SymWriterNotDeterministic : CodeAnalysisResources.SymWriterOlderVersionThanRequired, ex2.ImplementationModuleName));
            }
            _metadataWriter = metadataWriter;
            _symWriter = symUnmanagedWriter;
            _sequencePointsWriter = new SymUnmanagedSequencePointsWriter(symUnmanagedWriter);
        }

        public BlobContentId GetContentId()
        {
            BlobContentId result;
            if (IsDeterministic)
            {
                result = BlobContentId.FromHash(CryptographicHashProvider.ComputeHash(_hashAlgorithmNameOpt, _symWriter.GetUnderlyingData()));
                _symWriter.UpdateSignature(result.Guid, result.Stamp, 1);
            }
            else
            {
                _symWriter.GetSignature(out var guid, out var stamp, out var _);
                result = new BlobContentId(guid, stamp);
            }
            _symWriter.Dispose();
            return result;
        }

        public void SetEntryPoint(int entryMethodToken)
        {
            _symWriter.SetEntryPoint(entryMethodToken);
        }

        private int GetDocumentIndex(DebugSourceDocument document)
        {
            if (_documentIndex.TryGetValue(document, out var value))
            {
                return value;
            }
            return AddDocumentIndex(document);
        }

        private int AddDocumentIndex(DebugSourceDocument document)
        {
            DebugSourceInfo sourceInfo = document.GetSourceInfo();
            ImmutableArray<byte> immutableArray = sourceInfo.Checksum;
            Guid algorithmId;
            ReadOnlySpan<byte> checksum;
            if (!immutableArray.IsDefault)
            {
                algorithmId = sourceInfo.ChecksumAlgorithmId;
                immutableArray = sourceInfo.Checksum;
                checksum = immutableArray.AsSpan();
            }
            else
            {
                algorithmId = default(Guid);
                checksum = null;
            }
            immutableArray = sourceInfo.EmbeddedTextBlob;
            ReadOnlySpan<byte> source;
            if (!immutableArray.IsDefault)
            {
                immutableArray = sourceInfo.EmbeddedTextBlob;
                source = immutableArray.AsSpan();
            }
            else
            {
                source = null;
            }
            int num = _symWriter.DefineDocument(document.Location, document.Language, document.LanguageVendor, document.DocumentType, algorithmId, checksum, source);
            _documentIndex.Add(document, num);
            return num;
        }

        private void OpenMethod(int methodToken, IMethodDefinition method)
        {
            _symWriter.OpenMethod(methodToken);
            _symWriter.OpenScope(0);
        }

        private void CloseMethod(int ilLength)
        {
            _symWriter.CloseScope(ilLength);
            _symWriter.CloseMethod();
        }

        private void UsingNamespace(string fullName, INamedEntity errorEntity)
        {
            if (!_metadataWriter.IsUsingStringTooLong(fullName, errorEntity))
            {
                _symWriter.UsingNamespace(fullName);
            }
        }

        private void EmitSequencePoints(ImmutableArray<SequencePoint> sequencePoints)
        {
            int num = -1;
            DebugSourceDocument debugSourceDocument = null;
            ImmutableArray<SequencePoint>.Enumerator enumerator = sequencePoints.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SequencePoint current = enumerator.Current;
                DebugSourceDocument document = current.Document;
                int documentIndex;
                if (debugSourceDocument == document)
                {
                    documentIndex = num;
                }
                else
                {
                    debugSourceDocument = document;
                    documentIndex = (num = GetDocumentIndex(debugSourceDocument));
                }
                _sequencePointsWriter.Add(documentIndex, current.Offset, current.StartLine, current.StartColumn, current.EndLine, current.EndColumn);
            }
            _sequencePointsWriter.Flush();
        }

        [Conditional("DEBUG")]
        public void AssertAllDefinitionsHaveTokens(MultiDictionary<DebugSourceDocument, DefinitionWithLocation> file2definitions)
        {
            foreach (KeyValuePair<DebugSourceDocument, MultiDictionary<DebugSourceDocument, DefinitionWithLocation>.ValueSet> file2definition in file2definitions)
            {
                foreach (DefinitionWithLocation item in file2definition.Value)
                {
                    _metadataWriter.GetDefinitionHandle(item.Definition);
                }
            }
        }

        public void WriteDefinitionLocations(MultiDictionary<DebugSourceDocument, DefinitionWithLocation> file2definitions)
        {
            bool flag = false;
            foreach (KeyValuePair<DebugSourceDocument, MultiDictionary<DebugSourceDocument, DefinitionWithLocation>.ValueSet> file2definition in file2definitions)
            {
                foreach (DefinitionWithLocation item in file2definition.Value)
                {
                    if (!flag)
                    {
                        _symWriter.OpenTokensToSourceSpansMap();
                        flag = true;
                    }
                    int token = MetadataTokens.GetToken(_metadataWriter.GetDefinitionHandle(item.Definition));
                    _symWriter.MapTokenToSourceSpan(token, GetDocumentIndex(file2definition.Key), item.StartLine + 1, item.StartColumn + 1, item.EndLine + 1, item.EndColumn + 1);
                }
            }
            if (flag)
            {
                _symWriter.CloseTokensToSourceSpansMap();
            }
        }

        public void EmbedSourceLink(Stream stream)
        {
            byte[] sourceLinkData;
            try
            {
                sourceLinkData = stream.ReadAllBytes();
            }
            catch (Exception ex)
            {
                throw new SymUnmanagedWriterException(ex.Message, ex);
            }
            try
            {
                _symWriter.SetSourceLinkData(sourceLinkData);
            }
            catch (SymUnmanagedWriterException ex2) when (ex2.InnerException is NotSupportedException)
            {
                throw new SymUnmanagedWriterException(string.Format(CodeAnalysisResources.SymWriterDoesNotSupportSourceLink, ex2.ImplementationModuleName));
            }
        }

        public void WriteRemainingDebugDocuments(IReadOnlyDictionary<string, DebugSourceDocument> documents)
        {
            foreach (KeyValuePair<string, DebugSourceDocument> item in from kvp in documents
                                                                       where !_documentIndex.ContainsKey(kvp.Value)
                                                                       orderby kvp.Key
                                                                       select kvp)
            {
                AddDocumentIndex(item.Value);
            }
        }
    }
}

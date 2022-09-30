using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.Interop;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public class DesktopStrongNameProvider : StrongNameProvider
    {
        internal sealed class ClrStrongNameMissingException : Exception
        {
        }

        private readonly ImmutableArray<string> _keyFileSearchPaths;

        internal override StrongNameFileSystem FileSystem { get; }

        public DesktopStrongNameProvider(ImmutableArray<string> keyFileSearchPaths)
            : this(keyFileSearchPaths, StrongNameFileSystem.Instance)
        {
        }

        public DesktopStrongNameProvider(ImmutableArray<string> keyFileSearchPaths = default(ImmutableArray<string>), string? tempPath = null)
            : this(keyFileSearchPaths, (tempPath == null) ? StrongNameFileSystem.Instance : new StrongNameFileSystem(tempPath))
        {
        }

        internal DesktopStrongNameProvider(ImmutableArray<string> keyFileSearchPaths, StrongNameFileSystem strongNameFileSystem)
        {
            if (!keyFileSearchPaths.IsDefault && keyFileSearchPaths.Any((string path) => !PathUtilities.IsAbsolute(path)))
            {
                throw new ArgumentException(CodeAnalysisResources.AbsolutePathExpected, "keyFileSearchPaths");
            }
            FileSystem = strongNameFileSystem ?? StrongNameFileSystem.Instance;
            _keyFileSearchPaths = keyFileSearchPaths.NullToEmpty();
        }

        internal override StrongNameKeys CreateKeys(string? keyFilePath, string? keyContainerName, bool hasCounterSignature, CommonMessageProvider messageProvider)
        {
            ImmutableArray<byte> keyPair = default(ImmutableArray<byte>);
            ImmutableArray<byte> publicKey = default(ImmutableArray<byte>);
            string keyContainerName2 = null;
            if (!string.IsNullOrEmpty(keyFilePath))
            {
                try
                {
                    string text = ResolveStrongNameKeyFile(keyFilePath, FileSystem, _keyFileSearchPaths);
                    if (text == null)
                    {
                        return new StrongNameKeys(StrongNameKeys.GetKeyFileError(messageProvider, keyFilePath, CodeAnalysisResources.FileNotFound));
                    }
                    return StrongNameKeys.CreateHelper(ImmutableArray.Create(FileSystem.ReadAllBytes(text)), keyFilePath, hasCounterSignature);
                }
                catch (Exception ex)
                {
                    return new StrongNameKeys(StrongNameKeys.GetKeyFileError(messageProvider, keyFilePath, ex.Message));
                }
            }
            if (!string.IsNullOrEmpty(keyContainerName))
            {
                try
                {
                    ReadKeysFromContainer(keyContainerName, out publicKey);
                    keyContainerName2 = keyContainerName;
                }
                catch (ClrStrongNameMissingException)
                {
                    return new StrongNameKeys(StrongNameKeys.GetContainerError(messageProvider, keyContainerName, new CodeAnalysisResourcesLocalizableErrorArgument("AssemblySigningNotSupported")));
                }
                catch (Exception ex3)
                {
                    return new StrongNameKeys(StrongNameKeys.GetContainerError(messageProvider, keyContainerName, ex3.Message));
                }
            }
            return new StrongNameKeys(keyPair, publicKey, null, keyContainerName2, keyFilePath, hasCounterSignature);
        }

        internal static string? ResolveStrongNameKeyFile(string path, StrongNameFileSystem fileSystem, ImmutableArray<string> keyFileSearchPaths)
        {
            if (PathUtilities.IsAbsolute(path))
            {
                if (fileSystem.FileExists(path))
                {
                    return FileUtilities.TryNormalizeAbsolutePath(path);
                }
                return path;
            }
            ImmutableArray<string>.Enumerator enumerator = keyFileSearchPaths.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string text = PathUtilities.CombineAbsoluteAndRelativePaths(enumerator.Current, path);
                if (fileSystem.FileExists(text))
                {
                    return FileUtilities.TryNormalizeAbsolutePath(text);
                }
            }
            return null;
        }

        internal virtual void ReadKeysFromContainer(string keyContainer, out ImmutableArray<byte> publicKey)
        {
            try
            {
                publicKey = GetPublicKey(keyContainer);
            }
            catch (ClrStrongNameMissingException)
            {
                throw;
            }
            catch (Exception ex2)
            {
                throw new IOException(ex2.Message);
            }
        }

        internal override void SignFile(StrongNameKeys keys, string filePath)
        {
            if (!string.IsNullOrEmpty(keys.KeyFilePath))
            {
                Sign(filePath, keys.KeyPair);
            }
            else
            {
                Sign(filePath, keys.KeyContainer);
            }
        }

        internal override void SignBuilder(ExtendedPEBuilder peBuilder, BlobBuilder peBlob, RSAParameters privateKey)
        {
            // FilRip : not supported until we find something compatible net4.5.2
            //peBuilder.Sign(peBlob, (IEnumerable<Blob> content) => SigningUtilities.CalculateRsaSignature(content, privateKey));
        }

        internal virtual IClrStrongName GetStrongNameInterface()
        {
            try
            {
                return ClrStrongName.GetInstance();
            }
            catch (MarshalDirectiveException) when (PathUtilities.IsUnixLikePlatform)
            {
                throw new ClrStrongNameMissingException();
            }
        }

        internal ImmutableArray<byte> GetPublicKey(string keyContainer)
        {
            IClrStrongName strongNameInterface = GetStrongNameInterface();
            strongNameInterface.StrongNameGetPublicKey(keyContainer, (IntPtr)0, 0, out var ppbPublicKeyBlob, out var pcbPublicKeyBlob);
            byte[] array = new byte[pcbPublicKeyBlob];
            Marshal.Copy(ppbPublicKeyBlob, array, 0, pcbPublicKeyBlob);
            strongNameInterface.StrongNameFreeBuffer(ppbPublicKeyBlob);
            return array.AsImmutableOrNull();
        }

        private void Sign(string filePath, string keyName)
        {
            try
            {
                GetStrongNameInterface().StrongNameSignatureGeneration(filePath, keyName, IntPtr.Zero, 0, null, out var _);
            }
            catch (ClrStrongNameMissingException)
            {
                throw;
            }
            catch (Exception ex2)
            {
                throw new IOException(ex2.Message, ex2);
            }
        }

        private unsafe void Sign(string filePath, ImmutableArray<byte> keyPair)
        {
            try
            {
                IClrStrongName strongNameInterface = GetStrongNameInterface();
                fixed (byte* ptr = keyPair.ToArray())
                {
                    strongNameInterface.StrongNameSignatureGeneration(filePath, null, (IntPtr)ptr, keyPair.Length, null, out var _);
                }
            }
            catch (ClrStrongNameMissingException)
            {
                throw;
            }
            catch (Exception ex2)
            {
                throw new IOException(ex2.Message, ex2);
            }
        }

        public override int GetHashCode()
        {
            return Hash.CombineValues(_keyFileSearchPaths, StringComparer.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj!.GetType())
            {
                return false;
            }
            DesktopStrongNameProvider desktopStrongNameProvider = (DesktopStrongNameProvider)obj;
            if (FileSystem != desktopStrongNameProvider.FileSystem)
            {
                return false;
            }
            if (!_keyFileSearchPaths.SequenceEqual(desktopStrongNameProvider._keyFileSearchPaths, StringComparer.Ordinal))
            {
                return false;
            }
            return true;
        }
    }
}

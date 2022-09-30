using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection.Metadata;
using System.Security.Cryptography;

using Microsoft.CodeAnalysis.Collections;

namespace Microsoft.CodeAnalysis
{
    internal static class CryptoBlobParser
    {
        private enum AlgorithmClass
        {
            Signature = 1,
            Hash = 4
        }

        private enum AlgorithmSubId
        {
            Sha1Hash = 4,
            MacHash,
            RipeMdHash,
            RipeMd160Hash,
            Ssl3ShaMD5Hash,
            HmacHash,
            Tls1PrfHash,
            HashReplacOwfHash,
            Sha256Hash,
            Sha384Hash,
            Sha512Hash
        }

        private struct AlgorithmId
        {
            private const int AlgorithmClassOffset = 13;

            private const int AlgorithmClassMask = 7;

            private const int AlgorithmSubIdOffset = 0;

            private const int AlgorithmSubIdMask = 511;

            private readonly uint _flags;

            public const int RsaSign = 9216;

            public const int Sha = 32772;

            public bool IsSet => _flags != 0;

            public AlgorithmClass Class => (AlgorithmClass)((int)(_flags >> 13) & 7);

            public AlgorithmSubId SubId => (AlgorithmSubId)((int)_flags & 0x1FF);

            public AlgorithmId(uint flags)
            {
                _flags = flags;
            }
        }

        private static readonly ImmutableArray<byte> s_ecmaKey = ImmutableArray.Create(new byte[16]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 4, 0,
            0, 0, 0, 0, 0, 0
        });

        private const int SnPublicKeyBlobSize = 13;

        private const byte PublicKeyBlobId = 6;

        private const byte PrivateKeyBlobId = 7;

        internal const int s_publicKeyHeaderSize = 12;

        private const int BlobHeaderSize = 8;

        private const int RsaPubKeySize = 12;

        private const uint RSA1 = 826364754u;

        private const uint RSA2 = 843141970u;

        private const int s_offsetToKeyData = 20;

        internal static bool IsValidPublicKey(ImmutableArray<byte> blob)
        {
            if (blob.IsDefault || blob.Length < 13)
            {
                return false;
            }
            LittleEndianReader littleEndianReader = new LittleEndianReader(blob.AsSpan());
            uint flags = littleEndianReader.ReadUInt32();
            uint flags2 = littleEndianReader.ReadUInt32();
            uint num = littleEndianReader.ReadUInt32();
            byte b = littleEndianReader.ReadByte();
            if (blob.Length != 12 + num)
            {
                return false;
            }
            if (ByteSequenceComparer.Equals(blob, s_ecmaKey))
            {
                return true;
            }
            if (b != 6)
            {
                return false;
            }
            AlgorithmId algorithmId = new AlgorithmId(flags);
            if (algorithmId.IsSet && algorithmId.Class != AlgorithmClass.Signature)
            {
                return false;
            }
            AlgorithmId algorithmId2 = new AlgorithmId(flags2);
            if (algorithmId2.IsSet && (algorithmId2.Class != AlgorithmClass.Hash || algorithmId2.SubId < AlgorithmSubId.Sha1Hash))
            {
                return false;
            }
            return true;
        }

        private unsafe static ImmutableArray<byte> CreateSnPublicKeyBlob(byte type, byte version, uint algId, uint magic, uint bitLen, uint pubExp, ReadOnlySpan<byte> pubKeyData)
        {
            BlobWriter blobWriter = new BlobWriter(32 + pubKeyData.Length);
            blobWriter.WriteUInt32(9216u);
            blobWriter.WriteUInt32(32772u);
            blobWriter.WriteUInt32((uint)(20 + pubKeyData.Length));
            blobWriter.WriteByte(type);
            blobWriter.WriteByte(version);
            blobWriter.WriteUInt16(0);
            blobWriter.WriteUInt32(algId);
            blobWriter.WriteUInt32(magic);
            blobWriter.WriteUInt32(bitLen);
            blobWriter.WriteUInt32(pubExp);
            fixed (byte* buffer = pubKeyData)
            {
                blobWriter.WriteBytes(buffer, pubKeyData.Length);
            }
            return blobWriter.ToImmutableArray();
        }

        public static bool TryParseKey(ImmutableArray<byte> blob, out ImmutableArray<byte> snKey, out RSAParameters? privateKey)
        {
            privateKey = null;
            snKey = default(ImmutableArray<byte>);
            if (IsValidPublicKey(blob))
            {
                snKey = blob;
                return true;
            }
            if (blob.Length < 20)
            {
                return false;
            }
            try
            {
                LittleEndianReader littleEndianReader = new LittleEndianReader(blob.AsSpan());
                byte b = littleEndianReader.ReadByte();
                byte version = littleEndianReader.ReadByte();
                littleEndianReader.ReadUInt16();
                uint algId = littleEndianReader.ReadUInt32();
                uint num = littleEndianReader.ReadUInt32();
                uint num2 = littleEndianReader.ReadUInt32();
                uint pubExp = littleEndianReader.ReadUInt32();
                int num3 = (int)(num2 / 8u);
                if (blob.Length - 20 < num3)
                {
                    return false;
                }
                ReadOnlySpan<byte> pubKeyData = littleEndianReader.ReadBytes(num3);
                if ((b != 7 || num != 843141970) && (b != 6 || num != 826364754))
                {
                    return false;
                }
                if (b == 7)
                {
                    privateKey = blob.AsSpan().ToRSAParameters(includePrivateParameters: true);
                    algId = 9216u;
                    num = 826364754u;
                }
                snKey = CreateSnPublicKeyBlob(6, version, algId, 826364754u, num2, pubExp, pubKeyData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static RSAParameters ToRSAParameters(this ReadOnlySpan<byte> cspBlob, bool includePrivateParameters)
        {
            LittleEndianReader littleEndianReader = new LittleEndianReader(cspBlob);
            littleEndianReader.ReadByte();
            littleEndianReader.ReadByte();
            littleEndianReader.ReadUInt16();
            littleEndianReader.ReadInt32();
            littleEndianReader.ReadInt32();
            int num = littleEndianReader.ReadInt32() / 8;
            int byteCount = (num + 1) / 2;
            uint exponent = littleEndianReader.ReadUInt32();
            RSAParameters result = default(RSAParameters);
            result.Exponent = ExponentAsBytes(exponent);
            result.Modulus = littleEndianReader.ReadReversed(num);
            if (includePrivateParameters)
            {
                result.P = littleEndianReader.ReadReversed(byteCount);
                result.Q = littleEndianReader.ReadReversed(byteCount);
                result.DP = littleEndianReader.ReadReversed(byteCount);
                result.DQ = littleEndianReader.ReadReversed(byteCount);
                result.InverseQ = littleEndianReader.ReadReversed(byteCount);
                result.D = littleEndianReader.ReadReversed(num);
            }
            return result;
        }

        private static byte[] ExponentAsBytes(uint exponent)
        {
            if (exponent > 255)
            {
                if (exponent > 65535)
                {
                    if (exponent > 16777215)
                    {
                        return new byte[4]
                        {
                            (byte)(exponent >> 24),
                            (byte)(exponent >> 16),
                            (byte)(exponent >> 8),
                            (byte)exponent
                        };
                    }
                    return new byte[3]
                    {
                        (byte)(exponent >> 16),
                        (byte)(exponent >> 8),
                        (byte)exponent
                    };
                }
                return new byte[2]
                {
                    (byte)(exponent >> 8),
                    (byte)exponent
                };
            }
            return new byte[1] { (byte)exponent };
        }

        private static byte[] ReadReversed(this BinaryReader br, int count)
        {
            byte[] array = br.ReadBytes(count);
            Array.Reverse(array);
            return array;
        }
    }
}

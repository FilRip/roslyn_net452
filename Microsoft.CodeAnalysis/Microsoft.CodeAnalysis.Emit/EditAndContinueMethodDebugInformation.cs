using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.Emit
{
    public readonly struct EditAndContinueMethodDebugInformation
    {
        internal readonly int MethodOrdinal;

        public readonly ImmutableArray<LocalSlotDebugInfo> LocalSlots;

        internal readonly ImmutableArray<LambdaDebugInfo> Lambdas;

        internal readonly ImmutableArray<ClosureDebugInfo> Closures;

        private const byte SyntaxOffsetBaseline = byte.MaxValue;

        internal EditAndContinueMethodDebugInformation(int methodOrdinal, ImmutableArray<LocalSlotDebugInfo> localSlots, ImmutableArray<ClosureDebugInfo> closures, ImmutableArray<LambdaDebugInfo> lambdas)
        {
            MethodOrdinal = methodOrdinal;
            LocalSlots = localSlots;
            Lambdas = lambdas;
            Closures = closures;
        }

        public static EditAndContinueMethodDebugInformation Create(ImmutableArray<byte> compressedSlotMap, ImmutableArray<byte> compressedLambdaMap)
        {
            UncompressLambdaMap(compressedLambdaMap, out var methodOrdinal, out var closures, out var lambdas);
            return new EditAndContinueMethodDebugInformation(methodOrdinal, UncompressSlotMap(compressedSlotMap), closures, lambdas);
        }

        private static InvalidDataException CreateInvalidDataException(ImmutableArray<byte> data, int offset)
        {
            int num = Math.Max(0, offset - 512);
            int num2 = Math.Min(data.Length, offset + 512);
            byte[] array = new byte[offset - num];
            data.CopyTo(num, array, 0, array.Length);
            byte[] array2 = new byte[num2 - offset];
            data.CopyTo(offset, array2, 0, array2.Length);
            throw new InvalidDataException(string.Format(CodeAnalysisResources.InvalidDataAtOffset, offset, (num != 0) ? "..." : "", BitConverter.ToString(array), BitConverter.ToString(array2), (num2 != data.Length) ? "..." : ""));
        }

        private unsafe static ImmutableArray<LocalSlotDebugInfo> UncompressSlotMap(ImmutableArray<byte> compressedSlotMap)
        {
            if (compressedSlotMap.IsDefaultOrEmpty)
            {
                return default(ImmutableArray<LocalSlotDebugInfo>);
            }
            ArrayBuilder<LocalSlotDebugInfo> instance = ArrayBuilder<LocalSlotDebugInfo>.GetInstance();
            int num = -1;
            fixed (byte* buffer = &compressedSlotMap.ToArray()[0])
            {
                BlobReader blobReader = new BlobReader(buffer, compressedSlotMap.Length);
                while (blobReader.RemainingBytes > 0)
                {
                    try
                    {
                        byte b = blobReader.ReadByte();
                        switch (b)
                        {
                            case byte.MaxValue:
                                num = -blobReader.ReadCompressedInteger();
                                continue;
                            case 0:
                                instance.Add(new LocalSlotDebugInfo(SynthesizedLocalKind.LoweringTemp, default(LocalDebugId)));
                                continue;
                        }
                        SynthesizedLocalKind synthesizedKind = (SynthesizedLocalKind)((b & 0x3F) - 1);
                        bool num2 = (b & 0x80) != 0;
                        int syntaxOffset = blobReader.ReadCompressedInteger() + num;
                        int ordinal = (num2 ? blobReader.ReadCompressedInteger() : 0);
                        instance.Add(new LocalSlotDebugInfo(synthesizedKind, new LocalDebugId(syntaxOffset, ordinal)));
                    }
                    catch (BadImageFormatException)
                    {
                        throw CreateInvalidDataException(compressedSlotMap, blobReader.Offset);
                    }
                }
            }
            return instance.ToImmutableAndFree();
        }

        internal void SerializeLocalSlots(BlobBuilder writer)
        {
            int num = -1;
            ImmutableArray<LocalSlotDebugInfo>.Enumerator enumerator = LocalSlots.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSlotDebugInfo current = enumerator.Current;
                if (current.Id.SyntaxOffset < num)
                {
                    num = current.Id.SyntaxOffset;
                }
            }
            if (num != -1)
            {
                writer.WriteByte(byte.MaxValue);
                writer.WriteCompressedInteger(-num);
            }
            enumerator = LocalSlots.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSlotDebugInfo current2 = enumerator.Current;
                SynthesizedLocalKind synthesizedKind = current2.SynthesizedKind;
                if (!synthesizedKind.IsLongLived())
                {
                    writer.WriteByte(0);
                    continue;
                }
                byte b = (byte)(synthesizedKind + 1);
                bool num2 = current2.Id.Ordinal > 0;
                if (num2)
                {
                    b = (byte)(b | 0x80u);
                }
                writer.WriteByte(b);
                writer.WriteCompressedInteger(current2.Id.SyntaxOffset - num);
                if (num2)
                {
                    writer.WriteCompressedInteger(current2.Id.Ordinal);
                }
            }
        }

        private unsafe static void UncompressLambdaMap(ImmutableArray<byte> compressedLambdaMap, out int methodOrdinal, out ImmutableArray<ClosureDebugInfo> closures, out ImmutableArray<LambdaDebugInfo> lambdas)
        {
            methodOrdinal = -1;
            closures = default(ImmutableArray<ClosureDebugInfo>);
            lambdas = default(ImmutableArray<LambdaDebugInfo>);
            if (compressedLambdaMap.IsDefaultOrEmpty)
            {
                return;
            }
            ArrayBuilder<ClosureDebugInfo> instance = ArrayBuilder<ClosureDebugInfo>.GetInstance();
            ArrayBuilder<LambdaDebugInfo> instance2 = ArrayBuilder<LambdaDebugInfo>.GetInstance();
            fixed (byte* buffer = &compressedLambdaMap.ToArray()[0])
            {
                BlobReader blobReader = new BlobReader(buffer, compressedLambdaMap.Length);
                try
                {
                    methodOrdinal = blobReader.ReadCompressedInteger() - 1;
                    int num = -blobReader.ReadCompressedInteger();
                    int num2 = blobReader.ReadCompressedInteger();
                    for (int i = 0; i < num2; i++)
                    {
                        int num3 = blobReader.ReadCompressedInteger();
                        DebugId closureId = new DebugId(instance.Count, 0);
                        instance.Add(new ClosureDebugInfo(num3 + num, closureId));
                    }
                    while (blobReader.RemainingBytes > 0)
                    {
                        int num4 = blobReader.ReadCompressedInteger();
                        int num5 = blobReader.ReadCompressedInteger() + -2;
                        if (num5 >= num2)
                        {
                            throw CreateInvalidDataException(compressedLambdaMap, blobReader.Offset);
                        }
                        DebugId lambdaId = new DebugId(instance2.Count, 0);
                        instance2.Add(new LambdaDebugInfo(num4 + num, lambdaId, num5));
                    }
                }
                catch (BadImageFormatException)
                {
                    throw CreateInvalidDataException(compressedLambdaMap, blobReader.Offset);
                }
            }
            closures = instance.ToImmutableAndFree();
            lambdas = instance2.ToImmutableAndFree();
        }

        internal void SerializeLambdaMap(BlobBuilder writer)
        {
            writer.WriteCompressedInteger(MethodOrdinal + 1);
            int num = -1;
            ImmutableArray<ClosureDebugInfo>.Enumerator enumerator = Closures.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ClosureDebugInfo current = enumerator.Current;
                if (current.SyntaxOffset < num)
                {
                    num = current.SyntaxOffset;
                }
            }
            ImmutableArray<LambdaDebugInfo>.Enumerator enumerator2 = Lambdas.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                LambdaDebugInfo current2 = enumerator2.Current;
                if (current2.SyntaxOffset < num)
                {
                    num = current2.SyntaxOffset;
                }
            }
            writer.WriteCompressedInteger(-num);
            writer.WriteCompressedInteger(Closures.Length);
            enumerator = Closures.GetEnumerator();
            while (enumerator.MoveNext())
            {
                writer.WriteCompressedInteger(enumerator.Current.SyntaxOffset - num);
            }
            enumerator2 = Lambdas.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                LambdaDebugInfo current3 = enumerator2.Current;
                writer.WriteCompressedInteger(current3.SyntaxOffset - num);
                writer.WriteCompressedInteger(current3.ClosureOrdinal - -2);
            }
        }
    }
}

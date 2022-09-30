using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Microsoft.CodeAnalysis.Debugging
{
    internal struct CustomDebugInfoEncoder
    {
        private readonly Blob _recordCountFixup;

        private int _recordCount;

        internal const int DynamicAttributeSize = 64;

        internal const int IdentifierSize = 64;

        public BlobBuilder Builder { get; }

        public int RecordCount => _recordCount;

        public CustomDebugInfoEncoder(BlobBuilder builder)
        {
            Builder = builder;
            _recordCount = 0;
            builder.WriteByte(4);
            _recordCountFixup = builder.ReserveBytes(1);
            builder.WriteInt16(0);
        }

        public byte[] ToArray()
        {
            if (_recordCount == 0)
            {
                return null;
            }
            new BlobWriter(_recordCountFixup).WriteByte((byte)_recordCount);
            return Builder.ToArray();
        }

        public void AddStateMachineTypeName(string typeName)
        {
            AddRecord(CustomDebugInfoKind.StateMachineTypeName, typeName, delegate (string name, BlobBuilder builder)
            {
                builder.WriteUTF16(name);
                builder.WriteInt16(0);
            });
        }

        public void AddForwardMethodInfo(MethodDefinitionHandle methodHandle)
        {
            AddRecord(CustomDebugInfoKind.ForwardMethodInfo, methodHandle, delegate (MethodDefinitionHandle mh, BlobBuilder builder)
            {
                builder.WriteInt32(MetadataTokens.GetToken(mh));
            });
        }

        public void AddForwardModuleInfo(MethodDefinitionHandle methodHandle)
        {
            AddRecord(CustomDebugInfoKind.ForwardModuleInfo, methodHandle, delegate (MethodDefinitionHandle mh, BlobBuilder builder)
            {
                builder.WriteInt32(MetadataTokens.GetToken(mh));
            });
        }

        public void AddUsingGroups(IReadOnlyCollection<int> groupSizes)
        {
            if (groupSizes.Count == 0)
            {
                return;
            }
            AddRecord(CustomDebugInfoKind.UsingGroups, groupSizes, delegate (IReadOnlyCollection<int> uc, BlobBuilder builder)
            {
                builder.WriteUInt16((ushort)uc.Count);
                foreach (int item in uc)
                {
                    builder.WriteUInt16((ushort)item);
                }
            });
        }

        public void AddStateMachineHoistedLocalScopes(ImmutableArray<StateMachineHoistedLocalScope> scopes)
        {
            if (scopes.IsDefaultOrEmpty)
            {
                return;
            }
            AddRecord(CustomDebugInfoKind.StateMachineHoistedLocalScopes, scopes, delegate (ImmutableArray<StateMachineHoistedLocalScope> s, BlobBuilder builder)
            {
                builder.WriteInt32(s.Length);
                ImmutableArray<StateMachineHoistedLocalScope>.Enumerator enumerator = s.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    StateMachineHoistedLocalScope current = enumerator.Current;
                    if (current.IsDefault)
                    {
                        builder.WriteInt32(0);
                        builder.WriteInt32(0);
                    }
                    else
                    {
                        builder.WriteInt32(current.StartOffset);
                        builder.WriteInt32(current.EndOffset - 1);
                    }
                }
            });
        }

        public void AddDynamicLocals(IReadOnlyCollection<(string LocalName, byte[] Flags, int Count, int SlotIndex)> dynamicLocals)
        {
            AddRecord(CustomDebugInfoKind.DynamicLocals, dynamicLocals, delegate (IReadOnlyCollection<(string LocalName, byte[] Flags, int Count, int SlotIndex)> infos, BlobBuilder builder)
            {
                builder.WriteInt32(infos.Count);
                foreach (var info in infos)
                {
                    builder.WriteBytes(info.Flags);
                    builder.WriteBytes(0, 64 - info.Flags.Length);
                    builder.WriteInt32(info.Count);
                    builder.WriteInt32(info.SlotIndex);
                    builder.WriteUTF16(info.LocalName);
                    builder.WriteBytes(0, 2 * (64 - info.LocalName.Length));
                }
            });
        }

        public void AddTupleElementNames(IReadOnlyCollection<(string LocalName, int SlotIndex, int ScopeStart, int ScopeEnd, ImmutableArray<string> Names)> tupleLocals)
        {
            AddRecord(CustomDebugInfoKind.TupleElementNames, tupleLocals, delegate (IReadOnlyCollection<(string LocalName, int SlotIndex, int ScopeStart, int ScopeEnd, ImmutableArray<string> Names)> infos, BlobBuilder builder)
            {
                builder.WriteInt32(infos.Count);
                foreach (var info in infos)
                {
                    ImmutableArray<string> item = info.Names;
                    builder.WriteInt32(item.Length);
                    item = info.Names;
                    ImmutableArray<string>.Enumerator enumerator2 = item.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        string current2 = enumerator2.Current;
                        if (current2 != null)
                        {
                            builder.WriteUTF8(current2);
                        }
                        builder.WriteByte(0);
                    }
                    builder.WriteInt32(info.SlotIndex);
                    builder.WriteInt32(info.ScopeStart);
                    builder.WriteInt32(info.ScopeEnd);
                    if (info.LocalName != null)
                    {
                        builder.WriteUTF8(info.LocalName);
                    }
                    builder.WriteByte(0);
                }
            });
        }

        public void AddRecord<T>(CustomDebugInfoKind kind, T debugInfo, Action<T, BlobBuilder> recordSerializer)
        {
            int count = Builder.Count;
            Builder.WriteByte(4);
            Builder.WriteByte((byte)kind);
            Builder.WriteByte(0);
            BlobWriter blobWriter = new BlobWriter(Builder.ReserveBytes(5));
            recordSerializer(debugInfo, Builder);
            int num = Builder.Count - count;
            int num2 = 4 * ((num + 3) / 4);
            byte b = (byte)(num2 - num);
            Builder.WriteBytes(0, b);
            blobWriter.WriteByte((byte)(((int)kind > 5) ? b : 0));
            blobWriter.WriteUInt32((uint)num2);
            _recordCount++;
        }
    }
}

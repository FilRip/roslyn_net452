using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;

using Roslyn.Utilities;

namespace Microsoft.Cci
{
    internal static class NativeResourceWriter
    {
        private class Directory
        {
            internal readonly string Name;

            internal readonly int ID;

            internal ushort NumberOfNamedEntries;

            internal ushort NumberOfIdEntries;

            internal readonly List<object> Entries;

            internal Directory(string name, int id)
            {
                Name = name;
                ID = id;
                Entries = new List<object>();
            }
        }

        private static int CompareResources(IWin32Resource left, IWin32Resource right)
        {
            int num = CompareResourceIdentifiers(left.TypeId, left.TypeName, right.TypeId, right.TypeName);
            if (num != 0)
            {
                return num;
            }
            return CompareResourceIdentifiers(left.Id, left.Name, right.Id, right.Name);
        }

        private static int CompareResourceIdentifiers(int xOrdinal, string xString, int yOrdinal, string yString)
        {
            if (xString == null)
            {
                if (yString == null)
                {
                    return xOrdinal - yOrdinal;
                }
                return 1;
            }
            if (yString == null)
            {
                return -1;
            }
            return string.Compare(xString, yString, StringComparison.OrdinalIgnoreCase);
        }

        internal static IEnumerable<IWin32Resource> SortResources(IEnumerable<IWin32Resource> resources)
        {
            return resources.OrderBy(CompareResources);
        }

        public static void SerializeWin32Resources(BlobBuilder builder, IEnumerable<IWin32Resource> theResources, int resourcesRva)
        {
            theResources = SortResources(theResources);

            Directory typeDirectory = new Directory(string.Empty, 0);
            Directory nameDirectory = null;
            Directory languageDirectory = null;
            int lastTypeID = int.MinValue;
            string lastTypeName = null;
            int lastID = int.MinValue;
            string lastName = null;
            uint sizeOfDirectoryTree = 16;

            //EDMAURER note that this list is assumed to be sorted lowest to highest 
            //first by typeId, then by Id.
            foreach (IWin32Resource r in theResources)
            {
                bool typeDifferent = (r.TypeId < 0 && r.TypeName != lastTypeName) || r.TypeId > lastTypeID;
                if (typeDifferent)
                {
                    lastTypeID = r.TypeId;
                    lastTypeName = r.TypeName;
                    if (lastTypeID < 0)
                    {
                        typeDirectory.NumberOfNamedEntries++;
                    }
                    else
                    {
                        typeDirectory.NumberOfIdEntries++;
                    }

                    sizeOfDirectoryTree += 24;
                    typeDirectory.Entries.Add(nameDirectory = new Directory(lastTypeName, lastTypeID));
                }

                if (typeDifferent || (r.Id < 0 && r.Name != lastName) || r.Id > lastID)
                {
                    lastID = r.Id;
                    lastName = r.Name;
                    if (lastID < 0)
                    {
                        nameDirectory.NumberOfNamedEntries++;
                    }
                    else
                    {
                        nameDirectory.NumberOfIdEntries++;
                    }

                    sizeOfDirectoryTree += 24;
                    nameDirectory.Entries.Add(languageDirectory = new Directory(lastName, lastID));
                }

                languageDirectory.NumberOfIdEntries++;
                sizeOfDirectoryTree += 8;
                languageDirectory.Entries.Add(r);
            }

            var dataWriter = new BlobBuilder();

            //'dataWriter' is where opaque resource data goes as well as strings that are used as type or name identifiers
            WriteDirectory(typeDirectory, builder, 0, 0, sizeOfDirectoryTree, resourcesRva, dataWriter);
            builder.LinkSuffix(dataWriter);
            builder.WriteByte(0);
            builder.Align(4);
        }

        private static void WriteDirectory(Directory directory, BlobBuilder writer, uint offset, uint level, uint sizeOfDirectoryTree, int virtualAddressBase, BlobBuilder dataWriter)
        {
            writer.WriteUInt32(0u);
            writer.WriteUInt32(0u);
            writer.WriteUInt32(0u);
            writer.WriteUInt16(directory.NumberOfNamedEntries);
            writer.WriteUInt16(directory.NumberOfIdEntries);
            uint count = (uint)directory.Entries.Count;
            uint num = offset + 16 + count * 8;
            for (int i = 0; i < count; i++)
            {
                uint num2 = (uint)dataWriter.Count + sizeOfDirectoryTree;
                uint num3 = num;
                Directory directory2 = directory.Entries[i] as Directory;
                int num4;
                string text;
                if (directory2 != null)
                {
                    num4 = directory2.ID;
                    text = directory2.Name;
                    num = ((level != 0) ? (num + (uint)(16 + 8 * directory2.Entries.Count)) : (num + SizeOfDirectory(directory2)));
                }
                else
                {
                    IWin32Resource win32Resource = (IWin32Resource)directory.Entries[i];
                    num4 = level switch
                    {
                        1u => win32Resource.Id,
                        0u => win32Resource.TypeId,
                        _ => (int)win32Resource.LanguageId,
                    };
                    text = level switch
                    {
                        1u => win32Resource.Name,
                        0u => win32Resource.TypeName,
                        _ => null,
                    };
                    dataWriter.WriteUInt32((uint)(virtualAddressBase + sizeOfDirectoryTree + 16 + dataWriter.Count));
                    byte[] array = new List<byte>(win32Resource.Data).ToArray();
                    dataWriter.WriteUInt32((uint)array.Length);
                    dataWriter.WriteUInt32(win32Resource.CodePage);
                    dataWriter.WriteUInt32(0u);
                    dataWriter.WriteBytes(array);
                    while (dataWriter.Count % 4 != 0)
                    {
                        dataWriter.WriteByte(0);
                    }
                }
                if (num4 >= 0)
                {
                    writer.WriteInt32(num4);
                }
                else
                {
                    if (text == null)
                    {
                        text = string.Empty;
                    }
                    writer.WriteUInt32(num2 | 0x80000000u);
                    dataWriter.WriteUInt16((ushort)text.Length);
                    dataWriter.WriteUTF16(text);
                }
                if (directory2 != null)
                {
                    writer.WriteUInt32(num3 | 0x80000000u);
                }
                else
                {
                    writer.WriteUInt32(num2);
                }
            }
            num = offset + 16 + count * 8;
            for (int j = 0; j < count; j++)
            {
                if (directory.Entries[j] is Directory directory3)
                {
                    WriteDirectory(directory3, writer, num, level + 1, sizeOfDirectoryTree, virtualAddressBase, dataWriter);
                    num = ((level != 0) ? (num + (uint)(16 + 8 * directory3.Entries.Count)) : (num + SizeOfDirectory(directory3)));
                }
            }
        }

        private static uint SizeOfDirectory(Directory directory)
        {
            uint count = (uint)directory.Entries.Count;
            uint num = 16 + 8 * count;
            for (int i = 0; i < count; i++)
            {
                if (directory.Entries[i] is Directory directory2)
                {
                    num += (uint)(16 + 8 * directory2.Entries.Count);
                }
            }
            return num;
        }

        public static void SerializeWin32Resources(BlobBuilder builder, ResourceSection resourceSections, int resourcesRva)
        {
            BlobWriter blobWriter = new BlobWriter(builder.ReserveBytes(resourceSections.SectionBytes.Length));
            blobWriter.WriteBytes(resourceSections.SectionBytes);
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(resourceSections.SectionBytes));
            uint[] relocations = resourceSections.Relocations;
            for (int i = 0; i < relocations.Length; i++)
            {
                int num2 = (blobWriter.Offset = (int)relocations[i]);
                binaryReader.BaseStream.Position = num2;
                blobWriter.WriteUInt32(binaryReader.ReadUInt32() + (uint)resourcesRva);
            }
        }
    }
}

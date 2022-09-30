using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslyn.Utilities
{
    public readonly struct ObjectBinderSnapshot
    {
        private readonly Dictionary<Type, int> _typeToIndex;

        private readonly ImmutableArray<Type> _types;

        private readonly ImmutableArray<Func<ObjectReader, IObjectWritable>> _typeReaders;

        public ObjectBinderSnapshot(Dictionary<Type, int> typeToIndex, List<Type> types, List<Func<ObjectReader, IObjectWritable>> typeReaders)
        {
            _typeToIndex = new Dictionary<Type, int>(typeToIndex);
            _types = types.ToImmutableArray();
            _typeReaders = typeReaders.ToImmutableArray();
        }

        public int GetTypeId(Type type)
        {
            return _typeToIndex[type];
        }

        public Type GetTypeFromId(int typeId)
        {
            return _types[typeId];
        }

        public Func<ObjectReader, IObjectWritable> GetTypeReaderFromId(int typeId)
        {
            return _typeReaders[typeId];
        }
    }
}

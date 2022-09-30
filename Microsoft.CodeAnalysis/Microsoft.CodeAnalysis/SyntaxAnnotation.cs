using System;
using System.Diagnostics;
using System.Threading;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class SyntaxAnnotation : IObjectWritable, IEquatable<SyntaxAnnotation?>
    {
        private readonly long _id;

        private static long s_nextId;

        public static SyntaxAnnotation ElasticAnnotation { get; }

        public string? Kind { get; }

        public string? Data { get; }

        bool IObjectWritable.ShouldReuseInSerialization => true;

        static SyntaxAnnotation()
        {
            ElasticAnnotation = new SyntaxAnnotation();
            ObjectBinder.RegisterTypeReader(typeof(SyntaxAnnotation), (ObjectReader r) => new SyntaxAnnotation(r));
        }

        public SyntaxAnnotation()
        {
            _id = Interlocked.Increment(ref s_nextId);
        }

        public SyntaxAnnotation(string? kind)
            : this()
        {
            Kind = kind;
        }

        public SyntaxAnnotation(string? kind, string? data)
            : this(kind)
        {
            Data = data;
        }

        private SyntaxAnnotation(ObjectReader reader)
        {
            _id = reader.ReadInt64();
            Kind = reader.ReadString();
            Data = reader.ReadString();
        }

        void IObjectWritable.WriteTo(ObjectWriter writer)
        {
            writer.WriteInt64(_id);
            writer.WriteString(Kind);
            writer.WriteString(Data);
        }

        private string GetDebuggerDisplay()
        {
            return string.Format("Annotation: Kind='{0}' Data='{1}'", Kind ?? "", Data ?? "");
        }

        public bool Equals(SyntaxAnnotation? other)
        {
            if ((object)other != null)
            {
                return _id == other!._id;
            }
            return false;
        }

        public static bool operator ==(SyntaxAnnotation? left, SyntaxAnnotation? right)
        {
            return left?.Equals(right) ?? ((object)right == null);
        }

        public static bool operator !=(SyntaxAnnotation? left, SyntaxAnnotation? right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as SyntaxAnnotation);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}

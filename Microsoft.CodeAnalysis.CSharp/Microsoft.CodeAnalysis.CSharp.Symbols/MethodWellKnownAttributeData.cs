using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class MethodWellKnownAttributeData : CommonMethodWellKnownAttributeData, ISkipLocalsInitAttributeTarget, IMemberNotNullAttributeTarget
    {
        private bool _hasDoesNotReturnAttribute;

        private bool _hasSkipLocalsInitAttribute;

        private ImmutableArray<string> _memberNotNullAttributeData = ImmutableArray<string>.Empty;

        private ImmutableArray<string> _memberNotNullWhenTrueAttributeData = ImmutableArray<string>.Empty;

        private ImmutableArray<string> _memberNotNullWhenFalseAttributeData = ImmutableArray<string>.Empty;

        private UnmanagedCallersOnlyAttributeData? _unmanagedCallersOnlyAttributeData;

        public bool HasDoesNotReturnAttribute
        {
            get
            {
                return _hasDoesNotReturnAttribute;
            }
            set
            {
                _hasDoesNotReturnAttribute = value;
            }
        }

        public bool HasSkipLocalsInitAttribute
        {
            get
            {
                return _hasSkipLocalsInitAttribute;
            }
            set
            {
                _hasSkipLocalsInitAttribute = value;
            }
        }

        public ImmutableArray<string> NotNullMembers => _memberNotNullAttributeData;

        public ImmutableArray<string> NotNullWhenTrueMembers => _memberNotNullWhenTrueAttributeData;

        public ImmutableArray<string> NotNullWhenFalseMembers => _memberNotNullWhenFalseAttributeData;

        public UnmanagedCallersOnlyAttributeData? UnmanagedCallersOnlyAttributeData
        {
            get
            {
                return _unmanagedCallersOnlyAttributeData;
            }
            set
            {
                _unmanagedCallersOnlyAttributeData = value;
            }
        }

        public void AddNotNullMember(string memberName)
        {
            _memberNotNullAttributeData = _memberNotNullAttributeData.Add(memberName);
        }

        public void AddNotNullMember(ArrayBuilder<string> memberNames)
        {
            _memberNotNullAttributeData = _memberNotNullAttributeData.AddRange(memberNames);
        }

        public void AddNotNullWhenMember(bool sense, string memberName)
        {
            if (sense)
            {
                _memberNotNullWhenTrueAttributeData = _memberNotNullWhenTrueAttributeData.Add(memberName);
            }
            else
            {
                _memberNotNullWhenFalseAttributeData = _memberNotNullWhenFalseAttributeData.Add(memberName);
            }
        }

        public void AddNotNullWhenMember(bool sense, ArrayBuilder<string> memberNames)
        {
            if (sense)
            {
                _memberNotNullWhenTrueAttributeData = _memberNotNullWhenTrueAttributeData.AddRange(memberNames);
            }
            else
            {
                _memberNotNullWhenFalseAttributeData = _memberNotNullWhenFalseAttributeData.AddRange(memberNames);
            }
        }
    }
}

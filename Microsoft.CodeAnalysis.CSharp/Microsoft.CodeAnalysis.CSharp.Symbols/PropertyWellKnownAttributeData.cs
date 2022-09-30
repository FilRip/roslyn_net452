using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class PropertyWellKnownAttributeData : CommonPropertyWellKnownAttributeData, ISkipLocalsInitAttributeTarget, IMemberNotNullAttributeTarget
    {
        private bool _hasDisallowNullAttribute;

        private bool _hasAllowNullAttribute;

        private bool _hasMaybeNullAttribute;

        private bool _hasNotNullAttribute;

        private bool _hasSkipLocalsInitAttribute;

        private ImmutableArray<string> _memberNotNullAttributeData = ImmutableArray<string>.Empty;

        private ImmutableArray<string> _memberNotNullWhenTrueAttributeData = ImmutableArray<string>.Empty;

        private ImmutableArray<string> _memberNotNullWhenFalseAttributeData = ImmutableArray<string>.Empty;

        public bool HasDisallowNullAttribute
        {
            get
            {
                return _hasDisallowNullAttribute;
            }
            set
            {
                _hasDisallowNullAttribute = value;
            }
        }

        public bool HasAllowNullAttribute
        {
            get
            {
                return _hasAllowNullAttribute;
            }
            set
            {
                _hasAllowNullAttribute = value;
            }
        }

        public bool HasMaybeNullAttribute
        {
            get
            {
                return _hasMaybeNullAttribute;
            }
            set
            {
                _hasMaybeNullAttribute = value;
            }
        }

        public bool HasNotNullAttribute
        {
            get
            {
                return _hasNotNullAttribute;
            }
            set
            {
                _hasNotNullAttribute = value;
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

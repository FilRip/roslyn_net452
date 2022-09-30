namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct MemberModifiers
	{
		private readonly SourceMemberFlags _foundFlags;

		private readonly SourceMemberFlags _computedFlags;

		public SourceMemberFlags FoundFlags => _foundFlags;

		public SourceMemberFlags ComputedFlags => _computedFlags;

		public SourceMemberFlags AllFlags => _foundFlags | _computedFlags;

		public MemberModifiers(SourceMemberFlags foundFlags, SourceMemberFlags computedFlags)
		{
			this = default(MemberModifiers);
			_foundFlags = foundFlags;
			_computedFlags = computedFlags;
		}
	}
}

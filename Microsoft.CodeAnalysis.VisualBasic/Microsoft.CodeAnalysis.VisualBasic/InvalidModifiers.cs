using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class InvalidModifiers
	{
		public static SyntaxKind[] InvalidModifiersInNotInheritableClass = new SyntaxKind[3]
		{
			SyntaxKind.OverridableKeyword,
			SyntaxKind.NotOverridableKeyword,
			SyntaxKind.MustOverrideKeyword
		};

		public static SyntaxKind[] InvalidModifiersInNotInheritableOtherPartialClass = new SyntaxKind[1] { SyntaxKind.MustOverrideKeyword };

		public static SyntaxKind[] InvalidModifiersInModule = new SyntaxKind[8]
		{
			SyntaxKind.SharedKeyword,
			SyntaxKind.ProtectedKeyword,
			SyntaxKind.DefaultKeyword,
			SyntaxKind.MustOverrideKeyword,
			SyntaxKind.OverridableKeyword,
			SyntaxKind.ShadowsKeyword,
			SyntaxKind.OverridesKeyword,
			SyntaxKind.NotOverridableKeyword
		};

		public static SyntaxKind[] InvalidModifiersInInterface = new SyntaxKind[21]
		{
			SyntaxKind.PublicKeyword,
			SyntaxKind.PrivateKeyword,
			SyntaxKind.ProtectedKeyword,
			SyntaxKind.FriendKeyword,
			SyntaxKind.StaticKeyword,
			SyntaxKind.SharedKeyword,
			SyntaxKind.MustInheritKeyword,
			SyntaxKind.NotInheritableKeyword,
			SyntaxKind.OverridesKeyword,
			SyntaxKind.PartialKeyword,
			SyntaxKind.NotOverridableKeyword,
			SyntaxKind.OverridableKeyword,
			SyntaxKind.MustOverrideKeyword,
			SyntaxKind.DimKeyword,
			SyntaxKind.ConstKeyword,
			SyntaxKind.WithEventsKeyword,
			SyntaxKind.WideningKeyword,
			SyntaxKind.NarrowingKeyword,
			SyntaxKind.CustomKeyword,
			SyntaxKind.AsyncKeyword,
			SyntaxKind.IteratorKeyword
		};

		public static SyntaxKind[] InvalidModifiersIfShared = new SyntaxKind[5]
		{
			SyntaxKind.OverridesKeyword,
			SyntaxKind.OverridableKeyword,
			SyntaxKind.MustOverrideKeyword,
			SyntaxKind.NotOverridableKeyword,
			SyntaxKind.DefaultKeyword
		};

		public static SyntaxKind[] InvalidModifiersIfDefault = new SyntaxKind[1] { SyntaxKind.PrivateKeyword };

		public static SyntaxKind[] InvalidAsyncIterator = new SyntaxKind[2]
		{
			SyntaxKind.AsyncKeyword,
			SyntaxKind.IteratorKeyword
		};
	}
}

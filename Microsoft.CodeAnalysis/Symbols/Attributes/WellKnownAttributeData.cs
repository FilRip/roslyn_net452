// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Base class for storing information decoded from well-known custom attributes.
    /// </summary>
    public abstract class WellKnownAttributeData
    {
        /// <summary>
        /// Used to distinguish cases when attribute is applied with null value and when attribute is not applied.
        /// For some well-known attributes, the latter case will return string stored in <see cref="StringMissingValue"/>
        /// field.
        /// </summary>
        public static readonly string StringMissingValue = nameof(StringMissingValue);

#if DEBUG
        private bool _isSealed;
        private bool _anyDataStored;
#endif

        public WellKnownAttributeData()
        {
#if DEBUG
            _isSealed = false;
            _anyDataStored = false;
#endif
        }

        [Conditional("DEBUG")]
        protected void VerifySealed(bool expected = true)
        {
#if DEBUG
            Debug.Assert(_isSealed == expected);
#endif
        }

        [Conditional("DEBUG")]
        internal void VerifyDataStored(bool expected = true)
        {
#if DEBUG
            Debug.Assert(_anyDataStored == expected);
#endif
        }

        [Conditional("DEBUG")]
        protected void SetDataStored()
        {
#if DEBUG
            _anyDataStored = true;
#endif
        }

        [Conditional("DEBUG")]
        public static void Seal(WellKnownAttributeData data)
        {
#if DEBUG
            if (data != null)
            {
                Debug.Assert(!data._isSealed);
                Debug.Assert(data._anyDataStored);
                data._isSealed = true;
            }
#endif
        }
    }
}

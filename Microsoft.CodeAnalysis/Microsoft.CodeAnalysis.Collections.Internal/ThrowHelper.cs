using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable

namespace Microsoft.CodeAnalysis.Collections.Internal
{
    internal static class ThrowHelper
    {
        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowIndexOutOfRangeException()
        {
            throw new IndexOutOfRangeException();
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException();
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowArgumentOutOfRange_IndexException()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowArgumentException_BadComparer(object? comparer)
        {
            throw new ArgumentException(string.Format(SR.Arg_BogusIComparer, comparer));
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowIndexArgumentOutOfRange_NeedNonNegNumException()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowLengthArgumentOutOfRange_ArgumentOutOfRange_NeedNonNegNum()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.length, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count()
        {
            throw GetArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowWrongKeyTypeArgumentException<T>(T key, Type targetType)
        {
            throw GetWrongKeyTypeArgumentException(key, targetType);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowWrongValueTypeArgumentException<T>(T value, Type targetType)
        {
            throw GetWrongValueTypeArgumentException(value, targetType);
        }

        private static ArgumentException GetAddingDuplicateWithKeyArgumentException(object? key)
        {
            return new ArgumentException(string.Format(SR.Argument_AddingDuplicateWithKey, key));
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowAddingDuplicateWithKeyArgumentException<T>(T key)
        {
            throw GetAddingDuplicateWithKeyArgumentException(key);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowKeyNotFoundException<T>(T key)
        {
            throw GetKeyNotFoundException(key);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowArgumentException(ExceptionResource resource)
        {
            throw GetArgumentException(resource);
        }

        private static ArgumentNullException GetArgumentNullException(ExceptionArgument argument)
        {
            return new ArgumentNullException(GetArgumentName(argument));
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw GetArgumentNullException(argument);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument));
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
        {
            throw GetArgumentOutOfRangeException(argument, resource);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowInvalidOperationException(ExceptionResource resource, Exception e)
        {
            throw new InvalidOperationException(GetResourceString(resource), e);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowNotSupportedException(ExceptionResource resource)
        {
            throw new NotSupportedException(GetResourceString(resource));
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowArgumentException_Argument_InvalidArrayType()
        {
            throw new ArgumentException(SR.Argument_InvalidArrayType);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion()
        {
            throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen()
        {
            throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
        }

        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void ThrowInvalidOperationException_ConcurrentOperationsNotSupported()
        {
            throw new InvalidOperationException(SR.InvalidOperation_ConcurrentOperationsNotSupported);
        }

        private static ArgumentException GetArgumentException(ExceptionResource resource)
        {
            return new ArgumentException(GetResourceString(resource));
        }

        private static ArgumentException GetWrongKeyTypeArgumentException(object? key, Type targetType)
        {
            return new ArgumentException(string.Format(SR.Arg_WrongType, key, targetType), "key");
        }

        private static ArgumentException GetWrongValueTypeArgumentException(object? value, Type targetType)
        {
            return new ArgumentException(string.Format(SR.Arg_WrongType, value, targetType), "value");
        }

        private static KeyNotFoundException GetKeyNotFoundException(object? key)
        {
            return new KeyNotFoundException(string.Format(SR.Arg_KeyNotFoundWithKey, key));
        }

        private static ArgumentOutOfRangeException GetArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
        {
            return new ArgumentOutOfRangeException(GetArgumentName(argument), GetResourceString(resource));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void IfNullAndNullsAreIllegalThenThrow<T>(object? value, ExceptionArgument argName)
        {
            if (default(T) != null && value == null)
            {
                ThrowArgumentNullException(argName);
            }
        }

        private static string GetArgumentName(ExceptionArgument argument)
        {
            return argument switch
            {
                ExceptionArgument.dictionary => "dictionary",
                ExceptionArgument.array => "array",
                ExceptionArgument.key => "key",
                ExceptionArgument.value => "value",
                ExceptionArgument.startIndex => "startIndex",
                ExceptionArgument.index => "index",
                ExceptionArgument.capacity => "capacity",
                ExceptionArgument.collection => "collection",
                ExceptionArgument.item => "item",
                ExceptionArgument.converter => "converter",
                ExceptionArgument.match => "match",
                ExceptionArgument.count => "count",
                ExceptionArgument.action => "action",
                ExceptionArgument.comparison => "comparison",
                ExceptionArgument.source => "source",
                ExceptionArgument.length => "length",
                ExceptionArgument.destinationArray => "destinationArray",
                _ => "",
            };
        }

        private static string GetResourceString(ExceptionResource resource)
        {
            return resource switch
            {
                ExceptionResource.ArgumentOutOfRange_Index => SR.ArgumentOutOfRange_Index,
                ExceptionResource.ArgumentOutOfRange_Count => SR.ArgumentOutOfRange_Count,
                ExceptionResource.Arg_ArrayPlusOffTooSmall => SR.Arg_ArrayPlusOffTooSmall,
                ExceptionResource.Arg_RankMultiDimNotSupported => SR.Arg_RankMultiDimNotSupported,
                ExceptionResource.Arg_NonZeroLowerBound => SR.Arg_NonZeroLowerBound,
                ExceptionResource.ArgumentOutOfRange_ListInsert => SR.ArgumentOutOfRange_ListInsert,
                ExceptionResource.ArgumentOutOfRange_NeedNonNegNum => SR.ArgumentOutOfRange_NeedNonNegNum,
                ExceptionResource.ArgumentOutOfRange_SmallCapacity => SR.ArgumentOutOfRange_SmallCapacity,
                ExceptionResource.Argument_InvalidOffLen => SR.Argument_InvalidOffLen,
                ExceptionResource.ArgumentOutOfRange_BiggerThanCollection => SR.ArgumentOutOfRange_BiggerThanCollection,
                ExceptionResource.NotSupported_KeyCollectionSet => SR.NotSupported_KeyCollectionSet,
                ExceptionResource.NotSupported_ValueCollectionSet => SR.NotSupported_ValueCollectionSet,
                ExceptionResource.InvalidOperation_IComparerFailed => SR.InvalidOperation_IComparerFailed,
                _ => "",
            };
        }
    }
}

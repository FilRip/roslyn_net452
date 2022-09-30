using System;
using System.Globalization;
using System.Reflection;

#nullable enable

#nullable enable

#nullable enable

namespace Roslyn.Utilities
{
    public static class UICultureUtilities
    {
        private const string currentUICultureName = "CurrentUICulture";

        private static readonly Action<CultureInfo>? s_setCurrentUICulture;

        private static bool TryGetCurrentUICultureSetter([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Action<CultureInfo>? setter)
        {
            try
            {
                Type type = Type.GetType("System.Globalization.CultureInfo, System.Globalization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a") ?? typeof(object).GetTypeInfo().Assembly.GetType("System.Globalization.CultureInfo");
                if ((object)type == null)
                {
                    setter = null;
                    return false;
                }
                MethodInfo methodInfo = type.GetTypeInfo().GetDeclaredProperty("CurrentUICulture")?.SetMethod;
                if ((object)methodInfo == null || !methodInfo.IsStatic || methodInfo.ContainsGenericParameters || methodInfo.ReturnType != typeof(void))
                {
                    setter = null;
                    return false;
                }
                ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(CultureInfo))
                {
                    setter = null;
                    return false;
                }
                setter = (Action<CultureInfo>)methodInfo.CreateDelegate(typeof(Action<CultureInfo>));
                return true;
            }
            catch
            {
                setter = null;
                return false;
            }
        }

        private static bool TryGetCurrentThreadUICultureSetter([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Action<CultureInfo>? setter)
        {
            try
            {
                Type type = typeof(object).GetTypeInfo().Assembly.GetType("System.Threading.Thread");
                if ((object)type == null)
                {
                    setter = null;
                    return false;
                }
                TypeInfo typeInfo = type.GetTypeInfo();
                MethodInfo currentThreadGetter = typeInfo.GetDeclaredProperty("CurrentThread")?.GetMethod;
                if ((object)currentThreadGetter == null || !currentThreadGetter.IsStatic || currentThreadGetter.ContainsGenericParameters || currentThreadGetter.ReturnType != type || currentThreadGetter.GetParameters().Length != 0)
                {
                    setter = null;
                    return false;
                }
                MethodInfo currentUICultureSetter = typeInfo.GetDeclaredProperty("CurrentUICulture")?.SetMethod;
                if ((object)currentUICultureSetter == null || currentUICultureSetter.IsStatic || currentUICultureSetter.ContainsGenericParameters || currentUICultureSetter.ReturnType != typeof(void))
                {
                    setter = null;
                    return false;
                }
                ParameterInfo[] parameters = currentUICultureSetter.GetParameters();
                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(CultureInfo))
                {
                    setter = null;
                    return false;
                }
                setter = delegate (CultureInfo culture)
                {
                    MethodInfo methodInfo = currentUICultureSetter;
                    object obj2 = currentThreadGetter.Invoke(null, null);
                    object[] parameters2 = new CultureInfo[1] { culture };
                    methodInfo.Invoke(obj2, parameters2);
                };
                return true;
            }
            catch
            {
                setter = null;
                return false;
            }
        }

        static UICultureUtilities()
        {
            if (!TryGetCurrentUICultureSetter(out s_setCurrentUICulture) && !TryGetCurrentThreadUICultureSetter(out s_setCurrentUICulture))
            {
                s_setCurrentUICulture = null;
            }
        }

        public static Action WithCurrentUICulture(Action action)
        {
            Action action2 = action;
            if (s_setCurrentUICulture == null)
            {
                return action2;
            }
            CultureInfo savedCulture = CultureInfo.CurrentUICulture;
            return delegate
            {
                CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
                if (currentUICulture != savedCulture)
                {
                    s_setCurrentUICulture!(savedCulture);
                    try
                    {
                        action2();
                        return;
                    }
                    finally
                    {
                        s_setCurrentUICulture!(currentUICulture);
                    }
                }
                action2();
            };
        }

        public static Action<T> WithCurrentUICulture<T>(Action<T> action)
        {
            Action<T> action2 = action;
            if (s_setCurrentUICulture == null)
            {
                return action2;
            }
            CultureInfo savedCulture = CultureInfo.CurrentUICulture;
            return delegate (T param)
            {
                CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
                if (currentUICulture != savedCulture)
                {
                    s_setCurrentUICulture!(savedCulture);
                    try
                    {
                        action2(param);
                        return;
                    }
                    finally
                    {
                        s_setCurrentUICulture!(currentUICulture);
                    }
                }
                action2(param);
            };
        }

        public static Func<T> WithCurrentUICulture<T>(Func<T> func)
        {
            Func<T> func2 = func;
            if (s_setCurrentUICulture == null)
            {
                return func2;
            }
            CultureInfo savedCulture = CultureInfo.CurrentUICulture;
            return delegate
            {
                CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
                if (currentUICulture != savedCulture)
                {
                    s_setCurrentUICulture!(savedCulture);
                    try
                    {
                        return func2();
                    }
                    finally
                    {
                        s_setCurrentUICulture!(currentUICulture);
                    }
                }
                return func2();
            };
        }
    }
}

using GameManagement.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GameManagement
{
    public static class Extensions
    {
        public static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }
        public static T GetRequiredService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetRequiredService(typeof(T));
        }

        public static string GetRole(this ClaimsPrincipal cp)
        {
            return cp.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static string GetID(this ClaimsPrincipal cp)
        {
            return cp.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static async Task<ClaimsPrincipal> CurrentUser(this AuthenticationStateProvider asp)
        {
            return (await asp.GetAuthenticationStateAsync()).User;
        }

        public static string RequestLanguage(this IHttpContextAccessor http)
        {
            var feature = http.HttpContext.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>();
            return feature.RequestCulture.UICulture.Name;
        }

        public static List<Tuple<string, string>> GetSupportedLanguages(this IOptions<RequestLocalizationOptions> options)
        {
            List<Tuple<string, string>> langs = new List<Tuple<string, string>>();
            foreach (var c in options.Value.SupportedUICultures)
            {
                langs.Add(Tuple.Create(c.Name, c.EnglishName));
            }
            return langs;
        }

        public static string RoleName(this IStringLocalizer<SharedResources> L, string name)
        {
            return L[$"Role_{name}"];
        }
        public static string GoodsName(this IStringLocalizer<SharedResources> L, int id)
        {
            return L[$"GoodsName_{id.ToString()}"];
        }
        public static string CharClassName(this IStringLocalizer<SharedResources> L, GameChar.TypeID id)
        {
            return L[$"CharClass_{id.ToString()}"];
        }
        public static string SkillName(this IStringLocalizer<SharedResources> L, int id)
        {
            return L[$"CharSkillName_{id.ToString()}"];
        }
        public static string ItemName(this IStringLocalizer<SharedResources> L, int id)
        {
            return L[$"CharItemName_{id.ToString()}"];
        }

        public static string GetString(this IStringLocalizer<SharedResources> L, string header, int p1)
        {
            return L[$"{header}{p1.ToString()}"];
        }
    }

    public static class IntEx
    {
        public static int ParseEx(string text, int default_value = default)
        {
            if (int.TryParse(text, out var v))
                return v;

            return default_value;
        }
    }
    public static class Int64Ex
    {
        public static Int64 ParseEx(string text, Int64 default_value = default)
        {
            if (Int64.TryParse(text, out var v))
                return v;

            return default_value;
        }
    }

    public static class EnumEx
    {
        public static IEnumerable<TItem> GetEnumerable<TItem>()
        {
            return Enum.GetValues(typeof(TItem)).Cast<TItem>();
        }
    }

    public static class StringEx
    {
        public static bool IsNullOrEmptyOR(string value1, string value2)
        {
            return string.IsNullOrEmpty(value1) || string.IsNullOrEmpty(value2);
        }

        static string[] emptiesForSplit = new string[] { " ", "\t", "\r", "\n" };
        public static string[] SplitAllEmptyEntries(this string text)
        {
            return text?.Split(emptiesForSplit, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        }
    }

    public static class DateTimeEx
    {
        public static string ToDBFormatString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static DateTime RoundToHour(this DateTime dt)
        {
            if (dt.Minute > 0 || dt.Second > 0)
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0).AddHours(1);
            else
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
        }
    }

    public static class IEnumerableEx
    {
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> enumeration, TSource target)
        {
            return enumeration.Except(new TSource[] { target });
        }
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static bool IsNullOrEmpty<T>(IEnumerable<T> col)
        {
            return col == null || col.Any() == false;
        }

        public static int IndexOf<T>(this IEnumerable<T> self, T elementToFind)
        {
            int i = 0;
            foreach (T element in self)
            {
                if (Equals(element, elementToFind))
                    return i;
                i++;
            }
            return -1;
        }
        public static int IndexOf<T>(this IEnumerable<T> self, Func<T, bool> func)
        {
            int i = 0;
            foreach (T element in self)
            {
                if (func(element))
                    return i;
                i++;
            }
            return -1;
        }
        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            if (list is List<T> asList)
            {
                asList.AddRange(items);
            }
            else
            {
                foreach (T item in items)
                {
                    list.Add(item);
                }
            }
        }
    }
    public static class ArrayEx
    {
        public static T[] CreateWithNew<T>(int length) where T : new()
        {
            var arr = new T[length];
            for (int i = 0; i < length; i++)
            {
                arr[i] = new T();
            }
            return arr;
        }
    }


}

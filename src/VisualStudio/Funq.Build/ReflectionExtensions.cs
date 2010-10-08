using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Funq.Build
{
	internal static class ReflectionExtensions
	{
		public static bool HasCustomAttribute<TAttribute>(this ICustomAttributeProvider provider, bool inherit = true)
			where TAttribute : Attribute
		{
			return GetCustomAttributes<TAttribute>(provider, inherit).Any();
		}

		public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider provider, bool inherit = true)
			where TAttribute : Attribute
		{
			return GetCustomAttributes<TAttribute>(provider, inherit).FirstOrDefault();
		}

		public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this ICustomAttributeProvider provider, bool inherit = true)
			where TAttribute : Attribute
		{
			return provider
				.GetCustomAttributes(typeof(TAttribute), inherit)
				.Cast<TAttribute>();
		}
	}
}
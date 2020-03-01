using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ofl.Text.Json
{
    internal static class JsonConstructorJsonConverterExtensions
    {
        public static ConstructorInfo? GetJsonConstructor(this Type type)
        {
            // The constructors to work with.
            // We'll allow private constructors here.
            IReadOnlyCollection<ConstructorInfo> constructors = type
                .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // Is there a constructor on the type that has the
            // attribute, then use that.
            var query = 
                from c in constructors
                let attr = c.GetCustomAttribute<JsonConstructorAttribute>(false)
                where attr != null
                select c;

            // Return
            return query.SingleOrDefault();
        }
    }
}

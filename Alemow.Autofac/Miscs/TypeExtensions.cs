using System.Collections.Generic;
using System.Reflection;

namespace Alemow.Miscs
{
    public static class TypeExtensions
    {
        public static IEnumerable<FieldInfo> GetAllFields(this TypeInfo type)
        {
            do
            {
                foreach (var field in type.GetRuntimeFields())
                {
                    yield return field;
                }
            } while ((type = type.BaseType?.GetTypeInfo()) != null);
        }
    }
}
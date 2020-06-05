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
                foreach (var field in type.DeclaredFields)
                {
                    yield return field;
                }
            } while ((type = type.BaseType?.GetTypeInfo()) != null);
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this TypeInfo type)
        {
            do
            {
                foreach (var method in type.DeclaredMethods)
                {
                    yield return method;
                }
            } while ((type = type.BaseType?.GetTypeInfo()) != null);
        }
    }
}
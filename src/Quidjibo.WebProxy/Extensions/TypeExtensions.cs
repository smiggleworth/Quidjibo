using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Quidjibo.WebProxy.Extensions
{
    public static class TypeExtensions
    {
        // https://stackoverflow.com/questions/74616/how-to-detect-if-type-is-another-generic-type/1075059#1075059

        /// <summary>
        ///     Check if a type is assignable to an generic type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="generic">The generic type to check against.</param>
        /// <returns>Returns true if the type is assignable to the generic type.</returns>
        public static bool IsAssignableToGenericType(this Type type, Type generic)
        {
            return new[] {type}.Concat(type.GetTypeInfo().ImplementedInterfaces).Any(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == generic) || (type.GetTypeInfo().BaseType?.IsAssignableToGenericType(generic) ?? false);
        }

        /// <summary>
        ///     Check if the type is an IEnumerable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="innerType">The uderlying type.</param>
        /// <returns></returns>
        public static bool IsEnumerable(this Type type, out Type innerType)
        {
            innerType = null;
            if(type.IsArray)
            {
                innerType = type.GetElementType();
                return true;
            }

            if(type.IsAssignableToGenericType(typeof(IEnumerable<>)))
            {
                innerType = type.GenericTypeArguments[0];
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Check if this type is traversable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns></returns>
        public static bool IsTraversable(this Type type)
        {
            return type.IsTraversable(out var underlyingType);
        }

        /// <summary>
        ///     Check if this type is traversable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="underlyingType">The uderlying type that was checked.</param>
        /// <returns></returns>
        public static bool IsTraversable(this Type type, out Type underlyingType)
        {
            underlyingType = type;
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                underlyingType = Nullable.GetUnderlyingType(underlyingType).GetTypeInfo();
            }

            return !(underlyingType == null
                     || underlyingType.IsEnum
                     || underlyingType.IsNotPublic
                     || underlyingType.IsPointer
                     || underlyingType.IsPrimitive
                     || typeof(Action<>).IsAssignableFrom(underlyingType)
                     || typeof(bool).IsAssignableFrom(underlyingType)
                     || typeof(byte).IsAssignableFrom(underlyingType)
                     || typeof(byte[]).IsAssignableFrom(underlyingType)
                     || typeof(char).IsAssignableFrom(underlyingType)
                     || typeof(char[]).IsAssignableFrom(underlyingType)
                     || typeof(DateTime).IsAssignableFrom(underlyingType)
                     || typeof(DBNull).IsAssignableFrom(underlyingType)
                     || typeof(decimal).IsAssignableFrom(underlyingType)
                     || typeof(double).IsAssignableFrom(underlyingType)
                     || typeof(Exception).IsAssignableFrom(underlyingType)
                     || typeof(float).IsAssignableFrom(underlyingType)
                     || typeof(Func<>).IsAssignableFrom(underlyingType)
                     || typeof(Guid).IsAssignableFrom(underlyingType)
                     || typeof(int).IsAssignableFrom(underlyingType)
                     || typeof(long).IsAssignableFrom(underlyingType)
                     || typeof(MulticastDelegate).IsAssignableFrom(underlyingType)
                     || typeof(sbyte).IsAssignableFrom(underlyingType)
                     || typeof(short).IsAssignableFrom(underlyingType)
                     || typeof(string).IsAssignableFrom(underlyingType)
                     || typeof(Task).IsAssignableFrom(underlyingType)
                     || typeof(Type).IsAssignableFrom(underlyingType)
                     || typeof(uint).IsAssignableFrom(underlyingType)
                     || typeof(ulong).IsAssignableFrom(underlyingType)
                     || typeof(ushort).IsAssignableFrom(underlyingType)
                     || typeof(void).IsAssignableFrom(underlyingType)
                );
        }
    }
}
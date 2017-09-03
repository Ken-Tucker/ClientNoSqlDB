using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;


namespace ClientNoSqlDB
{
  static class TypeHelper
  {



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetCustomAttribute<T>(this Type type) where T : Attribute
    {
      return (T)Attribute.GetCustomAttribute(type, typeof(T));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValueType(this Type type)
    {
      return type.IsValueType;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGenericType(this Type type)
    {
      return type.IsGenericType;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnum(this Type type)
    {
      return type.IsEnum;
    }



    public static IEnumerable<MethodInfo> GetStaticMethods(this Type type)
    {

      return type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
    }

    public static MethodInfo GetPublicStaticMethod(this Type type, string name)
    {
      return type.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
    }

    public static MethodInfo GetPublicInstanceMethod(this Type type, string name)
    {
      return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
    }

    public static MethodInfo GetStaticMethod(this Type type, string name)
    {
      return type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
    }

    public static MethodInfo GetPrivateInstanceMethod(this Type type, string name)
    {
      return type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
    }

    public static IEnumerable<FieldInfo> GetPublicInstanceFields(this Type type)
    {
      return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
    }

    public static IEnumerable<PropertyInfo> GetPublicInstanceProperties(this Type type)
    {
      return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }


  }
}

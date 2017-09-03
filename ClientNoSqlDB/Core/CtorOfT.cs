using System;
using System.Linq.Expressions;


namespace ClientNoSqlDB
{
  /// <summary>
  /// Extreme fast generic constructor. 
  /// </summary>
  /// <remarks>
  /// About 20 times faster than "new T()", because the last one uses slow reflection-based Activator.CreateInstance internally.
  /// </remarks>
  /// <typeparam name="T">Type to construct</typeparam>
  static class Ctor<T>
  {


    /// <summary>
    /// Generic <typeparamref name="T"/> constructor function
    /// </summary>
    public static readonly Func<T> New = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
  }

  /// <summary>
  /// Extreme fast generic constructor. Constructs <typeparamref name="T"/>, but returns <typeparamref name="R"/>. So <typeparamref name="T"/> must be direct assignable to <typeparamref name="R"/>.
  /// </summary>
  /// <remarks>
  /// About 20 times faster than "new T()", because the last one uses slow reflection-based Activator.CreateInstance internally.
  /// </remarks>
  /// <typeparam name="T">Type to construct</typeparam>
  /// <typeparam name="R">Type to return</typeparam>
  static class Ctor<R, T> where T : R
  {


    /// <summary>
    /// Generic <typeparamref name="T"/> constructor function, returning <typeparamref name="T"/> as <typeparamref name="R"/> type
    /// </summary>
    public static readonly Func<R> New = Expression.Lambda<Func<R>>(Expression.New(typeof(T))).Compile();

  }
}

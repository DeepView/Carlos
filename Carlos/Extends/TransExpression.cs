using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
namespace Carlos.Extends
{
    /// <summary>
    /// 具有缓存加速功能的快速反射操作类。
    /// </summary>
    /// <typeparam name="TIn">被反射或者被复制的类的数据类型。</typeparam>
    /// <typeparam name="TOut">接受反射或者接受复制的类的数据类型。</typeparam>
    public class TransExpression<TIn, TOut>
    {
        private static readonly Func<TIn, TOut> cache = TransExpFunc();
        /// <summary>
        /// 该操作可以进行快速反射或者快速复制。
        /// </summary>
        /// <returns>该操作返回的是一个接受反射或者接受复制的对象。</returns>
        private static Func<TIn, TOut> TransExpFunc()
        {
            ParameterExpression defaultParamExpr = Expression.Parameter(typeof(TIn), "obj");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();
            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite) continue;
                PropertyInfo propertyInfo = typeof(TIn).GetProperty(item.Name) 
                    ?? throw new ArgumentException($"Property '{item.Name}' not found in type '{typeof(TIn).Name}'");
                MemberExpression property = Expression.Property(defaultParamExpr, propertyInfo);
                MemberBinding propertyBinding = Expression.Bind(item, property);
                memberBindingList.AddRange(new MemberBinding[] { propertyBinding });
            }
            MemberInitExpression memberInitExpression = Expression.MemberInit(
                Expression.New(typeof(TOut)),
                memberBindingList.ToArray()
            );
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(
                memberInitExpression,
                new ParameterExpression[] { defaultParamExpr }
            );
            return lambda.Compile();
        }
        /// <summary>
        /// 将参数指定的对象快速反射或者复制一次。
        /// </summary>
        /// <param name="source"></param>
        /// <returns>该操作将会返回一个从参数source复制或者反射的对象。</returns>
        public static TOut Trans(TIn source) => cache(source);
    }
}

using System.Collections.Generic;
namespace Carlos.Extends
{
    /// <summary>
    /// 适用于引用类型的引用计数查询器。
    /// </summary>
    /// <typeparam name="T">被查询引用计数的对象类型。</typeparam>
    /// <remarks>
    /// 该类用于管理引用类型对象的引用计数，提供了获取或创建实例、增加和减少引用计数等功能。但明确的是，所有的对象的创建和引用计数管理都在静态缓存中进行。也就是说，所有的对象都被缓存起来了。没有被缓存的对象，无法被查询器管理。
    /// </remarks>
    public class RefCountedQuerier<T> where T : class
    {
        private readonly T _value;
        private int _refCount;
        private static readonly Dictionary<T, RefCountedQuerier<T>> _objectCache = [];
        /// <summary>
        /// 创建一个新的引用计数查询器实例。
        /// </summary>
        /// <param name="value">需要被查询的对象。</param>
        private RefCountedQuerier(T value)
        {
            _value = value;
            _refCount = 1;
        }
        /// <summary>
        /// 从缓存中获取对象实例，如果不存在则创建一个新的实例，并增加引用计数。
        /// </summary>
        /// <param name="value">指定的引用类型对象。</param>
        /// <returns>该操作会返回一个当前Class的实例。</returns>
        public static RefCountedQuerier<T> GetOrCreate(T value)
        {
            if (_objectCache.TryGetValue(value, out var existing))
            {
                existing._refCount++;
                return existing;
            }
            var newObj = new RefCountedQuerier<T>(value);
            _objectCache[value] = newObj;
            return newObj;
        }
        /// <summary>
        /// 获取或设置当前对象需要被操作的对象值。
        /// </summary>
        public T Value => _value;
        /// <summary>
        /// 增加引用计数。
        /// </summary>
        public void IncrementRefCount() => _refCount++;
        /// <summary>
        /// 减少引用计数，如果引用计数降到0，则从缓存中移除该对象。
        /// </summary>
        /// <returns>该操作会返回一个Boolean类型的数据，用于表示被引用的对象，其引用计数是否为0。</returns>
        public bool DecrementRefCount()
        {
            _refCount--;
            if (_refCount <= 0)
            {
                _objectCache.Remove(_value);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取或设置当前对象的引用计数。
        /// </summary>
        public int RefCount => _refCount;
        /// <summary>
        /// 比较当前对象与另一个对象是否相等。
        /// </summary>
        /// <param name="obj">被比较的对象。</param>
        /// <returns>该操作会返回一个Boolean数据，表示被比较的对象是否与当前对象相等。</returns>
        public override bool Equals(object obj) => obj is RefCountedQuerier<T> other && _value == other._value;
        /// <summary>
        /// 获取当前对象的哈希码。
        /// </summary>
        /// <returns>该操作会返回一个Int32数据，用于存储当前对象的Hash Code。</returns>
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
    }
}

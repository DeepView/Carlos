using System;
using System.Linq;
using System.Collections.Generic;
namespace Carlos.Extends
{
    /// <summary>
    /// 一个支持泛型的定长队列类。
    /// </summary>
    /// <typeparam name="T">用于存放在定长队列中的数据的类型。</typeparam>
    public class FixedLengthQueue<T> : IDisposable
    {
        private bool mDisposedValue = false;//检测冗余调用
        /// <summary>
        /// 构造函数，创建一个指定长度的定长队列。
        /// </summary>
        /// <param name="length">该定长队列的长度，这个值一旦确定，将无法更改。</param>
        /// <exception cref="ArgumentOutOfRangeException">如果传递的参数length小于且等于0，则将会抛出这个异常。</exception>
        public FixedLengthQueue(int length)
        {
            if (length > 0)
            {
                IsEnableRecycle = true;
                RecycleNodeIndex = -1;
                Length = length;
                Head = null;
            }
            else throw new ArgumentOutOfRangeException("length", "The length must greater than zero.");
        }
        /// <summary>
        /// 获取当前定长队列的长度。
        /// </summary>
        public int Length { get; private set; }
        /// <summary>
        /// 获取当前定长队列实例的第一个节点。
        /// </summary>
        public ListNode<T> Head { get; private set; }
        /// <summary>
        /// 获取当前定长队列实例的最后一个节点。
        /// </summary>
        public ListNode<T> Tail
        {
            get
            {
                ListNode<T> node = Head;
                while (node.Next != null) node = node.Next;
                return node;
            }
        }
        /// <summary>
        /// 获取当前定长队列示例的节点数量，节点数量有时候会小于列表长度，有时候会和列表长度相同。
        /// </summary>
        public int Count
        {
            get
            {
                ListNode<T> node = Head;
                int count = 0;
                while (node != null)
                {
                    count++;
                    node = node.Next;
                }
                return count;
            }
        }
        /// <summary>
        /// 获取当前定长队列是否已经装满了数据。
        /// </summary>
        public bool IsFull => Count == Length;
        /// <summary>
        /// 获取或设置当前定长队列实例是否启用数据回收站。
        /// </summary>
        public bool IsEnableRecycle { get; set; }
        /// <summary>
        /// 获取或设置当前定长列表实例的回收站数据，这个属性只能保存一个节点数据，并且在实例被销毁之后，回收站数据也将会清空。
        /// </summary>
        public ListNode<T> Recycle { get; set; }
        /// <summary>
        /// 获取当前定长列表实例的回收站数据，在进入回收站之前于定长队列中的索引。如果这个索引值为-1，则说明Recycle属性为null。
        /// </summary>
        public int RecycleNodeIndex { get; private set; }
        /// <summary>
        /// 获取当前定长队列的节点恢复功能是否可用。
        /// </summary>
        /// <returns>该操作将会返回一个值，如果这个值为true，则表示该实例的节点恢复功能可用，否则不可用。</returns>
        public bool CanRecovery => Recycle != null;
        /// <summary>
        /// 获取当前定长队列实例中指定索引所对应的节点。
        /// </summary>
        /// <param name="index">指定的索引。</param>
        /// <exception cref="ArgumentOutOfRangeException">当参数index指定的索引超出范围时，则会抛出这个异常。</exception>
        public ListNode<T> this[int index]
        {
            get
            {
                ListNode<T> node = Head;
                int counter = 0;
                if (index >= Count || index < 0) throw new ArgumentOutOfRangeException("index", "Index out of range");
                else
                {
                    while (counter < index)
                    {
                        counter++;
                        node = node.Next;
                    }
                }
                return node;
            }
        }
        /// <summary>
        /// 在定长队列的尾部添加新的节点。
        /// </summary>
        /// <param name="element">需要在链表尾部添加的新节点所包含的元素。</param>
        /// <returns>如果操作成功，则返回true，否则返回false。</returns>

        public bool Add(T element)
        {
            int countBeforeAdd = Count;
            ListNode<T> node = new ListNode<T>();
            ListNode<T> inserted = new ListNode<T>(element);
            if (Head == null)
            {
                Head = inserted;
                return true;
            }
            node = Head;
            while (node.Next != null) node = node.Next;
            node.Next = inserted;
            if (Count > Length)
            {
                if (IsEnableRecycle)
                {
                    Recycle = Head;
                    RecycleNodeIndex = 0;
                }
                Head = Head.Next;
            }
            return countBeforeAdd < Count;
        }
        /// <summary>
        /// 从定长队列中移除指定索引所对应的节点。
        /// </summary>
        /// <param name="index">需要被移除的节点所对应的索引。</param>
        /// <returns>如果操作成功，则返回true，否则返回false。</returns>
        /// <exception cref="ArgumentOutOfRangeException">当参数index指定的索引超出范围时，则会抛出这个异常。</exception>
        public bool Remove(int index)
        {
            int countBeforeRemove = Count;
            int counter = 0;
            ListNode<T> node = Head;
            if (index >= Count || index < 0) throw new ArgumentOutOfRangeException("index", "Index out of range.");
            else
            {
                if (index == 0)
                {
                    if (IsEnableRecycle)
                    {
                        Recycle = Head.Next;
                        RecycleNodeIndex = index;
                    }
                    Head.BackwardsPointer();
                    return true;
                }
                while (counter < index - 1)
                {
                    counter++;
                    node = node.Next;
                }
                if (IsEnableRecycle)
                {
                    Recycle = node.Next;
                    RecycleNodeIndex = counter;
                }
                node.BackwardsPointer();
            }
            return countBeforeRemove > Count;
        }
        /// <summary>
        /// 从定长队列中移除指定元素所对应的节点，这个操作只会移除第一个匹配到的节点。
        /// </summary>
        /// <param name="element">用于匹配并移除节点的元素。</param>
        /// <returns>如果操作成功，则返回true，否则返回false。</returns>
        public bool Remove(T element)
        {
            int index = 0;
            int countBeforeRemove = Count;
            while (Head.Element.Equals(element)) Head = Head.Next;
            ListNode<T> node = Head;
            while (node.Next.Next != null)
            {
                if (node.Next.Element.Equals(element))
                {
                    if (IsEnableRecycle)
                    {
                        Recycle = node.Next;
                        RecycleNodeIndex = index;
                    }
                    node.BackwardsPointer();
                    continue;
                }
                index++;
                node = node.Next;
            }
            if (node.Next.Element.Equals(element)) node.NextToNull();
            return countBeforeRemove > Count;
        }
        /// <summary>
        /// 恢复Add或者Remove操作。
        /// </summary>
        public void Recovery()
        {
            if (CanRecovery)
            {
                if (RecycleNodeIndex == 0)
                {
                    Head = Recycle;
                    this[Length - 1].NextToNull();
                }
                else
                {
                    Recycle.Next = this[RecycleNodeIndex];
                    this[RecycleNodeIndex - 1].Next = Recycle;
                }
                Recycle = null;
                RecycleNodeIndex = -1;
            }
        }
        /// <summary>
        /// 获取指定元素所对应节点的第一个索引。
        /// </summary>
        /// <param name="element">进行节点匹配的元素。</param>
        /// <returns>如果操作成功，则返回这个元素所对应节点的索引，否则返回-1。</returns>
        public int FirstIndexOf(T element)
        {
            ListNode<T> node = Head;
            int counter = 0;
            while (node.Next != null)
            {
                if (node.Element.Equals(element)) return counter;
                counter++;
                node = node.Next;
            }
            if (!node.Element.Equals(element)) counter++;
            return counter >= Count ? -1 : counter;
        }
        /// <summary>
        /// 获取指定元素所对应节点的最后一个索引。
        /// </summary>
        /// <param name="element">进行节点匹配的元素。</param>
        /// <returns>如果操作成功，则返回这个元素所对应节点的索引，否则返回-1。</returns>
        public int LastIndexOf(T element)
        {
            ListNode<T> node = Head;
            int index = -1;
            int counter = 0;
            while (node.Next != null)
            {
                if (node.Element.Equals(element)) index = counter;
                counter++;
                node = node.Next;
            }
            if (!node.Element.Equals(element)) index = counter;
            return index;
        }
        /// <summary>
        /// 替换指定节点所对应元素的元素值，这个操作会替换所有匹配到的元素的元素值。
        /// </summary>
        /// <param name="replaced">需要被替换的元素。</param>
        /// <param name="element">被替换的元素的新元素值。</param>
        public void Replace(T replaced, T element)
        {
            ListNode<T> node = Head;
            while (node.Next != null)
            {
                if (node.Element.Equals(replaced)) node.Element = element;
                node = node.Next;
            }
            if (node.Element.Equals(replaced)) node.Element = element;
        }
        /// <summary>
        /// 将定长队列的所有节点进行一次反转排序操作。
        /// </summary>
        public void Reverse()
        {
            ListNode<T> node = Head;
            ListNode<T> nhNode = Head;
            ListNode<T> tempNode = node;
            node = node.Next;
            nhNode.NextToNull();
            while (node.Next != null)
            {
                tempNode = node;
                node = node.Next;
                tempNode.Next = nhNode;
                nhNode = tempNode;
            }
            tempNode = node;
            tempNode.Next = nhNode;
            nhNode = node;
            Head = nhNode;
        }
        /// <summary>
        /// 碎片整理，将零散的数据按照先后顺序进行重新存放
        /// </summary>
        public void Defragment()
        {
            if (Count == 0) return;
            else
            {
                int operatCount = 0;
                FixedLengthQueue<T> queue = new FixedLengthQueue<T>(Length);
                for (int i = 0; i < Length; i++)
                {
                    if (this[i].Element != null)
                    {
                        queue.Add(this[i].Element);
                        operatCount++;
                    }
                    if (operatCount >= Count) break;
                }
                Clear();
                for (int i = 0; i < queue.Length; i++)
                    Add(queue[i].Element);
                queue.Dispose();
            }
        }
        /// <summary>
        /// 判断当前的定长队列是否为空。
        /// </summary>
        /// <returns>如果定长队列为空，则返回true，否则返回false。</returns>
        public bool IsEmpty() => Head == null;
        /// <summary>
        /// 清除当前定长队列的所有节点。
        /// </summary>
        /// <returns>如果操作成功，则返回true，否则返回false。</returns>
        public bool Clear()
        {
            Head = null;
            return Count == 0;
        }
        /// <summary>
        /// 获取当前定长队列的数组表达形式。
        /// </summary>
        /// <returns>该操作会返回一个当前定长队列所对应的数组实例。</returns>
        public T[] ToArray()
        {
            T[] array = new T[Count];
            ListNode<T> node = Head;
            int index = 0;
            while (node.Next != null)
            {
                array[index++] = node.Element;
                node = node.Next;
            }
            array[Count - 1] = node.Element;
            return array;
        }
        /// <summary>
        /// 获取当前定长队列的List&lt;T&gt;表达形式。
        /// </summary>
        /// <returns>该操作会返回一个当前定长队列所对应的List&lt;T&gt;实例。</returns>
        public List<T> ToList() => ToArray().ToList();
        /// <summary>
        /// 释放该对象引用的所有内存资源。
        /// </summary>
        /// <param name="disposing">用于指示是否释放托管资源。</param>
        protected virtual void Dispose(bool disposing)
        {
            int headNodeMaxGene;
            if (Head != null) headNodeMaxGene = GC.GetGeneration(Head);
            else headNodeMaxGene = 2;
            if (!mDisposedValue)
            {
                if (disposing)
                {
                    if (Recycle != null)
                    {
                        Recycle.NextToNull();
                        Recycle = null;
                    }
                    while (null != Head)
                    {
                        Head.NextToNull();
                        Head = null;
                    }
                    bool condition = GC.CollectionCount(headNodeMaxGene) == 0;
                    if (condition) GC.Collect(headNodeMaxGene, GCCollectionMode.Forced, true);
                }
                mDisposedValue = true;
            }
        }
        /// <summary>
        /// 手动释放该对象引用的所有内存资源。
        /// </summary>
        public void Dispose() => Dispose(true);
    }
}

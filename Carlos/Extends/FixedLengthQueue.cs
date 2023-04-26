using System;
using System.Linq;
using System.Collections.Generic;
namespace Carlos.Extends
{
    /// <summary>
    /// 一个支持泛型的定长列表类。
    /// </summary>
    /// <typeparam name="T">用于存放在定长列表中的数据的类型。</typeparam>
    public class FixedLengthQueue<T> : IDisposable
    {
        private bool mDisposedValue = false;//检测冗余调用
        /// <summary>
        /// 构造函数，创建一个指定长度的定长列表。
        /// </summary>
        /// <param name="length">该定长列表的长度，这个值一旦确定，将无法更改。</param>
        /// <exception cref="ArgumentOutOfRangeException">如果传递的参数length小于且等于0，则将会抛出这个异常。</exception>
        public FixedLengthQueue(int length)
        {
            if (length > 0)
            {
                Length = length;
                Head = null;
            }
            else throw new ArgumentOutOfRangeException("length", "The length must greater than zero.");
        }
        /// <summary>
        /// 获取当前定长列表的长度。
        /// </summary>
        public int Length { get; private set; }
        /// <summary>
        /// 获取当前定长列表实例的第一个节点。
        /// </summary>
        public ListNode<T> Head { get; private set; }
        /// <summary>
        /// 获取当前定长列表实例的最后一个节点。
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
        /// 获取当前定长列表示例的节点数量，节点数量有时候会小于列表长度，有时候会和列表长度相同。
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
        /// 获取当前定长列表实例中指定索引所对应的节点。
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
                    Head.BackwardsPointer();
                    return true;
                }
                while (counter < index - 1)
                {
                    counter++;
                    node = node.Next;
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
            int countBeforeRemove = Count;
            while (Head.Element.Equals(element)) Head = Head.Next;
            ListNode<T> node = Head;
            while (node.Next.Next != null)
            {
                if (node.Next.Element.Equals(element))
                {
                    node.BackwardsPointer();
                    continue;
                }
                node = node.Next;
            }
            if (node.Next.Element.Equals(element)) node.NextToNull();
            return countBeforeRemove > Count;
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
        /// 将定长列表的所有节点进行一次反转排序操作。
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
        /// 判断当前的定长列表是否为空。
        /// </summary>
        /// <returns>如果定长列表为空，则返回true，否则返回false。</returns>
        public bool IsEmpty() => Head == null;
        /// <summary>
        /// 清除当前定长列表的所有节点。
        /// </summary>
        /// <returns>如果操作成功，则返回true，否则返回false。</returns>
        public bool Clear()
        {
            Head = null;
            return Count == 0;
        }
        /// <summary>
        /// 获取当前定长列表的数组表达形式。
        /// </summary>
        /// <returns>该操作会返回一个当前定长列表所对应的数组实例。</returns>
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
        /// 获取当前定长列表的List&lt;T&gt;表达形式。
        /// </summary>
        /// <returns>该操作会返回一个当前定长列表所对应的List&lt;T&gt;实例。</returns>
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

using System.Diagnostics;
namespace Carlos.Extends
{
    /// <summary>
    /// 一个适用于链表等数据结构的节点。
    /// </summary>
    /// <typeparam name="T">被存放的数据的类型。</typeparam>
    [DebuggerDisplay("ListNode=[Element:{Element}]")]
    public class ListNode<T>
    {
        /// <summary>
        /// 构造函数，创建一个内容和后继为空的节点。
        /// </summary>
        public ListNode()
        {
            Element = default;
            Next = null;
        }
        /// <summary>
        /// 构造函数，创建一个指定存储元素和一个NULL后继的节点。
        /// </summary>
        /// <param name="element">节点需要存储的数据。</param>
        public ListNode(T element)
        {
            Element = element;
            Next = null;
        }
        /// <summary>
        /// 构造函数，创建一个存储指定元素和后继的节点。
        /// </summary>
        /// <param name="element">节点需要存储的数据。</param>
        /// <param name="next">一个ListNode&lt;T&gt;实例，指向下一个节点。</param>
        public ListNode(T element, ListNode<T> next)
        {
            Element = element;
            Next = next;
        }
        /// <summary>
        /// 获取或设置当前实例的节点数据。
        /// </summary>
        public T Element { get; set; }
        /// <summary>
        /// 获取或设置当前实例的后继。
        /// </summary>
        public ListNode<T> Next { get; set; }
        /// <summary>
        /// 将当前节点的数据初始化。
        /// </summary>
        public void ElementToDefault() => Element = default;
        /// <summary>
        /// 将当前节点的后继指向NULL。
        /// </summary>
        public void NextToNull() => Next = null;
        /// <summary>
        /// 将指针移动到下一个后继，即当前元素后继的后继。
        /// </summary>
        public void BackwardsPointer() => Next = Next.Next;
    }
}

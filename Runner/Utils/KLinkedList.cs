using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Runner
{
    public class KLinkedList<T>
    {
        public KLinkedListNode<T> First;
        public KLinkedListNode<T> Last;
        public long Length;

        public KLinkedList(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                AddToEnd(value);
            }
        }

        public KLinkedList<T> AddToEnd(T value)
        {
            var newNode = new KLinkedListNode<T>(this)
            {
                Value = value
            };

            if (Last != null)
            {
                Last.Next = newNode;
                newNode.Prev = Last;
            }
            Last = newNode;
            if (First == null) First = newNode;
            Length++;
            return this;
        }

        public KLinkedList<T> Reverse()
        {
            KLinkedListNode<T> temp;
            var node = First;
            while (node != null)
            {
                temp = node.Next;
                node.Next = node.Prev;
                node.Prev = temp;
                node = temp;
            }
            temp = First;
            First = Last;
            Last = temp;
            return this;
        }

        public IEnumerable<T> Values
        {
            get
            {
                var node = First;
                T value;
                while (node != null)
                {
                    value = node.Value;
                    node = node.Next;
                    yield return value;
                }
            }
        }
    }

    public class KLinkedListNode<T>
    {
        public KLinkedList<T> List { get; set; }
        public KLinkedListNode<T> Prev { get; set; }
        public KLinkedListNode<T> Next { get; set; }
        public T Value;

        public KLinkedListNode(KLinkedList<T> list)
        {
            List = list;
        }

        public KLinkedListNode<T> InsertBefore(KLinkedListNode<T> newNode)
        {
            List.Length++;
            newNode.Next = this;
            newNode.Prev = this.Prev;
            this.Prev.Next = newNode;
            this.Prev = newNode;
            return newNode;
        }

        public void Remove()
        {
            List.Length--;
            this.Prev.Next = this.Next;
            this.Next.Prev = this.Prev;
        }

        public KLinkedListNode<T> Advance(long count, bool wrap = false)
        {
            var node = this;
            KLinkedListNode<T> next = null;
            var countSize = Math.Abs(count);
            var forward = count > 0;
            for (int i = 0; i < countSize; i++)
            {
                if (node == null) throw new IndexOutOfRangeException();
                next = forward ? node.Next : node.Prev;
                if (wrap && next == null) next = forward ? List.First : List.Last;
                node = next;
            }
            return node;
        }
    }
}

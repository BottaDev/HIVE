using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node<T1,T2>
{
    public T1 item1;
    public T2 item2;

    public Node<T1, T2> next;
}

public class LinkedList<T1,T2>
{
    public Node<T1,T2> first;
    public Node<T1,T2> last;

    int _count;

    public int Count { get => _count;}

    public void Add(T1 obj1, T2 obj2)
    {
        Node<T1,T2> newNode = new Node<T1,T2>();
        newNode.item1 = obj1;
        newNode.item2 = obj2;

        if (first != null)
        {
            last.next = newNode;
            last = newNode;
        }
        else
        {
            first = newNode;
            last = newNode;
        }

        _count++;
    }
}

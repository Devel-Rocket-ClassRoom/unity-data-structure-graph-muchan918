using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<TElement, TPriority>
{
    private readonly IComparer<TPriority> comparer;

    protected List<(TElement, TPriority)> list = new List<(TElement, TPriority)>();

    public int Count { get { return list.Count; } }

    public PriorityQueue(IComparer<TPriority> comparer = null)
    {
        this.comparer = comparer ?? Comparer<TPriority>.Default;
    }

    public void Enqueue(TElement element, TPriority priority)
    {
        list.Add((element, priority));
        HeapifyUp();
    }

    public TElement Dequeue()
    {
        if (list.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        (TElement, TPriority) temp = list[0];
        list[0] = list[Count - 1];
        list.RemoveAt(Count - 1);

        HeapifyDown();

        return temp.Item1;
    }
    public TElement Peek()
    {
        if (list.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        return list[0].Item1;
    }

    public void Clear()
    {
        list.Clear();
    }

    public void HeapifyUp()
    {
        int index = list.Count - 1;

        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (comparer.Compare(list[index].Item2, list[parentIndex].Item2) < 0)
            {
                Swap(index, parentIndex);
                index = parentIndex;
            }
            else
            {
                break;
            }
        }
    }

    public void HeapifyDown()
    {
        int index = 0;

        while (true)
        {
            int leftChildIndex = index * 2 + 1;
            int rightChildIndex = index * 2 + 2;
            int smallest = index;

            if (leftChildIndex < list.Count &&
                comparer.Compare(list[leftChildIndex].Item2, list[smallest].Item2) < 0)
            {
                smallest = leftChildIndex;
            }

            if (rightChildIndex < list.Count &&
                comparer.Compare(list[rightChildIndex].Item2, list[smallest].Item2) < 0)
            {
                smallest = rightChildIndex;
            }

            if (smallest == index) break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    public void Swap(int a, int b)
    {
        (list[a], list[b]) = (list[b], list[a]);
    }
}
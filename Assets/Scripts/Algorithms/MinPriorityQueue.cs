using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinPriorityQueue<T>
{
    List<(T, float)> Elements;
    bool Dirty;

    public MinPriorityQueue()
    {
        this.Elements = new();
        this.Dirty = false;
    }

    public void Equeue(T o, float priority)
    {
        Elements.Add((o, priority));
        Dirty = true;
    }

    public (T, float) Pop()
    {
        LazySorting();

        (T, float) minPriority = Elements.First();
        Elements.RemoveAt(0);
        return minPriority;
    }

    public (T, float) Peek()
    {
        LazySorting();

        return Elements[0];
    }

    private void LazySorting()
    {
        if (Dirty)
            Elements.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
    }

    public bool IsEmpty()
    {
        return Elements.Count == 0;
    }
}

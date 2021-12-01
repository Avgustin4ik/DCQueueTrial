using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientsQueue<T> : IEnumerable<T> where T : class
{
    private List<T> _list;
    public int Count => _list.Count;

    public T this[int index] => _list[index];

    public void Enqueue()
    {

    }
    public void Dequeue()
    {

    }
    public void Remove()
    {
        
    }

    public IEnumerator<T> GetEnumerator()
    {
        for(int i = 0; i < Count; i++) yield return this[i];
    }
 
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
 

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mutex : MonoBehaviour
{
    Object owner = null;

    public IEnumerator Lock(Object owner)
    {
        while(this.owner != null) yield return null;
        this.owner = owner;
    }

    public void Unlock(Object owner) {
        Debug.Assert(owner == this.owner);
        this.owner = null;
    }
}
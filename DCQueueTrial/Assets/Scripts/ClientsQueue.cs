using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientsQueue
{
    public List<Client> _data;
    private const int _defaultCapacity = 4; 

    public ClientsQueue(int capacity = _defaultCapacity) {
        _data = new List<Client>(capacity);
    }

    public int Count => _data.Count;
    public Client Peek() {
        return _data.Count > 0 ? _data[0] : null;
    }   
    public void Enqueue(Client client) => _data.Add(client);

    public bool Dequeue(Client client) {
        int index = _data.IndexOf(client);
        bool result = _data.Remove(client);
        for (int i = index; i < _data.Count; i++){
            _data[i].AdvanceInQueue();
        }
        return result;
        }    

    

}





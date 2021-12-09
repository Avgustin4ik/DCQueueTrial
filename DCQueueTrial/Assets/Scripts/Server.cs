using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Server : MonoBehaviour, IPointerDownHandler
{    

    public ClientsQueue queue = new ClientsQueue();
    private Client currentClient;
    
    public Client CurrentClient{
        get{
            return queue.Peek();
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(CurrentClient) {
            CurrentClient.Serve();
        }   
    }

}

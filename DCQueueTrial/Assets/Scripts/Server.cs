using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Server : MonoBehaviour, IPointerDownHandler
{
    public Client currentClient;
    public Queue<Client> clientsQueue = new Queue<Client>();
    
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(clientsQueue.Count > 0) {
            currentClient = clientsQueue.Peek();
            currentClient.Serve();
        }
    }

    public void AdvanceTheQueue()
    {
        clientsQueue.Dequeue();
        foreach (var client in clientsQueue)
        {
            client.MakeStepInQueue();                
        }
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Server : MonoBehaviour, IPointerDownHandler
{
    public Client currentClient;
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(currentClient) {
            currentClient.Serve();
        }
    }
}

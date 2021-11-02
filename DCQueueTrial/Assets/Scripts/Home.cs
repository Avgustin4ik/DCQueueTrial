using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Home : MonoBehaviour, IPointerDownHandler
{
    public Client client;
    public void Start()
    {
        if(client) {
            GetComponent<SpriteRenderer>().color = client.GetComponent<SpriteRenderer>().color * 0.66f;
            client.home = gameObject;
        }
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(client) client.OnPointerDown(pointerEventData);
    }
}

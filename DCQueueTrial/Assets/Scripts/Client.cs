using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Client : MonoBehaviour, IPointerDownHandler
{
    public float patience = 10f;
    public float speed = 3f;
    public int served = 0;
    public GameObject home;
    public SpriteMask orderTimer;
    private Coroutine activeOrder;
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        GetComponent<IBrain>().OnTapped();
    }
    public void Start()
    {
        orderTimer.gameObject.SetActive(false);
    }
    public void OrderStuff()
    {
        if(activeOrder != null) StopCoroutine(activeOrder);
        activeOrder = StartCoroutine(WaitForOrder());
    }
    float t;
    private IEnumerator WaitForOrder()
    {
        t = patience;
        orderTimer.gameObject.SetActive(true);
        while(t > 0) {
            t -= Time.deltaTime;
            orderTimer.alphaCutoff = t / patience;
            yield return null;
        }
        orderTimer.gameObject.SetActive(false);
        GetComponent<IBrain>().OnExpired();
        activeOrder = null;
    }
    public void Serve()
    {
        served ++;
        orderTimer.gameObject.SetActive(false);
        if(activeOrder != null) {
            StopCoroutine(activeOrder);
            activeOrder = null;
        }
        GetComponent<IBrain>().OnServed();
    }
    public void ExpireNow() {
        t = 0;
    }
}

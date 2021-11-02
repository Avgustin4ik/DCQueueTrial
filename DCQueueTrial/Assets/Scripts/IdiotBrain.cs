using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdiotBrain : MonoBehaviour, IBrain
{
    private bool tapped;
    private bool served;
    private bool expired;
    private bool destinationReached;
    private bool emotePlayed;
    public void OnTapped(){
        tapped = true;
    }
    public void OnServed(){
        served = true;
    }
    public void OnExpired(){
        expired = true;
    }
    public void OnDestinationReached(){
        destinationReached = true;
    }
    public void OnEmotePlayed(){
        emotePlayed = true;
    }

    Mutex mutex;
    // Start is called before the first frame update
    void Start()
    {
        mutex = GameObject.FindObjectOfType<Mutex>();
        StartCoroutine(Idle());
    }

    IEnumerator Idle() {
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        while(!tapped) {
            yield return null;
        }
        StartCoroutine(WaitForServerLock());
    }

    IEnumerator WaitForServerLock() {
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        IEnumerator mutexLock = mutex.Lock(this);
        while(true){
            if(!mutexLock.MoveNext()) {
                StartCoroutine(GoToServer());
                yield break;
            } else if(tapped) {
                StartCoroutine(GoToBase());
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator GoToServer() {
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        GetComponent<Legs>().GoTo(mutex.transform.position);
        while(true) {
            if(destinationReached) {
                StartCoroutine(WaitForServed());
                yield break;
            } else if(tapped) {
                StartCoroutine(GoToBase());
                mutex.Unlock(this);
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator WaitForServed() {
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        GetComponent<Client>().OrderStuff();
        Server server = GameObject.FindObjectOfType<Server>();
        server.currentClient = GetComponent<Client>();
        while(true) {
            if(served) {
                StartCoroutine(PlayEmote());
                server.currentClient = null;
                yield break;
            } else if(expired) {
                StartCoroutine(GoToBase());
                mutex.Unlock(this);
                server.currentClient = null;
                yield break;
            } else if(tapped) {
                GetComponent<Client>().ExpireNow();
            }
            yield return null;
        }
    }

    IEnumerator PlayEmote() {
        emotePlayed = false;
        GetComponent<Animator>().SetTrigger("Happy");
        while(!emotePlayed) {
            yield return null;
        }
        mutex.Unlock(this);
        StartCoroutine(GoToBase());
    }

    IEnumerator GoToBase() {
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        GetComponent<Legs>().GoTo(GetComponent<Client>().home.transform.position);
        while(true) {
            if(destinationReached) {
                StartCoroutine(Idle());
                yield break;
            } else if(tapped) {
                GetComponent<Legs>().Stop();
                StartCoroutine(WaitForServerLock());
                yield break;
            }
            yield return null;
        }
    }

}

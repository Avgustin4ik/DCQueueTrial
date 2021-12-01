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
    private bool queued;

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
    Server server;
    // Start is called before the first frame update
    void Start()
    {
        mutex = GameObject.FindObjectOfType<Mutex>();
        server = GameObject.FindObjectOfType<Server>();
        StartCoroutine(Idle());
    }

    IEnumerator Idle() {
        queued = false;
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
        queued = false;
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        StartCoroutine(GoToServer());
        while(true){
            if(server.clientsQueue.Peek() == this.GetComponent<Client>()) {
                IEnumerator mutexLock = mutex.Lock(this);
                if(!mutexLock.MoveNext()) {
                    yield break;
                }
            } 
            yield return null;
        }
    }

    float offset = 1f;

    public void MakeStepInQueue() {
                StartCoroutine(MakeStep());
    }

    IEnumerator MakeStep() {
        while(!queued) {
            yield return null;
        }
        GetComponent<Legs>().GoTo(transform.position + Vector3.left*offset);
    }


    IEnumerator GoToServer() {
        queued = false;
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        GetComponent<Legs>().GoTo(mutex.transform.position + Vector3.right*offset*server.clientsQueue.Count);
        server.clientsQueue.Enqueue(GetComponent<Client>());
        while(true) {
            if(destinationReached) {
                StartCoroutine(WaitForServed());
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator WaitForServed() {
        queued = true;
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        GetComponent<Client>().OrderStuff();
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
            }
            yield return null;
        }
    }

    IEnumerator PlayEmote() {
        emotePlayed = false;
        GetComponent<Animator>().SetTrigger("Happy");
        mutex.Unlock(this);
        while(!emotePlayed) {
            yield return null;
        }
        StartCoroutine(GoToBase());
    }

    IEnumerator GoToBase() {
        queued = false;
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        GetComponent<Legs>().GoTo(GetComponent<Client>().home.transform.position);
        server.AdvanceTheQueue();                   
        while(true) {
            if(destinationReached) {
                StartCoroutine(Idle());
                yield break;
            }
            yield return null;
        }
    }

}

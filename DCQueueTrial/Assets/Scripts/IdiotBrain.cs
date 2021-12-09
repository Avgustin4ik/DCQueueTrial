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
    bool isFirst => server.queue.Peek() == GetComponent<Client>();

    public void MakeStepInQueue(){
        StartCoroutine(MakeStep());
    }

    IEnumerator MakeStep() {
        while(!queued) {
            yield return null;
        }
        GetComponent<Legs>().GoTo(transform.position + Vector3.left*offset);
    }

    Mutex mutex;
    Server server;
    float offset = 1f;
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
        Client client = GetComponent<Client>();
        StartCoroutine(GoToServer());
        while(true){
            if(isFirst){
                IEnumerator mutexLock = mutex.Lock(this);
                if(!mutexLock.MoveNext()){
                    yield break;
                }
                else if(tapped){
                    StopCoroutine(mutexLock);
                    yield break;
                }
            }
            yield return null;
        }
    }


    IEnumerator GoToServer() {
        queued = false;
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        Vector3 queueLastPosition = Vector3.right*offset*server.queue.Count;
        GetComponent<Legs>().GoTo(mutex.transform.position + queueLastPosition);
        server.queue.Enqueue(GetComponent<Client>());
        while(true) {
            if(destinationReached) {
                StartCoroutine(WaitForServed());
                yield break;
            } else if(tapped) {
                if(isFirst)
                    mutex.Unlock(this);
                StartCoroutine(GoToBase());
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
                yield break;
            } else if(expired) {
                if(isFirst)
                    mutex.Unlock(this);
                StartCoroutine(GoToBase());
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
        queued = false;
        tapped = false;
        expired = false;
        served = false;
        destinationReached = false;
        GetComponent<Legs>().GoTo(GetComponent<Client>().home.transform.position);
        server.queue.Dequeue(GetComponent<Client>());
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

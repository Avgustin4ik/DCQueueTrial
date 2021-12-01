 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Legs : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    Coroutine walk = null;
    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    public void GoTo(Vector3 position) {
        Stop();
        agent.destination = position;
        agent.updateRotation = false;
        agent.speed = GetComponent<Client>()?.speed ?? agent.speed;
        walk = StartCoroutine(WalkRoutine());
    }

    // void SetSpeed(float value) => animator.SetFloat("Speed", value); возможно смысла не много вводить лямбда фунцкцию ради трех строчек

    private IEnumerator WalkRoutine()
    {
        while(agent.pathPending) yield return null;
        while(agent.remainingDistance > agent.stoppingDistance) 
        {
            animator.SetFloat("Speed", agent.speed);
            yield return null;
        }
        animator.SetFloat("Speed", 0f);
        GetComponent<IBrain>().OnDestinationReached();
    }

    public void Stop()
    {
        animator.SetFloat("Speed", 0f);
        if(walk != null) StopCoroutine(walk);
        walk = null;
    }
}

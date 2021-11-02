using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteState : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<IBrain>().OnEmotePlayed();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkFollowState : StateMachineBehaviour
{
    private EnemyPathfinder enemyPathfinder;
    private WalkBasicMovement walkbasicMovement;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPathfinder = animator.GetComponent<EnemyPathfinder>();
        walkbasicMovement = animator.GetComponent<WalkBasicMovement>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyPathfinder.PathfindingState==EnemyPathfinder.State.HasNoTarget)
        {
            animator.SetBool("IsReady", true);
            animator.SetBool("IsFollow", false);
            enemyPathfinder.canFindPath = false;
            walkbasicMovement.enabled = true;

        }
    }

}

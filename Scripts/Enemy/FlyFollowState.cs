using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyFollowState : StateMachineBehaviour
{
    EnemyPathfinder enemyPathfinder;
    FlyBasicMovement flybasicMovement;
    Entity entity;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPathfinder = animator.GetComponent<EnemyPathfinder>();
        flybasicMovement = animator.GetComponent<FlyBasicMovement>();
        entity = animator.GetComponent<Entity>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (entity.IsDead!=true)
        {
            if (enemyPathfinder.PathfindingState == EnemyPathfinder.State.HasNoTarget)
            {
                animator.SetBool("isReady", true);
                animator.SetBool("isFollow", false);
                enemyPathfinder.canFindPath = false;
                flybasicMovement.enabled = true;
            }
        }
    }
}

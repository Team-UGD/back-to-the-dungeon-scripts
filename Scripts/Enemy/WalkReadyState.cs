using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkReadyState : StateMachineBehaviour
{

    private EnemyDetection enemyDetection;
    private EnemyPathfinder enemyPathfinder;
    private WalkBasicMovement walkbasicMovement;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyDetection = animator.GetComponent<EnemyDetection>();
        enemyPathfinder = animator.GetComponent<EnemyPathfinder>();
        walkbasicMovement = animator.GetComponent<WalkBasicMovement>();
        enemyPathfinder.canFindPath = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyPathfinder.PathfindingState == EnemyPathfinder.State.HasNoTarget)
        {
            walkbasicMovement.enabled = true;
            walkbasicMovement.Move();
        }
        if (enemyDetection.Target!=null)
        {
            animator.SetBool("IsReady", false);
            animator.SetBool("IsFollow", true);
            enemyPathfinder.canFindPath = true;
            walkbasicMovement.enabled = false;

        }
    }

}

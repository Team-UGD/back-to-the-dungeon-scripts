using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyReadyState : StateMachineBehaviour
{
    private EnemyDetection enemyDetection;
    private EnemyPathfinder enemyPathfinder;
    private FlyBasicMovement flybasicMovement;
    private Entity entity;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyDetection = animator.GetComponent<EnemyDetection>();
        enemyPathfinder = animator.GetComponent<EnemyPathfinder>();
        flybasicMovement = animator.GetComponent<FlyBasicMovement>();
        entity = animator.GetComponent<Entity>();
        enemyPathfinder.canFindPath = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (entity.IsDead != true)
        {
            if (enemyPathfinder.PathfindingState == EnemyPathfinder.State.HasNoTarget)
            {
                flybasicMovement.enabled = true;
            }
            if (enemyDetection.Target != null)
            {
                animator.SetBool("isReady", false);
                animator.SetBool("isFollow", true);
                enemyPathfinder.canFindPath = true;
                flybasicMovement.enabled = false;
            }
        }
        else
            flybasicMovement.enabled = false;
    }

}

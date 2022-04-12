using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : StateMachineBehaviour
{
    private EnemyPathfinder enemyPathfinder;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyPathfinder = animator.GetComponent<EnemyPathfinder>();

        enemyPathfinder.canFindPath = false;
    }

}
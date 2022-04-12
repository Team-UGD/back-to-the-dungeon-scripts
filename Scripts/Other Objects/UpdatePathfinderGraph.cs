using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class UpdatePathfinderGraph : MonoBehaviour
{
    [SerializeField] private float updateRate = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating(nameof(UpdateGrpah), 0f, updateRate);
    }

    private void UpdateGrpah()
    {
        AstarPath.active.Scan();
    }
}

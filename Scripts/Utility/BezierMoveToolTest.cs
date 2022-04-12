using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMoveToolTest : MonoBehaviour
{
    private BezierMoveTool moveTool;

    private void Awake()
    {
        moveTool = GetComponent<BezierMoveTool>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            moveTool.MovePathOnce(0);
            moveTool.MovePathOnce(1);
            moveTool.MovePathOnce(1, BezierMoveDirection.Backward);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            moveTool.MovePathIteratively(0);
            moveTool.MovePathIteratively(1);
            moveTool.MovePathIteratively(1, BezierMoveDirection.Backward);
            moveTool.MovePathIteratively(0, BezierMoveDirection.Backward);
        }
    }
}

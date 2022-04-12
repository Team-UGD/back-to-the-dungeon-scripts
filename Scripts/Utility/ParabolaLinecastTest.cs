using UnityEngine;

public class ParabolaLinecastTest : MonoBehaviour
{
    public Vector2 relativeVertex;
    public Vector2 relativeEnd;
    public int intersectionCount = 2;
    public LayerMask layer;

    private void Start()
    {
        PhysicsUtility.DrawParabolaLine(transform.position, (Vector2)transform.position + relativeVertex, (Vector2)transform.position + relativeEnd, Color.red, intersectionCount, 5);
    }

    // Update is called once per frame
    void Update()
    {
        var temp = PhysicsUtility.ParabolaLinecast(transform.position, (Vector2)transform.position + relativeVertex, (Vector2)transform.position + relativeEnd, layer, intersectionCount);
    }
}

using UnityEngine;

public class FullScreenBackground : MonoBehaviour
{
    private GameObject player;
    private SpriteRenderer sprite;
    private Vector3 defaultLocalScale;

    private void Awake()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        sprite = GetComponent<SpriteRenderer>();
        defaultLocalScale = transform.localScale;

        SetBackground();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player)
        {
            transform.position = player.transform.position;
            SetBackground();
        }
    }

    private void SetBackground()
    {
        if (sprite.sprite)
        {
            float cameraHeight = Camera.main.orthographicSize * 2;
            Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
            Vector2 spriteSize = sprite.sprite.bounds.size;

            Vector2 scale = defaultLocalScale;
            scale.x *= cameraSize.x / spriteSize.x;
            scale.y *= cameraSize.y / spriteSize.y;

            //if (cameraSize.x >= cameraSize.y)
            //{
            //    scale *= cameraSize.x / spriteSize.x;
            //}
            //else
            //{
            //    scale *= cameraSize.y / spriteSize.y;
            //}

            //transform.parent = Vector2.zero;
            transform.localScale = scale;
        }
    }
}

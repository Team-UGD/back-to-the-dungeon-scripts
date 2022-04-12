using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    RectTransform rectTransform;
    Enemy enemy;

    [SerializeField]float height = 0.7f;
    bool isLeft;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        rectTransform = enemy.healthBar.GetComponent<RectTransform>();

        Vector3 pos = enemy.transform.position;
        rectTransform.position = new Vector3(pos.x, pos.y+height, 0);

    }

    private void Update()
    {
        if (transform.localScale.x < 0 && !isLeft)
        {
            rectTransform.localScale = new Vector3(-rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.z);
            isLeft = true;
        }

        if (transform.localScale.x > 0 && isLeft)
        {
            rectTransform.localScale = new Vector3(-rectTransform.localScale.x, rectTransform.localScale.y, rectTransform.localScale.z);
            isLeft = false;
        }
    }
}

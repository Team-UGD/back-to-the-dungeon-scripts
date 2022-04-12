using System.Collections;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;

    private void Start()
    {
        StartCoroutine(MyCursor());

        if (GameManager.Instance.IsControllerConnection)
            Cursor.visible = false;
        else
            Cursor.visible = true;

    }

    private IEnumerator MyCursor()
    {
        // 모든 렌더링이 완료될 때까지 대기
        yield return new WaitForEndOfFrame();
        hotSpot.x = cursorTexture.width / 2;
        hotSpot.y = cursorTexture.height / 2;

        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
}

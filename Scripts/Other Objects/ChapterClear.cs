using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterClear : MonoBehaviour
{
    [SerializeField] private Enemy chapterBoss;
    [SerializeField, ScenePopup] private int nextBaseCamp;

    private void Start()
    {
        chapterBoss.OnDeath += () =>
        {
            bool enabled = Store.SetCanChangeItemCount(nextBaseCamp);
            Debug.Log($"<{nameof(ChapterClear)}> {SceneManager.GetSceneByBuildIndex(nextBaseCamp).name}에서의 Store 아이템 개수 변경 가능 여부: {enabled} ", this);
        };
    }
}

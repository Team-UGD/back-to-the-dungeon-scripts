using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 추상 클레스
/// </summary>
public abstract class Item : MonoBehaviour
{
    public enum Type { Coin, Potion, Invincible, IncreaseMaxHealth, IncreaseMoveSpeed , Resurrection }

    Type itemType;
    string itemName;

    /// <summary>
    /// 아이템 타입 프로퍼티
    /// </summary>
    public Type ItemType
    {
        get;
        protected set;
    }

    /// <summary>
    /// 아이템 이름 프로퍼티
    /// </summary>
    public string ItemName
    {
        protected set;
        get;
    }

    /// <summary>
    /// 아이템 획득시 실행되는 추상 메서드
    /// </summary>
    public abstract void GetItem(GameObject player);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetItem(collision.gameObject);
            Destroy(this.gameObject, 0f);
        }
    }

    ///// <summary>
    ///// 아이템을 생성하기 위한 메서드
    ///// </summary>
    //public void CreateNewItem(Vector2 vec)
    //{
    //    string path = SelectedItem();
    //    var prefeb = Resources.Load<Item>(path);

    //    Instantiate(prefeb, vec, Quaternion.identity);
    //}

    //private string SelectedItem()
    //{
    //    int n = Random.Range(0,2);

    //    return n == 0 ? "Coin" : "Potion";
    //}
}
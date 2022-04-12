using UnityEngine;

[CreateAssetMenu(menuName = "Item Data/Coin")]
public class CoinData : SceneData<Item, int>
{
    // 직렬화 시 음수를 방지함.
    protected override int SetDataProperty(int value, int index)
    {
        return Mathf.Max(0, value);
    }

    public override bool TrySetValue(Item instance, int buildIndex)
    {
        bool result = base.TrySetValue(instance, buildIndex);
        if (result)
        {
            // item 인스턴스에 대한 추가 구현
            // 예를 들면 (item as IItemValue<int>).Value 가 500보다 클 때 동색 코인에서 은색 코인으로 변경

            Animator animator = instance.GetComponent<Animator>();

            if (this[buildIndex] >= 600)
                animator.SetTrigger("Gold");
            else if (this[buildIndex] >= 300)
                animator.SetTrigger("Silver");

        }

        return result;
    }
}

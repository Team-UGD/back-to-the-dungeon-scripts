using UnityEngine;
using UnityEngine.UI;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private Graphic itemBuyUI;
    [SerializeField] private Graphic weaponSlotSelectionUI;
    [SerializeField] private Button otherStoreBtn;

    public Graphic ItemBuyUI { get => itemBuyUI; }
    public Graphic WeaponSlotSelectionUI { get => weaponSlotSelectionUI; }

    public Button OtherStoreBtn { get => this.otherStoreBtn; }
}

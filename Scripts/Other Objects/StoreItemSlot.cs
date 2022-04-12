using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Button buyButton;

    public Image ItemImage { get => itemImage; }
    public TextMeshProUGUI Price { get => price; }

    public TextMeshProUGUI Count { get => count; }

    public Button BuyButton { get => buyButton; }
}

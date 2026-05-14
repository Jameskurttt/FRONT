using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemButton : MonoBehaviour
{
    [Header("UI")]
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    public Button buyButton;

    private ShopItemData currentItem;
    private ShopManager shopManager;

    public void Setup(ShopItemData item, ShopManager manager)
    {
        currentItem = item;
        shopManager = manager;

        if (itemIcon != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = item.itemIcon != null;
            itemIcon.preserveAspect = true;
        }

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (descriptionText != null)
            descriptionText.text = item.description;

        if (priceText != null)
            priceText.text = item.price + " Gold";

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(Buy);
        }
    }

    private void Buy()
    {
        if (shopManager != null && currentItem != null)
            shopManager.BuyItem(currentItem);
    }
}
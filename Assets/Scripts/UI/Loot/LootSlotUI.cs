using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text amountText;

    public void SetData(LootResult loot)
    {
        gameObject.SetActive(true);

        if (loot == null || loot.itemData == null)
        {
            Clear();
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = loot.itemData.icon;
            iconImage.enabled = loot.itemData.icon != null;
        }

        if (nameText != null)
        {
            nameText.text = loot.itemData.itemName;
        }

        if (amountText != null)
        {
            amountText.text = $"x{loot.amount}";
        }
    }

    public void Clear()
    {
        gameObject.SetActive(true);

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (nameText != null)
        {
            nameText.text = "";
        }

        if (amountText != null)
        {
            amountText.text = "";
        }
    }
}
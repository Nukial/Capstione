using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public TMP_Text quantityText;

    private Item item;
    private System.Action<Item> onClickCallback;
    private System.Action<Item> onHoverEnterCallback;
    private System.Action onHoverExitCallback;

    public void Setup(Item newItem, System.Action<Item> onClick, System.Action<Item> onHoverEnter, System.Action onHoverExit)
    {
        item = newItem;
        onClickCallback = onClick;
        onHoverEnterCallback = onHoverEnter;
        onHoverExitCallback = onHoverExit;

        if (iconImage != null)
        {
            iconImage.sprite = item.Data.icon;
            iconImage.enabled = true;
        }

        if (quantityText != null)
        {
            if (item is StackableItem stackableItem)
            {
                quantityText.text = stackableItem.Quantity.ToString();
            }
            else
            {
                quantityText.text = "";
            }
        }
    }

    public void Clear()
    {
        item = null;
        onClickCallback = null;
        onHoverEnterCallback = null;
        onHoverExitCallback = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (quantityText != null)
        {
            quantityText.text = "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClickCallback != null && item != null)
        {
            onClickCallback(item);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onHoverEnterCallback != null && item != null)
        {
            onHoverEnterCallback(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onHoverExitCallback != null)
        {
            onHoverExitCallback();
        }
    }
}

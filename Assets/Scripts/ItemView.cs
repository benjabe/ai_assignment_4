using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemView : MonoBehaviour
{
    private Item _item = null;
    private int _quantity;
    private Text _text;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = null;
        _text = GetComponentInChildren<Text>();
    }

    public void SetItem(Item item, int quantity)
    {
        _item = item;
        _quantity = quantity;
        if (item != null)
        {
            spriteRenderer.sprite = item.sprite;
            _text.text = "" + quantity;
        }
        else
        {
            spriteRenderer.sprite = null;
            _text.text = "";
        }
    }
}

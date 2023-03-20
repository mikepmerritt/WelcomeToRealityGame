using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ClickableText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text text;
    public TMPro.FontStyles initialStyle;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        initialStyle = text.fontStyle;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.fontStyle = initialStyle | FontStyles.Underline;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.fontStyle = initialStyle;
    }

}

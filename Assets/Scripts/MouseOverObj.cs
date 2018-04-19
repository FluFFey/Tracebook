using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverObj : MonoBehaviour
{
    public bool isMouseOver;
    public bool outlineOnMouseover;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propBlock;

    public bool lockedOutline;
    

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseEnter()
    {
        isMouseOver = true;
        if (outlineOnMouseover)
        {
            addOutline();
        }
    }

    void OnMouseOver()
    {
        isMouseOver = true;
        if (outlineOnMouseover)
        {
            addOutline();
        }
    }
    private void OnMouseExit()
    {
        isMouseOver = false;
        if (outlineOnMouseover && !lockedOutline)
        {
            removeOutline();
        }
    }
    public void addOutline()
    {
        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Outline", 1);
        spriteRenderer.SetPropertyBlock(propBlock);
    }

    public void removeOutline()
    {
        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Outline", 0);
        spriteRenderer.SetPropertyBlock(propBlock);
    }

}

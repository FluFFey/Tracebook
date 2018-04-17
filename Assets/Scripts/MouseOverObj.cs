using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverObj : MonoBehaviour
{
    public bool isMouseOver;
    public bool outlineOnMouseover;
    private SpriteRenderer renderer;
    private MaterialPropertyBlock propBlock;

    public bool lockedOutline;

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        renderer = GetComponent<SpriteRenderer>();
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
        renderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Outline", 1);
        renderer.SetPropertyBlock(propBlock);
    }

    public void removeOutline()
    {
        renderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Outline", 0);
        renderer.SetPropertyBlock(propBlock);
    }

}

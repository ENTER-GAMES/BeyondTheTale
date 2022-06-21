using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ControllerBasedShadowDetectorTool : MonoBehaviour
{
    [SerializeField]
    protected bool isSelected = false;

    public virtual void Select()
    {
        isSelected = true;
    }

    public virtual void Unselect()
    {
        isSelected = false;
    }

    protected virtual void Update()
    {
        UpdateMouseInput();
    }

    private void UpdateMouseInput()
    {
        if (!isSelected) return;

        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            OnMouse(Input.mousePosition);
        }
    }

    protected virtual void OnMouseDown(Vector3 mousePosition, int mouseIndex = 0)
    {
    }

    protected virtual void OnMouseUp(Vector3 mousePosition, int mouseIndex = 0)
    {
    }

    protected virtual void OnMouse(Vector3 mousePosition, int mouseIndex = 0)
    {
    }
}

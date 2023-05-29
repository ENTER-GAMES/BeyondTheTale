using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CatForMobile : Cat, IPointerDownHandler
{
    protected override void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        animator.SetBool("hit", true);
    }
}

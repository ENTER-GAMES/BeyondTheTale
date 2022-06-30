using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerBasedShadowDetector : ShadowDetector
{
    [SerializeField]
    private ControllerBasedShadowDetectorTool[] tools;
    private ControllerBasedShadowDetectorTool currentTool;

    [Header("@Debug: Shadows")]
    [SerializeField]
    private List<Shadow> shadows = new List<Shadow>();

    private void Start()
    {
        // 도구 초기화
        InitTools();

        // 0번 도구 선택
        DeselectAll();
        Select(0);
    }

    private void InitTools()
    {
        foreach (ControllerBasedShadowDetectorTool tool in tools)
        {
            tool.Init(this);
        }
    }

    public void Select(int index)
    {
        if (index >= tools.Length || index < 0) return;

        DeselectAll();
        tools[index].Select();
    }

    public void DeselectAll()
    {
        foreach (ControllerBasedShadowDetectorTool tool in tools)
        {
            tool.Deselect();
        }
    }

    public void AddShadow(Shadow shadow)
    {
        if (shadows.Contains(shadow)) return;

        shadows.Add(shadow);

        // 선택 도구에서 이동, 회전, 크기에 대한 변형이 일어나기 때문에
        // 새로 그려졌올 때, 딱 한 번만 그려준다.
        MeshDrawer.Draw(new List<Shadow>() { shadow });
    }

    public void RemoveShadow(Shadow shadow)
    {
        if (!shadows.Contains(shadow)) return;

        shadows.Remove(shadow);
    }
}

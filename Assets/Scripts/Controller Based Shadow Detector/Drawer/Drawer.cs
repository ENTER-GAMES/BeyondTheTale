using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drawer : ControllerBasedShadowDetectorTool
{
    [SerializeField]
    protected GameObject drawedShadowObjectPrefab;                        // 그려질 그림자 오브젝트 프리팹
}

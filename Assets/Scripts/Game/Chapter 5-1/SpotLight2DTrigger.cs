using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class SpotLight2DTrigger : MonoBehaviour
{
    private Light2D light2D;
    [SerializeField]
    private LayerMask targetLayerMask;
    [SerializeField]
    private float rayDeltaAngle = 5f;
    private int rayCount;

    private bool init = false;

    private List<Collider2D> targetCollider2Ds = new List<Collider2D>();

    #region Gizmos
    [Header("@Debug: Draw Gizmos")]
    [SerializeField]
    private bool drawGizmos = false;
    private List<GizmoInfo> gizmoInfos = new List<GizmoInfo>();

    private class GizmoInfo
    {
        public Vector3 endPoint;
        public bool hit;

        public GizmoInfo(Vector3 endPoint, bool hit)
        {
            this.endPoint = endPoint;
            this.hit = hit;
        }
    }
    #endregion

    private void Start()
    {
        light2D = GetComponent<Light2D>();

        if (light2D.lightType != Light2D.LightType.Point)
        {
            Debug.LogError($"SpotLight2DTrigger: {this.gameObject.name}.Light2D의 LightType이 Point(Spot)이 아닙니다.");
            this.enabled = false;
            return;
        }

        Init();
    }

    private void Init()
    {
        light2D.pointLightInnerAngle -= light2D.pointLightInnerAngle % (rayDeltaAngle * 2);
        light2D.pointLightOuterAngle = light2D.pointLightInnerAngle;
        rayCount = (int)(light2D.pointLightInnerAngle / rayDeltaAngle) + 1;
        init = true;
    }

    private void Update()
    {
        bool hit = Raycast(ref targetCollider2Ds, gizmoInfos);

        if (!hit) return;

        foreach (Collider2D col in targetCollider2Ds)
        {
            // 충돌된 오브젝트가 ISpotLight2DTarget 구현체라면 트리거
            if (col.TryGetComponent<ISpotLight2DTarget>(out ISpotLight2DTarget target))
            {
                target.OnHitBySpotLight2D();
            }
        }
    }

    private bool Raycast(ref List<Collider2D> result, List<GizmoInfo> gizmoInfos = null)
    {
        bool flag = false;  // 충돌 여부

        result.Clear();
        gizmoInfos?.Clear();

        float startAngle = -(light2D.pointLightInnerAngle / 2);
        float rayDistance = light2D.pointLightOuterRadius;

        for (int i = 0; i < rayCount; i++)
        {
            // 기준(시작) 각도 (standardAngle)
            Vector3 directionPoint = transform.position + transform.up;
            Vector2 standardDir = directionPoint - transform.position;
            float standardAngle = Mathf.Atan2(standardDir.y, standardDir.x) * Mathf.Rad2Deg;
            standardAngle = (standardAngle + startAngle + 360) % 360;

            // 생성원 중앙에서 생성 위치쪽을 바라보는 방향
            float spawnAngle = (standardAngle + (rayDeltaAngle * i)) % 360;
            float theta = spawnAngle * Mathf.PI / 180;
            Vector2 rayDir = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            rayDir.Normalize();

            // 레이 발사
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, rayDir, rayDistance, targetLayerMask);
            Collider2D targetCol = hit2D.collider;

            // 충돌했을 경우
            if (targetCol != null)
            {
                flag = true;

                if (!result.Contains(targetCol))
                    result.Add(targetCol);

                gizmoInfos?.Add(new GizmoInfo(hit2D.centroid, true));
            }
            // 충돌하지 않았을 경우
            else
            {
                gizmoInfos?.Add(new GizmoInfo(transform.position + (Vector3)rayDir * rayDistance, false));
            }
        }

        return flag;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        foreach (GizmoInfo info in gizmoInfos)
        {
            Gizmos.color = info.hit ? Color.red : Color.blue;
            Gizmos.DrawLine(transform.position, info.endPoint);
        }
    }
}


public interface ISpotLight2DTarget
{
    public void OnHitBySpotLight2D();
}

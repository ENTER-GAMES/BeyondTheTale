using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class SpotLight2DTrigger : MonoBehaviour
{
    private Light2D light2D;
    [SerializeField]
    private Transform forwardDirectionTransform;
    [SerializeField]
    private LayerMask targetLayerMask;
    [SerializeField]
    private float rayDeltaAngle = 5f;
    private int rayCount;

    private bool init = false;

    [Header("@Debug: Draw Gizmos")]
    [SerializeField]
    private bool drawGizmos = false;

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
        Collider2D collider2D = Raycast();

        if (collider2D == null) return;

        if (collider2D.TryGetComponent<ISpotLight2DTarget>(out ISpotLight2DTarget target))
        {
            target.OnHitBySpotLight2D();
        }
    }

    private Collider2D Raycast(bool drawGizmos = false)
    {
        float startAngle = -(light2D.pointLightInnerAngle / 2);
        float rayDistance = light2D.pointLightOuterRadius;

        for (int i = 0; i < rayCount; i++)
        {
            // 기준(시작) 각도 (standardAngle)
            Vector3 directionPoint = forwardDirectionTransform.position;
            Vector2 standardDir = directionPoint - transform.position;
            float standardAngle = Mathf.Atan2(standardDir.y, standardDir.x) * Mathf.Rad2Deg;
            standardAngle = (standardAngle + startAngle + 360) % 360;

            // 생성원 중앙에서 생성 위치쪽을 바라보는 방향
            float spawnAngle = (standardAngle + (rayDeltaAngle * i)) % 360;
            float theta = spawnAngle * Mathf.PI / 180;
            Vector2 rayDir = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            rayDir.Normalize();

            // 충돌 확인
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, rayDir, rayDistance, targetLayerMask);

            // 충돌했을 경우
            if (hit2D != null && hit2D.collider != null)
            {
                if (drawGizmos)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, hit2D.centroid);
                }
                return hit2D.collider;
            }
            // 충돌하지 않았을 경우
            else
            {
                if (drawGizmos)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.position, transform.position + (Vector3)rayDir * rayDistance);
                }
            }
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || !init) return;

        Raycast(true);
    }
}


public interface ISpotLight2DTarget
{
    public void OnHitBySpotLight2D();
}

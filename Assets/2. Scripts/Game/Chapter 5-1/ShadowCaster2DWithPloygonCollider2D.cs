using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(ShadowCaster2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class ShadowCaster2DWithPloygonCollider2D : MonoBehaviour
{
    [SerializeField]
    private bool setOnAwake;
    private ShadowCaster2D shadowCaster;
    private PolygonCollider2D polygonCollider2D;

    // ShadowCaster2D에서 바꾸려는 필드
    private readonly FieldInfo _shapePathField;
    private readonly FieldInfo _shapeHash;

    private ShadowCaster2DWithPloygonCollider2D()
    {
        _shapeHash = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
        _shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    private void Awake()
    {
        shadowCaster = GetComponent<ShadowCaster2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();

        if (setOnAwake)
            UpdateFromCollider();
    }

    private void Update()
    {
        UpdateFromCollider();
    }

    /// <summary>
    /// 콜라이더 좌표를 기반으로 그림자 업데이트
    /// </summary>
    public void UpdateFromCollider()
    {
        if (polygonCollider2D != null)
        {
            UpdateShadowFromPoints(polygonCollider2D.points);
        }
    }

    /// <summary>
    /// Vector3 배열로 그림자 업데이트
    /// </summary>
    public void UpdateShadowFromPoints(Vector3[] points)
    {
        // 그림자 패스(Path) 설정
        _shapePathField.SetValue(shadowCaster, points);

        unchecked
        {
            // I have no idea what im doing with hashcodes but other examples are done like this
            int hashCode = (int)2166136261 ^ _shapePathField.GetHashCode();
            hashCode = hashCode * 16777619 ^ (points.GetHashCode());

            // Set the shapes hash to a random value which forces it to update the mesh in the next frame
            _shapeHash.SetValue(shadowCaster, hashCode);
        }
    }

    /// <summary>
    /// Vector2 배열로 그림자 업데이트
    /// </summary>
    public void UpdateShadowFromPoints(Vector2[] points) => UpdateShadowFromPoints(Vector2ToVector3(points));

    /// <summary>
    /// Vector2 배열을 Vector3 배열로 변환
    /// </summary>
    private Vector3[] Vector2ToVector3(Vector2[] vector2s)
    {
        Vector3[] vector3s = new Vector3[vector2s.Length];

        for (int i = 0; i < vector2s.Length; i++)
        {
            vector3s[i] = vector2s[i];
        }

        return vector3s;
    }
}

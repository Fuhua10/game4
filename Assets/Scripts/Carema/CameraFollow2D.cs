using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public sealed class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Tilemap ground;
    [SerializeField, Min(0.1f)] private float viewHalfHeight = 3.5f;
    [SerializeField, Min(0f)] private float smoothTime = 0.16f;

    private Camera view;
    private Vector2 followVelocity;

    private void Awake()
    {
        view = GetComponent<Camera>();
        if (!target || !ground)
        {
            Debug.LogError("CameraFollow2D 需要指定 Player 和 Ground。", this);
            enabled = false;
        }
    }

    private void Start()
    {
        if (enabled) Follow(true);
    }

    private void LateUpdate()
    {
        Follow(false);
    }

    private void Follow(bool snap)
    {
        // 地面是 XY 平面的矩形 Grid。相机中心必须与外缘相隔半个视口。
        BoundsInt cells = ground.cellBounds;
        Vector2 min = ground.CellToWorld(new Vector3Int(cells.xMin, cells.yMin, 0));
        Vector2 max = ground.CellToWorld(new Vector3Int(cells.xMax, cells.yMax, 0));
        float aspect = view.aspect;
        if (max.x <= min.x || max.y <= min.y || aspect <= 0f) return;

        float halfHeight = Mathf.Min(viewHalfHeight, (max.y - min.y) * 0.5f,
                                     (max.x - min.x) * 0.5f / aspect);
        view.orthographicSize = halfHeight;
        Vector2 margin = new Vector2(halfHeight * aspect, halfHeight);
        Vector2 center = (max + min) * 0.5f;
        Vector2 travel = Vector2.Max(Vector2.zero, (max - min) * 0.5f - margin);

        Vector2 desired = Limit(target.position, center, travel);
        Vector2 next = snap || smoothTime <= 0f ? desired :
            Vector2.SmoothDamp(transform.position, desired, ref followVelocity, smoothTime);
        // 改变窗口宽高比时，正在平滑移动的相机也需要落在新边界内。
        next = Limit(next, center, travel);
        transform.position = new Vector3(next.x, next.y, transform.position.z);
    }

    private static Vector2 Limit(Vector2 point, Vector2 center, Vector2 travel)
    {
        return new Vector2(Mathf.Clamp(point.x, center.x - travel.x, center.x + travel.x),
                           Mathf.Clamp(point.y, center.y - travel.y, center.y + travel.y));
    }
}

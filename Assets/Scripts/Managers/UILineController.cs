using UnityEngine;

public class UILineController : MonoBehaviour
{
    [SerializeField] private RectTransform[] points;
    private LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        SetUpLine(points);
    }

    private void SetUpLine(RectTransform[] points)
    {
        lr.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 currentPos = points[i].position;

            if (i == 0 && points.Length > 1)
            {
                currentPos = GetOffsetPosition(points[0], points[1]);
            } else if(i == points.Length - 1 && points.Length > 1) {
                currentPos = GetOffsetPosition(points[i], points[i - 1]);
            }

            currentPos.z = transform.position.z;
            lr.SetPosition(i, currentPos);
        }
    }

    private Vector3 GetOffsetPosition(RectTransform origin, RectTransform target)
    {
        Vector3 position = (target.position - origin.position).normalized;
        float offset = (origin.rect.width / 2f) * origin.lossyScale.x;

        return origin.position + (position * offset);
    }
}

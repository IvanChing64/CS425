using UnityEngine;

//Author: Ivan Ching
//Developed from different tutorials
//Purpose: This script controls the line between two different nodes. it takes a game object and a line renderer to generate a line from 2 offseted points of the node
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


    //Sets the start and end points of the line to bridge 2 different nodes.
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


    //Calculates the offset of the line from the center of the node, moving it left or right depending on the where it starts and ends.
    private Vector3 GetOffsetPosition(RectTransform origin, RectTransform target)
    {
        Vector3 position = (target.position - origin.position).normalized;
        float offset = (origin.rect.width / 2f) * origin.lossyScale.x;

        return origin.position + (position * offset);
    }
}

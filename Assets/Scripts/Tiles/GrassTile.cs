using UnityEngine;

public class GrassTile : Tile
{
    [SerializeField] private Color baseColor, offsetColor;

    //Checkerboard Pattern
    public override void Init(int x, int y)
    {
        var isOffset = (x + y) % 2 == 1;
        Debug.Log($"{name} Init() called. Renderer is {_renderer}");
        _renderer.color = isOffset ? offsetColor : baseColor;
    }
}

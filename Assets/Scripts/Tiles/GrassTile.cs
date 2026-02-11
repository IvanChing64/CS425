using UnityEngine;

public class GrassTile : Tile
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private Sprite[] grassSprites;

    //Checkerboard Pattern
    public override void Init(int x, int y)
    {

        if (grassSprites != null && grassSprites.Length > 0)
        {
            _renderer.sprite = grassSprites[Random.Range(0, grassSprites.Length)];
        }
        var isOffset = (x + y) % 2 == 1;
        //Debug.Log($"{name} Init() called. Renderer is {_renderer}");
        _renderer.color = isOffset ? offsetColor : baseColor;
    }
}

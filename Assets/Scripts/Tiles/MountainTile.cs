using UnityEngine;

public class MountainTile : Tile
{
   [SerializeField] private Sprite[] mountainSprites;

    public override void Init(int x, int y)
    {

        if (mountainSprites != null && mountainSprites.Length > 0)
        {
            _renderer.sprite = mountainSprites[Random.Range(0, mountainSprites.Length)];
        }
    }


}

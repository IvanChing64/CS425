using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    public bool PlaceOnUnit(BaseUnit unit)
    {
        if (unit == null)
        {
            Hide();
            return false;
        }

        MoveToUnit(unit);
        Show();
        return true;

    }

    public void UpdateStatPanel()
    {

    }

    public void Hide() => gameObject.SetActive(false);

    /// <summary>
    /// Only use if you can guarantee the unit it was on will be in the same position
    /// </summary>
    public void Show() => gameObject.SetActive(true);

    private void MoveTo(Vector2 toPos)
    {
        // Why does Unity make me do this
        Vector3 newPos = new Vector3(toPos.x, toPos.y, transform.position.z);
        transform.position = newPos;
    }

    private void MoveToTile(Tile tile) => MoveTo(tile.Position);
    private void MoveToUnit(BaseUnit unit) => MoveTo(unit.transform.position);
}

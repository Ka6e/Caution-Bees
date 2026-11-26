using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cellPrefab;
    [SerializeField, Min(1)] private int level = 2;
    [SerializeField] private int sizeBonus;
    [SerializeField, Min(0f)] private float cellGap = 0.06f;

    private Transform cells;

    // private void Start()
    // {
    //     if (cells == null)
    //     {
    //         Generate(level, sizeBonus);
    //     }
    // }

    public void Generate(int levelNumber, int bonusRings)
    {
        if (cellPrefab == null || cellPrefab.sprite == null)
        {
            Debug.LogError("Assign a cell prefab with a point-top hexagon sprite.", this);
            return;
        }

        if (cells != null)
        {
            cells.gameObject.SetActive(false);
            Destroy(cells.gameObject);
        }

        cells = new GameObject("Cells").transform;
        cells.SetParent(transform, false);

        int radius = Mathf.Max(0, Mathf.Max(1, levelNumber) + bonusRings);
        Vector3 spriteSize = cellPrefab.sprite.bounds.size;
        Vector3 prefabScale = cellPrefab.transform.localScale;
        float spacing = 1f + cellGap;
        float columnStep = spriteSize.x * Mathf.Abs(prefabScale.x) * spacing;
        float rowStep = spriteSize.y * Mathf.Abs(prefabScale.y) * 0.75f * spacing;

        for (int row = -radius; row <= radius; row++)
        {
            // Axial coordinates also require -radius <= column + row <= radius.
            int firstColumn = Mathf.Max(-radius, -row - radius);
            int lastColumn = Mathf.Min(radius, -row + radius);

            for (int column = firstColumn; column <= lastColumn; column++)
            {
                Vector3 localPosition = new Vector3(
                    (column + row * 0.5f) * columnStep,
                    row * rowStep,
                    0f);

                SpriteRenderer cell = Instantiate(
                    cellPrefab, cells.TransformPoint(localPosition), cells.rotation, cells);
                cell.name = $"Cell ({column}, {row})";
            }
        }
    }
}

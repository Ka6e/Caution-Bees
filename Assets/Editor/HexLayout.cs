using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

internal static class HexLayout
{
    [MenuItem("Tools/Align Hexagons")]
    private static void Align()
    {
        var scene = SceneManager.GetActiveScene();
        var cells = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<SpriteRenderer>(true))
            .Where(cell => cell.sprite != null && cell.name.StartsWith("Hexagon Point Top"))
            .OrderBy(cell => cell.transform.GetSiblingIndex()).ToArray();
        if (cells.Length == 0)
        {
            EditorUtility.DisplayDialog("Соты", "В сцене не найдены объекты Hexagon Point Top.", "OK");
            return;
        }

        Vector2 origin = cells[0].bounds.center;
        const float spacingFactor = 1.04f;
        float width = cells[0].bounds.size.x * spacingFactor;
        float rowStep = cells[0].bounds.size.y * 0.75f * spacingFactor;
        if (width <= 0 || rowStep <= 0) return;

        var occupied = new HashSet<Vector2Int>();
        var offsets = new List<Vector2>();
        foreach (var cell in cells)
        {
            Vector2 delta = (Vector2)cell.bounds.center - origin;
            int row = Mathf.RoundToInt(delta.y / rowStep);
            int column = Mathf.RoundToInt(delta.x / width - row * 0.5f);
            if (!occupied.Add(new Vector2Int(column, row)))
            {
                Selection.activeGameObject = cell.gameObject;
                EditorUtility.DisplayDialog("Соты", "Две соты попали в одну ячейку. Передвинь выделенную соту ближе к свободному месту и повтори команду.", "OK");
                return;
            }
            offsets.Add(new Vector2((column + row * 0.5f) * width, row * rowStep) - delta);
        }

        Undo.RecordObjects(cells.Select(cell => cell.transform).ToArray(), "Align Hexagons");
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].transform.position += (Vector3)offsets[i];
            PrefabUtility.RecordPrefabInstancePropertyModifications(cells[i].transform);
        }
        EditorSceneManager.MarkSceneDirty(scene);
    }
}

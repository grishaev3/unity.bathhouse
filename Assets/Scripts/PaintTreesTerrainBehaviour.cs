using System.Collections.Generic;
using UnityEngine;

public class PaintTreesTerrainBehaviour : MonoBehaviour
{
    public Terrain terrain;
    public int treeCount = 0;


    private readonly Vector2 _terrainTopLeft = new(-25f, +25f);
    private readonly Vector2 _terrainBottomRight = new(+25f, -25f);

    private readonly Vector2 _borderTopLeft = new(-22.75f, 23f);
    private readonly Vector2 _borderTopRight = new(11.7f, 19f);
    private readonly Vector2 _borderBottomRight = new(11.7f, -11f);
    private readonly Vector2 _borderBottomLeft = new(-2.75f, -15f);

    void Start()
    {
        if (terrain == null)
        {
            terrain = GetComponent<Terrain>();
        }

        ClearTrees();

        GenerateTreesInQuad();
    }

    void ClearTrees()
    {
        terrain.terrainData.treeInstances = new TreeInstance[0];
        terrain.Flush();
    }

    void GenerateTreesInQuad()
    {
        List<TreeInstance> treesInBounds = new ();
        while (treesInBounds.Count < treeCount)
        {
            Vector3 candidatePos = new(
                Random.Range(0f, 1f),
                0f,
                Random.Range(0f, 1f)
            );

            Vector3 worldPos = NormalizedToWorld(candidatePos);

            if (IsPointInQuad(worldPos))
            {
                TreeInstance tree = new()
                {
                    position = candidatePos,
                    prototypeIndex = Random.Range(0, 6),
                    widthScale = Random.Range(0.6f, 0.8f),
                    heightScale = Random.Range(0.6f, 0.8f),
                    color = Color.white,
                    lightmapColor = Color.white
                };

                treesInBounds.Add(tree);
            }
        }

        // Применяем только деревья в границах
        terrain.terrainData.SetTreeInstances(treesInBounds.ToArray(), true);
        terrain.Flush();
    }

    bool IsPointInQuad(Vector3 point)
    {
        //// Проверка принадлежности неправильному четырехугольнику _border*
        //Vector2[] vertices = { _borderTopLeft, _borderTopRight, _borderBottomRight, _borderBottomLeft };

        //// Ray casting с правильной обработкой Z как Y (Unity XZ плоскость)
        //int crossings = 0;

        //for (int i = 0; i < 4; i++)
        //{
        //    Vector2 v1 = vertices[i];
        //    Vector2 v2 = vertices[(i + 1) % 4];

        //    // Исправлено: point.z → point.y (нормализация Y=Z для XZ)
        //    float pointY = point.z; // Z мира = Y для 2D проверки

        //    if ((v1.y > pointY) != (v2.y > pointY) &&
        //        point.x < (v1.x + (v2.x - v1.x) * (pointY - v1.y) / (v2.y - v1.y + 0.0001f)))
        //    {
        //        crossings++;
        //    }
        //}
        //return crossings % 2 == 1;

        return true;
    }



    Vector3 NormalizedToWorld(Vector3 normalized)
    {
        TerrainData data = terrain.terrainData;
        Vector3 worldPos;
        worldPos.x = Mathf.Lerp(_terrainTopLeft.x, _terrainBottomRight.x, normalized.x);
        worldPos.z = Mathf.Lerp(_terrainTopLeft.y, _terrainBottomRight.y, normalized.z);
        worldPos.y = 0;
        return worldPos;
    }
}
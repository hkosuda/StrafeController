using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] float floorWidth = 200.0f;
    [SerializeField] float floorDepth = 200.0f;
    [SerializeField] float boxSize = 5.0f;

    [SerializeField] float courceLength = 100.0f;
    [SerializeField] float tileSize = 2.0f;
    [SerializeField] float minGap = 1.0f;
    [SerializeField] float trySetTilesPer = 0.5f;
    [SerializeField] float lateralSpread = 5.0f;
    [SerializeField] float medialSpread = 0.1f;
    [SerializeField] float distanceCoeff = 0.1f;
    [SerializeField] float dropRate = 0.0f;

    static public float DropRate { get; private set; }

    GameObject root;
    List<GameObject> tiles;

    Color lightGray = new Color(0.45f, 0.45f, 0.45f);
    Color darkGray = new Color(0.2f, 0.2f, 0.2f);

    void Start()
    {
        root = new GameObject("root");

        SetFloor();

        SetTiles();

        DropRate = dropRate;
    }

    void SetFloor()
    {
        var _floor = Resources.Load<GameObject>("Floor");

        var maxZ = boxSize / 2.0f;
        var minZ = maxZ - floorDepth;

        var centerZ = (maxZ + minZ) / 2.0f;

        var floor = Instantiate(_floor, new Vector3(0.0f, 0.0f, centerZ), Quaternion.identity);
        floor.transform.SetParent(root.transform);

        floor.transform.localScale = new Vector3(floorWidth, 1.0f, floorDepth);

        // set material
        var mat = floor.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;

        mat.SetColor("_Color1", lightGray);
        mat.SetColor("_Color2", darkGray);
        mat.SetFloat("_SquareSize", boxSize);
    }

    void SetTiles()
    {
        // settings
        var n_trial = 10;

        // preprocessing
        var _tile = Resources.Load<GameObject>("Tile");

        // processing
        var tiles = new List<GameObject>();

        for(var n = 0; n < GetNumberOfTiles(); n++)
        {
            for (var n_try = 0; n_try < n_trial; n_try++)
            {
                var z = GetPositionZ(n);
                var magnification = GetMagnification(z);

                var x = GetPositionX(magnification);
                var currentTileSize = tileSize * magnification;

                if (!FloorCheck(z, magnification)) { continue; }
                if (!BoxCheck(tiles, z, x, currentTileSize, magnification)) { continue; }

                var y = GetPositionY(x, z);
                var tile = Instantiate(_tile, new Vector3(x, y, z), Quaternion.identity);

                tile.transform.SetParent(root.transform);
                tile.transform.localScale = new Vector3(currentTileSize, 1.0f, currentTileSize);

                tiles.Add(tile);

                break;
            }
        }

        SetRenderer(tiles);

        // - inner function
        int GetNumberOfTiles()
        {
            if (trySetTilesPer == 0.0f)
            {
                return 0;
            }

            else
            {
                return Mathf.RoundToInt(courceLength / trySetTilesPer);
            }
        }

        // - inner function
        float GetPositionZ(int counter)
        {
            var m_spread = medialSpread * GetMagnification(trySetTilesPer * counter);
            var spread = Random.Range(-m_spread, m_spread);

            return counter * trySetTilesPer + spread;
        }

        // - inner function
        float GetMagnification(float z)
        {
            return 1.0f + (z / 10.0f) * distanceCoeff;
        }

        // - inner function
        float GetPositionX(float magnification)
        {
            var l_spread = lateralSpread * magnification;

            return Random.Range(-l_spread, l_spread);
        }

        // - inner function
        bool FloorCheck(float z, float magnification)
        {
            var halfTileSize = tileSize / 2.0f;
            var halfBoxSize = boxSize / 2.0f;
            var currentMinGap = minGap * magnification;

            if (z - halfTileSize - halfBoxSize - currentMinGap < 0.0f)
            {
                return false;
            }

            return true;
        }

        // - inner function
        bool BoxCheck(List<GameObject> tiles, float z, float x, float tileSize, float magnification)
        {
            var _minGap = minGap * magnification;

            var x_min_1 = x - (tileSize / 2.0f + _minGap);
            var x_max_1 = x + (tileSize / 2.0f + _minGap);

            var z_min_1 = z - (tileSize / 2.0f + _minGap);
            var z_max_1 = z + (tileSize / 2.0f + _minGap);

            foreach(var tile in tiles)
            {
                var center = tile.transform.position;

                var x_min_2 = center.x - (tile.transform.localScale.x / 2.0f);
                var x_max_2 = center.x + (tile.transform.localScale.x / 2.0f);

                var z_min_2 = center.z - (tile.transform.localScale.z / 2.0f);
                var z_max_2 = center.z + (tile.transform.localScale.z / 2.0f);

                if (Include(z_min_1, x_min_1, z_min_2, z_max_2, x_min_2, x_max_2)) { return false; }
                if (Include(z_min_1, x_max_1, z_min_2, z_max_2, x_min_2, x_max_2)) { return false; }
                if (Include(z_max_1, x_min_1, z_min_2, z_max_2, x_min_2, x_max_2)) { return false; }
                if (Include(z_max_1, x_max_1, z_min_2, z_max_2, x_min_2, x_max_2)) { return false; }

                if (Include(z_min_2, x_min_2, z_min_1, z_max_1, x_min_1, x_max_1)) { return false; }
                if (Include(z_min_2, x_max_2, z_min_1, z_max_1, x_min_1, x_max_1)) { return false; }
                if (Include(z_max_2, x_min_2, z_min_1, z_max_1, x_min_1, x_max_1)) { return false; }
                if (Include(z_max_2, x_max_2, z_min_1, z_max_1, x_min_1, x_max_1)) { return false; }
            }

            return true;

            // - - inner function 
            static bool Include(float z, float x, float z_min, float z_max, float x_min, float x_max)
            {
                if (x_min <= x && x <= x_max)
                {
                    if (z_min <= z && z <= z_max)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        // - inner function
        float GetPositionY(float x, float z)
        {
            var magnitude = Vecf.Magnitude(new float[2] { x, z });

            return (magnitude / 10.0f) * dropRate;
        }

        // - inner function
        void SetRenderer(List<GameObject> tiles)
        {
            foreach(var tile in tiles)
            {
                var mat = tile.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;

                mat.SetFloat("_PositionX", tile.transform.position.x);
                mat.SetFloat("_PositionZ", tile.transform.position.z);
                mat.SetColor("_LineColor", darkGray);
                mat.SetColor("_TileColor", lightGray);
                mat.SetFloat("_TileSize", tile.transform.localScale.x);
            }
        }
    }

    void Update()
    {
        
    }
}

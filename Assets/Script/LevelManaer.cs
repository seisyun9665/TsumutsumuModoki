using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManaer : MonoBehaviour
{
    /// <summary>
    /// フルーツPrefabリスト
    /// </summary>
    public GameObject[] FruitPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        FruitSpawn(60);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// フルーツ生成
    /// </summary>
    /// <param name="count">生成数</param>
    private void FruitSpawn(int count)
    {
        var StartX = -2;
        var StartY = 5;
        var X = 0;
        var Y = 0;
        var MaxX = 5;

        for (int i = 0; i < count; i++)
        {
            var Position = new Vector3(StartX + X, StartY + Y, 0);
            Instantiate(FruitPrefabs[Random.Range(0,FruitPrefabs.Length)], Position, Quaternion.identity);

            X++;
            if(X==MaxX)
            {
                X = 0;
                Y++;
            }
        }
    }
}

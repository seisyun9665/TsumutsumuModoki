using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelManaer : MonoBehaviour
{
    /// <summary>選択中のフルーツ</summary>
    private List<Fruit> _SelectFruits = new List<Fruit>();

    /// <summary>選択中のフルーツのID</summary>
    private string _SelectID = "";

    /// <summary>シングルトンインスタンス</summary>
    public static LevelManaer Instance { get; private set; }
    /// <summary>フルーツPrefabリスト</summary>
    public GameObject[] FruitPrefabs;
    /// <summary>選択線描画</summary>
    public LineRenderer LineRenderer;
    /// <summary>フルーツを消すために必要な数</summary>
    public int FruitDestroyCount = 3;
    /// <summary>フルーツをつなぐ範囲</summary>
    public float FruitConnectRange = 1.5f;
    void Start()
    {
        Instance = this;
        FruitSpawn(60);
    }

    // Update is called once per frame
    void Update()
    {
        LineRendererUpdate();
    }

    /// <summary>
    /// 選択中のフルーツをつなぐ線の描画を更新
    /// </summary>
    private void LineRendererUpdate()
    {
        if (_SelectFruits.Count >= 2)
        {
            LineRenderer.positionCount = _SelectFruits.Count;
            LineRenderer.SetPositions(_SelectFruits.Select(fruit => fruit.transform.position).ToArray());
            LineRenderer.gameObject.SetActive(true);
        }
        else LineRenderer.gameObject.SetActive(false);
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
            Instantiate(FruitPrefabs[Random.Range(0, FruitPrefabs.Length)], Position, Quaternion.identity);

            X++;
            if (X == MaxX)
            {
                X = 0;
                Y++;
            }
        }
    }

    /// <summary>
    /// フルーツDownイベント
    /// </summary>
    /// <param name="fruit">フルーツ</param>
    public void FruitDown(Fruit fruit)
    {
        _SelectFruits.Add(fruit);
        fruit.SetIsSelect(true);

        _SelectID = fruit.ID;
    }

    /// <summary>
    /// フルーツEnterイベント
    /// </summary>
    /// <param name="fruit">フルーツ</param>
    public void FruitEnter(Fruit fruit)
    {
        if (_SelectID != fruit.ID) return;

        if (fruit.IsSelect)
        {
            if (_SelectFruits.Count >= 2 && fruit.ID == _SelectFruits[_SelectFruits.Count - 2].ID)
            {
                var RemoveFruit = _SelectFruits[_SelectFruits.Count - 1];
                _SelectFruits.Remove(RemoveFruit);
                RemoveFruit.SetIsSelect(false);
            }
        }
        else
        {
            var Length = (_SelectFruits[_SelectFruits.Count - 1].transform.position - fruit.transform.position).magnitude;

            if (Length < FruitConnectRange)
            {
                _SelectFruits.Add(fruit);
                fruit.SetIsSelect(true);
            }
        }
    }

    /// <summary>
    /// フルーツUpイベント
    /// </summary>
    public void FruitUp()
    {
        if (_SelectFruits.Count >= FruitDestroyCount)
        {
            DestroyFruits(_SelectFruits);
        }
        else
        {
            foreach (var FruitItem in _SelectFruits)
            {
                FruitItem.SetIsSelect(false);
            }
        }

        _SelectID = "";
        _SelectFruits.Clear();
    }

    /// <summary>
    /// フルーツを消す
    /// </summary>
    /// <param name="fruits">消すフルーツ</param>
    private void DestroyFruits(List<Fruit> fruits)
    {
        foreach (var FruitItem in fruits)
        {
            Destroy(FruitItem.gameObject);
        }

        FruitSpawn(fruits.Count);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class LevelManaer : MonoBehaviour
{

    /// <summary>全てのフルーツ</summary>
    private List<Fruit> _AllFruits = new List<Fruit>();
    /// <summary>選択中のフルーツ</summary>
    private List<Fruit> _SelectFruits = new List<Fruit>();

    /// <summary>選択中のフルーツのID</summary>
    private string _SelectID = "";

    /// <summary>スコア</summary>
    private int _Score = 0;
    /// <summary>現在時間[s]</summary>
    private float _CurrentTime = 0;
    /// <summary>プレイ中状態</summary>
    private bool _IsPlaying = true;

    /// <summary>シングルトンインスタンス</summary>
    public static LevelManaer Instance { get; private set; }
    /// <summary>フルーツPrefabリスト</summary>
    public GameObject[] FruitPrefabs;
    /// <summary>選択線描画オブジェクト</summary>
    public LineRenderer LineRenderer;
    /// <summary>ボムPrefab</summary>
    public GameObject BombPrefab;
    /// <summary>スコア表示テキスト</summary>
    public TextMeshProUGUI ScoreText;
    /// <summary>時間表示テキスト</summary>
    public TextMeshProUGUI TimerText;
    /// <summary>ゲーム終了画面</summary>
    public GameObject FinishDialog;

    /// <summary>フルーツを消すために必要な数</summary>
    public int FruitDestroyCount = 3;
    /// <summary>フルーツをつなぐ範囲</summary>
    public float FruitConnectRange = 1.5f;
    /// <summary>ボムを生成するために必要なフルーツの数</summary>
    public int BombSpawnCount = 5;
    /// <summary>ボムで消す範囲</summary>
    public float BombDestroyRange = 1.5f;
    /// <summary>プレイ時間</summary>
    public float PlayTime = 60;

    void Start()
    {
        Instance = this;
        FruitSpawn(60);
        ScoreText.text = "0";
        _CurrentTime = PlayTime;

    }

    // Update is called once per frame
    void Update()
    {
        LineRendererUpdate();
        TimerUpdate();
    }

    /// <summary>
    /// 時間更新
    /// </summary>
    private void TimerUpdate()
    {
        if (_IsPlaying)
        {
            _CurrentTime -= Time.deltaTime;
            if (_CurrentTime <= 0)
            {
                _CurrentTime = 0;

                // ゲーム終了時に繋げていたフルーツを強制的に消す
                FruitUp();

                _IsPlaying = false;
                FinishDialog.SetActive(true);
            }
            TimerText.text = ((int)_CurrentTime).ToString();
        }
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
            var FruitObject = Instantiate(FruitPrefabs[Random.Range(0, FruitPrefabs.Length)], Position, Quaternion.identity);
            _AllFruits.Add(FruitObject.GetComponent<Fruit>());

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
        if (!_IsPlaying) return;

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
        if (!_IsPlaying) return;

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
        if (!_IsPlaying) return;
        // フルーツを3つ以上消した
        if (_SelectFruits.Count >= FruitDestroyCount)
        {
            if (_SelectFruits.Count >= BombSpawnCount)
            {
                Instantiate(BombPrefab, _SelectFruits[_SelectFruits.Count - 1].transform.position, Quaternion.identity);
            }
            DestroyFruits(_SelectFruits);
        }
        // フルーツを消した数が3つ未満なら、選択をキャンセル
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
    /// ボムを押した
    /// </summary>
    /// <param name="Bomb"></param>
    public void BombDown(Bomb Bomb)
    {
        if (!_IsPlaying) return;
        var RemoveFruits = _AllFruits.
        Where(
            fruit =>
            (Bomb.transform.position - fruit.transform.position).magnitude < BombDestroyRange
            ).ToList();

        DestroyFruits(RemoveFruits);
        Destroy(Bomb.gameObject);
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
            _AllFruits.Remove(FruitItem);
        }

        FruitSpawn(fruits.Count);
        AddScore(fruits.Count);
    }

    /// <summary>
    /// スコアを追加
    /// </summary>
    /// <param name="fruitCount">消したフルーツの数</param>
    private void AddScore(int fruitCount)
    {
        _Score += (int)(fruitCount * 100 * (1 + (fruitCount - 3) * 0.1f));
        ScoreText.text = _Score.ToString();

    }
}

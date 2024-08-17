using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    /// <summary>識別用ID</summary>
    public string ID;
    /// <summary>選択状態表示Sprite</summary>
    public GameObject SelectSprite;
    
    /// <summary>選択状態</summary>
    public bool IsSelect { get; private set; }
     
    /// <summary>
    /// MouseDownイベント
    /// </summary>
    private void OnMouseDown() 
    {
        LevelManaer.Instance.FruitDown(this);
    }
    /// <summary>
    /// MouseEnterイベント
    /// </summary>
    private void OnMouseEnter() {
        LevelManaer.Instance.FruitEnter(this);
    }
    /// <summary>
    /// MouseUpイベント
    /// </summary>
    private void OnMouseUp() {
        LevelManaer.Instance.FruitUp();
    }

    /// <summary>
    /// フルーツの選択状態を設定
    /// </summary>
    /// <param name="isSelect">選択状態</param>
    public void SetIsSelect(bool isSelect)
    {
        IsSelect = isSelect;
        SelectSprite.SetActive(isSelect);
    }

}

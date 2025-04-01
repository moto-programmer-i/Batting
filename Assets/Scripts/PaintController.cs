using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using System.IO;
using System;
using Unity.VisualScripting;
using UnityEngine.Assertions;

public class PaintController : MonoBehaviour
{
    /// <summary>
    /// ペンの幅
    /// </summary>
    static readonly int PEN_WIDTH = 10;
    private static readonly Vector2Int PEN = new Vector2Int(PEN_WIDTH, PEN_WIDTH);

    /// <summary>
    /// スイング軌道のJSONのファイル名
    /// </summary>
    static readonly string SWING_FILE_NAME = "swingPath.json";

    [SerializeField]
    private RawImage image;

    private Texture2D texture;

    /// <summary>
    /// 線を描く入力
    /// </summary>
    [SerializeField]
    private InputAction draw;

    /// <summary>
    /// 入力座標
    /// </summary>
    [SerializeField]
    private InputAction position;


    /// <summary>
    /// 3D空間でスイングのy成分の最大値
    /// </summary>
    [SerializeField]
    private float swingMaxY = 1;

    /// <summary>
    /// 3D空間でスイングのy成分の最小値
    /// </summary>
    [SerializeField]
    private float swingMinY = 0;


    /// <summary>
    /// 3D空間でスイングのy成分がとりうる値の幅
    /// </summary>
    private float swingYRange = 10;


    /// <summary>
    /// 前回の入力座標
    /// </summary>
    private Vector2Ex prePenPosition;

    /// <summary>
    /// スイング軌道を表す座標のリスト
    /// </summary>
    private List<Vector2Ex> swingPath = new ();

    /// <summary>
    /// Actionの有効を自分で管理するクラス
    /// </summary>
    private List<InputAction> actions = new ();

    

    /// <summary>
    /// 基となるスイング軌道のJSONのファイル名
    /// </summary>
    static readonly string BASE_SWING_RESOURCE_NAME = "Animations/BaseSwing";

    /// <summary>
    /// 基となるスイングのJSON
    /// </summary>
    private static AnimationCurveJson baseCurve;

    [SerializeField]
    private GameObject swingDrawerCanvas;

    [SerializeField]
    private BatController batController;

    void Start()
    {
        // ファイルからスイング読み込み
        if (baseCurve == null) {
            baseCurve = ResourceUtils.LoadJson<AnimationCurveJson>(BASE_SWING_RESOURCE_NAME);
        }

        // スイング描画終了時
        draw.canceled += context => {
            // スイング変更ボタンを押してから、離した後もここにきてしまうので対処
            if (ListUtils.IsEmpty(swingPath)){
                return;
            }

            // 最後の点を保存する
            swingPath.Add(Vector2Ex.From(position.ReadValue<Vector2>()));
            
            try {
                // 最初と最後が同じ点ならばスイングを作れないので、無視
                if (ListUtils.IsEmpty(swingPath) || swingPath.First().Equals(swingPath.Last())) {
                    return;
                }

                // リストの順番と傾きを設定
                Vector2Ex.SortDesc(swingPath);
                Vector2Ex.SetSlope(swingPath);
                
                // 保存
                AnimationCurveJson swingPathJson = SwingPathToJson(swingPath);
                FileUtils.SaveJson(swingPathJson, SWING_FILE_NAME);

                // スイング設定
                batController.SetSwing(swingPathJson);
            }
            finally {
                // スイングを初期化
                swingPath.Clear();
                Destroy(texture);
                prePenPosition = null;
                ShowSwingDrawer(false);
            }
        };

        // スイングの高さに関する初期化
        Assert.IsTrue(swingMaxY > swingMinY, "スイングの高さの最大値と最低値が不正");
        swingYRange = swingMaxY - swingMinY;
    }
    
    /// <summary>
    /// スイング変更画面表示
    /// </summary>
    /// <param name="visible"></param>
    public void ShowSwingDrawer(bool visible)
    {
        // 内部のキャンバスだけを表示
        Canvas canvas = swingDrawerCanvas.GetComponent<Canvas>();
        canvas.enabled = visible;

        if (visible) {
            // 画像の用意、enabledをtrueにした後でなければうまく実行されない
            Rect rect = image.gameObject.GetComponent<RectTransform>().rect;

            // これのデフォルトが半透明の画像なのか？詳細は不明だが、都合は良いのでこのままいく
            // TextureFormat.ARGB32だとエラーになるので注意
            texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);

            image.texture = texture;
        }
    }

    void Update()
    {
        // 画面が表示されてなければ何もしない
        if (!swingDrawerCanvas.GetComponent<Canvas>().enabled) {
            return;
        }
        
        OnDraw();
    }

    public void OnDraw()
    {
        // 入力がなければ何もしない
        if (draw == null || position == null ) {
            return;
        }

        // 押されてなければなにもしない
        if (!draw.IsInProgress()) {
            return;
        }

        AddPenPosition();
 
        texture.Apply();
    }

    private void AddPenPosition() {
        // 描画中の点
        Vector2Ex penPosition = Vector2Ex.From(position.ReadValue<Vector2>());
        drawPoint(penPosition.Position);       

        try {
            // 前回の入力がなければ最初の点として保存
            if (prePenPosition == null) {
                swingPath.Add(penPosition);
                return;
            }
            
            // 前回の入力があれば線を描く
            VectorUtils.withLerpPoints(prePenPosition.Position, penPosition.Position, drawPoint);    

            // x座標が同じであれば無視
            if (penPosition.Position.x == prePenPosition.Position.x) {
                return;
            }

            swingPath.Add(penPosition);            
        }
        // 描画した点を、前回の座標として保存
        finally {
            prePenPosition = penPosition;
        }
    }

    public void drawPoint(Vector2 center) {
        Vector2Int min = Vector2Int.RoundToInt(center - PEN / 2);
        Vector2Int max = min + PEN;
        
        // 参考
        // https://qiita.com/maple-bitter/items/290ba820cffb8c97834f
        for(int x = min.x; x < max.x; ++x) {
            for(int y = min.y; y < max.y; ++y) {
                texture.SetPixel( x, y, Color.black);
            }
        }
    }

     void OnEnable()
    {
        // アクションが登録されていなければ登録する
        if (!actions.Any()) {
            if (draw != null) {
                actions.Add(draw);
            }
            
            if (position != null) {
                actions.Add(position);
            }
        }
        
        actions.ForEach(action => action.Enable());
    }

    void OnDisable()
    {
        actions.ForEach(action => action.Disable());
    }

    AnimationCurveJson SwingPathToJson(List<Vector2Ex> swingPath)
    {
        AnimationCurveJson json = new ();
        // 2Dから3D上のスイング軌道に変換

        // 描かれたスイングの情報
        float firstSwingX = swingPath.First().Position.x;
        float swingWidth = Math.Abs(firstSwingX - swingPath.Last().Position.x);

        // 元のスイングの情報
        int baseCount = baseCurve.Keyframes.Count;
        int baseLastIndex = baseCount - 1;
        float baseTime = baseCurve.Keyframes.Last().Time;

        // スイングの最初の高さを設定
        json.Keyframes.Add(CreateAnimationKeyframe(baseCurve.Keyframes[0], swingPath[0].Position.y));

        // 2番目以降の高さを描かれた点に応じて設定していく
        for(int baseIndex = 1, swingPathIndex = 1; baseIndex < baseLastIndex; ++baseIndex){
            // 元のスイングの割合
            float basePercent = baseCurve.Keyframes[baseIndex].Time / baseTime;

            // 元のスイングの点に対応する、描かれたスイングの点を決定
            float swingPercent = 0;
            for(;swingPercent < basePercent; ++swingPathIndex) {
                swingPercent = MathF.Abs(swingPath[swingPathIndex].Position.x - firstSwingX) / swingWidth;
            }

            // 1つ先の点まで行ってしまっているので、直前の点が必要
            int preSwingPathIndex = swingPathIndex - 1;

            // 直前の点の高さを設定、傾きがわかる場合は間の高さを補正
            float y = swingPath[preSwingPathIndex].Position.y;
            if (!NumberUtils.IsNaN(swingPath[preSwingPathIndex].Slope)) {
                y += (swingPercent - basePercent) * swingWidth * swingPath[preSwingPathIndex].Slope.Value; 
            }
             
            json.Keyframes.Add(CreateAnimationKeyframe(baseCurve.Keyframes[baseIndex], y));
        }

        // スイングの最後の高さを設定する
        json.Keyframes.Add(CreateAnimationKeyframe(baseCurve.Keyframes.Last(), swingPath.Last().Position.y));

        return json;
    }

    /// <summary>
    /// AnimationKeyframeを作成
    /// </summary>
    /// <param name="baseFrame">元のAnimationKeyframe</param>
    /// <param name="y2d">高さ計算用の2Dのy（baseFrameにPosition自体がない場合は無視）</param>
    /// <returns></returns>
    AnimationKeyframe CreateAnimationKeyframe(AnimationKeyframe baseFrame, float y2d) {
        AnimationKeyframe newFrame = baseFrame.Clone();
        if(newFrame.Position != null) {
            newFrame.Position.Y = ConvertSwingY2Dto3D(y2d);
        }
        return newFrame;
    }

    /// <summary>
    /// スイング軌道のY成分を2Dから3Dに変換
    /// </summary>
    /// <param name="y2d">2Dでのy成分（0～Screen.height）</param>
    /// <returns>3Dでのy成分</returns>
    float ConvertSwingY2Dto3D(float y2d)
    {
        // 2D y:0～Screen.height → swingMinY～swingMaxYに変換
        return y2d / Screen.height * swingYRange + swingMinY;
    }
}

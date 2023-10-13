using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using System.IO;
using System;
using Unity.VisualScripting;

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
    /// スイングを保存するときの点の最小の傾きの違い
    /// </summary>
    [SerializeField]
    private float swingMinSlopeDiff = 0.5f;

    /// <summary>
    /// 3D空間でスイングのy成分の最低値
    /// </summary>
    [SerializeField]
    private float swingMinY = 3;
    /// <summary>
    /// 3D空間でスイングのy成分がとりうる値の幅
    /// </summary>
    [SerializeField]
    private float swingYRange = 10;


    /// <summary>
    /// 前回の入力座標
    /// </summary>
    private Vector2Ex prePosition;

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

    /// <summary>
    /// 基となるスイングのインパクトの添え字
    /// </summary>
    private static int baseSwingImpactIndex;

    

    // Start is called before the first frame update
    void Start()
    {
        // ファイルからスイング読み込み
        if (baseCurve == null) {
            baseCurve = ResourceUtils.LoadJson<AnimationCurveJson>(BASE_SWING_RESOURCE_NAME);
        }
        

        // 画像の用意
        Rect rect = image.gameObject.GetComponent<RectTransform>().rect;
        texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        image.texture = texture;

        // スイング描画終了時
        draw.canceled += context => {
            // 保存
            FileUtils.SaveJson(SWING_FILE_NAME, SwingPathToJson(swingPath));

            // スイングを初期化
            swingPath.Clear();
            prePosition = null;
        };

        Debug.Log(FileUtils.GetCurrentDirectory());
    }

    // Update is called once per frame
    void Update()
    {
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

        addPenPosition();
 
        texture.Apply();
    }

    private void addPenPosition() {
        Vector2Ex penPosition = Vector2Ex.From(position.ReadValue<Vector2>(), prePosition);
        drawPoint(penPosition.Position);

        // 前回の入力がなければ最初の点として保存
        if (prePosition == null) {
            swingPath.Add(penPosition);
        }
        else {
            // 前回の入力があれば線を描く
            VectorUtils.withLerpPoints(prePosition.Position, penPosition.Position, drawPoint);

            // x座標が同じであれば無視
            if (penPosition.Position.x == prePosition.Position.x) {
                return;
            }

            // 前回の傾きがなければスイング軌道に追加
            else if (!prePosition.Slope.HasValue) {
                swingPath.Add(penPosition);
            }
            
            // 傾きが閾値より変わっていればスイング軌道に追加
            else if (swingMinSlopeDiff  <= Math.Abs(penPosition.Slope.Value - prePosition.Slope.Value)) {
                swingPath.Add(penPosition);
            }
        }

        // 前回の座標を保存
        prePosition = penPosition;
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
        // インパクトから逆順に追加していく
        int baseSwingIndex = baseCurve.ImpactIndex;
        int swingPathIndex = swingPath.Count - 1;
        json.Keyframes.Add(ToAnimationKeyframe(
                    baseCurve.Keyframes[baseSwingIndex],
                    ConvertSwingY2Dto3D(swingPath[swingPathIndex].Position.y),
                    SwingType.IMPACT));
        
        // トップの位置からインパクトまでの前後の距離を保存
        float swingWidth = swingPath.First().Position.x - swingPath.Last().Position.x;

        // スイングの段階を都合上MAXで初期化
        // 最初だけ必ず失敗する無駄な判定が入るが、2回目以降はこの方がきれいに動作する
        float swingPercent = 0;
        while(true) {
            // 次の点へ
            --baseSwingIndex;
            if (baseSwingIndex < 0) {
                break;
            }

            // 基のスイングの割合から、描かれたスイングの高さを調整する
            float baseSwingPercent = (baseCurve.Keyframes[baseCurve.ImpactIndex].Time - baseCurve.Keyframes[baseSwingIndex].Time) / baseCurve.TimeToImpact;
            while(true) {
                if (baseSwingPercent <= swingPercent) {
                    break;
                }
                --swingPathIndex;

                
                swingPercent = (swingPath[swingPathIndex].Position.x - swingPath.Last().Position.x) / swingWidth;
                
                // x座標が同じ点が入ってしまった場合など、スイングの割合が計算出来なくなってしまった場合は1にする
                if (float.NaN.Equals(swingPercent)) {
                    swingPercent = 1;
                }
            }
            
            // baseSwingの点をswingPathの点が超えているので、1つ前を使う
            int preSwingPathIndex = swingPathIndex + 1;
            float y = swingPath[preSwingPathIndex].Position.y + ((baseSwingPercent - swingPercent) * swingWidth * swingPath[preSwingPathIndex].Slope.Value); 

            // List.Prependが存在せず、エラーもでなかった
            // json.Keyframes.Prepend(ToAnimationKeyframe(
            json.Keyframes.Add(ToAnimationKeyframe(
                    baseCurve.Keyframes[baseSwingIndex],
                    ConvertSwingY2Dto3D(y)));
        }
        json.Keyframes.Reverse();
        
        // フォロースルーを追加（とりあえずフォロースルーの高さはインパクトと同じ）
        float lastHeight = json.Keyframes.Last().Position.Y;
        for(baseSwingIndex = baseCurve.ImpactIndex + 1; baseSwingIndex < baseCurve.Keyframes.Count; ++baseSwingIndex) {
            json.Keyframes.Add(ToAnimationKeyframe(
                    baseCurve.Keyframes[baseSwingIndex],
                    lastHeight));
        }
        return json;
    }

    static AnimationKeyframe ToAnimationKeyframe(AnimationKeyframe baseFrame, float y, SwingType? type = null) {
        return new AnimationKeyframe(
                baseFrame.Time,
                baseFrame.Position != null ? new Vector3Data(baseFrame.Position.X, y, baseFrame.Position.Z) : null,
                baseFrame.Rotation,
                type
                );
    }

    /// <summary>
    /// スイング軌道のY成分を2Dから3Dに変換
    /// </summary>
    /// <param name="y2d">2Dでのy成分（0～Screen.height）</param>
    /// <returns>3Dでのy成分</returns>
    float ConvertSwingY2Dto3D(float y2d)
    {
        // 2D y:0～Screen.height → MIN_Y～(MIN_Y + RANGE_Y)に変換
        return y2d / Screen.height * swingYRange + swingMinY;
    }
}

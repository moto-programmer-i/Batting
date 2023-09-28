using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

public class PaintController : MonoBehaviour
{
    /// <summary>
    /// ペンの幅
    /// </summary>
    static readonly int PEN_WIDTH = 10;
    private static readonly Vector2Int PEN = new Vector2Int(PEN_WIDTH, PEN_WIDTH);

    /// <summary>
    /// スイングを保存するときの点同士の最小マンハッタン距離をだすための分割数
    /// </summary>
    public static readonly int SWING_POINT_SPRIT_FOR_MIN_DISTANCE = 10;

    /// <summary>
    /// スイングを保存するときの点同士の最小マンハッタン距離
    /// </summary>
    private static int SWING_POINT_MIN_DISTANCE;

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
    /// 前回の入力座標
    /// </summary>
    private Vector2 prePosition = Vector2.zero;

    /// <summary>
    /// スイング軌道を表す座標のリスト
    /// </summary>
    private List<Vector2> swingPath = new ();

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

    static readonly float SWING_MIN_Y = 3;
    static readonly float SWING_Y_RANGE = 10;

    // Start is called before the first frame update
    void Start()
    {
        // スイングの最小距離を設定
        SWING_POINT_MIN_DISTANCE = Screen.width / SWING_POINT_SPRIT_FOR_MIN_DISTANCE;
        

        // ファイルからスイング読み込み
        if (baseCurve == null) {
            baseCurve = ResourceUtils.LoadJson<AnimationCurveJson>(BASE_SWING_RESOURCE_NAME);
            baseCurve.initImpactIndex();
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
            // prePosition = null; // nullをいれられない
            prePosition = Vector2.zero;
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
        Vector2 penPosition = position.ReadValue<Vector2>();
        drawPoint(penPosition);

        // 前回の入力がなければ最初の点として保存
        if (prePosition == Vector2.zero) {
            swingPath.Add(penPosition);
        }
        else {
            // 前回の入力があれば線を描く
            VectorUtils.withLerpPoints(prePosition, penPosition, drawPoint);

            // 最小距離より離れていればスイング軌道に追加
            if (SWING_POINT_MIN_DISTANCE  < VectorUtils.ManhattanDistance(penPosition, swingPath.Last())) {
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

    static AnimationCurveJson SwingPathToJson(List<Vector2> swingPath)
    {
        AnimationCurveJson json = new ();
        // 2Dから3D上のスイング軌道に変換
        // インパクトから逆順に追加していく
        int baseSwingIndex = baseCurve.ImpactIndex;
        int swingPathIndex = swingPath.Count - 1;
        json.keyframes.Add(ToAnimationKeyframe(
                    baseCurve.keyframes[baseSwingIndex],
                    ConvertSwingY2Dto3D(swingPath[swingPathIndex].y),
                    SwingType.IMPACT));
        
        // swingPathの最初の点を活かせない可能性がある、要調整
        for(--swingPathIndex; swingPathIndex >= 0 && baseSwingIndex >= 0; --swingPathIndex,--baseSwingIndex) {
            // List.Prependが存在せず、エラーもでなかった
            // json.keyframes.Prepend(ToAnimationKeyframe(
            json.keyframes.Add(ToAnimationKeyframe(
                    baseCurve.keyframes[baseSwingIndex],
                    ConvertSwingY2Dto3D(swingPath[swingPathIndex].y)));
        }
        json.keyframes.Reverse();
        
        // フォロースルーを追加（とりあえずフォロースルーの高さはインパクトと同じ）
        float lastHeight = json.keyframes.Last().position.y;
        for(baseSwingIndex = baseCurve.ImpactIndex + 1; baseSwingIndex < baseCurve.keyframes.Count; ++baseSwingIndex) {
            json.keyframes.Add(ToAnimationKeyframe(
                    baseCurve.keyframes[baseSwingIndex],
                    lastHeight));
        }
        return json;
    }

    static AnimationKeyframe ToAnimationKeyframe(AnimationKeyframe baseFrame, float y, SwingType type = SwingType.DEFAULT) {
        return new AnimationKeyframe(
                baseFrame.time,
                new Vector3(baseFrame.position.x, y, baseFrame.position.z),
                baseFrame.rotation,
                type
                );
    }

    /// <summary>
    /// スイング軌道のY成分を2Dから3Dに変換
    /// </summary>
    /// <param name="y2d">2Dでのy成分（0～Screen.height）</param>
    /// <returns>3Dでのy成分</returns>
    static float ConvertSwingY2Dto3D(float y2d)
    {
        // 2D y:0～Screen.height → MIN_Y～(MIN_Y + RANGE_Y)に変換
        return y2d / Screen.height * SWING_Y_RANGE + SWING_MIN_Y;
    }
}

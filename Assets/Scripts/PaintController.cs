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
    /// スイングを保存するときの点同士の最小マンハッタン距離
    /// </summary>
    static readonly int SWING_POINT_MIN_DISTANCE = 100;

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

    // Start is called before the first frame update
    void Start()
    {
        // 画像の用意
        Rect rect = image.gameObject.GetComponent<RectTransform>().rect;
        texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        image.texture = texture;

        // スイング描画終了時
        draw.canceled += context => {
            // 保存
            FileUtils.SaveJson(SWING_FILE_NAME, swingPathToJson(swingPath));

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
 
        texture.Apply();
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

    static AnimationCurveJson swingPathToJson(List<Vector2> swingPath)
    {
        AnimationCurveJson json = new ();
        swingPath.ForEach(point => {
            // 2Dから3D上のスイング軌道に変換
            // キーフレームと変換をかく
            json.keyframes.Add(new AnimationKeyframe(0, new Vector3(point.x, point.y, 0), Vector3.zero));
        });
        return json;
    }
}

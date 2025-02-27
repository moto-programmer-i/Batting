using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BatController : MonoBehaviour
{
    public static readonly string DEFAULT_SWING_FILENAME = "swingPath.json";

    public static readonly string SWING_CLIPNAME = "Swing";

    public Camera mainCamera; 

    /// <summary>
    /// スイングの入力
    /// </summary>
    public InputAction swing;

    /// <summary>
    /// 入力位置
    /// </summary>
    public InputAction position;

    /// <summary>
    /// バットのカメラからの距離
    /// </summary>
    public float distance = 10;

    /// <summary>
    /// Actionの有効を自分で管理するクラス
    /// </summary>
    private List<InputAction> actions = new ();

    private Animator animator;

    [SerializeField]
    private float amplifier = 1.0f;

    [SerializeField]
    private Transform bat;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        // ファイルからスイング読み込み
        AnimationCurveJson curve = FileUtils.LoadJson<AnimationCurveJson>(DEFAULT_SWING_FILENAME);

        // バットの位置に合わせて補正
        // curve.Offset(bat.transform);

        SetSwing(curve);
    }

    public void SetSwing(AnimationCurveJson curve)
    {
        AnimationClipLoader.setClip(curve, SWING_CLIPNAME, animator);
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse current = Mouse.current;
        // if (current == null) {
        //     return;
        // }
        // if (current.leftButton.wasPressedThisFrame) {
        //     current
        // }
        
        Swing();
    }

    void Swing() {
        // スイングの入力がなければ何もしない
        if (swing == null || position == null ) {
            return;
        }
        if (!swing.triggered) {
            return;
        }

        Vector2 screenPosition = position.ReadValue<Vector2>();
        if (screenPosition == null) {
            return;
        }
        if (mainCamera == null) {
            return;
        }

        // 押した位置にバットを移動
        // this.transform.position = CameraUtils.ScreenToWorldPoint(mainCamera, screenPosition, distance);

        // スイング
        animator.SetTrigger(AnimatorConstants.SWING);
    }

    void OnEnable()
    {
        // アクションが登録されていなければ登録する
        if (!actions.Any()) {
            if (swing != null) {
                actions.Add(swing);
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

    void OnCollisionExit(Collision collision)
    {
        // ボールと当たっていなければ無視
        if(!GameObjectTags.BALL.Equals(collision.gameObject.tag)) {
            return;
        }
        
        // 加速
        collision.rigidbody.velocity *= amplifier;
        
        // まだ当たっていないなら飛距離を出力
        var ballEvent = collision.gameObject.GetComponent<BallEventTrigger>();
        if (!ballEvent.Hit) {
            ballEvent.Hit = true;
            Debug.Log("予想飛距離: " + BattingInstances.GetDistanceManager().CalcDistanceMeter(collision.rigidbody));
        }

        
    }

    public void SetAmplifier(float amplifier)
    {
        this.amplifier = amplifier;
    }
}

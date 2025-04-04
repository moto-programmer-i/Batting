using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class BatController : MonoBehaviour
{
    const string DEFAULT_SWING_FILENAME = "Animations/defaultSwingPath";
    const string SAVED_SWING_FILENAME = "swingPath.json";

    const string SWING_CLIPNAME = "Swing";

    /// <summary>
    /// クリップが最初の状態に戻らないので、ダミー用
    /// </summary>
    const string SWING_DEFAULT_CLIPNAME = "SwingDefault";

    public Camera mainCamera; 

    /// <summary>
    /// スイングの入力
    /// </summary>
    public InputAction swing;
    /// <summary>
    /// Actionの有効を自分で管理するクラス
    /// </summary>
    private List<InputAction> actions = new ();

    // WebGLだと動作しなかった
    // private Animator animator;
    private Animation swingAnimation;

    [SerializeField]
    private float amplifier = 1.0f;

    [SerializeField]
    private Transform bat;

    [SerializeField]
    private DistanceManager distanceManager;

    [SerializeField]
    private SaveDataManager saveDataManager;

    [SerializeField]
    private AudioSource se;

    private BatSetting currentBat;

    /// <summary>
    /// スイング終了後、フォロースルーで停止する時間
    /// </summary>
    [SerializeField]
    private float swingStopTime = 1.0f;

    /// <summary>
    /// スイング終了後、構えに戻る時間
    /// </summary>
    [SerializeField]
    private float swingRewindTime = 0.1f;


    void Start()
    {
        // WebGLだと動作しなかった
        // animator = GetComponent<Animator>();
        swingAnimation = GetComponent<Animation>();

        // 保存されたスイング読み込み
        AnimationCurveJson curve;
        try {
            curve = FileUtils.LoadJson<AnimationCurveJson>(SAVED_SWING_FILENAME);
        }
        // なければデフォルトのスイングを使用
        catch(FileNotFoundException e) {
            _ = e;
            curve = ResourceUtils.LoadJson<AnimationCurveJson>(DEFAULT_SWING_FILENAME);
        }
        SetSwing(curve);

        // スイング入力設定
        swing.performed += context => Swing(context);
    }

    public void SetSwing(AnimationCurveJson curve)
    {
        // WebGLだと動作しなかった
        // AnimationClipLoader.setClip(curve, SWING_CLIPNAME, animator);

        AnimationClipLoader.setClip(curve, SWING_CLIPNAME, swingAnimation);

        // 最初の状態に戻らないので、無理やり戻す用のダミー
        AnimationCurveJson swingDefault = new ();
        AnimationKeyframe end = curve.Keyframes.Last().Clone();
        end.Time = swingStopTime;
        swingDefault.Keyframes.Add(end);
        swingDefault.Keyframes.Add(new (transform, swingStopTime + swingRewindTime));
        AnimationClipLoader.setClip(swingDefault, SWING_DEFAULT_CLIPNAME, swingAnimation);
    }

    

    void Swing(InputAction.CallbackContext context) {
        // スイング
        // WebGLだと動作しなかった
        // animator.SetTrigger(AnimatorConstants.SWING);

        
        swingAnimation.Play(AnimatorConstants.SWING);

        // 最初の状態に戻らないので無理やり戻す
        swingAnimation.PlayQueued(SWING_DEFAULT_CLIPNAME);
    }

    void OnEnable()
    {
        // アクションが登録されていなければ登録する
        if (!actions.Any()) {
            if (swing != null) {
                actions.Add(swing);
            }
            
        }
        
        actions.ForEach(action => action.Enable());
    }

    void OnDisable()
    {
        actions.ForEach(action => action.Disable());
    }

    void OnCollisionEnter(Collision collision)
    {
        // ボールと当たっていなければ無視
        if(!GameObjectTags.BALL.Equals(collision.gameObject.tag)) {
            return;
        }

        // すでにバットに当たっているなら無視
        var ballEvent = collision.gameObject.GetComponent<BallEventTrigger>();
        if (ballEvent.Hit) {
            return;
        }
        ballEvent.Hit = true;

        // ２度打ちにならないよう、レイヤーを変える
        // 参考 http://kan-kikuchi.hatenablog.com/entry/ChangeLayer
        collision.gameObject.layer = BallEventTrigger.FLYING_BALL_LAYER;
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
        try {
            float meter = MathF.Round(distanceManager.CalcDistanceMeter(collision.rigidbody));

            // 飛距離を表示
            distanceManager.ShowDistance(meter);

            // 最大飛距離更新処理
            distanceManager.UpdateMaxMeter(meter);

            // 効果音再生
            PlayHitSound(meter);
            
        // 飛距離が計算できなかった場合
        } catch(ArgumentException e) {
            // Debug.LogException(e);
            _ = e;

            // 一応当たってはいるので音を出す
            PlayHitSound(0);
        }
    }

    public void SetAmplifier(float amplifier)
    {
        this.amplifier = amplifier;
    }

    public void SetCurrentBat(BatSetting currentBat)
    {
        this.currentBat = currentBat;
    }

    public void PlayHitSound(float meter) {
        if (currentBat == null || currentBat.Hit == null || currentBat.Hardhit == null) {
            return;
        }

        // ジャストミートの距離ならジャストミートとする
        if (meter >= currentBat.HardHitMeter) {
            currentBat.Hardhit.Play(se);
            return;
        }

        // それ以外なら根っこ
        currentBat.Hit.Play(se);
    }
}

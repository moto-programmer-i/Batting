using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SwingDrawer : MonoBehaviour
{
    public const string SwingDrawerDisplayButtonName = "swing-drawer-button";

    [SerializeField]
    private UIDocument home;

    private Button swingDrawerDisplayButton;

    [SerializeField]
    private PaintController paintController;

    void Awake()
    {
        // スイング変更画面表示ボタン初期化
        swingDrawerDisplayButton = home.rootVisualElement.Q<Button>(SwingDrawerDisplayButtonName);
        swingDrawerDisplayButton.clicked += () => paintController.ShowSwingDrawer(true);
    }

    
}

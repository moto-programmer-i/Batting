using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SwingDrawer : MonoBehaviour
{
    public const string SwingDrawerDisplayButtonName = "swing-drawer-button";

    [SerializeField]
    private UIDocument swingDrawer;

    private Button swingDrawerDisplayButton;

    [SerializeField]
    private PaintController paintController;


    
    void Awake()
    {
        // バットリスト表示ボタン初期化
        swingDrawerDisplayButton = swingDrawer.rootVisualElement.Q<Button>(SwingDrawerDisplayButtonName);
        swingDrawerDisplayButton.clicked += () => paintController.ShowSwingDrawer(true);
    }

    
}

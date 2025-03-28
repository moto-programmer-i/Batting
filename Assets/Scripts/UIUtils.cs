using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIUtils
{
    public const int DOUBLE_CLICK = 2;
    public static bool IsDoubleClick(ClickEvent e)
    {
        return e != null && e.clickCount >= DOUBLE_CLICK;
    }
}

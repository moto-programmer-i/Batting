package batting_center.utils.dialog;

import android.app.AlertDialog;
import android.content.Context;
import java.util.ArrayList;
import java.util.List;

// 参考
// https://indie-du.com/entry/2016/07/14/200000
// https://docs.unity3d.com/ja/2022.3/Manual/AndroidJavaSourcePlugins.html


public class DialogUtils {
    private static AlertDialog dialog;
    /**
     * 表示待ちのタイトル
     */
    private static List<String> titles = new ArrayList<>();
    /**
     * 表示待ちのメッセージ
     */
    private static List<String> messages = new ArrayList<>();

    /**
     * ダイアログを表示
     */
    public static void showDialog(Context context, String title, String message) {
        if (dialog == null) {
            dialog = new AlertDialog.Builder(context)
                .setOnDismissListener(dialog -> {
                    // 表示待ちがあれば表示する
                    if (titles.isEmpty()) {
                        return;
                    }
                    DialogUtils.dialog.setTitle(titles.remove(0));
                    DialogUtils.dialog.setMessage(messages.remove(0));
                    DialogUtils.dialog.show();
                })
                .create();
        }

        // ダイアログ表示中の場合は待つ
        if (dialog.isShowing()) {
            titles.add(title);
            messages.add(message);
            return;
        }
        dialog.setTitle(title);
        dialog.setMessage(message);
        dialog.show();
    }
}
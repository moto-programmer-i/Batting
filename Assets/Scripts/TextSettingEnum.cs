public enum TextSettingEnum
{
    Normal = 0,
    Homerun = 100
}


public static class TextSettingEnumUtils {
    public static TextSettingEnum Of(this int distance) {
        if(distance >= (int)TextSettingEnum.Homerun) {
            return TextSettingEnum.Homerun;
        }
        return TextSettingEnum.Normal;
    }

}

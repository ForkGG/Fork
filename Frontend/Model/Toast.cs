using ForkFrontend.Model.Enums;

namespace ForkFrontend.Model;

public class Toast
{
    public Toast(ToastLevel level, string text)
    {
        Level = level;
        Text = text;
    }

    public Toast(ToastLevel level, string text, TimeSpan? hideDuration)
    {
        Level = level;
        Text = text;
        HideDuration = hideDuration;
    }

    public ToastLevel Level { get; set; }
    public string Text { get; set; }
    public TimeSpan? HideDuration { get; set; }

    public override string ToString()
    {
        return $"({Level}) {Text}";
    }
}
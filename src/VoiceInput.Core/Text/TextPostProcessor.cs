namespace VoiceInput.Core.Text;

public static class TextPostProcessor
{
    public static string Process(string text)
    {
        return CollapseSpacesAroundChinesePunctuation(text.Trim());
    }

    private static string CollapseSpacesAroundChinesePunctuation(string text)
    {
        foreach (var punctuation in new[] { "，", "。", "？", "！", "：", "；", "、", "）" })
        {
            text = text.Replace(" " + punctuation, punctuation, StringComparison.Ordinal);
        }

        text = text.Replace("（ ", "（", StringComparison.Ordinal);
        return text;
    }
}

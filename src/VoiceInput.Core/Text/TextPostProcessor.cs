namespace VoiceInput.Core.Text;

public static class TextPostProcessor
{
    private static readonly (string From, string To)[] Replacements =
    [
        ("换行", Environment.NewLine),
        ("新的一行", Environment.NewLine),
        ("空格", " "),
        ("逗号", "，"),
        ("句号", "。"),
        ("问号", "？"),
        ("感叹号", "！"),
        ("叹号", "！"),
        ("冒号", "："),
        ("分号", "；"),
        ("顿号", "、"),
        ("左括号", "（"),
        ("右括号", "）"),
    ];

    public static string Process(string text)
    {
        var result = text.Trim();
        foreach (var (from, to) in Replacements)
        {
            result = result.Replace(from, to, StringComparison.Ordinal);
        }

        return CollapseSpacesAroundChinesePunctuation(result);
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

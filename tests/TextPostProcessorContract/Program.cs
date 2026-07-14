using VoiceInput.Core.Text;

var naturalSentence = "这个空格不对，逗号和句号也不能被替换。";
if (TextPostProcessor.Process(naturalSentence) != naturalSentence)
{
    throw new InvalidOperationException("Natural speech words were replaced.");
}

var commandWords = "换行、空格、逗号、句号";
if (TextPostProcessor.Process(commandWords) != commandWords)
{
    throw new InvalidOperationException("Command-like words must remain literal text.");
}

var spacedPunctuation = " 你好 ，世界 。 ";
if (TextPostProcessor.Process(spacedPunctuation) != "你好，世界。")
{
    throw new InvalidOperationException("Whitespace cleanup around Chinese punctuation failed.");
}

Console.WriteLine("text-post-processor-contract=pass");

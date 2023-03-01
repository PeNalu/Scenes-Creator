using UnityEngine;
using Voxell;
using Voxell.NLP.SentenceDetect;
using Voxell.Inspector;
using System.IO;

public class NLPSentenceSplitter : MonoBehaviour
{
    [StreamingAssetFilePath] public string splitterModel;
    [TextArea(1, 5)] public string paragraph;
    [TextArea(1, 3)] public string[] sentences;

    private EnglishMaximumEntropySentenceDetector sentenceDetector;

    [Button]
    void SplitSentence()
    {
        sentenceDetector = new EnglishMaximumEntropySentenceDetector(Path.Combine(Application.streamingAssetsPath, splitterModel));
        sentences = sentenceDetector.SentenceDetect(paragraph);
    }

    public string[] GetSentence(string text)
    {
        sentenceDetector = new EnglishMaximumEntropySentenceDetector(Path.Combine(Application.streamingAssetsPath, splitterModel));
        sentences = sentenceDetector.SentenceDetect(text);
        return sentences;
    }
}

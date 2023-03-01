using UnityEngine;
using Voxell;
using Voxell.NLP.Tokenize;
using Voxell.Inspector;
using Voxell.NLP.Parser;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class NLPTokenizer : MonoBehaviour
{
    [StreamingAssetFilePath] public string tokenizerModel;
    [TextArea(1, 5)] public string sentence;
    public string[] tokens;

    private EnglishMaximumEntropyTokenizer tokenizer;

    [Button]
    public void Tokenize()
    {
        tokenizer = new EnglishMaximumEntropyTokenizer(Path.Combine(Application.streamingAssetsPath, tokenizerModel));
        tokens = tokenizer.Tokenize(sentence);
    }
}

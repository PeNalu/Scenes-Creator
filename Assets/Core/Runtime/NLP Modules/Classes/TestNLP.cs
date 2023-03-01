using NLP;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class TestNLP : MonoBehaviour
{
    // Stored required properties.
    private fsSerializer serializer = new fsSerializer();
    private static List<PartOfSpeech> partsOfSpeech;
    private TextData textData;
    private Text text;

    private void Start()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Resources/Output.txt");
        Deserialize(path);
        MakeText();
    }

    private void MakeText()
    {
        text = new Text(new List<Sentence>());
        ReadData();

        for (int i = 0; i < textData.datas.Length; i++)
        {
            List<Token> tokens = new List<Token>();
            List<KeyValuePair<string, PartOfSpeech>> parts = new List<KeyValuePair<string, PartOfSpeech>>();
            for (int j = 0; j < textData.datas[i].tokenDatas.Length; j++)
            {
                string value = textData.datas[i].tokenDatas[j].value;
                PartOfSpeech partOfSpeech = partsOfSpeech.Where(x => x.abbreviation == textData.datas[i].tokenDatas[j].part).FirstOrDefault();
                KeyValuePair<string, PartOfSpeech> keyValuePair = new KeyValuePair<string, PartOfSpeech>(value, partOfSpeech);

                tokens.Add(new Token(value));
                parts.Add(keyValuePair);
            }
            Sentence sentence = new Sentence(tokens, parts);
            text.sentences.Add(sentence);
        }
    }

    public Text GetText()
    {
        return text;
    }

    private static void ReadData()
    {
        partsOfSpeech = new List<PartOfSpeech>();
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Resources/PartsOfSpeech.txt");
        string[] lines = File.ReadLines(path).ToArray();

        for (int i = 0; i < lines.Length; i++)
        {
            string[] words = lines[i].Split("|");
            words[0] = words[0].Replace(" ", "");

            PartOfSpeech partOfSpeech = new PartOfSpeech(words[0], words[1], words[2]);
            partsOfSpeech.Add(partOfSpeech);
        }
    }

    private void Deserialize(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            string json = reader.ReadLine();

            if (!string.IsNullOrEmpty(json))
            {
                fsData data = fsJsonParser.Parse(json);

                object deserialized = null;
                serializer.TryDeserialize(data, typeof(TextData), ref deserialized).AssertSuccessWithoutWarnings();

                textData = (TextData)deserialized;
            }
        }
    }

    public class TextData
    {
        public SentenceData[] datas { get; set; }
    }

    public class SentenceData
    {
        public TokenData[] tokenDatas { get; set; }
    }

    public class TokenData
    {
        public string part { get; set; }
        public string value { get; set; }
    }

    #region [Getter / Setter]
    public TextData GetTextData()
    {
        return textData;
    }
    #endregion
}

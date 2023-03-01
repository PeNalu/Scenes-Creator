using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace NLP.UIModules
{
    public class TextProcessing : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private TextMeshProUGUI outputField;

        [SerializeField]
        private TestNLP desirealizer;

        [SerializeField]
        private List<KeyColor> keyColors;

        public void Processing()
        {
            StringBuilder resultString = new StringBuilder();
            Text text = desirealizer.GetText();
            List<Sentence> sentences = text.sentences;

            for (int i = 0; i < sentences.Count; i++)
            {
                for (int j = 0; j < sentences[i].parts.Count; j++)
                {
                    KeyValuePair<string, PartOfSpeech> keyValuePair = sentences[i].parts[j];
                    KeyColor keyColor = keyColors.Where(x => x.GetKey() == keyValuePair.Value.abbreviation).FirstOrDefault();
                    if(keyColor != null)
                    {
                        string part = $"<color={keyColor.GetColor()}> {keyValuePair.Key}</color>";
                        resultString.Append(part);
                    }
                    else
                    {
                        string toAdd = keyValuePair.Key == "." ? "." : " " + keyValuePair.Key;
                        resultString.Append(toAdd);
                    }
                }
            }

            outputField.text = resultString.ToString();
        }
    }
}

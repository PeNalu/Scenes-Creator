using UnityEngine;
using Voxell;
using Voxell.NLP.Parser;
using Voxell.Inspector;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NLPParser : MonoBehaviour
{
    [StreamingAssetFolderPath] public string parserModel;

    [SerializeField]
    private NLPNaiveBayesClassifier classifier;

    [SerializeField]
    private NLPSentenceSplitter sentenceSplitter;

    [SerializeField]
    private List<string> positionKeys;

    [TextArea]
    public string textToParse;

    //Stored required properties.
    private List<TextEntity> textEntities;
    private List<string> interactiveEntity;
    private List<string> grabbableEntity;

    private string lastEntity = "";
    private string lastPosition = "";
    private string lastNumeral = "";

    public List<TextEntity> Parse(string text)
    {
        textToParse = text;
        return Parse();
    }

    private List<TextEntity> Parse()
    {
        string text = ClassifySentances();

        lastEntity = null;
        lastPosition = null;
        textEntities = new List<TextEntity>();
        EnglishTreebankParser parser = new EnglishTreebankParser(Path.Combine(Application.streamingAssetsPath, "Models\\"), true, false);
        Parse result = parser.DoParse(text);
        Parse[] a = result.GetTagNodes();

        foreach (Parse parse in a)
        {
            if (positionKeys.Contains(parse.Head.ToString()))
            {
                lastPosition = parse.Head.ToString();
            }

            if (parse.Type == "CD" || parse.Type == "LS")
            {
                lastNumeral = parse.Head.ToString();
            }

            if (parse.Type == "NN")
            {
                if (positionKeys.Contains(parse.Head.ToString())) continue;

                if (!string.IsNullOrEmpty(lastEntity) && !string.IsNullOrEmpty(lastPosition))
                {
                    TextEntity entity = GetEntity(lastEntity);
                    entity.parentObj = parse.Head.ToString();
                    entity.position = lastPosition;

                    if (!string.IsNullOrEmpty(lastNumeral))
                    {
                        entity.count = int.Parse(lastNumeral);
                        lastNumeral = "";
                    }

                    lastEntity = null;
                    lastPosition = null;
                }

                lastEntity = parse.Head.ToString();
                TextEntity textEntity = GetEntity(parse.Head.ToString());
                if (textEntity == null)
                {
                    textEntity = new TextEntity();
                    textEntity.name = parse.Head.ToString().ToLower();
                    textEntities.Add(textEntity);
                }
            }
        }

        foreach (string item in interactiveEntity)
        {
            TextEntity entity = GetEntity(item);
            if (entity != null)
                entity.interactable = true;
        }

        foreach (string item in grabbableEntity)
        {
            TextEntity entity = GetEntity(item);
            if (entity != null)
                entity.grabbable = true;
        }

        return textEntities;
        //creator.GenerateRoom(textEntities);
    }

    private string ClassifySentances()
    {
        interactiveEntity = new List<string>();
        grabbableEntity = new List<string>();

        StringBuilder stringBuilder = new StringBuilder();
        string[] sentences = sentenceSplitter.GetSentence(textToParse);
        foreach (string sentence in sentences)
        {
            string classStr = classifier.Classify(sentence);
            if (classStr == "InteractQuery" || classStr == "GrabbableQuery")
            {
                EnglishTreebankParser pars = new EnglishTreebankParser(Path.Combine(Application.streamingAssetsPath, "Models\\"), true, false);
                Parse res = pars.DoParse(sentence);
                Parse[] tags = res.GetTagNodes();

                List<TextEntity> entities = new List<TextEntity>();
                foreach (Parse item in tags)
                {
                    if (item.Type == "NN")
                    {
                        if (classStr == "InteractQuery")
                        {
                            if (!interactiveEntity.Contains(item.Head.ToString()))
                            {
                                interactiveEntity.Add(item.Head.ToString());
                            }
                        }
                        else
                        {
                            if (!grabbableEntity.Contains(item.Head.ToString()))
                            {
                                grabbableEntity.Add(item.Head.ToString());
                            }
                        }
                    }
                }
            }
            else
            {
                stringBuilder.Append(sentence);
            }
        }

        return stringBuilder.ToString();
    }

    private TextEntity GetEntity(string name)
    {
        name = name.ToLower();
        return textEntities.Where(x => x.name == name).FirstOrDefault();
    }
}

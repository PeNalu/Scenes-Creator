using System.Collections.Generic;

namespace NLP
{
    public class Text
    {
        public List<Sentence> sentences;

        public Text(List<Sentence> sentences)
        {
            this.sentences = sentences;
        }

        public void AddSentance(Sentence sentence)
        {
            sentences.Add(sentence);
        }
    }
}

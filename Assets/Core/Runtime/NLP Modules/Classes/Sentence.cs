using System;
using System.Collections.Generic;
using System.Linq;

namespace NLP
{
    public class Sentence
    {
        public List<Token> sentance;
        public List<KeyValuePair<string, PartOfSpeech>> parts;

        public Sentence(List<Token> sentance)
        {
            this.sentance = sentance;
        }

        public Sentence(string text, List<KeyValuePair<string, PartOfSpeech>> parts)
        {
            this.sentance = new List<Token>();
            string[] tokens = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            for (int j = 0; j < tokens.Length; j++)
            {
                AddToken(new Token(tokens[j]));
            }

            AddToken(new Token("."));

            this.parts = parts;
        }

        public Sentence(List<Token> tokens, List<KeyValuePair<string, PartOfSpeech>> parts)
        {
            this.sentance = tokens;
            this.parts = parts;
        }

        public void AddToken(Token token)
        {
            sentance.Add(token);
        }

        public string[] GetSentence()
        {
            return sentance.Select(x => x.token).ToArray();
        }
    }
}

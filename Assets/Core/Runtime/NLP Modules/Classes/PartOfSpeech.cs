namespace NLP
{
    public struct PartOfSpeech
    {
        public string abbreviation;
        public string nameEU;
        public string nameRU;

        public PartOfSpeech(string abbreviation, string nameEU, string nameRU)
        {
            this.abbreviation = abbreviation;
            this.nameEU = nameEU;
            this.nameRU = nameRU;
        }
    }
}

using UnityEngine;

namespace NLP.UIModules
{
    [System.Serializable]
    public class KeyColor 
    {
        [SerializeField]
        private string key;

        [SerializeField]
        private string color;

        #region [Getter / Setter]
        public string GetKey()
        {
            return key;
        }

        public string GetColor()
        {
            return color;
        }
        #endregion
    }
}

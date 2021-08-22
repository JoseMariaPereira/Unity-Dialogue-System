using UnityEngine;
using System.Collections.Generic;

namespace FlyingCrow.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        [SerializeField] private string uniqueID;
        [SerializeField] private string text;
        [SerializeField] private List<string> children = new List<string>();
        [SerializeField] private Rect position;

        public string GetUniqueID()
        {
            return uniqueID;
        }

        public void SetUniqueID(string uniqueID)
        {
            this.uniqueID = uniqueID;
        }

        public string GetText()
        {
            return text;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public Rect GetPosition()
        {
            return position;
        }

    }
}

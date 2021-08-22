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

        public string GetText()
        {
            return text;
        }
    }
}

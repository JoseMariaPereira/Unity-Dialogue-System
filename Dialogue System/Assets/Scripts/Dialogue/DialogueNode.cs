using UnityEngine;
using System.Collections.Generic;

namespace FlyingCrow.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        [SerializeField] private string uniqueID;
        [SerializeField] private string text;
        [SerializeField] private Rect rect = new Rect(0, 0, 400, 100); 
        [SerializeField] private List<string> children = new List<string>();

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

        public Rect GetRect()
        {
            return rect;
        }

        public void SetRectPosition(Vector2 position)
        {
            rect.position = position;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public void AddChild(DialogueNode child)
        {
            children.Add(child.GetUniqueID());
        }

        public void RemoveChild(DialogueNode child)
        {
            children.Remove(child.GetUniqueID());
        }

    }
}

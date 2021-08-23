using UnityEngine;
using System.Collections.Generic;

namespace FlyingCrow.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private string text;
        [SerializeField] private Rect rect = new Rect(0, 0, 400, 120); 
        [SerializeField] private List<string> children = new List<string>();

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
            children.Add(child.name);
        }

        public void RemoveChild(DialogueNode child)
        {
            children.Remove(child.name);
        }

        public bool ContainsChild(DialogueNode child)
        {
            return children.Contains(child.name);
        }

    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace FlyingCrow.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool playerDecision = false;
        [SerializeField] private string text = "";
        [SerializeField] private Rect rect = new Rect(0, 0, 400, 120); 
        [SerializeField] private List<string> children = new List<string>();

        public string GetText()
        {
            return text;
        }

        public Rect GetRect()
        {
            return rect;
        }

        public List<string> GetChildren()
        {
            return children;
        }
        public bool ContainsChild(DialogueNode child)
        {
            return children.Contains(child.name);
        }
        public bool IsPlayer()
        {
            return playerDecision;
        }

#if UNITY_EDITOR
        public void SetText(string text)
        {
            if (!text.Equals(this.text))
            {
                Undo.RecordObject(this, "Dialogue Content");
                this.text = text;
                EditorUtility.SetDirty(this);
            }
        }

        public void SetRectPosition(Vector2 position)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = position;
            EditorUtility.SetDirty(this);
        }

        public void AddChild(DialogueNode child)
        {
            Undo.RecordObject(this, "Add Dialogue Child");
            children.Add(child.name);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(DialogueNode child)
        {
            Undo.RecordObject(this, "Remove Dialogue Child");
            children.Remove(child.name);
            EditorUtility.SetDirty(this);
        }
#endif

    }
}

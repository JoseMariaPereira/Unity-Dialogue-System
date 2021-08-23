using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace FlyingCrow.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        private Dictionary<string, DialogueNode> nodeDictionary = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            if (nodes.Count == 0)
            {
                CreateNode(null);
            }
            OnValidate();
        }
#endif

        private void OnValidate()
        {
            nodeDictionary.Clear();
            foreach(DialogueNode node in GetAllNodes())
            {
                nodeDictionary.Add(node.name, node);
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (string child in parentNode.GetChildren())
            {
                if (nodeDictionary.ContainsKey(child))
                {
                    yield return nodeDictionary[child];
                }
            }
        }

        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            if (parent != null)
            {
                parent.AddChild(newNode);
                Vector2 position = parent.GetRect().position 
                    + new Vector2 (parent.GetRect().width * 0.5f, parent.GetRect().height * 1.1f);
                newNode.SetRectPosition(position);
            }
            nodes.Add(newNode);
            nodeDictionary.Add(newNode.name, newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            nodeDictionary.Remove(nodeToDelete.name);
            RemoveChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void RemoveChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete);
            }
        }
    }
}

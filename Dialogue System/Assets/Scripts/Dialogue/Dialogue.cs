using UnityEngine;
using System.Collections.Generic;
using System;

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
                DialogueNode rootNode = new DialogueNode();
                rootNode.SetUniqueID(Guid.NewGuid().ToString());
                nodes.Add(rootNode);
            }
            OnValidate();
        }
#endif

        private void OnValidate()
        {
            nodeDictionary.Clear();
            foreach(DialogueNode node in GetAllNodes())
            {
                nodeDictionary.Add(node.GetUniqueID(), node);
                 
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
            DialogueNode newNode = new DialogueNode();
            newNode.SetUniqueID(Guid.NewGuid().ToString());
            parent.AddChild(newNode);
            nodes.Add(newNode);
            nodeDictionary.Add(newNode.GetUniqueID(), newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            nodeDictionary.Remove(nodeToDelete.GetUniqueID());
            RemoveChildren(nodeToDelete);
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

using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace FlyingCrow.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        private Dictionary<string, DialogueNode> nodeDictionary = new Dictionary<string, DialogueNode>();

        private void OnValidate()
        {
            if (nodes.Count == 0)
            {
                CreateNode(null);
            }

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

#if UNITY_EDITOR
        public DialogueNode CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateNewNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Add Dialogue Node");
            AddNewNode(newNode);
            return newNode;
        }

        private void AddNewNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            nodeDictionary.Add(newNode.name, newNode);
        }

        private static DialogueNode CreateNewNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent != null)
            {
                parent.AddChild(newNode);
                Vector2 position = parent.GetRect().position
                    + new Vector2(parent.GetRect().width * 0.5f, parent.GetRect().height * 1.1f);
                newNode.SetRectPosition(position);
            }

            return newNode;
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {

            Undo.RecordObject(this, "Remove Dialogue Node");
            nodes.Remove(nodeToDelete);
            nodeDictionary.Remove(nodeToDelete.name);
            if (nodes.Count == 0)
            {
                CreateNode(null).SetRectPosition(nodeToDelete.GetRect().position);
            }
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
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (!AssetDatabase.GetAssetPath(this).Equals(""))
            {
                if (nodes.Count == 0)
                {
                    AddNewNode(CreateNewNode(null));
                }

                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node).Equals(""))
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}

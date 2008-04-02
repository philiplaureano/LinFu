using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.Reflection.Extensions;
using System.Xml.Linq;

namespace LinFu.MxClone.Extensions
{
    public static class MetaNodeExtensions
    {
        public static IEnumerable<IMetaNode> Ancestors(this IMetaNode node)
        {
            var results = new List<IMetaNode>();
            var currentNode = node.Parent;
            while (currentNode != null)
            {
                results.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            return results;
        }
        public static void ApplyDescent(this IEnumerable<IMetaNode> nodes, Action<IMetaNode> action)
        {
            // Apply the action to the entire list of subtrees
            nodes.ForEach(n => n.ApplyDescent(action));
        }
        public static void ApplyDescent(this IMetaNode node, Action<IMetaNode> action)
        {
            // Execute the action on the current node
            if (node != null)
                action(node);

            // Next, traverse the subtree
            foreach (var child in node.Children)
            {
                child.ApplyDescent(action);
            }
        }
        public static bool IsDescendantOf(this IMetaNode node, params IMetaNode[] otherNodes)
        {
            bool result = true;

            foreach (var currentNode in otherNodes)
            {
                result &= node.IsDescendantOf(currentNode);

                if (result == false)
                    break;
            }


            return result;
        }

        public static bool IsDescendantOf(this IMetaNode node, IMetaNode otherNode)
        {
            var currentNode = node.Parent;
            while (currentNode != null)
            {
                if (currentNode == otherNode)
                    return true;

                currentNode = currentNode.Parent;
            }

            return false;
        }
        public static IEnumerable<XAttribute> Attributes(this IMetaNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var element = node.TargetNode;

            if (element == null)
                return null;

            return element.Attributes();
        }
        public static XAttribute GetAttribute(this IMetaNode node,
            string localName, string namespaceName)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var element = node.TargetNode;
            var matches = (from a in element.Attributes()
                          where a.Name.LocalName == localName &&
                          a.Name.NamespaceName == namespaceName
                          select a).ToList();

            if (matches.Count == 0)
                return null;

            return matches[0];
        }
        public static bool HasAttribute(this IMetaNode node, 
            string localName, string namespaceName)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var element = node.TargetNode;

            var matches = (from a in element.Attributes()
                          where a.Name.LocalName == localName &&
                          a.Name.NamespaceName == namespaceName
                          select a).Count();


            return matches > 0;
        }
        public static string LocalName(this IMetaNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var name = node.TargetNode.Name;

            return name.LocalName;
        }
        public static string NamespaceName(this IMetaNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var name = node.TargetNode.Name;

            return name.NamespaceName;
        }
        public static bool HasSameNameAs(this XAttribute attribute, XAttribute other)
        {
            var attributeName = attribute.Name;
            var otherName = other.Name;

            return attributeName.LocalName == otherName.LocalName &&
                attributeName.NamespaceName == otherName.NamespaceName;
        }
        public static int MaxDepth(this IList<IMetaNode> targetList)
        {
            int maxDepth = 0;
            foreach (var item in targetList)
            {
                maxDepth = item.Depth > maxDepth ? item.Depth : maxDepth;
            }

            return maxDepth;
        }
        public static void BuildNodeList(this XElement element, IList<IMetaNode> targetList)
        {
            Traverse(0, null, element, targetList);
        }

        private static void Traverse(int depth, IMetaNode parent,
            XElement element, IList<IMetaNode> targetList)
        {
            IMetaNode currentNode = new MetaNode()
            {
                TargetNode = element,
                Depth = depth,
                Parent = parent
            };

            // Traverse the child nodes
            var children = from n in element.Nodes()
                           where n is XElement
                           select n as XElement;

            if (parent != null)
                parent.Children.Add(currentNode);

            foreach (var child in children)
            {
                Traverse(depth + 1, currentNode, child, targetList);
            }

            if (targetList == null)
                return;

            targetList.Add(currentNode);
        }
    }    
}

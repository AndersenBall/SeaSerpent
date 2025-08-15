using System;
using System.Collections.Generic;
using System.Linq;

namespace GerneralScripts.DialogSystem
{
    public class DialogManager
    {
        private Dictionary<string, DialogNode> _nodes;
        private DialogNode? _currentNode;

        public DialogManager(List<DialogNode> nodes, string startId)
        {
            _nodes = nodes.ToDictionary(n => n.Id);
            GoToNode(startId);
        }

        public void GoToNode(string nodeId)
        {
            if (_nodes.TryGetValue(nodeId, out var node) && (node.Prerequisite?.Invoke() ?? true))
            {
                _currentNode = node;
                node.OnEnter?.Invoke();
            }
            else
            {
                _currentNode = null;
            }
        }

        public void Display()
        {
            if (_currentNode == null)
            {
                Console.WriteLine("End of dialog.");
                return;
            }

            Console.WriteLine(_currentNode.NpcLine);

            var availableChoices = _currentNode.Choices
                .Where(c => c.Prerequisite?.Invoke() ?? true)
                .ToList();

            for (int i = 0; i < availableChoices.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {availableChoices[i].PlayerLine}");
            }

            // You can wire this up to UI later — for now simulate input:
            var input = Console.ReadLine();
            if (int.TryParse(input, out int index) && index > 0 && index <= availableChoices.Count)
            {
                var choice = availableChoices[index - 1];
                choice.OnSelect?.Invoke();
                GoToNode(choice.NextDialogId ?? "");
                Display();
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
    }

}
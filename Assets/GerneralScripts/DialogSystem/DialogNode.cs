using System;
using System.Collections.Generic;

namespace GerneralScripts.DialogSystem
{
    public class DialogNode
    {
        public string Id { get; set; }  // Unique identifier for linking
        public string NpcLine { get; set; }

        // Optional condition to determine if this node should be shown
        public Func<bool>? Prerequisite { get; set; }

        // Optional action to run when this node is entered (e.g., give gold)
        public Action? OnEnter { get; set; }

        public List<DialogChoice> Choices { get; set; } = new();
    }
}
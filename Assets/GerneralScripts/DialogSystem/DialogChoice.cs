using System;

namespace GerneralScripts.DialogSystem
{
    public class DialogChoice
    {
        public string PlayerLine { get; set; }

        // Optional condition to show this choice
        public Func<bool>? Prerequisite { get; set; }

        // Optional action when this choice is selected
        public Action? OnSelect { get; set; }

        // Next node to go to after selection
        public string? NextDialogId { get; set; }
    }
}
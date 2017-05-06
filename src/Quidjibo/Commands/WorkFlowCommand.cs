using System;
using System.Collections.Generic;
using System.Linq;

namespace Quidjibo.Commands
{
    public sealed class WorkflowCommand : IQuidjiboCommand
    {
        public int Step { get; set; }

        public int CurrentStep { get; set; }

        public Dictionary<int, List<IQuidjiboCommand>> Entries { get; set; }

        public WorkflowCommand()
        {
            Step = 0;
            CurrentStep = 0;
            Entries = new Dictionary<int, List<IQuidjiboCommand>>();
        }

        public WorkflowCommand(params IQuidjiboCommand[] commands) : this()
        {
            Validate(commands);
            AddEntries(commands);
        }

        public WorkflowCommand Then(Func<int, IQuidjiboCommand> commands)
        {
            Step += 1;
            var steps = new[]
            {
                commands(Step)
            };
            Validate(steps);
            AddEntries(steps);
            return this;
        }

        public WorkflowCommand Then(Func<int, IQuidjiboCommand[]> commands)
        {
            Step += 1;
            var steps = commands(Step);
            Validate(steps);
            AddEntries(steps);
            return this;
        }

        internal void NextStep()
        {
            CurrentStep += 1;
        }


        private void AddEntries(IEnumerable<IQuidjiboCommand> steps)
        {
            Entries.Add(Step, steps.ToList());
        }

        private void Validate(IEnumerable<IQuidjiboCommand> commands)
        {
            if (commands.Any(command => command is WorkflowCommand))
            {
                throw new InvalidOperationException("Workflow commands cannot be nested.");
            }
        }
    }
}
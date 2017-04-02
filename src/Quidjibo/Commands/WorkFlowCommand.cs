using System;
using System.Collections.Generic;
using System.Linq;

namespace Quidjibo.Commands
{
    public sealed class WorkflowCommand : IWorkCommand
    {
        public int Step { get; set; }

        public int CurrentStep { get; set; }

        public Dictionary<int, List<IWorkCommand>> Entries { get; set; }

        public WorkflowCommand()
        {
            Step = 0;
            CurrentStep = 0;
            Entries = new Dictionary<int, List<IWorkCommand>>();
        }

        public WorkflowCommand(params IWorkCommand[] commands) : this()
        {
            Validate(commands);
            AddEntries(commands);
        }

        public WorkflowCommand Then(Func<int, IWorkCommand> commands)
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

        public WorkflowCommand Then(Func<int, IWorkCommand[]> commands)
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


        private void AddEntries(IEnumerable<IWorkCommand> steps)
        {
            Entries.Add(Step, steps.ToList());
        }

        private void Validate(IEnumerable<IWorkCommand> commands)
        {
            if (commands.Any(command => command is WorkflowCommand))
            {
                throw new InvalidOperationException("Workflow commands cannot be nested.");
            }
        }
    }
}
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

        /// <summary>
        /// Construct a new Workflow that does not contain any steps.
        /// </summary>
        public WorkflowCommand()
        {
            Step = 0;
            CurrentStep = 0;
            Entries = new Dictionary<int, List<IQuidjiboCommand>>();
        }

        /// <summary>
        /// Construct a new Workflow with commands that will be run when the first step executes.
        /// </summary>
        /// <param name="commands"></param>
        public WorkflowCommand(params IQuidjiboCommand[] commands) : this()
        {
            Validate(commands);
            AddEntries(commands);
            Step += 1;
        }

        /// <summary>
        /// Then, adds another step with a single command to the workflow.
        /// </summary>
        /// <typeparam name="T">The type of IQuidjiboCommand</typeparam>
        /// <param name="commands">The </param>
        /// <returns></returns>
        public WorkflowCommand Then<T>(Func<int, T> commands)
            where T : IQuidjiboCommand
        {
            var steps = new[]
            {
                commands(Step)
            };
            Validate(steps);
            AddEntries(steps);
            Step += 1;
            return this;
        }

        /// <summary>
        /// Then, adds another step with muliple commands to the workflow.
        /// </summary>
        /// <typeparam name="T">The type of IQuidjiboCommand</typeparam>
        /// <param name="commands">The commands that should be run in parallel when this step executes.</param>
        /// <returns></returns>
        public WorkflowCommand Then<T>(Func<int, T[]> commands)
            where T : IQuidjiboCommand
        {
            var steps = commands(Step);
            Validate(steps);
            AddEntries(steps);
            Step += 1;
            return this;
        }

        /// <summary>
        /// Increment the Current Step by One
        /// </summary>
        public void NextStep()
        {
            CurrentStep += 1;
        }

        private void AddEntries<T>(IEnumerable<T> steps)
            where T : IQuidjiboCommand
        {
            Entries.Add(Step, steps.OfType<IQuidjiboCommand>().ToList());
        }

        private void Validate<T>(IEnumerable<T> commands)
            where T : IQuidjiboCommand
        {
            if (commands.Any(command => command is WorkflowCommand))
            {
                throw new InvalidOperationException("Workflow commands cannot be nested.");
            }
        }
    }
}
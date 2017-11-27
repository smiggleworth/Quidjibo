using System.Collections.Generic;
using Quidjibo.Misc;
using Quidjibo.Models;
using Quidjibo.Pipeline.Misc;
using Quidjibo.Providers;

namespace Quidjibo.Pipeline.Contexts
{
    public interface IQuidjiboContext
    {
        /// <summary>
        ///     Gets or sets the progress.
        /// </summary>
        /// <value>
        ///     The progress.
        /// </value>
        IQuidjiboProgress Progress { get; set; }

        /// <summary>
        ///     Gets or sets the provider.
        /// </summary>
        /// <value>
        ///     The provider.
        /// </value>
        IWorkProvider Provider { get; set; }

        /// <summary>
        ///     Gets or sets the item.
        /// </summary>
        /// <value>
        ///     The item.
        /// </value>
        WorkItem Item { get; set; }

        /// <summary>
        ///     Gets the state.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        PipelineState State { get; set; }
    }
}
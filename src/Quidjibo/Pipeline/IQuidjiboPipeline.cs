// // Copyright (c) smiggleworth. All rights reserved.
// // Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Pipeline.Contexts;

namespace Quidjibo.Pipeline
{
    public interface IQuidjiboPipeline
    {
        /// <summary>
        ///     Start the pipeline
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartAsync(IQuidjiboContext context, CancellationToken cancellationToken);

        /// <summary>
        ///     Invoke the next step.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InvokeAsync(IQuidjiboContext context, CancellationToken cancellationToken);

        /// <summary>
        ///     End the pipeline
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task EndAsync(IQuidjiboContext context);
    }
}
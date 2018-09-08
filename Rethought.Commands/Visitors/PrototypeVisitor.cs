﻿using Optional;
using Rethought.Commands.Actions;
using Rethought.Extensions.Optional;

namespace Rethought.Commands.Visitors
{
    public class PrototypeVisitor<TContext> : IVisitor<TContext>
    {
        private readonly IAsyncAction<TContext> asyncAction;

        public PrototypeVisitor(IAsyncAction<TContext> asyncAction)
        {
            this.asyncAction = asyncAction;
        }

        public IAsyncAction<TContext> Invoke(Option<IAsyncAction<TContext>> nextAsyncActionOption)
        {
            return nextAsyncActionOption.TryGetValue(out var nextAction)
                ? EnumeratingAsyncAction<TContext>.Create(asyncAction, nextAction)
                : asyncAction;
        }
    }
}
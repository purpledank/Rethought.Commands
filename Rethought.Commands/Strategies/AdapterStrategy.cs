﻿using System;
using Optional;
using Rethought.Commands.Actions;
using Rethought.Commands.Builder;
using Rethought.Commands.Parser;
using Rethought.Extensions.Optional;

namespace Rethought.Commands.Strategies
{
    public class AdapterStrategy<TContext, TCommandSpecificContext> : IStrategy<TContext>
    {
        private readonly ITypeParser<TContext, TCommandSpecificContext> parser;
        private readonly Action<AsyncActionBuilder<TCommandSpecificContext>> configuration;

        public AdapterStrategy(
            ITypeParser<TContext, TCommandSpecificContext> parser,
            Action<AsyncActionBuilder<TCommandSpecificContext>> configuration)
        {
            this.parser = parser;
            this.configuration = configuration;
        }

        public IAsyncAction<TContext> Invoke(Option<IAsyncAction<TContext>> nextAsyncActionOption)
        {
            var asyncActionBuilder = new AsyncActionBuilder<TCommandSpecificContext>();
            configuration.Invoke(asyncActionBuilder);

            var command = asyncActionBuilder.Build();

            var asyncContextSwitchDecorator =
                new ContextAdapterAsyncActionDecorator<TContext, TCommandSpecificContext>(parser, command);

            if (nextAsyncActionOption.TryGetValue(out var nextAsyncAction))
            {
                return EnumeratingAsyncAction<TContext>.Create(asyncContextSwitchDecorator, nextAsyncAction);
            }

            return asyncContextSwitchDecorator;
        }
    }
}
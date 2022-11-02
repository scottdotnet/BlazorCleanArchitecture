﻿using Microsoft.Extensions.Logging;

namespace BlazorCleanArchitecture.WebUI.GrpcTests.Helpers
{
    internal class ForwardingLoggerProvider : ILoggerProvider
    {
        private readonly LogMessage _logAction;

        public ForwardingLoggerProvider(LogMessage logAction)
        {
            _logAction = logAction;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ForwardingLogger(categoryName, _logAction);
        }

        public void Dispose()
        {
        }

        internal class ForwardingLogger : ILogger
        {
            private readonly string _categoryName;
            private readonly LogMessage _logAction;

            public ForwardingLogger(string categoryName, LogMessage logAction)
            {
                _categoryName = categoryName;
                _logAction = logAction;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null!;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                _logAction(logLevel, _categoryName, eventId, formatter(state, exception), exception);
            }
        }
    }
}

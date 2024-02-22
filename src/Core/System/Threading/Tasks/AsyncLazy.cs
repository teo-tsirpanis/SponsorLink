﻿// <auto-generated />
#region License
// MIT License
// 
// Copyright (c) Daniel Cazzulino
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Provides factory methods to create <see cref="AsyncLazy{T}"/> so that 
    /// the <c>T</c> can be inferred from the received value factory return type.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// <code>
    /// var lazy = AsyncLazy.Create(() => ...);
    /// 
    /// var value = await lazy.Value;
    /// </code>
    /// </remarks>
    static partial class AsyncLazy
    {
        /// <summary>
        /// Creates an <see cref="AsyncLazy{T}"/> using the given <paramref name="valueFactory"/>.
        /// </summary>
        public static AsyncLazy<T> Create<T>(Func<T> valueFactory) => new AsyncLazy<T>(valueFactory);

        /// <summary>
        /// Creates an <see cref="AsyncLazy{T}"/> using the given <paramref name="asyncValueFactory"/>.
        /// </summary>
        public static AsyncLazy<T> Create<T>(Func<Task<T>> asyncValueFactory) => new AsyncLazy<T>(asyncValueFactory);
    }

    /// <summary>
    /// A <see cref="Lazy{T}"/> that initializes asynchronously and whose 
    /// <see cref="Lazy{T}.Value"/> can be awaited for initialization completion.
    /// </summary>
    /// <remarks>
    /// Basically taken from https://devblogs.microsoft.com/pfxteam/asynclazyt/.
    /// Usage:
    /// <code>
    /// var lazy = new AsyncLazy&lt;T&gt;(() => ...);
    /// 
    /// var value = await lazy.Value;
    /// </code>
    /// </remarks>
    /// <typeparam name="T">The type of async lazily-initialized value.</typeparam>
    partial class AsyncLazy<T> : Lazy<Task<T>>
    {
        /// <summary>
        /// Initializes the lazy, using <see cref="Task.Run(Func{T})"/> to asynchronously 
        /// schedule the value factory execution.
        /// </summary>
        public AsyncLazy(Func<T> valueFactory) : base(() => Task.Run(valueFactory))
        { }

        /// <summary>
        /// Initializes the lazy, using <see cref="Task.Run(Func{Task{T}})"/> to asynchronously 
        /// schedule the value factory execution.
        /// </summary>
        public AsyncLazy(Func<Task<T>> asyncValueFactory) : base(() => Task.Run(() => asyncValueFactory()))
        { }

        /// <summary>
        /// Allows awaiting the async lazy directly.
        /// </summary>
        public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
        
        /// <summary>
        /// Gets a value indicating whether the value factory has been invoked and has run to completion.
        /// </summary>
        public bool IsValueFactoryCompleted => base.Value.IsCompleted;        
    }
}

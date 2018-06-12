using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jdx.ApplMangaUWP.Helpers {
    /// <summary>
    /// A helper class to assist with the Asynchronous class initialization pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncInitializerHelper<T> {
        private ManualResetEvent _initializeLock = new ManualResetEvent(false);
        private bool _hasInitialized;
        private bool _hasErrored;
        private Exception _initializationException;

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncInitializerHelper{T}"/> class.
        /// </summary>
        public AsyncInitializerHelper() {
        }

        internal void InitializeWith(Func<Task> initializationTask) {
            try {
                initializationTask().ContinueWith((e) => {
                    try {
                        if (e.Exception != null) {
                            _hasErrored = true;
                            _initializationException = e.Exception;
                        }
                    } finally {
                        _hasInitialized = true;
                        _initializeLock.Set();
                    }
                });
            } catch(Exception synchronousEx) {
                _hasErrored = true;
                _initializationException = synchronousEx;
            }
        }

        internal void CheckInitialized() {
            if(!_hasInitialized) {
                _initializeLock.WaitOne();
            }

            if(_hasErrored) {
                throw new Exception($"Initialization of {typeof(T).FullName} failed with an exception");
            }
        }
    }
}

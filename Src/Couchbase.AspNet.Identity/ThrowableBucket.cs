using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.IO;

namespace Couchbase.AspNet.Identity
{
    /// <summary>
    /// Wrapper for common <see cref="IBucket"/> CRUD methods; if an operation fails an exception will be thrown.
    /// </summary>
    public class ThrowableBucket : IDisposable
    {
        private IBucket _bucket;

        public ThrowableBucket(IBucket bucket)
        {
            _bucket = bucket;
        }

        /// <summary>
        /// Gets a document by key asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task<T> GetAsync<T>(string key)
        {
            var result = await _bucket.GetAsync<T>(key);
            if (result.Success)
            {
                return result.Value;
            }
            if (result.Status == ResponseStatus.KeyNotFound)
            {
                return default(T);
            }
            if (result.Exception != null)
            {
                // ReSharper disable once ThrowingSystemException
                throw result.Exception;
            }
            throw new CouchbaseException(result, key);
        }

        /// <summary>
        /// Updates a document asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="doc">The document.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task UpdateAsync<T>(string key, T doc)
        {
            var result = await _bucket.ReplaceAsync(key, doc);
            if (result.Success)
            {
                return;
            }
            if (result.Exception != null)
            {
                // ReSharper disable once ThrowingSystemException
                throw result.Exception;
            }
            throw new CouchbaseException(result, key);
        }

        /// <summary>
        /// Creates a document asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="doc">The document.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task CreateAsync<T>(string key, T doc)
        {
            var result = await _bucket.InsertAsync(key, doc);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException(result, key);
            }
        }

        /// <summary>
        /// Deletes a document asynchronously.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task DeleteAsync(string key)
        {
            var result = await _bucket.RemoveAsync(key);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException(result, key);
            }
        }

        /// <summary>
        /// Gets all documents for a given keyset asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">The keys.</param>
        public async Task<List<T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            var tasks = keys.Select(x => _bucket.GetAsync<T>(x));
            var results = await Task.WhenAll(tasks);
            return await Task.FromResult(results.Select(x => x.Value).ToList());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

#region [ License information          ]

/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2015 Couchbase, Inc.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/

#endregion

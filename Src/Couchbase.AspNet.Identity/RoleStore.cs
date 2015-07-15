
using System;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.N1QL;
using Microsoft.AspNet.Identity;

namespace Couchbase.AspNet.Identity
{

    /// <summary>
    /// Contains methods for creating, deleting, updating and retrieving roles.
    /// </summary>
    /// <typeparam name="T">The <see cref="IdentityRole"/> being retrieved or mutated.</typeparam>
    public class RoleStore<T> : IQueryableRoleStore<T> where T : IdentityRole
    {
        private IBucket _bucket;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleStore{T}"/> class.
        /// </summary>
        /// <param name="bucket">The bucket.</param>
        public RoleStore(IBucket bucket)
        {
            _bucket = bucket;
        }

        public IQueryable<T> Roles
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// Creates a role as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException"></exception>
        public async Task CreateAsync(T role)
        {
            var result = await _bucket.InsertAsync(role.Id, role);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException(result, role.Id);
            }
        }

        /// <summary>
        /// Deletes a role as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException"></exception>
        public async Task DeleteAsync(T role)
        {
            var result = await _bucket.RemoveAsync(role.Id);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException(result, role.Id);
            }
        }

        /// <summary>
        /// Finds the role by it's unique identifier (key).
        /// </summary>
        /// <param name="roleId">The key for the role.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException"></exception>
        public async Task<T> FindByIdAsync(string roleId)
        {
            var result = await _bucket.GetAsync<T>(roleId);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException(result, roleId);
            }
            return result.Value;
        }

        /// <summary>
        /// Finds the role by it's name.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>An <see cref="IdentityRole"/> matching the name property.</returns>
        /// <exception cref="CouchbaseException"></exception>
        /// <remarks>Finds and returns the first valid match. If no match is found, will throw a <see cref="CouchbaseException"/>.</remarks>
        public async Task<T> FindByNameAsync(string roleName)
        {
            var statement = "SELECT COUNT(*) FROM `" + _bucket.Name + "` WHERE type='role' AND name = '$1';";
            var query = new QueryRequest(statement)
                .AddPositionalParameter(roleName);

            var result = await _bucket.QueryAsync<T>(query);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException((IOperationResult) result, roleName);
            }
            return result.Rows[0];
        }

        /// <summary>
        /// Updates the role by it's unique identifier (key).
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException"></exception>
        public async Task UpdateAsync(T role)
        {
            var result = await _bucket.ReplaceAsync(role.Id, role);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException(result, role.Id);
            }
        }

        public void Dispose()
        {
            _bucket.Dispose();
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

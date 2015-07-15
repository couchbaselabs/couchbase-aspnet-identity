using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.N1QL;
using Microsoft.AspNet.Identity;

namespace Couchbase.AspNet.Identity
{
    /// <summary>
    /// Contains methods for adding, removing, updating and retrieving users from Couchbase.
    /// </summary>
    /// <typeparam name="T">The <see cref="IdentityUser"/> being retrieved or mutated.</typeparam>
    public class UserStore<T> : IUserLoginStore<T>,
        IUserClaimStore<T>,
        IUserRoleStore<T>,
        IUserSecurityStampStore<T>,
        IQueryableUserStore<T>,
        IUserPasswordStore<T>,
        IUserPhoneNumberStore<T>,
        IUserStore<T>,
        IUserLockoutStore<T, string>,
        IUserTwoFactorStore<T, string>,
        IUserEmailStore<T> where T : IdentityUser
    {
        private IBucket _bucket;

        public UserStore(IBucket bucket)
        {
            _bucket = bucket;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task CreateAsync(T user)
        {
            var result = await _bucket.InsertAsync(user.Id, user);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException(result, user.Id);
            }
        }

        /// <summary>
        /// Updates a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task UpdateAsync(T user)
        {
            var result = await _bucket.ReplaceAsync(user.Id, user);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException(result, user.Id);
            }
        }

        /// <summary>
        /// Deletes a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task DeleteAsync(T user)
        {
            var result = await _bucket.RemoveAsync(user.Id);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException(result, user.Id);
            }
        }

        /// <summary>
        /// Finds the user by it's id (key) asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task<T> FindByIdAsync(string userId)
        {
            var result = await _bucket.GetAsync<T>(userId);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException(result, userId);
            }
            return result.Value;
        }

        /// <summary>
        /// Finds a user by it's name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition that cannot be resolved.</exception>
        public async Task<T> FindByNameAsync(string userName)
        {
            var statement = "SELECT COUNT(*) FROM `" + _bucket.Name + "` WHERE type='user' AND name = '$1';";
            var query = new QueryRequest(statement)
                .AddPositionalParameter(userName);

            var result = await _bucket.QueryAsync<T>(query);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchbaseException((IOperationResult)result, userName);
            }
            return result.Rows[0];
        }

        public Task AddLoginAsync(T user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(T user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task AddClaimAsync(T user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(T user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task SetSecurityStampAsync(T user, string stamp)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSecurityStampAsync(T user)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Users
        {
            get { throw new NotImplementedException(); }
        }

        public Task SetPasswordHashAsync(T user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(T user, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPhoneNumberAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(T user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(T user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAccessFailedCountAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetLockoutEnabledAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(T user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(T user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(T user, string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(T user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByEmailAsync(string email)
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
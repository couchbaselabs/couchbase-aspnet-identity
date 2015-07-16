using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
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
            await UpdateUser(user);
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

        /// <summary>
        /// Sets the password hash for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task SetPasswordHashAsync(T user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            await UpdateUser(user);
        }

        /// <summary>
        /// Gets the password hash for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(T user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        /// Determines whether a user has a password hash asychronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(T user)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        /// <summary>
        /// Sets the phone number for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task SetPhoneNumberAsync(T user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            await UpdateUser(user);
        }

        /// <summary>
        /// Gets the phone number for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<string> GetPhoneNumberAsync(T user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Gets the <see cref="IdentityUser.PhoneNumberConfirmed"/> for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<bool> GetPhoneNumberConfirmedAsync(T user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Sets the <see cref="IdentityUser.PhoneNumberConfirmed"/> for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="confirmed">if set to <c>true</c> [confirmed].</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task SetPhoneNumberConfirmedAsync(T user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            await UpdateUser(user);
        }

        /// <summary>
        /// Gets the lockout end date for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<DateTimeOffset> GetLockoutEndDateAsync(T user)
        {
            var lockoutEndDateUtc = user.LockoutEndDateUtc;
            return Task.FromResult(lockoutEndDateUtc.HasValue ?
                new DateTimeOffset(DateTime.SpecifyKind(lockoutEndDateUtc.Value, DateTimeKind.Utc)) :
                new DateTimeOffset());
        }

        /// <summary>
        /// Sets the lockout end date asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="lockoutEnd">The lockout end.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task SetLockoutEndDateAsync(T user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            await UpdateUser(user);
        }

        /// <summary>
        /// Increments the access failed count for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task<int> IncrementAccessFailedCountAsync(T user)
        {
            user.AccessFailedCount++;
            await UpdateUser(user);
            return user.AccessFailedCount;
        }

        /// <summary>
        /// Resets the access failed count asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task ResetAccessFailedCountAsync(T user)
        {
            user.AccessFailedCount = 0;
            await UpdateUser(user);
        }

        /// <summary>
        /// Gets the access failed count asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<int> GetAccessFailedCountAsync(T user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Gets the lockout enabled flag for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<bool> GetLockoutEnabledAsync(T user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        /// <summary>
        /// Sets the lockout enabled flag for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task SetLockoutEnabledAsync(T user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            await UpdateUser(user);
        }

        public Task SetTwoFactorEnabledAsync(T user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(T user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the user's email asynchronously.
        /// </summary>
        /// <param name="user">The user's email.</param>
        /// <param name="email">The email to set to the user's identity.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task SetEmailAsync(T user, string email)
        {
           user.Email = email;
           await UpdateUser(user);
        }

        /// <summary>
        /// Gets the user's email asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(T user)
        {
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Gets the confirmed status of the user's email.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmedAsync(T user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Sets the user's email confirmed flag asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="confirmed">if set to <c>true</c> the user's email has been confirmed.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task SetEmailConfirmedAsync(T user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            var result = await _bucket.ReplaceAsync(user.Id, user);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    // ReSharper disable once ThrowingSystemException
                    throw result.Exception;
                }
                throw new CouchbaseException(result, user.Id);
            }
        }

        /// <summary>
        /// Finds the user by email asynchronously.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task<T> FindByEmailAsync(string email)
        {
            var statement = "SELECT * FROM `" + _bucket.Name + "` WHERE type='user' AND email = '$1';";
            var query = new QueryRequest(statement)
                .AddPositionalParameter(email);

            var result = await _bucket.QueryAsync<T>(query);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    // ReSharper disable once ThrowingSystemException
                    throw result.Exception;
                }
                throw new CouchbaseException((IOperationResult)result, email);
            }
            return result.Rows[0];
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        async Task UpdateUser(T user)
        {
            var result = await _bucket.ReplaceAsync(user.Id, user);
            if (!result.Success)
            {
                if (result.Exception != null)
                {
                    // ReSharper disable once ThrowingSystemException
                    throw result.Exception;
                }
                throw new CouchbaseException(result, user.Id);
            }
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
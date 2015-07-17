using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        private readonly ThrowableBucket _bucket;

        public UserStore(ThrowableBucket bucket)
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
            var tasks = new List<Task>
            {
                _bucket.CreateAsync(user.Email, user.Id),
                _bucket.CreateAsync(user.Id, user)
            };

            if (user.Email != user.UserName)
            {
              tasks.Add(_bucket.CreateAsync(user.UserName, user.Id));
            }
            await Task.WhenAll(tasks);
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
            await _bucket.UpdateAsync(user.Id, user);
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
            var tasks = new List<Task>
            {
                _bucket.DeleteAsync(user.UserName),
                _bucket.DeleteAsync(user.Email),
                _bucket.DeleteAsync(user.Id)
            };
            await Task.WhenAll(tasks);
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
            return await _bucket.GetAsync<T>(userId);
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
            var userId = await _bucket.GetAsync<string>(userName);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return default(T);
            }
            return await _bucket.GetAsync<T>(userId);
        }

        /// <summary>
        /// Adds the login to a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task AddLoginAsync(T user, UserLoginInfo login)
        {
            var adapter = new UserLoginInfoAdapter
            {
                LoginInfo = login,
                UserId = user.Id
            };

            var key = KeyFormats.GetKey(login.LoginProvider, login.ProviderKey);
            await _bucket.CreateAsync(key, adapter);

            user.UserLoginIds.Add(key);
            await _bucket.UpdateAsync(user.Id, user);
        }

        /// <summary>
        /// Removes a given login asynchronously
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public async Task RemoveLoginAsync(T user, UserLoginInfo login)
        {
            var key = KeyFormats.GetKey(login.ProviderKey, user.Id);
            await _bucket.DeleteAsync(key);

            user.UserLoginIds.Remove(key);
            await _bucket.UpdateAsync(user.Id, user);
        }

        /// <summary>
        /// Gets the login asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task<IList<UserLoginInfo>> GetLoginsAsync(T user)
        {
            return await _bucket.GetAllAsync<UserLoginInfo>(user.UserLoginIds);
        }

        /// <summary>
        /// Finds the login asynchronously.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public async Task<T> FindAsync(UserLoginInfo login)
        {
            var key = KeyFormats.GetKey(login.LoginProvider, login.ProviderKey);
            var adapter = await _bucket.GetAsync<UserLoginInfoAdapter>(key);
            return await _bucket.GetAsync<T>(adapter.UserId);

        }

        /// <summary>
        /// Gets all of the claims for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(T user)
        {
            return Task.FromResult((IList <Claim>)user.Claims);
        }

        /// <summary>
        /// Adds a claim to a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public Task AddClaimAsync(T user, Claim claim)
        {
            user.Claims.Add(claim);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Removes a claim from a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public Task RemoveClaimAsync(T user, Claim claim)
        {
            user.Claims.Remove(claim);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Adds a user to a role asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public Task AddToRoleAsync(T user, string roleName)
        {
            user.Roles.Add(new IdentityRole(roleName));
            return Task.FromResult(0);
        }

        /// <summary>
        /// Removes a role from a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public Task RemoveFromRoleAsync(T user, string roleName)
        {
            var role = user.Roles.FirstOrDefault(x => x.Name.Equals(roleName));
            if (role != null)
            {
                user.Roles.Remove(role);
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets all of the roles for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<IList<string>> GetRolesAsync(T user)
        {
           return Task.FromResult((IList<string>)user.Roles.Select(x => x.Name).ToList());
        }

        /// <summary>
        /// Determines whether the user has a role asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public Task<bool> IsInRoleAsync(T user, string roleName)
        {
            return Task.FromResult(user.Roles.Any(x => x.Name.Equals(roleName)));
        }

        /// <summary>
        /// Sets the security stamp asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="stamp">The stamp.</param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(T user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the security stamp asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<string> GetSecurityStampAsync(T user)
        {
            return Task.FromResult(user.SecurityStamp);
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
        public Task SetPasswordHashAsync(T user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
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
        public Task SetPhoneNumberAsync(T user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
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
        public Task SetPhoneNumberConfirmedAsync(T user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
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
        public Task SetLockoutEndDateAsync(T user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
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
            await _bucket.UpdateAsync(user.Id, user);
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
            await _bucket.UpdateAsync(user.Id, user);
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
        public Task SetLockoutEnabledAsync(T user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Sets a flag indicating two factor authentication is enabled for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="enabled">if set to <c>true</c> <see cref="IdentityUser.TwoFactorEnabled"/>enabled.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public Task SetTwoFactorEnabledAsync(T user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets a flag indicating two factor authentication is enabled for a user asynchronously.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<bool> GetTwoFactorEnabledAsync(T user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        /// <summary>
        /// Sets the user's email asynchronously.
        /// </summary>
        /// <param name="user">The user's email.</param>
        /// <param name="email">The email to set to the user's identity.</param>
        /// <returns></returns>
        /// <exception cref="CouchbaseException">All server responses other than Success.</exception>
        /// <exception cref="Exception">Any client error condition.</exception>
        public Task SetEmailAsync(T user, string email)
        {
           user.Email = email;
           return Task.FromResult(0);
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
        public Task SetEmailConfirmedAsync(T user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
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
            var userId = await _bucket.GetAsync<string>(email);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return default(T);
            }
            return await _bucket.GetAsync<T>(userId);
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
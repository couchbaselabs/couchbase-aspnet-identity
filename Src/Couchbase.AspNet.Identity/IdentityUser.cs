using System;
using Microsoft.AspNet.Identity;

namespace Couchbase.AspNet.Identity
{
    /// <summary>
    /// Represents a user on the system and properties associated with that user.
    /// </summary>
    public class IdentityUser : IUser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUser"/> class.
        /// </summary>
        public IdentityUser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUser"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        public IdentityUser(string username)
        {
            UserName = username;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the user; this is a key in couchbase.
        /// </summary>
        /// <value>
        /// The identifier used to uniquely identify the user.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
    }
}
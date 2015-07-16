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

        /// <summary>
        /// Gets or sets the email for the user.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user's email has been confirmed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the user's email has been confirmed; otherwise, <c>false</c>.
        /// </value>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the password hash for the user.
        /// </summary>
        /// <value>
        /// The password hash.
        /// </value>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the phone number for the user.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PhoneNumber"/> was confirmed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the  <see cref="PhoneNumber"/>confirmed; otherwise, <c>false</c>.
        /// </value>
        public bool PhoneNumberConfirmed { get; set; }
    }
}

using System;
using Couchbase.Core;
using Couchbase.IO;
using Moq;
using NUnit.Framework;

namespace Couchbase.AspNet.Identity.Tests
{
    [TestFixture]
    public class UserStoreTests
    {
        [Test]
        public void When_Success_CreateAsync_Does_Not_Throw_Exception()
        {
            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(true);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.InsertAsync(It.IsAny<string>(), It.IsAny<IdentityUser>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.CreateAsync(new IdentityUser(Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Not_Success_CreateAsync_Throws_CouchbaseException()
        {
            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(false);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.KeyExists);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.InsertAsync(It.IsAny<string>(), It.IsAny<IdentityUser>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.CreateAsync(new IdentityUser(Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Success_DeleteAsync_Does_Not_Throw_Exception()
        {
            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(true);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.RemoveAsync(It.IsAny<string>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.DeleteAsync(new IdentityUser(Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Not_Success_DeleteAsync_Throws_CouchbaseException()
        {
            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(false);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.RemoveAsync(It.IsAny<string>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.DeleteAsync(new IdentityUser(Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Success_UpdateAsync_Does_Not_Throw_Exception()
        {
            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(true);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<IdentityUser>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.UpdateAsync(new IdentityUser(Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Not_Success_UpdateAsync_Throws_CouchbaseException()
        {
            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(false);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.KeyNotFound);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<IdentityUser>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.UpdateAsync(new IdentityUser(Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_User_Does_Not_Exist_FindByIdAsync_Fails()
        {
            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(false);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.KeyNotFound);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.GetAsync<IdentityUser>(It.IsAny<string>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.FindByIdAsync(Guid.NewGuid().ToString()));
        }

        [Test]
        public void When_User_Exists_FindByIdAsync_Succeeds()
        {
            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(true);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.Success);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.GetAsync<IdentityUser>(It.IsAny<string>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.FindByIdAsync(Guid.NewGuid().ToString()));
        }

        [Test]
        public void When_User_Exists_SetEmailAsync_Succeeds()
        {
            var user = new IdentityUser("foo");
            var expectedEmail = "fo0@bar.com";

            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(true);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.Success);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<IdentityUser>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.SetEmailAsync(user, expectedEmail));
            Assert.AreEqual(expectedEmail, user.Email);
        }

        [Test]
        public void When_User_Does_Not_Exist_SetEmailAsync_Fails()
        {
            var user = new IdentityUser("foo");
            var expectedEmail = "fo0@bar.com";

            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(false);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.KeyNotFound);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<IdentityUser>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.SetEmailAsync(user, expectedEmail));
        }

        [Test]
        public void When_User_Exists_SetEmailConfirmedAsync_Succeeds()
        {
            var user = new IdentityUser("foo");
            const bool confirmed = true;

            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(true);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.Success);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<IdentityUser>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.SetEmailConfirmedAsync(user, confirmed));
            Assert.AreEqual(confirmed, user.EmailConfirmed);
        }

        [Test]
        public void When_User_Does_Not_Exist_SetEmailConfirmedAsync_Fails()
        {
            var user = new IdentityUser("foo");
            var confirmed = true;

            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(false);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.KeyNotFound);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<IdentityUser>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.SetEmailConfirmedAsync(user, confirmed));
        }

        [Test]
        public async void When_User_Has_PasswordHash_HasPasswordHashAsync_Returns_True()
        {
            var user = new IdentityUser("foo")
            {
                PasswordHash = "somehashedpassword"
            };

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            var store = new UserStore<IdentityUser>(mockBucket.Object);

            Assert.IsTrue(await store.HasPasswordAsync(user));
        }

        [Test]
        public async void When_User_Does_Not_Have_PasswordHash_HasPasswordHashAsync_Returns_False()
        {
            var user = new IdentityUser("foo");

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            var store = new UserStore<IdentityUser>(mockBucket.Object);

            Assert.IsFalse(await store.HasPasswordAsync(user));
        }

        [Test]
        public async void When_User_Has_Password_Hash__GetPasswordHashAsync_Returns_It()
        {
            var user = new IdentityUser("foo")
            {
                PasswordHash = "somepasswordhash"
            };

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            var store = new UserStore<IdentityUser>(mockBucket.Object);

            Assert.IsNotNullOrEmpty(await store.GetPasswordHashAsync(user));
        }

        [Test]
        public void When_User_Exists_SetPasswordHashAsync_Succeeds()
        {
            var user = new IdentityUser("foo");
            const string passwordHash = "somepasswordhash";

            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(true);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.Success);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<IdentityUser>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.SetPasswordHashAsync(user, passwordHash));
            Assert.AreEqual(passwordHash, user.PasswordHash);
        }

        [Test]
        public void When_User_Does_Not_Exist_SetPasswordHashAsync_Fails()
        {
            var user = new IdentityUser("foo");
            var passwordHash = "somepasswordhash";

            var mockResult = new Mock<IOperationResult<IdentityUser>>();
            mockResult.SetupGet(x => x.Success).Returns(false);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.KeyNotFound);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<IdentityUser>()))
                .ReturnsAsync(mockResult.Object);

            var store = new UserStore<IdentityUser>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.SetPasswordHashAsync(user, passwordHash));
        }
    }
}

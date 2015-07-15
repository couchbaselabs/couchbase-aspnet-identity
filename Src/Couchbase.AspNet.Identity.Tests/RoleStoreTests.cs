
using System;
using Couchbase.Core;
using Couchbase.IO;
using Moq;
using NUnit.Framework;

namespace Couchbase.AspNet.Identity.Tests
{
    [TestFixture]
    public class RoleStoreTests
    {
        [Test]
        public void When_Success_CreateAsync_Does_Not_Throw_Exception()
        {
            var mockResult = new Mock<IOperationResult<IdentityRole>>();
            mockResult.SetupGet(x => x.Success).Returns(true);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.InsertAsync(It.IsAny<string>(), It.IsAny<IdentityRole>()))
                .ReturnsAsync(mockResult.Object);

            var store = new RoleStore<IdentityRole>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.CreateAsync(new IdentityRole("user", Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Not_Success_CreateAsync_Throws_CouchbaseException()
        {
            var mockResult = new Mock<IOperationResult<IdentityRole>>();
            mockResult.SetupGet(x => x.Success).Returns(false);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.KeyExists);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.InsertAsync(It.IsAny<string>(), It.IsAny<IdentityRole>()))
                .ReturnsAsync(mockResult.Object);

            var store = new RoleStore<IdentityRole>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.CreateAsync(new IdentityRole("user", Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Success_DeleteAsync_Does_Not_Throw_Exception()
        {
            var mockResult = new Mock<IOperationResult<IdentityRole>>();
            mockResult.SetupGet(x => x.Success).Returns(true);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.RemoveAsync(It.IsAny<string>()))
                .ReturnsAsync(mockResult.Object);

            var store = new RoleStore<IdentityRole>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.DeleteAsync(new IdentityRole("user", Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Not_Success_DeleteAsync_Throws_CouchbaseException()
        {
            var mockResult = new Mock<IOperationResult<IdentityRole>>();
            mockResult.SetupGet(x => x.Success).Returns(false);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.RemoveAsync(It.IsAny<string>()))
                .ReturnsAsync(mockResult.Object);

            var store = new RoleStore<IdentityRole>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.DeleteAsync(new IdentityRole("user", Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Success_UpdateAsync_Does_Not_Throw_Exception()
        {
            var mockResult = new Mock<IOperationResult<IdentityRole>>();
            mockResult.SetupGet(x => x.Success).Returns(true);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<IdentityRole>()))
                .ReturnsAsync(mockResult.Object);

            var store = new RoleStore<IdentityRole>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.UpdateAsync(new IdentityRole("user", Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Not_Success_UpdateAsync_Throws_CouchbaseException()
        {
            var mockResult = new Mock<IOperationResult<IdentityRole>>();
            mockResult.SetupGet(x => x.Success).Returns(false);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.KeyNotFound);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<IdentityRole>()))
                .ReturnsAsync(mockResult.Object);

            var store = new RoleStore<IdentityRole>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.UpdateAsync(new IdentityRole("user", Guid.NewGuid().ToString())));
        }

        [Test]
        public void When_Role_Does_Not_Exist_FindByIdAsync_Fails()
        {
            var mockResult = new Mock<IOperationResult<IdentityRole>>();
            mockResult.SetupGet(x => x.Success).Returns(false);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.KeyNotFound);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.GetAsync<IdentityRole>(It.IsAny<string>()))
                .ReturnsAsync(mockResult.Object);

            var store = new RoleStore<IdentityRole>(mockBucket.Object);
            Assert.Throws<CouchbaseException>(async () => await store.FindByIdAsync(Guid.NewGuid().ToString()));
        }

        [Test]
        public void When_Role_Exists_FindByIdAsync_Succeeds()
        {
            var mockResult = new Mock<IOperationResult<IdentityRole>>();
            mockResult.SetupGet(x => x.Success).Returns(true);
            mockResult.SetupGet(x => x.Status).Returns(ResponseStatus.Success);

            var mockBucket = new Mock<IBucket>();
            mockBucket.SetupGet(e => e.Name).Returns("default");
            mockBucket.Setup(x => x.GetAsync<IdentityRole>(It.IsAny<string>()))
                .ReturnsAsync(mockResult.Object);

            var store = new RoleStore<IdentityRole>(mockBucket.Object);
            Assert.DoesNotThrow(async () => await store.FindByIdAsync(Guid.NewGuid().ToString()));
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

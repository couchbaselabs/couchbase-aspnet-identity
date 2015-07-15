using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Couchbase.IO;

namespace Couchbase.AspNet.Identity
{
    public class CouchbaseException : Exception
    {
        public CouchbaseException()
        {
        }

        public CouchbaseException(IOperationResult result)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
        }

        public CouchbaseException(IDocumentResult result)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
        }

        public CouchbaseException(IOperationResult result, string key)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
            Key = key;
        }

        public CouchbaseException(IDocumentResult result, string key)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
            Key = key;
        }

        public CouchbaseException(string message)
            : base(message)
        {
        }

        public CouchbaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CouchbaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ResponseStatus Status { get; set; }

        public string Key { get; set; }
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

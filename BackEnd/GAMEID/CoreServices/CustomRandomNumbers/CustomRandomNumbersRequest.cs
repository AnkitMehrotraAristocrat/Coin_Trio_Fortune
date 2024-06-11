using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GameBackend
{
    /// <summary>
    /// Defines the structure of a request to the CustomRandomNumber service
    /// </summary>
    public struct CustomRandomNumbersRequest
    {
        // A queue of random numbers to pull from before generating random values with the RngHelper
        public List<JToken> RandomNumberQueue { get; set; }
    }
}

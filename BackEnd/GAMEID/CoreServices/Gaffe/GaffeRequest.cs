
namespace GameBackend
{
    /// <summary>
    /// Defines the structure of a request to the Gaffe service
    /// </summary>
    public struct GaffeRequest
    {
        /// <summary>
        /// The index of the gaffe to stage for the next service request that requests random values from the RngHelper.
        /// This property is required if the request's Name property is not specified.
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// The name of the gaffe to stage for the next service request that requests random values from the RngHelper.
        /// This property is required if the request's Index property is not specified.
        /// </summary>
        public string? Name { get; set; }
    }
}

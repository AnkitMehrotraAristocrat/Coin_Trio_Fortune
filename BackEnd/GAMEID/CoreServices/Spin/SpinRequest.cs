using System.Collections.Generic;

namespace GameBackend
{
    /// <summary>
    /// Defines the structure of a request to the Spin service.
    /// </summary>
    public struct SpinRequest
    {
        /// <summary>
        /// The index of the selected bet placed from the game's frontend.
        /// </summary>
        public int BetIndex { get; set; }

        /// <summary>
        /// The index of the selected lines from the game's frontend.
        /// </summary>
        public int LineIndex { get; set; }

        /// <summary>
        /// An optional queue of random numbers that will be used for random numbers until the queue is empty.
        /// </summary>
        public List<ulong?> RandomNumberQueue { get; set; }
    }
}

using System.Collections.Generic;

namespace Nightingale.Core.Mock.Models
{
    /// <summary>
    /// Represents a path segment in the mock server tree.
    /// </summary>
    public class MockNode
    {
        /// <summary>
        /// Method of the path if this node is a leaf.
        /// Null if it's an internal node.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The path segmented represented by this node.
        /// </summary>
        public string PathSegment { get; set; }

        /// <summary>
        /// This node's children.
        /// </summary>
        public List<MockNode> Children { get; set; } = new List<MockNode>();

        /// <summary>
        /// Return value of the path if this node is a leaf.
        /// Null if it's an internal node.
        /// </summary>
        public MockData ReturnValue { get; set; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    #region Enums
    /// <summary>
    /// Tree pass type
    /// </summary>
    public enum PassType
    {
        /// <summary>
        /// Root - left leaf - right leaf
        /// </summary>
        PreOrder,
        /// <summary>
        /// Left leaf - right leaf - root
        /// </summary>
        PostOrder,
        /// <summary>
        /// Left leaf - root - right leaf
        /// </summary>
        HybridOrder,
        /// <summary>
        /// Order by tree floors (root is zero floor, his left and right leafs are first etc)
        /// </summary>
        FloorsOrder
    }
    /// <summary>
    /// Output string format
    /// </summary>
    public enum StringFormat
    {
        /// <summary>
        /// Line without any indents
        /// </summary>
        SingleLine,
        /// <summary>
        /// Line with indents
        /// </summary>
        Indented
    }
    public enum RotationType
    {
        Right,
        Left
    }
    #endregion
    #region Exceptions
    /// <summary>
    /// <para>Object duplication exception</para>
    /// <para>Appears while adding object which already exist in a tree</para>
    /// </summary>
    /// <typeparam name="T">As tree can contain a different types of objects</typeparam>
    [Serializable]
    public class ObjectDuplicationException<T> : Exception
    {
        public ObjectDuplicationException(T Object) : base($"Object '{Object}' is already exist in current tree!") { }
        public ObjectDuplicationException(string message) : base(message) { }
        public ObjectDuplicationException(string message, Exception innerException) : base(message, innerException) { }
        public ObjectDuplicationException() { }
        protected ObjectDuplicationException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    /// <summary>
    /// Binary tree leaf class
    /// </summary>
    /// <typeparam name="T">Type of leaf value</typeparam>
    public class BinaryHeapNode<T> : IComparable<BinaryHeapNode<T>>, IEquatable<BinaryHeapNode<T>> where T : IComparable<T>
    {
        #region Properties
        /// <summary>
        /// Link to a parent leaf node
        /// </summary>
        public BinaryHeapNode<T> Parent { get; set; }
        /// <summary>
        /// Link to a left leaf node
        /// </summary>
        public BinaryHeapNode<T> Left { get; set; }
        /// <summary>
        /// Link to a right leaf node
        /// </summary>
        public BinaryHeapNode<T> Right { get; set; }
        /// <summary>
        /// Leaf value
        /// </summary>
        public T Data { get; set; }
        #endregion
        #region Constructors
        /// <summary>
        /// Tree node constructor
        /// </summary>
        /// <param name="Object">Leaf value to be set</param>
        public BinaryHeapNode(T Object)
        {
            if (Object == null) throw new NullReferenceException();
            Data = Object;
        }
        #endregion
        #region Service
        /// <summary>
        /// Leafs comparator
        /// </summary>
        /// <param name="obj">Another leaf</param>
        /// <returns>Difference between two leafs</returns>
        public int CompareTo(BinaryHeapNode<T> obj)
        {
            if (obj == null) throw new NullReferenceException();
            return Data.CompareTo(obj.Data);
        }
        /// <summary>
        /// Leafs equation
        /// </summary>
        /// <param name="OtherNode">Another leaf</param>
        /// <returns><c>true</c> if leafs values are the same and <c>false</c> otherwise</returns>
        public bool Equals(BinaryHeapNode<T> OtherNode)
        {
            if (OtherNode == null) throw new NullReferenceException();
            return Data.CompareTo(OtherNode.Data) == 0;
        }
        /// <summary>
        /// Swap nodes data
        /// </summary>
        /// <param name="OtherNode">Second node to swap data with</param>
        public void SwapData(BinaryHeapNode<T> OtherNode)
        {
            if (OtherNode == null) throw new NullReferenceException();
            (Data, OtherNode.Data) = (OtherNode.Data, Data);
        }
        /// <summary>
        /// Leaf to string converter
        /// </summary>
        /// <returns>Leaf value as string value</returns>
        public override string ToString() => Data.ToString();
        /// <summary>
        /// Leaf hash converter
        /// </summary>
        /// <returns>Leaf value hash</returns>
        public override int GetHashCode() => Data.GetHashCode();
        #endregion
    }
    /// <summary>
    /// Binary tree class
    /// </summary>
    /// <typeparam name="T">Comparable type of tree leafs values</typeparam>
    public class BinaryHeap<T> : IEnumerable<T> where T : IComparable<T>
    {
        #region Properties
        /// <summary>
        /// Link to tree root
        /// </summary>
        public BinaryHeapNode<T> Root { get; set; }
        /// <summary>
        /// Sorted list of tree leafs values
        /// </summary>
        public List<T> SortedPassList => Pass(PassType.HybridOrder, default);
        /// <summary>
        /// List of tree leafs value by tree floors
        /// </summary>
        public List<List<T>> FloorList => FloorPass(Root).Select(list => list.Select(node => node.Data).ToList()).ToList();
        /// <summary>
        /// Total tree leafs count
        /// </summary>
        public int Count => SortedPassList.Count;
        /// <summary>
        /// Total tree height
        /// </summary>
        public int Height => FloorPass(Root).Count;
        /// <summary>
        /// Check if tree is balanced or not
        /// </summary>
        public bool IsBalanced => Math.Abs(GetBalanceFactor(Root)) <= 1;
        /// <summary>
        /// Maximal value of tree leafs values
        /// </summary>
        public T MaxValue => SortedPassList.Aggregate((a, b) => a.CompareTo(b) > 0 ? a : b);
        /// <summary>
        /// Minimal value of tree leafs values
        /// </summary>
        public T MinValue => SortedPassList.Aggregate((a, b) => b.CompareTo(a) > 0 ? a : b);
        /// <summary>
        /// Tree indexer
        /// </summary>
        /// <param name="index">Leaf index</param>
        /// <returns>Leaf value</returns>
        public T this[int index]
        {
            get { return Pass(PassType.FloorsOrder)[index].Data; }
            set { Pass(PassType.FloorsOrder)[index].Data = value; }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Binary tree constructor
        /// </summary>
        /// <param name="ObjectSequence">Sequence of values</param>
        public BinaryHeap(params T[] ObjectSequence) => Add(ObjectSequence);
        /// <summary>
        /// Binary tree constructor
        /// </summary>
        /// <param name="Collection">Any enumerable collection</param>
        public BinaryHeap(IEnumerable<T> Collection) => Add(Collection);
        /// <summary>
        /// Binary tree static cunstructor
        /// </summary>
        /// <param name="ObjectSequence">Sequence of values</param>
        /// <returns>Link to created binary tree object</returns>
        public static BinaryHeap<T> Create(params T[] ObjectSequence) => new BinaryHeap<T>(ObjectSequence);
        /// <summary>
        /// Binary tree static cunstructor
        /// </summary>
        /// <param name="Collection">Any enumerable collection</param>
        /// <returns>Link to created binary tree object</returns>
        public static BinaryHeap<T> Create(IEnumerable<T> Collection) => new BinaryHeap<T>(Collection);
        #endregion
        #region Editors
        /// <summary>
        /// Allows to add new leafs by sequence of values
        /// </summary>
        /// <param name="ObjectSequence">Sequence of values</param>
        public void Add(params T[] ObjectSequence)
        {
            foreach (var obj in ObjectSequence) Add(obj);
        }
        /// <summary>
        /// Allows to add new leafs by any enumerable collection
        /// </summary>
        /// <param name="Collection">Any enumerable collection</param>
        public void Add(IEnumerable<T> Collection)
        {
            if (Collection == null) throw new NullReferenceException();
            foreach (var obj in Collection) Add(obj);
        }
        /// <summary>
        /// Allows to add new leaf
        /// </summary>
        /// <param name="Object">Leaf value</param>
        private void Add(T Object)
        {
            if (Root == null)
            {
                Root = new BinaryHeapNode<T>(Object);
            }
            else
            {
                if (IsItemExist(Object)) throw new ObjectDuplicationException<T>(Object);
                AddNode(new BinaryHeapNode<T>(Object));
                Heapify();
            }
        }
        /// <summary>
        /// Private tree node add method
        /// </summary>
        /// <param name="TreeNode">New leaf to add</param>
        /// <param name="AddLink">
        /// <para>Link of basic leaf to add new one</para>
        /// <para>Adding algorithm should always start from Root</para>
        /// </param>
        private void Add(BinaryHeapNode<T> TreeNode, BinaryHeapNode<T> AddLink)
        {
            if (TreeNode == null) throw new NullReferenceException();
            if (TreeNode.CompareTo(AddLink) > 0)
            {
                if (AddLink.Right == null)
                {
                    AddLink.Right = TreeNode;
                    TreeNode.Parent = AddLink;
                }
                else
                {
                    Add(TreeNode, AddLink.Right);
                }
            }
            else if (TreeNode.CompareTo(AddLink) <= 0)
            {
                if (AddLink.Left == null)
                {
                    AddLink.Left = TreeNode;
                    TreeNode.Parent = AddLink;
                }
                else
                {
                    Add(TreeNode, AddLink.Left);
                }
            }
        }
        /// <summary>
        /// Private tree node add method
        /// </summary>
        /// <param name="TreeNode">New leaf to add</param>
        private void AddNode(BinaryHeapNode<T> TreeNode)
        {
            if (TreeNode == null) throw new NullReferenceException();

            var floorList = FloorPass(Root);

            if (floorList.Count == 1)
            {
                Root.Left = TreeNode;
                TreeNode.Parent = Root;
                return;
            }

            for (var i = 0; i < floorList.Count; i++)
            {
                if (i == floorList.Count - 1)
                {
                    var Node = floorList[i][0];

                    if (Node.Left == null)
                    {
                        Node.Left = TreeNode;
                        TreeNode.Parent = Node;
                    }
                    else
                    {
                        Node.Right = TreeNode;
                        TreeNode.Parent = Node;
                    }

                    return;
                }
                if (floorList[i + 1].Count != Math.Pow(2, i + 1))
                {
                    var index =
                    (
                        floorList[i + 1].IndexesWhere(item => item != null).Last() + 1
                    );

                    var Node = floorList[i][index / 2];

                    if (Node.Left == null)
                    {
                        Node.Left = TreeNode;
                        TreeNode.Parent = Node;
                    }
                    else
                    {
                        Node.Right = TreeNode;
                        TreeNode.Parent = Node;
                    }

                    return;
                }
            }
        }
        /// <summary>
        /// Heapify tree
        /// </summary>
        public void Heapify()
        {
            var node = Pass(PassType.FloorsOrder).Last();
            while (node.Parent != null && node.Data.CompareTo(node.Parent.Data) > 0)
            {
                node.SwapData(node.Parent);
                node = node.Parent;
            }
        }
        /// <summary>
        /// Removes first found leaf
        /// </summary>
        /// <param name="Value">Value to search leafs</param>
        /// <param name="passOrder">Order of tree pass</param>
        /// <returns>Total removes leafs count</returns>
        public int Remove(T Value, PassType passOrder = PassType.FloorsOrder)
        {
            var RemoveItem = FindNodeByValue(Value, passOrder);
            if (RemoveItem != null) { RemoveNode(RemoveItem, passOrder); return 1; }
            return 0;
        }
        /// <summary>
        /// Removes all found leafs
        /// </summary>
        /// <param name="Value">Value to search leafs</param>
        /// <param name="passOrder">Order of tree pass</param>
        /// <returns>Total removes leafs count</returns>
        public int RemoveAll(T Value, PassType passOrder = PassType.FloorsOrder)
        {
            BinaryHeapNode<T> RemoveItem;
            int RemoveCount = 0;
            while ((RemoveItem = FindNodeByValue(Value, passOrder)) != null)
            {
                RemoveCount++;
                RemoveNode(RemoveItem, passOrder);
            }
            return RemoveCount;
        }
        /// <summary>
        /// Removes node from tree
        /// </summary>
        /// <param name="TreeNode">Tree node</param>
        /// <param name="passType">Order of tree pass</param>
        private void RemoveNode(BinaryHeapNode<T> TreeNode, PassType passType = PassType.FloorsOrder)
        {
            if ((TreeNode.Left ?? TreeNode.Right) == null && TreeNode != Root)
            {
                if (TreeNode == TreeNode.Parent.Left) TreeNode.Parent.Left = null;
                if (TreeNode == TreeNode.Parent.Right) TreeNode.Parent.Right = null;
            }
            else
            {
                var RemovedItemsList = Pass(TreeNode, passType).Where(x => x != TreeNode).Select(x => x.Data).ToList();
                if (TreeNode == Root)
                {
                    Root = null;
                }
                else
                {
                    if (TreeNode == TreeNode.Parent.Left) TreeNode.Parent.Left = null;
                    if (TreeNode == TreeNode.Parent.Right) TreeNode.Parent.Right = null;
                }
                foreach (var item in RemovedItemsList) Add(item);
            }
        }
        /// <summary>
        /// Clear tree
        /// </summary>
        public void Clear() => Root = null;
        #endregion
        #region Search
        /// <summary>
        /// Find all nodes by their value
        /// </summary>
        /// <param name="Value">Value to search</param>
        /// <param name="passOrder">Order of tree pass</param>
        /// <returns>List of tree nodes</returns>
        private List<BinaryHeapNode<T>> FindNodeListByValue(T Value, PassType passOrder = PassType.HybridOrder)
        {
            return Pass(passOrder).Where(x => x.Data.CompareTo(Value) == 0).ToList();
        }
        /// <summary>
        /// Find first node by value
        /// </summary>
        /// <param name="Value">Value to search</param>
        /// <param name="passOrder">Order of tree pass</param>
        /// <returns>Node link</returns>
        public BinaryHeapNode<T> FindNodeByValue(T Value, PassType passOrder = PassType.HybridOrder)
        {
            var PassList = FindNodeListByValue(Value, passOrder);
            if (PassList.Count != 0) return PassList[0];
            return null;
        }
        /// <summary>
        /// Find node by its index in tree
        /// </summary>
        /// <param name="NodeIndex">Index</param>
        /// <returns>Node link</returns>
        private BinaryHeapNode<T> FindNodeByIndex(int NodeIndex)
        {
            var pass = Pass(Root, PassType.FloorsOrder);
            if (NodeIndex < pass.Count) return pass[NodeIndex];
            return null;
        }
        #endregion
        #region Pass
        /// <summary>
        /// Get a list of tree leaf values with a specified pass order
        /// </summary>
        /// <param name="passOrder">Pass order type</param>
        /// <param name="NodeIndex">Start leaf index</param>
        /// <returns>List of tree leaf values</returns>
        public List<T> Pass(PassType passOrder = PassType.PreOrder, int NodeIndex = 0)
        {
            if (Root == null) return new List<T>();
            var Node = FindNodeByIndex(NodeIndex);
            switch (passOrder)
            {
                case PassType.PreOrder: { return PreOrderPass(Node).Select(x => x.Data).ToList(); }
                case PassType.PostOrder: { return PostOrderPass(Node).Select(x => x.Data).ToList(); }
                case PassType.HybridOrder: { return HybridOrderPass(Node).Select(x => x.Data).ToList(); }
                case PassType.FloorsOrder:
                    {
                        var floorList = FloorPass(Node);
                        if (floorList.Count != 0) return FloorPass(Node).Aggregate((x, y) => x.Concat(y).ToList()).Select(x => x.Data).ToList();
                        else return new List<T>();
                    }
                default: throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Privare pass method
        /// </summary>
        /// <param name="passOrder">Pass order type</param>
        /// <returns>List of tree leafs</returns>
        private List<BinaryHeapNode<T>> Pass(PassType passOrder = PassType.PreOrder)
        {
            switch (passOrder)
            {
                case PassType.PreOrder: { return PreOrderPass(Root); }
                case PassType.PostOrder: { return PostOrderPass(Root); }
                case PassType.HybridOrder: { return HybridOrderPass(Root); }
                case PassType.FloorsOrder:
                    {
                        var floorList = FloorPass(Root);
                        if (floorList.Count != 0) return FloorPass(Root).Aggregate((x, y) => x.Concat(y).ToList());
                        else return new List<BinaryHeapNode<T>>();
                    }
                default: throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Private pass method
        /// </summary>
        /// <param name="Node">Leaf to start pass from</param>
        /// <param name="passOrder">Pass order type</param>
        /// <returns>List of tree leafs</returns>
        private List<BinaryHeapNode<T>> Pass(BinaryHeapNode<T> Node, PassType passOrder = PassType.PreOrder)
        {
            switch (passOrder)
            {
                case PassType.PreOrder: { return PreOrderPass(Node); }
                case PassType.PostOrder: { return PostOrderPass(Node); }
                case PassType.HybridOrder: { return HybridOrderPass(Node); }
                case PassType.FloorsOrder:
                    {
                        var floorList = FloorPass(Node);
                        if (floorList.Count != 0) return FloorPass(Node).Aggregate((x, y) => x.Concat(y).ToList());
                        else return new List<BinaryHeapNode<T>>();
                    }
                default: throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Pass tree by its floors
        /// </summary>
        /// <param name="Node">Leaf to start pass from</param>
        /// <param name="FloorList">
        /// <para>Final list of tree floors</para>
        /// <para>Should be null on method call!</para>
        /// </param>
        /// <returns>List of tree leafs ordered by floors</returns>
        private List<List<BinaryHeapNode<T>>> FloorPass(BinaryHeapNode<T> Node, List<List<BinaryHeapNode<T>>> FloorList = null)
        {
            if (FloorList == null)
            {
                FloorList = new List<List<BinaryHeapNode<T>>>();
                if (Root == null) return FloorList;
                FloorList.Add(new List<BinaryHeapNode<T>>() { Node });
            }
            var CurrentFloor = new List<BinaryHeapNode<T>>();
            foreach (var item in FloorList.Last())
            {
                if (item.Left != null) CurrentFloor.Add(item.Left);
                if (item.Right != null) CurrentFloor.Add(item.Right);
            }
            FloorList.Add(CurrentFloor);
            if (FloorList.Last().Any(x => x != null)) FloorPass(null, FloorList);
            else FloorList.Remove(FloorList.Last());

            return FloorList;
        }
        /// <summary>
        /// Post order type pass method
        /// </summary>
        /// <param name="Node">Leaf to start pass from</param>
        /// <param name="PassList">
        /// <para>Final list of tree leafs</para>
        /// <para>Should be null on method call</para>
        /// </param>
        /// <returns>List of tree leafs</returns>
        private List<BinaryHeapNode<T>> PostOrderPass(BinaryHeapNode<T> Node, List<BinaryHeapNode<T>> PassList = null)
        {
            if (PassList == null) PassList = new List<BinaryHeapNode<T>>();
            if (Node != null)
            {
                PostOrderPass(Node.Left, PassList);
                PostOrderPass(Node.Right, PassList);
                PassList.Add(Node);
            }
            return PassList;
        }
        /// <summary>
        /// Pre order type pass method
        /// </summary>
        /// <param name="Node">Leaf to start pass from</param>
        /// <param name="PassList">
        /// <para>Final list of tree leafs</para>
        /// <para>Should be null on method call</para>
        /// </param>
        /// <returns>List of tree leafs</returns>
        private List<BinaryHeapNode<T>> PreOrderPass(BinaryHeapNode<T> Node, List<BinaryHeapNode<T>> PassList = null)
        {
            if (PassList == null) PassList = new List<BinaryHeapNode<T>>();
            if (Node != null)
            {
                PassList.Add(Node);
                PreOrderPass(Node.Left, PassList);
                PreOrderPass(Node.Right, PassList);
            }
            return PassList;
        }
        /// <summary>
        /// Hybrid order type pass method
        /// </summary>
        /// <param name="Node">Leaf to start pass from</param>
        /// <param name="PassList">
        /// <para>Final list of tree leafs</para>
        /// <para>Should be null on method call</para>
        /// </param>
        /// <returns>List of tree leafs</returns>
        private List<BinaryHeapNode<T>> HybridOrderPass(BinaryHeapNode<T> Node, List<BinaryHeapNode<T>> PassList = null)
        {
            if (PassList == null) PassList = new List<BinaryHeapNode<T>>();
            if (Node != null)
            {
                HybridOrderPass(Node.Left, PassList);
                PassList.Add(Node);
                HybridOrderPass(Node.Right, PassList);
            }
            return PassList;
        }
        #endregion
        #region Service
        /// <summary>
        /// Tree to string converter
        /// </summary>
        /// <param name="stringFormat">Format of output string</param>
        /// <returns>String representation of tree</returns>
        public string ToString(StringFormat stringFormat = StringFormat.SingleLine)
        {
            var tempStr = String.Empty;
            var tempList = FloorPass(Root);
            for (var i = tempList.Count - 1; i >= 0; i--)
            {
                for (var j = tempList[i].Count - 1; j >= 0; j--)
                {
                    if (i == 0) tempStr = tempStr.Insert(0, $"{tempList[i][j].Data}:Root ");
                    else
                    {
                        if (tempList[i][j].Parent.Left == tempList[i][j]) tempStr = tempStr.Insert(0, $"{tempList[i][j].Data}:{tempList[i][j].Parent.Data}(L) ");
                        if (tempList[i][j].Parent.Right == tempList[i][j]) tempStr = tempStr.Insert(0, $"{tempList[i][j].Data}:{tempList[i][j].Parent.Data}(R) ");
                    }
                }
                if (stringFormat == StringFormat.Indented && i != 0) tempStr = tempStr.Insert(0, Environment.NewLine);
            }
            return tempStr.Trim(' ');
        }
        /// <summary>
        /// Object existing checker
        /// </summary>
        /// <param name="Object">Object of tree</param>
        /// <returns><c>true</c> if object exists in tree and <c>false</c> otherwise</returns>
        public bool IsItemExist(T Object) => SortedPassList.Contains(Object);
        /// <summary>
        /// Allows to get height of any subtree
        /// </summary>
        /// <param name="Node">Tree leaf</param>
        /// <returns>Height of leafs subtree</returns>
        private int GetHeight(BinaryHeapNode<T> Node) => Node == null ? default : FloorPass(Node).Count;
        /// <summary>
        /// Allows to get height of any subtree
        /// </summary>
        /// <param name="Object">Tree leaf value</param>
        /// <returns>Height of leafs subtree</returns>
        public int GetHeight(T Object)
        {
            var Node = FindNodeByValue(Object);
            if (Node == null)
                return default;
            else
                return FloorPass(Node).Count;
        }
        /// <summary>
        /// Get balance factor of the leaf
        /// </summary>
        /// <param name="Node">Tree leaf</param>
        /// <returns>Difference between left and right subtrees heights</returns>
        private int GetBalanceFactor(BinaryHeapNode<T> Node)
        {
            if (Node != null)
                return GetHeight(Node.Left) - GetHeight(Node.Right);
            return 0;
        }
        /// <summary>
        /// Get balance factor of the leaf by its value
        /// </summary>
        /// <param name="Object">Tree leaf value</param>
        /// <returns>Difference between left and right subtrees heights</returns>
        public int GetBalanceFactor(T Object) => GetBalanceFactor(FindNodeByValue(Object));
        /// <summary>
        /// Check if stated subtree is full or not
        /// </summary>
        /// <param name="Node">Subtree root</param>
        /// <returns><c>true</c> if subtree is full and <c>false</c> otherwise</returns>
        private bool IsSubtreeFull(BinaryHeapNode<T> Node)
        {
            if (Node == null) throw new NullReferenceException();

            var floorList = FloorPass(Node);
            var count = floorList.Count == 0 ? 0 : floorList.Aggregate((x, y) => x.Concat(y).ToList()).Count;
            return count == 1 ? true : count == Math.Pow(2, floorList.Count) - 1;
        }
        /// <summary>
        /// Check if stated subtree is full or not
        /// </summary>
        /// <param name="Object">Tree leafs type object to find subtree root</param>
        /// <returns><c>true</c> if subtree is full and <c>false</c> otherwise</returns>
        public bool IsSubtreeFull(T Object) => IsSubtreeFull(FindNodeByValue(Object));
        /// <summary>
        /// Basic tree to string converter
        /// </summary>
        /// <returns>String representation of tree</returns>
        public override string ToString() => ToString(StringFormat.Indented);
        #endregion
        #region Interfaces
        public IEnumerator<T> GetEnumerator() => Pass(PassType.FloorsOrder, 0).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Pass(PassType.FloorsOrder, 0).GetEnumerator();
        #endregion
    }
    /// <summary>
    /// Binary tree creator class
    /// </summary>
    public static class BinaryHeap
    {
        /// <summary>
        /// Binary tree static cunstructor
        /// </summary>
        /// <typeparam name="T">Comparable type of tree leafs values</typeparam>
        /// <param name="ObjectSequence">Sequence of values</param>
        /// <returns>Link to created binary tree object</returns>
        public static BinaryHeap<T> Create<T>(params T[] ObjectSequence) where T : IComparable<T> => new BinaryHeap<T>(ObjectSequence);
        /// <summary>
        /// Binary tree static cunstructor
        /// </summary>
        /// <typeparam name="T">Comparable type of tree leafs values</typeparam>
        /// <param name="Collection">Any enumerable collection</param>
        /// <returns>Link to created binary tree object</returns>
        public static BinaryHeap<T> Create<T>(IEnumerable<T> Collection) where T : IComparable<T> => new BinaryHeap<T>(Collection);
    }
}

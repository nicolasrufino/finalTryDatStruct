using System;
using System.Collections.Generic;

namespace TreasureHuntGame
{
    public class Tree
    {
        public class RoomNode
        {
            public int RoomNumber { get; set; }
            public string Description { get; set; }
            public bool IsCompleted { get; set; } // Indicates if the room is completed
            public RoomNode Left { get; set; }
            public RoomNode Right { get; set; }

            public RoomNode(int roomNumber, string description)
            {
                RoomNumber = roomNumber;
                Description = description;
                IsCompleted = false;
                Left = null;
                Right = null;
            }
        }

        public RoomNode Root { get; private set; }

        public Tree()
        {
            Root = null;
            InitializeTree();
        }

        // Initialize the binary tree to represent the levels
        private void InitializeTree()
        {
            Root = new RoomNode(4, "Central Room");
            Root.Left = new RoomNode(2, "Left Branch Room");
            Root.Right = new RoomNode(6, "Right Branch Room");
            Root.Left.Left = new RoomNode(1, "Far Left Room");
            Root.Left.Right = new RoomNode(3, "Mid Left Room");
            Root.Right.Left = new RoomNode(5, "Mid Right Room");
            Root.Right.Right = new RoomNode(7, "Far Right Room");
        }

        public void AddRoom(int roomNumber, string description)
        {
            Root = AddRoomRecursive(Root, roomNumber, description);
        }

        private RoomNode AddRoomRecursive(RoomNode current, int roomNumber, string description)
        {
            if (current == null)
            {
                return new RoomNode(roomNumber, description);
            }

            if (roomNumber < current.RoomNumber)
            {
                current.Left = AddRoomRecursive(current.Left, roomNumber, description);
            }
            else if (roomNumber > current.RoomNumber)
            {
                current.Right = AddRoomRecursive(current.Right, roomNumber, description);
            }

            return current;
        }

        public RoomNode FindRoom(int roomNumber)
        {
            return FindRoomRecursive(Root, roomNumber);
        }

        private RoomNode FindRoomRecursive(RoomNode current, int roomNumber)
        {
            if (current == null || current.RoomNumber == roomNumber)
            {
                return current;
            }

            return roomNumber < current.RoomNumber
                ? FindRoomRecursive(current.Left, roomNumber)
                : FindRoomRecursive(current.Right, roomNumber);
        }

        public void MarkRoomAsCompleted(int roomNumber)
        {
            RoomNode room = FindRoom(roomNumber);
            if (room != null)
            {
                room.IsCompleted = true;
                Console.WriteLine($"Room {roomNumber} marked as completed.");
            }
        }

        public void RemoveRoomIfPossible(int roomNumber)
        {
            RoomNode room = FindRoom(roomNumber);
            if (room != null && room.IsCompleted && CanRemoveRoom(room))
            {
                Root = RemoveRoomRecursive(Root, roomNumber);
                Console.WriteLine($"Room {roomNumber} has been removed from the tree.");
            }
        }

        private bool CanRemoveRoom(RoomNode room)
        {
            // Room can be removed if it does not serve as a bridge to other rooms
            return (room.Left == null || room.Left.IsCompleted) && (room.Right == null || room.Right.IsCompleted);
        }

        private RoomNode RemoveRoomRecursive(RoomNode current, int roomNumber)
        {
            if (current == null)
            {
                return null;
            }

            if (roomNumber < current.RoomNumber)
            {
                current.Left = RemoveRoomRecursive(current.Left, roomNumber);
            }
            else if (roomNumber > current.RoomNumber)
            {
                current.Right = RemoveRoomRecursive(current.Right, roomNumber);
            }
            else
            {
                // Node with only one child or no child
                if (current.Left == null)
                {
                    return current.Right;
                }
                else if (current.Right == null)
                {
                    return current.Left;
                }

                // Node with two children: Get the inorder successor (smallest in the right subtree)
                RoomNode minNode = GetMinNode(current.Right);
                current.RoomNumber = minNode.RoomNumber;
                current.Description = minNode.Description;
                current.IsCompleted = minNode.IsCompleted;
                current.Right = RemoveRoomRecursive(current.Right, minNode.RoomNumber);
            }

            return current;
        }

        private RoomNode GetMinNode(RoomNode node)
        {
            while (node.Left != null)
            {
                node = node.Left;
            }

            return node;
        }

        public void DisplayTree()
        {
            Console.WriteLine("Rooms in the Tree (InOrder):");
            DisplayTreeRecursive(Root);
            Console.WriteLine();
        }

        private void DisplayTreeRecursive(RoomNode node)
        {
            if (node == null) return;

            DisplayTreeRecursive(node.Left);
            Console.WriteLine($"Room {node.RoomNumber}: {node.Description}, Completed: {node.IsCompleted}");
            DisplayTreeRecursive(node.Right);
        }
    }
}

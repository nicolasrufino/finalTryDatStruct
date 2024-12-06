using System;
using System.IO;
using System.Collections.Generic;

namespace TreasureHuntGame
{
    public class Map
    {
        public int[,] MapGrid { get; private set; }
        private bool[,] VisitedRooms;
        private Dictionary<string, string> ItemsInRooms; // Stores items like keys, water, etc.
        private Tree roomTree;

        public Map(Tree tree)
        {
            roomTree = tree;
        }

        public void LoadMapFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: Map file not found.");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);
            int rows = lines.Length;
            int cols = lines[0].Length;

            MapGrid = new int[rows, cols];
            VisitedRooms = new bool[rows, cols];
            ItemsInRooms = new Dictionary<string, string>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    char symbol = lines[i][j];
                    MapGrid[i, j] = symbol switch
                    {
                        '0' => 0,   // Empty space
                        'S' => 1,   // Starting point
                        'D' => 2,   // Door
                        'K' => 3,   // Key
                        'W' => 4,   // Water
                        'T' => 5,   // Trap (invisible to player)
                        '#' => -1,  // Wall
                        'N' => 6,  // Node (next path)
                        'F' => 7, // Food
                        _ => -1     // Unknown symbols default to wall
                    };

                    // Track items in specific rooms (keys, food, etc.)
                    if (symbol == 'K')
                    {
                        ItemsInRooms[$"{i},{j}"] = "Key";
                    }
                    else if (symbol == 'F')
                    {
                        ItemsInRooms[$"{i},{j}"] = "Food";
                    }
                }
            }
        }

        public void DisplayMap(int heroX, int heroY)
        {
            Console.Clear();
            for (int i = 0; i < MapGrid.GetLength(0); i++)
            {
                for (int j = 0; j < MapGrid.GetLength(1); j++)
                {
                    if (i == heroX && j == heroY)
                    {
                        Console.Write("👤 "); // Hero represented by a person emoji
                    }
                    else
                    {
                        Console.Write(MapGrid[i, j] switch
                        {
                            0 => "⬛ ", // Empty space
                            -1 => "🟩 ", // Wall
                            1 => "S  ", // Starting point
                            2 => "🚪 ", // Door
                            4 => "💧 ", // Water
                            6 => "➡️  ", // Pathway/Node
                            7 => "🍲 ", // Food
                            _ => "⬜ "  // Unknown
                        });
                    }
                }
                Console.WriteLine();
            }
        }

        public bool IsRoomVisited(int x, int y)
        {
            return VisitedRooms[x, y];
        }

        public void MarkRoomAsVisited(int x, int y)
        {
            VisitedRooms[x, y] = true;
        }

        public void HandleRoomInteraction(int x, int y, Hero hero)
        {
            string roomKey = $"{x},{y}";
            var currentRoom = roomTree.FindRoom(hero.CurrentRoomNumber);

            if (ItemsInRooms.ContainsKey(roomKey))
            {
                string item = ItemsInRooms[roomKey];
                Console.WriteLine($"You found a {item}!");
                hero.AddItem(item);
                ItemsInRooms.Remove(roomKey); // Remove item from room after it's picked up
            }

            if (MapGrid[x, y] == 2) // Door
            {
                if (hero.Inventory.Contains("Key"))
                {
                    Console.WriteLine("You used a key to unlock the door.");
                    hero.RemoveItem("Key");
                }
                else
                {
                    Console.WriteLine("You need a key to unlock this door.");
                    return;
                }
            }

            if (MapGrid[x, y] == 6) // Pathway/Node
            {
                if (currentRoom != null)
                {
                    Console.WriteLine("You have reached a pathway node.");
                    if (currentRoom.Left != null && !currentRoom.Left.IsCompleted)
                    {
                        Console.WriteLine("You proceed to the next room on the left.");
                        hero.CurrentRoomNumber = currentRoom.Left.RoomNumber; // Update hero's current room number
                        LoadMapFromFile($"levels/level{currentRoom.Left.RoomNumber}.txt"); // Load new level map
                    }
                    else if (currentRoom.Right != null && !currentRoom.Right.IsCompleted)
                    {
                        Console.WriteLine("You proceed to the next room on the right.");
                        hero.CurrentRoomNumber = currentRoom.Right.RoomNumber; // Update hero's current room number
                        LoadMapFromFile($"levels/level{currentRoom.Right.RoomNumber}.txt"); // Load new level map
                    }
                    else
                    {
                        Console.WriteLine("No available paths to proceed. Complete other challenges first.");
                    }
                }
            }

            if (MapGrid[x, y] == 5) // Trap (invisible)
            {
                Console.WriteLine("You stepped on a trap! You take 10 damage.");
                hero.TakeDamage(10);
            }
        }
    }
}

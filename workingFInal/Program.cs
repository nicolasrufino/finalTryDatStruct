using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace TreasureHuntGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("=====================================================");
            Console.WriteLine("     UNCHARTED: BLACKBEARD'S FINAL DESTINATION");
            Console.WriteLine("=====================================================");
            Console.WriteLine("1. New Game");
            Console.WriteLine("2. Continue Game");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            // Initialize game components
            Hero hero = new Hero();
            Tree roomTree = new Tree();
            Map map = new Map(roomTree);
            Items items = new Items();

            string levelsFolder = Path.Combine(Directory.GetCurrentDirectory(), "levels");
            string[] levels = {
                Path.Combine(levelsFolder, "level1.txt"),
                Path.Combine(levelsFolder, "level2.txt"),
                Path.Combine(levelsFolder, "level3.txt"),
                Path.Combine(levelsFolder, "level4.txt"),
                Path.Combine(levelsFolder, "level5.txt"),
                Path.Combine(levelsFolder, "level6.txt"),
                Path.Combine(levelsFolder, "level7.txt")
            };

            if (!Directory.Exists(levelsFolder) || !ValidateLevelFiles(levels))
            {
                Console.WriteLine("Error: Missing 'levels' folder or level files.");
                return;
            }

            int currentLevel = 0;
            if (choice == "2" && File.Exists("game_save.txt"))
            {
                (hero, map, items, currentLevel) = LoadGame(levels);
                Console.WriteLine("Game loaded successfully!");
                Thread.Sleep(2000);
            }
            else
            {
                map.LoadMapFromFile(levels[currentLevel]);
                // SetupChallenges(roomTree);
                Console.WriteLine("Starting a new game...");
                Thread.Sleep(2000);
                ShowIntro();
            }

            bool gameRunning = true;
            int hungerTimer = 0;

            do
            {
                Console.Clear();
                map.DisplayMap(hero.PositionX, hero.PositionY);
                Console.WriteLine($"Health: {hero.Health}, Food: {hero.Food}, Water: {hero.Water}");
                Console.WriteLine($"The hero's coordinates are: {hero.PositionX}, {hero.PositionY}");
                Items.ManageInventory(hero);

                int currentRoom = map.MapGrid[hero.PositionX, hero.PositionY];
                map.HandleRoomInteraction(hero.PositionX, hero.PositionY, hero);

                hungerTimer++;
                if (hungerTimer >= 20) // Every 20 ticks, decrease food and water
                {
                    hero.Food -= 2;
                    hero.Water -= 5;
                    if (hero.Food <= 0 || hero.Water <= 0)
                    {
                        hero.TakeDamage(5); // Lose health if starving or dehydrated
                        Console.WriteLine("You are starving or dehydrated! Find food and water quickly.");
                    }
                    hungerTimer = 0;
                }

                Console.WriteLine("Move using W (up), A (left), S (down), D (right). Press Q to quit.");
                ConsoleKey moveKey = Console.ReadKey(true).Key;

                if (moveKey == ConsoleKey.Q)
                {
                    SaveGame(hero, map, items, currentLevel);
                    Console.WriteLine("Game saved! Goodbye!");
                    break;
                }

                int newX = hero.PositionX;
                int newY = hero.PositionY;

                switch (moveKey)
                {
                    case ConsoleKey.W: newX--; break;
                    case ConsoleKey.A: newY--; break;
                    case ConsoleKey.S: newX++; break;
                    case ConsoleKey.D: newY++; break;
                    default: Console.WriteLine("Invalid input! Use W, A, S, or D to move."); continue;
                }

                if (!IsValidMove(map, newX, newY))
                {
                    Console.WriteLine("You can't move there. It's either a wall or out of bounds!");
                }
                else
                {
                    hero.PositionX = newX;
                    hero.PositionY = newY;
                }

                if (!hero.IsAlive())
                {
                    Console.WriteLine("You have perished. Game Over.");
                    gameRunning = false;
                }

            } while (gameRunning);

            Console.WriteLine("Thanks for playing!");
        }

        static bool IsValidMove(Map map, int x, int y)
        {
            return x >= 0 && x < map.MapGrid.GetLength(0) &&
                   y >= 0 && y < map.MapGrid.GetLength(1) &&
                   map.MapGrid[x, y] != -1; // Ensure not a wall
        }

        static void ShowIntro()
        {
            Console.Clear();
            TypewriterEffect("You are Nathan Drake, a legendary treasure hunter.", 50);
            Thread.Sleep(1000);
            TypewriterEffect("Your mission: Discover Blackbeard’s legendary treasure.", 50);
            Thread.Sleep(1000);
            TypewriterEffect("But beware: traps, puzzles, and rival treasure hunters stand in your way.", 50);
            Thread.Sleep(1000);
            TypewriterEffect("Use your skills: Strength for combat, Agility for traps, and Intelligence for puzzles.", 50);
            Thread.Sleep(1000);
            TypewriterEffect("Press 'S' at any time to skip this intro.", 50);

            if (Console.ReadKey(true).Key == ConsoleKey.S) return;
        }

        static void SaveGame(Hero hero, Map map, Items items, int level)
        {
            using (StreamWriter writer = new StreamWriter("game_save.txt"))
            {
                writer.WriteLine(level);
                writer.WriteLine(hero.PositionX);
                writer.WriteLine(hero.PositionY);
                writer.WriteLine(hero.Health);
                writer.WriteLine(hero.Food);
                writer.WriteLine(hero.Water);
                writer.WriteLine(hero.CurrentRoomNumber);
                writer.WriteLine(string.Join(",", hero.Inventory.ToArray()));
            }
        }

        static (Hero, Map, Items, int) LoadGame(string[] levels)
        {
            string[] saveData = File.ReadAllLines("game_save.txt");
            int level = int.Parse(saveData[0]);
            int row = int.Parse(saveData[1]);
            int col = int.Parse(saveData[2]);
            int health = int.Parse(saveData[3]);
            int food = int.Parse(saveData[4]);
            int water = int.Parse(saveData[5]);
            int currentRoomNumber = int.Parse(saveData[6]);
            string[] inventory = saveData[7].Split(',');

            Hero hero = new Hero
            {
                PositionX = row,
                PositionY = col,
                Health = health,
                Food = food,
                Water = water,
                CurrentRoomNumber = currentRoomNumber
            };
            foreach (var item in inventory)
            {
                hero.AddItem(item);
            }

            Map map = new Map(new Tree());
            map.LoadMapFromFile(levels[level]);

            Items items = new Items();

            return (hero, map, items, level);
        }

        static bool ValidateLevelFiles(string[] levels)
        {
            foreach (string levelPath in levels)
            {
                if (!File.Exists(levelPath)) return false;
            }
            return true;
        }

        static void TypewriterEffect(string text, int delay = 50)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }

        // static void SetupChallenges(Tree challenges)
        // {
        //     challenges.AddChallenge(3, "Trap");
        //     challenges.AddChallenge(5, "Puzzle");
        //     challenges.AddChallenge(7, "Combat");
        // }
    }
}

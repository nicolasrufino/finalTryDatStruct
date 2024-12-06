using System;
using System.Collections.Generic;

namespace TreasureHuntGame
{
    public class Hero
    {
        // Attributes
        public int Health { get; set; }
        public int Food { get; set; }
        public int Water { get; set; }
        public int Strength { get; private set; }
        public int Agility { get; private set; }
        public int Intelligence { get; private set; }
        public int PositionX { get; set; }
        public int CurrentRoomNumber { get; set; }
        public int PositionY { get; set; }
        public List<string> Inventory { get; private set; } = new List<string>();
        public string EquippedItem { get; private set; } // For currently equipped item

        private Timer resourceTimer;

        public Hero()
        {
            // Initialize hero with default values
            Health = 100;
            Food = 70;
            Water = 70;

            // Set default position (bottom middle of an 11x11 map)
            PositionX = 9;
            PositionY = 5;

            // Assign default attributes
            Strength = 5;
            Agility = 5;
            Intelligence = 5;

            // Start with two items in the inventory
            Inventory.Add("Water");
            Inventory.Add("Knife");

            // Default equipped item
            EquippedItem = "Knife";

            // Start the timer for resource depletion
            resourceTimer = new Timer(ResourceDepletion, null, 0, 20000); // Every 20 seconds
        }

        // Resource depletion logic
        private void ResourceDepletion(object state)
        {
            Water = Math.Max(Water - 5, 0);
            Food = Math.Max(Food - 2, 0);

            if (Water <= 10 && Food <= 10)
            {
                TakeDamage(5); // Health diminishes when both water and food are critically low
                Console.WriteLine("You are dehydrated and starving! Your health is diminishing.");
            }
        }

        // Check if the hero is alive
        public bool IsAlive() => Health > 0;

        // Take damage
        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }

        // Heal the hero
        public void Heal(int amount)
        {
            Health = Math.Min(Health + amount, 100);
        }

        // Consume food or water
        public void ConsumeResource(string resource)
        {
            switch (resource)
            {
                case "Food":
                    Food = Math.Min(Food + 20, 70);
                    Console.WriteLine("You consumed food. Food level: " + Food);
                    break;
                case "Water":
                    Water = Math.Min(Water + 20, 70);
                    Console.WriteLine("You drank water. Water level: " + Water);
                    break;
            }
        }

        // Use item (e.g., healing potion)
        public void UseItem(string item)
        {
            if (EquippedItem == item)
            {
                switch (item)
                {
                    case "Health Potion":
                        Heal(20);
                        Console.WriteLine("You used a Health Potion. Health: " + Health);
                        break;
                    case "Food":
                    case "Water":
                        ConsumeResource(item);
                        break;
                    default:
                        Console.WriteLine($"You can't use {item} directly.");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"{item} is not equipped.");
            }
        }

        // Add an item to the inventory
        public void AddItem(string item)
        {
            if (Inventory.Count >= 5)
            {
                Console.WriteLine("Inventory is full! Removing the oldest item.");
                Inventory.RemoveAt(0);
            }

            Inventory.Add(item);
            Console.WriteLine($"{item} has been added to your inventory.");
        }

        // Remove an item from the inventory
        public void RemoveItem(string item)
        {
            if (Inventory.Contains(item))
            {
                Inventory.Remove(item);
                Console.WriteLine($"{item} has been removed from your inventory.");
            }
            else
            {
                Console.WriteLine($"{item} is not in your inventory.");
            }
        }

        // Equip an item
        public void EquipItem(string item)
        {
            if (Inventory.Contains(item))
            {
                EquippedItem = item;
                Console.WriteLine($"You equipped {item}.");
            }
            else
            {
                Console.WriteLine($"You do not have {item} in your inventory to equip.");
            }
        }
    }
}

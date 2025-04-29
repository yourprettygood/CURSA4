using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AuctionSystem
{
    public static class DataStore
    {
        public static List<Auction> Auctions { get; set; } = new List<Auction>();
        public static List<Item> Items { get; set; } = new List<Item>();
        public static List<Seller> Sellers { get; set; } = new List<Seller>();
        public static List<Buyer> Buyers { get; set; } = new List<Buyer>();

        private const string AuctionsFile = "auctions.txt";
        private const string ItemsFile = "items.txt";
        private const string SellersFile = "sellers.txt";
        private const string BuyersFile = "buyers.txt";
        private const string Delimiter = "|";

        public static void Load()
        {
            Auctions = LoadAuctions();
            Items = LoadItems();
            Sellers = LoadSellers();
            Buyers = LoadBuyers();
        }

        public static void Save()
        {
            SaveAuctions();
            SaveItems();
            SaveSellers();
            SaveBuyers();
        }

        private static List<Auction> LoadAuctions()
        {
            var list = new List<Auction>();
            if (!File.Exists(AuctionsFile)) return list;
            foreach (var line in File.ReadAllLines(AuctionsFile))
            {
                var parts = line.Split(new[] { Delimiter }, StringSplitOptions.None);
                if (parts.Length != 5) continue;
                list.Add(new Auction
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    Location = parts[2],
                    Date = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Specification = parts[4]
                });
            }
            return list;
        }

        private static void SaveAuctions()
        {
            var lines = Auctions.Select(a => string.Join(Delimiter,
                a.Id, a.Name, a.Location, a.Date.ToString("yyyy-MM-dd"), a.Specification));
            File.WriteAllLines(AuctionsFile, lines);
        }

        private static List<Item> LoadItems()
        {
            var list = new List<Item>();
            if (!File.Exists(ItemsFile)) return list;
            foreach (var line in File.ReadAllLines(ItemsFile))
            {
                var parts = line.Split(new[] { Delimiter }, StringSplitOptions.None);
                if (parts.Length != 6) continue;
                list.Add(new Item
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    Specification = parts[2],
                    StartPrice = decimal.Parse(parts[3]),
                    FinalPrice = string.IsNullOrWhiteSpace(parts[4]) ? (decimal?)null : decimal.Parse(parts[4]),
                    AuctionId = int.Parse(parts[5])
                });
            }
            return list;
        }

        private static void SaveItems()
        {
            var lines = Items.Select(i => string.Join(Delimiter,
                i.Id, i.Name, i.Specification, i.StartPrice,
                i.FinalPrice?.ToString() ?? "", i.AuctionId));
            File.WriteAllLines(ItemsFile, lines);
        }

        private static List<Seller> LoadSellers()
        {
            var list = new List<Seller>();
            if (!File.Exists(SellersFile)) return list;
            foreach (var line in File.ReadAllLines(SellersFile))
            {
                var parts = line.Split(new[] { Delimiter }, StringSplitOptions.None);
                if (parts.Length != 4) continue;
                list.Add(new Seller
                {
                    Id = int.Parse(parts[0]),
                    FullName = parts[1],
                    PassportNumber = parts[2],
                    ItemId = int.Parse(parts[3])
                });
            }
            return list;
        }

        private static void SaveSellers()
        {
            var lines = Sellers.Select(s => string.Join(Delimiter,
                s.Id, s.FullName, s.PassportNumber, s.ItemId));
            File.WriteAllLines(SellersFile, lines);
        }

        private static List<Buyer> LoadBuyers()
        {
            var list = new List<Buyer>();
            if (!File.Exists(BuyersFile)) return list;
            foreach (var line in File.ReadAllLines(BuyersFile))
            {
                var parts = line.Split(new[] { Delimiter }, StringSplitOptions.None);
                if (parts.Length != 4) continue;
                list.Add(new Buyer
                {
                    Id = int.Parse(parts[0]),
                    FullName = parts[1],
                    PassportNumber = parts[2],
                    ItemId = int.Parse(parts[3])
                });
            }
            return list;
        }

        private static void SaveBuyers()
        {
            var lines = Buyers.Select(b => string.Join(Delimiter,
                b.Id, b.FullName, b.PassportNumber, b.ItemId));
            File.WriteAllLines(BuyersFile, lines);
        }
    }
}

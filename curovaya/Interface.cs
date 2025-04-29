using System;
using System.Globalization;
using System.Linq;

namespace AuctionSystem
{
    class Program
    {
        static void Main()
        {


            DataStore.Load();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Аукционная система ===");
                Console.WriteLine("1. Добавить аукцион");
                Console.WriteLine("2. Добавить предмет");
                Console.WriteLine("3. Удалить предмет");
                Console.WriteLine("4. Зарегистрировать продажу предмета");
                Console.WriteLine("5. Просмотреть аукционы");
                Console.WriteLine("6. Просмотреть предметы");
                Console.WriteLine("7. Запросы");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1": AddAuction(); break;
                    case "2": AddItem(); break;
                    case "3": RemoveItem(); break;
                    case "4": RegisterSale(); break;
                    case "5": ViewAuctions(); break;
                    case "6": ViewItems(); break;
                    case "7": RunQueries(); break;
                    case "0": DataStore.Save(); return;
                    default: Console.WriteLine("Неверный ввод."); break;
                }
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        static void AddAuction()
        {
            Console.Write("Название: ");
            string name = Console.ReadLine();
            Console.Write("Место: ");
            string location = Console.ReadLine();
            Console.Write("Дата (yyyy-MM-dd): ");
            DateTime date = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            Console.Write("Спецификация (напр. 'Картины до 1900 г.'): ");
            string spec = Console.ReadLine();

            int id = DataStore.Auctions.Any() ? DataStore.Auctions.Max(a => a.Id) + 1 : 1;
            DataStore.Auctions.Add(new Auction { Id = id, Name = name, Location = location, Date = date, Specification = spec });
            Console.WriteLine("Аукцион добавлен.");
        }

        static void AddItem()
        {
            Console.Write("Название предмета: ");
            string name = Console.ReadLine();
            Console.Write("Спецификация: ");
            string spec = Console.ReadLine();
            Console.Write("Начальная цена: ");
            decimal startPrice = decimal.Parse(Console.ReadLine());
            Console.Write("ID аукциона: ");
            int auctionId = int.Parse(Console.ReadLine());
            var auction = DataStore.Auctions.FirstOrDefault(a => a.Id == auctionId);
            if (auction == null)
            {
                Console.WriteLine("Аукцион не найден.");
                return;
            }

            Console.Write("ФИО продавца: ");
            string sellerName = Console.ReadLine();
            Console.Write("Паспорт продавца: ");
            string sellerPassport = Console.ReadLine();

            int itemId = DataStore.Items.Any() ? DataStore.Items.Max(i => i.Id) + 1 : 1;
            DataStore.Items.Add(new Item { Id = itemId, Name = name, Specification = spec, StartPrice = startPrice, FinalPrice = null, AuctionId = auctionId });

            int sellerId = DataStore.Sellers.Any() ? DataStore.Sellers.Max(s => s.Id) + 1 : 1;
            DataStore.Sellers.Add(new Seller { Id = sellerId, FullName = sellerName, PassportNumber = sellerPassport, ItemId = itemId });

            Console.WriteLine("Предмет и продавец добавлены.");
        }

        static void RemoveItem()
        {
            Console.Write("ID предмета для удаления: ");
            int itemId = int.Parse(Console.ReadLine());

            var item = DataStore.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                Console.WriteLine("Предмет не найден.");
                return;
            }

            DataStore.Items.Remove(item);
            DataStore.Sellers.RemoveAll(s => s.ItemId == itemId);
            DataStore.Buyers.RemoveAll(b => b.ItemId == itemId);

            Console.WriteLine("Предмет и связанные записи удалены.");
        }

        static void RegisterSale()
        {
            Console.Write("ID предмета, проданного: ");
            int itemId = int.Parse(Console.ReadLine());
            var item = DataStore.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null || item.FinalPrice != null)
            {
                Console.WriteLine("Предмет не найден или уже продан.");
                return;
            }

            Console.Write("Конечная цена: ");
            decimal finalPrice = decimal.Parse(Console.ReadLine());
            Console.Write("ФИО покупателя: ");
            string name = Console.ReadLine();
            Console.Write("Паспорт: ");
            string passport = Console.ReadLine();

            item.FinalPrice = finalPrice;
            int buyerId = DataStore.Buyers.Any() ? DataStore.Buyers.Max(b => b.Id) + 1 : 1;
            DataStore.Buyers.Add(new Buyer { Id = buyerId, FullName = name, PassportNumber = passport, ItemId = itemId });

            Console.WriteLine("Продажа зарегистрирована.");
        }

        static void ViewAuctions()
        {
            foreach (var a in DataStore.Auctions)
                Console.WriteLine($"ID:{a.Id}: ИМЯ:{a.Name}, Место:{a.Location}, Дата:{a.Date:yyyy-MM-dd}, Спецификация:{a.Specification}");
        }

        static void ViewItems()
        {
            foreach (var i in DataStore.Items)
            {
                var auction = DataStore.Auctions.FirstOrDefault(a => a.Id == i.AuctionId);
                Console.WriteLine($"ID:{i.Id}: ИМЯ:{i.Name}, Спецификация:{i.Specification}, нач. цена: {i.StartPrice}, " +
                    $"кон. цена: {(i.FinalPrice?.ToString() ?? "—")}, аукцион: {auction?.Name}");
            }
        }

        static void RunQueries()
        {
            Console.Clear();
            Console.WriteLine("=== Запросы ===");
            Console.WriteLine("1. Предметы на дату и аукцион");
            Console.WriteLine("2. Аукционы по специфике");
            Console.WriteLine("3. Предмет с макс. разницей в цене");
            Console.WriteLine("4. Аукцион с наибольшим числом продаж");
            Console.WriteLine("5. Покупатель самого дорогого лота");
            Console.WriteLine("6. Продавец самого дорогого лота");
            Console.Write("Выберите запрос: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": QueryItemsByDateAndAuction(); break;
                case "2": QueryAuctionsBySpec(); break;
                case "3": QueryMaxPriceDiffItem(); break;
                case "4": QueryAuctionWithMostSales(); break;
                case "5": QueryBuyerOfMostExpensive(); break;
                case "6": QuerySellerOfMostExpensive(); break;
                default: Console.WriteLine("Неверный ввод."); break;
            }
        }

        static void QueryItemsByDateAndAuction()
        {
            Console.Write("Дата (yyyy-MM-dd): ");
            DateTime date = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            Console.Write("ID аукциона: ");
            int auctionId = int.Parse(Console.ReadLine());

            var items = DataStore.Items
                .Where(i => i.AuctionId == auctionId)
                .Join(DataStore.Auctions.Where(a => a.Date == date && a.Id == auctionId),
                      i => i.AuctionId,
                      a => a.Id,
                      (i, a) => i);

            foreach (var i in items)
                Console.WriteLine($"{i.Id}: {i.Name}, {i.Specification}, нач. цена: {i.StartPrice}");
        }

        static void QueryAuctionsBySpec()
        {
            Console.Write("Спецификация: ");
            string spec = Console.ReadLine();
            var auctions = DataStore.Auctions.Where(a => a.Specification == spec);
            foreach (var a in auctions)
                Console.WriteLine($"{a.Id}: {a.Name}, {a.Date:yyyy-MM-dd}");
        }

        static void QueryMaxPriceDiffItem()
        {
            var item = DataStore.Items
                .Where(i => i.FinalPrice.HasValue)
                .OrderByDescending(i => i.FinalPrice.Value - i.StartPrice)
                .FirstOrDefault();

            if (item != null)
                Console.WriteLine($"ID {item.Id}: {item.Name}, разница: {item.FinalPrice - item.StartPrice}");
            else
                Console.WriteLine("Нет проданных предметов.");
        }

        static void QueryAuctionWithMostSales()
        {
            var result = DataStore.Items
                .Where(i => i.FinalPrice.HasValue)
                .GroupBy(i => i.AuctionId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (result != null)
            {
                var auction = DataStore.Auctions.FirstOrDefault(a => a.Id == result.Key);
                Console.WriteLine($"Аукцион: {auction?.Name}, Продано предметов: {result.Count()}");
            }
            else
                Console.WriteLine("Нет данных о продажах.");
        }

        static void QueryBuyerOfMostExpensive()
        {
            var item = DataStore.Items
                .Where(i => i.FinalPrice.HasValue)
                .OrderByDescending(i => i.FinalPrice)
                .FirstOrDefault();

            if (item == null)
            {
                Console.WriteLine("Нет проданных предметов.");
                return;
            }

            var buyer = DataStore.Buyers.FirstOrDefault(b => b.ItemId == item.Id);
            if (buyer != null)
                Console.WriteLine($"Покупатель: {buyer.FullName}, Паспорт: {buyer.PassportNumber}");
            else
                Console.WriteLine("Покупатель не найден.");
        }

        static void QuerySellerOfMostExpensive()
        {
            var item = DataStore.Items
                .Where(i => i.FinalPrice.HasValue)
                .OrderByDescending(i => i.FinalPrice)
                .FirstOrDefault();

            if (item == null)
            {
                Console.WriteLine("Нет проданных предметов.");
                return;
            }

            var seller = DataStore.Sellers.FirstOrDefault(s => s.ItemId == item.Id);
            if (seller != null)
                Console.WriteLine($"Продавец: {seller.FullName}, Паспорт: {seller.PassportNumber}");
            else
                Console.WriteLine("Продавец не найден.");
        }
    }
}

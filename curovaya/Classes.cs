using System;

namespace AuctionSystem
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specification { get; set; }
        public decimal StartPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public int AuctionId { get; set; }
    }

    public class Seller
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PassportNumber { get; set; }
        public int ItemId { get; set; }
    }

    public class Buyer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PassportNumber { get; set; }
        public int ItemId { get; set; }
    }

    public class Auction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string Specification { get; set; }
    }
}

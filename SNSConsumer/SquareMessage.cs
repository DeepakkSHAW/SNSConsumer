using System;
using System.Collections.Generic;
using System.Text;

namespace SNSConsumer
{
    public class BasePriceMoney
    {
        public int amount { get; set; }
        public string currency { get; set; }
    }

    public class LineItem
    {
        public string quantity { get; set; }
        public BasePriceMoney base_price_money { get; set; }
        public string name { get; set; }
        public string note { get; set; }
    }

    public class Order
    {
        public string reference_id { get; set; }
        public string customer_id { get; set; }
        public string location_id { get; set; }
        public IList<LineItem> line_items { get; set; }
    }

    public class OrderFromSNS
    {
        public int store_id { get; set; }
        public string idempotency_key { get; set; }
        public Order order { get; set; }
    }

}

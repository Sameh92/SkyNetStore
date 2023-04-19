using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<DeliveryMethod> _dmRepo;
        private readonly IBasketRepository _basketRepo;

        public OrderService(IGenericRepository<Product> productRepo,IGenericRepository<Order> orderRepo
            ,IGenericRepository<DeliveryMethod> dmRepo,IBasketRepository basketRepo)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _dmRepo = dmRepo;
            _basketRepo = basketRepo;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            // get basket from the repo
            var basket = await _basketRepo.GetBasketAsync(basketId);

            // get items frim the product repo
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await _productRepo.GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }

            // get delivery method from repo
            var deliveryMethod = await _dmRepo.GetByIdAsync(deliveryMethodId);

            // calc subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            // create Order
            var order= new Order(items,buyerEmail,shippingAddress,deliveryMethod,subtotal);

            // save to db (later)
            /*
             there is a reason we've deferred the saving of the database in here, and just Expand what's going on
             and if I we take a look at this, which represents our order service, then we can see that we've got out
             repositories being created and injected into here .
             Now we've got three generic repositories and we have a single Basket Repository being instantiated
             when we create this order service 
             and if we expand the repos , then we can see that this has an instance of our StoreDb Context
             If we expand the Order repo , we can see that this has an instance of our StoreDb Context 
             If we expand the Product repo, it's also got an instance of our storeDb Context
             If we expand the DmRepo, it's also got an instance of our storeDb Context
             If we expand the BasketRepo, it's also got an instance of our Basket Context
              
             is this good thing or bad thing ?
            -in our case at the moment, what we're actually going to be doing is we're only going to be saving 
             ONE entity to the database ,
             we are NOT saving TWO different entities
            -But what we do want to do is consider the possibility that we might be there is a possibility that
             we might want to create an inventory system and give the user the ability
             to create a product and new product brand at the same time, that would involve saving data to 
             TWO Different Tables, and if we use the same generic repository that would mean 
             that would have to Repositories their own instances of two different StoreDb Contexts
            -Now imagine one of them works and one of them didn't , then we'd have a partial update in our database
            and we have so many injections inside our constructer until now three
             

             */
            // return order
            return order;

        }

        public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderByOrderIdAsync(int id, string buyerEmail)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            throw new NotImplementedException();
        }
    }
}

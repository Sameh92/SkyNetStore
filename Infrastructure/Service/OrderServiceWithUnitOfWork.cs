using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class OrderServiceWithUnitOfWork : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepo;

        public OrderServiceWithUnitOfWork(IBasketRepository basketRepo, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }

            // get delivery method from repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // calc subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            // create Order
            var order= new Order(items,buyerEmail,shippingAddress,deliveryMethod,subtotal);
            _unitOfWork.Repository<Order>().Add(order);
            // save to db

            var result=await _unitOfWork.Complete();/*because our unit of work owns our DbContext , any changes 
             that tracked by ef core are going to be saved into database at this point 
             if this fail, then any changes that have taken place inside our method here are going to be 
             rolled back and will send an error
            */
            if (result <= 0)
                return null;

            // delete basket
            await _basketRepo.DeleteBasketAsync(basketId);

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

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public  async Task<Order> GetOrderByOrderIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);

            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);

            return await _unitOfWork.Repository<Order>().ListAsyncWitSpec(spec);
        }
    }
}

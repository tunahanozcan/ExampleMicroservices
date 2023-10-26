using AutoMapper;
using Course.Services.Order.Domain.OrderAggregate;

namespace Course.Services.Order.Application.Mapping
{
    public class CustomMapping:Profile
    {
        public CustomMapping()
        {
            CreateMap<Domain.OrderAggregate.Order, Dtos.OrderDto>().ReverseMap();
            CreateMap<OrderItem, Dtos.OrderItemDto>().ReverseMap();
            CreateMap<Address, Dtos.AddressDto>().ReverseMap();
        }
    }
}

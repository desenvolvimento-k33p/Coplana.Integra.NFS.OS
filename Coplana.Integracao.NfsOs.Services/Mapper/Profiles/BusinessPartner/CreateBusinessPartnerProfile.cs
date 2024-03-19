using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coplana.Integracao.NfsOs.Services.Mapper.Profiles.BusinessPartner
{
    public class CreateBusinessPartnerProfile : Profile
    {
        public CreateBusinessPartnerProfile()
        {
            //CreateMap<DetailOrder, BusinessPartnersSAP>()
            //    .ForMember(dst => dst.CardName, org => org.MapFrom(a => String.IsNullOrEmpty(a.Shipping.ReceiverAddress.Name) ?
            //                                                            a.Buyer.FullName :
            //                                                            a.Shipping.ReceiverAddress.Name))
            //    //.ForMember(dst => dst.Address, org => org.MapFrom(a => a.Shipping.ReceiverAddress.AddressLine))
            //    //.ForMember(dst => dst.ZipCode, org => org.MapFrom(a => a.Shipping.ReceiverAddress.ZipCode))
            //    .ForMember(dst => dst.Phone1, org => org.MapFrom(a => a.Buyer.Phone.Number))
            //    .ForMember(dst => dst.Cellular, org => org.MapFrom(a => a.Buyer.AlternativePhone.Number))
            //    //.ForMember(dst => dst.City, org => org.MapFrom(a => a.Shipping.ReceiverAddress.City.Name))
            //    .ForMember(dst => dst.EmailAddress, org => org.MapFrom(a => a.Buyer.Email));
        }
    }
}

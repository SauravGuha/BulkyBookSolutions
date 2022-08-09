

using BulkyBook.Common.Interfaces.ModelRepositories;

namespace BulkyBook.Common.Interfaces
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; set; }

        ICoverTypeRepository CoverTypeRepository { get; set; }

        IProductRepository ProductRepository { get; set; }

        ICompanyRepository CompanyRepository { get; set; }

        IFeatureRepository FeatureRepository { get; set; }

        IShoppingCartRepository ShoppingCartRepository { get; set; }

        IOrderHeaderRepository OrderHeaderRepository { get; set; }

        IOrderDetailsRepository OrderDetailsRepository { get; set; }

        IAddressRepository AddressRepository { get; set; }

        void SaveChanges();
    }
}

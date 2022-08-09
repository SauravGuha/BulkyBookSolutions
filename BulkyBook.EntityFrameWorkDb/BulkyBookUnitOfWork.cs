

using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories;

namespace BulkyBook.EntityFrameWorkDb
{
    public class BulkyBookUnitOfWork : IUnitOfWork
    {
        private readonly BulkyBookDbContext _bulkyBookDbContext;

        public BulkyBookUnitOfWork(BulkyBookDbContext bulkyBookDbContext)
        {
            this._bulkyBookDbContext = bulkyBookDbContext;
            CategoryRepository = new CategoryRepo(bulkyBookDbContext);
            CoverTypeRepository = new CoverTypeRepo(bulkyBookDbContext);
            ProductRepository = new ProductRepo(bulkyBookDbContext);
            CompanyRepository = new CompanyRepo(bulkyBookDbContext);
            FeatureRepository = new FeatureRepo(bulkyBookDbContext);
            ShoppingCartRepository = new ShoppingCartRepo(bulkyBookDbContext);
            OrderHeaderRepository = new OrderHeaderRepo(bulkyBookDbContext);
            OrderDetailsRepository = new OrderDetailRepo(bulkyBookDbContext);
            AddressRepository = new AddressRepo(bulkyBookDbContext);
        }

        public ICategoryRepository CategoryRepository { get; set; }
        public ICoverTypeRepository CoverTypeRepository { get; set; }
        public IProductRepository ProductRepository { get; set; }
        public ICompanyRepository CompanyRepository { get; set; }
        public IFeatureRepository FeatureRepository { get; set; }
        public IShoppingCartRepository ShoppingCartRepository { get; set; }
        public IOrderHeaderRepository OrderHeaderRepository { get; set; }
        public IOrderDetailsRepository OrderDetailsRepository { get; set; }

        public IAddressRepository AddressRepository { get; set; }

        public void SaveChanges()
        {
            _bulkyBookDbContext.SaveChanges();
        }
    }
}

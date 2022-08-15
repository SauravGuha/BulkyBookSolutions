
using BulkyBook.Common.Interfaces.ModelRepositories;
using BulkyBook.Common.Models;

namespace BulkyBook.EntityFrameWorkDb.EntityFrameworkRepositories
{
    public class FeatureRepo :BulkyBookRepo<Feature>, IFeatureRepository
    {

        public FeatureRepo(BulkyBookDbContext bulkyBookDbContext):base(bulkyBookDbContext)
        {
        }

    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RH.EntityFramework.Shared.DbContexts;

namespace RH.EntityFramework.Repositories.Label
{
    public class LabelRepository: ILabelRepository
    {
        private WeatherDbContext _dbContext;

        public LabelRepository(WeatherDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Shared.Entities.Label>> GetLabelsByDimensionId(int dimensionId)
        {
            return await _dbContext.Labels.Where(x => x.DimensionId == dimensionId).ToListAsync();
        }

        public async Task<Shared.Entities.Label> Add(Shared.Entities.Label label)
        {
            var dbRecord =
                await _dbContext.Labels.FirstOrDefaultAsync(x =>
                    x.DimensionId == label.DimensionId && x.Name == label.Name);
            if (dbRecord==null)
            {
                _dbContext.Labels.Add(label);
            }
            else
            {
                dbRecord.O = label.O;
                dbRecord.X = label.X;
                dbRecord.Y = label.Y;
                dbRecord.ExtraField1 = label.ExtraField1;
                dbRecord.ExtraField2 = label.ExtraField2;
                dbRecord.Type = label.Type;
                dbRecord.FullText = label.FullText;
                dbRecord.RegisterDate = label.RegisterDate;
                label.Id = dbRecord.Id;
            }

            await _dbContext.SaveChangesAsync();

            return label;
        }
    }
}

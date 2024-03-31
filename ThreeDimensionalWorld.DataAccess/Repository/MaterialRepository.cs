using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;


namespace ThreeDimensionalWorld.DataAccess.Repository
{
    public class MaterialRepository : Repository<Material>
    {
        public MaterialRepository(ApplicationDbContext db)
            : base(db)
        {
        }

        public override void Update(Material entity)
        {
            _db.Entry(entity).State = EntityState.Detached;
            Material? material = dbSet.Include(m => m.Colors).FirstOrDefault(m => m.Id == entity.Id);
            
            if(material == null)
            {
                throw new ArgumentException("The entity is not valid");
            }

            List<int> ids = entity.Colors.Select(c => c.Id).ToList();
            material.Colors = _db.MaterialColors.Where(c => ids.Contains(c.Id)).ToList();

            List<string> excludedProperties = new List<string>() { "Colors" };
            foreach (var sourceProp in entity.GetType().GetProperties().Where(p => !excludedProperties.Contains(p.Name)))
                material.GetType().GetProperty(sourceProp.Name)?.SetValue(material, sourceProp.GetValue(entity));

            base.Update(material);
        }
    }
}

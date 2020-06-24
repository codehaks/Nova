using Microsoft.EntityFrameworkCore;
using System;

namespace Portal.ImageCropService.Data
{
    public class ImageDbContext : DbContext
    {
        public ImageDbContext(DbContextOptions<ImageDbContext> options)
                : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }


    }

    public class Image
    {
        public Guid Id { get; set; }
        
        public Byte[] Full { get; set; }
        public Byte[] Thumb { get; set; }
        public DateTime TimeCreated { get; set; }

    }
}

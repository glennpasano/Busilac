using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Busilac.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Materials> Materials { get; set; }
        public DbSet<MaterialType> MaterialType { get; set; }
        public DbSet<MaterialsInventory> MaterialsInventory { get; set; }
        public DbSet<MaterialsDeliveryReceipt> MaterialsDeliveryReceipt { get; set; }
        public DbSet<MaterialsDeliveryReceiptDetails> MaterialsDeliveryReceiptDetails { get; set; }
        public DbSet<MaterialsSalesOrders> MaterialsSalesOrders { get; set; }
        public DbSet<MaterialsSalesOrdersDetails> MaterialsSalesOrdersDetails { get; set; }
        public DbSet<MaterialsStatus> MaterialsStatus { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductInventory> ProductInventory { get; set; }
        public DbSet<ProductBuildMaterials> ProductBuildMaterials { get; set; }
        public DbSet<ProductManufactureOrders> ProductManufactureOrders { get; set; }
        public DbSet<ProductManufactureOrderDetails> ProductManufactureOrderDetails { get; set; }
        public DbSet<ProductSalesOrders> ProductSalesOrders { get; set; }
        public DbSet<ProductSalesOrderDetails> ProductSalesOrderDetails { get; set; }
        public DbSet<ProductStatus> ProductStatus { get; set; }
    }
}
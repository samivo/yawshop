using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using YawShop.Services.EventService.Models;
using YawShop.Services.GiftcardService.Models;
using YawShop.Services.DiscountService.Models;
using YawShop.Services.ClientService.Models;
using YawShop.Services.ProductService.Models;
using YawShop.Services.CheckoutService.Models;
using YawShop.Services.Database;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<ClientModel> Clients { get; set; }
    public DbSet<ProductModel> Products { get; set; }
    public DbSet<EventModel> Events { get; set; }
    public DbSet<GiftcardModel> Giftcards { get; set; }
    public DbSet<DiscountModel> Discounts { get; set; }

    public DbSet<CheckoutModel> Checkouts { get; set; }

    public ApplicationDbContext()
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseMySql(DbConnectionString.GetString(), ServerVersion.AutoDetect(DbConnectionString.GetString()));
    }

}

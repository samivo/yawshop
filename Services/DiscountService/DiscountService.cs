using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using YawShop.Attributes;
using YawShop.Services.DiscountService;
using YawShop.Services.DiscountService.Models;
using YawShop.Utilities;


public class DiscountService : IDiscountService
{

    private readonly ILogger<DiscountService> _logger;
    private readonly ApplicationDbContext _context;

    public DiscountService(ILogger<DiscountService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<List<DiscountModel>> FindAsNoTrackingAsync(Expression<Func<DiscountModel,bool>> predicate)
    {
        try
        {
            var discounts = await _context.Discounts.AsNoTracking().Where(predicate).ToListAsync();

            return discounts;
        }
        catch (System.Exception ex)
        {
            _logger.LogError("Failed to get discount(s): {err}", ex.ToString());
            throw;
        }
    }

    public async Task<List<DiscountModel>> FindAsync(Expression<Func<DiscountModel,bool>> predicate)
    {
        try
        {
            var discounts = await _context.Discounts.Where(predicate).ToListAsync();

            return discounts;
        }
        catch (System.Exception ex)
        {
            _logger.LogError("Failed to get discount(s): {err}", ex.ToString());
            throw;
        }
    }

    public async Task CreateAsync(DiscountModel discountModel)
    {
        try
        {
            if (await _context.Discounts.AnyAsync(d => d.Code == discountModel.Code))
            {
                throw new InvalidOperationException("Duplicate discount code found in database.");
            }

            _context.Discounts.Add(discountModel);
            await _context.SaveChangesAsync();

            return;
        }
        catch (System.Exception ex)
        {
            _logger.LogError("Create discount failed: {err}", ex.ToString());
            throw;
        }
    }

    public async Task RemoveAsync(string discountCode)
    {
        try
        {
            var discount = await _context.Discounts.SingleAsync(d => d.Code == discountCode);
            _context.Remove(discount);
            await _context.SaveChangesAsync();
            
            return;
        }
        catch (System.Exception ex)
        {
            _logger.LogError("Removing discount code failed: {err}", ex.ToString());
            throw;
        }
    }

    public async Task UpdateAsync(string discountCode, DiscountModel updatedDiscount)
    {
        try
        {
            var oldDiscount = await _context.Discounts.SingleAsync(d => d.Code == discountCode);

            PropertyCopy.CopyWithoutAttribute(updatedDiscount,oldDiscount,typeof(NoApiUpdateAttribute));

            await _context.SaveChangesAsync();

            return;
        }
        catch (System.Exception ex)
        {
            _logger.LogError("Updating discount code failed: {err}", ex.ToString());
            throw;
        }
    }

    public async Task AddQuantityUsed(string discountCode, int value)
    {
        try
        {
            var discount = await _context.Discounts.SingleAsync(d => d.Code == discountCode);
            discount.QuantityUsed += value;

            await _context.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            _logger.LogError("Failed to sum quantity used: {err}", ex.ToString());
            throw;
        }
    }
}
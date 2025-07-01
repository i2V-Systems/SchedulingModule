using Microsoft.EntityFrameworkCore;
using SchedulingModule.Application.Context;

namespace SchedulingModule.Infrastructure.Data.Seeder;

public class DatabaseSeederService : IDatabaseSeederService
{
    private readonly ScheduleDbContext _context;
    // private readonly UserManager<ApplicationUser> _userManager;

    private readonly ILogger<DatabaseSeederService> _logger;

    public DatabaseSeederService(
        ScheduleDbContext context,
        // UserManager<ApplicationUser> userManager,
        // RoleManager<ApplicationRole> roleManager,
        ILogger<DatabaseSeederService> logger)
    {
        _context = context;
        // _userManager = userManager;
        // _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            if (_context.Database.IsNpgsql())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database seeding.");
            throw;
        }
    }
} 
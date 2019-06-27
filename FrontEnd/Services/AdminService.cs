using FrontEnd.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEnd.Services
{
    public class AdminService : IAdminService
    {
        private readonly Lazy<long> _creationKey = new Lazy<long>(() => BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 7));
        //If you run the app at this point, you'll see an exception stating that you can't inject a scoped type into a type registered as a singleton.
        //This is the DI system protecting you from a common anti - pattern that can arise when using IoC containers. 
        //private readonly IdentityDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private bool _adminExists;

        //public AdminService(IdentityDbContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}
        public AdminService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public long CreationKey => _creationKey.Value;

        public async Task<bool> AllowAdminUserCreationAsync()
        {
            if (_adminExists)
            {
                return false;
            }
            else
            {
                //create a service scope so we can ask for an instance of the IdentityDbContext within a scoped context
                //mozne fixnut problem s DI pri roznych: AddScoped, AddSingleton, AddTransient
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

                    if (await dbContext.Users.AnyAsync(user => user.IsAdmin))
                    {
                        // There are already admin users so disable admin creation
                        _adminExists = true;
                        return false;
                    }

                    // There are no admin users so enable admin creation
                    return true;
                }
            }
        }
    }
}

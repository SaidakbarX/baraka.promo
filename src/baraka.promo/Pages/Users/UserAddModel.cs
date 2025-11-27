using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Radzen;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Pages.Users
{
    public class UserAddModel
    {
        readonly DialogService _dialogService;
        readonly NotificationService _notificationService;
        readonly IServiceProvider _serviceProvider;
        readonly ILogger<UserAddModel> _logger;
        public UserAddModel(DialogService dialogService,
            NotificationService notificationService,
            IServiceProvider serviceProvider,
            ILogger<UserAddModel> logger)
        {
            _dialogService = dialogService;
            _notificationService = notificationService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public int TotalCount { get; set; }
        public NewUserUIModel Model { get; set; }
        public Action StateHasChanged { get; set; }
        public List<UsersList> Users { get; set; }

        public async Task OnInitializedAsync(string value)
        {
            Model = new NewUserUIModel();
            if (!string.IsNullOrWhiteSpace(value))
                JsonConvert.PopulateObject(value, Model);
            //await LoadCompanyData();
            //await LoadRoleData();
            StateHasChanged?.Invoke();
        }

        public async Task LoadData(LoadDataArgs args = null)
        {
            UserManager<IdentityUser> userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var users = userManager.Users.ToList();
            TotalCount = users.Count;
                var list = new List<UsersList>();
            foreach (var user in users)
            {
                list.Add(new UsersList()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                });
            }
            Users = list;

        }

        public async Task<bool> Delate(UsersList user)
        {
            UserManager<IdentityUser> userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var userToDelete = await userManager.FindByIdAsync(user.Id);

            if (userToDelete != null)
            {
                var result = await userManager.DeleteAsync(userToDelete);
                if (result.Succeeded)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task Save()
        {
            try
            {
                UserManager<IdentityUser> userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

                // Check if the user doesn't exist
                var user = await userManager.FindByIdAsync(Model.Id);
                if (user == null)
                {
                    var newUser = new IdentityUser
                    {
                        UserName = Model.UserName,
                    };
                    
                    // Create the user with a password
                    var result = await userManager.CreateAsync(newUser, Model.Password);

                    if (result.Succeeded)
                    {
                        var regUser = await userManager.FindByNameAsync(Model.UserName);
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(regUser);
                        var confirmationResult = await userManager.ConfirmEmailAsync(regUser, token);
                        // User creation was successful
                        Console.WriteLine($"User {Model.UserName} created successfully.");
                    }
                    else
                    {
                        throw new Exception(result.Errors.FirstOrDefault().ToString());
                    }
                }
                else
                {
                    if (user.UserName != Model.UserName)
                    {
                        user.UserName = Model.UserName;
                        await userManager.UpdateAsync(user);
                    }

                    if (!await userManager.CheckPasswordAsync(user, Model.Password))
                    {
                        // Change the password
                        var token = await userManager.GeneratePasswordResetTokenAsync(user);
                        var changePasswordResult = await userManager.ResetPasswordAsync(user, token, Model.Password);

                    }

                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        
        public void Close()
        {
            _dialogService.Close(null);
        }
        public class NewUserUIModel
        {
            public string Id { get; set; }
            [Required]
            public string UserName { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }
        }

        public class UsersList
        {
            public string Id { get; set; }
            public string UserName { get; set; }

        }
    }
}

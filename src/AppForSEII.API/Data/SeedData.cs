namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }


           
            try
                {
            SeedEditoriales(dbContext);
            }
             catch (Exception ex)
             {
                logger.LogError(ex, "An error occurred seeding the Editoriales in the Database.");
            }

            try
            {
                SeedGeneros(dbContext);
            }
            catch (Exception ex)
            {
            logger.LogError(ex, "An error occurred seeding the Generos in the Database.");
            }
 

        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {

            foreach (string roleName in roles) {
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }


            if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
                //A customer class has been defined because it has different attributes (purchase, rental, etc.)
                ApplicationUser user = new ApplicationUser("3", "Peter", "Jackson", "peter@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //customer role
                    userManager.AddToRoleAsync(user, roles[2]).Wait();

                }
            }

        }

        public static void SeedEditoriales(ApplicationDbContext dbContext) {
             if (!dbContext.Editoriales.Any(e => e.Nombre == "Planeta"))
    {
        dbContext.Database.ExecuteSqlRaw(
            "INSERT INTO [Editoriales] ([Nombre]) VALUES (N'Planeta')");
    }
    
    if (!dbContext.Editoriales.Any(e => e.Nombre == "Anagrama"))
    {
        dbContext.Database.ExecuteSqlRaw(
            "INSERT INTO [Editoriales] ([Nombre]) VALUES (N'Anagrama')");
    }

    if (!dbContext.Editoriales.Any(e => e.Nombre == "Tusquets Editores"))
    {
        dbContext.Database.ExecuteSqlRaw(
            "INSERT INTO [Editoriales] ([Nombre]) VALUES (N'Tusquets Editores')");
    }

    if (!dbContext.Editoriales.Any(e => e.Nombre == "Salamandra"))
    {
        dbContext.Database.ExecuteSqlRaw(
            "INSERT INTO [Editoriales] ([Nombre]) VALUES (N'Salamandra')");
    }

    if (!dbContext.Editoriales.Any(e => e.Nombre == "Penguin Random House"))
    {
        dbContext.Database.ExecuteSqlRaw(
            "INSERT INTO [Editoriales] ([Nombre]) VALUES (N'Penguin Random House')");
    }
        }


    public static void SeedGeneros(ApplicationDbContext dbContext) {
    // Géneros extraídos de tus requisitos y algunos adicionales comunes
    var generos = new List<string> { 
        "Misterio", 
        "Ficción", 
        "Novela", 
        "Fantasía", 
        "Ciencia Ficción", 
        "Ensayo" 
    };

    foreach (var genero in generos) {
        if (!dbContext.Generos.Any(g => g.Nombre == genero)) {
            // Se utiliza parametrización {0} para evitar problemas de formato y seguridad
            dbContext.Database.ExecuteSqlRaw(
                "INSERT INTO [Generos] ([Nombre]) VALUES ({0})", genero);
        }
    }
}


    }
}
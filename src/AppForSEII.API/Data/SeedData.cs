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

            try
            {
                SeedLibros(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the Libros in the Database.");
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
        var generos = new List<string> { 
            "Misterio", 
            "Ficción", 
            "Novela", 
            "Fantasía", 
            "Ciencia Ficción", 
            "Ensayo" 
        };

        bool guardarCambios = false;

        foreach (var genero in generos) {
            // Comprueba si el género ya existe
            if (!dbContext.Generos.Any(g => g.Nombre == genero)) {
                // Usa el método nativo .Add() en lugar de SQL directo
                dbContext.Generos.Add(new Genero { Nombre = genero });
                guardarCambios = true;
            }
        }

        // Si se ha añadido algún género nuevo, guarda los cambios en la BD
        if (guardarCambios) {
            dbContext.SaveChanges();
        }
    }

    public static void SeedLibros(ApplicationDbContext dbContext) {
    // Comprobamos si ya hay libros para no duplicarlos
    if (dbContext.Libros.Any()) {
        return; 
    }

    // Buscamos las entidades relacionadas en la base de datos para obtener sus IDs
    var planeta = dbContext.Editoriales.FirstOrDefault(e => e.Nombre == "Planeta");
    var anagrama = dbContext.Editoriales.FirstOrDefault(e => e.Nombre == "Anagrama");
    var tusquets = dbContext.Editoriales.FirstOrDefault(e => e.Nombre == "Tusquets Editores");
    var salamandra = dbContext.Editoriales.FirstOrDefault(e => e.Nombre == "Salamandra");

    var misterio = dbContext.Generos.FirstOrDefault(g => g.Nombre == "Misterio");
    var ficcion = dbContext.Generos.FirstOrDefault(g => g.Nombre == "Ficción");
    var novela = dbContext.Generos.FirstOrDefault(g => g.Nombre == "Novela");
    var fantasia = dbContext.Generos.FirstOrDefault(g => g.Nombre == "Fantasía");

    // Si por algún motivo faltan las editoriales o géneros, evitamos que la aplicación falle
    if (planeta == null || misterio == null) return;

    // Creamos la lista de libros asignando los IDs de las referencias encontradas
    var libros = new List<Libro> {
        new Libro {
            Titulo = "La sombra del viento",
            Autor = "Carlos Ruiz Zafón",
            PrecioTotal = 20.50m,
            PrecioReposicion = 15.00m,
            Stock = 10,
            TipoLibro = "Tapa Dura",
            CalificacionMedia = 4.8m,
            FechaLanzamiento = new DateTime(2001, 4, 1),
            Editorial = planeta,
            Genero = misterio
        },
        new Libro {
            Titulo = "El juego del ángel",
            Autor = "Carlos Ruiz Zafón",
            PrecioTotal = 22.00m,
            PrecioReposicion = 16.50m,
            Stock = 8,
            TipoLibro = "Bolsillo",
            CalificacionMedia = 4.5m,
            FechaLanzamiento = new DateTime(2008, 4, 17),
            Editorial = planeta,
            Genero = ficcion
        },
        new Libro {
            Titulo = "Los detectives salvajes",
            Autor = "Roberto Bolaño",
            PrecioTotal = 24.00m,
            PrecioReposicion = 18.00m,
            Stock = 5,
            TipoLibro = "Tapa Dura",
            CalificacionMedia = 4.6m,
            FechaLanzamiento = new DateTime(1998, 1, 1),
            Editorial = anagrama,
            Genero = ficcion
        },
        new Libro {
            Titulo = "Patria",
            Autor = "Fernando Aramburu",
            PrecioTotal = 21.00m,
            PrecioReposicion = 14.50m,
            Stock = 12,
            TipoLibro = "Bolsillo",
            CalificacionMedia = 4.7m,
            FechaLanzamiento = new DateTime(2016, 9, 1),
            Editorial = tusquets,
            Genero = novela
        },
        new Libro {
            Titulo = "Harry Potter y la piedra filosofal",
            Autor = "J.K. Rowling",
            PrecioTotal = 19.99m,
            PrecioReposicion = 10.00m,
            Stock = 20,
            TipoLibro = "Tapa Dura",
            CalificacionMedia = 4.9m,
            FechaLanzamiento = new DateTime(1997, 6, 26),
            Editorial = salamandra,
            Genero
             = fantasia
        }
    };

    // Añadimos todos de golpe y guardamos los cambios en la BD
    dbContext.Libros.AddRange(libros);
    dbContext.SaveChanges();
}


    }
}
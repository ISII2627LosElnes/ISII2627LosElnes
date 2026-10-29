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

            try
            {
                 SeedResenas(dbContext);
            }
                catch (Exception ex)
            {
                 logger.LogError(ex, "An error occurred seeding the Resenas in the Database.");
            }

            try
            {
                SeedResenaItems(dbContext);
            }
            catch (Exception ex)
            {
             logger.LogError(ex, "An error occurred seeding the ResenaItems in the Database.");
            }

            try
            {
                SeedReposiciones(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the Reposiciones in the Database.");
            }

            try
            {
                SeedReposicionItems(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the ReposicionItems in the Database.");
            }

            try
            {
                SeedCompras(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the Compras in the Database.");
            }

            try
            {
                SeedCompraItems(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the CompraItems in the Database.");
            }

            try
            {
                SeedSubastas(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the Subastas in the Database.");
            }

            try
            {
                SeedSubastaItems(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the SubastaItems in the Database.");
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
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es","Calle 123, ALbacete, España");
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
                ApplicationUser user = new ApplicationUser("3", "Peter", "Jackson", "peter@uclm.es", "Calle 456, Ciudad Real, España");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //customer role
                    userManager.AddToRoleAsync(user, roles[2]).Wait();

                }
            }

                if (userManager.FindByNameAsync("samuel@test.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser("4", "Samuel", "Martínez", "samuel@test.com","Calle Hellín, Albacete, España");
                user.EmailConfirmed = true;
                user.PhoneNumber = "600000001";

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully)
                {
                    userManager.AddToRoleAsync(user, roles[2]).Wait();
                }
            }

            if (userManager.FindByNameAsync("manuel@test.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser("5", "Manuel", "González", "manuel@test.com","Calle Talavera de la Reina, Toledo, España");
                user.EmailConfirmed = true;
                user.PhoneNumber = "600000002";

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully)
                {
                    userManager.AddToRoleAsync(user, roles[2]).Wait();
                }
            }

            if (userManager.FindByNameAsync("ismael@test.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser("6", "Ismael", "Isvoranu", "ismael@test.com","Calle Calea Victoriei, Bucarest ,Rumanía");
                user.EmailConfirmed = true;
                user.PhoneNumber = "600000003";

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully)
                {
                    userManager.AddToRoleAsync(user, roles[2]).Wait();
                }
            }

            if (userManager.FindByNameAsync("gabriel@test.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser("7", "Gabriel", "Esparcia", "gabriel@test.com","Calle Isso, Isso ,España");
                user.EmailConfirmed = true;
                user.PhoneNumber = "600000004";

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully)
                {
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
            PrecioCompra = 20.50m,
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
            PrecioCompra = 22.00m,
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
            PrecioCompra = 24.00m,
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
            PrecioCompra = 21.00m,
            PrecioReposicion = 14.50m,
            Stock = 0,
            TipoLibro = "Bolsillo",
            CalificacionMedia = 4.7m,
            FechaLanzamiento = new DateTime(2016, 9, 1),
            Editorial = tusquets,
            Genero = novela
        },
        new Libro {
            Titulo = "Harry Potter y la piedra filosofal",
            Autor = "J.K. Rowling",
            PrecioCompra = 19.99m,
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
public static void SeedResenas(ApplicationDbContext dbContext)
{
    var samuel = dbContext.Users.FirstOrDefault(u => u.Email == "samuel@test.com");
    var manuel = dbContext.Users.FirstOrDefault(u => u.Email == "manuel@test.com");
    var ismael = dbContext.Users.FirstOrDefault(u => u.Email == "ismael@test.com");
    var gabriel = dbContext.Users.FirstOrDefault(u => u.Email == "gabriel@test.com");

    if (samuel == null || manuel == null || ismael == null || gabriel == null)
    {
        return;
    }

    if (!dbContext.Resenas.Any(r => r.Titulo == "Reseña excelente"))
    {
        dbContext.Resenas.Add(new Resena
        {
            FechaResena = new DateTime(2026, 10, 10),
            Titulo = "Reseña excelente",
            Usuario = samuel
        });
    }

    if (!dbContext.Resenas.Any(r => r.Titulo == "Muy recomendable"))
    {
        dbContext.Resenas.Add(new Resena
        {
            FechaResena = new DateTime(2026, 10, 11),
            Titulo = "Muy recomendable",
            Usuario = manuel
        });
    }

    if (!dbContext.Resenas.Any(r => r.Titulo == "Lectura interesante"))
    {
        dbContext.Resenas.Add(new Resena
        {
            FechaResena = new DateTime(2026, 10, 12),
            Titulo = "Lectura interesante",
            Usuario = ismael
        });
    }

    if (!dbContext.Resenas.Any(r => r.Titulo == "Una gran lectura"))
    {
        dbContext.Resenas.Add(new Resena
        {
            FechaResena = new DateTime(2026, 10, 13),
            Titulo = "Una gran lectura",
            Usuario = gabriel
        });
    }

    if (!dbContext.Resenas.Any(r => r.Titulo == "Opinión del libro"))
    {
        dbContext.Resenas.Add(new Resena
        {
            FechaResena = new DateTime(2026, 10, 14),
            Titulo = "Opinión del libro",
            Usuario = manuel
        });
    }

    dbContext.SaveChanges();
}

public static void SeedResenaItems(ApplicationDbContext dbContext)
{
    var sombraDelViento = dbContext.Libros.FirstOrDefault(l => l.Titulo == "La sombra del viento");
    var juegoDelAngel = dbContext.Libros.FirstOrDefault(l => l.Titulo == "El juego del ángel");
    var detectivesSalvajes = dbContext.Libros.FirstOrDefault(l => l.Titulo == "Los detectives salvajes");
    var harryPotter = dbContext.Libros.FirstOrDefault(l => l.Titulo == "Harry Potter y la piedra filosofal");

    var resenaExcelente = dbContext.Resenas.FirstOrDefault(r => r.Titulo == "Reseña excelente");
    var muyRecomendable = dbContext.Resenas.FirstOrDefault(r => r.Titulo == "Muy recomendable");
    var lecturaInteresante = dbContext.Resenas.FirstOrDefault(r => r.Titulo == "Lectura interesante");
    var granLectura = dbContext.Resenas.FirstOrDefault(r => r.Titulo == "Una gran lectura");
    var opinionLibro = dbContext.Resenas.FirstOrDefault(r => r.Titulo == "Opinión del libro");

    if (sombraDelViento == null || juegoDelAngel == null || detectivesSalvajes == null || harryPotter == null ||
        resenaExcelente == null || muyRecomendable == null || lecturaInteresante == null ||
        granLectura == null || opinionLibro == null)
    {
        return;
    }

    if (!dbContext.ResenaItems.Any(ri => ri.LibroId == sombraDelViento.Id && ri.ResenaId == resenaExcelente.Id))
    {
        dbContext.ResenaItems.Add(new ResenaItem(sombraDelViento, "Una historia muy entretenida y bien escrita.", 5, resenaExcelente));
    }

    if (!dbContext.ResenaItems.Any(ri => ri.LibroId == harryPotter.Id && ri.ResenaId == muyRecomendable.Id))
    {
        dbContext.ResenaItems.Add(new ResenaItem(harryPotter, "Me ha gustado mucho el desarrollo de la historia.", 5, muyRecomendable));
    }

    if (!dbContext.ResenaItems.Any(ri => ri.LibroId == juegoDelAngel.Id && ri.ResenaId == lecturaInteresante.Id))
    {
        dbContext.ResenaItems.Add(new ResenaItem(juegoDelAngel, "El ritmo es bueno y mantiene el interés hasta el final.", 4, lecturaInteresante));
    }

    if (!dbContext.ResenaItems.Any(ri => ri.LibroId == detectivesSalvajes.Id && ri.ResenaId == granLectura.Id))
    {
        dbContext.ResenaItems.Add(new ResenaItem(detectivesSalvajes, "Una lectura sencilla pero muy disfrutable y recomendable.", 4, granLectura));
    }

    if (!dbContext.ResenaItems.Any(ri => ri.LibroId == sombraDelViento.Id && ri.ResenaId == opinionLibro.Id))
    {
        dbContext.ResenaItems.Add(new ResenaItem(sombraDelViento, null, 3, opinionLibro));
    }

    dbContext.SaveChanges();
}

public static void SeedReposiciones(ApplicationDbContext dbContext)
{
    var samuel = dbContext.Users.FirstOrDefault(u => u.Email == "samuel@test.com");
    var manuel = dbContext.Users.FirstOrDefault(u => u.Email == "manuel@test.com");
    var ismael = dbContext.Users.FirstOrDefault(u => u.Email == "ismael@test.com");
    var gabriel = dbContext.Users.FirstOrDefault(u => u.Email == "gabriel@test.com");

    if (samuel == null || manuel == null || ismael == null || gabriel == null)
    {
        return;
    }

    var visaSamuel = new Visa(0, "1234567890123456", new DateTime(2028, 12, 31));
    var paypalManuel = new PayPal(0, "600000002");
    var googlePayIsmael = new GooglePay(0, "ismael@test.com");
    var visaGabriel = new Visa(0, "9876543210987654", new DateTime(2027, 10, 31));
    var paypalSamuel2 = new PayPal(0, "600000001");
    
    
    if (!dbContext.Reposiciones.Any(r => r.FechaReposicion == new DateTime(2026, 10, 15)))
    {
        dbContext.Reposiciones.Add(new Reposicion {
            FechaReposicion = new DateTime(2026, 10, 15),
            PrecioTotal = 31.50,
            Comentario = "Reposición inicial de stock de Zafón",
            Usuario = samuel,
            MetodoPago = visaSamuel
        });
    }

    
    if (!dbContext.Reposiciones.Any(r => r.FechaReposicion == new DateTime(2026, 10, 16)))
    {
        dbContext.Reposiciones.Add(new Reposicion {
            FechaReposicion = new DateTime(2026, 10, 16),
            PrecioTotal = 36.00,
            Comentario = "Urgente: Reposición de libros de ficción",
            Usuario = manuel,
            MetodoPago = paypalManuel
        });
    }

    
    if (!dbContext.Reposiciones.Any(r => r.FechaReposicion == new DateTime(2026, 10, 17)))
    {
        dbContext.Reposiciones.Add(new Reposicion {
            FechaReposicion = new DateTime(2026, 10, 17),
            PrecioTotal = 29.00,
            Comentario = "Reposición de novela histórica",
            Usuario = ismael,
            MetodoPago = googlePayIsmael
        });
    }

    
    if (!dbContext.Reposiciones.Any(r => r.FechaReposicion == new DateTime(2026, 10, 18)))
    {
        dbContext.Reposiciones.Add(new Reposicion {
            FechaReposicion = new DateTime(2026, 10, 18),
            PrecioTotal = 45.00,
            Comentario = "Reposición de fantasía y misterio",
            Usuario = gabriel,
            MetodoPago = visaGabriel
        });
    }

    
    if (!dbContext.Reposiciones.Any(r => r.FechaReposicion == new DateTime(2026, 10, 19)))
    {
        dbContext.Reposiciones.Add(new Reposicion {
            FechaReposicion = new DateTime(2026, 10, 19),
            PrecioTotal = 49.00,
            Comentario = "Reposición variada semanal",
            Usuario = samuel,
            MetodoPago = paypalSamuel2
        });
    }

    dbContext.SaveChanges();
}

public static void SeedReposicionItems(ApplicationDbContext dbContext)
{
    // Buscamos todos los libros que vamos a reponer
    var sombraDelViento = dbContext.Libros.FirstOrDefault(l => l.Titulo == "La sombra del viento");
    var juegoDelAngel = dbContext.Libros.FirstOrDefault(l => l.Titulo == "El juego del ángel");
    var detectivesSalvajes = dbContext.Libros.FirstOrDefault(l => l.Titulo == "Los detectives salvajes");
    var patria = dbContext.Libros.FirstOrDefault(l => l.Titulo == "Patria");
    var harryPotter = dbContext.Libros.FirstOrDefault(l => l.Titulo == "Harry Potter y la piedra filosofal");

    // Buscamos las reposiciones base que acabamos de crear
    var rep1 = dbContext.Reposiciones.FirstOrDefault(r => r.Id == 1);
    var rep2 = dbContext.Reposiciones.FirstOrDefault(r => r.Id == 2);
    var rep3 = dbContext.Reposiciones.FirstOrDefault(r => r.Id == 3);
    var rep4 = dbContext.Reposiciones.FirstOrDefault(r => r.Id == 4);
    var rep5 = dbContext.Reposiciones.FirstOrDefault(r => r.Id == 5);

    if (sombraDelViento == null || juegoDelAngel == null || detectivesSalvajes == null || 
        patria == null || harryPotter == null || 
        rep1 == null || rep2 == null || rep3 == null || rep4 == null || rep5 == null)
    {
        return;
    }

    // Items para la Reposición 1
    if (!dbContext.ReposicionItems.Any(ri => ri.LibroId == sombraDelViento.Id && ri.ReposicionId == rep1.Id))
        dbContext.ReposicionItems.Add(new ReposicionItem(1, sombraDelViento, rep1));
    
    if (!dbContext.ReposicionItems.Any(ri => ri.LibroId == juegoDelAngel.Id && ri.ReposicionId == rep1.Id))
        dbContext.ReposicionItems.Add(new ReposicionItem(1, juegoDelAngel, rep1));

    // Items para la Reposición 2
    if (!dbContext.ReposicionItems.Any(ri => ri.LibroId == detectivesSalvajes.Id && ri.ReposicionId == rep2.Id))
        dbContext.ReposicionItems.Add(new ReposicionItem(2, detectivesSalvajes, rep2));

    // Items para la Reposición 3
    if (!dbContext.ReposicionItems.Any(ri => ri.LibroId == patria.Id && ri.ReposicionId == rep3.Id))
        dbContext.ReposicionItems.Add(new ReposicionItem(2, patria, rep3));

    // Items para la Reposición 4
    if (!dbContext.ReposicionItems.Any(ri => ri.LibroId == harryPotter.Id && ri.ReposicionId == rep4.Id))
        dbContext.ReposicionItems.Add(new ReposicionItem(3, harryPotter, rep4));
        
    if (!dbContext.ReposicionItems.Any(ri => ri.LibroId == sombraDelViento.Id && ri.ReposicionId == rep4.Id))
        dbContext.ReposicionItems.Add(new ReposicionItem(1, sombraDelViento, rep4));

    // Items para la Reposición 5
    if (!dbContext.ReposicionItems.Any(ri => ri.LibroId == juegoDelAngel.Id && ri.ReposicionId == rep5.Id))
        dbContext.ReposicionItems.Add(new ReposicionItem(1, juegoDelAngel, rep5));
        
    if (!dbContext.ReposicionItems.Any(ri => ri.LibroId == detectivesSalvajes.Id && ri.ReposicionId == rep5.Id))
        dbContext.ReposicionItems.Add(new ReposicionItem(1, detectivesSalvajes, rep5));
        
    if (!dbContext.ReposicionItems.Any(ri => ri.LibroId == patria.Id && ri.ReposicionId == rep5.Id))
        dbContext.ReposicionItems.Add(new ReposicionItem(1, patria, rep5));

    dbContext.SaveChanges();
}

public static void SeedCompras(ApplicationDbContext dbContext)
{
    var samuel = dbContext.Users.FirstOrDefault(u => u.Email == "samuel@test.com");
    var manuel = dbContext.Users.FirstOrDefault(u => u.Email == "manuel@test.com");
    var ismael = dbContext.Users.FirstOrDefault(u => u.Email == "ismael@test.com");
    var gabriel = dbContext.Users.FirstOrDefault(u => u.Email == "gabriel@test.com");

    var visaSamuel = new Visa(0, "1234567890123456", new DateTime(2028, 12, 31));
    var paypalManuel = new PayPal(0, "659286702");
    var googlePayIsmael = new GooglePay(0, "ismael@test.com");
    var visaGabriel = new Visa(0, "9876543210987654", new DateTime(2027, 10, 31));  

    if (samuel == null || manuel == null || ismael == null || gabriel == null)
    {
        return;
    }

    if (!dbContext.Compras.Any(c => c.CodigoDescuento == "DESC10"))
    {
        dbContext.Compras.Add(new Compra
        {
            FechaCompra = new DateTime(2026, 10, 15),
            PrecioTotal = 45.50m,
            CodigoDescuento = "DESC10",
            Usuario = samuel,
            MetodoPago = visaSamuel
        });
    }

    if (!dbContext.Compras.Any(c => c.PrecioTotal == 30.00m && c.CodigoDescuento == null))
    {
        dbContext.Compras.Add(new Compra
        {
            FechaCompra = new DateTime(2026, 10, 16),
            PrecioTotal = 30.00m,
            CodigoDescuento = null,
            Usuario = manuel,
            MetodoPago = paypalManuel
        });
    }

    if (!dbContext.Compras.Any(c => c.PrecioTotal == 25.00m && c.CodigoDescuento == "DESC10"))
    {
        dbContext.Compras.Add(new Compra
        {
            FechaCompra = new DateTime(2026, 10, 17),
            PrecioTotal = 25.00m,
            CodigoDescuento = "DESC10",
            Usuario = ismael,
            MetodoPago = googlePayIsmael
        });
    }

    if (!dbContext.Compras.Any(c => c.PrecioTotal == 40.00m && c.CodigoDescuento == null))
    {
        dbContext.Compras.Add(new Compra
        {
            FechaCompra = new DateTime(2026, 10, 18),
            PrecioTotal = 40.00m,
            CodigoDescuento = null,
            Usuario = gabriel,
            MetodoPago = visaGabriel
        });
    }

    dbContext.SaveChanges();
}

public static void SeedCompraItems(ApplicationDbContext dbContext)
{
    var sombraDelViento = dbContext.Libros.FirstOrDefault(l => l.Titulo == "La sombra del viento");
    var juegoDelAngel = dbContext.Libros.FirstOrDefault(l => l.Titulo == "El juego del ángel");
    var harryPotter = dbContext.Libros.FirstOrDefault(l => l.Titulo == "Harry Potter y la piedra filosofal");
    var detectivesSalvajes = dbContext.Libros.FirstOrDefault(l => l.Titulo == "Los detectives salvajes");

    var compraConDescuento = dbContext.Compras.FirstOrDefault(c => c.CodigoDescuento == "DESC10");
    var compraSinDescuento = dbContext.Compras.FirstOrDefault(c => c.PrecioTotal == 30.00m && c.CodigoDescuento == null);

    if (sombraDelViento == null || juegoDelAngel == null || harryPotter == null ||
        compraConDescuento == null || compraSinDescuento == null)
    {
        return;
    }

    if (!dbContext.CompraItems.Any(ci => ci.LibroId == sombraDelViento.Id && ci.CompraId == compraConDescuento.Id))
    {
        dbContext.CompraItems.Add(new CompraItem
        {
            Cantidad = 2,
            LibroId = sombraDelViento.Id,
            CompraId = compraConDescuento.Id
        });
    }

    if (!dbContext.CompraItems.Any(ci => ci.LibroId == juegoDelAngel.Id && ci.CompraId == compraConDescuento.Id))
    {
        dbContext.CompraItems.Add(new CompraItem
        {
            Cantidad = 3,
            LibroId = juegoDelAngel.Id,
            CompraId = compraConDescuento.Id
        });
    }

    if (!dbContext.CompraItems.Any(ci => ci.LibroId == harryPotter.Id && ci.CompraId == compraSinDescuento.Id))
    {
        dbContext.CompraItems.Add(new CompraItem
        {
            Cantidad = 2,
            LibroId = harryPotter.Id,
            CompraId = compraSinDescuento.Id
        });
    }

    if (!dbContext.CompraItems.Any(ci => ci.LibroId == detectivesSalvajes.Id && ci.CompraId == compraSinDescuento.Id))
    {
        dbContext.CompraItems.Add(new CompraItem
        {
            Cantidad = 4,
            LibroId = detectivesSalvajes.Id,
            CompraId = compraSinDescuento.Id
        });
    }

    dbContext.SaveChanges();
}


public static void SeedSubastas(ApplicationDbContext dbContext)
{
    var samuel = dbContext.Users.FirstOrDefault(u => u.Email == "samuel@test.com");
    var manuel = dbContext.Users.FirstOrDefault(u => u.Email == "manuel@test.com");
    var ismael = dbContext.Users.FirstOrDefault(u => u.Email == "ismael@test.com");
    var gabriel = dbContext.Users.FirstOrDefault(u => u.Email == "gabriel@test.com");

    if (samuel == null || manuel == null || ismael == null || gabriel == null)
    {
        return;
    }

    var visaSamuel = new Visa(0, "1234567890123456", new DateTime(2028, 12, 31));
    var paypalManuel = new PayPal(0, "659286702");
    var googlePayIsmael = new GooglePay(0, "ismael@test.com");
    var visaGabriel = new Visa(0, "9876543210987654", new DateTime(2027, 10, 31));

    // Subasta 1: Precio 20.8, (2x Harry Potter y la Piedra Filosofal a 10.40)
    if (!dbContext.Subastas.Any(s => s.FechaSubasta == new DateTime(2026, 10, 28) && s.Usuario.Email == samuel.Email))
    {
        dbContext.Subastas.Add(new Subasta {
            PrecioSubasta = 20.80m,
            FechaSubasta = new DateTime(2026, 10, 30),
            Usuario = samuel,
            MetodoPago = visaSamuel
        });
    }

    // Subasta 2: Precio 51.2, (2x Patria a 15.60 + 1x Los detectives salvajes a 20.00)
    if (!dbContext.Subastas.Any(s => s.FechaSubasta == new DateTime(2026, 10, 30) && s.Usuario.Email == gabriel.Email))
    {
        dbContext.Subastas.Add(new Subasta {
            PrecioSubasta = 51.20m,
            FechaSubasta = new DateTime(2026, 11, 13),
            Usuario = gabriel,
            MetodoPago = visaGabriel
        });
    }

    // Subasta 3: Precio 13, (1x Harry Potter y la Piedra Filosofal a 13.00)
    if (!dbContext.Subastas.Any(s => s.FechaSubasta == new DateTime(2026, 11, 15) && s.Usuario.Email == manuel.Email))
    {
        dbContext.Subastas.Add(new Subasta {
            PrecioSubasta = 13.00m,
            FechaSubasta = new DateTime(2026, 10, 27),
            Usuario = manuel,
            MetodoPago = paypalManuel
        });
    }

    // Subasta 4: Precio 60.6, (1x Harry Potter y la Piedra Filosofal a 13.00 + 2x La sombra del viento a 16.00 + 1x Patria a 15.6)
    if (!dbContext.Subastas.Any(s => s.FechaSubasta == new DateTime(2026, 12, 20) && s.Usuario.Email == ismael.Email))
    {
        dbContext.Subastas.Add(new Subasta {
            PrecioSubasta = 60.60m,
            FechaSubasta = new DateTime(2026, 12, 03),
            Usuario = ismael,
            MetodoPago = googlePayIsmael
        });
    }

    dbContext.SaveChanges();
}

public static void SeedSubastaItems(ApplicationDbContext dbContext)
{
    // Buscamos todos los libros que vamos a subastar
    var sombraDelViento = dbContext.Libros.FirstOrDefault(l => l.Titulo == "La sombra del viento");
    var detectivesSalvajes = dbContext.Libros.FirstOrDefault(l => l.Titulo == "Los detectives salvajes");
    var patria = dbContext.Libros.FirstOrDefault(l => l.Titulo == "Patria");
    var harryPotter = dbContext.Libros.FirstOrDefault(l => l.Titulo == "Harry Potter y la piedra filosofal");

    // Buscamos las subastas base que acabamos de crear
    var sub1 = dbContext.Subastas.FirstOrDefault(r => r.Id == 001);
    var sub2 = dbContext.Subastas.FirstOrDefault(r => r.Id == 002);
    var sub3 = dbContext.Subastas.FirstOrDefault(r => r.Id == 003);
    var sub4 = dbContext.Subastas.FirstOrDefault(r => r.Id == 004);

    if (sombraDelViento == null || detectivesSalvajes == null || 
        patria == null || harryPotter == null || 
        sub1 == null || sub2 == null || sub3 == null || sub4 == null)
    {
        return;
    }

    // Items para la subasta 1
    if (!dbContext.SubastaItems.Any(ri => ri.LibroId == harryPotter.Id && ri.SubastaId == sub1.Id))
        dbContext.SubastaItems.Add(new SubastaItem("", 20.80m, harryPotter, sub1));

    // Items para la subasta 2
    if (!dbContext.SubastaItems.Any(ri => ri.LibroId == patria.Id && ri.SubastaId == sub2.Id))
        dbContext.SubastaItems.Add(new SubastaItem("Se han vendido 2 patrias", 31.20m, patria, sub2));

    if (!dbContext.SubastaItems.Any(ri => ri.LibroId == detectivesSalvajes.Id && ri.SubastaId == sub2.Id))
        dbContext.SubastaItems.Add(new SubastaItem("Se ha vendido 1 detective salvaje", 20.00m, detectivesSalvajes, sub2));

    // Items para la subasta 3
    if (!dbContext.SubastaItems.Any(ri => ri.LibroId == harryPotter.Id && ri.SubastaId == sub3.Id))
        dbContext.SubastaItems.Add(new SubastaItem("", 13.00m, harryPotter, sub3));

    // Items para la subasta 4
    if (!dbContext.SubastaItems.Any(ri => ri.LibroId == harryPotter.Id && ri.SubastaId == sub4.Id))
        dbContext.SubastaItems.Add(new SubastaItem("Se ha vendido 1 harry potter", 13.00m, harryPotter, sub4));
        
    if (!dbContext.SubastaItems.Any(ri => ri.LibroId == sombraDelViento.Id && ri.SubastaId == sub4.Id))
        dbContext.SubastaItems.Add(new SubastaItem("Se han vendido 2 sombras del viento", 32.00m, sombraDelViento, sub4));

    if (!dbContext.SubastaItems.Any(ri => ri.LibroId == patria.Id && ri.SubastaId == sub4.Id))
        dbContext.SubastaItems.Add(new SubastaItem("", 15.60m, patria, sub4));


    dbContext.SaveChanges();
}

    }

    
}
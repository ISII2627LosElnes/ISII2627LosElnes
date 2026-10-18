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
    if (!dbContext.Resenas.Any(r => r.Titulo == "Reseña excelente"))
    {
        dbContext.Resenas.Add(new Resena
        {
            FechaResena = new DateTime(2026, 10, 10),
            Titulo = "Reseña excelente"
        });
    }

    if (!dbContext.Resenas.Any(r => r.Titulo == "Muy recomendable"))
    {
        dbContext.Resenas.Add(new Resena
        {
            FechaResena = new DateTime(2026, 10, 11),
            Titulo = "Muy recomendable"
        });
    }

    if (!dbContext.Resenas.Any(r => r.Titulo == "Lectura interesante"))
    {
        dbContext.Resenas.Add(new Resena
        {
            FechaResena = new DateTime(2026, 10, 12),
            Titulo = "Lectura interesante"
        });
    }

    if (!dbContext.Resenas.Any(r => r.Titulo == "Una gran lectura"))
    {
        dbContext.Resenas.Add(new Resena
        {
            FechaResena = new DateTime(2026, 10, 13),
            Titulo = "Una gran lectura"
        });
    }

    if (!dbContext.Resenas.Any(r => r.Titulo == "Opinión del libro"))
    {
        dbContext.Resenas.Add(new Resena
        {
            FechaResena = new DateTime(2026, 10, 14),
            Titulo = "Opinión del libro"
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
    // Reposición 1: Coste total 31.50 (1x La sombra del viento a 15.00 + 1x El juego del ángel a 16.50)
    if (!dbContext.Reposiciones.Any(r => r.Id == "REP-001"))
    {
        dbContext.Reposiciones.Add(new Reposicion {
            Id = "REP-001",
            FechaReposicion = new DateTime(2026, 10, 15),
            PrecioTotal = 31.50,
            Comentario = "Reposición inicial de stock de Zafón"
        });
    }

    // Reposición 2: Coste total 36.00 (2x Los detectives salvajes a 18.00)
    if (!dbContext.Reposiciones.Any(r => r.Id == "REP-002"))
    {
        dbContext.Reposiciones.Add(new Reposicion {
            Id = "REP-002",
            FechaReposicion = new DateTime(2026, 10, 16),
            PrecioTotal = 36.00,
            Comentario = "Urgente: Reposición de libros de ficción"
        });
    }

    // Reposición 3: Coste total 29.00 (2x Patria a 14.50)
    if (!dbContext.Reposiciones.Any(r => r.Id == "REP-003"))
    {
        dbContext.Reposiciones.Add(new Reposicion {
            Id = "REP-003",
            FechaReposicion = new DateTime(2026, 10, 17),
            PrecioTotal = 29.00,
            Comentario = "Reposición de novela histórica"
        });
    }

    // Reposición 4: Coste total 45.00 (3x Harry Potter a 10.00 + 1x La sombra del viento a 15.00)
    if (!dbContext.Reposiciones.Any(r => r.Id == "REP-004"))
    {
        dbContext.Reposiciones.Add(new Reposicion {
            Id = "REP-004",
            FechaReposicion = new DateTime(2026, 10, 18),
            PrecioTotal = 45.00,
            Comentario = "Reposición de fantasía y misterio"
        });
    }

    // Reposición 5: Coste total 49.00 (1x El juego del ángel a 16.50 + 1x Los detectives a 18.00 + 1x Patria a 14.50)
    if (!dbContext.Reposiciones.Any(r => r.Id == "REP-005"))
    {
        dbContext.Reposiciones.Add(new Reposicion {
            Id = "REP-005",
            FechaReposicion = new DateTime(2026, 10, 19),
            PrecioTotal = 49.00,
            Comentario = "Reposición variada semanal"
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
    var rep1 = dbContext.Reposiciones.FirstOrDefault(r => r.Id == "REP-001");
    var rep2 = dbContext.Reposiciones.FirstOrDefault(r => r.Id == "REP-002");
    var rep3 = dbContext.Reposiciones.FirstOrDefault(r => r.Id == "REP-003");
    var rep4 = dbContext.Reposiciones.FirstOrDefault(r => r.Id == "REP-004");
    var rep5 = dbContext.Reposiciones.FirstOrDefault(r => r.Id == "REP-005");

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


    }

    
}
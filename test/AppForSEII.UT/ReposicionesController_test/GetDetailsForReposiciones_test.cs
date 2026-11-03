namespace AppForSEII.UT.ReposicionesController_test;

using AppForSEII.API.Controllers;
using AppForSEII.API.ReposicionDTOs;
using AppForSEII.API.Models;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

public class GetDetailsForReposiciones_test : AppForSEII4SqliteUT
{
    public GetDetailsForReposiciones_test()
    {
        var editoriales = new List<Editorial>
        {
            new Editorial(10, "Minotauro"),
            new Editorial(11, "Planeta")
        };

        var generos = new List<Genero>
        {
            new Genero(10, "Fantasía"),
            new Genero(11, "Misterio")
        };

        var libros = new List<Libro>
        {
            new Libro(10, "El Señor de los Anillos", "Edición Tapa Dura", "J.R.R. Tolkien", 20m, 15m, 0, 4.9m, new DateTime(1954, 7, 29), generos[0], editoriales[0]),
            new Libro(11, "La Sombra del Viento", "Edición de Bolsillo", "Carlos Ruiz Zafón", 18m, 12.5m, 0, 4.8m, new DateTime(2001, 4, 1), generos[1], editoriales[1])
        };

        var usuarios = new List<ApplicationUser>
        {
            new ApplicationUser("1", "Héctor", "Fernández", "hector@uclm.es", "Avda. España s/n")
        };
        // CORRECCIÓN 1: Asignamos el teléfono al usuario para que no sea null en la base de datos
        usuarios[0].PhoneNumber = "600777666";

        var metodoPago = new PayPal(1, "600777666");

        var reposiciones = new List<Reposicion>
        {
            new Reposicion(1, new DateTime(2026, 10, 20), 20, "Muy buen libro para leer tranquilo", new List<ReposicionItem>(), metodoPago, usuarios[0])
        };

        var reposicionItems = new List<ReposicionItem>
        {
            new ReposicionItem(2, libros[0], reposiciones[0]),
            new ReposicionItem(1, libros[1], reposiciones[0]) // Aquí la cantidad es 1
        };

        reposiciones[0].ReposicionItems.Add(reposicionItems[0]);
        reposiciones[0].ReposicionItems.Add(reposicionItems[1]);

        _context.AddRange(usuarios);
        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.Add(metodoPago);
        _context.AddRange(reposiciones);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetDetailsForReposiciones_NotFound_test()
    {
        //arrange
        var mock = new Mock<ILogger<ReposicionesController>>();
        ILogger<ReposicionesController> logger = mock.Object;
        var controller = new ReposicionesController(_context, logger);

        //act
        var result = await controller.GetDetailsForReposiciones(100); //id que no existe

        //assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetDetailsForReposiciones_Found_test()
    {
        //arrange
        var mock = new Mock<ILogger<ReposicionesController>>();
        ILogger<ReposicionesController> logger = mock.Object;
        var controller = new ReposicionesController(_context, logger);

        var resenaEsperada = new ReposicionDetailDTO("Héctor", "Fernández", "600777666", new DateTime(2026, 10, 20), 20, new List<ReposicionItemDTO>(), "PayPal");

        resenaEsperada.ReposicionItems.Add(new ReposicionItemDTO("El Señor de los Anillos", "J.R.R. Tolkien", 2));
        // CORRECCIÓN 2: Ponemos cantidad 1 (antes tenías 3) para que coincida con el constructor
        resenaEsperada.ReposicionItems.Add(new ReposicionItemDTO("La Sombra del Viento", "Carlos Ruiz Zafón", 1));

        //act
        var result = await controller.GetDetailsForReposiciones(1);

        //assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var reposicionDTOActual = Assert.IsType<ReposicionDetailDTO>(okResult.Value);
        Assert.Equal(resenaEsperada, reposicionDTOActual);
    }
}
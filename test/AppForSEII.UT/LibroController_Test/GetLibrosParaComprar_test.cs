namespace AppForSEII.UT.LibroController_Test;

using AppForSEII.API.Controllers;
using AppForSEII.API.LibroDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GetLibrosParaComprar_test : AppForSEII4SqliteUT
{
    // Constructor con los datos necesarios para la prueba del select (Arrange)
    public GetLibrosParaComprar_test()
    {
        var generos = new List<Genero>
        {
            new Genero(1, "Ficción"),
            new Genero(2, "Infantil"),
            new Genero(3, "Fantasía")
        };

        var editoriales = new List<Editorial>
        {
            new Editorial(1, "Ediciones Martínez Roca"),
            new Editorial(2, "Anaya"),
            new Editorial(3, "Salamandra")
        };

        var libros = new List<Libro>
        {
            // Libro 1: PrecioCompra 19.50, Stock 200 (>0)
            new Libro(1, "Harry Potter y la piedra filosofal", "Tapa Blanda", "J.K. Rowling", 19.50m, 12m, 200, 4.9m, new DateTime(1997, 6, 26), generos[0], editoriales[0]),
            
            // Libro 2: PrecioCompra 14.50, Stock 120 (>0)
            new Libro(2, "Wigetta: Un viaje mágico", "Tapa Blanda", "Vegetta777 y Willyrex", 14.50m, 9m, 120, 4.7m, new DateTime(2015, 3, 26), generos[1], editoriales[1]),
            
            // Libro 3: PrecioCompra 18.00, Stock 80 (>0)
            new Libro(3, "Los secretos de YouTube", "Tapa Dura", "TheGrefg", 18.00m, 12m, 80, 4.9m, new DateTime(2018, 11, 20), generos[0], editoriales[2]),
            
            // Libro 4: Stock = 0 (Debe ser excluido por la cláusula Where stock > 0)
            new Libro(4, "El Principito", "Tapa Blanda", "Antoine de Saint-Exupéry", 10.00m, 5m, 0, 4.9m, new DateTime(1943, 4, 6), generos[2], editoriales[0])
        };

        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.SaveChanges();
    }

    // MemberData para identificar las fuentes de datos de la prueba (Data-Driven)
    public static IEnumerable<object[]> TestCasesFor_GetLibrosParaComprar_OK()
    {
        var todosLosLibros = new List<LibroParaComprarDTO>
        {
            new LibroParaComprarDTO(1, "Harry Potter y la piedra filosofal", 19.50m, new DateTime(1997, 6, 26), "Ficción", "Ediciones Martínez Roca"),
            new LibroParaComprarDTO(2, "Wigetta: Un viaje mágico", 14.50m, new DateTime(2015, 3, 26), "Infantil", "Anaya"),
            new LibroParaComprarDTO(3, "Los secretos de YouTube", 18.00m, new DateTime(2018, 11, 20), "Ficción", "Salamandra")
        };

        // CASO 1: Sin filtros (null, null) -> Devuelve todos los que tienen stock > 0 (3 libros)
        var librosSinFiltros = new List<LibroParaComprarDTO> 
        { 
            todosLosLibros[0], 
            todosLosLibros[1], 
            todosLosLibros[2] 
        };

        // CASO 2: Filtrando por precio de compra ("18.00") -> Devuelve los libros con precio <= 18.00 (Wigetta y Los secretos de YouTube)
        var librosPrecioCompra = new List<LibroParaComprarDTO> 
        { 
            todosLosLibros[1], // Wigetta (14.50m)
            todosLosLibros[2]  // Los secretos de YouTube (18.00m)
        };

        // CASO 3: Filtrando solo por género ("Infantil") -> Solo Wigetta
        var librosGenero = new List<LibroParaComprarDTO> 
        { 
            todosLosLibros[1] 
        };

        // CASO 4: Filtrando por género ("Ficción") y precio (18.00m) a la vez -> Solo Los secretos de YouTube
        var librosAmbos = new List<LibroParaComprarDTO> 
        { 
            todosLosLibros[2] 
        };

        var allTests = new List<object[]>
        {
            new object[] { null, null, librosSinFiltros },
            new object[] { null, 18.00m, librosPrecioCompra },
            new object[] { "Infantil", null, librosGenero },
            new object[] { "Ficción", 18.00m, librosAmbos }
        };

        return allTests;
    }

    [Theory]
    [MemberData(nameof(TestCasesFor_GetLibrosParaComprar_OK))]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetLibrosParaComprar_OK_test(string? filtroGenero, decimal? filtroPrecioCompra, IList<LibroParaComprarDTO> expectedLibros)
    {
        // Arrange
        var mock = new Mock<ILogger<LibroController>>();
        ILogger<LibroController> logger = mock.Object;
        var controller = new LibroController(_context, logger);

        // Act
        var result = await controller.GetLibrosParaComprar(filtroGenero, filtroPrecioCompra);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var libroDTOsActual = Assert.IsType<List<LibroParaComprarDTO>>(okResult.Value);
        
        // Comprobación segura elemento a elemento para evitar fallos de referencia
        Assert.Equal(expectedLibros.Count, libroDTOsActual.Count);
        for (int i = 0; i < expectedLibros.Count; i++)
        {
            Assert.Equal(expectedLibros[i].Id, libroDTOsActual[i].Id);
            Assert.Equal(expectedLibros[i].Titulo, libroDTOsActual[i].Titulo);
            Assert.Equal(expectedLibros[i].PrecioCompra, libroDTOsActual[i].PrecioCompra);
        }
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    public async Task GetLibrosParaComprar_badrequest_test()
    {
        // Arrange
        var mock = new Mock<ILogger<LibroController>>();
        ILogger<LibroController> logger = mock.Object;
        var controller = new LibroController(_context, logger);

        // Forzamos un error en el ModelState para probar el control de errores
        controller.ModelState.AddModelError("Error", "Parámetros de entrada no válidos al buscar libros para comprar.");

        // Act
        var result = await controller.GetLibrosParaComprar(null, null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }
}
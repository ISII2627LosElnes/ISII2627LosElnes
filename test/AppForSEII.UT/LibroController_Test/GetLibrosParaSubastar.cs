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

public class GetLibrosParaSubastar_test : AppForSEII4SqliteUT
{
    // Constructor con los datos necesarios para la prueba del select (Arrange)
    public GetLibrosParaSubastar_test()
    {
        var generos = new List<Genero>
        {
            new Genero(1, "Ficción"),
            new Genero(2, "Novela"),
            new Genero(3, "Misterio")
        };

        var editoriales = new List<Editorial>
        {
            new Editorial(1, "Vicens Vives"),
            new Editorial(2, "Anaya"),
            new Editorial(3, "Planeta")
        };

        var libros = new List<Libro>
        {
            // Libro 1: PrecioCompra 11.50, Stock 140 (>0)
            new Libro(1, "Novelas ejemplares", "Tapa Dura", "Cervantes", 11.50m, 7m, 140, 4.9m, new DateTime(2013, 10, 15  ), generos[1], editoriales[0]),
            
            // Libro 2: PrecioCompra 13.50, Stock 100 (>0)
            new Libro(2, "Vinland saga", "Tapa Blanda", "Yukimura", 13.50m, 8m, 100, 4.7m, new DateTime(2006, 8, 26), generos[0], editoriales[2]),
            
            // Libro 3: PrecioCompra 20.00, Stock 35 (>0)
            new Libro(3, "Sueños de acero y neon", "Tapa Dura", "Jordi", 20.00m, 12m, 35, 4.5m, new DateTime(2016, 2, 23), generos[0], editoriales[2]),
            
            // Libro 4: stock = 0 (Debe ser excluido por la cláusula Where stock > 0)
            new Libro(4, "Luna de plutón", "Tapa Blanda", "Angel David", 15.00m, 10m, 0, 4.4m, new DateTime(2015, 11, 12), generos[2], editoriales[1])
        };

        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.SaveChanges();
    }

    // MemberData para identificar las fuentes de datos de la prueba (Data-Driven)
    public static IEnumerable<object[]> TestCasesFor_GetLibrosParaSubastar_OK()
    {
        var todosLosLibros = new List<LibroParaSubastarDTO>
        {
            new LibroParaSubastarDTO(1, "Novelas ejemplares", "Vicens Vives", "Cervantes", "Tapa Dura"),
            new LibroParaSubastarDTO(2, "Vinland saga", "Planeta", "Yukimura", "Tapa Blanda"),
            new LibroParaSubastarDTO(3, "Sueños de acero y neon", "Planeta", "Jordi", "Tapa Dura")
        };

        // CASO 1: Sin filtros (null, null) -> Devuelve todos los que tienen stock > 0 (3 libros)
        var librosSinFiltros = new List<LibroParaSubastarDTO> 
        { 
            todosLosLibros[0], 
            todosLosLibros[1], 
            todosLosLibros[2] 
        };

        // CASO 2: Filtrando por título ("Novelas ejemplares") -> Devuelve los libros con el nombre igual a "Novelas ejemplares"
        var librosTitulo = new List<LibroParaSubastarDTO> 
        { 
            todosLosLibros[0] // Novelas ejemplares
        };

        // CASO 3: Filtrando solo por editorial ("Vicens Vives") -> Solo Novelas ejemplares
        var librosEditorial = new List<LibroParaSubastarDTO> 
        { 
            todosLosLibros[0] // Novelas ejemplares
        };

        // CASO 4: Filtrando por editorial ("Planeta") y título ("Vinland saga") a la vez
        var librosAmbos = new List<LibroParaSubastarDTO> 
        { 
            todosLosLibros[1] // Vinland saga
        };

        var allTests = new List<object[]>
        {
            new object[] { null, null, librosSinFiltros },
            new object[] { null, "Vicens Vives", librosTitulo },
            new object[] { "Novelas ejemplares", null, librosEditorial },
            new object[] { "Vinland saga", "Planeta", librosAmbos }
        };

        return allTests;
    }

    [Theory]
    [MemberData(nameof(TestCasesFor_GetLibrosParaSubastar_OK))]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetLibrosParaSubastar_OK_test(string? filtroEditorial, string? filtroTitulo, IList<LibroParaSubastarDTO> expectedLibros)
    {
        // Arrange
        var mock = new Mock<ILogger<LibroController>>();
        ILogger<LibroController> logger = mock.Object;
        var controller = new LibroController(_context, logger);

        // Act
        var result = await controller.GetLibrosParaSubastar(filtroEditorial, filtroTitulo);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var libroDTOsActual = Assert.IsType<List<LibroParaSubastarDTO>>(okResult.Value);
        
        // Comprobación de elementos iguales
        Assert.Equal(expectedLibros, libroDTOsActual);
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    public async Task GetLibrosParaSubastar_badrequest_test()
    {
        // Arrange
        var mock = new Mock<ILogger<LibroController>>();
        ILogger<LibroController> logger = mock.Object;
        var controller = new LibroController(_context, logger);

        // Forzamos un error en el ModelState para probar el control de errores
        controller.ModelState.AddModelError("Error", "Parámetros de entrada no válidos al buscar libros para subastar.");

        // Act
        var result = await controller.GetLibrosParaSubastar(null, null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }
}
namespace AppForSEII.UT.ComprasController_test;

using AppForSEII.API.Controllers;
using AppForSEII.API.CompraDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GetDetailsForCompras_test : AppForSEII4SqliteUT
{
    public GetDetailsForCompras_test()
    {
        // ARRANGE: Datos inventados diferentes para la Base de Datos en Memoria (Sqlite)
        
        var editoriales = new List<Editorial>
        {
            new Editorial (1, "Planeta"),
            new Editorial (2, "Alma")
        };

        var generos = new List<Genero>
        {
            new Genero (1, "Ciencia Ficción"),
            new Genero (2, "Novela ")
        };

        var libros = new List<Libro>
        {
            new Libro(10, "Dune", "Tapa Dura", "Frank Herbert", 25.00m, 15.00m, 50, 4.8m, new DateTime(1965, 8, 1), generos[0], editoriales[0]),
            new Libro(20, "Moby Dick", "Tapa Blanda", "Herman Melville", 18.50m, 10.00m, 30, 4.6m, new DateTime(2002, 5, 14), generos[1], editoriales[1]),
        };

        // Usuario inventado distinto
        var usuarios = new List<ApplicationUser>
        {
            new ApplicationUser("67", "Bruce", "Wayne", "batman@gotham.es", "Batcueva, 1")  
        };
        usuarios[0].PhoneNumber = "611222333";
        usuarios[0].EmailConfirmed = true;

        // Método de pago inventado 
        var metodosPago = new List<MetodoPago>
        {
            new PayPal(1, "batman@paypal.com")
        };

        // Compra principal
        var compras = new List<Compra>
        {
            new Compra(1, new DateTime(2026, 10, 25), 60.50m, "GOTHAM20", usuarios[0], metodosPago[0])
        };

        // Instancias de CompraItem pasando los 4 parámetros requeridos (id, cantidad, libro, compra)
        var compraItems = new List<CompraItem>
        {
            new CompraItem(1, 2, libros[0], compras[0]), // 2 unidades de Dune
            new CompraItem(2, 1, libros[1], compras[0])  // 1 unidad de Moby Dick
        };

        compras[0].CompraItems.Add(compraItems[0]);
        compras[0].CompraItems.Add(compraItems[1]);

        _context.AddRange(usuarios);
        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.AddRange(metodosPago);
        _context.AddRange(compras);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetDetailsForCompras_NotFound_test()
    {
        // Arrange
        var mock = new Mock<ILogger<ComprasController>>();
        ILogger<ComprasController> logger = mock.Object;
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        
        var controller = new ComprasController(_context, logger, mockUserManager.Object);

        // Act
        var result = await controller.GetDetailsForCompras(999); // ID que no existe

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    [Fact]
    public async Task GetDetailsForCompras_Found_test()
    {
        // Arrange
        var mock = new Mock<ILogger<ComprasController>>();
        ILogger<ComprasController> logger = mock.Object;
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        
        var controller = new ComprasController(_context, logger, mockUserManager.Object);

        // Items esperados con los nuevos datos inventados
        var itemsEsperados = new List<CompraItemDTO>
        {
            new CompraItemDTO("Dune", "Ciencia Ficción", "Planeta", 2),
            new CompraItemDTO("Moby Dick", "Novela ", "Alma", 1)
        };

        // Creamos el objeto completo que esperamos obtener 
        var compraEsperada = new ComprasDetailDTO(
            "Bruce", 
            "Wayne", 
            "Batcueva, 1", 
            "611222333", 
            new DateTime(2026, 10, 25), 
            60.50m, 
            "GOTHAM20", 
            itemsEsperados, 
            "PayPal"
        );

        // Act
        var result = await controller.GetDetailsForCompras(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var compraDTOActual = Assert.IsType<ComprasDetailDTO>(okResult.Value);

        // Comprobación directa del objeto entero utilizando el Equals del DTO
        Assert.Equal(compraEsperada, compraDTOActual);
    }
}
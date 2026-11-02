using AppForSEII.API.Controllers;
using AppForSEII.API.LibroDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII.UT.LibrosController_test
{
    public class GetLibrosParaReponer_test : AppForSEII4SqliteUT
    {
        public GetLibrosParaReponer_test()
        {
            var generos = new List<Genero>() {
                new Genero(5,"Fantasía"),
                new Genero(6,"Misterio"),
                new Genero(7,"Ciencia Ficción")
            };
            
            var editoriales = new List<Editorial>() {
                new Editorial(5,"Minotauro"),
                new Editorial(6,"Planeta")
            };

            // Ajusta los parámetros del constructor de Libro según el orden exacto que tengas en tu clase Libro.cs
            // Ejemplo asumido: new Libro(titulo, genero, fechaLanzamiento, precioReposicion, stock, autor)
            var libros = new List<Libro>() {
                new Libro(5, "El Señor de los Anillos", "Edición Tapa Dura", "J.R.R. Tolkien", 20.0m, 15.0m, 0, 4.9m, new DateTime(1954, 07, 29), generos[0], editoriales[0]),
                new Libro(6, "La Sombra del Viento", "Edición de Bolsillo", "Carlos Ruiz Zafón", 18.0m, 12.5m, 0, 4.8m, new DateTime(2001, 04, 01), generos[1], editoriales[1]),
                new Libro(7, "El Hobbit", "Edición Tapa Dura", "J.R.R. Tolkien", 15.0m, 10.0m, 0, 4.7m, new DateTime(1937, 09, 21), generos[0], editoriales[0]),
                // Este libro tiene stock = 5 (> 0), por lo que NO debe devolverse al buscar libros para reponer
                new Libro(8, "Dune", "Edición Tapa Dura", "Frank Herbert", 25.0m, 18.0m, 5, 4.6m, new DateTime(1965, 08, 01), generos[2], editoriales[0])
            };

            _context.AddRange(generos);
            _context.AddRange(libros);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetLibrosParaReponer_OK()
        {
            var libroDTOs = new List<LibroParaReponerDTO>() {
                new LibroParaReponerDTO(5, "El Señor de los Anillos", "Fantasía", new DateTime(1954, 07, 29), 15.0m, "J.R.R. Tolkien"),
                new LibroParaReponerDTO(6, "La Sombra del Viento", "Misterio", new DateTime(2001, 04, 01), 12.5m, "Carlos Ruiz Zafón"),
                new LibroParaReponerDTO(7, "El Hobbit", "Fantasía", new DateTime(1937, 09, 21), 10.0m, "J.R.R. Tolkien")
            };

            //1: Sin filtros (devuelve todos los libros con stock == 0)
            var libroDTOsSinFiltros = new List<LibroParaReponerDTO>() { libroDTOs[0], libroDTOs[1], libroDTOs[2] };

            //2: Filtrando solo por autor ("J.R.R. Tolkien")
            var libroDTOsAutor = new List<LibroParaReponerDTO>() { libroDTOs[0], libroDTOs[2] };

            //3: Filtrando solo por género ("Misterio")
            var libroDTOsGenero = new List<LibroParaReponerDTO>() { libroDTOs[1] };

            //4: Filtrando por autor y género a la vez
            var libroDTOsAmbos = new List<LibroParaReponerDTO>() { libroDTOs[0], libroDTOs[2] };

            var allTests = new List<object[]>
            {
                // Filtros a aplicar (autor, genero) y lista esperada
                new object[] { null, null, libroDTOsSinFiltros },
                new object[] { "J.R.R. Tolkien", null, libroDTOsAutor },
                new object[] { null, "Misterio", libroDTOsGenero },
                new object[] { "J.R.R. Tolkien", "Fantasía", libroDTOsAmbos }
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetLibrosParaReponer_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetLibrosParaReponer_OK_test(string? filtroAutor, string? filtroGenero,
            IList<LibroParaReponerDTO> expectedLibros)
        {
            // Arrange
            var controller = new LibroController(_context, null);

            // Act
            var result = await controller.GetLibrosParaReponer(filtroAutor, filtroGenero);

            // Assert
            // Comprobamos que el tipo de respuesta es OK (200)
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Obtenemos la lista de libros devuelta y la comparamos con la esperada
            var libroDTOsActual = Assert.IsType<List<LibroParaReponerDTO>>(okResult.Value);
            Assert.Equal(expectedLibros, libroDTOsActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetLibrosParaReponer_badrequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<LibroController>>();
            ILogger<LibroController> logger = mock.Object;
            var controller = new LibroController(_context, logger);
            
            // Forzamos un error en el ModelState para probar el bloque if (!ModelState.IsValid) de tu controlador
            controller.ModelState.AddModelError("Error", "Parámetros de entrada no válidos al buscar libros para reponer.");

            // Act
            var result = await controller.GetLibrosParaReponer(null, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }
    }
}
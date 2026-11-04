namespace AppForSEII.UT.ReposicionesController_test;

using AppForSEII.API.Controllers;
using AppForSEII.API.ReposicionDTOs;
using AppForSEII.API.Models;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

public class CreateReposicion_test : AppForSEII4SqliteUT
{
    public CreateReposicion_test()
    {
        var editoriales = new List<Editorial>
        {
            new Editorial(12, "Minotauro"),
            new Editorial(13, "Planeta")
        };

        var generos = new List<Genero>
        {
            new Genero(12, "Fantasía"),
            new Genero(13, "Misterio")
        };

        // Creo dos libros: uno con stock = 0 (se puede reponer) y otro con stock = 5 (no se puede reponer)
        var libros = new List<Libro>
        {
            new Libro(12, "El Señor de los Anillos", "Edición Tapa Dura", "J.R.R. Tolkien", 20m, 15m, 0, 4.9m, new DateTime(1954, 7, 29), generos[0], editoriales[0]),
            new Libro(13, "La Sombra del Viento", "Edición de Bolsillo", "Carlos Ruiz Zafón", 18m, 12.5m, 5, 4.8m, new DateTime(2001, 4, 1), generos[1], editoriales[1])
        };

        var usuarios = new List<ApplicationUser>
        {
            new ApplicationUser("1", "Héctor", "Fernández", "hector@uclm.es", "Avda. España s/n")
        };
        usuarios[0].PhoneNumber = "600555111";

        var metodoPago = new PayPal(12, "600555111");

        _context.AddRange(usuarios);
        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.AddRange(metodoPago);
        _context.SaveChanges();
    }

    public static IEnumerable<object[]> TestCasesForCreateReposicion()
    {
        // Lista de items válida para usar en las pruebas de usuario y método de pago
        var itemsValidos = new List<ReposicionItemDTO>
        {
            new ReposicionItemDTO("El Señor de los Anillos", "J.R.R. Tolkien", 2)
        };

        // Caso 1: reposicionItem es null
        var reposicionItemNull = new ReposicionForCreateDTO("Héctor", "Fernández", "600777666", 12, null, "Comentario válido de más de veinte caracteres");

        // Caso 2: reposicionItem está vacío
        var reposicionItemCero = new ReposicionForCreateDTO("Héctor", "Fernández", "600777666", 12, new List<ReposicionItemDTO>(), "Comentario válido de más de veinte caracteres");

        // Caso 3: usuario no existe ("Jesus" "Díaz")
        var usuarioNull = new ReposicionForCreateDTO("Jesus", "Díaz", "600777999", 12, itemsValidos, "Comentario válido de más de veinte caracteres");

        // Caso 4: método de pago no existe
        var metodoPagoNull = new ReposicionForCreateDTO("Héctor", "Fernández", "600777666", 100000, itemsValidos, "Comentario válido de más de veinte caracteres");

        // Caso 5: libro no existe ("Libro no válido")
        var libroNull = new ReposicionForCreateDTO("Héctor", "Fernández", "600777666", 12, new List<ReposicionItemDTO> { new ReposicionItemDTO("Libro no válido", "Autor no válido", 2) }, "Comentario válido de más de veinte caracteres");

        // Caso 6: stock es mayor a 0
        var stockMayor0 = new ReposicionForCreateDTO("Héctor", "Fernández", "600777666", 12, new List<ReposicionItemDTO> { new ReposicionItemDTO("La Sombra del Viento", "Carlos Ruiz Zafón", 2) }, "Comentario válido de más de veinte caracteres");

        var objetosADevolver = new List<object[]>
        {
            new object[] { reposicionItemNull, "El carrito de reposición no puede estar vacío." },
            new object[] { reposicionItemCero, "El carrito de reposición no puede estar vacío." },
            // CORREGIDO: ahora espera "Jesus" y "Díaz" igual que enviamos en usuarioNull
            new object[] { usuarioNull, "No existe el usuario con el nombre Jesus y apellido Díaz introducido en la base de datos" },
            new object[] { metodoPagoNull, "El método de pago no se encuentra en la base de datos" },
            // CORREGIDO: ahora espera 'Libro no válido' igual que enviamos en libroNull
            new object[] { libroNull, "El libro con título 'Libro no válido' no existe en el catálogo." },
            new object[] { stockMayor0, "No se puede reponer el libro 'La Sombra del Viento' porque actualmente hay stock disponible (5 unidades). Solo se pueden reponer libros con stock = 0." }
        };

        return objetosADevolver;
    }

    [Theory]
    [MemberData(nameof(TestCasesForCreateReposicion))]
    public async Task CreateReposicion_Error_test(ReposicionForCreateDTO reposicionDTO, string errorEsperado)
    {
        // Arrange
        var mock = new Mock<ILogger<ReposicionesController>>();
        ILogger<ReposicionesController> logger = mock.Object;
        var controller = new ReposicionesController(_context, logger);

        // Act
        var result = await controller.CreateReposicion(reposicionDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
        var errorReal = problemDetails.Errors.First().Value[0];
        Assert.StartsWith(errorEsperado, errorReal);
    }

    [Fact]
    public async Task CreateReposicion_Success_test()
    {
        // Arrange
        var mock = new Mock<ILogger<ReposicionesController>>();
        ILogger<ReposicionesController> logger = mock.Object;
        var controller = new ReposicionesController(_context, logger);

        // Creamos el DTO de entrada ReposicionForCreateDTO
        var reposicionDTO = new ReposicionForCreateDTO(
            "Héctor",
            "Fernández",
            "600555111",
            12,
            new List<ReposicionItemDTO> { new ReposicionItemDTO("El Señor de los Anillos", "J.R.R. Tolkien", 2) },
            "Comentario válido de más de veinte caracteres"
        );

        //Creamos el detail de salida
        var detailDTOEsperado = new ReposicionDetailDTO(
            "Héctor",
            "Fernández",
            "600555111",
            DateTime.Today,
            30.0,
            new List<ReposicionItemDTO> { new ReposicionItemDTO("El Señor de los Anillos", "J.R.R. Tolkien", 2) },
            "PayPal"
        );

        // Act
        var result = await controller.CreateReposicion(reposicionDTO);

        // Assert
        var resultCreado = Assert.IsType<CreatedAtActionResult>(result);
        var reposicionDetailDTOReal = Assert.IsType<ReposicionDetailDTO>(resultCreado.Value);

        Assert.Equal(detailDTOEsperado, reposicionDetailDTOReal);
    }
}
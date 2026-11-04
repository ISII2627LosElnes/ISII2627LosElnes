
namespace AppForSEII.UT.ComprasController_test;

using AppForSEII.API.Controllers;
using AppForSEII.API.CompraDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

public class CreateCompra_test : AppForSEII4SqliteUT
{
    public CreateCompra_test()
    {
        var editoriales = new List<Editorial>
        {
            new Editorial(1, "Planeta"),
            new Editorial(2, "Alma")
        };

        var generos = new List<Genero>
        {
            new Genero(1, "Ciencia Ficción"),
            new Genero(2, "Novela ")
        };

        var libros = new List<Libro>
        {
            new Libro(10, "Dune", "Tapa Dura", "Frank Herbert", 25.00m, 15.00m, 50, 4.8m, new DateTime(1965, 8, 1), generos[0], editoriales[0]),
            new Libro(20, "Moby Dick", "Tapa Blanda", "Herman Melville", 18.50m, 10.00m, 30, 4.6m, new DateTime(2002, 5, 14), generos[1], editoriales[1])
        };

        var usuarios = new List<ApplicationUser>
        {
            new ApplicationUser("67", "Bruce", "Wayne", "batman@gotham.es", "Batcueva, 1")
        };
        usuarios[0].PhoneNumber = "611222333";
        usuarios[0].EmailConfirmed = true;

        var metodosPago = new List<MetodoPago>
        {
            new PayPal(1, "batman@paypal.com")
        };

        _context.AddRange(usuarios);
        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.AddRange(metodosPago);
        _context.SaveChanges();
    }

    // Mock necesario para simular la actualización de los datos del usuario
    private Mock<UserManager<ApplicationUser>> CrearMockUserManager()
    {
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
        return mockUserManager;
    }

    public static IEnumerable<object[]> TestCasesForCreateCompra()
    {
        // Caso 1: librosSeleccionados nulo
        var librosNull = new CompraForCreateDTO
        {
            Nombre = "Bruce", Apellidos = "Wayne", Direccion = "Batcueva, 1",
            Telefono = "611222333", MetodoPagoId = 1, CodigoDescuento = "GOTHAM20",
            LibrosSeleccionados = null!
        };

        // Caso 2: librosSeleccionados.count = 0
        var librosCero = new CompraForCreateDTO
        {
            Nombre = "Bruce", Apellidos = "Wayne", Direccion = "Batcueva, 1",
            Telefono = "611222333", MetodoPagoId = 1, CodigoDescuento = "GOTHAM20",
            LibrosSeleccionados = new List<CompraItemForCreateDTO>()
        };

        // Caso 3: método de pago inexistente
        var metodoPagoInexistente = new CompraForCreateDTO
        {
            Nombre = "Bruce", Apellidos = "Wayne", Direccion = "Batcueva, 1",
            Telefono = "611222333", MetodoPagoId = 999, CodigoDescuento = "GOTHAM20",
            LibrosSeleccionados = new List<CompraItemForCreateDTO> { new CompraItemForCreateDTO { LibroId = 10, Cantidad = 2 } }
        };

        // Caso 4: usuario inexistente
        var usuarioInexistente = new CompraForCreateDTO
        {
            Nombre = "Clark", Apellidos = "Kent", Direccion = "Calle Krypton, 1",
            Telefono = "611222333", MetodoPagoId = 1, CodigoDescuento = "GOTHAM20",
            LibrosSeleccionados = new List<CompraItemForCreateDTO> { new CompraItemForCreateDTO { LibroId = 10, Cantidad = 2 } }
        };

        // Caso 5: stock insuficiente
        var stockInsuficiente = new CompraForCreateDTO
        {
            Nombre = "Bruce", Apellidos = "Wayne", Direccion = "Batcueva, 1",
            Telefono = "611222333", MetodoPagoId = 1, CodigoDescuento = "GOTHAM20",
            LibrosSeleccionados = new List<CompraItemForCreateDTO> { new CompraItemForCreateDTO { LibroId = 10, Cantidad = 51 } }
        };

        // Los objetos a devolver
        var objetosADevolver = new List<object[]>
        {
            new object[] { librosNull, "Debe seleccionar al menos un libro." },
            new object[] { librosCero, "Debe seleccionar al menos un libro." },
            new object[] { metodoPagoInexistente, "El método de pago no es válido." },
            new object[] { usuarioInexistente, "No existe el usuario con el nombre Clark" },
            new object[] { stockInsuficiente, "No hay stock suficiente para 'Dune'." }
        };

        return objetosADevolver;
    }

    [Theory]
    [MemberData(nameof(TestCasesForCreateCompra))]
    public async Task CreateCompra_Error_test(CompraForCreateDTO compraDTO, string errorEsperado)
    {
        // arrange
        var mock = new Mock<ILogger<ComprasController>>();
        ILogger<ComprasController> logger = mock.Object;
        var mockUserManager = CrearMockUserManager();
        var controller = new ComprasController(_context, logger, mockUserManager.Object);

        // act
        var result = await controller.CreateCompra(compraDTO);

        // Assert
        // comprueba que createcompra devuelve un bad request con contenido
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        // comprueba que el contenido del badrequest es el mensaje de error
        var errorReal = Assert.IsType<string>(badRequestResult.Value);
        // comprueba que el mensaje obtenido coincide con el error esperado
        Assert.StartsWith(errorEsperado, errorReal);
    }

    [Fact]
    public async Task CreateCompra_Success_test()
    {
        // Arrange
        var mock = new Mock<ILogger<ComprasController>>();
        ILogger<ComprasController> logger = mock.Object;
        var mockUserManager = CrearMockUserManager();
        var controller = new ComprasController(_context, logger, mockUserManager.Object);

        // preparamos el dto que vamos a usar de parámetro
        var compraDTO = new CompraForCreateDTO
        {
            Nombre = "Bruce", Apellidos = "Wayne", Direccion = "Batcueva, 2",
            Telefono = "622333444", MetodoPagoId = 1, CodigoDescuento = "GOTHAM20",
            LibrosSeleccionados = new List<CompraItemForCreateDTO>
            {
                new CompraItemForCreateDTO { LibroId = 10, Cantidad = 2 },
                new CompraItemForCreateDTO { LibroId = 20, Cantidad = 2 }
            }
        };

        // act
        var fechaAntes = DateTime.Now;
        var result = await controller.CreateCompra(compraDTO);
        var fechaDespues = DateTime.Now;

        // Assert
        var resultCreado = Assert.IsType<CreatedAtActionResult>(result);
        var compraDetailDTOReal = Assert.IsType<ComprasDetailDTO>(resultCreado.Value);

        // ahora el detaildto que debería devolver
        var DetailDTOEsperado = new ComprasDetailDTO(
            "Bruce", "Wayne", "Batcueva, 2", "622333444",
            compraDetailDTOReal.FechaCompra, 87.00m, "GOTHAM20",
            new List<CompraItemDTO>
            {
                new CompraItemDTO("Dune", "Ciencia Ficción", "Planeta", 2),
                new CompraItemDTO("Moby Dick", "Novela ", "Alma", 2)
            },
            "PayPal"
        );

        // comprobamos que la fecha de compra corresponde al momento de realizarla
        Assert.InRange(compraDetailDTOReal.FechaCompra, fechaAntes, fechaDespues);

        // comprobamos que CreateCompra devuelve el objeto completo esperado
        Assert.Equal(DetailDTOEsperado, compraDetailDTOReal);

        // comprobamos que se ha actualizado el stock de los libros
        var dune = await _context.Libros.FindAsync(10);
        var mobyDick = await _context.Libros.FindAsync(20);

        Assert.Equal(48, dune!.Stock);
        Assert.Equal(28, mobyDick!.Stock);
    }
}

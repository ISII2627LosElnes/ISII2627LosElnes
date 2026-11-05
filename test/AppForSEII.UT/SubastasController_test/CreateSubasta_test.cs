namespace AppForSEII.UT.SubastasController_test;

using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.SubastaDTOs;
using AppForSEII.API.Models;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

public class CreateSubasta_test : AppForSEII4SqliteUT
{
    public CreateSubasta_test()
    {
        var editoriales = new List<Editorial>
        {
            new Editorial (1, "Ediciones Martínez Roca"),
            new Editorial (2, "Anaya")
        };

        var generos = new List<Genero>
        {
            new Genero (1, "Ficción"),
            new Genero (2, "Infantil")
        };

        var metodosPago = new List<MetodoPago>
        {
            new Visa (1, "328372924", new DateTime(2026,11,15)),
            new PayPal (2, "384617483")
        };  

        var libros = new List<Libro>
        {
            new Libro(1,"Cronicas de la torre","Tapa Blanda", "Laura Gallego", 20, 10, 0, 4 , new DateTime(2017,9,5), generos[0], editoriales[0]),
            new Libro(2, "Caperucita roja", "Tapa Blanda", "Charles", 12, 8, 10, 4, new DateTime(2000, 1, 1), generos[1], editoriales[1]),
        };

        var usuarios = new List<ApplicationUser>
        {
          new ApplicationUser("1", "Mark", "Evans", "MarkEvans","Calle Miami Metro, 10")  
        };
        usuarios[0].PhoneNumber="410723429";

        _context.AddRange(usuarios);
        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.AddRange(metodosPago);
        _context.SaveChanges();
    }

    public static IEnumerable<object[]> TestCasesForCreateSubasta()
    {
        // Lista de items válida para usar en las pruebas de usuario y método de pago
        var itemsValidos = new List<SubastaItemDTO>
        {
            new SubastaItemDTO("Juego de tronos", "George", "Una obra que hay que consumir sí o sí", 15m, 2)
        };

        // Caso 1: subastaItem es null
        //var subastaItemNull = new SubastaForCreateDTO("Mark", "Evans", "410723429", 1, "Comentario válido de más de veinte caracteres", 7.5m, null);

        // Caso 2: subastaItem está vacío
        var subastaItemCero = new SubastaForCreateDTO("Mark", "Evans", "410723429", 1, "Comentario válido de más de veinte caracteres", new List<SubastaItemDTO>());

        //Caso 3: PrecioTotal < 0
        //var precioMenor0 = new SubastaForCreateDTO("Mark", "Evans", "410723429", 1, "Comentario válido de más de veinte caracteres", -2m, itemsValidos);

        // Caso 4: método de pago no existe
        var metodoPagoNull = new SubastaForCreateDTO("Mark", "Evans", "410723429", 600, "Comentario válido de más de veinte caracteres", itemsValidos);

        // Caso 5: usuario no existe ("Axel" "Blaze")
        var usuarioNull = new SubastaForCreateDTO("Axel", "Blaze", "414563429", 1, "Comentario válido de más de veinte caracteres", itemsValidos);

        // Caso 6: libro no existe ("Libro no válido")
        var libroNull = new SubastaForCreateDTO("Mark", "Evans", "410723429", 1, "Comentario válido de más de veinte caracteres", new List<SubastaItemDTO> { new SubastaItemDTO("Los 7 enenitos", "Michael Jackson", "Muy recomendado para niños", 5m, 1)});

        //Caso 7: no se selecciona ninguna unidad para subastar
        //var sinItemsSeleccionados = new SubastaForCreateDTO("Mark", "Evans", "410723429", 1, "Comentario válido de más de veinte caracteres", 7.5m, );

        // Caso 8: stock insuficiente
        var stockInsuficiente = new SubastaForCreateDTO("Mark", "Evans", "410723429", 1, "Comentario válido de más de veinte caracteres", new List<SubastaItemDTO> { new SubastaItemDTO("Cronicas de la torre", "Laura Gallego", "De españoles para españoles", 14m, 1)});

        //Caso 9: precio de la puja <= 0
        var precioMenorA0 = new SubastaForCreateDTO("Mark", "Evans", "410723429", 1, "Comentario válido de más de veinte caracteres", new List<SubastaItemDTO> { new SubastaItemDTO("Caperucita roja", "Charles", "Recomendado para niños", -4m, 1)});

        var objetosADevolver = new List<object[]>
        {
            //new object[] { subastaItemNull, "Error! Debes seleccionar al menos un libro para inciar la puja" },
            new object[] { subastaItemCero, "Error! Debes seleccionar al menos un libro para inciar la puja" },
            //new object[] { precioMenor0, "Error! Debes poner una puja mayor a 0" },
            // CORREGIDO: ahora espera "Axel" y "Blaze" igual que enviamos en usuarioNull
            new object[] { metodoPagoNull, "Error! El método de pago no es válido." },
            new object[] { usuarioNull, "Error! Usuario no registrado" },
            // CORREGIDO: ahora espera 'Libro no válido' igual que enviamos en libroNull
            new object[] { libroNull, "Error! Libro titulado 'Los 7 enenitos' no está disponible para ser subastado" },
            new object[] { stockInsuficiente, "Error! Libro titulado 'Cronicas de la torre' no está disponible para ser subastado" },
            new object[] { precioMenorA0, "Error! El precio de la puja para 'Caperucita roja' debe ser mayor de 0" }
        };

        return objetosADevolver;
    }

    [Theory]
    [MemberData(nameof(TestCasesForCreateSubasta))]
    public async Task CreateSubasta_Error_test(SubastaForCreateDTO subastaDTO, string errorEsperado)
    {
        // Arrange
        var mock = new Mock<ILogger<SubastasController>>();
        ILogger<SubastasController> logger = mock.Object;
        var controller = new SubastasController(_context, logger);

        // Act
        var result = await controller.CreateSubasta(subastaDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
    
        var errorReal = problemDetails.Errors.First().Value[0];
        Assert.StartsWith(errorEsperado, errorReal);
    }

    [Fact]
    public async Task CreateSubasta_Success_test()
    {
        // Arrange
        var mock = new Mock<ILogger<SubastasController>>();
        ILogger<SubastasController> logger = mock.Object;
        var controller = new SubastasController(_context, logger);

        // Creamos el DTO de entrada ReposicionForCreateDTO
        var subastaDTO = new SubastaForCreateDTO(
            "Mark",
            "Evans",
            "410723429",
            1, 
            "Comentario válido de más de veinte caracteres",
            new List<SubastaItemDTO> { new SubastaItemDTO("Caperucita roja", "Charles", "Una obra que hay que consumir sí o sí", 15m, 2) }
        );

        //Creamos el detail de salida
        var detailDTOEsperado = new SubastaDetailDTO(
            "Mark",
            "Evans",
            "410723429",
            DateTime.Today,
            15m,
            new List<SubastaItemDTO> { new SubastaItemDTO("Caperucita roja", "Charles", "Una obra que hay que consumir sí o sí", 15m, 2) }
        );

        // Act
        var result = await controller.CreateSubasta(subastaDTO);

        // Assert
        var resultCreado = Assert.IsType<CreatedAtActionResult>(result);
        var subastaDetailDTOReal = Assert.IsType<SubastaDetailDTO>(resultCreado.Value);

        Assert.Equal(detailDTOEsperado, subastaDetailDTOReal);
    }
}
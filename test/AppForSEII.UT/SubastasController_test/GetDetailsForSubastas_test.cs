namespace AppForSEII.UT.SubastasController_test;

using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.SubastaDTOs;
using AppForSEII.API.Models;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

public class GetDetailsForSubastas_test : AppForSEII4SqliteUT
{
    public GetDetailsForSubastas_test()
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
            new Libro(1,"Cronicas de la torre","Tapa Blanda", "Laura Gallego", 20, 10, 4, 4 , new DateTime(2017,9,5), generos[0], editoriales[0]),
            new Libro(2, "Caperucita roja", "Tapa Blanda", "Charles", 12, 8, 10, 4, new DateTime(2000, 1, 1), generos[1], editoriales[1]),
        };

        var usuarios = new List<ApplicationUser>
        {
          new ApplicationUser("1", "Mark", "Evans", "MarkEvans","Calle Miami Metro, 10")  
        };
        usuarios[0].PhoneNumber="410747629";

        var subastas = new List<Subasta>
        {
          new Subasta(1, 19.4m, new DateTime(2026,12,21), metodosPago[0], usuarios[0]),
        };
        subastas[0].Usuario=usuarios[0];

        var subastaItems = new List<SubastaItem>
        {
            new SubastaItem("Libro muy interesante, con buen trasfondo", 10m, libros[0], subastas[0], 1),
            new SubastaItem("Demasiado complicado de entender", 4.7m, libros[1], subastas[0], 2)
        };

        subastas[0].SubastaItems.Add(subastaItems[0]);
        subastas[0].SubastaItems.Add(subastaItems[1]);

        _context.AddRange(usuarios);
        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.AddRange(subastas);
        _context.AddRange(metodosPago);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetDetailsForSubastas_NotFound_test()
    {
        //arrange
        var mock = new Mock<ILogger<SubastasController>>();
        ILogger<SubastasController> logger = mock.Object;
        var controller = new SubastasController(_context, logger);
        //act
        var result=await controller.GetDetailsForSubastas(100); //id que no existe
        //assert

        //comprueba que el controlador devuelve un notfound con contenido (por eso es object, porque el notfound devuelve un mensaje string con el error)
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetDetailsForSubastas_Found_test()
    {
        //arrange
          var mock = new Mock<ILogger<SubastasController>>();
        ILogger<SubastasController> logger = mock.Object;
        var controller = new SubastasController(_context, logger);
        var subastaEsperada= new SubastaDetailDTO("Mark", "Evans", "410747629", new DateTime(2026,12,21), 19.4m, new List<SubastaItemDTO>());
        subastaEsperada.SubastaItems.Add(new SubastaItemDTO("Cronicas de la torre","Laura Gallego", "Libro muy interesante, con buen trasfondo", 10, 1));
        subastaEsperada.SubastaItems.Add(new SubastaItemDTO("Caperucita roja", "Charles", "Demasiado complicado de entender", 4.7m, 2));
        //act
        var result = await controller.GetDetailsForSubastas(1);

        //assert

        //comprueba que el resultado devuelto por el controlador es un okobject result
        var okResult = Assert.IsType<OkObjectResult>(result);
        //extrae el objeto devuelto del ok y comprueba que es un subastadetaildto
        var subastaDTOActual = Assert.IsType<SubastaDetailDTO>(okResult.Value);
        //compara el dto esperado con el obtenido
        Assert.Equal(subastaEsperada, subastaDTOActual);
    }
}
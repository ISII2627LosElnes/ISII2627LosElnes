namespace AppForSEII.UT.ResenasController_test;

using AppForSEII.API.Controllers;
using AppForSEII.API.ResenaDTOs;
using AppForSEII.API.Models;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

public class GetDetailsForResenas_test : AppForSEII4SqliteUT
{
    public GetDetailsForResenas_test()
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

        var libros = new List<Libro>
        {
             new Libro(1,"La leyenda de las tierras raras","Tapa Blanda", "Folagor", 20, 10, 4, 4 , new DateTime(2017,9,5), generos[0], editoriales[0]),
            new Libro(2, "Los tres cerditos", "Tapa Blanda", "Anónimo", 12, 8, 10, 4, new DateTime(2000, 1, 1), generos[1], editoriales[1]),
        };

        var usuarios = new List<ApplicationUser>
        {
          new ApplicationUser("1", "Dexter", "Morgan", "BayHarbourButcher","Calle Miami Metro, 10")  
        };
        usuarios[0].PhoneNumber="600777666";

        var resenas = new List<Resena>
        {
          new Resena(1,new DateTime(2026,10,20),"Muy entretenido")  
        };
        resenas[0].Usuario=usuarios[0];

        var resenaItems = new List<ResenaItem>
        {
            new ResenaItem(libros[0], "Me ha gustado mucho el libro para ser de un Youtuber", 5, resenas[0]),
            new ResenaItem(libros[1],"Demasiado complicado de entender", 2, resenas[0])
        };

        resenas[0].ResenaItems.Add(resenaItems[0]);
        resenas[0].ResenaItems.Add(resenaItems[1]);

        _context.AddRange(usuarios);
        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.AddRange(resenas);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetDetailsForResenas_NotFound_test()
    {
        //arrange
        var mock = new Mock<ILogger<ResenasController>>();
        ILogger<ResenasController> logger = mock.Object;
        var controller = new ResenasController(_context, logger);
        //act
        var result=await controller.GetDetailsForResenas(100); //id que no existe
        //assert

        //comprueba que el controlador devuelve un notfound con contenido (por eso es object, porque el notfound devuelve un mensaje string con el error)
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetDetailsForResenas_Found_test()
    {
        //arrange
          var mock = new Mock<ILogger<ResenasController>>();
        ILogger<ResenasController> logger = mock.Object;
        var controller = new ResenasController(_context, logger);
        var resenaEsperada= new ResenaDetailDTO("Dexter", "Morgan", "600777666", new DateTime(2026,10,20), new List<ResenaItemDTO>());
        resenaEsperada.ResenaItems.Add(new ResenaItemDTO("La leyenda de las tierras raras","Ediciones Martínez Roca", "Tapa Blanda", 5, "Me ha gustado mucho el libro para ser de un Youtuber"));
        resenaEsperada.ResenaItems.Add(new ResenaItemDTO("Los tres cerditos","Anaya", "Tapa Blanda", 2, "Demasiado complicado de entender"));
        //act
        var result = await controller.GetDetailsForResenas(1);

        //assert

        //comprueba que el resultado devuelto por el controlador es un okobject result
        var okResult = Assert.IsType<OkObjectResult>(result);
        //extrae el objeto devuelto del ok y comprueba que es un resenadetaildto
        var resenaDTOActual = Assert.IsType<ResenaDetailDTO>(okResult.Value);
        //compara el dto esperado con el obtenido
        Assert.Equal(resenaEsperada, resenaDTOActual);
    }
}
namespace AppForSEII.UT.ResenasController_test;

using AppForSEII.API.Controllers;
using AppForSEII.API.ResenaDTOs;
using AppForSEII.API.Models;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

public class CreateResena_test : AppForSEII4SqliteUT
{
    public CreateResena_test()
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
          new ApplicationUser("1", "Dexter", "Morgan", "BayHarbourButcher","Calle Miami Metro, 10", "600777666"),  
        };
      


        _context.AddRange(usuarios);
        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.SaveChanges();

    }

    public static IEnumerable<object[]> TestCasesForCreateResena()
    {
        //caso 1: resenaitems nulo
        var resenaItemNull= new ResenaForCreateDTO("Dexter","Morgan","Calle Miami Metro, 10","600777666","Muy buen libro",null);
        //caso2: resenaitems.count=0
         var resenaItemCero= new ResenaForCreateDTO("Dexter","Morgan","Calle Miami Metro, 10","600777666","Muy buen libro",new List<ResenaItemDTO>());
        //estas dos cubren el mismo if, que tiene las dos condiciones con un or

        //caso 3: usuario inexistente
        var usuarioInexistente = new ResenaForCreateDTO("Patrick","Jane","Calle Red John, 10", "600700800", "No me gustó",
         new List<ResenaItemDTO> {new ResenaItemDTO("La leyenda de las tierras raras", "Ediciones Martínez Roca", "Tapa Blanda", 4, "Me ha gustado muchísimo este libro") });

        //los objects
        var objetosADevolver = new List<object[]>
        {
            new object[] {resenaItemNull, "Debes seleccionar al menos un libro."},
            new object[] {resenaItemCero, "Debes seleccionar al menos un libro."},
            new object[] {usuarioInexistente, "¡Error! El usuario no está registrado"}
        };
        return objetosADevolver;
    }
    

    [Theory]
    [MemberData(nameof(TestCasesForCreateResena))]
    public async Task CreateResena_Error_test(ResenaForCreateDTO resenaDTO, string errorEsperado)
    {
        //arrange
        var mock = new Mock<ILogger<ResenasController>>();
        ILogger<ResenasController> logger = mock.Object;
        var controller = new ResenasController(_context,logger);

        //act
        var result = await controller.CreateResena(resenaDTO);

        //Assert
        //comprueba que createresena devuelve un bad request con contenido
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //comprueba que el contenido del badrequest son los errores de validacion
         var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
         //ovtiene el primer mensaje de error devuelto
        var errorReal = problemDetails.Errors.First().Value[0];
        //comprueba que el mensaje obtenido coincide con el error esperado
        Assert.StartsWith(errorEsperado, errorReal);
    }

    [Fact]  
    public async Task CreateResena_Success_test()  
    {
            //Arrange
        var mock = new Mock<ILogger<ResenasController>>();
        ILogger<ResenasController> logger = mock.Object;
        var controller = new ResenasController(_context,logger);
        //preparamos el dto que vamos a usar de parametro
        var resenaDTO = new ResenaForCreateDTO("Dexter", "Morgan","Calle Miami Metro, 10","600777666","Muy buenos libros",
        new List<ResenaItemDTO>() {new ResenaItemDTO("La leyenda de las tierras raras", "Ediciones Martínez Roca", "Tapa Blanda", 5,"Demasiado bueno para ser verdad")} );
        //ahora el detaildto que deberia de devolver
        var DetailDTOEsperado= new ResenaDetailDTO("Dexter", "Morgan", "600777666",DateTime.Today, new List<ResenaItemDTO>() { new ResenaItemDTO("La leyenda de las tierras raras", "Ediciones Martínez Roca", "Tapa Blanda", 5,"Demasiado bueno para ser verdad") });
        
        //act
        var result = await controller.CreateResena(resenaDTO);

        //assert
        var resultCreado=Assert.IsType<CreatedAtActionResult>(result);
        var resenaDetailDTOReal=Assert.IsType<ResenaDetailDTO>(resultCreado.Value);

        Assert.Equal(DetailDTOEsperado, resenaDetailDTOReal);
    }



}
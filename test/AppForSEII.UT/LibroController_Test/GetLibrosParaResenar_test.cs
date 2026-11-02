namespace AppForSEII.UT.LibroController_Test;
using AppForSEII.API.Controllers;
using AppForSEII.API.LibroDTOs;
using AppForSEII.API.Models;
public class GetLibrosParaResenar_test : AppForSEII4SqliteUT
{
        //constructor con los datos necesarios para la prueba de mi select
        public GetLibrosParaResenar_test()
    {
        var generos= new List<Genero>
        {
            new Genero(1, "Ficción"),
            new Genero(2, "Infantil"),
            new Genero(3,"Fantasía"),
            new Genero(4,"Novela")
        };

        var editoriales = new List<Editorial>
        {
          new Editorial(1,"Ediciones Martínez Roca"), 
          new Editorial(2,"Anaya"),
          new Editorial(3,"Salamandra"),
          new Editorial(4,"Penguin Clásicos")
        };

        var libros = new List<Libro>
        {
            //tapa blanda + ficcion
            new Libro(1,"La leyenda de las tierras raras","Tapa Blanda", "Folagor", 20, 10, 4, 4 , new DateTime(2017,9,5), generos[0], editoriales[0]),
            //tapa blanda + infantil
            new Libro(2, "Los tres cerditos", "Tapa Blanda", "Anónimo", 12, 8, 10, 4, new DateTime(2000, 1, 1), generos[1], editoriales[1]),
            //tapa dura + fantasia
            new Libro(3, "Harry Potter y la piedra filosofal", "Tapa Dura", "J.K. Rowling", 20, 15, 12, 5, new DateTime(1997, 6, 26), generos[2], editoriales[2]),
            //tapa blanda + novela
            new Libro(4, "Don Quijote de la Mancha", "Tapa Blanda", "Miguel de Cervantes", 18, 12, 6, 4, new DateTime(1605, 1, 16), generos[3], editoriales[3])
            
        };

        _context.AddRange(generos);
        _context.AddRange(editoriales);
        _context.AddRange(libros);
        _context.SaveChanges();
    }


    //member data para identificar las fuentes de datos de la prueba
    /*
    CASOS DE PRUEBA
    -1 tipolibro y genero null -> devuelve todos los libros
    -2 tipolibro Tapa Blanda y genero null -> devuelve la leyenda de las tierras raras, los tres cerditos y don quijote
    -3 tipolibro null y genero ficcion -> devuelve solo la leyenda de las tierras raras
    -4 tipolibro tapa blanda y genero ficcion -> devuelve solo la leyenda de las tierras raras
    */
    public static IEnumerable<object[]> TestCasesForGetLibrosParaResenar()
    {
        //caso 1
        var todosLosLibros = new List<LibroParaResenarDTO>
        {
            new LibroParaResenarDTO(1, "La leyenda de las tierras raras", "Tapa Blanda", "Ficción", "Ediciones Martínez Roca", "Folagor", 4, new DateTime(2017, 9, 5)),
            new LibroParaResenarDTO(2, "Los tres cerditos", "Tapa Blanda", "Infantil", "Anaya", "Anónimo", 4, new DateTime(2000, 1, 1)),
            new LibroParaResenarDTO(3, "Harry Potter y la piedra filosofal", "Tapa Dura", "Fantasía", "Salamandra", "J.K. Rowling", 5, new DateTime(1997, 6, 26)),
            new LibroParaResenarDTO(4, "Don Quijote de la Mancha", "Tapa Blanda", "Novela", "Penguin Clásicos", "Miguel de Cervantes", 4, new DateTime(1605, 1, 16))
        };
        //caso 2
        var librosTapaBlanda = new List<LibroParaResenarDTO>
        {
            todosLosLibros[0],
            todosLosLibros[1],
            todosLosLibros[3]
        };
        //caso 3
        var librosFiccion = new List<LibroParaResenarDTO>
        {
            todosLosLibros[0]
        };
        //caso 4
        var librosTapaBlandaFiccion = new List<LibroParaResenarDTO>
        {
            todosLosLibros[0]
        };

        var objetodevolver = new List<object[]>
        {
            new object[] { null, null, todosLosLibros },
            new object[] { "Tapa Blanda", null, librosTapaBlanda },
            new object[] { null, "Ficción", librosFiccion },
            new object[] { "Tapa Blanda", "Ficción", librosTapaBlandaFiccion }
        };

        return objetodevolver;

    }

    //método para comprobar el ok
    [Theory] //porque tiene argumentos
    [MemberData(nameof(TestCasesForGetLibrosParaResenar))]
    public async Task GetLibrosParaResenar_OK_test(string? tipoLibro, string? genero, List<LibroParaResenarDTO> librosEsperados)
    {
        //arrange
        var mock = new Mock<ILogger<LibroController>>();
        ILogger<LibroController> logger = mock.Object;
        LibroController controller = new LibroController(_context, logger);

        //act
        var result = await controller.GetLibrosParaResenar(tipoLibro,genero);

        //assert
        var okresultado = Assert.IsType<OkObjectResult>(result);
        var libroresultadoDeVerdad= Assert.IsType<List<LibroParaResenarDTO>>(okresultado.Value);
        Assert.Equal(librosEsperados, libroresultadoDeVerdad);
        
    }
}


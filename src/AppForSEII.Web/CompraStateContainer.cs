
using AppForSEII.Web.API;

namespace AppForSEII.Web;

public class CompraStateContainer
{
    // Libros seleccionados para comprar
    private readonly List<LibroParaComprarDTO> librosCarrito = new();

    public IReadOnlyList<LibroParaComprarDTO> LibrosCarrito => librosCarrito;

    // Datos de la compra
    public CompraForCreateDTO Compra { get; private set; } = new();

    // Calcula el precio total del carrito
    public double PrecioTotal => librosCarrito.Sum(libro =>
        libro.PrecioCompra * (Compra.LibrosSeleccionados
            .FirstOrDefault(item => item.LibroId == libro.Id)?.Cantidad ?? 1));

    // Comprueba si un libro ya está en el carrito
    public bool ContieneLibro(int libroId)
    {
        return librosCarrito.Any(libro => libro.Id == libroId);
    }

    // Añade un libro al carrito
    public void AddLibroToCompra(LibroParaComprarDTO libro)
    {
        if (!ContieneLibro(libro.Id))
        {
            librosCarrito.Add(libro);

            Compra.LibrosSeleccionados.Add(new CompraItemForCreateDTO
            {
                LibroId = libro.Id,
                Cantidad = 1
            });
        }
    }

    // Elimina un libro del carrito
    public void RemoveLibroFromCompra(int libroId)
    {
        librosCarrito.RemoveAll(libro => libro.Id == libroId);

        var item = Compra.LibrosSeleccionados
            .FirstOrDefault(i => i.LibroId == libroId);

        if (item != null)
        {
            Compra.LibrosSeleccionados.Remove(item);
        }
    }

    // Vacía el carrito de compra
    public void ClearCompra()
    {
        librosCarrito.Clear();
        Compra = new CompraForCreateDTO();
    }
}

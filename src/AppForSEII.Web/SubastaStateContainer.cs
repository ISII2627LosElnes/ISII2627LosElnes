using AppForSEII.Web.API;

namespace AppForSEII.Web
{
    public class SubastaStateContainer
    {
        // Instancia de la lista de libros seleccionados que se compartirá entre páginas
        public List<LibroParaSubastarDTO> librosSubasta { get; private set; } = new();

        // Comprueba si un libro ya está añadido al carrito de pujas
        public bool EstaLibroSeleccionado(LibroParaSubastarDTO libro)
        {
            return librosSubasta.Any(si => si.Titulo == libro.Titulo);
        }

        // Añade un libro a la lista de pujas evitando duplicados (Paso 3)
        public void AnadirLibroSubasta(LibroParaSubastarDTO libro)
        {
            if (!EstaLibroSeleccionado(libro))
            {
                librosSubasta.Add(libro);
            }
        }

        // Elimina un libro de la lista de pujas (Flujo Alternativo 3)
        public void EliminarLibro(LibroParaSubastarDTO libro)
        {
            librosSubasta.Remove(libro);
        }

        // Vacía la lista tras confirmar la subasta
        public void VaciarCarritoSubasta()
        {
            librosSubasta.Clear();
        }
    }
}
using AppForSEII.Web.API;

namespace AppForSEII.Web;

public class ResenaStateContainer
{
    public List<LibroParaResenarDTO> LibrosSeleccionados { get; private set; } = new();

    public void AddLibro(LibroParaResenarDTO libro)
    {
        if (!LibrosSeleccionados.Any(l => l.Id == libro.Id))
        {
            LibrosSeleccionados.Add(libro);
        }
    }

    public void RemoveLibro(LibroParaResenarDTO libro)
    {
        LibrosSeleccionados.RemoveAll(l => l.Id == libro.Id);
    }

    public void Clear()
    {
        LibrosSeleccionados.Clear();
    }
}
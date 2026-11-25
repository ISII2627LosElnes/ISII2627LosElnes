using AppForSEII.Web.API;

namespace AppForSEII.Web
{
    public class ReposicionStateContainer
    {
        // Guardamos el DTO que luego enviaremos a la API en CreateReposicion
        public ReposicionForCreateDTO Reposicion { get; private set; } = new ReposicionForCreateDTO
        {
            ReposicionItems = new List<ReposicionItemDTO>()
        };

        // Guardamos el precio unitario de cada libro por su Título para calcular el total
        private readonly Dictionary<string, double> _preciosPorLibro = new();

        public double PrecioTotal { get; private set; } = 0;

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        // Permite añadir el mismo libro varias veces incrementando su Cantidad
        public void AddLibroToReposicion(LibroParaReponerDTO libro)
        {
            _preciosPorLibro[libro.Titulo] = (double)libro.PrecioReposicion;

            var itemExistente = Reposicion.ReposicionItems
                .FirstOrDefault(ri => ri.Titulo == libro.Titulo);

            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
                Reposicion.ReposicionItems.Add(new ReposicionItemDTO
                {
                    Titulo = libro.Titulo,
                    Autor = libro.Autor,
                    Cantidad = 1
                });
            }

            ComputeTotalPrice();
            NotifyStateChanged();
        }

        // Resta 1 unidad; si llega a 0, lo quita del carrito
        public void DecreaseLibroQuantity(ReposicionItemDTO item)
        {
            if (item.Cantidad > 1)
            {
                item.Cantidad--;
            }
            else
            {
                Reposicion.ReposicionItems.Remove(item);
            }

            ComputeTotalPrice();
            NotifyStateChanged();
        }

        // Elimina el libro por completo del carrito (Flujo Alternativo 3)
        public void RemoveReposicionItem(ReposicionItemDTO item)
        {
            Reposicion.ReposicionItems.Remove(item);
            ComputeTotalPrice();
            NotifyStateChanged();
        }

        public double GetPrecioUnitario(string titulo)
        {
            return _preciosPorLibro.TryGetValue(titulo, out var precio) ? precio : 0;
        }

        public void ClearReposicionCart()
        {
            Reposicion.ReposicionItems.Clear();
            PrecioTotal = 0;
            NotifyStateChanged();
        }

        private void ComputeTotalPrice()
        {
            PrecioTotal = Reposicion.ReposicionItems
                .Sum(ri => GetPrecioUnitario(ri.Titulo) * ri.Cantidad);
        }
    }
}
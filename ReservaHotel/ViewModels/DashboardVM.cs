namespace ReservaHotel.ViewModels
{
    public class DashboardVM
    {
        public int TotalHoteles { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalReservas { get; set; }
        // Para la gráfica (JSON)
        public List<int>? ReservasPorMes { get; set; }
    }
}
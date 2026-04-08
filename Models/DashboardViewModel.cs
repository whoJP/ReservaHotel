namespace ReservaHotel.Models
{
    public class DashboardViewModel
    {
        public int TotalHoteles { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalReservas { get; set; }
        public List<ReservasPorMes> ReservasPorMes { get; set; } = new();
        public List<HotelMasReservado> HotelesMasReservados { get; set; } = new();
    }

    public class ReservasPorMes
    {
        public string Mes { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class HotelMasReservado
    {
        public string NombreHotel { get; set; } = string.Empty;
        public int CantidadReservas { get; set; }
    }
}
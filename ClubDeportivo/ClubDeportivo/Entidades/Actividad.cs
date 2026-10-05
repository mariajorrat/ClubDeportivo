namespace ClubDeportivo.Entidades
{
    /// <summary>Actividad que ofrece el club (musculación, clases grupales, natación, etc.).</summary>
    public class Actividad
    {
        public int IdActividad { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public decimal CostoNoSocio { get; set; }
        public string Horario { get; set; }
        public int CupoMaximo { get; set; }
        public int? IdProfesor { get; set; }

        /// <summary>Solo para mostrar en grillas; se completa con un JOIN en el DAO.</summary>
        public string NombreProfesor { get; set; }
    }
}

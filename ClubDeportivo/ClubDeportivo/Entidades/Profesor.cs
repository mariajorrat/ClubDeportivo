namespace ClubDeportivo.Entidades
{
    /// <summary>Profesor del club (titular o suplente). Dicta clases, controla el salón y confecciona rutinas.</summary>
    public class Profesor : Persona
    {
        public int IdProfesor { get; set; }
        public string Legajo { get; set; }
        public string Especialidad { get; set; }
        public string Tipo { get; set; } = "Titular";
        public string HorariosAsignados { get; set; }
    }
}

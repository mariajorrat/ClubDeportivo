namespace ClubDeportivo.Entidades
{
    /// <summary>Usuario del sistema, con rol para el control de acceso.</summary>
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string ContrasenaHash { get; set; }
        public string Rol { get; set; }
        public int? IdPersona { get; set; }
        public bool Activo { get; set; } = true;
    }
}

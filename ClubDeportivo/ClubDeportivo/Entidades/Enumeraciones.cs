namespace ClubDeportivo.Entidades
{
    /// <summary>Roles habilitados para iniciar sesión en el sistema.</summary>
    public enum RolUsuario
    {
        Administrador,
        Recepcionista,
        Profesor,
        Nutricionista
    }

    /// <summary>Estado de la membresía de un socio.</summary>
    public enum EstadoSocio
    {
        Activo,
        Inactivo
    }

    /// <summary>Forma de pago de la cuota mensual.</summary>
    public enum FormaPago
    {
        Efectivo,
        Tarjeta
    }

    /// <summary>Condición del profesor dentro del plantel.</summary>
    public enum TipoProfesor
    {
        Titular,
        Suplente
    }
}

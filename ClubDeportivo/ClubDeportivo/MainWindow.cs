using System.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using ClubDeportivo.Entidades;
using ClubDeportivo.Negocio;

namespace ClubDeportivo;

/// <summary>Navigation shell and functional Avalonia replacement for the MDI menu.</summary>
public sealed class MainWindow : Window
{
    private readonly Usuario usuario;
    private readonly StackPanel content = new() { Spacing = 10 };
    private TextBlock status = new() { TextWrapping = Avalonia.Media.TextWrapping.Wrap };

    public MainWindow(Usuario usuario)
    {
        this.usuario = usuario;
        Title = $"Club Deportivo - {usuario.NombreUsuario}";
        Width = 1200; Height = 760;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        var menu = new StackPanel { Spacing = 5, Width = 220 };
        BuildMenu(menu);
        content.Children.Add(new TextBlock { Text = "Seleccione un módulo", FontSize = 24 });
        content.Children.Add(new TextBlock { Text = $"Usuario: {usuario.NombreUsuario} | Rol: {usuario.Rol}" });
        Content = new DockPanel { Margin = new Thickness(18), Children =
        {
            new Border { Child = menu, Padding = new Thickness(0, 0, 18, 0) },
            new ScrollViewer { Content = content }
        }};
    }

    private void BuildMenu(Panel menu)
    {
        bool admin = usuario.Rol == "Administrador";
        bool reception = admin || usuario.Rol == "Recepcionista";
        bool teacher = admin || usuario.Rol == "Profesor";
        bool nutrition = admin || usuario.Rol == "Nutricionista";
        if (reception) { Add(menu, "Socios", ShowSocios); Add(menu, "No socios", ShowNoSocios); Add(menu, "Carnet", ShowCarnets); Add(menu, "Cuotas", ShowCuotas); Add(menu, "Vencimientos", ShowVencimientos); }
        if (admin) { Add(menu, "Profesores", ShowProfesores); Add(menu, "Actividades", ShowActividades); Add(menu, "Sueldos", ShowSueldos); }
        if (teacher) { Add(menu, "Asistencia", ShowAsistencia); Add(menu, "Rutinas", ShowRutinas); }
        if (nutrition) Add(menu, "Nutrición", ShowNutricion);
        if (admin || reception) Add(menu, "Reportes", ShowReportes);
        menu.Children.Add(new Separator { Margin = new Thickness(0, 12) });
        var exit = new Button { Content = "Salir", HorizontalContentAlignment = HorizontalAlignment.Left };
        exit.Click += (_, _) => Close(); menu.Children.Add(exit);
    }

    private static void Add(Panel menu, string caption, Action action)
    {
        var b = new Button { Content = caption, HorizontalContentAlignment = HorizontalAlignment.Left };
        b.Click += (_, _) => action(); menu.Children.Add(b);
    }
    private void Begin(string title)
    {
        content.Children.Clear();
        content.Children.Add(new TextBlock { Text = title, FontSize = 26, FontWeight = Avalonia.Media.FontWeight.Bold });
        status = new TextBlock { Foreground = Avalonia.Media.Brushes.SteelBlue, TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        content.Children.Add(status);
    }
    private Button Action(string text, Action action) { var b = new Button { Content = text, HorizontalAlignment = HorizontalAlignment.Left }; b.Click += (_, _) => Run(action); return b; }
    private void Run(Action action) { try { action(); } catch (Exception ex) { status.Text = $"Error: {ex.Message}"; } }
    private static TextBox Field(string watermark) => new() { Watermark = watermark, Width = 260 };
    private void AddFields(params Control[] controls) { foreach (var c in controls) content.Children.Add(c); }
    private static string Lines<T>(IEnumerable<T> values, Func<T, string> format) => string.Join(Environment.NewLine, values.Select(format));

    private void ShowSocios()
    {
        Begin("Socios"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        var search = Field("Nro. socio o DNI"); AddFields(search, Action("Buscar", () => { var s = new SocioNegocio().Buscar(search.Text ?? ""); output.Text = s == null ? "Socio no encontrado." : $"{s.IdSocio}: {s.NombreCompleto} | DNI {s.Dni} | Vencimiento {s.UltimoVencimiento:dd/MM/yyyy}"; }), Action("Actualizar listado", () => output.Text = Lines(new SocioNegocio().Listar(), s => $"{s.IdSocio}: {s.NombreCompleto} | {s.NroSocio} | {s.Estado}")), output);
    }
    private void ShowVencimientos()
    {
        Begin("Vencimientos diarios"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        AddFields(Action("Consultar hoy", () => output.Text = TableText(new SocioNegocio().ListarVencimientosDelDia(DateTime.Today))), output);
    }
    private void ShowActividades()
    {
        Begin("Actividades"); TextBox name = Field("Nombre"), type = Field("Tipo"), cost = Field("Costo no socio"), schedule = Field("Horario"), capacity = Field("Cupo máximo"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap }; var selected = new ComboBox { Width = 300 };
        void Load() { var list = new ActividadNegocio().Listar(); selected.ItemsSource = list; selected.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<Actividad>((a, _) => new TextBlock { Text = $"{a.IdActividad} - {a.Nombre}" }); output.Text = Lines(list, a => $"{a.IdActividad}: {a.Nombre} | {a.Tipo} | {a.Horario} | cupo {a.CupoMaximo}"); }
        selected.SelectionChanged += (_, _) => { if (selected.SelectedItem is Actividad a) { name.Text = a.Nombre; type.Text = a.Tipo; cost.Text = a.CostoNoSocio.ToString(); schedule.Text = a.Horario; capacity.Text = a.CupoMaximo.ToString(); } };
        void Save(bool update) { if (!decimal.TryParse(cost.Text, out var c) || !int.TryParse(capacity.Text, out var cap)) throw new ArgumentException("Costo y cupo deben ser numéricos."); var a = new Actividad { Nombre = name.Text ?? "", Tipo = type.Text ?? "", CostoNoSocio = c, Horario = schedule.Text ?? "", CupoMaximo = cap, IdProfesor = (selected.SelectedItem as Actividad)?.IdProfesor }; if (update) { a.IdActividad = ((Actividad)selected.SelectedItem!).IdActividad; new ActividadNegocio().ModificarActividad(a); } else new ActividadNegocio().AltaActividad(a); status.Text = "Actividad guardada."; Load(); }
        AddFields(name, type, cost, schedule, capacity, selected, Action("Listar", Load), Action("Alta", () => Save(false)), Action("Modificar seleccionada", () => Save(true)), Action("Baja seleccionada", () => { new ActividadNegocio().BajaActividad(((Actividad)selected.SelectedItem!).IdActividad); Load(); }), output);
    }
    private void ShowProfesores()
    {
        Begin("Profesores"); TextBox first = Field("Nombre"), last = Field("Apellido"), dni = Field("DNI"), legajo = Field("Legajo"), specialty = Field("Especialidad"), type = Field("Tipo (Titular/Suplente)"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap }; var selected = new ComboBox { Width = 300 };
        void Load() { var list = new ProfesorNegocio().Listar(); selected.ItemsSource = list; selected.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<Profesor>((p, _) => new TextBlock { Text = $"{p.IdProfesor} - {p.NombreCompleto}" }); output.Text = Lines(list, p => $"{p.IdProfesor}: {p.NombreCompleto} | legajo {p.Legajo} | {p.Especialidad} | {p.Tipo}"); }
        selected.SelectionChanged += (_, _) => { if (selected.SelectedItem is Profesor p) { first.Text = p.Nombre; last.Text = p.Apellido; dni.Text = p.Dni; legajo.Text = p.Legajo; specialty.Text = p.Especialidad; type.Text = p.Tipo; } };
        void Save(bool update) { var p = new Profesor { Nombre = first.Text ?? "", Apellido = last.Text ?? "", Dni = dni.Text ?? "", Legajo = legajo.Text ?? "", Especialidad = specialty.Text ?? "", Tipo = type.Text ?? "Titular" }; if (update) p.IdProfesor = ((Profesor)selected.SelectedItem!).IdProfesor; if (update) new ProfesorNegocio().ModificarProfesor(p); else new ProfesorNegocio().AltaProfesor(p); status.Text = "Profesor guardado."; Load(); }
        AddFields(first, last, dni, legajo, specialty, type, selected, Action("Listar", Load), Action("Alta", () => Save(false)), Action("Modificar seleccionada", () => Save(true)), Action("Baja seleccionada", () => { new ProfesorNegocio().BajaProfesor(((Profesor)selected.SelectedItem!).IdProfesor); Load(); }), output);
    }
    private void ShowNoSocios()
    {
        Begin("No socios / visitas"); TextBox dni = Field("DNI"), first = Field("Nombre"), last = Field("Apellido"), activity = Field("ID actividad"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        AddFields(dni, first, last, activity, Action("Listar visitas", () => output.Text = Lines(new NoSocioNegocio().Listar(), n => $"{n.IdNoSocio}: {n.NombreCompleto} | DNI {n.Dni} | {n.ActividadRealizada} | {n.FechaVisita:dd/MM/yyyy} | ${n.MontoPagado}")), Action("Registrar visita", () => { if (!int.TryParse(activity.Text, out var id)) throw new ArgumentException("ID de actividad inválido."); var n = new NoSocio { Dni = dni.Text ?? "", Nombre = first.Text ?? "", Apellido = last.Text ?? "" }; new NoSocioNegocio().RegistrarVisita(n, id); status.Text = "Visita registrada."; }), output);
    }
    private void ShowCarnets()
    {
        Begin("Carnets"); TextBox id = Field("ID socio"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        AddFields(id, Action("Listar emitidos", () => output.Text = Lines(new CarnetNegocio().Listar(), c => $"{c.IdCarnet}: socio {c.IdSocio} | emisión {c.FechaEmision:dd/MM/yyyy} | vence {c.FechaVencimiento:dd/MM/yyyy}")), Action("Emitir carnet", () => { if (!int.TryParse(id.Text, out var value)) throw new ArgumentException("ID socio inválido."); new CarnetNegocio().EmitirCarnet(value); status.Text = "Carnet emitido."; }), output);
    }
    private void ShowCuotas()
    {
        Begin("Cuotas"); TextBox id = Field("ID socio"), amount = Field("Monto"), payment = Field("Forma de pago"), installments = Field("Cuotas tarjeta"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        AddFields(id, amount, payment, installments, Action("Consultar historial", () => { if (!int.TryParse(id.Text, out var value)) throw new ArgumentException("ID socio inválido."); output.Text = Lines(new CuotaNegocio().HistorialPorSocio(value), c => $"{c.IdCuota}: {c.Mes}/{c.Anio} | ${c.Monto} | vence {c.FechaVencimiento:dd/MM/yyyy} | pago {c.FechaPago:dd/MM/yyyy}"); }), Action("Cobrar cuota", () => { if (!int.TryParse(id.Text, out var sid) || !decimal.TryParse(amount.Text, out var m)) throw new ArgumentException("Socio y monto son obligatorios."); int? parts = int.TryParse(installments.Text, out var p) ? p : null; new CuotaNegocio().CobrarCuota(sid, m, payment.Text ?? "", parts); status.Text = "Cuota cobrada."; }), output);
    }
    private void ShowAsistencia()
    {
        Begin("Asistencia de profesores"); TextBox id = Field("ID profesor"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        AddFields(id, Action("Registrar asistencia de hoy", () => { if (!int.TryParse(id.Text, out var p)) throw new ArgumentException("ID profesor inválido."); new ProfesorNegocio().RegistrarAsistencia(p); status.Text = "Asistencia registrada."; }), Action("Listar asistencia", () => { if (!int.TryParse(id.Text, out var p)) throw new ArgumentException("ID profesor inválido."); output.Text = Lines(new Datos.AsistenciaProfesorDAO().ListarPorProfesor(p), a => $"{a.Fecha:dd/MM/yyyy} - entrada {a.HoraEntrada}"); }), output);
    }
    private void ShowNutricion()
    {
        Begin("Nutrición"); TextBox id = Field("ID socio"), date = Field("Fecha (yyyy-MM-dd)"), hour = Field("Hora (HH:mm)"), notes = Field("Observaciones"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap }; var selected = new ComboBox { Width = 300 };
        void Load() { if (!int.TryParse(id.Text, out var sid)) throw new ArgumentException("ID socio inválido."); var list = new NutricionNegocio().ListarPorSocio(sid); selected.ItemsSource = list; selected.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<ConsultaNutricion>((c, _) => new TextBlock { Text = $"{c.IdConsulta} - {c.Fecha:dd/MM/yyyy} {c.Hora}" }); output.Text = Lines(list, c => $"{c.IdConsulta}: {c.Fecha:dd/MM/yyyy} {c.Hora} | {c.Observaciones}"); }
        selected.SelectionChanged += (_, _) => { if (selected.SelectedItem is ConsultaNutricion c) { date.Text = c.Fecha.ToString("yyyy-MM-dd"); hour.Text = c.Hora.ToString(@"hh\:mm"); notes.Text = c.Observaciones; } };
        void Save(bool update) { if (!int.TryParse(id.Text, out var sid) || !DateTime.TryParse(date.Text, out var day) || !TimeSpan.TryParse(hour.Text, out var time)) throw new ArgumentException("Socio, fecha y hora son obligatorios."); var c = new ConsultaNutricion { IdSocio = sid, Fecha = day, Hora = time, Observaciones = notes.Text ?? "" }; if (update) { c.IdConsulta = ((ConsultaNutricion)selected.SelectedItem!).IdConsulta; new NutricionNegocio().ModificarTurno(c); } else new NutricionNegocio().AsignarTurno(c); status.Text = "Turno guardado."; Load(); }
        AddFields(id, date, hour, notes, selected, Action("Listar por socio", Load), Action("Asignar turno", () => Save(false)), Action("Modificar turno", () => Save(true)), Action("Cancelar seleccionado", () => { new NutricionNegocio().CancelarTurno(((ConsultaNutricion)selected.SelectedItem!).IdConsulta); Load(); }), output);
    }
    private void ShowRutinas()
    {
        Begin("Rutinas"); TextBox socio = Field("ID socio"), teacher = Field("ID profesor"), description = Field("Descripción"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap }; var selected = new ComboBox { Width = 300 };
        void Load() { if (!int.TryParse(socio.Text, out var sid)) throw new ArgumentException("ID socio inválido."); var list = new RutinaNegocio().ListarPorSocio(sid); selected.ItemsSource = list; selected.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<Rutina>((r, _) => new TextBlock { Text = $"{r.IdRutina} - {r.Fecha:dd/MM/yyyy}" }); output.Text = Lines(list, r => $"{r.IdRutina}: profesor {r.IdProfesor} | {r.Fecha:dd/MM/yyyy} | {r.Descripcion}"); }
        selected.SelectionChanged += (_, _) => { if (selected.SelectedItem is Rutina r) { teacher.Text = r.IdProfesor.ToString(); description.Text = r.Descripcion; } };
        void Save(bool update) { if (!int.TryParse(socio.Text, out var sid) || !int.TryParse(teacher.Text, out var pid)) throw new ArgumentException("IDs inválidos."); var r = new Rutina { IdSocio = sid, IdProfesor = pid, Descripcion = description.Text ?? "", Fecha = DateTime.Today }; if (update) { r.IdRutina = ((Rutina)selected.SelectedItem!).IdRutina; new RutinaNegocio().ModificarRutina(r); } else new RutinaNegocio().AltaRutina(r); status.Text = "Rutina guardada."; Load(); }
        AddFields(socio, teacher, description, selected, Action("Listar por socio", Load), Action("Alta", () => Save(false)), Action("Modificar", () => Save(true)), Action("Baja seleccionada", () => { new RutinaNegocio().BajaRutina(((Rutina)selected.SelectedItem!).IdRutina); Load(); }), output);
    }
    private void ShowSueldos()
    {
        Begin("Sueldos"); TextBox professor = Field("ID profesor"), month = Field("Mes"), year = Field("Año"), amount = Field("Monto"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        AddFields(professor, month, year, amount, Action("Listar período", () => { if (!int.TryParse(month.Text, out var m) || !int.TryParse(year.Text, out var y)) throw new ArgumentException("Mes y año inválidos."); output.Text = TableText(new SueldoNegocio().ListarPorPeriodo(m, y)); }), Action("Liquidar sueldo", () => { if (!int.TryParse(professor.Text, out var p) || !int.TryParse(month.Text, out var m) || !int.TryParse(year.Text, out var y) || !decimal.TryParse(amount.Text, out var value)) throw new ArgumentException("Complete profesor, período y monto."); new SueldoNegocio().LiquidarSueldo(p, m, y, value); status.Text = "Sueldo liquidado."; }), output);
    }
    private void ShowReportes()
    {
        Begin("Reportes"); TextBox month = Field("Mes"), year = Field("Año"); var output = new TextBlock { TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        AddFields(month, year, Action("Socios activos", () => output.Text = TableText(new ReporteNegocio().SociosActivos())), Action("Socios inactivos", () => output.Text = TableText(new ReporteNegocio().SociosInactivos())), Action("Recaudación del mes", () => { if (!int.TryParse(month.Text, out var m) || !int.TryParse(year.Text, out var y)) throw new ArgumentException("Mes y año inválidos."); output.Text = TableText(new ReporteNegocio().RecaudacionDelMes(m, y)); }), Action("Carnets emitidos", () => output.Text = TableText(new ReporteNegocio().CarnetsEmitidos())), Action("Actividades con cupo", () => output.Text = TableText(new ReporteNegocio().ActividadesConCupo())), output);
    }
    private static string TableText(DataTable table) => table.Rows.Count == 0 ? "Sin resultados." : string.Join(Environment.NewLine, table.Rows.Cast<DataRow>().Select(r => string.Join(" | ", r.ItemArray)));
}

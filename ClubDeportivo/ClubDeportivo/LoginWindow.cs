using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using ClubDeportivo.Entidades;
using ClubDeportivo.Negocio;

namespace ClubDeportivo;

/// <summary>Cross-platform replacement for frmLogin.</summary>
public sealed class LoginWindow : Window
{
    private readonly TextBox usuario = new() { Watermark = "Usuario" };
    private readonly TextBox password = new() { Watermark = "Contraseña", PasswordChar = '•' };
    private readonly TextBlock mensaje = new() { Foreground = Brushes.IndianRed, TextWrapping = TextWrapping.Wrap };

    public LoginWindow()
    {
        Title = "Club Deportivo - Iniciar sesión";
        Width = 420;
        Height = 330;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        var ingresar = new Button { Content = "Ingresar", HorizontalAlignment = HorizontalAlignment.Stretch };
        ingresar.Click += Ingresar;
        password.KeyDown += (_, e) => { if (e.Key == Avalonia.Input.Key.Enter) Ingresar(this, e); };
        var salir = new Button { Content = "Salir", HorizontalAlignment = HorizontalAlignment.Stretch };
        salir.Click += (_, _) => Close();
        Content = new Border
        {
            Padding = new Thickness(36),
            Child = new StackPanel
            {
                Spacing = 14,
                Children =
                {
                    new TextBlock { Text = "Club Deportivo", FontSize = 28, FontWeight = FontWeight.Bold },
                    new TextBlock { Text = "Acceso al sistema", FontSize = 16 },
                    usuario, password, mensaje, ingresar, salir
                }
            }
        };
    }

    private void Ingresar(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(usuario.Text) || string.IsNullOrWhiteSpace(password.Text))
        {
            mensaje.Text = "Usuario y contraseña son obligatorios.";
            return;
        }
        try
        {
            Usuario? autenticado = new UsuarioNegocio().Login(usuario.Text.Trim(), password.Text);
            if (autenticado is null)
            {
                mensaje.Text = "Usuario o contraseña incorrectos.";
                password.Text = string.Empty;
                return;
            }
            if (Application.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                var main = new MainWindow(autenticado);
                desktop.MainWindow = main;
                main.Show();
                Close();
            }
        }
        catch (Exception ex)
        {
            mensaje.Text = $"No se pudo validar el usuario.{Environment.NewLine}{ex.Message}";
        }
    }
}

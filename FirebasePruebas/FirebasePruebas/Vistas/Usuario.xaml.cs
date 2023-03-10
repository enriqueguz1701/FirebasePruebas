using FirebasePruebas.Modelo;
using FirebasePruebas.ModeloVista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirebasePruebas.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Usuario : ContentPage
    {
        public Usuario()
        {
            InitializeComponent();
        }

        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {
            await InsertarUsuario();
        }

        private async Task InsertarUsuario()
        {
            MVUsuario usuarios = new MVUsuario();
            MUsuario parametros = new MUsuario();
            parametros.Usuario = UsuarioNuevo.Text;
            parametros.Pass = Pass.Text;
            parametros.Icono = "-";
            parametros.Estado = "-";

            await usuarios.InsertarUsuario(parametros);
            await DisplayAlert("Listo", "Usuario agregado", "OK");
            await MostrarUsuarios();
        }

        private async Task MostrarUsuarios()
        {
            MVUsuario funcion = new MVUsuario();
            var dato = await funcion.MostrarUsuarios();
            listaUsuarios.ItemsSource = dato;
        }
    }
}
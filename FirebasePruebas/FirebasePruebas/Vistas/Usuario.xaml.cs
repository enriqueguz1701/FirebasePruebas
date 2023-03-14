using FirebasePruebas.Modelo;
using FirebasePruebas.ModeloVista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FirebasePruebas.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Usuario : ContentPage
    {
        MediaFile archivo;
        string rutaFoto;
        string IDUsuario;
        string estado;
        string estadoImagen;
        public Usuario()
        {
            InitializeComponent();
        }

        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Comienzo a guardar el nuevo usuario");
            await InsertarUsuario();
            await SubirImagenAFirebase();
            await ActualizarIconoUsuario();
            Debug.WriteLine("Termino de guardar el nuevo usuario");
            //await MostrarUsuarios();
        }

        private async Task InsertarUsuario()
        {
            MVUsuario usuarios = new MVUsuario();
            MUsuario parametros = new MUsuario();
            parametros.Usuario = UsuarioNuevo.Text;
            parametros.Pass = Pass.Text;
            parametros.Icono = "-";
            parametros.Estado = "-";

            IDUsuario = await usuarios.InsertarUsuario(parametros);


        }

        private async Task MostrarUsuarios()
        {
            MVUsuario funcion = new MVUsuario();
            var dato = await funcion.MostrarUsuarios();
            listaUsuarios.ItemsSource = dato;
        }

        private async void btnAgregarImagen_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Busco la foto");
            await CrossMedia.Current.Initialize(); //Con esta línea se intenta acceder a la galería (si se quisiera acceder a la cámara habría que buscar cómo se hace)
            try
            {
                archivo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions() //Con esta línea se intenta acceder a las fotos del teléfono
                {
                    PhotoSize = PhotoSize.Small //Aquí se dice que el tamaño de la foto que tendrá en el teléfono será medio
                });

                if (archivo == null)
                {
                    estadoImagen = "NULO";
                    return;
                }

                estadoImagen = "LLENO";
                ImagenMovil.Source = ImageSource.FromStream(() => //El operador => es como si hiciera un método y lo llamara aquí. Si quisiera que el método tuviera parámetros irían entre los paréntesis
                {
                    var rutaImagen = archivo.GetStream(); //Obtengo la ruta de la imagen
                    return rutaImagen; //La devuelvo
                });
               

            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message); //Si ha habido algún error se mostrará aquí
            }
            Debug.WriteLine("Cargo la foto");
        }

        private async Task SubirImagenAFirebase()
        {
            MVUsuario mVUsuario = new MVUsuario();
            rutaFoto = await mVUsuario.SubirImagenes(archivo.GetStream(), IDUsuario);
        }

        private async Task ActualizarIconoUsuario()
        {
            MVUsuario mVUsuario = new MVUsuario();
            MUsuario mUsuario = new MUsuario();
            mUsuario.Icono = rutaFoto;
            mUsuario.Usuario = UsuarioNuevo.Text;
            mUsuario.Pass = Pass.Text;
            mUsuario.Estado = "Activo";
            mUsuario.Id_Usuario = IDUsuario;
            await mVUsuario.ActualizarIconoUsuario(mUsuario);
            await MostrarUsuarios();
            await DisplayAlert("Listo", "Usuario agregado", "OK");
        }

        private async void btnMostrarUsuarios_Clicked(object sender, EventArgs e)
        {
            await MostrarUsuarios();
        }

        private async void btnIcono_Clicked(object sender, EventArgs e)
        {
            IDUsuario = (sender as ImageButton).CommandParameter.ToString();
            await ObtenerDatosUsuario();
        }

        private async Task ObtenerDatosUsuario()
        {
            MVUsuario mVUsuario = new MVUsuario();
            MUsuario usuario = new MUsuario()
            {
                Id_Usuario = IDUsuario
            };
            var dato = await mVUsuario.ObtenerDatosUsuario(usuario);
            foreach (var item in dato)
            {
                UsuarioNuevo.Text = item.Usuario;
                Pass.Text = item.Pass;
                ImagenMovil.Source = item.Icono;
                rutaFoto = item.Icono;
                estado = item.Estado;
            }
        }

        private async void btnEliminarUsuario_Clicked(object sender, EventArgs e)
        {
            await EliminarUsuario();
            await EliminarImagenUsuario();
            await MostrarUsuarios();
        }

        private async Task EliminarUsuario()
        {
            MVUsuario mVUsuario = new MVUsuario();
            MUsuario mUsuario = new MUsuario()
            {
                Id_Usuario = IDUsuario
            };
            await mVUsuario.EliminarUsuario(mUsuario);

            
            /*UsuarioNuevo.Text = "";
            Pass.Text = "";
            ImagenMovil.Source = null;*/
        }

        private async Task EliminarImagenUsuario()
        {
            MVUsuario mVUsuario = new MVUsuario();
            await mVUsuario.EliminarImagenUsuario(IDUsuario);
        }

        private async void btnEditarUsuario_Clicked(object sender, EventArgs e)
        {
            if(estadoImagen == "LLENO")
            {
                await EliminarImagenUsuario();
                await SubirImagenAFirebase();
            }
            await ActualizarIconoUsuario();
        }
    }
}
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

namespace FirebasePruebas.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Usuario : ContentPage
    {
        MediaFile archivo;
        string rutaFoto;
        string IDUsuario;
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
            await MostrarUsuarios();
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
            await DisplayAlert("Listo", "Usuario agregado", "OK");
            
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
                }) ; 

                if(archivo == null)
                    return;
                else
                {
                    ImagenMovil.Source = ImageSource.FromStream(() => //El operador => es como si hiciera un método y lo llamara aquí. Si quisiera que el método tuviera parámetros irían entre los paréntesis
                    {
                        var rutaImagen = archivo.GetStream(); //Obtengo la ruta de la imagen
                        return rutaImagen; //La devuelvo
                    });
                    Debug.WriteLine("Imagen: " + archivo.AlbumPath);
                }
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
            mUsuario.Id_Usuario = IDUsuario;
            await mVUsuario.ActualizarIconoUsuario(mUsuario);
        }
    }
}
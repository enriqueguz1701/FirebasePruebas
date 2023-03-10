using Firebase.Database.Query;
using Firebase.Storage;
using FirebasePruebas.Modelo;
using FirebasePruebas.Servicios;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebasePruebas.ModeloVista
{
    public class MVUsuario
    {
        List<MUsuario> Usuario = new List<MUsuario>();
        string rutaImagenUsuario;
        string IDUsuario;

        public async Task<List<MUsuario>> MostrarUsuarios()
        {
            var datos = await ConexionFirebase.firebase.
                Child("Usuarios"). //Obtiene los datos de la tabla Usuarios
                OrderByKey(). //Los ordena por una clave
                OnceAsync<MUsuario>(); //Indica el tipo de dato en el que se van a guardar cada una de las filas de la tabla

            foreach (var dato in datos)
            {
                MUsuario parametros = new MUsuario();
                parametros.Id_Usuario = dato.Key; //Se coge la clave de la fila seleccionada
                parametros.Usuario = dato.Object.Usuario; //Se cogen los diferentes datos de los usuarios
                parametros.Pass = dato.Object.Pass;
                parametros.Icono = dato.Object.Icono;
                parametros.Estado = dato.Object.Estado;
                Usuario.Add(parametros); //Se añade el usuario de la fila a la lista
            }

            return Usuario;
        }

        public async Task<string> InsertarUsuario(MUsuario usuario)
        {
            var nuevoUsuario = await ConexionFirebase.firebase.
                Child("Usuarios"). //Se establece la conexión con la base de datos
                PostAsync(new MUsuario() //Se añade el nuevo usuario a la tabla
                { //Como se abren las llave se ponen los valores de los diferentes atributos de la clase MUsuarios
                    Usuario = usuario.Usuario,
                    Pass = usuario.Pass,
                    Icono = usuario.Icono,
                    Estado = usuario.Estado
                });

            IDUsuario = nuevoUsuario.Key;
            return IDUsuario;
        }

        /// <summary>
        /// Método para subir imágenes a la nube de firebase
        /// </summary>
        /// <param name="ImagenStream">Este parámetro sirve para indicar la ruta de la imagen que quiero subir</param>
        /// <param name="IDUsuario">ID del usuario</param>
        /// <returns></returns>
        public async Task<string> SubirImagenes(Stream ImagenStream,string IDUsuario)
        {
            try
            {
                var datosAlmacenamiento = await new FirebaseStorage("usuarios-ea685.appspot.com") //En esta parte del código establezco la conexión con la nube
                .Child("Usuarios") //Aquí estoy indicando de todas las carpetas que tengo declaradas en la nube cual quiero
                .Child(IDUsuario + ".jpg") //Aquí estoy diciendo a qué usuario voy a subir la imagen al final se añade el jpg para indicar la extensión de la imagen
                .PutAsync(ImagenStream); //Con este método indico la ruta de la imagen que quiero subir a la nube

                rutaImagenUsuario = datosAlmacenamiento; //En datosAlmacenamiento queda guardada la dirección web de la imagen del usuario
                Debug.WriteLine("Terminé de guardar una foto");
                Debug.WriteLine("Ruta imagen: " + rutaImagenUsuario);
                return rutaImagenUsuario;
            } catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return "foto";
            }
            
        }
    
        public async Task ActualizarIconoUsuario(MUsuario usuario)
        {
            var obtenerUsuarios = (await ConexionFirebase.firebase //Nos conectamos a la base de datos
                .Child("Usuarios") //Digo que quiero la tabla Usuarios
                .OnceAsync<MUsuario>()) //Obtengo los datos de la tabla y los conviertos a datos de tipo MUsuario
                .Where(a => a.Key == usuario.Id_Usuario).FirstOrDefault();  //Aquí el where, es decir, de todos los datos que tengo solo quiero el que tenga como Key el ID_Usuario

            await ConexionFirebase.firebase //Realizo la conexión con la base de datos
                .Child("Usuarios") //De toda la base de datos solo quiero la tabla Usuarios
                .Child(obtenerUsuarios.Key) //De todas las filas de la tabla Usuarios solo quiero la que tiene el Key del usuario a modificar
                .PutAsync(new MUsuario()
                {
                    Icono = usuario.Icono
                }); //Con el método Put se modifica el usuario
        }
    }
}

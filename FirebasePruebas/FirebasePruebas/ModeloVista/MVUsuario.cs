using Firebase.Database.Query;
using FirebasePruebas.Modelo;
using FirebasePruebas.Servicios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FirebasePruebas.ModeloVista
{
    public class MVUsuario
    {
        List<MUsuario> Usuario = new List<MUsuario>();

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

        public async Task InsertarUsuario(MUsuario usuario)
        {
            await ConexionFirebase.firebase.
                Child("Usuarios"). //Se establece la conexión con la base de datos
                PostAsync(new MUsuario() //Se añade el nuevo usuario a la tabla
                { //Como se abren las llave se ponen los valores de los diferentes atributos de la clase MUsuarios
                    Usuario = usuario.Usuario,
                    Pass = usuario.Pass,
                    Icono = usuario.Icono,
                    Estado = usuario.Estado
                });
        }
    }
}

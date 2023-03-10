using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace FirebasePruebas.Servicios
{
    public class ConexionFirebase
    {
        public static FirebaseClient firebase = new FirebaseClient("https://usuarios-ea685-default-rtdb.europe-west1.firebasedatabase.app/");
    }
}

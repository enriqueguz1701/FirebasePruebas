using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace FirebasePruebas.Servicios
{
    internal class ConexionFirebase
    {
        public static FirebaseClient firebase = new FirebaseClient("https://usuarios-67116-default-rtdb.europe-west1.firebasedatabase.app/");
    }
}

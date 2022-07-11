using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using AlbumOnline_HJMB.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace AlbumOnline_HJMB
{
    /// <summary>
    /// Descripción breve de AlbumOnlineWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class AlbumOnlineWS : System.Web.Services.WebService
    {

        AlbumOnlineModelContainer db = new AlbumOnlineModelContainer();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hola a todos";
        }

        [WebMethod]
        public bool AddImg(byte[] img, int tipo, string user)
        {
            Photo item = new Photo();
            item.Img = img;
            item.TipoId = tipo;
            item.User = user;
            item.Fecha = DateTime.Now.ToShortDateString();
            item.Status = "A";
            db.Photos.Add(item);
            if (db.SaveChanges() > 0)
            {
                return true;
            }
            else
                return false;
        }


        [WebMethod]
        public List<SWPhoto> get_photos(string user)
        {
            return db.Photos.Where(x => x.User == user && x.Status == "A").Select(x => new SWPhoto()
            {
                Id = x.Id,
                Img = x.Img,
                User = x.User,
                Fecha = x.Fecha,
                TipoId = x.TipoId,
                Status = x.Status
            }).ToList();
        }
        public List<SWPhoto> get_photos_by_tipo(int tipo)
        {
            return db.Photos.Where(x => x.TipoId == tipo && x.Status == "A").Select(x => new SWPhoto()
            {
                Id = x.Id,
                Img = x.Img,
                User = x.User,
                Fecha = x.Fecha,
                TipoId = x.TipoId,
                Status = x.Status
            }).ToList();
        }

        [WebMethod]
        public bool IniciarSesion(string user, string pass)
        {
            // No cuenta los errores de inicio de sesión para el bloqueo de la cuenta
            // Para permitir que los errores de contraseña desencadenen el bloqueo de la cuenta, cambie a shouldLockout: true
            // var result = SignInManager.PasswordSignInAsync(user, pass, false, shouldLockout:false);
            var result = HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>().PasswordSignIn(user, pass, false, false);
            if (result == SignInStatus.Success)
                return true;
            else
                return false;
        }

        [WebMethod]
        public bool CrearUsuario(String email, String contraseña)
        {

            ApplicationDbContext context = new ApplicationDbContext();
            var UserMNG = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RolMNG = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            bool band = false;

            //Tomamos los datos de los campos que llena el usuario
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };
            string userPWD = contraseña;
            var chkUser = UserMNG.Create(user, userPWD);

            //Agregamos el usuario a su nuevo rol
            if (chkUser.Succeeded)
            {
                var result1 = UserMNG.AddToRole(user.Id, "Freemium");
                band = true;
            }
            else band = false;

            //Agregamos una persona
            return band;
        }
        public partial class SWPhoto
        {
            public int Id { get; set; }
            public byte[] Img { get; set; }
            public string User { get; set; }
            public string Fecha { get; set; }
            public int TipoId { get; set; }
            public string Status { get; set; }
        }
    }
}

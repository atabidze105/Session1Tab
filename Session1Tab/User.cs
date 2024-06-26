using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Session1Tab
{
    internal class User //Пользователь
    {
        private string _name; //логин
        private string _password; //пароль
        private bool _isAdmin; //булево админ
        private bool _isGuest; //булево гость
        private List<CartItem> _UserCart = []; //Корзина (у каждого пользователя своя)

        public User(string name, string password, bool admin, bool guest)
        {
            _name = name;
            _password = password;
            _isAdmin = admin;
            _isGuest = guest;
        }

        public string uName
        {
            get { return _name; }
            set { _name = value; }
        }

        public string uPassword
        {
            get { return _password; }
            set { _password = value; }
        }

        public bool isAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; }
        }

        public bool isGuest
        {
            get { return _isGuest; }
            set { _isGuest = value; }
        }

        public List<CartItem> UserCart
        {
            get { return _UserCart; }
            set { _UserCart = value; }
        }
    }
}

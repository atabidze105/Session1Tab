using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session1Tab
{
    internal class Product //Товар
    {
        private string _productName; //Название
        private int _productId; //АЙдишник
        private int _cathegoryId; //Идентификатор категории
        private string _productDescription; //Описание
        private string _productSupplier; //Поставщик
        private string? _productImageSource; //Источник картинки, может быть не указан. Включает в себя только название файла с расширением
        private double _productPrice; //Цена
        private int _productQuantity; //Количество
        private int _measurement; //единица измерения


        public Product(string productName, int productId, int cathegory, string productDescription, string productSupplier, string productImageSource, double productPrice, int productQuantity, int measurement)
        {
            _productName = productName;
            _productId = productId;
            _cathegoryId = cathegory;
            _productDescription = productDescription;
            _productSupplier = productSupplier;
            _productImageSource = productImageSource;
            _productPrice = productPrice;
            _productQuantity = productQuantity;
            _measurement = measurement;
        }

        public string pName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        public int pId
        {
            get { return _productId; }
            set { _productId = value; }
        }

        public int pCathegory
        {
            get { return _cathegoryId; }
            set { _cathegoryId = value; }
        }

        public string pDescription
        {
            get { return _productDescription; }
            set { _productDescription = value; }
        }

        public string pSupplier
        {
            get { return _productSupplier; }
            set { _productSupplier = value; }
        }

        public double pPrice
        {
            get { return _productPrice; }
            set { _productPrice = value; }
        }

        public int pQuantity
        {
            get { return _productQuantity; }
            set { _productQuantity = value; }
        }

        public int pMeasurement
        {
            get { return _measurement; }
            set { _measurement = value; }
        }

        public string? pImageSource
        {
            get { return _productImageSource; }
            set { _productImageSource = value; }
        }

        public Bitmap? pImage => pImageSource != null ? new Bitmap($"Assets/{pImageSource}") : new Bitmap("Assets/placeholder.jpg"); //Если источник не указан, используется заглушка
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using negocio;
using dominio;
using System.IO;
using System.Configuration;

namespace Vista
{
    public partial class FrmAltaArticulo : Form
    {
        private Articulo articulo = null;
        private OpenFileDialog archivo =null;
        public FrmAltaArticulo()
        {
            InitializeComponent();
        }

        public FrmAltaArticulo(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modificar Articulo";
        }
        private void FrmAltaArticulo_Load(object sender, EventArgs e)
        {
            MarcaNegocio marcaNegocio= new MarcaNegocio();
            CategoriaNegocio categoriaNegocio= new CategoriaNegocio();

            try
            {
                cboMarca.DataSource = marcaNegocio.listar();
                cboMarca.ValueMember = "Id";
                cboMarca.DisplayMember = "Descripcion";
                cboCategoria.DataSource = categoriaNegocio.listarCat();
                cboCategoria.ValueMember = "Id";
                cboCategoria.DisplayMember = "Descripcion";

                if (articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtUrlImagen.Text = articulo.ImagenUrl;
                    txtPrecio.Text = articulo.Precio.ToString("N2");
                    cargarImagen(articulo.ImagenUrl);
                    cboMarca.SelectedValue = articulo.Marca.Id;
                    cboCategoria.SelectedValue = articulo.Categoria.Id;

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulo.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxArticulo.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }

        private void btnAceptarArt_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();

            try
            {
                if (articulo == null)
                    articulo = new Articulo();
                //if (camposVacios())
                //    return;
                if (formatoCodigo(articulo.Codigo))
                    return;
                if (camposVacios())
                {
                    return;
                }

                
                
                articulo.Codigo = txtCodigo.Text;  
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion= txtDescripcion.Text;
                articulo.ImagenUrl = txtUrlImagen.Text;
                articulo.Precio = decimal.Parse(txtPrecio.Text);
                articulo.Marca = (Marca)cboMarca.SelectedItem;
                articulo.Categoria = (Categoria)cboCategoria.SelectedItem;

                if (articulo.Id != 0)
                {
                    negocio.modificarArticulo(articulo);
                    MessageBox.Show("Modificado Exitosamente! ");
                }
                else
                {
                    negocio.agregarArticulo(articulo);
                    MessageBox.Show("Agregado Exitosamente! ");
                }

                if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP")))
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);


                Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            
        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
        }

        private void btnAddImagenArt_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";
            if (archivo.ShowDialog() == DialogResult.OK)
            {
                txtUrlImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);                
            }

        }

        private void btnCancelarArt_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region//VALIDACIONES ALTA/MODIFICACION ARTICULO

        private bool formatoCodigo(string codigo)
        {

            codigo = txtCodigo.Text;
            if (codigo.Length > 3 || codigo.Length < 3)
            {
                MessageBox.Show("El codigo del articulo no debe tener mas o menos de 3 caracteres!");
                return true;
            }      

            return false;          
        }

        private bool camposVacios()
        {
            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                MessageBox.Show("El campo nombre no debe estar vacio");
                return true;
            }
            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                MessageBox.Show("El campo descripcion no debe estar vacio");
                return true;
            }
            if (string.IsNullOrEmpty(txtUrlImagen.Text))
            {
                MessageBox.Show("El campo UrlImagen no debe estar vacio");
                return true;
            }          
            if(string.IsNullOrEmpty(txtPrecio.Text))
            {
                MessageBox.Show("El campo Precio no debe estar vacio");
                return true;
            }
            if (!(soloNumeros(txtPrecio.Text)))
            {
                MessageBox.Show("El campo precio debe ser solo numerico");
                return true;
            }
            if (cboMarca.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar Marca del producto");
                return true;
            }
            if (cboCategoria.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar categoria del producto");
                return true;
            }

            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter) || !(char.IsDigit(caracter) || char.IsLetter(caracter))))
                    return false;
            }
            return true;
        }

        #endregion
    }
}

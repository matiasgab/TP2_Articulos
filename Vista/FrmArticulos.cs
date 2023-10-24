using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;
using Microsoft.VisualBasic;

namespace Vista
{
    public partial class FrmArticulos : Form
    {

        private List<Articulo> listaArticulo = new List<Articulo>(); 
        public FrmArticulos()
        {
            InitializeComponent();
        }

        private void FrmArticulos_Load(object sender, EventArgs e)
        {
            cargar();
            CboCampo.Items.Add("Codigo");
            CboCampo.Items.Add("Nombre");
            CboCampo.Items.Add("Marca");
            CboCampo.Items.Add("Categoria");
            CboCampo.Items.Add("Precio");

        }

        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                listaArticulo = negocio.listar();              
                dgvArticulos.DataSource = listaArticulo;                
                ocultarColumnas();
                cargarImagen(listaArticulo[0].ImagenUrl);
                //formatea en dos decimales los valores de la columna PRECIO
                DataGridViewColumn columnaPrecio = dgvArticulos.Columns[7];               
                columnaPrecio.DefaultCellStyle.Format = "N2";

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            dgvArticulos.Columns["ImagenUrl"].Visible = false;
            dgvArticulos.Columns["Id"].Visible = false;
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

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.ImagenUrl);
            }


        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {

            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (validarFiltros())
                    return;
                string campo = CboCampo.SelectedItem.ToString();
                string criterio = CboCriterio.SelectedItem.ToString();
                string filtro = txtFiltro.Text;
                dgvArticulos.DataSource = negocio.filtrar(campo,criterio,filtro);   
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }


        }

        private void CboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {

            string opcion = CboCampo.SelectedItem.ToString();
            if (opcion == "Codigo" || opcion == "Nombre" || opcion == "Marca"  || opcion == "Categoria")
            {
                muestraCriterios();
            }
            else
            {
                muestraCriteriosNumericos();
            }


        }

        private void muestraCriterios()
        {
            CboCriterio.Items.Clear();
            CboCriterio.Items.Add("Comienza con");
            CboCriterio.Items.Add("Termina con");
            CboCriterio.Items.Add("Contiene");
        }
        private void muestraCriteriosNumericos()
        {
            CboCriterio.Items.Clear();
            CboCriterio.Items.Add("Mayor a");
            CboCriterio.Items.Add("Menor a");
            CboCriterio.Items.Add("Igual a");
        }


        private void btnSalirApp_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FrmAltaArticulo alta = new FrmAltaArticulo();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

            FrmAltaArticulo modificar = new FrmAltaArticulo(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            eliminaArticulo();
        }

        private void eliminaArticulo()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;

            try
            {
                DialogResult respuesta = MessageBox.Show("¿Una vez eliminado el articulo no se podra recuperar, desea continuar?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;                  
                    negocio.eliminar(seleccionado.Id);

                    cargar();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #region//VALIDACIONES
        private bool validarFiltros()
        {

            if (CboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar por que campo realizar la busqueda!");
                return true;
            }
            if (CboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar por que criterio realizar la busqueda!");
                return true;
            }
            if (CboCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltro.Text))
                {
                    MessageBox.Show("Debes cargar un numero en campo de busqueda...");
                    return true;
                }
                if (!(soloNumeros(txtFiltro.Text)))
                {
                    MessageBox.Show("Solo nros para filtrar por un campo numérico...");
                    return true;
                }

            }
            if(CboCampo.SelectedItem.ToString() != "Precio")
            {
                if (!(soloLetras(txtFiltro.Text)))
                {
                    MessageBox.Show("Debe ingresar una palabra para buscar por este tipo de campo...");
                    return true;
                }
                
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
        private bool soloLetras(string cadena)
        {
            foreach  (char caracter in cadena)
            {
                if (!(char.IsLetter(caracter)))
                    return false;
               
            }
            return true;
        }
        #endregion

        private void btnDetalleArticulo_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

            FrmDetalleArticulo modificar = new FrmDetalleArticulo(seleccionado);
            modificar.ShowDialog();
            
            cargar();

        }
    }
}

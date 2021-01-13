using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Agenda
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HabilitaCampos(false);
            ListarContatos();
            AlterarBotoes("Carregar");
        }

        private void ListarContatos()
        {
            try
            {
                using (var agendaCtx = new agendaEntities())
                {
                    var consulta = agendaCtx.contatos;
                    dgDados.ItemsSource = consulta.ToList();
                    lblCountContatos.Content = dgDados.ItemsSource.OfType<contato>().Count();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha ao listar contatos. Erro: {ex.Message}");
            }
        }

        private void btnInserirAlterar_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            AlterarBotoes(btn.Content.ToString());
            HabilitaCampos(true);
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            using (var agendaCtx = new agendaEntities())
            {
                var contatoEdit = agendaCtx.contatos.Find(Convert.ToInt32(txtId.Text));
                agendaCtx.contatos.Remove(contatoEdit);
                agendaCtx.SaveChanges();
            }

            ListarContatos();
            AlterarBotoes("Carregar");
            HabilitaCampos(false);
            ResetCampos();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if(!ValidarCampos())
            {
                MessageBox.Show("Preencha os campos obrigatórios");
                return;
            }

            if(string.IsNullOrWhiteSpace(txtId.Text))
            {
                using (var agendaCtx = new agendaEntities())
                {
                    var contato = new contato();
                    contato.nome = txtNome.Text;
                    contato.email = txtEmail.Text;
                    contato.telefone = txtTelefone.Text;

                    agendaCtx.contatos.Add(contato);
                    agendaCtx.SaveChanges();
                }
            }
            else
            {
                using (var agendaCtx = new agendaEntities())
                {
                    var contatoEdit = agendaCtx.contatos.Find(Convert.ToInt32(txtId.Text));
                    contatoEdit.nome = txtNome.Text;
                    contatoEdit.email = txtEmail.Text;
                    contatoEdit.telefone = txtTelefone.Text;
                    agendaCtx.SaveChanges();
                }
            }

            Reset();
        }

        private void btnLocalizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!string.IsNullOrWhiteSpace(txtId.Text))
                {
                    using (var agendaCtx = new agendaEntities())
                    {
                        var contato = agendaCtx.contatos.Find(Convert.ToInt32(txtId.Text));
                        dgDados.ItemsSource = new contato[] { contato };
                    }
                }
                else if (!string.IsNullOrWhiteSpace(txtNome.Text))
                {
                    using (var agendaCtx = new agendaEntities())
                    {
                        dgDados.ItemsSource = agendaCtx.contatos.Where(x => x.nome.Contains(txtNome.Text)).ToList();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    using (var agendaCtx = new agendaEntities())
                    {
                        dgDados.ItemsSource = agendaCtx.contatos.Where(x => x.email.Contains(txtEmail.Text)).ToList();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(txtTelefone.Text))
                {
                    using (var agendaCtx = new agendaEntities())
                    {
                        dgDados.ItemsSource = agendaCtx.contatos.Where(x => x.telefone.Contains(txtTelefone.Text)).ToList();
                    }
                }

                lblCountContatos.Content = dgDados.ItemsSource.OfType<contato>().Count();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha ao buscar registro. Erro: {ex.Message}");
            }
        }

        private void HabilitaCampos(bool habilitar = true)
        {
            txtNome.IsEnabled = habilitar;
            txtEmail.IsEnabled = habilitar;
            txtTelefone.IsEnabled = habilitar;
        }

        private void AlterarBotoes(string operacao)
        {
            btnInserir.IsEnabled = false;
            btnAlterar.IsEnabled = false;
            btnExcluir.IsEnabled = false;
            btnLocalizar.IsEnabled = false;
            btnSalvar.IsEnabled = false;
            btnCancelar.IsEnabled = false;

            if(operacao == "Carregar")
            {
                btnInserir.IsEnabled = true;
                btnLocalizar.IsEnabled = true;
            }
            if (operacao == "Inserir")
            {
                btnSalvar.IsEnabled = true;
                btnCancelar.IsEnabled = true;
                HabilitaCampos();
            }
            if (operacao == "Alterar")
            {
                btnAlterar.IsEnabled = true;
                btnExcluir.IsEnabled = true;
                HabilitaCampos();
            }
        }

        private void ResetCampos()
        {
            txtId.Clear();
            txtNome.Clear();
            txtEmail.Clear();
            txtTelefone.Clear();
        }


        private void dgDados_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(dgDados.SelectedItem != null)
            {
                var contato = (contato)dgDados.SelectedItem;
                txtId.Text = contato.id.ToString();
                txtNome.Text = contato.nome;
                txtEmail.Text = contato.email;
                txtTelefone.Text = contato.telefone;

                AlterarBotoes("Alterar");
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private bool ValidarCampos()
        {
            if(string.IsNullOrWhiteSpace(txtNome.Text))
            {
                txtNome.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                txtEmail.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTelefone.Text))
            {
                txtTelefone.Focus();
                return false;
            }

            return true;
        }

        private void Reset()
        {
            ListarContatos();
            AlterarBotoes("Carregar");
            HabilitaCampos(false);
            ResetCampos();
        }

    }

    public enum Operacao
    {
        Inserir,
        Alterar,
        Excluir,
        Localizar,
        Salvar,
        Cancelar,
        Carregar
    }
}

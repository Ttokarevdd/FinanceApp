using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace FinanceApp
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }
    }

    public class FinanceData
    {
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public decimal Balance { get; set; } = 0;
    }

    public class MainForm : Form
    {
        private FinanceData data;
        private string dataFilePath = "finance_data.json";

        private Label lblTitle;
        private Label lblAuthor;
        private Label lblBalance;
        private Label lblAmount;
        private Label lblCategory;
        private Label lblFilter;
        private TextBox txtAmount;
        private TextBox txtCategory;
        private Button btnAddIncome;
        private Button btnAddExpense;
        private Button btnDelete;
        private Button btnFilter;
        private Button btnClearFilter;
        private Button btnRefresh;
        private ComboBox cmbFilterCategory;
        private DataGridView dgvTransactions;

        public MainForm()
        {
            data = new FinanceData();
            CreateControls();
            SetupForm();
            LoadData();
            RefreshAll();
        }

        private void CreateControls()
        {
            lblTitle = new Label();
            lblAuthor = new Label();
            lblBalance = new Label();
            lblAmount = new Label();
            lblCategory = new Label();
            lblFilter = new Label();
            txtAmount = new TextBox();
            txtCategory = new TextBox();
            btnAddIncome = new Button();
            btnAddExpense = new Button();
            btnDelete = new Button();
            btnFilter = new Button();
            btnClearFilter = new Button();
            btnRefresh = new Button();
            cmbFilterCategory = new ComboBox();
            dgvTransactions = new DataGridView();
        }

        private void SetupForm()
        {
            Text = "FINANCE TRACKING SYSTEM";
            Text = "Author: Tokarev Dmitry";
            Width = 750;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "FINANCE TRACKING SYSTEM";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Size = new Size(400, 30);

            lblAuthor.Text = "Author: Tokarev Dmitry";
            lblAuthor.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblAuthor.Location = new Point(20, 45);
            lblAuthor.Size = new Size(300, 20);

            lblBalance.Text = "Balance: 0.00 USD";
            lblBalance.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblBalance.ForeColor = Color.DarkGreen;
            lblBalance.Location = new Point(20, 75);
            lblBalance.Size = new Size(400, 25);

            lblAmount.Text = "Amount:";
            lblAmount.Location = new Point(20, 120);
            lblAmount.Size = new Size(80, 20);

            txtAmount.Location = new Point(100, 117);
            txtAmount.Size = new Size(120, 23);

            lblCategory.Text = "Category:";
            lblCategory.Location = new Point(240, 120);
            lblCategory.Size = new Size(80, 20);

            txtCategory.Location = new Point(320, 117);
            txtCategory.Size = new Size(120, 23);

            btnAddIncome.Text = "Add Income";
            btnAddIncome.Location = new Point(460, 115);
            btnAddIncome.Size = new Size(120, 28);
            btnAddIncome.BackColor = Color.LightGreen;
            btnAddIncome.Click += BtnAddIncome_Click;

            btnAddExpense.Text = "Add Expense";
            btnAddExpense.Location = new Point(590, 115);
            btnAddExpense.Size = new Size(120, 28);
            btnAddExpense.BackColor = Color.LightCoral;
            btnAddExpense.Click += BtnAddExpense_Click;

            btnDelete.Text = "Delete";
            btnDelete.Location = new Point(20, 160);
            btnDelete.Size = new Size(100, 28);
            btnDelete.BackColor = Color.LightGray;
            btnDelete.Click += BtnDelete_Click;

            lblFilter.Text = "Filter:";
            lblFilter.Location = new Point(140, 165);
            lblFilter.Size = new Size(50, 20);

            cmbFilterCategory.Location = new Point(190, 162);
            cmbFilterCategory.Size = new Size(150, 23);
            cmbFilterCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            btnFilter.Text = "Apply";
            btnFilter.Location = new Point(350, 160);
            btnFilter.Size = new Size(90, 28);
            btnFilter.Click += BtnFilter_Click;

            btnClearFilter.Text = "Reset";
            btnClearFilter.Location = new Point(450, 160);
            btnClearFilter.Size = new Size(90, 28);
            btnClearFilter.Click += BtnClearFilter_Click;

            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(570, 160);
            btnRefresh.Size = new Size(140, 28);
            btnRefresh.BackColor = Color.LightBlue;
            btnRefresh.Click += BtnRefresh_Click;

            dgvTransactions.Location = new Point(20, 200);
            dgvTransactions.Size = new Size(690, 320);
            dgvTransactions.ReadOnly = true;
            dgvTransactions.AllowUserToAddRows = false;
            dgvTransactions.AllowUserToDeleteRows = false;
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransactions.MultiSelect = false;

            Controls.Add(lblTitle);
            Controls.Add(lblAuthor);
            Controls.Add(lblBalance);
            Controls.Add(lblAmount);
            Controls.Add(txtAmount);
            Controls.Add(lblCategory);
            Controls.Add(txtCategory);
            Controls.Add(btnAddIncome);
            Controls.Add(btnAddExpense);
            Controls.Add(btnDelete);
            Controls.Add(lblFilter);
            Controls.Add(cmbFilterCategory);
            Controls.Add(btnFilter);
            Controls.Add(btnClearFilter);
            Controls.Add(btnRefresh);
            Controls.Add(dgvTransactions);
        }

        private void LoadData()
        {
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                data = JsonSerializer.Deserialize<FinanceData>(json) ?? new FinanceData();
            }
            else
            {
                data = new FinanceData();
                SaveData();
            }
        }

        private void SaveData()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(dataFilePath, json);
        }

        private int GenerateId()
        {
            if (data.Transactions.Count > 0)
                return data.Transactions.Max(t => t.Id) + 1;
            return 1;
        }

        private void AddTransaction(string type, decimal amount, string category)
        {
            Transaction transaction = new Transaction
            {
                Id = GenerateId(),
                Type = type,
                Amount = amount,
                Category = category,
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            data.Transactions.Add(transaction);
            if (type == "income")
                data.Balance += amount;
            else if (type == "expense")
                data.Balance -= amount;
            SaveData();
            RefreshAll();
            MessageBox.Show($"Transaction added! ID: {transaction.Id}", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeleteTransaction(int id)
        {
            Transaction transaction = data.Transactions.Find(t => t.Id == id);
            if (transaction != null)
            {
                if (transaction.Type == "income")
                    data.Balance -= transaction.Amount;
                else if (transaction.Type == "expense")
                    data.Balance += transaction.Amount;
                data.Transactions.Remove(transaction);
                SaveData();
                RefreshAll();
                MessageBox.Show($"Transaction ID {id} deleted.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Transaction ID {id} not found.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshAll()
        {
            lblBalance.Text = $"Current balance: {data.Balance:F2} USD";
            lblBalance.ForeColor = data.Balance >= 0 ? Color.DarkGreen : Color.DarkRed;
            UpdateFilterComboBox();
            UpdateTransactionsGrid();
        }

        private void UpdateFilterComboBox()
        {
            string selected = cmbFilterCategory.SelectedItem?.ToString();
            var categories = data.Transactions.Select(t => t.Category).Distinct().ToList();
            categories.Sort();
            cmbFilterCategory.Items.Clear();
            cmbFilterCategory.Items.Add("All categories");
            foreach (string cat in categories)
                cmbFilterCategory.Items.Add(cat);
            if (selected != null && cmbFilterCategory.Items.Contains(selected))
                cmbFilterCategory.SelectedItem = selected;
            else
                cmbFilterCategory.SelectedIndex = 0;
        }

        private void UpdateTransactionsGrid(string filterCategory = null)
        {
            List<Transaction> displayList;
            if (string.IsNullOrEmpty(filterCategory) || filterCategory == "All categories")
                displayList = data.Transactions;
            else
                displayList = data.Transactions
                    .Where(t => t.Category.Equals(filterCategory, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            dgvTransactions.DataSource = null;
            dgvTransactions.DataSource = displayList.Select(t => new
            {
                t.Id,
                Type = t.Type == "income" ? "Income" : "Expense",
                Amount = t.Amount.ToString("F2"),
                Category = t.Category,
                Date = t.Date
            }).ToList();
        }

        private void BtnAddIncome_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Enter a valid amount!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string category = txtCategory.Text.Trim();
            if (string.IsNullOrEmpty(category))
            {
                MessageBox.Show("Enter a category!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AddTransaction("income", amount, category);
            txtAmount.Clear();
            txtCategory.Clear();
        }

        private void BtnAddExpense_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Enter a valid amount!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string category = txtCategory.Text.Trim();
            if (string.IsNullOrEmpty(category))
            {
                MessageBox.Show("Enter a category!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AddTransaction("expense", amount, category);
            txtAmount.Clear();
            txtCategory.Clear();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTransactions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a transaction to delete!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = (int)dgvTransactions.SelectedRows[0].Cells[0].Value;
            DialogResult result = MessageBox.Show($"Delete transaction ID {id}?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                DeleteTransaction(id);
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            string selected = cmbFilterCategory.SelectedItem?.ToString();
            UpdateTransactionsGrid(selected);
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            cmbFilterCategory.SelectedIndex = 0;
            UpdateTransactionsGrid();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
            RefreshAll();
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
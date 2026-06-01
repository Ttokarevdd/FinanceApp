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
        private string currentFilter = null;

        private List<string> incomeCategories = new List<string>
        {
            "Зарплата",
            "Подработка",
            "Подарок",
            "Продажа",
            "Инвестиции",
            "Прочее"
        };

        private List<string> expenseCategories = new List<string>
        {
            "Продукты",
            "Транспорт",
            "Жильё",
            "Развлечения",
            "Здоровье",
            "Одежда",
            "Связь",
            "Прочее"
        };

        private Label lblTitle;
        private Label lblAuthor;
        private Label lblBalance;
        private Label lblAmount;
        private Label lblCategory;
        private Label lblType;
        private Label lblFilter;
        private TextBox txtAmount;
        private ComboBox cmbCategory;
        private ComboBox cmbType;
        private Button btnAdd;
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
            lblType = new Label();
            lblFilter = new Label();
            txtAmount = new TextBox();
            cmbCategory = new ComboBox();
            cmbType = new ComboBox();
            btnAdd = new Button();
            btnDelete = new Button();
            btnFilter = new Button();
            btnClearFilter = new Button();
            btnRefresh = new Button();
            cmbFilterCategory = new ComboBox();
            dgvTransactions = new DataGridView();
        }

        private void SetupForm()
        {
            Text = "СИСТЕМА УЧЁТА ФИНАНСОВ";
            Text = "Автор: Токарев Дмитрий";
            Width = 750;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "СИСТЕМА УЧЁТА ФИНАНСОВ";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Size = new Size(400, 30);

            lblAuthor.Text = "Автор: Токарев Дмитрий";
            lblAuthor.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblAuthor.Location = new Point(20, 45);
            lblAuthor.Size = new Size(300, 20);

            lblBalance.Text = "Баланс: 0.00 руб.";
            lblBalance.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblBalance.ForeColor = Color.DarkGreen;
            lblBalance.Location = new Point(20, 75);
            lblBalance.Size = new Size(400, 25);

            lblType.Text = "Тип:";
            lblType.Location = new Point(20, 120);
            lblType.Size = new Size(30, 20);

            cmbType.Location = new Point(55, 117);
            cmbType.Size = new Size(100, 23);
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.Items.Add("Доход");
            cmbType.Items.Add("Расход");
            cmbType.SelectedIndex = 0;
            cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;

            lblAmount.Text = "Сумма:";
            lblAmount.Location = new Point(170, 120);
            lblAmount.Size = new Size(50, 20);

            txtAmount.Location = new Point(225, 117);
            txtAmount.Size = new Size(100, 23);

            lblCategory.Text = "Категория:";
            lblCategory.Location = new Point(340, 120);
            lblCategory.Size = new Size(65, 20);

            cmbCategory.Location = new Point(410, 117);
            cmbCategory.Size = new Size(130, 23);
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            btnAdd.Text = "Добавить";
            btnAdd.Location = new Point(560, 115);
            btnAdd.Size = new Size(120, 28);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += BtnAdd_Click;

            btnDelete.Text = "Удалить";
            btnDelete.Location = new Point(20, 160);
            btnDelete.Size = new Size(100, 28);
            btnDelete.BackColor = Color.LightGray;
            btnDelete.Click += BtnDelete_Click;

            lblFilter.Text = "Фильтр:";
            lblFilter.Location = new Point(135, 165);
            lblFilter.Size = new Size(50, 20);

            cmbFilterCategory.Location = new Point(185, 162);
            cmbFilterCategory.Size = new Size(150, 23);
            cmbFilterCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            btnFilter.Text = "Применить";
            btnFilter.Location = new Point(345, 160);
            btnFilter.Size = new Size(90, 28);
            btnFilter.Click += BtnFilter_Click;

            btnClearFilter.Text = "Сбросить";
            btnClearFilter.Location = new Point(445, 160);
            btnClearFilter.Size = new Size(90, 28);
            btnClearFilter.Click += BtnClearFilter_Click;

            btnRefresh.Text = "Обновить";
            btnRefresh.Location = new Point(545, 160);
            btnRefresh.Size = new Size(135, 28);
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
            Controls.Add(lblType);
            Controls.Add(cmbType);
            Controls.Add(lblAmount);
            Controls.Add(txtAmount);
            Controls.Add(lblCategory);
            Controls.Add(cmbCategory);
            Controls.Add(btnAdd);
            Controls.Add(btnDelete);
            Controls.Add(lblFilter);
            Controls.Add(cmbFilterCategory);
            Controls.Add(btnFilter);
            Controls.Add(btnClearFilter);
            Controls.Add(btnRefresh);
            Controls.Add(dgvTransactions);

            UpdateCategoryComboBox();
        }

        private void CmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCategoryComboBox();
        }

        private void UpdateCategoryComboBox()
        {
            cmbCategory.Items.Clear();
            if (cmbType.SelectedItem != null && cmbType.SelectedItem.ToString() == "Доход")
            {
                foreach (string cat in incomeCategories)
                    cmbCategory.Items.Add(cat);
            }
            else
            {
                foreach (string cat in expenseCategories)
                    cmbCategory.Items.Add(cat);
            }
            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
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
            MessageBox.Show($"Транзакция добавлена! ID: {transaction.Id}", "Успех",
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
                MessageBox.Show($"Транзакция ID {id} удалена.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Транзакция с ID {id} не найдена.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshAll()
        {
            lblBalance.Text = $"Текущий баланс: {data.Balance:F2} руб.";
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
            cmbFilterCategory.Items.Add("Все категории");
            foreach (string cat in categories)
                cmbFilterCategory.Items.Add(cat);
            if (selected != null && cmbFilterCategory.Items.Contains(selected))
                cmbFilterCategory.SelectedItem = selected;
            else if (currentFilter != null && cmbFilterCategory.Items.Contains(currentFilter))
                cmbFilterCategory.SelectedItem = currentFilter;
            else
                cmbFilterCategory.SelectedIndex = 0;
        }

        private void UpdateTransactionsGrid()
        {
            List<Transaction> displayList;
            if (string.IsNullOrEmpty(currentFilter) || currentFilter == "Все категории")
                displayList = data.Transactions;
            else
                displayList = data.Transactions
                    .Where(t => t.Category.Equals(currentFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            dgvTransactions.DataSource = null;
            dgvTransactions.DataSource = displayList.Select(t => new
            {
                t.Id,
                Тип = t.Type == "income" ? "Доход" : "Расход",
                Сумма = t.Amount.ToString("F2"),
                Категория = t.Category,
                Дата = t.Date
            }).ToList();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string type = cmbType.SelectedItem.ToString() == "Доход" ? "income" : "expense";
            string category = cmbCategory.SelectedItem.ToString();

            AddTransaction(type, amount, category);
            txtAmount.Clear();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTransactions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите транзакцию для удаления!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = (int)dgvTransactions.SelectedRows[0].Cells[0].Value;
            DialogResult result = MessageBox.Show($"Удалить транзакцию ID {id}?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                DeleteTransaction(id);
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            currentFilter = cmbFilterCategory.SelectedItem?.ToString();
            UpdateTransactionsGrid();
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            currentFilter = null;
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

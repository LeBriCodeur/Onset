using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MCF___Easy_Onset_Package
{
    /*
        public class PackageJson
        {
            public string author { get; set; }
            public string version { get; set; }
            public string[] server_scripts { get; set; }
            public string[] client_scripts { get; set; }
        }

    */
    public partial class Form1 : Form
    {
        private const string MY_NAME = "MCF - Easy Onset Package";
        private string pathDay;
        private string author;
        private List<string> listPackage = new List<string>();
        private StringBuilder myStr = new StringBuilder();
        public Form1()
        {
            InitializeComponent();
        }
        //-- Load
        private void Form1_Load(object sender, EventArgs e)
        {
            //-- No Loading
        }

        private void UpdateList()
        {
            var checkList = checkedListBox1;
            checkList.Items.Clear();

            if (listPackage.Count == 0) return;

            foreach (var item in listPackage)
            {
                checkList.Items.Add(item);
                checkList.SetItemChecked(checkList.Items.Count - 1, true);
            }
        }
        //-- Ajout des packages à la liste
        private void Button2_Click(object sender, EventArgs e)
        {
            var NamePackage = textBox1.Text.Trim();
            if (NamePackage == string.Empty) return;
            if (listPackage.Contains(NamePackage))
            {
                MessageBox.Show(string.Format("Package : \"{0}\"\nexiste déjà dans la liste", NamePackage), "Ajout impossible");
                return;
            }
            listPackage.Add(NamePackage);
            UpdateList();
        }

        //-- Creation des packages
        private void Button1_Click(object sender, EventArgs e)
        {
            author = textBox2.Text.Trim();
            if (author == string.Empty)
            {
                MessageBox.Show("Auteur manquant !");
                return;
            }
            var myDate = DateTime.Today.ToString().Split(new Char[] { ' ' })[0];
            myDate = myDate.Replace('/', '-');
            var pathDesk = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var pathEnv = Path.Combine(pathDesk, MY_NAME);
            pathDay = Path.Combine(pathEnv, string.Format("package_{0}", myDate));

            if (!Directory.Exists(pathEnv))
                Directory.CreateDirectory(pathEnv);

            if (!Directory.Exists(pathDay))
                Directory.CreateDirectory(pathDay);
            
            foreach (var item in checkedListBox1.CheckedItems)
            {
                var pathPackage = Path.Combine(pathDay, item.ToString());
                if (!Directory.Exists(pathPackage))
                    Directory.CreateDirectory(pathPackage);

                CreateFile(item.ToString(), pathPackage);
            }

            MessageBox.Show("Création avec succès.\nVous trouverez un dossier " +
                "\n\"" + MY_NAME + "\"\nsur votre bureau.", "Succès");
        }
        
        private string FileDefault(int p_step, string p_fileName)
        {
            var myFile = string.Empty;
            myStr.Clear();
            switch (p_step)
            {
                case 0:
                    myStr.Append("{\n\t\"author\": \"");
                    myStr.Append("" + author + "\",");
                    myStr.AppendLine("\n\t\"version\": \"1.0\",");
                    myStr.AppendLine("\t\"server_scripts\": [");
                    myStr.AppendFormat("\t\t\"{0}_s.lua\"\n\t],\n", p_fileName);
                    myStr.AppendLine("\t\"client_scripts\": [");
                    myStr.AppendFormat("\t\t\"{0}_c.lua\"", p_fileName);
                    myStr.AppendLine("\n\t],");
                    myStr.AppendLine("\t\"files\": []");
                    myStr.AppendLine("}");
                    break;
                case 1:
                    myStr.Append("function OnPackageStart()");
                    myStr.AppendFormat("\n\tprint(\"Run - CLIENT - {0}_c.lua\")\nend", p_fileName);
                    myStr.AppendLine("\nAddEvent(\"OnPackageStart\", OnPackageStart)");
                    break;
                case 2:
                    myStr.Append("function OnPackageStart()");
                    myStr.AppendFormat("\n\tprint(\"Run - SERVER - {0}_c.lua\")\nend", p_fileName);
                    myStr.AppendLine("\nAddEvent(\"OnPackageStart\", OnPackageStart)");
                    break;
            };
            return myStr.ToString();
        }

        private void CreateFile(string p_fileName, string p_pathPackage)
        {
            var path = Path.Combine(p_pathPackage, "package.json");
            File.WriteAllText(path, FileDefault(0, p_fileName));
            path = Path.Combine(p_pathPackage, string.Format("{0}_c.lua", p_fileName));
            File.WriteAllText(path, FileDefault(1, p_fileName));
            path = Path.Combine(p_pathPackage, string.Format("{0}_s.lua", p_fileName));
            File.WriteAllText(path, FileDefault(2, p_fileName));
        }
    }
}
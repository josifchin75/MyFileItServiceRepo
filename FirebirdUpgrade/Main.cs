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

namespace FirebirdUpgrade
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private string Firebird2ExeName = "Firebird-2.5.3.26780_0_Win32.exe";
        private string Firebird2GBAKPath = @"C:\Program Files (x86)\Firebird\Firebird_2_5\bin\gbak";
        private string FB2Path { get; set; }
        private bool AllowUpgrade { get; set; }

        private void Main_Load(object sender, EventArgs e)
        {
            lblFirebirdPath.Text = Firebird2GBAKPath;//@"C:\Development\Firebird\download\Firebird-2.5.3.26780_0_Win32.exe";
            lblGBAKPath.Text = @"C:\Program Files (x86)\Firebird\Firebird_1_5\bin\gbak";

            CheckFBPath();
            //FindFB2();
        }

        private void btnCreateScript_Click(object sender, EventArgs e)
        {
            var files = new List<string>();
            var root = new DirectoryInfo(@"C:\");
            var sb = new System.Text.StringBuilder();

            SearchForFilePattern(root, "*.fileit", ref files);
            files.ToList().ForEach(f =>
            {
                sb.AppendLine(CreateFileBackUpScript(f));
            });

            sb.AppendLine(CreateUpgradeDescription());

            files.ToList().ForEach(f =>
            {
                sb.AppendLine(CreateFileRestoreScript(f));
            });

            txtScript.Text = sb.ToString();
        }

        private void btnGetPath_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lblFirebirdPath.Text = openFileDialog1.FileName;
            }

            CheckFBPath();
        }

        private void btnGBAKPath_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                lblGBAKPath.Text = openFileDialog1.FileName;
            }

            CheckFBPath();
        }

        private string CreateFileBackUpScript(string f)
        {
            var sb = new System.Text.StringBuilder();
            ////"C:\Program Files (x86)\Firebird\Firebird_1_5\bin\gbak" -v -t -user SYSDBA -password "masterkey" localhost:%%i %%i.fbk
            var backupfilename = CreateBackupFileName(f);
            if (!File.Exists(CreateCopyFileName(f)))
            {
                try
                {
                    File.Copy(f, CreateCopyFileName(f));
                }
                catch (Exception ex) {
                }
            }
            sb.Append("\"" + lblGBAKPath.Text + "\"  -v -t -user SYSDBA -password \"masterkey\" localhost:\"" + f + "\" \"" + Path.Combine( Path.GetDirectoryName(f), backupfilename) + "\"");
            return sb.ToString();
        }

        private string CreateBackupFileName(string s)
        {
            return Path.GetFileNameWithoutExtension(s) + ".fbk";
        }

        private string CreateCopyFileName(string s)
        {
            return s + ".ORIG";
        }

        private string CreateUpgradeDescription()
        {
            var result = new System.Text.StringBuilder();
            result.Append(Environment.NewLine);
            result.Append(Environment.NewLine);
            result.Append("Please Upgrade to Firebird 2.# now, when that is complete run the second half of the script to restore the file tables as 2.# firebird dbs");
            result.Append(Environment.NewLine);
            result.Append(Environment.NewLine);
            return result.ToString();
        }

        private string CreateFileRestoreScript(string f)
        {
            //"C:\Program Files (x86)\Firebird\Firebird_2_5\bin\gbak" -c -t -user SYSDBA -password "masterkey" %%i localhost:%%i.FILEIT
            return "\"" + lblFirebirdPath.Text + "\"  -c -t -REP -user SYSDBA -password \"masterkey\" \"" + Path.Combine(Path.GetDirectoryName(f), CreateBackupFileName(f)) + "\" localhost:\"" + f + "\"";
        }



        private void CheckFBPath()
        {
            var found = true;
            //if (lblFirebirdPath.Text.Length > 0 && lblGBAKPath.Text.Length > 0)
            //{
            //    found = File.Exists(lblGBAKPath.Text);
            //}
            btnCreateScript.Enabled = found;
        }

        private void FindFB2()
        {
            AllowUpgrade = false;
            var dir = new DirectoryInfo(@"C:\");
            FB2Path = SearchForFile(dir, Firebird2ExeName);
            AllowUpgrade = FB2Path.Length > 0;

            //lblFB2.Text = AllowUpgrade ? "Firebird 2 install found at: " + FB2Path : "Could not find the Firebird 2 installation file.";
        }

        private string SearchForFile(DirectoryInfo dir, string filename)
        {
            var result = "";
            if (dir.Exists)
            {
                try
                {
                    foreach (var f in dir.GetFiles())
                    {
                        System.Diagnostics.Debug.WriteLine(f.FullName);
                        if (f.Name.Equals(filename, StringComparison.CurrentCultureIgnoreCase))
                        {
                            result = dir.FullName;
                        }
                    };

                    if (result.Length == 0 && dir.GetDirectories().Length > 0)
                    {
                        foreach (var d in dir.GetDirectories())
                        {
                            if (IncludeDirectoryInSearch(d))
                            {
                                result = SearchForFile(d, filename);
                            }
                        }
                    }
                }
                catch (Exception ex) { }
            }
            return result;
        }

        private void SearchForFilePattern(DirectoryInfo dir, string search, ref List<string> filepaths)
        {
            try
            {
                bool found = false;
                //System.Diagnostics.Debug.WriteLine(dir.FullName);
                if (search != null)
                {
                    foreach (FileInfo f in dir.GetFiles(search))
                    {
                        filepaths.Add(f.FullName);
                    }
                }

                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    if (IncludeDirectoryInSearch(d))
                    {
                        SearchForFilePattern(d, search, ref filepaths);
                    }
                }
            }
            catch (Exception ex) { }
        }

        private bool IncludeDirectoryInSearch(DirectoryInfo d)
        {
            var include = true;
            if (d.Attributes.HasFlag(FileAttributes.Hidden)) {
                include = false;
            }
            if (d.Attributes.HasFlag(FileAttributes.System))
            {
                include = false;
            }
            if (d.Attributes.HasFlag(FileAttributes.Temporary))
            {
                include = false;
            }
            //return !(d.Attributes.HasFlag(FileAttributes.Hidden) || !d.Attributes.HasFlag(FileAttributes.System) || !d.Attributes.HasFlag(FileAttributes.Temporary) || !d.FullName.Contains('$'));
            return include;
        }








    }

}

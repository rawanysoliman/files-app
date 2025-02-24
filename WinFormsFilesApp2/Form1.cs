namespace WinFormsFilesApp2
{
    public partial class FileManager : Form
    {

        private ListBox lastAccessedPane;
        public FileManager()
        {
            InitializeComponent();
        }


        private void LoadDrives()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (var drive in DriveInfo.GetDrives())
            {
                listBox1.Items.Add(drive.Name);
                listBox2.Items.Add(drive.Name);
            }
            AddNavigationItems(listBox1);
            AddNavigationItems(listBox2);
        }


        private void AddNavigationItems(ListBox listBox)
        {
            listBox.Items.Add(".");
            listBox.Items.Add("..");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDrives();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            lastAccessedPane = listBox1;
            LoadDirectory(listBox1, textBox1);
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            lastAccessedPane = listBox2;
            LoadDirectory(listBox2, textBox2);
        }

        private void LoadDirectory(ListBox listBox, TextBox textBox)
        {
            var selectedItem = listBox.SelectedItem?.ToString();   //null oprator

            if (selectedItem == ".")
            {
                GoBack(listBox, textBox);
                return;
            }
            if (selectedItem == "..")
            {
                LoadDrives();
                textBox.Text = "";
                return;
            }

            if (Directory.Exists(selectedItem))  //folder
            {
                listBox.Items.Clear();
                foreach (var dir in Directory.GetDirectories(selectedItem))
                {
                    listBox.Items.Add(dir);
                }
                foreach (var file in Directory.GetFiles(selectedItem))
                {
                    listBox.Items.Add(file);
                }
                AddNavigationItems(listBox);
                textBox.Text = selectedItem;
            }
            else if (File.Exists(selectedItem)) //file 
            {
                MessageBox.Show("This is a file and can't be opened.");
            }

        }

        private void button2_MouseClick(object sender, MouseEventArgs e) //right move btn
        {
            //MessageBox.Show($"Clickevnt for listBox1: {textBox1.Text}");
            //MessageBox.Show($"Clickevent for listBox2: {textBox2.Text}");
            MoveItem(listBox1, listBox2);
        }

        private void button1_MouseClick(object sender, MouseEventArgs e) //left move btn
        {

            MoveItem(listBox2, listBox1);
        }


        private void MoveItem(ListBox source, ListBox destination)
        {
            if (source.SelectedItem != null)
            {
                string itemName = source.SelectedItem.ToString();
                string sourcePath = itemName;


                //  name of the item
                string itemNamePart = Path.GetFileName(sourcePath);

                //MessageBox.Show($"itemNamePart: {itemNamePart}");

                string destinationPath = Path.Combine(GetCurrentPath(destination), itemNamePart);


                //MessageBox.Show($"Source Path: {sourcePath}");
                //MessageBox.Show($"Destination Path: {destinationPath}");

                try
                {
                    if (Directory.Exists(sourcePath))
                    {
                        if (Directory.Exists(destinationPath))
                        {
                            MessageBox.Show("A directory with the same name already exists in the destination.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        Directory.Move(sourcePath, destinationPath);  // Move directory
                    }
                    else if (File.Exists(sourcePath))
                    {
                        if (File.Exists(destinationPath))
                        {
                            MessageBox.Show("A file with the same name already exists in the destination.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        File.Move(sourcePath, destinationPath);  // Move file
                    }
                    else
                    {
                        MessageBox.Show("The selected item does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Remove from  ListBox 
                    source.Items.Remove(itemName);
                    destination.Items.Add(destinationPath); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error moving item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GetCurrentPath(ListBox listBox)
        {
            if (listBox == listBox1)
            {
             
                return textBox1.Text;
            }
            else
            {
                return textBox2.Text;
            }
        }

 


        private void button3_MouseClick(object sender, MouseEventArgs e) // copy btn
        {
            CopyItem(lastAccessedPane == listBox1 ? listBox1 : listBox2, lastAccessedPane == listBox1 ? listBox2 : listBox1);

        }

        private void CopyItem(ListBox source, ListBox destination)
        {
            if (source.SelectedItem != null)
            {
                string itemName = source.SelectedItem.ToString();

                string sourcePath = itemName;

                string itemNamePart = Path.GetFileName(sourcePath);

                string destinationPath = Path.Combine(GetCurrentPath(destination), itemNamePart);

                try
                {
                    if (Directory.Exists(sourcePath))
                    {
                        // If it's a directory
                        CopyDirectory(sourcePath, destinationPath); //strings
                    }


                    else if (File.Exists(sourcePath))
                    {
                        File.Copy(sourcePath, destinationPath, true); // Overwrite if exists
                    }

                    destination.Items.Add(destinationPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error copying item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void CopyDirectory(string sourceDir, string destinationDir) 
        {
            Directory.CreateDirectory(destinationDir);  //same name of destination path

            foreach (var file in Directory.GetFiles(sourceDir)) //copy files
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true); // Overwrite if exists
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                string destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
                CopyDirectory(directory, destDir);   //recursive
            }
        }


        private void button4_MouseClick(object sender, MouseEventArgs e) //delete btn
        {
            if (lastAccessedPane != null)
            {
                TextBox targetTextBox;

                if (lastAccessedPane == listBox1)
                {
                    targetTextBox = textBox1;
                }
                else
                {
                    targetTextBox = textBox2;
                }

                DeleteItem(lastAccessedPane, targetTextBox);
            }
        }


        private void DeleteItem(ListBox listBox, TextBox textBox)
        {
            if (listBox.SelectedItem == null) return;

            string itemName = listBox.SelectedItem.ToString();
            string itemPath = Path.Combine(textBox.Text, itemName);  //full name with extension

            var result = MessageBox.Show($"Are you sure you want to delete {itemName}?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                    if (Directory.Exists(itemPath))
                    {
                        Directory.Delete(itemPath, true); // Delete directory full name
                }

                    else if (File.Exists(itemPath))
                    {
                        File.Delete(itemPath); // Delete file
                    }

                    listBox.Items.Remove(itemName);//remove from list

            }
        }



        private void button5_MouseClick(object sender, MouseEventArgs e) //back btn 
        {
            if (lastAccessedPane != null)
            {
                GoBack(lastAccessedPane, lastAccessedPane == listBox1 ? textBox1 : textBox2);
            }
        }


        private void GoBack(ListBox listBox, TextBox textBox)
        {
            var currentPath = textBox.Text;

            if (string.IsNullOrEmpty(currentPath))
            {
                return;
            }

            var parentPath = Directory.GetParent(currentPath)?.FullName;  //null operator to handle null exception

            if (parentPath != null)
            {
                listBox.Items.Clear();
                foreach (var dir in Directory.GetDirectories(parentPath))
                {
                    listBox.Items.Add(dir);
                }
                foreach (var file in Directory.GetFiles(parentPath))
                {
                    listBox.Items.Add(file);
                }


                AddNavigationItems(listBox);
                textBox.Text = parentPath;
            }


            else //root (drive)
            {
                LoadDrives();
                textBox.Text = "";
            }
        }

        //private void UpdateButtonStates()
        //{
        //    button3.Enabled = lastAccessedPane?.SelectedItem != null;
        //    button4.Enabled = lastAccessedPane?.SelectedItem != null;
        //}


    }
}


//, in .NET, there isn't a built-in method that copies an entire directory and its contents. Unlike File.Copy for files
//, Directory doesn't have a Copy method. So you have to handle directories recursively yourself.
//There’s no Directory.Copy method in .NET, so you must implement it yourself.
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace DataStructure7._0
{
    public partial class Form1 : Form
    {
        static int maxRecords = 12; // Max number of records (12)
        static int attributes = 4; // The number of attribute per each record (4)
        int recordCount = 0; // Current record number
        string[,] dataRecords = new string[maxRecords, attributes]; // Data storage for the array
        private int selectedIndex = -1; // Declare selectedIndex at the class level

        public Form1()
        {
            InitializeComponent();// Initialize the forms components
            listView.SelectedIndexChanged += listView_SelectedIndexChanged; // Change of selections for listView
        }
        // Method to display records in the list view
        private void DisplayList()
        {
            listView.Items.Clear(); // Clear listView

            for (int i = 0; i < recordCount; i++)
            {
                string[] itemData = new string[2]; // Assuming two columns for name and category

                itemData[0] = dataRecords[i, 0]; // Name
                itemData[1] = dataRecords[i, 1]; // Category

                ListViewItem item = new ListViewItem(itemData); // Create an item for listView
                listView.Items.Add(item); // Add to listView
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                int resultIndex = listView.SelectedItems[0].Index;

                // Display the information in the appropriate textboxes
                dataStructureTxt.Text = dataRecords[resultIndex, 0]; // Name
                categoryTxt.Text = dataRecords[resultIndex, 1];      // Category
                structureTxt.Text = dataRecords[resultIndex, 2];
                definitionTxt.Text = dataRecords[resultIndex, 3];
            }
        }
        private void DisplayRecord(int index)
        {
            if (index >= 0 && index < recordCount)
            {
                // Display the selected record's information in the textboxes
                dataStructureTxt.Text = dataRecords[index, 0]; // Name
                categoryTxt.Text = dataRecords[index, 1];      // Category
                structureTxt.Text = dataRecords[index, 2]; // Structure
                definitionTxt.Text = dataRecords[index, 3]; // Definition

                // Set the selectedIndex variable for other operations
                selectedIndex = index;
            }
        }
        //Method to Add a Rcord
        private void AddRecord()
        {
            if (recordCount < maxRecords)
            {
                // Check all text boxes are fill
                if (!string.IsNullOrWhiteSpace(dataStructureTxt.Text) &&
                    !string.IsNullOrWhiteSpace(categoryTxt.Text) &&
                    !string.IsNullOrWhiteSpace(structureTxt.Text) &&
                    !string.IsNullOrWhiteSpace(definitionTxt.Text))
                {
                    // All text boxes are filled, proceed with adding the record
                    try
                    {
                        // Store data in the dataRecords array
                        dataRecords[recordCount, 0] = dataStructureTxt.Text; // Name
                        dataRecords[recordCount, 1] = categoryTxt.Text;      // Category
                        dataRecords[recordCount, 2] = structureTxt.Text;
                        dataRecords[recordCount, 3] = definitionTxt.Text;

                        // Create a list view item and add it to the list view
                        ListViewItem item = new ListViewItem(new string[] { dataStructureTxt.Text, categoryTxt.Text });
                        listView.Items.Add(item);

                        recordCount++; // Add to record count
                        toolStripStatusLabel1.Text = "Record added successfully"; // Notify user of success

                        // Clear text boxes
                        ClearTextBoxes();
                    }
                    catch
                    {
                        toolStripStatusLabel1.Text = "Error adding record";
                    }
                }
                else
                {
                    // Not all text boxes are filled, show an error message in the status strip
                    toolStripStatusLabel1.Text = "Please fill all text boxes";
                }
            }
            else
            {
                toolStripStatusLabel1.Text = "The array is FULL";
            }
        }
        // Method to edit an existing record
        private void EditRecord(int index)
        {
            dataRecords[index, 0] = dataStructureTxt.Text; // Update name
            dataRecords[index, 1] = categoryTxt.Text; // Update category
            dataRecords[index, 2] = structureTxt.Text; // Update structure
            dataRecords[index, 3] = definitionTxt.Text; // Update definition
        }
        // Method to delete a record
        private void DeleteRecord(int index)
        {
            // Warn user if they wish to delete record
            if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm Deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Shift records to remove the selected one
                for (int i = index; i < recordCount - 1; i++)
                {
                    for (int j = 0; j < attributes; j++)
                    {
                        dataRecords[i, j] = dataRecords[i + 1, j];
                    }
                }
                recordCount--; // Take one away from recordCount
                ClearTextBoxes(); 
                DisplayList(); // Refresh the listView to reflect changed
            }
        }

        // Method to clear textBoxes
        private void ClearTextBoxes()
        {
            dataStructureTxt.Clear(); ;
            categoryTxt.Clear();
            structureTxt.Clear();
            definitionTxt.Clear(); ;

        }
        // Add button implementing add method
         private void addBtn_Click(object sender, EventArgs e)
        {
            AddRecord();
            ClearTextBoxes();
            SortRecords();
            DisplayList();
        }

        // Edit button using Edit Method
        private void editBtn_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count > 0)
            {
                int selectedIndex = listView.SelectedIndices[0];
                EditRecord(selectedIndex);
                ClearTextBoxes();
                SortRecords();
                DisplayList();
                toolStripStatusLabel1.Text = "Record edited successfully";

            }
        }

        //Delete button using delete method
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count > 0)
            {
                int selectedIndex = listView.SelectedIndices[0];
                DeleteRecord(selectedIndex);
            }
        }
        // Save button using SaveToFile method
        private void saveBtn_Click(object sender, EventArgs e)
        {
            SaveToFile();
        }

        private void SaveToFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            // filter for file type and include all files options
            saveFileDialog.Filter = "Binary Files (*.dat)|*.dat|All Files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (BinaryWriter bw = new BinaryWriter(new FileStream(saveFileDialog.FileName, FileMode.Create)))
                    {
                        // Write data to file
                        for (int i = 0; i < recordCount; i++)
                        {
                            for (int j = 0; j < attributes; j++)
                            {
                                bw.Write(dataRecords[i, j]);
                            }
                        }
                    }

                    // Show a success message
                    toolStripStatusLabel1.Text = "Data saved successfully to file.";
                }
                catch (Exception ex)
                {
                    toolStripStatusLabel1.Text = "Error saving data to file: " + ex.Message;
                }
            }
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        
        private void SortRecords()
        {
            for (int i = 0; i < recordCount - 1; i++)
            {
                for (int j = 0; j < recordCount - i - 1; j++)
                {
                    if (string.Compare(dataRecords[j, 0], dataRecords[j + 1, 0]) > 0)
                    {
                        SwapRecords(j);
                    }
                }
            }

        }
        private void SwapRecords(int index)
        {
            for (int i = 0; i < attributes; i++)
            {
                string temp = dataRecords[index, i]; // Store the value in a temporary variable
                dataRecords[index, i] = dataRecords[index + 1, i]; // Replace the value with the next record's value
                dataRecords[index + 1, i] = temp; // Assign the temporary value to the next record
            }
        }
        // clear button implementing clear method
        private void clearBtn_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }
        
        private int BinarySearch(string targetName)
        {
            int low = 0;
            int high = recordCount - 1;

            while (low <= high)
            {
                int mid = (low + high) / 2;
                int compareResult = string.Compare(dataRecords[mid, 0], targetName);

                if (compareResult == 0)
                {
                    return mid; // Found the record
                }
                else if (compareResult < 0)
                {
                    low = mid + 1; // Search in the right half
                }
                else
                {
                    high = mid - 1; // Search in the left half
                }
            }

            return -1; // Not found
        }
        private void LoadFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Binary Files (*.dat)|*.dat|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (BinaryReader br = new BinaryReader(new FileStream(openFileDialog.FileName, FileMode.Open)))
                    {
                        recordCount = 0;
                        while (br.BaseStream.Position != br.BaseStream.Length)
                        {
                            for (int j = 0; j < attributes; j++)
                            {
                                dataRecords[recordCount, j] = br.ReadString();
                            }
                            recordCount++;
                        }
                    }

                    // Refresh the ListView and show a success message
                    DisplayList();
                    toolStripStatusLabel1.Text = "Data loaded successfully from file.";
                }
                catch (Exception ex)
                {
                    toolStripStatusLabel1.Text = "Error loading data from file: " + ex.Message;
                }
            }
        }

        private void dataStructureTxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void statusStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            LoadFromFile();
        }

        private void listView_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            string targetName = searchTxt.Text; // Get the target name from the textbox

            if (string.IsNullOrWhiteSpace(targetName))
            {
                toolStripStatusLabel1.Text = "Please enter a search term.";
                return;
            }

            int resultIndex = BinarySearch(targetName); // Perform a binary search

            if (resultIndex != -1)
            {
                // Display the information in the appropriate textboxes
                dataStructureTxt.Text = dataRecords[resultIndex, 0]; // Name
                categoryTxt.Text = dataRecords[resultIndex, 1];      // Category
                structureTxt.Text = dataRecords[resultIndex, 2]; // Structure
                definitionTxt.Text = dataRecords[resultIndex, 3]; // Difinition

                // Highlight the row in the ListView
                listView.Items[resultIndex].Selected = true;
                listView.Select();

                toolStripStatusLabel1.Text = "Record found.";
            }
            else
            {
                ClearTextBoxes();
                toolStripStatusLabel1.Text = "Record not found.";
            }
        }
       

        private void sortBtn_Click(object sender, EventArgs e)
        {
            SortRecords();
        }
    }
}
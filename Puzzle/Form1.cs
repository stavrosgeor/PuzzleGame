using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Stavros Georgiou Puzzle 

namespace Puzzle
{
    public partial class Form1 : Form
    {
        private int clickCount = 0;
        private Button[] btnNum;
        private int emptyIndex = 8; // Index of the empty button
        private int spacing = 10;   // Declare spacing at the class level
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            InitializeButtons();
        }
        // Initialize puzzle buttons
        private void InitializeButtons()
        {
            btnNum = new Button[9];

            int buttonSize = 80; // Increase button size for a bigger puzzle
            int spacing = 10;    // Increase the spaces between buttons

            for (int i = 0; i < 9; i++)
            {
                btnNum[i] = new Button();
                btnNum[i].Size = new System.Drawing.Size(buttonSize, buttonSize);
                btnNum[i].Click += BtnNum_Click;
                btnNum[i].Tag = i + 1;

                // Set the text on the buttons (except the last one)
                if (i < 8)
                {
                    btnNum[i].Text = (i + 1).ToString();
                    btnNum[i].ForeColor = System.Drawing.Color.Red; // Set text color to red
                    btnNum[i].Font = new System.Drawing.Font(btnNum[i].Font.FontFamily, 16, System.Drawing.FontStyle.Bold); // Set font size to 16
                }

                int row = i / 3;
                int col = i % 3;

                // Center the puzzle on the form
                int x = (this.ClientSize.Width - (3 * buttonSize + 2 * spacing)) / 2 + col * (buttonSize + spacing);
                int y = (this.ClientSize.Height - (3 * buttonSize + 2 * spacing)) / 2 + row * (buttonSize + spacing);

                btnNum[i].Location = new System.Drawing.Point(x, y);
                this.Controls.Add(btnNum[i]);
            }

            NewGame();
        }
        // Handle button click event
        private void BtnNum_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            int clickedIndex = Array.IndexOf(btnNum, clickedButton);

            if (IsAdjacent(clickedIndex, emptyIndex))
            {
                SwapButtons(clickedIndex, emptyIndex);
                emptyIndex = clickedIndex;

                clickCount++;
                label1.Text = $"Move # {clickCount}";

                if (Solved())
                {
                    MessageBox.Show("Congratulations! You've solved the puzzle.");
                    NewGame();
                    clickCount = 0;
                    label1.Text = "Move #0";
                }

                // Update button locations after each click
                UpdateButtonLocations();
            }
        }
        // Update button locations based on the puzzle state
        private void UpdateButtonLocations()
        {
            for (int i = 0; i < 9; i++)
            {
                int row = i / 3;
                int col = i % 3;
                int x = (this.ClientSize.Width - (3 * btnNum[i].Width + 2 * spacing)) / 2 + col * (btnNum[i].Width + spacing);
                int y = (this.ClientSize.Height - (3 * btnNum[i].Height + 2 * spacing)) / 2 + row * (btnNum[i].Height + spacing);
                btnNum[i].Location = new System.Drawing.Point(x, y);
            }
        }
        // Check if two buttons are adjacent
        private bool IsAdjacent(int index1, int index2)
        {
            int row1 = index1 / 3;
            int col1 = index1 % 3;
            int row2 = index2 / 3;
            int col2 = index2 % 3;

            // Check if the two buttons are horizontally or vertically adjacent
            return (Math.Abs(row1 - row2) == 1 && col1 == col2) || (Math.Abs(col1 - col2) == 1 && row1 == row2);
        }
        // Swap positions of two buttons
        private void SwapButtons(int index1, int index2)
        {
            Button temp = btnNum[index1];
            btnNum[index1] = btnNum[index2];
            btnNum[index2] = temp;

            UpdateButtonLocations();
        }
        // Check if the puzzle is in a solved state
        private bool Solved()
        {
            // Check for the two provided goal states
            bool isGoalState1 = btnNum[0].Text == "1" && btnNum[1].Text == "2" && btnNum[2].Text == "3" &&
                                btnNum[3].Text == "4" && btnNum[4].Text == "5" && btnNum[5].Text == "6" &&
                                btnNum[6].Text == "7" && btnNum[7].Text == "8" && btnNum[8].Text == "";

            bool isGoalState2 = btnNum[0].Text == "1" && btnNum[1].Text == "2" && btnNum[2].Text == "3" &&
                                btnNum[3].Text == "8" && btnNum[4].Text == "" && btnNum[5].Text == "4" &&
                                btnNum[6].Text == "7" && btnNum[7].Text == "6" && btnNum[8].Text == "5";

            return isGoalState1 || isGoalState2;
        }
        // Randomly allocate numbers function
        private void NewGame()
        {
            List<int> numbers = Enumerable.Range(1, 8).ToList();
            Random random = new Random();

            // Shuffle the numbers using Fisher-Yates shuffle
            for (int i = numbers.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                int temp = numbers[i];
                numbers[i] = numbers[j];
                numbers[j] = temp;
            }

            // Assign the shuffled numbers to buttons
            for (int i = 0; i < 8; i++)
            {
                btnNum[i].Text = numbers[i].ToString();
                btnNum[i].Tag = numbers[i];
                SetButtonStyle(btnNum[i]); // Set the font style for each numbered button
            }

            // Set the style for the empty button
            btnNum[8].Text = "";
            btnNum[8].Tag = 9;
            SetButtonStyle(btnNum[8]);

            // Ensure the puzzle is solvable
            if (!IsSolvable())
            {
                // If not solvable, swap the positions of the last two buttons
                SwapButtons(7, 8);
            }

            // Update the emptyIndex after shuffling
            emptyIndex = Array.IndexOf(btnNum, btnNum.First(b => b.Text == ""));
        }

        private void SetButtonStyle(Button button)
        {
            button.ForeColor = System.Drawing.Color.Red; // Set text color to red
            button.Font = new System.Drawing.Font(button.Font.FontFamily, 16, System.Drawing.FontStyle.Bold); // Set font size to 16
        }
        private bool IsSolvable()
        {
            // Count the number of inversions
            int inversions = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = i + 1; j < 8; j++)
                {
                    // Skip empty tile represented by ""
                    if (btnNum[i].Text != "" && btnNum[j].Text != "" && (int)btnNum[i].Tag > (int)btnNum[j].Tag)
                    {
                        inversions++;
                    }
                }
            }
            // If the grid width is even, the number of inversions should be odd for a solvable puzzle
            if (inversions % 2 == 1)
            {
                return true;
            }

            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NewGame();
            clickCount = 0;
            label1.Text = "Move #0";
        }
        //Menu Strip function
        private void InitializeMenu()
        {
            // Create a MenuStrip
            MenuStrip menuStrip = new MenuStrip();

            // Create menu items
            ToolStripMenuItem beginnerMenuItem = new ToolStripMenuItem("Beginner");
            beginnerMenuItem.Click += beginnerToolStripMenuItem_Click;

            ToolStripMenuItem advancedMenuItem = new ToolStripMenuItem("Advanced");
            advancedMenuItem.Click += advantageToolStripMenuItem_Click;

            ToolStripMenuItem newGameMenuItem = new ToolStripMenuItem("New Game");
            newGameMenuItem.Click += newGameToolStripMenuItem_Click;

            // Add menu items to the MenuStrip
            menuStrip.Items.Add(beginnerMenuItem);
            menuStrip.Items.Add(advancedMenuItem);
            menuStrip.Items.Add(newGameMenuItem); // Add the "New Game" menu item

            ToolStripMenuItem colorMenuItem = new ToolStripMenuItem("Color");

            // Add sub-menu items under Color
            ToolStripMenuItem otherMenuItem = new ToolStripMenuItem("Other");
            otherMenuItem.Click += otherToolStripMenuItem_Click;

            // Add sub-menu items to Color menu
            colorMenuItem.DropDownItems.Add(otherMenuItem);

            // Add Color menu to the MenuStrip
            menuStrip.Items.Add(colorMenuItem);

            // Set the MenuStrip for the form
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
        }

        private enum Difficulty
        {
            Beginner,
            Advanced
        }

        private void advantageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Implement logic for Advanced difficulty
            SetDifficulty(Difficulty.Advanced);
        }

        private void beginnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Implement logic for Beginner difficulty
            SetDifficulty(Difficulty.Beginner);
        }

        // Method to adjust the puzzle difficulty
        private void SetDifficulty(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Beginner:
                    NewGame(); // Reset the puzzle for the Beginner difficulty
                    break;

                case Difficulty.Advanced:
                    // Implement logic for Advanced difficulty
                    // For example, shuffle the puzzle more aggressively
                    for (int i = 0; i < 100; i++)
                    {
                        int randomIndex1 = random.Next(0, 8);
                        int randomIndex2 = random.Next(0, 8);
                        SwapButtons(randomIndex1, randomIndex2);
                    }
                    break;

                // Add more difficulty levels if needed

                default:
                    break;
            }

            // Update the emptyIndex after adjusting the puzzle
            emptyIndex = Array.IndexOf(btnNum, btnNum.First(b => b.Text == ""));
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Stavros Georgiou");
        }

        private void otherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show the ColorDialog control when "Other" menu item is clicked
            ColorDialog colorDialog = new ColorDialog();

            // Optionally set initial color
            colorDialog.Color = this.BackColor; // or any default color

            // Show the dialog and get the result
            DialogResult result = colorDialog.ShowDialog();

            // If the user clicks OK, set the background color to the selected color
            if (result == DialogResult.OK)
            {
                this.BackColor = colorDialog.Color;
            }
        }
    }
}
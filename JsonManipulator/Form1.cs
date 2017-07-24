using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace JsonManipulator
{
    public partial class Form1 : Form
    {
        private int numberOfClusters;
        private string targetPath;
        private List<User> users;
        private int numberOfEdges;

        public Form1()
        {
            InitializeComponent();

            PercentComboBox.DataSource = new[] {"5%", "10%", "15%", "20%", "25%", "30%"};
            radioButton2.Checked = true;
        }

        private void OpenFileDialog(EventHandler<ServiceResult<string>> callback)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                callback.Invoke(this, new ServiceResult<string>(openFileDialog.FileName));
            }
        }

        private IEnumerable<User> ReadUsersFromPath(string path)
        {
            try
            {
                var usersJson = File.ReadAllText(path);
                var users = JsonConvert.DeserializeObject<List<User>>(usersJson);

                var friendShipDict = new Dictionary<int, List<int>>();

                foreach (var user in users)
                {
                    var userIndex = user.Index;
                    if (!friendShipDict.ContainsKey(userIndex))
                        friendShipDict[userIndex] = new List<int>();

                    foreach (var friendIndex in user.FriendsIndexs.Distinct().Where(i => i != userIndex)) //syncronize friendsLists
                    {
                        friendShipDict[userIndex].Add(friendIndex);

                        if (friendShipDict.ContainsKey(friendIndex))
                            friendShipDict[friendIndex].Add(userIndex);

                        else
                        {
                            friendShipDict[friendIndex] = new List<int> { userIndex };
                            users[friendIndex].FriendsIndexs.Add(userIndex);
                        }

                    }

                    var friendsDictionary = new Dictionary<string, User>(); //add friends to friendsList

                    foreach (var userFriendIndex in friendShipDict[userIndex])
                        if (userIndex != userFriendIndex)
                            friendsDictionary[userFriendIndex.ToString()] = users[userFriendIndex];

                    user.FriendsList = friendsDictionary.Values.ToList();
                }

                return users;
            }
            catch (Exception)
            {
                throw new IncorrectUsersFileException();
            }
        }

        private List<List<int>> BuildAdjacencyMatrix(List<User> users)
        {
            var n = users.Count;
            var vertices = users;
            var adjacency = new List<List<int>>();
            for (var index = 0; index < n; index++)
                adjacency[index] = new List<int>();
            foreach (var v1 in vertices)
                foreach (var v2 in vertices)
                    if (adjacency[v1.Index][v2.Index] == 1 && v1.FriendsIndexs.Contains(v2.Index))
                    {
                        numberOfEdges++;
                        adjacency[v1.Index][v2.Index] = 1;
                        adjacency[v2.Index][v1.Index] = 1;
                    }

            return adjacency;
        }

        private async void BtnScrumble_Click(object sender, EventArgs e)
        {
            int.TryParse(PercentComboBox.SelectedText.Replace("%", ""), out int percent);
            var adjMatrix = await Task.Run(() => BuildAdjacencyMatrix(users));

            var ets = (numberOfEdges * (percent / 100)) / numberOfClusters;
            var random = new Random();

            for (var i = 0; i < numberOfClusters; i++)
            {
                var nets = ets;
                do
                {
                    var randomNumber = 100 * i + random.Next(0, 100);
                    var neighbor = adjMatrix[randomNumber].First(i1 => adjMatrix[randomNumber][i1] == 1 && i1/100 == i);
                    adjMatrix[randomNumber][neighbor] = 0;
                    adjMatrix[neighbor][randomNumber] = 0;

                    int oc1 = 0, oc2 = 0;
                    
                    if (numberOfClusters == 2)
                    {
                        oc1 = oc2 = (i + 1) % 2;

                    }
                    else if (numberOfClusters == 3)
                    {
                        oc1 = (i + 1) % 3;
                        oc2 = (i + 2) % 3;

                    }

                    var randomVertex1 = 100 * oc1 + random.Next(0, 100);
                    var randomVertex2 = 100 * oc2 + random.Next(0, 100);

                    adjMatrix[randomVertex1][randomNumber] = 1;
                    adjMatrix[randomNumber][randomVertex1] = 1;
                    adjMatrix[randomVertex2][neighbor] = 1;
                    adjMatrix[neighbor][randomVertex2] = 1;
                    nets--;
                } while (nets > 0);
            }

            foreach (var v1 in users)
                v1.FriendsIndexs = new HashSet<int>();

            foreach (var v1 in users)
                foreach (var v2 in users)
                    if (adjMatrix[v1.Index][v2.Index] == 1)
                    {
                        v1.FriendsIndexs.Add(v2.Index);
                        v2.FriendsIndexs.Add(v1.Index);
                    }

            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(targetPath, json);

        }

        private void LoadTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog(async (o, result) =>
            {
                var path = result.Object;
                textBox2.Text = path;
                var temp = await Task.Run(() => ReadUsersFromPath(path));
                users = temp.ToList();      
            });
        }

        private void TargetDirTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog((o, result) =>
            {
                var path = result.Object;
                textBox1.Text = path;
                targetPath = path;
            });
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                numberOfClusters = 2;
            }
            else if (radioButton3.Checked)
            {
                numberOfClusters = 3;
            }
        }

    }
}

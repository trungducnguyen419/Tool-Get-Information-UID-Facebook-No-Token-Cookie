using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Get_Info_UID_Facebook
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
            Button.CheckForIllegalCrossThreadCalls = false;
            DataGridView.CheckForIllegalCrossThreadCalls = false;
        }
        bool isStop = false;
        public struct InfoUID
        {
            public string UrlAvatar { get; set; }
            public string Name { get; set; }
            public string Birthday { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public int Friend { get; set; }
            public int Follow { get; set; }
            public string CreatedTime { get; set; }
        }
        public class GraphQLFacebook
        {
            private string uid { get; set; }
            private HttpClient httpClient = new HttpClient();
            public InfoUID ResultInfo = new InfoUID();
            public GraphQLFacebook(string uid)
            {
                this.uid = uid;
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36"); //thêm user agent để lấy đc danh sách ảnh
                httpClient.DefaultRequestHeaders.Add("Cookie", "locale=vi_VN"); //đặt ngôn ngữ facebook thành ngôn ngữ Việt Nam
                httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin"); //thêm sec-fetch-site để lấy đc danh sách ảnh
            }
            public async Task GetUrlAvatar()
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("doc_id", "5341536295888250"),
                    new KeyValuePair<string, string>("variables", "{\"height\":500,\"scale\":1,\"userID\":\"" + uid + "\",\"width\":500}")
                });
                var response = await httpClient.PostAsync("https://www.facebook.com/api/graphql/", content);
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic json = JsonConvert.DeserializeObject(result); //parse json
                if (json.data.profile != null && json.data.profile.profile_picture != null)
                {
                    ResultInfo.UrlAvatar = (string)json.data.profile.profile_picture.uri;
                }
            }
            public async Task GetName()
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("q", "node(" + uid + "){name}")
                });
                var response = await httpClient.PostAsync("https://www.facebook.com/api/graphql/", content);
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic json = JsonConvert.DeserializeObject(result.Replace(uid, "info")); //parse json
                if (json.info != null && json.info.name != null)
                {
                    ResultInfo.Name = (string)json.info.name;
                }
            }
            public async Task GetBirthday()
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("q", "node(" + uid + "){birthday}")
                });
                var response = await httpClient.PostAsync("https://www.facebook.com/api/graphql/", content);
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic json = JsonConvert.DeserializeObject(result.Replace(uid, "info")); //parse json
                if (json.info != null && json.info.birthday != null)
                {
                    ResultInfo.Birthday = (string)json.info.birthday;
                }
            }
            public async Task GetEmail()
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("q", "node(" + uid + "){email}")
                });
                var response = await httpClient.PostAsync("https://www.facebook.com/api/graphql/", content);
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic json = JsonConvert.DeserializeObject(result.Replace(uid, "info")); //parse json
                if (json.info != null && json.info.email != null)
                {
                    ResultInfo.Email = (string)json.info.email;
                }
            }
            public async Task GetPhone()
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("q", "node(" + uid + "){phone}")
                });
                var response = await httpClient.PostAsync("https://www.facebook.com/api/graphql/", content);
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic json = JsonConvert.DeserializeObject(result.Replace(uid, "info")); //parse json
                if (json.info != null && json.info.phone != null)
                {
                    ResultInfo.Phone = (string)json.info.phone;
                }
            }
            public async Task GetFriend()
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("q", "node(" + uid + "){friends{count}}")
                });
                var response = await httpClient.PostAsync("https://www.facebook.com/api/graphql/", content);
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic json = JsonConvert.DeserializeObject(result.Replace(uid, "info")); //parse json
                if (json.info != null && json.info.friends != null)
                {
                    ResultInfo.Friend = (int)json.info.friends.count;
                }
            }
            public async Task GetFollow()
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("q", "node(" + uid + "){subscribers{count}}")
                });
                var response = await httpClient.PostAsync("https://www.facebook.com/api/graphql/", content);
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic json = JsonConvert.DeserializeObject(result.Replace(uid, "info")); //parse json
                if (json.info != null && json.info.subscribers != null)
                {
                    ResultInfo.Follow = (int)json.info.subscribers.count;
                }
            }
            public async Task GetCreatedTime()
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("q", "node(" + uid + "){created_time}")
                });
                var response = await httpClient.PostAsync("https://www.facebook.com/api/graphql/", content);
                string result = response.Content.ReadAsStringAsync().Result;
                dynamic json = JsonConvert.DeserializeObject(result.Replace(uid, "info")); //parse json
                if (json.info != null && json.info.created_time != null)
                {
                    ResultInfo.CreatedTime = DateTimeOffset.FromUnixTimeSeconds((int)json.info.created_time).DateTime.ToString();
                }
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Get Info")
            {
                button1.Text = "Stop";
                isStop = false;
                await Task.Run(async () =>
                {
                    string[] uids = textBox1.Lines;
                    for (var i = 0; i < uids.Length; i++)
                    {
                        if (isStop) return;
                        string uid = uids[i];
                        if (!string.IsNullOrEmpty(uid))
                        {
                            var fb = new GraphQLFacebook(uid);
                            await fb.GetName();
                            await fb.GetBirthday();
                            await fb.GetEmail();
                            await fb.GetPhone();
                            await fb.GetFollow();
                            await fb.GetFriend();
                            await fb.GetCreatedTime();
                            await fb.GetUrlAvatar();
                            int add = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[add];
                            row.Cells[0].Value = (row.Index + 1).ToString();
                            row.Cells[1].Value = uid;
                            row.Cells[2].Value = fb.ResultInfo.Name;
                            row.Cells[3].Value = fb.ResultInfo.UrlAvatar;
                            row.Cells[4].Value = fb.ResultInfo.Birthday;
                            row.Cells[5].Value = fb.ResultInfo.Email;
                            row.Cells[6].Value = fb.ResultInfo.Phone;
                            row.Cells[7].Value = fb.ResultInfo.Follow;
                            row.Cells[8].Value = fb.ResultInfo.Friend;
                            row.Cells[9].Value = fb.ResultInfo.CreatedTime;
                        }    
                    }
                    button1.Text = "Get Info";
                });
            }    
            else
            {
                button1.Text = "Get Info";
                isStop = true;
            }   
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string info = "";
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string data = "";
                    for (var i = 1; i < dataGridView1.Columns.Count; i++)
                    {
                        data += row.Cells[i].Value?.ToString() + "|";
                    }
                    data = data.Substring(0, data.Length - 1);
                    info += data + "\r\n";
                }
                File.WriteAllText(saveFileDialog.FileName.ToString(), info);
            }    
        }
    }
}

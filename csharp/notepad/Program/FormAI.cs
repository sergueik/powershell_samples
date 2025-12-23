using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Xml;
using System.IO;
using System.Drawing;

namespace Program {
    public partial class FormAI : Form {
        HttpClient httpClientYaGPT = new HttpClient();

        string URL;
        string urlModel;
        string apiKey;

        static Dictionary<string, string> promptRetelling = new Dictionary<string, string> {
            { "role", "system" },
            { "text", "Твоя задача - грамотно пересказать текст, не расширяяя его и не меняя смысл. Соблюдай все правила русского языка." } 
        };

        static Dictionary<string, string> promptSqueeze = new Dictionary<string, string> {
            { "role", "system" },
            { "text", "Твоя задача - сократить текст, сохранив его смысл. Текст должен быть примерно в 2 раза меньше исходного. Соблюдай все правила русского языка." } 
        };

        static Dictionary<string, string> promptExpand = new Dictionary<string, string> {
            { "role", "system" },
            { "text", "Твоя задача - расширить текст, сохранив его смысл. Текст должен быть примерно в 2 раза больше исходного. Соблюдай все правила русского языка." }
        };

        public class Alternative {
            public Message message { get; set; }
            public string status { get; set; }
        }

        public class Message {
            public string role { get; set; }
            public string text { get; set; }
        }

        public class Result {
            public List<Alternative> alternatives { get; set; }
            public Usage usage { get; set; }
            public string modelVersion { get; set; }
        }

        public class API_Result {
            public Result result { get; set; }
        }

        public class Usage {
            public string inputTextTokens { get; set; }
            public string completionTokens { get; set; }
            public string totalTokens { get; set; }
        }

        public FormAI(string text) {
            InitializeComponent();

            this.Icon = new Icon(FormMain.pathIconBlue);

            try {
                prepareSecretData();
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Ошибка");

                this.DialogResult = DialogResult.No;
                this.Close();
                return;
            }

            aiTextField.Text = text;

            httpClientYaGPT.DefaultRequestHeaders.Add(
                "Authorization", "Api-Key" + " " + apiKey
            );
            httpClientYaGPT.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        async void sendQueryYaGPT() {
            List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();

            if (aiSelectAction_Retelling.Checked) {
                messages.Add(promptRetelling);
            } else if (aiSelectAction_Squeeze.Checked) {
                messages.Add(promptSqueeze);
            } else if (aiSelectAction_Expand.Checked) {
                messages.Add(promptExpand); 
            } else {
                aiLabelError.Text = "Ошибка! Не выбрано действие";
                return;
            }

            messages.Add(
                new Dictionary <string, string> {
                    { "role", "user" },
                    { "text", aiTextField.Text }
                }
            );

            Dictionary<string, object> requestPrompt = new Dictionary<string, object> {
                { "modelUri", urlModel }, {
                    "completionOptions", new Dictionary<string, object> {
                        { "stream", false },
                        { "temperature", 0.3 },
                        { "maxTokens", "100" }
                    }
                },
                { "messages", messages }
            };

            string requestPromptJSON = JsonConvert.SerializeObject(requestPrompt);

            StringContent requestBody = new StringContent(requestPromptJSON, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = await httpClientYaGPT.PostAsync(URL, requestBody);

            try {
                response.EnsureSuccessStatusCode();
            } catch {
                aiLabelError.Text = String.Format("Ошибка {0}\n{1}", response.ReasonPhrase, response.Headers);
                return;
            }
            
            string responseString = await response.Content.ReadAsStringAsync();

            API_Result responseJSON = JsonConvert.DeserializeObject<API_Result>(responseString);

            aiTextField.Text = responseJSON.result.alternatives[0].message.text;
            aiLabelError.Text = "";
        }

        private void aiButtonSend_Click(object sender, EventArgs e) {
            if (aiSelectAI.Text == "YaGPT Lite") {
                sendQueryYaGPT();
            } else {
                aiLabelError.Text = "Ошибка! Не выбран AI";
                return;
            }
        }

        private void aiButtonApply_Click(object sender, EventArgs e) {
            if (aiTextField.Text.Length == 0) {
                aiLabelError.Text = "Ошибка! Нет текста";
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void aiButtonCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void prepareSecretData() {
            XmlDocument file = new XmlDocument();
            file.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "secrets.xml"));
            XmlNode node = file.DocumentElement;

            URL = node["YandexCloud"].InnerText;
            urlModel = node["UrlModel"].InnerText;
            apiKey = node["ApiKey"].InnerText;
        }
    }
}

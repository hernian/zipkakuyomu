using System.Diagnostics;
using System.Text;
using System.Web;

namespace zipkakuyomu
{
    public partial class FormMain : Form
    {
        private static readonly string[] DRAG_DATA_FORMATS = ["UniformResourceLocator", "UniformResourceLocatorW"];
        private const string HTML_COMPLETED = """
            <html>
            <head>
            <title>ZIPファイルへ一括保存</title>
            </head>
            <body>
            <p>完了しました</p>
            </body>
            </html>
            """;
        private const string HTML_CANCELED = """
            <html>
            <head>
            <title>ZIPファイルへ一括保存</title>
            </head>
            <body>
            <p>キャンセルされました</p>
            </body>
            </html>
            """;
        private readonly KakuYomuNavigater _navi;
        private CancellationTokenSource _ctsNavigation = new();
        public FormMain()
        {
            InitializeComponent();
            _navi = new KakuYomuNavigater(webView2);
            this.Enabled = false;
            this.webView2.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.textBoxZipFilename.Text = Properties.Settings.Default.LastZipDir;
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.buttonOK.Enabled = false;
                this.buttonCancel.Enabled = true;

                var urlTitlePage = textBoxUrl.Text;
                var pathZipFile = textBoxZipFilename.Text;
                if (string.IsNullOrEmpty(Path.GetExtension(pathZipFile)))
                {
                    pathZipFile += ".zip";
                    textBoxZipFilename.Text = pathZipFile;
                }
                Properties.Settings.Default.LastZipDir = Path.GetDirectoryName(pathZipFile);
                Properties.Settings.Default.Save();

                _ctsNavigation.Dispose();
                _ctsNavigation = new CancellationTokenSource();
                var zipBook = new ZipBook();
                await _navi.NavigateAsync(zipBook, urlTitlePage, _ctsNavigation.Token);
                await zipBook.SaveAsync(pathZipFile, _ctsNavigation.Token);
                webView2.NavigateToString(HTML_COMPLETED);
            }
            catch (OperationCanceledException)
            {
                webView2.NavigateToString(HTML_CANCELED);
            }
            catch (Exception exc)
            {
                var sb = new StringBuilder();
                for (var ex = exc; ex != null; ex = ex.InnerException)
                {
                    Debug.WriteLine(ex.Message);
                    sb.AppendLine(
                        """
                        <html>
                        <head>
                        <title>エラー</title>
                        </head>
                        <body>
                        """);
                    sb.Append("<p>");
                    sb.Append(HttpUtility.HtmlEncode(ex.Message));
                    sb.AppendLine("</p>");
                    sb.AppendLine(
                        """
                        </body>
                        </html>
                        """);
                }
                webView2.NavigateToString(sb.ToString());
            }
            finally
            {
                this.buttonOK.Enabled = true;
                this.buttonCancel.Enabled = false;
            }
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            await webView2.EnsureCoreWebView2Async();
            this.Enabled = true;
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            saveFileDialogZipFile.FileName = textBoxZipFilename.Text;
            if (saveFileDialogZipFile.ShowDialog(this) == DialogResult.OK)
            {
                textBoxZipFilename.Text = saveFileDialogZipFile.FileName;
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            _ctsNavigation.Cancel();
            buttonCancel.Enabled = false;
        }

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            var dataObj = e.Data;
            if (dataObj == null)
            {
                return;
            }
            foreach (var dataFormat in DRAG_DATA_FORMATS)
            {
                if (dataObj.GetDataPresent(dataFormat))
                {
                    e.Effect = DragDropEffects.Copy;
                    break;
                }
            }
        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            var dataObj = e.Data;
            if (dataObj == null)
            {
                return;
            }
            textBoxUrl.Text = dataObj.GetData(DataFormats.Text)?.ToString() ?? string.Empty;
        }
    }
}

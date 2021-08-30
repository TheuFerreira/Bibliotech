using System.Windows.Forms;

namespace ReadExcel.Services
{
    public class DialogService
    {
        public string OpenSheetDialog()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Selecione a Planilha",
                CheckFileExists = true,
                Multiselect = false
            };

            DialogResult result = openFile.ShowDialog();
            if (result != DialogResult.OK)
            {
                return string.Empty;
            }

            return openFile.FileName;
        }
    }
}

using Bibliotech.UserControls.CustomDialog;

namespace Bibliotech.Services
{
    public class DialogService
    {
        public void ShowError(string description)
        {
            _ = new InformationDialog("ERRO", description, TypeDialog.Error).ShowDialog();
        }

        public void ShowSuccess(string description)
        {
            _ = new InformationDialog("SUCESSO", description, TypeDialog.Success).ShowDialog();
        }

        public bool ShowQuestion(string title, string description)
        {
            QuestionDialog questionDialog = new QuestionDialog(title, description);
            _ = questionDialog.ShowDialog();

            return questionDialog.DialogResult.Value;
        }

        public void ShowInformation(string description)
        {
           _ = new InformationDialog("INFORMAÇÃO", description, TypeDialog.Information).ShowDialog();
        }
    }
}

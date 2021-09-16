﻿using Bibliotech.UserControls.CustomDialog;
using Bibliotech.UserControls.CustomEnums;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Forms;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

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

        public string ShowPasswordDialog(string description)
        {
            TextFieldDialog fieldDialog = new TextFieldDialog("REPETIR SENHA:", description, TypeTextFieldDialog.Password, FieldType.String, "****");
            bool? result = fieldDialog.ShowDialog();

            return result == false ? string.Empty : fieldDialog.Text;
        }

        public string ShowAddDialog(string description, string textFieldTitle)
        {
            TextFieldDialog fieldDialog = new TextFieldDialog(textFieldTitle, description, TypeTextFieldDialog.Add, FieldType.Number, "123456");
            bool? result = fieldDialog.ShowDialog();

            return result == false ? string.Empty : fieldDialog.Text;
        }

        public string ShowAddTextDialog(string description, string textFieldTitle)
        {
            TextFieldDialog fieldDialog = new TextFieldDialog(textFieldTitle, description, TypeTextFieldDialog.Add, FieldType.String, "@autor");
            bool? result = fieldDialog.ShowDialog();

            return result == false ? string.Empty : fieldDialog.Text;
        }

        public string SaveFileDialg()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                AddExtension = true,
                RestoreDirectory = true,
                Title = "Salvar",
                Filter = "PDF Files|*.pdf",
                DefaultExt = "pdf",
                FileName = "Código de Barras"
            };
           
            return saveFileDialog.FileName;
        }
    }
}

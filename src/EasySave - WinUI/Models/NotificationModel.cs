using Microsoft.UI.Xaml;

namespace EasySave___WinUI.Models {
    internal class NotificationModel {

        public string Title { get; set; }
        public string Content { get; set; }
        public string CloseButtonText { get; set; }
        public string PrimaryButtonText { get; set; }
        public XamlRoot XamlRoot { get; set; }

		public NotificationModel(string title, string content, string closeButtonText, string primaryButtonText, XamlRoot xamlRoot) { 
			Title = title;
			Content = content;
			CloseButtonText = closeButtonText;
			PrimaryButtonText = primaryButtonText;
			XamlRoot = xamlRoot;
		}
    }
}

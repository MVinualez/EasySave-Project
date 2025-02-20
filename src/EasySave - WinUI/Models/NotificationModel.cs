using Microsoft.UI.Xaml;

namespace EasySave___WinUI.Models {
    internal class NotificationModel {

		private string _title = string.Empty;
        private string _content = string.Empty;
        private string _closeButtonText = string.Empty;
        private string _primaryButtonText = string.Empty;
        private XamlRoot? _xamlRoot;


        // ------ Getters / Setters ------
        public string Title {
			get { return _title; }
			set { _title = value; }
		}

		public string Content {
			get { return _content; }
			set { _content = value; }
		}

		public string CloseButtonText {
			get { return _closeButtonText; }
			set { _closeButtonText = value; }
		}

		public string PrimaryButtonText {
			get { return _primaryButtonText; }
			set { _primaryButtonText = value; }
		}

		public XamlRoot? XamlRoot {
			get { return _xamlRoot; }
			set { _xamlRoot = value; }
		}
		// -------------------------------

		public NotificationModel(string title, string content, string closeButtonText, string primaryButtonText, XamlRoot xamlRoot) { 
			Title = title;
			Content = content;
			CloseButtonText = closeButtonText;
			PrimaryButtonText = primaryButtonText;
			XamlRoot = xamlRoot;
		}
    }
}

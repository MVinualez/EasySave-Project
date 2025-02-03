using System;
using System.Collections.Generic;
namespace easysave_project.Models {
    internal class MenuAction {

        public string name { get; set; }
        public Action execute { get; set; }

        public MenuAction(string name, Action execute) {
            this.name = name;
            this.execute = execute;
        }
    }
}
